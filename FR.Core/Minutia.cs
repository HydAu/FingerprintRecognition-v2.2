/*
 * Created by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 * Created: Thursday, December 21, 2007
 * Comments by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 */

using System;

namespace PatternRecognition.FingerprintRecognition.Core
{
    /// <summary>
    ///     Represents the type of the minutia.
    /// </summary>
    public enum MinutiaType
    {
        /// <summary>
        ///     Represents a minutia which type could not be identified.
        /// </summary>
        Unknown,

        /// <summary>
        ///     Represents a ridge ending.
        /// </summary>
        End,

        /// <summary>
        ///     Represents a ridge bifurcation.
        /// </summary>
        Bifurcation,
    } ;

    /// <summary>
    ///     Represents a minutia.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This minutia representation assumes that the angle is represented in radians.
    ///     </para>
    ///     <para>
    ///         The hash code of these objects uses four bytes per minutia: <see cref="Minutia.X"/> in the left most 11 bits; <see cref="Minutia.Y"/> in the next 11 bits; <see cref="Minutia.Angle"/> in the next eight bits; and <see cref="Minutia.MinutiaType"/> in the last two bits. Therefore, this method is ineffective for values of <see cref="Minutia.X"/> or <see cref="Minutia.Y"/> greater than 2047.
    ///     </para>
    /// </remarks>
    [Serializable]
    public class Minutia
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Minutia"/> class using the specified location and angle.
        /// </summary>
        /// <remarks>
        ///     The default value of <see cref="MinutiaType"/> is <see cref="Core.MinutiaType.Unknown"/>.
        /// </remarks>
        /// <param name="x">
        ///     The horizontal position of the minutia.
        /// </param>
        /// <param name="y">
        ///     The vertical position of the minutia.
        /// </param>
        /// <param name="angle">
        ///     The angle of the minutia.
        /// </param>
        public Minutia(Int16 x, Int16 y, double angle)
        {
            X = x;
            Y = y;
            Angle = angle;
            MinutiaType = MinutiaType.Unknown;
            Flag = false;
        }

        public Minutia()
        {
            X = 0;
            Y = 0;
            Angle = 0;
            MinutiaType = MinutiaType.Unknown;
            Flag = false;
        }

        /// <summary>
        ///     Returns a hash code for the current minutia.
        /// </summary>
        /// <remarks>
        ///     When building hash code, this method stores value X in the left most 11 bits; value Y, in the next 11 bits; value Angle, in the next 8 bits; and value MinutiaType, in the last 2 bits. Therefore, this method could be ineffective for values of X or Y greater than 2047.
        /// </remarks>
        /// <returns>
        ///     A hash code for the current minutia.
        /// </returns>
        public override int GetHashCode()
        {
            // Storing value X in the left most 11 bits.
            int blockX = (2047 & X) << 21;
            // Storing value Y in the next 11 bits.
            int blockY = (2047 & Y) << 10;
            // Storing value Angle in the next 8 bits.
            int blockAngle = Convert.ToByte(Math.Round(Angle * 255 / (2 * Math.PI))) << 2;
            // Storing value MinutiaType in the last 2 bits.
            int blockType = MinutiaType == MinutiaType.Unknown ? 0 : (MinutiaType == MinutiaType.End ? 1 : 2);

            return blockX | blockY | blockAngle | blockType;
        }

        /// <summary>
        ///     Determines whether two specified minutiae have the same property values.
        /// </summary>
        /// <param name="m1">A minutia.</param>
        /// <param name="m2">A minutia.</param>
        /// <returns>
        ///     True if every property value of <paramref name="m1"/> is equal to the respective property value of <paramref name="m2"/>; otherwise, false.
        /// </returns>
        public static bool operator ==(Minutia m1, Minutia m2)
        {
            return m1.X == m2.X && m1.Y == m2.Y && m1.Angle == m2.Angle;
        }

        /// <summary>
        ///     Determines whether two specified minutiae have different property values.
        /// </summary>
        /// <param name="m1">A minutia.</param>
        /// <param name="m2">A minutia.</param>
        /// <returns>
        ///     True if every property value of <paramref name="m1"/> is different from the respective property value of <paramref name="m2"/>; otherwise, false.
        /// </returns>
        public static bool operator !=(Minutia m1, Minutia m2)
        {
            return !Equals(m1, m2);
        }

        /// <summary>
        ///     Gets or sets the horizontal position of the minutia.
        /// </summary>
        public Int16 X { set; get; }

        /// <summary>
        ///     Gets or sets the vertical position of the minutia.
        /// </summary>
        public Int16 Y { set; get; }

        /// <summary>
        ///     Gets or sets the angle of the minutia in radians.
        /// </summary>
        public double Angle { set; get; }

        /// <summary>
        ///     Gets or sets the minutia type.
        /// </summary>
        public MinutiaType MinutiaType { set; get; }

        /// <summary>
        ///     Allows flagging the minutia;
        /// </summary>
        public bool Flag { set; get; }
    }

    /// <summary>
    ///     Utility class used to store two minutiae and its matching value.
    /// </summary>
    public class MinutiaPair
    {
        /// <summary>
        ///     Gets or sets a minutia from the query fingerprint.
        /// </summary>
        public Minutia QueryMtia { set; get; }

        /// <summary>
        ///     Gets or sets a minutia from the template fingerprint.
        /// </summary>
        public Minutia TemplateMtia { set; get; }

        /// <summary>
        ///     Gets or sets the matching value of the two minutiae.
        /// </summary>
        public double MatchingValue { set; get; }

        /// <summary>
        ///     Returns a hash code for the current <see cref="MinutiaPair"/>.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (QueryMtia.GetHashCode() * 397) ^ TemplateMtia.GetHashCode();
            }
        }

        public override string ToString()
        {
            return GetHashCode().ToString();
        }

        /// <summary>
        ///     Determines whether two specified pairs have the same minutiae.
        /// </summary>
        /// <param name="mp1">A minutia pair.</param>
        /// <param name="mp2">A minutia pair.</param>
        /// <returns>
        ///     True if the <see cref="QueryMtia"/> objects of the specified pairs have equal values and the <see cref="TemplateMtia"/> objects have equal values.
        /// </returns>
        public static bool operator ==(MinutiaPair mp1, MinutiaPair mp2)
        {            
            if (!ReferenceEquals(null, mp1))
                return mp1.Equals(mp2);
            if (ReferenceEquals(null, mp1) && ReferenceEquals(null, mp2))
                return true;
            return false;
        }

        /// <summary>
        ///     Determines whether two specified pairs have different minutiae.
        /// </summary>
        /// <param name="mp1">A minutia pair.</param>
        /// <param name="mp2">A minutia pair.</param>
        /// <returns>
        ///     True if the <see cref="QueryMtia"/> objects of the specified pairs have different values or the <see cref="TemplateMtia"/> objects have different values.
        /// </returns>
        public static bool operator !=(MinutiaPair mp1, MinutiaPair mp2)
        {
            if (!ReferenceEquals(null, mp1))
                return !mp1.Equals(mp2);
            if (ReferenceEquals(null, mp1) && ReferenceEquals(null, mp2))
                return false;
            return true;
        }

        /// <summary>
        ///     Determines wheter the specified <see cref="MinutiaPair"/> and the current object have the same minutiae. 
        /// </summary>
        /// <param name="obj">
        ///     The <see cref="MinutiaPair"/> to compare with the current object.
        /// </param>
        /// <returns>
        ///     True if the <see cref="QueryMtia"/> and <see cref="TemplateMtia"/> objects of the specified pair have the same values as the <see cref="QueryMtia"/> and <see cref="TemplateMtia"/> objects of the current object respectively.
        /// </returns>
        public bool Equals(MinutiaPair obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj.QueryMtia.Equals(QueryMtia) && obj.TemplateMtia.Equals(TemplateMtia);
        }

        /// <summary>
        ///     Determines wheter the specified object and the current <see cref="MinutiaPair"/> have the same minutiae. 
        /// </summary>
        /// <param name="obj">
        ///     The object to compare with the current <see cref="MinutiaPair"/>.
        /// </param>
        /// <returns>
        ///     True if the <see cref="QueryMtia"/> and <see cref="TemplateMtia"/> objects of the specified pair have the same values as the <see cref="QueryMtia"/> and <see cref="TemplateMtia"/> objects of the current object respectively.
        /// </returns>
        public override bool Equals(object obj)
        {
            MinutiaPair mp = obj as MinutiaPair;
            return Equals(mp);
        }
    }

    public class MtiaMapper
    {
        public MtiaMapper(Minutia query, Minutia template)
        {
            dAngle = template.Angle - query.Angle;
            this.template = template;
            this.query = query;
        }

        public Minutia Map(Minutia m)
        {
            double newAngle = m.Angle + dAngle;
            double sin = Math.Sin(dAngle);
            double cos = Math.Cos(dAngle);
            return new Minutia
            {
                Angle = (newAngle > 2 * Math.PI) ? newAngle - 2 * Math.PI : (newAngle < 0) ? newAngle + 2 * Math.PI : newAngle,
                X = Convert.ToInt16(Math.Round((m.X - query.X) * cos - (m.Y - query.Y) * sin + template.X)),
                Y = Convert.ToInt16(Math.Round((m.X - query.X) * sin + (m.Y - query.Y) * cos + template.Y))
            };
        }

        private double dAngle;
        private Minutia template;
        private Minutia query;
    }
}