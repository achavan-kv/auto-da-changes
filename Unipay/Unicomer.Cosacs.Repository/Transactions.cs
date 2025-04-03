using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Unicomer.Cosacs.Model;

namespace Unicomer.Cosacs.Repository
{
    public class Transactions
    {
    }

    #region Credit Transaction 
    public partial class CreditTransaction : Blue.Transactions.Command<ContextBase>
    {
        public CreditTransaction() : base("dbo.GetCreditAccount_ConsolidatedDetails")
        {
            base.AddInParameter("@CustId", DbType.String);
            base.AddOutParameter("@Message", DbType.String, 5000);
        }
    }

    partial class CreditTransaction
    {
        public string GetUserAccounts(DataSet ds)
        {
            base.Fill(ds);
            return this.Message;
        }

        public string GetUserAccounts(DataSet ds, string _CustID)
        {
            this.CustId = _CustID;
            this.Message = string.Empty;
            return GetUserAccounts(ds);
        }

        #region "properties"
        public string CustId
        {
            get { return (string)base[0]; }
            set { base[0] = value; }
        }
        public string Message
        {
            get { return (string)base[1]; }
            set { base[1] = value; }
        }
        #endregion

    }
    #endregion

    #region Credit App Questions Transaction 
    public partial class CreditAppQuestionsTransaction : Blue.Transactions.Command<ContextBase>
    {
        public CreditAppQuestionsTransaction() : base("dbo.GetCreditAppQuestions")
        {
            base.AddInParameter("@CustId", DbType.String);
            base.AddOutParameter("@Message", DbType.String, 5000);
        }
    }

    partial class CreditAppQuestionsTransaction
    {
        public string GetCreditAppQuestions(DataSet ds)
        {
            base.Fill(ds);
            return this.Message;
        }

        public string GetCreditAppQuestions(DataSet ds, string _CustID)
        {
            this.CustId = _CustID;
            this.Message = string.Empty;
            return GetCreditAppQuestions(ds);
        }

        #region "properties"
        public string CustId
        {
            get { return (string)base[0]; }
            set { base[0] = value; }
        }
        public string Message
        {
            get { return (string)base[1]; }
            set { base[1] = value; }
        }
        #endregion

    }
    #endregion 

    #region GetContractRepository
    public partial class GetContractRepository : Blue.Transactions.Command<ContextBase>
    {
        public GetContractRepository() : base("dbo.GetCustomerContractDetails")
        {

            base.AddInParameter("@CustId", DbType.String);
            //base.AddInParameter("@ExtCustId", DbType.String);
            base.AddInParameter("@AccountNumber", DbType.String);
            base.AddInParameter("@DayOfMonth", DbType.Int32);
            base.AddInParameter("@Frequency", DbType.String);
            base.AddOutParameter("@Message", DbType.String, 5000);
        }
    }

    partial class GetContractRepository
    {
        public string GetCustomerContractDetails(DataSet ds)
        {
            base.Fill(ds);
            return this.Message;
        }

        public string GetCustomerContractDetails(DataSet ds, GetContract objGetContract)
        {
            this.CustId = objGetContract.CustId;
            this.AccountNumber = objGetContract.accountNumber;
            this.DayOfMonth = objGetContract.dayOfMonth;
            this.Frequency = objGetContract.frecuency != null ? objGetContract.frecuency.ToString() : string.Empty;
            this.Message = string.Empty;
            return GetCustomerContractDetails(ds);
        }

        #region "properties"

        public string CustId
        {
            get { return (string)base[0]; }
            set { base[0] = value; }
        }
        public string AccountNumber
        {
            get { return (string)base[1]; }
            set { base[1] = value; }
        }
        public int DayOfMonth
        {
            get { return (int)base[2]; }
            set { base[2] = value; }
        }
        public string Frequency
        {
            get { return (string)base[3]; }
            set { base[3] = value; }
        }
        public string Message
        {
            get { return (string)base[4]; }
            set { base[4] = value; }
        }

        #endregion

    }
    #endregion

    #region Customer Credit Docuemnts Save Transaction 
    public partial class CustCreditDocuemntsSaveTransaction : Blue.Transactions.Command<ContextBase>
    {
        public CustCreditDocuemntsSaveTransaction() : base("dbo.CustCreditDocumentsSave")
        {
            base.AddInParameter("@CustId", DbType.String);
            base.AddInParameter("@FolderPath", DbType.String);
            base.AddInParameter("@FileName", DbType.String);
            base.AddInParameter("@AccountNumber", DbType.String);
            base.AddInParameter("@IsThirdParty", DbType.Boolean);
            base.AddOutParameter("@Message", DbType.String, 5000);
        }
    }

    partial class CustCreditDocuemntsSaveTransaction
    {
        public string CustCreditDocuemntsSave()
        {
            base.ExecuteNonQuery();
            return this.Message;
        }

        public string CustCreditDocuemntsSave(CustCreditDocument custCreditDocument)
        {
            this.CustId = custCreditDocument.CustId;
            this.FolderPath = custCreditDocument.FolderPath;
            this.FileName = custCreditDocument.FileName;
            this.AccountNumber = custCreditDocument.AccountNumber;
            this.IsThirdParty = custCreditDocument.IsThirdParty;
            this.Message = custCreditDocument.Message;
            //this.CreatedOn = custCreditDocument.CreatedOn;
            return CustCreditDocuemntsSave();
        }

        #region "properties"
        public string CustId
        {
            get { return (string)base[0]; }
            set { base[0] = value; }
        }
        public string FolderPath
        {
            get { return (string)base[1]; }
            set { base[1] = value; }
        }
        public string FileName
        {
            get { return (string)base[2]; }
            set { base[2] = value; }
        }
        public string AccountNumber
        {
            get { return (string)base[3]; }
            set { base[3] = value; }
        }
        public bool IsThirdParty
        {
            get { return (bool)base[4]; }
            set { base[4] = value; }
        }
        public string Message
        {
            get { return (string)base[5]; }
            set { base[5] = value; }
        }

        #endregion
    }
    #endregion

    #region Get country Maintenance
    public partial class CountryMaintenance : Blue.Transactions.Command<ContextBase>
    {
        public CountryMaintenance() : base("dbo.GetCountryMaintenance")
        {
            base.AddInParameter("@CustId", DbType.String);
            base.AddOutParameter("@Message", DbType.String, 5000);
            base.AddOutParameter("@Status", DbType.String, 5);
        }
    }

    partial class CountryMaintenance
    {

        public List<string> Fill(DataSet ds)
        {
            List<string> returnCustId = new List<string>();// string.Empty;
            base.Fill(ds);
            returnCustId.Add(this.Message);
            returnCustId.Add(this.Status.ToString());
            return returnCustId;
        }
        public List<string> CountryMaintenanceDetails(string custId, DataSet dt)
        {
            this.CustId = custId;
            this.Message = string.Empty;
            this.Status = string.Empty;
            return Fill(dt);
        }
        #region "properties"
        public string CustId
        {
            get { return (string)base[0]; }
            set { base[0] = value; }
        }
        public string Message
        {
            get { return (string)base[1]; }
            set { base[1] = value; }
        }
        public string Status
        {
            get { return (string)base[2]; }
            set { base[2] = value; }
        }
        #endregion
    }
    #endregion

    #region "Update document confirmation flag"

    public partial class SaveDocConfirmation : Blue.Transactions.Command<ContextBase>
    {
        public SaveDocConfirmation() : base("dbo.ProposalFlagSaveSP")
        {
            base.AddInParameter("@origbr", DbType.Int16);
            base.AddInParameter("@custid", DbType.String);
            base.AddInParameter("@dateprop", DbType.DateTime);
            base.AddInParameter("@checktype", DbType.String);
            base.AddInParameter("@datecleared", DbType.DateTime);
            base.AddInParameter("@empeenopflg", DbType.Int32);
            base.AddInParameter("@acctno", DbType.String);
            base.AddOutParameter("@returnFlag", DbType.Int32, 32);



        }
    }

    partial class SaveDocConfirmation
    {
        public int updateDocFlag()
        {
            int RecordCount = base.ExecuteNonQuery();
            return RecordCount;
        }

        public int updateDocFlag(Int16 _origbr,
                                    string _custid,
                                    DateTime _dateprop,
                                    string _checktype,
                                    DateTime _datecleared,
                                    Int32 _empeenopflg,
                                    string _acctno

            )
        {
            this.origbr = _origbr;
            this.custid = _custid;
            this.dateprop = _dateprop;
            this.checktype = _checktype;
            this.datecleared = _datecleared;
            this.empeenopflg = _empeenopflg;
            this.acctno = _acctno;
            this.returnFlag = 0;
            int ReturnCustId = updateDocFlag();
            return ReturnCustId;

        }

        #region "properties"
        public Int16 origbr
        {
            get { return (Int16)base[0]; }
            set { base[0] = value; }
        }
        public string custid
        {
            get { return (string)base[1]; }
            set { base[1] = value; }
        }
        public DateTime dateprop
        {
            get { return (DateTime)base[2]; }
            set { base[2] = value; }
        }
        public string checktype
        {
            get { return (string)base[3]; }
            set { base[3] = value; }
        }
        public DateTime datecleared
        {
            get { return (DateTime)base[4]; }
            set { base[4] = value; }
        }
        public Int32 empeenopflg
        {
            get { return (Int32)base[5]; }
            set { base[5] = value; }
        }
        public string acctno
        {
            get { return (string)base[6]; }
            set { base[6] = value; }
        }
        public Int32 returnFlag
        {
            get { return (Int32)base[7]; }
            set { base[7] = value; }
        }

        #endregion

    }


    public partial class UpdateCustAddress : Blue.Transactions.Command<ContextBase>
    {
        public UpdateCustAddress() : base("dbo.UpdateCustAddress")
        {
            base.AddInParameter("@custId", DbType.String);
            base.AddInParameter("@addressType", DbType.String);
            base.AddInParameter("@address", DbType.String);
            base.AddInParameter("@resStatus", DbType.String);
            base.AddInParameter("@workAddress", DbType.String);
            base.AddInParameter("@workPhone", DbType.String);
        }
    }

    partial class UpdateCustAddress
    {
        public int SaveCustAddress()
        {
            int recordCount = base.ExecuteNonQuery();
            return recordCount;
        }

        public int SaveCustAddress(string custId, string addressType, string address, string @resStatus, string workAddress, string workPhone)
        {
            this.CustId = custId;
            this.AddressType = addressType;
            this.Address = address;
            this.ResStatus = resStatus;
            this.WorkAddress = workAddress;
            this.WorkPhone = workPhone;
            int recordCount = SaveCustAddress();
            return recordCount;
        }

        #region "properties"
        public string CustId
        {
            get { return (string)base[0]; }
            set { base[0] = value; }
        }
        public string AddressType
        {
            get { return (string)base[1]; }
            set { base[1] = value; }
        }
        public string Address
        {
            get { return (string)base[2]; }
            set { base[2] = value; }
        }
        public string ResStatus
        {
            get { return (string)base[3]; }
            set { base[3] = value; }
        }
        public string WorkAddress
        {
            get { return (string)base[4]; }
            set { base[4] = value; }
        }
        public string WorkPhone
        {
            get { return (string)base[5]; }
            set { base[5] = value; }
        }
        //public string FileName
        //{
        //    get { return (string)base[2]; }
        //    set { base[2] = value; }
        //}
        //public string AccountNumber
        //{
        //    get { return (string)base[3]; }
        //    set { base[3] = value; }
        //}
        //public string Message
        //{
        //    get { return (string)base[4]; }
        //    set { base[4] = value; }
        //}

        #endregion
    }

    #endregion


    #region Customer Credit Summary Transaction 
    public partial class CustomerCreditSummaryTransaction : Blue.Transactions.Command<ContextBase>
    {
        public CustomerCreditSummaryTransaction() : base("dbo.GetCustomerCreditSummaryDetails")
        {
            base.AddInParameter("@CustId", DbType.String);
            base.AddOutParameter("@Message", DbType.String, 5000);
        }
    }

    partial class CustomerCreditSummaryTransaction
    {
        public string GetCustomerCreditSummaryTransaction(DataSet ds)
        {
            base.Fill(ds);
            return this.Message;
        }

        public string GetCustomerCreditSummaryTransaction(DataSet ds, string _CustID)
        {
            this.CustId = _CustID;
            this.Message = string.Empty;
            return GetCustomerCreditSummaryTransaction(ds);
        }

        #region "properties"
        public string CustId
        {
            get { return (string)base[0]; }
            set { base[0] = value; }
        }
        public string Message
        {
            get { return (string)base[1]; }
            set { base[1] = value; }
        }
        #endregion

    }
    #endregion

    #region User Transaction Details 
    public partial class UserTransaction : Blue.Transactions.Command<ContextBase>
    {
        public UserTransaction() : base("dbo.GetCreditAccountDetailsAccountLevel")
        {
            base.AddInParameter("@CustId", DbType.String);
            base.AddInParameter("@PageNumber", DbType.Int32);
            base.AddInParameter("@PageSize", DbType.Int32);
            base.AddOutParameter("@Message", DbType.String, 5000);
        }
    }

    partial class UserTransaction
    {
        public string GetUserTransaction(DataSet ds)
        {
            base.Fill(ds);
            return this.Message;
        }

        public string GetUserTransaction(DataSet ds, UserTransactionInputModel _objUserTransactionInputModel)
        {
            this.CustId = _objUserTransactionInputModel.CustId;
            this.PageNumber = _objUserTransactionInputModel.PageNumber;
            this.PageSize = _objUserTransactionInputModel.PageSize;
            this.Message = string.Empty;
            return GetUserTransaction(ds);
        }

        #region "properties"
        public string CustId
        {
            get { return (string)base[0]; }
            set { base[0] = value; }
        }
        public int PageNumber
        {
            get { return (int)base[1]; }
            set { base[1] = value; }
        }
        public int PageSize
        {
            get { return (int)base[2]; }
            set { base[2] = value; }
        }
        public string Message
        {
            get { return (string)base[3]; }
            set { base[3] = value; }
        }
        #endregion

    }

    #endregion

    #region Get Proposal Date
    public partial class GetProposalDate : Blue.Transactions.Command<ContextBase>
    {
        public GetProposalDate() : base("dbo.GetProposalDate")
        {
            base.AddInParameter("@CustId", DbType.String);
            base.AddInParameter("@AccountNo", DbType.String);
            base.AddOutParameter("@Message", DbType.String, 5000);
        }
    }

    partial class GetProposalDate
    {
        public string GetProposalDateDetails(DataSet ds)
        {
            base.Fill(ds);
            return this.Message;
        }

        public string GetProposalDateDetails(DataSet ds, string _custID, string _accountId)
        {
            this.CustId = _custID;
            this.AccountNumber = _accountId;
            this.Message = string.Empty;
            return GetProposalDateDetails(ds);
        }

        #region "properties"
        public string CustId
        {
            get { return (string)base[0]; }
            set { base[0] = value; }
        }
        public string AccountNumber
        {
            get { return (string)base[1]; }
            set { base[1] = value; }
        }
        public string Message
        {
            get { return (string)base[2]; }
            set { base[2] = value; }
        }
        #endregion

    }
    #endregion

    #region Get country Maintenance
    public partial class DcoumentStatus : Blue.Transactions.Command<ContextBase>
    {
        public DcoumentStatus() : base("dbo.GetCreditdocumentStatus")
        {
            //base.AddInParameter("@Interval", DbType.Int32);
        }
    }

    partial class DcoumentStatus
    {
        public void getDcoumentStatus(DataSet dt)
        {

            base.Fill(dt);

        }

    }

    #endregion


    public partial class GetRFAccountInformation : Blue.Transactions.Command<ContextBase>
    {
        public GetRFAccountInformation() : base("dbo.GetAccountInformation")
        {
            base.AddInParameter("@CustId", DbType.String);
        }
    }

    partial class GetRFAccountInformation
    {
        public void GetRFAccountInformationDetails(DataSet dt, string _custid)
        {
            this.Custid = _custid;
            base.Fill(dt);

        }
        #region "properties"
        public string Custid
        {
            get { return (string)base[0]; }
            set { base[0] = value; }
        }

        #endregion
    }

    public partial class CheckAccount : Blue.Transactions.Command<ContextBase>
    {
        public CheckAccount() : base("dbo.CheckAccountDetails")
        {
            base.AddInParameter("@CustId", DbType.String);
            base.AddInParameter("@StoreId", DbType.String);
        }
    }

    partial class CheckAccount
    {
        public void CheckAccountDetails(DataSet dt, string _custid, string _storeId)
        {
            this.Custid = _custid;
            this.StoreId = _storeId;
            base.Fill(dt);

        }
        #region "properties"
        public string Custid
        {
            get { return (string)base[0]; }
            set { base[0] = value; }
        }

        public string StoreId
        {
            get { return (string)base[1]; }
            set { base[1] = value; }
        }

        #endregion
    }

    public partial class UpdateLineItem : Blue.Transactions.Command<ContextBase>
    {
        public UpdateLineItem() : base("dbo.TPConfirmTransactionLineItem")
        {
            base.AddInParameter("@CustId", DbType.String);
            base.AddInParameter("@loanAmount", DbType.Decimal);
            base.AddInParameter("@numberOfInstallments", DbType.Int32);
            base.AddInParameter("@storeId", DbType.String);
            base.AddInParameter("@acctno", DbType.String);
            base.AddInParameter("@BranchId", DbType.Int32);
        }
    }

    partial class UpdateLineItem
    {
        public int SaveLineItem()
        {
            int recordCount = base.ExecuteNonQuery();
            return recordCount;
        }

        public int SaveLineItem(string custId, decimal loanAmount, int numberOfInstallments, string storeId, string accountNumber, Int16 branch)
        {
            this.CustId = custId;
            this.LoanAmount = loanAmount;
            this.NumberOfInstallments = numberOfInstallments;
            this.StoreId = storeId;
            this.AccountNumber = accountNumber;
            this.BranchId = branch;
            int recordCount = SaveLineItem();
            return recordCount;
        }

        #region "properties"
        public string CustId
        {
            get { return (string)base[0]; }
            set { base[0] = value; }
        }
        public decimal LoanAmount
        {
            get { return (decimal)base[1]; }
            set { base[1] = value; }
        }
        public int NumberOfInstallments
        {
            get { return (int)base[2]; }
            set { base[2] = value; }
        }
        public string StoreId
        {
            get { return (string)base[3]; }
            set { base[3] = value; }
        }
        public string AccountNumber
        {
            get { return (string)base[4]; }
            set { base[4] = value; }
        }
        public int BranchId
        {
            get { return (int)base[5]; }
            set { base[5] = value; }
        }
        #endregion




    }

    #region Update column isTPContractUpload after pdf save 
    public partial class ContractStatusSaveOnServer : Blue.Transactions.Command<ContextBase>
    {
        public ContractStatusSaveOnServer() : base("dbo.EMA_UpIsTPContractUpload")
        {
            base.AddInParameter("@CustId", DbType.String);
            base.AddInParameter("@AcctNo", DbType.String);
        }
    }

    partial class ContractStatusSaveOnServer
    {
        public void ContractSaveOnServerStatus(string custId, string acctNo)
        {
            this.CustId = custId;
            this.AcctNo = acctNo;
            base.ExecuteNonQuery();
        }

        public string CustId
        {
            get { return (string)base[0]; }
            set { base[0] = value; }
        }
        public string AcctNo
        {
            get { return (string)base[1]; }
            set { base[1] = value; }
        }

    }

    #endregion

    #region Get EmailContracts
    public partial class EmailContracts : Blue.Transactions.Command<ContextBase>
    {
        public EmailContracts() : base("dbo.EMA_GetEmailContracts")
        {
            //base.AddInParameter("@Interval", DbType.Int32);
        }
    }

    partial class EmailContracts
    {
        public void getEmailContracts(DataSet dt)
        {

            base.Fill(dt);

        }

    }
    #endregion

    #region UpdateContractNotificationStatusXmlUpdateRepository 
    public partial class UpdateContractNotificationStatusXmlUpdateRepository : Blue.Transactions.Command<ContextBase>
    {
        public UpdateContractNotificationStatusXmlUpdateRepository() : base("dbo.EMA_UpdateContractNotificationStatus")
        {
            base.AddInParameter("@ContractNotificationStatus", DbType.Xml);
            base.AddOutParameter("@Message", DbType.String, 5000);
            base.AddOutParameter("@StatusCode", DbType.Int32, 32);
        }
    }
    partial class UpdateContractNotificationStatusXmlUpdateRepository
    {
        public List<string> UpdateContractNotificationStatusXmlUpdate()
        {
            List<string> returnInvoiceNo = new List<string>();
            int RecordCount = base.ExecuteNonQuery();
            returnInvoiceNo.Add(this.Message);
            returnInvoiceNo.Add(Convert.ToString(this.StatusCode));
            return returnInvoiceNo;
        }

        public List<string> UpdateContractNotificationStatusXmlUpdate(string UpdateContractNotificationStatus)
        {
            this.ContractNotificationStatus = UpdateContractNotificationStatus;
            this.Message = string.Empty;
            this.StatusCode = 0;
            return UpdateContractNotificationStatusXmlUpdate();
        }

        #region "properties"
        public string ContractNotificationStatus
        {
            get { return (string)base[0]; }
            set { base[0] = value; }
        }
        public string Message
        {
            get { return (string)base[1]; }
            set { base[1] = value; }
        }
        public int StatusCode
        {
            get { return (int)base[2]; }
            set { base[2] = value; }
        }
        #endregion
    }
    #endregion

}
