using System;
using System.Windows.Forms;

namespace Aeon.Acquisition
{
    public partial class DispenserStateControl : UserControl
    {
        int currentValue;

        public DispenserStateControl(DispenserState source)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            InitializeComponent();

            var dispenserName = Source.Name;
            if (!string.IsNullOrEmpty(dispenserName))
            {
                dispenserGroupBox.Text = $"{dispenserName} Dispenser";
            }
        }

        public DispenserState Source { get; }

        public int Value
        {
            get { return currentValue; }
            set
            {
                currentValue = value;
                currentValueLabel.Text = currentValue.ToString();
            }
        }

        private void refillButton_Click(object sender, EventArgs e)
        {
            var metadata = new DispenserStateMetadata(Source.Name, (int)refillUpDown.Value, DispenserEventType.Refill);
            Source.OnNext(metadata);
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            var delta = (int)refillUpDown.Value - Value;
            var metadata = new DispenserStateMetadata(Source.Name, delta, DispenserEventType.Reset);
            Source.OnNext(metadata);
        }
    }
}
