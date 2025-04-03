
using Blue.Cosacs.Merchandising.Models.Ashley;
using Blue.Cosacs.Merchandising.Repositories;
using Blue.Cosacs.Service.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Http.Results;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Transactions;
using Blue.Cosacs.Merchandising;
using AutoMapper;
using Domain = Blue.Cosacs.Service;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Web.Script.Serialization;
using System.Reflection;
using System.Linq;

namespace Blue.Cosacs.Web.Areas.Merchandising.Controllers
{
    /// <summary>
    /// Title : RenissanceSales creation in CoSACs
    /// Date : 01/03/2019
    /// Author : Rahul Dubey
    /// Details : Creates a cash sale in cosacs system for Renissance sale.
    /// </summary>
    public class RenissanceSalesController : Controller
    {
        private readonly IRenissanceSalesRepository objRenissanceSalesDataRepository;

        public RenissanceSalesController(IRenissanceSalesRepository objRenissanceSalesDataRepository)
        {
            this.objRenissanceSalesDataRepository = objRenissanceSalesDataRepository;
        }

        #region Create Ashley Customer 
        public int NotificationOfRenissanceSale()
        {
            string strRenissanceSalesData = objRenissanceSalesDataRepository.GetRenissanceSaleData();

            if (strRenissanceSalesData != string.Empty)
            {
                DataSet ds = JsonConvert.DeserializeObject<DataSet>(strRenissanceSalesData);

                List<Customer> lsCust = new List<Customer>();

                foreach (DataRow drCust in ds.Tables[0].Rows)
                {
                    Customer objCustomer = new Customer();

                    objCustomer.custid = drCust["Custid"].ToString();
                    objCustomer.title = drCust["title"].ToString();
                    objCustomer.firstName = drCust["firstName"].ToString();
                    objCustomer.lastName = drCust["lastName"].ToString();
                    objCustomer.alias = drCust["alias"].ToString();
                    objCustomer.accountNo = Convert.ToString(drCust["accountNo"]);
                    objCustomer.relationship = drCust["relationship"].ToString();
                    objCustomer.dob = Convert.ToDateTime(drCust["dob"].ToString());
                    objCustomer.accountType = drCust["accountType"].ToString();
                    objCustomer.maidenName = drCust["maidenName"].ToString();
                    objCustomer.loyaltyCardNo = drCust["loyaltyCardNo"].ToString();
                    objCustomer.countryCode = drCust["CountryCode"].ToString();
                    objCustomer.storeType = drCust["storeType"].ToString();
                    objCustomer.otherTabs = drCust["otherTabs"].ToString();
                    objCustomer.maritalStat = drCust["maritalStat"].ToString();
                    objCustomer.dependants = Convert.ToInt32(drCust["dependants"].ToString());
                    objCustomer.nationality = drCust["nationality"].ToString();
                    objCustomer.resieveSms = Convert.ToBoolean(drCust["resieveSms"].ToString());
                    objCustomer.branchNo = Convert.ToInt16(drCust["branchNo"].ToString());
                    objCustomer.CreatedBy = drCust["CreatedBy"].ToString();
                    objCustomer.Err = drCust["Err"].ToString();

                    List<CustomerAddress> objAddress = new List<CustomerAddress>();

                    foreach (DataRow drCustAddr in ds.Tables[1].Select("CutomerId =" + drCust["id"].ToString()))
                    {
                        CustomerAddress objCustAddress = new CustomerAddress();
                        objCustAddress.AddressType = drCustAddr["AddressType"].ToString();
                        objCustAddress.Address1 = drCustAddr["Address1"].ToString();
                        objCustAddress.Address2 = drCustAddr["Address2"].ToString();
                        objCustAddress.Address3 = drCustAddr["Address3"].ToString();
                        objCustAddress.PostCode = drCustAddr["PostCode"].ToString();
                        objCustAddress.Deliveryarea = drCustAddr["Deliveryarea"].ToString();
                        objCustAddress.EMail = drCustAddr["EMail"].ToString();
                        objCustAddress.DialCode = drCustAddr["DialCode"].ToString();
                        objCustAddress.PhoneNo = drCustAddr["PhoneNo"].ToString();
                        objCustAddress.Ext = drCustAddr["Ext"].ToString();
                        objCustAddress.DELTitleC = drCustAddr["DELTitleC"].ToString();
                        objCustAddress.DELFirstname = drCustAddr["DELFirstname"].ToString();
                        objCustAddress.DELLastname = drCustAddr["DELLastname"].ToString();
                        objCustAddress.Notes = drCustAddr["Notes"].ToString();
                        objCustAddress.DateIn = Convert.ToDateTime(drCustAddr["DateIn"].ToString());
                        objCustAddress.NewRecord = Convert.ToBoolean(drCustAddr["NewRecord"].ToString());
                        objCustAddress.Zone = drCustAddr["Zone"].ToString();

                        objAddress.Add(objCustAddress);
                    }

                    objCustomer.RenissanceCustomerAddressesList = objAddress;

                    List<LineItemList> objLineItemList = new List<LineItemList>();

                    foreach (DataRow drCustSale in ds.Tables[2].Select("CutomerId =" + drCust["id"].ToString()))
                    {

                        LineItemList objLineItems = new LineItemList();

                        objLineItems.Key = drCustSale["Key"].ToString();
                        //Productid = drCustSale["ProductId"].ToString(),
                        objLineItems.ItemNo = drCustSale["ItemNo"].ToString();
                        objLineItems.BranchNo = drCustSale["stocklocn"].ToString();
                        objLineItems.SaleQty = Convert.ToInt16(drCustSale["SaleQty"].ToString());
                        objLineItems.DelivaryDate = drCustSale["DelivaryDate"].ToString();
                        objLineItemList.Add(objLineItems);
                    }
                    objCustomer.RenissanceLineItemList = objLineItemList;

                    lsCust.Add(objCustomer);
                }

                // Call To Create Customer function  
                CreateCustomer(lsCust);
            }

            // return Json(objRenissanceSalesRepository.GetRenissanceSaleData(), JsonRequestBehavior.AllowGet);

            return 1;
        }

        // <summary>
        // Creates Ashley Customer
        // </summary>
        // <param name = "ashCust" > AshleyCustomer ashCust</param>
        // 
        public string CreateCustomer(List<Customer> renissanceCustomer)
        {

            string err = "";
            try
            {
                int limit = 1;
                int setteled = 1;
                bool exectMatch = true;
                string storeType = "C";
                int agreementno = 0;
                string propResult = "S";
                DateTime dateprop = DateTime.Now;
                string burufailure = "";
                int storeCardTransREfNo = 0;
                string Referralreason = "";
                // err = "";


                // Add Forech Loop 
                //foreach (var test in testcust)
                //{

                //int isCustExist = objrenissanceSalesRepository.CustomerSearch(renissanceCustomer, limit, setteled, exectMatch, storeType, out err);
                //if (isCustExist == 0)
                //{

                //}


                foreach (var objrenissanceCustomer in renissanceCustomer)
                {
                    objRenissanceSalesDataRepository.SaveBasicDetails(objrenissanceCustomer, ConvertAddressToDataset(objrenissanceCustomer.RenissanceCustomerAddressesList), out err, objrenissanceCustomer.RenissanceLineItemList);
                }

                // Update Receive Data Flag 
                objRenissanceSalesDataRepository.ReceiveRenissanceSaleDataFlag();

                return JsonConvert.SerializeObject(renissanceCustomer);

            }
            catch (Exception ex)
            {

                err = err + "\n" + ex.StackTrace;
                //renissanceCustomer.err = err;
                return JsonConvert.SerializeObject(renissanceCustomer);
            }
        }
        #endregion

        #region Convert Address To DataSet 
        /// <summary>
        /// Function will convert customer addresses from List to Dataset.
        /// </summary>
        /// <param name="addresses">List of customer addresses</param>
        /// <returns></returns>
        public DataSet ConvertAddressToDataset(List<CustomerAddress> addresses)
        {
            DataSet ds = new DataSet();
            ds.Tables.Add("Addresses");
            ds.Tables[0].Columns.Add("AddressType", typeof(string));
            ds.Tables[0].Columns.Add("Address1", typeof(string));
            ds.Tables[0].Columns.Add("Address2", typeof(string));
            ds.Tables[0].Columns.Add("Address3", typeof(string));
            ds.Tables[0].Columns.Add("PostCode", typeof(string));
            ds.Tables[0].Columns.Add("deliveryarea", typeof(string));
            ds.Tables[0].Columns.Add("EMail", typeof(string));
            ds.Tables[0].Columns.Add("DialCode", typeof(string));
            ds.Tables[0].Columns.Add("PhoneNo", typeof(string));
            ds.Tables[0].Columns.Add("Ext", typeof(string));
            ds.Tables[0].Columns.Add("DELTitleC", typeof(string));
            ds.Tables[0].Columns.Add("DELFirstname", typeof(string));
            ds.Tables[0].Columns.Add("DELLastname", typeof(string));
            ds.Tables[0].Columns.Add("DateIn", typeof(string));
            ds.Tables[0].Columns.Add("NewRecord", typeof(string));
            ds.Tables[0].Columns.Add("notes", typeof(string));
            ds.Tables[0].Columns.Add("Zone", typeof(string));

            var latitudeColumn = new DataColumn("Latitude", Type.GetType("System.Double"));// Address Standardization CR2019 - 025
            latitudeColumn.AllowDBNull = true;
            var longitudeColumn = new DataColumn("Longitude", Type.GetType("System.Double")); // Address Standardization CR2019 - 025
            longitudeColumn.AllowDBNull = true;
            ds.Tables[0].Columns.Add(latitudeColumn);
            ds.Tables[0].Columns.Add(longitudeColumn);

            foreach (var record in addresses)
            {
                ds.Tables[0].Rows.Add(record.AddressType,
                                      record.Address1,
                                      record.Address2,
                                      record.Address3,
                                      record.PostCode,
                                      record.Deliveryarea,
                                      record.EMail,
                                      record.DialCode,
                                      record.PhoneNo,
                                      record.Ext,
                                      record.DELTitleC,
                                      record.DELFirstname,
                                      record.DELLastname,
                                      record.DateIn,
                                      record.NewRecord,
                                      record.Notes,
                                      record.Zone,
                                      DBNull.Value,
                                      DBNull.Value);
            }
            return ds;
        }
        #endregion

    }
}