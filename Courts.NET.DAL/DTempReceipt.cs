using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common.Constants.Enums;
using STL.Common.Constants.TableNames;
using STL.Common.Static;

namespace STL.DAL
{
	/// <summary>
	/// Summary description for DTempReceipt.
	/// </summary>
	public class DTempReceipt : DALObject
	{
		public DTempReceipt()
		{
		}

		public DataTable GetTempReceipt(int receiptNo)
		{
			DataTable dt = null;
			try
			{
				dt = new DataTable(TN.TempReceipt);
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@receiptNo", SqlDbType.Int);
				parmArray[0].Value = receiptNo;
				this.RunSP("DN_TempReceiptGetSP", parmArray, dt);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return dt;
		}

		public void PayTempReceipt(SqlConnection conn, SqlTransaction trans,
									string accountNo, decimal paymentAmount, 
									int receiptNo, DateTime datePresented)
		{
			try
			{
				parmArray = new SqlParameter[4];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@receiptno", SqlDbType.Int);
				parmArray[1].Value = receiptNo;
				parmArray[2] = new SqlParameter("@payment", SqlDbType.Money);
				parmArray[2].Value = paymentAmount;
				parmArray[3] = new SqlParameter("@date", SqlDbType.DateTime);
				parmArray[3].Value = datePresented;
				this.RunSP(conn, trans, "DN_TempReceiptPaySP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}
		public DataTable TemporaryReceiptEnquiry(int empoyeeNo, 
											int firstReceiptNo, 
											int lastReceiptNo)
		{
			DataTable dt = null;
			try
			{
				dt = new DataTable(TN.TempReceipt);
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[0].Value = empoyeeNo;
				parmArray[1] = new SqlParameter("@firstreceipt", SqlDbType.Int);
				parmArray[1].Value = firstReceiptNo;
				parmArray[2] = new SqlParameter("@lastreceipt", SqlDbType.Int);
				parmArray[2].Value = lastReceiptNo;
				this.RunSP( "DN_TemporaryReceiptEnquirySP", parmArray,dt);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return dt;
		}

		public DataTable BailiffTemporaryReceiptEnquiry(int empoyeeNo)
		{
			DataTable dt = null;
			try
			{
				dt = new DataTable(TN.TempReceipt);
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[0].Value = empoyeeNo;
				this.RunSP( "DN_BailiffTemporaryReceiptEnquirySP", parmArray,dt);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return dt;
		}
		public void CancelTempReceipt(SqlConnection conn, SqlTransaction trans,
			int receiptNo, int empeeNo)
		{
			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@receiptno", SqlDbType.Int);
				parmArray[0].Value = receiptNo;
				parmArray[1] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[1].Value = empeeNo;
				this.RunSP(conn, trans, "DN_CancleTemporaryReceiptSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}
		public void VoidTempReceipt(SqlConnection conn, SqlTransaction trans,
			int receiptNo)
		{
			try
			{
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@receiptno", SqlDbType.Int);
				parmArray[0].Value = receiptNo;
				this.RunSP(conn, trans, "DN_VoidTemporaryReceiptSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}
		public void AllocateTempReceipt(SqlConnection conn, SqlTransaction trans,
			int empeeNo,int branchNo,int firstReceiptNo,int lastReceiptNo,DateTime issueDate)
		{
			try
			{
				parmArray = new SqlParameter[5];
				parmArray[0] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[0].Value = empeeNo;
				parmArray[1] = new SqlParameter("@branchno", SqlDbType.Int);
				parmArray[1].Value = branchNo;
				parmArray[2] = new SqlParameter("@firstreceiptno", SqlDbType.Int);
				parmArray[2].Value = firstReceiptNo;
				parmArray[3] = new SqlParameter("@lastreceiptno", SqlDbType.Int);
				parmArray[3].Value = lastReceiptNo;
				parmArray[4] = new SqlParameter("@issuedate", SqlDbType.DateTime);
				parmArray[4].Value = issueDate;
				this.RunSP(conn, trans, "DN_AllocateReceiptSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}
		public void ReallocateTempReceipt(SqlConnection conn, SqlTransaction trans,
			int empeeNo,int branchNo,int firstReceiptNo,int lastReceiptNo)
		{
			try
			{
				parmArray = new SqlParameter[4];
				parmArray[0] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[0].Value = empeeNo;
				parmArray[1] = new SqlParameter("@branchno", SqlDbType.Int);
				parmArray[1].Value = branchNo;
				parmArray[2] = new SqlParameter("@firstreceiptno", SqlDbType.Int);
				parmArray[2].Value = firstReceiptNo;
				parmArray[3] = new SqlParameter("@lastreceiptno", SqlDbType.Int);
				parmArray[3].Value = lastReceiptNo;
				this.RunSP(conn, trans, "DN_ReallocateReceiptSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}
		public void CheckReceiptNotIssued(int firstReceiptNo, int lastReceiptNo, int checkOption, ref int issuedCount)
		{
			try
			{
				parmArray = new SqlParameter[4];
				parmArray[0] = new SqlParameter("@firstreceiptno", SqlDbType.Int);
				parmArray[0].Value = firstReceiptNo;
				parmArray[1] = new SqlParameter("@lastreceiptno", SqlDbType.Int);
				parmArray[1].Value = lastReceiptNo;
				parmArray[2] = new SqlParameter("@checkoption", SqlDbType.Int);
				parmArray[2].Value = checkOption;
				parmArray[3] = new SqlParameter("@issuedCount", SqlDbType.Int);
				parmArray[3].Value = issuedCount;
				parmArray[3].Direction = ParameterDirection.Output;
				this.RunSP("DN_CheckReceiptNotIssuedSP", parmArray);
				if(parmArray[3].Value!=DBNull.Value)
					issuedCount = Convert.ToInt32(parmArray[3].Value);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}
		public void GetNextTemporaryReceptNo(ref int nextRecpNo)
		{
			try
			{
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@nextrecpno", SqlDbType.Int);
				parmArray[0].Value = nextRecpNo;
				parmArray[0].Direction = ParameterDirection.Output;
				this.RunSP("DN_GetNextTemporaryReceptNoSP", parmArray);
				if(parmArray[0].Value!=DBNull.Value)
					nextRecpNo = Convert.ToInt32(parmArray[0].Value);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}
	}
}
