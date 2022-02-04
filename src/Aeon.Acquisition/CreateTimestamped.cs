using Bonsai;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using Bonsai.Harp;

namespace Aeon.Acquisition
{
    [Combinator]
    [Description("Creates a timestamped structure from a value-timestamp pair.")]
    [WorkflowElementCategory(ElementCategory.Transform)]
    public class CreateTimestamped
    {
        public IObservable<Timestamped<TSource>> Process<TSource>(IObservable<Tuple<TSource, double>> source)
        {
            return source.Select(value => Timestamped.Create(value.Item1, value.Item2));
        }
    }
}