using Bonsai;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using Bonsai.Spinnaker;
using SpinnakerNET;
using Bonsai.Harp;

[Description("Configures and initializes a Spinnaker camera for triggered acquisition.")]
public class ThermalCapture : SpinnakerCapture
{
    public ThermalCapture()
    {
    }

    protected override void Configure(IManagedCamera camera)
    {
        try { camera.AcquisitionStop.Execute(); }
        catch { }
        camera.TriggerMode.Value = TriggerModeEnums.On.ToString();
        camera.TriggerSelector.Value = AcquisitionStatusSelectorEnums.AcquisitionActive.ToString();
        camera.TriggerSource.Value = TriggerSourceEnums.Line0.ToString();
        camera.TriggerActivation.Value = TriggerActivationEnums.RisingEdge.ToString();
        camera.LineSelector.Value = LineSelectorEnums.Line2.ToString();
        camera.LineSource.Value = "FrameSync";
        base.Configure(camera);
    }

    public IObservable<Timestamped<SpinnakerDataFrame>> Generate(IObservable<HarpMessage> source)
    {
        var frames = Generate();
        var triggers = source.Where(68, MessageType.Event);
        return frames.Zip(triggers, (frame, trigger) =>
        {
            var payload = trigger.GetTimestampedPayloadByte();
            return Timestamped.Create(frame, payload.Seconds);
        });
    }

    public IObservable<Timestamped<SpinnakerDataFrame>> Generate<TSource>(IObservable<Timestamped<TSource>> source)
    {
        return source.Publish(ps => Generate().CombineLatest(ps, (frame, trigger) => Timestamped.Create(frame, trigger.Seconds)).Sample(ps));
    }
}
