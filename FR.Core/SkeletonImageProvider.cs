/*
 * Created by: Miguel Angel Medina Pérez (migue.cu@gmail.com)
 * Created: June 8, 2010
 * Comments by: Miguel Angel Medina Pérez (migue.cu@gmail.com)
 */

using System;
using System.Drawing;
using PatternRecognition.FingerprintRecognition.Core;

namespace PatternRecognition.FingerprintRecognition.ResourceProviders
{
    /// <summary>
    ///     Allows retrieving skeleton image from a <see cref="ResourceRepository"/>.
    /// </summary>
    public class SkeletonImageProvider : IResourceProvider<SkeletonImage>
    {
        /// <summary>
        ///     Used to extract skeleton image in case that the resource have not being saved.
        /// </summary>
        public IFeatureExtractor<SkeletonImage> SkeletonImageExtractor { set; get; }

        /// <summary>
        ///     Gets skeleton image from the specified fingerprint and <see cref="ResourceRepository"/>.
        /// </summary>
        /// <param name="fingerprint">The fingerprint which skeleton image is being retrieved.</param>
        /// <param name="repository">The object used to store and retrieve resources.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the fingerprint is invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the skeleton image extractor is not assigned.</exception>
        /// <returns>The retrieved skeleton image.</returns>
        object IResourceProvider.GetResource(string fingerprint, ResourceRepository repository)
        {
            return GetResource(fingerprint, repository);
        }

        /// <summary>
        ///     Gets skeleton image from the specified fingerprint and <see cref="ResourceRepository"/>.
        /// </summary>
        /// <param name="fingerprint">The fingerprint which skeleton image are being retrieved.</param>
        /// <param name="repository">The object used to store and retrieve resources.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the fingerprint is invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the skeleton image extractor is not assigned.</exception>
        /// <returns>The retrieved skeleton image.</returns>
        public SkeletonImage GetResource(string fingerprint, ResourceRepository repository)
        {
            bool isPersistent = IsResourcePersistent();
            string resourceName =
                string.Format("{0}.{1}", fingerprint, GetSignature());
            if (isPersistent && repository.ResourceExists(resourceName))
                return SkeletonImageSerializer.FromByteArray(repository.RetrieveResource(resourceName));

            SkeletonImage resource = Extract(fingerprint, repository);
            if (resource == null)
                return null;

            if (isPersistent)
                repository.StoreResource(resourceName, SkeletonImageSerializer.ToByteArray(resource));
            return resource;
        }

        /// <summary>
        ///     Gets the signature of the <see cref="SkeletonImageProvider"/>.
        /// </summary>
        /// <returns>It returns a string formed by the name of the property <see cref="SkeletonImageExtractor"/> concatenated with ".ski".</returns>
        public string GetSignature()
        {
            return string.Format("{0}.ski", SkeletonImageExtractor.GetType().Name);
        }

        /// <summary>
        ///     Determines whether the provided <see cref="SkeletonImage"/> is persistent.
        /// </summary>
        /// <returns>Always returns true.</returns>
        public bool IsResourcePersistent()
        {
            return true;
        }

        #region private

        private SkeletonImage Extract(string fingerprintLabel, ResourceRepository repository)
        {
            Bitmap image = imageProvider.GetResource(fingerprintLabel, repository);
            if (image == null)
                throw new ArgumentOutOfRangeException("fingerprintLabel", "Unable to extract SkeletonImage: Invalid fingerprint!");
            if (SkeletonImageExtractor == null)
                throw new InvalidOperationException("Unable to extract SkeletonImage: Unassigned skeleton image extractor!");
            return SkeletonImageExtractor.ExtractFeatures(image);
        }

        private readonly FingerprintImageProvider imageProvider = new FingerprintImageProvider();

        #endregion
    }
}