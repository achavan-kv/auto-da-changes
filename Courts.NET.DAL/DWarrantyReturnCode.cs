using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common.Constants.Enums;
using STL.Common.Constants.TableNames;

namespace STL.DAL
{
	/// <summary>
	/// Summary description for DWarrantyReturnCode.
	/// </summary>
	public class DWarrantyReturnCode : DALObject
	{
		public string GetWarrantyReturnItem(SqlConnection conn, SqlTransaction trans, 
											int elapsedMonths)
		{
			string returnItem = "";
			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@elapsedMonths", SqlDbType.Int);
				parmArray[0].Value = elapsedMonths;
				parmArray[1] = new SqlParameter("@returnItem", SqlDbType.NVarChar,8);
				parmArray[1].Value = "";
				parmArray[1].Direction = ParameterDirection.Output;

				if(conn!=null&&trans!=null)
					RunSP(conn, trans, "DN_WarrantyReturnCodeGetSP", parmArray);
				else
					RunSP("DN_WarrantyReturnCodeGetSP", parmArray);
				if(parmArray[1].Value!=DBNull.Value)
					returnItem = (string)parmArray[1].Value;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return returnItem;
		}

		public DWarrantyReturnCode()
		{
		}
	}
}
