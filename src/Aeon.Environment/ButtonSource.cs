using System.ComponentModel;
using Aeon.Acquisition;
using Bonsai;

namespace Aeon.Environment
{
    [TypeVisualizer(typeof(ButtonSourceVisualizer))]
    [Description("Provides a labeled button control generating a sequence of events for each button click.")]
    public class ButtonSource : MetadataSource<string>
    {
        [Description("Specifies the text associated with this button.")]
        public string Text { get; set; }
    }
}
