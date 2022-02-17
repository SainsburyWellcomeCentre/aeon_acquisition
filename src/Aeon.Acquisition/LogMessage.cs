namespace Aeon.Acquisition
{
    public class LogMessage
    {
        public const string DefaultType = "Annotation";

        public LogMessage(PriorityLevel priority, string message)
            : this(priority, DefaultType, message)
        {
        }

        public LogMessage(PriorityLevel priority, string type, string message)
        {
            Priority = priority;
            Type = type;
            Message = message;
        }

        public PriorityLevel Priority { get; }

        public string Type { get; }

        public string Message { get; }

        public override string ToString()
        {
            return $"Log({Priority}:{Type}, Message:{Message})";
        }
    }

    public enum PriorityLevel
    {
        Notification,
        Alert
    }
}
