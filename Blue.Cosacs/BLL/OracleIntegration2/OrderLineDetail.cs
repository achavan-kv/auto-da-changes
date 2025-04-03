using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace STL.BLL.OracleIntegration2
{
    public class OrderLineDetail
    {
        [XmlElement(IsNullable = true)]
        public string AcctNo;
        public int? OrderNo;
        public DateTime? OrderedDate;
        public string TranClass; //OE.Type
        public int? SalesPersonId;
        [XmlElement(IsNullable = true)]
        public string SalesPersonName;
        public int? LineNumber; //OrderLineNo
        [XmlElement(IsNullable = true)]
        public string ItemNo;
        [XmlElement(IsNullable = true)]
        public string ItemDesc;
        public decimal? OrderedQty;
        public decimal? UnitPrice;
        public string UOM;
        public decimal? LineAmount;
        public string TaxFlag;
        public string TaxCode;
        public decimal? TaxRate;
        public string StatusFlag; //OE.Type 
        public string CancelReason;
        public string ReturnReason;
        public int? DeliveryNumber; //DEL.BuffNo
        public DateTime? DeliveredDate;
        public decimal? DeliveredQty;
        [XmlElement(IsNullable = true)]
        public string DeliveredFromLocn;
        public DateTime? DropOffTime;
        public DateTime? PickUpTime;
        public decimal? FreightCharge;
        [XmlElement(IsNullable = true)]
        public string FreightCarrier;
        [XmlElement(IsNullable = true)]
        public string DeliveryComments;
        public DateTime? ScheduledDelDate;
    }
}
