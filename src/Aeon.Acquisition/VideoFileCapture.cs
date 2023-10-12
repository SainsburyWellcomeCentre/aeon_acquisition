using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using Bonsai;
using Bonsai.Harp;
using Bonsai.Vision;

namespace Aeon.Acquisition
{
    [Description("Configures and initializes a file capture for timestamped replay of video data.")]
    public class VideoFileCapture : Source<Timestamped<VideoDataFrame>>
    {
        [Description("The path to the file used to source the video frames.")]
        [Editor("Bonsai.Design.OpenFileNameEditor, Bonsai.Design", DesignTypes.UITypeEditor)]
        public string Path { get; set; }

        public override IObservable<Timestamped<VideoDataFrame>> Generate()
        {
            var videoFileName = Path;
            if (string.IsNullOrEmpty(videoFileName))
            {
                throw new InvalidOperationException("A valid file name must be specified");
            }

            return Observable.Defer(() =>
            {
                var grayscale = new Grayscale();
                var capture = new FileCapture { FileName = videoFileName };
                var metadataFileName = System.IO.Path.ChangeExtension(videoFileName, ".csv");
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

                return grayscale.Process(capture.Generate()).Select((frame, index) =>
                {
                    var (seconds, frameID, frameTimestamp) = metadataContents[index];
                    var dataFrame = new VideoDataFrame(frame, frameID, frameTimestamp);
                    return Timestamped.Create(dataFrame, seconds);
                });
            });
        }
    }
}
