using Bonsai;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using Bonsai.Harp;

namespace Aeon.Acquisition
{
    [Combinator]
    [Description("Groups Harp time-series in whole hour chunks of fixed size.")]
    [WorkflowElementCategory(ElementCategory.Combinator)]
    public class GroupByTime
    {
        public GroupByTime()
        {
            BinSize = 1;
        }

        // The default real-time reference is unix time in total seconds from 1904
        static readonly DateTime ReferenceTime = new DateTime(1904, 1, 1);

        [Description("The size of each time bin, in whole hours.")]
        public int BinSize { get; set; }

        DateTime GetTimeBin(double seconds)
        {
            var currentTime = ReferenceTime.AddSeconds(seconds);
            var timeBin = currentTime.Hour / BinSize;
            return currentTime.Date.AddHours(timeBin * BinSize);
        }

        public IObservable<IGroupedObservable<DateTime, Timestamped<TSource>>> Process<TSource>(IObservable<Timestamped<TSource>> source)
        {
            return source.GroupBy(value => GetTimeBin(value.Seconds));
        }

        public IObservable<IGroupedObservable<DateTime, HarpMessage>> Process(IObservable<HarpMessage> source)
        {
            return source.GroupBy(value => GetTimeBin(value.GetTimestamp()));
        }

        public IObservable<IGroupedObservable<DateTime, Timestamped<TSource>>> Process<TSource>(IObservable<Tuple<TSource, double>> source)
        {
            return source.GroupBy(value => GetTimeBin(value.Item2), value => Timestamped.Create(value.Item1, value.Item2));
        }
    }
}