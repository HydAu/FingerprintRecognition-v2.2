/*
 * Created by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 *             Milton García Borroto (milton.garcia@gmail.com)
 * Created: 
 * Comments by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 */

using System;
using System.Collections.Generic;
using PatternRecognition.FingerprintRecognition.Core;
using PatternRecognition.FingerprintRecognition.FeatureRepresentation;

namespace PatternRecognition.FingerprintRecognition.Matchers
{
    /// <summary>
    ///     The implementation of the algorithm proposed by Medina-P&eacute;rez et al. in 2012.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This is an implementation of the algorithm proposed in [1].
    ///     </para>
    ///     <para>
    ///         Take into account that this algorithm is created to work with fingerprint images at 500 dpi. Proper modifications have to be made for different image resolutions.
    ///     </para>
    ///     <para>
    ///         References:
    ///     </para>
    ///     <para>
    ///         <list type="number">
    ///             <item>
    ///                M. A. Medina-P&eacute;rez, M. Garc&iacute;a-Borroto, A. E. Gutierrez-Rodriguez, L. Altamirano-Robles, “Improving Fingerprint Verification Using Minutiae Triplets,” Sensors, vol. 12, pp. 3418–3437, 2012.
    ///             </item>
    ///         </list>
    ///     </para>
    /// </remarks>
    public class M3gl : Matcher<MtripletsFeature>, IMinutiaMatcher
    {
        #region public

        public double LocalAngleThr
        {
            get { return MTriplet.AngleThreshold * 180 / Math.PI; }
            set { MTriplet.AngleThreshold = value * Math.PI / 180; }
        }

        public double LocalDistThr
        {
            get { return MTriplet.DistanceThreshold; }
            set { MTriplet.DistanceThreshold = value; }
        }

        public int GlobalDistThr
        {
            get { return gdThr; }
            set { gdThr = value; }
        }

        public double GlobalAngleThr
        {
            get { return gaThr * 180 / Math.PI; }
            set { gaThr = value * Math.PI / 180; }
        }

        public int MtiaCountThr
        {
            get { return mtiaCountThr; }
            set { mtiaCountThr = value; }
        }

        public override double Match(MtripletsFeature query, MtripletsFeature template)
        {
            List<MinutiaPair> matchingMtiae;
            return Match(query, template, out matchingMtiae);
        }

        public double Match(object query, object template, out List<MinutiaPair> matchingMtiae)
        {
            try
            {
                MtripletsFeature qMtp = query as MtripletsFeature;
                MtripletsFeature tMtp = template as MtripletsFeature;

                matchingMtiae = new List<MinutiaPair>();
                if (qMtp.Minutiae.Count < MtiaCountThr || tMtp.Minutiae.Count < MtiaCountThr)
                    return 0;
                IList<MtripletPair> matchingTriplets = GetMatchingTriplets(qMtp, tMtp);
                if (matchingTriplets.Count == 0)
                    return 0;
                List<MinutiaPair> localMatchingMtiae = GetLocalMatchingMtiae(qMtp, tMtp, matchingTriplets);
                if (localMatchingMtiae.Count < MtiaCountThr)
                    return 0;

                int max = 0;
                int notMatchingCount = int.MaxValue;
                for (int i = 0; i < localMatchingMtiae.Count; i++)
                {
                    List<MinutiaPair> currMatchingMtiae =
                        GetGlobalMatchingMtiae(localMatchingMtiae, localMatchingMtiae[i], ref notMatchingCount);
                    if (currMatchingMtiae != null && currMatchingMtiae.Count > max)
                    {
                        max = currMatchingMtiae.Count;
                        matchingMtiae = currMatchingMtiae;
                    }
                }

                return 1.0 * max * max / (qMtp.Minutiae.Count * tMtp.Minutiae.Count);
            }
            catch (Exception e)
            {
                if (query.GetType() != typeof(MtripletsFeature) || template.GetType() != typeof(MtripletsFeature))
                {
                    string msg = "Unable to match fingerprints: Invalid features type!";
                    throw new ArgumentOutOfRangeException(msg, e);
                }
                throw e;
            }
        }

        #endregion

        #region private

        private List<MtripletPair> GetMatchingTriplets(MtripletsFeature t1, MtripletsFeature t2)
        {
            var mostSimilar = new List<MtripletPair>();
            foreach (MTriplet queryTriplet in t1.MTriplets)
            {
                var mtpPairs = t2.FindNoRotateAllSimilar(queryTriplet);
                if (mtpPairs != null)
                    mostSimilar.AddRange(mtpPairs);
            }
            mostSimilar.Sort(new MtpPairComparer());
            return mostSimilar;
        }

        private List<MinutiaPair> GetLocalMatchingMtiae(MtripletsFeature query, MtripletsFeature template, IList<MtripletPair> matchingTriplets)
        {
            var minutiaMatches = new List<MinutiaPair>();
            var qMatches = new Dictionary<Minutia, Minutia>(60);
            var tMatches = new Dictionary<Minutia, Minutia>(60);
            foreach (MtripletPair pair in matchingTriplets)
            {
                Minutia qMtia0 = pair.queryMTp[0];
                Minutia qMtia1 = pair.queryMTp[1];
                Minutia qMtia2 = pair.queryMTp[2];
                Minutia tMtia0 = pair.templateMTp[pair.templateMtiaOrder[0]];
                Minutia tMtia1 = pair.templateMTp[pair.templateMtiaOrder[1]];
                Minutia tMtia2 = pair.templateMTp[pair.templateMtiaOrder[2]];

                if (!qMatches.ContainsKey(qMtia0) || !tMatches.ContainsKey(tMtia0))
                {
                    if (!qMatches.ContainsKey(qMtia0))
                        qMatches.Add(qMtia0, tMtia0);
                    if (!tMatches.ContainsKey(tMtia0))
                        tMatches.Add(tMtia0, qMtia0);
                    minutiaMatches.Add(new MinutiaPair { QueryMtia = qMtia0, TemplateMtia = tMtia0 });
                }
                if (!qMatches.ContainsKey(qMtia1) || !tMatches.ContainsKey(tMtia1))
                {
                    if (!qMatches.ContainsKey(qMtia1))
                        qMatches.Add(qMtia1, tMtia1);
                    if (!tMatches.ContainsKey(tMtia1))
                        tMatches.Add(tMtia1, qMtia1);
                    minutiaMatches.Add(new MinutiaPair { QueryMtia = qMtia1, TemplateMtia = tMtia1 });
                }
                if (!qMatches.ContainsKey(qMtia2) || !tMatches.ContainsKey(tMtia2))
                {
                    if (!qMatches.ContainsKey(qMtia2))
                        qMatches.Add(qMtia2, tMtia2);
                    if (!tMatches.ContainsKey(tMtia2))
                        tMatches.Add(tMtia2, qMtia2);
                    minutiaMatches.Add(new MinutiaPair { QueryMtia = qMtia2, TemplateMtia = tMtia2 });
                }
            }
            return minutiaMatches;
        }

        private List<MinutiaPair> GetGlobalMatchingMtiae(List<MinutiaPair> localMatchingPairs, MinutiaPair refMtiaPair, ref int notMatchingCount)
        {
            List<MinutiaPair> globalMatchingMtiae = new List<MinutiaPair>(localMatchingPairs.Count);
            var qMatches = new Dictionary<Minutia, Minutia>(localMatchingPairs.Count);
            var tMatches = new Dictionary<Minutia, Minutia>(localMatchingPairs.Count);
            qMatches.Add(refMtiaPair.QueryMtia, refMtiaPair.TemplateMtia);
            tMatches.Add(refMtiaPair.TemplateMtia, refMtiaPair.QueryMtia);

            MtiaMapper mm = new MtiaMapper(refMtiaPair.QueryMtia, refMtiaPair.TemplateMtia);
            int currNotMatchingMtiaCount = 0;
            int i;
            for (i = 0; i < localMatchingPairs.Count; i++)
            {
                MinutiaPair mtiaPair = localMatchingPairs[i];
                if (!qMatches.ContainsKey(mtiaPair.QueryMtia) && !tMatches.ContainsKey(mtiaPair.TemplateMtia))
                {
                    Minutia query = mm.Map(mtiaPair.QueryMtia);
                    Minutia template = mtiaPair.TemplateMtia;
                    if (dist.Compare(query, template) <= gdThr && MatchDirections(query, template) && MatchBetaAngle(refMtiaPair, mtiaPair))
                    {
                        globalMatchingMtiae.Add(mtiaPair);
                        qMatches.Add(mtiaPair.QueryMtia, mtiaPair.TemplateMtia);
                        tMatches.Add(mtiaPair.TemplateMtia, mtiaPair.QueryMtia);
                    }
                    else
                        currNotMatchingMtiaCount++;
                }
                if (currNotMatchingMtiaCount >= notMatchingCount)
                    break;
                if (globalMatchingMtiae.Count + (localMatchingPairs.Count - i - 1) < MtiaCountThr)
                    break;
            }
            if (i == localMatchingPairs.Count)
            {
                notMatchingCount = currNotMatchingMtiaCount;
                globalMatchingMtiae.Add(refMtiaPair);
                return globalMatchingMtiae;
            }
            return null;
        }

        private bool MatchDirections(Minutia query, Minutia template)
        {
            double alpha = query.Angle;
            double beta = template.Angle;
            double diff = Math.Abs(beta - alpha);
            return Math.Min(diff, 2 * Math.PI - diff) <= gaThr;
        }

        private bool MatchBetaAngle(MinutiaPair mtiaPair0, MinutiaPair mtiaPair1)
        {
            Minutia qMtiai = mtiaPair0.QueryMtia;
            Minutia qMtiaj = mtiaPair1.QueryMtia;
            double qbeta = Angle.Difference2Pi(qMtiai.Angle, qMtiaj.Angle);

            Minutia tMtiai = mtiaPair0.TemplateMtia;
            Minutia tMtiaj = mtiaPair1.TemplateMtia;
            double tbeta = Angle.Difference2Pi(tMtiai.Angle, tMtiaj.Angle);

            double diff = Math.Abs(tbeta - qbeta);
            return Math.Min(diff, 2 * Math.PI - diff) > gaThr ? false : true;
        }

        private class MtpPairComparer : IComparer<MtripletPair>
        {
            public int Compare(MtripletPair x, MtripletPair y)
            {
                return (x.matchingValue == y.matchingValue) ? 0 : (x.matchingValue < y.matchingValue) ? 1 : -1;
            }
        }

        private int gdThr = 12;

        private double gaThr = Math.PI / 6;

        private readonly MtiaEuclideanDistance dist = new MtiaEuclideanDistance();

        private int mtiaCountThr = 2;

        #endregion
    }
}