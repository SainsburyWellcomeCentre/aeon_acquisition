using Bonsai;
using Bonsai.Expressions;
using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive.Subjects;
using System.Xml;
using System.Xml.Serialization;

namespace Aeon.Acquisition
{
    [Description("A subject that persists its state across different executions of the same workflow.")]
    public class StateRecoverySubject : SubjectBuilder
    {
        [XmlIgnore]
        [Description("The minimum time between persistent state updates to disk.")]
        public TimeSpan MinUpdatePeriod { get; set; }

        [Browsable(false)]
        [XmlElement(nameof(MinUpdatePeriod))]
        public string MinUpdatePeriodXml
        {
            get { return XmlConvert.ToString(MinUpdatePeriod); }
            set { MinUpdatePeriod = XmlConvert.ToTimeSpan(value); }
        }

        protected override Expression BuildSubject(Expression expression)
        {
            var builderExpression = Expression.Constant(this);
            var parameterType = expression.Type.GetGenericArguments()[0];
            return Expression.Call(builderExpression, nameof(CreateSubject), new[] { parameterType });
        }

        StateRecoverySubject<TSource>.RecoverySubject CreateSubject<TSource>() where TSource : new()
        {
            return new StateRecoverySubject<TSource>.RecoverySubject(Name, MinUpdatePeriod);
        }
    }

    [DisplayName(nameof(StateRecoverySubject))]
    [Description("A subject that persists its state across different executions of the same workflow.")]
    public class StateRecoverySubject<T> : SubjectBuilder<T> where T : new()
    {
        [XmlIgnore]
        [Description("The minimum time between persistent state updates to disk.")]
        public TimeSpan MinUpdatePeriod { get; set; }

        [Browsable(false)]
        [XmlElement(nameof(MinUpdatePeriod))]
        public string MinUpdatePeriodXml
        {
            get { return XmlConvert.ToString(MinUpdatePeriod); }
            set { MinUpdatePeriod = XmlConvert.ToTimeSpan(value); }
        }

        protected override ISubject<T> CreateSubject()
        {
            return new RecoverySubject(Name, MinUpdatePeriod);
        }

        internal class RecoverySubject : ISubject<T>, IDisposable
        {
            readonly BehaviorSubject<T> subject;
            DateTimeOffset nextWriteTime;

            public RecoverySubject(string name, TimeSpan minUpdatePeriod)
            {
                var state = StateRecovery<T>.Deserialize(name);
                nextWriteTime = HighResolutionScheduler.Now + minUpdatePeriod;
                subject = new BehaviorSubject<T>(state);
                MinUpdatePeriod = minUpdatePeriod;
                Name = name;
            }

            string Name { get; }

            TimeSpan MinUpdatePeriod { get; }

            public void OnCompleted()
            {
            }

            public void OnError(Exception error)
            {
            }

            public void OnNext(T value)
            {
                subject.OnNext(value);
                var writeTime = HighResolutionScheduler.Now;
                if (writeTime > nextWriteTime)
                {
                    StateRecovery<T>.Serialize(Name, value);
                    nextWriteTime = writeTime + MinUpdatePeriod;
                }
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
