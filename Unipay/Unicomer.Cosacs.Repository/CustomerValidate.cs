using System;
using System.Collections.Generic;
using System.Data;

namespace Unicomer.Cosacs.Repository
{
    #region  CustomerValidate    
    public partial class CustomerValidate : Blue.Transactions.Command<ContextBase>
    {
        public CustomerValidate() : base("dbo.ValidateCustomer")
        {
            base.AddInParameter("@IdNumber", DbType.String);
            base.AddInParameter("@IdType", DbType.String);
            base.AddInParameter("@PhoneNumber", DbType.String);
        }
    }

    partial class CustomerValidate
    {
        public new void Fill(DataSet ds)
        {
            base.Fill(ds);
        }

        public void Fill(DataSet ds, string _IdNumber, string _IdType, string _PhNumber)
        {
            this.IdNumber = _IdNumber;
            this.IdType = _IdType;
            this.PhoneNumber = _PhNumber;
            Fill(ds);

        }

        public string IdNumber
        {
            get { return (string)base[0]; }
            set { base[0] = value; }
        }

        public string IdType
        {
            get { return (string)base[1]; }
            set { base[1] = value; }
        }

        public string PhoneNumber
        {
            get { return (string)base[2]; }
            set { base[2] = value; }
        }
    }
    #endregion

    #region CustomerInsertRepository 
    public partial class CustomerInsertRepository : Blue.Transactions.Command<ContextBase>
    {
        public CustomerInsertRepository() : base("dbo.CustomerDetailsSave")
        {
            base.AddInParameter("@UnipayId", DbType.String);
            base.AddInParameter("@FirstName", DbType.String);
            base.AddInParameter("@LastName", DbType.String);
            base.AddInParameter("@EmailId", DbType.String);
            base.AddInParameter("@PhoneNumber", DbType.String);
            base.AddInParameter("@CustId", DbType.String);
            base.AddInParameter("@DateBorn", DbType.DateTime);
            base.AddInParameter("@Origbr", DbType.Int16);
            base.AddInParameter("@OtherId", DbType.String);
            base.AddInParameter("@BranchNohdle", DbType.Int16);
            base.AddInParameter("@Title", DbType.String);
            base.AddInParameter("@Alias", DbType.String);
            base.AddInParameter("@AddrSort", DbType.String);
            base.AddInParameter("@NameSort", DbType.String);
            base.AddInParameter("@Sex", DbType.String);
            base.AddInParameter("@EthniCity", DbType.String);
            base.AddInParameter("@MoreRewardsNo", DbType.String);
            base.AddInParameter("@EffectiveDate", DbType.DateTime);
            base.AddInParameter("@IDType", DbType.String);
            base.AddInParameter("@IDNumber", DbType.String);
            base.AddInParameter("@UserNo", DbType.Int32);
            base.AddInParameter("@DateChanged", DbType.DateTime);
            base.AddInParameter("@MaidenName", DbType.String);
            base.AddInParameter("@StoreType", DbType.String);
            base.AddInParameter("@Dependants", DbType.Int32);
            base.AddInParameter("@MaritalStat", DbType.String);
            base.AddInParameter("@Nationality", DbType.String);
            base.AddInParameter("@ResieveSms", DbType.Boolean);
            base.AddInParameter("@AddressType", DbType.String);
            base.AddInParameter("@CusAddr1", DbType.String);
            base.AddInParameter("@CusAddr2", DbType.String);
            base.AddInParameter("@CusAddr3", DbType.String);
            base.AddInParameter("@NewRecord", DbType.Boolean);
            base.AddInParameter("@DeliveryArea", DbType.String);
            base.AddInParameter("@PostCode", DbType.String);
            base.AddInParameter("@Notes", DbType.String);
            base.AddInParameter("@DateIn", DbType.DateTime);
            base.AddInParameter("@User", DbType.Int32);
            base.AddInParameter("@Zone", DbType.String);
            base.AddInParameter("@DateTelAdd", DbType.DateTime);
            base.AddInParameter("@ExtnNo", DbType.String);
            base.AddInParameter("@TelLocn", DbType.String);
            base.AddInParameter("@DialCode", DbType.String);
            base.AddInParameter("@EmpeeNoChange", DbType.Int32);
            base.AddOutParameter("@ReturnCustId", DbType.String, 20);
            base.AddOutParameter("@Message", DbType.String, 5000);
            base.AddOutParameter("@StatusCode", DbType.Int32, 32);
        }
    }

    partial class CustomerInsertRepository
    {
        public List<string> InsertCustomer()
        {
            List<string> returnCustId = new List<string>();// string.Empty;
            int RecordCount = base.ExecuteNonQuery();
            returnCustId.Add(this.ReturnCustId);
            returnCustId.Add(this.Message);
            returnCustId.Add(this.StatusCode.ToString());
            return returnCustId; // this.ReturnCustId==""?"Message:"+this.Message:"CustID:"+this.ReturnCustId;
        }

        public List<string> InsertCustomer(string _UnipayId,
                                    string _FirstName,
                                    string _LastName,
                                    string _EmailId,
                                    string _PhoneNumber,
                                    string _CustID,
                                    DateTime? _DateBorn,
                                    Int16? _Origbr,
                                    string _OtherId,
                                    Int16 _BranchNohdle,
                                    string _Title,
                                    string _Alias,
                                    string _AddrSort,
                                    string _NameSort,
                                    string _Sex,
                                    string _EthniCity,
                                    string _MoreRewardsNo,
                                    DateTime? _EffectiveDate,
                                    string _IDType,
                                    string _IDNumber,
                                    int? _UserNo,
                                    DateTime? _DateChanged,
                                    string _MaidenName,
                                    string _StoreType,
                                    Int32? _Dependants,
                                    string _MaritalStat,
                                    string _nationality,
                                    Boolean? _ResieveSms,
                                    string _AddressType,
                                    string _CusAddr1,
                                    string _CusAddr2,
                                    string _CusAddr3,
                                    Boolean _newRecord,
                                    string _deliveryArea,
                                    string _postCode,
                                    string _notes,
                                    DateTime? _dateIn,
                                    Int32? _user,
                                    string _zone,
                                    DateTime? _dateTelAdd,
                                    string _extnNo,
                                    string _telLocn,
                                    string _dialCode,
                                    Int32? _empeeNoChange,
                                    string _ReturnCustId,
                                    string _Message,
                                    int _StatusCode)
        {
            this.UnipayId = _UnipayId;
            this.FirstName = _FirstName;
            this.LastName = _LastName;
            this.EmailId = _EmailId;
            this.PhoneNumber = _PhoneNumber;
            this.CustId = _CustID;
            this.DateBorn = _DateBorn;
            this.Origbr = _Origbr;
            this.OtherId = _OtherId;
            this.BranchNohdle = _BranchNohdle;
            this.Title = _Title;
            this.Alias = _Alias;
            this.AddrSort = _AddrSort;
            this.NameSort = _NameSort;
            this.Sex = _Sex;
            this.EthniCity = _EthniCity;
            this.MoreRewardsNo = _MoreRewardsNo;
            this.EffectiveDate = _EffectiveDate;
            this.IDType = _IDType;
            this.IDNumber = _IDNumber;
            this.UserNo = _UserNo;
            this.DateChanged = _DateChanged;
            this.MaidenName = _MaidenName;
            this.StoreType = _StoreType;
            this.Dependants = _Dependants;
            this.MaritalStat = _MaritalStat;
            this.Nationality = _nationality;
            this.ResieveSms = _ResieveSms;
            this.AddressType = _AddressType;
            this.CusAddr1 = _CusAddr1;
            this.CusAddr2 = _CusAddr2;
            this.CusAddr3 = _CusAddr3;
            this.NewRecord = _newRecord;
            this.DeliveryArea = _deliveryArea;
            this.PostCode = _postCode;
            this.Notes = _notes;
            this.DateIn = _dateIn;
            this.User = _user;
            this.Zone = _zone;
            this.DateTelAdd = _dateTelAdd;
            this.ExtnNo = _extnNo;
            this.TelLocn = _telLocn;
            this.DialCode = _dialCode;
            this.EmpeeNoChange = _empeeNoChange;
            this.ReturnCustId = _ReturnCustId;
            this.Message = _Message;
            this.StatusCode = _StatusCode;
            List<string> ReturnCustId = InsertCustomer();
            return ReturnCustId;

        }

        #region "properties"
        public string UnipayId
        {
            get { return (string)base[0]; }
            set { base[0] = value; }
        }
        public string FirstName
        {
            get { return (string)base[1]; }
            set { base[1] = value; }
        }
        public string LastName
        {
            get { return (string)base[2]; }
            set { base[2] = value; }
        }
        public string EmailId
        {
            get { return (string)base[3]; }
            set { base[3] = value; }
        }
        public string PhoneNumber
        {
            get { return (string)base[4]; }
            set { base[4] = value; }
        }
        public string CustId
        {
            get { return (string)base[5]; }
            set { base[5] = value; }
        }
        public DateTime? DateBorn
        {
            get { return (DateTime)base[6]; }
            set { base[6] = value; }
        }
        public Int16? Origbr
        {
            get { return (Int16?)base[7]; }
            set { base[7] = value; }
        }
        public string OtherId
        {
            get { return (string)base[8]; }
            set { base[8] = value; }
        }
        public Int16 BranchNohdle
        {
            get { return (Int16)base[9]; }
            set { base[9] = value; }
        }
        public string Title
        {
            get { return (string)base[10]; }
            set { base[10] = value; }
        }
        public string Alias
        {
            get { return (string)base[11]; }
            set { base[11] = value; }
        }
        public string AddrSort
        {
            get { return (string)base[12]; }
            set { base[12] = value; }
        }
        public string NameSort
        {
            get { return (string)base[13]; }
            set { base[13] = value; }
        }
        public string Sex
        {
            get { return (string)base[14]; }
            set { base[14] = value; }
        }
        public string EthniCity
        {
            get { return (string)base[15]; }
            set { base[15] = value; }
        }
        public string MoreRewardsNo
        {
            get { return (string)base[16]; }
            set { base[16] = value; }
        }
        public DateTime? EffectiveDate
        {
            get { return (DateTime?)base[17]; }
            set { base[17] = value; }
        }
        public string IDType
        {
            get { return (string)base[18]; }
            set { base[18] = value; }
        }
        public string IDNumber
        {
            get { return (string)base[19]; }
            set { base[19] = value; }
        }
        public Int32? UserNo
        {
            get { return (Int32?)base[20]; }
            set { base[20] = value; }
        }
        public DateTime? DateChanged
        {
            get { return (DateTime?)base[21]; }
            set { base[21] = value; }
        }
        public string MaidenName
        {
            get { return (string)base[22]; }
            set { base[22] = value; }
        }
        public string StoreType
        {
            get { return (string)base[23]; }
            set { base[23] = value; }
        }
        public Int32? Dependants
        {
            get { return (Int32?)base[24]; }
            set { base[24] = value; }
        }
        public string MaritalStat
        {
            get { return (string)base[25]; }
            set { base[25] = value; }
        }
        public string Nationality

        {
            get { return (string)base[26]; }
            set { base[26] = value; }
        }
        public bool? ResieveSms
        {
            get { return (bool?)base[27]; }
            set { base[27] = value; }
        }
        public string AddressType
        {
            get { return (string)base[28]; }
            set { base[28] = value; }
        }
        public string CusAddr1
        {
            get { return (string)base[29]; }
            set { base[29] = value; }
        }
        public string CusAddr2
        {
            get { return (string)base[30]; }
            set { base[30] = value; }
        }
        public string CusAddr3
        {
            get { return (string)base[31]; }
            set { base[31] = value; }
        }
        public bool NewRecord
        {
            get { return (bool)base[32]; }
            set { base[32] = value; }
        }
        public string DeliveryArea
        {
            get { return (string)base[33]; }
            set { base[33] = value; }

        }
        public string PostCode
        {
            get { return (string)base[34]; }
            set { base[34] = value; }
        }
        public string Notes
        {
            get { return (string)base[35]; }
            set { base[35] = value; }
        }
        public DateTime? DateIn
        {
            get { return (DateTime)base[36]; }
            set { base[36] = value; }
        }
        public Int32? User
        {
            get { return (Int32)base[37]; }
            set { base[37] = value; }
        }
        public string Zone
        {
            get { return (string)base[38]; }
            set { base[38] = value; }
        }
        public DateTime? DateTelAdd
        {
            get { return (DateTime?)base[39]; }
            set { base[39] = value; }
        }
        public string ExtnNo
        {
            get { return (string)base[40]; }
            set { base[40] = value; }
        }
        public string TelLocn
        {
            get { return (string)base[41]; }
            set { base[41] = value; }
        }
        public string DialCode
        {
            get { return (string)base[42]; }
            set { base[42] = value; }
        }
        public Int32? EmpeeNoChange
        {
            get { return (Int32?)base[43]; }
            set { base[43] = value; }

        }
        public string ReturnCustId
        {
            get { return (string)base[44]; }
            set { base[44] = value; }
        }
        public string Message
        {
            get { return (string)base[45]; }
            set { base[45] = value; }
        }
        public int StatusCode
        {
            get { return (int)base[46]; }
            set { base[46] = value; }
        }
        #endregion

    }
    #endregion

    #region SecurityAnswerRepository 
    public partial class SecurityAnswerRepository : Blue.Transactions.Command<ContextBase>
    {
        public SecurityAnswerRepository() : base("dbo.GetAuthQAndA")
        {

            base.AddInParameter("@CustId", DbType.String);

        }
    }

    partial class SecurityAnswerRepository
    {
        public void GetAuthQAndA(DataSet ds)
        {
            base.Fill(ds);
        }

        public void GetAuthQAndA(DataSet ds, string _CustID)
        {

            this.CustId = _CustID;
            GetAuthQAndA(ds);

        }

        #region "properties"

        public string CustId
        {
            get { return (string)base[0]; }
            set { base[0] = value; }
        }
        #endregion

    }
    #endregion   

    #region CustomerUpdateRepository 
    public partial class CustomerUpdateRepository : Blue.Transactions.Command<ContextBase>
    {
        public CustomerUpdateRepository() : base("dbo.CustomerDetailsSave")
        {
            base.AddInParameter("@UnipayId", DbType.String);
            base.AddInParameter("@FirstName", DbType.String);
            base.AddInParameter("@LastName", DbType.String);
            base.AddInParameter("@EmailId", DbType.String);
            base.AddInParameter("@PhoneNumber", DbType.String);
            base.AddInParameter("@CustId", DbType.String);
            base.AddInParameter("@DateBorn", DbType.DateTime);
            base.AddInParameter("@Origbr", DbType.Int16);
            base.AddInParameter("@OtherId", DbType.String);
            base.AddInParameter("@BranchNohdle", DbType.Int16);
            base.AddInParameter("@Title", DbType.String);
            base.AddInParameter("@Alias", DbType.String);
            base.AddInParameter("@AddrSort", DbType.String);
            base.AddInParameter("@NameSort", DbType.String);
            base.AddInParameter("@Sex", DbType.String);
            base.AddInParameter("@EthniCity", DbType.String);
            base.AddInParameter("@MoreRewardsNo", DbType.String);
            base.AddInParameter("@EffectiveDate", DbType.DateTime);
            base.AddInParameter("@IDType", DbType.String);
            base.AddInParameter("@IDNumber", DbType.String);
            base.AddInParameter("@UserNo", DbType.Int32);
            base.AddInParameter("@DateChanged", DbType.DateTime);
            base.AddInParameter("@MaidenName", DbType.String);
            base.AddInParameter("@StoreType", DbType.String);
            base.AddInParameter("@Dependants", DbType.Int32);
            base.AddInParameter("@MaritalStat", DbType.String);
            base.AddInParameter("@Nationality", DbType.String);
            base.AddInParameter("@ResieveSms", DbType.Boolean);
            base.AddInParameter("@AddressType", DbType.String);
            base.AddInParameter("@CusAddr1", DbType.String);
            base.AddInParameter("@CusAddr2", DbType.String);
            base.AddInParameter("@CusAddr3", DbType.String);
            base.AddInParameter("@NewRecord", DbType.Boolean);
            base.AddInParameter("@DeliveryArea", DbType.String);
            base.AddInParameter("@PostCode", DbType.String);
            base.AddInParameter("@Notes", DbType.String);
            base.AddInParameter("@DateIn", DbType.DateTime);
            base.AddInParameter("@User", DbType.Int32);
            base.AddInParameter("@Zone", DbType.String);
            base.AddInParameter("@DateTelAdd", DbType.DateTime);
            base.AddInParameter("@ExtnNo", DbType.String);
            base.AddInParameter("@TelLocn", DbType.String);
            base.AddInParameter("@DialCode", DbType.String);
            base.AddInParameter("@EmpeeNoChange", DbType.Int32);
            base.AddOutParameter("@ReturnCustId", DbType.String, 20);
            base.AddOutParameter("@Message", DbType.String, 5000);
            base.AddOutParameter("@StatusCode", DbType.String, 20);
        }
    }

    partial class CustomerUpdateRepository
    {
        public List<string> UpdateUser()
        {
            List<string> returnCustId = new List<string>();
            int RecordCount = base.ExecuteNonQuery();
            returnCustId.Add(this.ReturnCustId);
            returnCustId.Add(this.Message);
            returnCustId.Add(this.StatusCode);
            return returnCustId;
        }

        public List<string> UpdateUser(string _UnipayId,
                                    string _FirstName,
                                    string _LastName,
                                    string _EmailId,
                                    string _PhoneNumber,
                                    string _CustID,
                                    DateTime _DateBorn,
                                    Int16? _Origbr,
                                    string _OtherId,
                                    Int16 _BranchNohdle,
                                    string _Title,
                                    string _Alias,
                                    string _AddrSort,
                                    string _NameSort,
                                    string _Sex,
                                    string _EthniCity,
                                    string _MoreRewardsNo,
                                    DateTime? _EffectiveDate,
                                    string _IDType,
                                    string _IDNumber,
                                    int? _UserNo,
                                    DateTime? _DateChanged,
                                    string _MaidenName,
                                    string _StoreType,
                                    Int32? _Dependants,
                                    string _MaritalStat,
                                    string _nationality,
                                    Boolean? _ResieveSms,
                                    string _AddressType,
                                    string _CusAddr1,
                                    string _CusAddr2,
                                    string _CusAddr3,
                                    Boolean _newRecord,
                                    string _deliveryArea,
                                    string _postCode,
                                    string _notes,
                                    DateTime _dateIn,
                                    Int32? _user,
                                    string _zone,
                                    DateTime? _dateTelAdd,
                                    string _extnNo,
                                    string _telLocn,
                                    string _dialCode,
                                    Int32? _empeeNoChange,
                                    string _ReturnCustId,
                                    string _Message,
                                    string _StatusCode
            )
        {
            this.UnipayId = _UnipayId;
            this.FirstName = _FirstName;
            this.LastName = _LastName;
            this.EmailId = _EmailId;
            this.PhoneNumber = _PhoneNumber;
            this.CustId = _CustID;
            this.DateBorn = _DateBorn;
            this.Origbr = _Origbr;
            this.OtherId = _OtherId;
            this.BranchNohdle = _BranchNohdle;
            this.Title = _Title;
            this.Alias = _Alias;
            this.AddrSort = _AddrSort;
            this.NameSort = _NameSort;
            this.Sex = _Sex;
            this.EthniCity = _EthniCity;
            this.MoreRewardsNo = _MoreRewardsNo;
            this.EffectiveDate = _EffectiveDate;
            this.IDType = _IDType;
            this.IDNumber = _IDNumber;
            this.UserNo = _UserNo;
            this.DateChanged = _DateChanged;
            this.MaidenName = _MaidenName;
            this.StoreType = _StoreType;
            this.Dependants = _Dependants;
            this.MaritalStat = _MaritalStat;
            this.Nationality = _nationality;
            this.ResieveSms = _ResieveSms;
            this.AddressType = _AddressType;
            this.CusAddr1 = _CusAddr1;
            this.CusAddr2 = _CusAddr2;
            this.CusAddr3 = _CusAddr3;
            this.NewRecord = _newRecord;
            this.DeliveryArea = _deliveryArea;
            this.PostCode = _postCode;
            this.Notes = _notes;
            this.DateIn = _dateIn;
            this.User = _user;
            this.Zone = _zone;
            this.DateTelAdd = _dateTelAdd;
            this.ExtnNo = _extnNo;
            this.TelLocn = _telLocn;
            this.DialCode = _dialCode;
            this.EmpeeNoChange = _empeeNoChange;
            this.ReturnCustId = _ReturnCustId;
            this.Message = _Message;
            this.StatusCode = _StatusCode;
            List<string> ReturnCustId = UpdateUser();
            return ReturnCustId;

        }

        #region "properties"
        public string UnipayId
        {
            get { return (string)base[0]; }
            set { base[0] = value; }
        }
        public string FirstName
        {
            get { return (string)base[1]; }
            set { base[1] = value; }
        }
        public string LastName
        {
            get { return (string)base[2]; }
            set { base[2] = value; }
        }
        public string EmailId
        {
            get { return (string)base[3]; }
            set { base[3] = value; }
        }
        public string PhoneNumber
        {
            get { return (string)base[4]; }
            set { base[4] = value; }
        }
        public string CustId
        {
            get { return (string)base[5]; }
            set { base[5] = value; }
        }
        public DateTime DateBorn
        {
            get { return (DateTime)base[6]; }
            set { base[6] = value; }
        }
        public Int16? Origbr
        {
            get { return (Int16?)base[7]; }
            set { base[7] = value; }
        }
        public string OtherId
        {
            get { return (string)base[8]; }
            set { base[8] = value; }
        }
        public Int16 BranchNohdle
        {
            get { return (Int16)base[9]; }
            set { base[9] = value; }
        }
        public string Title
        {
            get { return (string)base[10]; }
            set { base[10] = value; }
        }
        public string Alias
        {
            get { return (string)base[11]; }
            set { base[11] = value; }
        }
        public string AddrSort
        {
            get { return (string)base[12]; }
            set { base[12] = value; }
        }
        public string NameSort
        {
            get { return (string)base[13]; }
            set { base[13] = value; }
        }
        public string Sex
        {
            get { return (string)base[14]; }
            set { base[14] = value; }
        }
        public string EthniCity
        {
            get { return (string)base[15]; }
            set { base[15] = value; }
        }
        public string MoreRewardsNo
        {
            get { return (string)base[16]; }
            set { base[16] = value; }
        }
        public DateTime? EffectiveDate
        {
            get { return (DateTime?)base[17]; }
            set { base[17] = value; }
        }
        public string IDType
        {
            get { return (string)base[18]; }
            set { base[18] = value; }
        }
        public string IDNumber
        {
            get { return (string)base[19]; }
            set { base[19] = value; }
        }
        public Int32? UserNo
        {
            get { return (Int32?)base[20]; }
            set { base[20] = value; }
        }
        public DateTime? DateChanged
        {
            get { return (DateTime?)base[21]; }
            set { base[21] = value; }
        }
        public string MaidenName
        {
            get { return (string)base[22]; }
            set { base[22] = value; }
        }
        public string StoreType
        {
            get { return (string)base[23]; }
            set { base[23] = value; }
        }
        public Int32? Dependants
        {
            get { return (Int32?)base[24]; }
            set { base[24] = value; }
        }
        public string MaritalStat
        {
            get { return (string)base[25]; }
            set { base[25] = value; }
        }
        public string Nationality

        {
            get { return (string)base[26]; }
            set { base[26] = value; }
        }
        public bool? ResieveSms
        {
            get { return (bool?)base[27]; }
            set { base[27] = value; }
        }
        public string AddressType
        {
            get { return (string)base[28]; }
            set { base[28] = value; }
        }
        public string CusAddr1
        {
            get { return (string)base[29]; }
            set { base[29] = value; }
        }
        public string CusAddr2
        {
            get { return (string)base[30]; }
            set { base[30] = value; }
        }
        public string CusAddr3
        {
            get { return (string)base[31]; }
            set { base[31] = value; }
        }
        public bool NewRecord
        {
            get { return (bool)base[32]; }
            set { base[32] = value; }
        }
        public string DeliveryArea
        {
            get { return (string)base[33]; }
            set { base[33] = value; }

        }
        public string PostCode
        {
            get { return (string)base[34]; }
            set { base[34] = value; }
        }
        public string Notes
        {
            get { return (string)base[35]; }
            set { base[35] = value; }
        }
        public DateTime DateIn
        {
            get { return (DateTime)base[36]; }
            set { base[36] = value; }
        }
        public Int32? User
        {
            get { return (Int32)base[37]; }
            set { base[37] = value; }
        }
        public string Zone
        {
            get { return (string)base[38]; }
            set { base[38] = value; }
        }
        public DateTime? DateTelAdd
        {
            get { return (DateTime?)base[39]; }
            set { base[39] = value; }
        }
        public string ExtnNo
        {
            get { return (string)base[40]; }
            set { base[40] = value; }
        }
        public string TelLocn
        {
            get { return (string)base[41]; }
            set { base[41] = value; }
        }
        public string DialCode
        {
            get { return (string)base[42]; }
            set { base[42] = value; }
        }
        public Int32? EmpeeNoChange
        {
            get { return (Int32?)base[43]; }
            set { base[43] = value; }

        }
        public string ReturnCustId
        {
            get { return (string)base[44]; }
            set { base[44] = value; }
        }
        public string Message
        {
            get { return (string)base[45]; }
            set { base[45] = value; }
        }
        public string StatusCode
        {
            get { return (string)base[46]; }
            set { base[46] = value; }
        }
        #endregion

    }
    #endregion
}
