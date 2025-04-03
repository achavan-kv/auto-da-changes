using System;
using STL.Common;
using System.Data;
using STL.DAL;
using System.Xml;
using STL.Common.Constants.ColumnNames;
using System.Data.SqlClient;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.Tags;

namespace STL.BLL
{
    public class BSpend : CommonObject
    {
        public decimal GetApplicantSpendFactor(SqlConnection conn, SqlTransaction trans,string income)
        {
            decimal spendFactor = 0;
            DSpend ds = new DSpend();
            try
            {
                spendFactor = ds.GetApplicantSpendFactor(conn, trans,income);
            }
            catch (Exception e)
            {
                throw e;
            }

            return spendFactor;
        }

        public decimal GetDependentSpendFactor(SqlConnection conn, SqlTransaction trans,decimal noOfDep)
        {
            decimal spendFactor = 0;
            DSpend ds = new DSpend();
            try
            {
                spendFactor = ds.GetDependentSpendFactor(conn, trans,noOfDep);
            }
            catch (Exception e)
            {
                throw e;
            }

            return spendFactor;

        }
        public DataTable GetApplicantSpendFactorMatrix(SqlConnection conn, SqlTransaction trans)
        {
            DataTable dt = new DataTable();
            DSpend ds = new DSpend();
            try
            {
                dt = ds.GetApplicantSpendFactorMatrix(conn, trans);
            }
            catch (Exception e)
            {
                throw e;
            }


            return dt;
        }
        public void SaveApplicantSpendFactorMatrix(SqlConnection conn, SqlTransaction trans, DataTable dt)
        {
            DSpend ds = new DSpend();
            try
            {
                ds.SaveApplicantSpendFactorMatrix(conn, trans, dt);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DataTable GetDependentSpendFactorMatrix(SqlConnection conn, SqlTransaction trans)
        {
            DataTable dt = new DataTable();
            DSpend ds = new DSpend();
            try
            {
                dt = ds.GetDependentSpendFactorMatrix(conn, trans);
            }
            catch(Exception e)
            {
                throw e;
            }


            return dt;
        }
        public void SaveDependentSpendFactorMatrix(SqlConnection conn, SqlTransaction trans, DataTable dt)
        {
            DSpend ds = new DSpend();
            try
            {
                ds.SaveDependentSpendFactorMatrix(conn, trans, dt);
            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }
}
