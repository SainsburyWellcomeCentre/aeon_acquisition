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
    public class Rhd2164ConfigurationTests
    {
        static Rhd2164Configuration CreateConfiguration()
        {
            return new Rhd2164Configuration
            {
                Enable = false,
                AnalogHighCutoff = Rhd2000AnalogHighCutoff.High5000Hz,
                AnalogLowCutoff = Rhd2000AnalogLowCutoff.Low10Hz,
                DspCutoff = Rhd2000DspCutoff.Dsp146mHz
            };
        }

        [Combinator]
        class ConfigurationSource
        {
            public IObservable<Rhd2164Configuration> Process()
            {
                return Observable.Empty<Rhd2164Configuration>();
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

        class UnbridgedConfiguration
        {
            public bool Enable { get; set; }
        }

        [Combinator]
        class UnbridgedSource
        {
            public IObservable<UnbridgedConfiguration> Process()
            {
                return Observable.Empty<UnbridgedConfiguration>();
            }
        }

        static ExpressionBuilderGraph CreateDeviceMapping(ExpressionBuilder source)
        {
            var workflow = new ExpressionBuilderGraph();
            var context = workflow.Add(new CombinatorBuilder { Combinator = new ContextSource() });
            var configuration = workflow.Add(source);
            var mapping = workflow.Add(new PropertyMappingBuilder
            {
                PropertyMappings = { new PropertyMapping { Name = nameof(ConfigureHeadstage64.Rhd2164) } }
            });
            var headstage = workflow.Add(new CombinatorBuilder { Combinator = new ConfigureHeadstage64() });
            workflow.AddEdge(configuration, mapping, new ExpressionBuilderArgument());
            workflow.AddEdge(mapping, headstage, new ExpressionBuilderArgument());
            workflow.AddEdge(context, headstage, new ExpressionBuilderArgument());
            return workflow;
        }

        [TestMethod]
        public void ConversionCopiesConfiguredSettings()
        {
            // Every setting the configuration declares reaches the device it converts to.
            var configuration = CreateConfiguration();
            var device = (ConfigureRhd2164)configuration;
            Assert.AreEqual(configuration.Enable, device.Enable);
            Assert.AreEqual(configuration.AnalogHighCutoff, device.AnalogHighCutoff);
            Assert.AreEqual(configuration.AnalogLowCutoff, device.AnalogLowCutoff);
            Assert.AreEqual(configuration.DspCutoff, device.DspCutoff);
        }

        [TestMethod]
        public void DeviceAssignedBeforePortDerivesAddressAndName()
        {
            // A converted device assigned before the port and group name is addressed and named
            // by the headstage itself, identically to the device it replaced.
            var reference = new ConfigureHeadstage64();
            reference.Name = nameof(ConfigureHeadstage64);
            reference.Port = PortName.PortB;

            var headstage = new ConfigureHeadstage64 { Rhd2164 = (ConfigureRhd2164)CreateConfiguration() };
            headstage.Name = nameof(ConfigureHeadstage64);
            headstage.Port = PortName.PortB;

            Assert.AreEqual(reference.Rhd2164.DeviceAddress, headstage.Rhd2164.DeviceAddress);
            Assert.AreEqual(reference.Rhd2164.DeviceName, headstage.Rhd2164.DeviceName);
        }

        [TestMethod]
        public void DeviceAssignedAfterPortLosesAddress()
        {
            // Records the ordering hazard the conversion is placed to avoid: the port setter
            // addresses the devices present when it runs, so a later assignment discards that.
            var headstage = new ConfigureHeadstage64();
            headstage.Name = nameof(ConfigureHeadstage64);
            headstage.Port = PortName.PortB;
            var derivedAddress = headstage.Rhd2164.DeviceAddress;

            headstage.Rhd2164 = (ConfigureRhd2164)CreateConfiguration();

            Assert.AreNotEqual(derivedAddress, headstage.Rhd2164.DeviceAddress);
        }

        [TestMethod]
        public void PropertyMappingConvertsConfigurationToDevice()
        {
            // The workflow build pipeline finds the conversion, so a configuration value can be
            // mapped onto a device property without an intermediate operator.
            var workflow = CreateDeviceMapping(new CombinatorBuilder { Combinator = new ConfigurationSource() });
            Assert.IsNotNull(workflow.Build());
        }

        [TestMethod]
        public void PropertyMappingWithoutConversionFailsToBuild()
        {
            // Confirms the mapping above succeeds because of the conversion rather than in spite
            // of it, so the test above cannot pass once the conversion is removed.
            var workflow = CreateDeviceMapping(new CombinatorBuilder { Combinator = new UnbridgedSource() });
            Assert.Throws<WorkflowBuildException>(() => workflow.Build());
        }
    }
}
