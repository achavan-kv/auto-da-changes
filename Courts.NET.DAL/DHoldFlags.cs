using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common.Constants.Enums;
using STL.Common.Constants.TableNames;

namespace STL.DAL
{
	/// <summary>
	/// Summary description for DHoldFlags.
	/// </summary>
	public class DHoldFlags: DALObject
	{
		private DataTable _table;



        private short _origbr = 0;
        public short OrigBr
        {
            get { return _origbr; }
            set { _origbr = value; }
        }
        private string _custid = "";
        public string CustomerID
        {
            get { return _custid; }
            set { _custid = value; }
        }
       
        private string _checktype = "";
        public string CheckType
        {
            get { return _checktype; }
            set { _checktype = value; }
        }
        private DateTime _datecleared = DateTime.MinValue.AddYears(1899);
        public DateTime DateCleared
        {
            get { return _datecleared; }
            set { _datecleared = value; }
        }
        private int _empeeno = 0;
        public int EmployeeNoFlag
        {
            get { return _empeeno; }
            set { _empeeno = value; }
        }
        

        private DataTable _icFlags = null;
        public DataTable InstantCreditFlags
        {
            get { return _icFlags; }
            set { _icFlags = value; }
        }



		public int GetHoldFlagCodes()
		{
			try
			{
				_table = new DataTable("HoldFlags");
				result = this.RunSP("DN_CodesGetHoldFlagSP", _table);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

		public int GetHoldFlags(string accountNumber)
		{
			try
			{
				_table = new DataTable("HoldFlags");
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
				parmArray[0].Value = accountNumber;
				result = this.RunSP("DN_HoldFlagsGetSP", parmArray, _table);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

        public int GetICFlags(string accountNumber)
        {
            try
            {
                _table = new DataTable("HoldFlags");
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[0].Value = accountNumber;
                result = this.RunSP("DN_ICFlagsGetSP", parmArray, _table);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;
        }



        public void SaveInstantCreditFlag(SqlConnection conn, SqlTransaction trans, string acctno)
        {
            try
            {
                _icFlags = new DataTable(TN.InstantCreditFlags);
                parmArray = new SqlParameter[6];
                parmArray[0] = new SqlParameter("@origbr", SqlDbType.SmallInt);
                parmArray[0].Value = this.OrigBr;
                parmArray[1] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
                parmArray[1].Value = this.CustomerID;;
                parmArray[2] = new SqlParameter("@checktype", SqlDbType.NVarChar, 4);
                parmArray[2].Value = this.CheckType;
                parmArray[3] = new SqlParameter("@datecleared", SqlDbType.DateTime);
                if (this.DateCleared == DateTime.MinValue.AddYears(1899))
                    parmArray[3].Value = DBNull.Value;
                else
                    parmArray[3].Value = this.DateCleared;
                parmArray[4] = new SqlParameter("@empeenopflg", SqlDbType.Int);
                parmArray[4].Value = this.EmployeeNoFlag;
                parmArray[5] = new SqlParameter("@acctno", SqlDbType.Char, 12);
                parmArray[5].Value = acctno;
                this.RunSP(conn, trans, "DN_InstantCreditFlagSaveSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

		public DataTable Table 
		{
			get 
			{
				return _table;
			}
		}
		public DHoldFlags()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}
}
