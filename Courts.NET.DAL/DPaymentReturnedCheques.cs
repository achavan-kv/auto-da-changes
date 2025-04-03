using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common.Constants.TableNames;

namespace STL.DAL
{
    /// <summary>
    /// Data Access Class for returned cheque functionality on payments
    /// </summary>
    public class DPaymentReturnedCheques : DALObject
    {

     /// <summary>
     /// Gets a list of returned cheques and boolean for authorisation
     /// </summary>
     /// <param name="conn">sql connection</param>
     /// <param name="trans">sql transaction</param>
     /// <param name="customerID"></param>
     /// <param name="returnChequePeriod"></param>
     /// <param name="returnChequeAllowedCount"></param>
     /// <param name="authorisationRequired"></param>
     /// <returns></returns>
     public DataTable Get(SqlConnection conn, SqlTransaction trans,
                            string customerID, int returnChequePeriod, int returnChequeAllowedCount, out bool authorisationRequired)
        {
           
            DataTable dt = new DataTable(TN.ReturnedCheques); 
            string storedProcedureName = "DN_PaymentReturnedChequesGetSP";
            authorisationRequired = false;
            
            try
            {
                parmArray = new SqlParameter[4];

                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
                parmArray[0].Value = customerID;

                parmArray[1] = new SqlParameter("@returnchequeperiod", SqlDbType.Int);
                parmArray[1].Value = returnChequePeriod;

                parmArray[2] = new SqlParameter("@returnchequeallowedcount", SqlDbType.Int);
                parmArray[2].Value = returnChequeAllowedCount;

                parmArray[3] = new SqlParameter("@authorisationrequired", SqlDbType.Bit);
                parmArray[3].Value = 0;
                parmArray[3].Direction = ParameterDirection.Output;
                	
                if (conn != null && trans != null)
                    this.RunSP(conn, trans, storedProcedureName, parmArray, dt);
                else
                    this.RunSP(storedProcedureName, parmArray, dt);

                if (parmArray[3].Value != DBNull.Value)
                    authorisationRequired = Convert.ToBoolean(parmArray[3].Value);
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
