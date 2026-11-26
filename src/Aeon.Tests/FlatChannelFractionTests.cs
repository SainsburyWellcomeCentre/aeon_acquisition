using System;
using System.Reactive.Linq;
using Aeon.Ephys;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenCV.Net;

namespace Aeon.Tests
{
    // The OpenCV native library ships in a Windows runtime package. These tests run only
    // where the Linux build filters driver-dependent tests out.
    [TestClass, TestCategory("DriverDependent")]
    public class FlatChannelFractionTests
    {
        const int Samples = 100;

        static Mat CreateFrame(params Func<int, double>[] channels)
        {
            // Rows are channels and columns are samples, as in a probe data frame.
            var values = new double[channels.Length, Samples];
            for (int channel = 0; channel < channels.Length; channel++)
            {
                for (int sample = 0; sample < Samples; sample++)
                {
                    values[channel, sample] = channels[channel](sample);
                }
            }

            return Mat.FromArray(values);
        }

        static double Fraction(Mat frame, double minimumStandardDeviation = 0)
        {
            var fraction = new FlatChannelFraction { MinimumStandardDeviation = minimumStandardDeviation };
            return fraction.Process(Observable.Return(frame)).Wait();
        }

        [TestMethod]
        public void ConstantChannelsCountAsFlat()
        {
            // One constant channel in four gives a quarter, whatever the constant is.
            var frame = CreateFrame(
                _ => 512,
                sample => sample,
                sample => Math.Sin(sample),
                sample => sample % 2 == 0 ? 10 : -10);

            Assert.AreEqual(0.25, Fraction(frame));
        }

        [TestMethod]
        public void AllFlatAndNoneFlatAreTheExtremes()
        {
            Assert.AreEqual(1.0, Fraction(CreateFrame(_ => 0, _ => 1, _ => -3)));
            Assert.AreEqual(0.0, Fraction(CreateFrame(sample => sample, sample => -sample)));
        }

        [TestMethod]
        public void ThresholdAdmitsLowAmplitudeChannels()
        {
            // A channel alternating between plus and minus a half has a standard deviation
            // of exactly a half. It is flat at that threshold and not below it.
            var frame = CreateFrame(
                sample => sample % 2 == 0 ? 0.5 : -0.5,
                sample => sample);

            Assert.AreEqual(0.0, Fraction(frame, 0.49));
            Assert.AreEqual(0.5, Fraction(frame, 0.5));
        }

        [TestMethod]
        public void BuffersFollowFrameShape()
        {
            // The working buffers are reused across frames and rebuilt when the shape
            // changes. A sequence of different shapes must not throw or go stale.
            var fraction = new FlatChannelFraction();
            var frames = Observable.Return(CreateFrame(_ => 0, sample => sample))
                .Concat(Observable.Return(CreateFrame(_ => 0, _ => 1, sample => sample)))
                .Concat(Observable.Return(CreateFrame(sample => sample)));

            var results = fraction.Process(frames).ToArray().Wait();
            CollectionAssert.AreEqual(new[] { 0.5, 2.0 / 3.0, 0.0 }, results);
        }
    }
}
