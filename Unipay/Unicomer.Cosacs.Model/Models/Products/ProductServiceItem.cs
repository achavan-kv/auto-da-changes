using System.Collections.Generic;

namespace Unicomer.Cosacs.Model.Models.Products
{
    public class ProductServiceItem
    {
        public string StoreName { get; set; }
        public string StoreId { get; set; }
        public string Sku { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string UPC { get; set; }
        public string Description { get; set; }
        public string Brand { get; set; }
        public string Style { get; set; }
        public string Color { get; set; }
        public int QuantityInStock { get; set; }
        public string Model { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountProduct { get; set; }
        public string ManufacturerWarranty { get; set; }
        public List<Promotion> Promotions { get; set; }
        public List<Warranty> Warranty { get; set; }
        public List<Gift> Gift { get; set; }
    }
}
