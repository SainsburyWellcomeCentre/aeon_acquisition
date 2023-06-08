using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Bonsai.Sleap;
using Bonsai.Harp;
using OpenCV.Net;
using System.Drawing;
using Bonsai.Reactive;

namespace Aeon.Acquisition 
{
        [Combinator]
        [Description("Converts a timestamped sleap pose collection into a sequence of Harp messages. " +
        "The output will take the form of: ArgMaxId, IdConf, N*[PartA.X, PartA.Y, PartA.Conf]")]
        [WorkflowElementCategory(ElementCategory.Transform)]
        public class FormatSleapPose
        {
            public int Address { get; set; } = 255;

            [Description("Optional value to override the ArgMax value returned by the predict Sleap operator.")]
            public int? Id { get; set; }

            public IObservable<HarpMessage> Process(IObservable<Tuple<PoseIdentity,double>> source)
            {
                return source.Select(value => {
                    var pose = value.Item1;
                    var timestamp = value.Item2;
                    var data = new float[2 + pose.Count*3];
                    data[0] = Id == null ? (float) pose.IdentityIndex : (float) Id;
                    data[1] = pose.Confidence;
                    int i = 1;
                    foreach (var bp in (pose))
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
                    data[0] = Id == null ? (float)pose.IdentityIndex : (float) Id;
                    data[1] = pose.Confidence;
                    int i = 1;
                    foreach (var bp in (pose))
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
                return source.Select(value => {
                    var pose = value.Item1;
                    var timestamp = value.Item2;
                    var data = new float[2 + pose.Count * 3];
                    data[0] = Id == null ? -1F : (float) Id;
                    data[1] = float.NaN;
                    int i = 1;
                    foreach (var bp in (pose))
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
                    data[0] = Id == null ? index : (float) Id;
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
