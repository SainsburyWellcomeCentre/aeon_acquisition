using Bonsai;
using Bonsai.Harp;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace Aeon.Acquisition
{
    [Combinator]
    [Description("Converts a sequence of weight measurements into a sequence of Harp messages.")]
    [WorkflowElementCategory(ElementCategory.Transform)]
    public class FormatWeight
    {
        const int Address = 200;

        public IObservable<HarpMessage> Process(IObservable<WeightMeasurement> source)
        {
            return source.Select(input => HarpMessage.FromSingle(
                Address,
                input.Timestamp,
                MessageType.Event,
                input.Value,
                input.Confidence));
        }
    }
}
