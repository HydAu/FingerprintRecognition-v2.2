/*
 * Created by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 *             Milton García Borroto (milton.garcia@gmail.com)
 * Created: 
 * Comments by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 */

using System;
using System.Collections.Generic;
using PatternRecognition.FingerprintRecognition.Core;

namespace PatternRecognition.FingerprintRecognition.FeatureRepresentation
{
    [Serializable]
    internal class MTriplet
    {
        #region Public

        public Minutia this[int i]
        {
            get { return minutiae[MtiaIdxs[i]]; }
        }

        public MTriplet(Int16[] mIdxs, List<Minutia> minutiae)
        {
            this.minutiae = minutiae;

            mtiaIdxs = ArrangeClockwise(mIdxs, minutiae);

            Minutia[] mtiaArr = new Minutia[3];
            mtiaArr[0] = minutiae[MtiaIdxs[0]];
            mtiaArr[1] = minutiae[MtiaIdxs[1]];
            mtiaArr[2] = minutiae[MtiaIdxs[2]];

            // Computing distances and maxBeta angle
            d[0] = ed.Compare(mtiaArr[0], mtiaArr[1]);
            d[1] = ed.Compare(mtiaArr[1], mtiaArr[2]);
            d[2] = ed.Compare(mtiaArr[0], mtiaArr[2]);
            maxBeta = double.MinValue;
            for (byte i = 0; i < 2; i++)
                for (byte j = (byte)(i + 1); j < 3; j++)
                {
                    if (d[sortedDistIdxs[i]] > d[sortedDistIdxs[j]])
                    {
                        byte temp = sortedDistIdxs[i];
                        sortedDistIdxs[i] = sortedDistIdxs[j];
                        sortedDistIdxs[j] = temp;
                    }
                    double alpha = mtiaArr[i].Angle;
                    double beta = mtiaArr[j].Angle;
                    double diff = Math.Abs(beta - alpha);
                    double currBeta = Math.Min(diff, 2 * Math.PI - diff);
                    maxBeta = Math.Max(maxBeta, Math.Max(currBeta, currBeta));
                }
        }

        public static double DistanceThreshold
        {
            get { return dThr; }
            set { dThr = value; }
        }

        public static double AngleThreshold
        {
            get { return aThr; }
            set { aThr = value; }
        }

        public double MinDistance
        {
            get { return d[sortedDistIdxs[0]]; }
        }

        public double MidDistance
        {
            get { return d[sortedDistIdxs[1]]; }
        }

        public double MaxDistance
        {
            get { return d[sortedDistIdxs[2]]; }
        }

        public short[] MtiaIdxs
        {
            get { return mtiaIdxs; }
        }

        public override int GetHashCode()
        {
            int Max = Math.Max(MtiaIdxs[0], Math.Max(MtiaIdxs[1], MtiaIdxs[2]));
            int Min = Math.Min(MtiaIdxs[0], Math.Min(MtiaIdxs[1], MtiaIdxs[2]));
            int Med = (MtiaIdxs[0] + MtiaIdxs[1] + MtiaIdxs[2]) - Max - Min;
            return Max * 1000000 + Med * 1000 + Min;
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2}", MtiaIdxs[0], MtiaIdxs[1], MtiaIdxs[2]);
        }

        public double SlowRotationInvariantMatch(MTriplet target, out byte[] mtiaOrder)
        {
            byte[] matchOrder = null;
            double maxSimil = 0;
            if (Math.Abs(MaxDistance - target.MaxDistance) < dThr && Math.Abs(MidDistance - target.MidDistance) < dThr && Math.Abs(MinDistance - target.MinDistance) < dThr /* && Angle.AngleDif180(maxBeta, target.maxBeta) < aThr */)
                foreach (byte[] order in Orders)
                {
                    //double typeSim = MatchByType(target, order);
                    //if (typeSim == 0)
                    //    continue;

                    double distSim = MatchDistances(target, order);
                    if (distSim == 0)
                        continue;

                    double betaSim = MatchBetaAngles(target, order);
                    if (betaSim == 0)
                        continue;

                    double alphaSim = MatchAlphaAngles(target, order);
                    if (alphaSim == 0)
                        continue;

                    double currentSimil = 1 - (1 - distSim) * (1 - alphaSim) * (1 - betaSim);

                    if (currentSimil > maxSimil)
                    {
                        matchOrder = order;
                        maxSimil = currentSimil;
                    }
                }
            mtiaOrder = matchOrder;
            return maxSimil;
        }

        public double NoRotateMatch(MTriplet target, out byte[] mtiaOrder)
        {
            byte[] matchOrder = null;
            double maxSimil = 0;
            if (Math.Abs(MaxDistance - target.MaxDistance) < dThr && Math.Abs(MidDistance - target.MidDistance) < dThr && Math.Abs(MinDistance - target.MinDistance) < dThr /* && Angle.AngleDif180(maxBeta, target.maxBeta) < aThr*/)
                foreach (byte[] order in Orders)
                {
                    double dirSim = MatchMtiaDirections(target, order);
                    if (dirSim == 0)
                        continue;

                    double distSim = MatchDistances(target, order);
                    if (distSim == 0)
                        continue;

                    double betaSim = MatchBetaAngles(target, order);
                    if (betaSim == 0)
                        continue;

                    double alphaSim = MatchAlphaAngles(target, order);
                    if (alphaSim == 0)
                        continue;

                    double currentSimil = 1 - (1 - distSim) * (1 - alphaSim) * (1 - betaSim);

                    if (currentSimil > maxSimil)
                    {
                        matchOrder = order;
                        maxSimil = currentSimil;
                    }
                }
            mtiaOrder = matchOrder;
            return maxSimil;
        }

        #endregion

        #region private methods

        private double MatchDistances(MTriplet compareTo, byte[] order)
        {
            double diff0 = Math.Abs(d[0] - compareTo.d[order[0]]);
            if (diff0 > dThr)
                return 0;
            double diff1 = Math.Abs(d[1] - compareTo.d[order[1]]);
            if (diff1 > dThr)
                return 0;
            double diff2 = Math.Abs(d[2] - compareTo.d[order[2]]);
            if (diff2 > dThr)
                return 0;
            return 1 - (Math.Max(diff0, Math.Max(diff1, diff2))) / dThr;
        }

        private double MatchBetaAngles(MTriplet compareTo, byte[] order)
        {
            var idxArr = new[] { 0, 1, 2, 0 };
            double maxdiff = 0;
            for (int i = 0; i < 3; i++)
            {
                int j = idxArr[i + 1];
                Minutia qMtiai = minutiae[MtiaIdxs[i]];
                Minutia qMtiaj = minutiae[MtiaIdxs[j]];
                double qbeta = Angle.Difference2Pi(qMtiai.Angle, qMtiaj.Angle);

                Minutia tMtiai = compareTo.minutiae[compareTo.MtiaIdxs[order[i]]];
                Minutia tMtiaj = compareTo.minutiae[compareTo.MtiaIdxs[order[j]]];
                double tbeta = Angle.Difference2Pi(tMtiai.Angle, tMtiaj.Angle);

                double diff1 = Math.Abs(tbeta - qbeta);
                double diff = Math.Min(diff1, 2 * Math.PI - diff1);
                if (diff >= aThr)
                    return 0;
                if (diff > maxdiff)
                    maxdiff = diff;
            }

            return 1 - maxdiff / aThr;
        }

        private double MatchMtiaDirections(MTriplet compareTo, byte[] order)
        {
            double maxdiff = 0;
            for (int i = 0; i < 3; i++)
            {
                Minutia qMtiai = minutiae[MtiaIdxs[i]];
                Minutia tMtiai = compareTo.minutiae[compareTo.MtiaIdxs[order[i]]];
                double alpha = qMtiai.Angle;
                double beta = tMtiai.Angle;
                double diff1 = Math.Abs(beta - alpha);
                double diff = Math.Min(diff1, 2 * Math.PI - diff1);
                if (diff >= Math.PI / 4)
                    return 0;
                if (diff > maxdiff)
                    maxdiff = diff;
            }

            return 1;
        }

        private double MatchAlphaAngles(MTriplet compareTo, byte[] order)
        {
            double maxdiff = 0;
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (i != j)
                    {
                        Minutia qMtiai = minutiae[MtiaIdxs[i]];
                        Minutia qMtiaj = minutiae[MtiaIdxs[j]];
                        double x = qMtiai.X - qMtiaj.X;
                        double y = qMtiai.Y - qMtiaj.Y;
                        double angleij = Angle.ComputeAngle(x, y);
                        double qAlpha = Angle.Difference2Pi(qMtiai.Angle, angleij);

                        Minutia tMtiai = compareTo.minutiae[compareTo.MtiaIdxs[order[i]]];
                        Minutia tMtiaj = compareTo.minutiae[compareTo.MtiaIdxs[order[j]]];
                        x = tMtiai.X - tMtiaj.X;
                        y = tMtiai.Y - tMtiaj.Y;
                        angleij = Angle.ComputeAngle(x, y);
                        double tAlpha = Angle.Difference2Pi(tMtiai.Angle, angleij);

                        double diff1 = Math.Abs(tAlpha - qAlpha);
                        double diff = Math.Min(diff1, 2 * Math.PI - diff1);
                        if (diff >= aThr)
                            return 0;
                        if (diff > maxdiff)
                            maxdiff = diff;
                    }

            return 1 - maxdiff / aThr;
        }

        private class DoubleComparer : IComparer<double>
        {
            public int Compare(double x, double y)
            {
                return (x > y) ? 1 : -1;
            }
        }

        private short[] ArrangeClockwise(short[] idxs, List<Minutia> minutiae)
        {
            double CenterX = ((1.0 * minutiae[idxs[0]].X + minutiae[idxs[1]].X) / 2 + minutiae[idxs[2]].X) / 2;
            double CenterY = ((1.0 * minutiae[idxs[0]].Y + minutiae[idxs[1]].Y) / 2 + minutiae[idxs[2]].Y) / 2;
            var list = new SortedList<double, short>(3, new DoubleComparer());
            for (int i = 0; i < 3; i++)
            {
                Minutia minutia = minutiae[idxs[i]];
                double DX = minutia.X - CenterX;
                double DY = minutia.Y - CenterY;
                //todo Arreglar, poner un órden ante los colineales
                if (DX == 0 && DY == 0)
                {
                    return idxs;
                }
                list.Add(Angle.ComputeAngle(DX, DY), idxs[i]);
            }
            return new[] { list.Values[0], list.Values[1], list.Values[2] };
        }

        #endregion

        #region private fields

        public double[] SortedDistances
        {
            get
            {
                var sortedDistances = new double[]
                                          {
                                              d[sortedDistIdxs[0]],
                                              d[sortedDistIdxs[1]],
                                              d[sortedDistIdxs[2]]
                                          };
                return sortedDistances;
            }
        }

        private readonly short[] mtiaIdxs = new short[3];

        private readonly List<Minutia> minutiae;

        private readonly double[] d = new double[3];

        private readonly byte[] sortedDistIdxs = new[] { (byte)0, (byte)1, (byte)2 };

        private readonly double maxBeta;

        private static MtiaEuclideanDistance ed = new MtiaEuclideanDistance();

        [NonSerialized]
        public static readonly byte[][] Orders = new[]
                                                      {
                                                          new[] {(byte) 0, (byte) 1, (byte) 2},
                                                          new[] {(byte) 1, (byte) 2, (byte) 0},
                                                          new[] {(byte) 2, (byte) 0, (byte) 1}
                                                      };

        [NonSerialized]
        private static double aThr = Math.PI / 6;

        [NonSerialized]
        private static double dThr = 12;

        #endregion
    }
}