namespace Aeon.Ephys;

public partial class DigitalIOConfiguration
{
    public static explicit operator OpenEphys.Onix1.ConfigureDigitalIO(DigitalIOConfiguration value)
    {
        return new OpenEphys.Onix1.ConfigureDigitalIO
        {
            Enable = value.Enable,
            DeadTime = value.DeadTime,
            SampleRate = value.SampleRate
        };
    }
}
