namespace Aeon.Ephys;

public partial class PortVoltageConfiguration
{
    public static explicit operator OpenEphys.Onix1.AutoPortVoltage(PortVoltageConfiguration value)
    {
        return new OpenEphys.Onix1.AutoPortVoltage
        {
            Requested = value.Requested
        };
    }
}
