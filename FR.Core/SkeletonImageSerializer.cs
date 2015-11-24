/*
 * Created by: Miguel Angel Medina Pérez (migue.cu@gmail.com)
 * Created: June 8, 2010
 * Comments by: Miguel Angel Medina Pérez (migue.cu@gmail.com)
 */

using System;
using System.IO;

namespace PatternRecognition.FingerprintRecognition.Core
{
    /// <summary>
    ///     Allows saving and retrieving instances of <see cref="SkeletonImage"/>.
    /// </summary>
    public static class SkeletonImageSerializer
    {
        /// <summary>
        ///     Save the specified <see cref="SkeletonImage"/> to the specified file name.
        /// </summary>
        /// <remarks>
        ///     Before saving, the <see cref="SkeletonImage"/> is encoded using the method <see cref="ToByteArray"/>.
        /// </remarks>
        /// <param name="fileName">The file name where the specified <see cref="SkeletonImage"/> will be saved.</param>
        /// <param name="skImg">The <see cref="SkeletonImage"/> to be saved.</param>
        public static void Serialize(string fileName, SkeletonImage skImg)
        {
            var byteArr = ToByteArray(skImg);
            File.WriteAllBytes(fileName, byteArr);
        }

        /// <summary>
        ///     Load the <see cref="SkeletonImage"/> saved in the specified file name.
        /// </summary>
        /// <remarks>
        ///     The <see cref="SkeletonImage"/> are decoded using method <see cref="FromByteArray"/>.
        /// </remarks>
        /// <param name="fileName">The file name where the<see cref="SkeletonImage"/> is saved.</param>
        /// <returns>The <see cref="SkeletonImage"/> loaded from file.</returns>
        public static SkeletonImage Deserialize(string fileName)
        {
            var byteArr = File.ReadAllBytes(fileName);
            return FromByteArray(byteArr);
        }

        /// <summary>
        ///     Decodes a <see cref="SkeletonImage"/> object from a byte array.
        /// </summary>
        /// <remarks>
        ///     This codification uses two bytes to store <see cref="SkeletonImage.Width"/>; two more bytes to store <see cref="SkeletonImage.Height"/>; and one bit for each pixel of the <see cref="SkeletonImage"/>. Therefore, this method is ineffective for values of <see cref="SkeletonImage.Width"/> and <see cref="SkeletonImage.Height"/> greater than 65535.
        /// </remarks>
        /// <param name="bytes">The byte array containing the encoded <see cref="SkeletonImage"/> object.</param>
        /// <returns>The <see cref="SkeletonImage"/> object decoded from the specified byte array.</returns>
        public static SkeletonImage FromByteArray(byte[] bytes)
        {
            int width = bytes[0];
            width |= bytes[1] << 8;
            int height = bytes[2];
            height |= bytes[3] << 8;

            int counter = 0;
            int cursor = 4;
            byte[,] imageData = new byte[height, width];
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                {
                    int value = 1 & (bytes[cursor] >> counter);
                    imageData[i, j] = (byte)(value == 1 ? 255 : 0);
                    if (counter == 7)
                    {
                        counter = 0;
                        cursor++;
                    }
                    else
                        counter++;
                }

            return new SkeletonImage(imageData, width, height);
        }

        /// <summary>
        ///     Encodes the specified <see cref="SkeletonImage"/> into a byte array.
        /// </summary>
        /// <remarks>
        ///     This codification uses two bytes to store <see cref="SkeletonImage.Width"/>; two more bytes to store <see cref="SkeletonImage.Height"/>; and one bit for each pixel of the <see cref="SkeletonImage"/>. Therefore, this method is ineffective for values of <see cref="SkeletonImage.Width"/> and <see cref="SkeletonImage.Height"/> greater than 65535.
        /// </remarks>
        /// <param name="skImg">The <see cref="SkeletonImage"/> object which is going to be encoded to a byte array.</param>
        /// <returns>The byte array containing the encoded <see cref="SkeletonImage"/> object.</returns>
        public static byte[] ToByteArray(SkeletonImage skImg)
        {
            int length = (int)Math.Ceiling(skImg.Width * skImg.Height / 8.0);
            byte[] raw = new byte[length + 4];

            raw[0] = (byte)(255 & skImg.Width);
            raw[1] = (byte)(255 & (skImg.Width >> 8));
            raw[2] = (byte)(255 & skImg.Height);
            raw[3] = (byte)(255 & (skImg.Height >> 8));

            int counter = 0;
            int cursor = 4;
            int currValue = 0;
            for (int i = 0; i < skImg.Height; i++)
                for (int j = 0; j < skImg.Width; j++)
                {
                    currValue |= (skImg[i, j] == 255 ? 1 : 0) << counter;
                    if (counter == 7)
                    {
                        raw[cursor++] = (byte)currValue;
                        currValue = 0;
                        counter = 0;
                    }
                    else
                        counter++;
                }

            return raw;
        }
    }
}
