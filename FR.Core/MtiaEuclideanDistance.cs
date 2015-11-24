/*
 * Created by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 * Created: 
 * Comments by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 */

using System;

namespace PatternRecognition.FingerprintRecognition.Core
{
    /// <summary>
    ///     Exposes a method to compute Euclidean distance between two minutiae.
    /// </summary>
    public class MtiaEuclideanDistance
    {
        /// <summary>
        ///     Computes Euclidean distance between the specified minutiae.
        /// </summary>
        /// <param name="m0">A minutia.</param>
        /// <param name="m1">A minutia.</param>
        /// <returns>
        ///     Distance computed from the specified minutiae.
        /// </returns>
        public double Compare(Minutia m0, Minutia m1)
        {
            double diff0 = m0.Y - m1.Y;
            double diff1 = m0.X - m1.X;
            return Math.Sqrt(diff0 * diff0 + diff1 * diff1);
        }
    }
}