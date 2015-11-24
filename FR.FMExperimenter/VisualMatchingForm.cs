using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using PatternRecognition.FingerprintRecognition.Core;
using PatternRecognition.FingerprintRecognition.FeatureDisplay;

namespace PatternRecognition.FingerprintRecognition.Applications
{
    public partial class VisualFingerprintMatchingFrm : Form
    {
        public VisualFingerprintMatchingFrm(IMatcher matcher, IResourceProvider resourceProvider, string resourcePath)
        {
            InitializeComponent();
            this.matcher = matcher;
            provider = resourceProvider;
            this.resourcePath = resourcePath;
            repository = new ResourceRepository(resourcePath);
        }

        private void btnLoadQueryImg_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = resourcePath;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string shortFileName = Path.GetFileNameWithoutExtension(openFileDialog1.FileName);
                try
                {
                    qFeatures = provider.GetResource(shortFileName, repository);
                }
                catch (Exception)
                {
                    MessageBox.Show("Unable to load features " + provider.GetSignature() + ". Try using different parameters.", "Feature Loading Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                qImage = ImageLoader.LoadImage(openFileDialog1.FileName);
                pbxQueryImg.Image = qImage;
            }
        }

        private void btnLoadTemplateImg_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = resourcePath;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string shortFileName = Path.GetFileNameWithoutExtension(openFileDialog1.FileName);
                try
                {
                    tFeatures = provider.GetResource(shortFileName, repository);
                }
                catch (Exception)
                {
                    MessageBox.Show("Unable to load features " + provider.GetSignature() + ". Try using different parameters.", "Feature Loading Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                tImage = ImageLoader.LoadImage(openFileDialog1.FileName);
                pbxTemplateImg.Image = tImage;
            }
        }

        private void ShowResults(double matchingScore, List<MinutiaPair> matchingMtiae)
        {
            if (matchingScore == 0 || matchingMtiae == null)
                MessageBox.Show(string.Format("Similarity: {0}.", matchingScore));
            else
            {
                List<Minutia> qMtiae = new List<Minutia>();
                List<Minutia> tMtiae = new List<Minutia>();
                foreach (MinutiaPair mPair in matchingMtiae)
                {
                    qMtiae.Add(mPair.QueryMtia);
                    tMtiae.Add(mPair.TemplateMtia);
                }
                IFeatureDisplay<List<Minutia>> display = new MinutiaeDisplay();

                pbxQueryImg.Image = qImage.Clone() as Bitmap;
                Graphics g = Graphics.FromImage(pbxQueryImg.Image);
                display.Show(qMtiae, g);
                pbxQueryImg.Invalidate();

                pbxTemplateImg.Image = tImage.Clone() as Bitmap;
                g = Graphics.FromImage(pbxTemplateImg.Image);
                display.Show(tMtiae, g);
                pbxTemplateImg.Invalidate();

                MessageBox.Show(string.Format("Similarity: {0}. Matching minutiae: {1}.", matchingScore,
                                              matchingMtiae.Count));
            }
        }

        #region private fields

        private Bitmap qImage, tImage;

        private IResourceProvider provider;

        private ResourceRepository repository;

        private string resourcePath;

        private IMatcher matcher;

        private object qFeatures, tFeatures;

        #endregion

        private void btnMatch_Click(object sender, EventArgs e)
        {
            if (qImage == null)
            {
                MessageBox.Show("Unable to match fingerprints: Unassigned query fingerprint!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (tImage == null)
            {
                MessageBox.Show("Unable to match fingerprints: Unassigned template fingerprint!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Matching features
            List<MinutiaPair> matchingMtiae = null;
            double score;
            IMinutiaMatcher minutiaMatcher = matcher as IMinutiaMatcher;
            if (minutiaMatcher != null)
            {
                score = minutiaMatcher.Match(qFeatures, tFeatures, out matchingMtiae);

                if (qFeatures is List<Minutia> && tFeatures is List<Minutia>)
                {
                    pbxQueryImg.Image = qImage.Clone() as Bitmap;
                    Graphics g1 = Graphics.FromImage(pbxQueryImg.Image);
                    ShowBlueMinutiae(qFeatures as List<Minutia>, g1);
                    //pbxQueryImg.Invalidate();

                    pbxTemplateImg.Image = tImage.Clone() as Bitmap;
                    Graphics g2 = Graphics.FromImage(pbxTemplateImg.Image);
                    ShowBlueMinutiae(tFeatures as List<Minutia>, g2);
                    //pbxTemplateImg.Invalidate();

                    if (score == 0 || matchingMtiae == null)
                        MessageBox.Show(string.Format("Similarity: {0}.", score));
                    else
                    {
                        List<Minutia> qMtiae = new List<Minutia>();
                        List<Minutia> tMtiae = new List<Minutia>();
                        foreach (MinutiaPair mPair in matchingMtiae)
                        {
                            qMtiae.Add(mPair.QueryMtia);
                            tMtiae.Add(mPair.TemplateMtia);
                        }
                        IFeatureDisplay<List<Minutia>> display = new MinutiaeDisplay();

                        display.Show(qMtiae, g1);
                        pbxQueryImg.Invalidate();

                        display.Show(tMtiae, g2);
                        pbxTemplateImg.Invalidate();

                        MessageBox.Show(string.Format("Similarity: {0}. Matching minutiae: {1}.", score,
                                                      matchingMtiae.Count));
                    }
                }
                else
                    ShowResults(score, matchingMtiae);
            }
            else
                score = matcher.Match(qFeatures, tFeatures);
        }

        public void ShowBlueMinutiae(List<Minutia> features, Graphics g)
        {
            int mtiaRadius = 6;
            int lineLength = 18;
            Pen pen = new Pen(Brushes.Blue) { Width = 3 };
            pen.Color = Color.LightBlue;

            Pen whitePen = new Pen(Brushes.Blue) { Width = 5 };
            whitePen.Color = Color.White;

            int i = 0;
            foreach (Minutia mtia in (IList<Minutia>)features)
            {
                g.DrawEllipse(whitePen, mtia.X - mtiaRadius, mtia.Y - mtiaRadius, 2 * mtiaRadius + 1, 2 * mtiaRadius + 1);
                g.DrawLine(whitePen, mtia.X, mtia.Y, Convert.ToInt32(mtia.X + lineLength * Math.Cos(mtia.Angle)), Convert.ToInt32(mtia.Y + lineLength * Math.Sin(mtia.Angle)));

                pen.Color = Color.LightBlue;

                g.DrawEllipse(pen, mtia.X - mtiaRadius, mtia.Y - mtiaRadius, 2 * mtiaRadius + 1, 2 * mtiaRadius + 1);
                g.DrawLine(pen, mtia.X, mtia.Y, Convert.ToInt32(mtia.X + lineLength * Math.Cos(mtia.Angle)), Convert.ToInt32(mtia.Y + lineLength * Math.Sin(mtia.Angle)));
                i++;
            }

            Minutia lastMtia = ((IList<Minutia>)features)[((IList<Minutia>)features).Count - 1];
            pen.Color = Color.LightBlue;
            g.DrawEllipse(pen, lastMtia.X - mtiaRadius, lastMtia.Y - mtiaRadius, 2 * mtiaRadius + 1, 2 * mtiaRadius + 1);
            g.DrawLine(pen, lastMtia.X, lastMtia.Y, Convert.ToInt32(lastMtia.X + lineLength * Math.Cos(lastMtia.Angle)), Convert.ToInt32(lastMtia.Y + lineLength * Math.Sin(lastMtia.Angle)));
        }

    }
}