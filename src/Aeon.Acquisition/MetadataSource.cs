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

        public virtual IObservable<TMetadata> Process()
        {
            return subject;
        }

        public virtual IObservable<Timestamped<TMetadata>> Process(IObservable<HarpMessage> source)
        {
            return source.Publish(
                ps => Process().Publish(
                    xs => xs.CombineLatest(ps, (data, message) => (data, message))
                            .Sample(xs.MergeUnit(ps.Take(1)))
                            .Select(x =>
                            {
                                var timestamp = x.message.GetTimestamp();
                                return Timestamped.Create(x.data, timestamp);
                            })));
        }
    }
}
