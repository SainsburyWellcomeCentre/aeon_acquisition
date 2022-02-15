using Bonsai;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace Aeon.Acquisition
{
    [TypeVisualizer(typeof(SubjectChangeVisualizer))]
    [Description("Generates a sequence of events about subjects entering or leaving the experiment.")]
    public class SubjectChange : MetadataSource<SubjectChangeMetadata>
    {
        internal SubjectStateRecovery State { get; set; }

        public override IObservable<SubjectChangeMetadata> Process()
        {
            var changes = base.Process();
            return Observable.Defer(() =>
            {
                State = StateRecovery<SubjectStateRecovery>.Deserialize(string.Empty);
                var initialState = State.ActiveSubjects
                    .Select(subject => new SubjectChangeMetadata(subject, SubjectChangeType.Remain))
                    .ToArray();
                return changes.Do(change =>
                {
                    switch (change.Type)
                    {
                        case SubjectChangeType.Enter:
                            State.ActiveSubjects.Add(new SubjectChangeEntry
                            {
                                Id = change.Id,
                                Weight = change.Weight,
                                ReferenceWeight = change.ReferenceWeight
                            });
                            break;
                        case SubjectChangeType.Exit:
                            State.ActiveSubjects.Remove(change.Id);
                            break;
                        default:
                            return;
                    }
                    StateRecovery<SubjectStateRecovery>.Serialize(string.Empty, State);
                }).StartWith(initialState);
            });
        }
    }

    public class SubjectStateRecovery
    {
        readonly ActiveSubjectCollection activeSubjects = new ActiveSubjectCollection();

        public ActiveSubjectCollection ActiveSubjects
        {
            get { return activeSubjects; }
        }
    }
    public class ActiveSubjectCollection : KeyedCollection<string, SubjectChangeEntry>
    {
        protected override string GetKeyForItem(SubjectChangeEntry item)
        {
            return item.Id;
        }
    }
}
