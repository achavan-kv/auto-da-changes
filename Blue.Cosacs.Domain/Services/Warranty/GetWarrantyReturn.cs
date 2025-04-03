
using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared.Services.Warranty
{
    partial class GetWarrantyReturnRequest
    {
        public string warrantyNumber { get; set; }
        public int warrantyItemID { get; set; }
        public string contractNo { get; set; }
        public int stocklocn { get; set; }
    }

    partial class GetWarrantyReturnResponse
    {
        public STL.Common.Services.Model.WarrantyReturnModel warrantyReturn { get; set; }
        public DateTime? DateDel { get; set; }
    }
}
