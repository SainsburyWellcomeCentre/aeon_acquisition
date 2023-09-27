using Bonsai;
using System.Xml;
using System.Reflection;
using System.IO;
using Bonsai.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Aeon.Acquisition;

namespace Aeon.Tests
{
    public static class AssertWorkflow
    {
        public static void CanBuildEmbeddedResources(Assembly assembly)
        {
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
                        if (workflowElement.GetType().Name != nameof(SpinnakerCapture) &&
                            workflowElement.GetType().Name != nameof(PylonCapture) &&
#pragma warning disable CS0612 // Type or member is obsolete
                            workflowElement.GetType().Name != nameof(AeonCapture))
#pragma warning restore CS0612 // Type or member is obsolete
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
