using System;
using System.Windows.Forms;

namespace Aeon.Environment
{
    public partial class EnvironmentStateControl : UserControl
    {
        public EnvironmentStateControl(EnvironmentState source)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            InitializeComponent();
        }

        public EnvironmentState Source { get; }

        public EnvironmentStateType State
        {
            get
            {
                return maintenanceButton.Checked
                    ? EnvironmentStateType.Maintenance
                    : EnvironmentStateType.Experiment;
            }
            set
            {
                maintenanceButton.Checked = value == EnvironmentStateType.Maintenance;
                maintenanceButton.Text = $"{(maintenanceButton.Checked ? "Stop" : "Start")} Maintenance";
            }
        }

        private void maintenanceButton_CheckedChanged(object sender, EventArgs e)
        {
            Source.OnNext(new EnvironmentStateMetadata(Source.Name, State));
        }
    }
}
