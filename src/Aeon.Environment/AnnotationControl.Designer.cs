namespace Aeon.Environment
{
    partial class AnnotationControl
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
            this.annotationsPanel = new System.Windows.Forms.Panel();
            this.annotationsTextBox = new System.Windows.Forms.TextBox();
            this.alertButton = new System.Windows.Forms.Button();
            this.annotationButton = new System.Windows.Forms.Button();
            this.annotationsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // annotationsPanel
            // 
            this.annotationsPanel.Controls.Add(this.annotationsTextBox);
            this.annotationsPanel.Controls.Add(this.alertButton);
            this.annotationsPanel.Controls.Add(this.annotationButton);
            this.annotationsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.annotationsPanel.Location = new System.Drawing.Point(0, 0);
            this.annotationsPanel.Margin = new System.Windows.Forms.Padding(6);
            this.annotationsPanel.Name = "annotationsPanel";
            this.annotationsPanel.Size = new System.Drawing.Size(400, 374);
            this.annotationsPanel.TabIndex = 10;
            // 
            // annotationsTextBox
            // 
            this.annotationsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.annotationsTextBox.Location = new System.Drawing.Point(14, 12);
            this.annotationsTextBox.Margin = new System.Windows.Forms.Padding(12);
            this.annotationsTextBox.Multiline = true;
            this.annotationsTextBox.Name = "annotationsTextBox";
            this.annotationsTextBox.Size = new System.Drawing.Size(374, 296);
            this.annotationsTextBox.TabIndex = 0;
            this.annotationsTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.annotationBox_KeyDown);
            // 
            // alertButton
            // 
            this.alertButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.alertButton.Location = new System.Drawing.Point(227, 313);
            this.alertButton.Margin = new System.Windows.Forms.Padding(12);
            this.alertButton.Name = "alertButton";
            this.alertButton.Size = new System.Drawing.Size(160, 52);
            this.alertButton.TabIndex = 6;
            this.alertButton.Text = "Alert";
            this.alertButton.UseVisualStyleBackColor = true;
            this.alertButton.Click += new System.EventHandler(this.alertButton_Click);
            // 
            // annotationButton
            // 
            this.annotationButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.annotationButton.Location = new System.Drawing.Point(13, 313);
            this.annotationButton.Margin = new System.Windows.Forms.Padding(12);
            this.annotationButton.Name = "annotationButton";
            this.annotationButton.Size = new System.Drawing.Size(190, 52);
            this.annotationButton.TabIndex = 5;
            this.annotationButton.Text = "Annotation";
            this.annotationButton.UseVisualStyleBackColor = true;
            this.annotationButton.Click += new System.EventHandler(this.annotationButton_Click);
            // 
            // AnnotationControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 26F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.annotationsPanel);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "AnnotationControl";
            this.Size = new System.Drawing.Size(400, 374);
            this.annotationsPanel.ResumeLayout(false);
            this.annotationsPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel annotationsPanel;
        private System.Windows.Forms.TextBox annotationsTextBox;
        private System.Windows.Forms.Button alertButton;
        private System.Windows.Forms.Button annotationButton;
    }
}
