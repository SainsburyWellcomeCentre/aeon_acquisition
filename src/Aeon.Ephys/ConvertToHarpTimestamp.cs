using Bonsai;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace Aeon.Ephys;

/// <summary>
/// Represents an operator that converts the whole-second <c>HarpTime</c> of an ONIX
/// Harp sync input frame into a fractional Harp timestamp.
/// </summary>
/// <remarks>
/// The Harp sync packet encodes the previous second, so a receiver must add one second
/// once. OpenEphys.Onix1 0.4.0 and later already do this in <c>HarpSyncInputDataFrame.HarpTime</c>
/// (<see href="https://github.com/open-ephys/bonsai-onix1/pull/319"/>), so this operator
/// applies no offset. Earlier rig workflows added the second again, which made the logged
/// <c>Seconds</c> one second late; see
/// <see href="https://github.com/SainsburyWellcomeCentre/aeon_exp_foragingABC/issues/141"/>.
/// </remarks>
[Combinator]
[Description("Converts a raw ONIX Harp sync whole-second input data to fractional aligned seconds. OpenEphys.Onix1 0.4.0 and later already add the one second the Harp sync protocol requires, so no offset is applied here.")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class ConvertToHarpTimestamp
{
    public IObservable<double> Process(IObservable<uint> source)
    {
        return source.Select(value => (double)value);
    }
}
