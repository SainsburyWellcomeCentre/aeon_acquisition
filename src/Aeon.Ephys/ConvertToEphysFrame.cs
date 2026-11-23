using Bonsai;
using OpenEphys.Onix1;
using System;
using System.ComponentModel;
using System.Reactive.Linq;

namespace Aeon.Ephys;

[Combinator]
[Description("Converts a headstage-specific data frame into a common ephys frame.")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class ConvertToEphysFrame
{
    public IObservable<EphysFrame> Process(IObservable<NeuropixelsV2DataFrame> source)
    {
        return source.Select(value => new EphysFrame(value));
    }

    public IObservable<EphysFrame> Process(IObservable<NeuropixelsV2eBetaDataFrame> source)
    {
        return source.Select(value => new EphysFrame(value));
    }

    public IObservable<EphysFrame> Process(IObservable<Rhd2000DataFrame> source)
    {
        return source.Select(value => new EphysFrame(value));
    }
}
