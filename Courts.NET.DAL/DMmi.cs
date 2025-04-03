using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common.Constants.TableNames;

namespace STL.DAL
{
    public class DMmi : DALObject
    {
        DataSet operands = null;
        private DataTable _matrix = null;

        public DataTable Matrix
        {
            get { return _matrix; }
        }

        public DMmi()
        {

        }

        public void GetMmiMatrix()
        {
            try
            {
                _matrix = new DataTable(TN.MmiMatrix);
                this.RunSP("DN_MmiMatrixGetSP", parmArray, _matrix); // SC CR1034 17-02-10
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void DeleteMmiMatrix(SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                this.RunSP(conn, trans, "DN_MmiMatrixDeleteSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void SaveMmiMatrixRow(SqlConnection conn, SqlTransaction trans, string label, int fromScore,
                                    int toScore, decimal mmiPercentage, DateTime configuredDate, int configuredBy)
        {
            try
            {
                parmArray = new SqlParameter[6];
                parmArray[0] = new SqlParameter("@Label", SqlDbType.NVarChar, 32);
                parmArray[0].Value = label;
                parmArray[1] = new SqlParameter("@FromScore", SqlDbType.SmallInt);
                parmArray[1].Value = fromScore;
                parmArray[2] = new SqlParameter("@ToScore", SqlDbType.SmallInt);
                parmArray[2].Value = toScore;
                parmArray[3] = new SqlParameter("@MmiPercentage", SqlDbType.Float);
                parmArray[3].Value = mmiPercentage;
                parmArray[4] = new SqlParameter("@ConfiguredDate", SqlDbType.DateTime);
                parmArray[4].Value = configuredDate;
                parmArray[5] = new SqlParameter("@ConfiguredBy", SqlDbType.Int);
                parmArray[5].Value = configuredBy;


                this.RunSP(conn, trans, "DN_MmiMatrixRowSaveSP", parmArray);

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void GetMmiThresholdForSale(SqlConnection conn, SqlTransaction trans, string custId, string acctNo, string termsType, out bool isMmiAllowed, out decimal mmiLimit, out decimal mmiThreshold)
        {
            try
            {
                parmArray = new SqlParameter[7];
                parmArray[0] = new SqlParameter("@CustId", SqlDbType.NChar, 20);
                parmArray[0].Value = custId;
                parmArray[1] = new SqlParameter("@AcctNo", SqlDbType.NChar, 20);
                parmArray[1].Value = acctNo;
                parmArray[2] = new SqlParameter("@TermsType", SqlDbType.NChar, 2);
                parmArray[2].Value = termsType;
                parmArray[3] = new SqlParameter("@UserId", SqlDbType.Int);
                parmArray[3].Value = User;
                parmArray[4] = new SqlParameter("@IsMmiAllowed", SqlDbType.Bit);
                parmArray[4].Value = 0;
                parmArray[4].Direction = ParameterDirection.Output;
                parmArray[5] = new SqlParameter("@MmiLimit", SqlDbType.Decimal);
                parmArray[5].Value = 0;
                parmArray[5].Direction = ParameterDirection.Output;
                parmArray[6] = new SqlParameter("@MmiThreshold", SqlDbType.Decimal);
                parmArray[6].Value = 0;
                parmArray[6].Direction = ParameterDirection.Output;
                this.RunSP(conn, trans, "GetMmiThresholdForSale", parmArray);
                isMmiAllowed = (bool)parmArray[4].Value;
                mmiLimit = (decimal)parmArray[5].Value;
                mmiThreshold = (decimal)parmArray[6].Value;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
           // return result;
        }

        public void SaveCustomerMmi(SqlConnection conn, SqlTransaction trans, string custId, int userId, string reasonChanged)
        {
            try
            {
                parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@CustId", SqlDbType.NVarChar, 32);
                parmArray[0].Value = custId;
                parmArray[1] = new SqlParameter("@UserId", SqlDbType.Int);
                parmArray[1].Value = userId;
                parmArray[2] = new SqlParameter("@ReasonChanged", SqlDbType.NVarChar, 32);
                parmArray[2].Value = reasonChanged;
                this.RunSP(conn, trans, "SaveCustomerMMI", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }
    }
}
