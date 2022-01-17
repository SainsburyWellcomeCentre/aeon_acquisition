using System;
using System.Windows.Forms;
using Bonsai.Design;
using System.Drawing;
using Bonsai.Expressions;

namespace Aeon.Acquisition
{
    public class LabelVisualizer : DialogTypeVisualizer
    {
        const int AutoScaleHeight = 13;
        const float DefaultDpi = 96f;

        TextBox textBox;
        UserControl textPanel;

        public override void Show(object value)
        {
            value = value ?? string.Empty;
            textBox.Text = value.ToString();
        }

        public override void Load(IServiceProvider provider)
        {
            var context = (ITypeVisualizerContext)provider.GetService(typeof(ITypeVisualizerContext));
            var visualizerElement = ExpressionBuilder.GetVisualizerElement(context.Source);
            var source = (LabelControl)ExpressionBuilder.GetWorkflowElement(visualizerElement.Builder);

            textBox = new TextBox { Dock = DockStyle.Fill };
            textBox.Font = new Font(textBox.Font.FontFamily, source.FontSize);
            textBox.ReadOnly = true;
            textBox.Multiline = true;
            textBox.WordWrap = true;
            textBox.TextChanged += (sender, e) => textPanel.Invalidate();

            textPanel = new UserControl();
            textPanel.SuspendLayout();
            textPanel.Dock = DockStyle.Fill;
            textPanel.MinimumSize = textPanel.Size = new Size(320, 2 * AutoScaleHeight);
            textPanel.AutoScaleDimensions = new SizeF(6F, AutoScaleHeight);
            textPanel.AutoScaleMode = AutoScaleMode.Font;
            textPanel.Paint += textPanel_Paint;
            textPanel.Controls.Add(textBox);
            textPanel.ResumeLayout(false);

            var visualizerService = (IDialogTypeVisualizerService)provider.GetService(typeof(IDialogTypeVisualizerService));
            if (visualizerService != null)
            {
                visualizerService.AddControl(textPanel);
            }
        }

        void textPanel_Paint(object sender, PaintEventArgs e)
        {
            var lineHeight = AutoScaleHeight * e.Graphics.DpiY / DefaultDpi;
            var textSize = TextRenderer.MeasureText(textBox.Text, textBox.Font);
            if (textBox.ScrollBars == ScrollBars.None && textBox.ClientSize.Width < textSize.Width)
            {
                textBox.ScrollBars = ScrollBars.Horizontal;
                var offset = 2 * lineHeight + SystemInformation.HorizontalScrollBarHeight - textPanel.Height;
                if (offset > 0)
                {
                    textPanel.Parent.Height += (int)offset;
                }
            }
        }

        public override void Unload()
        {
            textBox.Dispose();
            textBox = null;
        }
    }
}