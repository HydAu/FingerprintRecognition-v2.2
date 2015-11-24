namespace PatternRecognition.FingerprintRecognition.Applications
{
    partial class FeatureDisplayForm
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
            this.cbxFeatureDisplayers = new System.Windows.Forms.ComboBox();
            this.cbxFeatureExtractors = new System.Windows.Forms.ComboBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lblFeatExtractor = new System.Windows.Forms.Label();
            this.lblFeatDisplay = new System.Windows.Forms.Label();
            this.btnShowFeatures = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbxFeatureDisplayers
            // 
            this.cbxFeatureDisplayers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxFeatureDisplayers.FormattingEnabled = true;
            this.cbxFeatureDisplayers.Location = new System.Drawing.Point(99, 12);
            this.cbxFeatureDisplayers.Name = "cbxFeatureDisplayers";
            this.cbxFeatureDisplayers.Size = new System.Drawing.Size(188, 21);
            this.cbxFeatureDisplayers.Sorted = true;
            this.cbxFeatureDisplayers.TabIndex = 0;
            this.cbxFeatureDisplayers.SelectedValueChanged += new System.EventHandler(this.cbxFeatureTypes_SelectedValueChanged);
            // 
            // cbxFeatureExtractors
            // 
            this.cbxFeatureExtractors.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxFeatureExtractors.FormattingEnabled = true;
            this.cbxFeatureExtractors.Location = new System.Drawing.Point(99, 55);
            this.cbxFeatureExtractors.Name = "cbxFeatureExtractors";
            this.cbxFeatureExtractors.Size = new System.Drawing.Size(188, 21);
            this.cbxFeatureExtractors.Sorted = true;
            this.cbxFeatureExtractors.TabIndex = 2;
            this.cbxFeatureExtractors.SelectedValueChanged += new System.EventHandler(this.cbxFeatureExtractors_SelectedValueChanged);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "Image Files (*.bmp;*.gif;*.jpg;*.png;*.tif)|*.bmp;*.gif;*.jpg;*.png;*.tif";
            // 
            // panel2
            // 
            this.panel2.AutoScroll = true;
            this.panel2.Controls.Add(this.pictureBox1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(340, 441);
            this.panel2.TabIndex = 5;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(183, 178);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.lblFeatExtractor);
            this.panel3.Controls.Add(this.lblFeatDisplay);
            this.panel3.Controls.Add(this.cbxFeatureExtractors);
            this.panel3.Controls.Add(this.btnShowFeatures);
            this.panel3.Controls.Add(this.cbxFeatureDisplayers);
            this.panel3.Controls.Add(this.btnLoad);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel3.Location = new System.Drawing.Point(340, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(299, 441);
            this.panel3.TabIndex = 4;
            // 
            // lblFeatExtractor
            // 
            this.lblFeatExtractor.AutoSize = true;
            this.lblFeatExtractor.Location = new System.Drawing.Point(6, 58);
            this.lblFeatExtractor.Name = "lblFeatExtractor";
            this.lblFeatExtractor.Size = new System.Drawing.Size(88, 13);
            this.lblFeatExtractor.TabIndex = 4;
            this.lblFeatExtractor.Text = "Feature Extractor";
            // 
            // lblFeatDisplay
            // 
            this.lblFeatDisplay.AutoSize = true;
            this.lblFeatDisplay.Location = new System.Drawing.Point(6, 15);
            this.lblFeatDisplay.Name = "lblFeatDisplay";
            this.lblFeatDisplay.Size = new System.Drawing.Size(80, 13);
            this.lblFeatDisplay.TabIndex = 3;
            this.lblFeatDisplay.Text = "Feature Display";
            // 
            // btnShowFeatures
            // 
            this.btnShowFeatures.Location = new System.Drawing.Point(212, 104);
            this.btnShowFeatures.Name = "btnShowFeatures";
            this.btnShowFeatures.Size = new System.Drawing.Size(75, 23);
            this.btnShowFeatures.TabIndex = 1;
            this.btnShowFeatures.Text = "Show";
            this.btnShowFeatures.UseVisualStyleBackColor = true;
            this.btnShowFeatures.Click += new System.EventHandler(this.btnShowFeatures_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(120, 104);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 23);
            this.btnLoad.TabIndex = 0;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // FeatureDisplayForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(639, 441);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel3);
            this.Name = "FeatureDisplayForm";
            this.Text = "Fingerprint Feature Display";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmFeaturesDisplay_Load);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cbxFeatureDisplayers;
        private System.Windows.Forms.ComboBox cbxFeatureExtractors;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btnShowFeatures;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Label lblFeatExtractor;
        private System.Windows.Forms.Label lblFeatDisplay;
    }
}