using System;
using System.Collections.Generic;
using PatternRecognition.FingerprintRecognition.Core;

namespace PatternRecognition.FingerprintRecognition.Core
{
    public static class AvgMinutiae
    {
        public static Minutia Compute(ICollection<Minutia> mtiaCollection)
        {
            double sumX = 0;
            double sumY = 0;
            double sumAngleX = 0;
            double sumAngleY = 0;
            foreach (var mtia in mtiaCollection)
            {
                sumX += mtia.X;
                sumY += mtia.Y;
                sumAngleX += Math.Cos(mtia.Angle);
                sumAngleY += Math.Sin(mtia.Angle);
            }

            int n = mtiaCollection.Count;
            return new Minutia()
                       {
                           Angle = Angle.ComputeAngle(sumAngleX / n, sumAngleY / n),
                           X = Convert.ToInt16(Math.Round(sumX / n)),
                           Y = Convert.ToInt16(Math.Round(sumY / n))
                       };
        }

        public static MinutiaPair Compute(List<MinutiaPair> pairs)
        {
            double qX = 0,
                   tX = 0,
                   qY = 0,
                   tY = 0,
                   qAngleX = 0,
                   tAngleX = 0,
                   qAngleY = 0,
                   tAngleY = 0;
            foreach (var pair in pairs)
            {
                qX += pair.QueryMtia.X;
                qY += pair.QueryMtia.Y;
                qAngleX += Math.Cos(pair.QueryMtia.Angle);
                qAngleY += Math.Sin(pair.QueryMtia.Angle);

                tX += pair.TemplateMtia.X;
                tY += pair.TemplateMtia.Y;
                tAngleX += Math.Cos(pair.TemplateMtia.Angle);
                tAngleY += Math.Sin(pair.TemplateMtia.Angle);
            }
            int n = pairs.Count;
            Minutia qMtia = new Minutia()
            {
                Angle = Angle.ComputeAngle(qAngleX / n, qAngleY / n),
                X = Convert.ToInt16(Math.Round(qX / n)),
                Y = Convert.ToInt16(Math.Round(qY / n))
            };
            Minutia tMtia = new Minutia()
            {
                Angle = Angle.ComputeAngle(tAngleX / n, tAngleY / n),
                X = Convert.ToInt16(Math.Round(tX / n)),
                Y = Convert.ToInt16(Math.Round(tY / n))
            };
            return new MinutiaPair()
            {
                QueryMtia = qMtia,
                TemplateMtia = tMtia
            };
        }
    }
}
