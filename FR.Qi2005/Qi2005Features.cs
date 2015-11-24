/*
 * Created by: Andrés Eduardo Gutiérrez Rodríguez (andres@bioplantas.cu)
 *             Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 * Created: 
 * Comments by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 */

using System;
using System.Collections.Generic;
using PatternRecognition.FingerprintRecognition.Core;
using PatternRecognition.FingerprintRecognition.FeatureExtractors;
using PatternRecognition.FingerprintRecognition.FeatureRepresentation;
using PatternRecognition.FingerprintRecognition.Matchers;

namespace PatternRecognition.FingerprintRecognition.FeatureRepresentation
{

    /// <summary>
    ///     The features used by <see cref="QYW"/> to match fingerprints.
    /// </summary>
    /// <remarks>
    ///     The constructor of this class is internal. You must use <see cref="Qi2005FeatureExtractor"/> in order to extract these features from fingerprints.
    /// </remarks>
    [Serializable]
    public class Qi2005Features
    {
        internal List<GOwMtia> Minutiae;

        internal Qi2005Features(List<Minutia> minutiae, OrientationImage dImg)
        {
            Minutiae = new List<GOwMtia>(minutiae.Count);
            foreach (Minutia mtia in minutiae)
            {
                Minutiae.Add(new GOwMtia(mtia, dImg));
            }
        }
    }
}
