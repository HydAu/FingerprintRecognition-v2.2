/*
 * Created by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 * Created: Thursday, December 21, 2007
 * Comments by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using PatternRecognition.FingerprintRecognition.Core;

namespace PatternRecognition.FingerprintRecognition.FeatureDisplay
{
    /// <summary>
    ///     Used to paint minutiae.
    /// </summary>
    public class MinutiaeDisplay : FeatureDisplay<List<Minutia>>
    {
        #region IFeatureDisplay<List<Minutia>> Members

        /// <summary>
        ///     Paints the specified minutiae using the specified <see cref="Graphics"/>.
        /// </summary>
        /// <param name="features">The minutiae to be painted.</param>
        /// <param name="g">The <see cref="Graphics"/> object used to paint the minutiae.</param>
        public override void Show(List<Minutia> features, Graphics g)
        {
            int mtiaRadius = 6;
            int lineLength = 18;
            Pen pen = new Pen(Brushes.Blue) { Width = 3 };
            pen.Color = Color.Red;

            Pen whitePen = new Pen(Brushes.Blue) { Width = 5 };
            whitePen.Color = Color.White;

            int i = 0;
            foreach (Minutia mtia in (IList<Minutia>)features)
            {
                g.DrawEllipse(whitePen, mtia.X - mtiaRadius, mtia.Y - mtiaRadius, 2 * mtiaRadius + 1, 2 * mtiaRadius + 1);
                g.DrawLine(whitePen, mtia.X, mtia.Y, Convert.ToInt32(mtia.X + lineLength * Math.Cos(mtia.Angle)), Convert.ToInt32(mtia.Y + lineLength * Math.Sin(mtia.Angle)));

                pen.Color = Color.Red;

                g.DrawEllipse(pen, mtia.X - mtiaRadius, mtia.Y - mtiaRadius, 2 * mtiaRadius + 1, 2 * mtiaRadius + 1);
                g.DrawLine(pen, mtia.X, mtia.Y, Convert.ToInt32(mtia.X + lineLength * Math.Cos(mtia.Angle)), Convert.ToInt32(mtia.Y + lineLength * Math.Sin(mtia.Angle)));
                i++;
            }

            Minutia lastMtia = ((IList<Minutia>)features)[((IList<Minutia>)features).Count - 1];
            //pen.Color = Color.Green;
            g.DrawEllipse(pen, lastMtia.X - mtiaRadius, lastMtia.Y - mtiaRadius, 2 * mtiaRadius + 1, 2 * mtiaRadius + 1);
            g.DrawLine(pen, lastMtia.X, lastMtia.Y, Convert.ToInt32(lastMtia.X + lineLength * Math.Cos(lastMtia.Angle)), Convert.ToInt32(lastMtia.Y + lineLength * Math.Sin(lastMtia.Angle)));
        }

        #endregion

    }
}