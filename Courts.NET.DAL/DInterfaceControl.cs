using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common.Constants.Enums;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.EOD;
using System.Configuration;

namespace STL.DAL
{
	/// <summary>
	/// Summary description for DInterfaceControl.
	/// </summary>
	public class DInterfaceControl : DALObject
	{
		private string _interface = "";
		public string Interface
		{
			get{return _interface;}
			set{_interface = value;}
		}

        private string _interface2 = "";
        public string Interface2
        {
            get { return _interface2; }
            set { _interface2 = value; }
        }

        private DateTime _fact2000date = DateTime.MinValue.AddYears(1899);
        public DateTime FACT2000Date
        {
            get { return _fact2000date; }
            set { _fact2000date = value; }
        }

		private int _runno = 0;
		public int RunNumber 
		{
			get{return _runno;}
			set{_runno = value;}
		}

		private DataTable _control;
		public DataTable Control
		{
			get	{return _control;}
		}

		public void GetInterface(bool allRuns)
		{
			_control = new DataTable();

			try
			{
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@interface", SqlDbType.NVarChar,12);
				parmArray[0].Value = this.Interface;
                parmArray[1] = new SqlParameter("@interface2", SqlDbType.NVarChar, 12);     //UAT1010 jec 09/07/10
                parmArray[1].Value = this.Interface2;
                parmArray[2] = new SqlParameter("@allruns", SqlDbType.Bit);
                parmArray[2].Value = allRuns;

				this.RunSP("DN_InterfaceLoadSP", parmArray, _control);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void GetEODControl()
		{
			_control = new DataTable();

			try
			{
				this.RunSP("DN_EODControlGetSP", _control);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void EODControlUpdate(SqlConnection conn, SqlTransaction trans, string eodInterface,
			string donextrun, string dodefault)
		{
			try
			{
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@interface", SqlDbType.NVarChar, 12);
				parmArray[0].Value = eodInterface;
				parmArray[1] = new SqlParameter("@donextrun", SqlDbType.NChar, 1);
				parmArray[1].Value = donextrun;
				parmArray[2] = new SqlParameter("@dodefault", SqlDbType.NChar, 1);
				parmArray[2].Value = dodefault;

				this.RunSP(conn, trans, "DN_EODUpdateSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void GetInterfaceValue(string eodInterface, int runno)
		{
			_control = new DataTable();

			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@interface", SqlDbType.NVarChar,12);
				parmArray[0].Value = eodInterface;
				parmArray[1] = new SqlParameter("@runno", SqlDbType.Int);
				parmArray[1].Value = runno;
				
				this.RunSP("DN_InterfaceValueGetSP", parmArray, _control);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}


		public void AddInterfaceValue(string eodInterface, int runno, string counttype1, 
			string counttype2, int branchno, string accttype, int countvalue, decimal amount)
		{
			try
			{
				parmArray = new SqlParameter[8];
				parmArray[0] = new SqlParameter("@interface", SqlDbType.NVarChar,12);
				parmArray[0].Value = eodInterface;
				parmArray[1] = new SqlParameter("@runno", SqlDbType.Int);
				parmArray[1].Value = runno;
				parmArray[2] = new SqlParameter("@counttype1", SqlDbType.NVarChar, 25);
				parmArray[2].Value = counttype1;
				parmArray[3] = new SqlParameter("@counttype2", SqlDbType.NVarChar, 10);
				parmArray[3].Value = counttype2;
				parmArray[4] = new SqlParameter("@branchno", SqlDbType.Int);
				parmArray[4].Value = branchno;
				parmArray[5] = new SqlParameter("@accttype", SqlDbType.NVarChar, 10);
				parmArray[5].Value = accttype;
				parmArray[6] = new SqlParameter("@countvalue", SqlDbType.Int);
				parmArray[6].Value = countvalue;
				parmArray[7] = new SqlParameter("@value", SqlDbType.Money);
				parmArray[7].Value = amount;
				
				this.RunSP("DN_InterfaceValueAddSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}


		public void GetInterfaceFinancial(int runno)
		{
			_control = new DataTable();

			try
			{
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@runno", SqlDbType.Int);
				parmArray[0].Value = runno;
				
				this.RunSP("DN_InterfaceFinancialGetSP", parmArray, _control);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void GetInterfaceBreakdown(int runno, int branchNo, string interfaceAcctNo)
		{
			try
			{
				_control = new DataTable();

				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@runno", SqlDbType.Int);
				parmArray[0].Value = runno;
				parmArray[1] = new SqlParameter("@branch", SqlDbType.Int);
				parmArray[1].Value = branchNo;
				parmArray[2] = new SqlParameter("@interfaceacctno", SqlDbType.NVarChar, 10);
				parmArray[2].Value = interfaceAcctNo;

				this.RunSP("DN_GetInterfaceBreakdownTotalsSP", parmArray, _control);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void GetInterfaceTransactions(int runno, int empeeno, string code,
			string interfaceAcctNo, int branchNo)
		{
			try
			{
				_control = new DataTable();

				parmArray = new SqlParameter[5];
				parmArray[0] = new SqlParameter("@runno", SqlDbType.Int);
				parmArray[0].Value = runno;
				parmArray[1] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[1].Value = empeeno;
				parmArray[2] = new SqlParameter("@code", SqlDbType.NVarChar, 4);
				parmArray[2].Value = code;
				parmArray[3] = new SqlParameter("@interfaceacctno", SqlDbType.NVarChar, 10);
				parmArray[3].Value = interfaceAcctNo;
				parmArray[4] = new SqlParameter("@branchno", SqlDbType.Int);
				parmArray[4].Value = branchNo;

				this.RunSP("DN_InterfaceTransactionsGetSP", parmArray, _control);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}
		
		public void GetSummaryUpdateControlDetails(int firstrunno, int lastrunno)
		{
			try
			{

				_control = new DataTable(TN.SummaryControl);

				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@firstrunno", SqlDbType.Int);
				parmArray[0].Value = firstrunno;
				parmArray[1] = new SqlParameter("@lastrunno", SqlDbType.Int);
				parmArray[1].Value = lastrunno;

				this.RunSP("DN_GetSummaryUpdateControlDetailsSP", parmArray, _control);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void GetSummaryControlBrancgFigures(int runno)
		{
			try
			{

				_control = new DataTable(TN.SummaryControl);

				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@runno", SqlDbType.Int);
				parmArray[0].Value = runno;

				this.RunSP("DN_GetSummaryControlBrancgFiguresSP", parmArray, _control);
			}
			catch(SqlException ex)
			{	
				LogSqlException(ex);
				throw ex;
			}
		}

		public void GetSummaryControlTotals(int runno, int branchno, string type)
		{
			try
			{

				_control = new DataTable(TN.SummaryControl);

				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@runno", SqlDbType.Int);
				parmArray[0].Value = runno;
				parmArray[1] = new SqlParameter("@branchno", SqlDbType.SmallInt);
				parmArray[1].Value = Convert.ToInt16(branchno);
				parmArray[2] = new SqlParameter("@type", SqlDbType.NVarChar, 12);
				parmArray[2].Value = type;

				this.RunSP("dn_summarytotals_sp", parmArray, _control);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public DataSet GetInterestByAccount(int runno, int branchno)
		{
			DataSet interestSet = new DataSet();
			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@piRunNo", SqlDbType.Int);
				parmArray[0].Value = runno;
				parmArray[1] = new SqlParameter("@piBranchNo", SqlDbType.SmallInt);
				parmArray[1].Value = Convert.ToInt16(branchno);

				this.RunSP("DN_SUCRInterestByAccountSP", parmArray, interestSet);

				if (interestSet.Tables.Count > 0)
					interestSet.Tables[0].TableName = TN.InterestUnsettled;

				if (interestSet.Tables.Count > 1)
					interestSet.Tables[1].TableName = TN.InterestSettled;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return interestSet;
		}

		public DataTable GetEodOptionList (string configurationName)
		{
			// Return the list of EOD options for an EOD configuration
			DataTable eodOptionList = new DataTable();
			try
			{
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@configurationName", SqlDbType.NVarChar, 12);
				parmArray[0].Value = configurationName;

				RunSP("DN_EODConfigurationOptionsGetSP", parmArray, eodOptionList);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return eodOptionList;
		}

		public void ResetConfiguration (string configurationName)
		{
			// Clear the result status and current steps ready for a new EOD run
			try
			{
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@configurationName", SqlDbType.NVarChar, 12);
				parmArray[0].Value = configurationName;

				RunSP("DN_EODResetConfigurationSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

        public int StartNextRun(string configurationName, string optionCode, out bool rerun, out string filedate)     //jec 08/04/11 RI
		{
			// Add a new Interface Control entry and return the new run no
			int nextRunNo = 1;
            rerun = false;
            filedate = "";
			try
			{
				parmArray = new SqlParameter[5];
				parmArray[0] = new SqlParameter("@configurationName", SqlDbType.NVarChar, 12);
				parmArray[0].Value = configurationName;
				parmArray[1] = new SqlParameter("@optionCode", SqlDbType.NVarChar, 12);
				parmArray[1].Value = optionCode;
				parmArray[2] = new SqlParameter("@runNo", SqlDbType.Int);
				parmArray[2].Direction = ParameterDirection.Output;
                parmArray[3] = new SqlParameter("@rerun", SqlDbType.Bit);       //jec 08/04/11 RI
                parmArray[3].Direction = ParameterDirection.Output;
                parmArray[4] = new SqlParameter("@fileYYMMDD", SqlDbType.Char, 6);       //jec 08/04/11 RI
                parmArray[4].Direction = ParameterDirection.Output;

				result = this.RunSP("DN_EODStartNextRunSP", parmArray);
						
				if (result == 0)
				{
					if(parmArray[2].Value != DBNull.Value)
						nextRunNo = (int)parmArray[2].Value;
                    if (parmArray[3].Value != DBNull.Value)     //jec 08/04/11 RI
                        rerun = (bool)parmArray[3].Value;
                    if (parmArray[4].Value != DBNull.Value)     //jec 08/04/11 RI
                        filedate = (string)parmArray[4].Value;
				}
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return nextRunNo;
		}

		public int SetNextStep(string configurationName, string optionCode, int stepNo)
		{
			// Set the EOD step values to the step no specified
			// or if zero is specified then simply increment the step

			// The step will not be incremented past its TotalSteps value
			int nextStepNo = stepNo;
			try
			{
				parmArray = new SqlParameter[4];
				parmArray[0] = new SqlParameter("@configurationName", SqlDbType.NVarChar, 12);
				parmArray[0].Value = configurationName;
				parmArray[1] = new SqlParameter("@optionCode", SqlDbType.NVarChar, 12);
				parmArray[1].Value = optionCode;
				parmArray[2] = new SqlParameter("@stepNo", SqlDbType.Int);
				parmArray[2].Value = stepNo;
				parmArray[3] = new SqlParameter("@nextStepNo", SqlDbType.Int);
				parmArray[3].Direction = ParameterDirection.Output;

				result = this.RunSP("DN_EODSetNextStepSP", parmArray);
						
				if (result == 0)
				{
					if(parmArray[3].Value != DBNull.Value)
						nextStepNo = (int)parmArray[3].Value;
				}
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}

			// The new step no is returned
			return nextStepNo;
		}

		public void SetRunComplete(string configurationName, string optionCode, int runNo, string eodResult)
		{
			// Mark the Interface Control entry and EODConfigurationOption as finished
			try
			{
				parmArray = new SqlParameter[4];
				parmArray[0] = new SqlParameter("@configurationName", SqlDbType.NVarChar, 12);
				parmArray[0].Value = configurationName;
				parmArray[1] = new SqlParameter("@optionCode", SqlDbType.NVarChar, 12);
				parmArray[1].Value = optionCode;
				parmArray[2] = new SqlParameter("@runNo", SqlDbType.Int);
				parmArray[2].Value = runNo;
				parmArray[3] = new SqlParameter("@eodResult", SqlDbType.NVarChar, 1);
				parmArray[3].Value = eodResult;

				result = this.RunSP("DN_EODSetRunCompleteSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}
		// Database Backup
		public void DatabaseBackup(string interfaceName)
		{
			
			try
			{
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@interfaceName", SqlDbType.NVarChar, 12);
				parmArray[0].Value = interfaceName;
				this.RunSP("DN_DatabaseBackup",parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			
		}

		// Automated Bad Debt Write off
		public string AutomatedBDW() 
		{
			string eodResult = EODResult.Fail;
			try
			{
                SqlParameter[] paramArr = new SqlParameter[1];
                paramArr[0] = new SqlParameter("@containsWarnings", SqlDbType.Bit);
                paramArr[0].Direction = ParameterDirection.Output;

				this.RunSP("dbautomatedBDW",paramArr);

                eodResult = Convert.ToBoolean(paramArr[0].Value)?  EODResult.Warning: EODResult.Pass;
			}
			catch(SqlException ex)
			{
				eodResult = EODResult.Fail;
				LogSqlException(ex);
				throw ex;
			}
		return eodResult;
		}

		// Generate Bad Debt Write off accounts
		public string GenerateBDW() 
		{
			string eodResult = EODResult.Pass;
			try
			{
				this.RunSP("dbgenerateaccts");
			}
			catch(SqlException ex)
			{
				eodResult = EODResult.Fail;
				LogSqlException(ex);
				throw ex;
			}
		return eodResult;
		}

		// Create rebate report data
		public string RebateReport() 
		{
			string eodResult = EODResult.Pass;
			try
			{
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@Acctno", SqlDbType.NVarChar, 12);
				parmArray[0].Value = "All";
				this.RunSP("DN_RebateSP",parmArray);
			}
			catch(SqlException ex)
			{
				eodResult = EODResult.Fail;
				LogSqlException(ex);
				throw ex;
			}
			return eodResult;
		}

		// Create summary report data
		public string SummaryRptData(short type) 
		{
			string eodResult = EODResult.Pass;
			try
			{
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@isfullrun", SqlDbType.SmallInt);
				parmArray[0].Value = type;
				this.RunSP("DN_SummaryRptDataSP", parmArray);
			}
			catch(SqlException ex)
			{
				eodResult = EODResult.Fail;
				LogSqlException(ex);
				throw ex;
			}
			return eodResult;
		}

		// Create summary report data
        public string SummaryRptDataFull(short type)  
		{
			string eodResult = EODResult.Pass;
			try
			{
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@isfullrun", SqlDbType.SmallInt);
                parmArray[0].Value = type;
				this.RunSP("DN_SummaryRptDataSP", parmArray);
			}
			catch(SqlException ex)
			{
				eodResult = EODResult.Fail;
				LogSqlException(ex);
				throw ex;
			}
			return eodResult;
		}

		// Create summary report data
		public string SeasonedData() 
		{
			
			string eodResult = EODResult.Pass;
			try
			{
				this.RunSP("DN_SeasonedDataSP");
			}
			catch(SqlException ex)
			{
				eodResult = EODResult.Fail;
				LogSqlException(ex);
				throw ex;
			}
			return eodResult;	
		}

        // Archiving
        public string Archiving()
        {
            string eodResult = EODResult.Pass;
            try
            {
                this.RunSP("DN_ArchiveReindexingSP");
            }
            catch (SqlException ex)
            {
                eodResult = EODResult.Fail;
                LogSqlException(ex);
                throw ex;
            }
            return eodResult;
        }

		// Standing Order Load Processing 
		public string StandingOrderP1(int runNo)
		{
			string eodResult = EODResult.Pass;
			
			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@runNo", SqlDbType.Int);
				parmArray[0].Value = runNo;
                parmArray[1] = new SqlParameter("@containsWarnings", SqlDbType.Bit);        //IP - 31/08/10 - CR1092 - COASTER to CoSACS Enhancements
                parmArray[1].Direction = ParameterDirection.Output;

				this.RunSP("DN_StandingOrderVal", parmArray);

                eodResult = Convert.ToBoolean(parmArray[1].Value) ? EODResult.Warning : EODResult.Pass;     //IP - 31/08/10 - CR1092 - COASTER to CoSACS Enhancements

			}
			catch(Exception ex)
			{
				eodResult = EODResult.Fail;
				// Pass this on for logging in EODConfiguration
				throw ex;
			}
			return eodResult;
		}

        public string StandingOrderP1(SqlConnection conn, SqlTransaction trans, int runNo)
        {
            string eodResult = EODResult.Pass;

            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@runNo", SqlDbType.Int);
                parmArray[0].Value = runNo;
                parmArray[1] = new SqlParameter("@containsWarnings", SqlDbType.Bit);        //IP - 31/08/10 - CR1092 - COASTER to CoSACS Enhancements
                parmArray[1].Direction = ParameterDirection.Output;

                this.RunSP(conn, trans, "DN_StandingOrderVal", parmArray);

                eodResult = Convert.ToBoolean(parmArray[1].Value) ? EODResult.Warning : EODResult.Pass;     //IP - 31/08/10 - CR1092 - COASTER to CoSACS Enhancements

            }
            catch (Exception ex)
            {
                eodResult = EODResult.Fail;
                // Pass this on for logging in EODConfiguration
                throw;
            }
            return eodResult;
        }

		public int GetEODConfigurations()
		{
			// Return the list of EOD configurations
			_control = new DataTable(TN.EODConfigurations);
			try
			{
				result = RunSP("DN_EODConfigurationsGetSP", _control);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

		public int DeleteEODConfigurationOptions(SqlConnection con, SqlTransaction tran, string configurationName)
		{
			try
			{
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@configname", SqlDbType.NVarChar,12);
				parmArray[0].Value = configurationName;

				result = this.RunSP(con, tran, "DN_EODConfigurationOptionsDeleteSP", parmArray);
			
				if(result==0)
					result = (int)Return.Success;
				else
					result = (int)Return.Fail;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

		public int DeleteEODConfiguration(SqlConnection con, SqlTransaction tran, string configurationName)
		{
			try
			{
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@configname", SqlDbType.NVarChar,12);
				parmArray[0].Value = configurationName;

				result = this.RunSP(con, tran, "DN_EODConfigurationDeleteSP", parmArray);
			
				if(result==0)
					result = (int)Return.Success;
				else
					result = (int)Return.Fail;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

        public int SaveEODConfiguration(SqlConnection con, SqlTransaction tran, 
                        string configurationName, DateTime configStartDate, int freqType)
		{
			try
			{
				parmArray = new SqlParameter[4];
				parmArray[0] = new SqlParameter("@configname", SqlDbType.NVarChar,12);
				parmArray[0].Value = configurationName;
                parmArray[1] = new SqlParameter("@FACT2000date", SqlDbType.DateTime);
                parmArray[1].Value = this.FACT2000Date;
                parmArray[2] = new SqlParameter("@startdate", SqlDbType.DateTime);
                parmArray[2].Value = configStartDate;
                parmArray[3] = new SqlParameter("@frequency", SqlDbType.Int);
                parmArray[3].Value = freqType;

				result = this.RunSP(con, tran, "DN_EODConfigurationSaveSP", parmArray);
			
				if(result==0)
					result = (int)Return.Success;
				else
					result = (int)Return.Fail;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

		public int SaveEODConfigurationOptions(SqlConnection con, SqlTransaction tran, 
			string configurationName, string optionCode, short sortOrder, int reRunNo)
		{
			try
			{
				parmArray = new SqlParameter[5];
				parmArray[0] = new SqlParameter("@configname", SqlDbType.NVarChar,12);
				parmArray[0].Value = configurationName;
				parmArray[1] = new SqlParameter("@optioncode", SqlDbType.NVarChar,16);
				parmArray[1].Value = optionCode;
				parmArray[2] = new SqlParameter("@user", SqlDbType.Int);
				parmArray[2].Value = this.User;
				parmArray[3] = new SqlParameter("@sortorder", SqlDbType.SmallInt);
				parmArray[3].Value = sortOrder;
                parmArray[4] = new SqlParameter("@reRunNo", SqlDbType.SmallInt);
                parmArray[4].Value = reRunNo;

				result = this.RunSP(con, tran, "DN_EODConfigurationOptionsSaveSP", parmArray);
			
				if(result==0)
					result = (int)Return.Success;
				else
					result = (int)Return.Fail;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

		public string GetEODStatus(string configurationName, string optionCode)
		{
			string status = "";
			try
			{
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@configname", SqlDbType.NVarChar,12);
				parmArray[0].Value = configurationName;
				parmArray[1] = new SqlParameter("@option", SqlDbType.NVarChar,12);
				parmArray[1].Value = optionCode;
				parmArray[2] = new SqlParameter("@status", SqlDbType.NChar,1);
				parmArray[2].Direction = ParameterDirection.Output;

				result = this.RunSP("DN_EODOptionGetStatusSP", parmArray);
			
				if (result == 0)
				{
					if(parmArray[2].Value != DBNull.Value)
						status = (string)parmArray[2].Value;
				}
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return status;
		}

		public void EODSaveJob(string configurationName, string country,int freqType, 
								int startDate, int startTime,string url)
		{
			try
			{
				parmArray = new SqlParameter[7];
				parmArray[0] = new SqlParameter("@configuration", SqlDbType.NVarChar,12);
				parmArray[0].Value = configurationName;
				parmArray[1] = new SqlParameter("@country", SqlDbType.NChar,1);
				parmArray[1].Value = country;
				parmArray[2] = new SqlParameter("@user", SqlDbType.NVarChar, 10);
				parmArray[2].Value = this.User.ToString();
				parmArray[3] = new SqlParameter("@freqtype", SqlDbType.Int);
				parmArray[3].Value = freqType;
				parmArray[4] = new SqlParameter("@startdate", SqlDbType.Int);
				parmArray[4].Value = startDate;
				parmArray[5] = new SqlParameter("@starttime", SqlDbType.Int);
				parmArray[5].Value = startTime;
            parmArray[6] = new SqlParameter("@url", SqlDbType.NVarChar, 100);
            parmArray[6].Value = url;
                

				result = this.RunSP("DN_EODSaveJobSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void EODStartJob()
		{
			try
			{
				this.RunSP("DN_EODStartJobSP");
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}


		public DataTable GetEODAdHocScripts(string type)
		{
			try
			{
				_control = new DataTable();

				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@type", SqlDbType.NChar, 1);
				parmArray[0].Value = type;

				result = this.RunSP("DN_EODAdHocScriptsGetSP", parmArray, _control);
			
				if(result==0)
					result = (int)Return.Success;
				else
					result = (int)Return.Fail;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}

			return _control;
		}

        public string RunEODAdHocScripts(bool type)
        {
            //IP - 23/04/08 - UAT(260)
            string eodResult = EODResult.Pass;
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@before", SqlDbType.Bit);
                parmArray[0].Value = type;

                result = this.RunSP("DN_EODRunAdHocScriptsSP", parmArray);

                if (result == 0)
                    result = (int)Return.Success;
                else
                {
                    result = (int)Return.Fail;
                    eodResult = EODResult.Fail;     //CR1030 jec 
                }
            }
            catch (SqlException ex)
            {
                eodResult = EODResult.Fail;

                LogSqlException(ex);
                throw ex;
            }

            return eodResult;
        }

        public void RenameEODAdHocScripts()
        {
            try
            {
                result = this.RunSP("DN_EODRenameAdHocScriptsSP");
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void CheckDiskSpace(string option, out bool enoughSpace, out bool pathError)
        {
            string status = "";
            enoughSpace = false;
            pathError = false;

            try
            {
                parmArray = new SqlParameter[4];
                parmArray[0] = new SqlParameter("@BackupType", SqlDbType.VarChar, 12);
                parmArray[0].Value = option;
                parmArray[1] = new SqlParameter("@DatabaseSize", SqlDbType.Decimal);
                parmArray[1].Direction = ParameterDirection.Output;
                parmArray[2] = new SqlParameter("@FreeDiskSpace", SqlDbType.Decimal);
                parmArray[2].Direction = ParameterDirection.Output;
                parmArray[3] = new SqlParameter("@Ok", SqlDbType.Char, 2);
                parmArray[3].Direction = ParameterDirection.Output;

                result = this.RunSP("DN_DiskSpace", parmArray);

                if (parmArray[3].Value != DBNull.Value)
                    status = (string)parmArray[3].Value;

                enoughSpace = (status == "OK");
                pathError = (status == "PT");
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public DataTable GetEodOptionListDetails(string configurationName, out DateTime startDate, 
                                                    out int frequency)
        {
            startDate = DateTime.Now;
            frequency = 0;

            // Return the list of EOD options for an EOD configuration
            DataTable eodOptionList = new DataTable();
            try
            {
                parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@configurationName", SqlDbType.NVarChar, 12);
                parmArray[0].Value = configurationName;
                parmArray[1] = new SqlParameter("@startDate", SqlDbType.DateTime);
                parmArray[1].Direction = ParameterDirection.Output;
                parmArray[2] = new SqlParameter("@frequency", SqlDbType.Int);
                parmArray[2].Direction = ParameterDirection.Output;

                RunSP("DN_EODConfigurationOptionsGetDetailsSP", parmArray, eodOptionList);

                if (parmArray[1].Value != DBNull.Value)
                    startDate = (DateTime)parmArray[1].Value;
                if (parmArray[2].Value != DBNull.Value)
                    frequency = (int)parmArray[2].Value;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return eodOptionList;
        }

        public int SetReRunStatus(SqlConnection conn, SqlTransaction trans,
                                    string configurationName, string status)
        {
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@configname", SqlDbType.NVarChar, 12);
                parmArray[0].Value = configurationName;
                parmArray[1] = new SqlParameter("@status", SqlDbType.NVarChar, 6);
                parmArray[1].Value = status;

                result = this.RunSP(conn, trans, "DN_EODSetReRunStatusSP", parmArray);

                if (result == 0)
                    result = (int)Return.Success;
                else
                    result = (int)Return.Fail;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;
        }

        public bool IsEODReRun(string configurationName)
        {
            int isReRun = 0;

            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@configname", SqlDbType.NVarChar, 12);
                parmArray[0].Value = configurationName;
                parmArray[1] = new SqlParameter("@isrerun", SqlDbType.Int);
                parmArray[1].Value = 0;
                parmArray[1].Direction = ParameterDirection.Output;

                RunSP("DN_EODIsReRunSP", parmArray);

                if (!Convert.IsDBNull(parmArray[1].Value))
                    isReRun = (int)parmArray[1].Value;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return (isReRun > 0);
        }

        public int SaveFACT2000Options(SqlConnection conn, SqlTransaction trans, DateTime effDate,
                            string fullProduct, string excludeZeroStock, string processEOD,
                            string processEOW, string processEOP, string processCINT)
        {
            try
            {
                parmArray = new SqlParameter[7];
                parmArray[0] = new SqlParameter("@effectivedate", SqlDbType.DateTime);
                parmArray[0].Value = effDate;
                parmArray[1] = new SqlParameter("@fullproduct", SqlDbType.NChar, 1);
                parmArray[1].Value = fullProduct;
                parmArray[2] = new SqlParameter("@excludezerostock", SqlDbType.NChar, 1);
                parmArray[2].Value = excludeZeroStock;
                parmArray[3] = new SqlParameter("@processEOD", SqlDbType.NChar, 1);
                parmArray[3].Value = processEOD;
                parmArray[4] = new SqlParameter("@processEOW", SqlDbType.NChar, 1);
                parmArray[4].Value = processEOW;
                parmArray[5] = new SqlParameter("@processEOP", SqlDbType.NChar, 1);
                parmArray[5].Value = processEOP;
                parmArray[6] = new SqlParameter("@processCINT", SqlDbType.NChar, 1);
                parmArray[6].Value = processCINT;

                result = this.RunSP(conn, trans, "DN_FactAutoWriteSP", parmArray);

                if (result == 0)
                    result = (int)Return.Success;
                else
                    result = (int)Return.Fail;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;
        }

        public DataTable GetFactAutoDetails()
        {
            DataTable dtFactAuto = new DataTable();

            try
            {
                this.RunSP("DN_FactAutoGetSP", dtFactAuto);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            return dtFactAuto;
        }

        // Instant Credit Approval              CR907 jec 31/07/07
        public string InstantCredit()
        {
            string eodResult = EODResult.Pass;
            try
            {
                parmArray = new SqlParameter[5];                        // CR906 
                parmArray[0] = new SqlParameter("@piCustomerID", SqlDbType.VarChar, 20);
                parmArray[0].Value = " ";
                parmArray[1] = new SqlParameter("@piAccountNo", SqlDbType.VarChar, 12);    //jec 
                parmArray[1].Value = " ";
                parmArray[2] = new SqlParameter("@piProcess", SqlDbType.Char, 1);       // CR906 jec
                parmArray[2].Value = "I";       // Instant Credit
                parmArray[3] = new SqlParameter("@poInstantCredit", SqlDbType.Char, 1);
                parmArray[3].Value = string.Empty;
                parmArray[3].Direction = ParameterDirection.Output;
                parmArray[4] = new SqlParameter("@poLoanQualified", SqlDbType.Char, 1); // CR906 jec
                parmArray[4].Value = string.Empty;
                parmArray[4].Direction = ParameterDirection.Output;

                this.RunSP("InstantCreditApprovalsCheckGen", parmArray);
            }
            catch (SqlException ex)
            {
                eodResult = EODResult.Fail;
                LogSqlException(ex);
                throw ex;
            }
            return eodResult;
        }


		public DInterfaceControl()
		{

		}


        public DInterfaceControl(CosacsDatabase cosacsDatabase)
            : base(cosacsDatabase)
        {

        }

        // Cash Loan Qualification Processing             CR906 jec 04/09/07
        public string CashLoanQualification()
        {
            string eodResult = EODResult.Pass;
            try
            {
                parmArray = new SqlParameter[5];                        
                parmArray[0] = new SqlParameter("@piCustomerID", SqlDbType.VarChar, 20);
                parmArray[0].Value = " ";
                parmArray[1] = new SqlParameter("@piAccountNo", SqlDbType.VarChar, 12);    
                parmArray[1].Value = " ";
                parmArray[2] = new SqlParameter("@piProcess", SqlDbType.Char, 1);
                parmArray[2].Value = "L";       // Cash Loan
                parmArray[3] = new SqlParameter("@poInstantCredit", SqlDbType.Char, 1);
                parmArray[3].Value = string.Empty;
                parmArray[3].Direction = ParameterDirection.Output;
                parmArray[4] = new SqlParameter("@poLoanQualified", SqlDbType.Char, 1);  
                parmArray[4].Value = string.Empty;
                parmArray[4].Direction = ParameterDirection.Output;
                this.RunSP("InstantCreditApprovalsCheckGen", parmArray);
            }
            catch (SqlException ex)
            {
                eodResult = EODResult.Fail;
                LogSqlException(ex);
                throw ex;
            }
            return eodResult;
        }

        // WeeklyTrading Report Processing               CR975 jec 22/08/11
        public string WeeklyTrading()
        {
            string eodResult = EODResult.Pass;
            try
            {
                this.RunSP("WeeklyTradingDataSP");
            }
            catch (SqlException ex)
            {
                eodResult = EODResult.Fail;
                LogSqlException(ex);
                throw ex;
            }
            return eodResult;
        }

        //IP - 08/11/11 - Method to run Summary Reports
        public string SummaryRun()
        {
            string eodResult = EODResult.Pass;
            try
            {
                this.RunSP("SummaryEODSP");
            }
            catch (SqlException ex)
            {
                eodResult = EODResult.Fail;
                LogSqlException(ex);
                throw ex;
            }
            return eodResult;
        }

        // Hyperion data extract Processing   CR10450 jec 12/10/12
        public string HyperionExtract()
        {
            string eodResult = EODResult.Pass;
            try
            {
                this.RunSP("HyperionEODSP");
            }
            catch (SqlException ex)
            {
                eodResult = EODResult.Fail;
                LogSqlException(ex);
                throw ex;
            }
            return eodResult;
        }
	}
}
