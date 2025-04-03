using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common;
using STL.Common.Constants.Enums;
using System.Xml;
using STL.Common.Constants.Tags;
using STL.Common.Constants.TableNames;

namespace STL.DAL
{
	/// <summary>
	/// Summary description for DStockItem.
	/// </summary>
	public class DStockItem : DALObject  
	{
		private DataTable _bylocation;
		private DataTable _stocklocs;
        private DataSet _stockItem;       //CR1094 jec
        private DataTable _categories;       //CR1094 jec
        private DataTable _returnCodes;       //CR1094 jec
        private DataTable _warrantyItems;       //CR1094 jec
        private DataSet _dropDown;       //CR1212 jec
		private DataTable _bycode;
		private DataTable _warranties;
		private DataTable _components;
        private DataTable _topSelling;
		private DataTable _translations;
        private double _stock = 0;
		private double _stockdamage = 0;
		private string _itemdescr1 = "";
		private string _itemdescr2 = "";
		private string _suppliercode = "";
		private double _unitprice = 0;
        private decimal _costprice = 0;
		private double _cashprice = 0;
		private double _hpprice = 0;
		private double _dutyfreeprice = 0;
		private int _valuecontrolled = 0;
		private int _kit = 0;
		private int _isstock = 0;
		private int _isdiscount = 0;
		private int _iswarranty = 0;
		private double _kitdiscount = 0;
		private double _taxrate = 0;
        private double _Additionaltaxrate = 0; //BCX : This is used for LUX tax for curacao 
        private int _isaffinity = 0;
		private bool _isStampDuty = false;
		private bool _isFreeGift = false;
		private double _qtyonorder = 0;
		private short _leadtime = 0;
        private string _refcode = string.Empty; //IP - 28/01/10 - LW 72136

        private object _itemNumber = null;
        private object _itemDescr1 = null;
        private object _itemDescr2 = null;
        private object _itemSupplierName = null;
        private object _itemSupplierCode = null;
        private object _itemStatus = null;
        private object _itemCategory = null;
        private object _itemTaxRate = null;
        private object _unitHPPrice = null;
        private object _unitCashPrice = null;
        private object _unitDutyFreePrice = null;
        private object _unitCostPrice = null;
        private object _branchNo = null;
        private object _deletionDate = null;
        private object _itemId = null;          // RI
        //CR1094 jec 16/12/10
        private string _productType = null;
        private string _warrCategory = null;
        private string _returnCode = null;
        private int _expiredMonths = 0;
        private int _warrantyMonths = 0;
        private int _manufactMonths = 0;
        private double _refundPct = 0;
        private DateTime _dateNow;

        private string _productGroup = null;
        private string _class = null;
        private string _subClass = null;
        //private string _delete = null;
        private bool _repoItem = false;        //RI jec 16/06/11
        
        #region [ ---- Public Properties ---- ]
        public string ItemNo 
        { 
            set { _itemNumber = value; } 
            get { return (_itemNumber ?? "").ToString(); } 
        }
        public string ItemDescr1 { set { _itemDescr1 = value; } }
        public string ItemDescr2 { set { _itemDescr2 = value; } }
        public string ItemSupplierName { set { _itemSupplierName = value; } }
        public string ItemSupplierCode { set { _itemSupplierCode = value; } }
        public string ItemStatus { set { _itemStatus = value; } }
        public Int32 ItemCategory { set { _itemCategory = value; } }
        public double ItemTaxRate { set { _itemTaxRate = value; } }
        public decimal UnitHPPrice { set { _unitHPPrice = value; } }
        public decimal UnitCashPrice { set { _unitCashPrice = value; } }
        public decimal UnitDutyFreePrice { set { _unitDutyFreePrice = value; } }
        public decimal UnitCostPrice { set { _unitCostPrice = value; } }
        public int BranchNo { set { _branchNo = value; } }
        public DateTime DeletionDate { set { _deletionDate = value; } }
        public int ItemId { set { _itemId = value; } }          // RI
        //CR1094 jec 16/12/10
        public string ProductType { set { _productType = value; } }
        public string WarrCategory { set { _warrCategory = value; } }
        public string ReturnCode { set { _returnCode = value; } }
        public Int32 ExpiredMonths { set { _expiredMonths = value; } }
        public Int32 WarrantyMonths { set { _warrantyMonths = value; } }
        public Int32 ManufactMonths { set { _manufactMonths = value; } }
        public double RefundPct { set { _refundPct = value; } }
        public DateTime DateNow { set { _dateNow = value; } }
        public string ProductGroup { set { _productGroup = value; } }        // RI        
        public string Class 
        {
            get   { return _class; }                                        //IP - 27/07/11 - RI - #4415
             set { _class = value; } 
        }
        public string SubClass 
        {
            get { return _subClass; }                                        //IP - 27/07/11 - RI - #4415
            set { _subClass = value; } 
        }

        public bool RepoItem                    //RI jec 16/06/11
        {
            get { return _repoItem; }
            set { _repoItem = value; }
        }        
        
		public bool IsFreeGift
		{
			get{return _isFreeGift;}
		}

		public bool IsStampDuty
		{
			get{return _isStampDuty;}
		}

		public bool IsAffinity
		{
			get
			{
				return Convert.ToBoolean(_isaffinity);
			}
		}

		public double TaxRate
		{
			get
			{
				return _taxrate;
			}
			set
			{
				_taxrate = value;
			}
		}
        //BCX : This is used for LUX tax for curacao
        public double Additionaltaxrate
        {
            get
            {
                return _Additionaltaxrate;
            }
            set
            {
                _Additionaltaxrate = value;
            }
        }

        

        public DataTable Components
		{
			get
			{
				return _components;
			}
			set
			{
				_components = value;
			}
		}

		public bool IsWarranty
		{
			get
			{
				return Convert.ToBoolean(_iswarranty);
			}
		}

		public bool IsDiscount
		{
			get
			{
				return Convert.ToBoolean(_isdiscount);
			}
		}

		public bool IsStock
		{
			get
			{
				return Convert.ToBoolean(_isstock);
			}
		}

		public bool IsKit
		{
			get
			{
				return Convert.ToBoolean(_kit);
			}
		}
	 
		public bool ValueControlled
		{
			get
			{
				return Convert.ToBoolean(_valuecontrolled);
			}
		}
		public double CashPrice
		{
			get
			{
				return _cashprice;
			}
		}
		public double HPPrice
		{
			get
			{
				return _hpprice;
			}
		}
		public double DutyFreePrice
		{
			get
			{
				return _dutyfreeprice;
			}
		}
		public double UnitPrice
		{
			get
			{
				return _unitprice;
			}
		}
        public decimal CostPrice
        {
            get
            {
                return _costprice;
            }
        }
		public string SupplierCode
		{
			get
			{
				return _suppliercode;
			}
		}
		public string ProductDesc2
		{
			get
			{
				return _itemdescr2;
			}
		}
		public string ProductDesc1
		{
			get
			{
				return _itemdescr1;
			}
		}
		public double DamagedStock
		{
			get
			{
				return _stockdamage;
			}
		}
		public double AvailableStock
		{
			get
			{
				return _stock;
			}
		}
		public double QtyOnOrder
		{
			get{return _qtyonorder;}
		}
		
		public short LeadTime
		{
			get{return _leadtime;}
		}

		private short _promobranch = 0;
		public short PromoBranch
		{
			get{return _promobranch;}
			set{_promobranch = value;}
		}

		private short _delnotebranch = 0;
		public short DelNoteBranch
		{
			get{return _delnotebranch;}
			set{_delnotebranch = value;}
		}

		private bool _isComponent = false;
		public bool IsComponent
		{
			get{return _isComponent;}
			set{_isComponent = value;}
		}

		private string _assemblyrequired = "";
		public string AssemblyRequired
		{
			get{return _assemblyrequired;}
			set{_assemblyrequired = value;}
		}

		private string _productcategory = "";
		public string ProductCategory
		{
			get{return _productcategory;}
			set{_productcategory = value;}
		}

      private string m_sparePartsCategory;
      public string SparePartsCategory // Required for selecting a spare part for a Service Request JH 08/11/2007
      {
         get
         {
            return m_sparePartsCategory;
         }
         set
         {
            m_sparePartsCategory = value;
         }
      }

      //IP - 24/02/11 - #3130
      private bool isInstallation;
      public bool IsInstallation 
      {
          get
          {
              return isInstallation;
          }
          set
          {
              isInstallation = value;
          }
      }

      private bool isAssemblyCost;
      public bool IsAssemblyCost
      {
          get
          {
              return isAssemblyCost;
          }
          set
          {
              isAssemblyCost = value;
          }
      }

      private bool isAnnualServiceContract;
      public bool IsAnnualServiceContract
      {
          get
          {
              return isAnnualServiceContract;
          }
          set
          {
              isAnnualServiceContract = value;
          }
      }

      private bool isGenericService;
      public bool IsGenericService
      {
          get
          {
              return isGenericService;
          }
          set
          {
              isGenericService = value;
          }
      }

      public string ColourName { get; set; } //CR1212 jec 21/04/11
      public string Style { get; set; } //CR1212 jec 21/04/11
      public string Brand { get; set; } //IP - 19/09/11 - RI - #8218 - CR8201
      public int ItemID { get; set; } //CR1212 jec 21/04/11

        private string _deleted = "";
        public string Deleted
        {
            get { return _deleted; }
            set { _deleted = value; }
        }

        private short _category = 0;
        public short Category
        {
            get { return _category; }
            set { _category = value; }
        }

        //IP - 28/01/10 - LW 72136
        public string RefCode
        {
            get
            {
                return _refcode;
            }
        }

        public bool ReadyAssist {get;set;}  //#13716 - CR12949

        public string WarrantyType { get; set; } //#17883 //#15888

        #endregion

        public int GetStockByLocation(short stocklocn, short branchCode, int showDeleted, int showAvailable, string prodDesc, int limit)
		{
			try
			{
				_bylocation = new DataTable("ByLocation");

                parmArray = new SqlParameter[6];
                parmArray[0] = new SqlParameter("@stocklocn", SqlDbType.SmallInt);
                parmArray[0].Value = stocklocn;
				parmArray[1] = new SqlParameter("@branchCode", SqlDbType.SmallInt);
				parmArray[1].Value = branchCode;
				parmArray[2] = new SqlParameter("@showDeleted", SqlDbType.Int);
				parmArray[2].Value = showDeleted;
				parmArray[3] = new SqlParameter("@showAvailable", SqlDbType.Int);
				parmArray[3].Value = showAvailable;
				parmArray[4] = new SqlParameter("@prodDesc", SqlDbType.NVarChar,80);
                parmArray[4].Value = String.Format(@"""{0}""", prodDesc);  //Enclosed by double quotes for FTS Fulltext function;
				parmArray[5] = new SqlParameter("@limit", SqlDbType.Int);
				parmArray[5].Value = limit;

				result = this.RunSP("DN_StockGetByLocationSP", parmArray, _bylocation);
				if(result==0)
					result = (int)Return.Success;
				else
					result = (int)Return.Fail;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

		public int GetStockByCode(string productCode)
		{
			try
			{
				_bycode = new DataTable("ByCode");

				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@prodCode", SqlDbType.NVarChar,18);
				parmArray[0].Value = productCode;

				result = this.RunSP("DN_StockByCodeGetSP", parmArray, _bycode);
				if(result==0)
					result = (int)Return.Success;
				else
					result = (int)Return.Fail;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

		public int GetKitComponents(int itemId)
		{
			try
			{
				_components = new DataTable();
			
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@itemId", SqlDbType.Int);
				parmArray[0].Value = itemId;

				result = this.RunSP("DN_GetKitComponentsSP", parmArray, _components);
				if(result==0)
					result = (int)Return.Success;
				else
					result = (int)Return.Fail;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}




        public int GetProductWarranties(SqlConnection conn, SqlTransaction trans, int itemId, short branchCode, double unitPrice, string refCode, bool paidAndTaken)
		{
			try
			{
				_warranties = new DataTable(TN.Warranties);
				parmArray = new SqlParameter[5];
                parmArray[0] = new SqlParameter("@itemId", SqlDbType.Int);
				parmArray[0].Value = itemId;
				parmArray[1] = new SqlParameter("@location", SqlDbType.SmallInt);
				parmArray[1].Value = branchCode;
				parmArray[2] = new SqlParameter("@unitPrice", SqlDbType.Float);
				parmArray[2].Value = unitPrice;
				parmArray[3] = new SqlParameter("@refCode", SqlDbType.NVarChar, 2);
				parmArray[3].Value = refCode;
				parmArray[4] = new SqlParameter("@paidAndTaken", SqlDbType.SmallInt);
				parmArray[4].Value = Convert.ToInt16(paidAndTaken);

				if(conn!=null && trans != null)
					result = this.RunSP(conn, trans, "DN_WarrantiesGetProductSP", parmArray, _warranties);
				else
					result = this.RunSP("DN_WarrantiesGetProductSP", parmArray, _warranties);
				if(result==0)
					result = (int)Return.Success;
				else
					result = (int)Return.Fail;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

		public DataTable Warranties
		{
			get
			{
				return _warranties;
			}
		}


        public void GetStockLocations(ref int itemId, bool includeWarranties)       // RI jec 30/06/11
		{
			try
			{
				_stocklocs = new DataTable(TN.StockLocation);

				parmArray = new SqlParameter[4];
                //parmArray[0] = new SqlParameter("@productCode", SqlDbType.NVarChar,18);
                //parmArray[0].Value = itemNo;
                parmArray[0] = new SqlParameter("@itemid", SqlDbType.Int);          // RI
                parmArray[0].Value = itemId;
				parmArray[1] = new SqlParameter("@productCodeOut", SqlDbType.NVarChar,18);
				parmArray[1].Value = "";
				parmArray[1].Direction = ParameterDirection.Output;
                parmArray[2] = new SqlParameter("@deleted", SqlDbType.NChar, 1);
                parmArray[2].Value = "";
                parmArray[2].Direction = ParameterDirection.Output;
                parmArray[3] = new SqlParameter("@includeWarranties", SqlDbType.Bit); //IP - 17/02/10 - CR1072 - LW 71731 
                parmArray[3].Value = includeWarranties;

				this.RunSP("DN_StockGetLocationsSP", parmArray, _stocklocs);
				
                //if(DBNull.Value != parmArray[1].Value)
                //    itemNo = (string)parmArray[1].Value;
                if (DBNull.Value != parmArray[2].Value)
                    _deleted = (string)parmArray[2].Value;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public int GetKitDiscount(int itemId, short branchCode, string accountType, string countryCode, bool dutyFree, bool taxExempt)
		{
			try
			{
				parmArray = new SqlParameter[7];
                parmArray[0] = new SqlParameter("@itemId", SqlDbType.Int);
                parmArray[0].Value = itemId;
				parmArray[1] = new SqlParameter("@branchCode", SqlDbType.SmallInt);
				parmArray[1].Value = branchCode;
				parmArray[2] = new SqlParameter("@accountType", SqlDbType.NVarChar,3);
				parmArray[2].Value = accountType;
				parmArray[3] = new SqlParameter("@country", SqlDbType.NVarChar,1);
				parmArray[3].Value = countryCode;
				parmArray[4] = new SqlParameter("@dutyfree", SqlDbType.SmallInt);
				parmArray[4].Value = dutyFree;
				parmArray[5] = new SqlParameter("@taxExempt", SqlDbType.SmallInt);
				parmArray[5].Value = taxExempt;
				parmArray[6] = new SqlParameter("@discount", SqlDbType.Float);
				parmArray[6].Value = 0;
				parmArray[6].Direction = ParameterDirection.Output;

				result = this.RunSP("DN_GetKitDiscountSP", parmArray);
				if (result == 0)
				{
					if (parmArray[6].Value != DBNull.Value)
						_kitdiscount = (double)parmArray[6].Value;
					result = (int)Return.Success;
				}
				else
					result = (int)Return.Fail;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

		public DataTable StockLocations
		{
			get
			{
				return _stocklocs;
			}
		}
		public double KitDiscount
		{
			get
			{
				return _kitdiscount;
			}
		}

        //public int GetItemDetailsByItemNo(SqlConnection conn, SqlTransaction trans, string itemNo, short stocklocn, string accountType, string countryCode, bool dutyFree, bool taxExempt, string accountNo = null, int agrmtNo = 1)
        //{
        //    throw new NotImplementedException("Need to change the method to accept int ItemID");
        //}

        public int GetItemDetails(SqlConnection conn, SqlTransaction trans, int itemID, short stocklocn, string accountType, string countryCode, bool dutyFree, bool taxExempt, string accountNo = null, int agrmtNo = 1)
        {
            return GetItemDetails(conn, trans, itemID, stocklocn, stocklocn, accountType, countryCode, dutyFree, taxExempt, accountNo, agrmtNo);
        }

        public int GetItemDetails(SqlConnection conn, SqlTransaction trans, int itemID, short stocklocn, short branchCode,
            string accountType, string countryCode, bool dutyFree, bool taxExempt, string accountNo = null, int agrmtNo = 1)
        {
			try
			{
				short duty = Convert.ToInt16(dutyFree);
				short exempt = Convert.ToInt16(taxExempt);
				short component = Convert.ToInt16(this.IsComponent);

				if(this.PromoBranch == 0)
					this.PromoBranch = stocklocn;

                this.ItemID = itemID;

                parmArray = new SqlParameter[53];

                parmArray[0] = new SqlParameter("@itemNo", SqlDbType.NVarChar, 18);
				parmArray[0].Value = "";
                parmArray[0].Direction = ParameterDirection.InputOutput;

				parmArray[1] = new SqlParameter("@location", SqlDbType.SmallInt);
				parmArray[1].Value = stocklocn;

                parmArray[2] = new SqlParameter("@branch", SqlDbType.SmallInt);
                parmArray[2].Value = branchCode;

				parmArray[3] = new SqlParameter("@accounttype", SqlDbType.NVarChar,3);
				parmArray[3].Value = accountType;

				parmArray[4] = new SqlParameter("@country", SqlDbType.NVarChar,3);
				parmArray[4].Value = countryCode;

				parmArray[5] = new SqlParameter("@dutyfree", SqlDbType.SmallInt);
				parmArray[5].Value = duty;

				parmArray[6] = new SqlParameter("@taxExempt", SqlDbType.SmallInt);
				parmArray[6].Value = exempt;

				parmArray[7] = new SqlParameter("@stock", SqlDbType.Float);
				parmArray[7].Value = 0;
				parmArray[7].Direction = ParameterDirection.Output;

				parmArray[8] = new SqlParameter("@stockdamage", SqlDbType.Float);
				parmArray[8].Value = 0;
				parmArray[8].Direction = ParameterDirection.Output;

				parmArray[9] = new SqlParameter("@itemdescr1", SqlDbType.NVarChar,32);
				parmArray[9].Value = "";
				parmArray[9].Direction = ParameterDirection.Output;

				parmArray[10] = new SqlParameter("@itemdescr2", SqlDbType.NVarChar,40);
				parmArray[10].Value = "";
				parmArray[10].Direction = ParameterDirection.Output;

                parmArray[11] = new SqlParameter("@suppliercode", SqlDbType.NVarChar, 40); //CR 1024 (NM 23/04/2009)	
				parmArray[11].Value = "";
				parmArray[11].Direction = ParameterDirection.Output;

				parmArray[12] = new SqlParameter("@unitprice", SqlDbType.Float);
				parmArray[12].Value = 0;
				parmArray[12].Direction = ParameterDirection.Output;

				parmArray[13] = new SqlParameter("@cashprice", SqlDbType.Float);
				parmArray[13].Value = 0;
				parmArray[13].Direction = ParameterDirection.Output;

				parmArray[14] = new SqlParameter("@hpprice", SqlDbType.Float);
				parmArray[14].Value = 0;
				parmArray[14].Direction = ParameterDirection.Output;

				parmArray[15] = new SqlParameter("@dutyfreeprice", SqlDbType.Float);
				parmArray[15].Value = 0;
				parmArray[15].Direction = ParameterDirection.Output;

				parmArray[16] = new SqlParameter("@valueControlled", SqlDbType.Int);
				parmArray[16].Value = 0;
				parmArray[16].Direction = ParameterDirection.Output;

				parmArray[17] = new SqlParameter("@kit", SqlDbType.Int);
				parmArray[17].Value = 0;
				parmArray[17].Direction = ParameterDirection.Output;

				parmArray[18] = new SqlParameter("@isStock", SqlDbType.Int);
				parmArray[18].Value = 0;
				parmArray[18].Direction = ParameterDirection.Output;

				parmArray[19] = new SqlParameter("@discount", SqlDbType.Int);
				parmArray[19].Value = 0;
				parmArray[19].Direction = ParameterDirection.Output;

				parmArray[20] = new SqlParameter("@isWarranty", SqlDbType.Int);
				parmArray[20].Value = 0;
				parmArray[20].Direction = ParameterDirection.Output;

				parmArray[21] = new SqlParameter("@taxrate", SqlDbType.Float);
				parmArray[21].Value = 0;
				parmArray[21].Direction = ParameterDirection.Output;
            
				parmArray[22] = new SqlParameter("@isAffinity", SqlDbType.Int);
				parmArray[22].Value = 0;
				parmArray[22].Direction = ParameterDirection.Output;

				parmArray[23] = new SqlParameter("@isStampDuty", SqlDbType.SmallInt);
				parmArray[23].Value = 0;
				parmArray[23].Direction = ParameterDirection.Output;

				parmArray[24] = new SqlParameter("@isFreeGift", SqlDbType.SmallInt);
				parmArray[24].Value = 0;
				parmArray[24].Direction = ParameterDirection.Output;

				parmArray[25] = new SqlParameter("@promobranch", SqlDbType.SmallInt);
				parmArray[25].Value = this.PromoBranch;
            
				parmArray[26] = new SqlParameter("@isComponent", SqlDbType.SmallInt);
				parmArray[26].Value = component;

				parmArray[27] = new SqlParameter("@qtyonorder", SqlDbType.Float);
				parmArray[27].Value = 0;
				parmArray[27].Direction = ParameterDirection.Output;

				parmArray[28] = new SqlParameter("@leadtime", SqlDbType.SmallInt);
				parmArray[28].Value = 0;
				parmArray[28].Direction = ParameterDirection.Output;

				parmArray[29] = new SqlParameter("@delnotebranch", SqlDbType.SmallInt);
				parmArray[29].Value = 0;
				parmArray[29].Direction = ParameterDirection.Output;

				parmArray[30] = new SqlParameter("@assemblyrequired", SqlDbType.NChar,1);
				parmArray[30].Value = "";
				parmArray[30].Direction = ParameterDirection.Output;

				parmArray[31] = new SqlParameter("@productcategory", SqlDbType.NVarChar,4);
				parmArray[31].Value = "";
				parmArray[31].Direction = ParameterDirection.Output;

                parmArray[32] = new SqlParameter("@deleted", SqlDbType.NVarChar, 1);
                parmArray[32].Value = "";
                parmArray[32].Direction = ParameterDirection.Output;

                parmArray[33] = new SqlParameter("@itemCategory", SqlDbType.SmallInt, 1);
                parmArray[33].Value = "";
                parmArray[33].Direction = ParameterDirection.Output;
            
                parmArray[34] = new SqlParameter("@costPrice", SqlDbType.Money);
                parmArray[34].Value = 0;
                parmArray[34].Direction = ParameterDirection.Output;

                parmArray[35] = new SqlParameter("@refcode", SqlDbType.NVarChar, 3); //IP - 28/01/10 - LW 72136
                parmArray[35].Value = String.Empty;
                parmArray[35].Direction = ParameterDirection.Output;

                parmArray[36] = new SqlParameter("@sparePartsCategory", SqlDbType.NVarChar, 4);
                parmArray[36].Value = String.Empty;
                parmArray[36].Direction = ParameterDirection.Output;

                parmArray[37] = new SqlParameter("@acctno", SqlDbType.VarChar, 12);
                parmArray[37].Value = string.IsNullOrEmpty(accountNo)?"":accountNo;

                parmArray[38] = new SqlParameter("@agrmtno", SqlDbType.Int);
                parmArray[38].Value = agrmtNo;

                parmArray[39] = new SqlParameter("@isInstallation", SqlDbType.Bit);       //IP - 24/02/11 - #3130
                parmArray[39].Value = 0;
                parmArray[39].Direction = ParameterDirection.Output;

                parmArray[40] = new SqlParameter("@itemID", SqlDbType.Int);               //CR1212 jec - 21/04/11
                parmArray[40].Value = itemID;

                parmArray[41] = new SqlParameter("@colourName", SqlDbType.VarChar, 12);   //CR1212 jec - 21/04/11
                parmArray[41].Value = "";
                parmArray[41].Direction = ParameterDirection.Output;

                parmArray[42] = new SqlParameter("@style", SqlDbType.VarChar, 25);        //CR1212 jec - 21/04/11
                parmArray[42].Value = "";
                parmArray[42].Direction = ParameterDirection.Output;

                parmArray[43] = new SqlParameter("@repoItem", SqlDbType.Bit);            //jec 16/06/11
                parmArray[43].Direction = ParameterDirection.Output;

                parmArray[44] = new SqlParameter("@class", SqlDbType.VarChar,3);         //IP - 27/07/11 - RI - #4415        
                parmArray[44].Direction = ParameterDirection.Output;

                parmArray[45] = new SqlParameter("@subClass", SqlDbType.VarChar, 5);     //IP - 27/07/11 - RI - #4415                
                parmArray[45].Direction = ParameterDirection.Output;

                parmArray[46] = new SqlParameter("@brand", SqlDbType.VarChar, 25);      //IP - 19/09/11 - RI - #8218 - CR8201
                parmArray[46].Value = "";
                parmArray[46].Direction = ParameterDirection.Output;

                parmArray[47] = new SqlParameter("@readyAssist", SqlDbType.Bit);        //#13716 - CR12949
                parmArray[47].Value = 0;
                parmArray[47].Direction = ParameterDirection.Output;

                parmArray[48] = new SqlParameter("@warrantyType", SqlDbType.Char);      //#17883  //#15888 
                parmArray[48].Value = "";
                parmArray[48].Direction = ParameterDirection.Output;

                parmArray[49] = new SqlParameter("@isAssembly", SqlDbType.Bit);
                parmArray[49].Value = 0;
                parmArray[49].Direction = ParameterDirection.Output;

                parmArray[50] = new SqlParameter("@isAnnualServiceContract", SqlDbType.Bit);
                parmArray[50].Value = 0;
                parmArray[50].Direction = ParameterDirection.Output;

                parmArray[51] = new SqlParameter("@isGenericService", SqlDbType.Bit);
                parmArray[51].Value = 0;
                parmArray[51].Direction = ParameterDirection.Output;

                parmArray[52] = new SqlParameter("@Addisionaltaxrate", SqlDbType.Float);
                parmArray[52].Value = 0;
                parmArray[52].Direction = ParameterDirection.Output;

               

                if (conn!=null && trans != null)
                    result = this.RunSP(conn, trans, "DN_ItemGetDetailsSP", parmArray);
				else
					result = this.RunSP("DN_ItemGetDetailsSP", parmArray);
				if(result==0)
				{
                    if (parmArray[0].Value != DBNull.Value)
                        _itemNumber = (string)parmArray[0].Value;
					if(parmArray[7].Value!=DBNull.Value)
						_stock = (double)parmArray[7].Value;
					if(parmArray[8].Value!=DBNull.Value)
						_stockdamage = (double)parmArray[8].Value;
					if(parmArray[9].Value!=DBNull.Value)
						_itemdescr1 = (string)parmArray[9].Value;
					if(parmArray[10].Value!=DBNull.Value)
						_itemdescr2 = (string)parmArray[10].Value;
					if(parmArray[11].Value!=DBNull.Value)
						_suppliercode = (string)parmArray[11].Value;
					if(parmArray[12].Value!=DBNull.Value)
						_unitprice = (double)parmArray[12].Value;
					if(parmArray[13].Value!=DBNull.Value)
						_cashprice = (double)parmArray[13].Value;
					if(parmArray[14].Value!=DBNull.Value)
						_hpprice = (double)parmArray[14].Value;
					if(parmArray[15].Value!=DBNull.Value)
						_dutyfreeprice = (double)parmArray[15].Value;
					if(parmArray[16].Value!=DBNull.Value)
						_valuecontrolled = (int)parmArray[16].Value;
					if(parmArray[17].Value!=DBNull.Value)
						_kit = (int)parmArray[17].Value;
					if(parmArray[18].Value!=DBNull.Value)
						_isstock = (int)parmArray[18].Value;
					if(parmArray[19].Value!=DBNull.Value)
						_isdiscount = (int)parmArray[19].Value;
					if(parmArray[20].Value!=DBNull.Value)
						_iswarranty = (int)parmArray[20].Value;
					if(parmArray[21].Value!=DBNull.Value)
						_taxrate = (double)parmArray[21].Value;
					if(parmArray[22].Value!=DBNull.Value)
						_isaffinity = (int)parmArray[22].Value;
					if(parmArray[23].Value!=DBNull.Value)
						_isStampDuty = Convert.ToBoolean(parmArray[23].Value);
					if(parmArray[24].Value!=DBNull.Value)
						_isFreeGift = Convert.ToBoolean(parmArray[24].Value);
					if(parmArray[27].Value!=DBNull.Value)
						_qtyonorder = (double)parmArray[27].Value;
					if(parmArray[28].Value!=DBNull.Value)
						_leadtime = (short)parmArray[28].Value;
					if(parmArray[29].Value!=DBNull.Value)
						_delnotebranch = (short)parmArray[29].Value;
					if(parmArray[30].Value!=DBNull.Value)
						_assemblyrequired = (string)parmArray[30].Value;
					if(parmArray[31].Value!=DBNull.Value)
						_productcategory = (string)parmArray[31].Value;
                    if (parmArray[32].Value != DBNull.Value)
                        _deleted = (string)parmArray[32].Value;
                    if (parmArray[33].Value != DBNull.Value)
                        _category = (short)parmArray[33].Value;
                    if (parmArray[34].Value != DBNull.Value)
                        _costprice = (decimal)parmArray[34].Value;
                    if (parmArray[35].Value != DBNull.Value) //IP - 28/01/10 - LW 72136
                        _refcode = Convert.ToString(parmArray[35].Value);
                    if (parmArray[36].Value != DBNull.Value)
                        SparePartsCategory = parmArray[36].Value.ToString();
                    if (parmArray[39].Value != DBNull.Value)                        //IP - 24/02/11 - #3130
                        IsInstallation = Convert.ToBoolean(parmArray[39].Value);
                    if (parmArray[41].Value != DBNull.Value)                        //CR1212 jec - 21/04/11
                        ColourName = Convert.ToString(parmArray[41].Value);
                    if (parmArray[42].Value != DBNull.Value)                        //CR1212 jec - 21/04/11
                        Style = Convert.ToString(parmArray[42].Value);
                    if (parmArray[43].Value != DBNull.Value)                //jec 16/06/11
                        this.RepoItem = Convert.ToBoolean(parmArray[43].Value);
                    if (parmArray[49].Value != DBNull.Value)                       
                        isAssemblyCost = Convert.ToBoolean(parmArray[49].Value);
                    if (parmArray[50].Value != DBNull.Value)
                        isAnnualServiceContract = Convert.ToBoolean(parmArray[50].Value);
                    if (parmArray[51].Value != DBNull.Value)
                        isGenericService = Convert.ToBoolean(parmArray[51].Value);

                    if (parmArray[52].Value != DBNull.Value)    //BCX - ISSUE : This is used for LUX tax for curacao 
                        _Additionaltaxrate = (double)parmArray[52].Value;

                    this.Class = parmArray[44].Value != DBNull.Value ? Convert.ToString(parmArray[44].Value) : "";          //IP - 27/07/11 - RI - #4415

                    this.SubClass = parmArray[45].Value != DBNull.Value ? Convert.ToString(parmArray[45].Value) : "";       //IP - 27/07/11 - RI - #4415

                    this.Brand = parmArray[46].Value != DBNull.Value ? Convert.ToString(parmArray[46].Value) : "";          //IP - 19/09/11 - RI - #8218 - CR8201

                    this.ReadyAssist = parmArray[47].Value !=DBNull.Value ? Convert.ToBoolean(parmArray[47].Value) : false; //#13716 - CR12949

                    this.WarrantyType = parmArray[48].Value != DBNull.Value ? Convert.ToString(parmArray[48].Value) : "";    //#17883 //#15888

                    result = (int)Return.Success;
				}
				else
					result = (int)Return.Fail;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

		public int GetStockItemTranslations(SqlConnection conn, SqlTransaction trans,
			                                string itemno, string descr1_en, string descr1,
											string descr2_en, string descr2)
		{
			try
			{
				_translations = new DataTable("Translations");

				parmArray = new SqlParameter[5];
				parmArray[0] = new SqlParameter("@itemno", SqlDbType.NVarChar,8);
				parmArray[0].Value = itemno;
				parmArray[1] = new SqlParameter("@descr1_en", SqlDbType.NVarChar,25);
				parmArray[1].Value = descr1_en;
				parmArray[2] = new SqlParameter("@descr1", SqlDbType.NVarChar,25);
				parmArray[2].Value = descr1;
				parmArray[3] = new SqlParameter("@descr2_en", SqlDbType.NVarChar,40);
				parmArray[3].Value = descr2_en;
				parmArray[4] = new SqlParameter("@descr2", SqlDbType.NVarChar,40);
				parmArray[4].Value = descr2;

				result = this.RunSP("DN_GetStockItemTransSP", parmArray, _translations);

				if(result==0)
					result = (int)Return.Success;
				else
					result = (int)Return.Fail;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

		public int SetStockItemTranslation(SqlConnection conn, SqlTransaction trans,
			                               string itemno, string description1, string description2)
		{
			try
			{
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@itemno", SqlDbType.NVarChar,8);
				parmArray[0].Value = itemno;
				parmArray[1] = new SqlParameter("@descr1", SqlDbType.NVarChar,25);
				parmArray[1].Value = description1;
				parmArray[2] = new SqlParameter("@descr2", SqlDbType.NVarChar,40);
				parmArray[2].Value = description2;
				
				result = this.RunSP(conn, trans, "DN_UpdateStockItemTransSP", parmArray);

				if(result==0)
					result = (int)Return.Success;
				else
					result = (int)Return.Fail;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

		public int MaintainStockLevel(SqlConnection conn, SqlTransaction trans,
                                            string accountNo, int itemID,                   //IP - 20/05/11 - CR1212 - RI - #3664
											short stockLocn, double quantity,
											int agreementNo)
		{
			try
			{
				parmArray = new SqlParameter[5];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNo;
                parmArray[1] = new SqlParameter("@itemID", SqlDbType.Int);                  //IP - 20/05/11 - CR1212 - RI - #3664
				parmArray[1].Value = itemID;
				parmArray[2] = new SqlParameter("@stocklocn", SqlDbType.SmallInt);
				parmArray[2].Value = stockLocn;
				parmArray[3] = new SqlParameter("@quantity", SqlDbType.Float);
				parmArray[3].Value = quantity;
				parmArray[4] = new SqlParameter("@agreementno", SqlDbType.Int);
				parmArray[4].Value = agreementNo;
				
				this.RunSP(conn, trans, "DN_StockItemMaintainStockSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

        public int GetItemCount(int itemId, short location)         // RI jec 19/05/11
		{
			int rowCount = 0;
			try
			{
				parmArray = new SqlParameter[3];
				//parmArray[0] = new SqlParameter("@productcode", SqlDbType.NVarChar,8);
                parmArray[0] = new SqlParameter("@itemId", SqlDbType.Int);          // RI jec 19/05/11
                parmArray[0].Value = itemId;
				parmArray[1] = new SqlParameter("@stocklocn", SqlDbType.SmallInt);
				parmArray[1].Value = location;
				parmArray[2] = new SqlParameter("@rowcount", SqlDbType.Int);
				parmArray[2].Value = rowCount;
				parmArray[2].Direction = ParameterDirection.Output;

				result = this.RunSP("DN_StockCountItemSP", parmArray);
				if(DBNull.Value!=parmArray[2].Value)
					rowCount = (int)parmArray[2].Value;

			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return rowCount;
		}


		public DataTable ByLocation
		{
			get
			{
				return _bylocation;
			}
		}

        public DataTable TopSelling
        {
            get
            {
                return _topSelling;

            }
        }

		public DataTable ByCode
		{
			get
			{
				return _bycode;
			}
		}

		public DataTable Translations
		{
			get
			{
				return _translations;
			}
		}

        


		public DStockItem()
		{

		}

		/// <summary>
		/// IsItemInstantReplacement
		/// </summary>
		/// <param name="itemno">string</param>
		/// <param name="branchno">int</param>
		/// <param name="instant">int</param>
		/// <returns>bool</returns>
		/// 
		public bool IsItemInstantReplacement (SqlConnection conn, SqlTransaction trans, int itemId, int branchno)
		{
			short instant = 0;
			
			try
			{
				parmArray = new SqlParameter[3];
				
				parmArray[0] = new SqlParameter("@itemId", SqlDbType.Int);
                parmArray[0].Value = itemId;
				
				parmArray[1] = new SqlParameter("@branchno", SqlDbType.SmallInt);
				parmArray[1].Value = branchno;
				
				parmArray[2] = new SqlParameter("@instant", SqlDbType.SmallInt);
				parmArray[2].Value = instant;
				parmArray[2].Direction = ParameterDirection.Output;
				 
				if(conn!=null && trans!=null)
					this.RunSP(conn, trans, "DN_StockItemIsInstantReplacementSP", parmArray);
				else
					this.RunSP("DN_StockItemIsInstantReplacementSP", parmArray);
	
				if(parmArray[2].Value!=DBNull.Value)
					instant = (short)parmArray[2].Value;				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}

			return instant > 0;
		}

		public int GetItemsInRegion(int itemId, short branchNo)
		{
			try
			{
				_stocklocs = new DataTable(TN.StockLocation);

				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@itemId", SqlDbType.Int);
                parmArray[0].Value = itemId;
				parmArray[1] = new SqlParameter("@branchno", SqlDbType.SmallInt);
				parmArray[1].Value = branchNo;

				result = this.RunSP("DN_ItemGetInRegionSP", parmArray, _stocklocs);
				if(result==0)
					result = (int)Return.Success;
				else
					result = (int)Return.Fail;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

		public int GetPurchaseOrders(int itemId, short branchNo)
		{
			try
			{
				_bycode = new DataTable("ByCode");

				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@itemId", SqlDbType.Int);
                parmArray[0].Value = itemId;
				parmArray[1] = new SqlParameter("@branchno", SqlDbType.SmallInt);
				parmArray[1].Value = branchNo;

				result = this.RunSP("DN_ItemGetPurchaseOrdersSP", parmArray, _bycode);
				if(result==0)
					result = (int)Return.Success;
				else
					result = (int)Return.Fail;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

		public int MaintainPurchaseOrderStockLevel(SqlConnection conn, SqlTransaction trans,
			string accountNo, int itemId, short stockLocn, double quantity,
            int agreementNo, string purchaseOrderNumber)
		{
			try
			{
				parmArray = new SqlParameter[6];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@itemId", SqlDbType.Int);
                parmArray[1].Value = itemId;
				parmArray[2] = new SqlParameter("@stocklocn", SqlDbType.SmallInt);
				parmArray[2].Value = stockLocn;
				parmArray[3] = new SqlParameter("@quantity", SqlDbType.Float);
				parmArray[3].Value = quantity;
				parmArray[4] = new SqlParameter("@agreementno", SqlDbType.Int);
				parmArray[4].Value = agreementNo;
                parmArray[5] = new SqlParameter("@purchaseordernumber", SqlDbType.NVarChar,12);
                parmArray[5].Value = purchaseOrderNumber;
				
				this.RunSP(conn, trans, "DN_PurchaseOrderMaintainStockSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

		public void LockItem(SqlConnection conn, SqlTransaction trans, string itemNo, 
							short stockLocn)
		{
			try
			{
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@itemno", SqlDbType.NVarChar, 10);
				parmArray[0].Value = itemNo;
				parmArray[1] = new SqlParameter("@stocklocn", SqlDbType.SmallInt);
				parmArray[1].Value = stockLocn;
				parmArray[2] = new SqlParameter("@user", SqlDbType.Int);
				parmArray[2].Value = this.User;
				this.RunSP(conn, trans, "DN_LockItemSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

	public void UnlockItem(SqlConnection conn, SqlTransaction trans)
		{
			try
			{
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@user", SqlDbType.Int);
				parmArray[0].Value = this.User;
				this.RunSP(conn, trans, "DN_UnlockItemSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

        public void ProductDataLoad(SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                this.RunSP(conn, trans, "DN_EODProductDataLoadSP");
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void NonStockProductDataLoad(SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                this.RunSP(conn, trans, "DN_EODNonStockProductDataLoadSP");
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }
        
        public void ProductImport(SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                this.RunSP(conn, trans, "DN_EODProductImportSP");
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void NonStockProductImport(SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                this.RunSP(conn, trans, "DN_EODNonStockProductImportSP");
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }
        
        public void KitProductDataLoad(SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                this.RunSP(conn, trans, "DN_EODKitProductDataLoadSP");
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }
        
        public void KitProductImport(SqlConnection conn, SqlTransaction trans,
                                     string eodOption, int runNo)
        {
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@interface", SqlDbType.NVarChar, 10);
                parmArray[0].Value = eodOption;
                parmArray[1] = new SqlParameter("@runno", SqlDbType.Int);
                parmArray[1].Value = runNo;
                this.RunSP(conn, trans, "DN_EODKitProductImportSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }
        
        public void PromoPriceDataLoad(SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                this.RunSP(conn, trans, "DN_EODPromoDataLoadSP");
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void NonStockPromoPriceDataLoad(SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                this.RunSP(conn, trans, "DN_EODNonStockPromoDataLoadSP");
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void PromoPriceImport(SqlConnection conn, SqlTransaction trans,
                                     string eodOption, int runNo)
        {
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@interface", SqlDbType.NVarChar, 10);
                parmArray[0].Value = eodOption;
                parmArray[1] = new SqlParameter("@runno", SqlDbType.Int);
                parmArray[1].Value = runNo;
                this.RunSP(conn, trans, "DN_EODPromoPriceImportSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void NonStockPromoPriceImport(SqlConnection conn, SqlTransaction trans,
                                 string eodOption, int runNo)
        {
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@interface", SqlDbType.NVarChar, 10);
                parmArray[0].Value = eodOption;
                parmArray[1] = new SqlParameter("@runno", SqlDbType.Int);
                parmArray[1].Value = runNo;
                this.RunSP(conn, trans, "DN_EODNonStockPromoPriceImportSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void StockQtyDataLoad(SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                this.RunSP(conn, trans, "DN_EODStockQtyDataLoadSP");
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void StockQtyImport(SqlConnection conn, SqlTransaction trans,
                                   string eodOption, int runNo)
        {
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@interface", SqlDbType.NVarChar, 10);
                parmArray[0].Value = eodOption;
                parmArray[1] = new SqlParameter("@runno", SqlDbType.Int);
                parmArray[1].Value = runNo;
                this.RunSP(conn, trans, "DN_EODStockQtyImportSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void AssocProductDataLoad(SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                this.RunSP(conn, trans, "DN_EODAssocProductDataLoadSP");
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void AssociatedProductDataLoad(SqlConnection conn, SqlTransaction trans, string source)
        {
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@source", SqlDbType.NVarChar, 20);
                parmArray[0].Value = source;
                this.RunSP(conn, trans, "DN_EODAssociatedProductDataLoadSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void AssocProductImport(SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                this.RunSP(conn, trans, "DN_EODAssocProductImportSP");
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void AssociatedProductImport(SqlConnection conn, SqlTransaction trans, string source)
        {
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@source", SqlDbType.NVarChar, 20);
                parmArray[0].Value = source;

                this.RunSP(conn, trans, "DN_EODAssociatedProductImportSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void TranslateStockItems(SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                this.RunSP(conn, trans, "DN_TranslateStockItemsORSP");
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void RemoveRefCodeCR(SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                this.RunSP(conn, trans, "DN_RemoveRefCodeCRSP");
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public bool IsDiscountItem(string itemNo)
        {
            int isDiscount = 0;
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@itemno", SqlDbType.NChar, 18);
                parmArray[0].Value = itemNo;
                parmArray[1] = new SqlParameter("@isdiscount", SqlDbType.Int);
                parmArray[1].Direction = ParameterDirection.Output;

                RunSP("DN_ItemIsDiscountSP", parmArray);

                if (!Convert.IsDBNull(parmArray[1].Value))
                    isDiscount = (int)parmArray[1].Value;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return (isDiscount > 0);
        }

        public int GetTopSellingCashandGo(short branchCode)
        {
            try
            {
                _topSelling = new DataTable("TopSelling");

                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@branchno", SqlDbType.SmallInt);
                parmArray[0].Value = branchCode;

                result = this.RunSP("GetTopSellingCashandGo", parmArray, _topSelling);
                if (result == 0)
                    result = (int)Return.Success;
                else
                    result = (int)Return.Fail;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;
        }

        //CR1094 jec 09/12/10 - get Non stock items
        public DataSet GetNonStockByCode(string itemNo) 
        {
            try
            {
                _stockItem = new DataSet();

                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@itemNo", SqlDbType.VarChar, 8);
                parmArray[0].Value = itemNo;

                this.RunSP("NonStockGetDetailsSP", parmArray, _stockItem);

                return _stockItem;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        //CR1094 jec 09/12/10 - get all categories
        public DataTable GetCategories()
        {
            try
            {
                _categories = new DataTable(TN.Categories);

                this.RunSP("ProductCategoryGetSP", _categories);

                return _categories;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        //CR1094 jec 09/12/10 - save Non Stock Item
        public void SaveNonStockItem()
        {
            try
            {                
                parmArray = new SqlParameter[10];
                parmArray[0] = new SqlParameter("@itemNo", SqlDbType.VarChar, 8);
                parmArray[0].Value = _itemNumber;
                parmArray[1] = new SqlParameter("@itemdescr1", SqlDbType.VarChar, 25);
                parmArray[1].Value = _itemDescr1;
                parmArray[2] = new SqlParameter("@itemdescr2", SqlDbType.VarChar, 40);
                parmArray[2].Value = _itemDescr2;
                parmArray[3] = new SqlParameter("@suppliername", SqlDbType.VarChar, 40);
                parmArray[3].Value = _itemSupplierName;
                parmArray[4] = new SqlParameter("@suppliercode", SqlDbType.VarChar, 18);
                parmArray[4].Value = _itemSupplierCode;
                parmArray[5] = new SqlParameter("@category", SqlDbType.Int);
                parmArray[5].Value = _itemCategory;
                parmArray[6] = new SqlParameter("@taxrate", SqlDbType.Float);
                parmArray[6].Value = _itemTaxRate;                
                parmArray[7] = new SqlParameter("@deleted", SqlDbType.Char, 1);
                parmArray[7].Value = _deleted;
                parmArray[8] = new SqlParameter("@endDate", SqlDbType.DateTime);
                parmArray[8].Value = _deletionDate;
                parmArray[9] = new SqlParameter("@itemId", SqlDbType.Int);          // RI
                parmArray[9].Value = _itemId;
                this.RunSP("NonStockSaveDetailsSP", parmArray);

                
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        //CR1094 jec 08/02/10 - save Non Stock Price
        public void SaveNonStockPrice()
        {
            try
            {
                parmArray = new SqlParameter[8];
                parmArray[0] = new SqlParameter("@itemNo", SqlDbType.VarChar, 8);
                parmArray[0].Value = _itemNumber;
                parmArray[1] = new SqlParameter("@branchno", SqlDbType.Int);
                parmArray[1].Value = _branchNo; 
                parmArray[2] = new SqlParameter("@hpprice", SqlDbType.Money);
                parmArray[2].Value = _unitHPPrice;
                parmArray[3] = new SqlParameter("@cashprice", SqlDbType.Money);
                parmArray[3].Value = _unitCashPrice;
                parmArray[4] = new SqlParameter("@dutyfreeprice", SqlDbType.Money);
                parmArray[4].Value = _unitDutyFreePrice;
                parmArray[5] = new SqlParameter("@costprice", SqlDbType.Money);
                parmArray[5].Value = _unitCostPrice;
                parmArray[6] = new SqlParameter("@startDate", SqlDbType.DateTime);
                parmArray[6].Value = _deletionDate;
                parmArray[7] = new SqlParameter("@itemId", SqlDbType.Int);          // RI
                parmArray[7].Value = _itemId;
                
                this.RunSP("NonStockSavePricesSP", parmArray);


            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        //CR1094 jec 09/12/10 - get Warranty Return codes
        public DataTable GetWarrantyReturnCodes()
        {
            try
            {
                _returnCodes = new DataTable(TN.Warranties);
                
                this.RunSP("WarrantyReturnCodeGetDetailsSP", _returnCodes);

                return _returnCodes;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        //CR1094 jec 16/12/10 - save Warranty Codes
        public void SaveWarrantyReturnCodes()
        {
            try
            {
                parmArray = new SqlParameter[8];
                parmArray[0] = new SqlParameter("@productType", SqlDbType.Char, 1);
                parmArray[0].Value = _productType;
                parmArray[1] = new SqlParameter("@category", SqlDbType.VarChar, 20);
                parmArray[1].Value = _warrCategory;
                parmArray[2] = new SqlParameter("@returnCode", SqlDbType.VarChar, 8);
                parmArray[2].Value = _returnCode;
                parmArray[3] = new SqlParameter("@warrantyMonths", SqlDbType.Int);
                parmArray[3].Value = _warrantyMonths;
                parmArray[4] = new SqlParameter("@manufactMonths", SqlDbType.Int);
                parmArray[4].Value = _manufactMonths;
                parmArray[5] = new SqlParameter("@expiredMonths", SqlDbType.Int);
                parmArray[5].Value = _expiredMonths;
                parmArray[6] = new SqlParameter("@refundPct", SqlDbType.Float);
                parmArray[6].Value = _refundPct;
                parmArray[7] = new SqlParameter("@dateNow", SqlDbType.DateTime);
                parmArray[7].Value = _dateNow;

                this.RunSP("WarrantyReturnCodeSaveDetailsSP", parmArray);

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        //CR1094 jec 23/12/10 - get All Warranty Items
        public DataTable GetAllWarrantyItems()
        {
            try
            {
                _warrantyItems = new DataTable(TN.Warranties);

                this.RunSP("WarrantyItemsGetAllDetailsSP", _warrantyItems);

                return _warrantyItems;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        //CR1212 jec 09/06/11 - get details
        public DataSet ProductAssociationGetDetails()
        {
            try
            {
                _dropDown = new DataSet();

                this.RunSP("ProductAssociationGetDetailsSP", _dropDown);

                return _dropDown;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        //CR1212 jec 09/06/11 - get details
        public void ProductAssociationSaveDetails()
        {
            try
            {
                parmArray = new SqlParameter[6];
                parmArray[0] = new SqlParameter("@productGroup", SqlDbType.VarChar, 3);
                parmArray[0].Value = _productGroup;
                parmArray[1] = new SqlParameter("@category", SqlDbType.Int);
                parmArray[1].Value = _category;
                parmArray[2] = new SqlParameter("@class", SqlDbType.VarChar, 3);
                parmArray[2].Value = _class;
                parmArray[3] = new SqlParameter("@subclass", SqlDbType.VarChar, 5);
                parmArray[3].Value = _subClass;
                parmArray[4] = new SqlParameter("@itemid", SqlDbType.Int);
                parmArray[4].Value = _itemId;
                parmArray[5] = new SqlParameter("@delete", SqlDbType.Char, 1);
                parmArray[5].Value = _deleted;


                this.RunSP("ProductAssociationSaveDetailsSP", parmArray);

                
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public int OnlineProductSearch(string location, short category, string online, DateTime dateAdded, DateTime dateRemoved, string prodDesc, int limit)
        {
            try
            {
                _bylocation = new DataTable("ByLocation");

                parmArray = new SqlParameter[7];
                parmArray[0] = new SqlParameter("@location", SqlDbType.VarChar, 3);
                parmArray[0].Value = location;
                parmArray[1] = new SqlParameter("@category", SqlDbType.SmallInt);
                parmArray[1].Value = category;
                parmArray[2] = new SqlParameter("@online", SqlDbType.Char,1);
                parmArray[2].Value = online;
                parmArray[3] = new SqlParameter("@dateAdded", SqlDbType.DateTime);
                parmArray[3].Value = dateAdded;
                parmArray[4] = new SqlParameter("@dateRemoved", SqlDbType.DateTime);
                parmArray[4].Value = dateRemoved;
                parmArray[5] = new SqlParameter("@prodDesc", SqlDbType.NVarChar, 80);
                parmArray[5].Value = String.Format(@"""{0}""", prodDesc);  //Enclosed by double quotes for FTS Fulltext function;
                parmArray[6] = new SqlParameter("@limit", SqlDbType.Int);
                parmArray[6].Value = limit;

                result = this.RunSP("OnlineProductSearchSP", parmArray, _bylocation);
                if (result == 0)
                    result = (int)Return.Success;
                else
                    result = (int)Return.Fail;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;
        }
	}
}
