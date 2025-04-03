using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Merchandising.Models
{
    public class ProductSalesViewModel
    {
        public ProductSalesViewModel()
        {
            this.Sales = new List<ProductSalesModel>();   
        }
        public int ProductId { get; set; }
        public string Sku { get; set; }
        public int LocationId { get; set; }   
        public string Location { get; set; }
        public List<ProductSalesModel> Sales { get; set; } 
    }
}
