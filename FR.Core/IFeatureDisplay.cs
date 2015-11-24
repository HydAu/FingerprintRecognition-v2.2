/*
 * Created by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 * Created: Thursday, December 20, 2007
 * Comments by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 */

using System;
using System.Drawing;

namespace PatternRecognition.FingerprintRecognition.Core
{
    /// <summary>
    ///     Represents a non-generic object that can paint fingerprint features. 
    /// </summary>
    public interface IFeatureDisplay
    {
        /// <summary>
        ///     Paints the specified features using the specified <see cref="Graphics"/>.
        /// </summary>
        /// <param name="features">The features to be painted.</param>
        /// <param name="g">The <see cref="Graphics"/> object used to paint the features.</param>
        void Show(object features, Graphics g);
    }

    /// <summary>
    ///     Represents an object that can paint fingerprint features.
    /// </summary>
    /// <typeparam name="FeatureType">
    ///     The type of the features that the object can paint.
    /// </typeparam>
    public interface IFeatureDisplay<FeatureType> : IFeatureDisplay
    {
        /// <summary>
        ///     Paints the specified features using the specified <see cref="Graphics"/>.
        /// </summary>
        /// <param name="features">The features to be painted.</param>
        /// <param name="g">The <see cref="Graphics"/> object used to paint the features.</param>
        void Show(FeatureType features, Graphics g);
    }

    /// <summary>
    ///     Provides a base class for implementations of the <see cref="IFeatureDisplay&lt;FeatureType&gt;"/> generic interface.
    /// </summary>
    /// <typeparam name="FeatureType">
    ///     The type of the feature that the object can paint.
    /// </typeparam>
    public abstract class FeatureDisplay<FeatureType> : IFeatureDisplay<FeatureType>
    {
        #region IFeatureDisplay<FeatureType> Members

        /// <summary>
        ///     When implemented in a derived class, paints the specified features using the specified <see cref="Graphics"/>.
        /// </summary>
        /// <param name="features">The features to be painted.</param>
        /// <param name="g">The <see cref="Graphics"/> object used to paint the features.</param>
        public abstract void Show(FeatureType features, Graphics g);

        #endregion

        #region IFeatureDisplay Members

        /// <summary>
        ///     Paints the specified features using the specified <see cref="Graphics"/>.
        /// </summary>
        /// <param name="features">The features to be painted.</param>
        /// <param name="g">The <see cref="Graphics"/> object used to paint the features.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown when the type of the specified features is not correct.
        /// </exception>
        public void Show(object features, Graphics g)
        {
            if (features.GetType() != typeof(FeatureType))
            {
                string msg = "Unable to display features: Invalid features type!";
                throw new ArgumentOutOfRangeException("features", features, msg);
            }
            Show((FeatureType)features, g);
        }

        #endregion
    }
}