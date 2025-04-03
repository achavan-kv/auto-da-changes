using System;
using STL.DAL;
using System.Data.SqlClient;

namespace STL.BLL
{
    /// <summary>
    /// Summary description for Class1.
    /// </summary>
    public class BTallymanExtract
    {
        public BTallymanExtract()
        {
        }

        public static TallymanExtractResponse Run(SqlConnection conn, SqlTransaction trans, TallymanExtractRequest tallyReq)
        {
            TallymanExtractResponse response = new TallymanExtractResponse();

            if (tallyReq.NewAccountsExtract)
                response.NewAccountSuccess = DTallymanExtract.NewAccountsExtract(conn, trans);
            if (tallyReq.UpdateExistingDetails)
                response.UpdateAccountSuccess = DTallymanExtract.UpdateExistingDetails(conn, trans);

            return response;
        }

        public static bool RunJobs(SqlConnection conn, SqlTransaction trans, string[] jobNameList)
        {
            foreach (string jobName in jobNameList)
            {
                DTallymanExtract.RunJob(conn, trans, jobName);
            }
            return true;
        }

        public static TallymanImportResponse RunImport(SqlConnection conn, SqlTransaction trans)
        {

            TallymanImportResponse response = new TallymanImportResponse();
            response.success = DTallymanExtract.ImportTallymanSegments(conn, trans);
            return response;
        }

        public static MonitorResponse MonitorJobStatus(SqlConnection conn, SqlTransaction trans, string jobName)
        {
            #region returned value definitions
            //current_execution_status
            //1 = running
            //4= idle

            //notify_level_eventlog
            //0 = success
            //2 = Failed
            #endregion

            MonitorResponse response = new MonitorResponse();
            using (SqlDataReader rdr = DTallymanExtract.MonitorJobStatus(conn, trans, jobName))
            {

                if (rdr.Read())
                {
                    response.CurrentExecutionStatus = Convert.ToInt32(rdr["current_execution_status"]);
                    response.NotifyEventLog = Convert.ToInt32(rdr["notify_level_eventlog"]);
                    response.LastRunOutcome = Convert.ToInt32(rdr["last_run_outcome"]);

                    //rdr.NextResult();
                    //rdr.NextResult();
                    //rdr.NextResult();
                    //if (rdr.Read())
                    //{
                    // TODO  set last_run_date last_run_time
                    if (rdr["last_run_date"].ToString() != "0")
                    {
                        if (rdr["last_run_time"] == DBNull.Value)
                            response.DateLastRun = DateTime.Parse(long.Parse(rdr["last_run_date"].ToString()).ToString("####/##/##"));
                        else
                        {
                            string lastRunTime = rdr["last_run_time"].ToString().PadLeft(6, '0');


                            response.DateLastRun = DateTime.Parse(long.Parse(rdr["last_run_date"].ToString() + lastRunTime).ToString("####/##/## ##:##:##"));
                        }
                    }

                    //}
                }
            }

            return response;
        }
    }

    public struct TallymanExtractRequest
    {
        public bool NewAccountsExtract;
        public bool UpdateExistingDetails;
    }

    public struct TallymanExtractResponse
    {
        public bool NewAccountSuccess;
        public bool UpdateAccountSuccess;
    }

    public struct TallymanImportResponse
    {
        public bool success;
    }

    public struct MonitorResponse
    {
        public int CurrentExecutionStatus;
        public int NotifyEventLog;
        public int LastRunOutcome;
        public DateTime DateLastRun;
    }



}

