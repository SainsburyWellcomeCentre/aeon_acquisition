using Bonsai.Design;
using Bonsai.Expressions;
using Bonsai.Harp;
using System;
using System.Windows.Forms;

namespace Aeon.Acquisition
{
    public class DispenserStateVisualizer : DialogTypeVisualizer
    {
        DispenserStateControl control;

        public override void Load(IServiceProvider provider)
        {
            var context = (ITypeVisualizerContext)provider.GetService(typeof(ITypeVisualizerContext));
            var visualizerElement = ExpressionBuilder.GetVisualizerElement(context.Source);
            var source = (DispenserState)ExpressionBuilder.GetWorkflowElement(visualizerElement.Builder);

            control = new DispenserStateControl(source);
            control.Dock = DockStyle.Fill;
            var state = source.State;
            if (state != null) control.Value = state.Value;

            var visualizerService = (IDialogTypeVisualizerService)provider.GetService(typeof(IDialogTypeVisualizerService));
            if (visualizerService != null)
            {
                visualizerService.AddControl(control);
            }
        }

        public override void Show(object value)
        {
            if (value is DispenserStateMetadata metadata)
            {
                control.Value = metadata.Value;
            }
            else if (value is Timestamped<DispenserStateMetadata> timestampedMetadata)
            {
                control.Value = timestampedMetadata.Value.Value;
            }
        }

        public override void Unload()
        {
            if (control != null)
            {
                control.Dispose();
                control = null;
            }
        }
    }
}
