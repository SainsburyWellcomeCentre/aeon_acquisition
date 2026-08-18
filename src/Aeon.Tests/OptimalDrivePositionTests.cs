using Aeon.Tether.Translation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenCV.Net;
using System.Reactive.Linq;

namespace Aeon.Tests
{
    [TestClass]
    public class OptimalDrivePositionTests
    {
        static double Process(OptimalDrivePosition transform, Point2f centroid)
        {
            return transform.Process(Observable.Return(centroid)).Wait();
        }

        [TestMethod]
        public void Process_ZeroArenaRadiusPixels_ReturnsNaN()
        {
            // A zero arena radius would divide by zero when converting pixels to
            // centimeters, so the transform reports an invalid reading instead.
            var transform = new OptimalDrivePosition
            {
                ArenaRadiusPixels = 0,
                ArenaRadiusCentimeters = 100,
                TetherMaxSlackLength = 6,
                LinearRailLength = 8,
            };

            Assert.IsTrue(double.IsNaN(Process(transform, new Point2f(3, 0))));
        }

        [TestMethod]
        public void Process_CentroidWithinReach_ReturnsNormalizedPosition()
        {
            // With a one-to-one pixel to centimeter scale, a centroid three
            // centimeters from the plumb line and a four centimeter guide altitude
            // give a five centimeter guide distance and one centimeter of slack. The
            // remaining five centimeters against a three centimeter guide height map
            // to four centimeters of rail travel, half of the eight centimeter rail.
            var transform = new OptimalDrivePosition
            {
                MotorPlumbLine = new Point(0, 0),
                ArenaRadiusPixels = 100,
                ArenaRadiusCentimeters = 100,
                TetherGuideAltitude = 4,
                TetherGuideHeight = 3,
                TetherMaxSlackLength = 6,
                LinearRailLength = 8,
            };

            Assert.AreEqual(0.5, Process(transform, new Point2f(3, 0)), 1e-9);
        }

        [TestMethod]
        public void Process_SlackBeyondMaximum_ReturnsZero()
        {
            // A centroid far from the plumb line demands more slack than allowed, so
            // the drive is commanded fully extended at position zero.
            var transform = new OptimalDrivePosition
            {
                MotorPlumbLine = new Point(0, 0),
                ArenaRadiusPixels = 100,
                ArenaRadiusCentimeters = 100,
                TetherGuideAltitude = 4,
                TetherGuideHeight = 3,
                TetherMaxSlackLength = 6,
                LinearRailLength = 8,
            };

            Assert.AreEqual(0.0, Process(transform, new Point2f(100, 0)));
        }
    }
}
