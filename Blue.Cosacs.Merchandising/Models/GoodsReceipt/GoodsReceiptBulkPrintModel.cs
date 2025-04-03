using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Merchandising.Models
{
    public class GoodsReceiptBulkPrintModel
    {
        public List<GoodsReceiptPrintModel> GoodsReceiptPrintModels { get; set; }

        public List<GoodsReceiptDirectPrintModel> GoodsReceiptDirectPrintModels { get; set; }

        public bool IncludeCosts { get; set; }
    }
}
