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
	/// Summary description for DGiftVoucher.
	/// </summary>
	public class DGiftVoucher : DALObject
	{
		public DGiftVoucher()
		{
		}

		private decimal _value = 0;
		public decimal Value 
		{
			get{ return _value;}
		}

		private int _empeenosold = 0;
		public int EmpeeNoSold
		{
			get{ return _empeenosold;}
		}

		private DateTime _datesold = DateTime.MinValue.AddYears(1899);
		public DateTime DateSold
		{
			get{ return _datesold;}
		}

		private int _empeenoredeemed = 0;
		public int EmpeeNoRedeemed
		{
			get{ return _empeenoredeemed;}
		}

		private DateTime _dateredeemed = DateTime.MinValue.AddYears(1899);
		public DateTime DateRedeemed
		{
			get{ return _dateredeemed;}
		}

		private int _empeenoauth = 0;
		public int EmpeeNoAuth
		{
			get{ return _empeenoauth;}
		}

		private string _acctnosold = "";
		public string AccountNoSold 
		{
			get{return _acctnosold;}
		}

		private string _acctnoredeemed = "";
		public string AccountNoRedeemed
		{
			get{return _acctnoredeemed;}
		}

		private DateTime _dateexpiry = DateTime.MinValue.AddYears(1899);
		public DateTime DateExpiry
		{
			get{ return _dateexpiry;}
		}

		private bool _writtenoff = false;
		public bool WrittenOff
		{
			get{return _writtenoff;}
		}

		private DateTime _datewrittenoff = DateTime.MinValue.AddYears(1899);
		public DateTime DateWrittenOff
		{
			get{ return _datewrittenoff;}
		}

		private string _acctnocompany = "";
		public string AccountNoCompany
		{
			get{return _acctnocompany;}
		}

		private DateTime _dateconfirmed = DateTime.MinValue.AddYears(1899);
		public DateTime DateConfirmed
		{
			get{ return _dateconfirmed;}
		}

		private DateTime _datevoided = DateTime.MinValue.AddYears(1899);
		public DateTime DateVoided
		{
			get{ return _datevoided;}
		}

		private int _transrefnoredeemed = 0;
		public int TransRefNoRedeemed
		{
			get{return _transrefnoredeemed;}
		}

		private bool _free = false;
		public bool Free
		{
			get{return _free;}
		}

		public void VoidOther( SqlConnection conn, SqlTransaction trans, 
								string redeemedAccountNo, int oldRefNo )
		{
			try
			{
				parmArray = new SqlParameter[2];
				
				parmArray[0] = new SqlParameter("@acctnoredeemed", SqlDbType.NVarChar, 12);
				parmArray[0].Value = redeemedAccountNo;
				
				parmArray[1] = new SqlParameter("@refno", SqlDbType.Int);
				parmArray[1].Value = oldRefNo;
				
				this.RunSP(conn, trans, "DN_GiftVoucherVoidOtherSP", parmArray);

			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		/// <summary>
		/// Validate
		/// </summary>
		/// <param name="reference">string</param>
		/// <param name="courts">int</param>
		/// <param name="value">double</param>
		/// <param name="expiry">DateTime</param>
		/// <returns>void</returns>
		/// 
		public void Validate (string reference, bool courts, out decimal voucherValue, out DateTime expiry, bool includeRedeemed, out bool redeemed)
		{
			voucherValue = 0;
			expiry = DateTime.MinValue.AddYears(1899);
			redeemed = false;

			try
			{
				parmArray = new SqlParameter[6];
				
				parmArray[0] = new SqlParameter("@reference", SqlDbType.NVarChar, 64);
				parmArray[0].Value = reference;
				
				parmArray[1] = new SqlParameter("@courts", SqlDbType.SmallInt);
				parmArray[1].Value = courts;
				
				parmArray[2] = new SqlParameter("@value", SqlDbType.Money);
				parmArray[2].Value = voucherValue;
				parmArray[2].Direction = ParameterDirection.Output;

				parmArray[3] = new SqlParameter("@expiry", SqlDbType.DateTime);
				parmArray[3].Value = expiry;
				parmArray[3].Direction = ParameterDirection.Output;

				parmArray[4] = new SqlParameter("@includeredeemed", SqlDbType.Bit);
				parmArray[4].Value = includeRedeemed;

				parmArray[5] = new SqlParameter("@redeemed", SqlDbType.DateTime);
				parmArray[5].Direction = ParameterDirection.Output;
				 
				
				this.RunSP("DN_GiftVoucherValidateSP", parmArray);
	
				if(parmArray[2].Value!=DBNull.Value)
					voucherValue = (decimal)parmArray[2].Value;
				if(parmArray[3].Value!=DBNull.Value)
					expiry = (DateTime)parmArray[3].Value;
				if(parmArray[5].Value!=DBNull.Value)
					redeemed = true;
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		/// <summary>
		/// Redeem
		/// </summary>
		/// <param name="reference">string</param>
		/// <param name="courts">int</param>
		/// <param name="empeenoredeemed">int</param>
		/// <param name="dateredeemed">DateTime</param>
		/// <param name="empeenoauth">int</param>
		/// <param name="acctnoredeemed">string</param>
		/// <returns>void</returns>
		/// 
		public void Redeem (SqlConnection conn, SqlTransaction trans, 
							string reference, bool courts, 
							int empeenoredeemed, DateTime dateredeemed, 
							int empeenoauth, string acctnoredeemed,
							string acctnocompany, int transrefnoredeemed )
		{
			try
			{
				parmArray = new SqlParameter[8];
				
				parmArray[0] = new SqlParameter("@reference", SqlDbType.NVarChar, 64);
				parmArray[0].Value = reference;
				
				parmArray[1] = new SqlParameter("@courts", SqlDbType.SmallInt);
				parmArray[1].Value = Convert.ToInt16(courts);
				
				parmArray[2] = new SqlParameter("@empeenoredeemed", SqlDbType.Int);
				parmArray[2].Value = empeenoredeemed;
				
				parmArray[3] = new SqlParameter("@dateredeemed", SqlDbType.DateTime);
				parmArray[3].Value = dateredeemed;
				
				parmArray[4] = new SqlParameter("@empeenoauth", SqlDbType.Int);
				parmArray[4].Value = empeenoauth;
				
				parmArray[5] = new SqlParameter("@acctnoredeemed", SqlDbType.NVarChar, 24);
				parmArray[5].Value = acctnoredeemed;

				parmArray[6] = new SqlParameter("@acctnocompany", SqlDbType.NVarChar, 24);
				parmArray[6].Value = acctnocompany;

				parmArray[7] = new SqlParameter("@transrefnoredeemed", SqlDbType.Int);
				parmArray[7].Value = transrefnoredeemed;
				 
				if(conn!=null && trans!=null)
					RunSP(conn, trans, "DN_GiftVoucherRedeemSP", parmArray);
				else
					RunSP("DN_GiftVoucherRedeemSP", parmArray);	
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		/// <summary>
		/// Populate
		/// </summary>
		/// <param name="reference">string</param>
		/// <param name="courts">int</param>
		/// <param name="value">double</param>
		/// <param name="empeenosold">int</param>
		/// <param name="datesold">DateTime</param>
		/// <param name="empeenoredeemed">int</param>
		/// <param name="dateredeemed">DateTime</param>
		/// <param name="empeenoauth">int</param>
		/// <param name="acctnosold">string</param>
		/// <param name="acctnoredeemed">string</param>
		/// <param name="dateexpiry">DateTime</param>
		/// <param name="writtenoff"></param>
		/// <param name="datewrittenoff">DateTime</param>
		/// <param name="acctnocompany">string</param>
		/// <param name="dateconfirmed">DateTime</param>
		/// <param name="datevoided">DateTime</param>
		/// <returns>void</returns>
		/// 
		public void Populate (SqlConnection conn, SqlTransaction trans, 
								string reference, bool courts,
								string acctnocompany )
		{
			try
			{
				parmArray = new SqlParameter[18];
				
				parmArray[0] = new SqlParameter("@reference", SqlDbType.NVarChar, 64);
				parmArray[0].Value = reference;
				
				parmArray[1] = new SqlParameter("@courts", SqlDbType.SmallInt);
				parmArray[1].Value = Convert.ToInt16(courts);
				
				parmArray[2] = new SqlParameter("@value", SqlDbType.Money);
				parmArray[2].Value = 0;
				parmArray[2].Direction = ParameterDirection.Output;

				parmArray[3] = new SqlParameter("@empeenosold", SqlDbType.Int);
				parmArray[3].Value = 0;
				parmArray[3].Direction = ParameterDirection.Output;

				parmArray[4] = new SqlParameter("@datesold", SqlDbType.DateTime);
				parmArray[4].Value = null;
				parmArray[4].Direction = ParameterDirection.Output;

				parmArray[5] = new SqlParameter("@empeenoredeemed", SqlDbType.Int);
				parmArray[5].Value = 0;
				parmArray[5].Direction = ParameterDirection.Output;

				parmArray[6] = new SqlParameter("@dateredeemed", SqlDbType.DateTime);
				parmArray[6].Value = null;
				parmArray[6].Direction = ParameterDirection.Output;

				parmArray[7] = new SqlParameter("@empeenoauth", SqlDbType.Int);
				parmArray[7].Value = 0;
				parmArray[7].Direction = ParameterDirection.Output;

				parmArray[8] = new SqlParameter("@acctnosold", SqlDbType.NVarChar, 24);
				parmArray[8].Value = "";
				parmArray[8].Direction = ParameterDirection.Output;

				parmArray[9] = new SqlParameter("@acctnoredeemed", SqlDbType.NVarChar, 24);
				parmArray[9].Value = "";
				parmArray[9].Direction = ParameterDirection.Output;

				parmArray[10] = new SqlParameter("@dateexpiry", SqlDbType.DateTime);
				parmArray[10].Value = null;
				parmArray[10].Direction = ParameterDirection.Output;

				parmArray[11] = new SqlParameter("@writtenoff", SqlDbType.Bit);
				parmArray[11].Value = false;
				parmArray[11].Direction = ParameterDirection.Output;

				parmArray[12] = new SqlParameter("@datewrittenoff", SqlDbType.DateTime);
				parmArray[12].Value = null;
				parmArray[12].Direction = ParameterDirection.Output;

				parmArray[13] = new SqlParameter("@acctnocompany", SqlDbType.NVarChar, 24);
				parmArray[13].Value = acctnocompany;

				parmArray[14] = new SqlParameter("@dateconfirmed", SqlDbType.DateTime);
				parmArray[14].Value = null;
				parmArray[14].Direction = ParameterDirection.Output;

				parmArray[15] = new SqlParameter("@datevoided", SqlDbType.DateTime);
				parmArray[15].Value = null;
				parmArray[15].Direction = ParameterDirection.Output; 

				parmArray[16] = new SqlParameter("@transrefnoredeemed", SqlDbType.Int);
				parmArray[16].Value = 0;
				parmArray[16].Direction = ParameterDirection.Output; 

				parmArray[17] = new SqlParameter("@free", SqlDbType.Bit);
				parmArray[17].Value = 0;
				parmArray[17].Direction = ParameterDirection.Output; 
				
				this.RunSP(conn, trans, "DN_GiftVoucherPopulateSP", parmArray);
	
				if(parmArray[2].Value!=DBNull.Value)
					_value = (decimal)parmArray[2].Value;
				if(parmArray[3].Value!=DBNull.Value)
					_empeenosold = (int)parmArray[3].Value;
				if(parmArray[4].Value!=DBNull.Value)
					_datesold = (DateTime)parmArray[4].Value;
				if(parmArray[5].Value!=DBNull.Value)
					_empeenoredeemed = (int)parmArray[5].Value;
				if(parmArray[6].Value!=DBNull.Value)
					_dateredeemed = (DateTime)parmArray[6].Value;
				if(parmArray[7].Value!=DBNull.Value)
					_empeenoauth = (int)parmArray[7].Value;
				if(parmArray[8].Value!=DBNull.Value)
					_acctnosold = (string)parmArray[8].Value;
				if(parmArray[9].Value!=DBNull.Value)
					_acctnoredeemed = (string)parmArray[9].Value;
				if(parmArray[10].Value!=DBNull.Value)
					_dateexpiry = (DateTime)parmArray[10].Value;
				if(parmArray[11].Value!=DBNull.Value)
					_writtenoff = (bool)parmArray[11].Value;
				if(parmArray[12].Value!=DBNull.Value)
					_datewrittenoff = (DateTime)parmArray[12].Value;
				if(parmArray[14].Value!=DBNull.Value)
					_dateconfirmed = (DateTime)parmArray[14].Value;
				if(parmArray[15].Value!=DBNull.Value)
					_datevoided = (DateTime)parmArray[15].Value;
				if(parmArray[16].Value!=DBNull.Value)
					_transrefnoredeemed = (int)parmArray[16].Value;
				if(parmArray[17].Value!=DBNull.Value)
					_free = (bool)parmArray[17].Value;
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		/// <summary>
		/// InsertCourts
		/// </summary>
		/// <param name="reference">string</param>
		/// <param name="value">double</param>
		/// <param name="empeenosold">int</param>
		/// <param name="dateexpiry">DateTime</param>
		/// <param name="countrycode">string</param>
		/// <returns>void</returns>
		/// 
		public void InsertCourts (SqlConnection conn, SqlTransaction trans, 
									string reference, decimal voucherValue, 
									int empeenosold, DateTime dateexpiry, 
									string countrycode, bool free)
		{
			try
			{
				parmArray = new SqlParameter[6];
				
				parmArray[0] = new SqlParameter("@reference", SqlDbType.NVarChar, 40);
				parmArray[0].Value = reference;
				
				parmArray[1] = new SqlParameter("@value", SqlDbType.Money);
				parmArray[1].Value = voucherValue;
				
				parmArray[2] = new SqlParameter("@empeenosold", SqlDbType.Int);
				parmArray[2].Value = empeenosold;
				
				parmArray[3] = new SqlParameter("@dateexpiry", SqlDbType.DateTime);
				parmArray[3].Value = dateexpiry;
				
				parmArray[4] = new SqlParameter("@countrycode", SqlDbType.NVarChar, 4);
				parmArray[4].Value = countrycode;

				parmArray[5] = new SqlParameter("@free", SqlDbType.Bit);
				parmArray[5].Value = free;
				 
				
				this.RunSP(conn, trans, "DN_GiftVoucherCourtsInsertSP", parmArray);	
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void InsertOther (SqlConnection conn, SqlTransaction trans, 
								string reference, decimal voucherValue, 
								string acctNoCompany)
		{
			try
			{
				parmArray = new SqlParameter[3];
				
				parmArray[0] = new SqlParameter("@reference", SqlDbType.NVarChar, 40);
				parmArray[0].Value = reference;
				
				parmArray[1] = new SqlParameter("@value", SqlDbType.Money);
				parmArray[1].Value = voucherValue;
				
				parmArray[2] = new SqlParameter("@acctnocompany", SqlDbType.NVarChar, 12);
				parmArray[2].Value = acctNoCompany;
				
				this.RunSP(conn, trans, "DN_GiftVoucherOtherInsertSP", parmArray);	
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}
	}
}