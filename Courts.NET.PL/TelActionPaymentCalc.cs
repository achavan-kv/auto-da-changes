using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using STL.Common.Static;

namespace STL.PL
{
    public partial class TelActionPaymentCalc : CommonForm
    {
        private string accountNo=string.Empty;
        private decimal accountArrears = 0;
        private decimal accountBalance = 0;
        private decimal collectionFee = 0;

        public TelActionPaymentCalc()
        {
            InitializeComponent();
        }

        public TelActionPaymentCalc(string acctNo, decimal arrears, decimal balance)
        {
            InitializeComponent();
            this.accountNo=acctNo;
            this.accountArrears=arrears;
            this.accountBalance = balance;
            txtPayment.Text = Convert.ToString(arrears);
            CalculateFee(acctNo,  arrears,"Pay", false);
        }

        private void CalculateFee(string acctNo, decimal arrears,string txtChg, bool reverse)
        {
            var allocPerson = 0;
            decimal collectionFee = 0;
            decimal bailiffFee = 0;
            var debitAccount = 0;
            var segmentId = 0;
            decimal paymentAmount = 0;
            errorProvider1.SetError(txtPayment, "");
            errorProvider1.SetError(txtTotalDue, "");

            PaymentManager.CalculateCreditFee(acctNo, Config.CountryCode, "PAY", ref allocPerson, arrears, reverse, ref paymentAmount,
                                         out collectionFee, out bailiffFee, out debitAccount, out segmentId, out Error);

            txtFee.Text = Convert.ToString(collectionFee);
            if (txtChg=="Pay")
            {
                //txtPayment.Text = Convert.ToString(accountArrears);                
                txtTotalDue.Text = Convert.ToString(Convert.ToDecimal(txtPayment.Text) + collectionFee);
            }
            else
            {                
                txtPayment.Text = Convert.ToString(Convert.ToDecimal(txtTotalDue.Text) - Convert.ToDecimal(txtFee.Text));
                if (Convert.ToDecimal(txtPayment.Text) > accountBalance)
                {
                    errorProvider1.SetError(txtTotalDue, "Total Amount entered is greater than Outstanding Balance + Fee");
                }
            }           

        }


        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtPayment_Leave(object sender, EventArgs e)
        {
            var curPayAmount=0m;    
            // Check a numeric Payment Amount has been entered
            if (!this.ValidMoneyField(this.txtPayment, out curPayAmount)) return;

            if (Convert.ToDecimal(StripCurrency(txtPayment.Text))>accountBalance)
            {
                errorProvider1.SetError(txtPayment,"Amount entered is greater than Outstanding Balance");
            }
            else
            {
                CalculateFee(accountNo, Convert.ToDecimal(txtPayment.Text),"Pay",false);
            }
        }

        private void txtTotalDue_Leave(object sender, EventArgs e)
        {
            var curPayAmount = 0m;
            // Check a numeric Payment Amount has been entered
            if (!this.ValidMoneyField(this.txtTotalDue, out curPayAmount)) return;
            CalculateFee(accountNo, Convert.ToDecimal(txtTotalDue.Text), "Due", true);
        }

        private bool ValidMoneyField(TextBox moneyField, out decimal moneyValue)
        {
            // Check a blank or zero money value entered
            moneyValue = 0.0M;
            moneyField.Text = moneyField.Text.Trim();
            if (!IsStrictMoney(moneyField.Text))
            {
                ShowInfo("M_NUMERIC");
                // Trap the focus in this field
                moneyField.Focus();
                return false;
            }

            // Reformat
            moneyValue = MoneyStrToDecimal(moneyField.Text);
            //moneyField.Text = moneyValue.ToString(DecimalPlaces);

            return true;
        }  // End of ValidMoneyField
    }
}
