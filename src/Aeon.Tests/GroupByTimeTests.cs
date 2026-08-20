using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Xml;
using System.Xml.Linq;
using Aeon.Acquisition;
using Bonsai;
using Bonsai.Expressions;
using Bonsai.Harp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aeon.Tests
{
    [TestClass]
    public class GroupByTimeTests
    {
        static string SerializeWorkflow(GroupByTime operatorInstance)
        {
            using var writer = new StringWriter();
            var builder = new WorkflowBuilder { Workflow = { new CombinatorBuilder { Combinator = operatorInstance } } };
            WorkflowBuilder.Serializer.Serialize(writer, builder);
            return writer.ToString();
        }

        static GroupByTime DeserializeWorkflow(string xml)
        {
            using var reader = XmlReader.Create(new StringReader(xml));
            reader.MoveToContent();
            var builder = (WorkflowBuilder)WorkflowBuilder.Serializer.Deserialize(reader);
            return (GroupByTime)ExpressionBuilder.GetWorkflowElement(builder.Workflow.Single().Value);
        }

        sealed class GroupTracker<TElement>
        {
            public List<DateTime> Opened { get; } = new();

            public List<DateTime> Completed { get; } = new();

            public IDisposable Subscribe(IObservable<IGroupedObservable<DateTime, TElement>> grouped)
            {
                return grouped.Subscribe(group =>
                {
                    Opened.Add(group.Key);
                    group.Subscribe(_ => { }, () => Completed.Add(group.Key));
                });
            }
        }

        [TestMethod]
        public void Process_ClosingDurationZero_ClosesChunkWhenNextChunkArrives()
        {
            var source = new Subject<Timestamped<int>>();
            var groupByTime = new GroupByTime { ChunkSize = 1, ClosingDuration = TimeSpan.Zero };
            var tracker = new GroupTracker<Timestamped<int>>();
            using (tracker.Subscribe(groupByTime.Process(source)))
            {
                source.OnNext(Timestamped.Create(1, 0));
                source.OnNext(Timestamped.Create(2, 1800));
                Assert.IsEmpty(tracker.Completed);
                source.OnNext(Timestamped.Create(3, 3660));
                Assert.HasCount(1, tracker.Completed);
                Assert.AreEqual(tracker.Opened[0], tracker.Completed[0]);
                source.OnCompleted();
            }

            Assert.HasCount(2, tracker.Opened);
            Assert.AreNotEqual(tracker.Opened[0], tracker.Opened[1]);
            Assert.AreSequenceEqual(tracker.Opened, tracker.Completed);
        }

        [TestMethod]
        public void Process_ClosingDurationNull_KeepsChunksOpenUntilSourceCompletes()
        {
            var source = new Subject<Timestamped<int>>();
            var groupByTime = new GroupByTime { ChunkSize = 1, ClosingDuration = null };
            var tracker = new GroupTracker<Timestamped<int>>();
            using (tracker.Subscribe(groupByTime.Process(source)))
            {
                source.OnNext(Timestamped.Create(1, 0));
                source.OnNext(Timestamped.Create(2, 3660));
                Assert.IsEmpty(tracker.Completed);
                source.OnCompleted();
            }

            Assert.HasCount(2, tracker.Opened);
            Assert.HasCount(2, tracker.Completed);
        }

        [TestMethod]
        public void Process_ClosingDurationZero_ReopensChunkForLateElement()
        {
            var source = new Subject<Timestamped<int>>();
            var groupByTime = new GroupByTime { ChunkSize = 1, ClosingDuration = TimeSpan.Zero };
            var tracker = new GroupTracker<Timestamped<int>>();
            using (tracker.Subscribe(groupByTime.Process(source)))
            {
                source.OnNext(Timestamped.Create(1, 0));
                source.OnNext(Timestamped.Create(2, 3660));
                source.OnNext(Timestamped.Create(3, 60));
                source.OnCompleted();
            }

            Assert.HasCount(3, tracker.Opened);
            Assert.AreNotEqual(tracker.Opened[0], tracker.Opened[1]);
            Assert.AreEqual(tracker.Opened[0], tracker.Opened[2]);
        }

        [TestMethod]
        public void Process_ClosingDurationGrace_KeepsChunkOpenWithinGrace()
        {
            var source = new Subject<Timestamped<int>>();
            var groupByTime = new GroupByTime { ChunkSize = 1, ClosingDuration = TimeSpan.FromMinutes(30) };
            var tracker = new GroupTracker<Timestamped<int>>();
            using (tracker.Subscribe(groupByTime.Process(source)))
            {
                source.OnNext(Timestamped.Create(1, 0));
                source.OnNext(Timestamped.Create(2, 3600));
                Assert.IsEmpty(tracker.Completed);
                source.OnNext(Timestamped.Create(3, 5460));
                Assert.HasCount(1, tracker.Completed);
                Assert.AreEqual(tracker.Opened[0], tracker.Completed[0]);
                source.OnCompleted();
            }
        }

        [TestMethod]
        public void Process_TupleSource_ClosesChunkWhenNextChunkArrives()
        {
            var source = new Subject<Tuple<int, double>>();
            var groupByTime = new GroupByTime { ChunkSize = 1, ClosingDuration = TimeSpan.Zero };
            var tracker = new GroupTracker<Timestamped<int>>();
            using (tracker.Subscribe(groupByTime.Process(source)))
            {
                source.OnNext(Tuple.Create(1, 0.0));
                source.OnNext(Tuple.Create(2, 3660.0));
                Assert.HasCount(1, tracker.Completed);
                Assert.AreEqual(tracker.Opened[0], tracker.Completed[0]);
                source.OnCompleted();
            }
        }

        [TestMethod]
        public void Process_HarpMessageSource_ClosesChunkWhenNextChunkArrives()
        {
            var source = new Subject<HarpMessage>();
            var groupByTime = new GroupByTime { ChunkSize = 1, ClosingDuration = TimeSpan.Zero };
            var tracker = new GroupTracker<HarpMessage>();
            using (tracker.Subscribe(groupByTime.Process(source)))
            {
                source.OnNext(HarpMessage.FromByte(32, 0.0, MessageType.Event, 1));
                source.OnNext(HarpMessage.FromByte(32, 3660.0, MessageType.Event, 2));
                Assert.HasCount(1, tracker.Completed);
                Assert.AreEqual(tracker.Opened[0], tracker.Completed[0]);
                source.OnCompleted();
            }
        }

        [TestMethod]
        public void Serialization_ExplicitNull_RoundTripsAsNull()
        {
            var xml = SerializeWorkflow(new GroupByTime { ClosingDuration = null });
            Assert.Contains("nil", xml);
            Assert.IsNull(DeserializeWorkflow(xml).ClosingDuration);
        }

        [TestMethod]
        public void Serialization_Value_RoundTripsAsValue()
        {
            var restored = DeserializeWorkflow(SerializeWorkflow(new GroupByTime { ClosingDuration = TimeSpan.Zero }));
            Assert.AreEqual(TimeSpan.Zero, restored.ClosingDuration);
        }

        [TestMethod]
        public void Deserialization_AbsentElement_UsesConstructorDefault()
        {
            var document = XDocument.Parse(SerializeWorkflow(new GroupByTime()));
            document.Descendants().Where(element => element.Name.LocalName == "ClosingDuration").Remove();
            var restored = DeserializeWorkflow(document.ToString());
            Assert.AreEqual(new GroupByTime().ClosingDuration, restored.ClosingDuration);
        }

        [TestMethod]
        public void Constructor_DefaultsClosingDurationToNull()
        {
            Assert.IsNull(new GroupByTime().ClosingDuration);
        }

        [TestMethod]
        public void Process_HeartbeatAdvancesPastChunk_ClosesChunk()
        {
            var source = new Subject<Timestamped<int>>();
            var heartbeats = new Subject<HarpMessage>();
            var groupByTime = new GroupByTime { ChunkSize = 1, ClosingDuration = TimeSpan.Zero };
            var tracker = new GroupTracker<Timestamped<int>>();
            using (tracker.Subscribe(groupByTime.Process(source, heartbeats)))
            {
                source.OnNext(Timestamped.Create(1, 0));
                heartbeats.OnNext(HarpMessage.FromByte(32, 1800.0, MessageType.Event, 1));
                Assert.IsEmpty(tracker.Completed);
                heartbeats.OnNext(HarpMessage.FromByte(32, 3660.0, MessageType.Event, 2));
                Assert.HasCount(1, tracker.Completed);
                Assert.AreEqual(tracker.Opened[0], tracker.Completed[0]);
                source.OnCompleted();
                heartbeats.OnCompleted();
            }
        }

        [TestMethod]
        public void Process_HeartbeatCompletesWithNullClosingDuration_KeepsChunksOpenUntilSourceCompletes()
        {
            var source = new Subject<Timestamped<int>>();
            var heartbeats = new Subject<HarpMessage>();
            var groupByTime = new GroupByTime { ChunkSize = 1, ClosingDuration = null };
            var tracker = new GroupTracker<Timestamped<int>>();
            using (tracker.Subscribe(groupByTime.Process(source, heartbeats)))
            {
                source.OnNext(Timestamped.Create(1, 0));
                source.OnNext(Timestamped.Create(2, 3660));
                heartbeats.OnNext(HarpMessage.FromByte(32, 7260.0, MessageType.Event, 1));
                heartbeats.OnCompleted();
                Assert.IsEmpty(tracker.Completed);
                source.OnCompleted();
            }

            Assert.HasCount(2, tracker.Opened);
            Assert.HasCount(2, tracker.Completed);
        }

        [TestMethod]
        public void Process_HeartbeatCompletesBeforeSource_RaisesError()
        {
            var source = new Subject<Timestamped<int>>();
            var heartbeats = new Subject<HarpMessage>();
            var groupByTime = new GroupByTime { ChunkSize = 1, ClosingDuration = TimeSpan.Zero };
            Exception error = null;
            using (groupByTime.Process(source, heartbeats).Subscribe(_ => { }, exception => error = exception))
            {
                source.OnNext(Timestamped.Create(1, 0));
                heartbeats.OnCompleted();
            }

            Assert.IsInstanceOfType<InvalidOperationException>(error);
        }

        [TestMethod]
        public void Process_SourceCompletesWithoutHeartbeat_CompletesWithoutError()
        {
            // A source without a heartbeat is its own clock, so the clock terminating is the source
            // terminating and must not be reported as a heartbeat that stopped early.
            var source = new Subject<Timestamped<int>>();
            var groupByTime = new GroupByTime { ChunkSize = 1, ClosingDuration = TimeSpan.Zero };
            Exception error = null;
            var completed = false;
            using (groupByTime.Process(source).Subscribe(_ => { }, exception => error = exception, () => completed = true))
            {
                source.OnNext(Timestamped.Create(1, 0));
                source.OnNext(Timestamped.Create(2, 3660));
                source.OnCompleted();
            }

            Assert.IsNull(error);
            Assert.IsTrue(completed);
        }

        [TestMethod]
        public void Process_ChunkSizeChangedAfterProcessCall_UsesCapturedChunkSize()
        {
            var source = new Subject<Timestamped<int>>();
            var groupByTime = new GroupByTime { ChunkSize = 1, ClosingDuration = TimeSpan.Zero };
            var tracker = new GroupTracker<Timestamped<int>>();
            var grouped = groupByTime.Process(source);
            groupByTime.ChunkSize = 2;
            using (tracker.Subscribe(grouped))
            {
                source.OnNext(Timestamped.Create(1, 0));
                source.OnNext(Timestamped.Create(2, 3660));
                source.OnCompleted();
            }

            Assert.HasCount(2, tracker.Opened);
            Assert.AreNotEqual(tracker.Opened[0], tracker.Opened[1]);
        }
    }
}
