namespace Aeon.Ephys;

public partial class MemoryMonitorConfiguration
{
    public static explicit operator OpenEphys.Onix1.ConfigureMemoryMonitor(MemoryMonitorConfiguration value)
    {
        return new OpenEphys.Onix1.ConfigureMemoryMonitor
        {
            Enable = value.Enable,
            SamplesPerSecond = ConfigurationConversion.ToUInt32(value.SamplesPerSecond, nameof(SamplesPerSecond))
        };
    }
}
