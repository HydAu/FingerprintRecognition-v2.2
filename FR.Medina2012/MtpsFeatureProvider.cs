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
    ///     Allows retrieving features of type <see cref="MtripletsFeature"/> from a <see cref="ResourceRepository"/>.
    /// </summary>
    /// <remarks>
    ///     This features are computed from a <see cref="Minutia"/> list. This way, you must set the property <see cref="MtiaListProvider"/> in order to compute <see cref="MtripletsFeature"/>.
    /// </remarks>
    public class MtpsFeatureProvider : ResourceProvider<MtripletsFeature>
    {
        #region public

        /// <summary>
        ///     Provides the <see cref="Minutia"/> list to compute the features.
        /// </summary>
        public MinutiaListProvider MtiaListProvider { get; set; }

        public byte NeighborsCount
        {
            set
            {
                mTripletsCalculator.NeighborsCount = value;
            }
            get
            {
                return mTripletsCalculator.NeighborsCount;
            }
        }

        #endregion

        /// <summary>
        ///     Extracts <see cref="MtripletsFeature"/> from the specified fingerprint and <see cref="ResourceRepository"/>.
        /// </summary>
        /// <param name="fingerprint">The fingerprint which resource is being extracted.</param>
        /// <param name="repository">The object used to store and retrieve resources.</param>
        /// <exception cref="InvalidOperationException">Thrown when the minutia list provider is not assigned or the minutia list extractor is not assigned.</exception>
        /// <returns>The extracted <see cref="MtripletsFeature"/>.</returns>
        protected override MtripletsFeature Extract(string fingerprint, ResourceRepository repository)
        {
            try
            {
                var mtiae = MtiaListProvider.GetResource(fingerprint, repository);
                return mTripletsCalculator.ExtractFeatures(mtiae);
            }
            catch (Exception e)
            {
                if (MtiaListProvider == null)
                    throw new InvalidOperationException("Unable to extract PNFeatures: Unassigned minutia list provider!", e);
                throw;
            }
        }

        /// <summary>
        ///     Gets the signature of the resource provider.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the minutia list provider is not assigned or the minutia list extractor is not assigned.</exception>
        /// <returns>It returns a string formed by the name of the property <see cref="MtiaListProvider"/> concatenated with ".mtp".</returns>
        public override string GetSignature()
        {
            try
            {
                return string.Format("{0}({1}).mtp", MtiaListProvider.MinutiaListExtractor.GetType().Name, mTripletsCalculator.NeighborsCount);
            }
            catch (Exception)
            {
                if (MtiaListProvider == null)
                    throw new InvalidOperationException("Unable to get signature of MtpsFeatureProvider: Unassigned minutia list provider!");
                if (MtiaListProvider.MinutiaListExtractor == null)
                    throw new InvalidOperationException("Unable to get signature of MtpsFeatureProvider: Unassigned minutia list extractor!");
                throw;
            }
        }

        /// <summary>
        ///     Determines whether the provided <see cref="MtripletsFeature"/> is persistent.
        /// </summary>
        /// <returns>Always returns true.</returns>
        public override bool IsResourcePersistent()
        {
            return true;
        }

        private readonly MTripletsExtractor mTripletsCalculator = new MTripletsExtractor();

    }
}