using Aeon.Ephys;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenEphys.Onix1;

namespace Aeon.Tests
{
    [TestClass]
    public class HarpSyncInputConfigurationTests
    {
        static HarpSyncInputConfiguration CreateConfiguration()
        {
            return new HarpSyncInputConfiguration
            {
                Enable = false,
                Source = HarpSyncSource.Breakout
            };
        }

        [TestMethod]
        public void ConversionCopiesConfiguredSettings()
        {
            // Every setting the configuration declares reaches the device it converts to.
            var configuration = CreateConfiguration();
            var device = (ConfigureHarpSyncInput)configuration;
            Assert.AreEqual(configuration.Enable, device.Enable);
            Assert.AreEqual(configuration.Source, device.Source);
        }

        [TestMethod]
        public void DeviceAssignedBeforeNameDerivesAddressAndName()
        {
            // The operator assigns the board name after the converted device, so the breakout
            // addresses and names it itself, identically to the device it replaced.
            var reference = new ConfigureBreakoutBoard();
            var breakout = new ConfigureBreakoutBoardDevices { HarpInput = CreateConfiguration() }
                .CreateConfiguration();
            Assert.AreEqual(reference.HarpInput.DeviceAddress, breakout.HarpInput.DeviceAddress);
            Assert.AreEqual(reference.HarpInput.DeviceName, breakout.HarpInput.DeviceName);
        }

        [TestMethod]
        public void DeviceAssignedAfterNameLosesName()
        {
            // Records the ordering hazard the operator is placed to avoid: assigning a device
            // clears the name the board stamped on it, and only the name setter stamps it back.
            var breakout = new ConfigureBreakoutBoard();
            var derivedName = breakout.HarpInput.DeviceName;

            breakout.HarpInput = (ConfigureHarpSyncInput)CreateConfiguration();

            Assert.AreNotEqual(derivedName, breakout.HarpInput.DeviceName);
        }

        [TestMethod]
        public void ConfiguredBreakoutEnablesMemoryMonitor()
        {
            // The memory monitor is off by default, and the OnixMemoryUsage subject and its
            // alert are silent without it.
            var breakout = new ConfigureBreakoutBoardDevices().CreateConfiguration();
            Assert.IsTrue(breakout.MemoryMonitor.Enable);
        }
    }
}
