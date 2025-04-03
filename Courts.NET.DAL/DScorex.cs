using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common;
using STL.Common.Constants.Enums;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;

namespace STL.DAL
{
	/// <summary>
	/// This procedure executes the processes to 
	/// create the Scorex Delinquency and Application data
	/// and uploads the data to the FTP site
	/// </summary>
	public class DScorex : DALObject
	{
		public DScorex()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public void RunScorexExtract(SqlConnection conn, SqlTransaction trans)
		{	
			//scorexData(conn, trans);
			//scorexDelinquency(conn, trans);
			//scorexApplication(conn, trans);
			//ftp_ScorexDLQ(conn, trans);
			//ftp_ScorexAPP(conn, trans);
		}
		// Build ScorexData table
		public void ScorexData(SqlConnection conn, SqlTransaction trans)
		{
			string progress = "step1 - Scorex Data extract processing ...";
			Console.WriteLine(progress);

			try
			{
				this.RunSP(conn, trans, "DbScorexExtract");
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}
		// Create Delinquency data
		public void ScorexDelinquency(SqlConnection conn, SqlTransaction trans)
		{
			string progress = "step2 - Scorex Delinquency processing ...";
			Console.WriteLine(progress);

			try
			{
				this.RunSP(conn, trans, "scorexDelinq");
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}
		// Create Application data
		public void ScorexApplication(SqlConnection conn, SqlTransaction trans)
		{
			try
			{
				string progress = "step3 - Scorex Application processing ...";
				Console.WriteLine(progress);

				this.RunSP(conn, trans, "scorexApplication");
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}
		// ftp Delinquency data
		public void Ftp_ScorexDLQ(SqlConnection conn, SqlTransaction trans)
		{
			try
			{
				string progress = "step4 - FTP Delinquency Data ...";
				Console.WriteLine(progress);

				this.RunSP(conn, trans, "ftp_ScorexDLQ");
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
         }
		// ftp Application data
		public void Ftp_ScorexAPP(SqlConnection conn, SqlTransaction trans)
		{
			try
			{
				string progress = "step5 - FTP Application Data ...";
				Console.WriteLine(progress);

				this.RunSP(conn, trans, "ftp_ScorexAPP");
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}
       
        public string GetBCPpath(SqlConnection conn)
        {
            string path = "";
            try
            {
                path = this.ReturnScalarNoReturn("BCPpathget").ToString();
                return path;
            }

            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            
        }
	}
}
