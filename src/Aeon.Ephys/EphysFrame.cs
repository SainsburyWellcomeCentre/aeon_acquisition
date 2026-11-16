using OpenCV.Net;
using OpenEphys.Onix1;

namespace Aeon.Ephys;

/// <summary>
/// A buffer of electrophysiology samples, independent of which headstage produced it.
/// </summary>
/// <remarks>
/// Each headstage emits its own frame type, so a workflow serving any headstage cannot name one.
/// The frame it produced is kept as its declared base type, from which a logger builds the schema
/// of the concrete type at runtime, and the amplifier samples are exposed for display and analysis.
/// </remarks>
public class EphysFrame
{
    public EphysFrame(NeuropixelsV2DataFrame value)
    {
        BufferedDataFrame = value;
        AmplifierData = value.AmplifierData;
    }

    public EphysFrame(NeuropixelsV2eBetaDataFrame value)
    {
        BufferedDataFrame = value;
        AmplifierData = value.AmplifierData;
    }

    public EphysFrame(Rhd2000DataFrame value)
    {
        BufferedDataFrame = value;
        AmplifierData = value.AmplifierData;
    }

    public BufferedDataFrame BufferedDataFrame { get; private set; }

    public Mat AmplifierData { get; private set; }

    public ulong[] Clock
    {
        get { return BufferedDataFrame.Clock; }
    }

    public ulong[] HubClock
    {
        get { return BufferedDataFrame.HubClock; }
    }
}
