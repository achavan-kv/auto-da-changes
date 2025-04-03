using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unicomer.Cosacs.Model
{
    public class StockTransferModel
    {
        public string resourceType { get; set; }
        public string source { get; set; }
        public int stocktransferId { get; set; }
        public int sendingLocation { get; set; }
        public int receivingLocation { get; set; }
        public int vialocation { get; set; }
        public string documentReferenceNo { get; set; }
        public string comments { get; set; }
        public int createdById { get; set; }
        public string createdDate { get; set; }
        public List<Products> Products { get; set; }
    }

    public class Products
    {
        public string productType { get; set; }
        public int productid { get; set; }
        public int quantity { get; set; }
        public string reference { get; set; }
        public string comments { get; set; }
    }
}
