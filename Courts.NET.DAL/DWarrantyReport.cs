using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common;
using STL.Common.Constants.Enums;
using STL.Common.Constants.TableNames;

namespace STL.DAL
{
    public class DWarrantyReport : DALObject
    {
        private string _warrantyType = "";
        public string WarrantyType
        {
            get { return _warrantyType; }
            set { _warrantyType = value; }
        }

        private string _branch = "";
        public string Branch
        {
            get { return _branch; }
            set { _branch = value; }
        }

        private string _salesperson = "";
        public string SalesPerson
        {
            get { return _salesperson; }
            set { _salesperson = value; }
        }

        private string _categoryset = "";
        public string CategorySet
        {
            get { return _categoryset; }
            set { _categoryset = value; }
        }

        private short _includecash = 0;
        public short IncludeCash
        {
            get { return _includecash; }
            set { _includecash = value; }
        }

        private short _includecredit = 0;
        public short IncludeCredit
        {
            get { return _includecredit; }
            set { _includecredit = value; }
        }


        private short _includerep = 0;
        public short IncludeRep
        {
            get { return _includerep; }
            set { _includerep = value; }
        }

        private short _includespecial = 0;
        public short IncludeSpecial
        {

            get { return _includespecial; }
            set { _includespecial = value; }
        }

        private short _includecanc = 0;
        public short IncludeCanc
        {
            get { return _includecanc; }
            set { _includecanc = value; }
        }
       
        

        
        private DateTime _datefrom = DateTime.Now;
        public DateTime DateFrom
        {
            get { return _datefrom; }
            set { _datefrom = value; }
        }

        private DateTime _dateto = DateTime.Now;
        public DateTime DateTo
        {
            get { return _dateto; }
            set { _dateto = value; }
        }

        private short _branchtotal = 0;
        public short BranchTotal
        {
            get { return _branchtotal; }
            set { _branchtotal = value; }
        }

        private short _categorytotal = 0;
        public short CategoryTotal
        {
            get { return _categorytotal; }
            set { _categorytotal = value; }
        }

        private short _salespersontotal = 0;
        public short SalesPersonTotal
        {
            get { return _salespersontotal; }
            set { _salespersontotal = value; }
        }

        private short _accttypetotal = 0;
        public short AcctTypeTotal
        {
            get { return _accttypetotal; }
            set { _accttypetotal = value; }
        }

        private string _datesare = "";
        public string DatesAre
        {
            get { return _datesare; }
            set { _datesare = value; }
        }

        private DataTable _warranties = null;
        public DataTable Warranties
        {
            get { return _warranties; }
        }

        public void WarrantySalesReport()
        {
            try
            {
                _warranties = new DataTable(TN.Warranties);
                parmArray = new SqlParameter[14];
                parmArray[0] = new SqlParameter("@warrantytype", SqlDbType.NVarChar, 3);
                parmArray[0].Value = this.WarrantyType;
                parmArray[1] = new SqlParameter("@branch", SqlDbType.NVarChar, 14);
                parmArray[1].Value = this.Branch;
                parmArray[2] = new SqlParameter("@salesperson", SqlDbType.NVarChar, 10);
                parmArray[2].Value = this.SalesPerson;
                parmArray[3] = new SqlParameter("@categoryset", SqlDbType.NVarChar, 32);
                parmArray[3].Value = this.CategorySet;
                parmArray[4] = new SqlParameter("@includecash", SqlDbType.SmallInt);
                parmArray[4].Value = this.IncludeCash;
                parmArray[5] = new SqlParameter("@includecredit", SqlDbType.SmallInt);
                parmArray[5].Value = this.IncludeCredit;
                parmArray[6] = new SqlParameter("@includespecial", SqlDbType.SmallInt);
                parmArray[6].Value = this.IncludeSpecial;
                parmArray[7] = new SqlParameter("@datefrom", SqlDbType.DateTime);
                parmArray[7].Value = this.DateFrom;
                parmArray[8] = new SqlParameter("@dateto", SqlDbType.DateTime);
                parmArray[8].Value = this.DateTo;
                parmArray[9] = new SqlParameter("@branchtotal", SqlDbType.SmallInt);
                parmArray[9].Value = this.BranchTotal;
                parmArray[10] = new SqlParameter("@categorytotal", SqlDbType.SmallInt);
                parmArray[10].Value = this.CategoryTotal;
                parmArray[11] = new SqlParameter("@salespersontotal", SqlDbType.SmallInt);
                parmArray[11].Value = this.SalesPersonTotal;
                parmArray[12] = new SqlParameter("@accttypetotal", SqlDbType.SmallInt);
                parmArray[12].Value = this.AcctTypeTotal;
                parmArray[13] = new SqlParameter("@datesAre", SqlDbType.NVarChar, 10);
                parmArray[13].Value = this.DatesAre;

                this.RunSP("RP_WarrantySales", parmArray, _warranties);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void WarrantyHitRateReport()
        {
            try
            {
                _warranties = new DataTable(TN.Warranties);
                parmArray = new SqlParameter[15];
                parmArray[0] = new SqlParameter("@warrantytype", SqlDbType.NVarChar, 3);
                parmArray[0].Value = this.WarrantyType;
                parmArray[1] = new SqlParameter("@branch", SqlDbType.NVarChar, 6);
                parmArray[1].Value = this.Branch;
                parmArray[2] = new SqlParameter("@salesperson", SqlDbType.NVarChar, 10);
                parmArray[2].Value = this.SalesPerson;
                parmArray[3] = new SqlParameter("@categoryset", SqlDbType.NVarChar, 32);
                parmArray[3].Value = this.CategorySet;
                parmArray[4] = new SqlParameter("@includecash", SqlDbType.SmallInt);
                parmArray[4].Value = this.IncludeCash;
                parmArray[5] = new SqlParameter("@includecredit", SqlDbType.SmallInt);
                parmArray[5].Value = this.IncludeCredit;
                parmArray[6] = new SqlParameter("@includespecial", SqlDbType.SmallInt);
                parmArray[6].Value = this.IncludeSpecial;
                parmArray[7] = new SqlParameter("@datefrom", SqlDbType.DateTime);
                parmArray[7].Value = this.DateFrom;
                parmArray[8] = new SqlParameter("@dateto", SqlDbType.DateTime);
                parmArray[8].Value = this.DateTo;
                parmArray[9] = new SqlParameter("@branchtotal", SqlDbType.SmallInt);
                parmArray[9].Value = this.BranchTotal;
                parmArray[10] = new SqlParameter("@categorytotal", SqlDbType.SmallInt);
                parmArray[10].Value = this.CategoryTotal;
                parmArray[11] = new SqlParameter("@salespersontotal", SqlDbType.SmallInt);
                parmArray[11].Value = this.SalesPersonTotal;
                parmArray[12] = new SqlParameter("@accttypetotal", SqlDbType.SmallInt);
                parmArray[12].Value = this.AcctTypeTotal;
                parmArray[13] = new SqlParameter("@includerep", SqlDbType.SmallInt);
                parmArray[13].Value = this.IncludeRep;
                parmArray[14] = new SqlParameter("@includecanc", SqlDbType.SmallInt);
                parmArray[14].Value = this.IncludeCanc;

                this.RunSP("RP_HitRateMissed", parmArray, _warranties);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void WarrantyLostSalesReport()
        {
            try
            {
                _warranties = new DataTable(TN.Warranties);
                parmArray = new SqlParameter[12];
                parmArray[0] = new SqlParameter("@branch", SqlDbType.NVarChar, 14);
                parmArray[0].Value = this.Branch;
                parmArray[1] = new SqlParameter("@salesperson", SqlDbType.NVarChar, 10);
                parmArray[1].Value = this.SalesPerson;
                parmArray[2] = new SqlParameter("@categoryset", SqlDbType.NVarChar, 32);
                parmArray[2].Value = this.CategorySet;
                parmArray[3] = new SqlParameter("@includecash", SqlDbType.SmallInt);
                parmArray[3].Value = this.IncludeCash;
                parmArray[4] = new SqlParameter("@includecredit", SqlDbType.SmallInt);
                parmArray[4].Value = this.IncludeCredit;
                parmArray[5] = new SqlParameter("@includespecial", SqlDbType.SmallInt);
                parmArray[5].Value = this.IncludeSpecial;
                parmArray[6] = new SqlParameter("@datefrom", SqlDbType.DateTime);
                parmArray[6].Value = this.DateFrom;
                parmArray[7] = new SqlParameter("@dateto", SqlDbType.DateTime);
                parmArray[7].Value = this.DateTo;
                parmArray[8] = new SqlParameter("@branchtotal", SqlDbType.SmallInt);
                parmArray[8].Value = this.BranchTotal;
                parmArray[9] = new SqlParameter("@categorytotal", SqlDbType.SmallInt);
                parmArray[9].Value = this.CategoryTotal;
                parmArray[10] = new SqlParameter("@salespersontotal", SqlDbType.SmallInt);
                parmArray[10].Value = this.SalesPersonTotal;
                parmArray[11] = new SqlParameter("@accttypetotal", SqlDbType.SmallInt);
                parmArray[11].Value = this.AcctTypeTotal;

                this.RunSP("RP_HitRateLostSales", parmArray, _warranties);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

    }
}
