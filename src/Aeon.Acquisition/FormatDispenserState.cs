using Bonsai;
using Bonsai.Harp;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace Aeon.Acquisition
{
    [Combinator]
    [Description("Converts a sequence of dispenser states into a sequence of Harp messages.")]
    [WorkflowElementCategory(ElementCategory.Transform)]
    public class FormatDispenserState
    {
        const int Address = 200;

        public IObservable<HarpMessage> Process(IObservable<Timestamped<DispenserStateMetadata>> source)
        {
            return source.Select(input => HarpMessage.FromSingle(
                Address,
                input.Seconds,
                MessageType.Event,
                input.Value.Value));
        }
    }
}
