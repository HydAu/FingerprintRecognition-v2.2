/*
 * Created by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 *             Andrés Eduardo Gutiérrez Rodríguez (andres@bioplantas.cu)
 *             
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
    ///     The features used by <see cref="TK"/> and <see cref="MTK"/> to match fingerprints.
    /// </summary>
    /// <remarks>
    ///     The constructor of this class is internal. You must use <see cref="Tico2003FeatureExtractor"/> in order to extract these features from fingerprints.
    /// </remarks>
    [Serializable]
    public class Tico2003Features
    {
        internal List<OBMtiaDescriptor> Minutiae;

        internal Tico2003Features(List<Minutia> minutiae, OrientationImage dImg)
        {
            Minutiae = new List<OBMtiaDescriptor>(minutiae.Count);
            for (short i = 0; i < minutiae.Count; i++)
            {
                OBMtiaDescriptor mtiaDescriptor = new OBMtiaDescriptor(minutiae[i], dImg);
                Minutiae.Add(mtiaDescriptor);
            }
        }

    }
}
