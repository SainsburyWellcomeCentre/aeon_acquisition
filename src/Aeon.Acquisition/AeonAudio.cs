﻿using Bonsai;
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
        IEnumerable<Timestamped<Mat>> TimestampBuffers(IEnumerable<Mat> buffers, double timestamp)
        {
            foreach (var buffer in buffers)
            {
                yield return Timestamped.Create(buffer, timestamp);
            }
        }

        public IObservable<Timestamped<Mat>> Generate<TPayload>(IObservable<Timestamped<TPayload>> source)
        {
            var data = Generate();
            return source.Publish(triggers =>
                data.Buffer(triggers)
                    .Zip(triggers, (list, trigger) => TimestampBuffers(list, trigger.Seconds))
                    .SelectMany(x => x));
        }
    }
}
