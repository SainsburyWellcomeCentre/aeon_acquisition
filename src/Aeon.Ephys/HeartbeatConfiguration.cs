namespace Aeon.Ephys;

public partial class HeartbeatConfiguration
{
    public static explicit operator OpenEphys.Onix1.ConfigurePersistentHeartbeat(
        HeartbeatConfiguration value)
    {
        return new OpenEphys.Onix1.ConfigurePersistentHeartbeat
        {
            BeatsPerSecond = (uint)value.BeatsPerSecond
        };
    }
}
