using System.Globalization;
using System.IO;
using System.Xml.Serialization;

namespace Blue.Cosacs.Credit.Extensions
{
    public static class CreditExt
    {
        public static string SafeTrim(this string value)
        {
            return string.IsNullOrEmpty(value) ? null : value.Trim();
        }

        public static string SerializeToXml(this object value)
        {
            StringWriter writer = new StringWriter(CultureInfo.InvariantCulture);
            XmlSerializer serializer = new XmlSerializer(value.GetType());
            serializer.Serialize(writer, value);
            return writer.ToString();
        }
    }
}
