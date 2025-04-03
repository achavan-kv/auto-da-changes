using System;
using STL.DAL;
using STL.Common;
using STL.Common.Static;
using System.Data;
using System.Xml;
using System.Data.SqlClient;

namespace STL.BLL
{
	/// <summary>
	/// Summary description for BReceipt.
	/// </summary>
	public class BReceipt : CommonObject
	{
		public BReceipt()
		{
		}
		public DataSet TemporaryReceiptEnquiry(	int empoyeeNo, 
												int firstReceiptNo, 
												int lastReceiptNo)
		{
			DataSet ds = new DataSet();
			DTempReceipt tr =  new DTempReceipt();
			ds.Tables.Add(tr.TemporaryReceiptEnquiry(empoyeeNo,firstReceiptNo,lastReceiptNo));
			return ds;
		}
		public DataSet BailiffTemporaryReceiptEnquiry(int empoyeeNo)
		{
			DataSet ds = new DataSet();
			DTempReceipt tr =  new DTempReceipt();
			ds.Tables.Add(tr.BailiffTemporaryReceiptEnquiry(empoyeeNo));
			return ds;
		}
		public void VoidTempReceipt(SqlConnection conn, SqlTransaction trans,
			int receiptNo)
		{
			DTempReceipt tmpReceipt = new DTempReceipt();
			tmpReceipt.VoidTempReceipt(	conn, trans,
				receiptNo);
		}
		public void CancelTempReceipt(SqlConnection conn, SqlTransaction trans,
			int receiptNo)
		{
			DTempReceipt tmpReceipt = new DTempReceipt();
			tmpReceipt.User = this.User;
			tmpReceipt.CancelTempReceipt(	conn, trans,
				receiptNo,tmpReceipt.User);
		}
		public void AllocateTempReceipt(SqlConnection conn, SqlTransaction trans,
			int empeeNo,int branchNo,int firstReceiptNo,int lastReceiptNo,DateTime issueDate)
		{
			DTempReceipt tmpReceipt = new DTempReceipt();
			tmpReceipt.AllocateTempReceipt (conn, trans,
				 empeeNo, branchNo, firstReceiptNo, lastReceiptNo, issueDate);
		}
		public void ReallocateTempReceipt(SqlConnection conn, SqlTransaction trans,
			int empeeNo,int branchNo,int firstReceiptNo,int lastReceiptNo)
		{
			DTempReceipt tmpReceipt = new DTempReceipt();
			tmpReceipt.ReallocateTempReceipt (conn, trans,
				empeeNo, branchNo, firstReceiptNo, lastReceiptNo);
		}	
		public void CheckReceiptNotIssued( int firstReceiptNo,int lastReceiptNo, int checkOption, ref int issuedCount)
		{
			DTempReceipt tmpReceipt = new DTempReceipt();
			tmpReceipt.CheckReceiptNotIssued (firstReceiptNo, lastReceiptNo, checkOption, ref issuedCount);
		}
		public void GetNextTemporaryReceptNo(ref int nextRecpNo)
		{
			DTempReceipt tmpReceipt = new DTempReceipt();
			tmpReceipt.GetNextTemporaryReceptNo (ref nextRecpNo);
		}

	}
}
