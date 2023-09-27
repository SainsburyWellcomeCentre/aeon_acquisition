using System;
using System.Reactive;
using System.Windows.Forms;

namespace Aeon.Environment
{
    public partial class TareWeightControl : UserControl
    {
        public TareWeightControl(TareWeight source)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            InitializeComponent();
        }

        public TareWeight Source { get; }

        private void tareButton_Click(object sender, EventArgs e)
        {
            Source.OnNext(Unit.Default);
        }
    }
}
