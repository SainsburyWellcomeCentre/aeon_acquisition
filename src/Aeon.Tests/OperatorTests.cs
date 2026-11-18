using Aeon.Acquisition;
using Aeon.Environment;
using Aeon.Ephys;
using Aeon.Foraging;
using Aeon.Tether.Translation;
using Aeon.Video;
using Aeon.Vision;
using Aeon.Vision.Sleap;
using Bonsai.Expressions;
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
            var translation = typeof(OptimalDrivePosition).Assembly;
            AssertWorkflow.CanLoadEmbeddedResources(acquisition);
            AssertWorkflow.CanLoadEmbeddedResources(environment);
            AssertWorkflow.CanLoadEmbeddedResources(foraging);
            AssertWorkflow.CanLoadEmbeddedResources(vision);
            AssertWorkflow.CanLoadEmbeddedResources(sleap);
            AssertWorkflow.CanLoadEmbeddedResources(ephys);
            AssertWorkflow.CanLoadEmbeddedResources(commutation);
            AssertWorkflow.CanLoadEmbeddedResources(translation);
        }

        [TestMethod]
        public void Build_EphysAcquisition()
        {
            // EphysAcquisition declares the subjects and supplies the headstage input, so
            // building it builds every ephys workflow it includes.
            var ephys = typeof(ConvertToHarpTimestamp).Assembly;
            AssertWorkflow.CanBuildEmbeddedResource(ephys, "Aeon.Ephys.EphysAcquisition.bonsai");
        }

        [TestMethod]
        public void Build_EphysWatchdog()
        {
            // The watchdog subscribes only to subjects EphysAcquisition declares, so the
            // two includes side by side build as a rig workflow would include them.
            var workflow = new ExpressionBuilderGraph();
            workflow.Add(new IncludeWorkflowBuilder { Path = "Aeon.Ephys:EphysAcquisition.bonsai" });
            workflow.Add(new IncludeWorkflowBuilder { Path = "Aeon.Ephys:EphysWatchdog.bonsai" });
            Assert.IsNotNull(workflow.Build());
        }

        [TestMethod, TestCategory("DriverDependent")]
        public void Load_DriverDependentWorkflows()
        {
            var video = typeof(VideoDataFrame).Assembly;
            AssertWorkflow.CanLoadEmbeddedResources(video);
        }
    }
}
