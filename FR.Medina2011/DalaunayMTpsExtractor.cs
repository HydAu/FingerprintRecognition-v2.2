/*
 * Created by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 * Created: 
 * Comments by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using PatternRecognition.FingerprintRecognition.Core;
using PatternRecognition.FingerprintRecognition.FeatureRepresentation;
using PatternRecognition.FingerprintRecognition.Matchers;

namespace PatternRecognition.FingerprintRecognition.FeatureExtractors
{
    /// <summary>
    ///     A class to extract the features used by <see cref="MPN"/> to match fingerprints.
    /// </summary>
    /// <remarks>
    ///     This class can extract features from a minutia list or from an image. In order to extract features from an image, the property <see cref="MtiaExtractor"/> must be assigned.
    /// </remarks>
    public class DalaunayMTpsExtractor : FeatureExtractor<MtripletsFeature>
    {
        /// <summary>
        ///     The minutia list extractor used to compute <see cref="MtripletsFeature"/> in the method <see cref="ExtractFeatures(Bitmap)"/>.
        /// </summary>
        public IFeatureExtractor<List<Minutia>> MtiaExtractor { set; get; }

        /// <summary>
        ///     Extract features of type <see cref="MtripletsFeature"/> from the specified image.
        /// </summary>
        /// <remarks>
        ///     This method uses the property <see cref="MtiaExtractor"/> to extract features, so it raises an exception if the property is not assigned.
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        ///     Thrown when the minutia list extractor is not assigned
        /// </exception>
        /// <param name="image">The source image to extract features from.</param>
        /// <returns>
        ///     Features of type <see cref="MtripletsFeature"/> extracted from the specified image.
        /// </returns>
        public override MtripletsFeature ExtractFeatures(Bitmap image)
        {
            try
            {
                List<Minutia> minutiae = MtiaExtractor.ExtractFeatures(image);
                return ExtractFeatures(minutiae);
            }
            catch (Exception e)
            {
                if (MtiaExtractor == null)
                    throw new InvalidOperationException("Unable to extract MtripletsFeature: Unassigned minutia list extractor!", e);
                throw;
            }
        }

        /// <summary>
        ///     Extract features of type <see cref="MtripletsFeature"/> from the specified minutiae.
        /// </summary>
        /// <param name="minutiae">
        ///     The list of <see cref="Minutia"/> to extract the features from.
        /// </param>
        /// <returns>
        ///     Features of type <see cref="MtripletsFeature"/> extracted from the specified minutiae.
        /// </returns>
        public MtripletsFeature ExtractFeatures(List<Minutia> minutiae)
        {
            List<MTriplet> mtriplets = new List<MTriplet>();
            Dictionary<int, int> triplets = new Dictionary<int, int>();

            foreach (var triangle in Delaunay2D.Triangulate(minutiae))
            {
                var idxArr = new short[]
                                 {
                                     (short)triangle.A, 
                                     (short)triangle.B, 
                                     (short)triangle.C
                                 };
                MTriplet newMTriplet = new MTriplet(idxArr, minutiae);
                int newHash = newMTriplet.GetHashCode();
                if (!triplets.ContainsKey(newHash))
                {
                    triplets.Add(newHash, 0);
                    mtriplets.Add(newMTriplet);
                }
            }

            mtriplets.TrimExcess();
            return new MtripletsFeature(mtriplets, minutiae);
        }

    }
}