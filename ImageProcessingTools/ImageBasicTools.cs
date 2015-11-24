/*
 * Created by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 * Created: 
 * Comments by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 */

using System.Drawing;
using System.Drawing.Imaging;

namespace ImageProcessingTools
{
    /// <summary>
    ///     Provides basic functions to process images.
    /// </summary>
    public static class ImageBasicTools
    {
        /// <summary>
        ///     Invert the specified image.
        /// </summary>
        /// <remarks>
        ///     The specified image must contain 24 bits per pixel.
        /// </remarks>
        /// <param name="bmp">The image to invert.</param>
        public static void Invert(Bitmap bmp)
        {
            BitmapData bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int stride = bmData.Stride;
            System.IntPtr scan0 = bmData.Scan0;
            unsafe
            {
                byte* p = (byte*)(void*)scan0;
                int nOffset = stride - bmp.Width * 3;
                int nWidth = bmp.Width * 3;
                for (int y = 0; y < bmp.Height; ++y)
                {
                    for (int x = 0; x < nWidth; ++x)
                    {
                        p[0] = (byte)(255 - p[0]);
                        ++p;
                    }
                    p += nOffset;
                }
            }

            bmp.UnlockBits(bmData);
        }

        /// <summary>
        ///     Convert to gray scale the specified image.
        /// </summary>
        /// <remarks>
        ///     The specified image must contain 24 bits per pixel.
        /// </remarks>
        /// <param name="bmp">The image to convert to gray scale.</param>
        public static void ConvertToGrayScale(Bitmap bmp)
        {
            BitmapData bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int stride = bmData.Stride;
            System.IntPtr scan0 = bmData.Scan0;
            unsafe
            {
                byte* p = (byte*)(void*)scan0;

                int nOffset = stride - bmp.Width * 3;

                byte red, green, blue;

                for (int y = 0; y < bmp.Height; ++y)
                {
                    for (int x = 0; x < bmp.Width; ++x)
                    {
                        blue = p[0];
                        green = p[1];
                        red = p[2];

                        p[0] = p[1] = p[2] = (byte)(.299 * red
                            + .587 * green
                            + .114 * blue);

                        p += 3;
                    }
                    p += nOffset;
                }
            }

            bmp.UnlockBits(bmData);
        }
    }
}
