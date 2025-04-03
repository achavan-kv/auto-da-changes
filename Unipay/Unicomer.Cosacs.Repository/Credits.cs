using System;
using System.Collections.Generic;
using System.Data;

namespace Unicomer.Cosacs.Repository
{

    public partial class GetMaxWithdrawalAmountRepository : Blue.Transactions.Command<ContextBase>
    {
        public GetMaxWithdrawalAmountRepository() : base("dbo.GetMaxWithdrawalAmount")
        {
            base.AddInParameter("@CustId", DbType.String);
            base.AddOutParameter("@Message", DbType.String, 5000);
            base.AddOutParameter("@Status", DbType.String, 5000);
        }
    }

    #region GetMaxWithdrawalAmountRepository 
    partial class GetMaxWithdrawalAmountRepository
    {
        public List<string> GetMaxWithdrawalAmount(DataSet ds)
        {
            List<string> resultList = new List<string>();
            base.Fill(ds);
            resultList.Add(this.Message);
            resultList.Add(this.Status);
            return resultList;
        }

        public List<string> GetMaxWithdrawalAmount(DataSet ds, string _CustID)
        {
            this.CustId = _CustID;
            this.Message = string.Empty;
            this.Status = string.Empty;
            return GetMaxWithdrawalAmount(ds);
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

    #region GetPaymentOptionsByAmount 
    public partial class GetPaymentOptionsByAmountRepository : Blue.Transactions.Command<ContextBase>
    {
        public GetPaymentOptionsByAmountRepository() : base("dbo.GetPaymentOptionsByAmount")
        {
            base.AddInParameter("@CustId", DbType.String);
            base.AddInParameter("@LoanAmount", DbType.String);
            base.AddOutParameter("@Message", DbType.String, 5000);
            base.AddOutParameter("@Status", DbType.String, 5000);
        }
    }

    partial class GetPaymentOptionsByAmountRepository
    {
        public List<string> GetPaymentOptionsByAmount(DataSet ds)
        {
            List<string> resultList = new List<string>();
            base.Fill(ds);
            resultList.Add(this.Message);
            resultList.Add(this.Status);
            return resultList;
        }

        public List<string> GetPaymentOptionsByAmount(DataSet ds, string _CustID, string _LoanAmount)
        {
            this.CustId = _CustID;
            this.LoanAmount = _LoanAmount;
            this.Message = string.Empty;
            this.Status = string.Empty;
            return GetPaymentOptionsByAmount(ds);
        }

        #region "properties"
        public string CustId
        {
            get { return (string)base[0]; }
            set { base[0] = value; }
        }
        public string LoanAmount
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

    public partial class UpdateCreditInformation : Blue.Transactions.Command<ContextBase>
    {
        public UpdateCreditInformation() : base("dbo.UpdateCreditInformation")
        {
            base.AddInParameter("@custId", DbType.String);
            base.AddInParameter("@AccountNumber", DbType.String);
            base.AddInParameter("@DayOfMonth", DbType.Int32);
            base.AddOutParameter("@Message", DbType.String, 5000);
            base.AddOutParameter("@Status", DbType.Int32, 32);

        }
    }

    partial class UpdateCreditInformation
    {
        public List<string> SaveCreditInformation()
        {
            List<string> returnCustId = new List<string>();// string.Empty;
            int RecordCount = base.ExecuteNonQuery();
            //returnCustId.Add(this.ReturnCustId);
            returnCustId.Add(this.Message);
            returnCustId.Add(this.Status.ToString());
            return returnCustId;

            //DataSet recordCount = base.ExecuteNonQuery();
            //return recordCount;
        }

        public List<string> SaveCreditInformation(string custId, string acctno,int dayofmonth, string _Message, int _StatusCode)
        {
            this.CustId = custId;
            this.Acctno = acctno;
            this.dayofmonth = dayofmonth;
            this.Message = _Message;
            this.Status = _StatusCode;
            List<string> ReturnCustId = SaveCreditInformation();
            return ReturnCustId;
            //DataSet recordCount = SaveCreditInformation();
            //return recordCount;
        }

        #region "properties"
        public string CustId
        {
            get { return (string)base[0]; }
            set { base[0] = value; }
        }
        public string Acctno
        {
            get { return (string)base[1]; }
            set { base[1] = value; }
        }
        public int dayofmonth
        {
            get { return (int)base[2]; }
            set { base[2] = value; }
        }
        public string Message
        {
            get { return (string)base[3]; }
            set { base[3] = value; }
        }
        public int Status
        {
            get { return (int)base[4]; }
            set { base[4] = value; }
        }


        #endregion
    }
}
