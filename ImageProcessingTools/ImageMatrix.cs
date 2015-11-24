/*
 * Created by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 * Created: 
 * Comments by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 */

using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImageProcessingTools
{
    /// <summary>
    ///     A class to represent a gray scale image using a matrix.
    /// </summary>
    public class ImageMatrix
    {
        #region public

        /// <summary>
        ///     Initialize the <see cref="ImageMatrix"/> from the specied image.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The specified image must contain 24 bits per pixel.
        ///     </para>
        ///     <para>
        ///         The new object is built using the image in gray scale.
        ///     </para>
        /// </remarks>
        /// <param name="bmp">The image to convert to gray scale matrix.</param>
        public ImageMatrix(Bitmap bmp)
        {
            BitmapData bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int stride = bmData.Stride;
            System.IntPtr scan0 = bmData.Scan0;
            Height = bmp.Height;
            Width = bmp.Width;
            pixels = new int[bmp.Height, bmp.Width];
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

                        pixels[y, x] = (byte)(.299 * red
                            + .587 * green
                            + .114 * blue);

                        p += 3;
                    }
                    p += nOffset;
                }
            }

            bmp.UnlockBits(bmData);
        }

        /// <summary>
        ///     Initialize a new <see cref="ImageMatrix"/> with the specified width and height.
        /// </summary>
        /// <param name="width">
        ///     The width of the new <see cref="ImageMatrix"/>.
        /// </param>
        /// <param name="height">
        ///     The height of the new <see cref="ImageMatrix"/>.
        /// </param>
        public ImageMatrix(int width, int height)
        {
            Height = height;
            Width = width;
            pixels = new int[height, width];
        }

        /// <summary>
        ///     Gets or sets the gray scale value in the specified pixel.
        /// </summary>
        /// <param name="row">
        ///     The vertical component of the pixel coordinate.
        /// </param>
        /// <param name="column">
        ///     The horizontal component of the pixel coordinate.
        /// </param>
        /// <returns>
        ///     The gray scale value in the specified pixel.
        /// </returns>
        public int this[int row, int column]
        {
            get { return pixels[row, column]; }
            set { pixels[row, column] = value; }
        }

        /// <summary>
        ///     The height of the current <see cref="ImageMatrix"/>.
        /// </summary>
        public int Height { private set; get; }

        /// <summary>
        ///     The width of the current <see cref="ImageMatrix"/>.
        /// </summary>
        public int Width { private set; get; }

        /// <summary>
        ///     Convert the <see cref="ImageMatrix"/> to Bitmap.
        /// </summary>
        /// <remarks>
        ///     The returned image is in gray scale with 24 bits per pixel.
        /// </remarks>
        /// <returns>The computed Bitmap</returns>
        public Bitmap ToBitmap()
        {
            Bitmap bmp = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
            BitmapData bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int stride = bmData.Stride;
            System.IntPtr scan0 = bmData.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)scan0;
                int nOffset = stride - bmp.Width * 3;
                for (int y = 0; y < Height; ++y)
                {
                    for (int x = 0; x < Width; ++x)
                    {
                        p[0] = p[1] = p[2] = (byte)(pixels[y, x] < 0 ? 0 : pixels[y, x] > 255 ? 255 : pixels[y, x]);
                        p += 3;
                    }
                    p += nOffset;
                }
            }

            bmp.UnlockBits(bmData);
            return bmp;
        }

        #endregion

        #region private

        private int[,] pixels;

        #endregion
    }
}
