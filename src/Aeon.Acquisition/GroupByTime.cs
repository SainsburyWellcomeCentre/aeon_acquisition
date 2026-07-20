using Bonsai;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using Bonsai.Harp;
using System.Xml.Serialization;
using System.Xml;

namespace Aeon.Acquisition
{
    /// <summary>
    /// Represents an operator that groups Harp time-series in whole hour chunks of fixed size.
    /// </summary>
    /// <remarks>
    /// Each chunk group stays open until the sequence terminates unless <see cref="ClosingDuration"/>
    /// is set. When it is set, a chunk closes once the clock advances past the end of the chunk plus
    /// that duration, with a heartbeat source driving the clock when one is provided and the incoming
    /// timestamps driving it otherwise. A grace of about two seconds suits the Harp synchronization
    /// model, where sync pulses land on the whole-second boundary and a device may step backward around
    /// a boundary but never beyond one sync period, so the grace keeps such a step from falling into an
    /// already closed chunk. A larger backward jump, such as connecting a new synchronizer, exceeds the
    /// grace and reopens the closed chunk as a new group. The chunk size and closing duration are read
    /// once when the grouped sequence is created, so a property mapping applied while the sequence is
    /// running does not resize open chunks.
    /// </remarks>
    [Combinator]
    [Description("Groups Harp time-series in whole hour chunks of fixed size.")]
    [WorkflowElementCategory(ElementCategory.Combinator)]
    public class GroupByTime
    {
        public GroupByTime()
        {
            ChunkSize = 1;
        }

        // The default real-time reference is universal unix time in total seconds from 1904
        internal static readonly DateTime ReferenceTime = new(1904, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        [Description("The size of each chunk, in whole hours.")]
        public int ChunkSize { get; set; }

        [XmlIgnore]
        [Description("The relative time at which each group will close following the end of the chunk.")]
        public TimeSpan? ClosingDuration { get; set; }

        [Browsable(false)]
        [XmlElement(nameof(ClosingDuration), IsNullable = true)]
        public string ClosingDurationXml
        {
            get
            {
                var timeShift = ClosingDuration;
                if (timeShift.HasValue) return XmlConvert.ToString(timeShift.GetValueOrDefault());
                else return null;
            }
            set
            {
                if (!string.IsNullOrEmpty(value)) ClosingDuration = XmlConvert.ToTimeSpan(value);
                else ClosingDuration = null;
            }
        }

        static DateTime GetChunkIndex(double seconds, int chunkSize)
        {
            var currentTime = ReferenceTime.AddSeconds(seconds);
            var timeBin = currentTime.Hour / chunkSize;
            return currentTime.Date.AddHours(timeBin * chunkSize);
        }

        static bool ShouldCloseChunk(DateTime chunkKey, double seconds, int chunkSize, TimeSpan closingDuration)
        {
            var elementTimestamp = ReferenceTime.AddSeconds(seconds);
            var elementDelta = elementTimestamp - chunkKey;
            return elementDelta > new TimeSpan(chunkSize, 0, 0) + closingDuration;
        }

        IObservable<IGroupedObservable<DateTime, TResult>> Process<TSource, TResult>(
            IObservable<TSource> source,
            Func<TSource, double> timeSelector,
            Func<TSource, TResult> resultSelector,
            IObservable<HarpMessage> heartbeats)
        {
            var chunkSize = ChunkSize;
            var closingDuration = ClosingDuration;

            DateTime keySelector(TSource value) => GetChunkIndex(timeSelector(value), chunkSize);
            if (!closingDuration.HasValue)
                return source.GroupBy(keySelector, resultSelector);

            IObservable<IGroupedObservable<DateTime, TResult>> GroupByUntilChunkCloses(IObservable<TSource> source, IObservable<double> clock)
                => source.GroupByUntil(keySelector, resultSelector,
                    chunk => clock.FirstOrDefaultAsync(seconds => ShouldCloseChunk(chunk.Key, seconds, chunkSize, closingDuration.GetValueOrDefault())));

            return heartbeats != null
                ? GroupByUntilChunkCloses(source, heartbeats.Select(message => message.GetTimestamp()))
                : source.Publish(shared => GroupByUntilChunkCloses(shared, shared.Select(timeSelector)));
        }

        public IObservable<IGroupedObservable<DateTime, Timestamped<TSource>>> Process<TSource>(IObservable<Timestamped<TSource>> source)
            => Process(source, value => value.Seconds, value => value, heartbeats: null);

        public IObservable<IGroupedObservable<DateTime, HarpMessage>> Process(IObservable<HarpMessage> source)
            => Process(source, value => value.GetTimestamp(), value => value, heartbeats: null);

        public IObservable<IGroupedObservable<DateTime, Timestamped<TSource>>> Process<TSource>(IObservable<Tuple<TSource, double>> source)
            => Process(source, value => value.Item2, value => Timestamped.Create(value.Item1, value.Item2), heartbeats: null);

        public IObservable<IGroupedObservable<DateTime, Timestamped<TSource>>> Process<TSource>(
            IObservable<Timestamped<TSource>> source,
            IObservable<HarpMessage> heartbeats)
            => Process(source, value => value.Seconds, value => value, heartbeats);

        public IObservable<IGroupedObservable<DateTime, HarpMessage>> Process(
            IObservable<HarpMessage> source,
            IObservable<HarpMessage> heartbeats)
            => Process(source, value => value.GetTimestamp(), value => value, heartbeats);

        public IObservable<IGroupedObservable<DateTime, Timestamped<TSource>>> Process<TSource>(
            IObservable<Tuple<TSource, double>> source,
            IObservable<HarpMessage> heartbeats)
            => Process(source, value => value.Item2, value => Timestamped.Create(value.Item1, value.Item2), heartbeats);
    }
}
