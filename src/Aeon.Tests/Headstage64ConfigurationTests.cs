using System;
using System.Reactive.Linq;
using Aeon.Ephys;
using Bonsai;
using Bonsai.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenEphys.Onix1;

namespace Aeon.Tests
{
    [TestClass]
    public class Headstage64ConfigurationTests
    {
        const PortName TestPort = PortName.PortB;

        [Combinator]
        class ConfigurationSource
        {
            public IObservable<Headstage64Configuration> Process()
            {
                return Observable.Empty<Headstage64Configuration>();
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

        static ConfigureHeadstage64 CreateReference()
        {
            return new ConfigureHeadstage64 { Port = TestPort };
        }

        static ConfigureHeadstage64 CreateConverted()
        {
            return (ConfigureHeadstage64)new Headstage64Configuration { Port = TestPort };
        }

        [TestMethod]
        public void ConversionAddressesEveryDeviceAsHeadstageDoes()
        {
            // The port assignment in the conversion reaches the converted devices, so each is
            // addressed exactly as the device it replaced. Compared against the headstage rather
            // than against the address arithmetic, which belongs to ONIX.
            var reference = CreateReference();
            var converted = CreateConverted();
            Assert.AreEqual(reference.Rhd2164.DeviceAddress, converted.Rhd2164.DeviceAddress);
            Assert.AreEqual(reference.Bno055.DeviceAddress, converted.Bno055.DeviceAddress);
            Assert.AreEqual(reference.TS4231.DeviceAddress, converted.TS4231.DeviceAddress);
            Assert.AreEqual(
                reference.ElectricalStimulator.DeviceAddress,
                converted.ElectricalStimulator.DeviceAddress);
            Assert.AreEqual(
                reference.OpticalStimulator.DeviceAddress,
                converted.OpticalStimulator.DeviceAddress);
        }

        [TestMethod]
        public void ConversionNamesEveryDeviceAsHeadstageDoes()
        {
            // Device names bind a configuration to its data operators, and are derived from the
            // group name, so the conversion has to assign the name after the devices too.
            var reference = CreateReference();
            var converted = CreateConverted();
            Assert.AreEqual(reference.Rhd2164.DeviceName, converted.Rhd2164.DeviceName);
            Assert.AreEqual(reference.Bno055.DeviceName, converted.Bno055.DeviceName);
            Assert.AreEqual(reference.TS4231.DeviceName, converted.TS4231.DeviceName);
            Assert.AreEqual(
                reference.ElectricalStimulator.DeviceName,
                converted.ElectricalStimulator.DeviceName);
            Assert.AreEqual(
                reference.OpticalStimulator.DeviceName,
                converted.OpticalStimulator.DeviceName);
        }

        [TestMethod]
        public void ConversionCopiesConfiguredSettings()
        {
            // Settings survive the conversion across every child device, including the integer
            // narrowing the generated model requires.
            var configuration = new Headstage64Configuration();
            configuration.Rhd2164.DspCutoff = Rhd2000DspCutoff.Dsp146mHz;
            configuration.Bno055.Enable = false;
            configuration.Ts4231.Enable = false;
            configuration.PortVoltage.Requested = 5.5;
            configuration.ElectricalStimulator.BurstPulseCount = 7;
            configuration.OpticalStimulator.PulsesPerBurst = 3;

            var headstage = (ConfigureHeadstage64)configuration;

            Assert.AreEqual(Rhd2000DspCutoff.Dsp146mHz, headstage.Rhd2164.DspCutoff);
            Assert.IsFalse(headstage.Bno055.Enable);
            Assert.IsFalse(headstage.TS4231.Enable);
            Assert.AreEqual(5.5, headstage.PortVoltage.Requested);
            Assert.AreEqual(7u, headstage.ElectricalStimulator.BurstPulseCount);
            Assert.AreEqual(3u, headstage.OpticalStimulator.PulsesPerBurst);
        }

        [TestMethod]
        public void MappedConfigurationBuildsDeviceOperator()
        {
            // A single property mapping carries the whole configuration, so no ordering between
            // mappings decides whether the devices are addressed correctly.
            var workflow = new ExpressionBuilderGraph();
            var context = workflow.Add(new CombinatorBuilder { Combinator = new ContextSource() });
            var configuration = workflow.Add(new CombinatorBuilder { Combinator = new ConfigurationSource() });
            var mapping = workflow.Add(new PropertyMappingBuilder
            {
                PropertyMappings =
                {
                    new PropertyMapping { Name = nameof(ConfigureHeadstage64Devices.Configuration) }
                }
            });
            var devices = workflow.Add(new CombinatorBuilder { Combinator = new ConfigureHeadstage64Devices() });
            workflow.AddEdge(configuration, mapping, new ExpressionBuilderArgument());
            workflow.AddEdge(mapping, devices, new ExpressionBuilderArgument());
            workflow.AddEdge(context, devices, new ExpressionBuilderArgument());

            Assert.IsNotNull(workflow.Build());
        }
    }
}
