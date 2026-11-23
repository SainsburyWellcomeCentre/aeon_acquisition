namespace Aeon.Ephys;

public partial class NeuropixelsV2eConfiguration
{
    public static explicit operator OpenEphys.Onix1.ConfigureHeadstageNeuropixelsV2e(
        NeuropixelsV2eConfiguration value)
    {
        return new OpenEphys.Onix1.ConfigureHeadstageNeuropixelsV2e
        {
            NeuropixelsV2A = (OpenEphys.Onix1.ConfigureNeuropixelsV2PsbDecoder)value.ProbeA,
            NeuropixelsV2B = (OpenEphys.Onix1.ConfigureNeuropixelsV2PsbDecoder)value.ProbeB,
            Bno055 = (OpenEphys.Onix1.ConfigurePolledBno055)value.Bno055,
            PortVoltage = (OpenEphys.Onix1.AutoPortVoltage)value.PortVoltage,
            Name = value.Name,
            Port = value.Port
        };
    }
}
