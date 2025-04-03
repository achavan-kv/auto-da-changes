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
	/// Summary description for DTransactionAudit.
	/// </summary>
	public class DTransactionAudit : DALObject
	{
		public DTransactionAudit()
		{
		}

		public void Write(SqlConnection conn, SqlTransaction trans,
							string accountNo, int user, int authorisedBy, 
							decimal actualValue, decimal calcValue,  
							DateTime dateTrans, int refNo, 
							string transType )
		{
			try
			{
				parmArray = new SqlParameter[8];
				
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNo;
				
				parmArray[1] = new SqlParameter("@empeenoauth", SqlDbType.Int);
				parmArray[1].Value = authorisedBy;
				
				parmArray[2] = new SqlParameter("@userempeeno", SqlDbType.Int);
				parmArray[2].Value = user;
				
				parmArray[3] = new SqlParameter("@actualvalue", SqlDbType.Money);
				parmArray[3].Value = actualValue;
				
				parmArray[4] = new SqlParameter("@calcvalue", SqlDbType.Money);
				parmArray[4].Value = calcValue;
				
				parmArray[5] = new SqlParameter("@datetrans", SqlDbType.DateTime);
				parmArray[5].Value = dateTrans;
				
				parmArray[6] = new SqlParameter("@transrefno", SqlDbType.Int);
				parmArray[6].Value = refNo;
				
				parmArray[7] = new SqlParameter("@transtype", SqlDbType.NVarChar, 3);
				parmArray[7].Value = transType;
				 
				this.RunSP(conn, trans, "DN_TransactionAuditInsertSP", parmArray);
	
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}
	}
}
