using Bonsai;
using Bonsai.Design;
using System;
using System.Drawing;

namespace Aeon.Acquisition
{
    sealed class ExternalizedPropertiesVisualizer : DialogTypeVisualizer
    {
        PropertyGrid control;

        public override void Load(IServiceProvider provider)
        {
            var workflowBuilder = (WorkflowBuilder)provider.GetService(typeof(WorkflowBuilder));

            control = new PropertyGrid();
            control.Font = new Font(control.Font.FontFamily, 16.2F);
            control.Dock = System.Windows.Forms.DockStyle.Fill;
            control.SelectedObject = workflowBuilder.Workflow;
            control.Size = new Size(400, 450);

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
