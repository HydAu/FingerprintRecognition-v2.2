/*
 * Created by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
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
    ///     The features used by <see cref="M3gl"/> to match fingerprints.
    /// </summary>
    /// <remarks>
    ///     The constructor of this class is internal. You must use <see cref="MTripletsExtractor"/> in order to extract these features from fingerprints.
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
            MTriplets.Sort(new MTComparer());

            hashTable = new Dictionary<int, List<MTriplet>>();
            foreach (MTriplet mtp in MTriplets)
            {
                int[] alphaCodes = GetAlphaCodes(mtp);
                int hash = MergeHashes(alphaCodes[0], alphaCodes[1], alphaCodes[2]);
                if (!hashTable.ContainsKey(hash))
                    hashTable.Add(hash, new List<MTriplet>());
                hashTable[hash].Add(mtp);
            }
        }

        internal List<MtripletPair> FindNoRotateAllSimilar(MTriplet queryMTp)
        {
            // Indexing by MaxDistance
            double dThr = MTriplet.DistanceThreshold;
            double d = queryMTp.MaxDistance - dThr;
            int iniIdx = BinarySearch(MTriplets, d);
            d = queryMTp.MaxDistance + dThr;

            var result = new List<MtripletPair>();
            for (int j = iniIdx; j < MTriplets.Count && MTriplets[j].MaxDistance <= d; j++)
            {
                MTriplet currMTp = MTriplets[j];
                byte[] currOrder;
                double currSim = queryMTp.NoRotateMatch(currMTp, out currOrder);
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

        #region private

        private Dictionary<int, List<MTriplet>> hashTable;

        private int BinarySearch(List<MTriplet> mtps, double value)
        {
            int low = 0;
            int high = mtps.Count - 1;
            while (low < high)
            {
                int mid = (low + high) / 2;
                if (mtps[mid].MaxDistance > value)
                    high = mid - 1;
                else if (mtps[mid].MaxDistance < value)
                    low = mid + 1;
                else
                    return mid; // found
            }
            return low; // not found
        }

        private class MTComparer : Comparer<MTriplet>
        {
            public override int Compare(MTriplet x, MTriplet y)
            {
                return Math.Sign(x.MaxDistance - y.MaxDistance);
            }
        }

        private class DoubleComparer : IComparer<double>
        {
            public int Compare(double x, double y)
            {
                return (x > y) ? 1 : -1;
            }
        }

        #endregion

        #region Generating hashes

        private int DiscretizeAngle(double d)
        {
            int code = Convert.ToInt32((d * 180 / Math.PI) / 45);
            return (code != 8) ? code : 0;
        }

        private int[] GetAllRotationsHashes(MTriplet mtp)
        {
            // Computing hash for alpha angles
            int[] alphaCodes = GetAlphaCodes(mtp);

            // Generating all possible hashes
            int[] hashes = new int[3];
            int i = 0;
            foreach (byte[] order in MTriplet.Orders)
            {
                int a0 = alphaCodes[order[0]];
                int a1 = alphaCodes[order[1]];
                int a2 = alphaCodes[order[2]];
                hashes[i++] = MergeHashes(a0, a1, a2);
            }
            return hashes;
        }

        private int[] GetAlphaCodes(MTriplet mtp)
        {
            int[] alpha = new int[3];
            for (int i = 0; i < 3; i++)
            {
                int j;
                if (i == 2)
                    j = 0;
                else
                    j = i + 1;

                Minutia qMtiai = mtp[i];
                Minutia qMtiaj = mtp[j];
                double x = qMtiai.X - qMtiaj.X;
                double y = qMtiai.Y - qMtiaj.Y;
                double angleij = Angle.ComputeAngle(x, y);
                double qAlpha = Angle.Difference2Pi(qMtiai.Angle, angleij);

                alpha[i] = DiscretizeAngle(qAlpha);
            }
            return alpha;
        }

        private int MergeHashes(int a0, int a1, int a2)
        {
            int block2 = a2;
            // Storing d1 in the next 4 bits.
            int block1 = a1 << 4;
            // Storing d0 in the next 4 bits.
            int block0 = a0 << 8;
            int hash = block0 | block1 | block2;
            return hash;
        }

        #endregion

    }
}
