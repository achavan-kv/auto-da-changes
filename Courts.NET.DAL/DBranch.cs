using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common;
using STL.Common.Constants.Enums;
using STL.Common.Constants.TableNames;
using System.Diagnostics;


namespace STL.DAL
{
    /// <summary>
    /// Summary description for DBranch.
    /// </summary>
    public class DBranch : DALObject
    {
        DataTable _branchnos;

        private short _branchNo = 0;
        public short BranchNo
        {
            get { return _branchNo; }
            set { _branchNo = value; }
        }

        private string _branchname = "";
        public string BranchName
        {
            get { return _branchname; }
            set { _branchname = value; }
        }

        private string _branchaddr1 = "";
        public string BranchAddr1
        {
            get { return _branchaddr1; }
            set { _branchaddr1 = value; }
        }

        private string _branchaddr2 = "";
        public string BranchAddr2
        {
            get { return _branchaddr2; }
            set { _branchaddr2 = value; }
        }

        private string _branchaddr3 = "";
        public string BranchAddr3
        {
            get { return _branchaddr3; }
            set { _branchaddr3 = value; }
        }

        private string _postCode = "";
        public string PostCode
        {
            get { return _postCode; }
            set { _postCode = value; }
        }

        private string _telephoneNo = "";
        public string TelephoneNo
        {
            get { return _telephoneNo; }
            set { _telephoneNo = value; }
        }

        private double _servicepcent = 0;
        public double ServicePCent
        {
            get { return _servicepcent; }
            set { _servicepcent = value; }
        }

        private string _countryCode = "";
        public string CountryCode
        {
            get { return _countryCode; }
            set { _countryCode = value; }
        }

        private int _croffNo = 0;
        public int CroffNo
        {
            get { return _croffNo; }
            set { _croffNo = value; }
        }

        private DateTime _daterun;
        public DateTime DateRun
        {
            get { return _daterun; }
            set { _daterun = value; }
        }

        private int _weekNo = 0;
        public int WeekNo
        {
            get { return _weekNo; }
            set { _weekNo = value; }
        }

        private string _oldpcType = "";
        public string OldPCType
        {
            get { return _oldpcType; }
            set { _oldpcType = value; }
        }

        private string _newpcType = "";
        public string NewPCType
        {
            get { return _newpcType; }
            set { _newpcType = value; }
        }

        private DateTime _datepcchange;
        public DateTime DatePCChange
        {
            get { return _datepcchange; }
            set { _datepcchange = value; }
        }

        private int _batchControlNo = 0;
        public int BatchControlNo
        {
            get { return _batchControlNo; }
            set { _batchControlNo = value; }
        }

        private int _hissn = 0;
        public int Hissn
        {
            get { return _hissn; }
            set { _hissn = value; }
        }

        private int _hibuffno = 0;
        public int HiBuffNo
        {
            get { return _hibuffno; }
            set { _hibuffno = value; }
        }

        private string _warehouseNo = "";
        public string WarehouseNo
        {
            get { return _warehouseNo; }
            set { _warehouseNo = value; }
        }

        private string _as400exp = "";
        public string AS400Exp
        {
            get { return _as400exp; }
            set { _as400exp = value; }
        }

        private int _hirefno = 0;
        public int HiRefNo
        {
            get { return _hirefno; }
            set { _hirefno = value; }
        }

        private double _as400branch = 0;
        public double AS400BranchNo
        {
            get { return _as400branch; }
            set { _as400branch = value; }
        }

        private int _buffno = 0;
        public int BuffNo
        {
            get { return _buffno; }
            set { _buffno = value; }
        }

        private int _codreceipt = 0;
        public int CODReceipt
        {
            get { return _codreceipt; }
            set { _codreceipt = value; }
        }

        private string _region = "";
        public string Region
        {
            get { return _region; }
            set { _region = value; }
        }

        private short _serviceLocn = 0;
        public short ServiceLocation
        {
            get { return _serviceLocn; }
            set { _serviceLocn = value; }
        }


        public int GetAllBranchNos()
        {
            try
            {
                _branchnos = new DataTable(TN.BranchNumber);
                result = this.RunSP("DN_BranchNosGetAllSP", _branchnos);

                if (result == 0)
                    result = (int)Return.Success;
                else
                    result = (int)Return.Fail;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;
        }

        public DataTable GetAllRepairCentre()
        {
            DataTable dt = new DataTable(TN.BranchNumber);
            try
            {

                result = RunSP("GetAllRepairCentre", dt);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dt;
        }

        public void Populate(SqlConnection conn, SqlTransaction trans, short branchNo)
        {
            try
            {
                parmArray = new SqlParameter[25];
                parmArray[0] = new SqlParameter("@branch", SqlDbType.SmallInt);
                parmArray[0].Value = branchNo;
                parmArray[1] = new SqlParameter("@branchname", SqlDbType.NVarChar, 20);
                parmArray[1].Value = "";
                parmArray[2] = new SqlParameter("@address1", SqlDbType.NVarChar, 26);
                parmArray[2].Value = "";
                parmArray[3] = new SqlParameter("@address2", SqlDbType.NVarChar, 26);
                parmArray[3].Value = "";
                parmArray[4] = new SqlParameter("@address3", SqlDbType.NVarChar, 26);
                parmArray[4].Value = "";
                parmArray[5] = new SqlParameter("@postcode", SqlDbType.NVarChar, 10);
                parmArray[5].Value = "";
                parmArray[6] = new SqlParameter("@telno", SqlDbType.NVarChar, 13);
                parmArray[6].Value = "";
                parmArray[7] = new SqlParameter("@servicepcent", SqlDbType.Float);
                parmArray[7].Value = 0;
                parmArray[8] = new SqlParameter("@countrycode", SqlDbType.NChar, 2);
                parmArray[8].Value = "";
                parmArray[9] = new SqlParameter("@croffno", SqlDbType.Int);
                parmArray[9].Value = 0;
                parmArray[10] = new SqlParameter("@daterun", SqlDbType.DateTime);
                parmArray[11] = new SqlParameter("@weekno", SqlDbType.Int);
                parmArray[11].Value = 0;
                parmArray[12] = new SqlParameter("@oldpctype", SqlDbType.NChar, 1);
                parmArray[12].Value = "";
                parmArray[13] = new SqlParameter("@newpctype", SqlDbType.NChar, 1);
                parmArray[13].Value = "";
                parmArray[14] = new SqlParameter("@datepcchange", SqlDbType.DateTime);
                parmArray[15] = new SqlParameter("@batchcontrolno", SqlDbType.Int);
                parmArray[15].Value = 0;
                parmArray[16] = new SqlParameter("@hissn", SqlDbType.Int);
                parmArray[16].Value = 0;
                parmArray[17] = new SqlParameter("@hibuffno", SqlDbType.Int);
                parmArray[17].Value = 0;
                parmArray[18] = new SqlParameter("@warehouseno", SqlDbType.NVarChar, 3);
                parmArray[18].Value = "";
                parmArray[19] = new SqlParameter("@as400exp", SqlDbType.NChar, 1);
                parmArray[19].Value = "";
                parmArray[20] = new SqlParameter("@hirefno", SqlDbType.Int);
                parmArray[20].Value = 0;
                parmArray[21] = new SqlParameter("@as400branchno", SqlDbType.SmallInt);
                parmArray[21].Value = 0;
                parmArray[22] = new SqlParameter("@codreceipt", SqlDbType.Int);
                parmArray[22].Value = 0;
                parmArray[23] = new SqlParameter("@region", SqlDbType.NVarChar, 3);
                parmArray[23].Value = "";
                parmArray[24] = new SqlParameter("@servicelocation", SqlDbType.SmallInt);
                parmArray[24].Value = 0;
 

                foreach (SqlParameter s in parmArray)
                    s.Direction = ParameterDirection.Output;
                parmArray[0].Direction = ParameterDirection.Input;

                if (conn != null && trans != null)
                    RunSP(conn, trans, "DN_BranchPopulateSP", parmArray);
                else
                    RunSP("DN_BranchPopulateSP", parmArray);

                if (parmArray[1].Value != DBNull.Value)
                    this.BranchName = (string)parmArray[1].Value;
                if (parmArray[2].Value != DBNull.Value)
                    this.BranchAddr1 = (string)parmArray[2].Value;
                if (parmArray[3].Value != DBNull.Value)
                    this.BranchAddr2 = (string)parmArray[3].Value;
                if (parmArray[4].Value != DBNull.Value)
                    this.BranchAddr3 = (string)parmArray[4].Value;
                if (parmArray[5].Value != DBNull.Value)
                    this.PostCode = (string)parmArray[5].Value;
                if (parmArray[6].Value != DBNull.Value)
                    this.TelephoneNo = (string)parmArray[6].Value;
                if (parmArray[7].Value != DBNull.Value)
                    this.ServicePCent = (double)parmArray[7].Value;
                if (parmArray[8].Value != DBNull.Value)
                    this.CountryCode = (string)parmArray[8].Value;
                if (parmArray[9].Value != DBNull.Value)
                    this.CroffNo = (int)parmArray[9].Value;
                if (parmArray[10].Value != DBNull.Value)
                    this.DateRun = (DateTime)parmArray[10].Value;
                if (parmArray[11].Value != DBNull.Value)
                    this.WeekNo = (int)parmArray[11].Value;
                if (parmArray[12].Value != DBNull.Value)
                    this.OldPCType = (string)parmArray[12].Value;
                if (parmArray[13].Value != DBNull.Value)
                    this.NewPCType = (string)parmArray[13].Value;
                if (parmArray[14].Value != DBNull.Value)
                    this.DatePCChange = (DateTime)parmArray[14].Value;
                if (parmArray[15].Value != DBNull.Value)
                    this.BatchControlNo = (int)parmArray[15].Value;
                if (parmArray[16].Value != DBNull.Value)
                    this.Hissn = (int)parmArray[16].Value;
                if (parmArray[17].Value != DBNull.Value)
                    this.HiBuffNo = (int)parmArray[17].Value;
                if (parmArray[18].Value != DBNull.Value)
                    this.WarehouseNo = (string)parmArray[18].Value;
                if (parmArray[19].Value != DBNull.Value)
                    this.AS400Exp = (string)parmArray[19].Value;
                if (parmArray[20].Value != DBNull.Value)
                    this.HiRefNo = (int)parmArray[20].Value;
                if (parmArray[21].Value != DBNull.Value)
                    this.AS400BranchNo = (short)parmArray[21].Value;
                if (parmArray[22].Value != DBNull.Value)
                    this.CODReceipt = (int)parmArray[22].Value;
                if (parmArray[23].Value != DBNull.Value)
                    this.Region = (string)parmArray[23].Value;
                if (parmArray[24].Value != DBNull.Value)
                    this.ServiceLocation = (short)parmArray[24].Value;

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        //#15993
        public int GetBuffNo(SqlConnection conn, SqlTransaction trans, short branchNo)
        {
            int buffno = 0;
            try
            {
                string started = DateTime.Now.ToLongTimeString();
                string storedProc = "DN_BranchGetBuffNoSP";

                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@branch", SqlDbType.SmallInt);
                parmArray[0].Value = branchNo;
                parmArray[1] = new SqlParameter("@buffno", SqlDbType.Int);
                parmArray[1].Value = 0;
                parmArray[1].Direction = ParameterDirection.Output;

                this.RunSP(conn, trans, storedProc, parmArray);
                if (parmArray[1].Value != DBNull.Value)
                    buffno = (int)parmArray[1].Value;

                logTime(storedProc, started, DateTime.Now.ToLongTimeString(), branchNo.ToString());
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return buffno;
        }
        /// <summary>
        /// This overloaded method takes the same SqlConnection and SqlTransaction as other methods run as part of the same business process
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        /// <param name="branchNo"></param>
        /// <returns></returns>
        //public int GetBuffNo(SqlConnection conn, SqlTransaction trans, short branchNo)
        //{
        //   int buffno = 0;
        //   try
        //   {
        //      string started = DateTime.Now.ToLongTimeString();
        //      string storedProc = "DN_BranchGetBuffNoSP";

        //      parmArray = new SqlParameter[2];
        //      parmArray[0] = new SqlParameter("@branch", SqlDbType.SmallInt);
        //      parmArray[0].Value = branchNo;
        //      parmArray[1] = new SqlParameter("@buffno", SqlDbType.Int);
        //      parmArray[1].Value = 0;
        //      parmArray[1].Direction = ParameterDirection.Output;

        //      this.RunSP(conn,trans,storedProc, parmArray);
        //      if (parmArray[1].Value != DBNull.Value)
        //         buffno = (int)parmArray[1].Value;

        //      logTime(storedProc, started, DateTime.Now.ToLongTimeString(), branchNo.ToString());
        //   }
        //   catch (SqlException ex)
        //   {
        //      LogSqlException(ex);
        //      throw ex;
        //   }
        //   return buffno;
        //}

        //public int GetTransRefNo(short branchNo)
        //{
        //    int transno = 0;
        //    try
        //    {
        //        parmArray = new SqlParameter[3];
        //        parmArray[0] = new SqlParameter("@branch", SqlDbType.SmallInt);
        //        parmArray[0].Value = branchNo;
        //        parmArray[1] = new SqlParameter("@required", SqlDbType.Int);
        //        parmArray[1].Value = 1;
        //        parmArray[2] = new SqlParameter("@transno", SqlDbType.Int);
        //        parmArray[2].Value = 0;
        //        parmArray[2].Direction = ParameterDirection.Output;

        //        this.RunSP("DN_BranchGetTransRefNoSP", parmArray);
        //        if(parmArray[2].Value != DBNull.Value)
        //            transno = (int)parmArray[2].Value;
        //    }
        //    catch(SqlException ex)
        //    {
        //        LogSqlException(ex);
        //        throw ex;
        //    }
        //    return transno;
        //}
        public int GetTransRefNo(SqlConnection conn, SqlTransaction trans, short branchNo)
        {
            int transno = 0;
            var connWasNull = false;

            try
            {
                parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@branch", SqlDbType.SmallInt);
                parmArray[0].Value = branchNo;
                parmArray[1] = new SqlParameter("@required", SqlDbType.Int);
                parmArray[1].Value = 1;
                parmArray[2] = new SqlParameter("@transno", SqlDbType.Int);
                parmArray[2].Value = 0;
                parmArray[2].Direction = ParameterDirection.Output;

                if (conn == null)
                {
                    connWasNull = true;
                    conn = new SqlConnection(Connections.Default);
                    trans = conn.BeginTransaction();
                }

                this.RunSP(conn, trans, "DN_BranchGetTransRefNoSP", parmArray);
                if (parmArray[2].Value != DBNull.Value)
                    transno = (int)parmArray[2].Value;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            finally
            {
                if (connWasNull)
                {
                    trans.Rollback();
                    trans = null;
                    conn.Close();
                    conn = null;
                }
            }
            return transno;
        }

        public int GetTransRefNos(SqlConnection conn, SqlTransaction trans, short branchNo, int noRequired)
        {
            int transno = 0;
            try
            {
                parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@branch", SqlDbType.SmallInt);
                parmArray[0].Value = branchNo;
                parmArray[1] = new SqlParameter("@required", SqlDbType.Int);
                parmArray[1].Value = noRequired;
                parmArray[2] = new SqlParameter("@transno", SqlDbType.Int);
                parmArray[2].Value = 0;
                parmArray[2].Direction = ParameterDirection.Output;

                this.RunSP(conn, trans, "DN_BranchGetTransRefNoSP", parmArray);
                if (parmArray[2].Value != DBNull.Value)
                    transno = (int)parmArray[2].Value;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return transno;
        }

        public int GetTransRefNos(short branchNo, int noRequired)
        {
            int transno = 0;
            try
            {
                parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@branch", SqlDbType.SmallInt);
                parmArray[0].Value = branchNo;
                parmArray[1] = new SqlParameter("@required", SqlDbType.Int);
                parmArray[1].Value = noRequired;
                parmArray[2] = new SqlParameter("@transno", SqlDbType.Int);
                parmArray[2].Value = 0;
                parmArray[2].Direction = ParameterDirection.Output;

                this.RunSP("DN_BranchGetTransRefNoSP", parmArray);
                if (parmArray[2].Value != DBNull.Value)
                    transno = (int)parmArray[2].Value;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return transno;
        }

        public int BranchTransrefnoCheckUpdate(string acctno, short branchNo, int transferno)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Connections.Default))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                    {
                        parmArray = new SqlParameter[3];
                        parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar, 12);
                        parmArray[0].Value = acctno;
                        parmArray[1] = new SqlParameter("@branchNo", SqlDbType.SmallInt);
                        parmArray[1].Value = branchNo;
                        parmArray[2] = new SqlParameter("@transrefno", SqlDbType.Int);
                        parmArray[2].Value = transferno;
                        parmArray[2].Direction = ParameterDirection.InputOutput;

                        return ReturnInt(conn, trans, "BranchTransrefnoCheckUpdate", parmArray).Value;
                    }
                }
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// This overloaded method takes the same SqlConnection and SqlTransaction as other methods run as part of the same business process
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        /// <param name="branchNo"></param>
        /// <param name="noRequired"></param>
        /// <returns></returns>
        //public int GetTransRefNos(SqlConnection conn, SqlTransaction trans, short branchNo, int noRequired)
        //{
        //   int transno = 0;
        //   try
        //   {
        //      parmArray = new SqlParameter[3];
        //      parmArray[0] = new SqlParameter("@branch", SqlDbType.SmallInt);
        //      parmArray[0].Value = branchNo;
        //      parmArray[1] = new SqlParameter("@required", SqlDbType.Int);
        //      parmArray[1].Value = noRequired;
        //      parmArray[2] = new SqlParameter("@transno", SqlDbType.Int);
        //      parmArray[2].Value = 0;
        //      parmArray[2].Direction = ParameterDirection.Output;

        //      this.RunSP(conn,trans,"DN_BranchGetTransRefNoSP", parmArray);
        //      if (parmArray[2].Value != DBNull.Value)
        //         transno = (int)parmArray[2].Value;
        //   }
        //   catch (SqlException ex)
        //   {
        //      LogSqlException(ex);
        //      throw ex;
        //   }
        //   return transno;
        //}

        public DataTable BranchNumbers
        {
            get
            {
                return _branchnos;
            }
        }

        public int GetBranchAddress(int branchNo, int updhissn)
        {
            try
            {
                // 25/04/08 rdb added telno
                parmArray = new SqlParameter[9];
                parmArray[0] = new SqlParameter("@branch", SqlDbType.SmallInt);
                parmArray[0].Value = branchNo;
                parmArray[1] = new SqlParameter("@updhissn", SqlDbType.SmallInt);
                parmArray[1].Value = updhissn;
                parmArray[2] = new SqlParameter("@branchname", SqlDbType.NVarChar, 20);
                parmArray[2].Value = this.BranchName;
                parmArray[3] = new SqlParameter("@branchaddr1", SqlDbType.NVarChar, 26);
                parmArray[3].Value = this.BranchAddr1;
                parmArray[4] = new SqlParameter("@branchaddr2", SqlDbType.NVarChar, 26);
                parmArray[4].Value = this.BranchAddr2;
                parmArray[5] = new SqlParameter("@branchaddr3", SqlDbType.NVarChar, 26);
                parmArray[5].Value = this.BranchAddr3;
                parmArray[6] = new SqlParameter("@hissn", SqlDbType.Int);
                parmArray[6].Value = 0;
                parmArray[7] = new SqlParameter("@buffno", SqlDbType.Int);
                parmArray[7].Value = 0;
                // 25/04/08 rdb added telno             //IP - 11/05/10 - UAT(135) UAT5.2.1.0 log - Merged from 4.3
                parmArray[8] = new SqlParameter("@telno", SqlDbType.NVarChar, 13);



                foreach (SqlParameter parm in parmArray)
                {
                    parm.Direction = ParameterDirection.Output;
                }

                parmArray[0].Direction = ParameterDirection.Input;
                parmArray[1].Direction = ParameterDirection.Input;

                result = this.RunSP("DN_BranchGetAddressSP", parmArray);

                if (!Convert.IsDBNull(parmArray[2].Value))
                    _branchname = (string)parmArray[2].Value;
                if (!Convert.IsDBNull(parmArray[3].Value))
                    _branchaddr1 = (string)parmArray[3].Value;
                if (!Convert.IsDBNull(parmArray[4].Value))
                    _branchaddr2 = (string)parmArray[4].Value;
                if (!Convert.IsDBNull(parmArray[5].Value))
                    _branchaddr3 = (string)parmArray[5].Value;
                if (!Convert.IsDBNull(parmArray[6].Value))
                    _hissn = (int)parmArray[6].Value;
                if (!Convert.IsDBNull(parmArray[7].Value))
                    _buffno = (int)parmArray[7].Value;
                // 25/04/08 rdb added telno  //IP - 11/05/10 - UAT(135) UAT5.2.1.0 log - Merged from 4.3
                if (parmArray[8].Value != DBNull.Value)
                    _telephoneNo = parmArray[8].Value.ToString();
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;
        }

        public DBranch()
        {

        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="branchno">int</param>
        /// <param name="branchname">string</param>
        /// <param name="branchaddr1">string</param>
        /// <param name="branchaddr2">string</param>
        /// <param name="branchaddr3">string</param>
        /// <param name="branchpocode">string</param>
        /// <param name="telno">string</param>
        /// <param name="countrycode">string</param>
        /// <param name="croffno">int</param>
        /// <param name="oldpctype">string</param>
        /// <param name="newpctype">string</param>
        /// <param name="datepcchange">Datetime</param>
        /// <param name="hissn">int</param>
        /// <param name="hibuffno">int</param>
        /// <param name="warehouseno">string</param>
        /// <param name="hirefno">int</param>
        /// <param name="as400branchno">int</param>
        /// <param name="region">string</param>
        /// <returns>int</returns>
        /// 
        public int Update(SqlConnection conn, SqlTransaction trans, int branchno, string branchname,
            string branchaddr1, string branchaddr2, string branchaddr3, string branchpocode,
            string telno, string countrycode, int croffno, string oldpctype, string newpctype,
            DateTime datepcchange, int hissn, int hibuffno, string warehouseno, int hirefno,
            int as400branchno, string region, bool depositScreenLocked, string warehouseregion, string fact2000BranchLetter,
            string storeType, bool createRF, bool createCash, bool scoreHPbefore, bool createHP, bool serviceRepairCentre,
            bool behavioural,     //CR1034 SC 
                   int? defaultPrintLocation,   //IP - 23/02/10 - CR1072 - Malaysia 3PL for Version 5.2
            bool isThirdPartyWarehouse, bool createStore, bool isCashLoanBranch, bool? luckyDollarStore, bool?ashleyStore)  //IP - 23/02/10 - CR1072 - Malaysia 3PL for Version 5.2) 
        {
            int status = 0;

            try
            {

                parmArray = new SqlParameter[34];

                parmArray[0] = new SqlParameter("@branchno", SqlDbType.SmallInt);
                parmArray[0].Value = branchno;

                parmArray[1] = new SqlParameter("@branchname", SqlDbType.VarChar, 20);
                parmArray[1].Value = branchname;

                parmArray[2] = new SqlParameter("@branchaddr1", SqlDbType.VarChar, 26);
                parmArray[2].Value = branchaddr1;

                parmArray[3] = new SqlParameter("@branchaddr2", SqlDbType.VarChar, 26);
                parmArray[3].Value = branchaddr2;

                parmArray[4] = new SqlParameter("@branchaddr3", SqlDbType.VarChar, 26);
                parmArray[4].Value = branchaddr3;

                parmArray[5] = new SqlParameter("@branchpocode", SqlDbType.VarChar, 10);
                parmArray[5].Value = branchpocode;

                parmArray[6] = new SqlParameter("@telno", SqlDbType.VarChar, 13);
                parmArray[6].Value = telno;

                parmArray[7] = new SqlParameter("@countrycode", SqlDbType.Char, 2);
                parmArray[7].Value = countrycode;

                parmArray[8] = new SqlParameter("@croffno", SqlDbType.Int);
                parmArray[8].Value = croffno;

                parmArray[9] = new SqlParameter("@oldpctype", SqlDbType.Char, 1);
                parmArray[9].Value = oldpctype;

                parmArray[10] = new SqlParameter("@newpctype", SqlDbType.Char, 1);
                parmArray[10].Value = newpctype;

                parmArray[11] = new SqlParameter("@datepcchange", SqlDbType.DateTime);
                parmArray[11].Value = datepcchange;

                parmArray[12] = new SqlParameter("@hissn", SqlDbType.Int);
                parmArray[12].Value = hissn;

                parmArray[13] = new SqlParameter("@hibuffno", SqlDbType.Int);
                parmArray[13].Value = hibuffno;

                parmArray[14] = new SqlParameter("@warehouseno", SqlDbType.VarChar, 3);
                parmArray[14].Value = warehouseno;

                parmArray[15] = new SqlParameter("@hirefno", SqlDbType.Int);
                parmArray[15].Value = hirefno;

                parmArray[16] = new SqlParameter("@as400branchno", SqlDbType.SmallInt);
                parmArray[16].Value = as400branchno;

                parmArray[17] = new SqlParameter("@region", SqlDbType.VarChar, 3);
                parmArray[17].Value = region;

                parmArray[18] = new SqlParameter("@depositscreenlocked", SqlDbType.Bit);
                parmArray[18].Value = depositScreenLocked;

                parmArray[19] = new SqlParameter("@warehouseregion", SqlDbType.VarChar, 12);
                parmArray[19].Value = warehouseregion;

                parmArray[20] = new SqlParameter("@Fact2000BranchLetter", SqlDbType.Char, 1);
                parmArray[20].Value = fact2000BranchLetter;

                //CR903   jec
                parmArray[21] = new SqlParameter("@storeType", SqlDbType.Char, 1);
                parmArray[21].Value = storeType;

                parmArray[22] = new SqlParameter("@createRF", SqlDbType.Bit);
                parmArray[22].Value = createRF;

                parmArray[23] = new SqlParameter("@createCash", SqlDbType.Bit);
                parmArray[23].Value = createCash;

                parmArray[24] = new SqlParameter("@scoreHPbefore", SqlDbType.Bit);
                parmArray[24].Value = scoreHPbefore;

                parmArray[25] = new SqlParameter("@createHP", SqlDbType.Bit);
                parmArray[25].Value = createHP;
                // end CR903

                parmArray[26] = new SqlParameter("@serviceRepairCentre", SqlDbType.Bit); //CR 1056
                parmArray[26].Value = serviceRepairCentre;


                parmArray[27] = new SqlParameter("@Behavioural", SqlDbType.Bit); //CR 1034
                parmArray[27].Value = behavioural;

                //IP - 23/02/10 - CR1072 - Malaysia 3PL for Version 5.2
                parmArray[28] = new SqlParameter("@defaultPrintLocation", SqlDbType.Int);
                parmArray[28].Value = defaultPrintLocation;

                parmArray[29] = new SqlParameter("@isThirdPartyWarehouse", SqlDbType.Char, 1);
                parmArray[29].Value = isThirdPartyWarehouse ? "Y" : "N";

                parmArray[30] = new SqlParameter("@createstore", SqlDbType.Bit);
                parmArray[30].Value = createStore;

                parmArray[31] = new SqlParameter("@isCashLoanBranch", SqlDbType.Bit);
                parmArray[31].Value = isCashLoanBranch;

                parmArray[32] = new SqlParameter("@luckyDollarStore", SqlDbType.Bit);
                parmArray[32].Value = luckyDollarStore;

                parmArray[33] = new SqlParameter("@ashleyStore", SqlDbType.Bit);
                parmArray[33].Value = ashleyStore;

                status = this.RunSP(conn, trans, "DN_BranchUpdateSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            return status;
        }

        /// <summary>
        /// DGet
        /// </summary>
        /// <param name="branchno">int</param>
        /// <returns>DataSet</returns>
        /// 
        public DataSet Get(int branchno)
        {
            DataSet ds = new DataSet();

            try
            {
                parmArray = new SqlParameter[1];

                parmArray[0] = new SqlParameter("@branchno", SqlDbType.SmallInt);
                parmArray[0].Value = branchno;

                this.RunSP("DN_BranchGetSP", parmArray, ds);


            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            return ds;
        }
        public void LockDepositScreen(int branchno)
        {
            try
            {
                parmArray = new SqlParameter[1];

                parmArray[0] = new SqlParameter("@branchno", SqlDbType.SmallInt);
                parmArray[0].Value = branchno;

                this.RunSP("DN_BranchLockDepositScreenSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void UnLockDepositScreen(int branchno)
        {
            try
            {
                parmArray = new SqlParameter[1];

                parmArray[0] = new SqlParameter("@branchno", SqlDbType.SmallInt);
                parmArray[0].Value = branchno;

                this.RunSP("DN_BranchUnLockDepositScreenSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// SaveDepositType
        /// </summary>
        /// <param name="branchNo">int</param>
        /// <param name="payMethod">int</param>
        /// <param name="depositType">int</param>
        /// <returns>Int</returns>
        /// 
        public int SaveDepositType(SqlConnection conn, SqlTransaction trans, int branchNo, string payMethod, string depositType)
        {
            // Save the deposit type (transtypecode) for this branch and paymethod.
            int status = 0;

            try
            {
                parmArray = new SqlParameter[3];

                parmArray[0] = new SqlParameter("@piBranchNo", SqlDbType.SmallInt);
                parmArray[0].Value = branchNo;
                parmArray[1] = new SqlParameter("@piPayMethod", SqlDbType.NVarChar, 12);
                parmArray[1].Value = payMethod;
                parmArray[2] = new SqlParameter("@piDeposit", SqlDbType.NVarChar, 3);
                parmArray[2].Value = depositType;

                status = this.RunSP(conn, trans, "DN_BranchSaveDepositTypeSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            return status;
        }

        /// <summary>
        /// GetDepositType
        /// </summary>
        /// <param name="branchNo">int</param>
        /// <param name="payMethod">int</param>
        /// <returns>String</returns>
        /// 
        public string GetDepositType(int branchNo, string payMethod)
        {
            // Return the deposit type (transtypecode) for this branch and paymethod.
            try
            {
                parmArray = new SqlParameter[3];

                parmArray[0] = new SqlParameter("@piBranchNo", SqlDbType.SmallInt);
                parmArray[0].Value = branchNo;
                parmArray[1] = new SqlParameter("@piPayMethod", SqlDbType.NVarChar, 12);
                parmArray[1].Value = payMethod;
                parmArray[2] = new SqlParameter("@poDeposit", SqlDbType.NVarChar, 3);
                parmArray[2].Direction = ParameterDirection.Output;

                this.RunSP("DN_BranchGetDepositTypeSP", parmArray);

                if (!Convert.IsDBNull(parmArray[2].Value))
                    return (string)parmArray[2].Value;
                else
                    return "";
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// GetDepositList
        /// </summary>
        /// <param name="branchNo">int</param>
        /// <returns>DataSet</returns>
        /// 
        public DataTable GetDepositList(int branchNo)
        {
            DataTable dt = new DataTable();

            try
            {
                parmArray = new SqlParameter[1];

                parmArray[0] = new SqlParameter("@piBranchno", SqlDbType.SmallInt);
                parmArray[0].Value = branchNo;

                this.RunSP("DN_BranchGetDepositListSP", parmArray, dt);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dt;
        }

        //public int GetTransRefNo(SqlConnection conn, SqlTransaction trans, short branchNo)
        //{
        //    int transno = 0;
        //    try
        //    {
        //        parmArray = new SqlParameter[3];
        //        parmArray[0] = new SqlParameter("@branch", SqlDbType.SmallInt);
        //        parmArray[0].Value = branchNo;
        //        parmArray[1] = new SqlParameter("@required", SqlDbType.Int);
        //        parmArray[1].Value = 1;
        //        parmArray[2] = new SqlParameter("@transno", SqlDbType.Int);
        //        parmArray[2].Value = 0;
        //        parmArray[2].Direction = ParameterDirection.Output;

        //        this.RunSP(conn, trans, "DN_BranchGetTransRefNoSP", parmArray);
        //        if(parmArray[2].Value != DBNull.Value)
        //            transno = (int)parmArray[2].Value;
        //    }
        //    catch(SqlException ex)
        //    {
        //        LogSqlException(ex);
        //        throw ex;
        //    }
        //    return transno;
        //}

        public string GetStoreType(short branchNo)
        {
            string storeType = string.Empty;
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@branch", SqlDbType.SmallInt);
                parmArray[0].Value = branchNo;
                parmArray[1] = new SqlParameter("@storetype", SqlDbType.NVarChar, 2);
                parmArray[1].Value = string.Empty;
                parmArray[1].Direction = ParameterDirection.Output;

                this.RunSP("DN_BranchGetStoreTypeSP", parmArray);

                if (parmArray[1].Value != DBNull.Value)
                    storeType = (string)parmArray[1].Value;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return storeType;
        }

        //NM
        public DataSet GetBranchByStoreType(char storeType)
        {
            DataSet ds = new DataSet();
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@storetype", SqlDbType.Char, 1);
                parmArray[0].Value = storeType;                                                            //IP - 07/03/12 - #8848 - LW74316
                //parmArray[0].Value = string.Empty;

                this.RunSP("[DN_GetBranchByStoreType]", parmArray, ds);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            return ds;
        }


        private void logTime(string storedProc, string started, string ended, string parameter)
        {
            LogPerformanceMessage(storedProc + " Started at : " + started + " and Ended at : " + ended + " with parameter: " + parameter, Environment.StackTrace, EventLogEntryType.Information);
        }

        //IP - 25/02/10 - CR1072 - Malaysia 3PL for Version 5.2 - Merged from v4.3
        public SqlDataReader GetBranchDefaultPrintLocation(SqlConnection conn, SqlTransaction trans)
        {
            SqlCommand cmd = new SqlCommand("BranchDefaultPrintLocation", conn, trans);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader rdr = cmd.ExecuteReader();
            return rdr;
        }
    }
}
