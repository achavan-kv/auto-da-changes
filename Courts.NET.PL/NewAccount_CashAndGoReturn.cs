using System;
using System.Collections.Generic;
using Blue.Cosacs.Shared;
using Blue.Cosacs.Shared.Services;
using Blue.Cosacs.Shared.Services.StoreCard;
using STL.Common.Constants.FTransaction;
using Blue.Cosacs.Shared.Extensions;

namespace STL.PL
{
    public partial class NewAccount : CommonForm
    {
        GetCashandGoReturnResponse CashandGoReturnDetails;
        PaymentReturn paymentform;

        private void SetupCashAndGoReturn(string acctno,int agreementno)
        { 
            Client.Call(new GetCashandGoReturnRequest() 
                            { 
                              AcctNo = acctno,
                              AgreementNo = agreementno
                            },
                            response =>
                            {
                                SetPaymentReturns(response.Payments);
                                CashandGoReturnDetails = response;
                            }, 
                            this);
        }

        private void SetPaymentReturns(List<View_FintransPayMethod> payments)
        { 
            drpPayMethod.Enabled = false;
            if (payments.Count == 1)
            {
                drpPayMethod.SelectedValue = payments[0].paymethod;
                if (payments[0].paymethod == PayMethod.StoreCard)
                {
                    mtb_CardNo.Enabled = false;
                    mtb_CardNo.Text = payments[0].storecardno.ToString();
                }
            }
            else
            {
                btn_CashandGoReturnPay.Visible = true;
                paymentform = new PaymentReturn(payments, Convert.ToDecimal(txtSubTotal.Text.StripNonNumeric()));
            }
        }

        private void btn_CashandGoReturnPay_Click(object sender, EventArgs e)
        {
            paymentform.ReturnTarget = Convert.ToDecimal(txtSubTotal.Text.StripNonNumeric());
            paymentform.ShowDialog();
        }

    }
}
