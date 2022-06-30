using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace Aeon.Acquisition
{
    static class ObservableExtensions
    {
        public static IObservable<TSource> FillGaps<TSource>(this IObservable<TSource> source, Func<TSource, TSource, int> gapSelector)
        {
            return Observable.Create<TSource>(observer =>
            {
                bool hasPrevious = false;
                TSource previous = default;
                var gapObserver = Observer.Create<TSource>(value =>
                {
                    if (hasPrevious)
                    {
                        var missing = gapSelector(previous, value);
                        if (missing < 0)
                        {
                            observer.OnError(new InvalidOperationException("Negative gap sizes are not allowed."));
                        }

                        while (missing > 0)
                        {
                            observer.OnNext(default);
                            missing--;
                        }
                    }
                    observer.OnNext(value);
                    previous = value;
                    hasPrevious = true;
                });
                return source.SubscribeSafe(gapObserver);
            });
        }

        public static IObservable<Unit> MergeUnit<TSource, TOther>(this IObservable<TSource> source, IObservable<TOther> other)
        {
            return source.Select(x => Unit.Default).Merge(other.Select(x => Unit.Default));
        }
    }
}
