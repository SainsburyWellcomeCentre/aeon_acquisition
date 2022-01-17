using Bonsai;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using Bonsai.Vision;
using Bonsai.Harp;

[Combinator]
[Description("Converts timestamped binary regions into a sequence of Harp messages.")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class FormatBinaryRegions
{
    const int Address = 200;

    public IObservable<HarpMessage> Process(IObservable<Tuple<ConnectedComponent, double>> source)
    {
        return source.Select(value =>
        {
            var region = value.Item1;
            var timestamp = value.Item2;
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

    public IObservable<HarpMessage> Process(IObservable<Tuple<ConnectedComponentCollection, double>> source)
    {
        return source.SelectMany(value =>
        {
            var regions = value.Item1;
            var timestamp = value.Item2;
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
