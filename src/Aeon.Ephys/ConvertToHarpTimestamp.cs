using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace Aeon.Ephys;

[Combinator]
[Description("Converts a raw ONIX harp sync wholse second input data to fractional aligned seconds.")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class ConvertToHarpTimestamp
{
    public IObservable<double> Process(IObservable<uint> source)
    {
        return source.Select(value => (double)(value + 1));
    }
}
