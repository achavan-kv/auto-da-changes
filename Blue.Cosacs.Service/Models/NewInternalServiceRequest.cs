using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Blue.Cosacs.Service.Models;

namespace Blue.Cosacs.Service
{
    public partial class NewInternalServiceRequest
    {
        internal static IList<CustomerSearchResult> Fill(string strType, string customerId, string customerAccount)
        {
            var ds = new DataSet();
            var ws = new NewInternalServiceRequest();
            var retList = new List<CustomerSearchResult>();

            ws.Fill(ds, strType, customerId, customerAccount);

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                retList = ds.Tables[0].Rows.OfType<DataRow>()
                    .Select(p => new CustomerSearchResult()
                    {
                        Account = p["Account"].ToString(),
                        addtype = p["addtype"].ToString(),
                        
                        CustomerAddressLine1 = p["CustomerAddressLine1"].ToString(),
                        CustomerAddressLine2 = p["CustomerAddressLine2"].ToString(),
                        CustomerAddressLine3 = p["CustomerAddressLine3"].ToString(),
                        CustomerFirstName = p["CustomerFirstName"].ToString(),
                        CustomerId = p["CustomerId"].ToString(),
                        CustomerLastName = p["CustomerLastName"].ToString(),
                        CustomerPostcode = p["CustomerPostcode"].ToString(),
                        CustomerTitle = p["CustomerTitle"].ToString(),
                        CustomerNotes = p["CustomerNotes"].ToString(),
                        Item = p["Item"].ToString(),
                        ItemAmount = (decimal)p["ItemAmount"],
                        ItemCostPrice = (decimal)p["ItemCostPrice"],
                        ItemDeliveredOn = (DateTime)p["ItemDeliveredOn"],
                        ItemId = (int)p["ItemId"],
                        ItemNumber = p["ItemNumber"].ToString(),
                        Iupc = p["Iupc"].ToString(),
                        ItemStockLocation = (p["ItemStockLocation"] != DBNull.Value) ? (short?)p["ItemStockLocation"] : null,
                        ItemStockLocationName = p["ItemStockLocationName"].ToString(),
                        ItemSupplier = p["ItemSupplier"].ToString(),
                        ItemSoldOn = (DateTime)p["ItemSoldOn"],
                        ItemSoldBy = (int)p["ItemSoldBy"],
                        ItemSoldByName = p["ItemSoldByName"].ToString(),
                        ItemSerialNumber = p["ItemSerialNumber"].ToString(),
                        WarrantyGroupId = (p["WarrantyGroupId"] != DBNull.Value) ? (int?)p["WarrantyGroupId"] : null,
                        WarrantyNumber = p["WarrantyNumber"].ToString(),
                        WarrantyType = p["WarrantyType"].ToString(),
                        WarrantyLength = ((p["WarrantyLength"] == DBNull.Value) ? null : (int?)p["WarrantyLength"]) ,
                        WarrantyContractNumber = p["WarrantyContractNumber"].ToString(),
                        ManufacturerWarrantyNumber = p["ManufacturerWarrantyNumber"].ToString(),
                        ManufacturerWarrantyContractNumber = p["ManufacturerWarrantyContractNumber"].ToString(),
                        ManufacturerWarrantyLength = (int)p["ManufacturerWarrantyLength"],
                        WarrantyContractId = (p["WarrantyContractId"] != DBNull.Value) ? (int?)p["WarrantyContractId"] : null,
                        TotalItemCount = (int)p["ItemCount"],
                        TotalRequests = (int)p["TotalRequests"]
                    })
                    .ToList();
            }

            return retList;
        }

    }
}
