using Bonsai;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using Bonsai.Sleap;
using Bonsai.Harp;

namespace Aeon.Acquisition
{
    [Combinator]
    [Description("Converts a timestamped pose collection into a sequence of Harp messages.")]
    [WorkflowElementCategory(ElementCategory.Transform)]
    public class FormatPose
    {
        [Description("The address of the formatted Harp message.")]
        public int Address { get; set; } = 255;

        [Description("Optional value to override or set the identity index returned by the source pose.")]
        public int? IdentityIndex { get; set; }

        public IObservable<HarpMessage> Process(IObservable<Timestamped<PoseIdentity>> source)
        {
            return source.Select(payload =>
            {
                var i = 0;
                var pose = payload.Value;
                var timestamp = payload.Seconds;
                var data = new float[5 + pose.Count * 3];
                data[i++] = IdentityIndex.GetValueOrDefault(pose.IdentityIndex);
                data[i++] = pose.Confidence;
                data[i++] = pose.Centroid.Position.X;
                data[i++] = pose.Centroid.Position.Y;
                data[i++] = pose.Centroid.Confidence;
                foreach (var bp in pose)
                {
                    data[i++] = bp.Position.X;
                    data[i++] = bp.Position.Y;
                    data[i++] = bp.Confidence;
                }
                return HarpMessage.FromSingle(Address, timestamp, MessageType.Event, data);
            });
        }

        public IObservable<HarpMessage> Process(IObservable<Timestamped<PoseIdentityCollection>> source)
        {
            return source.SelectMany(payload =>
            {
                var poseCollection = payload.Value;
                var timestamp = payload.Seconds;
                return poseCollection.Select((pose, index) =>
                {
                    int i = 0;
                    var data = new float[5 + pose.Count * 3];
                    data[i++] = IdentityIndex.HasValue
                        ? IdentityIndex.GetValueOrDefault() + index
                        : pose.IdentityIndex;
                    data[i++] = pose.Confidence;
                    data[i++] = pose.Centroid.Position.X;
                    data[i++] = pose.Centroid.Position.Y;
                    data[i++] = pose.Centroid.Confidence;
                    foreach (var bp in pose)
                    {
                        data[i++] = bp.Position.X;
                        data[i++] = bp.Position.Y;
                        data[i++] = bp.Confidence;
                    }
                    return HarpMessage.FromSingle(Address, timestamp, MessageType.Event, data);
                });
            });
        }

        public IObservable<HarpMessage> Process(IObservable<Timestamped<Pose>> source)
        {
            return source.Select(payload =>
            {
                int i = 0;
                var pose = payload.Value;
                var timestamp = payload.Seconds;
                var data = new float[5 + pose.Count * 3];
                data[i++] = IdentityIndex.GetValueOrDefault(-1);
                data[i++] = float.NaN;
                data[i++] = pose.Centroid.Position.X;
                data[i++] = pose.Centroid.Position.Y;
                data[i++] = pose.Centroid.Confidence;
                foreach (var bp in pose)
                {
                    data[i++] = bp.Position.X;
                    data[i++] = bp.Position.Y;
                    data[i++] = bp.Confidence;
                }
                return HarpMessage.FromSingle(Address, timestamp, MessageType.Event, data);
            });
        }

        public IObservable<HarpMessage> Process(IObservable<Timestamped<PoseCollection>> source)
        {
            return source.SelectMany(payload =>
            {
                var poseCollection = payload.Value;
                var timestamp = payload.Seconds;
                return poseCollection.Select((pose, index) =>
                {
                    int i = 0;
                    var data = new float[5 + pose.Count * 3];
                    data[i++] = IdentityIndex.GetValueOrDefault() + index;
                    data[i++] = float.NaN;
                    data[i++] = pose.Centroid.Position.X;
                    data[i++] = pose.Centroid.Position.Y;
                    data[i++] = pose.Centroid.Confidence;
                    foreach (var bp in pose)
                    {
                        data[i++] = bp.Position.X;
                        data[i++] = bp.Position.Y;
                        data[i++] = bp.Confidence;
                    }
                    return HarpMessage.FromSingle(Address, timestamp, MessageType.Event, data);
                });
            });
        }
    }
}
