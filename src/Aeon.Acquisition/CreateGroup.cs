using Bonsai;
using Bonsai.Expressions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Aeon.Acquisition
{
    [Description("Creates a sequence of grouped observables for each key in the input sequence.")]
    public sealed class CreateGroup : WorkflowExpressionBuilder
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
            return Observable.Create<IGroupedObservable<TKey, TResult>>(observer =>
            {
                var outerDisposable = new SingleAssignmentDisposable();
                var groupDisposable = new CompositeDisposable(outerDisposable);

                var keyObserver = Observer.Create<TKey>(
                    key =>
                    {
                        var innerObservable = new GroupedObservable<TKey, TResult>(key, selector(Observable.Return(key)));
                        observer.OnNext(innerObservable);

                        var innerDisposable = new SingleAssignmentDisposable();
                        var innerObserver = Observer.Create<TResult>(
                            _ => { },
                            observer.OnError,
                            () =>
                            {
                                groupDisposable.Remove(innerDisposable);
                                if (outerDisposable.IsDisposed && groupDisposable.Count == 0)
                                {
                                    observer.OnCompleted();
                                }
                            });

                        innerObservable.SubscribeSafe(innerObserver);
                        innerDisposable.Disposable = innerObservable.Source.Connect();
                        groupDisposable.Add(innerDisposable);
                    },
                    observer.OnError,
                    () =>
                    {
                        groupDisposable.Remove(outerDisposable);
                        if (groupDisposable.Count == 0)
                        {
                            observer.OnCompleted();
                        }
                    });

                outerDisposable.Disposable = source.SubscribeSafe(keyObserver);
                return groupDisposable;
            });
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
                Key = key;
                if (source is null)
                    throw new ArgumentNullException(nameof(source));

                Source = source.Multicast(new ReplaySubject<TElement>(1));
            }

            public TKey Key { get; private set; }

            public IConnectableObservable<TElement> Source { get; set; }

            public IDisposable Subscribe(IObserver<TElement> observer)
            {
                return Source.Subscribe(observer);
            }
        }
    }
}
