using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using STL.Common;
using System.Web.Services.Protocols;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Threading;
using System.Xml;
using STL.PL.WS1;
using STL.PL.WS2;
using STL.PL.WS3;
using STL.PL.WS4;
using STL.PL.WS5;
using STL.Common.Constants.Tags;
using STL.Common.Static;
using STL.Common.Constants.Categories;
using STL.Common.Constants.ItemTypes;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.FTransaction;
using STL.Common.Constants.Enums;
using Crownwood.Magic.Menus;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.Elements;
using STL.Common.Constants.TableNames;
using STL.PL.XMLPrinting;
using mshtml;
using AxSHDocVw;
using STL.Common.Constants.ScreenModes;
using STL.Common.Constants.SanctionStages;
using STL.Common.Constants.TermsTypes;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace STL.PL
{
    public partial class NewAccount : CommonForm
    {
        private STL.PL.WS3.Loyalty _loyaltyinfo;
        //private bool loyaltycurrentmemberquestion = false;
        // private string _LoyaltyCustid = "";
        // private bool _LoyaltyJoin = false;
        private bool _LoyaltyMember = false;
        private string _LoyaltyMemberNo;
        private bool _LoyaltySuspended;
        private int _RedemptionNo = 0;
        public bool Redemption = false;
        char[] addtypes = new char[10];
        enum LStatusAcct { Current = 1, Cancelled = 2, Expired = 3, Unpaid = 4, None = 5 };
        enum LStatusVoucher { Active = 1, Review = 2, Suspended = 3, None = 4 };
        // private string Acctno = "";
        private bool showfreedelmessage = true;
        private bool _lasttab = false;
        // private bool LoyaltyNewAccount = false;
        private bool SaveVoucher = false;
        public int usedvoucher = 0;
        public bool voucheradd = true;
        private int DelAddressUpdated = 0;
        private bool voucheraddedonrevise = false;

        private List<STL.PL.WS3.LoyaltyValidAddresses> _ValidCustAddresses;
        public List<STL.PL.WS3.LoyaltyValidAddresses> ValidCustAddresses
        {
            get
            {
                if (_ValidCustAddresses == null)
                {
                    _ValidCustAddresses = new List<STL.PL.WS3.LoyaltyValidAddresses>();
                }

                if (loyaltyinfo != null && loyaltyinfo.custid != null)
                {
                    _ValidCustAddresses.AddRange(CustomerManager.LoyaltyGetValidAddress(loyaltyinfo.custid));
                }

                return _ValidCustAddresses;
            }

            set { _ValidCustAddresses = value; }
        }


        public STL.PL.WS3.Loyalty loyaltyinfo
        {
            get { return _loyaltyinfo; }
            set
            {
                _loyaltyinfo = value;
                if (_loyaltyinfo.statusacct == (int)LStatusAcct.Current)
                {
                    Loyalty_HClogo_pb.Visible = true;
                    LoyaltyMember = true;
                }
                CheckFreeDelWarning();

            }
        }

        public int RedemptionNo
        {
            get { return _RedemptionNo; }
            set
            {
                _RedemptionNo = value;
                Redemption = true;
            }
        }

        public bool lasttab
        {
            get { return _lasttab; }
            set { _lasttab = value; }
        }


        //public string LoyaltyCustid
        //{
        //    get { return _LoyaltyCustid; }
        //    set { _LoyaltyCustid = value; }
        //}

        public string LoyaltyMemberNo
        {
            get { return _LoyaltyMemberNo; }
            set
            {
                _LoyaltyMemberNo = value;
                Loyalty_HClogo_pb.Visible = true;
            }
        }

        //public bool LoyaltyJoin
        //{
        //    get { return _LoyaltyJoin; }
        //    set
        //    {
        //        _LoyaltyJoin = value;
        //        Loyalty_HClogo_pb.Visible = true;
        //    }
        //}

        public bool LoyaltySuspended
        {
            get { return _LoyaltySuspended; }
            set
            {
                _LoyaltySuspended = value;
            }
        }

        public bool LoyaltyMember
        {
            get { return _LoyaltyMember; }
            set
            {
                _LoyaltyMember = value;
            }
        }

        private void LoyaltyBreakRedemption()
        {
            if (Redemption)
            {
                RedemptionNo = 0;
                Redemption = false;
            }
        }


        //private void LoyaltyCheckMemberByCustid(string custid)
        //{
        //    if (Convert.ToBoolean(Country[CountryParameterNames.LoyaltyScheme]))
        //    {
        //        loyaltyinfo = CustomerManager.LoyaltyGetDatabycustid(custid);
        //        checkMember(loyaltyinfo, "");
        //    }
        //}

        private void LoyaltyCheckMemberbyAcctno(string acctno, bool FromCustomerDetails)
        {
            if (Convert.ToBoolean(Country[CountryParameterNames.LoyaltyScheme]))
            {
                loyaltyinfo = CustomerManager.LoyaltyGetDatabyacctno(acctno.Replace("-", ""));
                checkMember(loyaltyinfo, acctno.Replace("-", ""), FromCustomerDetails);
            }
        }

        //public void GetValidAdresses(string Custid)
        //{
        //    //ValidCustAddresses = new List<LoyaltyValidAddresses>();
        //    ValidCustAddresses.AddRange(CustomerManager.LoyaltyGetValidAddress(Custid));
        //}

        private void checkMember(STL.PL.WS3.Loyalty loyaltyinfo, string acctno, bool FromCustomerDetails)
        {

            if (acctno.Length == 12) // Account is being revised!
            {
                if (CustomerManager.LoyaltyCheckAccountPeriod(acctno))
                {
                    PopulateforValidMember(loyaltyinfo);
                }
                else
                {
                    if (loyaltyinfo.memberno == null ||
                        ((loyaltyinfo.memberno.Length == 16 && loyaltyinfo.statusacct == (int)LStatusAcct.Cancelled)
                        || (loyaltyinfo.memberno.Length == 16 && loyaltyinfo.statusacct == (int)LStatusAcct.Expired)))
                    {
                        PopulateforNonMember(loyaltyinfo, acctno, FromCustomerDetails);
                    }

                    LoyaltyMember = false;
                }

            }
            else
            {
                CheckMembership(acctno, FromCustomerDetails);
            }
        }


        private void CheckMembership(string acctno, bool FromCustomerDetails)
        {
            if (loyaltyinfo.memberno != null && loyaltyinfo.memberno.Length == 16 && loyaltyinfo.statusacct == (int)LStatusAcct.Current)
            {
                PopulateforValidMember(loyaltyinfo);
            }
            else
            {
                PopulateforNonMember(loyaltyinfo, acctno, FromCustomerDetails);
            }
        }
        private void PopulateforValidMember(STL.PL.WS3.Loyalty loyaltyinfo)
        {
            LoyaltyMemberNo = loyaltyinfo.memberno;
            //CustomerID = loyaltyinfo.custid;
            LoyaltyMember = true;

            //GetValidAdresses(loyaltyinfo.custid);

            if (loyaltyinfo.statusvoucher == (int)LStatusVoucher.Suspended)
            {
                LoyaltySuspended = true;
            }
        }

        private void PopulateforNonMember(STL.PL.WS3.Loyalty loyaltyinfo, string acctno, bool FromCustomerDetails)
        {
            if (!FromCustomerDetails && loyaltyinfo.statusacct != (int)LStatusAcct.Unpaid && loyaltyinfo.rejections < Convert.ToInt32(Country[CountryParameterNames.LoyaltyMaxJoinRejects]))
            {
                if (MessageBox.Show("Would you like to join or renew Home Club?" + Environment.NewLine +
                    Environment.NewLine +
                    "If YES the LoyaltyScreen will be opened.", "Customer currently not a Home Club member.", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    BasicCustomerDetails BCD = new BasicCustomerDetails(false, loyaltyinfo.custid, FormRoot, this);
                    BCD.FormParent = this;
                    BCD.FormRoot = this.FormRoot;
                    ((MainForm)this.FormRoot).AddTabPage(BCD, 10);
                    BCD.loaded = true;
                    BCD.LoyaltySelectTab();
                    // BCD.LoyaltyJoin();

                    lasttab = true;
                    //LoyaltyJoin = true;

                    //Loyalty_HClogo_pb.Visible = true;

                    //foreach (Crownwood.Magic.Controls.TabPage tab in ((MainForm)this.FormRoot).MainTabControl.TabPages)
                    //{
                    //    if (tab.Control is BasicCustomerDetails && ((BasicCustomerDetails)tab.Control).CustomerID == loyaltyinfo.custid)
                    //    {
                    //        ((MainForm)this.FormRoot).MainTabControl.SelectedTab = tab;
                    //    }
                    //}


                }
                else
                {

                    CustomerManager.LoyaltyAddRejection(loyaltyinfo.custid, acctno);
                }
            }
        }


        private bool LoyaltyDeliveryFree(decimal delcharge)
        {
            if (AgreementTotalDec - delcharge >= (decimal)Country[CountryParameterNames.LoyaltyCashThreshold] && (decimal)Country[CountryParameterNames.LoyaltyCashThreshold] != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //private void LoyaltyLoad()
        //{
        //    if (LoyaltyDropStatic.LoyatlyDrop == null || LoyaltyDropStatic.LoyatlyDrop.Tables.Count == 0)
        //    {
        //        LoyaltyDropStatic.LoyatlyDrop = StaticDataManager.GetLoyaltyDropData();
        //    }
        //}

        //private void LoyaltyCheckScreen()
        //{
        //    if (((bool)Country[CountryParameterNames.LoyaltyScheme]))
        //    {
        //        LoyaltyCheck Lcheck = new LoyaltyCheck(this);
        //        Lcheck.ShowDialog();
        //    }
        //}

        public void ClearItemCode()
        {
            txtProductCode.Text = "";
        }

        public void AddItem(string value)
        {
            ValidateProdcode();

            if (!((StringCollection)(drpLocation.DataSource)).Contains(drpBranch.SelectedValue.ToString()))
            {
                errorProvider1.SetError(drpLocation, "This item is not setup at branch " + drpBranch.SelectedValue.ToString());
                txtProductCode.Text = "";
            }
            else
            {
                errorProvider1.SetError(drpLocation, "");

                drpLocation.SelectedItem = drpBranch.SelectedValue.ToString();
                drpLocationValidation();
                txtUnitPrice.Text = value;
                txtValue.Text = value;
                txtValue.ReadOnly = true;
                txtAvailable.Text = "1";
                // txtDamaged.Text = "0";
                btnEnter.Enabled = true;
                txtColourTrim.Text = "";
                txtQuantity.Text = "1";
                //uat19 rdb 22/2/08 uncheck immediate del
                cbImmediate.Checked = false;

            }
        }

        private void CheckForVoucher(bool add, string itemno)
        {
            XmlNode Voucher = itemDoc.SelectSingleNode("//Item[@Code = '" + LoyaltyDropStatic.VoucherCode + "' ]");
            if (Voucher != null && itemno.ToUpper() == LoyaltyDropStatic.VoucherCode)
            {
                if (add && RedemptionNo != 0)
                {
                    usedvoucher = RedemptionNo;
                    voucheradd = true;
                }

                if (!add)
                {
                    usedvoucher = 0;
                    RedemptionNo = 0;
                    voucheradd = false;
                }
                SaveVoucher = true;
            }
        }

        private void CheckForFreeDelivery()
        {
            if (Convert.ToBoolean(Country[CountryParameterNames.LoyaltyScheme]) && LoyaltyMember && AccountType == "C")
            {
                XmlNode DEL = itemDoc.SelectSingleNode("//Item[@Code = 'DEL']");
                XmlNode HCDEL = itemDoc.SelectSingleNode("//Item[@Code = '" + LoyaltyDropStatic.FreeDelivery + "']");

                if (AgreementTotalDec >= (decimal)Country[CountryParameterNames.LoyaltyCashThreshold] && (decimal)Country[CountryParameterNames.LoyaltyCashThreshold] != 0
                    && showfreedelmessage)
                {
                    showfreedelmessage = false;
                    MessageBox.Show("Free delivery is available for this customer.", "Free Delivery Available", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                if (HCDEL != null || DEL != null)
                {
                    //Check if below threshold.
                    if (HCDEL != null && (!LoyaltyDeliveryFree(Convert.ToDecimal(DEL.Attributes[Tags.Value].Value))) || !ScanProductsForValidAdresses())
                    {
                        HCDEL.Attributes[Tags.Quantity].Value = "0";
                        HCDEL.Attributes[Tags.Value].Value = "0";
                    }

                    //If above threshold.
                    if (DEL != null && LoyaltyDeliveryFree(Convert.ToDecimal(DEL.Attributes[Tags.Value].Value)) && ScanProductsForValidAdresses())
                    {
                        if (HCDEL != null && DEL != null)
                        {
                            HCDEL.Attributes[Tags.Quantity].Value = DEL.Attributes[Tags.Quantity].Value;
                        }


                        if (DEL != null && (HCDEL == null || (Convert.ToDecimal(HCDEL.Attributes[Tags.Value].Value) * -1 != Convert.ToDecimal(DEL.Attributes[Tags.Value].Value))))
                        {
                            string HCDELcode = LoyaltyDropStatic.FreeDelivery;

                            if (HCDEL == null)
                            {


                                currentItem = AccountManager.GetItemDetails(new GetItemDetailsRequest
                                {
                                    ProductCode = HCDELcode,
                                    StockLocationNo = Convert.ToInt16(drpBranch.SelectedValue),
                                    BranchCode = Convert.ToInt16(drpBranch.SelectedValue),
                                    AccountType = AT.Cash,
                                    CountryCode = Config.CountryCode,
                                    IsDutyFree = chxDutyFree.Checked,
                                    IsTaxExempt = chxTaxExempt.Checked,
                                    PromoBranch = Convert.ToInt16(Config.BranchCode),
                                    AccountNo = this.AccountNo,
                                    AgrmtNo = this.AgreementNo

                                },
                                    out Error);

                                currentItem.Attributes[Tags.Value].Value = (Convert.ToDecimal(this.StripCurrency(DEL.Attributes[Tags.Value].Value.ToString())) * -1).ToString();
                                currentItem.Attributes[Tags.UnitPrice].Value = (Convert.ToDecimal(this.StripCurrency(DEL.Attributes[Tags.Value].Value.ToString())) * -1).ToString();
                                currentItem.Attributes[Tags.Quantity].Value = "1";
                                currentItem.Attributes[Tags.DeliveryDate].Value = ServerTime.Request().ToShortTimeString();
                                currentItem.Attributes[Tags.BranchForDeliveryNote].Value = DEL.Attributes[Tags.BranchForDeliveryNote].Value.ToString();
                                currentItem.Attributes[Tags.Location].Value = DEL.Attributes[Tags.Location].Value.ToString();

                                XmlNode tempNode = itemDoc.ImportNode(currentItem, true);
                                itemDoc.DocumentElement.AppendChild(tempNode);
                            }
                            else
                            {
                                HCDEL.Attributes[Tags.Value].Value = (Convert.ToDecimal(this.StripCurrency(DEL.Attributes[Tags.Value].Value.ToString())) * -1).ToString();
                                HCDEL.Attributes[Tags.UnitPrice].Value = (Convert.ToDecimal(this.StripCurrency(DEL.Attributes[Tags.Value].Value.ToString())) * -1).ToString();

                            }
                        }

                    }
                }
                CheckFreeDelWarning();
                populateTable();
            }
        }



        private void LoyaltyCheckItem()
        {

           // errorProvider1.SetError(txtProductCode, "");

            DataRow[] item = LoyaltyDropStatic.LoyatlyDrop.Tables[0].Select("Category = 'HCI' AND code = '" + txtProductCode.Text.Trim() + "'");

            if (LoyaltyMember == false && item.Length != 0)
            {
                errorProvider1.SetError(txtProductCode, "Customer is not a valid Home Club member. Item addition not valid.");
                txtProductCode.Text = "";
            }
            else
            {
                if (LoyaltyDropStatic.VoucherCode == txtProductCode.Text.Trim().ToUpper())
                {
                    if (drpAccountType.SelectedItem.ToString() == AT.Cash || drpAccountType.SelectedItem.ToString() == AT.Special)
                    {
                        errorProvider1.SetError(txtProductCode, "Voucher Redemption is only for credit accounts.");
                        txtProductCode.Text = "";
                    }
                    else
                    {
                        if ((LoyaltyMember == true) && LoyaltySuspended == false)
                        {

                            string decimalplaces = Country[CountryParameterNames.DecimalPlaces].ToString();
                            LoyaltyRedeemVoucher RVoucher = new LoyaltyRedeemVoucher(decimalplaces, loyaltyinfo.custid, LoyaltyMemberNo, this);
                            RVoucher.ShowDialog();
                        }
                        else
                        {
                            txtProductCode.Text = "";
                            errorProvider1.SetError(txtProductCode, "Customers who are not currently members or are suspended can not redeem vouchers.");
                        }
                    }
                }
            }

        }

        private void LoyaltySaveVouchers()
        {
            if (SaveVoucher)
            {
                CustomerManager.LoyaltySaveVouchers(usedvoucher, voucheradd, txtAccountNumber.Text.Replace("-", ""));
            }

            var node = itemDoc.SelectSingleNode("//Item[@Code = '" + LoyaltyDropStatic.VoucherCode + "' and @Value = 0]");
            if (voucheraddedonrevise && node != null)
            {
                CustomerManager.LoyaltyRemoveVoucher(txtAccountNumber.Text.Replace("-", ""));
            }

        }

        private void drpDeliveryAddress_SelectedIndexChanged(object sender, EventArgs e)
        {

            DelAddressUpdated++;

            if (loyaltyinfo != null)
            {
                if (AccountType == AT.Cash && (loyaltyinfo.memberno != null && (loyaltyinfo.memberno.Length > 0)))
                {
                    ValidCustAddresses.AddRange(CustomerManager.LoyaltyGetValidAddress(loyaltyinfo.custid));
                    CheckFreeDelWarning();
                }
            }
        }

        private void CheckFreeDelWarning()
        {
            if (drpDeliveryAddress.SelectedItem != null)
            {
                if (Convert.ToBoolean(Country[CountryParameterNames.LoyaltyScheme]) &&
                    (FindFreeDelItem() != null || LoyaltyMember) &&
                    !CheckAddress(drpDeliveryAddress.SelectedItem.ToString().Trim()) &&
                    AccountType == AT.Cash &&
                    Convert.ToDouble(StripCurrency(txtAgreementTotal.Text)) > Convert.ToInt32(Country[CountryParameterNames.LoyaltyCashThreshold]))
                {
                    errorProvider1.SetError(drpDeliveryAddress, "Postcode for this address is not a valid code. Free delivery may not applied.");
                }
                else
                {
                    errorProvider1.SetError(drpDeliveryAddress, "");
                }
            }
        }

        private bool ScanProductsForValidAdresses()
        {
            bool valid = true;
            foreach (XmlNode item in itemDoc.DocumentElement)
            {
                if (item.Attributes[Tags.Type].Value.ToString() == "Stock")
                {
                    if (!CheckAddress(item.Attributes[Tags.DeliveryAddress].Value.ToString()))
                    {
                        valid = false;
                    }
                }
            }
            return valid;
        }

        private XmlNode FindFreeDelItem()
        {
            return itemDoc.SelectSingleNode("//Item[@Code = '" + LoyaltyDropStatic.FreeDelivery + "']");
        }

        private bool ScanProductJoin()
        {
            bool valid = true;


            foreach (XmlNode item in itemDoc.DocumentElement)
            {
                if (item.Attributes[Tags.Type].Value.ToString() == "Stock")
                {
                    if (!CheckAddress(item.Attributes[Tags.DeliveryAddress].Value.ToString()))
                    {
                        valid = false;
                    }
                }
            }

            return valid;
        }

        private bool CheckAddress(string deladdress)
        {
            STL.PL.WS3.LoyaltyValidAddresses Found = ValidCustAddresses.Find(delegate(STL.PL.WS3.LoyaltyValidAddresses Vadd) { return Vadd.addtype == deladdress.Trim(); });
            if (Found != null)
            {
                return true;
            }
            else
            {
                return false;
            }

        }


        public bool CloseNewAccount()
        {
            bool close = false;

            STL.PL.WS3.Loyalty linfo = CustomerManager.LoyaltyGetDatabyacctno(txtAccountNumber.Text.Replace("-", ""));

            if (DelAddressUpdated > 1 && FindFreeDelItem() != null) //Revise & change location of free item. Check if valid.
            {
                close = ValidateItemAddresses(linfo.custid);
                SaveAccount();

            }
            else
            {
                close = true;
            }


            if (Error.Length > 0)
                ShowError(Error);


            return close;

        }

        private bool ValidateItemAddresses(string custid)
        {
            //GetValidAdresses(custid);
            XmlNode HCDEL = FindFreeDelItem();

            if (!ScanProductJoin() && HCDEL != null && Convert.ToDouble(HCDEL.Attributes[Tags.Value].Value) != 0)
            {
                MessageBox.Show("Items are delivered with invalid postcodes!" + Environment.NewLine + "Removing free delivery discount", "Removing delivery discount", MessageBoxButtons.OK);
                HCDEL.Attributes[Tags.Quantity].Value = "0";
                HCDEL.Attributes[Tags.Value].Value = "0";
                populateTable();

                return false;
            }
            else
            {
                return true;
            }
        }

        private void txtProductCode_TextChanged(object sender, EventArgs e)
        {
            this._hasdatachanged = true;
            Function = "txtProductCode_TextChanged";
            errorProvider1.SetError(txtProductCode, "");

            try
            {
                if (txtProductCode.Text.Trim() == "")
                    ItemID = null;

                if (Convert.ToBoolean(Country[CountryParameterNames.LoyaltyScheme]))
                {
                    if (!Redemption)
                    {
                        ClearItemDetails();
                        LoyaltyCheckItem();
                    }
                }
                else
                {
                    ClearItemDetails();
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void LoadVoucher()
        {
            decimal? amount = CustomerManager.LoyaltyGetVoucherValue(_acctNo);

            voucheraddedonrevise = (amount.HasValue && amount < 0);

            if (amount.HasValue && amount < 0 && itemDoc.SelectSingleNode("//Item[@Code = '" + LoyaltyDropStatic.VoucherCode + "' and @Value != 0]") == null)
            {
                currentItem = AccountManager.GetItemDetails(new STL.PL.WS2.GetItemDetailsRequest
                {
                    ProductCode = LoyaltyDropStatic.VoucherCode,
                    StockLocationNo = Convert.ToInt16(drpBranch.SelectedValue),
                    BranchCode = Convert.ToInt16(drpBranch.SelectedValue),
                    AccountType = AT.Cash,
                    CountryCode = Config.CountryCode,
                    IsDutyFree = chxDutyFree.Checked,
                    IsTaxExempt = chxTaxExempt.Checked,
                    PromoBranch = Convert.ToInt16(Config.BranchCode)
                }, out Error);

                currentItem.Attributes[Tags.Value].Value = (Convert.ToDecimal(amount).ToString());
                currentItem.Attributes[Tags.UnitPrice].Value = (Convert.ToDecimal(amount).ToString());
                currentItem.Attributes[Tags.Quantity].Value = "1";
                currentItem.Attributes[Tags.DeliveryDate].Value = ServerTime.Request().ToShortTimeString();
                currentItem.Attributes[Tags.BranchForDeliveryNote].Value = Config.BranchCode;
                currentItem.Attributes[Tags.Location].Value = Config.BranchCode;

                XmlNode tempNode = itemDoc.ImportNode(currentItem, true);
                itemDoc.DocumentElement.AppendChild(tempNode);
            }
        }
    }
}
