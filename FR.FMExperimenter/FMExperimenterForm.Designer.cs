namespace PatternRecognition.FingerprintRecognition.Applications
{
    partial class FMExperimenterForm
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
            this.label2 = new System.Windows.Forms.Label();
            this.tbxResources = new System.Windows.Forms.TextBox();
            this.btnFindResources = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.lblMinutiaExtractor = new System.Windows.Forms.Label();
            this.cbxMinutiaExtractor = new System.Windows.Forms.ComboBox();
            this.lblOrientationImageExtractor = new System.Windows.Forms.Label();
            this.cbxOrientationImageExtractor = new System.Windows.Forms.ComboBox();
            this.lblSkeletonImageExtractor = new System.Windows.Forms.Label();
            this.cbxSkeletonImageExtractor = new System.Windows.Forms.ComboBox();
            this.lblMatcher = new System.Windows.Forms.Label();
            this.cbxMatcher = new System.Windows.Forms.ComboBox();
            this.lblProgressValue = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btnExecute = new System.Windows.Forms.Button();
            this.lblFeatureProvider = new System.Windows.Forms.Label();
            this.cbxFeatureProvider = new System.Windows.Forms.ComboBox();
            this.gbxProperties = new System.Windows.Forms.GroupBox();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.lblExperiment = new System.Windows.Forms.Label();
            this.cbxExperiment = new System.Windows.Forms.ComboBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.btnVisualMatch = new System.Windows.Forms.Button();
            this.gbxProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "Resources";
            // 
            // tbxResources
            // 
            this.tbxResources.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbxResources.Location = new System.Drawing.Point(68, 12);
            this.tbxResources.Name = "tbxResources";
            this.tbxResources.Size = new System.Drawing.Size(528, 20);
            this.tbxResources.TabIndex = 15;
            this.tbxResources.Text = "D:\\Migue\\Code\\FingerprintRecognition\\CodeProject\\Fingerprint databases\\FVC2004\\DB" +
    "1_A";
            // 
            // btnFindResources
            // 
            this.btnFindResources.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFindResources.Location = new System.Drawing.Point(602, 9);
            this.btnFindResources.Name = "btnFindResources";
            this.btnFindResources.Size = new System.Drawing.Size(28, 23);
            this.btnFindResources.TabIndex = 14;
            this.btnFindResources.Text = "...";
            this.btnFindResources.UseVisualStyleBackColor = true;
            this.btnFindResources.Click += new System.EventHandler(this.btnFindResources_Click);
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.SelectedPath = "D:\\Fingerprints\\Resources\\FVC2004";
            // 
            // lblMinutiaExtractor
            // 
            this.lblMinutiaExtractor.AutoSize = true;
            this.lblMinutiaExtractor.Location = new System.Drawing.Point(56, 97);
            this.lblMinutiaExtractor.Name = "lblMinutiaExtractor";
            this.lblMinutiaExtractor.Size = new System.Drawing.Size(86, 13);
            this.lblMinutiaExtractor.TabIndex = 18;
            this.lblMinutiaExtractor.Text = "Minutia Extractor";
            // 
            // cbxMinutiaExtractor
            // 
            this.cbxMinutiaExtractor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxMinutiaExtractor.FormattingEnabled = true;
            this.cbxMinutiaExtractor.Location = new System.Drawing.Point(148, 94);
            this.cbxMinutiaExtractor.Name = "cbxMinutiaExtractor";
            this.cbxMinutiaExtractor.Size = new System.Drawing.Size(224, 21);
            this.cbxMinutiaExtractor.Sorted = true;
            this.cbxMinutiaExtractor.TabIndex = 17;
            this.cbxMinutiaExtractor.SelectedValueChanged += new System.EventHandler(this.cbxMinutiaExtractor_SelectedValueChanged);
            this.cbxMinutiaExtractor.Enter += new System.EventHandler(this.cbxMinutiaExtractor_Enter);
            // 
            // lblOrientationImageExtractor
            // 
            this.lblOrientationImageExtractor.AutoSize = true;
            this.lblOrientationImageExtractor.Location = new System.Drawing.Point(7, 136);
            this.lblOrientationImageExtractor.Name = "lblOrientationImageExtractor";
            this.lblOrientationImageExtractor.Size = new System.Drawing.Size(135, 13);
            this.lblOrientationImageExtractor.TabIndex = 20;
            this.lblOrientationImageExtractor.Text = "Orientation Image Extractor";
            // 
            // cbxOrientationImageExtractor
            // 
            this.cbxOrientationImageExtractor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxOrientationImageExtractor.FormattingEnabled = true;
            this.cbxOrientationImageExtractor.Location = new System.Drawing.Point(148, 133);
            this.cbxOrientationImageExtractor.Name = "cbxOrientationImageExtractor";
            this.cbxOrientationImageExtractor.Size = new System.Drawing.Size(224, 21);
            this.cbxOrientationImageExtractor.Sorted = true;
            this.cbxOrientationImageExtractor.TabIndex = 19;
            this.cbxOrientationImageExtractor.SelectedValueChanged += new System.EventHandler(this.cbxOrientationImageExtractor_SelectedValueChanged);
            this.cbxOrientationImageExtractor.Enter += new System.EventHandler(this.cbxOrientationImageExtractor_Enter);
            // 
            // lblSkeletonImageExtractor
            // 
            this.lblSkeletonImageExtractor.AutoSize = true;
            this.lblSkeletonImageExtractor.Location = new System.Drawing.Point(16, 175);
            this.lblSkeletonImageExtractor.Name = "lblSkeletonImageExtractor";
            this.lblSkeletonImageExtractor.Size = new System.Drawing.Size(126, 13);
            this.lblSkeletonImageExtractor.TabIndex = 22;
            this.lblSkeletonImageExtractor.Text = "Skeleton Image Extractor";
            // 
            // cbxSkeletonImageExtractor
            // 
            this.cbxSkeletonImageExtractor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxSkeletonImageExtractor.FormattingEnabled = true;
            this.cbxSkeletonImageExtractor.Location = new System.Drawing.Point(148, 172);
            this.cbxSkeletonImageExtractor.Name = "cbxSkeletonImageExtractor";
            this.cbxSkeletonImageExtractor.Size = new System.Drawing.Size(224, 21);
            this.cbxSkeletonImageExtractor.Sorted = true;
            this.cbxSkeletonImageExtractor.TabIndex = 21;
            this.cbxSkeletonImageExtractor.SelectedValueChanged += new System.EventHandler(this.cbxSkeletonImageExtractor_SelectedValueChanged);
            this.cbxSkeletonImageExtractor.Enter += new System.EventHandler(this.cbxSkeletonImageExtractor_Enter);
            // 
            // lblMatcher
            // 
            this.lblMatcher.AutoSize = true;
            this.lblMatcher.Location = new System.Drawing.Point(96, 214);
            this.lblMatcher.Name = "lblMatcher";
            this.lblMatcher.Size = new System.Drawing.Size(46, 13);
            this.lblMatcher.TabIndex = 24;
            this.lblMatcher.Text = "Matcher";
            // 
            // cbxMatcher
            // 
            this.cbxMatcher.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxMatcher.FormattingEnabled = true;
            this.cbxMatcher.Location = new System.Drawing.Point(148, 211);
            this.cbxMatcher.Name = "cbxMatcher";
            this.cbxMatcher.Size = new System.Drawing.Size(224, 21);
            this.cbxMatcher.TabIndex = 23;
            this.cbxMatcher.SelectedValueChanged += new System.EventHandler(this.cbxMatcher_SelectedValueChanged);
            this.cbxMatcher.Enter += new System.EventHandler(this.cbxMatcher_Enter);
            // 
            // lblProgressValue
            // 
            this.lblProgressValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblProgressValue.AutoSize = true;
            this.lblProgressValue.Location = new System.Drawing.Point(599, 311);
            this.lblProgressValue.Name = "lblProgressValue";
            this.lblProgressValue.Size = new System.Drawing.Size(21, 13);
            this.lblProgressValue.TabIndex = 27;
            this.lblProgressValue.Text = "0%";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(18, 311);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(48, 13);
            this.lblStatus.TabIndex = 26;
            this.lblStatus.Text = "Progress";
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(112, 306);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(474, 23);
            this.progressBar1.Step = 1;
            this.progressBar1.TabIndex = 25;
            // 
            // btnExecute
            // 
            this.btnExecute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExecute.Location = new System.Drawing.Point(403, 355);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(109, 23);
            this.btnExecute.TabIndex = 28;
            this.btnExecute.Text = "Execute Experiment";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // lblFeatureProvider
            // 
            this.lblFeatureProvider.AutoSize = true;
            this.lblFeatureProvider.Location = new System.Drawing.Point(57, 253);
            this.lblFeatureProvider.Name = "lblFeatureProvider";
            this.lblFeatureProvider.Size = new System.Drawing.Size(85, 13);
            this.lblFeatureProvider.TabIndex = 30;
            this.lblFeatureProvider.Text = "Feature Provider";
            // 
            // cbxFeatureProvider
            // 
            this.cbxFeatureProvider.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxFeatureProvider.FormattingEnabled = true;
            this.cbxFeatureProvider.Location = new System.Drawing.Point(148, 250);
            this.cbxFeatureProvider.Name = "cbxFeatureProvider";
            this.cbxFeatureProvider.Size = new System.Drawing.Size(224, 21);
            this.cbxFeatureProvider.TabIndex = 29;
            this.cbxFeatureProvider.SelectedValueChanged += new System.EventHandler(this.cbxFeatureProvider_SelectedValueChanged);
            this.cbxFeatureProvider.Enter += new System.EventHandler(this.cbxFeatureProvider_Enter);
            // 
            // gbxProperties
            // 
            this.gbxProperties.Controls.Add(this.propertyGrid1);
            this.gbxProperties.Location = new System.Drawing.Point(389, 55);
            this.gbxProperties.Name = "gbxProperties";
            this.gbxProperties.Size = new System.Drawing.Size(234, 216);
            this.gbxProperties.TabIndex = 31;
            this.gbxProperties.TabStop = false;
            this.gbxProperties.Text = "Properties";
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.Location = new System.Drawing.Point(3, 16);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(228, 197);
            this.propertyGrid1.TabIndex = 0;
            // 
            // lblExperiment
            // 
            this.lblExperiment.AutoSize = true;
            this.lblExperiment.Location = new System.Drawing.Point(83, 58);
            this.lblExperiment.Name = "lblExperiment";
            this.lblExperiment.Size = new System.Drawing.Size(59, 13);
            this.lblExperiment.TabIndex = 33;
            this.lblExperiment.Text = "Experiment";
            // 
            // cbxExperiment
            // 
            this.cbxExperiment.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxExperiment.FormattingEnabled = true;
            this.cbxExperiment.Location = new System.Drawing.Point(148, 55);
            this.cbxExperiment.Name = "cbxExperiment";
            this.cbxExperiment.Size = new System.Drawing.Size(224, 21);
            this.cbxExperiment.Sorted = true;
            this.cbxExperiment.TabIndex = 32;
            this.cbxExperiment.SelectedValueChanged += new System.EventHandler(this.cbxExperiment_SelectedValueChanged);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // btnVisualMatch
            // 
            this.btnVisualMatch.Location = new System.Drawing.Point(535, 355);
            this.btnVisualMatch.Name = "btnVisualMatch";
            this.btnVisualMatch.Size = new System.Drawing.Size(85, 23);
            this.btnVisualMatch.TabIndex = 34;
            this.btnVisualMatch.Text = "Visual Match";
            this.btnVisualMatch.UseVisualStyleBackColor = true;
            this.btnVisualMatch.Click += new System.EventHandler(this.btnVisualMatch_Click);
            // 
            // FMExperimenterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(635, 390);
            this.Controls.Add(this.btnVisualMatch);
            this.Controls.Add(this.lblExperiment);
            this.Controls.Add(this.cbxExperiment);
            this.Controls.Add(this.gbxProperties);
            this.Controls.Add(this.lblFeatureProvider);
            this.Controls.Add(this.cbxFeatureProvider);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.lblProgressValue);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.lblMatcher);
            this.Controls.Add(this.cbxMatcher);
            this.Controls.Add(this.lblSkeletonImageExtractor);
            this.Controls.Add(this.cbxSkeletonImageExtractor);
            this.Controls.Add(this.lblOrientationImageExtractor);
            this.Controls.Add(this.cbxOrientationImageExtractor);
            this.Controls.Add(this.lblMinutiaExtractor);
            this.Controls.Add(this.cbxMinutiaExtractor);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbxResources);
            this.Controls.Add(this.btnFindResources);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "FMExperimenterForm";
            this.Text = "Fingerprint Matching Experimenter";
            this.gbxProperties.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbxResources;
        private System.Windows.Forms.Button btnFindResources;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Label lblMinutiaExtractor;
        private System.Windows.Forms.ComboBox cbxMinutiaExtractor;
        private System.Windows.Forms.Label lblOrientationImageExtractor;
        private System.Windows.Forms.ComboBox cbxOrientationImageExtractor;
        private System.Windows.Forms.Label lblSkeletonImageExtractor;
        private System.Windows.Forms.ComboBox cbxSkeletonImageExtractor;
        private System.Windows.Forms.Label lblMatcher;
        private System.Windows.Forms.ComboBox cbxMatcher;
        private System.Windows.Forms.Label lblProgressValue;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.Label lblFeatureProvider;
        private System.Windows.Forms.ComboBox cbxFeatureProvider;
        private System.Windows.Forms.GroupBox gbxProperties;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.Label lblExperiment;
        private System.Windows.Forms.ComboBox cbxExperiment;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Button btnVisualMatch;
    }
}