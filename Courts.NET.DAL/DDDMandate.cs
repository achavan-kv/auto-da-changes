using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common.Static;
using STL.Common.Constants.Enums;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;

namespace STL.DAL
{
    /// <summary>
    /// Summary description for DDDMandate.
    /// </summary>
    public class DDDMandate : DALObject
    {
        private int _mandateId;
        private string _acctNo;
        private DateTime _approvalDate;
        private string _bankAcctName;
        private string _bankAcctNo;
        private string _bankBranchNo;
        private string _bankCode;
        private string _cancelReason;
        private int _changedBy;
        private string _comment;
        private int _dueDayId;
        private DateTime _endDate;
        private bool _noFees;
        private DateTime _receiveDate;
        private int _rejectCount;
        private DateTime _returnDate;
        private DateTime _startDate;
        private string _status;
        private DateTime _submitDate;
        private string _customerName;
        private decimal _instalAmount;
        private DataRow _mandateRow = null;


        public int mandateId
        {
            get
            {
                return _mandateId;
            }
            set
            {
                _mandateId = value;
            }
        }

        public string acctNo
        {
            get
            {
                return _acctNo;
            }
            set
            {
                _acctNo = value;
            }
        }

        public DateTime approvalDate
        {
            get
            {
                return _approvalDate;
            }
            set
            {
                _approvalDate = value;
            }
        }

        public string bankAcctName
        {
            get
            {
                return _bankAcctName;
            }
            set
            {
                _bankAcctName = value;
            }
        }

        public string bankAcctNo
        {
            get
            {
                return _bankAcctNo;
            }
            set
            {
                _bankAcctNo = value;
            }
        }

        public string bankBranchNo
        {
            get
            {
                return _bankBranchNo;
            }
            set
            {
                _bankBranchNo = value;
            }
        }

        public string bankCode
        {
            get
            {
                return _bankCode;
            }
            set
            {
                _bankCode = value;
            }
        }

        public string cancelReason
        {
            get
            {
                return _cancelReason;
            }
            set
            {
                _cancelReason = value;
            }
        }

        public int changedBy
        {
            get
            {
                return _changedBy;
            }
            set
            {
                _changedBy = value;
            }
        }

        public string comment
        {
            get
            {
                return _comment;
            }
            set
            {
                _comment = value;
            }
        }

        public int dueDayId
        {
            get
            {
                return _dueDayId;
            }
            set
            {
                _dueDayId = value;
            }
        }

        public DateTime endDate
        {
            get
            {
                return _endDate;
            }
            set
            {
                _endDate = value;
            }
        }

        public bool noFees
        {
            get
            {
                return _noFees;
            }
            set
            {
                _noFees = value;
            }
        }

        public DateTime receiveDate
        {
            get
            {
                return _receiveDate;
            }
            set
            {
                _receiveDate = value;
            }
        }

        public int rejectCount
        {
            get
            {
                return _rejectCount;
            }
            set
            {
                _rejectCount = value;
            }
        }

        public DateTime returnDate
        {
            get
            {
                return _returnDate;
            }
            set
            {
                _returnDate = value;
            }
        }

        public DateTime startDate
        {
            get
            {
                return _startDate;
            }
            set
            {
                _startDate = value;
            }
        }

        public string status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
            }
        }

        public DateTime submitDate
        {
            get
            {
                return _submitDate;
            }
            set
            {
                _submitDate = value;
            }
        }

        public string customerName
        {
            get
            {
                return _customerName;
            }
            set
            {
                _customerName = value;
            }
        }

        public decimal instalAmount
        {
            get
            {
                return _instalAmount;
            }
            set
            {
                _instalAmount = value;
            }
        }



        public DDDMandate()
        {
            this.Init();
        }


        public void Init()
        {
            // Constructor Init
            _mandateId = 0;
            _acctNo = "";
            _bankAcctName = "";
            _bankAcctNo = "";
            _bankBranchNo = "";
            _bankCode = "";
            _cancelReason = "I";
            _changedBy = 0;
            _comment = "";
            _dueDayId = 0;
            _noFees = false;
            _rejectCount = 0;
            _status = "C";
            _customerName = "";
            _instalAmount = 0;
            _receiveDate = Date.blankDate;
            _returnDate = Date.blankDate;
            _submitDate = Date.blankDate;
            _approvalDate = Date.blankDate;
            _startDate = Date.blankDate;
            _endDate = Date.blankDate;
        }


        public int Get(int mandateID)
        {
            return CommonGet(mandateID, "");
        }


        public int Get(string accountNo)
        {
            return CommonGet(0, accountNo);
        }


        public int CommonGet(int mandateID, string accountNo)
        {
            int result = 0;
            int rowCount = 0;

            try
            {
                this.Init();
                parmArray = new SqlParameter[2];

                parmArray[0] = new SqlParameter("@piMandateId", SqlDbType.Int);
                parmArray[0].Value = mandateID;

                parmArray[1] = new SqlParameter("@piAcctNo", SqlDbType.Char,12);
                parmArray[1].Value = accountNo;

                DataSet mandateSet = new DataSet();
                result = this.RunSP("DN_DDMandateGetSP", parmArray, mandateSet);

                this._mandateRow = null;
                foreach (DataTable dt in mandateSet.Tables)
                {
                    if (dt.Rows.Count > 0) this._mandateRow = dt.Rows[0];
                }

                if (result == 0 && this._mandateRow != null)
                {
                    rowCount = 1;
                    this._mandateId = (int)this._mandateRow[CN.MandateId];
                    this._acctNo = (string)this._mandateRow[CN.AcctNo];
                    this._bankAcctName = (string)this._mandateRow[CN.BankAccountName];
                    this._bankAcctNo = (string)this._mandateRow[CN.BankAccountNo];
                    this._bankBranchNo = (string)this._mandateRow[CN.BankBranchNo];
                    this._bankCode = (string)this._mandateRow[CN.BankCode];
                    this._cancelReason = (string)this._mandateRow[CN.CancelReason];
                    this._changedBy = (int)this._mandateRow[CN.ChangedBy];
                    this._dueDayId = (int)this._mandateRow[CN.DueDayId];
                    this._noFees = ("1" == (this._mandateRow[CN.NoFees].ToString()));
                    this._rejectCount = (int)this._mandateRow[CN.RejectCount];
                    this._status = (string)this._mandateRow[CN.Status];
                    this._customerName = (string)this._mandateRow[CN.CustomerName];
                    this._instalAmount = (decimal)this._mandateRow[CN.InstalAmount];

                    if (!Convert.IsDBNull(this._mandateRow[CN.Comment]))
                        this._comment = (string)this._mandateRow[CN.Comment];
                    if (!Convert.IsDBNull(this._mandateRow[CN.ReceiveDate]))
                        this._receiveDate = (DateTime)this._mandateRow[CN.ReceiveDate];
                    if (!Convert.IsDBNull(this._mandateRow[CN.ReturnDate]))
                        this._returnDate = (DateTime)this._mandateRow[CN.ReturnDate];
                    if (!Convert.IsDBNull(this._mandateRow[CN.SubmitDate]))
                        this._submitDate = (DateTime)this._mandateRow[CN.SubmitDate];
                    if (!Convert.IsDBNull(this._mandateRow[CN.ApprovalDate]))
                        this._approvalDate = (DateTime)this._mandateRow[CN.ApprovalDate];
                    if (!Convert.IsDBNull(this._mandateRow[CN.StartDate]))
                        this._startDate = (DateTime)this._mandateRow[CN.StartDate];
                    if (!Convert.IsDBNull(this._mandateRow[CN.EndDate]))
                        this._endDate = (DateTime)this._mandateRow[CN.EndDate];
                }

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return rowCount;
        }


        public int AnotherMandate(int mandateId, string accountNo, out int anMandateId,
            out DateTime startDate, out DateTime endDate)
        {
            int result = 0;
            int rowCount = 0;

            try
            {
                anMandateId = 0;
                startDate = Date.blankDate;
                endDate = Date.blankDate;

                parmArray = new SqlParameter[5];

                parmArray[0] = new SqlParameter("@piMandateId", SqlDbType.Int);
                parmArray[0].Value = this._mandateId;
                parmArray[1] = new SqlParameter("@piAcctNo", SqlDbType.Char, 12);
                parmArray[1].Value = this._acctNo;

                parmArray[2] = new SqlParameter("@poMandateId", SqlDbType.Int);
                parmArray[2].Direction = ParameterDirection.Output;
                parmArray[3] = new SqlParameter("@poStartDate", SqlDbType.SmallDateTime);
                parmArray[3].Direction = ParameterDirection.Output;
                parmArray[4] = new SqlParameter("@poEndDate", SqlDbType.SmallDateTime);
                parmArray[4].Direction = ParameterDirection.Output;

                result = this.RunSP("DN_DDAnotherMandateSP", parmArray);

                if (result == 0 && !Convert.IsDBNull(parmArray[2].Value))
                {
                    rowCount = 1;
                    startDate = (DateTime)parmArray[3].Value;
                    endDate = (DateTime)parmArray[4].Value;
                }
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return rowCount;
        }


        public int NewMandate(SqlConnection conn, SqlTransaction trans)
        {
            int result = 0;

            try
            {
                parmArray = new SqlParameter[17];

                parmArray[0] = new SqlParameter("@piAcctNo", SqlDbType.Char, 12);
                parmArray[0].Value = this._acctNo;
                parmArray[1] = new SqlParameter("@piDueDayId", SqlDbType.Int);
                parmArray[1].Value = this._dueDayId;
                parmArray[2] = new SqlParameter("@piBankAcctName", SqlDbType.NChar, 20);
                parmArray[2].Value = this._bankAcctName;
                parmArray[3] = new SqlParameter("@piBankCode", SqlDbType.NChar, 4);
                parmArray[3].Value = this._bankCode;
                parmArray[4] = new SqlParameter("@piBankBranchNo", SqlDbType.NChar, 3);
                parmArray[4].Value = this._bankBranchNo;
                parmArray[5] = new SqlParameter("@piBankAcctNo", SqlDbType.NChar, 11);
                parmArray[5].Value = this._bankAcctNo;
                parmArray[6] = new SqlParameter("@piReceiveDate", SqlDbType.DateTime);
                parmArray[6].Value = this._receiveDate;
                parmArray[7] = new SqlParameter("@piReturnDate", SqlDbType.DateTime);
                parmArray[7].Value = this._returnDate;
                parmArray[8] = new SqlParameter("@piSubmitDate", SqlDbType.DateTime);
                parmArray[8].Value = this._submitDate;
                parmArray[9] = new SqlParameter("@piApprovalDate", SqlDbType.DateTime);
                parmArray[9].Value = this._approvalDate;
                parmArray[10] = new SqlParameter("@piStartDate", SqlDbType.DateTime);
                parmArray[10].Value = this._startDate;
                parmArray[11] = new SqlParameter("@piEndDate", SqlDbType.DateTime);
                parmArray[11].Value = this._endDate;
                parmArray[12] = new SqlParameter("@piComment", SqlDbType.NChar, 200);
                parmArray[12].Value = this._comment;
                parmArray[13] = new SqlParameter("@piCancelReason", SqlDbType.NChar, 1);
                parmArray[13].Value = this._cancelReason;
                parmArray[14] = new SqlParameter("@piNoFees", SqlDbType.TinyInt);
                parmArray[14].Value = this._noFees;
                parmArray[15] = new SqlParameter("@piRejectCount", SqlDbType.Int);
                parmArray[15].Value = this._rejectCount;
                parmArray[16] = new SqlParameter("@piChangedBy", SqlDbType.Int);
                parmArray[16].Value = this._changedBy;

                result = this.RunSP(conn, trans, "DN_DDNewMandateSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            return result;
        }


        public int UpdateMandate(SqlConnection conn, SqlTransaction trans)
        {
            int result = 0;

            try
            {
                parmArray = new SqlParameter[17];

                parmArray[0] = new SqlParameter("@piMandateId", SqlDbType.Int);
                parmArray[0].Value = this._mandateId;
                parmArray[1] = new SqlParameter("@piAcctNo", SqlDbType.Char, 12);
                parmArray[1].Value = this._acctNo;
                parmArray[2] = new SqlParameter("@piDueDayId", SqlDbType.Int);
                parmArray[2].Value = this._dueDayId;
                parmArray[3] = new SqlParameter("@piBankAcctName", SqlDbType.NChar, 20);
                parmArray[3].Value = this._bankAcctName;
                parmArray[4] = new SqlParameter("@piBankCode", SqlDbType.NChar, 4);
                parmArray[4].Value = this._bankCode;
                parmArray[5] = new SqlParameter("@piBankBranchNo", SqlDbType.NChar, 3);
                parmArray[5].Value = this._bankBranchNo;
                parmArray[6] = new SqlParameter("@piBankAcctNo", SqlDbType.NChar, 11);
                parmArray[6].Value = this._bankAcctNo;
                parmArray[7] = new SqlParameter("@piReceiveDate", SqlDbType.DateTime);
                parmArray[7].Value = this._receiveDate;
                parmArray[8] = new SqlParameter("@piReturnDate", SqlDbType.DateTime);
                parmArray[8].Value = this._returnDate;
                parmArray[9] = new SqlParameter("@piSubmitDate", SqlDbType.DateTime);
                parmArray[9].Value = this._submitDate;
                parmArray[10] = new SqlParameter("@piApprovalDate", SqlDbType.DateTime);
                parmArray[10].Value = this._approvalDate;
                parmArray[11] = new SqlParameter("@piStartDate", SqlDbType.DateTime);
                parmArray[11].Value = this._startDate;
                parmArray[12] = new SqlParameter("@piEndDate", SqlDbType.DateTime);
                parmArray[12].Value = this._endDate;
                parmArray[13] = new SqlParameter("@piComment", SqlDbType.NChar, 200);
                parmArray[13].Value = this._comment;
                parmArray[14] = new SqlParameter("@piCancelReason", SqlDbType.NChar, 1);
                parmArray[14].Value = this._cancelReason;
                parmArray[15] = new SqlParameter("@piNoFees", SqlDbType.TinyInt);
                parmArray[15].Value = this._noFees;
                parmArray[16] = new SqlParameter("@piChangedBy", SqlDbType.Int);
                parmArray[16].Value = this._changedBy;

                result = this.RunSP(conn, trans, "DN_DDUpdateMandateSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            return result;
        }


        public int CancelMandates(SqlConnection conn, SqlTransaction trans,
            DateTime runDate, DateTime effectiveDate, DateTime nextDueDate, int maxRejections)
        {
            // Cancel mandates not approved or with a cancelled rejection
            int result = 0;

            try
            {
                parmArray = new SqlParameter[4];

                parmArray[0] = new SqlParameter("@piRunDate", SqlDbType.SmallDateTime);
                parmArray[0].Value = runDate;
                parmArray[1] = new SqlParameter("@piEffectiveDate", SqlDbType.SmallDateTime);
                parmArray[1].Value = effectiveDate;
                parmArray[2] = new SqlParameter("@piNextDueDate", SqlDbType.SmallDateTime);
                parmArray[2].Value = nextDueDate;
                parmArray[3] = new SqlParameter("@piMaxRejections", SqlDbType.Int);
                parmArray[3].Value = maxRejections;

                result = this.RunSP(conn, trans, "DN_DDCancelMandatesSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;
        }


        public DataTable ConfirmMandateList(SqlConnection conn, SqlTransaction trans,
            DateTime effectiveDate)
        {
            // Retrieve all mandates with an Approval Date and a Date Delivered

            DataTable mandateList = new DataTable(TN.DDMandate);

            try
            {
                parmArray = new SqlParameter[1];

                parmArray[0] = new SqlParameter("@piEffectiveDate", SqlDbType.SmallDateTime);
                parmArray[0].Value = effectiveDate;

                this.RunSP(conn, trans, "DN_DDConfirmMandateListSP", parmArray, mandateList);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return mandateList;
        }


        public int ConfirmMandate(SqlConnection conn, SqlTransaction trans,
            DateTime runDate, DateTime effectiveDate, string acctNo, int mandateId,
            DateTime approvalDate, int dueDay, DateTime dateFirst)
        {
            // Start a mandate with an Approval Date and a Date Delivered
            int result = 0;

            try
            {
                parmArray = new SqlParameter[7];

                parmArray[0] = new SqlParameter("@piRunDate", SqlDbType.SmallDateTime);
                parmArray[0].Value = runDate;
                parmArray[1] = new SqlParameter("@piEffectiveDate", SqlDbType.SmallDateTime);
                parmArray[1].Value = effectiveDate;
                parmArray[2] = new SqlParameter("@piAcctNo", SqlDbType.Char, 12);
                parmArray[2].Value = acctNo;
                parmArray[3] = new SqlParameter("@piMandateId", SqlDbType.Int);
                parmArray[3].Value = mandateId;
                parmArray[4] = new SqlParameter("@piApprovalDate", SqlDbType.SmallDateTime);
                parmArray[4].Value = approvalDate;
                parmArray[5] = new SqlParameter("@piDueDay", SqlDbType.Int);
                parmArray[5].Value = dueDay;
                parmArray[6] = new SqlParameter("@piDateFirst", SqlDbType.SmallDateTime);
                parmArray[6].Value = dateFirst;

                result = this.RunSP(conn, trans, "DN_DDConfirmMandateSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;
        }


        public DateTime GiroDateFirst(SqlConnection conn, SqlTransaction trans,
            string acctNo, DateTime effectiveDate, int minPeriod)
        {
            // Retrieve all mandates with an Approval Date and a Date Delivered
            DateTime giroDateFirst = Date.blankDate;

            try
            {
                parmArray = new SqlParameter[4];

                parmArray[0] = new SqlParameter("@piAcctNo", SqlDbType.Char, 12);
                parmArray[0].Value = acctNo;
                parmArray[1] = new SqlParameter("@piEffectiveDate", SqlDbType.SmallDateTime);
                parmArray[1].Value = effectiveDate;
                parmArray[2] = new SqlParameter("@piMinPeriod", SqlDbType.Int);
                parmArray[2].Value = minPeriod;
                parmArray[3] = new SqlParameter("@poDateFirst", SqlDbType.SmallDateTime);
                parmArray[3].Direction = ParameterDirection.Output;

                result = this.RunSP(conn, trans, "DN_DDGiroDateFirstSP", parmArray);

                if (result == 0 && !Convert.IsDBNull(parmArray[3].Value))
                {
                    giroDateFirst = (DateTime)parmArray[3].Value;
                }
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return giroDateFirst;
        }

        public decimal GetGiroPending(string accountNo)
        {
            decimal pending = 0;

            try
            {
                parmArray = new SqlParameter[2];

                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[0].Value = accountNo;
                parmArray[1] = new SqlParameter("@pending", SqlDbType.Money);
                parmArray[1].Direction = ParameterDirection.Output;

                result = this.RunSP("DN_DDPendingGetSP", parmArray);

                if (parmArray[1].Value != DBNull.Value)
                    pending = (decimal)parmArray[1].Value;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            return pending;
        }
    }
}

