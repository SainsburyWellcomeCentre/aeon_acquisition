using Bonsai;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using Bonsai.Vision;
using Bonsai.Harp;

[Combinator]
[Description("Packs a timestamped binary region onto a Harp message.")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class FormatBinaryRegion
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
}
