using Aeon.Acquisition;
using Aeon.Environment;
using Aeon.Foraging;
using Aeon.Video;
using Aeon.Vision;
using Aeon.Vision.Sleap;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aeon.Tests
{
    [TestClass]
    public class OperatorTests
    {
        [TestMethod]
        public void Build_Workflows()
        {
            var acquisition = typeof(GroupByTime).Assembly;
            var environment = typeof(EnvironmentState).Assembly;
            var foraging = typeof(WheelDisplacement).Assembly;
            var video = typeof(VideoDataFrame).Assembly;
            var vision = typeof(DistanceFromPoint).Assembly;
            var sleap = typeof(FormatPose).Assembly;
            AssertWorkflow.CanBuildEmbeddedResources(acquisition);
            AssertWorkflow.CanBuildEmbeddedResources(environment);
            AssertWorkflow.CanBuildEmbeddedResources(foraging);
            AssertWorkflow.CanBuildEmbeddedResources(video);
            AssertWorkflow.CanBuildEmbeddedResources(vision);
            AssertWorkflow.CanBuildEmbeddedResources(sleap);
        }
    }
}
