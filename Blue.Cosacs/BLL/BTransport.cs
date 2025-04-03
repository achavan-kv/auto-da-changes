using System;
using STL.Common;
using System.Data;
using STL.DAL;
using System.Xml;
using STL.Common.Constants.ColumnNames;
using System.Data.SqlClient;
using STL.Common.Constants.TableNames;

namespace STL.BLL
{
    /// <summary>
    /// Business Logic Layer for Transport
    /// </summary>
    public class BTransport : CommonObject
    {
        public DataSet GetTransport(string truckId)
        {
            DataSet ds = new DataSet();
            DTransport dTransport = new DTransport();
            dTransport.GetTransport(truckId);
            ds.Tables.Add(dTransport.TransportData);
            return ds;
        }
        
        public void Save(SqlConnection conn, SqlTransaction trans,
            string truckId, string driverName, string phoneNo, string carrierNumber)
        {
            DTransport dTransport = new DTransport();
            dTransport.TruckId = truckId;
            dTransport.DriverName = driverName;
            dTransport.PhoneNo = phoneNo;
            dTransport.CarrierNumber = carrierNumber;
            dTransport.Save(conn,trans);
        }

        public void Delete(SqlConnection conn, SqlTransaction trans, 
            string truckId)
        {
            DTransport dTransport = new DTransport();
            dTransport.TruckId = truckId;
            dTransport.Delete(conn,trans);
        }

        public BTransport()
        {
        }
    }
}
