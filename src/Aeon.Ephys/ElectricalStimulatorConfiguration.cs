namespace Aeon.Ephys;

public partial class ElectricalStimulatorConfiguration
{
    public static explicit operator OpenEphys.Onix1.ConfigureHeadstage64ElectricalStimulator(
        ElectricalStimulatorConfiguration value)
    {
        return new OpenEphys.Onix1.ConfigureHeadstage64ElectricalStimulator
        {
            Arm = value.Arm,
            TriggerDelay = ConfigurationConversion.ToUInt32(value.TriggerDelay, nameof(TriggerDelay)),
            PhaseOneDuration = ConfigurationConversion.ToUInt32(value.PhaseOneDuration, nameof(PhaseOneDuration)),
            PhaseOneCurrent = value.PhaseOneCurrent,
            InterPhaseInterval = ConfigurationConversion.ToUInt32(value.InterPhaseInterval, nameof(InterPhaseInterval)),
            InterPhaseCurrent = value.InterPhaseCurrent,
            PhaseTwoDuration = ConfigurationConversion.ToUInt32(value.PhaseTwoDuration, nameof(PhaseTwoDuration)),
            PhaseTwoCurrent = value.PhaseTwoCurrent,
            InterPulseInterval = ConfigurationConversion.ToUInt32(value.InterPulseInterval, nameof(InterPulseInterval)),
            BurstPulseCount = ConfigurationConversion.ToUInt32(value.BurstPulseCount, nameof(BurstPulseCount)),
            InterBurstInterval = ConfigurationConversion.ToUInt32(value.InterBurstInterval, nameof(InterBurstInterval)),
            TrainBurstCount = ConfigurationConversion.ToUInt32(value.TrainBurstCount, nameof(TrainBurstCount))
        };
    }
}
