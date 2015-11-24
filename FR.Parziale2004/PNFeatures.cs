/*
 * Created by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
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
    internal class MtiaeTripletPair
    {
        public MtiaTriplet queryMTp;
        public MtiaTriplet templateMTp;
    }

    /// <summary>
    ///     The features used by <see cref="PN"/> to match fingerprints.
    /// </summary>
    /// <remarks>
    ///     The constructor of this class is internal. You must use <see cref="PNFeatureExtractor"/> in order to extract these features from fingerprints.
    /// </remarks>
    [Serializable]
    public class PNFeatures
    {
        #region internal

        internal PNFeatures(List<MtiaTriplet> mtList, List<Minutia> mtiaList)
        {
            Minutiae = mtiaList;
            MTriplets = mtList;
        }

        internal List<MtiaeTripletPair> FindAllSimilar(MtiaTriplet queryMTp)
        {
            var result = new List<MtiaeTripletPair>();
            for (int j = 0; j < MTriplets.Count; j++)
            {
                MtiaTriplet currMTp = MTriplets[j];
                if (queryMTp.Match(currMTp))
                    result.Add(new MtiaeTripletPair
                                   {
                                       queryMTp = queryMTp,
                                       templateMTp = currMTp,
                                   }
                        );
            }
            if (result.Count > 0)
                return result;
            return null;
        }

        internal List<MtiaTriplet> MTriplets { get; private set; }

        internal List<Minutia> Minutiae { get; private set; }

        #endregion
    }
}
