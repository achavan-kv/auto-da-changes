using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Blue.Cosacs.Report.Service;

namespace Blue.Cosacs.Report
{
    public partial class ServiceClaims
    {

        internal static IList<ServiceClaimsResult> Fill(string dateLoggedFrom, string dateLoggedTo, string dateResolvedFrom, string dateResolvedTo,
                                                            string supplier, string primaryCharge, string department,
                                                            bool includeTechnicianReport, bool supplierCharged, bool fywCharged, bool ewCharged, short pageNumber, short pageSize)
        {
            var ds = new DataSet();
            var ws = new ServiceClaims();
            var retList = new List<ServiceClaimsResult>();

            ws.Fill(ds, dateLoggedFrom, dateLoggedTo, dateResolvedFrom, dateResolvedTo, supplier, primaryCharge, department, includeTechnicianReport,
                supplierCharged, fywCharged, ewCharged, pageNumber, pageSize);

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                retList = ds.Tables[0].Rows.OfType<DataRow>()
                    .Select(p => new ServiceClaimsResult()
                    {
                        CountryCode = Convert.ToString(p["CountryCode"]).Trim(),
                        ServiceRequestId = Convert.ToInt32(p["ServiceRequestId"]),
                        SupplierName = Convert.ToString(p["SupplierName"]),
                        ProductCategory = Convert.ToString(p["ProductCategory"]),
                        PrimaryCharge = Convert.ToString(p["PrimaryCharge"]),
                        DateLogged = Convert.ToString(p["DateLogged"]),
                        DateResolved = Convert.ToString(p["DateResolved"]),
                        DateDelivered = Convert.ToString(p["DateDelivered"]),
                        DateAccountOpened = Convert.ToString(p["DateAccountOpened"]),
                        FYWDescription = Convert.ToString(p["FYWDescription"]),
                        FYWContractNumber = Convert.ToString(p["FYWContractNumber"]),
                        EWDescription = Convert.ToString(p["EWDescription"]),
                        EWContractNumber = Convert.ToString(p["EWContractNumber"]),
                        ModelNumber = Convert.ToString(p["ModelNumber"]),
                        SerialNumber = Convert.ToString(p["SerialNumber"]),
                        ReplacementIssued = Convert.ToString(p["ReplacementIssued"]),
                        TechnicianReport = Convert.ToString(p["TechnicianReport"]),
                        CustomerName = Convert.ToString(p["CustomerName"]),
                        AccountNumber = Convert.ToString(p["AccountNumber"]),
                        Resolution = Convert.ToString(p["Resolution"]),
                        OriginalProductCostPrice = Convert.ToDecimal(p["OriginalProductCostPrice"]),
                        PartsCost = Convert.ToDecimal(p["PartsCost"]),
                        ProductCode = Convert.ToString(p["ProductCode"]),
                        ProductDescription = Convert.ToString(p["ProductDescription"]),
                        PartNumber = Convert.ToString(p["PartNumber"]),
                        PartDescription = Convert.ToString(p["PartDescription"]),
                        PartQuantity = Convert.ToInt32(p["PartQuantity"]),
                        PartUnitPrice = Convert.ToDecimal(p["PartUnitPrice"]),
                        PartCost = Convert.ToDecimal(p["PartCost"]),
                        SupplierPartsCharge = Convert.ToDecimal(p["SupplierPartsCharge"]),
                        FYWPartsCharge = Convert.ToDecimal(p["FYWPartsCharge"]),
                        EWPartsCharge = Convert.ToDecimal(p["EWPartsCharge"]),
                        SupplierLabourCharge = Convert.ToDecimal(p["SupplierLabourCharge"]),
                        FYWLabourCharge = Convert.ToDecimal(p["FYWLabourCharge"]),
                        EWLabourCharge = Convert.ToDecimal(p["EWLabourCharge"]),
                        SupplierAdditionalCharge = Convert.ToDecimal(p["SupplierAdditionalCharge"]),
                        FYWAdditionalCharge = Convert.ToDecimal(p["FYWAdditionalCharge"]),
                        EWAdditionalCharge = Convert.ToDecimal(p["EWAdditionalCharge"]),
                        FoodLossValue = Convert.ToDecimal(p["FoodLossValue"]),
                        TotalCharge = Convert.ToDecimal(p["TotalCharge"]),
                        PreviousRepairs = Convert.ToString(p["PreviousRepairsWithin90Days"]),
                        TotalRows = int.Parse(p["TotalRows"].ToString()),
                        FywId = DBNull.Value.Equals(p["FYWId"]) ? (int?)null : int.Parse(p["FYWId"].ToString()),
                        EwId = DBNull.Value.Equals(p["EWId"]) ? (int?)null : int.Parse(p["EWId"].ToString()),
                        Comments = p["Comments"].ToString()
                    })
                    .ToList();
            }

            return retList;
        }

    }
}
