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
	/// Summary description for DStorderProcess.
	/// </summary>
	public class DStorderProcess: DALObject
	{
		private DataTable _StorderTrans;
	/* 			
		private short _origBr = 0;
		public short OrigBr
		{
			get{return _origBr;}
			set{_origBr = value;}
		}
		private string _acctno = "";
		public string AccountNumber
		{
			get{return _acctno;}
			set{_acctno = value;}
		}
		private DateTime _datetrans = DateTime.Today;
		public DateTime DateTrans
		{
			get{return _datetrans;}
			set{_datetrans = value;}
		}
		private string _transcode = "";
		public string TransTypeCode
		{
			get	{return _transcode;}
			set{_transcode = value;}
		}
		private short _branchno = 0;
		public short BranchNumber
		{
			get{return _branchno;}
			set{_branchno = value;}
		}
		private int _transno = 0;
		public int TransRefNo
		{
			get{return _transno;}
			set{_transno = value;}
		}
		private decimal _transvalue = 0;
		public decimal TransValue
		{
			get{return _transvalue;}
			set{_transvalue = value;}
		}
		private int _runno = 0;
		public int RunNumber
		{
			get{return _runno;}
			set{_runno = value;}
		}
		private int _empeeno = 0;
		public int EmployeeNumber
		{
			get{return _empeeno;}
			set{_empeeno = value;}
		}
		private string _transupdated = "";
		public string TransUpdated
		{
			get	{return _transupdated;}
			set{_transupdated = value;}
		}
		private string _transprinted = "N";
		public string TransPrinted
		{
			get	{return _transprinted;}
			set{_transprinted = value;}
		}
		private string _bankcode = "";
		public string BankCode
		{
			get	{return _bankcode;}
			set{_bankcode = value;}
		}
		private string _bankacctno = "";
		public string BankAccountNumber
		{
			get	{return _bankacctno;}
			set{_bankacctno = value;}
		}
		private string _chequeno = "";
		public string ChequeNumber
		{
			get	{return _chequeno;}
			set{_chequeno = value;}
		}
		private string _ftnotes = "";
		public string FTNotes
		{
			get	{return _ftnotes;}
			set{_ftnotes = value;}
		}
		private short _paymethod = 0;
		public short PayMethod
		{
			get{return _paymethod;}
			set{_paymethod = value;}
		}
		private string _source = "";
		public string Source
		{
			get	{return _source;}
			set{_source = value;}
		}
		private string _error = "";
		public string Error
		{
			get	{return _error;}
			set{_error = value;}
		}
		private string _bankName = "";
		public string BankName
		{
			get	{return _bankName;}
			set{_bankName = value;}
		}
*/
		public DStorderProcess()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public DataTable StorderProcess
		{
			get
			{
				return _StorderTrans;
			}
		}

		/// <summary>
		/// Returns the Standing Order Transactions for a specific RunNo
		/// </summary>
		/// <param name="runNo"></param>
		/// <returns></returns>
		/// 
		public int GetByRunNo(SqlConnection conn, SqlTransaction trans, int runNo)
		{
			try
			{
				_StorderTrans = new DataTable(TN.StorderProcess);
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@RunNo", SqlDbType.Int ,4);
				parmArray[0].Value = runNo;
				
				if(conn!=null && trans!=null)
					result = this.RunSP(conn, trans, "DN_StorderProcess_GetByRunNo", parmArray, _StorderTrans);
				else
					result = this.RunSP("DN_StorderProcess_GetByRunNo", parmArray, _StorderTrans);

				if(result == 0)
				{
					result = (int)Return.Success;	
				}
				else
				{
					result = (int)Return.Fail;
				}
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}			
			return result;
		}
		/// <summary>
		/// Renames the raw Standing Order data files
		/// </summary>
		/// <param name=></param>
		/// <returns></returns>
		/// 
        //IP - 12/08/10 - CR1092 - COASTER to CoSACS Enhancements - added runNo
		public int RenameFiles(int runNo)
		{
			try
			{
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@runNo", SqlDbType.Int);
                parmArray[0].Value = runNo;

                result = this.RunSP("DN_StandingOrderRename", parmArray);

				if(result == 0)
				{
					result = (int)Return.Success;	
				}
				else
				{
					result = (int)Return.Fail;
				}
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}			
			return result;
		}

	}
}
