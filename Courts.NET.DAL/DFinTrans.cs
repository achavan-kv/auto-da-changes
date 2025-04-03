using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common;
using STL.Common.Constants.Enums;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;

namespace STL.DAL
{
    /// <summary>
    /// Summary description for DFinTrans.
    /// </summary>
    public class DFinTrans : DALObject
    {
        private DataTable _finTrans;
        private DataSet _transactions;
        private DataSet _transactionsRF;

        private short _origBr = 0;
        private Int32 _FintransId = 0;
        public Int32 FintransId
        {
            get { return _FintransId; }
            set { _FintransId = value; }
        }
        public short OrigBr
        {
            get { return _origBr; }
            set { _origBr = value; }
        }
        private string _acctno = "";
        public string AccountNumber
        {
            get { return _acctno; }
            set { _acctno = value; }
        }
        private DateTime _datetrans = DateTime.Today;
        public DateTime DateTrans
        {
            get { return _datetrans; }
            set { _datetrans = value; }
        }
        private string _transcode = "";
        public string TransTypeCode
        {
            get { return _transcode; }
            set { _transcode = value; }
        }
        private short _branchno = 0;
        public short BranchNumber
        {
            get { return _branchno; }
            set { _branchno = value; }
        }
        private int _transno = 0;
        public int TransRefNo
        {
            get { return _transno; }
            set { _transno = value; }
        }
        private decimal _transvalue = 0;
        public decimal TransValue
        {
            get { return _transvalue; }
            set { _transvalue = value; }
        }
        private int _runno = 0;
        public int RunNumber
        {
            get { return _runno; }
            set { _runno = value; }
        }
        private int _empeeno = 0;
        public int EmployeeNumber
        {
            get { return _empeeno; }
            set { _empeeno = value; }
        }
        private string _transupdated = "";
        public string TransUpdated
        {
            get { return _transupdated; }
            set { _transupdated = value; }
        }
        private string _transprinted = "N";
        public string TransPrinted
        {
            get { return _transprinted; }
            set { _transprinted = value; }
        }
        private string _bankcode = "";
        public string BankCode
        {
            get { return _bankcode; }
            set { _bankcode = value; }
        }
        private string _bankacctno = "";
        public string BankAccountNumber
        {
            get { return _bankacctno; }
            set { _bankacctno = value; }
        }
        private string _chequeno = "";
        public string ChequeNumber
        {
            get { return _chequeno; }
            set { _chequeno = value; }
        }
        private string _ftnotes = "";
        public string FTNotes
        {
            get { return _ftnotes; }
            set { _ftnotes = value; }
        }
        private short _paymethod = 0;
        public short PayMethod
        {
            get { return _paymethod; }
            set { _paymethod = value; }
        }
        private string _source = "";
        public string Source
        {
            get { return _source; }
            set { _source = value; }
        }
        private int _agrmtno = 0;
        public int Agrmtno
        {
            get { return _agrmtno; }
            set { _agrmtno = value; }
        }

        private int? _cashierTotID = 0;                              //IP/JC - 06/01/12 - #8821
        public int? CashierTotID
        {
            get { return _cashierTotID; }
            set { _cashierTotID = value; }
        }

        private string _codeSource = "";                              //IP/JC - 06/01/12 - #8821
        public string CodeSource
        {
            get { return _codeSource; }
            set { _codeSource = value; }
        }

        public DFinTrans()
        {

        }

        public void Write(SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                parmArray = new SqlParameter[21];
                parmArray[0] = new SqlParameter("@origbr", SqlDbType.SmallInt);
                parmArray[0].Value = this.OrigBr;
                parmArray[1] = new SqlParameter("@branchno", SqlDbType.SmallInt);
                parmArray[1].Value = this.BranchNumber;
                parmArray[2] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[2].Value = this.AccountNumber;
                parmArray[3] = new SqlParameter("@transrefno", SqlDbType.Int);
                parmArray[3].Value = this.TransRefNo;
                parmArray[4] = new SqlParameter("@datetrans", SqlDbType.DateTime);
                parmArray[4].Value = this.DateTrans;
                parmArray[5] = new SqlParameter("@transtypecode", SqlDbType.NVarChar, 3);
                parmArray[5].Value = this.TransTypeCode;
                parmArray[6] = new SqlParameter("@empeeno", SqlDbType.Int);
                parmArray[6].Value = this.EmployeeNumber;
                parmArray[7] = new SqlParameter("@transupdated", SqlDbType.NChar, 1);
                parmArray[7].Value = this.TransUpdated;
                parmArray[8] = new SqlParameter("@transprinted", SqlDbType.NChar, 1);
                parmArray[8].Value = this.TransPrinted;
                parmArray[9] = new SqlParameter("@transvalue", SqlDbType.Money);
                parmArray[9].Value = this.TransValue;
                parmArray[10] = new SqlParameter("@bankcode", SqlDbType.NVarChar, 6);
                parmArray[10].Value = this.BankCode;
                parmArray[11] = new SqlParameter("@bankacctno", SqlDbType.NVarChar, 11);
                parmArray[11].Value = this.BankAccountNumber;
                parmArray[12] = new SqlParameter("@chequeno", SqlDbType.NVarChar, 16);
                parmArray[12].Value = this.ChequeNumber;
                parmArray[13] = new SqlParameter("@ftnotes", SqlDbType.NVarChar, 4);
                parmArray[13].Value = this.FTNotes;
                parmArray[14] = new SqlParameter("@paymethod", SqlDbType.SmallInt);
                parmArray[14].Value = this.PayMethod;
                parmArray[15] = new SqlParameter("@runno", SqlDbType.SmallInt);
                parmArray[15].Value = this.RunNumber;
                parmArray[16] = new SqlParameter("@source", SqlDbType.NVarChar, 10);
                parmArray[16].Value = this.Source;
                parmArray[17] = new SqlParameter("@agrmtno", SqlDbType.Int);
                parmArray[17].Value = this.Agrmtno;
                parmArray[18] = new SqlParameter("@cashierTotID", SqlDbType.Int);       //IP/JC - 06/01/12 - #8821
                parmArray[18].Value = this.CashierTotID;
                parmArray[19] = new SqlParameter("@codesource", SqlDbType.VarChar, 10);
                parmArray[19].Value = this.CodeSource;
                parmArray[20] = new SqlParameter("@lastfintransid", SqlDbType.Int);
                parmArray[20].Direction = ParameterDirection.Output;
                this.RunSP(conn, trans, "DN_FinTransWriteSP", parmArray);
                if (parmArray[20].Value != DBNull.Value) this.FintransId = (Int32)parmArray[20].Value;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public string CustomerGetIdByAcctno(string acctno)
        {
            parmArray = new SqlParameter[1];
            parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar, 12);
            parmArray[0].Value = acctno;

            return ReturnString("CustomerGetIdByAcctno", parmArray);
        }
        /// <summary>
        /// Returns details of the financial transactions of an account.
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <returns></returns>
        /// 
        public int GetFinTransDetails(SqlConnection conn, SqlTransaction trans, string accountNumber) //IP - 11/02/11 - Sprint 5.10 - #2978 - Added conn, trans
        {
            try
            {
                _finTrans = new DataTable(TN.FinTrans);
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[0].Value = accountNumber;

                if (conn != null && trans != null)
                    result = this.RunSP(conn, trans, "DN_FintransSumAccountGetSP", parmArray, _finTrans);
                else
                    result = this.RunSP("DN_FintransSumAccountGetSP", parmArray, _finTrans);

                if (result == 0)
                {
                    result = (int)Return.Success;
                }
                else
                {
                    result = (int)Return.Fail;
                }
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;
        }

        public int GetReposessionAndRedelivery(string accountNumber)
        {
            try
            {
                _finTrans = new DataTable(TN.FinTrans);
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[0].Value = accountNumber;

                result = this.RunSP("DN_FintransGetRepRedelSP", parmArray, _finTrans);

                if (result == 0)
                {
                    result = (int)Return.Success;
                }
                else
                {
                    result = (int)Return.Fail;
                }
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;
        }


        /// <summary>
        /// Returns the financial transactions for an account
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <returns></returns>
        /// 
        public int GetByAcctNo(SqlConnection conn, SqlTransaction trans, string accountNumber)
        {
            try
            {
                _finTrans = new DataTable(TN.FinTrans);
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@piAcctNo", SqlDbType.NVarChar, 12);
                parmArray[0].Value = accountNumber;

                if (conn != null && trans != null)
                    result = this.RunSP(conn, trans, "DN_Fintrans_GetByAcctNo", parmArray, _finTrans);
                else
                    result = this.RunSP("DN_Fintrans_GetByAcctNo", parmArray, _finTrans);

                if (result == 0)
                {
                    result = (int)Return.Success;
                }
                else
                {
                    result = (int)Return.Fail;
                }
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;
        }

        // 68237 RD 26/06/06 Added to check the outstanding balance on the account
        public int GetOutstBalByAcctNo(SqlConnection conn, SqlTransaction trans, string accountNumber, out decimal outstbal)
        {
            int status = 0;
            outstbal = 0;
            try
            {

                parmArray = new SqlParameter[2];

                parmArray[0] = new SqlParameter("@piAcctNo", SqlDbType.NVarChar, 12);
                parmArray[0].Value = accountNumber;

                parmArray[1] = new SqlParameter("@outstbal", SqlDbType.Money);
                parmArray[1].Value = outstbal;
                parmArray[1].Direction = ParameterDirection.Output;

                status = this.RunSP("DN_FintransGetOutStBalByAcctNo", parmArray);

                if (parmArray[1].Value != DBNull.Value)
                    outstbal = (decimal)parmArray[1].Value;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            return status;
        }

        public void SetPaymentCardPrinted(SqlConnection conn, SqlTransaction trans, string accountNo,
                                            int transRefNo, DateTime transactionDate, string printed,
                                            int startLine)
        {
            try
            {
                parmArray = new SqlParameter[5];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[0].Value = accountNo;
                parmArray[1] = new SqlParameter("@transrefno", SqlDbType.Int);
                parmArray[1].Value = transRefNo;
                parmArray[2] = new SqlParameter("@transdate", SqlDbType.DateTime);
                parmArray[2].Value = transactionDate;
                parmArray[3] = new SqlParameter("@printed", SqlDbType.NChar, 1);
                parmArray[3].Value = printed;
                parmArray[4] = new SqlParameter("@startline", SqlDbType.Int);
                parmArray[4].Value = startLine;
                this.RunSP(conn, trans, "DN_FintransSetPrintedSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Returns a list of financial transactions for an account,
        /// the total admin fees and interest charged.
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <returns></returns>
        /// 
        public DataSet GetTransactions(string accountNumber)
        {
            try
            {
                _transactions = new DataSet();

                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 15);
                parmArray[0].Value = accountNumber;

                result = this.RunSP("DN_FintransGetDetailsSP", parmArray, _transactions);

                if (result == 0)
                {
                    result = (int)Return.Success;
                    if (_transactions.Tables.Count > 0)
                    {
                        _transactions.Tables[0].TableName = "Transactions";
                        _transactions.Tables[1].TableName = "TotalAdmin";
                        _transactions.Tables[2].TableName = "TotalInterest";
                    }
                }
                else
                    _transactions = null;
                //result = (int)Return.Fail;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return _transactions;
        }


        /// <summary>
        /// Returns a list of financial transactions for a set of delivered,
        /// RF accounts for a customer, grouped by date and transaction type.
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <returns></returns>
        /// 
        public DataSet GetRFGroupedTransactions(string customerID)
        {
            try
            {
                _transactionsRF = new DataSet();

                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@piCustId", SqlDbType.NVarChar, CW.CustomerID);
                parmArray[0].Value = customerID;

                result = this.RunSP("DN_FintransGetRFGroupedTransactionsSP", parmArray, _transactionsRF);

                if (result == 0)
                {
                    result = (int)Return.Success;
                    if (_transactionsRF.Tables.Count > 0)
                    {
                        _transactionsRF.Tables[0].TableName = TN.RFTransactions;
                    }
                }
                else
                    _transactionsRF = null;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return _transactionsRF;
        }

        /// <summary>
        /// Returns a list of financial transactions for a set of delivered,
        /// RF accounts for a customer, as discrete transactons for each account.
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <returns></returns>
        /// 
        public DataSet GetRFCombinedTransactions(string customerID)
        {
            try
            {
                _transactionsRF = new DataSet();

                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@piCustId", SqlDbType.NVarChar, CW.CustomerID);
                parmArray[0].Value = customerID;

                result = this.RunSP("DN_FintransGetRFCombinedTransactionsSP", parmArray, _transactionsRF);

                if (result == 0)
                {
                    result = (int)Return.Success;
                    if (_transactionsRF.Tables.Count > 0)
                    {
                        _transactionsRF.Tables[0].TableName = TN.Transactions;
                    }
                }
                else
                    _transactionsRF = null;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return _transactionsRF;
        }

        public decimal GetRebate(SqlConnection conn, SqlTransaction trans, string accountNo)
        {
            decimal rebate = 0;
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[0].Value = accountNo;
                parmArray[1] = new SqlParameter("@rebate", SqlDbType.Money);
                parmArray[1].Value = 0;
                parmArray[1].Direction = ParameterDirection.Output;

                if (conn != null && trans != null)
                    RunSP(conn, trans, "DN_FintransGetRebateSP", parmArray);
                else
                    RunSP("DN_FintransGetRebateSP", parmArray);

                if (parmArray[1].Value != DBNull.Value)
                    rebate = (decimal)parmArray[1].Value;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return rebate;
        }

        public string GetAddedToAccount(SqlConnection conn, SqlTransaction trans,
                                            string accountNo, decimal addToValue)
        {
            string addedTo = "";
            try
            {
                parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[0].Value = accountNo;
                parmArray[1] = new SqlParameter("@addToValue", SqlDbType.Money);
                parmArray[1].Value = addToValue;
                parmArray[2] = new SqlParameter("@addedTo", SqlDbType.NVarChar, 12);
                parmArray[2].Value = "";
                parmArray[2].Direction = ParameterDirection.Output;

                if (conn != null && trans != null)
                    RunSP(conn, trans, "DN_FintransGetAddedToAccountSP", parmArray);
                else
                    RunSP("DN_FintransGetAddedToAccountSP", parmArray);

                if (parmArray[2].Value != DBNull.Value)
                    addedTo = (string)parmArray[2].Value;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return addedTo;
        }


        public DataTable FinTrans
        {
            get
            {
                return _finTrans;
            }
        }

        public DataSet Transactions
        {
            get
            {
                return _transactions;
            }
        }

        public DataSet TransactionsRF
        {
            get
            {
                return _transactionsRF;
            }
        }


        /// <summary>
        /// GetCashierTotals
        /// </summary>
        /// <param name="byBranch">int</param>
        /// <param name="branchno">int</param>
        /// <param name="employeeno">int</param>
        /// <param name="datefrom">DateTime</param>
        /// <param name="dateto">DateTime</param>
        /// <returns>DataSet</returns>
        /// 
        public DataTable GetCashierTotals(bool byBranch, short branchno, int employeeno, DateTime datefrom, DateTime dateto, bool listCheques, out decimal total)
        {
            DataTable dt = new DataTable(TN.CashierTotals);
            total = 0;

            try
            {
                parmArray = new SqlParameter[7];

                parmArray[0] = new SqlParameter("@byBranch", SqlDbType.SmallInt);
                parmArray[0].Value = Convert.ToInt16(byBranch);

                parmArray[1] = new SqlParameter("@branchno", SqlDbType.SmallInt);
                parmArray[1].Value = branchno;

                parmArray[2] = new SqlParameter("@employeeno", SqlDbType.Int);
                parmArray[2].Value = employeeno;

                parmArray[3] = new SqlParameter("@datefrom", SqlDbType.DateTime);
                parmArray[3].Value = datefrom;

                parmArray[4] = new SqlParameter("@dateto", SqlDbType.DateTime);
                parmArray[4].Value = dateto;

                parmArray[5] = new SqlParameter("@listcheques", SqlDbType.SmallInt);
                parmArray[5].Value = Convert.ToBoolean(listCheques);

                parmArray[6] = new SqlParameter("@total", SqlDbType.Money);
                parmArray[6].Value = 0;
                parmArray[6].Direction = ParameterDirection.Output;

                this.RunSP("DN_FintransGetCashierTotalsSP", parmArray, dt);

                if (parmArray[6].Value != DBNull.Value)
                    total = (decimal)parmArray[6].Value;

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            return dt;
        }

        /// <summary>
        /// GetChequeDetails
        /// </summary>
        /// <param name="chequeno">string</param>
        /// <param name="bankcode">string</param>
        /// <param name="bankacctno">string</param>
        /// <param name="datefrom">DateTime</param>
        /// <param name="dateto">DateTime</param>
        /// <returns>DataSet</returns>
        /// 
        public DataSet GetChequeDetails(string chequeno, string bankcode, string bankacctno, DateTime datefrom, DateTime dateto)
        {
            DataSet ds = new DataSet();

            try
            {
                parmArray = new SqlParameter[5];

                parmArray[0] = new SqlParameter("@chequeno", SqlDbType.NVarChar, 32);
                parmArray[0].Value = chequeno;

                parmArray[1] = new SqlParameter("@bankcode", SqlDbType.NVarChar, 12);
                parmArray[1].Value = bankcode;

                parmArray[2] = new SqlParameter("@bankacctno", SqlDbType.NVarChar, 40);
                parmArray[2].Value = bankacctno;

                parmArray[3] = new SqlParameter("@datefrom", SqlDbType.DateTime);
                parmArray[3].Value = datefrom;

                parmArray[4] = new SqlParameter("@dateto", SqlDbType.DateTime);
                parmArray[4].Value = dateto;

                this.RunSP("DN_FintransGetChequeDetailsSP", parmArray, ds);


            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            return ds;
        }

        /// <summary>
        /// GetAccountPayments
        /// </summary>
        /// <param name="acctno">string</param>
        /// <returns>DataSet</returns>
        /// 
        public DataSet GetAccountPayments(string acctno)
        {
            DataSet ds = new DataSet();


            try
            {
                parmArray = new SqlParameter[1];

                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 24);
                parmArray[0].Value = acctno;

                this.RunSP("DN_FintransGetAccountPaymentsSP", parmArray, ds);


            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            return ds;
        }

        /// <summary>
        /// GetAmountPaid
        /// </summary>
        /// <param name="acctno">string</param>
        /// <param name="amount">double</param>
        /// <returns>decimal</returns>
        /// 
        public decimal GetAmountPaid(string acctno)
        {
            decimal amount = 0;

            try
            {
                parmArray = new SqlParameter[2];

                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 24);
                parmArray[0].Value = acctno;

                parmArray[1] = new SqlParameter("@amount", SqlDbType.Money);
                parmArray[1].Value = amount;
                parmArray[1].Direction = ParameterDirection.Output;

                this.RunSP("DN_FintransGetAmountPaidSP", parmArray);

                if (parmArray[1].Value != DBNull.Value)
                    amount = (decimal)parmArray[1].Value;

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return amount;
        }

        /// <summary>
        /// HasCashierTotalled
        /// </summary>
        /// <param name="empeeno">int</param>
        /// <param name="branchno">int</param>
        /// <param name="nottotalled">int</param>
        /// <returns>bool</returns>
        /// 
        public bool HasCashierTotalled(int empeeno)
        {
            bool hasTotalled = false;
            try
            {
                parmArray = new SqlParameter[2];

                parmArray[0] = new SqlParameter("@empeeno", SqlDbType.Int);
                parmArray[0].Value = empeeno;

                parmArray[1] = new SqlParameter("@hastotalled", SqlDbType.NChar, 1);
                parmArray[1].Value = 0;
                parmArray[1].Direction = ParameterDirection.Output;

                this.RunSP("DN_FintransHasCashierTotalledSP", parmArray);

                if (parmArray[1].Value != DBNull.Value)
                    hasTotalled = ((string)(parmArray[1].Value) == "Y");

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return hasTotalled;
        }

        /// <summary>
        /// GetAccountsAddedTo
        /// </summary>
        /// <param name="acctno">string</param>
        /// <returns>DataTable</returns>
        /// 
        public DataTable GetAccountsAddedTo(SqlConnection conn, SqlTransaction trans, string acctno)
        {
            DataTable dt = new DataTable();
            try
            {
                parmArray = new SqlParameter[1];

                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[0].Value = acctno;

                if (conn != null && trans != null)
                    this.RunSP(conn, trans, "DN_FintransGetAccountsAddedToSP", parmArray, dt);
                else
                    this.RunSP("DN_FintransGetAccountsAddedToSP", parmArray, dt);


            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dt;
        }

        /// <summary>
        /// GetAllRFTransactions
        /// </summary>
        /// <param name="custid">string</param>
        /// <returns>DataTable</returns>
        /// 
        public DataTable GetAllRFTransactions(string custid)
        {
            DataTable dt = new DataTable(TN.Transactions);
            try
            {
                parmArray = new SqlParameter[1];

                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, 40);
                parmArray[0].Value = custid;

                this.RunSP("DN_FintransGetAllRFTransactionsSP", parmArray, dt);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dt;
        }

        public DataTable GetBreakDownForEmployee(int employeeno, DateTime datefrom, DateTime dateto)
        {
            DataTable dt = new DataTable(TN.CashierTotals);

            try
            {
                parmArray = new SqlParameter[3];

                parmArray[0] = new SqlParameter("@employeeno", SqlDbType.Int);
                parmArray[0].Value = employeeno;

                parmArray[1] = new SqlParameter("@datefrom", SqlDbType.DateTime);
                parmArray[1].Value = datefrom;

                parmArray[2] = new SqlParameter("@dateto", SqlDbType.DateTime);
                parmArray[2].Value = dateto;

                this.RunSP("DN_CashierTotalsBreakdownGetForEmployeeSP", parmArray, dt);

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            return dt;
        }

        /// <summary>
        /// GetForTransfer
        /// </summary>
        /// <param name="acctno">string</param>
        /// <returns>DataSet</returns>
        /// 
        public DataSet GetForTransfer(string acctno, DateTime beforeDate, bool limitRows, out decimal availableTransfer)
        {
            DataSet ds = new DataSet();
            availableTransfer = 0;

            try
            {
                parmArray = new SqlParameter[4];

                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 24);
                parmArray[0].Value = acctno;

                parmArray[1] = new SqlParameter("@before", SqlDbType.DateTime);
                parmArray[1].Value = beforeDate;

                parmArray[2] = new SqlParameter("@availableTransfer", SqlDbType.Money);
                parmArray[2].Direction = ParameterDirection.Output;

                parmArray[3] = new SqlParameter("@limitrows", SqlDbType.Bit);
                parmArray[3].Value = limitRows;

                this.RunSP("DN_FintransGetForTransferSP", parmArray, ds);

                if (parmArray[2].Value != DBNull.Value)
                    availableTransfer = (decimal)parmArray[2].Value;

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            return ds;
        }

        public void JournalEnquiryGet(DateTime datefirst, DateTime datelast,
                                         int firstrefno, int lastrefno, int empeeno,
                                         int branch, int combination)
        {
            //_finTrans = new DataTable(TN.FinTrans);
            _transactions = new DataSet();

            try
            {
                parmArray = new SqlParameter[7];

                parmArray[0] = new SqlParameter("@datefirst", SqlDbType.DateTime);
                parmArray[0].Value = datefirst;
                parmArray[1] = new SqlParameter("@datelast", SqlDbType.DateTime);
                parmArray[1].Value = datelast;
                parmArray[2] = new SqlParameter("@firstrefno", SqlDbType.Int);
                parmArray[2].Value = firstrefno;
                parmArray[3] = new SqlParameter("@lastrefno", SqlDbType.Int);
                parmArray[3].Value = lastrefno;
                parmArray[4] = new SqlParameter("@empeeno", SqlDbType.Int);
                parmArray[4].Value = empeeno;
                parmArray[5] = new SqlParameter("@branch", SqlDbType.Int);
                parmArray[5].Value = branch;
                parmArray[6] = new SqlParameter("@combination", SqlDbType.Int);
                parmArray[6].Value = combination;


                this.RunSP("DN_JournalEnquiryGetSP", parmArray, _transactions);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void GetFinTransQueryResults(DateTime datestart,
                                            DateTime datefinish,
                                            string transtypeoperand,
                                            string transtypecode,
                                            string valueoperand,
                                            int startrunno,
                                            int endrunno,
                                            string runnooperand,
                                            int startrefno,
                                            int endrefno,
                                            string refnooperand,
                                            int empeeno,
                                            string empeenooperand,
                                            int branchno,
                                            string branchnooperand,
                                            int accountinbranch,
                                            string dateoperand,
                                            string branchsetname,
                                            string transtypesetname,
                                            string employeesetname,
                                            int valueonly,
                                            int includeothercharges)
        {
            _finTrans = new DataTable(TN.FinTrans);

            try
            {
                parmArray = new SqlParameter[22];
                parmArray[0] = new SqlParameter("@transtypeoperand", SqlDbType.VarChar);
                parmArray[0].Value = transtypeoperand;
                parmArray[1] = new SqlParameter("@transtypecode", SqlDbType.VarChar);
                parmArray[1].Value = transtypecode;
                parmArray[2] = new SqlParameter("@valueoperand", SqlDbType.VarChar);
                parmArray[2].Value = valueoperand;
                parmArray[3] = new SqlParameter("@startrunno", SqlDbType.Int);
                parmArray[3].Value = startrunno;
                parmArray[4] = new SqlParameter("@endrunno", SqlDbType.Int);
                parmArray[4].Value = endrunno;
                parmArray[5] = new SqlParameter("@runnooperand", SqlDbType.VarChar);
                parmArray[5].Value = runnooperand;
                parmArray[6] = new SqlParameter("@startrefno", SqlDbType.Int);
                parmArray[6].Value = startrefno;
                parmArray[7] = new SqlParameter("@endrefno", SqlDbType.Int);
                parmArray[7].Value = endrefno;
                parmArray[8] = new SqlParameter("@refnooperand", SqlDbType.VarChar);
                parmArray[8].Value = refnooperand;
                parmArray[9] = new SqlParameter("@empeeno", SqlDbType.Int);
                parmArray[9].Value = empeeno;
                parmArray[10] = new SqlParameter("@empeenooperand", SqlDbType.VarChar);
                parmArray[10].Value = empeenooperand;
                parmArray[11] = new SqlParameter("@branchno", SqlDbType.Int);
                parmArray[11].Value = branchno;
                parmArray[12] = new SqlParameter("@branchnooperand", SqlDbType.VarChar);
                parmArray[12].Value = branchnooperand;
                parmArray[13] = new SqlParameter("@accountinbranch", SqlDbType.Int);
                parmArray[13].Value = accountinbranch;
                parmArray[14] = new SqlParameter("@datestart", SqlDbType.DateTime);
                parmArray[14].Value = datestart;
                parmArray[15] = new SqlParameter("@datefinish", SqlDbType.DateTime);
                parmArray[15].Value = datefinish;
                parmArray[16] = new SqlParameter("@dateoperand", SqlDbType.VarChar);
                parmArray[16].Value = dateoperand;
                parmArray[17] = new SqlParameter("@branchsetname", SqlDbType.VarChar);
                parmArray[17].Value = branchsetname;
                parmArray[18] = new SqlParameter("@transtypesetname", SqlDbType.VarChar);
                parmArray[18].Value = transtypesetname;
                parmArray[19] = new SqlParameter("@employeesetname", SqlDbType.VarChar);
                parmArray[19].Value = employeesetname;
                parmArray[20] = new SqlParameter("@valueonly", SqlDbType.Int);
                parmArray[20].Value = valueonly;
                parmArray[21] = new SqlParameter("@includeothercharges", SqlDbType.Int);
                parmArray[21].Value = includeothercharges;

                this.RunSP("DN_FinTransQuerySP", parmArray, _finTrans);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }


        /// <summary>
        /// GetSundryTotal
        /// </summary>
        /// <param name="branchno">int</param>
        /// <param name="before">DateTime</param>
        /// <param name="total">double</param>
        /// <returns>int</returns>
        /// 
        public int GetSundryTotal(int branchno, DateTime before, out decimal total)
        {
            int status = 0;
            total = 0;
            try
            {
                parmArray = new SqlParameter[3];

                parmArray[0] = new SqlParameter("@branchno", SqlDbType.SmallInt);
                parmArray[0].Value = branchno;

                parmArray[1] = new SqlParameter("@before", SqlDbType.DateTime);
                parmArray[1].Value = before;

                parmArray[2] = new SqlParameter("@total", SqlDbType.Money);
                parmArray[2].Value = total;
                parmArray[2].Direction = ParameterDirection.Output;

                status = this.RunSP("DN_FintransGetSundryTotalSP", parmArray);

                if (parmArray[2].Value != DBNull.Value)
                    total = (decimal)parmArray[2].Value;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            return status;
        }

        public string GetGiftVoucherReference(string acctNoRedeemed, int refNo)
        {
            string reference = "";
            try
            {
                parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@acctnoredeemed", SqlDbType.NVarChar, 12);
                parmArray[0].Value = acctNoRedeemed;
                parmArray[1] = new SqlParameter("@refno", SqlDbType.Int);
                parmArray[1].Value = refNo;
                parmArray[2] = new SqlParameter("@reference", SqlDbType.NVarChar, 20);
                parmArray[2].Value = reference;
                parmArray[2].Direction = ParameterDirection.Output;

                result = this.RunSP("DN_GiftVoucherReferenceGetSP", parmArray);

                if (parmArray[2].Value != DBNull.Value)
                    reference = (string)parmArray[2].Value;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return reference;
        }

        public DataTable GetCashierTotalsForPrint(bool byBranch, short branchno, int employeeno, DateTime datefrom, DateTime dateto, bool listCheques, out decimal total)
        {
            DataTable dt = new DataTable(TN.CashierTotals);
            total = 0;

            try
            {
                parmArray = new SqlParameter[7];

                parmArray[0] = new SqlParameter("@byBranch", SqlDbType.SmallInt);
                parmArray[0].Value = Convert.ToInt16(byBranch);

                parmArray[1] = new SqlParameter("@branchno", SqlDbType.SmallInt);
                parmArray[1].Value = branchno;

                parmArray[2] = new SqlParameter("@employeeno", SqlDbType.Int);
                parmArray[2].Value = employeeno;

                parmArray[3] = new SqlParameter("@datefrom", SqlDbType.DateTime);
                parmArray[3].Value = datefrom;

                parmArray[4] = new SqlParameter("@dateto", SqlDbType.DateTime);
                parmArray[4].Value = dateto;

                parmArray[5] = new SqlParameter("@listcheques", SqlDbType.SmallInt);
                parmArray[5].Value = Convert.ToBoolean(listCheques);

                parmArray[6] = new SqlParameter("@total", SqlDbType.Money);
                parmArray[6].Value = 0;
                parmArray[6].Direction = ParameterDirection.Output;

                this.RunSP("DN_FintransGetCashierTotalsForPrintSP", parmArray, dt);

                if (parmArray[6].Value != DBNull.Value)
                    total = (decimal)parmArray[6].Value;

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            return dt;
        }
        /// <summary>
        /// populates datatable Fintrans also outputs privileCount
        /// </summary>
        /// <param name="acctNo"></param>
        /// <param name="privilegeCount"></param>
        public void GetDetailsForDebtCollector(string acctNo, out int privilegeCount)
        {
            _finTrans = new DataTable(TN.FinTrans);
            privilegeCount = 0;

            try
            {
                parmArray = new SqlParameter[2];

                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[0].Value = acctNo;
                parmArray[1] = new SqlParameter("@privilege", SqlDbType.Int);
                parmArray[1].Value = 0;
                parmArray[1].Direction = ParameterDirection.Output;

                this.RunSP("DN_FintransGetForDebtCollectorSP", parmArray, _finTrans);

                if (parmArray[1].Value != DBNull.Value)
                    privilegeCount = (int)parmArray[1].Value;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public DataTable SUCBGetFinancialTotals(int runno)
        {
            _finTrans = new DataTable(TN.FinTrans);

            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@runno", SqlDbType.Int);
                parmArray[0].Value = runno;

                this.RunSP("DN_SUCBGetFinancialTotalsSP", parmArray, _finTrans);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            return _finTrans;
        }

        public DataTable SUCBGetFinancialDetails(int runno, SqlConnection conn, SqlTransaction trans)
        {
            _finTrans = new DataTable(TN.Transactions);

            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@runno", SqlDbType.Int);
                parmArray[0].Value = runno;

                this.RunSP(conn, trans, "DN_SUCBGetFinancialDetailsSP", parmArray, _finTrans);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            return _finTrans;
        }

        public decimal GetWarrantyAdjustment(SqlConnection conn, SqlTransaction trans, string acctno)
        {
            decimal amount = 0;

            try
            {
                parmArray = new SqlParameter[2];

                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 24);
                parmArray[0].Value = acctno;

                parmArray[1] = new SqlParameter("@amount", SqlDbType.Money);
                parmArray[1].Value = amount;
                parmArray[1].Direction = ParameterDirection.Output;

                if (conn != null && trans != null)
                    this.RunSP(conn, trans, "DN_FintransGetWarrantyAdjustmentSP", parmArray);
                else
                    this.RunSP("DN_FintransGetWarrantyAdjustmentSP", parmArray);

                if (parmArray[1].Value != DBNull.Value)
                    amount = (decimal)parmArray[1].Value;

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return amount;
        }

        // 67825 RD 14/03/2006 Added to settle cash account with 0 agreement total
        public bool GetCashAcctWith0Agrmttotal(SqlConnection conn, SqlTransaction trans, string accountNumber)
        {
            bool isCashZeroAgrmt = false;
            try
            {

                parmArray = new SqlParameter[2];

                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[0].Value = accountNumber;

                parmArray[1] = new SqlParameter("@isCashZeroAgrmt", SqlDbType.NChar, 1);
                parmArray[1].Value = 0;
                parmArray[1].Direction = ParameterDirection.Output;

                this.RunSP(conn, trans, "DN_CashAcctWith0AgrmttotalSP", parmArray);

                if (parmArray[1].Value != DBNull.Value)
                    isCashZeroAgrmt = ((string)(parmArray[1].Value) == "Y");

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return isCashZeroAgrmt;
        }


        public void SaveFreeInstalment(SqlConnection conn, SqlTransaction trans,
            string accountNumber, short branchNo, DateTime timeToday, decimal amount, bool giftVoucher)
        {
            try
            {
                parmArray = new SqlParameter[5];
                parmArray[0] = new SqlParameter("@acctNo", SqlDbType.Char, 12);
                parmArray[0].Value = accountNumber;
                parmArray[1] = new SqlParameter("@branchNo", SqlDbType.Int);
                parmArray[1].Value = branchNo;
                parmArray[2] = new SqlParameter("@dateIssued", SqlDbType.DateTime);
                parmArray[2].Value = timeToday;
                parmArray[3] = new SqlParameter("@amount", SqlDbType.Money);
                parmArray[3].Value = amount;
                parmArray[4] = new SqlParameter("@giftVoucher", SqlDbType.Char, 1);
                parmArray[4].Value = (giftVoucher ? "Y" : "N");

                this.RunSP(conn, trans, "DN_FreeInstalmentSaveSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public bool CheckForCommittedData(string acctno, int transRefNo)
        {
            bool dataCommitted = true;
            int result = 0;
            try
            {
                parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.Char, 12);
                parmArray[0].Value = acctno;
                parmArray[1] = new SqlParameter("@transrefno", SqlDbType.Int);
                parmArray[1].Value = transRefNo;
                parmArray[2] = new SqlParameter("@datetrans", SqlDbType.DateTime);
                parmArray[2].Value = DateTime.Today;

                result = this.RunSPwithExecuteScalar("FinTransCheckForCommittedDataSP", parmArray);
                dataCommitted = (result == 1) ? true : false;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dataCommitted;
        }
        // uat376 rdb BDW Reversal
        public DataTable GetBDWTransactions(SqlConnection conn, SqlTransaction trans, string acctno)
        {
            DataTable finTrans = new DataTable("BDWTransactions");

            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar, 12);
                parmArray[0].Value = acctno;

                this.RunSP(conn, trans, "DN_SelectBDWTransactions", parmArray, finTrans);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            return finTrans;
        }

        #region CLA Outstanding Balance

        // CR       :  CLA outstanding Balance Calculation
        // Author   :  Rahul D
        // Details  :  function for amount decuction if the account is created on AmortizedOutStandingBal

        /// <summary>
        /// Function will deduct amount passed in sequence for account provided
        /// </summary>
        /// <param name="con">SQL Connection</param>
        /// <param name="tran">SQL Transaction</param>
        /// <param name="acctno">Account no from which amount to be deducted</param>
        /// <param name="amount">Amount to be deducted from account</param>
        /// <returns>true if operation success, else false</returns>
        public bool DeductAmountFromPaymentHistory(SqlConnection con, SqlTransaction tran, string acctno, decimal amount)
        {
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar, 12);
                parmArray[0].Value = acctno;
                parmArray[1] = new SqlParameter("@amount", SqlDbType.Money);
                parmArray[1].Value = amount;

                this.RunSP(con, tran, "CLAmortizationUpdateAccountBalance", parmArray);
                return true;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }


        }

        // CR       :  CLA outstanding Balance Calculation
        // Author   :  Kedar M
        // Details  :  function for amount reversal if the account is created on AmortizedOutStandingBal

        /// <summary>
        /// Function will reverse  amount credited  to account
        /// </summary>
        /// <param name="con">SQL Connection</param>
        /// <param name="tran">SQL Transaction</param>
        /// <param name="acctno">Account no from which amount to be reversed </param>
        /// <param name="amount">Amount to be Reversed  from account</param>
        /// <returns>true if operation success, else false</returns>
        public bool CLAmortizationReversePayment(SqlConnection con, SqlTransaction tran, string acctno, decimal amount)
        {
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar, 12);
                parmArray[0].Value = acctno;
                parmArray[1] = new SqlParameter("@amount", SqlDbType.Money);
                parmArray[1].Value = amount;

                this.RunSP(con, tran, "CLAmortizationReversePayment", parmArray);
                return true;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// GeneralFinanceTransaction For Cash Loan Account
        /// </summary>
        /// <param name="conn">SQL Connection</param>
        /// <param name="trans">SQL transaction</param>
        /// <param name="acctno">Account number : String</param>
        /// <param name="amount">Amount of transaction : Decimal</param>
        /// <param name="creditDebit">For Credit : -1, Debit : 1</param>
        /// <param name="transtype">Trans type code for Cash loan GFT</param>
        public void CLGeneralFinanceTransaction(SqlConnection conn, SqlTransaction trans, string acctno, decimal amount, int creditDebit, string transtype)
        {
            try
            {
                parmArray = new SqlParameter[4];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar, 12);
                parmArray[0].Value = acctno;
                parmArray[1] = new SqlParameter("@amount", SqlDbType.Money);
                parmArray[1].Value = amount;
                parmArray[2] = new SqlParameter("@creditDebit", SqlDbType.Int);
                parmArray[2].Value = creditDebit;
                parmArray[3] = new SqlParameter("@transtype", SqlDbType.VarChar, 5);
                parmArray[3].Value = transtype;
                this.RunSP("CLAmortizationGFTTransactions", parmArray);

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }
        // CR       :  CLA outstanding Balance Calculation
        // Author   :  Rahul D, Zensar
        // Details  :  Function to get max amount to be credited or debited for validation
        /// <summary>
        /// Function to get max amount to be credited or debited for validation
        /// </summary>
        /// <param name="conn">SQL Connection</param>
        /// <param name="trans">SQL Transation</param>
        /// <param name="acctno">Account number : String</param>
        /// <param name="amount">Amount : Decimal</param>
        /// <param name="creditDebit">For Debit 1, for credit -1</param>
        /// <param name="transtype">Trans type code</param>
        public int CLGeneralFinanceTransactionValidation(SqlConnection conn, SqlTransaction trans, string acctno, out decimal amount, int creditDebit, string transtype)
        {
            amount = 0;
            parmArray = new SqlParameter[4];
            parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar, 12);
            parmArray[0].Value = acctno;
            parmArray[1] = new SqlParameter("@transtypeCode", SqlDbType.VarChar, 5);
            parmArray[1].Value = transtype;
            parmArray[2] = new SqlParameter("@creditDebit", SqlDbType.Int);
            parmArray[2].Value = creditDebit;
            parmArray[3] = new SqlParameter("@amount", SqlDbType.Money);
            parmArray[3].Value = amount;
            parmArray[3].Direction = ParameterDirection.Output;

            int result = this.RunSP("CLAmortizationGFTValidation", parmArray);

            if (parmArray[3].Value != DBNull.Value)
                amount = (decimal)parmArray[3].Value;
            else
                amount = 0M;

            return result;
        }
        public void GetOutstandingAndCharges(string acctno, out decimal outstanding, out decimal bdCharges)
        {
            outstanding = 0;
            bdCharges = 0;
            parmArray = new SqlParameter[3];
            parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar, 12);
            parmArray[0].Value = acctno;
            parmArray[1] = new SqlParameter("@outstandingBalance", SqlDbType.Money);
            parmArray[1].Value = outstanding;
            parmArray[1].Direction = ParameterDirection.Output;
            parmArray[2] = new SqlParameter("@bdCharges", SqlDbType.Money);
            parmArray[2].Value = bdCharges;
            parmArray[2].Direction = ParameterDirection.Output;

            int result = this.RunSP("CLAmortizationUpdateAcctAfterGFT", parmArray);

            if (parmArray[1].Value != DBNull.Value)
                outstanding = (decimal)parmArray[1].Value;
            else
                outstanding = 0M;
            if (parmArray[2].Value != DBNull.Value)
                bdCharges = (decimal)parmArray[2].Value;
            else
                bdCharges = 0M;
        }
        #endregion
    }
}
