namespace Aeon.Acquisition
{
    public struct RoomLightMessage
    {
        public string Name;
        public int ColdWhite;
        public int WarmWhite;
        public int Red;

        public RoomLightMessage(string name, int coldWhite, int warmWhite, int red)
        {
            Name = name;
            ColdWhite = coldWhite;
            WarmWhite = warmWhite;
            Red = red;
        }

        public override string ToString()
        {
            return $"RoomLight({Name}, ColdWhite:{ColdWhite}, WarmWhite:{WarmWhite}, Red:{Red})";
        }
    }
}
