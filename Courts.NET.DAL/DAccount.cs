using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common;
using STL.Common.Constants.Enums;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;
using System.Collections;
using STL.Common.Structs;



namespace STL.DAL
{
    public class DAccountParms
    {
        public SqlConnection conn { get; set; }
        public SqlTransaction trans { get; set; }
        public string Acctno { get; set; }
        public string AcctList { get; set; }
        public int user { get; set; }
        public DateTime RunDate { get; set; }
    }
	
    
    /// <summary>
	/// Data access object for accounts
	/// </summary>
	public class DAccount: DALObject
	{
		private short _origBr = 0;
		public short OrigBr
		{
			get {return _origBr;}
			set {_origBr = value;}
		}

		private string _accountNo = "";
		public string AccountNumber
		{
			get {return _accountNo;}
			set {_accountNo = value;}
		}
        private string _recordCountt = "";
        public string RecordCount
        {
            get { return _recordCountt; }
            set { _recordCountt = value; }
        }
        private DateTime _dateAccountOpen = DateTime.MinValue.AddYears(1899);
		public DateTime DateAccountOpen
		{
			get {return _dateAccountOpen;}
			set {_dateAccountOpen = value;}
		}

		private decimal _as400bal = 0;
		public decimal AS400Bal
		{
			get {return _as400bal;}
			set {_as400bal = value;}
		}

		private short _paidpcent = 0;
		public short PaidPcent
		{
			get {return _paidpcent;}
			set {_paidpcent = value;}
		}

		private string _termsType = "";
		public string TermsType
		{
			get {return _termsType;}
			set {_termsType = value;}
		}

        private decimal _repossarrears = 0;
		public decimal RepossArrears
		{
			get {return _repossarrears;}
			set {_repossarrears = value;}
		}

		private decimal _repossvalue = 0;
		public decimal RepossValue
		{
			get {return _repossvalue;}
			set {_repossvalue = value;}
		}

		private DateTime _dateIntoArrears = DateTime.MinValue.AddYears(1899);
		public DateTime DateIntoArrears
		{
			get {return _dateIntoArrears;}
			set {_dateIntoArrears = value;}
		}

		private decimal _outbal = 0;
		public decimal OutstandingBalance
		{
			get	{return _outbal;}
			set	{_outbal = value;}
		}

		private string _currStatus = "";
		public string CurrentStatus
		{
			get	{return _currStatus;}
			set	{_currStatus = value;}
		}

		private short _creditDays=0;
		public short CreditDays
		{
			get {return _creditDays;}
			set	{_creditDays = value;}
		}

		private DateTime _dateLastPaid = DateTime.MinValue.AddYears(1899);
		public DateTime DateLastPaid
		{
			get {return _dateLastPaid;}
			set	{_dateLastPaid = value;}
		}

		private decimal _arrears=0;
		public decimal Arrears
		{
			get {return _arrears;}
			set	{_arrears = value;}
		}

		private string _highestStatus="";
		public string HighestStatus
		{
			get	{return _highestStatus;}
			set {_highestStatus = value;}
		}

		private short _highestStatusDays = 0;
		public short HighestStatusDays
		{
			get {return _highestStatusDays;}
			set	{_highestStatusDays = value;}
		}

		private short _branchNo = 0;
		public short BranchNo
		{
			get	{return _branchNo;}
			set	{_branchNo = value;}
		}
        

		private string _accountType;
		public string AccountType
		{
			get {return _accountType;}
			set	{_accountType = value;}
		}

		private decimal _agreementTotal = 0;
		public decimal AgreementTotal
		{
			get	{return _agreementTotal;}
			set	{_agreementTotal = value;}
		}

		private int		_accountExists;
		public int AccountExists
		{
			get	{return _accountExists;	}
			set	{_accountExists = value;}
		}
		
		private string	_customerID = "";
		public string CustomerID
		{
			get	{return _customerID;}
			set	{_customerID = value;}
		}

		private string	_holdorjoint;
		public string HoldOrJoint
		{
			get	{return _holdorjoint;}
			set	{_holdorjoint = value;}
		}

		private string	_jointcustid;
		public string JointCustomerID
		{
			get	{return _jointcustid;}
			set	{_jointcustid = value;}
		}

		private decimal _bdwbalance = 0;
		public decimal BDWBalance
		{
			get	{return _bdwbalance;}
			set	{_bdwbalance = value;}
		}

		private decimal _bdwcharges = 0;
		public decimal BDWCharges
		{
			get	{return _bdwcharges;}
			set	{_bdwcharges = value;}
		}

		private string _securitised = "";
		public string Securitised
		{
			get	{return _securitised;}
			set	{_securitised = value;}
		}

       

        public DataTable AccountCodes
		{
			get	{return _accountCodes;	}
		}

		public DataTable AccountName
		{
			get	{return _accountName;}
		}

		public DataTable AccountsAwaitingClearance
		{
			get	{return _awaitingClearance;}
		}

        public DataTable InstantCreditAwaitingClearance
        {
            get { return _icClearance; }
        }
		
		public DataTable AccountsList
		{
			get	{return _accountslist;}
		}
		public DataTable IncompleteCredits
		{
			get	{return _incompleteCredits;}
		}
		public DataTable ArrearsAccounts
		{
			get	{return _arrearsAccounts;}
		}
			
		public DataTable AccountDetails
		{
			get	{return _accountdetails;}
		}
		private DataTable _stage1 = null;
		public DataTable Stage1
		{
			get	{return _stage1;}
		}
		private string _country = "";
		public new string Country
		{
			get{return _country;}
			set{_country = value;}
		}
		private int _CAccounts = 0;
		public int CurrentAccounts 
		{
			get{return _CAccounts;}
			set{_CAccounts = value;}
		}
		private int _SAccounts = 0;
		public int SettledAccounts 
		{
			get{return _SAccounts;}
			set{_SAccounts = value;}
		}

        private bool _hasLineItems = false;
        public bool HasLineItems
        {
            get { return _hasLineItems; }
            set { _hasLineItems = value; }
        }

        private bool _isAmortized = false;
        public bool IsAmortized
        {
            get { return _isAmortized; }
            set { _isAmortized = value; }
        }

        public DataTable Deliveries
		{
			get{return _deliveries;}
		}

		public DataTable DeliveryLineItems
		{
			get{return _deliveryLineItems;}
		}

		private DateTime _dateAccountLocked = DateTime.MinValue.AddYears(1899);
		public DateTime DateAccountLocked
		{
			get {return _dateAccountLocked;}
		}

        //IP - 11/02/10 - CR1048 (Ref:3.1.2.5) Merged - Malaysia Enhancements (CR1072)
        private DataTable _cashAndGoPayments;
        public DataTable CashAndGoPayments
        {
            get { return _cashAndGoPayments; }
        }
        //CR        : CLA OutStanding Balance Calculation
        //Author    : Rahul D
        //Details   : boolean flag to check weather the account is created with new outstanding balance calculation or no.
        //Date      : 18/06/2019
        private bool _isAmortizedOutStandingBal = false;
        public bool IsAmortizedOutStandingBal
        {
            get { return _isAmortizedOutStandingBal; }
            set { _isAmortizedOutStandingBal = value; }
        }

        private DataTable _awaitingClearance;
        private DataTable _icClearance;
		private DataTable _accountslist;
		private DataTable _accountdetails;
		private DataTable _accountName;
		private DataTable _accountCodes;
		private DataTable _incompleteCredits;
		private DataTable _arrearsAccounts;

		private DataTable _deliveries;
		private DataTable _deliveryLineItems;

		private DataTable _segments;
        

        public DataTable Segments
		{
			get	{return _segments;}
		}
		
        private DataTable _warrantyProductList;
        private DataTable _warrantyClaimCollectionResasons;

        public DataTable WarrantyClaimCollectionResasons // CR 822 Peter Chong [02-Oct-2006]
        {
            get { return this._warrantyClaimCollectionResasons; }
        }
        
        public DataTable WarrantyProductList // CR 822 Peter Chong [28-Sep-2006]
        { 
            get { return _warrantyProductList; } 
        }

        

		private DataTable _activities;
		public DataTable Activities
		{
			get	{return _activities;}
		}

		private DataTable _endDates;
		public DataTable EndDates
		{
			get	{return _endDates;}
		}

		private DataTable _rebateReport;
		public DataTable RebateReport
		{
			get	{return _rebateReport;}
		}

        //private DataTable _summaryrptData;
        //public DataTable SummaryRptData
        //{
        //    get	{return _summaryrptData;}
        //}

        //private DataTable _summaryrptdataFull;
        //public DataTable SummaryrptdataFull
        //{
        //    get	{return _summaryrptdataFull;}
        //}

        //private DataTable _seasonedData;
        //public DataTable _SeasonedData
        //{
        //    get	{return _seasonedData;}
        //}

        //IP - 04/02/10 - CR1072 - 3.1.9 - Display Delivery Authorisation History in Account Details.
        private DataTable _acctDaHistory;
        public DataTable AcctDaHistory
        {
            get { return _acctDaHistory; } 
        }

      private DataSet _arrearAccts;
      public DataSet arrearAccounts
      {
         get
         {
            return _arrearAccts;
         }
         set
         {
            _arrearAccts = value;
         }
      }

        private DataTable _amortizedSchedule;
        public DataTable AmortizedSchedule
        {
            get { return _amortizedSchedule; }
        }


        public DAccount()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public DAccount(string accountNumber)
		{
			this.Populate(null, null, accountNumber);
		}

		public DAccount(SqlConnection conn, SqlTransaction trans, string accountNumber)
		{
			this.Populate(conn, trans, accountNumber);
		}

		//
		// Methods
		//
		public DataTable GetMambuCLPaymentMethods(string acctNo)
		{
			DataTable dt = null;
			try
			{
				dt = new DataTable(TN.CLPayMethods);

				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@AcctNo", SqlDbType.NChar, 12);
				parmArray[0].Value = acctNo;
				this.RunSP("SP_BS_GetMambuCLPaymentMethods", parmArray, dt);
			}
			catch (SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return dt;
		}

		public DataTable GetMambuHPPaymentMethods()
		{
			DataTable dt = null;
			try
			{
				dt = new DataTable(TN.HPPayMethods);
				this.RunSP("SP_BS_GetMambuHPPaymentMethods", dt);
			}
			catch (SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return dt;
		}
		/// <summary>
		/// This method will insert a record in the database to signify that an 
		/// account is locked and cannot therefore be locked by another user
		/// If the account is already locked then an exception will be
		/// thrown by the stored procedure.
		/// </summary>
		/// <param name="accountNumber">the account to lock</param>
		/// <param name="user">the user who is trying to lock the account</param>
		/// <returns></returns>
		/// <summary>
		/// getincompletecredits Returns an array of accounts
		/// </summary>
		/// <param name="branchrestriction">The branch from which the accounts are loaded</param>
		/// <param name="viewlimit">whether to return 200 or the maximum number of rows</param>
		/// <param name="holdflags">whether or not restricted by a particular checktype</param>
		/// <returns></returns>

		public int GetIncompleteCredits(string branchRestriction,string holdFlags,
			string viewLimit,string acctno,bool SingleFlagOnly,bool excludeNoItems,
			bool excludeUnpaid, string refCode, bool Referral, bool Pending)
		{
			try
			{
				_incompleteCredits = new DataTable(TN.IncompleteCredits);
				parmArray = new SqlParameter[10];
				parmArray[0] = new SqlParameter("@BranchRestriction", SqlDbType.NVarChar, 4);
				parmArray[0].Value = branchRestriction;
				parmArray[1] = new SqlParameter("@viewLimit", SqlDbType.NVarChar,50);
				parmArray[1].Value = viewLimit;
				parmArray[2] = new SqlParameter("@holdflags", SqlDbType.NVarChar, 4);
				parmArray[2].Value = holdFlags;
				parmArray[3] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
				parmArray[3].Value = acctno;
				parmArray[4] = new SqlParameter("@SingleFlagOnly", SqlDbType.Bit);
				parmArray[4].Value = SingleFlagOnly;
				parmArray[5] = new SqlParameter("@excludenoitems", SqlDbType.Bit);
				parmArray[5].Value = excludeNoItems;
				parmArray[6] = new SqlParameter("@excludeUnpaid", SqlDbType.Bit);
				parmArray[6].Value = excludeUnpaid;
                parmArray[7] = new SqlParameter("@refcode", SqlDbType.NVarChar, 2);
                parmArray[7].Value = refCode;
                parmArray[8] = new SqlParameter("@ReferralCL", SqlDbType.Bit);
                parmArray[8].Value = Referral;
                parmArray[9] = new SqlParameter("@PendingCL", SqlDbType.Bit);
                parmArray[9].Value = Pending;


				this.RunSP("DN_Incomplete_SP", parmArray, _incompleteCredits);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

		/* Issue 69238 - SC 3/9/07
				 * Get agreement number so cash and go accounts with agreement numbers greater than 1
				 * line items can be viewed. */

        public int GetAgreementNo(SqlConnection conn, SqlTransaction trans, string AcctNo)
        {
            int agreementNo = 0;
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@Acctno", SqlDbType.NVarChar, 12);
                parmArray[0].Value = AcctNo;
                parmArray[1] = new SqlParameter("@Agreementno", SqlDbType.Int);
                parmArray[1].Value = "";
                parmArray[1].Direction = ParameterDirection.Output;

                if (conn != null && trans != null)
                    RunSP(conn, trans, "DN_AccountGetAgreementNo", parmArray);
                else
                    RunSP("DN_AccountGetAgreementNo", parmArray);
                if (!Convert.IsDBNull(parmArray[1].Value))
                    agreementNo = (int)parmArray[1].Value;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return agreementNo;
        }
		public void Lock(SqlConnection conn, SqlTransaction trans, string accountNo, int user)
		{
			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NChar, 12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@user", SqlDbType.Int);
				parmArray[1].Value = user;
				if (conn !=null)
                    this.RunSP(conn, trans, "DN_LockAccountSP", parmArray);
				else
				{
                    this.RunSP("DN_LockAccountSP", parmArray);
				}
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void UpdateStatus(SqlConnection conn, SqlTransaction trans, string accountNo)
		{
			try
			{
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NChar, 12);
				parmArray[0].Value = accountNo;
				this.RunSP(conn, trans, "DN_AccountUpdateStatusSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

        

		public void EodArrearsCalculation(SqlConnection conn, SqlTransaction trans, bool NextDay)
		{
			try
			{  // first remove the delivery dates from those who have had collections
				this.RunSP(conn, trans, "dbremovedeliverydates" );
                System.Nullable<DateTime> RunDate = null;
			   //arrears calculation for the masses
                if (NextDay)// calculate arrears as of the next day
                {
                    // So rule is after 7 am means next day -- before 7am means current day
                    if (DateTime.Now.Hour < 7)
                        RunDate = DateTime.Now;
                    else
                        RunDate = DateTime.Now.AddDays(1);
                }
                else 
                    RunDate =null;

                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@rundate", SqlDbType.DateTime);
                parmArray[0].Value = RunDate;
				this.RunSP(conn, trans, "DN_ArrearsCalculation",parmArray );
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}

		}

        
		public void CalculateAvailableSpendForAllCusts(SqlConnection conn, SqlTransaction trans)
		{
			try
			{   
                this.RunSP(conn, trans, "dn_CustomerCalculateAvailableSpendAll");
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}

		}


        

		public void UpdateStatus(SqlConnection conn, SqlTransaction trans, 
			string accountNo,
			string currentStat,
			string newStat)
		{
			try
			{
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@current", SqlDbType.NChar, 1);
				parmArray[1].Value = currentStat;
				parmArray[2] = new SqlParameter("@new", SqlDbType.NChar, 1);
				parmArray[2].Value = newStat;
				this.RunSP(conn, trans, "DN_AccountUpdateStatus2SP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public short GetNumberOfAgreements(string accountNo)
		{
			short num = 1;
			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NChar, 12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@num", SqlDbType.SmallInt);
				parmArray[1].Value = 1;
				parmArray[1].Direction = ParameterDirection.Output;
				this.RunSP("DN_AccountGetNoOfAgreementsSP", parmArray);

				if(parmArray[1].Value!=DBNull.Value)
					num = (short)parmArray[1].Value;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return num;
		}

		/// <summary>
		/// This method will check to see whether a user has a valid lock 
		/// for a particular account. Must be called before update 
		/// operations on the account records.
		/// Doesn't need to return a boolean because an exception
		/// will be thrown by the stored procedure if there is no 
		/// valid lock.
		/// </summary>
		/// <param name="accountNumber">the account to check</param>
		/// <param name="user">the user to check</param>
		/// <returns></returns>
		public void ValidLock(SqlConnection conn, SqlTransaction trans, string accountNo, int user)
		{
			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NChar, 12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@user", SqlDbType.Int);
				parmArray[1].Value = user;
				this.RunSP(conn, trans, "DN_ValidLockSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void ValidLock(string accountNo, int user)
		{
			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NChar, 12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@user", SqlDbType.Int);
				parmArray[1].Value = user;
				this.RunSP("DN_ValidLockSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		/// <summary>
		/// This method will remove any lock that this user has on 
		/// this account
		/// </summary>
		/// <param name="accountNumber">the account to unlock</param>
		/// <param name="user">the user to unlock</param>
		/// <returns></returns>
		public void Unlock(SqlConnection conn, SqlTransaction trans, string accountNo, int user)
		{
			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NChar, 12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@user", SqlDbType.Int);
				parmArray[1].Value = user;
				this.RunSP(conn, trans, "DN_UnlockAccountSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

        //Stored procedure to validate itemno at the point of sale so that no poisons are generated later
        public void ValidateSaleForCINT(string sku, int quantity, int salelocation, int stocklocation, DateTime transactiondate, out string ErrorMessage, out int ErrorCount)
        {
            ErrorMessage = string.Empty;
            ErrorCount = 0;
            try
            {
                parmArray = new SqlParameter[7];
                parmArray[0] = new SqlParameter("@sku", SqlDbType.NChar, 9);
                parmArray[0].Value = sku;
                parmArray[1] = new SqlParameter("@Quantity", SqlDbType.Int);
                parmArray[1].Value = quantity;
                parmArray[2] = new SqlParameter("@salelocation", SqlDbType.Int);
                parmArray[2].Value = salelocation;
                parmArray[3] = new SqlParameter("@stocklocation", SqlDbType.Int);
                parmArray[3].Value = stocklocation;
                parmArray[4] = new SqlParameter("@Transactiondate", SqlDbType.DateTime);
                parmArray[4].Value = transactiondate;
                parmArray[5] = new SqlParameter("@ErrorMessage", SqlDbType.VarChar,5000);
                parmArray[5].Direction = ParameterDirection.Output;
                parmArray[5].Value = ErrorMessage;
                parmArray[6] = new SqlParameter("@ErrorCount", SqlDbType.Int);
                parmArray[6].Value = ErrorCount;
                parmArray[6].Direction = ParameterDirection.Output;
                this.RunNonQuery("Merchandising.ValidateSaleForCINT", parmArray,false);

                if (!Convert.IsDBNull(parmArray[5].Value))
                    ErrorMessage = parmArray[5].Value.ToString();
                if (!Convert.IsDBNull(parmArray[6].Value))
                    ErrorCount = int.Parse(parmArray[6].Value.ToString());
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
           
        }

        /// <summary>
        /// This method will find the next available lock on an account
        /// Using old style as have to do 5.2....  
        /// </summary>
        /// <param name="Parms">the account to unlock</param>
        ///
        /// <returns></returns>
        public void AccountLockingFindandLockForCaller(ref DAccountParms Parms)
        {
            try
            {
                parmArray = new SqlParameter[4];
                parmArray[0] = new SqlParameter("@acctnolist", SqlDbType.VarChar, 1000);
                parmArray[0].Value = Parms.AcctList;
                parmArray[1] = new SqlParameter("@user", SqlDbType.Int);
                parmArray[1].Value = Parms.user;
                parmArray[2] = new SqlParameter("@acctno", SqlDbType.Char, 12);
                parmArray[2].Value = Parms.Acctno;
                parmArray[2].Direction = ParameterDirection.Output;
                parmArray[3] = new SqlParameter("@rundate", SqlDbType.DateTime);
                parmArray[3].Value = Parms.RunDate;

                this.RunSP(Parms.conn, Parms.trans, "AccountLockingFindandLockForCaller", parmArray);

                if (!Convert.IsDBNull(parmArray[2].Value))
                    Parms.Acctno = Convert.ToString(parmArray[2].Value);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

      /// <summary>
      /// Removes the CurrentAction of 'R' from the AccountLocking table
      /// </summary>
      /// <param name="conn"></param>
      /// <param name="trans"></param>
      /// <param name="accountNo"></param>
      /// <param name="user"></param>
      public void UnlockAccountForGoodsReturn(SqlConnection conn, SqlTransaction trans, string accountNo, int user)
      {
         try
         {
            parmArray = new SqlParameter[2];
            parmArray[0] = new SqlParameter("@acctno", SqlDbType.NChar, 12);
            parmArray[0].Value = accountNo;
            parmArray[1] = new SqlParameter("@user", SqlDbType.Int);
            parmArray[1].Value = user;
            result = this.RunSPwithExecuteScalar(conn, trans, "AccountLockingRemoveReviseAccountSP", parmArray);
         }
         catch (SqlException ex)
         {
            LogSqlException(ex);
            throw ex;
         }
      }

		/// <summary>
		/// Checks to see whether the account exists. If it does it returns the customer ID
		/// </summary>
		/// <param name="accountNumber"></param>
		/// <returns></returns>
		public int Validate(SqlConnection conn, SqlTransaction trans, string accountNumber)
		{
			try
			{
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NChar, 12);
				parmArray[0].Value = accountNumber;
				parmArray[1] = new SqlParameter("@acctExists", SqlDbType.Int);
				parmArray[1].Direction = ParameterDirection.Output;
				parmArray[1].Value = 0;
				parmArray[2] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
				parmArray[2].Direction = ParameterDirection.Output;
				parmArray[2].Value = "";
				
				if(conn!=null && trans!=null)
					result = this.RunSP(conn, trans, "DN_AccountValidateSP", parmArray);
				else
					result = this.RunSP("DN_AccountValidateSP", parmArray);

				_accountExists = (int)parmArray[1].Value;

				if(!Convert.IsDBNull(parmArray[2].Value))
					_customerID = (string)parmArray[2].Value;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

        /// <summary>
        /// Runs the letters and charges routine - generates letters, status code changes and interest charges for accounts in arrears
        /// </summary>
        /// <param name="runno"></param>
        /// <returns></returns>
        public int RunArrearsLettersandCharges(SqlConnection conn, SqlTransaction trans, int runno)
        {
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@runno", SqlDbType.Int);
                parmArray[0].Value = runno;

                if (conn != null && trans != null)
                {
                    result = this.RunSP(conn, trans, "dn_ArrearslettersAndCharges", parmArray);
                }
                else
                {
                    result = this.RunSP("dn_ArrearslettersAndCharges", parmArray);
                }
    
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;
        }

        public int AccountGetForRenewal(SqlConnection conn, SqlTransaction trans)
        {
            try
            {

                parmArray = new SqlParameter[4];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[0].Value = "";
                parmArray[1] = new SqlParameter("@iscurrentsettled", SqlDbType.Bit);
                parmArray[1].Value = 0;
                parmArray[2] = new SqlParameter("@ismenucall", SqlDbType.Bit);
                parmArray[2].Value = 0;
                parmArray[3] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
                parmArray[3].Value = "";

                if (conn != null && trans != null)
                {
                    result = this.RunSP(conn, trans, "dn_AccountGetForRenewalSP", parmArray);
                }
                else
                {
                    result = this.RunSP("dn_AccountGetForRenewalSP", parmArray);
                }

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;
        }


        /// <summary>
        /// IP - 19/09/2007 
        /// Method sends one of two letters, 'WCR' (Warranty on Credit Reminder letter)
        /// or 'WGL' (Warranty on Credit Grace Letter) to Customers that have not paid
        /// for their Warranty on Credit within the alloted time.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public int WOCLetters(SqlConnection conn, SqlTransaction trans)
        {
            try
            {

                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@letterdate", SqlDbType.DateTime);
                parmArray[0].Value = System.DateTime.Now;
              

                if (conn != null && trans != null)
                {
                    result = this.RunSP(conn, trans, "or_creditwarrantyletters_sp", parmArray);
                }
                else
                {
                    result = this.RunSP("or_creditwarrantyletters_sp", parmArray);
                }

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// Sends addtoletters inviting customers who have paid of a certain percentage of their agreement to come in the store and buy more.
        /// Also sends I1,I2,I3 and possibly I4 letters for customers who have settled their account successfully. 
        /// </summary>
        /// <param name="runno"></param>
        /// <returns></returns>

        public int RunAddtoLetters(SqlConnection conn, SqlTransaction trans, int runno)
        {
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@runno", SqlDbType.Int);
                parmArray[0].Value = runno;
                parmArray[1] = new SqlParameter("@type", SqlDbType.VarChar,4);
                parmArray[1].Value = "";


                if (conn != null && trans != null)
                    result = this.RunSP(conn, trans, "dn_addtoletter", parmArray);
                else
                    result = this.RunSP("dn_addtoletter", parmArray);


            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;
        }


        /// <summary>
        /// Generates the letter files for the last letters and charges routine.
        /// </summary>
        /// <param name="runno"></param>
        /// <returns></returns>
        public int LettersGenerateCSVfiles(SqlConnection conn, SqlTransaction trans, int runno,string type)
        {
            try
            {
                //DataTable _acct = new DataTable(TN.AccountDetails);

                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@runno", SqlDbType.Int);
                parmArray[0].Value = runno;
                parmArray[1] = new SqlParameter("@type", SqlDbType.VarChar, 15); //IP - 18/07/08 - UAT 5.2 - UAT (24) changed from varchar 10 to 15 for 'COLLECTIONS' type.
                parmArray[1].Value = type;


                if (conn != null && trans != null)
                    result = this.RunSP(conn, trans, "dn_lettersGenerateCSVfiles", parmArray);
                else
                    result = this.RunSP("dn_lettersGenerateCSVfiles", parmArray);



            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;
        }
        /// <summary>
        /// Retrieves data so that letter files can be written.
        /// </summary>
        /// <param name="runno"></param>
        /// <returns></returns>
        public DataTable LoadLetterDetails(SqlConnection conn, SqlTransaction trans, int runno, string lettercode,
           DateTime datefrom,DateTime dateto       )
        {
            try
            {
                 _accountdetails = new DataTable(TN.AccountDetails);

                parmArray = new SqlParameter[5];
                parmArray[0] = new SqlParameter("@runno", SqlDbType.Int);
                parmArray[0].Value = runno;
                parmArray[1] = new SqlParameter("@lettercode", SqlDbType.VarChar, 10);
                parmArray[1].Value = lettercode;
                parmArray[2] = new SqlParameter("@datestart", SqlDbType.DateTime);
                parmArray[2].Value = datefrom;
                parmArray[3] = new SqlParameter("@datefinish", SqlDbType.DateTime);
                parmArray[3].Value = dateto;
                parmArray[4] = new SqlParameter("@selectonly", SqlDbType.Char, 1);
                parmArray[4].Value = 'Y' ;


                if (conn != null && trans != null)
                    result = this.RunSP(conn, trans, "dn_lglettergeneration", parmArray, _accountdetails);
                else
                    result = this.RunSP("dn_lglettergeneration", parmArray, _accountdetails);




            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return _accountdetails;
        }


        /// <summary>
        /// gets accounts for allocation based on many parameters
        /// 
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <returns></returns>
        /// 
        public DataSet Getacctsforalloc(string alreadyAllocated,
           string minimumStatus,
           string currStatus,
           string employeeType,
           DateTime dateStartAllocated,
           DateTime dateFinishedAllocated,
           string actionChoice,
           DateTime actionStart,
           DateTime actionEnd,
           double minimumArrears,
           double maximumArrears,
           string statusType,
           string arrearsChoice,
           double arrears,
           string actionCode,
           string letterCode,
           string letterRestriction,
           bool letterRadio,
           DateTime letterStart,
           DateTime letterFinish,
           string actionRestriction,
           int empeeno,
           DateTime actiondateStart,
           DateTime actiondateFinish,
           bool includePhone,
           bool includeAddress,
           short branchno,
              string branchset,
           short proposalPoints,
           string propPointsDirection,
           string codeRestriction,
           string accountBranch,
           bool viewTop,
           ref bool rowLimited,
           string code,
           short numActions,
           string actionOperand,
           string balanceOperand,
           short restrictEmployee,
           decimal balance,
           short includeCharges,
           DateTime dateMovedFrom,
           DateTime dateMovedTo,
           string dateMovedRestriction,
           DateTime datelastPaid,
           string dateOperand,
           bool actionDueDate,
              bool credit,
              bool cash,
              bool service)
        {
           try
           {
              _arrearsAccounts = new DataTable(TN.ArrearsAccounts);
              //arrearAccounts = new DataSet();
              parmArray = new SqlParameter[52];

              parmArray[0] = new SqlParameter("@alreadyallocated", SqlDbType.NVarChar, 3);
              parmArray[0].Value = alreadyAllocated;
              parmArray[1] = new SqlParameter("@minstatus", SqlDbType.NVarChar, 2);
              parmArray[1].Value = minimumStatus;
              parmArray[2] = new SqlParameter("@currstatus", SqlDbType.NVarChar, 2);
              parmArray[2].Value = currStatus;
              parmArray[3] = new SqlParameter("@employeetype", SqlDbType.NVarChar, 15);
              parmArray[3].Value = employeeType;
              parmArray[4] = new SqlParameter("@datestartallocated", SqlDbType.DateTime);
              parmArray[4].Value = dateStartAllocated;
              parmArray[5] = new SqlParameter("@datefinishallocated", SqlDbType.DateTime);
              parmArray[5].Value = dateFinishedAllocated;
              parmArray[6] = new SqlParameter("@actionchoice", SqlDbType.NVarChar, 3);
              parmArray[6].Value = actionChoice;
              parmArray[7] = new SqlParameter("@actionstart", SqlDbType.DateTime);
              parmArray[7].Value = actionStart;
              parmArray[8] = new SqlParameter("@actionend ", SqlDbType.DateTime);
              parmArray[8].Value = actionEnd;
              parmArray[9] = new SqlParameter("@minarrears", SqlDbType.Money);
              parmArray[9].Value = minimumArrears;
              parmArray[10] = new SqlParameter("@statustype ", SqlDbType.NVarChar, 12);
              parmArray[10].Value = statusType;
              parmArray[11] = new SqlParameter("@Maxarrears", SqlDbType.Money);
              parmArray[11].Value = maximumArrears;
              parmArray[12] = new SqlParameter("@arrearschoice ", SqlDbType.NVarChar, 3);
              parmArray[12].Value = arrearsChoice;
              parmArray[13] = new SqlParameter("@arrears", SqlDbType.Money);
              parmArray[13].Value = arrears;
              parmArray[14] = new SqlParameter("@actioncode ", SqlDbType.NVarChar, 6);
              parmArray[14].Value = actionCode;
              parmArray[15] = new SqlParameter("@lettercode ", SqlDbType.NVarChar, 10);
              parmArray[15].Value = letterCode;
              parmArray[16] = new SqlParameter("@letterRestriction ", SqlDbType.NVarChar, 4);
              parmArray[16].Value = letterRestriction;
              parmArray[17] = new SqlParameter("@letterradio  ", SqlDbType.Bit);
              parmArray[17].Value = 1;// letterRadio;
              parmArray[18] = new SqlParameter("@letterstart", SqlDbType.DateTime);
              parmArray[18].Value = letterStart;
              parmArray[19] = new SqlParameter("@letterfinish", SqlDbType.DateTime);
              parmArray[19].Value = letterFinish;
              parmArray[20] = new SqlParameter("@actionrestriction", SqlDbType.NVarChar, 4);
              parmArray[20].Value = actionRestriction;
              parmArray[21] = new SqlParameter("@actiondatestart", SqlDbType.DateTime);
              parmArray[21].Value = actiondateStart;
              parmArray[22] = new SqlParameter("@actiondatefinish", SqlDbType.DateTime);
              parmArray[22].Value = actiondateFinish;
              parmArray[23] = new SqlParameter("@includephone", SqlDbType.Bit);
              parmArray[23].Value = 1;//includePhone;
              parmArray[24] = new SqlParameter("@includeaddresses", SqlDbType.Bit);
              parmArray[24].Value = 1;//includeAddress;
              parmArray[25] = new SqlParameter("@empeeno ", SqlDbType.Int);
              parmArray[25].Value = empeeno;
              parmArray[26] = new SqlParameter("@empeetype ", SqlDbType.NVarChar, 4);
              parmArray[26].Value = employeeType;
              parmArray[27] = new SqlParameter("@branch  ", SqlDbType.SmallInt);
              parmArray[27].Value = branchno;
              parmArray[28] = new SqlParameter("@proppoints", SqlDbType.SmallInt);
              parmArray[28].Value = proposalPoints;
              parmArray[29] = new SqlParameter("@proppointsdirection", SqlDbType.NVarChar, 2);
              parmArray[29].Value = propPointsDirection;
              parmArray[30] = new SqlParameter("@coderestriction", SqlDbType.NVarChar, 3);
              parmArray[30].Value = codeRestriction;
              parmArray[31] = new SqlParameter("@accountbranch", SqlDbType.NVarChar, 4);
              parmArray[31].Value = accountBranch;
              parmArray[32] = new SqlParameter("@viewTop", SqlDbType.Bit);
              parmArray[32].Value = viewTop;
              parmArray[33] = new SqlParameter("@code", SqlDbType.NVarChar, 4);
              parmArray[33].Value = code;
              parmArray[34] = new SqlParameter("@rowLimited", SqlDbType.Bit);
              parmArray[34].Value = rowLimited;
              parmArray[34].Direction = ParameterDirection.Output;
              parmArray[35] = new SqlParameter("@user", SqlDbType.Int);
              parmArray[35].Value = this.User;
              parmArray[36] = new SqlParameter("@actionsfilter", SqlDbType.SmallInt);
              parmArray[36].Value = numActions;
              parmArray[37] = new SqlParameter("@actionsoperand", SqlDbType.NVarChar, 1);
              parmArray[37].Value = actionOperand;
              parmArray[38] = new SqlParameter("@actionsthisempeeno", SqlDbType.SmallInt);
              parmArray[38].Value = restrictEmployee;
              parmArray[39] = new SqlParameter("@balance", SqlDbType.Money);
              parmArray[39].Value = balance;
              parmArray[40] = new SqlParameter("@balanceoperand", SqlDbType.NChar, 1);
              parmArray[40].Value = balanceOperand;
              parmArray[41] = new SqlParameter("@arrearsinccharges", SqlDbType.SmallInt);
              parmArray[41].Value = includeCharges;
              parmArray[42] = new SqlParameter("@datemovedarrsfrom", SqlDbType.DateTime);
              parmArray[42].Value = dateMovedFrom;
              parmArray[43] = new SqlParameter("@datemovedarrsto", SqlDbType.DateTime);
              parmArray[43].Value = dateMovedTo;
              parmArray[44] = new SqlParameter("@datemovedrestriction", SqlDbType.NVarChar, 2);
              parmArray[44].Value = dateMovedRestriction;
              parmArray[45] = new SqlParameter("@datelastpaid", SqlDbType.DateTime);
              parmArray[45].Value = datelastPaid;
              parmArray[46] = new SqlParameter("@lastpaidoperand", SqlDbType.NChar, 1);
              parmArray[46].Value = dateOperand;
              parmArray[47] = new SqlParameter("@actionDueDate", SqlDbType.Bit);
              parmArray[47].Value = actionDueDate;
              parmArray[48] = new SqlParameter("@credit", SqlDbType.Bit);
              parmArray[48].Value = credit;
              parmArray[49] = new SqlParameter("@cash", SqlDbType.Bit);
              parmArray[49].Value = cash;
              parmArray[50] = new SqlParameter("@branchset", SqlDbType.NVarChar, 32);
              parmArray[50].Value = branchset;
              parmArray[51] = new SqlParameter("@service", SqlDbType.Bit);
              parmArray[51].Value = service;

              result = this.RunSP("dn_getacctsforalloc_sp", parmArray, _arrearsAccounts);

           }
           catch (SqlException ex)
           {
              LogSqlException(ex);
              throw ex;
           }
           return arrearAccounts;
        }

		/// <summary>
		/// Overloaded method that gets accounts for allocation based on many parameters including worklist and delivery area
		/// 
		/// </summary>
		/// <param name="accountNumber"></param>
		/// <returns></returns>
		/// 
		public  DataSet Getacctsforalloc(string alreadyAllocated,
			string minimumStatus,
			string currStatus,
			string employeeType,
			DateTime dateStartAllocated,
			DateTime dateFinishedAllocated,
			string actionChoice,
			DateTime actionStart,
			DateTime actionEnd,
			double minimumArrears,
			double maximumArrears,
			string statusType,
			string arrearsChoice,
			double arrears,
			string actionCode, 
			string letterCode,
			string letterRestriction,
			bool letterRadio,
			DateTime letterStart,
			DateTime letterFinish,
			string actionRestriction,
			int empeeno,
			DateTime actiondateStart,
			DateTime actiondateFinish,
			bool includePhone,
			bool includeAddress,
			short branchno,
            string branchset,
			short proposalPoints,
			string propPointsDirection ,
			string codeRestriction,
			string accountBranch,
			bool viewTop,
			ref bool rowLimited,
			string code,
			short numActions, 
			string actionOperand, 
			string balanceOperand,
			short restrictEmployee, 
			decimal balance, 
			short includeCharges, 
			DateTime dateMovedFrom,
			DateTime dateMovedTo, 
			string dateMovedRestriction, 
			DateTime datelastPaid, 
			string dateOperand,
			bool actionDueDate,
            bool credit,
            bool cash,
            bool service,
         string worklist,
         string deliveryArea)
     	
		{ 
			try
			{
				//_arrearsAccounts = new DataTable(TN.ArrearsAccounts);
            arrearAccounts = new DataSet();
				parmArray = new SqlParameter[54];

				parmArray[0] = new SqlParameter("@alreadyallocated", SqlDbType.NVarChar, 3);
				parmArray[0].Value = alreadyAllocated;
				parmArray[1] = new SqlParameter("@minstatus", SqlDbType.NVarChar,2);
				parmArray[1].Value = minimumStatus;
				parmArray[2] = new SqlParameter("@currstatus", SqlDbType.NVarChar, 2);
				parmArray[2].Value = currStatus;
				parmArray[3] = new SqlParameter("@employeetype", SqlDbType.NVarChar, 15);
				parmArray[3].Value = employeeType;
				parmArray[4] = new SqlParameter("@datestartallocated", SqlDbType.DateTime); 
				parmArray[4].Value = dateStartAllocated;
				parmArray[5] = new SqlParameter("@datefinishallocated", SqlDbType.DateTime); 
				parmArray[5].Value = dateFinishedAllocated;
				parmArray[6] = new SqlParameter("@actionchoice", SqlDbType.NVarChar, 3);
				parmArray[6].Value = actionChoice;
				parmArray[7] = new SqlParameter("@actionstart", SqlDbType.DateTime); 
				parmArray[7].Value = actionStart;
				parmArray[8] = new SqlParameter("@actionend ", SqlDbType.DateTime); 
				parmArray[8].Value = actionEnd;
				parmArray[9] = new SqlParameter("@minarrears", SqlDbType.Money); 
				parmArray[9].Value = minimumArrears;
				parmArray[10] = new SqlParameter("@statustype ", SqlDbType.NVarChar, 12);
				parmArray[10].Value = statusType;
				parmArray[11] = new SqlParameter("@Maxarrears", SqlDbType.Money); 
				parmArray[11].Value = maximumArrears;
				parmArray[12] = new SqlParameter("@arrearschoice ", SqlDbType.NVarChar,3);
				parmArray[12].Value = arrearsChoice;
				parmArray[13] = new SqlParameter("@arrears", SqlDbType.Money); 
				parmArray[13].Value = arrears;
				parmArray[14] = new SqlParameter("@actioncode ", SqlDbType.NVarChar, 6);
				parmArray[14].Value = actionCode; 
				parmArray[15] = new SqlParameter("@lettercode ", SqlDbType.NVarChar, 10);
				parmArray[15].Value = letterCode;
				parmArray[16] = new SqlParameter("@letterRestriction ", SqlDbType.NVarChar, 4);
				parmArray[16].Value = letterRestriction;
				parmArray[17] = new SqlParameter("@letterradio  ", SqlDbType.Bit);
				parmArray[17].Value =1;// letterRadio;
				parmArray[18] = new SqlParameter("@letterstart", SqlDbType.DateTime); 
				parmArray[18].Value = letterStart;
				parmArray[19] = new SqlParameter("@letterfinish", SqlDbType.DateTime); 
				parmArray[19].Value = letterFinish;
				parmArray[20] = new SqlParameter("@actionrestriction", SqlDbType.NVarChar, 4);
				parmArray[20].Value = actionRestriction;
				parmArray[21] = new SqlParameter("@actiondatestart", SqlDbType.DateTime); 
				parmArray[21].Value = actiondateStart;
				parmArray[22] = new SqlParameter("@actiondatefinish", SqlDbType.DateTime); 
				parmArray[22].Value = actiondateFinish;
				parmArray[23] = new SqlParameter("@includephone", SqlDbType.Bit);
				parmArray[23].Value = 1;//includePhone;
				parmArray[24] = new SqlParameter("@includeaddresses", SqlDbType.Bit);
				parmArray[24].Value = 1;//includeAddress;
				parmArray[25] = new SqlParameter("@empeeno ", SqlDbType.Int);
				parmArray[25].Value = empeeno;
				parmArray[26] = new SqlParameter("@empeetype ", SqlDbType.NVarChar,4);
				parmArray[26].Value = employeeType;
				parmArray[27] = new SqlParameter("@branch  ", SqlDbType.SmallInt);
				parmArray[27].Value = branchno;
				parmArray[28] = new SqlParameter("@proppoints", SqlDbType.SmallInt);
				parmArray[28].Value = proposalPoints;
				parmArray[29] = new SqlParameter("@proppointsdirection", SqlDbType.NVarChar,2);
				parmArray[29].Value = propPointsDirection ;
				parmArray[30] = new SqlParameter("@coderestriction", SqlDbType.NVarChar, 3);
				parmArray[30].Value = codeRestriction;
				parmArray[31] = new SqlParameter("@accountbranch", SqlDbType.NVarChar,4);
				parmArray[31].Value = accountBranch;
				parmArray[32] = new SqlParameter("@viewTop", SqlDbType.Bit);
				parmArray[32].Value = viewTop;
				parmArray[33] = new SqlParameter("@code", SqlDbType.NVarChar,4);
				parmArray[33].Value = code;
				parmArray[34] = new SqlParameter("@rowLimited", SqlDbType.Bit);
				parmArray[34].Value = rowLimited;
				parmArray[34].Direction = ParameterDirection.Output;
				parmArray[35] = new SqlParameter("@user", SqlDbType.Int);
				parmArray[35].Value = this.User;
				parmArray[36] = new SqlParameter("@actionsfilter", SqlDbType.SmallInt);
				parmArray[36].Value = numActions;
				parmArray[37] = new SqlParameter("@actionsoperand", SqlDbType.NVarChar,1);
				parmArray[37].Value = actionOperand;
				parmArray[38] = new SqlParameter("@actionsthisempeeno", SqlDbType.SmallInt);
				parmArray[38].Value = restrictEmployee;
				parmArray[39] = new SqlParameter("@balance", SqlDbType.Money);
				parmArray[39].Value = balance;
				parmArray[40] = new SqlParameter("@balanceoperand", SqlDbType.NChar,1);
				parmArray[40].Value = balanceOperand;
				parmArray[41] = new SqlParameter("@arrearsinccharges", SqlDbType.SmallInt);
				parmArray[41].Value = includeCharges;
				parmArray[42] = new SqlParameter("@datemovedarrsfrom", SqlDbType.DateTime);
				parmArray[42].Value = dateMovedFrom;
				parmArray[43] = new SqlParameter("@datemovedarrsto", SqlDbType.DateTime);
				parmArray[43].Value = dateMovedTo;
				parmArray[44] = new SqlParameter("@datemovedrestriction", SqlDbType.NVarChar,2);
				parmArray[44].Value = dateMovedRestriction;
				parmArray[45] = new SqlParameter("@datelastpaid", SqlDbType.DateTime);
				parmArray[45].Value = datelastPaid;
				parmArray[46] = new SqlParameter("@lastpaidoperand", SqlDbType.NChar,1);
				parmArray[46].Value = dateOperand;
				parmArray[47] = new SqlParameter("@actionDueDate", SqlDbType.Bit);
				parmArray[47].Value = actionDueDate;
                parmArray[48] = new SqlParameter("@credit", SqlDbType.Bit);
                parmArray[48].Value = credit;
                parmArray[49] = new SqlParameter("@cash", SqlDbType.Bit);
                parmArray[49].Value = cash;
                parmArray[50] = new SqlParameter("@branchset", SqlDbType.NVarChar,32);
                parmArray[50].Value = branchset;
                parmArray[51] = new SqlParameter("@service", SqlDbType.Bit);
                parmArray[51].Value = service;
                parmArray[52] = new SqlParameter("@worklist", SqlDbType.VarChar, 6);
                parmArray[52].Value = worklist;
                parmArray[53] = new SqlParameter("@deliveryarea", SqlDbType.VarChar, 8);
                parmArray[53].Value = deliveryArea;

                result = this.RunSP("dn_getacctsforalloc_5_2_sp", parmArray, arrearAccounts);
        
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
         return arrearAccounts;
      }

      /// <summary>
      /// Returns a DataSet that contains all the accounts in the bailiff or collector strategies
      /// </summary>
      /// <returns></returns>
      public DataSet GetStrategyAccountsToAllocate()
      {
         DataSet strategyAccounts = new DataSet();
         try
         {
            this.RunSP("GetStrategyAccountsToAllocateSP", strategyAccounts);
         }
         catch (SqlException ex)
         {
            LogSqlException(ex);
            throw ex;
         }
         return strategyAccounts;
      }

		/// <summary>
		/// Checks to see whether the account exists. If it does it returns the customer ID
		/// </summary>
		/// <param name="accountNumber"></param>
		/// <returns></returns>
		/// 
		public int GetStage1AccountSummary(string customerID)
		{
			try
			{
				_stage1 = new DataTable(TN.Stage1);
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
				parmArray[0].Value = customerID;
				parmArray[1] = new SqlParameter("@currentAccounts", SqlDbType.Int);
				parmArray[1].Value = 0;
				parmArray[2] = new SqlParameter("@settledAccounts", SqlDbType.Int);
				parmArray[2].Value = 0;
				foreach(SqlParameter p in parmArray)
					p.Direction = ParameterDirection.Output;
				parmArray[0].Direction = ParameterDirection.Input;

				result = this.RunSP("DN_AccountGetStage1SummarySP", parmArray, _stage1);

				if(!Convert.IsDBNull(parmArray[1].Value))
					this.CurrentAccounts = (int)parmArray[1].Value;
				if(!Convert.IsDBNull(parmArray[2].Value))
					this.SettledAccounts = (int)parmArray[2].Value;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

		/// <summary>
		/// Checks whether the account specified is a sole or joint account
		/// and returns the joint customer id as a property if the account
		/// is joint of spouse.
		/// </summary>
		/// <param name="accountNumber"></param>
		/// <returns></returns>
		public int SoleOrJoint(string accountNumber, string relation)
		{
			try
			{
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NChar, 12);
				parmArray[0].Value = accountNumber;
				parmArray[1] = new SqlParameter("@jntcustid", SqlDbType.NVarChar,20);
				parmArray[1].Direction = ParameterDirection.Output;
				parmArray[1].Value = "";
				parmArray[2] = new SqlParameter("@hldorjnt", SqlDbType.NVarChar, 1);
				parmArray[2].Value = relation;

				result = this.RunSP("DN_AccountSoleOrJointSP", parmArray);
                
				if(result == 0)
				{
					_jointcustid = (string)parmArray[1].Value;
				}
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

		/// <summary>
		/// Returns account type, paid percent and terms type of the account specified
		/// </summary>
		/// <param name="accountNumber"></param>
		/// <returns></returns>
		public int GetAccount(string accountNumber)
		{
			try
			{
				parmArray = new SqlParameter[6];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NChar, 12);
				parmArray[0].Value = accountNumber;
				parmArray[1] = new SqlParameter("@accttype", SqlDbType.NVarChar,1);
				parmArray[1].Direction = ParameterDirection.Output;
				parmArray[1].Value = "";
				parmArray[2] = new SqlParameter("@paidpcent", SqlDbType.SmallInt);
				parmArray[2].Direction = ParameterDirection.Output;
				parmArray[2].Value = 0;
				parmArray[3] = new SqlParameter("@termstype", SqlDbType.NVarChar,2);
				parmArray[3].Direction = ParameterDirection.Output;
				parmArray[3].Value = "";
                parmArray[4] = new SqlParameter("@isAmortized", SqlDbType.Bit);
				parmArray[4].Direction = ParameterDirection.Output;
				parmArray[4].Value = "";
                // Added a new parameter @isAmortizedOutStandingBal, to check wether account has isAmortizedOutStandingBal true or false
                // Added by RD, 18/06/2019
                parmArray[5] = new SqlParameter("@isAmortizedOutStandingBal ", SqlDbType.Bit);
                parmArray[5].Direction = ParameterDirection.Output;
                parmArray[5].Value = "";

                result = this.RunSP("DN_AccountGetSP", parmArray);

				AccountType = (string)parmArray[1].Value;
				PaidPcent = (short)parmArray[2].Value;
				TermsType = (string)parmArray[3].Value;
                //2019Feb23 - validate account is Cashloan account or not 
                IsAmortized = (bool)parmArray[4].Value;
                IsAmortizedOutStandingBal = (bool)parmArray[5].Value;
            }
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}


        // Archiving
        public int ArchiveAccounts()
		{
		try
			{
				
			result = this.RunSP("DN_archivecurrentdatabase");
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

		public int GetAccount(SqlConnection conn, SqlTransaction trans, string accountNumber)
		{
			try
			{
				parmArray = new SqlParameter[4];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NChar, 12);
				parmArray[0].Value = accountNumber;
				parmArray[1] = new SqlParameter("@accttype", SqlDbType.NVarChar,1);
				parmArray[1].Direction = ParameterDirection.Output;
				parmArray[1].Value = "";
				parmArray[2] = new SqlParameter("@paidpcent", SqlDbType.SmallInt);
				parmArray[2].Direction = ParameterDirection.Output;
				parmArray[2].Value = 0;
				parmArray[3] = new SqlParameter("@termstype", SqlDbType.NVarChar,2);
				parmArray[3].Direction = ParameterDirection.Output;
				parmArray[3].Value = "";

				result = this.RunSP(conn, trans, "DN_AccountGetSP", parmArray);

				AccountType = (string)parmArray[1].Value;
				PaidPcent = (short)parmArray[2].Value;
				TermsType = (string)parmArray[3].Value;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

        public int GetInstantCreditAwaitingClearance(string branchRestriction,
            int includeHP,
            int includeRF,
            string holdFlags, SqlConnection conn, SqlTransaction trans
            )
        {
            try
            {
                _icClearance = new DataTable();
                parmArray = new SqlParameter[4];
                parmArray[0] = new SqlParameter("@BranchRestriction", SqlDbType.NVarChar, 4);
                parmArray[0].Value = branchRestriction;
                parmArray[1] = new SqlParameter("@includeHP", SqlDbType.Int);
                parmArray[1].Value = includeHP;
                parmArray[2] = new SqlParameter("@includeRF", SqlDbType.Int);
                parmArray[2].Value = includeRF;
                parmArray[3] = new SqlParameter("@holdflags", SqlDbType.NVarChar, 4);
                parmArray[3].Value = holdFlags;
                result = this.RunSP(conn,trans,"DN_InstantCreditLoadClearance", parmArray, _icClearance);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;
        }

		public int GetAccountsAwaitingClearance(string branchRestriction,
			int includeCash,
			int includeHP,
			int includeRF,
			int includePaid, 
			int includeUnpaid,
			int includeItems,
			string holdFlags,
			int includeGOL)
		{
			try
			{
				_awaitingClearance = new DataTable();
				parmArray = new SqlParameter[9];
				parmArray[0] = new SqlParameter("@BranchRestriction", SqlDbType.NVarChar, 4);
				parmArray[0].Value = branchRestriction;
				parmArray[1] = new SqlParameter("@includecash", SqlDbType.Int);
				parmArray[1].Value = includeCash;
				parmArray[2] = new SqlParameter("@includeHP", SqlDbType.Int);
				parmArray[2].Value = includeHP;
				parmArray[3] = new SqlParameter("@includeRF", SqlDbType.Int);
				parmArray[3].Value = includeRF;
				parmArray[4] = new SqlParameter("@includePaid", SqlDbType.Int);
				parmArray[4].Value = includePaid;
				parmArray[5] = new SqlParameter("@includeUnpaid", SqlDbType.Int);
				parmArray[5].Value = includeUnpaid;
				parmArray[6] = new SqlParameter("@includeItems", SqlDbType.Int);
				parmArray[6].Value = includeItems;
				parmArray[7] = new SqlParameter("@holdflags", SqlDbType.NVarChar, 4);
				parmArray[7].Value = holdFlags;
				parmArray[8] = new SqlParameter("@includeGOL", SqlDbType.Int);
				parmArray[8].Value = includeGOL;
				result = this.RunSP("DN_AccountLoadClearance", parmArray, _awaitingClearance);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

		/// <summary>
		/// Returns an array of account deatils.
		/// </summary>
		/// <param name="customerID"></param>
		/// <returns></returns>
		public int AccountsSearch(	string accountNo, 
			string custId, 
			string firstName,
			string lastName,
			string address,
			string postCode,
            string phoneNumber,         //CR1084
			string accountStatus,
			int limit,
			bool exactMatch,
            string storeType,
			out int	accountExists,
			out string accountType)
		{
			try
			{
				accountExists = 0;
            accountType = String.Empty;
				_accountslist = new DataTable(TN.Accounts);
			
				// Make an array of input parameters
				parmArray = new SqlParameter[13];           //CR1084
				parmArray[0] = new SqlParameter("@accountNo", SqlDbType.NVarChar,50);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@custId", SqlDbType.NVarChar,50);
				parmArray[1].Value = custId;
				parmArray[2] = new SqlParameter("@firstName", SqlDbType.NVarChar,50);
				parmArray[2].Value = firstName;
				parmArray[3] = new SqlParameter("@lastName", SqlDbType.NVarChar,50);
				parmArray[3].Value = lastName;
				parmArray[4] = new SqlParameter("@address", SqlDbType.NVarChar,50);
				parmArray[4].Value = address;
				parmArray[5] = new SqlParameter("@postCode", SqlDbType.NVarChar,50);
				parmArray[5].Value = postCode;
				parmArray[6] = new SqlParameter("@accountStatus", SqlDbType.NVarChar,50);
				parmArray[6].Value = accountStatus;
				parmArray[7] = new SqlParameter("@limit", SqlDbType.Int);
				parmArray[7].Value = limit;
				parmArray[8] = new SqlParameter("@exact", SqlDbType.Int);
				parmArray[8].Value = Convert.ToInt32(exactMatch);
                parmArray[9] = new SqlParameter("@storetype", SqlDbType.NVarChar, 2);
                parmArray[9].Value = storeType;
                parmArray[10] = new SqlParameter("@accountExists", SqlDbType.Int);
				parmArray[10].Value = accountExists;
				parmArray[10].Direction = ParameterDirection.Output;
                parmArray[11] = new SqlParameter("@accountType", SqlDbType.NChar,1);
                parmArray[11].Value = accountType;
                parmArray[11].Direction = ParameterDirection.Output;
                parmArray[12] = new SqlParameter("@phoneNumber", SqlDbType.NVarChar, 20);       //CR1084
                parmArray[12].Value = phoneNumber;          //CR1084
			
				result = this.RunSP("DN_AccountsSearchSP", parmArray, _accountslist);

				if(result==0)
				{
					accountExists = (int)parmArray[10].Value;
               accountType = parmArray[11].Value.ToString();
					result = (int)Return.Success;
				
				}
				else
				{
					result = (int)Return.Fail;
				}
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}			
			return result;
		}



        /// <summary>
		/// Returns an array of account deatils.
		/// </summary>
		/// <param name="customerID"></param>
		/// <returns></returns>
		public void InvoiceAccountsSearch(int BranchNo,
            DateTime  InvoiceDateFrom,
            DateTime InvoiceDateTo,
            string InvoiceNo,
            string accountNo 
            //int Pageindex, 
            //int PageSize,
            //out int RecordCount



            )
        {
            try
            {
                //RecordCount = 0;
                _accountslist = new DataTable(TN.Accounts);
              
               
                // Make an array of input parameters
                parmArray = new SqlParameter[5];           
                parmArray[0] = new SqlParameter("@branchno", SqlDbType.SmallInt , 2);
                parmArray[0].Value = BranchNo;
                parmArray[1] = new SqlParameter("@invoiceDateFrom", SqlDbType.DateTime2 , 50);
                parmArray[1].Value = InvoiceDateFrom;
                parmArray[2] = new SqlParameter("@invoiceDateTo", SqlDbType.DateTime2, 50);
                parmArray[2].Value = InvoiceDateTo;
                parmArray[3] = new SqlParameter("@invoiceNo", SqlDbType.NVarChar, 50);
                parmArray[3].Value = InvoiceNo;
                parmArray[4] = new SqlParameter("@accountNo", SqlDbType.NVarChar, 50);
                parmArray[4].Value = accountNo;
                //parmArray[5] = new SqlParameter("@PageIndex", SqlDbType.SmallInt, 2);
                //parmArray[5].Value = Pageindex;
                //parmArray[6] = new SqlParameter("@PageSize", SqlDbType.Int);
                //parmArray[6].Value = PageSize;
                //parmArray[7] = new SqlParameter("@RecordCount", SqlDbType.Int, 4);
                //parmArray[7].Value = "";
                //parmArray[7].Direction = ParameterDirection.Output;
                result  = this.RunSP("SearchInvoices", parmArray, _accountslist);    
                //if(result ==0)
                //{
                //    RecordCount = (int)parmArray[7].Value;
                //}
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            
        }



        public int IsAccountValidForOnlyNonStockSale(string accountNumber) 
        {
            try
            {
                _accountdetails = new DataTable(TN.AccountDetails);
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar, 50);
                parmArray[0].Value = accountNumber;
            
               result = this.RunSP("IsAccountValidForOnlyNonStockSale", parmArray, _accountdetails);
        
                if (result == 0)
                {
                    result = (int)Return.Success;
                }
                else
                {
                    result = (int)Return.Fail;
                }
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;
        }

        public int ValidateNonSaleableNonStocks(string productSKU) 
        {
            try
            {
                result = (int)Return.Fail;

                _accountdetails = new DataTable(TN.AccountDetails);
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@sku", SqlDbType.NVarChar, 50);
                parmArray[0].Value = productSKU;
                result = this.RunSP("ValidateNonSaleableNonStocks", parmArray, _accountdetails);
                if (result == 0)
                {
                    result = (int)Return.Success;
                }
                else
                {
                    result = (int)Return.Fail;
                }
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;
        }

        public void SaveAccountForSaleOnlyNonStock(SqlConnection conn, SqlTransaction trans, string acctno)
        {
            parmArray = new SqlParameter[1];
            parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
            parmArray[0].Value = acctno;

            this.RunSP("SaveAccountForSaleOnlyNonStock", parmArray);
        }
		public int UpdateMaxSpendAndRFCreditLimit(SqlConnection conn, SqlTransaction trans, string customerId, decimal RFCreditLimit, decimal MaxSpend)
		{
			try
			{
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@cust ", SqlDbType.NVarChar, 20);
				parmArray[0].Value = customerId;
				parmArray[1] = new SqlParameter("@RFCreditLimit ", SqlDbType.Money);
				parmArray[1].Value = RFCreditLimit;
				parmArray[2] = new SqlParameter("@MaxSpend ", SqlDbType.Money);
				parmArray[2].Value = MaxSpend;

				result = this.RunSP("SP_BS_UpdateMaxSpendandRFLimit", parmArray);

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
		public void DisableSave(SqlConnection conn, SqlTransaction trans, string acctno,out string invoice,out int status)
		{
			 status = 0;
			 invoice = " ";

			try
			{
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@cosacsAcctNo ", SqlDbType.VarChar, 20);
				parmArray[0].Value = acctno;
				parmArray[1] = new SqlParameter("@InvoiceNumber", SqlDbType.VarChar,20);
				parmArray[1].Value = 0;
				parmArray[1].Direction = ParameterDirection.Output;
				parmArray[2] = new SqlParameter("@Status", SqlDbType.Int);
				parmArray[2].Value = 0;
				parmArray[2].Direction = ParameterDirection.Output;

				
					RunSP(conn, trans, "SP_BS_CheckMambuAccount", parmArray);
				
				if (!Convert.IsDBNull(parmArray[1].Value))
					invoice = (string)parmArray[1].Value;
				if (!Convert.IsDBNull(parmArray[2].Value))
					status = (int)parmArray[2].Value;

			}
			catch (SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			
		}
		
		public void DisableRFButton(SqlConnection conn, SqlTransaction trans, string custid, out int status) //Arunnew
		{
			status = 0;

			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@cosacscustId", SqlDbType.VarChar, 20);
				parmArray[0].Value = custid;
				parmArray[1] = new SqlParameter("@Status", SqlDbType.Int, 20);
				parmArray[1].Value = 0;
				parmArray[1].Direction = ParameterDirection.Output;	

				RunSP(conn, trans, "SP_BS_DisableRFButton", parmArray);
		
				if (!Convert.IsDBNull(parmArray[1].Value))
					status = (int)parmArray[1].Value;

			}
			catch (SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}

		}

		public bool IsDepositForRefund(SqlConnection conn, SqlTransaction trans, string accountNo,
			decimal amount, short payMethod, int fintransId)
		{
			bool isDeposit = false;

			try
			{
				parmArray = new SqlParameter[5];
				parmArray[0] = new SqlParameter("@AccountNo ", SqlDbType.VarChar, 20);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@Amount", SqlDbType.Money);
				parmArray[1].Value = amount;
				parmArray[2] = new SqlParameter("@PayMethod", SqlDbType.Int);
				parmArray[2].Value = payMethod;
				parmArray[3] = new SqlParameter("@FintransId", SqlDbType.Int);
				parmArray[3].Value = fintransId;
				parmArray[4] = new SqlParameter("@IsDeposit", SqlDbType.Bit);
				parmArray[4].Direction = ParameterDirection.Output;

				RunSP(conn, trans, "SP_BS_IsDepositForRefund", parmArray);

				if (!Convert.IsDBNull(parmArray[4].Value))
					isDeposit = (bool)parmArray[4].Value;
			}
			catch (SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return isDeposit;
		}

		public int Paymentref(SqlConnection conn, SqlTransaction trans, string accountNo, decimal amount)
		{

			int transrefno = 0;

			try
			{
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@AccountNo ", SqlDbType.VarChar, 20);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@Amount", SqlDbType.VarChar, 20);
				parmArray[1].Value = amount;
				parmArray[2] = new SqlParameter("@TransRefNo", SqlDbType.Int);
				parmArray[2].Value = 0;
				parmArray[2].Direction = ParameterDirection.Output;


				RunSP(conn, trans, "SP_BS_GetTransRefNoForCORorREF", parmArray);

				
				if (!Convert.IsDBNull(parmArray[2].Value))
					transrefno = (int)parmArray[2].Value;

			}
			catch (SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return transrefno;
		}
        /// <summary>
        /// Returns an array of account deatils.
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns></returns>
        /// 
        public int GetAccountDetails(SqlConnection conn, SqlTransaction trans, string accountNumber, int ageementNumber) //IP - 11/02/11 - Sprint 5.10 - #2978 - Added conn, trans
		{
			try
			{
				_accountdetails = new DataTable(TN.AccountDetails);			
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar,50);
				parmArray[0].Value = accountNumber;
                parmArray[1] = new SqlParameter("@agrmtno", SqlDbType.Int);
                parmArray[1].Value = ageementNumber;


                if (conn != null && trans != null) //IP - 11/02/11 - Sprint 5.10 - #2978 - Added conn, trans
                    result = this.RunSP(conn, trans,"DN_AccountGetDetailsSP", parmArray, _accountdetails);
                else
				    result = this.RunSP("DN_AccountGetDetailsSP", parmArray, _accountdetails);
			
				if(result==0)
				{
					result = (int)Return.Success;
				}
				else
				{
					result = (int)Return.Fail;
				}
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

      /// <summary>
      /// Checks for whether or not an account is locked when that account is opened in Goods Return
      /// </summary>
      /// <param name="accountNumber"></param>
      /// <param name="ageementNumber"></param>
      /// <returns></returns>
      public int CheckAccountLocked(string accountNumber, int user)
      {
         try
         {
            parmArray = new SqlParameter[2];
            parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar, 12);
            parmArray[0].Value = accountNumber;
            parmArray[1] = new SqlParameter("@user", SqlDbType.Int);
            parmArray[1].Value = user;

            result = this.RunSPwithExecuteScalar("AccountLockingSelectReviseAccountSP", parmArray);
         }
         catch (SqlException ex)
         {
            LogSqlException(ex);
            throw ex;
         }
         return result;
      }

      /// <summary>
      /// Locks the account with a CurrentAction of 'R'
      /// </summary>
      /// <param name="accountNumber"></param>
      /// <param name="user"></param>
      public void LockAccountForGoodsReturn(SqlConnection conn, SqlTransaction trans, string accountNo, int user)
      {
         try
         {
            parmArray = new SqlParameter[2];
            parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar, 12);
            parmArray[0].Value = accountNo;
            parmArray[1] = new SqlParameter("@user", SqlDbType.Int);
            parmArray[1].Value = user;

            result = this.RunSPwithExecuteScalar(conn, trans, "AccountLockingInsertReviseAccountSP", parmArray);
         }
         catch (SqlException ex)
         {
            LogSqlException(ex);
            throw ex;
         }
      }


		/// <summary>
		/// Returns a datatable of unpaid account details.
		/// </summary>
		/// <param name="branchNo">The branch to retrieve accounts for.</param>
		/// <param name="empeeNoSale">The employee number to limit accounts for.</param>
		/// <returns>int Status value</returns>
		public int GetUnpaidAccounts(int branchNo, int empeeNoSale)
		{
			try
			{
				_accountdetails = new DataTable(TN.Accounts);			
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@branchno", SqlDbType.SmallInt);
				parmArray[0].Value = branchNo;
				parmArray[1] = new SqlParameter("@empeenosale", SqlDbType.Int); //IP - 24/07/09 - UAT(691) Changed type from smallint to int
				parmArray[1].Value = empeeNoSale;
				result = this.RunSP("DN_UnpaidAccountsLoadSP", parmArray, _accountdetails);
			
				if(result==0)
				{
					result = (int)Return.Success;
				}
				else
				{
					result = (int)Return.Fail;
				}
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

		/// <summary>
		/// Returns ChargesData for a given AcctNo.
		/// </summary>
		/// <param name="acctNo">The acct no to retrieve data for</param>
		/// <returns>int Status value</returns>
		public int GetChargesByAcctNo(string acctNo)
		{
			try
			{
				_accountdetails = new DataTable(TN.AccountDetails);			
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar,12);
				parmArray[0].Value = acctNo;
				result = this.RunSP("DN_ChargesdataGetbyAcctnoSP", parmArray, _accountdetails);
			
				if(result==0)
				{
					result = (int)Return.Success;
				}
				else
				{
					result = (int)Return.Fail;
				}
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

		/// <summary>
		/// Returns ArrearsDaily data for a given AcctNo.
		/// </summary>
		/// <param name="acctNo">The acct no to retrieve data for</param>
		/// <returns>int Status value</returns>
		public int GetArrearsDailyByAcctNo(string acctNo)
		{
			try
			{
				_accountdetails = new DataTable(TN.AccountDetails);			
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar,12);
				parmArray[0].Value = acctNo;
				result = this.RunSP("DN_ArrearsDailyGetbyAcctnoSP", parmArray, _accountdetails);
			
				if(result==0)
				{
					result = (int)Return.Success;
				}
				else
				{
					result = (int)Return.Fail;
				}
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

		/// <summary>
		/// Returns the name and custid and acctno for adding customer and account codes
		/// </summary>
		/// <param name="customerID"></param>
		/// <returns></returns>
		/// 
		public int GetAccountName(string accountNumber, string customerID)
		{
			try
			{
				_accountName = new DataTable("AccountName");			
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@acct", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNumber;
				parmArray[1] = new SqlParameter("@cust", SqlDbType.NVarChar,20);  // 68218 RD 28/06/06 Modifed length from 12 to 20
				parmArray[1].Value = customerID;

				result = this.RunSP("DN_AccountFindNameSP", parmArray, _accountName);
			
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

		/// <summary>
		/// Returns the codes currently associated with an account
		/// </summary>
		/// <param name="accountNumber"></param>
		/// <returns></returns>
		/// 
		public int GetCodesOnAccount(string accountNumber, out bool noSuchAccount)
		{
			try
			{
				_accountCodes = new DataTable("AccountCodes");			
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNumber;

				result = this.RunSP("DN_AccountGetCodesOnSP", parmArray, _accountCodes);

				noSuchAccount = result == -1?true:false;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
            }
			return result;
		}

		public int AddCodeToAccount(SqlConnection con, SqlTransaction tran, string accountNumber, string code, 
                                    DateTime date, int employeeNumber, string reference)
		{
			try
			{
				parmArray = new SqlParameter[5];
				parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNumber;
				parmArray[1] = new SqlParameter("@code", SqlDbType.NVarChar,4);
				parmArray[1].Value = code;
				parmArray[2] = new SqlParameter("@date", SqlDbType.DateTime);
				parmArray[2].Value = date;
				parmArray[3] = new SqlParameter("@empno", SqlDbType.Int);
				parmArray[3].Value = employeeNumber;
                parmArray[4] = new SqlParameter("@reference", SqlDbType.NVarChar, 10);
                parmArray[4].Value = reference;

				result = this.RunSP(con, tran, "DN_AccountAddCodeToSP", parmArray);
			
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

		public int DeleteCodesFromAccount(SqlConnection con, SqlTransaction tran, string accountNumber)
		{
			try
			{
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNumber;

				result = this.RunSP(con, tran, "DN_AccountDeleteCodesFromSP", parmArray);
			
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

		public int DeleteCodeFromAccount(SqlConnection con, SqlTransaction tran, string accountNumber, string code,
                                         string reference)
		{
			try
			{
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNumber;
				parmArray[1] = new SqlParameter("@code", SqlDbType.NVarChar,4);
				parmArray[1].Value = code;
                parmArray[2] = new SqlParameter("@reference", SqlDbType.NVarChar, 10);
                parmArray[2].Value = reference;

				this.RunSP(con, tran, "DN_AccountDeleteCodeFromSP", parmArray);
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
				parmArray = new SqlParameter[25];
				parmArray[0] = new SqlParameter("@origBr", SqlDbType.SmallInt);
				parmArray[0].Value = this.OrigBr;
				parmArray[1] = new SqlParameter("@acctNo", SqlDbType.NVarChar,12);
				parmArray[1].Value = this.AccountNumber;
				parmArray[2] = new SqlParameter("@acctType", SqlDbType.NChar, 1);
				parmArray[2].Value = this.AccountType;
				parmArray[3] = new SqlParameter("@dateAcctOpen", SqlDbType.DateTime);
				parmArray[3].Value = this.DateAccountOpen;
				parmArray[4] = new SqlParameter("@creditDays", SqlDbType.SmallInt); 
				parmArray[4].Value = this.CreditDays;
				parmArray[5] = new SqlParameter("@agreementTotal", SqlDbType.Money);
				parmArray[5].Value = this.AgreementTotal;
				parmArray[6] = new SqlParameter("@dateLastPaid", SqlDbType.DateTime);
				if(this.DateLastPaid.Year == 1900)
					parmArray[6].Value = DBNull.Value;
				else
					parmArray[6].Value = this.DateLastPaid;
				parmArray[7] = new SqlParameter("@as400bal", SqlDbType.Money);
				parmArray[7].Value = this.AS400Bal;
				parmArray[8] = new SqlParameter("@outstandingBal", SqlDbType.Money);
				parmArray[8].Value = this.OutstandingBalance;
				parmArray[9] = new SqlParameter("@arrears", SqlDbType.Money);
				parmArray[9].Value = this.Arrears;
				parmArray[10] = new SqlParameter("@highestStatus", SqlDbType.NChar,1);
				parmArray[10].Value = this.HighestStatus;
				parmArray[11] = new SqlParameter("@currentStatus", SqlDbType.NChar,1);
				parmArray[11].Value = this.CurrentStatus;
				parmArray[12] = new SqlParameter("@highStatusDays", SqlDbType.SmallInt);
				parmArray[12].Value = this.HighestStatusDays;
				parmArray[13] = new SqlParameter("@branchNo", SqlDbType.SmallInt);
				parmArray[13].Value = this.BranchNo;
				parmArray[14] = new SqlParameter("@paidPcent", SqlDbType.SmallInt);
				parmArray[14].Value = this.PaidPcent;
				parmArray[15] = new SqlParameter("@termsType", SqlDbType.NVarChar,2);
				parmArray[15].Value = this.TermsType;
				parmArray[16] = new SqlParameter("@repossArrears", SqlDbType.Money);
				parmArray[16].Value = this.RepossArrears;
				parmArray[17] = new SqlParameter("@repossValue", SqlDbType.Money);
				parmArray[17].Value = this.RepossValue;
				parmArray[18] = new SqlParameter("@dateIntoArrears", SqlDbType.DateTime);
				if(this.DateIntoArrears == DateTime.MinValue.AddYears(1899))
					parmArray[18].Value = DBNull.Value;
				else
					parmArray[18].Value = this.DateIntoArrears;
				parmArray[19] = new SqlParameter("@country", SqlDbType.NVarChar,2);
				parmArray[19].Value = this.Country;
				parmArray[20] = new SqlParameter("@lastupdatedby", SqlDbType.Int);
				parmArray[20].Value = this.User;
				parmArray[21] = new SqlParameter("@bdwbalance", SqlDbType.Money);
				parmArray[21].Value = this.BDWBalance;
				parmArray[22] = new SqlParameter("@bdwcharges", SqlDbType.Money);
				parmArray[22].Value = this.BDWCharges;
				parmArray[23] = new SqlParameter("@securitised", SqlDbType.NVarChar,2);
				parmArray[23].Value = this.Securitised;
                parmArray[24] = new SqlParameter("@haslineitems", SqlDbType.Bit);
                parmArray[24].Value = this.HasLineItems;
               // parmArray[25] = new SqlParameter("@isAmortized", SqlDbType.Bit);
               // parmArray[25].Value = this.IsAmortized;

                this.RunSP(conn, trans, "DN_AccountUpdateSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public bool Populate(SqlConnection conn, SqlTransaction trans, string accountNo)
		{
			bool exists = false;
			try
			{
				parmArray = new SqlParameter[24];
				parmArray[0] = new SqlParameter("@origBr", SqlDbType.SmallInt);
				parmArray[0].Value = 0;
				parmArray[0].Direction = ParameterDirection.Output;
				parmArray[1] = new SqlParameter("@acctNo", SqlDbType.NVarChar,12);
				parmArray[1].Value = accountNo;
				parmArray[2] = new SqlParameter("@acctType", SqlDbType.NChar, 1);
				parmArray[2].Value = "";
				parmArray[2].Direction = ParameterDirection.Output;
				parmArray[3] = new SqlParameter("@dateAcctOpen", SqlDbType.DateTime);
				parmArray[3].Direction = ParameterDirection.Output;
				parmArray[4] = new SqlParameter("@creditDays", SqlDbType.SmallInt);
				parmArray[4].Value = 0;
				parmArray[4].Direction = ParameterDirection.Output;
				parmArray[5] = new SqlParameter("@agreementTotal", SqlDbType.Money);
				parmArray[5].Value = 0;
				parmArray[5].Direction = ParameterDirection.Output;
				parmArray[6] = new SqlParameter("@dateLastPaid", SqlDbType.DateTime);
				parmArray[6].Direction = ParameterDirection.Output;
				parmArray[7] = new SqlParameter("@as400bal", SqlDbType.Money);
				parmArray[7].Value = 0;
				parmArray[7].Direction = ParameterDirection.Output;
				parmArray[8] = new SqlParameter("@outstandingBal", SqlDbType.Money);
				parmArray[8].Value = 0;
				parmArray[8].Direction = ParameterDirection.Output;
				parmArray[9] = new SqlParameter("@arrears", SqlDbType.Money);
				parmArray[9].Value = 0;
				parmArray[9].Direction = ParameterDirection.Output;
				parmArray[10] = new SqlParameter("@highestStatus", SqlDbType.NChar,1);
				parmArray[10].Value = "";
				parmArray[10].Direction = ParameterDirection.Output;
				parmArray[11] = new SqlParameter("@currentStatus", SqlDbType.NChar,1);
				parmArray[11].Value = "";
				parmArray[11].Direction = ParameterDirection.Output;
				parmArray[12] = new SqlParameter("@highStatusDays", SqlDbType.SmallInt);
				parmArray[12].Value = 0;
				parmArray[12].Direction = ParameterDirection.Output;
				parmArray[13] = new SqlParameter("@branchNo", SqlDbType.SmallInt);
				parmArray[13].Value = 0;
				parmArray[13].Direction = ParameterDirection.Output;
				parmArray[14] = new SqlParameter("@paidPcent", SqlDbType.SmallInt);
				parmArray[14].Value = 0;
				parmArray[14].Direction = ParameterDirection.Output;
				parmArray[15] = new SqlParameter("@termsType", SqlDbType.NVarChar,2);
				parmArray[15].Value = "";
				parmArray[15].Direction = ParameterDirection.Output;
				parmArray[16] = new SqlParameter("@repossArrears", SqlDbType.Money);
				parmArray[16].Value = 0;
				parmArray[16].Direction = ParameterDirection.Output;
				parmArray[17] = new SqlParameter("@repossValue", SqlDbType.Money);
				parmArray[17].Value = 0;
				parmArray[17].Direction = ParameterDirection.Output;
				parmArray[18] = new SqlParameter("@dateIntoArrears", SqlDbType.DateTime);
				parmArray[18].Direction = ParameterDirection.Output;
				parmArray[19] = new SqlParameter("@country", SqlDbType.NVarChar,2);
				parmArray[19].Value = "";
				parmArray[19].Direction = ParameterDirection.Output;
				parmArray[20] = new SqlParameter("@custid", SqlDbType.NVarChar,20);
				parmArray[20].Value = "";
				parmArray[20].Direction = ParameterDirection.Output;
				parmArray[21] = new SqlParameter("@bdwbalance", SqlDbType.Money);
				parmArray[21].Value = 0;
				parmArray[21].Direction = ParameterDirection.Output;
				parmArray[22] = new SqlParameter("@bdwcharges", SqlDbType.Money);
				parmArray[22].Value = 0;
				parmArray[22].Direction = ParameterDirection.Output;
				parmArray[23] = new SqlParameter("@securitised", SqlDbType.NVarChar,2);
				parmArray[23].Value = 0;
				parmArray[23].Direction = ParameterDirection.Output;

				if(conn!=null && trans!=null)
					result = this.RunSP(conn, trans, "DN_AccountPopulateSP", parmArray);
				else
					result = this.RunSP("DN_AccountPopulateSP", parmArray);

				if(result == -1)
					exists = false;

				if(result == 0)
				{
					if(!Convert.IsDBNull(parmArray[0].Value))
						this.OrigBr = (short)parmArray[0].Value;
					if(!Convert.IsDBNull(parmArray[1].Value))
						this.AccountNumber = (string)parmArray[1].Value;
					if(!Convert.IsDBNull(parmArray[2].Value))
						this.AccountType = (string)parmArray[2].Value;
					if(!Convert.IsDBNull(parmArray[3].Value))
						this.DateAccountOpen = (DateTime)parmArray[3].Value;
					if(!Convert.IsDBNull(parmArray[4].Value))
						this.CreditDays = (short)parmArray[4].Value;
					if(!Convert.IsDBNull(parmArray[5].Value))
						this.AgreementTotal= (decimal)parmArray[5].Value;
					if(!Convert.IsDBNull(parmArray[6].Value))
						this.DateLastPaid = (DateTime)parmArray[6].Value;
					if(!Convert.IsDBNull(parmArray[7].Value))
						this.AS400Bal = (decimal)parmArray[7].Value;
                    //uat274 rdb 02/05/08 unable to settle some accounts due to outstbal and arrears being saved to 4 figures
					if(!Convert.IsDBNull(parmArray[8].Value))
						this.OutstandingBalance = decimal.Round((decimal)parmArray[8].Value, 2);	
					if(!Convert.IsDBNull(parmArray[9].Value))
						this.Arrears = decimal.Round((decimal)parmArray[9].Value,2);
					if(!Convert.IsDBNull(parmArray[10].Value))
						this.HighestStatus = (string)parmArray[10].Value;
					if(!Convert.IsDBNull(parmArray[11].Value))
						this.CurrentStatus = (string)parmArray[11].Value;
					if(!Convert.IsDBNull(parmArray[12].Value))
						this.HighestStatusDays = (short)parmArray[12].Value;
					if(!Convert.IsDBNull(parmArray[13].Value))
						this.BranchNo = (short)parmArray[13].Value;
					if(!Convert.IsDBNull(parmArray[14].Value))
						this.PaidPcent = (short)parmArray[14].Value;
					if(!Convert.IsDBNull(parmArray[15].Value))
						this.TermsType = (string)parmArray[15].Value;
					if(!Convert.IsDBNull(parmArray[16].Value))
						this.RepossArrears = (decimal)parmArray[16].Value;
					if(!Convert.IsDBNull(parmArray[17].Value))
						this.RepossValue = (decimal)parmArray[17].Value;
					if(!Convert.IsDBNull(parmArray[18].Value))
						this.DateIntoArrears = (DateTime)parmArray[18].Value;
					if(!Convert.IsDBNull(parmArray[19].Value))
						this.Country = (string)parmArray[19].Value;
					if(!Convert.IsDBNull(parmArray[20].Value))
						this.CustomerID = (string)parmArray[20].Value;
					if(!Convert.IsDBNull(parmArray[21].Value))
						this.BDWBalance = (decimal)parmArray[21].Value;
					if(!Convert.IsDBNull(parmArray[22].Value))
						this.BDWCharges = (decimal)parmArray[22].Value;
					if(!Convert.IsDBNull(parmArray[23].Value))
						this.Securitised = (string)parmArray[23].Value;
					exists = true;
				}
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return exists;
		}
		
		public void GetAccountForRevision(string accountNumber, int agreementNo)
		{
			try
			{
				_accountdetails = new DataTable(TN.AccountDetails);
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNumber;
				parmArray[1] = new SqlParameter("@agreementno", SqlDbType.Int);
				parmArray[1].Value = agreementNo;

				RunSP("DN_AccountGetForRevisionSP", parmArray, _accountdetails);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

        //#14626
        public void GetAccountForRevision(SqlConnection conn, SqlTransaction trans, string accountNumber, int agreementNo)
        {
            try
            {
                _accountdetails = new DataTable(TN.AccountDetails);
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar, 12);
                parmArray[0].Value = accountNumber;
                parmArray[1] = new SqlParameter("@agreementno", SqlDbType.Int);
                parmArray[1].Value = agreementNo;

                if (conn != null && trans != null)
                    result = this.RunSP(conn, trans, "DN_AccountGetForRevisionSP", parmArray, _accountdetails);
                else
                    result = RunSP("DN_AccountGetForRevisionSP", parmArray, _accountdetails);

             
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

		public void GetLinkedCustomerID(SqlConnection conn, SqlTransaction trans,string accountNumber)
		{
			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNumber;
				parmArray[1] = new SqlParameter("@custId", SqlDbType.NVarChar,20);
				parmArray[1].Value = "";
				parmArray[1].Direction = ParameterDirection.Output;

				this.RunSP(conn,trans, "DN_AccountGetLinkedCustomerSP", parmArray);
				if(!Convert.IsDBNull(parmArray[1].Value))
					_customerID = (string)parmArray[1].Value;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

        //include relationship
        public void GetLinkedCustomerIDbyType(SqlConnection conn, SqlTransaction trans, string accountNumber, string relationship)
        {
            try
            {
                parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar, 12);
                parmArray[0].Value = accountNumber;
                parmArray[1] = new SqlParameter("@custId", SqlDbType.NVarChar, 20);
                parmArray[1].Value = "";
                parmArray[1].Direction = ParameterDirection.Output;
                parmArray[2] = new SqlParameter("@relationship", SqlDbType.NVarChar, 1);
                parmArray[2].Value = relationship;

                this.RunSP(conn, trans, "CustacctGetCustidbyType", parmArray);
                if (!Convert.IsDBNull(parmArray[1].Value))
                    _customerID = (string)parmArray[1].Value;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }


		//transactional version
		public void AddCustomerToAccount(SqlConnection con, SqlTransaction trans, string accountNo, string customerID, string relationship, int user)
		{
			try
			{
				parmArray = new SqlParameter[4];
				parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@custId", SqlDbType.NVarChar,20);
				parmArray[1].Value = customerID;
				parmArray[2] = new SqlParameter("@relationship", SqlDbType.NVarChar,1);
				parmArray[2].Value = relationship;
				parmArray[3] = new SqlParameter("@user", SqlDbType.Int);
				parmArray[3].Value = user;

				this.RunSP(con, trans, "DN_AccountAddCustomerToSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public string GetLastRFAccount(SqlConnection con, SqlTransaction trans, string customerID)
		{
			string acctno = "";
			try
			{				
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar,20);
				parmArray[0].Value = customerID;
				parmArray[1] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[1].Value = "";
				parmArray[1].Direction = ParameterDirection.Output;

				this.RunSP(con, trans, "DN_AccountGetLastRFSP", parmArray);
				if(parmArray[1].Value != DBNull.Value)
					acctno = (string)parmArray[1].Value;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return acctno;
		}

		public bool CancelRFAccount(SqlConnection conn, SqlTransaction trans, string accountNo, string customerID)
		{
			bool cancelled = false;
			try
			{				
				parmArray = new SqlParameter[4];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@custid", SqlDbType.NVarChar,20);
				parmArray[1].Value = customerID;
				parmArray[2] = new SqlParameter("@user", SqlDbType.Int);
				parmArray[2].Value = this.User;
				parmArray[3] = new SqlParameter("@cancelled", SqlDbType.SmallInt);
				parmArray[3].Value = 0;
				parmArray[3].Direction = ParameterDirection.Output;

				this.RunSP(conn, trans, "DN_AccountCancelRFSP", parmArray);
				if(parmArray[2].Value != DBNull.Value)
					cancelled = Convert.ToBoolean(parmArray[2].Value);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return cancelled;
		}

		public bool CancelAccount(SqlConnection conn, SqlTransaction trans, string accountNo, 
			string customerID, DateTime dateCancelled, string code, string notes)
		{
			bool cancelled = false;
			try
			{				
				parmArray = new SqlParameter[7];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@custid", SqlDbType.NVarChar,20);
				parmArray[1].Value = customerID;
				parmArray[2] = new SqlParameter("@user", SqlDbType.Int);
				parmArray[2].Value = this.User;
				parmArray[3] = new SqlParameter("@datecancelled", SqlDbType.DateTime);
				parmArray[3].Value = dateCancelled;
				parmArray[4] = new SqlParameter("@code", SqlDbType.NVarChar,4);
				parmArray[4].Value = code;
				parmArray[5] = new SqlParameter("@notes", SqlDbType.NVarChar,300);
				parmArray[5].Value = notes;
				parmArray[6] = new SqlParameter("@cancelled", SqlDbType.SmallInt);
				parmArray[6].Value = 0;
				parmArray[6].Direction = ParameterDirection.Output;

				this.RunSP(conn, trans, "DN_AccountCancelSP", parmArray);
				if(parmArray[6].Value != DBNull.Value)
					cancelled = Convert.ToBoolean(parmArray[6].Value);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return cancelled;
		}

		public bool ConvertRFToHP(SqlConnection conn, SqlTransaction trans, string accountNo, 
			string customerID, string country, DateTime dateProp)
		{
			bool cancelled = false;
			short branch = Convert.ToInt16(accountNo.Substring(0, 3));
			try
			{				
				parmArray = new SqlParameter[5];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@custid", SqlDbType.NVarChar,20);
				parmArray[1].Value = customerID;
				parmArray[2] = new SqlParameter("@country", SqlDbType.NVarChar,2);
				parmArray[2].Value = country;
				parmArray[3] = new SqlParameter("@branch", SqlDbType.SmallInt);
				parmArray[3].Value = branch;
				parmArray[4] = new SqlParameter("@dateprop", SqlDbType.DateTime);
				parmArray[4].Value = dateProp;

				this.RunSP(conn, trans, "DN_AccountConvertRFToHPSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return cancelled;
		}

		public DataTable GetPaymentCardDetails(string customerID, string accountNo, string branchNo)
		{
			DataTable dt = null;
			try
			{				
				dt = new DataTable(TN.PaymentCard);
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar,20);
				parmArray[0].Value = customerID;
				parmArray[1] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[1].Value = accountNo;
				parmArray[2] = new SqlParameter("@branchno", SqlDbType.SmallInt);
				parmArray[2].Value = Convert.ToInt16(branchNo);

				this.RunSP("DN_PaymentCardDetailsGetSP", parmArray, dt);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return dt;
		}

		public string GetPaidAndTakenAccount(SqlConnection conn, SqlTransaction trans, string branchNo)
		{
			string acctno = "";
			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@branch", SqlDbType.NVarChar,3);
				parmArray[0].Value = branchNo;
				parmArray[1] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[1].Value = "";
				parmArray[1].Direction = ParameterDirection.Output;

				if(conn!=null && trans != null)
					RunSP(conn, trans, "DN_AccountGetPaidAndTakenSP", parmArray);
				else
					RunSP("DN_AccountGetPaidAndTakenSP", parmArray);
				if(!Convert.IsDBNull(parmArray[1].Value))
					acctno = (string)parmArray[1].Value;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return acctno;
		}

		public int GetDeliveries(SqlConnection conn, SqlTransaction trans,
			string acctno, int user, int branch)
		{
			_deliveries = new DataTable("Deliveries");
			
			try
			{
				parmArray = new SqlParameter[4];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = acctno;
				parmArray[1] = new SqlParameter("@user", SqlDbType.Int);
				parmArray[1].Value = user;
				parmArray[2] = new SqlParameter("@branch", SqlDbType.Int);
				parmArray[2].Value = branch;
				parmArray[3] = new SqlParameter("@TimeLocked", SqlDbType.DateTime);
				parmArray[3].Value = "";
				parmArray[3].Direction = ParameterDirection.Output;
			
				result = this.RunSP(conn, trans, "DN_DeliveryAcctsLoadSP", parmArray, _deliveries);
				
				if(result==0)
				{
					result = (int)Return.Success;
					if(!Convert.IsDBNull(parmArray[3].Value))
						_dateAccountLocked = (DateTime)parmArray[3].Value;
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

		public int GetDeliveryNotes(SqlConnection conn, SqlTransaction trans,
			string acctno, int user, int branch, string addr1, DateTime dateReqDel, 
			string addtype, string timeReqDel, int locn, ref int buffno)
		{
			_deliveryLineItems = new DataTable(TN.DeliveryLineItems);
			
			try
			{
				parmArray = new SqlParameter[9];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = acctno;
				parmArray[1] = new SqlParameter("@user", SqlDbType.Int);
				parmArray[1].Value = user;
				parmArray[2] = new SqlParameter("@branch", SqlDbType.Int);
				parmArray[2].Value = branch;
                parmArray[3] = new SqlParameter("@addr1", SqlDbType.NVarChar, CW.Address1);
				parmArray[3].Value = addr1;
				parmArray[4] = new SqlParameter("@datereqdel", SqlDbType.DateTime);
				parmArray[4].Value = dateReqDel;
				parmArray[5] = new SqlParameter("@addtype", SqlDbType.NChar, 2);
				parmArray[5].Value = addtype;
				parmArray[6] = new SqlParameter("@timereqdel", SqlDbType.NVarChar, 12);
				parmArray[6].Value = timeReqDel;
				parmArray[7] = new SqlParameter("@locn", SqlDbType.Int);
				parmArray[7].Value = locn;
				parmArray[8] = new SqlParameter("@buffno", SqlDbType.Int);
				parmArray[8].Value = buffno;
				parmArray[8].Direction = ParameterDirection.Output;

                //IP - 10/02/10 - CR1048 (Ref:3.1.3) - UAT(108) Merged - Malaysia Enhancements (CR1072)
                SqlParameter[] tempParmArray = new SqlParameter[1];
                tempParmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar, 12);
                tempParmArray[0].Value = acctno;

                RunSP(conn, trans, "DN_ResetLineItemPrintOrder", tempParmArray); 

				result = this.RunSP(conn, trans, "DN_DeliveryNotesLoadSP", parmArray, _deliveryLineItems);

				if(result==0)
				{
					result = (int)Return.Success;
					if(parmArray[8].Value!=DBNull.Value)
						buffno = (int)parmArray[8].Value;
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

		public int UnlockAccountsLockedAt(int user, DateTime TimeLocked)
		{
			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@user", SqlDbType.Int);
				parmArray[0].Value = user;
				parmArray[1] = new SqlParameter("@TimeLocked", SqlDbType.DateTime);
				parmArray[1].Value = TimeLocked;
			
				result = this.RunSP("DN_UnlockAccountsLockedAtSP", parmArray);
				
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

		public void SaveRepossArrears(SqlConnection conn, SqlTransaction trans)
		{
			try
			{
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar, 12);
				parmArray[0].Value = this.AccountNumber;
				parmArray[1] = new SqlParameter("@reposarrears", SqlDbType.Money);
				parmArray[1].Value = this.RepossArrears;
				parmArray[2] = new SqlParameter("@reposvalue", SqlDbType.Money);
				parmArray[2].Value = this.RepossValue;

				this.RunSP(conn, trans, "DN_AccountSaveReposArrears", parmArray);
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void SaveBalanceStatus(SqlConnection conn, SqlTransaction trans)
		{
			try
			{
				parmArray = new SqlParameter[4];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
				parmArray[0].Value = this.AccountNumber;
				parmArray[1] = new SqlParameter("@status", SqlDbType.NChar, 1);
				parmArray[1].Value = this.CurrentStatus;
				parmArray[2] = new SqlParameter("@outstbal", SqlDbType.Money);
				parmArray[2].Value = this.OutstandingBalance;
				parmArray[3] = new SqlParameter("@user", SqlDbType.Money);
				parmArray[3].Value = this.User;




				this.RunSP(conn, trans, "DN_AccountUpdateBalStatusSP", parmArray);
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void CalcArrears(SqlConnection conn, SqlTransaction trans, decimal CountPcent, int NoDates)
		{
			try
			{
				parmArray = new SqlParameter[5];
				parmArray[0] = new SqlParameter("@AcctNo", SqlDbType.NChar, 12);
				parmArray[0].Value = this.AccountNumber;
				parmArray[1] = new SqlParameter("@CountPcent", SqlDbType.Float);
				parmArray[1].Value = CountPcent;
				parmArray[2] = new SqlParameter("@NoDates", SqlDbType.SmallInt);
				parmArray[2].Value = NoDates;
				parmArray[3] = new SqlParameter("@Arrears", SqlDbType.Money);
				parmArray[3].Value = 0;
				parmArray[3].Direction = ParameterDirection.Output;
                parmArray[4] = new SqlParameter("@datefrom", SqlDbType.DateTime);
                parmArray[4].Value = DateTime.Now;
				this.RunSP(conn, trans, "DBArrearsCalc", parmArray);
				if(!Convert.IsDBNull(parmArray[3].Value))
					this.Arrears = (decimal)parmArray[3].Value;
				else
					this.Arrears = 0;

			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public bool Day90Check( SqlConnection conn, SqlTransaction trans,
			string accountNo, out decimal rebate )
		{
			bool addRebate = false;
			rebate = 0;
			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NChar, 12);
				parmArray[0].Value = this.AccountNumber;
				parmArray[1] = new SqlParameter("@rebate", SqlDbType.Money);
				parmArray[1].Value = 0;
				parmArray[1].Direction = ParameterDirection.Output;

				result = RunSP(conn, trans, "DN_AccountDay90CheckSP", parmArray);
				if(!Convert.IsDBNull(parmArray[1].Value))
					rebate = (decimal)parmArray[1].Value;
				addRebate = result != -1;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw;
			}
			return addRebate;
		}

		public decimal AddToCalculation(SqlConnection conn, SqlTransaction trans, 
			string accountNo, decimal rebate)
		{
			decimal addToValue = 0;
			try
			{
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NChar, 12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@rebate", SqlDbType.Money);
				parmArray[1].Value = rebate;
				parmArray[2] = new SqlParameter("@value", SqlDbType.Money);
				parmArray[2].Value = 0;
				parmArray[2].Direction = ParameterDirection.Output;

				RunSP(conn, trans, "DN_AddToCalcSP", parmArray);
				if(!Convert.IsDBNull(parmArray[2].Value))
					addToValue = (decimal)parmArray[2].Value;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return addToValue;
		}
		public void CalculateRebateForMambu(SqlConnection conn, SqlTransaction trans,
			string accountNo, out decimal rebate)
		{
			rebate = 0;
			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@AcctNo", SqlDbType.NChar, 12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@Rebate", SqlDbType.Money);
				parmArray[1].Value = 0;
				parmArray[1].Direction = ParameterDirection.Output;
				
				if (conn != null && trans != null)
					RunSP(conn, trans, "SP_BS_GetRebateForMambuAccount", parmArray);
				else
					RunSP("SP_BS_GetRebateForMambuAccount", parmArray);

				if (!Convert.IsDBNull(parmArray[1].Value))
					rebate = (decimal)parmArray[1].Value;
			}
			catch (SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			
		}
		public decimal CalculateRebate(SqlConnection conn, SqlTransaction trans, 
			string accountNo)
		{
			decimal rebate = 0;
			try
			{
				parmArray = new SqlParameter[4];
				parmArray[0] = new SqlParameter("@AcctNo", SqlDbType.NChar, 12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@poRebate", SqlDbType.Money);
				parmArray[1].Value = 0;
				parmArray[2] = new SqlParameter("@poRebateWithin12Mths", SqlDbType.Money);
				parmArray[2].Value = 0;
				parmArray[3] = new SqlParameter("@poRebateAfter12Mths", SqlDbType.Money);
				parmArray[3].Value = 0;
				foreach(SqlParameter p in parmArray)
					p.Direction = ParameterDirection.Output;
				parmArray[0].Direction = ParameterDirection.Input;

				if(conn!=null && trans!=null)
					RunSP(conn, trans, "DN_RebateSP", parmArray);
				else
					RunSP("DN_RebateSP", parmArray);
				if(!Convert.IsDBNull(parmArray[1].Value))
					rebate = (decimal)parmArray[1].Value;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return rebate;
		}

        public decimal CalculateBduRebate(SqlConnection conn, SqlTransaction trans,
           string accountNo)
        {
            decimal rebate = 0;
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@AcctNo", SqlDbType.NChar, 12);
                parmArray[0].Value = accountNo;
                parmArray[1] = new SqlParameter("@poRebate", SqlDbType.Money);
                parmArray[1].Value = 0;
                foreach (SqlParameter p in parmArray)
                    p.Direction = ParameterDirection.Output;
                parmArray[0].Direction = ParameterDirection.Input;

                if (conn != null && trans != null)
                    RunSP(conn, trans, "CalculateBduRebate", parmArray);
                else
                    RunSP("CalculateBduRebate", parmArray);
                if (!Convert.IsDBNull(parmArray[1].Value))
                    rebate = (decimal)parmArray[1].Value;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return rebate;
        }

		public bool IsCancelled(SqlConnection conn, SqlTransaction trans, string accountNo)
		{
			int isCancelled = 0;
			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@piAcctNo", SqlDbType.NChar, 12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@poIsCancelled", SqlDbType.Int);
				parmArray[1].Direction = ParameterDirection.Output;

				if(conn!=null && trans!=null)
					RunSP(conn, trans, "DN_AcctIsCancelledSP", parmArray);
				else
					RunSP("DN_AcctIsCancelledSP", parmArray);
				if(!Convert.IsDBNull(parmArray[1].Value))
					isCancelled = (int)parmArray[1].Value;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return (isCancelled > 0);
		}

		/// <summary>
		/// AccountApplicationStatus
		/// </summary>
		/// <param name="acctno">string</param>
		/// <returns>DataSet</returns>
		/// 
		public DataSet ApplicationStatus (string acctno)
		{
			DataSet ds = new DataSet();
		
			try
			{
				parmArray = new SqlParameter[1];
						
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar, 12);
				parmArray[0].Value = acctno;
						 
				this.RunSP("dn_AccountApplicationStatusGet", parmArray, ds);
			
						
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw;
			}
					
			return ds;
		}

		/// <summary>
		/// GetLetterByAcctNo
		/// </summary>
		/// <param name="acctno">string</param>
		/// <returns>DataSet</returns>
		/// 
		public DataSet GetLetterByAcctNo (string acctno, string serverDB, short linkToTallyman)
		{
			DataSet ds = new DataSet();
	
			try
			{
				parmArray = new SqlParameter[3];
				
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
				parmArray[0].Value = acctno;
				parmArray[1] = new SqlParameter("@serverdbname", SqlDbType.NVarChar, 40);
				parmArray[1].Value = serverDB;
				parmArray[2] = new SqlParameter("@linktotallyman", SqlDbType.SmallInt);
				parmArray[2].Value = linkToTallyman;
				 
				this.RunSP("DN_LetterGetByAcctNo", parmArray, ds);
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			
			return ds;
		}

		/// <summary>
		/// DeliveryNotesReprintLoad
		/// </summary>
		/// <param name="acctno">string</param>
		/// <param name="buffbranchno">int</param>
		/// <param name="bufffnofrom">int</param>
		/// <param name="bufffnoto">int</param>
		/// <returns>DataSet</returns>
		/// 
		public DataSet DeliveryNotesReprintLoad (string acctno, int stockLocn, int bufffnofrom, int bufffnoto)
		{
			DataSet ds = new DataSet();
			
			try
			{
				parmArray = new SqlParameter[4];
				
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar, 12);
				parmArray[0].Value = acctno;
				
				parmArray[1] = new SqlParameter("@stockLocn", SqlDbType.Int);
				parmArray[1].Value = stockLocn;
				
				parmArray[2] = new SqlParameter("@bufffnofrom", SqlDbType.Int);
				parmArray[2].Value = bufffnofrom;
				
				parmArray[3] = new SqlParameter("@bufffnoto", SqlDbType.Int);
				parmArray[3].Value = bufffnoto;
				 
				this.RunSP("DN_DeliveyAcctsReprintLoad", parmArray, ds);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			
			return ds;
		}

        //public int GetReprintDetails(SqlConnection conn, SqlTransaction trans, string acctno, int stockLocn, int buffbranchno, int buffno)  //CR1072 Malaysia merge -LW71408
        public int GetReprintDetails(SqlConnection conn, SqlTransaction trans, string acctno, int stockLocn, int buffno)  //CR1072 Malaysia merge -LW71408 --IP - 22/02/10 - Undone 71408 - reinstate later
		{
			_deliveryLineItems = new DataTable(TN.Schedules);
			
			try
			{
                //parmArray = new SqlParameter[4];    //CR1072 Malaysia merge -LW71408
                parmArray = new SqlParameter[3];    //CR1072 Malaysia merge -LW71408 --IP - 22/02/10 - Undone 71408 - reinstate later
				
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar, 12);
				parmArray[0].Value = acctno;
				
				parmArray[1] = new SqlParameter("@stocklocn", SqlDbType.Int);
				parmArray[1].Value = stockLocn;
				
				parmArray[2] = new SqlParameter("@buffno", SqlDbType.Int);
				parmArray[2].Value = buffno;
                //CR1072 Malaysia merge -LW71408 --IP - 22/02/10 - Undone 71408 - reinstate later
                //parmArray[3] = new SqlParameter("@buffbranchno", SqlDbType.Int);
                //parmArray[3].Value = buffbranchno;

                //IP - 09/02/10 - CR1048 (Ref:3.1.3) Merged - Malaysia Enhancements (CR1072)
                SqlParameter[] tempParmArray = new SqlParameter[1];
                tempParmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar, 12);
                tempParmArray[0].Value = acctno;

                if (conn != null && trans != null)
                {
                    RunSP(conn, trans, "DN_ResetLineItemPrintOrder", tempParmArray); //IP - 09/02/10 - CR1048 (Ref:3.1.3) Merged - Malaysia Enhancements (CR1072)
                    result = this.RunSP(conn, trans, "DN_DeliveryNotesReprintLoad", parmArray, _deliveryLineItems);
                }
                else
                {
                    RunSP("DN_ResetLineItemPrintOrder", tempParmArray); //IP - 09/02/10 - CR1048 (Ref:3.1.3) Merged - Malaysia Enhancements (CR1072)
                    result = this.RunSP("DN_DeliveryNotesReprintLoad", parmArray, _deliveryLineItems);
                }
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			
			return result;
		}

		public void ReverseCancellation(SqlConnection conn, SqlTransaction trans, 
			string accountNo, DateTime dateReversed, string code, string notes)
		{
			try
			{
				parmArray = new SqlParameter[5];
				parmArray[0] = new SqlParameter("@accountNo", SqlDbType.NVarChar, 12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@datereversed", SqlDbType.DateTime);
				parmArray[1].Value = dateReversed;
				parmArray[2] = new SqlParameter("@user", SqlDbType.Int);
				parmArray[2].Value = this.User;
				parmArray[3] = new SqlParameter("@code", SqlDbType.NVarChar, 4);
				parmArray[3].Value = code;
				parmArray[4] = new SqlParameter("@notes", SqlDbType.NVarChar, 300);
				parmArray[4].Value = notes;
				
				this.RunSP(conn, trans, "DN_AccountRemoveCancellationSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public bool IsTaxExempt(SqlConnection conn, SqlTransaction trans, string accountNo,string reference)
		{
			bool taxExempt = false;

            try
			{
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar,12);
				parmArray[0].Value = accountNo;
                parmArray[1] = new SqlParameter("@reference", SqlDbType.VarChar, 10);
                parmArray[1].Value = reference;
				parmArray[2] = new SqlParameter("@taxExempt", SqlDbType.SmallInt);
				parmArray[2].Value = 0;
				parmArray[2].Direction = ParameterDirection.Output;

				if(conn!=null && trans!=null)
					RunSP(conn, trans, "DN_AcctCodeIsTaxExemptSP", parmArray);
				else
					RunSP("DN_AcctCodeIsTaxExemptSP", parmArray);
			
				if(parmArray[2].Value!=DBNull.Value)
					taxExempt = Convert.ToBoolean(parmArray[2].Value);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return taxExempt;
		}

		public string GetSundryCreditAccount(SqlConnection conn, SqlTransaction trans, short branchNo)
		{
			string acctno = "";
			try
			{				
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@branchno", SqlDbType.SmallInt);
				parmArray[0].Value = branchNo;
				parmArray[1] = new SqlParameter("@acctno", SqlDbType.VarChar,12);
				parmArray[1].Value = "";
				parmArray[1].Direction = ParameterDirection.Output;

				if(conn!=null && trans!=null)
					RunSP(conn, trans, "DN_AccountGetSundryCreditSP", parmArray);
				else
					RunSP("DN_AccountGetSundryCreditSP", parmArray);

				if(parmArray[1].Value != DBNull.Value)
					acctno = (string)parmArray[1].Value;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return acctno;
		}

		public decimal GetCashierLastDifference(SqlConnection conn, SqlTransaction trans, int empeeno)
		{
			decimal lastDiff = 0;
			try
			{				
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@piEmpeeNo", SqlDbType.Int);
				parmArray[0].Value = empeeno;
				parmArray[1] = new SqlParameter("@poLastDifference", SqlDbType.Money);
				parmArray[1].Value = 0;
				parmArray[1].Direction = ParameterDirection.Output;

				if(conn!=null && trans!=null)
					this.RunSP(conn, trans, "DN_AccountGetLastCashierDifferenceSP", parmArray);
				else
					this.RunSP("DN_AccountGetLastCashierDifferenceSP", parmArray);
				if(parmArray[1].Value != DBNull.Value)
					lastDiff = (decimal)parmArray[1].Value;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw;
			}
			return lastDiff;
		}

		public string GetOveragesAccount(SqlConnection conn, SqlTransaction trans, short branchNo)
		{
			string acctno = "";
			try
			{				
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@branchno", SqlDbType.SmallInt);
				parmArray[0].Value = branchNo;
				parmArray[1] = new SqlParameter("@acctno", SqlDbType.VarChar,12);
				parmArray[1].Value = "";
				parmArray[1].Direction = ParameterDirection.Output;

				if(conn!=null && trans!=null)
					this.RunSP(conn, trans, "DN_AccountGetOveragesSP", parmArray);
				else
					this.RunSP("DN_AccountGetOveragesSP", parmArray);
				if(parmArray[1].Value != DBNull.Value)
					acctno = (string)parmArray[1].Value;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return acctno;
		}

		public string GetReceivableAccount(SqlConnection conn, SqlTransaction trans, int empeeno)
		{
			string acctno = "";
			try
			{				
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[0].Value = empeeno;
				parmArray[1] = new SqlParameter("@acctno", SqlDbType.VarChar,12);
				parmArray[1].Value = "";
				parmArray[1].Direction = ParameterDirection.Output;

				if(conn!=null && trans!=null)
					this.RunSP(conn, trans, "DN_AccountGetReceivableSP", parmArray);
				else
					this.RunSP("DN_AccountGetReceivableSP", parmArray);

				if(parmArray[1].Value != DBNull.Value)
					acctno = (string)parmArray[1].Value;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw;
			}
			return acctno;
		}

        public string GetApplicantTwoName(string customerID, string accountNo,
                                          out string jointid, out string relationship)
        {
			string name = "";
			jointid = "";
            relationship = "";

			try
			{
                parmArray = new SqlParameter[5];
				parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
				parmArray[0].Value = customerID;
				parmArray[1] = new SqlParameter("@acctno", SqlDbType.NVarChar,20);
				parmArray[1].Value = accountNo;
				parmArray[2] = new SqlParameter("@name", SqlDbType.NVarChar,50);
				parmArray[2].Value = "";
				parmArray[2].Direction = ParameterDirection.Output;
				parmArray[3] = new SqlParameter("@jointid", SqlDbType.NVarChar,20);
				parmArray[3].Value = "";
				parmArray[3].Direction = ParameterDirection.Output;
                parmArray[4] = new SqlParameter("@relationship", SqlDbType.NChar, 2);
                parmArray[4].Value = "";
                parmArray[4].Direction = ParameterDirection.Output;

				RunSP("DN_ProposalGetSecondApplicantNameSP", parmArray);
				if(parmArray[2].Value != DBNull.Value)
					name = (string)parmArray[2].Value;
				if(parmArray[3].Value != DBNull.Value)
					jointid = (string)parmArray[3].Value;
                if(parmArray[4].Value != DBNull.Value)
                    relationship = (string)parmArray[4].Value;
            }
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return name;
		}

		/// <summary>
		/// SelectType
		/// </summary>
		/// <param name="accttype">string</param>
		/// <param name="countryCode">string</param>
		/// <param name="mthsintfree">int</param>
		/// <param name="mthsdeferred">int</param>
		/// <returns>void</returns>
		/// 
		public void SelectType (SqlConnection conn, SqlTransaction trans, string accttype, string countryCode, out int mthsintfree, out int mthsdeferred)
		{
			mthsintfree = 0;
			mthsdeferred = 0;
			
			try
			{
				parmArray = new SqlParameter[4];
				
				parmArray[0] = new SqlParameter("@accttype", SqlDbType.NVarChar, 2);
				parmArray[0].Value = accttype;
				
				parmArray[1] = new SqlParameter("@countryCode", SqlDbType.NVarChar, 4);
				parmArray[1].Value = countryCode;
				
				parmArray[2] = new SqlParameter("@mthsintfree", SqlDbType.SmallInt);
				parmArray[2].Value = mthsintfree;
				parmArray[2].Direction = ParameterDirection.Output;
				
				parmArray[3] = new SqlParameter("@mthsdeferred", SqlDbType.SmallInt);
				parmArray[3].Value = mthsdeferred;
				parmArray[3].Direction = ParameterDirection.Output; 
				
				if(conn!=null && trans!=null)
					this.RunSP(conn, trans, "DN_AccountTypeSelectSP", parmArray);
				else
					this.RunSP("DN_AccountTypeSelectSP", parmArray);
	
				if(parmArray[2].Value!=DBNull.Value)
					mthsintfree = Convert.ToInt32(parmArray[2].Value);
				if(parmArray[3].Value!=DBNull.Value)
					mthsdeferred = Convert.ToInt32(parmArray[3].Value);
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		/// <summary>
		/// GetRFAgreementTotal
		/// </summary>
		/// <param name="custid">string</param>
		/// <param name="agreementTotal">double</param>
		/// <returns>void</returns>
		/// 
		public decimal GetRFAgreementTotal (string custid)
		{
			decimal agreementTotal = 0;
			
			try
			{
				parmArray = new SqlParameter[2];
				
				parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, 40);
				parmArray[0].Value = custid;
				
				parmArray[1] = new SqlParameter("@agreementTotal", SqlDbType.Money);
				parmArray[1].Value = agreementTotal;
				parmArray[1].Direction = ParameterDirection.Output; 
				
				this.RunSP("DN_AccountGetRFAgreementTotalSP", parmArray);
	
				if(parmArray[1].Value!=DBNull.Value)
					agreementTotal = (decimal)parmArray[1].Value;
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return agreementTotal;
		}

		public int FreeInstalmentAvailable (SqlConnection conn, SqlTransaction trans, string acctNo)
		{
			int result = 0;
			int freeInstal = 0;
			
			try
			{
				parmArray = new SqlParameter[2];
				
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
				parmArray[0].Value = acctNo;
							
				parmArray[1] = new SqlParameter("@freestatus", SqlDbType.Int);
				parmArray[1].Direction = ParameterDirection.Output; 

				result = this.RunSP(conn, trans, "DN_FreeInstalmentAvailableSP", parmArray);
	
				if (result == 0 && parmArray[1].Value != DBNull.Value)
				{
					freeInstal = (int)parmArray[1].Value;
				}
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return freeInstal;
		}

		public int PrivilegeClubVoucher(SqlConnection conn, SqlTransaction trans, string acctNo)
		{
			int voucherAmount = 0;
			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@acctNo", SqlDbType.VarChar,CW.AccountNo);
				parmArray[0].Value = acctNo;
				parmArray[1] = new SqlParameter("@voucherAmount", SqlDbType.Money);
				parmArray[1].Value = 0;
				parmArray[1].Direction = ParameterDirection.Output;

				this.RunSP(conn, trans, "DN_PrivilegeClubVoucherSP", parmArray);

				if (parmArray[1].Value != DBNull.Value)
					voucherAmount = Convert.ToInt32(parmArray[1].Value);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return voucherAmount;
		}

		public int CheckAccountToCancel(string accountNumber)
		{
			try
			{
				_accountdetails = new DataTable(TN.AccountDetails);			
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar,50);
				parmArray[0].Value = accountNumber;

				result = this.RunSP("DN_AccountCancelCheckSP", parmArray, _accountdetails);
			
				if(result==0)
				{
					result = (int)Return.Success;
				}
				else
				{
					result = (int)Return.Fail;
				}
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

		/// <summary>
		/// GetChargeableCashPrice
		/// </summary>
		/// <param name="acctno">string</param>
		/// <param name="price">double</param>
		/// <returns>decimal</returns>
		/// 
		public decimal GetChargeableCashPrice (SqlConnection conn, SqlTransaction trans, 
			string acctno, ref decimal chargeableAdminPrice)
		{
			decimal price = 0;
			
			try
			{
				parmArray = new SqlParameter[3];
				
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 24);
				parmArray[0].Value = acctno;
				
				parmArray[1] = new SqlParameter("@price", SqlDbType.Money);
				parmArray[1].Value = price;
				parmArray[1].Direction = ParameterDirection.Output; 

				parmArray[2] = new SqlParameter("@adminprice", SqlDbType.Money);
				parmArray[2].Value = chargeableAdminPrice;
				parmArray[2].Direction = ParameterDirection.Output; 
				
				if(conn!=null && trans!=null)
					this.RunSP(conn, trans, "DN_AccountGetChargeableCashPriceSP", parmArray);
				else
					this.RunSP("DN_AccountGetChargeableCashPriceSP", parmArray);
	
				if(parmArray[1].Value!=DBNull.Value)
					price = (decimal)parmArray[1].Value;

				if(parmArray[2].Value!=DBNull.Value)
					chargeableAdminPrice = (decimal)parmArray[2].Value;
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return price;
		}

		/// <summary>
		/// GetBadDebtWriteOffAccount
		/// </summary>
		/// <param name="branchno">int</param>
		/// <param name="acctno">string</param>
		/// <returns>string</returns>
		/// 
		public string GetBadDebtWriteOffAccount (SqlConnection conn, SqlTransaction trans, string securitised, short branchno)
		{			
			string acctno = "";

			try
			{
				parmArray = new SqlParameter[3];
				
				parmArray[0] = new SqlParameter("@branchno", SqlDbType.SmallInt);
				parmArray[0].Value = branchno;
				parmArray[1] = new SqlParameter("@securitised", SqlDbType.NVarChar, 2);
				parmArray[1].Value = securitised;

				parmArray[2] = new SqlParameter("@acctno", SqlDbType.NVarChar, 24);
				parmArray[2].Value = acctno;
				parmArray[2].Direction = ParameterDirection.Output; 
				

				if(conn!=null && trans!=null)
					this.RunSP(conn, trans, "DN_AccountGetBadDebtWriteOffSP", parmArray);
				else
					this.RunSP("DN_AccountGetBadDebtWriteOffSP", parmArray);
	
				if(parmArray[2].Value!=DBNull.Value)
					acctno = (string)parmArray[2].Value;
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return acctno;
		}

		public void ResetAgrmnTotal(SqlConnection conn, SqlTransaction trans, string accountNo)
		{
			try
			{
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NChar, 12);
				parmArray[0].Value = accountNo;
				this.RunSP(conn, trans, "DN_ResetAgrmnTotal", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public bool IsRepossessed(string accountNo)
		{
			bool repo = false;
			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@repo", SqlDbType.SmallInt);
				parmArray[1].Value = 0;
				parmArray[1].Direction = ParameterDirection.Output;

				RunSP("DN_AcctCodeIsRepossessedSP", parmArray);
			
				if(parmArray[1].Value!=DBNull.Value)
					repo = Convert.ToBoolean(parmArray[1].Value);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return repo;
		}

		public void GetSegments(string acctNo, string serverdb)
		{
			_segments = new DataTable(TN.Segments);

			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
				parmArray[0].Value = acctNo;
				parmArray[1] = new SqlParameter("@serverdbname", SqlDbType.NVarChar, 50);
				parmArray[1].Value = serverdb;
				
				this.RunSP("DN_AccountGetSegments", parmArray, _segments);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}
		
		public void GetActivities(string acctNo, string serverdb, short type)
		{
			string tableName = "";

			if(Convert.ToBoolean(type))
				tableName = TN.Activities;
			else
				tableName = TN.Arrangements;

			_activities = new DataTable(tableName);

			try
			{
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
				parmArray[0].Value = acctNo;
				parmArray[1] = new SqlParameter("@serverdbname", SqlDbType.NVarChar, 50);
				parmArray[1].Value = serverdb;
				parmArray[2] = new SqlParameter("@isactivities", SqlDbType.SmallInt);
				parmArray[2].Value = type;
				
				this.RunSP("DN_AccountGetActivities", parmArray, _activities);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public bool IsCancelled(string accountNo)
		{
			bool cancelled = false;
			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@cancelled", SqlDbType.SmallInt);
				parmArray[1].Value = 0;
				parmArray[1].Direction = ParameterDirection.Output;

				RunSP("DN_AccountIsCancelledSP", parmArray);
			
				if(parmArray[1].Value!=DBNull.Value)
					cancelled = Convert.ToBoolean(parmArray[1].Value);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return cancelled;
		}

		public void SaveManualCDV(SqlConnection conn, SqlTransaction trans, string accountNo)
		{
			try
			{
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@piAcctNo", SqlDbType.NVarChar, 12);
				parmArray[0].Value = accountNo;

				this.RunSP(conn, trans, "DN_SaveManualCDVSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public bool ManualCDVExists(string accountNo)
		{
			try
			{
				bool exists = false;

				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@piAcctNo", SqlDbType.NVarChar, 12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@poExists", SqlDbType.SmallInt);
				parmArray[1].Value = 0;
				parmArray[1].Direction = ParameterDirection.Output;

				this.RunSP("DN_ManualCDVExistsSP", parmArray);

				if(parmArray[1].Value != DBNull.Value)
					exists = Convert.ToBoolean(parmArray[1].Value);

				return exists;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void GetPeriodEndDates(out string nextPeriodEnd)
		{
			try
			{
				nextPeriodEnd = "";

				_endDates = new DataTable();      
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@nextperiodend", SqlDbType.NVarChar, 12);
				parmArray[0].Value = "";
				parmArray[0].Direction = ParameterDirection.Output;

				this.RunSP("DN_GetPeriodEndDatesSP", parmArray, _endDates);

				if(parmArray[0].Value != DBNull.Value)
					nextPeriodEnd = (string)(parmArray[0].Value);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

        public void GetRebateForecastReportA(string periodEnd, int branchNo)
		{
			try
			{
				_rebateReport = new DataTable(TN.ReportA);

                if (branchNo < 0)
                {
                    parmArray = new SqlParameter[1];
                    parmArray[0] = new SqlParameter("@periodend", SqlDbType.NVarChar, 12);
                    parmArray[0].Value = periodEnd;

                    this.RunSP("DN_GetRebateForecastReportASP", parmArray, _rebateReport);
                }
                // CR931 Forecast by Branch  jec 04/04/08
                else
                {
                    parmArray = new SqlParameter[2];
                    parmArray[0] = new SqlParameter("@periodend", SqlDbType.NVarChar, 12);
                    parmArray[0].Value = periodEnd;
                    parmArray[1] = new SqlParameter("@branchno", SqlDbType.Int);
                    parmArray[1].Value = branchNo;

                    this.RunSP("DN_GetRebateForecastReportA_BrnSP", parmArray, _rebateReport);
                }
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

        public void GetRebateForecastReportB(string periodEnd, int branchNo)
        {
            try
            {
                _rebateReport = new DataTable(TN.ReportB);


                if (branchNo < 0)
                {
                    parmArray = new SqlParameter[1];
                    parmArray[0] = new SqlParameter("@periodend", SqlDbType.NVarChar, 12);
                    parmArray[0].Value = periodEnd;

                    this.RunSP("DN_GetRebateForecastReportBSP", parmArray, _rebateReport);
                }
                // CR931 Forecast by Branch  jec 04/04/08
                else
                {
                    parmArray = new SqlParameter[2];
                    parmArray[0] = new SqlParameter("@periodend", SqlDbType.NVarChar, 12);
                    parmArray[0].Value = periodEnd;
                    parmArray[1] = new SqlParameter("@branchno", SqlDbType.Int);
                    parmArray[1].Value = branchNo;

                    this.RunSP("DN_GetRebateForecastReportB_BrnSP", parmArray, _rebateReport);

                }
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void GetRebateForecastReportC(string periodEnd, int branchNo)
        {
            try
            {
                _rebateReport = new DataTable(TN.ReportC);


                if (branchNo < 0)
                {
                    parmArray = new SqlParameter[0];
                    this.RunSP("DN_GetRebateForecastReportCSP", parmArray, _rebateReport);
                }
                // CR931 Forecast by Branch  jec 04/04/08
                else
                {
                    parmArray = new SqlParameter[1];
                    parmArray[0] = new SqlParameter("@branchno", SqlDbType.Int);
                    parmArray[0].Value = branchNo;
                    this.RunSP("DN_GetRebateForecastReportC_BrnSP", parmArray, _rebateReport);
                }
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void GetRebateForecastReportD(string periodEnd, int branchNo)
        {
            try
            {
                _rebateReport = new DataTable(TN.ReportD);


                if (branchNo < 0)
                {
                    parmArray = new SqlParameter[0];
                    this.RunSP("DN_GetRebateForecastReportDSP", parmArray, _rebateReport);
                }
                // CR931 Forecast by Branch  jec 04/04/08
                else
                {
                    parmArray = new SqlParameter[1];
                    parmArray[0] = new SqlParameter("@branchno", SqlDbType.Int);
                    parmArray[0].Value = branchNo;
                    this.RunSP("DN_GetRebateForecastReportD_BrnSP", parmArray, _rebateReport);
                }
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

		public int GetAllPeriodEndDates()
		{
			try
			{
				_endDates = new DataTable(TN.EndPeriods);      
				parmArray = new SqlParameter[0];
				result = this.RunSP("DN_GetAllPeriodEndDatesSP", parmArray, _endDates);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

		public void RunRebateForecastReports(SqlConnection conn, SqlTransaction trans, string periodEnd)
		{
			try
			{
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@PeriodEndchar", SqlDbType.NVarChar, 12);
				parmArray[0].Value = periodEnd;
				RunSP(conn, trans, "DN_RebateForecastSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public string IsPaidAndTakenWarranty()
		{
			string termsType = "";
			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar,12);
				parmArray[0].Value = this.AccountNumber;
				parmArray[1] = new SqlParameter("@termstype", SqlDbType.NVarChar,2);
				parmArray[1].Value = "";
				parmArray[1].Direction = ParameterDirection.Output;

				RunSP("DN_AccountGetTermsTypeSP", parmArray);
				
				if(!Convert.IsDBNull(parmArray[1].Value))
					termsType = (string)parmArray[1].Value;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return termsType;
		}

		public DataTable GetBookings(SqlConnection conn, SqlTransaction trans,
			string branchNo,
			string empeeNo,
			DateTime fromDate,
			DateTime toDate,
			int includeCash,
			int includeHP,
			int includeNonSec,
			int includePaidTaken,
			int includeRf,
			int includeSec,
			int rollUpResults)
		{
			DataTable bookingsTable = new DataTable(TN.MonitorBookings);
			try
			{
				parmArray = new SqlParameter[11];
				parmArray[0] = new SqlParameter("@branchNo", SqlDbType.NVarChar);
				parmArray[0].Value = branchNo;
				parmArray[1] = new SqlParameter("@salespersonNo", SqlDbType.NVarChar);
				parmArray[1].Value = empeeNo;
				parmArray[2] = new SqlParameter("@datefrom", SqlDbType.DateTime);
				parmArray[2].Value = fromDate;
				parmArray[3] = new SqlParameter("@dateto", SqlDbType.DateTime);
				parmArray[3].Value = toDate;
				parmArray[4] = new SqlParameter("@includeCash", SqlDbType.Int);
				parmArray[4].Value = includeCash;
				parmArray[5] = new SqlParameter("@includeHP", SqlDbType.Int);
				parmArray[5].Value = includeHP;
				parmArray[6] = new SqlParameter("@includeNonSec", SqlDbType.Int);
				parmArray[6].Value = includeNonSec;
				parmArray[7] = new SqlParameter("@includePaidTaken", SqlDbType.Int);
				parmArray[7].Value = includePaidTaken;
				parmArray[8] = new SqlParameter("@includeRf", SqlDbType.Int);
				parmArray[8].Value = includeRf;
				parmArray[9] = new SqlParameter("@includeSec", SqlDbType.Int);
				parmArray[9].Value = includeSec;
				parmArray[10] = new SqlParameter("@rollUpResults", SqlDbType.Int);
				parmArray[10].Value = rollUpResults;

				RunSP(conn, trans, "DN_GetBookingsSP", parmArray, bookingsTable);				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return bookingsTable;
		}


		public DataSet GetDeliveries(int bufferNo,
			int warehouseNo,
			DateTime fromDate,
			DateTime toDate,
			int includeSec,
			int includeNonSec,
			string operand)
		{
			DataSet deliveriesSet = new DataSet();
			try
			{
				parmArray = new SqlParameter[7];
				parmArray[0] = new SqlParameter("@bufferNo", SqlDbType.Int);
				parmArray[0].Value = bufferNo;
				parmArray[1] = new SqlParameter("@warehouseNo", SqlDbType.Int);
				parmArray[1].Value = warehouseNo;
				parmArray[2] = new SqlParameter("@datefrom", SqlDbType.DateTime);
				parmArray[2].Value = fromDate;
				parmArray[3] = new SqlParameter("@dateto", SqlDbType.DateTime);
				parmArray[3].Value = toDate;
				parmArray[4] = new SqlParameter("@includesecuritised", SqlDbType.Int);
				parmArray[4].Value = includeSec;
				parmArray[5] = new SqlParameter("@includenonsecuritised", SqlDbType.Int);
				parmArray[5].Value = includeNonSec;    
				parmArray[6] = new SqlParameter("@operand", SqlDbType.NVarChar,1);
				parmArray[6].Value = operand;
				RunSP("DN_GetOutstandingDeliveriesSP", parmArray, deliveriesSet);				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return deliveriesSet;
		}

		public void GetWarrantyRenewals(string acctNo, bool settled, bool ismenu, ref string custID)
		{
			_accountslist = new DataTable(TN.Accounts);

			try
			{
				parmArray = new SqlParameter[4];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
				parmArray[0].Value = acctNo;
				parmArray[1] = new SqlParameter("@iscurrentsettled", SqlDbType.Bit);
				parmArray[1].Value = settled;
				parmArray[2] = new SqlParameter("@ismenucall", SqlDbType.Bit);
				parmArray[2].Value = ismenu;
				parmArray[3] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
				parmArray[3].Value = custID;

				parmArray[3].Direction = ParameterDirection.Output;

				this.RunSP("DN_AccountGetForRenewalSP", parmArray, _accountslist);

				if(parmArray[3].Value!=DBNull.Value)
					custID = (string)parmArray[3].Value;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

        public void GetWarrantyProductsByAccount(string acctNo)
        {
            // CR 822 created to list products by account
            _warrantyProductList = new DataTable(TN.WarrantyList);
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
				parmArray[0].Value = acctNo;

                this.RunSP("DN_GetWarrantyProductsByAccountSP", parmArray, _warrantyProductList);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void GetWarrantyCollectionReasonsByAccount(string acctNo)
        {
                // CR 822 created to list products by account
            this._warrantyClaimCollectionResasons = new DataTable(TN.WarrantyCollectionReason);
            try
            {
                parmArray       = new SqlParameter[1];
                parmArray[0]    = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[0].Value = acctNo;

                this.RunSP("DN_CollectionReasonsGetSP", parmArray, _warrantyClaimCollectionResasons);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        

		public void SaveWarrantyRenewal(SqlConnection conn, SqlTransaction trans, 
			string acctNo, string origAcctNo, string contractNo, int warrantyId,
			short location, string origContractNo, short origLocation)
		{
			try
			{
				parmArray = new SqlParameter[8];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
				parmArray[0].Value = acctNo;
				parmArray[1] = new SqlParameter("@origacctno", SqlDbType.NVarChar, 12);
				parmArray[1].Value = origAcctNo;
				parmArray[2] = new SqlParameter("@contractno", SqlDbType.NVarChar, 10);
				parmArray[2].Value = contractNo;
                parmArray[3] = new SqlParameter("@warrantyId", SqlDbType.Int);
                parmArray[3].Value = warrantyId;
				parmArray[4] = new SqlParameter("@location", SqlDbType.SmallInt);
				parmArray[4].Value = location;
				parmArray[5] = new SqlParameter("@origcontractno", SqlDbType.NVarChar, 10);
				parmArray[5].Value = origContractNo;
				parmArray[6] = new SqlParameter("@origlocation", SqlDbType.SmallInt);
				parmArray[6].Value = origLocation;
				parmArray[7] = new SqlParameter("@user", SqlDbType.Int);
				parmArray[7].Value = this.User;

				this.RunSP(conn, trans, "DN_SaveWarrantyRenewalSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}
		public void AddWarrantRenewalCode(SqlConnection conn, SqlTransaction trans, 
			                                string acctNo, short empno, string contractNo)
		{
			try
			{
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
				parmArray[0].Value = acctNo;
				parmArray[1] = new SqlParameter("@empno", SqlDbType.SmallInt);
				parmArray[1].Value = empno;
                parmArray[2] = new SqlParameter("@contractno", SqlDbType.NVarChar, 10);
                parmArray[2].Value = contractNo;

				this.RunSP(conn, trans, "DN_AddWarrantRenewalCodeSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

        public string AccountGetAccountType(string acct)
        {
            parmArray = new SqlParameter[1];
            parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
            parmArray[0].Value = acct;

            return ReturnString("AccountGetAccountType", parmArray);
        }

		public bool IsWarrantyRenewal(string accountNo)
		{
			bool renewal = false;
			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@renewal", SqlDbType.SmallInt);
				parmArray[1].Value = 0;
				parmArray[1].Direction = ParameterDirection.Output;

				RunSP("DN_AccountIsWarrantyRenewalSP", parmArray);
			
				if(parmArray[1].Value!=DBNull.Value)
					renewal = Convert.ToBoolean(parmArray[1].Value);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return renewal;
		}
		public string AccountGetAccountNoByBuffNo(int stockLocn, int buffNo)
		{
			string  accountNo  = "";
			try
			{
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@stockLocn", SqlDbType.Int);
				parmArray[0].Value = stockLocn;
				parmArray[1] = new SqlParameter("@buffNo", SqlDbType.Int);
				parmArray[1].Value = buffNo;
				parmArray[2] = new SqlParameter("@acctNo", SqlDbType.NVarChar,12);
				parmArray[2].Value = accountNo;
				parmArray[2].Direction = ParameterDirection.Output;

				RunSP("DN_AccountGetAccountNoByBuffNoSP", parmArray);
			
				if(parmArray[2].Value!=DBNull.Value)
					accountNo = (parmArray[2].Value.ToString());
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return accountNo;
		}

		public bool IsRejected(SqlConnection conn, SqlTransaction trans, string accountNo)
		{
			int isRejected = 0;
			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NChar, 12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@isrejected", SqlDbType.Int);
				parmArray[1].Direction = ParameterDirection.Output;

				RunSP(conn, trans, "DN_AcctIsRejectedSP", parmArray);

				if(!Convert.IsDBNull(parmArray[1].Value))
					isRejected = (int)parmArray[1].Value;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return (isRejected > 0);
		}

		public void AuditReprint (SqlConnection conn, SqlTransaction trans,
			string accountNo, int agreementNo, string docType, int printedBy)
		{
			try
			{
				parmArray = new SqlParameter[4];
				parmArray[0] = new SqlParameter("@accountno", SqlDbType.Char,12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@agrmtno", SqlDbType.Int);
				parmArray[1].Value = agreementNo;
				parmArray[2] = new SqlParameter("@doctype", SqlDbType.Char,1);
				parmArray[2].Value = docType;
				parmArray[3] = new SqlParameter("@printedby", SqlDbType.Int);
				parmArray[3].Value = printedBy;

				RunSP(conn, trans, "DN_AuditReprintSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void AuditDiscount(SqlConnection conn, SqlTransaction trans, string accountNo, int agreementNo,
			string discountItemNo, string parentItemNo, short stockLocn, decimal amount, int salesPerson, int authorisedBy)
		{
			try
			{
				parmArray = new SqlParameter[8];
				parmArray[0] = new SqlParameter("@AcctNo", SqlDbType.Char,12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@AgrmtNo", SqlDbType.Int);
				parmArray[1].Value = agreementNo;
				parmArray[2] = new SqlParameter("@DiscountItemNo", SqlDbType.VarChar,8);
				parmArray[2].Value = discountItemNo;
				parmArray[3] = new SqlParameter("@ParentItemNo", SqlDbType.VarChar,8);
				parmArray[3].Value = parentItemNo;
				parmArray[4] = new SqlParameter("@StockLocn", SqlDbType.SmallInt);
				parmArray[4].Value = stockLocn;
				parmArray[5] = new SqlParameter("@Amount", SqlDbType.Money);
				parmArray[5].Value = amount;
				parmArray[6] = new SqlParameter("@SalesPerson", SqlDbType.Int);
				parmArray[6].Value = salesPerson;
				parmArray[7] = new SqlParameter("@AuthorisedBy", SqlDbType.Int);
				parmArray[7].Value = authorisedBy;

				RunSP(conn, trans, "DN_AuditDiscountSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void SecuritiseAccounts (SqlConnection conn, SqlTransaction trans,
			int employeeNo, int runNo, out decimal totalBalance, out int totalCount)
		{
			totalBalance = 0;
			totalCount = 0;
			try
			{
				parmArray = new SqlParameter[4];
				parmArray[0] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[0].Value = employeeNo;
				parmArray[1] = new SqlParameter("@runno", SqlDbType.Int);
				parmArray[1].Value = runNo;
				parmArray[2] = new SqlParameter("@totalBalance", SqlDbType.Money);
				parmArray[2].Value = 0;
				parmArray[2].Direction = ParameterDirection.Output;
				parmArray[3] = new SqlParameter("@totalCount", SqlDbType.Int);
				parmArray[3].Value = 0;
				parmArray[3].Direction = ParameterDirection.Output;

				this.RunSP(conn, trans, "SecuritiseAccounts", parmArray);

				if (!Convert.IsDBNull(parmArray[2].Value))
					totalBalance = Convert.ToDecimal(parmArray[2].Value);

				if (!Convert.IsDBNull(parmArray[3].Value))
					totalCount = Convert.ToInt32(parmArray[3].Value);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void SecuritiseAccountsReport14 (SqlConnection conn, SqlTransaction trans, int runNo)
		{
			try
			{
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@runno", SqlDbType.Int);
				parmArray[0].Value = runNo;

				this.RunSP(conn, trans, "Report14SecuritiseAccounts", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void SecuritiseAccountsReport15 (SqlConnection conn, SqlTransaction trans)
		{
			try
			{
				this.RunSP(conn, trans, "Report15NonSecuritiseAccounts");
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

        public void FincoBalances(SqlConnection conn, SqlTransaction trans,DateTime datefrom, DateTime dateto)
        {

            try
            {
                _accountslist = new DataTable(TN.Accounts);

                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@datefrom", SqlDbType.DateTime);
                parmArray[0].Value = datefrom;
                parmArray[1] = new SqlParameter("@dateto", SqlDbType.DateTime);
                parmArray[1].Value = dateto;

                this.RunSP(conn, trans, "dn_fincoedbalances",parmArray,_accountslist);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }


        public void FincoTransactions(SqlConnection conn, SqlTransaction trans, DateTime datefrom, DateTime dateto,string transtypeset)
        {

            try
            {
                _accountslist = new DataTable(TN.Accounts);

                parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@datefrom", SqlDbType.DateTime);
                parmArray[0].Value = datefrom;
                parmArray[1] = new SqlParameter("@dateto", SqlDbType.DateTime);
                parmArray[1].Value = dateto;
                parmArray[2] = new SqlParameter("@transtypeset", SqlDbType.NVarChar,64);
                parmArray[2].Value = transtypeset;

                this.RunSP(conn, trans, "dn_fincoedtransactions",parmArray,_accountslist);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public bool ValidDecimal(int itemId)
        {
            bool decimalpoint = false;
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@itemId", SqlDbType.Int);
                parmArray[0].Value = itemId;
                parmArray[1] = new SqlParameter("@decimal", SqlDbType.Int);
                parmArray[1].Value = 0;
                parmArray[1].Direction = ParameterDirection.Output;

                RunSP("DN_CheckValidDecimalQtySP", parmArray);

                if (parmArray[1].Value != DBNull.Value)
                    decimalpoint = Convert.ToBoolean(parmArray[1].Value);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return decimalpoint;
        }


        public void UpdateRepossessionStatus(SqlConnection conn, SqlTransaction trans, string acctNo)
        {
           try
           {
              parmArray = new SqlParameter[1];
              parmArray[0] = new SqlParameter("@acctno", SqlDbType.NChar, 12);
              parmArray[0].Value = acctNo;
              this.RunSP(conn, trans, "DN_AccountUpdateRepoStatusSP", parmArray);
           }
           catch (SqlException ex)
           {
              LogSqlException(ex);
              throw ex;
           }
        }
        // Instant Credit Approval          CR907  jec 31/07/07
        public string InstantCredit(SqlConnection conn, SqlTransaction trans, string customerID, string accountNo)
        {
            string approved = string.Empty;
            try
            {
                parmArray = new SqlParameter[5];
                parmArray[0] = new SqlParameter("@piCustomerID", SqlDbType.VarChar, 20);
                parmArray[0].Value = customerID;
                parmArray[1] = new SqlParameter("@piAccountNo", SqlDbType.VarChar, 12);
                parmArray[1].Value = accountNo;
                parmArray[2] = new SqlParameter("@piProcess", SqlDbType.Char, 1);   //CR906
                parmArray[2].Value = "I";           
                parmArray[3] = new SqlParameter("@poInstantCredit", SqlDbType.Char, 1);
                parmArray[3].Value = string.Empty;
                parmArray[3].Direction = ParameterDirection.Output;
                parmArray[4] = new SqlParameter("@poLoanQualified", SqlDbType.Char, 1);
                parmArray[4].Value = string.Empty;
                parmArray[4].Direction = ParameterDirection.Output;

                this.RunSP(conn, trans, "InstantCreditApprovalsCheckGen", parmArray);

                if (parmArray[3].Value != DBNull.Value)             //CR906 
                    approved = (string)parmArray[3].Value;          //CR906
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return approved;
        }

        // Deposit/First Instalment Paid         CR907  jec 31/07/07
        public string DepFirstInstal(SqlConnection conn, SqlTransaction trans, string accountNo, decimal paidTot, int empeeno)
        {
            string paid = string.Empty;
            try
            {
                parmArray = new SqlParameter[4];              
                parmArray[0] = new SqlParameter("@piAccountNo", SqlDbType.VarChar, 12);
                parmArray[0].Value = accountNo;
                parmArray[1] = new SqlParameter("@piPaidTot", SqlDbType.Decimal);
                parmArray[1].Value = paidTot;
                parmArray[2] = new SqlParameter("@empeeno", SqlDbType.Int);
                parmArray[2].Value = empeeno;
                parmArray[3] = new SqlParameter("@poInstDepPaid", SqlDbType.Char, 1);
                parmArray[3].Value = string.Empty;
                parmArray[3].Direction = ParameterDirection.Output;

                this.RunSP(conn, trans, "DepositFirstInstalPaidSP", parmArray);

                if (parmArray[3].Value != DBNull.Value)
                    paid = (string)parmArray[3].Value;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return paid;
        }

        public bool IsGiftItem(int itemId, string location)     // RI
        {
            bool isGift;
            try
            {
                parmArray = new SqlParameter[3];
                //parmArray[0] = new SqlParameter("@itemNo", SqlDbType.VarChar, 8);
                //parmArray[0].Value = itemNo;
                parmArray[0] = new SqlParameter("@itemId", SqlDbType.Int);          // RI
                parmArray[0].Value = itemId;
                parmArray[1] = new SqlParameter("@stocklocn", SqlDbType.SmallInt);
                parmArray[1].Value = location;
                parmArray[2] = new SqlParameter("@isGiftItem", SqlDbType.Bit);
                parmArray[2].Direction = ParameterDirection.Output;

                this.RunSP("GetIsGiftItem", parmArray);

                isGift = Convert.ToBoolean(parmArray[2].Value);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return isGift;
        }


        public bool GetAccountHasBDW(string accountNo)
        {
            bool retVal = false;
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.Char, 12);
                parmArray[0].Value = accountNo;
                parmArray[1] = new SqlParameter("@HasBDW", SqlDbType.Bit);
                parmArray[1].Direction = ParameterDirection.Output;

                RunSP("AccountHasBDWSP", parmArray);

                if (parmArray[1].Value != DBNull.Value)
                {
                    retVal = Convert.ToBoolean(parmArray[1].Value);
                }
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw;
            }
            return retVal;
        }

        //IP - 17/03/08 - (69630)
		//Retrieves the agrmtno for an existing Cash & Go account
		//with a warranty, which is used as its buffno.
		public int GetExistingBuffNo(string accountNo)
		{
            int agrmtno = 0;

            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.Char, 12);
                parmArray[0].Value = accountNo;
                parmArray[1] = new SqlParameter("@agrmtno", SqlDbType.Int);
                parmArray[1].Direction = ParameterDirection.Output;

                RunSP("DN_GetExistingBuffNoSP", parmArray);

                if (parmArray[1].Value != DBNull.Value)
                {
                    agrmtno = Convert.ToInt32(parmArray[1].Value);
                }
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw;
            }
            return agrmtno;
			
			
		}

        //IP - 19/05/08 - Method that returns 'Application Status' as well as other 
        //account details to be displayed in the 'Account Status' screen.
        public DataTable AccountStatusGet(DateTime dateFrom, DateTime dateTo, int branchno)
        {
            //Create a data table to hold the details returned from the procedure.
            DataTable _accountStatusDet = new DataTable(TN.AcctDelStatus);

            try
            {
                parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@datefrom", SqlDbType.DateTime);
                parmArray[0].Value = dateFrom;
                parmArray[1] = new SqlParameter("@dateto", SqlDbType.DateTime);
                parmArray[1].Value = dateTo;
                parmArray[2] = new SqlParameter("@branchno", SqlDbType.Int);
                parmArray[2].Value = branchno;

                RunSP("DN_AccountStatusGet", parmArray, _accountStatusDet);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw;
            }

            //Return the data table.
            return _accountStatusDet;
        }

        //IP - 01/10/08 - Special Arrangements screen (Credit Collections)
        //Method retrieves the 'Outstanding Balance', 'Arrears' and 'Instalment Amount'
        //for an account to be displayed on the 'Special Arrangements' screen.
        public SPAAccountDetails GetSPAAcctDetails(string acctNo)
        {
            SPAAccountDetails spaacctdetails = new SPAAccountDetails();
            spaacctdetails.Outstbal = 0;
            spaacctdetails.Arrears = 0;
            spaacctdetails.Instalamount = 0;
            //spaacctdetails.DateAcctOpen = DBNull;
            spaacctdetails.PercentPaid = 0;
            //spaacctdetails.FinalPayDate = DBNull;
            spaacctdetails.Term = 0;
            spaacctdetails.MaxTerm = 0;
            spaacctdetails.CurrInstNo = 0;
            spaacctdetails.TermsType = "";
            spaacctdetails.RefinDeposit = 0;
            spaacctdetails.ServPcent = 0;
            spaacctdetails.CashPrice = 0;
            spaacctdetails.DueDay = 0;
            spaacctdetails.InsPcent = 0;            //IP - 29/04/10 - UAT(983)UAT5.2
            spaacctdetails.AdminPcent = 0;          //IP - 29/04/10 - UAT(983)UAT5.2
            spaacctdetails.Rebate = 0;              //IP - 30/04/10 - UAT(983)UAT5.2
            spaacctdetails.MinTerm = 0;             //IP - 22/09/10 - UAT(1017)UAT5.2
            spaacctdetails.ScoringBand ="";         //IP - 23/09/10 - UAT(1017)UAT5.2

            try
            {
                parmArray = new SqlParameter[21];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar);
                parmArray[0].Value = acctNo;
                parmArray[1] = new SqlParameter("@outstbal", SqlDbType.Money);
                parmArray[1].Value = 0;
                parmArray[1].Direction = ParameterDirection.Output;
                parmArray[2] = new SqlParameter("@arrears", SqlDbType.Money);
                parmArray[2].Value = 0;
                parmArray[2].Direction = ParameterDirection.Output;
                parmArray[3] = new SqlParameter("@instalamount", SqlDbType.Money);
                parmArray[3].Value = 0;
                parmArray[3].Direction = ParameterDirection.Output;
                parmArray[4] = new SqlParameter("@dateacctopen", SqlDbType.DateTime);
                parmArray[4].Value = "";
                parmArray[4].Direction = ParameterDirection.Output;
                parmArray[5] = new SqlParameter("@percentpaid", SqlDbType.Int);
                parmArray[5].Value = 0;
                parmArray[5].Direction = ParameterDirection.Output;
                parmArray[6] = new SqlParameter("@finalpaydate", SqlDbType.DateTime);
                parmArray[6].Value = "";
                parmArray[6].Direction = ParameterDirection.Output;
                parmArray[7] = new SqlParameter("@type", SqlDbType.Char);
                parmArray[7].Value = acctNo;
                parmArray[7].Direction = ParameterDirection.Output;
                parmArray[8] = new SqlParameter("@term", SqlDbType.Int);
                parmArray[8].Value = 0;
                parmArray[8].Direction = ParameterDirection.Output;
                parmArray[9] = new SqlParameter("@maxterm", SqlDbType.Int);
                parmArray[9].Value = 0;
                parmArray[9].Direction = ParameterDirection.Output;
                parmArray[10] = new SqlParameter("@currinstno", SqlDbType.Int);
                parmArray[10].Value = 0;
                parmArray[10].Direction = ParameterDirection.Output;
                parmArray[11] = new SqlParameter("@termstype", SqlDbType.VarChar, 2);
                parmArray[11].Value = 0;
                parmArray[11].Direction = ParameterDirection.Output;
                parmArray[12] = new SqlParameter("@refindeposit", SqlDbType.Money);
                parmArray[12].Value = 0;
                parmArray[12].Direction = ParameterDirection.Output;
                parmArray[13] = new SqlParameter("@servpcent", SqlDbType.Money);
                parmArray[13].Value = 0;
                parmArray[13].Direction = ParameterDirection.Output;
                parmArray[14] = new SqlParameter("@cashprice", SqlDbType.Money);
                parmArray[14].Value = 0;
                parmArray[14].Direction = ParameterDirection.Output;
                parmArray[15] = new SqlParameter("@dueday", SqlDbType.Int);
                parmArray[15].Value = 0;
                parmArray[15].Direction = ParameterDirection.Output;
                parmArray[16] = new SqlParameter("@inspcent", SqlDbType.Money);             //IP - 29/04/10 - UAT(983) UAT5.2
                parmArray[16].Value = 0;
                parmArray[16].Direction = ParameterDirection.Output;
                parmArray[17] = new SqlParameter("@adminpcent", SqlDbType.Money);           //IP - 29/04/10 - UAT(983) UAT5.2
                parmArray[17].Value = 0;
                parmArray[17].Direction = ParameterDirection.Output;
                parmArray[18] = new SqlParameter("@rebate", SqlDbType.Money);               //IP - 30/04/10 - UAT(983) UAT5.2
                parmArray[18].Value = 0;
                parmArray[18].Direction = ParameterDirection.Output;
                parmArray[19] = new SqlParameter("@minterm", SqlDbType.Int);                //IP - 22/09/10 - UAT(1017) UAT5.2
                parmArray[19].Value = 0;
                parmArray[19].Direction = ParameterDirection.Output;
                parmArray[20] = new SqlParameter("@scoringBand", SqlDbType.VarChar,1);           //IP - 23/09/10 - UAT(1017)UAT5.2
                parmArray[20].Value = 0;
                parmArray[20].Direction = ParameterDirection.Output;

                RunSP("CM_SPAGetAcctDetailsSP", parmArray);

                if (!Convert.IsDBNull(parmArray[1].Value))
                    spaacctdetails.Outstbal = decimal.Round((decimal)parmArray[1].Value, 2);
                if (!Convert.IsDBNull(parmArray[2].Value))
                    spaacctdetails.Arrears = decimal.Round((decimal)parmArray[2].Value, 2);
                if (!Convert.IsDBNull(parmArray[3].Value))
                    spaacctdetails.Instalamount = decimal.Round((decimal)parmArray[3].Value, 2);
                if (!Convert.IsDBNull(parmArray[4].Value))
                    spaacctdetails.DateAcctOpen = (DateTime)parmArray[4].Value;
                if (!Convert.IsDBNull(parmArray[5].Value))
                    spaacctdetails.PercentPaid = (int)parmArray[5].Value;
                if (!Convert.IsDBNull(parmArray[6].Value))
                    spaacctdetails.FinalPayDate = (DateTime)parmArray[6].Value;
                if (!Convert.IsDBNull(parmArray[7].Value))
                    spaacctdetails.AcctType = (string)parmArray[7].Value;
                if (!Convert.IsDBNull(parmArray[8].Value))
                    spaacctdetails.Term= (int)parmArray[8].Value;
                if (!Convert.IsDBNull(parmArray[9].Value))
                    spaacctdetails.MaxTerm = (int)parmArray[9].Value;
                if (!Convert.IsDBNull(parmArray[10].Value))
                    spaacctdetails.CurrInstNo = (int)parmArray[10].Value;
                if (!Convert.IsDBNull(parmArray[11].Value))
                    spaacctdetails.TermsType = (string)parmArray[11].Value;
                if (!Convert.IsDBNull(parmArray[12].Value))
                    spaacctdetails.RefinDeposit = decimal.Round((decimal)parmArray[12].Value,2);
                if (!Convert.IsDBNull(parmArray[13].Value))
                    spaacctdetails.ServPcent = (decimal)parmArray[13].Value;
                if (!Convert.IsDBNull(parmArray[14].Value))
                    spaacctdetails.CashPrice = decimal.Round((decimal)parmArray[14].Value,2);
                if (!Convert.IsDBNull(parmArray[15].Value))
                    spaacctdetails.DueDay = (int)parmArray[15].Value;
                if (!Convert.IsDBNull(parmArray[16].Value))                      //IP - 29/04/10 - UAT(983) UAT5.2
                    spaacctdetails.InsPcent = (decimal)parmArray[16].Value;
                if (!Convert.IsDBNull(parmArray[17].Value))
                    spaacctdetails.AdminPcent = (decimal)parmArray[17].Value;   //IP - 29/04/10 - UAT(983) UAT5.2
                if (!Convert.IsDBNull(parmArray[18].Value))
                    spaacctdetails.Rebate = (decimal)parmArray[18].Value;       //IP - 30/04/10 - UAT(983) UAT5.2
                if (!Convert.IsDBNull(parmArray[19].Value))
                    spaacctdetails.MinTerm = (int)parmArray[19].Value;          //IP - 22/09/10 - UAT(1017)UAT5.2
                if (!Convert.IsDBNull(parmArray[20].Value))
                    spaacctdetails.ScoringBand = (string)parmArray[20].Value;     //IP - 23/09/10 - UAT(1017)UAT5.2

                // Get Interest and Admin
                parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar);
                parmArray[0].Value = acctNo;
                parmArray[1] = new SqlParameter("@interest", SqlDbType.Money);
                parmArray[1].Value = 0;
                parmArray[1].Direction = ParameterDirection.Output;
                parmArray[2] = new SqlParameter("@admin", SqlDbType.Money);
                parmArray[2].Value = 0;
                parmArray[2].Direction = ParameterDirection.Output;

                RunSP("FintransGetChargesSP", parmArray);

                if (!Convert.IsDBNull(parmArray[1].Value))
                    spaacctdetails.Interest = decimal.Round((decimal)parmArray[1].Value, 2);
                if (!Convert.IsDBNull(parmArray[2].Value))
                    spaacctdetails.Admin = decimal.Round((decimal)parmArray[2].Value, 2);

            }
            catch (SqlException ex)
            {
                
                throw ex;
            }

            return spaacctdetails;

        }

        //IP - 06/10/08 - Special Arrangements screen (Credit Collections)
        //Method calculates the SPA Arrangement Schedule for the account and 
        //returns a data table with the schedule to be displayed on the 'Special Arrangements' screen.
        public DataTable SPACalculateArrangementSchedule(//SqlConnection conn, 
                                                         //SqlTransaction trans, 
                                                         string acctNo, 
                                                         char period, 
                                                         decimal arrangementAmt,
                                                         int numberOfInstalments, 
                                                         decimal instalmentAmt, 
                                                         decimal oddPaymentAmt, 
                                                         DateTime firstPaymentDate,
                                                         int numberRemainInstals,
                                                         decimal remainInstalAmt,
                                                         out DateTime finalPayDate)
        {
            DataTable arrangementSchedule = new DataTable("ArrangementSchedule");
            finalPayDate = new DateTime(1900,1,1);

            //try
            //{
                parmArray = new SqlParameter[10];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar, 12);
                parmArray[0].Value = acctNo;
                parmArray[1] = new SqlParameter("@period", SqlDbType.Char, 1);
                parmArray[1].Value = period;
                parmArray[2] = new SqlParameter("@arrangementAmt", SqlDbType.Money);
                parmArray[2].Value = arrangementAmt;
                parmArray[3] = new SqlParameter("@numberOfInstalments", SqlDbType.Int);
                parmArray[3].Value = numberOfInstalments;
                parmArray[4] = new SqlParameter("@instalmentAmt", SqlDbType.Money);
                parmArray[4].Value = instalmentAmt;
                parmArray[5] = new SqlParameter("@oddPaymentAmt", SqlDbType.Money);
                parmArray[5].Value = oddPaymentAmt;
                parmArray[6] = new SqlParameter("@firstPaymentDate", SqlDbType.DateTime);
                parmArray[6].Value = firstPaymentDate;
                parmArray[7] = new SqlParameter("@finalpaydate", SqlDbType.DateTime);
                parmArray[7].Value = finalPayDate;
                parmArray[7].Direction = ParameterDirection.Output;
                parmArray[8] = new SqlParameter("@numberRemainInstals", SqlDbType.Int);
                parmArray[8].Value = numberRemainInstals;
                parmArray[9] = new SqlParameter("@remainInstalAmt", SqlDbType.Money);
                parmArray[9].Value = remainInstalAmt;

                RunSP("CM_SPACalculateArrangementScheduleSP", parmArray, arrangementSchedule);

                if (!Convert.IsDBNull(parmArray[7].Value))
                    finalPayDate = Convert.ToDateTime(parmArray[7].Value);

            //}
            //catch (SqlException ex)    
            //{
                
            //    throw ex;
            //}

            return arrangementSchedule;

        }

        public void SPAWriteArrangementSchedule(SqlConnection conn, SqlTransaction trans,int empeeno,
                                                         string acctNo,
                                                         char period,                                                         
                                                         int numberOfInstalments,
                                                         decimal instalmentAmt,
                                                         decimal oddPaymentAmt,
                                                         DateTime firstPaymentDate,
                                                         string ReasonCode,
                                                         int numberRemainInstals,
                                                         decimal remainInstalAmt)
        {


            try
            {
                parmArray = new SqlParameter[10];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar, 12);
                parmArray[0].Value = acctNo;
                parmArray[1] = new SqlParameter("@period", SqlDbType.Char, 1);
                parmArray[1].Value = period;
                parmArray[2] = new SqlParameter("@numberOfInstalments", SqlDbType.Int);
                parmArray[2].Value = numberOfInstalments;
                parmArray[3] = new SqlParameter("@instalmentAmt", SqlDbType.Money);
                parmArray[3].Value = instalmentAmt;
                parmArray[4] = new SqlParameter("@oddPaymentAmt", SqlDbType.Money);
                parmArray[4].Value = oddPaymentAmt;
                parmArray[5] = new SqlParameter("@firstPaymentDate", SqlDbType.DateTime);
                parmArray[5].Value = firstPaymentDate;
                parmArray[6] = new SqlParameter("@empeeno", SqlDbType.Int);
                parmArray[6].Value = empeeno;
                parmArray[7] = new SqlParameter("@reason", SqlDbType.VarChar, 2);
                parmArray[7].Value = ReasonCode;
                parmArray[8] = new SqlParameter("@numberRemainInstals", SqlDbType.Int);
                parmArray[8].Value = numberRemainInstals;
                parmArray[9] = new SqlParameter("@remainInstalAmt", SqlDbType.Money);
                parmArray[9].Value = remainInstalAmt;

                RunSP("CM_SPAWriteArrangementScheduleSP", parmArray);

            }
            catch (SqlException ex)
            {

                throw ex;
            }
          
        }

        //IP & JC - CR976 - 21/01/09
        //Method that will write the new Instalplan and Agreement records for Extended Term SPA.
        public void SPAWriteRefinance(SqlConnection conn, SqlTransaction trans,int empeeno,
                                                    string acctNo,
                                                    char period,
                                                    decimal arrangementAmt,
                                                    int numberOfInstalments,
                                                    decimal instalmentAmt,
                                                    decimal oddPaymentAmt,
                                                    DateTime firstPaymentDate,
                                                    DateTime finalPaymentDate,
                                                    string reasonCode,
                                                    decimal serviceChg,
                                                    bool FreezeIntAdmin)
        {


            try
            {
                parmArray = new SqlParameter[12];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar, 12);
                parmArray[0].Value = acctNo;
                parmArray[1] = new SqlParameter("@period", SqlDbType.Char, 1);
                parmArray[1].Value = period;
                parmArray[2] = new SqlParameter("@arrangementAmt", SqlDbType.Money);
                parmArray[2].Value = arrangementAmt;
                parmArray[3] = new SqlParameter("@numberOfInstalments", SqlDbType.Int);
                parmArray[3].Value = numberOfInstalments;
                parmArray[4] = new SqlParameter("@instalmentAmt", SqlDbType.Money);
                parmArray[4].Value = instalmentAmt;
                parmArray[5] = new SqlParameter("@oddPaymentAmt", SqlDbType.Money);
                parmArray[5].Value = oddPaymentAmt;
                parmArray[6] = new SqlParameter("@firstPaymentDate", SqlDbType.DateTime);
                parmArray[6].Value = firstPaymentDate;
                parmArray[7] = new SqlParameter("@finalPaymentDate", SqlDbType.DateTime);
                parmArray[7].Value = finalPaymentDate;
                parmArray[8] = new SqlParameter("@empeeno", SqlDbType.Int);
                parmArray[8].Value = empeeno;
                parmArray[9] = new SqlParameter("@reason", SqlDbType.VarChar, 2);
                parmArray[9].Value = reasonCode;
                parmArray[10] = new SqlParameter("@serviceChg", SqlDbType.Money);
                parmArray[10].Value = serviceChg;
                parmArray[11] = new SqlParameter("@freezeInd", SqlDbType.Bit);
                parmArray[11].Value = FreezeIntAdmin;

                RunSP("CM_InstalplanAgreementWriteRefinanceSP", parmArray);

            }
            catch (SqlException ex)
            {

                throw ex;
            }

        }


        //IP - 05/02/09 - CR971 - Method will Unarchive/ un-settle accounts
        public void UnarchiveUnsettle(SqlConnection conn, SqlTransaction trans,
                                                          string acctNo, 
                                                          bool archivedAcct, 
                                                          bool unsettleAcct)
        {


            try
            {
                parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar, 12);
                parmArray[0].Value = acctNo;
                parmArray[1] = new SqlParameter("@archivedAcct", SqlDbType.Bit);
                parmArray[1].Value = archivedAcct;
                parmArray[2] = new SqlParameter("@unsettleAcct", SqlDbType.Bit);
                parmArray[2].Value = unsettleAcct;

                RunSP("ReinstateArchivedAccountSP", parmArray);

            }
            catch (SqlException ex)
            {

                throw ex;
            }

        }

        //IP - 04/02/10 - CR1072 - 3.1.9 Display Delivery Authorisation History in Account Details.
        /// <summary>
        /// Method that retrieves Delivery Authorisation history for an account.
        /// </summary>
        /// <param name="acctno"></param>
        public void LoadDAHistory(string acctno)
        {
            _acctDaHistory = new DataTable(TN.AcctDaHistory);

            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar, 12);
                parmArray[0].Value = acctno;

                RunSP("LoadAcctDAHistorySP", parmArray, _acctDaHistory); 

            }
            catch (SqlException ex)
            {

                throw ex;
            }

        }

        //Loyalty CR1017
        public string LockCheckbyAccount(string acctno, string user)
        {
            string lockuser = "";

            try
            {

                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar, 12);
                parmArray[0].Value = acctno;
                parmArray[1] = new SqlParameter("@user", SqlDbType.VarChar, 10);
                parmArray[1].Value = user;


                lockuser = ReturnString("LockCheckbyAccount", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            return lockuser;

        }

        //IP - 11/02/10 - CR1048 (Ref:3.1.2.5) Merged - Malaysia Enhancements (CR1072)
        //Method which retrieves the last payment method for a Cash & Go sale.
        public void GetCashAndGoLastPayMethod(string acctNo, int agrmtNo)
        {
            try
            {
                _cashAndGoPayments = new DataTable(TN.CashAndGoPayments);

                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@AcctNo", SqlDbType.NVarChar, 12);
                parmArray[0].Value = acctNo;
                parmArray[1] = new SqlParameter("@AgrmtNo", SqlDbType.Int);
                parmArray[1].Value = agrmtNo;

                RunSP("CashAndGoLastPayMethodGetSP", parmArray, _cashAndGoPayments);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

        }

        //IP - 18/02/10 - CR1072 - LW 69897 - Payment Fixes from 4.3 - Merge
        public DateTime GetDateLastpaid(SqlConnection con, SqlTransaction tran, string accountNo, decimal value)
        {
            DateTime datelaspaid = DateTime.MinValue.AddYears(1899);
            try
            {
                parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NChar, 12);
                parmArray[0].Value = accountNo;
                parmArray[1] = new SqlParameter("@Datelastpaid", SqlDbType.DateTime);
                parmArray[1].Value = null;
                parmArray[1].Direction = ParameterDirection.Output;
                parmArray[2] = new SqlParameter("@CorrectValue", SqlDbType.Money);
                parmArray[2].Value = value;
                this.RunSP(con, tran, "DN_AccountGetDatelastPaid", parmArray);

                if (parmArray[1].Value != DBNull.Value)
                    datelaspaid = (DateTime)parmArray[1].Value;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return datelaspaid;
        }

        //IP - 19/02/10 - CR1072 - LW 69770 - Printing Fixes from 4.3 - Merge
        public DataTable GetRedeliveryAfterRepossessionDetails(SqlConnection conn, SqlTransaction trans,
           string accountNumber, int stockLocn, out int buffNo)
        {
            DataTable dt = new DataTable(TN.DeliveryLineItems);
            try
            {

                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar, 12);
                parmArray[0].Value = accountNumber;
                parmArray[1] = new SqlParameter("@stockLocn", SqlDbType.Int);
                parmArray[1].Value = stockLocn;

                result = this.RunSP(conn, trans, "DN_CheckRedeliveryAfterRepossessionSP", parmArray, dt);

                if (dt.Rows.Count > 0)
                {
                    result = (int)Return.Success;
                    // todo buffno
                    buffNo = Convert.ToInt32(dt.Rows[0]["BuffNo"]);

                }
                else
                {
                    buffNo = 0;
                    result = (int)Return.Fail;
                }
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dt;
        }

        public decimal? ProvisionGetForAccount(string acctno)
        {
            parmArray = new SqlParameter[1];
            parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar, 12);
            parmArray[0].Value = acctno;

            return ReturnDecimal("ProvisionGetForAccount", parmArray);
        }

        public DataTable MaxAction(string accountNo)
        {
            DataTable dt = new DataTable("MaxAction");
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NChar, 12);
                parmArray[0].Value = accountNo;

                this.RunSP("BailactionMaxactionGetRecent", parmArray, dt);
                
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            return dt;
        }
        public bool CheckSRAcct(string acctno)
        {
            parmArray = new SqlParameter[1];
            parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar, 12);
            parmArray[0].Value = acctno;

            return ReturnBool("SR_ChargeAcctCheck", parmArray);
        }

        //Get amortized cash loan account details to display in tab
        public void GetAmortizedScheduleDetails(SqlConnection conn, SqlTransaction trans, string accountNumber)
        {
            _amortizedSchedule = new DataTable(TN.AmortizedSchedule);
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[0].Value = accountNumber;

                if (conn != null && trans != null)
                    result = this.RunSP(conn, trans, "DN_DisplayAmortizationScheduleSP", parmArray, _amortizedSchedule);
                else
                    result = this.RunSP("DN_DisplayAmortizationScheduleSP", parmArray, _amortizedSchedule);
            }
            catch(SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }
        //CR - Procedure to close service request when item is removed from sales order screen
        public void CloseServiceRequest(SqlConnection conn, SqlTransaction trans, string acctno, int parentItemId, int itemId)
        {
            parmArray = new SqlParameter[3];
            parmArray[0] = new SqlParameter("@Acctno", SqlDbType.NVarChar, 12);
            parmArray[0].Value = acctno;
            parmArray[1] = new SqlParameter("@ParentItemid", SqlDbType.Int);
            parmArray[1].Value = parentItemId;
            parmArray[2] = new SqlParameter("@ItemId", SqlDbType.Int);
            parmArray[2].Value = itemId;
            this.RunSP("CloseServiceRequestSP", parmArray);
        }

        public string GetInvoiveNumberWithVersion(string accountNumber)        {            parmArray = new SqlParameter[1];            parmArray[0] = new SqlParameter("@acctNo", SqlDbType.VarChar, 12);            parmArray[0].Value = accountNumber;            return this.ReturnString("DN_GetInvoiceNumberAndVersionByAcctnoSP", parmArray);        }
		public void FetchMambuServiceChargeInstalDetails(SqlConnection conn, SqlTransaction trans,
		  string acctNo,
		  out decimal serviceCharge,
		  out decimal instalAmt,
		  out decimal finalinstalAmt,
		   out decimal adminTax,
		   out decimal stampDuty,
		   out decimal agreementTotal
		  )
		{
			serviceCharge = 0;
			instalAmt = 0;
			finalinstalAmt = 0;
			adminTax = 0;
			stampDuty = 0;
			agreementTotal = 0;
			try
			{
				parmArray = new SqlParameter[7];

				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
				parmArray[0].Value = acctNo;

				parmArray[1] = new SqlParameter("@ServiceCharges", SqlDbType.Money);
				parmArray[1].Value = serviceCharge;
				parmArray[1].Direction = ParameterDirection.Output;

				parmArray[2] = new SqlParameter("@InstallmentAmt", SqlDbType.Money);
				parmArray[2].Value = instalAmt;
				parmArray[2].Direction = ParameterDirection.Output;

				parmArray[3] = new SqlParameter("@FinalInstallmentAmt", SqlDbType.Money);
				parmArray[3].Value = finalinstalAmt;
				parmArray[3].Direction = ParameterDirection.Output;

				parmArray[4] = new SqlParameter("@AdminTax", SqlDbType.Money);
				parmArray[4].Value = adminTax;
				parmArray[4].Direction = ParameterDirection.Output;

				parmArray[5] = new SqlParameter("@StampDuty", SqlDbType.Money);
				parmArray[5].Value = stampDuty;
				parmArray[5].Direction = ParameterDirection.Output;

				parmArray[6] = new SqlParameter("@AgreementTotal", SqlDbType.Money);
				parmArray[6].Value = stampDuty;
				parmArray[6].Direction = ParameterDirection.Output;

				if (conn != null && trans != null)
					result = this.RunSP(conn, trans, "BS_SP_Get_ServiceCharges_Installment", parmArray);
				else
					result = this.RunSP("BS_SP_Get_ServiceCharges_Installment", parmArray);

				if (result == 0)
				{
					if (parmArray[1].Value != DBNull.Value)
						serviceCharge = Convert.ToDecimal(parmArray[1].Value);
					if (parmArray[2].Value != DBNull.Value)
						instalAmt = Convert.ToDecimal(parmArray[2].Value);
					if (parmArray[3].Value != DBNull.Value)
						finalinstalAmt = Convert.ToDecimal(parmArray[3].Value);
					if (parmArray[4].Value != DBNull.Value)
						adminTax = Convert.ToDecimal(parmArray[4].Value);
					if (parmArray[5].Value != DBNull.Value)
						stampDuty = Convert.ToDecimal(parmArray[5].Value);
					if (parmArray[6].Value != DBNull.Value)
						agreementTotal = Convert.ToDecimal(parmArray[6].Value);
				}

			}
			catch (SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			
		}
		public void CheckAccountType(string acctno, out bool result)
		{
			result = false;
			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@AcctNo", SqlDbType.NVarChar, 12);
				parmArray[0].Value = acctno;
				parmArray[1] = new SqlParameter("@result", SqlDbType.Bit);
				parmArray[1].Value = 0;
				parmArray[1].Direction = ParameterDirection.Output;
				this.RunSP("SP_BS_CheckAccountType", parmArray);

				if (parmArray[1].Value != DBNull.Value)
					result = (bool)parmArray[1].Value;
			}
			catch (SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}
		public void ValidatePayment(short payMethod, out bool result)
		{
			result = false;
			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@PayMethod", SqlDbType.SmallInt);
				parmArray[0].Value = payMethod;
				parmArray[1] = new SqlParameter("@result", SqlDbType.Bit);
				parmArray[1].Value = 0;
				parmArray[1].Direction = ParameterDirection.Output;
				this.RunSP("SP_BS_ValidatePayment", parmArray);

				if (parmArray[1].Value != DBNull.Value)
					result = (bool)parmArray[1].Value;
			}
			catch (SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
	   }

		public void CheckIfDeposit(string acctno, decimal paymentAmt ,out bool result)
		{
			result = false;
			try
			{
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@AcctNo", SqlDbType.NVarChar, 12);
				parmArray[0].Value = acctno;
				parmArray[1] = new SqlParameter("@PaymentAmount", SqlDbType.Money);
				parmArray[1].Value = paymentAmt;
				parmArray[2] = new SqlParameter("@IsDeposit", SqlDbType.Bit);
				parmArray[2].Value = 0;
				parmArray[2].Direction = ParameterDirection.Output;
				this.RunSP("SP_BS_CheckDeposit", parmArray);

				if (parmArray[2].Value != DBNull.Value)
					result = (bool)parmArray[2].Value;
			}
			catch (SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void GetDepositAmount(string acctno, out decimal depositAmt)
		{
			depositAmt = 0;
			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@AcctNo", SqlDbType.NVarChar, 12);
				parmArray[0].Value = acctno;
				parmArray[1] = new SqlParameter("@DepositAmount", SqlDbType.Money);
				parmArray[1].Value = 0;
				parmArray[1].Direction = ParameterDirection.Output;
				this.RunSP("SP_BS_GetDeposit", parmArray);

				if (parmArray[1].Value != DBNull.Value)
					depositAmt = (decimal)parmArray[1].Value;
			}
			catch (SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void CheckIfDepositPayment(string acctno, decimal paymentAmt, out bool result)
		{
			result = false;
			try
			{
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@AcctNo", SqlDbType.NVarChar, 12);
				parmArray[0].Value = acctno;
				parmArray[1] = new SqlParameter("@PaymentAmount", SqlDbType.Money);
				parmArray[1].Value = paymentAmt;
				parmArray[2] = new SqlParameter("@IsPayment", SqlDbType.Bit);
				parmArray[2].Value = 0;
				parmArray[2].Direction = ParameterDirection.Output;
				this.RunSP("SP_BS_CheckDepositPayment", parmArray);

				if (parmArray[2].Value != DBNull.Value)
					result = (bool)parmArray[2].Value;
			}
			catch (SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		//GDIEC-661 Modify COSACS to support Bluestar accounts
		public DataSet GetPrintingInfoForCL(string acctNo)
		{
			DataSet countryMaintenanceData = new DataSet();

			try
			{
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@AcctNo", SqlDbType.NVarChar, 12);
				parmArray[0].Value = acctNo;

				this.RunSP("SP_BS_GetPrintingInfoForCL", parmArray, countryMaintenanceData);
			}
			catch (SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}

			return countryMaintenanceData;
		}
	}
}
