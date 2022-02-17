using System;
using System.Windows.Forms;

namespace Aeon.Acquisition
{
    public partial class AnnotationControl : UserControl
    {
        public AnnotationControl(AnnotationSource source)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            InitializeComponent();
        }

        public AnnotationSource Source { get; }

        private void OnAnnotation(PriorityLevel priority)
        {
            if (string.IsNullOrEmpty(annotationsTextBox.Text))
            {
                return;
            }

            var metadata = new LogMessage(priority, annotationsTextBox.Text);
            annotationsTextBox.Text = string.Empty;
            Source.OnNext(metadata);
        }

        private void annotationButton_Click(object sender, EventArgs e)
        {
            OnAnnotation(PriorityLevel.Notification);
        }

        private void alertButton_Click(object sender, EventArgs e)
        {
            OnAnnotation(PriorityLevel.Alert);
        }

        private void annotationBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Enter)
            {
                OnAnnotation(PriorityLevel.Notification);
                e.SuppressKeyPress = true;
            }
        }
    }
}
