using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common.Constants.Enums;
using STL.Common.Constants.TableNames;

namespace STL.DAL
{
	/// <summary>
	/// Summary description for DUser.
	/// </summary>
	public class DUser : DALObject
	{
		private DataTable _userFunctions = null;
		public DataTable UserFunctions
		{
			get{return _userFunctions;}
		}
		private DataTable _usertypes = null;
		public DataTable UserTypes
		{
			get{return _usertypes;}
		}
		private DataTable _functions = null;
		public DataTable Functions
		{
			get{return _functions;}
		}

		public int GetUserTypes()
		{
			try
			{			
				_usertypes = new DataTable(TN.UserTypes);
				result = this.RunSP("DN_UserTypesGetSP", _usertypes);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}			
			return result;
		}

		public int GetUserFunctions()
		{
			try
			{
				_userFunctions = new DataTable(TN.UserFunctions);
				result = this.RunSP("DN_UserFunctionsGetSP", _userFunctions);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}			
			return result;
		}

		public int GetFunctionsForType(string userType)
		{
			try
			{
				_functions = new DataTable("Functions");
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@userType", SqlDbType.NVarChar,3); //IP - 21/05/08 - Credit Collections - Need to cater for (3) character Employee Types.
				parmArray[0].Value = userType;
				result = this.RunSP("DN_FunctionsGetForTypeSP", parmArray, _functions);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}			
			return result;
		}

		public int DeleteFunctionsForRole(SqlConnection conn, SqlTransaction trans, string userType)
		{
			try
			{
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@userType", SqlDbType.NVarChar,3); //IP - 21/05/08 - Credit Collections - Need to cater for (3) character Employee Types.
				parmArray[0].Value = userType;
				result = this.RunSP(conn, trans, "DN_FunctionsDeleteForRoleSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}			
			return result;
		}

		public int InsertFunctionForRole(SqlConnection conn, SqlTransaction trans, string userType, string function)
		{
			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@userType", SqlDbType.NVarChar,3); //IP - 21/05/08 - Credit Collections - Need to cater for (3) character Employee Types.
				parmArray[0].Value = userType;
				parmArray[1] = new SqlParameter("@function", SqlDbType.NVarChar,100);
				parmArray[1].Value = function;
				result = this.RunSP(conn, trans, "DN_FunctionInsertForRoleSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}			
			return result;
		}

		public int SaveCashDrawerOpen (SqlConnection conn, SqlTransaction trans, int user, string reason, string tillid)
		{
			try
			{
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@user", SqlDbType.Int);
				parmArray[0].Value = user;
				parmArray[1] = new SqlParameter("@reasoncode", SqlDbType.NVarChar,4);
				parmArray[1].Value = reason;
				parmArray[2] = new SqlParameter("@tillid", SqlDbType.NVarChar,16);
				parmArray[2].Value = tillid;

				result = this.RunSP(conn, trans, "dn_cashtillopensave", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}			
			return result;
		}

		public DataTable LoadCashDrawerOpen (int user)
		{
			DataTable dt = new DataTable();
			try
			{
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@user", SqlDbType.Int);
				parmArray[0].Value = user;

				result = this.RunSP("dn_cashtillopenloadempeeno", parmArray, dt);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}			
			return dt;
		}

		public DUser()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}
}
