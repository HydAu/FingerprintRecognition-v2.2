/*
 * Created by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 * Created: 
 * Comments by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using PatternRecognition.FingerprintRecognition.Core;

namespace PatternRecognition.FingerprintRecognition.ResourceProviders
{
    /// <summary>
    ///     Allows retrieving minutia list from a <see cref="ResourceRepository"/>.
    /// </summary>
    public class MinutiaListProvider : IResourceProvider<List<Minutia>>
    {
        /// <summary>
        ///     Gets minutia list from the specified fingerprint and <see cref="ResourceRepository"/>.
        /// </summary>
        /// <param name="fingerprint">The fingerprint which minutia list is being retrieved.</param>
        /// <param name="repository">The object used to store and retrieve resources.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the fingerprint is invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the minutia list extractor is not assigned.</exception>
        /// <returns>The retrieved minutia list.</returns>
        object IResourceProvider.GetResource(string fingerprint, ResourceRepository repository)
        {
            return GetResource(fingerprint, repository);
        }

        /// <summary>
        ///     Gets minutia list from the specified fingerprint and <see cref="ResourceRepository"/>.
        /// </summary>
        /// <param name="fingerprint">The fingerprint which minutia list is being retrieved.</param>
        /// <param name="repository">The object used to store and retrieve resources.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the fingerprint is invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the minutia list extractor is not assigned.</exception>
        /// <returns>The retrieved minutia list.</returns>
        public List<Minutia> GetResource(string fingerprint, ResourceRepository repository)
        {
            bool isPersistent = IsResourcePersistent();
            string resourceName =
                string.Format("{0}.{1}", fingerprint, GetSignature());
            if (isPersistent && repository.ResourceExists(resourceName))
                return MinutiaListSerializer.FromByteArray(repository.RetrieveResource(resourceName));

            List<Minutia> resource = Extract(fingerprint, repository);
            if (resource == null)
                return null;

            if (isPersistent)
                repository.StoreResource(resourceName, MinutiaListSerializer.ToByteArray(resource));
            return resource;
        }

        /// <summary>
        ///     Gets the signature of the <see cref="MinutiaListProvider"/>.
        /// </summary>
        /// <returns>It returns a string formed by the name of the property <see cref="MinutiaListExtractor"/> concatenated with ".mta".</returns>
        public string GetSignature()
        {
            return string.Format("{0}.mta", MinutiaListExtractor.GetType().Name);
        }

        /// <summary>
        ///     Determines whether the provided minutia list is persistent.
        /// </summary>
        /// <returns>Always returns true.</returns>
        public bool IsResourcePersistent()
        {
            return true;
        }

        /// <summary>
        ///     Used to extract minutia list in case that the resource have not being saved.
        /// </summary>
        public IFeatureExtractor<List<Minutia>> MinutiaListExtractor { set; get; }

        #region private

        private List<Minutia> Extract(string fingerprintLabel, ResourceRepository repository)
        {
            Bitmap image = imageProvider.GetResource(fingerprintLabel, repository);
            if (image == null)
                throw new ArgumentOutOfRangeException(fingerprintLabel, "Unable to extract minutia list: Invalid fingerprint!");
            if (MinutiaListExtractor == null)
                throw new InvalidOperationException("Unable to extract minutia list: Unassigned minutia list extractor!");
            return MinutiaListExtractor.ExtractFeatures(image);
        }

        private readonly FingerprintImageProvider imageProvider = new FingerprintImageProvider();

        #endregion
    }
}