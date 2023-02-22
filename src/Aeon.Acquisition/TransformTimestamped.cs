using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using Bonsai;
using Bonsai.Expressions;
using Bonsai.Harp;

namespace Aeon.Acquisition
{
    [DefaultProperty(nameof(IncludeTimestamp))]
    [Description("Applies a transformation to the elements of an observable sequence using the encapsulated workflow and preserves the timestamp of each element.")]
    public class TransformTimestamped : WorkflowExpressionBuilder
    {
        static readonly Range<int> argumentRange = Range.Create(lowerBound: 1, upperBound: 1);

        public TransformTimestamped()
        {
        }

        public TransformTimestamped(ExpressionBuilderGraph workflow)
            : base(workflow)
        {
        }

        public override Range<int> ArgumentRange => argumentRange;

        public bool IncludeTimestamp { get; set; }

        public override Expression Build(IEnumerable<Expression> arguments)
        {
            var source = arguments.FirstOrDefault();
            if (source == null)
            {
                throw new InvalidOperationException("There must be at least one input to the transform workflow.");
            }

            var parameterType = source.Type.GetGenericArguments()[0];
            if (!parameterType.IsGenericType || parameterType.GetGenericTypeDefinition() != typeof(Timestamped<>))
            {
                throw new InvalidOperationException("The input to the transform workflow must be Harp timestamped.");
            }

            // Assign input
            var timestampedType = parameterType.GetGenericArguments()[0];
            var selectorParameter = Expression.Parameter(IncludeTimestamp ? source.Type : typeof(IObservable<>).MakeGenericType(timestampedType));
            return BuildWorkflow(arguments, selectorParameter, selectorBody =>
            {
                var selector = Expression.Lambda(selectorBody, selectorParameter);
                var selectorObservableType = selector.ReturnType.GetGenericArguments()[0];
                return Expression.Call(GetType(), nameof(Process), new[] { timestampedType, selectorObservableType }, source, selector);
            });
        }

        static IObservable<Timestamped<TResult>> Process<TSource, TResult>(IObservable<Timestamped<TSource>> source, Func<IObservable<Timestamped<TSource>>, IObservable<TResult>> selector)
        {
            return source.Publish(ps => ps
                .Select(value => value.Seconds)
                .Zip(selector(ps), (seconds, value) => Timestamped.Create(value, seconds)));
        }

        static IObservable<Timestamped<TResult>> Process<TSource, TResult>(IObservable<Timestamped<TSource>> source, Func<IObservable<TSource>, IObservable<TResult>> selector)
        {
            return source.Publish(ps => ps
                .Select(input => input.Seconds)
                .Zip(selector(ps.Select(input => input.Value)), (seconds, value) => Timestamped.Create(value, seconds)));
        }
    }
}
