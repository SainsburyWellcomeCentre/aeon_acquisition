namespace Aeon.Ephys;

public partial class OpticalStimulatorConfiguration
{
    public static explicit operator OpenEphys.Onix1.ConfigureHeadstage64OpticalStimulator(
        OpticalStimulatorConfiguration value)
    {
        return new OpenEphys.Onix1.ConfigureHeadstage64OpticalStimulator
        {
            Arm = value.Arm,
            MaxCurrent = value.MaxCurrent,
            ChannelOneCurrent = value.ChannelOneCurrent,
            ChannelTwoCurrent = value.ChannelTwoCurrent,
            Delay = value.Delay,
            PulseDuration = value.PulseDuration,
            PulsesPerSecond = value.PulsesPerSecond,
            PulsesPerBurst = ConfigurationConversion.ToUInt32(value.PulsesPerBurst, nameof(PulsesPerBurst)),
            InterBurstInterval = value.InterBurstInterval,
            BurstsPerTrain = ConfigurationConversion.ToUInt32(value.BurstsPerTrain, nameof(BurstsPerTrain))
        };
    }
}
