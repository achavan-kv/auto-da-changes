using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using STL.Common.Static;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using STL.Common;

namespace STL.PL
{
    public partial class AccountStatus : CommonForm
    {
        private DataView AcctDeliveryStatus = new DataView();
        public DateTime serverTime;
        public string numOfRows;

        public AccountStatus(Form root, Form parent)
        {
            InitializeComponent();

            FormRoot = root;
            FormParent = parent;
            
            //IP - 06/06/08 - Default the 'Datefrom' to a month ago
            serverTime = StaticDataManager.GetServerDateTime();
            //IP - prevent the user from selecting a date greater than 
            //(3) months from todays date as this would increase the 
            //amount of data that would be returned.
            dtFrom.MinDate = serverTime.AddMonths(-3);
            dtFrom.Value = serverTime.AddMonths(-1);
            dtFrom.MaxDate = serverTime.AddMonths(0);
            dtTo.MinDate = serverTime.AddMonths(-3);
            dtTo.Value = serverTime.AddMonths(0);
            dtTo.MaxDate = serverTime.AddMonths(0);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {

            try
            {
                Function = "btnSearch_Click";

                //Clear the grid for a new search.
                dgAcctDelStatus.DataSource = null;

                ((MainForm)this.FormRoot).statusBar1.Text = "";

                //Set all the parameters
                DateTime dateFrom = Convert.ToDateTime(dtFrom.Text);
                DateTime dateTo = Convert.ToDateTime(dtTo.Text);
                bool cancelledAccounts = chkCancelled.Checked;
                int branchno = Convert.ToInt16(Config.BranchCode);

                //Return a datable that consists of accounts delivery status's for the selected branch.
                DataTable acctStatusDet = AccountManager.AccountStatusGet(dateFrom, dateTo, branchno, out Error);

                if(Error.Length > 0)
                {
                    ShowError(Error);
                }
                else
                {
                    AcctDeliveryStatus = new DataView(acctStatusDet);

                    if (AcctDeliveryStatus.Count > 0)
                    {
                        dgAcctDelStatus.DataSource = AcctDeliveryStatus;

                        dgAcctDelStatus.Columns[CN.AccountNumber2].Width = 100;
                        dgAcctDelStatus.Columns[CN.DateOpened1].Width = 100;
                        dgAcctDelStatus.Columns[CN.CustomerID].Width = 100;
                        dgAcctDelStatus.Columns[CN.Employee].Width = 60;
                        dgAcctDelStatus.Columns[CN.EmployeeName].Width = 100;
                        dgAcctDelStatus.Columns[CN.Status1].Width = 150;
                        dgAcctDelStatus.Columns[CN.Status2].Width = 150;
                        dgAcctDelStatus.Columns[CN.Status3].Width = 150;

                        if (chkCancelled.Checked == true)
                        {
                            AcctDeliveryStatus.RowFilter = "(" + CN.Status1 + " not = 'Account Cancelled'" + ")";
                        }
                        else if (chkCancelled.Checked == false)
                        {
                            AcctDeliveryStatus.RowFilter = "";
                        }
                        
                        numOfRows = ((DataView)dgAcctDelStatus.DataSource).Count + GetResource("M_ACCOUNTSLISTED");
                        ((MainForm)this.FormRoot).statusBar1.Text = numOfRows;
                        
                        
                    }

                   
                }

            }
            catch (Exception ex)
            {

                Catch(ex, Function);
            }    

        }

        private void chkCancelled_Click(object sender, EventArgs e)
        {
            //IP - 06/06/08
            //If the 'Exclude Cancelled Accounts' checkbox has been ticked
            //then filter the dataview so that these are not displayed.
            if (chkCancelled.Checked == true)
            {
                AcctDeliveryStatus.RowFilter = "("+ CN.Status1 + " not = 'Account Cancelled'" +")";
            }

            //If the 'Exclude Cancelled Accounts checkbox is un-ticked
            //then reset the rowfilter and display the cancelled accounts aswell.
            if (chkCancelled.Checked == false)
            {
                AcctDeliveryStatus.RowFilter = "";
            }

            if (dgAcctDelStatus.DataSource != null)
            {
                numOfRows = ((DataView)dgAcctDelStatus.DataSource).Count + GetResource("M_ACCOUNTSLISTED");
                ((MainForm)this.FormRoot).statusBar1.Text = numOfRows;
            }
        }

        private void dtFrom_MouseHover(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(dtFrom, "Please note you can only select a date within the last 3 months");
        }

        private void dtTo_MouseHover(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(dtTo, "Please note you can only select a date within the last 3 months");
        }

        private void chkCancelled_MouseHover(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(chkCancelled, "Tick to exclude cancelled accounts once accounts have been returned.");
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.CloseTab();
        }

        //private void dtFrom_ValueChanged(object sender, EventArgs e)
        //{
        //    errorProvider1.SetError(dtFrom, "");
        //    if (dtFrom.Value < serverTime.AddMonths(-3))
        //    {
        //        errorProvider1.SetError(dtFrom, "Please select a date greater than 3 months ago");

        //        dtFrom.Value = serverTime.AddMonths(-1);
        //    }
        //}
    }
}