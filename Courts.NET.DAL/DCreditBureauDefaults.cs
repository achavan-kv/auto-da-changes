using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common;
using STL.Common.Constants.TableNames;

namespace STL.DAL
{
	/// <summary>
	/// Summary description for DCreditBureauDefaults.
	/// </summary>
	public class DCreditBureauDefaults : DALObject
	{
		public DCreditBureauDefaults()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		/// <summary>
		/// Save
		/// </summary>
		/// <param name="custid">string</param>
		/// <param name="status">string</param>
		/// <param name="defaultsBalance">double</param>
		/// <param name="defaults">int</param>
		/// <param name="defaultsExMotorBalance">double</param>
		/// <param name="defaultsExMotor">int</param>
		/// <returns>void</returns>
		/// 
		public void Save (SqlConnection conn, SqlTransaction trans, string custid, string status, decimal defaultsBalance, short defaults, decimal defaultsExMotorBalance, short defaultsExMotor)
		{
			try
			{
				parmArray = new SqlParameter[6];
				
				parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, 40);
				parmArray[0].Value = custid;
				
				parmArray[1] = new SqlParameter("@status", SqlDbType.NVarChar, 16);
				parmArray[1].Value = status;
				
				parmArray[2] = new SqlParameter("@defaultsBalance", SqlDbType.Money);
				parmArray[2].Value = defaultsBalance;
				
				parmArray[3] = new SqlParameter("@defaults", SqlDbType.SmallInt);
				parmArray[3].Value = defaults;
				
				parmArray[4] = new SqlParameter("@defaultsExMotorBalance", SqlDbType.Money);
				parmArray[4].Value = defaultsExMotorBalance;
				
				parmArray[5] = new SqlParameter("@defaultsExMotor", SqlDbType.SmallInt);
				parmArray[5].Value = defaultsExMotor;
				 
				
				this.RunSP(conn, trans, "DN_CreditBureauDefaultsSaveSP", parmArray);
	
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		/// <summary>
		/// Get
		/// </summary>
		/// <param name="custid">string</param>
		/// <returns>DataTable</returns>
		/// 
		public DataTable Get (string custid)
		{
			DataTable dt = new DataTable(TN.CreditBureauDefaults);
			try
			{
				parmArray = new SqlParameter[1];
				
				parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, 40);
				parmArray[0].Value = custid;
				 
				
				this.RunSP("DN_CreditBureauDefaultsSelectSP", parmArray, dt);
	
				
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