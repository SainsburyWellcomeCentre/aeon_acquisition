using Bonsai;
using Bonsai.Harp;
using System;
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

        public IObservable<TMetadata> Process()
        {
            return subject;
        }

        public virtual IObservable<Timestamped<TMetadata>> Process(IObservable<HarpMessage> source)
        {
            return Observable.Defer(() =>
            {
                return subject.CombineLatest(source, (data, message) =>
                {
                    var timestamp = message.GetTimestamp();
                    return Timestamped.Create(data, timestamp);
                }).Sample(subject);
            });
        }
    }
}
