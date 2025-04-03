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
	/// Summary description for DCashierTotal.
	/// </summary>
	public class DCashierTotal : DALObject
	{
		public DCashierTotal()
		{
		}

		/// <summary>
		/// Save
		/// </summary>
		/// <param name="datefrom">DateTime</param>
		/// <param name="dateto">DateTime</param>
		/// <param name="empeeno">int</param>
		/// <param name="runno">int</param>
		/// <param name="empeenoauth">int</param>
		/// <param name="usertotal">double</param>
		/// <param name="systemtotal">double</param>
		/// <param name="difference">double</param>
		/// <param name="branchno">int</param>
		/// <param name="reason">string</param>
		/// <returns>int</returns>
		/// 
		public int Save (SqlConnection conn, SqlTransaction trans, DateTime datefrom, DateTime dateto, int empeeno, int runno, int empeenoauth, decimal usertotal, decimal systemtotal, decimal difference, decimal deposit, short branchno, out int identity)
		{
			int status = 0;
			identity = 0;

			try
			{
				parmArray = new SqlParameter[11];
				
				parmArray[0] = new SqlParameter("@datefrom", SqlDbType.DateTime);
				parmArray[0].Value = datefrom;
				
				parmArray[1] = new SqlParameter("@dateto", SqlDbType.DateTime);
				parmArray[1].Value = dateto;
				
				parmArray[2] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[2].Value = empeeno;
				
				parmArray[3] = new SqlParameter("@runno", SqlDbType.Int);
				parmArray[3].Value = runno;
				
				parmArray[4] = new SqlParameter("@empeenoauth", SqlDbType.Int);
				parmArray[4].Value = empeenoauth;
				
				parmArray[5] = new SqlParameter("@usertotal", SqlDbType.Money);
				parmArray[5].Value = usertotal;
				
				parmArray[6] = new SqlParameter("@systemtotal", SqlDbType.Money);
				parmArray[6].Value = systemtotal;
				
				parmArray[7] = new SqlParameter("@difference", SqlDbType.Money);
				parmArray[7].Value = difference;

				parmArray[8] = new SqlParameter("@deposit", SqlDbType.Money);
				parmArray[8].Value = deposit;
				
				parmArray[9] = new SqlParameter("@branchno", SqlDbType.SmallInt);
				parmArray[9].Value = branchno;

				parmArray[10] = new SqlParameter("@identity", SqlDbType.Int);
				parmArray[10].Value = identity;
				parmArray[10].Direction = ParameterDirection.Output;
				 
				status = this.RunSP(conn, trans, "DN_CashierTotalsSaveSP", parmArray);

				if(parmArray[10].Value!=DBNull.Value)
					identity = (int)parmArray[10].Value;	
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			
			return status;
		}

		/// <summary>
		/// GetHistory
		/// </summary>
		/// <param name="empeeno">int</param>
		/// <returns>DataSet</returns>
		/// 
		public DataTable GetHistory (int empeeno)
		{
			DataTable dt = new DataTable();
			
			
			try
			{
				parmArray = new SqlParameter[1];
				
				parmArray[0] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[0].Value = empeeno;
				 
				this.RunSP("DN_CashierTotalsGetHistorySP", parmArray, dt);
	
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			
			return dt;
		}

		/// <summary>
		/// GetReversal
		/// </summary>
		/// <param name="empeeno">int</param>
		/// <returns>DataTable</returns>
		/// 
		public DataTable GetReversal (int empeeno)
		{
			DataTable dt = new DataTable();
			
			
			try
			{
				parmArray = new SqlParameter[1];
				
				parmArray[0] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[0].Value = empeeno;
				 
				this.RunSP("DN_CashierTotalsGetReversalSP", parmArray, dt);
	
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			
			return dt;
		}

		/// <summary>
		/// SaveReversal
		/// </summary>
		/// <param name="empeeno">int</param>
		/// <param name="difference">decimal</param>
		/// <returns>int</returns>
		/// 
		public int SaveReversal (SqlConnection conn, SqlTransaction trans, int empeeNo, out decimal difference)
		{
			int status = 0;
			difference = 0;

			try
			{
				parmArray = new SqlParameter[2];
				
				parmArray[0] = new SqlParameter("@piEmployeeNo", SqlDbType.Int);
				parmArray[0].Value = empeeNo;
				
				parmArray[1] = new SqlParameter("@poDifference", SqlDbType.Money);
				parmArray[1].Value = 0;
				parmArray[1].Direction = ParameterDirection.Output;
								 
				status = this.RunSP(conn, trans, "DN_CashierTotalsReverseSP", parmArray);

				if(parmArray[1].Value != DBNull.Value)
					difference = (decimal)parmArray[1].Value;	
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			
			return status;
		}


		/// <summary>
		/// GetUnexportedTotals
		/// </summary>
		/// <param name="branchno">int</param>
		/// <param name="totals">double</param>
		/// <param name="deposits">double</param>
		/// <returns>int</returns>
		/// 
		public DataSet GetUnexportedTotals (int branchno)
		{
			//int status;  = 0;
			DataSet ds = new DataSet();
			
			try
			{
				parmArray = new SqlParameter[1];
				
				parmArray[0] = new SqlParameter("@branchno", SqlDbType.SmallInt);
				parmArray[0].Value = branchno;
				
				this.RunSP("DN_CashierTotalsGetUnexportedTotalsSP", parmArray, ds);
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			
			return ds;
		}

		/// <summary>
		/// GetUnexported
		/// </summary>
		/// <param name="branchno">int</param>
		/// <param name="total">double</param>
		/// <returns>DataSet</returns>
		/// 
		public DataSet GetUnexported (short branchno, out decimal total)
		{
			DataSet ds = new DataSet();
			
			total = 0;
			
			try
			{
				parmArray = new SqlParameter[2];
				
				parmArray[0] = new SqlParameter("@branchno", SqlDbType.SmallInt);
				parmArray[0].Value = branchno;
				
				parmArray[1] = new SqlParameter("@total", SqlDbType.Money);
				parmArray[1].Value = total;
				parmArray[1].Direction = ParameterDirection.Output; 
				
				this.RunSP("DN_CashierTotalsGetUnexportedSP", parmArray, ds);
	
				if(parmArray[1].Value!=DBNull.Value)
					total = (decimal)parmArray[1].Value;
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			
			return ds;
		}

		/// <summary>
		/// GetSummary
		/// </summary>
		/// <param name="branch">int</param>
		/// <param name="datefrom">DateTime</param>
		/// <param name="dateto">DateTime</param>
		/// <returns>DataSet</returns>
		/// 
		public DataTable GetSummary ( short branch, 
									DateTime datefrom, 
									DateTime dateto)
		{
			DataTable dt = new DataTable(TN.CashierTotals);
			try
			{
				parmArray = new SqlParameter[3];
				
				parmArray[0] = new SqlParameter("@branch", SqlDbType.SmallInt);
				parmArray[0].Value = branch;
				
				parmArray[1] = new SqlParameter("@datefrom", SqlDbType.DateTime);
				parmArray[1].Value = datefrom;
				
				parmArray[2] = new SqlParameter("@dateto", SqlDbType.DateTime);
				parmArray[2].Value = dateto;
				 
				
				this.RunSP("DN_CashierTotalsSummarySP", parmArray, dt);				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			
			return dt;
		}

		/// <summary>
		/// GetBranchCashierList
		/// </summary>
		/// <param name="branchNo">short</param>
		/// <param name="dateStart">DateTime</param>
		/// <param name="dateEnd">DateTime</param>
		/// <returns>DataTable</returns>
		/// 
		public DataTable GetBranchCashierList(short branchNo, DateTime dateStart, DateTime dateEnd)
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
				 
				
				this.RunSP("DN_CashierGetByBranchSP", parmArray, dt);				
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