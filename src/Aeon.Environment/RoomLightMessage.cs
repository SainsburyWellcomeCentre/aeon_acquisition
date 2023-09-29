namespace Aeon.Environment
{
    public struct RoomLightMessage
    {
        internal const int NoChange = -1;
        internal const int MaxLightValue = 254;

        public int Channel;
        public int Value;

        public RoomLightMessage(int channel, int value)
        {
            Channel = channel;
            Value = value;
        }

        public override string ToString()
        {
            return $"RoomLight({Channel}, {Value})";
        }
    }
}
