using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace STL.BLL.OracleIntegration2
{
    public class SalesOrder
    {
        [XmlElement(IsNullable = true)]
        public string CustId;
        [XmlElement(IsNullable = true)]
        public string AcctNo;
        public int? OrderNo;
        public int? PayTerm;
        public string OrderType;  // AccountType
        [XmlElement(IsNullable = true)]
        public string PayMethod;
        [XmlElement(IsNullable = true)]
        public string BranchNo;
        [XmlElement(IsNullable = true)]
        public string BillToAddr;
        [XmlElement(IsNullable = true)]
        public string ShipToAddr;
        [XmlElement(IsNullable = true)]
        public string SalesChannel; //AG.SOA
        public DateTime? InterfacedDate;

        public List<OrderLineDetail> OrderLineList = new List<OrderLineDetail>();
    }
}
