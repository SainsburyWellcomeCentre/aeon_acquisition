using System.Reactive.Linq;
using Aeon.Ephys;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aeon.Tests
{
    [TestClass]
    public class GetProbeEnableTests
    {
        static bool IsEnabled(EphysConfiguration configuration, ProbeName probe)
        {
            var enable = new GetProbeEnable { ProbeName = probe };
            return enable.Process(Observable.Return(configuration)).Wait();
        }

        [TestMethod]
        public void ReadsEachProbeFlagOfNeuropixelsV2e()
        {
            // The watchdog arms a probe's timeouts on this flag, so a disabled probe must
            // read false while its neighbour still reads true.
            var headstage = new NeuropixelsV2eConfiguration();
            headstage.ProbeB.Enable = false;
            var configuration = new EphysConfiguration { Headstage = headstage };

            Assert.IsTrue(IsEnabled(configuration, ProbeName.ProbeA));
            Assert.IsFalse(IsEnabled(configuration, ProbeName.ProbeB));
        }

        [TestMethod]
        public void ReadsEachProbeFlagOfNeuropixelsV2eBeta()
        {
            var headstage = new NeuropixelsV2eBetaConfiguration();
            headstage.ProbeA.Enable = false;
            var configuration = new EphysConfiguration { Headstage = headstage };

            Assert.IsFalse(IsEnabled(configuration, ProbeName.ProbeA));
            Assert.IsTrue(IsEnabled(configuration, ProbeName.ProbeB));
        }

        [TestMethod]
        public void HeadstageWithoutProbesIsNeverEnabled()
        {
            // The 64-channel headstage streams its amplifier as ProbeA but has no probe
            // settings, so the probe timeouts stay disarmed rather than fire on it.
            var configuration = new EphysConfiguration { Headstage = new Headstage64Configuration() };

            Assert.IsFalse(IsEnabled(configuration, ProbeName.ProbeA));
            Assert.IsFalse(IsEnabled(configuration, ProbeName.ProbeB));
        }
    }
}
