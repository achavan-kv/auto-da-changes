using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.Unipay.Test
{
    public class DAL
    {
        string _Con = string.Empty;

        public DAL()
        {
            _Con = System.Configuration.ConfigurationManager.ConnectionStrings["Default"].ToString();
        }

        public DataTable RunSP(string _spName, SqlParameter[] parmArray)
        {

            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(_Con))
            {

                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {

                    SqlDataAdapter da = new SqlDataAdapter(_spName, conn);
                    da.SelectCommand.Parameters.AddRange(parmArray);
                    da.SelectCommand.Transaction = trans;
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    da.Fill(dt);

                    trans.Commit();
                }
                return dt;
            }

        }

        public String RunSPInsertUpdate(string _spName, SqlParameter[] parmArray)
        {
            string result = Unicomer.Cosacs.Model.TestResult.Pass;
            using (SqlConnection conn = new SqlConnection(_Con))
            {
                try
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        SqlCommand cmd = new SqlCommand(_spName, conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddRange(parmArray);
                        cmd.Transaction = trans;
                        cmd.ExecuteNonQuery();
                        trans.Commit();
                    }
                }
                catch (SqlException ex)
                {
                    result = Unicomer.Cosacs.Model.TestResult.Fail;
                }
                finally
                {

                    string progress = "Finished Api testing ";
                    Console.WriteLine(progress);

                }
                return result;
            }

        }
    }
}
