namespace Aeon.Environment
{
    partial class EnvironmentStateControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.maintenancePanel = new System.Windows.Forms.Panel();
            this.maintenanceButton = new System.Windows.Forms.CheckBox();
            this.maintenancePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // maintenancePanel
            // 
            this.maintenancePanel.Controls.Add(this.maintenanceButton);
            this.maintenancePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.maintenancePanel.Location = new System.Drawing.Point(0, 0);
            this.maintenancePanel.Margin = new System.Windows.Forms.Padding(6);
            this.maintenancePanel.Name = "maintenancePanel";
            this.maintenancePanel.Size = new System.Drawing.Size(400, 240);
            this.maintenancePanel.TabIndex = 10;
            // 
            // maintenanceButton
            // 
            this.maintenanceButton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.maintenanceButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.maintenanceButton.Location = new System.Drawing.Point(6, 6);
            this.maintenanceButton.Margin = new System.Windows.Forms.Padding(6);
            this.maintenanceButton.Name = "maintenanceButton";
            this.maintenanceButton.Size = new System.Drawing.Size(388, 228);
            this.maintenanceButton.TabIndex = 0;
            this.maintenanceButton.Text = "Maintenance";
            this.maintenanceButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.maintenanceButton.UseVisualStyleBackColor = true;
            this.maintenanceButton.CheckedChanged += new System.EventHandler(this.maintenanceButton_CheckedChanged);
            // 
            // EnvironmentStateControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.maintenancePanel);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "EnvironmentStateControl";
            this.Size = new System.Drawing.Size(400, 240);
            this.maintenancePanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel maintenancePanel;
        private System.Windows.Forms.CheckBox maintenanceButton;
    }
}
