/*
 * Created by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 * Created: 
 * Comments by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 */

using System;
using System.Collections.Generic;
using PatternRecognition.FingerprintRecognition.Core;

namespace PatternRecognition.FingerprintRecognition.FeatureRepresentation
{
    [Serializable]
    internal class MtiaTriplet
    {
        #region internal

        internal Minutia this[int i]
        {
            get { return minutiae[MtiaIdxs[i]]; }
        }

        internal MtiaTriplet(Int16[] mIdxs, List<Minutia> minutiae)
        {
            this.minutiae = minutiae;
            mtiaIdxs = mIdxs;

            Minutia[] mtiaArr = new Minutia[3];
            mtiaArr[0] = minutiae[MtiaIdxs[0]];
            mtiaArr[1] = minutiae[MtiaIdxs[1]];
            mtiaArr[2] = minutiae[MtiaIdxs[2]];

            d[0] = dist.Compare(mtiaArr[0], mtiaArr[1]);
            d[1] = dist.Compare(mtiaArr[1], mtiaArr[2]);
            d[2] = dist.Compare(mtiaArr[0], mtiaArr[2]);
        }

        internal static double DistanceThreshold
        {
            get { return dThr; }
            set { dThr = value; }
        }

        internal static double AlphaThreshold
        {
            get { return alphaThr; }
            set { alphaThr = value; }
        }

        internal static double BetaThreshold
        {
            get { return betaThr; }
            set { betaThr = value; }
        }

        internal short[] MtiaIdxs
        {
            get { return mtiaIdxs; }
        }

        internal bool Match(MtiaTriplet target)
        {
            return MatchDistances(target) && MatchAlphaAngles(target) && MatchBetaAngles(target);
        }

        #endregion

        #region public

        public override int GetHashCode()
        {
            return MtiaIdxs[0] * 1000000 + MtiaIdxs[1] * 1000 + MtiaIdxs[2];
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2}", MtiaIdxs[0], MtiaIdxs[1], MtiaIdxs[2]);
        }

        #endregion

        #region private methods

        private bool MatchDistances(MtiaTriplet compareTo)
        {
            double ratio = Math.Abs(d[0] - compareTo.d[0]) / Math.Min(d[0], compareTo.d[0]);
            if (ratio >= dThr)
                return false;
            ratio = Math.Abs(d[1] - compareTo.d[1]) / Math.Min(d[1], compareTo.d[1]);
            if (ratio >= dThr)
                return false;
            ratio = Math.Abs(d[2] - compareTo.d[2]) / Math.Min(d[2], compareTo.d[2]);
            if (ratio >= dThr)
                return false;
            return true;
        }

        private bool MatchAlphaAngles(MtiaTriplet compareTo)
        {
            var idxArr = new[] { 0, 1, 2, 0 };
            for (int i = 0; i < 3; i++)
            {
                int j = idxArr[i + 1];
                Minutia qMtiai = minutiae[MtiaIdxs[i]];
                Minutia qMtiaj = minutiae[MtiaIdxs[j]];
                double qAlpha = Angle.DifferencePi(qMtiai.Angle, qMtiaj.Angle);

                Minutia tMtiai = compareTo.minutiae[compareTo.MtiaIdxs[i]];
                Minutia tMtiaj = compareTo.minutiae[compareTo.MtiaIdxs[j]];
                double tAlpha = Angle.DifferencePi(tMtiai.Angle, tMtiaj.Angle);

                double diff = Angle.DifferencePi(qAlpha, tAlpha);
                if (diff >= alphaThr)
                    return false;
            }

            return true;
        }

        private bool MatchBetaAngles(MtiaTriplet compareTo)
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (i != j)
                    {
                        Minutia qMtiai = minutiae[MtiaIdxs[i]];
                        Minutia qMtiaj = minutiae[MtiaIdxs[j]];
                        double x = qMtiai.X - qMtiaj.X;
                        double y = qMtiai.Y - qMtiaj.Y;
                        double angleij = Angle.ComputeAngle(x, y);
                        double qBeta = Angle.DifferencePi(qMtiai.Angle, angleij);

                        Minutia tMtiai = compareTo.minutiae[compareTo.MtiaIdxs[i]];
                        Minutia tMtiaj = compareTo.minutiae[compareTo.MtiaIdxs[j]];
                        x = tMtiai.X - tMtiaj.X;
                        y = tMtiai.Y - tMtiaj.Y;
                        angleij = Angle.ComputeAngle(x, y);
                        double tBeta = Angle.DifferencePi(tMtiai.Angle, angleij);

                        double diff = Angle.DifferencePi(qBeta, tBeta);
                        if (diff >= betaThr)
                            return false;
                    }

            return true;
        }

        #endregion

        #region private fields

        private readonly short[] mtiaIdxs = new short[3];

        private readonly List<Minutia> minutiae;

        private readonly double[] d = new double[3];

        [NonSerialized]
        private static readonly byte[][] Orders = new[]
                                                      {
                                                          new[] {(byte) 0, (byte) 1, (byte) 2},
                                                          new[] {(byte) 1, (byte) 2, (byte) 0},
                                                          new[] {(byte) 2, (byte) 0, (byte) 1}
                                                      };

        [NonSerialized]
        private static double alphaThr = Math.PI / 12;

        [NonSerialized]
        private static double betaThr = Math.PI / 9;

        [NonSerialized]
        private static double dThr = 0.2;

        [NonSerialized]
        private static MtiaEuclideanDistance dist = new MtiaEuclideanDistance();

        #endregion
    }
}