using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common.Constants.Enums;

namespace STL.DAL
{
	/// <summary>
	/// Summary description for DLetter.
	/// </summary>
	public class DLetter : DALObject
	{
		private short _origBr = 0;
		public short OrigBr 
		{
			get{return _origBr;}
			set{_origBr = value;}
		}
		private string _acctno = "";
		public string AccountNo
		{
			get{return _acctno;}
			set{_acctno = value;}
		}
		private DateTime _letterDate = DateTime.MinValue.AddYears(1899);
		public DateTime LetterDate
		{
			get{return _letterDate;}
			set{_letterDate = value;}
		}
		private DateTime _letterDue = DateTime.MinValue.AddYears(1899);
		public DateTime LetterDue
		{
			get{return _letterDue;}
			set{_letterDue = value;}
		}
		private string _letterCode = "";
		public string LetterCode
		{
			get{return _letterCode;}
			set{_letterCode = value;}
		}
		private decimal _addToVal = 0;
		public decimal AddToValue
		{
			get{return _addToVal;}
			set{_addToVal = value;}
		}
        private string _excelGen = "0";
        public string ExcelGen
        {
            get { return _excelGen; }
            set { _excelGen = value; }
        }

		public void Write(SqlConnection conn, SqlTransaction trans)
		{
			try
			{
				parmArray = new SqlParameter[7];
				parmArray[0] = new SqlParameter("@origbr", SqlDbType.SmallInt);
				parmArray[0].Value = this.OrigBr;
				parmArray[1] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
				parmArray[1].Value = this.AccountNo;
				parmArray[2] = new SqlParameter("@dateacctlttr", SqlDbType.DateTime);
				parmArray[2].Value = this.LetterDate;
				parmArray[3] = new SqlParameter("@datedue", SqlDbType.DateTime);
				parmArray[3].Value = this.LetterDue;
				parmArray[4] = new SqlParameter("@lettercode", SqlDbType.NVarChar, 10);
				parmArray[4].Value = this.LetterCode;
				parmArray[5] = new SqlParameter("@addtovalue", SqlDbType.Money);
				parmArray[5].Value = this.AddToValue;
                parmArray[6] = new SqlParameter("@excelgen", SqlDbType.Char);
                parmArray[6].Value = this.ExcelGen;
			
				this.RunSP(conn, trans, "DN_LetterWriteSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public DLetter()
		{
		}

		/// <summary>
		/// LettersAndStatusesByAcctNo
		/// </summary>
		/// <param name="acctno">string</param>
		/// <returns>DataSet</returns>
		/// 
		public DataSet LettersAndStatusesByAcctNo (string acctno)
		{
			DataSet ds = new DataSet();
	
			try
			{
				parmArray = new SqlParameter[1];
				
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar, 12);
				parmArray[0].Value = acctno;
				 
				this.RunSP("DN_LetterLoadByAcctNo", parmArray, ds);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return ds;
		}

		public void WritePotentialSpend(SqlConnection conn, SqlTransaction trans)
		{
			try
			{
				parmArray = new SqlParameter[6];
				parmArray[0] = new SqlParameter("@origbr", SqlDbType.SmallInt);
				parmArray[0].Value = this.OrigBr;
				parmArray[1] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
				parmArray[1].Value = this.AccountNo;
				parmArray[2] = new SqlParameter("@dateacctlttr", SqlDbType.DateTime);
				parmArray[2].Value = this.LetterDate;
				parmArray[3] = new SqlParameter("@datedue", SqlDbType.DateTime);
				parmArray[3].Value = this.LetterDue;
				parmArray[4] = new SqlParameter("@lettercode", SqlDbType.NVarChar, 10);
				parmArray[4].Value = this.LetterCode;
				parmArray[5] = new SqlParameter("@addtovalue", SqlDbType.Money);
				parmArray[5].Value = this.AddToValue;
			
				this.RunSP(conn, trans, "DN_LetterWritePotentialSpendSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

        public DataSet GetDistinctLetterCodesByRunNo(int runNo)
        {
            DataSet ds = new DataSet();
            try
            {
                parmArray = new SqlParameter[1];

                parmArray[0] = new SqlParameter("@runNo", SqlDbType.SmallInt);
                parmArray[0].Value = runNo;

                this.RunSP("DN_LetterLoadByRunNo", parmArray, ds);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return ds;
        }
	}
}