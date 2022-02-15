namespace Aeon.Acquisition
{
    public class SubjectChangeMetadata
    {
        public SubjectChangeMetadata(SubjectChangeEntry entry, SubjectChangeType entryType)
        {
            Id = entry.Id;
            Type = entryType;
            ReferenceWeight = entry.ReferenceWeight;
            Weight = entry.Weight;
        }

        public string Id { get; }

        public SubjectChangeType Type { get; }

        public float ReferenceWeight { get; }

        public float Weight { get; }

        public override string ToString()
        {
            return $"SubjectChange({Id}, {Type}, {Weight})";
        }
    }

    public enum SubjectChangeType
    {
        Enter,
        Exit,
        Remain
    }
}
