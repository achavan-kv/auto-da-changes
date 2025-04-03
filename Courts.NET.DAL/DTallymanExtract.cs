using System;
using System.Data.SqlClient;
using System.Data;

namespace STL.DAL
{
    /// <summary>
    /// Summary description for DTallymanExtract.
    /// </summary>
    public class DTallymanExtract
    {
        public DTallymanExtract()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public static bool NewAccountsExtract(SqlConnection conn, SqlTransaction trans)
        {
            //try
            //{
            //SqlCommand cmd = new SqlCommand("TallymanWeeklyExport", conn,trans);
            SqlCommand cmd = new SqlCommand("exec msdb..sp_start_job 'TallymanWeeklyExtract'", conn, trans);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = conn.ConnectionTimeout;
            cmd.ExecuteNonQuery();
            //}
            //catch(Exception ex)
            //{
            //    string a = ex.Message;
            //}

            return true;
        }

        public static bool UpdateExistingDetails(SqlConnection conn, SqlTransaction trans)
        {
            //SqlCommand cmd = new SqlCommand("TallymanDailyExport", conn,trans);
            SqlCommand cmd = new SqlCommand("exec msdb..sp_start_job 'TallymanDailyUpdate'", conn, trans);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = conn.ConnectionTimeout;
            cmd.ExecuteNonQuery();

            return true;
        }

        public static bool ImportTallymanSegments(SqlConnection conn, SqlTransaction trans)
        {
            SqlCommand cmd = new SqlCommand("exec msdb..sp_start_job 'TallymanSegmentImport'", conn, trans);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = conn.ConnectionTimeout;
            cmd.ExecuteNonQuery();

            return true;
        }

        public static bool RunJob(SqlConnection conn, SqlTransaction trans, string jobName)
        {
            SqlCommand cmd = new SqlCommand("exec msdb..sp_start_job '" + jobName + "'", conn, trans);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = conn.ConnectionTimeout;
            cmd.ExecuteNonQuery();

            return true;
        }

        public static SqlDataReader MonitorJobStatus(SqlConnection conn, SqlTransaction trans, string jobName)
        {
            SqlCommand cmd = new SqlCommand("exec msdb..sp_help_job @job_name='" + jobName + "'", conn, trans);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = conn.ConnectionTimeout;
            return cmd.ExecuteReader();
        }
    }
}

