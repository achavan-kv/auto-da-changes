using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common.Constants.Enums;

namespace STL.DAL
{
    /// <summary>
    /// Summary description for DInterfaceError.
    /// </summary>
    public class DInterfaceError : DALObject
    {
        private string _interface = "";
        public string Interface
        {
            get { return _interface; }
            set { _interface = value; }
        }

        private int _runno = 0;
        public int RunNumber
        {
            get { return _runno; }
            set { _runno = value; }
        }

        private DateTime _errorDate = DateTime.MinValue.AddYears(1899);
        public DateTime ErrorDate
        {
            get { return _errorDate; }
            set { _errorDate = value; }
        }

        private string _errorText = "";
        public string ErrorText
        {
            get { return _errorText; }
            set { _errorText = value; }
        }

        private string _severity = "";
        public string Severity
        {
            get { return _severity; }
            set { _severity = value; }
        }

        private DateTime _startDate = DateTime.MinValue.AddYears(1899);     //jec 06/05/11
        public DateTime StartDate
        {
            get { return _startDate; }
            set { _startDate = value; }
        }

        private DataTable _control;
        public DataTable Control
        {
            get { return _control; }
        }

        public void Write(SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                parmArray = new SqlParameter[5];
                parmArray[0] = new SqlParameter("@interface", SqlDbType.NVarChar, 20);
                parmArray[0].Value = this.Interface;
                parmArray[1] = new SqlParameter("@runno", SqlDbType.Int);
                parmArray[1].Value = this.RunNumber;
                parmArray[2] = new SqlParameter("@errorDate", SqlDbType.DateTime);
                parmArray[2].Value = this.ErrorDate;
                parmArray[3] = new SqlParameter("@errorText", SqlDbType.NText);
                parmArray[3].Value = this.ErrorText;
                parmArray[4] = new SqlParameter("@severity", SqlDbType.NChar, 1);
                parmArray[4].Value = this.Severity;

                if (conn != null && trans != null)
                    this.RunSP(conn, trans, "DN_InterfaceErrorWriteSP", parmArray);
                else
                    this.RunSP("DN_InterfaceErrorWriteSP", parmArray);

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void RemoveOld(SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@interface", SqlDbType.NVarChar, 20);
                parmArray[0].Value = this.Interface;
                parmArray[1] = new SqlParameter("@runno", SqlDbType.Int);
                parmArray[1].Value = this.RunNumber;
                
                if (conn != null && trans != null)
                    this.RunSP(conn, trans, "DN_InterfaceErrorRemoveOldSP", parmArray);
                else
                    this.RunSP("DN_InterfaceErrorRemoveOldSP", parmArray);

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }


        public void GetInterfaceError()
        {
            _control = new DataTable();

            try
            {
                parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@interface", SqlDbType.NVarChar, 12);
                parmArray[0].Value = this.Interface;
                parmArray[1] = new SqlParameter("@runno", SqlDbType.Int);
                parmArray[1].Value = this.RunNumber;
                parmArray[2] = new SqlParameter("@datestart", SqlDbType.DateTime);      //jec 06/04/11
                parmArray[2].Value = this.StartDate;

                this.RunSP("DN_InterfaceErrorLoadSP", parmArray, _control);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public DInterfaceError()
        {

        }
    }
}
