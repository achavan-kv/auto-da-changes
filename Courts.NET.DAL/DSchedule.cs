using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common.Constants.Enums;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;

namespace STL.DAL
{
	/// <summary>
	/// Summary description for DSchedule.
	/// </summary>
	public class DSchedule : DALObject
	{
		private double _scheduledQty = 0;
		public double ScheduledQuantity
		{
			get{return _scheduledQty;}
			set{_scheduledQty = value;}
		}
		private DataTable _schedules = null;
		public DataTable Schedules
		{
			get{return _schedules;}
		}

        private DataTable _picklist = null;
        public DataTable Picklist
        {
            get {return _picklist;}
        }

		private short _origbr = 0;
		public short OrigBr
		{
			get{return _origbr;}
			set{_origbr = value;}
		}

		private string _acctno = "";
		public string AccountNumber
		{
			get{return _acctno;}
			set{_acctno = value;}
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

		private int _agrmtno = 0;
		public int AgreementNumber 
		{
			get{return _agrmtno;}
			set{_agrmtno = value;}
		}

		private DateTime _datedelplan = DateTime.Today;
		public DateTime DateDelPlan
		{
			get{return _datedelplan;}
			set{_datedelplan = value;}
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

        //IP - 17/05/11 - CR1212 - #3627
        private int _itemID = 0;
        public int ItemID
        {
            get { return _itemID; }
            set { _itemID = value; }
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
		
		private short _retstocklocn = 0;
		public short ReturnStockLocation
		{
			get{return _retstocklocn;}
			set{_retstocklocn = value;}
		}
		
		private string _retitemno = "";
		public string ReturnItemNumber
		{
			get	{return _retitemno;}
			set{_retitemno = value;}
		}
        //RI jec 20/05/11
        private int _retitemID = 0;
        public int RetItemID
        {
            get { return _retitemID; }
            set { _retitemID = value; }
        }
		
		private double _retval = 0;
		public double RetVal
		{
			get	{return _retval;}
			set{_retval = value;}
		}
		
		private string _vanno = "";
		public string VanNo
		{
			get	{return _vanno;}
			set{_vanno = value;}
		}
		
		private int _buffbranchno = 0;
		public int BuffBranchNo
		{
			get	{return _buffbranchno;}
			set{_buffbranchno = value;}
		}
		
		private int _buffno = 0;
		public int BuffNo
		{
			get	{return _buffno;}
			set{_buffno = value;}
		}
		
		private int _loadno = 0;
		public int LoadNo
		{
			get	{return _loadno;}
			set{_loadno = value;}
		}

        private int _picklistnumber = 0;
        public int PicklistNumber
        {
            get {return _picklistnumber;}
            set {_picklistnumber = value;}
        }
		
		private int _picklistbranchnumber = 0;
		public int PicklistBranchNumber
		{
			get {return _picklistbranchnumber;}
			set {_picklistbranchnumber = value;}
		}
		
		private string _contractno = "";
		public string ContractNo
		{
			get {return _contractno;}
			set {_contractno = value;}
		}

        private string _undeliveredflag = "";
        public string UndeliveredFlag
        {
            get { return _undeliveredflag; }
            set { _undeliveredflag = value; }
        }

        //IP - 10/02/10 - CR1048 (Ref:3.1.4 & 3.1.5) Merged - Malaysia Enhancements (CR1072)
        private int _createdby = 0;
        public int CreatedBy
        {
            get { return _createdby; }
            set { _createdby = value; }
        }

        //IP - 10/02/10 - CR1048 (Ref:3.1.4 & 3.1.5) Merged - Malaysia Enhancements (CR1072)
        private DateTime _createddate = DateTime.Today;
        public DateTime DateCreated
        {
            get { return _createddate; }
            set { _createddate = value; }
        }

        //IP - 10/02/10 - CR1048 (Ref:3.1.4 & 3.1.5) Merged - Malaysia Enhancements (CR1072)
        private string _grtNotes = "";      
        public string GRTnotes
        {
            get { return _grtNotes; }
            set { _grtNotes = value; }
        }

        public int ParentItemID { get; set; }
		
		public void GetScheduledQuantity(string accountNo, int agreementNo, int itemID, short location)     //IP - 17/05/11 - CR1212 - #3627 - changed to use itemID rather than itemNo
		{
			try
			{
				parmArray = new SqlParameter[5];
				parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@agreementNo", SqlDbType.Int);
				parmArray[1].Value = agreementNo;
				parmArray[2] = new SqlParameter("@itemID", SqlDbType.Int);                           //IP - 17/05/11 - CR1212 - #3627
				parmArray[2].Value = itemID;
				parmArray[3] = new SqlParameter("@location", SqlDbType.SmallInt);
				parmArray[3].Value = location;
				parmArray[4] = new SqlParameter("@scheduled", SqlDbType.Float);
				parmArray[4].Value = 0;
				parmArray[4].Direction = ParameterDirection.Output;

				this.RunSP("DN_ScheduleGetQuantitySP", parmArray);

				if(parmArray[4].Value != DBNull.Value)
					_scheduledQty = (double)parmArray[4].Value;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

        public void GetNextPicklistNo(int branchNo, int user, string pickListType, 
										out int pickListNo)
        {
            try
            {
                pickListNo = 0;

				parmArray = new SqlParameter[4];
                parmArray[0] = new SqlParameter("@branchno", SqlDbType.Int);
                parmArray[0].Value = branchNo;
                parmArray[1] = new SqlParameter("@user", SqlDbType.Int);
                parmArray[1].Value = user;
				parmArray[2] = new SqlParameter("@picklisttype", SqlDbType.NChar, 1);
				parmArray[2].Value = pickListType;
				parmArray[3] = new SqlParameter("@picklistnumber", SqlDbType.Int);
				parmArray[3].Direction = ParameterDirection.Output;

                this.RunSP("DN_GetNextPicklistNoSP", parmArray);

				if(parmArray[3].Value != DBNull.Value)
					pickListNo = (int)parmArray[3].Value;

            }
            catch(SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void UpdateScheduleForPicklist(SqlConnection conn, SqlTransaction trans,
												string pickListType)
        {
            try
            {
                parmArray = new SqlParameter[7];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NChar, 12);
                parmArray[0].Value = this.AccountNumber;
                parmArray[1] = new SqlParameter("@stocklocn", SqlDbType.SmallInt);
                parmArray[1].Value = this.StockLocation;
                parmArray[2] = new SqlParameter("@buffno", SqlDbType.Int);
                parmArray[2].Value = this.BuffNo;
                parmArray[3] = new SqlParameter("@itemId", SqlDbType.Int);
                parmArray[3].Value = this.ItemID;
                parmArray[4] = new SqlParameter("@Picklistnumber", SqlDbType.Int);
                parmArray[4].Value = this.PicklistNumber;
				parmArray[5] = new SqlParameter("@picklistbranchnumber", SqlDbType.Int);
				parmArray[5].Value = this.PicklistBranchNumber;
				parmArray[6] = new SqlParameter("@type", SqlDbType.NChar, 1);
				parmArray[6].Value = pickListType;

                this.RunSP(conn, trans, "DN_ScheduleUpdatePicklistsSP", parmArray);
            }
            catch(SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }
		public void ScheduleAssignNewBufferNo(SqlConnection conn, SqlTransaction trans, int newBuffNo)
		{
			try
			{
				parmArray = new SqlParameter[5];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NChar, 12);
				parmArray[0].Value = this.AccountNumber;
				parmArray[1] = new SqlParameter("@stocklocn", SqlDbType.SmallInt);
				parmArray[1].Value = this.StockLocation;
				parmArray[2] = new SqlParameter("@buffno", SqlDbType.Int);
				parmArray[2].Value = this.BuffNo;
				//parmArray[3].Direction = ParameterDirection.Output;
				parmArray[3] = new SqlParameter("@itemId", SqlDbType.Int);
				parmArray[3].Value = this.ItemID;
				parmArray[4] = new SqlParameter("@newBuffNo", SqlDbType.Int);
				parmArray[4].Value = newBuffNo;

				this.RunSP(conn, trans, "DN_ScheduleAssignNewBufferNoSP", parmArray);
				//if(parmArray[3].Value != DBNull.Value)
				//	_buffno = (int)parmArray[3].Value;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

        public void GetScheduledDeliveriesForItem(string accountNo, int agreementNo, int itemID, short location)     //IP/NM - 18/05/11 -CR1212 - #3627 
		{
			try
			{
				_schedules = new DataTable("Schedules");
				parmArray = new SqlParameter[4];
				parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@agreementNo", SqlDbType.Int);
				parmArray[1].Value = agreementNo;
                parmArray[2] = new SqlParameter("@itemID", SqlDbType.Int);                  //IP/NM - 18/05/11 -CR1212 - #3627 
				parmArray[2].Value = itemID;
				parmArray[3] = new SqlParameter("@location", SqlDbType.SmallInt);
				parmArray[3].Value = location;

				this.RunSP("DN_ScheduleGetForItemSP", parmArray, _schedules);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void GetByBuffNo(int BranchNo, int BuffNo)
		{
			try
			{
				_schedules = new DataTable("Schedules");
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@piBranchNo", SqlDbType.Int);
				parmArray[0].Value = BranchNo;
				parmArray[1] = new SqlParameter("@piBuffNo", SqlDbType.Int);
				parmArray[1].Value = BuffNo;

				this.RunSP("DN_Schedule_GetByBuffNo", parmArray, _schedules);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void DeleteDeliverySchedule(SqlConnection conn, SqlTransaction trans, 
			string accountNo, int agreementNo, 
			int itemID, short location,                                         //IP - 07/06/11 - CR1212 - RI
			short buffBranch, int buffNo, double quantity, double qtyRemoved) //IP/JC - 03/03/10 - CR1072 - Malaysia 3PL 
		{
			try
			{
				parmArray = new SqlParameter[9];
				parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@agreementNo", SqlDbType.Int);
				parmArray[1].Value = agreementNo;
                //parmArray[2] = new SqlParameter("@itemNo", SqlDbType.NVarChar,8);
                //parmArray[2].Value = itemNo;
                parmArray[2] = new SqlParameter("@itemID", SqlDbType.Int);          //IP - 07/06/11 - CR1212 - RI
                parmArray[2].Value = itemID;
				parmArray[3] = new SqlParameter("@location", SqlDbType.SmallInt);
				parmArray[3].Value = location;
				parmArray[4] = new SqlParameter("@buffBranch", SqlDbType.SmallInt);
				parmArray[4].Value = buffBranch;
				parmArray[5] = new SqlParameter("@buffNo", SqlDbType.Int);
				parmArray[5].Value = buffNo;
				parmArray[6] = new SqlParameter("@quantity", SqlDbType.Float);
				parmArray[6].Value = quantity;
				parmArray[7] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[7].Value = this.User;
                parmArray[8] = new SqlParameter("@qtyRemoved", SqlDbType.Float); //IP/JC - 03/03/10 - CR1072 - Malaysia 3PL 
                parmArray[8].Value = qtyRemoved;

				this.RunSP(conn, trans, "DN_ScheduleDeleteDeliverySP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

        public void DeleteSchedule(SqlConnection conn, SqlTransaction trans, bool replacement)
		{
			try
			{
				parmArray = new SqlParameter[7];
				parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar,12);
				parmArray[0].Value = this.AccountNumber;
				parmArray[1] = new SqlParameter("@agreementNo", SqlDbType.Int);
				parmArray[1].Value = this.AgreementNumber;
				parmArray[2] = new SqlParameter("@itemID", SqlDbType.Int);                      //IP - 17/05/11 - CR1212 - #3627 - Changed to use @itemID rather than @itemNo
				parmArray[2].Value = this.ItemID;
				parmArray[3] = new SqlParameter("@location", SqlDbType.SmallInt);
				parmArray[3].Value = this.StockLocation;
				parmArray[4] = new SqlParameter("@buffBranch", SqlDbType.SmallInt);
				parmArray[4].Value = this.BuffBranchNo;
				parmArray[5] = new SqlParameter("@buffNo", SqlDbType.Int);
				parmArray[5].Value = this.BuffNo;
                parmArray[6] = new SqlParameter("@replacement", SqlDbType.Int);
                parmArray[6].Value = replacement;


				this.RunSP(conn, trans, "DN_ScheduleDeleteSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public bool GetScheduleItem(SqlConnection conn, SqlTransaction trans,
			short branchNo, int buffNo,	string acctNo, int agrmtNo,	int itemID, short stockLocn)    //IP - 07/06/11 - CR1212 - RI
		{
			bool loaded = false;
			DataTable scheduleItem = new DataTable();
			try
			{
				parmArray = new SqlParameter[6];
				parmArray[0] = new SqlParameter("@buffbranchno", SqlDbType.SmallInt);
				parmArray[0].Value = branchNo;
				parmArray[1] = new SqlParameter("@buffno", SqlDbType.Int);
				parmArray[1].Value = buffNo;
				parmArray[2] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
				parmArray[2].Value = acctNo;
				parmArray[3] = new SqlParameter("@agrmtno", SqlDbType.Int);
				parmArray[3].Value = agrmtNo;
                //parmArray[4] = new SqlParameter("@itemno", SqlDbType.NVarChar, 8);
                //parmArray[4].Value = itemNo;
				parmArray[4] = new SqlParameter("@itemID", SqlDbType.Int);                      //IP - 07/06/11 - CR1212 - RI
                parmArray[4].Value = itemID;
				parmArray[5] = new SqlParameter("@stocklocn", SqlDbType.SmallInt);
				parmArray[5].Value = stockLocn;

				loaded = Convert.ToBoolean(RunSP(conn, trans, "DN_ScheduleGetItemSP", parmArray, scheduleItem));

				if (scheduleItem.Rows.Count > 0)
				{
					this._buffbranchno = (short)scheduleItem.Rows[0][CN.BuffBranchNo];
					this._buffno = (int)scheduleItem.Rows[0][CN.BuffNo];
					this._acctno = (string)scheduleItem.Rows[0][CN.AccountNumber];
					this._agrmtno = (int)scheduleItem.Rows[0][CN.AgrmtNo];
					this._itemno = (string)scheduleItem.Rows[0][CN.ItemNo];
                    this.ItemID = Convert.ToInt32(scheduleItem.Rows[0][CN.ItemId]);             //IP - 07/06/11 - CR1212 - RI
					this._stocklocn = (short)scheduleItem.Rows[0][CN.StockLocn];

					if (scheduleItem.Rows[0][CN.OrigBR] != DBNull.Value)
						this._origbr = (short)scheduleItem.Rows[0][CN.OrigBR];
					else
						this._origbr = 0;

					this._datedelplan = (DateTime)scheduleItem.Rows[0][CN.DateDelPlan];
					this._delorcoll = (string)scheduleItem.Rows[0][CN.DelOrColl];
					this._quantity = Convert.ToDouble(scheduleItem.Rows[0][CN.Quantity]);

					if (scheduleItem.Rows[0][CN.RetItemNo] != DBNull.Value)
						this._retitemno = (string)scheduleItem.Rows[0][CN.RetItemNo];
					else
						this._retitemno = "";

                    this.RetItemID = Convert.ToInt32(scheduleItem.Rows[0][CN.RetItemId]);        //IP - 07/06/11 - CR1212 - RI

					if (scheduleItem.Rows[0][CN.RetStockLocn] != DBNull.Value)
						this._retstocklocn = (short)scheduleItem.Rows[0][CN.RetStockLocn];
					else
						this._retstocklocn = 0;

					if (scheduleItem.Rows[0][CN.RetVal] != DBNull.Value)
						this._retval = Convert.ToDouble(scheduleItem.Rows[0][CN.RetVal]);
					else
						this._retval = 0;

					if (scheduleItem.Rows[0][CN.VanNo] != DBNull.Value)
						this._vanno = (string)scheduleItem.Rows[0][CN.VanNo];
					else
						this._vanno = "";

					if (scheduleItem.Rows[0][CN.LoadNo] != DBNull.Value)
						this._loadno = (short)scheduleItem.Rows[0][CN.LoadNo];
					else
						this._loadno = 0;

					this._user = (int)scheduleItem.Rows[0][CN.PrintedBy];
                    this._undeliveredflag = ((string)scheduleItem.Rows[0][CN.UndeliveredFlag]).Trim();
				}
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return loaded;
		}

		public bool Write(SqlConnection conn, SqlTransaction trans, int changedBy)
		{
			bool inserted = false;
			try
			{
				parmArray = new SqlParameter[23];
				parmArray[0] = new SqlParameter("@origbr", SqlDbType.SmallInt);
				parmArray[0].Value = this.OrigBr;
				parmArray[1] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
				parmArray[1].Value = this.AccountNumber;
				parmArray[2] = new SqlParameter("@agrmtno", SqlDbType.Int);
				parmArray[2].Value = this.AgreementNumber;
				parmArray[3] = new SqlParameter("@datedelplan", SqlDbType.DateTime);
				parmArray[3].Value = this.DateDelPlan;
				parmArray[4] = new SqlParameter("@delorcol", SqlDbType.NChar,1);
				parmArray[4].Value = this.DeliveryOrCollection;
                //parmArray[5] = new SqlParameter("@itemno", SqlDbType.NVarChar,8);
                //parmArray[5].Value = this.ItemNumber;
                parmArray[5] = new SqlParameter("@itemId", SqlDbType.Int);          // RI
                parmArray[5].Value = this.ItemID;
				parmArray[6] = new SqlParameter("@stocklocn", SqlDbType.SmallInt);
				parmArray[6].Value = this.StockLocation;
				parmArray[7] = new SqlParameter("@quantity", SqlDbType.Float);
				parmArray[7].Value = this.Quantity;
                //parmArray[8] = new SqlParameter("@retitemno", SqlDbType.NVarChar,8);
                //parmArray[8].Value = this.ReturnItemNumber;
                parmArray[8] = new SqlParameter("@retitemId", SqlDbType.Int);       // RI
                parmArray[8].Value = this.RetItemID;
                parmArray[9] = new SqlParameter("@retstocklocn", SqlDbType.SmallInt);
				parmArray[9].Value = this.ReturnStockLocation;
				parmArray[10] = new SqlParameter("@retval", SqlDbType.Float);
				parmArray[10].Value = this.RetVal;
				parmArray[11] = new SqlParameter("@buffno", SqlDbType.Int);
				parmArray[11].Value = this.BuffNo;
				parmArray[12] = new SqlParameter("@buffbranchno", SqlDbType.SmallInt);
				parmArray[12].Value = this.BuffBranchNo;
				parmArray[13] = new SqlParameter("@vanno", SqlDbType.NVarChar, 8);
				parmArray[13].Value = this.VanNo;
				parmArray[14] = new SqlParameter("@loadNo", SqlDbType.SmallInt);
				parmArray[14].Value = this.LoadNo;
				parmArray[15] = new SqlParameter("@printedby", SqlDbType.Int);
				parmArray[15].Value = this.User;
				parmArray[16] = new SqlParameter("@changedby", SqlDbType.Int);
				parmArray[16].Value = changedBy;
				parmArray[17] = new SqlParameter("@contractno", SqlDbType.NVarChar, 10);
				parmArray[17].Value = this.ContractNo;
                parmArray[18] = new SqlParameter("@undeliveredflag", SqlDbType.NChar, 1);
                parmArray[18].Value = this.UndeliveredFlag;
                parmArray[19] = new SqlParameter("@createdby", SqlDbType.Int);   //IP - 10/02/10 - CR1048 (Ref:3.1.4 & 3.1.5) Merged - Malaysia Enhancements (CR1072)
                parmArray[19].Value = this.CreatedBy;
                parmArray[20] = new SqlParameter("@dateCreated", SqlDbType.DateTime);   //IP - 10/02/10 - CR1048 (Ref:3.1.4 & 3.1.5) Merged - Malaysia Enhancements (CR1072)
                parmArray[20].Value = this.DateCreated;
                parmArray[21] = new SqlParameter("@GRTnotes", SqlDbType.VarChar, 200);
                parmArray[21].Value = this.GRTnotes;
                parmArray[22] = new SqlParameter("@ParentItemID", SqlDbType.Int);
                parmArray[22].Value = this.ParentItemID;

				inserted = Convert.ToBoolean(RunSP(conn, trans, "DN_ScheduleUpdateSP", parmArray));
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return inserted;
		}

        public bool WriteCollectReason(SqlConnection conn, SqlTransaction trans, int changedBy)
        {
            bool inserted = false;
            try
            {
                parmArray = new SqlParameter[6];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[0].Value = this.AccountNumber;
                //parmArray[1] = new SqlParameter("@itemno", SqlDbType.NVarChar, 8);
                //parmArray[1].Value = this.ItemNumber;
                parmArray[1] = new SqlParameter("@itemId", SqlDbType.Int);          // RI
                parmArray[1].Value = this.ItemID;
                parmArray[2] = new SqlParameter("@stocklocn", SqlDbType.SmallInt);
                parmArray[2].Value = this.StockLocation;
                parmArray[3] = new SqlParameter("@changedby", SqlDbType.Int);
                parmArray[3].Value = changedBy;
                parmArray[4] = new SqlParameter("@collectreason", SqlDbType.NVarChar, 30);
                parmArray[4].Value = this.CollectReason;
                parmArray[5] = new SqlParameter("@collecttype", SqlDbType.NVarChar, 3);
                parmArray[5].Value = this.CollectType;


                inserted = Convert.ToBoolean(RunSP(conn, trans, "DN_GoodsReturn_CollectReason", parmArray));
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return inserted;
        }

		public void GetCollections()
		{
			try
			{
				_schedules = new DataTable("Schedules");
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@stocklocn", SqlDbType.SmallInt);
				parmArray[0].Value = this.StockLocation;

				this.RunSP("DN_LoadCollectionDetailsSP", parmArray, _schedules);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void AddDelivery(SqlConnection conn, SqlTransaction trans)
		{
			try
			{
				parmArray = new SqlParameter[8];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NChar, 12);
				parmArray[0].Value = this.AccountNumber;
				parmArray[1] = new SqlParameter("@agrmtno", SqlDbType.Int);
				parmArray[1].Value = this.AgreementNumber;
				parmArray[2] = new SqlParameter("@itemno", SqlDbType.NVarChar, 8);
				parmArray[2].Value = this.ItemNumber;
				parmArray[3] = new SqlParameter("@stocklocn", SqlDbType.SmallInt);
				parmArray[3].Value = this.StockLocation;
				parmArray[4] = new SqlParameter("@buffno", SqlDbType.Int);
				parmArray[4].Value = this.BuffNo;
				parmArray[5] = new SqlParameter("@buffbranchno", SqlDbType.SmallInt);
				parmArray[5].Value = this.BuffBranchNo;
				parmArray[6] = new SqlParameter("@loadno", SqlDbType.Int);
				parmArray[6].Value = this.LoadNo;
				parmArray[7] = new SqlParameter("@user", SqlDbType.Int);
				parmArray[7].Value = this.User;

				this.RunSP(conn, trans, "DN_ScheduleAddDeliverySP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void GetScheduledForAccount()
		{
			try
			{
				_schedules = new DataTable("Schedules");
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar,12);
				parmArray[0].Value = this.AccountNumber;

				this.RunSP("DN_Schedule_GetByAcctNo", parmArray, _schedules);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

        public void DeleteScheduledForAccount(SqlConnection conn, SqlTransaction trans)
		{
			try
			{
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar,12);
				parmArray[0].Value = this.AccountNumber;

                if (conn != null && trans != null)
                {
                    this.RunSP(conn, trans, "DN_ScheduleDeleteByAcctNoSP", parmArray);
                }
                else
                {
				this.RunSP("DN_ScheduleDeleteByAcctNoSP", parmArray);
                }
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void CancelSchedule(SqlConnection conn, SqlTransaction trans, 
								int newBuffNo, bool isDotNetWarehouse)
		{
			try
			{
				parmArray = new SqlParameter[9];
				parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar,12);
				parmArray[0].Value = this.AccountNumber;
				parmArray[1] = new SqlParameter("@agreementNo", SqlDbType.Int);
				parmArray[1].Value = this.AgreementNumber;
				parmArray[2] = new SqlParameter("@itemId", SqlDbType.Int);
				parmArray[2].Value = this.ItemID;
				parmArray[3] = new SqlParameter("@location", SqlDbType.SmallInt);
				parmArray[3].Value = this.StockLocation;
				parmArray[4] = new SqlParameter("@buffBranch", SqlDbType.SmallInt);
				parmArray[4].Value = this.BuffBranchNo;
				parmArray[5] = new SqlParameter("@buffNo", SqlDbType.Int);
				parmArray[5].Value = this.BuffNo;
				parmArray[6] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[6].Value = this.User;
				parmArray[7] = new SqlParameter("@newbuffno", SqlDbType.Int);
				parmArray[7].Value = newBuffNo;
				parmArray[8] = new SqlParameter("@type", SqlDbType.Bit);
				parmArray[8].Value = isDotNetWarehouse;

				this.RunSP(conn, trans, "DN_ScheduleCancelDeliverySP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void GetScheduledDelNote(out bool onPickList, out bool delNotePrinted, 
										out bool onLoad)
		{
			onPickList = false;
			delNotePrinted = false;
			onLoad = false;

			try
			{
				parmArray = new SqlParameter[7];
				parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar,12);
				parmArray[0].Value = this.AccountNumber;
				parmArray[1] = new SqlParameter("@agrmtno", SqlDbType.Int);
				parmArray[1].Value = this.AgreementNumber;
                parmArray[2] = new SqlParameter("@itemID", SqlDbType.Int);                  //IP/NM - 18/05/11 -CR1212 - #3627 
				parmArray[2].Value = this.ItemID;
				parmArray[3] = new SqlParameter("@stocklocn", SqlDbType.SmallInt);
				parmArray[3].Value = this.StockLocation;
				parmArray[4] = new SqlParameter("@delnoteprinted", SqlDbType.SmallInt);
				parmArray[4].Value = 0;
				parmArray[4].Direction = ParameterDirection.Output;
				parmArray[5] = new SqlParameter("@onpicklist", SqlDbType.SmallInt);
				parmArray[5].Value = 0;
				parmArray[5].Direction = ParameterDirection.Output;
				parmArray[6] = new SqlParameter("@onload", SqlDbType.SmallInt);
				parmArray[6].Value = 0;
				parmArray[6].Direction = ParameterDirection.Output;

				this.RunSP("DN_DeliveryNoteSchdulePrintedSP", parmArray);

				if(parmArray[4].Value != DBNull.Value)
					delNotePrinted = Convert.ToBoolean(parmArray[4].Value);
				if(parmArray[5].Value != DBNull.Value)
					onPickList = Convert.ToBoolean(parmArray[5].Value);
				if(parmArray[6].Value != DBNull.Value)
					onLoad = Convert.ToBoolean(parmArray[6].Value);
			}

			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void GetCancelledDelNote(SqlConnection conn, SqlTransaction trans, 
										out int buffNo, out DateTime datePrinted)
		{
			buffNo = 0;
			datePrinted = DateTime.MinValue.AddYears(1899);

			try
			{
				parmArray = new SqlParameter[6];
				parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar,12);
				parmArray[0].Value = this.AccountNumber;
				parmArray[1] = new SqlParameter("@agrmtno", SqlDbType.Int);
				parmArray[1].Value = this.AgreementNumber;
                parmArray[2] = new SqlParameter("@itemId", SqlDbType.Int);
				parmArray[2].Value = this.ItemID;
				parmArray[3] = new SqlParameter("@stocklocn", SqlDbType.SmallInt);
				parmArray[3].Value = this.StockLocation;
				parmArray[4] = new SqlParameter("@buffno", SqlDbType.Int);
				parmArray[4].Value = 0;
				parmArray[4].Direction = ParameterDirection.Output;
				parmArray[5] = new SqlParameter("@dateprinted", SqlDbType.DateTime);
				parmArray[5].Value = DateTime.MinValue.AddYears(1899);;
				parmArray[5].Direction = ParameterDirection.Output;

				if(conn!=null && trans!=null)
					this.RunSP(conn, trans, "DN_DeliveryNoteGetCancelledSP", parmArray);
				else
					this.RunSP("DN_DeliveryNoteGetCancelledSP", parmArray);

				if(parmArray[4].Value != DBNull.Value)
					buffNo = (int)parmArray[4].Value;
				if(parmArray[5].Value != DBNull.Value)
					datePrinted = (DateTime)parmArray[5].Value;
			}

			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}
		public int RemoveLoadFromContents(SqlConnection conn, SqlTransaction trans, 
			DateTime dateDel, short stocklocn, int buffNo, short loadNo, int user)
		{
			try
			{
				parmArray = new SqlParameter[5];
				parmArray[0] = new SqlParameter("@dateDel", SqlDbType.DateTime);
				parmArray[0].Value = dateDel;
				parmArray[1] = new SqlParameter("@stocklocn", SqlDbType.SmallInt);
				parmArray[1].Value = stocklocn;
				parmArray[2] = new SqlParameter("@buffNo", SqlDbType.Int);
				parmArray[2].Value = buffNo;
				parmArray[3] = new SqlParameter("@loadNo", SqlDbType.SmallInt);
				parmArray[3].Value = loadNo;
				parmArray[4] = new SqlParameter("@piEmpeeno", SqlDbType.Int);
				parmArray[4].Value = user;

				this.RunSP(conn, trans, "DN_RemoveLoadFromContentsSP", parmArray);
				return 0;

			}

			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public DataTable DeliveryScheduleGetItems(SqlConnection conn, SqlTransaction trans,
												short buffBranchNo, int buffNo)
		{
			DataTable dt = null;
			try
			{
				dt = new DataTable(TN.Schedules);

				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@buffbranchno", SqlDbType.SmallInt);
				parmArray[0].Value = buffBranchNo;
				parmArray[1] = new SqlParameter("@buffno", SqlDbType.Int);
				parmArray[1].Value = buffNo;

				this.RunSP(conn, trans, "DN_DeliveryScheduleGetItemsSP", parmArray, dt);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return dt;
		}
		
		public void GetPickListSchedule(int filter, int PickListNo, int BuffNo)
		{
			try
			{
				_schedules = new DataTable("Schedules");
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@piFilter", SqlDbType.Int);
				parmArray[0].Value = filter;
				parmArray[1] = new SqlParameter("@piPickListNo", SqlDbType.Int);
				parmArray[1].Value = PickListNo;
				parmArray[2] = new SqlParameter("@piBuffNo", SqlDbType.Int);
				parmArray[2].Value = BuffNo;

				this.RunSP("DN_GetPickListScheduleSP", parmArray, _schedules);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void GetDeliveryNotes(SqlConnection conn, SqlTransaction trans, int buffNo, int branchNo)
		{
			try
			{
				_schedules = new DataTable(TN.Schedules);
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@branchno", SqlDbType.Int);
				parmArray[0].Value = branchNo;
				parmArray[1] = new SqlParameter("@buffno", SqlDbType.Int);
				parmArray[1].Value = buffNo;

                //IP - 09/02/10 - CR1048 (Ref:3.1.3) Merged - Malaysia Enhancements (CR1072)
                SqlParameter[] tempParmArray = new SqlParameter[1];
                tempParmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar, 12);
                tempParmArray[0].Value = AccountNumber;

                RunSP(conn, trans, "DN_ResetLineItemPrintOrder", tempParmArray); //IP - 09/02/10 - CR1048 (Ref:3.1.3) Merged - Malaysia Enhancements (CR1072)
				this.RunSP(conn, trans, "DN_DeliveryLoadAcctsSP", parmArray, _schedules);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void SetTransportSchedulePrinted(SqlConnection conn, SqlTransaction trans,
			short loadNo, short branchNo, DateTime dateDel)
		{
			try
			{
				
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@loadno", SqlDbType.SmallInt);
				parmArray[0].Value = loadNo;
				parmArray[1] = new SqlParameter("@branchno", SqlDbType.SmallInt);
				parmArray[1].Value = branchNo;
				parmArray[2] = new SqlParameter("@datedel", SqlDbType.DateTime);
				parmArray[2].Value = dateDel;

				RunSP(conn, trans, "DN_TransportScheduleSetPrintedSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void SetDeliveryNotesPrinted(SqlConnection conn, SqlTransaction trans,
										string acctNo, int buffNo, int branchNo)
		{
			try
			{
				parmArray = new SqlParameter[4];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
				parmArray[0].Value = acctNo;
				parmArray[1] = new SqlParameter("@buffno", SqlDbType.Int);
				parmArray[1].Value = buffNo;
				parmArray[2] = new SqlParameter("@buffbranchno", SqlDbType.SmallInt);
				parmArray[2].Value = branchNo;
				parmArray[3] = new SqlParameter("@user", SqlDbType.Int);
				parmArray[3].Value = this.User;

				this.RunSP(conn, trans, "DN_ScheduleSetDeliveryPrintedSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void GetScheduledAssociatedItems(SqlConnection conn, SqlTransaction trans)
		{
			try
			{
				_schedules = new DataTable(TN.Schedules);
				parmArray = new SqlParameter[4];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
				parmArray[0].Value = this.AccountNumber;
				parmArray[1] = new SqlParameter("@agreementno", SqlDbType.Int);
				parmArray[1].Value = this.AgreementNumber;
				parmArray[2] = new SqlParameter("@itemID", SqlDbType.Int);              //IP - 17/05/11 - CR1212 - #3627 - Changed from @itemno to @itemID
				//parmArray[2].Value = this.ItemNumber;
                parmArray[2].Value = this.ItemID;                       // RI
				parmArray[3] = new SqlParameter("@location", SqlDbType.SmallInt);
				parmArray[3].Value = this.StockLocation;

				this.RunSP(conn, trans, "DN_ScheduleGetAssociatedItemsSP", parmArray, _schedules);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void AlignScheduledLineItems(SqlConnection conn, SqlTransaction trans, 
			string accountNo, int agreementNo, string itemNo, short location, double quantity)
		{
			try
			{
				parmArray = new SqlParameter[5];
				parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@agrmtno", SqlDbType.Int);
				parmArray[1].Value = agreementNo;
				parmArray[2] = new SqlParameter("@itemno", SqlDbType.NVarChar,8);
				parmArray[2].Value = itemNo;
				parmArray[3] = new SqlParameter("@stocklocn", SqlDbType.SmallInt);
				parmArray[3].Value = location;
				parmArray[4] = new SqlParameter("@quantity", SqlDbType.Float);
				parmArray[4].Value = quantity;

				this.RunSP(conn, trans, "DN_ScheduleAlignItemsSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void DeleteAssociatedItems(SqlConnection conn, SqlTransaction trans, int changedBy)
		{
			try
			{
				parmArray = new SqlParameter[5];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = this.AccountNumber;
				parmArray[1] = new SqlParameter("@agreementno", SqlDbType.Int);
				parmArray[1].Value = this.AgreementNumber;
				parmArray[2] = new SqlParameter("@itemID", SqlDbType.Int);              //IP - 06/06/11 - CR1212 - RI - #3806
				parmArray[2].Value = this.ItemID;
				parmArray[3] = new SqlParameter("@stocklocn", SqlDbType.SmallInt);
				parmArray[3].Value = this.StockLocation;
				parmArray[4] = new SqlParameter("@changedby", SqlDbType.Int);
				parmArray[4].Value = changedBy;

				this.RunSP(conn, trans, "DN_ScheduleDeleteAssociatedItemsSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void AuditItem(SqlConnection conn, SqlTransaction trans)
		{
			try
			{
				parmArray = new SqlParameter[6];
				parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar,12);
				parmArray[0].Value = this.AccountNumber;
				parmArray[1] = new SqlParameter("@agreementNo", SqlDbType.Int);
				parmArray[1].Value = this.AgreementNumber;
				parmArray[2] = new SqlParameter("@itemId", SqlDbType.Int);
				parmArray[2].Value = this.ItemID;
				parmArray[3] = new SqlParameter("@location", SqlDbType.SmallInt);
				parmArray[3].Value = this.StockLocation;
				parmArray[4] = new SqlParameter("@buffBranch", SqlDbType.SmallInt);
				parmArray[4].Value = this.BuffBranchNo;
				parmArray[5] = new SqlParameter("@buffNo", SqlDbType.Int);
				parmArray[5].Value = this.BuffNo;

				this.RunSP(conn, trans, "DN_ScheduleAuditItemSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void ScheduleCollectionAudit(SqlConnection conn, SqlTransaction trans,
            string acctNo, int agreementNo, int itemID, short curRetStockLocn, short retStockLocn,              //IP/NM - 18/05/11 -CR1212 - #3627 - use itemID rather than itemNo
			double curQuantity, double quantity, int changedBy)
		{
			try
			{
				parmArray = new SqlParameter[8];
				parmArray[0] = new SqlParameter("@AcctNo", SqlDbType.Char,12);
				parmArray[0].Value = acctNo;
				parmArray[1] = new SqlParameter("@AgrmtNo", SqlDbType.Int);
				parmArray[1].Value = agreementNo;
                parmArray[2] = new SqlParameter("@itemID", SqlDbType.Int);                                       //IP/NM - 18/05/11 -CR1212 - #3627
				parmArray[2].Value = itemID;
				parmArray[3] = new SqlParameter("@CurRetStockLocn", SqlDbType.SmallInt);
				parmArray[3].Value = curRetStockLocn;
				parmArray[4] = new SqlParameter("@RetStockLocn", SqlDbType.SmallInt);
				parmArray[4].Value = retStockLocn;
				parmArray[5] = new SqlParameter("@CurQuantity", SqlDbType.Float);
				parmArray[5].Value = curQuantity;
				parmArray[6] = new SqlParameter("@Quantity", SqlDbType.Float);
				parmArray[6].Value = quantity;
				parmArray[7] = new SqlParameter("@ChangedBy", SqlDbType.Int);
				parmArray[7].Value = changedBy;

				this.RunSP(conn, trans, "DN_ScheduleCollectionAuditSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void GetTransPickListDetails(short branchNo, int transPickickListNo)
		{
			try
			{
				_schedules = new DataTable("Schedules");
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@branch", SqlDbType.SmallInt);
				parmArray[0].Value = branchNo;
				parmArray[1] = new SqlParameter("@transpicklistno", SqlDbType.Int);
				parmArray[1].Value = transPickickListNo;

				this.RunSP("DN_GetTransportPickListDetailsSP", parmArray, _schedules);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void GetRevisedSchedules(short branchNo, int loadNo, int pickNo, 
			DateTime reviseFrom, DateTime reviseTo, out DateTime timeLocked)
		{
			try
			{
				_schedules = new DataTable(TN.Schedules);
				parmArray = new SqlParameter[7];
				parmArray[0] = new SqlParameter("@branchno", SqlDbType.SmallInt);
				parmArray[0].Value = branchNo;
				parmArray[1] = new SqlParameter("@loadno", SqlDbType.Int);
				parmArray[1].Value = loadNo;
				parmArray[2] = new SqlParameter("@pickno", SqlDbType.Int);
				parmArray[2].Value = pickNo;
				parmArray[3] = new SqlParameter("@revisefrom", SqlDbType.DateTime);
				parmArray[3].Value = reviseFrom;
				parmArray[4] = new SqlParameter("@reviseto", SqlDbType.DateTime);
				parmArray[4].Value = reviseTo;
				parmArray[5] = new SqlParameter("@user", SqlDbType.Int);
				parmArray[5].Value = this.User;
				parmArray[6] = new SqlParameter("@timelocked", SqlDbType.DateTime);
				parmArray[6].Direction = ParameterDirection.Output;

				this.RunSP("DN_ScheduleGetRevisedSchedulesSP", parmArray, _schedules);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			finally 
			{
				timeLocked = Convert.ToDateTime(parmArray[6].Value);
			}
		}

		public void GetRevisedScheduleDetails(string acctNo, int buffNo)
		{
			try
			{
				_schedules = new DataTable(TN.Schedules);
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = acctNo;
				parmArray[1] = new SqlParameter("@buffno", SqlDbType.Int);
				parmArray[1].Value = buffNo;

				this.RunSP("DN_ScheduleGetRevisedDetailsSP", parmArray, _schedules);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void GetRevisedScheduleChanges(string acctNo, int buffNo, int itemID, short locn)        //IP - 18/06/11 - CR1212 - RI - #4042
		{
			try
			{
				_schedules = new DataTable(TN.Schedules);
				parmArray = new SqlParameter[4];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = acctNo;
				parmArray[1] = new SqlParameter("@buffno", SqlDbType.Int);
				parmArray[1].Value = buffNo;
                //parmArray[2] = new SqlParameter("@itemNo", SqlDbType.NVarChar,10);
                //parmArray[2].Value = itemNo;
                parmArray[2] = new SqlParameter("@itemID", SqlDbType.Int);                              //IP - 18/06/11 - CR1212 - RI - #4042
                parmArray[2].Value = itemID;
				parmArray[3] = new SqlParameter("@locn", SqlDbType.SmallInt);
				parmArray[3].Value = locn;

				this.RunSP("DN_ScheduleGetChangesSP", parmArray, _schedules);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void ConfirmScheduleChanges(SqlConnection conn, SqlTransaction trans,
						short loadNo, int pickListNo, int pickListBranch, string acctNo, 
						//int agrmtno, string itemNo, short locn, int buffNo, int origBuffNo, 
                         int agrmtno, int itemID, short locn, int buffNo, int origBuffNo,                   //IP - 18/06/11 - CR1212 - RI - #4042
						//string removal, string origItemNo, int tranSchedNo, int tranSchedNoBranch)
                         string removal, int origItemID, int tranSchedNo, int tranSchedNoBranch)            //IP - 20/06/11 - CR1212 - RI - #4042
		{
			try
			{
				parmArray = new SqlParameter[14];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = acctNo;
				parmArray[1] = new SqlParameter("@agrmtno", SqlDbType.Int);
				parmArray[1].Value = agrmtno;
                //parmArray[2] = new SqlParameter("@itemno", SqlDbType.NVarChar,10);
                //parmArray[2].Value = itemNo;
                parmArray[2] = new SqlParameter("@itemID", SqlDbType.Int);                      //IP - 18/06/11 - CR1212 - RI - #4042
                parmArray[2].Value = itemID;
				parmArray[3] = new SqlParameter("@stocklocn", SqlDbType.SmallInt);
				parmArray[3].Value = locn;
				parmArray[4] = new SqlParameter("@buffno", SqlDbType.Int);
				parmArray[4].Value = buffNo;
				parmArray[5] = new SqlParameter("@origbuffno", SqlDbType.Int);
				parmArray[5].Value = origBuffNo;
				parmArray[6] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[6].Value = this.User;
				parmArray[7] = new SqlParameter("@removal", SqlDbType.NChar,1);
				parmArray[7].Value = removal;
				parmArray[8] = new SqlParameter("@loadno", SqlDbType.SmallInt);
				parmArray[8].Value = loadNo;
				parmArray[9] = new SqlParameter("@picklistno", SqlDbType.Int);
				parmArray[9].Value = pickListNo;
				parmArray[10] = new SqlParameter("@picklistbranch", SqlDbType.SmallInt);
				parmArray[10].Value = pickListBranch;
                //parmArray[11] = new SqlParameter("@origitemno", SqlDbType.NVarChar,12);
                //parmArray[11].Value = origItemNo;
                parmArray[11] = new SqlParameter("@origItemID", SqlDbType.Int);                 //IP - 20/06/11 - CR1212 - RI - #4042
                parmArray[11].Value = origItemID;
				parmArray[12] = new SqlParameter("@transchedno", SqlDbType.Int);
				parmArray[12].Value = tranSchedNo;
				parmArray[13] = new SqlParameter("@transchednobranch", SqlDbType.Int);
				parmArray[13].Value = tranSchedNoBranch;

				this.RunSP(conn, trans, "DN_ScheduleConfirmChangesSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void GetAdditionalItems(string acctNo, short loadNo, int pickListNo, int pickListBranch)
		{
			try
			{
				_schedules = new DataTable(TN.Schedules);
				parmArray = new SqlParameter[4];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = acctNo;
				parmArray[1] = new SqlParameter("@loadno", SqlDbType.SmallInt);
				parmArray[1].Value = loadNo;
				parmArray[2] = new SqlParameter("@picklistno", SqlDbType.Int);
				parmArray[2].Value = pickListNo;
				parmArray[3] = new SqlParameter("@picklistbranch", SqlDbType.SmallInt);
				parmArray[3].Value = pickListBranch;

				this.RunSP("DN_ScheduleGetAdditionalItemsSP", parmArray, _schedules);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

        //IP - 28/11/2007 - 69360
        //Method returns 'True' or 'False' when checking for scheduled records for an account.
        //Returns true if records are found.
        public bool AccountScheduleExists(SqlConnection conn, SqlTransaction trans, string accountNo)
        {
            bool hasSchedule = false;
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar, 12);
                parmArray[0].Value = accountNo;
                parmArray[1] = new SqlParameter("@scheduleExists", SqlDbType.Bit);
                parmArray[1].Value = 0;
                parmArray[1].Direction = ParameterDirection.Output;

                this.RunSP("DN_AccountScheduleExistsSP", parmArray);

                if (parmArray[1].Value != DBNull.Value)
                    hasSchedule = Convert.ToBoolean(parmArray[1].Value);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return hasSchedule;
        }

        public void SetDHLDeliveryNotesPrinted(SqlConnection conn, SqlTransaction trans,
                              string acctNo, int branchNo, int buffbranchno)
        {
            try
            {
                parmArray = new SqlParameter[4];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[0].Value = acctNo;
                parmArray[1] = new SqlParameter("@currentBranch", SqlDbType.Int);
                parmArray[1].Value = branchNo;
                parmArray[2] = new SqlParameter("@user", SqlDbType.Int);
                parmArray[2].Value = this.User;
                parmArray[3] = new SqlParameter("@buffbranchno", SqlDbType.Int);
                parmArray[3].Value = buffbranchno;

                this.RunSP(conn, trans, "DN_ScheduleSetDHLDeliveryPrintedSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void InsertIntoIgnoreCRECRF(SqlConnection conn, SqlTransaction trans, string acctno, string contractno, short stockLocn)
        {

            try
            {
                parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@AcctNo", SqlDbType.NVarChar, 12);
                parmArray[0].Value = acctno;
                parmArray[1] = new SqlParameter("@ContractNo", SqlDbType.NVarChar, 10);
                parmArray[1].Value = contractno;
                parmArray[2] = new SqlParameter("@StockLocn", SqlDbType.Int);
                parmArray[2].Value = stockLocn;

                this.RunSP(conn, trans, "InsertIntoIgnoreCRECRFSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void DeleteFromCalculateCRECRF(SqlConnection conn, SqlTransaction trans, string acctno, string contractno, short stockLocn)
        {
            try
            {
                parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@AcctNo", SqlDbType.NVarChar, 12);
                parmArray[0].Value = acctno;
                parmArray[1] = new SqlParameter("@ContractNo", SqlDbType.NVarChar, 10);
                parmArray[1].Value = contractno;
                parmArray[2] = new SqlParameter("@StockLocn", SqlDbType.Int);
                parmArray[2].Value = stockLocn;

                this.RunSP(conn, trans, "IgnoreCRECRFDeleteSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        //BOC Added by Suvidha - CR 2018-13 - 15/04/19 - to update data for GRT.
        public void UpdateInvoiceVersionForGRT(SqlConnection conn, SqlTransaction trans, string accountNo, int agreementNo, string origItemNo, int returnQuantity, string retItemNo, decimal retVal, string contractNo
            , int parentitemID, int lineitemID, decimal ordVal, decimal taxAmt)
        {
            try
            {
                parmArray = new SqlParameter[11];
                parmArray[0] = new SqlParameter("@acctNo", SqlDbType.VarChar, 12);
                parmArray[0].Value = accountNo;
                parmArray[1] = new SqlParameter("@agrmtno", SqlDbType.Int);
                parmArray[1].Value = agreementNo;
                parmArray[2] = new SqlParameter("@ReturnQuantity", SqlDbType.Int);
                parmArray[2].Value = returnQuantity;
                parmArray[3] = new SqlParameter("@RetItemNo", SqlDbType.VarChar);
                parmArray[3].Value = retItemNo;
                parmArray[4] = new SqlParameter("@RetVal", SqlDbType.Decimal);
                parmArray[4].Value = retVal;
                parmArray[5] = new SqlParameter("@orig_item_no", SqlDbType.VarChar);
                parmArray[5].Value = origItemNo;
                parmArray[6] = new SqlParameter("@contractNo", SqlDbType.VarChar);
                parmArray[6].Value = contractNo;
                parmArray[7] = new SqlParameter("@parentitemID", SqlDbType.Int);
                parmArray[7].Value = parentitemID;
                parmArray[8] = new SqlParameter("@lineItemID", SqlDbType.Int);
                parmArray[8].Value = lineitemID;
                parmArray[9] = new SqlParameter("@orderValue", SqlDbType.Decimal);
                parmArray[9].Value = ordVal; 
                parmArray[10] = new SqlParameter("@taxAmt", SqlDbType.Decimal);
                parmArray[10].Value = taxAmt;

                this.RunSP(conn, trans, "DN_UpdateInvoiceVersionForGRT", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }
        //EOC

        public DSchedule()
		{

		}
	}
}
