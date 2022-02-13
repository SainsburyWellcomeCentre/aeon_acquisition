namespace Aeon.Acquisition
{
    public class AlertMetadata
    {
        public AlertMetadata(AlertType type, string message)
        {
            Type = type;
            Message = message;
        }

        public AlertType Type { get; }

        public string Message { get; }

        public override string ToString()
        {
            return $"Alert({Type}, Message:{Message})";
        }
    }

    public enum AlertType
    {
        Annotation,
        PriorityAnnotation
    }
}
