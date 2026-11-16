using Aeon.Ephys;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenEphys.Onix1;

namespace Aeon.Tests
{
    [TestClass]
    public class NeuropixelsV2eBetaConfigurationTests
    {
        static NeuropixelsV2eBetaConfiguration CreateConfiguration()
        {
            return new NeuropixelsV2eBetaConfiguration
            {
                Name = "Headstage",
                Port = PortName.PortB,
                TrackingLed = true,
                ProbeA = new NeuropixelsV2BetaProbeConfiguration
                {
                    Enable = true,
                    EnableLed = false,
                    ProbeConfiguration = new NeuropixelsV2QuadShankConfiguration
                    {
                        Reference = NeuropixelsV2QuadShankReference.Tip1
                    }
                },
                ProbeB = new NeuropixelsV2BetaProbeConfiguration { Enable = false }
            };
        }

        [TestMethod]
        public void ConversionCopiesProbeSettings()
        {
            // The beta headstage carries the indicator LED setting of each probe and its own
            // tracking LED setting, alongside the electrode settings shared with the other probes.
            var configuration = CreateConfiguration();
            var headstage = (ConfigureHeadstageNeuropixelsV2eBeta)configuration;
            var probe = headstage.NeuropixelsV2A;
            var electrodes = (NeuropixelsV2QuadShankProbeConfiguration)probe.ProbeConfiguration;
            Assert.AreEqual(configuration.TrackingLed, headstage.TrackingLed);
            Assert.AreEqual(configuration.ProbeA.Enable, probe.Enable);
            Assert.AreEqual(configuration.ProbeA.EnableLed, probe.EnableLed);
            Assert.AreEqual(configuration.ProbeA.ProbeConfiguration.Reference, electrodes.Reference);
            Assert.AreEqual(configuration.ProbeB.Enable, headstage.NeuropixelsV2B.Enable);
        }

        [TestMethod]
        public void DevicesAssignedBeforePortDeriveAddressAndName()
        {
            // The converted devices are assigned before the port and group name, so the headstage
            // addresses and names them itself, identically to the devices they replaced.
            var reference = new ConfigureHeadstageNeuropixelsV2eBeta();
            reference.Name = "Headstage";
            reference.Port = PortName.PortB;

            var headstage = (ConfigureHeadstageNeuropixelsV2eBeta)CreateConfiguration();

            Assert.AreEqual(reference.NeuropixelsV2A.DeviceAddress, headstage.NeuropixelsV2A.DeviceAddress);
            Assert.AreEqual(reference.NeuropixelsV2A.DeviceName, headstage.NeuropixelsV2A.DeviceName);
            Assert.AreEqual(reference.NeuropixelsV2B.DeviceAddress, headstage.NeuropixelsV2B.DeviceAddress);
            Assert.AreEqual(reference.NeuropixelsV2B.DeviceName, headstage.NeuropixelsV2B.DeviceName);
            Assert.AreEqual(reference.Bno055.DeviceAddress, headstage.Bno055.DeviceAddress);
            Assert.AreEqual(reference.Bno055.DeviceName, headstage.Bno055.DeviceName);
        }
    }
}
