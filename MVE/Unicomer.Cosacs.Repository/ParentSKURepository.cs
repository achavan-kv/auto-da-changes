using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unicomer.Cosacs.Model;

namespace Unicomer.Cosacs.Repository
{
    public class ParentSKURepository
    {
        public List<string> UpdateParentSKUMaster(string objJSON)
        {
            var ICD = new UpdateParentSKUMasterRepository();
            return ICD.UpdateParentSKUMaster(objJSON);
        }
        public dynamic GetParentSKUEOD(int spanInMinutes, string ProductID)
        {
            var ds = new DataSet();
            var CV = new ParentSKUEOD();
            List<ParentSKU> retList = new List<ParentSKU>();
            CV.Fill(ds, spanInMinutes, ProductID);

            //retList.Message = "No records found.";
            //retList.StatusCode = 400;
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                List<BranchLink> branchList = (ds != null && ds.Tables != null && ds.Tables.Count > 1) ? ds.Tables[1].Rows.OfType<DataRow>()
                   .Select(p => new BranchLink()
                   {
                       ObjBranch = new Branch()
                       {
                           BranchNo = Convert.ToInt32(p["BranchNo"]),
                           Quantity = Convert.ToInt32(p["Quantity"]),
                           Retail = Convert.ToDecimal(p["Retail"]),
                       },
                       ProductId = Convert.ToString(p["ProductId"])
                   }).ToList()
                   : new List<BranchLink>();

                retList = ds.Tables[0].Rows.OfType<DataRow>()
                    .Select(p => new ParentSKU
                    {
                        ResourceType = "SKU",
                        Source = "COSACS",
                        ProductType = Convert.ToString(p["ProductType"]),
                        ExternalItemNo = Convert.ToString(p["ExternalItemNo"]),
                        Description = Convert.ToString(p["Description"]),
                        UPC = Convert.ToString(p["UPC"]),
                        Model = Convert.ToString(p["Model"]),
                        Brand = Convert.ToString(p["Brand"]),
                        ExternalProductID = Convert.ToString(p["ExternalProductID"]),
                        VendorCost = Convert.ToDecimal(p["VendorCost"]),
                        AverageWeightedCost = Convert.ToDecimal(p["AverageWeightedCost"]),
                        LatestLandedCost = Convert.ToDecimal(p["LatestLandedCost"]),
                        //Retail = Convert.ToDecimal(p["Retail"]),
                        Active = Convert.ToBoolean(p["Active"]),
                        Category = Convert.ToString(p["Category"]),
                        ExternalTaxID = Convert.ToString(p["ExternalTaxID"]),
                        ExternalVendorID = Convert.ToString(p["ExternalVendorID"]),
                        ExternalCommissionID = Convert.ToString(p["ExternalCommissionID"]),
                        SpectacleLensStyle = Convert.ToString(p["SpectacleLensStyle"]),
                        //BranchNo = p["BranchNo"] != null ? Convert.ToInt32(p["BranchNo"]) : 0,
                        //Quantity = p["Quantity"] != null ? Convert.ToInt32(p["Quantity"]) : 0
                        Branches = branchList
                        .Where(b => b.ProductId.Equals(Convert.ToString(p["ExternalProductID"])))
                        .Select(s => new Branch()
                        {
                            BranchNo = s.ObjBranch.BranchNo,
                            Quantity = s.ObjBranch.Quantity,
                            Retail = s.ObjBranch.Retail
                        }).ToList()
                    }).ToList();

                //retList.Message = "Records found.";
                //retList.StatusCode = 200;
            }
            return retList;
        }
        public UpdateParentSKUResult GetParentSKUUpdate()
        {
            var ds = new DataSet();
            var CV = new ParentSKUMasterUpdate();
            var retList = new UpdateParentSKUResult();
            CV.Fill(ds);

            retList.Message = "No records found.";
            retList.StatusCode = 400;
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                List<BranchLinking> branchList = (ds != null && ds.Tables != null && ds.Tables.Count > 1) ? ds.Tables[1].Rows.OfType<DataRow>()
                   .Select(p => new BranchLinking()
                   {
                       ObjBranch = new BranchNoQuantity()
                       {
                           BranchNo = Convert.ToInt32(p["BranchNo"]),
                           Retail = Convert.ToDecimal(p["Retail"]),
                       },
                       ProductId = Convert.ToString(p["ProductId"])
                   }).ToList()
                   : new List<BranchLinking>();

                retList.UpdateParentSKU = ds.Tables[0].Rows.OfType<DataRow>()
                    .Select(p => new ParentSKUUpdateWithoutQuantity
                    {
                        ResourceType = "SKU",
                        Source = "COSACS",
                        ProductType = Convert.ToString(p["ProductType"]),
                        ExternalItemNo = Convert.ToString(p["ExternalItemNo"]),
                        Description = Convert.ToString(p["Description"]),
                        UPC = Convert.ToString(p["UPC"]),
                        Model = Convert.ToString(p["Model"]),
                        Brand = Convert.ToString(p["Brand"]),
                        ExternalProductID = Convert.ToString(p["ExternalProductID"]),
                        VendorCost = Convert.ToDecimal(p["VendorCost"]),
                        AverageWeightedCost = Convert.ToDecimal(p["AverageWeightedCost"]),
                        LatestLandedCost = Convert.ToDecimal(p["LatestLandedCost"]),
                        //Retail = Convert.ToDecimal(p["Retail"]),
                        Active = Convert.ToBoolean(p["Active"]),
                        Category = Convert.ToString(p["Category"]),
                        ExternalTaxID = Convert.ToString(p["ExternalTaxID"]),
                        ExternalVendorID = Convert.ToString(p["ExternalVendorID"]),
                        ExternalCommissionID = Convert.ToString(p["ExternalCommissionID"]),
                        SpectacleLensStyle = Convert.ToString(p["SpectacleLensStyle"]),
                        Branches = branchList
                        .Where(b => b.ProductId.Equals(Convert.ToString(p["ExternalProductID"])))
                        .Select(s => new BranchNoQuantity()
                        {
                            BranchNo = s.ObjBranch.BranchNo,
                            Retail = s.ObjBranch.Retail
                        }).ToList()
                    }).ToList();

                retList.Message = "Records found.";
                retList.StatusCode = 200;
            }
            return retList;
        }
    }
}
