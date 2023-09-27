using Bonsai;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace Aeon.Environment
{
    [Combinator]
    [Description("Baselines a sequence of weight measurements on a specified trigger event.")]
    [WorkflowElementCategory(ElementCategory.Transform)]
    public class WeightBaseline
    {
        public IObservable<WeightMeasurement> Process<TOther>(IObservable<WeightMeasurement> source, IObservable<TOther> trigger)
        {
            return source
                .Window(trigger).Skip(1)
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
