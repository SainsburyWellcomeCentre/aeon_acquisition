using Bonsai;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using MathNet.Numerics;

namespace Aeon.Acquisition
{
    [Combinator]
    [Description("Calculates a linear regression filter over a sliding window of weights.")]
    [WorkflowElementCategory(ElementCategory.Transform)]
    public class WeightFilter
    {
        [Description("The number of samples in the sliding window.")]
        public int Count { get; set; } = 10;

        public IObservable<WeightMeasurement> Process(IObservable<WeightMeasurement> source)
        {
            return source
                .Buffer(Count, 1)
                .Where(buffer => buffer.Count == Count)
                .Select(buffer =>
                {
                    var xData = new double[buffer.Count];
                    var yData = new double[buffer.Count];
                    for (int i = 0; i < xData.Length; i++)
                    {
                        xData[i] = i;
                        yData[i] = buffer[i].Value;
                    }

                    WeightMeasurement result;
                    result.Timestamp = buffer[buffer.Count - 1].Timestamp;
                    var (intercept, slope) = Fit.Line(xData, yData);
                    result.Value = (float)intercept;
                    result.Confidence = 1 - (float)(Math.Atan(slope) / Constants.PiOver2);
                    return result;
                });
        }

        public IObservable<WeightMeasurement> Process<TOther>(IObservable<WeightMeasurement> source, IObservable<TOther> trigger)
        {
            return Process(source)
                .Window(trigger)
                .SelectMany(window =>
                    window.Publish(pwindow =>
                        pwindow.Take(1).CombineLatest(pwindow, (reference, measurement) =>
                        {
                            WeightMeasurement result;
                            result.Timestamp = measurement.Timestamp;
                            result.Confidence = measurement.Confidence;
                            result.Value = measurement.Value - reference.Value;
                            return result;
                        })));
        }
    }
}