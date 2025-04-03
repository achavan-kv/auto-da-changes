using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common;
using STL.Common.Constants.Enums;

namespace STL.DAL
{
	/// <summary>
	/// Data access object for account number
	/// </summary>
	public class DAccountNumber : DALObject
	{
		private DataTable _control;

		/// <summary>
		/// Retrieves all account control records from the 
		/// acctctrlno table for this branch accout type 
		/// combination
		/// </summary>
		/// <param name="branchNo">branch number</param>
		/// <param name="accountType">account type</param>
		/// <returns>return code</returns>
		public int GetAccountControl(string countryCode, short branchNo, string accountType)
		{
			try
			{
				_control = new DataTable("AccountControl");

				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@countryCode", SqlDbType.NVarChar,2);
				parmArray[0].Value = countryCode;
				parmArray[1] = new SqlParameter("@branchNo", SqlDbType.SmallInt);
				parmArray[1].Value = branchNo;
				parmArray[2] = new SqlParameter("@accountType", SqlDbType.NVarChar,3);
				parmArray[2].Value = accountType;

				result = this.RunSP("DN_AccountGetControlNoSP", parmArray, _control);
				if(result==0)
					result = (int)Return.Success;
				else
					result = (int)Return.Fail;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

		public int GetAccountControl(SqlConnection conn, SqlTransaction trans, string countryCode, short branchNo, string accountType)
		{
			try
			{
				_control = new DataTable("AccountControl");

				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@countryCode", SqlDbType.NVarChar,2);
				parmArray[0].Value = countryCode;
				parmArray[1] = new SqlParameter("@branchNo", SqlDbType.SmallInt);
				parmArray[1].Value = branchNo;
				parmArray[2] = new SqlParameter("@accountType", SqlDbType.NVarChar,3);
				parmArray[2].Value = accountType;

				result = this.RunSP(conn, trans, "DN_AccountGetControlNoSP", parmArray, _control);
				if(result==0)
					result = (int)Return.Success;
				else
					result = (int)Return.Fail;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

		/// <summary>
		/// Updates a specific account control record
		/// used to increment the hiallocated field
		/// during account number creation
		/// </summary>
		/// <param name="branchNo">branch number</param>
		/// <param name="accountType">account type</param>
		/// <param name="hiAllocated">hiallocated value to update to</param>
		/// <param name="hiAllowed">hiallowed field</param>
		/// <returns>return code</returns>
		public int UpdateAccountControl(SqlConnection conn, SqlTransaction trans, short branchNo, string accountType,
			int hiAllocated, int hiAllowed)
		{
			try
			{
				parmArray = new SqlParameter[4];
				parmArray[0] = new SqlParameter("@branchNo", SqlDbType.SmallInt);
				parmArray[0].Value = branchNo;
				parmArray[1] = new SqlParameter("@accountType", SqlDbType.NVarChar,3);
				parmArray[1].Value = accountType;
				parmArray[2] = new SqlParameter("@hiAllocated", SqlDbType.Int);
				parmArray[2].Value = hiAllocated;
				parmArray[3] = new SqlParameter("@hiAllowed", SqlDbType.Int);
				parmArray[3].Value = hiAllowed;

				result = this.RunSP(conn, trans, "DN_AccountUpdateControlNoSP", parmArray);
				if(result==0)
					result = (int)Return.Success;
				else
					result = (int)Return.Fail;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}


		/// <summary>
		/// Checks to see if a generated account number
		/// already exists
		/// </summary>
		/// <param name="accountNumber">account number to check</param>
		/// <returns>return code</returns>
		public int IsDuplicate(string accountNumber)
		{
			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@accountNumber", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNumber;
				parmArray[1] = new SqlParameter("@exists", SqlDbType.Int);
                parmArray[1].Direction=ParameterDirection.Output;
				parmArray[1].Value = 0;

				result = this.RunSP("DN_AccountDoesExistSP", parmArray);
				if(result==0)
					result = (int)parmArray[1].Value;
				else
					result = (int)Return.Fail;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

        
		public DataTable AccountControlTable
		{
			get
			{
				return _control;
			}
		}

		public DAccountNumber()
		{

		}
	}
}
