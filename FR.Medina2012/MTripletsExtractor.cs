/*
 * Created by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 *             Milton García Borroto (milton.garcia@gmail.com)
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
    ///     A class to extract the features used by <see cref="M3gl"/> to match fingerprints.
    /// </summary>
    /// <remarks>
    ///     This class can extract features from a minutia list or from an image. In order to extract features from an image, the property <see cref="MtiaExtractor"/> must be assigned.
    /// </remarks>
    public class MTripletsExtractor : FeatureExtractor<MtripletsFeature>
    {
        public byte NeighborsCount
        {
            set { neighborsCount = value; }
            get { return neighborsCount; }
        }

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
            catch (Exception)
            {
                if (MtiaExtractor == null)
                    throw new InvalidOperationException("Unable to extract MTriplets: Unassigned minutia list extractor!");
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
            List<MTriplet> result = new List<MTriplet>();
            Dictionary<int, int> triplets = new Dictionary<int, int>();

            var nearest = new short[minutiae.Count, neighborsCount];
            var distance = new double[minutiae.Count, neighborsCount];

            // Initializing distances
            for (int i = 0; i < minutiae.Count; i++)
                for (int j = 0; j < neighborsCount; j++)
                {
                    distance[i, j] = double.MaxValue;
                    nearest[i, j] = -1;
                }

            // Computing m-triplets
            for (short i = 0; i < minutiae.Count; i++)
            {
                // Updating nearest minutiae
                UpdateNearest(minutiae, i, nearest, distance);

                // Building m-triplets
                for (int j = 0; j < neighborsCount - 1; j++)
                    for (int k = j + 1; k < neighborsCount; k++)
                        if (nearest[i, j] != -1 && nearest[i, k] != -1)
                        {
                            if (i == nearest[i, j] || i == nearest[i, k] || nearest[i, j] == nearest[i, k])
                                throw new Exception("Wrong mtp");

                            MTriplet newMTriplet = new MTriplet(new short[] { i, nearest[i, j], nearest[i, k] }, minutiae);
                            int newHash = newMTriplet.GetHashCode();
                            if (!triplets.ContainsKey(newHash))
                            {
                                triplets.Add(newHash, 0);
                                result.Add(newMTriplet);
                            }
                        }
            }
            result.TrimExcess();
            return new MtripletsFeature(result, minutiae);
        }

        private void UpdateNearest(List<Minutia> minutiae, int idx, short[,] nearest, double[,] distance)
        {
            for (int i = idx + 1; i < minutiae.Count; i++)
            {
                double dValue = dist.Compare(minutiae[idx], minutiae[i]);

                int maxIdx = 0;
                for (int j = 1; j < neighborsCount; j++)
                    if (distance[idx, j] > distance[idx, maxIdx])
                        maxIdx = j;
                if (dValue < distance[idx, maxIdx])
                {
                    distance[idx, maxIdx] = dValue;
                    nearest[idx, maxIdx] = Convert.ToInt16(i);
                }

                maxIdx = 0;
                for (int j = 1; j < neighborsCount; j++)
                    if (distance[i, j] > distance[i, maxIdx])
                        maxIdx = j;
                if (dValue < distance[i, maxIdx])
                {
                    distance[i, maxIdx] = dValue;
                    nearest[i, maxIdx] = Convert.ToInt16(idx);
                }
            }
        }

        private MtiaEuclideanDistance dist = new MtiaEuclideanDistance();

        private byte neighborsCount = 4;
    }
}