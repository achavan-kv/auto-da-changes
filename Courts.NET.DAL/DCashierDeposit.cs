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
	/// Summary description for DCashierDeposit.
	/// </summary>
	public class DCashierDeposit : DALObject
	{
		public DCashierDeposit()
		{
		}

		/// <summary>
		/// Get
		/// </summary>
		/// <param name="empeeno">int</param>
		/// <param name="postedtofact">int</param>
		/// <param name="cashiertotals">int</param>
		/// <param name="datefrom">DateTime</param>
		/// <param name="dateto">DateTime</param>
		/// <returns>DataSet</returns>
		/// 
		public DataTable Get (int empeeno, short postedtofact, DateTime datefrom, DateTime dateto, short branchNo, string depositType, bool branchFloats, int paymentMethod)   //IP - 15/12/11 - #8810
		{
			DataTable dt = new DataTable(TN.CashierDeposits);
			
			try
			{
				parmArray = new SqlParameter[8];
				
				parmArray[0] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[0].Value = empeeno;
				
				parmArray[1] = new SqlParameter("@postedtofact", SqlDbType.SmallInt);
				parmArray[1].Value = postedtofact;
				
				parmArray[2] = new SqlParameter("@datefrom", SqlDbType.DateTime);
				parmArray[2].Value = datefrom;
				
				parmArray[3] = new SqlParameter("@dateto", SqlDbType.DateTime);
				parmArray[3].Value = dateto;
				
				parmArray[4] = new SqlParameter("@branchno", SqlDbType.SmallInt);
				parmArray[4].Value = Convert.ToInt16(branchNo);

				parmArray[5] = new SqlParameter("@deposittype", SqlDbType.NVarChar, 4);
				parmArray[5].Value = depositType;

				parmArray[6] = new SqlParameter("@branchfloats", SqlDbType.Bit);
				parmArray[6].Value = branchFloats;

                parmArray[7] = new SqlParameter("@paymentmethod", SqlDbType.Int);           //IP - 15/12/11 - #8810
                parmArray[7].Value = paymentMethod;

				this.RunSP("DN_CashierDepositsGetSP", parmArray, dt);
	
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			
			return dt;
		}

		/// <summary>
		/// Save
		/// </summary>
		/// <param name="datedeposit">DateTime</param>
		/// <param name="code">string</param>
		/// <param name="runno">int</param>
		/// <param name="empeeno">int</param>
		/// <param name="empeenoentered">int</param>
		/// <param name="cashiertotalid">int</param>
		/// <param name="empeenovoided">int</param>
		/// <param name="voided">string</param>
		/// <param name="datevoided">DateTime</param>
		/// <returns>int</returns>
		/// 
		public int Save (SqlConnection conn, SqlTransaction trans, DateTime datedeposit, string code, int runno, int empeeno, int empeenoentered, int cashiertotalid, int empeenovoided, string voided, DateTime datevoided, decimal depositvalue, short branchNo, string reference, string paymethod, short includeInCashierTotals, short isFloat)
		{
			int status = 0;
			
			try
			{
				parmArray = new SqlParameter[15];
				
				parmArray[0] = new SqlParameter("@datedeposit", SqlDbType.DateTime);
				parmArray[0].Value = datedeposit;
				
				parmArray[1] = new SqlParameter("@transtypecode", SqlDbType.NVarChar, 8);
				parmArray[1].Value = code;
				
				parmArray[2] = new SqlParameter("@runno", SqlDbType.Int);
				parmArray[2].Value = runno;
				
				parmArray[3] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[3].Value = empeeno;
				
				parmArray[4] = new SqlParameter("@empeenoentered", SqlDbType.Int);
				parmArray[4].Value = empeenoentered;
				
				parmArray[5] = new SqlParameter("@cashiertotalid", SqlDbType.Int);
				parmArray[5].Value = cashiertotalid;
				
				parmArray[6] = new SqlParameter("@empeenovoided", SqlDbType.Int);
				parmArray[6].Value = empeenovoided;
				
				parmArray[7] = new SqlParameter("@voided", SqlDbType.NChar, 2);
				parmArray[7].Value = voided;
				
				parmArray[8] = new SqlParameter("@datevoided", SqlDbType.DateTime);
				parmArray[8].Value = datevoided==DateTime.MinValue.AddYears(1899) ? DBNull.Value : (object)datevoided;

				parmArray[9] = new SqlParameter("@value", SqlDbType.Money);
				parmArray[9].Value = depositvalue;

				parmArray[10] = new SqlParameter("@branch", SqlDbType.SmallInt);
				parmArray[10].Value = branchNo;

				parmArray[11] = new SqlParameter("@reference", SqlDbType.NVarChar, 30);
				parmArray[11].Value = reference;

				parmArray[12] = new SqlParameter("@paymethod", SqlDbType.NVarChar, 8);
				parmArray[12].Value = paymethod;

				parmArray[13] = new SqlParameter("@includeincashiertotals", SqlDbType.SmallInt);
				parmArray[13].Value = includeInCashierTotals;

				parmArray[14] = new SqlParameter("@isCashierFloat", SqlDbType.SmallInt);
				parmArray[14].Value = isFloat;
				 
				status = this.RunSP(conn, trans, "DN_CashierDepositsSaveSP", parmArray);
	
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			
			return status;
		}

		/// <summary>
		/// Void
		/// </summary>
		/// <param name="depositid">int</param>
		/// <param name="datevoided">DateTime</param>
		/// <param name="voidedby">int</param>
		/// <returns>int</returns>
		/// 
		public int Void (int depositid, DateTime datevoided, int voidedby, bool reverse)
		{
			int status = 0;
			
			try
			{
				parmArray = new SqlParameter[4];
				
				parmArray[0] = new SqlParameter("@depositid", SqlDbType.Int);
				parmArray[0].Value = depositid;
				
				parmArray[1] = new SqlParameter("@datevoided", SqlDbType.DateTime);
				parmArray[1].Value = datevoided;
				
				parmArray[2] = new SqlParameter("@voidedby", SqlDbType.Int);
				parmArray[2].Value = voidedby;

				parmArray[3] = new SqlParameter("@reverse", SqlDbType.Bit);
				parmArray[3].Value = reverse;
				 
				status = this.RunSP("DN_CashierDepositVoidSP", parmArray);
	
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			
			return status;
		}

		public bool IsDepositReferenceUnique (string reference)
		{
			int status = 0;
			bool unique = false;
			
			try
			{
				parmArray = new SqlParameter[2];
				
				parmArray[0] = new SqlParameter("@reference", SqlDbType.VarChar, 50);
				parmArray[0].Value = reference;
				
				parmArray[1] = new SqlParameter("@unique", SqlDbType.SmallInt);
				parmArray[1].Value = 0;
				parmArray[1].Direction = ParameterDirection.Output;
				 
				status = this.RunSP("DN_CashierDepositIsReferenceUniqueSP", parmArray);
				if(parmArray[1].Value!=DBNull.Value)
					unique  = Convert.ToBoolean(parmArray[1].Value);
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			
			return unique;
		}

		public bool CashierMustDeposit (int empeeno)
		{
			bool mustdeposit = false;
			
			try
			{
				parmArray = new SqlParameter[2];
				
				parmArray[0] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[0].Value = empeeno;
				
				parmArray[1] = new SqlParameter("@mustdeposit", SqlDbType.Bit);
				parmArray[1].Value = mustdeposit;
				parmArray[1].Direction = ParameterDirection.Output;
				 
				this.RunSP("DN_CashierMustDepositSP", parmArray);
				
				if(parmArray[1].Value!=DBNull.Value)
					mustdeposit  = (bool)parmArray[1].Value;
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			
			return mustdeposit;
		}

		public decimal GetOutstandingSafeDeposits (int empeeno, short branchno)
		{
			decimal safeDeposits = 0;
		
			try
			{
				parmArray = new SqlParameter[4];
			
				parmArray[0] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[0].Value = empeeno;

				parmArray[1] = new SqlParameter("@paymethod", SqlDbType.NVarChar, 4);
				parmArray[1].Value = "";

				parmArray[2] = new SqlParameter("@branchno", SqlDbType.SmallInt);
				parmArray[2].Value = branchno;
			
				parmArray[3] = new SqlParameter("@safedeposits", SqlDbType.Money);
				parmArray[3].Value = 0;
				parmArray[3].Direction = ParameterDirection.Output;
				
				this.RunSP("DN_CashierSafeDepositsSP", parmArray);
			
				if(parmArray[3].Value!=DBNull.Value)
					safeDeposits  = (decimal)parmArray[3].Value;
			
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		
			return safeDeposits;
		}

		public void ReverseSafeDeposits (int empeeno, int user)
		{		
			try
			{
				parmArray = new SqlParameter[2];
			
				parmArray[0] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[0].Value = empeeno;
			
				parmArray[1] = new SqlParameter("@user", SqlDbType.Int);
				parmArray[1].Value = user;
				
				this.RunSP("DN_CashierReverseSafeDepositSP", parmArray);
			
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void IncludeDeposits(SqlConnection conn, SqlTransaction trans,
									int empNo, short includeDeposits)
		{		
			try
			{
				parmArray = new SqlParameter[2];
			
				parmArray[0] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[0].Value = empNo;
			
				parmArray[1] = new SqlParameter("@include", SqlDbType.SmallInt);
				parmArray[1].Value = includeDeposits;
				
				this.RunSP(conn, trans, "DN_CashierTotalsIncludeDeposit", parmArray);
			
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}
	}
}