using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ExchangeRate;



namespace STL.DAL
{
	/// <summary>
	/// Summary description for DExchangeRate.
	/// </summary>
	public class DExchangeRate : DALObject
	{
		private string _currency = "";
		private decimal _rate = 0;
		private DateTime _dateFrom = System.DateTime.Now;
		private int _empeeNo = 0;
		//private string _status = RateStatus.Current;

		public int empeeNo
		{
			get{return _empeeNo;}
			set{_empeeNo = value;}
		}

		public DateTime dateFrom
		{
			get{return _dateFrom;}
			set{_dateFrom = value;}
		}

		public decimal rate
		{
			get{return _rate;}
			set{_rate = value;}
		}

		public string currency
		{
			get{return _currency;}
			set{_currency = value;}
		}


		public DExchangeRate()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public DataTable GetCurrent(SqlConnection conn, SqlTransaction trans)
		{
			// Returns a list of rates currently in use
			DataTable dt = new DataTable(TN.ExchangeRates);
			
			try
			{
				if (conn != null && trans != null)
					this.RunSP(conn, trans, "DN_ExchangeRateGetCurrentSP", dt);
				else
					this.RunSP("DN_ExchangeRateGetCurrentSP", dt);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return dt;
		}

		public decimal GetCurrentByCurrency(SqlConnection conn, SqlTransaction trans, 
												string currency )
		{
			decimal rate = 0;		
			try
			{
				parmArray = new SqlParameter[2];
				
				parmArray[0] = new SqlParameter("@currency", SqlDbType.NVarChar, 4);
				parmArray[0].Value = currency;

				parmArray[1] = new SqlParameter("@rate", SqlDbType.Float);
				parmArray[1].Value = 0;
				parmArray[1].Direction = ParameterDirection.Output;
				
				if(conn!=null && trans!=null)
					this.RunSP(conn, trans, "DN_ExchangeRateGetCurrentByCurrencySP", parmArray);
				else
					this.RunSP("DN_ExchangeRateGetCurrentByCurrencySP", parmArray);

				if(parmArray[1].Value != DBNull.Value)
					rate = Convert.ToDecimal(parmArray[1].Value);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return rate;
		}

		public DataTable GetHistory(string currency, DateTime dateFrom, DateTime dateTo)
		{
			// Returns a list of both current and historical rates
			// Optional filters are Currency, DateFrom and DateTo
			DataTable dt = new DataTable(TN.ExchangeRates);
			
			try
			{
				parmArray = new SqlParameter[3];
				
				parmArray[0] = new SqlParameter("@piCurrency", SqlDbType.NVarChar);
				parmArray[0].Value = currency;
				parmArray[1] = new SqlParameter("@piDateFrom", SqlDbType.DateTime);
				parmArray[1].Value = dateFrom;
				parmArray[2] = new SqlParameter("@piDateTo", SqlDbType.DateTime);
				parmArray[2].Value = dateTo;
				
				this.RunSP("DN_ExchangeRateGetHistorySP", parmArray, dt);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return dt;
		}

		public void SaveNewRate(SqlConnection conn, SqlTransaction trans)
		{
			// Add a new user entered rate as the current rate
			// Any previous version of this rate is set to a history status
			try
			{
				parmArray = new SqlParameter[3];
				
				parmArray[0] = new SqlParameter("@piCurrency", SqlDbType.NVarChar);
				parmArray[0].Value = this._currency;
				parmArray[1] = new SqlParameter("@piRate", SqlDbType.Decimal);
				parmArray[1].Value = this._rate;
				parmArray[2] = new SqlParameter("@piEmpeeNo", SqlDbType.Int);
				parmArray[2].Value = this._empeeNo;
				
				this.RunSP(conn, trans, "DN_ExchangeRateSaveSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}

		}

	}
}
