using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Bonsai.Audio;
using Bonsai.Harp;
using OpenCV.Net;

namespace Aeon.Acquisition
{
    [Description("Configures and initializes a software timestamped audio capture.")]
    public class AeonAudio : AudioCapture
    {
        IEnumerable<Timestamped<Mat>> TimestampBuffers(IEnumerable<Mat> buffers, HarpMessage message)
        {
            var timestamp = message.GetTimestamp();
            foreach (var buffer in buffers)
            {
                yield return Timestamped.Create(buffer, timestamp);
            }
        }

        public IObservable<Timestamped<Mat>> Generate(IObservable<HarpMessage> source)
        {
            var frames = Generate();
            return source.Where(68, MessageType.Event).Publish(triggers =>
                frames.Buffer(triggers)
                      .Zip(triggers, (list, trigger) => TimestampBuffers(list, trigger))
                      .SelectMany(x => x));
        }
    }
}