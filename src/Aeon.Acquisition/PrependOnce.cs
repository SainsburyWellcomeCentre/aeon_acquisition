using Bonsai;
using System;
using System.ComponentModel;
using System.Reactive.Linq;

namespace Aeon.Acquisition
{
    [Combinator]
    [WorkflowElementCategory(ElementCategory.Combinator)]
    [Description("Subscribes only once to the auxiliary sequence and returns the stored value before publishing the main sequence.")]
    public class PrependOnce
    {
        public IObservable<TSource> Process<TSource>(IObservable<TSource> source, IObservable<TSource> other)
        {
            var first = true;
            return Observable.Create<TSource>(async (observer, cancellationToken) =>
            {
                if (first)
                {
                    var value = await other;
                    observer.OnNext(value);
                    first = false;
                }

                return source.SubscribeSafe(observer);
            });
        }
    }
}
