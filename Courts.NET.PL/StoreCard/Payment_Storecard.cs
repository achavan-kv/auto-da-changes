using System;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Blue.Cosacs.Shared;
using Blue.Cosacs.Shared.Services;
using Blue.Cosacs.Shared.Services.StoreCard;
using STL.Common;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.ColumnNames;
using STL.PL.StoreCard.Common;
using STL.PL.StoreCard;
using STL.PL.Utils;
using Blue.Cosacs.Shared.Extensions;
using STL.Common.Constants.FTransaction;

namespace STL.PL
{

    public partial class Payment : CommonForm
    {
        StoreCardValidated storeCardValidated;

        StoreCardPaymentCalc paymentCalc;

        const string StoreCard = "StoreCard";
        const string DefaultPaymentFilter = CN.Additional + " in ('B', 'P', '') and code < 200 and code >= 0"; //and code<> 0
        const string AdditionalPaymentFilterStoreCard = " and " + CN.CodeDescript + " not in ('StoreCard')";


        private void StoreCardCheckAccountType(string AccountType)
        {
            ((DataView)drpPayMethod.DataSource).RowFilter = ""; //IP - 29/11/10 - Need to re-set the filter before applying new filter

            if ((bool)Country[CountryParameterNames.StoreCardEnabled])
            {
                if (AccountType != AT.Cash)
                {
                    ((DataView)drpPayMethod.DataSource).RowFilter = DefaultPaymentFilter + AdditionalPaymentFilterStoreCard;
                }
                else //IP - 24/12/10 - Bug #2669 - Apply the normal filter for all other accounts
                {
                    ((DataView)drpPayMethod.DataSource).RowFilter = DefaultPaymentFilter;
                }
            }
        }

        private void CheckMaxItemVal()
        {
              StringBuilder sb = new StringBuilder();

            var selectedAcct = (((DataView)dgAccountList.DataSource)[dgAccountList.CurrentRowIndex].Row[CN.AccountNumber]).ToString();
            var maxItemVal = Convert.ToInt32(Country[CountryParameterNames.MaxItemValStoreCard]);

            //Retrieve the lineitems on the selected account
            DataSet itemsDs = AccountManager.GetItemsForAccount(selectedAcct, out error);
         
            itemsStoreCardView = new DataView(itemsDs.Tables[0], "ItemType = 'S'", "", DataViewRowState.OriginalRows);
            sb.Append("Item(s): ");
            string sep = string.Empty;
            foreach (DataRowView row in itemsStoreCardView)
            {
                if (Convert.ToDecimal(row[CN.Price]) > maxItemVal)
                {
                    sb.Append(String.Format("{0}{1} ", sep, row[CN.ItemNo]));
                    sep = ",";
                }
            }
            sb.Append("have exceeded the max value of " + maxItemVal.ToString() + " Please remove the item(s) before completing the sale.");

            if (sep != string.Empty)
            {
                errorProviderStoreCard.SetError(drpPayMethod, sb.ToString());
            }
            else
            {
                errorProviderStoreCard.SetError(drpPayMethod, "");
            }
        }

        private void StoreCardCheckPayButton()
        {
            if (storeCardValidated != null && storeCardValidated.Valid)
            {
                if (MoneyStrToDecimal(txtPayAmount.Text) > MoneyStrToDecimal(txtAgreementTotal.Text))
                    txtCardNo.Text = txtPayAmount.Text = txtAgreementTotal.Text;
                if (MoneyStrToDecimal(txtPayAmount.Text) > storeCardValidated.StoreCardAvailable)
                    errorProviderStoreCard.SetError(txtPayAmount, String.Format("Available Store Card credit ({0:0.00}) is less than amount to pay.", storeCardValidated.StoreCardAvailable));
                else
                    errorProviderStoreCard.SetError(txtPayAmount, string.Empty);
            }

            btnPay.Enabled = errorProviderStoreCard.GetError(drpPayMethod) == string.Empty &&
                             
                               errorProviderStoreCard.GetError(txtPayAmount) == string.Empty &&
                               errorProviderStoreCard.GetError(drpPayMethod) == string.Empty;       // #10126
               
        }

        private void SetFeeforStoreCard(bool set, bool? DisplayRF = null)
        {
            if (set)
            {
                txtFee.Enabled = false;
                txtFee.Visible = true;
                lFee.Visible = true;
                lFee.Text = "StoreCard Balance";
            }
            else
            {
                lFee.Text = "Credit Fee";
                //if (DisplayRF.HasValue)
                //    this.txtFee.Enabled = !DisplayRF.Value;

                this.txtFee.Visible = this._debitFee;
                this.lFee.Visible = this._debitFee;
            }
        }

        private void btn_calc_Click(object sender, EventArgs e)
        {
            if (storeCardValidated != null)
                paymentCalc = new StoreCardPaymentCalc(Math.Round(storeCardValidated.StoreCardBalance.Value + Convert.ToDecimal(txtPayAmount.Text.StripNonNumeric().ToDecimal()), 2).ToString(),
                    storeCardValidated.StoreCardInterest.ToString(), txtAccountNo.Text.Replace("-", ""), Country[CountryParameterNames.DecimalPlaces].ToString());
            paymentCalc.ShowDialog();
        }

        private void CheckPaymentCalc(int index)
        {
            if (((DataView)dgAccountList.DataSource)[index][CN.AccountType].ToString().Trim() == "T")
            {
                Client.Call(new GetInterestRequest
                {
                    acctno = ((DataView)dgAccountList.DataSource)[index][CN.AcctNo].ToString()
                },
               response => 
               {
                   paymentCalc = new StoreCardPaymentCalc(((DataView)dgAccountList.DataSource)[index][CN.OutstandingBalance].ToString(),
                       response.Interest.ToString(), ((DataView)dgAccountList.DataSource)[index][CN.AcctNo].ToString(), Country[CountryParameterNames.DecimalPlaces].ToString(),
                       response.MinimumPayment);   // #9859 jec 12/04/12
               }
            , this);
                
                
            }
        }

        public bool IsStoreCardPaymentSelected()
        {
            if (Convert.ToInt16(drpPayMethod.SelectedValue) == PayMethod.StoreCard)
                return true;
            else
                return false;
        }
    }
}
