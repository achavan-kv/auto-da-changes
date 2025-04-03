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
	/// Summary description for DEndOfDay.
	/// </summary>
	public class DEndOfDay : DALObject
	{
		public DEndOfDay()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public DataTable GetEodOptionList (string configuration)
		{
			DataTable eodOptionList = new DataTable();
			try
			{
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@configuration", SqlDbType.NVarChar,12);
				parmArray[0].Value = configuration;

				result = this.RunSP("DN_EODConfigurationOptionsGetSP", parmArray, eodOptionList);
			
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
			return eodOptionList;
		}

	}
}
