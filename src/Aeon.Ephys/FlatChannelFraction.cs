using Bonsai;
using OpenCV.Net;
using System;
using System.ComponentModel;
using System.Reactive.Linq;

namespace Aeon.Ephys
{
    /// <summary>
    /// Represents an operator that computes the fraction of channels in each input
    /// matrix whose standard deviation is at or below a threshold.
    /// </summary>
    [Combinator]
    [Description("Computes the fraction of channels in each input matrix whose standard deviation is at or below the specified threshold.")]
    [WorkflowElementCategory(ElementCategory.Transform)]
    public class FlatChannelFraction
    {
        /// <summary>
        /// Gets or sets the standard deviation at or below which a single channel is
        /// considered flat. Zero detects only channels which are completely constant.
        /// </summary>
        [Description("The standard deviation at or below which a single channel is considered flat. Zero detects only channels which are completely constant.")]
        public double MinimumStandardDeviation { get; set; }

        /// <summary>
        /// Computes the fraction of flat channels in each matrix of an observable
        /// sequence, taking rows as channels and columns as samples.
        /// </summary>
        /// <param name="source">The sequence of sample matrices.</param>
        /// <returns>
        /// A sequence with the fraction of flat channels in each matrix, from zero to one.
        /// </returns>
        public IObservable<double> Process(IObservable<Mat> source)
        {
            return Observable.Defer(() =>
            {
                ChannelStatistics? statistics = null;
                return source.Select(value =>
                {
                    if (statistics == null || !statistics.Matches(value))
                    {
                        statistics?.Dispose();
                        statistics = new ChannelStatistics(value.Rows, value.Cols);
                    }

                    return statistics.FlatFraction(value, MinimumStandardDeviation);
                }).Finally(() => statistics?.Dispose());
            });
        }

        sealed class ChannelStatistics : IDisposable
        {
            readonly int channels;
            readonly int samples;
            readonly Mat data;
            readonly Mat squares;
            readonly Mat mean;
            readonly Mat meanSquares;
            readonly Mat meanSquared;
            readonly Mat variance;
            readonly Mat flatChannels;

            public ChannelStatistics(int channels, int samples)
            {
                this.channels = channels;
                this.samples = samples;
                // Double precision throughout, as E[x^2] approaches 4.3e9 for 16-bit
                // samples and single precision would clip.
                data = new Mat(channels, samples, Depth.F64, 1);
                squares = new Mat(channels, samples, Depth.F64, 1);
                mean = new Mat(channels, 1, Depth.F64, 1);
                meanSquares = new Mat(channels, 1, Depth.F64, 1);
                meanSquared = new Mat(channels, 1, Depth.F64, 1);
                variance = new Mat(channels, 1, Depth.F64, 1);
                flatChannels = new Mat(channels, 1, Depth.U8, 1);
            }

            public bool Matches(Mat value)
            {
                return value.Rows == channels && value.Cols == samples;
            }

            public double FlatFraction(Mat value, double minimumStandardDeviation)
            {
                // Per-channel variance as E[x^2] - E[x]^2, reducing along the sample
                // dimension so that each channel is summarised separately.
                CV.Convert(value, data);
                CV.Reduce(data, mean, 1, ReduceOperation.Avg);
                CV.Mul(data, data, squares, 1.0);
                CV.Reduce(squares, meanSquares, 1, ReduceOperation.Avg);
                CV.Mul(mean, mean, meanSquared, 1.0);
                CV.Sub(meanSquares, meanSquared, variance, null);

                // Compared against the squared threshold to avoid a square root over every channel.
                var threshold = minimumStandardDeviation * minimumStandardDeviation;
                CV.CmpS(variance, threshold, flatChannels, ComparisonOperation.LessOrEqual);
                return CV.CountNonZero(flatChannels) / (double)channels;
            }

            public void Dispose()
            {
                data.Dispose();
                squares.Dispose();
                mean.Dispose();
                meanSquares.Dispose();
                meanSquared.Dispose();
                variance.Dispose();
                flatChannels.Dispose();
            }
        }
    }
}
