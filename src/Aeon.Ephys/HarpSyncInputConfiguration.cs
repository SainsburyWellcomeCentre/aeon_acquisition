namespace Aeon.Ephys;

public partial class HarpSyncInputConfiguration
{
    public static explicit operator OpenEphys.Onix1.ConfigureHarpSyncInput(
        HarpSyncInputConfiguration value)
    {
        return new OpenEphys.Onix1.ConfigureHarpSyncInput
        {
            Enable = value.Enable,
            Source = value.Source
        };
    }
}
