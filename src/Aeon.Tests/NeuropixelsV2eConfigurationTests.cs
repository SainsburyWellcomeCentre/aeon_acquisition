using Aeon.Ephys;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenEphys.Onix1;

namespace Aeon.Tests
{
    [TestClass]
    public class NeuropixelsV2eConfigurationTests
    {
        static NeuropixelsV2eConfiguration CreateConfiguration()
        {
            return new NeuropixelsV2eConfiguration
            {
                Name = "Headstage",
                Port = PortName.PortB,
                ProbeA = new NeuropixelsV2eProbeConfiguration
                {
                    Enable = true,
                    ProbeConfiguration = new NeuropixelsV2QuadShankConfiguration
                    {
                        InvertPolarity = false,
                        Reference = NeuropixelsV2QuadShankReference.Ground,
                        ProbeInterfaceFileName = "probe.json"
                    }
                },
                ProbeB = new NeuropixelsV2eProbeConfiguration { Enable = false },
                Bno055 = new PolledBno055Configuration
                {
                    Enable = false,
                    AxisMap = Bno055AxisMap.ZXY,
                    AxisSign = { Bno055AxisSign.MirrorZ }
                }
            };
        }

        [TestMethod]
        public void ConversionCopiesProbeSettings()
        {
            // Every probe setting the configuration declares reaches the device it converts to,
            // including the electrode settings of the concrete quad-shank probe.
            var configuration = CreateConfiguration();
            var headstage = (ConfigureHeadstageNeuropixelsV2e)configuration;
            var probe = headstage.NeuropixelsV2A;
            var electrodes = (NeuropixelsV2QuadShankProbeConfiguration)probe.ProbeConfiguration;
            var expected = configuration.ProbeA.ProbeConfiguration;
            Assert.AreEqual(configuration.ProbeA.Enable, probe.Enable);
            Assert.AreEqual(expected.InvertPolarity, electrodes.InvertPolarity);
            Assert.AreEqual(expected.Reference, electrodes.Reference);
            Assert.AreEqual(expected.ProbeInterfaceFileName, electrodes.ProbeInterfaceFileName);
            Assert.AreEqual(configuration.ProbeB.Enable, headstage.NeuropixelsV2B.Enable);
        }

        [TestMethod]
        public void ConversionCombinesInertialAxisSigns()
        {
            // The inverted axes are declared one by one, and combine into the flags the device
            // expects, so that a configuration can invert more than one axis.
            var configuration = CreateConfiguration();
            configuration.Bno055.AxisSign.Clear();
            configuration.Bno055.AxisSign.Add(Bno055AxisSign.MirrorY);
            configuration.Bno055.AxisSign.Add(Bno055AxisSign.MirrorX);
            var device = (ConfigurePolledBno055)configuration.Bno055;
            Assert.AreEqual(Bno055AxisSign.MirrorY | Bno055AxisSign.MirrorX, device.AxisSign);
            Assert.AreEqual(configuration.Bno055.AxisMap, device.AxisMap);
            Assert.AreEqual(configuration.Bno055.Enable, device.Enable);
        }

        [TestMethod]
        public void DevicesAssignedBeforePortDeriveAddressAndName()
        {
            // The converted devices are assigned before the port and group name, so the headstage
            // addresses and names them itself, identically to the devices they replaced.
            var reference = new ConfigureHeadstageNeuropixelsV2e();
            reference.Name = "Headstage";
            reference.Port = PortName.PortB;

            var headstage = (ConfigureHeadstageNeuropixelsV2e)CreateConfiguration();

            Assert.AreEqual(reference.NeuropixelsV2A.DeviceAddress, headstage.NeuropixelsV2A.DeviceAddress);
            Assert.AreEqual(reference.NeuropixelsV2A.DeviceName, headstage.NeuropixelsV2A.DeviceName);
            Assert.AreEqual(reference.NeuropixelsV2B.DeviceAddress, headstage.NeuropixelsV2B.DeviceAddress);
            Assert.AreEqual(reference.NeuropixelsV2B.DeviceName, headstage.NeuropixelsV2B.DeviceName);
            Assert.AreEqual(reference.Bno055.DeviceAddress, headstage.Bno055.DeviceAddress);
            Assert.AreEqual(reference.Bno055.DeviceName, headstage.Bno055.DeviceName);
        }

        [TestMethod]
        public void DeviceAssignedAfterPortLosesAddress()
        {
            // Records the ordering hazard the conversion is placed to avoid: the port setter
            // addresses the devices present when it runs, so a later assignment discards that.
            var headstage = new ConfigureHeadstageNeuropixelsV2e();
            headstage.Name = "Headstage";
            headstage.Port = PortName.PortB;
            var derivedAddress = headstage.NeuropixelsV2A.DeviceAddress;

            headstage.NeuropixelsV2A =
                (ConfigureNeuropixelsV2PsbDecoder)CreateConfiguration().ProbeA;

            Assert.AreNotEqual(derivedAddress, headstage.NeuropixelsV2A.DeviceAddress);
        }
    }
}
