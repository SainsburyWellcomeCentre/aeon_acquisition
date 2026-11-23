namespace Aeon.Ephys;

public partial class NeuropixelsV2eBetaConfiguration
{
    /// <summary>
    /// Returns a copy of the headstage settings using the specified probe settings.
    /// </summary>
    internal NeuropixelsV2eBetaConfiguration WithProbes(
        NeuropixelsV2BetaProbeConfiguration probeA,
        NeuropixelsV2BetaProbeConfiguration probeB)
    {
        return new NeuropixelsV2eBetaConfiguration(this) { ProbeA = probeA, ProbeB = probeB };
    }

    public static explicit operator OpenEphys.Onix1.ConfigureHeadstageNeuropixelsV2eBeta(
        NeuropixelsV2eBetaConfiguration value)
    {
        return new OpenEphys.Onix1.ConfigureHeadstageNeuropixelsV2eBeta
        {
            NeuropixelsV2A = (OpenEphys.Onix1.ConfigureNeuropixelsV2BetaPsbDecoder)value.ProbeA,
            NeuropixelsV2B = (OpenEphys.Onix1.ConfigureNeuropixelsV2BetaPsbDecoder)value.ProbeB,
            Bno055 = (OpenEphys.Onix1.ConfigurePolledBno055)value.Bno055,
            PortVoltage = (OpenEphys.Onix1.AutoPortVoltage)value.PortVoltage,
            TrackingLed = value.TrackingLed,
            Name = value.Name,
            Port = value.Port
        };
    }
}
