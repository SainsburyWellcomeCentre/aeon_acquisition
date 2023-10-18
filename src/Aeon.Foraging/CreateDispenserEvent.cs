using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using Bonsai;

namespace Aeon.Foraging
{
    [Description("Create a sequence of dispenser event commands.")]
    public class CreateDispenserEvent : Source<DispenserEventArgs>
    {
        [Description("The number of dispenser units associated with the event command.")]
        public int Value { get; set; }

        [Description("Specifies the type of dispenser event command to create.")]
        public DispenserEventType EventType { get; set; }

        public override IObservable<DispenserEventArgs> Generate()
        {
            return Observable.Return(new DispenserEventArgs(Value, EventType));
        }

        public IObservable<DispenserEventArgs> Generate<TSource>(IObservable<TSource> source)
        {
            return source.Select(value => new DispenserEventArgs(Value, EventType));
        }
    }
}
