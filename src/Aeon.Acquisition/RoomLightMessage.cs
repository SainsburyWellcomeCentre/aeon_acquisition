namespace Aeon.Acquisition
{
    public struct RoomLightMessage
    {
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
