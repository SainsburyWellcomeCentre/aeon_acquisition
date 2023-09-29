namespace Aeon.Environment
{
    public struct RoomLightPreset
    {
        public string Name;
        public int ColdWhite;
        public int WarmWhite;
        public int Red;

        public RoomLightPreset(string name, int coldWhite, int warmWhite, int red)
        {
            Name = name;
            ColdWhite = coldWhite;
            WarmWhite = warmWhite;
            Red = red;
        }

        public override readonly string ToString()
        {
            return $"RoomLightPreset({Name}, {ColdWhite}, {WarmWhite}, {Red})";
        }
    }
}
