using Aeon.Acquisition;
using Aeon.Environment;
using Aeon.Foraging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aeon.Tests
{
    [TestClass]
    public class OperatorTests
    {
        [TestMethod]
        public void Build_Workflows()
        {
            var acquisition = typeof(CreateTimestamped).Assembly;
            var environment = typeof(EnvironmentState).Assembly;
            var foraging = typeof(WheelDisplacement).Assembly;
            AssertWorkflow.CanBuildEmbeddedResources(acquisition);
            AssertWorkflow.CanBuildEmbeddedResources(environment);
            AssertWorkflow.CanBuildEmbeddedResources(foraging);
        }
    }
}
