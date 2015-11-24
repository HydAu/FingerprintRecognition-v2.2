/*
 * Created by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 * Created: 
 * Comments by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 */

using System.Collections.Generic;

namespace PatternRecognition.ROC
{
    /// <summary>
    ///     Builds ROC curve of type False Not Matching Rate versus False Matching Rate.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This implementation is based on the algorithm proposed in [1].
    ///     </para>
    ///     <para>
    ///         References:
    ///     </para>
    ///     <para>
    ///         <list type="number">
    ///             <item>
    ///                T. Fawcett, "An introduction to ROC analysis," Pattern Recognition Letters, vol. 27, pp. 861-874, 2006.
    ///             </item>
    ///         </list>
    ///     </para>
    /// </remarks>
    public class FNMRvsFMR : IROCBuilder
    {
        /// <summary>
        ///     Build a ROC curve from the specified matching scores and score comparer.
        /// </summary>
        /// <param name="positiveMatching">
        ///     A collection containing the matching scores of objects in the same class.
        /// </param>
        /// <param name="negativeMatching">
        ///     A collection containing the matching scores of objects in different class.
        /// </param>
        /// <param name="scoreComparer">
        ///     A comparer for matching scores.
        /// </param>
        /// <returns>
        ///     The points that compose the ROC curve.
        /// </returns>
        public List<ROCPoint> BuildROC(ICollection<double> positiveMatching, ICollection<double> negativeMatching, IComparer<double> scoreComparer)
        {
            var matchingResults = new List<MatchingResult>();
            foreach (double d in positiveMatching)
                matchingResults.Add(new MatchingResult(MatchingType.Positive, d));
            foreach (double d in negativeMatching)
                matchingResults.Add(new MatchingResult(MatchingType.Negative, d));

            matchingResults.Sort(new MatchingResultComparer() { ValuesComparer = scoreComparer });

            return BuildROC(matchingResults, negativeMatching.Count, positiveMatching.Count);
        }

        private List<ROCPoint> BuildROC(IList<MatchingResult> sortedResults, int negativeCount, int positiveCount)
        {
            int fp = 0;
            int tp = 0;
            var curve = new List<ROCPoint>();
            for (int i = 0; i < sortedResults.Count; i++)
            {
                MatchingResult mr = sortedResults[i];
                if (mr.Type == MatchingType.Positive)
                    tp++;
                else
                    fp++;
                    ROCPoint rocPoint = new ROCPoint(100.0 * fp / negativeCount, 100 - 100.0 * tp / positiveCount, mr.ComparissonValue);
                    if (curve.Count > 1)
                    {
                        if (curve[curve.Count - 2].x == rocPoint.x)
                            curve[curve.Count - 1] = rocPoint;
                        else
                            if (curve[curve.Count - 2].y == rocPoint.y)
                                curve[curve.Count - 1] = rocPoint;
                            else
                                curve.Add(rocPoint);
                    }
                    else
                        curve.Add(rocPoint);
            }

            curve.RemoveAt(0);
            curve.Add(new ROCPoint(100, curve[curve.Count - 1].y, curve[curve.Count - 1].matchingValue));
            curve.Add(new ROCPoint(100, 0, curve[curve.Count - 1].matchingValue));
            return curve;
        }
    }
}
