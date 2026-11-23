namespace Aeon.Ephys;

public partial class Bno055Configuration
{
    public static explicit operator OpenEphys.Onix1.ConfigureBno055(Bno055Configuration value)
    {
        return new OpenEphys.Onix1.ConfigureBno055
        {
            Enable = value.Enable
        };
    }
}
