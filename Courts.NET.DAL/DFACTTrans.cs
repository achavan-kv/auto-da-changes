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
	/// Summary description for DFACTTrans.
	/// </summary>
	public class DFACTTrans : DALObject
	{
		private string _acctNo = "";
		public string AccountNumber
		{
			get{return _acctNo;}
			set{_acctNo = value;}
		}
		private int _agreementNo = 1;
		public int AgreementNumber
		{
			get{return _agreementNo;}
			set{_agreementNo = value;}
		}
		private int _buffno = 0;
		public int BuffNo
		{
			get{return _buffno;}
			set{_buffno = value;}
		}
		private string _itemNo = "";
		public string ItemNumber
		{
			get{return _itemNo;}
			set{_itemNo = value;}
		}
		private double _quantity = 0;
		public double Quantity
		{
			get{return _quantity;}
			set{_quantity = value;}
		}
		private double _price = 0;
		public double Price
		{
			get{return _price;}
			set{_price = value;}
		}
		private double _value = 0;
		public double Value
		{
			get{return _value;}
			set{_value = value;}
		}
		private double _taxamt = 0;
		public double TaxAmt
		{
			get{return _taxamt;}
			set{_taxamt = value;}
		}
		private short _stockLocn = 0;
		public short StockLocation
		{
			get{return _stockLocn;}
			set{_stockLocn = value;}
		}
		private string _trantype = "";
		public string TranType
		{
			get{return _trantype;}
			set{_trantype = value;}
		}
		private string _tccode = "";
		public string TCCode
		{
			get{return _tccode;}
			set{_tccode = value;}
		}
		private DateTime _trandate = DateTime.MinValue.AddYears(1899);
		public DateTime TranDate
		{
			get{return _trandate;}
			set{_trandate = value;}
		}

		public int GetOrderNo(SqlConnection conn, SqlTransaction trans, 
								string accountNo, int agreementNo)
		{
			result = 0;
			try
			{
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@agreementno", SqlDbType.Int);
				parmArray[1].Value = agreementNo;
				parmArray[2] = new SqlParameter("@buffno", SqlDbType.Int);
				parmArray[2].Value = 0;
				parmArray[2].Direction = ParameterDirection.Output;
						
				RunSP(conn, trans, "DN_FactTransGetOrderNoSP", parmArray);

				if(parmArray[2].Value!=DBNull.Value)
					result = (int)parmArray[2].Value;				
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}			
			return result;
		}

		public void Save(SqlConnection conn, SqlTransaction trans)
		{
			try
			{
				parmArray = new SqlParameter[12];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar,12);
				parmArray[0].Value = this.AccountNumber;
				parmArray[1] = new SqlParameter("@agrmtno", SqlDbType.Int);
				parmArray[1].Value = this.AgreementNumber;
				parmArray[2] = new SqlParameter("@buffno", SqlDbType.Int);
				parmArray[2].Value = this.BuffNo;
				parmArray[3] = new SqlParameter("@itemno", SqlDbType.VarChar,8);
				parmArray[3].Value = this.ItemNumber;
				parmArray[4] = new SqlParameter("@stocklocn", SqlDbType.SmallInt);
				parmArray[4].Value = this.StockLocation;
				parmArray[5] = new SqlParameter("@tccode", SqlDbType.VarChar,2);
				parmArray[5].Value = this.TCCode;
				parmArray[6] = new SqlParameter("@trantype", SqlDbType.VarChar,3);
				parmArray[6].Value = this.TranType;
				parmArray[7] = new SqlParameter("@trandate", SqlDbType.DateTime);
				parmArray[7].Value = this.TranDate;
				parmArray[8] = new SqlParameter("@quantity", SqlDbType.Float);
				parmArray[8].Value = this.Quantity;
				parmArray[9] = new SqlParameter("@price", SqlDbType.Float);
				parmArray[9].Value = this.Price;
				parmArray[10] = new SqlParameter("@taxamt", SqlDbType.Float);
				parmArray[10].Value = this.TaxAmt;
				parmArray[11] = new SqlParameter("@value", SqlDbType.Float);
				parmArray[11].Value = this.Value;
						
				this.RunSP(conn, trans, "DN_FactTransSaveSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}			
		}

		public int DeleteCancellation(SqlConnection conn, SqlTransaction trans)
		{
			try
			{
				parmArray = new SqlParameter[6];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar,12);
				parmArray[0].Value = this.AccountNumber;
				parmArray[1] = new SqlParameter("@itemno", SqlDbType.VarChar,8);
				parmArray[1].Value = this.ItemNumber;
				parmArray[2] = new SqlParameter("@stocklocn", SqlDbType.SmallInt);
				parmArray[2].Value = this.StockLocation;
				parmArray[3] = new SqlParameter("@tccode", SqlDbType.VarChar,2);
				parmArray[3].Value = this.TCCode;
				parmArray[4] = new SqlParameter("@trantype", SqlDbType.VarChar,3);
				parmArray[4].Value = this.TranType;
				parmArray[5] = new SqlParameter("@rowcount", SqlDbType.Int);
				parmArray[5].Value = 0;
				parmArray[5].Direction = ParameterDirection.Output;
						
				RunSP(conn, trans, "DN_FactTransDeleteSP", parmArray);

				if(parmArray[5].Value!=DBNull.Value)
					result = (int)parmArray[5].Value;				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}			
			return result;
		}


		public DFACTTrans()
		{
		}
	}
}
