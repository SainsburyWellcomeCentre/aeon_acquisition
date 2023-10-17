using Bonsai.Design;
using Bonsai.Expressions;
using System;
using System.Windows.Forms;

namespace Aeon.Foraging
{
    public class DispenserEventVisualizer : DialogTypeVisualizer
    {
        DispenserEventControl control;
        IDisposable stateSubscription;

        public override void Load(IServiceProvider provider)
        {
            var context = (ITypeVisualizerContext)provider.GetService(typeof(ITypeVisualizerContext));
            var visualizerElement = ExpressionBuilder.GetVisualizerElement(context.Source);
            var source = (DispenserController)ExpressionBuilder.GetWorkflowElement(visualizerElement.Builder);

            control = new DispenserEventControl(source);
            control.Dock = DockStyle.Fill;
            control.HandleDestroyed += delegate { stateSubscription?.Dispose(); };
            control.HandleCreated += delegate
            {
                stateSubscription = source.State.ObserveOn(control).Subscribe(state =>
                {
                    if (state != null)
                    {
                        control.Value = state.Value;
                    }
                });
            };

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
