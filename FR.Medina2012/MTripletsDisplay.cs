/*
 * Created by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 * Created: 
 * Comments by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using PatternRecognition.FingerprintRecognition.FeatureDisplay;
using PatternRecognition.FingerprintRecognition.FeatureExtractors;
using PatternRecognition.FingerprintRecognition.FeatureRepresentation;
using PatternRecognition.FingerprintRecognition.Core;

namespace PRFramework.FingerprintRecognition.FeatureDisplay
{
    public class MTripletsDisplay : FeatureDisplay<List<Minutia>>
    {
        public override void Show(List<Minutia> features, Graphics g)
        {
            var mtpFeatureExtractor = new MTripletsExtractor(){NeighborsCount = 2};
            MtripletsFeature mtriplets = mtpFeatureExtractor.ExtractFeatures(features);

            foreach (MTriplet mt in mtriplets.MTriplets)
            {
                Pen pen = new Pen(Color.Blue) { Width = 2 };
                Point[] points = new Point[3];
                for (int i = 0; i < 3; i++)
                    points[i] = new Point()
                    {
                        X = Convert.ToInt32(mt[i].X),
                        Y = Convert.ToInt32(mt[i].Y)
                    };
                g.DrawPolygon(new Pen(Color.White, 4), points);
                g.DrawPolygon(pen, points);
            }

            var mtiaDisplay = new MinutiaeDisplay();
            mtiaDisplay.Show(mtriplets.Minutiae, g);
        }

    }
}