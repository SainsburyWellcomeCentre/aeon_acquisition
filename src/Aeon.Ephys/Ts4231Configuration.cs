namespace Aeon.Ephys;

public partial class Ts4231Configuration
{
    public static explicit operator OpenEphys.Onix1.ConfigureTS4231V1(Ts4231Configuration value)
    {
        return new OpenEphys.Onix1.ConfigureTS4231V1
        {
            Enable = value.Enable
        };
    }
}
