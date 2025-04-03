using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Blue.Cosacs.Shared;
using Blue.Cosacs.Shared.Extensions;
using Blue.Cosacs.Shared.Services;
using Blue.Cosacs.Shared.Services.StoreCard;
using STL.Common;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using STL.Common.Static;
using STL.PL.Utils;



namespace STL.PL.StoreCard
{
    public delegate void PrintStatementEvent(object sender, GenericEventHandler<Int32> args);

    public partial class StoreCardPaymentDetails : UserControl
    {
        private View_StoreCardWithPayments PD;
        public event PrintStatementEvent PrintStatementEvent;
        int StatementMonths = 1;
        public string AcctNo { get; set; }
        int Interestfreedays = 0;
        bool loading = false;
        decimal StoreCardLimit;
        private List<View_StoreCardRateDetailsGetforPoints> Rates;


        public StoreCardPaymentDetails()
        {
            InitializeComponent();
        }
        private string DecimalPlaces { get; set; }


        private void InitializeDropDowns(CountryParameterCollection Country)
        {
            DecimalPlaces = (string)Country[CountryParameterNames.DecimalPlaces];

            comboPaymentMethod.Set(((DataTable)StaticData.Tables[TN.StoreCardPaymentMethod]), CN.CodeDescript, CN.CodeDescript);
            ComboContactMethod.Set(((DataTable)StaticData.Tables[TN.StoreCardContactMethod]), CN.CodeDescript, CN.CodeDescript);
            comboStatements.Set(StoreCardFreq_Lookup.AsList(), "Description", "Code");

            comboStatements.SelectedValue = Convert.ToString(Country[CountryParameterNames.StoreCardStatementFrequency]);

            dtp_StatementDate.Value = DateTime.Today.AddMonths(1);
            dtp_DatePaymentDue.Value = DateTime.Today.AddMonths(1);
            dtp_DatePaymentDue.Value = dtp_DatePaymentDue.Value.AddDays(Convert.ToInt32(Country[CountryParameterNames.SCardInterestFreeDays]));
        }




        public void Setup(StoreCardAccountResult scar, CountryParameterCollection Country, bool CanEditFinancialDetails)
        {
            loading = true;
            InitializeDropDowns(Country);
            Interestfreedays = Convert.ToInt32(MainForm.Current.Country[CountryParameterNames.SCardInterestFreeDays]);

            txtStoreCardLimit.ReadOnly = !CanEditFinancialDetails;

            PopulateRates(scar.Rates);
            AcctNo = scar.Acct.acctno;
            Rates = scar.Rates;
            PopulateData(scar, CanEditFinancialDetails);
            SetBalance(scar);
            DisableControls(StoreCardAccountStatus_Lookup.Cancelled.Equals(scar.AccountStatus));
            loading = false;
        }

        public void DisableControls(bool disable)
        {
            if (disable)
            {
                cmb_InterestOption.Enabled = !disable;
                comboStatements.Enabled = !disable;
                ComboContactMethod.Enabled = !disable;
                txtStoreCardLimit.Enabled = !disable;
                dtp_StatementDate.Enabled = !disable;
                dtp_DatePaymentDue.Enabled = !disable;
            }
        }

        public void SetBalance(StoreCardAccountResult scar)
        {
            StoreCardLimit = Convert.ToDecimal(scar.StoreCardWithPayments[0].StoreCardLimit);
            txtStoreCardLimit.Text = StoreCardLimit.ToString(DecimalPlaces);
            txtArrears.Text = Convert.ToDecimal(scar.Acct.arrears).ToString(DecimalPlaces);
            txtBalance.Text = Convert.ToDecimal(scar.Acct.outstbal).ToString(DecimalPlaces);
            txtStoreCardAvailable.Text = Convert.ToDecimal(scar.StoreCardWithPayments[0].StoreCardAvailable).ToString(DecimalPlaces);
            //txtMinPayment.Text = Convert.ToDecimal(PD.MinimumPayment).ToString(DecimalPlaces);

            if (scar.StoreCardStatements.Count > 0 && Convert.ToDecimal(scar.Acct.outstbal) > 0) //21/03/12 - #9822
            {
                // #9841 jec 27/03/12 set min pay to greater of Calc min pay or fixed payment
                if (Convert.ToDecimal(PD.MinimumPayment) > Convert.ToDecimal(scar.StoreCardStatements[0].StmtMinPayment))
                {
                    txtMinPayment.Text = Convert.ToDecimal(PD.MinimumPayment).ToString(DecimalPlaces);
                }
                else
                {
                    txtMinPayment.Text = Convert.ToDecimal(scar.StoreCardStatements[0].StmtMinPayment).ToString(DecimalPlaces);             //IP - 21/03/12 - #9809 - display latest statements Minimum Payment
                }
            }
            else
            {
                txtMinPayment.Text = Convert.ToDecimal("0").ToString(DecimalPlaces);
            }

            // #9841 jec 27/03/12 set min pay to greater of Calc min pay or fixed payment
            //if (Convert.ToDecimal(PD.MinimumPayment) > Convert.ToDecimal(scar.StoreCardStatements[0].StmtMinPayment))         
            //{
            //    txtMinPayment.Text = Convert.ToDecimal(PD.MinimumPayment).ToString(DecimalPlaces);
            //}
            //else
            //{
            //    txtMinPayment.Text = Convert.ToDecimal(PD.MonthlyAmount).ToString(DecimalPlaces);
            //}

            //if (StoreCardAccountStatus_Lookup.Active.Equals(scar.AccountStatus) || scar.Acct.outstbal == 0.0m) 
            //    dtp_DatePaymentDue.Enabled = true; //allow changing of due date if balance 0 and no active cards
            //else
            dtp_DatePaymentDue.Enabled = false;
        }

        private void PopulateRates(List<View_StoreCardRateDetailsGetforPoints> rates)
        {
            cmb_InterestOption.Set(rates, "NameWithRate", "Id");

            if (rates.Count == 0)
            {
                errorProvider1.SetError(cmb_InterestOption, "No rates applicible for this customer scorecard and score. Please check rates setup.");
                btnSave.Enabled = false;
            }
        }

        public void CalculateStatementMonths(string frequency)
        {
            switch (frequency)
            {
                case "Q": StatementMonths = 3;
                    break;
                case "B": StatementMonths = 2;
                    break;
                case "E": StatementMonths = 6;
                    break;
                default: StatementMonths = 1;
                    break;

            }


            if (PD != null && PD.DateLastStatementPrinted != null)
                dtp_StatementDate.Value = Convert.ToDateTime(PD.DateLastStatementPrinted).AddMonths(StatementMonths);


        }

        public void PopulateData(StoreCardAccountResult scar, bool CanEditFinancialDetails)
        {

            PD = scar.StoreCardWithPayments[0];
            txtInterestRate.Text = Convert.ToDouble(PD.InterestRate).ToString().StripNonNumeric();

            txtPendingInterest.Text = PD.PendingInterest.ToString(DecimalPlaces);
            TimeSpan T = Convert.ToDateTime(PD.LastInterestDate).AddMonths(1) - Convert.ToDateTime(PD.LastInterestDate);

            toolTip1.SetToolTip(txtPendingInterest, "Calculated on Average Balance " + Convert.ToString(PD.AverageBalance) +
                " with Effective Monthly Interest Rate " + Convert.ToString(StoreCardInterest.EffectiveInterest(Convert.ToDouble(PD.InterestRate), T)).Substring(0, 7));
            //comboStatementDates.Set(scar.StoreCardStatements, "DateFrom", "Id");
            comboStatementDates.SetStmtDates(scar.StoreCardStatements, "FromTo", "Id");         // #12360
            comboStatementDates.Enabled = btnPrintStatement.Enabled = scar.StoreCardStatements.Count > 0;


            if (PD.DateLastStatementPrinted != null)
                dtp_StatementDate.Value = Convert.ToDateTime(PD.DateLastStatementPrinted).AddMonths(1);

            CalculateStatementMonths(PD.StatementFrequency);

            if (PD.DatePaymentDue != null)
                dtp_DatePaymentDue.Value = Convert.ToDateTime(PD.DatePaymentDue);
            else
            {
                dtp_DatePaymentDue.Value = DateTime.Today.AddMonths(1);
                dtp_DatePaymentDue.Value = dtp_DatePaymentDue.Value.AddDays(Interestfreedays);
            }


            if (cmb_InterestOption.Items.Count > 0)
            {
                cmb_InterestOption.SelectedValue = PD.RateId;
                //cmb_InterestOption.Enabled = CanEditFinancialDetails && (StoreCardAccountStatus_Lookup.CardToBeIssued.Equals(scar.AccountStatus) || StoreCardAccountStatus_Lookup.AwaitingActivation.Equals(scar.AccountStatus)); // to be issued or awating activation
                cmb_InterestOption.Enabled = CanEditFinancialDetails;        //#11182 - LW75324 - If they have user right then allow user to change Interest option regardless of status.
            }

            if (!String.IsNullOrEmpty(PD.StatementFrequency))
                comboStatements.SelectedValue = PD.StatementFrequency;
            else
                comboStatements.SelectedValue = StoreCardFreq_Lookup.None;

            if (!String.IsNullOrEmpty(PD.PaymentMethod))
                comboPaymentMethod.SelectedValue = PD.PaymentMethod; //.Description;


            if (PD.DateNotePrinted != null)
            {
                dtNotePrinted.Value = Convert.ToDateTime(PD.DateNotePrinted);
            }
            else
            {
                lblNotePrinted.Visible = false;
                dtNotePrinted.Visible = false;
            }

            chkFixed.Checked = Convert.ToBoolean(PD.RateFixed);

            txtAccountStatus.Text = StoreCardAccountStatus_Lookup.FromString(scar.AccountStatus).Description;

        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidateAll())
            {
                Client.Call(new PayDetailsSaveRequest
                {
                    StorecardPaymentDetails = new StorecardPaymentDetails
                                                 {
                                                     acctno = AcctNo,
                                                     PaymentMethod = comboPaymentMethod.Text,
                                                     RateId = Convert.ToInt32(cmb_InterestOption.SelectedValue),
                                                     //InterestRate = Convert.ToInt32(((View_StoreCardRateDetailsGetforPoints)cmb_InterestOption.SelectedItem).PurchaseInterestRate), //15,
                                                     InterestRate = Convert.ToDouble(((View_StoreCardRateDetailsGetforPoints)cmb_InterestOption.SelectedItem).PurchaseInterestRate),//IP - 16/01/13 - Merged from CoSACS 6.5 //15,
                                                     RateFixed = chkFixed.Checked,
                                                     StatementFrequency = comboStatements.SelectedValue.ToString(),
                                                     DateLastStatementPrinted = dtp_StatementDate.Value.AddMonths(-StatementMonths),
                                                     DatePaymentDue = dtp_DatePaymentDue.Value,
                                                     //Status = "",
                                                     ContactMethod = ComboContactMethod.Text,
                                                     //DateNotePrinted = null,
                                                     CardLimit = Convert.ToDecimal(txtStoreCardLimit.Text.StripNonNumeric()),
                                                     CardAvailable = Convert.ToDecimal(txtStoreCardAvailable.Text.StripNonNumeric()),
                                                     LastUpdatedBy = Credential.UserId
                                                 }
                },
                    response =>
                    {
                        MainForm.Current.ShowStatus("Details Saved.");
                    }, this);
            }
            else
                MainForm.Current.ShowStatus("Details not saved. Please check warnings.");
        }

        private bool ValidateAll()
        {
            if (CheckCombo(ComboContactMethod) &
                CheckCombo(cmb_InterestOption) &
                CheckCombo(comboPaymentMethod) &
                CheckCombo(comboStatements) &
                errorProvider1.GetError(txtStoreCardLimit).Length == 0)
                return true;
            else
                return false;
        }

        private bool CheckCombo(ComboBox c)
        {
            if (c.SelectedIndex == -1 && c.Enabled)
            {
                errorProvider1.SetError(c, "Please select a value.");
                return false;
            }
            else
            {
                errorProvider1.SetError(c, "");
                return true;
            }
        }

        private void btnPrintStatement_Click(object sender, EventArgs e)
        {
            PrintStatementEvent(this, new GenericEventHandler<Int32>(Convert.ToInt32(comboStatementDates.SelectedValue)));
        }

        private void txtStoreCardLimit_TextChanged(object sender, EventArgs e)
        {
            errorProvider1.Clear();

            if (!loading)
            {
                decimal val;
              
                if (!decimal.TryParse(txtStoreCardLimit.Text.StripNonNumeric(), out val))
                {
                    errorProvider1.SetError(txtStoreCardLimit, "This value is not valid");
                    return;
                }

                var balance = Convert.ToDecimal(txtBalance.Text.StripNonNumeric());

                if (val > StoreCardLimit)
                {
                    errorProvider1.SetError(txtStoreCardLimit, String.Format("Cannot increase limit from {0}. Limit decrease only valid.", StoreCardLimit));
                }

                if (val < balance)
                {
                    errorProvider1.SetError(txtStoreCardLimit, "Cannot decrease limit below current balance.");
                }
                else
                    txtStoreCardAvailable.Text = (val - balance).ToString(DecimalPlaces);
            }
        }


        private void comboInterestOption_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmb_InterestOption.SelectedItem != null)
            {
                chkFixed.Checked = ((View_StoreCardRateDetailsGetforPoints)cmb_InterestOption.SelectedItem).RateFixed;
                txtInterestRate.Text = ((View_StoreCardRateDetailsGetforPoints)cmb_InterestOption.SelectedItem).PurchaseInterestRate.Value.ToString(DecimalPlaces);
            }
        }

        private void txtStoreCardLimit_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!((e.KeyChar >= '0' && e.KeyChar <= '9') || e.KeyChar == '.' || e.KeyChar == (char)Keys.Back))
            {
                e.Handled = true;
            }
        }

        //private void StatementDate_ValueChanged(object sender, EventArgs e)
        //{// not so valid any more. Payment due date can now be changed providing that store card is not active.....
        //    if (!loading)
        //    {

        //        if (dtp_StatementDate.Value > DateTime.Today.AddMonths(4))
        //        {
        //            dtp_StatementDate.Value = DateTime.Today.AddMonths(1);
        //            errorProvider1.SetError(dtp_StatementDate, "Statement Date cannot be more than 4 months in advance");
        //        }
        //        else
        //        {
        //            errorProvider1.Clear();
        //        }




        //    }
        //    //   if (PD !=null && PD.DateLastStatementPrinted !=null )
        //  //  datepaymentdue.Value =  StatementDate.Value.AddDays(Interestfreedays);
        //}

        private void comboStatements_SelectedIndexChanged(object sender, EventArgs e)
        {
            CalculateStatementMonths(comboStatements.SelectedValue.ToString());
        }

        private void datepaymentdue_ValueChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                if (dtp_DatePaymentDue.Value > DateTime.Today.AddMonths(2))
                {
                    dtp_DatePaymentDue.Value = DateTime.Today.AddMonths(1);
                    errorProvider1.SetError(dtp_DatePaymentDue, "Payment Date cannot be more than 2 months in advance");
                }
                else
                    errorProvider1.Clear();

                dtp_StatementDate.Value = dtp_DatePaymentDue.Value.AddDays(-Interestfreedays);

            }
        }

        private void btn_calc_Click(object sender, EventArgs e)
        {

            StoreCardPaymentCalc sp = new StoreCardPaymentCalc(txtBalance.Text.StripNonNumeric(), txtInterestRate.Text.StripNonNumeric(), AcctNo, MainForm.Current.Country[CountryParameterNames.DecimalPlaces].ToString(), PD.MonthlyAmount);       // #9919
            sp.ShowDialog();
            if (sp.MinimumPayment > 0 && sp.MinimumPayment > Convert.ToDecimal(txtMinPayment.Text.StripNonNumeric()))     // #9919
                txtMinPayment.Text = sp.MinimumPayment.ToString(DecimalPlaces);
        }


    }
}
