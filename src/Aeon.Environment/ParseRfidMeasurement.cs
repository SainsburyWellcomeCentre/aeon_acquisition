using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using Bonsai;
using Bonsai.Harp;
using Harp.RfidReader;
using OpenCV.Net;

namespace Aeon.Environment
{
    [Description("Generates a sequence of spatialized detection events when a tag enters the area of the reader.")]
    public class ParseRfidMeasurement : Combinator<HarpMessage, Timestamped<RfidMeasurement>>
    {
        [Description("The location to associate with each detection event.")]
        public Point2f Location { get; set; }

        public override IObservable<Timestamped<RfidMeasurement>> Process(IObservable<HarpMessage> source)
        {
            return source.Where(InboundDetectionId.Address).Select(message =>
            {
                var (tagId, timestamp) = InboundDetectionId.GetTimestampedPayload(message);
                return Timestamped.Create(new RfidMeasurement(Location, tagId), timestamp);
            });
        }
    }
}
