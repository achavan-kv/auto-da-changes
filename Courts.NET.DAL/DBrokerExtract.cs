using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace STL.DAL
{
    public class DBrokerExtract : DALObject
    {
        private DataSet _brokerFinancialData;
        public DataSet brokerFinancialData
        {
            get
            {
                return _brokerFinancialData;
            }

            set
            {
                _brokerFinancialData = value;
            }
        }

        //IP - CR946 - The following method will return the data extract 
        //from the 'Interface_financial' table since the last successfull 'BROKERX' run.
        public DataSet GetBrokerExtractData(SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                _brokerFinancialData = new DataSet();
                RunSP("BrokerExtractSP", _brokerFinancialData);

            }
            catch (SqlException ex)
            {

                throw ex;
            }
            return _brokerFinancialData;
        }
    }
}
