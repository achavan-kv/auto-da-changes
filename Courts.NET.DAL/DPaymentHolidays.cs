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
	/// Summary description for DPaymentHolidays.
	/// </summary>
	public class DPaymentHolidays : DALObject
	{
		public DPaymentHolidays()
		{
		}

		/// <summary>
		/// Get
		/// </summary>
		/// <param name="acctno">string</param>
		/// <param name="agrmtno">int</param>
		/// <returns>DataSet</returns>
		/// 
		public DataTable Get (SqlConnection conn, SqlTransaction trans, 
							string customerID)
		{
			DataTable dt = new DataTable(TN.PaymentHolidays);	
			
			try
			{
				parmArray = new SqlParameter[1];
				
				parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
				parmArray[0].Value = customerID;
				
				if(conn!=null && trans != null)
					this.RunSP(conn, trans, "DN_PaymentHolidaysGetSP", parmArray, dt);
				else
					this.RunSP("DN_PaymentHolidaysGetSP", parmArray, dt);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			
			return dt;
		}

		/// <summary>
		/// Write
		/// </summary>
		/// <param name="acctno">string</param>
		/// <param name="agrmtno">int</param>
		/// <param name="datetaken">DateTime</param>
		/// <param name="empeeno">int</param>
		/// <param name="newdatefirst">DateTime</param>
		/// <returns>void</returns>
		/// 
		public void Write (SqlConnection conn, SqlTransaction trans, 
							string acctno, int agrmtno, DateTime datetaken, 
							int empeeno, DateTime newdatefirst)
		{
			try
			{
				parmArray = new SqlParameter[5];
				
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 24);
				parmArray[0].Value = acctno;
				
				parmArray[1] = new SqlParameter("@agrmtno", SqlDbType.Int);
				parmArray[1].Value = agrmtno;
				
				parmArray[2] = new SqlParameter("@datetaken", SqlDbType.DateTime);
				parmArray[2].Value = datetaken;
				
				parmArray[3] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[3].Value = empeeno;
				
				parmArray[4] = new SqlParameter("@newdatefirst", SqlDbType.DateTime);
				parmArray[4].Value = newdatefirst;				 
				
				if(conn!=null && trans!=null)
					this.RunSP(conn, trans, "DN_PaymentHolidaysInsertSP", parmArray);				
				else
					this.RunSP("DN_PaymentHolidaysInsertSP", parmArray);				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}
	}
}