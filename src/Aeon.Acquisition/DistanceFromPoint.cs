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
    [Description("Computes the distance from every component in the sequence to a point.")]
    public class DistanceFromPoint
    {
        [Description("Specifies the point used to calculate the distance from.")]
        [Editor("Bonsai.Vision.Design.IplImageInputRoiEditor, Bonsai.Vision.Design", DesignTypes.UITypeEditor)]
        public Point Value { get; set; }

        static double Distance(Point2f left, Point2f right)
        {
            var delta = right - left;
            return Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y);
        }

        public IObservable<Timestamped<double>> Process(IObservable<Tuple<ConnectedComponent, double>> source)
        {
            return source.Select(x =>
            {
                var distance = Distance(x.Item1.Centroid, new Point2f(Value));
                return Timestamped.Create(distance, x.Item2);
            });
        }

        public IObservable<Timestamped<double>> Process(IObservable<Tuple<ConnectedComponentCollection, double>> source)
        {
            return source.Select(x =>
            {
                var point = new Point2f(Value);
                var distance = x.Item1.Min(component => Distance(point, component.Centroid));
                return Timestamped.Create(distance, x.Item2);
            });
        }
    }
}
