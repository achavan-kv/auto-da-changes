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

namespace STL.Batch
{
	/// <summary>
	/// Summary description for EODConfiguration.
	/// </summary>
	public class EODConfiguration : CommonObject
	{
		private Logging _log;

		private string _configuration = "";
		public string configuration
		{
			get{return _configuration;}
			set{_configuration = value;}
		}

        private new int _user = 0;
		public int user
		{
			get{return _user;}
			set{_user = value;}
		}

		private string _countryCode = "";
		public string countryCode
		{
			get{return _countryCode;}
			set{_countryCode = value;}			
		}

        private bool _adHocScripts = false;
        public bool AdHocScripts
        {
            get { return _adHocScripts; }
            set { _adHocScripts = value; }
        }

		public EODConfiguration()
		{
			_log = new Logging();
		}

		public EODConfiguration(string configuration, int user, string countryCode)
		{
			_configuration = configuration;
			_user = user;
			_countryCode = countryCode;
			_log = new Logging();
			this.RunConfiguration();
		}

		/// <summary>
		/// Run this EOD configuration
		/// </summary>
		public void RunConfiguration()
		{
			string progress = "";

			try
			{
                DInterfaceControl ic = new DInterfaceControl();

                // Load the list of options in this configuration
				BInterfaceControl eodRun = new BInterfaceControl();				
				DataTable eodOptionList = eodRun.GetEodOptionList(this._configuration);

                // Check to see if this is a re-run of a failed run.
                if (!ic.IsEODReRun(this._configuration))
                {
                    eodOptionList.DefaultView.RowFilter = "";
                    // Reset the status flags and step numbers
                    eodRun.ResetConfiguration(this._configuration);
                    RunAdHocScripts(eodOptionList);
                }
                else
                    // This is a rerun so we are only interested in failed runs
                    eodOptionList.DefaultView.RowFilter = CN.Status + " = 'F'";

				progress = "Beginning EOD configuration " + this._configuration + " - " +
					eodOptionList.Rows.Count.ToString() + " options to process.";
				Console.WriteLine(progress);

				// Execute the options in the requested order
				eodOptionList.DefaultView.Sort = CN.SortOrder;

                //IP - 23/04/08 - UAT(260) - I have commented this out as the 'Adhoc scripts' routine
                //was run, but then when the EodInterface.RunOption method was called, there
                //was no case statement to run adhoc scipts, therefore it would always return the 
                //eodResult as 'Fail' even though it had been successfull. Therefore I have created
                //a case statement and the routine will now run from EodInterface.RunOption.
                //if (AdHocScripts)
                //    ic.RunEODAdHocScripts(true);
            //done factexport use to distinguish whether running FACT export and import so determines whether 
            // system should wait for FACTAUTO.ODF file should be created before running import        
            bool donefactexport = false;
            
				foreach (DataRowView optionRow in eodOptionList.DefaultView)
				{
					string optionCode = (string)optionRow[CN.OptionCode];
                    bool rerun = false;
                    var filedate = "";
                    int runNo = eodRun.StartNextRun(this._configuration, optionCode, out rerun, out filedate);        //jec 08/04/11 RI
					string eodResult = EODResult.Fail;
					
					progress = "Starting EOD option: " + optionCode + " with Run No: " + runNo.ToString();
					Console.WriteLine(progress);
					
					try
					{
						// The SQL transaction is opened within the EOD process because
						// some of these processes may need to roll back an error for one
						// customer or account but then continue with the rest. This also reduces
						// the size of each transaction.

						// Run the option within this EOD configuration
						EODInterface eodOption = new EODInterface(user, countryCode, runNo, optionCode);
						// The EOD result (P=Pass, F=Fail, W=Warning) must be returned
                        
						eodResult = eodOption.RunOption(this.configuration,ref donefactexport, rerun, filedate);

					}
					catch(Exception ex)
					{
						eodResult = EODResult.Fail;
						// Interface error table

                        if (ex is SqlException)
                        {
                            BInterfaceError ie = new BInterfaceError(
                                null,
                                null,
                                optionCode,
                                runNo,
                                DateTime.Now,
                                Environment.NewLine + Environment.NewLine +
                                "Error:" + ((SqlException)ex).Message + Environment.NewLine +
                                "Error number:" + ((SqlException)ex).Number + Environment.NewLine +
                                "Stored Procedure:" + ((SqlException)ex).Procedure + Environment.NewLine +
                                "Line:" + ((SqlException)ex).LineNumber + Environment.NewLine +
                                "Server:" + ((SqlException)ex).Server + Environment.NewLine +
                                "#System Message#" + Environment.NewLine +
                                "StackTrace:" + Environment.NewLine + ex.StackTrace + Environment.NewLine,
                                "E");
                        }
                        else
                        {
                            BInterfaceError ie = new BInterfaceError(
                                null,
                                null,
                                optionCode,
                                runNo,
                                DateTime.Now,
                                ex.Message + Environment.NewLine +
                                "#System Message#" + Environment.NewLine +
                                ex.StackTrace,
                                "E");
                        }

						_log.logException(ex, user.ToString(), "EOD Option: " + optionCode);
						Console.WriteLine(ex.Message);
					}
					finally
					{
						eodRun.SetRunComplete(this._configuration, optionCode, runNo, eodResult);
					}
				}

                if (AdHocScripts)
                {
                    ic.RunEODAdHocScripts(false);
                    ic.RenameEODAdHocScripts();
                }

				progress = "EOD finished.";
				Console.WriteLine(progress);
			}
			catch(Exception ex)
			{
				_log.logException(ex, user.ToString(), "EOD Configuration");
				Console.WriteLine(ex.Message);
			}
		}

        private void RunAdHocScripts(DataTable eodOptionList)
        {
            foreach (DataRowView optionRow in eodOptionList.DefaultView)
            {
                if ((string)optionRow[CN.OptionCode] == "ADHOC")
                {
                    AdHocScripts = true;
                    break;
                }
            }
        }
	}
}
