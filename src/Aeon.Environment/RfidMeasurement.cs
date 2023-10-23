using OpenCV.Net;

namespace Aeon.Environment
{
    public struct RfidMeasurement
    {
        public Point2f Location;
        public ulong TagId;

        public RfidMeasurement(Point2f location, ulong tagId)
        {
            Location = location;
            TagId = tagId;
        }

        public override readonly string ToString()
        {
            return $"{nameof(RfidMeasurement)}(Location: {Location}, TagId: {TagId})";
        }
    }
}
