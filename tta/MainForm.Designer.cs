namespace tta
{
    partial class MainForm
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
            this.listViewInputFiles = new System.Windows.Forms.ListView();
            this.columnHeaderPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.buttonConvert = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageInput = new System.Windows.Forms.TabPage();
            this.textBoxTitle = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tabPageConversion = new System.Windows.Forms.TabPage();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonStartNewConversion = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxLog = new System.Windows.Forms.TextBox();
            this.tabControl.SuspendLayout();
            this.tabPageInput.SuspendLayout();
            this.tabPageConversion.SuspendLayout();
            this.SuspendLayout();
            // 
            // listViewInputFiles
            // 
            this.listViewInputFiles.AllowDrop = true;
            this.listViewInputFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewInputFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderPath});
            this.listViewInputFiles.FullRowSelect = true;
            this.listViewInputFiles.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewInputFiles.HideSelection = false;
            this.listViewInputFiles.Location = new System.Drawing.Point(6, 55);
            this.listViewInputFiles.Name = "listViewInputFiles";
            this.listViewInputFiles.Size = new System.Drawing.Size(626, 447);
            this.listViewInputFiles.TabIndex = 0;
            this.listViewInputFiles.UseCompatibleStateImageBehavior = false;
            this.listViewInputFiles.View = System.Windows.Forms.View.Details;
            this.listViewInputFiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.listViewInputFiles_DragDrop);
            this.listViewInputFiles.DragEnter += new System.Windows.Forms.DragEventHandler(this.listViewInputFiles_DragEnter);
            // 
            // columnHeaderPath
            // 
            this.columnHeaderPath.Text = "Path";
            this.columnHeaderPath.Width = 615;
            // 
            // buttonConvert
            // 
            this.buttonConvert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonConvert.Location = new System.Drawing.Point(499, 508);
            this.buttonConvert.Name = "buttonConvert";
            this.buttonConvert.Size = new System.Drawing.Size(130, 48);
            this.buttonConvert.TabIndex = 1;
            this.buttonConvert.Text = "&Convert and copy to Pen";
            this.buttonConvert.UseVisualStyleBackColor = true;
            this.buttonConvert.Click += new System.EventHandler(this.buttonConvert_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(151, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Drop the input audio files here.";
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageInput);
            this.tabControl.Controls.Add(this.tabPageConversion);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(643, 588);
            this.tabControl.TabIndex = 3;
            // 
            // tabPageInput
            // 
            this.tabPageInput.Controls.Add(this.textBoxTitle);
            this.tabPageInput.Controls.Add(this.label3);
            this.tabPageInput.Controls.Add(this.buttonConvert);
            this.tabPageInput.Controls.Add(this.label1);
            this.tabPageInput.Controls.Add(this.listViewInputFiles);
            this.tabPageInput.Location = new System.Drawing.Point(4, 22);
            this.tabPageInput.Name = "tabPageInput";
            this.tabPageInput.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageInput.Size = new System.Drawing.Size(635, 562);
            this.tabPageInput.TabIndex = 0;
            this.tabPageInput.Text = "Input";
            this.tabPageInput.UseVisualStyleBackColor = true;
            // 
            // textBoxTitle
            // 
            this.textBoxTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxTitle.Location = new System.Drawing.Point(39, 6);
            this.textBoxTitle.Name = "textBoxTitle";
            this.textBoxTitle.Size = new System.Drawing.Size(588, 20);
            this.textBoxTitle.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(27, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Title";
            // 
            // tabPageConversion
            // 
            this.tabPageConversion.Controls.Add(this.buttonCancel);
            this.tabPageConversion.Controls.Add(this.buttonStartNewConversion);
            this.tabPageConversion.Controls.Add(this.label2);
            this.tabPageConversion.Controls.Add(this.textBoxLog);
            this.tabPageConversion.Location = new System.Drawing.Point(4, 22);
            this.tabPageConversion.Name = "tabPageConversion";
            this.tabPageConversion.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageConversion.Size = new System.Drawing.Size(635, 562);
            this.tabPageConversion.TabIndex = 1;
            this.tabPageConversion.Text = "Conversion";
            this.tabPageConversion.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.Location = new System.Drawing.Point(9, 506);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(130, 48);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "&Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonStartNewConversion
            // 
            this.buttonStartNewConversion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonStartNewConversion.Location = new System.Drawing.Point(497, 506);
            this.buttonStartNewConversion.Name = "buttonStartNewConversion";
            this.buttonStartNewConversion.Size = new System.Drawing.Size(130, 48);
            this.buttonStartNewConversion.TabIndex = 2;
            this.buttonStartNewConversion.Text = "&Start new Conversion";
            this.buttonStartNewConversion.UseVisualStyleBackColor = true;
            this.buttonStartNewConversion.Click += new System.EventHandler(this.buttonStartNewConversion_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "&Log";
            // 
            // textBoxLog
            // 
            this.textBoxLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxLog.Location = new System.Drawing.Point(9, 19);
            this.textBoxLog.Multiline = true;
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.ReadOnly = true;
            this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxLog.Size = new System.Drawing.Size(618, 481);
            this.textBoxLog.TabIndex = 0;
            this.textBoxLog.WordWrap = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(643, 588);
            this.Controls.Add(this.tabControl);
            this.MinimumSize = new System.Drawing.Size(300, 300);
            this.Name = "MainForm";
            this.Text = "tta";
            this.tabControl.ResumeLayout(false);
            this.tabPageInput.ResumeLayout(false);
            this.tabPageInput.PerformLayout();
            this.tabPageConversion.ResumeLayout(false);
            this.tabPageConversion.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewInputFiles;
        private System.Windows.Forms.Button buttonConvert;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ColumnHeader columnHeaderPath;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageInput;
        private System.Windows.Forms.TabPage tabPageConversion;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxLog;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonStartNewConversion;
        private System.Windows.Forms.TextBox textBoxTitle;
        private System.Windows.Forms.Label label3;
    }
}

