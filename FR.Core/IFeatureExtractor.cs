/*
 * Created by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 * Created: Thursday, December 20, 2007
 * Comments by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 */

using System.Drawing;

namespace PatternRecognition.FingerprintRecognition.Core
{
    /// <summary>
    ///     Represents a non-generic algorithm for features extraction. 
    /// </summary>
    public interface IFeatureExtractor
    {
        /// <summary>
        ///     Extract features from the specified image.
        /// </summary>
        /// <param name="image">The source image to extract features from.</param>
        /// <returns>The features extracted from the specified image.</returns>
        object ExtractFeatures(Bitmap image);
    }

    /// <summary>
    ///     Represents an algorithm for features extraction. 
    /// </summary>
    /// <typeparam name="FeatureType">
    ///     The type of the features to be extracted.
    /// </typeparam>
    public interface IFeatureExtractor<FeatureType> : IFeatureExtractor
    {
        /// <summary>
        ///     Extract features from the specified image.
        /// </summary>
        /// <param name="image">The source image to extract features from.</param>
        /// <returns>The features extracted from the specified image.</returns>
        new FeatureType ExtractFeatures(Bitmap image);
    }

    /// <summary>
    ///     Provides a base class for implementations of the <see cref="IFeatureExtractor&lt;FeatureType&gt;"/> generic interface.
    /// </summary>
    /// <typeparam name="FeatureType">
    ///     The type of the features to be extracted.
    /// </typeparam>
    public abstract class FeatureExtractor<FeatureType> : IFeatureExtractor<FeatureType>
    {
        #region IFeatureExtractor<FeatureType> Members

        /// <summary>
        ///     When implemented in a derived class, extract features from the specified image.
        /// </summary>
        /// <param name="image">The source image to extract features from.</param>
        /// <returns>The features extracted from the specified image.</returns>
        public abstract FeatureType ExtractFeatures(Bitmap image);

        #endregion

        #region IFeatureExtractor Members

        /// <summary>
        ///     Extract features from the specified image.
        /// </summary>
        /// <param name="image">The source image to extract features from.</param>
        /// <returns>The features extracted from the specified image.</returns>
        object IFeatureExtractor.ExtractFeatures(Bitmap image)
        {
            return ExtractFeatures(image);
        }

        #endregion
    }
}