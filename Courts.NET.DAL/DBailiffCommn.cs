using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common;
using STL.Common.Constants.Enums;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;

namespace STL.DAL
{
	/// <summary>
	/// Summary description for DBailiffCommn.
	/// </summary>
	public class DBailiffCommn : DALObject
	{
		private int _allocated = 0;
		public int AllocatedCourtsPerson
		{
			get{return _allocated;}
			set{_allocated = value;}
		}

		private string _acctno = "";
		public string AccountNo
		{
			get{return _acctno;}
			set{_acctno = value;}
		}

		private string _cheque = "N";
		public string ChequeColln
		{
			get{return _cheque;}
			set{_cheque = value;}
		}

		private DateTime _dateTrans;
		public DateTime DateTrans
		{
			get{return _dateTrans;}
			set{_dateTrans = value;}
		}

		private string _stat = "";
		public string Status
		{
			get{return _stat;}
			set{_stat = value;}
		}

		private int _transrefno = 0;
		public int TransRefNo
		{
			get{return _transrefno;}
			set{_transrefno = value;}
		}

		private decimal _transvalue = 0;
		public decimal TransValue
		{
			get{return _transvalue;}
			set{_transvalue = value;}
		}



		public void Save(SqlConnection conn, SqlTransaction trans)
		{
			try
			{
				parmArray = new SqlParameter[7];
				parmArray[0] = new SqlParameter("@EmpeeNo", SqlDbType.Int);
				parmArray[0].Value = this.AllocatedCourtsPerson;
				parmArray[1] = new SqlParameter("@TransRefNo", SqlDbType.Int);
				parmArray[1].Value = this.TransRefNo;
				parmArray[2] = new SqlParameter("@AcctNo", SqlDbType.NVarChar,CW.AccountNo);
				parmArray[2].Value = this.AccountNo;
				parmArray[3] = new SqlParameter("@DateTrans", SqlDbType.DateTime);
				parmArray[3].Value = this.DateTrans;
				parmArray[4] = new SqlParameter("@TransValue", SqlDbType.Float);
				parmArray[4].Value = this.TransValue;
				parmArray[5] = new SqlParameter("@ChequeColln", SqlDbType.Char,1);
				parmArray[5].Value = this.ChequeColln;
				parmArray[6] = new SqlParameter("@Status", SqlDbType.Char,1);
				parmArray[6].Value = this.Status;
						
				this.RunSP(conn, trans, "DN_SaveBailiffCommissionSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}			
		}

		public DBailiffCommn()
		{

		}

		/// <summary>
		/// Erase
		/// </summary>
		/// <param name="transrefno">int</param>
		/// <returns>void</returns>
		/// 
		public bool Erase (SqlConnection conn, SqlTransaction trans, int transrefno)
		{
			result = 0;
			try
			{
				parmArray = new SqlParameter[1];
				
				parmArray[0] = new SqlParameter("@transrefno", SqlDbType.Int);
				parmArray[0].Value = transrefno;

				result = this.RunSP(conn, trans, "DN_BailiffCommnEraseSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return Convert.ToBoolean(result);
		}

		public void PayBailiffCommission(SqlConnection conn,SqlTransaction trans,int empeeNo)
		{
			try
			{
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[0].Value = empeeNo;
						
				this.RunSP(conn, trans, "DN_BailiffPayCommissionSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}			
		}

		public void ResetCommissionDue(SqlConnection conn, SqlTransaction trans,
			int empeeno, decimal commValue)
		{
			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[0].Value = empeeno;
				parmArray[1] = new SqlParameter("@commvalue", SqlDbType.Money);
				parmArray[1].Value = commValue;
						
				this.RunSP(conn, trans, "DN_BailiffResetCommissionDueSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void DeleteCommissionTransaction(SqlConnection conn, SqlTransaction trans,
			int empeeNo, DateTime dateTrans, int transRefNo)
		{
			try
			{
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@empeeNo", SqlDbType.Int);
				parmArray[0].Value = empeeNo;
				parmArray[1] = new SqlParameter("@dateTrans", SqlDbType.DateTime);
				parmArray[1].Value = dateTrans;
				parmArray[2] = new SqlParameter("@transRefNo", SqlDbType.Int);
				parmArray[2].Value = transRefNo;
						
				this.RunSP(conn, trans, "DN_DeleteBailiffCommissionSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}			
		}

		public void RestoreCommissionTransaction(SqlConnection conn, SqlTransaction trans,
			int empeeNo, DateTime dateTrans, int transRefNo)
		{
			try
			{
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@empeeNo", SqlDbType.Int);
				parmArray[0].Value = empeeNo;
				parmArray[1] = new SqlParameter("@dateTrans", SqlDbType.DateTime);
				parmArray[1].Value = dateTrans;
				parmArray[2] = new SqlParameter("@transRefNo", SqlDbType.Int);
				parmArray[2].Value = transRefNo;
						
				this.RunSP(conn, trans, "DN_RestoreBailiffCommissionSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}			
		}

	}
}
