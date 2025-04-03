using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common;
using STL.Common.Constants.Enums;

namespace STL.DAL
{
	/// <summary>
	/// Summary description for CashierTotalsBreakdown.
	/// </summary>
	public class DCashierTotalsBreakdown : DALObject
	{
		public DCashierTotalsBreakdown() 
		{
		}

		/// <summary>
		/// Save
		/// </summary>
		/// <param name="cashiertotalid">int</param>
		/// <param name="paymethod">string</param>
		/// <param name="systemvalue">double</param>
		/// <param name="uservalue">double</param>
		/// <param name="depositvalue">double</param>
		/// <param name="differencevalue">double</param>
		/// <param name="reason">string</param>
		/// <returns>void</returns>
		/// 
		public void Save (SqlConnection conn, SqlTransaction trans, int cashiertotalid, string paymethod, decimal systemvalue, decimal uservalue, decimal depositvalue, decimal differencevalue, string reason, decimal securitisedValue)
		{
			try
			{
				parmArray = new SqlParameter[8];
				
				parmArray[0] = new SqlParameter("@cashiertotalid", SqlDbType.Int);
				parmArray[0].Value = cashiertotalid;
				
				parmArray[1] = new SqlParameter("@paymethod", SqlDbType.NVarChar, 4);
				parmArray[1].Value = paymethod;
				
				parmArray[2] = new SqlParameter("@systemvalue", SqlDbType.Money);
				parmArray[2].Value = systemvalue;
				
				parmArray[3] = new SqlParameter("@uservalue", SqlDbType.Money);
				parmArray[3].Value = uservalue;
				
				parmArray[4] = new SqlParameter("@depositvalue", SqlDbType.Money);
				parmArray[4].Value = depositvalue;
				
				parmArray[5] = new SqlParameter("@differencevalue", SqlDbType.Money);
				parmArray[5].Value = differencevalue;
				
				parmArray[6] = new SqlParameter("@reason", SqlDbType.NVarChar, 100);
				parmArray[6].Value = reason;

				parmArray[7] = new SqlParameter("@securitisedValue", SqlDbType.Money);
				parmArray[7].Value = securitisedValue;
				 
				
				this.RunSP(conn, trans, "DN_CashierTotalsBreakdownSaveSP", parmArray);
	
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		/// <summary>
		/// GetHistory
		/// </summary>
		/// <param name="empeeno">int</param>
		/// <returns>DataTable</returns>
		/// 
		public DataTable GetHistory (int empeeno)
		{
			DataTable dt = new DataTable();
			try
			{
				parmArray = new SqlParameter[1];
				
				parmArray[0] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[0].Value = empeeno;
				 
				
				this.RunSP("DN_CashierTotalsBreakdownGetHistorySP", parmArray, dt);
	
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return dt;
		}

		public DataTable Get (int cashiertotalsid, bool print)
		{
			DataTable dt = new DataTable();
			try
			{
				parmArray = new SqlParameter[1];
				
				parmArray[0] = new SqlParameter("@id", SqlDbType.Int);
				parmArray[0].Value = cashiertotalsid;
				 
				if(!print)
					this.RunSP("DN_CashierTotalsBreakdownGetSP", parmArray, dt);
				else
					this.RunSP("DN_CashierTotalsBreakdownGetForPrintSP", parmArray, dt);
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return dt;
		}

		/// <summary>
		/// GetBranchCashierBreakdown
		/// </summary>
		/// <param name="branchNo">short</param>
		/// <param name="dateStart">DateTime</param>
		/// <param name="dateEnd">DateTime</param>
		/// <returns>DataTable</returns>
		/// 
		public DataTable GetBranchCashierBreakdown(short branchNo, DateTime dateStart, DateTime dateEnd)
		{
			DataTable dt = new DataTable();
			try
			{
				parmArray = new SqlParameter[3];
				
				parmArray[0] = new SqlParameter("@piBranchNo", SqlDbType.SmallInt);
				parmArray[0].Value = branchNo;
				
				parmArray[1] = new SqlParameter("@piDateStart", SqlDbType.DateTime);
				parmArray[1].Value = dateStart;
				
				parmArray[2] = new SqlParameter("@piDateEnd", SqlDbType.DateTime);
				parmArray[2].Value = dateEnd;
				 
				
				this.RunSP("DN_CashierBreakdownByBranchSP", parmArray, dt);				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			
			return dt;
		}

	}
}