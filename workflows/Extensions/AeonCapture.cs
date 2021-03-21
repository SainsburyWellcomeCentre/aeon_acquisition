using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Bonsai.Spinnaker;
using SpinnakerNET;

[Combinator]
[Description("Configures and initializes a Spinnaker camera for triggered acquisition.")]
[WorkflowElementCategory(ElementCategory.Source)]
public class AeonCapture : SpinnakerCapture
{
    public AeonCapture()
    {
        ExposureTime = 1e6 / 50 - 1000;
    }

    [Description("The duration of each individual exposure, in microseconds. In general, this should be 1 / frameRate - 1 millisecond to prepare for next trigger.")]
    public double ExposureTime { get; set; }

    protected override void Configure(IManagedCamera camera)
    {
        camera.AcquisitionFrameRateEnable.Value = false;
        camera.TriggerMode.Value = TriggerModeEnums.On.ToString();
        camera.TriggerSelector.Value = TriggerSelectorEnums.FrameStart.ToString();
        camera.TriggerSource.Value = TriggerSourceEnums.Line0.ToString();
        camera.TriggerOverlap.Value = TriggerOverlapEnums.ReadOut.ToString();
        camera.TriggerActivation.Value = TriggerActivationEnums.RisingEdge.ToString();
        camera.ExposureAuto.Value = ExposureAutoEnums.Off.ToString();
        camera.ExposureMode.Value = ExposureModeEnums.Timed.ToString();
        camera.ExposureTime.Value = ExposureTime;
        camera.GainAuto.Value = GainAutoEnums.Off.ToString();
        camera.Gain.Value = 0;
        base.Configure(camera);
    }
}
