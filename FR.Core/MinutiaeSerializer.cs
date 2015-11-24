/*
 * Created by: Miguel Angel Medina Pérez (migue.cu@gmail.com)
 * Created: 
 * Comments by: Miguel Angel Medina Pérez (migue.cu@gmail.com)
 */

using System;
using System.Collections.Generic;
using System.IO;

namespace PatternRecognition.FingerprintRecognition.Core
{
    /// <summary>
    ///     Allows saving and retrieving minutiae.
    /// </summary>
    public static class MinutiaListSerializer
    {
        /// <summary>
        ///     Encodes the specified minutia list into a byte array.
        /// </summary>
        /// <remarks>
        ///     This codification uses four bytes per minutia: <see cref="Minutia.X"/> in the left most 11 bits; <see cref="Minutia.Y"/> in the next 11 bits; <see cref="Minutia.Angle"/> in the next eight bits; and <see cref="Minutia.MinutiaType"/> in the last two bits. Therefore, this method is ineffective for values of <see cref="Minutia.X"/> or <see cref="Minutia.Y"/> greater than 2047.
        /// </remarks>
        /// <param name="minutiaList">The minutia list which is going to be encoded to a byte array.</param>
        /// <returns>The byte array containing the encoded minutia list.</returns>
        public static byte[] ToByteArray(List<Minutia> minutiaList)
        {
            var bytes = new byte[minutiaList.Count * 4];
            int k = 0;
            for (int i = 0; i < minutiaList.Count; i++)
            {
                var currArr = MtiaToByteArray(minutiaList[i]);
                for (int j = 0; j < 4; j++)
                    bytes[k++] = currArr[j];
            }
            return bytes;
        }

        /// <summary>
        ///     Decodes a minutia list from a byte array.
        /// </summary>
        /// <remarks>
        ///     This codification uses four bytes per minutia: <see cref="Minutia.X"/> in the left most 11 bits; <see cref="Minutia.Y"/> in the next 11 bits; <see cref="Minutia.Angle"/> in the next eight bits; and <see cref="Minutia.MinutiaType"/> in the last two bits. Therefore, this method is ineffective for values of <see cref="Minutia.X"/> or <see cref="Minutia.Y"/> greater than 2047.
        /// </remarks>
        /// <param name="bytes">The byte array containing the encoded minutia list.</param>
        /// <returns>The minutia list decoded from the specified byte array.</returns>
        public static List<Minutia> FromByteArray(byte[] bytes)
        {
            if (bytes.Length % 4 != 0)
                throw new ArgumentOutOfRangeException("bytes", bytes, "Invalid bytes count: A correct bytes count can be divided by 4.");
            int mtiaCount = bytes.Length / 4;
            List<Minutia> mtiae = new List<Minutia>(mtiaCount);
            for (int i = 0; i < mtiaCount; i++)
            {
                var mtiaBytes = new byte[4];
                for (int k = 0; k < 4; k++)
                    mtiaBytes[k] = bytes[i * 4 + k];

                mtiae.Add(MtiaFromByteArray(mtiaBytes));
            }
            return mtiae;
        }

        /// <summary>
        ///     Save the specified minutia list to the specified file name.
        /// </summary>
        /// <remarks>
        ///     Before saving, the minutia list is encoded using the method <see cref="ToByteArray"/>.
        /// </remarks>
        /// <param name="fileName">The file name where the minutia list will be saved.</param>
        /// <param name="minutiae">The minutia List to be saved.</param>
        public static void Serialize(string fileName, List<Minutia> minutiae)
        {
            var byteArr = ToByteArray(minutiae);
            File.WriteAllBytes(fileName, byteArr);
        }

        /// <summary>
        ///     Load the minutia list saved in the specified file name.
        /// </summary>
        /// <remarks>
        ///     The minutia list are decoded using method <see cref="FromByteArray"/>.
        /// </remarks>
        /// <param name="fileName">The file name where the minutia list is saved.</param>
        /// <returns>The minutia list loaded from file.</returns>
        public static List<Minutia> Deserialize(string fileName)
        {
            var byteArr = File.ReadAllBytes(fileName);
            return FromByteArray(byteArr);
        }

        #region private

        private static Minutia MtiaFromByteArray(byte[] bytes)
        {
            var mtia = new Minutia();
            int info = (bytes[3] << 24) | (bytes[2] << 16) | (bytes[1] << 8) | bytes[0];

            mtia.MinutiaType = (MinutiaType)(3 & info);
            info >>= 2;
            mtia.Angle = 2 * Math.PI * (255 & info) / 255;
            info >>= 8;
            mtia.Y = Convert.ToInt16(info & 2047);
            info >>= 11;
            mtia.X = Convert.ToInt16(info & 2047);

            return mtia;
        }

        private static byte[] MtiaToByteArray(Minutia mtia)
        {
            byte[] bytes = new byte[4];
            // Storing value X in the left most 11 bits.
            int blockX = (2047 & mtia.X) << 21;
            // Storing value Y in the next 11 bits.
            int blockY = (2047 & mtia.Y) << 10;
            // Storing value Angle in the next 8 bits.
            int blockAngle = (Convert.ToByte(Math.Round(mtia.Angle * 255 / (2 * Math.PI)))) << 2;
            // Storing value MinutiaType in the last 2 bits.
            int blockType = (int)mtia.MinutiaType;
            // Merging all data
            int info = blockX | blockY | blockAngle | blockType;

            bytes[0] = Convert.ToByte(255 & info);
            info >>= 8;
            bytes[1] = Convert.ToByte(255 & info);
            info >>= 8;
            bytes[2] = Convert.ToByte(255 & info);
            info >>= 8;
            bytes[3] = Convert.ToByte(255 & info);

            return bytes;
        }

        #endregion
    }
}
