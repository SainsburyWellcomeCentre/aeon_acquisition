using Bonsai;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace Aeon.Acquisition
{
    [Combinator]
    [Description("Formats a date time into a string with no illegal path characters.")]
    [WorkflowElementCategory(ElementCategory.Transform)]
    public class FormatDate
    {
        public IObservable<string> Process(IObservable<DateTime> source)
        {
            return source.Select(value =>
            {
                var result = value.ToString("o").Replace(':', '-');
                return result.Substring(0, result.IndexOf('.'));
            });
        }
    }
}