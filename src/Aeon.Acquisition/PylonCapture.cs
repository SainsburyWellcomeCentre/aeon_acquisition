using Bonsai;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using Bonsai.Harp;

namespace Aeon.Acquisition
{
    [Description("Configures and initializes a Pylon camera for triggered acquisition.")]
    public class PylonCapture : Bonsai.Pylon.PylonCapture
    {
        public IObservable<Timestamped<VideoDataFrame>> Generate(IObservable<HarpMessage> source)
        {
            var frames = Generate();
            return frames
                .Select(frame => new VideoDataFrame(frame.Image, frame.GrabResult.ID, frame.GrabResult.Timestamp))
                .FillGaps(frame => frame.ChunkData.FrameID, (previous, current) => (int)(current - previous - 1))
                .Zip(source, (frame, trigger) =>
                {
                    var payload = trigger.GetTimestampedPayloadByte();
                    return Timestamped.Create(frame, payload.Seconds);
                })
                .Where(timestamped => timestamped.Value != null);
        }
    }
}
