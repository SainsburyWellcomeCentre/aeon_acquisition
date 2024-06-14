using Bonsai.Expressions;
using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Aeon.Acquisition
{
    [Description("A subject that persists its state across different executions of the same workflow.")]
    public class StateRecoverySubject : SubjectBuilder
    {
        protected override Expression BuildSubject(Expression expression)
        {
            var builderExpression = Expression.Constant(this);
            var parameterType = expression.Type.GetGenericArguments()[0];
            return Expression.Call(builderExpression, nameof(CreateSubject), new[] { parameterType });
        }

        StateRecoverySubject<TSource>.RecoverySubject CreateSubject<TSource>() where TSource : new()
        {
            return new StateRecoverySubject<TSource>.RecoverySubject(Name);
        }
    }

    [DisplayName(nameof(StateRecoverySubject))]
    [Description("A subject that persists its state across different executions of the same workflow.")]
    public class StateRecoverySubject<T> : SubjectBuilder<T> where T : new()
    {
        protected override ISubject<T> CreateSubject()
        {
            return new RecoverySubject(Name);
        }

        internal class RecoverySubject : ISubject<T>, IDisposable
        {
            readonly BehaviorSubject<T> subject;

            public RecoverySubject(string name)
            {
                var state = StateRecovery<T>.Deserialize(name);
                subject = new BehaviorSubject<T>(state);
                subject.Skip(1).Subscribe(value => StateRecovery<T>.Serialize(name, value));
            }

            public void OnCompleted()
            {
            }

            public void OnError(Exception error)
            {
            }

            public void OnNext(T value)
            {
                subject.OnNext(value);
            }

            public IDisposable Subscribe(IObserver<T> observer)
            {
                return subject.Subscribe(observer);
            }

            public void Dispose()
            {
                subject.Dispose();
            }
        }
    }
}
