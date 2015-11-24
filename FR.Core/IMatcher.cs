/*
 * Created by: Miguel Angel Medina Pérez (migue.cu@gmail.com)
 * Created: 1/5/2007
 * Comments by: Miguel Angel Medina Pérez (migue.cu@gmail.com)
 */

using System;
using System.Collections.Generic;

namespace PatternRecognition.FingerprintRecognition.Core
{
    /// <summary>
    ///     Represents a non-generic fingerprint matching algorithm.
    /// </summary>
    /// <remarks>
    ///     A fingerprint matching algorithm compares fingerprint features and returns a matching score. The higher returned value, the greater is the fingerprints similarity.
    /// </remarks>
    public interface IMatcher
    {
        /// <summary>
        ///     Matches the specified fingerprint features.
        /// </summary>
        /// <param name="query">
        ///     The query fingerprint features.
        /// </param>
        /// <param name="template">
        ///     The template fingerprint features.
        /// </param>
        /// <returns>
        ///     The fingerprint similarity value.
        /// </returns>
        double Match(object query, object template);
    }

    /// <summary>
    ///     Represents a minutia matching algorithm. 
    /// </summary>
    /// <remarks>
    ///     A minutia matching algorithm compares fingerprints based on minutia features. 
    /// </remarks>
    public interface IMinutiaMatcher : IMatcher
    {
        /// <summary>
        ///     Matches the specified fingerprint features and returns the matching minutiae.
        /// </summary>
        /// <param name="query">
        ///     The query fingerprint features.
        /// </param>
        /// <param name="template">
        ///     The template fingerprint features.
        /// </param>
        /// <param name="matchingMtiae">
        ///     The matching minutiae..
        /// </param>
        /// <returns>
        ///     The fingerprint similarity value.
        /// </returns>
        double Match(object query, object template, out List<MinutiaPair> matchingMtiae);
    }

    /// <summary>
    ///     Represents a fingerprint matching algorithm.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A fingerprint matching algorithm compares fingerprint features and returns a matching score. The higher returned value, the greater is the fingerprints similarity.
    ///     </para>
    /// </remarks>
    /// <typeparam name="FeatureType">
    ///     The type of the features that can be matched.
    /// </typeparam>
    public interface IMatcher<FeatureType> : IMatcher
    {
        /// <summary>
        ///     Matches the specified fingerprint features.
        /// </summary>
        /// <param name="query">
        ///     The query fingerprint features.
        /// </param>
        /// <param name="template">
        ///     The template fingerprint features.
        /// </param>
        /// <returns>
        ///     The fingerprint similarity value.
        /// </returns>
        double Match(FeatureType query, FeatureType template);
    }

    /// <summary>
    ///     Provides a base class for implementations of the <see cref="IMatcher&lt;FeatureType&gt;"/> generic interface.
    /// </summary>
    /// <typeparam name="FeatureType">
    ///     The type of the features that can be matched.
    /// </typeparam>
    public abstract class Matcher<FeatureType> : IMatcher<FeatureType>
    {
        #region IMatcher<FeatureType> Members

        /// <summary>
        ///     When implemented in a derived class, matches the specified fingerprint features.
        /// </summary>
        /// <param name="query">
        ///     The query fingerprint features.
        /// </param>
        /// <param name="template">
        ///     The template fingerprint features.
        /// </param>
        /// <returns>
        ///     The fingerprint similarity value.
        /// </returns>
        public abstract double Match(FeatureType query, FeatureType template);

        #endregion

        #region IMatcher Members

        /// <summary>
        ///     Matches the specified fingerprint features.
        /// </summary>
        /// <param name="query">
        ///     The query fingerprint features.
        /// </param>
        /// <param name="template">
        ///     The template fingerprint features.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the specified features has invalid type.</exception>
        /// <returns>
        ///     The fingerprint matching value.
        /// </returns>
        public double Match(object query, object template)
        {
            //try
            {
                return Match((FeatureType)query, (FeatureType)template);
            }
            //catch (Exception e)
            //{
            //    if (query.GetType() != typeof(FeatureType) || template.GetType() != typeof(FeatureType))
            //    {
            //        string msg = "Unable to match fingerprints: Invalid features type!";
            //        throw new ArgumentOutOfRangeException(msg, e);
            //    }
            //    throw e;
            //}
        }

        #endregion
    }



}