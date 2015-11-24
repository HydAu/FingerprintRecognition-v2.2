/*
 * Created by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 *             Andrés Eduardo Gutiérrez Rodríguez (andres@bioplantas.cu)
 *             
 * Created: 
 * Comments by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using PatternRecognition.FingerprintRecognition.Core;
using PatternRecognition.FingerprintRecognition.FeatureRepresentation;
using PatternRecognition.FingerprintRecognition.Matchers;

namespace PatternRecognition.FingerprintRecognition.FeatureExtractors
{
    /// <summary>
    ///     A class to extract the features used by <see cref="TK"/> and <see cref="MTK"/> to match fingerprints.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         In order to extract features from an image, the properties <see cref="MtiaExtractor"/> and <see cref="OrImgExtractor"/> must be assigned.
    ///     </para>
    ///     <para>
    ///         The features can also be extracted specifying a minutia list and an orientation image.
    ///     </para>
    /// </remarks>
    public class Tico2003FeatureExtractor : FeatureExtractor<Tico2003Features>
    {
        /// <summary>
        ///     The minutia list extractor used to compute <see cref="Tico2003Features"/> in the method <see cref="ExtractFeatures(Bitmap)"/>.
        /// </summary>
        public IFeatureExtractor<List<Minutia>> MtiaExtractor { set; get; }

        /// <summary>
        ///     The orientation image extractor used to compute <see cref="Tico2003Features"/> in the method <see cref="ExtractFeatures(Bitmap)"/>.
        /// </summary>
        public IFeatureExtractor<OrientationImage> OrImgExtractor { set; get; }

        /// <summary>
        ///     Extract features of type <see cref="Tico2003Features"/> from the specified image.
        /// </summary>
        /// <remarks>
        ///     This method uses the properties <see cref="MtiaExtractor"/> and <see cref="OrImgExtractor"/> to extract features, so it raises an exception if any of these properties is not assigned.
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        ///      Thrown when the minutia list extractor is not assigned or the orientation image extractor is not assigned.
        /// </exception>
        /// <param name="image">The source image to extract features from.</param>
        /// <returns>
        ///     Features of type <see cref="Tico2003Features"/> extracted from the specified image.
        /// </returns>
        public override Tico2003Features ExtractFeatures(Bitmap image)
        {
            try
            {
                var mtiae = MtiaExtractor.ExtractFeatures(image);
                var dImg = OrImgExtractor.ExtractFeatures(image);

                return new Tico2003Features(mtiae, dImg);
            }
            catch (Exception e)
            {
                if (MtiaExtractor == null)
                    throw new InvalidOperationException("Can not extract Tico2003Features: Unassigned minutia list extractor!", e);
                if (OrImgExtractor == null)
                    throw new InvalidOperationException("Can not extract Tico2003Features: Unassigned orientation image extractor!", e);
                throw;
            }
        }

        /// <summary>
        ///     Extract features of type <see cref="Tico2003Features"/> from the specified minutia list and orientation image.
        /// </summary>
        /// <param name="mtiae">
        ///     The minutia list to extract the features from.
        /// </param>
        /// <param name="orImg">
        ///     The skeleton image to extract the features from.
        /// </param>
        /// <returns>
        ///     Features of type <see cref="Tico2003Features"/> extracted from the specified minutiae and skeleton image.
        /// </returns>
        public Tico2003Features ExtractFeatures(List<Minutia> mtiae, OrientationImage orImg)
        {
            return new Tico2003Features(mtiae, orImg);
        }
    }
}