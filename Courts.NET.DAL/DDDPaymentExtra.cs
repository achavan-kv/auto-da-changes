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
    /// Summary description for DDDPaymentExtra.
    /// </summary>
    public class DDDPaymentExtra : DALObject
    {
        private int _mandateId;
        private int _paymentId;
        private string _acctNo;
        private string _customerName;
        private decimal _amount;
        private decimal _curAmount;
        private short _origMonth;
        private bool _consent;
        private bool _curConsent;
        private decimal _arrears;
        private decimal _curDDPending;
        private decimal _DDPending;
        private decimal _outStBal;


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

        public decimal curAmount
        {
            get
            {
                return _curAmount;
            }
            set
            {
                _curAmount = value;
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

        public bool consent
        {
            get
            {
                return _consent;
            }
            set
            {
                _consent = value;
            }
        }

        public bool curConsent
        {
            get
            {
                return _curConsent;
            }
            set
            {
                _curConsent = value;
            }
        }
        public decimal arrears
        {
            get
            {
                return _arrears;
            }
            set
            {
                _arrears = value;
            }
        }
        public decimal curDDPending
        {
            get
            {
                return _curDDPending;
            }
            set
            {
                _curDDPending = value;
            }
        }
        public decimal DDPending
        {
            get
            {
                return _DDPending;
            }
            set
            {
                _DDPending = value;
            }
        }
        public decimal outStBal
        {
            get
            {
                return _outStBal;
            }
            set
            {
                _outStBal = value;
            }
        }



        public DDDPaymentExtra()
        {
        }


        public DataSet GetExtraList(DateTime effectiveDate)
        {
            int result = 0;
            DataSet extraPaymentSet = new DataSet();
            try
            {
                parmArray = new SqlParameter[1];

                parmArray[0] = new SqlParameter("@piEffectiveDate", SqlDbType.SmallDateTime);
                parmArray[0].Value = effectiveDate;

                result = this.RunSP("DN_DDPaymentExtraListGetSP", parmArray, extraPaymentSet);

                if (extraPaymentSet != null)
                    if (extraPaymentSet.Tables.Count > 0)
                        extraPaymentSet.Tables[0].TableName = TN.DDPaymentExtra;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            return extraPaymentSet;
        }


        public bool InsertExtraPayment(SqlConnection conn, SqlTransaction trans)
        {
            int result = 0;
            bool sessionConflict = false;
            try
            {
                parmArray = new SqlParameter[3];

                parmArray[0] = new SqlParameter("@piMandateId", SqlDbType.Int);
                parmArray[0].Value = this._mandateId;
                parmArray[1] = new SqlParameter("@piAmount", SqlDbType.Decimal);
                parmArray[1].Value = this._amount;

                parmArray[2] = new SqlParameter("@poSessionConflict", SqlDbType.SmallInt);
                parmArray[2].Direction = ParameterDirection.Output;

                result = this.RunSP(conn, trans, "DN_DDPaymentExtraInsertSP", parmArray);

                if (result == 0 && !Convert.IsDBNull(parmArray[2].Value))
                {
                    sessionConflict = Convert.ToBoolean(parmArray[2].Value);
                }
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return sessionConflict;
        }


        public bool UpdateExtraPayment(SqlConnection conn, SqlTransaction trans)
        {
            int result = 0;
            bool sessionConflict = false;
            try
            {
                parmArray = new SqlParameter[6];

                parmArray[0] = new SqlParameter("@piMandateId", SqlDbType.Int);
                parmArray[0].Value = this._mandateId;
                parmArray[1] = new SqlParameter("@piPaymentId", SqlDbType.Int);
                parmArray[1].Value = this._paymentId;
                parmArray[2] = new SqlParameter("@piCurAmount", SqlDbType.Decimal);
                parmArray[2].Value = this._curAmount;
                parmArray[3] = new SqlParameter("@piOrigMonth", SqlDbType.SmallInt);
                parmArray[3].Value = this._origMonth;
                parmArray[4] = new SqlParameter("@piAmount", SqlDbType.Decimal);
                parmArray[4].Value = this._amount;

                parmArray[5] = new SqlParameter("@poSessionConflict", SqlDbType.SmallInt);
                parmArray[5].Direction = ParameterDirection.Output;

                result = this.RunSP(conn, trans, "DN_DDPaymentExtraUpdateSP", parmArray);

                if (result == 0 && !Convert.IsDBNull(parmArray[5].Value))
                {
                    sessionConflict = Convert.ToBoolean(parmArray[5].Value);
                }
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return sessionConflict;
        }


        public bool DeleteExtraPayment(SqlConnection conn, SqlTransaction trans)
        {
            int result = 0;
            bool sessionConflict = false;
            try
            {
                parmArray = new SqlParameter[5];

                parmArray[0] = new SqlParameter("@piMandateId", SqlDbType.Int);
                parmArray[0].Value = this._mandateId;
                parmArray[1] = new SqlParameter("@piPaymentId", SqlDbType.Int);
                parmArray[1].Value = this._paymentId;
                parmArray[2] = new SqlParameter("@piOrigMonth", SqlDbType.SmallInt);
                parmArray[2].Value = this._origMonth;
                parmArray[3] = new SqlParameter("@piCurAmount", SqlDbType.Decimal);
                parmArray[3].Value = this._curAmount;

                parmArray[4] = new SqlParameter("@poSessionConflict", SqlDbType.SmallInt);
                parmArray[4].Direction = ParameterDirection.Output;

                result = this.RunSP(conn, trans, "DN_DDPaymentExtraDeleteSP", parmArray);

                if (result == 0 && !Convert.IsDBNull(parmArray[4].Value))
                {
                    sessionConflict = Convert.ToBoolean(parmArray[4].Value);
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

