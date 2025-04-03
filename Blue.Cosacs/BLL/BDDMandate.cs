using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using STL.DAL;
using STL.Common;
using STL.Common.Static;
using STL.Common.Constants.Giro;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;


namespace STL.BLL
{
    /// <summary>
    /// The DDMandate table holds history records and 'deleted' mandate records as
    /// well as current mandate records. Normally only the current records have a null
    /// end date or an end date after the next effective date (next earliest bank date).
    /// However this might not be the case if the LeadTime country parameter is changed.
    /// Therefore a query for a Current record must check for a Current mandate status.
    ///
    /// METHODS:
    ///
    /// BDDMandate - Constructor to init the attributes (must init dates used everywhere)
    /// CancelMandates - Cancel mandates not approved or with a cancelled rejection
    /// ConfirmMandates - Start all mandates with an Approval Date and a Date Delivered
    /// Get - Get the mandate for the Mandate Id or the Account Number
    /// GiroDateFirst - Adjust Date First for giro if necessary
    /// InitDates - Calc the effective date and retrieve the minimum period
    /// NewMandate (PRIVATE) - Create a new mandate or a history record
    /// NullDates - Set blank dates to NULL
    /// PrivateInit - Initialise private attributes
    /// ReturnMandate - Send a return mandate letter
    /// SaveMandate - Save new or changed mandate details
    /// UnDelete - Copy bank details to a new mandate application
    /// Validate - Validate mandate details
    /// </summary>
    /// 


    public class BDDMandate : CommonObject
    {
        //
        // DA properties
        //
        private int _mandateId;
        private string _acctNo;
        private DateTime _approvalDate;
        private string _bankAcctName;
        private string _bankAcctNo;
        private string _bankBranchNo;
        private string _bankCode;
        private string _cancelReason;
        private int _changedBy;
        private string _comment;
        private int _dueDayId;
        private DateTime _endDate;
        private bool _noFees;
        private DateTime _receiveDate;
        private int _rejectCount;
        private DateTime _returnDate;
        private DateTime _startDate;
        private string _status;
        private DateTime _submitDate;
        private string _customerName;
        private decimal _instalAmount;

        public int mandateId
        {
            get
            {
                return _mandateId;
            }
            set
            {
                _mandateId = value;
            }
        }

        public string acctNo
        {
            get
            {
                return _acctNo;
            }
            set
            {
                _acctNo = value;
            }
        }

        public DateTime approvalDate
        {
            get
            {
                return _approvalDate;
            }
            set
            {
                _approvalDate = value;
            }
        }

        public string bankAcctName
        {
            get
            {
                return _bankAcctName;
            }
            set
            {
                _bankAcctName = value;
            }
        }

        public string bankAcctNo
        {
            get
            {
                return _bankAcctNo;
            }
            set
            {
                _bankAcctNo = value;
            }
        }

        public string bankBranchNo
        {
            get
            {
                return _bankBranchNo;
            }
            set
            {
                _bankBranchNo = value;
            }
        }

        public string bankCode
        {
            get
            {
                return _bankCode;
            }
            set
            {
                _bankCode = value;
            }
        }

        public string cancelReason
        {
            get
            {
                return _cancelReason;
            }
            set
            {
                _cancelReason = value;
            }
        }

        public int changedBy
        {
            get
            {
                return _changedBy;
            }
            set
            {
                _changedBy = value;
            }
        }

        public string comment
        {
            get
            {
                return _comment;
            }
            set
            {
                _comment = value;
            }
        }

        public int dueDayId
        {
            get
            {
                return _dueDayId;
            }
            set
            {
                _dueDayId = value;
            }
        }

        public DateTime endDate
        {
            get
            {
                return _endDate;
            }
            set
            {
                _endDate = value;
            }
        }

        public bool noFees
        {
            get
            {
                return _noFees;
            }
            set
            {
                _noFees = value;
            }
        }

        public DateTime receiveDate
        {
            get
            {
                return _receiveDate;
            }
            set
            {
                _receiveDate = value;
            }
        }

        public int rejectCount
        {
            get
            {
                return _rejectCount;
            }
            set
            {
                _rejectCount = value;
            }
        }

        public DateTime returnDate
        {
            get
            {
                return _returnDate;
            }
            set
            {
                _returnDate = value;
            }
        }

        public DateTime startDate
        {
            get
            {
                return _startDate;
            }
            set
            {
                _startDate = value;
            }
        }

        public string status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
            }
        }

        public DateTime submitDate
        {
            get
            {
                return _submitDate;
            }
            set
            {
                _submitDate = value;
            }
        }

        public string customerName
        {
            get
            {
                return _customerName;
            }
            set
            {
                _customerName = value;
            }
        }

        public decimal instalAmount
        {
            get
            {
                return _instalAmount;
            }
            set
            {
                _instalAmount = value;
            }
        }


        //
        // BL private properties
        //
        private bool _init = false;
        private bool _acctLoaded = false;
        private bool _loaded = false;
        private string _activeText;
        private Color _activeTextColour;
        private string _cancelDesc;
        private string _curAcctNo;
        private string _curBankAcctName;
        private string _curBankAcctNo;
        private string _curBankBranchNo;
        private string _curBankCode;
        private int _curDueDayId;
        private DateTime _curEndDate;
        private DateTime _curStartDate;
        private bool _confirmLetter = false;
        private bool _returnLetter = false;


        //
        // BL public properties
        //
        private int _countryDDLeadTime;
        public int countryDDLeadTime
        {
            get { return _countryDDLeadTime; }
        }
        private int _countryMinPeriod;
        public int countryMinPeriod
        {
            get { return _countryMinPeriod; }
        }
        private int _countryMaxRejections;
        public int countryMaxRejections
        {
            get { return _countryMaxRejections; }
        }
        private decimal _countryFee;
        public decimal countryFee
        {
            get { return _countryFee; }
        }
        private DateTime _runDate = DateTime.MinValue;
        public DateTime runDate
        {
            get { return _runDate; }
        }
        private DateTime _today;
        public DateTime today
        {
            get { return _today; }
        }
        private DateTime _effDate;
        public DateTime effectiveDate
        {
            get { return _effDate; }
        }
        private int _effDay;
        public int effectiveDay
        {
            get { return _effDay; }
        }
        private DateTime _nextDueDate;
        public DateTime nextDueDate
        {
            get { return _nextDueDate; }
        }
        private int _nextDueDayId;
        public int nextDueDayId
        {
            get { return _nextDueDayId; }
        }
        private short _nextDueDay;
        public short nextDueDay
        {
            get { return _nextDueDay; }
        }
        private string _fileNameDate;
        public string fileNameDate
        {
            get { return _fileNameDate; }
        }


        private DDDMandate _ddmandate = new DDDMandate();
        private DataSet _giroDateSet = new DataSet();


        //
        // BL Methods
        //

        public BDDMandate()
        {

        }

        public BDDMandate(string countryCode, DateTime piRunDate)
        {
            DCountry country = new DCountry();
            country.GetDefaults(countryCode);
            this._countryDDLeadTime = (short)country.Table.Rows[0]["ddleadtime"];
            this._countryMinPeriod = (short)country.Table.Rows[0]["minperiod"];
            this._countryMaxRejections = (short)country.Table.Rows[0]["ddmaxrejections"];
            this._countryFee = (decimal)country.Table.Rows[0]["ddfee"];
            // Set up various DD dates as of today
            this.InitDates(piRunDate);
        }


        public void CancelMandates(SqlConnection conn, SqlTransaction trans)
        {
            // Cancel mandates not approved or with a cancelled rejection
            DDDMandate eodMandate = new DDDMandate();
            eodMandate.CancelMandates(conn, trans, this._today, this.effectiveDate, this._nextDueDate, this._countryMaxRejections);
        }


        public DataSet ConfirmMandateList(SqlConnection conn, SqlTransaction trans)
        {
            // Retrieve a list of approved mandates with a delivery date that are ready to start
            DDDMandate eodMandate = new DDDMandate();
            DataTable mandateList = eodMandate.ConfirmMandateList(conn, trans, this.effectiveDate);
            DataSet mandateSet = new DataSet();
            mandateSet.Tables.Add(mandateList);
            return mandateSet;
        }


        public void ConfirmMandate(SqlConnection conn, SqlTransaction trans, string acctNo, int mandateId, DateTime approvalDate, int dueDay)
        {
            // Start a mandate with an Approval Date and a Date Delivered
            // Each approved mandate with a delivery date must have Date First set up for Giro
            DDDMandate eodMandate = new DDDMandate();
            DateTime dateFirst = eodMandate.GiroDateFirst(conn, trans, acctNo, this.effectiveDate, this._countryMinPeriod);

            // If Date First has been set successfully then set up the mandate Start Date.
            // Note that Date First is always calculated after the Delivery Date. The mandate
            // Start Date will be on/after all of the Date First, the mandate Approval Date
            // and the next earliest bank date.
            if (dateFirst > Date.blankDate)
            {
                eodMandate.ConfirmMandate(conn, trans,
                    this._today, this.effectiveDate, acctNo, mandateId, approvalDate, dueDay, dateFirst);
            }
        }


        public DataSet Get(string accountNo, out int rowCount)
        {
            rowCount = _ddmandate.Get(accountNo);
            return this.ReturnProperties(rowCount);
        }


        public DataSet Get(int mandateId, out int rowCount)
        {
            rowCount = _ddmandate.Get(mandateId);
            return this.ReturnProperties(rowCount);
        }


        private DataSet ReturnProperties(int rowCount)
        {
            DataSet mandateSet = null;

            this._acctLoaded = false;	// Customer Account details loaded
            this._loaded = false;       // Mandate details loaded

            if (rowCount > 0)
            {
                // Get OK - copy properties
                this._acctLoaded = true;
                this._loaded = (_ddmandate.mandateId != 0);
                this._mandateId = _ddmandate.mandateId;
                this._acctNo = _ddmandate.acctNo;
                this._approvalDate = _ddmandate.approvalDate;
                this._bankAcctName = _ddmandate.bankAcctName;
                this._bankAcctNo = _ddmandate.bankAcctNo;
                this._bankBranchNo = _ddmandate.bankBranchNo;
                this._bankCode = _ddmandate.bankCode;
                this._cancelReason = _ddmandate.cancelReason;
                this._changedBy = _ddmandate.changedBy;
                this._comment = _ddmandate.comment;
                this._customerName = _ddmandate.customerName;
                this._dueDayId = _ddmandate.dueDayId;
                this._endDate = _ddmandate.endDate;
                this._noFees = _ddmandate.noFees;
                this._receiveDate = _ddmandate.receiveDate;
                this._rejectCount = _ddmandate.rejectCount;
                this._returnDate = _ddmandate.returnDate;
                this._startDate = _ddmandate.startDate;
                this._status = _ddmandate.status;
                this._submitDate = _ddmandate.submitDate;
                this._customerName = _ddmandate.customerName;
                this._instalAmount = _ddmandate.instalAmount;

                // Translate the cancel reason
                switch (this._cancelReason)
                {
                    case DDCancel.NotApproved:
                        this._cancelDesc = "Not approved by second due date";
                        break;
                    case DDCancel.TwoRejections:
                        this._cancelDesc = "Two consecutive rejections received";
                        break;
                    case DDCancel.MaxRejections:
                        this._cancelDesc = "Maximum number of rejections received";
                        break;
                    case DDCancel.CancelledReject:
                        this._cancelDesc = "Cancelled by user after rejection";
                        break;
                    case DDCancel.UserCancelled:
                        this._cancelDesc = "User cancelled";
                        break;
                    case DDCancel.Settled:
                        this._cancelDesc = "Account settled";
                        break;
                    default:
                        this._cancelDesc = "";
                        break;
                }

                // Translate whether the mandate is Active, Terminated, Pending or Deleted
                if (this._status != DDStatus.Deleted)
                {
                    // Set the status as Active, Terminated or Pending
                    this.InitDates(System.DateTime.Today);

                    if (this._startDate != Date.blankDate && this._startDate <= this._effDate
                        && (this._endDate == Date.blankDate || this._endDate > this._effDate))
                    {
                        this._status = DDStatus.Active;
                    }
                    else if (this._endDate != Date.blankDate && this._endDate <= this._effDate)
                    {
                        this._status = DDStatus.Terminated;
                    }
                    else
                    {
                        this._status = DDStatus.Pending;
                    }
                }

                switch (this._status)
                {
                    case DDStatus.Active:
                        this._activeText = GetResource("T_MANDATEACTIVE");
                        this._activeTextColour = Color.Red;
                        break;
                    case DDStatus.Terminated:
                        this._activeText = GetResource("T_MANDATETERMINATED");
                        this._activeTextColour = Color.Green;
                        break;
                    case DDStatus.Deleted:
                        this._activeText = GetResource("T_MANDATEDELETED");
                        this._activeTextColour = Color.Red;
                        break;
                    default:
                        this._activeText = "";
                        break;
                }

            }

            // Private properties to record changes
            this.PrivateInit();

            // Return as a DataSet (required even if no row retrieved)

            DataTable details = new DataTable(TN.DDMandate);
            details.Columns.AddRange(new DataColumn[]	{   new DataColumn(CN.MandateId, System.Type.GetType("System.Int32")),
															new DataColumn(CN.AcctNo),
															new DataColumn(CN.ApprovalDate,Type.GetType("System.DateTime")),
															new DataColumn(CN.BankAccountName),
															new DataColumn(CN.BankAccountNo),
															new DataColumn(CN.BankBranchNo),
															new DataColumn(CN.BankCode),
															new DataColumn(CN.CancelReason),
															new DataColumn(CN.ChangedBy),
															new DataColumn(CN.Comment),
															new DataColumn(CN.DueDayId, System.Type.GetType("System.Int32")),
															new DataColumn(CN.EndDate,Type.GetType("System.DateTime")),
															new DataColumn(CN.NoFees, System.Type.GetType("System.Boolean")),
															new DataColumn(CN.ReceiveDate,Type.GetType("System.DateTime")),
															new DataColumn(CN.RejectCount, System.Type.GetType("System.Int32")),
															new DataColumn(CN.ReturnDate,Type.GetType("System.DateTime")),
															new DataColumn(CN.StartDate,Type.GetType("System.DateTime")),
															new DataColumn(CN.Status),
															new DataColumn(CN.SubmitDate,Type.GetType("System.DateTime")),
															new DataColumn(CN.CustomerName),
															new DataColumn(CN.InstalAmount, System.Type.GetType("System.Decimal")),
															new DataColumn(CN.AcctLoaded, System.Type.GetType("System.Boolean")),
															new DataColumn(CN.Loaded, System.Type.GetType("System.Boolean")),
															new DataColumn(CN.ActiveText),
															new DataColumn(CN.ActiveTextColor),
															new DataColumn(CN.CancelDesc),
															new DataColumn(CN.CurAcctNo),
															new DataColumn(CN.CurBankAcctName),
															new DataColumn(CN.CurBankAcctNo),
															new DataColumn(CN.CurBankBranchNo),
															new DataColumn(CN.CurBankCode),
															new DataColumn(CN.CurDueDayId, System.Type.GetType("System.Int32")),
															new DataColumn(CN.CurEndDate,Type.GetType("System.DateTime")),
															new DataColumn(CN.CurStartDate,Type.GetType("System.DateTime")),
															new DataColumn(CN.ConfirmLetter, System.Type.GetType("System.Boolean")),
															new DataColumn(CN.ReturnLetter, System.Type.GetType("System.Boolean")) });

            DataRow row = details.NewRow();

            row[CN.MandateId] = _mandateId;
            row[CN.AcctNo] = _acctNo;
            row[CN.ApprovalDate] = _approvalDate;
            row[CN.BankAccountName] = _bankAcctName;
            row[CN.BankAccountNo] = _bankAcctNo;
            row[CN.BankBranchNo] = _bankBranchNo;
            row[CN.BankCode] = _bankCode;
            row[CN.CancelReason] = _cancelReason;
            row[CN.ChangedBy] = _changedBy;
            row[CN.Comment] = _comment;
            row[CN.DueDayId] = _dueDayId;
            row[CN.EndDate] = _endDate;
            row[CN.NoFees] = _noFees;
            row[CN.ReceiveDate] = _receiveDate;
            row[CN.RejectCount] = _rejectCount;
            row[CN.ReturnDate] = _returnDate;
            row[CN.StartDate] = _startDate;
            row[CN.Status] = _status;
            row[CN.SubmitDate] = _submitDate;
            row[CN.CustomerName] = _customerName;
            row[CN.InstalAmount] = _instalAmount;

            row[CN.AcctLoaded] = _acctLoaded;
            row[CN.Loaded] = _loaded;
            row[CN.ActiveText] = _activeText;
            row[CN.ActiveTextColor] = _activeTextColour;
            row[CN.CancelDesc] = _cancelDesc;
            row[CN.CurAcctNo] = _curAcctNo;
            row[CN.CurBankAcctName] = _curBankAcctName;
            row[CN.CurBankAcctNo] = _curBankAcctNo;
            row[CN.CurBankBranchNo] = _curBankBranchNo;
            row[CN.CurBankCode] = _curBankCode;
            row[CN.CurDueDayId] = _curDueDayId;
            row[CN.CurEndDate] = _curEndDate;
            row[CN.CurStartDate] = _curStartDate;
            row[CN.ConfirmLetter] = _confirmLetter;
            row[CN.ReturnLetter] = _returnLetter;

            details.Rows.Add(row);

            mandateSet = new DataSet();
            mandateSet.Tables.Add(details);

            return mandateSet;
        }


        /**************************************************************/
        /* Calc the effective date and retrieve the minimum period    */
        /**************************************************************/
        public DataSet InitDates(DateTime piRunDate)
        {
            /* Check whether the object has already been initialised */
            if (this._init && this._runDate == piRunDate) return this._giroDateSet;

            /* Work out the next effective date as today + lead time, and 
            ** retrieve the minimum period of grace.
            */
            System.TimeSpan duration = new System.TimeSpan(this._countryDDLeadTime, 0, 0, 0);
            if (piRunDate > Date.blankDate)
            {
                this._today = piRunDate;
            }
            else
            {
                this._today = System.DateTime.Today;
            }
            this._effDate = this._today.Add(duration);
            this._effDay = this._effDate.Day;

            // Get the next Due Date
            DDDDueDay nextDueDay = new DDDDueDay();
            this._nextDueDate = nextDueDay.NextDueDate(this._effDate, out this._nextDueDayId, out this._nextDueDay);

            // Set up the filename date string as 'yymmdd' from the due date.
            this._fileNameDate =
                this._nextDueDate.Year.ToString().Substring(2, 2) +
                this._nextDueDate.Month.ToString().PadLeft(2, '0') +
                this._nextDueDate.Day.ToString().PadLeft(2, '0');

            // Provide these values as a DataSet
            DataRow row = null;
            if (_giroDateSet.Tables.Contains(TN.DDGiroDates))
            {
                row = _giroDateSet.Tables[TN.DDGiroDates].Rows[0];
            }
            else
            {
                DataTable details = new DataTable(TN.DDGiroDates);
                details.Columns.AddRange(new DataColumn[]	{   new DataColumn(CN.EffDate,		System.Type.GetType("System.DateTime")),
																new DataColumn(CN.EffDay,		System.Type.GetType("System.Int32")),
																new DataColumn(CN.MinPeriod,	System.Type.GetType("System.Int32")),
																new DataColumn(CN.DueDate,		System.Type.GetType("System.DateTime")),
																new DataColumn(CN.DueDayId,		System.Type.GetType("System.Int32")),
																new DataColumn(CN.DueDay,		System.Type.GetType("System.Int16")),
																new DataColumn(CN.FileNameDate,	System.Type.GetType("System.String"))});
                row = details.NewRow();
                details.Rows.Add(row);
                _giroDateSet.Tables.Add(details);
            }

            row[CN.EffDate] = this._effDate;
            row[CN.EffDay] = this._effDay;
            row[CN.MinPeriod] = this._countryMinPeriod;
            row[CN.DueDate] = this._nextDueDate;
            row[CN.DueDayId] = this._nextDueDayId;
            row[CN.DueDay] = this._nextDueDay;
            row[CN.FileNameDate] = this._fileNameDate;

            this._init = true;
            this._runDate = piRunDate;
            return this._giroDateSet;
        }     /* End of InitDates */


        private void PrivateInit()
        {
            // A history record will be required if the user changes
            // any of these fields on an active mandate.
            this._curAcctNo = this.acctNo;
            this._curBankAcctName = this.bankAcctName;
            this._curBankAcctNo = this.bankAcctNo;
            this._curBankBranchNo = this.bankBranchNo;
            this._curBankCode = this.bankCode;
            this._curDueDayId = this.dueDayId;
            this._curEndDate = this.endDate;
            this._curStartDate = this.startDate;
        }


        public int AnotherMandate(int mandateId, string accountNo, out int anMandateId,
            out DateTime startDate, out DateTime endDate, out DateTime effDate)
        {
            this.InitDates(System.DateTime.Today);
            effDate = this._effDate;
            return this._ddmandate.AnotherMandate(mandateId, accountNo, out anMandateId,
                out startDate, out endDate);
        }


        public DataSet SaveMandate(SqlConnection conn, SqlTransaction trans,
            bool addNewRecord, DataSet mandateSet)
        {
            DataRow mandateDetails = mandateSet.Tables[TN.DDMandate].Rows[0];

            // BL properties
            this._mandateId = (int)mandateDetails[CN.MandateId];
            this._acctNo = (string)mandateDetails[CN.AcctNo];
            this._approvalDate = (DateTime)mandateDetails[CN.ApprovalDate];
            this._bankAcctName = (string)mandateDetails[CN.BankAccountName];
            this._bankAcctNo = (string)mandateDetails[CN.BankAccountNo];
            this._bankBranchNo = (string)mandateDetails[CN.BankBranchNo];
            this._bankCode = (string)mandateDetails[CN.BankCode];
            this._cancelReason = (string)mandateDetails[CN.CancelReason];
            this._comment = (string)mandateDetails[CN.Comment];
            this._dueDayId = (int)mandateDetails[CN.DueDayId];
            this._endDate = (DateTime)mandateDetails[CN.EndDate];
            this._noFees = (bool)mandateDetails[CN.NoFees];
            this._receiveDate = (DateTime)mandateDetails[CN.ReceiveDate];
            this._rejectCount = (int)mandateDetails[CN.RejectCount];
            this._returnDate = (DateTime)mandateDetails[CN.ReturnDate];
            this._startDate = (DateTime)mandateDetails[CN.StartDate];
            this._status = (string)mandateDetails[CN.Status];
            this._submitDate = (DateTime)mandateDetails[CN.SubmitDate];
            this._customerName = (string)mandateDetails[CN.CustomerName];
            this._instalAmount = (decimal)mandateDetails[CN.InstalAmount];

            this._curAcctNo = (string)mandateDetails[CN.CurAcctNo];
            this._curBankAcctName = (string)mandateDetails[CN.CurBankAcctName];
            this._curBankAcctNo = (string)mandateDetails[CN.CurBankAcctNo];
            this._curBankBranchNo = (string)mandateDetails[CN.CurBankBranchNo];
            this._curBankCode = (string)mandateDetails[CN.CurBankCode];
            this._curDueDayId = (int)mandateDetails[CN.CurDueDayId];
            this._curEndDate = (DateTime)mandateDetails[CN.CurEndDate];
            this._curStartDate = (DateTime)mandateDetails[CN.CurStartDate];
            this._confirmLetter = (bool)mandateDetails[CN.ConfirmLetter];
            this._returnLetter = (bool)mandateDetails[CN.ReturnLetter];

            // DA properties
            this._ddmandate.mandateId = this._mandateId;
            this._ddmandate.acctNo = this._acctNo;
            this._ddmandate.approvalDate = this._approvalDate;
            this._ddmandate.bankAcctName = this._bankAcctName;
            this._ddmandate.bankAcctNo = this._bankAcctNo;
            this._ddmandate.bankBranchNo = this._bankBranchNo;
            this._ddmandate.cancelReason = this._cancelReason;
            this._ddmandate.changedBy = this._changedBy;
            this._ddmandate.comment = this._comment;
            this._ddmandate.endDate = this._endDate;
            this._ddmandate.noFees = this._noFees;
            this._ddmandate.receiveDate = this._receiveDate;
            this._ddmandate.rejectCount = this._rejectCount;
            this._ddmandate.returnDate = this._returnDate;
            this._ddmandate.startDate = this._startDate;
            this._ddmandate.submitDate = this._submitDate;
            this._ddmandate.dueDayId = this._dueDayId;
            this._ddmandate.bankCode = this._bankCode;
            this._ddmandate.customerName = this._customerName;
            this._ddmandate.instalAmount = this._instalAmount;

            /* ------------------------- START OF MST ------------------------- */

            /* Change/add the Return Letter if it was requested */
            if (this._returnLetter)
            {
                /* Construct the Return letter */
                BLetter returnLetter = new BLetter();
                returnLetter.Write(conn, trans, this._acctNo, this._returnDate, this._returnDate, DDLetter.Return, 0, "0");
            }

            else if (this._endDate != this._curEndDate)
            {
                /* Construct the Cancellation letter.
                ** The letter is generated here instead of Daily Housekeeping, so that
                ** a new letter will be generated each time the end date is changed.
                */
                BLetter cancelLetter = new BLetter();
                cancelLetter.Write(conn, trans, this._acctNo, System.DateTime.Today, this._endDate, DDLetter.Cancel, 0, "0");
            }

            /* Save details */
            this._loaded = false;
            if (this._mandateId == 0 || addNewRecord)
            {
                /* Call NewMandate to create a new record or a history record */

                this._ddmandate.NewMandate(conn, trans);
            }
            else
            {
                this._ddmandate.UpdateMandate(conn, trans);
            }


            /* HEAT 113592 Check in case Date First should be recalculated for Giro.
            ** (This supersedes FR1312)
            */
            //	TODO:		Status = this.GiroDateFirst(pi_AcctNo = this.AcctNo);


            /* ------------------------- END OF MST ------------------------- */

            // Some non-displayed properties have been updated
            // so return all the properties back to the PL
            return this.ReturnProperties(1);

        }     /* End of SaveMandate */

        public decimal GetGiroPending(string accountNo)
        {
            decimal pending = 0;
            pending = this._ddmandate.GetGiroPending(accountNo);

            return pending;
        }
    }
}

