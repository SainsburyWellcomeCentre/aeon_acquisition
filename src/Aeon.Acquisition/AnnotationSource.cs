using Bonsai;
using System.ComponentModel;

namespace Aeon.Acquisition
{
    [TypeVisualizer(typeof(AnnotationSourceVisualizer))]
    [Description("Generates a sequence of manual annotations or alerts about the experiment.")]
    public class AnnotationSource : MetadataSource<LogMessage>
    {
    }
}
