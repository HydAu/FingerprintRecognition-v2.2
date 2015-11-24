/*
 * Created by: Andrés Eduardo Gutiérrez Rodríguez (andres@bioplantas.cu)
 *             Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 *             
 *             
 * Created: 
 * Comments by: 
 */

using System;
using System.Drawing;
using PatternRecognition.FingerprintRecognition.Core;

namespace PatternRecognition.FingerprintRecognition.FeatureRepresentation
{
    [Serializable]
    internal class OBMtiaDescriptor
    {
        internal double[] Orientations { get; private set; }

        internal Minutia Minutia { get; private set; }

        internal byte EmptyFeaturesCount { private set; get; }

        internal bool IsEmptyFeature(int index)
        {
            return double.IsNaN(Orientations[index]);
        }

        public static implicit operator Minutia(OBMtiaDescriptor desc)
        {
            return desc.Minutia;
        }

        internal OBMtiaDescriptor(Minutia mnt, OrientationImage dImg)
        {
            Minutia = mnt;
            //difRadio = (int) Math.Truncate((Resolution/25.4)*ridgePeriod*2);

            EmptyFeaturesCount = 0;
            Orientations = new double[72];
            for (int i = 0, j = 0; i < 4; i++)
            {
                var curr = GetOrientations(initRadio + i * difRadio, Minutia, dImg);
                for (int k = 0; k < curr.Length; k++)
                {
                    Orientations[j++] = curr[k];
                    if (double.IsNaN(curr[k]))
                        EmptyFeaturesCount++;
                }
            }
        }

        internal double Compare(OBMtiaDescriptor mtiaDesc)
        {
            double sum = 0;
            for (int i = 0; i < 72; i++)
            {
                var or1 = Orientations[i];
                var or2 = mtiaDesc.Orientations[i];
                if (!double.IsNaN(or1) && !double.IsNaN(or2))
                {
                    double diffOr = Math.Abs(or1 - or2);

                    double difAng = (2 / Math.PI) * diffOr;

                    sum += Math.Exp(-16 * difAng);
                }
            }

            return sum / 72;
        }

        #region private 

        [NonSerialized]
        private const int difRadio = 18;

        [NonSerialized]
        private const int initRadio = 27;

        private double[] GetOrientations(int radio, Minutia mtia, OrientationImage dirImg)
        {
            double[] currOrientations = new double[radio / 3];
            int n = radio / 3;
            double incAng = 2 * Math.PI * 3.0 / radio;
            for (int i = 0; i < n; i++)
            {
                double myAng = mtia.Angle + i * incAng;
                if (myAng > 2 * Math.PI)
                    myAng -= (double)(2 * Math.PI);
                Point pnt = SetPosToSPoint(myAng, radio, new Point(mtia.X, mtia.Y));
                int row, col;
                dirImg.GetBlockCoordFromPixel(pnt.X, pnt.Y, out row, out col);
                if ((col < 0) || (row < 0) || (row >= dirImg.Height) ||
                    (col >= dirImg.Width) || (dirImg.IsNullBlock(row, col)))
                    currOrientations[i] = double.NaN;
                else
                    currOrientations[i] =
                        Math.Min(Angle.DifferencePi(mtia.Angle, dirImg.AngleInRadians(row, col)),
                                 Angle.DifferencePi(mtia.Angle, dirImg.AngleInRadians(row, col) + Math.PI));
            }
            return currOrientations;
        }

        private Point SetPosToSPoint(double angle, int radio, Point p)
        {
            Point point = new Point();
            var dx = radio * Math.Cos(angle);
            var dy = radio * Math.Sin(angle);
            point.X = p.X - Convert.ToInt32(Math.Round(dx));
            point.Y = p.Y - Convert.ToInt32(Math.Round(dy));
            return point;
        }

        #endregion
    }
}