using Bonsai;
using Bonsai.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Xml;

namespace Aeon.Acquisition.Tests
{
    [TestClass]
    public class OperatorTests
    {
        [TestMethod]
        public void Build_Workflows()
        {
            var assembly = typeof(CreateTimestamped).Assembly;
            foreach (var name in assembly.GetManifestResourceNames())
            {
                if (Path.GetExtension(name) != ".bonsai")
                {
                    continue;
                }

                using (var workflowStream = assembly.GetManifestResourceStream(name))
                using (var reader = XmlReader.Create(workflowStream))
                {
                    reader.MoveToContent();
                    var workflowBuilder = (WorkflowBuilder)WorkflowBuilder.Serializer.Deserialize(reader);
                    workflowBuilder.Workflow.Convert(builder =>
                    {
                        var workflowElement = ExpressionBuilder.GetWorkflowElement(builder);
                        if (workflowElement.GetType().Name != nameof(AeonCapture))
                        {
                            Assert.IsNotInstanceOfType(workflowElement, typeof(UnknownTypeBuilder));
                        }
                        return builder;
                    }, recurse: true);
                }
            }
        }
    }
}
