using System;
using System.Collections.Generic;
using System.Text;

namespace STL.Common.Services.Model
{
    public class WarrantyRenewal : Warranty
    {
        public string WarrantyNumber { get; set; }
        public int Location { get; set; }
        public List<Warranty> Warranties { get; set; }

        public IEnumerable<WarrantyRenewal> ToFlat()
        {
            var display = new List<WarrantyRenewal>();
            if (Warranties != null)                      //#15168
            { 
                foreach (var warranty in Warranties)
                {
                    display.Add(new WarrantyRenewal
                    {
                        Deleted = warranty.Deleted,
                        Description = warranty.Description,
                        Free = warranty.Free,
                        Id = warranty.Id,
                        Length = warranty.Length,
                        Number = warranty.Number,
                        TaxRate = warranty.TaxRate.HasValue == true ? warranty.TaxRate : (decimal?)null,  //#17219
                        Location = this.Location,
                        Price = warranty.Price,
                        CostPrice = warranty.CostPrice,
                        WarrantyNumber = this.WarrantyNumber,
                        TypeCode = warranty.TypeCode            // #17313
                    });
                }
            }
            return display;
        }
    }

    public class Warranty
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public string Description { get; set; }
        public short Length { get; set; }
        public decimal? TaxRate { get; set; }
        public bool Free { get; set; }
        public bool Deleted { get; set; }
        public decimal Price { get; set; }
        public decimal CostPrice { get; set; }
        public string TypeCode { get; set; }        // 17313
    }
}
