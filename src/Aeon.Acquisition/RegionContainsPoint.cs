using Bonsai;
using Bonsai.Harp;
using Bonsai.Vision;
using OpenCV.Net;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace Aeon.Acquisition
{
    [Combinator]
    [Description("Generates boolean values indicating whether each point in the sequence is inside a region of interest.")]
    public class RegionContainsPoint
    {
        readonly ContainsPoint containsPoint = new ContainsPoint();

        [Description("The array of vertices specifying the region of interest.")]
        [Editor("Bonsai.Vision.Design.IplImageInputRoiEditor, Bonsai.Vision.Design", DesignTypes.UITypeEditor)]
        public Point[][] Regions { get; set; }
        
        public IObservable<Timestamped<bool>> Process(IObservable<Tuple<ConnectedComponent, double>> source)
        {
            return source.Publish(ps => ps.Zip(
                containsPoint.Process(ps.Select(x => Tuple.Create(x.Item1.Centroid, Regions))),
                (time, result) => Timestamped.Create(result, time.Item2)));
        }
    }
}
