/*
 * Created by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 * Created: 
 * Comments by: 
 */

using System;
using System.Collections.Generic;
using PatternRecognition.FingerprintRecognition.Core;

namespace PatternRecognition.FingerprintRecognition.FeatureRepresentation
{
    [Serializable]
    internal class JYMtiaDescriptor
    {
        #region internal

        internal JYMtiaDescriptor(SkeletonImage skeletonImage, List<Minutia> minutiae, short mainMtiaIdx, short mtiaIdx0, short mtiaIdx1)
        {
            this.minutiae = minutiae;
            this.mainMtiaIdx = mainMtiaIdx;
            nearestMtiaIdx = mtiaIdx0;
            farthestMtiaIdx = mtiaIdx1;

            MtiaEuclideanDistance dist = new MtiaEuclideanDistance();
            dist0 = dist.Compare(MainMinutia, NearestMtia);
            dist1 = dist.Compare(MainMinutia, FarthestMtia);
            if (dist1 < dist0)
            {
                nearestMtiaIdx = mtiaIdx1;
                farthestMtiaIdx = mtiaIdx0;
                double temp = dist0;
                dist0 = dist1;
                dist1 = temp;
            }

            alpha0 = ComputeAlpha(MainMinutia, NearestMtia);
            alpha1 = ComputeAlpha(MainMinutia, FarthestMtia);

            beta0 = ComputeBeta(MainMinutia, NearestMtia);
            beta1 = ComputeBeta(MainMinutia, FarthestMtia);

            ridgeCount0 = ComputeRidgeCount(skeletonImage, MainMinutia, NearestMtia);
            ridgeCount1 = ComputeRidgeCount(skeletonImage, MainMinutia, FarthestMtia);
        }

        public static implicit operator Minutia(JYMtiaDescriptor desc)
        {
            return desc.MainMinutia;
        }

        internal Minutia MainMinutia
        {
            get { return minutiae[mainMtiaIdx]; }
        }

        internal Minutia NearestMtia
        {
            get { return minutiae[nearestMtiaIdx]; }
        }

        internal Minutia FarthestMtia
        {
            get { return minutiae[farthestMtiaIdx]; }
        }

        internal static double DistanceThreshold
        {
            get { return dThr; }
            set { dThr = value; }
        }

        internal static double AngleThreshold
        {
            get { return aThr; }
            set { aThr = value; }
        }

        public override int GetHashCode()
        {
            return mainMtiaIdx * 1000000 + nearestMtiaIdx * 1000 + farthestMtiaIdx;
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2}", mainMtiaIdx, nearestMtiaIdx, farthestMtiaIdx);
        }

        internal double RotationInvariantMatch(JYMtiaDescriptor target)
        {
            double distDiff = MatchDistances(target);
            double alphaDiff = MatchAlphaAngles(target);
            double betaDiff = MatchBetaAngles(target);
            double ridgeCountDiff = MatchRidgeCounts(target);
            double mtiaTypeDiff = MatchByType(target);

            double dist = Math.Sqrt(distDiff + alphaDiff + betaDiff + ridgeCountDiff + mtiaTypeDiff);

            return dist < 66 ? (66 - dist)/66 : 0;
        }

        internal double NoRotateMatch(JYMtiaDescriptor target)
        {
            if (!MatchMtiaDirections(target))
                return 0;
            double distDiff = MatchDistances(target);
            double alphaDiff = MatchAlphaAngles(target);
            double betaDiff = MatchBetaAngles(target);
            double ridgeCountDiff = MatchRidgeCounts(target);
            double mtiaTypeDiff = MatchByType(target);

            double dist = Math.Sqrt(distDiff + alphaDiff + betaDiff + ridgeCountDiff + mtiaTypeDiff);

            return dist < 66 ? (66 - dist) / 66 : 0;
        }

        #endregion

        #region private methods

        private byte ComputeRidgeCount(SkeletonImage skeletonImage, Minutia mtia0, Minutia mtia1)
        {
            return skeletonImage.RidgeCount(mtia0.X, mtia0.Y, mtia1.X, mtia1.Y);
        }

        private double ComputeAlpha(Minutia mtia0, Minutia mtia1)
        {
            double x = mtia0.X - mtia1.X;
            double y = mtia0.Y - mtia1.Y;
            return Angle.Difference2Pi(mtia0.Angle, Angle.ComputeAngle(x, y));
        }

        private double ComputeBeta(Minutia mtia0, Minutia mtia1)
        {
            return Angle.Difference2Pi(mtia0.Angle, mtia1.Angle);
        }

        private double MatchDistances(JYMtiaDescriptor target)
        {
            double diff0 = Math.Abs(target.dist0 - dist0);
            double diff1 = Math.Abs(target.dist1 - dist1);

            return diff0 + diff1;
        }

        private bool MatchMtiaDirections(JYMtiaDescriptor target)
        {
            double diff = Angle.DifferencePi(target.MainMinutia.Angle, MainMinutia.Angle);
            if (diff >= Math.PI / 4)
                return false;
            diff = Angle.DifferencePi(target.NearestMtia.Angle, NearestMtia.Angle);
            if (diff >= Math.PI / 4)
                return false;
            diff = Angle.DifferencePi(target.FarthestMtia.Angle, FarthestMtia.Angle);
            if (diff >= Math.PI / 4)
                return false;

            return true;
        }

        private double MatchRidgeCounts(JYMtiaDescriptor target)
        {
            double diff0 = Math.Abs(target.ridgeCount0 - ridgeCount0);
            double diff1 = Math.Abs(target.ridgeCount1 - ridgeCount1);

            return 3 * (Math.Pow(diff0, 2) + Math.Pow(diff1, 2));
        }

        private double MatchAlphaAngles(JYMtiaDescriptor target)
        {
            double diff0 = Angle.DifferencePi(target.alpha0, alpha0);
            double diff1 = Angle.DifferencePi(target.alpha1, alpha1);

            return 54 * (Math.Pow(diff0, 2) + Math.Pow(diff1, 2)) / Math.PI;
        }

        private double MatchBetaAngles(JYMtiaDescriptor target)
        {
            double diff0 = Angle.DifferencePi(target.beta0, beta0);
            double diff1 = Angle.DifferencePi(target.beta1, beta1);

            return 54 * (Math.Pow(diff0, 2) + Math.Pow(diff1, 2)) / Math.PI;
        }

        private double MatchByType(JYMtiaDescriptor target)
        {
            int diff0 = target.MainMinutia.MinutiaType == MainMinutia.MinutiaType ? 0 : 1;
            int diff1 = target.NearestMtia.MinutiaType == NearestMtia.MinutiaType ? 0 : 1;
            int diff2 = target.FarthestMtia.MinutiaType == FarthestMtia.MinutiaType ? 0 : 1;
            return 3 * (diff0 + diff1 + diff2);
        }

        #endregion

        #region private fields

        private readonly short mainMtiaIdx, nearestMtiaIdx, farthestMtiaIdx;

        private readonly double dist0, dist1;

        private readonly double alpha0, alpha1, beta0, beta1;

        private byte ridgeCount0, ridgeCount1;

        private readonly List<Minutia> minutiae;

        [NonSerialized]
        private static double aThr = Math.PI / 6;

        [NonSerialized]
        private static double dThr = 12;

        #endregion
    }
}