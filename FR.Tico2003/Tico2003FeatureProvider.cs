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
    ///     Allows retrieving features of type <see cref="Tico2003Features"/> from a <see cref="ResourceRepository"/>.
    /// </summary>
    /// <remarks>
    ///     This features are computed from a <see cref="Minutia"/> list and <see cref="OrientationImage"/>. This way, in order to compute <see cref="Tico2003Features"/>, you must set the properties <see cref="MtiaListProvider"/> and <see cref="OrImgProvider"/>.
    /// </remarks>
    public class Tico2003FeatureProvider : ResourceProvider<Tico2003Features>
    {
        /// <summary>
        ///     Provides the <see cref="Minutia"/> list to compute the features.
        /// </summary>
        public MinutiaListProvider MtiaListProvider { get; set; }

        /// <summary>
        ///     Provides the <see cref="OrientationImage"/> to compute the features.
        /// </summary>
        public OrientationImageProvider OrImgProvider { get; set; }

        /// <summary>
        ///     Gets the signature of the resource provider.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the minutia list provider is not assigned, the orientation image provider is not assigned, the minutia list extractor is not assigned or the orientation image extractor is not assigned.</exception>
        /// <returns>It returns a string formed by the name of the properties <see cref="MtiaListProvider"/> and <see cref="OrImgProvider"/> concatenated with ".tk".</returns>
        public override string GetSignature()
        {
            try
            {
                return string.Format("{0}_{1}.tk", MtiaListProvider.MinutiaListExtractor.GetType().Name,
                                     OrImgProvider.OrientationImageExtractor.GetType().Name);
            }
            catch (Exception e)
            {
                if (MtiaListProvider == null)
                    throw new InvalidOperationException("Unable to get signature of Tico2003FeatureProvider: Unassigned minutia list provider!", e);
                if (OrImgProvider == null)
                    throw new InvalidOperationException("Unable to get signature of Tico2003FeatureProvider: Unassigned orientation image provider!", e);
                if (MtiaListProvider.MinutiaListExtractor == null)
                    throw new InvalidOperationException("Unable to get signature of Tico2003FeatureProvider: Unassigned minutia list extractor!", e);
                if (OrImgProvider.OrientationImageExtractor == null)
                    throw new InvalidOperationException("Unable to get signature of Tico2003FeatureProvider: Unassigned orientation image extractor!", e);
                throw;
            }
        }

        /// <summary>
        ///     Determines whether the provided <see cref="Tico2003Features"/> is persistent.
        /// </summary>
        /// <returns>Always returns true.</returns>
        public override bool IsResourcePersistent()
        {
            return true;
        }

        /// <summary>
        ///     Extracts <see cref="Tico2003Features"/> from the specified fingerprint and <see cref="ResourceRepository"/>.
        /// </summary>
        /// <param name="fingerprint">The fingerprint which resource is being extracted.</param>
        /// <param name="repository">The object used to store and retrieve resources.</param>
        /// <exception cref="InvalidOperationException">Thrown when the minutia list provider is not assigned, the orientation image provider is not assigned, the minutia list extractor is not assigned or the orientation image extractor is not assigned.</exception>
        /// <returns>The extracted <see cref="Tico2003Features"/>.</returns>
        protected override Tico2003Features Extract(string fingerprint, ResourceRepository repository)
        {
            try
            {
                var mtiae = MtiaListProvider.GetResource(fingerprint, repository);
                var dirImg = OrImgProvider.GetResource(fingerprint, repository);

                return featureExtractor.ExtractFeatures(mtiae, dirImg);
            }
            catch (Exception)
            {
                if (MtiaListProvider == null)
                    throw new InvalidOperationException("Unable to extract Tico2003Features: Unassigned minutia list provider!");
                if (OrImgProvider == null)
                    throw new InvalidOperationException("Unable to extract Tico2003Features: Unassigned orientation image provider!");
                throw;
            }
        }

        private Tico2003FeatureExtractor featureExtractor = new Tico2003FeatureExtractor();

    }
}
