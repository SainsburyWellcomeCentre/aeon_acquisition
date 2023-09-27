using Bonsai;
using System.ComponentModel;
using System.Reactive;
using Aeon.Acquisition;

namespace Aeon.Environment
{
    [TypeVisualizer(typeof(TareWeightVisualizer))]
    [Description("Generates a sequence of commands to tare a weight scale.")]
    public class TareWeight : MetadataSource<Unit>
    {
    }
}
