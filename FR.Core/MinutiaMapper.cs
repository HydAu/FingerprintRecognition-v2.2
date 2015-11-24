using System;

namespace PatternRecognition.FingerprintRecognition.Core
{
    public class MinutiaMapper
    {
        public MinutiaMapper(Minutia minutia)
        {
            Minutia t = new Minutia(0, 0, 0);
            dAngle = t.Angle - minutia.Angle;
            this.template = t;
            this.query = minutia;
        }

        public MinutiaMapper(Minutia query, Minutia template)
        {
            dAngle = template.Angle - query.Angle;
            this.template = template;
            this.query = query;
        }

        public Minutia Map(Minutia m)
        {
            return new Minutia
            {
                Angle = m.Angle + dAngle,
                X = Convert.ToInt16(Math.Round((m.X - query.X) * Math.Cos(dAngle) - (m.Y - query.Y) * Math.Sin(dAngle) + template.X)),
                Y = Convert.ToInt16(Math.Round((m.X - query.X) * Math.Sin(dAngle) + (m.Y - query.Y) * Math.Cos(dAngle) + template.Y))
            };
        }

        private double dAngle;
        private Minutia template;
        private Minutia query;
    }
}
