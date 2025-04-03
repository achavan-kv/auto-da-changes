using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Unicomer.Cosacs.Model;

namespace Unicomer.Cosacs.Repository
{
    public partial class CustomerRepository
    {
        public CustomerRepository()
        {
            log4net.GlobalContext.Properties["LogFileName"] = ConfigurationManager.AppSettings["LogFolderPath"] != null ? ConfigurationManager.AppSettings["LogFolderPath"] : @"C://MVE_Unicomer//Log//VE_COSACS_";
            log4net.Config.XmlConfigurator.Configure();
        }
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public ValidateCustomerResult ValidateAndGetUesrDetails(ValidatetUser objValidateUser)
        {
            var ds = new DataSet();
            var CV = new CustomerValidate();
            var retList = new ValidateCustomerResult();
            CV.Fill(ds, objValidateUser.IdNumber, objValidateUser.IdType, objValidateUser.PhNumber);

            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                retList = ds.Tables[0].Rows.OfType<DataRow>()
                    .Select(p => new ValidateCustomerResult()
                    {
                        UserResult = new ValidateUserResult
                        {
                            id = Convert.ToString(p["IdNumber"]).Trim(),
                            idType = Convert.ToString(p["IdType"]).Trim(),
                            extUId = Convert.ToString(p["CustomerId"]).Trim(),
                            lastName = Convert.ToString(p["LastName"]).Trim(),
                            firstName = Convert.ToString(p["FirstName"]).Trim(),
                            //DateOfBirth = Convert.ToDateTime(p["DateOfBirth"]),
                            email = Convert.ToString(p["EmailId"]).Trim()
                        },
                        Message = (string)p["Message"],
                        StatusCode = (int)p["StatusCode"]
                    })
                    .FirstOrDefault();
            }
            return retList;
        }
        public List<string> CreateUser(User objUser)
        {
            var ICD = new CustomerInsertRepository();
            List<string> extUId = new List<string>();
            string ExtCustId = string.Empty;
            string Message = string.Empty;
            int StatusCode = 0;
            {
                extUId = ICD.InsertCustomer(objUser.extUId,
                                            objUser.firstName,
                                            objUser.lastName,
                                            objUser.email,
                                            string.IsNullOrWhiteSpace(objUser.phoneNumber) ? string.Empty : objUser.phoneNumber,
                                            objUser.CustId,
                                            Convert.ToDateTime(objUser.DOB),//null,//DOB,
                                            null,//origbr
                                            null, //otherid
                                            0,//objUser.BranchNoHdle.HasValue ? objUser.BranchNoHdle.Value : Convert.ToInt16(0),//branchnohdle
                                            null,//objUser.Title,
                                            null,//alias,
                                            null,//addrsort,
                                            null, //namesort,
                                            null, //sex,
                                            null, //ethnicity,
                                            null, //morerewardsno,
                                            null, //effectivedate,
                                            objUser.idType,
                                            objUser.id,
                                            null,//UserNo
                                            null,//datechange,
                                            null, //maidenname,
                                            null,//StoreType,
                                            null, //dependants,
                                            null, //maritalstat,
                                            null, //Nationality,
                                            null, //ResieveSms, 
                                            null,//objUser.AddressType,
                                           null,// objUser.Address,
                                            null,//objUser.custaddresses.Select(a=>a.cusaddr2), 
                                            null,//objUser.custaddresses.Select(a=>a.cusaddr3),
                                            objUser.NewRecord, //new record                                         
                                            null,//objUser.DeliveryArea,//.custaddresses.Select(a=>a.deliveryarea), 
                                            null,//postcode, 
                                           null,// objUser.Notes,// notes, 
                                           null,//Convert.ToDateTime(objUser.DateIn),//dateIn 
                                            null,//User
                                            null,//zone,
                                            null, //dateteladd 
                                            null, //extnno,
                                            null,//tellocn, 
                                            null,//DialCode, 
                                            null,//empeenochang
                                            ExtCustId,
                                            Message,
                                            StatusCode);
            }
            return extUId;
        }
        public dynamic GetSupplierEOD(int spanInMinutes)
        {
            throw new NotImplementedException();
        }
        public GetAuthQAndA GetAuthQAndA(string CustId)
        {
            GetAuthQAndA objGetAuthQAndA = new GetAuthQAndA();
            List<questionsAndAnswers> questionsAndAnswers = new List<questionsAndAnswers>();
            //List<ListBranch> lstBranch = new List<ListBranch>();
            var ds = new DataSet();
            var SAR = new SecurityAnswerRepository();
            SAR.GetAuthQAndA(ds, CustId);

            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                questionsAndAnswers = ds.Tables[0].Rows.OfType<DataRow>()
                     .Select(p => new questionsAndAnswers()
                     {
                         qId = Convert.ToInt32(p["qId"]),
                         question = Convert.ToString(p["question"]).Trim(),
                         answers = GetAnswerList(Convert.ToString(p["answers"]).Trim()),
                         inputType = Convert.ToString(p["inputType"]),
                         inputCategory = Convert.ToString(p["inputCategory"])

                     }).ToList();


                //lstBranch = ds.Tables[1].Rows.OfType<DataRow>()
                //    .Select(p => new ListBranch()
                //    {
                //        BranchName = Convert.ToString(p["BranchName"]),
                //        BranchNo = Convert.ToInt32(p["BranchNo"])
                //    }).ToList();
            }
            objGetAuthQAndA.questionsAndAnswers = questionsAndAnswers;
            //objGetAuthQAndA.listBranch = lstBranch;
            objGetAuthQAndA.numCorrectRequired = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Attempts"]);
            return objGetAuthQAndA;
        }

        private List<string> GetAnswerList(string v)
        {
            List<string> result = new List<string>();
            if (!string.IsNullOrWhiteSpace(v))
            {
                if (v.Contains(";"))
                    result.AddRange(v.Split(';').ToList());
                else
                    result.Add(v);
            }
            return result;
        }

        public int UpdateUser(UpdateUser objUpdateUser)
        {
            string ExtCustId = string.Empty;
            string Message = string.Empty;
            var CUR = new CustomerUpdateRepository();
            CUR.UpdateUser(objUpdateUser.YPUserID,
                                            objUpdateUser.FirstName,
                                            objUpdateUser.LastName,
                                            objUpdateUser.Email,
                                            null,//PhNumber,
                                            objUpdateUser.CustID,
                                            Convert.ToDateTime(System.DateTime.Now),// Convert.ToDateTime(objUser.DOB),
                                            null,//origbr
                                            null, //otherid
                                            Convert.ToInt16(0),//branchnohdle
                                            null,//Title,
                                            null,//alias,
                                            null,//addrsort,
                                            null, //namesort,
                                            null, //sex,
                                            null, //ethnicity,
                                            null, //morerewardsno,
                                            null, //effectivedate,
                                            null,//IDType,
                                            null,//IDNumber,
                                            null,//UserNo
                                            null,//datechange,
                                            null, //maidenname,
                                            null,//StoreType,
                                            null, //dependants,
                                            null, //maritalstat,
                                            null, //Nationality,
                                            null, //ResieveSms, 
                                            null, //AddressType,
                                            null, //Address,
                                            null,//objUser.custaddresses.Select(a=>a.cusaddr2), 
                                            null,//objUser.custaddresses.Select(a=>a.cusaddr3),
                                            objUpdateUser.NewRecord, //new record                                         
                                            null,//.custaddresses.Select(a=>a.deliveryarea), 
                                            null,//postcode, 
                                            null,// notes, 
                                            Convert.ToDateTime(System.DateTime.Now),//dateIn 
                                            null,//User
                                            null,//zone,
                                            null, //dateteladd 
                                            null, //extnno,
                                            null,//tellocn, 
                                            null,//DialCode, 
                                            null,//empeenochang
                                            ExtCustId,
                                            Message);
            return Convert.ToInt32(ExtCustId);
        }

        public List<string> CreateUsers(string objJSON)
        {
            var ICD = new CustomerXmlInsertRepository();
            //_log.Info("Info" + " - Create Customer " + objJSON);
            // _log.Info("Info" + " - INSERT Customer " + ICD.InsertCustomer(objJSON));
            return ICD.InsertCustomer(objJSON);
        }

        public ParentSKUResult GetParentSKUMaster()
        {
            var ds = new DataSet();
            var CV = new ParentSKUMaster();
            var retList = new ParentSKUResult();
            CV.Fill(ds);

            retList.Message = "No records found.";
            retList.StatusCode = 400;
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

                retList.ParentSKU = ds.Tables[0].Rows.OfType<DataRow>()
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
                        Features = Convert.ToString(p["Features"]),
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

                retList.Message = "Records found.";
                retList.StatusCode = 200;
            }
            return retList;
        }

        public ParentSKUResult GetParentSKUMasterEOD()
        {
            var ds = new DataSet();
            var CV = new ParentSKUMasterEOD();
            var retList = new ParentSKUResult();
            CV.Fill(ds);

            retList.Message = "No records found.";
            retList.StatusCode = 400;
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

                retList.ParentSKU = ds.Tables[0].Rows.OfType<DataRow>()
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
                        Features = Convert.ToString(p["Features"]),
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

                retList.Message = "Records found.";
                retList.StatusCode = 200;
            }
            return retList;
        }

        public SupplierMasterResult GetSupplierMaster()
        {
            var ds = new DataSet();
            var CV = new Supplier();
            var retList = new SupplierMasterResult();
            CV.Fill(ds);


            retList.Message = "No records found.";
            retList.StatusCode = 400;
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                retList.SupplierMaster = ds.Tables[0].Rows.OfType<DataRow>()
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

                retList.Message = "Records found.";
                retList.StatusCode = 200;
            }
            _log.Info("Info" + " - Vendor Record" + retList);
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

        public List<Customer> SearchCustomer(CustomerRequest customerRequest)
        {
            List<Customer> customerList = null;
            var ds = new DataSet();
            var CV = new CustomerSearch();
            CV.Fill(ds, customerRequest);

            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                customerList = ds.Tables[0].Rows.OfType<DataRow>()
                   .Select(p => new Customer()
                   {
                       ResourceType = "Customer",
                       Source = "Cosacs",
                       IdType = p["IdType"] != DBNull.Value ? Convert.ToString(p["IdType"]).Trim() : String.Empty,
                       ID = p["IdNumber"] != DBNull.Value ? Convert.ToString(p["IdNumber"]).Trim() : String.Empty,
                       Title = p["Title"] != DBNull.Value ? Convert.ToString(p["Title"]).Trim() : String.Empty,
                       FirstName = p["FirstName"] != DBNull.Value ? Convert.ToString(p["FirstName"]).Trim() : String.Empty,
                       LastName = p["LastName"] != DBNull.Value ? Convert.ToString(p["LastName"]).Trim() : String.Empty,
                       BranchNo = p["BranchNo"] != DBNull.Value ? Convert.ToInt16(p["BranchNo"]) : Convert.ToInt16(0),
                       CustomerID = p["CustomerID"] != DBNull.Value ? Convert.ToString(p["CustomerID"]).Trim() : String.Empty,
                       DOB = p["DOB"] != DBNull.Value ? Convert.ToString(p["DOB"]).Trim() : string.Empty,
                       //Mobile = p["Mobile"] != DBNull.Value ? Convert.ToString(p["Mobile"]).Trim() : String.Empty,
                       Address = ds.Tables.Count > 1 ? ds.Tables[1].Rows.OfType<DataRow>()
                       .Where(ad => Convert.ToString(ad["CustomerId"]) == Convert.ToString(p["CustomerId"]))
                       .Select(a => new Address()
                       {
                           DeliveryArea = a["DeliveryArea"] != DBNull.Value ? Convert.ToString(a["DeliveryArea"]).Trim() : String.Empty,
                           Address1 = a["Address1"] != DBNull.Value ? Convert.ToString(a["Address1"]).Trim() : String.Empty,
                           Address2 = a["Address2"] != DBNull.Value ? Convert.ToString(a["Address2"]).Trim() : String.Empty,
                           Address3 = a["Address3"] != DBNull.Value ? Convert.ToString(a["Address3"]).Trim() : String.Empty,
                           AddressType = a["AddressType"] != DBNull.Value ? Convert.ToString(a["AddressType"]).Trim() : String.Empty,
                           DateIn = a["DateIn"] != DBNull.Value ? Convert.ToString(a["DateIn"]).Trim() : string.Empty,
                           Email = a["Email"] != DBNull.Value ? Convert.ToString(a["Email"]).Trim() : String.Empty,
                           Notes = a["Notes"] != DBNull.Value ? Convert.ToString(a["Notes"]).Trim() : String.Empty,
                           Postalcode = a["Postalcode"] != DBNull.Value ? Convert.ToString(a["Postalcode"]) : String.Empty
                       }).ToList() : new List<Address>(),
                       Contact = ds.Tables.Count > 2 ? ds.Tables[2].Rows.OfType<DataRow>()
                       .Where(ct => Convert.ToString(ct["CustomerId"]) == Convert.ToString(p["CustomerId"]))
                       .Select(c => new Contact()
                       {
                           ContactNumber = c["ContactNumber"] != DBNull.Value ? Convert.ToString(c["ContactNumber"]).Trim() : String.Empty,
                           ContactLocation = c["ContactLocation"] != DBNull.Value ? Convert.ToString(c["ContactLocation"]).Trim() : String.Empty,
                           Ext = c["Ext"] != DBNull.Value ? Convert.ToString(c["Ext"]) : string.Empty
                       }).ToList() : new List<Contact>()
                   }).ToList();
            }
            _log.Info("Info" + " - Search List of Customer " + customerList);
            return customerList;
        }

        public static DateTime GetCultDateTime(string dateString)
        {
            DateTime dateVal = DateTime.MinValue;
            string[] formats = { "dd/MM/yyyy", "MM/dd/yyyy", "MM/dd/yyyy hh:mm:ss tt", "M/d/yyyy", "d/M/yyyy", "M-d-yyyy", "d-M-yyyy", "d-MMM-yy", "d-MMMM-yyyy" };
            for (int i = 0; i < formats.Length; i++)
            {
                DateTime date;
                if (DateTime.TryParseExact(dateString, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                {
                    dateVal = date;
                    break;
                }
            }
            return dateVal;
        }

        public List<string> UpdateUser(string objJSON)
        {
            var ICD = new CustomerXmlUpdateRepository();
            //_log.Info("Info" + " - Update Customer " + objJSON);
            return ICD.UpdateCustomer(objJSON);
        }
    }
}
