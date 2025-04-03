using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace STL.Common
{
    
    public class BranchDefaultPrintLocation
    {
        [XmlElement("bn")]
        public int BranchNo;
        [XmlElement("dpl")]
        public int DefaultPrintLocation;
    }
}
