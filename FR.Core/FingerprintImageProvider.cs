/*
 * Created by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 * Created: 
 * Comments by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 */

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using PatternRecognition.FingerprintRecognition.Core;

namespace PatternRecognition.FingerprintRecognition.Core
{
    /// <summary>
    ///     Allows retrieving fingerprint image resource.
    /// </summary>
    public class FingerprintImageProvider : IResourceProvider<Bitmap>
    {
        /// <summary>
        ///     Gets the fingerprint image from the specified <see cref="ResourceRepository"/>.
        /// </summary>
        /// <param name="fingerprint">The fingerprint which image is being retrieved.</param>
        /// <param name="repository">The object used to store and retrieve resources.</param>
        /// <returns>The retrieved fingerprint image.</returns>
        object IResourceProvider.GetResource(string fingerprint, ResourceRepository repository)
        {
            return GetResource(fingerprint, repository);
        }

        /// <summary>
        ///     Gets the signature of the fingerprint image provider.
        /// </summary>
        /// <remarks>This method is irrelevant, so it returns an empty string.</remarks>
        public string GetSignature()
        {
            return "";
        }

        /// <summary>
        ///     Determines whether the fingerprint image provider is persistent.
        /// </summary>
        /// <returns>Always returns true.</returns>
        public bool IsResourcePersistent()
        {
            return true;
        }

        /// <summary>
        ///     Gets the fingerprint image from the specified <see cref="ResourceRepository"/>.
        /// </summary>
        /// <param name="fingerprint">The fingerprint which image is being retrieved.</param>
        /// <param name="repository">The object used to store and retrieve resources.</param>
        /// <returns>The retrieved fingerprint image.</returns>
        public Bitmap GetResource(string fingerprint, ResourceRepository repository)
        {
            byte[] rawImage = null;
            foreach (string ext in new[] { "tif", "bmp", "jpg" })
            {
                string resourceName = string.Format("{0}.{1}", fingerprint, ext);
                rawImage = repository.RetrieveResource(resourceName);
                if (rawImage != null)
                    break;
            }
            if (rawImage == null)
                return null;
            Bitmap srcBitmap = Image.FromStream(new MemoryStream(rawImage)) as Bitmap;
            if (srcBitmap == null)
                return null;
            Bitmap returnBitmap;
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