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
	/// Summary description for DTermsType.
	/// </summary>
	public class DTermsType : DALObject
	{
        DataTable _termstypes;
        DataTable _termsTypeBands;
        DataTable _termsdetails;
		DataTable _variablerates;

		private string _countryCode = "";
		public string CountryCode
		{
			get{return _countryCode;}
			set{_countryCode = value;}
		}

		private string _termsType = "";
		public string TermsType
		{
			get{return _termsType;}
			set{_termsType = value;}
		}

		private string _description = "";
		public string Description
		{
			get{return _description;}
			set{_description = value;}
		}

		private short _mthintfree = 0;
		public short MonthInterestFree
		{
			get{return _mthintfree;}
			set{_mthintfree = value;}
		}

		private string _depositpaid = "N";
		public string DepositPaid
		{
			get{return _depositpaid;}
			set{_depositpaid = value;}
		}

		private string _instalpredel = "N";
		public string InstalPreDelivery
		{
			get{return _instalpredel;}
			set{_instalpredel = value;}
		}

		private string _dtnetfirstin = "N";
		public string DotNetFirstIn
		{
			get{return _dtnetfirstin;}
			set{_dtnetfirstin = value;}
		}

		private string _affinity = "N";
		public string Affinity
		{
			get{return _affinity;}
			set{_affinity = value;}
		}

		private int _minterm = 0;
		public int MinTerm
		{
			get{return _minterm;}
			set{_minterm = value;}
		}

		private int _maxterm = 0;
		public int MaxTerm
		{
			get{return _maxterm;}
			set{_maxterm = value;}
		}

		private string _agrtext = "";
		public string AgreementText
		{
			get{return _agrtext;}
			set{_agrtext = value;}
		}

		private int _agrtextx = 0;
		public int AgreementTextX
		{
			get{return _agrtextx;}
			set{_agrtextx = value;}
		}

		private int _agrtexty = 0;
		public int AgreementTextY
		{
			get{return _agrtexty;}
			set{_agrtexty = value;}
		}

		private short _noarrearsletters = 0;
		public short NoArrearsLetters
		{
			get{return _noarrearsletters;}
			set{_noarrearsletters = value;}
		}
		

		private short _cashBackMonth;
		public short CashBackMonth
		{
			get{return _cashBackMonth;}
			set{_cashBackMonth = value;}
		}
		
		private short _cashBackPc;
		public short CashBackPc
		{
			get{return _cashBackPc;}
			set{_cashBackPc = value;}
		}

		private decimal _cashBackAmount;
		public decimal CashBackAmount
		{
			get{return _cashBackAmount;}
			set{_cashBackAmount = value;}
		}

		private string _agreementPrint;
		public string AgreementPrint
		{
			get{return _agreementPrint;}
			set{_agreementPrint = value;}
		}

		private short _deferredMonths;
		public short DeferredMonths
		{
			get{return _deferredMonths;}
			set{_deferredMonths = value;}
		}
		
		private short _fullRebateDays;
		public short FullRebateDays
		{
			get{return _fullRebateDays;}
			set{_fullRebateDays = value;}
		}

		private short _STCPc;
		public short STCPc
		{
			get{return _STCPc;}
			set{_STCPc = value;}
		}

		private decimal _STCAmount;
		public decimal STCAmount
		{
			get{return _STCAmount;}
			set{_STCAmount = value;}
		}
		

		private double _defaultdeposit = 0;
		public double DefaultDeposit
		{
			get{return _defaultdeposit;}
			set{_defaultdeposit = value;}
		}

		private int _defaultterm = 0;
		public int DefaultTerm
		{
			get{return _defaultterm;}
			set{_defaultterm = value;}
		}

		private string _apr = "";
		public string APR
		{
			get{return _apr;}
			set{_apr = value;}
		}

        private short _donotsecuritise;
        public short DoNotSecuritise
        {
            get { return _donotsecuritise; }
            set { _donotsecuritise = value; }
        }
        
         private string _storeType = String.Empty;
      public string StoreType
      {
         get
         {
            return _storeType;
         }
         set
         {
            _storeType = value;
         }
      }


		//private string _pClubTier1 = "N";
		//public string pClubTier1
		//{
		//	get{return _pClubTier1;}
		//	set{_pClubTier1 = value;}
		//}

		//private string _pClubTier2 = "N";
		//public string pClubTier2
		//{
		//	get{return _pClubTier2;}
		//	set{_pClubTier2 = value;}
		//}

		private short _isactive = 0;
		public short IsActive
		{
			get{return _isactive;}
			set{_isactive = value;}
		}

		private short _hasfreeinstallments = 0;
		public short HasFreeInstallments
		{
			get{return _hasfreeinstallments;}
			set{_hasfreeinstallments = value;}
		}

		private DateTime _datefrom;
		public DateTime DateFrom
		{
			get{return _datefrom;}
			set{_datefrom = value;}
		}

		private DateTime _dateto;
		public DateTime DateTo
		{
			get{return _dateto;}
			set{_dateto = value;}
		}

		private double _servpcent = 0;
		public double ServicePercent
		{
			get{return _servpcent;}
			set{_servpcent = value;}
		}

		private double _inspcent = 0;
		public double InsurancePercent
		{
			get{return _inspcent;}
			set{_inspcent = value;}
		}

		private double _adminpcent = 0;
		public double AdminPercent
		{
			get{return _adminpcent;}
			set{_adminpcent = value;}
		}

		private short _insincluded = 0;
		public short InsuranceIncluded
		{
			get{return _insincluded;}
			set{_insincluded = value;}
		}

		private short _includewarranty = 0;
		public short IncludeWarranty
		{
			get{return _includewarranty;}
			set{_includewarranty = value;}
		}

		private bool _depispercent = false;
		public bool DepositIsPercentage
		{
			get{return _depispercent;}
			set{_depispercent = value;}
		}

		private bool _paymentholidays = false;
		public bool PaymentHolidays
		{
			get{return _paymentholidays;}
			set{_paymentholidays = value;}
		}

		private bool _delNonStocks = false;
		public bool DelNonStocks
		{
			get{return _delNonStocks;}
			set{_delNonStocks = value;}
		}
  // CR906 
        private bool _isLoan = false;
        public bool IsLoan
        {
            get { return _isLoan; }
            set { _isLoan = value; }
        }

        private bool _loanNewCustomer = false;
        public bool LoanNewCustomer
        {
            get { return _loanNewCustomer; }
            set { _loanNewCustomer = value; }
        }

        private bool _loanRecentCustomer = false;
        public bool LoanRecentCustomer
        {
            get { return _loanRecentCustomer; }
            set { _loanRecentCustomer = value; }
        }

        private bool _loanExistingCustomer = false;
        public bool LoanExistingCustomer
        {
            get { return _loanExistingCustomer; }
            set { _loanExistingCustomer = value; }
        }
        
        private bool _loanStaff = false;
        public bool LoanStaff
        {
            get { return _loanStaff; }
            set { _loanStaff = value; }
        }

        private bool _isMmiActive = false;
        public bool IsMmiActive
        {
            get { return _isMmiActive; }
            set { _isMmiActive = value; }
        }


        private double _mmiThresholdPercentage = 0;
        public double MmiThresholdPercentage
        {
            get { return _mmiThresholdPercentage; }
            set { _mmiThresholdPercentage = value; }
        }


        public int GetTermsTypeSummary(XmlNode parms)
		{
			try
			{
				_termstypes = new DataTable(TN.TermsType);

				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@countryCode", SqlDbType.NVarChar,2);
				parmArray[0].Value = parms.FirstChild.Attributes[Tags.Value].Value;

				result = this.RunSP("DN_TermsTypeSummaryGetSP", parmArray, _termstypes);
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

        public int GetTermsTypeBands(XmlNode parms)
        {
            try
            {
                _termsTypeBands = new DataTable(TN.TermsTypeBand);

                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@CountryCode", SqlDbType.Char,2);
                parmArray[0].Value = parms.FirstChild.Attributes[Tags.Value].Value;

                result = this.RunSP("DN_TermsTypeBandsGetSP", parmArray, _termsTypeBands);
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

        public DataTable GetTermsTypeBandsOverview()
        {
            DataTable termsTypeBandsOverview = new DataTable();
            try
            {
                result = this.RunSP("DN_TermsTypeBandsOverviewSP", termsTypeBandsOverview);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return termsTypeBandsOverview;
        }


        public void TermsTypeBandsAdjust(DateTime adjustDate, decimal adjustIns, decimal adjustSC, int user)
        {
            try
            {
                parmArray = new SqlParameter[4];
                parmArray[0] = new SqlParameter("@AdjustDate", SqlDbType.SmallDateTime);
                parmArray[0].Value = adjustDate;
                parmArray[1] = new SqlParameter("@AdjustIns", SqlDbType.Float);
                parmArray[1].Value = adjustIns;
                parmArray[2] = new SqlParameter("@AdjustSC", SqlDbType.Float);
                parmArray[2].Value = adjustSC;
                parmArray[3] = new SqlParameter("@EmpeeNo", SqlDbType.Int);
                parmArray[3].Value = user;
                
                result = this.RunSP("DN_TermsTypeBandsAdjustSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }


		public int GetTermsTypeDetail(SqlConnection conn, SqlTransaction trans, string countryCode, string termsType, string acctNo, string overrideBand, DateTime dateOpened)
		{
			try
			{
				_termsdetails = new DataTable("TermsDetails");

				parmArray = new SqlParameter[5];
				parmArray[0] = new SqlParameter("@countryCode", SqlDbType.NVarChar,2);
				parmArray[0].Value = countryCode;
				parmArray[1] = new SqlParameter("@termstype", SqlDbType.NVarChar,2);
				parmArray[1].Value = termsType;
                parmArray[2] = new SqlParameter("@acctNo", SqlDbType.NChar, 12);
                parmArray[2].Value = acctNo;
                parmArray[3] = new SqlParameter("@overrideBand", SqlDbType.NVarChar, 4);
                parmArray[3].Value = overrideBand;
                parmArray[4] = new SqlParameter("@dateOpened", SqlDbType.DateTime);
				parmArray[4].Value = dateOpened;


				if(conn!=null&&trans!=null)
					result = this.RunSP(conn, trans, "DN_TermsTypeDetailsGetSP", parmArray, _termsdetails);
				else
					result = this.RunSP("DN_TermsTypeDetailsGetSP", parmArray, _termsdetails);
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


        public DataTable TermsTypeGetDetails(string acctNo)
		{
          try
			{
				_termsdetails = new DataTable("TermsDetails");

				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar,12);
				parmArray[0].Value = acctNo;


                result = this.RunSP("TermsTypeGetDetailsbyAcctno", parmArray, _termsdetails);
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}

          return _termsdetails;
		}
        
		public DataTable TermsTypeDetails
		{
			get
			{
				return _termsdetails;
			}
		}


        public DataTable TermsTypes
        {
            get
            {
                return _termstypes;
            }
        }

        public DataTable TermsTypeBands
        {
            get
            {
                return _termsTypeBands;
            }
        }

        public DataTable VariableRates
		{
			get{return _variablerates;}
		}

		public DTermsType()
		{

		}

		/// <summary>
		/// GetDefault
		/// </summary>
		/// <param name="termstype">string</param>
		/// <returns>string</returns>
		/// 
        // CR903 jec 22/08/07
        // CR906 rdb 06/09/07 - added IsLoan param so default TermsType for loan can be accessed
        public string GetDefault(SqlConnection conn, SqlTransaction trans, short branchNo, bool isLoan) 
		{			
			string termstype = "";			
			try
			{
				parmArray = new SqlParameter[3];
				
				parmArray[0] = new SqlParameter("@termstype", SqlDbType.NVarChar, 4);
				parmArray[0].Value = termstype;
				parmArray[0].Direction = ParameterDirection.Output; 
                parmArray[1] = new SqlParameter("@pibranchNo", SqlDbType.SmallInt);     // CR903 jec 22/08/07
                parmArray[1].Value = branchNo;
                parmArray[2] = new SqlParameter("@IsLoan", SqlDbType.Bit);
                parmArray[2].Value = isLoan;
				
				if(conn!=null && trans!=null)
					this.RunSP(conn, trans, "DN_TermsTypeGetDefaultSP", parmArray);
				else
					this.RunSP("DN_TermsTypeGetDefaultSP", parmArray);
	
				if(parmArray[0].Value!=DBNull.Value)
					termstype = (string)parmArray[0].Value;
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return termstype;
		}

		public DataSet LoadTermsTypeDetails (string termstype)
		{			
			DataSet ds = new DataSet();			
			try
			{
				parmArray = new SqlParameter[1];
				
				parmArray[0] = new SqlParameter("@termstype", SqlDbType.NVarChar, 4);
				parmArray[0].Value = termstype;
                
				this.RunSP("DN_TermsTypeLoadDetailsSP", parmArray, ds);	
			
				ds.Tables[0].TableName = TN.TermsType;
				ds.Tables[1].TableName = TN.TermsTypeAccountType;
				ds.Tables[2].TableName = TN.IntRateHistory;
				ds.Tables[3].TableName = TN.TermsTypeLength;
				ds.Tables[4].TableName = TN.TermsTypeFreeInstallments;
				ds.Tables[5].TableName = TN.TermsTypeVariableRates;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return ds;
		}

		public void SaveTermsTypeDetails(SqlConnection conn, SqlTransaction trans, int user)
		{
			try
			{
				parmArray = new SqlParameter[35];               // CR906
				
				parmArray[0] = new SqlParameter("@termstype", SqlDbType.VarChar, 4);
				parmArray[0].Value = TermsType;

				parmArray[1] = new SqlParameter("@description", SqlDbType.VarChar, 20);
				parmArray[1].Value = this.Description;

				parmArray[2] = new SqlParameter("@mthsintfree", SqlDbType.SmallInt);
				parmArray[2].Value = this.MonthInterestFree;

				parmArray[3] = new SqlParameter("@instalpredel", SqlDbType.Char, 1);
				parmArray[3].Value = this.InstalPreDelivery;

				parmArray[4] = new SqlParameter("@affinity", SqlDbType.Char, 1);
				parmArray[4].Value = this.Affinity;

				parmArray[5] = new SqlParameter("@noarrearsletter", SqlDbType.SmallInt);
				parmArray[5].Value = this.NoArrearsLetters;

				parmArray[6] = new SqlParameter("@defaultdeposit", SqlDbType.Float);
				parmArray[6].Value = this.DefaultDeposit;

				parmArray[7] = new SqlParameter("@depositispercentage", SqlDbType.Bit);
				parmArray[7].Value = this.DepositIsPercentage;

				parmArray[8] = new SqlParameter("@isactive", SqlDbType.SmallInt);
				parmArray[8].Value = this.IsActive;

				parmArray[9] = new SqlParameter("@countrycode", SqlDbType.Char, 1);
				parmArray[9].Value = (string)Country[CountryParameterNames.CountryCode];

				parmArray[10] = new SqlParameter("@paymentholidays", SqlDbType.Bit);
				parmArray[10].Value = this.PaymentHolidays;

				parmArray[11] = new SqlParameter("@agrtext", SqlDbType.NVarChar, 400);
				parmArray[11].Value = this.AgreementText;

				parmArray[12] = new SqlParameter("@minterm", SqlDbType.SmallInt);
				parmArray[12].Value = this.MinTerm;

				parmArray[13] = new SqlParameter("@maxterm", SqlDbType.SmallInt);
				parmArray[13].Value = this.MaxTerm;
  
				parmArray[14] = new SqlParameter("@CashBackMonth", SqlDbType.SmallInt);
				parmArray[14].Value = this.CashBackMonth;

				parmArray[15] = new SqlParameter("@CashBackPc", SqlDbType.SmallInt);
				parmArray[15].Value = this.CashBackPc;

				parmArray[16] = new SqlParameter("@CashBackAmount", SqlDbType.SmallInt);
				parmArray[16].Value = this.CashBackAmount;

				parmArray[17] = new SqlParameter("@AgreementPrint", SqlDbType.NVarChar,80);
				parmArray[17].Value = this.AgreementPrint;

				parmArray[18] = new SqlParameter("@DeferredMonths", SqlDbType.SmallInt);
				parmArray[18].Value = this.DeferredMonths;

				parmArray[19] = new SqlParameter("@FullRebateDays", SqlDbType.SmallInt);
				parmArray[19].Value = this.FullRebateDays;

				parmArray[20] = new SqlParameter("@STCPc", SqlDbType.SmallInt);
				parmArray[20].Value = this.STCPc;

				parmArray[21] = new SqlParameter("@STCAmount", SqlDbType.SmallInt);
				parmArray[21].Value = this.STCAmount;

                parmArray[22] = new SqlParameter("@defaultterm", SqlDbType.SmallInt);
                parmArray[22].Value = this.DefaultTerm;

				parmArray[23] = new SqlParameter("@delnonstocks", SqlDbType.SmallInt);
				parmArray[23].Value = Convert.ToInt16(this.DelNonStocks);

				parmArray[24] = new SqlParameter("@apr", SqlDbType.NVarChar,6);
				parmArray[24].Value = this.APR;

				//parmArray[25] = new SqlParameter("@PClubTier1", SqlDbType.Char, 1);
				//parmArray[25].Value = this.pClubTier1;

				//parmArray[26] = new SqlParameter("@PClubTier2", SqlDbType.Char, 1);
				//parmArray[26].Value = this.pClubTier2;

				parmArray[25] = new SqlParameter("@user", SqlDbType.Int);
				parmArray[25].Value = user;

                parmArray[26] = new SqlParameter("@donotsecuritise", SqlDbType.SmallInt);
                parmArray[26].Value = this.DoNotSecuritise;
				
				parmArray[27] = new SqlParameter("@storeType", SqlDbType.Char, 1);
                parmArray[27].Value = this.StoreType;

                parmArray[28] = new SqlParameter("@isLoan", SqlDbType.Bit);         // CR906 jec
                parmArray[28].Value = this.IsLoan;

                parmArray[29] = new SqlParameter("@loanNewCustomer", SqlDbType.Bit);       
                parmArray[29].Value = this.LoanNewCustomer;

                parmArray[30] = new SqlParameter("@loanRecentCustomer", SqlDbType.Bit);         
                parmArray[30].Value = this.LoanRecentCustomer;

                parmArray[31] = new SqlParameter("@loanExistingCustomer", SqlDbType.Bit);         
                parmArray[31].Value = this.LoanExistingCustomer;

                parmArray[32] = new SqlParameter("@loanStaff", SqlDbType.Bit);         
                parmArray[32].Value = this.LoanStaff;

                parmArray[33] = new SqlParameter("@IsMmiActive", SqlDbType.Bit);
                parmArray[33].Value = this.IsMmiActive;

                parmArray[34] = new SqlParameter("@MmiThresholdPercentage ", SqlDbType.Float);
                parmArray[34].Value = this.MmiThresholdPercentage;

                this.RunSP(conn, trans, "DN_TermsTypeSaveDetailsSP", parmArray);	
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		/// <summary>
		/// DeleteTermsTypeAccountTypes
		/// </summary>
		/// <param name="termstype">string</param>
		/// <returns>void</returns>
		/// 
		public void DeleteTermsTypeAccountTypes (SqlConnection conn, SqlTransaction trans, string termstype)
		{
			try
			{
				parmArray = new SqlParameter[1];
				
				parmArray[0] = new SqlParameter("@termstype", SqlDbType.VarChar, 4);
				parmArray[0].Value = termstype;				 
				
				this.RunSP(conn, trans, "DN_TermsTypeAccountTypesDeleteSP", parmArray);

			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		/// <summary>
		/// SaveTermsTypeAccountType
		/// </summary>
		/// <param name="termstype">string</param>
		/// <param name="accttype">string</param>
		/// <returns>void</returns>
		/// 
		public void SaveTermsTypeAccountType (SqlConnection conn, SqlTransaction trans, string termstype, string accttype)
		{
			try
			{
				parmArray = new SqlParameter[2];
				
				parmArray[0] = new SqlParameter("@termstype", SqlDbType.NVarChar, 8);
				parmArray[0].Value = termstype;
				
				parmArray[1] = new SqlParameter("@accttype", SqlDbType.NChar, 2);
				parmArray[1].Value = accttype;
				 
				
				this.RunSP(conn, trans, "DN_TermsTypeAccountTypeSaveSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		/// <summary>
		/// DeleteAllIntRateHistory
		/// </summary>
		/// <param name="termstype">string</param>
		/// <returns>void</returns>
		/// 
		public void DeleteAllIntRateHistory (SqlConnection conn, SqlTransaction trans, string termstype)
		{
			try
			{
				parmArray = new SqlParameter[1];
				
				parmArray[0] = new SqlParameter("@termstype", SqlDbType.VarChar, 4);
				parmArray[0].Value = termstype;

				this.RunSP(conn, trans, "DN_IntRateHistoryDeleteSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		/// <summary>
		/// SaveIntRateHistory
		/// </summary>
		/// <param name="termstype">string</param>
		/// <param name="datefrom">DateTime</param>
		/// <param name="dateto">DateTime</param>
		/// <param name="intrate">float</param>
		/// <param name="inspcent">float</param>
		/// <param name="adminpcent">float</param>
		/// <param name="insincluded">int</param>
		/// <param name="includewarranty">int</param>
		/// <param name="ratetype">string</param>
		/// <returns>void</returns>
		/// 
		public void SaveIntRateHistory (SqlConnection conn, SqlTransaction trans, string termstype, DateTime datefrom, DateTime dateto, double intrate, double inspcent, double adminpcent, short insincluded, short includewarranty, string ratetype, string band, short pointsFrom, short pointsTo, int user, double adminValue)
		{
			try
			{
				parmArray = new SqlParameter[14];
				
				parmArray[0] = new SqlParameter("@termstype", SqlDbType.NVarChar, 8);
				parmArray[0].Value = termstype;
				
				parmArray[1] = new SqlParameter("@datefrom", SqlDbType.DateTime);
				parmArray[1].Value = datefrom;
				
				parmArray[2] = new SqlParameter("@dateto", SqlDbType.DateTime);
				parmArray[2].Value = dateto;
				
				parmArray[3] = new SqlParameter("@intrate", SqlDbType.Float);
				parmArray[3].Value = intrate;
				
				parmArray[4] = new SqlParameter("@inspcent", SqlDbType.Float);
				parmArray[4].Value = inspcent;
				
				parmArray[5] = new SqlParameter("@adminpcent", SqlDbType.Float);
				parmArray[5].Value = adminpcent;
				
				parmArray[6] = new SqlParameter("@insincluded", SqlDbType.SmallInt);
				parmArray[6].Value = insincluded;
				
				parmArray[7] = new SqlParameter("@includewarranty", SqlDbType.SmallInt);
				parmArray[7].Value = includewarranty;

                parmArray[8] = new SqlParameter("@ratetype", SqlDbType.NVarChar, 4);
                parmArray[8].Value = ratetype;

                parmArray[9] = new SqlParameter("@band", SqlDbType.VarChar, 32);
                parmArray[9].Value = band;

                parmArray[10] = new SqlParameter("@pointsFrom", SqlDbType.SmallInt);
                parmArray[10].Value = pointsFrom;

                parmArray[11] = new SqlParameter("@pointsTo", SqlDbType.SmallInt);
                parmArray[11].Value = pointsTo;

                parmArray[12] = new SqlParameter("@user", SqlDbType.Int);
				parmArray[12].Value = user;

                parmArray[13] = new SqlParameter("@adminValue", SqlDbType.Float);
                parmArray[13].Value = adminValue;

				this.RunSP(conn, trans, "DN_IntRateHistorySaveSP", parmArray);
	
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		/// <summary>
		/// SaveTermsTypeLength
		/// </summary>
		/// <param name="termstype">string</param>
		/// <param name="length">int</param>
		/// <returns>void</returns>
		/// 
		public void SaveTermsTypeLength (SqlConnection conn, SqlTransaction trans, string termstype, int length)
		{
			try
			{
				parmArray = new SqlParameter[2];
				
				parmArray[0] = new SqlParameter("@termstype", SqlDbType.NVarChar, 8);
				parmArray[0].Value = termstype;
				
				parmArray[1] = new SqlParameter("@length", SqlDbType.Int);
				parmArray[1].Value = length;
				 
				
				this.RunSP(conn, trans, "DN_TermsTypeLengthSaveSP", parmArray);
	
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		/// <summary>
		/// DeleteAllTermsTypeLengths
		/// </summary>
		/// <param name="termstype">string</param>
		/// <returns>void</returns>
		/// 
		public void DeleteAllTermsTypeLengths (SqlConnection conn, SqlTransaction trans, string termstype)
		{
			try
			{
				parmArray = new SqlParameter[1];
				
				parmArray[0] = new SqlParameter("@termstype", SqlDbType.VarChar, 4);
				parmArray[0].Value = termstype;
				 
				
				this.RunSP(conn, trans, "DN_TermsTypeLengthsDeleteSP", parmArray);
	
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		/// <summary>
		/// DeleteAllFreeInstallments
		/// </summary>
		/// <param name="termstype">string</param>
		/// <returns>void</returns>
		/// 
		public void DeleteAllFreeInstallments (SqlConnection conn, SqlTransaction trans, string termstype)
		{
			try
			{
				parmArray = new SqlParameter[1];
				
				parmArray[0] = new SqlParameter("@termstype", SqlDbType.VarChar, 4);
				parmArray[0].Value = termstype;
				 
				
				this.RunSP(conn, trans, "DN_TermsTypeFreeInstallmentsDeleteSP", parmArray);
	
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		/// <summary>
		/// SaveFreeInstallment
		/// </summary>
		/// <param name="termstype">string</param>
		/// <param name="intratefrom">DateTime</param>
		/// <param name="intrateto">DateTime</param>
		/// <param name="datefrom">DateTime</param>
		/// <param name="dateto">DateTime</param>
		/// <param name="month">int</param>
		/// <returns>void</returns>
		/// 
		public void SaveFreeInstallment (SqlConnection conn, SqlTransaction trans, string termstype, DateTime intratefrom, DateTime intrateto, DateTime datefrom, DateTime dateto, int month)
		{
			try
			{
				parmArray = new SqlParameter[6];
				
				parmArray[0] = new SqlParameter("@termstype", SqlDbType.NVarChar, 8);
				parmArray[0].Value = termstype;
				
				parmArray[1] = new SqlParameter("@intratefrom", SqlDbType.DateTime);
				parmArray[1].Value = intratefrom;
				
				parmArray[2] = new SqlParameter("@intrateto", SqlDbType.DateTime);
				parmArray[2].Value = intrateto;
				
				parmArray[3] = new SqlParameter("@datefrom", SqlDbType.DateTime);
				parmArray[3].Value = datefrom;
				
				parmArray[4] = new SqlParameter("@dateto", SqlDbType.DateTime);
				parmArray[4].Value = dateto;
				
				parmArray[5] = new SqlParameter("@month", SqlDbType.Int);
				parmArray[5].Value = month;
				 
				
				this.RunSP(conn, trans, "DN_TermsTypeFreeInstallmentSaveSP", parmArray);
	
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		/// <summary>
		/// DeleteAllVariableRates
		/// </summary>
		/// <param name="termstype">string</param>
		/// <returns>void</returns>
		/// 
		public void DeleteAllVariableRates (SqlConnection conn, SqlTransaction trans, string termstype)
		{
			try
			{
				parmArray = new SqlParameter[1];
				
				parmArray[0] = new SqlParameter("@termstype", SqlDbType.VarChar, 4);
				parmArray[0].Value = termstype;
				 
				
				this.RunSP(conn, trans, "DN_TermsTypeVariableRatesDeleteSP", parmArray);
		
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		/// <summary>
		/// SaveVariableRate
		/// </summary>
		/// <param name="termstype">string</param>
		/// <param name="intratefrom">DateTime</param>
		/// <param name="intrateto">DateTime</param>
		/// <param name="frommonth">int</param>
		/// <param name="tomonth">int</param>
		/// <param name="rate">double</param>
		/// <returns>void</returns>
		/// 
		public void SaveVariableRate (SqlConnection conn, SqlTransaction trans, string termstype, DateTime intratefrom, DateTime intrateto, int frommonth, int tomonth, decimal rate)
		{
			try
			{
				parmArray = new SqlParameter[6];
				
				parmArray[0] = new SqlParameter("@termstype", SqlDbType.NVarChar, 8);
				parmArray[0].Value = termstype;
				
				parmArray[1] = new SqlParameter("@intratefrom", SqlDbType.DateTime);
				parmArray[1].Value = intratefrom;
				
				parmArray[2] = new SqlParameter("@intrateto", SqlDbType.DateTime);
				parmArray[2].Value = intrateto;
				
				parmArray[3] = new SqlParameter("@frommonth", SqlDbType.Int);
				parmArray[3].Value = frommonth;
				
				parmArray[4] = new SqlParameter("@tomonth", SqlDbType.Int);
				parmArray[4].Value = tomonth;
				
				parmArray[5] = new SqlParameter("@rate", SqlDbType.Decimal);
				parmArray[5].Value = rate;
				 
				
				this.RunSP(conn, trans, "DN_TermsTypeVariableRateSaveSP", parmArray);
	
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public int GetVariableRates(SqlConnection conn, SqlTransaction trans, string termsType, DateTime dateOpened)
		{
			try
			{
				_variablerates = new DataTable();

				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@termstype", SqlDbType.NVarChar,2);
				parmArray[0].Value = termsType;
				parmArray[1] = new SqlParameter("@dateOpened", SqlDbType.DateTime);
				parmArray[1].Value = dateOpened;

				if(conn!=null&&trans!=null)
					result = this.RunSP(conn, trans, "DN_TermsTypeGetVariableRatesSP", parmArray, _variablerates);
				else
					result = this.RunSP("DN_TermsTypeGetVariableRatesSP", parmArray, _variablerates);
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

        public DataTable TermsTypeBandListGet()
        {
            DataTable BandsTable = new DataTable("TermsTypeBandList");

            try
            {
               RunSP("TermsTypeBandListGet", BandsTable);
               
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return BandsTable;
        }

	}
}