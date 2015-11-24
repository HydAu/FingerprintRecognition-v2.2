using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using PatternRecognition.FingerprintRecognition.Core;

namespace PatternRecognition.FingerprintRecognition.Applications
{
    public partial class FeatureDisplayForm : Form
    {
        public FeatureDisplayForm()
        {
            InitializeComponent();
            Assembly thisAss = Assembly.GetExecutingAssembly();
            string dir = Path.GetDirectoryName(thisAss.Location);
            foreach (string fileName in Directory.GetFiles(dir))
            {
                string fileExtension = Path.GetExtension(fileName);
                if (fileExtension == ".dll")
                {
                    try
                    {
                        Assembly currAssembly = Assembly.LoadFile(fileName);
                        foreach (Type type in currAssembly.GetExportedTypes())
                        {
                            var currInterface = type.GetInterface("IFeatureDisplay`1");
                            if (type.IsClass && !type.IsAbstract && currInterface != null)
                            {
                                var featType = currInterface.GetGenericArguments()[0];
                                featTypeByDisplay.Add(type, featType);
                            }

                            currInterface = type.GetInterface("IFeatureExtractor`1");
                            if (type.IsClass && !type.IsAbstract && currInterface != null)
                            {
                                var featType = currInterface.GetGenericArguments()[0];
                                if (!extractorsByFeatType.ContainsKey(featType))
                                    extractorsByFeatType.Add(featType, new List<Type>());
                                extractorsByFeatType[featType].Add(type);
                            }
                        }
                    }
                    catch { }
                }
            }
            var toDelete = new List<Type>();
            foreach (var pair in featTypeByDisplay)
                if (!extractorsByFeatType.ContainsKey(pair.Value))
                    toDelete.Add(pair.Key);
            foreach (var type in toDelete)
                featTypeByDisplay.Remove(type);
        }

        private void frmFeaturesDisplay_Load(object sender, EventArgs e)
        {
            cbxFeatureDisplayers.DataSource = new List<Type>(featTypeByDisplay.Keys);
            cbxFeatureDisplayers.DisplayMember = "Name";
            cbxFeatureDisplayers.ValueMember = "Name";
        }

        private void cbxFeatureTypes_SelectedValueChanged(object sender, EventArgs e)
        {
            object selectedValue = ((ComboBox)sender).SelectedItem;
            if (selectedValue != null)
            {
                Type selectedType = (Type)selectedValue;
                currFeatDisplay = Activator.CreateInstance(selectedType) as IFeatureDisplay;
                Type currFeatType = featTypeByDisplay[selectedType];
                cbxFeatureExtractors.DataSource = extractorsByFeatType[currFeatType];
                cbxFeatureExtractors.DisplayMember = "Name";
                cbxFeatureExtractors.ValueMember = "Name";
            }
        }

        private void cbxFeatureExtractors_SelectedValueChanged(object sender, EventArgs e)
        {
            object selectedValue = ((ComboBox)sender).SelectedItem;
            if (selectedValue != null)
            {
                Type selectedType = (Type)selectedValue;
                currExtractor = Activator.CreateInstance(selectedType) as IFeatureExtractor;
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                img = ImageLoader.LoadImage(openFileDialog1.FileName);
                pictureBox1.Image = img;
            }
        }

        private void btnShowFeatures_Click(object sender, EventArgs e)
        {
            if (img == null)
            {
                MessageBox.Show("You must select an image!", "Displaying error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            pictureBox1.Image = img.Clone() as Bitmap;
            Graphics g = Graphics.FromImage(pictureBox1.Image);
            //try
            {
                var features = currExtractor.ExtractFeatures(img);
                currFeatDisplay.Show(features, g);
            }
            //catch (Exception exc)
            {
                //MessageBox.Show(exc.Message, "Displaying error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        readonly Dictionary<Type, Type> featTypeByDisplay = new Dictionary<Type, Type>();
        readonly Dictionary<Type, List<Type>> extractorsByFeatType = new Dictionary<Type, List<Type>>();
        private IFeatureDisplay currFeatDisplay;
        private IFeatureExtractor currExtractor;
        private Bitmap img = null;
    }
}