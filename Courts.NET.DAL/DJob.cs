using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common.Constants.Enums;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;


namespace STL.DAL
{
	/// <summary>
	/// Summary description for DJob.
	/// </summary>
	public class DJob : DALObject
    {
		private int _coSACSId;
		private int _runSeq;
		private string _jobName;
		private string _description;
		private int _enabled;
		private int _freqType;
		private int _freqInterval;

		private DataTable _jobList = null;

		public int CoSACSId
		{
			get{return _coSACSId;}
			set{_coSACSId = value;}
		}

		public int RunSeq
		{
			get{return _runSeq;}
			set{_runSeq = value;}
		}

		public string JobName
		{
			get{return _jobName;}
			set{_jobName = value;}
		}

		public string Description
		{
			get{return _description;}
			set{_description = value;}
		}

		public int Enabled
		{
			get{return _enabled;}
			set{_enabled = value;}
		}

		public int FreqType
		{
			get{return _freqType;}
			set{_freqType = value;}
		}

		public int FreqInterval
		{
			get{return _freqInterval;}
			set{_freqInterval = value;}
		}

		public DataTable jobList
		{
			get{return _jobList;}
			set{_jobList = value;}
		}


		public int GetJobList()
		{
			int result = 0;

			try
			{
			//	_jobList = new DataTable(TN.JobList);
				result = this.RunSP("DN_GetJobListSP", _jobList);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}


		public int SaveJob(SqlConnection conn, SqlTransaction trans)
		{
			int result = 0;

			try
			{
				parmArray = new SqlParameter[6];

				parmArray[0] = new SqlParameter("@CoSACSId", SqlDbType.Int);
				parmArray[0].Value = this._coSACSId;
				parmArray[1] = new SqlParameter("@RunSeq", SqlDbType.Int);
				parmArray[1].Value = this._runSeq;
				parmArray[2] = new SqlParameter("@JobName", SqlDbType.NVarChar,128);
				parmArray[2].Value = this._jobName;
				parmArray[3] = new SqlParameter("@Description", SqlDbType.NVarChar,512);
				parmArray[3].Value = this._description;
				parmArray[4] = new SqlParameter("@Enabled", SqlDbType.Int);
				parmArray[4].Value = this._enabled;
				parmArray[5] = new SqlParameter("@FreqType", SqlDbType.Int);
				parmArray[5].Value = this._freqType;
				parmArray[6] = new SqlParameter("@FreqInterval", SqlDbType.Int);
				parmArray[6].Value = this._freqInterval;

				result = this.RunSP(conn, trans, "DN_SaveJobSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}
        /// <summary>
        /// Checks number of days since last Collections commission job has been run. Returns 1000 if OK. 
        /// </summary>
        /// <param name="NumdaysSinceLastRun"></param>
        public void CheckCommissionDays (out int NumdaysSinceLastRun)
        {
            try
            {
                NumdaysSinceLastRun =1000; 
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@NumdaysSinceLastRun", SqlDbType.Int);
                parmArray[0].Value = NumdaysSinceLastRun;
                parmArray[0].Direction = ParameterDirection.Output;

                RunSP("CheckCommissionDays", parmArray);

                if (!Convert.IsDBNull(parmArray[0].Value))
                    NumdaysSinceLastRun = (Int32)parmArray[0].Value;

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }



		public DJob()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}
}
