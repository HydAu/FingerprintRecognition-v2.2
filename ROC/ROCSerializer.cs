/*
 * Created by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 * Created: 
 * Comments by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 */

using System;
using System.Collections.Generic;
using System.IO;

namespace PatternRecognition.ROC
{
    /// <summary>
    ///     A utility class used to serialize and deserialize ROC curves.
    /// </summary>
    public static class ROCSerializer
    {
        /// <summary>
        ///     Serialize the ROC curve to the specified file name.
        /// </summary>
        /// <param name="roc">
        ///     The points composing the ROC curve.
        /// </param>
        /// <param name="xLabel">
        ///     The label of horizontal axis in the ROC curve.
        /// </param>
        /// <param name="yLabel">
        ///     The label of vertical axis in the ROC curve.
        /// </param>
        /// <param name="matcherName">
        ///     The name of the fingerprint matcher.
        /// </param>
        /// <param name="fileName">
        ///     The file name where the curve will be serialized.
        /// </param>
        public static void Serialize(List<ROCPoint> roc, string xLabel, string yLabel, string matcherName, string fileName)
        {
            using (var fs = new StreamWriter(fileName, true))
            {
                fs.WriteLine("Matcher:;" + matcherName);
                fs.WriteLine(xLabel + ";" + yLabel);
                foreach (ROCPoint rocPoint in roc)
                    fs.WriteLine(string.Format("{0:f4};{1:f4}", rocPoint.x, rocPoint.y));
                fs.Close();
            }
        }

        /// <summary>
        ///     Serialize the ROC curve to the specified <see cref="StreamWriter"/>.
        /// </summary>
        /// <param name="roc">
        ///     The points composing the ROC curve.
        /// </param>
        /// <param name="xLabel">
        ///     The label of horizontal axis in the ROC curve.
        /// </param>
        /// <param name="yLabel">
        ///     The label of vertical axis in the ROC curve.
        /// </param>
        /// <param name="matcherName">
        ///     The name of the fingerprint matcher.
        /// </param>
        /// <param name="fs">
        ///     The <see cref="StreamWriter"/> where the curve will be serialized.
        /// </param>
        public static void Serialize(List<ROCPoint> roc, string xLabel, string yLabel, string matcherName, StreamWriter fs)
        {
                fs.WriteLine("Matcher:;" + matcherName);
                fs.WriteLine(xLabel + ";" + yLabel + ";Threshold");
                foreach (ROCPoint rocPoint in roc)
                    fs.WriteLine(string.Format("{0:f4};{1:f4};{2:f4}", rocPoint.x, rocPoint.y, rocPoint.matchingValue));
                fs.Close();
        }

        /// <summary>
        ///     Deserialize the ROC curve from the specified file name.
        /// </summary>
        /// <param name="fileName">
        ///     The file name where the ROC curve is serialized.
        /// </param>
        /// <param name="xLabel">
        ///     The label of horizontal axis in the ROC curve.
        /// </param>
        /// <param name="yLabel">
        ///     The label of vertical axis in the ROC curve.
        /// </param>
        /// <param name="matcherName">
        ///     The name of the fingerprint matcher.
        /// </param>
        /// <returns>
        ///     The points composing the ROC curve.
        /// </returns>
        public static List<ROCPoint> Deserialize(string fileName, out string xLabel, out string yLabel, out string matcherName)
        {
            var roc = new List<ROCPoint>();
            using (var sr = new StreamReader(fileName))
            {
                matcherName = sr.ReadLine();
                var line = sr.ReadLine();
                var splitStringArray = new string[1] { ";" };
                var members = line.Split(splitStringArray, StringSplitOptions.None);
                xLabel = members[0];
                yLabel = members[1];

                // Read lines from the file until the end of the file is reached.
                while ((line = sr.ReadLine()) != null)
                {
                    members = line.Split(splitStringArray, StringSplitOptions.None);
                    var rocPoint = new ROCPoint(Convert.ToDouble(members[0]), Convert.ToDouble(members[1]), Convert.ToDouble(members[2]));
                    roc.Add(rocPoint);
                }
                sr.Close();
            }
            return roc;
        }
    }
}