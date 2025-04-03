using System;
using STL.DAL;
using STL.Common;
using System.Data;
using System.Xml;
using System.Data.SqlClient;
using STL.Common.Constants.Enums;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;
using System.Collections.Specialized;
namespace STL.BLL
{
	/// <summary>
	/// Summary description for BBDW.
	/// </summary>
	public class BBDW : CommonObject
	{

		public DataSet GetForWOReview(string code, string branchFilter, int excludeAccepted, int limit, string category)
		{
			DataSet ds = new DataSet();
			DBDW bdw = new DBDW();
			bdw.GetForWOReview(code, branchFilter, excludeAccepted, limit, category);
			ds.Tables.Add(bdw.Accounts); 
			return ds;
		}

		public void AcceptForWO(SqlConnection conn, SqlTransaction trans, string acctNo, int user, out int exists)
		{
			DBDW bdw = new DBDW();
			bdw.AcceptForWO(acctNo, user, out exists);

			if(Convert.ToBoolean(exists))
			{
				string status = "6";

				DAccount acct = new DAccount(conn, trans, acctNo);
			
				acct.User = user;
				acct.CurrentStatus = status;
				acct.Save(conn, trans);	
			}
		}

		public void SavePending(SqlConnection conn, SqlTransaction trans, string acctNo, 
			int user, string code, int runno, int manualUser)
		{
			string status = "6";

			DBDW bdw = new DBDW();
			bdw.SavePending(conn, trans, acctNo, user, code, runno, manualUser);

			DAccount acct = new DAccount(conn, trans, acctNo);
			
			acct.User = manualUser;
			acct.CurrentStatus = status;
			acct.Save(conn, trans);	

		}

		public void SaveRejection(SqlConnection conn, SqlTransaction trans, string acctNo, 
			int user, string rejectcode)
		{
			string status = "6";

			DBDW bdw = new DBDW();
			bdw.SaveRejection(conn, trans, acctNo, user, rejectcode);

			bdw.ReverseStatusCode(conn, trans, acctNo, user, status);
		}

		public BBDW()
		{

		}
	}
}
