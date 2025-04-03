using System.Xml.Serialization;
using System;
using System.Collections.Generic;

namespace Blue.Cosacs.Report.Xml
{
    [XmlRoot(Namespace = "http://www.bluebridltd.com/cosacs/reports/", ElementName = "cosacsreports")]
    public class Reports
    {
        [XmlArray("reports")]
        [XmlArrayItem("report")]
        public List<Report> ReportList
        {
            get;
            set;
        }
    }
}
