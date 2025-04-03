using System.ComponentModel;
using System.Xml.Serialization;
using System;
using System.Collections.Generic;

namespace Blue.Cosacs.Report.Xml
{
    public class Report
    {
        [XmlAttribute("id")]
        public string Id
        {
            get;
            set;
        }

        [XmlAttribute("serverpaging"),
        DefaultValue(false)]
        public bool IsServerPaging
        {
            get;
            set;
        }

        [XmlElement("query")]
        public string Query
        {
            get;
            set;
        }

        [XmlArray("columns")]
        [XmlArrayItem("column")]
        public List<Column> Columns
        {
            get;
            set;
        }

        [XmlAttribute("source")]
        public string Source
        {
            get;
            set;
        }

    }
}