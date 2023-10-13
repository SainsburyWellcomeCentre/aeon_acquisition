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
                        if (workflowElement is IncludeWorkflowBuilder includeWorkflow)
                        {
                            var pathComponents = includeWorkflow.Path.Split(':');
                            var assembly = Assembly.Load(pathComponents[0]);
                            var resourceName = string.Join(ExpressionHelper.MemberSeparator, pathComponents);
                            Assert.IsNotNull(
                                assembly.GetManifestResourceStream(resourceName),
                                $"Embedded workflow: {name}. Missing resource name: {resourceName}");
                        }
                        else if (workflowElement is BinaryOperatorBuilder binaryOperator &&
                                 binaryOperator.Operand is WorkflowProperty operand &&
                                 operand.GetType().IsGenericType)
                        {
                            var valueType = operand.GetType().GetGenericArguments()[0];
                            Assert.IsFalse(
                                typeof(UnknownTypeBuilder).IsAssignableFrom(valueType),
                                $"Binary operator operand is unknown: {valueType}. Embedded workflow: {name}.");
                        }

                        if (workflowElement.GetType().Name != nameof(SpinnakerCapture) &&
                            workflowElement.GetType().Name != nameof(PylonCapture) &&
#pragma warning disable CS0612 // Type or member is obsolete
                            workflowElement.GetType().Name != nameof(AeonCapture))
#pragma warning restore CS0612 // Type or member is obsolete
                        {
                            Assert.IsNotInstanceOfType(
                                workflowElement,
                                typeof(UnknownTypeBuilder),
                                $"Embedded workflow: {name}.");
                        }
                        return builder;
                    }, recurse: true);
                }
            }
        }
    }
}
