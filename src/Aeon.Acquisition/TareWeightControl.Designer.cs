namespace Aeon.Acquisition
{
    partial class TareWeightControl
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
            this.tarePanel = new System.Windows.Forms.Panel();
            this.tareButton = new System.Windows.Forms.Button();
            this.tarePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tarePanel
            // 
            this.tarePanel.Controls.Add(this.tareButton);
            this.tarePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tarePanel.Location = new System.Drawing.Point(0, 0);
            this.tarePanel.Margin = new System.Windows.Forms.Padding(6);
            this.tarePanel.Name = "tarePanel";
            this.tarePanel.Size = new System.Drawing.Size(400, 240);
            this.tarePanel.TabIndex = 10;
            // 
            // tareButton
            // 
            this.tareButton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tareButton.Location = new System.Drawing.Point(6, 6);
            this.tareButton.Margin = new System.Windows.Forms.Padding(6);
            this.tareButton.Name = "tareButton";
            this.tareButton.Size = new System.Drawing.Size(388, 228);
            this.tareButton.TabIndex = 0;
            this.tareButton.Text = "Tare";
            this.tareButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.tareButton.UseVisualStyleBackColor = true;
            this.tareButton.Click += new System.EventHandler(this.tareButton_Click);
            // 
            // TareWeightControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tarePanel);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "TareWeightControl";
            this.Size = new System.Drawing.Size(400, 240);
            this.tarePanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel tarePanel;
        private System.Windows.Forms.Button tareButton;
    }
}
