using System;
using STL.DAL;
using STL.Common;
using STL.Common.Static;
using System.Data;
using System.Xml;
using System.Data.SqlClient;
using STL.Common.Constants.FTransaction;
using STL.Common.Constants.Delivery;
using STL.Common.Constants.Tags;
using STL.Common.Constants.Elements;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.ItemTypes;
using STL.Common.Constants.EOD;

namespace STL.BLL
{
	/// <summary>
	/// Summary description for BInterfaceControl.
	/// </summary>
	public class BInterfaceControl : CommonObject
	{
		public BInterfaceControl()
		{
		}

        public DataSet GetInterface(string eodInterface, string eodInterface2, bool allRuns)        //UAT1010 jec 09/07/10
		{
			DataSet ds = new DataSet();
			DInterfaceControl ifControl = new DInterfaceControl();
			ifControl.Interface = eodInterface;
            ifControl.Interface2 = eodInterface2;       //UAT1010 jec 09/07/10
            ifControl.GetInterface(allRuns);
			ds.Tables.Add(ifControl.Control);

			return ds;
		}

		public DataSet GetEODControl()
		{
			DataSet ds = new DataSet();
			DInterfaceControl ifControl = new DInterfaceControl();
			ifControl.GetEODControl();
			ds.Tables.Add(ifControl.Control);

			return ds;
		}

		public void EODControlUpdate(SqlConnection conn, SqlTransaction trans, DataSet controls)
		{
			string eodInterface = "";
			string donextrun = ""; 
			string dodefault = ""; 

			DInterfaceControl ic = new DInterfaceControl();
			foreach(DataTable dt in controls.Tables)
			{
				foreach(DataRow r in dt.Rows)
				{
					eodInterface = (string)r[CN.Interface];
					donextrun = (string)r[CN.DoNextRun]; 
					dodefault = (string)r[CN.DoDefault]; 
					
					ic.EODControlUpdate(conn, trans, eodInterface, donextrun, dodefault);
				}
			}
		}

		public DataSet GetInterfaceValue(string eodInterface, int runno)
		{
			DataSet ds = new DataSet();
			DInterfaceControl ifControl = new DInterfaceControl();
			ifControl.GetInterfaceValue(eodInterface, runno);
			ds.Tables.Add(ifControl.Control);

			return ds;
		}

		public DataSet GetInterfaceFinancial(int runno)
		{
			DataSet ds = new DataSet();
			DInterfaceControl ifControl = new DInterfaceControl();
			ifControl.GetInterfaceFinancial(runno);
			ds.Tables.Add(ifControl.Control);

			return ds;
		}

		public DataSet GetInterfaceBreakdown(int runno, int branchNo, string interfaceAcctNo)
		{
			DataSet ds = new DataSet();
			DInterfaceControl ifControl = new DInterfaceControl();
			ifControl.GetInterfaceBreakdown(runno, branchNo, interfaceAcctNo);
			ds.Tables.Add(ifControl.Control);

			return ds;
		}

		public DataSet GetInterfaceTransactions(int runno, int empeeno, string code, 
			string interfaceAcctNo, int branchNo)
		{
			DataSet ds = new DataSet();
			DInterfaceControl ifControl = new DInterfaceControl();
			ifControl.GetInterfaceTransactions(runno, empeeno, code, interfaceAcctNo, branchNo);
			ds.Tables.Add(ifControl.Control);

			return ds;
		}

		public DataSet GetSummaryUpdateControlDetails(int firstrunno, int lastrunno, bool useLiveDatabase)
		{
			DataSet ds = new DataSet();
            
            // instantiate ifContol with the seected database
            CosacsDatabase cosacsDatabase = useLiveDatabase ? CosacsDatabase.Live : CosacsDatabase.Reporting;

            DInterfaceControl ifControl = new DInterfaceControl(cosacsDatabase);
			ifControl.GetSummaryUpdateControlDetails(firstrunno,lastrunno);
			ds.Tables.Add(ifControl.Control);

			return ds;
		}
        public DataSet GetSummaryControlBrancgFigures(int runno, bool useLiveDatabase)
		{
			DataSet ds = new DataSet();

            // instantiate ifContol with the seected database
            CosacsDatabase cosacsDatabase = useLiveDatabase ? CosacsDatabase.Live : CosacsDatabase.Reporting;

			DInterfaceControl ifControl = new DInterfaceControl();
			ifControl.GetSummaryControlBrancgFigures(runno);
			ds.Tables.Add(ifControl.Control);

			return ds;
		}
		public DataSet GetSummaryControlTotals(int runNo, int branchNo, string type, bool useLiveDatabase)
		{
			DataSet ds = null;

            // instantiate ifContol with the seected database
            CosacsDatabase cosacsDatabase = useLiveDatabase ? CosacsDatabase.Live : CosacsDatabase.Reporting;

			DInterfaceControl ifControl = new DInterfaceControl();
			switch (type)
			{
				case "INTERESTBYACCOUNT":
					ds = ifControl.GetInterestByAccount(runNo, branchNo);
					break;
				default:
					ifControl.GetSummaryControlTotals(runNo, branchNo, type);
					ds = new DataSet();
					ds.Tables.Add(ifControl.Control);
					break;
			}
			return ds;
		}

		public DataTable GetEodOptionList (string configurationName)
		{
			DInterfaceControl ic = new DInterfaceControl();
			return ic.GetEodOptionList(configurationName);
		}

		public void ResetConfiguration (string configurationName)
		{
			// Clear the result status and current steps ready for a new EOD run
			DInterfaceControl ic = new DInterfaceControl();
			ic.ResetConfiguration(configurationName);
		}

        public int StartNextRun(string configurationName, string optionCode, out bool rerun, out string filedate)     //jec 08/04/11 RI
		{
			// Add a new Interface Control entry and return the new run no
			DInterfaceControl ic = new DInterfaceControl();
            return ic.StartNextRun(configurationName, optionCode, out rerun, out filedate);
		}

		public int SetNextStep(string configurationName, string optionCode)
		{
			// Overloaded so that if the step no is not specified
			// it will be defaulted to zero and incremented instead

			// The new step no is returned
			return this.SetNextStep(configurationName, optionCode, 0);
		}

		public int SetNextStep(string configurationName, string optionCode, int stepNo)
		{
			// Mark the EodConfigurationOption as being on the next step
			DInterfaceControl ic = new DInterfaceControl();
			// The new step no is returned
			return ic.SetNextStep(configurationName, optionCode, stepNo);
		}

		public void SetRunComplete(string configurationName, string optionCode, int runNo, string eodResult)
		{
			// Mark the Interface Control entry as finished
			DInterfaceControl ic = new DInterfaceControl();
			ic.SetRunComplete(configurationName, optionCode, runNo, eodResult);
		}

		public void DatabaseBackup(string interfaceName)
		{
			// Database Backup
			DInterfaceControl ic = new DInterfaceControl();
			ic.DatabaseBackup(interfaceName);
		}

		public string AutomatedBDW() 
		{
			// Automated Bad Debt Write off
			DInterfaceControl ic = new DInterfaceControl();
			return ic.AutomatedBDW();
		}
		public string GenerateBDW() 
		{
			// Generate Bad Debt Write off accounts
			DInterfaceControl ic = new DInterfaceControl();
			return ic.GenerateBDW();
		}
		public string RebateReport() 
		{
			// Create rebate report data
			DInterfaceControl ic = new DInterfaceControl();
			return ic.RebateReport();
		}

		public string SeasonedData() 
		{
			// Create rebate report data
			DInterfaceControl ic = new DInterfaceControl();
			return ic.SeasonedData();
		}

		public string StandingOrderP1(int runNo) 
		{
			// Standing Order Load Processing 
			DInterfaceControl so = new DInterfaceControl();
			return so.StandingOrderP1(runNo);
		}

        public string StandingOrderP1(SqlConnection conn, SqlTransaction trans, int runNo)
        {
            // Standing Order Load Processing 
            DInterfaceControl so = new DInterfaceControl();
            return so.StandingOrderP1(conn, trans, runNo);
        }

		public void SaveEODConfigurationOptions(SqlConnection con, SqlTransaction tran, 
			string configurationName, string country, int freqType, int startDate,
            int startTime, DataSet ds, DateTime configStartDate, string url)
		{
			DInterfaceControl ic = new DInterfaceControl();
			ic.User = this.User;

			//Delete this configuration
			ic.DeleteEODConfiguration(con, tran, configurationName);
			
			//Save the new configuration
            ic.SaveEODConfiguration(con, tran, configurationName, configStartDate, freqType);

			//Delete all the options for this configuration
			ic.DeleteEODConfigurationOptions(con, tran, configurationName);

			//Then add all options in the new dataset
			foreach (DataRow row in ds.Tables[0].Rows)
			{
				ic.SaveEODConfigurationOptions(con, tran, configurationName,
                    (string)row[CN.OptionCode], Convert.ToInt16(row[CN.SortOrder]), row.IsNull(CN.ReRunNo)?0:Convert.ToInt16(row[CN.ReRunNo]));      //jec 05/04/11
			}
            
			ic.EODSaveJob(configurationName, country, freqType, startDate, startTime,url);
		}

		public string GetEODStatus(string configurationName, string optionCode)
		{
			DInterfaceControl ic = new DInterfaceControl();
			return ic.GetEODStatus(configurationName, optionCode);
		}

		public DataSet GetEODAdHocScripts()
		{
			DataSet ds = new DataSet();
			DInterfaceControl ic = new DInterfaceControl();

			ds.Tables.Add(ic.GetEODAdHocScripts("D"));
			ds.Tables.Add(ic.GetEODAdHocScripts("O"));

			return ds;
		}

		public void EODStartJob(string configurationName, string country)
		{
			DInterfaceControl ic = new DInterfaceControl();
			ic.User = this.User;
			//ic.EODSaveJob(configurationName, country);

			ic.EODStartJob();
		}

        public void DeleteEODConfiguration(SqlConnection con, SqlTransaction tran,
                                            string configurationName)
        {
            DInterfaceControl ic = new DInterfaceControl();
            ic.User = this.User;

            //Delete this configuration
            ic.DeleteEODConfiguration(con, tran, configurationName);

            //Delete all the options for this configuration
            ic.DeleteEODConfigurationOptions(con, tran, configurationName);
        }

        public void ProductImport(SqlConnection conn, SqlTransaction trans)
        {
            DStockItem stock = new DStockItem();
            stock.ProductDataLoad(conn, trans);
            stock.ProductImport(conn, trans);
        }

        public void NonStockProductImport(SqlConnection conn, SqlTransaction trans)
        {
            DStockItem stock = new DStockItem();
            stock.NonStockProductDataLoad(conn, trans);
            stock.NonStockProductImport(conn, trans);
        }

        public void KitProductImport(SqlConnection conn, SqlTransaction trans,
                                     string eodOption, int runNo)
        {
            DStockItem stock = new DStockItem();
            stock.KitProductDataLoad(conn, trans);
            stock.KitProductImport(conn, trans, eodOption, runNo);
        }

        public void PromoPriceImport(SqlConnection conn, SqlTransaction trans,
                                     string eodOption, int runNo)
        {
            DStockItem stock = new DStockItem();
            stock.PromoPriceDataLoad(conn, trans);
            stock.PromoPriceImport(conn, trans, eodOption, runNo);
        }

        public void NonStockPromoPriceImport(SqlConnection conn, SqlTransaction trans,
                                    string eodOption, int runNo)
        {
            DStockItem stock = new DStockItem();
            stock.NonStockPromoPriceDataLoad(conn, trans);
            stock.NonStockPromoPriceImport(conn, trans, eodOption, runNo);
        }

        public void StockQtyImport(SqlConnection conn, SqlTransaction trans,
                                   string eodOption, int runNo)
        {
            DStockItem stock = new DStockItem();
            stock.StockQtyDataLoad(conn, trans);
            stock.StockQtyImport(conn, trans, eodOption, runNo);
        }

        public void AssocProductImport(SqlConnection conn, SqlTransaction trans)
        {
            DStockItem stock = new DStockItem();
            stock.AssocProductDataLoad(conn, trans);
            stock.AssocProductImport(conn, trans);
        }

        public void AssociatedProductImport(SqlConnection conn, SqlTransaction trans, string source)
        {
            DStockItem stock = new DStockItem();
            stock.AssociatedProductDataLoad(conn, trans, source);
            stock.AssociatedProductImport(conn, trans, source);
        }

        public void CheckDiskSpace(string option, out bool enoughSpace, out bool pathError)
        {
            DInterfaceControl ic = new DInterfaceControl();
            ic.CheckDiskSpace(option, out enoughSpace, out pathError);
        }

        public DataTable GetEodOptionListDetails(string configurationName,
                                    out DateTime startDate, out int frequency)
        {
            DInterfaceControl ic = new DInterfaceControl();
            return ic.GetEodOptionListDetails(configurationName, out startDate, out frequency);
        }

        public void SetReRunStatus(SqlConnection conn, SqlTransaction trans, 
                                    string configurationName, string status)
        {
            DInterfaceControl ic = new DInterfaceControl();
            ic.SetReRunStatus(conn, trans, configurationName, status);
        }

        public void SaveFACT2000Options(SqlConnection conn, SqlTransaction trans, DateTime effDate,
                            string fullProduct, string excludeZeroStock, string processEOD,
                            string processEOW, string processEOP, string processCINT)
        {
            DInterfaceControl ic = new DInterfaceControl();
            ic.SaveFACT2000Options(conn, trans, effDate, fullProduct, excludeZeroStock, processEOD,
                                   processEOW, processEOP, processCINT);
        }

        public string InstantCredit()
        {
            // Instant Credit Approval              CR907 jec 31/07/07
            DInterfaceControl inst = new DInterfaceControl();
            return inst.InstantCredit();
        }

        public string CashLoanQualification()
        {
            // Cash Loan Qualification Processing               CR906 jec 04/09/07
            DInterfaceControl loan = new DInterfaceControl();
            return loan.CashLoanQualification();
        }
        // WeeklyTrading Report Processing               CR975 jec 22/08/11
        public string WeeklyTrading()
        {
            DInterfaceControl wtr = new DInterfaceControl();
            return wtr.WeeklyTrading();
        }

        // Hyperion data extract Processing   CR10450 jec 12/10/12
        public string HyperionExtract()
        {
            DInterfaceControl hyp = new DInterfaceControl();
            return hyp.HyperionExtract();
        }
	}
}
