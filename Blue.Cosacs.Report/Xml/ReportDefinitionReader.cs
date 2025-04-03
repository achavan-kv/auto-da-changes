using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Linq;
using System.Web;

namespace Blue.Cosacs.Report.Xml
{
    internal static class ReportDefinitionReader
    {
        public static Report GetReportDefinition(string reportName)
        {
            return GetReport(reportName);
        }

        private static Report GetReport(string reportName)
        {
            var reports = Load();

            if (reports != null)
            {
                return reports.ReportList.FirstOrDefault(p => string.Compare(p.Id, reportName, true) == 0);
            }

            return null;
        }

        private static Reports Load()
        {
            Reports returnValue;
            var filePath = HttpContext.Current.Server.MapPath(@"~\reports.xml");

            using (var textReader = new StreamReader(filePath))
            {
                var deserializer = new XmlSerializer(typeof(Reports));
                returnValue = (Reports)deserializer.Deserialize(textReader);
            }

            return returnValue;
        }
    }
}
