using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common.Constants.TableNames;

namespace STL.DAL
{
    public class DSpend : DALObject
    {
        public decimal GetDependentSpendFactor(SqlConnection conn, SqlTransaction trans, decimal noOfDep)
        {
            decimal spendFactor = 0;

            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@noOfDep", SqlDbType.NVarChar, 10);
                parmArray[0].Value = noOfDep;
                parmArray[1] = new SqlParameter("@SpendFactor", SqlDbType.VarChar, 10);
                parmArray[1].Value = spendFactor;
                parmArray[1].Direction = ParameterDirection.Output;
                this.RunSP(conn, trans, "GetDependentSpendFactorByVal", parmArray);

                if (parmArray[1].Value != DBNull.Value)
                    spendFactor = Convert.ToDecimal(parmArray[1].Value.ToString());
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            return spendFactor;

        }

        public decimal GetApplicantSpendFactor(SqlConnection conn, SqlTransaction trans, string income)
        {
            decimal spendFactor = 0;

            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@income", SqlDbType.NVarChar, 10);
                parmArray[0].Value = income;
                parmArray[1] = new SqlParameter("@SpendFactor", SqlDbType.VarChar, 10);
                parmArray[1].Value = spendFactor;
                parmArray[1].Direction = ParameterDirection.Output;
                this.RunSP(conn, trans, "GetApplicantSpendFactorByVal", parmArray);

                if (parmArray[1].Value != DBNull.Value)
                    spendFactor = Convert.ToDecimal(parmArray[1].Value.ToString());



                // this.RunSP(conn, trans, "GetApplicantSpendFactorByIncome", dt);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            return spendFactor;
        }

        public DataTable GetApplicantSpendFactorMatrix(SqlConnection conn, SqlTransaction trans)
        {
            DataTable dt = new DataTable();
            try
            {
                this.RunSP(conn, trans, "GetApplicantSpendFactor", dt);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dt;
        }

        public void SaveApplicantSpendFactorMatrix(SqlConnection conn, SqlTransaction trans, DataTable dt)
        {
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter();
                parmArray[0].ParameterName = "@ApplicantSpendFactorType";
                parmArray[0].Value = dt;

                this.RunSP(conn, trans, "ApplicantSpendFactorInsert", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

        }

        public DataTable GetDependentSpendFactorMatrix(SqlConnection conn, SqlTransaction trans)
        {
            DataTable dt = new DataTable();
            try
            {
                this.RunSP(conn, trans, "GetDependentSpendFactor", dt);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dt;
        }

        public void SaveDependentSpendFactorMatrix(SqlConnection conn, SqlTransaction trans, DataTable dt)
        {
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter();
                parmArray[0].ParameterName = "@DependentSpendFactorType";
                parmArray[0].Value = dt;

                this.RunSP(conn, trans, "DependentSpendFactorInsert", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

        }

    }
}