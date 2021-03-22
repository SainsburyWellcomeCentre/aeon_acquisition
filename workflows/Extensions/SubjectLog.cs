using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Bonsai.Harp;

public class LogMetadata
{
    public string Id { get; set; }

    public float Weight { get; set; }

    [Browsable(false)]
    public EventType Event { get; set; }

    public override string ToString()
    {
        return string.Format("Id:{0}, Weight:{1}, Event:{2}", Id, Weight, Event);
    }
}

public enum EventType
{
    Start,
    Stop,
    End
}

[Combinator]
[Description("Logs information about subjects manually entered or removed into the arena.")]
[WorkflowElementCategory(ElementCategory.Source)]
[TypeVisualizer(typeof(SubjectLogVisualizer))]
public class SubjectLog
{
    readonly Subject<LogMetadata> subject = new Subject<LogMetadata>();

    public void OnNext(LogMetadata value)
    {
        subject.OnNext(value);
    }

    public IObservable<LogMetadata> Process()
    {
        return subject.Where(data => data.Event != EventType.Stop);
    }

    public IObservable<Timestamped<LogMetadata>> Process(IObservable<HarpMessage> source)
    {
        return Observable.Defer(()=>
        {
            var stopTimestamp = 0.0;
            return subject.CombineLatest(source, (data, message) =>
            {
                var timestamp = message.GetTimestamp();
                if (data.Event == EventType.Stop) stopTimestamp = timestamp;
                else if (data.Event == EventType.End) timestamp = stopTimestamp;
                return Timestamped.Create(data, timestamp);
            }).Sample(subject).Where(data => data.Value.Event != EventType.Stop);
        });
    }
}
