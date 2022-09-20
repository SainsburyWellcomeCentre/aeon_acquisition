using Bonsai;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using Bonsai.Spinnaker;
using Bonsai.Harp;
using Bonsai.Pylon;

namespace Aeon.Acquisition
{
    [Description("Configures and initializes a Spinnaker camera for triggered acquisition.")]
    public class AeonPylonCapture : PylonCapture
    {
        public AeonPylonCapture()
        {
            ExposureTime = 1e6 / 50 - 1000;
            TriggerAddress = 68;
            Binning = 1;
        }

        [Description("The duration of each individual exposure, in microseconds. In general, this should be 1 / frameRate - 1 millisecond to prepare for next trigger.")]
        public double ExposureTime { get; set; }

        [Description("The gain of the sensor.")]
        public double Gain { get; set; }

        [Description("The size of the binning area of the sensor, e.g. a binning size of 2 specifies a 2x2 binning region.")]
        public int Binning { get; set; }

        [Description("The address of the Harp register used for triggering new exposures.")]
        public int TriggerAddress { get; set; }

        public IObservable<Timestamped<PylonDataFrame>> Generate(IObservable<HarpMessage> source)
        {
            var frames = Generate();
            var triggers = source.Where(TriggerAddress, MessageType.Event);
            return frames
                .FillGaps(frame => frame.GrabResult.ID, (previous, current) => (int)(current - previous - 1))
                .Zip(triggers, (frame, trigger) =>
                {
                    var payload = trigger.GetTimestampedPayloadByte();
                    return Timestamped.Create(frame, payload.Seconds);
                })
                .Where(timestamped => timestamped.Value != null);
        }
    }
}
