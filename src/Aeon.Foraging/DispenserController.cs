using Aeon.Acquisition;
using Bonsai;
using Bonsai.Harp;
using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Aeon.Foraging
{
    [DefaultProperty(nameof(Name))]
    [TypeVisualizer(typeof(DispenserEventVisualizer))]
    [Description("Generates a sequence of event commands for the specified dispenser.")]
    public class DispenserController : MetadataSource<DispenserEventArgs>, INamedElement
    {
        [Description("The name of the dispenser.")]
        public string Name { get; set; }

        string INamedElement.Name => $"{Name}{nameof(DispenserController)}";

        internal BehaviorSubject<DispenserState> State { get; } = new(value: default);

        public IObservable<DispenserEventArgs> Process(IObservable<DispenserState> source)
        {
            return Process().Merge(source
                .Do(State).IgnoreElements()
                .Cast<DispenserEventArgs>());
        }

        public IObservable<Timestamped<DispenserEventArgs>> Process(IObservable<DispenserState> source, IObservable<HarpMessage> clockSource)
        {
            return Process(clockSource).Merge(source
                .Do(State).IgnoreElements()
                .Cast<Timestamped<DispenserEventArgs>>());
        }
    }
}
