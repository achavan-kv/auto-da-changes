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
	/// Summary description for DSourceOfAttraction.
	/// </summary>
	public class DSourceOfAttraction : DALObject
	{
		private DataTable _soa;

		public int GetSOASummary()
		{
			try
			{
				_soa = new DataTable(TN.SourceOfAttraction);

				result = this.RunSP("DN_SOASummaryGetSP", _soa);
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

		public DataTable SOA
		{
			get
			{
				return _soa;
			}
		}
		public DSourceOfAttraction()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}
}
