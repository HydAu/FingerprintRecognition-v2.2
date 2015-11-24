/*
 * Created by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 * Created: 
 * Comments by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 */

using System;
using System.Collections.Generic;
using System.IO;

namespace PatternRecognition.FingerprintRecognition.Core
{
    /// <summary>
    ///     Allows loading minutiae from files with ISO/IEC 19794-2:2005 format.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This class assumes that there is only one fingerprint for each template file.
    ///     </para>
    /// </remarks>
    public static class ISOMinutiaeLoader
    {
        /// <summary>
        ///     Load minutiae from the file in the specified location.
        /// </summary>
        /// <param name="fileName">
        ///     The location of the file with the minutiae.
        /// </param>
        /// <returns>
        ///     The minutia list loaded from the specified file.
        /// </returns>
        public static List<Minutia> Load(string fileName)
        {
            var data = File.ReadAllBytes(fileName);
            return FromByteArray(data);
        }

        public static List<Minutia> FromByteArray(byte[] data)
        {
            int cursor = 27;
            // Reading Number of Minutiae
            byte mtiaeCount = data[cursor++];
            List<Minutia> list = new List<Minutia>(mtiaeCount);
            // Reading minutiae
            for (int i = 0; i < mtiaeCount; i++, cursor += 6)
            {
                var mtiaTypeCode = (data[cursor] >> 6) & 3;
                var mtiaType = mtiaTypeCode == 0
                                   ? MinutiaType.Unknown
                                   : mtiaTypeCode == 1 ? MinutiaType.End : MinutiaType.Bifurcation;

                var x = ((data[cursor] & 63) << 8) | (data[cursor + 1]);
                var y = ((data[cursor + 2] & 63) << 8) | (data[cursor + 3]);
                var angle = Convert.ToDouble(2 * Math.PI - data[cursor + 4] * 2.0 * Math.PI / 255.0);

                var mtia = new Minutia
                {
                    MinutiaType = mtiaType,
                    X = (short)x,
                    Y = (short)y,
                    Angle = angle
                };
                list.Add(mtia);
            }

            return list;
        }
    }
}