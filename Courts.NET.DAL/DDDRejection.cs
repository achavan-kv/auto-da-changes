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
    /// Summary description for DDDRejection.
    /// </summary>
    public class DDDRejection : DALObject
    {
        private string _acctNo;
        private decimal _amount;
        private string _curRejectAction;
        private string _customerName;
        private int _mandateId;
        private short _origMonth;
        private int _paymentId;
        private string _paymentType;
        private string _rejectAction;


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

        public decimal amount
        {
            get
            {
                return _amount;
            }
            set
            {
                _amount = value;
            }
        }

        public string curRejectAction
        {
            get
            {
                return _curRejectAction;
            }
            set
            {
                _curRejectAction = value;
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

        public short origMonth
        {
            get
            {
                return _origMonth;
            }
            set
            {
                _origMonth = value;
            }
        }

        public int paymentId
        {
            get
            {
                return _paymentId;
            }
            set
            {
                _paymentId = value;
            }
        }

        public string paymentType
        {
            get
            {
                return _paymentType;
            }
            set
            {
                _paymentType = value;
            }
        }

        public string rejectAction
        {
            get
            {
                return _rejectAction;
            }
            set
            {
                _rejectAction = value;
            }
        }



        public DDDRejection()
        {
        }


        public DataSet GetRejectList(DateTime effectiveDate)
        {
            int result = 0;
            DataSet rejectionSet = new DataSet();
            try
            {
                parmArray = new SqlParameter[1];

                parmArray[0] = new SqlParameter("@piEffectiveDate", SqlDbType.SmallDateTime);
                parmArray[0].Value = effectiveDate;

                result = this.RunSP("DN_DDRejectionListGetSP", parmArray, rejectionSet);

                if (rejectionSet != null)
                    if (rejectionSet.Tables.Count > 0)
                        rejectionSet.Tables[0].TableName = TN.DDRejection;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            return rejectionSet;
        }


        public DateTime NearestDueDate()
        {
            /* Get the nearest due date to today, either before or after today */
            DateTime nearestDueDate = Date.blankDate;
            DataTable nearestList = new DataTable();
            try
            {
                result = this.RunSP("DN_DDNearestDueDateSP", nearestList);

                if (result == 0 && nearestList.Rows.Count > 0)
                {
                    if (!Convert.IsDBNull(nearestList.Rows[0][CN.NearestDueDate]))
                    {
                        nearestDueDate = Convert.ToDateTime(nearestList.Rows[0][CN.NearestDueDate]);
                    }
                }
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return nearestDueDate;
        }


        public void LoadRejectionFile(SqlConnection conn, SqlTransaction trans,
            string rejectFileName, string rejectFormatName, DateTime today)
        {
            /* Load the rejections from the bank file and pre-process */
            try
            {
                parmArray = new SqlParameter[3];

                parmArray[0] = new SqlParameter("@piRejectFileName", SqlDbType.NVarChar, 200);
                parmArray[0].Value = rejectFileName;
                parmArray[1] = new SqlParameter("@piRejectFormatName", SqlDbType.NVarChar, 200);
                parmArray[1].Value = rejectFormatName;
                parmArray[2] = new SqlParameter("@piToday", SqlDbType.SmallDateTime);
                parmArray[2].Value = today;

                this.RunSP(conn, trans, "DN_DDLoadRejectionFileSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }


        public void LogUnmatchedRejections(SqlConnection conn, SqlTransaction trans,
            string interfaceName, int runNo)
        {
            /* Log rejection records that do NOT match any payments */
            try
            {
                parmArray = new SqlParameter[2];

                parmArray[0] = new SqlParameter("@piInterface", SqlDbType.VarChar, 12);
                parmArray[0].Value = interfaceName;
                parmArray[1] = new SqlParameter("@piRunNo", SqlDbType.Int);
                parmArray[1].Value = runNo;

                this.RunSP(conn, trans, "DN_DDLogUnmatchedRejectionsSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }


        public void RejectPayments(SqlConnection conn, SqlTransaction trans,
            DateTime today, int leadTime, decimal countryFee)
        {
            /* Reject payments that match rejection records */
            try
            {
                parmArray = new SqlParameter[3];

                parmArray[0] = new SqlParameter("@piToday", SqlDbType.SmallDateTime);
                parmArray[0].Value = today;
                parmArray[1] = new SqlParameter("@piLeadTime", SqlDbType.Int);
                parmArray[1].Value = leadTime;
                parmArray[2] = new SqlParameter("@piCountryFee", SqlDbType.Money);
                parmArray[2].Value = countryFee;

                this.RunSP(conn, trans, "DN_DDRejectPaymentsSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public DataTable FeeOnHoldList(SqlConnection conn, SqlTransaction trans)
        {
            /* List the fees on hold waiting to create their financial transaction */
            DataTable onHoldSet = new DataTable();
            try
            {
                this.RunSP(conn, trans, "DN_DDFeeOnHoldListSP", onHoldSet);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return onHoldSet;
        }


        public void FeeOffHold(SqlConnection conn, SqlTransaction trans,
            int paymentId)
        {
            /* Take the fee payment off hold */
            try
            {
                parmArray = new SqlParameter[1];

                parmArray[0] = new SqlParameter("@piPaymentId", SqlDbType.Int);
                parmArray[0].Value = paymentId;

                this.RunSP(conn, trans, "DN_DDFeeOffHoldSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }


        public DataTable CreditList(SqlConnection conn, SqlTransaction trans,
            DateTime today, int leadTime)
        {
            /* Create credit transactions for all payments that were submitted before 
            ** (today - lead time) and have not been rejected. */
            DataTable creditSet = new DataTable();
            try
            {
                parmArray = new SqlParameter[2];

                parmArray[0] = new SqlParameter("@piToday", SqlDbType.SmallDateTime);
                parmArray[0].Value = today;
                parmArray[1] = new SqlParameter("@piLeadTime", SqlDbType.SmallInt);
                parmArray[1].Value = leadTime;

                this.RunSP(conn, trans, "DN_DDCreditListSP", parmArray, creditSet);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return creditSet;
        }


        public void CompletePayment(SqlConnection conn, SqlTransaction trans,
            int paymentId, string paymentType, string acctNo)
        {
            /* Mark the DD payment as complete and reset the rejection counter
            ** on the mandate for a successful Normal payment or Representation. */
            try
            {
                parmArray = new SqlParameter[3];

                parmArray[0] = new SqlParameter("@piPaymentId", SqlDbType.Int);
                parmArray[0].Value = paymentId;
                parmArray[1] = new SqlParameter("@piPaymentType", SqlDbType.NChar,1);
                parmArray[1].Value = paymentType;
                parmArray[2] = new SqlParameter("@piAcctNo", SqlDbType.Char,12);
                parmArray[2].Value = acctNo;

                this.RunSP(conn, trans, "DN_DDCompletePaymentSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }


        public bool SaveRejectAction(SqlConnection conn, SqlTransaction trans)
        {
            int result = 0;
            bool sessionConflict = false;
            try
            {
                parmArray = new SqlParameter[8];

                parmArray[0] = new SqlParameter("@piRejectAction", SqlDbType.NChar,1);
                parmArray[0].Value = this._rejectAction;
                parmArray[1] = new SqlParameter("@piPaymentId", SqlDbType.Int);
                parmArray[1].Value = this._paymentId;
                parmArray[2] = new SqlParameter("@piMandateId", SqlDbType.Int);
                parmArray[2].Value = this._mandateId;
                parmArray[3] = new SqlParameter("@piPaymentType", SqlDbType.NChar,1);
                parmArray[3].Value = this._paymentType;
                parmArray[4] = new SqlParameter("@piOrigMonth", SqlDbType.SmallInt);
                parmArray[4].Value = this._origMonth;
                parmArray[5] = new SqlParameter("@piAmount", SqlDbType.Decimal);
                parmArray[5].Value = this._amount;
                parmArray[6] = new SqlParameter("@piCurRejectAction", SqlDbType.NChar,1);
                parmArray[6].Value = this._curRejectAction;

                parmArray[7] = new SqlParameter("@poSessionConflict", SqlDbType.SmallInt);
                parmArray[7].Direction = ParameterDirection.Output;

                result = this.RunSP(conn, trans, "DN_DDRejectActionSaveSP", parmArray);

                if (result == 0 && !Convert.IsDBNull(parmArray[7].Value))
                {
                    sessionConflict = Convert.ToBoolean(parmArray[7].Value);
                }
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return sessionConflict;
        }

    }
}

