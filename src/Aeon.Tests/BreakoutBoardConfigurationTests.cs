using System;
using System.Linq;
using System.Reactive.Linq;
using Aeon.Ephys;
using Bonsai;
using Bonsai.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenEphys.Onix1;

namespace Aeon.Tests
{
    [TestClass]
    public class BreakoutBoardConfigurationTests
    {
        [Combinator]
        class ConfigurationSource
        {
            public IObservable<BreakoutBoardConfiguration> Process()
            {
                return Observable.Empty<BreakoutBoardConfiguration>();
            }
        }

        [Combinator]
        class ContextSource
        {
            public IObservable<ContextTask> Process()
            {
                return Observable.Empty<ContextTask>();
            }
        }

        static ConfigureBreakoutBoard CreateReference()
        {
            return new ConfigureBreakoutBoard();
        }

        static ConfigureBreakoutBoard CreateConverted()
        {
            return (ConfigureBreakoutBoard)new BreakoutBoardConfiguration();
        }

        [TestMethod]
        public void ConversionAddressesEveryDeviceAsBoardDoes()
        {
            // The breakout is hub zero with fixed addresses, so every converted device must
            // carry the address the device it replaced had. Compared against the board rather
            // than against the address table, which belongs to ONIX.
            var reference = CreateReference();
            var converted = CreateConverted();
            Assert.AreEqual(reference.Heartbeat.DeviceAddress, converted.Heartbeat.DeviceAddress);
            Assert.AreEqual(reference.AnalogIO.DeviceAddress, converted.AnalogIO.DeviceAddress);
            Assert.AreEqual(reference.DigitalIO.DeviceAddress, converted.DigitalIO.DeviceAddress);
            Assert.AreEqual(reference.ClockOutput.DeviceAddress, converted.ClockOutput.DeviceAddress);
            Assert.AreEqual(reference.HarpInput.DeviceAddress, converted.HarpInput.DeviceAddress);
            Assert.AreEqual(reference.MemoryMonitor.DeviceAddress, converted.MemoryMonitor.DeviceAddress);
        }

        [TestMethod]
        public void ConversionNamesEveryDeviceAsBoardDoes()
        {
            // Device names bind the board to the monitoring workflows, and are derived from
            // the board name, so the conversion has to assign the name after the devices.
            var reference = CreateReference();
            var converted = CreateConverted();
            Assert.AreEqual(reference.Heartbeat.DeviceName, converted.Heartbeat.DeviceName);
            Assert.AreEqual(reference.AnalogIO.DeviceName, converted.AnalogIO.DeviceName);
            Assert.AreEqual(reference.DigitalIO.DeviceName, converted.DigitalIO.DeviceName);
            Assert.AreEqual(reference.ClockOutput.DeviceName, converted.ClockOutput.DeviceName);
            Assert.AreEqual(reference.HarpInput.DeviceName, converted.HarpInput.DeviceName);
            Assert.AreEqual(reference.MemoryMonitor.DeviceName, converted.MemoryMonitor.DeviceName);
        }

        [TestMethod]
        public void DeviceAssignedAfterNameLosesName()
        {
            // Records the ordering hazard the conversion is placed to avoid: assigning a device
            // clears the name the board stamped on it, and only the name setter stamps it back.
            var breakout = CreateReference();
            var derivedName = breakout.HarpInput.DeviceName;

            breakout.HarpInput = (ConfigureHarpSyncInput)new HarpSyncInputConfiguration();

            Assert.AreNotEqual(derivedName, breakout.HarpInput.DeviceName);
        }

        [TestMethod]
        public void ConversionCopiesConfiguredSettings()
        {
            // Settings survive the conversion across every child device, including the integer
            // narrowing the generated model requires and the per-channel analog lists.
            var configuration = new BreakoutBoardConfiguration();
            configuration.Heartbeat.BeatsPerSecond = 25;
            configuration.AnalogIo.Enable = false;
            // The generated constructors start every list empty, so the lists are built as
            // a YAML file listing all twelve channels would populate them.
            configuration.AnalogIo.InputRange.AddRange(
                Enumerable.Repeat(AnalogIOVoltageRange.TenVolts, 12));
            configuration.AnalogIo.InputRange[3] = AnalogIOVoltageRange.FiveVolts;
            configuration.AnalogIo.Direction.AddRange(Enumerable.Repeat(AnalogIODirection.Input, 12));
            configuration.AnalogIo.Direction[11] = AnalogIODirection.Output;
            configuration.DigitalIo.DeadTime = 4.5;
            configuration.DigitalIo.SampleRate = 1000;
            configuration.ClockOutput.ClockGate = true;
            configuration.ClockOutput.Frequency = 2e6;
            configuration.ClockOutput.DutyCycle = 25;
            configuration.ClockOutput.Delay = 1.5;
            configuration.HarpInput.Enable = false;
            configuration.HarpInput.Source = HarpSyncSource.Breakout;
            configuration.MemoryMonitor.Enable = false;
            configuration.MemoryMonitor.SamplesPerSecond = 2;

            var breakout = (ConfigureBreakoutBoard)configuration;

            Assert.AreEqual(25u, breakout.Heartbeat.BeatsPerSecond);
            Assert.IsFalse(breakout.AnalogIO.Enable);
            Assert.AreEqual(AnalogIOVoltageRange.FiveVolts, breakout.AnalogIO.InputRange3);
            Assert.AreEqual(AnalogIOVoltageRange.TenVolts, breakout.AnalogIO.InputRange4);
            Assert.AreEqual(AnalogIODirection.Output, breakout.AnalogIO.Direction11);
            Assert.AreEqual(AnalogIODirection.Input, breakout.AnalogIO.Direction0);
            Assert.AreEqual(4.5, breakout.DigitalIO.DeadTime);
            Assert.AreEqual(1000, breakout.DigitalIO.SampleRate);
            Assert.IsTrue(breakout.ClockOutput.ClockGate);
            Assert.AreEqual(2e6, breakout.ClockOutput.Frequency);
            Assert.AreEqual(25, breakout.ClockOutput.DutyCycle);
            Assert.AreEqual(1.5, breakout.ClockOutput.Delay);
            Assert.IsFalse(breakout.HarpInput.Enable);
            Assert.AreEqual(HarpSyncSource.Breakout, breakout.HarpInput.Source);
            Assert.IsFalse(breakout.MemoryMonitor.Enable);
            Assert.AreEqual(2u, breakout.MemoryMonitor.SamplesPerSecond);
        }

        [TestMethod]
        public void ConversionRejectsNegativeRates()
        {
            // Nothing in the generated model or its deserializers enforces the schema minimum.
            // A negative value read from a file must fail rather than wrap to a large rate.
            var heartbeat = new HeartbeatConfiguration { BeatsPerSecond = -1 };
            var memoryMonitor = new MemoryMonitorConfiguration { SamplesPerSecond = -1 };

            var heartbeatError = Assert.Throws<ArgumentOutOfRangeException>(
                () => (ConfigurePersistentHeartbeat)heartbeat);
            var memoryMonitorError = Assert.Throws<ArgumentOutOfRangeException>(
                () => (ConfigureMemoryMonitor)memoryMonitor);
            Assert.AreEqual(nameof(HeartbeatConfiguration.BeatsPerSecond), heartbeatError.ParamName);
            Assert.AreEqual(nameof(MemoryMonitorConfiguration.SamplesPerSecond), memoryMonitorError.ParamName);
        }

        [TestMethod]
        public void DefaultsEnableMemoryMonitorAndKeepBoardName()
        {
            // The memory monitor is off in ONIX by default, and the OnixMemoryUsage subject and
            // its alert are silent without it. The board name is not configurable, since the
            // monitoring workflows ask for its devices by name.
            var breakout = CreateConverted();
            Assert.IsTrue(breakout.MemoryMonitor.Enable);
            Assert.AreEqual(BreakoutBoardConfiguration.BreakoutBoardName, breakout.Name);
            Assert.AreEqual(CreateReference().Name, breakout.Name);
        }

        [TestMethod]
        public void ShortChannelListsFallBackToDefaults()
        {
            // The schema requires twelve entries per list, but the YAML deserializer does not
            // enforce it, so a short list must not throw and must leave the rest at default.
            var configuration = new BreakoutBoardConfiguration();
            configuration.AnalogIo.InputRange.Clear();
            configuration.AnalogIo.InputRange.Add(AnalogIOVoltageRange.TwoPointFiveVolts);
            configuration.AnalogIo.Direction = null;

            var analog = (ConfigureAnalogIO)configuration.AnalogIo;

            Assert.AreEqual(AnalogIOVoltageRange.TwoPointFiveVolts, analog.InputRange0);
            Assert.AreEqual(AnalogIOVoltageRange.TenVolts, analog.InputRange1);
            Assert.AreEqual(AnalogIODirection.Input, analog.Direction5);
        }

        [TestMethod]
        public void MappedConfigurationBuildsDeviceOperator()
        {
            // A single property mapping carries the whole configuration, so no ordering between
            // mappings decides whether the devices are named correctly.
            var workflow = new ExpressionBuilderGraph();
            var context = workflow.Add(new CombinatorBuilder { Combinator = new ContextSource() });
            var configuration = workflow.Add(new CombinatorBuilder { Combinator = new ConfigurationSource() });
            var mapping = workflow.Add(new PropertyMappingBuilder
            {
                PropertyMappings =
                {
                    new PropertyMapping { Name = nameof(ConfigureBreakoutBoardDevices.Configuration) }
                }
            });
            var devices = workflow.Add(new CombinatorBuilder { Combinator = new ConfigureBreakoutBoardDevices() });
            workflow.AddEdge(configuration, mapping, new ExpressionBuilderArgument());
            workflow.AddEdge(mapping, devices, new ExpressionBuilderArgument());
            workflow.AddEdge(context, devices, new ExpressionBuilderArgument());

            Assert.IsNotNull(workflow.Build());
        }
    }
}
