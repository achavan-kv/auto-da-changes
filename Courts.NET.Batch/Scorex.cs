using System;
using STL.Common;
using STL.BLL;
using System.Data;
using STL.Common.Constants.ColumnNames;
using System.Data.SqlClient;
using System.Configuration;
using System.Reflection;
using System.Xml;
using System.IO;
using STL.DAL;
using STL.Common.Constants.EOD;
using System.Diagnostics;


namespace STL.Batch
{
	/// <summary>
	/// Summary description for Scorex.
	/// </summary>
	public class Scorex : CommonObject
	{
        private new int _user = 0;
		public new int User
		{
			get{return _user;}
			set{_user = value;}
		}

		private string _countrycode = "";
		public string CountryCode
		{
			get{return _countrycode;}
			set{_countrycode = value;}			
		}

		private int _runno = 0;
		public int RunNumber
		{
			get{return _runno;}
			set{_runno = value;}
		}

		private string _interface = "";
		public string Interface
		{
			get{return _interface;}
			set{_interface = value;}			
		}

		public Scorex()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public string RunScorexExtract()
		{	
			string result = EODResult.Pass;

            result = this.ScorexData();
            if (result == EODResult.Pass)
                result = this.ScorexDelinquency();
			
			this.ExportDelinquency();
			
			if (result == EODResult.Pass)
				result = this.ScorexApplication();

			this.ExportApplication();

			if (result == EODResult.Pass)
				result = this.Ftp_ScorexDLQ();
			if (result == EODResult.Pass)
				result = this.Ftp_ScorexAPP();
			
			return result;
		}
		// update ScorexData table
		private string ScorexData()
		{
			string eodResult = EODResult.Pass;
			SqlConnection conn = null;
		
			try
			{
				conn = new SqlConnection(Connections.Default);
				do
				{
					try
					{
						conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            // Execute Stored procedures
                            DScorex ds = new DScorex();
                            ds.ScorexData(conn, trans);
                            trans.Commit();
                        }
						break;
					}
					catch(SqlException ex)
					{
						if(ex.Number==Deadlock && retries < maxRetries)
						{
							retries++;
							if(conn.State != ConnectionState.Closed)
								conn.Close();
						}
						else
							throw ex;
					}
				} while(retries <= maxRetries);
			}
			catch(Exception ex)
			{
				eodResult = EODResult.Fail;
				// Pass this on for logging in EODConfiguration
				throw ex;
			}
			finally
			{
				if(conn.State != ConnectionState.Closed)
					conn.Close();
			}
			return eodResult;
		}
		// Scorex Delinquency
		private string ScorexDelinquency()
		{
			string eodResult = EODResult.Pass;
			SqlConnection conn = null;
			
			try
			{
                conn = new SqlConnection(Connections.Default);
				do
				{
					try
					{
						conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            // Execute Stored procedures
                            DScorex ds = new DScorex();
                            ds.ScorexDelinquency(conn, trans);
                            trans.Commit();
                        }
						break;
					}
					catch(SqlException ex)
					{
						if(ex.Number==Deadlock && retries < maxRetries)
						{
							retries++;
							if(conn.State != ConnectionState.Closed)
								conn.Close();
						}
						else
							throw ex;
					}
				} while(retries <= maxRetries);
			}
			catch(Exception ex)
			{
				eodResult = EODResult.Fail;
				// Pass this on for logging in EODConfiguration
				throw ex;
			}
			finally
			{
				if(conn.State != ConnectionState.Closed)
					conn.Close();
			}
			return eodResult;
		}

		// Scorex Application
		private string ScorexApplication()
		{
			string eodResult = EODResult.Pass;
			SqlConnection conn = null;
			
			try
			{
				conn = new SqlConnection(Connections.Default);
				do
				{
					try
					{
						conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            // Execute Stored procedures
                            DScorex ds = new DScorex();
                            ds.ScorexApplication(conn, trans);
                            trans.Commit();
                        }
						break;
					}
					catch(SqlException ex)
					{
						if(ex.Number==Deadlock && retries < maxRetries)
						{
							retries++;
							if(conn.State != ConnectionState.Closed)
								conn.Close();
						}
						else
							throw ex;
					}
				} while(retries <= maxRetries);
			}
			catch(Exception ex)
			{
				eodResult = EODResult.Fail;
				// Pass this on for logging in EODConfiguration
				throw ex;
			}
			finally
			{
				if(conn.State != ConnectionState.Closed)
					conn.Close();
			}
			return eodResult;
		}
		// Scorex FTP Delinquency
		private string Ftp_ScorexDLQ()
		{
			string eodResult = EODResult.Pass;
			SqlConnection conn = null;
			
			try
			{
                conn = new SqlConnection(Connections.Default);
				do
				{
					try
					{
						conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            // Execute Stored procedures
                            DScorex ds = new DScorex();
                            ds.Ftp_ScorexDLQ(conn, trans);
                            trans.Commit();
                        }
						break;
					}
					catch(SqlException ex)
					{
						if(ex.Number==Deadlock && retries < maxRetries)
						{
							retries++;
							if(conn.State != ConnectionState.Closed)
								conn.Close();
						}
						else
							throw ex;
					}
				} while(retries <= maxRetries);
			}
			catch(Exception ex)
			{
				eodResult = EODResult.Fail;
				// Pass this on for logging in EODConfiguration
				throw ex;
			}
			finally
			{
				if(conn.State != ConnectionState.Closed)
					conn.Close();
			}
			return eodResult;
		}
		// Scorex FTP Application
		private string Ftp_ScorexAPP()
		{
			string eodResult = EODResult.Pass;
			SqlConnection conn = null;
	
			try
			{
				conn = new SqlConnection(Connections.Default);
				do
				{
					try
					{
						conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            // Execute Stored procedures
                            DScorex ds = new DScorex();
                            ds.Ftp_ScorexAPP(conn, trans);
                            trans.Commit();
                        }
						break;
					}
					catch(SqlException ex)
					{
						if(ex.Number==Deadlock && retries < maxRetries)
						{
							retries++;
							if(conn.State != ConnectionState.Closed)
								conn.Close();
						}
						else
							throw ex;
					}
				} while(retries <= maxRetries);
			}
			catch(Exception ex)
			{
				eodResult = EODResult.Fail;
				// Pass this on for logging in EODConfiguration
				throw ex;
			}
			finally
			{
				if(conn.State != ConnectionState.Closed)
					conn.Close();
			}
			return eodResult;
		}
		// execute BCP to export Delinquency table
		private void ExportDelinquency()
		{
			string progress = "step2a - Exporting Delinquency data ...";
			Console.WriteLine(progress);
			
			string dbname=this.GetDatabaseName();

            string path = GetBCPpath();

            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Default"].ConnectionString);
            string servername = conn.DataSource;
			System.Diagnostics.Process proc = new System.Diagnostics.Process(); 
			proc.EnableRaisingEvents=false;
						
			proc.StartInfo.FileName= path + "\\bcp.exe";  
			proc.StartInfo.Arguments=dbname + @".dbo.delinquency out d:\users\default\ScorexDLQ.csv " + "-c -t, -q -T -S" + servername;
			proc.StartInfo.UseShellExecute = true;
			
			proc.Start(); 
			// Wait for completion
			do 
			{
				// Forces the Process component to get a new set 
				// of property values.
				//proc.Refresh();
				// Writes the property value to the console screen.
				//Console.WriteLine(proc.WorkingSet.ToString());
				// Waits two seconds before running the next loop.
				System.Threading.Thread.Sleep(2000);
			}while(proc.HasExited == false);

		}

		// execute BCP to export Application table
		private void ExportApplication()
		{
			string progress = "step3a - Exporting Application data ...";
			Console.WriteLine(progress);

			string dbname=this.GetDatabaseName();
            string path = GetBCPpath();

            System.Diagnostics.Process proc = new System.Diagnostics.Process(); 
			proc.EnableRaisingEvents=false;

            proc.StartInfo.FileName = path + "\\bcp.exe";   
			proc.StartInfo.Arguments=dbname + @".dbo.application out d:\users\default\ScorexAPP.csv " + "-c -t, -q -T";
			proc.StartInfo.UseShellExecute = true;
			
			proc.Start(); 
			// Wait for completion
			do
			{
				// Forces the Process component to get a new set 
				// of property values.
				//proc.Refresh();
				// Writes the property value to the console screen.
				//Console.WriteLine(proc.WorkingSet.ToString());
				// Waits two seconds before running the next loop.
				System.Threading.Thread.Sleep(2000);
			}while(proc.HasExited == false);
			
		}
		// Get database name from configuration settings
		private string GetDatabaseName()
		{
            /*
			int start = 0;
			int end = 0;
			string db = "";

			string conn = Connections.Default;
			start = conn.IndexOf(";database")+10;
			end = conn.Length; //conn.IndexOf(";", start);
			db = conn.Substring(start, (end-start));
			 
			return db;*/
            return Connections.DefaultDatabaseName;
           
		}

        private string GetBCPpath()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Connections.Default))
                {
                    DScorex DS = new DScorex();
                    return DS.GetBCPpath(conn);
                }
            }
            catch 
            {
                throw;
            }

        }

	}

}
