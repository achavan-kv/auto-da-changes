using System;
using System.Data;
using System.Data.SqlClient;
using STL.Common.Constants.TableNames;

namespace STL.DAL
{
    /// <summary>
    /// Summary description for DSalesCommission.
    /// </summary>
    public class DSalesCommission : DALObject
    {
        #region [   private variables   ]
        private DataTable _dt = null;
        private string _itemText = "";
        private string _item1 = "";
        private string _item2 = "";
        private string _item3 = "";
        private string _item4 = "";
        private string _item5 = "";
        private string _description = "";
        private string _commissionType = "";
        private double _percentage = 0;
        private double _percentageCash = 0;
        private decimal _value = 0;
        private double _repoPercentage = 0;             // RI
        private double _repoPercentageCash = 0;
        private decimal _repoValue = 0;
        private DateTime _dateFrom = System.DateTime.Now;
        private DateTime _dateTo = System.DateTime.Now;
        private string _branch = "";             // CR1035
        #endregion

        #region [   public properties   ]
        public string itemText
        {
            get { return _itemText; }
            set { _itemText = value; }
        }

        public string item1
        {
            get { return _item1; }
            set { _item1 = value; }
        }

        public string item2
        {
            get { return _item2; }
            set { _item2 = value; }
        }

        public string item3
        {
            get { return _item3; }
            set { _item3 = value; }
        }

        public string item4
        {
            get { return _item4; }
            set { _item4 = value; }
        }

        public string item5
        {
            get { return _item5; }
            set { _item5 = value; }
        }

        public string description
        {
            get { return _description; }
            set { _description = value; }
        }

        public string commissionType
        {
            get { return _commissionType; }
            set { _commissionType = value; }
        }

        public DateTime dateFrom
        {
            get { return _dateFrom; }
            set { _dateFrom = value; }
        }

        public DateTime dateTo
        {
            get { return _dateTo; }
            set { _dateTo = value; }
        }

        public decimal value
        {
            get { return _value; }
            set { _value = value; }
        }
        public decimal repoValue            // RI
        {
            get { return _repoValue; }
            set { _repoValue = value; }
        }

        public double percentage
        {
            get { return _percentage; }
            set { _percentage = value; }
        }
        public double percentageCash
        {
            get { return _percentageCash; }
            set { _percentageCash = value; }
        }
        public double repoPercentage            // RI
        {
            get { return _repoPercentage; }
            set { _repoPercentage = value; }
        }
        public double repoPercentageCash        // RI
        {
            get { return _repoPercentageCash; }
            set { _repoPercentageCash = value; }
        }
        public string branch            // CR1035
        {
            get { return _branch; }
            set { _branch = value; }
        }
        #endregion

        public DSalesCommission()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public DataTable GetSalesCommissionRates(string commItemStr, DateTime selectDate)
        {
            try
            {
                _dt = new DataTable(TN.SalesCommissionRates);
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@piCommissionItem", SqlDbType.NVarChar, 20);
                parmArray[0].Value = commItemStr;
                parmArray[1] = new SqlParameter("@piSelectDate", SqlDbType.DateTime);
                parmArray[1].Value = selectDate;

                this.RunSP("DN_SalesCommissionsRates_Get", parmArray, _dt);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return _dt;
        }

        public void SaveCommissionRates(SqlConnection conn, SqlTransaction trans, string commItemStr)
        {
            // Add/update rates
            // Any previous version of a rate is set terminated day before new rate
            try
            {
                parmArray = new SqlParameter[12];        //CR1035

                parmArray[0] = new SqlParameter("@piCommissionItem", SqlDbType.NVarChar);
                parmArray[0].Value = commItemStr;
                parmArray[1] = new SqlParameter("@piItemText", SqlDbType.NVarChar);
                parmArray[1].Value = this._itemText;
                parmArray[2] = new SqlParameter("@piPercentage", SqlDbType.Float);
                parmArray[2].Value = this._percentage;
                parmArray[3] = new SqlParameter("@piPercentageCash", SqlDbType.Float);
                parmArray[3].Value = this._percentageCash;
                parmArray[4] = new SqlParameter("@piValue", SqlDbType.Money);
                parmArray[4].Value = this._value;
                parmArray[5] = new SqlParameter("@piDateFrom", SqlDbType.DateTime);
                parmArray[5].Value = this._dateFrom;
                parmArray[6] = new SqlParameter("@piDateTo", SqlDbType.DateTime);
                parmArray[6].Value = this._dateTo;
                parmArray[7] = new SqlParameter("@piEmpeeNo", SqlDbType.Int);
                parmArray[7].Value = this.User;
                parmArray[8] = new SqlParameter("@piBranchNo", SqlDbType.VarChar);      //CR1035
                parmArray[8].Value = this._branch;
                parmArray[9] = new SqlParameter("@piRepoPercentage", SqlDbType.Float);          // RI
                parmArray[9].Value = this._repoPercentage;
                parmArray[10] = new SqlParameter("@piRepoPercentageCash", SqlDbType.Float);
                parmArray[10].Value = this._repoPercentageCash;
                parmArray[11] = new SqlParameter("@piRepoValue", SqlDbType.Money);
                parmArray[11].Value = this._repoValue;

                this.RunSP(conn, trans, "DN_SalesCommissionRates_Save", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

        }

        public void SaveLinkedSpiffRates(SqlConnection conn, SqlTransaction trans, string commItemStr)
        {
            // Add/update rates
            // Any previous version of a rate is set terminated day before new rate
            try
            {
                parmArray = new SqlParameter[13];           //CR1035

                parmArray[0] = new SqlParameter("@piCommissionItem", SqlDbType.NVarChar);
                parmArray[0].Value = commItemStr;
                parmArray[1] = new SqlParameter("@piDescription", SqlDbType.NVarChar);
                parmArray[1].Value = this._description;
                parmArray[2] = new SqlParameter("@piItem1", SqlDbType.NVarChar);
                parmArray[2].Value = this._item1;
                parmArray[3] = new SqlParameter("@piItem2", SqlDbType.NVarChar);
                parmArray[3].Value = this._item2;
                parmArray[4] = new SqlParameter("@piItem3", SqlDbType.NVarChar);
                parmArray[4].Value = this._item3;
                parmArray[5] = new SqlParameter("@piItem4", SqlDbType.NVarChar);
                parmArray[5].Value = this._item4;
                parmArray[6] = new SqlParameter("@piItem5", SqlDbType.NVarChar);
                parmArray[6].Value = this._item5;
                parmArray[7] = new SqlParameter("@piPercentage", SqlDbType.Float);
                parmArray[7].Value = this._percentage;
                parmArray[8] = new SqlParameter("@piValue", SqlDbType.Money);
                parmArray[8].Value = this._value;
                parmArray[9] = new SqlParameter("@piDateFrom", SqlDbType.DateTime);
                parmArray[9].Value = this._dateFrom;
                parmArray[10] = new SqlParameter("@piDateTo", SqlDbType.DateTime);
                parmArray[10].Value = this._dateTo;
                parmArray[11] = new SqlParameter("@piEmpeeNo", SqlDbType.Int);
                parmArray[11].Value = this.User;
                parmArray[12] = new SqlParameter("@piBranchNo", SqlDbType.VarChar);      //CR1035
                parmArray[12].Value = this._branch;

                this.RunSP(conn, trans, "DN_SalesCommissionMultiSpiffRates_Save", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

        }


        // Validate Commission Item  
        public string ValidateCommItem(SqlConnection conn, SqlTransaction trans, string commItemStr, string commItemText, DateTime CommDateFrom, DateTime CommDateTo, string CommSpiffBranch)  //CR1035
        {
            string exists = "";
            try
            {
                parmArray = new SqlParameter[6];        //CR1035

                parmArray[0] = new SqlParameter("@piCommissionItem", SqlDbType.NVarChar);
                parmArray[0].Value = commItemStr;
                parmArray[1] = new SqlParameter("@piItemText", SqlDbType.NVarChar);
                parmArray[1].Value = commItemText;
                parmArray[2] = new SqlParameter("@piDateFrom", SqlDbType.DateTime);
                parmArray[2].Value = CommDateFrom;
                parmArray[3] = new SqlParameter("@piDateTo", SqlDbType.DateTime);
                parmArray[3].Value = CommDateTo;
                parmArray[4] = new SqlParameter("@poValid", SqlDbType.Char);
                parmArray[4].Value = " ";
                parmArray[4].Direction = ParameterDirection.Output;
                parmArray[5] = new SqlParameter("@piBranchNo", SqlDbType.VarChar);      //CR1035
                parmArray[5].Value = CommSpiffBranch;

                this.RunSP(conn, trans, "DN_SalesCommissionsRates_Check", parmArray);

                if (parmArray[4].Value != DBNull.Value)
                    exists = (parmArray[4].Value.ToString());
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return exists;
        }

        // Validate Product Categories  
        public string ValidateCategory(SqlConnection conn, SqlTransaction trans)
        {
            string category = "";
            try
            {
                parmArray = new SqlParameter[1];

                parmArray[0] = new SqlParameter("@poCategory", SqlDbType.VarChar, 10);  //jec 02/07/08
                parmArray[0].Value = " ";
                parmArray[0].Direction = ParameterDirection.Output;

                this.RunSP(conn, trans, "DN_SalesCommissionsRates_CategoryCheck", parmArray);

                if (parmArray[0].Value != DBNull.Value)
                    category = (parmArray[0].Value.ToString());
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return category;
        }

        public void EODCommCalc(SqlConnection conn, SqlTransaction trans, int runNo)
        {
            try
            {
                parmArray = new SqlParameter[1];

                parmArray[0] = new SqlParameter("@piRunNo", SqlDbType.Int);
                parmArray[0].Value = runNo;

                this.RunSP(conn, trans, "DN_EOD_Commissions_Calculation", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

        }

        //Returns a datatable containing information for the eod csv extract
        public DataTable EODGetCommissionsExtract(SqlConnection conn, SqlTransaction trans, int runNo)
        {
            DataTable dt = new DataTable(TN.SalesCommission);
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@RunNo", SqlDbType.Int);
                parmArray[0].Value = runNo;

                this.RunSP(conn, trans, "DN_EOD_Commissions_CSVExtractSP", parmArray, dt);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dt;
        }
        // Get basic Commission details
        // This routine will retrieve Commission Summary, Commission Details, Order/delivery details
        // depending on parameters passed
        public DataTable GetBasicSalesCommission(string branchNo, int employee, DateTime fromDate, DateTime toDate, string accountNo, int agreementNo, string sumDet, string category)
        {
            try
            {
                _dt = new DataTable(TN.SalesCommission);
                parmArray = new SqlParameter[8];
                parmArray[0] = new SqlParameter("@piBranchNo", SqlDbType.Char);
                parmArray[0].Value = branchNo;
                parmArray[1] = new SqlParameter("@piEmpeeNo", SqlDbType.Int);
                parmArray[1].Value = employee;
                parmArray[2] = new SqlParameter("@piDateFrom", SqlDbType.DateTime);
                parmArray[2].Value = fromDate;
                parmArray[3] = new SqlParameter("@piDateTo", SqlDbType.DateTime);
                parmArray[3].Value = toDate;
                parmArray[4] = new SqlParameter("@piAccountNo", SqlDbType.VarChar);
                parmArray[4].Value = accountNo;
                parmArray[5] = new SqlParameter("@piAgreementNo", SqlDbType.Int);
                parmArray[5].Value = agreementNo;
                parmArray[6] = new SqlParameter("@piSumDets", SqlDbType.Char);
                parmArray[6].Value = sumDet;
                parmArray[7] = new SqlParameter("@piCategory", SqlDbType.VarChar);      //CR1035
                parmArray[7].Value = category;

                this.RunSP("DN_SalesCommissions_GetBasicDetails", parmArray, _dt);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return _dt;
        }

        public void AddAdditionalSpiff(SqlConnection conn, SqlTransaction trans, string acctNo, int authorisedBy,
                                        string itemNo, short stockLocn, decimal amount, int agrmtNo, int salesPerson)
        {
            try
            {
                parmArray = new SqlParameter[7];

                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[0].Value = acctNo;
                parmArray[1] = new SqlParameter("@authorisedby", SqlDbType.Int);
                parmArray[1].Value = authorisedBy;
                parmArray[2] = new SqlParameter("@itemno", SqlDbType.NVarChar, 18);         // RI
                parmArray[2].Value = itemNo;
                parmArray[3] = new SqlParameter("@stocklocn", SqlDbType.SmallInt);
                parmArray[3].Value = stockLocn;
                parmArray[4] = new SqlParameter("@amount", SqlDbType.Money);
                parmArray[4].Value = amount;
                parmArray[5] = new SqlParameter("@agrmtNo", SqlDbType.Int);
                parmArray[5].Value = agrmtNo;
                parmArray[6] = new SqlParameter("@empeeno", SqlDbType.Int);
                parmArray[6].Value = salesPerson;

                this.RunSP(conn, trans, "DN_AddAdditionalSpiffSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void DeleteSpiff(SqlConnection conn, SqlTransaction trans, string acctNo, string itemNo, short stockLocn)
        {
            try
            {
                parmArray = new SqlParameter[3];

                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[0].Value = acctNo;
                parmArray[1] = new SqlParameter("@itemno", SqlDbType.NVarChar, 18);         // RI
                parmArray[1].Value = itemNo;
                parmArray[2] = new SqlParameter("@stocklocn", SqlDbType.SmallInt);
                parmArray[2].Value = stockLocn;

                this.RunSP(conn, trans, "DN_DeleteSpiffSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public DataTable GetSpiffs(string itemNo, short stocklocn, string type, int itemId)          // RI
        {
            DataTable dt = new DataTable(TN.Spiffs);
            try
            {
                parmArray = new SqlParameter[4];
                parmArray[0] = new SqlParameter("@itemno", SqlDbType.NVarChar, 18);     // RI
                parmArray[0].Value = itemNo;
                parmArray[1] = new SqlParameter("@stocklocn", SqlDbType.SmallInt);
                parmArray[1].Value = stocklocn;
                parmArray[2] = new SqlParameter("@type", SqlDbType.NChar, 3);
                parmArray[2].Value = type;
                parmArray[3] = new SqlParameter("@itemId", SqlDbType.Int);            // RI
                parmArray[3].Value = itemId;

                this.RunSP("DN_SpiffSelectionSP", parmArray, dt);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dt;
        }

        public DataTable GetSalesCommissionReportHeader(int branchNo, int employee, DateTime fromDate, DateTime toDate, bool showStandardCommission, bool showSPIFFCommission)
        {
            DataTable dt = new DataTable();
            try
            {
                parmArray = new SqlParameter[6];
                parmArray[0] = new SqlParameter("@piBranchNo", SqlDbType.Int);
                parmArray[0].Value = branchNo;
                parmArray[1] = new SqlParameter("@empeeNo", SqlDbType.Int);
                parmArray[1].Value = employee;
                parmArray[2] = new SqlParameter("@fromDate", SqlDbType.DateTime);
                parmArray[2].Value = fromDate;
                parmArray[3] = new SqlParameter("@toDate", SqlDbType.DateTime);
                parmArray[3].Value = toDate;
                parmArray[4] = new SqlParameter("@ShowCommission", SqlDbType.Bit);
                parmArray[4].Value = showStandardCommission;
                parmArray[5] = new SqlParameter("@ShowSPIFF", SqlDbType.Bit);
                parmArray[5].Value = showSPIFFCommission;

                this.RunSP("DN_SalesCommissionReportHeaderSP", parmArray, dt);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dt;
        }

        public DataTable GetSalesCommissionReportDetail(int empeeNo, DateTime fromDate, DateTime toDate, bool showStandardCommission, bool showSPIFFCommission)
        {
            DataTable dt = new DataTable();
            try
            {
                parmArray = new SqlParameter[5];
                parmArray[0] = new SqlParameter("@empeeno", SqlDbType.Int);
                parmArray[0].Value = empeeNo;
                parmArray[1] = new SqlParameter("@fromDate", SqlDbType.DateTime);
                parmArray[1].Value = fromDate;
                parmArray[2] = new SqlParameter("@toDate", SqlDbType.DateTime);
                parmArray[2].Value = toDate;
                parmArray[3] = new SqlParameter("@ShowCommission", SqlDbType.Bit);
                parmArray[3].Value = showStandardCommission;
                parmArray[4] = new SqlParameter("@ShowSPIFF", SqlDbType.Bit);
                parmArray[4].Value = showSPIFFCommission;

                this.RunSP("DN_SalesCommissionReportDetailSP", parmArray, dt);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dt;
        }

        public DataTable GetLinkedSpiffItems(string itemNo, short stocklocn)
        {
            DataTable dt = new DataTable(TN.LinkedSpiffs);
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@itemno", SqlDbType.NVarChar, 18);         // RI
                parmArray[0].Value = itemNo;
                parmArray[1] = new SqlParameter("@stocklocn", SqlDbType.SmallInt);
                parmArray[1].Value = stocklocn;

                this.RunSP("DN_SpiffGetLinkedItemsSP", parmArray, dt);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dt;
        }
    }
}
