using Bonsai;
using System.ComponentModel;
using System.Reactive;

namespace Aeon.Acquisition
{
    [TypeVisualizer(typeof(TareWeightVisualizer))]
    [Description("Generates a sequence of commands to tare a weight scale.")]
    public class TareWeight : MetadataSource<Unit>
    {
    }
}
