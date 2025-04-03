using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Blue.Cosacs.Shared;
using System.Text.RegularExpressions;

namespace STL.PL.CashLoan
{
    public partial class CashLoanRecordBankTransfer : CommonForm
    {
        string err = string.Empty;

        public CashLoanRecordBankTransfer()
        {
            InitializeComponent();

            dtBankTransferDate.Value = DateTime.Today;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            ResetFields();

            LoadCashLoanDisbursementDetails();   
        }

        public void LoadCashLoanDisbursementDetails()
        {
            var acctNo = txtAccountNo.Text.Replace("-", "");

            var disbursementDetails = AccountManager.GetCashLoanDisbursementDetails(acctNo, out err);

            if (err.Length > 0)
            {
                ShowError(err);
            }
            else
            {
                if (disbursementDetails != null)
                {
                    PopulateDisbursementDetails(disbursementDetails);
                }
            }

        }

        public void PopulateDisbursementDetails(STL.PL.WS2.CashLoanDisbursementDetailsView disbursementDetails)
        {
            txtCustId.Text = disbursementDetails.CustomerId;
            txtCustomerName.Text = disbursementDetails.CustomerName;
            txtLoanAmount.Text = disbursementDetails.LoanAmount.HasValue? disbursementDetails.LoanAmount.Value.ToString(DecimalPlaces) : string.Empty;
            txtDisbursementType.Text = disbursementDetails.DisbursementType;
            txtBank.Text = disbursementDetails.BankName;
            txtBankAccountType.Text = disbursementDetails.BankAccountType;
            txtBankBranch.Text = disbursementDetails.BankBranch;
            txtBankAcctNo.Text = disbursementDetails.BankAccountNumber;
            txtBankReferenceNo.Text = disbursementDetails.BankReferenceNo;
            txtBankAccountName.Text = disbursementDetails.BankAccountName;
            txtNotes.Text = disbursementDetails.Notes;
            txtBankTransferReferenceNo.Text = disbursementDetails.BankTransferNumber;
            dtBankTransferDate.Value = disbursementDetails.BankTransferDate.HasValue ? disbursementDetails.BankTransferDate.Value : DateTime.Today;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            CloseTab();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var acctNo = txtAccountNo.Text.Replace("-", "");
            var bankTransRefNo = txtBankTransferReferenceNo.Text;
            var transferDate = dtBankTransferDate.Value;

            if (txtBankTransferReferenceNo.Text.Length == 0)
            {
                ShowInfo("M_REQUIREBANKREFNO");
                this.txtBankTransferReferenceNo.Focus();
                return;
            }

            Regex regExp = null;
            Match regMatch = null;

            regExp = new Regex("^[a-zA-Z0-9]*$");
            regMatch = regExp.Match(txtBankTransferReferenceNo.Text);

            if (regMatch.Success)
            {
                errorProvider1.SetError(txtBankTransferReferenceNo, String.Empty);  
            }
            else
            {
                errorProvider1.SetError(txtBankTransferReferenceNo, GetResource("M_INVALIDBANKREFERENCENO")); 
                return;
            }

            err = AccountManager.SaveCashLoanDisbursementBankTransfer(acctNo, bankTransRefNo, transferDate);


            if (err.Length > 0)
            {
                ShowError(err);
            }
            else
            {
                ((MainForm)FormRoot).statusBar1.Text = "Changes have been saved";
            }
        }

        private void ResetFields()
        {
            ((MainForm)FormRoot).statusBar1.Text = string.Empty;

            txtCustId.Text = string.Empty;
            txtCustomerName.Text = string.Empty;
            txtLoanAmount.Text = string.Empty;
            txtDisbursementType.Text = string.Empty;
            txtBank.Text = string.Empty;
            txtBankAccountType.Text = string.Empty;
            txtBankBranch.Text = string.Empty;
            txtBankAcctNo.Text = string.Empty;
            txtNotes.Text = string.Empty;
            txtBankTransferReferenceNo.Text = string.Empty;
            txtBankReferenceNo.Text = string.Empty;
            txtBankAccountName.Text = string.Empty;
            
            dtBankTransferDate.Value = DateTime.Today;
        }
    }
}
