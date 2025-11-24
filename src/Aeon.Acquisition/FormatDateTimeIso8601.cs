using Bonsai;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace Aeon.Acquisition
{
    [Combinator]
    [Description("Formats a date time into an ISO 8601 string with no illegal path characters.")]
    [WorkflowElementCategory(ElementCategory.Transform)]
    public class FormatDateTimeIso8601
    {
        public IObservable<string> Process(IObservable<DateTime> source)
        {
            return source.Select(value => value.ToString("yyyy-MM-ddTHHmmss.FFFFFFFK").Replace(":", string.Empty));
        }

        public IObservable<string> Process(IObservable<DateTimeOffset> source)
        {
            return source.Select(value => value.ToString("yyyy-MM-ddTHHmmss.FFFFFFFzzz").Replace(":", string.Empty));
        }
    }
}
