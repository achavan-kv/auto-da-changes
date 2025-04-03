using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common;
using STL.Common.Constants.Enums;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;

namespace STL.DAL
{
	/// <summary>
	/// Summary description for DFinXfr.
	/// </summary>
	public class DFinXfr : DALObject
	{
		public DFinXfr()
		{
		}

		/// <summary>
		/// Write
		/// </summary>
		/// <param name="acctno">string</param>
		/// <param name="transrefno">int</param>
		/// <param name="datetrans">DateTime</param>
		/// <param name="acctnoxfr">string</param>
		/// <param name="acctname">string</param>
		/// <returns>void</returns>
		/// 
		public void Write (SqlConnection conn, SqlTransaction trans, 
							string acctno, 
							int transrefno, 
							DateTime datetrans, 
							string acctnoxfr,
                            int agrmtno,
                            Int64? storecardno,                                 //IP - 30/11/2010 - StoreCard - added agrmtno and storecardno
                            string cashierTotID = "",                           //IP - 14/02/12 - #8819 - CR1234
                            int origTransRefNo = 0)                             //IP - 20/02/12 - #9633 - CR1234 - Store original TransRefNo
		{
			try
			{
				parmArray = new SqlParameter[8];
				
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 24);
				parmArray[0].Value = acctno;
				
				parmArray[1] = new SqlParameter("@transrefno", SqlDbType.Int);
				parmArray[1].Value = transrefno;
				
				parmArray[2] = new SqlParameter("@datetrans", SqlDbType.DateTime);
				parmArray[2].Value = datetrans;
				
				parmArray[3] = new SqlParameter("@acctnoxfr", SqlDbType.NVarChar, 24);
				parmArray[3].Value = acctnoxfr;

                parmArray[4] = new SqlParameter("@agrmtno", SqlDbType.Int);                 //IP - 30/11/2010 - StoreCard
                parmArray[4].Value = agrmtno;

                parmArray[5] = new SqlParameter("@storecardno", SqlDbType.BigInt);          //IP - 30/11/2010 - StoreCard
                parmArray[5].Value = storecardno;

                parmArray[6] = new SqlParameter("@cashierTotID", SqlDbType.VarChar,10);     //IP - 14/02/12 - #8819 - CR1234
                parmArray[6].Value = cashierTotID;

                parmArray[7] = new SqlParameter("@origTransRefNo", SqlDbType.Int);          //IP - 20/02/12 - #9633 - CR1234
                parmArray[7].Value = origTransRefNo;
				 
				if(conn!=null && trans!=null)
					RunSP(conn, trans, "DN_FinXfrInsertSP", parmArray);
				else
					RunSP("DN_FinXfrInsertSP", parmArray);
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}
	}
}