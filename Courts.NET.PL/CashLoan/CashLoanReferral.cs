using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using STL.Common.Constants.CashLoans;

namespace STL.PL.CashLoan
{
    //#8670 Referral messages
    public partial class CashLoanReferral : CommonForm
    {
        public bool Return = false;
        public string ReferralNote = string.Empty;
        public string ReasonForReferral = string.Empty;
        public CashLoanReferral(string ReferText, string BlockStatus, out bool Return)
        {
            InitializeComponent();
            txtReferralMessage.Text = ReferText;
            Return = false;

            if (BlockStatus == CashLoanBlockedStatus.Blocked)
            {
                btnRefer.Visible = false;
                lblRefer.Text = "Contact Credit Department to Unblock";
            }
            if (BlockStatus == CashLoanStatus.LowAvailableSpend)        // #8780
            {
                btnRefer.Visible = false;
                lblRefer.Text = "Contact Credit Department";
            }
        }

        public void SetCtrlForReferral()
        {
            if (ReasonForReferral == CashLoanBlockedStatus.NotQualified)
            {
                lblRefer.ForeColor = Color.Red;
                lblRefer.Text = ReferralNote;
            }
        }


        private void btnRefer_Click(object sender, EventArgs e)
        {
            Return = true;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Return = false;
            Close();
        }
        
    }
}
