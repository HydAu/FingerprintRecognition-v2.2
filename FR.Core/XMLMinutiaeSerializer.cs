using System;
using System.Collections.Generic;
using System.Xml;

namespace PatternRecognition.FingerprintRecognition.Core
{
    public static class XMLMinutiaeSerializer
    {
        public static void Serialize(IEnumerable<Minutia> minutiae, string fileName)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml("<MinutiaeList/>");
            foreach (var mtia in minutiae)
            {
                XmlNode nodeMtia = xml.CreateElement("Minutia");
                
                XmlAttribute attX = xml.CreateAttribute("X");
                attX.Value = mtia.X.ToString();
                nodeMtia.Attributes.Append(attX);

                XmlAttribute attY = xml.CreateAttribute("Y");
                attY.Value = mtia.Y.ToString();
                nodeMtia.Attributes.Append(attY);

                XmlAttribute attAngle = xml.CreateAttribute("Angle");
                attAngle.Value = (mtia.Angle * 180 / Math.PI).ToString("f1");
                nodeMtia.Attributes.Append(attAngle);

                XmlAttribute attType = xml.CreateAttribute("Type");
                attType.Value = mtia.MinutiaType.ToString();
                nodeMtia.Attributes.Append(attType);

                xml.DocumentElement.AppendChild(nodeMtia);
            }
            xml.Save(fileName);
        }
    }
}
