/*
 * Created by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 * Created: 
 * Comments by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 */

using System.Drawing;
using PatternRecognition.FingerprintRecognition.Core;

namespace PatternRecognition.FingerprintRecognition.FeatureDisplay
{
    /// <summary>
    ///     Used to paint skeleton image.
    /// </summary>
    public class SkeletonImageDisplay : FeatureDisplay<SkeletonImage>
    {
        /// <summary>
        ///     Paints the specified <see cref="SkeletonImage"/> using the specified <see cref="Graphics"/>.
        /// </summary>
        /// <param name="skImg">
        ///     The skeleton image to be painted.
        /// </param>
        /// <param name="g">
        ///     The <see cref="Graphics"/> object used to paint the orientation image.
        /// </param>
        public override void Show(SkeletonImage skImg, Graphics g)
        {
            Image img = skImg.ConvertToBitmap();
            g.DrawImage(img, 0, 0);
        }
    }
}
