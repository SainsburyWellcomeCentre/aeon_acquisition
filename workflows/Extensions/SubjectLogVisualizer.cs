using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Bonsai.Design;
using Bonsai.Expressions;
using System.Windows.Forms;
using System.Drawing;

public class SubjectLogVisualizer : DialogTypeVisualizer
{
    TableLayoutPanel panel;

    public override void Load(IServiceProvider provider)
    {
        var context = (ITypeVisualizerContext)provider.GetService(typeof(ITypeVisualizerContext));
        var visualizerElement = ExpressionBuilder.GetVisualizerElement(context.Source);
        var source = (SubjectLog)ExpressionBuilder.GetWorkflowElement(visualizerElement.Builder);

        panel = new TableLayoutPanel();
        panel.Font = new System.Drawing.Font(panel.Font.FontFamily, panel.Font.SizeInPoints * 2);
        panel.Dock = DockStyle.Fill;
        panel.ColumnCount = 1;
        panel.RowCount = 4;
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        panel.Size = new System.Drawing.Size(400, 800);

        var metadata = new LogMetadata();
        var propertyGrid = new Bonsai.Design.PropertyGrid();
        var button = new Button();
        button.Text = metadata.Event.ToString();
        button.Dock = DockStyle.Fill;
        button.Click += delegate
        {
            if (string.IsNullOrWhiteSpace(metadata.Id))
            {
                MessageBox.Show("A valid subject ID is required to log event data.",
                    "SubjectLog",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            source.OnNext(new LogMetadata
            {
                Id = metadata.Id,
                Weight = metadata.Weight,
                Event = metadata.Event
            });

            if (metadata.Event == EventType.Start)
            {
                metadata.Event = EventType.Stop;
            }
            else if (metadata.Event == EventType.Stop)
            {
                metadata.Event = EventType.End;
            }
            else
            {
                propertyGrid.SelectedObject = metadata = new LogMetadata();
            }

            button.Text = metadata.Event.ToString();
        };
        panel.Controls.Add(button);

        propertyGrid.HelpVisible = false;
        propertyGrid.Dock = DockStyle.Fill;
        propertyGrid.SelectedObject = metadata;
        panel.Controls.Add(propertyGrid);
        
        var annotationButton = new Button();
        var annotationBox = new TextBox();
        annotationButton.Text = "Annotate";
        annotationButton.Dock = DockStyle.Fill;
        annotationButton.Click += delegate
        {
            if (string.IsNullOrWhiteSpace(metadata.Id))
            {
                MessageBox.Show("A valid subject ID is required to log event data.",
                    "SubjectLog",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            source.OnNext(new LogMetadata
            {
                Id = metadata.Id,
                Weight = metadata.Weight,
                Event = EventType.Annotation,
                Annotation = annotationBox.Text
            });
        };

        annotationBox.Dock = DockStyle.Fill;
        annotationBox.Multiline = true;
        annotationBox.WordWrap = true;
        panel.Controls.Add(annotationButton);
        panel.Controls.Add(annotationBox);

        var visualizerService = (IDialogTypeVisualizerService)provider.GetService(typeof(IDialogTypeVisualizerService));
        if (visualizerService != null)
        {
            visualizerService.AddControl(panel);
        }
    }

    public override void Show(object value)
    {
    }

    public override void Unload()
    {
        panel.Dispose();
        panel = null;
    }
}