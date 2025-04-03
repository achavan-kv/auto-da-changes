using System;
using STL.DAL;
using STL.Common;
using System.Data;
using System.Xml;
using System.Data.SqlClient;

namespace STL.BLL
{
	/// <summary>
	/// Summary description for BUser.
	/// </summary>
	public class BUser : CommonObject
	{
		public DataSet GetFunctionsForType(string userType)
		{
			DataSet ds = new DataSet();
			DUser user = new DUser();
			user.GetFunctionsForType(userType);
			ds.Tables.Add(user.Functions); 
			return ds;
		}

		public void UpdateFunctionsForRole(SqlConnection conn, SqlTransaction trans, string userType, string[] functions)
		{
			DUser user = new DUser();

			//First delete existing functions for this user type
			user.DeleteFunctionsForRole(conn, trans, userType);

			//Next add the functions described in the string array
			foreach(string function in functions)
			{
				user.InsertFunctionForRole(conn, trans, userType, function);
			}

		}

		public void SaveCashDrawerOpen (SqlConnection conn, SqlTransaction trans, int user, string reason, string tillid)
		{
			DUser u = new DUser();
			u.SaveCashDrawerOpen(conn, trans, user, reason, tillid);
		}

		public DataSet LoadCashDrawerOpen (int user)
		{
			DataSet ds = new DataSet();
			DUser u = new DUser();
			ds.Tables.Add(u.LoadCashDrawerOpen(user));
			return ds;
		}

		public BUser()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}
}
