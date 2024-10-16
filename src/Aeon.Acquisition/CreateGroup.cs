using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using Bonsai;
using Bonsai.Expressions;

namespace Aeon.Acquisition
{
    [Description("Creates a sequence of grouped observables for each key in the input sequence.")]
    public class CreateGroup : WorkflowExpressionBuilder
    {
        static readonly Range<int> argumentRange = Range.Create(lowerBound: 1, upperBound: 1);

        public CreateGroup()
            : this(new ExpressionBuilderGraph())
        {
        }

        public CreateGroup(ExpressionBuilderGraph workflow)
            : base(workflow)
        {
        }

        public override Range<int> ArgumentRange => argumentRange;

        public override Expression Build(IEnumerable<Expression> arguments)
        {
            var source = arguments.FirstOrDefault();
            if (source == null)
            {
                throw new InvalidOperationException("There must be at least one input to the create group operator.");
            }

            Type keyType, elementType;
            var sourceType = source.Type.GetGenericArguments()[0];
            var selectorParameter = Expression.Parameter(source.Type);
            if (sourceType.IsGenericType && sourceType.GetGenericTypeDefinition() == typeof(IGroupedObservable<,>))
            {
                var sourceTypeArguments = sourceType.GetGenericArguments();
                keyType = sourceTypeArguments[0];
                elementType = sourceTypeArguments[1];
            }
            else
            {
                keyType = sourceType;
                elementType = null;
            }

            return BuildWorkflow(new[] { selectorParameter }, null, selectorBody =>
            {
                var selector = Expression.Lambda(selectorBody, selectorParameter);
                var resultType = selectorBody.Type.GetGenericArguments()[0];

                return Expression.Call(
                    typeof(CreateGroup),
                    nameof(Process),
                    elementType != null
                        ? new[] { keyType, elementType, resultType }
                        : new[] { keyType, resultType },
                    source, selector);
            });
        }

        static IObservable<IGroupedObservable<TKey, TResult>> Process<TKey, TResult>(
            IObservable<TKey> source,
            Func<IObservable<TKey>, IObservable<TResult>> selector)
        {
            return source.Select(key => new GroupedObservable<TKey, TResult>(
                key,
                selector(Observable.Return(key))));
        }

        static IObservable<IGroupedObservable<TKey, TResult>> Process<TKey, TSource, TResult>(
            IObservable<IGroupedObservable<TKey, TSource>> source,
            Func<IObservable<IGroupedObservable<TKey, TSource>>, IObservable<TResult>> selector)
        {
            return source.Select(group => new GroupedObservable<TKey, TResult>(
                group.Key,
                selector(Observable.Return(group))));
        }

        class GroupedObservable<TKey, TElement> : IGroupedObservable<TKey, TElement>
        {
            public GroupedObservable(TKey key, IObservable<TElement> source)
            {
                if (source is null)
                {
                    throw new ArgumentNullException(nameof(source));
                }

                Key = key;
                Source = source.Publish().RefCount();
            }

            public TKey Key { get; }

            private IObservable<TElement> Source { get; }

            public IDisposable Subscribe(IObserver<TElement> observer)
            {
                return Source.Subscribe(observer);
            }
        }
    }
}
