using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Bonsai.Harp;

namespace Aeon.Acquisition
{
    internal static class ObservableExtensions
    {
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
                var sourceSubscription = new SingleAssignmentDisposable();
                var trigger = Observer.Create<HarpMessage>(
                    _ => sourceSubscription.Disposable = ps.Connect(),
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
                    sourceSubscription,
                    pc.Connect());
            });
        }
    }
}
