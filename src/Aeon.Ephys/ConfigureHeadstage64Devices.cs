using Bonsai;
using OpenEphys.Onix1;
using System;
using System.ComponentModel;
using System.Reactive.Linq;

namespace Aeon.Ephys;

[Combinator]
[Description("Configures the devices in an ONIX 64-channel headstage from the specified settings.")]
[WorkflowElementCategory(ElementCategory.Sink)]
public class ConfigureHeadstage64Devices
{
    public Headstage64Configuration Configuration { get; set; } = new Headstage64Configuration();

    public IObservable<ContextTask> Process(IObservable<ContextTask> source)
    {
        return Observable.Defer(() => ((ConfigureHeadstage64)Configuration).Process(source));
    }
}
