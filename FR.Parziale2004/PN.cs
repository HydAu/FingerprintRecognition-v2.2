/*
 * Created by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
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
    ///     An implementation of the algorithm proposed by Parziale and Niel in 2003.
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
    ///                G. Parziale and A. Niel, "A fingerprint matching using minutiae triangulation," in 1st International Conference on Biometric Authentication, Hong Kong, China, 2004, pp. 241-248.
    ///             </item>
    ///         </list>
    ///     </para>
    /// </remarks>
    public class PN : Matcher<PNFeatures>, IMinutiaMatcher
    {
        #region public

        /// <summary>
        ///     The threshold of alpha angles.
        /// </summary>
        /// <remarks>
        ///     Alpha angles are used in the local minutia matching step. For more information refer to the original paper.
        /// </remarks>
        public double AlphaAngleThr
        {
            get { return MtiaTriplet.AlphaThreshold * 180 / Math.PI; }
            set { MtiaTriplet.AlphaThreshold = value * Math.PI / 180; }
        }

        /// <summary>
        ///     The threshold of beta angles.
        /// </summary>
        /// <remarks>
        ///     This threshold is used to compare beta angles in the local minutia matching step. For more information refer to the original paper.
        /// </remarks>
        public double BetaAngleThr
        {
            get { return MtiaTriplet.BetaThreshold * 180 / Math.PI; }
            set { MtiaTriplet.BetaThreshold = value * Math.PI / 180; }
        }

        /// <summary>
        ///     Distance threshold for the local minutia matching step.
        /// </summary>
        /// <remarks>
        ///     This threshold is used to compare minutia distances in the local minutia matching step. For more information refer to the original paper.
        /// </remarks>
        public double LocalDistThr
        {
            get { return MtiaTriplet.DistanceThreshold; }
            set { MtiaTriplet.DistanceThreshold = value; }
        }

        /// <summary>
        ///     Distance threshold for the global minutia matching step.
        /// </summary>
        /// <remarks>
        ///     This threshold is used to compare minutia distances in the global minutia matching step. For more information refer to the original paper.
        /// </remarks>
        public int GlobalDistThr
        {
            get { return gdThr; }
            set { gdThr = value; }
        }

        /// <summary>
        ///     Angle threshold for the global minutia matching step.
        /// </summary>
        /// <remarks>
        ///     This threshold is used to compare angles in the global minutia matching step. For more information refer to the original paper.
        /// </remarks>
        public double GlobalAngleThr
        {
            get { return gaThr * 180 / Math.PI; }
            set { gaThr = value * Math.PI / 180; }
        }

        /// <summary>
        ///     Matches the specified fingerprint features.
        /// </summary>
        /// <param name="query">
        ///     The query fingerprint features.
        /// </param>
        /// <param name="template">
        ///     The template fingerprint features.
        /// </param>
        /// <returns>
        ///     The fingerprint similarity value.
        /// </returns>
        public override double Match(PNFeatures query, PNFeatures template)
        {
            List<MinutiaPair> matchingMtiae;
            return Match(query, template, out matchingMtiae);
        }

        /// <summary>
        ///     Matches the specified fingerprint features and outputs the matching minutiae.
        /// </summary>
        /// <param name="query">
        ///     The query fingerprint features.
        /// </param>
        /// <param name="template">
        ///     The template fingerprint features.
        /// </param>
        /// <param name="matchingMtiae">
        ///     The matching minutiae.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the specified features has invalid type.</exception>
        /// <returns>
        ///     The fingerprint similarity value.
        /// </returns>
        public double Match(object query, object template, out List<MinutiaPair> matchingMtiae)
        {
            PNFeatures qPNFeatures = query as PNFeatures;
            PNFeatures tPNFeatures = template as PNFeatures;
            try
            {
                matchingMtiae = new List<MinutiaPair>();
                IList<MtiaeTripletPair> matchingTriplets = GetMatchingTriplets(qPNFeatures, tPNFeatures);
                if (matchingTriplets.Count == 0)
                    return 0;

                List<MinutiaPair> localMatchingMtiae = new List<MinutiaPair>(3600);
                foreach (var qMtia in qPNFeatures.Minutiae)
                    foreach (var tMtia in tPNFeatures.Minutiae)
                        localMatchingMtiae.Add(new MinutiaPair() { QueryMtia = qMtia, TemplateMtia = tMtia, MatchingValue = 1 });

                List<MinutiaPair> refMtiaePairs = GetReferenceMtiae(matchingTriplets);

                // Iterating over the reference Minutia pair
                int max = 0;
                int notMatchingCount = int.MaxValue;
                for (int i = 0; i < refMtiaePairs.Count; i++)
                {
                    List<MinutiaPair> currMatchingMtiae =
                        GetGlobalMatchingMtiae(localMatchingMtiae, refMtiaePairs[i], ref notMatchingCount);
                    if (currMatchingMtiae != null && currMatchingMtiae.Count > max)
                    {
                        max = currMatchingMtiae.Count;
                        matchingMtiae = currMatchingMtiae;
                    }
                }

                return 100 * Math.Sqrt(1.0 * max * max / (qPNFeatures.Minutiae.Count * tPNFeatures.Minutiae.Count));
            }
            catch (Exception e)
            {
                if (query.GetType() != typeof(PNFeatures) || template.GetType() != typeof(PNFeatures))
                {
                    string msg = "Unable to match fingerprints: Invalid features type!";
                    throw new ArgumentOutOfRangeException(msg, e);
                }
                throw e;
            }
        }

        #endregion

        #region private

        private List<MtiaeTripletPair> GetMatchingTriplets(PNFeatures t1, PNFeatures t2)
        {
            var mostSimilar = new List<MtiaeTripletPair>();
            foreach (MtiaTriplet queryTriplet in t1.MTriplets)
            {
                var mtpPairs = t2.FindAllSimilar(queryTriplet);
                if (mtpPairs != null)
                    mostSimilar.AddRange(mtpPairs);
            }
            return mostSimilar;
        }

        private List<MinutiaPair> GetReferenceMtiae(IList<MtiaeTripletPair> matchingTriplets)
        {
            var pairs = new List<MinutiaPair>();
            var matches = new Dictionary<MinutiaPair, byte>(60);
            foreach (MtiaeTripletPair pair in matchingTriplets)
            {
                Minutia qMtia0 = pair.queryMTp[0];
                Minutia qMtia1 = pair.queryMTp[1];
                Minutia qMtia2 = pair.queryMTp[2];

                Minutia tMtia0 = pair.templateMTp[0];
                Minutia tMtia1 = pair.templateMTp[1];
                Minutia tMtia2 = pair.templateMTp[2];

                Minutia qRefMtia = new Minutia();
                qRefMtia.X = Convert.ToInt16(Math.Round((qMtia0.X + qMtia1.X + qMtia2.X) / 3.0));
                qRefMtia.Y = Convert.ToInt16(Math.Round((qMtia0.Y + qMtia1.Y + qMtia2.Y) / 3.0));
                double diffY = (Math.Sin(qMtia0.Angle) + Math.Sin(qMtia1.Angle) + Math.Sin(qMtia2.Angle)) / 3.0;
                double diffX = (Math.Cos(qMtia0.Angle) + Math.Cos(qMtia1.Angle) + Math.Cos(qMtia2.Angle)) / 3.0;
                qRefMtia.Angle = Angle.ComputeAngle(diffX, diffY);

                Minutia tRefMtia = new Minutia();
                tRefMtia.X = Convert.ToInt16(Math.Round((tMtia0.X + tMtia1.X + tMtia2.X) / 3.0));
                tRefMtia.Y = Convert.ToInt16(Math.Round((tMtia0.Y + tMtia1.Y + tMtia2.Y) / 3.0));
                diffY = (Math.Sin(tMtia0.Angle) + Math.Sin(tMtia1.Angle) + Math.Sin(tMtia2.Angle)) / 3.0;
                diffX = (Math.Cos(tMtia0.Angle) + Math.Cos(tMtia1.Angle) + Math.Cos(tMtia2.Angle)) / 3.0;
                tRefMtia.Angle = Angle.ComputeAngle(diffX, diffY);

                var mPair = new MinutiaPair { QueryMtia = qRefMtia, TemplateMtia = tRefMtia };
                if (!matches.ContainsKey(mPair))
                {
                    matches.Add(mPair, 0);
                    pairs.Add(mPair);
                }
            }
            return pairs;
        }

        private List<MinutiaPair> GetLocalMatchingMtiae(IList<MtiaeTripletPair> matchingTriplets)
        {
            var minutiaMatches = new List<MinutiaPair>();
            var matches = new Dictionary<MinutiaPair, byte>(60);
            foreach (MtiaeTripletPair pair in matchingTriplets)
            {
                Minutia qMtia0 = pair.queryMTp[0];
                Minutia qMtia1 = pair.queryMTp[1];
                Minutia qMtia2 = pair.queryMTp[2];
                Minutia tMtia0 = pair.templateMTp[0];
                Minutia tMtia1 = pair.templateMTp[1];
                Minutia tMtia2 = pair.templateMTp[2];

                var mPair0 = new MinutiaPair { QueryMtia = qMtia0, TemplateMtia = tMtia0 };
                if (!matches.ContainsKey(mPair0))
                {
                    matches.Add(mPair0, 0);
                    minutiaMatches.Add(mPair0);
                }
                var mPair1 = new MinutiaPair { QueryMtia = qMtia1, TemplateMtia = tMtia1 };
                if (!matches.ContainsKey(mPair1))
                {
                    matches.Add(mPair1, 0);
                    minutiaMatches.Add(mPair1);
                }
                var mPair2 = new MinutiaPair { QueryMtia = qMtia2, TemplateMtia = tMtia2 };
                if (!matches.ContainsKey(mPair2))
                {
                    matches.Add(mPair2, 0);
                    minutiaMatches.Add(mPair2);
                }

            }
            return minutiaMatches;
        }
        
        private List<MinutiaPair> GetGlobalMatchingMtiae(List<MinutiaPair> localMatchingPairs, MinutiaPair refMtiaPair, ref int notMatchingCount)
        {
            List<MinutiaPair> globalMatchingMtiae = new List<MinutiaPair>(localMatchingPairs.Count);
            var qMatches = new Dictionary<Minutia, Minutia>(localMatchingPairs.Count);
            var tMatches = new Dictionary<Minutia, Minutia>(localMatchingPairs.Count);

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
                    if (dist.Compare(query, template) <= gdThr && MatchDirections(query, template))
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
            }
            if (i == localMatchingPairs.Count)
            {
                notMatchingCount = currNotMatchingMtiaCount;
                return globalMatchingMtiae;
            }
            return null;
        }

        private bool MatchDirections(Minutia query, Minutia template)
        {
            return Angle.DifferencePi(query.Angle, template.Angle) <= gaThr;
        }

        private int gdThr = 12;

        private double gaThr = Math.PI / 6;

        private readonly MtiaEuclideanDistance dist = new MtiaEuclideanDistance();

        #endregion
    }
}