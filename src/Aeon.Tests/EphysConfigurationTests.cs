using System;
using System.Linq.Expressions;
using System.Reactive.Linq;
using Aeon.Ephys;
using Bonsai.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenEphys.Onix1;

namespace Aeon.Tests
{
    [TestClass]
    public class EphysConfigurationTests
    {
        static EphysConfiguration Deserialize(string yaml)
        {
            // Builds the deserializer as a workflow does, so the test exercises the expression
            // the operator emits rather than a method it happens to call.
            var builder = new DeserializeFromYaml { Type = new TypeMapping<EphysConfiguration>() };
            var source = Expression.Constant(Observable.Return(yaml), typeof(IObservable<string>));
            var expression = builder.Build(new[] { source });
            return Expression.Lambda<Func<IObservable<EphysConfiguration>>>(expression)
                .Compile()
                .Invoke()
                .Wait();
        }

        [TestMethod]
        public void NeuropixelsV2eYamlConfiguresDevices()
        {
            // The discriminator selects the headstage, and every setting reaches the ONIX
            // devices the configuration converts into.
            var yaml = string.Join("\n",
                "subject: BA-12345",
                "headstage:",
                "  type: NeuropixelsV2eConfiguration",
                "  port: PortB",
                "  portVoltage:",
                "    requested: 8",
                "  probeA:",
                "    enable: true",
                "    probeConfiguration:",
                "      reference: Ground",
                "      gainCalibrationFileName: gain.csv",
                "  probeB:",
                "    enable: false",
                "  bno055:",
                "    axisMap: YZX",
                "    axisSign: [MirrorY, MirrorX]",
                "harpInput:",
                "  source: ClockAdapter");

            var configuration = Deserialize(yaml);
            Assert.AreEqual("BA-12345", configuration.Subject);
            var headstage = configuration.Headstage as NeuropixelsV2eConfiguration;
            Assert.IsNotNull(headstage);
            Assert.AreEqual(PortName.PortB, headstage.Port);
            Assert.IsFalse(headstage.ProbeB.Enable);

            var device = (ConfigureHeadstageNeuropixelsV2e)headstage;
            Assert.AreEqual("HeadstageNeuropixelsV2e/NeuropixelsV2A", device.NeuropixelsV2A.DeviceName);
            Assert.AreEqual(512u, device.NeuropixelsV2A.DeviceAddress);
            Assert.AreEqual(513u, device.NeuropixelsV2B.DeviceAddress);
            Assert.AreEqual(514u, device.Bno055.DeviceAddress);
            Assert.AreEqual(Bno055AxisSign.MirrorY | Bno055AxisSign.MirrorX, device.Bno055.AxisSign);
            var electrodes = (NeuropixelsV2QuadShankProbeConfiguration)device.NeuropixelsV2A.ProbeConfiguration;
            Assert.AreEqual(NeuropixelsV2QuadShankReference.Ground, electrodes.Reference);
            Assert.AreEqual("gain.csv", electrodes.GainCalibrationFileName);
            Assert.AreEqual(8, device.PortVoltage.Requested);

            var harp = (ConfigureHarpSyncInput)configuration.HarpInput;
            Assert.AreEqual(HarpSyncSource.ClockAdapter, harp.Source);
        }

        [TestMethod]
        public void MinimalYamlUsesDefaults()
        {
            // A file naming only the headstage type gets every default, and the defaults leave
            // the stimulators disarmed and the lighthouse receivers off.
            var yaml = string.Join("\n",
                "subject: BA-12345",
                "headstage:",
                "  type: Headstage64Configuration");

            var configuration = Deserialize(yaml);
            var headstage = configuration.Headstage as Headstage64Configuration;
            Assert.IsNotNull(headstage);
            Assert.AreEqual(PortName.PortA, headstage.Port);
            Assert.AreEqual(3000, headstage.BufferSize);
            var device = (ConfigureHeadstage64)headstage;
            Assert.AreEqual("Headstage64/Rhd2164", device.Rhd2164.DeviceName);
            Assert.AreEqual(256u, device.Rhd2164.DeviceAddress);
            Assert.IsFalse(device.OpticalStimulator.Arm);
            Assert.IsFalse(device.ElectricalStimulator.Arm);
            Assert.IsFalse(device.TS4231.Enable);
        }

        [TestMethod]
        public void BetaYamlCarriesLedSettings()
        {
            var yaml = string.Join("\n",
                "subject: BA-12345",
                "headstage:",
                "  type: NeuropixelsV2eBetaConfiguration",
                "  trackingLed: true",
                "  probeA:",
                "    enableLed: false");

            var configuration = Deserialize(yaml);
            var headstage = configuration.Headstage as NeuropixelsV2eBetaConfiguration;
            Assert.IsNotNull(headstage);
            var device = (ConfigureHeadstageNeuropixelsV2eBeta)headstage;
            Assert.IsTrue(device.TrackingLed);
            Assert.IsFalse(device.NeuropixelsV2A.EnableLed);
            Assert.AreEqual("HeadstageNeuropixelsV2eBeta/NeuropixelsV2A", device.NeuropixelsV2A.DeviceName);
        }
    }
}
