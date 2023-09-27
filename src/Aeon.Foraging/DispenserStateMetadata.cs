namespace Aeon.Foraging
{
    public class DispenserStateMetadata
    {
        public DispenserStateMetadata(string name, int value, DispenserEventType eventType)
        {
            Name = name;
            Value = value;
            EventType = eventType;
        }

        public string Name { get; }

        public int Value { get; }

        public DispenserEventType EventType { get; }

        public override string ToString()
        {
            return $"DispenserState({Name}, {EventType}, Total:{Value})";
        }
    }

    public enum DispenserEventType
    {
        Discount,
        Refill,
        Reset
    }
}
