using Bonsai;
using OpenEphys.Onix1;
using System;
using System.ComponentModel;
using System.Reactive.Linq;

namespace Aeon.Ephys;

[Combinator]
[Description("Configures the devices in the ONIX breakout board from the specified settings.")]
[WorkflowElementCategory(ElementCategory.Sink)]
public class ConfigureBreakoutBoardDevices
{
    /// <summary>
    /// The name every breakout device is addressed by. Data operators ask for devices by
    /// name, so changing this would strand every one of them.
    /// </summary>
    const string BreakoutBoardName = "BreakoutBoard";

    public HarpSyncInputConfiguration HarpInput { get; set; } = new HarpSyncInputConfiguration();

    /// <summary>
    /// Builds the breakout board configuration. Exposed so the device names and addresses
    /// it derives can be asserted without a subscription, and so without hardware.
    /// </summary>
    public ConfigureBreakoutBoard CreateConfiguration()
    {
        var breakout = new ConfigureBreakoutBoard();
        breakout.MemoryMonitor.Enable = true;
        breakout.HarpInput = (ConfigureHarpSyncInput)HarpInput;

        // Assigning a device clears the name the board stamped on it, and only the board
        // name setter stamps it back, so this must stay the last assignment.
        breakout.Name = BreakoutBoardName;
        return breakout;
    }

    public IObservable<ContextTask> Process(IObservable<ContextTask> source)
    {
        return Observable.Defer(() => CreateConfiguration().Process(source));
    }
}
