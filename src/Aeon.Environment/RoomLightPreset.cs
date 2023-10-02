namespace Aeon.Environment
{
    public struct RoomLightPreset
    {
        public float ColdWhite;
        public float WarmWhite;
        public float Red;

        public RoomLightPreset(float coldWhite, float warmWhite, float red)
        {
            ColdWhite = coldWhite;
            WarmWhite = warmWhite;
            Red = red;
        }

        public override readonly string ToString()
        {
            return $"RoomLightPreset({ColdWhite}, {WarmWhite}, {Red})";
        }
    }
}
