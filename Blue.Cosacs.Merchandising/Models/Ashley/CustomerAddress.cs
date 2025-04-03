using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.Merchandising.Models.Ashley
{
    public class CustomerAddress
    {
        public string AddressType { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string PostCode { get; set; }
        public string Deliveryarea { get; set; }
        public string EMail { get; set; }
        public string DialCode { get; set; }
        public string PhoneNo { get; set; }
        public string Ext { get; set; }
        public string DELTitleC { get; set; }
        public string DELFirstname { get; set; }
        public string DELLastname { get; set; }
        public string Notes { get; set; }
        public DateTime DateIn { get; set; }
        public bool NewRecord { get; set; }
        public string Zone { get; set; }
    }
}
