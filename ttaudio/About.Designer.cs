namespace Sidi
{
    partial class About
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
            this.buttonOK = new System.Windows.Forms.Button();
            this.textBoxAboutInfo = new System.Windows.Forms.TextBox();
            this.linkLabelHomepage = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(12, 214);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 0;
            this.buttonOK.Text = "&OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // textBoxAboutInfo
            // 
            this.textBoxAboutInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxAboutInfo.Location = new System.Drawing.Point(12, 12);
            this.textBoxAboutInfo.Multiline = true;
            this.textBoxAboutInfo.Name = "textBoxAboutInfo";
            this.textBoxAboutInfo.ReadOnly = true;
            this.textBoxAboutInfo.Size = new System.Drawing.Size(447, 145);
            this.textBoxAboutInfo.TabIndex = 2;
            // 
            // linkLabelHomepage
            // 
            this.linkLabelHomepage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkLabelHomepage.AutoSize = true;
            this.linkLabelHomepage.Location = new System.Drawing.Point(12, 172);
            this.linkLabelHomepage.Name = "linkLabelHomepage";
            this.linkLabelHomepage.Size = new System.Drawing.Size(55, 13);
            this.linkLabelHomepage.TabIndex = 4;
            this.linkLabelHomepage.TabStop = true;
            this.linkLabelHomepage.Text = "linkLabel1";
            // 
            // About
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonOK;
            this.ClientSize = new System.Drawing.Size(471, 249);
            this.Controls.Add(this.linkLabelHomepage);
            this.Controls.Add(this.textBoxAboutInfo);
            this.Controls.Add(this.buttonOK);
            this.MinimumSize = new System.Drawing.Size(487, 288);
            this.Name = "About";
            this.Text = "About";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.TextBox textBoxAboutInfo;
        private System.Windows.Forms.LinkLabel linkLabelHomepage;
    }
}