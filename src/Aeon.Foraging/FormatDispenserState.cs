﻿using Bonsai;
using Bonsai.Harp;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace Aeon.Foraging
{
    [Combinator]
    [Description("Converts a sequence of dispenser states into a sequence of Harp messages.")]
    [WorkflowElementCategory(ElementCategory.Transform)]
    public class FormatDispenserState
    {
        [Description("The address of the virtual Harp register.")]
        public int Address { get; set; } = 200;

        public IObservable<HarpMessage> Process(IObservable<Timestamped<DispenserState>> source)
        {
            return source.Select(input => HarpMessage.FromSingle(
                Address,
                input.Seconds,
                MessageType.Event,
                input.Value.Count));
        }
    }
}
