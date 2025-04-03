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
	/// Summary description for KitLineItem.
	/// </summary>
	public class DKitLineItem : DALObject
	{
		public DKitLineItem()
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

		private double _dsprice = 0;
		public double DiscountPrice
		{
			get{return _dsprice;}
			set{_dsprice = value;}
		}

		private double _dsordval = 0;
		public double DiscountOrderValue
		{
			get{return _dsordval;}
			set{_dsordval = value;}
		}

		public void Save(SqlConnection conn, SqlTransaction trans)
		{
			try
			{
				parmArray = new SqlParameter[6];
				parmArray[0] = new SqlParameter("@AcctNo", SqlDbType.NVarChar,12);
				parmArray[0].Value = this.AccountNo;
				parmArray[1] = new SqlParameter("@KitNo", SqlDbType.NVarChar, 8);
				parmArray[1].Value = this.KitNo;
				parmArray[2] = new SqlParameter("@StockLocn", SqlDbType.SmallInt);
				parmArray[2].Value = this.StockLocation;
				parmArray[3] = new SqlParameter("@Quantity", SqlDbType.Float);
				parmArray[3].Value = this.Quantity;
				parmArray[4] = new SqlParameter("@DSPrice", SqlDbType.Float);
				parmArray[4].Value = this.DiscountPrice;
				parmArray[5] = new SqlParameter("@DSOrdVal", SqlDbType.Float);
				parmArray[5].Value = this.DiscountOrderValue;

				this.RunSP(conn, trans, "DN_KitLineItemSaveSP", parmArray);
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
				parmArray = new SqlParameter[6];
				parmArray[0] = new SqlParameter("@AcctNo", SqlDbType.NVarChar,12);
				parmArray[0].Value = this.AccountNo;
				parmArray[1] = new SqlParameter("@KitNo", SqlDbType.NVarChar, 8);
				parmArray[1].Value = this.KitNo;
				parmArray[2] = new SqlParameter("@StockLocn", SqlDbType.SmallInt);
				parmArray[2].Value = this.StockLocation;
				parmArray[3] = new SqlParameter("@Quantity", SqlDbType.Float);
				parmArray[3].Value = this.Quantity;
				parmArray[4] = new SqlParameter("@DSPrice", SqlDbType.Float);
				parmArray[4].Value = this.DiscountPrice;
				parmArray[5] = new SqlParameter("@DSOrdVal", SqlDbType.Float);
				parmArray[5].Value = this.DiscountOrderValue;

				this.RunSP(conn, trans, "DN_KitLineItemSavePTSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}
	}
}
