/*
 * Created by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 * Created: 
 * Comments by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 */

using System;
using PatternRecognition.FingerprintRecognition.Core;
using PatternRecognition.FingerprintRecognition.FeatureExtractors;
using PatternRecognition.FingerprintRecognition.FeatureRepresentation;

namespace PatternRecognition.FingerprintRecognition.ResourceProviders
{
    /// <summary>
    ///     Allows retrieving features of type <see cref="JYFeatures"/> from a <see cref="ResourceRepository"/>.
    /// </summary>
    /// <remarks>
    ///     This features are computed from a <see cref="Minutia"/> list and <see cref="SkeletonImage"/>. This way, in order to compute <see cref="JYFeatures"/>, you must set the properties <see cref="MtiaListProvider"/> and <see cref="SkeletonImgProvider"/>.
    /// </remarks>
    public class JYFeaturesProvider : ResourceProvider<JYFeatures>
    {
        /// <summary>
        ///     Provides the <see cref="Minutia"/> list to compute the features.
        /// </summary>
        public MinutiaListProvider MtiaListProvider { get; set; }

        /// <summary>
        ///     Provides the <see cref="SkeletonImage"/> to compute features.
        /// </summary>
        public SkeletonImageProvider SkeletonImgProvider { get; set; }


        /// <summary>
        ///     Gets the signature of the resource provider.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the minutia list provider is not assigned, the skeleton image provider is not assigned, the minutia list extractor is not assigned or the skeleton image extractor is not assigned.</exception>
        /// <returns>It returns a string formed by the name of the properties <see cref="MtiaListProvider"/> and <see cref="SkeletonImgProvider"/> concatenated with ".jy".</returns>
        public override string GetSignature()
        {
            try
            {
                return string.Format("{0}_{1}.jy", MtiaListProvider.MinutiaListExtractor.GetType().Name,
                                     SkeletonImgProvider.SkeletonImageExtractor.GetType().Name);
            }
            catch(Exception e)
            {
                if (MtiaListProvider == null)
                    throw new InvalidOperationException("Unable to get signature of JYFeaturesProvider: Unassigned minutia list provider!", e);
                if (SkeletonImgProvider == null)
                    throw new InvalidOperationException("Unable to get signature of JYFeaturesProvider: Unassigned skeleton image provider!", e);
                if (MtiaListProvider.MinutiaListExtractor == null)
                    throw new InvalidOperationException("Unable to get signature of JYFeaturesProvider: Unassigned minutia list extractor!", e);
                if (SkeletonImgProvider.SkeletonImageExtractor == null)
                    throw new InvalidOperationException("Unable to get signature of JYFeaturesProvider: Unassigned skeleton image extractor!", e);
                throw;
            }
        }

        /// <summary>
        ///     Determines whether the provided <see cref="JYFeatures"/> is persistent.
        /// </summary>
        /// <returns>Always returns true.</returns>
        public override bool IsResourcePersistent()
        {
            return true;
        }

        /// <summary>
        ///     Extracts <see cref="JYFeatures"/> from the specified fingerprint and <see cref="ResourceRepository"/>.
        /// </summary>
        /// <param name="fingerprint">The fingerprint which resource is being extracted.</param>
        /// <param name="repository">The object used to store and retrieve resources.</param>
        /// <exception cref="InvalidOperationException">Thrown when the minutia list provider is not assigned, the skeleton image provider is not assigned, the minutia list extractor is not assigned or the skeleton image extractor is not assigned.</exception>
        /// <returns>The extracted <see cref="JYFeatures"/>.</returns>
        protected override JYFeatures Extract(string fingerprint, ResourceRepository repository)
        {
            try
            {
                var mtiae = MtiaListProvider.GetResource(fingerprint, repository);
                var skeletonImg = SkeletonImgProvider.GetResource(fingerprint, repository);

                return featureExtractor.ExtractFeatures(mtiae, skeletonImg);
            }
            catch (Exception)
            {
                if (MtiaListProvider == null)
                    throw new InvalidOperationException("Unable to extract JYFeatures: Unassigned minutia list provider!");
                if (SkeletonImgProvider == null)
                    throw new InvalidOperationException("Unable to extract JYFeatures: Unassigned skeleton image provider!");
                throw;
            }
        }

        private JYFeatureExtractor featureExtractor = new JYFeatureExtractor();
    }
}
