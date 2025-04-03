using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Blue.Cosacs.Shared;
using Blue.Cosacs.Shared.Services;
using Blue.Cosacs.Shared.Services.EOD;
using System.Text.RegularExpressions;

namespace STL.PL
{
    public partial class WTRSetup : CommonForm
    {
        private WTRDates wtr;
        //private string error = string.Empty;
        private const string dateEndError = "Date End must be greater than Date Start";
        private const string filenameError = "A file name must be entered";
        private const string dateStartDayWarning = "Date Start day is not a Monday";
        private const string dateEndDayWarning = "Date End day is not a Sunday";
        private const string dateRangeError = "This date range is used on another report";
        private const string dateWithinDays = "Date must be within 7 days of Current Year date";
        private const string invalidFileName = "Invalid filename";
        private const string duplicateFilename = "Duplicate filename";

        private DateTime serverDateTime = DateTime.Today;
        private DateTime currentFinYearDate = DateTime.Today;
        private DateTime lastFinYearDate = DateTime.Today;
        private DateTime lastFinYearDateMax = DateTime.Today;

        public WTRSetup(Form root, Form parent)
        {
            InitializeComponent();

            FormRoot = root;
            FormParent = parent;

            setup();

            serverDateTime = StaticDataManager.GetServerDateTime();
            if (serverDateTime.Month > 4 || (serverDateTime.Month == 4 && serverDateTime.Day >= 3))           // #9379 use previous year for financial year when date before 3rd April
            {
                currentFinYearDate = new DateTime(serverDateTime.Year, 4, 1);       //Set the current financial year to April of the current year
            }
            else
            {
                currentFinYearDate = new DateTime(serverDateTime.Year-1, 4, 1);       //Set the current financial year to April of the previous year
            }
            //lastFinYearDate = new DateTime(serverDateTime.Year-1, 4, 1);
            lastFinYearDate = currentFinYearDate.AddYears(-1);
            lastFinYearDateMax = currentFinYearDate.AddDays(-1);

            //Set the Min and Max dates for all Date Start Current Year
            dtStartCY1.MinDate = currentFinYearDate;
            dtStartCY1.MaxDate = DateTime.Today;

            dtStartCY2.MinDate = currentFinYearDate;
            dtStartCY2.MaxDate = DateTime.Today;

            dtStartCY3.MinDate = currentFinYearDate;
            dtStartCY3.MaxDate = DateTime.Today;

            dtStartCY4.MinDate = currentFinYearDate;
            dtStartCY4.MaxDate = DateTime.Today;

            dtStartCY5.MinDate = currentFinYearDate;
            dtStartCY5.MaxDate = DateTime.Today;

            dtEndCY1.MinDate = currentFinYearDate;
            dtEndCY1.MaxDate = DateTime.Today;

            dtEndCY2.MinDate = currentFinYearDate;
            dtEndCY2.MaxDate = DateTime.Today;

            dtEndCY3.MinDate = currentFinYearDate;
            dtEndCY3.MaxDate = DateTime.Today;

            dtEndCY4.MinDate = currentFinYearDate;
            dtEndCY4.MaxDate = DateTime.Today;

            dtEndCY5.MinDate = currentFinYearDate;
            dtEndCY5.MaxDate = DateTime.Today;

            dtStartLY1.MinDate = lastFinYearDate;
            dtStartLY1.MaxDate = lastFinYearDateMax;

            dtStartLY2.MinDate = lastFinYearDate;
            dtStartLY2.MaxDate = lastFinYearDateMax;

            dtStartLY3.MinDate = lastFinYearDate;
            dtStartLY3.MaxDate = lastFinYearDateMax;

            dtStartLY4.MinDate = lastFinYearDate;
            dtStartLY4.MaxDate = lastFinYearDateMax;

            dtStartLY5.MinDate = lastFinYearDate;
            dtStartLY5.MaxDate = lastFinYearDateMax;

            dtEndLY1.MinDate = lastFinYearDate;
            dtEndLY1.MaxDate = lastFinYearDateMax;

            dtEndLY2.MinDate = lastFinYearDate;
            dtEndLY2.MaxDate = lastFinYearDateMax;

            dtEndLY3.MinDate = lastFinYearDate;
            dtEndLY3.MaxDate = lastFinYearDateMax;

            dtEndLY4.MinDate = lastFinYearDate;
            dtEndLY4.MaxDate = lastFinYearDateMax;

            dtEndLY5.MinDate = lastFinYearDate;
            dtEndLY5.MaxDate = lastFinYearDateMax;

            //currentFinYearDate = (serverDateTime).AddMonths(-(currentMonth - april)).Subtract(TimeSpan.FromDays(serverDateTime - 1));
        }


        
        private void setup()
        {
             //Return the Weekly Trading Report dates saved on the database
            Client.Call(new WTRDatesGetRequest(),
            response =>
            {
                System.Threading.Thread.Sleep(2000);            // jec 27/10/11 may not be required. Added to resovle intermittent Invoke error when screen loading.
                wtr = response.wtrDates;
                SetupDateFields();
            }, this);

          
          
           
        }

        private void SetupDateFields()
        {

            chkActivateDate1.Checked = Convert.ToBoolean(wtr.DtActive1);
            dtStartCY1.Enabled = Convert.ToBoolean(wtr.DtActive1);
            dtEndCY1.Enabled = Convert.ToBoolean(wtr.DtActive1);
            dtStartLY1.Enabled = Convert.ToBoolean(wtr.DtActive1);
            dtEndLY1.Enabled = Convert.ToBoolean(wtr.DtActive1);
            txtFileName1.Enabled = Convert.ToBoolean(wtr.DtActive1);

            if (Convert.ToDateTime(wtr.DtStartCY1) >= dtStartCY1.MinDate)
            {
                dtStartCY1.Value = Convert.ToDateTime(wtr.DtStartCY1);
                dtEndCY1.Value = Convert.ToDateTime(wtr.DtEndCY1);
            }
            else
            {
                dtStartCY1.Value = dtStartCY1.MinDate;
                dtEndCY1.Value = dtEndCY1.MinDate;
            }
            if (Convert.ToDateTime(wtr.DtStartLY1) >= dtStartLY1.MinDate)
            {
                dtStartLY1.Value = Convert.ToDateTime(wtr.DtStartLY1);
                dtEndLY1.Value = Convert.ToDateTime(wtr.DtEndLY1);
            }
            else
            {
                dtStartLY1.Value = dtStartLY1.MinDate;
                dtEndLY1.Value = dtEndLY1.MinDate;
            }
            
            txtFileName1.Text = Convert.ToString(wtr.DtFilename1);
           

            chkActivateDate2.Checked = Convert.ToBoolean(wtr.DtActive2);
            dtStartCY2.Enabled = Convert.ToBoolean(wtr.DtActive2);
            dtEndCY2.Enabled = Convert.ToBoolean(wtr.DtActive2);
            dtStartLY2.Enabled = Convert.ToBoolean(wtr.DtActive2);
            dtEndLY2.Enabled = Convert.ToBoolean(wtr.DtActive2);
            txtFileName2.Enabled = Convert.ToBoolean(wtr.DtActive2);

            if (Convert.ToDateTime(wtr.DtStartCY2) >= dtStartCY2.MinDate)
            {
                dtStartCY2.Value = Convert.ToDateTime(wtr.DtStartCY2);
                dtEndCY2.Value = Convert.ToDateTime(wtr.DtEndCY2);
            }
            else
            {
                dtStartCY2.Value = dtStartCY2.MinDate;
                dtEndCY2.Value = dtEndCY2.MinDate;
            }
            if (Convert.ToDateTime(wtr.DtStartLY2) >= dtStartLY2.MinDate)
            {
                dtStartLY2.Value = Convert.ToDateTime(wtr.DtStartLY2);
                dtEndLY2.Value = Convert.ToDateTime(wtr.DtEndLY2);
            }
            else
            {
                dtStartLY2.Value = dtStartLY2.MinDate;
                dtEndLY2.Value = dtEndLY2.MinDate;
            }
                        
            txtFileName2.Text = Convert.ToString(wtr.DtFilename2);
           

            chkActivateDate3.Checked = Convert.ToBoolean(wtr.DtActive3);
            dtStartCY3.Enabled = Convert.ToBoolean(wtr.DtActive3);
            dtEndCY3.Enabled = Convert.ToBoolean(wtr.DtActive3);
            dtStartLY3.Enabled = Convert.ToBoolean(wtr.DtActive3);
            dtEndLY3.Enabled = Convert.ToBoolean(wtr.DtActive3);
            txtFileName3.Enabled = Convert.ToBoolean(wtr.DtActive3);

            if (Convert.ToDateTime(wtr.DtStartCY3) >= dtStartCY3.MinDate)
            {
                dtStartCY3.Value = Convert.ToDateTime(wtr.DtStartCY3);
                dtEndCY3.Value = Convert.ToDateTime(wtr.DtEndCY3);
            }
            else
            {
                dtStartCY3.Value = dtStartCY3.MinDate;
                dtEndCY3.Value = dtEndCY3.MinDate;
            }
            if (Convert.ToDateTime(wtr.DtStartLY3) >= dtStartLY3.MinDate)
            {
                dtStartLY3.Value = Convert.ToDateTime(wtr.DtStartLY3);
                dtEndLY3.Value = Convert.ToDateTime(wtr.DtEndLY3);
            }
            else
            {
                dtStartLY3.Value = dtStartLY3.MinDate;
                dtEndLY3.Value = dtEndLY3.MinDate;
            }

            txtFileName3.Text = Convert.ToString(wtr.DtFilename3);



            chkActivateDate4.Checked = Convert.ToBoolean(wtr.DtActive4);
            dtStartCY4.Enabled = Convert.ToBoolean(wtr.DtActive4);
            dtEndCY4.Enabled = Convert.ToBoolean(wtr.DtActive4);
            dtStartLY4.Enabled = Convert.ToBoolean(wtr.DtActive4);
            dtEndLY4.Enabled = Convert.ToBoolean(wtr.DtActive4);
            txtFileName4.Enabled = Convert.ToBoolean(wtr.DtActive4);

            if (Convert.ToDateTime(wtr.DtStartCY4) >= dtStartCY4.MinDate)
            {
                dtStartCY4.Value = Convert.ToDateTime(wtr.DtStartCY4);
                dtEndCY4.Value = Convert.ToDateTime(wtr.DtEndCY4);
            }
            else
            {
                dtStartCY4.Value = dtStartCY4.MinDate;
                dtEndCY4.Value = dtEndCY4.MinDate;
            }
            if (Convert.ToDateTime(wtr.DtStartLY4) >= dtStartLY4.MinDate)
            {
                dtStartLY4.Value = Convert.ToDateTime(wtr.DtStartLY4);
                dtEndLY4.Value = Convert.ToDateTime(wtr.DtEndLY4);
            }
            else
            {
                dtStartLY4.Value = dtStartLY4.MinDate;
                dtEndLY4.Value = dtEndLY4.MinDate;
            }

            txtFileName4.Text = Convert.ToString(wtr.DtFilename4);



            chkActivateDate5.Checked = Convert.ToBoolean(wtr.DtActive5);
            dtStartCY5.Enabled = Convert.ToBoolean(wtr.DtActive5);
            dtEndCY5.Enabled = Convert.ToBoolean(wtr.DtActive5);
            dtStartLY5.Enabled = Convert.ToBoolean(wtr.DtActive5);
            dtEndLY5.Enabled = Convert.ToBoolean(wtr.DtActive5);
            txtFileName5.Enabled = Convert.ToBoolean(wtr.DtActive5);

            if (Convert.ToDateTime(wtr.DtStartCY5) >= dtStartCY5.MinDate)
            {
                dtStartCY5.Value = Convert.ToDateTime(wtr.DtStartCY5);
                dtEndCY5.Value = Convert.ToDateTime(wtr.DtEndCY5);
            }
            else
            {
                dtStartCY5.Value = dtStartCY5.MinDate;
                dtEndCY5.Value = dtEndCY5.MinDate;
            }
            if (Convert.ToDateTime(wtr.DtStartLY5) >= dtStartLY5.MinDate)
            {
                dtStartLY5.Value = Convert.ToDateTime(wtr.DtStartLY5);
                dtEndLY5.Value = Convert.ToDateTime(wtr.DtEndLY5);
            }
            else
            {
                dtStartLY5.Value = dtStartLY5.MinDate;
                dtEndLY5.Value = dtEndLY5.MinDate;
            }

            txtFileName5.Text = Convert.ToString(wtr.DtFilename5);
            
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (Validate())
            {
                wtr.DtActive1 = Convert.ToBoolean(chkActivateDate1.Checked);
                wtr.DtStartCY1 = chkActivateDate1.Checked ? Convert.ToDateTime(dtStartCY1.Value) : DateTime.Today;
                wtr.DtEndCY1 = chkActivateDate1.Checked ? Convert.ToDateTime(dtEndCY1.Value) : DateTime.Today;
                wtr.DtStartLY1 = chkActivateDate1.Checked ? Convert.ToDateTime(dtStartLY1.Value) : DateTime.Today.AddYears(-1);
                wtr.DtEndLY1 = chkActivateDate1.Checked ? Convert.ToDateTime(dtEndLY1.Value) : DateTime.Today.AddYears(-1);
                wtr.DtFilename1 = chkActivateDate1.Checked ? txtFileName1.Text : string.Empty;

                wtr.DtActive2 = Convert.ToBoolean(chkActivateDate2.Checked);
                wtr.DtStartCY2 = chkActivateDate2.Checked ? Convert.ToDateTime(dtStartCY2.Value) : DateTime.Today;
                wtr.DtEndCY2 = chkActivateDate2.Checked ? Convert.ToDateTime(dtEndCY2.Value) : DateTime.Today;
                wtr.DtStartLY2 = chkActivateDate2.Checked ? Convert.ToDateTime(dtStartLY2.Value) : DateTime.Today.AddYears(-1);
                wtr.DtEndLY2 = chkActivateDate2.Checked ? Convert.ToDateTime(dtEndLY2.Value) : DateTime.Today.AddYears(-1);
                wtr.DtFilename2 = chkActivateDate2.Checked ? txtFileName2.Text : string.Empty;

                wtr.DtActive3 = Convert.ToBoolean(chkActivateDate3.Checked);
                wtr.DtStartCY3 = chkActivateDate3.Checked ? Convert.ToDateTime(dtStartCY3.Value) : DateTime.Today;
                wtr.DtEndCY3 = chkActivateDate3.Checked ? Convert.ToDateTime(dtEndCY3.Value) : DateTime.Today;
                wtr.DtStartLY3 = chkActivateDate3.Checked ? Convert.ToDateTime(dtStartLY3.Value) : DateTime.Today.AddYears(-1);
                wtr.DtEndLY3 = chkActivateDate3.Checked ? Convert.ToDateTime(dtEndLY3.Value) : DateTime.Today.AddYears(-1);
                wtr.DtFilename3 = chkActivateDate3.Checked ? txtFileName3.Text : string.Empty;

                wtr.DtActive4 = Convert.ToBoolean(chkActivateDate4.Checked);
                wtr.DtStartCY4 = chkActivateDate4.Checked ? Convert.ToDateTime(dtStartCY4.Value) : DateTime.Today;
                wtr.DtEndCY4 = chkActivateDate4.Checked ? Convert.ToDateTime(dtEndCY4.Value) : DateTime.Today;
                wtr.DtStartLY4 = chkActivateDate4.Checked ? Convert.ToDateTime(dtStartLY4.Value) : DateTime.Today.AddYears(-1);
                wtr.DtEndLY4 = chkActivateDate4.Checked ? Convert.ToDateTime(dtEndLY4.Value) : DateTime.Today.AddYears(-1);
                wtr.DtFilename4 = chkActivateDate4.Checked ? txtFileName4.Text : string.Empty;

                wtr.DtActive5 = Convert.ToBoolean(chkActivateDate5.Checked);
                wtr.DtStartCY5 = chkActivateDate5.Checked ? Convert.ToDateTime(dtStartCY5.Value) : DateTime.Today;
                wtr.DtEndCY5 = chkActivateDate5.Checked ? Convert.ToDateTime(dtEndCY5.Value) : DateTime.Today;
                wtr.DtStartLY5 = chkActivateDate5.Checked ? Convert.ToDateTime(dtStartLY5.Value) : DateTime.Today.AddYears(-1);
                wtr.DtEndLY5 = chkActivateDate5.Checked ? Convert.ToDateTime(dtEndLY5.Value) : DateTime.Today.AddYears(-1);
                wtr.DtFilename5 = chkActivateDate5.Checked ? txtFileName5.Text : string.Empty;

                Client.Call(new WTRDatesSaveRequest() { WTRDates = wtr },
               response =>
               {

               }, this);
            }
        }

        private void chkActivateDate1_CheckedChanged(object sender, EventArgs e)
        {
            if (chkActivateDate1.Checked)
            {
                dtStartCY1.Enabled = true;
                dtEndCY1.Enabled = true;
                dtStartLY1.Enabled = true;
                dtEndLY1.Enabled = true;
                txtFileName1.Enabled = true;
            }
            else
            {
                dtStartCY1.Enabled = false;
                dtEndCY1.Enabled = false;
                dtStartLY1.Enabled = false;
                dtEndLY1.Enabled = false;
                txtFileName1.Enabled = false;
            }
        }

        private void chkActivateDate2_CheckedChanged(object sender, EventArgs e)
        {
            if (chkActivateDate2.Checked)
            {
                dtStartCY2.Enabled = true;
                dtEndCY2.Enabled = true;
                dtStartLY2.Enabled = true;
                dtEndLY2.Enabled = true;
                txtFileName2.Enabled = true;
            }
            else
            {
                dtStartCY2.Enabled = false;
                dtEndCY2.Enabled = false;
                dtStartLY2.Enabled = false;
                dtEndLY2.Enabled = false;
                txtFileName2.Enabled = false;
            }
        }

        private void chkActivateDate3_CheckedChanged(object sender, EventArgs e)
        {
            if (chkActivateDate3.Checked)
            {
                dtStartCY3.Enabled = true;
                dtEndCY3.Enabled = true;
                dtStartLY3.Enabled = true;
                dtEndLY3.Enabled = true;
                txtFileName3.Enabled = true;
            }
            else
            {
                dtStartCY3.Enabled = false;
                dtEndCY3.Enabled = false;
                dtStartLY3.Enabled = false;
                dtEndLY3.Enabled = false;
                txtFileName3.Enabled = false;
            }
        }

        private void chkActivateDate4_CheckedChanged(object sender, EventArgs e)
        {
            if (chkActivateDate4.Checked)
            {
                dtStartCY4.Enabled = true;
                dtEndCY4.Enabled = true;
                dtStartLY4.Enabled = true;
                dtEndLY4.Enabled = true;
                txtFileName4.Enabled = true;
            }
            else
            {
                dtStartCY4.Enabled = false;
                dtEndCY4.Enabled = false;
                dtStartLY4.Enabled = false;
                dtEndLY4.Enabled = false;
                txtFileName4.Enabled = false;
            }
        }

        private void chkActivateDate5_CheckedChanged(object sender, EventArgs e)
        {
            if (chkActivateDate5.Checked)
            {
                dtStartCY5.Enabled = true;
                dtEndCY5.Enabled = true;
                dtStartLY5.Enabled = true;
                dtEndLY5.Enabled = true;
                txtFileName5.Enabled = true;
            }
            else
            {
                dtStartCY5.Enabled = false;
                dtEndCY5.Enabled = false;
                dtStartLY5.Enabled = false;
                dtEndLY5.Enabled = false;
                txtFileName5.Enabled = false;
            }
        }

        private bool Validate()
        {
            var status = true;
            Regex regExp = null;
            Match regMatch = null;

            regExp = new Regex("[A-Z]|[a-z]|[0-9]");

            ClearWarnings();

            //Check that Date End is greater than Date Start
            if (dtEndCY1.Value < dtStartCY1.Value)
            {
                status = false;
                errorProvider1.SetError(dtEndCY1, dateEndError);
            }
            else
            {
                errorProvider1.SetError(dtEndCY1, "");
            }

            if (dtEndCY2.Value < dtStartCY2.Value)
            {
                status = false;
                errorProvider1.SetError(dtEndCY2, dateEndError);
            }
            else
            {
                errorProvider1.SetError(dtEndCY2, "");
            }

            if (dtEndCY3.Value < dtStartCY3.Value)
            {
                status = false;
                errorProvider1.SetError(dtEndCY3, dateEndError);
            }
            else
            {
                errorProvider1.SetError(dtEndCY3, "");
            }

            if (dtEndCY4.Value < dtStartCY4.Value)
            {
                status = false;
                errorProvider1.SetError(dtEndCY4, dateEndError);
            }
            else
            {
                errorProvider1.SetError(dtEndCY4, "");
            }

            if (dtEndCY5.Value < dtStartCY5.Value)
            {
                status = false;
                errorProvider1.SetError(dtEndCY5, dateEndError);
            }
            else
            {
                errorProvider1.SetError(dtEndCY5, "");
            }

            if (dtEndLY1.Value < dtStartLY1.Value)
            {
                status = false;
                errorProvider1.SetError(dtEndLY1, dateEndError);
            }
            else
            {
                errorProvider1.SetError(dtEndLY1, "");
            }

            if (dtEndLY2.Value < dtStartLY2.Value)
            {
                status = false;
                errorProvider1.SetError(dtEndLY2, dateEndError);
            }
            else
            {
                errorProvider1.SetError(dtEndLY2, "");
            }

            if (dtEndLY3.Value < dtStartLY3.Value)
            {
                status = false;
                errorProvider1.SetError(dtEndLY3, dateEndError);
            }
            else
            {
                errorProvider1.SetError(dtEndLY3, "");
            }

            if (dtEndLY4.Value < dtStartLY4.Value)
            {
                status = false;
                errorProvider1.SetError(dtEndLY4, dateEndError);
            }
            else
            {
                errorProvider1.SetError(dtEndLY4, "");
            }

            if (dtEndLY5.Value < dtStartLY5.Value)
            {
                status = false;
                errorProvider1.SetError(dtEndLY5, dateEndError);
            }
            else
            {
                errorProvider1.SetError(dtEndLY5, "");
            }

            //Check that no two date ranges are the same for different reports
            if ((dtStartCY1.Value == dtStartCY2.Value && dtEndCY1.Value == dtEndCY2.Value && chkActivateDate2.Checked
                || dtStartCY1.Value == dtStartCY3.Value && dtEndCY1.Value == dtEndCY3.Value && chkActivateDate3.Checked
                || dtStartCY1.Value == dtStartCY4.Value && dtEndCY1.Value == dtEndCY4.Value && chkActivateDate4.Checked
                || dtStartCY1.Value == dtStartCY5.Value && dtEndCY1.Value == dtEndCY5.Value && chkActivateDate5.Checked) && chkActivateDate1.Checked)
            {
                status = false;
                errorProvider1.SetError(dtStartCY1, dateRangeError);

            }
            else
            {
                errorProvider1.SetError(dtStartCY1, "");
            }

            if ((dtStartCY2.Value == dtStartCY1.Value && dtEndCY2.Value == dtEndCY1.Value && chkActivateDate1.Checked
               || dtStartCY2.Value == dtStartCY3.Value && dtEndCY2.Value == dtEndCY3.Value && chkActivateDate3.Checked
               || dtStartCY2.Value == dtStartCY4.Value && dtEndCY2.Value == dtEndCY4.Value && chkActivateDate4.Checked
               || dtStartCY2.Value == dtStartCY5.Value && dtEndCY2.Value == dtEndCY5.Value && chkActivateDate5.Checked) && chkActivateDate2.Checked)
            {
                status = false;
                errorProvider1.SetError(dtStartCY2, dateRangeError);

            }
            else
            {
                errorProvider1.SetError(dtStartCY2, "");
            }

            if ((dtStartCY3.Value == dtStartCY1.Value && dtEndCY3.Value == dtEndCY1.Value && chkActivateDate1.Checked
             || dtStartCY3.Value == dtStartCY2.Value && dtEndCY3.Value == dtEndCY2.Value && chkActivateDate2.Checked
             || dtStartCY3.Value == dtStartCY4.Value && dtEndCY3.Value == dtEndCY4.Value && chkActivateDate4.Checked
             || dtStartCY3.Value == dtStartCY5.Value && dtEndCY3.Value == dtEndCY5.Value && chkActivateDate5.Checked) && chkActivateDate3.Checked)
            {
                status = false;
                errorProvider1.SetError(dtStartCY3, dateRangeError);

            }
            else
            {
                errorProvider1.SetError(dtStartCY3, "");
            }

            if ((dtStartCY4.Value == dtStartCY1.Value && dtEndCY4.Value == dtEndCY1.Value && chkActivateDate1.Checked
             || dtStartCY4.Value == dtStartCY2.Value && dtEndCY4.Value == dtEndCY2.Value && chkActivateDate2.Checked
             || dtStartCY4.Value == dtStartCY3.Value && dtEndCY4.Value == dtEndCY3.Value && chkActivateDate3.Checked
             || dtStartCY4.Value == dtStartCY5.Value && dtEndCY4.Value == dtEndCY5.Value && chkActivateDate5.Checked) && chkActivateDate4.Checked)
            {
                status = false;
                errorProvider1.SetError(dtStartCY4, dateRangeError);

            }
            else
            {
                errorProvider1.SetError(dtStartCY4, "");
            }

            if ((dtStartCY5.Value == dtStartCY1.Value && dtEndCY5.Value == dtEndCY1.Value && chkActivateDate1.Checked
            || dtStartCY5.Value == dtStartCY2.Value && dtEndCY5.Value == dtEndCY2.Value && chkActivateDate2.Checked
            || dtStartCY5.Value == dtStartCY3.Value && dtEndCY5.Value == dtEndCY3.Value && chkActivateDate3.Checked
            || dtStartCY5.Value == dtStartCY4.Value && dtEndCY5.Value == dtEndCY4.Value && chkActivateDate4.Checked) && chkActivateDate5.Checked)
            {
                status = false;
                errorProvider1.SetError(dtStartCY5, dateRangeError);

            }
            else
            {
                errorProvider1.SetError(dtStartCY5, "");
            }

            //Check Date Start Last Year is within 7 days of Date Start Current Year
            if (dtStartLY1.Value.AddYears(1) > dtStartCY1.Value.AddDays(7)
                || dtStartLY1.Value.AddYears(1) < dtStartCY1.Value.AddDays(-7) && chkActivateDate1.Checked)
            {
                status = false;
                errorProvider1.SetError(dtStartLY1, dateWithinDays);
            }
            else
            {
                errorProvider1.SetError(dtStartLY1, "");
            }

            if (dtStartLY2.Value.AddYears(1) > dtStartCY2.Value.AddDays(7)
             || dtStartLY2.Value.AddYears(1) < dtStartCY2.Value.AddDays(-7) && chkActivateDate2.Checked)
            {
                status = false;
                errorProvider1.SetError(dtStartLY2, dateWithinDays);
            }
            else
            {
                errorProvider1.SetError(dtStartLY2, "");
            }

            if (dtStartLY3.Value.AddYears(1) > dtStartCY3.Value.AddDays(7)
             || dtStartLY3.Value.AddYears(1) < dtStartCY3.Value.AddDays(-7) && chkActivateDate3.Checked)
            {
                status = false;
                errorProvider1.SetError(dtStartLY3, dateWithinDays);
            }
            else
            {
                errorProvider1.SetError(dtStartLY3, "");
            }

            if (dtStartLY4.Value.AddYears(1) > dtStartCY4.Value.AddDays(7)
            || dtStartLY4.Value.AddYears(1) < dtStartCY4.Value.AddDays(-7) && chkActivateDate4.Checked)
            {
                status = false;
                errorProvider1.SetError(dtStartLY4, dateWithinDays);
            }
            else
            {
                errorProvider1.SetError(dtStartLY4, "");
            }

            if (dtStartLY5.Value.AddYears(1) > dtStartCY5.Value.AddDays(7)
            || dtStartLY5.Value.AddYears(1) < dtStartCY5.Value.AddDays(-7) && chkActivateDate5.Checked)
            {
                status = false;
                errorProvider1.SetError(dtStartLY5, dateWithinDays);
            }
            else
            {
                errorProvider1.SetError(dtStartLY5, "");
            }

            if (dtEndLY1.Value.AddYears(1) > dtEndCY1.Value.AddDays(7)
               || dtEndLY1.Value.AddYears(1) < dtEndCY1.Value.AddDays(-7) && chkActivateDate1.Checked)
            {
                status = false;
                errorProvider1.SetError(dtEndLY1, dateWithinDays);
            }
            else
            {
                errorProvider1.SetError(dtEndLY1, "");
            }

            if (dtEndLY2.Value.AddYears(1) > dtEndCY2.Value.AddDays(7)
            || dtEndLY2.Value.AddYears(1) < dtEndCY2.Value.AddDays(-7) && chkActivateDate2.Checked)
            {
                status = false;
                errorProvider1.SetError(dtEndLY2, dateWithinDays);
            }
            else
            {
                errorProvider1.SetError(dtEndLY2, "");
            }

            if (dtEndLY3.Value.AddYears(1) > dtEndCY3.Value.AddDays(7)
            || dtEndLY3.Value.AddYears(1) < dtEndCY3.Value.AddDays(-7) && chkActivateDate3.Checked)
            {
                status = false;
                errorProvider1.SetError(dtEndLY3, dateWithinDays);
            }
            else
            {
                errorProvider1.SetError(dtEndLY3, "");
            }

            if (dtEndLY4.Value.AddYears(1) > dtEndCY4.Value.AddDays(7)
            || dtEndLY4.Value.AddYears(1) < dtEndCY4.Value.AddDays(-7) && chkActivateDate4.Checked)
            {
                status = false;
                errorProvider1.SetError(dtEndLY4, dateWithinDays);
            }
            else
            {
                errorProvider1.SetError(dtEndLY4, "");
            }

            if (dtEndLY5.Value.AddYears(1) > dtEndCY5.Value.AddDays(7)
            || dtEndLY5.Value.AddYears(1) < dtEndCY5.Value.AddDays(-7) && chkActivateDate5.Checked)
            {
                status = false;
                errorProvider1.SetError(dtEndLY5, dateWithinDays);
            }
            else
            {
                errorProvider1.SetError(dtEndLY5, "");
            }


            //Check that a filename has been entered for active dates
            if (txtFileName1.Text.Trim() == string.Empty && chkActivateDate1.Checked)
            {
                status = false;
                errorProvider1.SetError(txtFileName1, filenameError);
            }
            else
            {
                errorProvider1.SetError(txtFileName1, "");
            }

            regMatch = regExp.Match(txtFileName1.Text.Trim(), 0, txtFileName1.Text.Length);

            if (!regMatch.Success && chkActivateDate1.Checked)
            {
                status = false;
                errorProvider1.SetError(txtFileName1, invalidFileName);
            }
            else
            {
                errorProvider1.SetError(txtFileName1, "");
            }

            if (txtFileName2.Text.Trim() == string.Empty && chkActivateDate2.Checked)
            {
                status = false;
                errorProvider1.SetError(txtFileName2, filenameError);
            }
            else
            {
                errorProvider1.SetError(txtFileName2, "");
            }

            regMatch = regExp.Match(txtFileName2.Text.Trim(), 0, txtFileName2.Text.Length);

            if (!regMatch.Success && chkActivateDate2.Checked)
            {
                status = false;
                errorProvider1.SetError(txtFileName2, invalidFileName);
            }
            else
            {
                errorProvider1.SetError(txtFileName2, "");
            }

            if (txtFileName3.Text.Trim() == string.Empty && chkActivateDate3.Checked)
            {
                status = false;
                errorProvider1.SetError(txtFileName3, filenameError);
            }
            else
            {
                errorProvider1.SetError(txtFileName3, "");
            }

            regMatch = regExp.Match(txtFileName3.Text.Trim(), 0, txtFileName3.Text.Length);

            if (!regMatch.Success && chkActivateDate3.Checked)
            {
                status = false;
                errorProvider1.SetError(txtFileName3, invalidFileName);
            }
            else
            {
                errorProvider1.SetError(txtFileName3, "");
            }

            if (txtFileName4.Text.Trim() == string.Empty && chkActivateDate4.Checked)
            {
                status = false;
                errorProvider1.SetError(txtFileName4, filenameError);
            }
            else
            {
                errorProvider1.SetError(txtFileName4, "");
            }

            regMatch = regExp.Match(txtFileName4.Text.Trim(), 0, txtFileName4.Text.Length);

            if (!regMatch.Success && chkActivateDate4.Checked)
            {
                status = false;
                errorProvider1.SetError(txtFileName4, invalidFileName);
            }
            else
            {
                errorProvider1.SetError(txtFileName4, "");
            }

            if (txtFileName5.Text.Trim() == string.Empty && chkActivateDate5.Checked)
            {
                status = false;
                errorProvider1.SetError(txtFileName5, filenameError);
            }
            else
            {
                errorProvider1.SetError(txtFileName5, "");
            }

            regMatch = regExp.Match(txtFileName5.Text.Trim(), 0, txtFileName5.Text.Length);

            if (!regMatch.Success && chkActivateDate5.Checked)
            {
                status = false;
                errorProvider1.SetError(txtFileName5, invalidFileName);
            }
            else
            {
                errorProvider1.SetError(txtFileName5, "");
            }

            if ((txtFileName1.Text.Trim() == txtFileName2.Text.Trim() && chkActivateDate2.Checked
                || txtFileName1.Text.Trim() == txtFileName3.Text.Trim() && chkActivateDate3.Checked
                || txtFileName1.Text.Trim() == txtFileName4.Text.Trim() && chkActivateDate4.Checked
                || txtFileName1.Text.Trim() == txtFileName5.Text.Trim() && chkActivateDate5.Checked) && chkActivateDate1.Checked)
            {
                errorProvider1.SetError(txtFileName1, duplicateFilename);
            }
            else
            {
                errorProvider1.SetError(txtFileName1, "");
            }

            if (( txtFileName2.Text.Trim() == txtFileName3.Text.Trim() && chkActivateDate3.Checked
             || txtFileName2.Text.Trim() == txtFileName4.Text.Trim() && chkActivateDate4.Checked
             || txtFileName2.Text.Trim() == txtFileName5.Text.Trim() && chkActivateDate5.Checked) && chkActivateDate2.Checked)
            {
                errorProvider1.SetError(txtFileName2, duplicateFilename);
            }
            else
            {
                errorProvider1.SetError(txtFileName2, "");
            }

            if ((txtFileName3.Text.Trim() == txtFileName4.Text.Trim() && chkActivateDate4.Checked
             || txtFileName3.Text.Trim() == txtFileName5.Text.Trim() && chkActivateDate5.Checked) && chkActivateDate3.Checked)
            {
                errorProvider1.SetError(txtFileName2, duplicateFilename);
            }
            else
            {
                errorProvider1.SetError(txtFileName3, "");
            }


            if (( txtFileName4.Text.Trim() == txtFileName5.Text.Trim() && chkActivateDate5.Checked) && chkActivateDate4.Checked)
            {
                errorProvider1.SetError(txtFileName4, duplicateFilename);
            }
            else
            {
                errorProvider1.SetError(txtFileName4, "");
            }

            return status;
     
        }

        private void dtStartCY1_ValueChanged(object sender, EventArgs e)
        {

            if (dtStartCY1.Value.DayOfWeek != DayOfWeek.Monday && chkActivateDate1.Checked)
            {
                errorProviderForWarning.SetError(dtStartCY1, dateStartDayWarning);
            }
            else
            {
                errorProviderForWarning.SetError(dtStartCY1, "");
            }
        }

        private void dtEndCY1_ValueChanged(object sender, EventArgs e)
        {

            if (dtEndCY1.Value.DayOfWeek != DayOfWeek.Sunday && chkActivateDate1.Checked)
            {
                errorProviderForWarning.SetError(dtEndCY1, dateEndDayWarning);
            }
            else
            {
                errorProviderForWarning.SetError(dtEndCY1, "");
            }
        }

        private void dtStartCY2_ValueChanged(object sender, EventArgs e)
        {

            if (dtStartCY2.Value.DayOfWeek != DayOfWeek.Monday && chkActivateDate2.Checked)
            {
                errorProviderForWarning.SetError(dtStartCY2, dateStartDayWarning);
            }
            else
            {
                errorProviderForWarning.SetError(dtStartCY2, "");
            }
        }

        private void dtEndCY2_ValueChanged(object sender, EventArgs e)
        {

            if (dtEndCY2.Value.DayOfWeek != DayOfWeek.Sunday && chkActivateDate2.Checked)
            {
                errorProviderForWarning.SetError(dtEndCY2, dateEndDayWarning);
            }
            else
            {
                errorProviderForWarning.SetError(dtEndCY2, "");
            }
        }

        private void dtStartCY3_ValueChanged(object sender, EventArgs e)
        {
            if (dtStartCY3.Value.DayOfWeek != DayOfWeek.Monday && chkActivateDate3.Checked)
            {
                errorProviderForWarning.SetError(dtStartCY3, dateStartDayWarning);
            }
            else
            {
                errorProviderForWarning.SetError(dtStartCY3, "");
            }
        }

        private void dtEndCY3_ValueChanged(object sender, EventArgs e)
        {
            if (dtEndCY3.Value.DayOfWeek != DayOfWeek.Sunday && chkActivateDate3.Checked)
            {
                errorProviderForWarning.SetError(dtEndCY3, dateEndDayWarning);
            }
            else
            {
                errorProviderForWarning.SetError(dtEndCY3, "");
            }
        }

        private void dtStartCY4_ValueChanged(object sender, EventArgs e)
        {
            if (dtStartCY4.Value.DayOfWeek != DayOfWeek.Monday && chkActivateDate4.Checked)
            {
                errorProviderForWarning.SetError(dtStartCY4, dateStartDayWarning);
            }
            else
            {
                errorProviderForWarning.SetError(dtStartCY4, "");
            }
        }

        private void dtEndCY4_ValueChanged(object sender, EventArgs e)
        {
            if (dtEndCY4.Value.DayOfWeek != DayOfWeek.Sunday && chkActivateDate4.Checked)
            {
                errorProviderForWarning.SetError(dtEndCY4, dateEndDayWarning);
            }
            else
            {
                errorProviderForWarning.SetError(dtEndCY4, "");
            }
        }

        private void dtStartCY5_ValueChanged(object sender, EventArgs e)
        {
            if (dtStartCY5.Value.DayOfWeek != DayOfWeek.Monday && chkActivateDate5.Checked)
            {
                errorProviderForWarning.SetError(dtStartCY5, dateStartDayWarning);
            }
            else
            {
                errorProviderForWarning.SetError(dtStartCY5, "");
            }
        }

        private void dtEndCY5_ValueChanged(object sender, EventArgs e)
        {
            if (dtEndCY5.Value.DayOfWeek != DayOfWeek.Sunday && chkActivateDate5.Checked)
            {
                errorProviderForWarning.SetError(dtEndCY5, dateEndDayWarning);
            }
            else
            {
                errorProviderForWarning.SetError(dtEndCY5, "");
            }
        }

        private void dtStartLY1_ValueChanged(object sender, EventArgs e)
        {
            if (dtStartLY1.Value.DayOfWeek != DayOfWeek.Monday && chkActivateDate1.Checked)
            {
                errorProviderForWarning.SetError(dtStartLY1, dateStartDayWarning);
            }
            else
            {
                errorProviderForWarning.SetError(dtStartLY1, "");
            }
        }

        private void dtEndLY1_ValueChanged(object sender, EventArgs e)
        {
            if (dtEndLY1.Value.DayOfWeek != DayOfWeek.Sunday && chkActivateDate1.Checked)
            {
                errorProviderForWarning.SetError(dtEndLY1, dateEndDayWarning);
            }
            else
            {
                errorProviderForWarning.SetError(dtEndLY1, "");
            }
        }

        private void dtStartLY2_ValueChanged(object sender, EventArgs e)
        {
            if (dtStartLY2.Value.DayOfWeek != DayOfWeek.Monday && chkActivateDate2.Checked)
            {
                errorProviderForWarning.SetError(dtStartLY2, dateStartDayWarning);
            }
            else
            {
                errorProviderForWarning.SetError(dtStartLY2, "");
            }
        }

        private void dtEndLY2_ValueChanged(object sender, EventArgs e)
        {

            if (dtEndLY2.Value.DayOfWeek != DayOfWeek.Sunday && chkActivateDate2.Checked)
            {
                errorProviderForWarning.SetError(dtEndLY2, dateEndDayWarning);
            }
            else
            {
                errorProviderForWarning.SetError(dtEndLY2, "");
            }
        }

        private void dtStartLY3_ValueChanged(object sender, EventArgs e)
        {
            if (dtStartLY3.Value.DayOfWeek != DayOfWeek.Monday && chkActivateDate3.Checked)
            {
                errorProviderForWarning.SetError(dtStartLY3, dateStartDayWarning);
            }
            else
            {
                errorProviderForWarning.SetError(dtStartLY3, "");
            }
        }

        private void dtEndLY3_ValueChanged(object sender, EventArgs e)
        {
            if (dtEndLY3.Value.DayOfWeek != DayOfWeek.Sunday && chkActivateDate3.Checked)
            {
                errorProviderForWarning.SetError(dtEndLY3, dateEndDayWarning);
            }
            else
            {
                errorProviderForWarning.SetError(dtEndLY3, "");
            }
        }

        private void dtStartLY4_ValueChanged(object sender, EventArgs e)
        {
            if (dtStartLY4.Value.DayOfWeek != DayOfWeek.Monday && chkActivateDate4.Checked)
            {
                errorProviderForWarning.SetError(dtStartLY4, dateStartDayWarning);
            }
            else
            {
                errorProviderForWarning.SetError(dtStartLY4, "");
            }
        }

        private void dtEndLY4_ValueChanged(object sender, EventArgs e)
        {
            if (dtEndLY4.Value.DayOfWeek != DayOfWeek.Sunday && chkActivateDate4.Checked)
            {
                errorProviderForWarning.SetError(dtEndLY4, dateEndDayWarning);
            }
            else
            {
                errorProviderForWarning.SetError(dtEndLY4, "");
            }
        }

        private void dtStartLY5_ValueChanged(object sender, EventArgs e)
        {
            if (dtStartLY5.Value.DayOfWeek != DayOfWeek.Monday && chkActivateDate5.Checked)
            {
                errorProviderForWarning.SetError(dtStartLY5, dateStartDayWarning);
            }
            else
            {
                errorProviderForWarning.SetError(dtStartLY5, "");
            }
        }

        private void dtEndLY5_ValueChanged(object sender, EventArgs e)
        {
            if (dtEndLY5.Value.DayOfWeek != DayOfWeek.Sunday && chkActivateDate5.Checked)
            {
                errorProviderForWarning.SetError(dtEndLY5, dateEndDayWarning);
            }
            else
            {
                errorProviderForWarning.SetError(dtEndLY5, "");
            }
        }

        private void ClearWarnings()
        {
            errorProviderForWarning.SetError(dtStartCY1, "");
            errorProviderForWarning.SetError(dtStartCY2, "");
            errorProviderForWarning.SetError(dtStartCY3, "");
            errorProviderForWarning.SetError(dtStartCY4, "");
            errorProviderForWarning.SetError(dtStartCY5, "");

            errorProviderForWarning.SetError(dtEndCY1, "");
            errorProviderForWarning.SetError(dtEndCY2, "");
            errorProviderForWarning.SetError(dtEndCY3, "");
            errorProviderForWarning.SetError(dtEndCY4, "");
            errorProviderForWarning.SetError(dtEndCY5, "");

            errorProviderForWarning.SetError(dtStartLY1, "");
            errorProviderForWarning.SetError(dtStartLY2, "");
            errorProviderForWarning.SetError(dtStartLY3, "");
            errorProviderForWarning.SetError(dtStartLY4, "");
            errorProviderForWarning.SetError(dtStartLY5, "");

            errorProviderForWarning.SetError(dtEndLY1, "");
            errorProviderForWarning.SetError(dtEndLY2, "");
            errorProviderForWarning.SetError(dtEndLY3, "");
            errorProviderForWarning.SetError(dtEndLY4, "");
            errorProviderForWarning.SetError(dtEndLY5, "");

        }

        private void txtFileName1_Leave(object sender, EventArgs e)
        {
           txtFileName1.Text =  txtFileName1.Text.Replace(" ","");   
        }

        private void txtFileName2_Leave(object sender, EventArgs e)
        {
            txtFileName2.Text = txtFileName2.Text.Replace(" ", "");  
        }

        private void txtFileName3_Leave(object sender, EventArgs e)
        {
            txtFileName3.Text = txtFileName3.Text.Replace(" ", "");  
        }

        private void txtFileName4_Leave(object sender, EventArgs e)
        {
            txtFileName4.Text = txtFileName4.Text.Replace(" ", "");  
        }

        private void txtFileName5_Leave(object sender, EventArgs e)
        {
            txtFileName5.Text = txtFileName5.Text.Replace(" ", "");  
        }

       

   
    }
}
