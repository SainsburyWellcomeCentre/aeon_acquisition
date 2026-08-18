using Bonsai;
using Bonsai.Vision;
using OpenCV.Net;
using System;
using System.ComponentModel;
using System.Reactive.Linq;

namespace Aeon.Tether.Translation
{
    [Combinator]
    [Description("Calculates the optimal horizontal position of the linear drive for maintaining tether slack, where zero corresponds to the tether fully extended and one to fully retracted.")]
    [WorkflowElementCategory(ElementCategory.Transform)]
    public class OptimalDrivePosition
    {
        [Description("The coordinates of the arena center, in pixels.")]
        public Point ArenaCenter { get; set; }

        [Description("The coordinates of the motor plumb line on the arena floor, in pixels.")]
        public Point MotorPlumbLine { get; set; }

        [Description("The radius of the circular arena, in pixels.")]
        public double ArenaRadiusPixels { get; set; }

        [Description("The radius of the circular arena, in centimeters.")]
        public double ArenaRadiusCentimeters { get; set; }

        [Description("The distance from the arena floor to the bottom of the tether guide chute, in centimeters.")]
        public double TetherGuideAltitude { get; set; }

        [Description("The vertical distance from the bottom of the tether guide to the tether origin, in centimeters.")]
        public double TetherGuideHeight { get; set; }

        [Description("The maximum tether slack length allowed, in centimeters.")]
        public double TetherMaxSlackLength { get; set; }

        [Description("The length of the linear drive rail, in centimeters.")]
        public double LinearRailLength { get; set; }

        public IObservable<double> Process(IObservable<Point2f> source)
        {
            return source.Select(centroid =>
            {
                if (ArenaRadiusPixels == 0) return double.NaN;

                var center = ArenaCenter;
                var tetherGuideAltitude = TetherGuideAltitude;
                var tetherGuideHeight = TetherGuideHeight;
                var motorPlumbLine = MotorPlumbLine;
                var pixelsToCentimeters = ArenaRadiusCentimeters / ArenaRadiusPixels;
                var centroidX = centroid.X - center.X;
                var centroidY = centroid.Y - center.Y;

                // Get motor plumb line position relative to the arena center
                var motorOffsetX = motorPlumbLine.X - center.X;
                var motorOffsetY = motorPlumbLine.Y - center.Y;

                // Compute radial distance from the motor's plumb line
                var radialDistance = Math.Sqrt((centroidX - motorOffsetX) * (centroidX - motorOffsetX) + (centroidY - motorOffsetY) * (centroidY - motorOffsetY)) * pixelsToCentimeters;
                var distanceToTetherGuide = Math.Sqrt(radialDistance * radialDistance + tetherGuideAltitude * tetherGuideAltitude);

                // Slack adjustment and motor position
                var tetherGuideSlack = distanceToTetherGuide - tetherGuideAltitude;
                var remainingSlack = TetherMaxSlackLength - tetherGuideSlack;

                if (remainingSlack <= tetherGuideHeight) return 0.0;

                var linearMotorPosition = Math.Sqrt(remainingSlack * remainingSlack - tetherGuideHeight * tetherGuideHeight);
                return linearMotorPosition / LinearRailLength;
            });
        }

        public IObservable<double> Process(IObservable<ConnectedComponent> source)
        {
            return Process(source.Select(value => value.Centroid));
        }

        public IObservable<double> Process(IObservable<ConnectedComponentCollection> source)
        {
            return Process(source.Select(value =>
                value.Count == 1 ? value[0].Centroid : new Point2f(float.NaN, float.NaN)));
        }
    }
}
