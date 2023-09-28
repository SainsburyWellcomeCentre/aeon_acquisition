using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using Bonsai;
using Bonsai.Harp;

namespace Aeon.Acquisition
{
    [Combinator]
    [WorkflowElementCategory(ElementCategory.Transform)]
    [Description("Converts a sequence of referenced Harp timestamps into system date-time objects.")]
    public class GetDateTime
    {
        static DateTime FromSeconds(double seconds)
        {
            return GroupByTime.ReferenceTime.AddSeconds(seconds);
        }

        public IObservable<DateTime> Process(IObservable<HarpMessage> source)
        {
            return source.Select(message => FromSeconds(message.GetTimestamp()));
        }

        public IObservable<DateTime> Process<T>(IObservable<Timestamped<T>> source)
        {
            return source.Select(_ => FromSeconds(_.Seconds));
        }
    }
}
