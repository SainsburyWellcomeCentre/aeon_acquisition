namespace Aeon.Ephys;

public partial class NeuropixelsV2eProbeConfiguration
{
    public static explicit operator OpenEphys.Onix1.ConfigureNeuropixelsV2PsbDecoder(
        NeuropixelsV2eProbeConfiguration value)
    {
        return new OpenEphys.Onix1.ConfigureNeuropixelsV2PsbDecoder
        {
            Enable = value.Enable,
            ProbeConfiguration =
                (OpenEphys.Onix1.NeuropixelsV2QuadShankProbeConfiguration)value.ProbeConfiguration
        };
    }
}
