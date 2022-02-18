using Bonsai;
using Bonsai.Harp;
using System;
using System.ComponentModel;
using System.Reactive.Linq;

namespace Aeon.Acquisition
{
    [DefaultProperty(nameof(Name))]
    [TypeVisualizer(typeof(DispenserStateVisualizer))]
    [Description("Generates a sequence of the estimated number of units in the specified dispenser.")]
    public class DispenserState : MetadataSource<DispenserStateMetadata>, INamedElement
    {
        [Description("The name of the dispenser.")]
        public string Name { get; set; }

        string INamedElement.Name => $"{Name}Dispenser";

        internal DispenserStateRecovery State { get; set; }

        public override IObservable<Timestamped<DispenserStateMetadata>> Process(IObservable<HarpMessage> source)
        {
            var refill = Process();
            return Observable.Defer(() =>
            {
                State = StateRecovery<DispenserStateRecovery>.Deserialize(Name);
                var initialState = new DispenserStateMetadata(Name, State.Value);
                return source.Publish(ps =>
                {
                    const int DigitalOutput = 35;
                    const int TriggerPellet = 0x80;
                    var discount = ps.Where(message =>
                        message.Address == DigitalOutput &&
                        message.MessageType == MessageType.Write &&
                        message.GetPayloadByte() == TriggerPellet)
                        .Select(_ => new DispenserStateMetadata(Name, -1));
                    return refill.Merge(discount).StartWith(initialState).Publish(changes =>
                        changes.CombineLatest(ps, (data, message) => (data, message))
                        .Sample(changes.MergeUnit(ps.Take(1)))
                        .Select((x, i) =>
                        {
                            var data = x.data;
                            var timestamp = x.message.GetTimestamp();
                            if (i > 0) State.Value += data.Value;
                            StateRecovery<DispenserStateRecovery>.Serialize(Name, State);
                            data = new DispenserStateMetadata(data.Name, State.Value);
                            return Timestamped.Create(data, timestamp);
                        }));
                });
            });
        }
    }

    public class DispenserStateRecovery
    {
        public int Value { get; set; }
    }
}
