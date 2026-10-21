namespace Aeon.Ephys;

public partial class ElectricalStimulatorConfiguration
{
    public static explicit operator OpenEphys.Onix1.ConfigureHeadstage64ElectricalStimulator(
        ElectricalStimulatorConfiguration value)
    {
        return new OpenEphys.Onix1.ConfigureHeadstage64ElectricalStimulator
        {
            Arm = value.Arm,
            TriggerDelay = (uint)value.TriggerDelay,
            PhaseOneDuration = (uint)value.PhaseOneDuration,
            PhaseOneCurrent = value.PhaseOneCurrent,
            InterPhaseInterval = (uint)value.InterPhaseInterval,
            InterPhaseCurrent = value.InterPhaseCurrent,
            PhaseTwoDuration = (uint)value.PhaseTwoDuration,
            PhaseTwoCurrent = value.PhaseTwoCurrent,
            InterPulseInterval = (uint)value.InterPulseInterval,
            BurstPulseCount = (uint)value.BurstPulseCount,
            InterBurstInterval = (uint)value.InterBurstInterval,
            TrainBurstCount = (uint)value.TrainBurstCount
        };
    }
}
