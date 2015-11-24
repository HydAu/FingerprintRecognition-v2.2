/*
 * Created by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 * Created:
 * Comments by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using PatternRecognition.FingerprintRecognition.Core;
using PatternRecognition.ROC;

namespace PatternRecognition.FingerprintRecognition.Experiments
{
    /// <summary>
    ///     Abstract class containing the evaluation protocol for testing a fingerprint matching algorithm in databases DB1_A, DB2_A, DB3_A and DB4_A from FVC2004.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This experiment uses the evaluation protocol of the Fingerprint Verification Competitions [1]. The performance indicators computed in this experiments are: EER(%), FMR100(%), FMR1000(%), ZeroFMR(%), Time(ms) and ROC curves. This indicators are saved in a file which name is formed by the name of the matching algorithm concatenated with ".Summary.csv". This file is saved in a folder by the name of "Results" in the same folder where the fingerprints are stored. Two more files are saved, one file containing the false fingerprint matches and the other containing the false not matches.
    ///     </para>
    ///     <para>
    ///         In order to execute the experiment, you must set the properties <see cref="Matcher"/>, <see cref="ResourceProvider"/> and <see cref="ResourcePath"/>. Optionally, you can set the property <see cref="StatusEvent"/> in order to get updates concerning the status of the experiment.
    ///     </para>
    ///     <para>
    ///         References:
    ///     </para>
    ///     <para>
    ///         <list type="number">
    ///             <item>
    ///                R. Cappelli, D. Maio, D. Maltoni, J. L. Wayman, and A. K. Jain, "Performance evaluation of fingerprint verification systems," IEEE Transactions on Pattern Analysis and Machine Intelligence, vol. 28, pp. 3-18, 2006.
    ///             </item>
    ///         </list>
    ///     </para>
    /// </remarks>
    /// <seealso cref="FVC2004_DB1_A"/>
    /// <seealso cref="FVC2004_DB2_A"/>
    /// <seealso cref="FVC2004_DB3_A"/>
    /// <seealso cref="FVC2004_DB4_A"/>
    public abstract class FVC2004_DB_A : IMatchingExperiment
    {
        #region public

        /// <summary>
        ///     The matcher to be evaluated.
        /// </summary>
        public IMatcher Matcher { set; get; }

        /// <summary>
        ///     Used to provide the features used by the specified matcher.
        /// </summary>
        public IResourceProvider ResourceProvider { set; get; }

        /// <summary>
        ///     The path where the resources are stored.
        /// </summary>
        public string ResourcePath { get; set; }

        /// <summary>
        ///     Event that provides information about the experiment status.
        /// </summary>
        public event ProgressChangedEventHandler StatusEvent = null;

        /// <summary>
        ///     Executes the experiment.
        /// </summary>
        ///<exception cref="InvalidOperationException">Thrown when the resource path is invalid, the matcher is not assigned or the resource provider is not assigned.</exception>
        public void Execute()
        {
            VerifyPreconditions();

            // Loading features
            List<string> imgCaptions;
            var sortedFeatures = GetFeatures(out imgCaptions);

            // Perform all comparissons
            List<double> genuine;
            List<double> impostor;
            List<string> falseNotMatches;
            List<string> falseMatches;
            double speed = DoComparisons(sortedFeatures, imgCaptions, out genuine, out impostor, out falseNotMatches, out falseMatches);

            Directory.CreateDirectory(string.Format(@"{0}\Results\", ResourcePath));
            string fileName = string.Format(@"{0}\Results\{1}.falseNotMatches.csv", ResourcePath, Matcher.GetType().Name);
            SaveBadMatches(fileName, falseNotMatches);
            fileName = string.Format(@"{0}\Results\{1}.falseMatches.csv", ResourcePath, Matcher.GetType().Name);
            SaveBadMatches(fileName, falseMatches);

            fileName = string.Format(@"{0}\Results\{1}.Summary.csv", ResourcePath, Matcher.GetType().Name);
            StreamWriter fs = new StreamWriter(fileName, false);
            fs.WriteLine(String.Format("Features: {0}", ResourceProvider.GetSignature()));
            fs.WriteLine("Average match time(ms):; {0:f5}", speed);
            double eer, fmr100, fmr1000, zeroFmr;
            List<ROCPoint> roc;
            ComputeAccuracy(genuine, impostor, out eer, out fmr100, out fmr1000, out zeroFmr, out roc);
            fs.WriteLine("EER(%):; {0:f3}", eer);
            fs.WriteLine("FMR100(%):; {0:f3}", fmr100);
            fs.WriteLine("FMR1000(%):; {0:f3}", fmr1000);
            fs.WriteLine("ZeroFmr(%):; {0:f3}", zeroFmr);
            ROCSerializer.Serialize(roc, "FMR", "FNMR", Matcher.GetType().Name, fs);
            fs.Close();

            //genuine.Sort();
            //impostor.Sort();
            //fileName = string.Format(@"{0}\Results\{1}.FMRvsFNMR.csv", ResourcePath, Matcher.GetType().Name);
            //fs = new StreamWriter(fileName, false);
            //fs.WriteLine("FMR;FNMR");
            //int max = Math.Max(genuine.Count, impostor.Count);
            //for (int i = 0; i < max; i++)
            //    fs.WriteLine("{0};{1}", impostor[i], genuine[i]);
            //fs.Close();
            //fileName = string.Format(@"{0}\Results\{1}.Genuine.bin", ResourcePath, Matcher.GetType().Name);
            //BinarySerializer.Serialize(genuine, fileName);
            //fileName = string.Format(@"{0}\Results\{1}.Impostor.bin", ResourcePath, Matcher.GetType().Name);
            //BinarySerializer.Serialize(impostor, fileName);
        }

        #endregion

        #region private

        private void SaveBadMatches(string fileName, List<string> matches)
        {
            using (StreamWriter fs = new StreamWriter(fileName, false))
            {
                foreach (var pair in matches)
                    fs.WriteLine(pair);
                fs.Close();
            }
        }

        private double DoComparisons(IList<object> features, List<string> imgCaptions, out List<double> genuine, out List<double> impostor, out List<string> falseNotMatches, out List<string> falseMatches)
        {
            genuine = new List<double>();
            impostor = new List<double>();
            int total = 28 * features.Count / 8 + (features.Count / 8) * (features.Count / 8 - 1) / 2;
            int percentDelta = total / 100;
            int idx = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            // Performing genuine comparisons
            falseNotMatches = new List<string>();
            for (int i = 0; i < features.Count / 8; i++)
                for (int j = 0; j < 7; j++)
                    for (int k = j + 1; k < 8; k++)
                    {
                        double s = Matcher.Match(features[i * 8 + j], features[i * 8 + k]);
                        //if (s == 0)
                        if (s <= 0.025)
                        {
                            string fingerprint1 = imgCaptions[i * 8 + j];
                            string fingerprint2 = imgCaptions[i * 8 + k];

                            falseNotMatches.Add(fingerprint1 + ";" + fingerprint2 + ";" + s);
                        }
                        genuine.Add(s);
                        idx++;
                        if (StatusEvent != null && (total / 100.0 < 1 || idx % percentDelta == 0))
                        {
                            int progressPercent = Convert.ToInt32(Math.Round(1.0 * idx / total * 100));
                            StatusEvent(this, new ProgressChangedEventArgs(progressPercent, "Matching"));
                        }
                    }
            // Performing impostor comparisons
            falseMatches = new List<string>();
            for (int i = 0; i < features.Count / 8 - 1; i++)
                for (int j = i + 1; j < features.Count / 8; j++)
                {
                    double s = Matcher.Match(features[i * 8], features[j * 8]);
                    //if (s > 0)
                    if (s > 0.025)
                    {
                        string fingerprint1 = imgCaptions[i * 8];
                        string fingerprint2 = imgCaptions[j * 8];
                        falseMatches.Add(fingerprint1 + ";" + fingerprint2 + ";" + s);
                    }
                    impostor.Add(s);
                    idx++;
                    if (StatusEvent != null && (total / 100.0 < 1 || idx % percentDelta == 0))
                    {
                        int progressPercent = Convert.ToInt32(Math.Round(1.0 * idx / total * 100));
                        StatusEvent(this, new ProgressChangedEventArgs(progressPercent, "Matching"));
                    }
                }

            sw.Stop();
            StatusEvent(this, new ProgressChangedEventArgs(100, "Matching"));
            GC.Collect();
            int ms = sw.Elapsed.Days * 24 * 3600 * 1000 + sw.Elapsed.Hours * 3600 * 1000 + sw.Elapsed.Minutes * 60 * 1000 +
                 sw.Elapsed.Seconds * 1000 + sw.Elapsed.Milliseconds;
            double speed;
            speed = 1.0 * ms / total;

            genuine.TrimExcess();
            impostor.TrimExcess();

            return speed;
        }

        private void ComputeAccuracy(List<double> genuine, List<double> impostor, out double eer, out double fmr100, out double fmr1000, out double zeroFmr, out List<ROCPoint> roc)
        {
            var rocBuilder = new FNMRvsFMR();
            roc = new List<ROCPoint>();
            foreach (ROCPoint rocPoint in rocBuilder.BuildROC(genuine, impostor, new SimilarityValuesComparer()))
                roc.Add(new ROCPoint(rocPoint.x / 100, rocPoint.y / 100, rocPoint.matchingValue));

            fmr100 = double.NaN;
            fmr1000 = double.NaN;
            double fnmr2 = double.NaN,
                   fmr2 = double.NaN;
            zeroFmr = roc[0].y * 100;
            for (int i = 0; i < roc.Count; i++)
            {
                if (roc[i].x <= 0.001)
                    fmr1000 = roc[i].y * 100;
                else
                    if (roc[i].x <= 0.01)
                        fmr100 = roc[i].y * 100;
                if (roc[i].y > roc[i].x)
                {
                    fnmr2 = roc[i + 1].y;
                    fmr2 = roc[i + 1].x;
                }
            }
            double fnmr1 = double.NaN,
                   fmr1 = double.NaN;
            for (int i = roc.Count - 1; i > 0; i--)
                if (roc[i].y < roc[i].x)
                {
                    fnmr1 = roc[i - 1].y;
                    fmr1 = roc[i - 1].x;
                }
            eer = (fnmr1 + fmr1 <= fnmr2 + fmr2) ? 100 * (fnmr1 + fmr1) / 2 : 100 * (fnmr2 + fmr2) / 2;
        }

        private List<object> GetFeatures(out List<string> imgCaptions)
        {
            ResourceRepository repository = new ResourceRepository(ResourcePath);
            List<object> features = new List<object>(800);
            imgCaptions = new List<string>(800);
            int percentDelta = 800 / 100;
            int idx = 0;
            for (int i = 1; i <= 100; i++)
                for (int j = 1; j <= 8; j++)
                {
                    string label = string.Format("{0}_{1}", i, j);
                    imgCaptions.Add(label);
                    object currFeat = ResourceProvider.GetResource(label, repository);
                    features.Add(currFeat);
                    idx++;
                    if (StatusEvent != null && idx % percentDelta == 0)
                    {
                        int progressPercent = Convert.ToInt32(Math.Round(1.0 * idx / 800 * 100));
                        StatusEvent(this, new ProgressChangedEventArgs(progressPercent, "Loading Features"));
                    }
                }
            return features;
        }

        private void VerifyPreconditions()
        {
            if (!Directory.Exists(ResourcePath))
                throw new InvalidOperationException("Unable to execute experiment: Invalid resource path!");
            if (Matcher == null)
                throw new InvalidOperationException("Unable to execute experiment: Unassigned matcher!");
            if (ResourceProvider == null)
                throw new InvalidOperationException("Unable to execute experiment: Unassigned resource provider!");
        }

        private class SimilarityValuesComparer : Comparer<double>
        {
            public override int Compare(double x, double y)
            {
                return (x > y) ? -1 : (x < y) ? 1 : 0;
            }
        }

        #endregion
    }
}
