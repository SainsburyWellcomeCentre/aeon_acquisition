using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

[Combinator]
[Description("Selects the substring preceding the first occurrence of the specified separator.")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class StripSubstring
{
    public string Separator { get; set; }

    public IObservable<string> Process(IObservable<string> source)
    {
        return source.Select(value =>
        {
            var parts = value.Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries);
            return parts.Length > 0 ? parts[0] : value;
        });
    }
}
