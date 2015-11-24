/*
 * Created by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 * Created: 
 * Comments by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 */

using System;
using System.Drawing;
using PatternRecognition.FingerprintRecognition.Core;

namespace PatternRecognition.FingerprintRecognition.ResourceProviders
{
    /// <summary>
    ///     Allows retrieving orientation image from a <see cref="ResourceRepository"/>.
    /// </summary>
    public class OrientationImageProvider : IResourceProvider<OrientationImage>
    {
        /// <summary>
        ///     Gets orientation image from the specified fingerprint and <see cref="ResourceRepository"/>.
        /// </summary>
        /// <param name="fingerprint">The fingerprint which orientation image is being retrieved.</param>
        /// <param name="repository">The object used to store and retrieve resources.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the fingerprint is invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the orientation image extractor is not assigned.</exception>
        /// <returns>The retrieved orientation image.</returns>
        object IResourceProvider.GetResource(string fingerprint, ResourceRepository repository)
        {
            return GetResource(fingerprint, repository);
        }

        /// <summary>
        ///     Gets orientation image from the specified fingerprint and <see cref="ResourceRepository"/>.
        /// </summary>
        /// <param name="fingerprint">The fingerprint which orientation image are being retrieved.</param>
        /// <param name="repository">The object used to store and retrieve resources.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the fingerprint is invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the orientation image extractor is not assigned.</exception>
        /// <returns>The retrieved orientation image.</returns>
        public OrientationImage GetResource(string fingerprint, ResourceRepository repository)
        {
            bool isPersistent = IsResourcePersistent();
            string resourceName =
                string.Format("{0}.{1}", fingerprint, GetSignature());
            if (isPersistent && repository.ResourceExists(resourceName))
                return OrientationImageSerializer.FromByteArray(repository.RetrieveResource(resourceName));

            OrientationImage resource = Extract(fingerprint, repository);
            if (resource == null)
                return null;

            if (isPersistent)
                repository.StoreResource(resourceName, OrientationImageSerializer.ToByteArray(resource));
            return resource;
        }

        /// <summary>
        ///     Used to extract orientation image in case that the resource have not being saved.
        /// </summary>
        public IFeatureExtractor<OrientationImage> OrientationImageExtractor { set; get; }

        /// <summary>
        ///     Gets the signature of the <see cref="OrientationImageProvider"/>.
        /// </summary>
        /// <returns>It returns a string formed by the name of the property <see cref="OrientationImageExtractor"/> concatenated with ".ori".</returns>
        public string GetSignature()
        {
            return string.Format("{0}.ori", OrientationImageExtractor.GetType().Name);   
        }

        /// <summary>
        ///     Determines whether the provided orientation image is persistent.
        /// </summary>
        /// <returns>Always returns true.</returns>
        public bool IsResourcePersistent()
        {
            return true;
        }

        #region private

        private OrientationImage Extract(string fingerprintLabel, ResourceRepository repository)
        {
            Bitmap image = imageProvider.GetResource(fingerprintLabel, repository);
            if (image == null)
                throw new ArgumentOutOfRangeException("fingerprintLabel", "Unable to extract OrientationImage: Invalid fingerprint!");
            if (OrientationImageExtractor == null)
                throw new InvalidOperationException("Unable to extract OrientationImage: Unassigned orientation image extractor!");
            return OrientationImageExtractor.ExtractFeatures(image);
        }

        private readonly FingerprintImageProvider imageProvider = new FingerprintImageProvider();

        #endregion
    }
}