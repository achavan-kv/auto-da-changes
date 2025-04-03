using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common.Constants.Enums;

namespace STL.DAL
{
	/// <summary>
	/// Summary description for DSundryCharge.
	/// </summary>
	public class DSundryCharge : DALObject
	{
		private DataTable _items = null;
		public DataTable Items
		{
			get{return _items;}
		}

		public void GetSundryChargeItem(string accountType)
		{
			try
			{
				_items = new DataTable("Items");
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@acctType", SqlDbType.NVarChar,1);
				parmArray[0].Value = accountType;
				this.RunSP("DN_SundryChargeTypeGetItemsSP", parmArray, _items);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void GetSundryChargeItem(SqlConnection conn, SqlTransaction trans, string accountType)
		{
			try
			{
				_items = new DataTable("Items");
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@acctType", SqlDbType.NVarChar,1);
				parmArray[0].Value = accountType;
				this.RunSP(conn, trans, "DN_SundryChargeTypeGetItemsSP", parmArray, _items);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public DSundryCharge()
		{

		}
	}
}
