using Bonsai;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using Bonsai.Vision;
using OpenCV.Net;

[Combinator]
[Description("Takes the N-largest binary regions.")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class TakeLargestRegions
{
    [Description("The number of largest binary regions to take.")]
    public int Count { get; set; }

    public IObservable<ConnectedComponentCollection> Process(IObservable<ConnectedComponentCollection> source)
    {
        return source.Select(value =>
        {
            var regionCount = Count;
            var largestRegions = value.OrderByDescending(x => x.Area).Take(regionCount).ToList();
            while (largestRegions.Count < regionCount)
            {
                var missingRegion = new ConnectedComponent();
                missingRegion.Centroid = new Point2f(float.NaN, float.NaN);
                missingRegion.Orientation = double.NaN;
                largestRegions.Add(missingRegion);
            }
            return new ConnectedComponentCollection(largestRegions, value.ImageSize);
        });
    }
}
