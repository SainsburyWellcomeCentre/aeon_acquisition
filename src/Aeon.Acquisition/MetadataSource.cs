using Bonsai;
using Bonsai.Harp;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Aeon.Acquisition
{
    [Combinator]
    [WorkflowElementCategory(ElementCategory.Source)]
    public abstract class MetadataSource<TMetadata>
    {
        readonly Subject<TMetadata> subject = new Subject<TMetadata>();

        public void OnNext(TMetadata value)
        {
            subject.OnNext(value);
        }

        public virtual IObservable<TMetadata> Process()
        {
            return subject.ObserveOn(Scheduler.TaskPool);
        }

        public virtual IObservable<Timestamped<TMetadata>> Process(IObservable<HarpMessage> source)
        {
            return Process().Timestamp(source);
        }
    }
}
