using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Bonsai.Harp;

namespace Aeon.Acquisition
{
    static class ObservableExtensions
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
                            observer.OnError(new InvalidOperationException("Negative gap sizes are not allowed."));
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

        public static IObservable<Unit> MergeUnit<TSource, TOther>(this IObservable<TSource> source, IObservable<TOther> other)
        {
            return source.Select(x => Unit.Default).Merge(other.Select(x => Unit.Default));
        }

        public static IObservable<Bonsai.Harp.Timestamped<TSource>> Timestamp<TSource>(this IObservable<TSource> source, IObservable<HarpMessage> clock)
        {
            return clock.Publish(
                pc => source.Publish(
                    ps => ps.CombineLatest(pc, (data, tick) => (data, tick))
                            .Sample(ps.MergeUnit(pc.Take(1)))
                            .Select(x =>
                            {
                                var timestamp = x.tick.GetTimestamp();
                                return Bonsai.Harp.Timestamped.Create(x.data, timestamp);
                            })));
        }
    }
}
