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
	/// Summary description for DFactExport.
	/// </summary>
	public class DFactExport: DALObject
	{
		private int _runno = 0;
		public int RunNumber 
		{
			get{return _runno;}
			set{_runno = value;}
		}
		private short _fullRebateDays;
		public short FullRebateDays
		{
			get{return _fullRebateDays;}
			set{_fullRebateDays = value;}
		}
		private string _controlfile;
		public string ControlFile
			{
				get{return _controlfile;}
				set{_controlfile = value;}
			}

		private decimal _deliverytotal;
		public decimal DeliveryTotal
		{
			get{return _deliverytotal;}
			set{_deliverytotal = value;}
		}
		private decimal _financialtotal;
		public decimal FinancialTotal
		{
			get{return _financialtotal;}
			set{_financialtotal = value;}
		}

		private decimal _orderanddeliverytotal;
		public decimal OrderandDeliveryTotal
		{
			get{return _orderanddeliverytotal;}
			set{_orderanddeliverytotal = value;}
		}
		private DataTable _deliveryLineItems;
		public DataTable DeliveryLineItems
		{
			get{return _deliveryLineItems;}
		}
		private DataTable _financialexporttotals;
		public DataTable FinancialExportTotals
		{
			get{return _financialexporttotals;}
		}

		
		public DFactExport()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		public void DeliverNonStocks(SqlConnection conn, SqlTransaction trans)
		{
			try
			{
				parmArray = new SqlParameter[0];

				this.RunSP(conn,trans, "dn_FactExportDeliverNonStocks",parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

        public void BalanceServiceAccounts(SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                parmArray = new SqlParameter[0];

                this.RunSP(conn, trans, "BalanceServiceAccounts", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

		public void LoadOrdersandDeliveries(SqlConnection conn, SqlTransaction trans)
		{
			try
			{   int Return=0;
				_deliveryLineItems = new DataTable(TN.DeliveryLineItems);
				
				parmArray = new SqlParameter[4];
				
				parmArray[0] = new SqlParameter("@runno", SqlDbType.Int);
				parmArray[0].Value = this.RunNumber;
				parmArray[0].Direction = ParameterDirection.Output;
				parmArray[1] = new SqlParameter("@DeliveryTotal", SqlDbType.Money);
				parmArray[1].Value = this.DeliveryTotal;
				parmArray[1].Direction = ParameterDirection.Output;
				parmArray[2] = new SqlParameter("@OrderandDeliveryTotal", SqlDbType.Money);
				parmArray[2].Value = this.OrderandDeliveryTotal;
				parmArray[2].Direction = ParameterDirection.Output;
                parmArray[3] = new SqlParameter("@empeeno", SqlDbType.Int);
                parmArray[3].Value = this.User;
				
				Return = this.RunSP(conn, trans, "dn_DeliveriesandOrdersLoadforExport", parmArray, _deliveryLineItems);
				if (Return ==0)
				{
					if(!Convert.IsDBNull(parmArray[0].Value))
						this.RunNumber = (int)parmArray[0].Value;
					if(!Convert.IsDBNull(parmArray[1].Value))
						this.DeliveryTotal = (decimal)parmArray[1].Value;
					if(!Convert.IsDBNull(parmArray[2].Value))
						this.OrderandDeliveryTotal = (decimal)parmArray[2].Value;
				}
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void DeliveriesandOrdersRemoveafterExport(SqlConnection conn, SqlTransaction trans)
		{
			try
			{
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@runno", SqlDbType.Int);
				parmArray[0].Value = this._runno;
				
				this.RunSP(conn,trans,"dn_DeliveriesandOrdersRemoveafterExport", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

        public void EodUpdateSummaryTotals(SqlConnection conn, SqlTransaction trans,int Summaryrunno)
		{
			try
			{
								
				parmArray = new SqlParameter[1];
				
				parmArray[0] = new SqlParameter("@runno", SqlDbType.Int);
				parmArray[0].Value = Summaryrunno;
				
				this.RunSP(conn,trans,"dn_EodUpdateSummaryTotals", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}

		}
		public void LoadforInterfaceFinancial(SqlConnection conn, SqlTransaction trans)
		{
			try
			{
				decimal Return = 0;
				_financialexporttotals = new DataTable(TN.FinancialExportTotals);
				
				parmArray = new SqlParameter[2];
				
				parmArray[0] = new SqlParameter("@runno", SqlDbType.Int);
				parmArray[0].Value = this.RunNumber;
				parmArray[1] = new SqlParameter("@financial_total", SqlDbType.Money);
				parmArray[1].Value = this.FinancialTotal;
				parmArray[1].Direction = ParameterDirection.Output;
				
				
				Return =this.RunSP(conn,trans,"dn_FactInterfaceFinancial", parmArray,_financialexporttotals);
				if (Return ==0)
				{
						if(!Convert.IsDBNull(parmArray[1].Value))
							this.FinancialTotal = (decimal)parmArray[1].Value;
				}

			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}

		}

		public void StampFintransRunno(SqlConnection conn, SqlTransaction trans,int Summaryrunno)
		{
			try
			{
								
				parmArray = new SqlParameter[1];
				
				parmArray[0] = new SqlParameter("@runno", SqlDbType.Int);
				parmArray[0].Value = Summaryrunno;
				
				this.RunSP(conn,trans,"dn_StampFintransRunno", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}

		}

		public void GetControlFileDetails(string configuration)
		{
			try
			{
				int Return=0;				
				parmArray = new SqlParameter[2];
				
				parmArray[0] = new SqlParameter("@configurationname", SqlDbType.VarChar,32);
				parmArray[0].Value = configuration;
				parmArray[1] = new SqlParameter("@ControlFile", SqlDbType.VarChar,32);
				parmArray[1].Value = this.ControlFile;
				parmArray[1].Direction = ParameterDirection.Output;
				
				this.RunSP("dn_FACT2000controlfiledetails", parmArray);
				if (Return ==0)
				{
					if(!Convert.IsDBNull(parmArray[1].Value))
						this.ControlFile = parmArray[1].Value.ToString();
				}
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}

		}
	}
	}



