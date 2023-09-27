namespace Aeon.Environment
{
    public class EnvironmentSubjectStateMetadata
    {
        public EnvironmentSubjectStateMetadata(EnvironmentSubjectStateEntry entry, EnvironmentSubjectChangeType entryType)
        {
            Id = entry.Id;
            Type = entryType;
            ReferenceWeight = entry.ReferenceWeight;
            Weight = entry.Weight;
        }

        public string Id { get; }

        public EnvironmentSubjectChangeType Type { get; }

        public float ReferenceWeight { get; }

        public float Weight { get; }

        public override string ToString()
        {
            return $"EnvironmentSubject({Id}, {Type}, {Weight})";
        }
    }

    public enum EnvironmentSubjectChangeType
    {
        Enter,
        Exit,
        Remain
    }
}
