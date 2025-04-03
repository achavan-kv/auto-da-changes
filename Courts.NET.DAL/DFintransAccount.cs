using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common.Constants.TableNames;

namespace STL.DAL
{
	/// <summary>
	/// Summary description for DFintransAccount.
	/// </summary>
	public class DFintransAccount : DALObject
	{
		private string _acctNo = "";
		private string _linkedAcctNo = "";
		private DateTime _dateTrans = DateTime.Today;
		private short _branchNo = 0;
		private int _transRefNo = 0;

		public string acctNo
		{
			get{return _acctNo;}
			set{_acctNo = value;}
		}

		public string linkedAcctNo
		{
			get{return _linkedAcctNo;}
			set{_linkedAcctNo = value;}
		}

		public DateTime dateTrans
		{
			get{return _dateTrans;}
			set{_dateTrans = value;}
		}

		public short branchNumber
		{
			get{return _branchNo;}
			set{_branchNo = value;}
		}

		public int transRefNo
		{
			get{return _transRefNo;}
			set{_transRefNo = value;}
		}

		public DFintransAccount()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public DFintransAccount(string acctNo, string linkedAcctNo,
			DateTime dateTrans,	short branchNumber, int transRefNo)
		{
			//
			// Initialise properties
			//
			this._acctNo = acctNo;
			this._linkedAcctNo = linkedAcctNo;
			this._dateTrans = dateTrans;
			this._branchNo = branchNumber;
			this._transRefNo = transRefNo;
		}



		public void SaveAccountLink(SqlConnection conn, SqlTransaction trans)
		{
			// Save a relationship between two accounts for a DPY transaction.
			try
			{
				parmArray = new SqlParameter[5];
				
				parmArray[0] = new SqlParameter("@piAcctNo", SqlDbType.NVarChar);
				parmArray[0].Value = this._acctNo;
				parmArray[1] = new SqlParameter("@piLinkedAcctNo", SqlDbType.NVarChar);
				parmArray[1].Value = this._linkedAcctNo;
				parmArray[2] = new SqlParameter("@piDateTrans", SqlDbType.DateTime);
				parmArray[2].Value = this._dateTrans;
				parmArray[3] = new SqlParameter("@piBranchNo", SqlDbType.Int);
				parmArray[3].Value = this._branchNo;
				parmArray[4] = new SqlParameter("@piTransRefNo", SqlDbType.Int);
				parmArray[4].Value = this._transRefNo;
				
				this.RunSP(conn, trans, "DN_FinTransLinkSaveSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}

		}

		public string GetLink(string acctNo, DateTime dateTrans, short branchNo, int transRefNo)
		{
			// Retrieve a relationship between two accounts for a DPY transaction.
			// Returns the linked account no.
			try
			{
				parmArray = new SqlParameter[5];
				
				parmArray[0] = new SqlParameter("@piAcctNo", SqlDbType.NVarChar,12);
				parmArray[0].Value = acctNo;
				parmArray[1] = new SqlParameter("@piDateTrans", SqlDbType.DateTime);
				parmArray[1].Value = dateTrans;
				parmArray[2] = new SqlParameter("@piBranchNo", SqlDbType.Int);
				parmArray[2].Value = branchNo;
				parmArray[3] = new SqlParameter("@piTransRefNo", SqlDbType.Int);
				parmArray[3].Value = transRefNo;
				parmArray[4] = new SqlParameter("@poLinkedAcctNo", SqlDbType.NVarChar,12);
				parmArray[4].Value = "";
				parmArray[4].Direction = ParameterDirection.Output;

				this.RunSP("DN_FinTransLinkGetSP", parmArray);

				if(!Convert.IsDBNull(parmArray[4].Value))
					this.linkedAcctNo = (string)parmArray[4].Value;

				return this.linkedAcctNo;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}

		}
	}
}
