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

        private void OnAnnotation(AlertType alertType)
        {
            if (string.IsNullOrEmpty(annotationsTextBox.Text))
            {
                return;
            }

            var metadata = new AlertMetadata(alertType, annotationsTextBox.Text);
            annotationsTextBox.Text = string.Empty;
            Source.OnNext(metadata);
        }

        private void annotationButton_Click(object sender, EventArgs e)
        {
            OnAnnotation(AlertType.Annotation);
        }

        private void alertButton_Click(object sender, EventArgs e)
        {
            OnAnnotation(AlertType.PriorityAnnotation);
        }

        private void annotationBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Enter)
            {
                OnAnnotation(AlertType.Annotation);
                e.SuppressKeyPress = true;
            }
        }
    }
}
