namespace Aeon.Ephys;

public partial class NeuropixelsV2eBetaConfiguration
{
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
