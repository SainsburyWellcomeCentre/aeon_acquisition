namespace Aeon.Ephys;

public partial class NeuropixelsV2QuadShankConfiguration
{
    public static explicit operator OpenEphys.Onix1.NeuropixelsV2QuadShankProbeConfiguration(
        NeuropixelsV2QuadShankConfiguration value)
    {
        return new OpenEphys.Onix1.NeuropixelsV2QuadShankProbeConfiguration
        {
            InvertPolarity = value.InvertPolarity,
            Reference = value.Reference,
            GainCalibrationFileName = value.GainCalibrationFileName,
            ProbeInterfaceFileName = value.ProbeInterfaceFileName
        };
    }
}
