using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace STL.PL.Tallyman
{
    public partial class TallymanExtract : CommonForm
    {
        private Timer timerCheck;
        private int _nextCheck = 0;

        public TallymanExtract()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
            MonitorJobs();

            timerCheck = new Timer();
            timerCheck.Interval = 1000;
            timerCheck.Tick += new EventHandler(timerCheck_Tick);
            timerCheck.Start();
		}

        ///// <summary>
        ///// Clean up any resources being used.
        ///// </summary>
        //protected override void Dispose( bool disposing )
        //{
        //    if( disposing )
        //    {
        //        if(components != null)
        //        {
        //            components.Dispose();
        //        }
        //    }
        //    base.Dispose( disposing );
        //}

		

		private void btnRun_Click(object sender, System.EventArgs e)
		{
            if (chkNewAccountExtract.Checked || chkUpdateExistingAccounts.Checked)
            {
                btnRun.Enabled = false;
                //base.SystemConfig.Timeout = 400000;
                

                WS6.TallymanExtractRequest request = new WS6.TallymanExtractRequest();
                request.NewAccountsExtract = chkNewAccountExtract.Checked;
                request.UpdateExistingDetails = chkUpdateExistingAccounts.Checked;
                WS6.TallymanExtractResponse response = base.SystemConfig.TallymanExtract(request);
                //base.SystemConfig.BeginTallymanExtract(request, new AsyncCallback(  TallyExtractCallback), base.SystemConfig);

                MonitorJobs();
            }
		}

        void timerCheck_Tick(object sender, EventArgs e)
        {
            CheckForMonitorJobs();
        }

        private void CheckForMonitorJobs()
        {
            lblNextCheck.Text = _nextCheck.ToString() + " seconds";
            if (_nextCheck > 0)
            {
                _nextCheck--;
            }
            else
            {
                MonitorJobs();
            }

        }

        private bool MonitorJobStatus(string jobName, Label statusLabel, Label lastRunLabel)
        {
            statusLabel.Text = "Checking Status";
            lastRunLabel.Text = string.Empty;
            bool active = false;

            try
            {
                WS6.MonitorResponse response = base.SystemConfig.MonitorJobStatus(jobName);
                statusLabel.Text = GetStatus(response);
                if(response.LastRunOutcome != 5)
                    lastRunLabel.Text = response.DateLastRun.ToString();

                if (response.CurrentExecutionStatus == 1)
                    active = true;
            }
            catch (Exception)
            {
                statusLabel.Text = "Job not set up";
            }
            return active;
        }

        private void MonitorJobs()
        {
            Wait();
            try
            {
                bool anyActiveExtracts = false;

                lblStatusWeekly.Text = "Checking Status:";
                lblStatusDaily.Text = "Checking Status:";
                anyActiveExtracts = MonitorJobStatus("TallymanWeeklyExtract", lblStatusWeekly, lblLastRunWeekly);
                anyActiveExtracts = MonitorJobStatus("TallymanDailyUpdate", lblStatusDaily, lblLastRunDaily);
                btnRun.Enabled = !anyActiveExtracts;

                MonitorJobStatus("TallymanSegmentImport", lblStatusSegments, lblLastRunSegments);

                bool dhlJobActive = false;
                dhlJobActive = MonitorJobStatus("DHLDeliveryOrderFileExport", lblExportEastStatus, lblExportEastLastRun);
                //dhlJobActive = MonitorJobStatus("DHLWestDeliveryOrderFileExport", lblExportWestStatus, lblExportWestLastRun);
                //dhlJobActive = MonitorJobStatus("DHLDespatchNoteImport", lblImportDespatchNoteStatus, lblImportDespatchNoteRunNo);
                dhlJobActive = MonitorJobStatus("DHLCustomerDeliveriesImport", lblImportCustomerDeliveriesStatus, lblImportCustomerDeliveriesRunNo);
                //dhlJobActive = MonitorJobStatus("DHLEastCustomerReturnsFileExport", lblExportReturnsEastStatus, lblExportReturnsEastLastRun);
                //dhlJobActive = MonitorJobStatus("DHLWestCustomerReturnsFileExport", lblExportReturnsWestStatus, lblExportReturnsWestLastRun);
                btnRunDeliveries.Enabled = !dhlJobActive;


                //CR - 1037 -----------------------------------------------------------------
                bool homeClubJobActive = false;
                homeClubJobActive = MonitorJobStatus("HomeClubVoucher_EOD", lblHomeClubStatus, lblLastRunHomeClub);
                btnRunHomeClubEOD.Enabled = !homeClubJobActive;
                //---------------------------------------------------------------------------
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                _nextCheck = 30;
                StopWait();
            }
        }

        private string GetStatus(WS6.MonitorResponse response)
        {
            string status;
            if (response.CurrentExecutionStatus == 1)
                status = "Running";
            else
                switch (response.LastRunOutcome)
                { 
                    case 0:
                        status = "Completed Failed";
                        break;
                    case 1:
                        status = "Completed Success";
                        break;
                    case 3:
                        status = "Cancelled";
                        break;
                    case 5:
                        status = "Never Run";
                        break;
                    default:
                        status = "Unknown";
                        break;
                }
            return status;
        }
        
        private void btnRunImport_Click(object sender, EventArgs e)
        {
            btnRun.Enabled = false;
            WS6.TallymanImportResponse response = base.SystemConfig.TallymanSegmentImport();
            MonitorJobs();
        }

        private void btnRunDeliveries_Click(object sender, EventArgs e)
        {
            System.Collections.Generic.List<string> jobList = new System.Collections.Generic.List<string>();
            if (chkExportEast.Checked)
                jobList.Add("DHLDeliveryOrderFileExport");
            //if (chkExportWest.Checked)
            //    jobList.Add("DHLWestDeliveryOrderFileExport");
            //if (chkImportDespatchNote.Checked)
            //    jobList.Add("DHLDespatchNoteImport");
            if (chkImportCustomerDelivery.Checked)
                jobList.Add("DHLCustomerDeliveriesImport");
            //if (chkExportReturnsEast.Checked)
            //    jobList.Add("DHLEastCustomerReturnsFileExport");
            //if(chkExportReturnsWest.Checked)
            //    jobList.Add("DHLWestCustomerReturnsFileExport");

            base.SystemConfig.RunJobs(jobList.ToArray());

        }

        
        //CR - 1037 -----------------------------------------------------------------
        private void btnRunHomeClubEOD_Click(object sender, EventArgs e)
        {
            try
            {
                btnRunHomeClubEOD.Enabled = false;
                List<string> jobList = new System.Collections.Generic.List<string>();
                jobList.Add("HomeClubVoucher_EOD");
                base.SystemConfig.RunJobs(jobList.ToArray());
                MonitorJobs();
            }
            catch(Exception ex)
            {
                btnRunHomeClubEOD.Enabled = true;
                Catch(ex, "btnRunHomeClubEOD_Click");
            }
        }
        //---------------------------------------------------------------------------
        
	}
}

    

