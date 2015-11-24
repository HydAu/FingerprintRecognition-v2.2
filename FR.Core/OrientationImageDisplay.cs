/*
 * Created by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 * Created: 
 * Comments by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 */

using System;
using System.Drawing;
using PatternRecognition.FingerprintRecognition.Core;

namespace PatternRecognition.FingerprintRecognition.FeatureDisplay
{
    /// <summary>
    ///     Used to paint orientation image.
    /// </summary>
    public class OrientationImageDisplay : FeatureDisplay<OrientationImage>
    {
        #region IFeatureDisplay<List<Minutia>> Members

        /// <summary>
        ///     Paints the specified <see cref="OrientationImage"/> using the specified <see cref="Graphics"/>.
        /// </summary>
        /// <param name="orImg">The orientation image to be painted.</param>
        /// <param name="g">The <see cref="Graphics"/> object used to paint the orientation image.</param>
        public override void Show(OrientationImage orImg, Graphics g)
        {
            int lineLength = orImg.WindowSize / 2;
            Pen greenPen = new Pen(Brushes.Green) { Width = 2 };
            Pen redPen = new Pen(Brushes.Red) { Width = 2 };
            Pen currentPen;

            for (int i = 0; i < orImg.Height; i++)
                for (int j = 0; j < orImg.Width; j++)
                {
                    double angle;
                    if (orImg.IsNullBlock(i, j))
                    {
                        currentPen = redPen;
                        angle = 0;
                    }
                    else
                    {
                        currentPen = greenPen;
                        angle = orImg.AngleInRadians(i, j);
                    }
                    //double angle = orImg.IsNullBlock(i, j) ? 0 : orImg.AngleInRadians(i, j);
                    int x = j * orImg.WindowSize + orImg.WindowSize / 2;
                    int y = i * orImg.WindowSize + orImg.WindowSize / 2;

                    Point p0 = new Point
                    {
                        X = Convert.ToInt32(x - lineLength * Math.Cos(angle)),
                        Y = Convert.ToInt32(y - lineLength * Math.Sin(angle))
                    };

                    Point p1 = new Point
                    {
                        X = Convert.ToInt32(x + lineLength * Math.Cos(angle)),
                        Y = Convert.ToInt32(y + lineLength * Math.Sin(angle))
                    };

                    g.DrawLine(currentPen, p0, p1);
                }
        }

        #endregion
    }
}