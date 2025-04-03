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

    #region Get country Maintenance
    public partial class CountryMaintenance : Blue.Transactions.Command<ContextBase>
    {
        public CountryMaintenance() : base("dbo.VE_CountryMaintenance")
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

    #region BillGenerationXmlInsertRepository 
    public partial class BillGenerationXmlInsertRepository : Blue.Transactions.Command<ContextBase>
    {
        public BillGenerationXmlInsertRepository() : base("dbo.VE_BillGenerationSave")
        {
            base.AddInParameter("@BillGenerationHeader", DbType.Xml);
            base.AddInParameter("@IsUpdate", DbType.Boolean);
            base.AddOutParameter("@Message", DbType.String, 5000);
            base.AddOutParameter("@StatusCode", DbType.Int32, 32);
        }
    }
    partial class BillGenerationXmlInsertRepository
    {
        public List<string> InsertBillGeneration()
        {
            List<string> returnInvoiceNo = new List<string>();// string.Empty;
            int RecordCount = base.ExecuteNonQuery();
            returnInvoiceNo.Add(this.Message);
            returnInvoiceNo.Add(Convert.ToString(this.StatusCode));
            return returnInvoiceNo; // this.ReturnCustId==""?"Message:"+this.Message:"CustID:"+this.ReturnCustId;
        }

        public List<string> InsertBillGeneration(string BillGeneration, bool isUpdate)
        {
            this.BillGeneration = BillGeneration;
            this.IsUpdate = isUpdate;
            this.Message = string.Empty;
            this.StatusCode = 0;
            return InsertBillGeneration();
        }

        #region "properties"
        public string BillGeneration
        {
            get { return (string)base[0]; }
            set { base[0] = value; }
        }
        public bool IsUpdate
        {
            get { return Convert.ToBoolean(base[1]); }
            set { base[1] = value; }
        }
        public string Message
        {
            get { return (string)base[2]; }
            set { base[2] = value; }
        }
        public int StatusCode
        {
            get { return (int)base[3]; }
            set { base[3] = value; }
        }
        #endregion
    }
    #endregion

    #region POXmlInsertRepository 
    public partial class POXmlInsertRepository : Blue.Transactions.Command<ContextBase>
    {
        public POXmlInsertRepository() : base("dbo.VE_PurchaseOrderSave")
        {
            base.AddInParameter("@PurchaseOrderxml", DbType.Xml);
            base.AddInParameter("@isUpdate", DbType.Boolean);
            base.AddOutParameter("@Message", DbType.String, 5000);
            base.AddOutParameter("@StatusCode", DbType.Int32, 32);
        }
    }
    partial class POXmlInsertRepository
    {
        public List<string> InsertPO()
        {
            List<string> returnCustId = new List<string>();// string.Empty;
            int RecordCount = base.ExecuteNonQuery();
            returnCustId.Add(this.Message);
            returnCustId.Add(Convert.ToString(this.StatusCode));
            return returnCustId; // this.ReturnCustId==""?"Message:"+this.Message:"CustID:"+this.ReturnCustId;
        }

        public List<string> InsertPO(string strPO, bool isUpdate)
        {
            this.PO = strPO;
            this.IsUpdate = isUpdate;
            this.Message = string.Empty;
            this.StatusCode = 0;
            return InsertPO();
        }

        #region "properties"
        public string PO
        {
            get { return (string)base[0]; }
            set { base[0] = value; }
        }
        public bool IsUpdate
        {
            get { return (bool)base[1]; }
            set { base[1] = value; }
        }
        public string Message
        {
            get { return (string)base[2]; }
            set { base[2] = value; }
        }
        public int StatusCode
        {
            get { return (int)base[3]; }
            set { base[3] = value; }
        }
        #endregion

    }
    #endregion

    #region GetUserAccountsRepository 
    public partial class GetUserAccountsRepository : Blue.Transactions.Command<ContextBase>
    {
        public GetUserAccountsRepository() : base("dbo.VE_GetUserAccounts")
        {
            base.AddInParameter("@CustId", DbType.String);
            base.AddOutParameter("@Message", DbType.String, 5000);
            base.AddOutParameter("@Status", DbType.String, 5000);
        }
    }

    partial class GetUserAccountsRepository
    {
        public List<string> GetUserAccounts(DataSet ds)
        {
            List<string> resultList = new List<string>();
            base.Fill(ds);
            resultList.Add(this.Message);
            resultList.Add(this.Status);
            return resultList;
        }

        public List<string> GetUserAccounts(DataSet ds, string _CustID)
        {
            this.CustId = _CustID;
            this.Message = string.Empty;
            this.Status = string.Empty;
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
        public string Status
        {
            get { return (string)base[2]; }
            set { base[2] = value; }
        }

        #endregion

    }
    #endregion

    #region GetDateofBirth 
    public partial class GetDateofBirth : Blue.Transactions.Command<ContextBase>
    {
        public GetDateofBirth() : base("dbo.VE_GetDateofBirth")
        {
            base.AddInParameter("@custId", DbType.String);

        }
    }

    partial class GetDateofBirth
    {
        public string GetDateofBirthValue(DataSet ds)
        {
            base.Fill(ds);
            // return this.Message;
            //return ds;
            return null;
        }

        public string GetDateofBirthValue(DataSet ds, string _custID)
        {
            this.CustId = _custID;

            return GetDateofBirthValue(ds);
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

    #region GetGRNDetails
    public partial class GetGRNRepository : Blue.Transactions.Command<ContextBase>
    {
        public GetGRNRepository() : base("VE_GetGRNDetails")
        {
            base.AddInParameter("@GRNNo", DbType.String);
            base.AddOutParameter("@Message", DbType.String, 5000);
            base.AddOutParameter("@Status", DbType.String, 5000);
        }
    }

    partial class GetGRNRepository
    {

        public new void Fill(DataSet ds, string GRNNo)
        {
            this.GRNNo = GRNNo;
            this.Message = this.Message;
            this.Status = this.Status;
            base.Fill(ds);
        }
        #region "properties"
        public string GRNNo
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

    #region DeliveryAuthorization
    public partial class DeliveryAuthRepository : Blue.Transactions.Command<ContextBase>
    {
        public DeliveryAuthRepository() : base("VE_DeliveryAuthorization")
        {
            base.AddInParameter("@AccountNo", DbType.String);
            base.AddInParameter("@DocType", DbType.String);
            base.AddOutParameter("@Message", DbType.String, 5000);
            base.AddOutParameter("@Status", DbType.String, 5000);
        }
    }

    partial class DeliveryAuthRepository
    {

        public new void Fill(DataSet ds, string AcctNo, string DocType)
        {
            this.AccountNo = AcctNo;
            this.DocType = DocType;
            this.Message = this.Message;
            this.Status = this.Status;
            base.Fill(ds);
        }
        #region "properties"
        public string AccountNo
        {
            get { return (string)base[0]; }
            set { base[0] = value; }
        }
        public string DocType
        {
            get { return (string)base[1]; }
            set { base[1] = value; }
        }
        public string Message
        {
            get { return (string)base[2]; }
            set { base[2] = value; }
        }
        public string Status
        {
            get { return (string)base[3]; }
            set { base[3] = value; }
        }

        #endregion

    }
    #endregion

    #region VendorReturnXmlInsertRepository 
    public partial class VendorReturnXmlInsertRepository : Blue.Transactions.Command<ContextBase>
    {
        public VendorReturnXmlInsertRepository() : base("dbo.VE_VendorReturn")
        {
            base.AddInParameter("@VendorReturnxml", DbType.Xml);
            base.AddOutParameter("@Message", DbType.String, 5000);
            base.AddOutParameter("@StatusCode", DbType.Int32, 32);
        }
    }
    partial class VendorReturnXmlInsertRepository
    {
        public List<string> InsertVendorReturn()
        {
            List<string> returnCustId = new List<string>();// string.Empty;
            int RecordCount = base.ExecuteNonQuery();
            returnCustId.Add(this.Message);
            returnCustId.Add(Convert.ToString(this.StatusCode));
            return returnCustId; // this.ReturnCustId==""?"Message:"+this.Message:"CustID:"+this.ReturnCustId;
        }

        public List<string> InsertVendorReturn(string strVendorReturn)
        {
            this.VendorReturn = strVendorReturn;
            this.Message = string.Empty;
            this.StatusCode = 0;
            return InsertVendorReturn();
        }

        #region "properties"
        public string VendorReturn
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

    #region Commissions 
    public partial class CommissionsXmlInsertRepository : Blue.Transactions.Command<ContextBase>
    {
        public CommissionsXmlInsertRepository() : base("dbo.VE_SaveCommissions")
        {
            base.AddInParameter("@Commissionsxml", DbType.Xml);
            base.AddInParameter("@IsUpdate", DbType.Boolean);
            base.AddOutParameter("@Message", DbType.String, 5000);
            base.AddOutParameter("@StatusCode", DbType.Int32, 32);
        }
    }
    partial class CommissionsXmlInsertRepository
    {
        public List<string> InsertCommissions()
        {
            List<string> returnCustId = new List<string>();// string.Empty;
            int RecordCount = base.ExecuteNonQuery();
            returnCustId.Add(this.Message);
            returnCustId.Add(Convert.ToString(this.StatusCode));
            return returnCustId; // this.ReturnCustId==""?"Message:"+this.Message:"CustID:"+this.ReturnCustId;
        }

        public List<string> InsertCommissions(string strCommissions, bool isUpdate)
        {
            this.Commissions = strCommissions;
            this.IsUpdate = isUpdate;
            this.Message = string.Empty;
            this.StatusCode = 0;
            return InsertCommissions();
        }

        #region "properties"
        public string Commissions
        {
            get { return (string)base[0]; }
            set { base[0] = value; }
        }
        public bool IsUpdate
        {
            get { return (bool)base[1]; }
            set { base[1] = value; }
        }
        public string Message
        {
            get { return (string)base[2]; }
            set { base[2] = value; }
        }
        public int StatusCode
        {
            get { return (int)base[3]; }
            set { base[3] = value; }
        }
        #endregion

    }
    #endregion

    #region DeleteTaskSchedular
    public partial class DeleteTaskSchedularRepository : Blue.Transactions.Command<ContextBase>
    {
        public DeleteTaskSchedularRepository() : base("dbo.VE_DeleteSyncData")
        {
            base.AddInParameter("@ServiceCode", DbType.String);
            base.AddInParameter("@Code", DbType.String);
            base.AddInParameter("@IsInsertRecord", DbType.Boolean);
            base.AddInParameter("@IsEODRecords", DbType.Boolean);
            base.AddInParameter("@Message", DbType.String);
            base.AddInParameter("@Orderid", DbType.String);
            base.AddInParameter("@ID", DbType.String);
        }
    }
    partial class DeleteTaskSchedularRepository
    {
        public string DeleteSyncDocumentUpdate()
        {
            base.ExecuteNonQuery();
            //return this.Message;
            return "true";
        }

        public string DeleteSyncDocumentUpdate(string ServiceCode, string Code, bool IsInsertRecord, bool IsEODRecords, string Message, string Orderid, string ID)
        {
            this.ServiceCode = ServiceCode;
            this.Code = Code;
            this.IsInsertRecord = IsInsertRecord;
            this.IsEODRecords = IsEODRecords;
            this.Message = Message;
            this.Orderid = Orderid;
            this.ID = ID;
            return DeleteSyncDocumentUpdate();
        }

        #region "properties"
        public string ServiceCode
        {
            get { return (string)base[0]; }
            set { base[0] = value; }
        }
        public string Code
        {
            get { return (string)base[1]; }
            set { base[1] = value; }
        }
        public bool IsInsertRecord
        {
            get { return (bool)base[2]; }
            set { base[2] = value; }
        }
        public bool IsEODRecords
        {
            get { return (bool)base[3]; }
            set { base[3] = value; }
        }
        public string Message
        {
            get { return (string)base[4]; }
            set { base[4] = value; }
        }
        public string Orderid
        {
            get { return (string)base[5]; }
            set { base[5] = value; }
        }
        public string ID
        {
            get { return (string)base[6]; }
            set { base[6] = value; }
        }
        #endregion
    }

    #endregion

    #region GetPaymentsOrderList
    public partial class GetPaymentsOrderListRepository : Blue.Transactions.Command<ContextBase>
    {
        public GetPaymentsOrderListRepository() : base("dbo.VE_GetPaymentsOrderList")
        {
            base.AddInParameter("@acctno", DbType.String);
            base.AddInParameter("@ID", DbType.String);
            base.AddOutParameter("@Message", DbType.String, 5000);
            base.AddOutParameter("@StatusCode", DbType.String, 5000);
        }
    }
    partial class GetPaymentsOrderListRepository
    {
        public List<Payments> GetPaymentsOrderList(DataSet ds)
        {
            List<Payments> resultList = new List<Payments>();
            base.Fill(ds);
            //resultList.Add(this.Message);
            //resultList.Add(this.StatusCode);
            return resultList;
        }

        public List<Payments> GetPaymentsOrderList(DataSet ds, string _AcctNo, string ID)
        {
            this.AcctNo = _AcctNo;
            this.ID = ID;
            this.Message = string.Empty;
            this.StatusCode = string.Empty;
            return GetPaymentsOrderList(ds);
        }

        #region "properties"
        public string AcctNo
        {
            get { return (string)base[0]; }
            set { base[0] = value; }
        }
        public string ID
        {
            get { return (string)base[1]; }
            set { base[1] = value; }
        }
        public string Message
        {
            get { return (string)base[2]; }
            set { base[2] = value; }
        }
        public string StatusCode
        {
            get { return (string)base[3]; }
            set { base[3] = value; }
        }
        #endregion
    }
    #endregion

    #region GetCancelPaymentsOrderList
    public partial class GetCancelPaymentsOrderListRepository : Blue.Transactions.Command<ContextBase>
    {
        public GetCancelPaymentsOrderListRepository() : base("dbo.VE_GetCancelPaymentsOrderList")
        {
            base.AddInParameter("@acctno", DbType.String);
            base.AddOutParameter("@Message", DbType.String, 5000);
            base.AddOutParameter("@StatusCode", DbType.String, 5000);
        }
    }
    partial class GetCancelPaymentsOrderListRepository
    {
        public List<Payments> GetCancelPaymentsOrderList(DataSet ds)
        {
            List<Payments> resultList = new List<Payments>();
            base.Fill(ds);
            //resultList.Add(this.Message);
            //resultList.Add(this.StatusCode);
            return resultList;
        }

        public List<Payments> GetCancelPaymentsOrderList(DataSet ds, string _AcctNo)
        {
            this.AcctNo = _AcctNo;
            this.Message = string.Empty;
            this.StatusCode = string.Empty;
            return GetCancelPaymentsOrderList(ds);
        }

        #region "properties"
        public string AcctNo
        {
            get { return (string)base[0]; }
            set { base[0] = value; }
        }
        public string Message
        {
            get { return (string)base[1]; }
            set { base[1] = value; }
        }
        public string StatusCode
        {
            get { return (string)base[2]; }
            set { base[2] = value; }
        }
        #endregion
    }
    #endregion

    #region DeliveryConfirmation
    public partial class DeliveryConfirmationRepository : Blue.Transactions.Command<ContextBase>
    {
        public DeliveryConfirmationRepository() : base("VE_GetDeliveryConfirmation")
        {
            base.AddInParameter("@AccountNo", DbType.String);
            base.AddInParameter("@Id", DbType.String);
            base.AddOutParameter("@Message", DbType.String, 5000);
            base.AddOutParameter("@Status", DbType.String, 5000);
        }
    }

    partial class DeliveryConfirmationRepository
    {

        public new void Fill(DataSet ds, string AcctNo, string Id)
        {
            this.AccountNo = AcctNo;
            this.Id = Id;
            this.Message = this.Message;
            this.Status = this.Status;
            base.Fill(ds);
        }
        #region "properties"
        public string AccountNo
        {
            get { return (string)base[0]; }
            set { base[0] = value; }
        }
        public string Id
        {
            get { return (string)base[1]; }
            set { base[1] = value; }
        }
        public string Message
        {
            get { return (string)base[2]; }
            set { base[2] = value; }
        }
        public string Status
        {
            get { return (string)base[3]; }
            set { base[3] = value; }
        }

        #endregion

    }
    #endregion

    #region CustomerReturnXmlInsertRepository 
    public partial class CustomerReturnXmlInsertRepository : Blue.Transactions.Command<ContextBase>
    {
        public CustomerReturnXmlInsertRepository() : base("dbo.VE_CustomerReturn")
        {
            base.AddInParameter("@CustomerReturnxml", DbType.Xml);
            base.AddOutParameter("@Message", DbType.String, 5000);
            base.AddOutParameter("@StatusCode", DbType.Int32, 32);
        }
    }
    partial class CustomerReturnXmlInsertRepository
    {
        public List<string> InsertCustomerReturn()
        {
            List<string> returnCustId = new List<string>();// string.Empty;
            int RecordCount = base.ExecuteNonQuery();
            returnCustId.Add(this.Message);
            returnCustId.Add(Convert.ToString(this.StatusCode));
            return returnCustId; // this.ReturnCustId==""?"Message:"+this.Message:"CustID:"+this.ReturnCustId;
        }

        public List<string> InsertCustomerReturn(string strCustomerReturn)
        {
            this.CustomerReturn = strCustomerReturn;
            this.Message = string.Empty;
            this.StatusCode = 0;
            return InsertCustomerReturn();
        }

        #region "properties"
        public string CustomerReturn
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

    #region GetVendorReturns
    public partial class GetVendorReturnRepository : Blue.Transactions.Command<ContextBase>
    {
        public GetVendorReturnRepository() : base("VE_VendorReturn")
        {
            base.AddInParameter("@VendorReturnID", DbType.String);
            base.AddOutParameter("@Message", DbType.String, 5000);
            base.AddOutParameter("@Status", DbType.String, 5000);
        }
    }

    partial class GetVendorReturnRepository
    {
        public new void Fill(DataSet ds, string VendorReturnID)
        {
            this.VendorReturnID = VendorReturnID;
            this.Message = this.Message;
            this.Status = this.Status;
            base.Fill(ds);
        }
        #region "properties"
        public string VendorReturnID
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
}
