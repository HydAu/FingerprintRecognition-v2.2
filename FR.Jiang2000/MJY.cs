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
    ///     This algorithm, proposed by Medina-P&eacute;rez et al. in 2012, is an improved version of <see cref="JY"/>. 
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This algorithm was proposed by Medina-P&eacute;rez et al. in [1]. 
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
    ///                M. A. Medina-P&eacute;rez, M. Garc&iacute;a-Borroto, A. E. Gutierrez-Rodriguez, L. Altamirano-Robles, "Improving the multiple alignments strategy for fingerprint verification," Lecture Notes in Computer Science, vol. 7329, 2012.
    ///             </item>
    ///         </list>
    ///     </para>
    /// </remarks>
    public class MJY : Matcher<JYFeatures>, IMinutiaMatcher
    {

        #region Miembros de ISimilarity<JYFeatures>

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
        public override double Match(JYFeatures query, JYFeatures template)
        {
            List<MinutiaPair> matchingMtiae;
            return Match(query, template, out matchingMtiae);
        }

        #endregion

        #region public

        /// <summary>
        ///     Distance threshold for the global minutia matching step.
        /// </summary>
        /// <remarks>
        ///     This threshold is used to compare minutia distances in the global minutia matching step. For more information refer to the original paper.
        /// </remarks>
        public int GlobalDistThr
        {
            get { return gDistThr; }
            set { gDistThr = value; }
        }

        /// <summary>
        ///     Angle threshold for the global minutia matching step.
        /// </summary>
        /// <remarks>
        ///     This threshold is used to compare angles in the global minutia matching step. For more information refer to the original paper.
        /// </remarks>
        public double GlobalAngleThr
        {
            get { return gAngThr * 180 / Math.PI; }
            set { gAngThr = value * Math.PI / 180; }
        }

        public double Match(object query, object template, out List<MinutiaPair> matchingMtiae)
        {
            JYFeatures qJYFeatures = query as JYFeatures;
            JYFeatures tJYFeatures = template as JYFeatures;
            try
            {
                matchingMtiae = null;
                var localMatchingMtiae = GetLocalMatchingMtiae(qJYFeatures, tJYFeatures);
                if (localMatchingMtiae.Count == 0)
                    return 0;
                double max = 0;
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
                
                if (matchingMtiae == null)
                    return 0;

                double sum = 0;
                foreach (var mtiaPair in matchingMtiae)
                {
                    sum += 0.5 + 0.5 * mtiaPair.MatchingValue;
                }

                return 100.0 * max / Math.Max(qJYFeatures.Minutiae.Count, tJYFeatures.Minutiae.Count);
            }
            catch (Exception e)
            {
                if (query.GetType() != typeof(JYFeatures) || template.GetType() != typeof(JYFeatures))
                {
                    string msg = "Unable to match fingerprints: Invalid features type!";
                    throw new ArgumentOutOfRangeException(msg, e);
                }
                throw e;
            }
        }

        #endregion

        #region private

        private class JYTriplet
        {
            public MinutiaPair MainMinutia { set; get; }
            public MinutiaPair NearestMtia { set; get; }
            public MinutiaPair FarthestMtia { set; get; }
            public double MatchingValue { set; get; }
        }

        private IList<MinutiaPair> GetLocalMatchingMtiae(JYFeatures query, JYFeatures template)
        {
            var triplets = new List<JYTriplet>(query.Minutiae.Count * template.Minutiae.Count);
            for (int i = 0; i < query.Minutiae.Count; i++)
            {
                var qMtia = query.Minutiae[i];
                for (int j = 0; j < template.Minutiae.Count; j++)
                {
                    var tMtia = template.Minutiae[j];
                    double currSim = qMtia.RotationInvariantMatch(tMtia);

                    if (currSim != 0)
                    {
                        JYTriplet currTriplet = new JYTriplet();
                        var currMtiaPair = new MinutiaPair
                        {
                            QueryMtia = qMtia.MainMinutia,
                            TemplateMtia = tMtia.MainMinutia,
                            MatchingValue = currSim
                        };
                        currTriplet.MainMinutia = currMtiaPair;

                        currMtiaPair = new MinutiaPair
                        {
                            QueryMtia = qMtia.NearestMtia,
                            TemplateMtia = tMtia.NearestMtia,
                            MatchingValue = currSim
                        };
                        currTriplet.NearestMtia = currMtiaPair;

                        currMtiaPair = new MinutiaPair
                        {
                            QueryMtia = qMtia.FarthestMtia,
                            TemplateMtia = tMtia.FarthestMtia,
                            MatchingValue = currSim
                        };
                        currTriplet.FarthestMtia = currMtiaPair;

                        currTriplet.MatchingValue = currSim;
                        triplets.Add(currTriplet);
                    }
                }
            }
            triplets.Sort(new MtiaTripletComparer());
            var mtiaPairs = new List<MinutiaPair>(3 * triplets.Count);
            foreach (var triplet in triplets)
            {
                mtiaPairs.Add(triplet.MainMinutia);
                mtiaPairs.Add(triplet.NearestMtia);
                mtiaPairs.Add(triplet.FarthestMtia);
            }

            var qMatches = new Dictionary<Minutia, Minutia>(60);
            var tMatches = new Dictionary<Minutia, Minutia>(60);
            var matchingPairs = new List<MinutiaPair>(60);
            for (int i = 0; i < mtiaPairs.Count; i++)
            {
                var pair = mtiaPairs[i];
                if (!qMatches.ContainsKey(pair.QueryMtia) || !tMatches.ContainsKey(pair.TemplateMtia))
                {
                    matchingPairs.Add(pair);
                    if (!qMatches.ContainsKey(pair.QueryMtia))
                        qMatches.Add(pair.QueryMtia, pair.TemplateMtia);
                    if (!tMatches.ContainsKey(pair.TemplateMtia))
                        tMatches.Add(pair.TemplateMtia, pair.QueryMtia);
                }
            }

            return matchingPairs;
        }

        private List<MinutiaPair> GetGlobalMatchingMtiae(IList<MinutiaPair> localMatchingPairs, MinutiaPair refMtiaPair, ref int notMatchingCount)
        {
            var globalMatchingMtiae = new List<MinutiaPair>(localMatchingPairs.Count);
            var qMatches = new Dictionary<Minutia, Minutia>(localMatchingPairs.Count);
            var tMatches = new Dictionary<Minutia, Minutia>(localMatchingPairs.Count);
            qMatches.Add(refMtiaPair.QueryMtia, refMtiaPair.TemplateMtia);
            tMatches.Add(refMtiaPair.TemplateMtia, refMtiaPair.QueryMtia);

            MtiaMapper mm = new MtiaMapper(refMtiaPair.QueryMtia, refMtiaPair.TemplateMtia);
            Minutia refQuery = mm.Map(refMtiaPair.QueryMtia);
            Minutia refTemplate = refMtiaPair.TemplateMtia;
            int currNotMatchingMtiaCount = 0;
            int i;
            for (i = 0; i < localMatchingPairs.Count; i++)
            {
                MinutiaPair mtiaPair = localMatchingPairs[i];
                if (!qMatches.ContainsKey(mtiaPair.QueryMtia) && !tMatches.ContainsKey(mtiaPair.TemplateMtia))
                {
                    Minutia query = mm.Map(mtiaPair.QueryMtia);
                    Minutia template = mtiaPair.TemplateMtia;
                    if (MatchDistance(refQuery, refTemplate, query, template) && MatchDirections(query, template) && MatchPosition(refQuery, refTemplate, query, template))
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
                //if (globalMatchingMtiae.Count + (localMatchingPairs.Count - i - 1) < MtiaCountThr)
                //    break;
            }
            if (i == localMatchingPairs.Count)
            {
                notMatchingCount = currNotMatchingMtiaCount;
                globalMatchingMtiae.Add(refMtiaPair);
                return globalMatchingMtiae;
            }
            return globalMatchingMtiae;
        }

        private class MtiaMapper
        {
            public MtiaMapper(Minutia query, Minutia template)
            {
                dAngle = template.Angle - query.Angle;
                this.template = template;
                this.query = query;
            }

            public Minutia Map(Minutia m)
            {
                return new Minutia
                {
                    Angle = m.Angle + dAngle,
                    X = Convert.ToInt16(Math.Round((m.X - query.X) * Math.Cos(dAngle) - (m.Y - query.Y) * Math.Sin(dAngle) + template.X)),
                    Y = Convert.ToInt16(Math.Round((m.X - query.X) * Math.Sin(dAngle) + (m.Y - query.Y) * Math.Cos(dAngle) + template.Y))
                };
            }

            private double dAngle;
            private Minutia template;
            private Minutia query;
        }

        private bool MatchDistance(Minutia refQuery, Minutia refTemplate, Minutia query, Minutia template)
        {
            double d0 = dist.Compare(refQuery, query);
            double d1 = dist.Compare(refTemplate, template);
            return Math.Abs(d0 - d1) <= gDistThr;
        }

        private bool MatchPosition(Minutia refQuery, Minutia refTemplate, Minutia query, Minutia template)
        {
            Minutia qMtiai = refQuery;
            Minutia qMtiaj = query;
            double x = qMtiai.X - qMtiaj.X;
            double y = qMtiai.Y - qMtiaj.Y;
            double qAngle = Angle.ComputeAngle(x, y);

            Minutia tMtiai = refTemplate;
            Minutia tMtiaj = template;
            x = tMtiai.X - tMtiaj.X;
            y = tMtiai.Y - tMtiaj.Y;
            double tAngle = Angle.ComputeAngle(x, y);

            return Angle.DifferencePi(qAngle, tAngle) <= gAngThr;
        }

        private bool MatchDirections(Minutia query, Minutia template)
        {
            return Angle.DifferencePi(query.Angle, template.Angle) <= gAngThr;
        }

        private class MtiaTripletComparer : IComparer<JYTriplet>
        {
            public int Compare(JYTriplet x, JYTriplet y)
            {
                return (x == y) ? 0 : (x.MatchingValue < y.MatchingValue) ? 1 : -1;
            }
        }

        private int gDistThr = 8;

        private double gAngThr = Math.PI / 6;

        private readonly MtiaEuclideanDistance dist = new MtiaEuclideanDistance();

        #endregion
    }
}