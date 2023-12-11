using System;
using System.Windows.Forms;

namespace Aeon.Environment
{
    partial class ButtonControl : UserControl
    {
        public ButtonControl(ButtonSource source)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            InitializeComponent();
            var text = Source.Text;
            if (!string.IsNullOrEmpty(text))
            {
                button.Text = text;
            }
        }

        public ButtonSource Source { get; }

        private void button_Click(object sender, EventArgs e)
        {
            Source.OnNext(button.Text);
        }
    }
}
