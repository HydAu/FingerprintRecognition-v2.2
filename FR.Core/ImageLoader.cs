/*
 * Created by: Milton García Borroto (milton.garcia@gmail.com)
 * Created: 
 * Comments by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 */

using System.Drawing;
using System.Drawing.Imaging;

namespace PatternRecognition.FingerprintRecognition.Core
{
    /// <summary>
    ///     Provides a method to load the image in the specified location and returns a copy with 24 bits per pixel.
    /// </summary>
    public static class ImageLoader
    {
        /// <summary>
        ///     Load the image from the specified location and returns a copy with 24 bits per pixel.
        /// </summary>
        /// <param name="fileName">
        ///     The location of the image to load.
        /// </param>
        /// <returns>
        ///     A copy of the loaded image with 24 bits per pixel.
        /// </returns>
        public static Bitmap LoadImage(string fileName)
        {
            Bitmap returnBitmap;
            Bitmap srcBitmap = new Bitmap(fileName);
            using (srcBitmap)
            {
                PixelFormat pixelFormat;
                switch (srcBitmap.PixelFormat)
                {
                    case PixelFormat.Format8bppIndexed:
                    case PixelFormat.Indexed:
                    case PixelFormat.Format4bppIndexed:
                    case PixelFormat.Format1bppIndexed:
                        pixelFormat = PixelFormat.Format24bppRgb;
                        break;
                    default:
                        pixelFormat = srcBitmap.PixelFormat;
                        break;
                }
                returnBitmap = new Bitmap(srcBitmap.Width, srcBitmap.Height, pixelFormat);
                returnBitmap.SetResolution(srcBitmap.HorizontalResolution, srcBitmap.VerticalResolution);
                Graphics g = Graphics.FromImage(returnBitmap);
                g.DrawImage(srcBitmap, 0, 0);
            }
            return returnBitmap;
        }
    }
}
