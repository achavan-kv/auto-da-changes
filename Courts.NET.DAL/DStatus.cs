using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common;
using STL.Common.Constants.Enums;
using STL.Common.Constants.TableNames;

namespace STL.DAL
{
	/// <summary>
	/// Summary description for DStatus.
	/// </summary>
	public class DStatus : DALObject
	{
		private DataTable _statusCodes;
		public DataTable StatusCodes
		{
			get	{return _statusCodes;}
		}
		
		public void Write(SqlConnection conn, SqlTransaction trans,
							string accountNo, DateTime dateChanged,
							int user, string status)
		{
			try
			{
				parmArray = new SqlParameter[4];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@datechanged", SqlDbType.DateTime);
				parmArray[1].Value = dateChanged;
				parmArray[2] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[2].Value = user;
				parmArray[3] = new SqlParameter("@status", SqlDbType.NChar, 1);
				parmArray[3].Value = status;

				this.RunSP(conn, trans, "DN_StatusWriteSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		/// <summary>
		/// reverts the account to whatever status it was at before it was settled
		/// </summary>
		/// <param name="conn"></param>
		/// <param name="trans"></param>
		/// <param name="accountNo"></param>
		/// <param name="dateChanged"></param>
		/// <param name="user"></param>
		/// <returns>the updated acct status</returns>
		public string Unsettle(SqlConnection conn, SqlTransaction trans,
							string accountNo, DateTime dateChanged,
							int user)
		{
			string newStat = "1";
			try
			{
				parmArray = new SqlParameter[4];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@datechanged", SqlDbType.DateTime);
				parmArray[1].Value = dateChanged;
				parmArray[2] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[2].Value = user;
				parmArray[3] = new SqlParameter("@status", SqlDbType.NChar, 1);
				parmArray[3].Value = "";
				parmArray[3].Direction = ParameterDirection.Output;

				this.RunSP(conn, trans, "DN_StatusUnsettleSP", parmArray);
				if(DBNull.Value != parmArray[3].Value)
					newStat = (string)parmArray[3].Value;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return newStat;
		}

		public void Update(SqlConnection conn, SqlTransaction trans,
			string accountNo, DateTime dateChanged,
			int user, string status)
		{
			try
			{
				parmArray = new SqlParameter[4];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@datechanged", SqlDbType.DateTime);
				parmArray[1].Value = dateChanged;
				parmArray[2] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[2].Value = user;
				parmArray[3] = new SqlParameter("@status", SqlDbType.NChar, 1);
				parmArray[3].Value = status;

				this.RunSP(conn, trans, "DN_StatusUpdateSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void GetStatusForAccount(string accountNo)
		{
			_statusCodes = new DataTable();

			try
			{
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNo;

				this.RunSP("DN_StatusGetSP", parmArray, _statusCodes);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}
		public DStatus()
		{

		}
	}
}
