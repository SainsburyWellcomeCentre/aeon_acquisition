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
            ChunkSize = 1;
        }

        // The default real-time reference is unix time in total seconds from 1904
        static readonly DateTime ReferenceTime = new DateTime(1904, 1, 1);

        [Description("The size of each chunk, in whole hours.")]
        public int ChunkSize { get; set; }

        DateTime GetChunkIndex(double seconds)
        {
            var currentTime = ReferenceTime.AddSeconds(seconds);
            var timeBin = currentTime.Hour / ChunkSize;
            return currentTime.Date.AddHours(timeBin * ChunkSize);
        }

        public IObservable<IGroupedObservable<DateTime, Timestamped<TSource>>> Process<TSource>(IObservable<Timestamped<TSource>> source)
        {
            return source.GroupBy(value => GetChunkIndex(value.Seconds));
        }

        public IObservable<IGroupedObservable<DateTime, HarpMessage>> Process(IObservable<HarpMessage> source)
        {
            return source.GroupBy(value => GetChunkIndex(value.GetTimestamp()));
        }

        public IObservable<IGroupedObservable<DateTime, Timestamped<TSource>>> Process<TSource>(IObservable<Tuple<TSource, double>> source)
        {
            return source.GroupBy(value => GetChunkIndex(value.Item2), value => Timestamped.Create(value.Item1, value.Item2));
        }
    }
}