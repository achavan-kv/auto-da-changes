using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common.Constants.Enums;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.Tags;
using System.Xml;


namespace STL.DAL
{
    /// <summary>
    /// Summary description for DProposal.
    /// </summary>
    public class DProposal : DALObject
    {
        private string _s1 = "";
        private string _s2 = "";
        private string _dc = "";
        private string _ad = "";
        private string _uw = "";
        private string _appstatus = "";
        private string _manualrefer = "";
        private string _adreqd = "";

        private string _custid = "";
        private DateTime _dateprop = DateTime.MinValue.AddYears(1899);
        private string _maritalstatus = "";
        private bool _IsSpouseWorking = false;
        private int _dependants = 0;
        private int _sanctserno = 0;
        private string _nationality = "";
        private double _yrscuremplmt = 0;
        private object _mthlyincome = DBNull.Value;
        private object _otherpmnts = DBNull.Value;
        private int _pempyy = 0;
        private int _pempmm = 0;
        private object _addincome = DBNull.Value;
        private string _location = "";
        private string _ccardno1 = "";
        private string _ccardno2 = "";
        private string _ccardno3 = "";
        private string _ccardno4 = "";
        private object _commitments1 = DBNull.Value;
        private object _commitments2 = DBNull.Value;
        private object _commitments3 = DBNull.Value;
        private string _empaddr1 = "";
        private string _empaddr2 = "";
        private string _empcity = "";
        private string _emppostcode = "";
        private string _empdept = "";
        private string _empname = "";
        private int _noofref = 0;
        private string _proofaddress = "";
        private string _proofid = "";
        private string _proofincome = "";
        private string _proofOfBank= "";                    //IP - 14/12/10 - Store Card
        private string _s1comment = "";
        private string _s2comment = "";
        private string _specialpromo = "";
        private object _a2mthlyincome = DBNull.Value;
        private object _a2addincome = DBNull.Value;
        private string _a2maritalstat = "";
        private string _paddress1 = "";
        private string _paddress2 = "";
        private string _pcity = "";
        private string _ppostcode = "";
        private int _paddyy = 0;
        private int _paddmm = 0;
        private string _presstatus = "";
        private string _bankaccttype = "";
        private bool _propexists = false;
        private string _a2custid = "";
        private string _a2relation = "";
        private short _rfCat = 0;
        private string _acctno = "";

        private string _employmentstatus = "";
        private string _occupation = "";
        private string _payfrequency = "";
        private DateTime _dateempstart;
        private DateTime _datepempstart;
        private string _employmenttelno = "";
        private string _employmentdialcode = "";
        private string _bankcode = "";
        private string _bankaccountno = "";
        private DateTime _bankaccountopened;
        private float _creditawarded = 0;
        //CR 835 Additional fields for customer additional details [Peter Chong] 13-Oct-2006
        private int _duedayid = 0;
        private string _bankaccountname = "";
        private string _paymentmethod = "";
        private DateTime datein  = new DateTime(1900,1,1);
        private string residentialstatus;
        private string propertytype;
        private bool allowReopenS1 = false;        //#10477
        private bool purchaseCashLoan = false;

        public DateTime DateIn
        {
            get { return datein; }
            set { datein = value; }
        }

        public string ResidentialStatus
        {
            get { return residentialstatus; }
            set { residentialstatus = value; }
        }

        public string PropertyType
        {
            get { return propertytype; }
            set { propertytype = value; }
        }

        //#10477
        public bool AllowReopenS1
        {
            get {return allowReopenS1;}
            set{allowReopenS1 = value;}
        }

        public bool PurchaseCashLoan
        {
            get { return purchaseCashLoan; }
            set { purchaseCashLoan = value; }
        }

        #region member variable properties
        public float creditawarded
        {
            get { return this._creditawarded; }
            set { this._creditawarded = value; }
        }

        public int DueDayId
        {
            get { return this._duedayid; }
            set { this._duedayid = value; }
        }

        public string BankAccountName
        {
            get { return this._bankaccountname; }
            set { this._bankaccountname = value; }
        }

        public string PaymentMethod
        {
            get { return this._paymentmethod; }
            set { this._paymentmethod = value; }
        }

        public string EmploymentStatus
        {
            get { return _employmentstatus; }
            set { _employmentstatus = value; }
        }
        public string Occupation
        {
            get { return _occupation; }
            set { _occupation = value; }
        }
        public string PayFrequency
        {
            get { return _payfrequency; }
            set { _payfrequency = value; }
        }
        public DateTime DateEmpStart
        {
            get { return _dateempstart; }
            set { _dateempstart = value; }
        }
        public DateTime DatePEmpStart
        {
            get { return _datepempstart; }
            set { _datepempstart = value; }
        }
        public string EmploymentTelNo
        {
            get { return _employmenttelno; }
            set { _employmenttelno = value; }
        }
        public string EmploymentDialCode
        {
            get { return _employmentdialcode; }
            set { _employmentdialcode = value; }
        }
        public string BankCode
        {
            get { return _bankcode; }
            set { _bankcode = value; }
        }
        public string BankAccountNo
        {
            get { return _bankaccountno; }
            set { _bankaccountno = value; }
        }
        public DateTime BankAccountOpened
        {
            get { return _bankaccountopened; }
            set { _bankaccountopened = value; }
        }

        public string AccountNo
        {
            get { return _acctno; }
            set { _acctno = value; }
        }
        public short RFCategory
        {
            get { return _rfCat; }
            set { _rfCat = value; }
        }
        public string A2Relation
        {
            get { return _a2relation; }
            set { _a2relation = value; }
        }
        public string A2CustomerID
        {
            get { return _a2custid; }
            set { _a2custid = value; }
        }
        public bool ProposalExists
        {
            get { return _propexists; }
            set { _propexists = value; }
        }
        public string BankAccountType
        {
            get { return _bankaccttype; }
            set { _bankaccttype = value; }
        }
        public string S1
        {
            get
            {
                return _s1;
            }
            set
            {
                _s1 = value;
            }
        }

        public string S2
        {
            get
            {
                return _s2;
            }
            set
            {
                _s2 = value;
            }
        }

        public string DC
        {
            get
            {
                return _dc;
            }
            set
            {
                _dc = value;
            }
        }

        public string AD
        {
            get
            {
                return _ad;
            }
            set
            {
                _ad = value;
            }
        }

        public string UW
        {
            get
            {
                return _uw;
            }
            set
            {
                _uw = value;
            }
        }

        public string AppStatus
        {
            get
            {
                return _appstatus;
            }
            set
            {
                _appstatus = value;
            }
        }

        public string ManualRefer
        {
            get
            {
                return _manualrefer;
            }
            set
            {
                _manualrefer = value;
            }
        }

        public string ADReqd
        {
            get
            {
                return _adreqd;
            }
            set
            {
                _adreqd = value;
            }
        }

        public string CustomerID
        {
            get
            {
                return _custid;
            }
            set
            {
                _custid = value;
            }
        }

        public DateTime DateProp
        {
            get
            {
                return _dateprop;
            }
            set
            {
                _dateprop = value;
            }
        }

        public bool IsSpouseWorking
        {
            get { return _IsSpouseWorking; }
            set { _IsSpouseWorking = value; }
        }

        public string MaritalStatus
        {
            get
            {
                return _maritalstatus;
            }
            set
            {
                _maritalstatus = value;
            }
        }

        public int Dependants
        {
            get
            {
                return _dependants;
            }
            set
            {
                _dependants = value;
            }
        }

        public int SanctionSerialNumber
        {
            get
            {
                return _sanctserno;
            }
            set
            {
                _sanctserno = value;
            }
        }

        public string Nationality
        {
            get
            {
                return _nationality;
            }
            set
            {
                _nationality = value;
            }
        }

        public double YearsCurrentEmployment
        {
            get
            {
                return _yrscuremplmt;
            }
            set
            {
                _yrscuremplmt = value;
            }
        }

        public object MonthlyIncome
        {
            get
            {
                return _mthlyincome;
            }
            set
            {
                _mthlyincome = value;
            }
        }

        public object OtherPayments
        {
            get
            {
                return _otherpmnts;
            }
            set
            {
                _otherpmnts = value;
            }
        }

        public int PreviousEmploymentYY
        {
            get
            {
                return _pempyy;
            }
            set
            {
                _pempyy = value;
            }
        }

        public int PreviousEmploymentMM
        {
            get
            {
                return _pempmm;
            }
            set
            {
                _pempmm = value;
            }
        }

        public object AdditionalIncome
        {
            get
            {
                return _addincome;
            }
            set
            {
                _addincome = value;
            }
        }

        public string Location
        {
            get
            {
                return _location;
            }
            set
            {
                _location = value;
            }
        }

        public string CreditCardNo1
        {
            get
            {
                return _ccardno1;
            }
            set
            {
                _ccardno1 = value;
            }
        }

        public string CreditCardNo2
        {
            get
            {
                return _ccardno2;
            }
            set
            {
                _ccardno2 = value;
            }
        }

        public string CreditCardNo3
        {
            get
            {
                return _ccardno3;
            }
            set
            {
                _ccardno3 = value;
            }
        }

        public string CreditCardNo4
        {
            get
            {
                return _ccardno4;
            }
            set
            {
                _ccardno4 = value;
            }
        }

        public object Commitments1
        {
            get
            {
                return _commitments1;
            }
            set
            {
                _commitments1 = value;
            }
        }

        public object Commitments2
        {
            get
            {
                return _commitments2;
            }
            set
            {
                _commitments2 = value;
            }
        }

        public object Commitments3
        {
            get
            {
                return _commitments3;
            }
            set
            {
                _commitments3 = value;
            }
        }

        public string EmployerAddress1
        {
            get
            {
                return _empaddr1;
            }
            set
            {
                _empaddr1 = value;
            }
        }

        public string EmployerAddress2
        {
            get
            {
                return _empaddr2;
            }
            set
            {
                _empaddr2 = value;
            }
        }

        public string EmployerCity
        {
            get
            {
                return _empcity;
            }
            set
            {
                _empcity = value;
            }
        }

        public string EmployerPostCode
        {
            get
            {
                return _emppostcode;
            }
            set
            {
                _emppostcode = value;
            }
        }

        public string EmployerDept
        {
            get
            {
                return _empdept;
            }
            set
            {
                _empdept = value;
            }
        }

        public string EmployerName
        {
            get
            {
                return _empname;
            }
            set
            {
                _empname = value;
            }
        }

        public int NoOfReferences
        {
            get
            {
                return _noofref;
            }
            set
            {
                _noofref = value;
            }
        }

        public string ProofOfAddress
        {
            get
            {
                return _proofaddress;
            }
            set
            {
                _proofaddress = value;
            }
        }

        public string ProofOfID
        {
            get
            {
                return _proofid;
            }
            set
            {
                _proofid = value;
            }
        }

        public string ProofOfIncome
        {
            get
            {
                return _proofincome;
            }
            set
            {
                _proofincome = value;
            }
        }

        //IP - 14/12/10 - Store Card
        public string ProofOfBank
        {
            get
            {
                return _proofOfBank;
            }
            set
            {
                _proofOfBank = value;
            }
        }

        public string S1Comment
        {
            get
            {
                return _s1comment;
            }
            set
            {
                _s1comment = value;
            }
        }

        public string S2Comment
        {
            get
            {
                return _s2comment;
            }
            set
            {
                _s2comment = value;
            }
        }

        public string SpecialPromo
        {
            get
            {
                return _specialpromo;
            }
            set
            {
                _specialpromo = value;
            }
        }

        public object A2MonthlyIncome
        {
            get
            {
                return _a2mthlyincome;
            }
            set
            {
                _a2mthlyincome = value;
            }
        }

        public object A2AdditionalIncome
        {
            get
            {
                return _a2addincome;
            }
            set
            {
                _a2addincome = value;
            }
        }

        private object _addexp1;
        public object AdditionalExpenditure1
        {
            get
            {
                return _addexp1;
            }
            set
            {
                _addexp1 = value;
            }
        }

        private object _addexp2;
        public object AdditionalExpenditure2
        {
            get
            {
                return _addexp2;
            }
            set
            {
                _addexp2 = value;
            }
        }

        public string A2MaritalStatus
        {
            get
            {
                return _a2maritalstat;
            }
            set
            {
                _a2maritalstat = value;
            }
        }

        public string PreviousAddress1
        {
            get
            {
                return _paddress1;
            }
            set
            {
                _paddress1 = value;
            }
        }

        public string PreviousAddress2
        {
            get
            {
                return _paddress2;
            }
            set
            {
                _paddress2 = value;
            }
        }

        public string PreviousCity
        {
            get
            {
                return _pcity;
            }
            set
            {
                _pcity = value;
            }
        }

        public string PreviousPostCode
        {
            get
            {
                return _ppostcode;
            }
            set
            {
                _ppostcode = value;
            }
        }

        public int PreviousAddressYY
        {
            get
            {
                return _paddyy;
            }
            set
            {
                _paddyy = value;
            }
        }

        public int PreviousAddressMM
        {
            get
            {
                return _paddmm;
            }
            set
            {
                _paddmm = value;
            }
        }

        public string PreviousResidentialStatus
        {
            get
            {
                return _presstatus;
            }
            set
            {
                _presstatus = value;
            }
        }

        private DataTable _types = null;
        public DataTable ApplicationTypes
        {
            get { return _types; }
        }
        private short _origbr = 0;
        public short OrigBr
        {
            get { return _origbr; }
            set { _origbr = value; }
        }
        private short _origbranchno = 0;
        public short OrigBranchNo
        {
            get { return _origbranchno; }
            set { _origbranchno = value; }
        }
        private int _empeenoprop = 0;
        public int EmployeeNoProp
        {
            get { return _empeenoprop; }
            set { _empeenoprop = value; }
        }
        private int _jobslastyear = 0;
        public int JobsLastYear
        {
            get { return _jobslastyear; }
            set { _jobslastyear = value; }
        }
        private string _health = "";
        public string Health
        {
            get { return _health; }
            set { _health = value; }
        }
        private short _scorecardno = 0;
        public short ScorecardNo
        {
            get { return _scorecardno; }
            set { _scorecardno = value; }
        }
        private short? _points;
        public short? Points
        {
            get { return _points; }
            set { _points = value; }
        }
        private string _propresult = "";
        public string PropResult
        {
            get { return _propresult; }
            set { _propresult = value; }
        }
        private string _scoringBand = "";
        public string ScoringBand
        {
            get { return _scoringBand; }
            set { _scoringBand = value; }
        }
        private string _reason1 = "";
        public string Reason
        {
            get { return _reason1; }
            set { _reason1 = value; }
        }

        private string _reason2 = "";
        public string Reason2
        {
            get { return _reason2; }
            set { _reason2 = value; }
        }

        private string _reason3 = "";
        public string Reason3
        {
            get { return _reason3; }
            set { _reason3 = value; }
        }

        private string _reason4 = "";
        public string Reason4
        {
            get { return _reason4; }
            set { _reason4 = value; }
        }

        private string _reason5 = "";
        public string Reason5
        {
            get { return _reason5; }
            set { _reason5 = value; }
        }

        private string _reason6 = "";
        public string Reason6
        {
            get { return _reason6; }
            set { _reason6 = value; }
        }

        private short _yrscurraddress = 0;
        public short YearsCurrentAddress
        {
            get { return _yrscurraddress; }
            set { _yrscurraddress = value; }
        }
        private short _yrsprevaddress = 0;
        public short YearsPreviousAddress
        {
            get { return _yrsprevaddress; }
            set { _yrsprevaddress = value; }
        }
        private short _yrsbankheld = 0;
        public short YearsBankAccountHeld
        {
            get { return _yrsbankheld; }
            set { _yrsbankheld = value; }
        }
        private string _propnotes = "";
        public string ProposalNotes
        {
            get { return _propnotes; }
            set { _propnotes = value; }
        }
        private short _hasstring = 0;
        public short HasString
        {
            get { return _hasstring; }
            set { _hasstring = value; }
        }
        private string _notes = "";
        public string Notes
        {
            get { return _notes; }
            set { _notes = value; }
        }
        private string _transactid = "";
        public string TransactIdNo
        {
            get { return _transactid; }
            set { _transactid = value; }
        }
        private int _empeenochange = 0;
        public int EmployeeNoChanged
        {
            get { return _empeenochange; }
            set { _empeenochange = value; }
        }
        private DateTime _datechanged = DateTime.Now;
        public DateTime DateChanged
        {
            get { return _datechanged; }
            set { _datechanged = value; }
        }
        private string _dctext1 = "";
        public string DCText1
        {
            get { return _dctext1; }
            set { _dctext1 = value; }
        }
        private string _dctext2 = "";
        public string DCText2
        {
            get { return _dctext2; }
            set { _dctext2 = value; }
        }
        private string _dctext3 = "";
        public string DCText3
        {
            get { return _dctext3; }
            set { _dctext3 = value; }
        }

        //IP - 14/12/10 - Store Card
        private string _proofOfBankTxt = "";
        public string ProofOfBankTxt
        {
            get { return _proofOfBankTxt; }
            set { _proofOfBankTxt = value; }
        }

        private string _vehicleregistration = "";
        public string VehicleRegistration
        {
            get { return _vehicleregistration; }
            set { _vehicleregistration = value; }
        }

        private string _systemrecommendation = "";
        public string SystemRecommendation
        {
            get { return _systemrecommendation; }
            set { _systemrecommendation = value; }
        }
        //CR 866 Added Job title
        private string _jobtitle = "";
        public string JobTitle
        {
            get { return _jobtitle; }
            set { _jobtitle = value; }
        }

        //CR 866 Added Industry
        private string _industry = "";
        public string Industry
        {
            get { return _industry; }
            set { _industry = value; }
        }

        //CR 866 Added organisation
        private string _organisation = "";
        public string Organisation
        {
            get { return _organisation; }
            set { _organisation = value; }
        }

        //CR 866 Added transportType 
        private string _transportType = "";
        public string TransportType
        {
            get { return _transportType; }
            set { _transportType = value; }
        }

        //CR 866 Added EducationLevel
        private string _educationLevel = "";
        public string EducationLevel
        {
            get { return _educationLevel; }
            set { _educationLevel = value; }
        }

        //CR 866 Added DistanceFromStore
        private Int16 _distanceFromStore = 0;
        public Int16 DistanceFromStore
        {
            get { return _distanceFromStore; }
            set { _distanceFromStore = value; }
        }
        #endregion

        public int GetApplicationTypes(XmlNode parms, string name)
        {
            try
            {
                _types = new DataTable(name);
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
                parmArray[0].Value = parms.FirstChild.Attributes[Tags.Value].Value;
                parmArray[1] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[1].Value = parms.FirstChild.NextSibling.Attributes[Tags.Value].Value;
                result = this.RunSP("DN_GetApplicationTypesSP", parmArray, _types);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;
        }

        public DataTable GetStage1Row(string name)
        {
            DataTable dt = new DataTable(name);
            //CR 866 Added additional columns for new proposal fields
            dt.Columns.AddRange(new DataColumn[] { new DataColumn(CN.DateProp, Type.GetType("System.DateTime")),
													 new DataColumn(CN.ApplicationStatus),
													 new DataColumn(CN.ApplicationType),
													 new DataColumn(CN.S1Comment),
													 new DataColumn(CN.NewS1Comment),
													 new DataColumn(CN.MaritalStatus),
													 new DataColumn(CN.Dependants, Type.GetType("System.Int32")),
													 new DataColumn(CN.Nationality),
													 new DataColumn(CN.PrevEmpYY, Type.GetType("System.Int32")),
													 new DataColumn(CN.PrevEmpMM, Type.GetType("System.Int32")),
													 new DataColumn(CN.AdditionalIncome, Type.GetType("System.Decimal")),
													 new DataColumn(CN.AdditionalIncome2, Type.GetType("System.Decimal")),
													 new DataColumn(CN.OtherPayments, Type.GetType("System.Double")),
													 new DataColumn(CN.CCardNo1),
													 new DataColumn(CN.CCardNo2),
													 new DataColumn(CN.CCardNo3),
													 new DataColumn(CN.CCardNo4),
													 new DataColumn(CN.Commitments1, Type.GetType("System.Decimal")),
													 new DataColumn(CN.Commitments2, Type.GetType("System.Decimal")),
													 new DataColumn(CN.Commitments3, Type.GetType("System.Decimal")),
													 new DataColumn(CN.A2Relation),
													 new DataColumn(CN.RFCategory, Type.GetType("System.Int16")),
													 new DataColumn(CN.MonthlyIncome, Type.GetType("System.Double")),
													 new DataColumn(CN.AccountNumber),
													 new DataColumn(CN.PrevResidentialStatus),
													 new DataColumn(CN.PrevAddYY, Type.GetType("System.Int32")),
													 new DataColumn(CN.PrevAddMM, Type.GetType("System.Int32")),
													 new DataColumn(CN.PropResult),
													 new DataColumn(CN.PAddress1),
													 new DataColumn(CN.PAddress2),
													 new DataColumn(CN.PCity),
													 new DataColumn(CN.PPostCode),
													 new DataColumn(CN.EmployeeName),
													 new DataColumn(CN.EmploymentStatus),
								 					 new DataColumn(CN.Occupation),
								 					 new DataColumn(CN.PayFrequency),
													 new DataColumn(CN.DateEmpStart, Type.GetType("System.DateTime")),
													 new DataColumn(CN.DatePEmpStart, Type.GetType("System.DateTime")),
													 new DataColumn(CN.EmploymentTelNo),
													 new DataColumn(CN.EmploymentDialCode),
													 new DataColumn(CN.BankCode),
													 new DataColumn(CN.BankAccountNo),
													 new DataColumn(CN.BankAccountOpened, Type.GetType("System.DateTime")),
													 new DataColumn(CN.BankAccountType),
                                                     new DataColumn(CN.AdditionalExpenditure1, Type.GetType("System.Decimal")),
													 new DataColumn(CN.AdditionalExpenditure2, Type.GetType("System.Decimal")),
                                                     new DataColumn(CN.ScoringBand),
													 new DataColumn(CN.TransportType), //CR 866
													 new DataColumn(CN.EducationLevel), //CR 866
													 new DataColumn(CN.DistanceFromStore, Type.GetType("System.Int16")), //CR 866
													 new DataColumn(CN.Industry), //CR 866
													 new DataColumn(CN.JobTitle), //CR 866
                                                     new DataColumn(CN.Organisation),
													 new DataColumn(CN.DateIn), 
                                                     new DataColumn(CN.ResidentialStatus), 
                                                     new DataColumn(CN.PropertyType),
                                                     new DataColumn(CN.AllowReopenS1),//#10477
                                                     new DataColumn("PurchaseCashLoan"),
                                                     new DataColumn(CN.IsSpouseWorking)
												 });
            if (this.ProposalExists)
            {

                DataRow r = dt.NewRow();
                r[CN.DateProp] = this.DateProp;
                r[CN.S1Comment] = this.S1Comment;
                r[CN.NewS1Comment] = "";
                r[CN.MaritalStatus] = this.MaritalStatus;
                r[CN.Dependants] = this.Dependants;
                r[CN.Nationality] = this.Nationality;
                r[CN.PrevEmpYY] = this.PreviousEmploymentYY;
                r[CN.PrevEmpMM] = this.PreviousEmploymentMM;
                r[CN.AdditionalIncome] = this.AdditionalIncome;
                r[CN.AdditionalIncome2] = this.A2AdditionalIncome;
                r[CN.OtherPayments] = this.OtherPayments;
                r[CN.CCardNo1] = this.CreditCardNo1;
                r[CN.CCardNo2] = this.CreditCardNo2;
                r[CN.CCardNo3] = this.CreditCardNo3;
                r[CN.CCardNo4] = this.CreditCardNo4;
                r[CN.Commitments1] = this.Commitments1;
                r[CN.Commitments2] = this.Commitments2;
                r[CN.Commitments3] = this.Commitments3;
                r[CN.ApplicationStatus] = this.AppStatus;
                r[CN.A2Relation] = this.A2Relation;
                r[CN.RFCategory] = this.RFCategory;
                r[CN.MonthlyIncome] = this.MonthlyIncome;
                r[CN.AccountNumber] = this.AccountNo;
                r[CN.PrevResidentialStatus] = this.PreviousResidentialStatus;
                r[CN.PrevAddYY] = this.PreviousAddressYY;
                r[CN.PrevAddMM] = this.PreviousAddressMM;
                r[CN.PropResult] = this.PropResult;
                r[CN.PAddress1] = this.PreviousAddress1;
                r[CN.PAddress2] = this.PreviousAddress2;
                r[CN.PCity] = this.PreviousCity;
                r[CN.PPostCode] = this.PreviousPostCode;
                r[CN.EmployeeName] = this.EmployerName;

                r[CN.EmploymentStatus] = this.EmploymentStatus;
                r[CN.Occupation] = this.Occupation;
                r[CN.PayFrequency] = this.PayFrequency;
                r[CN.DateEmpStart] = this.DateEmpStart;
                r[CN.DatePEmpStart] = this.DatePEmpStart;
                r[CN.EmploymentTelNo] = this.EmploymentTelNo;
                r[CN.EmploymentDialCode] = this.EmploymentDialCode;
                r[CN.BankCode] = this.BankCode;
                r[CN.BankAccountNo] = this.BankAccountNo;
                r[CN.BankAccountOpened] = this.BankAccountOpened;
                r[CN.BankAccountType] = this.BankAccountType;
                r[CN.AdditionalExpenditure1] = this.AdditionalExpenditure1;
                r[CN.AdditionalExpenditure2] = this.AdditionalExpenditure2;
                r[CN.ScoringBand] = this.ScoringBand;

                //CR 866 Add additional fields 
                r[CN.TransportType] = this.TransportType;
                r[CN.EducationLevel] = this.EducationLevel;
                r[CN.DistanceFromStore] = this.DistanceFromStore;
                r[CN.Industry] = this.Industry;
                r[CN.JobTitle] = this.JobTitle;
                r[CN.Organisation] = this.Organisation;
                //End CR 866
                r[CN.DateIn] = this.DateIn;
                r[CN.ResidentialStatus] = this.ResidentialStatus;
                r[CN.PropertyType] = this.PropertyType;
                r[CN.AllowReopenS1] = this.AllowReopenS1;                  //#10477
                r["PurchaseCashLoan"] = this.PurchaseCashLoan;
                r[CN.IsSpouseWorking] = this.IsSpouseWorking;
                dt.Rows.Add(r);
            }
            return dt;
        }

        public DataTable GetStage2Row(string name)
        {
            DataTable dt = new DataTable(name);
            dt.Columns.AddRange(new DataColumn[] {	new DataColumn(CN.DateProp, Type.GetType("System.DateTime")),
													 new DataColumn(CN.ApplicationStatus),
													 new DataColumn(CN.A2Relation),
													 new DataColumn(CN.S2Comment),
													 new DataColumn(CN.SpecialPromo),
													 new DataColumn(CN.PAddress1),
													 new DataColumn(CN.PAddress2),
													 new DataColumn(CN.PCity),
													 new DataColumn(CN.PPostCode),
													 new DataColumn(CN.EmployeeName),
													 new DataColumn(CN.EmpDept),
													 new DataColumn(CN.EAddress1),
													 new DataColumn(CN.EAddress2),
													 new DataColumn(CN.ECity),
													 new DataColumn(CN.EPostCode),
													 new DataColumn(CN.NoOfRef, Type.GetType("System.Int32")),
													 new DataColumn(CN.S1Comment),
													 new DataColumn(CN.NewComment),
													 new DataColumn(CN.VehicleRegistration)});
            if (this.ProposalExists)
            {

                DataRow r = dt.NewRow();

                r[CN.DateProp] = this.DateProp;
                r[CN.ApplicationStatus] = this.AppStatus;
                r[CN.A2Relation] = this.A2Relation;
                r[CN.S2Comment] = this.S2Comment;
                r[CN.SpecialPromo] = this.SpecialPromo;
                r[CN.PAddress1] = this.PreviousAddress1;
                r[CN.PAddress2] = this.PreviousAddress2;
                r[CN.PCity] = this.PreviousCity;
                r[CN.PPostCode] = this.PreviousPostCode;
                r[CN.EmployeeName] = this.EmployerName;
                r[CN.EmpDept] = this.EmployerDept;
                r[CN.EAddress1] = this.EmployerAddress1;
                r[CN.EAddress2] = this.EmployerAddress2;
                r[CN.ECity] = this.EmployerCity;
                r[CN.EPostCode] = this.EmployerPostCode;
                r[CN.NoOfRef] = this.NoOfReferences;
                r[CN.S1Comment] = this.S1Comment;
                r[CN.NewComment] = "";
                r[CN.VehicleRegistration] = this.VehicleRegistration;
                dt.Rows.Add(r);
            }
            return dt;
        }

        public int GetProposal(string accountNumber, string customerID)
        {
            return GetProposal(null,null,accountNumber,customerID);
        }
        public int GetProposal( SqlConnection conn, SqlTransaction trans,string accountNumber, string customerID)
        {
            try
            {
                //CR 866 Changed this to 86
                parmArray = new SqlParameter[92];
                parmArray[0] = new SqlParameter("@origbr", SqlDbType.SmallInt);
                parmArray[0].Value = 0;
                parmArray[1] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
                parmArray[1].Value = customerID;
                parmArray[2] = new SqlParameter("@dateprop", SqlDbType.SmallDateTime);
                parmArray[2].Value = DBNull.Value;
                parmArray[3] = new SqlParameter("@origbranchno", SqlDbType.SmallInt);
                parmArray[3].Value = 0;
                parmArray[4] = new SqlParameter("@sanctserno", SqlDbType.Int);
                parmArray[4].Value = 0;
                parmArray[5] = new SqlParameter("@empeenoprop", SqlDbType.Int);
                parmArray[5].Value = 0;
                parmArray[6] = new SqlParameter("@maritalstat", SqlDbType.NVarChar, 1);
                parmArray[6].Value = "";
                parmArray[7] = new SqlParameter("@dependants", SqlDbType.Int);
                parmArray[7].Value = 0;
                parmArray[8] = new SqlParameter("@yrscuremplmt", SqlDbType.Float);
                parmArray[8].Value = 0;
                parmArray[9] = new SqlParameter("@mthlyincome", SqlDbType.Float);
                parmArray[9].Value = 0;
                parmArray[10] = new SqlParameter("@jobslstyrs", SqlDbType.Int);
                parmArray[10].Value = 0;
                parmArray[11] = new SqlParameter("@health", SqlDbType.NChar, 1);
                parmArray[11].Value = "";
                parmArray[12] = new SqlParameter("@otherpmnts", SqlDbType.Float);
                parmArray[12].Value = 0;
                parmArray[13] = new SqlParameter("@scorecardno", SqlDbType.SmallInt);
                parmArray[13].Value = 0;
                parmArray[14] = new SqlParameter("@points", SqlDbType.SmallInt);
                parmArray[14].Value = 0;
                parmArray[15] = new SqlParameter("@propresult", SqlDbType.NChar, 1);
                parmArray[15].Value = "";
                parmArray[16] = new SqlParameter("@reason", SqlDbType.NChar, 2);
                parmArray[16].Value = "";
                parmArray[17] = new SqlParameter("@yrscurraddr", SqlDbType.SmallInt);
                parmArray[17].Value = 0;
                parmArray[18] = new SqlParameter("@yrsprevaddr", SqlDbType.SmallInt);
                parmArray[18].Value = 0;
                parmArray[19] = new SqlParameter("@bankaccttype", SqlDbType.NChar, 1);
                parmArray[19].Value = "";
                parmArray[20] = new SqlParameter("@yrsbankachld", SqlDbType.SmallInt);
                parmArray[20].Value = 0;
                parmArray[21] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[21].Value = accountNumber;
                parmArray[22] = new SqlParameter("@propnotes", SqlDbType.NVarChar, 1000);
                parmArray[22].Value = "";
                parmArray[23] = new SqlParameter("@hasstring", SqlDbType.SmallInt);
                parmArray[23].Value = 0;
                parmArray[24] = new SqlParameter("@addincome", SqlDbType.Money);
                parmArray[24].Value = 0;
                parmArray[25] = new SqlParameter("@appstatus", SqlDbType.NChar, 4);
                parmArray[25].Value = "";
                parmArray[26] = new SqlParameter("@ccardno1", SqlDbType.NVarChar, 4);
                parmArray[26].Value = "";
                parmArray[27] = new SqlParameter("@ccardno2", SqlDbType.NVarChar, 4);
                parmArray[27].Value = "";
                parmArray[28] = new SqlParameter("@ccardno3", SqlDbType.NVarChar, 4);
                parmArray[28].Value = "";
                parmArray[29] = new SqlParameter("@ccardno4", SqlDbType.NVarChar, 4);
                parmArray[29].Value = "";
                parmArray[30] = new SqlParameter("@commitments1", SqlDbType.Money);
                parmArray[30].Value = 0;
                parmArray[31] = new SqlParameter("@commitments2", SqlDbType.Money);
                parmArray[31].Value = 0;
                parmArray[32] = new SqlParameter("@commitments3", SqlDbType.Money);
                parmArray[32].Value = 0;
                parmArray[33] = new SqlParameter("@empaddr1", SqlDbType.NVarChar, CW.Address1);
                parmArray[33].Value = "";
                parmArray[34] = new SqlParameter("@empaddr2", SqlDbType.NVarChar, CW.Address2);
                parmArray[34].Value = "";
                parmArray[35] = new SqlParameter("@empcity", SqlDbType.NVarChar, CW.Address3);
                parmArray[35].Value = "";
                parmArray[36] = new SqlParameter("@emppostcode", SqlDbType.NVarChar, 26);
                parmArray[36].Value = "";
                parmArray[37] = new SqlParameter("@empdept", SqlDbType.NVarChar, 26);
                parmArray[37].Value = "";
                parmArray[38] = new SqlParameter("@empname", SqlDbType.NVarChar, 26);
                parmArray[38].Value = "";
                parmArray[39] = new SqlParameter("@location", SqlDbType.NVarChar, 4);
                parmArray[39].Value = "";
                parmArray[40] = new SqlParameter("@nationality", SqlDbType.NVarChar, 4);
                parmArray[40].Value = "";
                parmArray[41] = new SqlParameter("@noofref", SqlDbType.Int);
                parmArray[41].Value = 0;
                parmArray[42] = new SqlParameter("@pempmm", SqlDbType.Int);
                parmArray[42].Value = 0;
                parmArray[43] = new SqlParameter("@pempyy", SqlDbType.Int);
                parmArray[43].Value = 0;
                parmArray[44] = new SqlParameter("@proofaddress", SqlDbType.NVarChar, 4);
                parmArray[44].Value = "";
                parmArray[45] = new SqlParameter("@proofid", SqlDbType.NVarChar, 4);
                parmArray[45].Value = "";
                parmArray[46] = new SqlParameter("@proofincome", SqlDbType.NVarChar, 4);
                parmArray[46].Value = "";
                parmArray[47] = new SqlParameter("@s1comment", SqlDbType.NVarChar, 1000);
                parmArray[47].Value = "";
                parmArray[48] = new SqlParameter("@s2comment", SqlDbType.NVarChar, 1000);
                parmArray[48].Value = "";
                parmArray[49] = new SqlParameter("@specialpromo", SqlDbType.NVarChar, 1);
                parmArray[49].Value = "";
                parmArray[50] = new SqlParameter("@a2mthlyincome", SqlDbType.Money);
                parmArray[50].Value = 0;
                parmArray[51] = new SqlParameter("@a2addincome", SqlDbType.Money);
                parmArray[51].Value = 0;
                parmArray[52] = new SqlParameter("@a2maritalstat", SqlDbType.NVarChar, 4);
                parmArray[52].Value = "";
                parmArray[53] = new SqlParameter("@transactidno", SqlDbType.NChar, 10);
                parmArray[53].Value = "";
                parmArray[54] = new SqlParameter("@paddress1", SqlDbType.NVarChar, CW.Address1);
                parmArray[54].Value = "";
                parmArray[55] = new SqlParameter("@paddress2", SqlDbType.NVarChar, CW.Address2);
                parmArray[55].Value = "";
                parmArray[56] = new SqlParameter("@pcity", SqlDbType.NVarChar, CW.Address3);
                parmArray[56].Value = "";
                parmArray[57] = new SqlParameter("@ppostcode", SqlDbType.NVarChar, 26);
                parmArray[57].Value = "";
                parmArray[58] = new SqlParameter("@paddyy", SqlDbType.Int);
                parmArray[58].Value = 0;
                parmArray[59] = new SqlParameter("@paddmm", SqlDbType.Int);
                parmArray[59].Value = 0;
                parmArray[60] = new SqlParameter("@presstatus", SqlDbType.NVarChar, 4);
                parmArray[60].Value = "";
                parmArray[61] = new SqlParameter("@empeenochange", SqlDbType.Int);
                parmArray[61].Value = 0;
                parmArray[62] = new SqlParameter("@datechange", SqlDbType.DateTime);
                parmArray[62].Value = DBNull.Value;
                parmArray[63] = new SqlParameter("@A2Relation", SqlDbType.NVarChar, 1);
                parmArray[63].Value = "";
                parmArray[64] = new SqlParameter("@RFCategory", SqlDbType.SmallInt);
                parmArray[64].Value = 0;
                parmArray[65] = new SqlParameter("@outAcctno", SqlDbType.NVarChar, 12);
                parmArray[65].Value = 0;

                parmArray[66] = new SqlParameter("@EmploymentStatus", SqlDbType.NChar, 1);
                parmArray[66].Value = "";
                parmArray[67] = new SqlParameter("@Occupation", SqlDbType.NVarChar, 2);
                parmArray[67].Value = "";
                parmArray[68] = new SqlParameter("@PayFrequency", SqlDbType.NChar, 1);
                parmArray[68].Value = "";
                parmArray[69] = new SqlParameter("@DateEmpStart", SqlDbType.DateTime);
                parmArray[69].Value = DBNull.Value;
                parmArray[70] = new SqlParameter("@DatePEmpStart", SqlDbType.DateTime);
                parmArray[70].Value = DBNull.Value;
                parmArray[71] = new SqlParameter("@EmploymentTelNo", SqlDbType.NChar, 13);
                parmArray[71].Value = "";
                parmArray[72] = new SqlParameter("@EmploymentDialCode", SqlDbType.NChar, 8);
                parmArray[72].Value = "";
                parmArray[73] = new SqlParameter("@BankCode", SqlDbType.NVarChar, 6);
                parmArray[73].Value = "";
                parmArray[74] = new SqlParameter("@BankAccountNo", SqlDbType.NVarChar, 20);
                parmArray[74].Value = 0;
                parmArray[75] = new SqlParameter("@BankAccountOpened", SqlDbType.DateTime);
                parmArray[75].Value = DBNull.Value;
                parmArray[76] = new SqlParameter("@additionalexpenditure1", SqlDbType.Money);
                parmArray[76].Value = 0;
                parmArray[77] = new SqlParameter("@additionalexpenditure2", SqlDbType.Money);
                parmArray[77].Value = 0;
                parmArray[78] = new SqlParameter("@vehicleregistration", SqlDbType.NVarChar, 15);
                parmArray[78].Value = "";
                parmArray[79] = new SqlParameter("@scoringBand", SqlDbType.NVarChar, 4);
                parmArray[79].Value = "";
                
                //CR 866a
                //CR 866 Additional fields
                parmArray[80] = new SqlParameter("@TransportType", SqlDbType.NVarChar, CW.TransportType);
                parmArray[80].Value = "";

                parmArray[81] = new SqlParameter("@EducationLevel", SqlDbType.NVarChar, CW.EducationLevel);
                parmArray[81].Value = "";

                parmArray[82] = new SqlParameter("@DistanceFromStore", SqlDbType.SmallInt);
                parmArray[82].Value = "";

                parmArray[83] = new SqlParameter("@Industry", SqlDbType.NVarChar, CW.Industry);
                parmArray[83].Value = "";

                parmArray[84] = new SqlParameter("@JobTitle", SqlDbType.NVarChar, CW.JobTitle);
                parmArray[84].Value = "";

                parmArray[85] = new SqlParameter("@Organisation", SqlDbType.NVarChar, CW.Organisation);
                parmArray[85].Value = "";

                parmArray[86] = new SqlParameter("@DateCurrAddress", SqlDbType.DateTime);
                parmArray[86].Value = this.DateIn;

                parmArray[87] = new SqlParameter("@CurrentResStatus", SqlDbType.VarChar,4);
                parmArray[87].Value = this.ResidentialStatus;

                parmArray[88] = new SqlParameter("@PropertyType", SqlDbType.VarChar,4);
                parmArray[88].Value = this.PropertyType;

                //End CR 866 

                parmArray[89] = new SqlParameter("@AllowReopenS1", SqlDbType.Bit);    //#10477
                parmArray[89].Value = this.AllowReopenS1;

                parmArray[90] = new SqlParameter("@PurchaseCashLoan", SqlDbType.Bit);    
                parmArray[90].Value = this.PurchaseCashLoan;

                parmArray[91] = new SqlParameter("@IsSpouseWorking", SqlDbType.Bit);
                parmArray[91].Value = this.IsSpouseWorking;

                foreach (SqlParameter parm in parmArray)
                {
                    parm.Direction = ParameterDirection.Output;
                }
                parmArray[1].Direction = ParameterDirection.Input;	//customerID
                parmArray[21].Direction = ParameterDirection.Input;	//accountNumber
                if (conn ==null)
                    result = this.RunSP("DN_ProposalGetSP", parmArray);
                else
                    result = this.RunSP(conn,trans,"DN_ProposalGetSP", parmArray);

                if (result == -1)
                    this.ProposalExists = false;
                else
                {
                    this.ProposalExists = true;

                    if (!Convert.IsDBNull(parmArray[0].Value))
                        this.OrigBr = (short)parmArray[0].Value;
                    if (!Convert.IsDBNull(parmArray[2].Value))
                        this.DateProp = (DateTime)parmArray[2].Value;
                    if (!Convert.IsDBNull(parmArray[3].Value))
                        this.OrigBranchNo = (short)parmArray[3].Value;
                    if (!Convert.IsDBNull(parmArray[4].Value))
                        this.SanctionSerialNumber = (int)parmArray[4].Value;
                    if (!Convert.IsDBNull(parmArray[5].Value))
                        this.EmployeeNoProp = (int)parmArray[5].Value;
                    if (!Convert.IsDBNull(parmArray[6].Value))
                        this.MaritalStatus = (string)parmArray[6].Value;
                    if (!Convert.IsDBNull(parmArray[7].Value))
                        this.Dependants = (int)parmArray[7].Value;
                    if (!Convert.IsDBNull(parmArray[8].Value))
                        this.YearsCurrentEmployment = (double)parmArray[8].Value;
                    //if(!Convert.IsDBNull(parmArray[9].Value))
                    this.MonthlyIncome = parmArray[9].Value;
                    if (!Convert.IsDBNull(parmArray[10].Value))
                        this.JobsLastYear = (int)parmArray[10].Value;
                    if (!Convert.IsDBNull(parmArray[11].Value))
                        this.Health = (string)parmArray[11].Value;
                    //if(!Convert.IsDBNull(parmArray[12].Value))
                    this.OtherPayments = parmArray[12].Value;
                    if (!Convert.IsDBNull(parmArray[13].Value))
                        this.ScorecardNo = (short)parmArray[13].Value;
                    if (!Convert.IsDBNull(parmArray[14].Value))
                        this.Points = (short)parmArray[14].Value;
                    if (!Convert.IsDBNull(parmArray[15].Value))
                        this.PropResult = (string)parmArray[15].Value;
                    if (!Convert.IsDBNull(parmArray[16].Value))
                        this.Reason = (string)parmArray[16].Value;
                    if (!Convert.IsDBNull(parmArray[17].Value))
                        this.YearsCurrentAddress = (short)parmArray[17].Value;
                    if (!Convert.IsDBNull(parmArray[18].Value))
                        this.YearsPreviousAddress = (short)parmArray[18].Value;
                    if (!Convert.IsDBNull(parmArray[19].Value))
                        this.BankAccountType = (string)parmArray[19].Value;
                    if (!Convert.IsDBNull(parmArray[20].Value))
                        this.YearsBankAccountHeld = (short)parmArray[20].Value;
                    if (!Convert.IsDBNull(parmArray[22].Value))
                        this.ProposalNotes = (string)parmArray[22].Value;
                    if (!Convert.IsDBNull(parmArray[23].Value))
                        this.HasString = (short)parmArray[23].Value;
                    //if(!Convert.IsDBNull(parmArray[24].Value))
                    this.AdditionalIncome = parmArray[24].Value;
                    if (!Convert.IsDBNull(parmArray[25].Value))
                        this.AppStatus = (string)parmArray[25].Value;
                    if (!Convert.IsDBNull(parmArray[26].Value))
                        this.CreditCardNo1 = (string)parmArray[26].Value;
                    if (!Convert.IsDBNull(parmArray[27].Value))
                        this.CreditCardNo2 = (string)parmArray[27].Value;
                    if (!Convert.IsDBNull(parmArray[28].Value))
                        this.CreditCardNo3 = (string)parmArray[28].Value;
                    if (!Convert.IsDBNull(parmArray[29].Value))
                        this.CreditCardNo4 = (string)parmArray[29].Value;
                    //if(!Convert.IsDBNull(parmArray[30].Value))
                    this.Commitments1 = parmArray[30].Value;
                    //if(!Convert.IsDBNull(parmArray[31].Value))
                    this.Commitments2 = parmArray[31].Value;
                    //if(!Convert.IsDBNull(parmArray[32].Value))
                    this.Commitments3 = parmArray[32].Value;
                    if (!Convert.IsDBNull(parmArray[33].Value))
                        this.EmployerAddress1 = (string)parmArray[33].Value;
                    if (!Convert.IsDBNull(parmArray[34].Value))
                        this.EmployerAddress2 = (string)parmArray[34].Value;
                    if (!Convert.IsDBNull(parmArray[35].Value))
                        this.EmployerCity = (string)parmArray[35].Value;
                    if (!Convert.IsDBNull(parmArray[36].Value))
                        this.EmployerPostCode = (string)parmArray[36].Value;
                    if (!Convert.IsDBNull(parmArray[37].Value))
                        this.EmployerDept = (string)parmArray[37].Value;
                    if (!Convert.IsDBNull(parmArray[38].Value))
                        this.EmployerName = (string)parmArray[38].Value;
                    if (!Convert.IsDBNull(parmArray[39].Value))
                        this.Location = (string)parmArray[39].Value;
                    if (!Convert.IsDBNull(parmArray[40].Value))
                        this.Nationality = (string)parmArray[40].Value;
                    if (!Convert.IsDBNull(parmArray[41].Value))
                        this.NoOfReferences = (int)parmArray[41].Value;
                    if (!Convert.IsDBNull(parmArray[42].Value))
                        this.PreviousEmploymentMM = (int)parmArray[42].Value;
                    if (!Convert.IsDBNull(parmArray[43].Value))
                        this.PreviousEmploymentYY = (int)parmArray[43].Value;
                    if (!Convert.IsDBNull(parmArray[44].Value))
                        this.ProofOfAddress = (string)parmArray[44].Value;
                    if (!Convert.IsDBNull(parmArray[45].Value))
                        this.ProofOfID = (string)parmArray[45].Value;
                    if (!Convert.IsDBNull(parmArray[46].Value))
                        this.ProofOfIncome = (string)parmArray[46].Value;
                    if (!Convert.IsDBNull(parmArray[47].Value))
                        this.S1Comment = (string)parmArray[47].Value;
                    if (!Convert.IsDBNull(parmArray[48].Value))
                        this.S2Comment = (string)parmArray[48].Value;
                    if (!Convert.IsDBNull(parmArray[49].Value))
                        this.SpecialPromo = (string)parmArray[49].Value;
                    //if(!Convert.IsDBNull(parmArray[50].Value))
                    this.A2MonthlyIncome = parmArray[50].Value;
                    //if(!Convert.IsDBNull(parmArray[51].Value))
                    this.A2AdditionalIncome = parmArray[51].Value;
                    if (!Convert.IsDBNull(parmArray[52].Value))
                        this.A2MaritalStatus = (string)parmArray[52].Value;
                    if (!Convert.IsDBNull(parmArray[53].Value))
                        this.TransactIdNo = (string)parmArray[53].Value;
                    if (!Convert.IsDBNull(parmArray[54].Value))
                        this.PreviousAddress1 = (string)parmArray[54].Value;
                    if (!Convert.IsDBNull(parmArray[55].Value))
                        this.PreviousAddress2 = (string)parmArray[55].Value;
                    if (!Convert.IsDBNull(parmArray[56].Value))
                        this.PreviousCity = (string)parmArray[56].Value;
                    if (!Convert.IsDBNull(parmArray[57].Value))
                        this.PreviousPostCode = (string)parmArray[57].Value;
                    if (!Convert.IsDBNull(parmArray[58].Value))
                        this.PreviousAddressYY = (int)parmArray[58].Value;
                    if (!Convert.IsDBNull(parmArray[59].Value))
                        this.PreviousAddressMM = (int)parmArray[59].Value;
                    if (!Convert.IsDBNull(parmArray[60].Value))
                        this.PreviousResidentialStatus = (string)parmArray[60].Value;
                    if (!Convert.IsDBNull(parmArray[61].Value))
                        this.EmployeeNoChanged = (int)parmArray[61].Value;
                    if (!Convert.IsDBNull(parmArray[62].Value))
                        this.DateChanged = (DateTime)parmArray[62].Value;
                    if (!Convert.IsDBNull(parmArray[63].Value))
                        this.A2Relation = ((string)parmArray[63].Value).Trim();
                    if (!Convert.IsDBNull(parmArray[64].Value))
                        this.RFCategory = (short)parmArray[64].Value;
                    if (!Convert.IsDBNull(parmArray[65].Value))
                        this.AccountNo = (string)parmArray[65].Value;

                    if (!Convert.IsDBNull(parmArray[66].Value))
                        this.EmploymentStatus = (string)parmArray[66].Value;
                    if (!Convert.IsDBNull(parmArray[67].Value))
                        this.Occupation = (string)parmArray[67].Value;
                    if (!Convert.IsDBNull(parmArray[68].Value))
                        this.PayFrequency = (string)parmArray[68].Value;
                    if (!Convert.IsDBNull(parmArray[69].Value))
                        this.DateEmpStart = (DateTime)parmArray[69].Value;
                    else
                        this.DateEmpStart = DateTime.MinValue.AddYears(1899);
                    if (!Convert.IsDBNull(parmArray[70].Value))
                        this.DatePEmpStart = (DateTime)parmArray[70].Value;
                    else
                        this.DatePEmpStart = DateTime.MinValue.AddYears(1899);
                    if (!Convert.IsDBNull(parmArray[71].Value))
                        this.EmploymentTelNo = (string)parmArray[71].Value;
                    if (!Convert.IsDBNull(parmArray[72].Value))
                        this.EmploymentDialCode = (string)parmArray[72].Value;
                    if (!Convert.IsDBNull(parmArray[73].Value))
                        this.BankCode = (string)parmArray[73].Value;
                    if (!Convert.IsDBNull(parmArray[74].Value))
                        this.BankAccountNo = (string)parmArray[74].Value;
                    if (!Convert.IsDBNull(parmArray[75].Value))
                        this.BankAccountOpened = (DateTime)parmArray[75].Value;
                    else
                        this.BankAccountOpened = DateTime.MinValue.AddYears(1899);
                    //if(!Convert.IsDBNull(parmArray[76].Value))
                    this.AdditionalExpenditure1 = parmArray[76].Value;
                    //if(!Convert.IsDBNull(parmArray[77].Value))
                    this.AdditionalExpenditure2 = parmArray[77].Value;
                    if (!Convert.IsDBNull(parmArray[78].Value))
                        this.VehicleRegistration = (string)parmArray[78].Value;
                    if (!Convert.IsDBNull(parmArray[79].Value))
                        this.ScoringBand = (string)parmArray[79].Value;

                    //CR 866 Additional fields
                    if (!Convert.IsDBNull(parmArray[80].Value))
                        this.TransportType = parmArray[80].Value.ToString();
                    if (!Convert.IsDBNull(parmArray[81].Value))
                        this.EducationLevel = parmArray[81].Value.ToString();
                    if (!Convert.IsDBNull(parmArray[82].Value))
                        this.DistanceFromStore = Convert.ToInt16(parmArray[82].Value);
                    if (!Convert.IsDBNull(parmArray[83].Value))
                        this.Industry = parmArray[83].Value.ToString();
                    if (!Convert.IsDBNull(parmArray[84].Value))
                        this.JobTitle = parmArray[84].Value.ToString();
                    if (!Convert.IsDBNull(parmArray[85].Value))
                        this.Organisation = parmArray[85].Value.ToString();
                    if (!Convert.IsDBNull(parmArray[86].Value))
                        this.DateIn = Convert.ToDateTime(parmArray[86].Value);
                    if (!Convert.IsDBNull(parmArray[87].Value))
                        this.ResidentialStatus = parmArray[87].Value.ToString();
                    if (!Convert.IsDBNull(parmArray[88].Value))
                        this.PropertyType= parmArray[88].Value.ToString();
                    if (!Convert.IsDBNull(parmArray[89].Value))                             //#10477
                        this.AllowReopenS1 = Convert.ToBoolean(parmArray[89].Value);
                    if (!Convert.IsDBNull(parmArray[90].Value))                             
                        this.PurchaseCashLoan = Convert.ToBoolean(parmArray[90].Value);
                    if (!Convert.IsDBNull(parmArray[91].Value))
                        this.IsSpouseWorking = Convert.ToBoolean(parmArray[91].Value);

                    //End CR 866
                }

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;
        }

        public int Save(SqlConnection conn, SqlTransaction trans, string customerID, string accountNumber)
        {
            try
            { 
                if (accountNumber.Length == 0)
                    accountNumber = "ORF";
                //CR 866 Changed this to 85
                parmArray = new SqlParameter[90];
                parmArray[0] = new SqlParameter("@origbr", SqlDbType.SmallInt);
                parmArray[0].Value = this.OrigBr;
                parmArray[1] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
                parmArray[1].Value = customerID;
                parmArray[2] = new SqlParameter("@dateprop", SqlDbType.SmallDateTime);
                parmArray[2].Value = this.DateProp;
                parmArray[3] = new SqlParameter("@origbranchno", SqlDbType.SmallInt);
                parmArray[3].Value = this.OrigBranchNo;
                parmArray[4] = new SqlParameter("@sanctserno", SqlDbType.Int);
                parmArray[4].Value = this.SanctionSerialNumber;
                parmArray[5] = new SqlParameter("@empeenoprop", SqlDbType.Int);
                parmArray[5].Value = this.EmployeeNoProp;
                parmArray[6] = new SqlParameter("@maritalstat", SqlDbType.NVarChar, 1);
                parmArray[6].Value = this.MaritalStatus;
                parmArray[7] = new SqlParameter("@dependants", SqlDbType.Int);
                parmArray[7].Value = this.Dependants;
                parmArray[8] = new SqlParameter("@yrscuremplmt", SqlDbType.Float);
                parmArray[8].Value = this.YearsCurrentEmployment;
                parmArray[9] = new SqlParameter("@mthlyincome", SqlDbType.Float);
                parmArray[9].Value = this.MonthlyIncome;
                parmArray[10] = new SqlParameter("@jobslstyrs", SqlDbType.Int);
                parmArray[10].Value = this.JobsLastYear;
                parmArray[11] = new SqlParameter("@health", SqlDbType.NChar, 1);
                parmArray[11].Value = this.Health;
                parmArray[12] = new SqlParameter("@otherpmnts", SqlDbType.Float);
                parmArray[12].Value = this.OtherPayments;
                parmArray[13] = new SqlParameter("@scorecardno", SqlDbType.SmallInt);
                parmArray[13].Value = this.ScorecardNo;
                parmArray[14] = new SqlParameter("@points", SqlDbType.SmallInt);
                parmArray[14].Value = this.Points;
                parmArray[15] = new SqlParameter("@propresult", SqlDbType.NChar, 1);
                parmArray[15].Value = this.PropResult;
                parmArray[16] = new SqlParameter("@reason", SqlDbType.NChar, 2);
                parmArray[16].Value = this.Reason;
                parmArray[17] = new SqlParameter("@yrscurraddr", SqlDbType.SmallInt);
                parmArray[17].Value = this.YearsCurrentAddress;
                parmArray[18] = new SqlParameter("@yrsprevaddr", SqlDbType.SmallInt);
                parmArray[18].Value = this.YearsPreviousAddress;
                parmArray[19] = new SqlParameter("@bankaccttype", SqlDbType.NChar, 1);
                parmArray[19].Value = this.BankAccountType;
                parmArray[20] = new SqlParameter("@yrsbankachld", SqlDbType.SmallInt);
                parmArray[20].Value = this.YearsBankAccountHeld;
                parmArray[21] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[21].Value = accountNumber;
                parmArray[22] = new SqlParameter("@propnotes", SqlDbType.NVarChar, 1000);
                parmArray[22].Value = this.ProposalNotes;
                parmArray[23] = new SqlParameter("@hasstring", SqlDbType.SmallInt);
                parmArray[23].Value = this.HasString;
                parmArray[24] = new SqlParameter("@addincome", SqlDbType.Money);
                parmArray[24].Value = this.AdditionalIncome;
                parmArray[25] = new SqlParameter("@appstatus", SqlDbType.NChar, 4);
                parmArray[25].Value = this.AppStatus;
                parmArray[26] = new SqlParameter("@ccardno1", SqlDbType.NVarChar, 4);
                parmArray[26].Value = this.CreditCardNo1;
                parmArray[27] = new SqlParameter("@ccardno2", SqlDbType.NVarChar, 4);
                parmArray[27].Value = this.CreditCardNo2;
                parmArray[28] = new SqlParameter("@ccardno3", SqlDbType.NVarChar, 4);
                parmArray[28].Value = this.CreditCardNo3;
                parmArray[29] = new SqlParameter("@ccardno4", SqlDbType.NVarChar, 4);
                parmArray[29].Value = this.CreditCardNo4;
                parmArray[30] = new SqlParameter("@commitments1", SqlDbType.Money);
                parmArray[30].Value = this.Commitments1;
                parmArray[31] = new SqlParameter("@commitments2", SqlDbType.Money);
                parmArray[31].Value = this.Commitments2;
                parmArray[32] = new SqlParameter("@commitments3", SqlDbType.Money);
                parmArray[32].Value = this.Commitments3;
                parmArray[33] = new SqlParameter("@empaddr1", SqlDbType.NVarChar, CW.Address1);
                parmArray[33].Value = this.EmployerAddress1;
                parmArray[34] = new SqlParameter("@empaddr2", SqlDbType.NVarChar, CW.Address2);
                parmArray[34].Value = this.EmployerAddress2;
                parmArray[35] = new SqlParameter("@empcity", SqlDbType.NVarChar, CW.Address3);
                parmArray[35].Value = this.EmployerCity;
                parmArray[36] = new SqlParameter("@emppostcode", SqlDbType.NVarChar, 26);
                parmArray[36].Value = this.EmployerPostCode;
                parmArray[37] = new SqlParameter("@empdept", SqlDbType.NVarChar, 26);
                parmArray[37].Value = this.EmployerDept;
                parmArray[38] = new SqlParameter("@empname", SqlDbType.NVarChar, 26);
                parmArray[38].Value = this.EmployerName;
                parmArray[39] = new SqlParameter("@location", SqlDbType.NVarChar, 4);
                parmArray[39].Value = this.Location;
                parmArray[40] = new SqlParameter("@nationality", SqlDbType.NVarChar, 4);
                parmArray[40].Value = this.Nationality;
                parmArray[41] = new SqlParameter("@noofref", SqlDbType.Int);
                parmArray[41].Value = this.NoOfReferences;
                parmArray[42] = new SqlParameter("@pempmm", SqlDbType.Int);
                parmArray[42].Value = this.PreviousEmploymentMM;
                parmArray[43] = new SqlParameter("@pempyy", SqlDbType.Int);
                parmArray[43].Value = this.PreviousEmploymentYY;
                parmArray[44] = new SqlParameter("@proofaddress", SqlDbType.NVarChar, 4);
                parmArray[44].Value = this.ProofOfAddress;
                parmArray[45] = new SqlParameter("@proofid", SqlDbType.NVarChar, 4);
                parmArray[45].Value = this.ProofOfID;
                parmArray[46] = new SqlParameter("@proofincome", SqlDbType.NVarChar, 4);
                parmArray[46].Value = this.ProofOfIncome;
                parmArray[47] = new SqlParameter("@s1comment", SqlDbType.NVarChar, 1000);
                parmArray[47].Value = this.S1Comment;
                parmArray[48] = new SqlParameter("@s2comment", SqlDbType.NVarChar, 1000);
                parmArray[48].Value = this.S2Comment;
                parmArray[49] = new SqlParameter("@specialpromo", SqlDbType.NVarChar, 1);
                parmArray[49].Value = this.SpecialPromo;
                parmArray[50] = new SqlParameter("@a2mthlyincome", SqlDbType.Money);
                parmArray[50].Value = this.A2MonthlyIncome;
                parmArray[51] = new SqlParameter("@a2addincome", SqlDbType.Money);
                parmArray[51].Value = this.A2AdditionalIncome;
                parmArray[52] = new SqlParameter("@a2maritalstat", SqlDbType.NVarChar, 4);
                parmArray[52].Value = this.A2MaritalStatus;
                parmArray[53] = new SqlParameter("@transactidno", SqlDbType.NChar, 10);
                parmArray[53].Value = this.TransactIdNo;
                parmArray[54] = new SqlParameter("@paddress1", SqlDbType.NVarChar, CW.Address1);
                parmArray[54].Value = this.PreviousAddress1;
                parmArray[55] = new SqlParameter("@paddress2", SqlDbType.NVarChar, CW.Address2);
                parmArray[55].Value = this.PreviousAddress2;
                parmArray[56] = new SqlParameter("@pcity", SqlDbType.NVarChar, CW.Address3);
                parmArray[56].Value = this.PreviousCity;
                parmArray[57] = new SqlParameter("@ppostcode", SqlDbType.NVarChar, 26);
                parmArray[57].Value = this.PreviousPostCode;
                parmArray[58] = new SqlParameter("@paddyy", SqlDbType.Int);
                parmArray[58].Value = this.PreviousAddressYY;
                parmArray[59] = new SqlParameter("@paddmm", SqlDbType.Int);
                parmArray[59].Value = this.PreviousAddressMM;
                parmArray[60] = new SqlParameter("@presstatus", SqlDbType.NVarChar, 4);
                parmArray[60].Value = this.PreviousResidentialStatus;
                parmArray[61] = new SqlParameter("@empeenochange", SqlDbType.Int);
                parmArray[61].Value = this.EmployeeNoChanged;
                parmArray[62] = new SqlParameter("@datechange", SqlDbType.DateTime);
                parmArray[62].Value = DBNull.Value;
                parmArray[63] = new SqlParameter("@A2Relation", SqlDbType.NVarChar, 1);
                parmArray[63].Value = this.A2Relation;
                parmArray[64] = new SqlParameter("@RFCategory", SqlDbType.SmallInt);
                parmArray[64].Value = this.RFCategory;

                parmArray[65] = new SqlParameter("@EmploymentStatus", SqlDbType.NChar, 1);
                parmArray[65].Value = this.EmploymentStatus;
                parmArray[66] = new SqlParameter("@Occupation", SqlDbType.NVarChar, 2);
                parmArray[66].Value = this.Occupation;
                parmArray[67] = new SqlParameter("@PayFrequency", SqlDbType.NChar, 1);
                parmArray[67].Value = this.PayFrequency;
                parmArray[68] = new SqlParameter("@DateEmpStart", SqlDbType.DateTime);
                parmArray[68].Value = this.DateEmpStart;
                parmArray[69] = new SqlParameter("@DatePEmpStart", SqlDbType.DateTime);
                parmArray[69].Value = this.DatePEmpStart;
                parmArray[70] = new SqlParameter("@EmploymentTelNo", SqlDbType.NChar, 13);
                parmArray[70].Value = this.EmploymentTelNo;
                parmArray[71] = new SqlParameter("@EmploymentDialCode", SqlDbType.NChar, 8);
                parmArray[71].Value = this.EmploymentDialCode;
                parmArray[72] = new SqlParameter("@BankCode", SqlDbType.NVarChar, 6);
                parmArray[72].Value = this.BankCode;
                parmArray[73] = new SqlParameter("@BankAccountNo", SqlDbType.NVarChar, 20);
                parmArray[73].Value = this.BankAccountNo;
                parmArray[74] = new SqlParameter("@BankAccountOpened", SqlDbType.DateTime);
                parmArray[74].Value = this.BankAccountOpened;
                parmArray[75] = new SqlParameter("@additionalexpenditure1", SqlDbType.Money);
                parmArray[75].Value = this.AdditionalExpenditure1;
                parmArray[76] = new SqlParameter("@additionalexpenditure2", SqlDbType.Money);
                parmArray[76].Value = this.AdditionalExpenditure2;
                parmArray[77] = new SqlParameter("@vehicleregistration", SqlDbType.NVarChar, 15);
                parmArray[77].Value = this.VehicleRegistration;
                parmArray[78] = new SqlParameter("@scoringBand", SqlDbType.NVarChar, 4);
                parmArray[78].Value = this.ScoringBand;
                
                //CR 866 Additional fields
                //CR 866a
                parmArray[79] = new SqlParameter("@TransportType", SqlDbType.NVarChar, CW.TransportType);
                parmArray[79].Value = this.TransportType;

                parmArray[80] = new SqlParameter("@EducationLevel", SqlDbType.NVarChar, CW.EducationLevel);
                parmArray[80].Value = this.EducationLevel;

                parmArray[81] = new SqlParameter("@DistanceFromStore", SqlDbType.SmallInt);
                parmArray[81].Value = this.DistanceFromStore;

                parmArray[82] = new SqlParameter("@Industry", SqlDbType.NVarChar, CW.Industry);
                parmArray[82].Value = this.Industry;

                parmArray[83] = new SqlParameter("@JobTitle", SqlDbType.NVarChar, CW.JobTitle);
                parmArray[83].Value = this.JobTitle;

                parmArray[84] = new SqlParameter("@Organisation", SqlDbType.NVarChar, CW.Organisation);
                parmArray[84].Value = this.Organisation;

                parmArray[85] = new SqlParameter("@DateCurrAddress", SqlDbType.DateTime);
                if (this.DateIn > DateTime.MinValue)
                    parmArray[85].Value = this.DateIn;
                else // set to null to prevent overflow
                    parmArray[85].Value = null;

                parmArray[86] = new SqlParameter("@CurrentResStatus", SqlDbType.VarChar,4);
                parmArray[86].Value = this.ResidentialStatus;

                parmArray[87] = new SqlParameter("@PropertyType", SqlDbType.VarChar,4);
                parmArray[87].Value = this.PropertyType;

                parmArray[88] = new SqlParameter("@PurchaseCashLoan", SqlDbType.Bit);
                parmArray[88].Value = this.PurchaseCashLoan;

                parmArray[89] = new  SqlParameter("@IsSpouseWorking", SqlDbType.Bit);
                parmArray[89].Value = this.IsSpouseWorking;

                //End CR 866 

                result = this.RunSP(conn, trans, "DN_ProposalSaveSP", parmArray);
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

        //CR 835 Added [Peter Chong] 11-Oct-2006
        public int SaveCustomerAdditionalDetailsFinancial(SqlConnection conn, SqlTransaction trans)
        {
            try{
                int result = 0;
                parmArray = new SqlParameter[16];

                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, CW.CustomerID);
                parmArray[0].Value = this.CustomerID;

                parmArray[1] = new SqlParameter("@mthlyincome", SqlDbType.Money);
                parmArray[1].Value = this.MonthlyIncome;

                parmArray[2] = new SqlParameter("@AdditionalIncome", SqlDbType.Money);
                parmArray[2].Value = this.AdditionalIncome;

                parmArray[3] = new SqlParameter("@Commitments1", SqlDbType.Money);
                parmArray[3].Value = this.Commitments1;

                parmArray[4] = new SqlParameter("@Commitments2", SqlDbType.Money);
                parmArray[4].Value = this.Commitments2;

                parmArray[5] = new SqlParameter("@Commitments3", SqlDbType.Money);
                parmArray[5].Value = this.Commitments3;

                parmArray[6] = new SqlParameter("@OtherPayments", SqlDbType.Money);
                parmArray[6].Value = this.OtherPayments;

                parmArray[7] = new SqlParameter("@AdditionalExpenditure1", SqlDbType.Money);
                parmArray[7].Value = this.AdditionalExpenditure1;

                parmArray[8] = new SqlParameter("@AdditionalExpenditure2", SqlDbType.Money);
                parmArray[8].Value = this.AdditionalExpenditure2;

                parmArray[9] = new SqlParameter("@CCardNo1", SqlDbType.Char, 4);
                parmArray[9].Value = this.CreditCardNo1;

                parmArray[10] = new SqlParameter("@CCardNo2", SqlDbType.Char, 4);
                parmArray[10].Value = this.CreditCardNo2;

                parmArray[11] = new SqlParameter("@CCardNo3", SqlDbType.Char, 4);
                parmArray[11].Value = this.CreditCardNo3;

                parmArray[12] = new SqlParameter("@CCardNo4", SqlDbType.Char, 4);
                parmArray[12].Value = this.CreditCardNo4;

                parmArray[13] = new SqlParameter("@DueDayId", SqlDbType.Int);
                parmArray[13].Value = this.DueDayId;

                parmArray[14] = new SqlParameter("@BankAccountname", SqlDbType.NVarChar, 40);
                parmArray[14].Value = this.BankAccountName;

                parmArray[15] = new SqlParameter("@PaymentMethod", SqlDbType.NVarChar, 12);
                parmArray[15].Value = this.PaymentMethod;


                result = this.RunSP(conn, trans, "DN_CustomerAdditionalDetailsFinancialUpdate", parmArray);
                
                if (result == 0)
                    result = (int)Return.Success;
                else
                    result = (int)Return.Fail;
                return result;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public int GetPropsalStatus(string accountNumber)
        {
            try
            {
                parmArray = new SqlParameter[9];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[0].Value = accountNumber;
                parmArray[1] = new SqlParameter("@S1", SqlDbType.NVarChar, 1);
                parmArray[1].Value = "";
                parmArray[1].Direction = ParameterDirection.Output;
                parmArray[2] = new SqlParameter("@S2", SqlDbType.NVarChar, 1);
                parmArray[2].Value = "";
                parmArray[2].Direction = ParameterDirection.Output;
                parmArray[3] = new SqlParameter("@DC", SqlDbType.NVarChar, 1);
                parmArray[3].Value = "";
                parmArray[3].Direction = ParameterDirection.Output;
                parmArray[4] = new SqlParameter("@AD", SqlDbType.NVarChar, 1);
                parmArray[4].Value = "";
                parmArray[4].Direction = ParameterDirection.Output;
                parmArray[5] = new SqlParameter("@UW", SqlDbType.NVarChar, 1);
                parmArray[5].Value = "";
                parmArray[5].Direction = ParameterDirection.Output;
                parmArray[6] = new SqlParameter("@appstatus", SqlDbType.NVarChar, 4);
                parmArray[6].Value = "";
                parmArray[6].Direction = ParameterDirection.Output;
                parmArray[7] = new SqlParameter("@manualrefer", SqlDbType.NVarChar, 4);
                parmArray[7].Value = "";
                parmArray[7].Direction = ParameterDirection.Output;
                parmArray[8] = new SqlParameter("@adreqd", SqlDbType.NVarChar, 1);
                parmArray[8].Value = "";
                parmArray[8].Direction = ParameterDirection.Output;

                result = this.RunSP("DN_ProposalGetStatusSP", parmArray);

                if (result == 0)	//populate property variables
                {
                    _s1 = (string)parmArray[1].Value;
                    _s2 = (string)parmArray[2].Value;
                    _dc = (string)parmArray[3].Value;
                    _ad = (string)parmArray[4].Value;
                    _uw = (string)parmArray[5].Value;
                    _appstatus = (string)parmArray[6].Value;
                    _manualrefer = (string)parmArray[7].Value;
                    _adreqd = (string)parmArray[8].Value;
                }
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;
        }

        //IP - CR916 - 10/04/08
        //Passed in the employee number as this is needed when writing the record to 
        //the Proposalaudit table.
        /// <summary>
        /// Updates the customer and proposal tables with the new band
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        /// <param name="newBand"></param>
        public void OverrideBand(SqlConnection conn, SqlTransaction trans, string newBand)
        {
            try
            {
                parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[0].Value = this.AccountNo;
                parmArray[1] = new SqlParameter("@newBand", SqlDbType.NVarChar, 4);
                parmArray[1].Value = newBand;
                parmArray[2] = new SqlParameter("@user", SqlDbType.NVarChar, 1000);
                parmArray[2].Value = this.EmployeeNoChanged;

                this.RunSP(conn, trans, "DN_ProposalOverrideBandSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }
        /// <summary>
        /// updates proposal table settting result and referral reason codes also calculates the scoringband
        /// and updates the customer and proposal tables with this
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        /// <param name="newBand"></param>
        /// <param name="scoretype"></param>
        public void WriteResult(SqlConnection conn, SqlTransaction trans,  string newBand, char scoretype, string AccountBand)
        {
            
            try
            {
                parmArray = new SqlParameter[16];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
                parmArray[0].Value = this.CustomerID;
                parmArray[1] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[1].Value = this.AccountNo;
                parmArray[2] = new SqlParameter("@dateprop", SqlDbType.SmallDateTime);
                parmArray[2].Value = this.DateProp;
                parmArray[3] = new SqlParameter("@points", SqlDbType.SmallInt);
                parmArray[3].Value = this.Points;
                parmArray[4] = new SqlParameter("@propresult", SqlDbType.NChar, 1);
                parmArray[4].Value = this.PropResult;
                parmArray[5] = new SqlParameter("@reason", SqlDbType.NChar, 2);
                parmArray[5].Value = this.Reason;
                parmArray[6] = new SqlParameter("@notes", SqlDbType.NVarChar, 1000);
                parmArray[6].Value = this.ProposalNotes;
                parmArray[7] = new SqlParameter("@user", SqlDbType.NVarChar, 1000);
                parmArray[7].Value = this.EmployeeNoChanged;
                parmArray[8] = new SqlParameter("@reason2", SqlDbType.NChar, 2);
                parmArray[8].Value = this.Reason2;
                parmArray[9] = new SqlParameter("@reason3", SqlDbType.NChar, 2);
                parmArray[9].Value = this.Reason3;
                parmArray[10] = new SqlParameter("@reason4", SqlDbType.NChar, 2);
                parmArray[10].Value = this.Reason4;
                parmArray[11] = new SqlParameter("@reason5", SqlDbType.NChar, 2);
                parmArray[11].Value = this.Reason5;
                parmArray[12] = new SqlParameter("@reason6", SqlDbType.NChar, 2);
                parmArray[12].Value = this.Reason6;
                parmArray[13] = new SqlParameter("@newBand", SqlDbType.NVarChar, 4);
                parmArray[13].Value = newBand;
               //parmArray[13].Direction = ParameterDirection.Output;
                parmArray[14] = new SqlParameter("@scorecard", SqlDbType.Char, 1);
                parmArray[14].Value = scoretype;
                parmArray[15] = new SqlParameter("@AccountBand", SqlDbType.NVarChar, 4);
                parmArray[15].Value = AccountBand;
               
                
                this.RunSP(conn, trans, "DN_ProposalWriteResultSP", parmArray);
                
                //if (parmArray[13].Value != DBNull.Value)
                //    newBand = Convert.ToString(parmArray[13].Value);

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }
        /// <summary>
        /// Writes to the ProposalBS table. This table will store intermediate results for Rescore or for the Parallel run. 
        /// These can later be applied. Also stores old points...
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        /// <param name="newBand"></param>
        /// <param name="scoretype"></param>
        public void WriteResultforBS(SqlConnection conn, SqlTransaction trans, string newBand, char scoretype, decimal newlimit,
            string oldBand, decimal oldlimit)
        {
           // newBand = "";
            try
            {
                parmArray = new SqlParameter[18];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
                parmArray[0].Value = this.CustomerID;
                parmArray[1] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[1].Value = this.AccountNo;
                parmArray[2] = new SqlParameter("@dateprop", SqlDbType.SmallDateTime);
                parmArray[2].Value = this.DateProp;
                parmArray[3] = new SqlParameter("@points", SqlDbType.SmallInt);
                parmArray[3].Value = this.Points;
                parmArray[4] = new SqlParameter("@propresult", SqlDbType.NChar, 1);
                parmArray[4].Value = this.PropResult;
                parmArray[5] = new SqlParameter("@reason", SqlDbType.NChar, 2);
                parmArray[5].Value = this.Reason;
                parmArray[6] = new SqlParameter("@notes", SqlDbType.NVarChar, 1000);
                parmArray[6].Value = this.ProposalNotes;
                parmArray[7] = new SqlParameter("@user", SqlDbType.NVarChar, 1000);
                parmArray[7].Value = this.EmployeeNoChanged;
                parmArray[8] = new SqlParameter("@reason2", SqlDbType.NChar, 2);
                parmArray[8].Value = this.Reason2;
                parmArray[9] = new SqlParameter("@reason3", SqlDbType.NChar, 2);
                parmArray[9].Value = this.Reason3;
                parmArray[10] = new SqlParameter("@reason4", SqlDbType.NChar, 2);
                parmArray[10].Value = this.Reason4;
                parmArray[11] = new SqlParameter("@reason5", SqlDbType.NChar, 2);
                parmArray[11].Value = this.Reason5;
                parmArray[12] = new SqlParameter("@reason6", SqlDbType.NChar, 2);
                parmArray[12].Value = this.Reason6;
                parmArray[13] = new SqlParameter("@newBand", SqlDbType.NVarChar, 4);
                parmArray[13].Value = newBand;
          //      parmArray[13].Direction = ParameterDirection.Output; don't require as calculated in application
                parmArray[14] = new SqlParameter("@scoretype", SqlDbType.Char, 1);
                parmArray[14].Value = scoretype;
                parmArray[15] = new SqlParameter("@newlimit", SqlDbType.Money, 1);
                parmArray[15].Value = newlimit;
                parmArray[16] = new SqlParameter("@oldBand", SqlDbType.VarChar, 4);
                parmArray[16].Value = oldBand;
                parmArray[17] = new SqlParameter("@oldlimit", SqlDbType.Money, 1);
                parmArray[17].Value = oldlimit;

                this.RunSP(conn, trans, "ProposalBSWriteSP", parmArray);
                
                //if (parmArray[13].Value != DBNull.Value)
                //    newBand = Convert.ToString(parmArray[13].Value); don't need as done in application

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }


        public void SetManualRefer(SqlConnection conn, SqlTransaction trans, string customerID, string accountNo, DateTime dateProp, bool isManualRefer, bool cashLoan= false)
        {
            string reason = "";

            try
            {
                if (isManualRefer && cashLoan)
                {
                    reason = "CL";
                }
                else if (isManualRefer)
                {
                    reason = "MN";
                }
                else
                {
                    reason = "SL";                  //IP - 26/03/12 - #8459 - UAT(10)
                    //reason = "LX";
                }
   
                parmArray = new SqlParameter[5];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
                parmArray[0].Value = customerID;
                parmArray[1] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[1].Value = accountNo;
                parmArray[2] = new SqlParameter("@dateprop", SqlDbType.SmallDateTime);
                parmArray[2].Value = dateProp;
                parmArray[3] = new SqlParameter("@empeeno", SqlDbType.Int);
                parmArray[3].Value = this.User;
                parmArray[4] = new SqlParameter("@reason", SqlDbType.NChar, 2);
                parmArray[4].Value = reason;

                if (conn != null && trans != null)
                    this.RunSP(conn, trans, "DN_ProposalManualReferSP", parmArray);
                else
                    this.RunSP("DN_ProposalManualReferSP", parmArray);

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }


        public void SaveReferralNotes(SqlConnection conn,
                                        SqlTransaction trans,
                                        string customerID,
                                        string accountNo,
                                        DateTime dateProp,
                                        string notes,
                                        decimal creditLimit)
        {
            try
            {
                parmArray = new SqlParameter[5];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
                parmArray[0].Value = customerID;
                parmArray[1] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[1].Value = accountNo;
                parmArray[2] = new SqlParameter("@dateprop", SqlDbType.SmallDateTime);
                parmArray[2].Value = dateProp;
                parmArray[3] = new SqlParameter("@notes", SqlDbType.NVarChar, 1000);
                parmArray[3].Value = notes;
                parmArray[4] = new SqlParameter("@creditLimit", SqlDbType.Money);
                parmArray[4].Value = creditLimit;

                this.RunSP(conn, trans, "DN_ProposalSaveReferralNotesSP", parmArray);

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void SaveProposalNotes(SqlConnection conn,
            SqlTransaction trans,
            string customerID,
            string accountNo,
            DateTime dateProp,
            string notes)
        {
            try
            {
                parmArray = new SqlParameter[4];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
                parmArray[0].Value = customerID;
                parmArray[1] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[1].Value = accountNo;
                parmArray[2] = new SqlParameter("@dateprop", SqlDbType.SmallDateTime);
                parmArray[2].Value = dateProp;
                parmArray[3] = new SqlParameter("@notes", SqlDbType.NVarChar, 1000);
                parmArray[3].Value = notes;

                this.RunSP(conn, trans, "DN_ProposalNotesSaveSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void SetPropResult(SqlConnection conn,
            SqlTransaction trans,
            string customerID,
            string accountNo,
            DateTime dateProp,
            string propResult,
            string notes)
        {
            try
            {
                parmArray = new SqlParameter[5];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
                parmArray[0].Value = customerID;
                parmArray[1] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[1].Value = accountNo;
                parmArray[2] = new SqlParameter("@dateprop", SqlDbType.SmallDateTime);
                parmArray[2].Value = dateProp;
                parmArray[3] = new SqlParameter("@notes", SqlDbType.NVarChar, 1000);
                parmArray[3].Value = notes;
                parmArray[4] = new SqlParameter("@propResult", SqlDbType.NChar, 1);
                parmArray[4].Value = propResult;

                this.RunSP(conn, trans, "DN_ProposalSetResultSP", parmArray);

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void AuditPropResult(
            SqlConnection conn,
            SqlTransaction trans,
            string customerID,
            string accountNo,
            DateTime dateProp,
            int empeeNo)
        {
            try
            {
                parmArray = new SqlParameter[4];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.VarChar, 20);
                parmArray[0].Value = customerID;
                parmArray[1] = new SqlParameter("@acctno", SqlDbType.Char, 12);
                parmArray[1].Value = accountNo;
                parmArray[2] = new SqlParameter("@dateprop", SqlDbType.SmallDateTime);
                parmArray[2].Value = dateProp;
                parmArray[3] = new SqlParameter("@empeeno", SqlDbType.Int);
                parmArray[3].Value = empeeNo;

                this.RunSP(conn, trans, "DN_ProposalAuditResultSP", parmArray);

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public DataTable GetProposalsToRescore(char scoretype)
        {
            DataTable dt = null;
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@scoretype", SqlDbType.Char, 1);
                parmArray[0].Value = scoretype;
                dt = new DataTable(TN.ReScore);
                this.RunSP("DN_ProposalGetRescoresSP", parmArray,dt);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dt;
        }

        public DataTable GetDocConfirmationData()
        {
            DataTable dt = null;
            try
            {
                dt = new DataTable(TN.DocConfirmation);
                parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[0].Value = this.AccountNo;
                parmArray[1] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
                parmArray[1].Value = this.CustomerID;
                parmArray[2] = new SqlParameter("@dateprop", SqlDbType.SmallDateTime);
                parmArray[2].Value = this.DateProp;

                this.RunSP("DN_ProposalGetDocConfirmationDataSP", parmArray, dt);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dt;
        }

        public void SaveDocConfirmation(SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                parmArray = new SqlParameter[12];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
                parmArray[0].Value = this.CustomerID;
                parmArray[1] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[1].Value = this.AccountNo;
                parmArray[2] = new SqlParameter("@dateprop", SqlDbType.SmallDateTime);
                parmArray[2].Value = this.DateProp;
                parmArray[3] = new SqlParameter("@proofID", SqlDbType.NChar, 4);
                parmArray[3].Value = this.ProofOfID;
                parmArray[4] = new SqlParameter("@proofAddress", SqlDbType.NChar, 4);
                parmArray[4].Value = this.ProofOfAddress;
                parmArray[5] = new SqlParameter("@proofIncome", SqlDbType.NChar, 4);
                parmArray[5].Value = this.ProofOfIncome;
                parmArray[6] = new SqlParameter("@proofOfBank", SqlDbType.NChar, 4);            //IP - 14/12/10 - Store Card
                parmArray[6].Value = this.ProofOfBank;
                parmArray[7] = new SqlParameter("@dctext1", SqlDbType.NVarChar, 350);
                parmArray[7].Value = this.DCText1;
                parmArray[8] = new SqlParameter("@dctext2", SqlDbType.NVarChar, 350);
                parmArray[8].Value = this.DCText2;
                parmArray[9] = new SqlParameter("@dctext3", SqlDbType.NVarChar, 350);
                parmArray[9].Value = this.DCText3;
                parmArray[10] = new SqlParameter("@proofOfBankTxt", SqlDbType.NVarChar, 350);    //IP - 14/12/10 - Store Card
                parmArray[10].Value = this.ProofOfBankTxt;
                parmArray[11] = new SqlParameter("@empeeno", SqlDbType.NVarChar, 350);
                parmArray[11].Value = this.EmployeeNoChanged;


                this.RunSP(conn, trans, "DN_ProposalSaveDocConfirmationSP", parmArray);

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public bool CloneProposal(SqlConnection conn, SqlTransaction trans, string customerID, string accountNo, out bool cloned)
        {
            cloned = false;
            bool rescore = false;
            try
            {
                parmArray = new SqlParameter[4];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
                parmArray[0].Value = customerID;
                parmArray[1] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[1].Value = accountNo;
                parmArray[2] = new SqlParameter("@rescore", SqlDbType.Bit);
                parmArray[2].Value = 0;
                parmArray[2].Direction = ParameterDirection.Output;
                parmArray[3] = new SqlParameter("@cloned", SqlDbType.Bit);
                parmArray[3].Direction = ParameterDirection.Output;

                if (conn != null && trans != null)
                    this.RunSP(conn, trans, "DN_ProposalCloneSP", parmArray);
                else
                    this.RunSP("DN_ProposalCloneSP", parmArray);

                if (parmArray[2].Value != DBNull.Value)
                    rescore = Convert.ToBoolean(parmArray[2].Value);
                cloned = Convert.ToBoolean(parmArray[3].Value);

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return rescore;
        }

        public void GetUnclearedStage(SqlConnection conn, SqlTransaction trans, string accountNo, ref string newAccount, ref string checkType, ref DateTime dateProp, ref string propResult, ref int points)
        {
            try
            {
                parmArray = new SqlParameter[6];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[0].Value = accountNo;
                parmArray[1] = new SqlParameter("@checktype", SqlDbType.NVarChar, 4);
                parmArray[1].Value = checkType;
                parmArray[1].Direction = ParameterDirection.Output;
                parmArray[2] = new SqlParameter("@dateprop", SqlDbType.SmallDateTime);
                parmArray[2].Value = dateProp;
                parmArray[2].Direction = ParameterDirection.Output;
                parmArray[3] = new SqlParameter("@acctnoOut", SqlDbType.NVarChar, 12);
                parmArray[3].Value = newAccount;
                parmArray[3].Direction = ParameterDirection.Output;
                parmArray[4] = new SqlParameter("@propresult", SqlDbType.NVarChar, 12);
                parmArray[4].Value = propResult;
                parmArray[4].Direction = ParameterDirection.Output;
                parmArray[5] = new SqlParameter("@points", SqlDbType.Int);
                parmArray[5].Value = points;
                parmArray[5].Direction = ParameterDirection.Output;


                if (conn != null && trans != null)
                    this.RunSP(conn, trans, "DN_ProposalFlagGetUnclearedStageSP", parmArray);
                else
                    this.RunSP("DN_ProposalFlagGetUnclearedStageSP", parmArray);
                if (parmArray[1].Value != DBNull.Value)
                    checkType = (string)parmArray[1].Value;
                if (parmArray[2].Value != DBNull.Value)
                    dateProp = (DateTime)parmArray[2].Value;
                if (parmArray[3].Value != DBNull.Value)
                    newAccount = (string)parmArray[3].Value;
                if (parmArray[4].Value != DBNull.Value)
                    propResult = (string)parmArray[4].Value;
                if (parmArray[5].Value != DBNull.Value)
                    points = (int)parmArray[5].Value;

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public DataTable GetReferralSummaryData(string accountNo, string customerID, string accountType, DateTime dateProp)
        {
            DataTable dt = null;
            try
            {
                dt = new DataTable(TN.ReferralData);
                parmArray = new SqlParameter[4];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[0].Value = accountNo;
                parmArray[1] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
                parmArray[1].Value = customerID;
                parmArray[2] = new SqlParameter("@accttype", SqlDbType.NVarChar, 1);
                parmArray[2].Value = accountType;
                parmArray[3] = new SqlParameter("@dateprop", SqlDbType.DateTime);
                parmArray[3].Value = dateProp;

                this.RunSP("DN_ProposalGetReferralSummaryDataSP", parmArray, dt);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dt;
        }

        public DataTable GetPreviousDocConfirmationData()
        {
            DataTable dt = null;
            try
            {
                dt = new DataTable(TN.PrevDocConf);
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[0].Value = this.AccountNo;
                parmArray[1] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
                parmArray[1].Value = this.CustomerID;

                this.RunSP("DN_ProposalGetPreviousDocConfSP", parmArray, dt);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dt;
        }

        public DataTable GetNonRFProposals()
        {
            DataTable dt = null;
            try
            {
                dt = new DataTable(TN.NonRFProposals);
                this.RunSP("DN_GetNonRFProposals", dt);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dt;
        }

        public void GetDatePropForAccount(SqlConnection conn, SqlTransaction trans,
                                    string acctNo, string custID, ref DateTime dateProp)
        {
            try
            {
                parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[0].Value = acctNo;
                parmArray[1] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
                parmArray[1].Value = custID;
                parmArray[2] = new SqlParameter("@dateprop", SqlDbType.SmallDateTime);
                parmArray[2].Value = dateProp;
                parmArray[2].Direction = ParameterDirection.Output;

                this.RunSP(conn, trans, "DN_ProposalGetDatePropSP", parmArray);

                if (parmArray[2].Value != DBNull.Value)
                    dateProp = (DateTime)parmArray[2].Value;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void GetDetailsForRescore(SqlConnection conn, SqlTransaction trans, string acctNo, string custID, 
                                         ref DateTime dateLastScored, ref string highStatus)
        {
            try
            {
                parmArray = new SqlParameter[4];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[0].Value = acctNo;
                parmArray[1] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
                parmArray[1].Value = custID;
                parmArray[2] = new SqlParameter("@datelastscored", SqlDbType.DateTime);
                parmArray[2].Value = dateLastScored;
                parmArray[2].Direction = ParameterDirection.Output;
                parmArray[3] = new SqlParameter("@highstatus", SqlDbType.NVarChar, 2);
                parmArray[3].Value = highStatus;
                parmArray[3].Direction = ParameterDirection.Output;

                this.RunSP(conn, trans, "DN_ProposalGetDetailsForRescoreSP", parmArray);

                if (parmArray[2].Value != DBNull.Value)
                    dateLastScored = (DateTime)parmArray[2].Value;

                if (parmArray[3].Value != DBNull.Value)
                    highStatus = (string)parmArray[3].Value;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// determines which scorecard to use A for Applicant B for Behavioural and P for parallel run (i.e. both )
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        /// <param name="acctNo"></param>
        /// <param name="custID"></param>
        /// <param name="scorecard"></param>
        public void ScoreTypetoUse(SqlConnection conn, SqlTransaction trans, string acctNo, string custID,out char scorecard )
        {
            try
            {
                scorecard = ' ';

                parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[0].Value = acctNo;
                parmArray[1] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
                parmArray[1].Value = custID;
                parmArray[2] = new SqlParameter("@scorecard", SqlDbType.Char,1);
                parmArray[2].Value = scorecard;
                parmArray[2].Direction = ParameterDirection.Output;
                this.RunSP(conn, trans, "ScoreTypeToUse", parmArray);

                if (parmArray[2].Value != DBNull.Value)
                    scorecard = Convert.ToChar(parmArray[2].Value);
                
                    
                
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }


        public DataTable LoadBSCustomers(string category, int runno )
        {
            DataTable dt = null;
            try
            {
                dt = new DataTable(TN.DocConfirmation);
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@category", SqlDbType.NVarChar, 32);
                parmArray[0].Value = category;
                parmArray[1] = new SqlParameter("@runno", SqlDbType.NVarChar, 20);
                parmArray[1].Value = runno;
                
                this.RunSP("ProposalBSLoadCustomers", parmArray, dt);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dt;
        }

        /// <summary>
        /// Marks rescore as being applied for each custid and run 
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        /// <param name="custID"></param>
        /// <param name="runno"></param>
        public void ApplyBSRescore(SqlConnection conn, SqlTransaction trans, string custID,
                                         int runno, int user)
        {
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
                parmArray[0].Value = custID;
                parmArray[1] = new SqlParameter("@runno", SqlDbType.Int);
                parmArray[1].Value = runno;
              
                
                
                this.RunSP(conn, trans, "ProposalBSApply", parmArray);

                
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void ApplyLatestBSRescoreForRun(SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                this.RunSP(conn, trans, "ProposalBSApplyRescore");
                
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }
        /// <summary>
        /// Saves details to the CustomerScore Hist Table -- also updates the customer table with the score card.... 
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        /// <param name="reasonchanged"></param>
        public void SaveScoreHist(SqlConnection conn, SqlTransaction trans,
            string reasonchanged,char? scorecard )
        {
            try
            {
                parmArray = new SqlParameter[9];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
                parmArray[0].Value = this.CustomerID;
                parmArray[1] = new SqlParameter("@dateprop", SqlDbType.SmallDateTime);
                parmArray[1].Value = this.DateProp;
                parmArray[2] = new SqlParameter("@scorecard", SqlDbType.Char,1);
                if (scorecard.HasValue)
                {
                    parmArray[2].Value = scorecard;
                }
                else
                {
                    parmArray[2].Value = DBNull.Value;
                }
                parmArray[3] = new SqlParameter("@points", SqlDbType.Int, 4);
                if (scorecard.HasValue)
                {
                    parmArray[3].Value = Points;
                }
                else
                {
                    parmArray[3].Value = DBNull.Value;
                }
                parmArray[4] = new SqlParameter("@creditlimit", SqlDbType.Money, 4);
                parmArray[4].Value = this.creditawarded;
                parmArray[5] = new SqlParameter("@band", SqlDbType.Char, 2);
                parmArray[5].Value = this.ScoringBand;
                parmArray[6] = new SqlParameter("@empeeno", SqlDbType.Int,4);
                parmArray[6].Value = this.EmployeeNoChanged;
                parmArray[7] = new SqlParameter("@reasonchanged", SqlDbType.VarChar, 12);
                parmArray[7].Value = reasonchanged; 
                parmArray[8] = new SqlParameter("@acctno", SqlDbType.Char, 12);
                parmArray[8].Value = this.AccountNo;

                this.RunSP(conn, trans, "CustomerScoreHistSave", parmArray);

                /*Custid varchar(20), Dateprop, Scorecard char(1), Points smallint, 
                 * CreditLimit money, Band char(1), DateChange datetime , 
                 * Empeeno int , ReasonChanged varchar(5) ,*/
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public DataTable LoadScoreHistforCustomer( )
        {
            DataTable dt = null;
            try
            {
                dt = new DataTable();
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.VarChar, 20);
                parmArray[0].Value = this.CustomerID;

                this.RunSP("CustomerScoreHistLoadforCustomer", parmArray, dt);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dt;
        }

        public DataSet ScoreBandLoadData(SqlConnection conn, SqlTransaction trans)
        {
            DataSet DS = null;
            try
            {
                DS = new DataSet();
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.VarChar, 20);
                parmArray[0].Value = this.CustomerID;
                parmArray[1] = new SqlParameter("@acctno", SqlDbType.Char, 12);
                parmArray[1].Value = this.AccountNo;


                this.RunSP(conn,trans,"ScoreBandLoadData", parmArray, DS);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return DS;
        }

        


        public DProposal()
        {
            _s1 = "";
            _s2 = "";
            _dc = "";
            _ad = "";
            _uw = "";
            _appstatus = "";
            _manualrefer = "";
            _adreqd = "";
        }


    }
}
