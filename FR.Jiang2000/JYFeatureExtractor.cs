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
    ///     A class to extract the features used by <see cref="JY"/> to match fingerprints.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         In order to extract features from an image, the properties <see cref="MtiaExtractor"/> and <see cref="SkeletonImgExtractor"/> must be assigned.
    ///     </para>
    ///     <para>
    ///         The features can also be extracted specifying a minutia list and a skeleton image.
    ///     </para>
    /// </remarks>
    public class JYFeatureExtractor : FeatureExtractor<JYFeatures>
    {
        /// <summary>
        ///     The minutia list extractor used to compute <see cref="JYFeatures"/> in the method <see cref="ExtractFeatures(Bitmap)"/>.
        /// </summary>
        public IFeatureExtractor<List<Minutia>> MtiaExtractor { set; get; }

        /// <summary>
        ///     The skeleton image extractor used to compute <see cref="JYFeatures"/> in the method <see cref="ExtractFeatures(Bitmap)"/>.
        /// </summary>
        public IFeatureExtractor<SkeletonImage> SkeletonImgExtractor { set; get; }

        /// <summary>
        ///     Extract features of type <see cref="JYFeatures"/> from the specified image.
        /// </summary>
        /// <remarks>
        ///     This method uses the properties <see cref="MtiaExtractor"/> and <see cref="SkeletonImgExtractor"/> to extract features, so it raises an exception if any of these properties is not assigned.
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        ///     Thrown when the minutia list extractor is not assigned or the skeleton image extractor is not assigned.
        /// </exception>
        /// <param name="image">The source image to extract features from.</param>
        /// <returns>
        ///     Features of type <see cref="JYFeatures"/> extracted from the specified image.
        /// </returns>
        public override JYFeatures ExtractFeatures(Bitmap image)
        {
            try
            {
                List<Minutia> minutiae = MtiaExtractor.ExtractFeatures(image);
                SkeletonImage skeletonImg = SkeletonImgExtractor.ExtractFeatures(image);
                return ExtractFeatures(minutiae, skeletonImg);
            }
            catch(Exception e)
            {
                if (MtiaExtractor == null)
                    throw new InvalidOperationException("Unable to extract JYFeatures: Unassigned minutia list extractor!", e);
                if (SkeletonImgExtractor == null)
                    throw new InvalidOperationException("Unable to extract JYFeatures: Unassigned skeleton image extractor!", e);
                throw;
            }
        }

        /// <summary>
        ///     Extract features of type <see cref="JYFeatures"/> from the specified minutiae and skeleton image.
        /// </summary>
        /// <param name="minutiae">
        ///     The minutia list to extract the features from.
        /// </param>
        /// <param name="skeletonImg">
        ///     The skeleton image to extract the features from.
        /// </param>
        /// <returns>
        ///     Features of type <see cref="JYFeatures"/> extracted from the specified minutiae and skeleton image.
        /// </returns>
        public JYFeatures ExtractFeatures(List<Minutia> minutiae, SkeletonImage skeletonImg)
        {
            var descriptorsList = new List<JYMtiaDescriptor>();

            if (minutiae.Count > 3)
            {
                var mtiaIdx = new Dictionary<Minutia, int>();
                for (int i = 0; i < minutiae.Count; i++)
                    mtiaIdx.Add(minutiae[i], i);
                for (Int16 idx = 0; idx < minutiae.Count; idx++)
                {
                    Minutia query = minutiae[idx];
                    Int16[] nearest = GetNearest(minutiae, query);
                    for (int i = 0; i < nearest.Length - 1; i++)
                        for (int j = i + 1; j < nearest.Length; j++)
                        {
                            JYMtiaDescriptor newMTriplet = new JYMtiaDescriptor(skeletonImg, minutiae, idx, nearest[i],
                                                                                nearest[j]);
                            descriptorsList.Add(newMTriplet);
                        }
                }
                descriptorsList.TrimExcess();
            }
            return new JYFeatures(descriptorsList);
        }

        #region private

        private Int16[] GetNearest(List<Minutia> minutiae, Minutia query)
        {
            double[] distances = new double[neighborsCount];
            Int16[] nearestM = new Int16[neighborsCount];
            for (int i = 0; i < distances.Length; i++)
                distances[i] = double.MaxValue;
            MtiaEuclideanDistance dist = new MtiaEuclideanDistance();
            for (Int16 i = 0; i < minutiae.Count; i++)
                if (minutiae[i] != query)
                {
                    double CurrentDistance = dist.Compare(query, minutiae[i]);
                    int MaxIdx = 0;
                    for (int j = 1; j < neighborsCount; j++)
                        if (distances[j] > distances[MaxIdx])
                            MaxIdx = j;
                    if (CurrentDistance < distances[MaxIdx])
                    {
                        distances[MaxIdx] = CurrentDistance;
                        nearestM[MaxIdx] = i;
                    }
                }
            return nearestM;
        }

        private const byte neighborsCount = 2;

        #endregion
    }
}