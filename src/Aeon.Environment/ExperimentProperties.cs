using Bonsai;
using Bonsai.Expressions;
using System.ComponentModel;

namespace Aeon.Environment
{
    [TypeVisualizer(typeof(ExternalizedPropertiesVisualizer))]
    [Description("Provides a configurable visualizer for global workflow properties.")]
    public class ExperimentProperties : UnitBuilder
    {
        [Description("Specifies whether the help text box is visible.")]
        public bool HelpVisible { get; set; } = true;

        [Description("Specifies whether the toolbar is visible.")]
        public bool ToolbarVisible { get; set; } = true;
    }
}
