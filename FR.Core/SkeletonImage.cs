/*
 * Created by: Andrés Eduardo Gutiérrez Rodríguez (andres@bioplantas.cu), 
 *             Milton García Borroto (milton.garcia@gmail.com) and
 *             Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)             
 * Created: 
 * Comments by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace PatternRecognition.FingerprintRecognition.Core
{
    /// <summary>
    ///     Represents a skeleton image.
    /// </summary>
    [Serializable]
    public class SkeletonImage
    {
        #region public

        /// <summary>
        ///     Initialize an <see cref="SkeletonImage"/> from the specified byte array, with the specified width and height.
        /// </summary>
        /// <param name="img">
        ///     A byte array containing the image pixel data.
        /// </param>
        /// <param name="width">
        ///     The width of the skeleton image.
        /// </param>
        /// <param name="height">
        ///     The height of the skeleton image.
        /// </param>
        public SkeletonImage(byte[] img, int width, int height)
        {
            Width = width;
            Height = height;
            image = new byte[Height, Width];
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++)
                    image[i, j] = img[Width * i + j];
        }

        /// <summary>
        ///     Initialize an <see cref="SkeletonImage"/> from the specified byte matrix, with the specified width and height.
        /// </summary>
        /// <param name="img">
        ///     A byte matrix containing the image pixel data.
        /// </param>
        /// <param name="width">
        ///     The width of the skeleton image.
        /// </param>
        /// <param name="height">
        ///     The height of the skeleton image.
        /// </param>
        public SkeletonImage(byte[,] img, int width, int height)
        {
            Width = width;
            Height = height;
            image = img;
        }

        /// <summary>
        ///     Converts the current <see cref="SkeletonImage"/> object to <see cref="Bitmap"/>.
        /// </summary>
        /// <returns>
        ///     The computed <see cref="Bitmap"/>.
        /// </returns>
        public unsafe Bitmap ConvertToBitmap()
        {
            Bitmap bmp = new Bitmap(Width, Height, PixelFormat.Format8bppIndexed);
            ColorPalette cp = bmp.Palette;
            for (int i = 1; i < 256; i++)
                cp.Entries[i] = Color.FromArgb(i, i, i);
            cp.Entries[0] = Color.Black;
            bmp.Palette = cp;
            BitmapData bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                                             ImageLockMode.ReadWrite, bmp.PixelFormat);

            byte* p = (byte*)(void*)bmData.Scan0;
            int b = 0;
            int nOffset = bmData.Stride - bmp.Width;
            for (int y = 0; y < bmp.Height; ++y)
            {
                Marshal.Copy(ConvertMatrixToArray(), b, (IntPtr)p, bmp.Width);
                p += bmp.Width + nOffset;
                b += bmp.Width;
            }
            bmp.UnlockBits(bmData);
            return bmp;
        }

        /// <summary>
        ///     Determines the ridge count between the specified points.
        /// </summary>
        /// <param name="x0">The x component of the first point.</param>
        /// <param name="y0">The y component of the first point.</param>
        /// <param name="x1">The x component of the second point.</param>
        /// <param name="y1">The y component of the second point.</param>
        /// <returns>The ridge count between the specified points.</returns>
        public byte RidgeCount(int x0, int y0, int x1, int y1)
        {
            byte count = 0;
            var points = Bresenham(x0, y0, x1, y1);
            int i = 0;
            while (i < points.Count)
            {
                int j = i;
                if (i == 0)
                {
                    while ((i < points.Count) && (PixelEnviroment(points[i]) == 0))
                        i++;
                    j = i;
                    if (i >= points.Count)
                        break;
                }
                while ((j < points.Count) && (PixelEnviroment(points[j]) == 255))
                    j++;
                i = j;
                if (i >= points.Count)
                    break;
                while ((i < points.Count) && (PixelEnviroment(points[i]) == 0))
                    i++;
                if (i >= points.Count)
                    break;
                count++;
            }
            return count;
        }

        /// <summary>
        ///     Gets the gray intensity at the specified pixel coordinates.
        /// </summary>
        /// <param name="row">The row of the specified pixel.</param>
        /// <param name="col">The column of the specified pixel.</param>
        /// <returns>
        ///     The gray intensity at the specified pixel coordinates.
        /// </returns>
        public byte this[int row, int col]
        {
            get { return image[row, col]; }
        }

        /// <summary>
        ///     Gets or sets the width of the skeleton image.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        ///     Gets or sets the height of the skeleton image.
        /// </summary>
        public int Height { get; private set; }

        #endregion

        #region private

        private byte[,] image;

        private byte[] ConvertMatrixToArray()
        {
            byte[] img = new byte[Width * Height];
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++)
                    img[Width * i + j] = image[i, j];
            return img;
        }

        private byte PixelEnviroment(Point p)
        {
            if (image[p.Y - 1, p.X - 1] == 0) return 0;
            if (image[p.Y - 1, p.X] == 0) return 0;
            if (image[p.Y - 1, p.X + 1] == 0) return 0;
            if (image[p.Y, p.X - 1] == 0) return 0;
            if (image[p.Y, p.X] == 0) return 0;
            if (image[p.Y, p.X + 1] == 0) return 0;
            if (image[p.Y + 1, p.X - 1] == 0) return 0;
            if (image[p.Y + 1, p.X] == 0) return 0;
            if (image[p.Y + 1, p.X + 1] == 0) return 0;

            return 255;
        }

        private List<Point> Bresenham(int x0, int y0, int x1, int y1)
        {
            List<Point> pixels = new List<Point>();
            int x, y, dx, dy, p, incE, incNE, stepx, stepy;
            dx = (x1 - x0);
            dy = (y1 - y0);
            if (dy < 0)
            {
                dy = -dy; stepy = -1;
            }
            else
                stepy = 1;
            if (dx < 0)
            {
                dx = -dx; stepx = -1;
            }
            else
                stepx = 1;
            x = x0;
            y = y0;
            pixels.Add(new Point(x, y));
            if (dx > dy)
            {
                p = 2 * dy - dx;
                incE = 2 * dy;
                incNE = 2 * (dy - dx);
                while (x != x1)
                {
                    x = x + stepx;
                    if (p < 0)
                    {
                        p = p + incE;
                    }
                    else
                    {
                        y = y + stepy;
                        p = p + incNE;
                    }
                    pixels.Add(new Point(x, y));
                }
            }
            else
            {
                p = 2 * dx - dy;
                incE = 2 * dx;
                incNE = 2 * (dx - dy);
                while (y != y1)
                {
                    y = y + stepy;
                    if (p < 0)
                    {
                        p = p + incE;
                    }
                    else
                    {
                        x = x + stepx;
                        p = p + incNE;
                    }
                    pixels.Add(new Point(x, y));
                }
            }
            return pixels;
        }

        #endregion
    }
}