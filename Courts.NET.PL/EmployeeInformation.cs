using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using STL.Common.Constants.ColumnNames;




namespace STL.PL
{
    public partial class EmployeeInformation : CommonForm
    {
        private string error = string.Empty;
        private DataTable acctDaHistory = null;

        public EmployeeInformation(string acctno)
        {
            InitializeComponent();

            LoadDAHistory(acctno);

            tpEmployeeSummary.Selected = true;
           
        }

        /// <summary>
        /// Method which is called from AccountDetails.cs to populate the controls on the 
        /// Employee Summary tab.
        /// </summary>
        /// <param name="row"></param>
        public void ProcessEmployeeSummary(DataRow row)
        {
            if (row[CN.CreatedByName] != DBNull.Value)
            {
                txtCreatedByName.Text = Convert.ToString(row[CN.CreatedByName]);
            }

            if (row[CN.CreatedBy] != DBNull.Value)
            {
                txtCreatedByNo.Text = Convert.ToString(row[CN.CreatedBy]);
            }
          
            if (DBNull.Value != row["Account Opened"])
            {
                txtCreatedDate.Text = ((DateTime)row["Account Opened"]).ToShortDateString();
            }

            if (DBNull.Value != row["Sold By Employee Name"])
            {
                txtSoldBYEmpeeName.Text = (string)row["Sold By Employee Name"];
            }

            if (DBNull.Value != row["Sold By"])
            {
                txtSoldByEmpeeNo.Text = (row["Sold By"]).ToString();
            }

            if (DBNull.Value != row["Date Sold On"])
            {
                txtSoldOn.Text = ((DateTime)row["Date Sold On"]).ToShortDateString();
            }

            if (DBNull.Value != row["DAed By Employee Name"])
            {
                txtDAbyEmpeeName.Text = (string)row["DAed By Employee Name"];
            }

            if (DBNull.Value != row["DAed By EmpeeNo"])
            {
                txtDAEmpeeNo.Text = (row["DAed By EmpeeNo"]).ToString();
            }

            if (DBNull.Value != row["Date DAed On"])
            {
                txtDAedOn.Text = ((DateTime)row["Date DAed On"]).ToShortDateString();
            }

            if (DBNull.Value != row["LstChanged By Employee Name"])
            {
                txtLstChgEmpeeName.Text = (string)row["LstChanged By Employee Name"];
            }

            if (DBNull.Value != row["Last Changed By"])
            {
                txtLstChgEmpeeNo.Text = (row["Last Changed By"]).ToString();
            }

            if (DBNull.Value != row["Date Last Changed"])
            {
                txtLastChangedOn.Text = ((DateTime)row["Date Last Changed"]).ToShortDateString();
            }

            int empNoRev = Convert.ToInt32(row["Reopened By"]);
            if (empNoRev > 0)
            {
                this.txtReopenOn.BackColor = SystemColors.Window;
                this.txtReopenEmpeeNo.BackColor = SystemColors.Window;
                this.txtReopenEmpeeName.BackColor = SystemColors.Window;

                txtReopenEmpeeNo.Text = empNoRev.ToString();

                if (DBNull.Value != row["Date Reopened"])
                    txtReopenOn.Text = ((DateTime)row["Date Reopened"]).ToShortDateString();
                if (row["Reopened By Employee Name"] != DBNull.Value)
                    txtReopenEmpeeName.Text = Convert.ToString(row["Reopened By Employee Name"]);
            }
            else
            {
                this.txtReopenOn.BackColor = SystemColors.Control;
                this.txtReopenEmpeeNo.BackColor = SystemColors.Control;
                this.txtReopenEmpeeName.BackColor = SystemColors.Control;

                txtReopenEmpeeNo.Text = "";
                txtReopenOn.Text = "";
                txtReopenEmpeeName.Text = "";
            }

        }

        /// <summary>
        /// Method to retrieve and display Delivery Authorisation history for an account.
        /// If there is no history to display, the Delivery Authorisation History tab will not be displayed.
        /// </summary>
        /// <param name="acctno"></param>
        public void LoadDAHistory(string acctno)
        {
           acctDaHistory = AccountManager.LoadDAHistory(acctno, out error);

            if (error.Length > 0)
            {
                ShowError(error);
            }
            else
            {
                if (acctDaHistory.Rows.Count > 0)
                { 
                    dgvDaHist.DataSource = acctDaHistory;
                    
                }
                //else
                //{
                //    tcEmployeeInfo.TabPages.Remove(tpDaHistory);
                //}
            }
        }
    }
}
