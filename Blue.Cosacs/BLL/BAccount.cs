using System;
using STL.DAL;
using STL.Common;
using System.Data;
using System.Xml;
using System.Data.SqlClient;
using STL.Common.Constants.Categories;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.CustomerTypes;
using STL.Common.Constants.Enums;
using STL.Common.Constants.Tags;
using STL.Common.Constants.ItemTypes;
using STL.Common.Constants.Elements;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using System.Collections.Specialized;
using STL.Common.Constants.FTransaction;
using STL.Common.Constants.Delivery;
using STL.Common.Constants.SanctionStages;
using STL.Common.Constants.TermsTypes;
using STL.Common.Constants.EOD;
using System.Collections;
using System.IO;
using System.Configuration;
using STL.Common.Constants.AuditSource;
using STL.Common.Structs;
using STL.Common.Static;
using STL.Common.Constants.InstantCredit;
using Blue.Cosacs.Shared;
using Blue.Cosacs.Repositories;
using STL.Common.Constants.StoreCard;
using Blue.Cosacs.Shared.Services.StoreCard;   //IP - 09/05/11 - Store Card - Feature - #3004

using STL.Common.Constants.StoreCard;   //IP - 09/05/11 - Store Card - Feature - #3004
using System.Linq;
using Blue.Cosacs;
using System.Collections.Generic;
using STL.Common.Constants.CashLoans;
using Blue.Cosacs.BLL.WebApi;
using STL.BLL.WebApi;


namespace STL.BLL
{
    /// <summary>
    /// Business logic component for account functions
    /// </summary>

    public class BAccount : CommonObject
    {
        private DataTable tab;
        private DAccount data;
        private string _newAccountNo = "000000000000";
        private int mod11 = 11;
        private int[] array1;
        private int[] array2;
        private int hiAllowed;
        private int hiAllocated;
        private string acctInd;
        private short _branchNo;
        private string _accountType;
        private bool created = false;
        private DAccountNumber acctNo;
        private BAgreement _agreement;
        private XmlUtilities xml;
        private DataTable dtItems = new DataTable();
        private short _origBr = 0;
        public short OrigBr
        {
            get { return _origBr; }
            set { _origBr = value; }
        }

        private string _accountNo = "";
        public string AccountNumber
        {
            get { return _accountNo; }
            set { _accountNo = value; }
        }
        private string _recordCountt = "";
        public string RecordCount
        {
            get { return _recordCountt; }
            set { _recordCountt = value; }
        }

        private string _customerID = "";
        public string CustomerID
        {
            get { return _customerID; }
            set { _customerID = value; }
        }

        private DateTime _dateAccountOpen = DateTime.MinValue.AddYears(1899);
        public DateTime DateAccountOpen
        {
            get { return _dateAccountOpen; }
            set { _dateAccountOpen = value; }
        }
        // jec 26/01/07 dueday
        private short _dueday = 0;
        public short DueDay
        {
            get { return _dueday; }
            set { _dueday = value; }
        }

        private decimal _as400bal = 0;
        public decimal AS400Bal
        {
            get { return _as400bal; }
            set { _as400bal = value; }
        }

        private short _paidpcent = 0;
        public short PaidPcent
        {
            get { return _paidpcent; }
            set { _paidpcent = value; }
        }

        private string _termsType = "";
        public string TermsType
        {
            get { return _termsType; }
            set { _termsType = value; }
        }

        private decimal _repossarrears = 0;
        public decimal RepossArrears
        {
            get { return _repossarrears; }
            set { _repossarrears = value; }
        }

        private decimal _repossvalue = 0;
        public decimal RepossValue
        {
            get { return _repossvalue; }
            set { _repossvalue = value; }
        }
        private decimal _bdwbalance = 0;
        public decimal BDWBalance
        {
            get { return _bdwbalance; }
            set { _bdwbalance = value; }
        }

        private decimal _bdwcharges = 0;
        public decimal BDWCharges
        {
            get { return _bdwcharges; }
            set { _bdwcharges = value; }
        }

        private string _securitised = "";
        public string Securitised
        {
            get { return _securitised; }
            set { _securitised = value; }
        }


        private DateTime _dateIntoArrears = DateTime.MinValue.AddYears(1899);
        public DateTime DateIntoArrears
        {
            get { return _dateIntoArrears; }
            set { _dateIntoArrears = value; }
        }

        public BAgreement Agreement
        {
            get
            {
                return _agreement;
            }
            set
            {
                _agreement = value;
            }
        }
        private BInstalPlan _instalPlan;
        public BInstalPlan InstalPlan
        {
            get
            {
                return _instalPlan;
            }
            set
            {
                _instalPlan = value;
            }
        }
        private string accountStatus = "";
        private string viewLimit = "";

        private decimal _outbal = 0;
        public decimal OutstandingBalance
        {
            get
            {
                return _outbal;
            }
            set
            {
                // value is 4 decimal places in db but when doing a bad debt write off
                // las two decimal places remain
                _outbal = Decimal.Round(value, 2);
            }
        }
        private string _currStatus = "";
        public string CurrentStatus
        {
            get
            {
                return _currStatus;
            }
            set
            {
                _currStatus = value;
            }
        }
        private short _creditDays = 0;
        public short CreditDays
        {
            get
            {
                return _creditDays;
            }
            set
            {
                _creditDays = value;
            }
        }
        private DateTime _dateLastPaid = DateTime.MinValue.AddYears(1899);
        public DateTime DateLastPaid
        {
            get
            {
                return _dateLastPaid;
            }
            set
            {
                _dateLastPaid = value;
            }
        }
        private decimal _arrears = 0;
        public decimal Arrears
        {
            get
            {
                return _arrears;
            }
            set
            {
                _arrears = value;
            }
        }
        private string _highestStatus = "1";
        public string HighestStatus
        {
            get
            {
                return _highestStatus;
            }
            set
            {
                _highestStatus = value;
            }
        }
        private short _highestStatusDays = 0;
        public short HighestStatusDays
        {
            get
            {
                return _highestStatusDays;
            }
            set
            {
                _highestStatusDays = value;
            }
        }
        public short BranchNo
        {
            get
            {
                return _branchNo;
            }
            set
            {
                _branchNo = value;
            }
        }
        public string AccountType
        {
            get
            {
                return _accountType;
            }
            set
            {
                _accountType = value;
            }
        }
        private decimal _agreementTotal = 0;
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
        private string _country = "";
        public string CountryCode
        {
            get { return _country; }
            set { _country = value; }
        }

        private bool _hasLineItems = false;
        public bool HasLineItems
        {
            get { return _hasLineItems; }
            set { _hasLineItems = value; }
        }

        private bool _reOpenS1 = false;
        public bool ReOpenS1
        {
            get { return _reOpenS1; }
            set { _reOpenS1 = value; }
        }

        private bool _reOpenS2 = false;
        public bool ReOpenS2
        {
            get { return _reOpenS2; }
            set { _reOpenS2 = value; }
        }

        //IP - 09/05/11 - Store Card - Feature - #3004 - will be used to indicate where Baccount was called from.
        private string _source = string.Empty;
        public string Source
        {
            get { return _source; }
            set { _source = value; }
        }

        public BAccount()
        {
            xml = new XmlUtilities();
        }

        public string NewAccountNo
        {
            get
            {
                if (_newAccountNo != null)
                    return _newAccountNo;
                else
                    return String.Empty;
            }
        }

        private bool _notifyReplacement = false;
        public bool NotifyReplacement
        {
            get { return _notifyReplacement; }
            set { _notifyReplacement = value; }
        }

        private decimal _clamt = 0;
        public decimal ClAmt
        {
            get
            {
                return _clamt;
            }
            set
            {
                _clamt = value;
            }
        }


        private void FormatAccountNo(ref string unformatted)
        {
            int i = 0;
            string temp = null;
            foreach (char letter in unformatted)
            {
                temp += letter;
                switch (i)
                {
                    case 2:
                        temp += "-";
                        break;
                    case 6:
                        temp += "-";
                        break;
                    case 10:
                        temp += "-";
                        break;
                    default: break;
                }
                i++;
            }
            unformatted = temp;
        }

        public DataSet GetTransactionDetails(int FinTransId)
        {
            DataSet ds = new DataSet();
            DBSPaymentDetails da = new DBSPaymentDetails();
            ds = da.GetTransactionDetails(FinTransId);
            return ds;
        }

        public XmlNode GenerateAccountNumber(SqlConnection conn, SqlTransaction trans, string countryCode, short branchNumber, string accountType, bool manualSale, out string newAccountNo)
        {
            _branchNo = branchNumber;
            _accountType = accountType;
            newAccountNo = "000-0000-0000-0";
            XmlNode stampDuty = null;

            countryCode = countryCode.Trim();

            if (!manualSale)
            {
                array1 = new int[11];
                array2 = new int[12];

                if ((countryCode == "A" ||
                    countryCode == "M" ||
                    countryCode == "N" ||
                    countryCode == "P") &&
                    AT.IsCreditType(accountType))
                {					//HP weightings
                    array1[0] = 9;
                    array1[1] = 8;
                    array1[2] = 4;
                    array1[3] = 10;
                    array1[4] = 7;
                    array1[5] = 1;
                    array1[6] = 6;
                    array1[7] = 3;
                    array1[8] = 5;
                    array1[9] = 2;
                    array1[10] = 1;
                }
                else
                {					//Option weightings
                    array1[0] = 9;
                    array1[1] = 8;
                    array1[2] = 4;
                    array1[3] = 10;
                    array1[4] = 1;
                    array1[5] = 6;
                    array1[6] = 3;
                    array1[7] = 5;
                    array1[8] = 2;
                    array1[9] = 7;
                    array1[10] = 1;
                }

                acctNo = new DAccountNumber();
                if (acctNo.GetAccountControl(conn, trans, countryCode, branchNumber, accountType) == (int)Return.Success)
                {
                    DataTable dt = acctNo.AccountControlTable;

                    //loop through the records. Usually only one
                    foreach (DataRow row in dt.Rows)
                    {
                        hiAllocated = (int)row["hiallocated"];
                        hiAllowed = (int)row["hiallowed"];
                        acctInd = (string)row["acctind"];



                        if (++hiAllocated >= hiAllowed)
                        {
                            new AccountRepository().ResetAcctnos(conn, trans, branchNumber, accountType);
                            hiAllocated = 1;
                        }


                        //increment hiAllocated trying each time to create
                        //an account number based on it.
                        while (hiAllocated++ < hiAllowed)
                        {
                            //update first so that we don't clash with other
                            //users
                            acctNo.UpdateAccountControl(conn, trans, branchNumber, accountType, hiAllocated, hiAllowed);
                            if (createNumber())
                            {
                                //When an account number has been successfully 
                                //created break from the loop
                                created = true;
                                break;
                            }
                        }
                        if (created)
                        {
                            //No need to carry on looking
                            break;
                        }
                    }
                }
            }
            else
                created = true;

            if (created)
            {
                //Format the account number i.e. add hyphens
                FormatAccountNo(ref _newAccountNo);
                newAccountNo = _newAccountNo;

                //create stamp duty line items if necessary
                if (!(bool)Country[CountryParameterNames.CL_Amortized])
                    stampDuty = CreateStampDutyItems(conn, trans, branchNumber, countryCode, accountType);
            }
            return stampDuty;
        }

        private XmlNode CreateStampDutyItems(SqlConnection conn, SqlTransaction trans, short branch, string country, string accountType)
        {
            XmlDocument doc = new XmlDocument();
            XmlUtilities xml = new XmlUtilities();
            XmlNode itemNode = null;
            XmlNode stampDuty = null;

            DSundryCharge sundry = new DSundryCharge();
            BItem item = new BItem();
            sundry.GetSundryChargeItem(conn, trans, accountType);
            DataTable dt = sundry.Items;
            if (dt.Rows.Count > 0)
            {
                stampDuty = doc.CreateElement("ITEMS");
                foreach (DataRow row in dt.Rows)
                {
                    var itemId = Convert.ToInt32(row[CN.ItemId]);
                    itemNode = item.GetItemDetails(conn, trans, itemId, branch, accountType, country, false, false);     // CR1212 jec need to supply itemID not zero
                    itemNode.Attributes[Tags.Quantity].Value = "1";
                    itemNode.Attributes[Tags.Value].Value = itemNode.Attributes[Tags.UnitPrice].Value;
                    itemNode.Attributes[Tags.Type].Value = IT.SundryCharge;
                    itemNode.Attributes[Tags.DeliveryDate].Value = DateTime.MinValue.AddYears(1899).ToString();
                    itemNode.Attributes[Tags.BranchForDeliveryNote].Value = branch.ToString();
                    itemNode.Attributes[Tags.ItemId].Value = Convert.ToString(itemId);                                   //IP - 19/05/11 - CR1212 - setting the ItemID for StampDuty in xml                                                            
                    if (itemNode != null)
                    {
                        itemNode = doc.ImportNode(itemNode, true);
                        stampDuty.AppendChild(itemNode);
                    }
                }
            }
            return stampDuty;
        }

        /// <summary>
        /// Attempts to create a new account number using the 
        /// current allocated number
        /// </summary>
        /// <returns>success or failure</returns>
        private bool createNumber()
        {
            bool succeed = false;
            string branchStr = Convert.ToString(_branchNo);
            string hiAllocStr = Convert.ToString(hiAllocated);
            for (int i = 0; i < array2.Length; i++)
                array2[i] = 0;

            for (int i = branchStr.Length; i > 0; i--)
            {
                array2[i - 1] = Convert.ToInt32(branchStr.Substring(i - 1, 1));
            }

            array2[3] = Convert.ToInt32(acctInd);

            //put hiAllocated in position 
            int y = 9;
            for (int i = hiAllocStr.Length; i > 0; i--, y--)
            {
                array2[y] = Convert.ToInt32(hiAllocStr.Substring(i - 1, 1));
            }

            //Calculate the check digit
            int modCalc = 0;
            for (int i = 0; i < 10; i++)
            {
                modCalc += array1[i] * array2[i];
            }

            modCalc = mod11 - (modCalc % mod11);

            if (modCalc != 10)
            {
                if (modCalc == 11)
                    modCalc = 0;
                array2[10] = modCalc;
                if (AT.IsCashType(_accountType))
                    array2[11] = 0;
                else
                    array2[11] = 1;

                _newAccountNo = "";
                foreach (int letter in array2)
                {
                    _newAccountNo += Convert.ToString(letter);
                }
                //Check to see if the account already exists
                if (!Convert.ToBoolean(acctNo.IsDuplicate(_newAccountNo)))
                {
                    succeed = true;
                }
            }
            return succeed;
        }

        public void Lock(SqlConnection conn, SqlTransaction trans, string accountNo, int user)
        {
            DAccount acct = new DAccount();
            acct.Lock(conn, trans, accountNo, user);
        }

        public void Lock(SqlConnection conn, SqlTransaction trans, string accountNo, int user, bool revise)
        {
            DAccount acct = new DAccount();
            acct.Lock(conn, trans, accountNo, user);

            //69650 Lock the account for a Goods Return
            if (revise)
            {
                acct.LockAccountForGoodsReturn(conn, trans, accountNo, user);
            }
        }

        /* Issue 69238 - SC 3/9/07
                 * Get agreement number so cash and go accounts with agreement numbers greater than 1
                 * line items can be viewed. */

        public int GetAgreementNo(SqlConnection conn, SqlTransaction trans, string acctno)
        {
            DAccount acct = new DAccount();
            return acct.GetAgreementNo(conn, trans, acctno);
        }

        public string ArchiveAccounts()
        {
            DAccount acct = new DAccount();
            int result = acct.ArchiveAccounts();

            if (result == 0)
                return "P";
            else
                return "F";

        }
        /*
        public void ValidLock(string accountNo, int user)
        {
            DAccount acct = new DAccount();
            acct.ValidLock(accountNo, user);
        }
        */

        public void ValidLock(SqlConnection conn, SqlTransaction trans, string accountNo, int user)
        {
            DAccount acct = new DAccount();
            acct.ValidLock(conn, trans, accountNo, user);
        }

        public void Unlock(SqlConnection conn, SqlTransaction trans, string accountNo, int user)
        {
            DAccount acct = new DAccount();
            acct.Unlock(conn, trans, accountNo, user);
        }

        public void Unlock(SqlConnection conn, SqlTransaction trans, string accountNo, bool revise, int user)
        {
            DAccount acct = new DAccount();
            acct.Unlock(conn, trans, accountNo, user);

            //69650 Unlock the account for a Goods Return
            if (revise)
            {
                acct.UnlockAccountForGoodsReturn(conn, trans, accountNo, user);
            }
        }

        public void ValidateSaleForCINT(string sku, int quantity, int salelocation, int stocklocation, DateTime transactiondate, out string ErrorMessage, out int ErrorCount)
        {
            //Changes to validate itemno at the point of sale so that no poisons are generated later
            DAccount acct = new DAccount();
            ErrorMessage = "";
            ErrorCount = -1;
            acct.ValidateSaleForCINT(sku, quantity, salelocation, stocklocation, transactiondate, out ErrorMessage, out ErrorCount);

        }

        /// <summary>
        /// Property returning a dataset containing all 
        /// instant credit accounts awaiting clearance
        /// </summary>
        private DataSet InstantCreditAwaitingClearance
        {
            get
            {
                DataSet ds = new DataSet();
                //We need to create a new table displaying only the columns we
                //want. There's got to be a better way.
                tab = new DataTable();
                tab.Columns.Add("Account");
                tab.Columns.Add("Type");
                tab.Columns.Add("Terms");
                tab.Columns.Add("Date Opened", Type.GetType("System.DateTime"));
                tab.Columns.Add("Sales Person");
                tab.Columns.Add("Last Updated", Type.GetType("System.DateTime"));
                tab.Columns.Add("By");
                tab.Columns.Add("ChqOS");
                tab.Columns.Add("PayOS");
                tab.Columns.Add("InstantCredit");
                tab.Columns.Add("CustId");
                tab.Columns.Add("DateProp", Type.GetType("System.DateTime"));
                tab.Columns.Add("Name");
                tab.Columns.Add("SubAgreement");
                tab.Columns.Add("FOC"); //IP/JC - 16/02/10 - CR1048 4.10 (REF:3.1.36) - FOC - CR1072 

                DataRow newRow;
                foreach (DataRow row in data.InstantCreditAwaitingClearance.Rows) //IP - 17/03/11 - #3340 - Changed the below to look at the column heading rather than the position.
                {
                    newRow = tab.NewRow();
                    newRow["Account"] = row["AcctNo"];
                    newRow["Type"] = row["AcctType"];
                    newRow["Terms"] = row["termstype"];
                    newRow["Date Opened"] = row["DateAcctOpen"];
                    newRow["Sales Person"] = row["salesperson"];
                    newRow["Last Updated"] = row["datechange"];
                    newRow["By"] = row["empeenochange"];
                    newRow["ChqOS"] = row["chequeos"];
                    newRow["PayOS"] = row["paymentos"];
                    newRow["InstantCredit"] = row["InstantCredit"];
                    newRow["CustId"] = row["CustId"];
                    newRow["DateProp"] = row["dateprop"];
                    newRow["Name"] = row["name"];
                    newRow["SubAgreement"] = row["SubAgreement"];
                    newRow["FOC"] = row["FOC"]; //IP/JC - 16/02/10 - CR1048 4.10 (REF:3.1.36) - FOC - CR1072 
                    tab.Rows.Add(newRow);
                }
                ds.Tables.Add(tab);
                return ds;
            }
        }

        public DataSet GetInstantCreditAwaitingClearance(string branchRestriction, int includeHP,
            int includeRF, string holdFlags, SqlConnection conn, SqlTransaction trans)
        {
            Function = "BAccount::GetInstantCreditAwaitingClearance()";

            int result = 0;
            DataSet ds = new DataSet();
            data = new DAccount();
            result = data.GetInstantCreditAwaitingClearance(branchRestriction,
                includeHP,
                includeRF,
                holdFlags, conn, trans);

            ds.Tables.Add(data.InstantCreditAwaitingClearance);
            DataView autoClear = ds.Tables["Table1"].DefaultView;

            //for (int i = 0; i < autoClear.Count; i++)       //CR1225 jec 25/02/11 for loop incorrect - i becomes >count when rows deleted
            for (int i = autoClear.Count - 1; i >= 0; i--)
            {
                AccountRepository AcctRepos = new AccountRepository();
                bool autocleared = AcctRepos.InstantCreditDACheck(Convert.ToString(autoClear[i]["Acctno"]), User, conn, trans); //IP - 03/03/11 - #3255 - Added User
                if (autocleared) //remove row from data set...
                {
                    ds.Tables["Table1"].Rows[i].Delete();       //delete row processed

                }
            }
            ds.Tables["Table1"].AcceptChanges();
            return InstantCreditAwaitingClearance;
        }

        /// <summary>
        /// Property returning a dataset containing all accounts
        /// awaiting clearance
        /// </summary>
        private DataSet AccountsAwaitingClearance
        {
            get
            {
                DataSet ds = new DataSet();
                //We need to create a new table displaying only the columns we
                //want. There's got to be a better way.
                tab = new DataTable();
                tab.Columns.Add("Account");
                tab.Columns.Add("Type");
                tab.Columns.Add("Terms");
                tab.Columns.Add("Date Opened", Type.GetType("System.DateTime"));
                tab.Columns.Add("Sales Person");
                tab.Columns.Add("Last Updated", Type.GetType("System.DateTime"));
                tab.Columns.Add("By");
                tab.Columns.Add("ChqOS");
                tab.Columns.Add("PayOS");
                tab.Columns.Add("InstantCredit");
                tab.Columns.Add("CustId");
                tab.Columns.Add("DateProp", Type.GetType("System.DateTime"));
                tab.Columns.Add("Name");
                tab.Columns.Add("SubAgreement");
                tab.Columns.Add("FOC"); //IP/JC - 16/02/10 - CR1048 4.10 (REF:3.1.36) - FOC - CR1072 
                tab.Columns.Add("IsLoan"); // #8489 jec 01/11/11

                DataRow newRow;
                foreach (DataRow row in data.AccountsAwaitingClearance.Rows) //IP - 15/03/11 - Changed the below to look at the column heading rather than the position.
                {
                    newRow = tab.NewRow();
                    newRow["Account"] = row["AcctNo"];
                    newRow["Type"] = row["AcctType"];
                    newRow["Terms"] = row["termstype"];
                    newRow["Date Opened"] = row["DateAcctOpen"];
                    newRow["Sales Person"] = row["salesperson"];
                    newRow["Last Updated"] = row["datechange"];
                    newRow["By"] = row["empeenochange"];
                    newRow["ChqOS"] = row["chequeos"];
                    newRow["PayOS"] = row["paymentos"];
                    newRow["InstantCredit"] = row["InstantCredit"];
                    newRow["CustId"] = row["custid"];
                    newRow["DateProp"] = row["dateprop"];
                    newRow["Name"] = row["name"];
                    newRow["SubAgreement"] = row["SubAgreement"];
                    newRow["FOC"] = row["FOC"]; //IP/JC - 16/02/10 - CR1048 4.10 (REF:3.1.36) - FOC - CR1072
                    newRow["IsLoan"] = row["IsLoan"];       // #8489 jec 01/11/11
                    tab.Rows.Add(newRow);
                }
                ds.Tables.Add(tab);
                return ds;
            }
        }

        public DataSet GetAccountsAwaitingClearance(string branchRestriction, int includeCash, int includeHP, int includeRF,
            int includePaid, int includeUnpaid, int includeItems, string holdFlags, int includeGOL)
        {
            Function = "BAccount::GetAccountsAwaitingClearance()";

            int result = 0;
            DataSet ds = new DataSet();
            data = new DAccount();
            result = data.GetAccountsAwaitingClearance(branchRestriction,
                includeCash,
                includeHP,
                includeRF,
                includePaid,
                includeUnpaid,
                includeItems,
                holdFlags,
                includeGOL);

            ds.Tables.Add(data.AccountsAwaitingClearance);
            return AccountsAwaitingClearance;
        }


        public DataSet GetUnpaidAccounts(int branchNo, int empeeNoSale)
        {
            Function = "BAccount::GetUnpaidAccounts()";

            int result = 0;
            DataSet ds = new DataSet();
            data = new DAccount();
            result = data.GetUnpaidAccounts(branchNo,
                empeeNoSale);

            ds.Tables.Add(data.AccountDetails);
            return ds;
        }

        public DataSet GetChargesByAcctNo(string acctNo)
        {
            Function = "BAccount::GetChargesByAcctNo()";

            int result = 0;
            DataSet ds = new DataSet();
            data = new DAccount();
            result = data.GetChargesByAcctNo(acctNo);

            ds.Tables.Add(data.AccountDetails);
            return ds;
        }

        public DataSet GetArrearsDailyByAcctNo(string acctNo)
        {
            Function = "BAccount::GetArrearsDailyByAcctNo()";

            int result = 0;
            DataSet ds = new DataSet();
            data = new DAccount();
            result = data.GetArrearsDailyByAcctNo(acctNo);

            ds.Tables.Add(data.AccountDetails);
            return ds;
        }

        /// <summary>
        /// Gets tho hold flag records for a particular account
        /// </summary>
        /// <param name="accountNumber">account number </param>
        /// <returns>a dataset</returns>
        public DataSet GetHoldFlags(string accountNumber)
        {
            Function = "BAccount::GetHoldFlags()";

            DHoldFlags flags = new DHoldFlags();
            DataSet ds = new DataSet();
            flags.GetHoldFlags(accountNumber);
            ds.Tables.Add(flags.Table);

            return ds;
        }

        /// <summary>
        /// Gets the instant credit flag records for a particular account
        /// </summary>
        /// <param name="accountNumber">account number </param>
        /// <returns>a dataset</returns>
        public DataSet GetICFlags(string accountNumber)
        {
            Function = "BAccount::GetICFlags()";

            DHoldFlags flags = new DHoldFlags();
            DataSet ds = new DataSet();
            flags.GetICFlags(accountNumber);
            ds.Tables.Add(flags.Table);

            return ds;
        }

        public DataSet GetAcctNoCtrl(int branchNo)
        {
            Function = "BAccount::GetAcctNoCtrl()";

            int result = 0;
            DataSet ds = new DataSet();
            DAcctNoCtrl ctrl = new DAcctNoCtrl();
            result = ctrl.GetAcctNoCtrl(branchNo);

            ds.Tables.Add(ctrl.AcctNoCtrlData);
            return ds;
        }

        /// <summary>
        /// This method will retrieve the components for a kit product
        /// It will then construct a nodelist of these items and
        /// add them to the document in the correct place.
        /// We must then ensure that none of the components are already 
        /// part of the account item list otherwise the kit cannot be added
        /// </summary>
        /// <param name="doc">The current account lineitems XML document</param>
        /// <param name="kitNode">The XmlNode relating to this kit i.e. the one we should append components to</param>
        /// <param name="productCode">the Kit code</param>
        /// <param name="branchCode">Location selected for the kit</param>
        /// <returns>true if the kit was added false otherwise</returns>
        public XmlNode AddKitToAccount(ref XmlNode doc, XmlNode kitNode, int itemId, short branchCode, string accountType, double kitQty, string countryCode, bool dutyFree, bool taxExempt, short promoBranch, out string err)
        {
            bool status = true;
            XmlUtilities xml = new XmlUtilities();
            XmlDocument create = new XmlDocument();
            XmlNode compParent = create.CreateElement(Elements.RelatedItem);
            XmlNode found = null;
            //double kitDiscount = 0;
            err = "";
            decimal componentTotal = 0;

            int componentId;
            string componentCode;

            DStockItem stock = new DStockItem();
            stock.GetKitComponents(itemId);
            stock.PromoBranch = promoBranch;

            //Create a list of nodes for the components
            foreach (DataRow row in stock.Components.Rows)
            {
                componentId = Convert.ToInt32(row[CN.ComponentID]);
                componentCode = Convert.ToString(row[CN.ComponentNo]);
                stock.IsComponent = true;
                stock.GetItemDetails(null, null, componentId, branchCode, accountType, countryCode, dutyFree, taxExempt);    // CR1212 jec need to supply itemID not zero

                XmlNode itemNode = create.CreateElement(Elements.Item);

                itemNode.Attributes.Append(xml.Attribute(create, Tags.Key, componentId + "|" + branchCode.ToString()));
                itemNode.Attributes.Append(xml.Attribute(create, Tags.Type, IT.Component));

                //Check to see if this key already exists in the document.
                //if it does then there's no point in continuing
                // todo: uat363 rdb need to store parentProductCode as class level variable
                // when an item is selected to edit and use it here when we search for item in xml

                found = xml.findItem(doc, componentId + "|" + branchCode.ToString(), "");
                if (found != null)
                {
                    if (Convert.ToDouble(found.Attributes[Tags.Quantity].Value) > 0)
                    {
                        status = false;
                        found = null;
                        err = GetResource("M_COMPONENTEXISTS", new object[] { componentCode, branchCode.ToString() });
                        //err = "Component with the key: "+compCode+"|"+branchCode.ToString()+" already exists on this account";
                        break;
                    }
                    else
                    {
                        found.ParentNode.RemoveChild(found);
                    }
                }

                itemNode.Attributes.Append(xml.Attribute(create, Tags.Code, componentCode));
                itemNode.Attributes.Append(xml.Attribute(create, Tags.ItemId, componentId.ToString()));
                itemNode.Attributes.Append(xml.Attribute(create, Tags.Location, branchCode.ToString()));
                itemNode.Attributes.Append(xml.Attribute(create, Tags.AvailableStock, stock.AvailableStock.ToString()));
                itemNode.Attributes.Append(xml.Attribute(create, Tags.DamagedStock, stock.DamagedStock.ToString()));
                itemNode.Attributes.Append(xml.Attribute(create, Tags.Description1, stock.ProductDesc1));
                itemNode.Attributes.Append(xml.Attribute(create, Tags.Description2, stock.ProductDesc2));
                itemNode.Attributes.Append(xml.Attribute(create, Tags.SupplierCode, stock.SupplierCode));
                itemNode.Attributes.Append(xml.Attribute(create, Tags.UnitPrice, StripCurrency(stock.UnitPrice.ToString(DecimalPlaces))));
                itemNode.Attributes.Append(xml.Attribute(create, Tags.CashPrice, StripCurrency(stock.CashPrice.ToString(DecimalPlaces))));
                itemNode.Attributes.Append(xml.Attribute(create, Tags.HPPrice, StripCurrency(stock.HPPrice.ToString(DecimalPlaces))));
                itemNode.Attributes.Append(xml.Attribute(create, Tags.DutyFreePrice, StripCurrency(stock.DutyFreePrice.ToString(DecimalPlaces))));
                itemNode.Attributes.Append(xml.Attribute(create, Tags.ValueControlled, stock.ValueControlled.ToString()));
                itemNode.AppendChild(create.CreateElement(Elements.RelatedItem));
                itemNode.Attributes.Append(xml.Attribute(create, Tags.Quantity, ((double)row["componentqty"] * kitQty).ToString()));
                itemNode.Attributes.Append(xml.Attribute(create, Tags.Value, StripCurrency((((double)row["componentqty"] * kitQty) * stock.UnitPrice).ToString(DecimalPlaces))));
                itemNode.Attributes.Append(xml.Attribute(create, Tags.DeliveryDate, kitNode.Attributes[Tags.DeliveryDate].Value));
                itemNode.Attributes.Append(xml.Attribute(create, Tags.DeliveryTime, kitNode.Attributes[Tags.DeliveryTime].Value));
                itemNode.Attributes.Append(xml.Attribute(create, Tags.DeliveryArea, kitNode.Attributes[Tags.DeliveryArea].Value));
                itemNode.Attributes.Append(xml.Attribute(create, Tags.DeliveryProcess, kitNode.Attributes[Tags.DeliveryProcess].Value));
                itemNode.Attributes.Append(xml.Attribute(create, Tags.BranchForDeliveryNote, kitNode.Attributes[Tags.BranchForDeliveryNote].Value));
                itemNode.Attributes.Append(xml.Attribute(create, Tags.ColourTrim, ""));
                itemNode.Attributes.Append(xml.Attribute(create, Tags.TaxRate, stock.TaxRate.ToString()));
                itemNode.Attributes.Append(xml.Attribute(create, Tags.DeliveredQuantity, "0"));
                itemNode.Attributes.Append(xml.Attribute(create, Tags.PlannedDeliveryDate, ""));
                itemNode.Attributes.Append(xml.Attribute(create, Tags.CanAddWarranty, System.Boolean.TrueString));
                itemNode.Attributes.Append(xml.Attribute(create, Tags.DeliveryAddress, kitNode.Attributes[Tags.DeliveryAddress].Value));    //LW72108 jec 09/03/10
                itemNode.Attributes.Append(xml.Attribute(create, Tags.QuantityDiff, "Y"));
                itemNode.Attributes.Append(xml.Attribute(create, Tags.ScheduledQuantity, "0"));
                itemNode.Attributes.Append(xml.Attribute(create, Tags.TaxAmount, "0"));
                itemNode.Attributes.Append(xml.Attribute(create, Tags.ContractNumber, ""));
                itemNode.Attributes.Append(xml.Attribute(create, Tags.ReturnItemNo, componentCode));
                itemNode.Attributes.Append(xml.Attribute(create, Tags.ReturnLocation, branchCode.ToString()));
                itemNode.Attributes.Append(xml.Attribute(create, Tags.ReplacementItem, false.ToString()));
                itemNode.Attributes.Append(xml.Attribute(create, Tags.FreeGift, stock.IsFreeGift.ToString()));
                itemNode.Attributes.Append(xml.Attribute(create, Tags.ExpectedReturnDate, ""));
                itemNode.Attributes.Append(xml.Attribute(create, Tags.PurchaseOrder, false.ToString()));
                itemNode.Attributes.Append(xml.Attribute(create, Tags.Damaged, kitNode.Attributes[Tags.Damaged].Value));
                itemNode.Attributes.Append(xml.Attribute(create, Tags.Assembly, kitNode.Attributes[Tags.Assembly].Value));
                itemNode.Attributes.Append(xml.Attribute(create, Tags.ProductCategory, kitNode.Attributes[Tags.ProductCategory].Value));
                itemNode.Attributes.Append(xml.Attribute(create, Tags.Category, stock.Category.ToString()));
                itemNode.Attributes.Append(xml.Attribute(create, Tags.QtyOnOrder, stock.QtyOnOrder.ToString()));
                itemNode.Attributes.Append(xml.Attribute(create, Tags.Deleted, stock.Deleted));
                itemNode.Attributes.Append(xml.Attribute(create, Tags.SPIFFItem, Convert.ToBoolean(0).ToString()));
                itemNode.Attributes.Append(xml.Attribute(create, Tags.SalesBrnNo, kitNode.Attributes[Tags.SalesBrnNo].Value));
                itemNode.Attributes.Append(xml.Attribute(create, Tags.RefCode, stock.RefCode));
                itemNode.Attributes.Append(xml.Attribute(create, Tags.RepoItem, "false"));
                itemNode.Attributes.Append(xml.Attribute(create, Tags.Express, Convert.ToString(kitNode.Attributes[Tags.Express].Value)));      //IP - 14/06/12 - #10378 - Warehouse & Deliveries
                itemNode.Attributes.Append(xml.Attribute(create, Tags.WarrantyType, ""));        //#17883             //#17434
                itemNode.Attributes.Append(xml.Attribute(create, Tags.DateDelivered, ""));      //#19016
                componentTotal += Convert.ToDecimal(itemNode.Attributes[Tags.Value].Value);

                compParent.AppendChild(itemNode);
            }

            if (status)
            {
                SetUpKitDiscount(ref doc, kitNode, create, ref compParent, itemId, branchCode, accountType,
                                 kitQty, countryCode, dutyFree, taxExempt, promoBranch, componentTotal, stock, ref err);
            }
            return compParent;
        }

        /// <summary>
        /// Returns a dataset containing customer account details.
        /// Searches by customerID, firstName, lastName, address
        /// or phone number.
        /// </summary>
        /// <param name="customerId">customer ID</param>
        /// <returns></returns>
        public DataSet AccountsSearch(
            string accountNo,
            string custId,
            string firstName,
            string lastName,
            string address,
            string postCode,
            string phoneNumber,     //CR1084
            bool settled,
            bool exactMatch,
            int limit,
            string storeType,
            out int accountExists,
            out string accountType)
        {
            accountExists = 0;
            accountType = string.Empty;
            DataSet ds = null;
            //DataTable dt = null;
            DAccount accounts = new DAccount();

            if (settled)
                accountStatus = "";
            else
                accountStatus = "S";

            // Search for customer accounts
            accounts.AccountsSearch(accountNo, custId, firstName, lastName, address, postCode, phoneNumber, accountStatus, limit,   //CR1084
                                    exactMatch, storeType, out accountExists, out accountType);

            if (accountExists != 1)
            {
                ds = new DataSet();
                ds.Tables.Add(accounts.AccountsList);
                accountType = String.Empty;
            }
            //else
            //{
            // Need to know the account type in case we cannot revise cash accounts
            // but no need to go back to the database to get this (as well as instantiating yet another object in the process of doing so)
            //DAccount acct = new DAccount(accountNo);
            //accountType = acct.AccountType;
            //}
            return ds;
        }




        /// <summary>
        /// Returns a dataset containing customer account details.
        /// Searches by customerID, firstName, lastName, address
        /// or phone number.
        /// </summary>       
        /// <returns></returns>
        public DataSet InvoiceAccountsSearch(
            int BranchNo,
            DateTime InvoiceDateFrom,
            DateTime InvoiceDateTo,
            string InvoiceNo,
            string accountNo



            )
        {

            DataSet ds = null;

            DAccount accounts = new DAccount();
            accounts.InvoiceAccountsSearch(BranchNo, InvoiceDateFrom, InvoiceDateTo, InvoiceNo, accountNo);
            ds = new DataSet();
            ds.Tables.Add(accounts.AccountsList);
            return ds;
        }






        /// <summary>
        /// Returns a dataset containing incomplete credit applications
        /// Searches by branch restriction
        /// </summary>
        /// <param name="customerId">customer ID</param>
        /// <returns></returns>  

        public DataSet IncompleteCredits(string branchRestriction,
            string holdFlags,
            bool viewTop,
            string acctno,
            bool ChxOnly,
            bool ChxItems,
            bool ChxUnpaid,
            string refCode,
            bool ChxReferral,
            bool ChxPending)
        {

            DataSet ds = null;
            //DataTable dt = null;
            DAccount accounts = new DAccount();


            if (viewTop)
                viewLimit = "MIN";
            else
                viewLimit = "MAX";

            // Search for customer accounts
            accounts.GetIncompleteCredits(branchRestriction, holdFlags, viewLimit, acctno,
                ChxOnly, ChxItems, ChxUnpaid, refCode, ChxReferral, ChxPending);

            ds = new DataSet();
            ds.Tables.Add(accounts.IncompleteCredits);

            return ds;

        }
        /// <summary>
        /// Returns a dataset containing incomplete credit applications
        /// Searches by branch restriction
        /// </summary>
        /// <param name="customerId">customer ID</param>
        /// <returns></returns>  

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

            //DataSet ds = null;
            //DataTable dt = null;
            DAccount accounts = new DAccount();
            DataSet ds = new DataSet();

            /*if(viewTop)
                viewLimit = "MIN";
            else
                viewLimit = "MAX";*/

            // Search for customer accounts
            accounts.Getacctsforalloc(alreadyAllocated,
                minimumStatus,
                currStatus,
                employeeType,
                dateStartAllocated,
                dateFinishedAllocated,
                actionChoice,
                actionStart,
                actionEnd,
                minimumArrears,
                maximumArrears,
                statusType,
                arrearsChoice,
                arrears,
                actionCode,
                letterCode,
                letterRestriction,
                letterRadio,
                letterStart,
                letterFinish,
                actionRestriction,
                empeeno,
                actiondateStart,
                actiondateFinish,
                includePhone,
                includeAddress,
                branchno,
                branchset,
                proposalPoints,
                propPointsDirection,
                codeRestriction,
                accountBranch,
                viewTop,
                ref rowLimited,
                code,
                numActions,
                actionOperand,
                balanceOperand,
                restrictEmployee,
                balance,
                includeCharges,
                dateMovedFrom,
                dateMovedTo,
                dateMovedRestriction,
                datelastPaid,
                dateOperand,
                actionDueDate,
                credit,
                cash,
                service);


            ds.Tables.Add(accounts.ArrearsAccounts);

            //return accounts.arrearAccounts; 
            return ds;
        }

        /// <summary>
        /// Overloaded method that returns a dataset containing accounts allocated to a worklist
        /// Searches by branch restriction, worklist restriction, area restriction
        /// </summary>
        /// <param name="customerId">customer ID</param>
        /// <returns></returns>  

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
              bool service,
           string worklist,
           string deliveryArea)
        {

            //DataSet ds = null;
            //DataTable dt = null;
            DAccount accounts = new DAccount();
            DataSet ds = new DataSet();

            /*if(viewTop)
               viewLimit = "MIN";
            else
               viewLimit = "MAX";*/

            // Search for customer accounts
            ds = accounts.Getacctsforalloc(alreadyAllocated,
               minimumStatus,
               currStatus,
               employeeType,
               dateStartAllocated,
               dateFinishedAllocated,
               actionChoice,
               actionStart,
               actionEnd,
               minimumArrears,
               maximumArrears,
               statusType,
               arrearsChoice,
               arrears,
               actionCode,
               letterCode,
               letterRestriction,
               letterRadio,
               letterStart,
               letterFinish,
               actionRestriction,
               empeeno,
               actiondateStart,
               actiondateFinish,
               includePhone,
               includeAddress,
               branchno,
                   branchset,
               proposalPoints,
               propPointsDirection,
               codeRestriction,
               accountBranch,
               viewTop,
               ref rowLimited,
               code,
               numActions,
               actionOperand,
               balanceOperand,
               restrictEmployee,
               balance,
               includeCharges,
               dateMovedFrom,
               dateMovedTo,
               dateMovedRestriction,
               datelastPaid,
               dateOperand,
               actionDueDate,
                   credit,
                   cash,
                   service,
                   worklist,
                   deliveryArea);


            //ds.Tables.Add(accounts.ArrearsAccounts);

            //return accounts.arrearAccounts; 
            return ds;
        }

        /// <summary>
        /// Returns a DataSet that contains all the accounts in the bailiff or collector strategies
        /// </summary>
        /// <returns></returns>
        public DataSet GetStrategyAccountsToAllocate()
        {
            DAccount accounts = new DAccount();
            DataSet ds = new DataSet();
            ds = accounts.GetStrategyAccountsToAllocate();
            return ds;
        }

        /// <summary>
        /// Returns a dataset containing customer account details.
        /// Searches by customer ID
        /// </summary>
        /// <param name="customerId">customer ID</param>
        /// <returns></returns>
        public DataSet GetAccountDetails(string accountNumber)
        {
            DataSet ds = new DataSet();
            DAccount details = new DAccount();
            details.GetAccountDetails(null, null, accountNumber, 1); //IP - 11/02/11 - Sprint 5.10 - #2978 - added null, null for conn, trans
            ds.Tables.Add(details.AccountDetails);
            return ds;
        }

        /// <summary>
        /// Returns a dataset containing the account no, custid and cust name
        /// </summary>
        /// <param name="accountNumber">account number</param>
        /// <param name="customerId">customer ID</param>
        /// <returns></returns>
        public DataSet GetAccountName(string accountNumber, string customerID)
        {
            DataSet ds = new DataSet();
            DAccount name = new DAccount();
            name.GetAccountName(accountNumber, customerID);
            ds.Tables.Add(name.AccountName);
            if (ds.Tables[0].Rows.Count > 0)
            {
                string str = (string)ds.Tables[0].Rows[0]["AccountNo"];
                FormatAccountNo(ref str);
                ds.Tables[0].Rows[0]["AccountNo"] = str;
            }
            return ds;
        }

        /// <summary>
        /// Returns a dataset containing the codes currently on an account
        /// </summary>
        /// <param name="accountNumber">account number</param>
        /// <returns></returns>
        public DataSet GetCodesOnAccount(string accountNumber, out bool noSuchAccount)
        {
            DataSet ds = new DataSet();
            DAccount codes = new DAccount();
            codes.GetCodesOnAccount(accountNumber, out noSuchAccount);
            ds.Tables.Add(codes.AccountCodes);
            return ds;
        }

        /// <summary>
        /// This method will update the acctcodes table with the current selection
        /// </summary>
        /// <param name="con">SqlConnection object</param>
        /// <param name="tran">SqlTransaction object</param>
        /// <param name="accountNumber">account number</param>
        /// <param name="ds">dataset containing updates</param>
        public void AddCodesToAccount(SqlConnection con, SqlTransaction tran,
            string accountNumber, DataSet ds)
        {
            DAccount acct = new DAccount();

            //First delete all the codes on the account
            acct.DeleteCodesFromAccount(con, tran, accountNumber);

            //Then add all codes in the new dataset
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                acct.AddCodeToAccount(con, tran, accountNumber, (string)row["Code"], Convert.ToDateTime(row["Date Added"]), Convert.ToInt32(row["Added By"]), "");
            }
        }

        public InstantReplacementDetails SaveReplacement(SqlConnection conn, SqlTransaction trans,
            string accountNo, string accountType,
            string country, short branchNo, bool taxExempt,
            XmlNode replacementXml, string loyaltyCardNo, short promoBranch)
        {
            string xPath = "";
            int refNo = 0;
            string contractNo = "";
            DBranch branch = new DBranch();
            BDelivery del = new BDelivery();
            BTransaction fintrans = null;
            XmlNode toCollect = null;
            XmlNode warranty = null;
            XmlNode stax = null;
            XmlNode discount = null;
            decimal collectedValue = 0;
            decimal tax = 0;
            decimal oldQty = 0;
            decimal newQty = 0;

            /* deserialise the xmlnode back into more useable form */
            InstantReplacementDetails replacement = InstantReplacementDetails.DeSerialise(replacementXml);

            /* the branch number for that the original item was sold at may
             * not be the branch we are currently logged into, therefore 
             * for the collections we should use the branchno from the original
             * account */
            branchNo = replacement.StockLocn;
            //branchNo = Convert.ToInt16(accountNo.Substring(0, 3));

            branch.Populate(conn, trans, branchNo);
            refNo = branch.GetTransRefNo(conn, trans, branchNo);              // #17290  fix

            /* deserialise the xmlnode back into more useable form */
            //InstantReplacementDetails replacement = InstantReplacementDetails.DeSerialise(replacementXml);

            /* retrieve the lineitems for this agreement no */
            BItem item = new BItem();
            item.PromoBranch = promoBranch;
            XmlNode lineItems = item.GetLineItems(conn, trans, accountNo, accountType, country, replacement.AgreementNo);

            /* find the item we need to collect */
            xPath = "//Item[@Key = '" + replacement.ItemId + "|" + branchNo.ToString() + "']";
            toCollect = lineItems.SelectSingleNode(xPath);

            xPath = String.Format("//Item[@Key = '{0}|{1}']", StockItemCache.Get(StockItemKeys.STAX), branchNo);
            stax = lineItems.SelectSingleNode(xPath);

            if (toCollect != null)
            {
                /* collect the item */
                if (!replacement.Notify)
                {
                    //del = new BDelivery();
                    collectedValue += del.ReplacementDelivery(conn, trans, accountNo,
                        replacement.AgreementNo,
                        -replacement.Quantity,
                        branchNo, User, refNo,
                        replacement.NewBuffNo, contractNo,
                        replacement.ItemNo, replacement.ItemId,
                        branchNo,
                        replacement.Price, replacement.ReturnItemId,
                        replacement.ReturnStockLocn);

                    //#17290
                    if (accountType == AT.Special)          // only for Cash & Go   
                    {
                        DDelivery delivery = new DDelivery();
                        delivery.WarrantyFulFilled(conn, trans, replacement.NewBuffNo, accountNo,      //#14313
                                replacement.AgreementNo, //sched.ItemNumber,
                                replacement.ItemId,        // RI
                                replacement.ReturnStockLocn, true, Math.Abs((Convert.ToInt32(replacement.Quantity))), "I", "Y"); //#17678
                    }

                }
                else
                {
                    NotifyReplacement = true;
                    ProcessSchedule(conn, trans, accountNo, replacement.AgreementNo, "C", replacement.ItemNo,
                                    replacement.StockLocn, Convert.ToDouble(-replacement.Quantity), contractNo, 0); //IP - 29/07/11 - RI - #4429
                }

                /* update the lineitem qty and value*/
                oldQty = Convert.ToDecimal(toCollect.Attributes[Tags.Quantity].Value);
                newQty = oldQty - replacement.Quantity;
                // todo uat363 need to get the parentItemNo here 
                item.UpdateItemQuantity(conn, trans, accountNo,
                    replacement.AgreementNo,
                    replacement.ItemId,                                 //IP - 17/05/11 - CR1212 - #3627 - Changed to use ItemID rather than ItemNo
                    branchNo, "",
                    newQty, 0);                                         //IP - 17/05/11 - CR1212 - #3627 - pass through 0 for parentItemID was previously ""

                /* update the delivered quantity */
                // todo uat363 rdb need to get ParentProductCode
                item.UpdateDelQty(conn, trans, accountNo,
                    replacement.AgreementNo,
                    branchNo, replacement.ItemId,                       //IP - 17/05/11 - CR1212 - #3627 - Changed to use ItemID rather than ItemNo
                    "", -replacement.Quantity, 0);                      //IP - 17/05/11 - CR1212 - #3627 - pass through 0 for parentItemID was previously ""

                /* accumulate any sales tax which we may need to collect */
                toCollect.Attributes[Tags.Quantity].Value = replacement.Quantity.ToString();
                if ((string)Country[CountryParameterNames.AgreementTaxType] == "E")
                    tax += item.CalculateTaxAmount(toCollect, taxExempt);

                /* handle any discounts*/
                xPath = "//Item[@Type='Discount' and @Quantity!='0']";
                discount = toCollect.SelectSingleNode(xPath);
                if (discount != null)
                {
                    /* deliver the discount */

                    collectedValue += del.ReplacementDelivery(conn, trans, accountNo,
                        replacement.AgreementNo,
                        -1,
                        branchNo, User,
                        refNo, replacement.NewBuffNo,
                        contractNo,
                        discount.Attributes[Tags.Code].Value, Convert.ToInt32(discount.Attributes[Tags.ItemId].Value),
                        branchNo,
                        Convert.ToDecimal(discount.Attributes[Tags.UnitPrice].Value),
                        Convert.ToInt32(discount.Attributes[Tags.ItemId].Value), Convert.ToInt16(discount.Attributes[Tags.Location].Value), replacement.ItemId);  //#18575

                    /* update the lineitem qty and value*/
                    // todo uat363 need to get the parentItemNo here 
                    item.UpdateItemQuantity(conn, trans, accountNo,
                        replacement.AgreementNo,
                        Convert.ToInt32(discount.Attributes[Tags.ItemId].Value),                     //IP - 17/05/11 - CR1212 - #3627 - Changed to use ItemID
                        branchNo, contractNo,
                        0, replacement.ItemId);                                                      //#18575 //IP - 17/05/11 - CR1212 - #3627 - Changed to use 0 rather than "" for parentItemID

                    /* update the delivered quantity */
                    // todo uat363 rdb need to get ParentProductCode
                    item.UpdateDelQty(conn, trans, accountNo,
                        replacement.AgreementNo,
                        branchNo,
                        Convert.ToInt32(discount.Attributes[Tags.ItemId].Value),                     //IP - 17/05/11 - CR1212 - #3627 - Changed to use ItemID
                        contractNo, -1, 0);                                                         //IP - 17/05/11 - CR1212 - #3627 - Changed to use 0 rather than "" for parentItemID

                    discount.Attributes[Tags.Quantity].Value = "1";
                    if ((string)Country[CountryParameterNames.AgreementTaxType] == "E")
                        tax += item.CalculateTaxAmount(discount, taxExempt);
                }

                /* drill down to the warranty */
                xPath = "//Item[@Key = '" + replacement.WarrantyID + "|" + branchNo.ToString() + "' and @ContractNumber = '" + replacement.ContractNo + "']";
                warranty = toCollect.SelectSingleNode(xPath);
                if (warranty != null)
                {
                    contractNo = replacement.ContractNo;
                    string returnItem = "";
                    if (replacement.TimePeriod != OneForOneTimePeriod.IRPeriod1 && replacement.TimePeriod != OneForOneTimePeriod.IRPeriod2)
                    {
                        item.ItemId = replacement.ItemId;
                        item.SalesBrnNo = replacement.StockLocn;
                        returnItem = item.GetWarrantyReturnItem(conn, trans, replacement.ElapsedMonths);
                    }

                    if (accountType == AT.Special)
                    {
                        //#17290 - Warranty now delivered through WarrantyRepository
                        ///////* deliver the warranty */
                        //////collectedValue += del.ReplacementDelivery(conn, trans, accountNo,
                        //////    replacement.AgreementNo,
                        //////    -1,
                        //////    branchNo, User,
                        //////    refNo, replacement.NewBuffNo,
                        //////    contractNo, replacement.WarrantyNo,
                        //////    replacement.WarrantyID,
                        //////    branchNo,
                        //////    Convert.ToDecimal(warranty.Attributes[Tags.UnitPrice].Value),
                        //////    0,   //TODO; Need to pass in RetItemId
                        //////    0);


                        //#17290 - Do not update for warranties, as warranties not collected for Replacement
                        /* update the lineitem qty and value*/
                        // todo uat363 need to get the parentItemNo here 
                        ////item.UpdateItemQuantity(conn, trans, accountNo,
                        ////    replacement.AgreementNo,
                        ////    replacement.WarrantyID,                 //IP/NM - 18/05/11 -CR1212 - #3627
                        ////    branchNo, contractNo,
                        ////    0, 0);                                  //IP - 18/05/11 - CR1212 - #3627 - Replaced "" with 0 for parentItemID

                        /* update the delivered quantity */
                        // todo uat363 rdb need to get PArentProductCode
                        ////item.UpdateDelQty(conn, trans, accountNo,
                        ////    replacement.AgreementNo,
                        ////    branchNo,
                        ////    replacement.WarrantyID,                  //IP/NM - 18/05/11 -CR1212 - #3627
                        ////    contractNo, -1, 0);                     //IP - 18/05/11 - CR1212 - #3627 - Replaced "" with 0 for parentItemID

                        //#17290 - Warranties not collected on Replacement
                        /* accumulate any sales tax which we may need to collect */
                        ////if ((string)Country[CountryParameterNames.AgreementTaxType] == "E")
                        ////    tax += item.CalculateTaxAmount(warranty, taxExempt);
                    }

                    replacement.CurrentWarranty = warranty.CloneNode(true);
                    warranty.Attributes[Tags.Quantity].Value = "1";
                }

                /* collect the tax */
                if (tax > 0)
                {
                    collectedValue += del.ReplacementDelivery(conn, trans, accountNo,
                        replacement.AgreementNo, -1,
                        branchNo, User, refNo,
                        replacement.NewBuffNo,
                        "", "STAX",
                        StockItemCache.Get(StockItemKeys.STAX),
                        branchNo,
                        tax, 0, 0);

                    /* update lineitem value */
                    decimal oldTaxValue = 0;
                    if (stax != null)
                        oldTaxValue = Convert.ToDecimal(stax.Attributes[Tags.Value].Value);

                    item.UpdateItemValue(conn, trans, accountNo,
                        replacement.AgreementNo,
                        StockItemCache.Get(StockItemKeys.STAX),  //IP/NM - 18/05/11 -CR1212 - #3627 - cached item
                        branchNo,
                        oldTaxValue - tax);
                }

                DateTime dateTrans = DateTime.Now;
                /* write a fintrans record for the total amount that has been collected */
                fintrans = new BTransaction(conn, trans, accountNo, branchNo, refNo,
                    collectedValue, User, "GRT", "", "", "", 0,
                    country, dateTrans, "", 0);

                /* if required write a loyalty card record */
                if ((bool)Country[CountryParameterNames.LoyaltyCard])
                {
                    BTransaction t = new BTransaction();
                    t.SaveLoyaltyCardRecord(conn, trans, refNo, dateTrans, accountNo, loyaltyCardNo, replacement.AgreementNo);
                }
            }
            return replacement;
        }

        public int SaveNewAccount(SqlConnection conn, SqlTransaction trans,
            string accountNumber, short branchNo,
            string accountType, string CODFlag,
            int salesPerson, string SOA, decimal agreementTotal,
            decimal deposit, decimal serviceCharge,
            XmlNode lineItems, string termsType, string newBand,
            string countryCode, DateTime dateFirst,
            decimal instalAmount, decimal finalInstalment,
            string paymentMethod, int months, Int16 PrefDay, 
            bool taxExempt, bool dutyFree, decimal taxAmount,
            bool collection, string bankCode,
            string bankAcctNo, string chequeNo,
            short payMethod, XmlNode replacementXml,
            decimal dtTaxAmount, string loyaltyCardNo, bool reScore,
            decimal tendered, decimal giftVoucherValue,
            bool courtsVoucher, string voucherReference,
            int voucherAuthorisedBy, string acctNoCompany, short promoBranch,
            short paymentHolidays,
            DataTable payMethodList, short dueDay, int returnAuthorisedBy,
            DataTable warrantyRenewal, DataTable variableRates,
            ref string propResult, ref DateTime dateProp,
            ref int agreementNo, out string bureauFailure, int user, bool resetHoldProp, bool autoda, string storeCardAcctNo, long? storeCardNumber, out int storeCardTransRefNo, out string referralReasons, bool readyAssist = false, int? readyAssistTermLength = null //IP - 17/01/11 - Added storeCardAcctNo, storeCardNumber and storeCardTransRefNo //IP - 15/03/11 - #3314 - CR1245 - Return referral reasons
            , bool reviseAccount = false
            )//set to true when account is being revised
        {
            Function = "BAccount::SaveNewAccount()";
            bureauFailure = "";
            int buffNo = 0;
            bool agreementTotalChanged = false;
            bool paidAndTaken = false;
            InstantReplacementDetails replacement = null;
            BItem bItem = new BItem();
            BBranch bBranch = new BBranch();  //#17290

            int result = (int)Return.Success;
            bool existingAccount = false;

            DLineItem lineItem = new DLineItem();
            int stockCount = 0;
            int affinityCount = 0;
            bool isNewAccount = true;
            storeCardTransRefNo = 0;            //IP - 17/01/11 - Store Card
            // Need Custoemr Id  for behavioural scoring.
            referralReasons = string.Empty;  //IP - 14/03/11 - #3314 - CR1245


            bool schedRejItems = false;     //IP - 03/06/10 - UAT(262) UAT5.2.1.0 Log

            // DSR UAT 84 - Do NOT set properties here before the account is populated
            //this.TermsType = termsType;

            //does the account exist?
            DAccount acctDA = new DAccount();
            existingAccount = Populate(conn, trans, accountNumber);

            //paidAndTaken = accountNumber == GetPaidAndTakenAccount(conn, trans, branchNo.ToString());
            paidAndTaken = accountNumber == GetPaidAndTakenAccount(conn, trans, accountNumber.Substring(0, 3));
            this.TermsType = termsType;

            /* if we're doing an IR for a warranty	*/
            /* which was bought on credit we must	*/
            /* treat this like the paid and taken account also	*/
            paidAndTaken = ((paidAndTaken) ||
                            (accountType == AT.Special && TT.IsPaidAndTaken(TermsType)) ||
                            (replacementXml != null || collection));

            #region Replacement processing
            if (paidAndTaken && accountType == AT.Special)  //#17290 - Only do this for Cash & Go
            {
                /* if we're saving a replacement we need to collect the item we're 
                * replacing etc. All the details should be in the xml doc */
                if (replacementXml != null)
                {
                    replacement = SaveReplacement(conn, trans, accountNumber, accountType, countryCode, branchNo, taxExempt, replacementXml, loyaltyCardNo, promoBranch);

                    if (accountType == AT.Special && TermsType == TT.WarrantyOnCredit)
                    {							/* keep the same agreement */
                        agreementNo = replacement.AgreementNo;
                    }
                    else
                        if (accountType == AT.Special)
                    {
                        //#17290
                        //buffNo = replacement.NewBuffNo;
                        buffNo = bBranch.GetBuffNo(conn, trans, replacement.StockLocn);   // #17290 
                        agreementNo = replacement.AgreementNo;
                        //agreementNo = buffNo = replacement.NewBuffNo;
                    }

                    //#17290 - Removed ??
                    /* there ought to be only one stock item in the items doc for
                     * a replacement. Find it and attach the old warranty to it, but
                     * only if the TimePeriod is 1 or 2 so the warranty has not terminated
                     * and the replacement item is on the IR scheme*/
                    //////if ((replacement.TimePeriod == OneForOneTimePeriod.IRPeriod1 || replacement.TimePeriod == OneForOneTimePeriod.IRPeriod2)
                    //////    && bItem.IsItemInstantReplacement(conn, trans, replacement.ItemId, Convert.ToInt32(branchNo)))
                    //////{
                    //////    /* find the sales tax item */
                    //////    string xPath = "//Item[@Key = 'STAX|" + branchNo.ToString() + "']";
                    //////    XmlNode stax = lineItems.SelectSingleNode(xPath);

                    //////    foreach (XmlNode i in lineItems.ChildNodes)
                    //////    {
                    //////        if (i.Attributes[Tags.Type].Value == IT.Stock &&
                    //////            i.Attributes[Tags.Quantity].Value != "0")
                    //////        {

                    //////            bool found = false;
                    //////            DataSet ds = bItem.GetProductWarranties(conn,
                    //////                                        trans,
                    //////                                        Convert.ToInt32(i.Attributes[Tags.ItemId].Value),
                    //////                                        branchNo,
                    //////                                        Convert.ToDouble(i.Attributes[Tags.UnitPrice].Value),
                    //////                                        "",
                    //////                                        true);

                    //////            foreach (DataRow r in ds.Tables[0].Rows)
                    //////                if ((string)r[CN.waritemno] == replacement.CurrentWarranty.Attributes[Tags.Code].Value)
                    //////                {
                    //////                    found = true;
                    //////                    break;
                    //////                }

                    //////            if (found)	/* i.e. if the current warranty os valid for the replacement item */
                    //////            {
                    //////                string path = "//Item[@Key = '" + replacement.WarrantyID + "|" + replacement.StockLocn.ToString() + "' and @ContractNumber = '" + replacement.ContractNo + "']";
                    //////                XmlNode toDelete = i.SelectSingleNode(path);
                    //////                if (toDelete != null)
                    //////                    toDelete.ParentNode.RemoveChild(toDelete);

                    //////                XmlNode related = i.SelectSingleNode(Elements.RelatedItem);
                    //////                replacement.CurrentWarranty = related.OwnerDocument.ImportNode(replacement.CurrentWarranty, true);
                    //////                related.AppendChild(replacement.CurrentWarranty);

                    //////                if (paidAndTaken && stax != null)	/* add the tax for the warranty onto the sales tax line item */
                    //////                {
                    //////                    decimal oldTax = Convert.ToDecimal(stax.Attributes[Tags.Value].Value);
                    //////                    oldTax += Convert.ToDecimal(replacement.CurrentWarranty.Attributes[Tags.TaxAmount].Value);
                    //////                    stax.Attributes[Tags.Value].Value = oldTax.ToString();
                    //////                    stax.Attributes[Tags.UnitPrice].Value = oldTax.ToString();
                    //////                }
                    //////            }
                    //////            else
                    //////            {
                    //////                /* TO DO what can/do we do????? */
                    //////            }
                    //////        }
                    //////    }
                    //////}
                }
                else
                {
                    // 5.1 uat144 19/11/07 rdb if c+g acc attached to customer
                    // and then printed agreementno increases, check for existingaccount
                    if (paidAndTaken &&
                       (!existingAccount || accountNumber == GetPaidAndTakenAccount(conn, trans, accountNumber.Substring(0, 3))))
                    {
                        BBranch b = new BBranch();
                        buffNo = b.GetBuffNo(conn, trans, branchNo);        // #16339

                        HiLo cashAndGoAgrmtNo = HiLo.Cache("CashAndGoAgrmtNo");                                 //IP - 28/03/12 - #9846 

                        if (!collection)					/* for collections re-use the	*/
                            agreementNo = cashAndGoAgrmtNo.NextId();                                             //IP - 28/03/12 - #9486
                        //agreementNo = buffNo;		/* the previous agreement no	*/
                    }

                    //					//IP - 17/03/08 - (69630)
                    //					//Retrieve the buffno for the Cash & Go account.
                    //Previously there wasnt anything to handle collection and there was
                    //no code to retrieve the buffno for an existing account. If collection
                    //then generate a new buffno. If existing use the existing buffno.
                    else
                    {
                        if (collection)
                        {
                            BBranch b = new BBranch();
                            buffNo = b.GetBuffNo(conn, trans, branchNo);        // #16339
                        }
                        else
                        {
                            buffNo = acctDA.GetExistingBuffNo(accountNumber);
                            agreementNo = buffNo;
                        }
                    }
                }
            }
            #endregion

            /* don't lock the paid and taken account because other people
			 * need to use it. No need to lock new accounts */
            if (!paidAndTaken && existingAccount)
                this.ValidLock(conn, trans, accountNumber, this.User);

            agreementTotalChanged = (AgreementTotal < agreementTotal);

            if (replacementXml != null)
            {
                if ((accountType == AT.Special && !replacement.Notify) || accountType != AT.Special) //#17290
                    agreementTotalChanged = false;

                if (accountType != AT.Special) //#17290
                {
                    replacement = InstantReplacementDetails.DeSerialise(replacementXml);
                }

                //#17290
                string xPathItem = "//Item[@ReplacementItem = 'True']";
                XmlNode item = lineItems.SelectSingleNode(xPathItem);

                new AccountRepository().UpdateExchangeReplaceItem(conn, trans, accountNumber, agreementNo, replacement.ItemId, replacement.StockLocn, Convert.ToInt32(item.Attributes[Tags.ItemId].Value));
            }

            this.BranchNo = branchNo;
            this.AccountType = accountType;
            this.AccountNumber = accountNumber;
            this.AgreementTotal = agreementTotal;
            this.CountryCode = countryCode;

            /* do some initialisation for new accounts */
            if (!existingAccount)
            {
                this.DateAccountOpen = DateTime.Now;
                this.OutstandingBalance = 0;
                if (AT.IsCashType(accountType))
                    this.CurrentStatus = "1";
                else
                    this.CurrentStatus = "0";
                this.CreditDays = 0;
                this.Arrears = 0;
                this.HighestStatus = "1";
                this.HighestStatusDays = 0;

            }

            if (lineItems != null)
                this.HasLineItems = (lineItems.SelectNodes("//Item[@Type='Stock' and @Quantity != '0']").Count > 0);

            this.Save(conn, trans);	//Save the account record
            BProposal BProp = new BProposal();
            string CustomerBand = "";

            if (existingAccount) //IP - 09/03/11 - #3291 - Only determine band for existing accounts.
            {
                BProp.DetermineBands(conn, trans, accountNumber, CustomerID, out newBand, out CustomerBand, "", 0);
            }

            if ((bool)Country[CountryParameterNames.TermsTypeBandEnabled] &&
                !(existingAccount && newBand == "")) // But do not update a blank band when revising
            {
                // Save the scoring band entered on NSO
                // If 'newBand' is blank the band will be calculated from the current score
                DProposal prop = new DProposal();
                prop.AccountNo = accountNumber;
                //CR916 - IP - 10/04/08 - Need to set the employee number
                //which will be used when inserting into the ProposalAudit table
                prop.EmployeeNoChanged = this.User;
                prop.OverrideBand(conn, trans, newBand);
            }
            // Code for scoring moved from here to after save of lineitems!!!! 68538 jec 10/10/06
            // scoring was using previous line items for calculation of @loanamt

            //add account codes for taxExempt and dutyFree if necessary
            string reference = "";
            if (paidAndTaken)
                reference = agreementNo.ToString();

            if (taxExempt)
                acctDA.AddCodeToAccount(conn, trans, accountNumber, "TE", DateTime.Now, salesPerson, reference);
            else
                acctDA.DeleteCodeFromAccount(conn, trans, accountNumber, "TE", reference);

            if (dutyFree)
                acctDA.AddCodeToAccount(conn, trans, accountNumber, "DF", DateTime.Now, salesPerson, reference);
            else
                acctDA.DeleteCodeFromAccount(conn, trans, accountNumber, "DF", reference);


            lineItem.AccountNumber = accountNumber;
            lineItem.CheckForStockItems(conn, trans, out stockCount, out affinityCount);
            isNewAccount = (stockCount == 0 && DateTime.Now.Date == this.DateAccountOpen.Date);

            if (!isNewAccount && !agreementTotalChanged)
            {
                agreementTotalChanged = GiftItemAdded(conn, trans, lineItems);
            }

            if (agreementNo == 0)
                agreementNo = 1;
            //Set up the agreement information
            Agreement = new BAgreement();
            Agreement.Populate(conn, trans, accountNumber, agreementNo);

            if (!existingAccount)
            {
                Agreement.AccountNumber = accountNumber;
                Agreement.AgreementNumber = agreementNo;
                Agreement.HoldMerch = "Y";
                Agreement.OldAgreementBalance = 0;
                Agreement.SundryChargeTotal = 0;
                Agreement.HoldProp = "Y";
                Agreement.CreatedBy = User;
            }

            //IP - 04/06/10 - UAT(262) UAT5.2.1.0 Log - Check if any items have been rejected by DHL
            if (existingAccount && !paidAndTaken)
            {

                if (lineItems != null)
                {
                    foreach (XmlNode item in lineItems)
                    {
                        if (Convert.ToBoolean(item.Attributes[Tags.ItemRejected].Value) && Convert.ToInt32(item.Attributes[Tags.Quantity].Value) != 0)
                        {
                            schedRejItems = true;
                            break;
                        }

                    }
                }
            }

            // if (agreementTotalChanged)
            if (agreementTotalChanged || schedRejItems)      //IP - 04/06/10 - UAT(262) UAT5.2.1.0 Log
            {
                if (agreementTotalChanged)
                {
                    Agreement.AgreementDate = DateTime.Today;
                }
                // todo v5.1 uat74 rdb - 8/11/07 if this is an account with an item of quantity 2
                // 1 item delivered, 1 not, then 2nd item deleted here dont reset propFlag
                if (resetHoldProp)
                    Agreement.HoldProp = "Y";
            }


            if (termsType != "" && AT.IsCreditType(accountType))
            {
                // 66534 Terms Type can deliver non-stocks immediately
                DTermsType tt = new DTermsType();
                tt.GetTermsTypeDetail(conn, trans, countryCode, termsType, accountNumber, newBand, DateTime.Now);
                if ((short)(tt.TermsTypeDetails.Rows[0][CN.DeliverNonStocks]) == 1)
                    Agreement.DeliveryFlag = "Y";
            }

            /* find the value of the insurance and admin charges and cater for it
            * when working out the cash price */
            decimal extraCharges = 0;

            if (lineItems != null)
            {
                XmlNodeList charges = lineItems.SelectNodes("//Item[(@Code='" + (string)Country[CountryParameterNames.InsuranceChargeItem] + "' or @Code='" + (string)Country[CountryParameterNames.AdminChargeItem] + "') and @Quantity!='0']");
                foreach (XmlNode n in charges)
                {
                    extraCharges += Convert.ToDecimal(n.Attributes[Tags.Value].Value);
                }
            }

            if (isNewAccount)
            {
                Agreement.AuditSource = AS.NewAccount;

                //IP - 09/05/11 - Store Card - Feature - #3004
                if (accountType == AT.StoreCard && Source == "PreApprove")
                {
                    Agreement.AuditSource = StoreCardSource.Preapproval;
                }
            }
            else
            {
                Agreement.AuditSource = AS.Revise;
            }

            Agreement.DepositChequeClears = Agreement.AgreementDate + (new TimeSpan(Convert.ToInt32(Country[CountryParameterNames.ChequeDays]), 0, 0, 0));

            Agreement.PaymentMethod = Agreement.PayMethod = paymentMethod;
            Agreement.EmployeeNumChange = this.User;
            Agreement.DateChange = DateTime.Now;
            if (this.AccountType == AT.Cash)
                Agreement.CODFlag = CODFlag;
            else
                Agreement.CODFlag = "";
            Agreement.SalesPerson = salesPerson;
            Agreement.SOA = SOA;
            Agreement.ServiceCharge = serviceCharge;
            Agreement.AgreementTotal = agreementTotal;
            Agreement.Deposit = deposit;
            //Agreement.CashPrice = 
            //	Agreement.AgreementTotal - Agreement.Deposit - Agreement.ServiceCharge - dtTaxAmount - extraCharges;
            Agreement.CashPrice =
                Agreement.AgreementTotal - Agreement.ServiceCharge - dtTaxAmount - extraCharges;

            /* we don't always have line items to save because this routine is also
             * used to create RF sub agreements before lineitems are added */
            if (lineItems != null)
                Agreement.Discount = xml.CalculateDiscount(lineItems);
            else
                Agreement.Discount = 0;

            Agreement.PaymentHolidays = paymentHolidays;
            Agreement.EmployeeNumChange = this.User;

            if (taxExempt)
            {
                Agreement.TaxFree = taxExempt;
            }

            Agreement.Save(conn, trans);    //save agreement record			
                                            /* if paid & taken record the loyalty card no against the 
                                             * customer record so that it shows up on the receipt */
            if (paidAndTaken &&
                (bool)Country[CountryParameterNames.LoyaltyCard])
            {
                BCustomer cust = new BCustomer();
                cust.UpdateLoyaltyCardNo(conn, trans, "PAID & TAKEN", loyaltyCardNo);
            }


            //Build the instal plan for credit accounts only
            if (AT.IsCreditType(accountType))
            {
                InstalPlan = new BInstalPlan();
                InstalPlan.MonthsInterestFree = 0;
                InstalPlan.AccountNumber = accountNumber;
                InstalPlan.AgreementNumber = agreementNo;
                InstalPlan.DateFirst = dateFirst;
                if (months > 0)
                {
                    InstalPlan.DateLast = InstalPlan.DateFirst.AddMonths(months - 1);
                }
                else
                {
                    InstalPlan.DateLast = InstalPlan.DateFirst.AddMonths(months);
                }

                InstalPlan.NumberOfInstalments = months;
                InstalPlan.InstalTotal = agreementTotal;
                InstalPlan.InstalmentFrequency = "M";
                InstalPlan.InstalmentAmount = instalAmount;
                InstalPlan.FinalInstalment = finalInstalment;
                InstalPlan.User = User;
                InstalPlan.DueDay = dueDay;
                InstalPlan.Band = newBand;//aahh
                InstalPlan.autoda = autoda;
                InstalPlan.PrefDay = PrefDay;  
                InstalPlan.Save(conn, trans);   //Save instal plan record  TTT

            }

            //Build the line item rows 
            if (lineItems != null)
            {
                DBranch b = new DBranch();
                int refNo = b.GetTransRefNo(conn, trans, branchNo);     // #16339
                if (paidAndTaken)
                    BuildPTLineItems(conn, trans, lineItems, accountNumber, branchNo, refNo, collection, buffNo, agreementNo, loyaltyCardNo, replacement, giftVoucherValue, courtsVoucher,
                        voucherReference, voucherAuthorisedBy, acctNoCompany, payMethodList, returnAuthorisedBy,/*storeCardAcctNo, storeCardNumber,*/ out storeCardTransRefNo); //IP - 17/01/11 - Added storeCardAcctNo, storeCardNumber and storeCardTransRefNo
                else
                    BuildLineItems(conn, trans, lineItems, accountNumber, branchNo, serviceCharge, isNewAccount, refNo, bankCode, bankAcctNo, chequeNo, payMethod, loyaltyCardNo, agreementNo);

                //BOC Added by Suvidha - CR 2018-13 - 05/01/19 - to update invoice version in table InvoiceDetails.
                if (reviseAccount == true)
                {
                    BItem bitem = new BItem();
                    bitem.UpdateInvoiceVersion(conn, trans, accountNumber, agreementNo);
                }
            }



            // scoring moved to here - after save of lineitems  68538 jec 10/10/06

            /* if agreement total has increased for an existing HP account
			 * un-clear stage1 flag  - may also reset status*/
            if (existingAccount &&
                agreementTotalChanged &&
                accountType != AT.Cash && accountType != AT.Special
                && accountType != AT.ReadyFinance
                && accountType != AT.GoodsOnLoan && //Acct Type Translation DSR 29/9/03
                CustomerID.Length != 0 && reScore)
            {
                DProposalFlag propFlag = new DProposalFlag();
                propFlag.UnClearFlag(conn, trans, accountNumber, SS.S1, false, user);
                propFlag.UnClearFlag(conn, trans, accountNumber, SS.S2, false, user);		/* issue 90 - JJ */
                propFlag.UnClearFlag(conn, trans, accountNumber, SS.DC, false, user);		/* issue 90 - JJ */


                string checkType = "";
                string refCode = "";
                decimal score = 0;
                decimal rfLimit = 0;
                string newAccount = "";

                /* perform an automatic rescore of the proposal */
                BProposal prop = new BProposal();
                prop.User = User;
                int points = 0;
                prop.GetUnclearedStage(conn, trans, accountNumber, ref newAccount, ref checkType, ref dateProp, ref propResult, ref points);

                if (checkType == SS.S1 && points > 0)
                {
                    bool referDeclined = false;
                    // When the account is scored it will re-calculate the scoring band
                    prop.Score(conn, trans, countryCode, accountNumber, accountType, CustomerID, dateProp, branchNo, out newBand, out refCode, out score, out rfLimit, salesPerson, out propResult, out bureauFailure, ref referDeclined, out referralReasons); //IP - 14/03/11 - #3314 - CR1245 - Returning referral reasons
                }
            }

            if ((bool)Country[CountryParameterNames.PrizeVouchersActive] &&
                agreementTotalChanged &&
                !paidAndTaken &&
                !isNewAccount &&
                (propResult == PR.Accepted || propResult == ""))
            {
                BCustomer cust = new BCustomer();
                cust.User = this.User;
                cust.IssueAdditionalPrizeVouchers(conn, trans, accountNumber, Agreement.CashPrice, 0);
            }

            // end scoring




            if (replacementXml != null)
            {
                if ((replacement.TimePeriod != OneForOneTimePeriod.IRPeriod1 && replacement.TimePeriod != OneForOneTimePeriod.IRPeriod2)
                     && bItem.IsItemInstantReplacement(conn, trans, replacement.ItemId, Convert.ToInt32(branchNo))
                     && (accountType == AT.ReadyFinance || accountType == AT.Cash))
                {
                    lineItem.AccountNumber = accountNumber;
                    lineItem.AgreementNumber = agreementNo;
                    lineItem.ItemNumber = "";
                    lineItem.StockLocation = 0;
                    lineItem.ContractNo = replacement.ContractNo;
                    lineItem.LinkWarrantyToItem(conn, trans, replacement.WarrantyID, branchNo);
                }

            }

            #region Warranty Reneweal Processing
            if (warrantyRenewal.Rows.Count > 0)
            {
                DLineItem line = new DLineItem();
                DateTime renewalDateProp = DateTime.Now;

                foreach (DataRow r in warrantyRenewal.Rows)
                {
                    AddCodeToAccount(conn, trans, (string)r[CN.AccountNo], "WRP", this.User, DateTime.Now);
                    CustomerID = (string)r[CN.CustID];

                    line.AccountNumber = accountNumber;
                    line.ItemNumber = Convert.ToString(r[CN.ItemNo]);
                    line.ItemID = Convert.ToInt32(r[CN.ItemId]);
                    line.StockLocation = Convert.ToInt16(r[CN.StockLocn]);
                    line.ContractNo = (string)r[CN.RenewalContractNo];

                    line.LinkWarrantyToItem(conn, trans, Convert.ToInt32(r[CN.NewWarrantyID]), branchNo);          //IP - CR1212 - TODO: Need to replace 0 with correct warrantyID
                }

                bool rescore = false;
                AddCustomerToAccount(conn, trans, accountNumber, CustomerID, "H",
                    accountType, out rescore);

                SaveWarrantyRenewal(conn, trans, accountNumber, branchNo, warrantyRenewal);

                if (AT.IsCreditType(accountType))
                {

                    string checkType = "";
                    string newAccount = "";
                    string source = DASource.Auto; //IP - 04/02/10 - CR1072 - 3.1.9

                    BProposal bProp = new BProposal();
                    bProp.User = User;
                    int points = 0;
                    bProp.GetUnclearedStage(conn, trans, accountNumber, ref newAccount, ref checkType, ref dateProp, ref propResult, ref points);

                    DProposal prop = new DProposal();
                    prop.AccountNo = accountNumber;
                    prop.CustomerID = CustomerID;
                    prop.DateProp = dateProp;
                    prop.DateEmpStart = DateTime.Now;
                    prop.DatePEmpStart = DateTime.Now;
                    prop.BankAccountOpened = DateTime.Now;
                    prop.PropResult = "A";
                    prop.ScoringBand = newBand;
                    prop.AdditionalExpenditure1 = 0;
                    prop.AdditionalExpenditure2 = 0;
                    prop.EmployeeNoChanged = this.User;
                    prop.Save(conn, trans, CustomerID, accountNumber);

                    DProposalFlag flag = new DProposalFlag();
                    flag.User = this.User;
                    flag.CustomerID = CustomerID;
                    flag.DateProp = dateProp;
                    flag.ClearAll(conn, trans, accountNumber);

                    Agreement.User = this.User;
                    Agreement.ClearProposal(conn, trans, accountNumber, source); //IP - 04/02/10 - CR1072 - 3.1.9 - Added Source of Delivery Authorisation.
                }

                this.CurrentStatus = "1";
                this.Save(conn, trans);
            }
            #endregion

            if (variableRates.Rows.Count > 0)
            {
                BInstalPlan instal = new BInstalPlan();
                instal.SaveVariableInstalments(conn, trans, variableRates);
            }

            BCustomer bCust = new BCustomer();
            bCust.SetAvailableSpend(conn, trans, CustomerID);


            if (autoda == true)
            {
                BInstalPlan binstalplan = new BInstalPlan();
                binstalplan.AccountNumber = accountNumber;
                binstalplan.User = User;
                binstalplan.AutoDA(conn, trans);
            }

            //#18603 - CR15594
            if (lineItems != null)
            {
                var xPathRA = "//Item[@Quantity!='0' and @ReadyAssist='True']";
                XmlNode itemRA = lineItems.SelectSingleNode(xPathRA);

                if (itemRA != null)
                {
                    var repository = new AccountRepository();
                    repository.SaveRemoveReadyAssistDetails(conn, trans, readyAssist, accountNumber, agreementNo, readyAssistTermLength, int.Parse(Convert.ToString(itemRA.Attributes[Tags.ItemId].Value)),
                                                                 Convert.ToString(itemRA.Attributes[Tags.ContractNumber].Value));
                }

                var xPathRARemoved = "//Item[@Quantity='0' and @ReadyAssist='True']";
                XmlNodeList itemRARemoved = lineItems.SelectNodes(xPathRARemoved);

                if (itemRARemoved != null)
                {
                    foreach (XmlNode ra in itemRARemoved)
                    {
                        var repository = new AccountRepository();
                        repository.SaveRemoveReadyAssistDetails(conn, trans, readyAssist, accountNumber, agreementNo, readyAssistTermLength, int.Parse(Convert.ToString(ra.Attributes[Tags.ItemId].Value)),
                                                                     Convert.ToString(ra.Attributes[Tags.ContractNumber].Value));
                    }

                }
            }



            return result;
        }



        /// <summary>
        /// call a recursive function to create a lineItem object
        /// and save that line item for each xml ITEM node
        /// If the taxType in exclusive then keep track of the 
        /// quantity that we'll need for the tax record.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="taxType"></param>
        public void BuildLineItems(SqlConnection conn, SqlTransaction trans,
            XmlNode items, string accountNumber,
            short branchNumber, decimal serviceCharge,
            bool isNewAccount, int refNo,
            string bankCode,
            string bankAcctNo, string chequeNo,
            short payMethod, string loyaltyCardNo,
            int agreementNo)
        {
            DDelivery del = new DDelivery();
            BItem item = new BItem();
            decimal transValue = 0;
            string type = "";
            DateTime transDate = DateTime.Now;

            //#region Bit of debugging
            ///* log the number of DT items in the lineitems XML document 
            // * this is to investigate the possibility that we sometimes have
            // * multiple DT nodes in a single account*/
            //try
            //{
            //    if(items.SelectNodes("//Item[@Code='DT']").Count > 1)
            //    {
            //        logMessage("Number of DT items: " + items.SelectNodes("//Item[@Code='DT']").Count, User.ToString(), System.Diagnostics.EventLogEntryType.Warning);

            //        string outFile = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\STL\\Courts.NET\\Server\\DTTrace.xml";
            //        XmlDocument lineXML = new XmlDocument();	

            //        FileInfo fi = new FileInfo(outFile);
            //        if(fi.Exists)
            //            fi.Delete();

            //        FileStream fs = fi.Create();
            //        lineXML.LoadXml(items.OuterXml);
            //        lineXML.Save(fs);
            //        fs.Close();
            //    }
            //}
            //catch(Exception ex){/*just make sure this can't cause a problem*/}
            //#endregion

            if (!CurrentStatus.Equals("6"))
            {
                //might need to update the account status when the user enters 
                //line items
                this.UpdateStatus(conn, trans, accountNumber);
            }

            #region Warranties on Credit Processing
            ///If this is an account with Warranties on Credit and an attached
            ///customer then we need to deliver everything undelivered and 
            ///process a payment for anything we deliver that isn't a warranty			
            if (TT.IsPaidAndTaken(TermsType) &&
                CustomerID.Length > 0 &&
                AccountType == AT.Special)
            {
                decimal deliveredVal = 0;
                decimal paymentVal = 0;

                //BBranch b = new BBranch();
                //int buffNo = b.GetBuffNo(branchNumber);
                int buffNo = agreementNo;

                ///get a node list of all items
                XmlNodeList itemsList = items.SelectNodes("//Item");
                foreach (XmlNode itemNode in itemsList)
                {
                    bool valueControlled = Convert.ToBoolean(itemNode.Attributes[Tags.ValueControlled].Value);
                    decimal diff = 0;
                    decimal delValue = 0;
                    decimal newValue = 0;
                    double dQuantity = 0;

                    // rdb this needs testing
                    //string parentItemNo = string.Empty;
                    int parentItemID = 0;                   //IP - 17/05/11 - CR1212 - #3627
                    if (itemNode.ParentNode.ParentNode != null && itemNode.ParentNode.ParentNode.Name == "Item")
                        parentItemID = Convert.ToInt32(itemNode.ParentNode.ParentNode.Value);         //IP - 17/05/11 - CR1212 - #3627
                                                                                                      //parentItemNo = itemNode.ParentNode.ParentNode.Value;

                    if (valueControlled ||
                        Convert.ToInt32(itemNode.Attributes[Tags.ItemId].Value).In(
                            StockItemCache.Get(StockItemKeys.DT),
                            StockItemCache.Get(StockItemKeys.InsuranceChargeItem),
                            StockItemCache.Get(StockItemKeys.AdminChargeItem)
                        ))		///deliver difference in value
                    {
                        delValue = del.GetDeliveredValue(conn, trans,
                            accountNumber, agreementNo,
                            Convert.ToInt32(itemNode.Attributes[Tags.ItemId].Value),                         //IP - 17/05/11 - CR1212 - #3627 - changed to use ItemID
                            Convert.ToInt16(itemNode.Attributes[Tags.Location].Value),
                            itemNode.Attributes[Tags.ContractNumber].Value, parentItemID);                  //IP - 17/05/11 - CR1212 - #3627 - changed to use parentItemID rather than parentItemNo
                        newValue = Convert.ToDecimal(itemNode.Attributes[Tags.Value].Value);
                        diff = newValue - delValue;
                        if (Math.Abs(diff) > 0.01M)
                        {
                            // DSR UAT 181: The quantity is always 1 for a delivery or -1 for a collection
                            // This has to work for discounts that deliver a negative amount
                            if (delValue == 0)
                            {
                                // Nothing yet delivered so this cannot be a collection
                                dQuantity = 1;
                            }
                            else if (delValue > 0)
                            {
                                if (newValue < delValue) dQuantity = -1;	// Collection
                                else dQuantity = 1;							// Delivery
                            }
                            else  // delValue < 0
                            {
                                if (newValue > delValue) dQuantity = -1;	// Collection
                                else dQuantity = 1;							// Delivery
                            }

                            ///Deliver the difference
                            new BDelivery(conn, trans, accountNumber, dQuantity, branchNumber, User, itemNode, refNo, diff, buffNo, agreementNo);

                            ///Keep a tally
                            deliveredVal += diff;
                            if ((itemNode.Attributes[Tags.Type].Value != IT.Warranty &&
                                itemNode.Attributes[Tags.Type].Value != IT.KitWarranty) ||
                                TermsType == TT.PTWarranty)
                                paymentVal += diff;

                        }
                    }
                    else					///deliver difference in quantity
                    {
                        del.GetDeliveredQuantity(conn, trans,
                            accountNumber, agreementNo,
                            Convert.ToInt32(itemNode.Attributes[Tags.ItemId].Value),                        //IP - 17/05/11 - CR1212 - #3627 - changed to use ItemID      
                            Convert.ToInt16(itemNode.Attributes[Tags.Location].Value),
                            itemNode.Attributes[Tags.ContractNumber].Value, parentItemID);                  //IP - 17/05/11 - CR1212 - #3627 - changed to use parentItemID rather than parentItemNo
                        diff = Convert.ToDecimal(Convert.ToDouble(itemNode.Attributes[Tags.Quantity].Value) - del.DeliveredQuantity);
                        if (Math.Abs(diff) > 0)
                        {
                            ///Deliver the difference
                            new BDelivery(conn, trans, accountNumber, Convert.ToDouble(diff), branchNumber, User, itemNode, refNo, buffNo, agreementNo);

                            ///Keep a tally
                            deliveredVal += Convert.ToDecimal(itemNode.Attributes[Tags.UnitPrice].Value) * diff;
                            if ((itemNode.Attributes[Tags.Type].Value != IT.Warranty &&
                                itemNode.Attributes[Tags.Type].Value != IT.KitWarranty) ||
                                TermsType == TT.PTWarranty)
                                paymentVal += Convert.ToDecimal(itemNode.Attributes[Tags.UnitPrice].Value) * diff;

                        }
                    }
                }
                ///Write fintrans records for the total amount delivered and the total
                ///payment made
                if (Math.Abs(deliveredVal) > 0.01M)
                {
                    type = deliveredVal > 0 ? "DEL" : "GRT";
                    BTransaction t = new BTransaction(conn, trans, accountNumber, branchNumber, refNo, deliveredVal, User, type, "", "", "", 0, (string)Country[CountryParameterNames.CountryCode], transDate, "", agreementNo);

                    if ((bool)Country[CountryParameterNames.LoyaltyCard])
                        t.SaveLoyaltyCardRecord(conn, trans, refNo, transDate, accountNumber, loyaltyCardNo, agreementNo);
                }

                if (Math.Abs(paymentVal) > 0.01M)
                {
                    type = paymentVal > 0 ? "PAY" : "REF";
                    paymentVal *= -1;
                    new BTransaction(conn, trans, accountNumber, branchNumber, refNo, paymentVal, User, type, bankCode, bankAcctNo, chequeNo, payMethod, (string)Country[CountryParameterNames.CountryCode], transDate, "", agreementNo);
                }
            }
            #endregion

            //CR795
            string xPath = "//Item[@ReplacementItem = 'True']";
            XmlNode replacement = items.SelectSingleNode(xPath);
            if (replacement != null)
            {
                decimal deliveryVal = Convert.ToDecimal(replacement.Attributes[Tags.Value].Value);
                double qty = Convert.ToDouble(replacement.Attributes[Tags.Quantity].Value);

                if (Math.Abs(deliveryVal) > 0.01M)
                {
                    if (!NotifyReplacement)
                    {
                        BBranch b = new BBranch();
                        int buffNumber = b.GetBuffNo(conn, trans, branchNumber);

                        new BDelivery(conn, trans, accountNumber, qty, branchNumber, User, replacement, refNo, deliveryVal, buffNumber, agreementNo);
                        BTransaction t = new BTransaction(conn, trans, accountNumber, branchNumber, refNo, deliveryVal, User, "DEL", "", "", "", 0, (string)Country[CountryParameterNames.CountryCode], transDate, "", agreementNo);
                    }
                }
            }

            //Delete all the line items first
            item.DeleteAllLineItems(conn, trans, accountNumber, agreementNo);

            //build all the normal line items

            BuildLineItems(conn, trans, items, accountNumber, branchNumber, refNo, agreementNo, isNewAccount, ref transValue);

            // Now we have to audit those non stocks which have been removed which where not being audited. 
            DLineItem lineItem = new DLineItem();
            lineItem.AccountNumber = accountNumber;
            lineItem.AgreementNumber = agreementNo;
            lineItem.User = User;
            lineItem.LineItemAuditRemovedNonStocks(conn, trans); //Need to audit changes to removed non stocks. 
                                                                 /* write a single fintrans record for any deliveries which we've made */

            //We need to drop the ##templineitems table at this point
            DropTempLineItems(conn, trans, accountNumber, agreementNo);

            /* write a single fintrans record for any deliveries which we've made */
            if (Math.Abs(transValue) > 0.01M)
            {
                type = transValue > 0 ? "DEL" : "GRT";
                BTransaction t = new BTransaction(conn, trans, accountNumber, branchNumber, refNo, transValue, User, type, "", "", "", 0, (string)Country[CountryParameterNames.CountryCode], transDate, "REVN", agreementNo);
            }
        }

        public void DropTempLineItems(SqlConnection conn, SqlTransaction trans,
            string accountNumber, int agreementNo)
        {
            BItem item = new BItem();
            item.DropTempLineItems(conn, trans, accountNumber, agreementNo);
        }

        private void BuildLineItems(SqlConnection conn, SqlTransaction trans,
            XmlNode items, string accountNumber,
            short branchNo, int refNo,
            int agreementNo, bool isNewAccount,
            ref decimal transValue)
        {
            XmlNode parent = null;
            /* loop through the Xml document recursively to 
             * process all lineitems */
            foreach (XmlNode item in items.ChildNodes)
            {
                if (item.NodeType == XmlNodeType.Element && item.Name == Tags.Item)
                {
                    BItem lineItem = new BItem();
                    lineItem.User = User;

                    //Check to see if there is a parent item and if there is record the details
                    if (item.ParentNode.ParentNode != null)
                    {
                        parent = item.ParentNode.ParentNode;

                        if (item.Attributes[Tags.ParentItemId] != null && item.Attributes[Tags.Quantity].Value == "0" && item.Attributes[Tags.ParentItemId].Value == "0")
                        {
                            lineItem.ParentItemNumber = item.Attributes[Tags.Code].Value;
                            lineItem.ParentItemId = Convert.ToInt32(item.Attributes[Tags.ParentItemId].Value);
                            lineItem.ParentStockLocation = Convert.ToInt16(item.Attributes[Tags.Location].Value);
                        }
                        else
                        {
                            lineItem.ParentItemNumber = parent.Attributes[Tags.Code].Value;
                            lineItem.ParentItemId = Convert.ToInt32(parent.Attributes[Tags.ItemId].Value);
                            lineItem.ParentStockLocation = Convert.ToInt16(parent.Attributes[Tags.Location].Value);
                        }

                    }

                    //Set up the basic variables for the line item
                    lineItem.OrigBr = 0;
                    lineItem.AccountNumber = accountNumber;
                    lineItem.AgreementNumber = agreementNo;
                    lineItem.ItemNumber = item.Attributes[Tags.Code].Value;
                    lineItem.ItemId = Convert.ToInt32(item.Attributes[Tags.ItemId].Value);
                    lineItem.ItemSuppText = "";
                    lineItem.Quantity = Convert.ToDouble(item.Attributes[Tags.Quantity].Value);
                    lineItem.DeliveredQuantity = Convert.ToDouble(item.Attributes[Tags.DeliveredQuantity].Value);
                    lineItem.StockLocation = Convert.ToInt16(item.Attributes[Tags.Location].Value);
                    lineItem.Price = Convert.ToDecimal(StripCurrency(item.Attributes[Tags.UnitPrice].Value));
                    lineItem.OrderValue = Convert.ToDecimal(StripCurrency(item.Attributes[Tags.Value].Value));
                    lineItem.DateRequiredDelivery = Convert.ToDateTime(item.Attributes[Tags.DeliveryDate].Value);
                    lineItem.TimeRequiredDelivery = item.Attributes[Tags.DeliveryTime].Value;
                    lineItem.DeliveryNoteBranch = Convert.ToInt16(item.Attributes[Tags.BranchForDeliveryNote].Value);
                    //lineItem.QuantityDiff = lineItem.Quantity==lineItem.DeliveredQuantity ? "N":"Y";
                    lineItem.QuantityDiff = item.Attributes[Tags.QuantityDiff].Value;
                    lineItem.ScheduledQuantity = Convert.ToDouble(item.Attributes[Tags.ScheduledQuantity].Value);
                    lineItem.ContractNo = item.Attributes[Tags.ContractNumber].Value;

                    switch (item.Attributes[Tags.Type].Value)
                    {
                        case IT.Stock:
                            lineItem.ItemType = "S"; this.HasLineItems = true;
                            break;
                        case IT.Component:
                            lineItem.ItemType = "S";
                            break;
                        default:
                            lineItem.ItemType = "N";
                            break;
                    }
                    lineItem.IsKit = Convert.ToInt16(item.Attributes[Tags.Type].Value == IT.Kit ? 1 : 0);
                    lineItem.HasString = 0;
                    lineItem.Notes = item.Attributes[Tags.ColourTrim].Value;
                    lineItem.CalculateTaxAmount(item, IsTaxExempt(conn, trans, accountNumber, agreementNo.ToString()));
                    lineItem.DeliveryAddress = item.Attributes[Tags.DeliveryAddress].Value;
                    lineItem.ExpectedReturnDate = item.Attributes[Tags.ExpectedReturnDate].Value == "" ? DateTime.MinValue.AddYears(1899) : Convert.ToDateTime(item.Attributes[Tags.ExpectedReturnDate].Value);
                    lineItem.DeliveryArea = item.Attributes[Tags.DeliveryArea].Value;
                    lineItem.DeliveryProcess = item.Attributes[Tags.DeliveryProcess].Value;
                    lineItem.Damaged = item.Attributes[Tags.Damaged].Value;
                    lineItem.Assembly = item.Attributes[Tags.Assembly].Value;
                    lineItem.Taxrate = Convert.ToDouble(item.Attributes[Tags.TaxRate].Value);
                    lineItem.SalesBrnNo = Convert.ToString(item.Attributes[Tags.SalesBrnNo].Value) != string.Empty ? Convert.ToInt16(item.Attributes[Tags.SalesBrnNo].Value) : (Int16?)null;         //IP - 23/05/11 - CR1212 - RI - #3651
                    lineItem.Express = item.Attributes[Tags.Express].Value;         //IP - 07/06/12 - #10229 - Warehouse & Deliveries


                    if (isNewAccount)
                        lineItem.AuditSource = AS.NewAccount;
                    else
                        lineItem.AuditSource = AS.Revise;

                    if (lineItem.ItemNumber == "SD" && AccountType == AT.ReadyFinance && isNewAccount)
                    {
                        if (items.ChildNodes.Count == 1)
                        {
                            lineItem.AuditSource = AS.Revise;
                        }
                        else
                        {
                            lineItem.UpdateAuditItem(conn, trans);
                        }
                    }

                    //Write this item to the database
                    lineItem.Save(conn, trans);

                    /* JJ 8/4/4 moved from further up the method so that
                     * deliveries happen after lineitem is saved */
                    if (item.Attributes[Tags.Type].Value != IT.Stock &&
                        item.Attributes[Tags.Type].Value != IT.Component &&
                        item.Attributes[Tags.Type].Value != IT.Kit &&
                        item.Attributes[Tags.Type].Value != IT.Warranty &&
                        item.Attributes[Tags.ReadyAssist] != null &&
                        Convert.ToBoolean(item.Attributes[Tags.ReadyAssist].Value == "" ? "false" : item.Attributes[Tags.ReadyAssist].Value) != true) //#19689        //#19459
                    /* || item.Attributes[Tags.FreeGift].Value == Boolean.TrueString ) add this when we're ready */
                    {	/* include free gifts in the BuildNonStock processing */
                        //if non-stockitem then stock location is left three digits of account number
                        //removed for time being as doing this on newaccount screen
                        BuildNonStock(conn, trans, parent, item, lineItem, accountNumber, branchNo, refNo, (string)Country[CountryParameterNames.CountryCode], ref transValue, agreementNo);
                    }
                    else
                    {
                        /* for all stock items except kits */
                        if (item.Attributes[Tags.Type].Value != IT.Kit &&
                            (item.Attributes[Tags.Type].Value == IT.Stock ||
                            item.Attributes[Tags.Type].Value == IT.Component))
                        {
                            /* compare new qty with old qty and update stock accordingly */
                            bool isPurchaseOrder = Convert.ToBoolean(item.Attributes[Tags.PurchaseOrder].Value);
                            if (!isPurchaseOrder)
                                MaintainStockLevel(conn, trans, accountNumber, item, lineItem, false, agreementNo);
                            else
                                MaintainPurchaseOrderStockLevel(conn, trans, accountNumber, item, lineItem, false, agreementNo);

                            // 68421 RD/PN 07/08/06 No removed call to DN_ScheduleAlignItemsSP as removing collection notes incorretly
                            /*
                             // 67980 RD 28/03/06 Modified to enusre that warranty items are not removed from schedule table.
                            if(Convert.ToDouble(item.Attributes[Tags.Quantity].Value) > 0 && item.Attributes[Tags.Type].Value!=IT.Warranty)
                            {
                                DSchedule sched = new DSchedule();
                                sched.AlignScheduledLineItems(conn, trans, accountNumber, agreementNo,
                                    item.Attributes[Tags.Code].Value,
                                    Convert.ToInt16(item.Attributes[Tags.Location].Value),
                                    Convert.ToDouble(item.Attributes[Tags.Quantity].Value));
                            }
                            */
                        }
                    }

                    //if it's a kit put it in the KitLineItem and KitCLineItem tables
                    //This is purely to satisfy OpenRoad invoice printing and can be 
                    //removed when OpenRoad goes.
                    if (item.Attributes[Tags.Type].Value == IT.Kit)
                    {
                        lineItem.SaveKit(conn, trans, item);
                    }

                    //if we have any related items, then recurse
                    foreach (XmlNode child in item)
                        if (child.NodeType == XmlNodeType.Element && child.HasChildNodes)
                        {
                            BuildLineItems(conn, trans, child, accountNumber, branchNo, refNo, agreementNo, isNewAccount, ref transValue);
                        }
                }

            }

        }

        private void MaintainStockLevel(SqlConnection conn, SqlTransaction trans,
            string accountNo,
            XmlNode item, BItem lineItem,
            bool collection, int agreementNo)
        {
            lineItem.AccountNumber = accountNo;
            lineItem.ItemNumber = item.Attributes[Tags.Code].Value;
            lineItem.ItemId = Convert.ToInt32(item.Attributes[Tags.ItemId].Value);              //IP - 20/05/11 - CR1212 - RI - #3664
            lineItem.StockLocation = Convert.ToInt16(item.Attributes[Tags.Location].Value);
            lineItem.Quantity = Convert.ToDouble(item.Attributes[Tags.Quantity].Value);
            if (collection)
                lineItem.Quantity *= -1;
            lineItem.AgreementNumber = agreementNo;
            lineItem.MaintainStockLevel(conn, trans);
        }


        /// <summary>
        /// this method builds the line items for the paid and taken account
        /// It is different from normal accounts because the line items should be updated
        /// rather than deleted and inserted.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        /// <param name="items"></param>
        /// <param name="taxType"></param>
        /// <param name="accountNumber"></param>
        /// <param name="branchNumber"></param>
        /// <param name="taxAmount"></param>
        private void BuildPTLineItems(SqlConnection conn, SqlTransaction trans, XmlNode items,
            string accountNumber, short branchNumber,
            int refNo, bool collection,
            //string bankCode, string bankAcctNo, string chequeNo, short payMethod,
            int buffNo, int agreementNo, string loyaltyCardNo,
            InstantReplacementDetails replacement,
            //decimal localTender,
            decimal giftVoucherValue, bool courtsVoucher,
            string voucherReference, int voucherAuthorisedBy,
            string acctNoCompany, DataTable payMethodList,
            int returnAuthorisedBy, // string storeCardAcctno, long? storeCardNumber, Removed as in array and can be more than one storecard used for paying account (in theory). 
            out int storeCardTransRefNo) //IP - 17/01/11 - Added storeCardTransRefNo
        {
            //build all the normal line items
            decimal deliveredVal = 0;
            decimal paymentVal = 0;
            decimal replacementVal = 0;
            decimal replacementWarrantyVal = 0;
            string type = "";
            string bankCode = ""; string bankAcctNo = ""; string chequeNo = ""; short payMethod = 0;
            storeCardTransRefNo = 0;        //IP - 17/01/11 - Store Card

            DateTime transDate = DateTime.Now;
            BTransaction t = new BTransaction();

            //update the line items
            BuildPTLineItems(conn, trans, items, accountNumber, branchNumber, refNo, ref deliveredVal, ref paymentVal, collection, buffNo, agreementNo, ref replacementVal, ref replacementWarrantyVal, replacement == null ? 0 : Convert.ToDecimal(replacement.Quantity));    // #17290 

            // Use the bank details from a pay method with these details
            foreach (DataRow row in payMethodList.Rows)
            {
                if (bankCode == "" && row[CN.BankCode].ToString().Trim().Length > 0)
                {
                    bankCode = row[CN.BankCode].ToString().Trim();
                    bankAcctNo = row[CN.BankAccountNo].ToString().Trim();
                    chequeNo = row[CN.ChequeNo].ToString().Trim();
                    payMethod = System.Convert.ToInt16(row[CN.PayMethod]);
                }
            }
            //write the delivery record to the fintrans table
            //and the delivery table
            if (deliveredVal != 0)
            {

                type = deliveredVal > 0 ? "DEL" : "GRT";
                t = new BTransaction(conn, trans, accountNumber, branchNumber, refNo, deliveredVal, User, type, bankCode, bankAcctNo, chequeNo, payMethod, (string)Country[CountryParameterNames.CountryCode], transDate, "", agreementNo);

                /* if required write a loyalty card record */
                if ((bool)Country[CountryParameterNames.LoyaltyCard])
                    t.SaveLoyaltyCardRecord(conn, trans, refNo, transDate, accountNumber, loyaltyCardNo, agreementNo);

                if (type == TransType.GoodsReturn)
                {
                    DTransactionAudit ta = new DTransactionAudit();
                    ta.Write(conn, trans, accountNumber, this.User, returnAuthorisedBy, deliveredVal, deliveredVal, transDate, refNo, type);
                }
            }

            if (paymentVal != 0)
            {
                /* JJ - we need to take into account whether this is a replacement */
                /* if it is it has already been at least partially paid for ! */
                if (replacement != null)
                {
                    //paymentVal = replacementVal - (replacement.Price * replacement.Quantity);
                    paymentVal = replacementVal - (replacement.OrderValue + replacement.TaxAmount);

                    // LiveWire Issue 68077 - A new warranty could have been added
                    // when processing an instant replacement, so we must make
                    // sure we write a fintrans record for the payment of the new
                    // warranty.
                    paymentVal += replacementWarrantyVal;

                    if (paymentVal < 0)
                        paymentVal = 0;
                }

                /* need to account for full or part payment by gift voucher before posting 
                 * any remaining payment transaction */
                if (giftVoucherValue != 0)
                {
                    int refNoRedeemed = 0;

                    if (giftVoucherValue > paymentVal)		/* ensure the transfer never exceeds the payment amount */
                        giftVoucherValue = paymentVal;

                    paymentVal -= giftVoucherValue;
                    paymentVal = paymentVal < 0 ? 0 : paymentVal;

                    DGiftVoucher gv = new DGiftVoucher();
                    gv.Populate(conn, trans, voucherReference, courtsVoucher, acctNoCompany);

                    string voucherAcct = courtsVoucher ? gv.AccountNoSold : acctNoCompany;
                    /* verify that the account exists */

                    DAccount acct = new DAccount();
                    acct.Validate(conn, trans, voucherAcct);
                    if (Convert.ToBoolean(acct.AccountExists))
                    {
                        /* write a transfer record */
                        refNoRedeemed = t.TransferTransaction(conn, trans, voucherAcct, accountNumber, TransType.Transfer, giftVoucherValue, branchNumber, (string)Country[CountryParameterNames.CountryCode], transDate, "", 0, PayMethod.GiftVoucher, agreementNo, null); //IP - 29/11/10 - Store Card - added agreementNo //IP - 30/11/10 - added storeCadrNo 
                        if (gv.Free)		/* if it was a free gift voucher journal it off */
                        {
                            DBranch b = new DBranch();
                            int jgfRef = b.GetTransRefNo(conn, trans, branchNumber);
                            t = new BTransaction(conn, trans, gv.AccountNoSold, branchNumber, jgfRef, -gv.Value, User, TransType.GiftVoucherJournal, "", "", "", 0, (string)Country[CountryParameterNames.CountryCode], transDate, "", 0);
                        }
                    }
                    else
                    {
                        throw new STLException(GetResource("M_NOGIFTVOUCHERACCOUNT", new object[] { voucherAcct }));
                    }

                    /* update the gift voucher record to show it has been redeemed */
                    gv.Redeem(conn, trans, voucherReference, courtsVoucher, User, DateTime.Now, voucherAuthorisedBy, accountNumber, acctNoCompany, refNoRedeemed);
                }

                if (paymentVal != 0)
                {
                    // Cash and Go can enter multiple pay methods
                    // paymentVal is the amount due, to be recorded on Fintrans as one or more
                    // pay methods. If change was given then this will always be against the last
                    // pay method in the list, and the amount of the last pay method must be 
                    // reduced to only pay the amount due overall.
                    decimal paymentTotal = 0;
                    decimal curPayment = 0;
                    decimal localTender = 0;
                    decimal localChange = 0;
                    foreach (DataRow row in payMethodList.Rows)
                    {
                        // Write each payment record to the fintrans table with a new trans refNo
                        refNo = (new DBranch()).GetTransRefNo(conn, trans, branchNumber);       // #16339
                        type = paymentVal > 0 ? "PAY" : "REF";

                        if (type == "PAY")
                        {
                            // Payment - Check for any change
                            curPayment = System.Convert.ToDecimal(row[CN.Value]);
                            localTender = curPayment;
                            localChange = 0;
                            paymentTotal = paymentTotal + curPayment;
                            if (paymentTotal > paymentVal)
                            {
                                localChange = paymentTotal - paymentVal;
                                curPayment = curPayment - localChange;
                            }
                        }
                        else
                        {
                            // Refund - No change
                            curPayment = -System.Convert.ToDecimal(row[CN.Value]);
                            localTender = curPayment;
                            localChange = 0;
                        }

                        //IP - 30/11/10 - Store Card - If the payment method is a store card then process using TransferTransaction else as normal.
                        if (row[CN.CodeDescript].ToString() == "StoreCard")
                        {
                            // retrieve account number from payment card number 
                            var ScRepos = new StoreCardRepository();
                            long storecardno = 0;
                            if (row[CN.BankAccountNo].ToString().Trim() != string.Empty && Int64.TryParse(row[CN.BankAccountNo].ToString().Trim(), out storecardno))
                            {
                                storeCardTransRefNo = t.TransferTransaction(conn, trans, row[CN.ChequeNo].ToString().Trim(), accountNumber, curPayment >= 0 ? TransType.StoreCardPayment : TransType.StoreCardRefund, curPayment, branchNumber, (string)Country[CountryParameterNames.CountryCode], transDate, string.Empty, 0, System.Convert.ToInt16(row[CN.PayMethod]), agreementNo, storecardno == 0 ? (long?)null : (long?)storecardno); //IP - 30/11/10 - Store Card - Added StoreCardNumber
                            }
                            else
                            {
                                var SCP = ScRepos.Get(Convert.ToInt64(row[CN.ChequeNo]), conn, trans);
                                storeCardTransRefNo = t.TransferTransaction(conn, trans, SCP.Acctno, accountNumber, curPayment >= 0 ? TransType.StoreCardPayment : TransType.StoreCardRefund, curPayment, branchNumber, (string)Country[CountryParameterNames.CountryCode], transDate, string.Empty, 0, System.Convert.ToInt16(row[CN.PayMethod]), agreementNo, SCP.CardNumber); //IP - 30/11/10 - Store Card - Added StoreCardNumber
                                ScRepos.RecalculateAvailable(SCP.CardNumber, conn, trans);
                            }

                        }
                        else
                        {

                            new BTransaction(conn, trans, accountNumber, branchNumber, refNo, -curPayment, User, type,
                                row[CN.BankCode].ToString(), row[CN.BankAccountNo].ToString(), row[CN.ChequeNo].ToString(),
                                System.Convert.ToInt16(row[CN.PayMethod]), (string)Country[CountryParameterNames.CountryCode], transDate, "",
                                agreementNo);
                        }

                        // Save an Exchange transaction if a foreign currency was taken
                        if (PayMethod.IsForeignCurrencyPayMethod(System.Convert.ToInt16(row[CN.PayMethod])))
                        {
                            BPayment Payment = new BPayment();
                            Payment.SaveExchangeTransaction(conn, trans, accountNumber, refNo, transDate, System.Convert.ToInt16(row[CN.PayMethod]), localTender, localChange, branchNumber);
                        }
                    }
                }
            }
        }



        public void SaveAcctNoCtrl(SqlConnection conn, SqlTransaction trans,
            int branchNo, string acctCat, string acctCatDesc, int hiAllocated, int hiAllowed)
        {
            DAcctNoCtrl dCtrl = new DAcctNoCtrl();
            dCtrl.BranchNo = branchNo;
            dCtrl.AcctCat = acctCat;
            dCtrl.AcctCatDesc = acctCatDesc;
            dCtrl.HiAllocated = hiAllocated;
            dCtrl.HiAllowed = hiAllowed;
            dCtrl.Save(conn, trans);
        }

        private void BuildPTLineItems(SqlConnection conn, SqlTransaction trans,
            XmlNode items, string accountNumber,
            short branchNo, int refNo, ref decimal deliveredVal,
            ref decimal paymentVal, bool collection,
            int buffNo, int agreementNo,
            ref decimal replacementVal, ref decimal replacementWarrantyVal, decimal replacementQuantity = 0)          // #17290
        {
            decimal oldQty = 0;
            decimal newQty = 0;
            decimal newValue = 0;
            decimal oldValue = 0;
            bool valueControlled = false;

            XmlNode parent = null;
            foreach (XmlNode item in items.ChildNodes)
            {
                //uat363 add parentItemNo
                string parentItemNo = string.Empty;
                int parentItemId = 0;           // RI jec 21/06/11
                if (item.ParentNode.ParentNode != null && item.ParentNode.ParentNode.Name == "Item")
                {
                    parentItemNo = item.ParentNode.ParentNode.Attributes["Code"].Value;
                    parentItemId = Convert.ToInt32(item.ParentNode.ParentNode.Attributes[Tags.ItemId].Value);        // RI jec 21/06/11
                }

                if (item.NodeType == XmlNodeType.Element && item.Name == Tags.Item)
                {
                    BItem lineItem = new BItem();

                    //Check to see if there is a parent item and if there is record the details
                    if (item.ParentNode.ParentNode != null)
                    {
                        parent = item.ParentNode.ParentNode;
                        lineItem.ParentItemNumber = parent.Attributes[Tags.Code].Value;
                        lineItem.ParentItemId = Convert.ToInt32(parent.Attributes[Tags.ItemId].Value);
                        lineItem.ParentStockLocation = Convert.ToInt16(parent.Attributes[Tags.Location].Value);
                    }

                    /* for all stock items except kits */
                    if (item.Attributes[Tags.Type].Value != IT.Kit &&
                        (item.Attributes[Tags.Type].Value == IT.Stock ||
                        item.Attributes[Tags.Type].Value == IT.Component))
                    {
                        /* compare new qty with old qty and update stock accordingly */
                        MaintainStockLevel(conn, trans, accountNumber, item, lineItem, collection, agreementNo);
                    }

                    /* for collections we need to update the qty rather than 
                     * insert it. If it's value controlled update the value */
                    newQty = Convert.ToDecimal(item.Attributes[Tags.Quantity].Value);
                    newValue = Convert.ToDecimal(StripCurrency(item.Attributes[Tags.Value].Value));
                    valueControlled = Convert.ToBoolean(item.Attributes[Tags.ValueControlled].Value);

                    if (collection) //#15926 - Cash & Go Warranties dealt with in WarrantyRepository)
                    {
                        if (newQty != 0)  //#18087
                        {
                            // Cash & Go Return is opposite to the approach for Revise Agreement
                            // The order items that are NOT being refunded have been removed from the
                            // Line Items on display and so have a zero quantity and order value!
                            // The items being refunded will have a quantity and order value for the
                            // amount to refund.
                            // Note that line items are being updated (not deleted and inserted).
                            // Note that the SProc that updates the line item adds to the quantity
                            // already on the line item (it does not overwrite it).

                            if (item.Attributes[Tags.Type].Value != IT.Warranty) //#15926 - Cash & Go Warranties dealt with in WarrantyRepository
                            {
                                if (valueControlled)
                                {
                                    oldValue = lineItem.GetItemValue(conn, trans, accountNumber,
                                        agreementNo, branchNo, Convert.ToInt32(item.Attributes[Tags.ItemId].Value),     //IP/NM - 18/05/11 -CR1212 - #3627
                                        item.Attributes[Tags.ContractNumber].Value,
                                        true); // 'true' means get from LineItem instead of LineItem_Amend

                                    // When newValue is zero nothing is being refunded and the old value is unchanged.
                                    lineItem.OrderValue = oldValue - newValue;
                                    item.Attributes[Tags.UnitPrice].Value = lineItem.OrderValue.ToString();
                                    if (lineItem.OrderValue <= 0.01M)               // #9854 
                                    {
                                        lineItem.Quantity = -1; //#18087 // Reduce qty to zero    // #9854 
                                        item.Attributes[Tags.Quantity].Value = "0";
                                        newValue = newValue + lineItem.OrderValue;          // #9854
                                        lineItem.OrderValue -= lineItem.OrderValue;         // #9854 
                                        item.Attributes[Tags.UnitPrice].Value = lineItem.OrderValue.ToString();         // #9854

                                    }
                                    else
                                    {
                                        lineItem.Quantity = -1; //#18087 // No change to the qty
                                        item.Attributes[Tags.Quantity].Value = "1";
                                    }
                                }
                                else
                                {
                                    //#18526
                                    XmlNode parentItemIdAttr = item.Attributes[Tags.ParentItemId];
                                    var parentItemID = parentItemIdAttr == null ? 0 : int.Parse(item.Attributes[Tags.ParentItemId].Value);

                                    oldQty = Convert.ToDecimal(lineItem.GetItemQuantity(conn, trans, accountNumber,
                                        agreementNo, branchNo, Convert.ToInt32(item.Attributes[Tags.ItemId].Value),
                                        parentItemID,   //#18526
                                                        //Convert.ToInt32(item.Attributes[Tags.ParentItemId].Value),              // jec 30/03/12
                                        item.Attributes[Tags.ContractNumber].Value,
                                        true)); // 'true' means get from LineItem instead of LineItem_Amend

                                    // When newQty is zero nothing is being refunded and the old qty is unchanged.
                                    lineItem.Quantity = Convert.ToDouble(-newQty);
                                    lineItem.OrderValue = (oldQty - newQty) * Convert.ToDecimal(StripCurrency(item.Attributes[Tags.UnitPrice].Value));
                                    item.Attributes[Tags.Quantity].Value = Convert.ToString(oldQty - newQty);
                                }
                                // Update the tax amount
                                lineItem.CalculateTaxAmount(item, IsTaxExempt(conn, trans, accountNumber, agreementNo.ToString()));
                            }
                        }
                        else  //#18087 Cash & Go Return - items removed i.e. not being collected 
                        {
                            oldQty = Convert.ToDecimal(lineItem.GetItemQuantity(conn, trans, accountNumber,//#18087
                              agreementNo, branchNo, Convert.ToInt32(item.Attributes[Tags.ItemId].Value),
                              Convert.ToInt32(item.Attributes[Tags.ParentItemId].Value),              // jec 30/03/12
                              item.Attributes[Tags.ContractNumber].Value,
                              true)); // 'true' means get from LineItem instead of LineItem_Amend

                            lineItem.TaxAmount = Convert.ToDouble(StripCurrency(item.Attributes[Tags.TaxAmount].Value));
                            lineItem.OrderValue = (oldQty - newQty) * Convert.ToDecimal(StripCurrency(item.Attributes[Tags.UnitPrice].Value));
                        }
                    }
                    else
                    {
                        lineItem.Quantity = Convert.ToDouble(item.Attributes[Tags.Quantity].Value);
                        lineItem.OrderValue = Convert.ToDecimal(StripCurrency(item.Attributes[Tags.Value].Value));
                        lineItem.CalculateTaxAmount(item, IsTaxExempt(conn, trans, accountNumber, agreementNo.ToString()));
                    }

                    lineItem.OrigBr = 0;
                    lineItem.AccountNumber = accountNumber;
                    lineItem.ItemNumber = item.Attributes[Tags.Code].Value;
                    lineItem.ItemId = Convert.ToInt32(item.Attributes[Tags.ItemId].Value);
                    lineItem.ItemSuppText = "";
                    lineItem.StockLocation = Convert.ToInt16(item.Attributes[Tags.Location].Value);
                    lineItem.Price = Convert.ToDecimal(StripCurrency(item.Attributes[Tags.UnitPrice].Value));
                    lineItem.DateRequiredDelivery = DateTime.MinValue.AddYears(1899);
                    lineItem.TimeRequiredDelivery = "";
                    lineItem.DeliveryNoteBranch = Convert.ToInt16(item.Attributes[Tags.BranchForDeliveryNote].Value);
                    lineItem.QuantityDiff = "N";
                    lineItem.DeliveredQuantity = lineItem.Quantity;
                    lineItem.ScheduledQuantity = lineItem.Quantity;
                    lineItem.ContractNo = item.Attributes[Tags.ContractNumber].Value;
                    switch (item.Attributes[Tags.Type].Value)
                    {
                        case IT.Stock:
                            lineItem.ItemType = "S";
                            break;
                        case IT.Component:
                            lineItem.ItemType = "S";
                            break;
                        default:
                            lineItem.ItemType = "N";
                            break;
                    }
                    lineItem.IsKit = Convert.ToInt16(item.Attributes[Tags.Type].Value == IT.Kit ? 1 : 0);
                    lineItem.HasString = 0;
                    lineItem.Notes = item.Attributes[Tags.ColourTrim].Value;
                    lineItem.DeliveryAddress = item.Attributes[Tags.DeliveryAddress].Value;
                    lineItem.AgreementNumber = agreementNo;
                    lineItem.ExpectedReturnDate = item.Attributes[Tags.ExpectedReturnDate].Value == "" ? DateTime.MinValue.AddYears(1899) : Convert.ToDateTime(item.Attributes[Tags.ExpectedReturnDate].Value);
                    lineItem.DeliveryArea = item.Attributes[Tags.DeliveryArea].Value;
                    lineItem.DeliveryProcess = item.Attributes[Tags.DeliveryProcess].Value;
                    lineItem.Damaged = item.Attributes[Tags.Damaged].Value;
                    lineItem.Assembly = item.Attributes[Tags.Assembly].Value;
                    lineItem.Taxrate = Convert.ToDouble(item.Attributes[Tags.TaxRate].Value);
                    lineItem.SalesBrnNo = Convert.ToString(item.Attributes[Tags.SalesBrnNo].Value) != string.Empty ? Convert.ToInt16(item.Attributes[Tags.SalesBrnNo].Value) : (Int16?)null;    //IP - 24/05/11 - CR1212 - RI = #3651
                    lineItem.Save(conn, trans);

                    //if it's a kit put it in the KitLineItem and KitCLineItem tables
                    //This is purely to satisfy OpenRoad invoice printing and can be 
                    //removed when OpenRoad goes.
                    if (item.Attributes[Tags.Type].Value == IT.Kit)
                        lineItem.SaveKitPT(conn, trans, item);

                    //Deliver the item immediately as long as it's not a kit
                    if (item.Attributes[Tags.Type].Value != IT.Kit
                            && item.Attributes[Tags.Type].Value != IT.Warranty)             // #15181 Warranty delivered in WarrantyRepository code 
                    {
                        // Heat Call 67751 - Always write a delivery record for 
                        // Paid & Taken accounts, even if the value is zero.
                        // This is to make sure stock levels are correctly 
                        // recorded in FACT.
                        bool deliver = (Math.Abs(newQty) > 0);

                        if (deliver)
                        {
                            if (Convert.ToBoolean(item.Attributes[Tags.ReplacementItem].Value) == true)
                            {
                                newValue = collection ? -newValue : replacementQuantity == 0 ? newValue : newValue / newQty * replacementQuantity; // #17290 if replacement - value of Replacement only
                                newQty = collection ? -newQty : replacementQuantity;       //#17290                          
                            }
                            else
                            {
                                if (!this.IsPaidAndTakenAccount(conn, trans, accountNumber) && item.Attributes[Tags.Type].Value != IT.Stock
                                        && item.Attributes[Tags.Type].Value != IT.KitWarranty)// (item.Attributes[Tags.Code].Value == "STAX" || item.Attributes[Tags.Code].Value == "DT")) //#18575          // #17290 if replacement only deliver replacement tax
                                {
                                    lineItem.Delivery = new DDelivery();
                                    oldValue = lineItem.Delivery.GetDeliveredValue(conn, trans, accountNumber, agreementNo,
                                                    Convert.ToInt32(item.Attributes[Tags.ItemId].Value),
                                                    Convert.ToInt16(item.Attributes[Tags.Location].Value),
                                                    item.Attributes[Tags.ContractNumber].Value, 0);

                                    newQty = collection ? -newQty : newQty;
                                    newValue = collection ? -newValue : newValue;
                                    newValue -= oldValue;
                                }
                                else
                                {
                                    newQty = collection ? -newQty : newQty;
                                    newValue = collection ? -newValue : newValue;
                                }
                            }

                            if (replacementQuantity == 0 || (!(item.Attributes[Tags.Type].Value == IT.Stock && Convert.ToBoolean(item.Attributes[Tags.ReplacementItem].Value) == false) && replacementQuantity != 0)) //#17290
                            {
                                if (!(item.Attributes[Tags.Type].Value != IT.Stock && newValue == 0))  //#18575
                                {
                                    BDelivery d = new BDelivery();
                                    d = new BDelivery();
                                    d.CashAndGoDelivery(conn, trans, accountNumber,
                                        (double)newQty, newValue,
                                        branchNo, User,
                                        item, refNo, buffNo,

                                        agreementNo, parentItemNo, parentItemId);       // RI jec 21/06/11

                                    deliveredVal += newValue;
                                    paymentVal += newValue;
                                    if (item.Attributes[Tags.Type].Value != IT.Warranty &&
                                        item.Attributes[Tags.Type].Value != IT.KitWarranty)
                                    {
                                        replacementVal += newValue;
                                    }
                                    else
                                    {
                                        /* HACK replacement value must NOT include the sales tax on the 
                                         * warranty since this has already been posted */
                                        if ((string)Country[CountryParameterNames.AgreementTaxType] == "E")
                                            replacementVal -= Convert.ToDecimal(lineItem.TaxAmount);

                                        replacementWarrantyVal += (newValue + Convert.ToDecimal(lineItem.TaxAmount));
                                    }
                                }

                            }
                        }
                    }
                    else
                    // #18488 - paymentVal (payment required) needs to include value of warranties otherwise fintrans payments are not correct when multiple payment types processed
                    {
                        paymentVal += newValue;
                    }

                    //if we have any related items, then recurse
                    foreach (XmlNode child in item)
                        if (child.NodeType == XmlNodeType.Element && child.HasChildNodes)
                        {
                            BuildPTLineItems(conn, trans, child, accountNumber, branchNo, refNo, ref deliveredVal, ref paymentVal, collection, buffNo, agreementNo, ref replacementVal, ref replacementWarrantyVal);
                        }
                }

            }
        }

        /// <summary>
        /// This takes care of automatic collection and delivery of non-stock items
        /// which may be required when an account is saved (if has already been delivered).
        /// 
        /// This routine is very similar to a routine with the same name in
        /// BDelivery. This routine is called when creating a new account or
        /// saving a revised account (SaveNewAccount, CreateRFAccount,
        /// NewCashierTotalAcct, CreateManualRFAcct). The BDelivery routine
        /// is used when making deliveries and collections. The BAccount routine
        /// will only deliver items when the delivery flag is set to 'Y'. If an
        /// item has a parent then either the parent or the child must already
        /// be delivered.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        /// <param name="lineItem"></param>
        /// <param name="accountNo"></param>
        /// <param name="branchNo"></param>
        /// <param name="refNo"></param>
        /// <param name="countryCode"></param>
        /// <param name="transValue"></param>
        /// <param name="agreementNo"></param>
        public void BuildNonStock(SqlConnection conn, SqlTransaction trans, XmlNode parent,
            XmlNode child, BItem lineItem, string accountNo,
            short branchNo, int refNo, string countryCode,
            ref decimal transValue, int agreementNo)
        {
            decimal parentQty = 0;
            decimal parentDel = 0;
            decimal newValue = 0;
            decimal oldValue = 0;
            decimal newQty = 0;
            decimal oldQty = 0;
            decimal difference = 0;
            decimal qtyToDeliver = 0;
            decimal childDelQty = 0;
            bool childDelivered = false;

            bool parentDelivered = false;
            DateTime dateTrans = DateTime.Now;

            string itemType = child.Attributes[Tags.Type].Value;

            // rdb addded code to retrieve parentItemNo, needs testing
            string parentType = "";
            string parentItemNo = string.Empty;
            int parentItemID = 0;               //IP - 17/05/11 - CR1212 - #3627
            int parentsParentID = 0;            //IP - 13/06/11 - CR1212 - for instances such as a kit component...we would need the parentid of the kit component

            if (parent != null)
            {
                parentType = parent.Attributes[Tags.Type].Value;
                parentItemNo = parent.Attributes[Tags.Code].Value;
                parentItemID = Convert.ToInt32(parent.Attributes[Tags.ItemId].Value);     //IP - 17/05/11 - CR1212 - #3627
            }



            lineItem.Delivery = new DDelivery();

            if (parent != null && parentType != IT.Kit)
            {
                if (parentType == IT.Component && parent.ParentNode.ParentNode.Attributes[Tags.ItemId] != null)     //IP - 13/06/11 - CR1212 - RI
                {
                    parentsParentID = Convert.ToInt32(parent.ParentNode.ParentNode.Attributes[Tags.ItemId].Value);
                }

                /* in general - if the parent item is fully delivered, deliver the 
                * undelivered bit of the child (which may be negative) */
                parentQty = Convert.ToDecimal(parent.Attributes[Tags.Quantity].Value);
                lineItem.Delivery.GetDeliveredQuantity(conn, trans, accountNo, agreementNo, Convert.ToInt32(parent.Attributes[Tags.ItemId].Value),          //IP - 17/05/11 - CR1212 - #3627 - changed to use ItemID
                    Convert.ToInt16(parent.Attributes[Tags.Location].Value),
                   // parent.Attributes[Tags.ContractNumber].Value, parent.Attributes[Tags.Code].Value); --IP - 14/12/09 - UAT(939) - For the parent the 'ParentItemNo' will always be ""
                   parent.Attributes[Tags.ContractNumber].Value, parentsParentID);        //IP - 17/05/11 - CR1212 - #3627 - changed to pass in 0 for parentItemID rather than "" //IP - 13/06/11 - CR1212 - RI - Pass in ParentItemID of the parent as it may be a kit which is the parent of a component
                parentDel = Convert.ToDecimal(lineItem.Delivery.DeliveredQuantity);
                parentDelivered = (parentDel == parentQty) && parentQty != 0; //IP - 14/12/09 - UAT(939) - Added check to ensure that the parentQty !=0 as was incorrectly returning as delivered for an item (not delivered) which was removed from the revise screen.
            }

            /* only ever need to deliver or collect non-stocks if
             * the Agreement.DeliveryFlag is set to 'Y' */
            if (Agreement.DeliveryFlag == "Y")
            {
                bool valueControlled = Convert.ToBoolean(child.Attributes[Tags.ValueControlled].Value);
                lineItem.Delivery.GetDeliveredQuantity(conn, trans, accountNo, agreementNo, Convert.ToInt32(child.Attributes[Tags.ItemId].Value),          //IP - 17/05/11 - CR1212 - #3627 - changed to use ItemID
                    Convert.ToInt16(child.Attributes[Tags.Location].Value),
                    child.Attributes[Tags.ContractNumber].Value, parentItemID);     //IP - 17/05/11 - CR1212 - #3627 - changed to use parentItemID rather than parentItemNo
                childDelQty = Convert.ToDecimal(lineItem.Delivery.DeliveredQuantity);

                if (parent != null && parentType != IT.Kit)
                {
                    /* check the type of an item, if it is a warranty set the delivery date to the delivery date
                     * of the parent */
                    DateTime dateDelivered = DateTime.Today;
                    if ((itemType.Equals(IT.Warranty) || itemType.Equals(IT.KitWarranty)) && parent.Attributes[Tags.DateDelivered].Value.Length > 0)
                        dateDelivered = Convert.ToDateTime(parent.Attributes[Tags.DateDelivered].Value); //CR 478 Find the parent delivery date here

                    /* is the child delivered */
                    oldQty = Convert.ToDecimal(lineItem.GetItemQuantity(conn, trans, accountNo, agreementNo,
                        Convert.ToInt16(child.Attributes[Tags.Location].Value),
                        Convert.ToInt32(child.Attributes[Tags.ItemId].Value),
                        parentItemID,                                                             //IP - 16/05/12 - #10122
                                                                                                  //Convert.ToInt32(child.Attributes[Tags.ParentItemId].Value),             // jec 30/03/12
                        child.Attributes[Tags.ContractNumber].Value, false));

                    /* if it is a new item then the oldqty = 0 and the childdelqty = 0
                     * which it is being incorrectly interpreted as delivered. Therefore
                     * must make sure that the oldQty was not 0 as well 
                    childDelivered = (childDelQty == oldQty); */
                    childDelivered = (childDelQty == oldQty) && oldQty != 0;

                    if (parentDelivered || childDelivered)	/* must add the || so that delivered children of */
                    {										/* undelivered parents can't be removed without being collected */

                        /* if valueControlled deliver the difference between the new value and 
                         * the old value (NB not delivered value) 
                         * NB we will now use the delivered value since it is more accurate */
                        if (valueControlled)
                        {
                            oldValue = lineItem.Delivery.GetDeliveredValue(conn, trans, accountNo, agreementNo,
                                Convert.ToInt32(child.Attributes[Tags.ItemId].Value),                           //IP - 17/05/11 - CR1212 - #3627 - changed to use ItemID 
                                Convert.ToInt16(child.Attributes[Tags.Location].Value),
                                child.Attributes[Tags.ContractNumber].Value, parentItemID);                     //IP - 17/05/11 - CR1212 - #3627 - changed to use parentItemID rather than parentItemNo

                            /*
                            oldValue = lineItem.GetItemValue(conn, trans, accountNo, 1,
                                Convert.ToInt16(child.Attributes[Tags.Location].Value), 
                                child.Attributes[Tags.Code].Value, 
                                child.Attributes[Tags.ContractNumber].Value,
                                false);
                            */
                            newValue = Convert.ToDecimal(child.Attributes[Tags.Value].Value);

                            //TO DO Alex thinking about 
                            difference = newValue - oldValue;

                            if (Math.Abs(difference) > (decimal).01)
                            {
                                /* always deliver the difference (even if it's negative) */
                                BDelivery d = new BDelivery(conn, trans, accountNo, 1, branchNo, User, child, refNo, Math.Round(difference, 2), 0, agreementNo, dateDelivered);
                                transValue += difference;
                                qtyToDeliver = 1;
                            }
                        }
                        else
                        {
                            /*
                            oldQty = Convert.ToDecimal(lineItem.GetItemQuantity(conn, trans, accountNo, 1, 
                                Convert.ToInt16(child.Attributes[Tags.Location].Value), 
                                child.Attributes[Tags.Code].Value,
                                child.Attributes[Tags.ContractNumber].Value));
                            */
                            oldQty = childDelQty;

                            newQty = Convert.ToDecimal(child.Attributes[Tags.Quantity].Value);
                            difference = newQty - oldQty;

                            if (Math.Abs(difference) > 0)
                            {
                                BDelivery d = new BDelivery(conn, trans,
                                    accountNo, (double)difference,
                                    branchNo, this.User, child, refNo, 0, agreementNo, dateDelivered);
                                transValue += Convert.ToDecimal(child.Attributes[Tags.UnitPrice].Value) * difference;
                                qtyToDeliver = difference;
                            }
                        }
                    }
                }
                else
                {
                    /* here we handle orphaned non-stocks (DT/SD, account level discounts) */
                    /* NB - orphaned non-stocks are NOT necessarily value controlled */
                    if (valueControlled ||
                        Convert.ToInt32(child.Attributes[Tags.ItemId].Value).In(
                            StockItemCache.Get(StockItemKeys.DT),               /* DT is not value controlled but should be treated as if it is in this case */
                            StockItemCache.Get(StockItemKeys.InsuranceChargeItem),
                            StockItemCache.Get(StockItemKeys.AdminChargeItem)
                        ))

                    {
                        oldValue = lineItem.Delivery.GetDeliveredValue(conn, trans, accountNo, agreementNo,
                            Convert.ToInt32(child.Attributes[Tags.ItemId].Value),               //IP - 17/05/11 - CR1212 - #3627 - use ItemID
                            Convert.ToInt16(child.Attributes[Tags.Location].Value),
                            child.Attributes[Tags.ContractNumber].Value, parentItemID);         //IP - 17/05/11 - CR1212 - #3627 - changed to use parentItemID rather than parentItemNo
                        /*
                        oldValue = lineItem.GetItemValue(conn, trans, accountNo, 1, 
                            Convert.ToInt16(child.Attributes[Tags.Location].Value), 
                            child.Attributes[Tags.Code].Value,
                            child.Attributes[Tags.ContractNumber].Value,
                            false);
                        */
                        newValue = Convert.ToDecimal(child.Attributes[Tags.Value].Value);
                        difference = newValue - oldValue;

                        if (Math.Abs(difference) > (decimal).01)
                        {
                            BDelivery d = new BDelivery(conn, trans, accountNo, 1, branchNo, User, child, refNo, Math.Round(difference, 2), 0, agreementNo);
                            transValue += difference;
                            qtyToDeliver = 1;
                        }
                    }
                    else
                    {
                        /*
                        oldQty = Convert.ToDecimal(lineItem.GetItemQuantity(conn, trans, accountNo, 1, 
                            Convert.ToInt16(child.Attributes[Tags.Location].Value), 
                            child.Attributes[Tags.Code].Value,
                            child.Attributes[Tags.ContractNumber].Value));
                        */
                        oldQty = childDelQty;
                        newQty = Convert.ToDecimal(child.Attributes[Tags.Quantity].Value);
                        difference = newQty - oldQty;

                        if (Math.Abs(difference) > 0)
                        {
                            BDelivery d = new BDelivery(conn, trans,
                                accountNo, (double)difference,
                                branchNo, this.User, child, refNo, 0, agreementNo);
                            transValue += Convert.ToDecimal(child.Attributes[Tags.UnitPrice].Value) * difference;
                            qtyToDeliver = difference;
                        }
                    }
                }
                if (qtyToDeliver != 0)
                {
                    // Update delqty
                    DLineItem li = new DLineItem();
                    li.UpdateDelQty(conn, trans, accountNo,
                        Convert.ToInt16(child.Attributes[Tags.Location].Value),
                        agreementNo, Convert.ToInt32(child.Attributes[Tags.ItemId].Value),  //IP - 17/05/11 - CR1212 - #3627 - use ItemID
                        child.Attributes[Tags.ContractNumber].Value,
                        (double)qtyToDeliver, parentItemID);                                //IP - 17/05/11 - CR1212 - #3627 - use parentItemID rather than parentItemNo

                }
            }
        }

        private void UpdateStatus(SqlConnection conn, SqlTransaction trans, string accountNo)
        {
            DAccount acct = new DAccount();
            acct.UpdateStatus(conn, trans, accountNo);
        }

        public void Save(SqlConnection conn, SqlTransaction trans)
        {
            DAccount acct = new DAccount();
            acct.OrigBr = this.OrigBr;
            acct.AccountNumber = this.AccountNumber;
            acct.AccountType = this.AccountType;
            acct.DateAccountOpen = this.DateAccountOpen;
            acct.CreditDays = this.CreditDays;
            acct.AgreementTotal = this.AgreementTotal;
            acct.DateLastPaid = this.DateLastPaid;
            acct.AS400Bal = this.AS400Bal;
            acct.OutstandingBalance = this.OutstandingBalance;
            acct.Arrears = this.Arrears;
            acct.HighestStatus = this.HighestStatus;
            acct.CurrentStatus = this.CurrentStatus;
            acct.HighestStatusDays = this.HighestStatusDays;
            acct.BranchNo = this.BranchNo;
            acct.PaidPcent = this.PaidPcent;
            acct.TermsType = this.TermsType;
            acct.RepossArrears = this.RepossArrears;
            acct.RepossValue = this.RepossValue;
            acct.DateIntoArrears = this.DateIntoArrears;
            acct.Country = this.CountryCode;
            acct.User = this.User;
            acct.BDWBalance = this.BDWBalance;
            acct.BDWCharges = this.BDWCharges;
            acct.HasLineItems = this.HasLineItems;
            //if ((bool)Country[CountryParameterNames.CL_Amortized])
            /// acct.IsAmortized = true;
            acct.Save(conn, trans);
        }

        /// <summary>
        /// This method will bring back all the details for an account
        /// and populate all the internal properties of this object
        /// </summary>
        /// <param name="accountNo"></param>
        /// <returns></returns>
        public bool Populate(string accountNo)
        {
            bool exists = false;
            DAccount acct = new DAccount();
            exists = acct.Populate(null, null, accountNo);
            if (exists)
            {
                this.OrigBr = acct.OrigBr;
                this.AccountNumber = acct.AccountNumber;
                this.AccountType = acct.AccountType;
                this.DateAccountOpen = acct.DateAccountOpen;
                this.CreditDays = acct.CreditDays;
                this.AgreementTotal = acct.AgreementTotal;
                this.DateLastPaid = acct.DateLastPaid;
                this.AS400Bal = acct.AS400Bal;
                this.OutstandingBalance = acct.OutstandingBalance;
                this.Arrears = acct.Arrears;
                this.HighestStatus = acct.HighestStatus;
                this.CurrentStatus = acct.CurrentStatus;
                this.HighestStatusDays = acct.HighestStatusDays;
                this.BranchNo = acct.BranchNo;
                this.PaidPcent = acct.PaidPcent;
                this.TermsType = acct.TermsType;
                this.RepossArrears = acct.RepossArrears;
                this.RepossValue = acct.RepossValue;
                this.DateIntoArrears = acct.DateIntoArrears;
                this.CountryCode = acct.Country;
                this.CustomerID = acct.CustomerID;
                this.BDWBalance = acct.BDWBalance;
                this.BDWCharges = acct.BDWCharges;
            }
            return exists;
        }

        public bool Populate(SqlConnection conn, SqlTransaction trans, string accountNo)
        {
            bool exists = false;
            DAccount acct = new DAccount();
            exists = acct.Populate(conn, trans, accountNo);
            if (exists)
            {
                this.OrigBr = acct.OrigBr;
                this.AccountNumber = acct.AccountNumber;
                this.AccountType = acct.AccountType;
                this.DateAccountOpen = acct.DateAccountOpen;
                this.CreditDays = acct.CreditDays;
                this.AgreementTotal = acct.AgreementTotal;
                this.DateLastPaid = acct.DateLastPaid;
                this.AS400Bal = acct.AS400Bal;
                this.OutstandingBalance = acct.OutstandingBalance;
                this.Arrears = acct.Arrears;
                this.HighestStatus = acct.HighestStatus;
                this.CurrentStatus = acct.CurrentStatus;
                this.HighestStatusDays = acct.HighestStatusDays;
                this.BranchNo = acct.BranchNo;
                this.PaidPcent = acct.PaidPcent;
                this.TermsType = acct.TermsType;
                this.RepossArrears = acct.RepossArrears;
                this.RepossValue = acct.RepossValue;
                this.DateIntoArrears = acct.DateIntoArrears;
                this.CountryCode = acct.Country;
                this.CustomerID = acct.CustomerID;
                this.BDWBalance = acct.BDWBalance;
                this.BDWCharges = acct.BDWCharges;

            }
            return exists;
        }

        public DataSet GetAccountForRevision(string accountNumber, int agreementNo, SqlConnection conn, SqlTransaction trans)
        {
            DataSet ds = new DataSet();
            DAccount acct = new DAccount();
            acct.GetAccountForRevision(conn, trans, accountNumber, agreementNo);  //#17290 - conn, trans

            //IP - 28/05/12 - #9877 - Warehouse & Deliveries Integration
            var booking = new LineItemBookingGetSP(conn, trans);

            DataSet lineItemBooking = booking.ExecuteDataSet(accountNumber);
            lineItemBooking.Tables[0].TableName = "LineItemBooking";

            ds.Tables.Add(acct.AccountDetails);

            if (lineItemBooking.Tables[0].Rows.Count > 0)
            {
                ds.Tables.Add(lineItemBooking.Tables[0].Copy());
            }

            return ds;
        }

        //14626
        public DataTable GetAccountForRevision(SqlConnection conn, SqlTransaction trans, string accountNumber, int agreementNo)
        {
            DataTable dt = new DataTable();
            DAccount acct = new DAccount();
            acct.GetAccountForRevision(conn, trans, accountNumber, agreementNo);

            dt = acct.AccountDetails;

            return dt;
        }

        /// <summary>
        /// Returns a dataset containing transactions of an account.
        /// </summary>
        /// <param name="customerID">customerID</param>
        /// <returns></returns>
        public DataSet GetTransactions(string accountNo)
        {
            DFinTrans transactions = new DFinTrans();
            DAccount account = new DAccount();

            DataSet dsTransaction = transactions.GetTransactions(accountNo);
            account.GetWarrantyCollectionReasonsByAccount(accountNo);
            dsTransaction.Tables.Add(account.WarrantyClaimCollectionResasons);

            return dsTransaction;
        }

        /// <summary>
        /// This method gets the customer ID which an accoutn is linked to
        /// if any
        /// </summary>
        /// <param name="accountNo"></param>
        /// <returns></returns>
        public string GetLinkedCustomerID(string accountNo, SqlConnection conn, SqlTransaction trans)
        {
            DAccount acct = new DAccount();
            acct.GetLinkedCustomerID(conn, trans, accountNo);
            return acct.CustomerID;
        }

        public string GetLinkedCustomerIDbyType(string accountNo, string relationship, SqlConnection conn, SqlTransaction trans)
        {
            DAccount acct = new DAccount();
            acct.GetLinkedCustomerIDbyType(conn, trans, accountNo, relationship);
            return acct.CustomerID;
        }

        public bool AddCustomerToAccount(SqlConnection conn, SqlTransaction trans,
            string accountNo, string customerID,
            string relationship, string accountType,
            out bool rescore, string source = "")         //#15188
        {
            rescore = false;
            bool exists = true;
            if (accountType.Length == 0)		/* not known therefore we must look it up */
            {
                exists = Populate(conn, trans, accountNo);
                accountType = AccountType;
            }

            if (exists)
            {
                //if (accountType == AT.ReadyFinance && relationship == "H")
                if ((accountType == AT.ReadyFinance || accountType == AT.StoreCard) && relationship == "H")  // #11026 - LW75408
                {
                    //check that the customer's RF credit is not blocked
                    DCustomer cust = new DCustomer();
                    cust.CustID = customerID;
                    if (cust.CreditBlocked(conn, trans))
                    {
                        throw new STLException(GetResource("M_CREDITBLOCKED"));
                    }

                    //Make sure that the customer has no RF accounts that have not yet been scored
                    if (cust.HasUnsanctionedRFAccounts(conn, trans) && source == "")                    //#15188
                    {
                        throw new STLException(GetResource("M_UNSANCTIONEDRF"));
                    }

                    // rdb CR868 6/11/07 -  if customer has previous HP accounts only
                    // it clones stages from this have added cloned out param to CloneProposal
                    bool cloned;

                    //if this customer already has an RF account with a matching proposal
                    //copy that proposal record with a new dateprop. Copy the proposal flags and referral records
                    //from the most recent proposal also. If there is a DC flag, set it's date cleared to null.
                    DProposal prop = new DProposal();
                    rescore = prop.CloneProposal(conn, trans, customerID, accountNo, out cloned);

                    //CR868
                    // rdb CR868 6/11/07 - cannot understand logic of !rescore then OpenStageForRescore
                    // will remove ! and see if an RF account opened after a HP account
                    // does not reopen stage1
                    // rescore = does customer have any expired RF accounts
                    // logic to only openstageforrescore if no expired RF accounts seems flawed

                    //if (!rescore)
                    if (cloned)
                    {
                        OpenStageForRescore(conn, trans, accountNo, customerID);
                    }
                }

                DAccount acct = new DAccount();
                acct.AddCustomerToAccount(conn, trans, accountNo, customerID, relationship, User);

                if (BDW.IsBDWSecuritised(customerID))
                {
                    // Make sure the account is marked as Securitised
                    BAccount BAcct = new BAccount();
                    BAcct.Populate(conn, trans, accountNo);
                    if (BAcct.Securitised != "Y")
                    {
                        BAcct.Securitised = "Y";
                        BAcct.Save(conn, trans);
                    }
                }
            }
            return exists;
        }

        public bool ValidateAccountNumber(string accountNo, string countryCode, string accountType)
        {
            int i = 0;
            bool status = true;
            int mod11 = 11;

            countryCode = countryCode.Trim();

            string digit = accountNo.Substring(3, 1);
            if (AT.IsCreditType(accountType) && digit != "0") //Acct Type Translation DSR 29/9/03
                status = false;

            if (accountType == AT.Cash && digit != "4")
                status = false;

            //69678 Cash accounts must end with 0
            if (accountType == AT.Cash)
            {
                digit = accountNo.Substring(11, 1);
                if (digit != "0")
                {
                    status = false;
                }
            }

            //69678 RF accounts must not end with 0
            if (accountType == AT.ReadyFinance)
            {
                digit = accountNo.Substring(11, 1);
                if (digit == "0")
                {
                    status = false;
                }
            }
            //should we include a check to make sure the account doesn't already 
            //exist? Probably
            DAccount acct = new DAccount();
            acct.Validate(null, null, accountNo);
            if (Convert.ToBoolean(acct.AccountExists))
                status = false;

            if (status)
            {
                array1 = new int[11];
                array2 = new int[12];

                for (i = 0; i < 12; i++)
                {
                    array2[i] = Convert.ToInt32(accountNo.Substring(i, 1));
                }

                if ((countryCode == "A" ||
                    countryCode == "M" ||
                    countryCode == "N" ||
                    countryCode == "P") &&
                    (accountType != AT.Cash &&
                    accountType != AT.Special))
                {					//HP weightings
                    array1[0] = 9;
                    array1[1] = 8;
                    array1[2] = 4;
                    array1[3] = 10;
                    array1[4] = 7;
                    array1[5] = 1;
                    array1[6] = 6;
                    array1[7] = 3;
                    array1[8] = 5;
                    array1[9] = 2;
                    array1[10] = 1;
                }
                else
                {					//Option weightings
                    array1[0] = 9;
                    array1[1] = 8;
                    array1[2] = 4;
                    array1[3] = 10;
                    array1[4] = 1;
                    array1[5] = 6;
                    array1[6] = 3;
                    array1[7] = 5;
                    array1[8] = 2;
                    array1[9] = 7;
                    array1[10] = 1;
                }

                //Calculate the check digit
                int modCalc = 0;
                for (i = 0; i < 11; i++)
                {
                    modCalc += array1[i] * array2[i];
                }

                status = modCalc % mod11 == 0;
            }

            return status;
        }

        public XmlNode GenerateRFAccountNo(SqlConnection conn, SqlTransaction trans, string countryCode, short branchNo, string customerID, out string accountNo)
        {
            XmlNode stampDuty = null;
            accountNo = "";

            //Get the highest type 'R' account number for this customer
            DAccount acct = new DAccount();
            accountNo = acct.GetLastRFAccount(conn, trans, customerID);

            stampDuty = this.GenerateAccountNumber(conn, trans, countryCode, branchNo, AT.ReadyFinance, false, out accountNo);

            return stampDuty;
        }

        public string CreateRFAccount(SqlConnection conn,
            SqlTransaction trans,
            string countryCode,
            short branchNo,
            string customerID,
            int user,
            bool isLoan, //CR906 rdb 06/09/07
            out bool rescore)
        {
            rescore = false;
            XmlNode stampDuty = null;
            string newAccountNo = "";
            decimal agreementTotal = 0;
            DateTime dateFirst = DateTime.MinValue.AddYears(1899);

            int agreementNo = 1;
            DataTable payMethodList = new DataTable(TN.PayMethodList);
            DataTable warrantyRenewalList = new DataTable(TN.WarrantyList);
            DataTable variableRates = new DataTable(TN.Rates);
            string referralreasons = string.Empty; //IP - 15/03/11 - #3314 - CR1245

            this.CustomerID = customerID; //IP - 09/03/11 - #3291

            stampDuty = this.GenerateRFAccountNo(conn, trans, countryCode, branchNo, customerID, out newAccountNo);
            if (newAccountNo == "000-0000-0000-0")
                newAccountNo = "";

            if (stampDuty != null)
                agreementTotal += Convert.ToDecimal(stampDuty.Attributes[Tags.Value]);

            if (newAccountNo.Length != 0)
            {
                //this.Lock(conn, trans, newAccountNo.Replace("-",""),User);

                string propResult = "";
                string bureauFailure = "";
                DateTime dateProp = DateTime.Today;
                string newBand = "";
                DCustomer cust = new DCustomer();
                cust.GetCustomerDetails(conn, trans, customerID);
                newBand = cust.scoringBand;

                int storeCardTransRefNo = 0;    //IP - 17/01/11 - Store Card

                // CR903 - add branchno to GetDefaultTermsType()
                this.SaveNewAccount(conn, trans, newAccountNo.Replace("-", ""), branchNo, AT.ReadyFinance,
                    "N", User, "", agreementTotal, 0, 0, stampDuty,
                    GetDefaultTermsType(conn, trans, branchNo, isLoan), newBand, countryCode, dateFirst, 0, 0, "", 0, 0, 
                    false, false, 0, false,
                    "", "", "", 0, null, 0, "", true, 0, 0, true, "", 0, "", branchNo, 0, payMethodList, 0, 0, warrantyRenewalList, variableRates,
                    ref propResult, ref dateProp, ref agreementNo, out bureauFailure, user, true, false, "", null, out storeCardTransRefNo, out referralreasons
                    ); //17/01/11 - Added "" for storeCardAcctNo,null for storeCardNumber and 0 for storeCardTransRefNo //IP - 15/03/11 - #3314 - CR1245 - Return referral reasons

                //this.Unlock(conn, trans, newAccountNo.Replace("-",""), User);

                //link the account to the customer
                this.AddCustomerToAccount(conn, trans, newAccountNo.Replace("-", ""), customerID, "H", AT.ReadyFinance, out rescore);
                // basing this on customer rather than previous proposal for new account...
                if ((bool)Country[CountryParameterNames.TermsTypeBandEnabled] && newBand == "")
                {
                    // Calculate the band from the current score
                    DProposal prop = new DProposal();
                    //IP - CR916 - 10/04/08 - Need to pass in the user when inserting into
                    //the ProposalAudit table.
                    prop.EmployeeNoChanged = this.User;
                    prop.AccountNo = newAccountNo.Replace("-", "");
                    prop.OverrideBand(conn, trans, "");
                }
            }

            return newAccountNo;
        }

        public string CreateStoreAccount(SqlConnection conn,
            SqlTransaction trans,
            string countryCode,
            short branchNo,
            string customerID,
            int user,
            bool isLoan, //CR906 rdb 06/09/07
            out bool rescore)
        {
            rescore = false;
            XmlNode stampDuty = null;
            string newAccountNo = "";
            decimal agreementTotal = 0;
            DateTime dateFirst = DateTime.MinValue.AddYears(1899);
            int agreementNo = 1;
            DataTable payMethodList = new DataTable(TN.PayMethodList);
            DataTable warrantyRenewalList = new DataTable(TN.WarrantyList);
            DataTable variableRates = new DataTable(TN.Rates);
            string referralReasons = string.Empty; //IP - 15/03/11 - #3314 - CR1245 

            stampDuty = this.GenerateRFAccountNo(conn, trans, countryCode, branchNo, customerID, out newAccountNo);
            if (newAccountNo == "000-0000-0000-0")
                newAccountNo = "";

            if (stampDuty != null)
                agreementTotal += Convert.ToDecimal(stampDuty.Attributes[Tags.Value]);

            if (newAccountNo.Length != 0)
            {
                //this.Lock(conn, trans, newAccountNo.Replace("-",""),User);

                string propResult = "";
                string bureauFailure = "";
                DateTime dateProp = DateTime.Today;
                string newBand = "";
                DCustomer cust = new DCustomer();
                cust.GetCustomerDetails(conn, trans, customerID);
                newBand = cust.scoringBand;

                int storeCardTransRefNo = 0;    //IP - 17/01/11 - Store Card

                // CR903 - add branchno to GetDefaultTermsType()
                this.SaveNewAccount(conn, trans, newAccountNo.Replace("-", ""), branchNo, AT.ReadyFinance,
                    "N", User, "", agreementTotal, 0, 0, stampDuty,
                    GetDefaultTermsType(conn, trans, branchNo, isLoan), newBand, countryCode, dateFirst, 0, 0, "", 0, 0, 
                    false, false, 0, false,
                    "", "", "", 0, null, 0, "", true, 0, 0, true, "", 0, "", branchNo, 0, payMethodList, 0, 0, warrantyRenewalList, variableRates,
                    ref propResult, ref dateProp, ref agreementNo, out bureauFailure, user, true, false, "", null, out storeCardTransRefNo, out referralReasons); //17/01/11 - Added "" for storeCardAcctNo,null for storeCardNumber and 0 for storeCardTransRefNo //IP - 15/03/11 - #3314 - CR1245 - Return referral reasons

                //this.Unlock(conn, trans, newAccountNo.Replace("-",""), User);

                //link the account to the customer
                this.AddCustomerToAccount(conn, trans, newAccountNo.Replace("-", ""), customerID, "H", AT.ReadyFinance, out rescore);
                // basing this on customer rather than previous proposal for new account...
                if ((bool)Country[CountryParameterNames.TermsTypeBandEnabled] && newBand == "")
                {
                    // Calculate the band from the current score
                    DProposal prop = new DProposal();
                    //IP - CR916 - 10/04/08 - Need to pass in the user when inserting into
                    //the ProposalAudit table.
                    prop.EmployeeNoChanged = this.User;
                    prop.AccountNo = newAccountNo.Replace("-", "");
                    prop.OverrideBand(conn, trans, "");
                }
            }

            return newAccountNo;
        }

        public string CreateCustomerAccount(SqlConnection conn,
            SqlTransaction trans,
            string countryCode,
            short branchNo,
            string customerID,
            string accountType,
            int user,
            bool isLoan,
            out bool rescore,
            string Source,
            string serviceRequest = null)
        {
            rescore = false;
            XmlNode stampDuty = null;
            string newAccountNo = "";
            decimal agreementTotal = 0;
            DateTime dateFirst = DateTime.MinValue.AddYears(1899);

            int agreementNo = 1;
            if (!string.IsNullOrEmpty(serviceRequest))
            {
                agreementNo = Convert.ToInt32(serviceRequest);
            }
            DataTable payMethodList = new DataTable(TN.PayMethodList);
            DataTable warrantyRenewalList = new DataTable(TN.WarrantyList);
            DataTable variableRates = new DataTable(TN.Rates);
            string referralReasons = string.Empty; //IP - 15/03/11 - #3314 - CR1245 

            stampDuty = this.GenerateAccountNumber(conn, trans, countryCode, branchNo, accountType, false, out newAccountNo);
            if (newAccountNo == "000-0000-0000-0")
                newAccountNo = "";

            if (stampDuty != null)
                agreementTotal += Convert.ToDecimal(stampDuty.Attributes[Tags.Value]);

            if (newAccountNo.Length != 0)
            {
                //this.Lock(conn, trans, newAccountNo.Replace("-",""),User);

                string propResult = "";
                string bureauFailure = "";
                DateTime dateProp = DateTime.Today;
                int storeCardTransRefNo = 0;        //IP - 17/01/11 - Store Card 
                this.Source = Source;
                // CR903 - add branchno to GetDefaultTermsType()
                this.SaveNewAccount(conn, trans, newAccountNo.Replace("-", ""), branchNo, accountType,
                    "N", User, "", agreementTotal, 0, 0, stampDuty,
                    GetDefaultTermsType(conn, trans, branchNo, isLoan), "", countryCode, dateFirst, 0, 0, "", 0, 0,
                    false, false, 0, false,
                    "", "", "", 0, null, 0, "", true, 0, 0, true, "", 0, "", branchNo, 0, payMethodList, 0, 0, warrantyRenewalList, variableRates,
                    ref propResult, ref dateProp, ref agreementNo, out bureauFailure, user, true, false, "", null, out storeCardTransRefNo, out referralReasons); //17/01/11 - Added "" for storeCardAcctNo,null for storeCardNumber and 0 for storeCardTransRefNo //IP - 15/03/11 - #3314 - CR1245 - Return referral reasons

                //link the account to the customer
                this.AddCustomerToAccount(conn, trans, newAccountNo.Replace("-", ""), customerID, "H", accountType, out rescore, Source);        //#15188
            }

            //bbhh check the storecard total amount and also available
            return newAccountNo;
        }

        //IP - 30/04/08 - UAT(362) v 5.1
        //IP - 01/05/08 - UAT(362) - Added 'ref decimal taxWarrantyOnCredit'
        private void CalculateSalesTax(string countryCode, string accountType,
            bool dutyFree, XmlNode item,
            bool includeWarranty, bool warrantiesOnCredit,
            ref decimal chargeableAdmin,
            ref decimal tax, ref decimal chargeable, ref decimal taxWarrantyOnCredit)
        {
            DStockItem stock = new DStockItem();
            decimal taxAmount = 0;

            //certain items should not be considered in this calculation
            //kits, STAX and value controlled items (including discounts)
            if (item.Attributes[Tags.Type].Value != IT.Kit &&
                Convert.ToInt32(item.Attributes[Tags.ItemId].Value).NotIn(
                    StockItemCache.Get(StockItemKeys.STAX),
                    StockItemCache.Get(StockItemKeys.DT),
                    StockItemCache.Get(StockItemKeys.InsuranceChargeItem),
                    StockItemCache.Get(StockItemKeys.AdminChargeItem)
                )
                &&
                (!warrantiesOnCredit || item.Attributes[Tags.Type].Value != IT.Warranty))
            {
                decimal taxRate = Convert.ToDecimal(item.Attributes[Tags.TaxRate].Value);
                decimal itemValue = Convert.ToDecimal(item.Attributes[Tags.UnitPrice].Value) *
                    Convert.ToDecimal(item.Attributes[Tags.Quantity].Value);
                string itemType = item.Attributes[Tags.Type].Value;

                if (item.Attributes[Tags.Type].Value != IT.Discount)
                {

                    // 5.1 uat 138 rdb I have removed this line
                    // because printing on client the tax is calculated on the total
                    // value of all items then rounded, rounding on each item
                    // then summing can leave a rounding difference
                    // the aim should be to use the same code from both places
                    //taxAmount = itemValue * (taxRate / 100);
                    //CountryRound(ref itemValue);
                    taxAmount = itemValue * (taxRate / 100);
                    //CountryRound(ref taxAmount);
                    taxAmount = Math.Round(taxAmount, 2);
                    tax += taxAmount;
                }
                else
                {
                    if (Convert.ToDecimal(item.Attributes[Tags.Quantity].Value) > 0)
                        tax += Convert.ToDecimal(item.Attributes[Tags.TaxAmount].Value);
                }

                if (item.Attributes[Tags.Code].Value.Length > 1 && item.Attributes[Tags.Code].Value.Substring(0, 2) != (string)Country[CountryParameterNames.NonInterestItem])
                {
                    chargeable += itemValue * (taxRate / 100);

                    if ((itemType != IT.Warranty &&
                        itemType != IT.KitWarranty) ||
                        includeWarranty)
                        chargeableAdmin += itemValue * (taxRate / 100);
                }
            }
            //IP - 01/05/08 - UAT(362) - Adding 'else if' to calculate tax if the 
            //warranty is on credit
            else if (item.Attributes[Tags.Type].Value == IT.Warranty && warrantiesOnCredit)
            {
                string itemType = item.Attributes[Tags.Type].Value;
                decimal warrantyOnCreditTaxAmount = 0;

                decimal taxRateWarrantyOnCredit = Convert.ToDecimal(item.Attributes[Tags.TaxRate].Value);
                decimal warrantyOnCreditValue = Convert.ToDecimal(item.Attributes[Tags.UnitPrice].Value) *
                    Convert.ToDecimal(item.Attributes[Tags.Quantity].Value);

                warrantyOnCreditTaxAmount = warrantyOnCreditValue * (taxRateWarrantyOnCredit / 100);

                warrantyOnCreditTaxAmount = Math.Round(warrantyOnCreditTaxAmount, 2);

                taxWarrantyOnCredit += warrantyOnCreditTaxAmount;

                if (item.Attributes[Tags.Code].Value.Substring(0, 2) != (string)Country[CountryParameterNames.NonInterestItem])
                {
                    chargeable += warrantyOnCreditValue * (taxRateWarrantyOnCredit / 100);

                    //dont think I need this as this block of code only deals with warranty
                    //if ((itemType != IT.Warranty &&
                    //    itemType != IT.KitWarranty) ||
                    //    includeWarranty)
                    chargeableAdmin += warrantyOnCreditValue * (taxRateWarrantyOnCredit / 100);
                }

            }
            XmlNode related = item.SelectSingleNode(Elements.RelatedItem);
            foreach (XmlNode child in related.ChildNodes)
                //IP - 01/05/08 - UAT(362) Added 'ref taxWarrantyOnCredit'
                CalculateSalesTax(countryCode, accountType, dutyFree, child, includeWarranty, warrantiesOnCredit, ref chargeableAdmin, ref tax, ref chargeable, ref taxWarrantyOnCredit);
        }

        //IP - 30/04/08 - UAT(362) - original signature
        public decimal CalculateSalesTax(string countryCode, XmlNode lineItems,
            string accountType, bool dutyFree, bool includeWarranty,
            ref decimal chargeableAdmin, ref decimal chargeable)
        {
            //IP - 01/05/08 - UAT(362) - Added 'ref taxWarrantyOnCredit'
            decimal taxWarrantyOnCredit = 0;
            return CalculateSalesTax(countryCode, lineItems, accountType, dutyFree, includeWarranty, false,
                ref chargeableAdmin, ref chargeable, ref taxWarrantyOnCredit);
        }

        //IP - 30/04/08 - UAT(362) new signature
        //IP - 01/05/08 - UAT(362) - Added 'ref taxWarrantyOnCredit'
        public decimal CalculateSalesTax(string countryCode, XmlNode lineItems,
            string accountType, bool dutyFree, bool includeWarranty, bool warrantiesOnCredit,
            ref decimal chargeableAdmin, ref decimal chargeable, ref decimal taxWarrantyOnCredit)
        {
            decimal tax = 0;

            //only add tax if it's not inclusive
            if ((string)Country[CountryParameterNames.AgreementTaxType] == "E")
            {
                foreach (XmlNode child in lineItems.ChildNodes)
                {
                    if (child.Name == Elements.Item && child.NodeType == XmlNodeType.Element)
                    {
                        CalculateSalesTax(countryCode, accountType, dutyFree, child, includeWarranty, warrantiesOnCredit, ref chargeableAdmin, ref tax, ref chargeable, ref taxWarrantyOnCredit);
                    }
                }
            }
            return tax;
        }

        public bool CancelRFAccount(SqlConnection conn, SqlTransaction trans, string accountNo, string customerID)
        {
            bool cancelled = false;
            DAccount acct = new DAccount();
            acct.User = this.User;
            cancelled = acct.CancelRFAccount(conn, trans, accountNo, customerID);
            return cancelled;
        }

        public bool CancelAccount(SqlConnection conn, SqlTransaction trans, string accountNo,
            string customerID, int branch, string code, decimal balance, string countryCode, string notes, short ContactMonths)
        {
            bool cancelled = false;
            bool otherTransactions = false;
            bool adjustmentAdded = false;
            string sundryAcctNo = "";
            string reasonCode = "CANC";
            decimal total = 0;
            decimal outstbal = 0;  // 68237 RD 27/06/06
            DateTime dateCancelled = DateTime.Now;

            DAccount acct = new DAccount(conn, trans, accountNo);
            DAgreement agree = new DAgreement(conn, trans, accountNo, 1);
            DStatus stat = new DStatus();
            DSchedule sched = new DSchedule();
            DDeliveryLoad del = new DDeliveryLoad();
            DLineItem line = new DLineItem();
            BTransaction tr = new BTransaction();
            DFinTrans finTrans = new DFinTrans();

            agree.AccountNumber = accountNo;
            agree.AgreementNumber = 1;
            agree.HoldProp = "Y";
            agree.EmployeeNumChange = this.User;
            agree.DateChange = DateTime.Now;                //IP - 03/06/10 - UAT(255) UAT5.2.1.0 Log
            if (acct.AccountType != AT.StoreCard)
                agree.AuditSource = AS.CancelAccount;

            agree.Save(conn, trans);

            if (code == (string)Country[CountryParameterNames.CancellationRejectionCode])
                this.User = 99999;

            tr.User = this.User;
            decimal warrantyAdjustment = finTrans.GetWarrantyAdjustment(conn, trans, accountNo);

            // 68237 Added to get balance from database before processing transfers
            finTrans.GetOutstBalByAcctNo(conn, trans, accountNo, out outstbal);
            if (outstbal != 0)  // Only do this if the outstanding balance is not zero
            {

                finTrans.GetByAcctNo(conn, trans, accountNo);
                foreach (DataRow row in finTrans.FinTrans.Rows)
                {
                    if ((string)row[CN.TransTypeCode] == TransType.Correction ||
                        (string)row[CN.TransTypeCode] == TransType.Refund ||
                        (string)row[CN.TransTypeCode] == TransType.Return ||
                        (string)row[CN.TransTypeCode] == TransType.Transfer ||
                        (string)row[CN.TransTypeCode] == TransType.SundryCreditTransfer)
                    {
                        otherTransactions = true;
                        total += (decimal)row[CN.TransValue];
                    }

                    if ((string)row[CN.TransTypeCode] == TransType.Payment)
                        total += (decimal)row[CN.TransValue];
                }

                total += warrantyAdjustment;
            }

            short sundryBranch = Convert.ToInt16(accountNo.Substring(0, 3));  //68282 RD 21/06/06

            if (total != 0)
            {
                sundryAcctNo = this.GetSundryCreditAccount(conn, trans, sundryBranch);   //68282 RD 21/06/06
                tr.IsCancellation = true;

                if (otherTransactions)
                {
                    tr.TransferTransaction(conn, trans, accountNo, sundryAcctNo,
                        TransType.Transfer, Math.Abs(total), (short)branch,
                        (string)Country[CountryParameterNames.CountryCode],
                        DateTime.Now, reasonCode, 0, 0, agree.AgreementNumber, null); //IP - 29/11/10 - Store Card - Added AgreementNumber //IP - 30/11/10 - Store Card - Added StoreCardNumber 
                }
                else
                {
                    decimal transValue = 0;
                    foreach (DataRow row in finTrans.FinTrans.Rows)
                    {
                        if ((string)row[CN.TransTypeCode] == TransType.Payment)
                        {
                            // The warranty adjustment needs to be added onto the
                            // payments, to ensure if a refund is given to the customer
                            // the adjustment would have been paid for.
                            transValue = (decimal)row[CN.TransValue];
                            if (warrantyAdjustment != 0 && !adjustmentAdded)
                            {
                                transValue += warrantyAdjustment;
                                adjustmentAdded = true;
                            }

                            tr.TransferTransaction(conn, trans, accountNo, sundryAcctNo,
                                TransType.SundryCreditTransfer, Math.Abs(transValue), (short)branch,
                                (string)Country[CountryParameterNames.CountryCode],
                                DateTime.Now, reasonCode, 0, 0, agree.AgreementNumber, null); //IP - 29/11/10 - Store Card - Added Agreement Number //IP - 30/11/10 - Store Card - Added StoreCardNumber 
                        }
                    }
                }

                acct.OutstandingBalance = 0;
                acct.Arrears = 0;
                acct.Save(conn, trans);
            }

            //stat.Write(conn, trans, accountNo, dateCancelled, this.User, "S");
            sched.DeleteScheduledForAccount(conn, trans);

            FactTransCancel(conn, trans, accountNo, branch, false, dateCancelled);

            line.AccountNumber = accountNo;
            //69218/29300 The datatable dtItems has already been populated in FactTransCancel(). It does not need populating again. JH 19/10/2007
            //line.GetItemsForCanxAccount(conn, trans);  // 68181 RD 22/02/06 Modified so that canx records are posted to fact
            foreach (DataRow row in dtItems.Rows)
            {
                if ((double)row[CN.DelQty] != (double)row[CN.Quantity])
                {
                    line.ItemNumber = (string)row[CN.ItemNo];
                    line.StockLocation = (short)row[CN.StockLocn];
                    line.Quantity = (double)row[CN.Quantity];
                    line.UpdateStockLevel(conn, trans);
                }

                DLineItem item = new DLineItem();
                // todo uat363 need to get the parentItemNo here 
                item.UpdateItemQuantity(conn, trans, accountNo, (int)row[CN.AgrmtNo],
                    Convert.ToInt32(row[CN.ItemId]), (short)row[CN.StockLocn], (string)row[CN.ContractNo], 0, Convert.ToInt32(row[CN.ParentItemId])); //IP - 15/01/2010 - UAT(965) - Added ParentItemNo //IP - 17/05/11 - CR1212 - #3627 - changed to use itemID and parentItemID rather than itemNo and parentItemNo

                //if (Convert.ToDouble(row[CN.Quantity]) > 0 && Convert.ToDecimal(row[CN.Price]) > 0)
                //{
                BItem bItem = new BItem();
                bItem.AccountNumber = accountNo;
                bItem.AgreementNumber = Convert.ToInt32(row[CN.AgrmtNo]);
                bItem.User = this.User;
                bItem.ItemNumber = (string)row[CN.ItemNo];
                bItem.StockLocation = Convert.ToInt16(row[CN.StockLocn]);
                bItem.ContractNo = (string)row[CN.ContractNo];
                bItem.AuditSource = AS.CancelAccount;
                bItem.ItemId = Convert.ToInt32(row[CN.ItemId]);
                bItem.UpdateLineItemAudit(conn, trans, Convert.ToDouble(row[CN.Quantity]), 0,
                                          Convert.ToDecimal(row[CN.Price]), 0,
                                          Convert.ToDouble(row[CN.TaxAmt]), 0);
                //}
            }

            acct.User = this.User;
            cancelled = acct.CancelAccount(conn, trans, accountNo, customerID, dateCancelled, code, notes);

            //824 - Void any prize vouchers issued against this account
            DCustomer cust = new DCustomer();
            cust.DeletePrizeVouchers(conn, trans, DateTime.Now, accountNo, true);

            return cancelled;
        }

        public void ConvertRFToHP(SqlConnection conn, SqlTransaction trans,
            string accountNo, string customerID, string country, DateTime dateProp)
        {
            // when converting an RF account to HP, stage 1 should remain cleared
            // and a referral flag should be created for the newly converted
            // HP account.
            DProposalFlag propFlag = new DProposalFlag();
            propFlag.OrigBr = 0;
            propFlag.CustomerID = customerID;
            propFlag.DateProp = dateProp;
            propFlag.DateCleared = DateTime.MinValue.AddYears(1899);
            propFlag.EmployeeNoFlag = this.User;
            propFlag.CheckType = "R";
            propFlag.Save(conn, trans, accountNo);

            DAccount acct = new DAccount();
            acct.ConvertRFToHP(conn, trans, accountNo, customerID, country, dateProp);
        }

        public void SaveInstantCreditFlag(SqlConnection conn,
                              SqlTransaction trans,
                              string custID,
                              string chkType, string acctno)
        {
            DHoldFlags flag = new DHoldFlags();
            flag.CustomerID = custID;
            flag.EmployeeNoFlag = this.User;
            flag.CheckType = chkType;
            flag.SaveInstantCreditFlag(conn, trans, acctno);
        }

        public DataSet GetPaymentCardDetails(string customerID, string accountNo, string branchNo)
        {
            DataSet ds = new DataSet();
            DAccount acct = new DAccount();
            ds.Tables.Add(acct.GetPaymentCardDetails(customerID, accountNo, branchNo));
            return ds;
        }

        public string GetPaidAndTakenAccount(SqlConnection conn, SqlTransaction trans, string branchNo)
        {
            DAccount acct = new DAccount();
            return acct.GetPaidAndTakenAccount(conn, trans, branchNo);
        }

        public DataSet GetDeliveryNotes(SqlConnection conn, SqlTransaction trans,
            string acctno, int user, int branch, string addr1, DateTime dateReqDel,
            string addtype, string timeReqDel, int locn, ref int buffno, out decimal amountPayable,
            out decimal charges, out bool cod)
        {
            DDelivery dDel = new DDelivery();
            DataSet ds = new DataSet();
            DAccount ac = new DAccount();
            DSchedule sched = new DSchedule();
            ac.GetDeliveryNotes(conn, trans, acctno, user, branch, addr1, dateReqDel, addtype,
                timeReqDel, locn, ref buffno);
            ds.Tables.Add(ac.DeliveryLineItems);

            /* TO DO charges have to be calculated before the schedule record is written 
             * which leaves a problem for re-prints which is yet to be resolved */
            dDel.GetCODCharges(conn, trans, acctno, buffno, 1, out amountPayable,
                out charges, out cod, dateReqDel, timeReqDel, addtype, locn);

            //Schedule a delivery for each row in the DeliveryLineItems DataTable
            foreach (DataRow r in ac.DeliveryLineItems.Rows)
            {
                //if ((double)r["delqty"] != (double)r["quantity"])
                if ((double)r["delqty"] != (double)r["quantity"] && r["DelorColl"].ToString() != "R") // if blank then not already scheduled so insert //IP - 19/02/10 - CR1072 - LW 69770 - Printing Fixes from 4.3 - Merge				
                {
                    sched.AccountNumber = acctno;
                    sched.AgreementNumber = 1;
                    sched.BuffBranchNo = branch;
                    sched.LoadNo = 0;
                    sched.BuffNo = buffno;
                    sched.ItemNumber = (string)r["itemno"];
                    sched.StockLocation = (short)r["stocklocn"];
                    sched.User = user;
                    sched.AddDelivery(conn, trans);
                }
            }

            return ds;
        }

        public DataSet GetDeliveries(SqlConnection conn, SqlTransaction trans,
            string acctno, int user, int branch, ref DateTime TimeLocked)
        {
            DataSet ds = new DataSet();
            DAccount ac = new DAccount();
            ac.GetDeliveries(conn, trans, acctno, user, branch);
            ds.Tables.Add(ac.Deliveries);
            TimeLocked = ac.DateAccountLocked;
            return ds;
        }

        public void SaveRepossArrears(SqlConnection conn, SqlTransaction trans,
            decimal arrears, decimal repossvalue, string acctno)
        {
            DAccount acct = new DAccount();
            acct.RepossArrears = arrears;
            acct.RepossValue = repossvalue;
            acct.AccountNumber = acctno;
            acct.SaveRepossArrears(conn, trans);
        }

        public int UnlockAccountsLockedAt(int user, DateTime TimeLocked)
        {
            int status = 0;
            DAccount ac = new DAccount();
            status = ac.UnlockAccountsLockedAt(user, TimeLocked);
            return status;
        }

        public DataSet GetReposessionAndRedelivery(string accountNo)
        {
            DataSet ds = new DataSet();
            DFinTrans transactions = new DFinTrans();
            transactions.GetReposessionAndRedelivery(accountNo);
            ds.Tables.Add(transactions.FinTrans);
            return ds;
        }

        public DataSet GetAccountsAllocated(int empeeNo, string branchOrAcctFilter, short acctNoSearch)
        {
            DataSet ds = new DataSet();
            DFollUpAlloc fa = new DFollUpAlloc();
            fa.GetAccountsAllocated(empeeNo, branchOrAcctFilter, acctNoSearch);
            ds.Tables.Add(fa.AllocAccounts);
            return ds;
        }

        public DataSet GetStrategyAccountsAllocated(int empeeNo, string branchOrAcctFilter, short acctNoSearch, string strategy)
        {
            DataSet ds = new DataSet();
            DFollUpAlloc fa = new DFollUpAlloc();
            fa.GetStrategyAccountsAllocated(empeeNo, branchOrAcctFilter, acctNoSearch, strategy);
            ds.Tables.Add(fa.AllocAccounts);
            return ds;
        }

        public DataSet GetWorklistAccounts(int empeeNo, string worklist, bool viewTop500) //IP - 12/11/09 UAT5.2 (882) - added control to either return top 500 or all accounts
        {
            DataSet ds = new DataSet();
            DFollUpAlloc fa = new DFollUpAlloc();
            ds = fa.GetWorklistAccounts(empeeNo, worklist, viewTop500);
            return ds;
        }

        public DataSet GetWorklistAccountsData(string acct, string storeType)
        {
            DataSet ds = new DataSet();
            DFollUpAlloc fa = new DFollUpAlloc();
            DAccount da = new DAccount();
            DCustomer dc = new DCustomer();

            //NM - 12/06/2009, Changes made to display other customer accounts in TelAction screen
            string custid = "";

            da.GetAccountDetails(null, null, acct, 1);  //IP - 11/02/11 - Sprint 5.10 - #2978 - Added null, null for conn, trans
            if (da.AccountDetails.Rows.Count > 0)
            {
                custid = da.AccountDetails.Rows[0]["Customer ID"].ToString();
            }

            ds.Tables.Add(fa.GetLineItemsForAWorklistAccount(acct));
            ds.Tables.Add(fa.GetTransactionsForAWorklistAccount(acct));
            ds.Tables.Add(fa.GetStrategiesForAWorklistAccount(acct));
            ds.Tables.Add(fa.GetWorklistsForAWorklistAccount(acct));
            ds.Tables.Add(fa.GetLettersForAWorklistAccount(acct));
            ds.Tables.Add(fa.GetSMSForAWorklistAccount(acct));
            ds.Tables.Add(fa.WorklistCustomerGetAllAccountsSP(acct));

            //-- To get other accounts ---------
            dc.CustomerSearch(custid, String.Empty, String.Empty, String.Empty, String.Empty, 100, 1, true, storeType,String.Empty);
            ds.Tables.Add(dc.CustSearch);
            return ds;
        }



        public DataSet GetFollowUpHistory(string accountNo)
        {
            DataSet ds = new DataSet();
            DFollUpAlloc fa = new DFollUpAlloc();
            fa.GetFollowUpHistory(accountNo);
            ds.Tables.Add(fa.AllocHistory);
            return ds;
        }

        public DataSet GetBailActions(string accountNo)
        {
            DataSet ds = new DataSet();
            DBailAction ba = new DBailAction();
            ba.GetBailActions(accountNo);
            ds.Tables.Add(ba.BailActions);
            return ds;
        }

        public DataSet GetSPAHistory(string accountNo)
        {
            DataSet ds = new DataSet();
            DSpa spa = new DSpa();
            spa.GetSpaHistory(accountNo);
            ds.Tables.Add(spa.SpaHistory);
            return ds;
        }

        public void Deallocate(SqlConnection conn, SqlTransaction trans,
            string acctNo)
        {
            DFollUpAlloc Fu = new DFollUpAlloc();
            Fu.User = this.User;
            Fu.DeAllocate(conn, trans, acctNo);
        }

        //IP - 08/10/09 - UAT(909)
        //public bool SaveAllocation(SqlConnection conn, SqlTransaction trans,
        //    string acctNo, int empeeNo, bool checkMaxAccounts)
        public void SaveAllocation(SqlConnection conn, SqlTransaction trans,
           string acctNo, int empeeNo)
        {
            DFollUpAlloc Fu = new DFollUpAlloc();
            Fu.User = this.User;
            //return Fu.Save(conn, trans, acctNo, empeeNo, checkMaxAccounts);
            Fu.Save(conn, trans, acctNo, empeeNo);
        }

        public void SaveBailActions(SqlConnection conn, SqlTransaction trans,
            string acctNo, int empeeNo, string code, string notes, DateTime dateDue,
            double actionValue, DateTime spaDateExpiry, string spaReasonCode, double spaInstal, DateTime reminderDateTime, bool cancelOutstandingReminders, string callingSource)
        {
            DateTime dateTrans = DateTime.Now;

            DBailAction ba = new DBailAction();
            ba.AccountNo = acctNo;
            ba.EmployeeNo = empeeNo;
            ba.Code = code;
            ba.Notes = notes;

            ba.DateAdded = dateTrans;
            ba.DateDue = dateDue;
            ba.ActionValue = actionValue;
            ba.AddedBy = this.User;
            ba.SpaDateExpiry = spaDateExpiry;
            ba.SpaReasonCode = spaReasonCode;
            ba.SpaInstal = spaInstal;
            ba.CallingSource = callingSource;
            ba.CancelOutstandingReminders = cancelOutstandingReminders;

            if (code == "REM" || code == "PREM")
                ba.ReminderDateTime = reminderDateTime;

            Int64 cardNumber = 0;
            if (code == "STL") // storecard lost or stolen
            { // extract card number from notes passed in( a bit lazy)
                cardNumber = Convert.ToInt64(notes.Substring(0, 16));
                ba.AllocNo = Convert.ToInt32(notes.Substring(12, 4));
            }
            else
                ba.AllocNo = 0;

            ba.Save(conn, trans);

            // Save the EXTRA COMMISSION
            if (code == "COM" && Math.Abs(actionValue) >= 0.01)
            {
                DBailiffCommn bcm = new DBailiffCommn();
                bcm.AllocatedCourtsPerson = empeeNo;
                bcm.AccountNo = acctNo;
                bcm.Status = "H";	        // Hold commission payment
                bcm.DateTrans = dateTrans;	// Must match BailAction datetime
                bcm.TransRefNo = 0;
                bcm.ChequeColln = "N";
                bcm.TransValue = Convert.ToDecimal(actionValue);
                bcm.Save(conn, trans);
            }

            if (code == "SPA")
            {
                DAccount acct = new DAccount(conn, trans, acctNo);
                if (acct.CurrentStatus != "1" && acct.CurrentStatus != "2")
                {
                    acct.CurrentStatus = "2";
                    acct.User = this.User;
                    acct.Save(conn, trans);
                }
            }

            if (code == "STL") // storecard lost or stolen
            {
                Blue.Cosacs.StorecardMarkLost Lost = new Blue.Cosacs.StorecardMarkLost();
                Lost.Execute(cardNumber, dateDue);

            }

        }

        public DataSet GetFollupAllocForActionSheet(string acctNo, int empeeNo)
        {
            DFollUpAlloc collections = new DFollUpAlloc();
            return collections.GetFollupAllocForActionSheet(acctNo, empeeNo);
        }

        public DataSet SetAllocDate(SqlConnection conn, SqlTransaction trans, int empeeNo)
        {
            int count = 0;
            DFollUpAlloc fu = new DFollUpAlloc();
            fu.getNewAllocations(empeeNo);
            DataTable dt = fu.AllocAccounts;

            DBailAction ba = new DBailAction();

            foreach (DataRow row in dt.Rows)
            {
                ba.AccountNo = (string)row["AcctNo"];
                ba.EmployeeNo = (int)row["EmpeeNo"];
                ba.Code = "ALL";
                ba.DateAdded = DateTime.Now;
                ba.Save(conn, trans);
                count++;
            }

            fu.UpdateDateAlloc(conn, trans, empeeNo);

            DataSet ds = new DataSet();
            ds.Tables.Add(fu.AllocAccounts);

            return ds;
        }

        /// <summary>
        /// AccountApplicationStatus
        /// </summary>
        /// <param name="acctno">string</param>
        /// <returns>DataSet</returns>
        /// 
        public DataSet AccountApplicationStatus(string acctno)
        {
            DataSet ds = new DataSet();
            DAccount da = new DAccount();
            ds = da.ApplicationStatus(acctno);
            return ds;
        }

        /// <summary>
        /// GetLetterByAcctNo
        /// </summary>
        /// <param name="acctno">string</param>
        /// <returns>DataSet</returns>
        /// 
        public DataSet GetLetterByAcctNo(string acctno)
        {
            DataSet ds = new DataSet();
            DAccount da = new DAccount();
            string serverDB = (string)Country[CountryParameterNames.TallymanServerDB];
            short linkToTallyman = Convert.ToInt16(Country[CountryParameterNames.LinkToTallyman]);
            ds = da.GetLetterByAcctNo(acctno, serverDB, linkToTallyman);
            return ds;
        }

        /// <summary>
        /// LettersAndStatusesByAcctNo
        /// </summary>
        /// <param name="acctno">string</param>
        /// <returns>DataSet</returns>
        /// 
        public DataSet LettersAndStatusesByAcctNo(string acctno)
        {
            DataSet ds = new DataSet();
            DLetter da = new DLetter();
            ds = da.LettersAndStatusesByAcctNo(acctno);
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
        public DataSet DeliveryNotesReprintLoad(string acctno, int stockLocn, int bufffnofrom, int bufffnoto)
        {
            DataSet ds = new DataSet();
            DAccount da = new DAccount();
            ds = da.DeliveryNotesReprintLoad(acctno, stockLocn, bufffnofrom, bufffnoto);
            return ds;
        }

        public DataTable GetReprintDetails(SqlConnection conn, SqlTransaction trans, string acctno, int stockLocn,
             //int buffbranchno, int buffno, DateTime dateReqDel, string addtype, string timeReqDel,   //CR1072 Malaysia merge -LW71408
             int buffno, DateTime dateReqDel, string addtype, string timeReqDel,   //CR1072 Malaysia merge -LW71408 --IP - 22/02/10 - Undone 71408 - reinstate later
            out decimal amountPayable, out decimal charges, out bool cod)
        {
            DDelivery del = new DDelivery();
            DAccount da = new DAccount();
            //da.GetReprintDetails(conn, trans, acctno, stockLocn, buffbranchno, buffno);     //CR1072 Malaysia merge -LW71408
            da.GetReprintDetails(conn, trans, acctno, stockLocn, buffno);     //CR1072 Malaysia merge -LW71408 - --IP - 22/02/10 - Undone 71408 - reinstate later


            del.GetCODChargesForReprint(conn, trans, acctno, buffno, 1, out amountPayable,
                out charges, out cod, dateReqDel, timeReqDel, addtype);

            return da.DeliveryLineItems;
        }

        public decimal GetChargeableCashPrice(SqlConnection conn, SqlTransaction trans,
            string accountNo, ref decimal chargeableAdminPrice)
        {
            DAccount acct = new DAccount();
            return acct.GetChargeableCashPrice(conn, trans, accountNo, ref chargeableAdminPrice);
        }

        /// <summary>
        /// To check the deposit amount is the min percentage of the
        /// cash price. This cannot be calculated until the add-to
        /// function has calculated the new cash price.
        /// </summary>
        /// <param name="curDeposit"></param>
        /// <param name="subTotal"></param>
        private decimal SetDeposit(string termsType, bool depositChecked,
            decimal curDeposit, decimal subTotal)
        {
            DTermsType tt = new DTermsType();
            DataSet termsTypeSet = tt.LoadTermsTypeDetails(termsType);

            decimal defaultDeposit = Convert.ToDecimal(termsTypeSet.Tables[TN.TermsType].Rows[0][CN.DefaultDeposit]);
            bool depositIsPercentage = (bool)(termsTypeSet.Tables[TN.TermsType].Rows[0][CN.DepositIsPercentage]);
            decimal minDeposit = 0;

            if (depositChecked && defaultDeposit > 0)
            {
                // The terms type has a default deposit
                if (depositIsPercentage)
                {
                    // The default deposit is a percentage of the subtotal
                    minDeposit = subTotal * (defaultDeposit / 100);
                }
                else
                {
                    // The default deposit is a fixed amount
                    minDeposit = defaultDeposit;
                }
            }

            // Make sure the deposit is not less then the min
            curDeposit = (curDeposit < minDeposit) ? minDeposit : curDeposit;
            // Make sure the deposit is not more than the cash price
            // (Note: cannot use agr total here because it varies with a different deposit)
            curDeposit = (curDeposit > subTotal) ? subTotal : curDeposit;

            CountryRound(ref curDeposit);
            return curDeposit;
        }

        /// <summary>
        /// This method will calculate the implications of an add to but will 
        /// Not actually perform the Add to. This will be done when the add to 
        /// is accepted by the user.
        /// </summary>
        /// <param name="addToAccount"></param>
        /// <param name="accounts"></param>
        /// <param name="newCashPrice"></param>
        /// <param name="newAgreementTotal"></param>
        /// <param name="newMonthlyInstal"></param>
        /// <param name="newFinalInstal"></param>
        /// <param name="newNoInstalments"></param>
        public void CalculateAddTo(string addToAccount,
            StringCollection accounts,
            string countryCode,
            short branchNo,
            string termsType,
            string scoringBand,
            ref decimal deposit,
            short months,
            short paymentHolidays,
            bool depositChecked,
            out decimal sumBalances,
            out decimal newCashPrice,
            out decimal newAgreementTotal,
            out decimal newMonthlyInstal,
            out decimal newFinalInstal,
            out int newNoInstalments)
        {
            decimal deferredTerms = 0;
            decimal monthly = 0;
            decimal final = 0;
            decimal insuranceCharge = 0;
            decimal adminCharge = 0;
            decimal dtTax = 0;
            decimal insTax = 0;
            decimal adminTax = 0;

            /* call Account.Populate on each account */
            DAccount[] accts = new DAccount[accounts.Count];
            DAccount addTo = new DAccount(addToAccount);
            BAgreement addToAgreement = new BAgreement();
            DInstalPlan addToIP = new DInstalPlan();
            addToIP.Populate(null, null, addToAccount, 1);
            DAgreement agree = new DAgreement(null, null, addToAccount, 1);

            // CR440 Terms passed from Add-To screen
            addTo.TermsType = termsType;
            addToIP.NumberOfInstalments = months;

            sumBalances = 0;
            for (int i = 0; i < accounts.Count; i++)
            {
                accts[i] = new DAccount(accounts[i]);

                /* calculate the rebate on each account */
                /* subtract the rebate from the outstanding balance */
                decimal rebate = accts[i].CalculateRebate(null, null, accounts[i]);
                CountryRound(ref rebate);
                accts[i].OutstandingBalance -= rebate;
                sumBalances += accts[i].OutstandingBalance;
            }

            /* sum the new outstanding balances and add this to the 
             * cash price of the addToAccount */
            agree.CashPrice += sumBalances;

            /* we need to obtain the cash price excluding non interest items to base the DT on */
            decimal chargeableAdminPrice = 0;
            decimal chargeablePrice = GetChargeableCashPrice(null, null, addToAccount, ref chargeableAdminPrice);
            chargeablePrice += sumBalances;
            chargeableAdminPrice += sumBalances;

            // Check the deposit is in the valid range
            deposit = this.SetDeposit(termsType, depositChecked, deposit, chargeablePrice);
            agree.Deposit = deposit;
            agree.PaymentHolidays = paymentHolidays;
            addToAgreement.Deposit = agree.Deposit;
            addToAgreement.PaymentHolidays = agree.PaymentHolidays;

            DataSet variableRatesSet = new DataSet();
            /* calculate the service charge of the addToAccount given the
             * new agerement cash price */
            deferredTerms = addToAgreement.CalculateServiceCharge(null, null,
                countryCode,
                addTo.TermsType,
                addTo.AccountNumber,
                scoringBand,
                agree.Deposit,
                addToIP.NumberOfInstalments,
                chargeablePrice,
                addTo.DateAccountOpen,
                addTo.AccountType,
                chargeableAdminPrice,
                ref insuranceCharge,
                ref adminCharge,
                ref variableRatesSet);

            /* need to add tax onto deferred terms and admin charge and insurance charge
             * and then pass the total of all of that into CreateInstalPlan */

            BItem item = new BItem();
            XmlNode itemNode = item.GetItemDetails(StockItemCache.Get(StockItemKeys.DT), branchNo, AT.HP, countryCode, false, false);     // CR1212 jec need to supply itemID not zero //IP/NM - 18/05/11 -CR1212 - #3627 - Use cached item 
            itemNode.Attributes[Tags.Quantity].Value = "1";
            itemNode.Attributes[Tags.UnitPrice].Value = StripCurrency(deferredTerms.ToString(DecimalPlaces));
            itemNode.Attributes[Tags.TaxAmount].Value = "0";
            dtTax = item.CalculateTaxAmount(itemNode, IsTaxExempt(null, null, addToAccount, DBNull.Value.ToString()));

            if (insuranceCharge > 0)
            {
                itemNode = item.GetItemDetails(StockItemCache.Get(StockItemKeys.InsuranceChargeItem), branchNo, AT.HP, countryCode, false, false);   // CR1212 jec need to supply itemID not zero  //IP/NM - 18/05/11 -CR1212 - #3627 - Use cached item 
                itemNode.Attributes[Tags.Quantity].Value = "1";
                itemNode.Attributes[Tags.UnitPrice].Value = StripCurrency(insuranceCharge.ToString(DecimalPlaces));
                itemNode.Attributes[Tags.TaxAmount].Value = "0";
                insTax = item.CalculateTaxAmount(itemNode, IsTaxExempt(null, null, addToAccount, DBNull.Value.ToString()));
            }

            if (adminCharge > 0)
            {
                itemNode = item.GetItemDetails(StockItemCache.Get(StockItemKeys.AdminChargeItem), branchNo, AT.HP, countryCode, false, false);       // CR1212 jec need to supply itemID not zero //IP/NM - 18/05/11 -CR1212 - #3627 - Use cached item 
                itemNode.Attributes[Tags.Quantity].Value = "1";
                itemNode.Attributes[Tags.UnitPrice].Value = StripCurrency(adminCharge.ToString(DecimalPlaces));
                itemNode.Attributes[Tags.TaxAmount].Value = "0";
                adminTax = item.CalculateTaxAmount(itemNode, IsTaxExempt(null, null, addToAccount, DBNull.Value.ToString()));
            }

            if ((string)Country[CountryParameterNames.AgreementTaxType] == "I")
                dtTax = adminTax = insTax = 0;

            addToAgreement.CalculateInstalPlan(agree.CashPrice,
                agree.Deposit,
                deferredTerms + dtTax + insuranceCharge + insTax + adminCharge + adminTax,
                addToIP.NumberOfInstalments,
                out monthly,
                out final);

            newCashPrice = agree.CashPrice;
            newAgreementTotal = deferredTerms + dtTax + insuranceCharge + insTax + adminCharge + adminTax + agree.CashPrice;
            newNoInstalments = addToIP.NumberOfInstalments;
            newMonthlyInstal = monthly;
            newFinalInstal = final;

            CountryRound(ref newCashPrice);
            CountryRound(ref newAgreementTotal);
        }

        /// <summary>
        /// This method will perform an add to by carrying out the 
        /// following steps:
        /// 1)	Calculate the rebate for each account to be added to
        /// 2)	Post the rebate to the fintrans table if required
        /// 3)	Subtract the rebate from the outstanding balance and
        ///		add a non-stock lineitem (ADDCR) to the account with
        ///		the value of the new outstanding balance.
        ///	4)	Deliver this ADDCR item and place the added to account
        ///		number in the chequeno field on the fintrans table
        ///		so we can reverse the add to later. This delivery 
        ///		should cause the account to be settled.
        ///	5)	Reduce the agreement total by the value of the ADDCR item
        ///	6)	A non-stock item (ADDDR) will be added to the addto account 
        ///		with the combined value of the outstanding balances 
        ///		of the added to accounts. This item will be delivered.
        ///	7)	The balance of the addto account must be increased 
        ///		and the service charge recalculated.
        /// </summary>
        /// <param name="addToAccount"></param>
        /// <param name="accounts"></param>
        /// <param name="countryCode"></param>
        /// <param name="branchNo"></param>
        public void ProcessAddTo(SqlConnection conn, SqlTransaction trans,
            string addToAccountNo,
            StringCollection accounts,
            string countryCode,
            string termsType,
            string scoringBand,
            ref decimal deposit,
            short months,
            short paymentHolidays,
            bool depositChecked)
        {
            decimal sumBalances = 0;
            decimal rebate = 0;
            DateTime dateTrans = DateTime.Now;
            decimal deferredTerms = 0;
            decimal monthly = 0;
            decimal final = 0;
            decimal insuranceCharge = 0;
            decimal adminCharge = 0;
            decimal dtTax = 0;
            decimal insTax = 0;
            decimal adminTax = 0;
            short branchNo = 0;

            //
            // Added FROM Account(s)
            //

            /* call Account.Populate on each account */
            DAccount[] accts = new DAccount[accounts.Count];
            BAgreement[] agreements = new BAgreement[accounts.Count];
            DAccount addToAcct = new DAccount(conn, trans, addToAccountNo);
            DInstalPlan dueDay = new DInstalPlan();     //jec 26/01/07  get dueday
            BAgreement addToAgreement = new BAgreement();
            DInstalPlan addToIP = new DInstalPlan();
            addToIP.User = User;
            BItem item = new BItem();

            addToIP.Populate(conn, trans, addToAccountNo, 1);
            addToAgreement.Populate(conn, trans, addToAccountNo, 1);

            // CR440 Terms passed from Add-To screen
            addToAcct.TermsType = termsType;
            addToIP.NumberOfInstalments = months;
            dueDay.GetDueDay(this.GetLinkedCustomerID(addToAccountNo, conn, trans));     //jec 26/01/07 get dueday
            this.DueDay = dueDay.DueDay;                                    //jec 26/01/07 get dueday

            // 5.1 uat153 rdb 07/04/08 set datenextdue to datenextdue of the account we are adding to
            DateTime dueNextDate = DateTime.MinValue;

            for (int i = 0; i < accounts.Count; i++)
            {
                /* calculate the rebate on each account */
                accts[i] = new DAccount(conn, trans, accounts[i]);
                agreements[i] = new BAgreement();

                rebate = 0;
                rebate = accts[i].CalculateRebate(conn, trans, accounts[i]);
                CountryRound(ref rebate);

                if (rebate > 0)
                {
                    branchNo = Convert.ToInt16(accts[i].AccountNumber.Substring(0, 3));
                    /* post the rebate to the fintrans table */
                    BTransaction transaction = new BTransaction(conn, trans, accounts[i],
                        branchNo, 0 /*refno*/,
                        -rebate, User, TransType.Rebate,
                        "" /*bankCode*/, "" /*bankAcctNo*/,
                        addToAccountNo /*chequeNo*/, 0 /*paymethod*/,       //CR1072 - LW71187
                        countryCode, dateTrans, "" /*footnote*/, 0);

                    /* subtract the rebate from the outstanding balance */
                    accts[i].OutstandingBalance -= rebate;
                }
                sumBalances += accts[i].OutstandingBalance;

                /* update the agreement total by value of ADDCR 
                 * what about the instal plan? */
                accts[i].AgreementTotal -= accts[i].OutstandingBalance;
                if (accts[i].AgreementTotal < 0) accts[i].AgreementTotal = 0;
                agreements[i].Populate(conn, trans, accounts[i], 1);
                agreements[i].AgreementTotal = accts[i].AgreementTotal;
                agreements[i].EmployeeNumChange = this.User;          //IP - UAT(251) UAT5.2.1.0 Log
                agreements[i].DateChange = DateTime.Now;              //IP - UAT(251) UAT5.2.1.0 Log           
                agreements[i].Save(conn, trans);
                accts[i].Save(conn, trans);

                // 5.1 uat153 rdb 07/04/08 set datenextdue to datenextdue of the account we are adding to
                dueNextDate = agreements[i].DateNextDue;

                /* add the ADDCR non-stock item to the account with the value of
                 * the Outstanding balance */
                InsertAddToItem(conn, trans, addToAccountNo, accounts[i],
                    -accts[i].OutstandingBalance, dateTrans, countryCode);

            }

            //
            // Added TO Account
            //
            branchNo = Convert.ToInt16(addToAccountNo.Substring(0, 3));

            /* sum the new outstanding balances and add this to the 
             * cash price of the addToAccount */
            addToAgreement.CashPrice += sumBalances;

            /* we need to obtain the cash price excluding non interest items to base the DT on */
            decimal chargeableAdminPrice = 0;
            decimal chargeablePrice = GetChargeableCashPrice(conn, trans, addToAccountNo, ref chargeableAdminPrice);
            chargeablePrice += sumBalances;
            chargeableAdminPrice += sumBalances;

            // Check the deposit is in the valid range
            deposit = this.SetDeposit(termsType, depositChecked, deposit, chargeablePrice);
            addToAgreement.Deposit = deposit;
            addToAgreement.PaymentHolidays = paymentHolidays;

            DataSet variableRatesSet = new DataSet();
            /* calculate the service charge of the addToAccount given the
             * new agreement cash price */
            deferredTerms = addToAgreement.CalculateServiceCharge(conn, trans,
                countryCode,
                addToAcct.TermsType,
                addToAcct.AccountNumber,
                scoringBand,
                addToAgreement.Deposit,
                addToIP.NumberOfInstalments,
                /*addToAgreement.CashPrice,  exclude non-interest items */
                chargeablePrice,
                addToAcct.DateAccountOpen,
                addToAcct.AccountType,
                chargeableAdminPrice,
                ref insuranceCharge,
                ref adminCharge,
                ref variableRatesSet);

            // If we are adding to a new RF account we must insert the non-stock lines
            this.InsertAddToNonStock(conn, trans, addToAccountNo, branchNo, deferredTerms, insuranceCharge, adminCharge);

            /* need to add tax onto deferred terms and admin charge and insurance charge
             * and then pass the total of all of that into CreateInstalPlan */

            XmlNode itemNode = item.GetItemDetails(StockItemCache.Get(StockItemKeys.DT), branchNo, AT.HP, countryCode, false, false);     // CR1212 jec need to supply itemID not zero  //IP/NM - 18/05/11 -CR1212 - #3627 - Use cached item 
            itemNode.Attributes[Tags.Quantity].Value = "1";
            itemNode.Attributes[Tags.UnitPrice].Value = StripCurrency(deferredTerms.ToString(DecimalPlaces));
            itemNode.Attributes[Tags.TaxAmount].Value = "0";
            dtTax = item.CalculateTaxAmount(itemNode, IsTaxExempt(conn, trans, addToAccountNo, ""));

            if (insuranceCharge > 0)
            {
                itemNode = item.GetItemDetails(StockItemCache.Get(StockItemKeys.InsuranceChargeItem), branchNo, AT.HP, countryCode, false, false);   // CR1212 jec need to supply itemID not zero  //IP/NM - 18/05/11 -CR1212 - #3627 - Use cached item 
                itemNode.Attributes[Tags.Quantity].Value = "1";
                itemNode.Attributes[Tags.UnitPrice].Value = StripCurrency(insuranceCharge.ToString(DecimalPlaces));
                itemNode.Attributes[Tags.TaxAmount].Value = "0";
                insTax = item.CalculateTaxAmount(itemNode, IsTaxExempt(conn, trans, addToAccountNo, ""));
            }

            if (adminCharge > 0)
            {
                itemNode = item.GetItemDetails(StockItemCache.Get(StockItemKeys.AdminChargeItem), branchNo, AT.HP, countryCode, false, false);       // CR1212 jec need to supply itemID not zero  //IP/NM - 18/05/11 -CR1212 - #3627 - Use cached item 
                itemNode.Attributes[Tags.Quantity].Value = "1";
                itemNode.Attributes[Tags.UnitPrice].Value = StripCurrency(adminCharge.ToString(DecimalPlaces));
                itemNode.Attributes[Tags.TaxAmount].Value = "0";
                adminTax = item.CalculateTaxAmount(itemNode, IsTaxExempt(conn, trans, addToAccountNo, DBNull.Value.ToString()));
            }

            if ((string)Country[CountryParameterNames.AgreementTaxType] == "I")
                dtTax = adminTax = insTax = 0;

            addToAgreement.CalculateInstalPlan(addToAgreement.CashPrice,
                addToAgreement.Deposit,
                deferredTerms + dtTax + insuranceCharge + insTax + adminCharge + adminTax,
                addToIP.NumberOfInstalments,
                out monthly,
                out final);

            /* update the relevant tables with the new agreement information */
            addToAgreement.AgreementTotal = deferredTerms + dtTax + insuranceCharge + insTax + adminCharge + adminTax + addToAgreement.CashPrice;
            addToAcct.AgreementTotal = addToAgreement.AgreementTotal;
            addToIP.InstalmentAmount = monthly;
            addToIP.FinalInstalment = final;
            addToAgreement.ServiceCharge = deferredTerms;
            // calculate the balance as the add-to cash price - if the account delivers all non stocks then it will recalculate the balance automatically 
            addToAcct.OutstandingBalance += sumBalances;

            /* Insert the add to item to the addToAccount */
            InsertAddToItem(conn, trans, accounts[0],       //CR1072 LW71187
                addToAccountNo, sumBalances, dateTrans, countryCode);

            // 5.1 uat153 rdb 07/04/08 set datenextdue to datenextdue of the account we are adding to
            addToAgreement.DateNextDue = dueNextDate;
            addToAgreement.DateChange = DateTime.Now;
            addToAgreement.EmployeeNumChange = this.User;
            addToAgreement.Save(conn, trans);

            addToAcct.Save(conn, trans);
            addToIP.DueDay = this.DueDay;           //jec 26/01/07 set dueday
            addToIP.Save(conn, trans);

            /*need to update the value STAX items too if dtTax || insTax || adminTax > 0 */
            item.UpdateItemValue(conn, trans, addToAccountNo, 1, StockItemCache.Get(StockItemKeys.DT), branchNo, deferredTerms); //IP/NM - 18/05/11 -CR1212 - #3627 - Use cached item
            item.UpdateItemValue(conn, trans, addToAccountNo, 1, StockItemCache.Get(StockItemKeys.InsuranceChargeItem), branchNo, insuranceCharge); //IP/NM - 18/05/11 -CR1212 - #3627 - Use cached item
            item.UpdateItemValue(conn, trans, addToAccountNo, 1, StockItemCache.Get(StockItemKeys.AdminChargeItem), branchNo, adminCharge); //IP/NM - 18/05/11 -CR1212 - #3627 - Use cached item

            if (dtTax > 0)
                item.UpdateTaxAmount(conn, trans, addToAccountNo, StockItemCache.Get(StockItemKeys.DT), branchNo, dtTax); //IP/NM - 18/05/11 -CR1212 - #3627 - Use cached item
            if (insTax > 0)
                item.UpdateTaxAmount(conn, trans, addToAccountNo, StockItemCache.Get(StockItemKeys.InsuranceChargeItem), branchNo, insTax); //IP/NM - 18/05/11 -CR1212 - #3627 - Use cached item
            if (adminTax > 0)
                item.UpdateTaxAmount(conn, trans, addToAccountNo, StockItemCache.Get(StockItemKeys.AdminChargeItem), branchNo, adminTax); //IP/NM - 18/05/11 -CR1212 - #3627 - Use cached item

            // Deliver non-stocks
            DTermsType tt = new DTermsType();
            tt.GetTermsTypeDetail(conn, trans, countryCode, addToAcct.TermsType, addToAcct.AccountNumber, "", addToAcct.DateAccountOpen);
            if ((short)(tt.TermsTypeDetails.Rows[0][CN.DeliverNonStocks]) == 1)
            {
                // Perform the delivery of non-stock items on this account
                DBranch branch = new DBranch();
                int transRefNo = branch.GetTransRefNo(conn, trans, branchNo);
                decimal transValue = 0;
                BDelivery del = new BDelivery();
                del.User = this.User;
                del.DeliverNonStocks(conn, trans, addToAcct.AccountNumber, addToAcct.AccountType, countryCode, branchNo, transRefNo, ref transValue, addToAgreement.AgreementNumber);
                addToAgreement.DeliveryFlag = "Y";
                addToAgreement.Save(conn, trans);

                if (Math.Abs(transValue) >= 0.01M)
                {
                    BTransaction t = new BTransaction(conn, trans, addToAcct.AccountNumber, branchNo,
                        transRefNo, transValue, User,
                        TransType.Delivery, "", "", "", 0, countryCode,
                        dateTrans, FootNote.AddToDelivery, addToAgreement.AgreementNumber);
                }
            }
        }

        public void ReverseAddTo(SqlConnection conn, SqlTransaction trans,
            string accountNo,
            string accountType,
            string countryCode)
        {
            decimal rebate = 0;
            decimal addcrValue = 0;
            decimal adddrValue = 0;
            DateTime dateTrans = DateTime.Now;
            string addedTo = "";
            decimal deferredTerms = 0;
            decimal monthly = 0;
            decimal final = 0;
            decimal insuranceCharge = 0;
            decimal adminCharge = 0;
            decimal dtTax = 0;
            decimal insTax = 0;
            decimal adminTax = 0;
            short branchNo = 0;
            BTransaction transaction = new BTransaction();

            /* find out the original added to account no from the fintrans table */
            addedTo = transaction.GetAddedToAccount(conn, trans, accountNo, addcrValue);
            if (addedTo.Length == 0)
            {
                /* cannot continue if we can't find the added to account */
                throw new STLException(GetResource("M_CANTFINDADDEDTO"));
                //throw new Exception("Unable to perform reversal. Cannot determine account added to.");
            }

            //
            // Added FROM Account
            //

            branchNo = Convert.ToInt16(accountNo.Substring(0, 3));

            /* find out if a rebate was posted - if so reverse it */
            rebate = transaction.GetRebate(conn, trans, accountNo);

            if (Math.Abs(rebate) > 0)
            {
                /* post a reversal of the rebate to the fintrans table */
                BTransaction reverse = new BTransaction(conn, trans, accountNo,
                    branchNo, 0 /*refno*/,
                    -rebate, User, TransType.Rebate,
                    "" /*bankCode*/, "" /*bankAcctNo*/,
                    "" /*chequeNo*/, 0 /*paymethod*/,
                    countryCode, dateTrans, "" /*footnote*/, 0);
            }

            /* remove the ADDCR line item from the account */
            BItem item = new BItem();
            addcrValue = item.GetItemValue(conn, trans, accountNo, 1, branchNo, StockItemCache.Get(StockItemKeys.ADDCR), "", true);     //IP/NM - 18/05/11 -CR1212 - #3627
            item.DeleteLineItem(conn, trans, accountNo, 1, StockItemCache.Get(StockItemKeys.ADDCR), branchNo);                   //IP/NM - 18/05/11 -CR1212 - #3627

            addcrValue = Math.Round(addcrValue, 2);

            /* Collect the ADDCR item
			 * Include the footnote so we can distinguish an
			 * ADDCR Reversal from an ADDDR.
			 */
            DBranch branch = new DBranch();
            int refNo = branch.GetTransRefNo(conn, trans, branchNo);
            BTransaction collect = new BTransaction(conn, trans, accountNo, branchNo, refNo,
                -addcrValue, User, TransType.AddTo,
                "", "", addedTo, 0, countryCode,
                dateTrans, FootNote.AddtoCreditReversal, 1);     //UAT172 related jec 08/10/10 hard code agreementnumber   

            DDelivery del = new DDelivery();
            del.OrigBr = 0;
            del.AccountNumber = accountNo;
            del.AgreementNumber = 1;
            del.DateDelivered = DateTime.Today;
            del.DeliveryOrCollection = DelType.Collection;
            del.ItemNumber = "ADDCR";
            del.ItemId = StockItemCache.Get(StockItemKeys.ADDCR);
            del.StockLocation = branchNo;
            del.Quantity = 1;
            del.BuffNo = branch.GetBuffNo(conn, trans, branchNo);
            del.BuffBranchNumber = branchNo;
            del.DateTrans = dateTrans;
            del.BranchNumber = branchNo;
            del.TransRefNo = refNo;
            del.TransValue = -addcrValue;
            del.RunNumber = 0;
            del.ftNotes = "DNRA";
            del.NotifiedBy = User;
            del.ParentItemNo = ""; //IP - 03/08/09 - UAT(657) - Need to set the parentitemno
            del.Write(conn, trans);

            /* TO DO - this is inadequate, the agreement total is not simply 
             * a function of the addcr value being removed. The service charge
             * must be recalculated to work out the new agreement total */

            /* Update the agreement total */
            BAgreement agreement = new BAgreement();
            agreement.Populate(conn, trans, accountNo, 1);
            agreement.AgreementTotal -= addcrValue;
            agreement.EmployeeNumChange = this.User;
            agreement.DateChange = DateTime.Now;
            agreement.Save(conn, trans);

            BAccount acct = new BAccount();
            acct.Populate(conn, trans, accountNo);
            acct.AgreementTotal = agreement.AgreementTotal;
            acct.Save(conn, trans);

            /* recalculate service charge if HP? No don't bother (says Alex) */
            /* Re-open stage 1 if HP ? No don't bother (says Alex) */

            //
            // Added TO Account
            //

            branchNo = Convert.ToInt16(addedTo.Substring(0, 3));

            /* For added to account */
            /* Edit the agreement total */

            /* TO DO - safer to recalculate cash price using the routine */
            BAgreement addToAgree = new BAgreement();
            addToAgree.Populate(conn, trans, addedTo, 1);
            addToAgree.CashPrice += addcrValue;

            BAccount addToAcct = new BAccount();
            addToAcct.Populate(conn, trans, addedTo);

            DInstalPlan addToIP = new DInstalPlan();
            addToIP.Populate(conn, trans, addedTo, 1);
            addToIP.User = User;

            /* remove or edit the ADDDR lineitem depending on its value */
            adddrValue = item.GetItemValue(conn, trans, addedTo, 1, branchNo, StockItemCache.Get(StockItemKeys.ADDDR), "", true);
            if (Math.Abs(adddrValue) == Math.Abs(addcrValue))
                item.DeleteLineItem(conn, trans, addedTo, 1, StockItemCache.Get(StockItemKeys.ADDDR), branchNo);    //IP/NM - 18/05/11 -CR1212 - #3627 
            else
                item.UpdateItemValue(conn, trans, addedTo, 1, StockItemCache.Get(StockItemKeys.ADDDR), branchNo, adddrValue + addcrValue); //IP/NM - 18/05/11 -CR1212 - #3627 

            decimal chargeableAdminPrice = 0;
            decimal chargeablePrice = GetChargeableCashPrice(conn, trans, addedTo, ref chargeableAdminPrice);
            /* the diff in addcr value will already be reflected because it's been
             * removed from the lineitem table 
            chargeablePrice += addcrValue;  */

            // Make sure the deposit is not too big for this cash price
            // otherwise the instalplan save will error saying the instalplan
            // does not match the agreement total.
            if (addToAgree.Deposit > chargeablePrice) addToAgree.Deposit = chargeablePrice;

            /* Recalculate the service charge */
            DataSet variableRatesSet = new DataSet();
            deferredTerms = addToAgree.CalculateServiceCharge(conn, trans, countryCode,
                addToAcct.TermsType,
                addToAcct.AccountNumber,
                "",
                addToAgree.Deposit,
                addToIP.NumberOfInstalments,
                chargeablePrice,
                addToAcct.DateAccountOpen,
                addToAcct.AccountType,
                chargeableAdminPrice,
                ref insuranceCharge,
                ref adminCharge,
                ref variableRatesSet);

            /* need to add tax onto deferred terms and admin charge and insurance charge
             * and then pass the total of all of that into CreateInstalPlan */
            XmlNode itemNode = item.GetItemDetails(StockItemCache.Get(StockItemKeys.DT), branchNo, AT.HP, countryCode, false, false);     // CR1212 jec need to supply itemID not zero  //IP/NM - 18/05/11 -CR1212 - #3627 - Use cached item
            itemNode.Attributes[Tags.Quantity].Value = "1";
            itemNode.Attributes[Tags.UnitPrice].Value = StripCurrency(deferredTerms.ToString(DecimalPlaces));
            itemNode.Attributes[Tags.TaxAmount].Value = "0";
            dtTax = item.CalculateTaxAmount(itemNode, IsTaxExempt(conn, trans, addedTo, ""));

            if (insuranceCharge > 0)
            {
                itemNode = item.GetItemDetails(StockItemCache.Get(StockItemKeys.InsuranceChargeItem), branchNo, AT.HP, countryCode, false, false);   // CR1212 jec need to supply itemID not zero  //IP/NM - 18/05/11 -CR1212 - #3627 - Use cached item 
                itemNode.Attributes[Tags.Quantity].Value = "1";
                itemNode.Attributes[Tags.UnitPrice].Value = StripCurrency(insuranceCharge.ToString(DecimalPlaces));
                itemNode.Attributes[Tags.TaxAmount].Value = "0";
                insTax = item.CalculateTaxAmount(itemNode, IsTaxExempt(conn, trans, addedTo, ""));
            }

            if (adminCharge > 0)
            {
                itemNode = item.GetItemDetails(StockItemCache.Get(StockItemKeys.AdminChargeItem), branchNo, AT.HP, countryCode, false, false);   // CR1212 jec need to supply itemID not zero  //IP/NM - 18/05/11 -CR1212 - #3627 - Use cached item 
                itemNode.Attributes[Tags.Quantity].Value = "1";
                itemNode.Attributes[Tags.UnitPrice].Value = StripCurrency(adminCharge.ToString(DecimalPlaces));
                itemNode.Attributes[Tags.TaxAmount].Value = "0";
                adminTax = item.CalculateTaxAmount(itemNode, IsTaxExempt(conn, trans, addedTo, ""));
            }

            if ((string)Country[CountryParameterNames.AgreementTaxType] == "I")
                dtTax = adminTax = insTax = 0;

            addToAgree.CalculateInstalPlan(addToAgree.CashPrice, addToAgree.Deposit,
                deferredTerms + dtTax + insuranceCharge + insTax + adminCharge + adminTax,
                addToIP.NumberOfInstalments,
                out monthly, out final);

            /* update the relevant tables with the new agreement information */
            addToAgree.AgreementTotal = deferredTerms + dtTax + insuranceCharge + insTax + adminCharge + adminTax + addToAgree.CashPrice;
            addToAcct.AgreementTotal = addToAgree.AgreementTotal;
            addToIP.InstalmentAmount = monthly;
            addToIP.FinalInstalment = final;
            addToAgree.ServiceCharge = deferredTerms;

            addToAgree.Save(conn, trans);
            addToAcct.Save(conn, trans);
            addToIP.Save(conn, trans);

            /* Collect the value of the ADDCR item which may or may not 
			 * be all of the value of the ADDDR depending on whether 
			 * multiple accounts were origanlly added to.
			 * Include the footnote so we can distinguish an
			 * ADDDR Reversal from an ADDCR.
			*/
            decimal toCollect = addcrValue;
            if (toCollect != 0)
            {
                refNo = branch.GetTransRefNo(conn, trans, branchNo);
                BTransaction collDiff = new BTransaction(conn, trans, addedTo, branchNo, refNo,
                    toCollect, User, TransType.AddTo,
                    "", "", accountNo, 0, countryCode,
                    dateTrans, FootNote.AddtoDebitReversal, agreement.AgreementNumber);

                DDelivery diff = new DDelivery();
                diff.OrigBr = 0;
                diff.AccountNumber = addedTo;
                diff.AgreementNumber = 1;
                diff.DateDelivered = DateTime.Today;
                diff.DeliveryOrCollection = DelType.Collection;
                diff.ItemNumber = "ADDDR";
                diff.ItemId = StockItemCache.Get(StockItemKeys.ADDDR);
                diff.StockLocation = branchNo;
                diff.Quantity = -1;
                diff.BuffNo = branch.GetBuffNo(conn, trans, branchNo);
                diff.BuffBranchNumber = branchNo;
                diff.DateTrans = dateTrans;
                diff.BranchNumber = branchNo;
                diff.TransRefNo = refNo;
                diff.TransValue = toCollect;
                diff.RunNumber = 0;
                diff.ParentItemNo = ""; //IP - 03/08/09 - UAT(657) - Need to set the parentitemno
                diff.Write(conn, trans);
            }

            /* TO DO need to update the value STAX items too if dtTax || insTax || adminTax > 0 */
            item.UpdateItemValue(conn, trans, addedTo, 1, StockItemCache.Get(StockItemKeys.DT), branchNo, deferredTerms);  //IP/NM - 18/05/11 -CR1212 - #3627 - Use cached item
            item.UpdateItemValue(conn, trans, addedTo, 1, StockItemCache.Get(StockItemKeys.InsuranceChargeItem), branchNo, insuranceCharge);  //IP/NM - 18/05/11 -CR1212 - #3627 - Use cached item
            item.UpdateItemValue(conn, trans, addedTo, 1, StockItemCache.Get(StockItemKeys.AdminChargeItem), branchNo, adminCharge);  //IP/NM - 18/05/11 -CR1212 - #3627 - Use cached item

            if (dtTax > 0)
                item.UpdateTaxAmount(conn, trans, addedTo, StockItemCache.Get(StockItemKeys.DT), branchNo, dtTax); //IP/NM - 18/05/11 -CR1212 - #3627 - Use cached item
            if (insTax > 0)
                item.UpdateTaxAmount(conn, trans, addedTo, StockItemCache.Get(StockItemKeys.InsuranceChargeItem), branchNo, insTax); //IP/NM - 18/05/11 -CR1212 - #3627 - Use cached item
            if (adminTax > 0)
                item.UpdateTaxAmount(conn, trans, addedTo, StockItemCache.Get(StockItemKeys.AdminChargeItem), branchNo, adminTax); //IP/NM - 18/05/11 -CR1212 - #3627 - Use cached item

            // If DT has been delivered then we need to collect it (and admin and ins)
            DDelivery deliveredDT = new DDelivery();
            deliveredDT.GetDeliveredQuantity(conn, trans, addedTo, 1, StockItemCache.Get(StockItemKeys.DT), branchNo, "", 0);  //IP/NM - 18/05/11 -CR1212 - #3627 - Use cached item and pass in 0 for ParentItemID
            if (deliveredDT.DeliveredQuantity > 0)
            {
                // Collect non-stock items
                BDelivery delivery = new BDelivery();
                refNo = branch.GetTransRefNo(conn, trans, branchNo);
                decimal transValue = 0;
                delivery.User = this.User;
                delivery.DeliverNonStocks(conn, trans, addedTo, addToAcct.AccountType,
                    countryCode, branchNo, refNo, ref transValue, 1);

                // Fintrans collection record for all items
                BTransaction t = null;
                if (Math.Abs(transValue) > 0)
                    t = new BTransaction(conn, trans, addedTo, branchNo,
                        refNo, transValue, User,
                        TransType.GoodsReturn, "", "", accountNo, 0, countryCode,
                        dateTrans, FootNote.AddtoDebitReversal, addToAgree.AgreementNumber);
            }
        }

        private void InsertAddToItem(SqlConnection conn, SqlTransaction trans,
            string addToAccountNo, // for fintrans.chequeno
            string accountNo,
            decimal orderValue,
            DateTime dateTrans,
            string countryCode)
        {
            /* TO DO : if this is an ADDDR then me may have already added to
             * this account. This means that there may already be a ADDDR line
             * item. In which case we need to update the ordervalue and deliver 
             * the difference. */

            short acctBranchNo = Convert.ToInt16(accountNo.Substring(0, 3));

            int itemId = StockItemCache.Get(orderValue < 0 ? StockItemKeys.ADDCR : StockItemKeys.ADDDR);
            string itemNo = orderValue < 0 ? "ADDCR" : "ADDDR";
            double quantity = orderValue < 0 ? -1 : 1;
            DBranch branch = new DBranch();
            int refNo = branch.GetTransRefNo(conn, trans, acctBranchNo);

            DDelivery del = new DDelivery();
            del.OrigBr = 0;
            del.AccountNumber = accountNo;
            del.AgreementNumber = 1;
            del.DateDelivered = DateTime.Today;
            del.DeliveryOrCollection = DelType.Normal;
            del.ItemId = itemId;
            del.StockLocation = acctBranchNo;
            del.Quantity = quantity;
            del.BuffNo = branch.GetBuffNo(conn, trans, acctBranchNo);
            del.BuffBranchNumber = acctBranchNo;
            del.DateTrans = dateTrans;
            del.BranchNumber = acctBranchNo;
            del.TransRefNo = refNo;
            del.TransValue = Math.Round(orderValue, 2);
            del.RunNumber = 0;
            del.ftNotes = "DNAD";
            del.NotifiedBy = User;
            del.ParentItemNo = ""; //IP & JC -26/01/09 - Need to set this
            del.Write(conn, trans);

            DFACTTrans fact = new DFACTTrans();
            DLineItem lineItem = new DLineItem();
            lineItem.AccountNumber = accountNo;
            lineItem.AgreementNumber = 1;
            lineItem.ItemNumber = itemNo;
            lineItem.ItemID = itemId;
            lineItem.Quantity = 1;
            lineItem.StockLocation = acctBranchNo;
            lineItem.Price = Math.Round(orderValue, 2);
            lineItem.OrderValue = Math.Round(orderValue, 2);
            lineItem.DeliveryNoteBranch = acctBranchNo;
            lineItem.QuantityDiff = "N";
            lineItem.ItemType = "N";
            lineItem.BuffNo = fact.GetOrderNo(conn, trans, accountNo, 1);
            if (lineItem.BuffNo == 0)
            {
                branch = new DBranch();
                lineItem.BuffNo = branch.GetBuffNo(conn, trans, acctBranchNo);
            }
            lineItem.User = User;       //UAT171 jec 07/10/10
            lineItem.Save(conn, trans);

            BTransaction transaction = new BTransaction(conn, trans, accountNo,
                acctBranchNo, refNo, orderValue,
                User, TransType.AddTo,
                "", "", addToAccountNo, 0,
                countryCode, dateTrans, "", 1);
        }

        private void InsertAddToNonStock(SqlConnection conn, SqlTransaction trans,
            string addToAccountNo, short branchNo,
            decimal deferredTerms, decimal insuranceCharge, decimal adminCharge)
        {
            if (Math.Abs(Math.Round(deferredTerms, 2)) > 0.01M)
            {
                DLineItem lineItem = new DLineItem();
                lineItem.AccountNumber = addToAccountNo;
                lineItem.AgreementNumber = 1;
                lineItem.ItemNumber = "DT";
                lineItem.ItemID = StockItemCache.Get(StockItemKeys.DT);
                lineItem.StockLocation = branchNo;
                lineItem.Price = Math.Round(deferredTerms, 2);
                lineItem.OrderValue = Math.Round(deferredTerms, 2);
                lineItem.DeliveryNoteBranch = branchNo;
                lineItem.QuantityDiff = "N";
                lineItem.ItemType = "N";
                lineItem.Quantity = 0;
                // If the item already exists then do not save it again
                lineItem.GetItemQuantity(conn, trans, true);
                lineItem.User = User;       //UAT171 jec 07/10/10
                if (lineItem.Quantity == 0)
                {
                    lineItem.Quantity = 1;
                    lineItem.Save(conn, trans);
                }
            }

            if (Math.Abs(Math.Round(insuranceCharge, 2)) > 0.01M)
            {
                DLineItem lineItem = new DLineItem();
                lineItem.AccountNumber = addToAccountNo;
                lineItem.AgreementNumber = 1;
                lineItem.ItemID = StockItemCache.Get(StockItemKeys.InsuranceChargeItem);
                lineItem.StockLocation = branchNo;
                lineItem.Price = Math.Round(insuranceCharge, 2);
                lineItem.OrderValue = Math.Round(insuranceCharge, 2);
                lineItem.DeliveryNoteBranch = branchNo;
                lineItem.QuantityDiff = "N";
                lineItem.ItemType = "N";
                lineItem.Quantity = 0;
                // If the item already exists then do not save it again
                lineItem.GetItemQuantity(conn, trans, true);
                lineItem.User = User;       //UAT171 jec 07/10/10
                if (lineItem.Quantity == 0)
                {
                    lineItem.Quantity = 1;
                    lineItem.Save(conn, trans);
                }
            }

            if (Math.Abs(Math.Round(adminCharge, 2)) > 0.01M)
            {
                DLineItem lineItem = new DLineItem();
                lineItem.AccountNumber = addToAccountNo;
                lineItem.AgreementNumber = 1;
                lineItem.ItemID = StockItemCache.Get(StockItemKeys.AdminChargeItem);
                lineItem.StockLocation = branchNo;
                lineItem.Price = Math.Round(adminCharge, 2);
                lineItem.OrderValue = Math.Round(adminCharge, 2);
                lineItem.DeliveryNoteBranch = branchNo;
                lineItem.QuantityDiff = "N";
                lineItem.ItemType = "N";
                lineItem.Quantity = 0;
                // If the item already exists then do not save it again
                lineItem.GetItemQuantity(conn, trans, true);
                lineItem.User = User;       //UAT171 jec 07/10/10
                if (lineItem.Quantity == 0)
                {
                    lineItem.Quantity = 1;
                    lineItem.Save(conn, trans);
                }
            }
        }

        public short HasAddToOrDelivery(SqlConnection conn, SqlTransaction trans, string accountNo)
        {
            DLineItem li = new DLineItem();
            return li.HasAddToOrDelivery(conn, trans, accountNo);
        }

        public short SettledByAddTo(SqlConnection conn, SqlTransaction trans, string accountNo)
        {
            DLineItem li = new DLineItem();
            return li.SettledByAddTo(conn, trans, accountNo);
        }

        public bool IsTaxExempt(SqlConnection conn, SqlTransaction trans, string accountNo, string reference)
        {
            DAccount acct = new DAccount();
            return acct.IsTaxExempt(conn, trans, accountNo, reference);
        }

        public void FactTransCancel(SqlConnection conn, SqlTransaction trans, string acctNo,
            int branch, bool reOpen, DateTime dateCancelled)
        {
            DLineItem item = new DLineItem();
            DFACTTrans fact = new DFACTTrans();
            DBranch bc = new DBranch();
            DDelivery del = new DDelivery();

            double quantity = 0;
            double price = 0;
            double taxamt = 0;
            double value = 0;
            string tranType = "";
            string tcCode = "";
            bool status = true;
            int buffNo = 0;

            buffNo = bc.GetBuffNo(conn, trans, (short)branch);

            del.AccountNumber = acctNo;
            del.GetDeliveries(conn, trans);
            fact.TranDate = dateCancelled;
            fact.BuffNo = buffNo;

            foreach (DataRow row in del.Deliveries.Rows)
            {
                if (Convert.ToDecimal(row[CN.Value]) == 0)
                {
                    fact.ItemNumber = (string)row[CN.ItemNo];
                    fact.StockLocation = (short)row[CN.StockLocn];
                    //if ((decimal)row[CN.OrdVal] > 0) // IP - 22/02/10 - CR1072 - LW 70059 - Sanction Fixes from 4.3 - Merge
                    if (Convert.ToDecimal(row[CN.Value]) > 0)
                    {
                        fact.Quantity = -1;
                        fact.TranType = "13";
                        fact.TCCode = "11";
                    }
                    else
                    {
                        fact.Quantity = 1;
                        fact.TranType = "03";
                        fact.TCCode = "32";
                    }

                    //69474 no account number is being passed to the facttrans table
                    fact.AccountNumber = acctNo;
                    fact.Price = 0;
                    //decimal total = 0 - (decimal)row[CN.OrdVal]; // IP - 22/02/10 - CR1072 - LW 70059 - Sanction Fixes from 4.3 - Merge
                    decimal total = 0 - Convert.ToDecimal(row[CN.Value]);
                    fact.Value = (double)total;
                    fact.Save(conn, trans);
                }
            }

            tranType = "04";
            tcCode = "58";

            buffNo = bc.GetBuffNo(conn, trans, (short)branch);

            item.AccountNumber = acctNo;

            dtItems = item.GetItemsForCanxAccount(conn, trans);    // 68181 RD 22/02/06 Modified so that canx records are posted to fact

            foreach (DataRow row in dtItems.Rows)
            {
                if ((string)row["ItemType"] == "S")
                {
                    fact.AccountNumber = acctNo;
                    fact.ItemNumber = (string)row[CN.ItemNo];
                    fact.StockLocation = (short)Convert.ToInt32(row[CN.StockLocn]);
                    fact.TranType = tranType;
                    fact.TCCode = tcCode;


                    if (reOpen)
                    {
                        int rowCount = fact.DeleteCancellation(conn, trans);

                        if (rowCount == 0 && (Convert.ToDouble(row[CN.Quantity]) - Convert.ToDouble(row[CN.DelQty])) > 0)
                        {
                            quantity = Convert.ToDouble(row[CN.Quantity]) - Convert.ToDouble(row[CN.DelQty]);
                            price = Convert.ToDouble(row[CN.Price]);
                            taxamt = Convert.ToDouble(row[CN.TaxAmt]);
                            value = quantity * price;
                            tranType = "01";
                            tcCode = "61";
                            status = true;
                        }
                        else
                            status = false;
                    }
                    else
                    {
                        quantity = -1 * (Convert.ToDouble(row[CN.Quantity]) - Convert.ToDouble(row[CN.DelQty]));
                        price = Convert.ToDouble(row[CN.Price]);
                        taxamt = Convert.ToDouble(row[CN.TaxAmt]);
                        value = quantity * price;
                    }

                    if (status)
                    {
                        fact.AgreementNumber = 1;
                        fact.BuffNo = buffNo;
                        fact.TranType = tranType;
                        fact.TCCode = tcCode;
                        fact.Quantity = quantity;
                        fact.Price = price;
                        fact.TaxAmt = taxamt;
                        fact.Value = value;
                        fact.Save(conn, trans);
                    }
                }
            }
        }

        public void GetAllocatedCourtsPerson(string accountNo, ref int empNo, ref string empType, ref string empName)
        {
            DFollUpAlloc follup = new DFollUpAlloc();
            follup.GetAllocatedCourtsPerson(accountNo, ref empNo, ref empType, ref empName);
        }

        public string GetSundryCreditAccount(SqlConnection conn, SqlTransaction trans, short branchNo)
        {
            DAccount acct = new DAccount();
            string acctNo = acct.GetSundryCreditAccount(conn, trans, branchNo);
            if (acctNo.Length != 12) throw new STLException(GetResource("M_NOSUNDRYACCOUNTBRANCH", new object[] { branchNo }));
            return acctNo;
        }

        public string NewCashierTotalAcct(SqlConnection conn, SqlTransaction trans, string countryCode, short branchno, int empeeno, string custId)
        {
            // For shortage accounts that are per employee, so employee branch number is used
            DEmployee employee = new DEmployee();
            employee.GetEmployeeDetails(conn, trans, empeeno);
            return NewCashierTotalAcct(conn, trans, countryCode, employee.BranchNumber, branchno, custId);
        }

        public string NewCashierTotalAcct(SqlConnection conn, SqlTransaction trans, string countryCode, short branchno, short promoBranchNo, string custId)
        {
            // For shortage and overage accounts.
            // Shortage accounts are per employee, so the employee branch number should be passed in.
            // Overage accounts are per branch, so the current branch number should be passed in.
            string acctNo = "";
            this.GenerateAccountNumber(conn, trans, countryCode,
                branchno,
                AT.Special, false, out acctNo);
            this.AccountNumber = acctNo.Replace("-", "");

            string propResult = "";
            DateTime dateProp = DateTime.Today;
            int agreementNo = 1;
            this.User = User;
            string bureauFailure = "";
            DataTable payMethodList = new DataTable(TN.PayMethodList);
            DataTable warrantyRenewalList = new DataTable(TN.WarrantyList);
            DataTable variableRates = new DataTable(TN.Rates);

            int storeCardTransRefNo = 0; //IP - 17/01/11 - Store Card
            string referralReasons = string.Empty; //IP - 15/03/11 - #3314 - CR1245

            // CR903 - add branchno to GetDefaultTermsType() 
            // CR906 rdb 06/09/07 added isLoan
            // CR906 IsLoan parameter is not required here as the account being set up will not be a loan account JH 24/09/2007
            // Instead pass a value of false
            this.SaveNewAccount(conn, trans, this.AccountNumber, branchno,
                AT.Special, "N", User, "", 0, 0, 0, null,
                this.GetDefaultTermsType(conn, trans, branchno, false), "", countryCode, DateTime.MinValue, 0,
                0, "", 0, 0, 
                false, false, 0, false,
                "", "", "", 0, null, 0, "", true, 0, 0, true,
                "", 0, "", promoBranchNo, 0, payMethodList, 0, 0, warrantyRenewalList, variableRates,
                ref propResult, ref dateProp, ref agreementNo, out bureauFailure, 0, true, false, "", null, out storeCardTransRefNo, out referralReasons); //17/01/11 - Added "" for storeCardAcctNo,null for storeCardNumber and 0 for storeCardTransRefNo //IP - 15/03/11 - #3314 - CR1245 - Return referral reasons

            bool rescore;
            this.AddCustomerToAccount(conn, trans, this.AccountNumber, custId,
                "H", AT.Special, out rescore);

            return this.AccountNumber;
        }

        public bool CanReverseOverage(SqlConnection conn, SqlTransaction trans, int empeeno, decimal overage)
        {
            DAccount acct = new DAccount();
            decimal lastDiff = acct.GetCashierLastDifference(conn, trans, empeeno);
            return (overage == -lastDiff);
        }

        public string GetOveragesAccount(SqlConnection conn, SqlTransaction trans, short branchNo)
        {
            DAccount acct = new DAccount();
            this.AccountNumber = acct.GetOveragesAccount(conn, trans, branchNo);
            return this.AccountNumber;
        }

        public string GetReceivableAccount(SqlConnection conn, SqlTransaction trans, int empeeno)
        {
            DAccount acct = new DAccount();
            this.AccountNumber = acct.GetReceivableAccount(conn, trans, empeeno);
            return this.AccountNumber;
        }
        public void CheckAccountType(string acctno, out bool result)
        {
            result = false;
            DAccount payment = null;
            payment = new DAccount();
            payment.CheckAccountType(acctno, out result);


        }
        public void CheckAccountToCancel(SqlConnection conn, SqlTransaction trans,
            string accountNo, string countryCode, int user, ref decimal outstBalance,
            ref bool outstPayments)
        {
            DAccount acct = new DAccount();
            DFinTrans finTrans = new DFinTrans();
            DLineItem line = new DLineItem();
            DCustomer cust = new DCustomer();
            DAccountNumber acctNo = new DAccountNumber();
            BTransaction tr = new BTransaction();

            DataSet ds = new DataSet();
            //SqlConnection conn = null;
            //SqlTransaction trans = null;

            this.User = user;

            bool status = true;
            bool exists = false;
            decimal balance = 0;
            decimal as400balance = 0;

            //acct.CheckAccountToCancel(accountNo);
            //ds.Tables.Add(acct.AccountDetails); 

            exists = acct.Populate(conn, trans, accountNo);

            // Check to see if account exists or has been settled.
            if (!exists)
            {
                status = false;
                throw new STLException(GetResource("M_NOACCOUNTDATA"));
            }
            else
            {
                balance = acct.OutstandingBalance;
                as400balance = acct.AS400Bal;
                outstBalance = balance;

                if (acct.CurrentStatus == "S")
                {
                    status = false;
                    throw new STLException(GetResource("M_ACCOUNTSETTLED"));
                }

                //IP - 13/04/12 - #9920 - UAT150
                if (balance > 0 && acct.AccountType == AT.StoreCard)
                {
                    status = false;
                    throw new STLException(GetResource("M_CANNOTCANCELSTORECARD", new object[] { balance }));
                }
            }

            RemoveStampDuty(conn, trans, accountNo, countryCode);

            if (new WarehouseRepository().HasOutstanding(accountNo))
            {
                throw new STLException("This account has outstanding items in warehouse. Please cancel shipments from warehouse before cancelling.");      //#15493 
            }

            if (new WarehouseRepository().HasOutstandingCollectionsToBeActioned(accountNo))
            {
                throw new STLException("There are outstanding shipments that need to be actioned in the Failed Deliveries & Collection Screen. Please action before cancelling.");  //#15493 
            }

            // Check to see if there are any items that need to be collected
            // before account can be cancelled.
            if (status)
                ProcessDeliveries(accountNo, balance, ref status, conn, trans);

            // Check to see if there are any non stock deliveries for account.
            // Non Stock deliveries should not exist for account.
            if (status)
            {
                exists = line.NonStockCheck(conn, trans, accountNo);

                if (exists)
                {
                    status = false;
                    throw new STLException(GetResource("M_NONSTOCKSEXISTS"));
                }
            }

            // Check pre existing accounts to make sure:
            //		no line items exists
            //		no delivery items exist
            //		delivery total = 0
            if (status)
            {
                cust.GetBasicCustomerDetails(conn, trans, "", accountNo, "H");

                if (accountNo.Substring(0, 11) == cust.CustID && balance < (decimal)0.01)
                {
                    exists = line.CheckPreExistingDels(conn, trans, accountNo);
                    if (!exists && as400balance > (decimal)0.01)
                    {
                        status = false;
                        throw new STLException(GetResource("M_PREEXISTING"));
                    }
                }
            }

            decimal total = 0;
            decimal paymentTotal = 0;

            finTrans.GetByAcctNo(conn, trans, accountNo);
            foreach (DataRow row in finTrans.FinTrans.Rows)
            {
                total += (decimal)row[CN.TransValue];

                if ((string)row[CN.TransTypeCode] == TransType.Correction ||
                    (string)row[CN.TransTypeCode] == TransType.GiroExtra ||
                    (string)row[CN.TransTypeCode] == TransType.GiroNormal ||
                    (string)row[CN.TransTypeCode] == TransType.GiroRepresent ||
                    (string)row[CN.TransTypeCode] == TransType.TakeonTransfer ||
                    (string)row[CN.TransTypeCode] == TransType.Payment ||
                    (string)row[CN.TransTypeCode] == TransType.Refund ||
                    (string)row[CN.TransTypeCode] == TransType.Return ||
                    (string)row[CN.TransTypeCode] == TransType.SundryCreditTransfer ||
                    (string)row[CN.TransTypeCode] == TransType.Transfer)
                {
                    paymentTotal += (decimal)row[CN.TransValue];
                }
            }

            decimal warrantyAdjustment = finTrans.GetWarrantyAdjustment(conn, trans, accountNo);
            if (warrantyAdjustment > 0)
            {
                if (paymentTotal + warrantyAdjustment > 0)
                {
                    status = false;
                    throw new STLException(GetResource("M_OUTSTANDINGADJUSTMENT"));
                }
            }

            if (status)
            {
                outstPayments = total != 0;
            }

        }

        void ProcessDeliveries(string accountNo, decimal balance, ref bool status, SqlConnection conn, SqlTransaction trans)
        {
            decimal nonstockValue = 0;
            DDelivery del = new DDelivery();
            DLineItem line = new DLineItem();
            var loan = false;

            nonstockValue = del.GetNonStockValue(conn, trans, accountNo);

            line.AccountNumber = accountNo;
            line.GetItemsForAccount(conn, trans);  // 68181 PN/RD 09/08/06 Added conn, trans to resolve timeout issue

            // Check if Cash Loan account - CR1232 jec
            foreach (DataRow row in line.ItemDetails.Rows)
            {
                if (StockItemCache.Get(StockItemKeys.LOAN) == Convert.ToInt32(row[CN.ItemId]))
                {
                    loan = true;
                    //status = false;
                    //throw new STLException("This is a Cash Loan account which cannot be cancelled");
                }
            }

            foreach (DataRow row in line.ItemDetails.Rows)
            {
                if ((decimal)row[CN.Price] > 0 && status)
                {
                    // rdb added parentItemNo test
                    del.GetDeliveredQuantity(conn, trans, accountNo, 1,
                        Convert.ToInt32(row[CN.ItemId]),
                        (short)row[CN.StockLocn],
                        (string)row[CN.ContractNo], Convert.ToInt32(row[CN.ParentItemId]));         //IP - 17/05/11 - CR1212 - #3627 - changed to use ItemID and ParentItemID rather than ItemNo and ParentItemNo

                    if (del.DeliveredQuantity != 0)
                    {
                        if (loan == true)   //#8669
                        {
                            status = false;
                            throw new STLException("This is a disbursed Cash Loan account which cannot be cancelled");        // CR1232 jec
                        }
                        else
                        {
                            status = false;
                            throw new STLException(GetResource("M_OUTSTANDINGDELIVERIESS"));
                        }
                    }
                }
            }

            /*if(balance != nonstockValue)
            {
                status = false;
                throw new Exception(GetResource("M_OUTSTANDINGTRANSACTIONS"));
            }*/
        }

        public void AddCodeToAccount(SqlConnection conn, SqlTransaction trans,
            string accountNo, string code, int user, DateTime date)
        {
            DAccount acct = new DAccount();
            acct.AddCodeToAccount(conn, trans, accountNo, code, date, user, "");
        }

        public string GetApplicantTwoName(string customerID, string accountNo,
                                          out string jointid, out string relationship)
        {
            DAccount acct = new DAccount();
            return acct.GetApplicantTwoName(customerID, accountNo, out jointid, out relationship);
        }

        /// <summary>
        /// GetAccountsAddedTo
        /// </summary>
        /// <param name="acctno">string</param>
        /// <returns>DataTable</returns>
        /// 
        public DataTable GetAccountsAddedTo(SqlConnection conn, SqlTransaction trans, string acctno)
        {

            DFinTrans da = new DFinTrans();
            return da.GetAccountsAddedTo(conn, trans, acctno);

        }

        public void UpdateStatus(SqlConnection conn, SqlTransaction trans, string accountNo,
            DateTime dateChanged, string status)
        {
            DAccount acct = new DAccount(conn, trans, accountNo);

            acct.User = this.User;
            acct.CurrentStatus = status;
            acct.Save(conn, trans);
        }

        public DataSet GetStatusForAccount(string accountNo)
        {
            DataSet ds = new DataSet();
            DStatus stat = new DStatus();
            stat.GetStatusForAccount(accountNo);
            ds.Tables.Add(stat.StatusCodes);
            return ds;
        }

        /// <summary>
        /// GetDefaultTermsType
        /// </summary>
        /// <param name="termstype">string</param>
        /// <returns>string</returns>
        /// 
        public string GetDefaultTermsType(SqlConnection conn,
            SqlTransaction trans, short branchNo, bool isLoan)            //CR906 rdb 06/09/07 //CR903 jec 22/08/07
        {
            DTermsType da = new DTermsType();
            return da.GetDefault(conn, trans, branchNo, isLoan);
        }

        public DataSet GetRebatesTotal(int branchNo)
        {
            DRebatesTotal rebates = new DRebatesTotal();
            return rebates.GetRebatesTotal(branchNo);
        }

        public DateTime GetRebatesAsAt()
        {
            DRebatesTotal rebates = new DRebatesTotal();
            return rebates.AsAtDate;
        }

        public string[] GetAccountStatuses(StringCollection accounts)
        {
            DAccount acct = new DAccount();
            string[] s = new string[accounts.Count];
            int i = 0;

            foreach (string a in accounts)
            {
                acct.Populate(null, null, a);
                s[i++] = acct.CurrentStatus;
            }
            return s;
        }

        public string[] GetAccountNos(SqlConnection conn, SqlTransaction trans, string accountType, short branchNo, int number)
        {
            string[] accounts = new string[number];
            DAccount dacct = new DAccount();
            for (int i = 0; i < accounts.Length; i++)
            {
                GenerateAccountNumber(conn, trans, (string)Country[CountryParameterNames.CountryCode], branchNo, accountType, false, out accounts[i]);
                dacct.SaveManualCDV(conn, trans, accounts[i].Replace("-", ""));
            }
            return accounts;
        }

        public bool ManualCDVExists(string accountNo)
        {
            DAccount dacct = new DAccount();
            return dacct.ManualCDVExists(accountNo.Replace("-", ""));
        }

        public DataSet GetAccountAuditData(string accountNo)
        {
            DataSet changes = new DataSet();
            DAgreement agree = new DAgreement();
            DLineItem item = new DLineItem();
            DInstalPlan instal = new DInstalPlan();
            DDeliveryLoad dLoad = new DDeliveryLoad();

            // Increasing row counts for audit records as Singapore had
            // as issue where not all there changes were being displayed
            // in the Audit Changes tab in Account Details.
            changes.Tables.AddRange(new DataTable[] {   agree.GetAuditData(accountNo, 50),
                                                        item.GetAuditData(accountNo, 50),
                                                        instal.GetAuditData(accountNo, 50),
                                                        dLoad.GetAuditData(accountNo, 50)}); //IP - 18/11/09 - CR929 & 974 - Audit
            return changes;
        }

        public bool IsRepossessed(string accountNo)
        {
            DAccount acct = new DAccount();
            return acct.IsRepossessed(accountNo);
        }

        public void CreateManualRFAccount(SqlConnection conn,
            SqlTransaction trans,
            string countryCode,
            short branchNo,
            string customerID,
            string acctNo,
            bool isLoan,  //CR906 rdb 06/09/07 
            out bool rescore)
        {
            rescore = false;
            XmlNode stampDuty = null;
            decimal agreementTotal = 0;
            DateTime dateFirst = DateTime.MinValue.AddYears(1899);
            int agreementNo = 1;
            DataTable payMethodList = new DataTable(TN.PayMethodList);
            DataTable warrantyRenewalList = new DataTable(TN.WarrantyList);
            DataTable variableRates = new DataTable(TN.Rates);
            string referralReasons = string.Empty; //IP - 15/03/11 - #3314 - CR1245

            stampDuty = CreateStampDutyItems(conn, trans, branchNo, countryCode, AT.ReadyFinance);

            if (stampDuty != null)
                agreementTotal += Convert.ToDecimal(stampDuty.Attributes[Tags.Value]);

            if (acctNo.Length != 0)
            {
                //this.Lock(conn, trans, newAccountNo.Replace("-",""),User);

                string propResult = "";
                string bureauFailure = "";
                DateTime dateProp = DateTime.Today;

                int storeCardTransRefNo = 0;        //IP - 17/01/11 - Store Card
                                                    // CR903 - add branchno to GetDefaultTermsType()
                this.SaveNewAccount(conn, trans, acctNo, branchNo, AT.ReadyFinance,
                    "N", User, "", agreementTotal, 0, 0, stampDuty,
                    GetDefaultTermsType(conn, trans, branchNo, isLoan), "", countryCode, dateFirst, 0, 0, "", 0, 0, 
                    false, false, 0, false,
                    "", "", "", 0, null, 0, "", true, 0, 0, true, "", 0, "", branchNo, 0, payMethodList, 0, 0,
                    warrantyRenewalList, variableRates, ref propResult, ref dateProp, ref agreementNo, out bureauFailure, 0, true, false, "", null, out storeCardTransRefNo, out referralReasons); //17/01/11 - Added "" for storeCardAcctNo,null for storeCardNumber and 0 for storeCardTransRefNo //IP - 15/03/11 - #3314 - CR1245 - Return referral reasons

                //link the account to the customer
                this.AddCustomerToAccount(conn, trans, acctNo, customerID, "H", AT.ReadyFinance, out rescore);
            }
        }

        public void CreateManualAccount(SqlConnection conn,
            SqlTransaction trans,
            string countryCode,
            short branchNo,
            string customerID,
            string acctNo,
            string accountType,
            bool isLoan,  //CR906 rdb 06/09/07 
            out bool rescore)
        {
            rescore = false;
            XmlNode stampDuty = null;
            decimal agreementTotal = 0;
            DateTime dateFirst = DateTime.MinValue.AddYears(1899);
            int agreementNo = 1;
            DataTable payMethodList = new DataTable(TN.PayMethodList);
            DataTable warrantyRenewalList = new DataTable(TN.WarrantyList);
            DataTable variableRates = new DataTable(TN.Rates);
            string referralReasons = string.Empty; //IP - 15/03/11 - #3314 - CR1245

            stampDuty = CreateStampDutyItems(conn, trans, branchNo, countryCode, accountType);

            if (stampDuty != null)
                agreementTotal += Convert.ToDecimal(stampDuty.Attributes[Tags.Value]);

            if (acctNo.Length != 0)
            {
                //this.Lock(conn, trans, newAccountNo.Replace("-",""),User);

                string propResult = "";
                string bureauFailure = "";
                DateTime dateProp = DateTime.Today;

                int storeCardTransRefNo = 0;        //IP - 17/01/11 - Store Card

                // CR903 - add branchno to GetDefaultTermsType()
                this.SaveNewAccount(conn, trans, acctNo, branchNo, accountType,
                    "N", User, "", agreementTotal, 0, 0, stampDuty,
                    GetDefaultTermsType(conn, trans, branchNo, isLoan), "", countryCode, dateFirst, 0, 0, "", 0, 0, 
                    false, false, 0, false,
                    "", "", "", 0, null, 0, "", true, 0, 0, true, "", 0, "", branchNo, 0, payMethodList, 0, 0,
                    warrantyRenewalList, variableRates, ref propResult, ref dateProp, ref agreementNo, out bureauFailure, 0, true, false, "", null, out storeCardTransRefNo, out referralReasons); //17/01/11 - Added "" for storeCardAcctNo,null for storeCardNumber and 0 for storeCardTransRefNo //IP - 15/03/11 - #3314 - CR1245 - Return referral reasons

                //this.Unlock(conn, trans, newAccountNo.Replace("-",""), User);

                //link the account to the customer
                this.AddCustomerToAccount(conn, trans, acctNo, customerID, "H", accountType, out rescore);
            }
        }


        public DataSet GetActivitySegments(string acctNo)
        {
            DataSet ds = new DataSet();
            DAccount acct = new DAccount();

            acct.GetSegments(acctNo, (string)Country[CountryParameterNames.TallymanServerDB]);
            ds.Tables.Add(acct.Segments);

            acct.GetActivities(acctNo, (string)Country[CountryParameterNames.TallymanServerDB], 1);
            ds.Tables.Add(acct.Activities);

            acct.GetActivities(acctNo, (string)Country[CountryParameterNames.TallymanServerDB], 0);
            ds.Tables.Add(acct.Activities);

            return ds;
        }

        public bool IsCancelled(string accountNo)
        {
            DAccount acct = new DAccount();
            return acct.IsCancelled(accountNo);
        }

        public void ReverseCancellation(SqlConnection conn, SqlTransaction trans,
            string accountNo, string code, string notes)
        {
            DateTime dateReversed = DateTime.Now;

            DLineItem item = new DLineItem();
            DAccount acct = new DAccount(conn, trans, accountNo);
            DSundryCharge sundry = new DSundryCharge();

            acct.User = this.User;
            acct.ReverseCancellation(conn, trans, accountNo, dateReversed, code, notes);

            sundry.GetSundryChargeItem(conn, trans, acct.AccountType);
            DataTable dt = sundry.Items;
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string productCode = (string)row["chargecode"];
                    int itemID = Convert.ToInt32(row[CN.ItemId]);                //IP - 18/05/11 - CR1212 - #3627    

                    short locn = Convert.ToInt16(accountNo.Substring(0, 3));
                    // todo uat363 need to get the parentItemNo here 
                    item.UpdateItemQuantity(conn, trans, accountNo, 1,          //IP - 18/05/11 - CR1212 - #3627 - replaced productCode with itemID and "" for 0 for the parentItemID
                        itemID, locn, "", 1, 0);
                }
            }
            //update store card status now reversing cancellation on storecard payment history
            //if (accountNo.Substring(3, 1) == "9")
            //{
            //    throw new NotImplementedException();
            //var SCRep = new StoreCardRepository();
            //var Pay = new StorecardPaymentDetails { acctno = accountNo, LastUpdatedBy = this.User };
            //using (var ctx = Context.Create(conn, trans))
            //{
            //    SCRep.UpdateAccountStatus(ctx,a );
            //    ctx.SubmitChanges();
            //}
            //}
        }

        public DataSet GetPeriodEndDates(out string nextPeriodEnd)
        {
            DataSet ds = new DataSet();
            DAccount acct = new DAccount();

            nextPeriodEnd = "";

            acct.GetPeriodEndDates(out nextPeriodEnd);
            ds.Tables.Add(acct.EndDates);

            return ds;
        }

        // CR931 Forecast by Branch  jec 04/04/08
        public DataSet GetRebateForecastReports(string periodEnd, int branchNo)  // CR931 Forecast by Branch  jec 04/04/08
        {
            DataSet ds = new DataSet();
            DAccount acct = new DAccount();

            acct.GetRebateForecastReportA(periodEnd, branchNo);
            ds.Tables.Add(acct.RebateReport);

            acct.GetRebateForecastReportB(periodEnd, branchNo);
            ds.Tables.Add(acct.RebateReport);

            acct.GetRebateForecastReportC(periodEnd, branchNo);
            ds.Tables.Add(acct.RebateReport);

            acct.GetRebateForecastReportD(periodEnd, branchNo);
            ds.Tables.Add(acct.RebateReport);


            return ds;
        }

        public void RunRebateForecastReports(SqlConnection conn, SqlTransaction trans, string periodEnd)
        {
            DataSet ds = new DataSet();
            DAccount acct = new DAccount();

            acct.RunRebateForecastReports(conn, trans, periodEnd);
        }

        public string IsPaidAndTakenWarranty(string acctNo)
        {
            DAccount acct = new DAccount();
            acct.AccountNumber = acctNo;
            return acct.IsPaidAndTakenWarranty();
        }

        public DataSet GetBookings(SqlConnection conn, SqlTransaction trans,
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
            DataSet bookingsSet = new DataSet(); ;
            DAccount acct = new DAccount();

            DataTable bookingsList = acct.GetBookings(conn, trans,
                branchNo,
                empeeNo,
                fromDate,
                toDate,
                includeCash,
                includeHP,
                includeNonSec,
                includePaidTaken,
                includeRf,
                includeSec,
                rollUpResults);

            if (bookingsList != null)
            {
                bookingsSet.Tables.Add(bookingsList);
            }
            return bookingsSet;
        }


        public DataSet GetDeliveries(int bufferNo,
            int warehouseNo,
            DateTime fromDate,
            DateTime toDate,
            int includeSec,
            int includeNonSec,
            string operand)
        {
            DAccount acct = new DAccount();
            DataSet deliveriesSet = acct.GetDeliveries(bufferNo,
                warehouseNo,
                fromDate,
                toDate,
                includeSec,
                includeNonSec,
                operand);
            if (deliveriesSet.Tables.Count > 0)
            {
                deliveriesSet.Tables[0].TableName = TN.MonitorDeliveries;
            }
            return deliveriesSet;
        }

        public DataSet GetWarrantyRenewals(string acctNo, bool settled, bool ismenu, ref string custID)
        {
            DataSet ds = new DataSet();

            DAccount acct = new DAccount();
            acct.GetWarrantyRenewals(acctNo, settled, ismenu, ref custID);

            ds.Tables.Add(acct.AccountsList);

            return ds;
        }

        public DataSet GetWarrantyProductsByAccount(string acctNo)
        {
            DataSet ds = new DataSet();

            DAccount acct = new DAccount();
            acct.GetWarrantyProductsByAccount(acctNo);

            ds.Tables.Add(acct.WarrantyProductList);

            return ds;
        }

        public void SaveWarrantyRenewal(SqlConnection conn, SqlTransaction trans,
            string acctNo, short branch, DataTable renewals)
        {
            DAccount acct = new DAccount();
            acct.User = this.User;
            foreach (DataRow r in renewals.Rows)
            {
                acct.SaveWarrantyRenewal(conn, trans,
                    acctNo,
                    (string)r[CN.AccountNo],
                    (string)r[CN.RenewalContractNo],
                    Convert.ToInt32(r[CN.NewWarrantyID]),
                    branch,
                    (string)r[CN.ContractNo],
                    Convert.ToInt16(r[CN.WarrantyLocation]));
            }
        }
        public void AddWarrantRenewalCode(SqlConnection conn, SqlTransaction trans,
                                            string acctNo, string contractNo)
        {

            DAccount acct = new DAccount();
            acct.User = this.User;
            acct.AddWarrantRenewalCode(conn, trans, acctNo, (short)acct.User, contractNo);
        }

        public string EodArrearsCalculation(bool NextDay)
        {
            try
            {
                using (var conn = new SqlConnection(Connections.Default))
                {
                    conn.Open();

                    using (var trans = conn.BeginTransaction())
                    {
                        // calculate arrears for end of day - will take a long time.
                        var Acct = new DAccount();
                        Acct.EodArrearsCalculation(conn, trans, NextDay);
                        Acct.CalculateAvailableSpendForAllCusts(conn, trans);
                        trans.Commit();
                    }
                }
                return "P";
            }
            catch
            {
                return "F";
            }
        }


        private void RemoveStampDuty(SqlConnection conn, SqlTransaction trans,
            string accountNo, string countryCode)
        {
            bool exists = false;
            decimal transValue = 0;

            DSundryCharge sundry = new DSundryCharge();
            DAccount account = new DAccount(conn, trans, accountNo);
            sundry.GetSundryChargeItem(conn, trans, account.AccountType);
            DataTable sundryTable = sundry.Items;
            if (sundryTable.Rows.Count > 0)
            {
                DAgreement agreement = new DAgreement(conn, trans, accountNo, 1);
                short location = 0;
                XmlNode itemNode = null;

                BItem i = new BItem();

                DLineItem item = new DLineItem();
                item.AccountNumber = accountNo;
                item.GetItemsForAccount(conn, trans);

                foreach (DataRow row in item.ItemDetails.Rows)
                {
                    if ((string)row[CN.ItemNo] == "SD")
                    {
                        location = (short)row[CN.StockLocn];
                        exists = true;
                        break;
                    }
                }

                if (exists)
                {
                    DBranch branch = new DBranch();
                    int transRefNo = branch.GetTransRefNo(conn, trans, location);

                    BDelivery del = new BDelivery();

                    foreach (DataRow r in sundryTable.Rows)
                    {
                        var itemId = Convert.ToInt32(r[CN.ItemId]);
                        var itemNo = Convert.ToString(r["chargecode"]);
                        itemNode = i.GetItemDetails(conn, trans, itemId, location, account.AccountType, countryCode, false, false);  // CR1212 jec need to supply itemID not zero

                        decimal adminPrice = 0;
                        agreement.CashPrice = account.GetChargeableCashPrice(conn, trans, accountNo, ref adminPrice);

                        if (agreement.CashPrice == Convert.ToDecimal(itemNode.Attributes[Tags.UnitPrice].Value))
                        {
                            // todo uat363 need to get the parentItemNo here 
                            item.UpdateItemQuantity(conn, trans, accountNo, 1,
                                                    itemId, Convert.ToInt16(itemNode.Attributes[Tags.Location].Value),      //IP - 17/05/11 - CR1212 - #3627 - changed to use itemID rather than itemNo
                                                    itemNode.Attributes[Tags.ContractNumber].Value, 0, 0);                  //IP - 17/05/11 - CR1212 - #3627 - pass in 0 for parentItemID. Previously was ""

                            // 69231 ip rdb additional fixes (need to be copied to current)
                            decimal originalValue = Convert.ToDecimal(itemNode.Attributes[Tags.UnitPrice].Value);
                            item.AuditSource = AS.GRTCancel;
                            item.AgreementNumber = 1;
                            item.User = this.User;
                            item.ItemNumber = "SD";
                            item.StockLocation = Convert.ToInt16(itemNode.Attributes[Tags.Location].Value);
                            item.ContractNo = itemNode.Attributes[Tags.ContractNumber].Value;
                            item.ItemID = itemId;

                            if (item.ItemID > 0)
                                item.UpdateLineItemAudit(conn, trans, 1, 0, originalValue, 0, 0, 0);


                        }
                    }

                    if (AT.IsCreditType(account.AccountType))
                    {
                        // Recalculate Service Charge using new OrdVal amounts
                        // This expects the agreement cash price to exclude service charges
                        decimal chargeableAdminPrice = 0;
                        agreement.CashPrice = account.GetChargeableCashPrice(conn, trans, accountNo, ref chargeableAdminPrice);
                        // This will save the agreement with a new total

                        // lw 69231 rdb 18/10/07 fix copied from 5.0.6.0
                        //BDelivery del = new BDelivery();
                        agreement.AuditSource = AS.GRTCancel;
                        del.User = this.User;
                        del.RecalculateServiceCharge(conn, trans, account, agreement);
                    }
                    else
                    {
                        // Save new Agreement Total
                        agreement.DateChange = DateTime.Now;
                        agreement.EmployeeNumChange = this.User;
                        agreement.Save(conn, trans);
                    }

                    // 69411 DeliverNonStocks must not be called if SD item does not exist in the database
                    DDelivery delivery = new DDelivery();
                    int n = delivery.CheckForSD(conn, trans, accountNo);
                    if (n == 1)
                    {
                        // lw 69231 rdb 18/10/07 fix copied from 5.0.6.0
                        del.User = this.User;
                        del.DeliverNonStocks(conn, trans, accountNo, account.AccountType,
                        countryCode, location, transRefNo, ref transValue, 1);
                    }

                    account.OutstandingBalance += transValue;
                    account.Save(conn, trans);
                }
            }
        }

        public DataSet LoadAllocationsForReprint(int empeeNo)
        {
            DataSet ds = new DataSet();
            DFollUpAlloc alloc = new DFollUpAlloc();

            alloc.EmployeeNo = empeeNo;
            alloc.LoadAllocationsForReprint(Convert.ToInt32(Country[CountryParameterNames.MonthsAllocated]));
            ds.Tables.Add(alloc.AllocAccounts);
            return ds;
        }

        public DataSet LoadAllocationDetails(int empeeNo, DateTime dateAllocated)
        {
            DataSet ds = new DataSet();
            DFollUpAlloc alloc = new DFollUpAlloc();

            alloc.EmployeeNo = empeeNo;
            alloc.DateAllocated = dateAllocated;
            alloc.LoadAllocationDetails();
            ds.Tables.Add(alloc.AllocAccounts);
            return ds;
        }

        public void UpdateAllocForReprint(SqlConnection conn, SqlTransaction trans,
            string acctNo, int empeeNo, DateTime dateAllocated, bool batch)
        {
            DataSet ds = new DataSet();
            DFollUpAlloc alloc = new DFollUpAlloc();

            alloc.AccountNo = acctNo;
            alloc.EmployeeNo = empeeNo;
            alloc.DateAllocated = dateAllocated;
            alloc.UpdateAllocForReprint(conn, trans, batch);
        }

        public void UpdateRebateTotals(SqlConnection conn, SqlTransaction trans,
            string acctNo,
            DateTime fromThresDate,
            DateTime toThresDate,
            DateTime acctsFromDate,
            DateTime ruleChangeDate,
            DateTime rebateDate,
            out decimal poRebate,
            out decimal poRebateWithin12Mths,
            out decimal poRebateAfter12Mths)
        {
            DRebatesTotal rebates = new DRebatesTotal();
            rebates.UpdateTotals(conn, trans, acctNo, fromThresDate, toThresDate, acctsFromDate,
                ruleChangeDate, rebateDate, out poRebate, out poRebateWithin12Mths,
                out poRebateAfter12Mths);
        }

        private void MaintainPurchaseOrderStockLevel(SqlConnection conn, SqlTransaction trans,
            string accountNo,
            XmlNode item, BItem lineItem,
            bool collection, int agreementNo)
        {
            string purchaseOrderNumber = item.Attributes[Tags.PurchaseOrderNumber].Value;

            lineItem.AccountNumber = accountNo;
            lineItem.ItemNumber = item.Attributes[Tags.Code].Value;
            lineItem.ItemId = Convert.ToInt32(item.Attributes[Tags.ItemId].Value);
            lineItem.StockLocation = Convert.ToInt16(item.Attributes[Tags.Location].Value);
            lineItem.Quantity = Convert.ToDouble(item.Attributes[Tags.Quantity].Value);
            if (collection)
                lineItem.Quantity *= -1;
            lineItem.AgreementNumber = agreementNo;
            lineItem.MaintainPurchaseOrderStockLevel(conn, trans, purchaseOrderNumber);
        }

        public DataTable GetDeliveryNotes(string acctNo, SqlConnection conn, SqlTransaction trans, int branchNo, int buffNo,
                                        DateTime dateReqDel, string addtype, string timeReqDel,
                                        out decimal amountPayable, out decimal charges, out bool cod)
        {
            DDelivery dDel = new DDelivery();
            DSchedule sched = new DSchedule();
            sched.AccountNumber = this.AccountNumber; //IP - 10/02/10 - CR1048 (Ref:3.1.3) Merged - Malaysia Enhancements (CR1072)
            sched.GetDeliveryNotes(conn, trans, buffNo, branchNo);
            dDel.GetCODCharges(conn, trans, acctNo, buffNo, 1, out amountPayable,
                out charges, out cod, dateReqDel, timeReqDel, addtype, branchNo);

            return sched.Schedules;
        }
        public string AccountGetAccountNoByBuffNo(int stockLocn, int buffNo)
        {
            DAccount dAcct = new DAccount();
            string acctNo = dAcct.AccountGetAccountNoByBuffNo(stockLocn, buffNo);

            return acctNo;
        }

        public void AuditReprint(SqlConnection conn, SqlTransaction trans, string accountNo, int agreementNo, string docType)
        {
            DAccount accountPrint = new DAccount();
            accountPrint.AuditReprint(conn, trans, accountNo, agreementNo, docType, this.User);

            // #12594
            var document = docType == "T" ? "TaxInvoice" : docType;
            EventStore.Instance.Log(new { AccountNo = accountNo, AgreementNo = agreementNo, User = this.User }, document, EventCategory.Printing
                                          , new { empeeno = this.User });
        }

        public void AuditDiscount(SqlConnection conn, SqlTransaction trans, string accountNo, int agreementNo,
            string discountItemNo, string parentItemNo, short stockLocn, decimal amount, int salesPerson, int authorisedBy)
        {
            DAccount accountDiscount = new DAccount();
            accountDiscount.AuditDiscount(conn, trans,
                accountNo, agreementNo, discountItemNo, parentItemNo, stockLocn, amount, salesPerson, authorisedBy);
        }

        public void SecuritiseAccounts(SqlConnection conn, SqlTransaction trans, string optionCode, int runNo)
        {
            decimal totalBalance = 0;
            int totalCount = 0;
            DAccount secAccounts = new DAccount();
            secAccounts.SecuritiseAccounts(conn, trans, this.User, runNo, out totalBalance, out totalCount);
            if (totalBalance > 0)
            {
                DInterfaceControl interfaceValue = new DInterfaceControl();
                int branchNo = Convert.ToInt16(Country[CountryParameterNames.HOBranchNo]);
                interfaceValue.AddInterfaceValue(optionCode, runNo, CountType1.Processed, "", branchNo, "", totalCount, totalBalance);
            }
            secAccounts.SecuritiseAccountsReport14(conn, trans, runNo);
            secAccounts.SecuritiseAccountsReport15(conn, trans);
        }

        public DataSet FincoTransactions(SqlConnection conn, SqlTransaction trans, DateTime datefrom, DateTime dateto, string transtypeset)
        {
            DataSet ds = new DataSet();
            try
            {


                DAccount FAccounts = new DAccount();
                FAccounts.FincoTransactions(conn, trans, datefrom, dateto, transtypeset);
                ds.Tables.Add(FAccounts.AccountsList);




            }
            catch (Exception)
            {

            }
            return ds;

        }

        public DataSet FincoBalances(SqlConnection conn, SqlTransaction trans, DateTime datefrom, DateTime dateto)
        {
            DataSet ds = new DataSet();

            try
            {

                DAccount FAccounts = new DAccount();
                FAccounts.FincoBalances(conn, trans, datefrom, dateto);
                ds.Tables.Add(FAccounts.AccountsList);


            }
            catch (Exception)
            {

            }
            return ds;

        }



        public string LetterAndCharges(int runNo)
        {
            // Alex to add the rest
            SqlConnection conn = null;
            SqlTransaction trans = null;
            string Eodresult = "F";
            try
            {
                //remove previous error messages if rerun
                DInterfaceError ierror = new DInterfaceError();
                ierror.RunNumber = runNo;
                ierror.Interface = "CHARGES";
                ierror.RemoveOld(conn, trans);

                DAccount account = new DAccount();
                account.RunArrearsLettersandCharges(conn, trans, runNo);
                //68935 thank you letters generated after account settled
                account.RunAddtoLetters(conn, trans, runNo);
                account.AccountGetForRenewal(conn, trans);
                //IP - 69201 WOC Reminder Letters
                account.WOCLetters(conn, trans);

                Eodresult = "P";
            }
            catch (Exception)
            {

            }

            return Eodresult;


        }

        public void LettersGenerateCSVfiles(int runNo, string type)
        {

            SqlConnection conn = null;
            SqlTransaction trans = null;
            try
            {
                DAccount account = new DAccount();
                account.LettersGenerateCSVfiles(conn, trans, runNo, type);

            }
            catch (Exception)
            {

            }
        }

        public bool IsWarrantyRenewal(string accountNo)
        {
            DAccount acct = new DAccount();
            return acct.IsWarrantyRenewal(accountNo);
        }
        // Check if decimal quantity allowed
        public bool ValidDecimal(int itemId)
        {
            DAccount dec = new DAccount();
            return dec.ValidDecimal(itemId);
        }

        void ProcessSchedule(SqlConnection conn, SqlTransaction trans, string acctNo, int agrmtNo,
                             string delOrColl, string itemNo, short locn, double qty, string contractNo, int parentItemId)
        {
            DSchedule sched = new DSchedule();
            sched.User = this.User;

            int buffNo = 0;

            if (contractNo.Length == 0)
            {
                DBranch branch = new DBranch();
                buffNo = branch.GetBuffNo(conn, trans, locn);               // #17290 - fix
            }

            sched.AccountNumber = acctNo;
            sched.AgreementNumber = agrmtNo;
            sched.DateDelPlan = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            sched.DeliveryOrCollection = delOrColl;
            sched.ItemNumber = itemNo;
            sched.StockLocation = locn;
            sched.Quantity = qty;
            sched.BuffNo = buffNo;
            sched.BuffBranchNo = locn;
            sched.ContractNo = contractNo;
            sched.ParentItemID = parentItemId;
            sched.Write(conn, trans, this.User);
        }

        void SetUpKitDiscount(ref XmlNode doc, XmlNode kitNode, XmlDocument create, ref XmlNode compParent, int itemId,
                              short branchCode, string accountType, double kitQty, string countryCode, bool dutyFree,
                              bool taxExempt, short promoBranch, decimal compTotal, DStockItem stock, ref string err)
        {
            //bool status = true;
            XmlNode found = null;
            decimal kitDiscount = 0;
            decimal discountPercentage = 0;
            decimal discountValue = 0;
            string key = "";

            //find the kitPrice which represents the amount to use for the discount
            if (stock.GetKitDiscount(itemId, branchCode, accountType, countryCode, dutyFree, taxExempt) == (int)Return.Success)
            {
                XmlNode clone = compParent.Clone();
                kitDiscount = Convert.ToDecimal(stock.KitDiscount);
                if (kitDiscount != 0)
                {
                    if (compTotal > 0)
                    {
                        // calcualte the discount percentage to be applied across the components
                        discountPercentage = (kitDiscount / (compTotal / (decimal)kitQty)) * 100;  //LW 70876
                        //this.CountryRound(ref discountPercentage); Rounding is not required as it's not displayed anywhere - NM 19/03/2009
                    }

                    foreach (XmlNode c in clone.ChildNodes)
                    {
                        discountValue = (Convert.ToDecimal(c.Attributes[Tags.UnitPrice].Value) * discountPercentage) / 100; //LW 70876
                        if ((bool)Country[CountryParameterNames.NoCents]) discountValue = Decimal.Floor(discountValue);

                        var discItem = new StockRepository().GetKitDiscountItem(c.Attributes[Tags.Category].Value);
                        if (discItem == null)
                            continue;

                        var discItemId = discItem.Id;
                        var discItemNo = discItem.IUPC;

                        key = discItemId + "|" + branchCode.ToString();

                        // check to see if this kit discount already exists and,
                        // if so, increase the value by the current discount amount
                        found = compParent.SelectSingleNode("//Item[@Key = '" + key + "']");
                        if (found != null)
                        {
                            decimal tmpValue = Convert.ToDecimal(found.Attributes[Tags.Value].Value) + (discountValue * (decimal)kitQty);
                            found.Attributes[Tags.UnitPrice].Value = StripCurrency(tmpValue.ToString(DecimalPlaces));
                            found.Attributes[Tags.Value].Value = StripCurrency(tmpValue.ToString(DecimalPlaces));
                        }
                        else
                        {
                            //Create a node for the discount 
                            XmlNode discNode = create.CreateElement(Elements.Item);

                            discNode.Attributes.Append(xml.Attribute(create, Tags.Key, discItemId + "|" + branchCode.ToString()));
                            discNode.Attributes.Append(xml.Attribute(create, Tags.Type, IT.KitDiscount));

                            // calculate discount value on each component
                            discNode.Attributes.Append(xml.Attribute(create, Tags.Value, StripCurrency((discountValue).ToString(DecimalPlaces))));

                            stock.GetItemDetails(null, null, discItemId, branchCode, accountType, countryCode, dutyFree, taxExempt);

                            //Adding the discount Tax Rate so that we can use this in AddKitDiscountToKit to apply the Tax Rate on the Discount item.
                            discNode.Attributes.Append(xml.Attribute(create, Tags.TaxRate, stock.TaxRate.ToString()));

                            AddKitDiscountToKit(discItemId, discItemNo, create, ref compParent, kitNode, discNode,
                                                kitDiscount, kitQty, stock, branchCode, discountValue);
                        }

                        CountryRound(ref discountValue);
                        kitDiscount -= discountValue;
                    }

                    // if the kit discount <> 0 at this stage, the difference needs to be added onto 
                    // a kit discount line, so that the individual kit discounts equal the total
                    // discount value taken from the stockitem table
                    if (kitDiscount != 0)
                    {
                        XmlNodeList kitDiscounts = compParent.SelectNodes("//Item[@Type='KitDiscount' and @Quantity!='0']");
                        foreach (XmlNode ds in kitDiscounts)
                        {
                            if (Convert.ToDecimal(ds.Attributes[Tags.Value].Value) != 0)
                            {
                                decimal dsValue = Convert.ToDecimal(ds.Attributes[Tags.Value].Value);
                                dsValue += kitDiscount;
                                ds.Attributes[Tags.UnitPrice].Value = StripCurrency(dsValue.ToString(DecimalPlaces));
                                ds.Attributes[Tags.Value].Value = StripCurrency(dsValue.ToString(DecimalPlaces));
                                break;
                            }
                        }
                    }
                }
            }
        }

        void AddKitDiscountToKit(int itemId, string itemNo, XmlDocument create, ref XmlNode compParent, XmlNode kitNode, XmlNode discNode,
                                decimal kitDiscount, double kitQty, DStockItem stock, short stockLocn, decimal discountValue)
        {
            discNode.Attributes.Append(xml.Attribute(create, Tags.ItemId, itemId.ToString()));
            discNode.Attributes.Append(xml.Attribute(create, Tags.Code, itemNo));
            discNode.Attributes.Append(xml.Attribute(create, Tags.Location, stockLocn.ToString()));
            discNode.Attributes.Append(xml.Attribute(create, Tags.AvailableStock, stock.AvailableStock.ToString()));
            discNode.Attributes.Append(xml.Attribute(create, Tags.DamagedStock, stock.DamagedStock.ToString()));
            discNode.Attributes.Append(xml.Attribute(create, Tags.Description1, stock.ProductDesc1));
            discNode.Attributes.Append(xml.Attribute(create, Tags.Description2, stock.ProductDesc2));
            discNode.Attributes.Append(xml.Attribute(create, Tags.SupplierCode, stock.SupplierCode));
            discNode.Attributes.Append(xml.Attribute(create, Tags.UnitPrice, StripCurrency(discountValue.ToString(DecimalPlaces))));
            discNode.Attributes.Append(xml.Attribute(create, Tags.CashPrice, StripCurrency(stock.CashPrice.ToString(DecimalPlaces))));
            discNode.Attributes.Append(xml.Attribute(create, Tags.HPPrice, StripCurrency(stock.HPPrice.ToString(DecimalPlaces))));
            discNode.Attributes.Append(xml.Attribute(create, Tags.DutyFreePrice, StripCurrency(stock.DutyFreePrice.ToString(DecimalPlaces))));
            discNode.Attributes.Append(xml.Attribute(create, Tags.ValueControlled, stock.ValueControlled.ToString()));
            discNode.AppendChild(create.CreateElement(Elements.RelatedItem));
            discNode.Attributes.Append(xml.Attribute(create, Tags.Quantity, kitQty.ToString()));
            discNode.Attributes.Append(xml.Attribute(create, Tags.Value, StripCurrency((discountValue * Convert.ToDecimal(kitQty)).ToString(DecimalPlaces))));
            discNode.Attributes.Append(xml.Attribute(create, Tags.DeliveryDate, kitNode.Attributes[Tags.DeliveryDate].Value));
            discNode.Attributes.Append(xml.Attribute(create, Tags.DeliveryTime, kitNode.Attributes[Tags.DeliveryTime].Value));
            discNode.Attributes.Append(xml.Attribute(create, Tags.DeliveryArea, kitNode.Attributes[Tags.DeliveryArea].Value));
            discNode.Attributes.Append(xml.Attribute(create, Tags.DeliveryProcess, kitNode.Attributes[Tags.DeliveryProcess].Value));
            discNode.Attributes.Append(xml.Attribute(create, Tags.BranchForDeliveryNote, kitNode.Attributes[Tags.BranchForDeliveryNote].Value));
            discNode.Attributes.Append(xml.Attribute(create, Tags.ColourTrim, ""));
            discNode.Attributes.Append(xml.Attribute(create, Tags.TaxRate, discNode.Attributes[Tags.TaxRate].Value));
            discNode.Attributes.Append(xml.Attribute(create, Tags.DeliveredQuantity, "0"));
            discNode.Attributes.Append(xml.Attribute(create, Tags.PlannedDeliveryDate, ""));
            discNode.Attributes.Append(xml.Attribute(create, Tags.CanAddWarranty, System.Boolean.FalseString));
            discNode.Attributes.Append(xml.Attribute(create, Tags.DeliveryAddress, ""));
            discNode.Attributes.Append(xml.Attribute(create, Tags.QuantityDiff, "Y"));
            discNode.Attributes.Append(xml.Attribute(create, Tags.ScheduledQuantity, "0"));
            discNode.Attributes.Append(xml.Attribute(create, Tags.TaxAmount, "0"));
            discNode.Attributes.Append(xml.Attribute(create, Tags.ContractNumber, ""));
            discNode.Attributes.Append(xml.Attribute(create, Tags.ReturnItemNo, itemNo));
            discNode.Attributes.Append(xml.Attribute(create, Tags.ReplacementItem, false.ToString()));
            discNode.Attributes.Append(xml.Attribute(create, Tags.ReturnLocation, stockLocn.ToString()));
            discNode.Attributes.Append(xml.Attribute(create, Tags.FreeGift, stock.IsFreeGift.ToString()));
            discNode.Attributes.Append(xml.Attribute(create, Tags.ExpectedReturnDate, ""));
            discNode.Attributes.Append(xml.Attribute(create, Tags.Damaged, kitNode.Attributes[Tags.Damaged].Value));
            discNode.Attributes.Append(xml.Attribute(create, Tags.Assembly, kitNode.Attributes[Tags.Assembly].Value));
            discNode.Attributes.Append(xml.Attribute(create, Tags.ProductCategory, kitNode.Attributes[Tags.ProductCategory].Value));
            discNode.Attributes.Append(xml.Attribute(create, Tags.Category, stock.Category.ToString())); //IP - 10/03/11 - #3304 - Category was not being returned previously
            discNode.Attributes.Append(xml.Attribute(create, Tags.SPIFFItem, Convert.ToBoolean(0).ToString()));
            discNode.Attributes.Append(xml.Attribute(create, Tags.SalesBrnNo, Config.BranchCode));
            discNode.Attributes.Append(xml.Attribute(create, Tags.RepoItem, "false"));
            discNode.Attributes.Append(xml.Attribute(create, Tags.Express, kitNode.Attributes[Tags.Express].Value));    //IP - 07/06/12 - #10229 - Warehouse & Deliveries
            discNode.Attributes.Append(xml.Attribute(create, Tags.WarrantyType, ""));      //#17883       //#17434
            discNode.Attributes.Append(xml.Attribute(create, Tags.DateDelivered, ""));     //#19016
            compParent.AppendChild(discNode);
        }

        //string GetKitDiscountItem(string category)
        //{
        //    string itemNo = "";
        //    DCode code = new DCode();
        //    code.GetCategoryCodes("KCD", "L", TN.Discounts);
        //    DataView dv = new DataView(code.Codes);

        //    dv.RowFilter = CN.Code + " = " + category;

        //    if (dv.Count > 0)
        //    {
        //        itemNo = (string)dv[0][CN.Reference];
        //    }

        //    return itemNo;
        //}

        //IP - 26/08/09 - UAT(819) - Added bool strategyHasWorklists
        public DataSet LoadStrategiesandWorklistsByAcctNo(string acctNo, out bool strategyHasWorklists)
        {
            Function = "BAccount::LoadStrategiesandWorklistsByAcctNo()";

            int result = 0;
            strategyHasWorklists = false;
            DataSet ds = new DataSet();
            DCollectionsModule collections = new DCollectionsModule();

            result = collections.GetStrategiesbyacctno(acctNo, out strategyHasWorklists);

            ds.Tables.Add(collections.Strategies);

            if (result == 0)
            {
                result = collections.GetWorklistsbyacctno(acctNo);
            }

            ds.Tables.Add(collections.Worklists);

            return ds;
        }

        //CR868
        public void OpenStageForRescore(SqlConnection conn, SqlTransaction trans, string accountNo, string custID)
        {
            var status = true;
            var highStatus = "";
            var dateLastScored = DateTime.MinValue.AddYears(1899);

            DProposalFlag propFlag = null;
            var prop = new DProposal();

            prop.GetDetailsForRescore(conn, trans, accountNo, custID, ref dateLastScored, ref highStatus);

            if (highStatus.Length == 0)
            {
                highStatus = "0";
            }

            if (Convert.ToInt32(Country[CountryParameterNames.RescoreMonths]) == 0)
            {
                ReOpenS1 = true;
                ReOpenS2 = true;
                status = false;
            }

            if (status)
            {
                //New code added for log 6632907
                DateTime dateAggrementDocStage = DateTime.Now.AddMonths(-Convert.ToInt32(Country[CountryParameterNames.RescoreMonths]));
                if (dateLastScored <= dateAggrementDocStage)
                {
                    var countryStatus = (string)Country[CountryParameterNames.RescoreStatus];
                    ReOpenS1 = (Convert.ToInt16(highStatus) > Convert.ToInt16(countryStatus));
                    ReOpenS2 = true;
                }
            }

            if (ReOpenS1)
            {
                propFlag = new DProposalFlag();
                propFlag.UnClearFlag(conn, trans, accountNo, SS.S1, true, this.User);
                propFlag = null;
            }

            if (ReOpenS2)
            {
                propFlag = new DProposalFlag();
                propFlag.UnClearFlag(conn, trans, accountNo, SS.S2, true, this.User);
                propFlag = null;
            }
        }

        // Instant Credit Approval          CR907  jec 31/07/07
        public string InstantCredit(string customerID, string accountNo)
        {
            using (var conn = new SqlConnection(Connections.Default))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    var approved = new DAccount().InstantCredit(conn, trans, customerID, accountNo);
                    trans.Commit();
                    return approved;
                }
            }
        }

        public bool IsGiftItem(int itemId, string location)      // RI
        {
            var acct = new DAccount();
            return acct.IsGiftItem(itemId, location);           // RI
        }

        public bool GiftItemAdded(SqlConnection conn, SqlTransaction trans, XmlNode lineItems)
        {
            // rdb 08/04/08 uat261 faulty logic here, if no giftItems exist
            // will return true as it is unable to find a gift when there are none to set newGiftItem to false
            // (this eventually leads to holdprop being reset when a cash account is saved without any changes
            // therefore requiring DA again)
            // bool newGiftItem = true;
            bool newGiftItem = false;

            var list = new List<int>();

            if (lineItems != null)
            {
                XmlNodeList freeGifts = lineItems.SelectNodes("//Item[@FreeGift = 'True']");
                // rdb 08/04/08 uat261
                newGiftItem = freeGifts.Count > 0;
                foreach (XmlNode node in freeGifts)
                {
                    if (Convert.ToDecimal(node.Attributes[Tags.Value].Value) == 0)
                    {
                        list.Add(Convert.ToInt32(node.Attributes[Tags.ItemId].Value));
                    }
                }
            }

            DLineItem lineItem = new DLineItem();
            DataTable dtAuditDetails = lineItem.GetAuditData(AccountNumber, 50, conn, trans);

            DataRow[] auditRows = dtAuditDetails.Select();
            foreach (DataRow row in auditRows)
            {
                if (list.Contains(Convert.ToInt32(row[CN.ItemId])))         // RI jec 27/05/11
                {
                    newGiftItem = false;
                    break;
                }
            }

            list = null;
            lineItem = null;

            return newGiftItem;
        }

        public string AccountGetAccountType(string acctno)
        {
            DAccount acct = new DAccount();
            return acct.AccountGetAccountType(acctno);
        }

        public bool GetAccountHasBDW(string accountNo)
        {
            DAccount acct = new DAccount();
            return acct.GetAccountHasBDW(accountNo);
        }

        //IP - 19/05/08 - Method that returns 'Application Status' as well as other 
        //account details to be displayed in the 'Account Status' screen.
        public DataTable AccountStatusGet(DateTime dateFrom, DateTime dateTo, int branchno)
        {
            DAccount dacct = new DAccount();
            DataTable accountStatusDet;

            accountStatusDet = dacct.AccountStatusGet(dateFrom, dateTo, branchno);
            return accountStatusDet;
        }

        //IP - 01/10/08 - Special Arrangements Screen (Credit Collections)
        //Method retrieves the 'Outstanding Balance', 'Arrears' and 'Instalment Amount'
        //for an account to be displayed on the 'Special Arrangements' screen.
        public SPAAccountDetails GetSPAAcctDetails(string acctNo)
        {
            DAccount dacct = new DAccount();
            SPAAccountDetails spaacctdetails = new SPAAccountDetails();

            try
            {

                spaacctdetails = dacct.GetSPAAcctDetails(acctNo);

            }
            catch (Exception e)
            {

                throw e;
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
            DataTable arrangementSchedule = new DataTable();

            DAccount dacct = new DAccount();
            arrangementSchedule = dacct.SPACalculateArrangementSchedule(acctNo, period, arrangementAmt, numberOfInstalments,
                                                        instalmentAmt, oddPaymentAmt, firstPaymentDate, numberRemainInstals,
                                                        remainInstalAmt, out finalPayDate);
            return arrangementSchedule;

        }

        //IP & JC - 15/01/09 - CR976 
        public void SPAWriteArrangementSchedule(SqlConnection conn, SqlTransaction trans, int empeeno,
                                                        DataTable dtSPADetails)
        {
            DAccount dacct = new DAccount();

            //Loop through each of the accounts and only write a record
            //for the SPA arrangement if an arrangement amount has been entered.
            foreach (DataRow dr in dtSPADetails.Rows)
            {
                if (Convert.ToDecimal(dr[CN.ArrangementAmount]) > 0)
                {
                    dacct.SPAWriteArrangementSchedule(conn, trans, empeeno,
                                                        dr[CN.AccountNo].ToString(),
                                                        Convert.ToChar(dr[CN.Period]),
                                                        Convert.ToInt32(dr[CN.NoOfIns]),
                                                        Convert.ToDecimal(dr[CN.InstalAmount]),
                                                        Convert.ToDecimal(dr[CN.OddPayment].ToString()),
                                                        Convert.ToDateTime(dr[CN.FirstPayDate].ToString()),
                                                        Convert.ToString(dr[CN.ReasonCode].ToString()),
                                                        Convert.ToInt32(dr[CN.NoOfRemainInstals]),
                                                        Convert.ToDecimal(dr[CN.RemainInstalAmt]));
                }
            }
        }


        //IP & JC - CR976 - 21/01/09
        //Method that will write the new Instalplan and Agreement records for Extended Term SPA.
        public void SPAWriteRefinance(SqlConnection conn, SqlTransaction trans, int empeeno,
                                                        DataTable dtSPADetails)
        {
            bool rescore = false;
            string NewAccount = "";
            double NewAcctNo = 0;
            string propResult = "";
            string bureauFailure = "";
            DateTime dateProp = DateTime.Today;
            short branchno = 0;
            string scoringBand = string.Empty;      //IP - 23/09/10 - UAT(1017) UAT5.2

            XmlNode stampDuty = null;
            //decimal agreementTotal = 0;
            int agreementNo = 1;
            DataTable payMethodList = new DataTable(TN.PayMethodList);
            DataTable warrantyRenewalList = new DataTable(TN.WarrantyList);
            DataTable variableRates = new DataTable(TN.Rates);
            DAccount code = new DAccount();


            //decimal sumBalances = 0;
            //decimal newCashPrice = 0;
            //decimal newAgreementTotal = 0;
            //decimal monthlyInstal = 0;
            //decimal finalInstal = 0;
            //int newNoOfInstalments = 0;

            //DAccount dacct = new DAccount();
            conn.Open();
            trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);

            foreach (DataRow dr in dtSPADetails.Rows)
            {
                NewAccount = Convert.ToString(dr[CN.AccountNo]);
                NewAcctNo = Convert.ToDouble(NewAccount);
                NewAcctNo += 1;
                //NewAccount = Convert.ToString(NewAcctNo);


                branchno = Convert.ToInt16((NewAccount.Substring(0, 3)));
                decimal refinDeposit = Convert.ToDecimal(dr[CN.RefinDeposit]);
                StringCollection oldAcctno = new StringCollection();
                oldAcctno.Add(Convert.ToString(dr[CN.AccountNo]));
                //moved after
                NewAccount = Convert.ToString(NewAcctNo);
                scoringBand = Convert.ToString(dr[CN.ScoringBand]);         //IP - 23/09/10 - UAT(1017) UAT5.2

                int storeCardTransRefNo = 0;        //IP - 17/01/11 - Store Card
                string referralReasons = string.Empty; //IP - 15/03/11 - #3314 - CR1245

                this.SaveNewAccount(conn, trans, NewAccount, branchno, dr[CN.AcctType].ToString(),
                "N", this.User, "", Convert.ToDecimal(dr[CN.ArrangementAmount]), Convert.ToDecimal(dr[CN.RefinDeposit]), Convert.ToDecimal(dr[CN.ServiceCharge]), stampDuty,
                Convert.ToString(dr[CN.TermsType]), scoringBand, Convert.ToString(Country[CountryParameterNames.CountryCode]), Convert.ToDateTime(dr[CN.FirstPayDate].ToString()),   //IP - 23/09/10 - UAT(1017) UAT5.2
                Convert.ToDecimal(dr[CN.InstalAmount]), Convert.ToDecimal(dr[CN.OddPayment].ToString()), "", Convert.ToInt32(dr[CN.NoOfIns]), 0, 
                false, false, 0, false,
                "", "", "", 0, null, 0, "", true, 0, 0, true, "", 0, "", branchno, 0, payMethodList, Convert.ToInt16(dr[CN.DueDay]), 0, warrantyRenewalList, variableRates,
                ref propResult, ref dateProp, ref agreementNo, out bureauFailure, this.User, true, false, "", null, out storeCardTransRefNo, out referralReasons); //17/01/11 - Added "" for storeCardAcctNo,null for storeCardNumber and 0 for storeCardTransRefNo //IP - 15/03/11 - #3314 - CR1245 - Return referral reasons


                AddCustomerToAccount(conn, trans,
                    Convert.ToString(NewAccount), Convert.ToString(dr[CN.CustID]),
                    "H", Convert.ToString(dr[CN.AcctType]), out rescore);

                //trans.Commit();        // commit changes so far
                //if (conn.State != ConnectionState.Closed)
                //    conn.Close();

                //conn = new SqlConnection(Connections.Default);
                //conn.Open();
                //trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
                //trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
                //TO DO   Process Refinance similar to AddTo process
                //    CalculateRefinance(NewAccount, oldAcctno, Convert.ToString(Country[CountryParameterNames.CountryCode]), branchno,
                //Convert.ToString(dr[CN.TermsType]), "", Convert.ToDecimal(dr[CN.RefinDeposit]), Convert.ToDecimal(dr[CN.ServiceCharge]),
                //Convert.ToDecimal(dr[CN.InstalAmount]), Convert.ToDecimal(dr[CN.OddPayment]),
                //Convert.ToInt16(dr[CN.NoOfIns]), 0, true, out sumBalances,
                //out newCashPrice, out newAgreementTotal, out monthlyInstal, out finalInstal, out newNoOfInstalments);

                ProcessRefinance(conn, trans, NewAccount, oldAcctno, Convert.ToString(Country[CountryParameterNames.CountryCode]),
                Convert.ToString(dr[CN.TermsType]), scoringBand, Convert.ToDecimal(dr[CN.RefinDeposit]), Convert.ToDecimal(dr[CN.ServiceCharge]), //IP - 23/09/10 - UAT(1017) UAT5.2 - Added ScoringBand
                Convert.ToDecimal(dr[CN.InstalAmount]), Convert.ToDecimal(dr[CN.OddPayment]), Convert.ToInt16(dr[CN.NoOfIns]), 0, true);

                if (Convert.ToBoolean(dr[CN.FreezeIntAdmin]))
                {
                    code.AddCodeToAccount(conn, trans, NewAccount, "F", Convert.ToDateTime(DateTime.Now), this.User, "");
                }


            }
            trans.Commit();        // commit changes so far
            if (conn.State != ConnectionState.Closed)
                conn.Close();

        }

        //IP - 05/02/09 - CR971 - Method will Unarchive/ un-settle accounts
        public void UnarchiveUnsettle(SqlConnection conn, SqlTransaction trans,
                                                         string acctNo,
                                                         bool archivedAcct,
                                                         bool unsettleAcct)
        {
            DAccount dacct = new DAccount();
            dacct.UnarchiveUnsettle(conn, trans, acctNo, archivedAcct, unsettleAcct);
        }

        //Refinance
        //public void CalculateRefinance(string addToAccount,
        //    StringCollection accounts,
        //    string countryCode,
        //    short branchNo,
        //    string termsType,
        //    string scoringBand,
        //    decimal deposit,
        //    decimal deferredTerms,         //CR976 jec
        //    decimal monthly,         //CR976 jec
        //    decimal final,         //CR976 jec
        //    short months,
        //    short paymentHolidays,
        //    bool depositChecked,
        //    out decimal sumBalances,
        //    out decimal newCashPrice,
        //    out decimal newAgreementTotal,
        //    out decimal newMonthlyInstal,
        //    out decimal newFinalInstal,
        //    out int newNoInstalments)
        //{
        //    //decimal deferredTerms = 0;
        //    //decimal monthly = 0;
        //    //decimal final = 0;
        //    decimal insuranceCharge = 0;
        //    decimal adminCharge = 0;
        //    decimal dtTax = 0;
        //    decimal insTax = 0;
        //    decimal adminTax = 0;

        //    /* call Account.Populate on each account */
        //    DAccount[] accts = new DAccount[accounts.Count];
        //    DAccount addTo = new DAccount(addToAccount);
        //    BAgreement addToAgreement = new BAgreement();
        //    DInstalPlan addToIP = new DInstalPlan();
        //    addToIP.Populate(null, null, addToAccount, 1);
        //    DAgreement agree = new DAgreement(null, null, addToAccount, 1);

        //    // CR440 Terms passed from Add-To screen
        //    addTo.TermsType = termsType;
        //    addToIP.NumberOfInstalments = months;

        //    // jec   removed not required for CR976
        //    sumBalances = 0;
        //    for (int i = 0; i < accounts.Count; i++)
        //    {
        //        accts[i] = new DAccount(accounts[i]);

        //        /* calculate the rebate on each account */
        //        /* subtract the rebate from the outstanding balance */
        //        decimal rebate = accts[i].CalculateRebate(null, null, accounts[i]);
        //        CountryRound(ref rebate);
        //        accts[i].OutstandingBalance -= rebate;
        //        sumBalances += accts[i].OutstandingBalance;
        //    }

        //    /* sum the new outstanding balances and add this to the 
        //     * cash price of the addToAccount */
        //    //agree.CashPrice += sumBalances;      // CR976
        //    //agree.CashPrice = sumBalances;

        //    /* we need to obtain the cash price excluding non interest items to base the DT on */
        //    decimal chargeableAdminPrice = 0;
        //    decimal chargeablePrice = GetChargeableCashPrice(null, null, addToAccount, ref chargeableAdminPrice);
        //    chargeablePrice = sumBalances;         //+= removed
        //    chargeableAdminPrice = sumBalances;    //+= removed

        //    // Check the deposit is in the valid range
        //    //deposit = this.SetDeposit(termsType, depositChecked, deposit, chargeablePrice);  CR976 jec 30/03/09
        //    agree.Deposit = deposit;
        //    agree.PaymentHolidays = paymentHolidays;
        //    addToAgreement.Deposit = agree.Deposit;
        //    addToAgreement.PaymentHolidays = agree.PaymentHolidays;

        //    DataSet variableRatesSet = new DataSet();
        //    /* calculate the service charge of the addToAccount given the
        //     * new agerement cash price */
        //    // not required for CR976
        //    //deferredTerms = addToAgreement.CalculateServiceCharge(null, null,
        //    //    countryCode,
        //    //    addTo.TermsType,
        //    //    addTo.AccountNumber,
        //    //    scoringBand,
        //    //    agree.Deposit,
        //    //    addToIP.NumberOfInstalments,
        //    //    chargeablePrice,
        //    //    addTo.DateAccountOpen,
        //    //    addTo.AccountType,
        //    //    chargeableAdminPrice,
        //    //    ref insuranceCharge,
        //    //    ref adminCharge,
        //    //    ref variableRatesSet);

        //    /* need to add tax onto deferred terms and admin charge and insurance charge
        //     * and then pass the total of all of that into CreateInstalPlan */

        //    BItem item = new BItem();
        //    XmlNode itemNode = item.GetItemDetails("DT", branchNo, AT.HP, countryCode, false, false);
        //    itemNode.Attributes[Tags.Quantity].Value = "1";
        //    itemNode.Attributes[Tags.UnitPrice].Value = StripCurrency(deferredTerms.ToString(DecimalPlaces));
        //    itemNode.Attributes[Tags.TaxAmount].Value = "0";
        //    dtTax = item.CalculateTaxAmount(itemNode, IsTaxExempt(null, null, addToAccount, DBNull.Value.ToString()));

        //    if (insuranceCharge > 0)
        //    {
        //        itemNode = item.GetItemDetails((string)Country[CountryParameterNames.InsuranceChargeItem], branchNo, AT.HP, countryCode, false, false);
        //        itemNode.Attributes[Tags.Quantity].Value = "1";
        //        itemNode.Attributes[Tags.UnitPrice].Value = StripCurrency(insuranceCharge.ToString(DecimalPlaces));
        //        itemNode.Attributes[Tags.TaxAmount].Value = "0";
        //        insTax = item.CalculateTaxAmount(itemNode, IsTaxExempt(null, null, addToAccount, DBNull.Value.ToString()));
        //    }

        //    if (adminCharge > 0)
        //    {
        //        itemNode = item.GetItemDetails((string)Country[CountryParameterNames.AdminChargeItem], branchNo, AT.HP, countryCode, false, false);
        //        itemNode.Attributes[Tags.Quantity].Value = "1";
        //        itemNode.Attributes[Tags.UnitPrice].Value = StripCurrency(adminCharge.ToString(DecimalPlaces));
        //        itemNode.Attributes[Tags.TaxAmount].Value = "0";
        //        adminTax = item.CalculateTaxAmount(itemNode, IsTaxExempt(null, null, addToAccount, DBNull.Value.ToString()));
        //    }

        //    if ((string)Country[CountryParameterNames.AgreementTaxType] == "I")
        //        dtTax = adminTax = insTax = 0;

        //    //addToAgreement.CalculateInstalPlan(agree.CashPrice,
        //    //    agree.Deposit,
        //    //    deferredTerms + dtTax + insuranceCharge + insTax + adminCharge + adminTax,
        //    //    addToIP.NumberOfInstalments,
        //    //    out monthly,
        //    //    out final);

        //    newCashPrice = agree.CashPrice;
        //    newAgreementTotal = deferredTerms + dtTax + insuranceCharge + insTax + adminCharge + adminTax + agree.CashPrice;
        //    newNoInstalments = addToIP.NumberOfInstalments;
        //    newMonthlyInstal = monthly;
        //    newFinalInstal = final;

        //    CountryRound(ref newCashPrice);
        //    CountryRound(ref newAgreementTotal);
        //}

        // Process the Refinance 
        public void ProcessRefinance(SqlConnection conn, SqlTransaction trans,
            string addToAccountNo,
            StringCollection accounts,
            string countryCode,
            string termsType,
            string scoringBand,
            decimal deposit,
            decimal deferredTerms,         //CR976 jec
            decimal monthly,         //CR976 jec
            decimal final,         //CR976 jec
            short months,
            short paymentHolidays,
            bool depositChecked)
        {
            decimal sumBalances = 0;
            decimal rebate = 0;
            DateTime dateTrans = DateTime.Now;
            //decimal deferredTerms = 0;
            //decimal monthly = 0;
            //decimal final = 0;
            decimal insuranceCharge = 0;
            decimal adminCharge = 0;
            decimal dtTax = 0;
            decimal insTax = 0;
            decimal adminTax = 0;
            short branchNo = 0;
            DateTime origDelDate = DateTime.Now;

            //
            // Added FROM Account(s)
            //

            /* call Account.Populate on each account */
            DAccount[] accts = new DAccount[accounts.Count];
            BAgreement[] agreements = new BAgreement[accounts.Count];
            DAccount addToAcct = new DAccount(conn, trans, addToAccountNo);
            DInstalPlan dueDay = new DInstalPlan();     //jec 26/01/07  get dueday
            BAgreement addToAgreement = new BAgreement();
            DInstalPlan addToIP = new DInstalPlan();
            addToIP.User = User;
            BItem item = new BItem();

            addToIP.Populate(conn, trans, addToAccountNo, 1);
            addToIP.Band = scoringBand;                             //IP - 23/09/10 - UAT(1017) UAT5.2
            addToAgreement.Populate(conn, trans, addToAccountNo, 1);


            addToAcct.TermsType = termsType;
            //addToIP.NumberOfInstalments = months;
            dueDay.GetDueDay(conn, trans, this.GetLinkedCustomerID(addToAccountNo, conn, trans));     //jec 26/01/07 get dueday
            this.DueDay = dueDay.DueDay;                                    //jec 26/01/07 get dueday

            // 5.1 uat153 rdb 07/04/08 set datenextdue to datenextdue of the account we are adding to
            DateTime dueNextDate = DateTime.MinValue;

            for (int i = 0; i < accounts.Count; i++)
            {
                /* calculate the rebate on each account */
                accts[i] = new DAccount(conn, trans, accounts[i]);
                agreements[i] = new BAgreement();

                rebate = 0;
                rebate = accts[i].CalculateRebate(conn, trans, accounts[i]);
                CountryRound(ref rebate);

                if (rebate > 0)
                {
                    branchNo = Convert.ToInt16(accts[i].AccountNumber.Substring(0, 3));
                    /* post the rebate to the fintrans table */
                    BTransaction transaction = new BTransaction(conn, trans, accounts[i],
                        branchNo, 0 /*refno*/,
                        -rebate, User, TransType.Rebate,
                        "" /*bankCode*/, "" /*bankAcctNo*/,
                        "" /*chequeNo*/, 0 /*paymethod*/,
                        countryCode, dateTrans, "" /*footnote*/, 0);

                    /* subtract the rebate from the outstanding balance */
                    accts[i].OutstandingBalance -= rebate;
                }
                sumBalances += accts[i].OutstandingBalance;

                /* update the agreement total by value of REFINCR 
                 * what about the instal plan? */
                accts[i].AgreementTotal -= accts[i].OutstandingBalance;
                if (accts[i].AgreementTotal < 0) accts[i].AgreementTotal = 0;
                agreements[i].Populate(conn, trans, accounts[i], 1);
                agreements[i].AgreementTotal = accts[i].AgreementTotal;
                origDelDate = agreements[i].DateDel;
                agreements[i].Save(conn, trans);
                accts[i].Save(conn, trans);

                // 5.1 uat153 rdb 07/04/08 set datenextdue to datenextdue of the account we are adding to
                dueNextDate = agreements[i].DateNextDue;

                /* add the REFINCR non-stock item to the account with the value of
                 * the Outstanding balance */
                InsertRefinanceItem(conn, trans, addToAccountNo, accounts[i],
                    -accts[i].OutstandingBalance, dateTrans, countryCode);

            }

            //
            // Added TO Account
            //
            branchNo = Convert.ToInt16(addToAccountNo.Substring(0, 3));

            /* sum the new outstanding balances and add this to the 
             * cash price of the addToAccount */
            //addToAgreement.CashPrice = sumBalances;       //+= removed

            /* we need to obtain the cash price excluding non interest items to base the DT on */
            decimal chargeableAdminPrice = 0;
            decimal chargeablePrice = GetChargeableCashPrice(conn, trans, addToAccountNo, ref chargeableAdminPrice);
            chargeablePrice = sumBalances;                 //+= removed
            chargeableAdminPrice = sumBalances;            //+= removed

            // Check the deposit is in the valid range
            //deposit = this.SetDeposit(termsType, depositChecked, deposit, chargeablePrice); //CR976 jec 30/03/09
            addToAgreement.Deposit = deposit;
            addToAgreement.PaymentHolidays = paymentHolidays;
            addToAgreement.DateDel = DateTime.Now;


            DataSet variableRatesSet = new DataSet();
            /* calculate the service charge of the addToAccount given the
             * new agreement cash price */
            // Not required for CR976 ??? jec
            //deferredTerms = addToAgreement.CalculateServiceCharge(conn, trans,
            //    countryCode,
            //    addToAcct.TermsType,
            //    addToAcct.AccountNumber,
            //    scoringBand,
            //    addToAgreement.Deposit,
            //    addToIP.NumberOfInstalments,
            //    /*addToAgreement.CashPrice,  exclude non-interest items */
            //    chargeablePrice,
            //    addToAcct.DateAccountOpen,
            //    addToAcct.AccountType,
            //    chargeableAdminPrice,
            //    ref insuranceCharge,
            //    ref adminCharge,
            //    ref variableRatesSet);

            // If we are adding to a new RF account we must insert the non-stock lines
            this.InsertRefinanceNonStock(conn, trans, addToAccountNo, branchNo, deferredTerms, insuranceCharge, adminCharge, deposit);

            /* need to add tax onto deferred terms and admin charge and insurance charge
             * and then pass the total of all of that into CreateInstalPlan */

            XmlNode itemNode = item.GetItemDetails(StockItemCache.Get(StockItemKeys.DT), branchNo, AT.HP, countryCode, false, false);    // CR1212 jec need to supply itemID not zero  //IP/NM - 18/05/11 -CR1212 - #3627 - Use cached item 
            itemNode.Attributes[Tags.Quantity].Value = "1";
            itemNode.Attributes[Tags.UnitPrice].Value = StripCurrency(deferredTerms.ToString(DecimalPlaces));
            itemNode.Attributes[Tags.TaxAmount].Value = "0";
            dtTax = item.CalculateTaxAmount(itemNode, IsTaxExempt(conn, trans, addToAccountNo, ""));

            if (insuranceCharge > 0)
            {
                itemNode = item.GetItemDetails(StockItemCache.Get(StockItemKeys.InsuranceChargeItem), branchNo, AT.HP, countryCode, false, false);   // CR1212 jec need to supply itemID not zero  //IP/NM - 18/05/11 -CR1212 - #3627 - Use cached item 
                itemNode.Attributes[Tags.Quantity].Value = "1";
                itemNode.Attributes[Tags.UnitPrice].Value = StripCurrency(insuranceCharge.ToString(DecimalPlaces));
                itemNode.Attributes[Tags.TaxAmount].Value = "0";
                insTax = item.CalculateTaxAmount(itemNode, IsTaxExempt(conn, trans, addToAccountNo, ""));
            }

            if (adminCharge > 0)
            {
                itemNode = item.GetItemDetails(StockItemCache.Get(StockItemKeys.AdminChargeItem), branchNo, AT.HP, countryCode, false, false);   // CR1212 jec need to supply itemID not zero  //IP/NM - 18/05/11 -CR1212 - #3627 - Use cached item 
                itemNode.Attributes[Tags.Quantity].Value = "1";
                itemNode.Attributes[Tags.UnitPrice].Value = StripCurrency(adminCharge.ToString(DecimalPlaces));
                itemNode.Attributes[Tags.TaxAmount].Value = "0";
                adminTax = item.CalculateTaxAmount(itemNode, IsTaxExempt(conn, trans, addToAccountNo, DBNull.Value.ToString()));
            }

            if ((string)Country[CountryParameterNames.AgreementTaxType] == "I")
                dtTax = adminTax = insTax = 0;
            // CR976 not required   jec
            //addToAgreement.CalculateInstalPlan(addToAgreement.CashPrice,
            //    addToAgreement.Deposit,
            //    deferredTerms + dtTax + insuranceCharge + insTax + adminCharge + adminTax,
            //    addToIP.NumberOfInstalments,
            //    out monthly,
            //    out final);

            /* update the relevant tables with the new agreement information */
            addToAgreement.AgreementTotal = deferredTerms + dtTax + insuranceCharge + insTax + adminCharge + adminTax + addToAgreement.CashPrice;
            addToAcct.AgreementTotal = addToAgreement.AgreementTotal;
            addToIP.InstalmentAmount = monthly;
            addToIP.FinalInstalment = final;
            addToIP.InstalmentFrequency = "M";
            addToAgreement.ServiceCharge = deferredTerms;
            // calculate the balance as the add-to cash price - if the account delivers all non stocks then it will recalculate the balance automatically 
            addToAcct.OutstandingBalance += sumBalances;

            /* Insert the Refin item to the RefinanceAccount */
            InsertRefinanceItem(conn, trans, addToAccountNo,
                addToAccountNo, sumBalances, dateTrans, countryCode);

            // 5.1 uat153 rdb 07/04/08 set datenextdue to datenextdue of the account we are adding to
            //addToAgreement.DateNextDue = dueNextDate;    // removed 16/04/09
            addToAgreement.EmployeeNumChange = this.User;
            addToAgreement.DateChange = DateTime.Now;
            addToAgreement.Save(conn, trans);

            addToAcct.Save(conn, trans);
            addToIP.DueDay = this.DueDay;           //jec 26/01/07 set dueday
            addToIP.Save(conn, trans);

            /*need to update the value STAX items too if dtTax || insTax || adminTax > 0 */
            item.UpdateItemValue(conn, trans, addToAccountNo, 1, StockItemCache.Get(StockItemKeys.DT), branchNo, deferredTerms);  //IP/NM - 18/05/11 -CR1212 - #3627 - Use cached item
            item.UpdateItemValue(conn, trans, addToAccountNo, 1, StockItemCache.Get(StockItemKeys.InsuranceChargeItem), branchNo, insuranceCharge);  //IP/NM - 18/05/11 -CR1212 - #3627 - Use cached item
            item.UpdateItemValue(conn, trans, addToAccountNo, 1, StockItemCache.Get(StockItemKeys.AdminChargeItem), branchNo, adminCharge);  //IP/NM - 18/05/11 -CR1212 - #3627 - Use cached item

            if (dtTax > 0)
                item.UpdateTaxAmount(conn, trans, addToAccountNo, StockItemCache.Get(StockItemKeys.DT), branchNo, dtTax); //IP/NM - 18/05/11 -CR1212 - #3627 - Use cached item
            if (insTax > 0)
                item.UpdateTaxAmount(conn, trans, addToAccountNo, StockItemCache.Get(StockItemKeys.InsuranceChargeItem), branchNo, insTax); //IP/NM - 18/05/11 -CR1212 - #3627 - Use cached item
            if (adminTax > 0)
                item.UpdateTaxAmount(conn, trans, addToAccountNo, StockItemCache.Get(StockItemKeys.AdminChargeItem), branchNo, adminTax); //IP/NM - 18/05/11 -CR1212 - #3627 - Use cached item

            // Deliver non-stocks
            DTermsType tt = new DTermsType();
            tt.GetTermsTypeDetail(conn, trans, countryCode, addToAcct.TermsType, addToAcct.AccountNumber, "", addToAcct.DateAccountOpen);
            //if ((short)(tt.TermsTypeDetails.Rows[0][CN.DeliverNonStocks]) == 1 )
            //{
            // Always deliver for Refinance??
            // Dont deliver -  think this is causing doubling up of DT in fintrans??
            // Perform the delivery of non-stock items on this account
            DBranch branch = new DBranch();
            int transRefNo = branch.GetTransRefNo(conn, trans, branchNo);
            decimal transValue = 0;
            BDelivery del = new BDelivery();
            del.User = this.User;
            del.DeliverNonStocks(conn, trans, addToAcct.AccountNumber, addToAcct.AccountType, countryCode, branchNo, transRefNo, ref transValue, addToAgreement.AgreementNumber);
            addToAgreement.DeliveryFlag = "Y";
            addToAgreement.Save(conn, trans);

            if (Math.Abs(transValue) >= 0.01M)
            {
                BTransaction t = new BTransaction(conn, trans, addToAcct.AccountNumber, branchNo,
                    transRefNo, transValue, User,
                    TransType.Delivery, "", "", "", 0, countryCode,
                    dateTrans, FootNote.AddToDelivery, 0);
            }
            //}
        }

        private void InsertRefinanceItem(SqlConnection conn, SqlTransaction trans,
        string addToAccountNo, // for fintrans.chequeno
        string accountNo,
        decimal orderValue,
        DateTime dateTrans,
        string countryCode)
        {
            /* TO DO : if this is an ADDDR then me may have already added to
             * this account. This means that there may already be a ADDDR line
             * item. In which case we need to update the ordervalue and deliver 
             * the difference. */

            short acctBranchNo = Convert.ToInt16(accountNo.Substring(0, 3));

            int itemId = StockItemCache.Get(orderValue < 0 ? StockItemKeys.REFINCR : StockItemKeys.REFINDR);
            string itemNo = orderValue < 0 ? "REFINCR" : "REFINDR";
            double quantity = orderValue < 0 ? -1 : 1;
            DBranch branch = new DBranch();
            int refNo = branch.GetTransRefNo(conn, trans, acctBranchNo);

            DDelivery del = new DDelivery();
            del.OrigBr = 0;
            del.AccountNumber = accountNo;
            del.AgreementNumber = 1;
            del.DateDelivered = DateTime.Today;
            del.DeliveryOrCollection = DelType.Normal;
            del.ItemNumber = itemNo;
            del.ItemId = itemId;
            del.StockLocation = acctBranchNo;
            del.Quantity = quantity;
            del.BuffNo = branch.GetBuffNo(conn, trans, acctBranchNo);
            del.BuffBranchNumber = acctBranchNo;
            del.DateTrans = dateTrans;
            del.BranchNumber = acctBranchNo;
            del.TransRefNo = refNo;
            del.TransValue = Math.Round(orderValue, 2);
            del.RunNumber = 0;
            del.ftNotes = "DNAD";
            del.NotifiedBy = User;
            del.ParentItemNo = ""; //IP & JC -26/01/09 - Need to set this
            del.Write(conn, trans);

            DFACTTrans fact = new DFACTTrans();
            DLineItem lineItem = new DLineItem();
            lineItem.AccountNumber = accountNo;
            lineItem.AgreementNumber = 1;
            lineItem.ItemNumber = itemNo;
            lineItem.ItemID = itemId;
            lineItem.Quantity = 1;
            lineItem.StockLocation = acctBranchNo;
            lineItem.Price = Math.Round(orderValue, 2);
            lineItem.OrderValue = Math.Round(orderValue, 2);
            lineItem.DeliveryNoteBranch = acctBranchNo;
            lineItem.QuantityDiff = "N";
            lineItem.ItemType = "N";
            lineItem.BuffNo = fact.GetOrderNo(conn, trans, accountNo, 1);
            if (lineItem.BuffNo == 0)
            {
                branch = new DBranch();
                lineItem.BuffNo = branch.GetBuffNo(conn, trans, acctBranchNo);
            }
            lineItem.User = User;       //UAT171 jec 07/10/10
            lineItem.Save(conn, trans);

            // Reinstating - required to update account balance 
            if (itemNo == "REFINCR")
            {
                BTransaction transaction = new BTransaction(conn, trans, accountNo, acctBranchNo,
                    refNo, orderValue, User, orderValue < 0 ? TransType.Refinance : TransType.RefinanceDep,
                    "", "", addToAccountNo,
                    0, countryCode, dateTrans,
                    "", 0);
            }
        }


        private void InsertRefinanceNonStock(SqlConnection conn, SqlTransaction trans,
            string refinAccountNo, short branchNo,
            decimal deferredTerms, decimal insuranceCharge, decimal adminCharge, decimal deposit)
        {
            if (Math.Abs(Math.Round(deferredTerms, 2)) > 0.01M)
            {
                DLineItem lineItem = new DLineItem();
                lineItem.AccountNumber = refinAccountNo;
                lineItem.AgreementNumber = 1;
                lineItem.ItemNumber = "DT";
                lineItem.ItemID = StockItemCache.Get(StockItemKeys.DT);
                lineItem.StockLocation = branchNo;
                lineItem.Price = Math.Round(deferredTerms, 2);
                lineItem.OrderValue = Math.Round(deferredTerms, 2);
                lineItem.DeliveryNoteBranch = branchNo;
                lineItem.QuantityDiff = "N";
                lineItem.ItemType = "N";
                lineItem.Quantity = 0;
                // If the item already exists then do not save it again
                lineItem.GetItemQuantity(conn, trans, true);
                lineItem.User = User;       //UAT171 jec 07/10/10
                if (lineItem.Quantity == 0)
                {
                    lineItem.Quantity = 1;
                    lineItem.Save(conn, trans);
                }
            }

            if (Math.Abs(Math.Round(insuranceCharge, 2)) > 0.01M)
            {
                DLineItem lineItem = new DLineItem();
                lineItem.AccountNumber = refinAccountNo;
                lineItem.AgreementNumber = 1;
                lineItem.ItemNumber = (string)Country[CountryParameterNames.InsuranceChargeItem];
                lineItem.ItemID = StockItemCache.Get(StockItemKeys.InsuranceChargeItem);                    //IP - 04/01/12 - #8778 - LW73615 
                lineItem.StockLocation = branchNo;
                lineItem.Price = Math.Round(insuranceCharge, 2);
                lineItem.OrderValue = Math.Round(insuranceCharge, 2);
                lineItem.DeliveryNoteBranch = branchNo;
                lineItem.QuantityDiff = "N";
                lineItem.ItemType = "N";
                lineItem.Quantity = 0;
                // If the item already exists then do not save it again
                lineItem.GetItemQuantity(conn, trans, true);
                lineItem.User = User;       //UAT171 jec 07/10/10
                if (lineItem.Quantity == 0)
                {
                    lineItem.Quantity = 1;
                    lineItem.Save(conn, trans);
                }
            }

            if (Math.Abs(Math.Round(adminCharge, 2)) > 0.01M)
            {
                DLineItem lineItem = new DLineItem();
                lineItem.AccountNumber = refinAccountNo;
                lineItem.AgreementNumber = 1;
                lineItem.ItemNumber = (string)Country[CountryParameterNames.AdminChargeItem];
                lineItem.ItemID = StockItemCache.Get(StockItemKeys.AdminChargeItem);                    //IP - 04/01/12 - #8778 - LW73615
                lineItem.StockLocation = branchNo;
                lineItem.Price = Math.Round(adminCharge, 2);
                lineItem.OrderValue = Math.Round(adminCharge, 2);
                lineItem.DeliveryNoteBranch = branchNo;
                lineItem.QuantityDiff = "N";
                lineItem.ItemType = "N";
                lineItem.Quantity = 0;
                // If the item already exists then do not save it again
                lineItem.GetItemQuantity(conn, trans, true);
                lineItem.User = User;       //UAT171 jec 07/10/10
                if (lineItem.Quantity == 0)
                {
                    lineItem.Quantity = 1;
                    lineItem.Save(conn, trans);
                }
            }

            if (Math.Abs(Math.Round(deposit, 2)) > 0.01M)
            {
                DLineItem lineItem = new DLineItem();
                lineItem.AccountNumber = refinAccountNo;
                lineItem.AgreementNumber = 1;
                lineItem.ItemNumber = "REFINCR";   //refinance Credit item i.e. value of cash price paid on previous a/c
                lineItem.ItemID = StockItemCache.Get(StockItemKeys.REFINCR);                            //IP - 04/01/12 - #8778 - LW73615
                lineItem.StockLocation = branchNo;
                lineItem.Price = Math.Round(deposit, 2);       //value of cash price paid on previous a/c
                lineItem.OrderValue = Math.Round(deposit, 2);
                lineItem.DeliveryNoteBranch = branchNo;
                lineItem.QuantityDiff = "N";
                lineItem.ItemType = "N";
                lineItem.Quantity = 0;
                // If the item already exists then do not save it again
                lineItem.GetItemQuantity(conn, trans, true);
                lineItem.User = User;       //UAT171 jec 07/10/10
                if (lineItem.Quantity == 0)
                {
                    lineItem.Quantity = 1;
                    lineItem.Save(conn, trans);
                }
            }

        }

        //IP - 08/10/09 - UAT(909) - Method which checks if accounts can be re-allocated to the selected employee.
        public bool CheckCanReallocate(SqlConnection conn, SqlTransaction trans, int countAcctsToRealloc, int EmployeeNo, ref int noCanAlloc)
        {
            DFollUpAlloc Fu = new DFollUpAlloc();
            bool canReallocate = Fu.CheckCanReallocate(conn, trans, countAcctsToRealloc, EmployeeNo, ref noCanAlloc);
            return canReallocate;
        }

        //Loyalty CR1017
        public string LockCheckbyAccount(string acctno, string user)
        {
            DAccount da = new DAccount();
            return da.LockCheckbyAccount(acctno, user);
        }

        //IP - 04/02/10 - CR1072 - 3.1.9 Display Delivery Authorisation History in Account Details.
        /// <summary>
        /// Method that retrieves Delivery Authorisation History for an account.
        /// </summary>
        /// <param name="acctno"></param>
        /// <returns></returns>
        public DataTable LoadDAHistory(string acctno)
        {
            DAccount da = new DAccount();
            da.LoadDAHistory(acctno);
            DataTable acctDaHistory = da.AcctDaHistory;
            return acctDaHistory;
        }

        //IP - 08/02/10 - CR1037 Merged - Malaysia Enhancements (CR1072)
        public bool IsPaidAndTakenAccount(string accountNo) //CR 1037 
        {
            DCustomer customer = new DCustomer();
            DataTable dt = customer.GetCustomerID(accountNo.Trim());
            DAccount da = new DAccount();    //IP - 21/09/10 - UAT5.2 Log - UAT(1118)
            da.Populate(null, null, accountNo);   //IP - 21/09/10 - UAT5.2 Log - UAT(1118)
            int agreementNo = da.GetAgreementNo(null, null, accountNo);  //IP - 21/09/10 - UAT5.2 Log - UAT(1118)

            if (dt.Rows.Count > 0 && dt.Rows[0][CN.CustID].ToString().Trim() == "PAID & TAKEN"
                   && dt.Rows[0][CN.AccountNumber].ToString().Trim()[3] == '5'
                   || dt.Rows.Count > 0 && dt.Rows[0][CN.AccountNumber].ToString().Trim()[3] == '5' //IP - 21/09/10 - UAT5.2 Log - UAT(1118) - Check if special account created for Cash & Go warranties
                    && da.TermsType == "PT" && agreementNo > 1)
            // -- 4th character must be '5'
            {
                return true;
            }

            return false;
        }


        // #17290 - Conn trans version
        public bool IsPaidAndTakenAccount(SqlConnection conn, SqlTransaction trans, string accountNo)
        {
            DCustomer customer = new DCustomer();
            DataTable dt = customer.GetCustomerID(accountNo.Trim());
            DAccount da = new DAccount();
            da.Populate(conn, trans, accountNo);
            int agreementNo = da.GetAgreementNo(conn, trans, accountNo);

            if (dt.Rows.Count > 0 && dt.Rows[0][CN.CustID].ToString().Trim() == "PAID & TAKEN"
                   && dt.Rows[0][CN.AccountNumber].ToString().Trim()[3] == '5'
                   || dt.Rows.Count > 0 && dt.Rows[0][CN.AccountNumber].ToString().Trim()[3] == '5'
                    && da.TermsType == "PT" && agreementNo > 1)
            // -- 4th character must be '5'
            {
                return true;
            }

            return false;
        }

        //IP - 11/02/10 - CR1048 (Ref:3.1.2.5) Merged - Malaysia Enhancements (CR1072)
        //Method which retrieves the last payment method for a Cash & Go sale.
        public DataSet GetCashAndGoLastPayMethod(string acctNo, int agrmtNo)
        {
            DAccount da = new DAccount();
            da.GetCashAndGoLastPayMethod(acctNo, agrmtNo);
            DataSet ds = new DataSet();
            ds.Tables.Add(da.CashAndGoPayments);
            return ds;
        }

        //IP - 19/02/10 - CR1072 - LW 69770 - Printing Fixes from 4.3 - Merge
        // 19/06/08 rdb calling form OrigDeliveryNote
        // add some sort of check for redeliver after repossession
        // this method is intended as a quick fix rather than high quality code
        public DataSet GetRedeliveryAfterRepossessionDetails(SqlConnection conn, SqlTransaction trans,
            string acctno, int stockLocn, out int buffNo)
        {
            DAccount da = new DAccount();
            DataTable dt = da.GetRedeliveryAfterRepossessionDetails(conn, trans, acctno, stockLocn, out buffNo);
            DataSet ds = new DataSet("lineItems");
            ds.Tables.Add(dt);
            return ds;
        }

        public void LettersGenerateCSharpCSVfiles(int runNo, string lettercode,
           DateTime starttime, DateTime endttime)
        {

            SqlConnection conn = null;
            SqlTransaction trans = null;
            try
            {
                DAccount account = new DAccount();
                DataTable DT =
                account.LoadLetterDetails(conn, trans, runNo, lettercode, starttime, endttime);
                string strfilepath =
                 (string)Country[CountryParameterNames.SystemDrive] + "\\LTR" + lettercode +
                Convert.ToString(runNo) + ".csv";

                DTransfer Transfer = new DTransfer();
                Transfer.ExportToCSV(DT, strfilepath);
            }
            catch (Exception)
            {

            }
        }

        public decimal ProvisionGetForAccount(string acctno)
        {
            DAccount acct = new DAccount();
            var provision = acct.ProvisionGetForAccount(acctno);

            return provision.HasValue ? provision.Value : 0;
        }

        public DataTable MaxAction(string accountNo)
        {
            DAccount acct = new DAccount();
            DataTable dt = acct.MaxAction(accountNo);

            return dt;
        }

        public bool CheckSRAcct(string acctno)
        {
            DAccount acct = new DAccount();
            return acct.CheckSRAcct(acctno);

        }

        public void NewAccountCreditSave(SqlConnection conn, SqlTransaction trans,
            ref SaveNewAccountCreditParameters Parms)
        {
            DStockItem Stock = new DStockItem();
            Stock.User = Parms.User;
            Stock.UnlockItem(conn, trans);

            if (AT.IsCreditType(Parms.AccountType) && Parms.CustomerID.Length > 0 && !Parms.IsLoan
                && !Parms.SPAPrint)
            {

                decimal agrTot = (Convert.ToDecimal(Country[CountryParameterNames.IC_MaxAgrmtTotal]));
                if (Parms.AgreementTotal <= agrTot && Parms.AgreementTotal > 0 && Parms.PropResult != PR.Referred)
                {
                    Parms.Approved = InstantCredit("??", Parms.AccountNumber);  //This will return the current instant credit flag.

                    if (Parms.DateAccountOpened.AddMonths(Convert.ToInt32(Country[CountryParameterNames.IC_ReviseMonths])) < DateTime.Now || Parms.Approved != IC.Approved) //Only check if outside revised months or not approved
                    {
                        Parms.Approved = InstantCredit(Parms.CustomerID, Parms.AccountNumber);
                    }

                    if (Parms.Approved == IC.Approved || Parms.Approved == IC.Granted)
                    {
                        DProposal prop = new DProposal();
                        string newAccount = ""; int points = 0; string CheckType = Parms.CheckType;
                        DateTime dateProp = Parms.DateProp; string propResult = Parms.PropResult;

                        prop.GetUnclearedStage(conn, trans, Parms.AccountNumber, ref newAccount,
                         ref CheckType, ref dateProp, ref propResult, ref points);
                        Parms.NewAccountNumber = newAccount; Parms.CheckType = CheckType;
                        Parms.DateProp = dateProp; Parms.PropResult = propResult;
                        DHoldFlags Hflags = new DHoldFlags();
                        Hflags.CustomerID = Parms.CustomerID;
                        Hflags.EmployeeNoFlag = Parms.User;

                        if (Parms.Deposit > 0)
                        {
                            Hflags.CheckType = "DEP";
                            Hflags.SaveInstantCreditFlag(conn, trans, Parms.AccountNumber);
                        }

                        DTermsType tt = new DTermsType();
                        DataSet termsTypeSet = tt.LoadTermsTypeDetails(Parms.TermsType);

                        //IP - 04/03/11 - #3275
                        DInstalPlan ip = new DInstalPlan();
                        ip.Populate(conn, trans, Parms.AccountNumber, 1);

                        //#19219 - CR15594 - Prevent INST flag for Ready Assist
                        var readyAssist = new AccountRepository().IsReadyAssist(conn, trans, Parms.AccountNumber, 1);

                        if (termsTypeSet.Tables[TN.TermsType].Rows[0][CN.InstalPreDel].ToString() == "Y" && ip.InstalmentWaived == false && readyAssist == false) //IP - 04/03/11 - #3275 - and not qualified for first instalment waived
                        {
                            Hflags.CheckType = "INST";
                            Hflags.SaveInstantCreditFlag(conn, trans, Parms.AccountNumber);
                        }

                        if (Parms.CheckType == SS.S2) //clear stage 2 automatically
                        {
                            DProposalFlag propFlag = new DProposalFlag();
                            propFlag.OrigBr = 0;
                            propFlag.CustomerID = Parms.CustomerID;
                            propFlag.DateProp = Parms.DateProp;
                            propFlag.DateCleared = DateTime.Now;

                            propFlag.EmployeeNoFlag = this.User;
                            propFlag.CheckType = Parms.CheckType;
                            propFlag.Save(conn, trans, Parms.AccountNumber);

                        }

                        //DInstalPlan ip = new DInstalPlan();
                        //ip.AccountNumber = Parms.AccountNumber;
                        //ip.User = Parms.User;
                        //ip.AutoDA(conn, trans);

                    }

                }

            }

        }

        //Create Cash Loan Account - Insert Lineitem, update Acct, Agreement, Instalplan
        public void InsertCashLoanItem(SqlConnection conn, SqlTransaction trans, CashLoanDetails det, string auditSource, string countryCode, short branchNo)
        {
            double adminTaxRate = 0;
            double insuranceTaxRate = 0;
            short acctBranchNo = Convert.ToInt16(det.accountNo.Substring(0, 3));

            //Insert the LOAN item
            int itemId = StockItemCache.Get(det.loanAmount < 0 ? StockItemKeys.LOAN : StockItemKeys.LOAN);
            string itemNo = "LOAN";

            DLineItem lineItem = new DLineItem();
            //BItem bItem = new BItem();

            //Delete all the line items first
            lineItem.DeleteAllLineItems(conn, trans, det.accountNo, 1);

            lineItem.AccountNumber = det.accountNo;
            lineItem.AgreementNumber = 1;
            lineItem.ItemNumber = itemNo;
            lineItem.ItemID = itemId;
            lineItem.Quantity = 1;
            lineItem.StockLocation = acctBranchNo;
            lineItem.Price = Math.Round(det.loanAmount, 2);
            lineItem.OrderValue = Math.Round(det.loanAmount, 2);
            lineItem.DeliveryNoteBranch = acctBranchNo;
            lineItem.QuantityDiff = "N";
            lineItem.ItemType = "N";
            lineItem.AuditSource = auditSource;

            lineItem.User = User;
            lineItem.Save(conn, trans);

            //Insert the Service Charge item
            itemNo = "DT";
            itemId = StockItemCache.Get(StockItemKeys.DT);

            lineItem.AccountNumber = det.accountNo;
            lineItem.AgreementNumber = 1;
            lineItem.ItemNumber = itemNo;
            lineItem.ItemID = itemId;
            lineItem.Quantity = 1;
            lineItem.StockLocation = acctBranchNo;
            lineItem.Price = Math.Round(det.serviceChg, 2);
            lineItem.OrderValue = Math.Round(det.serviceChg, 2);
            lineItem.DeliveryNoteBranch = acctBranchNo;
            lineItem.QuantityDiff = "N";
            lineItem.ItemType = "N";
            lineItem.AuditSource = auditSource;

            lineItem.User = User;
            lineItem.Save(conn, trans);

            //Insert the Admin Charge item
            if (det.adminChg > 0)
            {
                var adminCharge = det.adminChg;

                var adminItemId = StockItemCache.Get(StockItemKeys.AdminChargeItem);

                DStockItem itemDetails = new DStockItem();
                itemDetails.GetItemDetails(null, null, adminItemId, branchNo, "R", countryCode, false, false);
                adminTaxRate = itemDetails.TaxRate;

                //If agreement tax inclusive we need to add tax onto Admin and not consider the Admin as already including tax  //kedar
                if (((string)Country[CountryParameterNames.TaxType] == "I" || (string)Country[CountryParameterNames.TaxType] == "E"))
                {
                    if ((string)Country[CountryParameterNames.AgreementTaxType] == "I" && (bool)Country[CountryParameterNames.CL_Amortized])
                    {
                        if ((bool)(Country[CountryParameterNames.CL_TaxRateApplied]))
                        {
                            adminCharge = adminCharge + 0; //(adminCharge * (Convert.ToDecimal(adminTaxRate) / 100));
                        }
                        else
                        {
                            adminCharge = adminCharge + 0;
                        }
                    }
                    else if ((string)Country[CountryParameterNames.AgreementTaxType] == "I")
                    {
                        adminCharge = adminCharge + (adminCharge * (Convert.ToDecimal(adminTaxRate) / 100));
                    }

                }

                itemNo = Convert.ToString(Country[CountryParameterNames.AdminChargeItem]);
                itemId = StockItemCache.Get(StockItemKeys.AdminChargeItem);

                lineItem.AccountNumber = det.accountNo;
                lineItem.AgreementNumber = 1;
                lineItem.ItemNumber = itemNo;
                lineItem.ItemID = itemId;
                lineItem.Quantity = 1;
                lineItem.StockLocation = acctBranchNo;
                lineItem.Price = Math.Round(adminCharge, 2);
                lineItem.OrderValue = Math.Round(adminCharge, 2);
                lineItem.DeliveryNoteBranch = acctBranchNo;
                lineItem.QuantityDiff = "N";
                lineItem.ItemType = "N";
                if (!(bool)(Country[CountryParameterNames.CL_TaxRateApplied]))
                {
                    det.adminTax = 0;
                    adminTaxRate = 0;
                }
                lineItem.TaxAmount = Math.Round(Convert.ToDouble(det.adminTax), 2);
                lineItem.Taxrate = adminTaxRate;

                lineItem.AuditSource = auditSource;

                lineItem.User = User;
                lineItem.Save(conn, trans);
                ClAmt = det.loanAmount;
            }

            //Insert the Insurance Charge item
            if (det.insuranceChg > 0)
            {
                var insuranceCharge = det.insuranceChg;

                var insuranceItemId = StockItemCache.Get(StockItemKeys.InsuranceChargeItem);

                DStockItem itemDetails = new DStockItem();
                itemDetails.GetItemDetails(null, null, insuranceItemId, branchNo, "R", countryCode, false, false);
                insuranceTaxRate = itemDetails.TaxRate;

                //If agreement tax inclusive we need to add tax onto Insurance and not consider the Insurance as already including tax
                if ((string)Country[CountryParameterNames.TaxType] == "I" || (string)Country[CountryParameterNames.TaxType] == "E")
                {
                    if ((string)Country[CountryParameterNames.AgreementTaxType] == "I")
                    {
                        insuranceCharge = insuranceCharge + (insuranceCharge * Convert.ToDecimal(insuranceTaxRate)) / 100;
                    }
                }

                itemNo = Convert.ToString(Country[CountryParameterNames.InsuranceChargeItem]);
                itemId = StockItemCache.Get(StockItemKeys.InsuranceChargeItem);

                lineItem.AccountNumber = det.accountNo;
                lineItem.AgreementNumber = 1;
                lineItem.ItemNumber = itemNo;
                lineItem.ItemID = itemId;
                lineItem.Quantity = 1;
                lineItem.StockLocation = acctBranchNo;
                lineItem.Price = Math.Round(insuranceCharge, 2);
                lineItem.OrderValue = Math.Round(insuranceCharge, 2);
                lineItem.DeliveryNoteBranch = acctBranchNo;
                lineItem.QuantityDiff = "N";
                lineItem.ItemType = "N";
                lineItem.TaxAmount = Math.Round(Convert.ToDouble(det.insuranceTax), 2);
                lineItem.Taxrate = insuranceTaxRate;
                lineItem.AuditSource = auditSource;

                lineItem.User = User;
                lineItem.Save(conn, trans);
            }

            //Insert the Stamp Duty item            #10013
            if (det.stampDuty > 0)
            {

                itemNo = "SD";
                itemId = StockItemCache.Get(StockItemKeys.SD);

                lineItem.AccountNumber = det.accountNo;
                lineItem.AgreementNumber = 1;
                lineItem.ItemNumber = itemNo;
                lineItem.ItemID = itemId;
                lineItem.Quantity = 1;
                lineItem.StockLocation = acctBranchNo;
                lineItem.Price = Math.Round(det.stampDuty, 2);
                lineItem.OrderValue = Math.Round(det.stampDuty, 2);
                lineItem.DeliveryNoteBranch = acctBranchNo;
                lineItem.QuantityDiff = "N";
                lineItem.ItemType = "N";
                lineItem.Taxrate = Convert.ToDouble(0);         // #10013
                lineItem.TaxAmount = Convert.ToDouble(Math.Round(det.stampDuty, 2)) * lineItem.Taxrate;         // #10013
                lineItem.AuditSource = auditSource;

                lineItem.User = User;
                lineItem.Save(conn, trans);

            }

            if (!(bool)Country[CountryParameterNames.CL_Amortized])
            {
                //Insert the Sales Tax item
                if ((string)Country[CountryParameterNames.AgreementTaxType] == "E")
                {
                    itemNo = "STAX";
                    itemId = StockItemCache.Get(StockItemKeys.STAX);

                    lineItem.AccountNumber = det.accountNo;
                    lineItem.AgreementNumber = 1;
                    lineItem.ItemNumber = itemNo;
                    lineItem.ItemID = itemId;
                    lineItem.Quantity = 1;
                    lineItem.StockLocation = acctBranchNo;
                    lineItem.Price = Math.Round(det.insuranceTax + det.adminTax, 2);
                    lineItem.OrderValue = lineItem.Price;
                    lineItem.DeliveryNoteBranch = acctBranchNo;
                    lineItem.QuantityDiff = "N";
                    lineItem.ItemType = "N";
                    lineItem.TaxAmount = 0;
                    lineItem.AuditSource = auditSource;

                    lineItem.User = User;
                    lineItem.Save(conn, trans);

                }
            }
            DAccount da = new DAccount();
            da.User = User;

            DAgreement ag = new DAgreement();
            ag.User = User;

            DInstalPlan ip = new DInstalPlan();
            ip.User = User;

            DCustomer dc = new DCustomer();

            da.Populate(conn, trans, det.accountNo);
            ag.Populate(conn, trans, det.accountNo, 1);
            ip.Populate(conn, trans, det.accountNo, 1);

            da.AgreementTotal = det.agreementTotal;
            da.TermsType = det.termsType;

            ag.AgreementTotal = det.agreementTotal;
            ag.ServiceCharge = det.serviceChg;
            ag.CashPrice = det.loanAmount + det.stampDuty;      // #10013
            ag.AdminFee = det.adminChg;
            ag.InsCharge = det.insuranceChg;
            ag.SOA = "CL";                          //CL = Cash Loan Letter
            ag.HoldProp = "Y";
            ag.DateChange = DateTime.Now;           //#17890
                                                    //ag.DateDel = DateTime.Today;          //IP - 15/11/11 - #8642  - datedel will be set once CashLoan has been disbursed.

            if ((bool)Country[CountryParameterNames.CL_Amortized])   //kedar
            {
                if ((bool)(Country[CountryParameterNames.CL_TaxRateApplied]))
                {

                    //ip.InstalmentAmount = det.instalment + det.adminChg * (det.term / 12) + (det.insuranceChg / det.term) + (det.adminTax / det.term);
                    //ip.FinalInstalment = det.finInstal + det.adminChg * (det.term / 12) + (det.insuranceChg / det.term) + (det.adminTax / det.term);
                    //det.adminChg = det.adminChg + (det.adminChg * (det.adminTax / 100));
                    // det.adminChg = det.adminChg * (det.term / 12);
                    //det.adminChg = det.adminChg + det.adminTax;
                    ip.InstalmentAmount = det.instalment + (det.adminChg / det.term); //(det.adminTax/det.term);//(det.insuranceChg / det.term) + (det.adminChg * (det.adminTax / 100));
                    ip.FinalInstalment = det.finInstal + (det.adminChg / det.term);// (det.adminTax/det.term);// (det.insuranceChg / det.term) + (det.adminChg * (det.adminTax / 100));

                }
                else
                {
                    // det.adminChg = det.adminChg * (det.term / 12);
                    ip.InstalmentAmount = det.instalment + det.adminChg / det.term;// * (det.term / 12))+ (det.insuranceChg / det.term);
                    ip.FinalInstalment = det.finInstal + det.adminChg / det.term;// * (det.term / 12))+ (det.insuranceChg / det.term);

                }
                ip.AInstalmentAmount = det.instalment;
                ip.ClAmount = det.loanAmount;
            }
            else
            {
                ip.InstalmentAmount = det.instalment;
                ip.FinalInstalment = det.finInstal;
            }


            ip.NumberOfInstalments = det.term;
            // if ((bool)Country[CountryParameterNames.CL_Amortized]) //kedar
            //  if ((bool)(Country[CountryParameterNames.CL_TaxRateApplied]))
            //  {
            //  det.adminChg = det.adminChg + 0;// (det.adminChg * (det.adminTax / 100));
            //  ip.InstalTotal = det.agreementTotal + det.serviceChg + det.adminChg ; //+ det.adminTax;
            // }
            //   else
            //  {
            //    ip.InstalTotal = det.agreementTotal + det.serviceChg  + det.adminChg;
            // }
            // else
            ip.InstalTotal = det.agreementTotal;
            ip.AdmnChg = det.adminChg;
            da.Save(conn, trans);
            ag.Save(conn, trans);
            ip.Save(conn, trans);


            ip.SaveAmortizedSchedule(conn, trans);

            //Recalculate the customers availablespend
            //dc.GetRFLimit(conn, trans, det.custId, String.Empty);
            ClAmt = det.loanAmount;
            // #8670 LoanAmount zero indicates loan is referred - set value on CashLoan table to country maximum
            if (det.loanAmount == 0)
            {
                det.loanAmount = Convert.ToInt32(Country[CountryParameterNames.CL_MaxLoanAmount]);
                det.loanStatus = CashLoanStatus.Referred;
            }
            //Insert a record into the CashLoan table
            CreditRepository c = new CreditRepository();
            c.CashLoanStausInsert(det, conn, trans);
        }

        // This method is used to validate -
        // 1) if only non stock item is sold for this account.
        // 2) to check revise case - where do not allow revise if only non stock item is delivered for this account.
        public DataSet IsAccountValidForOnlyNonStockSale(string accountNumber)
        {
            DataSet ds = new DataSet();
            DAccount details = new DAccount();
            details.IsAccountValidForOnlyNonStockSale(accountNumber);
            ds.Tables.Add(details.AccountDetails);
            return ds;
        }

        // This method is used to deliver only non stock items for account.
        public void DeliverNonStock(SqlConnection conn, SqlTransaction trans, string accountNo)
        {
            DAgreement details = new DAgreement();
            details.DeliverNonStocks(conn, trans, accountNo);
        }

        // This method is used to track if respective account is used for only non stock sale.
        public void SaveAccountForSaleOnlyNonStock(SqlConnection conn, SqlTransaction trans, string acctno)
        {
            DAccount da = new DAccount();
            da.SaveAccountForSaleOnlyNonStock(conn, trans, acctno);
        }
        public void DisableSave(SqlConnection conn, SqlTransaction trans, string acctno, out string invoice, out int status)
        {
            DAccount da = new DAccount();
            
            da.DisableSave(conn, trans, acctno,out invoice,out status);
            
        }
        public void DisableRFButton(SqlConnection conn, SqlTransaction trans, string custid, out int status)
        {
            DAccount da = new DAccount();

            da.DisableRFButton(conn, trans, custid, out status);

        }
		
        // This method is used to validate if selected item is valid for sale as non stock item.
        public DataSet ValidateNonSaleableNonStocks(string productSKU)
        {
            Function = "BAccount::ValidateNonSaleableNonStocks()";

            int result = 0;
            DataSet ds = new DataSet();
            data = new DAccount();
            result = data.ValidateNonSaleableNonStocks(productSKU);

            ds.Tables.Add(data.AccountDetails);
            return ds;
        }
        //CR - Procedure to close service request when item is removed from sales order screen
        public void CloseServiceRequest(SqlConnection conn, SqlTransaction trans, string acctno, int ParentItemid, int ItemId)
        {
            DAccount da = new DAccount();
            da.CloseServiceRequest(conn, trans, acctno, ParentItemid, ItemId);
        }

		public string GetInvoiveNumberWithVersion(string accountNumber)
        {
            return new DAccount().GetInvoiveNumberWithVersion(accountNumber);
        }

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
            Function = "BAccount::FetchMambuServiceChargeInstalDetails()";

            DAccount dAcct = new DAccount();
            dAcct.FetchMambuServiceChargeInstalDetails(conn, trans, acctNo, out serviceCharge, out instalAmt, out finalinstalAmt, out adminTax, out stampDuty, out agreementTotal);
            CountryRound(ref serviceCharge);
            CountryRound(ref instalAmt);
            CountryRound(ref finalinstalAmt);
            CountryRound(ref adminTax);
            CountryRound(ref stampDuty);
            CountryRound(ref agreementTotal);


        }

        //GDIEC-661 Modify COSACS to support Bluestar accounts
        public DataSet GetPrintingInfoForCL(string acctNo)
        {
            DataSet countryMaintenanceData = new DataSet();
            DAccount acct = new DAccount();
            countryMaintenanceData = acct.GetPrintingInfoForCL(acctNo);
            return countryMaintenanceData;
        }

    }
}