using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unicomer.Cosacs.Model;

namespace Unicomer.Cosacs.Repository
{
    public class VendorRepository
    {

        public List<string> UpdateVendorMaster(string objJSON)
        {
            var ICD = new UpdateVendorRepository();
            return ICD.UpdateVendorMaster(objJSON);
        }

        public List<SupplierMaster> GetSupplierEOD(int spanInMinutes)
        {
            var ds = new DataSet();
            var CV = new SupplierEodRepository();
            var retList = new List<SupplierMaster>();
            CV.Fill(ds, spanInMinutes);

            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                retList = ds.Tables[0].Rows.OfType<DataRow>()
                    .Select(p => new SupplierMaster
                    {
                        ResourceType = "Vendor",
                        Source = "COSACS",
                        Active = Convert.ToBoolean(p["Active"]),
                        ExternalVendorID = Convert.ToString(p["ExternalVendorID"]),
                        SupplierName = Convert.ToString(p["SupplierName"]),
                        ContactName = FindValueFromJSon(Convert.ToString(p["ContactName"]), "Contact Name"),
                        ContactTitle = Convert.ToString(p["ContactTitle"]),
                        AddressLine1 = Convert.ToString(p["AddressLine1"]),
                        AddressLine2 = Convert.ToString(p["AddressLine2"]),
                        AddressLine3 = Convert.ToString(p["AddressLine3"]),
                        PostalCode = Convert.ToString(p["PostalCode"]),
                        StateorProvince = Convert.ToString(p["StateorProvince"]),
                        PhoneNumber = FindValueFromJSon(Convert.ToString(p["ContactName"]), "Company Phone"),
                        //Convert.ToInt32(p["PhoneNumber"]),
                        FaxNumber = Convert.ToString(p["FaxNumber"]),
                        EmailAddress = FindValueFromJSon(Convert.ToString(p["ContactName"]), "Contact Email"),
                        //Convert.ToString(p["EmailAddress"]),
                        Notes = Convert.ToString(p["Notes"]),
                        LastUpdatedBy = Convert.ToString(p["LastUpdatedBy"]),
                        SupplierType = p["SupplierType"] != DBNull.Value && !string.IsNullOrWhiteSpace(Convert.ToString(p["SupplierType"])) ? Convert.ToString(p["SupplierType"]).Split(',').ToList<string>() : new List<string>()//["F\",O"]
                    }).ToList();
            }
            return retList;
        }

        private string FindValueFromJSon(string jSon, string key)
        {
            string value = string.Empty;
            if (!string.IsNullOrWhiteSpace(jSon))
            {
                var array = JArray.Parse(jSon);
                if (array != null)
                {
                    JObject jo = array.Children<JObject>().FirstOrDefault(o => o["Key"] != null && o["Key"].ToString() == key);
                    if (key.Contains("Phone"))
                    {
                        value = ((Convert.ToString(jo["Key"]).ToLower().Contains("Company Phone") || Convert.ToString(jo["Key"]).ToLower().Contains("Contact Phone")) && Convert.ToString(jo["Value"]) == string.Empty) ? null : Regex.Replace(Convert.ToString(jo["Value"]), "[^0-9]", "");
                    }
                    else
                    {
                        if (jo != null)
                        {
                            value = (Convert.ToString(jo["Key"]).ToLower().Contains("email") && Convert.ToString(jo["Value"]) == string.Empty) ? null : Convert.ToString(jo["Value"]);
                        }
                    }

                }
            }
            return value;
        }

        public List<SupplierMaster> GetSupplierRTS(string vendorCode)
        {
            var ds = new DataSet();
            var CV = new SupplierRTSRepository();
            var retList = new List<SupplierMaster>();
            CV.Fill(ds, vendorCode);

            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                retList = ds.Tables[0].Rows.OfType<DataRow>()
                    .Select(p => new SupplierMaster
                    {
                        ResourceType = "Vendor",
                        Source = "COSACS",
                        Active = Convert.ToBoolean(p["Active"]),
                        ExternalVendorID = Convert.ToString(p["ExternalVendorID"]),
                        SupplierName = Convert.ToString(p["SupplierName"]),
                        ContactName = FindValueFromJSon(Convert.ToString(p["ContactName"]), "Contact Name"),
                        ContactTitle = Convert.ToString(p["ContactTitle"]),
                        AddressLine1 = Convert.ToString(p["AddressLine1"]),
                        AddressLine2 = Convert.ToString(p["AddressLine2"]),
                        AddressLine3 = Convert.ToString(p["AddressLine3"]),
                        PostalCode = Convert.ToString(p["PostalCode"]),
                        StateorProvince = Convert.ToString(p["StateorProvince"]),
                        PhoneNumber = FindValueFromJSon(Convert.ToString(p["ContactName"]), "Company Phone"),
                        //Convert.ToInt32(p["PhoneNumber"]),
                        FaxNumber = Convert.ToString(p["FaxNumber"]),
                        EmailAddress = FindValueFromJSon(Convert.ToString(p["ContactName"]), "Contact Email"),
                        //Convert.ToString(p["EmailAddress"]),
                        Notes = Convert.ToString(p["Notes"]),
                        LastUpdatedBy = Convert.ToString(p["LastUpdatedBy"]),
                        SupplierType = p["SupplierType"] != DBNull.Value && !string.IsNullOrWhiteSpace(Convert.ToString(p["SupplierType"])) ? Convert.ToString(p["SupplierType"]).Split(',').ToList<string>() : new List<string>()//["F\",O"]
                    }).ToList();
            }
            return retList;
        }
    }
}
