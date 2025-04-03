using System;
using System.Collections.Generic;
using System.Linq;
using Unicomer.Cosacs.Model;
using System.Data;
using System.Data.SqlClient;
using STL.BLL;
using STL.DAL;
using STL.Common.Constants.AuditSource;
using Blue.Cosacs.Repositories;
using Blue.Cosacs.Shared;
//using static Unicomer.Cosacs.Repository.InventoryTransaction;

namespace Unicomer.Cosacs.Repository
{
    public class TransactionRepository
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public CreateRFaccount CreateRFAccount(string countryCode, short branchNo, string customerID, int user, bool isLoan,
                                  ref Blue.Cosacs.Shared.CashLoanDetails det, out bool rescore,
                                  out bool reOpenS1, out bool reOpenS2, out string err)
        {
            CreateRFaccount objRFacct = new CreateRFaccount();
            SqlConnection conn = null;
            rescore = false;
            reOpenS1 = false;
            reOpenS2 = false;
            err = "";

            BAccount acct = null;
            string accountNo = "";
            string auditSource = string.Empty;

            using (conn = new SqlConnection(Connections.Default))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    acct = new BAccount();
                    acct.User = STL.Common.Static.Credential.UserId;

                    if (det == null || (det != null && det.accountNo == "000000000000"))
                    {
                        accountNo = acct.CreateRFAccount(conn, trans, countryCode, branchNo, customerID, user, isLoan, out rescore);

                        reOpenS1 = acct.ReOpenS1;
                        reOpenS2 = acct.ReOpenS2;

                        if (accountNo.Length == 0)
                            err = "Unable to create RF account.";
                    }

                    if (det != null)
                    {
                        var dateprop = new DateTime();
                        var unclearStage = "";
                        var propResult = "";
                        var points = 0;
                        var newaccountNo = "";

                        if (det.accountNo == "000000000000")
                        {
                            auditSource = AS.NewAccount;
                            det.accountNo = accountNo.Replace("-", "");
                        }
                        else
                        {
                            auditSource = AS.Revise;
                            det.accountNo = det.accountNo.Replace("-", "");
                            accountNo = det.accountNo;
                        }

                        DProposal pr = new DProposal();
                        pr.GetUnclearedStage(conn, trans, det.accountNo, ref newaccountNo, ref unclearStage, ref dateprop, ref propResult, ref points);
                        det.unclearStage = unclearStage;
                        det.dateprop = dateprop;

                        BAccount ba = new BAccount();
                        ba.User = STL.Common.Static.Credential.UserId;
                        ba.InsertCashLoanItem(conn, trans, det, auditSource, countryCode, branchNo);

                        if (det.loanStatus == "R")
                        {
                            DProposalFlag pf = new DProposalFlag();
                            pf.EmployeeNoFlag = STL.Common.Static.Credential.UserId;
                            pf.CustomerID = det.custId;
                            pf.DateProp = dateprop;
                            pf.DateCleared = DateTime.MinValue.AddYears(1899);
                            pf.CheckType = "R";
                            pf.Save(conn, trans, det.accountNo);
                        }
                    }

                    trans.Commit();
                }

            }

            objRFacct.accountNumber = accountNo;

            return objRFacct;
        }



        //public CreditApp CreateRFAccount(string countryCode, short branchNo, string customerID, int user, bool isLoan,
        //                              ref Blue.Cosacs.Shared.CashLoanDetails det, out bool rescore,
        //                              out bool reOpenS1, out bool reOpenS2, out string err)
        //{
        //    CreditApp objCrApp = new CreditApp();
        //    SqlConnection conn = null;
        //    rescore = false;
        //    reOpenS1 = false;
        //    reOpenS2 = false;
        //    err = "";


        //    BAccount acct = null;
        //    string accountNo = "";
        //    string auditSource = string.Empty;

        //    using (conn = new SqlConnection(Connections.Default))
        //    {
        //        conn.Open();
        //        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
        //        {
        //            acct = new BAccount();
        //            acct.User = STL.Common.Static.Credential.UserId;

        //            if (det == null || (det != null && det.accountNo == "000000000000"))
        //            {
        //                accountNo = acct.CreateRFAccount(conn, trans, countryCode, branchNo, customerID, user, isLoan, out rescore);


        //                reOpenS1 = acct.ReOpenS1;
        //                reOpenS2 = acct.ReOpenS2;

        //                if (accountNo.Length == 0)
        //                    err = "Unable to create RF account.";
        //            }

        //            if (det != null)
        //            {
        //                var dateprop = new DateTime();
        //                var unclearStage = "";
        //                var propResult = "";
        //                var points = 0;
        //                var newaccountNo = "";

        //                if (det.accountNo == "000000000000")
        //                {
        //                    auditSource = AS.NewAccount;
        //                    det.accountNo = accountNo.Replace("-", "");
        //                }
        //                else
        //                {
        //                    auditSource = AS.Revise;
        //                    det.accountNo = det.accountNo.Replace("-", "");
        //                    accountNo = det.accountNo;
        //                }

        //                DProposal pr = new DProposal();
        //                pr.GetUnclearedStage(conn, trans, det.accountNo, ref newaccountNo, ref unclearStage, ref dateprop, ref propResult, ref points);
        //                det.unclearStage = unclearStage;
        //                det.dateprop = dateprop;

        //                BAccount ba = new BAccount();
        //                ba.User = STL.Common.Static.Credential.UserId;
        //                ba.InsertCashLoanItem(conn, trans, det, auditSource, countryCode, branchNo);

        //                if (det.loanStatus == "R")
        //                {
        //                    DProposalFlag pf = new DProposalFlag();
        //                    pf.EmployeeNoFlag = STL.Common.Static.Credential.UserId;
        //                    pf.CustomerID = det.custId;
        //                    pf.DateProp = dateprop;
        //                    pf.DateCleared = DateTime.MinValue.AddYears(1899);
        //                    pf.CheckType = "R";
        //                    pf.Save(conn, trans, det.accountNo);
        //                }
        //            }

        //            trans.Commit();
        //        }

        //    }

        //    objCrApp.accountNumber = accountNo;

        //    return objCrApp;
        //}

        public CountryMaintenanceModel GetCountryMaintenceDetails(string custId)
        {
            CountryMaintenanceModel dtCountryMaintenanceModel = new CountryMaintenanceModel();
            DataSet ds = new DataSet();
            var CM = new CountryMaintenance();
            List<string> result = CM.CountryMaintenanceDetails(custId, ds);
            dtCountryMaintenanceModel.DtCountryMaintenance = ds;
            if (result != null && result.Count > 1)
            {
                dtCountryMaintenanceModel.Message = result[0];
                dtCountryMaintenanceModel.StatusCode = result[1];
            }
            return dtCountryMaintenanceModel;
        }

        public List<string> CreateBillGeneration(string objJSON, bool isUpdate)
        {
            var ICD = new BillGenerationXmlInsertRepository();
            return ICD.InsertBillGeneration(objJSON, isUpdate);
        }

        public List<string> CreatePO(string objJSON, bool isUpdate)
        {
            var ICD = new POXmlInsertRepository();
            return ICD.InsertPO(objJSON, isUpdate);
        }

        public CreateCashaccount CreateCustomerAccount(string countryCode, short branchNo, string customerID, string accountType, int user,
            bool isLoan, out bool rescore, out string err)
        {
            CreateCashaccount objCashAcct = new CreateCashaccount();
            SqlConnection conn = null;
            rescore = false;
            string Source = "MVE";
            err = "";


            BAccount acct = null;
            string accountNo = "";
            string accountNoreplace = "";
            string auditSource = string.Empty;

            using (conn = new SqlConnection(Connections.Default))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    acct = new BAccount();
                    acct.User = STL.Common.Static.Credential.UserId;

                    accountNo = acct.CreateCustomerAccount(conn, trans, countryCode, branchNo, customerID, accountType, user, isLoan,
                    out rescore, Source);


                    if (accountNo.Length == 0)
                        err = "Unable to create Cash account.";

                    trans.Commit();
                }

            }

            accountNoreplace = accountNo.Replace("-", "");
            objCashAcct.accountNumber = accountNoreplace;

            return objCashAcct;
        }

        public List<string> GetUserAccount(string custId)
        {
            List<string> resultList = new List<string>();
            DataSet ds = new DataSet();
            GetUserAccountsRepository GUAR = new GetUserAccountsRepository();
            resultList = GUAR.GetUserAccounts(ds, custId); // 0- Message, 1- Status, 2- maxAmount
            string CustId = ""; string CreditLimit = "0.00"; string CreditAvailable = "0.00";
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                CustId = custId;
                string CreditLimitvalue = Convert.ToString(ds.Tables[0].Rows[0]["CreditLimit"]);
                string CreditAvailablevalue = Convert.ToString(ds.Tables[0].Rows[0]["CreditAvailable"]);

                if (!string.IsNullOrWhiteSpace(CreditLimitvalue))
                {
                    CreditLimit = Convert.ToString(Math.Round(Convert.ToDecimal(CreditLimitvalue), 2));
                }
                if (!string.IsNullOrWhiteSpace(CreditAvailablevalue))
                {
                    CreditAvailable = Convert.ToString(Math.Round(Convert.ToDecimal(CreditAvailablevalue), 2));
                }
            }
            resultList.Add(CustId);
            resultList.Add(CreditLimit);
            resultList.Add(CreditAvailable);

            return resultList;
        }

        public DateTime GetDateOfBirth(string custId)
        {
            var ds = new DataSet();
            var CT = new GetDateofBirth();

            CT.GetDateofBirthValue(ds, custId);
            return Convert.ToDateTime(ds.Tables[0].Rows[0]["dateborn"]);

        }

        #region GetGRN
        public dynamic GetGRN(string GRNNo)
        {
            GRN grn = new GRN();
            try
            {

                var ds = new DataSet();
                var CV = new GetGRNRepository();
                List<GRN> resultList = new List<GRN>();
                CV.Fill(ds, GRNNo);

                List<grnItems> grnItems = new List<grnItems>();
                grnItems = ds.Tables[0].Rows.OfType<DataRow>()
                    .Select(p => new grnItems
                    {
                        productType = p["productType"] != DBNull.Value ? Convert.ToString(p["productType"]).Trim() : String.Empty,
                        externalItemNo = p["ProductCode"] != DBNull.Value ? Convert.ToString(p["ProductCode"]).Trim() : String.Empty,
                        description = p["description"] != DBNull.Value ? Convert.ToString(p["description"]).Trim() : String.Empty,
                        quantityReceived = p["QuantityReceived"] != DBNull.Value ? Convert.ToInt32(p["QuantityReceived"]) : Convert.ToInt32(0),
                        quantityBackOrdered = p["QuantityBackOrdered"] != DBNull.Value ? Convert.ToInt32(p["QuantityBackOrdered"]) : Convert.ToInt32(0),
                        quantityCancelled = p["QuantityCancelled"] != DBNull.Value ? Convert.ToInt32(p["QuantityCancelled"]) : Convert.ToInt32(0),
                        reasonForCancellation = p["ReasonForCancellation"] != DBNull.Value ? Convert.ToString(p["ReasonForCancellation"]).Trim() : String.Empty,
                        lastLandedCost = p["LastLandedCost"] != DBNull.Value ? Convert.ToDecimal(p["LastLandedCost"]) : Convert.ToDecimal(0),
                    }).ToList();
                grn.resourceType = "GRN";
                grn.source = "MVE";
                grn.externalGRNId = ds.Tables[0].Rows[0]["GRNID"] != DBNull.Value ? Convert.ToString(ds.Tables[0].Rows[0]["GRNID"]).Trim() : String.Empty;
                grn.locationId = ds.Tables[0].Rows[0]["LocationId"] != DBNull.Value ? Convert.ToInt32(ds.Tables[0].Rows[0]["LocationId"]) : Convert.ToInt32(0);
                grn.receivedById = ds.Tables[0].Rows[0]["ReceivedById"] != DBNull.Value ? Convert.ToInt32(ds.Tables[0].Rows[0]["ReceivedById"]) : Convert.ToInt32(0);
                grn.vendorInvoiceNumber = ds.Tables[0].Rows[0]["VendorInvoiceNumber"] != DBNull.Value ? Convert.ToString(ds.Tables[0].Rows[0]["VendorInvoiceNumber"]).Trim() : String.Empty;
                grn.VendorDeliveryNumber = ds.Tables[0].Rows[0]["VendorDeliveryNumber"] != DBNull.Value ? Convert.ToString(ds.Tables[0].Rows[0]["VendorDeliveryNumber"]).Trim() : String.Empty;
                grn.vendorInvoiceDate = ds.Tables[0].Rows[0]["vendorInvoiceDate"] != DBNull.Value ? Convert.ToString(ds.Tables[0].Rows[0]["vendorInvoiceDate"]).Trim() : String.Empty;
                grn.purchaseOrderId = ds.Tables[0].Rows[0]["Po-Id"] != DBNull.Value ? Convert.ToInt32(ds.Tables[0].Rows[0]["Po-Id"]) : Convert.ToInt32(0);
                grn.dateReceived = ds.Tables[0].Rows[0]["DateReceived"] != DBNull.Value ? Convert.ToString(ds.Tables[0].Rows[0]["DateReceived"]).Trim() : String.Empty;
                grn.comments = ds.Tables[0].Rows[0]["Comments"] != DBNull.Value ? Convert.ToString(ds.Tables[0].Rows[0]["Comments"]).Trim() : String.Empty;
                grn.grnItems = grnItems;
                resultList.Add(grn);
            }
            catch (Exception ex)
            {
                _log.Error("Error " + ex.Message);
            }
            return grn;
        }

        #endregion

        #region DeliveryAuthorization
        public dynamic DeliveryAuthorization(string AcctNo, string DocType)
        {
            var ds = new DataSet();
            var CV = new DeliveryAuthRepository();
            List<DeliveryAuth> resultList = new List<DeliveryAuth>();
            CV.Fill(ds, AcctNo, DocType);

            DeliveryAuth delAuth = new DeliveryAuth();
            //if (ds.Tables[0].Rows.Count > 0)
            //{
            //    delAuth.resourceType = "DeliveryAuthorization";
            //    delAuth.source = "Cosacs";
            //    delAuth.checkoutID = Convert.ToInt32(ds.Tables[0].Rows[0]["CheckOutID"]);
            //    delAuth.authorization = Convert.ToBoolean(ds.Tables[0].Rows[0]["Authorization"]);
            //}

            List<orderDetails> orderDetail = new List<orderDetails>();
            orderDetail = ds.Tables[0].Rows.OfType<DataRow>()
                .Select(p => new orderDetails
                {
                    orderID = p["OrderNo"] != DBNull.Value ? Convert.ToInt32(p["OrderNo"]) : Convert.ToInt32(0),
                }).ToList();
            delAuth.resourceType = "DeliveryAuthorization";
            delAuth.source = "Cosacs";
            delAuth.checkoutID = ds.Tables[0].Rows[0]["CheckOutID"] != DBNull.Value ? Convert.ToInt32(ds.Tables[0].Rows[0]["CheckOutID"]) : Convert.ToInt32(0);
            delAuth.authorization = Convert.ToBoolean(ds.Tables[0].Rows[0]["Authorization"]);
            delAuth.orderDetails = orderDetail;
            return delAuth;
        }
        #endregion

        #region CreateVendorReturn
        public List<string> CreateVendorReturn(string objJSON)
        {
            var ICD = new VendorReturnXmlInsertRepository();
            return ICD.InsertVendorReturn(objJSON);
        }

        #endregion

        public List<string> CreateCommissions(string objJSON, bool isUpdate)
        {
            var ICD = new CommissionsXmlInsertRepository();
            return ICD.InsertCommissions(objJSON, isUpdate);
        }

        #region SyncDataUpdates
        public dynamic SyncDataUpdate(string ServiceCode, string Code, bool IsInsertRecord, bool IsEODRecords, string Message, string Orderid, string ID)
        {
            var ds = new DataSet();
            var CV = new DeleteTaskSchedularRepository();
            DeleteScheduleRecord resultList = new DeleteScheduleRecord();
            _log.Info("Info : SyncDataUpdate  Result : " + ServiceCode + " - " + Code + " - " + IsInsertRecord + " - " + IsEODRecords + " - " + Message + " - " + Orderid + "-" + ID);
            var aa = CV.DeleteSyncDocumentUpdate(ServiceCode, Code, IsInsertRecord, IsEODRecords, Message, Orderid, ID);
            return aa;
        }

        #endregion

        /// Get Payments Order Wise
        #region GetPaymentsOrderList
        public dynamic GetPaymentsOrderList(string AcctNo, string ID)
        {
            GetPaymentsOrderListRepository GPR = new GetPaymentsOrderListRepository();
            var ds = new DataSet();
            List<Payments> PaymentsList = new List<Payments>();
            PaymentsList = GPR.GetPaymentsOrderList(ds, AcctNo, ID);
            Payments pyt = new Payments();
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                List<Orders> OrderList = new List<Orders>();
                OrderList = ds.Tables[0].Rows.OfType<DataRow>()
                    .Select(ord => new Orders
                    {
                        orderId = ord["OrderId"] != DBNull.Value ? Convert.ToInt32(ord["OrderId"]) : Convert.ToInt32(0),
                        externalItemNo = ord["ExternalItemNo"] != DBNull.Value ? Convert.ToString(ord["ExternalItemNo"]).Trim() : String.Empty,
                        amount = ord["Amount"] != DBNull.Value ? Convert.ToDecimal(ord["Amount"]) : Convert.ToDecimal(0),

                    }).ToList();


                pyt.resourceType = "Payments";
                pyt.source = "Cosacs";
                pyt.externalPaymentID = ds.Tables[0].Rows[0]["ExternalPaymentId"] != DBNull.Value ? Convert.ToString(ds.Tables[0].Rows[0]["ExternalPaymentId"]).Trim() : String.Empty;
                pyt.checkoutId = ds.Tables[0].Rows[0]["CheckOutId"] != DBNull.Value ? Convert.ToInt32(ds.Tables[0].Rows[0]["CheckOutId"]) : Convert.ToInt32(0);
                pyt.paymentType = ds.Tables[0].Rows[0]["PaymentType"] != DBNull.Value ? Convert.ToString(ds.Tables[0].Rows[0]["PaymentType"]).Trim() : String.Empty;
                pyt.adjustmentType = ds.Tables[0].Rows[0]["AdjustmentType"] != DBNull.Value ? Convert.ToString(ds.Tables[0].Rows[0]["AdjustmentType"]).Trim() : String.Empty;
                //pyt.PaymentDate = ds.Tables[0].Rows[0]["PaymentDate"] != DBNull.Value ? Convert.ToDateTime(ds.Tables[0].Rows[0]["PaymentDate"]) : Convert.ToDateTime(0);
                pyt.paymentDate = ds.Tables[0].Rows[0]["PaymentDate"] != DBNull.Value ? Convert.ToString(ds.Tables[0].Rows[0]["PaymentDate"]) : String.Empty;
                pyt.employeeID = ds.Tables[0].Rows[0]["EmployeeId"] != DBNull.Value ? Convert.ToInt32(ds.Tables[0].Rows[0]["EmployeeId"]) : Convert.ToInt32(0);
                pyt.paymentMethod = ds.Tables[0].Rows[0]["PaymentMethod"] != DBNull.Value ? Convert.ToString(ds.Tables[0].Rows[0]["PaymentMethod"]).Trim() : String.Empty;
                pyt.checkNumber = ds.Tables[0].Rows[0]["ChequeNumber"] != DBNull.Value ? Convert.ToString(ds.Tables[0].Rows[0]["ChequeNumber"]).Trim() : String.Empty;

                pyt.orderList = OrderList;
                PaymentsList.Add(pyt);
            }
            return pyt;
        }
        #endregion

        #region GetPaymentsOrderList
        public dynamic GetCancelPaymentsOrderList(string AcctNo)
        {
            GetCancelPaymentsOrderListRepository GPR = new GetCancelPaymentsOrderListRepository();
            var ds = new DataSet();
            List<Payments> PaymentsList = new List<Payments>();
            PaymentsList = GPR.GetCancelPaymentsOrderList(ds, AcctNo);
            Payments pyt = new Payments();
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                List<Orders> OrderList = new List<Orders>();
                OrderList = ds.Tables[0].Rows.OfType<DataRow>()
                    .Select(ord => new Orders
                    {
                        orderId = ord["OrderId"] != DBNull.Value ? Convert.ToInt32(ord["OrderId"]) : Convert.ToInt32(0),
                        externalItemNo = ord["ExternalItemNo"] != DBNull.Value ? Convert.ToString(ord["ExternalItemNo"]).Trim() : String.Empty,
                        amount = ord["Amount"] != DBNull.Value ? Convert.ToDecimal(ord["Amount"]) : Convert.ToDecimal(0),

                    }).ToList();


                pyt.resourceType = "Payments";
                pyt.source = "Cosacs";
                pyt.externalPaymentID = ds.Tables[0].Rows[0]["ExternalPaymentId"] != DBNull.Value ? Convert.ToString(ds.Tables[0].Rows[0]["ExternalPaymentId"]).Trim() : String.Empty;
                pyt.checkoutId = ds.Tables[0].Rows[0]["CheckOutId"] != DBNull.Value ? Convert.ToInt32(ds.Tables[0].Rows[0]["CheckOutId"]) : Convert.ToInt32(0);
                pyt.paymentType = ds.Tables[0].Rows[0]["PaymentType"] != DBNull.Value ? Convert.ToString(ds.Tables[0].Rows[0]["PaymentType"]).Trim() : String.Empty;
                pyt.adjustmentType = ds.Tables[0].Rows[0]["AdjustmentType"] != DBNull.Value ? Convert.ToString(ds.Tables[0].Rows[0]["AdjustmentType"]).Trim() : String.Empty;
                //pyt.PaymentDate = ds.Tables[0].Rows[0]["PaymentDate"] != DBNull.Value ? Convert.ToDateTime(ds.Tables[0].Rows[0]["PaymentDate"]) : Convert.ToDateTime(0);
                pyt.paymentDate = ds.Tables[0].Rows[0]["PaymentDate"] != DBNull.Value ? Convert.ToString(ds.Tables[0].Rows[0]["PaymentDate"]) : String.Empty;
                pyt.employeeID = ds.Tables[0].Rows[0]["EmployeeId"] != DBNull.Value ? Convert.ToInt32(ds.Tables[0].Rows[0]["EmployeeId"]) : Convert.ToInt32(0);
                pyt.paymentMethod = ds.Tables[0].Rows[0]["PaymentMethod"] != DBNull.Value ? Convert.ToString(ds.Tables[0].Rows[0]["PaymentMethod"]).Trim() : String.Empty;
                pyt.checkNumber = ds.Tables[0].Rows[0]["ChequeNumber"] != DBNull.Value ? Convert.ToString(ds.Tables[0].Rows[0]["ChequeNumber"]).Trim() : String.Empty;

                pyt.orderList = OrderList;
                PaymentsList.Add(pyt);
            }
            return pyt;
        }
        #endregion

        #region DeliveryConfirmation
        public dynamic DeliveryConfirmation(string AcctNo, string Id)
        {
            var ds = new DataSet();
            var CV = new DeliveryConfirmationRepository();
            CV.Fill(ds, AcctNo, Id);

            List<items> DeliveryConfirmationItems = new List<items>();
            DeliveryConfirm objDC = new DeliveryConfirm();
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DeliveryConfirmationItems = ds.Tables[0].Rows.OfType<DataRow>()
                    .Select(p => new items
                    {
                        orderId = Convert.ToString(p["OrderId"]),
                        type = Convert.ToString(p["Type"]),
                        itemNo = Convert.ToString(p["Itemno"]),
                        quantityOrdered = Convert.ToInt32(p["QuantityOrdered"]),
                        quantityDelivered = Convert.ToInt32(p["QuantityDelivered"]),
                        addressType = Convert.ToString(p["AddressType"]),
                        delivered = Convert.ToBoolean(p["Delivered"]),
                        comments = Convert.ToString(p["Comments"]),
                        deliveryDate = Convert.ToString(p["DeliveryDate"]),
                        employeeId = Convert.ToInt32(p["employeeId"])
                    }).ToList();
                objDC.resourceType = "DeliveryConfirmation";
                objDC.source = "Cosacs";
                objDC.checkoutID = Convert.ToString(ds.Tables[0].Rows[0]["CheckOutId"]);
                objDC.items = DeliveryConfirmationItems;
            }
            return objDC;
        }

        #endregion

        #region CreateCustomerReturn
        public List<string> CreateCustomerReturn(string objJSON)
        {
            var ICD = new CustomerReturnXmlInsertRepository();
            return ICD.InsertCustomerReturn(objJSON);
        }

        #endregion


        #region GetVendorReturns
        public dynamic GetVendorReturns(string VendorReturnID)
        {
            var ds = new DataSet();
            var CV = new GetVendorReturnRepository();
            List<VendorReturnModel> resultList = new List<VendorReturnModel>();
            CV.Fill(ds, VendorReturnID);
            List<VendorReturnList> VReturnList = new List<VendorReturnList>();
            VReturnList = ds.Tables[0].Rows.OfType<DataRow>()
                .Select(p => new VendorReturnList
                {
                    productType = p["productType"] != DBNull.Value ? Convert.ToString(p["productType"]).Trim() : String.Empty,
                    externalItemNo = p["externalItemNo"] != DBNull.Value ? Convert.ToString(p["externalItemNo"]).Trim() : String.Empty,
                    description = p["description"] != DBNull.Value ? Convert.ToString(p["description"]).Trim() : String.Empty,
                    comments = p["comments"] != DBNull.Value ? Convert.ToString(p["comments"]).Trim() : String.Empty,
                    quantityReturned = p["quantityReturned"] != DBNull.Value ? Convert.ToInt32(p["quantityReturned"]) : Convert.ToInt32(0),
                    unitLandedCost = p["unitLandedCost"] != DBNull.Value ? Convert.ToDecimal(p["unitLandedCost"]) : Convert.ToDecimal(0),
                }).ToList();

            VendorReturnModel VRList = new VendorReturnModel();
            VRList.resourceType = "VendorReturn";
            VRList.source = "MVE";
            VRList.externalVendorReturnId = ds.Tables[0].Rows[0]["externalVendorReturnId"] != DBNull.Value ? Convert.ToString(ds.Tables[0].Rows[0]["externalVendorReturnId"]).Trim() : String.Empty;
            VRList.externalGRNId = ds.Tables[0].Rows[0]["externalGRNId"] != DBNull.Value ? Convert.ToString(ds.Tables[0].Rows[0]["externalGRNId"]).Trim() : String.Empty;
            VRList.createdById = ds.Tables[0].Rows[0]["createdById"] != DBNull.Value ? Convert.ToInt32(ds.Tables[0].Rows[0]["createdById"]) : Convert.ToInt32(0);
            VRList.referenceNumber = ds.Tables[0].Rows[0]["referenceNumber"] != DBNull.Value ? Convert.ToString(ds.Tables[0].Rows[0]["referenceNumber"]).Trim() : String.Empty;
            VRList.createdDate = ds.Tables[0].Rows[0]["createdDate"] != DBNull.Value ? Convert.ToString(ds.Tables[0].Rows[0]["createdDate"]).Trim() : String.Empty;
            VRList.approvedDate = ds.Tables[0].Rows[0]["approvedDate"] != DBNull.Value ? Convert.ToString(ds.Tables[0].Rows[0]["approvedDate"]).Trim() : String.Empty;
            VRList.approvedById = ds.Tables[0].Rows[0]["approvedById"] != DBNull.Value ? Convert.ToInt32(ds.Tables[0].Rows[0]["approvedById"]) : Convert.ToInt32(0);

            VRList.vendorDeliveryNumber = ds.Tables[0].Rows[0]["vendorDeliveryNumber"] != DBNull.Value ? Convert.ToString(ds.Tables[0].Rows[0]["vendorDeliveryNumber"]).Trim() : String.Empty;
            VRList.vendorInvoiceNumber = ds.Tables[0].Rows[0]["vendorInvoiceNumber"] != DBNull.Value ? Convert.ToString(ds.Tables[0].Rows[0]["vendorInvoiceNumber"]).Trim() : String.Empty;
            VRList.externalPONumber = ds.Tables[0].Rows[0]["externalPONumber"] != DBNull.Value ? Convert.ToString(ds.Tables[0].Rows[0]["externalPONumber"]).Trim() : String.Empty;

            VRList.VendorReturnList = VReturnList;
            resultList.Add(VRList);
            return VRList;
        }

        #endregion
    }
}

