using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using Aeon.Acquisition;
using Bonsai;

namespace Aeon.Foraging
{
    [Combinator]
    [DefaultProperty(nameof(Name))]
    [Description("Generates a sequence of the estimated number of units in the specified dispenser.")]
    public class DispenserState : INamedElement
    {
        [Description("The name of the dispenser.")]
        public string Name { get; set; }

        string INamedElement.Name => $"{Name}{nameof(DispenserState)}";

        public IObservable<DispenserStateRecovery> Process(IObservable<DispenserEventArgs> source)
        {
            var name = Name;
            return Observable.Defer(() =>
            {
                var state = StateRecovery<DispenserStateRecovery>.Deserialize(name);
                return Observable.Return(state).Concat(source.Select(evt =>
                {
                    state = evt.EventType switch
                    {
                        DispenserEventType.Discount => new DispenserStateRecovery { Value = state.Value - evt.Value },
                        DispenserEventType.Refill => new DispenserStateRecovery { Value = state.Value + evt.Value },
                        DispenserEventType.Reset => new DispenserStateRecovery { Value = evt.Value },
                        _ => throw new InvalidOperationException("Invalid dispenser event type."),
                    };
                    StateRecovery<DispenserStateRecovery>.Serialize(name, state);
                    return state;
                }));
            });
        }
    }

    public class DispenserStateRecovery
    {
        public int Value { get; set; }
    }
}
