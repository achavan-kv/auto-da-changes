using System;
using System.Data;
using System.Windows.Forms;
using STL.Common;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.ColumnNames;
using STL.Common.Static;

namespace STL.PL
{

    public partial class BasicCustomerDetails : CommonForm
    {
        private bool _LoyaltyNewMember;
        //   private int LoyaltyAccountStatusPrevious;
        private int LoyaltyVoucherStatusPrevious;
        private int LoyaltyAccountStatus;
        private int LoyaltyVoucherStatus;
        private bool LEndenabled;
        //  private bool cancelled;
        private string LoyaltyAcct;
        private bool authorised = false;

        public bool LoyaltyNewMember
        {
            get { return _LoyaltyNewMember; }
            set { _LoyaltyNewMember = value; }
        }

        //private bool lostprevious;

        enum LStatusAcct : int { Current = 1, Cancelled = 2, Expired = 3, Unpaid = 4, None = 5 };
        enum LStatusVoucher : int { Active = 1, Review = 2, Suspended = 3, None = 4 };

        private void LoyaltySetup()
        {

            if (LoyaltyDropStatic.LoyatlyDrop == null || LoyaltyDropStatic.LoyatlyDrop.Tables.Count == 0)
            {
                LoyaltyDropStatic.LoyatlyDrop = StaticDataManager.GetLoyaltyDropData();
            }

            if (LoyaltyEnd_dtp.Enabled == true)
            {
                LEndenabled = true;
            }

            LoyaltyEnd_dtp.Visible = true;


            DataView memberview = new DataView(LoyaltyDropStatic.LoyatlyDrop.Tables[0], "Category = 'HCM'", "sortorder", DataViewRowState.CurrentRows);
            DataView reasonview = new DataView(LoyaltyDropStatic.LoyatlyDrop.Tables[0], "Category = 'HCC'", "sortorder", DataViewRowState.CurrentRows);

            LoyaltyType_cmb.DataSource = memberview;
            LoyaltyType_cmb.DisplayMember = "codedescript";
            LoyaltyType_cmb.ValueMember = "code";

            LoyaltyReason_cmb.DataSource = reasonview;
            LoyaltyReason_cmb.DisplayMember = "codedescript";
            LoyaltyReason_cmb.ValueMember = "code";

            DisableAll();

            this.LoyaltyType_cmb.SelectedIndexChanged += new System.EventHandler(this.LoyaltyType_cmb_SelectedIndexChanged);



            LoyaltyAccountStatus = (int)LStatusAcct.None;
            LoyaltyVoucherStatus = (int)LStatusVoucher.None;

        }

        private string ParseAcct(string name)
        {
            return Enum.Parse(typeof(LStatusAcct), name, true).ToString();
        }

        private string ParseVoucher(string name)
        {
            return Enum.Parse(typeof(LStatusVoucher), name, true).ToString();
        }

        private void DisableAll()
        {

            if (LoyaltyEnd_dtp.Enabled == true)
            {
                LEndenabled = true;
                btn_override.Enabled = false;
            }
            else
            {
                btn_override.Enabled = true;
            }
            loyaltymemno_mtb.Enabled = false;
            loyaltymemno_mtb.ReadOnly = false;
            LoyaltyEnd_dtp.Enabled = false;
            LoyaltyStart_dtp.Enabled = false;
            LoyaltyType_cmb.Enabled = false;
            loyaltyinfo_lbl.Visible = false;
            Loyalty_save_btn.Enabled = false;
            LoyaltyJoin_btn.Enabled = false;
            LoyaltyLogo_pb.Visible = false;
            LoyaltyAcct_btn.Enabled = false;
            LoyaltyVoucher_btn.Enabled = false;
            LoyaltyStatusVoucher_txt.Text = "Current";
            LoyaltyStatusAcct_txt.Text = "Active";
            LoyaltyAcct_btn.Enabled = false;
            LoyaltyReason_cmb.Visible = false;
            Reason_lbl.Visible = false;
            LoyaltyAcct = "";

            errorProvider1.SetError(Loyalty_save_btn, "");
            errorProvider1.SetError(loyaltymemno_mtb, "");
            errorProvider1.SetError(LoyaltyEnd_dtp, "");

            LoyaltyAccountStatus = (int)LStatusAcct.None;
            LoyaltyVoucherStatus = (int)LStatusVoucher.None;

        }


        public void LoyaltySetupRegister()
        {

            if (LEndenabled)
            {
                LoyaltyEnd_dtp.Enabled = true;
            }

            loyaltymemno_mtb.Enabled = true;
            loyaltymemno_mtb.ReadOnly = false;


            LoyaltyStart_dtp.Enabled = false;
            LoyaltyType_cmb.Enabled = true;
            loyaltyinfo_lbl.Visible = false;

            loyaltymemno_mtb.Text = "";
            LoyaltyStart_dtp.Value = ServerTime.Request();
            LoyaltyEnd_dtp.Value = ServerTime.Request().AddMonths(Convert.ToInt32(Country[CountryParameterNames.LoyaltyMembershipPeriod]));
            LoyaltyType_cmb.SelectedIndex = 0;
            LoyaltyStatusAcct_txt.Text = "Current";
            LoyaltyStatusVoucher_txt.Text = "Active";

            LoyaltyJoin_btn.Enabled = false;
            LoyaltyReason_cmb.Visible = false;
            Reason_lbl.Visible = false;
            LoyaltyVoucher_btn.Enabled = false;
            LoyaltyAcct_btn.Enabled = false;
            Loyalty_save_btn.Enabled = true;

            UpdateLoyaltyType();

            LoyaltyVoucherStatus = (int)LStatusVoucher.Active;
        }

        private void EnableEdit()
        {
            loyaltymemno_mtb.ReadOnly = true;
            loyaltymemno_mtb.Enabled = true;
            LoyaltyStart_dtp.Enabled = false;
            LoyaltyType_cmb.Enabled = false;
            loyaltyinfo_lbl.Visible = true;
            LoyaltyJoin_btn.Enabled = false;

            if (LoyaltyAccountStatus == (int)LStatusAcct.Unpaid)
            {
                LoyaltyJoin_btn.Enabled = false;
                loyaltyinfo_lbl.Text = "Membership will begin on payment of membership fee";
            }
            else if (LoyaltyAccountStatus == (int)LStatusAcct.Current)
            {
                loyaltyinfo_lbl.Text = "Membership will expire in " + LoyaltyEnd_dtp.Value.Subtract(ServerTime.Request()).Days.ToString() + " days ";
            }
            else
            {
                loyaltyinfo_lbl.Text = "Customer is not currently a member.";
            }

            if (LoyaltyAccountStatus == (int)LStatusAcct.Current)
            {
                LoyaltyLogo_pb.Visible = true;
            }

            if (LoyaltyAccountStatus == (int)LStatusAcct.Current || LoyaltyAccountStatus == (int)LStatusAcct.Unpaid)
            {
                LoyaltyAcct_btn.Enabled = true;
                LoyaltyVoucher_btn.Enabled = true;
                if (LEndenabled)
                {
                    LoyaltyEnd_dtp.Enabled = true;
                    Loyalty_save_btn.Enabled = true;
                    btn_override.Enabled = false;
                }
                else
                {
                    LoyaltyEnd_dtp.Enabled = false;
                    btn_override.Enabled = true;
                }
            }
            else
            {
                LoyaltyJoin_btn.Enabled = true;
                Loyalty_save_btn.Enabled = false;
            }
        }

        private void LoyaltyShow()
        {
            if (!((bool)Country[CountryParameterNames.LoyaltyScheme]))
            {
                tcDetails.TabPages.Remove(tpLoyaltyScheme);
            }
        }

        private void LoyaltyGetDetails(string custid)
        {
            if (custid == null)
            {
                custid = "";
            }
            if ((bool)Country[CountryParameterNames.LoyaltyScheme])
            {
                LoyaltySetup();


                STL.PL.WS3.Loyalty linfo = CustomerManager.LoyaltyGetDatabycustid(custid);


                if (linfo.memberno == null)
                {
                    DisableAll();
                    ClearScreen();
                    showtext("No membership found for this customer.");
                    loyaltyinfo_lbl.Visible = true;
                    LoyaltyJoin_btn.Enabled = true;
                    LoyaltyLogo_pb.Visible = false;
                }
                else
                {
                    loyaltymemno_mtb.Text = linfo.memberno;
                    LoyaltyStatusVoucher_txt.Text = ParseVoucher(linfo.statusvoucher.ToString());
                    LoyaltyStatusAcct_txt.Text = ParseAcct(linfo.statusacct.ToString());
                    LoyaltyAccountStatus = linfo.statusacct;
                    LoyaltyVoucherStatus = linfo.statusvoucher;
                    LoyaltyVoucherStatusPrevious = linfo.statusvoucher;
                    LoyaltyType_cmb.SelectedValue = linfo.membertype.ToString();
                    LoyaltyStart_dtp.Value = linfo.startdate;
                    LoyaltyEnd_dtp.Value = linfo.enddate;

                    LoyaltyAcct = CustomerManager.LoyaltyGetCashAccount(linfo.custid);
                    EnableEdit();
                    GetLoyaltyHistory(linfo.custid);
                }
            }
            UpdateVoucherButton();
        }

        private void GetLoyaltyHistory(string custid)
        {
            dgv_HChistory.DataSource = ((DataSet)CustomerManager.LoyaltyGetHistory(custid)).Tables[0];
        }

        private void showtext(string text)
        {
            loyaltyinfo_lbl.Text = text;
            loyaltyinfo_lbl.Visible = true;
        }

        private void ClearScreen()
        {
            loyaltymemno_mtb.Text = "";
            LoyaltyStatusAcct_txt.Text = "";
            LoyaltyStatusVoucher_txt.Text = "";
            LoyaltyType_cmb.SelectedIndex = 1;
            LoyaltyStart_dtp.Value = ServerTime.Request();
            LoyaltyEnd_dtp.Value = ServerTime.Request().AddMonths(Convert.ToInt32(Country[CountryParameterNames.LoyaltyMembershipPeriod]));
        }

        private void Loyalty_save_btn_Click(object sender, EventArgs e)
        {
            bool ok;
            try
            {
                ok = LoyaltySave();
            }
            catch (Exception ex)
            {
                Catch(ex, "LoyaltySave");
            }
        }

        private bool LoyaltySave()
        {
            bool saved = false;

            if (LoyaltyVerify())
            {
                STL.PL.WS3.Loyalty loyalty = new STL.PL.WS3.Loyalty();
                loyalty.custid = CustomerID;
                loyalty.memberno = loyaltymemno_mtb.Text;

                loyalty.statusacct = LoyaltyAccountStatus;
                loyalty.statusvoucher = LoyaltyVoucherStatus;

                loyalty.startdate = LoyaltyStart_dtp.Value;
                loyalty.enddate = LoyaltyEnd_dtp.Value;

                loyalty.user =  Credential.UserId.ToString();


                loyalty.membertype = Convert.ToChar(LoyaltyType_cmb.SelectedValue);

                if (loyalty.statusacct == (int)LStatusAcct.Cancelled)
                {
                    loyalty.enddate = ServerTime.Request();
                }

                if (LoyaltyReason_cmb.Visible == true)
                {
                    loyalty.cancel = Convert.ToChar(LoyaltyReason_cmb.SelectedValue);
                }

                if (errorProvider1.GetError(LoyaltyEnd_dtp).ToString().Length == 0)
                {
                    if (!ReturnSaveErrors(CustomerManager.LoyaltySave(loyalty)))
                    {
                        if (LoyaltyNewMember)
                        {
                            LoyaltyAddFee();
                            LoyaltyNewMember = false;
                        }

                        UpdateNewAcct(loyalty);
                        saved = true;
                        LoyaltyGetDetails(loyalty.custid);
                        ((MainForm)this.FormRoot).statusBar1.Text = "Membership saved.";
                        if (!LEndenabled)
                        {
                            Loyalty_save_btn.Enabled = false;
                        }
                    }
                }
            }
            else
            {
                ((MainForm)this.FormRoot).statusBar1.Text = "Changes not saved. Please correct warnings and try again.";
            }
            return saved;
        }

        private void UpdateNewAcct(STL.PL.WS3.Loyalty loyalty)
        {
            foreach (Crownwood.Magic.Controls.TabPage tp in ((MainForm)this.FormRoot).MainTabControl.TabPages)
            {
                if (tp.Control is NewAccount)
                {
                    foreach (DataRowView acct in dv)
                    {
                        if (((NewAccount)tp.Control).AccountNo == acct["AccountNumber"].ToString())
                        {
                            ((NewAccount)tp.Control).loyaltyinfo = loyalty;
                        }
                    }
                }

            }
        }

        private bool ReturnSaveErrors(string error)
        {

            if (error.Length > 0)
            {
                if (error.Contains("cancellation"))
                {
                    errorProvider1.SetError(Loyalty_save_btn, error);
                }
                else
                {
                    errorProvider1.SetError(loyaltymemno_mtb, error);
                }
                return true;
            }
            else
            {
                errorProvider1.SetError(Loyalty_save_btn, "");
                errorProvider1.SetError(loyaltymemno_mtb, "");
                return false;
            }
        }


        private bool CheckStaffisFree(char type)
        {
            DataView Staff = new DataView(LoyaltyDropStatic.LoyatlyDrop.Tables[0], "Category = 'HCM' AND reference = '1' AND code = '" + type + "'", "", DataViewRowState.CurrentRows);
            if (Staff.Count > 0 && !Convert.ToBoolean(Country[CountryParameterNames.LoyaltyMembershipFee]))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void LoyaltyAddFee()
        {
            string acctno = CustomerManager.LoyaltyGetCashAccount(CustomerID);

            if (acctno.Length == 0)
            {
                AccountManager.GenerateAccountNumber(Config.CountryCode,
                               Convert.ToInt16(Config.BranchCode),
                               AT.Cash,
                               false, 0,
                               out acctno,
                               out Error);
                if (Error.Length > 0)
                    ShowError(Error);
            }

            CustomerManager.LoyaltyAddFee(acctno.Replace("-", ""), LoyaltyType_cmb.SelectedValue.ToString(), CustomerID, Credential.UserId);
        }

        //private void UpdatePrevious(int acct, int voucher)
        //{
        //    LoyaltyAccountStatusPrevious = acct;
        //    LoyaltyVoucherStatusPrevious = voucher;
        //}

        //private void loyaltymemno_mtb_Leave(object sender, EventArgs e)
        //{
        //    LoyaltyVerify();
        //}

        public void LoyaltySelectTab()
        {
            tcDetails.SelectedTab = tpLoyaltyScheme;
        }

        private bool LoyaltyVerify()
        {
            bool valid = true;

            if (!LoyaltyCheckMembershipDigit())
            {
                errorProvider1.SetError(loyaltymemno_mtb, GetResource("LOYALTY_MEMDIGITCHECK"));
                valid = false;
            }
            else
            {
                errorProvider1.SetError(loyaltymemno_mtb, "");
            }



            if ((LoyaltyType_cmb.SelectedValue == null || LoyaltyType_cmb.SelectedValue.ToString().Trim() == "") && LoyaltyType_cmb.Enabled == true)
            {
                errorProvider1.SetError(LoyaltyType_cmb, GetResource("LOYALTY_MEMBERCHECK"));
                valid = false;
            }
            else
            {
                errorProvider1.SetError(LoyaltyType_cmb, "");
            }
            return valid;
        }

        public bool LoyaltyCheckMembershipDigit()
        {
            if (loyaltymemno_mtb.Text.Trim().Length != 16 && loyaltymemno_mtb.ReadOnly == false && loyaltymemno_mtb.Enabled == true)
            {
                return false;
            }
            else
            {
                return true;
            }

        }


        private void LoyaltyJoin_btn_Click(object sender, EventArgs e)
        {
            LoyaltyJoin();
        }

        public void LoyaltyJoin()
        {
            try
            {
                if (!CustomerManager.LoyaltyCheckCustomer(CustomerID))
                {
                    MessageBox.Show("Please save customer details before joining Home Club.", "Please save customer first", MessageBoxButtons.OK);
                }
                else
                {
                    LoyaltyNewMember = true;
                    LoyaltySetupRegister();
                }
            }
            catch (Exception ex)
            {
                Catch(new Exception("Loyalty New Membership" + ex.Message.ToString()), "LoyaltyJoin_btn");
            }
        }

        private void LoyaltyAcct_btn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you wish to cancel membership?", "Cancel Membership?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                LoyaltyAccountStatus = (int)LStatusAcct.Cancelled;
                LoyaltyStatusAcct_txt.Text = ParseAcct(LStatusAcct.Cancelled.ToString());
                LoyaltyAcct_btn.Enabled = false;
                LoyaltyVoucher_btn.Enabled = false;
                LoyaltyReason_cmb.Visible = true;
                Reason_lbl.Visible = true;
                Loyalty_save_btn.Enabled = true;
            }

        }

        private void LoyaltyVoucher_btn_Click(object sender, EventArgs e)
        {
            if (!authorised)
            {
                AuthorisePrompt auth = new AuthorisePrompt(this, LoyaltyVoucher_btn, "Authorisation is required for changing voucher status");
                auth.ShowDialog();
                authorised = auth.Authorised;
                //IP - 13/04/10 - UAT(79) UAT5.2
                LoyaltyVoucher_btn.Visible = true;
                LoyaltyVoucher_btn.Enabled = true;
            }

            if (authorised)
            {
                Loyalty_save_btn.Enabled = true;
                if (LoyaltyVoucherStatus == (int)LStatusVoucher.Suspended || LoyaltyVoucherStatus == (int)LStatusVoucher.Active)
                {
                    LoyaltyVoucherStatus = (int)LStatusVoucher.Review;
                }
                else
                {
                    if (LoyaltyVoucherStatusPrevious != (int)LStatusVoucher.None && LoyaltyVoucherStatusPrevious != (int)LStatusVoucher.Review)
                    {
                        LoyaltyVoucherStatus = LoyaltyVoucherStatusPrevious;
                    }
                    else
                    {
                        LoyaltyVoucherStatus = (int)LStatusVoucher.Active;
                    }
                }
                LoyaltyStatusVoucher_txt.Text = ParseVoucher(LoyaltyVoucherStatus.ToString());
                UpdateVoucherButton();
            }
        }

        private void UpdateVoucherButton()
        {
            if (LoyaltyVoucherStatus == (int)LStatusVoucher.Suspended || LoyaltyVoucherStatus == (int)LStatusVoucher.Active)
            {
                LoyaltyVoucher_btn.Text = "Review";
            }
            else
            {
                LoyaltyVoucher_btn.Text = "Cancel Review";
            }
        }
        private void loyaltymemno_mtb_KeyPress(object sender, KeyEventArgs e)
        {
            LoyaltyVerify();
        }

        private void LoyaltyType_cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (LoyaltyType_cmb.Enabled == true)
            {
                UpdateLoyaltyType();
            }
        }

        private void UpdateLoyaltyType()
        {
            if (LoyaltyType_cmb.SelectedValue != null)
            {
                if (!CheckStaffisFree(Convert.ToChar(LoyaltyType_cmb.SelectedValue)))
                {
                    LoyaltyAccountStatus = (int)LStatusAcct.Unpaid;
                    LoyaltyStatusAcct_txt.Text = ParseAcct(LStatusAcct.Unpaid.ToString());
                }
                else
                {
                    LoyaltyAccountStatus = (int)LStatusAcct.Current;
                    LoyaltyStatusAcct_txt.Text = ParseAcct(LStatusAcct.Current.ToString());
                }
            }
        }


        private void LoyaltyEnd_dtp_ValueChanged(object sender, EventArgs e)
        {
            if (LoyaltyEnd_dtp.Enabled == true)
            {
                if (LoyaltyEnd_dtp.Value < ServerTime.Request())
                {
                    errorProvider1.SetError(LoyaltyEnd_dtp, "The end date can not be in the past.");
                    Loyalty_save_btn.Enabled = false;
                }
                else
                {
                    errorProvider1.SetError(LoyaltyEnd_dtp, "");
                    Loyalty_save_btn.Enabled = true;
                }
            }
        }

        private void UpdateHomeClubAcctCode(ref DataView dv)
        {
            foreach (DataRow row in dv.Table.Rows)
            {
                if (row[CN.AccountNumber2].ToString() == LoyaltyAcct)
                {
                    row[CN.Type] = "HCC";
                }
            }
        }
        private void btn_override_Click(object sender, EventArgs e)
        {

            if (!LEndenabled)
            {
                AuthorisePrompt AP = new AuthorisePrompt(this, btn_override, "Authorisation required to change end date!");
                AP.ShowDialog();
                btn_override.Visible = true;

                if (AP.Authorised)
                {
                    LoyaltyEnd_dtp.Enabled = true;
                }
            }
        }

        private bool CheckCanJoinLoyalty(string acctno)
        {
            bool check = false;

            STL.PL.WS3.Loyalty loyaltyinfo = new STL.PL.WS3.Loyalty();
            loyaltyinfo = CustomerManager.LoyaltyGetDatabyacctno(acctno.Replace("-", ""));
            if ((loyaltyinfo.memberno == null || loyaltyinfo.statusacct != (int)LStatusAcct.Current) &&
                 loyaltyinfo.rejections < Convert.ToInt32(Country[CountryParameterNames.LoyaltyMaxJoinRejects]))
            {
                if (MessageBox.Show("Would you like to join or renew Home Club?",
                    "Customer currently not a Home Club member.", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    check = true;
                }
                else
                {
                    CustomerManager.LoyaltyAddRejection(loyaltyinfo.custid, acctno);
                }
            }
            return check;
        }
    }
}
