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
	/// Summary description for DAgreement.
	/// </summary>
	public class DAgreement:DALObject
	{
		private int _agreementNo=0;
		private DataTable _agreementlist;

		public int AgreementNumber
		{
			get
			{
				return _agreementNo;
			}
			set
			{
				_agreementNo = value;
			}
		}
		private string _codFlag="";
		public string CODFlag
		{
			get
			{
				return _codFlag;
			}
			set
			{
				_codFlag = value;
			}
		}
		private int _salesPerson=0;
		public int SalesPerson
		{
			get
			{
				return _salesPerson;
			}
			set
			{
				_salesPerson = value;
			}
		}
		private string _soa="";
		public string SOA
		{
			get
			{
				return _soa;
			}
			set
			{
				_soa = value;
			}
		}
		private string _holdProp="";
		public string HoldProp
		{
			get
			{
				return _holdProp;
			}
			set
			{
				_holdProp = value;
			}
		}
		private decimal _cashPrice=0;
		public decimal CashPrice
		{
			get
			{
				return _cashPrice;
			}
			set
			{
				_cashPrice = value;
			}
		}
		private decimal _agreementTotal=0;
		public decimal AgreementTotal
		{
			get
			{
				return _agreementTotal;
			}
			set
			{
				_agreementTotal = value;
			}
		}
		private decimal _deposit=0;
		public decimal Deposit
		{
			get{return _deposit;}
			set{_deposit=value;}
		}

		private decimal _serviceCharge=0;
		public decimal ServiceCharge
		{
			get{return _serviceCharge;}
			set{_serviceCharge=value;}
		}
		private decimal _discount=0;
		public decimal Discount
		{
			get{return _discount;}
			set{_discount = value;}
		}
		private string _accountNo = "";
		public string AccountNumber
		{
			get{return _accountNo;}
			set{_accountNo = value;}
		}
		private DateTime _agreementDate = DateTime.Today;
		public DateTime AgreementDate
		{
			get{return _agreementDate;}
			set{_agreementDate = value;}
		}
		private short _origBr = 0;
		public short OrigBr
		{
			get{return _origBr;}
			set{_origBr = value;}
		}
		private DateTime _depChequeClear = DateTime.Today;
		public DateTime DepositChequeClears
		{
			get{return _depChequeClear;}
			set{_depChequeClear = value;}
		}
		private string _holdMerch = "";
		public string HoldMerch
		{
			get{return _holdMerch;}
			set{_holdMerch = value;}
		}
		private DateTime _dateDel = DateTime.MinValue.AddYears(1899);
		public DateTime DateDel
		{
			get{return _dateDel;}
			set{_dateDel = value;}
		}
		private DateTime _dateNextDue = DateTime.Today;
		public DateTime DateNextDue
		{
			get{return _dateNextDue;}
			set{_dateNextDue = value;}
		}
		private decimal _oldAgreementBal = 0;
		public decimal OldAgreementBalance
		{
			get{return _oldAgreementBal;}
			set{_oldAgreementBal = value;}
		}
		private decimal _sundryChargeTotal = 0;
		public decimal SundryChargeTotal
		{
			get{return _sundryChargeTotal;}
			set{_sundryChargeTotal = value;}
		}
		private string _paymethod = "";
		public string PayMethod
		{
			get{return _paymethod;}
			set{_paymethod = value;}
		}
		private string _unpaidFlag = "";
		public string UnpaidFlag
		{
			get{return _unpaidFlag;}
			set{_unpaidFlag = value;}
		}
		private string _deliveryFlag = "";
		public string DeliveryFlag
		{
			get{return _deliveryFlag;}
			set{_deliveryFlag = value;}
		}
		private string _fullDelFlag = "";
		public string FullDelFlag
		{
			get{return _fullDelFlag;}
			set{_fullDelFlag = value;}
		}
		private string _paymentMethod = "";
		public string PaymentMethod
		{
			get{return _paymentMethod;}
			set{_paymentMethod = value;}
		}
		private int? _employeeNumAuth = null;
		public int? EmployeeNumAuth
		{
			get{return _employeeNumAuth;}
			set{_employeeNumAuth = value;}
		}
		private DateTime? _dateAuth = null;
		public DateTime? DateAuth
		{
			get{return _dateAuth;}
			set{_dateAuth = value;}
		}
		private int _employeeNumChange = 0;
		public int EmployeeNumChange
		{
			get{return _employeeNumChange;}
			set{_employeeNumChange = value;}
		}
		private DateTime _dateChange = DateTime.Now;
		public DateTime DateChange
		{
			get{return _dateChange;}
			set{_dateChange = value;}
		}
		private decimal _pxalloxed = 0;
		public decimal PxAllowed
		{
			get{return _pxalloxed;}
			set{_pxalloxed = value;}
		}
		private decimal _instalamount = 0;
		private decimal _finalinstalamt = 0;

		private int _createdBy = 0;
		public int CreatedBy
		{
			get{return _createdBy;}
			set{_createdBy = value;}
		}
		private short _paymentholidays = 0;
		public short PaymentHolidays
		{
			get{return _paymentholidays;}
			set{_paymentholidays = value;}
		}

        private string _auditsource = "";
        public string AuditSource
        {
            get { return _auditsource; }
            set { _auditsource = value; }
        }

        //IP - #3921 - CR1232
        private decimal _adminFee = 0;
        public decimal AdminFee
        {
            get { return _adminFee; }
            set { _adminFee = value; }
        }

        //IP - #3921 - CR1232
        private decimal _insCharge = 0;
        public decimal InsCharge
        {
            get { return _insCharge; }
            set { _insCharge = value; }
        }

        private bool _taxFree = false;
        public bool TaxFree
        {
            get { return _taxFree; }
            set { _taxFree = value; }
        }

        //IP/JC - 29/05/12 - Warehouse & Deliveries
        private DataTable _lineItemBooking = null;
        public DataTable LineItemBooking
        {
            get { return _lineItemBooking; }
            set { _lineItemBooking = value; }
        }
        private string _AgreementInvoiceNumber = "";
        public string AgreementInvoiceNumber
        {
            get { return _AgreementInvoiceNumber; }
            set { _AgreementInvoiceNumber = value; }
        }


        //
        // Constructors
        //

        public DAgreement()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public DAgreement(string accountNo, int agreementNo)
		{
			this.Populate(null, null, accountNo, agreementNo);
		}

		public DAgreement(SqlConnection conn, SqlTransaction trans, string accountNo, int agreementNo)
		{
			this.Populate(conn, trans, accountNo, agreementNo);
		}

		public bool Populate(SqlConnection conn, SqlTransaction trans, 
								string accountNo, int agreementNo)
		{
			bool exists = true;

			try
			{
				parmArray = new SqlParameter[35];
				parmArray[0] = new SqlParameter("@origBr", SqlDbType.SmallInt);
				parmArray[0].Value = 0;
				parmArray[0].Direction = ParameterDirection.Output;
				parmArray[1] = new SqlParameter("@acctNo", SqlDbType.NVarChar,12);
				parmArray[1].Value = accountNo;
				parmArray[2] = new SqlParameter("@agreementNo", SqlDbType.Int);
				parmArray[2].Value = agreementNo;
				parmArray[3] = new SqlParameter("@agreementDate", SqlDbType.DateTime);
				parmArray[3].Direction = ParameterDirection.Output;
				parmArray[4] = new SqlParameter("@salesPerson", SqlDbType.Int);
				parmArray[4].Value = 0;
				parmArray[4].Direction = ParameterDirection.Output;
				parmArray[5] = new SqlParameter("@depChqClears", SqlDbType.DateTime);
				parmArray[5].Direction = ParameterDirection.Output;
				parmArray[6] = new SqlParameter("@holdMerch", SqlDbType.NChar,1);
				parmArray[6].Value = "";
				parmArray[6].Direction = ParameterDirection.Output;
				parmArray[7] = new SqlParameter("@holdProp", SqlDbType.NChar,1);
				parmArray[7].Value = "";
				parmArray[7].Direction = ParameterDirection.Output;
				parmArray[8] = new SqlParameter("@dateDel", SqlDbType.DateTime);
				parmArray[8].Direction = ParameterDirection.Output;
				parmArray[9] = new SqlParameter("@dateNextDue", SqlDbType.DateTime);
				parmArray[9].Direction = ParameterDirection.Output;
				parmArray[10] = new SqlParameter("@oldAgreementBal", SqlDbType.Money);
				parmArray[10].Value = 0;
				parmArray[10].Direction = ParameterDirection.Output;
				parmArray[11] = new SqlParameter("@cashPrice", SqlDbType.Money);
				parmArray[11].Value = 0;
				parmArray[11].Direction = ParameterDirection.Output;
				parmArray[12] = new SqlParameter("@discount", SqlDbType.Money);
				parmArray[12].Value = 0;
				parmArray[12].Direction = ParameterDirection.Output;
				parmArray[13] = new SqlParameter("@pxallowed", SqlDbType.Money);
				parmArray[13].Value = 0;
				parmArray[13].Direction = ParameterDirection.Output;
				parmArray[14] = new SqlParameter("@serviceCharge", SqlDbType.Money);
				parmArray[14].Value = 0;
				parmArray[14].Direction = ParameterDirection.Output;
				parmArray[15] = new SqlParameter("@sundryChargeTotal", SqlDbType.Money);
				parmArray[15].Value = 0;
				parmArray[15].Direction = ParameterDirection.Output;
				parmArray[16] = new SqlParameter("@agreementTotal", SqlDbType.Money);
				parmArray[16].Value =0;
				parmArray[16].Direction = ParameterDirection.Output;
				parmArray[17] = new SqlParameter("@deposit", SqlDbType.Money);
				parmArray[17].Value = 0;
				parmArray[17].Direction = ParameterDirection.Output;
				parmArray[18] = new SqlParameter("@codFlag", SqlDbType.NChar,1);
				parmArray[18].Value = "";
				parmArray[18].Direction = ParameterDirection.Output;
				parmArray[19] = new SqlParameter("@soa", SqlDbType.NVarChar,4);
				parmArray[19].Value = "";
				parmArray[19].Direction = ParameterDirection.Output;
				parmArray[20] = new SqlParameter("@paymethod", SqlDbType.NVarChar,1);
				parmArray[20].Value = "";
				parmArray[20].Direction = ParameterDirection.Output;
				parmArray[21] = new SqlParameter("@unpaidFlag", SqlDbType.NVarChar,1);
				parmArray[21].Value = "";
				parmArray[21].Direction = ParameterDirection.Output;
				parmArray[22] = new SqlParameter("@deliveryFlag", SqlDbType.NVarChar,1);
				parmArray[22].Value = "";
				parmArray[22].Direction = ParameterDirection.Output;
				parmArray[23] = new SqlParameter("@fullDelFlag", SqlDbType.NVarChar,1);
				parmArray[23].Value = "";
				parmArray[23].Direction = ParameterDirection.Output;
				parmArray[24] = new SqlParameter("@PaymentMethod", SqlDbType.NChar,1);
				parmArray[24].Value = "";
				parmArray[24].Direction = ParameterDirection.Output;
				parmArray[25] = new SqlParameter("@employeeNumAuth", SqlDbType.Int);
				parmArray[25].Value = 0;
				parmArray[25].Direction = ParameterDirection.Output;
				parmArray[26] = new SqlParameter("@dateAuth", SqlDbType.DateTime);
				parmArray[26].Direction = ParameterDirection.Output;
				parmArray[27] = new SqlParameter("@employeeNumChange", SqlDbType.Int);
				parmArray[27].Value = 0;
				parmArray[27].Direction = ParameterDirection.Output;
				parmArray[28] = new SqlParameter("@dateChange", SqlDbType.DateTime);
				parmArray[28].Direction = ParameterDirection.Output;
				parmArray[29] = new SqlParameter("@createdby", SqlDbType.Int);
				parmArray[29].Direction = ParameterDirection.Output;
				parmArray[30] = new SqlParameter("@paymentholidays", SqlDbType.SmallInt);
				parmArray[30].Direction = ParameterDirection.Output;
                parmArray[31] = new SqlParameter("@source", SqlDbType.VarChar,32);
                parmArray[31].Direction = ParameterDirection.Output;
                parmArray[32] = new SqlParameter("@adminFee", SqlDbType.Money);
                parmArray[32].Direction = ParameterDirection.Output;
                parmArray[33] = new SqlParameter("@insCharge", SqlDbType.Money);
                parmArray[33].Direction = ParameterDirection.Output;
                parmArray[34] = new SqlParameter("@taxFree", SqlDbType.Bit);
                parmArray[34].Direction = ParameterDirection.Output;
             
				if(conn!=null && trans!=null)
                    result = this.RunSP(conn, trans, "DN_AgreementPopulateSP", parmArray);
				else
					result = this.RunSP("DN_AgreementPopulateSP", parmArray);

				if(result == -1)
					exists = false;

				if(result == 0)
				{
                    this.EmployeeNumAuth = null;        // #16198
                    this.DateAuth = null;               // #16198
					if(!Convert.IsDBNull(parmArray[0].Value))
						this.OrigBr = (short)parmArray[0].Value;
					this.AccountNumber = accountNo;
					this.AgreementNumber = agreementNo;
					if(!Convert.IsDBNull(parmArray[3].Value))
						this.AgreementDate = (DateTime)parmArray[3].Value;
					if(!Convert.IsDBNull(parmArray[4].Value))
						this.SalesPerson = (int)parmArray[4].Value;
					if(!Convert.IsDBNull(parmArray[5].Value))
						this.DepositChequeClears = (DateTime)parmArray[5].Value;
					if(!Convert.IsDBNull(parmArray[6].Value))
						this.HoldMerch = (string)parmArray[6].Value;
					if(!Convert.IsDBNull(parmArray[7].Value))
						this.HoldProp = (string)parmArray[7].Value;
					if(!Convert.IsDBNull(parmArray[8].Value))
						this.DateDel = (DateTime)parmArray[8].Value;
					if(!Convert.IsDBNull(parmArray[9].Value))
						this.DateNextDue = (DateTime)parmArray[9].Value;
					if(!Convert.IsDBNull(parmArray[10].Value))
						this.OldAgreementBalance = (decimal)parmArray[10].Value;
					if(!Convert.IsDBNull(parmArray[11].Value))
						this.CashPrice = (decimal)parmArray[11].Value;
					if(!Convert.IsDBNull(parmArray[12].Value))
						this.Discount = (decimal)parmArray[12].Value;
					if(!Convert.IsDBNull(parmArray[13].Value))
						this.PxAllowed = (decimal)parmArray[13].Value;
					if(!Convert.IsDBNull(parmArray[14].Value))
						this.ServiceCharge = (decimal)parmArray[14].Value;
					if(!Convert.IsDBNull(parmArray[15].Value))
						this.SundryChargeTotal = (decimal)parmArray[15].Value;
					if(!Convert.IsDBNull(parmArray[16].Value))
						this.AgreementTotal = (decimal)parmArray[16].Value;
					if(!Convert.IsDBNull(parmArray[17].Value))
						this.Deposit = (decimal)parmArray[17].Value;
					if(!Convert.IsDBNull(parmArray[18].Value))
						this.CODFlag = (string)parmArray[18].Value;
					if(!Convert.IsDBNull(parmArray[19].Value))
						this.SOA = (string)parmArray[19].Value;
					if(!Convert.IsDBNull(parmArray[20].Value))
						this.PayMethod = (string)parmArray[20].Value;
					if(!Convert.IsDBNull(parmArray[21].Value))
						this.UnpaidFlag = (string)parmArray[21].Value;
					if(!Convert.IsDBNull(parmArray[22].Value))
						this.DeliveryFlag = (string)parmArray[22].Value;
					if(!Convert.IsDBNull(parmArray[23].Value))
						this.FullDelFlag = (string)parmArray[23].Value;
					if(!Convert.IsDBNull(parmArray[24].Value))
						this.PaymentMethod = (string)parmArray[24].Value;
                    if (!Convert.IsDBNull(parmArray[25].Value))
						this.EmployeeNumAuth = (int?)parmArray[25].Value;
                    if (!Convert.IsDBNull(parmArray[26].Value))
						this.DateAuth = (DateTime?)parmArray[26].Value;
                    if (!Convert.IsDBNull(parmArray[27].Value))
						this.EmployeeNumChange = (int)parmArray[27].Value;
					if(!Convert.IsDBNull(parmArray[28].Value))
						this.DateChange = (DateTime)parmArray[28].Value;
					if(parmArray[29].Value != DBNull.Value)
						this.CreatedBy = (int)parmArray[29].Value;
					if(parmArray[30].Value != DBNull.Value)
						this.PaymentHolidays = (short)parmArray[30].Value;
                    if (parmArray[31].Value != DBNull.Value)
                        this.AuditSource= (string)parmArray[31].Value;
                    if (parmArray[32].Value != DBNull.Value)
                        this.AdminFee = (decimal)parmArray[32].Value;                           //IP - 11/10/11 - #3921 - CR1232
                    if (parmArray[33].Value != DBNull.Value)
                        this.InsCharge = (decimal)parmArray[33].Value;                          //IP - 11/10/11 - #3921 - CR1232
                    if(parmArray[34].Value != DBNull.Value)
                        this.TaxFree = (bool)parmArray[34].Value;        
				}
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return exists;
		}

		public void Save(SqlConnection conn, SqlTransaction trans)
		{
			try
			{
                if (this.AgreementNumber == 0) // Agreement Number must always be 1.
                    this.AgreementNumber = 1;

				parmArray = new SqlParameter[33];
				parmArray[0] = new SqlParameter("@origBr", SqlDbType.SmallInt);
				parmArray[0].Value = this.OrigBr;
				parmArray[1] = new SqlParameter("@acctNo", SqlDbType.NVarChar,12);
				parmArray[1].Value = this.AccountNumber;
				parmArray[2] = new SqlParameter("@agreementNo", SqlDbType.Int);
				parmArray[2].Value = this.AgreementNumber;
				parmArray[3] = new SqlParameter("@agreementDate", SqlDbType.DateTime);
				parmArray[3].Value = this.AgreementDate;
				parmArray[4] = new SqlParameter("@salesPerson", SqlDbType.Int);
				parmArray[4].Value = this.SalesPerson;
				parmArray[5] = new SqlParameter("@depChqClears", SqlDbType.DateTime);
				parmArray[5].Value = this.DepositChequeClears;
				parmArray[6] = new SqlParameter("@holdMerch", SqlDbType.NChar,1);
				parmArray[6].Value = this.HoldMerch;
				parmArray[7] = new SqlParameter("@holdProp", SqlDbType.NChar,1);
				parmArray[7].Value = this.HoldProp;
				parmArray[8] = new SqlParameter("@dateDel", SqlDbType.DateTime);
				parmArray[8].Value = this.DateDel;				
				parmArray[9] = new SqlParameter("@dateNextDue", SqlDbType.DateTime);
				parmArray[9].Value = this.DateNextDue;
				parmArray[10] = new SqlParameter("@oldAgreementBal", SqlDbType.Money);
				parmArray[10].Value = this.OldAgreementBalance;
				parmArray[11] = new SqlParameter("@cashPrice", SqlDbType.Money);
				parmArray[11].Value = this.CashPrice;
				parmArray[12] = new SqlParameter("@discount", SqlDbType.Money);
				parmArray[12].Value = this.Discount;
				parmArray[13] = new SqlParameter("@pxallowed", SqlDbType.Money);
				parmArray[13].Value = this.PxAllowed;
				parmArray[14] = new SqlParameter("@serviceCharge", SqlDbType.Money);
				parmArray[14].Value = this.ServiceCharge;
				parmArray[15] = new SqlParameter("@sundryChargeTotal", SqlDbType.Money);
				parmArray[15].Value = this.SundryChargeTotal;
				parmArray[16] = new SqlParameter("@agreementTotal", SqlDbType.Money);
				parmArray[16].Value = this.AgreementTotal;
				parmArray[17] = new SqlParameter("@deposit", SqlDbType.Money);
				parmArray[17].Value = this.Deposit;
				parmArray[18] = new SqlParameter("@codFlag", SqlDbType.NChar,1);
				parmArray[18].Value = this.CODFlag;
				parmArray[19] = new SqlParameter("@soa", SqlDbType.NVarChar,4);
				parmArray[19].Value = this.SOA;
				parmArray[20] = new SqlParameter("@paymethod", SqlDbType.NVarChar,1);
				parmArray[20].Value = this.PayMethod;
				parmArray[21] = new SqlParameter("@unpaidFlag", SqlDbType.NVarChar,1);
				parmArray[21].Value = this.UnpaidFlag;
				parmArray[22] = new SqlParameter("@deliveryFlag", SqlDbType.NVarChar,1);
				parmArray[22].Value = this.DeliveryFlag;
				parmArray[23] = new SqlParameter("@fullDelFlag", SqlDbType.NVarChar,1);
				parmArray[23].Value = this.FullDelFlag;
				parmArray[24] = new SqlParameter("@PaymentMethod", SqlDbType.NChar,1);
				parmArray[24].Value = this.PaymentMethod;
                parmArray[25] = new SqlParameter("@employeeNumAuth", SqlDbType.Int);
                if(this.EmployeeNumAuth.HasValue)
                    parmArray[25].Value = this.EmployeeNumAuth.Value;
                else
                    parmArray[25].Value = DBNull.Value;
				parmArray[26] = new SqlParameter("@dateAuth", SqlDbType.DateTime);
                if (this.DateAuth.HasValue)
                    parmArray[26].Value = this.DateAuth.Value;
                else
                    parmArray[26].Value = DBNull.Value;
				parmArray[27] = new SqlParameter("@employeeNumChange", SqlDbType.Int);
				parmArray[27].Value = this.EmployeeNumChange;
				parmArray[28] = new SqlParameter("@dateChange", SqlDbType.DateTime);
				parmArray[28].Value = this.DateChange;
				parmArray[29] = new SqlParameter("@createdby", SqlDbType.Int);
				parmArray[29].Value = this.CreatedBy;
				parmArray[30] = new SqlParameter("@paymentholidays", SqlDbType.SmallInt);
				parmArray[30].Value = this.PaymentHolidays;
                parmArray[31] = new SqlParameter("@source", SqlDbType.NVarChar, 15);
                parmArray[31].Value = this.AuditSource;
                parmArray[32] = new SqlParameter("@taxFree", SqlDbType.Bit);
                parmArray[32].Value = this.TaxFree;

				this.RunSP(conn, trans, "DN_AgreementUpdateSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

        public int GetAgreement(SqlConnection conn, SqlTransaction trans,string accountNumber, int ageementNumber)
		{
			try
			{
				_agreementlist = new DataTable(TN.Agreements);

				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNumber;
                parmArray[1] = new SqlParameter("@agrmtno", SqlDbType.Int);
                parmArray[1].Value = ageementNumber;
                if (conn !=null)
                    result = this.RunSP(conn, trans, "DN_AgreementGetSP", parmArray, _agreementlist);
			    else
                    result = this.RunSP("DN_AgreementGetSP", parmArray, _agreementlist);
			
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

        //CR-2018-13 – Raj – 07 Dec 18 –  Pass  the parameter Invoice Number to Get Account Number 
        public string GetInvoiceAcctDetails(string invoicenumber)
        {
            string accountNo = string.Empty;
            try
            {
                DataTable dt = new DataTable();
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@invoiceno", SqlDbType.NVarChar, 14);
                parmArray[0].Value = invoicenumber;
                result = this.RunSP("DN_GetInvoiceAccountDetailsSP", parmArray,dt);
                //checked dt has rows and return value in variable 
                if (dt.Rows.Count > 0)
                {
                    accountNo = Convert.ToString(dt.Rows[0]["acctno"]);
                } 
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return accountNo;
        }

        //IP - 03/02/10 - CR1072 - 3.1.9 - Added Source of Delivery Authorisation
        public int ClearProposal(SqlConnection conn, SqlTransaction trans, string accountNumber, string source)
		{
			try
			{
                _lineItemBooking = new DataTable();

				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNumber;
				parmArray[1] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[1].Value = User;
                parmArray[2] = new SqlParameter("@source", SqlDbType.NVarChar, 10);
                parmArray[2].Value = source;

                result = this.RunSP(conn, trans, "DN_ProposalClearSP", parmArray, _lineItemBooking);     //IP/JC - 29/05/12 - Warehouse & Deliveries
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

        // This method is used to deliver only non stock items for account.
        public void DeliverNonStocks(SqlConnection conn, SqlTransaction trans, string accountNumber)
        {
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[0].Value = accountNumber;
                parmArray[1] = new SqlParameter("user", SqlDbType.NVarChar, 12);
                parmArray[1].Value = User;

                this.RunSP(conn, trans, "DeliverNonStocks", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }            

        //Get amortized cash loan accounts instalment plan
        public void CalculateAmortizedCashLoan(SqlConnection conn, SqlTransaction trans, decimal loanAmount, decimal serviceChgpct, decimal term, decimal instalment, out decimal totalServiceCharge, out decimal finalInstal)
        {
            totalServiceCharge = 0;
            finalInstal = 0;

            parmArray = new SqlParameter[6];
            parmArray[0] = new SqlParameter("@principal", SqlDbType.Decimal);
            parmArray[0].Value = loanAmount;
            parmArray[1] = new SqlParameter("@servicechgpct", SqlDbType.Float);
            parmArray[1].Value = serviceChgpct;
            parmArray[2] = new SqlParameter("@term", SqlDbType.Decimal);
            parmArray[2].Value = term;
            parmArray[3] = new SqlParameter("@instalment", SqlDbType.Decimal);
            parmArray[3].Value = instalment;
            parmArray[4] = new SqlParameter("@totalservicechg", SqlDbType.Decimal);
            parmArray[4].Direction = ParameterDirection.Output;
            parmArray[4].Scale = 2;
            parmArray[5] = new SqlParameter("@finalinstal", SqlDbType.Decimal);
            parmArray[5].Direction = ParameterDirection.Output;
            parmArray[5].Scale = 2;

            if (conn != null && trans != null)
                result = this.RunSP(conn, trans, "DN_CalculateAmortizedCLScheduleSP", parmArray);
            else
                result = this.RunSP("DN_CalculateAmortizedCLScheduleSP", parmArray);

            totalServiceCharge = Convert.ToDecimal(parmArray[4].Value);
            finalInstal = Convert.ToDecimal(parmArray[5].Value);
        }


        public int UpdateHoldProp(SqlConnection conn, SqlTransaction trans)
		{
			try
			{
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar,12);
				parmArray[0].Value = this.AccountNumber;
				parmArray[1] = new SqlParameter("@agreementNo", SqlDbType.Int);
				parmArray[1].Value = this.AgreementNumber;
				parmArray[2] = new SqlParameter("@holdProp", SqlDbType.NChar,1);
				parmArray[2].Value = this.HoldProp;

				result = this.RunSP(conn, trans, "DN_AgreementUpdateHoldPropSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

		public DataTable GetAuditData(string accountNo, int rowcount)
		{
			DataTable dt = new DataTable(TN.AgreementAudit);

			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@rowcount", SqlDbType.Int);
				parmArray[1].Value = rowcount;

				this.RunSP("DN_AgreementAuditGetSP", parmArray, dt);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return dt;
		}

        public void AgrmtTotalBFCollection(SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar, 12);
                parmArray[0].Value = this.AccountNumber;
                parmArray[1] = new SqlParameter("@agrmttotal", SqlDbType.Money);
                parmArray[1].Value = this.AgreementTotal;
                parmArray[2] = new SqlParameter("@user", SqlDbType.Int);
                parmArray[2].Value = this.EmployeeNumChange;

                this.RunSP(conn, trans, "DN_AgreementBfCollectionSaveSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public bool isReprint(string acctno)
        {
            parmArray = new SqlParameter[1];
            parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar, 12);
            parmArray[0].Value = acctno;
            return ReturnBool("AgreementIsTaxPrinted", parmArray);
        }


		public decimal InstalAmount
		{
			get
			{
				return _instalamount;
			}
			set
			{
				_instalamount = value;
			}
		}

		public decimal FinalInstalAmount
		{
			get
			{
				return _finalinstalamt;
			}
			set
			{
				_finalinstalamt = value;
			}
		}
		
		public DataTable AgreementList
		{
			get
			{
				return _agreementlist;
			}
		}
		
		
	}
}
