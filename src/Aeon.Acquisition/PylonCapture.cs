using Bonsai;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using Bonsai.Harp;
using Basler.Pylon;

namespace Aeon.Acquisition
{
    [Description("Configures and initializes a Pylon camera for triggered acquisition.")]
    public class PylonCapture : Bonsai.Pylon.PylonCapture
    {
        public IObservable<Timestamped<VideoDataFrame>> Generate<TPayload>(IObservable<Timestamped<TPayload>> source)
        {
            var frames = Generate();
            return frames
                .Select(frame => new VideoDataFrame(
                    frame.Image,
                    frame.GrabResult.ChunkData[PLChunkData.ChunkCounterValue].GetValue(),
                    frame.GrabResult.ChunkData[PLChunkData.ChunkTimestamp].GetValue()))
                .FillGaps(frame => frame.ChunkData.FrameID, (previous, current) => (int)(current - previous - 1))
                .Zip(source, (frame, payload) => Timestamped.Create(frame, payload.Seconds))
                .Where(timestamped => timestamped.Value != null);
        }
    }
}
