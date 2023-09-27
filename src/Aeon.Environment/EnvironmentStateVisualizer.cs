using System;
using Bonsai.Design;
using Bonsai.Expressions;
using System.Windows.Forms;
using Bonsai.Harp;

namespace Aeon.Environment
{
    public class EnvironmentStateVisualizer : DialogTypeVisualizer
    {
        EnvironmentStateControl control;

        public override void Load(IServiceProvider provider)
        {
            var context = (ITypeVisualizerContext)provider.GetService(typeof(ITypeVisualizerContext));
            var visualizerElement = ExpressionBuilder.GetVisualizerElement(context.Source);
            var source = (EnvironmentState)ExpressionBuilder.GetWorkflowElement(visualizerElement.Builder);

            control = new EnvironmentStateControl(source);
            control.Dock = DockStyle.Fill;
            if (source.State != null)
            {
                control.State = source.State.Type;
            }

            var visualizerService = (IDialogTypeVisualizerService)provider.GetService(typeof(IDialogTypeVisualizerService));
            if (visualizerService != null)
            {
                visualizerService.AddControl(control);
            }
        }

        public override void Show(object value)
        {
            if (value is EnvironmentStateMetadata metadata)
            {
                control.State = metadata.Type;
            }
            else if (value is Timestamped<EnvironmentStateMetadata> timestampedMetadata)
            {
                control.State = timestampedMetadata.Value.Type;
            }
        }

        public override void Unload()
        {
            control.Dispose();
            control = null;
        }
    }
}
