
using System;
using System.Data;
using System.IO;
using System.Net;
using System.Web;
using Unicomer.Cosacs.Model;
using Unicomer.Cosacs.Repository;
using Newtonsoft.Json;
using STL.Common;
using STL.Common.Constants.AccountHolders;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using STL.Common.Static;
using System.Configuration;
using System.Globalization;
using System.Threading;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml;

namespace Unicomer.Cosacs.Business
{
    public class TransactionBusiness : Interfaces.ITransaction
    {
        private string _error = String.Empty;
        DateTime _dateProp = DateTime.Now;
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public dynamic CreateAccount(CreateAccount objCreateAccount)
        {
            string AccountType = objCreateAccount.AccountType;
            string CustId = objCreateAccount.CustId;
            JResponse objJResponse = new JResponse();
            TransactionRepository ObjTrans = new TransactionRepository();
            CreateCashaccount objCashAccount = new CreateCashaccount();
            _log.Info("Info : Create Account Json  : " + JsonConvert.SerializeObject(objCreateAccount));
            if (AccountType == "C")
            {
                // string obj = XmlObjectSerializer.Serialize<CashAccount>(objJSON);

                string Error = string.Empty;
                string Country = string.Empty;
                string accountType = string.Empty;
                short Branch;
                bool rescore = false;

                //Read Country maintenance data from DB.
                CountryMaintenanceModel dtCountryMaintenanceModel = new CountryMaintenanceModel();
                dtCountryMaintenanceModel = ObjTrans.GetCountryMaintenceDetails(CustId);
                DataSet DtCountryMaintenance = dtCountryMaintenanceModel.DtCountryMaintenance;
                if (DtCountryMaintenance.Tables.Count == 0)
                {
                    Country = string.Empty;
                    Branch = 0;
                    objJResponse.Result = string.Empty;
                    objJResponse.Status = false;
                    objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    objJResponse.Message = "Customer not found.";
                }
                else
                {
                    Country = (string)DtCountryMaintenance.Tables[0].Rows[0]["countrycode"];
                    //Branch = (short)DtCountryMaintenance.Tables[0].Rows[0]["origbr"];
                    Branch = objCreateAccount.BranchNo;
                    objCashAccount = ObjTrans.CreateCustomerAccount(Country, Branch, CustId, AccountType, 11111, false, out rescore, out Error);
                    if (objCashAccount.accountNumber.Length < 1)
                    {
                        objJResponse.Result = JsonConvert.SerializeObject(objCashAccount);
                        objJResponse.Status = false;
                        objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
                        objJResponse.Message = "Unable to create cash account.";
                    }
                    else
                    {
                        objJResponse.Result = JsonConvert.SerializeObject(objCashAccount);
                        objJResponse.Status = true;
                        objJResponse.StatusCode = (int)HttpStatusCode.OK;
                        objJResponse.Message = "Cash Account Created successfully";
                        _log.Info("Cash Account no: " + objJResponse.Result);
                        //objJResponse.Message = "Cash Account Created successfully with account number:" + " " + objCashAccount.accountNumber;
                    }
                }
            }
            else if (AccountType == "R")
            {
                Blue.Cosacs.Shared.CashLoanDetails det = null;
                bool rescore = false;
                bool reOpenS1 = false;
                bool reOpenS2 = false;
                string Error = string.Empty;
                string Country = string.Empty;
                short Branch;

                CreateRFaccount objRFacct = new CreateRFaccount();

                //Read Country maintenance data from DB.
                CountryMaintenanceModel dtCountryMaintenanceModel = new CountryMaintenanceModel();
                dtCountryMaintenanceModel = ObjTrans.GetCountryMaintenceDetails(objCreateAccount.CustId);
                if (dtCountryMaintenanceModel != null && !string.IsNullOrWhiteSpace(dtCountryMaintenanceModel.Message) && !string.IsNullOrWhiteSpace(dtCountryMaintenanceModel.StatusCode))
                {
                    if (!dtCountryMaintenanceModel.StatusCode.Equals("200"))
                    {
                        objJResponse.Result = string.Empty;
                        objJResponse.Status = false;
                        objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
                        objJResponse.Message = "Customer not found";//Need to create resource file.
                                                                    //objJResponse.Message = dtCountryMaintenanceModel.Message;//Need to create resource file.
                        return objJResponse;
                    }
                }
                DataSet DtCountryMaintenance = dtCountryMaintenanceModel.DtCountryMaintenance;
                DataRow[] foundRows;

                //Read Country and branch code from DB.
                Country = (string)DtCountryMaintenance.Tables[0].Rows[0]["countrycode"];
                //Branch = (short)DtCountryMaintenance.Tables[0].Rows[0]["origbr"];
                Branch = objCreateAccount.BranchNo;
                //(short)Convert.ToInt16(ConfigurationManager.AppSettings["BrachCode"]);

                //(short)DtCountryMaintenance.Tables[0].Rows[0]["hobranchno"];
                foundRows = DtCountryMaintenance.Tables[1].Select("codename='" + CountryParameterNames.MinHPage + "'");
                DateTime DateOfBirth = ObjTrans.GetDateOfBirth(objCreateAccount.CustId);
                bool ValidCustoermAge = VerifyCustomerAge(Convert.ToDecimal(foundRows[0]["value"]), DateOfBirth);//New

                //validate customer Age.
                if (ValidCustoermAge == true)
                    objRFacct = ObjTrans.CreateRFAccount(Country, Branch, objCreateAccount.CustId, 11111, false, ref det,
                                                        out rescore, out reOpenS1, out reOpenS2, out Error);

                objRFacct.accountNumber = objRFacct.accountNumber.Replace("-", "");

                if (!string.IsNullOrEmpty(objRFacct.accountNumber))
                {
                    objJResponse.Result = JsonConvert.SerializeObject(objRFacct);
                    objJResponse.Status = true;
                    objJResponse.StatusCode = (int)HttpStatusCode.OK;
                    objJResponse.Message = "RF Account Created successfully.";
                    _log.Info("RF Account no: " + objJResponse.Result);
                }
                else
                {
                    if (ValidCustoermAge == false)
                    {
                        objJResponse.Result = string.Empty;
                        objJResponse.Status = false;
                        objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
                        objJResponse.Message = "Customer is not eligible because age should be greater than 18";//Need to create resource file.
                    }
                    else
                    {
                        objJResponse.Result = string.Empty;
                        objJResponse.Status = false;
                        objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
                        objJResponse.Message = "Account not registered";//Need to create resource file.
                    }
                }
            }

            return objJResponse;
        }

        private bool VerifyCustomerAge(decimal MinHPage, DateTime DOB)
        {
            bool valid = true;
            if (CalculateAge(DOB) < MinHPage)
            {
                valid = false;
            }


            return valid;
        }

        private decimal CalculateAge(DateTime dtDOB)
        {
            int y = DateTime.Today.Year - dtDOB.Year;
            int m = DateTime.Today.Month - dtDOB.Month;
            int d = DateTime.Today.Day - dtDOB.Day;
            if (d < 0) m--;
            if (m < 0) y--;

            return Convert.ToDecimal(y);
        }

        public dynamic BillGeneration(BillGenerationHeader objJSON, bool isUpdate)
        {
            //var myDetails = JsonConvert.DeserializeObject<BillGenerationHeader>(objJSON);
            _log.Info("Info : Bill Generation Json request with flag " + isUpdate + " : " + JsonConvert.SerializeObject(objJSON));
            string obj = XmlObjectSerializer.Serialize<BillGenerationHeader>(objJSON);
            _log.Info("Info : Bill Generation Xml request with flag " + isUpdate + " : " + PrintXmlsingleLine(obj));

            JResponse objJResponse = new JResponse();
            TransactionRepository objCustomer = new TransactionRepository();
            List<string> Result = objCustomer.CreateBillGeneration(obj, isUpdate);

            if (Result != null && Result.Count > 1)
            {
                if (Result[1].Equals("201"))
                {
                    //objJResponse.Result = string.Empty;
                    objJResponse.Result = JsonConvert.SerializeObject(new { AccountNumber = Result[0] });
                    objJResponse.Status = true;
                    objJResponse.StatusCode = (int)HttpStatusCode.Created;
                    objJResponse.Message = "Bill Generated successfully";//Need to create resource file.
                }
                else if (Result[1].Equals("202"))
                {
                    //objJResponse.Result = string.Empty;
                    objJResponse.Result = JsonConvert.SerializeObject(new { AccountNumber = Result[0] });
                    objJResponse.Status = true;
                    objJResponse.StatusCode = (int)HttpStatusCode.Accepted;
                    objJResponse.Message = "Bill Updated successfully";//Need to create resource file.
                }
                else
                {
                    objJResponse.Result = string.Empty;
                    objJResponse.Status = false;
                    objJResponse.StatusCode = Convert.ToInt32(Result[1]);
                    objJResponse.Message = Result[0];//Need to create resource file.
                }
            }
            else
            {
                objJResponse.Result = string.Empty;
                objJResponse.Status = false;
                objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
                objJResponse.Message = "No record found.";
            }
            return objJResponse;
        }

        public dynamic CreatePO(PurchaseOrderModel objJSON, bool isUpdate)
        {
            _log.Info("Info : Create Purchase Order Json request with flag " + isUpdate + " : " + JsonConvert.SerializeObject(objJSON));
            string obj = XmlObjectSerializer.Serialize<PurchaseOrderModel>(objJSON);
            _log.Info("Info : Create Purchase Order XML request with flag " + isUpdate + " : " + PrintXmlsingleLine(obj));
            JResponse objJResponse = new JResponse();
            TransactionRepository objPurchaseBill = new TransactionRepository();
            List<string> Result = objPurchaseBill.CreatePO(obj, isUpdate);
            if (Result != null && Result.Count > 1)
            {
                if (Result[1].Equals("201"))
                {
                    objJResponse.Result = JsonConvert.SerializeObject(new { PONumber = Result[0] });
                    objJResponse.Status = true;
                    objJResponse.StatusCode = (int)HttpStatusCode.Created;
                    objJResponse.Message = "Purchase Order created successfully";//Need to create resource file.
                    InventoryBusiness ib = new InventoryBusiness();
                    ib.ReIndexing("PO", null);
                    _log.Info("Info : Purchase Order created Indexing Done.");

                }
                else if (Result[1].Equals("202"))
                {
                    objJResponse.Result = JsonConvert.SerializeObject(new { PONumber = Result[0] });
                    objJResponse.Status = true;
                    objJResponse.StatusCode = (int)HttpStatusCode.Accepted;
                    objJResponse.Message = "Purchase Order updated successfully";//Need to create resource file.
                    InventoryBusiness ib = new InventoryBusiness();
                    ib.ReIndexing("PO", null);
                    _log.Info("Info : Purchase Order updated Indexing Done");
                }
                else
                {
                    objJResponse.Result = string.Empty;
                    objJResponse.Status = false;
                    objJResponse.StatusCode = Convert.ToInt32(Result[1]);
                    objJResponse.Message = Result[0];//Need to create resource file.
                }
            }
            else
            {
                objJResponse.Result = string.Empty;
                objJResponse.Status = false;
                objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
                objJResponse.Message = "No record found.";
            }
            return objJResponse;
        }
        public dynamic GetCreditAvailability(string custId)
        {


            JResponse objJResponse = new JResponse();
            TransactionRepository ObjTrans = new TransactionRepository();
            List<string> UserAccount = ObjTrans.GetUserAccount(custId); // 0- Message, 1- Status, 2- maxAmount

            if (UserAccount != null && UserAccount.Count > 1 && !string.IsNullOrWhiteSpace(UserAccount[2]))
            {
                objJResponse.Result = JsonConvert.SerializeObject(new { CustId = UserAccount[2], CreditLimit = UserAccount[3], CreditAvailable = UserAccount[4] });
                objJResponse.Status = true;
                objJResponse.StatusCode = Convert.ToInt16(UserAccount[1]);
                objJResponse.Message = UserAccount[0];
            }
            else
            {
                objJResponse.Result = string.Empty;
                objJResponse.Status = false;
                objJResponse.StatusCode = Convert.ToInt16(UserAccount[1]);
                objJResponse.Message = UserAccount[0];
            }
            return objJResponse;

        }

        public dynamic GetGRN(string GRNNo)
        {
            TransactionRepository objGRN = new TransactionRepository();
            _log.Info("Info" + " - " + "get GRN Details" + " - " + GRNNo);
            return objGRN.GetGRN(GRNNo);
        }
        public dynamic DeliveryAuthorization(string AccountNo, string DocType)
        {
            TransactionRepository objDelAuth = new TransactionRepository();
            _log.Info("Info" + " - " + "Delivery Authorization Details" + " - " + AccountNo + "-" + DocType);
            return objDelAuth.DeliveryAuthorization(AccountNo, DocType);
        }
        public dynamic CreateVendorReturn(VendorReturnModel objJSON)
        {
            _log.Info("Info : Create Vendor Return Json request : " + JsonConvert.SerializeObject(objJSON));
            string obj = XmlObjectSerializer.Serialize<VendorReturnModel>(objJSON);
            _log.Info("Info : Create Vendor Return xml request : " + PrintXmlsingleLine(obj));
            JResponse objJResponse = new JResponse();
            TransactionRepository objVendorReturn = new TransactionRepository();
            List<string> Result = objVendorReturn.CreateVendorReturn(obj);
            if (Result != null && Result.Count > 1)
            {
                if (Result[1].Equals("201"))
                {
                    objJResponse.Result = JsonConvert.SerializeObject(new { VendorReturnId = Result[0] });
                    objJResponse.Status = true;
                    objJResponse.StatusCode = (int)HttpStatusCode.Created;
                    objJResponse.Message = "Vendor return created successfully";//Need to create resource file.
                }
                else
                {
                    objJResponse.Result = string.Empty;
                    objJResponse.Status = false;
                    objJResponse.StatusCode = Convert.ToInt32(Result[1]);
                    objJResponse.Message = Result[0];//Need to create resource file.
                }
            }
            else
            {
                objJResponse.Result = string.Empty;
                objJResponse.Status = false;
                objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
                objJResponse.Message = "No record found.";
            }
            return objJResponse;
        }
        public dynamic CreateCommissions(Commissions objJSON, bool isUpdate)
        {
            var myDetails = JsonConvert.SerializeObject(objJSON);
            _log.Info("Info : Create Commissions Json request : " + isUpdate + " : " + myDetails);
            string obj = XmlObjectSerializer.Serialize<Commissions>(objJSON);
            _log.Info("Info : Create Commissions Xml request : " + isUpdate + " : " + PrintXmlsingleLine(obj));
            JResponse objJResponse = new JResponse();
            TransactionRepository objCreateCommissions = new TransactionRepository();
            List<string> Result = objCreateCommissions.CreateCommissions(obj, isUpdate);
            if (Result != null && Result.Count > 1)
            {
                if (Result[1].Equals("200"))
                {
                    objJResponse.Result = JsonConvert.SerializeObject(new { commissions = Result[0] });
                    objJResponse.Status = true;
                    objJResponse.StatusCode = (int)HttpStatusCode.Created;
                    if (isUpdate == false)
                    {
                        objJResponse.Message = "Commission created successfully";//Need to create resource file.
                    }
                    else
                    {
                        objJResponse.Message = "Commission updated successfully";//Need to create resource file.
                    }
                }
                else
                {
                    objJResponse.Result = string.Empty;
                    objJResponse.Status = false;
                    objJResponse.StatusCode = Convert.ToInt32(Result[1]);
                    objJResponse.Message = Result[0];//Need to create resource file.
                }
            }
            else
            {
                objJResponse.Result = string.Empty;
                objJResponse.Status = false;
                objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
                objJResponse.Message = "No record found.";
            }
            return objJResponse;
        }
        public dynamic SyncDataUpdate(string ServiceCode, string Code, bool IsInsertRecord, bool IsEODRecords, string Message, string Orderid, string ID)
        {
            TransactionRepository objGRN = new TransactionRepository();
            //_log.Info("Info" + " - " + "get GRN Details" + " - " + objGRN.SyncDataUpdate(ServiceCode, Code, IsInsertRecord, IsEODRecords));
            return objGRN.SyncDataUpdate(ServiceCode, Code, IsInsertRecord, IsEODRecords, Message, Orderid, ID);
        }

        /// Get Payments Order Wise
        public dynamic GetPaymentsOrderList(string AcctNo, string ID)
        {
            JResponse objJResponse = new JResponse();
            TransactionRepository ObjTrans = new TransactionRepository();
            Payments objPayments = ObjTrans.GetPaymentsOrderList(AcctNo, ID);// 0- Message, 1- Status, 2- maxAmount

            if (objPayments.checkoutId != 0)
            {
                objJResponse.Result = JsonConvert.SerializeObject(objPayments);
                objJResponse.Status = true;
                objJResponse.StatusCode = (int)HttpStatusCode.OK;
                objJResponse.Message = "Payment Details found.";
            }
            else
            {
                objJResponse.Result = string.Empty;
                objJResponse.Status = false;
                objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
                objJResponse.Message = "No Payment Details found.";
            }
            // return objJResponse;
            return objPayments;

        }

        public dynamic GetCancelPaymentsOrderList(string AcctNo)
        {
            JResponse objJResponse = new JResponse();
            TransactionRepository ObjTrans = new TransactionRepository();
            Payments objPayments = ObjTrans.GetCancelPaymentsOrderList(AcctNo);// 0- Message, 1- Status, 2- maxAmount

            if (objPayments != null)
            {
                objJResponse.Result = JsonConvert.SerializeObject(objPayments);
                objJResponse.Status = true;
                objJResponse.StatusCode = (int)HttpStatusCode.OK;
                objJResponse.Message = "Payment Details found.";
            }
            else
            {
                objJResponse.Result = string.Empty;
                objJResponse.Status = false;
                objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
                objJResponse.Message = "No Payment Details found.";
            }
            // return objJResponse;
            return objPayments;

        }

        public dynamic CreateDeliveryConfirmation(string AccountNo, string Id)
        {
            TransactionRepository objDelConf = new TransactionRepository();
            _log.Info("Info" + " - " + "Delivery Confirmation Details" + " - " + (AccountNo));
            return objDelConf.DeliveryConfirmation(AccountNo, Id);
        }

        static string PrintXmlsingleLine(string xml)
        {
            var stringBuilder = new StringBuilder();
            var element = XElement.Parse(xml);
            var settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = false;
            settings.Indent = false;
            settings.NewLineOnAttributes = false;
            using (var xmlWriter = XmlWriter.Create(stringBuilder, settings))
            {
                element.Save(xmlWriter);
            }
            return stringBuilder.ToString();
        }

        public dynamic CreateCustomerReturn(CustomerReturnModel objJSON)
        {
            _log.Info("Info : Create Customer Return Json request : " + JsonConvert.SerializeObject(objJSON));
            string obj = XmlObjectSerializer.Serialize<CustomerReturnModel>(objJSON);
            _log.Info("Info : Create Customer Return xml request : " + PrintXmlsingleLine(obj));
            JResponse objJResponse = new JResponse();
            TransactionRepository objCustomerReturn = new TransactionRepository();
            List<string> Result = null;
            Result = objCustomerReturn.CreateCustomerReturn(obj);
            if (Result != null)
            {
                if (Result[1].Equals("201"))
                {
                    objJResponse.Result = JsonConvert.SerializeObject(new { CustomerReturnId = Result[0] });
                    objJResponse.Status = true;
                    objJResponse.StatusCode = (int)HttpStatusCode.Created;
                    objJResponse.Message = "Customer return created successfully";//Need to create resource file.
                    InventoryBusiness ib = new InventoryBusiness();
                    int[] CrID = { Convert.ToInt32(Result[0]) };
                    ib.ReIndexing("CR", CrID);  
                    _log.Info("Info : Indexing done Customer Return ID :- " + Result[0]);
                }
                else
                {
                    objJResponse.Result = string.Empty;
                    objJResponse.Status = false;
                    objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    objJResponse.Message = Result[0];//.
                    _log.Info("Info :Else Customer Return ID");
                }
            }
            else
            {
                objJResponse.Result = string.Empty;
                objJResponse.Status = false;
                objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
                objJResponse.Message = "No record found.";
                _log.Info("Info :Main else Customer Return ID");
            }
            return objJResponse;
        }

        public dynamic GetVendorReturn(string VendorReturnID)
        {
            TransactionRepository objVendorReturn = new TransactionRepository();
            _log.Info("Info" + " - " + "get VendorReturn Details" + " - " + VendorReturnID);
            return objVendorReturn.GetVendorReturns(VendorReturnID);
        }
    }

}






