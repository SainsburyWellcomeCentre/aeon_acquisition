using OpenCV.Net;

namespace Aeon.Video
{
    public class VideoDataFrame
    {
        public VideoDataFrame(IplImage image, long frameID, long timestamp)
            : this(image, new VideoChunkData(frameID, timestamp))
        {
        }

        public VideoDataFrame(IplImage image, VideoChunkData chunkData)
        {
            Image = image;
            ChunkData = chunkData;
        }

        public IplImage Image { get; }

        public VideoChunkData ChunkData { get; }
    }

    public struct VideoChunkData
    {
        public VideoChunkData(long frameID, long timestamp)
        {
            FrameID = frameID;
            Timestamp = timestamp;
        }

        public long FrameID { get; }

        public long Timestamp { get; }
    }
}
