using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using Bonsai;

namespace Aeon.Foraging
{
    [Combinator]
    [Description("Generates a sequence of the estimated number of units in the specified dispenser.")]
    public class DispenserAccumulate
    {
        public IObservable<DispenserState> Process(IObservable<DispenserEventArgs> source)
        {
            return source.Scan(new DispenserState(), Accumulate);
        }

        public IObservable<DispenserState> Process(IObservable<DispenserEventArgs> source, IObservable<DispenserState> seed)
        {
            return seed.Take(1).SelectMany(state => source.Scan(state, Accumulate));
        }

        static DispenserState Accumulate(DispenserState state, DispenserEventArgs evt)
        {
            return evt.EventType switch
            {
                DispenserEventType.Discount => new DispenserState { Count = state.Count - evt.Value },
                DispenserEventType.Refill => new DispenserState { Count = state.Count + evt.Value },
                DispenserEventType.Reset => new DispenserState { Count = evt.Value },
                _ => throw new InvalidOperationException("Invalid dispenser event type."),
            };
        }
    }

    public class DispenserState
    {
        public int Count { get; set; }

        public override string ToString()
        {
            return $"DispenserState(Total: {Count})";
        }
    }
}
