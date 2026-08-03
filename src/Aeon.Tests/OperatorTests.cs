using Aeon.Acquisition;
using Aeon.Environment;
using Aeon.Ephys;
using Aeon.Foraging;
using Aeon.Video;
using Aeon.Vision;
using Aeon.Vision.Sleap;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace Aeon.Tests
{
    [TestClass]
    public class OperatorTests
    {
        [TestMethod]
        public void Load_Workflows()
        {
            var acquisition = typeof(GroupByTime).Assembly;
            var environment = typeof(EnvironmentState).Assembly;
            var foraging = typeof(WheelDisplacement).Assembly;
            var vision = typeof(DistanceFromPoint).Assembly;
            var sleap = typeof(FormatPose).Assembly;
            var ephys = typeof(ConvertToHarpTimestamp).Assembly;
            var commutation = Assembly.Load("Aeon.Tether.Commutation");
            AssertWorkflow.CanLoadEmbeddedResources(acquisition);
            AssertWorkflow.CanLoadEmbeddedResources(environment);
            AssertWorkflow.CanLoadEmbeddedResources(foraging);
            AssertWorkflow.CanLoadEmbeddedResources(vision);
            AssertWorkflow.CanLoadEmbeddedResources(sleap);
            AssertWorkflow.CanLoadEmbeddedResources(ephys);
            AssertWorkflow.CanLoadEmbeddedResources(commutation);
        }

        [TestMethod, TestCategory("DriverDependent")]
        public void Load_DriverDependentWorkflows()
        {
            var video = typeof(VideoDataFrame).Assembly;
            AssertWorkflow.CanLoadEmbeddedResources(video);
        }
    }
}
