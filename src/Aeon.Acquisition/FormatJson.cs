using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using Bonsai;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Aeon.Acquisition
{
    [Combinator]
    [Description("Formats a sequence of values into a sequence of JSON strings.")]
    [WorkflowElementCategory(ElementCategory.Transform)]
    public class FormatJson
    {
        static readonly JsonSerializerSettings DefaultSettings = new()
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            }
        };

        public IObservable<string> Process<TSource>(IObservable<TSource> source)
        {
            return source.Select(value => JsonConvert.SerializeObject(value, DefaultSettings));
        }
    }
}
