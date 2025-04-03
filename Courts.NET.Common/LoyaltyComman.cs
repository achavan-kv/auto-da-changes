using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace STL.Common
{


    [Serializable]
    public class LoyaltyValidAddresses
    {
        private string _addtype;

        public string addtype
        {
            set { _addtype = value; }
            get { return _addtype; }
        }
    }

    //public class combovalues
    //{
    //    private string _text;
    //    private bool _Value;

    //    public string text
    //    {
    //        set { _text = value; }
    //        get { return _text; }
    //    }

    //    public bool Value
    //    {
    //        set { _Value = value; }
    //        get { return _Value; }
    //    }

    //    public combovalues(string name, bool val)
    //    {
    //        _text = name;
    //        _Value = val;
    //    }
    //}

    //[Serializable]
    //public class AgeRange
    //{
    //    private int _maxage;
    //    private int _minage;

    //    public int maxage
    //    {
    //        get { return _maxage; }
    //        set { _maxage = value; }
    //    }

    //    public int minage
    //    {
    //        get { return _minage; }
    //        set { _minage = value; }
    //    }
    //}

    public static class LoyaltyDropStatic
    {
        private static DataSet _LoyatlyDrop;

        public static DataSet LoyatlyDrop
        {
            set 
            {
                _LoyatlyDrop = value;

                if (_LoyatlyDrop != null)
            {
                _VoucherCode = ((DataRow[])LoyaltyDropStatic.LoyatlyDrop.Tables[0].Select("Category = 'HCI' AND reference = '1'"))[0]["code"].ToString().ToUpper();
                _MembershipFee = ((DataRow[])LoyaltyDropStatic.LoyatlyDrop.Tables[0].Select("Category = 'HCI' AND reference = '3'"))[0]["code"].ToString().ToUpper();
                _MembershipFeeFree = ((DataRow[])LoyaltyDropStatic.LoyatlyDrop.Tables[0].Select("Category = 'HCI' AND reference = '2'"))[0]["code"].ToString().ToUpper();
                _FreeDelivery = ((DataRow[])LoyaltyDropStatic.LoyatlyDrop.Tables[0].Select("Category = 'HCI' AND reference = '4'"))[0]["code"].ToString().ToUpper();
            }
            
            }
            get { return _LoyatlyDrop; }
        }

        private static string _VoucherCode;

        public static string VoucherCode
        {
            //set { _VoucherCode = value; }
            get { return _VoucherCode; }
        }

        private static string _MembershipFee;

        public static string MembershipFee
        {
            //set { _MembershipFee = value; }
            get { return _MembershipFee; }
        }

        private static string _MembershipFeeFree;

        public static string MembershipFeeFree
        {
            //set { _MembershipFeeFree = value; }
            get { return _MembershipFeeFree; }
        }

        private static string _FreeDelivery;

        public static string FreeDelivery
        {
            //set { _FreeDelivery = value; }
            get { return _FreeDelivery; }
        }
    }


    [Serializable]
    public class Loyalty
    {
        private string _memberno;
        private string _custid;
        private DateTime _startdate;
        private DateTime _enddate;
        private char _membertype;
        private int _statusvoucher;
        private int _statusacct;
        private int _rejections;
        private string _user;

        private char _cancel;
        //private bool _lost;
        //private bool _renew;

        //public bool mainholder
        //{
        //    set { _mainholder = value; }
        //    get { return _mainholder; }
        //}

        public string user
        {
            set { _user = value; }
            get { return _user; }
        }

        public char cancel
        {
            set { _cancel = value; }
            get { return _cancel; }
        }

        public string memberno
        {
            set { _memberno = value; }
            get { return _memberno; }
        }

        public string custid
        {
            set { _custid = value; }
            get { return _custid; }
        }

        public DateTime startdate
        {
            set { _startdate = value; }
            get { return _startdate; }
        }

        public DateTime enddate
        {
            set { _enddate = value; }
            get { return _enddate; }
        }

        public char membertype
        {
            set { _membertype = value; }
            get { return _membertype; }
        }

        public int statusacct
        {
            set { _statusacct = value; }
            get { return _statusacct; }
        }
        public int statusvoucher
        {
            set { _statusvoucher = value; }
            get { return _statusvoucher; }
        }


        //public bool lost
        //{
        //    set { _lost = value; }
        //    get { return _lost; }
        //}

        //public bool renew
        //{
        //    set { _renew = value; }
        //    get { return _renew; }
        //}
        public int rejections
        {
            set { _rejections = value; }
            get { return _rejections; }
        }
    }

    [Serializable]
    public class LoyaltyVoucher
    {
        private string _memberno;
        private string _custid;
        private string _acctno;
        private DateTime _voucherdate;
        private decimal _vouchervalue;
        private int _voucherref;

        public string memberno
        {
            set { _memberno = value; }
            get { return _memberno; }
        }

        public string acctno
        {
            set { _acctno = value; }
            get { return _acctno; }
        }
        public string custid
        {
            set { _custid = value; }
            get { return _custid; }
        }

        public DateTime voucherdate
        {
            set { _voucherdate = value; }
            get { return _voucherdate; }
        }

        public decimal vouchervalue
        {
            set { _vouchervalue = value; }
            get { return _vouchervalue; }
        }

        public int voucherref
        {
            set { _voucherref = value; }
            get { return _voucherref; }
        }
    }
}
