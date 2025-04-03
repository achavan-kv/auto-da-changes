using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ExchangeRate;

namespace STL.DAL
{
	/// <summary>
	/// Summary description for DExchangeTrans.
	/// </summary>
	public class DExchangeTrans : DALObject
	{
		private string _acctNo;
		private int _transRefNo;
		private DateTime _dateTrans;
		private int _payMethod;
		private decimal _foreignTender;
		private decimal _localChange;
		private int _branchNo;

		public string acctNo
		{
			get{return _acctNo;}
			set{_acctNo = value;}
		}

		public int transRefNo
		{
			get{return _transRefNo;}
			set{_transRefNo = value;}
		}

		public DateTime dateTrans
		{
			get{return _dateTrans;}
			set{_dateTrans = value;}
		}

		public int payMethod
		{
			get{return _payMethod;}
			set{_payMethod = value;}
		}

		public decimal foreignTender
		{
			get{return _foreignTender;}
			set{_foreignTender = value;}
		}

		public decimal localChange
		{
			get{return _localChange;}
			set{_localChange = value;}
		}

		public int branchNo
		{
			get{return _branchNo;}
			set{_branchNo = value;}
		}

		public DExchangeTrans()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public void SaveExchangeTrans(SqlConnection conn, SqlTransaction trans)
		{
			// Save a new Exchange Transaction to record foreign currency
			// taken and local currency returned as change. This will be used
			// to balance the Cashier Totals.
			try
			{
				parmArray = new SqlParameter[7];
				
				parmArray[0] = new SqlParameter("@piAcctNo", SqlDbType.NVarChar);
				parmArray[0].Value = this._acctNo;
				parmArray[1] = new SqlParameter("@piTransRefNo", SqlDbType.Int);
				parmArray[1].Value = this._transRefNo;
				parmArray[2] = new SqlParameter("@piDateTrans", SqlDbType.DateTime);
				parmArray[2].Value = this._dateTrans;
				parmArray[3] = new SqlParameter("@piPayMethod", SqlDbType.Int);
				parmArray[3].Value = this._payMethod;
				parmArray[4] = new SqlParameter("@piForeignTender", SqlDbType.Decimal);
				parmArray[4].Value = this._foreignTender;
				parmArray[5] = new SqlParameter("@piLocalChange", SqlDbType.Decimal);
				parmArray[5].Value = this._localChange;
				parmArray[6] = new SqlParameter("@piBranchNo", SqlDbType.SmallInt);
				parmArray[6].Value = this._branchNo;
				
				this.RunSP(conn, trans, "DN_ExchangeTransSaveSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}

		}

	}
}
