using System.Xml.Serialization;
using System;
using System.Collections.Generic;

namespace Blue.Cosacs.Report.Xml
{
    public class Column
    {
        [XmlAttribute("parentcolumn")]
        public string ParentColumn
        {
            get;
            set;
        }

        [XmlAttribute("format")]
        public string Format
        {
            get;
            set;
        }

        [XmlAttribute("type")]
        public string Type
        {
            get;
            set;
        }

        [XmlAttribute("columnName")]
        public string ColumnName
        {
            get;
            set;
        }

        [XmlAttribute("replaceValue")]
        public string ColumnReplaceValue
        {
            get;
            set;
        }
    }
}
