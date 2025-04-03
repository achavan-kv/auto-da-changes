using System;
using System.Data;
using System.Windows.Forms;
using Blue.Cosacs.Shared;
using Blue.Cosacs.Shared.Services.StoreCard;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using STL.Common.Static;
using STL.PL.Utils;


namespace STL.PL.StoreCard
{

    public delegate void Activation(object sender, GenericEventHandler<ActivateRequest> args);

    public partial class StoreCardActivate : UserControl
    {
        public event Activation ActivateEvent;
        private long CardNo;
        private bool NewCard;
        private Customer CurrentCust;
        private CustAddress CurrentAdd;

        public StoreCardActivate()
        {
            InitializeComponent();
        }

        public void Setup(StoreCardAccountResult scar, long cardNo, bool newcard)
        {

            cmb_ProofId.Set(((DataTable)StaticData.Tables[TN.ProofOfID]).AddBlankCode(), CN.CodeDescription, CN.Code);
            cmb_ProofAddress.Set(((DataTable)StaticData.Tables[TN.ProofOfAddress]).AddBlankCode(), CN.CodeDescription, CN.Code);
            var secQ = ((DataTable)StaticData.Tables[TN.StoreCardSecurityQuestion]).AddBlankCode();     // #9715
            secQ.DefaultView.Sort = CN.CodeDescription;                 // #9715
            cmb_SecurityQ.Set(secQ.DefaultView, CN.CodeDescription, CN.Code);

            CardNo = cardNo;
            NewCard = newcard;

            var custid = scar.GetPaymentByCard(cardNo).custid;

            CurrentCust = scar.Customers.Find(delegate(Customer c) { return c.custid == custid; });
            CurrentAdd = scar.Addresses.Find(delegate(CustAddress c) { return c.custid == custid; });

            txt_fullname.Text = String.Format("{0} {1}", CurrentCust.firstname, CurrentCust.name);

            if (CurrentAdd != null)
                txt_address.Text = String.Format("{0}" + Environment.NewLine +
                                                 "{1}" + Environment.NewLine +
                                                 "{2}" + Environment.NewLine +
                                                 "{3}" + Environment.NewLine, CurrentAdd.cusaddr1, CurrentAdd.cusaddr2, CurrentAdd.cusaddr3, CurrentAdd.cuspocode);
            else
            {
                txt_address.Text = String.Empty;
            }


            if (CurrentCust != null)
                txt_dob.Text = CurrentCust.dateborn.ToShortDateString();

            var status = scar.GetCardStatusCodeByCard(cardNo);
            var datechanged = scar.GetCardStatusDateCodeByCard(cardNo);

            if (StoreCardCardStatus_Lookup.Cancelled.Code.Equals(status))
                btn_activate.Text = "Re-Activate";
            else if (StoreCardCardStatus_Lookup.Active.Equals(status))
                btn_activate.Text = "Save Details";
            else if (StoreCardCardStatus_Lookup.AwaitingActivation.Equals(status))
                btn_activate.Text = "Activate";

            Activated(status, datechanged.Value, scar.GetPaymentByCard(cardNo), scar.AccountStatus);


            //if (scar.History.Count > 1 && StoreCardAccountStatus_Lookup.Cancelled.Equals(status))//check most recent record before cancellation and if to be issued do not allow activation
            //{
            //    if (scar.History[1].StatusCode == StoreCardAccountStatus_Lookup.CardToBeIssued.Code)
            //    {
            //        EnableControls(false);
            //    }
            //}
        }

        private void Activated(string status, DateTime datechanged, View_StoreCardWithPayments p, string accountStatus)
        {
            //EnableControls(true);

            // We need to add in card status filters here...

            EnableControls(status != string.Empty &&
                               (((StoreCardCardStatus_Lookup.AwaitingActivation.Equals(status) ||
                               StoreCardCardStatus_Lookup.Active.Equals(status) ||
                               StoreCardCardStatus_Lookup.Cancelled.Equals(status)) &&
                               (StoreCardAccountStatus_Lookup.Active.Equals(accountStatus) || StoreCardAccountStatus_Lookup.AwaitingActivation.Equals(accountStatus))) ||
                                    (StoreCardCardStatus_Lookup.Cancelled.Equals(status) && StoreCardAccountStatus_Lookup.CardToBeIssued.Equals(accountStatus))));

            if (status != null && p.ProofAddress != null)
            {
                txt_ProofAddressNotes.Text = p.ProofAddNotes;
                txt_ProofIdNotes.Text = p.ProofIDNotes;
                cmb_ProofAddress.SelectedValue = p.ProofAddress;
                cmb_ProofId.SelectedValue = p.ProofID;
                cmb_SecurityQ.SelectedValue = p.SecurityQ;
                txt_SecurityAnswer.Text = p.SecurityA;
                txt_ActivatedOn.Text = datechanged.ToShortDateString();
            }
            else
            {
                txt_ProofAddressNotes.Text = string.Empty;
                txt_ProofIdNotes.Text = string.Empty;
                cmb_ProofAddress.SelectedIndex = 0;
                cmb_ProofId.SelectedIndex = 0;
                cmb_SecurityQ.SelectedIndex = 0;
                txt_SecurityAnswer.Text = string.Empty;
                if (StoreCardCardStatus_Lookup.Active.Equals(status))       // #9908 jec 13/04/12
                {
                    txt_ActivatedOn.Text = datechanged.ToShortDateString();
                }
                else
                    txt_ActivatedOn.Text = string.Empty;
            }

        }

        private void EnableControls(bool enable)
        {
            cmb_ProofAddress.Enabled = enable;
            cmb_ProofId.Enabled = enable;
            cmb_SecurityQ.Enabled = enable;
            txt_ProofAddressNotes.Enabled = enable;
            txt_ProofIdNotes.Enabled = enable;
            txt_SecurityAnswer.Enabled = enable;
            btn_activate.Enabled = enable && !NewCard;
        }

        private void btn_activate_Click(object sender, EventArgs e)
        {
            if (ActivateEvent != null && ValidateForm())
            {
                ActivateEvent(this, new GenericEventHandler<ActivateRequest>
                (
                    new ActivateRequest()
                        {
                            CardNumber = CardNo,
                            SecurityA = txt_SecurityAnswer.Text,
                            SecurityQ = cmb_SecurityQ.SelectedValue.ToString(),
                            ProofID = cmb_ProofId.SelectedValue.ToString(),
                            ProofIDNotes = txt_ProofIdNotes.Text,
                            ProofAddress = cmb_ProofAddress.SelectedValue.ToString(),
                            ProofAddNotes = txt_ProofAddressNotes.Text,
                            BranchNo = Convert.ToInt16(Config.BranchCode),
                            EmpeeNo = Credential.UserId,
                            Reason = btn_activate.Text,
                            Date = DateTime.Now
                        }
                ));

                if (btn_activate.Text == "Activate")
                    txt_ActivatedOn.Text = DateTime.Now.ToShortDateString();
                btn_activate.Text = "Save Details";

            }
        }

        private bool ValidateForm()
        {
            return CheckSelected(cmb_SecurityQ) &
                   CheckSelected(txt_SecurityAnswer) &
                   CheckSelected(cmb_ProofId) &
                   CheckSelected(cmb_ProofAddress);
        }

        private bool CheckSelected(ComboBox c)
        {
            if (c.SelectedIndex == 0)
            {
                ErrorP.SetError(c, "Please choose a value from the list.");
                return false;
            }
            else
            {
                ErrorP.SetError(c, string.Empty);
                return true;
            }
        }

        private bool CheckSelected(TextBox t)
        {
            if (t.Text.Trim().Length < 2)
            {
                ErrorP.SetError(t, "You must supply a secret answer.");
                return false;
            }
            else
            {
                ErrorP.SetError(t, string.Empty);
                return true;
            }
        }

        private void cmb_SecurityQ_SelectedIndexChanged(object sender, EventArgs e)
        {
            UnError((ComboBox)sender);
        }

        private void UnError(ComboBox cmb)
        {
            if (ErrorP.GetError(cmb) != String.Empty && (cmb).SelectedIndex != 0)
                ErrorP.SetError(cmb, string.Empty);
        }

        private void cmb_ProofId_SelectedIndexChanged(object sender, EventArgs e)
        {
            UnError((ComboBox)sender);
        }

        private void cmb_ProofAddress_SelectedIndexChanged(object sender, EventArgs e)
        {
            UnError((ComboBox)sender);
        }

        private void txt_SecurityAnswer_TextChanged(object sender, EventArgs e)
        {
            if (ErrorP.GetError(txt_SecurityAnswer) != string.Empty && txt_SecurityAnswer.Text.Trim().Length > 1)
            {
                ErrorP.SetError(txt_SecurityAnswer, String.Empty);
            }
        }
    }
}
