namespace Aeon.Acquisition
{
    public class DispenserStateMetadata
    {
        public DispenserStateMetadata(string name, int value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }

        public int Value { get; }

        public override string ToString()
        {
            return $"DispenserState({Name}, {Value})";
        }
    }
}
