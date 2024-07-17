using System;
using System.Reactive;
using System.Reactive.Linq;

namespace Aeon.Video
{
    internal static class ObservableExtensions
    {
        public static IObservable<TSource> FillGaps<TSource>(this IObservable<TSource> source, Func<TSource, TSource, int> gapSelector)
        {
            return FillGaps(source, value => value, gapSelector);
        }

        public static IObservable<TSource> FillGaps<TSource, TCounter>(
            this IObservable<TSource> source,
            Func<TSource, TCounter> counterSelector,
            Func<TCounter, TCounter, int> gapSelector)
        {
            return Observable.Create<TSource>(observer =>
            {
                bool hasPrevious = false;
                TCounter previousCounter = default;
                var gapObserver = Observer.Create<TSource>(value =>
                {
                    var counter = counterSelector(value);
                    if (hasPrevious)
                    {
                        var missing = gapSelector(previousCounter, counter);
                        if (missing < 0)
                        {
                            observer.OnError(new InvalidOperationException(
                                $"Negative gap sizes are not allowed.\n  Previous counter: {previousCounter}\n  Current counter: {counter}"));
                        }

                        while (missing > 0)
                        {
                            observer.OnNext(default);
                            missing--;
                        }
                    }
                    observer.OnNext(value);
                    previousCounter = counter;
                    hasPrevious = true;
                });
                return source.SubscribeSafe(gapObserver);
            });
        }
    }
}
