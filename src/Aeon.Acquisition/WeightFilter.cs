using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using OpenCV.Net;
using MathNet.Numerics;

namespace Aeon.Acquisition
{
    [Combinator]
    [Description("Calculates the Linear regresison assuming x as sample index, return is intercept point")]
    [WorkflowElementCategory(ElementCategory.Transform)]
    public class WeightFilter
    {
        public int Count { get; set; } = 10;
        public double Threshold { get; set; }
        public IObservable<double> Process(IObservable<double> source)
        {
            return source.Buffer<double>(Count, 1).Where(buf => buf.Count == Count).Select(value =>
             {

                 int samples = value.Count;
                 double[] xData = new double[samples];
                 for (int count = 0; count < samples; count++)
                 {
                     xData[count] = count;
                 }
                 return Fit.Line(xData, value.ToArray());
             }).Where(f => Math.Abs(f.Item2) <= Threshold).Select(f => f.Item1);
        }

        public IObservable<double> Process<TOther>(IObservable<double> source, IObservable<TOther> trigger)
        {
            return Process(source).Window(trigger).SelectMany(window => window.Publish(f => f.Take(1).CombineLatest(f, (reference, value) =>
            {
                return value - reference;
            })));
        }
    }
}