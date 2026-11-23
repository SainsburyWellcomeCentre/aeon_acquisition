namespace Aeon.Ephys;

public partial class NeuropixelsV2QuadShankConfiguration
{
    // The gain calibration file is resolved from the surgery record, never read from a
    // configuration file. YAML ignores the property. A configuration file cannot name it.
    // JSON writes it. The session metadata records the resolved path.
    // The generated copy constructor does not copy it. Copies set it again, as
    // WithGainCalibrationFileName does.
    /// <summary>
    /// Gets the path of the gain calibration file resolved from the surgery record.
    /// </summary>
    [Newtonsoft.Json.JsonProperty("gainCalibrationFileName")]
    [YamlDotNet.Serialization.YamlIgnore]
    [System.ComponentModel.Browsable(false)]
    public string GainCalibrationFileName { get; internal set; } = "";

    /// <summary>
    /// Returns a copy of the electrode settings using the specified gain calibration file.
    /// </summary>
    internal NeuropixelsV2QuadShankConfiguration WithGainCalibrationFileName(string fileName)
    {
        return new NeuropixelsV2QuadShankConfiguration(this) { GainCalibrationFileName = fileName };
    }

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
