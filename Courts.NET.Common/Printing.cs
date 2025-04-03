using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Xml;
using System.Globalization;

namespace STL.Common
{
    [Serializable]
    public struct DNparameters
    {
        public string custID;
        public int userSale;
        public int user;
        public string printText;
        public string acctno;
        public int buffLocn;
        public int delnotenum;
        public DateTime dateReqDel;
        public string delAddressType;
        public string timeReqDel;
    }

    [Serializable]
    public struct DNLocalparameters
    {
        public string locn;
        public string customername;
        public string alias;
        public int lastBuffNo;
        public string buffBranchNo;
    }

    [Serializable]
    public class PrintingDN
    {
        private DataSet _Customer;

        public DataSet Customer
        {
            get { return _Customer; }
            set { _Customer = value; }
        }


        private string _empname;

        public string empname
        {
            get { return _empname; }
            set { _empname = value; }

        }

        private string _printedby;

        public string printedby
        {
            get { return _printedby; }
            set { _printedby = value; }

        }

        private decimal _amountPayable;

        public decimal amountPayable
        {
            get { return _amountPayable; }
            set { _amountPayable = value; }

        }
        private decimal _charges;

        public decimal charges
        {
            get { return _charges; }
            set { _charges = value; }
        }

        private bool _cod;

        public bool cod
        {
            get { return _cod; }
            set { _cod = value; }
        }

        private string _acctno;

        public string acctno
        {
            get { return _acctno; }
            set { _acctno = value; }
        }


        private string _printText;

        public string printText
        {
            get { return _printText; }
            set { _printText = value; }
        }


        private string _alias;

        public string alias
        {
            get { return _alias; }
            set { _alias = value; }
        }


        private string _customername;

        public string customername
        {
            get { return _customername; }
            set { _customername = value; }
        }


        private bool _delnotenum;

        public bool delnotenum
        {
            get { return _delnotenum; }
            set { _delnotenum = value; }
        }


        private string _locn;

        public string locn
        {
            get { return _locn; }
            set { _locn = value; }
        }

        private string _buffno;

        public string buffno
        {
            get { return _buffno; }
            set { _buffno = value; }
        }

        //private string _username;
        //public string username
        //{
        //    get { return _username; }
        //    set { _username = value; }
        //}

        private int _user;
        public int user
        {
            get { return _user; }
            set { _user = value; }
        }

        private string _origbuffbranchno; //UAT(5.2) - 568	

        public string origbuffbranchno
        {
            get { return _origbuffbranchno; }
            set { _origbuffbranchno = value; }
        }


        public PrintingDN()
        {
            _Customer = new DataSet();
            _Customer.DataSetName = "Customer";
        }   

    }
    ///// <summary>
    ///// Class PrintingAction used to store all the variables and DataSet ActionSet
    ///// </summary>
    ///// 
    //[Serializable]
    //public class PrintingAction
    //{
    //    private DataSet _ActionSet;

    //    public DataSet ActionSet
    //    {
    //        get { return _ActionSet; }
    //        set { _ActionSet = value; }
    //    }


    //    private string _alias;
    //    public string Alias
    //    {
    //        get { return _alias; }
    //        set { _alias = value; }
    //    }


    //    private decimal _paymentAmount;

    //    public decimal paymentAmount
    //    {
    //        get { return _paymentAmount; }
    //        set { _paymentAmount = value; }
    //    }

    //    private decimal _arrears;

    //    public decimal Arrears
    //    {
    //        get { return _arrears; }
    //        set { _arrears = value; }
    //    }


    //    private decimal _bailiffFee;

    //    public decimal bailiffFee
    //    {
    //        get { return _bailiffFee; }
    //        set { _bailiffFee = value; }
    //    }

        
    //    private decimal _collectionFee;

    //    public decimal collectionFee
    //    {
    //        get { return _collectionFee; }
    //        set { _collectionFee = value; }
    //    }

    //    private string _custid;
    //    public string custID
    //    {
    //        get { return _custid; }
    //        set { _custid = value; }
    //    }


    //    private string _acctno;
    //    public string Acctno
    //    {
    //        get { return _acctno; }
    //        set { _acctno = value; }
    //    }

    //    private string _currstatus;
    //    public string Currstatus
    //    {
    //        get { return _currstatus; }
    //        set { _currstatus = value; }
    //    }

    //    private DateTime _dateLastPaid;
    //    public DateTime DateLastPaid
    //    {
    //        get { return _dateLastPaid; }
    //        set { _dateLastPaid = value; }
    //    }
        
    //    private DateTime _deadLineDate;
    //    public DateTime DeadLineDate
    //    {
    //        get { return _deadLineDate; }
    //        set { _deadLineDate = value; }
    //    }


    //    private int _day;
    //    public int day
    //    {
    //        get { return _day; }
    //        set { _day = value; }
    //    }

    //    private string _firstname;
    //    public string Firstname
    //    {
    //        get { return _firstname; }
    //        set { _firstname = value; }
    //    }
        
    //    private decimal _instAmount;
    //    public decimal instAmount
    //    {
    //        get { return _instAmount; }
    //        set { _instAmount = value; }
    //    }

    //    private string _name;
    //    public string Name
    //    {
    //        get { return _name; }
    //        set { _name = value; }
    //    }

    //    private decimal _outstbal;
    //    public decimal Outstbal
    //    {
    //        get { return _outstbal; }
    //        set { _outstbal = value; }
    //    }

    //    private string _photo;
    //    public string Photo
    //    {
    //        get { return _photo; }
    //        set { _photo = value; }
    //    }
    //   private int _privilegeCount;
    //    public int privilegeCount
    //    {
    //        get { return _privilegeCount; }
    //        set { _privilegeCount = value; }
    //    }

    //    private int _segmentId;
    //    public int segmentId
    //    {
    //        get { return _segmentId; }
    //        set { _segmentId = value; }
    //    }

    //    private string _signaturefile;
    //    public string SignatureFile
    //    {
    //        get { return _signaturefile; }
    //        set { _signaturefile = value; }
    //    }

    //    private string _title;
    //    public string Title
    //    {
    //        get { return _title; }
    //        set { _title = value; }
    //    }


    //}
    /// <summary>
    /// Class PrintingAction used to store all the variables and DataSet ActionSet
    /// </summary>
    ///   
    [Serializable]
    public class PrintingAction
    {
        private DataSet _ActionSet;

        public DataSet ActionSet
        {
            get { return _ActionSet; }
            set { _ActionSet = value; }
        }


        private string _alias;
        public string Alias
        {
            get { return _alias; }
            set { _alias = value; }
        }


        private decimal _paymentAmount;

        public decimal paymentAmount
        {
            get { return _paymentAmount; }
            set { _paymentAmount = value; }
        }

        private decimal _arrears;

        public decimal Arrears
        {
            get { return _arrears; }
            set { _arrears = value; }
        }


        private decimal _bailiffFee;

        public decimal bailiffFee
        {
            get { return _bailiffFee; }
            set { _bailiffFee = value; }
        }


        private decimal _collectionFee;

        public decimal collectionFee
        {
            get { return _collectionFee; }
            set { _collectionFee = value; }
        }

        private string _custid;
        public string custID
        {
            get { return _custid; }
            set { _custid = value; }
        }


        private string _acctno;
        public string Acctno
        {
            get { return _acctno; }
            set { _acctno = value; }
        }

        private string _currstatus;
        public string Currstatus
        {
            get { return _currstatus; }
            set { _currstatus = value; }
        }

        private DateTime _dateLastPaid;
        public DateTime DateLastPaid
        {
            get { return _dateLastPaid; }
            set { _dateLastPaid = value; }
        }

        private DateTime _deadLineDate;
        public DateTime DeadLineDate
        {
            get { return _deadLineDate; }
            set { _deadLineDate = value; }
        }


        private int _day;
        public int day
        {
            get { return _day; }
            set { _day = value; }
        }

        private string _firstname;
        public string Firstname
        {
            get { return _firstname; }
            set { _firstname = value; }
        }

        private decimal _instAmount;
        public decimal instAmount
        {
            get { return _instAmount; }
            set { _instAmount = value; }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private decimal _outstbal;
        public decimal Outstbal
        {
            get { return _outstbal; }
            set { _outstbal = value; }
        }

        private string _photo;
        public string Photo
        {
            get { return _photo; }
            set { _photo = value; }
        }
        private int _privilegeCount;
        public int privilegeCount
        {
            get { return _privilegeCount; }
            set { _privilegeCount = value; }
        }

        private int _segmentId;
        public int segmentId
        {
            get { return _segmentId; }
            set { _segmentId = value; }
        }

        private string _signaturefile;
        public string SignatureFile
        {
            get { return _signaturefile; }
            set { _signaturefile = value; }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }


        private int _user;
        public int user
        {
            get { return _user; }
            set { _user = value; }
        }

    }

    public class PrintingAgreementRequest
    {
        public string customerID;
        public string accountNo;
        public string accountType;
        public string countrycode;
    }
    
    public class PrintingAgreementResult
    {
        public double itemsPerPage;
        public string filename;
        public XmlNode lineitems;
        public int noCopies;
        public string JointName;
        public int custCopies;
        public string relationship;
        public string JointID;
        public DataSet customer;
        public bool AgrTimePrint;
        public DataSet agreement;
        public NumberFormatInfo localformat;
        public decimal percenttopay;
        public bool IncInsinServAgrPrint;
        public bool insIncluded;
        public decimal chargeablePrice;
        public decimal chargeableAdminPrice;
        public decimal insPcent;
        public int months;
        public int PaymentHolidaysMax;
        public short PaymentHolidaysMin;
        public short Print90;
        public string TermsDescription;
        public string AgreementText;
        public bool PaymentHolidays;
        public decimal ServicePCent;
        public string CountryName;
    }



}
