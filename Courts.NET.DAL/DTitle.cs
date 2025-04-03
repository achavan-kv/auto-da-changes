using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common.Constants.Enums;
using STL.Common.Constants.TableNames;

namespace STL.DAL
{
	/// <summary>
	/// Summary description for DTitle.
	/// </summary>
	public class DTitle:DALObject
	{
		private DataTable _table;

		public int GetTitleCodes()
		{
			try
			{            	
				_table = new DataTable(TN.Title);
				result = this.RunSP("DN_CodesGetTitleSP", _table);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

		public DataTable Titles
		{
			get 
			{
				return _table;
			}
		}

		public DTitle()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}
}
