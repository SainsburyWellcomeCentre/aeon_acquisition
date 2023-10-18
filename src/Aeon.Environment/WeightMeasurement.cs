namespace Aeon.Environment
{
    public struct WeightMeasurement
    {
        public double Timestamp;
        public float Value;
        public float Confidence;

        public WeightMeasurement(double timestamp, float value, float confidence)
        {
            Timestamp = timestamp;
            Value = value;
            Confidence = confidence;
        }

        public override string ToString()
        {
            return $"{nameof(WeightMeasurement)}(Seconds: {Timestamp}, Value: {Value}, Confidence: {Confidence})";
        }
    }
}
