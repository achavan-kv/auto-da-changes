using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common;
using STL.Common.Constants.Enums;
using STL.Common.Constants.TableNames;

namespace STL.DAL
{
    public class DWarrantyBand : DALObject
    {
        private string _itemno = "";
        public string Itemno
        {
            get { return _itemno; }
            set { _itemno = value; }
        }

        private string _refcode = "";
        public string Refcode
        {
            get { return _refcode; }
            set { _refcode = value; }
        }

        private decimal _minprice = 0;
        public decimal MinPrice
        {
            get { return _minprice; }
            set { _minprice = value; }
        }

        private decimal _maxprice = 0;
        public decimal MaxPrice
        {
            get { return _maxprice; }
            set { _maxprice = value; }
        }

        // CR903 - warranty length is now stored as a
        // float to accomadate 6 & 18 month warranties.
        private double _warrantylength = 0;
        public double WarrantyLength
        {
            get { return _warrantylength; }
            set { _warrantylength = value; }
        }

        public DWarrantyBand()
		{
		}
        
        public void Save(SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                parmArray = new SqlParameter[5];
                //parmArray[0] = new SqlParameter("@itemno", SqlDbType.NVarChar, 10);
                parmArray[0] = new SqlParameter("@itemno", SqlDbType.NVarChar, 18);                 //IP - 31/08/11 - RI - #4960
                parmArray[0].Value = this.Itemno;
                parmArray[1] = new SqlParameter("@refcode", SqlDbType.NVarChar, 3);
                parmArray[1].Value = this.Refcode;
                parmArray[2] = new SqlParameter("@minprice", SqlDbType.Money);
                parmArray[2].Value = this.MinPrice;
                parmArray[3] = new SqlParameter("@maxprice", SqlDbType.Money);
                parmArray[3].Value = this.MaxPrice;
                parmArray[4] = new SqlParameter("@length", SqlDbType.Float);
                parmArray[4].Value = this.WarrantyLength;

                this.RunSP(conn, trans, "DN_WarrantyBandUpdateSP", parmArray);
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
                this.RunSP(conn, trans, "DN_WarrantyBandDelete");
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }
    }
}
