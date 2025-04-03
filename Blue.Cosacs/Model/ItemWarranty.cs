using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Model
{
    public class ItemWarranty
    {
            public string WarrantyContractNo { get; set; }
            public int? WarrantyItemId { get; set; }
            public string WarrantyItemNo { get; set; }
            public short? WarrantyLength { get; set; }
            public decimal? WarrantyTaxRate { get; set; }
            public string WarrantyDepartment { get; set; }
            //public bool? WarrantyIsFree { get; set; }
            public string WarrantyType { get; set; }                     //#17883
            public decimal? WarrantyCostPrice { get; set; }
            public decimal? WarrantyRetailPrice { get; set; }
            public decimal? WarrantySalePrice { get; set; }
            public string WarrantyStatus { get; set; }
            public short? WarrantyStockLocn { get; set; }
            public double? WarrantyQuantity { get; set; }
            public decimal? WarrantyPrice { get; set; }
            public decimal? WarrantyOrdval { get; set; }
            public int? WarrantyParentItemId { get; set; }
            public string WarrantyParentItemNo { get; set; }
            public short? WarrantyParentLocation { get; set; }
            public string WarrantyAcctno { get; set; }
            public int? WarrantyAgrmtno { get; set; }
            public int? WarrantyGroupId { get; set; }
            public DateTime? WarrantyEffectiveDate { get; set; }            //  #17313
    }
}
