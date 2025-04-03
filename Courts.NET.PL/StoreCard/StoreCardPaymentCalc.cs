using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Blue.Cosacs.Shared;
using STL.PL.Utils;
using Blue.Cosacs.Shared.Services.StoreCard;
using Blue.Cosacs.Shared.Services;
using STL.Common.Static;
using  Blue.Cosacs.Shared.Extensions;
using STL.Common;
namespace STL.PL.StoreCard
{
    public partial class StoreCardPaymentCalc : CommonForm
    {
        private string decimalplace = "F2";
        //private bool calc = false;
        private string acctno;
        public Decimal MinimumPayment { get; set; }
        public Decimal monthpayments = 0;
        public Decimal storeCardMinPayment = 0;
        public StoreCardPaymentCalc()                       //#9919
        {
            InitializeComponent();
        }

        public StoreCardPaymentCalc(string amount, string interest,string acctno,string decimalplace,decimal? minimumPayment=0)
            : this()
         {
            txtBalance.Text = amount;
            txtInterest.Text = interest;
            nud_term.Text = "12";
            nud_term.Focus();
            this.acctno = acctno;
            this.decimalplace = decimalplace;
            var StoreCardPaymentPercent = Convert.ToDecimal(MainForm.Current.Country[CountryParameterNames.StoreCardPaymentPercent]);       //#9919
            storeCardMinPayment = Convert.ToDecimal(MainForm.Current.Country[CountryParameterNames.StoreCardMinPayment]);       //#9919
            monthpayments = Math.Round(Convert.ToDecimal(Convert.ToDecimal(amount) * StoreCardPaymentPercent / 100),2);                       //#9919

            if (minimumPayment > monthpayments)
            {
                txtMonthlyPay.Text = Convert.ToDecimal(minimumPayment).ToString(decimalplace).StripNonNumeric();
            }
            else
            {
                txtMonthlyPay.Text = monthpayments.ToString(decimalplace).StripNonNumeric();
            }
            
        }

        public void CalcPayments(decimal balance, decimal interest)
        {
            errorProvider2.SetError(txtMonthPayResult, "");
            var term = Convert.ToInt32(nud_term.Value);
            var monthlypay = StoreCardCalc.CalculatePayments(balance, interest, term);
            txtMonthPayResult.Text = monthlypay.ToString(decimalplace).StripNonNumeric();
            txtTotalTerms.Text = (monthlypay * Convert.ToInt32(nud_term.Value)).ToString(decimalplace).StripNonNumeric();
            
        }

        public void CalcMonths(decimal balance, decimal interest, decimal monthpayments)
        {
            errorProvider1.SetError(btnPrint, "");
            errorProvider2.SetError(txtMonthPayResult, "");
            var result = StoreCardCalc.CalculateMonths(balance, monthpayments, interest);
            if (result != null && result.Months >= 1 && result.Months <= 60)
            {
                txtTermsResult.Text = result.Months.ToString();
                txtTotalPayments.Text = result.TotalAmount.ToString(decimalplace).StripNonNumeric();
                errorProvider1.SetError(txtTermsResult, "");
            }
            else
                errorProvider1.SetError(txtTermsResult, "The monthly amount is too high or too low.");
        }


        private decimal? Parse(TextBox t)
        {
            var dec = 0m;
            if (decimal.TryParse(t.Text, out dec))
                errorProvider1.SetError(t, "");
            else
            {
                errorProvider1.SetError(t, "This not a valid value");
                return null;
            }
            return dec;
        }

        private decimal? ParseInt(TextBox t)
        {
            var dec = 0m;
            if (decimal.TryParse(t.Text, out dec))
                txtInterestCheck(dec);
            else
            {
                errorProvider1.SetError(t, "This not a valid value");
                return null;
            }
            return dec;
        }

        private void txtInterestCheck(decimal interest)
        {
            if (interest > 0 && interest <= 100)
                errorProvider1.SetError(txtInterest, "");
            else
                errorProvider1.SetError(txtInterest, "Interest value out of range. Value must be between 1 and 100.");
        }

        private void txtPayAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = CheckKey(e.KeyChar);
        }

        private void txtInterest_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = CheckKey(e.KeyChar);
        }

        //private void txtMonthlyPay_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    e.Handled = CheckKey(e.KeyChar);
        //}

        private bool CheckKey(char key)
        {
            return !(char.IsDigit(key) ||
                     key == (Char)Keys.Decimal ||
                     key == (Char)Keys.Back ||
                     key == (Char)Keys.Delete);
        }

    

        private void nud_term_Leave(object sender, EventArgs e)
        {
            nud_term.Text = nud_term.Value.ToString();
        }

      
        private void nud_term_ValueChanged(object sender, EventArgs e)
        {
            var balance = Parse(txtBalance);
            var interest = ParseInt(txtInterest);

            if (balance.HasValue && interest.HasValue && errorProvider1.GetError(txtInterest) == string.Empty)
                CalcPayments(balance.Value, interest.Value / 100.00m);

            if (txtMonthPayResult.Text !="" && Convert.ToDecimal(txtMonthPayResult.Text) < monthpayments)
            {
                errorProvider2.SetError(txtMonthPayResult, String.Format("Calculated Payment less than minimum monthly percentage allows"));
                btnMinPay.Enabled = false;
                btnPrint.Enabled = false;
            }
            else
            {
                errorProvider2.SetError(txtMonthPayResult, "");
                btnMinPay.Enabled = true;
                btnPrint.Enabled = true;
            }
        }

        private void txtMonthlyPay_TextChanged(object sender, EventArgs e)
        {
            var balance = Parse(txtBalance);
            var interest = ParseInt(txtInterest);
            var monthlypayments = Parse(txtMonthlyPay);

            if (balance.HasValue && interest.HasValue && monthlypayments.HasValue && errorProvider1.GetError(txtInterest) == string.Empty)
                CalcMonths(balance.Value, interest.Value / 100.00m, monthlypayments.Value);
            // check monthly payment entered >= minimum % allowed
            if (monthlypayments < monthpayments)
            {
                errorProvider2.SetError(txtMonthlyPay, String.Format("Payment less than minimum monthly percentage allows"));
                btnMinPay.Enabled = false;
                btnPrint.Enabled = false;
            }
            else
            {
                errorProvider2.SetError(txtMonthlyPay, "");
                btnMinPay.Enabled = true;
                btnPrint.Enabled = true;
            }

        }

        private void txtBalance_TextChanged(object sender, EventArgs e)
        {
            if (tabFixedPay.SelectedIndex == 0)
                nud_term_ValueChanged(this, e);
            else
                txtMonthlyPay_TextChanged(this, e);

        }

        private void txtInterest_TextChanged(object sender, EventArgs e)
        {
            if (tabFixedPay.SelectedIndex == 0)
                nud_term_ValueChanged(this, e);
            else
                txtMonthlyPay_TextChanged(this, e);
        }

        private void btnMinPay_Click(object sender, EventArgs e)
        {
            var MinimumPay= tabFixedPay.SelectedIndex == 0 ? txtMonthPayResult.Text : txtMonthlyPay.Text;

            decimal selectedMinPay;
            if (!decimal.TryParse(MinimumPay.StripNonNumeric(), out selectedMinPay))
                errorProvider2.SetError(txtMonthlyPay, String.Format("Please enter a monthy payment."));
            else
            {
                Client.Call(new StoreCardUpdateMinimumPaymentRequest
                {
                    AcctNo = acctno,
                    MinimumPayment = selectedMinPay < storeCardMinPayment ? storeCardMinPayment : selectedMinPay,               // //#9919
                    MonthlyAmount = selectedMinPay < storeCardMinPayment ? storeCardMinPayment : selectedMinPay,                // //#9919 #9841 jec 27/03/12 set fixed payment
                },
                response =>
                {
                    Response(response.calcMinPayment, selectedMinPay);
                }, this);
            }
        }

        private void Response(decimal countryMin, decimal selectedMinPay)
        {

            if (countryMin > selectedMinPay)
            {
                if (tabFixedPay.SelectedIndex == 0)
                    errorProvider2.SetError(txtMonthPayResult, String.Format("Lowest minimum payment possible is {0}. Minimum payment updated to {0}.", countryMin.ToString(decimalplace)));
                else
                {
                    errorProvider2.SetError(txtMonthlyPay, String.Format("Lowest minimum payment possible is {0}. Minimum payment updated to {0}.", countryMin.ToString(decimalplace)));
                    txtMonthlyPay.Text = countryMin.ToString(decimalplace).StripNonNumeric();
                }
                MinimumPayment = countryMin;
            }
            else
            {
                MinimumPayment = selectedMinPay;
                Close();
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (tabFixedPay.SelectedIndex == 1 && txtMonthlyPay.Text == String.Empty || errorProvider1.GetError(txtTermsResult) != String.Empty)
                errorProvider1.SetError(btnPrint, "Please enter a valid select before printing.");
            else
            {
                var browser = MainForm.Current.CreateBrowserArray(1);
                object x = "";
                if (tabFixedPay.SelectedIndex == 0)
                    browser[0].Navigate(Config.Url + String.Format("Storecard/Calculator?acctno={0}&&balance={1}&&interestRate={2}&&term={3}&&monthyRepayments={4}&&total={5}", acctno, txtBalance.Text.StripNonNumeric(), txtInterest.Text.StripNonNumeric(), nud_term.Value.ToString(), txtMonthPayResult.Text.StripNonNumeric(), txtTotalTerms.Text.StripNonNumeric()), ref x, ref x, ref x, ref x);
                else
                    browser[0].Navigate(Config.Url + String.Format("Storecard/Calculator?acctno={0}&&balance={1}&&interestRate={2}&&term={3}&&monthyRepayments={4}&&total={5}", acctno, txtBalance.Text.StripNonNumeric(), txtInterest.Text.StripNonNumeric(), txtTermsResult.Text.StripNonNumeric(), txtMonthlyPay.Text.StripNonNumeric(), txtTotalPayments.Text.StripNonNumeric()), ref x, ref x, ref x, ref x);
            }
        }

        private void tabFixedPay_Selected(object sender, TabControlEventArgs e)
        {
            // if tabs changed check monthly payment
            if (tabFixedPay.SelectedIndex == 0)
            {
                nud_term_ValueChanged(null,null);
            }
            else
            {
                txtMonthlyPay_TextChanged(null, null);                
            }
        }
    }
}
