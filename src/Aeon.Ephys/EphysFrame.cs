using Bonsai;
using OpenCV.Net;
using OpenEphys.Onix1;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace Aeon.Ephys;

/// <summary>
/// A buffer of electrophysiology samples, independent of which headstage produced it.
/// </summary>
/// <remarks>
/// Each headstage emits its own frame type.
/// This wrapper lets one subject serve every headstage, so downstream consumers need not know which will be active at runtime.
/// </remarks>
public class EphysFrame
{
    public Mat AmplifierData { get; set; }

    public ulong[] Clock { get; set; }

    public ulong[] HubClock { get; set; }

    /// <summary>
    /// Frame counter reported by the NeuropixelsV2e beta headstage. Null for headstages that do
    /// not report one.
    /// </summary>
    public int[] FrameCount { get; set; }

    /// <summary>
    /// Auxiliary channel data reported by the RHD2164. Null for headstages without one.
    /// </summary>
    public Mat AuxData { get; set; }
}

[Combinator]
[Description("Converts a headstage-specific data frame into a common ephys frame.")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class ConvertToEphysFrame
{
    // Members a headstage does not report are left null: FrameCount is beta-only and AuxData is
    // RHD2164-only.
    public IObservable<EphysFrame> Process(IObservable<NeuropixelsV2DataFrame> source)
    {
        return source.Select(value => new EphysFrame
        {
            AmplifierData = value.AmplifierData,
            Clock = value.Clock,
            HubClock = value.HubClock,
        });
    }

    public IObservable<EphysFrame> Process(IObservable<NeuropixelsV2eBetaDataFrame> source)
    {
        return source.Select(value => new EphysFrame
        {
            AmplifierData = value.AmplifierData,
            Clock = value.Clock,
            HubClock = value.HubClock,
            FrameCount = value.FrameCount,
        });
    }

    public IObservable<EphysFrame> Process(IObservable<Rhd2000DataFrame> source)
    {
        return source.Select(value => new EphysFrame
        {
            AmplifierData = value.AmplifierData,
            Clock = value.Clock,
            HubClock = value.HubClock,
            AuxData = value.AuxData,
        });
    }
}
