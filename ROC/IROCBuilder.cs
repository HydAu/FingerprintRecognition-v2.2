/*
 * Created by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 * Created: 
 * Comments by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 */

using System.Collections.Generic;

namespace PatternRecognition.ROC
{
    /// <summary>
    ///     Represents a point in the ROC curve.
    /// </summary>
    public struct ROCPoint
    {
        /// <summary>
        ///     The value of the point in the horizontal axis.
        /// </summary>
        public double x;

        /// <summary>
        ///     The value of the point in the vertical axis.
        /// </summary>
        public double y;

        public double matchingValue;

        /// <summary>
        ///     Initialize a point with the specified values of horizontal and vertical axis.
        /// </summary>
        /// <param name="x">
        ///     The value of the new point in the horizontal axis.
        /// </param>
        /// <param name="y">
        ///     The value of the new point in the vertical axis.
        /// </param>
        public ROCPoint(double x, double y, double matchingValue)
        {
            this.x = x;
            this.y = y;
            this.matchingValue = matchingValue;
        }
    }

    /// <summary>
    ///     Represents an algorithm to build ROC curves.
    /// </summary>
    public interface IROCBuilder
    {
        /// <summary>
        ///     Build a ROC curve from the specified matching scores and score comparer.
        /// </summary>
        /// <param name="sameClassScores">
        ///     A collection containing the matching scores of objects in the same class.
        /// </param>
        /// <param name="diffClassScores">
        ///     A collection containing the matching scores of objects in different class.
        /// </param>
        /// <param name="scoreComparer">
        ///     A comparer for matching scores.
        /// </param>
        /// <returns>
        ///     The points that compose the ROC curve.
        /// </returns>
        List<ROCPoint> BuildROC(ICollection<double> sameClassScores, ICollection<double> diffClassScores, IComparer<double> scoreComparer);
    }

    internal enum MatchingType
    {
        Positive,
        Negative
    } ;

    internal struct MatchingResult
    {
        internal MatchingType Type;
        internal double ComparissonValue;
        internal MatchingResult(MatchingType type, double dissimilarity)
        {
            Type = type;
            ComparissonValue = dissimilarity;
        }
    }

    internal class MatchingResultComparer : Comparer<MatchingResult>
    {
        public override int Compare(MatchingResult x, MatchingResult y)
        {
            int value = ValuesComparer.Compare(x.ComparissonValue, y.ComparissonValue);
            return value == 0 ? ((x.Type == MatchingType.Negative) ? -1 : 1) : value;
        }

        internal IComparer<double> ValuesComparer { set; get; }
    }
}