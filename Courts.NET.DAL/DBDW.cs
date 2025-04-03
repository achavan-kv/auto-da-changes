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
	/// Summary description for DBDW.
	/// </summary>
	public class DBDW:DALObject
	{

		private DataTable _accounts;
		public DataTable Accounts
		{
			get	{return _accounts;}
		}

		public void GetForWOReview(string code, string branchFilter, int excludeAccepted, int limit, string category)
		{
			_accounts = new DataTable();

			try
			{
				parmArray = new SqlParameter[5];
				parmArray[0] = new SqlParameter("@code", SqlDbType.NVarChar,6);
				parmArray[0].Value = code;
				parmArray[1] = new SqlParameter("@branch", SqlDbType.NVarChar,4);
				parmArray[1].Value = branchFilter;
				parmArray[2] = new SqlParameter("@excludeAccepted", SqlDbType.Int);
				parmArray[2].Value = excludeAccepted;
				parmArray[3] = new SqlParameter("@limit", SqlDbType.Int);
				parmArray[3].Value = limit;
				parmArray[4] = new SqlParameter("@category", SqlDbType.NVarChar, 12);
				parmArray[4].Value = category;

				this.RunSP("DN_WriteOffReviewGetSP", parmArray, _accounts);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void AcceptForWO(string acctNo, int user, out int exists)
		{
			try
			{
				exists = 0;

				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = acctNo;
				parmArray[1] = new SqlParameter("@user", SqlDbType.Int);
				parmArray[1].Value = user;
				parmArray[2] = new SqlParameter("@exists", SqlDbType.Int);
				parmArray[2].Value = exists;
				parmArray[2].Direction = ParameterDirection.Output;

				this.RunSP("DN_BDWPendingAcceptForWOSP", parmArray);
		
				exists = (int)parmArray[2].Value;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}
		
		public void SavePending(SqlConnection conn, SqlTransaction trans, string acctNo, 
			int user, string code, int runno, int manualUser)
		{
			try
			{
				parmArray = new SqlParameter[5];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = acctNo;
				parmArray[1] = new SqlParameter("@user", SqlDbType.Int);
				parmArray[1].Value = user;
				parmArray[2] = new SqlParameter("@code", SqlDbType.NVarChar,4);
				parmArray[2].Value = code;
				parmArray[3] = new SqlParameter("@runno", SqlDbType.Int);
				parmArray[3].Value = runno;
				parmArray[4] = new SqlParameter("@manualuser", SqlDbType.Int);
				parmArray[4].Value = manualUser;

				this.RunSP("DN_BDWPendingSaveSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void SaveRejection(SqlConnection conn, SqlTransaction trans, string acctNo, 
			int user, string rejectcode)
		{
			try
			{
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = acctNo;
				parmArray[1] = new SqlParameter("@user", SqlDbType.Int);
				parmArray[1].Value = user;
				parmArray[2] = new SqlParameter("@rejectcode", SqlDbType.NVarChar);
				parmArray[2].Value = rejectcode;

				this.RunSP("DN_BDWRejectionSaveSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void ReverseStatusCode(SqlConnection conn, SqlTransaction trans, string acctNo, 
			int user, string status)
		{
			try
			{
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = acctNo;
				parmArray[1] = new SqlParameter("@status", SqlDbType.NChar,1);
				parmArray[1].Value = status;
				parmArray[2] = new SqlParameter("@user", SqlDbType.Int);
				parmArray[2].Value = user;

				this.RunSP("DN_ReverseStatusCodeSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public DBDW()
		{
		}
	}
}
