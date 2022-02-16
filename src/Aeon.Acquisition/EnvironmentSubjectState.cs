using Bonsai;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Xml.Serialization;

namespace Aeon.Acquisition
{
    [TypeVisualizer(typeof(EnvironmentSubjectStateVisualizer))]
    [Description("Generates a sequence of events about subjects entering or leaving an environment.")]
    public class EnvironmentSubjectState : MetadataSource<EnvironmentSubjectStateMetadata>
    {
        [Description("The name of the environment.")]
        public string Name { get; set; }

        internal SubjectStateRecovery State { get; set; }

        public override IObservable<EnvironmentSubjectStateMetadata> Process()
        {
            var changes = base.Process();
            return Observable.Defer(() =>
            {
                State = StateRecovery<SubjectStateRecovery>.Deserialize(Name);
                var initialState = State.ActiveSubjects
                    .Select(subject => new EnvironmentSubjectStateMetadata(subject, EnvironmentSubjectChangeType.Remain))
                    .ToArray();
                return changes.Do(change =>
                {
                    switch (change.Type)
                    {
                        case EnvironmentSubjectChangeType.Enter:
                            State.ActiveSubjects.Add(new EnvironmentSubjectStateEntry
                            {
                                Id = change.Id,
                                Weight = change.Weight,
                                ReferenceWeight = change.ReferenceWeight
                            });
                            break;
                        case EnvironmentSubjectChangeType.Exit:
                            State.ActiveSubjects.Remove(change.Id);
                            break;
                        default:
                            return;
                    }
                    StateRecovery<SubjectStateRecovery>.Serialize(Name, State);
                }).StartWith(initialState);
            });
        }
    }

    public class SubjectStateRecovery
    {
        readonly ActiveSubjectCollection activeSubjects = new ActiveSubjectCollection();

        [XmlArrayItem("SubjectStateEntry")]
        public ActiveSubjectCollection ActiveSubjects
        {
            get { return activeSubjects; }
        }
    }
    public class ActiveSubjectCollection : KeyedCollection<string, EnvironmentSubjectStateEntry>
    {
        protected override string GetKeyForItem(EnvironmentSubjectStateEntry item)
        {
            return item.Id;
        }
    }
}
