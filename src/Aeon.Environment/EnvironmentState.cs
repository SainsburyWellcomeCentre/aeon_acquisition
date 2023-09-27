using Bonsai;
using System;
using System.ComponentModel;
using System.Reactive.Linq;
using Aeon.Acquisition;

namespace Aeon.Environment
{
    [TypeVisualizer(typeof(EnvironmentStateVisualizer))]
    [Description("Generates a sequence indicating changes in environment state.")]
    public class EnvironmentState : MetadataSource<EnvironmentStateMetadata>, INamedElement
    {
        [Description("The name of the environment.")]
        public string Name { get; set; }

        string INamedElement.Name => $"{Name.AsNullIfEmpty() ?? "Environment"}State";

        internal EnvironmentStateRecovery State { get; set; }

        public override IObservable<EnvironmentStateMetadata> Process()
        {
            var changes = base.Process();
            return Observable.Defer(() =>
            {
                State = StateRecovery<EnvironmentStateRecovery>.Deserialize(Name);
                var initialState = new EnvironmentStateMetadata(Name, State.Type);
                return changes.Do(change =>
                {
                    State.Type = change.Type;
                    StateRecovery<EnvironmentStateRecovery>.Serialize(Name, State);
                }).StartWith(initialState);
            });
        }
    }

    public class EnvironmentStateRecovery
    {
        public EnvironmentStateType Type { get; set; }
    }
}
