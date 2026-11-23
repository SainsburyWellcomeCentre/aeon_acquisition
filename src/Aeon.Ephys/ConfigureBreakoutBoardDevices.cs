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
    public BreakoutBoardConfiguration Configuration { get; set; } = new BreakoutBoardConfiguration();

    public IObservable<ContextTask> Process(IObservable<ContextTask> source)
    {
        return Observable.Defer(() => ((ConfigureBreakoutBoard)Configuration).Process(source));
    }
}
