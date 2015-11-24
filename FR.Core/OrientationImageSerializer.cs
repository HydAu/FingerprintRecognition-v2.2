/*
 * Created by: Miguel Angel Medina Pérez (migue.cu@gmail.com)
 * Created: 
 * Comments by: Miguel Angel Medina Pérez (migue.cu@gmail.com)
 */

using System;
using System.IO;

namespace PatternRecognition.FingerprintRecognition.Core
{
    /// <summary>
    ///     Allows saving and retrieving instances of <see cref="OrientationImage"/>.
    /// </summary>
    public static class OrientationImageSerializer
    {
        /// <summary>
        ///     Encodes the specified <see cref="OrientationImage"/> into a byte array.
        /// </summary>
        /// <remarks>
        ///     This codification stores <see cref="OrientationImage.Width"/> in the first byte; <see cref="OrientationImage.Height"/> in the second byte; <see cref="OrientationImage.Width"/> in the third byte; and uses one byte for each orientation value. Therefore, this method is ineffective for values of <see cref="OrientationImage.WindowSize"/>,  <see cref="OrientationImage.Height"/> and <see cref="OrientationImage.Width"/> greater than 255.
        /// </remarks>
        /// <param name="orImg">The <see cref="OrientationImage"/> which is going to be encoded to a byte array.</param>
        /// <returns>The byte array containing the encoded <see cref="OrientationImage"/>.</returns>
        public static byte[] ToByteArray(OrientationImage orImg)
        {
            byte[] bytes = new byte[orImg.Width * orImg.Height + 3];
            bytes[0] = orImg.WindowSize;
            bytes[1] = orImg.Height;
            bytes[2] = orImg.Width;
            int k = 3;
            for (int i = 0; i < orImg.Height; i++)
                for (int j = 0; j < orImg.Width; j++)
                    if (orImg.IsNullBlock(i, j))
                        bytes[k++] = 255;
                    else
                        bytes[k++] = Convert.ToByte(Math.Round(orImg.AngleInRadians(i, j) * 180 / Math.PI));
            return bytes;
        }

        /// <summary>
        ///     Decodes an <see cref="OrientationImage"/> from a byte array.
        /// </summary>
        /// <remarks>
        ///     This codification stores <see cref="OrientationImage.Width"/> in the first byte; <see cref="OrientationImage.Height"/> in the second byte; <see cref="OrientationImage.Width"/> in the third byte; and uses one byte for each orientation value. Therefore, this method is ineffective for values of <see cref="OrientationImage.WindowSize"/>,  <see cref="OrientationImage.Height"/> and <see cref="OrientationImage.Width"/> greater than 255.
        /// </remarks>
        /// <param name="bytes">The byte array containing the encoded <see cref="OrientationImage"/>.</param>
        /// <returns>The <see cref="OrientationImage"/> decoded from the specified byte array.</returns>
        public static OrientationImage FromByteArray(byte[] bytes)
        {
            byte height = bytes[1];
            byte width = bytes[2];
            byte[,] orientations = new byte[height, width];
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    orientations[i, j] = Convert.ToByte(bytes[i * width + j + 3]);

            OrientationImage orImg = new OrientationImage(width, height, orientations, bytes[0]);
            return orImg;
        }

        /// <summary>
        ///     Save the specified <see cref="OrientationImage"/> to the specified file name.
        /// </summary>
        /// <remarks>
        ///     Before saving, the <see cref="OrientationImage"/> is encoded using the method <see cref="ToByteArray"/>.
        /// </remarks>
        /// <param name="fileName">The file name where the specified <see cref="OrientationImage"/> will be saved.</param>
        /// <param name="orImg">The <see cref="OrientationImage"/> to be saved.</param>
        public static void Serialize(string fileName, OrientationImage orImg)
        {
            var byteArr = ToByteArray(orImg);
            File.WriteAllBytes(fileName, byteArr);
        }

        /// <summary>
        ///     Load the <see cref="OrientationImage"/> saved in the specified file name.
        /// </summary>
        /// <remarks>
        ///     The <see cref="OrientationImage"/> are decoded using method <see cref="FromByteArray"/>.
        /// </remarks>
        /// <param name="fileName">The file name where the<see cref="OrientationImage"/> is saved.</param>
        /// <returns>The <see cref="OrientationImage"/> loaded from file.</returns>
        public static OrientationImage Deserialize(string fileName)
        {
            var byteArr = File.ReadAllBytes(fileName);
            return FromByteArray(byteArr);
        }
    }
}
