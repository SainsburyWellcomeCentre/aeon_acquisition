using Bonsai;
using OpenCV.Net;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace Aeon.Video
{
    [Combinator]
    [Description("Converts a sequence of image-metadata pairs into a sequence of VideoDataFrame objects.")]
    public class CreateVideoDataFrame
    {
        public IObservable<VideoDataFrame> Process(IObservable<Tuple<IplImage, VideoChunkData>> source)
        {
            return source.Select(xs => new VideoDataFrame(xs.Item1, xs.Item2));
        }
    }
}
