namespace Aeon.Ephys;

public partial class Headstage64Configuration
{
    public static explicit operator OpenEphys.Onix1.ConfigureHeadstage64(Headstage64Configuration value)
    {
        return new OpenEphys.Onix1.ConfigureHeadstage64
        {
            Rhd2164 = (OpenEphys.Onix1.ConfigureRhd2164)value.Rhd2164,
            Bno055 = (OpenEphys.Onix1.ConfigureBno055)value.Bno055,
            TS4231 = (OpenEphys.Onix1.ConfigureTS4231V1)value.Ts4231,
            ElectricalStimulator =
                (OpenEphys.Onix1.ConfigureHeadstage64ElectricalStimulator)value.ElectricalStimulator,
            OpticalStimulator =
                (OpenEphys.Onix1.ConfigureHeadstage64OpticalStimulator)value.OpticalStimulator,
            PortVoltage = (OpenEphys.Onix1.AutoPortVoltage)value.PortVoltage,
            Name = value.Name,
            Port = value.Port
        };
    }
}
