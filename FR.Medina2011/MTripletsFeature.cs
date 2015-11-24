/*
 * Created by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 *             Milton García Borroto (milton.garcia@gmail.com)
 * Created: 
 * Comments by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 */

using System;
using System.Collections.Generic;
using PatternRecognition.FingerprintRecognition.Core;
using PatternRecognition.FingerprintRecognition.FeatureExtractors;
using PatternRecognition.FingerprintRecognition.Matchers;

namespace PatternRecognition.FingerprintRecognition.FeatureRepresentation
{
    internal class MtripletPair
    {
        public MTriplet queryMTp;
        public MTriplet templateMTp;
        public double matchingValue;
        public byte[] templateMtiaOrder;
    }

    /// <summary>
    ///     The features used by <see cref="MPN"/> to match fingerprints.
    /// </summary>
    /// <remarks>
    ///     The constructor of this class is internal. You must use <see cref="DalaunayMTpsExtractor"/> in order to extract these features from fingerprints.
    /// </remarks>
    [Serializable]
    public class MtripletsFeature
    {
        #region public

        internal MtripletsFeature(List<MTriplet> mtList, List<Minutia> mtiaList)
        {
            mtiaList.TrimExcess();
            Minutiae = mtiaList;

            mtList.TrimExcess();
            MTriplets = mtList;
        }
        
        internal List<MtripletPair> FindSimilarMTriplets(MTriplet queryMTp)
        {
            var result = new List<MtripletPair>();
            for (int j = 0; j < MTriplets.Count; j++)
            {
                MTriplet currMTp = MTriplets[j];
                byte[] currOrder;

                double currSim = queryMTp.Match(currMTp, out currOrder);

                if (currSim > 0)
                    result.Add(new MtripletPair
                    {
                        queryMTp = queryMTp,
                        templateMTp = currMTp,
                        matchingValue = currSim,
                        templateMtiaOrder = currOrder
                    }
                        );
            }
            if (result.Count > 0)
                return result;
            return null;
        }

        internal List<MTriplet> MTriplets { get; private set; }

        public List<Minutia> Minutiae { get; private set; }

        #endregion
    }
}
