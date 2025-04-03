using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Collections;
using STL.Common;
using STL.Common.Constants.Enums;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;


namespace STL.DAL
{
    /// <summary>
    /// Summary description for DCustomer.
    /// </summary>
    public class DRenissanceSale : DALObject
    {
        private string _custID = "";
        private short _origbr = 0;
        private string _otherid = "";
        private short _branchnohdle = 0;
        private string _name = "";
        private string _firstname = "";
        private string _title = "";
        private string _DELLastName = "";
        private string _addtype = "";

        private string _DELFirstName = "";
        private string _DELTitleC = "";
        private string _alias = "";
        private int _budget = 0;
        private string _addrsort = "";
        private string _namesort = "";
        private string _datebornstr = "";
        private string _sex = "";
        private string _ethnicity = "";
        private string _morerewardsno = "";
        private byte _rfcardseqno = 0;
        private DateTime _effectivedate;
        private string _idtype = "";
        private string _idnumber = "";
        private string _maidenname = "";
        private string _maritalStatus = "";
        private int _dependants = 0;
        private string _nationality = "";
        private DateTime _dateborn;
        private DataTable _custCodes;
        private DataTable _custSearch;
        private DataTable _addresses = null;
        private DataTable _cust = null;

        public int Dependants
        {
            get { return _dependants; }
            set { _dependants = value; }
        }
        public string MaritalStatus
        {
            get { return _maritalStatus; }
            set { _maritalStatus = value; }
        }
        public string Nationality
        {
            get { return _nationality; }
            set { _nationality = value; }
        }
        public DataTable Addresses
        {
            get { return _addresses; }
        }

        public string MaidenName
        {
            get { return _maidenname; }
            set { _maidenname = value; }
        }

        private DateTime _datein;
        public DateTime DateIn
        {
            get { return _datein; }
            set { _datein = value; }
        }
        private DateTime _dateout;
        public DateTime DateOut
        {
            get { return _dateout; }
            set { _dateout = value; }
        }
        private string _resstatus = "";
        public string ResidentialStatus
        {
            get { return _resstatus; }
            set { _resstatus = value; }
        }
        private string _proptype = "";
        public string PropertyType
        {
            get { return _proptype; }
            set { _proptype = value; }
        }
        // DSR 24 Jan 2003 - Load address lines as well
        private string _address1 = "";
        public string Address1
        {
            get { return _address1; }
            set { _address1 = value; }
        }
        private string _address2 = "";
        public string Address2
        {
            get { return _address2; }
            set { _address2 = value; }
        }
        private string _address3 = "";
        public string Address3
        {
            get { return _address3; }
            set { _address3 = value; }
        }
        private string _postcode = "";
        public string PostCode
        {
            get { return _postcode; }
            set { _postcode = value; }
        }
        private string _deliveryArea = "";
        public string DeliveryArea
        {
            get { return _deliveryArea; }
            set { _deliveryArea = value; }
        }
        private DateTime _pdatein;
        public DateTime PrevDateIn
        {
            get { return _pdatein; }
            set { _pdatein = value; }
        }
        private DateTime _pdateout;
        public DateTime PrevDateOut
        {
            get { return _pdateout; }
            set { _pdateout = value; }
        }
        private string _presstatus = "";
        public string PrevResidentialStatus
        {
            get { return _presstatus; }
            set { _presstatus = value; }
        }
        private string _pproptype = "";
        public string PrevPropertyType
        {
            get { return _pproptype; }
            set { _pproptype = value; }
        }
        // DSR 24 Jan 2003 - Load previous address lines as well
        private string _paddress1 = "";
        public string PAddress1
        {
            get { return _paddress1; }
            set { _paddress1 = value; }
        }
        private string _paddress2 = "";
        public string PAddress2
        {
            get { return _paddress2; }
            set { _paddress2 = value; }
        }
        private string _paddress3 = "";
        public string PAddress3
        {
            get { return _paddress3; }
            set { _paddress3 = value; }
        }
        private string _ppostcode = "";
        public string PPostCode
        {
            get { return _ppostcode; }
            set { _ppostcode = value; }
        }
        private string _pDeliveryArea = "";
        public string PDeliveryArea
        {
            get { return _pDeliveryArea; }
            set { _pDeliveryArea = value; }
        }

        private object _mthlyrent;
        public object MonthlyRent
        {
            get { return _mthlyrent; }
            set { _mthlyrent = value; }
        }
        private string _linked = "";
        public string LinkedCustomer
        {
            get { return _linked; }
            set { _linked = value; }
        }
        private string _spouseorjoint = "";
        public string SpouseOrJoint
        {
            get { return _spouseorjoint; }
            set { _spouseorjoint = value; }
        }
        private decimal _limit = 0;
        public decimal RFLimit
        {
            get { return _limit; }
            set { _limit = value; }
        }
        private decimal _oldlimit = 0;
        public decimal OldRFCreditLimit
        {
            get { return _oldlimit; }
            set { _oldlimit = value; }
        }
        private decimal _available = 0;
        public decimal RFAvailable
        {
            get { return _available; }
            set { _available = value; }
        }

        private decimal _availablecredit = 0;
        public decimal AvailableCredit
        {
            get { return _availablecredit; }
            set { _availablecredit = value; }
        }

        private string _cardprinted = "";
        public string CardPrinted
        {
            get { return _cardprinted; }
            set { _cardprinted = value; }
        }

        private decimal _totalagreements = 0;
        public decimal TotalAgreements
        {
            get { return _totalagreements; }
            set { _totalagreements = value; }
        }

        private decimal _totalarrears = 0;
        public decimal TotalArrears
        {
            get { return _totalarrears; }
            set { _totalarrears = value; }
        }

        private decimal _totalbalances = 0;
        public decimal TotalBalances
        {
            get { return _totalbalances; }
            set { _totalbalances = value; }
        }

        private decimal _totalcredit = 0;
        public decimal TotalCredit
        {
            get { return _totalcredit; }
            set { _totalcredit = value; }
        }

        private decimal _totalallinstalments = 0;
        public decimal TotalAllInstalments
        {
            get { return _totalallinstalments; }
            set { _totalallinstalments = value; }
        }
        private decimal _totaldeliveredinstalments = 0;
        public decimal TotalDeliveredInstalments
        {
            get { return _totaldeliveredinstalments; }
            set { _totaldeliveredinstalments = value; }
        }


        private DateTime _dateLastScored = DateTime.MinValue.AddYears(1899);
        public DateTime DateLastScored
        {
            get { return _dateLastScored; }
            set { _dateLastScored = value; }
        }

        //@StoreCardLimit MONEY OUT,
        //@StoreCardAvailable MONEY OUT,
        //@StoreCardApproved
        private decimal _storeCardLimit = 0;
        public decimal StoreCardLimit
        {
            get { return _storeCardLimit; }
            set { _storeCardLimit = value; }
        }

        private decimal _storeCardAvailable = 0;
        public decimal StoreCardAvailable
        {
            get { return _storeCardAvailable; }
            set { _storeCardAvailable = value; }
        }


        private bool _storeCardApproved = false;
        public bool StoreCardApproved
        {
            get { return _storeCardApproved; }
            set { _storeCardApproved = value; }
        }
        //private int _userNo = 0;
        public int UserNo
        {
            get { return _user; }
            set { _user = value; }
        }

        private DateTime _dateChanged = DateTime.MinValue.AddYears(1899);
        public DateTime DateChanged
        {
            get { return _dateChanged; }
            set { _dateChanged = value; }
        }

        private DateTime _datenextpaymentdue = DateTime.MinValue.AddYears(1899);
        public DateTime DateNextPaymentDue
        {
            get { return _datenextpaymentdue; }
            set { _datenextpaymentdue = value; }
        }

        private string _limittype = "";
        public string LimitType
        {
            get { return _limittype; }
            set { _limittype = value; }
        }

        private string _scoringBand = "";
        public string scoringBand
        {
            get { return _scoringBand; }
            set { _scoringBand = value; }
        }

        private string _storeType = "";
        public string StoreType
        {
            get { return _storeType; }
            set { _storeType = value; }
        }

        private bool _loanQualified = false;
        public bool LoanQualified
        {
            get { return _loanQualified; }
            set { _loanQualified = value; }
        }

        //IP - 28/08/09 - 5.2 UAT(823)
        private bool _blacklisted = false;
        public bool Blacklisted
        {
            get { return _blacklisted; }
            set { _blacklisted = value; }
        }

        //IP - 03/11/11 - CR1232 - Cash Loans
        private string _cashLoanBlocked = string.Empty;
        public string CashLoanBlocked
        {
            get { return _cashLoanBlocked; }
            set { _cashLoanBlocked = value; }
        }

        public bool ResieveSms { get; set; }

        //public DataSet dsRenissanceSales { get; set; }

        public DataSet GetRenissanceSalesDetails(SqlConnection conn, SqlTransaction trans)
        {
            DataSet dsRenissanceSales = new DataSet();
            try
            {

                if (conn != null && trans != null)
                    this.RunSP(conn, trans, "DN_GetRenissanceSaleRawData", null, dsRenissanceSales);
                else
                    this.RunSP("DN_GetRenissanceSaleRawData", dsRenissanceSales);


            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dsRenissanceSales;
        }

        public void ReceiveRenissanceSaleDataFlag(SqlConnection conn, SqlTransaction trans)
        {
            try
            {

                if (conn != null && trans != null)
                    this.RunSP("DN_ReceiveRenissanceSaleDataFlg");
                else
                    this.RunSP("DN_ReceiveRenissanceSaleDataFlg");

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

    }
}
