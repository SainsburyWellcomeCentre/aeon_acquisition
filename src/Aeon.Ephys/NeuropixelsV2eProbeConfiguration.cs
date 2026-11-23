namespace Aeon.Ephys;

public partial class NeuropixelsV2eProbeConfiguration
{
    /// <summary>
    /// Returns a copy of the probe settings using the specified gain calibration file.
    /// </summary>
    internal NeuropixelsV2eProbeConfiguration WithGainCalibrationFileName(string fileName)
    {
        return new NeuropixelsV2eProbeConfiguration(this)
        {
            ProbeConfiguration = ProbeConfiguration.WithGainCalibrationFileName(fileName)
        };
    }

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
