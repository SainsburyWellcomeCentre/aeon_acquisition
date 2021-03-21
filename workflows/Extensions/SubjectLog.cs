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
}

public enum EventType
{
    Enter,
    Exit
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
        return subject;
    }

    public IObservable<Timestamped<LogMetadata>> Process(IObservable<HarpMessage> source)
    {
        return subject.CombineLatest(source, (data, message) =>
        {
            var timestamp = message.GetTimestamp();
            return Timestamped.Create(data, timestamp);
        }).Sample(subject);
    }
}
