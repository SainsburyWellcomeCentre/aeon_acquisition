using System;
using Bonsai.Design;
using Bonsai.Expressions;
using System.Windows.Forms;

namespace Aeon.Acquisition
{
    public class SubjectChangeVisualizer : DialogTypeVisualizer
    {
        SubjectChangeControl control;

        public override void Load(IServiceProvider provider)
        {
            var context = (ITypeVisualizerContext)provider.GetService(typeof(ITypeVisualizerContext));
            var visualizerElement = ExpressionBuilder.GetVisualizerElement(context.Source);
            var source = (SubjectChange)ExpressionBuilder.GetWorkflowElement(visualizerElement.Builder);

            control = new SubjectChangeControl(source);
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