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
	/// Summary description for DChequeDetail.
	/// </summary>
	public class DChequeDetail : DALObject
	{
		public void Write(SqlConnection conn, SqlTransaction trans, 
							string accountNo, 
							string bankCode, string bankAcctNo, string chequeNo,
							double chequeVal, int transRefNo)
		{
			try
			{
				parmArray = new SqlParameter[6];
				parmArray[0] = new SqlParameter("@bankCode", SqlDbType.NVarChar, 6);
				parmArray[0].Value = bankCode;
				parmArray[1] = new SqlParameter("@bankAcctNo", SqlDbType.NVarChar, 20);
				parmArray[1].Value = bankAcctNo;
				parmArray[2] = new SqlParameter("@chequeNo", SqlDbType.NVarChar, 14);
				parmArray[2].Value = chequeNo;
				parmArray[3] = new SqlParameter("@chequeVal", SqlDbType.Float);
				parmArray[3].Value = chequeVal;
				parmArray[4] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
				parmArray[4].Value = accountNo;
				parmArray[5] = new SqlParameter("@transrefno", SqlDbType.Int);
				parmArray[5].Value = transRefNo;

				RunSP(conn, trans, "DN_CheqDetailWriteSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public DChequeDetail()
		{
		}
	}
}
