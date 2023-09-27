namespace Aeon.Foraging
{
    partial class DispenserStateControl
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
            this.dispenserPanel = new System.Windows.Forms.Panel();
            this.dispenserGroupBox = new System.Windows.Forms.GroupBox();
            this.currentValueLabel = new System.Windows.Forms.Label();
            this.currentLabel = new System.Windows.Forms.Label();
            this.refillUpDown = new System.Windows.Forms.NumericUpDown();
            this.refillButton = new System.Windows.Forms.Button();
            this.resetButton = new System.Windows.Forms.Button();
            this.dispenserPanel.SuspendLayout();
            this.dispenserGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.refillUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // dispenserPanel
            // 
            this.dispenserPanel.Controls.Add(this.resetButton);
            this.dispenserPanel.Controls.Add(this.dispenserGroupBox);
            this.dispenserPanel.Controls.Add(this.refillButton);
            this.dispenserPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dispenserPanel.Location = new System.Drawing.Point(0, 0);
            this.dispenserPanel.Margin = new System.Windows.Forms.Padding(6);
            this.dispenserPanel.Name = "dispenserPanel";
            this.dispenserPanel.Size = new System.Drawing.Size(400, 240);
            this.dispenserPanel.TabIndex = 10;
            // 
            // dispenserGroupBox
            // 
            this.dispenserGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dispenserGroupBox.Controls.Add(this.currentValueLabel);
            this.dispenserGroupBox.Controls.Add(this.currentLabel);
            this.dispenserGroupBox.Controls.Add(this.refillUpDown);
            this.dispenserGroupBox.Location = new System.Drawing.Point(12, 12);
            this.dispenserGroupBox.Margin = new System.Windows.Forms.Padding(12);
            this.dispenserGroupBox.Name = "dispenserGroupBox";
            this.dispenserGroupBox.Padding = new System.Windows.Forms.Padding(12);
            this.dispenserGroupBox.Size = new System.Drawing.Size(375, 153);
            this.dispenserGroupBox.TabIndex = 4;
            this.dispenserGroupBox.TabStop = false;
            this.dispenserGroupBox.Text = "Dispenser";
            // 
            // currentValueLabel
            // 
            this.currentValueLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.currentValueLabel.AutoSize = true;
            this.currentValueLabel.Location = new System.Drawing.Point(145, 43);
            this.currentValueLabel.Name = "currentValueLabel";
            this.currentValueLabel.Size = new System.Drawing.Size(31, 32);
            this.currentValueLabel.TabIndex = 2;
            this.currentValueLabel.Text = "0";
            // 
            // currentLabel
            // 
            this.currentLabel.AutoSize = true;
            this.currentLabel.Location = new System.Drawing.Point(15, 43);
            this.currentLabel.Name = "currentLabel";
            this.currentLabel.Size = new System.Drawing.Size(124, 32);
            this.currentLabel.TabIndex = 1;
            this.currentLabel.Text = "Current: ";
            // 
            // refillUpDown
            // 
            this.refillUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.refillUpDown.Location = new System.Drawing.Point(15, 100);
            this.refillUpDown.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.refillUpDown.Name = "refillUpDown";
            this.refillUpDown.Size = new System.Drawing.Size(345, 38);
            this.refillUpDown.TabIndex = 0;
            // 
            // refillButton
            // 
            this.refillButton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.refillButton.Location = new System.Drawing.Point(12, 175);
            this.refillButton.Margin = new System.Windows.Forms.Padding(12);
            this.refillButton.Name = "refillButton";
            this.refillButton.Size = new System.Drawing.Size(176, 52);
            this.refillButton.TabIndex = 5;
            this.refillButton.Text = "Refill";
            this.refillButton.UseVisualStyleBackColor = true;
            this.refillButton.Click += new System.EventHandler(this.refillButton_Click);
            // 
            // resetButton
            // 
            this.resetButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.resetButton.Location = new System.Drawing.Point(212, 175);
            this.resetButton.Margin = new System.Windows.Forms.Padding(12);
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(175, 52);
            this.resetButton.TabIndex = 6;
            this.resetButton.Text = "Reset";
            this.resetButton.UseVisualStyleBackColor = true;
            this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
            // 
            // DispenserStateControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dispenserPanel);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "DispenserStateControl";
            this.Size = new System.Drawing.Size(400, 240);
            this.dispenserPanel.ResumeLayout(false);
            this.dispenserGroupBox.ResumeLayout(false);
            this.dispenserGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.refillUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel dispenserPanel;
        private System.Windows.Forms.GroupBox dispenserGroupBox;
        private System.Windows.Forms.Button refillButton;
        private System.Windows.Forms.NumericUpDown refillUpDown;
        private System.Windows.Forms.Label currentValueLabel;
        private System.Windows.Forms.Label currentLabel;
        private System.Windows.Forms.Button resetButton;
    }
}
