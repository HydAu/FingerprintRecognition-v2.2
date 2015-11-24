namespace PatternRecognition.FingerprintRecognition.Applications
{
    partial class VisualFingerprintMatchingFrm
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
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.btnMatch = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.gbxQueryFingerprint = new System.Windows.Forms.GroupBox();
            this.pnlQueryImage = new System.Windows.Forms.Panel();
            this.pbxQueryImg = new System.Windows.Forms.PictureBox();
            this.pnlQueryButtons = new System.Windows.Forms.Panel();
            this.btnLoadQueryImg = new System.Windows.Forms.Button();
            this.gbxTemplateFingerprint = new System.Windows.Forms.GroupBox();
            this.pnlTemplateImage = new System.Windows.Forms.Panel();
            this.pbxTemplateImg = new System.Windows.Forms.PictureBox();
            this.pnlTemplateButtons = new System.Windows.Forms.Panel();
            this.btnLoadTemplateImg = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.pnlBottom.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.gbxQueryFingerprint.SuspendLayout();
            this.pnlQueryImage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxQueryImg)).BeginInit();
            this.pnlQueryButtons.SuspendLayout();
            this.gbxTemplateFingerprint.SuspendLayout();
            this.pnlTemplateImage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTemplateImg)).BeginInit();
            this.pnlTemplateButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlBottom
            // 
            this.pnlBottom.Controls.Add(this.btnMatch);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 400);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(651, 62);
            this.pnlBottom.TabIndex = 0;
            // 
            // btnMatch
            // 
            this.btnMatch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMatch.Location = new System.Drawing.Point(545, 16);
            this.btnMatch.Name = "btnMatch";
            this.btnMatch.Size = new System.Drawing.Size(75, 23);
            this.btnMatch.TabIndex = 6;
            this.btnMatch.Text = "Match";
            this.btnMatch.UseVisualStyleBackColor = true;
            this.btnMatch.Click += new System.EventHandler(this.btnMatch_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.gbxQueryFingerprint);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.gbxTemplateFingerprint);
            this.splitContainer1.Size = new System.Drawing.Size(651, 400);
            this.splitContainer1.SplitterDistance = 306;
            this.splitContainer1.TabIndex = 1;
            // 
            // gbxQueryFingerprint
            // 
            this.gbxQueryFingerprint.Controls.Add(this.pnlQueryImage);
            this.gbxQueryFingerprint.Controls.Add(this.pnlQueryButtons);
            this.gbxQueryFingerprint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbxQueryFingerprint.Location = new System.Drawing.Point(0, 0);
            this.gbxQueryFingerprint.Name = "gbxQueryFingerprint";
            this.gbxQueryFingerprint.Size = new System.Drawing.Size(306, 400);
            this.gbxQueryFingerprint.TabIndex = 0;
            this.gbxQueryFingerprint.TabStop = false;
            this.gbxQueryFingerprint.Text = "Query Fingerprint";
            // 
            // pnlQueryImage
            // 
            this.pnlQueryImage.AutoScroll = true;
            this.pnlQueryImage.Controls.Add(this.pbxQueryImg);
            this.pnlQueryImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlQueryImage.Location = new System.Drawing.Point(3, 16);
            this.pnlQueryImage.Name = "pnlQueryImage";
            this.pnlQueryImage.Size = new System.Drawing.Size(300, 313);
            this.pnlQueryImage.TabIndex = 0;
            // 
            // pbxQueryImg
            // 
            this.pbxQueryImg.Location = new System.Drawing.Point(0, 0);
            this.pbxQueryImg.Name = "pbxQueryImg";
            this.pbxQueryImg.Size = new System.Drawing.Size(154, 142);
            this.pbxQueryImg.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbxQueryImg.TabIndex = 0;
            this.pbxQueryImg.TabStop = false;
            // 
            // pnlQueryButtons
            // 
            this.pnlQueryButtons.Controls.Add(this.btnLoadQueryImg);
            this.pnlQueryButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlQueryButtons.Location = new System.Drawing.Point(3, 329);
            this.pnlQueryButtons.Name = "pnlQueryButtons";
            this.pnlQueryButtons.Size = new System.Drawing.Size(300, 68);
            this.pnlQueryButtons.TabIndex = 1;
            // 
            // btnLoadQueryImg
            // 
            this.btnLoadQueryImg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoadQueryImg.Location = new System.Drawing.Point(201, 22);
            this.btnLoadQueryImg.Name = "btnLoadQueryImg";
            this.btnLoadQueryImg.Size = new System.Drawing.Size(75, 23);
            this.btnLoadQueryImg.TabIndex = 0;
            this.btnLoadQueryImg.Text = "Load Image";
            this.btnLoadQueryImg.UseVisualStyleBackColor = true;
            this.btnLoadQueryImg.Click += new System.EventHandler(this.btnLoadQueryImg_Click);
            // 
            // gbxTemplateFingerprint
            // 
            this.gbxTemplateFingerprint.Controls.Add(this.pnlTemplateImage);
            this.gbxTemplateFingerprint.Controls.Add(this.pnlTemplateButtons);
            this.gbxTemplateFingerprint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbxTemplateFingerprint.Location = new System.Drawing.Point(0, 0);
            this.gbxTemplateFingerprint.Name = "gbxTemplateFingerprint";
            this.gbxTemplateFingerprint.Size = new System.Drawing.Size(341, 400);
            this.gbxTemplateFingerprint.TabIndex = 1;
            this.gbxTemplateFingerprint.TabStop = false;
            this.gbxTemplateFingerprint.Text = "Template Fingerprint";
            // 
            // pnlTemplateImage
            // 
            this.pnlTemplateImage.AutoScroll = true;
            this.pnlTemplateImage.Controls.Add(this.pbxTemplateImg);
            this.pnlTemplateImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTemplateImage.Location = new System.Drawing.Point(3, 16);
            this.pnlTemplateImage.Name = "pnlTemplateImage";
            this.pnlTemplateImage.Size = new System.Drawing.Size(335, 313);
            this.pnlTemplateImage.TabIndex = 2;
            // 
            // pbxTemplateImg
            // 
            this.pbxTemplateImg.Location = new System.Drawing.Point(0, 0);
            this.pbxTemplateImg.Name = "pbxTemplateImg";
            this.pbxTemplateImg.Size = new System.Drawing.Size(154, 142);
            this.pbxTemplateImg.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbxTemplateImg.TabIndex = 1;
            this.pbxTemplateImg.TabStop = false;
            // 
            // pnlTemplateButtons
            // 
            this.pnlTemplateButtons.Controls.Add(this.btnLoadTemplateImg);
            this.pnlTemplateButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlTemplateButtons.Location = new System.Drawing.Point(3, 329);
            this.pnlTemplateButtons.Name = "pnlTemplateButtons";
            this.pnlTemplateButtons.Size = new System.Drawing.Size(335, 68);
            this.pnlTemplateButtons.TabIndex = 3;
            // 
            // btnLoadTemplateImg
            // 
            this.btnLoadTemplateImg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoadTemplateImg.Location = new System.Drawing.Point(232, 22);
            this.btnLoadTemplateImg.Name = "btnLoadTemplateImg";
            this.btnLoadTemplateImg.Size = new System.Drawing.Size(75, 23);
            this.btnLoadTemplateImg.TabIndex = 2;
            this.btnLoadTemplateImg.Text = "Load Image";
            this.btnLoadTemplateImg.UseVisualStyleBackColor = true;
            this.btnLoadTemplateImg.Click += new System.EventHandler(this.btnLoadTemplateImg_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "Image Files|*.tif;*.bmp;*.jpg";
            // 
            // VisualFingerprintMatchingFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(651, 462);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.pnlBottom);
            this.Name = "VisualFingerprintMatchingFrm";
            this.Text = "Visual Fingeprint Matching";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.pnlBottom.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.gbxQueryFingerprint.ResumeLayout(false);
            this.pnlQueryImage.ResumeLayout(false);
            this.pnlQueryImage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxQueryImg)).EndInit();
            this.pnlQueryButtons.ResumeLayout(false);
            this.gbxTemplateFingerprint.ResumeLayout(false);
            this.pnlTemplateImage.ResumeLayout(false);
            this.pnlTemplateImage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTemplateImg)).EndInit();
            this.pnlTemplateButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox gbxQueryFingerprint;
        private System.Windows.Forms.Panel pnlQueryImage;
        private System.Windows.Forms.Panel pnlQueryButtons;
        private System.Windows.Forms.GroupBox gbxTemplateFingerprint;
        private System.Windows.Forms.PictureBox pbxQueryImg;
        private System.Windows.Forms.Panel pnlTemplateImage;
        private System.Windows.Forms.PictureBox pbxTemplateImg;
        private System.Windows.Forms.Panel pnlTemplateButtons;
        private System.Windows.Forms.Button btnLoadQueryImg;
        private System.Windows.Forms.Button btnLoadTemplateImg;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button btnMatch;
    }
}