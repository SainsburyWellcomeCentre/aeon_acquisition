using Bonsai.Design;
using Bonsai.Expressions;
using System;
using System.Windows.Forms;

namespace Aeon.Environment
{
    public class ButtonSourceVisualizer : DialogTypeVisualizer
    {
        ButtonControl control;

        public override void Load(IServiceProvider provider)
        {
            var context = (ITypeVisualizerContext)provider.GetService(typeof(ITypeVisualizerContext));
            var visualizerElement = ExpressionBuilder.GetVisualizerElement(context.Source);
            var source = (ButtonSource)ExpressionBuilder.GetWorkflowElement(visualizerElement.Builder);

            control = new ButtonControl(source);
            control.Dock = DockStyle.Fill;

            var visualizerService = (IDialogTypeVisualizerService)provider.GetService(typeof(IDialogTypeVisualizerService));
            if (visualizerService != null)
            {
                visualizerService.AddControl(control);
            }
        }

        public override void Show(object value)
        {
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
