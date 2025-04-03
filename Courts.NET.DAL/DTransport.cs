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
    /// Data Access methods for the Transport table.
    /// </summary>
    public class DTransport : DALObject
    {
        private string _truckid = "";
        public string TruckId
        {
            get{return _truckid;}
            set{_truckid = value;}
        }

        private string _drivername = "";
        public string DriverName
        {
            get{return _drivername;}
            set{_drivername = value;}
        }

        private string _phoneno = "";
        public string PhoneNo
        {
            get{return _phoneno;}
            set{_phoneno = value;}
        }

        private string _carrierNumber = "";
        public string CarrierNumber
        {
            get { return _carrierNumber; }
            set { _carrierNumber = value; }
        }

        private DataTable _transportdata = null;
        public DataTable TransportData
        {
            get{return _transportdata;}
        }

        public void Save(SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                parmArray = new SqlParameter[4];
                parmArray[0] = new SqlParameter("@truckid", SqlDbType.VarChar, 26);
                parmArray[0].Value = this.TruckId;
                parmArray[1] = new SqlParameter("@drivername", SqlDbType.VarChar, 50);
                parmArray[1].Value = this.DriverName;
                parmArray[2] = new SqlParameter("@phoneno", SqlDbType.VarChar, 20);
                parmArray[2].Value = this.PhoneNo;
                parmArray[3] = new SqlParameter("@carrierNumber", SqlDbType.VarChar, 20);
                parmArray[3].Value = this.CarrierNumber;

                this.RunSP(conn, trans, "DN_TransportSaveSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void Delete(SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@truckid", SqlDbType.VarChar,26);
                parmArray[0].Value = this.TruckId;
                this.RunSP(conn, trans, "DN_TransportDeleteSP", parmArray);
            }
            catch(SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }			
        }

        public int GetTransport(string truckId)
        {
            _transportdata = new DataTable(TN.TransportData);

            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@truckid", SqlDbType.VarChar,26);
                parmArray[0].Value = truckId;
                result = this.RunSP("DN_TransportGetSP", parmArray, _transportdata);
            }
            catch(SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }			
            return result;
        }

        public DTransport()
        {
            //
            // TODO: Add constructor logic here
            //
        }
    }
}
