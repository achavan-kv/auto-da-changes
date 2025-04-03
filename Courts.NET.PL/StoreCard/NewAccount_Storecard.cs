using System;
using System.Collections.Generic;
using System.Text;
using STL.Common.Constants.ColumnNames;
using Blue.Cosacs.Shared;
using System.Data;
using STL.Common;
using STL.PL.WS2;
using STL.PL.StoreCard.Common;
using Blue.Cosacs.Shared.Services;
using Blue.Cosacs.Shared.Services.StoreCard;
using Blue.Cosacs.Shared.Extensions;
using STL.Common.Static;
using STL.Common.Constants.FTransaction;

namespace STL.PL
{
    public partial class NewAccount : CommonForm
    {
        const string AdditionalPaymentFilterStoreCard = " and " + CN.CodeDescript + " not in ('StoreCard') ";
        const string DefaultPayFilter = CN.Additional + " in ('B', 'C', '') and " + CN.Code + " not = '8' and code >= 0 and code < 200";
        const string AllowCashAndGoChequesFilter = " and " + CN.Code + " not LIKE '%2'";
        const string StoreCard = "StoreCard";

        StoreCardCardReader CardReader = new StoreCardCardReader();

        StoreCardValidated storeCardValidated;
        CustDetails StoreCardCustDetails;
        int storeCardTransRefNo = 0;                //IP - 17/01/11 - Store Card
        private List<GetValidatedCardForRecieptResponse> storeCardPayInfoList = new List<GetValidatedCardForRecieptResponse>();
        private GetValidatedCardForRecieptResponse storeCardPayInfo;

       private void SetStoreCardControls(bool set)
        {
            if (set)
            {
                CheckMaxItemVal();  //IP - 03/12/10 - Store Card
                 ResetStoreCard();
                   btnStoreCardManualEntry.Visible = mtb_CardNo.ReadOnly = ((bool)Country[CountryParameterNames.StoreCardPasswordforManualEntry]);
                btnStoreCardManualEntry.Enabled = true;

                if (btnStoreCardManualEntry.Visible == false) //Country parameter not set so allow it
                    mtb_CardNo.ReadOnly = false;
            }
            else
            {
                btnStoreCardManualEntry.Visible = false;
                mtb_CardNo.Mask = "XXXX-XXXX-XXXX-0000";
                errorProviderStoreCard.SetError(drpPayMethod, "");
                itemValExceeded = false;
                itemsStoreCardView = null; 
                lblStoreCardBal.Visible = false;
                txtStoreCardBal.Visible = false;
                btnPrintReceipt.Enabled = true;
                mtb_CardNo.Enabled = true;
                lbl_BankAcctNo.Text = "Bank Account No";
                txtBankAcctNo.ReadOnly = false;
            }
        }

       private void ClearStoreCardErrors()
       {
           errorProviderStoreCard.SetError(drpPayMethod, string.Empty);
           errorProviderStoreCard.SetError(btnStoreCardManualEntry, string.Empty);
           errorProviderStoreCard.SetError(txtAmount, string.Empty);
           btnAddPaymethod.Enabled = true;      // #10126
       }

       private void ResetStoreCard(bool showbox = true)
       {
           txtBankAcctNo.Enabled = true;
           drpBank.Enabled = false;
           mtb_CardNo.Mask = "0000-0000-0000-0000";
           btnPrintReceipt.Enabled = CashAndGoReturn;
           mtb_CardNo.Visible = true;
           mtb_CardNo.Enabled = true;
           lblStoreCardBal.Visible = showbox;
           txtStoreCardBal.Visible = showbox;
           lbl_BankAcctNo.Text = "StoreCard Name";
           txtBankAcctNo.Text = string.Empty;
           txtStoreCardBal.Text = string.Empty;
           storeCardValidated = new StoreCardValidated();
           txtBankAcctNo.ReadOnly = true;
       }

        private void ValidateStoreCard()
        {
            if (!StoreCardValidation.IsStoreCardValid(mtb_CardNo.Text))
            {
                storeCardValidated = new StoreCardValidated();
                StoreCardCheckPayButton();
            }
            else
            {
                Client.Call(new GetValidatedCardForRecieptRequest()
                {
                    CardNo = Convert.ToInt64(mtb_CardNo.Text)
                },
                response =>
                {
                    storeCardValidated = response.StoreCardValidated;
                    StoreCardCustDetails = response.CustDetails;
                    storeCardPayInfo = response;
                    ValidateResponse(response.StoreCardValidated.Name, response.StoreCardValidated.Valid, response.StoreCardValidated.RejectReason, response.StoreCardValidated.StoreCardAvailable);
                },
                this);
            }
        }

        private void ValidateResponse(string name, bool valid, string reason,decimal? avail)
        {
            errorProviderStoreCard.SetError(btnStoreCardManualEntry, reason);
            if (valid)
            {
                txtBankAcctNo.Text = name;
               
                txtStoreCardBal.Text = avail.HasValue ? Convert.ToDecimal(avail).ToString("F2") : "0";
            }
            StoreCardCheckPayButton();
        }

        private void StoreCardCheckPayButton()
        {
            if (storeCardValidated != null && storeCardValidated.Valid)
            {
                if (MoneyStrToDecimal(txtAmount.Text) > MoneyStrToDecimal(txtDue.Text))
                    txtAmount.Text = txtDue.Text;
                if (MoneyStrToDecimal(txtAmount.Text) > storeCardValidated.StoreCardAvailable)
                    errorProviderStoreCard.SetError(txtAmount, String.Format("Available Store Card credit ({0:0.00}) is less than amount to pay.", storeCardValidated.StoreCardAvailable));
                else
                    errorProviderStoreCard.SetError(txtAmount, string.Empty);
            }
            else
            {
                if (errorProviderStoreCard.GetError(btnStoreCardManualEntry) == string.Empty)
                {
                    errorProviderStoreCard.SetError(btnStoreCardManualEntry, "Please enter a valid storecard.");
                    ResetStoreCard();
                }
            }

            btnPrintReceipt.Enabled = errorProviderStoreCard.GetError(drpPayMethod) == string.Empty &&
                                      errorProviderStoreCard.GetError(btnStoreCardManualEntry) == string.Empty &&
                                      errorProviderStoreCard.GetError(txtAmount) == string.Empty;

            btnAddPaymethod.Enabled = errorProviderStoreCard.GetError(txtAmount) == string.Empty;                         //#14358
        }


        private void CheckMaxItemVal()
        {
            itemValExceeded = false;
            StringBuilder sb = new StringBuilder();

            var maxItemVal = Convert.ToInt32(Country[CountryParameterNames.MaxItemValStoreCard]);

            itemsStoreCardView = new DataView(itemsTable, "ItemType = 'Stock' OR ItemType = 'Component'", "", DataViewRowState.CurrentRows);
            itemsStoreCardView.RowFilter = "ItemType = 'Stock' OR ItemType = 'Component'"; //IP - 06/12/10 - also include kit components

            sb.Append("Item(s): ");
            string sep = string.Empty;
            foreach (DataRowView row in itemsStoreCardView)
            {
                if (Convert.ToDecimal(StripCurrency(row[CN.UnitPrice].ToString())) > maxItemVal)
                {
                    sb.Append(String.Format("{0}{1} ", sep, row[CN.ProductCode]));
                    sep = ",";
                }
            }
            sb.Append("have exceeded the max value of " + maxItemVal.ToString() +  " Please remove the item(s) before completing the sale.");

            if (sep != string.Empty)
            {
                btnAddPaymethod.Enabled = false;        // #10126
                errorProviderStoreCard.SetError(drpPayMethod, sb.ToString());
            }
            else
            {
                errorProviderStoreCard.SetError(drpPayMethod, "");
                btnAddPaymethod.Enabled = true;        // #10126
            }
        }

        private void btnStoreCard_Click(object sender, EventArgs e)
        {
            AuthoriseCheck Auth = new AuthoriseCheck("Payment", "mtb_Cardno");
             
            mtb_CardNo.Visible = true;

            if (!Auth.ControlPermissionCheck(Credential.User).HasValue)
            {
                //    tcMain.SelectedTab = tpEmployee;
                Auth.ShowDialog();
                if (Auth.IsAuthorised)
                {
                    mtb_CardNo.ReadOnly = false;
                    mtb_CardNo.Focus();
                }
                else
                    mtb_CardNo.ReadOnly = true;
            }
            else
            {
                mtb_CardNo.ReadOnly = false;
                mtb_CardNo.Focus();
            }
        }

        public bool IsStoreCardPaymentSelected()
        {
            if (PaidAndTaken && Convert.ToInt16(drpPayMethod.SelectedValue) == PayMethod.StoreCard && !CashAndGoReturn)
                return true;
            else
                return false;
        }

        

    }
}
