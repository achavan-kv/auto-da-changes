using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared.CosacsWeb.Models
{
    public class WarrantyReturnList
    {
        public string warrantyNumber { get; set; }
        public int warrantyItemID { get; set; }
        public string contractNo { get; set; }
        public int stocklocn { get; set; }

    }

    public class WarrantyReturnDetails
    {
        public STL.Common.Services.Model.WarrantyReturnModel warrantyReturn { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? WarrantyDeliveredOn { get; set; }          // #17506
        public string WarrantyContractNo { get; set; }
    }
}
