namespace Aeon.Ephys;

public partial class NeuropixelsV2BetaProbeConfiguration
{
    public static explicit operator OpenEphys.Onix1.ConfigureNeuropixelsV2BetaPsbDecoder(
        NeuropixelsV2BetaProbeConfiguration value)
    {
        return new OpenEphys.Onix1.ConfigureNeuropixelsV2BetaPsbDecoder
        {
            Enable = value.Enable,
            EnableLed = value.EnableLed,
            ProbeConfiguration =
                (OpenEphys.Onix1.NeuropixelsV2QuadShankProbeConfiguration)value.ProbeConfiguration
        };
    }
}
