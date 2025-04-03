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
	/// Summary description for DComponentLineItem.
	/// </summary>
	public class DComponentLineItem : DALObject
	{
		public DComponentLineItem()
		{
		}
		private string _acctno = "";
		public string AccountNo 
		{
			get{return _acctno;}
			set{_acctno = value;}
		}

		private string _kitno = "";
		public string KitNo 
		{
			get{return _kitno;}
			set{_kitno = value;}
		}

		private string _compno = "";
		public string ComponentNo 
		{
			get{return _compno;}
			set{_compno = value;}
		}

		private short _stocklocn = 0;
		public short StockLocation
		{
			get{return _stocklocn;}
			set{_stocklocn = value;}
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

		private double _ordval = 0;
		public double OrderValue
		{
			get{return _ordval;}
			set{_ordval = value;}
		}

		public void Save(SqlConnection conn, SqlTransaction trans)
		{
			try
			{
				parmArray = new SqlParameter[7];
				parmArray[0] = new SqlParameter("@AcctNo", SqlDbType.NVarChar,12);
				parmArray[0].Value = this.AccountNo;
				parmArray[1] = new SqlParameter("@KitNo", SqlDbType.NVarChar, 8);
				parmArray[1].Value = this.KitNo;
				parmArray[2] = new SqlParameter("@ComponentNo", SqlDbType.NVarChar, 8);
				parmArray[2].Value = this.ComponentNo;
				parmArray[3] = new SqlParameter("@StockLocn", SqlDbType.SmallInt);
				parmArray[3].Value = this.StockLocation;
				parmArray[4] = new SqlParameter("@Quantity", SqlDbType.Float);
				parmArray[4].Value = this.Quantity;
				parmArray[5] = new SqlParameter("@Price", SqlDbType.Float);
				parmArray[5].Value = this.Price;
				parmArray[6] = new SqlParameter("@OrdVal", SqlDbType.Float);
				parmArray[6].Value = this.OrderValue;

				this.RunSP(conn, trans, "DN_KitCLineItemSaveSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}
		public void SavePT(SqlConnection conn, SqlTransaction trans)
		{
			try
			{
				parmArray = new SqlParameter[7];
				parmArray[0] = new SqlParameter("@AcctNo", SqlDbType.NVarChar,12);
				parmArray[0].Value = this.AccountNo;
				parmArray[1] = new SqlParameter("@KitNo", SqlDbType.NVarChar, 8);
				parmArray[1].Value = this.KitNo;
				parmArray[2] = new SqlParameter("@ComponentNo", SqlDbType.NVarChar, 8);
				parmArray[2].Value = this.ComponentNo;
				parmArray[3] = new SqlParameter("@StockLocn", SqlDbType.SmallInt);
				parmArray[3].Value = this.StockLocation;
				parmArray[4] = new SqlParameter("@Quantity", SqlDbType.Float);
				parmArray[4].Value = this.Quantity;
				parmArray[5] = new SqlParameter("@Price", SqlDbType.Float);
				parmArray[5].Value = this.Price;
				parmArray[6] = new SqlParameter("@OrdVal", SqlDbType.Float);
				parmArray[6].Value = this.OrderValue;

				this.RunSP(conn, trans, "DN_KitCLineItemSavePTSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}
	}
}
