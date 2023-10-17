namespace Aeon.Foraging
{
    public class DispenserEventArgs
    {
        public DispenserEventArgs(int value, DispenserEventType eventType)
        {
            Value = value;
            EventType = eventType;
        }

        public int Value { get; }

        public DispenserEventType EventType { get; }

        public override string ToString()
        {
            return $"DispenserEvent({EventType}, Value:{Value})";
        }
    }

    public enum DispenserEventType
    {
        Discount,
        Refill,
        Reset
    }
}
