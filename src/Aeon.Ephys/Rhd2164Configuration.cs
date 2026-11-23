namespace Aeon.Ephys;

public partial class Rhd2164Configuration
{
    public static explicit operator OpenEphys.Onix1.ConfigureRhd2164(Rhd2164Configuration value)
    {
        return new OpenEphys.Onix1.ConfigureRhd2164
        {
            Enable = value.Enable,
            AnalogHighCutoff = value.AnalogHighCutoff,
            AnalogLowCutoff = value.AnalogLowCutoff,
            DspCutoff = value.DspCutoff
        };
    }
}
