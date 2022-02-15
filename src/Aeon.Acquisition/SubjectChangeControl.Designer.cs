
namespace Aeon.Acquisition
{
    partial class SubjectChangeControl
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.addButton = new System.Windows.Forms.Button();
            this.subjectsGroupBox = new System.Windows.Forms.GroupBox();
            this.subjectListView = new System.Windows.Forms.ListView();
            this.removeButton = new System.Windows.Forms.Button();
            this.subjectMetadataGroupBox = new System.Windows.Forms.GroupBox();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.subjectListViewPanel = new System.Windows.Forms.Panel();
            this.subjectsGroupBox.SuspendLayout();
            this.subjectMetadataGroupBox.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            this.subjectListViewPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // addButton
            // 
            this.addButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.addButton.Location = new System.Drawing.Point(12, 278);
            this.addButton.Margin = new System.Windows.Forms.Padding(6);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(195, 52);
            this.addButton.TabIndex = 0;
            this.addButton.Text = "Add";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // subjectsGroupBox
            // 
            this.subjectsGroupBox.Controls.Add(this.subjectListView);
            this.subjectsGroupBox.Controls.Add(this.removeButton);
            this.subjectsGroupBox.Controls.Add(this.addButton);
            this.subjectsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.subjectsGroupBox.Location = new System.Drawing.Point(0, 0);
            this.subjectsGroupBox.Margin = new System.Windows.Forms.Padding(6);
            this.subjectsGroupBox.Name = "subjectsGroupBox";
            this.subjectsGroupBox.Padding = new System.Windows.Forms.Padding(6);
            this.subjectsGroupBox.Size = new System.Drawing.Size(394, 339);
            this.subjectsGroupBox.TabIndex = 2;
            this.subjectsGroupBox.TabStop = false;
            this.subjectsGroupBox.Text = "Active Subjects";
            // 
            // subjectListView
            // 
            this.subjectListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.subjectListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.subjectListView.HideSelection = false;
            this.subjectListView.Location = new System.Drawing.Point(6, 37);
            this.subjectListView.Margin = new System.Windows.Forms.Padding(6);
            this.subjectListView.MultiSelect = false;
            this.subjectListView.Name = "subjectListView";
            this.subjectListView.Size = new System.Drawing.Size(382, 224);
            this.subjectListView.TabIndex = 2;
            this.subjectListView.UseCompatibleStateImageBehavior = false;
            this.subjectListView.View = System.Windows.Forms.View.Details;
            this.subjectListView.SelectedIndexChanged += new System.EventHandler(this.subjectListView_SelectedIndexChanged);
            // 
            // removeButton
            // 
            this.removeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.removeButton.Location = new System.Drawing.Point(219, 278);
            this.removeButton.Margin = new System.Windows.Forms.Padding(6);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(169, 52);
            this.removeButton.TabIndex = 3;
            this.removeButton.Text = "Remove";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // subjectMetadataGroupBox
            // 
            this.subjectMetadataGroupBox.Controls.Add(this.propertyGrid);
            this.subjectMetadataGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.subjectMetadataGroupBox.Location = new System.Drawing.Point(406, 3);
            this.subjectMetadataGroupBox.Margin = new System.Windows.Forms.Padding(6, 3, 6, 6);
            this.subjectMetadataGroupBox.Name = "subjectMetadataGroupBox";
            this.subjectMetadataGroupBox.Padding = new System.Windows.Forms.Padding(6);
            this.subjectMetadataGroupBox.Size = new System.Drawing.Size(388, 336);
            this.subjectMetadataGroupBox.TabIndex = 7;
            this.subjectMetadataGroupBox.TabStop = false;
            this.subjectMetadataGroupBox.Text = "Subject Metadata";
            // 
            // propertyGrid
            // 
            this.propertyGrid.CommandsVisibleIfAvailable = false;
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.HelpVisible = false;
            this.propertyGrid.Location = new System.Drawing.Point(6, 37);
            this.propertyGrid.Margin = new System.Windows.Forms.Padding(6);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.propertyGrid.Size = new System.Drawing.Size(376, 293);
            this.propertyGrid.TabIndex = 0;
            this.propertyGrid.ToolbarVisible = false;
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.Controls.Add(this.subjectMetadataGroupBox, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.subjectListViewPanel, 0, 0);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 1;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(800, 345);
            this.tableLayoutPanel.TabIndex = 8;
            // 
            // subjectListViewPanel
            // 
            this.subjectListViewPanel.Controls.Add(this.subjectsGroupBox);
            this.subjectListViewPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.subjectListViewPanel.Location = new System.Drawing.Point(3, 3);
            this.subjectListViewPanel.Name = "subjectListViewPanel";
            this.subjectListViewPanel.Size = new System.Drawing.Size(394, 339);
            this.subjectListViewPanel.TabIndex = 8;
            // 
            // SubjectChangeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "SubjectChangeControl";
            this.Size = new System.Drawing.Size(800, 345);
            this.subjectsGroupBox.ResumeLayout(false);
            this.subjectMetadataGroupBox.ResumeLayout(false);
            this.tableLayoutPanel.ResumeLayout(false);
            this.subjectListViewPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.GroupBox subjectsGroupBox;
        private System.Windows.Forms.ListView subjectListView;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.GroupBox subjectMetadataGroupBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Panel subjectListViewPanel;
        private System.Windows.Forms.PropertyGrid propertyGrid;
    }
}