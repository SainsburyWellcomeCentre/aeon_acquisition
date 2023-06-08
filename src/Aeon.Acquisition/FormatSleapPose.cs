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
    public class FormatSleapPose
    {
        [Description("The address of the formatted Harp message.")]
        public int Address { get; set; } = 255;

        [Description("Optional value to override or set the identity index returned by the source pose.")]
        public int? IdentityIndex { get; set; }

        public IObservable<HarpMessage> Process(IObservable<Tuple<PoseIdentity, double>> source)
        {
            return source.Select(value =>
            {
                var pose = value.Item1;
                var timestamp = value.Item2;
                var data = new float[2 + pose.Count * 3];
                data[0] = IdentityIndex == null ? pose.IdentityIndex : (float)IdentityIndex;
                data[1] = pose.Confidence;
                int i = 1;
                foreach (var bp in pose)
                {
                    data[++i] = bp.Position.X;
                    data[++i] = bp.Position.Y;
                    data[++i] = bp.Confidence;
                }
                return HarpMessage.FromSingle(Address, timestamp, MessageType.Event, data);
            });
        }

        public IObservable<HarpMessage> Process(IObservable<Tuple<PoseIdentityCollection, double>> source)
        {
            return source.SelectMany(value =>
            {
                var poseCollection = value.Item1;
                var timestamp = value.Item2;
                return poseCollection.Select((pose, index) =>
                {
                    var data = new float[2 + pose.Count * 3];
                    data[0] = IdentityIndex == null ? pose.IdentityIndex : (float)IdentityIndex;
                    data[1] = pose.Confidence;
                    int i = 1;
                    foreach (var bp in pose)
                    {
                        data[++i] = bp.Position.X;
                        data[++i] = bp.Position.Y;
                        data[++i] = bp.Confidence;
                    }
                    return HarpMessage.FromSingle(Address, timestamp, MessageType.Event, data);
                });
            });
        }

        public IObservable<HarpMessage> Process(IObservable<Tuple<Pose, double>> source)
        {
            return source.Select(value =>
            {
                var pose = value.Item1;
                var timestamp = value.Item2;
                var data = new float[2 + pose.Count * 3];
                data[0] = IdentityIndex == null ? -1f : (float)IdentityIndex;
                data[1] = float.NaN;
                int i = 1;
                foreach (var bp in pose)
                {
                    data[++i] = bp.Position.X;
                    data[++i] = bp.Position.Y;
                    data[++i] = bp.Confidence;
                }
                return HarpMessage.FromSingle(Address, timestamp, MessageType.Event, data);
            });
        }

        public IObservable<HarpMessage> Process(IObservable<Tuple<PoseCollection, double>> source)
        {
            return source.SelectMany(value =>
            {
                var poseCollection = value.Item1;
                var timestamp = value.Item2;
                return poseCollection.Select((pose, index) =>
                {
                    var data = new float[2 + pose.Count * 3];
                    data[0] = IdentityIndex == null ? index : (float)IdentityIndex;
                    data[1] = float.NaN;
                    int i = 1;
                    foreach (var bp in pose)
                    {
                        data[++i] = bp.Position.X;
                        data[++i] = bp.Position.Y;
                        data[++i] = bp.Confidence;
                    }
                    return HarpMessage.FromSingle(Address, timestamp, MessageType.Event, data);
                });
            });
        }
    }
}
