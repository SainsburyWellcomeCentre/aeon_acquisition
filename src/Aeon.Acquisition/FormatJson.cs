using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using Bonsai;
using Newtonsoft.Json;

namespace Aeon.Acquisition
{
    [Combinator]
    [Description("Formats a sequence of values into a sequence of JSON strings.")]
    [WorkflowElementCategory(ElementCategory.Transform)]
    public class FormatJson
    {
        public IObservable<string> Process<TSource>(IObservable<TSource> source)
        {
            return source.Select(value => JsonConvert.SerializeObject(value));
        }
    }
}
