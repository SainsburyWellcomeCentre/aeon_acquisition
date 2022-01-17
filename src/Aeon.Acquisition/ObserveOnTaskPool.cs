using Bonsai;
using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reactive.Concurrency;

namespace Aeon.Acquisition
{
    [Combinator]
    [Description("Runs observer callbacks in the Task Parallel Library (TPL) task pool.")]
    [WorkflowElementCategory(ElementCategory.Combinator)]
    public class ObserveOnTaskPool
    {
        public IObservable<TSource> Process<TSource>(IObservable<TSource> source)
        {
            return source.ObserveOn(Scheduler.TaskPool);
        }
    }
}