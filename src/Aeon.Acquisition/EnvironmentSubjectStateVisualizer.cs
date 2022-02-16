using System;
using Bonsai.Design;
using Bonsai.Expressions;
using System.Windows.Forms;

namespace Aeon.Acquisition
{
    public class EnvironmentSubjectStateVisualizer : DialogTypeVisualizer
    {
        EnvironmentSubjectStateControl control;

        public override void Load(IServiceProvider provider)
        {
            var context = (ITypeVisualizerContext)provider.GetService(typeof(ITypeVisualizerContext));
            var visualizerElement = ExpressionBuilder.GetVisualizerElement(context.Source);
            var source = (EnvironmentSubjectState)ExpressionBuilder.GetWorkflowElement(visualizerElement.Builder);

            control = new EnvironmentSubjectStateControl(source);
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
            control.Dispose();
            control = null;
        }
    }
}