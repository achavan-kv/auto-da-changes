using System;
using STL.Common;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace STL.DAL
{
    /// <summary>
    /// Summary description for DOracleIntegration.
    /// </summary>
    public class DOracleIntegration : DALObject
    {

        private DataSet _invoices;
        public DataSet invoices
        {
            get
            {
                return _invoices;
            }
            set
            {
                _invoices = value;
            }
        }
        public DataSet receipts;
        public DataSet customers;

        // Get Invoice data
        public DataSet GetInvoicedata()
        {
            try
            {
                invoices = new DataSet();
                RunSP("OracleExport_GetInvoicesSP", invoices);
            }
            catch (SqlException ex)
            {
                //LogSqlException(ex);
                throw ex;
            }

            return invoices;
        }

        // Get Invoice data
        public DataSet GetReceiptdata()
        {
            try
            {
                receipts = new DataSet();
                RunSP("OracleExport_GetReceiptSP", receipts);
            }
            catch (SqlException ex)
            {
                //LogSqlException(ex);
                throw ex;
            }

            return receipts;
        }

        // Get Customer data
        public DataSet GetCustomerdata()
        {
            try
            {
                customers = new DataSet();
                RunSP("OracleExport_GetCustomersSP", customers);
            }
            catch (SqlException ex)
            {
                //LogSqlException(ex);
                throw ex;
            }

            return customers;
        }

        // Create Export tables for Oracle
        public void CreateOracleData(SqlConnection conn, SqlTransaction trans, int factRunNo, int smryRunNo)
        {
            //try
            //{
            parmArray = new SqlParameter[2];
            parmArray[0] = new SqlParameter("@RunnoFact", SqlDbType.Int) {Value = factRunNo};
            parmArray[1] = new SqlParameter("@runnoUPDSMRY", SqlDbType.Int) {Value = smryRunNo};

            RunSP(conn, trans, "OracleExport_InvoiceCustomerReceiptSP", parmArray);
            //}
            //catch (SqlException ex)
            //{
            //    //LogSqlException(ex);
            //    throw;
            //}           
        }

        // Clear Oracle export tables - after data transferred
        public void ClearOracleData()
        {
            try
            {
                RunSP("OracleExport_TruncateSP");
            }
            catch (SqlException ex)
            {
                //LogSqlException(ex);
                throw ex;
            }
        }
    }
}
