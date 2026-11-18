namespace Aeon.Ephys;

public partial class BreakoutBoardConfiguration
{
    /// <summary>
    /// The name every breakout device is addressed by. The monitoring workflows ask for
    /// devices by this name, so it is fixed rather than configured.
    /// </summary>
    public const string BreakoutBoardName = "BreakoutBoard";

    public static explicit operator OpenEphys.Onix1.ConfigureBreakoutBoard(BreakoutBoardConfiguration value)
    {
        // Assigning a device clears the name the board stamped on it, and only the board
        // name setter stamps it back, so the name must stay the last assignment.
        return new OpenEphys.Onix1.ConfigureBreakoutBoard
        {
            Heartbeat = (OpenEphys.Onix1.ConfigurePersistentHeartbeat)value.Heartbeat,
            AnalogIO = (OpenEphys.Onix1.ConfigureAnalogIO)value.AnalogIo,
            DigitalIO = (OpenEphys.Onix1.ConfigureDigitalIO)value.DigitalIo,
            ClockOutput = (OpenEphys.Onix1.ConfigureOutputClock)value.ClockOutput,
            HarpInput = (OpenEphys.Onix1.ConfigureHarpSyncInput)value.HarpInput,
            MemoryMonitor = (OpenEphys.Onix1.ConfigureMemoryMonitor)value.MemoryMonitor,
            Name = BreakoutBoardName
        };
    }
}
