using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common;
using STL.Common.Constants.Enums;
using STL.Common.Constants.TableNames;


namespace STL.DAL
{
    /// <summary>
    /// Data Access methods for the SPA table.
    /// </summary>
    public class DSpa : DALObject
    {
        private string _acctno = "";
        public string AccountNo
        {
            get{return _acctno;}
            set{_acctno = value;}
        }

        private int _allocno = 0;
        public int AllocNo 
        {
            get{return _allocno;}
            set{_allocno = value;}
        }

        private int _empeeno = 0;
        public int EmployeeNo
        {
            get{return _empeeno;}
            set{_empeeno = value;}
        }

        private int _empeenospa = 0;
        public int EmpeeNoSpa
        {
            get{return _empeenospa;}
            set{_empeenospa = value;}
        }

        private DateTime _dateAdded;
        public DateTime DateAdded
        {
            get{return _dateAdded;}
            set{_dateAdded = value;}
        }

        private DateTime _dateExpiry;
        public DateTime DateExpiry
        {
            get{return _dateExpiry;}
            set{_dateExpiry = value;}
        }

        private string _code = "";
        public string ReasonCode
        {
            get{return _code;}
            set{_code = value;}
        }

        private short _actionNo = 0;
        public short ActionNo
        {
            get{return _actionNo;}
            set{_actionNo = value;}
        }

        private double _spainstal = 0;
        public double SpaInstal
        {
            get{return _spainstal;}
            set{_spainstal = value;}
        }

        private DataTable _spahistory = null;
        public DataTable SpaHistory
        {
            get{return _spahistory;}
        }

        public void Save(SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                parmArray = new SqlParameter[9];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
                parmArray[0].Value = this.AccountNo;
                parmArray[1] = new SqlParameter("@allocno", SqlDbType.Int);
                parmArray[1].Value = this.AllocNo;
                parmArray[2] = new SqlParameter("@actionno", SqlDbType.Int);
                parmArray[2].Value = this.ActionNo;
                parmArray[3] = new SqlParameter("@employeeno", SqlDbType.Int);
                parmArray[3].Value = this.EmployeeNo;
                parmArray[4] = new SqlParameter("@empeenospa", SqlDbType.Int);
                parmArray[4].Value = this.EmpeeNoSpa;
                parmArray[5] = new SqlParameter("@dateadded", SqlDbType.DateTime);
                parmArray[5].Value = this.DateAdded;
                parmArray[6] = new SqlParameter("@reasoncode", SqlDbType.NVarChar,4);
                parmArray[6].Value = this.ReasonCode;
                parmArray[7] = new SqlParameter("@dateexpiry", SqlDbType.DateTime);
                parmArray[7].Value = this.DateExpiry;
                parmArray[8] = new SqlParameter("@spainstal", SqlDbType.Float);
                parmArray[8].Value = this.SpaInstal;
                this.RunSP(conn, trans, "DN_SpaSaveSP", parmArray);
            }
            catch(SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }			
        }

        public int GetSpaHistory(string accountNo)
        {
            _spahistory = new DataTable("SpaHistory");

            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
                parmArray[0].Value = accountNo;
						
                result = this.RunSP("DN_SpaGetSP", parmArray, _spahistory);
            }
            catch(SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }			
            return result;
        }

        public DSpa()
        {
            //
            // TODO: Add constructor logic here
            //
        }
    }
}
