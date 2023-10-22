using Bonsai;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using Bonsai.Vision;
using Bonsai.Harp;

namespace Aeon.Acquisition
{
    [Combinator]
    [Description("Converts timestamped binary regions into a sequence of Harp messages.")]
    [WorkflowElementCategory(ElementCategory.Transform)]
    public class FormatBinaryRegions
    {
        const int Address = 200;

        public IObservable<HarpMessage> Process(IObservable<Timestamped<ConnectedComponent>> source)
        {
            return source.Select(x =>
            {
                var region = x.Value;
                var timestamp = x.Seconds;
                return HarpMessage.FromSingle(
                    Address,
                    timestamp,
                    MessageType.Event,
                    region.Centroid.X,
                    region.Centroid.Y,
                    (float)region.Orientation,
                    (float)region.MajorAxisLength,
                    (float)region.MinorAxisLength,
                    (float)region.Area);
            });
        }

        public IObservable<HarpMessage> Process(IObservable<Timestamped<ConnectedComponentCollection>> source)
        {
            return source.SelectMany(x =>
            {
                var regions = x.Value;
                var timestamp = x.Seconds;
                return regions.Select((region, index) => HarpMessage.FromSingle(
                    Address,
                    timestamp,
                    MessageType.Event,
                    region.Centroid.X,
                    region.Centroid.Y,
                    (float)region.Orientation,
                    (float)region.MajorAxisLength,
                    (float)region.MinorAxisLength,
                    (float)region.Area,
                    index));
            });
        }
    }
}
