using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common.Constants.Enums;
using STL.Common.Constants.TableNames;


namespace STL.DAL
{
    /// <summary>
    /// Summary description for DInstalPlan.
    /// </summary>
    public class DInstalPlan : DALObject
    {
        private short _monthsintfree = 0;
        public short MonthsInterestFree
        {
            get { return _monthsintfree; }
            set { _monthsintfree = value; }
        }
        private string _accountno = "";
        public string AccountNumber
        {
            get { return _accountno; }
            set { _accountno = value; }
        }
        private int _agreementNo = 0;
        public int AgreementNumber
        {
            get { return _agreementNo; }
            set { _agreementNo = value; }
        }

        private string _band = "";
        public string Band
        {
            get
            {
                return _band;
            }
            set
            {
                _band = value;
            }
        }

        //IP - 04/03/11 - #3275
        private bool _instalmentWaived = false;
        public bool InstalmentWaived
        {
            get
            {
                return _instalmentWaived;
            }
            set
            {
                _instalmentWaived = value;
            }
        }


        private DateTime _dateFirst = DateTime.MinValue.AddYears(1899);
        public DateTime DateFirst
        {
            get { return _dateFirst; }
            set { _dateFirst = value; }
        }
        private DateTime _dateLast = DateTime.MinValue.AddYears(1899);
        public DateTime DateLast
        {
            get { return _dateLast; }
            set { _dateLast = value; }
        }
        private string _instalfreq = "";
        public string InstalmentFrequency
        {
            get { return _instalfreq; }
            set { _instalfreq = value; }
        }
        private decimal _instalAmount = 0;
        public decimal InstalmentAmount
        {
            get { return _instalAmount; }
            set { _instalAmount = value; }
        }
        private decimal _finalInstal = 0;
        public decimal FinalInstalment
        {
            get { return _finalInstal; }
            set { _finalInstal = value; }
        }
        private int _instalNo = 0;
        public int NumberOfInstalments
        {
            get { return _instalNo; }
            set { _instalNo = value; }
        }
        private short _origBr = 0;
        public short OrigBr
        {
            get { return _origBr; }
            set { _origBr = value; }
        }
        private decimal _instalTotal = 0;
        public decimal InstalTotal
        {
            get { return _instalTotal; }
            set { _instalTotal = value; }
        }

        private DataTable _accounts;
        public DataTable Accounts
        {
            get { return _accounts; }
        }
        private short _dueday = 0;
        public short DueDay
        {
            get { return _dueday; }
            set { _dueday = value; }
        }

        private bool _autoda;
        public bool autoda
        {
            get { return _autoda; }
            set { _autoda = value; }
        }

        private DataTable _amortizedCLSchedule;
        public DataTable AmortizedCLSchedule
        {
            get { return _amortizedCLSchedule; }
        }
        //Arun
        private DataTable _printpromissonary;
        public DataTable Printpromissonary
        {
            get { return _printpromissonary; }
        }

        private decimal _ainstalAmount = 0;
        public decimal AInstalmentAmount
        {
            get { return _ainstalAmount; }
            set { _ainstalAmount = value; }
        }
        private decimal _clamount = 0;
        public decimal ClAmount
        {
            get { return _clamount; }
            set { _clamount = value; }
        }

        private decimal _admnchg = 0;
        public decimal AdmnChg
        {
            get { return _admnchg; }
            set { _admnchg = value; }
        }

        private Int16 _prefDay = 0; 
        public Int16 PrefDay 
        {
            get { return _prefDay; }
            set { _prefDay = value; }
        }


        public void Save(SqlConnection conn, SqlTransaction trans)
        {

            try
            {
                parmArray = new SqlParameter[16];   
                parmArray[0] = new SqlParameter("@origBr", SqlDbType.SmallInt);
                parmArray[0].Value = this.OrigBr;
                parmArray[1] = new SqlParameter("@acctNo", SqlDbType.NVarChar, 12);
                parmArray[1].Value = this.AccountNumber;
                parmArray[2] = new SqlParameter("@agreementNo", SqlDbType.Int);
                parmArray[2].Value = this.AgreementNumber;
                parmArray[3] = new SqlParameter("@dateFirst", SqlDbType.DateTime);
                parmArray[3].Value = this.DateFirst;
                parmArray[4] = new SqlParameter("@instalno", SqlDbType.Int);
                parmArray[4].Value = this.NumberOfInstalments;
                parmArray[5] = new SqlParameter("@instalFreq", SqlDbType.NChar, 1);
                parmArray[5].Value = this.InstalmentFrequency;
                parmArray[6] = new SqlParameter("@dateLast", SqlDbType.DateTime);
                parmArray[6].Value = this.DateLast;
                parmArray[7] = new SqlParameter("@instalAmount", SqlDbType.Money);
                parmArray[7].Value = this.InstalmentAmount;
                parmArray[8] = new SqlParameter("@finalInstal", SqlDbType.Money);
                parmArray[8].Value = this.FinalInstalment;
                parmArray[9] = new SqlParameter("@instalTotal", SqlDbType.Money);
                parmArray[9].Value = this.InstalTotal;
                parmArray[10] = new SqlParameter("@monthIntFree", SqlDbType.SmallInt);
                parmArray[10].Value = this.MonthsInterestFree;
                parmArray[11] = new SqlParameter("@empeenochange", SqlDbType.Int);
                parmArray[11].Value = this.User;
                parmArray[12] = new SqlParameter("@dueday", SqlDbType.SmallInt);
                parmArray[12].Value = this.DueDay;
                parmArray[13] = new SqlParameter("@scoringband", SqlDbType.VarChar, 1);
                parmArray[13].Value = this.Band;
                parmArray[14] = new SqlParameter("@autoda", SqlDbType.Bit);
                parmArray[14].Value = this.autoda;
                // parmArray[15] = new SqlParameter("@MP", SqlDbType.Money);
                parmArray[15] = new SqlParameter("@PrefInstalmentDay", SqlDbType.TinyInt);  
                parmArray[15].Value = this.PrefDay; 

                this.RunSP(conn, trans, "DN_InstalPlanUpdateSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Automatically clears Proposal if exists a check type of DC. Not sure why... 
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        public void AutoDA(SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar, 12);
                parmArray[0].Value = this.AccountNumber;
                parmArray[1] = new SqlParameter("@empeeno", SqlDbType.Int, 16);
                parmArray[1].Value = this.User;

                this.RunSP(conn, trans, "AutoDASP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void Populate(SqlConnection conn, SqlTransaction trans,
                                string accountNo, int agreementNo)
        {
            try
            {
                this.AccountNumber = accountNo;
                this.AgreementNumber = agreementNo;

                parmArray = new SqlParameter[13];
                parmArray[0] = new SqlParameter("@origBr", SqlDbType.SmallInt);
                parmArray[0].Value = 0;
                parmArray[1] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[1].Value = accountNo;
                parmArray[2] = new SqlParameter("@agrmtno", SqlDbType.Int);
                parmArray[2].Value = agreementNo;
                parmArray[3] = new SqlParameter("@datefirst", SqlDbType.DateTime);
                parmArray[3].Value = DBNull.Value;
                parmArray[4] = new SqlParameter("@instalno", SqlDbType.Int);
                parmArray[4].Value = 0;
                parmArray[5] = new SqlParameter("@instalfreq", SqlDbType.NChar, 1);
                parmArray[5].Value = "";
                parmArray[6] = new SqlParameter("@datelast", SqlDbType.DateTime);
                parmArray[6].Value = DBNull.Value;
                parmArray[7] = new SqlParameter("@instalamount", SqlDbType.Money);
                parmArray[7].Value = 0;
                parmArray[8] = new SqlParameter("@finalinstalamt", SqlDbType.Money);
                parmArray[8].Value = 0;
                parmArray[9] = new SqlParameter("@instaltotal", SqlDbType.Money);
                parmArray[9].Value = 0;
                parmArray[10] = new SqlParameter("@mthsintfree", SqlDbType.SmallInt);
                parmArray[10].Value = 0;
                parmArray[11] = new SqlParameter("@scoringband", SqlDbType.VarChar, 4);
                parmArray[11].Value = 0;
                parmArray[12] = new SqlParameter("@instalmentWaived", SqlDbType.Bit); //IP - 04/03/11 - #3275
                parmArray[12].Value = 0;

                foreach (SqlParameter p in parmArray)
                    p.Direction = ParameterDirection.Output;
                parmArray[1].Direction = ParameterDirection.Input;
                parmArray[2].Direction = ParameterDirection.Input;

                if (conn != null && trans != null)
                    result = this.RunSP(conn, trans, "DN_InstalplanPopulateSP", parmArray);
                else
                    result = this.RunSP("DN_InstalplanPopulateSP", parmArray);

                if (result == 0)
                {
                    if (parmArray[0].Value != DBNull.Value)
                        this.OrigBr = (short)parmArray[0].Value;
                    if (parmArray[3].Value != DBNull.Value)
                        this.DateFirst = (DateTime)parmArray[3].Value;
                    if (parmArray[4].Value != DBNull.Value)
                        this.NumberOfInstalments = (int)parmArray[4].Value;
                    if (parmArray[5].Value != DBNull.Value)
                        this.InstalmentFrequency = (string)parmArray[5].Value;
                    if (parmArray[6].Value != DBNull.Value)
                        this.DateLast = (DateTime)parmArray[6].Value;
                    if (parmArray[7].Value != DBNull.Value)
                        this.InstalmentAmount = (decimal)parmArray[7].Value;
                    if (parmArray[8].Value != DBNull.Value)
                        this.FinalInstalment = (decimal)parmArray[8].Value;
                    if (parmArray[9].Value != DBNull.Value)
                        this.InstalTotal = (decimal)parmArray[9].Value;
                    if (parmArray[10].Value != DBNull.Value)
                        this.MonthsInterestFree = (short)parmArray[10].Value;
                    if (parmArray[11].Value != DBNull.Value)
                        this.Band = (string)parmArray[11].Value;
                    if (parmArray[12].Value != DBNull.Value)
                        this.InstalmentWaived = Convert.ToBoolean(parmArray[12].Value);     //IP - 04/03/11 - #3275
                }
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void UpdateDateFirst(SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[0].Value = this.AccountNumber;
                parmArray[1] = new SqlParameter("@datefirst", SqlDbType.DateTime);
                parmArray[1].Value = this.DateFirst;
                parmArray[2] = new SqlParameter("@user", SqlDbType.Int);
                parmArray[2].Value = this.User;

                this.RunSP(conn, trans, "DN_InstalPlanUpdateDateFirstSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void GetAccounts()
        {
            _accounts = new DataTable();

            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[0].Value = this.AccountNumber;

                this.RunSP("DN_GetInstalAcctsSP", parmArray, _accounts);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public DataTable GetAuditData(string accountNo, int rowcount)
        {
            DataTable dt = new DataTable(TN.InstalPlanAudit);

            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[0].Value = accountNo;
                parmArray[1] = new SqlParameter("@rowcount", SqlDbType.Int);
                parmArray[1].Value = rowcount;

                this.RunSP("DN_InstalPlanAuditGetSP", parmArray, dt);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dt;
        }

        public void GetDueDay(SqlConnection conn, SqlTransaction trans, string custID)
        {
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
                parmArray[0].Value = custID;
                parmArray[0].Direction = ParameterDirection.Input;
                parmArray[1] = new SqlParameter("@dueday", SqlDbType.SmallInt);
                parmArray[1].Value = 0;
                parmArray[1].Direction = ParameterDirection.Output;

                this.RunSP(conn, trans, "DN_AccountGetDueDay", parmArray);

                if (parmArray[1].Value != DBNull.Value)
                    this.DueDay = (short)parmArray[1].Value;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }


        public void GetDueDay(string custID)
        {
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
                parmArray[0].Value = custID;
                parmArray[0].Direction = ParameterDirection.Input;
                parmArray[1] = new SqlParameter("@dueday", SqlDbType.SmallInt);
                parmArray[1].Value = 0;
                parmArray[1].Direction = ParameterDirection.Output;

                this.RunSP("DN_AccountGetDueDay", parmArray);

                if (parmArray[1].Value != DBNull.Value)
                    this.DueDay = (short)parmArray[1].Value;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }


        public void SaveVariableInstalments(SqlConnection conn, SqlTransaction trans,
                                            string acctNo, short order, decimal instalment,
                                            short months, DateTime dateFrom, decimal charge)
        {
            try
            {
                parmArray = new SqlParameter[6];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[0].Value = acctNo;
                parmArray[1] = new SqlParameter("@instalorder", SqlDbType.SmallInt);
                parmArray[1].Value = order;
                parmArray[2] = new SqlParameter("@instalment", SqlDbType.Money);
                parmArray[2].Value = instalment;
                parmArray[3] = new SqlParameter("@instalmentnumber", SqlDbType.SmallInt);
                parmArray[3].Value = months;
                parmArray[4] = new SqlParameter("@datefrom", SqlDbType.DateTime);
                parmArray[4].Value = dateFrom;
                parmArray[5] = new SqlParameter("@servicecharge", SqlDbType.Money);
                parmArray[5].Value = charge;

                this.RunSP(conn, trans, "DN_instalmentvariableSaveSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public DataSet GetVariableInstalmentsByAcctNo(string acctNo)
        {
            DataSet instalmentSet = new DataSet(TN.VariableInstal);
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[0].Value = acctNo;

                this.RunSP("DN_instalmentvariableGetbyacctnoSP", parmArray, instalmentSet);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return instalmentSet;
        }

        //Get amortized cash loan schedule to print promissory note
        public void GetAmortizedCLSchedule(string AccountNo, decimal InstalNo)
        {
            _amortizedCLSchedule = new DataTable();

            parmArray = new SqlParameter[1];
            parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
            parmArray[0].Value = AccountNo;

            this.RunSP("DN_GetAmortizedCLSchedule", parmArray, _amortizedCLSchedule);
        }

        //Arun Changes
        public void PrintPromisonaryNote(string AccountNo)
        {
            _printpromissonary = new DataTable();

            parmArray = new SqlParameter[1];
            parmArray[0] = new SqlParameter("@CosacsAcctNo", SqlDbType.NVarChar, 12);
            parmArray[0].Value = AccountNo;

            this.RunSP("SP_BS_SelectAcctInstallment", parmArray, _printpromissonary);


            
        }
        public void DeleteAccountInstallment(string AccountNo)
        {
            _printpromissonary = new DataTable();

            parmArray = new SqlParameter[1];
            parmArray[0] = new SqlParameter("@CosacsAcctNo", SqlDbType.NVarChar, 12);
            parmArray[0].Value = AccountNo;

            this.RunSP("SP_BS_DeleteAcctInstallment", parmArray, _printpromissonary);
        }

        public void SaveAmortizedSchedule(SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                parmArray = new SqlParameter[6];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[0].Value = this.AccountNumber;
                parmArray[1] = new SqlParameter("@instalment", SqlDbType.Money);
                parmArray[1].Value = this.AInstalmentAmount;
                parmArray[2] = new SqlParameter("@term", SqlDbType.Decimal);
                parmArray[2].Value = this.NumberOfInstalments;
                parmArray[3] = new SqlParameter("@scoringband", SqlDbType.VarChar, 1);
                parmArray[3].Value = this.Band;
                parmArray[4] = new SqlParameter("@amount", SqlDbType.Decimal);
                parmArray[4].Value = this.ClAmount;
                parmArray[5] = new SqlParameter("@adminCharge", SqlDbType.Money);
                parmArray[5].Value = this.AdmnChg;
                this.RunSP(conn, trans, "DN_SaveAmortizedCLScheduleSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }



        public DInstalPlan()
        {
            //
            // TODO: Add constructor logic here
            //
        }
    }
}
