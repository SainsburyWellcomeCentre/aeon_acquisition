using Bonsai;
using Bonsai.Harp;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace Aeon.Acquisition
{
    [Description("Generates a sequence of prioritised messages for the event log.")]
    public class CreateLogMessage : Source<LogMessage>
    {
        [Description("The priority level of the log message.")]
        public PriorityLevel Priority { get; set; }

        [Description("The type or category of the log message.")]
        public string Type { get; set; } = LogMessage.DefaultType;

        [Description("The log message text.")]
        public string Message { get; set; }

        public override IObservable<LogMessage> Generate()
        {
            return Observable.Defer(() =>
            {
                var eventLog = new LogMessage(Priority, Type, Message);
                return Observable.Return(eventLog);
            });
        }

        public IObservable<LogMessage> Generate<TSource>(IObservable<TSource> source)
        {
            return source.Select(_ => new LogMessage(Priority, Type, Message));
        }

        public IObservable<Timestamped<LogMessage>> Generate<TSource>(IObservable<Timestamped<TSource>> source)
        {
            return source.Select(value => Timestamped.Create(
                new LogMessage(Priority, Type, Message),
                value.Seconds));
        }

        public IObservable<Timestamped<LogMessage>> Generate(IObservable<HarpMessage> source)
        {
            return source.Select(value => Timestamped.Create(
                new LogMessage(Priority, Type, Message),
                value.GetTimestamp()));
        }
    }
}
