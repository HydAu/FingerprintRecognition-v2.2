/*
 * Created by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
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
    ///     A class to extract the features used by <see cref="PN"/> to match fingerprints.
    /// </summary>
    /// <remarks>
    ///     This class can extract features from a minutia list or from an image. In order to extract features from an image, the property <see cref="MtiaExtractor"/> must be assigned.
    /// </remarks>
    public class PNFeatureExtractor : FeatureExtractor<PNFeatures>
    {
        /// <summary>
        ///     The minutia list extractor used to compute <see cref="PNFeatures"/> in the method <see cref="ExtractFeatures(Bitmap)"/>.
        /// </summary>
        public IFeatureExtractor<List<Minutia>> MtiaExtractor { set; get; }

        /// <summary>
        ///     Extract features of type <see cref="PNFeatures"/> from the specified image.
        /// </summary>
        /// <remarks>
        ///     This method uses the property <see cref="MtiaExtractor"/> to extract features, so it raises an exception if the property is not assigned.
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        ///     Thrown when the minutia list extractor is not assigned
        /// </exception>
        /// <param name="image">The source image to extract features from.</param>
        /// <returns>
        ///     Features of type <see cref="PNFeatures"/> extracted from the specified image.
        /// </returns>
        public override PNFeatures ExtractFeatures(Bitmap image)
        {
            try
            {
                List<Minutia> minutiae = MtiaExtractor.ExtractFeatures(image);
                return ExtractFeatures(minutiae);
            }
            catch (Exception e)
            {
                if (MtiaExtractor == null)
                    throw new InvalidOperationException("Unable to extract PNFeatures: Unassigned minutia list extractor!", e);
                throw;
            }
        }

        /// <summary>
        ///     Extract features of type <see cref="PNFeatures"/> from the specified minutiae.
        /// </summary>
        /// <param name="minutiae">
        ///     The list of <see cref="Minutia"/> to extract the features from.
        /// </param>
        /// <returns>
        ///     Features of type <see cref="PNFeatures"/> extracted from the specified minutiae.
        /// </returns>
        public PNFeatures ExtractFeatures(List<Minutia> minutiae)
        {
            List<MtiaTriplet> result = new List<MtiaTriplet>();
            if (minutiae.Count > 3)
                foreach (var triangle in Delaunay2D.Triangulate(minutiae))
                {
                    var idxArr = new short[]
                                     {
                                         (short) triangle.A,
                                         (short) triangle.B,
                                         (short) triangle.C
                                     };
                    MtiaTriplet newMTriplet = new MtiaTriplet(idxArr, minutiae);
                    result.Add(newMTriplet);

                    idxArr = new short[]
                                 {
                                     (short) triangle.A,
                                     (short) triangle.C,
                                     (short) triangle.B
                                 };
                    newMTriplet = new MtiaTriplet(idxArr, minutiae);
                    result.Add(newMTriplet);

                    idxArr = new short[]
                                 {
                                     (short) triangle.B,
                                     (short) triangle.A,
                                     (short) triangle.C
                                 };
                    newMTriplet = new MtiaTriplet(idxArr, minutiae);
                    result.Add(newMTriplet);

                    idxArr = new short[]
                                 {
                                     (short) triangle.B,
                                     (short) triangle.C,
                                     (short) triangle.A
                                 };
                    newMTriplet = new MtiaTriplet(idxArr, minutiae);
                    result.Add(newMTriplet);

                    idxArr = new short[]
                                 {
                                     (short) triangle.C,
                                     (short) triangle.A,
                                     (short) triangle.B
                                 };
                    newMTriplet = new MtiaTriplet(idxArr, minutiae);
                    result.Add(newMTriplet);

                    idxArr = new short[]
                                 {
                                     (short) triangle.C,
                                     (short) triangle.B,
                                     (short) triangle.A
                                 };
                    newMTriplet = new MtiaTriplet(idxArr, minutiae);
                    result.Add(newMTriplet);
                }
            result.TrimExcess();
            return new PNFeatures(result, minutiae);
        }

        public PNFeatures ExtractFeatures1(List<Minutia> minutiae)
        {
            List<MtiaTriplet> result = new List<MtiaTriplet>();
            if (minutiae.Count > 3)
            {
                List<int[]> triplets;
                SHullDelaunay.Triangulate(minutiae, out triplets);
                foreach (var triangle in triplets)
                {
                    var idxArr = new short[]
                                     {
                                         (short) triangle[0],
                                         (short) triangle[1],
                                         (short) triangle[2]
                                     };
                    MtiaTriplet newMTriplet = new MtiaTriplet(idxArr, minutiae);
                    result.Add(newMTriplet);

                    idxArr = new short[]
                                 {
                                     (short) triangle[0],
                                     (short) triangle[1],
                                     (short) triangle[2]
                                 };
                    newMTriplet = new MtiaTriplet(idxArr, minutiae);
                    result.Add(newMTriplet);

                    idxArr = new short[]
                                 {
                                     (short) triangle[0],
                                     (short) triangle[1],
                                     (short) triangle[2]
                                 };
                    newMTriplet = new MtiaTriplet(idxArr, minutiae);
                    result.Add(newMTriplet);

                    idxArr = new short[]
                                 {
                                     (short) triangle[0],
                                     (short) triangle[1],
                                     (short) triangle[2]
                                 };
                    newMTriplet = new MtiaTriplet(idxArr, minutiae);
                    result.Add(newMTriplet);

                    idxArr = new short[]
                                 {
                                     (short) triangle[0],
                                     (short) triangle[1],
                                     (short) triangle[2]
                                 };
                    newMTriplet = new MtiaTriplet(idxArr, minutiae);
                    result.Add(newMTriplet);

                    idxArr = new short[]
                                 {
                                     (short) triangle[0],
                                     (short) triangle[1],
                                     (short) triangle[2]
                                 };
                    newMTriplet = new MtiaTriplet(idxArr, minutiae);
                    result.Add(newMTriplet);
                }
            }

            result.TrimExcess();
            return new PNFeatures(result, minutiae);
        }
    }


}