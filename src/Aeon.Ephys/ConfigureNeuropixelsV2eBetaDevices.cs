using Bonsai;
using OpenEphys.Onix1;
using System;
using System.ComponentModel;
using System.Reactive.Linq;

namespace Aeon.Ephys;

[Combinator]
[Description("Configures the devices in an ONIX beta NeuropixelsV2e headstage from the specified settings.")]
[WorkflowElementCategory(ElementCategory.Sink)]
public class ConfigureNeuropixelsV2eBetaDevices
{
    public NeuropixelsV2eBetaConfiguration Configuration { get; set; } =
        new NeuropixelsV2eBetaConfiguration();

    public IObservable<ContextTask> Process(IObservable<ContextTask> source)
    {
        return Observable.Defer(
            () => ((ConfigureHeadstageNeuropixelsV2eBeta)Configuration).Process(source));
    }
}
