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
    public class DCustomer : DALObject
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
        private string _lastname = "";
        private double? _latitude; // Address Standardization CR2019 - 025
        private double? _longitude; // Address Standardization CR2019 - 025


        public string Lastname //CR 2018 13
        {
            get { return _lastname; }
            set { _lastname = value; }
        }
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
        // Address Standardization CR2019 - 025
        public double? Latitude
        {
            get { return _latitude; }
            set { _latitude = value; }
        }
        // Address Standardization CR2019 - 025
        public double? Longitude
        {
            get { return _longitude; }
            set { _longitude = value; }
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

        private decimal _ExistCashLoan = 0;
        public decimal ExistCashLoan
        {
            get { return _ExistCashLoan; }
            set { _ExistCashLoan = value; }
        }

        public bool ResieveSms { get; set; }

        public bool IsSpouseWorking { get; set; }

        public int DependantsFromProposal { get; set; }

        public bool CreditBlocked(SqlConnection conn, SqlTransaction trans)
        {
            bool blocked = false;
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, CW.CustomerID);
                parmArray[0].Value = this.CustID;
                parmArray[1] = new SqlParameter("@blocked", SqlDbType.TinyInt);
                parmArray[1].Value = 0;
                parmArray[1].Direction = ParameterDirection.Output;

                if (conn != null && trans != null)
                    this.RunSP(conn, trans, "DN_CustomerIsCreditBlockedSP", parmArray);
                else
                    this.RunSP("DN_CustomerIsCreditBlockedSP", parmArray);

                if (parmArray[1].Value != DBNull.Value)
                    blocked = Convert.ToBoolean(parmArray[1].Value);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return blocked;
        }

        public bool HasUnsanctionedRFAccounts(SqlConnection conn, SqlTransaction trans)
        {
            bool hasAccounts = false;
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, CW.CustomerID);
                parmArray[0].Value = this.CustID;
                parmArray[1] = new SqlParameter("@hasAccounts", SqlDbType.TinyInt);
                parmArray[1].Value = 0;
                parmArray[1].Direction = ParameterDirection.Output;

                if (conn != null && trans != null)
                    this.RunSP(conn, trans, "DN_CustomerHasUnsanctionedRFAccountsSP", parmArray);
                else
                    this.RunSP("DN_CustomerHasUnsanctionedRFAccountsSP", parmArray);

                if (parmArray[1].Value != DBNull.Value)
                    hasAccounts = Convert.ToBoolean(parmArray[1].Value);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return hasAccounts;
        }

        public void SetDateLastScored(SqlConnection conn, SqlTransaction trans, DateTime lastScored)
        {
            try
            {


                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, CW.CustomerID);
                parmArray[0].Value = this.CustID;
                parmArray[1] = new SqlParameter("@lastScored", SqlDbType.DateTime);
                parmArray[1].Value = lastScored;

                this.RunSP(conn, trans, "DN_CustomerSetDateLastScoredSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public DataTable GetRow(string name)
        {
            DataTable dt = new DataTable(name);
            //dt.TableName = "custinfo";
            dt.Columns.AddRange(new DataColumn[] { new DataColumn(CN.CustomerID),
                                                     new DataColumn(CN.LastName),
                                                     new DataColumn(CN.FirstName),
                                                     new DataColumn(CN.Title),
                                                     new DataColumn(CN.Alias),
                                                     new DataColumn(CN.Sex),
                                                     new DataColumn(CN.Ethnicity),
                                                     new DataColumn(CN.MoreRewardsNo),
                                                     new DataColumn(CN.EffectiveDate, Type.GetType("System.DateTime")),
                                                     new DataColumn(CN.IDType),
                                                     new DataColumn(CN.DOB, Type.GetType("System.DateTime")),
                                                     new DataColumn(CN.DateIn, Type.GetType("System.DateTime")),
                                                     new DataColumn(CN.DateOut, Type.GetType("System.DateTime")),
                                                     new DataColumn(CN.ResidentialStatus),
                                                     new DataColumn(CN.PropertyType),
                                                     new DataColumn(CN.Address1),
                                                     new DataColumn(CN.Address2),
                                                     new DataColumn(CN.Address3),
                                                     new DataColumn(CN.PostCode),
                                                     new DataColumn(CN.DeliveryArea),
                                                     new DataColumn(CN.PrevDateIn, Type.GetType("System.DateTime")),
                                                     new DataColumn(CN.PrevDateOut, Type.GetType("System.DateTime")),
                                                     new DataColumn(CN.PrevResidentialStatus),
                                                     new DataColumn(CN.PrevPropertyType),
                                                     new DataColumn(CN.MonthlyRent, Type.GetType("System.Double")),
                                                     new DataColumn(CN.PAddress1),
                                                     new DataColumn(CN.PAddress2),
                                                     new DataColumn(CN.PAddress3),
                                                     new DataColumn(CN.PPostCode),
                                                     new DataColumn(CN.PDeliveryArea),
                                                     new DataColumn(CN.Dependants, Type.GetType("System.Int32")),
                                                     new DataColumn(CN.MaritalStatus),
                                                     new DataColumn(CN.Nationality)
            });
            var latitudeColumn = new DataColumn(CN.Latitude, Type.GetType("System.Double"));// Address Standardization CR2019 - 025
            latitudeColumn.AllowDBNull = true;
            var longitudeColumn = new DataColumn(CN.Longitude, Type.GetType("System.Double")); // Address Standardization CR2019 - 025
            longitudeColumn.AllowDBNull = true;

            dt.Columns.Add(latitudeColumn);
            dt.Columns.Add(longitudeColumn);

            dt.Rows.Add(new object[] { this.CustID,
                                         this.Name,
                                         this.FirstName,
                                         this.Title,
                                         this.Alias,
                                         this.Sex,
                                         this.Ethnicity,
                                         this.MoreRewardsNo,
                                         this.EffectiveDate,
                                         this.IDType,
                                         this.DateBorn,
                                         this.DateIn,
                                         this.DateOut,
                                         this.ResidentialStatus,
                                         this.PropertyType,
                                         this.Address1,
                                         this.Address2,
                                         this.Address3,
                                         this.PostCode,
                                         this.DeliveryArea,
                                         this.PrevDateIn,
                                         this.PrevDateOut,
                                         this.PrevResidentialStatus,
                                         this.PrevPropertyType,
                                         this.MonthlyRent,
                                         this.PAddress1,
                                         this.PAddress2,
                                         this.PAddress3,
                                         this.PPostCode,
                                         this.PDeliveryArea,
                                         this.Dependants,
                                         this.MaritalStatus,
                                         this.Nationality,
                                         this.Latitude.HasValue ? this.Latitude.Value : (object)DBNull.Value, // Address Standardization CR2019 - 025
                                         this.Longitude.HasValue ? this.Longitude.Value : (object)DBNull.Value // Address Standardization CR2019 - 025
            });

            return dt;
        }

        public int GetCustomerDetails(SqlConnection conn, SqlTransaction trans, string customerID)
        {
            try
            {
                parmArray = new SqlParameter[24];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, CW.CustomerID);
                parmArray[0].Value = customerID;
                parmArray[1] = new SqlParameter("@origbr", SqlDbType.SmallInt);
                parmArray[1].Value = 0;
                parmArray[2] = new SqlParameter("@otherid", SqlDbType.NVarChar, 15);
                parmArray[2].Value = "";
                parmArray[3] = new SqlParameter("@branchnohdle", SqlDbType.SmallInt);
                parmArray[3].Value = 0;
                parmArray[4] = new SqlParameter("@name", SqlDbType.NVarChar, 60);
                parmArray[4].Value = "";
                parmArray[5] = new SqlParameter("@firstname", SqlDbType.NVarChar, 30);
                parmArray[5].Value = "";
                parmArray[6] = new SqlParameter("@title", SqlDbType.NVarChar, 25);
                parmArray[6].Value = "";
                parmArray[7] = new SqlParameter("@alias", SqlDbType.NVarChar, 25);
                parmArray[7].Value = "";
                parmArray[8] = new SqlParameter("@addrsort", SqlDbType.NVarChar, 20);
                parmArray[8].Value = "";
                parmArray[9] = new SqlParameter("@namesort", SqlDbType.NVarChar, 20);
                parmArray[9].Value = "";
                parmArray[10] = new SqlParameter("@datebornstr", SqlDbType.NVarChar, 10);
                parmArray[10].Value = "";
                parmArray[11] = new SqlParameter("@sex", SqlDbType.NVarChar, 1);
                parmArray[11].Value = "";
                parmArray[12] = new SqlParameter("@ethnicity", SqlDbType.NVarChar, 1);
                parmArray[12].Value = "";
                parmArray[13] = new SqlParameter("@morerewardsno", SqlDbType.NVarChar, 16);
                parmArray[13].Value = "";
                parmArray[14] = new SqlParameter("@effectivedate", SqlDbType.DateTime);
                parmArray[14].Value = DBNull.Value;
                parmArray[15] = new SqlParameter("@idtype", SqlDbType.NVarChar, 4);
                parmArray[15].Value = "";
                parmArray[16] = new SqlParameter("@idnumber", SqlDbType.NVarChar, 30);
                parmArray[16].Value = "";
                parmArray[17] = new SqlParameter("@dateborn", SqlDbType.DateTime);
                parmArray[17].Value = DBNull.Value;
                parmArray[18] = new SqlParameter("@maidenname", SqlDbType.NVarChar, 30);
                parmArray[18].Value = "";
                parmArray[19] = new SqlParameter("@dependants", SqlDbType.Int);
                parmArray[19].Value = ""; //Added for CR835 [PC]
                parmArray[20] = new SqlParameter("@maritalStat", SqlDbType.NChar, 1);
                parmArray[20].Value = "";  //Added for CR835 [PC]
                parmArray[21] = new SqlParameter("@nationality", SqlDbType.NChar, 4);
                parmArray[21].Value = "";  //Added for CR835 [PC]
                parmArray[22] = new SqlParameter("@storetype", SqlDbType.NChar, 2);
                parmArray[22].Value = "";
                parmArray[23] = new SqlParameter("@scoringband", SqlDbType.VarChar, 2);
                parmArray[23].Value = "";


                foreach (SqlParameter parm in parmArray)
                {
                    parm.Direction = ParameterDirection.Output;
                }
                parmArray[0].Direction = ParameterDirection.Input;

                if (conn != null && trans != null)
                    this.RunSP(conn, trans, "DN_CustomerGetSP", parmArray);
                else
                    this.RunSP("DN_CustomerGetSP", parmArray);

                if (!Convert.IsDBNull(parmArray[0].Value))
                    _custID = (string)parmArray[0].Value;
                if (!Convert.IsDBNull(parmArray[1].Value))
                    _origbr = (short)parmArray[1].Value;
                if (!Convert.IsDBNull(parmArray[2].Value))
                    _otherid = (string)parmArray[2].Value;
                if (!Convert.IsDBNull(parmArray[3].Value))
                    _branchnohdle = (short)parmArray[3].Value;
                if (!Convert.IsDBNull(parmArray[4].Value))
                    _name = (string)parmArray[4].Value;
                if (!Convert.IsDBNull(parmArray[5].Value))
                    _firstname = (string)parmArray[5].Value;
                if (!Convert.IsDBNull(parmArray[6].Value))
                    _title = (string)parmArray[6].Value;
                if (!Convert.IsDBNull(parmArray[7].Value))
                    _alias = (string)parmArray[7].Value;
                if (!Convert.IsDBNull(parmArray[8].Value))
                    _addrsort = (string)parmArray[8].Value;
                if (!Convert.IsDBNull(parmArray[9].Value))
                    _namesort = (string)parmArray[9].Value;
                if (!Convert.IsDBNull(parmArray[10].Value))
                    _datebornstr = (string)parmArray[10].Value;
                if (!Convert.IsDBNull(parmArray[11].Value))
                    _sex = (string)parmArray[11].Value;
                if (!Convert.IsDBNull(parmArray[12].Value))
                    _ethnicity = (string)parmArray[12].Value;
                if (!Convert.IsDBNull(parmArray[13].Value))
                    _morerewardsno = (string)parmArray[13].Value;
                if (!Convert.IsDBNull(parmArray[14].Value))
                    _effectivedate = (DateTime)parmArray[14].Value;
                if (!Convert.IsDBNull(parmArray[15].Value))
                    _idtype = (string)parmArray[15].Value;
                if (!Convert.IsDBNull(parmArray[16].Value))
                    _idnumber = (string)parmArray[16].Value;
                if (!Convert.IsDBNull(parmArray[17].Value))
                    _dateborn = (DateTime)parmArray[17].Value;
                if (!Convert.IsDBNull(parmArray[18].Value))
                    _maidenname = (string)parmArray[18].Value;
                if (!Convert.IsDBNull(parmArray[19].Value))
                    _dependants = (int)parmArray[19].Value;
                if (!Convert.IsDBNull(parmArray[20].Value))
                    _maritalStatus = (string)parmArray[20].Value;
                if (!Convert.IsDBNull(parmArray[21].Value))
                    _nationality = (string)parmArray[21].Value;
                if (!Convert.IsDBNull(parmArray[22].Value))
                    _storeType = (string)parmArray[22].Value;
                if (!Convert.IsDBNull(parmArray[23].Value))
                    _scoringBand = (string)parmArray[23].Value;

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;
        }

        public int Save(SqlConnection conn, SqlTransaction trans, string customerID)
        {
            try
            {
                parmArray = new SqlParameter[25];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, CW.CustomerID);
                parmArray[0].Value = customerID;
                parmArray[1] = new SqlParameter("@origbr", SqlDbType.SmallInt);
                parmArray[1].Value = this.OrigBr;
                parmArray[2] = new SqlParameter("@otherid", SqlDbType.NVarChar, 20);     //IP - 11/05/10 - UAT(128) UAT5.2.1.0 log - Changed from NVarChar(15)
                parmArray[2].Value = this.OtherId;
                parmArray[3] = new SqlParameter("@branchnohdle", SqlDbType.SmallInt);
                parmArray[3].Value = this.BranchNoHandle;
                parmArray[4] = new SqlParameter("@name", SqlDbType.NVarChar, 60);
                parmArray[4].Value = this.Name;
                parmArray[5] = new SqlParameter("@firstname", SqlDbType.NVarChar, 30);
                parmArray[5].Value = this.FirstName;
                parmArray[6] = new SqlParameter("@title", SqlDbType.NVarChar, 25);
                parmArray[6].Value = this.Title;
                parmArray[7] = new SqlParameter("@alias", SqlDbType.NVarChar, 25);
                parmArray[7].Value = this.Alias;
                parmArray[8] = new SqlParameter("@addrsort", SqlDbType.NVarChar, 20);
                parmArray[8].Value = this.AddressSort;
                parmArray[9] = new SqlParameter("@namesort", SqlDbType.NVarChar, 20);
                parmArray[9].Value = this.NameSort;
                parmArray[10] = new SqlParameter("@dateborn", SqlDbType.DateTime);
                if (DateBorn.Year == 1)
                    DateBorn = DateBorn.AddYears(1900);	//So that it inserts OK...really?

                parmArray[10].Value = this.DateBorn;
                parmArray[11] = new SqlParameter("@sex", SqlDbType.NChar, 1);
                parmArray[11].Value = this.Sex;
                parmArray[12] = new SqlParameter("@ethnicity", SqlDbType.NChar, 1);
                parmArray[12].Value = this.Ethnicity;
                parmArray[13] = new SqlParameter("@morerewardsno", SqlDbType.NVarChar, 16);
                parmArray[13].Value = this.MoreRewardsNo;
                parmArray[14] = new SqlParameter("@effectivedate", SqlDbType.SmallDateTime);

                if (EffectiveDate.Year == 1)//this must be the amazing hack of the 'NULL DATE' = 01-01-1900
                    parmArray[14].Value = Convert.DBNull;
                else
                    parmArray[14].Value = this.EffectiveDate;

                parmArray[15] = new SqlParameter("@idtype", SqlDbType.NChar, 4);
                parmArray[15].Value = this.IDType;
                parmArray[16] = new SqlParameter("@idnumber", SqlDbType.NChar, 30);
                parmArray[16].Value = this.IDNumber;
                parmArray[17] = new SqlParameter("@userno", SqlDbType.Int);
                parmArray[17].Value = this.UserNo;
                parmArray[18] = new SqlParameter("@datechanged", SqlDbType.DateTime);
                parmArray[18].Value = this.DateChanged;
                parmArray[19] = new SqlParameter("@maidenname", SqlDbType.NVarChar, 30);
                parmArray[19].Value = this.MaidenName;

                parmArray[20] = new SqlParameter("@storetype", SqlDbType.NVarChar, 2);
                parmArray[20].Value = this.StoreType;

                parmArray[21] = new SqlParameter("@dependants", SqlDbType.Int);
                parmArray[21].Value = this.Dependants;

                parmArray[22] = new SqlParameter("@maritalStat", SqlDbType.NChar, 1);
                parmArray[22].Value = this.MaritalStatus;

                parmArray[23] = new SqlParameter("@Nationality", SqlDbType.NChar, 4);
                parmArray[23].Value = this.Nationality;

                parmArray[24] = new SqlParameter("@ResieveSms", SqlDbType.Bit, 1);
                parmArray[24].Value = this.ResieveSms;

                result = this.RunSP(conn, trans, "DN_CustomerUpdateSP", parmArray);

                if (result == 0)
                {
                    result = (int)Return.Success;
                }
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

        //public int SaveAddress(SqlConnection conn, SqlTransaction trans, string customerID, int user, DataRow row)
        //{
        //    try
        //    {
        //        parmArray = new SqlParameter[15];
        //        parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, CW.CustomerID);
        //        parmArray[0].Value = customerID;
        //        parmArray[1] = new SqlParameter("@addressType", SqlDbType.NChar, CW.AddressType);
        //        parmArray[1].Value = row["AddressType"];
        //        parmArray[2] = new SqlParameter("@address1", SqlDbType.NVarChar, CW.Address1);
        //        parmArray[2].Value = row["Address1"];
        //        parmArray[3] = new SqlParameter("@address2", SqlDbType.NVarChar, CW.Address2);
        //        parmArray[3].Value = row["Address2"];
        //        parmArray[4] = new SqlParameter("@address3", SqlDbType.NVarChar, CW.Address3);
        //        parmArray[4].Value = row["Address3"];
        //        parmArray[5] = new SqlParameter("@postcode", SqlDbType.NVarChar, CW.PostCode);
        //        parmArray[5].Value = row["PostCode"];
        //        parmArray[6] = new SqlParameter("@DeliveryArea", SqlDbType.NVarChar, CW.DeliveryArea);
        //        parmArray[6].Value = row[CN.DeliveryArea];
        //        parmArray[7] = new SqlParameter("@email", SqlDbType.NVarChar, CW.Email);
        //        parmArray[7].Value = row["EMail"];
        //        parmArray[8] = new SqlParameter("@dialcode", SqlDbType.NVarChar, CW.DialCode);
        //        parmArray[8].Value = row["DialCode"];
        //        parmArray[9] = new SqlParameter("@phone", SqlDbType.NVarChar, CW.Phone);
        //        parmArray[9].Value = row["PhoneNo"];
        //        parmArray[10] = new SqlParameter("@ext", SqlDbType.NVarChar, CW.Ext);
        //        parmArray[10].Value = row["Ext"];
        //        parmArray[11] = new SqlParameter("@datein", SqlDbType.DateTime);
        //        parmArray[11].Value = row["DateIn"];
        //        parmArray[12] = new SqlParameter("@user", SqlDbType.Int);
        //        parmArray[12].Value = user;
        //        parmArray[13] = new SqlParameter("@newRecord", SqlDbType.Int);
        //        parmArray[13].Value = row["NewRecord"];
        //        /*25/09/02 any notes aa*/
        //        parmArray[14] = new SqlParameter("@notes", SqlDbType.NVarChar, CW.AddressNotes);
        //        parmArray[14].Value = row["Notes"];


        //        result = this.RunSP(conn, trans, "DN_CustAddressSaveSP", parmArray);

        //        if (result == 0)
        //            result = (int)Return.Success;
        //    }
        //    catch (SqlException ex)
        //    {
        //        LogSqlException(ex);
        //        throw ex;
        //    }
        //    return result;
        //}

        public int SaveAddress(SqlConnection conn, SqlTransaction trans, string customerID, int user, DataRow row)
        {
            try
            {
                parmArray = new SqlParameter[18];           //CR1084
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, CW.CustomerID);
                parmArray[0].Value = customerID;
                parmArray[1] = new SqlParameter("@addressType", SqlDbType.NChar, CW.AddressType);
                parmArray[1].Value = row["AddressType"];
                parmArray[2] = new SqlParameter("@address1", SqlDbType.NVarChar, CW.Address1);
                parmArray[2].Value = row["Address1"];
                parmArray[3] = new SqlParameter("@address2", SqlDbType.NVarChar, CW.Address2);
                parmArray[3].Value = row["Address2"];
                parmArray[4] = new SqlParameter("@address3", SqlDbType.NVarChar, CW.Address3);
                parmArray[4].Value = row["Address3"];
                parmArray[5] = new SqlParameter("@postcode", SqlDbType.NVarChar, CW.PostCode);
                parmArray[5].Value = row["PostCode"];
                parmArray[6] = new SqlParameter("@DeliveryArea", SqlDbType.NVarChar, CW.DeliveryArea);
                parmArray[6].Value = row[CN.DeliveryArea];
                /*25/09/02 any notes aa*/
                parmArray[7] = new SqlParameter("@notes", SqlDbType.NVarChar, CW.AddressNotes);
                parmArray[7].Value = row["Notes"];
                parmArray[8] = new SqlParameter("@email", SqlDbType.NVarChar, CW.Email);
                parmArray[8].Value = row["EMail"];
                parmArray[9] = new SqlParameter("@datein", SqlDbType.DateTime);
                parmArray[9].Value = row["DateIn"];
                parmArray[10] = new SqlParameter("@user", SqlDbType.Int);
                parmArray[10].Value = user;
                parmArray[11] = new SqlParameter("@newRecord", SqlDbType.Bit);
                parmArray[11].Value = row["NewRecord"];
                parmArray[12] = new SqlParameter("@Zone", SqlDbType.VarChar, CW.Zone);         //CR1084
                parmArray[12].Value = row[CN.Zone];
                // CR2018-008 by KM 16/10/2018*@
                parmArray[13] = new SqlParameter("@DELTitleC", SqlDbType.VarChar, CW.DELTitleC);
                parmArray[13].Value = row["DELTitleC"];
                parmArray[14] = new SqlParameter("@DELFirstname", SqlDbType.VarChar, CW.DELFirstname);//CRKD
                parmArray[14].Value = row["DELFirstname"];                                                //CRKD    
                parmArray[15] = new SqlParameter("@DELLastname", SqlDbType.VarChar, CW.DELLastname);
                parmArray[15].Value = row["DELLastname"];
                parmArray[16] = new SqlParameter("@Latitude", SqlDbType.Float); // Address Standardization CR2019 - 025
                parmArray[16].Value = row["Latitude"];
                parmArray[17] = new SqlParameter("@Longitude", SqlDbType.Float); // Address Standardization CR2019 - 025
                parmArray[17].Value = row["Longitude"];

                result = this.RunSP(conn, trans, "CustAddressSave", parmArray);

                if (result == 0)
                    result = (int)Return.Success;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;
        }

        public int SaveTelephone(SqlConnection conn, SqlTransaction trans, string customerID, int user, DataRow row)
        {
            try
            {
                parmArray = new SqlParameter[8];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, CW.CustomerID);
                parmArray[0].Value = customerID;
                parmArray[1] = new SqlParameter("@tellocn", SqlDbType.NChar, 3);
                parmArray[1].Value = row["AddressType"];
                parmArray[2] = new SqlParameter("@dateteladd", SqlDbType.DateTime);
                parmArray[2].Value = row["DateIn"];
                parmArray[3] = new SqlParameter("@telno", SqlDbType.NVarChar, 20);
                parmArray[3].Value = row["phoneNo"];
                parmArray[4] = new SqlParameter("@extnno", SqlDbType.NVarChar, 6);
                parmArray[4].Value = row["ext"];
                parmArray[5] = new SqlParameter("@dialcode", SqlDbType.NChar, 8);
                parmArray[5].Value = row["dialcode"];
                parmArray[6] = new SqlParameter("@empeenochange", SqlDbType.Int);
                parmArray[6].Value = user;
                parmArray[7] = new SqlParameter("@newRecord", SqlDbType.Bit);
                parmArray[7].Value = row["NewRecord"];


                result = this.RunSP(conn, trans, "CustTelSave", parmArray);

                if (result == 0)
                    result = (int)Return.Success;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;
        }

        public int SaveHomeAddress(SqlConnection conn, SqlTransaction trans, string customerID)
        {
            try
            {
                parmArray = new SqlParameter[8];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, CW.CustomerID);
                parmArray[0].Value = customerID;
                parmArray[1] = new SqlParameter("@datein", SqlDbType.DateTime);
                parmArray[1].Value = this.DateIn;
                parmArray[2] = new SqlParameter("@resstatus", SqlDbType.NChar, CW.ResStatus);
                parmArray[2].Value = this.ResidentialStatus;
                parmArray[3] = new SqlParameter("@proptype", SqlDbType.NChar, CW.PropType);
                parmArray[3].Value = this.PropertyType;
                parmArray[4] = new SqlParameter("@pdatein", SqlDbType.DateTime);
                parmArray[4].Value = this.PrevDateIn;
                parmArray[5] = new SqlParameter("@presstatus", SqlDbType.NChar, CW.ResStatus);
                parmArray[5].Value = this.PrevResidentialStatus;
                parmArray[6] = new SqlParameter("@pproptype", SqlDbType.NChar, CW.PropType);
                parmArray[6].Value = this.PrevPropertyType;
                parmArray[7] = new SqlParameter("@mthlyrent", SqlDbType.Float);
                parmArray[7].Value = this.MonthlyRent;
                string s11 = "";
                foreach (SqlParameter p in parmArray) { s11 += p.ParameterName + " = " + p.Value.ToString() + ","; }
                result = this.RunSP(conn, trans, "DN_CustomerAddressUpdateHomeSP", parmArray);

                if (result == 0)
                    result = (int)Return.Success;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;
        }
        //CR 835 Added [Peter Chong] 11-Oct-2006
        public int SaveCustomerAdditionalDetailsResidential(SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                int result = 0;
                parmArray = new SqlParameter[7];

                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, CW.CustomerID);
                parmArray[0].Value = this.CustID;

                parmArray[1] = new SqlParameter("@DateinPreviousAddress", SqlDbType.DateTime);
                parmArray[1].Value = this.PrevDateIn;

                parmArray[2] = new SqlParameter("@DateinCurrentAddress", SqlDbType.DateTime);
                parmArray[2].Value = this.DateIn;

                parmArray[3] = new SqlParameter("@ResidentialStatus", SqlDbType.NChar, CW.ResStatus);
                parmArray[3].Value = this.ResidentialStatus;

                parmArray[4] = new SqlParameter("@PrevResidentialStatus", SqlDbType.NChar, CW.ResStatus);
                parmArray[4].Value = this.PrevResidentialStatus;

                parmArray[5] = new SqlParameter("@Mthlyrent", SqlDbType.Float);
                parmArray[5].Value = this.MonthlyRent;

                parmArray[6] = new SqlParameter("@PropertyType", SqlDbType.Char, 4);
                parmArray[6].Value = this.PropertyType;

                result = this.RunSP(conn, trans, "DN_CustomerAdditionalDetailsResidentialUpdate", parmArray);

                if (result == 0)
                    result = (int)Return.Success;

                return result;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }


        /// <summary>
        /// Returns the codes currently associated with a customer
        /// </summary>
        /// <param name="custID"></param>
        /// <returns></returns>
        /// 
        public int GetCodesForCustomer(string customerID, out bool noSuchCust)
        {
            try
            {
                _custCodes = new DataTable("CustomerCodes");
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, CW.CustomerID);
                parmArray[0].Value = customerID;

                result = this.RunSP("DN_CustomerCodesGetSP", parmArray, _custCodes);

                noSuchCust = result == -1 ? true : false;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;
        }

        public int GetCustomerHomeAddress(string customerID)
        {
            try
            {
                parmArray = new SqlParameter[22];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, CW.CustomerID);
                parmArray[0].Value = customerID;
                parmArray[1] = new SqlParameter("@datein", SqlDbType.DateTime);
                parmArray[1].Value = this.DateIn;
                parmArray[2] = new SqlParameter("@dateout", SqlDbType.DateTime);
                parmArray[2].Value = this.DateOut;
                parmArray[3] = new SqlParameter("@resstatus", SqlDbType.NChar, CW.ResStatus);
                parmArray[3].Value = this.ResidentialStatus;
                parmArray[4] = new SqlParameter("@proptype", SqlDbType.NChar, CW.PropType);
                parmArray[4].Value = this.PropertyType;
                // DSR 21 Jan 2003 - Load address lines as well
                parmArray[5] = new SqlParameter("@address1", SqlDbType.NChar, CW.Address1);
                parmArray[5].Value = this.Address1;
                parmArray[6] = new SqlParameter("@address2", SqlDbType.NChar, CW.Address2);
                parmArray[6].Value = this.Address2;
                parmArray[7] = new SqlParameter("@address3", SqlDbType.NChar, CW.Address3);
                parmArray[7].Value = this.Address3;
                parmArray[8] = new SqlParameter("@postcode", SqlDbType.NChar, CW.PostCode);
                parmArray[8].Value = this.PostCode;
                parmArray[9] = new SqlParameter("@DeliveryArea", SqlDbType.NVarChar, CW.DeliveryArea);
                parmArray[9].Value = this.DeliveryArea;
                parmArray[10] = new SqlParameter("@pdatein", SqlDbType.DateTime);
                parmArray[10].Value = this.PrevDateIn;
                parmArray[11] = new SqlParameter("@pdateout", SqlDbType.DateTime);
                parmArray[11].Value = this.PrevDateOut;
                parmArray[12] = new SqlParameter("@presstatus", SqlDbType.NChar, CW.ResStatus);
                parmArray[12].Value = this.PrevResidentialStatus;
                parmArray[13] = new SqlParameter("@pproptype", SqlDbType.NChar, CW.PropType);
                parmArray[13].Value = this.PrevPropertyType;
                parmArray[14] = new SqlParameter("@mthlyrent", SqlDbType.Float);
                parmArray[14].Value = this.MonthlyRent;
                // DSR 21 Jan 2003 - Load previous address lines as well
                parmArray[15] = new SqlParameter("@paddress1", SqlDbType.NChar, CW.Address1);
                parmArray[15].Value = this.PAddress1;
                parmArray[16] = new SqlParameter("@paddress2", SqlDbType.NChar, CW.Address2);
                parmArray[16].Value = this.PAddress2;
                parmArray[17] = new SqlParameter("@paddress3", SqlDbType.NChar, CW.Address3);
                parmArray[17].Value = this.PAddress3;
                parmArray[18] = new SqlParameter("@ppostcode", SqlDbType.NChar, CW.PostCode);
                parmArray[18].Value = this.PPostCode;
                parmArray[19] = new SqlParameter("@pDeliveryArea", SqlDbType.NVarChar, CW.DeliveryArea);
                parmArray[19].Value = this.PDeliveryArea;
                parmArray[20] = new SqlParameter("@Latitude", SqlDbType.Float); // Address Standardization CR2019 - 025
                parmArray[20].Value = this.Latitude;
                parmArray[21] = new SqlParameter("@Longitude", SqlDbType.Float); // Address Standardization CR2019 - 025
                parmArray[21].Value = this.Longitude;

                foreach (SqlParameter p in parmArray)
                    p.Direction = ParameterDirection.Output;

                parmArray[0].Direction = ParameterDirection.Input;

                result = this.RunSP("DN_CustomerAddressGetHomeSP", parmArray);
                if (result == 0)
                {
                    if (!Convert.IsDBNull(parmArray[1].Value))
                        this.DateIn = (DateTime)parmArray[1].Value;
                    if (!Convert.IsDBNull(parmArray[2].Value))
                        this.DateOut = (DateTime)parmArray[2].Value;
                    if (!Convert.IsDBNull(parmArray[3].Value))
                        this.ResidentialStatus = (string)parmArray[3].Value;
                    if (!Convert.IsDBNull(parmArray[4].Value))
                        this.PropertyType = (string)parmArray[4].Value;
                    // DSR 21 Jan 2003 - Load address lines as well
                    if (!Convert.IsDBNull(parmArray[5].Value))
                        this.Address1 = (string)parmArray[5].Value;
                    if (!Convert.IsDBNull(parmArray[6].Value))
                        this.Address2 = (string)parmArray[6].Value;
                    if (!Convert.IsDBNull(parmArray[7].Value))
                        this.Address3 = (string)parmArray[7].Value;
                    if (!Convert.IsDBNull(parmArray[8].Value))
                        this.PostCode = (string)parmArray[8].Value;
                    if (!Convert.IsDBNull(parmArray[9].Value))
                        this.DeliveryArea = (string)parmArray[9].Value;
                    if (!Convert.IsDBNull(parmArray[10].Value))
                        this.PrevDateIn = (DateTime)parmArray[10].Value;
                    if (!Convert.IsDBNull(parmArray[11].Value))
                        this.PrevDateOut = (DateTime)parmArray[11].Value;
                    if (!Convert.IsDBNull(parmArray[12].Value))
                        this.PrevResidentialStatus = (string)parmArray[12].Value;
                    if (!Convert.IsDBNull(parmArray[13].Value))
                        this.PrevPropertyType = (string)parmArray[13].Value;
                    //if(!Convert.IsDBNull(parmArray[14].Value))
                    this.MonthlyRent = parmArray[14].Value;
                    // DSR 21 Jan 2003 - Load previous address lines as well
                    if (!Convert.IsDBNull(parmArray[15].Value))
                        this.PAddress1 = (string)parmArray[15].Value;
                    if (!Convert.IsDBNull(parmArray[16].Value))
                        this.PAddress2 = (string)parmArray[16].Value;
                    if (!Convert.IsDBNull(parmArray[17].Value))
                        this.PAddress3 = (string)parmArray[17].Value;
                    if (!Convert.IsDBNull(parmArray[18].Value))
                        this.PPostCode = (string)parmArray[18].Value;
                    if (!Convert.IsDBNull(parmArray[19].Value))
                        this.PDeliveryArea = (string)parmArray[19].Value;
                    if (!Convert.IsDBNull(parmArray[20].Value))
                        this.Latitude = double.Parse(parmArray[20].Value.ToString()); // Address Standardization CR2019 - 025
                    if (!Convert.IsDBNull(parmArray[21].Value))
                        this.Longitude = double.Parse(parmArray[21].Value.ToString()); // Address Standardization CR2019 - 025

                    result = (int)Return.Success;
                }
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

        public int GetCustomerAddresses(SqlConnection conn, SqlTransaction trans, string customerID)
        {
            try
            {
                _addresses = new DataTable(TN.CustomerAddresses);
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, CW.CustomerID);
                parmArray[0].Value = customerID;

                if (conn != null && trans != null)
                    result = this.RunSP(conn, trans, "DN_CustomerAddressGet1SP", parmArray, _addresses);
                else
                    result = this.RunSP("DN_CustomerAddressGet1SP", parmArray, _addresses);

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

        public int AddCodeToCustomer(SqlConnection con, SqlTransaction tran, string customerID, string code, DateTime date,
                                        int User, string reference)
        {
            try
            {
                parmArray = new SqlParameter[5];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, CW.CustomerID);
                parmArray[0].Value = customerID;
                parmArray[1] = new SqlParameter("@code", SqlDbType.NVarChar, 4);
                parmArray[1].Value = code;
                parmArray[2] = new SqlParameter("@date", SqlDbType.DateTime);
                parmArray[2].Value = date;
                parmArray[3] = new SqlParameter("@codedby", SqlDbType.Int);
                parmArray[3].Value = User;
                parmArray[4] = new SqlParameter("@reference", SqlDbType.NVarChar, 10);
                parmArray[4].Value = reference;

                result = this.RunSP(con, tran, "DN_CustomerCodeAddSP", parmArray);

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

        public int RemoveCodeFromCustomer(SqlConnection con, SqlTransaction tran, string customerID, string code)
        {
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, CW.CustomerID);
                parmArray[0].Value = customerID;
                parmArray[1] = new SqlParameter("@code", SqlDbType.NVarChar, 4);
                parmArray[1].Value = code;

                result = this.RunSP(con, tran, "DN_CustomerCodeRemoveSP", parmArray);

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

        public int SoleOrJoint(string customerID, string relation)
        {
            try
            {
                parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NChar, CW.CustomerID);
                parmArray[0].Value = customerID;
                parmArray[1] = new SqlParameter("@linked", SqlDbType.NVarChar, 20);
                parmArray[1].Direction = ParameterDirection.Output;
                parmArray[1].Value = "";
                parmArray[2] = new SqlParameter("@type", SqlDbType.NVarChar, 1);
                parmArray[2].Value = relation;

                result = this.RunSP("DN_CustomerSoleOrJointSP", parmArray);

                if (result == 0)
                {
                    _linked = (string)parmArray[1].Value;
                }
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;
        }

        public int Link(SqlConnection con, SqlTransaction tran, string holder, string linked, string relationship)
        {
            try
            {
                parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@holder", SqlDbType.NVarChar, 20);
                parmArray[0].Value = holder;
                parmArray[1] = new SqlParameter("@linked", SqlDbType.NVarChar, 20);
                parmArray[1].Value = linked;
                parmArray[2] = new SqlParameter("@relationship", SqlDbType.NVarChar, 1);
                parmArray[2].Value = relationship;
                result = this.RunSP(con, tran, "DN_CustomerLinkSP", parmArray);

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

        public int DeleteCodesFromCustomer(SqlConnection con, SqlTransaction tran, string customerID, string custCode) //IP - 01/01/09 - 5.2 UAT(823) - Added custCode
        {
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, CW.CustomerID);
                parmArray[0].Value = customerID;
                parmArray[1] = new SqlParameter("@custCode", SqlDbType.NVarChar, 4);
                parmArray[1].Value = custCode;

                result = this.RunSP(con, tran, "DN_CustomerCodesDeleteSP", parmArray);

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

        public int GetCustomerAddress(string customerID, DataSet ds)
        {
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, CW.CustomerID);
                parmArray[0].Value = customerID;

                result = this.RunSP("DN_CustomerAddressGetSP", parmArray, ds);

                if (result == 0)
                {
                    ds.Tables[0].TableName = "CustomerAddress";
                    ds.Tables[1].TableName = "CustomerPhone";
                    result = (int)Return.Success;
                }
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

        //CR 835 Returns a datatable containing addtitional customer details
        public DataTable GetCustomerAdditionalDetailsFinancial(string customerId)
        {
            try
            {
                DataTable dt = new DataTable(TN.CustomerAdditionalDetailsFinancial);
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, CW.CustomerID);
                parmArray[0].Value = customerId;

                this.RunSP("DN_CustomerAdditionalDetailsFinancialGetSP", parmArray, dt);

                return dt;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }


        }
        //CR 835 Returns a datatable containing addtitional customer residential details
        public DataTable GetCustomerAdditionalDetailsResidential(string customerId)
        {
            try
            {
                DataTable dt = new DataTable(TN.CustomerAdditionalDetailsResidential);
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, CW.CustomerID);
                parmArray[0].Value = customerId;

                this.RunSP("DN_CustomerAdditionalDetailsResidentialGetSP", parmArray, dt);

                return dt;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public DataTable GetCustomerAccountsAndDetails(string accountNo)
        {
            try
            {
                _cust = new DataTable(TN.Customer);
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, CW.AccountNo);
                parmArray[0].Value = accountNo;

                result = this.RunSP("DN_CustomerGetAccountsAndDetailsSP", parmArray, _cust);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return _cust;
        }
        public int GetCustomerDetailsD(SqlConnection conn, SqlTransaction trans, string RequestID)      //new address pop add hear by tosif ali 27/12/2018 jec
        {

            try
            {
                parmArray = new SqlParameter[5];          //new address pop add hear by tosif ali 27/12/2018 jec

                parmArray[0] = new SqlParameter("@RequestID", SqlDbType.NVarChar, 12);
                parmArray[0].Value = RequestID;

                parmArray[1] = new SqlParameter("@DELTitleC", SqlDbType.NVarChar, 25);
                parmArray[1].Value = "";
                parmArray[1].Direction = ParameterDirection.Output;
                parmArray[2] = new SqlParameter("@DELFirstName", SqlDbType.NVarChar, 30);
                parmArray[2].Value = "";
                parmArray[2].Direction = ParameterDirection.Output;
                parmArray[3] = new SqlParameter("@DELLastName", SqlDbType.NVarChar, 60);
                parmArray[3].Value = "";
                parmArray[4] = new SqlParameter("@addtype", SqlDbType.NVarChar, 25);
                parmArray[4].Value = "";
                parmArray[4].Direction = ParameterDirection.Output;

                if (conn != null && trans != null)
                    result = this.RunSP(conn, trans, "DN_CustomerDetailsSP", parmArray);
                else
                    result = this.RunSP("DN_CustomerDetailsSP", parmArray);



                if (result == 0)
                {
                    if (!Convert.IsDBNull(parmArray[1].Value))
                        this.DELTitleC = (string)parmArray[1].Value;
                    if (!Convert.IsDBNull(parmArray[2].Value))
                        this.DELFirstName = (string)parmArray[2].Value;
                    if (!Convert.IsDBNull(parmArray[3].Value))
                        this.DELLastName = (string)parmArray[3].Value;
                    if (!Convert.IsDBNull(parmArray[4].Value))
                        this.addtype = (string)parmArray[4].Value;
                    //End Hear
                }
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;
        }


        public ArrayList GetCustomerAccountsDetails(string accountNo)
        {
            var al = new ArrayList();
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, CW.AccountNo) { Value = accountNo };

                al = this.ReturnAL("DN_CustomerGetAccountsAndDetailsSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw;
            }
            return al;
        }

        /// <summary>
        /// Returns all the special arrangement accounts that have not expired
        /// </summary>
        /// <param name="accountNo"></param>
        /// <returns></returns>
        public DataSet GetSPADetails(string acctNo)      //CR1084 jec
        {
            var dsSPA = new DataSet();
            try
            {
                parmArray = new SqlParameter[1];          //CR1084 jec
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12) { Value = acctNo };

                result = this.RunSP("GetSPADetailsSP", parmArray, dsSPA);     //CR1084 jec
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dsSPA;
        }

        public int CustomerSearch(string customerID,
            string firstName,
            string lastName,
            string address,     //CR1084
            string phoneNumber,     //CR1084
            int limit,
            int settled,
            bool exactMatch,
            string storeType, string source)
        {
            try
            {
                _custSearch = new DataTable("CustSearch");
                parmArray = new SqlParameter[10];            //CR1084
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, CW.CustomerID);
                parmArray[0].Value = customerID;
                parmArray[1] = new SqlParameter("@first", SqlDbType.NVarChar, 30);
                parmArray[1].Value = firstName;
                parmArray[2] = new SqlParameter("@last", SqlDbType.NVarChar, 60);
                parmArray[2].Value = lastName;
                parmArray[3] = new SqlParameter("@limit", SqlDbType.Int);
                parmArray[3].Value = limit;
                parmArray[4] = new SqlParameter("@settled", SqlDbType.Int);
                parmArray[4].Value = settled;
                parmArray[5] = new SqlParameter("@exact", SqlDbType.Int);
                parmArray[5].Value = Convert.ToInt32(exactMatch);
                parmArray[6] = new SqlParameter("@storetype", SqlDbType.NVarChar, 2);
                parmArray[6].Value = storeType;
                parmArray[7] = new SqlParameter("@address", SqlDbType.NVarChar, 60);        //CR1084
                parmArray[7].Value = address;           //CR1084
                parmArray[8] = new SqlParameter("@phone", SqlDbType.NVarChar, 20);          //CR1084
                parmArray[8].Value = phoneNumber;               //CR1084
                parmArray[9] = new SqlParameter("@source", SqlDbType.NVarChar, 10);          
                parmArray[9].Value = source;
                result = this.RunSP("DN_CustomerSearchSP", parmArray, _custSearch);

                if (result == 0)
                {
                    result = (int)Return.Success;
                }
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

        public int GetBasicCustomerDetails(SqlConnection conn, SqlTransaction trans, string customerID, string accountNo, string relationship)
        {
            try
            {
                parmArray = new SqlParameter[36];
                parmArray[0] = new SqlParameter("@custID", SqlDbType.NVarChar, CW.CustomerID);
                parmArray[0].Value = customerID;
                parmArray[1] = new SqlParameter("@acctNo", SqlDbType.NVarChar, CW.AccountNo);
                parmArray[1].Value = accountNo;
                parmArray[2] = new SqlParameter("@relationship", SqlDbType.NVarChar, 2);
                parmArray[2].Value = relationship;
                parmArray[3] = new SqlParameter("@title", SqlDbType.NVarChar, 25);
                parmArray[3].Value = "";
                parmArray[3].Direction = ParameterDirection.Output;
                parmArray[4] = new SqlParameter("@firstName", SqlDbType.NVarChar, 30);
                parmArray[4].Value = "";
                parmArray[4].Direction = ParameterDirection.Output;
                parmArray[5] = new SqlParameter("@lastName", SqlDbType.NVarChar, 60);
                parmArray[5].Value = "";
                parmArray[5].Direction = ParameterDirection.Output;
                parmArray[6] = new SqlParameter("@alias", SqlDbType.NVarChar, 25);
                parmArray[6].Value = "";
                parmArray[6].Direction = ParameterDirection.Output;
                parmArray[7] = new SqlParameter("@budgetCard", SqlDbType.Int);
                parmArray[7].Value = 0;
                parmArray[7].Direction = ParameterDirection.Output;
                parmArray[8] = new SqlParameter("@custout", SqlDbType.NVarChar, 20);
                parmArray[8].Value = "";
                parmArray[8].Direction = ParameterDirection.Output;
                parmArray[9] = new SqlParameter("@RFLimit", SqlDbType.Money);
                parmArray[9].Value = 0;
                parmArray[9].Direction = ParameterDirection.Output;
                parmArray[10] = new SqlParameter("@idnumber", SqlDbType.NVarChar, 30);
                parmArray[10].Value = "";
                parmArray[10].Direction = ParameterDirection.Output;
                parmArray[11] = new SqlParameter("@dateborn", SqlDbType.DateTime);
                parmArray[11].Value = DBNull.Value;
                parmArray[11].Direction = ParameterDirection.Output;
                parmArray[12] = new SqlParameter("@RFAvailable", SqlDbType.Money);
                parmArray[12].Value = 0;
                parmArray[12].Direction = ParameterDirection.Output;
                parmArray[13] = new SqlParameter("@maidenname", SqlDbType.NVarChar, 30);
                parmArray[13].Value = "";
                parmArray[13].Direction = ParameterDirection.Output;
                parmArray[14] = new SqlParameter("@sex", SqlDbType.NChar, 1);
                parmArray[14].Value = "";
                parmArray[14].Direction = ParameterDirection.Output;
                parmArray[15] = new SqlParameter("@morerewardsno", SqlDbType.NChar, 16);
                parmArray[15].Value = "";
                parmArray[15].Direction = ParameterDirection.Output;
                parmArray[16] = new SqlParameter("@rfcardseqno", SqlDbType.TinyInt);
                parmArray[16].Value = 0;
                parmArray[16].Direction = ParameterDirection.Output;
                parmArray[17] = new SqlParameter("@oldrfcreditlimit", SqlDbType.Money);
                parmArray[17].Value = 0;
                parmArray[17].Direction = ParameterDirection.Output;
                parmArray[18] = new SqlParameter("@LimitType", SqlDbType.NChar, 1);
                parmArray[18].Value = "";
                parmArray[18].Direction = ParameterDirection.Output;
                parmArray[19] = new SqlParameter("@ScoringBand", SqlDbType.NChar, 1);
                parmArray[19].Value = "";
                parmArray[19].Direction = ParameterDirection.Output;

                parmArray[20] = new SqlParameter("@StoreType", SqlDbType.NVarChar, 2);
                parmArray[20].Value = "";
                parmArray[20].Direction = ParameterDirection.Output;

                parmArray[21] = new SqlParameter("@LoanQualified", SqlDbType.Bit);
                parmArray[21].Direction = ParameterDirection.Output;


                parmArray[22] = new SqlParameter("@dependants", SqlDbType.Int);
                parmArray[22].Value = 0;
                parmArray[22].Direction = ParameterDirection.Output;

                parmArray[23] = new SqlParameter("@maritalStat", SqlDbType.NChar, 1);
                parmArray[23].Value = "";
                parmArray[23].Direction = ParameterDirection.Output;

                parmArray[24] = new SqlParameter("@nationality", SqlDbType.NChar, 4);
                parmArray[24].Value = "";
                parmArray[24].Direction = ParameterDirection.Output;

                //IP - 28/08/09 - 5.2 UAT(823) - Check if Customer has been blacklisted.
                parmArray[25] = new SqlParameter("@blacklisted", SqlDbType.Bit);
                parmArray[25].Value = "";
                parmArray[25].Direction = ParameterDirection.Output;

                parmArray[26] = new SqlParameter("@StoreCardLimit", SqlDbType.Money);
                parmArray[26].Value = "";
                parmArray[26].Direction = ParameterDirection.Output;

                parmArray[27] = new SqlParameter("@StoreCardAvailable", SqlDbType.Money);
                parmArray[27].Value = "";
                parmArray[27].Direction = ParameterDirection.Output;

                parmArray[28] = new SqlParameter("@StoreCardApproved ", SqlDbType.Bit);
                parmArray[28].Value = "";
                parmArray[28].Direction = ParameterDirection.Output;

                parmArray[29] = new SqlParameter("@cashLoanBlocked", SqlDbType.VarChar, 1);          //IP - 03/11/11 - CR1232 - Cash Loans
                parmArray[29].Value = "";
                parmArray[29].Direction = ParameterDirection.Output;

                parmArray[30] = new SqlParameter("@ResieveSms", SqlDbType.Bit, 1);
                parmArray[30].Value = DBNull.Value;
                parmArray[30].Direction = ParameterDirection.Output;

                parmArray[31] = new SqlParameter("@DELTitleC", SqlDbType.NVarChar, 25);
                parmArray[31].Value = "";
                parmArray[31].Direction = ParameterDirection.Output;
                parmArray[32] = new SqlParameter("@DELFirstName", SqlDbType.NVarChar, 30);
                parmArray[32].Value = "";
                parmArray[32].Direction = ParameterDirection.Output;
                parmArray[33] = new SqlParameter("@DELLastName", SqlDbType.NVarChar, 60);
                parmArray[33].Value = "";
                parmArray[33].Direction = ParameterDirection.Output;

                parmArray[34] = new SqlParameter("@IsSpouseWorking", SqlDbType.Bit);
                parmArray[34].Direction = ParameterDirection.Output;
                parmArray[35] = new SqlParameter("@dependantsFromProposal", SqlDbType.Int);
                parmArray[35].Value = 0;
                parmArray[35].Direction = ParameterDirection.Output;

                if (conn != null && trans != null)
                    result = this.RunSP(conn, trans, "DN_CustomerGetBasicDetailsSP", parmArray);
                else
                    result = this.RunSP("DN_CustomerGetBasicDetailsSP", parmArray);


                if (result == 0)
                {
                    if (!Convert.IsDBNull(parmArray[3].Value))
                        this.Title = (string)parmArray[3].Value;
                    if (!Convert.IsDBNull(parmArray[4].Value))
                        this.FirstName = (string)parmArray[4].Value;
                    if (!Convert.IsDBNull(parmArray[5].Value))
                        this.Name = (string)parmArray[5].Value;
                    if (!Convert.IsDBNull(parmArray[6].Value))
                        this.Alias = (string)parmArray[6].Value;
                    if (!Convert.IsDBNull(parmArray[7].Value))
                        this.BudgetCard = (int)parmArray[7].Value;
                    if (!Convert.IsDBNull(parmArray[8].Value))
                        this.CustID = (string)parmArray[8].Value;
                    if (!Convert.IsDBNull(parmArray[9].Value))
                        this.RFLimit = (decimal)parmArray[9].Value;
                    if (!Convert.IsDBNull(parmArray[10].Value))
                        this.IDNumber = (string)parmArray[10].Value;
                    if (!Convert.IsDBNull(parmArray[11].Value))
                        this.DateBorn = (DateTime)parmArray[11].Value;
                    if (!Convert.IsDBNull(parmArray[12].Value))
                        this.AvailableCredit = (decimal)parmArray[12].Value;
                    if (!Convert.IsDBNull(parmArray[13].Value))
                        this.MaidenName = (string)parmArray[13].Value;
                    if (!Convert.IsDBNull(parmArray[14].Value))
                        this.Sex = (string)parmArray[14].Value;

                    if (!Convert.IsDBNull(parmArray[15].Value))
                        this.MoreRewardsNo = (string)parmArray[15].Value;

                    if (!Convert.IsDBNull(parmArray[16].Value))
                        this.RFCardSeqNo = (byte)parmArray[16].Value;

                    if (!Convert.IsDBNull(parmArray[17].Value))
                        this.OldRFCreditLimit = (decimal)parmArray[17].Value;

                    if (!Convert.IsDBNull(parmArray[18].Value))
                        this.LimitType = (string)parmArray[18].Value;

                    if (!Convert.IsDBNull(parmArray[19].Value))
                        this.scoringBand = (string)parmArray[19].Value;

                    if (!Convert.IsDBNull(parmArray[20].Value))
                        this.StoreType = (string)parmArray[20].Value;

                    if (!Convert.IsDBNull(parmArray[21].Value))
                        this.LoanQualified = Convert.ToBoolean(parmArray[21].Value);

                    if (!Convert.IsDBNull(parmArray[22].Value))
                        this.Dependants = (int)parmArray[22].Value;

                    if (!Convert.IsDBNull(parmArray[23].Value))
                        this.MaritalStatus = (string)parmArray[23].Value;

                    if (!Convert.IsDBNull(parmArray[24].Value))
                        this.Nationality = (string)parmArray[24].Value;

                    if (!Convert.IsDBNull(parmArray[25].Value))
                        this.Blacklisted = (bool)parmArray[25].Value;

                    if (!Convert.IsDBNull(parmArray[26].Value))
                        this.StoreCardLimit = (decimal)parmArray[26].Value;

                    if (!Convert.IsDBNull(parmArray[27].Value))
                        this.StoreCardAvailable = (decimal)parmArray[27].Value;

                    if (!Convert.IsDBNull(parmArray[28].Value))
                        this.StoreCardApproved = (bool)parmArray[28].Value;

                    if (!Convert.IsDBNull(parmArray[29].Value))
                        this.CashLoanBlocked = (string)parmArray[29].Value;

                    if (!Convert.IsDBNull(parmArray[30].Value))
                        this.ResieveSms = (bool)parmArray[30].Value;
                    // new address pop add hear by tosif ali 16/10/2018*@
                    if (!Convert.IsDBNull(parmArray[31].Value))
                        this.DELTitleC = (string)parmArray[31].Value;
                    if (!Convert.IsDBNull(parmArray[32].Value))
                        this.DELFirstName = (string)parmArray[32].Value;
                    if (!Convert.IsDBNull(parmArray[33].Value))
                        this.DELLastName = (string)parmArray[33].Value;
                    if (!Convert.IsDBNull(parmArray[34].Value))
                        this.IsSpouseWorking = (bool)parmArray[34].Value;
                    if (!Convert.IsDBNull(parmArray[35].Value))
                        this.DependantsFromProposal = (int)parmArray[35].Value;
                    //End Hear

                }
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw;
            }

            return result;
        }

        public DataTable GetBasicCustomerDetailsForReprint(SqlConnection conn, SqlTransaction trans, string agrmtno)
        {
            DataTable cust = null;
            try
            {
                cust = new DataTable(TN.Customer);
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@Orderid", SqlDbType.NVarChar, CW.agrmtno);
                parmArray[0].Value = agrmtno;

                if (conn != null && trans != null)
                    this.RunSP(conn, trans, "DN_GetBasicCustomerDetailsForReprint", parmArray, cust);
                else
                    this.RunSP("DN_GetBasicCustomerDetailsForReprint", parmArray, cust);



            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw;
            }

            return cust;
        }

        public int GetRFLimit(SqlConnection conn, SqlTransaction trans,
                                string customerID, string acctList)
        {
            // The acctList parameter is used to exclude certain accounts from the calculation.
            // Used by the Add-To calculation to exclude the accounts to be settled and the new account.
            try
            {
                parmArray = new SqlParameter[4];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, CW.CustomerID);
                parmArray[0].Value = customerID;
                parmArray[1] = new SqlParameter("@AcctList", SqlDbType.NVarChar, 400);
                parmArray[1].Value = acctList;

                parmArray[2] = new SqlParameter("@limit", SqlDbType.Money);
                parmArray[2].Value = 0;
                parmArray[2].Direction = ParameterDirection.Output;
                parmArray[3] = new SqlParameter("@available", SqlDbType.Money);
                parmArray[3].Value = 0;
                parmArray[3].Direction = ParameterDirection.Output;

                if (conn != null && trans != null)
                    this.RunSP(conn, trans, "DN_CustomerGetRFLimitSP", parmArray);
                else
                    this.RunSP("DN_CustomerGetRFLimitSP", parmArray);

                decimal rfLimit = 0;
                decimal rfAvailable = 0;

                if (parmArray[2].Value != DBNull.Value)
                    rfLimit = (decimal)parmArray[2].Value;
                if (parmArray[3].Value != DBNull.Value)
                    rfAvailable = (decimal)parmArray[3].Value;

                this.CountryRound(ref rfLimit);
                this.CountryRound(ref rfAvailable);
                this.RFLimit = rfLimit;
                this.RFAvailable = rfAvailable;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;
        }


        public int GetExistCashLoan(SqlConnection conn, SqlTransaction trans, string customerID)
        {
            // The acctList parameter is used to exclude certain accounts from the calculation.
            // Used by the Add-To calculation to exclude the accounts to be settled and the new account.
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@CustomerId", SqlDbType.NVarChar, CW.CustomerID);
                parmArray[0].Value = customerID;
                parmArray[1] = new SqlParameter("@ExistCashLoanAmt", SqlDbType.Money);
                parmArray[1].Value = 0;
                parmArray[1].Direction = ParameterDirection.Output;

                if (conn != null && trans != null)
                    this.RunSP(conn, trans, "DN_AvailableCashLoanForNonEligibleCustomer", parmArray);
                else
                    this.RunSP("DN_AvailableCashLoanForNonEligibleCustomer", parmArray);

                decimal ExistCashLoan = 0;

                if (parmArray[1].Value != DBNull.Value)
                    ExistCashLoan = (decimal)parmArray[1].Value;

                this.CountryRound(ref ExistCashLoan);
                this.ExistCashLoan = ExistCashLoan;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;
        }

        public int GetRFCombinedDetails(string customerID)
        {
            try
            {
                parmArray = new SqlParameter[11];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, CW.CustomerID);
                parmArray[0].Value = customerID;
                parmArray[1] = new SqlParameter("@available_credit", SqlDbType.Money);
                parmArray[1].Value = 0;
                parmArray[2] = new SqlParameter("@Cardprinted", SqlDbType.NChar, 1);
                parmArray[2].Value = "";
                parmArray[3] = new SqlParameter("@total_agreements", SqlDbType.Money);
                parmArray[3].Value = 0;
                parmArray[4] = new SqlParameter("@total_arrears", SqlDbType.Money);
                parmArray[4].Value = 0;
                parmArray[5] = new SqlParameter("@total_balances", SqlDbType.Money);
                parmArray[5].Value = 0;
                parmArray[6] = new SqlParameter("@total_credit", SqlDbType.Money);
                parmArray[6].Value = 0;
                parmArray[7] = new SqlParameter("@total_delivered_instalments", SqlDbType.Money);
                parmArray[7].Value = 0;
                parmArray[8] = new SqlParameter("@total_all_instalments", SqlDbType.Money);
                parmArray[8].Value = 0;
                parmArray[9] = new SqlParameter("@rfcardseqno", SqlDbType.TinyInt);
                parmArray[9].Value = 0;
                parmArray[10] = new SqlParameter("@datenextpaymentdue", SqlDbType.DateTime);

                foreach (SqlParameter p in parmArray)
                    p.Direction = ParameterDirection.Output;

                parmArray[0].Direction = ParameterDirection.Input;

                result = this.RunSP("DN_CustomerGetRFCombinedSP", parmArray);

                if (result == 0)
                {
                    if (parmArray[1].Value != DBNull.Value)
                        this.AvailableCredit = (decimal)parmArray[1].Value;

                    if (parmArray[2].Value != DBNull.Value)
                        this.CardPrinted = (string)parmArray[2].Value;

                    if (parmArray[3].Value != DBNull.Value)
                        this.TotalAgreements = (decimal)parmArray[3].Value;

                    if (parmArray[4].Value != DBNull.Value)
                        this.TotalArrears = (decimal)parmArray[4].Value;

                    if (parmArray[5].Value != DBNull.Value)
                        this.TotalBalances = (decimal)parmArray[5].Value;

                    if (parmArray[6].Value != DBNull.Value)
                        this.TotalCredit = (decimal)parmArray[6].Value;

                    if (parmArray[7].Value != DBNull.Value)
                        this.TotalDeliveredInstalments = (decimal)parmArray[7].Value;

                    if (parmArray[8].Value != DBNull.Value)
                        this.TotalAllInstalments = (decimal)parmArray[8].Value;

                    if (parmArray[9].Value != DBNull.Value)
                        this.RFCardSeqNo = (byte)parmArray[9].Value;

                    if (parmArray[10].Value != DBNull.Value)
                        this.DateNextPaymentDue = (DateTime)parmArray[10].Value;

                }
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;
        }

        public void SetCreditLimit(SqlConnection conn, SqlTransaction trans, string customerID, decimal creditLimit, string limitType)
        {
            try
            {
                parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, CW.CustomerID);
                parmArray[0].Value = customerID;
                parmArray[1] = new SqlParameter("@limit", SqlDbType.Money);
                parmArray[1].Value = creditLimit;
                parmArray[2] = new SqlParameter("@type", SqlDbType.NChar, 1);
                parmArray[2].Value = limitType;

                this.RunSP(conn, trans, "DN_CustomerSetCreditLimitSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void SetOverrideLimit(SqlConnection conn, SqlTransaction trans, string customerID, decimal creditLimit)
        {
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, CW.CustomerID);
                parmArray[0].Value = customerID;
                parmArray[1] = new SqlParameter("@limit", SqlDbType.Money);
                parmArray[1].Value = creditLimit;

                this.RunSP(conn, trans, "DN_CustomerSetOverrideCreditLimitSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public string GetMoreRewardsNo(string customerID)
        {
            string m = "";
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, CW.CustomerID);
                parmArray[0].Value = customerID;
                parmArray[1] = new SqlParameter("@morerewardsno", SqlDbType.NVarChar, 16);
                parmArray[1].Value = "";
                parmArray[1].Direction = ParameterDirection.Output;

                this.RunSP("DN_CustomerGetMoreRewardsNoSP", parmArray);
                if (DBNull.Value != parmArray[1].Value)
                    m = (string)parmArray[1].Value;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return m;
        }

        public DataTable GetDetailsForPayment(SqlConnection conn, SqlTransaction trans,
                                                string customerID)
        {
            DataTable cust = null;
            try
            {
                cust = new DataTable(TN.Customer);
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, CW.CustomerID);
                parmArray[0].Value = customerID;

                this.RunSP(conn, trans, "DN_CustomerGetDetailsForPaymentSP", parmArray, cust);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return cust;
        }

        public DataTable GetAccountsForPayment(SqlConnection conn, SqlTransaction trans,
                                                string customerID)
        {
            DataTable cust = null;
            try
            {
                cust = new DataTable(TN.Accounts);
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, CW.CustomerID);
                parmArray[0].Value = customerID;

                this.RunSP("DN_CustomerGetAccountsForPaymentSP", parmArray, cust);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return cust;
        }

        public DataTable GetTransactionId(string accountID, int transRefNo)
        {
          
            DataTable trans = null;
            try
            {
                trans = new DataTable(TN.Transactions);
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@CosacsAcctNumber", SqlDbType.NVarChar, CW.AccountNo);
                parmArray[0].Value = accountID;

                parmArray[1] = new SqlParameter("@Transrefno", SqlDbType.Int, 16);//update CW.CustomerID with transaction id
                parmArray[1].Value = transRefNo;


                this.RunSP("SP_BS_GetLoanTransId", parmArray, trans);//need to use new SP
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return trans;
        }
        
        public DataTable GetInvoiceNo(string accountID)
        {

            DataTable invoice = null;
            try
            {
                invoice = new DataTable(TN.Accounts);
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@CosacsAcctNumber", SqlDbType.NVarChar, CW.AccountNo);
                parmArray[0].Value = accountID;

                this.RunSP("SP_BS_GetInvoiceNo", parmArray, invoice);//need to use new SP
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return invoice;
        }


        /// <summary>
        /// Loads Account and customer details from acct,instalplan, customer and custacct tables. Populates Datatable _cust
        /// </summary>
        /// <param name="acctNo">The acct no to retrieve data for</param>
        /// <returns>int Status value</returns>
        public int GetDetailsForDebtCollector(string acctNo)
        {
            try
            {
                _cust = new DataTable(TN.Customer);
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, CW.AccountNo);
                parmArray[0].Value = acctNo;

                result = this.RunSP("DN_CustomerGetForDebtCollector", parmArray, _cust);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;
        }

        public DataTable GetRequiredAddressTypes(string customerID)
        {
            DataTable dt = new DataTable(TN.AddressType);
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, CW.CustomerID);
                parmArray[0].Value = customerID;

                RunSP("DN_CustomerGetRequiredAddressTypesSP", parmArray, dt);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dt;
        }

        public DataTable GetDistinctAddressTypes(string customerID)
        {
            DataTable dt = new DataTable(TN.AddressType);
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, CW.CustomerID);
                parmArray[0].Value = customerID;

                RunSP("DN_CustomerGetDistinctAddressTypesSP", parmArray, dt);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dt;
        }

        public void DeleteAddress(SqlConnection conn, SqlTransaction trans,
                                    string customerID, string addressType,
                                    string category)
        {
            try
            {
                parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, CW.CustomerID);
                parmArray[0].Value = customerID;
                parmArray[1] = new SqlParameter("@addressType", SqlDbType.NVarChar, 3);
                parmArray[1].Value = addressType;
                parmArray[2] = new SqlParameter("@category", SqlDbType.NVarChar, 12);
                parmArray[2].Value = category;

                if (conn != null && trans != null)
                    this.RunSP(conn, trans, "DN_CustomerDeleteAddressSP", parmArray);
                else
                    this.RunSP("DN_CustomerDeleteAddressSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public DCustomer()
        {

        }

        public DataTable CustomerCodes
        {
            get
            {
                return _custCodes;
            }
        }

        public DataTable CustSearch
        {
            get
            {
                return _custSearch;
            }
        }

        public DataTable Customer
        {
            get { return _cust; }
        }

        public string CustID
        {
            get
            {
                return _custID;
            }
            set
            {
                _custID = value;
            }
        }
        public short OrigBr
        {
            get
            {
                return _origbr;
            }
            set
            {
                _origbr = value;
            }
        }

        public string OtherId
        {
            get
            {
                return _otherid;
            }
            set
            {
                _otherid = value;
            }
        }

        public short BranchNoHandle
        {
            get
            {
                return _branchnohdle;
            }
            set
            {
                _branchnohdle = value;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }


        public string FirstName
        {
            get
            {
                return _firstname;
            }
            set
            {
                _firstname = value;
            }
        }

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
            }
        }
        // new address pop add hear by tosif ali 23/10/2018*@

        public string addtype
        {
            get
            {
                return _addtype;
            }
            set
            {
                _addtype = value;
            }
        }
        public string DELLastName
        {
            get
            {
                return _DELLastName;
            }
            set
            {
                _DELLastName = value;
            }
        }


        public string DELFirstName
        {
            get
            {
                return _DELFirstName;
            }
            set
            {
                _DELFirstName = value;
            }
        }

        public string DELTitleC
        {
            get
            {
                return _DELTitleC;
            }
            set
            {
                _DELTitleC = value;
            }
        }
        //End hear
        public string Alias
        {
            get
            {
                return _alias;
            }
            set
            {
                _alias = value;
            }
        }

        public int BudgetCard
        {
            get { return _budget; }
            set { _budget = value; }
        }

        public string AddressSort
        {
            get
            {
                return _addrsort;
            }
            set
            {
                _addrsort = value;
            }
        }

        public string NameSort
        {
            get
            {
                return _namesort;
            }
            set
            {
                _namesort = value;
            }
        }

        public string DateBornStr
        {
            get
            {
                return _datebornstr;
            }
            set
            {
                _datebornstr = value;
            }
        }

        public string Sex
        {
            get
            {
                return _sex;
            }
            set
            {
                _sex = value;
            }
        }

        public string Ethnicity
        {
            get
            {
                return _ethnicity;
            }
            set
            {
                _ethnicity = value;
            }
        }

        public string MoreRewardsNo
        {
            get
            {
                return _morerewardsno;
            }
            set
            {
                _morerewardsno = value;
            }
        }

        public byte RFCardSeqNo
        {
            get
            {
                return _rfcardseqno;
            }
            set
            {
                _rfcardseqno = value;
            }
        }

        public DateTime EffectiveDate
        {
            get
            {
                return _effectivedate;
            }
            set
            {
                _effectivedate = value;
            }
        }

        public string IDType
        {
            get
            {
                return _idtype;
            }
            set
            {
                _idtype = value;
            }
        }

        public string IDNumber
        {
            get
            {
                return _idnumber;
            }
            set
            {
                _idnumber = value;
            }
        }

        public DateTime DateBorn
        {
            get
            {
                return _dateborn;
            }
            set
            {
                _dateborn = value;
            }
        }

        //		public DataSet CustAccsAndDetails
        //		{
        //			get
        //			{
        //				return _adetails; // CustomerAccounts
        //			}
        //		}

        /// <summary>
        /// UpdateLoyaltyCardNo
        /// </summary>
        /// <param name="custid">string</param>
        /// <param name="loyaltycard">string</param>
        /// <returns>void</returns>
        /// 
        public void UpdateLoyaltyCardNo(SqlConnection conn, SqlTransaction trans, string custid, string loyaltycard)
        {
            try
            {
                parmArray = new SqlParameter[2];

                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
                parmArray[0].Value = custid;

                parmArray[1] = new SqlParameter("@loyaltycard", SqlDbType.NVarChar, 16);
                parmArray[1].Value = loyaltycard;


                this.RunSP(conn, trans, "DN_CustomerUpdateLoyaltyCardNoSP", parmArray);


            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void UnblockCredit(SqlConnection conn, SqlTransaction trans, string customerID)
        {
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, CW.CustomerID);
                parmArray[0].Value = customerID;

                this.RunSP(conn, trans, "DN_CustomerUnblockCredit", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public bool IsPrivilegeMember(string customerID, out string privilegeClubCode, out string privilegeClubDesc)
        {
            bool result = false;
            privilegeClubCode = "";
            privilegeClubDesc = "";
            try
            {
                parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.VarChar, CW.CustomerID);
                parmArray[0].Value = customerID;
                parmArray[1] = new SqlParameter("@privClubCode", SqlDbType.VarChar, 4);
                parmArray[1].Value = "";
                parmArray[1].Direction = ParameterDirection.Output;
                parmArray[2] = new SqlParameter("@privClubDesc", SqlDbType.NVarChar, 64);
                parmArray[2].Value = "";
                parmArray[2].Direction = ParameterDirection.Output;

                this.RunSP("DN_CustomerIsPrivilegeClubMemberSP", parmArray);

                if (parmArray[1].Value != DBNull.Value)
                {
                    result = true;
                    privilegeClubCode = Convert.ToString(parmArray[1].Value);
                }

                if (parmArray[2].Value != DBNull.Value)
                    privilegeClubDesc = Convert.ToString(parmArray[2].Value);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;
        }

        public DataTable GetOtherCustomers(string acctNo)
        {
            DataTable dt = new DataTable();
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, CW.AccountNo);
                parmArray[0].Value = acctNo;

                RunSP("DN_CustomerGetOtherCustomers", parmArray, dt);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dt;
        }

        public int GetRFCombinedDetailsForPrint(string customerID)
        {
            try
            {
                parmArray = new SqlParameter[9];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, CW.CustomerID);
                parmArray[0].Value = customerID;
                parmArray[1] = new SqlParameter("@available_credit", SqlDbType.Money);
                parmArray[1].Value = 0;
                parmArray[2] = new SqlParameter("@Cardprinted", SqlDbType.NChar, 1);
                parmArray[2].Value = "";
                parmArray[3] = new SqlParameter("@total_agreements", SqlDbType.Money);
                parmArray[3].Value = 0;
                parmArray[4] = new SqlParameter("@total_arrears", SqlDbType.Money);
                parmArray[4].Value = 0;
                parmArray[5] = new SqlParameter("@total_balances", SqlDbType.Money);
                parmArray[5].Value = 0;
                parmArray[6] = new SqlParameter("@total_credit", SqlDbType.Money);
                parmArray[6].Value = 0;
                parmArray[7] = new SqlParameter("@total_delivered_instalments", SqlDbType.Money);
                parmArray[7].Value = 0;
                parmArray[8] = new SqlParameter("@total_all_instalments", SqlDbType.Money);
                parmArray[8].Value = 0;

                foreach (SqlParameter p in parmArray)
                    p.Direction = ParameterDirection.Output;

                parmArray[0].Direction = ParameterDirection.Input;

                result = this.RunSP("DN_CustomerGetRFCombinedForPrintSP", parmArray);

                if (result == 0)
                {
                    if (parmArray[1].Value != DBNull.Value)
                        this.AvailableCredit = (decimal)parmArray[1].Value;

                    if (parmArray[2].Value != DBNull.Value)
                        this.CardPrinted = (string)parmArray[2].Value;

                    if (parmArray[3].Value != DBNull.Value)
                        this.TotalAgreements = (decimal)parmArray[3].Value;

                    if (parmArray[4].Value != DBNull.Value)
                        this.TotalArrears = (decimal)parmArray[4].Value;

                    if (parmArray[5].Value != DBNull.Value)
                        this.TotalBalances = (decimal)parmArray[5].Value;

                    if (parmArray[6].Value != DBNull.Value)
                        this.TotalCredit = (decimal)parmArray[6].Value;

                    if (parmArray[7].Value != DBNull.Value)
                        this.TotalDeliveredInstalments = (decimal)parmArray[7].Value;

                    if (parmArray[8].Value != DBNull.Value)
                        this.TotalAllInstalments = (decimal)parmArray[8].Value;
                }
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;
        }

        public void UpdateCustomerID(SqlConnection conn, SqlTransaction trans,
                                    string newCustID, string oldCustID)
        {
            try
            {
                parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@newcustid", SqlDbType.NVarChar, 20);
                parmArray[0].Value = newCustID;
                parmArray[1] = new SqlParameter("@oldcustid", SqlDbType.NVarChar, 20);
                parmArray[1].Value = oldCustID;
                parmArray[2] = new SqlParameter("@user", SqlDbType.Int);
                parmArray[2].Value = this.User;

                this.RunSP(conn, trans, "DN_CustomerUpdateCustIDSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void DeleteCustomer(SqlConnection conn, SqlTransaction trans, string custID)
        {
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
                parmArray[0].Value = custID;

                this.RunSP(conn, trans, "DN_CustomerDeleteSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public int GetCustomerWorkAddress(string customerID)
        {
            try
            {
                parmArray = new SqlParameter[13];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, CW.CustomerID);
                parmArray[0].Value = customerID;
                parmArray[1] = new SqlParameter("@datein", SqlDbType.DateTime);
                parmArray[1].Value = this.DateIn;
                parmArray[2] = new SqlParameter("@dateout", SqlDbType.DateTime);
                parmArray[2].Value = this.DateOut;
                parmArray[3] = new SqlParameter("@resstatus", SqlDbType.NChar, CW.ResStatus);
                parmArray[3].Value = this.ResidentialStatus;
                parmArray[4] = new SqlParameter("@proptype", SqlDbType.NChar, CW.PropType);
                parmArray[4].Value = this.PropertyType;
                parmArray[5] = new SqlParameter("@address1", SqlDbType.NChar, CW.Address1);
                parmArray[5].Value = this.Address1;
                parmArray[6] = new SqlParameter("@address2", SqlDbType.NChar, CW.Address2);
                parmArray[6].Value = this.Address2;
                parmArray[7] = new SqlParameter("@address3", SqlDbType.NChar, CW.Address3);
                parmArray[7].Value = this.Address3;
                parmArray[8] = new SqlParameter("@postcode", SqlDbType.NChar, CW.PostCode);
                parmArray[8].Value = this.PostCode;
                parmArray[9] = new SqlParameter("@DeliveryArea", SqlDbType.NVarChar, CW.DeliveryArea);
                parmArray[9].Value = this.DeliveryArea;
                parmArray[10] = new SqlParameter("@mthlyrent", SqlDbType.Float);
                parmArray[10].Value = this.MonthlyRent;
                parmArray[11] = new SqlParameter("@Latitude", SqlDbType.Float); // Address Standardization CR2019 - 025
                parmArray[11].Value = this.Latitude;
                parmArray[12] = new SqlParameter("@Longitude", SqlDbType.Float); // Address Standardization CR2019 - 025
                parmArray[12].Value = this.Longitude;

                foreach (SqlParameter p in parmArray)
                    p.Direction = ParameterDirection.Output;

                parmArray[0].Direction = ParameterDirection.Input;

                result = this.RunSP("DN_CustomerAddressGetWorkSP", parmArray);
                if (result == 0)
                {
                    if (!Convert.IsDBNull(parmArray[1].Value))
                        this.DateIn = (DateTime)parmArray[1].Value;
                    if (!Convert.IsDBNull(parmArray[2].Value))
                        this.DateOut = (DateTime)parmArray[2].Value;
                    if (!Convert.IsDBNull(parmArray[3].Value))
                        this.ResidentialStatus = (string)parmArray[3].Value;
                    if (!Convert.IsDBNull(parmArray[4].Value))
                        this.PropertyType = (string)parmArray[4].Value;
                    if (!Convert.IsDBNull(parmArray[5].Value))
                        this.Address1 = (string)parmArray[5].Value;
                    if (!Convert.IsDBNull(parmArray[6].Value))
                        this.Address2 = (string)parmArray[6].Value;
                    if (!Convert.IsDBNull(parmArray[7].Value))
                        this.Address3 = (string)parmArray[7].Value;
                    if (!Convert.IsDBNull(parmArray[8].Value))
                        this.PostCode = (string)parmArray[8].Value;
                    if (!Convert.IsDBNull(parmArray[9].Value))
                        this.DeliveryArea = (string)parmArray[9].Value;
                    if (!Convert.IsDBNull(parmArray[10].Value))
                        this.MonthlyRent = parmArray[10].Value;
                    if (!Convert.IsDBNull(parmArray[11].Value))
                        this.Latitude = double.Parse(parmArray[11].Value.ToString()); // Address Standardization CR2019 - 025
                    if (!Convert.IsDBNull(parmArray[12].Value))
                        this.Longitude = double.Parse(parmArray[12].Value.ToString()); // Address Standardization CR2019 - 025

                    result = (int)Return.Success;
                }
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

        public void SetPotentialCalcDate(SqlConnection conn, SqlTransaction trans, DateTime calcDate, decimal score)
        {
            try
            {
                parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, CW.CustomerID);
                parmArray[0].Value = this.CustID;
                parmArray[1] = new SqlParameter("@calcDate", SqlDbType.DateTime);
                parmArray[1].Value = calcDate;
                parmArray[2] = new SqlParameter("@score", SqlDbType.Int);
                parmArray[2].Value = score;


                this.RunSP(conn, trans, "DN_CustomerSetPotentialCalcDateSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void SetAvailableSpend(SqlConnection conn, SqlTransaction trans,
                                        string custID, decimal available)
        {
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
                parmArray[0].Value = custID;
                parmArray[1] = new SqlParameter("@available", SqlDbType.Money, 20);
                parmArray[1].Value = available;

                this.RunSP(conn, trans, "DN_CustomerSetAvailableSpendSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public DataTable GetCustomerAudit(string customerId)
        {
            DataTable dt = new DataTable(TN.CustomerAudit);
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@piCustId", SqlDbType.VarChar, 20);
                parmArray[0].Value = customerId;

                this.RunSP("DN_CustomerAuditGetSP", parmArray, dt);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dt;
        }

        public DataTable GetAddressHistory(string customerId)
        {
            DataTable dt = new DataTable(TN.AddressHistory);
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@piCustId", SqlDbType.VarChar, 20);
                parmArray[0].Value = customerId;

                this.RunSP("DN_AddressHistoryGetSP", parmArray, dt);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dt;
        }

        public DataTable GetAddressAuditHistory(string customerId)
        {
            DataTable dt = new DataTable(TN.AddressHistory);
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@CustId", SqlDbType.VarChar, 20);
                parmArray[0].Value = customerId;

                this.RunSP("CustomerAddressAuditGet", parmArray, dt);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dt;
        }


        public DataTable GetTelephoneHistory(string customerId)
        {
            DataTable dt = new DataTable("TelHistory");
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@CustId", SqlDbType.VarChar, 20);
                parmArray[0].Value = customerId;

                this.RunSP("CustTelGet", parmArray, dt);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dt;
        }

        public DataTable GetTelephoneAuditHistory(string customerId)
        {
            DataTable dt = new DataTable("TelHistory");
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@CustId", SqlDbType.VarChar, 20);
                parmArray[0].Value = customerId;

                this.RunSP("CustTelAuditGet", parmArray, dt);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dt;
        }

        //public DataTable GetTelephoneHistory(string customerId)
        //{
        //    DataTable dt = new DataTable(TN.TelephoneHistory);
        //    try
        //    {
        //        parmArray = new SqlParameter[1];
        //        parmArray[0] = new SqlParameter("@piCustId", SqlDbType.VarChar, 20);
        //        parmArray[0].Value = customerId;

        //        this.RunSP("DN_TelephoneHistoryGetSP", parmArray, dt);
        //    }
        //    catch (SqlException ex)
        //    {
        //        LogSqlException(ex);
        //        throw ex;
        //    }
        //    return dt;
        //}
        public DataTable GetWarrantySecondEffortSolicitationItems(string customerId, int numberOfPrompts, int warrantyAfterDeliveryDays)
        {
            DataTable dt = new DataTable();
            try
            {
                parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@custId", SqlDbType.NVarChar, 20);
                parmArray[0].Value = customerId;

                parmArray[1] = new SqlParameter("@numberofPrompts", SqlDbType.Int);
                parmArray[1].Value = numberOfPrompts;

                parmArray[2] = new SqlParameter("@warrantyAfterDeliveryDays", SqlDbType.Int);
                parmArray[2].Value = warrantyAfterDeliveryDays;

                this.RunSP("DN_WarrantyGetSecondEffortSolicitationItems", parmArray, dt);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dt;
        }

        public void PCCustomerTiers(SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                this.RunSP(conn, trans, "DN_PCCustomerTiersSP");
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void PCCustomerTiersUpdate(SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                this.RunSP(conn, trans, "DN_PCCustomerTiersUpdateSP");
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public DataTable RFCardPrintList(SqlConnection conn, SqlTransaction trans, bool privilege)
        {
            DataTable customerList = new DataTable();
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@piPrivilege", SqlDbType.SmallInt);
                parmArray[0].Value = privilege;

                this.RunSP(conn, trans, "DN_RFCardPrintListSP", parmArray, customerList);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return customerList;
        }

        public void RFCardPrinted(SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                this.RunSP(conn, trans, "DN_RFCardPrintedSP");
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void RFCheckExpired(SqlConnection conn, SqlTransaction trans, int user)
        {
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@empeeno", SqlDbType.Int);
                parmArray[0].Value = user;
                this.RunSP(conn, trans, "DN_RFCheckExpiredSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void IssuePrizeVouchers(SqlConnection conn, SqlTransaction trans,
                      string acctNo, decimal cashPrice, DateTime dateIssued,
                      int buffNo, out int voucherID)
        {
            try
            {
                voucherID = 0;

                parmArray = new SqlParameter[6];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NChar, 12);
                parmArray[0].Value = acctNo;
                parmArray[1] = new SqlParameter("@dateissued", SqlDbType.DateTime);
                parmArray[1].Value = dateIssued;
                parmArray[2] = new SqlParameter("@buffno", SqlDbType.Int);
                parmArray[2].Value = buffNo;
                parmArray[3] = new SqlParameter("@subtotal", SqlDbType.Money);
                parmArray[3].Value = cashPrice;
                parmArray[4] = new SqlParameter("@issuedby", SqlDbType.Int);
                parmArray[4].Value = this.User;
                parmArray[5] = new SqlParameter("@voucherid", SqlDbType.Int);
                parmArray[5].Direction = ParameterDirection.Output;

                this.RunSP(conn, trans, "DN_CustomerIssuePrizeVouchersSP", parmArray);

                if (!Convert.IsDBNull(parmArray[5].Value))
                    voucherID = (int)parmArray[5].Value;

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void SavePrizeVouchers(SqlConnection conn, SqlTransaction trans, int voucherID)
        {
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@voucherid", SqlDbType.Int);
                parmArray[0].Value = voucherID;

                this.RunSP(conn, trans, "DN_CustomerSavePrizeVouchersSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public DataTable GetPrizeVouchers(SqlConnection conn, SqlTransaction trans, string acctNo,
                                            DateTime dateIssued, int buffNo)
        {
            DataTable dtVoucher = new DataTable();

            try
            {
                parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NChar, 12);
                parmArray[0].Value = acctNo;
                parmArray[1] = new SqlParameter("@dateissued", SqlDbType.DateTime);
                parmArray[1].Value = dateIssued;
                parmArray[2] = new SqlParameter("@buffno", SqlDbType.Int);
                parmArray[2].Value = buffNo;

                this.RunSP(conn, trans, "DN_CustomerGetPrizeVouchersSP", parmArray, dtVoucher);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            return dtVoucher;
        }

        public void GetCashPriceForPrizeVouchers(SqlConnection conn, SqlTransaction trans,
                                                 string acctNo, out decimal cashPrice)
        {
            try
            {
                cashPrice = 0;

                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NChar, 12);
                parmArray[0].Value = acctNo;
                parmArray[1] = new SqlParameter("@cashprice", SqlDbType.Money);
                parmArray[1].Direction = ParameterDirection.Output;

                this.RunSP(conn, trans, "DN_GetCashPriceForPrizeVouchersSP", parmArray);

                if (!Convert.IsDBNull(parmArray[1].Value))
                    cashPrice = Convert.ToDecimal(parmArray[1].Value);

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public DataTable GetPrizeVoucherDetails(string acctNo, string custID,
            string branchFilter, DateTime dateFrom, DateTime dateTo, int buffNo)
        {
            DataTable dtVoucher = new DataTable();

            try
            {
                parmArray = new SqlParameter[6];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[0].Value = acctNo;
                parmArray[1] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
                parmArray[1].Value = custID;
                parmArray[2] = new SqlParameter("@branchfilter", SqlDbType.NVarChar, 5);
                parmArray[2].Value = branchFilter;
                parmArray[3] = new SqlParameter("@datefrom", SqlDbType.DateTime);
                parmArray[3].Value = dateFrom;
                parmArray[4] = new SqlParameter("@dateto", SqlDbType.DateTime);
                parmArray[4].Value = dateTo;
                parmArray[5] = new SqlParameter("@buffno", SqlDbType.Int);
                parmArray[5].Value = buffNo;

                this.RunSP("DN_GetPrizeVoucherDetailsSP", parmArray, dtVoucher);

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            return dtVoucher;
        }

        public void SetPrizeVouchersPrinted(SqlConnection conn, SqlTransaction trans,
                                            string acctNo, DateTime dateIssued, int buffNo)
        {
            try
            {
                parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NChar, 12);
                parmArray[0].Value = acctNo;
                parmArray[1] = new SqlParameter("@dateissued", SqlDbType.DateTime);
                parmArray[1].Value = dateIssued;
                parmArray[2] = new SqlParameter("@buffno", SqlDbType.Int);
                parmArray[2].Value = buffNo;

                this.RunSP(conn, trans, "DN_CustomerSetVouchersPrintedSP", parmArray);

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public string GetCashLoanQualified(string custID, string acctNo)
        {
            return GetCashLoanQualified(null, null, custID, acctNo);
        }

        public string GetCashLoanQualified(SqlConnection conn, SqlTransaction trans, string custID, string acctNo)
        {
            string retVal = String.Empty;
            try
            {
                parmArray = new SqlParameter[5];
                parmArray[0] = new SqlParameter("@piCustomerID", SqlDbType.VarChar, 20);
                parmArray[0].Value = custID;
                parmArray[1] = new SqlParameter("@piAccountNo", SqlDbType.VarChar, 12);
                parmArray[1].Value = acctNo;
                parmArray[2] = new SqlParameter("@piProcess", SqlDbType.Char, 1);
                parmArray[2].Value = "L";
                parmArray[3] = new SqlParameter("@poInstantCredit", SqlDbType.Char, 1);
                parmArray[3].Direction = ParameterDirection.Output;
                parmArray[4] = new SqlParameter("@poLoanQualified", SqlDbType.Char, 1);
                parmArray[4].Direction = ParameterDirection.Output;

                if (conn == null)
                    this.RunSP("InstantCreditApprovalsCheckGen", parmArray);
                else
                    this.RunSP(conn, trans, "InstantCreditApprovalsCheckGen", parmArray);

                retVal = parmArray[4].Value.ToString();


            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return retVal;
        }

        public void VoidPrizeVouchers(SqlConnection conn, SqlTransaction trans, string acctNo, DateTime dateIssued,
                                        int buffNo, out decimal cashPrice, out int voucherID, out int numVouchers)
        {
            try
            {
                cashPrice = 0;
                voucherID = 0;
                numVouchers = 0;

                parmArray = new SqlParameter[6];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NChar, 12);
                parmArray[0].Value = acctNo;
                parmArray[1] = new SqlParameter("@dateissued", SqlDbType.DateTime);
                parmArray[1].Value = dateIssued;
                parmArray[2] = new SqlParameter("@buffno", SqlDbType.Int);
                parmArray[2].Value = buffNo;
                parmArray[3] = new SqlParameter("@cashprice", SqlDbType.Money);
                parmArray[3].Direction = ParameterDirection.Output;
                parmArray[4] = new SqlParameter("@voucherID", SqlDbType.Int);
                parmArray[4].Direction = ParameterDirection.Output;
                parmArray[5] = new SqlParameter("@numvouchers", SqlDbType.Int);
                parmArray[5].Direction = ParameterDirection.Output;

                this.RunSP(conn, trans, "DN_CustomerVoidPrizeVouchersSP", parmArray);

                if (!Convert.IsDBNull(parmArray[3].Value))
                    cashPrice = Convert.ToDecimal(parmArray[3].Value);
                if (!Convert.IsDBNull(parmArray[4].Value))
                    voucherID = (int)(parmArray[4].Value);
                if (!Convert.IsDBNull(parmArray[5].Value))
                    numVouchers = (int)(parmArray[5].Value);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void DeletePrizeVouchers(SqlConnection conn, SqlTransaction trans, DateTime endDate,
                                        string acctNo, bool isCancellation)
        {
            try
            {
                parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@enddate", SqlDbType.DateTime);
                parmArray[0].Value = endDate;
                parmArray[1] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[1].Value = acctNo;
                parmArray[2] = new SqlParameter("@iscancellation", SqlDbType.Bit);
                parmArray[2].Value = isCancellation;

                this.RunSP(conn, trans, "[DN_CustomerDeletePrizeVouchersSP]", parmArray);

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public bool SaveCustomerPhoto(SqlConnection conn, SqlTransaction trans, string custID, string fileName, int takenBy)
        {
            bool fileExists = true;
            try
            {

                parmArray = new SqlParameter[4];
                parmArray[0] = new SqlParameter("@customerID", SqlDbType.VarChar, 20);
                parmArray[0].Value = custID;
                parmArray[1] = new SqlParameter("@filename", SqlDbType.NVarChar, 100);
                parmArray[1].Value = fileName;
                parmArray[2] = new SqlParameter("@takenby", SqlDbType.Int);
                parmArray[2].Value = takenBy;
                parmArray[3] = new SqlParameter("@fileExists", SqlDbType.Bit);
                parmArray[3].Value = false;
                parmArray[3].Direction = ParameterDirection.Output;

                this.RunSP(conn, trans, "[SaveCustomerPhotoSP]", parmArray);

                if (parmArray[3].Value != DBNull.Value)
                {
                    fileExists = (bool)parmArray[3].Value;
                }
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return fileExists;
        }

        public bool SaveCustomerSignature(SqlConnection conn, SqlTransaction trans, string custID, string fileName)
        {
            bool fileExists = true;
            try
            {

                parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@customerID", SqlDbType.VarChar, 20);
                parmArray[0].Value = custID;
                parmArray[1] = new SqlParameter("@filename", SqlDbType.NVarChar, 100);
                parmArray[1].Value = fileName;
                parmArray[2] = new SqlParameter("@fileExists", SqlDbType.Bit);
                parmArray[2].Value = false;
                parmArray[2].Direction = ParameterDirection.Output;

                this.RunSP(conn, trans, "[SaveCustomerSignatureSP]", parmArray);

                if (parmArray[2].Value != DBNull.Value)
                {
                    fileExists = (bool)parmArray[2].Value;
                }
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return fileExists;
        }

        public string GetCustomerPhoto(SqlConnection conn, SqlTransaction trans, string custID)
        {
            string fileName = String.Empty;
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@customerID", SqlDbType.VarChar, 20);
                parmArray[0].Value = custID;
                parmArray[1] = new SqlParameter("@fileName", SqlDbType.NVarChar, 100);
                parmArray[1].Value = String.Empty;
                parmArray[1].Direction = ParameterDirection.Output;

                this.RunSP(conn, trans, "[GetCustomerPhotoSP]", parmArray);

                if (parmArray[1].Value != DBNull.Value)
                {
                    fileName = (string)parmArray[1].Value;
                }
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return fileName;
        }

        public DataTable GetAllCustomerPhotos(SqlConnection conn, SqlTransaction trans, string custID)
        {
            DataTable dtPhotos = new DataTable();

            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@customerID", SqlDbType.VarChar, 20);
                parmArray[0].Value = custID;

                this.RunSP("GetAllCustomerPhotosSP", parmArray, dtPhotos);

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            return dtPhotos;
        }

        public string GetCustomerSignature(SqlConnection conn, SqlTransaction trans, string custID)
        {
            string fileName = String.Empty;
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@customerID", SqlDbType.VarChar, 20);
                parmArray[0].Value = custID;
                parmArray[1] = new SqlParameter("@fileName", SqlDbType.NVarChar, 100);
                parmArray[1].Value = String.Empty;
                parmArray[1].Direction = ParameterDirection.Output;

                this.RunSP(conn, trans, "[GetCustomerSignatureSP]", parmArray);

                if (parmArray[1].Value != DBNull.Value)
                {
                    fileName = (string)parmArray[1].Value;
                }
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return fileName;
        }

        //IP - 18/11/08 -  UAT5.1 - UAT(580) - Method that will generate a new Customer ID for a
        //Cash & Go Service Request and populate the Customer ID field on the Service Request screen.
        public string GenerateSRCashAndGoCustid(SqlConnection conn, SqlTransaction trans, int branchNo)
        {
            string SRCashAndGoCustid = String.Empty;

            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@branch", SqlDbType.Int);
                parmArray[0].Value = branchNo;
                parmArray[1] = new SqlParameter("@NextSRCashAndGoCustid", SqlDbType.NVarChar, 17);
                parmArray[1].Value = String.Empty;
                parmArray[1].Direction = ParameterDirection.Output;

                this.RunSP(conn, trans, "GenerateCashAndGoSRCustidSP", parmArray);

                if (parmArray[1].Value != DBNull.Value)
                {
                    SRCashAndGoCustid = Convert.ToString(parmArray[1].Value);
                }

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return SRCashAndGoCustid;
        }

        //IP - 08/02/10 - CR1037 Merged - Malaysia Enhancements (CR1072)
        public DataTable GetCustomerID(string accountNo) //CR 1037 
        {
            DataTable dt = new DataTable("CustomerID");
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@AcctNo", SqlDbType.Char, 12);
                parmArray[0].Value = accountNo;

                RunSP("DN_GetCustID", parmArray, dt);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dt;
        }

        public string CustomerGetBand(string acctno)
        {
            string band = String.Empty;
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar, 20);
                parmArray[0].Value = acctno;

                band = this.ReturnString("CustomerGetBand", parmArray);

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return band;
        }

        public void CustomerSaveBand(string custid, char band)
        {
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.VarChar, 20);
                parmArray[0].Value = custid;
                parmArray[1] = new SqlParameter("@band", SqlDbType.Char, 1);
                parmArray[1].Value = band;

                RunSP("CustomerSaveBand", parmArray);

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

        }

        public string CustomerGetIdByAcctno(string acctno)
        {
            parmArray = new SqlParameter[1];
            parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar, 12);
            parmArray[0].Value = acctno;

            return ReturnString("CustomerGetIdByAcctno", parmArray);
        }


        public void CustomerSaveBand(SqlConnection conn, SqlTransaction trans, string custid, char band)
        {
            try
            {

                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.VarChar, 20);
                parmArray[0].Value = custid;
                parmArray[1] = new SqlParameter("@band", SqlDbType.Char, 1);
                parmArray[1].Value = band;

                RunSP(conn, trans, "CustomerSaveBand", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }


        }

        //ScorecardAccountGet

        public DataSet ScorecardAccountGet(SqlConnection conn, SqlTransaction trans, string custid)
        {

            DataSet ds = new DataSet();
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.VarChar, 20);
                parmArray[0].Value = custid;


                RunSP(conn, trans, "ScorecardAccountGet", parmArray, ds);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            ds.Tables[0].TableName = "ScoreCardAccount";

            return ds;
        }

        //IP - 21/12/10 - Store Card - Method to update the Customers Store Card Limit
        public void CustomerUpdateStoreCardLimit(SqlConnection conn, SqlTransaction trans, string custID)
        {
            try
            {

                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@custID", SqlDbType.VarChar, 20);
                parmArray[0].Value = custID;
                parmArray[1] = new SqlParameter("@storeCardlimit", SqlDbType.Money);
                parmArray[1].Value = this.StoreCardLimit;

                RunSP(conn, trans, "CustomerUpdateStoreCardLimit", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        //IP - 21/12/10 - Store Card - Method to calculate/update and retrieve the Store Card Available for a customer
        public void CustomerUpdateAndGetStoreCardAvailable(SqlConnection conn, SqlTransaction trans, ref decimal storeCardLimit, ref decimal storeCardAvailable, string custID)
        {
            try
            {
                storeCardLimit = 0;
                storeCardAvailable = 0;

                parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@custID", SqlDbType.VarChar, 20);
                parmArray[0].Value = custID;
                parmArray[1] = new SqlParameter("@storeCardLimit", SqlDbType.Money);
                parmArray[1].Value = 0;
                parmArray[1].Direction = ParameterDirection.Output;
                parmArray[2] = new SqlParameter("@storeCardAvailable", SqlDbType.Money);
                parmArray[2].Value = 0;
                parmArray[2].Direction = ParameterDirection.Output;

                RunSP(conn, trans, "CustomerUpdateStoreCardAvailable", parmArray);

                if (parmArray[1].Value != DBNull.Value)
                    storeCardLimit = Convert.ToDecimal(parmArray[1].Value);
                if (parmArray[2].Value != DBNull.Value)
                    storeCardAvailable = Convert.ToDecimal(parmArray[2].Value);

                this.CountryRound(ref storeCardLimit);
                this.CountryRound(ref storeCardAvailable);
                this._storeCardLimit = storeCardLimit;
                this._storeCardAvailable = storeCardAvailable;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }


        }

        //Get early settlement figure for cash loan accounts
        public void GetEarlySettlementFig(string accountNumber, out decimal settlementFig)
        {
            settlementFig = 0;

            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[0].Value = accountNumber;
                parmArray[1] = new SqlParameter("@settlementFig", SqlDbType.Decimal);
                parmArray[1].Direction = ParameterDirection.Output;
                parmArray[1].Scale = 2;

                this.RunSP("DN_GetEarlySettlementFigure", parmArray);
                if (parmArray[1].Value != DBNull.Value)
                    settlementFig = Convert.ToDecimal(parmArray[1].Value);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        //BOC Added by Suvidha - CR 2018-13 - 21/12/18 - to get the InvoiceDetails.
        public string GenerateAgreementInvNo(string branch_no)//SqlConnection conn, SqlTransaction trans, 
        {
            string agr_inv_no = "";

            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@BranchNumber", SqlDbType.VarChar);
                parmArray[0].Value = branch_no;
                parmArray[1] = new SqlParameter("@InvoiceNumber", SqlDbType.VarChar, 14);
                parmArray[1].Value = String.Empty;
                parmArray[1].Direction = ParameterDirection.Output;

                this.RunSP("GenerateInvoiceNumber", parmArray);
                if (parmArray[1].Value != DBNull.Value)
                    agr_inv_no = Convert.ToString(parmArray[1].Value);
                return agr_inv_no;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            //return n;

        }

        //BOC Added by Suvidha - CR 2018-13 - 21/12/18 - to get the InvoiceDetails.
        public DataTable GetInvoiceDetails(SqlConnection conn, SqlTransaction trans, string acctNo, string agrmtno, string AgreementInvoiceNumber)
        {
            try
            {
                DataTable dtInvoiceDetails = new DataTable("InvoiceDetails");
                try
                {
                    parmArray = new SqlParameter[3];
                    parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar, 15);
                    parmArray[0].Value = acctNo;

                    parmArray[1] = new SqlParameter("@agrmtno", SqlDbType.NVarChar, 10);
                    parmArray[1].Value = agrmtno;

                    parmArray[2] = new SqlParameter("@AgreementInvoiceNumber", SqlDbType.NVarChar, 15);
                    parmArray[2].Value = AgreementInvoiceNumber;

                    this.RunSP("DN_GetInvoiceDetails", parmArray, dtInvoiceDetails);
                }
                catch (SqlException ex)
                {
                    LogSqlException(ex);
                    throw ex;
                }
                return dtInvoiceDetails;

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            //return result;
        }

        //BOC Added by Suvidha - CR 2018-13 - 21/12/18 - to get the InvoiceDetails.
        public DataTable GetInvoicePaymentDetails(SqlConnection conn, SqlTransaction trans, string acctNo, string agrmtno, string AgreementInvoiceNumber)
        {
            try
            {
                DataTable dtInvoicePaymentDetails = new DataTable("InvoicePaymentDetails");
                try
                {
                    parmArray = new SqlParameter[3];
                    parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar, 15);
                    parmArray[0].Value = acctNo;

                    parmArray[1] = new SqlParameter("@agrmtno", SqlDbType.NVarChar, 10);
                    parmArray[1].Value = agrmtno;

                    parmArray[2] = new SqlParameter("@AgreementInvoiceNumber", SqlDbType.NVarChar, 15);
                    parmArray[2].Value = AgreementInvoiceNumber;

                    this.RunSP("DN_GetInvoicePaymentDetails", parmArray, dtInvoicePaymentDetails);
                }
                catch (SqlException ex)
                {
                    LogSqlException(ex);
                    throw ex;
                }
                return dtInvoicePaymentDetails;

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            //return result;
        }

        // CR - MMI (Maximum Monthly Instalment)
        // This method is used to get MMI limit of customer.
        public decimal? GetCustomerMmiLimit(string custId)
        {
            parmArray = new SqlParameter[1];
            parmArray[0] = new SqlParameter("@CustId", SqlDbType.NVarChar, 12);
            parmArray[0].Value = custId;

            return ReturnDecimal("GetCustomerMmiLimit", parmArray);
        }
        public DataTable GetMambuAccountsForPayment(string customerID, string accountID)

        {
            DataTable cust = null;
            //SqlConnection conn = new SqlConnection();
            //conn.Open();
            //using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))


            using (var conn = new SqlConnection(Connections.Default))
            {
               
                try
                {
                    conn.Open();    
                   
                    cust = new DataTable(TN.Accounts);
                    parmArray = new SqlParameter[2];
                    parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, CW.CustomerID);
                    parmArray[0].Value = customerID;
                    parmArray[1] = new SqlParameter("@AcctNo", SqlDbType.NVarChar, CW.AccountNo);
                    if (string.IsNullOrEmpty(accountID))
                        accountID = "";
                    parmArray[1].Value = accountID;


                    this.RunSP("SP_BS_GetLoanAccountNo", parmArray, cust);//need to use new SP
                }
                catch (SqlException ex)
                {
                    LogSqlException(ex);
                    throw ex;
                }
            }
                
           

           

               
            return cust;
        }

        public void GetEarlySettlementFigForMambu(string accountNumber, out decimal settlementFig)
        {
            settlementFig = 0; try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@Cosacsacctno", SqlDbType.NVarChar, 12);
                parmArray[0].Value = accountNumber;
                parmArray[1] = new SqlParameter("@settlementFigure", SqlDbType.Money);
                parmArray[1].Direction = ParameterDirection.Output;
                parmArray[1].Scale = 2; this.RunSP("SP_BS_GetSettlementFigureFromMambo", parmArray);
                if (parmArray[1].Value != DBNull.Value)
                    settlementFig = Convert.ToDecimal(parmArray[1].Value);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

 public DataTable GetMambuAccountsSettled(string customerID, string accountID)

        {
            DataTable cust = null;
            //SqlConnection conn = new SqlConnection();
            //conn.Open();
            //using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))


            using (var conn = new SqlConnection(Connections.Default))
            {

                try
                {
                    conn.Open();

                    cust = new DataTable(TN.Accounts);
                    parmArray = new SqlParameter[2];
                    parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, CW.CustomerID);
                    parmArray[0].Value = customerID;
                    parmArray[1] = new SqlParameter("@AcctNo", SqlDbType.NVarChar, CW.AccountNo);
                    if (string.IsNullOrEmpty(accountID))
                        accountID = "";
                    parmArray[1].Value = accountID;


                    this.RunSP("SP_BS_CheckSettledMambuAccount", parmArray, cust);//need to use new SP
                }
                catch (SqlException ex)
                {
                    LogSqlException(ex);
                    throw ex;
                }
            }






            return cust;
        }


    }
}
