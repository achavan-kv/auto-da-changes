using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.Sales.Models
{
    public class ReceiptReprintResponse
    {
        public ReceiptReprintResponse()
        {
            OrdersList = new List<ReceiptReprintDto>();
        }

        public int Count { get; set; }
        public List<ReceiptReprintDto> OrdersList { get; set; }
    }
}