namespace Aeon.Ephys;

public partial class NeuropixelsV2BetaProbeConfiguration
{
    /// <summary>
    /// Returns a copy of the probe settings using the specified gain calibration file.
    /// </summary>
    internal NeuropixelsV2BetaProbeConfiguration WithGainCalibrationFileName(string fileName)
    {
        return new NeuropixelsV2BetaProbeConfiguration(this)
        {
            ProbeConfiguration = ProbeConfiguration.WithGainCalibrationFileName(fileName)
        };
    }

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
