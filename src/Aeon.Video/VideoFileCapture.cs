using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using Bonsai;
using Bonsai.Harp;
using Bonsai.Vision;
using OpenCV.Net;

namespace Aeon.Video
{
    [Description("Configures and initializes a file capture for timestamped replay of video data.")]
    public class VideoFileCapture : Source<Timestamped<VideoDataFrame>>
    {
        [Description("The path to the file used to source the video frames.")]
        [Editor("Bonsai.Design.OpenFileNameEditor, Bonsai.Design", DesignTypes.UITypeEditor)]
        public string FileName { get; set; }

        public override IObservable<Timestamped<VideoDataFrame>> Generate()
        {
            var videoFileName = FileName;
            if (string.IsNullOrEmpty(videoFileName))
            {
                throw new InvalidOperationException("A valid file name must be specified");
            }

            return Observable.Defer(() =>
            {
                var grayscale = new Grayscale();
                var capture = new FileCapture { FileName = videoFileName };
                var metadataFileName = Path.ChangeExtension(videoFileName, ".csv");
                var metadataContents = File.ReadAllLines(metadataFileName).Skip(1).Select(row =>
                {
                    var values = row.Split(',');
                    if (values.Length != 3 ||
                        !double.TryParse(values[0], out double seconds) ||
                        !long.TryParse(values[1], out long frameID) ||
                        !long.TryParse(values[2], out long frameTimestamp))
                    {
                        throw new InvalidOperationException(
                            "Frame metadata file should be in 3-column comma-separated text format.");
                    }

                    return (seconds, frameID, frameTimestamp);
                }).ToArray();

                var frames = capture.Generate();
                return grayscale.Process(frames).Select((frame, index) =>
                {
                    var (seconds, frameID, frameTimestamp) = metadataContents[index];
                    var dataFrame = new VideoDataFrame(frame, frameID, frameTimestamp);
                    return Timestamped.Create(dataFrame, seconds);
                });
            });
        }

        public IObservable<Timestamped<VideoDataFrame>> Generate<TPayload>(IObservable<Timestamped<TPayload>> source)
        {
            var videoFileName = FileName;
            if (string.IsNullOrEmpty(videoFileName))
            {
                throw new InvalidOperationException("A valid file name must be specified");
            }

            const string ImageExtensions = ".png;.bmp;.jpg;.jpeg;.tif;.tiff;.exr";
            var extension = Path.GetExtension(videoFileName);
            return source.Publish(trigger =>
            {
                var frameID = 0L;
                Timestamped<VideoDataFrame> TimestampFrame(Timestamped<TPayload> timestamped, IplImage frame)
                {
                    var dataFrame = new VideoDataFrame(frame, frameID++, (long)(timestamped.Seconds * 1e6));
                    return Timestamped.Create(dataFrame, timestamped.Seconds);
                }

                if (!string.IsNullOrEmpty(extension) && ImageExtensions.Contains(extension))
                {
                    var capture = new LoadImage { FileName = videoFileName, Mode = LoadImageFlags.Grayscale };
                    return trigger.CombineLatest(capture.Generate(), TimestampFrame);
                }
                else
                {
                    var grayscale = new Grayscale();
                    var capture = new FileCapture { FileName = videoFileName, Loop = true };
                    return trigger.Zip(grayscale.Process(capture.Generate(trigger)), TimestampFrame);
                }
            });
        }
    }
}
