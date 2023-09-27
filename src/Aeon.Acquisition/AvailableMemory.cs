using Bonsai;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace Aeon.Acquisition
{
    [Combinator]
    [Description("Samples a performance counter reporting the available system memory, in megabytes.")]
    [WorkflowElementCategory(ElementCategory.Transform)]
    public class AvailableMemory
    {
        public IObservable<float> Process<TSource>(IObservable<TSource> source)
        {
            return Observable.Using(
                () => new System.Diagnostics.PerformanceCounter("Memory", "Available MBytes"),
                performance => source.Select(_ => performance.NextValue()));
        }
    }
}
