using System;
using Bonsai.Design;
using Bonsai.Expressions;
using System.Windows.Forms;
using Bonsai.Harp;

namespace Aeon.Environment
{
    public class EnvironmentSubjectStateVisualizer : DialogTypeVisualizer
    {
        EnvironmentSubjectStateControl control;

        public override void Load(IServiceProvider provider)
        {
            var context = (ITypeVisualizerContext)provider.GetService(typeof(ITypeVisualizerContext));
            var visualizerElement = ExpressionBuilder.GetVisualizerElement(context.Source);
            var source = (EnvironmentSubjectState)ExpressionBuilder.GetWorkflowElement(visualizerElement.Builder);

            control = new EnvironmentSubjectStateControl(source, provider);
            control.Dock = DockStyle.Fill;
            if (source.State != null)
            {
                foreach (var subject in source.State.ActiveSubjects)
                {
                    control.AddSubject(subject);
                }
            }

            var visualizerService = (IDialogTypeVisualizerService)provider.GetService(typeof(IDialogTypeVisualizerService));
            if (visualizerService != null)
            {
                visualizerService.AddControl(control);
            }
        }

        private void UpdateSubject(EnvironmentSubjectStateMetadata metadata)
        {
            switch (metadata.Type)
            {
                case EnvironmentSubjectChangeType.Remain:
                case EnvironmentSubjectChangeType.Enter:
                    control.AddSubject(new EnvironmentSubjectStateEntry
                    {
                        Id = metadata.Id,
                        ReferenceWeight = metadata.ReferenceWeight,
                        Weight = metadata.Weight,
                        Type = EnvironmentSubjectChangeType.Enter
                    });
                    break;
                case EnvironmentSubjectChangeType.Exit:
                    control.RemoveSubject(metadata.Id);
                    break;
                default:
                    break;
            }
        }

        public override void Show(object value)
        {
            if (value is EnvironmentSubjectStateMetadata metadata)
            {
                UpdateSubject(metadata);
            }
            else if (value is Timestamped<EnvironmentSubjectStateMetadata> timestampedMetadata)
            {
                UpdateSubject(timestampedMetadata.Value);
            }
        }

        public override void Unload()
        {
            control.Dispose();
            control = null;
        }
    }
}