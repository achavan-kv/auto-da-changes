using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common.Constants.Enums;
using STL.Common.Constants.ColumnNames;
using STL.Common;
using STL.Common.Constants.TableNames;
using System.Diagnostics;

namespace STL.DAL
{
	/// <summary>
	/// Summary description for DLineItem.
	/// </summary>
	public class DLineItem : DALObject
	{
		private short _origbr = 0;
		public short OrigBr
		{
			get{return _origbr;}
			set{_origbr = value;}
		}
		private string _acctNo = "";
		public string AccountNumber
		{
			get{return _acctNo;}
			set{_acctNo = value;}
		}
        #region Is Amortization
        private bool _isAmortized = false;
        public bool IsAmortized
        {
            get { return _isAmortized; }
            set { _isAmortized = value; }
        }
        #endregion
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
        private int _itemID = 0;        //RI jec 28/04/11
        public int ItemID
        {
            get { return _itemID; }
            set { _itemID = value; }
        }
		private string _itemSuppText = "";
		public string ItemSuppText
		{
			get{return _itemSuppText;}
			set{_itemSuppText = value;}
		}
		private double _quantity = 0;
		public double Quantity
		{
			get{return _quantity;}
			set{_quantity = value;}
		}
		private decimal _value = 0;
		public decimal Value
		{
			get{return _value;}
			set{_value = value;}
		}
		private double _delqty = 0;
		public double DeliveredQuantity
		{
			get{return _delqty;}
			set{_delqty = value;}
		}
		private double _scheduled = 0;
		public double ScheduledQuantity
		{
			get{return _scheduled;}
			set{_scheduled = value;}
		}
		private short _stockLocn = 0;
		public short StockLocation
		{
			get{return _stockLocn;}
			set{_stockLocn = value;}
		}
		private decimal _price = 0;
		public decimal Price
		{
			get{return _price;}
			set{_price = value;}
		}
		private decimal _orderVal = 0;
		public decimal OrderValue
		{
			get{return _orderVal;}
			set{_orderVal = value;}
		}
		private DateTime _dateReqDel = DateTime.MinValue.AddYears(1899);
		public DateTime DateRequiredDelivery
		{
			get{return _dateReqDel;}
			set{_dateReqDel = value;}
		}
		private string _timeReqDel = "";
		public string TimeRequiredDelivery
		{
			get{return _timeReqDel;}
			set{_timeReqDel = value;}
		}
		private DateTime _datePlanDel = DateTime.MinValue.AddYears(1899);
		public DateTime DatePlannedDelivery
		{
			get{return _datePlanDel;}
			set{_datePlanDel = value;}
		}
		private short _delNoteBranch = 0;
		public short DeliveryNoteBranch
		{
			get{return _delNoteBranch;}
			set{_delNoteBranch = value;}
		}
		private string _qtyDiff = "";
		public string QuantityDiff
		{
			get{return _qtyDiff;}
			set{_qtyDiff = value;}
		}
		private string _itemType = "";
		public string ItemType
		{
			get{return _itemType;}
			set{_itemType = value;}
		}
		private short _hasString = 0;
		public short HasString 
		{
			get{return _hasString;}
			set{_hasString = value;}
		}
		private string _notes = "";
		public string Notes
		{
			get{return _notes;}
			set{_notes = value;}
		}
		private double _taxAmount = 0;
		public double TaxAmount
		{
			get{return _taxAmount;}
			set{_taxAmount = value;}
		}
		private string _parentItemNo = "";
		public string ParentItemNumber
		{
			get{return _parentItemNo;}
			set{_parentItemNo = value;}
		}
        private int _parentItemID = 0;        //RI jec 28/04/11
        public int ParentItemID
        {
            get { return _parentItemID; }
            set { _parentItemID = value; }
        }
        private bool _repoItem = false;        //RI jec 16/06/11
        public bool RepoItem
        {
            get { return _repoItem; }
            set { _repoItem = value; }
        }
		private short _parentStockLocn = 0;
		public short ParentStockLocation
		{
			get{return _parentStockLocn;}
			set{_parentStockLocn = value;}
		}
		private short _isKit = 0;
		public short IsKit
		{
			get{return _isKit;}
			set{_isKit = value;} 
		}	
	
		private DataTable _codes = null;
		public DataTable Codes 
		{
			get{return _codes;}
		}
		private DateTime _dateLastDel = DateTime.MinValue.AddYears(1899);
		public DateTime DateOfLastDelivery
		{
			get{return _dateLastDel;}
			set{_dateLastDel = value;}
		}

		private string _deliveryAddress = "H";
		public string DeliveryAddress
		{
			get{return _deliveryAddress;}
			set{_deliveryAddress = value;}
		}

		private string _deliveryArea = "";
		public string DeliveryArea
		{
			get{return _deliveryArea;}
			set{_deliveryArea = value;}
		}

		private string _deliveryProcess = "";
		public string DeliveryProcess
		{
			get{return _deliveryProcess;}
			set{_deliveryProcess = value;}
		}

		private DataTable _itemdetails;
		public DataTable ItemDetails
		{
			get	{return _itemdetails;}
		}


		private decimal _realDiscount = 0;
		public decimal realDiscount
		{
			get
			{return _realDiscount;}
		}

		private string _contractNo = "";
		public string ContractNo
		{
			get{return _contractNo;}
			set{_contractNo = value;}
		}

		private DateTime _expectedReturnDate = DateTime.MinValue.AddYears(1899);
		public DateTime ExpectedReturnDate
		{
			get{return _expectedReturnDate;}
			set{_expectedReturnDate = value;}
		}

		private string _returnItemNo = "";
		public string ReturnItemNumber
		{
			get{return _returnItemNo;}
			set{_returnItemNo = value;}
		}

        public int ReturnItemId { get; set; }

		private short _returnStockLocn = 0;
		public short ReturnStockLocn
		{
			get{return _returnStockLocn;}
			set{_returnStockLocn = value;}
		}

        private string _auditsource = "";
        public string AuditSource
        {
            get { return _auditsource; }
            set { _auditsource = value; }
        }

        private string _damaged = "";
		public string Damaged
		{
			get{return _damaged;}
			set{_damaged = value;}
		}

		private string _assembly = "";
		public string Assembly
		{
			get{return _assembly;}
			set{_assembly = value;}
		}

        private bool _spiffitem = false;
        public bool SPIFFItem
        {
            get { return _spiffitem; }
            set { _spiffitem = value; }
        }

        private bool _iscomponent= false;
        public bool IsComponent
        {
            get { return _iscomponent; }
            set { _iscomponent = value; }
        }

        //IP/JC - 02/03/10 - CR1072 - Malaysia 3PL 
        private string _vanNo;
        public string VanNo
        {
            get { return _vanNo; }
            set { _vanNo = value; }
        }

        private DateTime _dhlInterfaceDate;
        public DateTime DhlInterfaceDate
        {
            get { return _dhlInterfaceDate; }
            set { _dhlInterfaceDate = value; }
        }

        private DateTime _dhlPickingDate;
        public DateTime DhlPickingDate
        {
            get { return _dhlPickingDate; }
            set { _dhlPickingDate = value; }
        }

        private string _dhlDNNo;
        public string DhlDNNo
        {
            get { return _dhlDNNo; }
            set { _dhlDNNo = value; }
        }

        private double _taxrate;
        public double Taxrate
        {
            get { return _taxrate; }
            set { _taxrate = value; }
        }      

        //IP/JC - 02/03/10 - CR1072 - Malaysia 3PL 
        //private string _origQty;
        //public string OrigQty
        //{
        //    get { return _origQty; }
        //    set { _origQty = value; }
        //}

        //jec - 08/03/10 - Malaysia 3PL
        private string _shipQty;
        public string ShipQty
        {
            get { return _shipQty; }
            set { _shipQty = value; }
        }

        //IP - 04/06/10 - UAT(262) UAT5.2.1.0 Log
        private bool _itemRejected;
        public bool ItemRejected
        {
            get { return _itemRejected; }
            set { _itemRejected = value; }
        }

        //IP - 23/05/11 - CR1212 - RI - #3651
        private short? _salesBrnNo = 0;
        public short? SalesBrnNo
        {
            get { return _salesBrnNo; }
            set { _salesBrnNo = value; }
        }

        //IP - 06/06/12 - #10229 - Warehouse & Deliveries
        private string _express = string.Empty;
        public string Express
        {
            get { return _express; }
            set { _express = value; }
        }

        //#13716 - CR12949
        private int _lineItemId = 0;
        public int LineItemId
        {
            get{return _lineItemId;}
            set {_lineItemId = value;}
        }

        //#13716 - CR12949
        private bool _readyAssist = false;
        public bool ReadyAssist
        {
            get{return _readyAssist;}
            set{_readyAssist = value;}
        }

        private int _invoiceversion = 0;
        public int InvoiceVersion
        {
            get { return _invoiceversion; }
            set { _invoiceversion = value; }
        }
        private string _DeliveryorCollection;
        public string DeliveryorCollection
        {
            get { return _DeliveryorCollection; }
            set { _DeliveryorCollection = value; }
        }
        // Constructors
        public DLineItem()
		{
		
		}

		// Methods
		public void Save(SqlConnection conn, SqlTransaction trans)
		{
			try
			{
				if(this.DeliveryAddress.Length == 0)
					this.DeliveryAddress = "H";
				DeliveryAddress = DeliveryAddress.ToUpper();

				parmArray = new SqlParameter[35];
				parmArray[0] = new SqlParameter("@origbr", SqlDbType.SmallInt);
				parmArray[0].Value = this.OrigBr;
				parmArray[1] = new SqlParameter("@acctNo", SqlDbType.NVarChar, 12);
				parmArray[1].Value = this.AccountNumber;
				parmArray[2] = new SqlParameter("@agreementNo", SqlDbType.Int);
				parmArray[2].Value = this.AgreementNumber;
                //parmArray[3] = new SqlParameter("@itemNo", SqlDbType.NVarChar,8);
                //parmArray[3].Value = this.ItemNumber;
                parmArray[3] = new SqlParameter("@itemID", SqlDbType.Int);      //RI jec 28/04/11
                parmArray[3].Value = this.ItemID;
				parmArray[4] = new SqlParameter("@itemSuppText", SqlDbType.NVarChar, 76);
				parmArray[4].Value = this.ItemSuppText;
				parmArray[5] = new SqlParameter("@quantity", SqlDbType.Float);
				parmArray[5].Value = this.Quantity;
				//JJ - we don't maintain delqty anymore
				//parmArray[6] = new SqlParameter("@delQty", SqlDbType.Float);
				//parmArray[6].Value = this.DeliveredQuantity;
				parmArray[6] = new SqlParameter("@stockLocn", SqlDbType.SmallInt);
				parmArray[6].Value = this.StockLocation;
				parmArray[7] = new SqlParameter("@price", SqlDbType.Money);
				parmArray[7].Value = this.Price;
				parmArray[8] = new SqlParameter("@orderValue", SqlDbType.Money);
				parmArray[8].Value = this.OrderValue;
				parmArray[9] = new SqlParameter("@dateReqDel", SqlDbType.DateTime);
				parmArray[9].Value = this.DateRequiredDelivery;
				parmArray[10] = new SqlParameter("@timeReqDel", SqlDbType.NVarChar,12);
				parmArray[10].Value = this.TimeRequiredDelivery;
				parmArray[11] = new SqlParameter("@datePlanDel", SqlDbType.DateTime);
				parmArray[11].Value = this.DatePlannedDelivery;
				parmArray[12] = new SqlParameter("@delNoteBranch", SqlDbType.SmallInt);
				parmArray[12].Value = this.DeliveryNoteBranch;
				parmArray[13] = new SqlParameter("@qtyDiff", SqlDbType.NChar,1);
				parmArray[13].Value = this.QuantityDiff;
				parmArray[14] = new SqlParameter("@itemType", SqlDbType.NVarChar,1);
				parmArray[14].Value = this.ItemType;
				parmArray[15] = new SqlParameter("@hasString", SqlDbType.SmallInt);
				parmArray[15].Value = this.HasString;
				parmArray[16] = new SqlParameter("@notes", SqlDbType.NVarChar,200);
				parmArray[16].Value = this.Notes;
				parmArray[17] = new SqlParameter("@taxAmount", SqlDbType.Float);
				parmArray[17].Value = this.TaxAmount;
                //parmArray[18] = new SqlParameter("@parentItemNo", SqlDbType.NVarChar,8);
                //parmArray[18].Value = this.ParentItemNumber;
                parmArray[18] = new SqlParameter("@parentItemID", SqlDbType.Int);      //RI jec 28/04/11
                parmArray[18].Value = this.ParentItemID;
				parmArray[19] = new SqlParameter("@parentStockLocn", SqlDbType.SmallInt);
				parmArray[19].Value = this.ParentStockLocation;
				parmArray[20] = new SqlParameter("@isKit", SqlDbType.SmallInt);
				parmArray[20].Value = this.IsKit;
				parmArray[21] = new SqlParameter("@deliveryAddress", SqlDbType.NChar, 2);
				parmArray[21].Value = this.DeliveryAddress;
				parmArray[22] = new SqlParameter("@ordbuffno", SqlDbType.Int);
				parmArray[22].Value = this.BuffNo;
				parmArray[23] = new SqlParameter("@contractNo", SqlDbType.NVarChar, 10);
				parmArray[23].Value = this.ContractNo;
				parmArray[24] = new SqlParameter("@expectedreturndate", SqlDbType.DateTime);
				if(ExpectedReturnDate==DateTime.MinValue.AddYears(1899))
					parmArray[24].Value = DBNull.Value;
				else
					parmArray[24].Value = this.ExpectedReturnDate;
				parmArray[25] = new SqlParameter("@empeenochange", SqlDbType.Int);
				parmArray[25].Value = User;
				parmArray[26] = new SqlParameter("@countrycode", SqlDbType.NVarChar, 1);
				parmArray[26].Value = (string)Country[CountryParameterNames.CountryCode];
				parmArray[27] = new SqlParameter("@deliveryarea", SqlDbType.NVarChar, 8);
				parmArray[27].Value = this.DeliveryArea;
				parmArray[28] = new SqlParameter("@deliveryprocess", SqlDbType.NChar, 1);
				parmArray[28].Value = this.DeliveryProcess;
				parmArray[29] = new SqlParameter("@damaged", SqlDbType.NChar, 1);
				parmArray[29].Value = this.Damaged;
				parmArray[30] = new SqlParameter("@assembly", SqlDbType.NChar, 1);
				parmArray[30].Value = this.Assembly;
                parmArray[31] = new SqlParameter("@source", SqlDbType.NVarChar, 15);
                parmArray[31].Value = this.AuditSource;
                parmArray[32] = new SqlParameter("@taxrate", SqlDbType.Float);
                parmArray[32].Value = this.Taxrate;
                parmArray[33] = new SqlParameter("@salesBrnNo", SqlDbType.SmallInt);        //IP - 23/05/11 - CR1212 - RI - #3651
                parmArray[33].Value = this.SalesBrnNo??(object)DBNull.Value;
                parmArray[34] = new SqlParameter("@express", SqlDbType.NChar, 1);           //IP - 06/06/12 - #10229 - Warehouse & Deliveries
                parmArray[34].Value = this.Express;
              
                
				this.RunSP(conn, trans, "DN_LineItemUpdateSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public DataTable GetRootLineItemCodes(SqlConnection conn, SqlTransaction trans, 
			string accountNumber, int agreementNo, int invoiceversion)
		{
			try
			{
				_codes = new DataTable("Codes");
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNumber;
				parmArray[1] = new SqlParameter("@agreementNo", SqlDbType.Int);
				parmArray[1].Value = agreementNo;
                parmArray[2] = new SqlParameter("@invoiceversion", SqlDbType.Int);
                parmArray[2].Value = invoiceversion;
                if (conn!=null && trans!=null)
                    RunSP(conn, trans, "DN_LineItemGetRootCodesSP", parmArray, _codes);
				else
					RunSP("DN_LineItemGetRootCodesSP", parmArray, _codes);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return Codes;
		}

        //IP - 11/10/11 - #3921 - CR1232
        public DataTable GetCashLoanItem(SqlConnection conn, SqlTransaction trans,
            string accountNumber, int agreementNo)
        {
            try
            {
                _codes = new DataTable("Codes");
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar, 12);
                parmArray[0].Value = accountNumber;
                parmArray[1] = new SqlParameter("@agreementNo", SqlDbType.Int);
                parmArray[1].Value = agreementNo;
                if (conn != null && trans != null)
                    RunSP(conn, trans, "GetCashLoanItemSP", parmArray, _codes);
                else
                    RunSP("GetCashLoanItemSP", parmArray, _codes);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return Codes;
        }

		public DataTable GetChildLineItemCodes(SqlConnection conn, SqlTransaction trans, string accountNumber, int agreementNo, int parentItemId, short branch)
		{
			try
			{
				_codes = new DataTable("Codes");
				parmArray = new SqlParameter[4];
				parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNumber;
                parmArray[1] = new SqlParameter("@itemId", SqlDbType.Int);
                parmArray[1].Value = parentItemId;
				parmArray[2] = new SqlParameter("@location", SqlDbType.SmallInt);
				parmArray[2].Value = branch;
				parmArray[3] = new SqlParameter("@agreementNo", SqlDbType.Int);
				parmArray[3].Value = agreementNo;
				if(conn!=null && trans!=null)
					this.RunSP(conn, trans, "DN_LineItemGetChildCodesSP", parmArray, _codes);
				else
					this.RunSP("DN_LineItemGetChildCodesSP", parmArray, _codes);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return Codes;
		}

		public int CanAddWarranty(SqlConnection conn, SqlTransaction trans)
		{
			int result = 0;
			try
			{
				parmArray = new SqlParameter[4];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = this.AccountNumber;
				parmArray[1] = new SqlParameter("@itemId", SqlDbType.Int);
				parmArray[1].Value = this.ItemID;
				parmArray[2] = new SqlParameter("@stocklocn", SqlDbType.SmallInt);
				parmArray[2].Value = this.StockLocation;
				parmArray[3] = new SqlParameter("@price", SqlDbType.Money);
				parmArray[3].Value = this.Price;

				if(conn!=null && trans !=null)
					result = RunSP(conn, trans, "DN_WarrantyCanAddSP", parmArray);
				else
					result = this.RunSP("DN_WarrantyCanAddSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

        //IP - 19/08/08 - (69962) - Procedure that will determine whether a warranty can 
        //be added for an item that is being added to the sale. Previously 
        //a warranty was being attached to a non-warrantable item when adding a 
        //new item after an exchange.
        public int CanAddWarrantyOnNewItem(SqlConnection conn, SqlTransaction trans)
        {
            int result = 0;
            try
            {
                parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@itemId", SqlDbType.Int);
                parmArray[0].Value = this.ItemID;
                parmArray[1] = new SqlParameter("@stocklocn", SqlDbType.SmallInt);
                parmArray[1].Value = this.StockLocation;
                parmArray[2] = new SqlParameter("@price", SqlDbType.Money);
                parmArray[2].Value = this.Price;

                if (conn != null && trans != null)
                    result = RunSP(conn, trans, "DN_WarrantyCanAddOnNewItemSP", parmArray);
                else
                    result = this.RunSP("DN_WarrantyCanAddOnNewItemSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;

        }
		public void GetItemDetails(SqlConnection conn, SqlTransaction trans)
		{
			try
			{
				parmArray = new SqlParameter[45];
				parmArray[0] = new SqlParameter("@origbr", SqlDbType.SmallInt);
				parmArray[0].Value = this.OrigBr;
				parmArray[0].Direction = ParameterDirection.Output;
				parmArray[1] = new SqlParameter("@acctNo", SqlDbType.NVarChar, 12);
				parmArray[1].Value = this.AccountNumber;
				parmArray[2] = new SqlParameter("@agreementNo", SqlDbType.Int);
				parmArray[2].Value = this.AgreementNumber;
				parmArray[3] = new SqlParameter("@itemId", SqlDbType.Int);
				parmArray[3].Value = this.ItemID;
				parmArray[4] = new SqlParameter("@itemSuppText", SqlDbType.NVarChar, 76);
				parmArray[4].Value = this.ItemSuppText;
				parmArray[4].Direction = ParameterDirection.Output;
				parmArray[5] = new SqlParameter("@quantity", SqlDbType.Float);
				parmArray[5].Value = this.Quantity;
				parmArray[5].Direction = ParameterDirection.Output;
				parmArray[6] = new SqlParameter("@delQty", SqlDbType.Float);
				parmArray[6].Value = this.DeliveredQuantity;
				parmArray[6].Direction = ParameterDirection.Output;
				parmArray[7] = new SqlParameter("@stockLocn", SqlDbType.SmallInt);
				parmArray[7].Value = this.StockLocation;
				parmArray[8] = new SqlParameter("@price", SqlDbType.Money);
				parmArray[8].Value = this.Price;
				parmArray[8].Direction = ParameterDirection.Output;
				parmArray[9] = new SqlParameter("@orderValue", SqlDbType.Money);
				parmArray[9].Value = this.OrderValue;
				parmArray[9].Direction = ParameterDirection.Output;
				parmArray[10] = new SqlParameter("@dateReqDel", SqlDbType.DateTime);
				parmArray[10].Value = this.DateRequiredDelivery;
				parmArray[10].Direction = ParameterDirection.Output;
				parmArray[11] = new SqlParameter("@timeReqDel", SqlDbType.NVarChar,12);
				parmArray[11].Value = this.TimeRequiredDelivery;
				parmArray[11].Direction = ParameterDirection.Output;
				parmArray[12] = new SqlParameter("@datePlanDel", SqlDbType.DateTime);
				parmArray[12].Value = this.DatePlannedDelivery;
				parmArray[12].Direction = ParameterDirection.Output;
				parmArray[13] = new SqlParameter("@delNoteBranch", SqlDbType.SmallInt);
				parmArray[13].Value = this.DeliveryNoteBranch;
				parmArray[13].Direction = ParameterDirection.Output;
				parmArray[14] = new SqlParameter("@qtyDiff", SqlDbType.NChar,1);
				parmArray[14].Value = this.QuantityDiff;
				parmArray[14].Direction = ParameterDirection.Output;
				parmArray[15] = new SqlParameter("@itemType", SqlDbType.NVarChar,1);
				parmArray[15].Value = this.ItemType;
				parmArray[15].Direction = ParameterDirection.Output;
				parmArray[16] = new SqlParameter("@hasString", SqlDbType.SmallInt);
				parmArray[16].Value = this.HasString;
				parmArray[16].Direction = ParameterDirection.Output;
				parmArray[17] = new SqlParameter("@notes", SqlDbType.NVarChar,200);
				parmArray[17].Value = this.Notes;
				parmArray[17].Direction = ParameterDirection.Output;
				parmArray[18] = new SqlParameter("@taxAmount", SqlDbType.Float);
				parmArray[18].Value = this.TaxAmount;
				parmArray[18].Direction = ParameterDirection.Output;
				parmArray[19] = new SqlParameter("@parentItemId", SqlDbType.Int);
				parmArray[19].Value = this.ParentItemID;
                // uat363 rdb will now filter on ParentItemNumber use as input value
				//parmArray[19].Direction = ParameterDirection.Output;
				parmArray[20] = new SqlParameter("@parentStockLocn", SqlDbType.SmallInt);
				parmArray[20].Value = this.ParentStockLocation;
				parmArray[20].Direction = ParameterDirection.Output;
				parmArray[21] = new SqlParameter("@isKit", SqlDbType.SmallInt);
				parmArray[21].Value = this.IsKit;
				parmArray[21].Direction = ParameterDirection.Output;
				parmArray[22] = new SqlParameter("@lastDelivery", SqlDbType.DateTime);
				parmArray[22].Value = this.DateOfLastDelivery;
				parmArray[22].Direction = ParameterDirection.Output;
				parmArray[23] = new SqlParameter("@deliveryAddress", SqlDbType.NChar, 2);
				parmArray[23].Value = this.DeliveryAddress;
				parmArray[23].Direction = ParameterDirection.Output;
				parmArray[24] = new SqlParameter("@scheduled", SqlDbType.Float);
				parmArray[24].Value = this.ScheduledQuantity;
				parmArray[24].Direction = ParameterDirection.Output;
				parmArray[25] = new SqlParameter("@contractNo", SqlDbType.NVarChar, 10);
				parmArray[25].Value = this.ContractNo;
				parmArray[26] = new SqlParameter("@expectedreturndate", SqlDbType.DateTime);
				parmArray[26].Value = this.ExpectedReturnDate;
				parmArray[26].Direction = ParameterDirection.Output;
				parmArray[27] = new SqlParameter("@deliveryarea", SqlDbType.NVarChar,8);
				parmArray[27].Value = this.DeliveryArea;
				parmArray[27].Direction = ParameterDirection.Output;
				parmArray[28] = new SqlParameter("@deliveryprocess", SqlDbType.NChar,1);
				parmArray[28].Value = this.DeliveryProcess;
				parmArray[28].Direction = ParameterDirection.Output;
				parmArray[29] = new SqlParameter("@damaged", SqlDbType.NChar,1);
				parmArray[29].Value = this.Damaged;
				parmArray[29].Direction = ParameterDirection.Output;
				parmArray[30] = new SqlParameter("@assembly", SqlDbType.NChar,1);
				parmArray[30].Value = this.Assembly;
				parmArray[30].Direction = ParameterDirection.Output;
                parmArray[31] = new SqlParameter("@spiff", SqlDbType.Bit);
                parmArray[31].Value = this.SPIFFItem;
                parmArray[31].Direction = ParameterDirection.Output;
                

                //IP/JC - 02/03/10 - CR1072 - Malaysia 3PL 
                parmArray[32] = new SqlParameter("@VanNo", SqlDbType.NVarChar, 8);
                parmArray[32].Direction = ParameterDirection.Output;
                parmArray[33] = new SqlParameter("@DhlInterfaceDate", SqlDbType.DateTime);
                parmArray[33].Direction = ParameterDirection.Output;
                parmArray[34] = new SqlParameter("@DhlPickingDate", SqlDbType.DateTime);
                parmArray[34].Direction = ParameterDirection.Output;
                parmArray[35] = new SqlParameter("@DHLDNNo", SqlDbType.NVarChar, 10);
                parmArray[35].Direction = ParameterDirection.Output;
                
                //parmArray[36] = new SqlParameter("@OrigQty", SqlDbType.Int);
                //parmArray[36].Direction = ParameterDirection.Output;
                parmArray[36] = new SqlParameter("@ShipQty", SqlDbType.Int);
                parmArray[36].Direction = ParameterDirection.Output;

                parmArray[37] = new SqlParameter("@ItemRejected", SqlDbType.Bit);       //IP - 04/06/10 - UAT(262) UAT5.2.1.0 Log
                parmArray[37].Direction = ParameterDirection.Output;

                parmArray[38] = new SqlParameter("@IsComponent", SqlDbType.Bit);
                parmArray[38].Value = this.IsComponent;
                parmArray[38].Direction = ParameterDirection.Output;

                parmArray[39] = new SqlParameter("@itemno", SqlDbType.VarChar, 18);
                parmArray[39].Value = this.ItemNumber;
                parmArray[39].Direction = ParameterDirection.Output;

                parmArray[40] = new SqlParameter("@parentItemNo", SqlDbType.VarChar, 18);
                parmArray[40].Value = this.ParentItemNumber;
                parmArray[40].Direction = ParameterDirection.Output;

                parmArray[41] = new SqlParameter("@salesBrnNo", SqlDbType.SmallInt);            //IP - 23/05/11 - CR1212 - RI - #3651
                parmArray[41].Value = this.SalesBrnNo;
                parmArray[41].Direction = ParameterDirection.Output;

                parmArray[42] = new SqlParameter("@repoItem", SqlDbType.Bit);            //jec 16/06/11
                parmArray[42].Direction = ParameterDirection.Output;

                parmArray[43] = new SqlParameter("@express", SqlDbType.Char,1);            //IP - 07/06/12 - #10229 - Warehouse & Deliveries
                parmArray[43].Direction = ParameterDirection.Output;

                parmArray[44] = new SqlParameter("@lineItemId", SqlDbType.Int);            //#13716 - CR12949
                parmArray[44].Direction = ParameterDirection.Output;

				if(conn!=null && trans !=null)
					RunSP(conn, trans, "DN_LineItemGetDetailsSP", parmArray);
				else
					RunSP("DN_LineItemGetDetailsSP", parmArray);

				if(!Convert.IsDBNull(parmArray[0].Value))
					this.OrigBr = (short)parmArray[0].Value;
				if(!Convert.IsDBNull(parmArray[2].Value))
					this.AgreementNumber = (int)parmArray[2].Value;
				if(!Convert.IsDBNull(parmArray[4].Value))
					this.ItemSuppText = (string)parmArray[4].Value;
				if(!Convert.IsDBNull(parmArray[5].Value))
					this.Quantity = (double)parmArray[5].Value;
				if(!Convert.IsDBNull(parmArray[6].Value))
					this.DeliveredQuantity = (double)parmArray[6].Value;
				if(!Convert.IsDBNull(parmArray[8].Value))
					this.Price = (decimal)parmArray[8].Value;
				if(!Convert.IsDBNull(parmArray[9].Value))
					this.OrderValue = (decimal)parmArray[9].Value;
				if(!Convert.IsDBNull(parmArray[10].Value))
					this.DateRequiredDelivery = (DateTime)parmArray[10].Value;
				if(!Convert.IsDBNull(parmArray[11].Value))
					this.TimeRequiredDelivery = (string)parmArray[11].Value;
				if(!Convert.IsDBNull(parmArray[12].Value))
					this.DatePlannedDelivery = (DateTime)parmArray[12].Value;
				if(!Convert.IsDBNull(parmArray[13].Value))
					this.DeliveryNoteBranch = (short)parmArray[13].Value;
				if(!Convert.IsDBNull(parmArray[14].Value))
					this.QuantityDiff = (string)parmArray[14].Value;
				if(!Convert.IsDBNull(parmArray[15].Value))
					this.ItemType = (string)parmArray[15].Value;
				if(!Convert.IsDBNull(parmArray[16].Value))
					this.HasString = (short)parmArray[16].Value;
				if(!Convert.IsDBNull(parmArray[17].Value))
					this.Notes = (string)parmArray[17].Value;
				if(!Convert.IsDBNull(parmArray[18].Value))
					this.TaxAmount = (double)parmArray[18].Value;
				if(!Convert.IsDBNull(parmArray[20].Value))
					this.ParentStockLocation = (short)parmArray[20].Value;
				if(!Convert.IsDBNull(parmArray[21].Value))
					this.IsKit = (short)parmArray[21].Value;
				if(!Convert.IsDBNull(parmArray[22].Value))
					this.DateOfLastDelivery = (DateTime)parmArray[22].Value;
				if(!Convert.IsDBNull(parmArray[23].Value))
					this.DeliveryAddress = (string)parmArray[23].Value;
				if(!Convert.IsDBNull(parmArray[24].Value))
					this.ScheduledQuantity = (double)parmArray[24].Value;
				if(!Convert.IsDBNull(parmArray[26].Value))
					this.ExpectedReturnDate = (DateTime)parmArray[26].Value;
				if(!Convert.IsDBNull(parmArray[27].Value))
					this.DeliveryArea = (string)parmArray[27].Value;
				if(!Convert.IsDBNull(parmArray[28].Value))
					this.DeliveryProcess = (string)parmArray[28].Value;
				if(!Convert.IsDBNull(parmArray[29].Value))
					this.Damaged = (string)parmArray[29].Value;
				if(!Convert.IsDBNull(parmArray[30].Value))
					this.Assembly = (string)parmArray[30].Value;
                if (!Convert.IsDBNull(parmArray[31].Value))
                    this.SPIFFItem = (bool)parmArray[31].Value;
                //IP/JC - 02/03/10 - CR1072 - Malaysia 3PL 
                if (parmArray[32].Value != DBNull.Value)
                    this.VanNo = parmArray[32].Value.ToString();
                if (parmArray[33].Value != DBNull.Value)
                    this.DhlInterfaceDate = Convert.ToDateTime(parmArray[33].Value);
                if (parmArray[34].Value != DBNull.Value)
                    this.DhlPickingDate = Convert.ToDateTime(parmArray[34].Value);
                if (parmArray[35].Value != DBNull.Value)
                    this.DhlDNNo = parmArray[35].Value.ToString();
                
                ////IP/JC - 02/03/10 - CR1072 - Malaysia 3PL 
                //if (parmArray[36].Value != DBNull.Value)
                //    this.OrigQty= parmArray[36].Value.ToString();
                if (parmArray[36].Value != DBNull.Value)        //JeC - 08/03/10 - Malaysia 3PL
                    this.ShipQty = parmArray[36].Value.ToString();

                if (parmArray[37].Value != DBNull.Value)
                    this.ItemRejected = Convert.ToBoolean(parmArray[37].Value);     //IP - 04/06/10 - UAT(262) UAT5.2.1.0 Log
                if (parmArray[38].Value != DBNull.Value)
                    this.IsComponent= Convert.ToBoolean(parmArray[38].Value);     //AA - 07/07/10 - UAT(267) UAT5.2.1.0 Log

                if (parmArray[39].Value != DBNull.Value)
                    this.ItemNumber = Convert.ToString(parmArray[39].Value) ?? "";
                if (parmArray[40].Value != DBNull.Value)
                    this.ParentItemNumber = Convert.ToString(parmArray[40].Value) ?? "";

                //IP - 23/05/11 - CR1212 - RI - #3651
                this.SalesBrnNo = parmArray[41].Value != DBNull.Value ? Convert.ToInt16(parmArray[41].Value) :(Int16?)null;

                if (parmArray[42].Value != DBNull.Value)                //jec 16/06/11
                    this.RepoItem = Convert.ToBoolean(parmArray[42].Value);

                //IP - 07/06/12 - #102229 - Warehouse & Deliveries
                if (!Convert.IsDBNull(parmArray[43].Value))     
                    this.Express = (string)parmArray[43].Value;

                //#13716 - CR12949
                if (!Convert.IsDBNull(parmArray[44].Value))
                    this.LineItemId = (Int32)parmArray[44].Value;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		

		public void GetItemQuantity(SqlConnection conn, SqlTransaction trans, bool current)
		{
			try
			{
				parmArray = new SqlParameter[7];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
				parmArray[0].Value = this.AccountNumber;
				parmArray[1] = new SqlParameter("@itemID", SqlDbType.Int);           //IP - 24/05/11 - CR1212 - RI
				parmArray[1].Value = this.ItemID;
				parmArray[2] = new SqlParameter("@stocklocn", SqlDbType.SmallInt);
				parmArray[2].Value = this.StockLocation;
				parmArray[3] = new SqlParameter("@contractno", SqlDbType.NVarChar,10);
				parmArray[3].Value =this.ContractNo;
				parmArray[4] = new SqlParameter("@agreementno", SqlDbType.Int);
				parmArray[4].Value =this.AgreementNumber;
				parmArray[5] = new SqlParameter("@quantity", SqlDbType.Float);
				parmArray[5].Value =0;
				parmArray[5].Direction = ParameterDirection.Output;
                parmArray[6] = new SqlParameter("@ParentItemID", SqlDbType.Int);        // RI 
                parmArray[6].Value = this.ParentItemID;
		

				if(current)
					this.RunSP(conn, trans, "DN_LineItemGetCurrentQtySP", parmArray);
				else
					this.RunSP(conn, trans, "DN_LineItemGetOldQtySP", parmArray);

				if(!Convert.IsDBNull(parmArray[5].Value))
					this.Quantity = (double)parmArray[5].Value;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void GetItemValue(SqlConnection conn, SqlTransaction trans, bool current)
		{
			try
			{
				parmArray = new SqlParameter[7];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
				parmArray[0].Value = this.AccountNumber;
                //parmArray[1] = new SqlParameter("@itemno", SqlDbType.NVarChar,8);
                //parmArray[1].Value = this.ItemNumber;
                parmArray[1] = new SqlParameter("@itemid", SqlDbType.Int);          // RI
                parmArray[1].Value = this.ItemID;
				parmArray[2] = new SqlParameter("@stocklocn", SqlDbType.SmallInt);
				parmArray[2].Value = this.StockLocation;
				parmArray[3] = new SqlParameter("@contractNo", SqlDbType.NVarChar,10);
				parmArray[3].Value =this.ContractNo;
				parmArray[4] = new SqlParameter("@agreementno", SqlDbType.Int);
				parmArray[4].Value =this.AgreementNumber;
				parmArray[5] = new SqlParameter("@value", SqlDbType.Money);
				parmArray[5].Value =0;
				parmArray[5].Direction = ParameterDirection.Output;
                parmArray[6] = new SqlParameter("@parentitemid", SqlDbType.Int);
                parmArray[6].Value = ParentItemID;

				if(current)
					this.RunSP(conn, trans, "DN_LineItemGetCurrentValueSP", parmArray);
				else
					this.RunSP(conn, trans, "DN_LineItemGetOldValueSP", parmArray);

				if(!Convert.IsDBNull(parmArray[5].Value))
					this.Value = (decimal)parmArray[5].Value;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void DeleteLineItem(SqlConnection conn, SqlTransaction trans, 
			string accountNo, 
			int agreementNo,
            int itemID,                 //IP/NM - 18/05/11 -CR1212 - #3627 
			short location)
		{
			try
			{
				parmArray = new SqlParameter[4];
				parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar, 12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@agreementNo", SqlDbType.Int);
				parmArray[1].Value = agreementNo;
				parmArray[2] = new SqlParameter("@itemID", SqlDbType.Int);
                parmArray[2].Value = itemID;                                        //IP/NM - 18/05/11 -CR1212 - #3627 
				parmArray[3] = new SqlParameter("@location", SqlDbType.SmallInt);
				parmArray[3].Value = location;

				this.RunSP(conn, trans, "DN_LineItemDeleteSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void UpdateItemValue(SqlConnection conn, SqlTransaction trans, 
									string accountNo, int agreementNo,
									int itemID, 
									short location,
									decimal newValue)       //IP - 17/05/11 - CR1212 - #3627 - Changed to use itemID rather than itemNo
		{
			try
			{
				parmArray = new SqlParameter[5];
				parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar, 12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@itemID", SqlDbType.Int);              //IP - 17/05/11 - CR1212 - #3627
				parmArray[1].Value = itemID;
				parmArray[2] = new SqlParameter("@location", SqlDbType.SmallInt);
				parmArray[2].Value = location;
				parmArray[3] = new SqlParameter("@newvalue", SqlDbType.Money);
				parmArray[3].Value = newValue;
				parmArray[4] = new SqlParameter("@agreementno", SqlDbType.Int);
				parmArray[4].Value = agreementNo;

				this.RunSP(conn, trans, "DN_LineItemUpdateValueSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

        public void UpdateDeliveryArea(SqlConnection conn, SqlTransaction trans, 
            string accountNo, int agreementNo,
            //string itemNo, 
            int itemID,                                                                                 //IP - 08/06/11 - CR1212 - RI
            short location,
            string contractNo, 
            string deliveryArea,
            string deliveryProcess)
        {
            try
            {
                parmArray = new SqlParameter[7];
                parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar, 12);
                parmArray[0].Value = accountNo;
                //parmArray[1] = new SqlParameter("@itemNo", SqlDbType.NVarChar, 8);
                //parmArray[1].Value = itemNo;
                parmArray[1] = new SqlParameter("@itemID", SqlDbType.Int);                              //IP - 08/06/11 - CR1212 - RI
                parmArray[1].Value = itemID;
                parmArray[2] = new SqlParameter("@location", SqlDbType.SmallInt);
                parmArray[2].Value = location;
                parmArray[3] = new SqlParameter("@deliveryArea", SqlDbType.NVarChar, 16);
                parmArray[3].Value = deliveryArea;
                parmArray[4] = new SqlParameter("@deliveryProcess", SqlDbType.NVarChar, 16);
                parmArray[4].Value = deliveryProcess;
                parmArray[5] = new SqlParameter("@agreementno", SqlDbType.Int);
                parmArray[5].Value = agreementNo;
                parmArray[6] = new SqlParameter("@contractno", SqlDbType.NVarChar,10);
                parmArray[6].Value = contractNo;

                this.RunSP(conn, trans, "DN_LineItemUpdateDeliveryAreaSP", parmArray);
            }
            catch(SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }



		public void UpdateItemQuantity(SqlConnection conn, SqlTransaction trans, 
									string accountNo, int agreementNo,
									int itemID, 
									short location,
									string contractNo, 
									decimal newValue, int parentItemID)     //IP - 17/05/11 - CR1212 - #3627 - Changed to use itemID,parentItemID rather than itemNo and parentItemNo
		{
			try
			{
				parmArray = new SqlParameter[9];
				parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar, 12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@itemID", SqlDbType.Int);          //IP - 17/05/11 - CR1212 - #3627
				parmArray[1].Value = itemID;
				parmArray[2] = new SqlParameter("@location", SqlDbType.SmallInt);
				parmArray[2].Value = location;
				parmArray[3] = new SqlParameter("@newQty", SqlDbType.Float);
				parmArray[3].Value = newValue;
				parmArray[4] = new SqlParameter("@agreementno", SqlDbType.Int);
				parmArray[4].Value = agreementNo;
				parmArray[5] = new SqlParameter("@contractno", SqlDbType.NVarChar,10);
				parmArray[5].Value = contractNo;
                parmArray[6] = new SqlParameter("@source", SqlDbType.NVarChar, 15);
                parmArray[6].Value = AuditSource;
                //uat363 rdb add parentItemNo
                parmArray[7] = new SqlParameter("@parentItemID",SqlDbType.Int);     //IP - 17/05/11 - CR1212 - #3627
                parmArray[7].Value = parentItemID;
                parmArray[8] = new SqlParameter("@user", SqlDbType.Int);
                parmArray[8].Value = User;

				this.RunSP(conn, trans, "DN_LineItemUpdateQuantitySP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void DeleteAllLineItems(SqlConnection conn, SqlTransaction trans, 
			string accountNo, int agreementNo)
		{
			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar, 12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@agreementNo", SqlDbType.Int);
				parmArray[1].Value = agreementNo;

				this.RunSP(conn, trans, "DN_LineItemDeleteAllSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void DropTempLineItems(SqlConnection conn, SqlTransaction trans,
										string accountNo, int agreementNo )
		{
			parmArray = new SqlParameter[2];
			parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar, 12);
			parmArray[0].Value = accountNo;	
			parmArray[1] = new SqlParameter("@agreementno", SqlDbType.Int);
			parmArray[1].Value = agreementNo;	
			try
			{
				this.RunSP(conn, trans, "DN_LineItemDropTempSP",parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		/*
		public void SavePT(SqlConnection conn, SqlTransaction trans)
		{
			try
			{
				parmArray = new SqlParameter[23];
				parmArray[0] = new SqlParameter("@origbr", SqlDbType.SmallInt);
				parmArray[0].Value = this.OrigBr;
				parmArray[1] = new SqlParameter("@acctNo", SqlDbType.NVarChar, 12);
				parmArray[1].Value = this.AccountNumber;
				parmArray[2] = new SqlParameter("@agreementNo", SqlDbType.Int);
				parmArray[2].Value = this.AgreementNumber;
				parmArray[3] = new SqlParameter("@itemNo", SqlDbType.NVarChar,8);
				parmArray[3].Value = this.ItemNumber;
				parmArray[4] = new SqlParameter("@itemSuppText", SqlDbType.NVarChar, 76);
				parmArray[4].Value = this.ItemSuppText;
				parmArray[5] = new SqlParameter("@quantity", SqlDbType.Float);
				parmArray[5].Value = this.Quantity;
				parmArray[6] = new SqlParameter("@delQty", SqlDbType.Float);
				parmArray[6].Value = this.DeliveredQuantity;
				parmArray[7] = new SqlParameter("@stockLocn", SqlDbType.SmallInt);
				parmArray[7].Value = this.StockLocation;
				parmArray[8] = new SqlParameter("@price", SqlDbType.Money);
				parmArray[8].Value = this.Price;
				parmArray[9] = new SqlParameter("@orderValue", SqlDbType.Money);
				parmArray[9].Value = this.OrderValue;
				parmArray[10] = new SqlParameter("@dateReqDel", SqlDbType.DateTime);
				parmArray[10].Value = this.DateRequiredDelivery;
				parmArray[11] = new SqlParameter("@timeReqDel", SqlDbType.NVarChar,12);
				parmArray[11].Value = this.TimeRequiredDelivery;
				parmArray[12] = new SqlParameter("@datePlanDel", SqlDbType.DateTime);
				parmArray[12].Value = this.DatePlannedDelivery;
				parmArray[13] = new SqlParameter("@delNoteBranch", SqlDbType.SmallInt);
				parmArray[13].Value = this.DeliveryNoteBranch;
				parmArray[14] = new SqlParameter("@qtyDiff", SqlDbType.NChar,1);
				parmArray[14].Value = this.QuantityDiff;
				parmArray[15] = new SqlParameter("@itemType", SqlDbType.NVarChar,1);
				parmArray[15].Value = this.ItemType;
				parmArray[16] = new SqlParameter("@hasString", SqlDbType.SmallInt);
				parmArray[16].Value = this.HasString;
				parmArray[17] = new SqlParameter("@notes", SqlDbType.NVarChar,200);
				parmArray[17].Value = this.Notes;
				parmArray[18] = new SqlParameter("@taxAmount", SqlDbType.Float);
				parmArray[18].Value = this.TaxAmount;
				parmArray[19] = new SqlParameter("@parentItemNo", SqlDbType.NVarChar,8);
				parmArray[19].Value = this.ParentItemNumber;
				parmArray[20] = new SqlParameter("@parentStockLocn", SqlDbType.SmallInt);
				parmArray[20].Value = this.ParentStockLocation;
				parmArray[21] = new SqlParameter("@isKit", SqlDbType.SmallInt);
				parmArray[21].Value = this.IsKit;
				parmArray[22] = new SqlParameter("@deliveryAddress", SqlDbType.NChar, 2);
				parmArray[22].Value = this.DeliveryAddress;				

				this.RunSP(conn, trans, "DN_LineItemPTUpdateSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}
		*/

        public void GetSingleItem(SqlConnection conn, SqlTransaction trans, int locn, int itemID,                //IP - 17/05/11 - CR1212 - #3627 - Changed to use itemID and parentItemID rather than itemNo and parentItemNumber
            string acctno, int agreementNo, string contractNo, int parentItemID)
		{
			_itemdetails = new DataTable();
			try
			{
				parmArray = new SqlParameter[6];
				parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar,12);
				parmArray[0].Value = acctno;
				parmArray[1] = new SqlParameter("@itemID", SqlDbType.Int);
				parmArray[1].Value = itemID;                                                                    //IP - 17/05/11 - CR1212 - #3627
				parmArray[2] = new SqlParameter("@locn", SqlDbType.Int);
				parmArray[2].Value = locn;
				parmArray[3] = new SqlParameter("@agrmtno", SqlDbType.Int);
				parmArray[3].Value = agreementNo;
				parmArray[4] = new SqlParameter("@contractno", SqlDbType.NVarChar,10);
				parmArray[4].Value = contractNo;
                parmArray[5] = new SqlParameter("@parentItemID", SqlDbType.Int);                                //IP - 17/05/11 - CR1212 - #3627
                parmArray[5].Value = parentItemID;

				if(conn!=null && trans !=null)
					this.RunSP(conn, trans, "DN_ItemGetSP", parmArray, _itemdetails);
				else
					this.RunSP("DN_ItemGetSP", parmArray, _itemdetails);

				// Sometimes expect the properties to be populated
				foreach(DataRow row in _itemdetails.Rows)
				{				
					this._acctNo				= (string)row[CN.acctno];
					this._agreementNo			= (int)row[CN.AgrmtNo];
                    this.ItemNumber				= (string)row[CN.ItemNo];
                    this.ItemID                 =  Convert.ToInt32(row[CN.ItemID]);                             //IP - 17/05/11 - CR1212 - #3627
					this.Quantity				= (double)row[CN.Quantity];
					this.DeliveredQuantity		= (double)row[CN.DelQty];
					this.StockLocation			= (short)row[CN.StockLocn];
					this.ItemSuppText			= (string)row[CN.ItemSuppText];
					this.Price					= (decimal)row[CN.Price];
					this.TaxAmount				= (double)row[CN.TaxAmt];
					this.OrderValue				= (decimal)row[CN.OrdVal];
					this.DateRequiredDelivery	= (DateTime)row[CN.DateReqDel];
					this.TimeRequiredDelivery	= (string)row[CN.TimeReqDel];
					this.DatePlannedDelivery	= (DateTime)row[CN.DatePlanDel];
					this.ItemType				= (string)row[CN.ItemType];
					this.QuantityDiff			= (string)row[CN.Qtydiff];
					this.ExpectedReturnDate		= (DateTime)row[CN.ExpectedReturnDate];
					this.ContractNo				= (string)row[CN.ContractNo];
					this.DeliveryArea			= (string)row[CN.DeliveryArea];
					this.DeliveryProcess		= (string)row[CN.DeliveryProcess];
                    this.ParentItemNumber       = row["ParentItemNo"].ToString();
                    this.ParentItemID           = Convert.ToInt32(row[CN.ParentItemId]);                        //IP - 17/05/11 - CR1212 - #3627
                    this.ParentStockLocation    = (short)row[CN.ParentLocation]; //IP - 23/04/09 - CR929 & 974
				}
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void GetItemsForAccount()
		{
			_itemdetails = new DataTable();

			try
			{
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar,12);
				parmArray[0].Value = this.AccountNumber;

				this.RunSP("DN_LineItemsGetForAccountSP", parmArray, _itemdetails);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void GetUndeliveredItemsForAccount()
		{
			_itemdetails = new DataTable();

			try
			{
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar,12);
				parmArray[0].Value = this.AccountNumber;

				this.RunSP("DN_UndeliveredLineItemsGetForAccountSP", parmArray, _itemdetails);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void GetItemsForAccount(SqlConnection conn, SqlTransaction trans)
		{
			_itemdetails = new DataTable();

			try
			{
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar,12);
				parmArray[0].Value = this.AccountNumber;

				this.RunSP(conn, trans, "DN_LineItemsGetForAccountSP", parmArray, _itemdetails);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

        // 68181 RD 27/06/06 Added to ensure that cancellation record is posted to fact
      //69218/69300 method now returns a datatable so that stored procedure does not have to be repeatedly called - also repeated use of _itemdetails could lead to a problem JH 19/10/2007
        public DataTable GetItemsForCanxAccount(SqlConnection conn, SqlTransaction trans)
        {
            DataTable dtItems = new DataTable();

            try
            {
               string started = DateTime.Now.ToLongTimeString();
               string storedProc = "DN_LineItemsGetForCanxAccountSP";

                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar, 12);
                parmArray[0].Value = this.AccountNumber;

                this.RunSP(conn, trans, storedProc, parmArray, dtItems);

                logTime(storedProc,started, DateTime.Now.ToLongTimeString(), AccountNumber);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dtItems;
        }

		public int GetRealDiscount (SqlConnection conn, SqlTransaction trans, string accountNo)
		{
			DataTable realDiscount = new DataTable("realDiscount");

			try
			{
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@piAcctNo", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNo;

				if(conn!=null && trans!=null)
					result = this.RunSP(conn, trans, "DN_LineItem_GetRealDiscount", parmArray, realDiscount);
				else
					result = this.RunSP("DN_LineItem_GetRealDiscount", parmArray, realDiscount);
				this._realDiscount = Convert.ToDecimal(realDiscount.Rows[0]["realDiscount"]);

			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

		public void UpdateDelQty(SqlConnection conn, SqlTransaction trans, 
								string accountNo, short stockLocn, int agreementNo, 
								int itemID, string contractNo, double qty,
                                int parentItemID)                                           //IP - 17/05/11 - CR1212 - #3627 - Changed to use itemID and parentItemID rather than itemNo and parentItemNo
		{
			try
			{
				parmArray = new SqlParameter[7];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@agreementno", SqlDbType.Int);
				parmArray[1].Value = agreementNo;
				parmArray[2] = new SqlParameter("@stocklocn", SqlDbType.SmallInt);
				parmArray[2].Value = stockLocn;
				parmArray[3] = new SqlParameter("@itemID", SqlDbType.Int);                  //IP - 17/05/11 - CR1212 - #3627
				parmArray[3].Value = itemID;
				parmArray[4] = new SqlParameter("@contractno", SqlDbType.NVarChar, 10);
				parmArray[4].Value = contractNo;
				parmArray[5] = new SqlParameter("@qty", SqlDbType.Float);
				parmArray[5].Value = qty;
                parmArray[6] = new SqlParameter("@parentItemID", SqlDbType.Int);            //IP - 17/05/11 - CR1212 - #3627
                parmArray[6].Value = parentItemID;

				this.RunSP(conn, trans, "DN_LineItemsUpdateDelQtySP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

        public short HasAddToOrDelivery(SqlConnection conn, SqlTransaction trans, 
									string accountNo)
		{
			short addTo = 0;
			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@addto", SqlDbType.SmallInt);
				parmArray[1].Value = 0;
				parmArray[1].Direction = ParameterDirection.Output;				

				if(conn!=null && trans!=null)
                    RunSP(conn, trans, "DN_HasAddToOrDeliverySP", parmArray);
				else
                    RunSP("DN_HasAddToOrDeliverySP", parmArray);

				addTo = (short)parmArray[1].Value;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return addTo;
		}

		public short SettledByAddTo(SqlConnection conn, SqlTransaction trans, 
			string accountNo)
		{
			short reversible = 0;
			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@reversible", SqlDbType.SmallInt);
				parmArray[1].Value = 0;
				parmArray[1].Direction = ParameterDirection.Output;				

				if(conn!=null && trans!=null)
					RunSP(conn, trans, "DN_LineitemAccountAddedToSP", parmArray);
				else
					RunSP("DN_LineitemAccountSettledByAddToSP", parmArray);

				reversible = (short)parmArray[1].Value;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return reversible;
		}


		/// <summary>
		/// ProcessReplacement
		/// </summary>
		/// <param name="acctno">string</param>
		/// <param name="agrmtno">int</param>
		/// <param name="itemno">string</param>
		/// <param name="stocklocn">int</param>
		/// <param name="quantity">float</param>
		/// <returns>int</returns>
		/// 
		public int ProcessReplacement (SqlConnection conn, SqlTransaction trans, string acctno, 
							//int agrmtno, string itemno, int stocklocn, 
                            int agrmtno, int itemID, int stocklocn,                                         //IP - 09/06/11 - CR1212 - RI
							double quantity, string user, string contractNo, 
							short retStockLocn, bool nonStock)
		{
			int status = 0;
			
			try
			{
				parmArray = new SqlParameter[8];
				
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NChar, 24);
				parmArray[0].Value = acctno;
				
				parmArray[1] = new SqlParameter("@agrmtno", SqlDbType.Int);
				parmArray[1].Value = agrmtno;
				
                //parmArray[2] = new SqlParameter("@itemno", SqlDbType.NVarChar, 16);
                //parmArray[2].Value = itemno;

                parmArray[2] = new SqlParameter("@itemID", SqlDbType.Int);
                parmArray[2].Value = itemID;
				
				parmArray[3] = new SqlParameter("@stocklocn", SqlDbType.SmallInt);
				parmArray[3].Value = stocklocn;

				parmArray[4] = new SqlParameter("@contractno", SqlDbType.NVarChar,10);
				parmArray[4].Value = contractNo;
				
				parmArray[5] = new SqlParameter("@quantity", SqlDbType.Float);
				parmArray[5].Value = quantity;
				 
				parmArray[6] = new SqlParameter("@user", SqlDbType.NVarChar, 10);
				parmArray[6].Value = user;

				parmArray[7] = new SqlParameter("@retstocklocn", SqlDbType.SmallInt);
				parmArray[7].Value = retStockLocn;

				if(!nonStock)
					status = this.RunSP(conn, trans, "DN_ReplacementDeliveryNote", parmArray);
				else
					status = this.RunSP(conn, trans, "DN_ReplacementDeliveryNoteForWarranty", parmArray);

			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			
			return status;
		}

		public bool ContractNoUnique(SqlConnection conn, SqlTransaction trans, string accountNo, int agreementNo, string contractNo)
		{
			bool unique = false;
			try
			{
				parmArray = new SqlParameter[4];
				parmArray[0] = new SqlParameter("@contract", SqlDbType.NVarChar, 10);
				parmArray[0].Value = contractNo;
				parmArray[1] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
				parmArray[1].Value = accountNo;
				parmArray[2] = new SqlParameter("@agreementno", SqlDbType.Int);
				parmArray[2].Value = agreementNo;
				parmArray[3] = new SqlParameter("@unique", SqlDbType.SmallInt);
				parmArray[3].Value = 0;
				parmArray[3].Direction = ParameterDirection.Output;
		
				if(conn != null && trans != null)
					RunSP(conn, trans, "DN_LineItemContractUniqueSP", parmArray);
				else
					RunSP("DN_LineItemContractUniqueSP", parmArray);
				unique = !Convert.ToBoolean(parmArray[3].Value);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return unique;
		}

		public bool AffinityContractNoUnique(SqlConnection conn, SqlTransaction trans, string accountNo, int agreementNo, string contractNo)
		{
			bool unique = false;
			try
			{
				parmArray = new SqlParameter[4];
				parmArray[0] = new SqlParameter("@contract", SqlDbType.NVarChar, 10);
				parmArray[0].Value = contractNo;
				parmArray[1] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
				parmArray[1].Value = accountNo;
				parmArray[2] = new SqlParameter("@agreementno", SqlDbType.Int);
				parmArray[2].Value = agreementNo;
				parmArray[3] = new SqlParameter("@unique", SqlDbType.SmallInt);
				parmArray[3].Value = 0;
				parmArray[3].Direction = ParameterDirection.Output;
		
				if(conn != null && trans != null)
					RunSP(conn, trans, "DN_LineItemAffinityContractUniqueSP", parmArray);
				else
					RunSP("DN_LineItemAffinityContractUniqueSP", parmArray);
				unique = !Convert.ToBoolean(parmArray[3].Value);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return unique;
		}

		public bool NonStockCheck(SqlConnection conn, SqlTransaction trans, string accountNo)
		{
			bool nonstockexists = false;

			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@nonstockexists", SqlDbType.SmallInt);
				parmArray[1].Value = 0;
				parmArray[1].Direction = ParameterDirection.Output;				

				if(conn != null && trans != null)
					RunSP(conn, trans, "DN_NonStockCheckGetSP", parmArray);
				else
					RunSP("DN_NonStockCheckGetSP", parmArray);

				if(!Convert.IsDBNull(parmArray[1].Value))
					nonstockexists = Convert.ToBoolean(parmArray[1].Value);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return nonstockexists;
		}

		public bool CheckPreExistingDels(SqlConnection conn, SqlTransaction trans, string accountNo)
		{
			bool exists = false;

			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@exists", SqlDbType.SmallInt);
				parmArray[1].Value = 0;
				parmArray[1].Direction = ParameterDirection.Output;				

				RunSP(conn, trans, "DN_PreExistDelCheckSP", parmArray);

				if(!Convert.IsDBNull(parmArray[1].Value))
					exists = Convert.ToBoolean(parmArray[1].Value);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return exists;
		}

		public void UpdateStockLevel(SqlConnection conn, SqlTransaction trans)
		{
			try
			{
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@itemno", SqlDbType.NVarChar,8);
				parmArray[0].Value = this.ItemNumber;
				parmArray[1] = new SqlParameter("@stocklocn", SqlDbType.SmallInt);
				parmArray[1].Value = this.StockLocation;
				parmArray[2] = new SqlParameter("@quantity", SqlDbType.Float);
				parmArray[2].Value = this.Quantity;

				RunSP(conn, trans, "DN_UpdateStockLevelSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public int UpdateNotes(SqlConnection conn, SqlTransaction trans, string acctno,
            int agrmtno, int itemID, int stocklocn, string contractNo, string notes,                    //IP - 17/05/11 - CR1212 - #3627 - Changed to use itemID rather than itemno
			bool overWrite)
		{
			int status = 0;
			
			try
			{
				parmArray = new SqlParameter[7];
				
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NChar, 24);
				parmArray[0].Value = acctno;
				
				parmArray[1] = new SqlParameter("@agrmtno", SqlDbType.Int);
				parmArray[1].Value = agrmtno;
				
				parmArray[2] = new SqlParameter("@itemID", SqlDbType.Int);                               //IP - 17/05/11 - CR1212 - #3627
				parmArray[2].Value = itemID;
				
				parmArray[3] = new SqlParameter("@stocklocn", SqlDbType.SmallInt);
				parmArray[3].Value = stocklocn;

				parmArray[4] = new SqlParameter("@contractno", SqlDbType.NVarChar,10);
				parmArray[4].Value = contractNo;

                parmArray[5] = new SqlParameter("@notes", SqlDbType.NVarChar, 300); // CR1048  jec -- IP - 10/02/10 - CR1048 (Ref:3.1.4 & 3.1.5) Merged - Malaysia Enhancements (CR1072)
				parmArray[5].Value = notes;

				parmArray[6] = new SqlParameter("@overwrite", SqlDbType.Bit);
				parmArray[6].Value = overWrite;

				status = this.RunSP(conn, trans, "DN_LineItemUpdateNotesSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			
			return status;
		}

		/// <summary>
		/// UpdateTaxAmount
		/// </summary>
		/// <param name="acctno">string</param>
		/// <param name="itemno">string</param>
		/// <param name="branchno">int</param>
		/// <param name="taxamount">double</param>
		/// <returns>void</returns>
		/// 
        public void UpdateTaxAmount(SqlConnection conn, SqlTransaction trans, string acctno, int itemID, short branchno, decimal taxamount)    //IP/NM - 18/05/11 -CR1212 - #3627
		{
			try
			{
				parmArray = new SqlParameter[7];
				
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 24);
				parmArray[0].Value = acctno;

                parmArray[1] = new SqlParameter("@itemID", SqlDbType.Int);              //IP/NM - 18/05/11 -CR1212 - #3627
				parmArray[1].Value = itemID;
				
				parmArray[2] = new SqlParameter("@branchno", SqlDbType.SmallInt);
				parmArray[2].Value = branchno;
				
				parmArray[3] = new SqlParameter("@taxamount", SqlDbType.Money);
				parmArray[3].Value = taxamount;

                parmArray[4] = new SqlParameter("@agrmtno", SqlDbType.Int);
                parmArray[4].Value = AgreementNumber;

                parmArray[5] = new SqlParameter("@source", SqlDbType.NVarChar, 16); //IP - CR929 & 974 - Deliveries, changed from (15) to (16)
                parmArray[5].Value = AuditSource;

                parmArray[6] = new SqlParameter("@empeenochange", SqlDbType.Int);
                parmArray[6].Value = User;
				 				
				 				
				this.RunSP(conn, trans, "DN_LineItemUpdateTaxAmountSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public DataTable GetAuditData(string accountNo, int rowcount, SqlConnection conn = null, SqlTransaction trans = null)
		{
			DataTable dt = new DataTable(TN.LineItemAudit);

			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@rowcount", SqlDbType.Int);
				parmArray[1].Value = rowcount;

                if (conn != null && trans != null)
                    this.RunSP(conn, trans, "DN_LineItemAuditGetSP", parmArray, dt);
                else
                    this.RunSP("DN_LineItemAuditGetSP", parmArray, dt);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return dt;
		}

		/// <summary>
		/// GetWarrantiesOnCreditValue
		/// </summary>
		/// <param name="acctno">string</param>
		/// <param name="value">double</param>
		/// <returns>decimal</returns>
		/// 
		public decimal GetWarrantiesOnCreditValue (SqlConnection conn, SqlTransaction trans, string acctno)
		{			
			decimal val = 0;
			
			try
			{
				parmArray = new SqlParameter[2];
				
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 24);
				parmArray[0].Value = acctno;
				
				parmArray[1] = new SqlParameter("@value", SqlDbType.Money);
				parmArray[1].Value = 0;
				parmArray[1].Direction = ParameterDirection.Output; 
				
				this.RunSP(conn, trans, "DN_LineItemGetWarrantiesOnCreditValueSP", parmArray);
	
				if(parmArray[1].Value!=DBNull.Value)
					val = (decimal)parmArray[1].Value;
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return val;
		}

		public void UpdateItemLocation(SqlConnection conn, SqlTransaction trans, 
										int newbuffNo, short origLocation)		
		{
			try
			{
				parmArray = new SqlParameter[20];
				
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = this.AccountNumber;
				parmArray[1] = new SqlParameter("@agreementno", SqlDbType.Int);
				parmArray[1].Value = this.AgreementNumber;
				parmArray[2] = new SqlParameter("@itemId", SqlDbType.Int);
				parmArray[2].Value = this.ItemID;
				parmArray[3] = new SqlParameter("@stocklocn", SqlDbType.SmallInt);
				parmArray[3].Value = this.StockLocation;
				parmArray[4] = new SqlParameter("@datereqdel", SqlDbType.DateTime);
				parmArray[4].Value = this.DateRequiredDelivery;
				parmArray[5] = new SqlParameter("@timereqdel", SqlDbType.NVarChar,12);
				parmArray[5].Value = this.TimeRequiredDelivery;
				parmArray[6] = new SqlParameter("@dnbranch", SqlDbType.SmallInt);
				parmArray[6].Value = this.DeliveryNoteBranch;
				parmArray[7] = new SqlParameter("@notes", SqlDbType.NVarChar,200);
				parmArray[7].Value = this.Notes;
				parmArray[8] = new SqlParameter("@deliveryaddress", SqlDbType.NChar,2);
				parmArray[8].Value = this.DeliveryAddress;
				parmArray[9] = new SqlParameter("@contractno", SqlDbType.NVarChar,10);
				parmArray[9].Value = this.ContractNo;
				parmArray[10] = new SqlParameter("@deliveryarea", SqlDbType.VarChar,8);         // #10712 
				parmArray[10].Value = this.DeliveryArea;
				parmArray[11] = new SqlParameter("@deliveryprocess", SqlDbType.NChar,1);
				parmArray[11].Value = this.DeliveryProcess;
				parmArray[12] = new SqlParameter("@damaged", SqlDbType.NChar,1);
				parmArray[12].Value = this.Damaged;
				parmArray[13] = new SqlParameter("@buffno", SqlDbType.Int);
				parmArray[13].Value = this.BuffNo;
				parmArray[14] = new SqlParameter("@newbuffno", SqlDbType.Int);
				parmArray[14].Value = newbuffNo;
				parmArray[15] = new SqlParameter("@origlocation", SqlDbType.SmallInt);
				parmArray[15].Value = origLocation;
				parmArray[16] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[16].Value = this.User;
                parmArray[17] = new SqlParameter("@assembly", SqlDbType.NChar, 1);
                parmArray[17].Value = this.Assembly;
                parmArray[18] = new SqlParameter("@parentitemId", SqlDbType.Int);
                parmArray[18].Value = this.ParentItemID;
                parmArray[19] = new SqlParameter("@express", SqlDbType.Char, 1);                    //IP - 07/06/12 - #10229 - Warehouse & Deliveries
                parmArray[19].Value = this.Express;

				this.RunSP(conn, trans, "DN_LineItemUpdateItemLocationSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

        //IP - 28/04/09 - CR929 & 974 Deliveries - boolean to determine whether order details can be changed
        //prior to being DA'ed.
		public DataSet GetItemsForLocationChange(bool loadBeforeDA)
		{
            var schedulechange = new DataSet() { DataSetName = "schedulechange" };

				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar,12);
				parmArray[0].Value = this.AccountNumber;
                parmArray[1] = new SqlParameter("@loadBeforeDA", SqlDbType.Bit);
                parmArray[1].Value = loadBeforeDA;
                this.RunSP("DN_LineItemsGetForLocationChangeSP", parmArray, schedulechange);
                return schedulechange;
		}

		public void UpdateDeliveryNote(SqlConnection conn, SqlTransaction trans)
		{
			try
			{
				parmArray = new SqlParameter[6];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = this.AccountNumber;
				parmArray[1] = new SqlParameter("@agreementno", SqlDbType.Int);
				parmArray[1].Value = this.AgreementNumber;
				parmArray[2] = new SqlParameter("@stocklocn", SqlDbType.SmallInt);
				parmArray[2].Value = this.StockLocation;
				parmArray[3] = new SqlParameter("@itemId", SqlDbType.Int);
				parmArray[3].Value = this.ItemID;
				parmArray[4] = new SqlParameter("@contractno", SqlDbType.NVarChar, 10);
				parmArray[4].Value = this.ContractNo;
				parmArray[5] = new SqlParameter("@qty", SqlDbType.Float);
				parmArray[5].Value = this.Quantity;

				this.RunSP(conn, trans, "DN_LineItemsUpdateDelNote", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void UpdateItemReturnNo(SqlConnection conn, SqlTransaction trans)
		{
			try
			{
				parmArray = new SqlParameter[8];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = this.AccountNumber;
				parmArray[1] = new SqlParameter("@agreementno", SqlDbType.Int);
				parmArray[1].Value = this.AgreementNumber;
				parmArray[2] = new SqlParameter("@locn", SqlDbType.SmallInt);
				parmArray[2].Value = this.StockLocation;
                parmArray[3] = new SqlParameter("@itemId", SqlDbType.Int);
				parmArray[3].Value = this.ItemID;
				parmArray[4] = new SqlParameter("@contractno", SqlDbType.NVarChar, 10);
				parmArray[4].Value = this.ContractNo;
                parmArray[5] = new SqlParameter("@returnitemId", SqlDbType.Int);
				parmArray[5].Value = this.ReturnItemId;
                parmArray[6] = new SqlParameter("@parentItemId", SqlDbType.Int);
                parmArray[6].Value = this.ParentItemID;
				parmArray[7] = new SqlParameter("@retlocn", SqlDbType.SmallInt);
				parmArray[7].Value = this.ReturnStockLocn;

				this.RunSP(conn, trans, "DN_DeliveryUpdateReturnItemNoSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public DataTable GetAssociatedDiscounts(string accountNo, int itemId, short stockLocn)          // RI
		{
			DataTable dt = new DataTable(TN.Discounts);
			try
			{
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
				parmArray[0].Value = accountNo;
                //parmArray[1] = new SqlParameter("@itemno", SqlDbType.NVarChar, 8);
                //parmArray[1].Value = itemNo;
                parmArray[1] = new SqlParameter("@itemid", SqlDbType.Int);              // RI
                parmArray[1].Value = itemId;
				parmArray[2] = new SqlParameter("@stocklocn", SqlDbType.SmallInt);
				parmArray[2].Value = stockLocn;
		
				RunSP("DN_LineItemGetAssociatedDiscountsSP", parmArray, dt);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return dt;
		}


		// 67977 RD 22/02/06  Added TaxAmtAfter and TaxAmtBefore
		public void UpdateLineItemAudit(SqlConnection conn, SqlTransaction trans, double quantityBefore,
			double quantityAfter, decimal valueBefore, decimal valueAfter,double taxamtBefore, double taxamtAfter)  
		{
			parmArray = new SqlParameter[18];
			parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
			parmArray[0].Value = this.AccountNumber;
			parmArray[1] = new SqlParameter("@agrmtno", SqlDbType.Int);
			parmArray[1].Value = this.AgreementNumber;
			parmArray[2] = new SqlParameter("@empeenochange", SqlDbType.Int);
			parmArray[2].Value = this.User;
			parmArray[3] = new SqlParameter("@itemId", SqlDbType.Int);
			parmArray[3].Value = this.ItemID;
			parmArray[4] = new SqlParameter("@stocklocn", SqlDbType.SmallInt);
			parmArray[4].Value = this.StockLocation;
			parmArray[5] = new SqlParameter("@quantitybefore", SqlDbType.Float);
			parmArray[5].Value = quantityBefore;
			parmArray[6] = new SqlParameter("@quantityafter", SqlDbType.Float);
			parmArray[6].Value = quantityAfter;
			parmArray[7] = new SqlParameter("@valuebefore", SqlDbType.Money);
			parmArray[7].Value = valueBefore;
			parmArray[8] = new SqlParameter("@valueafter", SqlDbType.Money);
			parmArray[8].Value = valueAfter;
			parmArray[9] = new SqlParameter("@taxamtbefore", SqlDbType.Float); // 67977 RD 22/02/06  
			parmArray[9].Value = taxamtBefore;
			parmArray[10] = new SqlParameter("@taxamtafter", SqlDbType.Float); // 67977 RD 22/02/06 
			parmArray[10].Value = taxamtAfter;
			parmArray[11] = new SqlParameter("@datechange", SqlDbType.DateTime);
			parmArray[11].Value = DateTime.Now;
			parmArray[12] = new SqlParameter("@contractno", SqlDbType.NVarChar,10);
			parmArray[12].Value = this.ContractNo;
            parmArray[13] = new SqlParameter("@source", SqlDbType.NVarChar, 15);
            parmArray[13].Value = this.AuditSource;
            parmArray[14] = new SqlParameter("@parentItemId", SqlDbType.Int); //UAT 114 - IP - 22/11/2007
            parmArray[14].Value = this.ParentItemID; //IP - CR929 & 974 - 23/04/09 - should use the classes values
            parmArray[15] = new SqlParameter("@parentStockLocn", SqlDbType.SmallInt); //UAT 114 - IP - 22/11/2007
            parmArray[15].Value = this.ParentStockLocation; //IP - CR929 & 974 - 23/04/09 - should use the classes values
            parmArray[16] = new SqlParameter("@delnotebranch", SqlDbType.SmallInt); //IP - 05/02/10 - CR1072 - 3.1.12 - Add delnotebranch to Lineitemaudit
            parmArray[16].Value = this.DeliveryNoteBranch;
            parmArray[17] = new SqlParameter("@salesBrnNo", SqlDbType.SmallInt); //IP - 24/05/11 - CR1212 - RI - #3651
            parmArray[17].Value = this.SalesBrnNo ?? (object)DBNull.Value;

			this.RunSP(conn, trans, "DN_LineItemAuditUpdateSP", parmArray);
		}
        /// <summary>
        /// Puts record into audit table for removed non-stocks which are not stored in the XML lineitems. Uses acctno,agrmtno and current user
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        public void LineItemAuditRemovedNonStocks(SqlConnection conn, SqlTransaction trans)
        {
            parmArray = new SqlParameter[3];
            parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
            parmArray[0].Value = this.AccountNumber;
            parmArray[1] = new SqlParameter("@agrmtno", SqlDbType.Int);
            parmArray[1].Value = this.AgreementNumber;
            parmArray[2] = new SqlParameter("@empeenochange", SqlDbType.Int);
            parmArray[2].Value = this.User;

            this.RunSP(conn, trans, "DN_LineItemAuditRemovedNonStocks", parmArray);
        }


		public void CheckForStockItems(SqlConnection conn, SqlTransaction trans, out int stockCount  , out int affinityCount)
		{
			try
			{
				stockCount = 0;
				affinityCount = 0;
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar,12);
				parmArray[0].Value = this.AccountNumber;
				parmArray[1] = new SqlParameter("@stockCount", SqlDbType.Int);
				parmArray[1].Value = stockCount;
				parmArray[1].Direction = ParameterDirection.Output;
				parmArray[2] = new SqlParameter("@affinityCount", SqlDbType.Int);
				parmArray[2].Value = affinityCount;
				parmArray[2].Direction = ParameterDirection.Output;

				this.RunSP(conn, trans, "DN_LineItemsCheckForStockItems", parmArray);
				
				if(!Convert.IsDBNull(parmArray[1].Value))
					stockCount = (int)parmArray[1].Value;
				if(!Convert.IsDBNull(parmArray[2].Value))    
					affinityCount = (int)parmArray[2].Value; 
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void GetRelatedItems()
		{
			_itemdetails = new DataTable("RelatedItems");

			try
			{
				parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@itemId", SqlDbType.Int);
				parmArray[0].Value = this.ItemID;
                parmArray[1] = new SqlParameter("@branchno", SqlDbType.SmallInt);
                parmArray[1].Value = this.StockLocation;

				this.RunSP("DN_GetRelatedLineItemsSP", parmArray, _itemdetails);
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void LinkWarrantyToItem(SqlConnection conn, SqlTransaction trans,
            int warrantyID, short warrantylocn)                                     //IP/NM - 18/05/11 -CR1212 - #3627 
		{
			try
			{
				parmArray = new SqlParameter[6];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = this.AccountNumber;
                parmArray[1] = new SqlParameter("@itemID", SqlDbType.Int);           //IP/NM - 18/05/11 -CR1212 - #3627 
				parmArray[1].Value = this.ItemID;
				parmArray[2] = new SqlParameter("@locn", SqlDbType.SmallInt);
				parmArray[2].Value = this.StockLocation;
                parmArray[3] = new SqlParameter("@warrantyID", SqlDbType.Int);      //IP/NM - 18/05/11 -CR1212 - #3627 
				parmArray[3].Value = warrantyID;
				parmArray[4] = new SqlParameter("@warrantylocn", SqlDbType.SmallInt);
				parmArray[4].Value = warrantylocn;
				parmArray[5] = new SqlParameter("@contractno", SqlDbType.NVarChar,10);
				parmArray[5].Value = this.ContractNo;

				this.RunSP(conn, trans, "DN_LineItemLinkWarrantySP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

        

        public void UpdateRequiredDelDate(SqlConnection conn, SqlTransaction trans,
                                //string accountNo, int agreementNo, string itemNo,
                                string accountNo, int agreementNo, int itemID,                      //IP - 22/07/11 - RI
                                short location, string contractNo, DateTime dateReqDel)
        {
            try
            {
                parmArray = new SqlParameter[6];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[0].Value = accountNo;
                parmArray[1] = new SqlParameter("@agreementno", SqlDbType.Int);
                parmArray[1].Value = agreementNo;
                //parmArray[2] = new SqlParameter("@itemno", SqlDbType.NVarChar, 8);
                //parmArray[2].Value = itemNo;
                parmArray[2] = new SqlParameter("@itemID", SqlDbType.Int);                          //IP - 22/07/11 - RI    
                parmArray[2].Value = itemID;
                parmArray[3] = new SqlParameter("@location", SqlDbType.SmallInt);
                parmArray[3].Value = location;
                parmArray[4] = new SqlParameter("@contractno", SqlDbType.NVarChar, 10);
                parmArray[4].Value = contractNo;
                parmArray[5] = new SqlParameter("@datereqdel", SqlDbType.DateTime);
                parmArray[5].Value = dateReqDel;

                this.RunSP(conn, trans, "DN_LineItemUpdateDateReqDelSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void UpdateAuditItem(SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                parmArray = new SqlParameter[5];
                parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar, 12);
                parmArray[0].Value = this.AccountNumber;
                parmArray[1] = new SqlParameter("@agreementNo", SqlDbType.Int);
                parmArray[1].Value = this.AgreementNumber;
                parmArray[2] = new SqlParameter("@itemNo", SqlDbType.NVarChar, 8);
                parmArray[2].Value = this.ItemNumber;
                parmArray[3] = new SqlParameter("@location", SqlDbType.SmallInt);
                parmArray[3].Value = this.StockLocation;
                parmArray[4] = new SqlParameter("@source", SqlDbType.NChar, 15);
                parmArray[4].Value = this.AuditSource;

                this.RunSP(conn, trans, "DN_LineItemAuditUpdateItemSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

      public void UpdateLineItemDelNoteBranch(SqlConnection conn, SqlTransaction trans,
                                //string accountNo, string itemNo, short retStockLocn)
                                 string accountNo, int itemID, short retStockLocn)              //IP - 01/08/11 - RI
      {
         try
         {
            parmArray = new SqlParameter[3];
            parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
            parmArray[0].Value = accountNo;          
            //parmArray[1] = new SqlParameter("@itemno", SqlDbType.NVarChar, 8);
            //parmArray[1].Value = itemNo;
            parmArray[1] = new SqlParameter("@itemID", SqlDbType.Int);                          //IP - 01/08/11 - RI
            parmArray[1].Value = ItemID;
            parmArray[2] = new SqlParameter("@retstocklocn", SqlDbType.SmallInt);
            parmArray[2].Value = retStockLocn;

            this.RunSP(conn, trans, "LineItemUpdateDelNoteBranchSP", parmArray);
         }
         catch (SqlException ex)
         {
            LogSqlException(ex);
            throw ex;
         }
      }

        //BOC Added by Suvidha - CR 2018-13 - 05/01/19 - to update invoice version in table InvoiceDetails.
        public void UpdateInvoiceVersion(SqlConnection conn, SqlTransaction trans, string accountNo, int agreementNo)
        {
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar, 12);
                parmArray[0].Value = accountNo;                
                parmArray[1] = new SqlParameter("@agrmtno", SqlDbType.Int);
                parmArray[1].Value = agreementNo;

                this.RunSP(conn, trans, "DN_UpdateInvoiceVersion", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }
        //EOC

        public void GetItemDetailsForReprint(SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                parmArray = new SqlParameter[46];
                parmArray[0] = new SqlParameter("@origbr", SqlDbType.SmallInt);
                parmArray[0].Value = this.OrigBr;
                parmArray[0].Direction = ParameterDirection.Output;
                parmArray[1] = new SqlParameter("@acctNo", SqlDbType.NVarChar, 12);
                parmArray[1].Value = this.AccountNumber;
                parmArray[2] = new SqlParameter("@agreementNo", SqlDbType.Int);
                parmArray[2].Value = this.AgreementNumber;
                parmArray[3] = new SqlParameter("@itemId", SqlDbType.Int);
                parmArray[3].Value = this.ItemID;
                parmArray[4] = new SqlParameter("@itemSuppText", SqlDbType.NVarChar, 76);
                parmArray[4].Value = this.ItemSuppText;
                parmArray[4].Direction = ParameterDirection.Output;
                parmArray[5] = new SqlParameter("@quantity", SqlDbType.Float);
                parmArray[5].Value = this.Quantity;
                parmArray[5].Direction = ParameterDirection.Output;
                parmArray[6] = new SqlParameter("@delQty", SqlDbType.Float);
                parmArray[6].Value = this.DeliveredQuantity;
                parmArray[6].Direction = ParameterDirection.Output;
                parmArray[7] = new SqlParameter("@stockLocn", SqlDbType.SmallInt);
                parmArray[7].Value = this.StockLocation;
                parmArray[8] = new SqlParameter("@price", SqlDbType.Money);
                parmArray[8].Value = this.Price;
                parmArray[8].Direction = ParameterDirection.Output;
                parmArray[9] = new SqlParameter("@orderValue", SqlDbType.Money);
                parmArray[9].Value = this.OrderValue;
                parmArray[9].Direction = ParameterDirection.Output;
                parmArray[10] = new SqlParameter("@dateReqDel", SqlDbType.DateTime);
                parmArray[10].Value = this.DateRequiredDelivery;
                parmArray[10].Direction = ParameterDirection.Output;
                parmArray[11] = new SqlParameter("@timeReqDel", SqlDbType.NVarChar, 12);
                parmArray[11].Value = this.TimeRequiredDelivery;
                parmArray[11].Direction = ParameterDirection.Output;
                parmArray[12] = new SqlParameter("@datePlanDel", SqlDbType.DateTime);
                parmArray[12].Value = this.DatePlannedDelivery;
                parmArray[12].Direction = ParameterDirection.Output;
                parmArray[13] = new SqlParameter("@delNoteBranch", SqlDbType.SmallInt);
                parmArray[13].Value = this.DeliveryNoteBranch;
                parmArray[13].Direction = ParameterDirection.Output;
                parmArray[14] = new SqlParameter("@qtyDiff", SqlDbType.NChar, 1);
                parmArray[14].Value = this.QuantityDiff;
                parmArray[14].Direction = ParameterDirection.Output;
                parmArray[15] = new SqlParameter("@itemType", SqlDbType.NVarChar, 1);
                parmArray[15].Value = this.ItemType;
                parmArray[15].Direction = ParameterDirection.Output;
                parmArray[16] = new SqlParameter("@hasString", SqlDbType.SmallInt);
                parmArray[16].Value = this.HasString;
                parmArray[16].Direction = ParameterDirection.Output;
                parmArray[17] = new SqlParameter("@notes", SqlDbType.NVarChar, 200);
                parmArray[17].Value = this.Notes;
                parmArray[17].Direction = ParameterDirection.Output;
                parmArray[18] = new SqlParameter("@taxAmount", SqlDbType.Float);
                parmArray[18].Value = this.TaxAmount;
                parmArray[18].Direction = ParameterDirection.Output;
                parmArray[19] = new SqlParameter("@parentItemId", SqlDbType.Int);
                parmArray[19].Value = this.ParentItemID;
                // uat363 rdb will now filter on ParentItemNumber use as input value
                //parmArray[19].Direction = ParameterDirection.Output;
                parmArray[20] = new SqlParameter("@parentStockLocn", SqlDbType.SmallInt);
                parmArray[20].Value = this.ParentStockLocation;
                parmArray[20].Direction = ParameterDirection.Output;
                parmArray[21] = new SqlParameter("@isKit", SqlDbType.SmallInt);
                parmArray[21].Value = this.IsKit;
                parmArray[21].Direction = ParameterDirection.Output;
                parmArray[22] = new SqlParameter("@lastDelivery", SqlDbType.DateTime);
                parmArray[22].Value = this.DateOfLastDelivery;
                parmArray[22].Direction = ParameterDirection.Output;
                parmArray[23] = new SqlParameter("@deliveryAddress", SqlDbType.NChar, 2);
                parmArray[23].Value = this.DeliveryAddress;
                parmArray[23].Direction = ParameterDirection.Output;
                parmArray[24] = new SqlParameter("@scheduled", SqlDbType.Float);
                parmArray[24].Value = this.ScheduledQuantity;
                parmArray[24].Direction = ParameterDirection.Output;
                parmArray[25] = new SqlParameter("@contractNo", SqlDbType.NVarChar, 10);
                parmArray[25].Value = this.ContractNo;
                parmArray[26] = new SqlParameter("@expectedreturndate", SqlDbType.DateTime);
                parmArray[26].Value = this.ExpectedReturnDate;
                parmArray[26].Direction = ParameterDirection.Output;
                parmArray[27] = new SqlParameter("@deliveryarea", SqlDbType.NVarChar, 8);
                parmArray[27].Value = this.DeliveryArea;
                parmArray[27].Direction = ParameterDirection.Output;
                parmArray[28] = new SqlParameter("@deliveryprocess", SqlDbType.NChar, 1);
                parmArray[28].Value = this.DeliveryProcess;
                parmArray[28].Direction = ParameterDirection.Output;
                parmArray[29] = new SqlParameter("@damaged", SqlDbType.NChar, 1);
                parmArray[29].Value = this.Damaged;
                parmArray[29].Direction = ParameterDirection.Output;
                parmArray[30] = new SqlParameter("@assembly", SqlDbType.NChar, 1);
                parmArray[30].Value = this.Assembly;
                parmArray[30].Direction = ParameterDirection.Output;
                parmArray[31] = new SqlParameter("@spiff", SqlDbType.Bit);
                parmArray[31].Value = this.SPIFFItem;
                parmArray[31].Direction = ParameterDirection.Output;


                //IP/JC - 02/03/10 - CR1072 - Malaysia 3PL 
                parmArray[32] = new SqlParameter("@VanNo", SqlDbType.NVarChar, 8);
                parmArray[32].Direction = ParameterDirection.Output;
                parmArray[33] = new SqlParameter("@DhlInterfaceDate", SqlDbType.DateTime);
                parmArray[33].Direction = ParameterDirection.Output;
                parmArray[34] = new SqlParameter("@DhlPickingDate", SqlDbType.DateTime);
                parmArray[34].Direction = ParameterDirection.Output;
                parmArray[35] = new SqlParameter("@DHLDNNo", SqlDbType.NVarChar, 10);
                parmArray[35].Direction = ParameterDirection.Output;

                //parmArray[36] = new SqlParameter("@OrigQty", SqlDbType.Int);
                //parmArray[36].Direction = ParameterDirection.Output;
                parmArray[36] = new SqlParameter("@ShipQty", SqlDbType.Int);
                parmArray[36].Direction = ParameterDirection.Output;

                parmArray[37] = new SqlParameter("@ItemRejected", SqlDbType.Bit);       //IP - 04/06/10 - UAT(262) UAT5.2.1.0 Log
                parmArray[37].Direction = ParameterDirection.Output;

                parmArray[38] = new SqlParameter("@IsComponent", SqlDbType.Bit);
                parmArray[38].Value = this.IsComponent;
                parmArray[38].Direction = ParameterDirection.Output;

                parmArray[39] = new SqlParameter("@itemno", SqlDbType.VarChar, 18);
                parmArray[39].Value = this.ItemNumber;
                parmArray[39].Direction = ParameterDirection.Output;

                parmArray[40] = new SqlParameter("@parentItemNo", SqlDbType.VarChar, 18);
                parmArray[40].Value = this.ParentItemNumber;
                parmArray[40].Direction = ParameterDirection.Output;

                parmArray[41] = new SqlParameter("@salesBrnNo", SqlDbType.SmallInt);            //IP - 23/05/11 - CR1212 - RI - #3651
                parmArray[41].Value = this.SalesBrnNo;
                parmArray[41].Direction = ParameterDirection.Output;

                parmArray[42] = new SqlParameter("@repoItem", SqlDbType.Bit);            //jec 16/06/11
                parmArray[42].Direction = ParameterDirection.Output;

                parmArray[43] = new SqlParameter("@express", SqlDbType.Char, 1);            //IP - 07/06/12 - #10229 - Warehouse & Deliveries
                parmArray[43].Direction = ParameterDirection.Output;

                parmArray[44] = new SqlParameter("@lineItemId", SqlDbType.Int);            //#13716 - CR12949
                parmArray[44].Direction = ParameterDirection.Output;

                parmArray[45] = new SqlParameter("@invoiceVersion", SqlDbType.Int);
                parmArray[45].Value = this.InvoiceVersion;

                if (conn != null && trans != null)
                    RunSP(conn, trans, "DN_LineItemGetDetailsSPForTaxInvoice", parmArray);
                else
                    RunSP("DN_LineItemGetDetailsSPForTaxInvoice", parmArray);

                if (!Convert.IsDBNull(parmArray[0].Value))
                    this.OrigBr = (short)parmArray[0].Value;
                if (!Convert.IsDBNull(parmArray[2].Value))
                    this.AgreementNumber = (int)parmArray[2].Value;
                if (!Convert.IsDBNull(parmArray[4].Value))
                    this.ItemSuppText = (string)parmArray[4].Value;
                if (!Convert.IsDBNull(parmArray[5].Value))
                    this.Quantity = (double)parmArray[5].Value;
                if (!Convert.IsDBNull(parmArray[6].Value))
                    this.DeliveredQuantity = (double)parmArray[6].Value;
                if (!Convert.IsDBNull(parmArray[8].Value))
                    this.Price = (decimal)parmArray[8].Value;
                if (!Convert.IsDBNull(parmArray[9].Value))
                    this.OrderValue = (decimal)parmArray[9].Value;
                if (!Convert.IsDBNull(parmArray[10].Value))
                    this.DateRequiredDelivery = (DateTime)parmArray[10].Value;
                if (!Convert.IsDBNull(parmArray[11].Value))
                    this.TimeRequiredDelivery = (string)parmArray[11].Value;
                if (!Convert.IsDBNull(parmArray[12].Value))
                    this.DatePlannedDelivery = (DateTime)parmArray[12].Value;
                if (!Convert.IsDBNull(parmArray[13].Value))
                    this.DeliveryNoteBranch = (short)parmArray[13].Value;
                if (!Convert.IsDBNull(parmArray[14].Value))
                    this.QuantityDiff = (string)parmArray[14].Value;
                if (!Convert.IsDBNull(parmArray[15].Value))
                    this.ItemType = (string)parmArray[15].Value;
                if (!Convert.IsDBNull(parmArray[16].Value))
                    this.HasString = (short)parmArray[16].Value;
                if (!Convert.IsDBNull(parmArray[17].Value))
                    this.Notes = (string)parmArray[17].Value;
                if (!Convert.IsDBNull(parmArray[18].Value))
                    this.TaxAmount = (double)parmArray[18].Value;
                if (!Convert.IsDBNull(parmArray[20].Value))
                    this.ParentStockLocation = (short)parmArray[20].Value;
                if (!Convert.IsDBNull(parmArray[21].Value))
                    this.IsKit = (short)parmArray[21].Value;
                if (!Convert.IsDBNull(parmArray[22].Value))
                    this.DateOfLastDelivery = (DateTime)parmArray[22].Value;
                if (!Convert.IsDBNull(parmArray[23].Value))
                    this.DeliveryAddress = (string)parmArray[23].Value;
                if (!Convert.IsDBNull(parmArray[24].Value))
                    this.ScheduledQuantity = (double)parmArray[24].Value;
                if (!Convert.IsDBNull(parmArray[26].Value))
                    this.ExpectedReturnDate = (DateTime)parmArray[26].Value;
                if (!Convert.IsDBNull(parmArray[27].Value))
                    this.DeliveryArea = (string)parmArray[27].Value;
                if (!Convert.IsDBNull(parmArray[28].Value))
                    this.DeliveryProcess = (string)parmArray[28].Value;
                if (!Convert.IsDBNull(parmArray[29].Value))
                    this.Damaged = (string)parmArray[29].Value;
                if (!Convert.IsDBNull(parmArray[30].Value))
                    this.Assembly = (string)parmArray[30].Value;
                if (!Convert.IsDBNull(parmArray[31].Value))
                    this.SPIFFItem = (bool)parmArray[31].Value;
                //IP/JC - 02/03/10 - CR1072 - Malaysia 3PL 
                if (parmArray[32].Value != DBNull.Value)
                    this.VanNo = parmArray[32].Value.ToString();
                if (parmArray[33].Value != DBNull.Value)
                    this.DhlInterfaceDate = Convert.ToDateTime(parmArray[33].Value);
                if (parmArray[34].Value != DBNull.Value)
                    this.DhlPickingDate = Convert.ToDateTime(parmArray[34].Value);
                if (parmArray[35].Value != DBNull.Value)
                    this.DhlDNNo = parmArray[35].Value.ToString();

                ////IP/JC - 02/03/10 - CR1072 - Malaysia 3PL 
                //if (parmArray[36].Value != DBNull.Value)
                //    this.OrigQty= parmArray[36].Value.ToString();
                if (parmArray[36].Value != DBNull.Value)        //JeC - 08/03/10 - Malaysia 3PL
                    this.ShipQty = parmArray[36].Value.ToString();

                if (parmArray[37].Value != DBNull.Value)
                    this.ItemRejected = Convert.ToBoolean(parmArray[37].Value);     //IP - 04/06/10 - UAT(262) UAT5.2.1.0 Log
                if (parmArray[38].Value != DBNull.Value)
                    this.IsComponent = Convert.ToBoolean(parmArray[38].Value);     //AA - 07/07/10 - UAT(267) UAT5.2.1.0 Log

                if (parmArray[39].Value != DBNull.Value)
                    this.ItemNumber = Convert.ToString(parmArray[39].Value) ?? "";
                if (parmArray[40].Value != DBNull.Value)
                    this.ParentItemNumber = Convert.ToString(parmArray[40].Value) ?? "";

                //IP - 23/05/11 - CR1212 - RI - #3651
                this.SalesBrnNo = parmArray[41].Value != DBNull.Value ? Convert.ToInt16(parmArray[41].Value) : (Int16?)null;

                if (parmArray[42].Value != DBNull.Value)                //jec 16/06/11
                    this.RepoItem = Convert.ToBoolean(parmArray[42].Value);

                //IP - 07/06/12 - #102229 - Warehouse & Deliveries
                if (!Convert.IsDBNull(parmArray[43].Value))
                    this.Express = (string)parmArray[43].Value;

                //#13716 - CR12949
                if (!Convert.IsDBNull(parmArray[44].Value))
                    this.LineItemId = (Int32)parmArray[44].Value;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        //Suvidha CR 2018-13
        public DataTable GetSalesOrderLineItemCodes(SqlConnection conn, SqlTransaction trans,
            int agreementNo, string agreementInvNo)
        {
            try
            {
                _codes = new DataTable("SalesOrderItems");
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@agreementNo", SqlDbType.Int);
                parmArray[0].Value = agreementNo;
                parmArray[1] = new SqlParameter("@AgreementInvoiceNumber", SqlDbType.NVarChar, 14);
                parmArray[1].Value = agreementInvNo;
                if (conn != null && trans != null)
                    RunSP(conn, trans, "DN_GetSalesOrderItemSP", parmArray, _codes);
                else
                    RunSP("DN_GetSalesOrderItemSP", parmArray, _codes);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return Codes;
        }

        public DataTable GetChildLineItemCodes_Reprint(SqlConnection conn, SqlTransaction trans, string accountNumber, int agreementNo, int parentItemId, short branch, int invoiceversion)
        {
            try
            {
                _codes = new DataTable("Codes");
                parmArray = new SqlParameter[5];
                parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar, 12);
                parmArray[0].Value = accountNumber;
                parmArray[1] = new SqlParameter("@itemId", SqlDbType.Int);
                parmArray[1].Value = parentItemId;
                parmArray[2] = new SqlParameter("@location", SqlDbType.SmallInt);
                parmArray[2].Value = branch;
                parmArray[3] = new SqlParameter("@agreementNo", SqlDbType.Int);
                parmArray[3].Value = agreementNo;
                parmArray[4] = new SqlParameter("@invoiceversion", SqlDbType.Int);
                parmArray[4].Value = invoiceversion;
                if (conn != null && trans != null)
                    this.RunSP(conn, trans, "DN_LineItemGetChildCodesSP_Reprint", parmArray, _codes);
                else
                    this.RunSP("DN_LineItemGetChildCodesSP_Reprint", parmArray, _codes);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return Codes;
        }

        private void logTime(string storedProc, string started, string ended, string parameter)
      {
         LogPerformanceMessage(storedProc + " Started at : " + started + " and Ended at : " + ended + " with parameter: " + parameter, Environment.StackTrace, EventLogEntryType.Information);
      }
	}
}
