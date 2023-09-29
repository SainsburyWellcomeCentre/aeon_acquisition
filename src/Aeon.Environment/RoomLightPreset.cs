namespace Aeon.Environment
{
    public struct RoomLightPreset
    {
        public string Name;
        public float ColdWhite;
        public float WarmWhite;
        public float Red;

        public RoomLightPreset(string name, float coldWhite, float warmWhite, float red)
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
