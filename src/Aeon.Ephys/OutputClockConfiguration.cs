namespace Aeon.Ephys;

public partial class OutputClockConfiguration
{
    public static explicit operator OpenEphys.Onix1.ConfigureOutputClock(OutputClockConfiguration value)
    {
        return new OpenEphys.Onix1.ConfigureOutputClock
        {
            ClockGate = value.ClockGate,
            Frequency = value.Frequency,
            DutyCycle = value.DutyCycle,
            Delay = value.Delay
        };
    }
}
