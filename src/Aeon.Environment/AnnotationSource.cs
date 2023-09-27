using Bonsai;
using System.ComponentModel;
using Aeon.Acquisition;

namespace Aeon.Environment
{
    [TypeVisualizer(typeof(AnnotationSourceVisualizer))]
    [Description("Generates a sequence of manual annotations or alerts about the experiment.")]
    public class AnnotationSource : MetadataSource<LogMessage>
    {
    }
}
