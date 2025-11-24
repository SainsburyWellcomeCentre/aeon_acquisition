using Bonsai;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace Aeon.Acquisition
{
    [Combinator]
    [WorkflowElementCategory(ElementCategory.Transform)]
    [Description("Converts a sequence of date-time objects by truncating each value to whole seconds.")]
    public class TruncateDateTime
    {
        public IObservable<DateTime> Process(IObservable<DateTime> source)
        {
            return source.Select(value => value.AddTicks(-(value.Ticks % TimeSpan.TicksPerSecond)));
        }

        public IObservable<DateTimeOffset> Process(IObservable<DateTimeOffset> source)
        {
            return source.Select(value => value.AddTicks(-(value.Ticks % TimeSpan.TicksPerSecond)));
        }
    }
}
