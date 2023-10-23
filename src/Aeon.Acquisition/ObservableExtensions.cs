using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Bonsai.Harp;

namespace Aeon.Acquisition
{
    public static class ObservableExtensions
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

        public static IObservable<Unit> MergeUnit<TSource, TOther>(this IObservable<TSource> source, IObservable<TOther> other)
        {
            return source.Select(x => Unit.Default).Merge(other.Select(x => Unit.Default));
        }

        public static IObservable<Bonsai.Harp.Timestamped<TSource>> Timestamp<TSource>(this IObservable<TSource> source, IObservable<HarpMessage> clock)
        {
            return Observable.Create<Bonsai.Harp.Timestamped<TSource>>(observer =>
            {
                var pc = clock.Publish();
                var ps = source.Publish();
                var trigger = Observer.Create<HarpMessage>(
                    _ => ps.Connect(),
                    observer.OnError);
                var result = ps.CombineLatest(pc, (data, message) => (data, message))
                               .Sample(ps.MergeUnit(pc.Take(1)))
                               .Select(x =>
                               {
                                   var timestamp = x.message.GetTimestamp();
                                   return Bonsai.Harp.Timestamped.Create(x.data, timestamp);
                               });
                return new CompositeDisposable(
                    result.SubscribeSafe(observer),
                    pc.Take(1).SubscribeSafe(trigger),
                    pc.Connect());
            });
        }
    }
}
