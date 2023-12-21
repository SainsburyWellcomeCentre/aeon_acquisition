using System;
using System.Windows.Forms;

namespace Aeon.Foraging
{
    public partial class DispenserEventControl : UserControl
    {
        int currentValue;

        public DispenserEventControl(DispenserController source)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            InitializeComponent();

            var dispenserName = Source.Name;
            if (!string.IsNullOrEmpty(dispenserName))
            {
                dispenserGroupBox.Text = $"{dispenserName}";
            }
        }

        public DispenserController Source { get; }

        public int Value
        {
            get { return currentValue; }
            set
            {
                currentValue = value;
                currentValueLabel.Text = currentValue.ToString();
            }
        }

        private void deliverButton_Click(object sender, EventArgs e)
        {
            OnDispenserEvent(1m, DispenserEventType.Discount);
        }

        private void refillButton_Click(object sender, EventArgs e)
        {
            OnDispenserEvent(refillUpDown.Value, DispenserEventType.Refill);
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            OnDispenserEvent(refillUpDown.Value, DispenserEventType.Reset);
        }

        private void OnDispenserEvent(decimal value, DispenserEventType eventType)
        {
            if (value != 0)
            {
                var metadata = new DispenserEventArgs((int)value, eventType);
                Source.OnNext(metadata);
                refillUpDown.Value = 0;
            }
        }
    }
}
