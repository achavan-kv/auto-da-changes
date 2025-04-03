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
	/// Summary description for DCorrectedPayments.
	/// </summary>
	public class DCorrectedPayments : DALObject
	{
		public DCorrectedPayments()
		{
		}

		public void Write (SqlConnection conn, SqlTransaction trans, string accountNo, 
							int paymentRef, int correctionRef)
		{
			try
			{
				parmArray = new SqlParameter[3];
				
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
				parmArray[0].Value = accountNo;
				
				parmArray[1] = new SqlParameter("@paymentref", SqlDbType.Int);
				parmArray[1].Value = paymentRef;
				
				parmArray[2] = new SqlParameter("@correctionref", SqlDbType.Int);
				parmArray[2].Value = correctionRef;
							 
				this.RunSP(conn, trans, "DN_CorrectedPaymentsInsertSP", parmArray);
	
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}
	}
}
