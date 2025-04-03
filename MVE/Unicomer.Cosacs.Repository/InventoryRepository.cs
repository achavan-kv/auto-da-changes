using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unicomer.Cosacs.Model;

namespace Unicomer.Cosacs.Repository
{
    public class InventoryRepository
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public List<string> UpdateInventory(string objJSON)
        {
            var ICD = new UpdateInventoryRepository();
            return ICD.PriceUpdate(objJSON);
        }

        public List<string> CreateStockTransfer(string objStockTransfer)
        {
            var ICD = new StockTransferInsertRepository();
            return ICD.CreateStockTransfer(objStockTransfer);
        }
        //public dynamic GetParentSKUEOD(int spanInMinutes)
        //{
        //    var ds = new DataSet();
        //    var CV = new ParentSKUEOD();
        //    List<ParentSKU> retList = new List<ParentSKU>();
        //    CV.Fill(ds, spanInMinutes);

        //    //retList.Message = "No records found.";
        //    //retList.StatusCode = 400;
        //    if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        //    {
        //        List<BranchLink> branchList = (ds != null && ds.Tables != null && ds.Tables.Count > 1) ? ds.Tables[1].Rows.OfType<DataRow>()
        //           .Select(p => new BranchLink()
        //           {
        //               ObjBranch = new Branch()
        //               {
        //                   BranchNo = Convert.ToInt32(p["BranchNo"]),
        //                   Quantity = Convert.ToInt32(p["Quantity"]),
        //                   Retail = Convert.ToDecimal(p["Retail"]),
        //               },
        //               ProductId = Convert.ToString(p["ProductId"])
        //           }).ToList()
        //           : new List<BranchLink>();

        //        retList = ds.Tables[0].Rows.OfType<DataRow>()
        //            .Select(p => new ParentSKU
        //            {
        //                ResourceType = "SKU",
        //                Source = "COSACS",
        //                ProductType = Convert.ToString(p["ProductType"]),
        //                ExternalItemNo = Convert.ToString(p["ExternalItemNo"]),
        //                Description = Convert.ToString(p["Description"]),
        //                UPC = Convert.ToString(p["UPC"]),
        //                ExternalProductID = Convert.ToString(p["ExternalProductID"]),
        //                Cost = Convert.ToDecimal(p["Cost"]),
        //                //Retail = Convert.ToDecimal(p["Retail"]),
        //                Active = Convert.ToBoolean(p["Active"]),
        //                Category = Convert.ToString(p["Category"]),
        //                ExternalTaxID = Convert.ToString(p["ExternalTaxID"]),
        //                ExternalVendorID = Convert.ToString(p["ExternalVendorID"]),
        //                ExternalCommissionID = Convert.ToString(p["ExternalCommissionID"]),
        //                SpectacleLensStyle = Convert.ToString(p["SpectacleLensStyle"]),
        //                //BranchNo = p["BranchNo"] != null ? Convert.ToInt32(p["BranchNo"]) : 0,
        //                //Quantity = p["Quantity"] != null ? Convert.ToInt32(p["Quantity"]) : 0
        //                Branches = branchList
        //                .Where(b => b.ProductId.Equals(Convert.ToString(p["ExternalProductID"])))
        //                .Select(s => new Branch()
        //                {
        //                    BranchNo = s.ObjBranch.BranchNo,
        //                    Quantity = s.ObjBranch.Quantity,
        //                    Retail = s.ObjBranch.Retail
        //                }).ToList()
        //            }).ToList();

        //        //retList.Message = "Records found.";
        //        //retList.StatusCode = 200;
        //    }
        //    return retList;
        //}

        public dynamic StockTransfer(string StockTrfNo)
        {
            StockTransferModel stkTrf = new StockTransferModel();
            try
            {
                var ds = new DataSet();
                var CV = new GetStockTransferRepository();
                List<StockTransferModel> resultList = new List<StockTransferModel>();
                CV.Fill(ds, StockTrfNo);

                List<Products> StockItems = new List<Products>();
                StockItems = ds.Tables[0].Rows.OfType<DataRow>()
                    .Select(p => new Products
                    {
                        productType = p["productType"] != DBNull.Value ? Convert.ToString(p["productType"]).Trim() : String.Empty,
                        productid = p["productid"] != DBNull.Value ? Convert.ToInt32(p["productid"]) : Convert.ToInt32(0),
                        quantity = p["quantity"] != DBNull.Value ? Convert.ToInt32(p["quantity"]) : Convert.ToInt32(0),
                        reference = p["reference"] != DBNull.Value ? Convert.ToString(p["reference"]).Trim() : String.Empty,
                        comments = p["linecomments"] != DBNull.Value ? Convert.ToString(p["linecomments"]).Trim() : String.Empty,
                    }).ToList();
                stkTrf.resourceType = ds.Tables[0].Rows[0]["resourceType"] != DBNull.Value ? Convert.ToString(ds.Tables[0].Rows[0]["resourceType"]).Trim() : String.Empty;
                stkTrf.source = ds.Tables[0].Rows[0]["source"] != DBNull.Value ? Convert.ToString(ds.Tables[0].Rows[0]["source"]).Trim() : String.Empty;
                stkTrf.stocktransferId = ds.Tables[0].Rows[0]["stocktransferId"] != DBNull.Value ? Convert.ToInt32(ds.Tables[0].Rows[0]["stocktransferId"]) : Convert.ToInt32(0);
                stkTrf.sendingLocation = ds.Tables[0].Rows[0]["sendingLocation"] != DBNull.Value ? Convert.ToInt32(ds.Tables[0].Rows[0]["sendingLocation"]) : Convert.ToInt32(0);
                stkTrf.receivingLocation = ds.Tables[0].Rows[0]["receivingLocation"] != DBNull.Value ? Convert.ToInt32(ds.Tables[0].Rows[0]["receivingLocation"]) : Convert.ToInt32(0);
                stkTrf.vialocation = ds.Tables[0].Rows[0]["vialocation"] != DBNull.Value ? Convert.ToInt32(ds.Tables[0].Rows[0]["vialocation"]) : Convert.ToInt32(0);
                stkTrf.documentReferenceNo = ds.Tables[0].Rows[0]["documentReferenceNo"] != DBNull.Value ? Convert.ToString(ds.Tables[0].Rows[0]["documentReferenceNo"]).Trim() : String.Empty;
                stkTrf.comments = ds.Tables[0].Rows[0]["comments"] != DBNull.Value ? Convert.ToString(ds.Tables[0].Rows[0]["comments"]).Trim() : String.Empty;
                stkTrf.createdById = ds.Tables[0].Rows[0]["createdById"] != DBNull.Value ? Convert.ToInt32(ds.Tables[0].Rows[0]["createdById"]) : Convert.ToInt32(0);
                stkTrf.createdDate = ds.Tables[0].Rows[0]["createdDate"] != DBNull.Value ? Convert.ToString(ds.Tables[0].Rows[0]["createdDate"]).Trim() : String.Empty;
                stkTrf.Products = StockItems;
                resultList.Add(stkTrf);

            }
            catch (Exception ex)
            {
                _log.Error("StockTransfer Error " + ex.Message);
            }
            return stkTrf;
        }

    }
}
