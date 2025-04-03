using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common.Constants.Enums;
using STL.Common.Constants.TableNames;
using STL.Common.Static;


namespace STL.DAL
{
	/// <summary>
	/// Summary description for DDelivery.
	/// </summary>
	public class DDelivery : DALObject
	{
		private string _contractno = "";
		public string ContractNo
		{
			get{return _contractno;}
			set{_contractno = value;}
		}
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
		private int _agreementNo = 0;
		public int AgreementNumber 
		{
			get{return _agreementNo;}
			set{_agreementNo = value;}
		}

        private string _collectreason = "";
        public string CollectReason
        {
            get { return _collectreason; }
            set { _collectreason = value; }
        }

        private string _collecttype = "";
        public string CollectType
        {
            get { return _collecttype; }
            set { _collecttype = value; }
        }

        private DateTime _dateDel = DateTime.Today;
		public DateTime DateDelivered
		{
			get{return _dateDel;}
			set{_dateDel = value;}
		}
		private string _delorcoll = "";
		public string DeliveryOrCollection
		{
			get	{return _delorcoll;}
			set{_delorcoll = value;}
		}
		private string _itemno = "";
		public string ItemNumber
		{
			get	{return _itemno;}
			set{_itemno = value;}
		}

        public int ItemId { get; set; }

		private short _stockLocn = 0;
		public short StockLocation
		{
			get{return _stockLocn;}
			set{_stockLocn = value;}
		}
		private double _quantity = 0;
		public double Quantity
		{
			get{return _quantity;}
			set{_quantity = value;}
		}
		private string _retitemno = "";
		public string ReturnItemNumber
		{
			get	{return _retitemno;}
			set{_retitemno = value;}
		}

        public int ReturnItemId { get; set; }

		private short _retstockLocn = 0;
		public short ReturnStockLocation
		{
			get{return _retstockLocn;}
			set{_retstockLocn = value;}
		}
		private double _retvalue = 0;
		public double ReturnValue
		{
			get{return _retvalue;}
			set{_retvalue = value;}
		}
		private int _buffno = 0;
		public int BuffNo
		{
			get{return _buffno;}
			set{_buffno = value;}
		}
		private short _buffBranchNo = 0;
		public short BuffBranchNumber
		{
			get{return _buffBranchNo;}
			set{_buffBranchNo = value;}
		}
		private DateTime _datetrans = DateTime.Today;
		public DateTime DateTrans
		{
			get{return _datetrans;}
			set{_datetrans = value;}
		}
		private short _branchNo = 0;
		public short BranchNumber
		{
			get{return _branchNo;}
			set{_branchNo = value;}
		}
		private int _transref = 0;
		public int TransRefNo
		{
			get{return _transref;}
			set{_transref = value;}
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
		private double _deliveredQty = 0;
		public double DeliveredQuantity
		{
			get{return _deliveredQty;}
			set{_deliveredQty = value;}
		}

		private int _notifiedBy = 0;
		public int NotifiedBy
		{
			get{return _notifiedBy;}
			set{_notifiedBy = value;}
		}
		private string _ftnotes = "";
		public string ftNotes
		{
			get{return _ftnotes;}
			set{_ftnotes = value;}
		}
        private string _parentItemNo;
        public string ParentItemNo
        {
            get { return _parentItemNo; }
            set { _parentItemNo = value.Trim(); }
        }

        public int ParentItemId { get; set; }

		private DataTable _deliveries;
		public DataTable Deliveries
		{
			get	{return _deliveries;}
		}

        //IP - 08/05/12 - #9608 - CR8520
        public DataSet CashAndGoDels
        {
            get;
            set;
        }

        private DataSet _deliveriesGRT;
        public DataSet DeliveriesGRT
        {
            get { return _deliveriesGRT; }
        }

		public void GetDeliveredQuantity(SqlConnection conn, SqlTransaction trans, 
										string accountNo, int agreementNo, 
										int itemID, short location,
										string contractNo, int parentItemID)  //IP - 16/05/11 - CR1212 - #3627 - Use ItemID and ParentItemID
		{
			try
			{
				parmArray = new SqlParameter[7];
				parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@agreementNo", SqlDbType.Int);
				parmArray[1].Value = agreementNo;
				parmArray[2] = new SqlParameter("@itemID", SqlDbType.Int);
				parmArray[2].Value = itemID;
				parmArray[3] = new SqlParameter("@location", SqlDbType.SmallInt);
				parmArray[3].Value = location;
				parmArray[4] = new SqlParameter("@contractno", SqlDbType.NVarChar, 10);
				parmArray[4].Value = contractNo;
                parmArray[5] = new SqlParameter("@parentItemID", SqlDbType.Int);
                parmArray[5].Value = parentItemID;
				parmArray[6] = new SqlParameter("@delivered", SqlDbType.Float);
				parmArray[6].Value = 0;
				parmArray[6].Direction = ParameterDirection.Output;

				if(conn!=null && trans!=null)
					this.RunSP(conn, trans, "DN_DeliveryGetQuantitySP", parmArray);
				else
					this.RunSP("DN_DeliveryGetQuantitySP", parmArray);

				if(parmArray[6].Value != DBNull.Value)
					_deliveredQty = (double)parmArray[6].Value;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}


        public DataSet GetDeliveryOrders(DateTime fromDate,
                                            DateTime toDate,
                                            string deliveryArea,
                                            int includeDeliveries,
                                            int includeCollections,
                                            int includeAddresses,
                                            int includeLinkedItems,
                                            string deliveryProcess,
                                            string majorProductCategory,
                                            string minorProductCategory,
                                            string acctNo,
                                            int user,
                                            int branchNo,
											int delNotBranchNo,
											bool reqDelSearch,
											bool includeAssembly,
											bool includeNonAssembly,
                                            out DateTime timeLocked)
        {
            DataSet ordersSet = new DataSet();
            try
            {
                parmArray = new SqlParameter[18];
                parmArray[0] = new SqlParameter("@orderfrom", SqlDbType.DateTime);
                parmArray[0].Value = fromDate;
                parmArray[1] = new SqlParameter("@orderto", SqlDbType.DateTime);
                parmArray[1].Value = toDate;
                parmArray[2] = new SqlParameter("@deliveryarea", SqlDbType.NVarChar,6);
                parmArray[2].Value = deliveryArea;
                parmArray[3] = new SqlParameter("@includedeliveries", SqlDbType.Int);
                parmArray[3].Value = includeDeliveries;
                parmArray[4] = new SqlParameter("@includecollections", SqlDbType.Int);
                parmArray[4].Value = includeCollections;
                parmArray[5] = new SqlParameter("@includeaddresses", SqlDbType.Int);
                parmArray[5].Value = includeAddresses;
                parmArray[6] = new SqlParameter("@retrievelinkeditems", SqlDbType.Int);
                parmArray[6].Value = includeLinkedItems;
                parmArray[7] = new SqlParameter("@deliveryprocess", SqlDbType.NVarChar,1);
                parmArray[7].Value = deliveryProcess;
                parmArray[8] = new SqlParameter("@majorcategory", SqlDbType.NVarChar,12);
                parmArray[8].Value = majorProductCategory;
                parmArray[9] = new SqlParameter("@minorcategory", SqlDbType.NVarChar,12);
                parmArray[9].Value = minorProductCategory;
                parmArray[10] = new SqlParameter("@acctno", SqlDbType.NVarChar,13);
                parmArray[10].Value = acctNo;
                parmArray[11] = new SqlParameter("@user", SqlDbType.Int);
                parmArray[11].Value = user;
                parmArray[12] = new SqlParameter("@branch", SqlDbType.Int);
                parmArray[12].Value = branchNo;
				parmArray[13] = new SqlParameter("@delnotebranch", SqlDbType.Int);
				parmArray[13].Value = delNotBranchNo;
				parmArray[14] = new SqlParameter("@reqdelsearch", SqlDbType.Bit);
				parmArray[14].Value = reqDelSearch;
				parmArray[15] = new SqlParameter("@includeassembly", SqlDbType.Bit);
				parmArray[15].Value = includeAssembly;
				parmArray[16] = new SqlParameter("@includenonassembly", SqlDbType.Bit);
				parmArray[16].Value = includeNonAssembly;
				parmArray[17] = new SqlParameter("@TimeLocked", SqlDbType.DateTime);
                parmArray[17].Direction = ParameterDirection.Output;
                RunSP("DN_DeliveryOrdersLoadSP", parmArray, ordersSet);				
            }
            catch(SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            finally 
            {
                timeLocked = Convert.ToDateTime(parmArray[17].Value);
            }
                   
            return ordersSet;
        }


        public decimal GetDeliveryTotal(SqlConnection conn, SqlTransaction trans, string acctNo)  //IP - 28/06/11 - 5.13 - LW73619 - #3751 - changed return type
        {
            decimal delTotal = 0;       //IP - 28/06/11 - 5.13 - LW73619 - #3751 

            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 13);
                parmArray[0].Value = acctNo;
                parmArray[1] = new SqlParameter("@delivered", SqlDbType.Money);
                parmArray[1].Value = delTotal;
                parmArray[1].Direction = ParameterDirection.Output;
                
                if(conn!=null && trans!=null)
					this.RunSP(conn, trans, "DN_DeliveryGetTotalSP", parmArray);
                else
                    this.RunSP("DN_DeliveryGetTotalSP", parmArray);

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            finally
            {
                if(parmArray[1].Value != DBNull.Value)
                    delTotal = Convert.ToDecimal(parmArray[1].Value);       //IP - 28/06/11 - 5.13 - LW73619 - #3751 
            }

            return delTotal;
        }



        public DataSet GetDeliveryNotes(
            string acctNo,
            int user,
            bool collectionsOnly,
            out DateTime timeLocked)
        {
            DataSet ordersSet = new DataSet();
            try
            {
                parmArray = new SqlParameter[4];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,13);
                parmArray[0].Value = acctNo;
                parmArray[1] = new SqlParameter("@user", SqlDbType.Int);
                parmArray[1].Value = user;
                parmArray[2] = new SqlParameter("@collectionsOnly", SqlDbType.Bit);
                parmArray[2].Value = collectionsOnly;
                parmArray[3] = new SqlParameter("@TimeLocked", SqlDbType.DateTime);
                parmArray[3].Direction = ParameterDirection.Output;
                RunSP("DN_DeliveryNotesGetSP", parmArray, ordersSet);				
            }
            catch(SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            finally 
            {
                timeLocked = Convert.ToDateTime(parmArray[3].Value);
            }
                   
            return ordersSet;
        }

		public decimal GetDeliveredValue(SqlConnection conn, SqlTransaction trans, 
										string accountNo, int agreementNo, 
										int itemID, short location,
                                        string contractNo, int parentItemID)    //IP - 16/05/11 - CR1212 - #3627 - Changed to use ItemID and ParentItemID
		{
			decimal delValue = 0;
			try
			{
				parmArray = new SqlParameter[7];
				parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@agreementNo", SqlDbType.Int);
				parmArray[1].Value = agreementNo;
				parmArray[2] = new SqlParameter("@itemID", SqlDbType.Int);          //IP - 16/05/11 - CR1212 - #3627
				parmArray[2].Value = itemID;
				parmArray[3] = new SqlParameter("@location", SqlDbType.SmallInt);
				parmArray[3].Value = location;
				parmArray[4] = new SqlParameter("@contractNo", SqlDbType.NVarChar,10);
				parmArray[4].Value = contractNo;
                parmArray[5] = new SqlParameter("@parentItemID", SqlDbType.Int);     //IP - 16/05/11 - CR1212 - #3627
                parmArray[5].Value = parentItemID;
				parmArray[6] = new SqlParameter("@delivered", SqlDbType.Money);
				parmArray[6].Value = 0;
				parmArray[6].Direction = ParameterDirection.Output;

				this.RunSP(conn, trans, "DN_DeliveryGetValueSP", parmArray);

				if(parmArray[6].Value != DBNull.Value)
					delValue = (decimal)parmArray[6].Value;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return delValue;
		}

		public void Write(SqlConnection conn, SqlTransaction trans)
		{
			try
			{
				parmArray = new SqlParameter[22];
				parmArray[0] = new SqlParameter("@origbr", SqlDbType.SmallInt);
				parmArray[0].Value = this.OrigBr;
				parmArray[1] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
				parmArray[1].Value = this.AccountNumber;
				parmArray[2] = new SqlParameter("@agrmtno", SqlDbType.Int);
				parmArray[2].Value = this.AgreementNumber;
				parmArray[3] = new SqlParameter("@datedel", SqlDbType.DateTime);
				parmArray[3].Value = this.DateDelivered;
				parmArray[4] = new SqlParameter("@delorcol", SqlDbType.NChar,1);
				parmArray[4].Value = this.DeliveryOrCollection;
				parmArray[5] = new SqlParameter("@itemId", SqlDbType.Int);
				parmArray[5].Value = this.ItemId;
				parmArray[6] = new SqlParameter("@stocklocn", SqlDbType.SmallInt);
				parmArray[6].Value = this.StockLocation;
				parmArray[7] = new SqlParameter("@quantity", SqlDbType.Float);
				parmArray[7].Value = this.Quantity;
				parmArray[8] = new SqlParameter("@retitemId", SqlDbType.Int);
				parmArray[8].Value = this.ReturnItemId;
				parmArray[9] = new SqlParameter("@retstocklocn", SqlDbType.SmallInt);
				parmArray[9].Value = this.ReturnStockLocation;
				parmArray[10] = new SqlParameter("@retval", SqlDbType.Float);
				parmArray[10].Value = this.ReturnValue;
				parmArray[11] = new SqlParameter("@buffno", SqlDbType.Int);
				parmArray[11].Value = this.BuffNo;
				parmArray[12] = new SqlParameter("@buffbranchno", SqlDbType.SmallInt);
				parmArray[12].Value = this.BuffBranchNumber;
				parmArray[13] = new SqlParameter("@datetrans", SqlDbType.DateTime);
				parmArray[13].Value = this.DateTrans;
				parmArray[14] = new SqlParameter("@branchno", SqlDbType.SmallInt);
				parmArray[14].Value = this.BranchNumber;
				parmArray[15] = new SqlParameter("@transrefno", SqlDbType.Int);
				parmArray[15].Value = this.TransRefNo;
				parmArray[16] = new SqlParameter("@transvalue", SqlDbType.Money);
				parmArray[16].Value = this.TransValue;
				parmArray[17] = new SqlParameter("@runno", SqlDbType.Int);
				parmArray[17].Value = this.RunNumber;
				parmArray[18] = new SqlParameter("@contractno", SqlDbType.NVarChar,10);
				parmArray[18].Value = this.ContractNo;
				parmArray[19] = new SqlParameter("@notifiedby", SqlDbType.Int);
				parmArray[19].Value = this.NotifiedBy;
				parmArray[20] = new SqlParameter("@ftnotes", SqlDbType.VarChar,4);
				parmArray[20].Value = this.ftNotes;
                parmArray[21] = new SqlParameter("@ParentItemId", SqlDbType.Int);
                parmArray[21].Value = this.ParentItemId;
	
				this.RunSP(conn, trans, "DN_DeliveryWriteSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

        public void WriteCollectReason(SqlConnection conn, SqlTransaction trans)
        {
            try
            {                
                parmArray = new SqlParameter[6];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[0].Value = this.AccountNumber;
                //parmArray[1] = new SqlParameter("@itemno", SqlDbType.NVarChar, 8);
                //parmArray[1].Value = this.ItemNumber;
                parmArray[1] = new SqlParameter("@itemID", SqlDbType.Int);                  //IP - 25/05/11 - CR1212 - RI
                parmArray[1].Value = this.ItemId;
                parmArray[2] = new SqlParameter("@stocklocn", SqlDbType.SmallInt);
                parmArray[2].Value = this.StockLocation;
                parmArray[3] = new SqlParameter("@changedby", SqlDbType.Int);
                parmArray[3].Value = this.User;
                parmArray[4] = new SqlParameter("@collectreason", SqlDbType.NVarChar, 30);
                parmArray[4].Value = this.CollectReason;
                parmArray[5] = new SqlParameter("@collecttype", SqlDbType.NVarChar, 3);
                parmArray[5].Value = this.CollectType;


                RunSP(conn, trans, "DN_GoodsReturn_CollectReason", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void UpdateRepossessedStock(SqlConnection conn, SqlTransaction trans, int itemId, int stockLocation, decimal salePrice, decimal qty)
        {
            try
            {
                parmArray = new SqlParameter[4];

                parmArray[0] = new SqlParameter("@itemId", SqlDbType.Int);
                parmArray[0].Value = itemId;
                parmArray[1] = new SqlParameter("@stockLocn", SqlDbType.SmallInt);
                parmArray[1].Value = stockLocation;
                parmArray[2] = new SqlParameter("@salePrice", SqlDbType.Money);         // #4167 jec 06/07/11
                parmArray[2].Value = salePrice;
                parmArray[3] = new SqlParameter("@qty", SqlDbType.Float);
                parmArray[3].Value = qty;

                this.RunSP(conn, trans, "UpdateRepossessedStock", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Writes a record in the delivery table to collect the warranty on an item for which there has been an insurance claim
        /// </summary>
        public void WriteInsuranceReturn(SqlConnection conn, SqlTransaction trans, string acctNo, int agreementNo,
            int warrantyID, int stocklocn, int buffNo, string contractNo, int empeeno, string returnCode, int newBuffNo)            //IP - 08/06/11 - CR1212 - RI 
        {
            try
            {
                parmArray = new SqlParameter[9];

                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[0].Value = acctNo;

                parmArray[1] = new SqlParameter("@agreementno", SqlDbType.Int);
                parmArray[1].Value = agreementNo;

                //parmArray[2] = new SqlParameter("@itemno", SqlDbType.NVarChar, 10);
                //parmArray[2].Value = itemNo;

                parmArray[2] = new SqlParameter("@warrantyID", SqlDbType.Int);                                                      //IP - 08/06/11 - CR1212 - RI
                parmArray[2].Value = warrantyID;
                
                parmArray[3] = new SqlParameter("@stocklocn", SqlDbType.SmallInt);
                parmArray[3].Value = stocklocn;

                parmArray[4] = new SqlParameter("@buffno", SqlDbType.Int);
                parmArray[4].Value = buffNo;

                parmArray[5] = new SqlParameter("@contractno", SqlDbType.NVarChar, 10);
                parmArray[5].Value = contractNo;

                parmArray[6] = new SqlParameter("@empeeno", SqlDbType.Int);
                parmArray[6].Value = empeeno;

                parmArray[7] = new SqlParameter("@returnCode", SqlDbType.NVarChar, 10);
                parmArray[7].Value = returnCode;

                parmArray[8] = new SqlParameter("@newbuffno", SqlDbType.Int);
                parmArray[8].Value = newBuffNo;
                
                this.RunSP(conn, trans, "DN_InsuranceClaimWarrantyCollectSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }
		
        //public void GetForRepo(string accountNo, out string accountType)
        public DataSet GetForRepo(string accountNo, out string accountType)     // #14927 
		{
            //_deliveries = new DataTable();
            _deliveriesGRT = new DataSet();             // #14927 
			accountType = "";

			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@accountType", SqlDbType.NVarChar,1);
				parmArray[1].Value = "";
				parmArray[1].Direction = ParameterDirection.Output;

                //this.RunSP("DN_DeliveryGetForRepoSP", parmArray, _deliveries);
                this.RunSP("DN_DeliveryGetForRepoSP", parmArray, _deliveriesGRT);           // #14927 
				if(parmArray[1].Value != DBNull.Value)
					accountType = (string)parmArray[1].Value;

                return _deliveriesGRT;      // #14927 
            }

			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}
		
        public void GetDeliveries(SqlConnection conn, SqlTransaction trans)
		{
			_deliveries = new DataTable();

			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = this.AccountNumber;
				parmArray[1] = new SqlParameter("@agreementno", SqlDbType.Int);
				parmArray[1].Value = this.AgreementNumber;

                if (conn != null && trans != null)
                {
                    this.RunSP(conn, trans, "DN_DeliveryGetSP", parmArray, _deliveries);
                }
                else
                {
				this.RunSP("DN_DeliveryGetSP", parmArray, _deliveries);
			}
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void GetCashAndGoAccts(long BuffNo, int BranchNo, DateTime From, DateTime To,
			bool searchWarrantyReturns)
		{
            //_deliveries = new DataTable(); //IP - 08/05/12 - #9608 - CR8520
            CashAndGoDels = new DataSet(); //IP - 08/05/12 - #9608 - CR8520
			try
			{
				parmArray = new SqlParameter[5];
				parmArray[0] = new SqlParameter("@buffno", SqlDbType.BigInt);
				parmArray[0].Value = BuffNo;
				parmArray[1] = new SqlParameter("@branchno", SqlDbType.Int);
				parmArray[1].Value = BranchNo;
				parmArray[2] = new SqlParameter("@datefrom", SqlDbType.DateTime);
				parmArray[2].Value = From;
				parmArray[3] = new SqlParameter("@dateto", SqlDbType.DateTime);
				parmArray[3].Value = To;
				parmArray[4] = new SqlParameter("@searchWarrantyReturns", SqlDbType.SmallInt);
				parmArray[4].Value = Convert.ToInt16(searchWarrantyReturns);

                //this.RunSP("DN_DeliveryCashAndGoSearchSP", parmArray, _deliveries);
                this.RunSP("DN_DeliveryCashAndGoSearchSP", parmArray, CashAndGoDels);           //IP - 08/05/12 - #9608 - CR8520
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}
        /// <summary>
        /// loads deliveries using datatable TN.DeliveryLineItems
        /// </summary>
        /// <param name="acctNo"></param>
        /// <returns></returns>
		public int GetItemsForDebtCollector(string acctNo)
		{
			try
			{
				_deliveries = new DataTable(TN.DeliveryLineItems);
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = acctNo;

				result = this.RunSP("DN_DeliveryGetForDebtCollector", parmArray, _deliveries);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

		public decimal GetNonStockValue(SqlConnection conn, SqlTransaction trans, string accountNo)
		{
			decimal nonstockValue = 0;

			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@value", SqlDbType.Money);
				parmArray[1].Value = 0;
				parmArray[1].Direction = ParameterDirection.Output;				

				RunSP(conn, trans, "DN_DeliveryGetNonStockValueSP", parmArray);

				if(!Convert.IsDBNull(parmArray[1].Value))
					nonstockValue = (decimal)parmArray[1].Value;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return nonstockValue;
		}

		public void GetDeliveries(string accountNo)
		{
			_deliveries = new DataTable();

			try
			{
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = this.AccountNumber;

				RunSP("DN_GetDeliveriesSP", parmArray, _deliveries);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public DDelivery()
		{

		}

		/// <summary>
		/// GetCODCharges
		/// </summary>
		/// <param name="acctno">string</param>
		/// <param name="buffno">int</param>
		/// <param name="agrmtno">int</param>
		/// <param name="totalamountdue">double</param>
		/// <param name="nonstocktotal">double</param>
		/// <returns>void</returns>
		/// 
		public void GetCODCharges (SqlConnection conn, SqlTransaction trans, string acctno, int buffno, 
			int agrmtno, out decimal totalamountdue,out decimal nonstocktotal, out bool cod, 
			System.DateTime dateReqDel,string timeReqDel,string addtype, int locn)
		{
			totalamountdue = 0;
			nonstocktotal = 0;
			cod = false;
			
			try
			{
				parmArray = new SqlParameter[10];
				
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NChar, 24);
				parmArray[0].Value = acctno;
				
				parmArray[1] = new SqlParameter("@buffno", SqlDbType.Int);
				parmArray[1].Value = buffno;
				
				parmArray[2] = new SqlParameter("@agrmtno", SqlDbType.Int);
				parmArray[2].Value = agrmtno;
				
				parmArray[3] = new SqlParameter("@totalamountdue", SqlDbType.Money);
				parmArray[3].Value = totalamountdue;
				parmArray[3].Direction = ParameterDirection.Output;

				parmArray[4] = new SqlParameter("@nonstocktotal", SqlDbType.Money);
				parmArray[4].Value = nonstocktotal;
				parmArray[4].Direction = ParameterDirection.Output; 

				parmArray[5] = new SqlParameter("@cod", SqlDbType.SmallInt);
				parmArray[5].Value = 0;
				parmArray[5].Direction = ParameterDirection.Output; 
			
				parmArray[6] = new SqlParameter("@addtype", SqlDbType.NVarChar, 2);
				parmArray[6].Value = addtype;

				parmArray[7] = new SqlParameter("@dateReqDel", SqlDbType.DateTime);
				parmArray[7].Value = dateReqDel;

				parmArray[8] = new SqlParameter("@timeReqDel", SqlDbType.NVarChar, 10);
				parmArray[8].Value = timeReqDel;

				parmArray[9] = new SqlParameter("@locn", SqlDbType.Int);
				parmArray[9].Value = locn;
			
				if(conn!=null && trans!=null)
					this.RunSP(conn, trans, "DN_CODdeliveryNoteCalculationSP", parmArray);
				else
					this.RunSP("DN_CODdeliveryNoteCalculationSP", parmArray);
	
				if(parmArray[3].Value!=DBNull.Value)
					totalamountdue = (decimal)parmArray[3].Value;
				if(parmArray[4].Value!=DBNull.Value)
					nonstocktotal = (decimal)parmArray[4].Value;
				if(parmArray[5].Value!=DBNull.Value)
					cod = Convert.ToBoolean(parmArray[5].Value);
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void GetCODChargesForReprint(SqlConnection conn, SqlTransaction trans, string acctno, int buffno, int agrmtno, out decimal totalamountdue,
			out decimal nonstocktotal, out bool cod, System.DateTime dateReqDel,string timeReqDel,string addtype)
		{
			totalamountdue = 0;
			nonstocktotal = 0;
			cod = false;
			
			try
			{
				parmArray = new SqlParameter[9];
				
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NChar, 24);
				parmArray[0].Value = acctno;
				
				parmArray[1] = new SqlParameter("@buffno", SqlDbType.Int);
				parmArray[1].Value = buffno;
				
				parmArray[2] = new SqlParameter("@agrmtno", SqlDbType.Int);
				parmArray[2].Value = agrmtno;
				
				parmArray[3] = new SqlParameter("@totalamountdue", SqlDbType.Money);
				parmArray[3].Value = totalamountdue;
				parmArray[3].Direction = ParameterDirection.Output;

				parmArray[4] = new SqlParameter("@nonstocktotal", SqlDbType.Money);
				parmArray[4].Value = nonstocktotal;
				parmArray[4].Direction = ParameterDirection.Output; 

				parmArray[5] = new SqlParameter("@cod", SqlDbType.SmallInt);
				parmArray[5].Value = 0;
				parmArray[5].Direction = ParameterDirection.Output; 
			
				parmArray[6] = new SqlParameter("@addtype", SqlDbType.NVarChar, 2);
				parmArray[6].Value = addtype;

				parmArray[7] = new SqlParameter("@dateReqDel", SqlDbType.DateTime);
				parmArray[7].Value = dateReqDel;

				parmArray[8] = new SqlParameter("@timeReqDel", SqlDbType.NVarChar, 10);
				parmArray[8].Value = timeReqDel;
			
				if(conn!=null && trans!=null)
					this.RunSP(conn, trans, "DN_CODReprintCalculationSP", parmArray);
				else
					this.RunSP("DN_CODReprintCalculationSP", parmArray);
	
				if(parmArray[3].Value!=DBNull.Value)
					totalamountdue = (decimal)parmArray[3].Value;
				if(parmArray[4].Value!=DBNull.Value)
					nonstocktotal = (decimal)parmArray[4].Value;
				if(parmArray[5].Value!=DBNull.Value)
					cod = Convert.ToBoolean(parmArray[5].Value);
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void GetDeliveriesForAccount(SqlConnection conn, SqlTransaction trans, 
											string accountNo, int agreementNo)
		{
			_deliveries = new DataTable();

			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@agreementno", SqlDbType.Int);
				parmArray[1].Value = agreementNo;

				this.RunSP(conn, trans, "DN_DeliveryGetSP", parmArray, _deliveries);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void GetWarrantyReturnCode(SqlConnection conn, SqlTransaction trans,
										string acctno, short stocklocn, short replacement,
										//string parentItemNo, string WarrantyNo, string contractNo,
                                        int parentItemID, int warrantyItemID, string contractNo,                                                             //IP - 21/06/11 - CR1212 - RI - #3939           
                                        out string returnWarranty, out decimal refund, out DateTime deliveryDate, out int warrantyRetCodeItemID)             //IP - 13/09/11 - RI - #8112 - Added warrantyRetCodeItemID
               // out string returnWarranty, out decimal refund, out DateTime deliveryDate)             
		{
			try
			{
				returnWarranty = "";
				refund = 0;
                deliveryDate = DateTime.Now;
                warrantyRetCodeItemID = 0;                                                                     //IP - 13/09/11 - RI - #8112
				
				parmArray = new SqlParameter[10];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = acctno;
				parmArray[1] = new SqlParameter("@stocklocn", SqlDbType.SmallInt);
				parmArray[1].Value = stocklocn;
				parmArray[2] = new SqlParameter("@replacement", SqlDbType.SmallInt);
				parmArray[2].Value = replacement;
                //parmArray[3] = new SqlParameter("@parentitemno", SqlDbType.NVarChar, 10);
                //parmArray[3].Value = parentItemNo;
                parmArray[3] = new SqlParameter("@parentItemID", SqlDbType.Int);                                //IP - 21/06/11 - CR1212 - RI - #3939   
                parmArray[3].Value = parentItemID;
                //parmArray[4] = new SqlParameter("@warrantyno", SqlDbType.NVarChar, 10);
                //parmArray[4].Value = WarrantyNo;
                parmArray[4] = new SqlParameter("@warrantyItemID", SqlDbType.Int);                              //IP - 21/06/11 - CR1212 - RI - #3939  
                parmArray[4].Value = warrantyItemID;
				parmArray[5] = new SqlParameter("@contractno", SqlDbType.NVarChar, 12);
				parmArray[5].Value = contractNo;
				parmArray[6] = new SqlParameter("@returnwarranty", SqlDbType.NVarChar, 10);
				parmArray[6].Value = "";
				parmArray[6].Direction = ParameterDirection.Output;
				parmArray[7] = new SqlParameter("@refund", SqlDbType.Float);
				parmArray[7].Value = 0;
				parmArray[7].Direction = ParameterDirection.Output;
                parmArray[8] = new SqlParameter("@datedel", SqlDbType.DateTime);
                parmArray[8].Direction = ParameterDirection.Output;
                parmArray[9] = new SqlParameter("@warrantyRetCodeItemID", SqlDbType.Int);                       //IP - 13/09/11 - RI - #8112
                parmArray[9].Direction = ParameterDirection.Output;


				if(conn!=null && trans!=null)
					this.RunSP(conn, trans, "DN_GetWarrantyReturnCodes", parmArray);
				else
					this.RunSP("DN_GetWarrantyReturnCodes", parmArray);

				if(parmArray[6].Value != DBNull.Value)
					returnWarranty = (string)parmArray[6].Value;
				if(parmArray[7].Value != DBNull.Value)
					refund = Convert.ToDecimal(parmArray[7].Value);
                if (parmArray[8].Value != DBNull.Value)
                    deliveryDate = Convert.ToDateTime(parmArray[8].Value);
                if (parmArray[9].Value != DBNull.Value)
                    warrantyRetCodeItemID = Convert.ToInt32(parmArray[9].Value);                                //IP - 13/09/11 - RI - #8112
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public DataSet GetRepossessedItemDetails(string accountNo)          // #14927
		{
			//DataTable dt = null;
            DataSet ds = new DataSet();
			try
			{
				//dt = new DataTable(TN.Accounts);
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNo;

                RunSP("DN_GetRepossessedItemDetailsSP", parmArray, ds);     // #14927

                return ds;          // #14927
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
            
		}

		public void GetPickList(SqlConnection conn, SqlTransaction trans, int pickListNo, 
								short branchNo, bool isReprint, bool isAmendment, bool isOrderPicklist)
		{
			_deliveries = new DataTable();

			try
			{
				parmArray = new SqlParameter[4];
				parmArray[0] = new SqlParameter("@picklistno", SqlDbType.Int);
				parmArray[0].Value = pickListNo;
				parmArray[1] = new SqlParameter("@branch", SqlDbType.SmallInt);
				parmArray[1].Value = branchNo;
				parmArray[2] = new SqlParameter("@reprint", SqlDbType.Bit);
				parmArray[2].Value = isReprint;
				parmArray[3] = new SqlParameter("@amendment", SqlDbType.Bit);
				parmArray[3].Value = isAmendment;

				if(isOrderPicklist)
					this.RunSP(conn, trans, "DN_DeliveryGetPickListSP", parmArray, _deliveries);
				else
					this.RunSP(conn, trans, "DN_DeliveryGetTransportPickListSP", parmArray, _deliveries);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}
		public void ScheduleRedelRepo(SqlConnection conn, SqlTransaction trans,
                                short origbr, string acctno, DateTime datedelplan, char delorcoll,
                                int itemID, short stocklocn, short quantity, short retstocklocn,        //IP - 26/05/11 - CR1212 - RI - #3636
                                int retItemID, decimal retval, int buffbranchno, int buffno,
                                string delArea, int agrmtNo, string contractNo, int parentItemID, int lineItemId)   //IP - 15/06/12 - #10387 - added lineItemID
		{
			try
			{
				
				parmArray = new SqlParameter[18];
				parmArray[1] = new SqlParameter("@origbr", SqlDbType.SmallInt);
				parmArray[1].Value = origbr;
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = acctno;
				parmArray[2] = new SqlParameter("@datedelplan", SqlDbType.DateTime);
				parmArray[2].Value = datedelplan;
				parmArray[3] = new SqlParameter("@delorcoll", SqlDbType.Char);
				parmArray[3].Value = delorcoll;
                parmArray[4] = new SqlParameter("@itemID", SqlDbType.Int);               //IP - 26/05/11 - CR1212 - RI - #3636
				parmArray[4].Value = itemID;
				parmArray[5] = new SqlParameter("@stocklocn", SqlDbType.SmallInt);
				parmArray[5].Value = stocklocn;
				parmArray[6] = new SqlParameter("@quantity", SqlDbType.SmallInt);
				parmArray[6].Value = quantity;
				parmArray[7] = new SqlParameter("@retstocklocn", SqlDbType.SmallInt);
				parmArray[7].Value = retstocklocn;
                parmArray[8] = new SqlParameter("@retItemID", SqlDbType.Int);            //IP - 26/05/11 - CR1212 - RI - #3636
				parmArray[8].Value = retItemID;
				parmArray[9] = new SqlParameter("@retval", SqlDbType.Money);
				parmArray[9].Value = retval;
				parmArray[10] = new SqlParameter("@buffbranchno", SqlDbType.Int);
				parmArray[10].Value = buffbranchno;
				parmArray[11] = new SqlParameter("@buffno", SqlDbType.Int);
				parmArray[11].Value = buffno;
				parmArray[12] = new SqlParameter("@deliveryarea", SqlDbType.NVarChar, 8);
				parmArray[12].Value = delArea;
                parmArray[13] = new SqlParameter("@agrmtno", SqlDbType.Int);
                parmArray[13].Value = agrmtNo;
                parmArray[14] = new SqlParameter("@contractno", SqlDbType.NVarChar, 10);
                parmArray[14].Value = contractNo;
                parmArray[15] = new SqlParameter("@datereqdel", SqlDbType.DateTime);
                parmArray[15].Value = DateTime.Today;
                parmArray[16] = new SqlParameter("@parentItemID", SqlDbType.Int);       //IP - 26/05/11 - CR1212 - RI - #3636
                parmArray[16].Value = parentItemID;
                parmArray[17] = new SqlParameter("@lineItemId", SqlDbType.Int);         //IP - 15/06/12 - #10387 - Warehouse & Deliveries
                parmArray[17].Value = lineItemId;

				if(conn!=null && trans!=null)
					this.RunSP(conn, trans, "dn_ScheduleRedelRepoSP", parmArray);
				else
					this.RunSP("dn_ScheduleRedelRepoSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}
		public DataTable GetTransportList()
		{
			DataTable dt = null;
			try
			{
				dt = new DataTable(TN.Transport);

				RunSP("DN_GetTransportListSP", dt);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return dt;
		}
		public DataTable GetScheduledLoads(short branchNo,DateTime dateFrom, DateTime dateTo,
			                               short printed, short loadNo,short withSchedules)
		{
			DataTable dt = null;
			try
			{
				dt = new DataTable(TN.Schedules);
				parmArray = new SqlParameter[6];
				parmArray[0] = new SqlParameter("@branchno", SqlDbType.SmallInt);
				parmArray[0].Value = branchNo;
				parmArray[1] = new SqlParameter("@dateFrom", SqlDbType.DateTime);
				parmArray[1].Value = dateFrom;
				parmArray[2] = new SqlParameter("@dateTo", SqlDbType.DateTime);
				parmArray[2].Value = dateTo;
				parmArray[3] = new SqlParameter("@printed", SqlDbType.SmallInt);
				parmArray[3].Value = printed;
				parmArray[4] = new SqlParameter("@loadNo", SqlDbType.SmallInt);
				parmArray[4].Value = loadNo;
				parmArray[5] = new SqlParameter("@withschedules", SqlDbType.SmallInt);
				parmArray[5].Value = withSchedules;

				RunSP("DN_GetScheduledLoadsSP", parmArray, dt);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return dt;
		}
		public DataTable GetLoadContents(short loadNo, DateTime dateDel, short branchNo)
		{
			DataTable dt = null;
			try
			{
				dt = new DataTable(TN.Deliveries);
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@loadno", SqlDbType.SmallInt);
				parmArray[0].Value = loadNo;
				parmArray[1] = new SqlParameter("@datedel", SqlDbType.DateTime);
				parmArray[1].Value = dateDel;
				parmArray[2] = new SqlParameter("@branchno", SqlDbType.SmallInt);
				parmArray[2].Value = branchNo;

				RunSP("DN_GetLoadContentsSP", parmArray, dt);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return dt;
		}
		public int TransportSchedAdd(SqlConnection conn, SqlTransaction trans,
				short branchNo, DateTime dateDel, short loadNo, string TruckId, short printed)
		{
			int added = 0;
			try
			{
				parmArray = new SqlParameter[5];
				parmArray[0] = new SqlParameter("@branchno", SqlDbType.SmallInt);
				parmArray[0].Value = branchNo;
				parmArray[1] = new SqlParameter("@datedel", SqlDbType.DateTime);
				parmArray[1].Value = dateDel;
				parmArray[2] = new SqlParameter("@loadno", SqlDbType.SmallInt);
				parmArray[2].Value = loadNo;
				parmArray[3] = new SqlParameter("@truckid", SqlDbType.NVarChar, 26);
				parmArray[3].Value = TruckId;
				parmArray[4] = new SqlParameter("@printed", SqlDbType.SmallInt);
				parmArray[4].Value = printed;
				if(conn!=null && trans!=null)
					added = this.RunSP(conn, trans, "dn_TransportSchedAddsp", parmArray);
				else
					added = this.RunSP("dn_TransportSchedAddsp", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return added;
		}
		public int DeliveryScheduleUpdate(SqlConnection conn, SqlTransaction trans,
			int loadNo, int buffNo, int filter, int branchNo, int pickListNo, DateTime dateDel)
		{
			//DataTable dt = null;
			try
			{
				parmArray = new SqlParameter[6];
				parmArray[0] = new SqlParameter("@loadno", SqlDbType.Int);
				parmArray[0].Value = loadNo;
				parmArray[1] = new SqlParameter("@buffno", SqlDbType.Int);
				parmArray[1].Value = buffNo;
				parmArray[2] = new SqlParameter("@filter", SqlDbType.Int);
				parmArray[2].Value = filter;
				parmArray[3] = new SqlParameter("@branchno", SqlDbType.Int);
				parmArray[3].Value = branchNo;
				parmArray[4] = new SqlParameter("@picklistnumber", SqlDbType.Int);
				parmArray[4].Value = pickListNo;
				parmArray[5] = new SqlParameter("@datedel", SqlDbType.DateTime);
				parmArray[5].Value = dateDel;


				if(conn!=null && trans!=null)
					this.RunSP(conn, trans, "dn_DeliveryScheduleUpdateSP", parmArray);
				else
					this.RunSP("dn_DeliveryScheduleUpdateSP", parmArray);	
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return 0;
		}

		public DataTable LoadAvailablePicklists(short branchNo, char type)
		{
			DataTable dt = null;
			try
			{
				dt = new DataTable(TN.Deliveries);
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@branchno", SqlDbType.SmallInt);
				parmArray[0].Value = branchNo;
				parmArray[1] = new SqlParameter("@type", SqlDbType.Char);
				parmArray[1].Value = type;

				RunSP("[DN_LoadAvailablePicklistsSP]", parmArray, dt);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return dt;
		}

		public DataTable GetDeliveryScheduleDetails(short branchNo, DateTime dateDel, short loadNo)
		{
			DataTable dt = null;
			try
			{
				dt = new DataTable(TN.Deliveries);
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@branchno", SqlDbType.SmallInt);
				parmArray[0].Value = branchNo;
				parmArray[1] = new SqlParameter("@dateDel", SqlDbType.DateTime);
				parmArray[1].Value = dateDel;
				parmArray[2] = new SqlParameter("@loadNo", SqlDbType.SmallInt);
				parmArray[2].Value = loadNo;


				RunSP("[DN_GetDeliveryScheduleprintDetailsSP]", parmArray, dt);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return dt;
		}

		public DataTable GetDeliveryScheduleCustomerDetails(SqlConnection conn, SqlTransaction trans,
													 short loadNo, short branchNo, DateTime dateDel)
		{
			DataTable dt = null;
			try
			{
				dt = new DataTable(TN.Deliveries);
				
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@loadno", SqlDbType.SmallInt);
				parmArray[0].Value = loadNo;
				parmArray[1] = new SqlParameter("@branchno", SqlDbType.SmallInt);
				parmArray[1].Value = branchNo;
				parmArray[2] = new SqlParameter("@datedel", SqlDbType.DateTime);
				parmArray[2].Value = dateDel;

				if(conn!=null && trans!=null)
					RunSP(conn, trans, "DN_DeliveryScheduleGetCustomerSP", parmArray, dt);
				else
					RunSP("DN_DeliveryScheduleGetCustomerSP", parmArray, dt);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return dt;
		}

		public void SetPickListPrinted(SqlConnection conn, SqlTransaction trans, 
										int pickListNo, bool isOrderPicklist)
		{
			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@picklistno", SqlDbType.Int);
				parmArray[0].Value = pickListNo;
				parmArray[1] = new SqlParameter("@type", SqlDbType.Bit);
				parmArray[1].Value = isOrderPicklist;

				this.RunSP(conn, trans, "DN_DeliverySetPickListPrintedSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void AuditDeliveryReprint(SqlConnection conn, SqlTransaction trans, 
			string accountNo, int agreementNo, int itemId,
			short stockLocn, int buffNo, int printedBy)
		{
			try
			{
				parmArray = new SqlParameter[6];
				parmArray[0] = new SqlParameter("@accountno", SqlDbType.Char,12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@agrmtno", SqlDbType.Int);
				parmArray[1].Value = agreementNo;
                parmArray[2] = new SqlParameter("@itemId", SqlDbType.Int);
				parmArray[2].Value = itemId;
				parmArray[3] = new SqlParameter("@stocklocn", SqlDbType.SmallInt);
				parmArray[3].Value = stockLocn;
				parmArray[4] = new SqlParameter("@buffNo", SqlDbType.Int);
				parmArray[4].Value = buffNo;
				parmArray[5] = new SqlParameter("@printedBy", SqlDbType.Int);
				parmArray[5].Value = printedBy;

				RunSP(conn, trans, "DN_AuditDeliveryReprintSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void SUCBGetDelTotals(int runno,SqlConnection conn,SqlTransaction trans,	out decimal delTotal)
		{
			_deliveries = new DataTable(TN.Deliveries);
			delTotal = 0;

			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@runno", SqlDbType.Int);
				parmArray[0].Value = runno;
				parmArray[1] = new SqlParameter("@deltotal", SqlDbType.Money);
				parmArray[1].Value = 0;
				parmArray[1].Direction = ParameterDirection.Output;

				this.RunSP(conn,trans,"DN_SUCBGetDelTotalsSP", parmArray, _deliveries);
				if(parmArray[1].Value != DBNull.Value)
					delTotal = (decimal)parmArray[1].Value;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

        //public void SUCBGetDelDetails(int runno, string branchNo)
        public void SUCBGetDelDetails(string datetrans, string branchNo)                          //IP - 20/02/12 - #9423 - CR8262
		{
			_deliveries = new DataTable(TN.Deliveries);

			try
			{
				parmArray = new SqlParameter[2];
                //parmArray[0] = new SqlParameter("@runno", SqlDbType.Int);
                //parmArray[0].Value = runno;
                parmArray[0] = new SqlParameter("@datetrans", SqlDbType.VarChar,20);                  //IP - 20/02/12 - #9423 - CR8262
                parmArray[0].Value = datetrans;
				parmArray[1] = new SqlParameter("@branch", SqlDbType.NVarChar, 5);
				parmArray[1].Value = branchNo;

				this.RunSP("DN_SUCBGetDelDetailsSP", parmArray, _deliveries);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

        public void WarrantyFulFilled(SqlConnection conn, SqlTransaction trans,
                    int buffNo, string acctNo, int agrmtNo, int itemId, short locn, bool exchange,      // RI
            int quantityReturned, string collectionType, string warrantyFullFilled)           //#17678
        {
            try
            {
                parmArray = new SqlParameter[10];
                parmArray[0] = new SqlParameter("@buffno", SqlDbType.Int);
                parmArray[0].Value = buffNo;
                parmArray[1] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[1].Value = acctNo;
                parmArray[2] = new SqlParameter("@agrmtno", SqlDbType.Int);
                parmArray[2].Value = agrmtNo;
                //parmArray[3] = new SqlParameter("@itemno", SqlDbType.NVarChar, 8);
                //parmArray[3].Value = itemNo;
                parmArray[3] = new SqlParameter("@itemid", SqlDbType.Int);          // RI
                parmArray[3].Value = itemId;                            // RI
                parmArray[4] = new SqlParameter("@stocklocn", SqlDbType.SmallInt);
                parmArray[4].Value = locn;
                parmArray[5] = new SqlParameter("@empeeno", SqlDbType.Int);
                parmArray[5].Value = this.User;
                parmArray[6] = new SqlParameter("@exchange", SqlDbType.Bit);
                parmArray[6].Value = exchange;
                parmArray[7] = new SqlParameter("@quantityReturned", SqlDbType.Int);
                parmArray[7].Value = quantityReturned;
                parmArray[8] = new SqlParameter("@collectionType", SqlDbType.Char);   //#17678
                parmArray[8].Value = collectionType;
                parmArray[9] = new SqlParameter("@warrantyFullFilled", SqlDbType.Char);   //#17678
                parmArray[9].Value = warrantyFullFilled;

                this.RunSP(conn, trans, "DN_LineItemWarrantyFulFilledSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }
        
        public void GetExchangeDetails(string acctNo, int agrmtNo)
		{
			_deliveries = new DataTable(TN.Warranties);

			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
				parmArray[0].Value = acctNo;
				parmArray[1] = new SqlParameter("@agrmtno", SqlDbType.Int);
				parmArray[1].Value = agrmtNo;

				this.RunSP("DN_ExchangeGetSP", parmArray, _deliveries);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public bool IsDotNetWarehouse(short branchNo)
		{
			int isDotNetWarehouse = 0;
			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@branchno", SqlDbType.SmallInt);
				parmArray[0].Value = branchNo;
				parmArray[1] = new SqlParameter("@isdotnetwarehouse", SqlDbType.Int);
				parmArray[1].Value = 0;
				parmArray[1].Direction = ParameterDirection.Output;

				RunSP("DN_DeliveryIsDotNetWarehouseSP", parmArray);

				if(!Convert.IsDBNull(parmArray[1].Value))
					isDotNetWarehouse = (int)parmArray[1].Value;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return (isDotNetWarehouse > 0);
		}

        //IP - 12/04/10 - UAT(66) UAT5.2
        public bool IsThirdPartyWarehouse(short branchNo)
        {
            int isThirdPartyWarehouse = 0;
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@branchno", SqlDbType.SmallInt);
                parmArray[0].Value = branchNo;
                parmArray[1] = new SqlParameter("@isthirdpartywarehouse", SqlDbType.Int);
                parmArray[1].Value = 0;
                parmArray[1].Direction = ParameterDirection.Output;

                RunSP("IsThirdPartyWarehouseSP", parmArray);

                if (!Convert.IsDBNull(parmArray[1].Value))
                    isThirdPartyWarehouse = (int)parmArray[1].Value;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return (isThirdPartyWarehouse > 0);
        }

		public void GetDeliverySchedules(DateTime fromDate,
							DateTime toDate,
							string deliveryArea,
							int includeDeliveries,
							int includeCollections,
							string majorProductCategory,
							string minorProductCategory,
							string acctNo,
							int user,
							int branchNo,
							int delNotBranchNo,
							string truckID,
							bool includeAssembly,
							bool includeNonAssembly,
							out DateTime timeLocked)
		{
			try
			{
				_deliveries = new DataTable(TN.Deliveries);

				parmArray = new SqlParameter[15];
				parmArray[0] = new SqlParameter("@orderfrom", SqlDbType.DateTime);
				parmArray[0].Value = fromDate;
				parmArray[1] = new SqlParameter("@orderto", SqlDbType.DateTime);
				parmArray[1].Value = toDate;
				parmArray[2] = new SqlParameter("@deliveryarea", SqlDbType.NVarChar,6);
				parmArray[2].Value = deliveryArea;
				parmArray[3] = new SqlParameter("@includedeliveries", SqlDbType.Int);
				parmArray[3].Value = includeDeliveries;
				parmArray[4] = new SqlParameter("@includecollections", SqlDbType.Int);
				parmArray[4].Value = includeCollections;
				parmArray[5] = new SqlParameter("@majorcategory", SqlDbType.NVarChar,12);
				parmArray[5].Value = majorProductCategory;
				parmArray[6] = new SqlParameter("@minorcategory", SqlDbType.NVarChar,12);
				parmArray[6].Value = minorProductCategory;
				parmArray[7] = new SqlParameter("@acctno", SqlDbType.NVarChar,13);
				parmArray[7].Value = acctNo;
				parmArray[8] = new SqlParameter("@user", SqlDbType.Int);
				parmArray[8].Value = user;
				parmArray[9] = new SqlParameter("@branch", SqlDbType.Int);
				parmArray[9].Value = branchNo;
				parmArray[10] = new SqlParameter("@delnotebranch", SqlDbType.Int);
				parmArray[10].Value = delNotBranchNo;
				parmArray[11] = new SqlParameter("@truckid", SqlDbType.NVarChar,26);
				parmArray[11].Value = truckID;
				parmArray[12] = new SqlParameter("@includeassembly", SqlDbType.Bit);
				parmArray[12].Value = includeAssembly;
				parmArray[13] = new SqlParameter("@includenonassembly", SqlDbType.Bit);
				parmArray[13].Value = includeNonAssembly;
				parmArray[14] = new SqlParameter("@TimeLocked", SqlDbType.DateTime);
				parmArray[14].Direction = ParameterDirection.Output;
				
				RunSP("DN_DeliveryScheduledOrdersLoadSP", parmArray, _deliveries);				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			finally 
			{
				timeLocked = Convert.ToDateTime(parmArray[14].Value);
			}
		}

		public DataTable LoadAvailableTransPicklists(short branchNo)
		{
			DataTable dt = null;
			try
			{
				dt = new DataTable(TN.Deliveries);
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@branchno", SqlDbType.SmallInt);
				parmArray[0].Value = branchNo;

				RunSP("[DN_LoadAvailableTransportPicklistsSP]", parmArray, dt);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return dt;
		}

        public void GetIRItems(string acctNo, string custID, int buffNo, 
                                DateTime dateFrom, DateTime dateTo, string acctType)
        {
            _deliveries = new DataTable();

            try
            {
                parmArray = new SqlParameter[6];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[0].Value = acctNo;
                parmArray[1] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
                parmArray[1].Value = custID;
                parmArray[2] = new SqlParameter("@buffno", SqlDbType.Int);
                parmArray[2].Value = buffNo;
                parmArray[3] = new SqlParameter("@datefrom", SqlDbType.DateTime);
                parmArray[3].Value = dateFrom;
                parmArray[4] = new SqlParameter("@dateto", SqlDbType.DateTime);
                parmArray[4].Value = dateTo;
                parmArray[5] = new SqlParameter("@type", SqlDbType.NChar, 1);
                parmArray[5].Value = acctType;

                this.RunSP("DN_DeliveryIRSearchSP", parmArray, _deliveries);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void UpdateDeliveryDate(SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                parmArray = new SqlParameter[6];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[0].Value = this.AccountNumber;
                parmArray[1] = new SqlParameter("@agreementno", SqlDbType.Int);
                parmArray[1].Value = this.AgreementNumber;
                parmArray[2] = new SqlParameter("@locn", SqlDbType.SmallInt);
                parmArray[2].Value = this.StockLocation;
                parmArray[3] = new SqlParameter("@itemId", SqlDbType.Int);
                parmArray[3].Value = this.ItemId;
                parmArray[4] = new SqlParameter("@contractno", SqlDbType.NVarChar, 10);
                parmArray[4].Value = this.ContractNo;
                parmArray[5] = new SqlParameter("@deldate", SqlDbType.DateTime);
                parmArray[5].Value = DateTime.Now;

                this.RunSP(conn, trans, "DN_DeliveryUpdateDelDateSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void GetExchangeWarranty(SqlConnection conn, SqlTransaction trans, string acctNo, int agrmtNo)
        {
            _deliveries = new DataTable(TN.Warranties);

            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[0].Value = acctNo;
                parmArray[1] = new SqlParameter("@agrmtno", SqlDbType.Int);
                parmArray[1].Value = agrmtNo;

                this.RunSP("DN_ExchangeGetWarrantySP", parmArray, _deliveries);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

      public int CheckForSD(SqlConnection conn, SqlTransaction trans, string acctNo)
      {
         int n = 0;
         try
         {
            parmArray = new SqlParameter[1];
            parmArray[0] = new SqlParameter("@acctno", SqlDbType.Char, 12);
            parmArray[0].Value = acctNo;

            n = this.RunSPdrInt(conn, trans, "DeliveryCheckForSD_SP", parmArray);
         }
         catch (SqlException ex)
         {
            LogSqlException(ex);
            throw ex;
         }
         return n;
      }
	}
}
