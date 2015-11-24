/*
 * Created by: Andrés Eduardo Gutiérrez Rodríguez (andres@bioplantas.cu)
 *             Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 * Created: 
 * Comments by: 
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using PatternRecognition.FingerprintRecognition.Core;

namespace PatternRecognition.FingerprintRecognition.FeatureRepresentation
{
    [Serializable]
    internal class GOwMtia
    {
        internal GOwMtia(Minutia mnt, OrientationImage dImg)
        {
            Minutia = mnt;
            Segments = new Segment[6];
            for (int i = 0; i < Segments.Length; i++)
            {
                Segments[i] = new Segment(i * (2 * Math.PI / 6) + mnt.Angle, mnt, dImg);
            }
        }

        internal double Compare(GOwMtia gOwMtia)
        {
            double sum = 0.0;
            for (int i = 0; i < Segments.Length; i++)
            {
                for (int j = Math.Min(Segments[i].directions.Length, gOwMtia.Segments[i].directions.Length) - 1; j >= 0; j--)
                {
                    if (double.IsNaN(Segments[i].directions[j]) && !double.IsNaN(gOwMtia.Segments[i].directions[j]))
                        sum += Math.Pow(gOwMtia.Segments[i].directions[j], 2);
                    else
                    {
                        if (!double.IsNaN(Segments[i].directions[j]) && double.IsNaN(gOwMtia.Segments[i].directions[j]))
                            sum += Math.Pow(Segments[i].directions[j], 2);
                        else
                            if (!double.IsNaN(Segments[i].directions[j]) && !double.IsNaN(gOwMtia.Segments[i].directions[j]))
                                sum += Math.Pow(Segments[i].directions[j] - gOwMtia.Segments[i].directions[j], 2);
                    }
                }
                if (Segments[i].directions.Length > gOwMtia.Segments[i].directions.Length)
                {
                    for (int k = Segments[i].directions.Length - 1; k >= gOwMtia.Segments[i].directions.Length; k--)
                        if (!double.IsNaN(Segments[i].directions[k]))
                            sum += Math.Pow(Segments[i].directions[k], 2);
                }
                else
                    if (Segments[i].directions.Length < gOwMtia.Segments[i].directions.Length)
                    {
                        for (int k = gOwMtia.Segments[i].directions.Length - 1; k >= Segments[i].directions.Length; k--)
                            if (!double.IsNaN(gOwMtia.Segments[i].directions[k]))
                                sum += Math.Pow(gOwMtia.Segments[i].directions[k], 2);
                    }
            }
            if (Math.Sqrt(sum) < threshold)
                return (threshold - Math.Sqrt(sum)) / threshold;
            return 0;
        }

        internal Minutia Minutia { get; private set; }

        internal Segment[] Segments { get; private set; }

        public static implicit operator Minutia(GOwMtia desc)
        {
            return desc.Minutia;
        }

        [NonSerialized]
        private const double threshold = 125 * Math.PI;

    }

    [Serializable]
    internal class Segment
    {
        internal Segment(double ang, Minutia mnt, OrientationImage dImg)
        {
            bool endOfPoints = false;
            int i = 1;
            List<double> points = new List<double>();
            while (!endOfPoints)
            {
                Point pnt = SetPosToSPoint(ang, i*interval, new Point(mnt.X, mnt.Y));
                if (IsInBound(pnt, dImg))
                {
                    int row, col;
                    dImg.GetBlockCoordFromPixel(pnt.X, pnt.Y, out row, out col);
                    if ((col < 0) || (row < 0) || (row >= dImg.Height) ||
                        (col >= dImg.Width) || (dImg.IsNullBlock(row, col)))
                        points.Add(double.NaN);
                    else
                        points.Add(Math.Min(Angle.DifferencePi(mnt.Angle, dImg.AngleInRadians(row, col)),
                                                                   Angle.DifferencePi(mnt.Angle, dImg.AngleInRadians(row, col) + Math.PI)));
                    i++;
                }
                else
                    endOfPoints = true;
            }
            bool isLastNan = false;
            int j = points.Count - 1;
            while (!isLastNan && j >= 0)
            {
                if (double.IsNaN(points[j]))
                {
                    points.RemoveAt(j);
                    j--;
                }
                else
                    isLastNan = true;
            }
            directions = points.ToArray();
        }

        internal double[] directions;

        internal Point SetPosToSPoint(double angle, int radio, Point p)
        {
            Point point = new Point();
            var dx = radio * Math.Cos(angle);
            var dy = radio * Math.Sin(angle);
            point.X = p.X - Convert.ToInt32(Math.Round(dx));
            point.Y = p.Y - Convert.ToInt32(Math.Round(dy));
            return point;
        }

        internal bool IsInBound(Point pnt, OrientationImage dImg)
        {
            if (pnt.X > 0 && pnt.X < dImg.Width * dImg.WindowSize &&
                pnt.Y > 0 && pnt.Y < dImg.Height * dImg.WindowSize)
                return true;
            return false;
        }

        [NonSerialized]
        private int interval = 18;
    }

}
