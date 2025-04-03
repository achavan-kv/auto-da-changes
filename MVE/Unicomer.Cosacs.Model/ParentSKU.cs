using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unicomer.Cosacs.Model
{
    public class ParentSKUResult
    {
        public List<ParentSKU> ParentSKU { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
    }
    public class ParentSKU
    {
        public string ResourceType { get; set; }
        public string Source { get; set; }
        public string ProductType { get; set; }
        public string ExternalItemNo { get; set; }
        public decimal VendorCost { get; set; }
        public decimal AverageWeightedCost { get; set; }
        public decimal LatestLandedCost { get; set; }

        //public decimal Retail { get; set; }
        public string Description { get; set; }
        public string UPC { get; set; }
        public string Model { get; set; }
        public string Brand { get; set; }
        public string ExternalProductID { get; set; }
        public bool Active { get; set; }
        public string Category { get; set; }
        public string ExternalVendorID { get; set; }
        public string ExternalTaxID { get; set; }
        public string ExternalCommissionID { get; set; }
        public string SpectacleLensStyle { get; set; }
        public dynamic Features { get; set; }
        public List<Branch> Branches { get; set; }


    }

    public class Branch
    {
        public int BranchNo { get; set; }
        public decimal Retail { get; set; }
        public int Quantity { get; set; }
    }

    public class BranchLink
    {
        public Branch ObjBranch { get; set; }
        public string ProductId { get; set; }
    }

    public class ParentSKUUpdate
    {
        public string ResourceType { get; set; }
        public string Source { get; set; }
        public string externalProductID { get; set; }
        public string description { get; set; }
        public string upc { get; set; }
        public string category { get; set; }
        public decimal cost { get; set; }
        public int externalTaxID { get; set; }
        public string externalVendorID { get; set; }
        public int externalCommissionID { get; set; }
        public string spectacleLensStyle { get; set; }
        public List<Branch> Branches { get; set; }
        public bool Active { get; set; }

    }


    public class UpdateParentSKUResult
    {
        public List<ParentSKUUpdateWithoutQuantity> UpdateParentSKU { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
    }
    public class ParentSKUUpdateWithoutQuantity
    {
        public string ResourceType { get; set; }
        public string Source { get; set; }
        public string ProductType { get; set; }
        public string ExternalItemNo { get; set; }
        public decimal VendorCost { get; set; }
        public decimal AverageWeightedCost { get; set; }
        public decimal LatestLandedCost { get; set; }

        //public decimal Retail { get; set; }
        public string Description { get; set; }
        public string UPC { get; set; }
        public string Model { get; set; }
        public string Brand { get; set; }
        public string ExternalProductID { get; set; }
        public bool Active { get; set; }
        public string Category { get; set; }
        public string ExternalVendorID { get; set; }
        public string ExternalTaxID { get; set; }
        public string ExternalCommissionID { get; set; }
        public string SpectacleLensStyle { get; set; }
        public List<BranchNoQuantity> Branches { get; set; }

    }
    public class BranchLinking
    {
        public BranchNoQuantity ObjBranch { get; set; }
        public string ProductId { get; set; }
    }
    public class BranchNoQuantity
    {
        public int BranchNo { get; set; }
        public decimal Retail { get; set; }
    }

}
