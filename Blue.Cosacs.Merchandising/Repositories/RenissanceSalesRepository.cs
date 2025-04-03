using STL.BLL;
using STL.DAL;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Blue.Cosacs.Merchandising.Models.Ashley;
using Blue.Cosacs.Repositories;
using Context = Blue.Cosacs.Merchandising.Context;
using System.Xml;
using STL.Common.Services;
using STL.Common.Services.Model;
using System.Collections.Generic;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.Tags;
using System.Collections.Generic;
using Unicomer.CosacsService;

namespace Blue.Cosacs.Merchandising.Repositories
{
    public interface IRenissanceSalesRepository
    {
        bool SaveBasicDetails(Customer renissanceCustomer, DataSet addresses, out string err, List<LineItemList> lineItems);
        int CustomerSearch(Customer renissanceCustomer, int limit, int settled, bool exactMatch, string storeType, out string err);
        string GetRenissanceSaleData();
        void ReceiveRenissanceSaleDataFlag();
    }
    public class RenissanceSalesRepository : IRenissanceSalesRepository
    {
        public string AcctNo { get; set; }
        public Decimal Netpayment { get; set; }
        #region Create Customer, Account, LineItem & Save Payment  
        /// <summary>
        /// Saves Basic Details of Renissance Customer and return True if data saved sucessfully or return false if any exeption occured
        /// </summary>
        /// <param name="renissanceCustomer">Object of Renissance Customer</param>
        /// <param name="err">An output paramerter which stroes the exception if any occured</param>
        bool IRenissanceSalesRepository.SaveBasicDetails(Customer renissanceCustomer, DataSet addresses, out string err, List<LineItemList> lineItems)
        {
            SqlConnection conn = null;
            AcctNo = "";
            string Function = "RenissanceSalesAPI::SaveBasicDetails()";
            STL.Common.Static.Credential.User = renissanceCustomer.CreatedBy;
            //renissanceCustomer.CreatedBy = user;
            err = "";
            string Remark = "";
            BAccount ObjBAccount = null;
            bool isLoan = false;
            bool rescore = false;

            string propResult = "";
            string bureauFailure = "";
            int storeCardTransRefNo = 0;
            DateTime dateProp = DateTime.Now;
            int agreementNo = 1;
            string referralreasons = string.Empty;

            // Below Variable Required for Customer Search
            int limit = 1;
            int setteled = 1;
            bool exectMatch = true;
            string storeType = "C";

            try
            {
                conn = new SqlConnection(Connections.Default);

                try
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
                    {
                        BCustomer objBCustomer = new BCustomer();

                        // Create Customer 
                        Remark = "Error: Customer Basic details are not able to save";
                        err = "Error: Customer Basic details are not able to save";
                        if (string.IsNullOrEmpty(renissanceCustomer.accountNo) || renissanceCustomer.accountNo == "")
                        {
                            renissanceCustomer.accountNo = null;
                        }

                        //int isCustExist = CustomerSearch(renissanceCustomer, limit, setteled, exectMatch, storeType, out err);

                        //if (isCustExist == 0)
                        //{

                        // Create CustomerId for New Customer
                        //var customerid = renissanceCustomer.firstName + renissanceCustomer.dob;

                        objBCustomer.SaveBasicDetails(conn, trans, renissanceCustomer.firstName + DateTime.UtcNow.AddYears(Convert.ToInt16(DateTime.Now.ToString("mm"))).ToString("ddMMyyyy"), renissanceCustomer.title, renissanceCustomer.firstName, renissanceCustomer.lastName, renissanceCustomer.alias, renissanceCustomer.accountNo,
                                                renissanceCustomer.relationship, renissanceCustomer.CreatedBy, renissanceCustomer.dob, renissanceCustomer.accountType, renissanceCustomer.maidenName,
                                                renissanceCustomer.loyaltyCardNo, renissanceCustomer.countryCode, null, renissanceCustomer.maritalStat, renissanceCustomer.dependants, renissanceCustomer.nationality, renissanceCustomer.storeType,
                                                renissanceCustomer.resieveSms);

                        // Create Customer Address 
                        Remark = "Error: CustomerAddresses details are not able to save";
                        err = "Error: CustomerAddresses details are not able to save";
                        objBCustomer.SaveCustomerAddresses(conn, trans, renissanceCustomer.firstName + DateTime.UtcNow.AddYears(Convert.ToInt16(DateTime.Now.ToString("mm"))).ToString("ddMMyyyy"), addresses, renissanceCustomer.CreatedBy);
                        //}

                        // Create Customer Account 
                        ObjBAccount = new BAccount();
                        ObjBAccount.User = STL.Common.Static.Credential.UserId;
                        if (AcctNo == "")
                            AcctNo = ObjBAccount.CreateCustomerAccount(conn, trans, renissanceCustomer.countryCode, renissanceCustomer.branchNo,
                                renissanceCustomer.firstName + DateTime.UtcNow.AddYears(Convert.ToInt16(DateTime.Now.ToString("mm"))).ToString("ddMMyyyy"), renissanceCustomer.accountType, Convert.ToInt32(renissanceCustomer.CreatedBy),
                                isLoan, out rescore, "NewApp");

                        if (AcctNo.Length == 0)
                        {
                            err = "Error : Unable to create customer account.";
                            Remark = "Error : Unable to create customer account.";
                            throw new Exception("Error : Unable to create customer account.");
                        }

                        Remark = "Error: LineItems details are not able to save";
                        err = "Error: LineItems details are not able to save";

                        // Create/Add in LineItem 
                        string strAccNo = AcctNo.Replace("-", "");

                        SaveNewAccountParameters objSaveParameter = new SaveNewAccountParameters();
                        objSaveParameter.LineItems = lineItems;
                        objSaveParameter.BranchNo = renissanceCustomer.branchNo;
                        objSaveParameter.AccountType = renissanceCustomer.accountType;
                        objSaveParameter.CountryCode = renissanceCustomer.countryCode;

                        objSaveParameter = SaveNewAccountParametersData(objSaveParameter, lineItems);

                        ObjBAccount.SaveNewAccount(conn, trans, strAccNo, renissanceCustomer.branchNo, renissanceCustomer.accountType, objSaveParameter.CODFlag, objSaveParameter.SalesPerson,
                                 objSaveParameter.SOA, Convert.ToDecimal(objSaveParameter.AgreementTotal), Convert.ToDecimal(objSaveParameter.Deposit),
                                 Convert.ToDecimal(objSaveParameter.ServiceCharge), objSaveParameter.ItemDetails, objSaveParameter.TermsType, objSaveParameter.NewBand,
                                 objSaveParameter.CountryCode, objSaveParameter.DateFirst,
                                 Convert.ToDecimal(objSaveParameter.InstalAmount),
                                 Convert.ToDecimal(objSaveParameter.FinalInstalment), objSaveParameter.PaymentMethod, objSaveParameter.Months,objSaveParameter.PrefDay,  
                                 objSaveParameter.TaxExempt, objSaveParameter.DutyFree, objSaveParameter.TaxAmount, objSaveParameter.Collection, objSaveParameter.BankCode,
                                 objSaveParameter.BankAcctNo, objSaveParameter.ChequeNo, objSaveParameter.PayMethod, objSaveParameter.ReplacementXml, objSaveParameter.DtTaxAmount,
                                 objSaveParameter.LoyaltyCardNo, objSaveParameter.ReScore, objSaveParameter.Tendered, objSaveParameter.GiftVoucherValue,
                                 objSaveParameter.CourtsVoucher, objSaveParameter.VoucherReference, objSaveParameter.VoucherAuthorisedBy,
                                 objSaveParameter.AccountNoCompany, objSaveParameter.PromoBranch, objSaveParameter.PaymentHolidays, objSaveParameter.PayMethodSet, objSaveParameter.DueDay,
                                 objSaveParameter.ReturnAuthorisedBy, objSaveParameter.WarrantyRenewalSet, objSaveParameter.VariableRatesSet,
                                 ref propResult, ref dateProp, ref agreementNo, out bureauFailure, objSaveParameter.User, objSaveParameter.ResetPropResult, objSaveParameter.Autoda, objSaveParameter.StoreCardAcctNo,
                                 objSaveParameter.StoreCardNumber, out storeCardTransRefNo, out referralreasons, objSaveParameter.ReadyAssist, objSaveParameter.ReadyAssistTermLength);


                        Remark = "Error: Payment details are not able to save";
                        err = "Error: Payment details are not able to save";


                        // Create/Add in Payment  
                        BPayment objPayment = new BPayment();

                        PaymentDetails objPaymentDetails = new PaymentDetails();
                        Netpayment = Convert.ToDecimal(objSaveParameter.AgreementTotal);

                        objPaymentDetails = SavePaymentDetailsData(objPaymentDetails);

                        int CommissionRef = 0;
                        int paymentRef = 0;
                        int rebateRef = 0;
                        Decimal rebateSum = 0.0M;

                        int id = 0;
                        objPayment.SavePayment(conn, trans, strAccNo, objPaymentDetails.sundryCredit,
                            objPaymentDetails.paymentMethod, objPaymentDetails.chequeNo, objPaymentDetails.bankCode,
                            objPaymentDetails.bankAcctNo, renissanceCustomer.branchNo, objPaymentDetails.payments,
                            objPaymentDetails.localTender, objPaymentDetails.localChange, out CommissionRef, out paymentRef,
                            out rebateRef, out rebateSum, STL.Common.Static.Credential.UserId,
                            objPaymentDetails.authorisedBy, objPaymentDetails.chequeClearance, objPaymentDetails.receiptNo, renissanceCustomer.countryCode,
                            objPaymentDetails.voucherReference, objPaymentDetails.courtsVoucher, objPaymentDetails.voucherAuthorisedBy,
                            objPaymentDetails.accountNoCompany, objPaymentDetails.returnedChequeAuthorisedBy, objPaymentDetails.agrmtno,
                            objPaymentDetails.storeCardAcctno, objPaymentDetails.storeCardNo, out id);

                        //commit the transaction
                        err = "";
                        Remark = "";

                        trans.Commit();
                    }
                }
                catch (SqlException ex)
                {
                    err = err + " " + ex.StackTrace;
                    LogError(new AshleyErrorLog() { FunctionName = Function, Error = err, Remark = Remark + " " + renissanceCustomer.custid, ErrorDate = DateTime.UtcNow, Id = 0 });
                    return false;
                }
            }
            catch (Exception ex)
            {
                err = ex.StackTrace;
                LogError(new AshleyErrorLog() { FunctionName = Function, Error = err, Remark = "Error in saving basic for customer : " + renissanceCustomer.custid, ErrorDate = DateTime.UtcNow, Id = 0 });
                return false;
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();

                using (var context = Blue.Cosacs.Context.Create())
                {
                    var phones = context.CustTel
                                          .Where(p => p.custid == renissanceCustomer.custid && p.datediscon == null)
                                          .Select(p => p.telno)
                                          .ToArray();

                    if (phones.Any() || !renissanceCustomer.resieveSms)
                    {
                        Chub.SubmitSmsUnsubscriptions(new Blue.Cosacs.Messages.CustomerPhoneNumbers.CustomerPhoneNumbers
                        {
                            CustomerId = renissanceCustomer.custid,
                            PhoneNumbers = phones,
                            Unsubscribe = !renissanceCustomer.resieveSms
                        });
                    }
                }

            }
            return true;
        }

        #region Customer Search 
        public int CustomerSearch(Customer renissanceCustomer,  //CR1084
           int limit,
           int settled,
           bool exactMatch,
           string storeType,
           out string err)
        {
            string Function = "BCustomerManager::CustomerSearch()";
            int isCustExist = 0;
            err = "";
            try
            {
                BCustomer accts = new BCustomer();
                DataTable dtCustomer = accts.CustomerSearch(renissanceCustomer.custid,
                                          renissanceCustomer.firstName,
                                          renissanceCustomer.lastName,
                                          renissanceCustomer.RenissanceCustomerAddressesList.FirstOrDefault().Address1,
                                          renissanceCustomer.RenissanceCustomerAddressesList.FirstOrDefault().PhoneNo,
                                          limit,
                                          settled,
                                          exactMatch,
                                          storeType,String.Empty).Tables[0];
                isCustExist = dtCustomer.Rows.Count;
                if (isCustExist > 0)
                {
                    renissanceCustomer.custid = dtCustomer.Rows[0]["custid"].ToString();
                }
            }
            catch (Exception ex)
            {
                err = ex.StackTrace;
                LogError(new AshleyErrorLog() { FunctionName = Function, Error = err, Remark = "Unable to find customer : " + renissanceCustomer.custid, ErrorDate = DateTime.UtcNow, Id = 0 });
            }
            return isCustExist;
        }
        #endregion

        #endregion

        private SaveNewAccountParameters SaveNewAccountParametersData(SaveNewAccountParameters objSaveNewAccountParameters, List<LineItemList> ItemList)
        {
            SqlConnection conn = null;

            DataTable payMethodList = new DataTable(TN.PayMethodList);
            DataTable warrantyRenewalList = new DataTable(TN.WarrantyList);
            DataTable variableRates = new DataTable(TN.Rates);

            objSaveNewAccountParameters.CODFlag = "Y";
            objSaveNewAccountParameters.SalesPerson = 99999;  // Set Sale Person 
            objSaveNewAccountParameters.SOA = "ADV";    // Set Sale SOA
            //objSaveNewAccountParameters.AgreementTotal = 999; // Calculate AgreementTotal
            objSaveNewAccountParameters.Deposit = 0;
            objSaveNewAccountParameters.ServiceCharge = 0;
            objSaveNewAccountParameters.LineItems = null;
            objSaveNewAccountParameters.TermsType = "00";
            objSaveNewAccountParameters.InstalAmount = 0;
            objSaveNewAccountParameters.FinalInstalment = 0;
            objSaveNewAccountParameters.PaymentMethod = "C";
            objSaveNewAccountParameters.Months = 0;
            objSaveNewAccountParameters.TaxExempt = false;
            objSaveNewAccountParameters.DutyFree = false;
            objSaveNewAccountParameters.TaxAmount = 0;  // Calculate TaxAmount
            objSaveNewAccountParameters.Collection = false;
            objSaveNewAccountParameters.BankCode = "";
            objSaveNewAccountParameters.BankAcctNo = "";
            objSaveNewAccountParameters.ChequeNo = "";
            objSaveNewAccountParameters.PayMethod = 1;
            objSaveNewAccountParameters.ReplacementXml = null;  // Set ReplacementXml 
            objSaveNewAccountParameters.DtTaxAmount = 0.0m;  // Discuss DtTaxAmount
            objSaveNewAccountParameters.LoyaltyCardNo = " ";
            objSaveNewAccountParameters.ReScore = false;
            objSaveNewAccountParameters.Tendered = 0.0m;
            objSaveNewAccountParameters.GiftVoucherValue = 0.0m;
            objSaveNewAccountParameters.CourtsVoucher = false;
            objSaveNewAccountParameters.VoucherReference = " ";
            objSaveNewAccountParameters.VoucherAuthorisedBy = 0;
            objSaveNewAccountParameters.AccountNoCompany = " ";
            objSaveNewAccountParameters.PromoBranch = 0;
            objSaveNewAccountParameters.PaymentHolidays = 0;
            objSaveNewAccountParameters.PayMethodSet = payMethodList;
            objSaveNewAccountParameters.DueDay = 0;
            objSaveNewAccountParameters.ReturnAuthorisedBy = 0;
            objSaveNewAccountParameters.WarrantyRenewalSet = warrantyRenewalList;
            objSaveNewAccountParameters.VariableRatesSet = variableRates;
            objSaveNewAccountParameters.AgreementNo = 1;
            objSaveNewAccountParameters.PropResult = "";
            objSaveNewAccountParameters.DateProp = DateTime.Now; // Discuss Null value 
            objSaveNewAccountParameters.ResetPropResult = false;
            objSaveNewAccountParameters.Autoda = false;
            objSaveNewAccountParameters.User = 99999;       // Set User 
            objSaveNewAccountParameters.StoreCardAcctNo = "";
            objSaveNewAccountParameters.StoreCardNumber = 0;
            objSaveNewAccountParameters.PaidAndTaken = false;
            objSaveNewAccountParameters.HasInstallation = false;
            objSaveNewAccountParameters.CustLinkRequired = false;
            objSaveNewAccountParameters.CashAndGoReturn = false;
            objSaveNewAccountParameters.CollectionType = null;
            //objSaveNewAccountParameters.CountryCode = "B";   // Add/Set Country Code 
            objSaveNewAccountParameters.NewBand = "A";       // Discuss 
            objSaveNewAccountParameters.ReadyAssist = true;  // Check 
            objSaveNewAccountParameters.ReadyAssistTermLength = 0;
            // Create XMLNode 
            conn = new SqlConnection(Connections.Default);
            conn.Open();
            using (SqlTransaction trans = conn.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
            {

                String strAccNo = AcctNo.Replace("-", "");
                BItem objBItem = new BItem();

                XmlDocument xmlDoc = new XmlDocument();
                XmlNode ItemNode = null;
                XmlNode ItemDetails = null;
                ItemDetails = xmlDoc.CreateElement("ITEMS");

                foreach (var Lineitemlst in ItemList)
                {

                    ItemNode = objBItem.GetItemDetails(conn, trans, Convert.ToInt32(Lineitemlst.ItemNo), objSaveNewAccountParameters.BranchNo, objSaveNewAccountParameters.AccountType, objSaveNewAccountParameters.CountryCode, false, false);

                    // Update Item Details in XmlNode  

                    // Update Type  
                    if (ItemNode.Attributes[Tags.Type].Value == "0" || ItemNode.Attributes[Tags.Type].Value == "")
                    {
                        ItemNode.Attributes[Tags.Type].Value = "Stock";
                    }

                    // Update AvailableStock  
                    if (ItemNode.Attributes[Tags.AvailableStock].Value == "0" || ItemNode.Attributes[Tags.AvailableStock].Value == "")
                    {
                        ItemNode.Attributes[Tags.AvailableStock].Value = "0.00";
                    }

                    // Update DamagedStock  
                    if (ItemNode.Attributes[Tags.DamagedStock].Value == "0" || ItemNode.Attributes[Tags.DamagedStock].Value == "")
                    {
                        ItemNode.Attributes[Tags.DamagedStock].Value = "0.00";
                    }

                    // Update SupplierCode  
                    if (ItemNode.Attributes[Tags.SupplierCode].Value == "0" || ItemNode.Attributes[Tags.SupplierCode].Value == "")
                    {
                        ItemNode.Attributes[Tags.SupplierCode].Value = "0";
                    }

                    // Update UnitPrice  
                    if (ItemNode.Attributes[Tags.UnitPrice].Value == "0" || ItemNode.Attributes[Tags.UnitPrice].Value == "")
                    {
                        ItemNode.Attributes[Tags.UnitPrice].Value = "0.00";
                    }

                    // Update CashPrice  
                    if (ItemNode.Attributes[Tags.CashPrice].Value == "0" || ItemNode.Attributes[Tags.CashPrice].Value == "")
                    {
                        ItemNode.Attributes[Tags.CashPrice].Value = "0.00";
                    }
                    // Update CostPrice  
                    if (ItemNode.Attributes[Tags.CostPrice].Value == "0" || ItemNode.Attributes[Tags.CostPrice].Value == "")
                    {
                        ItemNode.Attributes[Tags.CostPrice].Value = "0.00";
                    }
                    // Update HPPrice  Done
                    if (ItemNode.Attributes[Tags.HPPrice].Value == "0" || ItemNode.Attributes[Tags.HPPrice].Value == "")
                    {
                        ItemNode.Attributes[Tags.HPPrice].Value = "0.00";
                    }
                    // Update DutyFreePrice  Done
                    if (ItemNode.Attributes[Tags.DutyFreePrice].Value == "0" || ItemNode.Attributes[Tags.DutyFreePrice].Value == "")
                    {
                        ItemNode.Attributes[Tags.DutyFreePrice].Value = "0.00";
                    }

                    // Update ValueControlled  Done
                    if (ItemNode.Attributes[Tags.ValueControlled].Value == "0" || ItemNode.Attributes[Tags.ValueControlled].Value == "")
                    {
                        ItemNode.Attributes[Tags.ValueControlled].Value = "False";
                    }

                    // Update Quantity Done
                    //if (ItemNode.Attributes[Tags.Quantity].Value == "0" || ItemNode.Attributes[Tags.Quantity].Value == "")
                    //{
                    ItemNode.Attributes[Tags.Quantity].Value = Convert.ToString(Lineitemlst.SaleQty);
                    //}

                    // Update Value 
                    //if (ItemNode.Attributes[Tags.Value].Value == "0" || ItemNode.Attributes[Tags.Value].Value == "")
                    //{
                    ItemNode.Attributes[Tags.Value].Value = Convert.ToString(Lineitemlst.SaleQty * Convert.ToDouble(ItemNode.Attributes[Tags.UnitPrice].Value));
                    //}

                    // Update DeliveryDate  Done
                    //if (ItemNode.Attributes[Tags.DeliveryDate].Value == "0" || ItemNode.Attributes[Tags.DeliveryDate].Value == "")
                    //{
                    ItemNode.Attributes[Tags.DeliveryDate].Value = Lineitemlst.DelivaryDate;
                    // }

                    // Update DeliveryTime Done
                    if (ItemNode.Attributes[Tags.DeliveryTime].Value == "0" || ItemNode.Attributes[Tags.DeliveryTime].Value == "")
                    {
                        ItemNode.Attributes[Tags.DeliveryTime].Value = "AM";
                    }

                    // Update BranchForDeliveryNote Discuss 
                    if (ItemNode.Attributes[Tags.BranchForDeliveryNote].Value == "0" || ItemNode.Attributes[Tags.BranchForDeliveryNote].Value == "")
                    {
                        ItemNode.Attributes[Tags.BranchForDeliveryNote].Value = Convert.ToString(objSaveNewAccountParameters.BranchNo);
                    }

                    // Update ColourTrim Set Default 
                    if (ItemNode.Attributes[Tags.ColourTrim].Value == "0" || ItemNode.Attributes[Tags.ColourTrim].Value == "")
                    {
                        ItemNode.Attributes[Tags.ColourTrim].Value = "0.00";
                    }

                    // Update TaxRate
                    if (ItemNode.Attributes[Tags.TaxRate].Value == "0" || ItemNode.Attributes[Tags.TaxRate].Value == "")
                    {
                        ItemNode.Attributes[Tags.TaxRate].Value = "0.00";
                    }

                    // Update DeliveredQuantity
                    if (ItemNode.Attributes[Tags.DeliveredQuantity].Value == "0" || ItemNode.Attributes[Tags.DeliveredQuantity].Value == "")
                    {
                        ItemNode.Attributes[Tags.DeliveredQuantity].Value = Convert.ToString(Lineitemlst.SaleQty);
                    }
                    // Update PlannedDeliveryDate
                    if (ItemNode.Attributes[Tags.PlannedDeliveryDate].Value == "0" || ItemNode.Attributes[Tags.PlannedDeliveryDate].Value == "")
                    {
                        ItemNode.Attributes[Tags.PlannedDeliveryDate].Value = Lineitemlst.DelivaryDate;
                    }

                    // Update CanAddWarranty
                    if (ItemNode.Attributes[Tags.CanAddWarranty].Value == "0" || ItemNode.Attributes[Tags.CanAddWarranty].Value == "")
                    {
                        ItemNode.Attributes[Tags.CanAddWarranty].Value = "True";
                    }

                    // Update DeliveryAddress
                    if (ItemNode.Attributes[Tags.DeliveryAddress].Value == "0" || ItemNode.Attributes[Tags.DeliveryAddress].Value == "")
                    {
                        ItemNode.Attributes[Tags.DeliveryAddress].Value = "H";
                    }

                    // Update DeliveryArea
                    if (ItemNode.Attributes[Tags.DeliveryArea].Value == "0" || ItemNode.Attributes[Tags.DeliveryArea].Value == "")
                    {
                        ItemNode.Attributes[Tags.DeliveryArea].Value = "H";
                    }

                    // Update DeliveryProcess
                    if (ItemNode.Attributes[Tags.DeliveryProcess].Value == "0" || ItemNode.Attributes[Tags.DeliveryProcess].Value == "")
                    {
                        ItemNode.Attributes[Tags.DeliveryProcess].Value = "I";
                    }

                    // Update DateDelivered Discuss 
                    if (ItemNode.Attributes[Tags.DateDelivered].Value == "0" || ItemNode.Attributes[Tags.DateDelivered].Value == "")
                    {
                        ItemNode.Attributes[Tags.DateDelivered].Value = Lineitemlst.DelivaryDate;
                    }

                    // Update QuantityDiff
                    if (ItemNode.Attributes[Tags.QuantityDiff].Value == "0" || ItemNode.Attributes[Tags.QuantityDiff].Value == "")
                    {
                        ItemNode.Attributes[Tags.QuantityDiff].Value = "Y";
                    }

                    // Update ScheduledQuantity
                    if (ItemNode.Attributes[Tags.ScheduledQuantity].Value == "0" || ItemNode.Attributes[Tags.ScheduledQuantity].Value == "")
                    {
                        ItemNode.Attributes[Tags.ScheduledQuantity].Value = Convert.ToString(Lineitemlst.SaleQty);
                    }

                    // Update TaxAmount
                    if (ItemNode.Attributes[Tags.TaxAmount].Value == "0" || ItemNode.Attributes[Tags.TaxAmount].Value == "")
                    {
                        ItemNode.Attributes[Tags.TaxAmount].Value = "0.00";
                    }

                    // Update ContractNumber
                    if (ItemNode.Attributes[Tags.ContractNumber].Value == "0" || ItemNode.Attributes[Tags.ContractNumber].Value == "")
                    {
                        ItemNode.Attributes[Tags.ContractNumber].Value = "";
                    }

                    // Update ReturnItemNo
                    if (ItemNode.Attributes[Tags.ReturnItemNo].Value == "0" || ItemNode.Attributes[Tags.ReturnItemNo].Value == "")
                    {
                        ItemNode.Attributes[Tags.ReturnItemNo].Value = ItemNode.Attributes[Tags.Code].Value;
                    }

                    // Update ReturnLocation
                    if (ItemNode.Attributes[Tags.ReturnLocation].Value == "0" || ItemNode.Attributes[Tags.ReturnLocation].Value == "")
                    {
                        ItemNode.Attributes[Tags.ReturnLocation].Value = ItemNode.Attributes[Tags.SalesBrnNo].Value;
                    }

                    // Update FreeGift
                    if (ItemNode.Attributes[Tags.FreeGift].Value == "0" || ItemNode.Attributes[Tags.FreeGift].Value == "")
                    {
                        ItemNode.Attributes[Tags.FreeGift].Value = "False";
                    }

                    // Update ExpectedReturnDate
                    if (ItemNode.Attributes[Tags.ExpectedReturnDate].Value == "0" || ItemNode.Attributes[Tags.ExpectedReturnDate].Value == "")
                    {
                        ItemNode.Attributes[Tags.ExpectedReturnDate].Value = Lineitemlst.DelivaryDate;
                    }

                    // Update QtyOnOrder
                    if (ItemNode.Attributes[Tags.QtyOnOrder].Value == "0" || ItemNode.Attributes[Tags.QtyOnOrder].Value == "")
                    {
                        ItemNode.Attributes[Tags.QtyOnOrder].Value = "0";
                    }

                    // Update PurchaseOrder
                    if (ItemNode.Attributes[Tags.PurchaseOrder].Value == "0" || ItemNode.Attributes[Tags.PurchaseOrder].Value == "")
                    {
                        ItemNode.Attributes[Tags.PurchaseOrder].Value = "False";
                    }

                    // Update LeadTime
                    if (ItemNode.Attributes[Tags.LeadTime].Value == "0" || ItemNode.Attributes[Tags.LeadTime].Value == "")
                    {
                        ItemNode.Attributes[Tags.LeadTime].Value = "0";
                    }

                    // Update Assembly
                    if (ItemNode.Attributes[Tags.Assembly].Value == "0" || ItemNode.Attributes[Tags.Assembly].Value == "")
                    {
                        ItemNode.Attributes[Tags.Assembly].Value = "N";
                    }

                    // Update Damaged
                    if (ItemNode.Attributes[Tags.Damaged].Value == "0" || ItemNode.Attributes[Tags.Damaged].Value == "")
                    {
                        ItemNode.Attributes[Tags.Damaged].Value = "N";
                    }

                    // Update ProductCategory
                    if (ItemNode.Attributes[Tags.ProductCategory].Value == "0" || ItemNode.Attributes[Tags.ProductCategory].Value == "")
                    {
                        ItemNode.Attributes[Tags.ProductCategory].Value = "H";
                    }

                    // Update SparePartsCategory
                    if (ItemNode.Attributes[Tags.SparePartsCategory].Value == "0" || ItemNode.Attributes[Tags.SparePartsCategory].Value == "")
                    {
                        ItemNode.Attributes[Tags.SparePartsCategory].Value = "H";
                    }

                    // Update Deleted
                    if (ItemNode.Attributes[Tags.Deleted].Value == "0" || ItemNode.Attributes[Tags.Deleted].Value == "")
                    {
                        ItemNode.Attributes[Tags.Deleted].Value = "Y";
                    }

                    // Update PurchaseOrderNumber
                    if (ItemNode.Attributes[Tags.PurchaseOrderNumber].Value == "0" || ItemNode.Attributes[Tags.PurchaseOrderNumber].Value == "")
                    {
                        ItemNode.Attributes[Tags.PurchaseOrderNumber].Value = "";
                    }

                    // Update ReplacementItem
                    if (ItemNode.Attributes[Tags.ReplacementItem].Value == "0" || ItemNode.Attributes[Tags.ReplacementItem].Value == "")
                    {
                        ItemNode.Attributes[Tags.ReplacementItem].Value = "False";
                    }

                    // Update SPIFFItem
                    if (ItemNode.Attributes[Tags.SPIFFItem].Value == "0" || ItemNode.Attributes[Tags.SPIFFItem].Value == "")
                    {
                        ItemNode.Attributes[Tags.SPIFFItem].Value = "False";
                    }

                    // Update RefCode
                    if (ItemNode.Attributes[Tags.RefCode].Value == "0" || ItemNode.Attributes[Tags.RefCode].Value == "")
                    {
                        ItemNode.Attributes[Tags.RefCode].Value = "00";
                    }

                    // Update ItemRejected
                    if (ItemNode.Attributes[Tags.ItemRejected].Value == "0" || ItemNode.Attributes[Tags.ItemRejected].Value == "")
                    {
                        ItemNode.Attributes[Tags.ItemRejected].Value = "False";
                    }

                    // Update Category
                    if (ItemNode.Attributes[Tags.Category].Value == "0" || ItemNode.Attributes[Tags.Category].Value == "")
                    {
                        ItemNode.Attributes[Tags.Category].Value = "H";
                    }

                    // Update ItemId
                    //if (ItemNode.Attributes[Tags.ItemId].Value == "0" || ItemNode.Attributes[Tags.ItemId].Value == "")
                    //{
                    //    ItemNode.Attributes[Tags.ItemId].Value = "H";
                    //}

                    // Update ColourName
                    if (ItemNode.Attributes[Tags.ColourName].Value == "0" || ItemNode.Attributes[Tags.ColourName].Value == "")
                    {
                        ItemNode.Attributes[Tags.ColourName].Value = "";
                    }
                    // Update Style
                    if (ItemNode.Attributes[Tags.Style].Value == "0" || ItemNode.Attributes[Tags.Style].Value == "")
                    {
                        ItemNode.Attributes[Tags.Style].Value = "";
                    }

                    // Update SalesBrnNo
                    if (ItemNode.Attributes[Tags.SalesBrnNo].Value == "0" || ItemNode.Attributes[Tags.SalesBrnNo].Value == "")
                    {
                        ItemNode.Attributes[Tags.SalesBrnNo].Value = Convert.ToString(objSaveNewAccountParameters.BranchNo);
                    }

                    // Update RepoItem
                    if (ItemNode.Attributes[Tags.RepoItem].Value == "0" || ItemNode.Attributes[Tags.RepoItem].Value == "")
                    {
                        ItemNode.Attributes[Tags.RepoItem].Value = "False";
                    }

                    // Update Class
                    if (ItemNode.Attributes[Tags.Class].Value == "0" || ItemNode.Attributes[Tags.Class].Value == "")
                    {
                        ItemNode.Attributes[Tags.Class].Value = "";
                    }

                    // Update SubClass
                    if (ItemNode.Attributes[Tags.SubClass].Value == "0" || ItemNode.Attributes[Tags.SubClass].Value == "")
                    {
                        ItemNode.Attributes[Tags.SubClass].Value = "";
                    }

                    // Update Brand
                    if (ItemNode.Attributes[Tags.Brand].Value == "0" || ItemNode.Attributes[Tags.Brand].Value == "")
                    {
                        ItemNode.Attributes[Tags.Brand].Value = "";
                    }

                    // Update Express
                    if (ItemNode.Attributes[Tags.Express].Value == "0" || ItemNode.Attributes[Tags.Express].Value == "")
                    {
                        ItemNode.Attributes[Tags.Express].Value = "";
                    }

                    // Update WarrantyType
                    if (ItemNode.Attributes[Tags.WarrantyType].Value == "0" || ItemNode.Attributes[Tags.WarrantyType].Value == "")
                    {
                        ItemNode.Attributes[Tags.WarrantyType].Value = "";
                    }

                    // Update ReadyAssist
                    if (ItemNode.Attributes[Tags.ReadyAssist].Value == "0" || ItemNode.Attributes[Tags.ReadyAssist].Value == "")
                    {
                        ItemNode.Attributes[Tags.ReadyAssist].Value = "";
                    }

                    if (ItemNode != null)
                    {
                        ItemNode = xmlDoc.ImportNode(ItemNode, true);
                        ItemDetails.AppendChild(ItemNode);
                    }
                }
                objSaveNewAccountParameters.AgreementTotal += Convert.ToDouble(ItemNode.Attributes[Tags.Value].Value);
                objSaveNewAccountParameters.ItemDetails = ItemDetails;

            }

            // ********

            #region OLD XMLNODE CODE 
            // Below Running Coode 

            /*
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement root = xmlDoc.CreateElement("ITEMS");
            xmlDoc.AppendChild(root);

            XmlElement child = xmlDoc.CreateElement("Item");

            child.SetAttribute("Key", "62364|900");
            child.SetAttribute("Type", "Stock");
            child.SetAttribute("Code", "R42014");
            child.SetAttribute("Location", "900");
            child.SetAttribute("AvailableStock", "-1.00");
            child.SetAttribute("DamagedStock", "0");
            child.SetAttribute("Description1", "RADIO SHACK 4203204 3.5MM MALE");
            child.SetAttribute("Description2", "TO MALE CABLE 6FT");
            child.SetAttribute("SupplierCode", "4203204");
            child.SetAttribute("UnitPrice", "11.06");
            child.SetAttribute("CashPrice", "11.06");
            child.SetAttribute("CostPrice", "3.21");
            child.SetAttribute("HPPrice", "11.06");
            child.SetAttribute("DutyFreePrice", "11.06");
            child.SetAttribute("ValueControlled", "False");
            child.SetAttribute("Quantity", "1");
            child.SetAttribute("Value", "11.06");
            child.SetAttribute("DeliveryDate", "18/04/2019 12:00:00 AM");
            child.SetAttribute("DeliveryTime", "AM");
            child.SetAttribute("BranchForDeliveryNote", "900");
            child.SetAttribute("ColourTrim", "Special Order: User: System Administrator (99999). Item not in stock, not on order");
            child.SetAttribute("TaxRate", "17.5");
            child.SetAttribute("DeliveredQuantity", "0");
            child.SetAttribute("PlannedDeliveryDate", "");
            child.SetAttribute("CanAddWarranty", "True");
            child.SetAttribute("DeliveryAddress", "H");
            child.SetAttribute("DeliveryArea", "CH CH");
            child.SetAttribute("DeliveryProcess", "S");
            child.SetAttribute("DateDelivered", "");
            child.SetAttribute("QuantityDiff", "Y");
            child.SetAttribute("ScheduledQuantity", "0");
            child.SetAttribute("TaxAmount", "0");
            child.SetAttribute("ContractNumber", "");
            child.SetAttribute("ReturnItemNo", "R42014");
            child.SetAttribute("ReturnLocation", "900");
            child.SetAttribute("FreeGift", "false");
            child.SetAttribute("ExpectedReturnDate", "");
            child.SetAttribute("QtyOnOrder", "N");
            child.SetAttribute("PurchaseOrder", "False");
            child.SetAttribute("LeadTime", "0");
            child.SetAttribute("Assembly", "N");
            child.SetAttribute("Damaged", "N");
            child.SetAttribute("ProductCategory", "PCE");
            child.SetAttribute("SparePartsCategory", "");
            child.SetAttribute("Deleted", "N");
            child.SetAttribute("PurchaseOrderNumber", "");
            child.SetAttribute("ReplacementItem", "False");
            child.SetAttribute("SPIFFItem", "False");
            child.SetAttribute("RefCode", "00");
            child.SetAttribute("ItemRejected", "False");
            child.SetAttribute("Category", "10");
            child.SetAttribute("ItemId", "62364");
            child.SetAttribute("ColourName", "");
            child.SetAttribute("Style", "");
            child.SetAttribute("SalesBrnNo", "900");
            child.SetAttribute("RepoItem", "False");
            child.SetAttribute("Class", "R42");
            child.SetAttribute("SubClass", "");
            child.SetAttribute("Brand", "");
            child.SetAttribute("Express", "N");
            child.SetAttribute("WarrantyType", "");
            child.SetAttribute("ReadyAssist", "False");
            child.SetAttribute("xmlns", "");

            root.AppendChild(child);
            XmlNode node2 = xmlDoc.ImportNode(xmlDoc.DocumentElement, true); */
            #endregion

            return objSaveNewAccountParameters;
        }
        private PaymentDetails SavePaymentDetailsData(PaymentDetails objPaymentDetails)
        {
            objPaymentDetails.accountNo = "";
            objPaymentDetails.sundryCredit = false;
            objPaymentDetails.paymentMethod = 1;
            objPaymentDetails.chequeNo = "";
            objPaymentDetails.bankCode = "";
            objPaymentDetails.bankAcctNo = "";
            objPaymentDetails.branchNo = 0;
            objPaymentDetails.payments = null;
            objPaymentDetails.localTender = 0.0m;
            objPaymentDetails.localChange = 0.0m;
            objPaymentDetails.commissionRef = 0;
            objPaymentDetails.paymentRef = 0;
            objPaymentDetails.rebateRef = 0;
            objPaymentDetails.rebateSum = 0;
            //objPaymentDetails.UserId ="";
            objPaymentDetails.authorisedBy = 0;
            objPaymentDetails.chequeClearance = DateTime.Now;
            objPaymentDetails.receiptNo = 0;
            objPaymentDetails.countryCode = "";
            objPaymentDetails.voucherReference = "";
            objPaymentDetails.courtsVoucher = true;
            objPaymentDetails.voucherAuthorisedBy = 0;
            objPaymentDetails.accountNoCompany = "";
            objPaymentDetails.returnedChequeAuthorisedBy = 0;
            objPaymentDetails.agrmtno = 1;
            objPaymentDetails.storeCardAcctno = "";
            objPaymentDetails.storeCardNo = 0;

            DataSet accountSet = new DataSet();
            accountSet.Tables.Add(TN.Payments);
            accountSet.Tables[TN.Payments].Columns.AddRange(
            new DataColumn[] {
                new DataColumn(CN.DateAcctOpen, Type.GetType("System.DateTime")),  // If Fitst create Now Date 
                new DataColumn(CN.RatioPay, Type.GetType("System.Decimal")),
                new DataColumn(CN.LockedBy),
                new DataColumn(CN.AccountNo),
                new DataColumn(CN.OutstandingBalance, Type.GetType("System.Decimal")),
                new DataColumn(CN.Arrears, Type.GetType("System.Decimal")),
                new DataColumn(CN.Rebate, Type.GetType("System.Decimal")),
                new DataColumn(CN.SettlementFigure, Type.GetType("System.Decimal")),
                new DataColumn(CN.InstalAmount, Type.GetType("System.Decimal")),
                new DataColumn(CN.NetPayment, Type.GetType("System.Decimal")),
                new DataColumn(CN.CollectionFee, Type.GetType("System.Decimal")),
                new DataColumn(CN.BailiffFee, Type.GetType("System.Decimal")),
                new DataColumn(CN.Payment, Type.GetType("System.Decimal")),
                new DataColumn(CN.CalculatedFee, Type.GetType("System.Decimal")),
                new DataColumn(CN.EmployeeNo, Type.GetType("System.Int32")),
                new DataColumn(CN.SegmentID, Type.GetType("System.Int32")),
                new DataColumn(CN.Status),
                new DataColumn(CN.ReadOnly),
                new DataColumn(CN.DebitAccount, Type.GetType("System.Int16")),
                new DataColumn(CN.BDWBalance, Type.GetType("System.Decimal")),
                new DataColumn(CN.BDWCharges, Type.GetType("System.Decimal"))
            });

            //DataRow accountRow=null;

            //accountSet.Tables[TN.Payments].Rows.Add(new Object[] {
            //    accountRow[CN.DateAcctOpen],
            //    accountRow[CN.RatioPay],
            //    accountRow[CN.LockedBy],
            //    accountRow[CN.acctno],
            //    accountRow[CN.OutstandingBalance],
            //    accountRow[CN.Arrears],
            //    accountRow[CN.Rebate],
            //    accountRow[CN.SettlementFigure],
            //    accountRow[CN.InstalAmount],
            //    curPayment,	            // Net Payment (can be changed by user when no fee)
            // 0,	            // Collection Fee (can be changed by user when a fee)
            // 0,            // Bailiff Fee (not visible to the user)
            // curPayment,    // Total Payment (can be changed by user when a fee)
            //    0,            // Calculated Collection Fee (not visible to the user)
            //    accountRow[CN.EmployeeNo],
            //    accountRow[CN.SegmentID],
            //    accountRow[CN.Status],
            //    accountRow[CN.ReadOnly],
            //    accountRow[CN.DebitAccount],
            //    accountRow[CN.BDWBalance],
            //    accountRow[CN.BDWCharges]
            //});

            String strAccNo = AcctNo.Replace("-", "");
            accountSet.Tables[TN.Payments].Rows.Add(new Object[] {
             DateTime.Now,
              0.0m,
              "", //Add Locked By User Name 
              strAccNo,
              0.00m,
              0.00m,
              0.00m,
              0.00m,
              0.00m,
              Netpayment, // Net Payment (can be changed by user when no fee) current 
              0.00, // Collection Fee (can be changed by user when a fee)
              0.00, // Bailiff Fee (not visible to the user)
              Netpayment, // Total Payment (can be changed by user when a fee)
              0.00, // Calculated Collection Fee (not visible to the user)
              0,
              0,
              "Y",
              "",
              0,
              0.00m,
              0.00m
            });

            objPaymentDetails.payments = accountSet;

            return objPaymentDetails;
        }

        #region Log Error Function
        /// <summary>
        /// Logs the Error the if any occured in repository
        /// </summary>
        /// <param name="err">Object of AshleyErrorLog</param>
        void LogError(AshleyErrorLog err)
        {
            using (var scope = Context.Write())
            {
                scope.Context.AshleyErrorLogs.Add(err);
                scope.Context.SaveChanges();
                scope.Complete();
            }

        }
        #endregion


        #region Moved from Blue.Cosacs.Service
        public string GetRenissanceSaleData()
        {
            RenissanceSales objRenissanceSales = new RenissanceSales();
            return objRenissanceSales.GetRenissanceSaleData();
        }
        public void ReceiveRenissanceSaleDataFlag()
        {
            RenissanceSales objRenissanceSales = new RenissanceSales();
            objRenissanceSales.ReceiveRenissanceSaleData();
        }
        #endregion

    }
}
