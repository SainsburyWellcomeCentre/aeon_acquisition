using Bonsai;
using OpenEphys.Onix1;
using System;
using System.ComponentModel;
using System.Reactive.Linq;

namespace Aeon.Ephys;

[Combinator]
[Description("Configures the devices in an ONIX NeuropixelsV2e headstage from the specified settings.")]
[WorkflowElementCategory(ElementCategory.Sink)]
public class ConfigureNeuropixelsV2eDevices
{
    public NeuropixelsV2eConfiguration Configuration { get; set; } = new NeuropixelsV2eConfiguration();

    public IObservable<ContextTask> Process(IObservable<ContextTask> source)
    {
        return Observable.Defer(() => ((ConfigureHeadstageNeuropixelsV2e)Configuration).Process(source));
    }
}
