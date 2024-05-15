using Bonsai;
using Bonsai.Harp;
using Bonsai.Vision;
using OpenCV.Net;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace Aeon.Vision
{
    [Combinator]
    [Description("Generates boolean values indicating whether each point in the sequence is inside a region of interest.")]
    public class RegionContainsPoint
    {
        [Description("The array of vertices specifying the region of interest.")]
        [Editor("Bonsai.Vision.Design.IplImageInputRoiEditor, Bonsai.Vision.Design", DesignTypes.UITypeEditor)]
        public Point[][] Regions { get; set; }

        static bool Contains(Point[][] contour, Point2f point)
        {
            if (contour == null) return false;
            for (int i = 0; i < contour.Length; i++)
            {
                using (var contourHeader = Mat.CreateMatHeader(contour[i], contour[i].Length, 2, Depth.S32, 1))
                {
                    if (CV.PointPolygonTest(contourHeader, point, false) > 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public IObservable<Timestamped<bool>> Process(IObservable<Timestamped<Point2f>> source)
        {
            return source.Select(x =>
            {
                var containsPoint = Contains(Regions, x.Value);
                return Timestamped.Create(containsPoint, x.Seconds);
            });
        }

        public IObservable<Timestamped<bool>> Process(IObservable<Timestamped<ConnectedComponent>> source)
        {
            return source.Select(x =>
            {
                var containsPoint = Contains(Regions, x.Value.Centroid);
                return Timestamped.Create(containsPoint, x.Seconds);
            });
        }

        public IObservable<Timestamped<bool>> Process(IObservable<Timestamped<ConnectedComponentCollection>> source)
        {
            return source.Select(x =>
            {
                var regions = Regions;
                var containsPoint = x.Value.Any(component => Contains(regions, component.Centroid));
                return Timestamped.Create(containsPoint, x.Seconds);
            });
        }
    }
}
