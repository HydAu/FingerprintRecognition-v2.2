/*
 * Created by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 * Created: 
 * Comments by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 */

using System;
using System.Collections.Generic;
using System.IO;
using PatternRecognition.FingerprintRecognition.Core;
using PatternRecognition.FingerprintRecognition.FeatureExtractors;
using PatternRecognition.FingerprintRecognition.FeatureRepresentation;
using PatternRecognition.FingerprintRecognition.ResourceProviders;

namespace PatternRecognition.FingerprintRecognition.ResourceProviders
{
    /// <summary>
    ///     Allows retrieving features of type <see cref="PNFeatures"/> from a <see cref="ResourceRepository"/>.
    /// </summary>
    /// <remarks>
    ///     This features are computed from a <see cref="Minutia"/> list. This way, you must set the property <see cref="MtiaListProvider"/> in order to compute <see cref="PNFeatures"/>.
    /// </remarks>
    public class PNFeatureProvider : ResourceProvider<PNFeatures>
    {
        /// <summary>
        ///     Provides the <see cref="Minutia"/> list to compute the features.
        /// </summary>
        public MinutiaListProvider MtiaListProvider { get; set; }

        /// <summary>
        ///     Gets the signature of the resource provider.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the minutia list provider is not assigned or the minutia list extractor is not assigned.</exception>
        /// <returns>It returns a string formed by the name of the property <see cref="MtiaListProvider"/> concatenated with ".pn".</returns>
        public override string GetSignature()
        {
            try
            {
                return string.Format("{0}.pn", MtiaListProvider.MinutiaListExtractor.GetType().Name);
            }
            catch (Exception e)
            {
                if (MtiaListProvider == null)
                    throw new InvalidOperationException("Unable to get signature of PNFeatureProvider: Unassigned minutia list provider!", e);
                if (MtiaListProvider.MinutiaListExtractor == null)
                    throw new InvalidOperationException("Unable to get signature of Qi2005FeatureProvider: Unassigned minutia list extractor!", e);
                throw;
            }
        }

        /// <summary>
        ///     Determines whether the provided <see cref="PNFeatures"/> is persistent.
        /// </summary>
        /// <returns>Always returns true.</returns>
        public override bool IsResourcePersistent()
        {
            return true;
        }

        /// <summary>
        ///     Extracts <see cref="PNFeatures"/> from the specified fingerprint and <see cref="ResourceRepository"/>.
        /// </summary>
        /// <param name="fingerprint">The fingerprint which resource is being extracted.</param>
        /// <param name="repository">The object used to store and retrieve resources.</param>
        /// <exception cref="InvalidOperationException">Thrown when the minutia list provider is not assigned or the minutia list extractor is not assigned.</exception>
        /// <returns>The extracted <see cref="PNFeatures"/>.</returns>
        protected override PNFeatures Extract(string fingerprint, ResourceRepository repository)
        {
            try
            {
                List<Minutia> mtiae = MtiaListProvider.GetResource(fingerprint, repository);

                //using (StreamWriter sw = new StreamWriter("d:\\Points.txt"))
                //{
                //    foreach (var minutia in mtiae)
                //        sw.WriteLine("(" + minutia.X + "," + minutia.Y + ")");
                //    sw.Close();
                //}


                return mTripletsCalculator.ExtractFeatures(mtiae);
            }
            catch (Exception)
            {
                if (MtiaListProvider == null)
                    throw new InvalidOperationException("Unable to extract PNFeatures: Unassigned minutia list provider!");
                throw;
            }
        }

        private readonly PNFeatureExtractor mTripletsCalculator = new PNFeatureExtractor();

    }
}