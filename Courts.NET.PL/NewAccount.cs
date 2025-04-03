using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using AxSHDocVw;
using Blue.Cosacs.Client;
using Blue.Cosacs.Shared;
using Blue.Cosacs.Shared.Extensions;
using Crownwood.Magic.Menus;
using Microsoft.PointOfService;
using STL.Common;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.Categories;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.Elements;
using STL.Common.Constants.Enums;
using STL.Common.Constants.FTransaction;
using STL.Common.Constants.InstantCredit;
using STL.Common.Constants.ItemTypes;
using STL.Common.Constants.SanctionStages;
using STL.Common.Constants.StoreInfo;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.Tags;
using STL.Common.Constants.TermsTypes;
using STL.Common.Static;
using STL.PL.StoreCard.Common;
using Blue.Cosacs.Shared;
using System.Net;
using System.Web;
using System.IO;
using System.Xml.Serialization;
using STL.Common.Services;
using STL.Common.Services.Model;
using Blue.Cosacs.Shared.CosacsWeb.Models;
using Blue.Cosacs.Shared.Services.Warranty;
using Blue.Cosacs.Shared.Services;

namespace STL.PL
{
    /// <summary>
    /// The New Sales Order screen for creating and revising a customer order.
    /// Different account types can be selected on this screen to create a cash,
    /// HP, Ready Finance or Special account. The screen automatically generates
    /// the appropiate account number for the account type. A main menu option
    /// allows this screen to be invoked to allow manual entry of the account
    /// number in the event that manual account numbers have been used.
    /// Products can be searched for and added as line items to the customer order.
    /// The line items can be arranged hierarchically where one item is the parent of
    /// one or more other items. For example a stock item can be the parent of a
    /// warranty, or a kit product will be the parent of the kit items.
    /// For credit accounts the terms type, deposit and number of instalments
    /// are selected, from which the service charge and agreement total are
    /// automatically calculated.
    /// An account can be revised by adding or removing order items or by changing
    /// their quantities. Related child items can be automatically revised.
    /// This screen is also used as the Cash and Go screen to make use of much
    /// common functionality. For Cash and Go the delivery and credit fields
    /// are hidden and not used.
    /// </summary>
    public partial class NewAccount : CommonForm
    {
        //OMG how much memory is needed just for this from?!?!?!?!?!?!
        List<STL.PL.WS2.BranchDefaultPrintLocation> _printBranches;  //IP - 25/02/10 - CR1072 - Malaysia 3PL for Version 5.2 - Merged from v4.3

        //IP - 25/02/10 - CR1072 - Malaysia 3PL for Version 5.2 - Merged from v4.3
        //CR 953 bool for checking whether default print location exists
        bool _defaultPrintLocation = true;
        public bool DefaultPrintLocation
        {
            get { return _defaultPrintLocation; }
            set { _defaultPrintLocation = value; }
        }

        bool _saveButtonClicked;
        //10.6 CR- Sales Order - Print Save- Allow user to save while printing 
        //and track UI changes to increment invoice version
        bool _isSalesOrderChanged;

        bool _closeSaleWithoutSaving = false;

        //CR903 Include a public property to hold the selected branch's store type
        private string m_storeType = String.Empty;
        public string SType
        {
            get { return m_storeType; }
            set { m_storeType = value; }
        }

        private bool _isLoan;
        public bool IsLoan
        {
            get { return _isLoan; }
            set { _isLoan = value; }
        }

        private bool AddedExistingWarrantyLink { get; set; }
        private bool AskedExistingWarranty { get; set; }

        public AccountTextBox txtAccountNumber;
        public string AccountNo
        {
            get
            {
                if (PaidAndTaken && PTWarrantyAccountNo.Length > 0)
                    return PTWarrantyAccountNo;
                else
                    return txtAccountNumber.UnformattedText;
            }
        }

        public string AccountType
        {
            get { return drpAccountType.Text; }
        }

        public XmlDocument itemDoc;
        public XmlNode LineItems
        {
            get { return itemDoc.DocumentElement; }
        }

        public XmlNode ptReturnItems;
        public string TermsType = "";
        private string _origTerms = "";
        public string ptCustomerID; //IP - 15/04/08 - (69630)
        private bool _userChanged = true;
        private DateTime _today = Date.blankDate;
        private short _contReqWeekDay = -1;
        private string _contDeliveryArea = "-1";
        private bool _loadAccount = false;
        private decimal _deposit = 0;
        private bool IncludeWarranty = true;
        private bool staticLoaded = false;
        private DataTable itemsTable;
        private DataView itemsView;
        //private string _warranty = "";
        private bool _valuecontrolled = false;
        private bool _kit = false;
        private XmlNode currentItem; //xml really?
        private XmlNode affinityItem;
        private string _linktokey = "";
        private decimal dtTaxAmount = 0;
        private decimal _warrantiesOnCreditAT = 0;
        private Control allowTaxExempt = null;
        private Control overrideDiscountLimit = null;
        private decimal _origAgreementTotal = 0;
        private string _currentBand = "";
        private string _scoringBand = "";
        private string _scorecardtype = "";
        private string refinTermsType;
        //private string customerband = "";
        private ErrorProvider errorProviderForWarning;
        private DataView itemsStoreCardView;              //IP - 03/12/10 - Store Card 
        private bool itemValExceeded;                     //IP - 03/12/10 - Store Card
        private Crownwood.Magic.Controls.TabPage docTab;
        private bool associatedItemRightClick = false;    //IP - 25/02/11 - #3230 - Variable to determine if check on associated items was fired from the right-click option
        private bool callRelatedProducts = false;   //IP - 28/02/11 - #3180 - moved from btnEnter_Click
        private bool instalmentWaived = false;

        int insuranceChargeItemId = 0;
        int adminChargeItemId = 0;
        int staxItemId = 0;
        int tier2DiscountItemId = 0;
        bool percentage = false;             // RI - % value entered for discounts
        bool accountWideDiscount = false;    //IP - 31/05/11 - CR1212 - RI - #2315
        bool acctWideDisAuthorised = false;  //IP - 09/06/11 - CR1212 - RI - #3817
        bool acctWideDisRequireAuth = false; //#13791
        bool acctWideDisCancel = false;      //#13971

        bool _linkDiscToAll = false;          //IP - 31/05/11 - CR1212 - RI - #2315
        public bool LinkDiscToAll
        {
            get { return _linkDiscToAll; }
            set { _linkDiscToAll = value; }
        }

        public string ScoringBand //LW 70788 - NM - CR1005 - Service Charge Calculation - HP accts
        {
            get { return _scoringBand; }
        }
        private string _origScoringBand = "";
        private bool _cashAndGoSaved = false; //IP - 15/04/08 - (69630)

        public decimal WarrantiesOnCreditAgreementTotal
        {
            get { return _warrantiesOnCreditAT; }
            set { _warrantiesOnCreditAT = value; }
        }
        private bool _warrantiesOnCredit = false;
        public bool WarrantiesOnCredit
        {
            get { return _warrantiesOnCredit; }
            set { _warrantiesOnCredit = value; }
        }
        private bool _ptWarranties = false;
        public bool ptWarranties
        {
            get { return _ptWarranties; }
            set { _ptWarranties = value; }
        }
        private bool _hasInstallation = false;
        public bool hasInstallation
        {
            get { return _hasInstallation; }
            set { _hasInstallation = value; }
        }
        private string _PTWarrantyAccountNo = "";
        public string PTWarrantyAccountNo
        {
            get { return _PTWarrantyAccountNo; }
            set { _PTWarrantyAccountNo = value; }
        }
        private bool enterRaised = false;
        private System.Windows.Forms.CheckBox chxTaxExempt;
        private System.Windows.Forms.CheckBox chxDutyFree;
        private bool _exchangeChanged = false;
        bool confirm = true;
        public bool Confirm
        {
            get { return confirm; }
            set { confirm = value; }
        }
        private decimal _outstbal = 0;
        public decimal OutstandingBalance
        {
            get { return _outstbal; }
            set { _outstbal = value; }
        }
        private bool _kitclicked = false;
        public bool KitClicked
        {
            get { return _kitclicked; }
            set { _kitclicked = value; }
        }
        private decimal _agreementTotal = 0;
        public decimal AgreementTotal
        {
            get { return Convert.ToDecimal(StripCurrency(txtAgreementTotal.Text)); }
            set
            {
                decimal current = Convert.ToDecimal(StripCurrency(txtAgreementTotal.Text));
                if (value > current &&
                    !supressEvents &&
                    AccountType != AT.Cash && AccountType != AT.Special && AccountType != AT.ReadyFinance && AccountType != AT.GoodsOnLoan && AccountType != AT.HP) //Acct Type Translation DSR 29/9/03
                {
                    menuPrint.Enabled = false;
                    btnPrint.Enabled = false;
                    menuReprint.Enabled = false;
                }
                _agreementTotal = value;
                txtAgreementTotal.Text = value.ToString(DecimalPlaces);

                if (Renewal && renewalsLoaded)
                {
                    menuPrint.Enabled = true;
                    btnPrint.Enabled = true;
                }
            }
        }

        //CR1017 HomeClub
        public decimal AgreementTotalDec
        {
            get { return _agreementTotal; }
        }

        public int VoucherAuthorisedBy = 0;

        private decimal _defaultDeposit = 0;
        private bool _depositIsPercentage = false;
        public string VoucherCompanyAcctNo = "";
        public bool CourtsVoucher = true;
        public string VoucherReference = "";
        public decimal GiftVoucherValue
        {
            get
            {
                if (chxGiftVoucher.Checked)
                    return Convert.ToDecimal(StripCurrency(txtGiftVoucherValue.Text));
                else
                    return 0;
            }
            set
            {
                txtGiftVoucherValue.Text = (value).ToString(DecimalPlaces);
                txtAmount_Validating(null, null);
                this.SetAmountEnabled();
            }
        }

        bool revision = false;
        bool supressEvents = false;
        string _acctNo = "";
        private DataTable dtTermsTypes = null;
        private DataView dvTermsTypes = null;
        private DataTable _areaTable = null;
        public DateTime DateAccountOpened
        {
            get { return _dateOpened; }
            set { _dateOpened = value; }
        }
        private double _schedQtyDel = 0;
        public bool Collection
        {
            get { return _collection; }
            set
            {
                _collection = value;
                //pnBank.Visible = !value;
                lDue.Text = "Refund Amt.";
                txtTendered.Visible = this.lAmountPaid.Visible = lChangeDue.Visible = txtChange.Visible = !value;
                txtAmount.Visible = this.lAmount.Visible = this.btnAddPaymethod.Visible = this.btnRemovePaymethod.Visible = !value;
                gbPaidAndTaken.Text = "Refund";
                txtColourTrim.Visible = !value;
                lColourTrim.Visible = !value;
                menuReprint.Visible = menuPrint.Visible = btnPrint.Visible = !value;
                chxGiftVoucher.Visible = txtGiftVoucherValue.Visible = lGiftVoucher.Visible = !value;
            }
        }

        private bool _replacement = false;
        public bool Replacement
        {
            get { return _replacement; }
            set
            {
                _replacement = value;
                pReplacement.Visible = value;
                txtReplacementValue.Text = (instantReplacement.OrderValue + instantReplacement.TaxAmount).ToString(DecimalPlaces);
                txtReplacementValue.BackColor = SystemColors.Control;
                txtReplacementValue.ForeColor = SystemColors.Highlight;
                //lAmountPaid.Text = value ? GetResource("M_AMOUNTDUE") : lAmountPaid.Text;
            }
        }

        private bool _cancelled = false;
        public bool Cancelled
        {
            get { return _cancelled; }
            set { _cancelled = value; }
        }
        //UAT 267 jec 16/08/07  
        private bool _scoreHPbefore = false;
        public bool ScoreHPbefore
        {
            get { return _scoreHPbefore; }
            set { _scoreHPbefore = value; }
        }

        private bool _rescore = false;
        public bool ReScore
        {
            get { return _rescore; }
            set { _rescore = value; }
        }

        private bool _renwewal = false;
        public bool Renewal
        {
            get { return _renwewal; }
            set { _renwewal = value; }
        }

        private bool _renwewalrevision = false;
        public bool RenewalRevision
        {
            get { return _renwewalrevision; }
            set { _renwewalrevision = value; }
        }

        private bool _purchaseorder = false;
        public bool PurchaseOrder
        {
            get { return _purchaseorder; }
            set { _purchaseorder = value; }
        }
        public bool AlignDates { get; set; }

        private DateTime aligneddeldate = Date.blankDate;
        public DateTime AlignedDelDate
        {
            get { return aligneddeldate; }
            set { aligneddeldate = value; }
        }

        //IP - 17/02/10 - CR1072 - LW 71731 - Cash&Go Fixes from 4.3
        private bool isLegacyPTReturn = false; //LW 71731 - To allow warranties to be added to the lineitem grid
        public bool IsLegacyPTReturn
        {
            set
            {
                isLegacyPTReturn = value;
            }
        }

        private DataTable warrantyRenewals = null;
        private string contractNo = "";
        private DateTimePicker dtRenewalDelDate = new DateTimePicker();
        private bool renewalsLoaded = false;
        private decimal ChargeableSubTotal = 0;
        private decimal ChargeableAdminSubTotal = 0;
        private decimal ChargeableAdminTax = 0;

        public double ScheduledQtyDeleted
        {
            get { return _schedQtyDel; }
            set { _schedQtyDel = value; }
        }

        private bool _acctLocked = false;
        private int _agreementNo = 1;
        public int AgreementNo
        {
            get { return _agreementNo; }
            set { _agreementNo = value; }
        }

        public bool AccountLocked
        {
            get { return _acctLocked; }
            set { _acctLocked = value; }
        }

        public bool SupressEvents
        {
            set { supressEvents = value; }
        }

        public string LinkToKey
        {
            get { return _linktokey; }
            set { _linktokey = value; }
        }

        public string ItemKey { get; set; }

        public string ItemType { get; set; }

        public bool IsKit
        {
            get { return _kit; }
            set { _kit = value; }
        }

        public bool ValueControlled
        {
            get { return _valuecontrolled; }
            set { _valuecontrolled = value; }
        }

        public DataTable ContractNos { get; set; }

        public string Warranty { get; set; }
        public int WarrantyId { get; set; }

        public string ProductCode
        {
            get { return txtProductCode.Text; }
            set { txtProductCode.Text = value; }
        }

        public int? ItemID { get; set; }

        public new string Location
        {
            get { return drpLocation.Text; }
            set
            {
                drpLocation.DataSource = new string[] { value };
                drpLocation.Focus();
            }
        }

        public string UnitPrice
        {
            set { txtUnitPrice.Text = value; }
        }

        public string AvailableStock
        {
            get { return txtAvailable.Text; }
            set { txtAvailable.Text = value; }
        }

        /*public string DamagedStock
		{
			get
			{
				return txtDamaged.Text;
			}
			set
			{
				txtDamaged.Text = value;
			}
		}*/

        private bool _manualSale = false;
        public bool ManualSale
        {
            get { return _manualSale; }
            set
            {
                _manualSale = value;
                if (drpBranch.Enabled)  /* it can disabled but not enabled */
                    drpBranch.Enabled = !value;
            }
        }

        private decimal _rfcredit = 0;
        public decimal RFCreditLimit
        {
            get { return _rfcredit; }
            set { _rfcredit = value; }
        }
        private decimal _rfavailable = 0;
        public decimal RFAvailableCredit
        {
            get { return _rfavailable; }
            set { _rfavailable = value; }
        }

        public decimal RFMax
        {
            get { return Convert.ToDecimal(StripCurrency(txtRFMax.Text)); }
            set { txtRFMax.Text = value.ToString(DecimalPlaces); }
        }
        private DataSet _custAddressSet = null;
        private string _custid = "";
        // 5.1 uat117 rdb 12/11/07 use a Cash And Go bool for closing screen
        private bool _cashAndGo;
        public string CustomerID
        {
            get { return _custid; }
            set
            {
                //uat117
                if (_custid == String.Empty && value == "PAID & TAKEN")
                {
                    _cashAndGo = true;
                }

                _custid = value;
                if (!PaidAndTaken)
                {
                    menuReprint.Enabled = btnPrint.Enabled = menuPrint.Enabled = value.Length != 0 ? true : false;
                }

                //IP - 14/03/08 - (69630) - Once the Paid & Taken account has had a customer
                //linked to it then re-enable the 'Print Receipt' button in the 'Cash & Go' screen,
                //and disable the 'Link to Customer Account' button,'Customer Search' menu option
                //preventing the user on clicking on these as this would incorrectly save the account again.
                if (PaidAndTaken && (ptWarranties || hasInstallation) && this.CustomerID.Length > 0)
                {
                    btnPrintReceipt.Enabled = true;
                    menuPrintReceipt.Enabled = true;
                }

                // The address list must be reloaded for a new cust id
                if (_custid.Length == 0)
                    _custAddressSet = null;
            }
        }
        private bool _acctLoaded = true;
        public bool AccountLoaded
        {
            get { return _acctLoaded; }
            set { _acctLoaded = value; }
        }

        private string _delflag = "N";
        public string DeliveryFlag
        {
            get { return _delflag; }
            set { _delflag = value; }
        }

        //private bool _exchanged = false;
        //public bool Exchanged
        //{
        //    get { return _exchanged; }
        //    set { _exchanged = value; }
        //}

        private bool _printVouchers = false;
        public bool PrintVouchers
        {
            get { return _printVouchers; }
            set { _printVouchers = value; }
        }

        private decimal _cashPrice = 0;
        public decimal CashPrice
        {
            get { return _cashPrice; }
            set { _cashPrice = value; }
        }

        private bool _replacementAdded = false;
        public bool ReplacementAdded
        {
            get { return _replacementAdded; }
            set { _replacementAdded = value; }
        }

        private bool _displaySpiffs = true;
        public bool DisplaySpiffs
        {
            get { return _displaySpiffs; }
            set { _displaySpiffs = value; }
        }

        private bool _paidAndTaken = false;
        private DateTime _dateOpened = Date.blankDate;
        public bool PaidAndTaken
        {
            get { return _paidAndTaken; }
            set
            {
                _paidAndTaken = value;
                menuCustomerSearch.Enabled = !value;
                menuPrint.Enabled = !value;
                menuPaymentCard.Enabled = !value;
                menuPrintReceipt.Enabled = value;
                menuCustomerSearch.Visible = !value;
                menuHelp.Visible = !value;
                btnCustomerSearch.Visible = !value;
                btnPrint.Visible = !value;
                menuReprint.Enabled = !value;
                menuSave.Enabled = btnSave.Visible = !value;

                /* hide delivery fields for PaidAndTaken accounts */
                dtDeliveryRequired.Visible = !value;
                //txtTimeRequired.Visible = !value;
                cbTime.Visible = !value;                 //IP - 25/05/12 - #10225

                drpBranchForDel.Visible = lBranchForDel.Visible = (bool)Country[CountryParameterNames.DisplayDelNoteBranch] && !value;
                drpBranchForDel.Enabled = true;        // #13526

                drpDeliveryArea.Visible = !value;
                cbImmediate.Visible = !value;
                cbAssembly.Visible = !value;
                cbDamaged.Visible = !value;
                cbExpress.Visible = !value;             //IP - 07/06/12 - #10229 - Warehouse & Deliveries
                lblExpress.Visible = !value;            //IP - 14/06/12 - #10379
                drpDeliveryAddress.Visible = !value;
                lDeliveryRequired.Visible = !value;
                lTimeRequired.Visible = !value;

                lDeliveryArea.Visible = !value;
                lImmediate.Visible = !value;
                lDeliveryAddress.Visible = !value;

                drpAccountType.Visible = !value;
                drpSOA.Visible = !value;
                drpLocation.Visible = !value;
                txtColourTrim.Visible = !value;
                lAccountType.Visible = !value;
                lLocation.Visible = !value;
                lColourTrim.Visible = !value;
                drpBranch.Visible = !value;
                lBranch.Visible = !value;
                lDamaged.Visible = !value;
                lAssembly.Visible = !value;
                btnDelAreas.Visible = !value;       // #15074

                menuCashTill.Visible = value;
                menuOpenCashTill.Visible = value;
            }
        }

        //LiveWire 69185 boolean value required to know if a warranty has been selected
        private bool m_warrantySelected = false;
        public bool warrantySelected
        {
            get { return m_warrantySelected; }
            set { m_warrantySelected = value; }
        }

        //CR903 Include a public property to hold the selected branch's  
        // createRFAccount value                        jec
        private bool _createRF = false;
        public bool createRF
        {
            get { return _createRF; }
            set { _createRF = value; }
        }
        //CR903 Include a public property to hold the selected branch's  
        // createHPAccount value                        jec
        private bool _createHP = false;
        public bool createHP
        {
            get { return _createHP; }
            set { _createHP = value; }
        }
        //CR903 Include a public property to hold the selected branch's  
        // createCashAccount value                        jec
        private bool _createCash = false;
        public bool createCash
        {
            get { return _createCash; }
            set { _createCash = value; }
        }

        //LiveWire 69215 property to determine if insurance node should be deleted from xml document
        private bool m_deleteInsuranceNode = true;
        public bool deleteInsuranceNode
        {
            get { return m_deleteInsuranceNode; }
            set { m_deleteInsuranceNode = value; }
        }

        //LiveWire 69215 property to determine if admin node should be deleted from xml document
        private bool m_deleteAdminNode = true;
        public bool deleteAdminNode
        {
            get { return m_deleteAdminNode; }
            set { m_deleteAdminNode = value; }
        }

        public bool CashAndGoReturn //-- uat(5.2)907 //-4.3 merge uat(4.3) - 162(4.3), 172, LW71722
        {
            get;
            set;
        }

        //IP - 25/02/10 - CR1072 - Malaysia 3PL for Version 5.2
        private bool thirdPartyDeliveriesEnabled
        {
            get
            {
                return Convert.ToBoolean(Country[CountryParameterNames.ThirdPartyDeliveriesWarehouse]);
            }
        }

        //IP/JC - 01/03/10 - CR1072 - Malaysia 3PL for Version 5.2
        private bool scheduleDelEnabled
        {
            get
            {
                return Convert.ToBoolean(Country[CountryParameterNames.ScheduledDeliveries]);
            }

        }

        private double creditMinPrice
        {
            get
            {
                return Convert.ToDouble(Country[CountryParameterNames.CreditMinPrice]);      // #17287
            }

        }

        private bool _spaPrint = false;     //UAT1012 jec
        public bool spaPrint
        {
            get { return _spaPrint; }
            set { _spaPrint = value; }
        }

        private bool _printSchedule = false;     //UAT1012 jec
        public bool printSchedule
        {
            get { return _printSchedule; }
            set { _printSchedule = value; }
        }

        //CR907 copied from BasicCustomerDetails   jec 23/08/07
        private struct SanctionStage
        {
            //public string CustID;
            //public string AcctType;
            public string AccountNo;
            public DateTime DateProp;
            //public string ScreenMode;
            //public string CheckType;
            //public string Stage;
            //public int ImageIndex;
        }
        private SanctionStage StageToLaunch;

        private bool _collection = false;
        private StringCollection salesStaff = null;
        private StringCollection accountTypes = null;
        private bool stockExists = false;

        // CR586 Cash and Go can use multiple payment methods
        private DataSet payMethodSet = new DataSet();
        public DateTime datePropRF = Date.blankDate;
        private decimal nonStockTotal = 0;
        private decimal loanTotal = 0;
        private decimal tier2SubTotal = 0;
        public int returnAuthorisedBy = 0;
        private DataSet warrantyRenewalSet = new DataSet();
        private DataSet variableRatesSet = new DataSet();
        private bool _affinityTerms = false;
        private InstantReplacementDetails instantReplacement = null;
        private decimal ChargeableSalesTax = 0;
        private bool _authorisationRequired = false;
        private bool hasPClubDiscount = false;
        private string customerPClubCode = "";
        private string privilegeClubDesc = "";
        DataView dvLinkedSpiffs = null;
        private ArrayList branches = new ArrayList();
        private bool AutoDA = false;

        private bool ManualAccountKeepLock = false;
        public bool custidrequested = false;
        public bool reqTaxInvoicePrint = false;          //IP - 05/03/12 - #9403

        private string _custidCG = "";
        public string custidCG
        {
            get { return _custidCG; }
            set
            {
                if (custidrequested)
                {
                    txt_linkedcustid.Text = value;
                    _custidCG = value;
                    txt_linkedcustid.Visible = true;
                    btnCustomerSearch.Enabled = false;   //IP - 18/02/11 - Sprint 5.11 - #2947
                    menuCustomerSearch.Enabled = false;  //IP - 18/02/11 - Sprint 5.11 - #2947
                    lbl_linkedcust.Visible = true;
                    errorProvider1.SetError(btnCustomerSearch, "");
                    custidrequested = false;
                }

                if (value.Length == 0)
                {
                    _custidCG = value;
                }
            }
        }

        private DataTable LineItemBooking = null;       //IP - 28/05/12 - #9877

        private string referralReasons = string.Empty; //IP - 15/03/11 - #3314 - CR1245
        private string calledFromScreen = string.Empty; //#10535
        private DataTable delAreas = null;  //#12224 - CR12249

        private bool isStaff = false;

        bool isMmiAllowed = false;
        bool isMmiAppliedForSaleAtrr = false;
        private decimal mmiSubTotal;
        private decimal mmiDeposit;
        private decimal mmiDeferredTerms;
        private decimal mmiMonths;
        private decimal cmInsuranceTax = 0;
        private decimal cmAdminTax = 0;
        private string mmiPropResult = string.Empty;


        private decimal saleOrderInsuranceTax = 0;
        private decimal saleOrderAdminTax = 0;
        private decimal saleOrderVoucherValue = 0;
        private XmlNode saleOrderDT;

        public delegate void InvokeDelegate();

        // UAT 509 - allow only selection of one row
        private int mouseX, mouseY;

        public System.Windows.Forms.TextBox txtQuantity;
        public System.Windows.Forms.Button btnAlign;
        public System.Windows.Forms.TextBox txtGiftVoucherValue;
        public System.Windows.Forms.CheckBox chxGiftVoucher;
        public System.Windows.Forms.Button btnPrint;
        public System.Windows.Forms.Button btnCustomerSearch;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private Control allowInstantReplacement = null;
        public Control AllowInstantReplacement      // #15642
        {
            get { return allowInstantReplacement; }
        }
        private Control allowSupaShield = null;
        public Control AllowSupaShield               // #15642
        {
            get { return allowSupaShield; }
        }
        private Control addAdditionalSpiff = null;

        Dictionary<DictionaryKey, WarrantyReturnDetails> warrantyReturnPC = new Dictionary<DictionaryKey, WarrantyReturnDetails>(); //#16607
        List<WarrantyRefund> warrantyRefunds = new List<WarrantyRefund>(); //#16607

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox drpAccountType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox drpSoldBy;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox drpSOA;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtUnitPrice;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtValue;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtAvailable;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtColourTrim;
        private System.Windows.Forms.TextBox txtSubTotal;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox txtDeposit;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.NumericUpDown txtNoMonths;
        private System.Windows.Forms.TextBox txtInstalment;
        private System.Windows.Forms.TextBox txtFinalInstalment;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnEnter;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.ContextMenu contextMenu1;
        private System.Windows.Forms.MenuItem menuItem13;
        private System.Windows.Forms.MenuItem menuItem14;
        private System.Windows.Forms.MenuItem menuItem15;
        private System.Windows.Forms.CheckBox chxCOD;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.ComboBox drpLocation;
        private System.Windows.Forms.ComboBox drpProductDesc;
        private System.Windows.Forms.TextBox txtAgreementTotal;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TextBox txtTerms;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label lblCHOrdInvoice;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.ComboBox drpPaymentMethod;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.ColumnHeader Quantity;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.DataGrid dgLineItems;
        private System.Windows.Forms.TreeView tvItems;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox drpTermsType;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.DateTimePicker dtDateFirst;
        private System.Windows.Forms.Label allowRF;
        private System.Windows.Forms.Control allowGOL;
        private System.Windows.Forms.ComboBox drpLengths;
        private System.Windows.Forms.Label lAuthorise;
        private System.Windows.Forms.Button btnPlan;
        private System.Windows.Forms.Label lImmediate;
        private System.Windows.Forms.CheckBox cbImmediate;
        private System.Windows.Forms.ComboBox drpBranchForDel;
        private System.Windows.Forms.Label lBranchForDel;
        private System.Windows.Forms.ErrorProvider errorProvider2;
        private System.Windows.Forms.Label lTimeRequired;
        private System.Windows.Forms.DateTimePicker dtDeliveryRequired;
        private System.Windows.Forms.Label lDeliveryRequired;
        private System.Windows.Forms.ComboBox drpDeliveryArea;
        private System.Windows.Forms.Label lDeliveryArea;
        private System.Windows.Forms.ComboBox drpDeliveryAddress;
        private System.Windows.Forms.RichTextBox txtProductCode;
        private System.Windows.Forms.Label lblMaxSpend;
        private System.Windows.Forms.TextBox txtRFMax;
        private System.Windows.Forms.TextBox txtSalesTax;
        private System.Windows.Forms.ImageList imageList2;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label lbl_BankAcctNo;
        private System.Windows.Forms.Label lblChequeNo;
        private System.Windows.Forms.Button btnPrintReceipt;
        private System.Windows.Forms.TextBox txtChequeNo;
        private System.Windows.Forms.TextBox txtBankAcctNo;
        private System.Windows.Forms.GroupBox gbPaidAndTaken;
        private System.Windows.Forms.GroupBox gbHP;
        private System.Windows.Forms.Label lChangeDue;
        private System.Windows.Forms.Label lAmountPaid;
        private System.Windows.Forms.Panel pnBank;
        private System.Windows.Forms.ComboBox drpBank;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.ComboBox drpPayMethod;
        private System.Windows.Forms.Label lDeliveryAddress;
        private System.Windows.Forms.Label lColourTrim;
        private System.Windows.Forms.Label lAccountType;
        private System.Windows.Forms.Label lLocation;
        private System.Windows.Forms.Label lblTax;
        private System.Windows.Forms.Label viewProposal;
        private System.Windows.Forms.Label changeTermsType;
        private System.Windows.Forms.Label allowSpecial;
        private System.Windows.Forms.TextBox txtEmpNumber;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel pReplacement;
        private System.Windows.Forms.TextBox txtReplacementValue;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lShowResult;
        private System.Windows.Forms.ComboBox drpBranch;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label lBranch;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lLoyaltyCardNo;
        private System.Windows.Forms.TextBox txtLoyaltyCardNo;
        private System.Windows.Forms.Button btnExchange;
        private System.Windows.Forms.Label lGiftVoucher;
        private System.Windows.Forms.TextBox txtTendered;
        private System.Windows.Forms.TextBox txtDue;
        private System.Windows.Forms.TextBox txtChange;
        private System.Windows.Forms.Label lDue;
        private System.Windows.Forms.Label lInsuranceCharge;
        private System.Windows.Forms.TextBox txtInsuranceCharge;
        private System.Windows.Forms.NumericUpDown numPaymentHolidays;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtAdminCharge;
        private System.Windows.Forms.Label lAmount;
        private System.Windows.Forms.TextBox txtAmount;
        private System.Windows.Forms.Button btnRemovePaymethod;
        private System.Windows.Forms.Button btnAddPaymethod;
        private System.Windows.Forms.Label lSpendLimit;
        private System.Windows.Forms.Label lDueDay;
        private System.Windows.Forms.ComboBox drpDueDay;
        private System.Windows.Forms.CheckBox cbDeposit;
        private Crownwood.Magic.Menus.MenuCommand menuPaymentCard;
        private Crownwood.Magic.Menus.MenuCommand menuAccount;
        private Crownwood.Magic.Menus.MenuCommand menuCustomer;
        private Crownwood.Magic.Menus.MenuCommand menuStock;
        private Crownwood.Magic.Menus.MenuCommand menuPrint;
        private Crownwood.Magic.Menus.MenuCommand menuSave;
        public Crownwood.Magic.Menus.MenuCommand menuCustomerSearch; //IP - 18/03/08 - (69630) Changed from 'private' to 'public' as need to access in 'SearchCashAndGo.cs' and 'OneForOneReplacement.cs'
        private Crownwood.Magic.Menus.MenuCommand menuEnquire;
        private Crownwood.Magic.Menus.MenuCommand menuLocation;
        private Crownwood.Magic.Menus.MenuCommand menuProduct;
        public Crownwood.Magic.Menus.MenuCommand menuPrintReceipt; //IP - 18/03/08 - (69630) Changed from 'private' to 'public' as need to access in 'SearchCashAndGo.cs' and 'OneForOneReplacement.cs'
        private Crownwood.Magic.Menus.MenuCommand menuCancel;
        private Crownwood.Magic.Menus.MenuCommand menuReprint;
        private Crownwood.Magic.Menus.MenuCommand menuCashTill;
        private System.Windows.Forms.Label lAuthoriseCGR;
        private Crownwood.Magic.Menus.MenuCommand menuHelp;
        private Crownwood.Magic.Menus.MenuCommand menuHelpRequested;
        private System.Windows.Forms.CheckBox cbAssembly;
        private System.Windows.Forms.CheckBox cbDamaged;
        private System.Windows.Forms.Label lDamaged;
        private System.Windows.Forms.Label lAssembly;
        private System.Windows.Forms.Label lreviseScheduled;
        private System.Windows.Forms.Label lreviseNonScheduled;
        private System.Windows.Forms.TextBox txtPrivClubDiscount;
        private System.Windows.Forms.Label lPrivilegeClubDesc;
        private Label lAuthoriseDP;
        private Button btnTermsTypeBand;
        private Label allowHP;
        private Button btnLinkedSpiffItems;
        private PictureBox Loyalty_HClogo_pb;
        private Label lbl_linkedcust;
        private TextBox txt_linkedcustid;
        private Label lblDummyForErrorProvider;
        private Label lblStoreCardBal;
        private TextBox txtStoreCardBal;
        private ErrorProvider errorProviderStoreCard;
        private MaskedTextBox mtb_CardNo;
        private Label lblDepositWaiver;
        private Label lblFirstInstalmentWaiver;
        private System.Windows.Forms.Label lAdminCharge;
        private Crownwood.Magic.Menus.MenuCommand menuOpenCashTill;
        private Button btn_CashandGoReturnPay;
        private Button btnStoreCardManualEntry;
        private bool loaded = false;
        private ComboBox cbTime;
        private CheckBox cbExpress;
        private Label lblExpress;
        public Button btnDelAreas;
        private ToolTip toolTipDelArea;
        public StoreCardMagStripeReader MagStripeReader;
        private WarrantyResult warranties = new WarrantyResult();       // #16292
        private DataView dvExchange = null;         //#16828
        private DataView replacementItems = null;   //#17678
        private string collectionType = "";         //#17678
        private bool warrantyRefund = false;        //#16607
        private bool freeReplacementWarrantyAdded = false;   //#18437
        private bool readyAssistExists = false;     //#18604 - CR15594
        private int? readyAssistLength = null;      //#18604 - CR15594
        //private GroupBox groupBox3;
        private Label lblAgrInvoieNum;
        private Label lblOrdInvoice;
        private Label lblNonStockChkBox;
        private CheckBox chxNonStock;
        private ComboBox txtPrefDay;
        private Label lblPrefDay;
        private DataTable readyAssistTerms = null;  //#18604 - CR15594

        public NewAccount(TranslationDummy d)
        {
            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuAccount, menuCustomer, menuStock, menuCashTill, menuHelp });
            loaded = true;
        }

        /// <summary>
        /// Alternative constructor for account revision
        /// </summary>
        /// <param name="accountNumber">account number</param>
        public NewAccount(string accountNumber, int agreementNo, InstantReplacementDetails instant, bool authorisationRequired, Form root, Form parent)
        {
            this._today = StaticDataManager.GetServerDate();
            aligneddeldate = this._today;
            _dateOpened = this._today;
            datePropRF = this._today;

            revision = true;
            supressEvents = true;
            _acctNo = accountNumber;
            AccountLoaded = false;
            InitializeComponent();
            FormRoot = root;
            FormParent = parent;
            AgreementNo = agreementNo;
            _authorisationRequired = authorisationRequired;

            calledFromScreen = parent.Name; //#10535

            //IP - 14/01/11 - Bool was previously not being set.
            if (parent.Name == "SearchCashAndGo")
            {
                CashAndGoReturn = true;
            }

            if (instant != null)
                instantReplacement = instant;

            InitialiseStaticData();

            if (!PaidAndTaken && instant != null)
            {
                string key = instant.ItemId + "|" + instant.StockLocn.ToString();
                string contract = "";

                string xPath = "//Item[@Key = '" + key + "' and @ContractNumber = '" + contract + "' and @Quantity = 0]";
                XmlNode toDelete = itemDoc.DocumentElement.SelectSingleNode(xPath);

                if (toDelete != null)
                {
                    //xml.deleteItem(toDelete, false, itemDoc.DocumentElement);  //#17290
                    toDelete.Attributes[Tags.Description1].Value = "To Be Replaced";
                }
            }

            if (AccountLocked)
            {
                menuSave.Enabled = !PaidAndTaken;
                FormatAccountNo(ref accountNumber);
                txtAccountNumber.Text = accountNumber;
                //CR903 Find the account branch's store type
                SType = FindStoreType(txtAccountNumber.Text);
                //chxTaxExempt.Enabled = false;
                chxDutyFree.Enabled = false;
                populateTable();
                _origAgreementTotal = this.AgreementTotal;
            }
            //TranslateControls();
            SetCtrlToolTip();

            toolTip1.SetToolTip(btnPrintReceipt, GetResource("TT_PRINTRECEIPT"));
            toolTip1.SetToolTip(btnEnter, GetResource("TT_ENTERLINEITEM"));
            toolTip1.SetToolTip(btnRemove, GetResource("TT_REMOVELINEITEM"));
            toolTip1.SetToolTip(btnPrint, GetResource("TT_PRINTAGREEMENT"));
            toolTip1.SetToolTip(btnCustomerSearch, GetResource("TT_LINKCUSTOMER"));
            toolTip1.SetToolTip(btnAddPaymethod, GetResource("TT_MULTIPAYMETHOD"));
            toolTip1.SetToolTip(btnRemovePaymethod, GetResource("TT_CLEARMULTIPAY"));
            toolTip1.SetToolTip(txtRFMax, GetResource("TT_RFMAXSPEND"));
            toolTip1.SetToolTip(lAssembly, GetResource("T_ASSEMBLY"));
            toolTip1.SetToolTip(lDamaged, GetResource("T_DAMAGESTOCK"));
            toolTip1.SetToolTip(mtb_CardNo, GetResource("TT_CARDNO"));          //IP - 26/01/11 - Sprint 5.9 - #2785
            toolTip1.SetToolTip(lblExpress, GetResource("T_EXPRESS"));                            //IP - 08/06/12 - #10336

            toolTipDelArea.SetToolTip(btnDelAreas, GetResource("TT_DELAREA"));                //#14796

            if (!stockExists && drpAccountType.Text == AT.ReadyFinance)
                PrintVouchers = true;

            RenewalRevision = AccountManager.IsWarrantyRenewal(_acctNo, out Error);
            if (Error.Length > 0)
                ShowError(Error);
            else
                btnSave.Enabled = !RenewalRevision;

            if (!PaidAndTaken)
            {
                LoyaltyCheckMemberbyAcctno(accountNumber, parent is BasicCustomerDetails);
            }

            //#17712 #16607 
            if (CashAndGoReturn)
            {
                PopulateWarrantyReturnDictionary();
            }
            this.collectionType = "I"; //#17290
            loaded = true;

            string invoice = "";
            int status = 0;
            AccountManager.DisableSave(AccountNo, out invoice, out status);
            //Mambu
             if (status == 1 )
            {
                btnSave.Enabled = true;
                dgLineItems.Enabled = true;

            }
            /// Second time- Line item for Mambu and Revise account -mambu
            else if (status == 0 )
            {
                btnSave.Enabled = false;
                dgLineItems.Enabled = false;
                txtProductCode.Enabled = false; 
            }
            
        }

        private void SetCtrlToolTip()
        {

        }


        //#17678 - Called from Goods Return when processing Identical Replacement
        public NewAccount(string accountNumber, int agreementNo, string collectionType, DataView replacements, bool authorisationRequired, Form root, Form parent)
        {
            replacementItems = replacements;
            this.collectionType = collectionType;

            this._today = StaticDataManager.GetServerDate();
            aligneddeldate = this._today;
            _dateOpened = this._today;
            datePropRF = this._today;

            revision = true;
            supressEvents = true;
            _acctNo = accountNumber;
            AccountLoaded = false;
            InitializeComponent();
            FormRoot = root;
            FormParent = parent;
            AgreementNo = agreementNo;
            _authorisationRequired = authorisationRequired;

            calledFromScreen = parent.Name; //#10535

            InitialiseStaticData();

            if (AccountLocked)
            {
                menuSave.Enabled = !PaidAndTaken;
                FormatAccountNo(ref accountNumber);
                txtAccountNumber.Text = accountNumber;
                //CR903 Find the account branch's store type
                SType = FindStoreType(txtAccountNumber.Text);
                //chxTaxExempt.Enabled = false;
                chxDutyFree.Enabled = false;
                populateTable();
                _origAgreementTotal = this.AgreementTotal;
            }

            //TranslateControls();
            toolTip1.SetToolTip(btnPrintReceipt, GetResource("TT_PRINTRECEIPT"));
            toolTip1.SetToolTip(btnEnter, GetResource("TT_ENTERLINEITEM"));
            toolTip1.SetToolTip(btnRemove, GetResource("TT_REMOVELINEITEM"));
            toolTip1.SetToolTip(btnPrint, GetResource("TT_PRINTAGREEMENT"));
            toolTip1.SetToolTip(btnCustomerSearch, GetResource("TT_LINKCUSTOMER"));
            toolTip1.SetToolTip(btnAddPaymethod, GetResource("TT_MULTIPAYMETHOD"));
            toolTip1.SetToolTip(btnRemovePaymethod, GetResource("TT_CLEARMULTIPAY"));
            toolTip1.SetToolTip(txtRFMax, GetResource("TT_RFMAXSPEND"));
            toolTip1.SetToolTip(lAssembly, GetResource("T_ASSEMBLY"));
            toolTip1.SetToolTip(lDamaged, GetResource("T_DAMAGESTOCK"));
            toolTip1.SetToolTip(mtb_CardNo, GetResource("TT_CARDNO"));
            toolTip1.SetToolTip(lblExpress, GetResource("T_EXPRESS"));

            toolTipDelArea.SetToolTip(btnDelAreas, GetResource("TT_DELAREA"));

            if (!stockExists && drpAccountType.Text == AT.ReadyFinance)
                PrintVouchers = true;

            RenewalRevision = AccountManager.IsWarrantyRenewal(_acctNo, out Error);
            if (Error.Length > 0)
                ShowError(Error);
            else
                btnSave.Enabled = !RenewalRevision;

            if (!PaidAndTaken)
            {
                LoyaltyCheckMemberbyAcctno(accountNumber, parent is BasicCustomerDetails);
            }

            loaded = true;

            if (replacementItems != null)       //#18608 - CR15594
            {
                foreach (DataRow drv in replacementItems.Table.Rows)
                {
                    string key = Convert.ToInt32(drv["ItemId"]) + "|" + Convert.ToString(drv["StockLocn"]);

                    AttachFreeReplacementWarranty(Convert.ToString(drv["ItemNo"]), Convert.ToString(drv["StockLocn"]), Convert.ToInt32(drv["ItemId"]), Convert.ToInt32(drv["Quantity"]));
                }
            }

            //Need to re-populate the table if free warranties added
            populateTable();

            menuSave_Click(null, null);          // force save after adding Free Warranties
        }





        public NewAccount(bool manualSale, Form root, Form parent)
        {
            InitializeComponent();
            this._today = StaticDataManager.GetServerDate();
            aligneddeldate = this._today;
            _dateOpened = this._today;
            datePropRF = this._today;

            FormRoot = root;
            FormParent = parent;
            ManualSale = manualSale;
            InitialiseStaticData();
            ManualSale = manualSale;
            // #8269 jec 27/09/11 ensure account type selected first
            if (ManualSale)
            {
                txtAccountNumber.Enabled = false;
                drpSOA.Enabled = false;
                drpSoldBy.Enabled = false;
                btnSave.Enabled = false;
                txtEmpNumber.Enabled = false;
            }

            //TranslateControls();
            toolTip1.SetToolTip(btnPrintReceipt, GetResource("TT_PRINTRECEIPT"));
            toolTip1.SetToolTip(btnEnter, GetResource("TT_ENTERLINEITEM"));
            toolTip1.SetToolTip(btnRemove, GetResource("TT_REMOVELINEITEM"));
            toolTip1.SetToolTip(btnPrint, GetResource("TT_PRINTAGREEMENT"));
            toolTip1.SetToolTip(btnCustomerSearch, GetResource("TT_LINKCUSTOMER"));
            toolTip1.SetToolTip(txtRFMax, GetResource("TT_RFMAXSPEND"));
            toolTip1.SetToolTip(lAssembly, GetResource("T_ASSEMBLY"));
            toolTip1.SetToolTip(lDamaged, GetResource("T_DAMAGESTOCK"));
            toolTip1.SetToolTip(lblExpress, GetResource("T_EXPRESS"));                            //IP - 08/06/12 - #10336

            toolTipDelArea.SetToolTip(btnDelAreas, GetResource("TT_DELAREA"));                //#14796

            loaded = true;
        }

        public NewAccount(DataTable renewals, bool manualSale, string custID, bool isRenewal, Form root, Form parent)
        {
            InitializeComponent();
            this._today = StaticDataManager.GetServerDate();
            aligneddeldate = this._today;
            _dateOpened = this._today;
            datePropRF = this._today;

            FormRoot = root;
            FormParent = parent;
            ManualSale = manualSale;
            Renewal = isRenewal;
            InitialiseStaticData();
            ManualSale = manualSale;
            CustomerID = custID;

            warrantyRenewals = new DataTable();
            warrantyRenewals = renewals;

            //TranslateControls();
            toolTip1.SetToolTip(btnPrintReceipt, GetResource("TT_PRINTRECEIPT"));
            toolTip1.SetToolTip(btnEnter, GetResource("TT_ENTERLINEITEM"));
            toolTip1.SetToolTip(btnRemove, GetResource("TT_REMOVELINEITEM"));
            toolTip1.SetToolTip(btnPrint, GetResource("TT_PRINTAGREEMENT"));
            toolTip1.SetToolTip(btnCustomerSearch, GetResource("TT_LINKCUSTOMER"));
            toolTip1.SetToolTip(txtRFMax, GetResource("TT_RFMAXSPEND"));
            toolTip1.SetToolTip(lAssembly, GetResource("T_ASSEMBLY"));
            toolTip1.SetToolTip(lDamaged, GetResource("T_DAMAGESTOCK"));
            toolTip1.SetToolTip(lblExpress, GetResource("T_EXPRESS"));                            //IP - 08/06/12 - #10336

            toolTipDelArea.SetToolTip(btnDelAreas, GetResource("TT_DELAREA"));                //#14796

            loaded = true;
            btnCustomerSearch.Visible = false; //#16237
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewAccount));
            this.allowTaxExempt = new System.Windows.Forms.Control();
            this.overrideDiscountLimit = new System.Windows.Forms.Control();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.menuItem15 = new System.Windows.Forms.MenuItem();
            this.menuItem14 = new System.Windows.Forms.MenuItem();
            this.menuItem13 = new System.Windows.Forms.MenuItem();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnStoreCardManualEntry = new System.Windows.Forms.Button();
            this.imageList2 = new System.Windows.Forms.ImageList(this.components);
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenu1 = new System.Windows.Forms.ContextMenu();
            this.changeTermsType = new System.Windows.Forms.Label();
            this.Quantity = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnAlign = new System.Windows.Forms.Button();
            this.menuAccount = new Crownwood.Magic.Menus.MenuCommand();
            this.menuPrint = new Crownwood.Magic.Menus.MenuCommand();
            this.menuPaymentCard = new Crownwood.Magic.Menus.MenuCommand();
            this.menuPrintReceipt = new Crownwood.Magic.Menus.MenuCommand();
            this.menuCancel = new Crownwood.Magic.Menus.MenuCommand();
            this.menuSave = new Crownwood.Magic.Menus.MenuCommand();
            this.menuReprint = new Crownwood.Magic.Menus.MenuCommand();
            this.menuCustomer = new Crownwood.Magic.Menus.MenuCommand();
            this.menuCustomerSearch = new Crownwood.Magic.Menus.MenuCommand();
            this.menuStock = new Crownwood.Magic.Menus.MenuCommand();
            this.menuEnquire = new Crownwood.Magic.Menus.MenuCommand();
            this.menuLocation = new Crownwood.Magic.Menus.MenuCommand();
            this.menuProduct = new Crownwood.Magic.Menus.MenuCommand();
            this.allowRF = new System.Windows.Forms.Label();
            this.allowGOL = new System.Windows.Forms.Control();
            this.viewProposal = new System.Windows.Forms.Label();
            this.menuCashTill = new Crownwood.Magic.Menus.MenuCommand();
            this.menuOpenCashTill = new Crownwood.Magic.Menus.MenuCommand();
            this.menuHelp = new Crownwood.Magic.Menus.MenuCommand();
            this.menuHelpRequested = new Crownwood.Magic.Menus.MenuCommand();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnDelAreas = new System.Windows.Forms.Button();
            this.cbExpress = new System.Windows.Forms.CheckBox();
            this.lblExpress = new System.Windows.Forms.Label();
            this.cbTime = new System.Windows.Forms.ComboBox();
            this.gbPaidAndTaken = new System.Windows.Forms.GroupBox();
            this.btn_CashandGoReturnPay = new System.Windows.Forms.Button();
            this.lDue = new System.Windows.Forms.Label();
            this.lGiftVoucher = new System.Windows.Forms.Label();
            this.btnRemovePaymethod = new System.Windows.Forms.Button();
            this.btnAddPaymethod = new System.Windows.Forms.Button();
            this.lAmount = new System.Windows.Forms.Label();
            this.txtAmount = new System.Windows.Forms.TextBox();
            this.txtDue = new System.Windows.Forms.TextBox();
            this.lChangeDue = new System.Windows.Forms.Label();
            this.btnExchange = new System.Windows.Forms.Button();
            this.chxGiftVoucher = new System.Windows.Forms.CheckBox();
            this.txtGiftVoucherValue = new System.Windows.Forms.TextBox();
            this.pReplacement = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.txtReplacementValue = new System.Windows.Forms.TextBox();
            this.label34 = new System.Windows.Forms.Label();
            this.drpPayMethod = new System.Windows.Forms.ComboBox();
            this.pnBank = new System.Windows.Forms.Panel();
            this.mtb_CardNo = new System.Windows.Forms.MaskedTextBox();
            this.txtChequeNo = new System.Windows.Forms.TextBox();
            this.allowSpecial = new System.Windows.Forms.Label();
            this.drpBank = new System.Windows.Forms.ComboBox();
            this.txtBankAcctNo = new System.Windows.Forms.TextBox();
            this.label31 = new System.Windows.Forms.Label();
            this.lbl_BankAcctNo = new System.Windows.Forms.Label();
            this.lblChequeNo = new System.Windows.Forms.Label();
            this.btnPrintReceipt = new System.Windows.Forms.Button();
            this.lAmountPaid = new System.Windows.Forms.Label();
            this.txtChange = new System.Windows.Forms.TextBox();
            this.txtTendered = new System.Windows.Forms.TextBox();
            this.txtStoreCardBal = new System.Windows.Forms.TextBox();
            this.lblStoreCardBal = new System.Windows.Forms.Label();
            this.lblDummyForErrorProvider = new System.Windows.Forms.Label();
            this.txtProductCode = new System.Windows.Forms.RichTextBox();
            this.btnLinkedSpiffItems = new System.Windows.Forms.Button();
            this.txtPrivClubDiscount = new System.Windows.Forms.TextBox();
            this.lPrivilegeClubDesc = new System.Windows.Forms.Label();
            this.lDamaged = new System.Windows.Forms.Label();
            this.lAssembly = new System.Windows.Forms.Label();
            this.cbDamaged = new System.Windows.Forms.CheckBox();
            this.cbAssembly = new System.Windows.Forms.CheckBox();
            this.drpDeliveryArea = new System.Windows.Forms.ComboBox();
            this.lDeliveryArea = new System.Windows.Forms.Label();
            this.lTimeRequired = new System.Windows.Forms.Label();
            this.dtDeliveryRequired = new System.Windows.Forms.DateTimePicker();
            this.lDeliveryRequired = new System.Windows.Forms.Label();
            this.drpBranchForDel = new System.Windows.Forms.ComboBox();
            this.lBranchForDel = new System.Windows.Forms.Label();
            this.lLoyaltyCardNo = new System.Windows.Forms.Label();
            this.txtLoyaltyCardNo = new System.Windows.Forms.TextBox();
            this.lblTax = new System.Windows.Forms.Label();
            this.txtSalesTax = new System.Windows.Forms.TextBox();
            this.drpDeliveryAddress = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dgLineItems = new System.Windows.Forms.DataGrid();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.tvItems = new System.Windows.Forms.TreeView();
            this.lAuthoriseCGR = new System.Windows.Forms.Label();
            this.lSpendLimit = new System.Windows.Forms.Label();
            this.lAuthoriseDP = new System.Windows.Forms.Label();
            this.lAuthorise = new System.Windows.Forms.Label();
            this.lreviseScheduled = new System.Windows.Forms.Label();
            this.lreviseNonScheduled = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.lblCHOrdInvoice = new System.Windows.Forms.Label();
            this.drpProductDesc = new System.Windows.Forms.ComboBox();
            this.drpLocation = new System.Windows.Forms.ComboBox();
            this.txtSubTotal = new System.Windows.Forms.TextBox();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnEnter = new System.Windows.Forms.Button();
            this.txtColourTrim = new System.Windows.Forms.TextBox();
            this.lColourTrim = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.txtAvailable = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtUnitPrice = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtQuantity = new System.Windows.Forms.TextBox();
            this.lLocation = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.drpPaymentMethod = new System.Windows.Forms.ComboBox();
            this.cbImmediate = new System.Windows.Forms.CheckBox();
            this.lDeliveryAddress = new System.Windows.Forms.Label();
            this.lImmediate = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblNonStockChkBox = new System.Windows.Forms.Label();
            this.chxNonStock = new System.Windows.Forms.CheckBox();
            this.txt_linkedcustid = new System.Windows.Forms.TextBox();
            this.lbl_linkedcust = new System.Windows.Forms.Label();
            this.Loyalty_HClogo_pb = new System.Windows.Forms.PictureBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.label14 = new System.Windows.Forms.Label();
            this.lBranch = new System.Windows.Forms.Label();
            this.drpBranch = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtEmpNumber = new System.Windows.Forms.TextBox();
            this.btnCustomerSearch = new System.Windows.Forms.Button();
            this.chxDutyFree = new System.Windows.Forms.CheckBox();
            this.chxTaxExempt = new System.Windows.Forms.CheckBox();
            this.txtAccountNumber = new STL.PL.AccountTextBox();
            this.chxCOD = new System.Windows.Forms.CheckBox();
            this.drpSOA = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.drpSoldBy = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lAccountType = new System.Windows.Forms.Label();
            this.drpAccountType = new System.Windows.Forms.ComboBox();
            this.btnPrint = new System.Windows.Forms.Button();
            this.lShowResult = new System.Windows.Forms.Label();
            this.allowHP = new System.Windows.Forms.Label();
            this.gbHP = new System.Windows.Forms.GroupBox();
            this.txtPrefDay = new System.Windows.Forms.ComboBox();
            this.lblPrefDay = new System.Windows.Forms.Label();
            this.lblFirstInstalmentWaiver = new System.Windows.Forms.Label();
            this.lblDepositWaiver = new System.Windows.Forms.Label();
            this.btnTermsTypeBand = new System.Windows.Forms.Button();
            this.btnPlan = new System.Windows.Forms.Button();
            this.cbDeposit = new System.Windows.Forms.CheckBox();
            this.drpDueDay = new System.Windows.Forms.ComboBox();
            this.numPaymentHolidays = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.lAdminCharge = new System.Windows.Forms.Label();
            this.lInsuranceCharge = new System.Windows.Forms.Label();
            this.txtInsuranceCharge = new System.Windows.Forms.TextBox();
            this.txtAdminCharge = new System.Windows.Forms.TextBox();
            this.lblMaxSpend = new System.Windows.Forms.Label();
            this.txtRFMax = new System.Windows.Forms.TextBox();
            this.dtDateFirst = new System.Windows.Forms.DateTimePicker();
            this.label27 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.drpTermsType = new System.Windows.Forms.ComboBox();
            this.label21 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.txtFinalInstalment = new System.Windows.Forms.TextBox();
            this.txtInstalment = new System.Windows.Forms.TextBox();
            this.txtDeposit = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.txtAgreementTotal = new System.Windows.Forms.TextBox();
            this.label25 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.txtTerms = new System.Windows.Forms.TextBox();
            this.txtNoMonths = new System.Windows.Forms.NumericUpDown();
            this.drpLengths = new System.Windows.Forms.ComboBox();
            this.lDueDay = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.errorProviderForWarning = new System.Windows.Forms.ErrorProvider(this.components);
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.errorProvider2 = new System.Windows.Forms.ErrorProvider(this.components);
            this.errorProviderStoreCard = new System.Windows.Forms.ErrorProvider(this.components);
            this.toolTipDelArea = new System.Windows.Forms.ToolTip(this.components);
            this.lblAgrInvoieNum = new System.Windows.Forms.Label();
            this.lblOrdInvoice = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            this.gbPaidAndTaken.SuspendLayout();
            this.pReplacement.SuspendLayout();
            this.pnBank.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgLineItems)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Loyalty_HClogo_pb)).BeginInit();
            this.gbHP.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPaymentHolidays)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNoMonths)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderForWarning)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderStoreCard)).BeginInit();
            this.SuspendLayout();
            // 
            // allowTaxExempt
            // 
            this.allowTaxExempt.Enabled = false;
            this.allowTaxExempt.Location = new System.Drawing.Point(0, 0);
            this.allowTaxExempt.Name = "allowTaxExempt";
            this.allowTaxExempt.Size = new System.Drawing.Size(0, 0);
            this.allowTaxExempt.TabIndex = 0;
            this.allowTaxExempt.Visible = false;
            // 
            // overrideDiscountLimit
            // 
            this.overrideDiscountLimit.Enabled = false;
            this.overrideDiscountLimit.Location = new System.Drawing.Point(0, 0);
            this.overrideDiscountLimit.Name = "overrideDiscountLimit";
            this.overrideDiscountLimit.Size = new System.Drawing.Size(0, 0);
            this.overrideDiscountLimit.TabIndex = 0;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "");
            this.imageList1.Images.SetKeyName(1, "");
            this.imageList1.Images.SetKeyName(2, "");
            this.imageList1.Images.SetKeyName(3, "");
            this.imageList1.Images.SetKeyName(4, "");
            this.imageList1.Images.SetKeyName(5, "");
            this.imageList1.Images.SetKeyName(6, "");
            this.imageList1.Images.SetKeyName(7, "");
            this.imageList1.Images.SetKeyName(8, "");
            this.imageList1.Images.SetKeyName(9, "dollar.jpg");
            this.imageList1.Images.SetKeyName(10, "ExpDelivery.jpg");
            // 
            // menuItem15
            // 
            this.menuItem15.Index = 1;
            this.menuItem15.Text = "Product";
            this.menuItem15.Click += new System.EventHandler(this.menuItem15_Click);
            // 
            // menuItem14
            // 
            this.menuItem14.Index = 0;
            this.menuItem14.Text = "Location";
            this.menuItem14.Click += new System.EventHandler(this.menuItem14_Click);
            // 
            // menuItem13
            // 
            this.menuItem13.Index = 0;
            this.menuItem13.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem14,
            this.menuItem15});
            this.menuItem13.Text = "Enquire By";
            // 
            // btnStoreCardManualEntry
            // 
            this.btnStoreCardManualEntry.Image = ((System.Drawing.Image)(resources.GetObject("btnStoreCardManualEntry.Image")));
            this.btnStoreCardManualEntry.Location = new System.Drawing.Point(490, 21);
            this.btnStoreCardManualEntry.Name = "btnStoreCardManualEntry";
            this.btnStoreCardManualEntry.Size = new System.Drawing.Size(22, 22);
            this.btnStoreCardManualEntry.TabIndex = 93;
            this.toolTip1.SetToolTip(this.btnStoreCardManualEntry, "Keyboard Entry of Store Card Account");
            this.btnStoreCardManualEntry.Visible = false;
            this.btnStoreCardManualEntry.Click += new System.EventHandler(this.btnStoreCard_Click);
            // 
            // imageList2
            // 
            this.imageList2.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList2.ImageStream")));
            this.imageList2.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList2.Images.SetKeyName(0, "");
            this.imageList2.Images.SetKeyName(1, "");
            this.imageList2.Images.SetKeyName(2, "");
            this.imageList2.Images.SetKeyName(3, "");
            this.imageList2.Images.SetKeyName(4, "");
            this.imageList2.Images.SetKeyName(5, "");
            // 
            // contextMenu1
            // 
            this.contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem13});
            // 
            // changeTermsType
            // 
            this.changeTermsType.Enabled = false;
            this.changeTermsType.Location = new System.Drawing.Point(0, 0);
            this.changeTermsType.Name = "changeTermsType";
            this.changeTermsType.Size = new System.Drawing.Size(100, 23);
            this.changeTermsType.TabIndex = 0;
            this.changeTermsType.Visible = false;
            // 
            // Quantity
            // 
            this.Quantity.Text = "Quantity";
            // 
            // btnAlign
            // 
            this.btnAlign.Enabled = false;
            this.errorProvider1.SetIconAlignment(this.btnAlign, System.Windows.Forms.ErrorIconAlignment.TopLeft);
            this.btnAlign.Image = ((System.Drawing.Image)(resources.GetObject("btnAlign.Image")));
            this.btnAlign.Location = new System.Drawing.Point(123, 76);
            this.btnAlign.Name = "btnAlign";
            this.btnAlign.Size = new System.Drawing.Size(24, 24);
            this.btnAlign.TabIndex = 41;
            // 
            // menuAccount
            // 
            this.menuAccount.Description = "MenuItem";
            this.menuAccount.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuPrint,
            this.menuPaymentCard,
            this.menuPrintReceipt,
            this.menuCancel,
            this.menuSave,
            this.menuReprint});
            this.menuAccount.Text = "&Account";
            // 
            // menuPrint
            // 
            this.menuPrint.Description = "MenuItem";
            this.menuPrint.Enabled = false;
            this.menuPrint.ImageIndex = 1;
            this.menuPrint.ImageList = this.imageList2;
            this.menuPrint.Text = "&Print Agr/Warranty";
            this.menuPrint.Click += new System.EventHandler(this.menuPrint_Click);
            // 
            // menuPaymentCard
            // 
            this.menuPaymentCard.Description = "MenuItem";
            this.menuPaymentCard.Enabled = false;
            this.menuPaymentCard.Text = "P&rint Payment Card";
            this.menuPaymentCard.Click += new System.EventHandler(this.menuPaymentCard_Click);
            // 
            // menuPrintReceipt
            // 
            this.menuPrintReceipt.Description = "MenuItem";
            this.menuPrintReceipt.Enabled = false;
            this.menuPrintReceipt.Text = "Print &Tax Invoice";
            this.menuPrintReceipt.Click += new System.EventHandler(this.menuPrintReceipt_Click);
            // 
            // menuCancel
            // 
            this.menuCancel.Description = "MenuItem";
            this.menuCancel.Text = "&Cancel Sale";
            this.menuCancel.Click += new System.EventHandler(this.menuCancel_Click);
            // 
            // menuSave
            // 
            this.menuSave.Description = "MenuItem";
            this.menuSave.ImageIndex = 0;
            this.menuSave.ImageList = this.imageList2;
            this.menuSave.Text = "&Save";
            this.menuSave.Click += new System.EventHandler(this.menuSave_Click);
            // 
            // menuReprint
            // 
            this.menuReprint.Description = "MenuItem";
            this.menuReprint.Enabled = false;
            this.menuReprint.Text = "Re-print Agreement Documents";
            this.menuReprint.Click += new System.EventHandler(this.menuReprint_Click);
            // 
            // menuCustomer
            // 
            this.menuCustomer.Description = "MenuItem";
            this.menuCustomer.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuCustomerSearch});
            this.menuCustomer.Text = "&Customer";
            // 
            // menuCustomerSearch
            // 
            this.menuCustomerSearch.Description = "MenuItem";
            this.menuCustomerSearch.ImageIndex = 2;
            this.menuCustomerSearch.ImageList = this.imageList2;
            this.menuCustomerSearch.Text = "Customer &Details";
            this.menuCustomerSearch.Click += new System.EventHandler(this.menuCustomerSearch_Click);
            // 
            // menuStock
            // 
            this.menuStock.Description = "MenuItem";
            this.menuStock.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuEnquire});
            this.menuStock.Text = "&Stock";
            // 
            // menuEnquire
            // 
            this.menuEnquire.Description = "MenuItem";
            this.menuEnquire.ImageIndex = 4;
            this.menuEnquire.ImageList = this.imageList2;
            this.menuEnquire.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuLocation,
            this.menuProduct});
            this.menuEnquire.Text = "Enquire by...";
            // 
            // menuLocation
            // 
            this.menuLocation.Description = "MenuItem";
            this.menuLocation.ImageIndex = 3;
            this.menuLocation.ImageList = this.imageList2;
            this.menuLocation.Text = "Location";
            this.menuLocation.Click += new System.EventHandler(this.menuItem14_Click);
            // 
            // menuProduct
            // 
            this.menuProduct.Description = "MenuItem";
            this.menuProduct.Text = "Product";
            this.menuProduct.Click += new System.EventHandler(this.menuItem15_Click);
            // 
            // allowRF
            // 
            this.allowRF.Enabled = false;
            this.allowRF.Location = new System.Drawing.Point(0, 0);
            this.allowRF.Name = "allowRF";
            this.allowRF.Size = new System.Drawing.Size(100, 23);
            this.allowRF.TabIndex = 0;
            this.allowRF.Visible = false;
            // 
            // allowGOL
            // 
            this.allowGOL.Enabled = false;
            this.allowGOL.Location = new System.Drawing.Point(0, 0);
            this.allowGOL.Name = "allowGOL";
            this.allowGOL.Size = new System.Drawing.Size(0, 0);
            this.allowGOL.TabIndex = 0;
            this.allowGOL.Visible = false;
            // 
            // viewProposal
            // 
            this.viewProposal.Enabled = false;
            this.viewProposal.Location = new System.Drawing.Point(0, 0);
            this.viewProposal.Name = "viewProposal";
            this.viewProposal.Size = new System.Drawing.Size(100, 23);
            this.viewProposal.TabIndex = 0;
            // 
            // menuCashTill
            // 
            this.menuCashTill.Description = "MenuItem";
            this.menuCashTill.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuOpenCashTill});
            this.menuCashTill.Text = "Cash Till";
            // 
            // menuOpenCashTill
            // 
            this.menuOpenCashTill.Description = "MenuItem";
            this.menuOpenCashTill.Text = "Open Cash Till";
            this.menuOpenCashTill.Click += new System.EventHandler(this.menuOpenCashTill_Click);
            // 
            // menuHelp
            // 
            this.menuHelp.Description = "MenuItem";
            this.menuHelp.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuHelpRequested});
            this.menuHelp.Text = "&Help";
            this.menuHelp.Click += new System.EventHandler(this.menuHelp_Click);
            // 
            // menuHelpRequested
            // 
            this.menuHelpRequested.Description = "MenuItem";
            this.menuHelpRequested.Text = "&About this Screen";
            this.menuHelpRequested.Click += new System.EventHandler(this.menuHelpRequested_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox2.Controls.Add(this.btnDelAreas);
            this.groupBox2.Controls.Add(this.cbExpress);
            this.groupBox2.Controls.Add(this.lblExpress);
            this.groupBox2.Controls.Add(this.cbTime);
            this.groupBox2.Controls.Add(this.gbPaidAndTaken);
            this.groupBox2.Controls.Add(this.txtStoreCardBal);
            this.groupBox2.Controls.Add(this.lblStoreCardBal);
            this.groupBox2.Controls.Add(this.lblDummyForErrorProvider);
            this.groupBox2.Controls.Add(this.txtProductCode);
            this.groupBox2.Controls.Add(this.btnLinkedSpiffItems);
            this.groupBox2.Controls.Add(this.txtPrivClubDiscount);
            this.groupBox2.Controls.Add(this.lPrivilegeClubDesc);
            this.groupBox2.Controls.Add(this.lDamaged);
            this.groupBox2.Controls.Add(this.lAssembly);
            this.groupBox2.Controls.Add(this.cbDamaged);
            this.groupBox2.Controls.Add(this.cbAssembly);
            this.groupBox2.Controls.Add(this.drpDeliveryArea);
            this.groupBox2.Controls.Add(this.lDeliveryArea);
            this.groupBox2.Controls.Add(this.lTimeRequired);
            this.groupBox2.Controls.Add(this.dtDeliveryRequired);
            this.groupBox2.Controls.Add(this.lDeliveryRequired);
            this.groupBox2.Controls.Add(this.drpBranchForDel);
            this.groupBox2.Controls.Add(this.lBranchForDel);
            this.groupBox2.Controls.Add(this.lLoyaltyCardNo);
            this.groupBox2.Controls.Add(this.txtLoyaltyCardNo);
            this.groupBox2.Controls.Add(this.lblTax);
            this.groupBox2.Controls.Add(this.txtSalesTax);
            this.groupBox2.Controls.Add(this.drpDeliveryAddress);
            this.groupBox2.Controls.Add(this.panel1);
            this.groupBox2.Controls.Add(this.label26);
            this.groupBox2.Controls.Add(this.lblCHOrdInvoice);
            this.groupBox2.Controls.Add(this.drpProductDesc);
            this.groupBox2.Controls.Add(this.drpLocation);
            this.groupBox2.Controls.Add(this.txtSubTotal);
            this.groupBox2.Controls.Add(this.btnRemove);
            this.groupBox2.Controls.Add(this.btnEnter);
            this.groupBox2.Controls.Add(this.txtColourTrim);
            this.groupBox2.Controls.Add(this.lColourTrim);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.txtAvailable);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.txtValue);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.txtUnitPrice);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.txtQuantity);
            this.groupBox2.Controls.Add(this.lLocation);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label17);
            this.groupBox2.Controls.Add(this.drpPaymentMethod);
            this.groupBox2.Controls.Add(this.cbImmediate);
            this.groupBox2.Controls.Add(this.lDeliveryAddress);
            this.groupBox2.Controls.Add(this.lImmediate);
            this.groupBox2.Enabled = false;
            this.groupBox2.Location = new System.Drawing.Point(8, 72);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(776, 291);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Product Information";
            // 
            // btnDelAreas
            // 
            this.btnDelAreas.BackColor = System.Drawing.SystemColors.HotTrack;
            this.btnDelAreas.Image = ((System.Drawing.Image)(resources.GetObject("btnDelAreas.Image")));
            this.btnDelAreas.Location = new System.Drawing.Point(267, 80);
            this.btnDelAreas.Name = "btnDelAreas";
            this.btnDelAreas.Size = new System.Drawing.Size(20, 18);
            this.btnDelAreas.TabIndex = 94;
            this.btnDelAreas.UseVisualStyleBackColor = false;
            this.btnDelAreas.Click += new System.EventHandler(this.btnDelAreas_Click);
            // 
            // cbExpress
            // 
            this.cbExpress.Location = new System.Drawing.Point(720, 34);
            this.cbExpress.Name = "cbExpress";
            this.cbExpress.Size = new System.Drawing.Size(16, 16);
            this.cbExpress.TabIndex = 93;
            // 
            // lblExpress
            // 
            this.lblExpress.Image = global::STL.PL.Properties.Resources.ExpDelivery;
            this.lblExpress.Location = new System.Drawing.Point(716, 16);
            this.lblExpress.Name = "lblExpress";
            this.lblExpress.Size = new System.Drawing.Size(24, 16);
            this.lblExpress.TabIndex = 92;
            this.lblExpress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cbTime
            // 
            this.cbTime.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTime.FormattingEnabled = true;
            this.cbTime.Items.AddRange(new object[] {
            "AM",
            "PM"});
            this.cbTime.Location = new System.Drawing.Point(426, 81);
            this.cbTime.Name = "cbTime";
            this.cbTime.Size = new System.Drawing.Size(42, 21);
            this.cbTime.TabIndex = 91;
            this.cbTime.SelectedIndexChanged += new System.EventHandler(this.cbTime_SelectedIndexChanged);
            // 
            // gbPaidAndTaken
            // 
            this.gbPaidAndTaken.BackColor = System.Drawing.SystemColors.Control;
            this.gbPaidAndTaken.Controls.Add(this.btn_CashandGoReturnPay);
            this.gbPaidAndTaken.Controls.Add(this.lDue);
            this.gbPaidAndTaken.Controls.Add(this.lGiftVoucher);
            this.gbPaidAndTaken.Controls.Add(this.btnRemovePaymethod);
            this.gbPaidAndTaken.Controls.Add(this.btnAddPaymethod);
            this.gbPaidAndTaken.Controls.Add(this.lAmount);
            this.gbPaidAndTaken.Controls.Add(this.txtAmount);
            this.gbPaidAndTaken.Controls.Add(this.txtDue);
            this.gbPaidAndTaken.Controls.Add(this.lChangeDue);
            this.gbPaidAndTaken.Controls.Add(this.btnExchange);
            this.gbPaidAndTaken.Controls.Add(this.chxGiftVoucher);
            this.gbPaidAndTaken.Controls.Add(this.txtGiftVoucherValue);
            this.gbPaidAndTaken.Controls.Add(this.pReplacement);
            this.gbPaidAndTaken.Controls.Add(this.label34);
            this.gbPaidAndTaken.Controls.Add(this.drpPayMethod);
            this.gbPaidAndTaken.Controls.Add(this.pnBank);
            this.gbPaidAndTaken.Controls.Add(this.btnPrintReceipt);
            this.gbPaidAndTaken.Controls.Add(this.lAmountPaid);
            this.gbPaidAndTaken.Controls.Add(this.txtChange);
            this.gbPaidAndTaken.Controls.Add(this.txtTendered);
            this.gbPaidAndTaken.Location = new System.Drawing.Point(0, 290);
            this.gbPaidAndTaken.Name = "gbPaidAndTaken";
            this.gbPaidAndTaken.Size = new System.Drawing.Size(776, 112);
            this.gbPaidAndTaken.TabIndex = 3;
            this.gbPaidAndTaken.TabStop = false;
            this.gbPaidAndTaken.Text = "Payment";
            // 
            // btn_CashandGoReturnPay
            // 
            this.btn_CashandGoReturnPay.Image = global::STL.PL.Properties.Resources.Icon_Dollar_Sign_Blue_sm;
            this.btn_CashandGoReturnPay.Location = new System.Drawing.Point(191, 32);
            this.btn_CashandGoReturnPay.Name = "btn_CashandGoReturnPay";
            this.btn_CashandGoReturnPay.Size = new System.Drawing.Size(23, 23);
            this.btn_CashandGoReturnPay.TabIndex = 75;
            this.btn_CashandGoReturnPay.UseVisualStyleBackColor = true;
            this.btn_CashandGoReturnPay.Visible = false;
            this.btn_CashandGoReturnPay.Click += new System.EventHandler(this.btn_CashandGoReturnPay_Click);
            // 
            // lDue
            // 
            this.lDue.Location = new System.Drawing.Point(552, 16);
            this.lDue.Name = "lDue";
            this.lDue.Size = new System.Drawing.Size(96, 16);
            this.lDue.TabIndex = 70;
            this.lDue.Text = "Due";
            this.lDue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lGiftVoucher
            // 
            this.lGiftVoucher.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.lGiftVoucher.Location = new System.Drawing.Point(244, 16);
            this.lGiftVoucher.Name = "lGiftVoucher";
            this.lGiftVoucher.Size = new System.Drawing.Size(100, 16);
            this.lGiftVoucher.TabIndex = 36;
            this.lGiftVoucher.Text = "Gift Voucher";
            this.lGiftVoucher.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnRemovePaymethod
            // 
            this.btnRemovePaymethod.BackColor = System.Drawing.Color.SlateBlue;
            this.btnRemovePaymethod.Enabled = false;
            this.btnRemovePaymethod.Font = new System.Drawing.Font("Arial Narrow", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRemovePaymethod.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.btnRemovePaymethod.Image = ((System.Drawing.Image)(resources.GetObject("btnRemovePaymethod.Image")));
            this.btnRemovePaymethod.Location = new System.Drawing.Point(744, 38);
            this.btnRemovePaymethod.Name = "btnRemovePaymethod";
            this.btnRemovePaymethod.Size = new System.Drawing.Size(20, 21);
            this.btnRemovePaymethod.TabIndex = 74;
            this.btnRemovePaymethod.TabStop = false;
            this.btnRemovePaymethod.UseVisualStyleBackColor = false;
            this.btnRemovePaymethod.Click += new System.EventHandler(this.btnRemovePaymethod_Click);
            // 
            // btnAddPaymethod
            // 
            this.btnAddPaymethod.BackColor = System.Drawing.Color.SlateBlue;
            this.btnAddPaymethod.Font = new System.Drawing.Font("Arial Narrow", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddPaymethod.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.btnAddPaymethod.Image = ((System.Drawing.Image)(resources.GetObject("btnAddPaymethod.Image")));
            this.btnAddPaymethod.Location = new System.Drawing.Point(744, 16);
            this.btnAddPaymethod.Name = "btnAddPaymethod";
            this.btnAddPaymethod.Size = new System.Drawing.Size(20, 22);
            this.btnAddPaymethod.TabIndex = 73;
            this.btnAddPaymethod.TabStop = false;
            this.btnAddPaymethod.UseVisualStyleBackColor = false;
            this.btnAddPaymethod.Click += new System.EventHandler(this.btnAddPaymethod_Click);
            // 
            // lAmount
            // 
            this.lAmount.Location = new System.Drawing.Point(584, 40);
            this.lAmount.Name = "lAmount";
            this.lAmount.Size = new System.Drawing.Size(64, 16);
            this.lAmount.TabIndex = 72;
            this.lAmount.Text = "Amount";
            this.lAmount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtAmount
            // 
            this.txtAmount.Location = new System.Drawing.Point(656, 40);
            this.txtAmount.Name = "txtAmount";
            this.txtAmount.Size = new System.Drawing.Size(80, 20);
            this.txtAmount.TabIndex = 20;
            this.txtAmount.Text = "0";
            this.txtAmount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtAmount.TextChanged += new System.EventHandler(this.txtAmount_TextChanged);
            this.txtAmount.Validating += new System.ComponentModel.CancelEventHandler(this.txtAmount_Validating);
            // 
            // txtDue
            // 
            this.txtDue.Location = new System.Drawing.Point(656, 16);
            this.txtDue.Name = "txtDue";
            this.txtDue.ReadOnly = true;
            this.txtDue.Size = new System.Drawing.Size(80, 20);
            this.txtDue.TabIndex = 69;
            this.txtDue.TabStop = false;
            this.txtDue.Text = "0";
            this.txtDue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lChangeDue
            // 
            this.lChangeDue.Location = new System.Drawing.Point(592, 88);
            this.lChangeDue.Name = "lChangeDue";
            this.lChangeDue.Size = new System.Drawing.Size(56, 16);
            this.lChangeDue.TabIndex = 9;
            this.lChangeDue.Text = "Change";
            this.lChangeDue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnExchange
            // 
            this.btnExchange.Image = ((System.Drawing.Image)(resources.GetObject("btnExchange.Image")));
            this.btnExchange.Location = new System.Drawing.Point(170, 32);
            this.btnExchange.Name = "btnExchange";
            this.btnExchange.Size = new System.Drawing.Size(20, 24);
            this.btnExchange.TabIndex = 68;
            this.btnExchange.Visible = false;
            this.btnExchange.Click += new System.EventHandler(this.btnExchange_Click);
            // 
            // chxGiftVoucher
            // 
            this.chxGiftVoucher.Location = new System.Drawing.Point(220, 32);
            this.chxGiftVoucher.Name = "chxGiftVoucher";
            this.chxGiftVoucher.Size = new System.Drawing.Size(16, 24);
            this.chxGiftVoucher.TabIndex = 30;
            this.chxGiftVoucher.TabStop = false;
            this.chxGiftVoucher.CheckedChanged += new System.EventHandler(this.rbGiftVoucher_CheckedChanged);
            // 
            // txtGiftVoucherValue
            // 
            this.txtGiftVoucherValue.Location = new System.Drawing.Point(244, 32);
            this.txtGiftVoucherValue.Name = "txtGiftVoucherValue";
            this.txtGiftVoucherValue.ReadOnly = true;
            this.txtGiftVoucherValue.Size = new System.Drawing.Size(88, 20);
            this.txtGiftVoucherValue.TabIndex = 40;
            this.txtGiftVoucherValue.Text = "0";
            this.txtGiftVoucherValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // pReplacement
            // 
            this.pReplacement.Controls.Add(this.label5);
            this.pReplacement.Controls.Add(this.txtReplacementValue);
            this.pReplacement.Location = new System.Drawing.Point(392, 8);
            this.pReplacement.Name = "pReplacement";
            this.pReplacement.Size = new System.Drawing.Size(138, 48);
            this.pReplacement.TabIndex = 34;
            this.pReplacement.Visible = false;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(3, 5);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(112, 16);
            this.label5.TabIndex = 1;
            this.label5.Text = "Replacement Value";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtReplacementValue
            // 
            this.txtReplacementValue.Location = new System.Drawing.Point(3, 25);
            this.txtReplacementValue.Name = "txtReplacementValue";
            this.txtReplacementValue.ReadOnly = true;
            this.txtReplacementValue.Size = new System.Drawing.Size(100, 20);
            this.txtReplacementValue.TabIndex = 0;
            this.txtReplacementValue.TabStop = false;
            this.txtReplacementValue.Text = "0";
            this.txtReplacementValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label34
            // 
            this.label34.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label34.Location = new System.Drawing.Point(20, 16);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(96, 16);
            this.label34.TabIndex = 26;
            this.label34.Text = "Payment Method";
            this.label34.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // drpPayMethod
            // 
            this.drpPayMethod.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.drpPayMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpPayMethod.DropDownWidth = 80;
            this.drpPayMethod.ItemHeight = 13;
            this.drpPayMethod.Location = new System.Drawing.Point(26, 32);
            this.drpPayMethod.Name = "drpPayMethod";
            this.drpPayMethod.Size = new System.Drawing.Size(144, 21);
            this.drpPayMethod.TabIndex = 10;
            this.drpPayMethod.SelectedIndexChanged += new System.EventHandler(this.drpPayMethod_SelectedIndexChanged);
            // 
            // pnBank
            // 
            this.pnBank.Controls.Add(this.btnStoreCardManualEntry);
            this.pnBank.Controls.Add(this.mtb_CardNo);
            this.pnBank.Controls.Add(this.txtChequeNo);
            this.pnBank.Controls.Add(this.allowSpecial);
            this.pnBank.Controls.Add(this.drpBank);
            this.pnBank.Controls.Add(this.txtBankAcctNo);
            this.pnBank.Controls.Add(this.label31);
            this.pnBank.Controls.Add(this.lbl_BankAcctNo);
            this.pnBank.Controls.Add(this.lblChequeNo);
            this.pnBank.Location = new System.Drawing.Point(24, 56);
            this.pnBank.Name = "pnBank";
            this.pnBank.Size = new System.Drawing.Size(564, 48);
            this.pnBank.TabIndex = 24;
            // 
            // mtb_CardNo
            // 
            this.mtb_CardNo.Location = new System.Drawing.Point(351, 23);
            this.mtb_CardNo.Mask = "XXXX-XXXX-XXXX-0000";
            this.mtb_CardNo.Name = "mtb_CardNo";
            this.mtb_CardNo.Size = new System.Drawing.Size(137, 20);
            this.mtb_CardNo.TabIndex = 92;
            this.mtb_CardNo.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
            this.mtb_CardNo.Visible = false;
            this.mtb_CardNo.TextChanged += new System.EventHandler(this.mtb_CardNo_TextChanged);
            // 
            // txtChequeNo
            // 
            this.txtChequeNo.Location = new System.Drawing.Point(350, 23);
            this.txtChequeNo.MaxLength = 14;
            this.txtChequeNo.Name = "txtChequeNo";
            this.txtChequeNo.Size = new System.Drawing.Size(136, 20);
            this.txtChequeNo.TabIndex = 3;
            this.txtChequeNo.Visible = false;
            // 
            // allowSpecial
            // 
            this.allowSpecial.Enabled = false;
            this.allowSpecial.Location = new System.Drawing.Point(85, 4);
            this.allowSpecial.Name = "allowSpecial";
            this.allowSpecial.Size = new System.Drawing.Size(38, 17);
            this.allowSpecial.TabIndex = 36;
            this.allowSpecial.Text = "label1";
            this.allowSpecial.Visible = false;
            // 
            // drpBank
            // 
            this.drpBank.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpBank.Location = new System.Drawing.Point(2, 24);
            this.drpBank.Name = "drpBank";
            this.drpBank.Size = new System.Drawing.Size(168, 21);
            this.drpBank.TabIndex = 1;
            // 
            // txtBankAcctNo
            // 
            this.txtBankAcctNo.Location = new System.Drawing.Point(188, 24);
            this.txtBankAcctNo.MaxLength = 20;
            this.txtBankAcctNo.Name = "txtBankAcctNo";
            this.txtBankAcctNo.Size = new System.Drawing.Size(136, 20);
            this.txtBankAcctNo.TabIndex = 2;
            // 
            // label31
            // 
            this.label31.Location = new System.Drawing.Point(2, 8);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(100, 16);
            this.label31.TabIndex = 5;
            this.label31.Text = "Bank Name";
            this.label31.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbl_BankAcctNo
            // 
            this.lbl_BankAcctNo.Location = new System.Drawing.Point(188, 8);
            this.lbl_BankAcctNo.Name = "lbl_BankAcctNo";
            this.lbl_BankAcctNo.Size = new System.Drawing.Size(100, 16);
            this.lbl_BankAcctNo.TabIndex = 6;
            this.lbl_BankAcctNo.Text = "Bank Account No";
            this.lbl_BankAcctNo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblChequeNo
            // 
            this.lblChequeNo.Location = new System.Drawing.Point(348, 8);
            this.lblChequeNo.Name = "lblChequeNo";
            this.lblChequeNo.Size = new System.Drawing.Size(100, 16);
            this.lblChequeNo.TabIndex = 7;
            this.lblChequeNo.Text = "Cheque / Card No";
            this.lblChequeNo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnPrintReceipt
            // 
            this.btnPrintReceipt.ImageIndex = 1;
            this.btnPrintReceipt.ImageList = this.imageList2;
            this.btnPrintReceipt.Location = new System.Drawing.Point(744, 72);
            this.btnPrintReceipt.Name = "btnPrintReceipt";
            this.btnPrintReceipt.Size = new System.Drawing.Size(24, 23);
            this.btnPrintReceipt.TabIndex = 30;
            this.btnPrintReceipt.Click += new System.EventHandler(this.menuPrintReceipt_Click);
            // 
            // lAmountPaid
            // 
            this.lAmountPaid.Location = new System.Drawing.Point(584, 64);
            this.lAmountPaid.Name = "lAmountPaid";
            this.lAmountPaid.Size = new System.Drawing.Size(64, 16);
            this.lAmountPaid.TabIndex = 8;
            this.lAmountPaid.Text = "Tendered";
            this.lAmountPaid.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtChange
            // 
            this.txtChange.Location = new System.Drawing.Point(656, 88);
            this.txtChange.Name = "txtChange";
            this.txtChange.ReadOnly = true;
            this.txtChange.Size = new System.Drawing.Size(80, 20);
            this.txtChange.TabIndex = 30;
            this.txtChange.TabStop = false;
            this.txtChange.Text = "0";
            this.txtChange.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtTendered
            // 
            this.txtTendered.Location = new System.Drawing.Point(656, 64);
            this.txtTendered.Name = "txtTendered";
            this.txtTendered.ReadOnly = true;
            this.txtTendered.Size = new System.Drawing.Size(80, 20);
            this.txtTendered.TabIndex = 4;
            this.txtTendered.TabStop = false;
            this.txtTendered.Text = "0";
            this.txtTendered.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtStoreCardBal
            // 
            this.txtStoreCardBal.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.txtStoreCardBal.Location = new System.Drawing.Point(395, 263);
            this.txtStoreCardBal.Name = "txtStoreCardBal";
            this.txtStoreCardBal.ReadOnly = true;
            this.txtStoreCardBal.Size = new System.Drawing.Size(87, 20);
            this.txtStoreCardBal.TabIndex = 2;
            this.txtStoreCardBal.Text = "0";
            this.txtStoreCardBal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblStoreCardBal
            // 
            this.lblStoreCardBal.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.lblStoreCardBal.AutoSize = true;
            this.lblStoreCardBal.Location = new System.Drawing.Point(392, 250);
            this.lblStoreCardBal.Name = "lblStoreCardBal";
            this.lblStoreCardBal.Size = new System.Drawing.Size(103, 13);
            this.lblStoreCardBal.TabIndex = 3;
            this.lblStoreCardBal.Text = "Store Card Available";
            // 
            // lblDummyForErrorProvider
            // 
            this.lblDummyForErrorProvider.Location = new System.Drawing.Point(579, 16);
            this.lblDummyForErrorProvider.Name = "lblDummyForErrorProvider";
            this.lblDummyForErrorProvider.Size = new System.Drawing.Size(31, 11);
            this.lblDummyForErrorProvider.TabIndex = 90;
            this.lblDummyForErrorProvider.Text = "Warning";
            // 
            // txtProductCode
            // 
            this.txtProductCode.Location = new System.Drawing.Point(6, 30);
            this.txtProductCode.MaxLength = 16;
            this.txtProductCode.Multiline = false;
            this.txtProductCode.Name = "txtProductCode";
            this.txtProductCode.Size = new System.Drawing.Size(106, 20);
            this.txtProductCode.TabIndex = 0;
            this.txtProductCode.Text = "";
            this.txtProductCode.TextChanged += new System.EventHandler(this.txtProductCode_TextChanged);
            this.txtProductCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtProductCode_KeyDown);
            this.txtProductCode.Leave += new System.EventHandler(this.txtProductCode_Leave);
            this.txtProductCode.MouseUp += new System.Windows.Forms.MouseEventHandler(this.txtProductCode_MouseUp);
            this.txtProductCode.Validating += new System.ComponentModel.CancelEventHandler(this.txtProductCode_Validating);
            // 
            // btnLinkedSpiffItems
            // 
            this.btnLinkedSpiffItems.ImageIndex = 9;
            this.btnLinkedSpiffItems.ImageList = this.imageList1;
            this.btnLinkedSpiffItems.Location = new System.Drawing.Point(744, 26);
            this.btnLinkedSpiffItems.Name = "btnLinkedSpiffItems";
            this.btnLinkedSpiffItems.Size = new System.Drawing.Size(22, 22);
            this.btnLinkedSpiffItems.TabIndex = 62;
            this.btnLinkedSpiffItems.Visible = false;
            this.btnLinkedSpiffItems.Click += new System.EventHandler(this.btnLinkedSpiffItems_Click);
            // 
            // txtPrivClubDiscount
            // 
            this.txtPrivClubDiscount.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.txtPrivClubDiscount.Location = new System.Drawing.Point(296, 263);
            this.txtPrivClubDiscount.Name = "txtPrivClubDiscount";
            this.txtPrivClubDiscount.ReadOnly = true;
            this.txtPrivClubDiscount.Size = new System.Drawing.Size(88, 20);
            this.txtPrivClubDiscount.TabIndex = 60;
            this.txtPrivClubDiscount.TabStop = false;
            this.txtPrivClubDiscount.Text = "0";
            this.txtPrivClubDiscount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtPrivClubDiscount.Visible = false;
            // 
            // lPrivilegeClubDesc
            // 
            this.lPrivilegeClubDesc.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.lPrivilegeClubDesc.Location = new System.Drawing.Point(296, 247);
            this.lPrivilegeClubDesc.Name = "lPrivilegeClubDesc";
            this.lPrivilegeClubDesc.Size = new System.Drawing.Size(104, 16);
            this.lPrivilegeClubDesc.TabIndex = 61;
            this.lPrivilegeClubDesc.Text = "Priv Club Discount";
            this.lPrivilegeClubDesc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lPrivilegeClubDesc.Visible = false;
            // 
            // lDamaged
            // 
            this.lDamaged.ImageIndex = 6;
            this.lDamaged.ImageList = this.imageList1;
            this.lDamaged.Location = new System.Drawing.Point(666, 14);
            this.lDamaged.Name = "lDamaged";
            this.lDamaged.Size = new System.Drawing.Size(24, 16);
            this.lDamaged.TabIndex = 57;
            this.lDamaged.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lAssembly
            // 
            this.lAssembly.ImageIndex = 4;
            this.lAssembly.ImageList = this.imageList1;
            this.lAssembly.Location = new System.Drawing.Point(690, 14);
            this.lAssembly.Name = "lAssembly";
            this.lAssembly.Size = new System.Drawing.Size(24, 16);
            this.lAssembly.TabIndex = 56;
            this.lAssembly.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cbDamaged
            // 
            this.cbDamaged.Location = new System.Drawing.Point(674, 34);
            this.cbDamaged.Name = "cbDamaged";
            this.cbDamaged.Size = new System.Drawing.Size(16, 16);
            this.cbDamaged.TabIndex = 55;
            // 
            // cbAssembly
            // 
            this.cbAssembly.Location = new System.Drawing.Point(698, 34);
            this.cbAssembly.Name = "cbAssembly";
            this.cbAssembly.Size = new System.Drawing.Size(16, 16);
            this.cbAssembly.TabIndex = 54;
            // 
            // drpDeliveryArea
            // 
            this.drpDeliveryArea.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpDeliveryArea.DropDownWidth = 48;
            this.drpDeliveryArea.Location = new System.Drawing.Point(184, 80);
            this.drpDeliveryArea.Name = "drpDeliveryArea";
            this.drpDeliveryArea.Size = new System.Drawing.Size(77, 21);
            this.drpDeliveryArea.TabIndex = 45;
            this.drpDeliveryArea.SelectedIndexChanged += new System.EventHandler(this.drpDeliveryArea_SelectedIndexChanged);
            // 
            // lDeliveryArea
            // 
            this.lDeliveryArea.Location = new System.Drawing.Point(187, 64);
            this.lDeliveryArea.Name = "lDeliveryArea";
            this.lDeliveryArea.Size = new System.Drawing.Size(72, 16);
            this.lDeliveryArea.TabIndex = 53;
            this.lDeliveryArea.Text = "Delivery Area";
            this.lDeliveryArea.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lTimeRequired
            // 
            this.lTimeRequired.Location = new System.Drawing.Point(422, 64);
            this.lTimeRequired.Name = "lTimeRequired";
            this.lTimeRequired.Size = new System.Drawing.Size(32, 16);
            this.lTimeRequired.TabIndex = 51;
            this.lTimeRequired.Text = "Time";
            this.lTimeRequired.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dtDeliveryRequired
            // 
            this.dtDeliveryRequired.CustomFormat = "ddd dd MMM yyyy";
            this.dtDeliveryRequired.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtDeliveryRequired.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.dtDeliveryRequired.Location = new System.Drawing.Point(293, 80);
            this.dtDeliveryRequired.Name = "dtDeliveryRequired";
            this.dtDeliveryRequired.Size = new System.Drawing.Size(127, 20);
            this.dtDeliveryRequired.TabIndex = 46;
            this.dtDeliveryRequired.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.dtDeliveryRequired.Leave += new System.EventHandler(this.dtDeliveryRequired_Leave);
            // 
            // lDeliveryRequired
            // 
            this.lDeliveryRequired.Location = new System.Drawing.Point(290, 64);
            this.lDeliveryRequired.Name = "lDeliveryRequired";
            this.lDeliveryRequired.Size = new System.Drawing.Size(112, 16);
            this.lDeliveryRequired.TabIndex = 49;
            this.lDeliveryRequired.Text = "Delivery Required";
            this.lDeliveryRequired.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // drpBranchForDel
            // 
            this.drpBranchForDel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpBranchForDel.DropDownWidth = 48;
            this.drpBranchForDel.Location = new System.Drawing.Point(8, 82);
            this.drpBranchForDel.Name = "drpBranchForDel";
            this.drpBranchForDel.Size = new System.Drawing.Size(56, 21);
            this.drpBranchForDel.TabIndex = 44;
            this.drpBranchForDel.SelectedIndexChanged += new System.EventHandler(this.drpBranchForDel_SelectedIndexChanged);
            // 
            // lBranchForDel
            // 
            this.lBranchForDel.Location = new System.Drawing.Point(8, 53);
            this.lBranchForDel.Name = "lBranchForDel";
            this.lBranchForDel.Size = new System.Drawing.Size(56, 27);
            this.lBranchForDel.TabIndex = 45;
            this.lBranchForDel.Text = "Delivery Location";
            this.lBranchForDel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lLoyaltyCardNo
            // 
            this.lLoyaltyCardNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lLoyaltyCardNo.Location = new System.Drawing.Point(172, 244);
            this.lLoyaltyCardNo.Name = "lLoyaltyCardNo";
            this.lLoyaltyCardNo.Size = new System.Drawing.Size(122, 16);
            this.lLoyaltyCardNo.TabIndex = 33;
            this.lLoyaltyCardNo.Text = "Loyalty Card No";
            this.lLoyaltyCardNo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lLoyaltyCardNo.Visible = false;
            // 
            // txtLoyaltyCardNo
            // 
            this.txtLoyaltyCardNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.txtLoyaltyCardNo.Location = new System.Drawing.Point(168, 263);
            this.txtLoyaltyCardNo.MaxLength = 16;
            this.txtLoyaltyCardNo.Name = "txtLoyaltyCardNo";
            this.txtLoyaltyCardNo.Size = new System.Drawing.Size(117, 20);
            this.txtLoyaltyCardNo.TabIndex = 34;
            this.txtLoyaltyCardNo.Tag = "lMoreRewards1";
            this.txtLoyaltyCardNo.Visible = false;
            // 
            // lblTax
            // 
            this.lblTax.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.lblTax.Location = new System.Drawing.Point(484, 265);
            this.lblTax.Name = "lblTax";
            this.lblTax.Size = new System.Drawing.Size(32, 16);
            this.lblTax.TabIndex = 32;
            this.lblTax.Text = "Tax";
            this.lblTax.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtSalesTax
            // 
            this.txtSalesTax.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.txtSalesTax.Location = new System.Drawing.Point(520, 263);
            this.txtSalesTax.Name = "txtSalesTax";
            this.txtSalesTax.ReadOnly = true;
            this.txtSalesTax.Size = new System.Drawing.Size(88, 20);
            this.txtSalesTax.TabIndex = 16;
            this.txtSalesTax.TabStop = false;
            this.txtSalesTax.Text = "0";
            this.txtSalesTax.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // drpDeliveryAddress
            // 
            this.drpDeliveryAddress.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpDeliveryAddress.DropDownWidth = 48;
            this.drpDeliveryAddress.Items.AddRange(new object[] {
            "H ",
            "W ",
            "D ",
            "D1",
            "D2",
            "D3"});
            this.drpDeliveryAddress.Location = new System.Drawing.Point(124, 80);
            this.drpDeliveryAddress.Name = "drpDeliveryAddress";
            this.drpDeliveryAddress.Size = new System.Drawing.Size(48, 21);
            this.drpDeliveryAddress.TabIndex = 11;
            this.drpDeliveryAddress.SelectedIndexChanged += new System.EventHandler(this.drpDeliveryAddress_SelectedIndexChanged);
            this.drpDeliveryAddress.SelectionChangeCommitted += new System.EventHandler(this.drpDeliveryAddress_SelectionChangeCommitted);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.dgLineItems);
            this.panel1.Controls.Add(this.splitter1);
            this.panel1.Controls.Add(this.tvItems);
            this.panel1.Controls.Add(this.lAuthoriseCGR);
            this.panel1.Controls.Add(this.lSpendLimit);
            this.panel1.Controls.Add(this.lAuthoriseDP);
            this.panel1.Controls.Add(this.lAuthorise);
            this.panel1.Controls.Add(this.lreviseScheduled);
            this.panel1.Controls.Add(this.lreviseNonScheduled);
            this.panel1.Location = new System.Drawing.Point(8, 104);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(760, 136);
            this.panel1.TabIndex = 27;
            // 
            // dgLineItems
            // 
            this.dgLineItems.DataMember = "";
            this.dgLineItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgLineItems.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgLineItems.Location = new System.Drawing.Point(129, 0);
            this.dgLineItems.Name = "dgLineItems";
            this.dgLineItems.ReadOnly = true;
            this.dgLineItems.Size = new System.Drawing.Size(631, 136);
            this.dgLineItems.TabIndex = 2;
            this.dgLineItems.TabStop = false;
            this.dgLineItems.CurrentCellChanged += new System.EventHandler(this.dgLineItems_CurrentCellChanged);
            this.dgLineItems.Click += new System.EventHandler(this.dgLineItems_Click);
            this.dgLineItems.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dgLineItems_MouseMove);
            this.dgLineItems.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgLineItems_MouseUp);
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(126, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 136);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // tvItems
            // 
            this.tvItems.Dock = System.Windows.Forms.DockStyle.Left;
            this.tvItems.ImageIndex = 0;
            this.tvItems.ImageList = this.imageList1;
            this.tvItems.Indent = 19;
            this.tvItems.Location = new System.Drawing.Point(0, 0);
            this.tvItems.Name = "tvItems";
            this.tvItems.SelectedImageIndex = 0;
            this.tvItems.Size = new System.Drawing.Size(126, 136);
            this.tvItems.TabIndex = 0;
            this.tvItems.TabStop = false;
            this.tvItems.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvItems_AfterSelect);
            this.tvItems.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tvItems_MouseDown);
            // 
            // lAuthoriseCGR
            // 
            this.lAuthoriseCGR.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lAuthoriseCGR.Enabled = false;
            this.lAuthoriseCGR.Location = new System.Drawing.Point(658, 81);
            this.lAuthoriseCGR.Name = "lAuthoriseCGR";
            this.lAuthoriseCGR.Size = new System.Drawing.Size(10, 10);
            this.lAuthoriseCGR.TabIndex = 47;
            this.lAuthoriseCGR.Visible = false;
            // 
            // lSpendLimit
            // 
            this.lSpendLimit.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.lSpendLimit.Enabled = false;
            this.lSpendLimit.Location = new System.Drawing.Point(312, 83);
            this.lSpendLimit.Name = "lSpendLimit";
            this.lSpendLimit.Size = new System.Drawing.Size(8, 8);
            this.lSpendLimit.TabIndex = 40;
            this.lSpendLimit.Visible = false;
            // 
            // lAuthoriseDP
            // 
            this.lAuthoriseDP.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lAuthoriseDP.Enabled = false;
            this.lAuthoriseDP.Location = new System.Drawing.Point(636, 81);
            this.lAuthoriseDP.Name = "lAuthoriseDP";
            this.lAuthoriseDP.Size = new System.Drawing.Size(10, 10);
            this.lAuthoriseDP.TabIndex = 48;
            this.lAuthoriseDP.Visible = false;
            // 
            // lAuthorise
            // 
            this.lAuthorise.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.lAuthorise.Enabled = false;
            this.lAuthorise.Location = new System.Drawing.Point(335, 83);
            this.lAuthorise.Name = "lAuthorise";
            this.lAuthorise.Size = new System.Drawing.Size(8, 8);
            this.lAuthorise.TabIndex = 41;
            this.lAuthorise.Visible = false;
            // 
            // lreviseScheduled
            // 
            this.lreviseScheduled.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.lreviseScheduled.Enabled = false;
            this.lreviseScheduled.Location = new System.Drawing.Point(354, 83);
            this.lreviseScheduled.Name = "lreviseScheduled";
            this.lreviseScheduled.Size = new System.Drawing.Size(8, 8);
            this.lreviseScheduled.TabIndex = 58;
            this.lreviseScheduled.Visible = false;
            // 
            // lreviseNonScheduled
            // 
            this.lreviseNonScheduled.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.lreviseNonScheduled.Enabled = false;
            this.lreviseNonScheduled.Location = new System.Drawing.Point(370, 83);
            this.lreviseNonScheduled.Name = "lreviseNonScheduled";
            this.lreviseNonScheduled.Size = new System.Drawing.Size(8, 8);
            this.lreviseNonScheduled.TabIndex = 59;
            this.lreviseNonScheduled.Visible = false;
            // 
            // label26
            // 
            this.label26.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label26.Location = new System.Drawing.Point(620, 265);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(56, 16);
            this.label26.TabIndex = 26;
            this.label26.Text = "Sub total";
            this.label26.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblCHOrdInvoice
            // 
            this.lblCHOrdInvoice.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.lblCHOrdInvoice.Location = new System.Drawing.Point(250, 265);
            this.lblCHOrdInvoice.Name = "lblCHOrdInvoice";
            this.lblCHOrdInvoice.Size = new System.Drawing.Size(200, 16);
            this.lblCHOrdInvoice.TabIndex = 26;
            this.lblCHOrdInvoice.Text = "Ord/Invoice:";
            this.lblCHOrdInvoice.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // drpProductDesc
            // 
            this.drpProductDesc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpProductDesc.DropDownWidth = 184;
            this.drpProductDesc.Location = new System.Drawing.Point(254, 30);
            this.drpProductDesc.Name = "drpProductDesc";
            this.drpProductDesc.Size = new System.Drawing.Size(210, 21);
            this.drpProductDesc.TabIndex = 3;
            // 
            // drpLocation
            // 
            this.drpLocation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpLocation.DropDownWidth = 48;
            this.drpLocation.Location = new System.Drawing.Point(133, 30);
            this.drpLocation.Name = "drpLocation";
            this.drpLocation.Size = new System.Drawing.Size(48, 21);
            this.drpLocation.TabIndex = 1;
            this.drpLocation.Validating += new System.ComponentModel.CancelEventHandler(this.drpLocation_Validating);
            // 
            // txtSubTotal
            // 
            this.txtSubTotal.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.txtSubTotal.Location = new System.Drawing.Point(680, 263);
            this.txtSubTotal.Name = "txtSubTotal";
            this.txtSubTotal.ReadOnly = true;
            this.txtSubTotal.Size = new System.Drawing.Size(88, 20);
            this.txtSubTotal.TabIndex = 17;
            this.txtSubTotal.TabStop = false;
            this.txtSubTotal.Text = "0";
            this.txtSubTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnRemove
            // 
            this.btnRemove.BackColor = System.Drawing.Color.SlateBlue;
            this.btnRemove.Enabled = false;
            this.btnRemove.Font = new System.Drawing.Font("Arial Narrow", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRemove.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.btnRemove.Image = ((System.Drawing.Image)(resources.GetObject("btnRemove.Image")));
            this.btnRemove.Location = new System.Drawing.Point(742, 81);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(22, 22);
            this.btnRemove.TabIndex = 14;
            this.btnRemove.UseVisualStyleBackColor = false;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnEnter
            // 
            this.btnEnter.BackColor = System.Drawing.Color.SlateBlue;
            this.btnEnter.Enabled = false;
            this.btnEnter.Font = new System.Drawing.Font("Arial Narrow", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEnter.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.btnEnter.Image = ((System.Drawing.Image)(resources.GetObject("btnEnter.Image")));
            this.btnEnter.Location = new System.Drawing.Point(742, 59);
            this.btnEnter.Name = "btnEnter";
            this.btnEnter.Size = new System.Drawing.Size(22, 22);
            this.btnEnter.TabIndex = 13;
            this.btnEnter.UseVisualStyleBackColor = false;
            this.btnEnter.Click += new System.EventHandler(this.btnEnter_Click);
            // 
            // txtColourTrim
            // 
            this.txtColourTrim.Location = new System.Drawing.Point(541, 67);
            this.txtColourTrim.Multiline = true;
            this.txtColourTrim.Name = "txtColourTrim";
            this.txtColourTrim.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtColourTrim.Size = new System.Drawing.Size(196, 32);
            this.txtColourTrim.TabIndex = 12;
            // 
            // lColourTrim
            // 
            this.lColourTrim.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lColourTrim.Location = new System.Drawing.Point(474, 68);
            this.lColourTrim.Name = "lColourTrim";
            this.lColourTrim.Size = new System.Drawing.Size(66, 32);
            this.lColourTrim.TabIndex = 20;
            this.lColourTrim.Text = "Delivery Instructions";
            this.lColourTrim.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(616, 14);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(56, 16);
            this.label12.TabIndex = 14;
            this.label12.Text = "Available";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtAvailable
            // 
            this.txtAvailable.Location = new System.Drawing.Point(616, 30);
            this.txtAvailable.Name = "txtAvailable";
            this.txtAvailable.ReadOnly = true;
            this.txtAvailable.Size = new System.Drawing.Size(46, 20);
            this.txtAvailable.TabIndex = 6;
            this.txtAvailable.TabStop = false;
            this.txtAvailable.Text = "0";
            this.txtAvailable.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(535, 14);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(40, 16);
            this.label11.TabIndex = 12;
            this.label11.Text = "Value";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtValue
            // 
            this.txtValue.Location = new System.Drawing.Point(537, 30);
            this.txtValue.Name = "txtValue";
            this.txtValue.ReadOnly = true;
            this.txtValue.Size = new System.Drawing.Size(76, 20);
            this.txtValue.TabIndex = 5;
            this.txtValue.TabStop = false;
            this.txtValue.Text = "0";
            this.txtValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtValue.Validating += new System.ComponentModel.CancelEventHandler(this.txtValue_Validating);
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(464, 14);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(56, 16);
            this.label10.TabIndex = 10;
            this.label10.Text = "Unit Price";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtUnitPrice
            // 
            this.txtUnitPrice.Location = new System.Drawing.Point(467, 30);
            this.txtUnitPrice.Name = "txtUnitPrice";
            this.txtUnitPrice.ReadOnly = true;
            this.txtUnitPrice.Size = new System.Drawing.Size(69, 20);
            this.txtUnitPrice.TabIndex = 4;
            this.txtUnitPrice.TabStop = false;
            this.txtUnitPrice.Text = "0";
            this.txtUnitPrice.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(254, 14);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(128, 16);
            this.label9.TabIndex = 8;
            this.label9.Text = "Product Description";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(204, 14);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(48, 16);
            this.label8.TabIndex = 6;
            this.label8.Text = "Quantity";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtQuantity
            // 
            this.txtQuantity.Location = new System.Drawing.Point(209, 30);
            this.txtQuantity.Name = "txtQuantity";
            this.txtQuantity.Size = new System.Drawing.Size(37, 20);
            this.txtQuantity.TabIndex = 2;
            this.txtQuantity.Text = "0";
            this.txtQuantity.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtQuantity.Validating += new System.ComponentModel.CancelEventHandler(this.txtQuantity_Validating);
            // 
            // lLocation
            // 
            this.lLocation.Location = new System.Drawing.Point(118, 14);
            this.lLocation.Name = "lLocation";
            this.lLocation.Size = new System.Drawing.Size(85, 16);
            this.lLocation.TabIndex = 4;
            this.lLocation.Text = "Stock Location";
            this.lLocation.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(4, 14);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(100, 16);
            this.label6.TabIndex = 2;
            this.label6.Text = "Product Code";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label17
            // 
            this.label17.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label17.Location = new System.Drawing.Point(16, 247);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(96, 16);
            this.label17.TabIndex = 24;
            this.label17.Text = "Payment Method";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // drpPaymentMethod
            // 
            this.drpPaymentMethod.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.drpPaymentMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpPaymentMethod.DropDownWidth = 80;
            this.drpPaymentMethod.ItemHeight = 13;
            this.drpPaymentMethod.Location = new System.Drawing.Point(16, 263);
            this.drpPaymentMethod.Name = "drpPaymentMethod";
            this.drpPaymentMethod.Size = new System.Drawing.Size(144, 21);
            this.drpPaymentMethod.TabIndex = 15;
            this.drpPaymentMethod.SelectedIndexChanged += new System.EventHandler(this.drpPaymentMethod_SelectedIndexChanged);
            // 
            // cbImmediate
            // 
            this.cbImmediate.Location = new System.Drawing.Point(83, 80);
            this.cbImmediate.Name = "cbImmediate";
            this.cbImmediate.Size = new System.Drawing.Size(24, 16);
            this.cbImmediate.TabIndex = 43;
            this.cbImmediate.CheckedChanged += new System.EventHandler(this.cbImmediate_CheckedChanged);
            // 
            // lDeliveryAddress
            // 
            this.lDeliveryAddress.Location = new System.Drawing.Point(119, 64);
            this.lDeliveryAddress.Name = "lDeliveryAddress";
            this.lDeliveryAddress.Size = new System.Drawing.Size(72, 16);
            this.lDeliveryAddress.TabIndex = 30;
            this.lDeliveryAddress.Text = "Del Address";
            this.lDeliveryAddress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lImmediate
            // 
            this.lImmediate.Location = new System.Drawing.Point(60, 64);
            this.lImmediate.Name = "lImmediate";
            this.lImmediate.Size = new System.Drawing.Size(64, 13);
            this.lImmediate.TabIndex = 42;
            this.lImmediate.Text = "Immediate";
            this.lImmediate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.lblNonStockChkBox);
            this.groupBox1.Controls.Add(this.chxNonStock);
            this.groupBox1.Controls.Add(this.txt_linkedcustid);
            this.groupBox1.Controls.Add(this.lbl_linkedcust);
            this.groupBox1.Controls.Add(this.Loyalty_HClogo_pb);
            this.groupBox1.Controls.Add(this.btnSave);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.lBranch);
            this.groupBox1.Controls.Add(this.drpBranch);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtEmpNumber);
            this.groupBox1.Controls.Add(this.btnCustomerSearch);
            this.groupBox1.Controls.Add(this.chxDutyFree);
            this.groupBox1.Controls.Add(this.chxTaxExempt);
            this.groupBox1.Controls.Add(this.txtAccountNumber);
            this.groupBox1.Controls.Add(this.chxCOD);
            this.groupBox1.Controls.Add(this.drpSOA);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.drpSoldBy);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.lAccountType);
            this.groupBox1.Controls.Add(this.drpAccountType);
            this.groupBox1.Controls.Add(this.btnPrint);
            this.groupBox1.Location = new System.Drawing.Point(8, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(776, 72);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Account Information";
            // 
            // lblNonStockChkBox
            // 
            this.lblNonStockChkBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNonStockChkBox.Location = new System.Drawing.Point(146, 27);
            this.lblNonStockChkBox.Name = "lblNonStockChkBox";
            this.lblNonStockChkBox.Size = new System.Drawing.Size(32, 16);
            this.lblNonStockChkBox.TabIndex = 59;
            this.lblNonStockChkBox.Text = "NS";
            this.lblNonStockChkBox.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chxNonStock
            // 
            this.chxNonStock.CheckAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.chxNonStock.Location = new System.Drawing.Point(140, 46);
            this.chxNonStock.Name = "chxNonStock";
            this.chxNonStock.Size = new System.Drawing.Size(32, 16);
            this.chxNonStock.TabIndex = 11;
            this.chxNonStock.CheckedChanged += new System.EventHandler(this.chxNonStock_CheckedChanged);
            // 
            // txt_linkedcustid
            // 
            this.txt_linkedcustid.Enabled = false;
            this.txt_linkedcustid.Location = new System.Drawing.Point(612, 42);
            this.txt_linkedcustid.Name = "txt_linkedcustid";
            this.txt_linkedcustid.Size = new System.Drawing.Size(124, 20);
            this.txt_linkedcustid.TabIndex = 57;
            this.txt_linkedcustid.Visible = false;
            // 
            // lbl_linkedcust
            // 
            this.lbl_linkedcust.AutoSize = true;
            this.lbl_linkedcust.Location = new System.Drawing.Point(519, 44);
            this.lbl_linkedcust.Name = "lbl_linkedcust";
            this.lbl_linkedcust.Size = new System.Drawing.Size(89, 13);
            this.lbl_linkedcust.TabIndex = 56;
            this.lbl_linkedcust.Text = "Linked Customer:";
            this.lbl_linkedcust.Visible = false;
            // 
            // Loyalty_HClogo_pb
            // 
            this.Loyalty_HClogo_pb.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Loyalty_HClogo_pb.Image = ((System.Drawing.Image)(resources.GetObject("Loyalty_HClogo_pb.Image")));
            this.Loyalty_HClogo_pb.Location = new System.Drawing.Point(329, 8);
            this.Loyalty_HClogo_pb.Name = "Loyalty_HClogo_pb";
            this.Loyalty_HClogo_pb.Size = new System.Drawing.Size(113, 26);
            this.Loyalty_HClogo_pb.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.Loyalty_HClogo_pb.TabIndex = 55;
            this.Loyalty_HClogo_pb.TabStop = false;
            this.Loyalty_HClogo_pb.Visible = false;
            // 
            // btnSave
            // 
            this.btnSave.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSave.BackgroundImage")));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSave.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnSave.Location = new System.Drawing.Point(696, 16);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(24, 24);
            this.btnSave.TabIndex = 40;
            this.btnSave.Click += new System.EventHandler(this.menuSave_Click);
            // 
            // label14
            // 
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(115, 27);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(32, 16);
            this.label14.TabIndex = 39;
            this.label14.Text = "COD";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lBranch
            // 
            this.lBranch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lBranch.Location = new System.Drawing.Point(8, 24);
            this.lBranch.Name = "lBranch";
            this.lBranch.Size = new System.Drawing.Size(48, 16);
            this.lBranch.TabIndex = 38;
            this.lBranch.Text = "Branch";
            this.lBranch.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // drpBranch
            // 
            this.drpBranch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpBranch.DropDownWidth = 48;
            this.drpBranch.Enabled = false;
            this.drpBranch.Location = new System.Drawing.Point(8, 40);
            this.drpBranch.Name = "drpBranch";
            this.drpBranch.Size = new System.Drawing.Size(56, 21);
            this.drpBranch.TabIndex = 0;
            this.drpBranch.SelectedIndexChanged += new System.EventHandler(this.drpBranch_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(448, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 16);
            this.label1.TabIndex = 36;
            this.label1.Text = "Emp No";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtEmpNumber
            // 
            this.txtEmpNumber.Location = new System.Drawing.Point(448, 40);
            this.txtEmpNumber.MaxLength = 4;
            this.txtEmpNumber.Name = "txtEmpNumber";
            this.txtEmpNumber.Size = new System.Drawing.Size(64, 20);
            this.txtEmpNumber.TabIndex = 5;
            this.txtEmpNumber.Leave += new System.EventHandler(this.txtEmpNumber_Leave);
            // 
            // btnCustomerSearch
            // 
            this.errorProvider1.SetIconAlignment(this.btnCustomerSearch, System.Windows.Forms.ErrorIconAlignment.TopLeft);
            this.btnCustomerSearch.ImageIndex = 2;
            this.btnCustomerSearch.ImageList = this.imageList2;
            this.btnCustomerSearch.Location = new System.Drawing.Point(736, 16);
            this.btnCustomerSearch.Name = "btnCustomerSearch";
            this.btnCustomerSearch.Size = new System.Drawing.Size(32, 24);
            this.btnCustomerSearch.TabIndex = 9;
            this.btnCustomerSearch.Click += new System.EventHandler(this.menuCustomerSearch_Click);
            // 
            // chxDutyFree
            // 
            this.chxDutyFree.Enabled = false;
            this.chxDutyFree.Location = new System.Drawing.Point(608, 16);
            this.chxDutyFree.Name = "chxDutyFree";
            this.chxDutyFree.Size = new System.Drawing.Size(80, 24);
            this.chxDutyFree.TabIndex = 8;
            this.chxDutyFree.Text = "Duty Free";
            this.chxDutyFree.Visible = false;
            this.chxDutyFree.CheckedChanged += new System.EventHandler(this.chxDutyFree_CheckedChanged);
            // 
            // chxTaxExempt
            // 
            this.chxTaxExempt.Enabled = false;
            this.chxTaxExempt.Location = new System.Drawing.Point(520, 16);
            this.chxTaxExempt.Name = "chxTaxExempt";
            this.chxTaxExempt.Size = new System.Drawing.Size(88, 24);
            this.chxTaxExempt.TabIndex = 7;
            this.chxTaxExempt.Text = "Tax Exempt";
            this.chxTaxExempt.CheckedChanged += new System.EventHandler(this.chxTaxExempt_CheckedChanged);
            // 
            // txtAccountNumber
            // 
            this.txtAccountNumber.Location = new System.Drawing.Point(184, 40);
            this.txtAccountNumber.Name = "txtAccountNumber";
            this.txtAccountNumber.PreventPaste = false;
            this.txtAccountNumber.ReadOnly = true;
            this.txtAccountNumber.Size = new System.Drawing.Size(88, 20);
            this.txtAccountNumber.TabIndex = 3;
            this.txtAccountNumber.TabStop = false;
            this.txtAccountNumber.Text = "000-0000-0000-0";
            this.txtAccountNumber.Leave += new System.EventHandler(this.txtAccountNumber_Leave);
            // 
            // chxCOD
            // 
            this.chxCOD.CheckAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.chxCOD.Enabled = false;
            this.chxCOD.Location = new System.Drawing.Point(112, 46);
            this.chxCOD.Name = "chxCOD";
            this.chxCOD.Size = new System.Drawing.Size(32, 16);
            this.chxCOD.TabIndex = 2;
            this.chxCOD.CheckedChanged += new System.EventHandler(this.chxCOD_CheckedChanged);
            // 
            // drpSOA
            // 
            this.drpSOA.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpSOA.DropDownWidth = 120;
            this.drpSOA.Location = new System.Drawing.Point(520, 40);
            this.drpSOA.Name = "drpSOA";
            this.drpSOA.Size = new System.Drawing.Size(160, 21);
            this.drpSOA.TabIndex = 6;
            this.drpSOA.SelectedIndexChanged += new System.EventHandler(this.drpSOA_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(184, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 16);
            this.label3.TabIndex = 6;
            this.label3.Text = "Account No";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // drpSoldBy
            // 
            this.drpSoldBy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpSoldBy.DropDownWidth = 121;
            this.drpSoldBy.Location = new System.Drawing.Point(280, 40);
            this.drpSoldBy.Name = "drpSoldBy";
            this.drpSoldBy.Size = new System.Drawing.Size(160, 21);
            this.drpSoldBy.TabIndex = 4;
            this.drpSoldBy.SelectedIndexChanged += new System.EventHandler(this.drpSoldBy_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(280, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Sold By";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lAccountType
            // 
            this.lAccountType.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lAccountType.Location = new System.Drawing.Point(80, 24);
            this.lAccountType.Name = "lAccountType";
            this.lAccountType.Size = new System.Drawing.Size(40, 16);
            this.lAccountType.TabIndex = 1;
            this.lAccountType.Text = "Type";
            this.lAccountType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // drpAccountType
            // 
            this.drpAccountType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpAccountType.DropDownWidth = 40;
            this.drpAccountType.Location = new System.Drawing.Point(70, 41);
            this.drpAccountType.Name = "drpAccountType";
            this.drpAccountType.Size = new System.Drawing.Size(40, 21);
            this.drpAccountType.TabIndex = 1;
            this.drpAccountType.SelectedIndexChanged += new System.EventHandler(this.drpAccountType_SelectedIndexChanged);
            // 
            // btnPrint
            // 
            this.btnPrint.Enabled = false;
            this.errorProvider1.SetIconAlignment(this.btnPrint, System.Windows.Forms.ErrorIconAlignment.TopLeft);
            this.btnPrint.Image = ((System.Drawing.Image)(resources.GetObject("btnPrint.Image")));
            this.btnPrint.Location = new System.Drawing.Point(736, 40);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(32, 24);
            this.btnPrint.TabIndex = 10;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // lShowResult
            // 
            this.lShowResult.Enabled = false;
            this.lShowResult.Location = new System.Drawing.Point(336, 32);
            this.lShowResult.Name = "lShowResult";
            this.lShowResult.Size = new System.Drawing.Size(72, 24);
            this.lShowResult.TabIndex = 37;
            this.lShowResult.Text = "lShowResult";
            this.lShowResult.Visible = false;
            // 
            // allowHP
            // 
            this.allowHP.BackColor = System.Drawing.SystemColors.ControlDark;
            this.allowHP.Enabled = false;
            this.allowHP.Location = new System.Drawing.Point(16, 9);
            this.allowHP.Name = "allowHP";
            this.allowHP.Size = new System.Drawing.Size(16, 16);
            this.allowHP.TabIndex = 48;
            this.allowHP.Visible = false;
            // 
            // gbHP
            // 
            this.gbHP.BackColor = System.Drawing.SystemColors.Control;
            this.gbHP.Controls.Add(this.txtPrefDay);
            this.gbHP.Controls.Add(this.lblPrefDay);
            this.gbHP.Controls.Add(this.lblFirstInstalmentWaiver);
            this.gbHP.Controls.Add(this.lblDepositWaiver);
            this.gbHP.Controls.Add(this.btnTermsTypeBand);
            this.gbHP.Controls.Add(this.btnPlan);
            this.gbHP.Controls.Add(this.cbDeposit);
            this.gbHP.Controls.Add(this.drpDueDay);
            this.gbHP.Controls.Add(this.numPaymentHolidays);
            this.gbHP.Controls.Add(this.label7);
            this.gbHP.Controls.Add(this.lAdminCharge);
            this.gbHP.Controls.Add(this.lInsuranceCharge);
            this.gbHP.Controls.Add(this.txtInsuranceCharge);
            this.gbHP.Controls.Add(this.txtAdminCharge);
            this.gbHP.Controls.Add(this.lblMaxSpend);
            this.gbHP.Controls.Add(this.txtRFMax);
            this.gbHP.Controls.Add(this.dtDateFirst);
            this.gbHP.Controls.Add(this.label27);
            this.gbHP.Controls.Add(this.label4);
            this.gbHP.Controls.Add(this.drpTermsType);
            this.gbHP.Controls.Add(this.label21);
            this.gbHP.Controls.Add(this.label20);
            this.gbHP.Controls.Add(this.txtFinalInstalment);
            this.gbHP.Controls.Add(this.txtInstalment);
            this.gbHP.Controls.Add(this.txtDeposit);
            this.gbHP.Controls.Add(this.label18);
            this.gbHP.Controls.Add(this.txtAgreementTotal);
            this.gbHP.Controls.Add(this.label25);
            this.gbHP.Controls.Add(this.label24);
            this.gbHP.Controls.Add(this.txtTerms);
            this.gbHP.Controls.Add(this.txtNoMonths);
            this.gbHP.Controls.Add(this.drpLengths);
            this.gbHP.Controls.Add(this.lDueDay);
            this.gbHP.Controls.Add(this.label19);
            this.gbHP.Enabled = false;
            this.gbHP.Location = new System.Drawing.Point(8, 367);
            this.gbHP.Name = "gbHP";
            this.gbHP.Size = new System.Drawing.Size(776, 107);
            this.gbHP.TabIndex = 2;
            this.gbHP.TabStop = false;
            this.gbHP.Text = "Agreement Information";
            this.gbHP.Enter += new System.EventHandler(this.gbHP_Enter);
            // 
            // txtPrefDay
            // 
            this.txtPrefDay.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.txtPrefDay.DropDownWidth = 48;
            this.txtPrefDay.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24",
            "25",
            "26",
            "27",
            "28"});
            this.txtPrefDay.Location = new System.Drawing.Point(421, 44);
            this.txtPrefDay.Name = "txtPrefDay";
            this.txtPrefDay.Size = new System.Drawing.Size(48, 21);
            this.txtPrefDay.TabIndex = 68;
            this.txtPrefDay.Visible = false;
            this.txtPrefDay.SelectedIndex = 0;
            // 
            // lblPrefDay
            // 
            this.lblPrefDay.Location = new System.Drawing.Point(420, 28);
            this.lblPrefDay.Name = "lblPrefDay";
            this.lblPrefDay.Size = new System.Drawing.Size(56, 16);
            this.lblPrefDay.TabIndex = 67;
            this.lblPrefDay.Text = "Pref Day";
            this.lblPrefDay.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblPrefDay.Visible = false;
            // 
            // lblFirstInstalmentWaiver
            // 
            this.lblFirstInstalmentWaiver.AutoSize = true;
            this.lblFirstInstalmentWaiver.BackColor = System.Drawing.SystemColors.Control;
            this.lblFirstInstalmentWaiver.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFirstInstalmentWaiver.ForeColor = System.Drawing.Color.DarkGreen;
            this.lblFirstInstalmentWaiver.Location = new System.Drawing.Point(488, 9);
            this.lblFirstInstalmentWaiver.Name = "lblFirstInstalmentWaiver";
            this.lblFirstInstalmentWaiver.Size = new System.Drawing.Size(252, 13);
            this.lblFirstInstalmentWaiver.TabIndex = 66;
            this.lblFirstInstalmentWaiver.Text = "Customer qualifies for delivery without first instalment";
            this.lblFirstInstalmentWaiver.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblFirstInstalmentWaiver.Visible = false;
            // 
            // lblDepositWaiver
            // 
            this.lblDepositWaiver.AutoSize = true;
            this.lblDepositWaiver.ForeColor = System.Drawing.Color.DarkGreen;
            this.lblDepositWaiver.Location = new System.Drawing.Point(188, 9);
            this.lblDepositWaiver.Name = "lblDepositWaiver";
            this.lblDepositWaiver.Size = new System.Drawing.Size(178, 13);
            this.lblDepositWaiver.TabIndex = 65;
            this.lblDepositWaiver.Text = "Customer qualifies for deposit waiver";
            this.lblDepositWaiver.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblDepositWaiver.Visible = false;
            // 
            // btnTermsTypeBand
            // 
            this.btnTermsTypeBand.Image = global::STL.PL.Properties.Resources.TTBands1;
            this.btnTermsTypeBand.Location = new System.Drawing.Point(137, 20);
            this.btnTermsTypeBand.Name = "btnTermsTypeBand";
            this.btnTermsTypeBand.Size = new System.Drawing.Size(24, 22);
            this.btnTermsTypeBand.TabIndex = 49;
            this.btnTermsTypeBand.Visible = false;
            this.btnTermsTypeBand.Click += new System.EventHandler(this.btnTermsTypeBand_Click);
            // 
            // btnPlan
            // 
            this.btnPlan.Location = new System.Drawing.Point(426, 70);
            this.btnPlan.Name = "btnPlan";
            this.btnPlan.Size = new System.Drawing.Size(56, 24);
            this.btnPlan.TabIndex = 45;
            this.btnPlan.Text = "Plan";
            this.btnPlan.Visible = false;
            this.btnPlan.Click += new System.EventHandler(this.btnPlan_Click);
            // 
            // cbDeposit
            // 
            this.cbDeposit.Location = new System.Drawing.Point(185, 43);
            this.cbDeposit.Name = "cbDeposit";
            this.cbDeposit.Size = new System.Drawing.Size(16, 24);
            this.cbDeposit.TabIndex = 43;
            this.cbDeposit.Click += new System.EventHandler(this.cbDeposit_Click);
            // 
            // drpDueDay
            // 
            this.drpDueDay.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpDueDay.DropDownWidth = 48;
            this.drpDueDay.Items.AddRange(new object[] {
            "H",
            "W",
            "D",
            "D1",
            "D2",
            "D3"});
            this.drpDueDay.Location = new System.Drawing.Point(364, 44);
            this.drpDueDay.Name = "drpDueDay";
            this.drpDueDay.Size = new System.Drawing.Size(48, 21);
            this.drpDueDay.TabIndex = 40;
            // 
            // numPaymentHolidays
            // 
            this.numPaymentHolidays.Enabled = false;
            this.numPaymentHolidays.Location = new System.Drawing.Point(9, 86);
            this.numPaymentHolidays.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numPaymentHolidays.Name = "numPaymentHolidays";
            this.numPaymentHolidays.Size = new System.Drawing.Size(48, 20);
            this.numPaymentHolidays.TabIndex = 41;
            this.numPaymentHolidays.ValueChanged += new System.EventHandler(this.numPaymentHolidays_ValueChanged);
            this.numPaymentHolidays.Validating += new System.ComponentModel.CancelEventHandler(this.numPaymentHolidays_Validating);
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(7, 69);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(128, 16);
            this.label7.TabIndex = 38;
            this.label7.Text = "No. of payment holidays";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lAdminCharge
            // 
            this.lAdminCharge.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.lAdminCharge.Location = new System.Drawing.Point(588, 65);
            this.lAdminCharge.Name = "lAdminCharge";
            this.lAdminCharge.Size = new System.Drawing.Size(80, 12);
            this.lAdminCharge.TabIndex = 37;
            this.lAdminCharge.Text = "Admin Charge";
            this.lAdminCharge.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lInsuranceCharge
            // 
            this.lInsuranceCharge.Location = new System.Drawing.Point(488, 64);
            this.lInsuranceCharge.Name = "lInsuranceCharge";
            this.lInsuranceCharge.Size = new System.Drawing.Size(100, 16);
            this.lInsuranceCharge.TabIndex = 35;
            this.lInsuranceCharge.Text = "Insurance Charge";
            this.lInsuranceCharge.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtInsuranceCharge
            // 
            this.txtInsuranceCharge.Location = new System.Drawing.Point(503, 83);
            this.txtInsuranceCharge.Name = "txtInsuranceCharge";
            this.txtInsuranceCharge.ReadOnly = true;
            this.txtInsuranceCharge.Size = new System.Drawing.Size(72, 20);
            this.txtInsuranceCharge.TabIndex = 34;
            this.txtInsuranceCharge.TabStop = false;
            this.txtInsuranceCharge.Text = "0";
            this.txtInsuranceCharge.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtAdminCharge
            // 
            this.txtAdminCharge.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.txtAdminCharge.Location = new System.Drawing.Point(587, 84);
            this.txtAdminCharge.Name = "txtAdminCharge";
            this.txtAdminCharge.ReadOnly = true;
            this.txtAdminCharge.Size = new System.Drawing.Size(72, 20);
            this.txtAdminCharge.TabIndex = 36;
            this.txtAdminCharge.TabStop = false;
            this.txtAdminCharge.Text = "0";
            this.txtAdminCharge.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblMaxSpend
            // 
            this.lblMaxSpend.Location = new System.Drawing.Point(584, 67);
            this.lblMaxSpend.Name = "lblMaxSpend";
            this.lblMaxSpend.Size = new System.Drawing.Size(80, 16);
            this.lblMaxSpend.TabIndex = 33;
            this.lblMaxSpend.Text = "Max Spend:";
            this.lblMaxSpend.Visible = false;
            // 
            // txtRFMax
            // 
            this.txtRFMax.Location = new System.Drawing.Point(586, 85);
            this.txtRFMax.Name = "txtRFMax";
            this.txtRFMax.ReadOnly = true;
            this.txtRFMax.Size = new System.Drawing.Size(72, 20);
            this.txtRFMax.TabIndex = 8;
            this.txtRFMax.TabStop = false;
            this.txtRFMax.Text = "0";
            this.txtRFMax.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtRFMax.Visible = false;
            // 
            // dtDateFirst
            // 
            this.dtDateFirst.CustomFormat = "ddd dd MMM yyyy";
            this.dtDateFirst.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtDateFirst.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.dtDateFirst.Location = new System.Drawing.Point(125, 86);
            this.dtDateFirst.Name = "dtDateFirst";
            this.dtDateFirst.Size = new System.Drawing.Size(112, 20);
            this.dtDateFirst.TabIndex = 42;
            // 
            // label27
            // 
            this.label27.Location = new System.Drawing.Point(125, 69);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(112, 16);
            this.label27.TabIndex = 31;
            this.label27.Text = "First Instalment Date";
            this.label27.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(8, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(96, 16);
            this.label4.TabIndex = 8;
            this.label4.Text = "Terms Type";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // drpTermsType
            // 
            this.drpTermsType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpTermsType.DropDownWidth = 152;
            this.drpTermsType.Location = new System.Drawing.Point(8, 44);
            this.drpTermsType.Name = "drpTermsType";
            this.drpTermsType.Size = new System.Drawing.Size(152, 21);
            this.drpTermsType.TabIndex = 0;
            this.drpTermsType.SelectedIndexChanged += new System.EventHandler(this.drpTermsType_SelectedIndexChanged);
            // 
            // label21
            // 
            this.label21.Location = new System.Drawing.Point(581, 26);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(88, 16);
            this.label21.TabIndex = 6;
            this.label21.Text = "Final Instalment";
            this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label20
            // 
            this.label20.Location = new System.Drawing.Point(488, 26);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(56, 16);
            this.label20.TabIndex = 5;
            this.label20.Text = "Instalment";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtFinalInstalment
            // 
            this.txtFinalInstalment.Location = new System.Drawing.Point(582, 43);
            this.txtFinalInstalment.Name = "txtFinalInstalment";
            this.txtFinalInstalment.ReadOnly = true;
            this.txtFinalInstalment.Size = new System.Drawing.Size(72, 20);
            this.txtFinalInstalment.TabIndex = 6;
            this.txtFinalInstalment.TabStop = false;
            this.txtFinalInstalment.Text = "0";
            this.txtFinalInstalment.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtInstalment
            // 
            this.txtInstalment.Location = new System.Drawing.Point(495, 43);
            this.txtInstalment.Name = "txtInstalment";
            this.txtInstalment.ReadOnly = true;
            this.txtInstalment.Size = new System.Drawing.Size(72, 20);
            this.txtInstalment.TabIndex = 5;
            this.txtInstalment.TabStop = false;
            this.txtInstalment.Text = "0";
            this.txtInstalment.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtDeposit
            // 
            this.txtDeposit.Enabled = false;
            this.txtDeposit.Location = new System.Drawing.Point(208, 45);
            this.txtDeposit.Name = "txtDeposit";
            this.txtDeposit.Size = new System.Drawing.Size(72, 20);
            this.txtDeposit.TabIndex = 1;
            this.txtDeposit.Text = "0";
            this.txtDeposit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtDeposit.Validating += new System.ComponentModel.CancelEventHandler(this.txtDeposit_Validating);
            // 
            // label18
            // 
            this.label18.Location = new System.Drawing.Point(208, 28);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(48, 16);
            this.label18.TabIndex = 0;
            this.label18.Text = "Deposit";
            this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtAgreementTotal
            // 
            this.txtAgreementTotal.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.txtAgreementTotal.Location = new System.Drawing.Point(680, 83);
            this.txtAgreementTotal.Name = "txtAgreementTotal";
            this.txtAgreementTotal.ReadOnly = true;
            this.txtAgreementTotal.Size = new System.Drawing.Size(88, 20);
            this.txtAgreementTotal.TabIndex = 9;
            this.txtAgreementTotal.TabStop = false;
            this.txtAgreementTotal.Text = "0";
            this.txtAgreementTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtAgreementTotal.TextChanged += new System.EventHandler(this.txtAgreementTotal_TextChanged);
            // 
            // label25
            // 
            this.label25.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label25.Location = new System.Drawing.Point(680, 64);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(93, 16);
            this.label25.TabIndex = 29;
            this.label25.Text = "Agreement Total";
            this.label25.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label24
            // 
            this.label24.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label24.Location = new System.Drawing.Point(680, 23);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(88, 16);
            this.label24.TabIndex = 28;
            this.label24.Text = "Service Charge";
            this.label24.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtTerms
            // 
            this.txtTerms.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.txtTerms.Location = new System.Drawing.Point(680, 40);
            this.txtTerms.Name = "txtTerms";
            this.txtTerms.ReadOnly = true;
            this.txtTerms.Size = new System.Drawing.Size(88, 20);
            this.txtTerms.TabIndex = 7;
            this.txtTerms.TabStop = false;
            this.txtTerms.Text = "0";
            this.txtTerms.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtNoMonths
            // 
            this.txtNoMonths.Enabled = false;
            this.txtNoMonths.Location = new System.Drawing.Point(287, 45);
            this.txtNoMonths.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txtNoMonths.Name = "txtNoMonths";
            this.txtNoMonths.Size = new System.Drawing.Size(56, 20);
            this.txtNoMonths.TabIndex = 2;
            this.txtNoMonths.ValueChanged += new System.EventHandler(this.txtNoMonths_ValueChanged);
            this.txtNoMonths.Validating += new System.ComponentModel.CancelEventHandler(this.txtNoMonths_Validating);
            // 
            // drpLengths
            // 
            this.drpLengths.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpLengths.DropDownWidth = 152;
            this.drpLengths.Location = new System.Drawing.Point(288, 44);
            this.drpLengths.Name = "drpLengths";
            this.drpLengths.Size = new System.Drawing.Size(56, 21);
            this.drpLengths.TabIndex = 44;
            this.drpLengths.Visible = false;
            this.drpLengths.SelectedIndexChanged += new System.EventHandler(this.drpLengths_SelectedIndexChanged);
            // 
            // lDueDay
            // 
            this.lDueDay.Location = new System.Drawing.Point(364, 26);
            this.lDueDay.Name = "lDueDay";
            this.lDueDay.Size = new System.Drawing.Size(56, 16);
            this.lDueDay.TabIndex = 7;
            this.lDueDay.Text = "Due Day";
            this.lDueDay.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label19
            // 
            this.label19.Location = new System.Drawing.Point(284, 28);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(100, 16);
            this.label19.TabIndex = 2;
            this.label19.Text = "No of Instalments";
            this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // errorProviderForWarning
            // 
            this.errorProviderForWarning.ContainerControl = this;
            this.errorProviderForWarning.Icon = ((System.Drawing.Icon)(resources.GetObject("errorProviderForWarning.Icon")));
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // errorProvider2
            // 
            this.errorProvider2.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider2.ContainerControl = this;
            this.errorProvider2.DataMember = "";
            this.errorProvider2.Icon = ((System.Drawing.Icon)(resources.GetObject("errorProvider2.Icon")));
            // 
            // errorProviderStoreCard
            // 
            this.errorProviderStoreCard.ContainerControl = this;
            // 
            // lblAgrInvoieNum
            // 
            this.lblAgrInvoieNum.AutoSize = true;
            this.lblAgrInvoieNum.ForeColor = System.Drawing.Color.Black;
            this.lblAgrInvoieNum.Location = new System.Drawing.Point(70, 5);
            this.lblAgrInvoieNum.Name = "lblAgrInvoieNum";
            this.lblAgrInvoieNum.Size = new System.Drawing.Size(19, 13);
            this.lblAgrInvoieNum.TabIndex = 71;
            this.lblAgrInvoieNum.Text = "10";
            this.lblAgrInvoieNum.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblOrdInvoice
            // 
            this.lblOrdInvoice.AutoSize = true;
            this.lblOrdInvoice.ForeColor = System.Drawing.Color.Black;
            this.lblOrdInvoice.Location = new System.Drawing.Point(6, 5);
            this.lblOrdInvoice.Name = "lblOrdInvoice";
            this.lblOrdInvoice.Size = new System.Drawing.Size(67, 13);
            this.lblOrdInvoice.TabIndex = 70;
            this.lblOrdInvoice.Text = "Ord/Invoice:";
            this.lblOrdInvoice.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // NewAccount
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lShowResult);
            this.Controls.Add(this.allowHP);
            this.Controls.Add(this.gbHP);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "NewAccount";
            this.Text = "New Sales Order";
            this.Load += new System.EventHandler(this.NewAccount_Load);
            this.Load += new System.EventHandler(this.SetNonStockFlag);
            this.Enter += new System.EventHandler(this.NewAccount_Enter);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.gbPaidAndTaken.ResumeLayout(false);
            this.gbPaidAndTaken.PerformLayout();
            this.pReplacement.ResumeLayout(false);
            this.pReplacement.PerformLayout();
            this.pnBank.ResumeLayout(false);
            this.pnBank.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgLineItems)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Loyalty_HClogo_pb)).EndInit();
            this.gbHP.ResumeLayout(false);
            this.gbHP.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPaymentHolidays)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNoMonths)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderForWarning)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderStoreCard)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private void NewAccount_Load(object sender, System.EventArgs e)
        {
            //IP - 26/01/11 - Sprint 5.9 - Display the correct controls
            if (PaidAndTaken)
            {
                gbHP.Visible = false;
                gbPaidAndTaken.Visible = true;
            }
            else
            {
                gbPaidAndTaken.Visible = false;
                gbHP.Visible = true;
                cbTime.SelectedIndex = 0;                       //IP - 28/05/12 - #10225
            }

            // Init Delivery Address Type to HOME
            int i = drpDeliveryAddress.FindStringExact(GetResource("L_HOME"));
            if (i >= 0) drpDeliveryAddress.SelectedIndex = i;

            // Reset the item Delivery Area, but only when intialising
            cbImmediate.Checked = false;
            SetDeliveryImmediate();

            dtDeliveryRequired.Value = this._today.AddDays(Convert.ToInt32(Country[CountryParameterNames.DefaultDelDays]));
            // Show button for terms type bands if this is enabled
            btnTermsTypeBand.Visible = (Convert.ToBoolean(Country[CountryParameterNames.TermsTypeBandEnabled]));

            //IP - 15/02/10 - CR1072 - If this is the Cash & Go Return screen, then prevent users from entering into this textbox.
            if (CashAndGoReturn)
            {
                txtProductCode.Enabled = false;
                SetupCashAndGoReturn(_acctNo.Replace("-", ""), AgreementNo);
            }

            if (((bool)Country[CountryParameterNames.StoreCardEnabled]) && (CashAndGoReturn || PaidAndTaken))
            {
                MagStripeReader = new StoreCardMagStripeReader(Country[CountryParameterNames.StoreCardMagStripeReaderName].ToString(),
                readEvent =>
                {
                    if (readEvent.state == ControlState.Idle && IsStoreCardPaymentSelected())
                    {
                        this.BeginInvoke(new InvokeDelegate(delegate
                        {
                            mtb_CardNo.Text = DecodeMsrTrack.Decode(readEvent.Track1);
                        }));

                    }
                },
                    errorevent =>
                    {
                        //Was going to put some error code thing here but too hard.
                        MainForm.Current.ShowStatus("Magnetic Stripe Read failed.");
                    }
                );
            }
            //10.6 CR- Sales Order - Print Save          
            _isSalesOrderChanged = false;
        }
        private void SetNonStockFlag(object sender, System.EventArgs e)
        {
            DataSet ns = AccountManager.IsAccountValidForOnlyNonStockSale(this.AccountNo, out Error);
            if (ns != null)
            {
                bool isNonStock = Convert.ToBoolean(ns.Tables["AccountDetails"].Rows[0]["isNonStock"]);
                if (isNonStock)
                {
                    this.chxNonStock.Checked = true;
                    this.chxNonStock.Enabled = false;
                }
            }
        }
        private void InitData()
        {
            Function = "txtProductCode_TextChanged";
            try
            {
                ClearItemDetails();

                // Init Delivery Address Type to HOME
                int i = drpDeliveryAddress.FindStringExact(GetResource("L_HOME"));
                if (i >= 0) drpDeliveryAddress.SelectedIndex = i;

                // Reset the item Delivery Area, but only when intialising
                cbImmediate.Checked = false;
                SetDeliveryImmediate();

                dtDeliveryRequired.Value = this._today.AddDays(Convert.ToInt32(Country[CountryParameterNames.DefaultDelDays]));

                drpPayMethod.SelectedIndex = 0;
                drpSoldBy.SelectedIndex = 0;
                drpSOA.SelectedIndex = 0;
                drpBank.SelectedIndex = 0;
                itemDoc.LoadXml("<ITEMS/>");
                _origAgreementTotal = 0;
                populateTable();
                btnRemove.Enabled = false;
                txtLoyaltyCardNo.Text = "";
                WarrantiesOnCredit = false;
                ptWarranties = false;
                PTWarrantyAccountNo = "";
                chxGiftVoucher.Checked = false;
                txtTendered.Text = "0";
                txtAmount.Text = "0";
                txtChange.Text = "0";
                txtDue.Text = "0";
                txtSubTotal.Text = "0";
                txtPrivClubDiscount.Text = "";
                txtSalesTax.Text = "0";
                txtQuantity.Text = "0";
                txtBankAcctNo.Text = "";
                txtChequeNo.Text = "";
                InitPaymentSet();
                AgreementNo = 1;
                cbDamaged.Checked = false;
                cbAssembly.Checked = false;
                cbExpress.Checked = false;              //IP - 07/06/12 - #10229 - Warehouse & Deliveries
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void HashMenus()
        {
            dynamicMenus[this.Name + ":chxDutyFree"] = this.chxDutyFree;
            dynamicMenus[this.Name + ":allowHP"] = this.allowHP;
            dynamicMenus[this.Name + ":allowRF"] = this.allowRF;
            dynamicMenus[this.Name + ":changeTermsType"] = this.changeTermsType;
            dynamicMenus[this.Name + ":allowSpecial"] = this.allowSpecial;
            dynamicMenus[this.Name + ":lShowResult"] = this.lShowResult;
            dynamicMenus[this.Name + ":drpBranch"] = this.drpBranch;
            dynamicMenus[this.Name + ":allowInstantReplacement"] = this.allowInstantReplacement;
            dynamicMenus[this.Name + ":allowSupaShield"] = this.allowSupaShield;
            dynamicMenus[this.Name + ":allowGOL"] = this.allowGOL;
            dynamicMenus[this.Name + ":allowTaxExempt"] = this.allowTaxExempt;
            dynamicMenus[this.Name + ":lSpendLimit"] = this.lSpendLimit;
            dynamicMenus[this.Name + ":lreviseNonScheduled"] = this.lreviseNonScheduled;
            dynamicMenus[this.Name + ":lreviseScheduled"] = this.lreviseScheduled;
        }

        /// <summary>
        /// Sets the curerntly selected item controls to blanks
        /// </summary>
        public void ClearItemDetails()
        {

            StringCollection empty = new StringCollection();
            drpLocation.DataSource = empty;
            drpProductDesc.DataSource = empty;
            txtUnitPrice.Text = "0";
            txtValue.Text = "0";
            txtValue.ReadOnly = true;
            txtAvailable.Text = "0";
            btnEnter.Enabled = false;
            txtColourTrim.Text = "";
            txtQuantity.Text = "0";
            //txtTimeRequired.Text = ""; //IP - 21/01/08            //IP - 28/05/12 - #10225
            cbImmediate.Checked = false;
            cbImmediate.Enabled = true;

            if (chxNonStock.Checked)
            {
                cbImmediate.Checked = true;
                cbImmediate.Enabled = false;
            }

            cbDamaged.Checked = false;
            cbAssembly.Checked = false;
            //cbExpress.Checked = false;  //IP - 07/06/12 - #10229 - Warehouse & Deliveries

            if (custidrequested)
            {
                lbl_linkedcust.Visible = true;
                txt_linkedcustid.Visible = true;
                btnCustomerSearch.Visible = true;
                btnCustomerSearch.Enabled = true;
                menuCustomerSearch.Visible = true;
                menuCustomerSearch.Enabled = true;
            }

            if (AlignDates) dtDeliveryRequired.Value = AlignedDelDate;

            //FA 21-12 Eliminated this as it would reset error icon when not wanted.
            // This function is within the checkdeliverydate and setitemdetail functions so it should not be here too...
            //errorProvider2.SetError(dtDeliveryRequired, "");
            errorProvider2.SetError(drpDeliveryArea, "");
            errorProvider1.SetError(txtValue, ""); //IP - 27/10/09 - CoSACS Improvements - Credit Discounts
            errorProviderForWarning.SetError(lblDummyForErrorProvider, "");
        }

        /// <summary>
        /// populates the currently selected item controls with information
        /// from an XML node returned by the business logic
        /// </summary>
        /// <param name="item">Xml node representing an item</param>
        public void SetItemDetails(XmlNode item)
        {
            this._userChanged = false;

            StringCollection desc = new StringCollection();

            ItemKey = item.Attributes[Tags.Key].Value;
            ItemType = item.Attributes[Tags.Type].Value;

            if (Convert.ToBoolean(item.Attributes[Tags.RepoItem].Value) == true)
                ProductCode = "R" + item.Attributes[Tags.Code].Value;     // RI jec 16/06/11 Repo Item
            else
                ProductCode = item.Attributes[Tags.Code].Value;
            ItemID = item.Attributes[Tags.ItemId].Value.TryParseInt32();
            desc.Add(item.Attributes[Tags.Description1].Value);
            desc.Add(item.Attributes[Tags.Description2].Value);
            UnitPrice = Convert.ToDecimal(item.Attributes[Tags.UnitPrice].Value).ToString(DecimalPlaces);
            AvailableStock = item.Attributes[Tags.AvailableStock].Value;
            //DamagedStock = item.Attributes[Tags.DamagedStock].Value;
            ValueControlled = Convert.ToBoolean(item.Attributes[Tags.ValueControlled].Value);
            //IP - 05/11/08 - UAT5.1 - UAT(555) - Needed to set the datasource of the drop down
            //to null as this would not always get set to the correct location of the item
            //and would cause an error when adding a warranty as the incorrect 'key' was 
            //being used to search for the item in the XML.
            drpLocation.DataSource = null;
            drpLocation.DataSource = new string[] { item.Attributes[Tags.Location].Value };
            txtQuantity.Text = item.Attributes[Tags.Quantity].Value;
            txtColourTrim.Text = item.Attributes[Tags.ColourTrim].Value;
            //txtTimeRequired.Text = item.Attributes[Tags.DeliveryTime].Value;
            cbTime.SelectedItem = item.Attributes[Tags.DeliveryTime].Value;                 //IP - 25/05/12 - #10225

            if (item.Attributes[Tags.DeliveryDate].Value.Length != 0)
            {
                dtDeliveryRequired.Value = Convert.ToDateTime(item.Attributes[Tags.DeliveryDate].Value);
            }

            int i = drpBranchForDel.FindStringExact(item.Attributes[Tags.BranchForDeliveryNote].Value);
            //IP/JC - 02/03/10 - CR1072 - Malaysia 3PL 
            //if (i != -1) drpBranchForDel.SelectedIndex = i;
            if (i != -1)
                drpBranchForDel.SelectedIndex = i;
            else
            {
                i = drpBranchForDel.FindString(Config.BranchCode);
                drpBranchForDel.SelectedIndex = i;
            }

            i = drpDeliveryArea.FindStringExact(item.Attributes[Tags.DeliveryArea].Value);
            if (i != -1) drpDeliveryArea.SelectedIndex = i;

            if (item.Attributes[Tags.DeliveryProcess].Value.Length != 0)
                cbImmediate.Checked = (item.Attributes[Tags.DeliveryProcess].Value == "I");

            if (item.Attributes[Tags.DeliveryAddress].Value.Length != 0)
            {
                //removed jec 09/05/07 drpDeliveryAddress.Text = item.Attributes[Tags.DeliveryAddress].Value;    
                // set Delivery Address Type to Lineitem value      (68949 jec 09/05/07)
                i = drpDeliveryAddress.FindStringExact(item.Attributes[Tags.DeliveryAddress].Value);
                drpDeliveryAddress.SelectedIndex = i >= 0 ? i : 0;
            }

            if (item.Attributes[Tags.Damaged].Value.Length != 0)
                cbDamaged.Checked = (item.Attributes[Tags.Damaged].Value == "Y");

            if (item.Attributes[Tags.Assembly].Value.Length != 0)
                cbAssembly.Checked = (item.Attributes[Tags.Assembly].Value == "Y");

            //IP - 07/06/12 - #10229 - Warehouse & Deliveries
            if (item.Attributes[Tags.Express].Value.Length != 0)
                cbExpress.Checked = (item.Attributes[Tags.Express].Value == "Y");

            txtValue.Text = UInt16.MinValue.ToString(DecimalPlaces);
            txtValue.ReadOnly = !ValueControlled;
            drpProductDesc.DataSource = desc;
            txtQuantity.Focus();

            errorProvider2.SetError(dtDeliveryRequired, "");
            errorProvider2.SetError(drpDeliveryArea, "");

            _userChanged = true;
        }

        private bool CheckDeliveryAreaDate()
        {
            // Check the planned delivery date coincides with this delivery area
            // If it does not then the user is shown the days for this delivery
            // area in a popup, but it is not enforced
            bool found = true;
            errorProvider2.SetError(dtDeliveryRequired, "");
            errorProvider2.SetError(drpDeliveryArea, "");

            short reqWeekDay = (short)dtDeliveryRequired.Value.DayOfWeek;
            if (reqWeekDay == 0) reqWeekDay = 7;

            if (drpDeliveryArea.Text != GetResource("L_ALL") && drpDeliveryArea.Enabled)
            {
                // Load the days for this set
                DataSet deliveryAreaSet = SetDataManager.GetSetDetailsForSetName(drpDeliveryArea.Text, TN.DeliveryArea, out this.Error);
                if (Error.Length > 0)
                    ShowError(Error);
                else if (deliveryAreaSet.Tables[0].Rows.Count > 0)
                {
                    // Only validate against areas with one or more days defined
                    found = false;
                    foreach (DataRow row in deliveryAreaSet.Tables[0].Rows)
                    {
                        if (row[CN.Code].ToString().Substring(0, 1) == reqWeekDay.ToString())
                            found = true;
                    }

                    if (!found)
                    {
                        errorProvider2.SetError(dtDeliveryRequired, GetResource("M_DELIVERYAREADAY"));
                        errorProvider2.SetError(drpDeliveryArea, GetResource("M_DELIVERYAREADAY"));

                        // Don't prompt again for the last values the user opted to continue
                        if (reqWeekDay != _contReqWeekDay || drpDeliveryArea.Text != _contDeliveryArea)
                        {
                            // Show this set to the user
                            ListInfoPopup listPop = new ListInfoPopup(
                                GetResource("T_DELIVERYAREAINFO"),
                                GetResource("M_DELIVERYAREAINFO", new object[] { dtDeliveryRequired.Value, drpDeliveryArea.Text }),
                                drpDeliveryArea.Text,
                                TN.DeliveryArea,
                                FormRoot, this);

                            // The user can override and continue
                            listPop.ShowDialog(this);
                            if (listPop.resultIgnore)
                            {
                                found = true;
                                _contReqWeekDay = reqWeekDay;
                                _contDeliveryArea = drpDeliveryArea.Text;
                            }
                        }
                        else
                        {
                            found = true;
                        }
                    }
                }
            }

            return found;
        }

        private void SetDeliveryImmediate()
        {
            if (cbImmediate.Checked)
            {
                // The customer is picking up the item
                // so disable the delivery address and the delivery area
                drpDeliveryAddress.SelectedIndex = 0;
                //drpDeliveryAddress.Enabled = false;           // #13841
                drpDeliveryArea.SelectedIndex = 0;
                //drpDeliveryArea.Enabled = false;              // #13841
                int i = drpDeliveryAddress.FindStringExact(GetResource("L_HOME"));          // #13841
                if (i >= 0) drpDeliveryAddress.SelectedIndex = i;
                SetDeliveryArea();
            }
            else
            {
                // Enable the delivery address and area
                drpDeliveryAddress.Enabled = true;
                drpDeliveryArea.Enabled = true;

                if (_userChanged)
                {
                    // Default to the home address
                    int i = drpDeliveryAddress.FindStringExact(GetResource("L_HOME"));
                    if (i >= 0) drpDeliveryAddress.SelectedIndex = i;
                    SetDeliveryArea();
                }
            }
        }

        private void SetDeliveryAreaList(string branchNo)
        {
            // The Delivery Area list is specific to the branch selected
            string curDeliveryArea = drpDeliveryArea.Text;
            StringCollection areaList = new StringCollection();         //#12855
                                                                        //areaList.Add(GetResource("L_ALL"));
            areaList.Add(" ");                                          //#12855

            if (_areaTable != null)
            {
                foreach (DataRow row in _areaTable.Rows)
                {
                    if (Convert.ToString(row[CN.BranchNo]) == branchNo)
                        areaList.Add((string)row.ItemArray[1]);
                }
            }
            drpDeliveryArea.DataSource = areaList;

            // Leave the Delivery Area on the current setting if it
            // is still in the list
            int i = drpDeliveryArea.FindStringExact(curDeliveryArea);
            drpDeliveryArea.SelectedIndex = i >= 0 ? i : 0;
        }

        private void SetDeliveryArea()
        {
            if (_userChanged)
            {
                if (CustomerID.Length > 0 && _custAddressSet == null)
                {
                    // Load the address types with their delivery areas for this customer
                    _custAddressSet = CustomerManager.GetCustomerAddresses(CustomerID, out Error);
                    if (Error.Length > 0)
                    {
                        ShowError(Error);
                    }
                }

                if (_custAddressSet != null)
                {
                    // Find the customer delivery area for this address type
                    foreach (DataTable addressTable in _custAddressSet.Tables)
                        foreach (DataRow addressRow in addressTable.Rows)
                        {
                            if (((string)addressRow[CN.AddressType]).Trim() == drpDeliveryAddress.Text.Trim())
                            {
                                int i = drpDeliveryArea.FindStringExact((string)addressRow[CN.DeliveryArea]);
                                if (i >= 0) drpDeliveryArea.SelectedIndex = i;
                            }
                        }
                }
            }
        }

        /// <summary>
        /// provides screen specific localisation based on country parameters
        /// </summary>
        private void Localise()
        {
            if (!(bool)Country[CountryParameterNames.PaymentMethod])
            {
                label17.Visible = false;
                drpPaymentMethod.Visible = false;
            }

            if ((decimal)Country[CountryParameterNames.FixedDateFirst] == 0)
            {
                label27.Visible = false;
                dtDateFirst.Visible = false;
                lDueDay.Visible = false;
                drpDueDay.Visible = false;
            }
            if ((decimal)Country[CountryParameterNames.FixedDateFirst] == 1)
            {
                label27.Visible = true;
                dtDateFirst.Visible = true;

                if ((!revision) || (!stockExists && drpAccountType.Text == AT.ReadyFinance))
                    dtDateFirst.Value = this._today.AddMonths(1);    //date without time (jec 29/06/06)

                lDueDay.Visible = false;
                drpDueDay.Visible = false;
            }
            if ((decimal)Country[CountryParameterNames.FixedDateFirst] == 2)
            {
                label27.Visible = false;
                dtDateFirst.Visible = false;
                lDueDay.Visible = true;
                drpDueDay.Visible = true;
            }

            //if (!(bool)Country[CountryParameterNames.DutyFree])
            //{
            //    chxDutyFree.Visible = false;
            //}

            // #10993
            if (Credential.HasPermission(CosacsPermissionEnum.AllowDutyFreeSales))
            {
                chxDutyFree.Visible = true;
            }
            else
            {
                chxDutyFree.Visible = false;
            }

            if ((bool)Country[CountryParameterNames.CODDefault])
            {
                chxCOD.Checked = true;
            }
            if ((string)Country[CountryParameterNames.AgreementTaxType] == "I") //hide tax fields if agreement inclusive
            {
                lblTax.Visible = false;
                txtSalesTax.Visible = false;
            }

            if (PaidAndTaken)
            {
                txtLoyaltyCardNo.Visible = (bool)Country[CountryParameterNames.PaymentMethod];
                lLoyaltyCardNo.Visible = (bool)Country[CountryParameterNames.PaymentMethod];
            }
            if (AccountType == AT.ReadyFinance && (bool)Country[CountryParameterNames.AbilitySetFirstPaymentDate])
            {
                lblPrefDay.Visible = true;
                txtPrefDay.Visible = true;
            }
            else
            {
                lblPrefDay.Visible = false;
                txtPrefDay.Visible = false;
            }
        }

        private void InitPaymentSet()
        {
            // CR586 Allow multiple pay methods held as a list
            if (!payMethodSet.Tables.Contains(TN.PayMethodList))
            {
                // Create the blank set of payments
                payMethodSet.Tables.Add(TN.PayMethodList);
                payMethodSet.Tables[TN.PayMethodList].Columns.AddRange(
                    new DataColumn[] {   new DataColumn(CN.Value,           Type.GetType("System.Decimal")),
                                         new DataColumn(CN.BankCode,        Type.GetType("System.String")),
                                         new DataColumn(CN.BankAccountNo,   Type.GetType("System.String")),
                                         new DataColumn(CN.ChequeNo,        Type.GetType("System.String")),
                                         new DataColumn(CN.PayMethod,       Type.GetType("System.Int16")),
                                         new DataColumn(CN.CodeDescription, Type.GetType("System.String"))});
            }
            else
            {
                // Clear the set of payments
                payMethodSet.Tables[TN.PayMethodList].Clear();
            }

            // Clear amount for next entry
            txtAmount.Text = (0).ToString(DecimalPlaces);

            drpBank.SelectedIndex = 0;
            txtBankAcctNo.Text = "";
            txtChequeNo.Text = "";

            // Enable amount field and add btn (unless Gift Voucher has fully paid)
            SetAmountEnabled();
            // Disable remove btn
            btnRemovePaymethod.Enabled = false;
            // Invoke leave method for amount
            txtAmount_Validating(new object(), new System.ComponentModel.CancelEventArgs());
            errorProvider1.SetError(txtBankAcctNo, "");
            errorProvider1.SetError(txtChequeNo, "");

            //IP - 11/02/10 - CR1048 (Ref:3.1.2.2 & 3.1.2.3) Merged - Malaysia Enhancements (CR1072)
            if (drpPayMethod.Items.Count > 0)
            {
                //If this is a Cash & Go return being processed.
                //if (PaidAndTaken && revision && AgreementNo != 1)
                if (CashAndGoReturn)
                {
                    //IP - CR1048 - 3.1.2.5 - Call to retrieve last fintrans record for the Cash & Go agreement.
                    SetCashGoRefundPayMethod();
                }
                else
                {
                    drpPayMethod.SelectedIndex = 0;
                }
            }
            else
            {
                drpPayMethod.SelectedIndex = -1;
                menuPrintReceipt.Enabled = false;
                btnPrintReceipt.Enabled = false;
                drpPayMethod.Enabled = false;
                drpBank.Enabled = false;
                txtBankAcctNo.Enabled = false;
                txtChequeNo.Enabled = false;
            }

            drpPayMethod.Focus();
            storeCardPayInfoList.Clear();
        }

        private void SetCustomerDetails()
        {
            // For Cash & Go customer details can be entered when either of:
            // . warranties on credit is checked on warranties dialog
            // . country parameter requires customer details for cash & go warranties
            hasInstallation = false;
            foreach (XmlNode item in itemDoc.DocumentElement.SelectNodes("//Item[@Quantity != '0']"))
            {
                foreach (DataRow row in ((DataTable)StaticData.Tables[TN.InstallationItemCat]).Rows)
                {
                    if (item.SelectSingleNode("@Code").Value.ToString().ToUpper() == Convert.ToString(row[CN.Code]))
                    {
                        hasInstallation = true;
                        break;
                    }
                }

            }
            XmlNodeList warranties = itemDoc.DocumentElement.SelectNodes("//Item[@ContractNumber != '' and @Quantity != '0']");
            //code reinstated - required for GeneratePTAccountNumber() routine  jec 01/12/10  CR1030 
            ptWarranties = PaidAndTaken && (((warranties.Count > 0) && (bool)Country[CountryParameterNames.WarrantyCustomerDetails])
                           || TermsType == TT.PTWarranty);

            if (PaidAndTaken && (warranties.Count > 0 && ((bool)Country[CountryParameterNames.WarrantyCustomerDetails] || WarrantiesOnCredit)       // #17097
                && (!this.Replacement)
                || hasInstallation))
            {
                btnPrintReceipt.Enabled = false;
                menuPrintReceipt.Enabled = false;

                //IP - 17/04/08 - (69630)
                //Set the CustomerID back to blank if warranty attached.
                if (CustomerID == ptCustomerID)
                    CustomerID = String.Empty;

                custidrequested = true;
            }
            else
            {
                if (PaidAndTaken)
                {
                    btnPrintReceipt.Enabled = true;
                    menuPrintReceipt.Enabled = true;
                    custidrequested = false;
                    btnCustomerSearch.Visible = false;
                    //txt_linkedcustid.Visible = false;
                    //lbl_linkedcust.Visible = false;
                    //txt_linkedcustid.Text = "";
                    //custidCG = "";                            //IP - 23/12/10 - bug #2751 - if linked to a Customer then we do not want to set linked customer id to blank
                    //CustomerID = "PAID & TAKEN";

                    //IP - 23/12/10 - bug #2751 - only if the Cash & Go is not linked to a Customer do we then set the following.
                    if (this.custidCG == string.Empty)
                    {
                        txt_linkedcustid.Visible = false;
                        lbl_linkedcust.Visible = false;
                        txt_linkedcustid.Text = "";
                        CustomerID = "PAID & TAKEN";

                        //Need to set the account number back to Paid & Taken account number as this may have been populated with a special account number
                        //if a warranty was added where customer details were required (linked to a Customer).
                        if (txtAccountNumber.Text.Replace("-", "") != _acctNo)
                        {
                            txtAccountNumber.Text = _acctNo;
                            PTWarrantyAccountNo = string.Empty; //Need to set this to blank as we want the sale to be processed with the Paid & Taken account
                        }

                    }
                }
            }

            //// block of Code reinstated for Cash & Go (x.Count replaced with warranties.Count) due to change in XmlNodeList above  - jec 01/12/10
            //ptWarranties = PaidAndTaken && (((warranties.Count > 0) && (bool)Country[CountryParameterNames.WarrantyCustomerDetails])
            //               || TermsType == TT.PTWarranty);

            ////IP - 17/04/08 - Changed according to what was in version (4.2.1.20)
            ////if (PaidAndTaken && (WarrantiesOnCredit || ptWarranties))
            //if (PaidAndTaken && (WarrantiesOnCredit || ptWarranties) &&
            //    (CustomerID.Length == 0 || CustomerID.Equals("PAID & TAKEN")) && (!this.Replacement)) //IP - 28/04/08 - UAT(287) v 5.1, if performing an instant replacement do not set customer details.
            //{
            //    // Allow customer details to be entered for the warranty
            //    //btnCustomerSearch.Visible = true;
            //    //btnCustomerSearch.Enabled = true;
            //    //menuCustomerSearch.Visible = true;
            //    //menuCustomerSearch.Enabled = true;
            //    //IP - 14/03/08 - (69630)
            //    //Disable the 'Print Receipt' button and 'Print Tax Invoice' menu option if this is a Paid & Taken
            //    //with a warranty, as this should only be re-enabled once a 
            //    //customer has been linked.
            //    btnPrintReceipt.Enabled = false;
            //    menuPrintReceipt.Enabled = false;

            //    //IP - 17/04/08 - (69630)
            //    //Set the CustomerID back to blank if warranty attached.
            //    if (CustomerID == ptCustomerID)
            //        CustomerID = String.Empty;

            //    custidrequested = true;
            //}

            //// End of reinstated code


            //    // Make sure a new account number has been generated
            //    /*if (PTWarrantyAccountNo.Length == 0)
            //    {
            //        AccountManager.GenerateAccountNumber(
            //            Config.CountryCode, 
            //            Convert.ToInt16((string)drpBranch.SelectedItem),
            //            AT.Special,
            //            false,
            //            out _PTWarrantyAccountNo,
            //            out Error);

            //        if (Error.Length > 0)
            //            ShowError(Error);
            //        else
            //        {
            //            if (_PTWarrantyAccountNo == "000-0000-0000-0")
            //                ShowInfo("M_ACCOUNTNOFAILED");
            //            else
            //                _PTWarrantyAccountNo = _PTWarrantyAccountNo.Replace("-","");
            //        }
            //        CustomerID = "";*/

            //    // Generate an agreement no other than 1 so that the special account
            //    // created can masquerade as a P&T account for IR warranties
            //    /*AgreementNo = AccountManager.GetBuffNo(Convert.ToInt16((string)drpBranch.SelectedItem), out Error);
            //        /if (Error.Length > 0) ShowError(Error);
            //    }*/
            //}
            //else
            //{
            //    if (PaidAndTaken)
            //    {
            //        btnPrintReceipt.Enabled = true;
            //        menuPrintReceipt.Enabled = true;
            //        custidrequested = false;
            //        btnCustomerSearch.Visible = false;
            //        txt_linkedcustid.Visible = false;
            //        lbl_linkedcust.Visible = false;
            //        txt_linkedcustid.Text = "";
            //        custidrequested = false;
            //        custidCG = "";
            //        CustomerID = "PAID & TAKEN";
            //    }
            //}

        }

        // 5.1 uat89 return if is loyalty club
        private bool SetupPrivilegeClub(string custid)
        {
            bool isMember = false;
            bool termsTypeValid = false;

            // Check whether eligible for a privilige club discount
            if (custid.Trim().Length > 0)
            {
                // 5.1 uat253 was this termstype valid when the account opened?
                if ((bool)Country[CountryParameterNames.LoyaltyCard])
                {
                    CustomerManager.IsPrivilegeMemberWithValidTermsType(custid, TermsType, DateAccountOpened, out isMember, out customerPClubCode, out privilegeClubDesc, out hasPClubDiscount, out Error, out termsTypeValid);
                    //CustomerManager.IsPrivilegeMember(custid,  out isMember, out customerPClubCode, out privilegeClubDesc, out hasPClubDiscount, out Error);
                }

                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    // Filter the terms types
                    //  we only want to run this code if they are a member
                    // 5.1 uat253 was this termstype valid when the account opened?
                    if (isMember && termsTypeValid)
                    {
                        DataView dvTermsType = (DataView)drpTermsType.DataSource;

                        // Get the current setting
                        string curTermsType = drpTermsType.Text;

                        // 5.1 uat89 loyalty club not feeding in to generate default terms type
                        // try setting scoringBand to this value
                        _currentBand = customerPClubCode;
                        _scoringBand = customerPClubCode;
                        _origScoringBand = customerPClubCode;

                        FilterTermsType(ref dvTermsType, _affinityTerms, drpAccountType.Text, _scoringBand, SType, IsLoan);

                        // 5.1 uat89 display error message if no terms types match 
                        if (drpTermsType.Items.Count == 1)
                        {
                            ShowError("This customer qualifies for loyalty club '" + _scoringBand + "'.  Please set up at least one terms type for this.");
                        }
                        else
                        {
                            // Make sure the TermsType has not changed if it is still available
                            int selectedTermsType = drpTermsType.FindStringExact(curTermsType);
                            // 5.1 uat89 loyalty club not feeding in to generate default terms type
                            //drpTermsType.SelectedIndex = (selectedTermsType != -1) ? selectedTermsType : 0;
                            if (selectedTermsType < 1)
                            {
                                // todo set terms type to the lowest available %
                                drpTermsType.SelectedIndex = 1;
                            }
                        }
                    }

                    // Set up any discount
                    if (hasPClubDiscount)
                    {
                        lPrivilegeClubDesc.Text = privilegeClubDesc + " " + GetResource("T_DISCOUNT");
                        lPrivilegeClubDesc.Visible = true;
                        txtPrivClubDiscount.Visible = true;
                        txtPrivClubDiscount.Text = "";
                    }
                    else
                    {
                        lPrivilegeClubDesc.Visible = false;
                        txtPrivClubDiscount.Visible = false;
                    }
                }
            }

            return (isMember && termsTypeValid);
        }

        public void LinkCustomer(string custId)
        {
            // The customer search and details screens must call this setup when a customer is associated
            CustomerID = custId;

            SetupPrivilegeClub(custId);

            CalculateServiceCharge(Config.CountryCode,
                Convert.ToDecimal(StripCurrency(txtDeposit.Text)),
                Convert.ToDecimal(txtNoMonths.Value),
                Convert.ToDecimal(StripCurrency(txtSubTotal.Text)) - Convert.ToDecimal(StripCurrency(txtSalesTax.Text)),
                Convert.ToDecimal(numPaymentHolidays.Value));
        }

        private bool AddPayMethod(Decimal payValue, string bankCode, string bankAccountNo,
            string chequeNo, short curPayMethod, string description)
        {
            // CR586 Allow multiple pay methods held as a list
            bool status = true;

            // Important for this routine to clear the amount to zero below
            // so that if the same event fires again we do not keep adding
            // the same payment to the list.
            if ((payValue != 0) || (instantReplacement != null && MoneyStrToDecimal(txtReplacementValue.Text) == MoneyStrToDecimal(txtSubTotal.Text)))
            {

                //#14439
                if (mtb_CardNo.Visible == true && mtb_CardNo.Text.Length == 0 && PayMethod.IsPayMethod(Convert.ToInt16(((DataRowView)drpPayMethod.SelectedItem)[CN.Code].ToString()), PayMethod.StoreCard))
                {
                    errorProviderStoreCard.SetError(btnStoreCardManualEntry, "Please enter a valid storecard.");
                    status = false;
                }
                else
                {
                    errorProviderStoreCard.SetError(btnStoreCardManualEntry, "");
                }

                // When paying by cheque the Bank Account number must be entered
                if (bankAccountNo.Trim().Length == 0
                    && PayMethod.IsPayMethod(curPayMethod, PayMethod.Cheque))
                {
                    errorProvider1.SetError(txtBankAcctNo, GetResource("M_ENTERMANDATORY"));
                    status = false;
                }
                else
                {
                    errorProvider1.SetError(txtBankAcctNo, "");
                }

                // When paying by cheque/card the cheque/card number must be entered
                if (chequeNo.Trim().Length == 0 && txtChequeNo.Visible == true          //IP - 25/11/10 - Store Card
                    && PayMethod.IsPayMethod(curPayMethod, PayMethod.Cheque))

                //|| PayMethod.IsPayMethod(curPayMethod, PayMethod.CreditCard)      //IP - 26/01/11 - Sprint 5.9 - #2785 - Commented out as checked below.
                //|| PayMethod.IsPayMethod(curPayMethod, PayMethod.DebitCard)))
                {
                    errorProvider1.SetError(txtChequeNo, GetResource("M_ENTERMANDATORY"));
                    status = false;
                }
                else
                {
                    errorProvider1.SetError(txtChequeNo, "");
                }

                //IP - 26/01/11 0 Sprint 5.9 - #2785
                // Bug #3044
                if (PayMethod.IsPayMethod(curPayMethod, PayMethod.CreditCard) || PayMethod.IsPayMethod(curPayMethod, PayMethod.DebitCard) &&
                    mtb_CardNo.Visible == true)
                {
                    if (chequeNo.Trim().Length == 0)
                    {
                        errorProvider1.SetError(mtb_CardNo, GetResource("M_ENTERMANDATORY"));
                        status = false;
                    }
                    else if (!mtb_CardNo.MaskCompleted)
                    {
                        errorProvider1.SetError(mtb_CardNo, GetResource("M_INCOMPLETECARDNO"));
                        status = false;
                    }
                    else
                    {
                        errorProvider1.SetError(mtb_CardNo, "");
                    }
                }
                else
                {
                    errorProvider1.SetError(mtb_CardNo, "");
                }


                if (status)
                {
                    if (storeCardPayInfo != null)
                    {
                        storeCardPayInfoList.Add(storeCardPayInfo);
                    }

                    payMethodSet.Tables[TN.PayMethodList].Rows.Add(
                        new Object[] { payValue, bankCode, bankAccountNo, chequeNo, curPayMethod, description });

                    // Clear amount for next entry
                    txtAmount.Text = (0).ToString(DecimalPlaces);
                    //drpPayMethod.SelectedIndex = 0;
                    drpBank.SelectedIndex = 0;
                    txtBankAcctNo.Text = "";
                    txtChequeNo.Text = "";

                    //IP - 26/01/11 - Sprint 5.9 - #2785
                    if (mtb_CardNo.Visible == true && !PayMethod.IsPayMethod(Convert.ToInt16(((DataRowView)drpPayMethod.SelectedItem)[CN.Code].ToString()), PayMethod.StoreCard))
                    {
                        mtb_CardNo.ResetText();
                    }
                    // Enable remove btn
                    btnRemovePaymethod.Enabled = true;
                    SetAmountEnabled();
                }
            }
            return status;
        }

        private void SetAmountEnabled()
        {
            // Disable amount field and add btn when fully paid.
            // This ensures any local change is against last pay method
            bool fullyPaid = (PayMethodTotal() >= MoneyStrToDecimal(txtDue.Text));
            txtAmount.Enabled = !fullyPaid;
            if (!CashAndGoReturn)
                drpPayMethod.Enabled = !fullyPaid;
            if (!drpPayMethod.Enabled && !CashAndGoReturn)
                drpPayMethod.Enabled = !(instantReplacement == null);
            if (!chxGiftVoucher.Checked) chxGiftVoucher.Enabled = !fullyPaid;
            btnAddPaymethod.Enabled = !fullyPaid;
        }

        private decimal PayMethodTotal()
        {
            decimal totalAmount = 0;
            if (payMethodSet.Tables.Count > 0)
            {
                foreach (DataRow row in payMethodSet.Tables[TN.PayMethodList].Rows)
                {
                    totalAmount = totalAmount + System.Convert.ToDecimal(row[CN.Value]);
                }
            }
            return totalAmount;
        }

        /// <summary>
        /// Generate an account number of the right type.
        /// Disable the dropdown
        /// optionally enable the COD checkbox
        /// If this is an RF account or a manual sale don't
        /// generate an account number
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void drpAccountType_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            _hasdatachanged = true;
            if (staticLoaded && drpAccountType.SelectedIndex != 0 && !supressEvents && AccountType != "")
            {
                // Filter the terms type for the account type
                DataView dvTermsType = (DataView)drpTermsType.DataSource;

                // Get the current setting
                string curTermsType = drpTermsType.Text;

                FilterTermsType(ref dvTermsType, _affinityTerms, drpAccountType.Text, _scoringBand, SType, IsLoan);

                // Make sure the TermsType has not changed if it is still available
                int selectedTermsType = drpTermsType.FindStringExact(curTermsType);
                drpTermsType.SelectedIndex = (selectedTermsType != -1) ? selectedTermsType : 0;

                if (AT.IsCashType(AccountType))
                {
                    if (PaidAndTaken)
                    {
                        gbPaidAndTaken.BringToFront();
                        gbPaidAndTaken.Visible = true;
                        gbHP.Visible = false;
                    }
                    else
                    {
                        Rectangle rect1 = new Rectangle(groupBox2.Location.X, groupBox2.Location.Y, groupBox2.Size.Width, 400);
                        Rectangle rect2 = new Rectangle(panel1.Location.X, panel1.Location.Y, panel1.Size.Width, 200);
                        gbHP.Visible = false;
                        groupBox2.Bounds = rect1;
                        panel1.Bounds = rect2;
                        chxCOD.Enabled = true;
                        label26.Text = "Total:";
                        gbPaidAndTaken.SendToBack();
                        gbPaidAndTaken.Visible = false;
                    }

                    PrintVouchers = true;
                }

                //Generate a new account number
                try
                {
                    //Has ScoreHPbefore been enabled for the selected branch
                    //bool ScoreHPbefore = false;                //UAT267   jec 16/08/07
                    foreach (DataRow row in ((DataTable)StaticData.Tables[TN.BranchNumber]).Rows)
                    {
                        if (row[CN.BranchNo].ToString() == (string)drpBranch.SelectedItem)
                        {
                            ScoreHPbefore = (bool)row[CN.ScoreHPbefore];
                            createRF = (bool)row[CN.CreateRFAccounts];         //CR903 jec 07/08/07
                            createCash = (bool)row[CN.CreateCashAccounts];     //CR903 jec
                            createHP = (bool)row[CN.CreateHPAccounts];         //CR903 jec 24/08/07
                            break;
                        }
                    }
                    //don't generate an account number for ready finance (or HP if ScoreHPbefore has been enabled CR903).
                    if ((AccountType != AT.ReadyFinance && AccountType != AT.HP) || (AccountType == AT.HP && ScoreHPbefore == false) ||
                         ManualSale)
                    {
                        Function = "BAccountManager::GenerateNewAccountNumber";
                        string newAccountNo = "";

                        XmlNode stampDuty = AccountManager.GenerateAccountNumber(Config.CountryCode,
                            //Convert.ToInt16(Config.BranchCode), 
                            Convert.ToInt16((string)drpBranch.SelectedItem),
                            AccountType,
                            ManualSale, 0,
                            out newAccountNo,
                            out Error);
                        if (Error.Length > 0)
                            ShowError(Error);
                        else
                        {
                            AccountLocked = true;
                            if (stampDuty != null)
                            {
                                stampDuty = itemDoc.ImportNode(stampDuty, true);
                                foreach (XmlNode child in stampDuty.ChildNodes)
                                {
                                    if (child.NodeType == XmlNodeType.Element && child.Name == Elements.Item)
                                        itemDoc.DocumentElement.AppendChild(child);
                                }
                                populateTable();
                            }
                            if (newAccountNo == "000-0000-0000-0" && !ManualSale)
                            {
                                ShowInfo("M_ACCOUNTNOFAILED");
                            }
                            else
                            {
                                txtAccountNumber.Text = newAccountNo;
                                //CR903 Find the account branch's store type
                                SType = FindStoreType(txtAccountNumber.Text);
                                drpAccountType.Enabled = false;
                                drpBranch.Enabled = Credential.HasPermission(CosacsPermissionEnum.NewSalesChangeBranch);

                                if (!ManualSale)
                                {
                                    groupBox2.Enabled = true;
                                    gbHP.Enabled = true;
                                }
                                else
                                {
                                    txtAccountNumber.Focus();
                                    txtAccountNumber.Enabled = true;         // #8269 jec 27/09/11
                                    drpSOA.Enabled = true;         // #8269 jec 27/09/11
                                    drpSoldBy.Enabled = true;      // #8269 jec 27/09/11
                                    btnSave.Enabled = true;        // #8269 jec 27/09/11
                                    txtEmpNumber.Enabled = true;    // #8269 jec 28/09/11
                                }
                            }
                            menuSave.Enabled = !PaidAndTaken;
                            menuStock.Enabled = true;
                        }
                    }
                    else    //if this is an RF account
                    {
                        ShowInfo("M_CUSTOMERSEARCH");
                        ManualAccountKeepLock = true;
                        //launch the customer search screen so the user
                        //can select a customer to use
                        CustomerSearch cust = new CustomerSearch("", AccountType, false);
                        cust.FormRoot = this.FormRoot;
                        cust.FormParent = this;
                        ((MainForm)this.FormRoot).AddTabPage(cust, 9);
                    }

                    if (Renewal)
                    {
                        txtProductCode.Enabled = false;
                        drpLocation.Enabled = false;

                        SetUpWarrantyRenewals();
                    }
                }
                catch (Exception ex)
                {
                    Catch(ex, Function);
                }
            }
        }

        public override bool ConfirmClose()
        {
            bool mstatus = true;
            string invoice = "";
            int status = 0;
            try
            {
                Function = "ConfirmClose()";
                Wait();
                AccountManager.DisableSave(AccountNo, out invoice, out status);

                if (status == 0)
                {
                    
                    return true;


                }
                else if (status ==1 && invoice == "NewMambu")
                {

                    return true;


                }


                if (AccountLoaded && AccountLocked)
                {
                    if (!Cancelled && !PaidAndTaken && CustomerID.Length > 0)
                    {
                        if (_hasdatachanged && !RenewalRevision)    /* JJ don't even ask them if they want to save it if it hasn't changed */
                        {
                            if(_closeSaleWithoutSaving && isMmiAppliedForSaleAtrr)
                            {
                                _saveButtonClicked = false;
                                if (FormParent.GetType().Name.Equals("BasicCustomerDetails"))
                                    ((BasicCustomerDetails)FormParent).btnRefresh_Click(null, null);
                            }
                            // 14/11/07 rdb if user clicked save supress dialog
                            else if (_saveButtonClicked || DialogResult.Yes == ShowInfo("M_SAVECHANGES", MessageBoxButtons.YesNo))
                            {
                                //reset if fail to close
                                _saveButtonClicked = false;

                                if (!SaveAccount()) /* unable to save for some reason */
                                {
                                    mstatus = DialogResult.Yes == ShowInfo("M_CANTSAVEACCOUNT", MessageBoxButtons.YesNo);
                                }
                                else
                                {
                                    if (FormParent.GetType().Name.Equals("BasicCustomerDetails"))
                                        ((BasicCustomerDetails)FormParent).btnRefresh_Click(null, null);
                                }
                            }
                        }
                    }
                    if (mstatus && !PaidAndTaken && !ManualAccountKeepLock) //if doing manual account keep lock.                    {
                    {
                        AccountManager.UnlockAccount(AccountNo, Credential.UserId.ToString(), true, out Error);
                        if (Error.Length > 0)
                        {
                            ShowError(Error);
                            mstatus = false;
                        }
                    }

                    if (mstatus && !PaidAndTaken)
                    {
                        AccountManager.UnlockItem(Credential.UserId, out Error);
                        if (Error.Length > 0)
                        {
                            ShowError(Error);
                            mstatus = false;
                        }
                    }
                }

                // Livewire 69106 JH 23/05/2007
                // Cancel account if it hasn't been linked to an account
                // 5.1 uat117 rdb 12/11/2007 added _cashAndGo test
                // 5.1 UAT129 RDB 19/11/2007 added acctno test
                // (no account number is generated when automatic number generation not enabled)
                if (CustomerID == String.Empty && !_cashAndGo && AccountNo != "000000000000" && !ManualAccountKeepLock && !Renewal)
                {
                    AccountManager.CancelAccount(this.AccountNo, this.CustomerID, Convert.ToInt16(Config.BranchCode),
                                (string)Country[CountryParameterNames.CancellationRejectionCode],
                                0, Config.CountryCode, "", 0, out Error);
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
                // rdb 6/11/07 during merge this existing code now causes
                // controls to clear from screen, will add if status to only
                // dispose if screen closing

                if (mstatus)
                {
                    Dispose(true);

                    //If dispose is called already then say GC to skip finalize on this instance.
                    GC.SuppressFinalize(this);
                }
            }
            return mstatus;
        }

        /// <summary>
        /// Load up the search by location screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItem14_Click(object sender, System.EventArgs e)
        {
            ((MainForm)FormRoot).StockEnquiryByLocation.FormRoot = FormRoot;
            ((MainForm)FormRoot).StockEnquiryByLocation.FormParent = this;
            txtQuantity.Text = "1";     //default to 1
            txtQuantity.Select(0, txtQuantity.Text.Length);
            txtQuantity.Focus();    //has to be done while the page is visible
            ((MainForm)this.FormRoot).AddTabPage(((MainForm)FormRoot).StockEnquiryByLocation);
        }

        /// <summary>
        /// This method validates the product code entered by the user.
        /// If the location field is empty, get a list of available locations
        /// for this product and populate dropdown.
        /// If location field is populated call the GetItemDetails method
        /// and either populate the other fields or report that the 
        /// product does not exist at that location.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtProductCode_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ValidateProdcode();
        }

        public void ValidateProdcode()
        {

            cbImmediate.Enabled = true;
            _hasdatachanged = true;
            Function = "txtProductCode_Validating";
            var repoitem = false;

            try
            {
                //  var items = new List<WSStock.StockInfo>(StockManager.GetStockInfo(txtProductCode.Text, repoitem, itemId, (isLegacyPTReturn || Renewal), out Error));

                Wait();
                //needs to Be upper case
                txtProductCode.Text = txtProductCode.Text.ToUpper();
                //CR906 rdb 11/09/07 do not allow loan items to be included on non loan accounts
                //CR906 This code needs to be run from the txtQuantity and txtValue Validating events as well JH 25/09/2007

                if (txtProductCode.Text.Length > 6 && txtProductCode.Text.Substring(0, 1) == "R" && !Renewal)   //#19470
                {
                    txtProductCode.Text = txtProductCode.Text.Substring(1, txtProductCode.Text.Length - 1);
                    repoitem = true;
                }


                if (txtProductCode.Text == "LOAN" && !IsLoan)
                {
                    SetLoanError();
                }
                else
                {
                    if (ValidateNonGiftItem() == false)
                        return;

                    var itemId = (int?)null;
                    var itemIdvalue = 0;

                    //CR906 we can only allow LOAN items and gift items to be added here
                    //CR906 This code needs to be run from the txtQuantity and txtValue Validating events as well JH 25/09/2007
                    if ((drpLocation.Text.Length == 0 || PaidAndTaken) && txtProductCode.Text.Length > 0 /*&& !Renewal*/)
                    {
                        //#17528
                        if (Renewal)
                        {
                            itemId = int.TryParse(Convert.ToString(warrantyRenewals.Rows[0]["newwarrantyid"]), out itemIdvalue) == true ? itemIdvalue : (int?)null;
                        }

                        var items = new List<WSStock.StockInfo>(StockManager.GetStockInfo(txtProductCode.Text, repoitem, itemId, (isLegacyPTReturn || Renewal), out Error));
                        var CheckForNonStock = items.FindAll(x => x.itemtype.Equals("N"));

                        if (Error.Length > 0)
                        {
                            ShowError(Error);
                            return;
                        }

                        if (items.Count == 1)
                        {
                            ItemID = items[0].Id;
                            txtProductCode.Text = items[0].IUPC;  //if the barcode was entered
                        }
                        else if (items.Count > 1 && ItemID == null) //IP - 20/05/11 - CR1212 - check ItemID is null as we do not want to call Stock Enquiry By Product if item has already beem selected from thsi screen
                        {
                            //IP - 16/05/11 - #3626 - If the SKU has been entered and more than one item returned (IUPC different) then load the
                            //Stock Enquiry By Product screen.

                            var mainForm = FormRoot as MainForm;
                            var form = mainForm.GetIfExists<CodeStockEnquiry>();

                            if (form == null)
                            {
                                form = new CodeStockEnquiry();
                                mainForm.AddTabPage(form);
                            }

                            form.ItemNo = txtProductCode.Text;
                            form.FormParent = this;
                            form.FormRoot = this.FormRoot;

                            mainForm.FocusIfExists<CodeStockEnquiry>(form);

                            txtQuantity.Select(0, txtQuantity.Text.Length);
                            txtQuantity.Focus();
                        }

                        if (chxNonStock.Checked && !(CheckForNonStock.Count > 0))
                        {
                            errorProvider1.SetError(txtProductCode, "Product is not NonStock");
                            return;
                        }
                        else
                        {
                            cbImmediate.Checked = true;
                            cbImmediate.Enabled = false;

                            if (chxNonStock.Checked)
                            {
                                DataSet ds = AccountManager.ValidateNonSaleableNonStocks(txtProductCode.Text);
                                if (ds.Tables["AccountDetails"].Rows.Count > 0)
                                {
                                    errorProvider1.SetError(txtProductCode, "Product " + ds.Tables["AccountDetails"].Rows[0]["SKU"] + " is nonsaleable");
                                    return;
                                }
                            }
                        }

                        if (ItemID != null)
                        {
                            var locations = new List<short>();
                            locations.AddRange(StockManager.GetStockLocation(ItemID.Value, out Error)); //IP - 17/02/10 - CR1072 - LW 71731 - Cash&Go Fixes from 4.3 - added isLegacyPTReturn

                            drpLocation.DataSource = locations;
                            drpLocation.Enabled = true;

                            if (Error.Length > 0)
                            {
                                ShowError(Error);
                                return;
                            }


                            if (locations.Count == 0)
                            {
                                if (repoitem == true) txtProductCode.Text = "R" + txtProductCode.Text;
                                throw new NullReferenceException("No product found. If trying to add warranty or installation right click on tree below to add to parent item");
                            }

                            if (PaidAndTaken && !locations.Contains(Config.BranchCode.TryParseInt16(0).Value))
                                throw new NullReferenceException("This product cannot be found at this location");
                        }
                        else if (items.Count == 0)          // RI jec
                        {
                            errorProvider1.Clear();
                            errorProvider2.Clear();
                            errorProvider1.SetError(txtProductCode, "Product not found");

                        }
                    }

                    if (PaidAndTaken)
                        drpLocation_Validating(this, null);
                }

            }



            catch (NullReferenceException ex)
            {
                txtProductCode.Focus();
                txtProductCode.Select(0, txtProductCode.Text.Length);
                errorProvider1.SetError(txtProductCode, ex.Message);
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
            }
        }

        /// <summary>
        /// Sets an error provider for the txtProductCode text field
        /// </summary>
        private void SetLoanError()
        {
            txtProductCode.Text = String.Empty;
            errorProvider1.SetError(txtProductCode, "Cash loan cannot be applied to non-loan accounts.");
            btnEnter.Enabled = false;
        }


        /// <summary>
        /// Determines if the selected product is a gift item and if it isn't sets an error provider for a cash loan account
        /// </summary>
        /// <returns></returns>
        private bool ValidateNonGiftItem()
        {
            bool result = true;

            if (!IsLoan || txtProductCode.Text == "LOAN")
                return result;

            string locationCode = drpLocation.SelectedText != "" ? drpLocation.SelectedText : Config.BranchCode;
            string err = string.Empty;
            if (txtProductCode.Text != "" && !AccountManager.IsGiftItem(Convert.ToInt32(ItemID), locationCode, out err))     // RI
            {
                result = false;
                txtProductCode.Text = "";
                btnEnter.Enabled = false;
                errorProvider1.SetError(txtProductCode, "Cash loan accounts can only have gift items added.");
            }
            return result;
        }

        /// <summary>
        /// This method validates the product code and location combination
        /// by retrieving the item details as an xml node
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void drpLocation_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!Redemption)
            {
                drpLocationValidation();
            }
        }

        private void drpLocationValidation()
        {
            try
            {
                if (drpLocation.Text.Length == 0)
                    return;

                Wait();
                Function = "drpLocation_Validating";
                short branch = 0;
                short accountBranch;
                short stocklocn = 0;
                SetDeliveryAreaList(drpLocation.Text);
                // 5.1 uat331 rdb 18/12/07 ref:69416 caused new error in Cash&Go
                // drpLocation is not used here so when C&G use branch
                if (PaidAndTaken)
                {
                    branch = Convert.ToInt16(drpBranch.SelectedItem.ToString());
                    accountBranch = branch;
                    stocklocn = branch;
                }
                else
                {
                    stocklocn = Convert.ToInt16(drpLocation.Text);
                    branch = Convert.ToInt16(Config.BranchCode);
                    accountBranch = Convert.ToInt16(drpLocation.SelectedItem.ToString());
                }

                currentItem = AccountManager.GetItemDetails(new STL.PL.WS2.GetItemDetailsRequest
                {
                    ProductCode = txtProductCode.Text,
                    StockLocationNo = stocklocn,
                    BranchCode = branch,
                    AccountType = AccountType,
                    CountryCode = Config.CountryCode,
                    IsDutyFree = chxDutyFree.Checked,
                    IsTaxExempt = chxTaxExempt.Checked,
                    PromoBranch = Convert.ToInt16(Config.BranchCode),
                    AccountBranch = accountBranch,
                    ItemID = ItemID ?? 0
                },
                out Error);

                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    currentItem = itemDoc.ImportNode(currentItem, true);
                    SetItemDetails(currentItem);

                    if ((txtUnitPrice.Text == "0.00") && (chxNonStock.Checked))
                    {
                        txtUnitPrice.ReadOnly = false;
                        txtValue.ReadOnly = false;
                    }

                    //CR735 - the del note branch will be set in the stored procedure,
                    // DN_ItemGetDetailsSP, to one of the following:
                    //	1)default del note branch for item category
                    //	2)default del note branch for country
                    //	3)item stock location
                    //int index = drpBranchForDel.FindString(drpLocation.Text);
                    //if(index!=-1 && (decimal)Country[CountryParameterNames.DefaultDeliveryNoteBranch] == 0)
                    //	drpBranchForDel.SelectedIndex = index;

                    //if(currentItem.Attributes[Tags.Type].Value==ItemTypes.Affinity.ToString())
                    txtQuantity.Text = "1";     //default to 1
                    txtQuantity.Select(0, txtQuantity.Text.Length);

                    if (DisplaySpiffs)
                        ListSpiffs(txtProductCode.Text, branch);
                }

                //IP - 25/02/10 - CR1072 - Malaysia 3PL for Version 5.2 --Merged from v4.3
                // CR 04/08/08 rdb Malaysia 
                // set delPrintNote selected value to defaultPrintLocation here
                // unless stocklocation is the current branch
                //if (thirdPartyDeliveriesEnabled && !scheduleDelEnabled)
                if ((thirdPartyDeliveriesEnabled && !scheduleDelEnabled) || !thirdPartyDeliveriesEnabled)           //IP - 25/05/12 - #10225 - Warehouse & Deliveries Integration
                {
                    DefaultPrintLocation = true;
                    //if (stocklocn != Convert.ToInt32(Config.BranchCode))
                    if ((stocklocn != Convert.ToInt32(Config.BranchCode) && thirdPartyDeliveriesEnabled) || !thirdPartyDeliveriesEnabled)       //IP - 25/05/12 - #10225 - Warehouse & Deliveries Integration
                    {
                        WS2.BranchDefaultPrintLocation details = _printBranches.Find(delegate (WS2.BranchDefaultPrintLocation item) { return item.bn == stocklocn; });
                        if (details != null)
                        {
                            drpBranchForDel.SelectedIndex = details.dpl == 0 ? drpBranchForDel.FindString(Config.BranchCode) : drpBranchForDel.FindString(details.dpl.ToString());
                            // CR 953 Warn the user that no default print location has been set up for this stock location
                            if (details.dpl == 0)
                            {
                                DefaultPrintLocation = false;
                                if (thirdPartyDeliveriesEnabled)                        //IP - 25/05/12 - #10225 - Warehouse & Deliveries Integration
                                {
                                    ShowWarning("No default print location has ben set up for this stock location.");
                                }
                                else
                                {
                                    ShowWarning("No default delivery location has ben set up for this stock location.");    //IP - 25/05/12 - #10225 - Warehouse & Deliveries Integration
                                }
                            }
                        }
                    }
                    else
                    {
                        //If the Location equals the current logged in branch, then this is an immediate delivery.
                        //Therefore default to immediate delivery.
                        cbImmediate.Checked = true;
                        cbImmediate.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
            }
        }

        /// <summary>
        /// This event handler ensures that the product details are set to 
        /// blank each time the product code text box is edited. This is to
        /// ensure that the correct item has been retrieved from the database
        /// at all times.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void txtProductCode_TextChanged(object sender, System.EventArgs e)
        //{
        //    this._hasdatachanged = true;
        //    Function = "txtProductCode_TextChanged";
        //    try
        //    {
        //        ClearItemDetails();
        //    }
        //    catch (Exception ex)
        //    {
        //        Catch(ex, Function);
        //    }
        //}

        /// <summary>
        /// This handler validates the quantity entered by the user.
        /// They must enter more than zero. It will also enable the value text box if the
        /// item is value controlled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtQuantity_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            bool status = true;

            percentage = false;     //RI jec 31/5/11

            Function = "txtQuantity_Validating";
            try
            {
                // 5.1 uat150 16/11/07 rdb if no item selected dont bother validating
                if (txtProductCode.Text.Trim() == string.Empty)
                    status = false;

                if (status && txtQuantity.Text.IndexOf(".") > 0) // Check if decimal quantities are allowed - 68500 (jec 11/09/06)
                {
                    bool validDecimal = AccountManager.ValidDecimal(ItemID ?? 0, out Error);  //68500 (jec 12/09/06)

                    if (Error.Length > 0)
                    {
                        ShowError(Error);
                    }
                    else if (!validDecimal)
                    {
                        status = false;
                        ShowInfo("M_DECIMALNOTVALID");
                        txtQuantity.Focus();
                    }
                }

                if (status)
                {
                    this.errorProvider1.SetError(this.txtQuantity, "");

                    double q = Convert.ToDouble(txtQuantity.Text);
                    double p = Convert.ToDouble(StripCurrency(txtUnitPrice.Text));
                    double a = Convert.ToDouble(txtAvailable.Text);

                    if (q <= 0)
                        throw new Exception(GetResource("M_QUANTITYTOOLOW"));

                    if (q > 10 && DialogResult.No == ShowInfo("M_AREYOUSURE", MessageBoxButtons.YesNo))
                        throw new Exception(GetResource("M_REENTERQUANTITY"));

                    if (!ValueControlled)
                        txtValue.Text = (q * p).ToString(DecimalPlaces);
                    else
                    {
                        if (q > 1)
                            txtQuantity.Text = "1";
                        txtValue.Focus();
                    }

                    if (currentItem != null)
                    {
                        /* update the available text box using the difference between the old
						 * qty and the new qty */
                        double oldQty = Convert.ToDouble(currentItem.Attributes[Tags.Quantity].Value);
                        double diff = q - oldQty;
                        a = Collection ? a + diff : a - diff;
                        txtAvailable.Text = a.ToString("F2");

                        //update the currentItem node quantity field with this new value
                        currentItem.Attributes[Tags.Quantity].Value = txtQuantity.Text;
                        currentItem.Attributes[Tags.Value].Value = this.StripCurrency(txtValue.Text);

                        //IP - 11/02/10 - CR1048 (Ref:3.1.2.2/3.1.2.3) Merged - Malaysia Enhancements (CR1072)
                        //If this is a Cash & Go and the DrpPayMethod is not populated, then we do not want to enable
                        //the enter button to add an item.
                        //btnEnter.Enabled = txtProductCode.Text.Length > 0 && drpLocation.Text.Length > 0;
                        if (PaidAndTaken && drpPayMethod.Items.Count == 0)
                        {
                            btnEnter.Enabled = false;
                        }
                        else
                        {
                            btnEnter.Enabled = txtProductCode.Text.Length > 0 && drpLocation.Text.Length > 0;
                        }

                        if (txtProductCode.Text == "LOAN" && !this.IsLoan)
                        {
                            SetLoanError();
                        }

                        ValidateNonGiftItem();
                    }
                }
            }
            catch (Exception ex)
            {
                //e.Cancel = true;
                txtQuantity.Focus();
                txtQuantity.Select(0, txtQuantity.Text.Length);
                errorProvider1.SetError(txtQuantity, ex.Message);
            }
        }

        private bool CheckAffinityTerm()
        {
            bool status = true;
            if (!AT.IsCreditType(AccountType))
                return status;

            // If there are any affinity items being sold on a non-Affinity
            // Terms Type then we must check the agreement term is not too long

            errorProvider1.SetError(txtNoMonths, "");
            errorProvider1.SetError(drpLengths, "");

            DataRowView row = (DataRowView)drpTermsType.SelectedItem;

            if ((string)row["Affinity"] != "Y")
            {
                // This is a non-Affinity Terms Type or the Terms Type is not set
                if (xml.findAffinities(itemDoc.DocumentElement))
                {
                    // An Affinity item is present so restrict the term
                    if (txtNoMonths.Value > (decimal)Country[CountryParameterNames.AffinityMaxTerm])
                    {
                        errorProvider1.SetError(txtNoMonths, GetResource("M_AFFINITYMAXTERM", new object[] { (decimal)Country[CountryParameterNames.AffinityMaxTerm] }));
                        errorProvider1.SetError(drpLengths, GetResource("M_AFFINITYMAXTERM", new object[] { (decimal)Country[CountryParameterNames.AffinityMaxTerm] }));
                        status = false;
                    }
                }
            }
            return status;
        }

        private bool AuthoriseDiscount(XmlNode discountItem, XmlNode linkItem)
        {
            if (linkItem.Attributes == null) // There is no linked stock item
                return true;

            if (drpSoldBy.SelectedIndex == 0)
            {
                errorProvider1.SetError(drpSoldBy, GetResource("M_ENTERMANDATORY"));
                drpSoldBy.Focus();
                return false;
            }
            else
            {
                errorProvider1.SetError(drpSoldBy, "");
            }

            // The value of the discount must not be more than the
            //// allowed percentage of the value of the linked stock item
            //decimal dsVal = 0;
            //if (percentage && !accountWideDiscount) //IP - CR1212 - RI - #2315
            //{
            //    // RI remove % sign and calc percentage value
            //    discountItem.Attributes[Tags.Value].Value = discountItem.Attributes[Tags.Value].Value.Replace("%", "");
            //    dsVal = -Convert.ToDecimal(discountItem.Attributes[Tags.Value].Value) / 100 * Convert.ToDecimal(linkItem.Attributes[Tags.Value].Value);
            //    discountItem.Attributes[Tags.Value].Value = Convert.ToString(-dsVal);
            //    currentItem.Attributes[Tags.Value].Value = Convert.ToString(-dsVal);
            //    currentItem.Attributes[Tags.UnitPrice].Value = Convert.ToString(-dsVal);
            //}
            //else
            //{
            //    dsVal = -Convert.ToDecimal(discountItem.Attributes[Tags.Value].Value);
            //}

            decimal dsVal = 0;
            decimal dsTax = 0;                                                                                                       //IP - 12/09/11 - RI - #8132
            decimal newVal = 0;                                                                                                      //IP - 12/09/11 - RI - #8132

            decimal itemVal = Convert.ToDecimal(linkItem.Attributes[Tags.Value].Value);
            decimal itemTax = 0;                                                                                                     //IP - 02/04/12 - #8613 - LW74242

            if ((string)Country[CountryParameterNames.AgreementTaxType] == "E")                                                      //IP - 02/04/12 - #8613 - LW74242
            {
                itemTax += AccountManager.CalculateTaxAmount(Config.CountryCode, linkItem, chxTaxExempt.Checked, out Error);
                if (Error.Length > 0)
                {
                    ShowError(Error);
                    return false;
                }
            }

            itemVal += itemTax;

            if (discountItem.Attributes[Tags.Value].Value.Contains("%"))
            {

                currentItem.Attributes[Tags.TaxAmount].Value = Convert.ToString(0);                                                 //#14751 

                //dsVal = -Convert.ToDecimal(discountItem.Attributes[Tags.Value].Value.Replace("%", "")) / 100 * Convert.ToDecimal(linkItem.Attributes[Tags.Value].Value);
                dsVal = -Convert.ToDecimal(discountItem.Attributes[Tags.Value].Value.Replace("%", "")) / 100 * (Convert.ToDecimal(linkItem.Attributes[Tags.Value].Value) + itemTax);        //IP - 02/04/12 - #8613 - LW74242

                //discountItem.Attributes[Tags.Value].Value = Convert.ToString(-dsVal);
                //discountItem.Attributes[Tags.UnitPrice].Value = Convert.ToString(-dsVal);               //IP - 12/09/11 - RI - #8132

                var discountValue = -dsVal;                         //#15457
                discountItem.Attributes[Tags.Value].Value = StripCurrency(discountValue.ToString(DecimalPlaces));       //#15457
                discountItem.Attributes[Tags.UnitPrice].Value = StripCurrency(discountValue.ToString(DecimalPlaces));   //#15457              //IP - 12/09/11 - RI - #8132

                discountItem.Attributes[Tags.TaxRate].Value = linkItem.Attributes[Tags.TaxRate].Value;  //IP - 03/10/11 - #8132 - The taxrate of the discount should be taken from the item it is linked to

                dsTax = AccountManager.CalculateTaxAmount(Config.CountryCode, currentItem, chxTaxExempt.Checked, out Error);        //IP - 12/09/11 - RI - #8132 - First work out tax on the discount value

                //IP - 03/10/11 - RI - #8132 - Where agreement tax is exclusive
                if ((string)Country[CountryParameterNames.AgreementTaxType] == "E")
                {
                    newVal = Convert.ToDecimal(currentItem.Attributes[Tags.Value].Value);                                               //IP - 12/09/11 - RI - #8132
                    newVal -= dsTax;

                    currentItem.Attributes[Tags.Value].Value = StripCurrency(newVal.ToString(DecimalPlaces));                           //IP - 12/09/11 - RI - #8132 - reduce the discount value by tax on discount
                    currentItem.Attributes[Tags.UnitPrice].Value = StripCurrency(newVal.ToString(DecimalPlaces));                       //IP - 12/09/11 - RI - #8132
                }
                //currentItem.Attributes[Tags.Value].Value = Convert.ToString(-dsVal);
                //currentItem.Attributes[Tags.UnitPrice].Value = Convert.ToString(-dsVal);
                currentItem.Attributes[Tags.TaxAmount].Value = dsTax.ToString();                                                    //IP - 12/09/11 - RI - #8132 - update the tax amount for the discount

            }
            else
            {

                currentItem.Attributes[Tags.TaxAmount].Value = Convert.ToString(0);                                                 //#14751 

                dsVal = -Convert.ToDecimal(discountItem.Attributes[Tags.Value].Value);

                discountItem.Attributes[Tags.TaxRate].Value = linkItem.Attributes[Tags.TaxRate].Value;                              //IP - 05/10/11 - #8132 - The taxrate of the discount should be taken from the item it is linked to

                dsTax = AccountManager.CalculateTaxAmount(Config.CountryCode, currentItem, chxTaxExempt.Checked, out Error);        //IP - 05/10/11 - #8132 

                //IP - 05/10/11 - RI - #8132 - Where agreement tax is exclusive
                if ((string)Country[CountryParameterNames.AgreementTaxType] == "E")
                {
                    newVal = Convert.ToDecimal(currentItem.Attributes[Tags.Value].Value);
                    newVal -= dsTax;

                    currentItem.Attributes[Tags.Value].Value = StripCurrency(newVal.ToString(DecimalPlaces));
                    currentItem.Attributes[Tags.UnitPrice].Value = StripCurrency(newVal.ToString(DecimalPlaces));
                }

                currentItem.Attributes[Tags.TaxAmount].Value = dsTax.ToString();                                                    //IP - 12/09/11 - RI - #8132 - update the tax amount for the discount
            }

            //IP - 02/04/12 - #8613 - LW74242 - Moved to above
            //decimal itemVal = Convert.ToDecimal(linkItem.Attributes[Tags.Value].Value);
            //if ((string)Country[CountryParameterNames.AgreementTaxType] == "E")
            //{
            //    itemVal += AccountManager.CalculateTaxAmount(Config.CountryCode, linkItem, chxTaxExempt.Checked, out Error);
            //    if (Error.Length > 0)
            //    {
            //        ShowError(Error);
            //        return false;
            //    }
            //}


            if (Math.Abs(dsVal) > itemVal)    //IP - 02/04/12 - #8613 - LW74242
            {
                errorProvider1.SetError(btnEnter, "Discount value cannot be greater than the item you are attempting to discount");
                return false;
            }

            AuthorisePrompt auth = new AuthorisePrompt(this, overrideDiscountLimit, GetResource("M_DISCOUNTAUTH", new object[] { Country[CountryParameterNames.DiscountPercentage] }));
            decimal discountRate = (decimal)Country[CountryParameterNames.DiscountPercentage] / 100;

            //if (!acctWideDisAuthorised)     //IP - 16/04/12 - #8613
            if (!accountWideDiscount || (accountWideDiscount && acctWideDisRequireAuth))   //#13791
            {
                if (Math.Abs(dsVal) > (itemVal * discountRate))     //#13791
                    auth.ShowDialog();
                else
                    auth.Authorised = true;
            }
            else
            {
                if (accountWideDiscount && acctWideDisCancel)
                {
                    auth.Authorised = false;
                }
                else
                {
                    auth.Authorised = true;
                }
            }

            if (auth.Authorised)
            {
                //acctWideDisAuthorised = true;       // #11090 jec - auth all discounts  //IP - 09/06/11 - CR1212 - RI - #3817

                if (accountWideDiscount)            //#13791
                {
                    acctWideDisRequireAuth = false;
                }

                // Audit the discount authorisation
                string salesPersonName = drpSoldBy.SelectedItem.ToString();
                int salesPerson = Convert.ToInt32(salesPersonName.Substring(0, salesPersonName.IndexOf(":") - 1));

                AccountManager.AuditDiscount(
                    AccountNo,
                    AgreementNo,
                    discountItem.Attributes[Tags.Code].Value,
                    linkItem.Attributes[Tags.Code].Value,
                    Convert.ToInt16(discountItem.Attributes[Tags.Location].Value),
                    Convert.ToDecimal(discountItem.Attributes[Tags.Value].Value),
                    salesPerson,
                    auth.AuthorisedBy);
            }
            else
            {
                if (accountWideDiscount)        //#13791
                {
                    acctWideDisCancel = true;
                    acctWideDisRequireAuth = false;
                }
            }

            return auth.Authorised;
        }

        /// <summary>
        /// This method is called when the user adds a line item to the account.
        /// It will perform the appropriate checks to keep the underlying xml
        /// document in the correct state and then refresh the datagrid from the 
        /// document. 
        /// It will prompt the user to add warranties if any exist.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEnter_Click(object sender, System.EventArgs e)
        {
            try
            {
                Function = "btnEnter_Click";
                decimal newValue = 0;
                decimal valueTax = 0;
                _hasdatachanged = true;
                //10.6 CR- Sales Order - Print Save- Allow user to save while printing 
                //and track UI changes to increment invoice version
                _isSalesOrderChanged = true;
                string itemType = "";
                string foundType = "";
                bool status = true;
                bool discount = false;
                string accountNo = AccountNo;
                //bool callRelatedProducts = false; //IP - 28/02/11 - #3180 - made private to the class
                bool authorised = false;
                bool readyAssistItem = false;     //#18604 - CR15594
                itemType = currentItem.Attributes[Tags.Type].Value;
                readyAssistItem = bool.Parse(currentItem.Attributes[Tags.ReadyAssist].Value);     //#18604 - CR15594
                                                                                                  //double qty = Convert.ToDouble(txtQuantity.Text);
                int curPayMethod = Convert.ToInt16(((DataRowView)drpPayMethod.SelectedItem)[CN.Code].ToString()); //IP - 03/12/10 - Store Card



                errorProvider1.SetError(btnEnter, "");              //IP - 16/04/12 - #9909
                errorProvider2.SetError(txtProductCode, "");

                //Changes to validate itemno at the point of sale so that no poisons are generated later
                string CintMessage = string.Empty;
                int CintError = AccountManager.ValidateSaleForCINT(txtProductCode.Text, int.Parse(txtQuantity.Text), int.Parse(drpBranch.SelectedValue.ToString()), int.Parse(drpLocation.SelectedValue.ToString()), DateTime.UtcNow, out CintMessage);

                if (CintError > 0)
                {
                    var CintMessages = CintMessage.Replace("\\n", Environment.NewLine);
                    MessageBox.Show(CintMessages);
                    return;
                }

                //#14644 - Allow to increase qty // #14603 
                if (currentItem.Attributes[Tags.ScheduledQuantity].Value != "0" && !(Convert.ToInt32(currentItem.Attributes[Tags.Quantity].Value) > Convert.ToInt32(currentItem.Attributes[Tags.ScheduledQuantity].Value)))
                {
                    MessageBox.Show("This item has already been exported for delivery can not be changed");
                    status = false;
                }

                //IP - 26/07/11 - CR1254 - RI - #4405
                if (currentItem.Attributes[Tags.RepoItem].Value == "True")
                {
                    if (DialogResult.No == MessageBox.Show("You have selected a Repossessed item." + Environment.NewLine + Environment.NewLine + "Do you still want to add this item?", "Repossessed Item", MessageBoxButtons.YesNo))
                    {
                        status = false;
                    }
                }

                //#19146 - CR15594 - Pre
                if (bool.Parse(currentItem.Attributes[Tags.ReadyAssist].Value) == true && PaidAndTaken)
                {
                    MessageBox.Show("You cannot sell a Ready Assist product on a Cash & Go");
                    status = false;
                }
                //For KitItems. This is a hack. No point trying to fix this as this whole class should be rewritten.
                //If you are trying to fix this give up now and go home as this is the worlds biggest method.
                XmlNode kitWarranty = null;

                //IP - 06/06/11 - CR1212 - RI - #2327 - if this is an assembly item then display the available options
                if (currentItem.Attributes[Tags.Assembly].Value == "Y" && Convert.ToBoolean(Country[CountryParameterNames.DisplayAssemblyOptions]) == true && status) //IP - 18/06/11 - CR1212 - RI - #3960
                {
                    AssemblyOptions assembly = new AssemblyOptions(this, this.FormRoot);
                    assembly.ShowDialog();

                    var assemblyRequired = assembly.AssemblyRequired;

                    //If assembly is not required (Customer selected) then update the assembly attribute to N.
                    if (assemblyRequired == false)
                    {
                        currentItem.Attributes[Tags.Assembly].Value = "N";
                        cbAssembly.Checked = false;
                    }
                }


                // uat363 rdb get parentProductCode
                string parentProductCode = string.Empty;
                var parentItemId = 0;           // RI jec
                if (currentItem.ParentNode != null &&
                    currentItem.ParentNode.ParentNode != null &&
                    currentItem.ParentNode.ParentNode.Name == "Item")
                {
                    parentProductCode = currentItem.ParentNode.ParentNode.Attributes[Tags.Code].Value;
                    parentItemId = Convert.ToInt32(currentItem.ParentNode.ParentNode.Attributes[Tags.ItemId].Value);         // RI
                }

                // CR906 cash loans - todo add logic to check if loan exceeds maximum loan ammount
                if (txtProductCode.Text.ToUpper() == "LOAN")
                {
                    decimal maxLoanAmount = Convert.ToInt32(Country[CountryParameterNames.CL_MaxLoanAmount]);
                    if (Convert.ToDecimal(StripCurrency(txtValue.Text)) > maxLoanAmount)
                    {
                        ShowError("The maximum loan amount is: " + maxLoanAmount.ToString());
                        status = false;
                    }
                }

                if (!PaidAndTaken)
                {
                    if (cbImmediate.Checked == false)
                    {
                        //#12855
                        if (drpDeliveryArea.SelectedIndex == 0)
                        {
                            errorProvider1.SetError(drpDeliveryArea, CommonForm.GetResource("M_MANDATORYDELAREA"));
                            status = false;
                        }
                        else
                        {
                            errorProvider1.SetError(drpDeliveryArea, "");
                            status = true;
                        }
                    }
                }

                //IP - 25/02/10 - CR1072 - Malaysia 3PL for Version 5.2
                //if (thirdPartyDeliveriesEnabled && !scheduleDelEnabled)
                if ((thirdPartyDeliveriesEnabled && !scheduleDelEnabled) || !thirdPartyDeliveriesEnabled)           //IP - 25/05/12 - #10225 - Warehouse & Deliveries Integration
                {
                    // CR 953 Check for whether or not a default print location exists
                    //if (DefaultPrintLocation == false && drpBranch.Text != Config.BranchCode)
                    if (DefaultPrintLocation == false)
                    {
                        var message = string.Empty;

                        if (thirdPartyDeliveriesEnabled)            //IP - 25/05/12 - #10225
                        {
                            message = "No default print location exists for this stock location.";
                        }
                        else
                        {
                            message = "No default delivery location exists for this stock location.";
                        }

                        //if (DialogResult.No == MessageBox.Show("No default print location exists for this stock location." + Environment.NewLine + Environment.NewLine + "Do you still want to add this item?", "Add Item?", MessageBoxButtons.YesNo))
                        if (DialogResult.No == MessageBox.Show(message + Environment.NewLine + Environment.NewLine + "Do you still want to add this item?", "Add Item?", MessageBoxButtons.YesNo))  //IP - 25/05/12 - #10225
                            status = false;
                    }
                }

                #region Affinity Code goes here
                //The following code (are you sure that you can call this code?) deals with affinity items. It ensures that if the
                //terms type has already been selected then only the correct types
                //of items can be entered and if the terms type hasn't been 
                //selected, then affinity and non-affinity items are not permitted
                //on the same account

                //#18604 - CR15594
                findReadyAssist(itemDoc.DocumentElement);

                if (status && !(bool)Country[CountryParameterNames.AffinityStockSales] && !readyAssistItem && !readyAssistExists)
                {
                    DataRowView row = (DataRowView)drpTermsType.SelectedItem;
                    string affinityTerms = ((string)row["Affinity"]);
                    // Must only allow affinity items when an affinity terms type
                    // or an affinity item has already been added (on any terms type).
                    bool isAffinity = (affinityTerms == "Y") || (xml.findAffinities(itemDoc.DocumentElement));
                    // Must only allow NON-affinity items when a NON-affinity terms type
                    // or a NON-affinity item has already been added (on any terms type).
                    bool isNonAffinity = (affinityTerms == "N") || (xml.findNonAffinities(itemDoc.DocumentElement));

                    if (isAffinity && (itemType != IT.Affinity))
                    {
                        status = false;
                        ShowInfo(affinityTerms != "Y" && affinityTerms != "N" ? "M_REMOVEAFFINITY" : "M_ONLYAFFINITY");
                    }
                    else if (isNonAffinity && itemType == IT.Affinity)
                    {
                        status = false;
                        ShowInfo(affinityTerms != "Y" && affinityTerms != "N" ? "M_REMOVENONAFFINITY" : "M_NOTAFFINITY");
                    }
                }

                //#18604 - CR15594 - Prevent users adding more than 1 Ready Assist product to the sale.
                if (readyAssistExists)
                {
                    if (bool.Parse(currentItem.Attributes[Tags.ReadyAssist].Value) == true)
                    {
                        status = false;
                        ShowError("A Ready Assist item exists on the account ");

                    }
                }

                //End of affinity item processing
                #endregion

                if (status && !PaidAndTaken)
                    status = CheckDeliveryAreaDate();

                #region out of stock order processing
                // CR556 - The following code deals with out of stock items that are 
                // available on outstanding purchase orders.  If the item being 
                // purchased is out of stock a country parameter will determine whether 
                // the system should check if the item is available at other branches 
                // within the same region.  
                // If the out of stock item has not been selected from another branch
                // a pop-up screen will appear with details of outstanding purchase
                // orders that this item appears on.
                if (status && !PaidAndTaken)
                {
                    // rdb logic is correct here but KitDiscount passing over first statement [(itemType == IT.Stock || itemType == IT.Kit) which evaluates to false]
                    // then failing because QtyOnOrder not present
                    //if ((itemType == IT.Stock || itemType == IT.Kit) &&
                    //    (Convert.ToDecimal(currentItem.Attributes[Tags.AvailableStock].Value) <= (decimal)Country[CountryParameterNames.StockLockingQty]) ||
                    //    (Convert.ToDecimal(currentItem.Attributes[Tags.AvailableStock].Value) <= 0 && Convert.ToDecimal(currentItem.Attributes[Tags.QtyOnOrder].Value) > 0 &&
                    //    Convert.ToDecimal(currentItem.Attributes[Tags.QtyOnOrder].Value) <= (decimal)Country[CountryParameterNames.StockLockingQty]))
                    if (itemType == IT.Stock || itemType == IT.Kit)
                        if ((Convert.ToDecimal(currentItem.Attributes[Tags.AvailableStock].Value) <= (decimal)Country[CountryParameterNames.StockLockingQty]) ||
                            (Convert.ToDecimal(currentItem.Attributes[Tags.AvailableStock].Value) <= 0 && Convert.ToDecimal(currentItem.Attributes[Tags.QtyOnOrder].Value) > 0 &&
                            Convert.ToDecimal(currentItem.Attributes[Tags.QtyOnOrder].Value) <= (decimal)Country[CountryParameterNames.StockLockingQty]))
                        {
                            string lockString = "";
                            AccountManager.LockItem(txtProductCode.Text, Convert.ToInt16(drpLocation.Text), ref lockString, out Error);
                            if (Error.Length > 0)
                            {
                                ShowError(Error);
                                status = false;
                            }
                            else if (lockString.Length > 0)
                            {
                                ShowInfo("M_STOCKITEMLOCKED", new object[] { Credential.Name, Credential.User });
                                status = false;
                            }
                        }

                    if (status)
                    {
                        //IP - 25/01/2008 - Added this check, as previously if there was an item 
                        //with (1) in stock, when the item was added to a sale, the available would reduce to (0)
                        //but when selecting the item again after ammending details other than 'Quantity' and 
                        //updating the item, the 'Stock Availability' prompt would incorrectly be displayed.

                        decimal origQtyOrdered = 0;
                        decimal itemAvailableStock = 0;
                        decimal itemNewQty = 0;
                        itemAvailableStock = Convert.ToDecimal(currentItem.Attributes[Tags.AvailableStock].Value);
                        itemNewQty = Convert.ToDecimal(currentItem.Attributes[Tags.Quantity].Value);

                        //IP - 25/01/2008 - Find the value of the quantity already ordered
                        //for the item that has been entered.
                        DataView dv = (DataView)dgLineItems.DataSource;

                        foreach (DataRow row in dv.Table.Rows)
                        {
                            //uat363 rdb  add check fot parentProductCode
                            if (txtProductCode.Text == row["ProductCode"].ToString() && parentItemId == Convert.ToInt32(row["ParentItemID"]))  // RI jec 08/07/11
                            {
                                origQtyOrdered = Convert.ToInt32(row["QuantityOrdered"]);
                            }
                        }

                        decimal checkExceededAvailable = (itemAvailableStock + origQtyOrdered) - itemNewQty;

                        //if (Convert.ToDecimal(currentItem.Attributes[Tags.AvailableStock].Value) <= 0 &&
                        //    (itemType == IT.Stock || itemType == IT.Kit))
                        if (Convert.ToDecimal(currentItem.Attributes[Tags.AvailableStock].Value) <= 0 &&
                            (itemType == IT.Stock || itemType == IT.Kit) && checkExceededAvailable < 0)
                        {
                            bool checkPurchaseOrders = true;
                            DataSet ds = new DataSet();

                            if ((bool)Country[CountryParameterNames.CheckRegion])
                            {
                                // check items at other branches within the same region
                                ds = AccountManager.GetItemsInRegion(ItemID ?? 0, Convert.ToInt16(Config.BranchCode), out Error);
                                if (Error.Length > 0)
                                {
                                    status = false;
                                    ShowError(Error);
                                }
                                else
                                {
                                    DataView itemsInRegion = ds.Tables[TN.StockLocation].DefaultView;

                                    if (itemsInRegion.Count > 0)
                                    {
                                        // there are items available at other branches in the 
                                        // same region for this out of stock item

                                        // FA 14/12/09 UAT 662
                                        // Needs authorisation before proceeding
                                        if ((bool)Country[CountryParameterNames.OutOfStockAuth])
                                        {
                                            AuthorisePrompt auth = new AuthorisePrompt(this, lAuthorise, GetResource("M_NOSTOCKAUTH"));
                                            auth.ShowDialog();
                                            authorised = auth.Authorised;
                                        }
                                        else
                                            authorised = true;

                                        if (authorised)
                                        {
                                            // authorisation has been given to process
                                            // this out of stock item
                                            double qty = Convert.ToDouble(txtQuantity.Text);
                                            //IP - 21/01/08 - Added 'time' and 'trim' to hold values as previously
                                            //these were cleared once exiting the 'Stock Availability' prompt as they were getting 
                                            //set to default values (blank).
                                            //string time = txtTimeRequired.Text;
                                            string time = Convert.ToString(cbTime.SelectedItem);            //IP - 25/05/12 - #10225
                                            string trim = txtColourTrim.Text;
                                            StockAvailabilityRegion sar = new StockAvailabilityRegion(itemsInRegion, this, this.FormRoot);
                                            sar.ShowDialog();
                                            txtQuantity.Text = qty.ToString();
                                            //IP - 21/01/08 - Setting the 'txtTimeRequired' and 'txtColourTrim.Text' back 
                                            //to what was originally entered.
                                            //txtTimeRequired.Text = time;
                                            cbTime.SelectedItem = time;                                     //IP - 28/05/12 - #10225
                                            txtColourTrim.Text = trim;
                                            txtQuantity_Validating(this, null);
                                            checkPurchaseOrders = false;
                                        }
                                    }
                                }
                            }

                            if (checkPurchaseOrders)
                            {
                                // check outstanding purchase order for this out of stock item
                                ds = AccountManager.GetPurchaseOrders(ItemID.HasValue ? ItemID.Value : 0, Convert.ToInt16(Config.BranchCode), out Error);
                                if (Error.Length > 0)
                                {
                                    status = false;
                                    ShowError(Error);
                                }
                                else
                                {
                                    DataView availability = ds.Tables["ByCode"].DefaultView;

                                    if (availability.Count > 0)
                                    {
                                        //IP - 06/07/11 - RI - #3974 - authorisation is required to sell items on order
                                        if ((bool)Country[CountryParameterNames.StockOnOrderAuth])
                                        {
                                            AuthorisePrompt auth = new AuthorisePrompt(this, lAuthorise, GetResource("M_STOCKONORDERAUTH"));
                                            auth.ShowDialog();
                                            authorised = auth.Authorised;
                                        }
                                        else
                                            authorised = true;

                                        if (authorised) //IP - 06/07/11 - RI - #3974 - If authorised proceed to load the purchase orders
                                        {
                                            // there are oustanding purchase orders for this
                                            // out of stock item, so display pop-up screen
                                            // of out standing purchase orders
                                            //double qty = Convert.ToDouble(txtQuantity.Text);
                                            double qty = Convert.ToDouble(txtQuantity.Text) - Convert.ToDouble(origQtyOrdered);           //IP - 29/05/12
                                            StockAvailability sa = new StockAvailability(availability, this, this.FormRoot);
                                            sa.ShowDialog();
                                            if (PurchaseOrder)
                                            {
                                                txtQuantity.Text = qty.ToString();
                                                txtQuantity_Validating(this, null);
                                                dtDeliveryRequired.Value = sa.dtDeliveryRequired.Value;
                                                //txtTimeRequired.Text = sa.txtTimeRequired.Text;
                                                cbTime.SelectedItem = sa.cbTime.SelectedItem;                   //IP - 28/05/12 - #10225

                                                // set purchase order flag to true so we know
                                                // we have to update the available qty on
                                                // outstanding purchase orders
                                                currentItem.Attributes[Tags.PurchaseOrder].Value = true.ToString();
                                                currentItem.Attributes[Tags.PurchaseOrderNumber].Value = sa.purchaseOrderNumber;

                                                // align existing items planned delivery dates to
                                                // be the same as the out of stock item
                                                if (AlignDates)
                                                {
                                                    string xPath = "//Item[@Key != '" + ItemKey + "' and @Quantity != 0]";
                                                    //string xPath = "//Item[@Quantity != '0']";
                                                    foreach (XmlNode toAlign in itemDoc.SelectNodes(xPath))
                                                    {
                                                        toAlign.Attributes[Tags.DeliveryDate].Value = Convert.ToString(sa.dtDeliveryRequired.Value);
                                                        //toAlign.Attributes[Tags.DeliveryTime].Value = sa.txtTimeRequired.Text;
                                                        toAlign.Attributes[Tags.DeliveryTime].Value = Convert.ToString(sa.cbTime.SelectedItem);                           //IP - 28/05/12 - #10225
                                                    }
                                                }
                                                errorProvider2.SetError(dtDeliveryRequired, GetResource("M_DATEWARNING"));
                                            }
                                            else
                                            {
                                                // user has not chosen an item from the purchase
                                                // order so re-set the item details
                                                string itemNo = txtProductCode.Text;
                                                ClearItemDetails();
                                                txtProductCode.Text = itemNo;
                                                txtProductCode.Focus();
                                                status = false;
                                            }
                                        }
                                        else //IP - 06/07/11 - RI - #3974 - do not load the purchase orders
                                        {
                                            // user has not been authorised to purchase an item on order
                                            // so re-set the item details
                                            string itemNo = txtProductCode.Text;
                                            ClearItemDetails();
                                            txtProductCode.Text = itemNo;
                                            txtProductCode.Focus();
                                            status = false;
                                        }
                                    }
                                    else
                                    {
                                        // item is out of stock and there are no 
                                        // outstanding purchase orders for this item
                                        if ((bool)Country[CountryParameterNames.OutOfStockAuth])
                                        {
                                            AuthorisePrompt auth = new AuthorisePrompt(this, lAuthorise, GetResource("M_NOSTOCKAUTH"));
                                            auth.ShowDialog();
                                            authorised = auth.Authorised;
                                        }
                                        else
                                            authorised = true;

                                        if (authorised)
                                        {
                                            // authorisation has been given to process
                                            // this out of stock item
                                            short cWarehouseTime = Convert.ToInt16(Country[CountryParameterNames.WarehouseTime]);
                                            short leadtime = Convert.ToInt16(currentItem.Attributes[Tags.LeadTime].Value);
                                            dtDeliveryRequired.Value = dtDeliveryRequired.Value.AddDays(leadtime + cWarehouseTime);

                                            //68002 - Ensure special order message is not duplicated.
                                            if (txtColourTrim.Text.IndexOf("Special Order:") < 0)
                                                txtColourTrim.Text = txtColourTrim.Text + " " + GetResource("M_OUTOFSTOCK", new object[] { Credential.Name + " (" + Credential.User + ")" });

                                            // pop-up option to allow user to align req
                                            // delivery date for existing items to be the
                                            // same as the out of stock item
                                            if (DialogResult.Yes == ShowInfo("M_ALIGNDELDATE", MessageBoxButtons.YesNo))
                                            {
                                                //string xPath = "//Item[@Quantity != '0']";
                                                string xPath = "//Item[@Key != '" + ItemKey + "' and @Quantity != 0]";
                                                foreach (XmlNode toAlign in itemDoc.SelectNodes(xPath))
                                                {
                                                    toAlign.Attributes[Tags.DeliveryDate].Value = Convert.ToString(dtDeliveryRequired.Value);
                                                }

                                                AlignedDelDate = dtDeliveryRequired.Value;
                                                AlignDates = true;
                                            }
                                            //FA - 21/12 UAT 665
                                            errorProvider2.SetError(dtDeliveryRequired, GetResource("M_DATEWARNING"));
                                        }
                                        else
                                        {
                                            // authorisation has not been given
                                            // so re-set the item details
                                            string itemNo = txtProductCode.Text;
                                            ClearItemDetails();
                                            txtProductCode.Text = itemNo;
                                            txtProductCode.Focus();
                                            status = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                /*if(Convert.ToDecimal(currentItem.Attributes[Tags.AvailableStock].Value)<=0 &&
						(itemType==IT.Stock ||
						itemType==IT.Kit))
					{
						if((bool)Country[CountryParameterNames.AllowZeroStock])
						{
							if(DialogResult.Yes!=ShowInfo("M_NOSTOCKOPTION", MessageBoxButtons.YesNo))
								status = false;
						}
						else
						{
							ShowInfo("M_NOSTOCK");
							status = false;
						}
					}*/

                #endregion

                if (status)
                {
                    //See if this item is already in the document. If it is
                    //replace it having cloned the existing related items
                    // uat363 rdb todo add check for parentproductno here

                    XmlNode found = xml.findItem(itemDoc.DocumentElement, ItemKey, parentProductCode);

                    //update some more attributes to the current node which we didn't
                    //know before, or which may have changed
                    currentItem.Attributes[Tags.Quantity].Value = txtQuantity.Text;
                    currentItem.Attributes[Tags.Value].Value = StripCurrency(txtValue.Text);
                    currentItem.Attributes[Tags.DeliveryDate].Value = Convert.ToString(dtDeliveryRequired.Value);
                    //currentItem.Attributes[Tags.DeliveryTime].Value = txtTimeRequired.Text;
                    currentItem.Attributes[Tags.DeliveryTime].Value = Convert.ToString(cbTime.SelectedItem);                //IP - 25/05/12 - #10225
                    currentItem.Attributes[Tags.BranchForDeliveryNote].Value = drpBranchForDel.Text; //drpLocation.Text;
                    currentItem.Attributes[Tags.DeliveryArea].Value = drpDeliveryArea.Text == GetResource("L_ALL") ? "" : drpDeliveryArea.Text;
                    currentItem.Attributes[Tags.DeliveryProcess].Value = cbImmediate.Checked ? "I" : "S";
                    currentItem.Attributes[Tags.ColourTrim].Value = txtColourTrim.Text;
                    currentItem.Attributes[Tags.DeliveryAddress].Value = drpDeliveryAddress.Text;
                    currentItem.Attributes[Tags.AvailableStock].Value = txtAvailable.Text;
                    currentItem.Attributes[Tags.Assembly].Value = cbAssembly.Checked ? "Y" : "N";
                    currentItem.Attributes[Tags.Damaged].Value = cbDamaged.Checked ? "Y" : "N";
                    currentItem.Attributes[Tags.SalesBrnNo].Value = Convert.ToString(Config.BranchCode);         //IP - 24/05/11 - CR1212 - RI - #3651
                    currentItem.Attributes[Tags.Express].Value = cbImmediate.Checked ? "N" : cbExpress.Checked ? "Y" : "N";                // #10376  jec 

                    //IP - 12/06/12 - #10328 - Change no longer required //IP - 29/05/12 - #9877 - Warehouse & Deliveries Integration
                    //if (LineItemBooking != null)
                    //{
                    //    foreach (DataRow dr in LineItemBooking.Rows)
                    //    {
                    //        if (found != null && Convert.ToInt32(dr["ItemId"]) == Convert.ToInt32(currentItem.Attributes[Tags.ItemId].Value) 
                    //            && Convert.ToInt32(currentItem.Attributes[Tags.Quantity].Value) < Convert.ToInt32(found.Attributes[Tags.Quantity].Value))
                    //        {
                    //            ShowInfo("M_CANNOTREMOVEITEM", MessageBoxButtons.OK);
                    //            status = false;
                    //            break;
                    //        }
                    //    }
                    //}

                    if ((bool)Country[CountryParameterNames.LoyaltyScheme]) // HC only
                    {
                        //Update contract number to redemption number & price if loyalty item.
                        if (currentItem.Attributes[Tags.Code].Value.ToUpper() == LoyaltyDropStatic.VoucherCode)
                        {
                            currentItem.Attributes[Tags.ContractNumber].Value = RedemptionNo.ToString();
                            currentItem.Attributes[Tags.UnitPrice].Value = currentItem.Attributes[Tags.Value].Value;
                        }
                    }

                    if (chxNonStock.Checked)
                    {
                        currentItem.Attributes[Tags.UnitPrice].Value = StripCurrency(txtUnitPrice.Text);
                    }

                    if (Renewal)
                    {
                        currentItem.Attributes[Tags.ContractNumber].Value = contractNo;
                        currentItem.Attributes[Tags.DeliveryDate].Value = Convert.ToString(dtRenewalDelDate.Value);
                    }

                    //Instant Replacement - Commented out in version 5.1
                    if (!ReplacementAdded && instantReplacement != null && Replacement) //#17290 //!PaidAndTaken)
                    {
                        currentItem.Attributes[Tags.ReplacementItem].Value = Convert.ToBoolean(1).ToString();
                        ReplacementAdded = true;
                    }

                    #region Value Controlled tax shenanigans

                    discount = Convert.ToBoolean(currentItem.Attributes[Tags.Type].Value == "Discount");

                    //IP - 10/06/11 - CR1212 - RI - #3817 - If this is a discount we need to check if its an account wide discount.
                    if (discount)
                    {

                        Blue.Cosacs.Shared.CosacsWeb.Models.NonStock nonstock = new Blue.Cosacs.Shared.CosacsWeb.Models.NonStock();
                        nonstock = Services.GetService(STL.Common.Services.Services.ServiceTypes.CosacsWeb).GetDiscountDetails(Convert.ToString(currentItem.Attributes[Tags.Code].Value));

                        if (nonstock != null)
                        {
                            //Can only apply staff discount to staff accounts
                            if (isStaff != true && nonstock.IsStaffDiscount == true)
                            {
                                status = false;
                                errorProvider2.SetError(txtProductCode, "Staff discount cannot be added to this account");
                            }
                            else
                            {
                                if (!PaidAndTaken)
                                {
                                    AuthorisePrompt auth = new AuthorisePrompt(this, overrideDiscountLimit, GetResource("M_DISCOUNTMONTHSPASSEDAUTH", nonstock.DiscountRecurringPeriod));

                                    var discountDeliveryMonths = AccountManager.GetDiscountDeliveryMonths(CustomerID, Int32.Parse(currentItem.Attributes[Tags.ItemId].Value.ToString()));

                                    if (discountDeliveryMonths != -1 && discountDeliveryMonths <= nonstock.DiscountRecurringPeriod)
                                    {
                                        auth.ShowDialog();
                                    }
                                    else
                                    {
                                        auth.Authorised = true;
                                    }

                                    if (auth.Authorised == false)
                                    {
                                        status = false;
                                    }
                                }

                            }
                        }

                        accountWideDiscount = isAcctWideDiscount(Convert.ToString(currentItem.Attributes[Tags.ProductCategory].Value));
                    }

                    //IP - 03/10/11 - RI - #8132 - Commented out the below. Was incorrectly calculating tax on discount for discount that is to be linked to specific item. Taxrate for linked discounts should be taken from parent item which is processed later. This also applies to account wide discounts
                    //if (discount && !chxTaxExempt.Checked && !accountWideDiscount && !percentage) //IP - 12/09/11 - RI - #8132 - and not a percentage discount //IP - 10/06/11 - CR1212 - RI - #3817
                    //{
                    //    currentItem.Attributes[Tags.TaxAmount].Value = "0";
                    //    if (found != null)
                    //    { // set the tax rate of the discount to the tax rate of the parent item. 
                    //        currentItem.Attributes[Tags.TaxRate].Value = found.ParentNode.ParentNode.Attributes[Tags.TaxRate].Value;
                    //    }
                    //    valueTax = AccountManager.CalculateTaxAmount(Config.CountryCode, currentItem, chxTaxExempt.Checked, out Error);
                    //    if (Error.Length > 0)
                    //    {
                    //        ShowError(Error);
                    //        status = false;
                    //    }
                    //    else
                    //    {
                    //        if ((string)Country[CountryParameterNames.AgreementTaxType] == "E")
                    //        {
                    //            newValue = Convert.ToDecimal(currentItem.Attributes[Tags.Value].Value);
                    //            newValue -= valueTax;
                    //            currentItem.Attributes[Tags.Value].Value = StripCurrency(newValue.ToString(DecimalPlaces));
                    //            currentItem.Attributes[Tags.UnitPrice].Value = StripCurrency(newValue.ToString(DecimalPlaces));
                    //        }
                    //        currentItem.Attributes[Tags.TaxAmount].Value = valueTax.ToString();
                    //    }
                    //}

                    #endregion

                    if (found != null)
                    {
                        // Item is already there. Depending on the item type the user
                        // may be allowed to update the item
                        foundType = found.Attributes[Tags.Type].Value;
                        double qty = Convert.ToDouble(found.Attributes[Tags.Quantity].Value);

                        //#14644 - Allow to increase qty // #14603 
                        if (found.Attributes[Tags.ScheduledQuantity].Value != "0" && !(Convert.ToInt32(currentItem.Attributes[Tags.Quantity].Value) > Convert.ToInt32(found.Attributes[Tags.ScheduledQuantity].Value)))
                        {
                            MessageBox.Show("This item has already been exported for delivery can not be changed");
                            status = false;
                        }

                        if (status)
                        {
                            if (Collection && AgreementNo != 1)     /* ensure qty has not been increased */
                            {
                                decimal newQty = Convert.ToDecimal(currentItem.Attributes[Tags.Quantity].Value);
                                decimal oldQty = Convert.ToDecimal(found.Attributes[Tags.Quantity].Value);
                                if (newQty > oldQty)
                                {
                                    status = false;
                                    ShowInfo("M_CANTINCREASEQTY");
                                }
                                else
                                {
                                    if (newQty < oldQty)    // it may be necessary to automatically
                                    {                   // remove one or more warranty items
                                        string xPath = "RelatedItems/Item[@Type = 'Warranty']";
                                        XmlNodeList warranties = found.SelectNodes(xPath);
                                        if (warranties.Count > newQty)
                                        {
                                            decimal remove = warranties.Count - newQty;
                                            for (int i = 0; i < remove; i++)
                                            {
                                                xml.deleteItem(warranties[i]);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (status)
                        {
                            if (itemType == IT.Warranty || itemType == IT.KitWarranty)
                            {
                                ShowInfo("M_EDITWARRANTY");
                                status = false;
                            }
                            else
                            {
                                #region DeliverySchedule checking for revisions goes here
                                //if we're revising an account we need to check deliveries and 
                                //scedules to see if the proposed update is permissable
                                if (revision)
                                {
                                    //Both the item to be deleted and it's related items must have their 
                                    //schedules checked
                                    double newQty = Convert.ToDouble(currentItem.Attributes[Tags.Quantity].Value);
                                    double oldQty = Convert.ToDouble(found.Attributes[Tags.Quantity].Value);
                                    double ratio = newQty / oldQty;


                                    // to rdb get deliveredquantity here form found and if we call schedule here
                                    // set resetHoldProp to false if
                                    // deliveredQuantity > 0 and newQty - delQuantity = 0

                                    if (newQty < oldQty)        //only check if reducing qty
                                    {
                                        int parentItemID = 0;

                                        if (found.Attributes[Tags.ParentItemId] != null)
                                            parentItemID = Convert.ToInt32(found.Attributes[Tags.ParentItemId].Value);


                                        if (thirdPartyDeliveriesEnabled)
                                        {
                                            //IP/JC - 01/03/10 - CR1072 - Malaysia 3PL 
                                            //if (found.Attributes["VanNo"] != null && found.Attributes["VanNo"].Value == "DHL"
                                            //    && found.Attributes["DhlDNNo"].Value == null)
                                            //{
                                            //    MessageBox.Show("Item has been exported to DHL.");


                                            //    // CR953 Can't remove an item if it is in transit with DHL

                                            //    //if (found.Attributes["DhlDNNo"] != null && found.Attributes["DhlDNNo"].Value != String.Empty)
                                            //    //{
                                            //    // UAT 38 Authorisation required to remove an item
                                            //    AuthorisePrompt auth = new AuthorisePrompt(this, btnRemove, "Item is in transit with DHL. Authorisation is required to remove this item.");
                                            //    auth.ShowDialog();

                                            //    if (auth.Authorised)
                                            //    {
                                            //        status = true;
                                            //    }
                                            //    else
                                            //    {
                                            //        //MessageBox.Show("Item in transit with DHL - it cannot be removed");
                                            //        status = false;
                                            //    }
                                            //    //}
                                            //}


                                            double delivered = 0;
                                            double scheduled = 0;
                                            bool repo = false;                                                         //IP - 26/06/12 - #10516
                                                                                                                       //string itemNo = found.Attributes[Tags.Code].Value;
                                            int itemID = Convert.ToInt32(found.Attributes[Tags.ItemId].Value);          //IP/NM - 18/05/11 -CR1212 - #3627           
                                            short location = Convert.ToInt16(found.Attributes[Tags.Location].Value);
                                            string contractNo = found.Attributes[Tags.ContractNumber].Value;
                                            double amendedQty = Convert.ToDouble(currentItem.Attributes[Tags.Quantity].Value);


                                            AccountManager
                                                .GetItemsDeliveredAndScheduled(accountNo, AgreementNo, itemID,          //IP/NM - 18/05/11 -CR1212 - #3627   
                                                                               location, contractNo, parentItemID,       //IP
                                                                               out delivered, out scheduled, out repo, out Error); //IP - 26/06/12 - #10516

                                            var vanNo = found.Attributes["VanNo"] != null ? found.Attributes["VanNo"].Value : null;
                                            var shipQty = Convert.ToDouble(found.Attributes["ShipQty"].Value);

                                            if (vanNo == "DHL" &&
                                                //found.Attributes["DhlDNNo"].Value != null && 
                                                //found.Attributes["DhlDNNo"].Value != string.Empty && 
                                                (shipQty > (oldQty - newQty) || shipQty > (newQty - delivered))
                                               )
                                            {
                                                MessageBox.Show("In transit - unable to reduce item quantity.");
                                                status = false;
                                            }
                                        }


                                        if (CashAndGoReturn ||  //uat(5.2)-907. If cash and go return no need to check the delivery or schedule (4.3 merge)
                                            CheckSchedules(accountNo,
                                                            currentItem.Attributes[Tags.Code].Value,
                                                            Convert.ToInt32(currentItem.Attributes[Tags.ItemId].Value),    //IP/NM - 18/05/11 -CR1212 - #3627      
                                                            Convert.ToInt16(currentItem.Attributes[Tags.Location].Value),
                                                            currentItem.Attributes[Tags.ContractNumber].Value,
                                                            newQty,
                                                            itemType, parentItemID)) //IP
                                        {
                                            var childParentItemID = 0;                      //IP - 08/07/11 - RI 

                                            foreach (XmlNode child in found.SelectSingleNode(Elements.RelatedItem))
                                            {
                                                //IP - 08/07/11 - RI - ParentItemID tag does not exist for the child therefore select the parent nodes itemid
                                                if (child.ParentNode.ParentNode.Attributes[Tags.ItemId].Value != null)
                                                {
                                                    childParentItemID = Convert.ToInt32(child.ParentNode.ParentNode.Attributes[Tags.ItemId].Value);
                                                }
                                                //if the related item is a warranty we don't need to check schedules
                                                //at this point - we'll deal with it by automatically collecting any
                                                //delivered warranties at the back end when we save the account
                                                if (child.NodeType == XmlNodeType.Element &&
                                                    child.Name == Tags.Item &&
                                                    child.Attributes[Tags.Type].Value != IT.Warranty)
                                                {
                                                    oldQty = Convert.ToDouble(child.Attributes[Tags.Quantity].Value);
                                                    newQty = oldQty * ratio;
                                                    if (CashAndGoReturn == false && !CheckSchedules(accountNo, child.Attributes[Tags.Code].Value, Convert.ToInt32(child.Attributes[Tags.ItemId].Value), //IP/NM - 18/05/11 -CR1212 - #3627 
                                                        Convert.ToInt16(child.Attributes[Tags.Location].Value),
                                                        child.Attributes[Tags.ContractNumber].Value,
                                                        newQty, itemType, childParentItemID))   //IP - 08/07/11 - RI
                                                    {
                                                        status = false;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                            status = false;
                                    }
                                }
                            }
                            #endregion

                            if (status)
                            {
                                //otherwise the item can be replaced, but doesn't have to be
                                //if ((found.Attributes[Tags.Quantity].Value != "0" && foundType != IT.Kit) || (found.Attributes[Tags.Quantity].Value == "0" && Replacement))
                                if ((foundType != IT.Kit) || (found.Attributes[Tags.Quantity].Value == "0" && Replacement))
                                {
                                    if (itemType == IT.Discount && !accountWideDiscount)            //IP - 10/06/11 - CR1212 - RI - #3817
                                    {
                                        // Check whether the discount is linked to an item
                                        if (found.ParentNode != null)
                                        {
                                            if (found.ParentNode.ParentNode != null)
                                            {
                                                status = this.AuthoriseDiscount(currentItem, found.ParentNode.ParentNode);
                                            }
                                        }
                                    }

                                    if (status)
                                    {
                                        //UAT Issue 70 - Only replace an item if it
                                        //has not already been delivered.
                                        double delivered = 0;
                                        double scheduled = 0;
                                        int parentItemID = 0;
                                        bool repo = false;

                                        string itemNo = found.Attributes[Tags.Code].Value;
                                        int itemID = Convert.ToInt32(found.Attributes[Tags.ItemId].Value);
                                        short location = Convert.ToInt16(found.Attributes[Tags.Location].Value);
                                        string contractNo = found.Attributes[Tags.ContractNumber].Value;
                                        double amendedQty = Convert.ToDouble(currentItem.Attributes[Tags.Quantity].Value);
                                        if (found.Attributes[Tags.ParentItemId] != null)
                                            parentItemID = Convert.ToInt32(found.Attributes[Tags.ParentItemId].Value);

                                        if (AccountManager.HasReturnsItens(accountNo, this.AgreementNo, itemID, location, contractNo, parentItemID))
                                        {
                                            MessageBox.Show(this, "Please process item collection prior to re-adding it on the account.", "Item not added", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            status = false;
                                        }

                                        if (status)
                                        {
                                            AccountManager.GetItemsDeliveredAndScheduled(accountNo, this.AgreementNo,
                                                itemID,
                                                location,
                                                contractNo,
                                                parentItemID,
                                                out delivered,
                                                out scheduled,
                                                out repo,
                                                out Error);
                                            if (Error.Length > 0)
                                            {
                                                ShowError(Error);
                                                status = false;
                                            }
                                            else
                                            {
                                                //

                                                //if (delivered > amendedQty)           
                                                if (delivered > amendedQty && !CashAndGoReturn && (itemType == IT.Stock || itemType == IT.Kit))  //IP - 15/02/10 - LW - 71860
                                                {
                                                    ShowInfo("M_PARTIALLYDELIVERED", new object[] { itemNo });
                                                    status = false;
                                                }
                                                if (status)
                                                {
                                                    double newQty = Convert.ToDouble(currentItem.Attributes[Tags.Quantity].Value);
                                                    double oldQty = Convert.ToDouble(found.Attributes[Tags.Quantity].Value);
                                                    if (scheduled > 0)
                                                        status = NewDelNoteRequired(found, accountNo, itemID, location, scheduled, newQty, oldQty);     //IP/NM - 18/05/11 -CR1212 - #3627  
                                                }
                                                if (status)
                                                {
                                                    //IP - 15/02/10 - LW 71860
                                                    string msg = string.Empty;

                                                    msg = CashAndGoReturn ? "M_REPLACECASHANDGOITEM" : "M_REPLACEITEM";

                                                    //if (DialogResult.OK == ShowInfo("M_REPLACEITEM", MessageBoxButtons.OKCancel))
                                                    if (DialogResult.OK == ShowInfo(msg, MessageBoxButtons.OKCancel))
                                                    {
                                                        if (!accountWideDiscount)            //IP - 10/06/11 - CR1212 - RI - #3817
                                                        {
                                                            xml.replaceItem(found, currentItem);

                                                            if (Replacement) //#17290 
                                                            {

                                                                string replacementCode = currentItem.Attributes[Tags.Code].Value;
                                                                int replacementItemID = Convert.ToInt32(currentItem.Attributes[Tags.ItemId].Value);                  //IP/NM - 18/05/11 -CR1212 - #3627 
                                                                string replacementLocation = Convert.ToString(currentItem.Attributes[Tags.Location].Value);
                                                                int replacementQuantity = Convert.ToInt16(currentItem.Attributes[Tags.Quantity].Value);

                                                                //#18437
                                                                if (freeReplacementWarrantyAdded == false)
                                                                {
                                                                    AttachFreeReplacementWarranty(replacementCode, replacementLocation, replacementItemID, replacementQuantity);
                                                                }

                                                            }
                                                        }
                                                        else //If an account wide discount then we need to replace all instances.
                                                        {
                                                            status = ProcessAccountWideDiscount(true);      //IP - 16/04/12 - #8613
                                                        }
                                                        currentItem.Attributes[Tags.ContractNumber].Value = found.Attributes[Tags.ContractNumber].Value;
                                                        if (itemType == IT.Affinity || readyAssistItem)      //#18604 - CR15594
                                                            AddAffinityContractNo(currentItem);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    //xml.replaceItem(found, currentItem);
                                    //JJ - 12/11/02 Bug J46 Remove item it if it has a zero qty and is 
                                    //being re-added. This will solve the problem of a zero valued parent   
                                    //having it's children overwritten

                                    // CR795 - When processing an instant replacement with an identical item, we
                                    // must attach the orginal warranty to this newly added item, provided we are
                                    // in period 1 or period 2
                                    if (Replacement)
                                    {
                                        XmlNode replacementWarranty = found.SelectSingleNode("RelatedItems/Item[@Type = 'Warranty']");
                                        if (replacementWarranty != null //// &&
                                                                        //Convert.ToInt32(replacementWarranty.Attributes[Tags.ItemId].Value) == instantReplacement.WarrantyID 
                                                                        ////&&
                                                                        ////(instantReplacement.TimePeriod == OneForOneTimePeriod.IRPeriod1 || instantReplacement.TimePeriod == OneForOneTimePeriod.IRPeriod2)
                                            )
                                        {
                                            replacementWarranty.ParentNode.RemoveChild(replacementWarranty);
                                            replacementWarranty = itemDoc.ImportNode(replacementWarranty, true);
                                            currentItem.SelectSingleNode("RelatedItems").AppendChild(replacementWarranty);
                                            currentItem.Attributes[Tags.CanAddWarranty].Value = Boolean.FalseString;
                                            //////itemDoc.DocumentElement.AppendChild(currentItem);     //#17290 ????
                                        }

                                        //#17290 ???????
                                        //////string code = currentItem.Attributes[Tags.Code].Value;
                                        //////int itemID = Convert.ToInt32(currentItem.Attributes[Tags.ItemId].Value);                  //IP/NM - 18/05/11 -CR1212 - #3627 
                                        //////string location = Convert.ToString(currentItem.Attributes[Tags.Location].Value);
                                        //////int quantity = Convert.ToInt16(currentItem.Attributes[Tags.Quantity].Value);

                                        //////AttachFreeReplacementWarranty(code, location, itemID, quantity);
                                    }
                                    kitWarranty = found.SelectSingleNode("RelatedItems/Item[@Type='KitWarranty']");
                                    found.ParentNode.RemoveChild(found);
                                    found = null;
                                }
                            }
                        }
                    }

                    if (found == null)
                    {
                        /* if it's a replacement the order can only contain
						 * one item and one discount */
                        if (Replacement && PaidAndTaken)
                        {
                            if (itemType == IT.Discount)    /* make sure there are no other discounts */
                            {
                                XmlNodeList x = itemDoc.DocumentElement.SelectNodes("//Item[@Quantity != '0' and @Type = 'Discount']");
                                if (x.Count > 0)
                                    status = false;
                            }
                            else    /* make sure there are no other non-discount items */
                            {
                                XmlNodeList x = itemDoc.DocumentElement.SelectNodes("//Item[@Quantity != '0' and @Code != 'STAX' and @Code != 'DT' and @Type != 'Discount']");
                                if (x.Count > 0)
                                    status = false;
                            }

                            if (!status)
                                ShowInfo("M_TOOMANYITEMS");
                        }

                        /* if it's a collection it must already exist in the document
						 * i.e. you can't add items (unless it's a collection of a pre .net cash and go sale) */
                        if (Collection && AgreementNo != 1)
                        {
                            status = false;
                            ShowInfo("M_CANTADDITEMS");
                        }

                        if (status)
                        {
                            // If it's a discount ask if the user wants to tie it any
                            // existing stock items on the account. Also do this if it a an installation
                            if (itemType == IT.Discount)
                            {

                                //CR545 - Discounts can only be applied to items in the same product category
                                string filter = BuildCategoryFilter(currentItem.Attributes[Tags.ProductCategory].Value);

                                StockForDiscount next = new StockForDiscount(itemDoc, this, filter, accountWideDiscount);   //IP - 31/05/11 - CR1212 - RI - #2315
                                if (next.Count > 0)
                                    next.ShowDialog();
                                else
                                {
                                    ShowInfo("M_DISCOUNTTYPE");
                                    //status = false;                   //IP - 07/06/11 - CR1212 - RI - #3817
                                }

                                status = next.addDiscount;              //IP - 07/06/11 - CR1212 - RI - #3817

                                if (status)
                                {
                                    if (LinkToKey.Length > 0)
                                    {
                                        XmlNode link = xml.findItem(itemDoc.DocumentElement, LinkToKey, parentProductCode);
                                        if (link != null)
                                        {
                                            status = this.AuthoriseDiscount(currentItem, link);
                                            if (status)
                                            {
                                                XmlNode field = null;
                                                foreach (XmlNode child in link.ChildNodes)
                                                {
                                                    if (child.NodeType == XmlNodeType.Element && child.Name == Elements.RelatedItem)
                                                        field = child;
                                                }

                                                if (field.NodeType == XmlNodeType.Element)
                                                {
                                                    if (field.Name == Elements.RelatedItem)
                                                    {
                                                        field.AppendChild(currentItem);
                                                    }
                                                }

                                                /* discount taxrate must = taxrate of the item it discounts */
                                                /* if taxrate has changed we must recalculate the tax amount */
                                                if (currentItem.Attributes[Tags.TaxRate].Value != link.Attributes[Tags.TaxRate].Value)
                                                {
                                                    currentItem.Attributes[Tags.TaxRate].Value = link.Attributes[Tags.TaxRate].Value;
                                                    currentItem.Attributes[Tags.TaxAmount].Value = "0"; //setting tax rate to 0 -- this allows recalculation of correct amount. 
                                                                                                        //if agreements exclude tax i.e. there is a seperate tax line then value of line is reduced by tax amount


                                                    if ((string)Country[CountryParameterNames.AgreementTaxType] == "E")
                                                    {
                                                        decimal oldTax = Convert.ToDecimal(currentItem.Attributes[Tags.TaxAmount].Value);
                                                        newValue = Convert.ToDecimal(currentItem.Attributes[Tags.Value].Value);
                                                        newValue += oldTax;//So the original value was the value + previous tax
                                                        currentItem.Attributes[Tags.Value].Value = newValue.ToString();
                                                        currentItem.Attributes[Tags.UnitPrice].Value = newValue.ToString();
                                                        currentItem.Attributes[Tags.TaxAmount].Value = "0"; // setting value to 0 as separate tax line
                                                    }

                                                    valueTax = AccountManager.CalculateTaxAmount(Config.CountryCode, currentItem, chxTaxExempt.Checked, out Error);
                                                    if (Error.Length > 0)
                                                    {
                                                        ShowError(Error);
                                                        status = false;
                                                    }
                                                    else
                                                    {
                                                        if ((string)Country[CountryParameterNames.AgreementTaxType] == "E")
                                                        {
                                                            newValue -= valueTax; //  take off newly calculated tax from the value as value excludes tax
                                                            currentItem.Attributes[Tags.Value].Value = newValue.ToString();
                                                            currentItem.Attributes[Tags.UnitPrice].Value = newValue.ToString();
                                                        }
                                                        // always store the tax amount even if sales include tax.... 
                                                        currentItem.Attributes[Tags.TaxAmount].Value = valueTax.ToString();

                                                    }
                                                }
                                            }
                                        }

                                        LinkToKey = "";
                                    }
                                    else if (LinkDiscToAll)              //IP - 31/05/11 - CR1212 - RI - #2315
                                    {
                                        status = ProcessAccountWideDiscount(false);         //IP - 16/04/12 - #8613
                                    }
                                    else
                                    {
                                        // 68495 RD/PN 19/09/2006 Modified to display Authorise Discount if item is not linked
                                        decimal discountVal = Convert.ToDecimal(currentItem.Attributes[Tags.Value].Value);
                                        AuthorisePrompt auth = new AuthorisePrompt(this, overrideDiscountLimit, GetResource("M_DISCOUNTAUTHNOTLINKED"));
                                        if (Math.Abs(discountVal) >= 0.01M)
                                        {
                                            auth.ShowDialog();
                                        }

                                        if (auth.Authorised || Math.Abs(discountVal) == 0)
                                        {
                                            itemDoc.DocumentElement.AppendChild(currentItem);
                                        }

                                    }
                                }
                            }
                            else if (Convert.ToBoolean(currentItem.Attributes[Tags.FreeGift].Value) == true) //CR1048 - If freegift //IP - 10/02/10 - CR1048 (Ref:3.1.3) Merged - Malaysia Enhancements (CR1072)
                            {

                                StockForDiscount next = new StockForDiscount(itemDoc, this, "", accountWideDiscount);   //IP - 31/05/11 - CR1212 - RI - #2315
                                if (next.Count > 0)
                                    next.ShowDialog();

                                if (LinkToKey.Length > 0)
                                {
                                    XmlNode link = xml.findItem(itemDoc.DocumentElement, LinkToKey);
                                    if (link != null)
                                    {
                                        XmlNode field = null;
                                        foreach (XmlNode child in link.ChildNodes)
                                        {
                                            if (child.NodeType == XmlNodeType.Element && child.Name == Elements.RelatedItem)
                                                field = child;
                                        }

                                        if (field.NodeType == XmlNodeType.Element)
                                        {
                                            if (field.Name == Elements.RelatedItem)
                                            {
                                                field.AppendChild(currentItem);
                                            }
                                        }
                                    }
                                    LinkToKey = "";
                                }
                                else
                                {
                                    itemDoc.DocumentElement.AppendChild(currentItem);
                                }
                            }
                            else    /* if it's not a discount */
                            {
                                if (itemType == IT.Kit)
                                {
                                    status = AddKit(ItemID ?? 0,
                                                    Convert.ToInt16(drpLocation.Text),
                                                    drpAccountType.Text,
                                                    Convert.ToDouble(txtQuantity.Text));
                                    if (kitWarranty != null)
                                    {
                                        string xpath = String.Format("//Item[@ItemId = '{0}' and @Location = '{1}']/RelatedItems", ItemID ?? 0, Convert.ToInt16(drpLocation.Text));
                                        itemDoc.SelectSingleNode(xpath).AppendChild(kitWarranty);
                                    }

                                }
                                else
                                {
                                    if (itemType == IT.Affinity || readyAssistItem)      //#18604 - CR15594
                                    {
                                        /* prompt the user to enter a contract number for the affintiy item */
                                        AddAffinityContractNo(currentItem);
                                    }
                                    //add the new item to the document
                                    itemDoc.DocumentElement.AppendChild(currentItem);
                                }
                            }
                        }
                    }
                }

                if (status)
                {
                    // rdb uat363 noticed discount under kit does not have a Deleted attribute
                    //if (currentItem.Attributes[Tags.Deleted].Value == "Y")
                    if (currentItem.Attributes[Tags.Deleted] != null && currentItem.Attributes[Tags.Deleted].Value == "Y")
                    {
                        AuthorisePrompt ap = new AuthorisePrompt(this, lAuthoriseDP, GetResource("M_DELETEDITEM"));
                        ap.ShowDialog();
                        if (!ap.Authorised)
                        {
                            currentItem.ParentNode.RemoveChild(currentItem);
                            string itemNo = txtProductCode.Text;
                            ClearItemDetails();
                            txtProductCode.Text = itemNo;
                            txtProductCode.Focus();
                            status = false;
                        }
                    }
                }
                if (status)
                {
                    string productCode = txtProductCode.Text;
                    string branchNo = drpLocation.Text;
                    string itemID = Convert.ToString(currentItem.Attributes[Tags.ItemId].Value);                            //IP - 09/06/11 - CR1212 - RI

                    //If this product has associated warranties, then prompt the 
                    //user to select one.  However the user will not be prompted 
                    //if this is a Luck Dollar store.
                    if (currentItem.Attributes[Tags.Type].Value == IT.Stock && Convert.ToInt32(currentItem.Attributes[Tags.Quantity].Value) > 0 && !Replacement)
                    {
                        //#17290
                        var refCode = currentItem.Attributes[Tags.RefCode].Value;

                        var warrantyType = refCode == "ZZ" ? WarrantyType.InstantReplacement : WarrantyType.Extended;

                        // #16292
                        warranties = Services.GetService(STL.Common.Services.Services.ServiceTypes.CosacsWeb).GetWarranties(txtProductCode.Text, drpLocation.SelectedItem.ToString(), warrantyType);

                        if (warranties.Items == null)
                        {
                            warranties.Items = new List<WarrantyResult.Item>();
                        }

                        var taxType = (string)Country[CountryParameterNames.TaxType];
                        //var agreementTaxType = (string)Country[CountryParameterNames.AgreementTaxType];
                        var taxRate = Convert.ToDecimal(Country[CountryParameterNames.TaxRate]);
                        //NewAccount.ApplyTaxInTaxInclusiveCountries(warranties, taxType, agreementTaxType, taxRate);

                        //why check if it is null if up there you set it to new List<....
                        if (warranties.Items != null)
                        {
                            //do you really need to type hack? EVERYTHING here is a hack...
                            //hack to make warranty prices work without changing the Web
                            foreach (WarrantyResult.Item warrantyItem in warranties.Items)
                            {
                                warrantyItem.price.RetailPrice = warrantyItem.price.TaxInclusivePriceChange;
                            }
                        }

                        // 69282 rdb 12/10/07 in the event that we are dealing with a replacement item with a warranty, where the new account screen
                        // has been reenterered and a lineitem added, the warranty will have automatically hve moved to that item (which is fine)
                        // however if more than one item is added to the account then we need a new  yes/no pop-up to move the warranty to this item
                        bool warrantyContinue = true;
                        // first check we are in the situation of having a delivered warranty attached to an undelivered item

                        //IP - 20/08/08 - (69962) - When processing an exchange on a kit component and revising the account to add the new exchange
                        //item, a prompt would appear to ask the user if they would like to move the existing kit warranty to the
                        //new item. Once the warranty had been moved and a second exchange was processed on a different kit component
                        //the prompt would appear but would incorrectly move and attach a kit component (not a warranty) to the new item.
                        //Added a check to @type being 'Warranty' or @Type = 'KitWarranty'.
                        //string refc = currentItem.Attributes[Tags.RefCode].Value;
                        //string key = productCode + "|" + branchNo.ToString();
                        string key = itemID + "|" + branchNo.ToString();                                //IP - 09/06/11 - CR1212 - RI
                        AttachWarranty(AccountNo, AgreementNo, key);
                        //XmlNode warranty = itemDoc.DocumentElement.SelectSingleNode(@"//Item[@DeliveredQuantity>0 and @ParentItemNo = '' and @RefCode = '" + refc + "' and ( @Type = 'Warranty' or @Type = 'KitWarranty')]");
                        //if (warranty != null)
                        //{
                        //    //warranty = warranty.ParentNode;
                        //    //XmlNode itemToRemoveFrom = warranty.ParentNode.ParentNode;
                        //    if (MessageBox.Show("An existing warranty '" + warranty.Attributes["Description1"].Value.Trim() + "' is available for this item.  Do you want to move it to this item?", "Warranty Available", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        //    {
                        //        // move warranty to this item
                        //        //itemToRemoveFrom.ChildNodes[0].RemoveChild(warranty);
                        //        currentItem.ChildNodes[0].AppendChild(warranty);
                        //        warrantyContinue = false;
                        //    }
                        //}

                        if (warrantyContinue)
                        {
                            // 69282 changes end

                            if (!MaximumWarrantiesPurchased(ItemKey, Convert.ToInt32(Convert.ToDouble(txtQuantity.Text)))) //68500 (jec 11/09/06)
                                AddWarranty(ItemID ?? 0,
                                            productCode,
                                            Convert.ToInt16(drpLocation.Text),
                                            Convert.ToDouble(StripCurrency(txtUnitPrice.Text)),
                                            Convert.ToDouble(txtQuantity.Text));
                        }
                    }

                    if (AT.IsCreditType(AccountType) &&
                       Country.GetCountryParameterValue<bool>(CountryParameterNames.AssociatedProductsCredit))
                    {
                        callRelatedProducts = true;
                    }
                    else if (Country.GetCountryParameterValue<bool>(CountryParameterNames.AssociatedProductsCash))
                    {
                        callRelatedProducts = true;
                    }

                    if (!Replacement)  //#17290
                    {
                        if (!this.Collection)   //#18526
                        {
                            AddFreeWarranty(productCode, branchNo, itemID + "|" + branchNo, Convert.ToInt32(currentItem.Attributes[Tags.Quantity].Value));
                        }
                    }
                    else
                    {
                        //if(AccountType == AT.Special)       // #17290
                        //{
                        var itemQty = AccountType == AT.Special ? Convert.ToInt32(instantReplacement.Quantity) : Convert.ToInt32(currentItem.Attributes[Tags.Quantity].Value);  //#18389

                        if (freeReplacementWarrantyAdded == false) //#18437
                        {
                            AttachFreeReplacementWarranty(productCode, branchNo, Convert.ToInt32(itemID), Convert.ToInt32(itemQty));  //#18731
                        }

                        //}
                    }

                    //rebuild the datagrid from the document
                    populateTable();

                    if (callRelatedProducts)  // Moved to after rebuild of datagrid   jec ???
                    {
                        ItemID = Convert.ToInt32(itemID);        // jec 17/06/11 - ItemID set to null by txtProductCode.Text = ""; in AddWarranty method
                        AddRelatedItems(ItemID ?? 0,
                            Convert.ToDouble(currentItem.Attributes[Tags.Quantity].Value),
                            Convert.ToInt16(branchNo));

                        //rebuild the datagrid from the document
                        populateTable();            //jec 20/06/11
                    }

                    if (Convert.ToBoolean(Country[CountryParameterNames.LoyaltyScheme]))
                    {
                        CheckForVoucher(true, txtProductCode.Text);
                        CheckForFreeDelivery();
                    }


                    ClearItemDetails();

                    ProductCode = "";

                    dgLineItems.DataSource = itemsView;
                    currentItem = null;
                    this.CheckAffinityTerm();
                }

                if (PaidAndTaken) this.SetAmountEnabled();

                //IP - 03/12/10 - Store Card - If there were items that exceeded the maximum item value 
                //we need to check if these are now removed.
                if (PaidAndTaken && PayMethod.IsPayMethod(curPayMethod, PayMethod.StoreCard))
                {
                    CheckMaxItemVal();
                }

                accountWideDiscount = false;  //#14051

                if (Renewal)
                {
                    drpBranch.Enabled = false;
                }


            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void OnEditContractNo(object sender, EventArgs e)
        {
            AddAffinityContractNo(affinityItem);
            populateTable();
        }

        private void OnEditReturnDate(object sender, EventArgs e)
        {
            ExpectedReturnDate er = new ExpectedReturnDate(affinityItem);
            er.ShowDialog();
        }

        private void AddAffinityContractNo(XmlNode item)
        {
            AffinityContractNo a = new AffinityContractNo(FormRoot, this, item);
            a.ShowDialog();
        }

        private void OnAddSPIFF(object sender, EventArgs e)
        {
            bool status = true;
            errorProvider1.SetError(drpSoldBy, "");

            if (drpSoldBy.SelectedIndex == 0)
            {
                drpSoldBy.Focus();
                errorProvider1.SetError(drpSoldBy, GetResource("M_ENTERMANDATORY"));
                status = false;
            }

            if (status)
            {
                string salesPersonStr = drpSoldBy.SelectedItem.ToString();
                int salesPerson = Convert.ToInt32(salesPersonStr.Substring(0, salesPersonStr.IndexOf(":") - 1));
                var addSpiff = new AddSpiff(AccountNo, currentItem.Attributes[Tags.Code].Value, Convert.ToInt16(currentItem.Attributes[Tags.Location].Value), AgreementNo, salesPerson);
                addSpiff.ShowDialog();

                if (addSpiff.SpiffAdded)
                {
                    string xPath = "//Item[@Code='" + currentItem.Attributes[Tags.Code].Value +
                        "' and @Location='" + currentItem.Attributes[Tags.Location].Value +
                        "' and @ContractNumber='" + currentItem.Attributes[Tags.ContractNumber].Value + "']";
                    XmlNode found = itemDoc.SelectSingleNode(xPath);

                    if (found != null)
                        found.Attributes[Tags.SPIFFItem].Value = Convert.ToBoolean(1).ToString();
                }
            }

            ClearItemDetails();
            ProductCode = "";
        }

        private void OnRemoveSPIFF(object sender, EventArgs e)
        {
            try
            {
                Wait();
                AccountManager.DeleteSpiff(AccountNo, currentItem.Attributes[Tags.Code].Value,
                                        Convert.ToInt16(currentItem.Attributes[Tags.Location].Value), out Error);
                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    string xPath = "//Item[@Code='" + currentItem.Attributes[Tags.Code].Value +
                        "' and @Location='" + currentItem.Attributes[Tags.Location].Value +
                        "' and @ContractNumber='" + currentItem.Attributes[Tags.ContractNumber].Value + "']";
                    XmlNode found = itemDoc.SelectSingleNode(xPath);

                    if (found != null)
                        found.Attributes[Tags.SPIFFItem].Value = Convert.ToBoolean(0).ToString();
                }

                ClearItemDetails();
                ProductCode = "";
            }
            catch (Exception ex)
            {
                Catch(ex, "OnRemoveSPIFF");
            }
            finally
            {
                StopWait();
            }
        }

        private void populateTable()
        {
            itemsTable.Clear();
            tvItems.Nodes.Clear();
            tvItems.Nodes.Add(new TreeNode("Account"));

            double subTotal = 0;
            ChargeableSubTotal = 0;
            ChargeableSalesTax = 0;
            ChargeableAdminSubTotal = 0;
            nonStockTotal = 0;
            loanTotal = 0;
            tier2SubTotal = 0;
            WarrantiesOnCreditAgreementTotal = 0;

            _affinityTerms = false;
            populateTable(itemDoc.DocumentElement, tvItems.Nodes[0], ref subTotal, ref this._affinityTerms);

            subTotal += Convert.ToDouble(MoneyStrToDecimal(txtPrivClubDiscount.Text));

            /* JJ - applying the filter may change the positions of the terms types in 
			 * the list and therefore the selected index may need to change also
			 * after the filter has been changed */
            //string temp = drpTermsType.Text;

            //// Filter the terms type for affinity
            //DataView dvTermsType = (DataView)drpTermsType.DataSource;

            //// Get the current setting
            //string curTermsType = drpTermsType.Text;

            //FilterTermsType(ref dvTermsType, _affinityTerms, drpAccountType.Text, _scoringBand, SType, IsLoan);

            //// Make sure the TermsType has not changed if it is still available
            //int selectedTermsType = drpTermsType.FindStringExact(curTermsType);
            //drpTermsType.SelectedIndex = (selectedTermsType != -1) ? selectedTermsType : 0;

            //if (temp.Length > 0)
            //    drpTermsType.SelectedIndex = drpTermsType.FindString(temp) == -1 ? 0 : drpTermsType.FindString(temp);

            tvItems.ExpandAll();

            if (itemsView.Count > 0)
            {
                dgLineItems.CurrentRowIndex = 0;
                chxTaxExempt.Enabled = false;
                chxDutyFree.Enabled = false;
                btnRemove.Enabled = true;
            }
            else
            {
                btnRemove.Enabled = false;
                chxTaxExempt.Enabled = allowTaxExempt.Enabled;
                chxDutyFree.Enabled = true;
            }

            CalculateServiceCharge(Config.CountryCode,
                Convert.ToDecimal(StripCurrency(txtDeposit.Text)),
                Convert.ToDecimal(txtNoMonths.Value),
                Convert.ToDecimal(subTotal),
                Convert.ToDecimal(numPaymentHolidays.Value));

            decimal st = Convert.ToDecimal(StripCurrency(txtSubTotal.Text)) - Convert.ToDecimal(StripCurrency(txtDeposit.Text));
            // cr906 we do not want to subtract Loan items  from sub total 
            // for  nonStockTotal when we are calculating

            if ((st - (nonStockTotal - loanTotal)) > RFMax && this.AccountType == AT.ReadyFinance)
            {
                txtRFMax.BackColor = Color.Red;
                txtRFMax.ForeColor = Color.White;
                txtSubTotal.BackColor = Color.Red;
                txtSubTotal.ForeColor = Color.White;
            }
            else
            {
                txtRFMax.BackColor = SystemColors.Control;
                txtRFMax.ForeColor = SystemColors.ControlText;
                txtSubTotal.BackColor = SystemColors.Control;
                txtSubTotal.ForeColor = SystemColors.ControlText;
            }

            /* default the amount paid to the amount due */
            txtDue.Text = txtSubTotal.Text;
            st = Convert.ToDecimal(StripCurrency(txtSubTotal.Text));

            /* if paying my gift voucher then the amount must be at least the value of 
			 * the voucher */
            if (chxGiftVoucher.Checked)
            {
                txtDue.Text = (MoneyStrToDecimal(txtDue.Text) - GiftVoucherValue).ToString(DecimalPlaces);
            }
            if (Replacement)
            {
                decimal oldVal = instantReplacement.OrderValue + instantReplacement.TaxAmount;
                txtDue.Text = (st - oldVal).ToString(DecimalPlaces);
            }

            // Check the agreement term is not too long if selling an Affinity item
            CheckAffinityTerm();

            txtAmount.Text = txtDue.Text;
            txtAmount_Validating(null, null);
        }

        /// <summary>
        /// This method calls a web service to return details of the agreement
        /// including instalment plan and service charges.
        /// </summary>
        /// <param name="countryCode">Country Code</param>
        /// <param name="termsType">Terms Type code</param>
        /// <param name="deposit">deposit entered</param>
        /// <param name="months">number of months for agreement</param>
        /// <param name="subTotal">current subtotal</param>
        private void CalculateServiceCharge(string countryCode, decimal newDeposit, decimal months,
            decimal subTotal, decimal paymentHolidays)
        {
            Function = "CalculateServiceCharge";
            decimal tax = 0;
            decimal monthly = 0;
            decimal final = 0;
            decimal deferredTerms = 0;
            this._hasdatachanged = true;
            //10.6 CR- Sales Order - Print Save- Allow user to save while printing 
            //and track UI changes to increment invoice version
            _isSalesOrderChanged = true;
            decimal insuranceCharge = 0;
            decimal adminCharge = 0;
            decimal dtTax = dtTaxAmount = 0;
            decimal insuranceTax = 0;
            decimal adminTax = 0;
            decimal totalInstalments = 0;
            decimal taxWarrantyOnCredit = 0; //IP - 01/05/08 - UAT(362)

            string sBranchno = _acctNo.Length != 0 ? _acctNo.Substring(0, 3) : drpBranch.SelectedItem.ToString();

            insuranceChargeItemId = StockItemCache.Get(StockItemKeys.InsuranceChargeItem);      //IP - 25/05/11 - CR1212 - #3668
            adminChargeItemId = StockItemCache.Get(StockItemKeys.AdminChargeItem);              //IP - 25/05/11 - CR1212 - #3668
            staxItemId = StockItemCache.Get(StockItemKeys.STAX);                                //IP - 25/05/11 - CR1212 - #3668
            tier2DiscountItemId = StockItemCache.Get(StockItemKeys.Tier2DiscountItemNumber);    //IP - 25/05/11 - CR1212 - #3668

            // Subtract any privilege Club discount if eligible
            if (this.hasPClubDiscount)
            {
                if (((string)Country[CountryParameterNames.Tier2DiscountItemNumber]).Trim().Length < 2)
                {
                    // The Tier2 discount item code has not been set up in country maintenance
                    // A Tier2 discount cannot be added
                    ShowInfo("M_T2NODISCOUNTCODE");
                }
                else
                {
                    // Check whether a discount is already applied
                    XmlNode discountNode = itemDoc.SelectSingleNode("//Item[@Code = '" + (string)Country[CountryParameterNames.Tier2DiscountItemNumber] + "']");
                    if (discountNode != null)
                    {
                        // Adjust to the actual subtotal
                        subTotal -= Convert.ToDecimal(discountNode.Attributes[Tags.Value].Value);
                    }

                    // Calculate the new discount
                    decimal tier2Discount = -(tier2SubTotal / 100) * (decimal)Country[CountryParameterNames.Tier2Discount];
                    // Make sure this is rounded for the country
                    tier2Discount = MoneyStrToDecimal(tier2Discount.ToString(DecimalPlaces));

                    if (Math.Abs(tier2Discount) >= 0.01M)
                    {
                        if (discountNode == null)
                        {
                            // Add a new discount line
                            discountNode = AccountManager.GetItemDetails(new STL.PL.WS2.GetItemDetailsRequest
                            {
                                ProductCode = (string)Country[CountryParameterNames.Tier2DiscountItemNumber],
                                StockLocationNo = Convert.ToInt16(Config.BranchCode),
                                BranchCode = Convert.ToInt16(Config.BranchCode),
                                AccountType = this.AccountType,
                                CountryCode = Config.CountryCode,
                                AccountNo = this.AccountNo,
                                AgrmtNo = this.AgreementNo,
                                ItemID = tier2DiscountItemId
                            }, out Error);

                            if (Error.Length > 0)
                                ShowError(Error);
                            else
                            {
                                discountNode.Attributes[Tags.DeliveryDate].Value = DateTime.MinValue.AddYears(1899).ToString();
                                discountNode.Attributes[Tags.BranchForDeliveryNote].Value = drpBranchForDel.Text;
                                discountNode = itemDoc.ImportNode(discountNode, true);
                                itemDoc.DocumentElement.AppendChild(discountNode);
                            }
                        }

                        // Update the discount line
                        discountNode.Attributes[Tags.UnitPrice].Value = tier2Discount.ToString();
                        discountNode.Attributes[Tags.Value].Value = tier2Discount.ToString();
                        discountNode.Attributes[Tags.Quantity].Value = "1";
                        decimal tier2DiscountTax = AccountManager.CalculateTaxAmount(Config.CountryCode, discountNode, chxTaxExempt.Checked, out Error);
                        discountNode.Attributes[Tags.TaxAmount].Value = tier2DiscountTax.ToString();
                        // Display the Tier2 discount
                        txtPrivClubDiscount.Text = tier2Discount.ToString(DecimalPlaces);
                        subTotal += tier2Discount;
                    }
                    else
                    {
                        txtPrivClubDiscount.Text = "";
                        // Remove a zero discount line
                        if (discountNode != null)
                            discountNode.ParentNode.RemoveChild(discountNode);
                    }
                }
            }


            XmlNode stax = null;
            //Send the lineitems doc to the back end to calculate any sales
            //tax that will need to be added
            /*
			ChargeableSubTotal -= ChargeableSalesTax;
			ChargeableAdminSubTotal -= ChargeableSalesTax;
			*/

            ChargeableSubTotal = 0;
            ChargeableAdminSubTotal = 0;
            RecalculateChargeableTotals(itemDoc.DocumentElement);

            if (!chxTaxExempt.Checked && (string)Country[CountryParameterNames.AgreementTaxType] == "E" && (decimal)Country[CountryParameterNames.TaxRate] > 0)
            {
                //IP - 30/04/08 - UAT(362) V 5.1
                //tax = AccountManager.CalculateSalesTax(Config.CountryCode, itemDoc.DocumentElement, drpAccountType.Text, chxDutyFree.Checked, IncludeWarranty, ref ChargeableAdminTax, ref ChargeableSalesTax, out Error);
                tax = AccountManager.CalculateSalesTax(Config.CountryCode, itemDoc.DocumentElement, drpAccountType.Text, chxDutyFree.Checked, IncludeWarranty, WarrantiesOnCredit, ref ChargeableAdminTax, ref ChargeableSalesTax, ref taxWarrantyOnCredit, out Error);
                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    stax = itemDoc.SelectSingleNode("//Item[@Code = 'STAX']");
                    /* stax = xml.findItem(itemDoc.DocumentElement, "STAX|"); doesn't work JJ */
                    if (stax == null)
                    {
                        stax = AccountManager.GetItemDetails(new STL.PL.WS2.GetItemDetailsRequest
                        {
                            ItemID = staxItemId,
                            StockLocationNo = Convert.ToInt16((string)drpBranch.SelectedItem),
                            BranchCode = Convert.ToInt16(Config.BranchCode),
                            AccountType = this.AccountType,
                            CountryCode = Config.CountryCode,
                            AccountNo = this.AccountNo,
                            AgrmtNo = this.AgreementNo
                        }, out Error);

                        if (Error.Length > 0)
                            ShowError(Error);
                        else
                        {
                            stax.Attributes[Tags.DeliveryDate].Value = DateTime.MinValue.AddYears(1899).ToString();
                            stax.Attributes[Tags.BranchForDeliveryNote].Value = drpBranchForDel.Text; //drpLocation.Text;
                            stax.Attributes[Tags.UnitPrice].Value = tax.ToString();
                            stax = itemDoc.ImportNode(stax, true);
                            itemDoc.DocumentElement.AppendChild(stax);
                        }
                    }

                    //set STAX qty here as it may already exist in the xml documnet but
                    //have a qty of 0.
                    stax.Attributes[Tags.Quantity].Value = "1";
                }
            }
            subTotal += tax;
            ChargeableSubTotal += ChargeableSalesTax;   //this needs to be chargeable tax
            ChargeableAdminSubTotal += ChargeableAdminTax;

            // Check the deposit is at least the minimum for the terms type
            if (!_loadAccount)
            {
                SetDeposit(this.cbDeposit, this.txtDeposit,
                    _defaultDeposit, _depositIsPercentage, ref newDeposit, subTotal, true, CheckDepositWaiver());

                // Adjust the RF Spend Available by the change in the Deposit amount
                RFMax = RFMax - _deposit + newDeposit;
                _deposit = newDeposit;
            }

            if (drpTermsType.SelectedIndex != 0 &&
                /*(drpAccountType.Text != AT.Cash || drpAccountType.Text != AT.Special))  this will always be true! */
                //( drpAccountType.Text != AT.Cash && drpAccountType.Text != AT.Special ))
                AT.IsCreditType(drpAccountType.Text))
            {
                string terms = drpTermsType.Text;
                terms = terms.Substring(0, terms.IndexOf("-") - 1);

                variableRatesSet.Tables.Clear();
                if (terms == _origTerms && _scoringBand == _origScoringBand)
                {
                    // Revising with the original terms type and scoring band - so use historic SC%
                    deferredTerms = AccountManager.CalculateServiceCharge(countryCode, terms, this.AccountNo, this._scoringBand, newDeposit, months, ChargeableSubTotal, DateAccountOpened, drpAccountType.Text, ChargeableAdminSubTotal, ref insuranceCharge, ref adminCharge, ref variableRatesSet, out Error);
                }
                else
                {
                    // Revising with a new terms type or scoring band - so use current SC%

                    //If scoring band in the database is '' then pass _origScoringBand UAT163 JH 22/11/2007 
                    // 5.1 uat253 rdb 10/12/07 deferred terms should be calculated from date account opened
                    // note: I am unsure what needs to happen when a terms type is changed
                    if (_origScoringBand == String.Empty && this._currentBand != String.Empty)
                    {
                        //deferredTerms = AccountManager.CalculateServiceCharge(countryCode, terms, this.AccountNo, this._origScoringBand, newDeposit, months, ChargeableSubTotal, _today, drpAccountType.Text, ChargeableAdminSubTotal, ref insuranceCharge, ref adminCharge, ref variableRatesSet, out Error);
                        deferredTerms = AccountManager.CalculateServiceCharge(countryCode, terms, this.AccountNo, this._currentBand, newDeposit, months, ChargeableSubTotal, DateAccountOpened, drpAccountType.Text, ChargeableAdminSubTotal, ref insuranceCharge, ref adminCharge, ref variableRatesSet, out Error);
                    }
                    else
                    {
                        //deferredTerms = AccountManager.CalculateServiceCharge(countryCode, terms, this.AccountNo, this._scoringBand, newDeposit, months, ChargeableSubTotal, _today, drpAccountType.Text, ChargeableAdminSubTotal, ref insuranceCharge, ref adminCharge, ref variableRatesSet, out Error);
                        deferredTerms = AccountManager.CalculateServiceCharge(countryCode, terms, this.AccountNo, this._scoringBand, newDeposit, months, ChargeableSubTotal, DateAccountOpened, drpAccountType.Text, ChargeableAdminSubTotal, ref insuranceCharge, ref adminCharge, ref variableRatesSet, out Error);
                    }
                }
                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    #region insurance charge
                    XmlNode insuranceNode = xml.findItem(itemDoc.DocumentElement, insuranceChargeItemId + "|" + sBranchno);
                    if (insuranceNode == null && insuranceCharge > 0)
                    {
                        //LiveWire Issue 68015 - Create an insurance item if one does
                        //not already exist and the insurance charge is greater than 0.
                        insuranceNode = AccountManager.GetItemDetails(new STL.PL.WS2.GetItemDetailsRequest
                        {
                            ProductCode = (string)Country[CountryParameterNames.InsuranceChargeItem],
                            StockLocationNo = Convert.ToInt16(sBranchno),
                            BranchCode = Convert.ToInt16(sBranchno),
                            AccountType = this.AccountType,
                            CountryCode = Config.CountryCode,
                            AccountNo = this.AccountNo,
                            AgrmtNo = this.AgreementNo,
                            ItemID = insuranceChargeItemId
                        }, out Error);

                        if (Error.Length > 0)
                            ShowError(Error);
                        else
                        {
                            insuranceNode.Attributes[Tags.DeliveryDate].Value = DateTime.MinValue.AddYears(1899).ToString();
                            insuranceNode.Attributes[Tags.BranchForDeliveryNote].Value = drpBranchForDel.Text; // drpLocation.Text;
                            insuranceNode = itemDoc.ImportNode(insuranceNode, true);
                            itemDoc.DocumentElement.AppendChild(insuranceNode);
                        }
                    }
                    if (insuranceNode != null)
                    {
                        if (insuranceCharge <= 0)
                        {
                            //LiveWire Call 69215 - If termstype has been revised and an insurance\admin
                            //charge no longer applies, then the existing insurance\admin charge should
                            //be set to zero.
                            if (Convert.ToDecimal(insuranceNode.Attributes[Tags.Value].Value) > 0)
                            {
                                insuranceNode.Attributes[Tags.Quantity].Value = "0";
                                insuranceNode.Attributes[Tags.Value].Value = "0";
                                deleteInsuranceNode = false;
                            }

                            if (deleteInsuranceNode)
                            {
                                /* make sure we don't leave zero value nodes lying around */
                                insuranceNode.ParentNode.RemoveChild(insuranceNode);
                            }
                        }
                        else
                        {
                            //NM ------------------------------------------------------
                            //UAT 645 - Insurance must be calculated on taxed value - if taxtype CountryParam = E
                            //if ((string)Country[CountryParameterNames.TaxType] == "E")
                            //{
                            if ((string)Country[CountryParameterNames.TaxType] == "I" || (string)Country[CountryParameterNames.TaxType] == "E")
                            {
                                //If agreement tax inclusive we need to add tax onto Insurance and not consider the Insurance as already including tax
                                if ((string)Country[CountryParameterNames.AgreementTaxType] == "I")
                                {
                                    insuranceCharge = insuranceCharge + (insuranceCharge * Convert.ToDecimal(insuranceNode.Attributes[Tags.TaxRate].Value)) / 100;
                                }
                            }

                            //---------------------------------------------------------
                            insuranceNode.Attributes[Tags.UnitPrice].Value = StripCurrency(insuranceCharge.ToString(DecimalPlaces));
                            insuranceNode.Attributes[Tags.Value].Value = StripCurrency(insuranceCharge.ToString(DecimalPlaces));
                            insuranceNode.Attributes[Tags.Quantity].Value = insuranceCharge > 0 ? "1" : "0";
                            insuranceTax = AccountManager.CalculateTaxAmount(Config.CountryCode, insuranceNode, chxTaxExempt.Checked, out Error);
                            insuranceNode.Attributes[Tags.TaxAmount].Value = insuranceTax.ToString();
                        }
                    }

                    saleOrderInsuranceTax = insuranceTax;
                    #endregion

                    #region admin charge
                    XmlNode adminNode = xml.findItem(itemDoc.DocumentElement, adminChargeItemId + "|" + sBranchno);
                    if (adminNode == null && adminCharge > 0)
                    {
                        //LiveWire Issue 68015 - Create an admin item if one does
                        //not already exist and the admin charge is greater than 0.
                        adminNode = AccountManager.GetItemDetails(new STL.PL.WS2.GetItemDetailsRequest
                        {
                            ProductCode = (string)Country[CountryParameterNames.AdminChargeItem],
                            StockLocationNo = Convert.ToInt16(sBranchno),
                            BranchCode = Convert.ToInt16(sBranchno),
                            AccountType = this.AccountType,
                            CountryCode = Config.CountryCode,
                            AccountNo = this.AccountNo,
                            AgrmtNo = this.AgreementNo,
                            ItemID = adminChargeItemId
                        }, out Error);

                        if (Error.Length > 0)
                            ShowError(Error);
                        else
                        {
                            adminNode.Attributes[Tags.DeliveryDate].Value = DateTime.MinValue.AddYears(1899).ToString();
                            adminNode.Attributes[Tags.BranchForDeliveryNote].Value = drpBranchForDel.Text; //drpLocation.Text;
                            adminNode = itemDoc.ImportNode(adminNode, true);
                            itemDoc.DocumentElement.AppendChild(adminNode);
                        }
                    }
                    if (adminNode != null)
                    {
                        if (adminCharge <= 0)
                        {
                            //LiveWire Call 69215 - If termstype has been revised and an insurance\admin
                            //charge no longer applies, then the existing insurance\admin charge should
                            //be set to zero.
                            if (Convert.ToDecimal(adminNode.Attributes[Tags.Value].Value) > 0)
                            {
                                adminNode.Attributes[Tags.Quantity].Value = "0";
                                adminNode.Attributes[Tags.Value].Value = "0";
                                deleteAdminNode = false;
                            }

                            if (deleteAdminNode)
                            {
                                /* make sure we don't leave zero value nodes lying around */
                                adminNode.ParentNode.RemoveChild(adminNode);
                            }
                        }
                        else
                        {
                            //If agreement tax inclusive we need to add tax onto Admin and not consider the Admin as already including tax
                            if ((string)Country[CountryParameterNames.TaxType] == "I" || (string)Country[CountryParameterNames.TaxType] == "E")
                            {
                                if ((string)Country[CountryParameterNames.AgreementTaxType] == "I")
                                {
                                    adminCharge = adminCharge + (adminCharge * Convert.ToDecimal(adminNode.Attributes[Tags.TaxRate].Value)) / 100;
                                }
                            }


                            adminNode.Attributes[Tags.UnitPrice].Value = StripCurrency(adminCharge.ToString(DecimalPlaces));
                            adminNode.Attributes[Tags.Value].Value = StripCurrency(adminCharge.ToString(DecimalPlaces));
                            adminNode.Attributes[Tags.Quantity].Value = adminCharge > 0 ? "1" : "0";
                            adminTax = AccountManager.CalculateTaxAmount(Config.CountryCode, adminNode, chxTaxExempt.Checked, out Error);
                            adminNode.Attributes[Tags.TaxAmount].Value = adminTax.ToString();
                        }
                    }
                    saleOrderAdminTax = adminTax;

                    #endregion
                    //XmlNode dt = xml.findItem(itemDoc.DocumentElement, "DT|" + sBranchno);

                    XmlNode dt = xml.findItem(itemDoc.DocumentElement, Convert.ToString(StockItemCache.Get(StockItemKeys.DT)) + "|" + sBranchno); //IP - CR1212 - RI - #3668 - DT was not being found as ID should now be part of the key when searching the xml
                    if (dt == null)
                    {
                        dt = AccountManager.GetItemDetails(new STL.PL.WS2.GetItemDetailsRequest
                        {
                            ProductCode = "DT",
                            ItemID = StockItemCache.Get(StockItemKeys.DT),//IP/NM - 18/05/11 -CR1212 - #3627
                            StockLocationNo = Convert.ToInt16(sBranchno),
                            BranchCode = Convert.ToInt16(sBranchno),
                            AccountType = this.AccountType,
                            CountryCode = Config.CountryCode,
                            AccountNo = this.AccountNo,
                            AgrmtNo = this.AgreementNo
                        }, out Error);

                        if (Error.Length > 0)
                            ShowError(Error);
                        else
                        {
                            dt.Attributes[Tags.DeliveryDate].Value = DateTime.MinValue.AddYears(1899).ToString();
                            dt.Attributes[Tags.BranchForDeliveryNote].Value = drpBranchForDel.Text; //drpLocation.Text;
                            dt = itemDoc.ImportNode(dt, true);
                            itemDoc.DocumentElement.AppendChild(dt);
                        }
                    }

                    //Add item to cal DT if not HC voucher
                    XmlNode Voucher = itemDoc.SelectSingleNode("//Item[@Code = '" + LoyaltyDropStatic.VoucherCode + "']");

                    if (Voucher != null)
                    {
                        saleOrderVoucherValue = Convert.ToDecimal(Voucher.Attributes[Tags.Value].Value);
                        deferredTerms += Convert.ToDecimal(Voucher.Attributes[Tags.Value].Value);
                        if (deferredTerms < 0)
                        {
                            deferredTerms = 0;
                        }
                    }

                    //set DT qty here as it may already exist in the xml documnet but
                    //have a qty of 0.
                    dt.Attributes[Tags.Quantity].Value = "1";
                    dt.Attributes[Tags.UnitPrice].Value = StripCurrency(deferredTerms.ToString(DecimalPlaces));
                    dt.Attributes[Tags.Value].Value = StripCurrency(deferredTerms.ToString(DecimalPlaces));
                    saleOrderDT = dt;
                    dtTaxAmount = dtTax = AccountManager.CalculateTaxAmount(Config.CountryCode, dt, chxTaxExempt.Checked, out Error);

                    dtTaxAmount += adminTax + insuranceTax;

                    if ((string)Country[CountryParameterNames.AgreementTaxType] == "I")
                        dtTaxAmount = dtTax = adminTax = insuranceTax = 0;

                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                    {
                        cmInsuranceTax = insuranceTax;
                        cmAdminTax = adminTax;
                        mmiSubTotal = subTotal;
                        mmiDeposit = newDeposit;
                        mmiDeferredTerms = deferredTerms + dtTax + insuranceCharge + insuranceTax + adminCharge + adminTax;
                        mmiMonths = months - paymentHolidays;

                        AccountManager.CalculateInstalPlan(subTotal, newDeposit, deferredTerms + dtTax + insuranceCharge + insuranceTax + adminCharge + adminTax, (months - paymentHolidays), out monthly, out final, out Error);
                        if (Error.Length > 0)
                            ShowError(Error);
                        else
                        {
                            txtInstalment.Text = monthly.ToString(DecimalPlaces);
                            txtFinalInstalment.Text = final.ToString(DecimalPlaces);

                            if (variableRatesSet.Tables.Contains(TN.Rates))
                            {
                                // variable interest rates exist for this terms type so
                                // we must set the instalment amount to be the same as
                                // the first instalment amount for the first period of 
                                // variable rates.
                                variableRatesSet.Tables[TN.Rates].DefaultView.Sort = CN.InstalOrder + " ASC";
                                txtInstalment.Text = Convert.ToDecimal(variableRatesSet.Tables[0].Rows[0][CN.Instalment2]).ToString();

                                foreach (DataRow r in variableRatesSet.Tables[0].Rows)
                                    totalInstalments += Convert.ToDecimal(r[CN.InstalmentNumber]) * Convert.ToDecimal(r[CN.Instalment2]);

                                txtInstalment.Visible = false;
                                label20.Visible = false;
                                btnPlan.Visible = true;
                            }
                            else
                            {
                                txtInstalment.Visible = true;
                                label20.Visible = true;
                                btnPlan.Visible = false;
                            }
                            //UAT48 jec 01/03/10  - code duplicated
                            //XmlNode VoucherCode = itemDoc.SelectSingleNode("//Item[@Code = '" + LoyaltyDropStatic.VoucherCode + "']");

                            //if (VoucherCode != null)
                            //{
                            //    deferredTerms += Convert.ToDecimal(Voucher.Attributes[Tags.Value].Value);
                            //    if (deferredTerms < 0)
                            //    {
                            //        deferredTerms = 0;
                            //    }
                            //}

                            txtTerms.Text = deferredTerms.ToString(DecimalPlaces);
                        }
                    }

                    if (!chxTaxExempt.Checked && (string)Country[CountryParameterNames.AgreementTaxType] == "E")
                    {
                        subTotal += dtTax + adminTax + insuranceTax;
                        tax += dtTax + adminTax + insuranceTax;
                    }
                }
            }
            if (PaidAndTaken)       //jec 19/05/08 UAT452 
            {
                txtSubTotal.Text = subTotal.ToString(DecimalPlaces);
                txtSalesTax.Text = tax.ToString(DecimalPlaces);
            }
            else
            {
                txtSubTotal.Text = (subTotal + taxWarrantyOnCredit).ToString(DecimalPlaces); //jec 19/05/08 UAT452
                txtSalesTax.Text = (tax + taxWarrantyOnCredit).ToString(DecimalPlaces);         //jec 19/05/08 UAT452 
            }

            //txtSalesTax.Text = tax.ToString(DecimalPlaces);
            txtAdminCharge.Text = adminCharge.ToString(DecimalPlaces);
            txtInsuranceCharge.Text = insuranceCharge.ToString(DecimalPlaces);

            if (stax != null)
            {
                decimal staxAmount = tax + taxWarrantyOnCredit;
                //IP - 01/05/08 - UAT(362) - Update STAX 'Value' and 'Unitprice' as the tax on any items and warranties on credit.
                //stax.Attributes[Tags.Value].Value = StripCurrency(tax.ToString(DecimalPlaces));
                //stax.Attributes[Tags.UnitPrice].Value = StripCurrency(tax.ToString(DecimalPlaces));
                stax.Attributes[Tags.Value].Value = StripCurrency(staxAmount.ToString(DecimalPlaces));
                stax.Attributes[Tags.UnitPrice].Value = StripCurrency(staxAmount.ToString(DecimalPlaces));
            }
            //AgreementTotal = subTotal + deferredTerms;	- this could cause a rounding error
            AgreementTotal = Convert.ToDecimal(StripCurrency(txtSubTotal.Text)) +
                                Convert.ToDecimal(StripCurrency(txtTerms.Text)) +
                                MoneyStrToDecimal(txtAdminCharge.Text) +
                                MoneyStrToDecimal(txtInsuranceCharge.Text);

            // if variable interest rates exists we must set the final instalment to be:
            // agreementTotal - totalVariableInstalments
            if (variableRatesSet.Tables.Contains(TN.Rates))
                txtFinalInstalment.Text = Convert.ToDecimal(AgreementTotal - totalInstalments).ToString(DecimalPlaces);
        }


        private decimal CalculateMonthToExtend(string countryCode, decimal deposit, decimal months, decimal subTotal, decimal paymentHolidays, decimal mmiThresholdLimit)
        {
            Function = "CalculateMonthToExtend";

            decimal cmDtTax = 0;
            decimal monthToExtend = 0;

            string sBranchno = _acctNo.Length != 0 ? _acctNo.Substring(0, 3) : drpBranch.SelectedItem.ToString();

            if (drpTermsType.SelectedIndex != 0 && AT.IsCreditType(drpAccountType.Text))
            {
                string terms = drpTermsType.Text;
                terms = terms.Substring(0, terms.IndexOf("-") - 1);
                string band = string.Empty;

                variableRatesSet.Tables.Clear();
                if (terms == _origTerms && _scoringBand == _origScoringBand)
                {
                    band = this._scoringBand;
                }
                else
                {
                    if (_origScoringBand == String.Empty && this._currentBand != String.Empty)
                    {
                        band = this._currentBand;
                    }
                    else
                    {
                        band = this._scoringBand;
                    }
                }

                decimal monthlyInstalment = Convert.ToDecimal(StripCurrency(txtInstalment.Text));
                monthToExtend = AccountManager.CalculateMonthToExtend(countryCode, terms, this.AccountNo, band, DateAccountOpened
                                                                    , deposit, months, ChargeableSubTotal, drpAccountType.Text
                                                                    , ChargeableAdminSubTotal, monthlyInstalment
                                                                    //, ref cmInsuranceCharge, ref cmAdminCharge
                                                                    , saleOrderInsuranceTax, saleOrderAdminTax, cmDtTax, mmiThresholdLimit
                                                                    , ref variableRatesSet, saleOrderVoucherValue, saleOrderDT, chxTaxExempt.Checked
                                                                    , out Error);

                if (Error.Length > 0)
                    ShowError(Error);
            }

            return monthToExtend;
        }

        private decimal CalculateServiceChargeForMonthCount(string countryCode, decimal newDeposit, decimal months, decimal subTotal, decimal paymentHolidays)
        {
            Function = "CalculateServiceChargeForMonthCount";

            decimal cmDeferredTerms = 0;
            decimal cmDtTax = 0;
            decimal cmInsuranceCharge = 0;
            decimal cmAdminCharge = 0;


            string sBranchno = _acctNo.Length != 0 ? _acctNo.Substring(0, 3) : drpBranch.SelectedItem.ToString();

            if (drpTermsType.SelectedIndex != 0 && AT.IsCreditType(drpAccountType.Text))
            {
                string terms = drpTermsType.Text;
                terms = terms.Substring(0, terms.IndexOf("-") - 1);

                variableRatesSet.Tables.Clear();
                if (terms == _origTerms && _scoringBand == _origScoringBand)
                {
                    // Revising with the original terms type and scoring band - so use historic SC%
                    cmDeferredTerms = AccountManager.CalculateServiceCharge(countryCode, terms, this.AccountNo, this._scoringBand, newDeposit, months, ChargeableSubTotal, DateAccountOpened, drpAccountType.Text, ChargeableAdminSubTotal, ref cmInsuranceCharge, ref cmAdminCharge, ref variableRatesSet, out Error);
                }
                else
                {
                    if (_origScoringBand == String.Empty && this._currentBand != String.Empty)
                    {
                        cmDeferredTerms = AccountManager.CalculateServiceCharge(countryCode, terms, this.AccountNo, this._currentBand, newDeposit, months, ChargeableSubTotal, DateAccountOpened, drpAccountType.Text, ChargeableAdminSubTotal, ref cmInsuranceCharge, ref cmAdminCharge, ref variableRatesSet, out Error);
                    }
                    else
                    {
                        cmDeferredTerms = AccountManager.CalculateServiceCharge(countryCode, terms, this.AccountNo, this._scoringBand, newDeposit, months, ChargeableSubTotal, DateAccountOpened, drpAccountType.Text, ChargeableAdminSubTotal, ref cmInsuranceCharge, ref cmAdminCharge, ref variableRatesSet, out Error);
                    }
                }
                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    XmlNode dt = xml.findItem(itemDoc.DocumentElement, Convert.ToString(StockItemCache.Get(StockItemKeys.DT)) + "|" + sBranchno); //IP - CR1212 - RI - #3668 - DT was not being found as ID should now be part of the key when searching the xml
                    if (dt == null)
                    {
                        dt = AccountManager.GetItemDetails(new STL.PL.WS2.GetItemDetailsRequest
                        {
                            ProductCode = "DT",
                            ItemID = StockItemCache.Get(StockItemKeys.DT),
                            StockLocationNo = Convert.ToInt16(sBranchno),
                            BranchCode = Convert.ToInt16(sBranchno),
                            AccountType = this.AccountType,
                            CountryCode = Config.CountryCode,
                            AccountNo = this.AccountNo,
                            AgrmtNo = this.AgreementNo
                        }, out Error);

                        if (Error.Length > 0)
                            ShowError(Error);
                        else
                        {
                            dt.Attributes[Tags.DeliveryDate].Value = DateTime.MinValue.AddYears(1899).ToString();
                            dt.Attributes[Tags.BranchForDeliveryNote].Value = drpBranchForDel.Text; //drpLocation.Text;
                            dt = itemDoc.ImportNode(dt, true);
                            itemDoc.DocumentElement.AppendChild(dt);
                        }
                    }

                    //Add item to cal DT if not HC voucher
                    XmlNode Voucher = itemDoc.SelectSingleNode("//Item[@Code = '" + LoyaltyDropStatic.VoucherCode + "']");

                    if (Voucher != null)
                    {
                        cmDeferredTerms += Convert.ToDecimal(Voucher.Attributes[Tags.Value].Value);
                        if (cmDeferredTerms < 0)
                        {
                            cmDeferredTerms = 0;
                        }
                    }

                    //set DT qty here as it may already exist in the xml documnet but
                    //have a qty of 0.
                    dt.Attributes[Tags.Quantity].Value = "1";
                    dt.Attributes[Tags.UnitPrice].Value = StripCurrency(cmDeferredTerms.ToString(DecimalPlaces));
                    dt.Attributes[Tags.Value].Value = StripCurrency(cmDeferredTerms.ToString(DecimalPlaces));
                    cmDtTax = AccountManager.CalculateTaxAmount(Config.CountryCode, dt, chxTaxExempt.Checked, out Error);


                    if ((string)Country[CountryParameterNames.AgreementTaxType] == "I")
                        cmDtTax = cmAdminTax = cmInsuranceTax = 0;
                }
            }

            return (cmDeferredTerms + cmDtTax + cmInsuranceCharge + cmAdminCharge + cmInsuranceTax + cmAdminTax);
        }

        private bool CheckDepositWaiver()
        {
            bool found = false;

            if ((AccountType == AT.HP || AccountType == AT.ReadyFinance) && _scorecardtype == "B")
            {
                //Here Codedescript stores the Band..
                foreach (XmlNode item in itemDoc.DocumentElement.SelectNodes("//Item[@Quantity != '0']"))
                {
                    foreach (DataRow row in ((DataTable)StaticData.Tables[TN.DepositWaiver_Category]).Rows)
                    {
                        if (row[CN.Code].ToString().ToUpper() == item.SelectSingleNode("@Category").Value.ToString().ToUpper()
                            && row[CN.CodeDescription].ToString() == drpTermsType.SelectedValue.ToString())
                        {
                            found = true;
                        }
                    }

                    //foreach (DataRow row in ((DataTable)StaticData.Tables[TN.DepositWaiver_Level]).Rows)
                    //{
                    //    if (item.SelectSingleNode("@Code").Value.ToString().ToUpper().IndexOf(row[CN.Code].ToString().ToUpper()) == 0
                    //         && row[CN.CodeDescription].ToString() == drpTermsType.SelectedValue.ToString())
                    //    {
                    //        found = true;
                    //    }
                    //}

                    //IP - 28/07/11 - RI - #4415 - Check if items Class matches that setup in WDL code maintenance
                    foreach (DataRow row in ((DataTable)StaticData.Tables[TN.DepositWaiver_Level]).Rows)
                    {
                        if (item.SelectSingleNode("@Class").Value.ToString().ToUpper() == row[CN.Code].ToString().ToUpper()
                             && row[CN.CodeDescription].ToString() == drpTermsType.SelectedValue.ToString())
                        {
                            found = true;
                        }
                    }

                    //IP - 28/07/11 - RI - #4415 - Check if items SubClass matches that setup in WDS code maintenance
                    foreach (DataRow row in ((DataTable)StaticData.Tables[TN.DepositWaiver_SubClass]).Rows)
                    {
                        if (item.SelectSingleNode("@SubClass").Value.ToString().ToUpper() == row[CN.Code].ToString().ToUpper()
                             && row[CN.CodeDescription].ToString() == drpTermsType.SelectedValue.ToString())
                        {
                            found = true;
                        }
                    }

                    foreach (DataRow row in ((DataTable)StaticData.Tables[TN.DepositWaiver_Product]).Rows)
                    {
                        if (item.SelectSingleNode("@Code").Value.ToString().ToUpper() == row[CN.Code].ToString().ToUpper()
                             && row[CN.CodeDescription].ToString() == drpTermsType.SelectedValue.ToString())
                        {
                            found = true;
                        }
                    }
                }
            }

            lblDepositWaiver.Visible = found ? true : false;

            AutoDA = found;

            return found;
        }

        /// <summary>
        /// This function will call itself recursively until all 
        /// item nodes in the XML document have been entered into 
        /// the table.
        /// For each child node of the node passed in (which will
        /// always be an <item></item> node) create a row for that item
        /// and, if it's related items node has children, call this function
        /// again passing in that node.
        /// Also build up the tree view in the same format as the document
        /// </summary>
        /// <param name="relatedItems"></param>
        private void populateTable(XmlNode relatedItems, TreeNode tvNode, ref double sub, ref bool affinity)
        {
            Function = "populateTable";
            string itemType = "";
            double qty = 0;
            var express = "";      // #10337

            //outer loop iterates through <item> tags
            foreach (XmlNode item in relatedItems.ChildNodes)
            {
                bool showRow = true;
                if (item.NodeType == XmlNodeType.Element && item.Name == Elements.Item)
                {
                    var id = Convert.ToInt32(item.Attributes[Tags.ItemId].Value);
                    if (id != StockItemCache.Get(StockItemKeys.DT) &&
                        id != StockItemCache.Get(StockItemKeys.InsuranceChargeItem) &&
                        id != StockItemCache.Get(StockItemKeys.AdminChargeItem) &&
                        id != StockItemCache.Get(StockItemKeys.Tier2DiscountItemNumber) &&
                        id != StockItemCache.Get(StockItemKeys.STAX))
                    {
                        if (AccountType == AT.GoodsOnLoan)
                        {
                            if (item.Attributes[Tags.ExpectedReturnDate].Value == "")
                            {
                                ExpectedReturnDate er = new ExpectedReturnDate(item);
                                er.ShowDialog();
                            }
                        }

                        itemType = item.Attributes[Tags.Type].Value;
                        qty = Convert.ToDouble(item.Attributes[Tags.Quantity].Value);
                        express = Convert.ToString(item.Attributes[Tags.Express].Value);        //IP - 14/06/12 - #10378


                        if (itemType == IT.Kit)
                            xml.RecalculateKitPrice(item, false);

                        //TO DO need to display zero qty rows if they have a 
                        //+ve qty warranty attached.
                        if (itemType == IT.Kit ||
                            (qty <= 0 &&
                            item.SelectNodes("RelatedItems/Item[@Type='Warranty' and @Quantity != '0']").Count == 0 &&
                            item.SelectNodes("RelatedItems/Item[@Type='KitWarranty' and @Quantity != '0']").Count == 0))
                            showRow = false;

                        TreeNode tvChild = new TreeNode();
                        // uat363 add parentItemCode to Tag
                        int parentItemId = 0;
                        if (item.ParentNode.ParentNode.Name == "Item")
                            parentItemId = Convert.ToInt32(item.ParentNode.ParentNode.Attributes[Tags.ItemId].Value);
                        tvChild.Tag = item.Attributes[Tags.Key].Value + "|" + item.Attributes[Tags.ContractNumber].Value + "|" + parentItemId;
                        DataRow row = itemsTable.NewRow();
                        //tvChild.Text = itemType; 
                        tvChild.Text = item.Attributes[Tags.FreeGift].Value.ToUpper().Trim() == "TRUE" ? "FreeGift" : itemType; //IP - 21/04/10 - UAT(53) UAT5.2 - Merged from 4.3
                        tvChild.ImageIndex = 1;
                        tvChild.SelectedImageIndex = 1;
                        if (itemType == IT.Stock || itemType == IT.Component)
                        {
                            tvChild.ImageIndex = 0;
                            tvChild.SelectedImageIndex = 0;
                            // Caribbean 121 - allow changing of employee number on all accounts 
                            // unless there are stockitems ordered
                            stockExists = true;

                            if (express == "Y")         // #10337 set Express image
                            {
                                tvChild.ImageIndex = 10;
                                tvChild.SelectedImageIndex = 10;
                            }
                        }
                        //if (itemType == IT.Discount || itemType == IT.KitDiscount)
                        if (itemType == IT.Discount || itemType == IT.KitDiscount || item.Attributes[Tags.FreeGift].Value.ToUpper().Trim() == "TRUE") //IP - 21/04/10 - UAT(53) UAT5.2 - Merged from 4.3
                        {
                            tvChild.ImageIndex = 2;
                            tvChild.SelectedImageIndex = 2;
                        }
                        if (itemType == IT.Warranty || itemType == IT.KitWarranty)
                        {
                            tvChild.ImageIndex = 3;
                            tvChild.SelectedImageIndex = 3;
                        }
                        /*#if(DEBUG)
												logMessage(item.OuterXml, Credential.User, EventLogEntryType.Information);
						#endif*/
                        if ((bool)Country[CountryParameterNames.LoggingEnabled])
                            logMessage(item.OuterXml, Credential.User, STL.PL.WS4.EventLogEntryType.Information);

                        row["ProductCode"] = item.Attributes[Tags.Code].Value;
                        row["ItemID"] = item.Attributes[Tags.ItemId].Value;
                        row["ProductDescription"] = item.Attributes[Tags.Description1].Value;
                        row["StockLocation"] = item.Attributes[Tags.Location].Value;
                        row["DelAddress"] = item.Attributes[Tags.DeliveryAddress].Value; //IP - 09/02/10 - CR1049 Merged - Malaysia Enhancements (CR1072)
                        row["QuantityOrdered"] = item.Attributes[Tags.Quantity].Value;
                        row["UnitPrice"] = Convert.ToDecimal(item.Attributes[Tags.UnitPrice].Value).ToString(DecimalPlaces);
                        row[CN.ContractNo] = item.Attributes[Tags.ContractNumber].Value;
                        //row[CN.DeliveryArea] = item.Attributes[Tags.DeliveryArea].Value;
                        //row[CN.DeliveryProcess] = item.Attributes[Tags.DeliveryProcess].Value;
                        //uat363 rdb trying to build a unique key on items with same code by adding parent product code
                        row["ParentProductCode"] = item.ParentNode.ParentNode.Name == "Item" ? item.ParentNode.ParentNode.Attributes[Tags.Code].Value : "NA";
                        row["ParentItemID"] = item.ParentNode.ParentNode.Name == "Item" ? item.ParentNode.ParentNode.Attributes[Tags.ItemId].Value : "0";
                        //IP - 03/12/10 - StoreCard - Need to store the item type 
                        row["ItemType"] = item.Attributes[Tags.Type].Value;
                        row["RepoItem"] = item.Attributes[Tags.RepoItem].Value;            // RI jec 16/06/11
                        row["WarrantyType"] = item.Attributes[Tags.WarrantyType].Value;    //#17883  // #16607
                        row["DateDelivered"] = item.Attributes[Tags.DateDelivered].Value;   //#16607
                        row["ProductCategory"] = item.Attributes[Tags.ProductCategory].Value; //#16607

                        if (itemType == IT.Affinity && qty > 0)
                            affinity = true;

                        if (showRow)
                        {
                            row["Value"] = Convert.ToDecimal(item.Attributes[Tags.Value].Value).ToString(DecimalPlaces);
                            /* make sure that the sub total doesn't inlcude the price of
							 * any warranties being bought on credit for cash and go accounts */
                            if (!((item.Attributes[Tags.Type].Value == IT.Warranty ||
                                 item.Attributes[Tags.Type].Value == IT.KitWarranty) &&
                                 PaidAndTaken &&
                                 WarrantiesOnCredit || item.Attributes[Tags.Code].Value == LoyaltyDropStatic.VoucherCode))
                            {
                                sub += Convert.ToDouble(StripCurrency(item.Attributes[Tags.Value].Value));
                            }

                            //WarrantiesOnCreditAgreementTotal += Convert.ToDecimal(StripCurrency(item.Attributes[Tags.Value].Value));
                            // 5.1 uat329 rdb 18/12/07 add tax
                            decimal itemValue = Convert.ToDecimal(StripCurrency(item.Attributes[Tags.Value].Value));
                            // 5.1 uat329 jec 04/01/08 add tax only if excluded
                            decimal itemTaxValue = 0;
                            if ((string)Country[CountryParameterNames.AgreementTaxType] == "E")
                            {
                                itemTaxValue = itemValue * 0.01m * Convert.ToDecimal(item.Attributes[Tags.TaxRate].Value);
                            }

                            WarrantiesOnCreditAgreementTotal += itemValue + itemTaxValue;

                            if (item.Attributes[Tags.Type].Value != IT.Stock && item.Attributes[Tags.Type].Value != IT.Component &&
                                item.Attributes[Tags.Type].Value != IT.Discount && item.Attributes[Tags.Type].Value != IT.KitDiscount &&
                                item.Attributes[Tags.Type].Value != IT.SundryCharge && item.Attributes[Tags.Type].Value != IT.Kit &&
                                item.Attributes[Tags.Type].Value != "DT" && AccountType == AT.ReadyFinance)
                            {
                                nonStockTotal += Convert.ToDecimal(StripCurrency(item.Attributes[Tags.Value].Value));

                                // CR906 do not add the value of Loan items when
                                // we are calculating max spend so keep a running total
                                if (item.Attributes["Code"].Value == "LOAN")
                                    loanTotal += Convert.ToDecimal(StripCurrency(item.Attributes[Tags.Value].Value));
                            }
                            // Tier2 Loyalty Club discounts are calculated on stock items less discounts
                            if (item.Attributes[Tags.Type].Value == IT.Stock ||
                                item.Attributes[Tags.Type].Value == IT.Discount ||
                                item.Attributes[Tags.Type].Value == IT.KitDiscount)

                                tier2SubTotal += Convert.ToDecimal(StripCurrency(item.Attributes[Tags.Value].Value));
                        }
                        try
                        {
                            if (showRow)
                                itemsTable.Rows.Add(row);

                            foreach (XmlNode child in item.ChildNodes)
                                if (child.NodeType == XmlNodeType.Element && child.Name == Elements.RelatedItem)
                                {
                                    if (child.HasChildNodes)
                                        populateTable(child, tvChild, ref sub, ref affinity);
                                }
                            if (qty > 0 ||
                                item.SelectNodes("RelatedItems/Item[@Type='Warranty' and @Quantity != '0']").Count > 0 ||
                                item.SelectNodes("RelatedItems/Item[@Type='KitWarranty' and @Quantity != '0']").Count > 0)
                                tvNode.Nodes.Add(tvChild);
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Attempting to Add new item when already exists. Please edit existing item in list");
                        }

                        // itemsTable.Rows.Find(row)

                    }
                }
            }
        }

        private void RecalculateChargeableTotals(XmlNode relatedItems)
        {
            foreach (XmlNode item in relatedItems)
            {
                bool showRow = true;

                var id = Convert.ToInt32(item.Attributes[Tags.ItemId].Value);
                if (id != StockItemCache.Get(StockItemKeys.DT) &&
                    id != StockItemCache.Get(StockItemKeys.InsuranceChargeItem) &&
                    id != StockItemCache.Get(StockItemKeys.AdminChargeItem) &&
                    id != StockItemCache.Get(StockItemKeys.STAX))
                {
                    decimal qty = Convert.ToDecimal(item.Attributes[Tags.Quantity].Value);
                    string itemType = item.Attributes[Tags.Type].Value;
                    if (itemType == IT.Kit)
                        xml.RecalculateKitPrice(item, false);

                    if (itemType == IT.Kit || qty <= 0)
                        showRow = false;

                    if (showRow)
                    {
                        if (!((item.Attributes[Tags.Type].Value == IT.Warranty ||
                            item.Attributes[Tags.Type].Value == IT.KitWarranty) &&
                            PaidAndTaken &&
                            WarrantiesOnCredit))
                        {
                            if (item.Attributes[Tags.Code].Value.Length > 2 &&
                                item.Attributes[Tags.Code].Value.Substring(0, 2) != (string)Country[CountryParameterNames.NonInterestItem] &&
                                Convert.ToInt32(item.Attributes[Tags.ItemId].Value) != StockItemCache.Get(StockItemKeys.InsuranceChargeItem) &&
                                Convert.ToInt32(item.Attributes[Tags.ItemId].Value) != StockItemCache.Get(StockItemKeys.AdminChargeItem))
                            {
                                IncludeWarranty = true;
                                if (drpTermsType.SelectedIndex != 0)
                                    IncludeWarranty = Convert.ToBoolean(((DataRowView)drpTermsType.SelectedItem)[CN.IncludeWarranty]);

                                if (!(item.Attributes[Tags.Type].Value == IT.Warranty ||
                                    item.Attributes[Tags.Type].Value == IT.KitWarranty) ||
                                    IncludeWarranty)
                                {
                                    ChargeableAdminSubTotal += Convert.ToDecimal(StripCurrency(item.Attributes[Tags.Value].Value));
                                }
                                ChargeableSubTotal += Convert.ToDecimal(StripCurrency(item.Attributes[Tags.Value].Value));
                            }
                        }
                    }

                    foreach (XmlNode child in item.ChildNodes)
                        if (child.NodeType == XmlNodeType.Element && child.Name == Elements.RelatedItem)
                        {
                            if (child.HasChildNodes)
                                RecalculateChargeableTotals(child);
                        }
                }
            }
        }

        /// <summary>
        /// This will launch the "enquiry by stock" screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItem15_Click(object sender, System.EventArgs e)
        {
            CodeStockEnquiry next = new CodeStockEnquiry();
            next.FormParent = this;
            next.FormRoot = this.FormRoot;
            txtQuantity.Select(0, txtQuantity.Text.Length);
            txtQuantity.Focus();
            ((MainForm)this.FormRoot).AddTabPage(next);
        }

        private bool CheckSchedules(string accountNo, XmlNode item, double newQty)
        {
            if (PaidAndTaken)       //no need to check this for paid and taken
                return true;

            bool status = true;
            double delivered = 0;
            double scheduled = 0;
            bool repo = false;          //IP - 26/06/12 - #10516
            bool onPickList = false;
            bool delNotePrinted = false;
            bool onLoad = false;
            ScheduledQtyDeleted = 0.0; //IP - 19/11/09 - UAT5.2 (591) - Re-set to 0 as previously was set to the last schedule overide quantity to delete.

            string itemNo = item.Attributes[Tags.Code].Value;
            int itemID = Convert.ToInt32(item.Attributes[Tags.ItemId].Value);                   //IP/NM - 18/05/11 -CR1212 - #3627 

            int parentItemID = 0;
            if (item.Attributes[Tags.ParentItemId] != null)
                parentItemID = Convert.ToInt32(item.Attributes[Tags.ParentItemId].Value);       //IP - 23/06/11 - CR1212 - RI - #4067
            short location = Convert.ToInt16(item.Attributes[Tags.Location].Value);
            double ratio = newQty / Convert.ToDouble(item.Attributes[Tags.Quantity].Value);
            string itemType = item.Attributes[Tags.Type].Value;
            string contractNo = item.Attributes[Tags.ContractNumber].Value;
            string delProcess = item.Attributes[Tags.DeliveryProcess].Value;

            //if it's a warranty / discount don't bother checking delivery table
            /* also don't need to check if it's a sundry charge item */
            if (itemType != IT.Warranty && itemType != IT.KitWarranty
                && itemType != IT.Discount && itemType != IT.KitDiscount
                && itemType != IT.SundryCharge)
            {
                AccountManager.GetItemsDeliveredAndScheduled(accountNo, this.AgreementNo,
                    itemID,                                                                   //IP/NM - 18/05/11 -CR1212 - #3627 
                    location,
                    contractNo,
                    parentItemID,           //IP
                    out delivered,
                    out scheduled,
                    out repo,               //IP - 26/06/12 - #10516
                    out Error);
                if (Error.Length > 0)
                {
                    ShowError(Error);
                    status = false;
                }
                else
                {
                    //if (delivered > newQty)
                    if (delivered > newQty || (repo))  //IP - 26/06/12 - #10516
                    {
                        ShowInfo("M_PARTIALLYDELIVERED", new object[] { itemNo });
                        status = false;
                    }
                    else
                    {
                        //sheduled deliveries can be overridden with the right permission
                        if (scheduled > (newQty - delivered))
                        {
                            AccountManager.GetScheduledDelNote(accountNo,
                                this.AgreementNo, itemID, location,                         //IP/NM - 18/05/11 -CR1212 - #3627 
                                out onPickList, out delNotePrinted, out onLoad,
                                out Error);

                            //delivery note has been scheduled for delivery through
                            //transport scheduling
                            if (delNotePrinted)
                            {
                                ShowInfo("M_ITEMSCHEDULED");
                                status = false;
                            }
                            if (status)
                            {
                                if (onLoad)
                                {
                                    if (!lreviseScheduled.Enabled)
                                    {
                                        ShowInfo("M_REVISESCHEDULED");
                                        status = false;
                                    }
                                }
                                else
                                {
                                    if (!lreviseNonScheduled.Enabled)
                                    {
                                        ShowInfo("M_REVISEAWAITING");
                                        status = false;
                                    }
                                }
                            }
                            if (status)
                            {
                                // 5.1 uat74 rdb 8/11/07
                                bool resetHoldProp = !(delivered > 0 && (newQty - delivered) == 0);

                                ScheduleOverride sched = new ScheduleOverride(true, accountNo, this.AgreementNo, itemID,    //IP/NM - 18/05/11 -CR1212 - #3627 
                                    Convert.ToInt16(location),
                                    scheduled - (newQty - delivered), FormRoot, this, resetHoldProp);

                                sched.ShowDialog();
                                if (this.ScheduledQtyDeleted < scheduled - (newQty - delivered))
                                    status = false;
                            }
                        }
                    }
                }

                if (status)
                {
                    //check if the item has related items and recurse
                    XmlNode related = item.SelectSingleNode(Elements.RelatedItem);
                    if (related != null)
                    {
                        foreach (XmlNode child in related.ChildNodes)
                        {
                            /* JJ - must make sure that status is never set from false to true
							 * this has caused kits to be removed despite being delivered in the past. */
                            if (child.NodeType == XmlNodeType.Element && child.Name == Elements.Item)
                            {
                                bool stat = CheckSchedules(accountNo, child, (Convert.ToDouble(child.Attributes[Tags.Quantity].Value) * ratio));
                                status = status == false ? false : stat;
                            }
                        }
                    }
                }
            }
            return status;
        }

        public bool CheckSchedules(string accountNo, string itemNo, int itemID, short location, string contractNo, double newQty, string itemType, int parentItemID) //IP - 23/06/11 - RI
        {
            double delivered = 0;
            double scheduled = 0;
            bool repo = false;              //IP - 26/06/12 - #10516
            bool onPickList = false;
            bool delNotePrinted = false;
            bool onLoad = false;
            bool status = true;

            //if it's a warranty don't bother checking delivery table
            if (itemType != IT.Warranty && itemType != IT.KitWarranty)
            {
                AccountManager.GetItemsDeliveredAndScheduled(accountNo, this.AgreementNo,
                    itemID,                                                                         //IP/NM - 18/05/11 -CR1212 - #3627 
                    Convert.ToInt16(location),
                    contractNo,
                    parentItemID,                   //IP
                    out delivered,
                    out scheduled,
                    out repo,                      //IP - 26/06/12 - #10516
                    out Error);

                if (Error.Length > 0)
                {
                    ShowError(Error);
                    status = false;
                }
                else
                {
                    if (delivered > newQty)
                    {
                        ShowInfo("M_PARTIALLYDELIVERED", new object[] { itemNo });
                        status = false;
                    }
                    else
                    {
                        //sheduled deliveries can be overridden with the right permission
                        if (scheduled > (newQty - delivered))
                        {
                            AccountManager.GetScheduledDelNote(accountNo,
                                this.AgreementNo, itemID, location,                         //IP/NM - 18/05/11 -CR1212 - #3627 
                                out onPickList, out delNotePrinted, out onLoad,
                                out Error);

                            //delivery note has been scheduled for delivery through
                            //transport scheduling
                            if (delNotePrinted)
                            {
                                ShowInfo("M_ITEMSCHEDULED");
                                status = false;
                            }
                            if (status)
                            {
                                if (onLoad)
                                {
                                    if (!lreviseScheduled.Enabled)
                                    {
                                        ShowInfo("M_REVISESCHEDULED");
                                        status = false;
                                    }
                                }
                                else
                                {
                                    if (!lreviseNonScheduled.Enabled)
                                    {
                                        ShowInfo("M_REVISEAWAITING");
                                        status = false;
                                    }
                                }
                            }
                            if (status)
                            {
                                // 5.1 uat74 rdb 8/11/07
                                bool resetHoldProp = !(delivered > 0 && (newQty - delivered) == 0);
                                ScheduleOverride sched = new ScheduleOverride(true, accountNo, this.AgreementNo, itemID,        //IP/NM - 18/05/11 -CR1212 - #3627 
                                    Convert.ToInt16(location),
                                    scheduled - (newQty - delivered), FormRoot, this, resetHoldProp);
                                sched.ShowDialog();
                                if (this.ScheduledQtyDeleted < scheduled - (newQty - delivered))
                                    status = false;
                            }
                        }
                    }
                }
            }
            return status;
        }

        /// <summary>
        /// This will remove items from the account by removing them
        /// from the underlying xml document.
        /// If the item is a kit component then we must delete the 
        /// whole kit.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRemove_Click(object sender, System.EventArgs e)
        {
            Function = "btnRemove_Click";
            this._hasdatachanged = true;
            //10.6 CR- Sales Order - Print Save- Allow user to save while printing 
            //and track UI changes to increment invoice version
            _isSalesOrderChanged = true;
            string key;
            string product = "";
            string itemID = string.Empty;                 //IP - 19/05/11 - CR1212 - RI - #3665
            string location = "";
            int index = 0;
            string itemType = "";
            string accountNo = AccountNo;
            bool status = true;
            XmlNode toDelete = null;
            int itemId = 0;
            string contract = "";
            string parentProductCode;
            decimal itemDelQty = 0;
            int curPayMethod = Convert.ToInt16(((DataRowView)drpPayMethod.SelectedItem)[CN.Code].ToString()); //IP - 03/12/10 - Store Card
            XmlNode KitWarranty = null;
            try
            {
                Wait();

                if (this.KitClicked)
                {
                    key = (string)tvItems.SelectedNode.Tag;
                    string[] k = key.Split('|');
                    product = k[0];
                    location = k[1];
                    contract = k[2];
                    parentProductCode = k[3];
                    key = product + "|" + location;
                }
                else
                {
                    DataView dv = (DataView)dgLineItems.DataSource;
                    index = dgLineItems.CurrentRowIndex;
                    product = (string)dv[index]["ProductCode"];
                    itemID = Convert.ToString(dv[index][CN.ItemId]);         //IP - 19/05/11 - CR1212 - RI - #3665
                    location = (string)dv[index]["StockLocation"];
                    contract = (string)dv[index][CN.ContractNo];
                    parentProductCode = dv[index]["ParentProductCode"].ToString();
                    //key = product + "|" + location;   
                    key = itemID + "|" + location;                          //IP - 19/05/11 - CR1212 - RI - #3665
                }

                string xPath = string.Empty;
                XmlNode found;

                //IP - 12/06/12 - #10328 - change no longer required      //IP - 29/05/12 - #9877 - Warehouse & Deliveries Integration
                //if (LineItemBooking != null)
                //{
                //    foreach (DataRow dr in LineItemBooking.Rows)
                //    {
                //        if (Convert.ToInt32(dr["ItemId"]) == Convert.ToInt32(itemID))
                //        {
                //            ShowInfo("M_CANNOTREMOVEITEM", MessageBoxButtons.OK);
                //            return;
                //        }
                //    }
                //}

                if (KitClicked)
                {
                    xPath = "//Item[@Key = '" + key + "' and @ContractNumber = '" + contract + "']";
                    found = itemDoc.DocumentElement.SelectSingleNode(xPath);

                    //KitWarranty = found.SelectSingleNode("//Item[@Type = 'Warranty' or @Type = 'KitWarranty']");
                    KitWarranty = found.SelectSingleNode("//Item[(@Type = 'Warranty' or @Type = 'KitWarranty') and @Quantity !='0']");
                    if (KitWarranty != null)
                    {
                        if (MessageBox.Show("This kit item has a warranty attached. Do you wish to remove?", "Remove Kit Warranty", MessageBoxButtons.OKCancel) == DialogResult.OK)
                            found.SelectSingleNode("RelatedItems").RemoveChild(KitWarranty);
                    }
                }

                xPath = "//Item[@Key = '" + key + "' and @ContractNumber = '" + contract + "' and @Quantity != 0";

                if (parentProductCode != string.Empty &&
                    parentProductCode != "NA" &&
                    parentProductCode != "0")                                         //IP - 02/06/11 - #3787
                    xPath += " and ../../@Code = '" + parentProductCode + "' ";
                xPath += "]";

                found = itemDoc.DocumentElement.SelectSingleNode(xPath);

                if (found != null)
                {
                    itemId = Convert.ToInt32(found.Attributes[Tags.ItemId].Value);
                    itemType = found.Attributes[Tags.Type].Value;
                    itemDelQty = System.Convert.ToDecimal(found.Attributes[Tags.DeliveredQuantity].Value);

                    #region Revising account code
                    //First, if we're revising check for deliveries. If there are any
                    //then we can't delete
                    if (revision)
                    {
                        //IP - 25/02/10 - CR1072 - Malaysia 3PL for Version 5.2
                        if (thirdPartyDeliveriesEnabled)
                        {
                            //IP/JC - 01/03/10 - CR1072 - Malaysia 3PL 
                            if (found.Attributes["VanNo"] != null && found.Attributes["VanNo"].Value == "DHL"
                                && found.Attributes["DhlDNNo"].Value == "")
                            {
                                MessageBox.Show("Item has been exported to DHL.");


                                // CR953 Can't remove an item if it is in transit with DHL

                                //if (found.Attributes["DhlDNNo"] != null && found.Attributes["DhlDNNo"].Value != String.Empty)
                                //{
                                // UAT 38 Authorisation required to remove an item
                                //AuthorisePrompt auth = new AuthorisePrompt(this, btnRemove, "Item is in transit with DHL. Authorisation is required to remove this item.");
                                //auth.ShowDialog();

                                //if (auth.Authorised)
                                //{
                                //    status = true;
                                //}
                                //else
                                //{
                                //    //MessageBox.Show("Item in transit with DHL - it cannot be removed");
                                //    status = false;
                                //}
                                //}
                            }

                            if (found.Attributes["VanNo"] != null && found.Attributes["VanNo"].Value == "DHL"
                            && found.Attributes["DhlDNNo"].Value != "")
                            // && found.Attributes["ShipQty"].Value !="0")
                            {
                                MessageBox.Show("In transit - unable to remove item.");
                                status = false;
                            }

                        }

                        //The xml ScheduledQuantity may not be correct if the user has revised an account prior to DA
                        //Therefore need to re-check the Scheduled Quantity on the server.
                        var isItemScheduled = false;

                        if (found.Attributes["LineItemId"] != null)
                        {
                            isItemScheduled = AccountManager.IsItemScheduled(Convert.ToInt32(found.Attributes["LineItemId"].Value), out Error);
                        }

                        // #14603
                        if ((found.Attributes["ScheduledQuantity"] != null && found.Attributes["ScheduledQuantity"].Value != "0") || isItemScheduled)
                        {
                            MessageBox.Show("This item has been sent to Warehouse and can not be changed");
                            status = false;
                        }

                        /* make sure we cannot remove balances from previous accounts (i.e. add to items */
                        if (itemId == StockItemCache.Get(StockItemKeys.ADDDR) ||
                            itemId == StockItemCache.Get(StockItemKeys.ADDCR))
                        {
                            ShowWarning(GetResource("M_CANNOTREMOVEADDTO"));
                            status = false;
                        }

                        // Cannot remove a warranty until it is collected
                        if (!Collection
                            && (itemType == IT.Warranty || itemType == IT.KitWarranty)
                            && itemDelQty > 0)
                        {
                            ShowWarning(GetResource("M_CANNOTREMOVEWARRANTY"));
                            status = false;
                        }

                        if (status)
                        {
                            //if its part of a kit then check the schedules for every component
                            //of the kit
                            if (itemType == IT.KitDiscount ||
                                itemType == IT.KitWarranty ||
                                itemType == IT.Component)
                            {
                                XmlNode kitNode = found.ParentNode.ParentNode;
                                foreach (XmlNode child in kitNode.ChildNodes)
                                {
                                    if (child.NodeType == XmlNodeType.Element && child.Name == Tags.Item)
                                    {
                                        if (!CheckSchedules(accountNo, child, 0))
                                        {
                                            status = false;
                                            break;
                                        }
                                    }
                                }
                            }
                            status = CheckSchedules(accountNo, found, 0);
                        }
                    }
                    #endregion

                    if (status)
                    {
                        /* if the item to be deleted has
						 * child items which are warranties, ask the user
						 * if they want to delete the warranties too */

                        // 5.1 uat145 rdb 5/12/07 ref 69282
                        // suppress dialog asking if you want to delete attached warranty IF
                        // warranty already delivered.  Do not delete warranty in this case
                        //bool deleteWarranties = true;
                        bool deleteWarranties = false;

                        //--uat(5.2) - 907, 4.3 merge ----------------------
                        string xpathRelatedItem = "", xpathItem = "";
                        if (CashAndGoReturn) //cash and go return
                        {
                            xpathRelatedItem = "RelatedItems/Item[(@Type='Warranty' or @Type='KitWarranty') and @Quantity != 0 ]";
                            xpathItem = "//Item[(@Type='Warranty' or @Type='KitWarranty') and @Quantity != 0 ]";
                        }
                        else
                        {
                            xpathRelatedItem = "RelatedItems/Item[(@Type='Warranty' or @Type='KitWarranty') and @Quantity != 0 and @DeliveredQuantity = 0 ]";
                            xpathItem = "//Item[(@Type='Warranty' or @Type='KitWarranty') and @Quantity != 0 and @DeliveredQuantity = 0 ]";
                        }
                        //-------------------------------------------------

                        //if (found.SelectNodes("RelatedItems/Item[(@Type='Warranty' or @Type='KitWarranty') and @Quantity != 0 ]").Count > 0)
                        if (found.SelectNodes(xpathRelatedItem).Count > 0 && found.ParentNode.Name != "RelatedItems") //SC 72098 03/02/2010
                        {
                            deleteWarranties = DialogResult.Yes == ShowInfo("M_DELETEWARRANTIES", MessageBoxButtons.YesNo);

                            //IP - 02/01/2008
                            //If a warranty is to be removed set the boolean warrantySelected to false
                            if (deleteWarranties)
                            {
                                warrantySelected = false;
                            }
                        }

                        //IP - 02/01/2008 - if the warranty itself is selected to be removed from the grid
                        else if (found.SelectNodes(xpathItem).Count > 0 && found.ParentNode.Name == "RelatedItems") //SC 72098 03/02/2010
                        {
                            deleteWarranties = true;
                            warrantySelected = true;
                        }

                        //Code possibly needed for livewire 69312
                        //if (found.SelectNodes("RelatedItems/Item[(@Type='Discount') and @Quantity != 0 and @DeliveredQuantity = 0 ]").Count > 0)
                        //{
                        //   deleteWarranties = true;
                        //}

                        //IP - 09/06/11 - CR1212 - RI - #3817 - If this is a discount we need to check if its an account wide discount.
                        if (Convert.ToString(found.Attributes[Tags.Type].Value) == "Discount")
                        {
                            accountWideDiscount = isAcctWideDiscount(Convert.ToString(found.Attributes[Tags.ProductCategory].Value));
                        }

                        if (accountWideDiscount) //If this is an account wide discount then we must remove all occurrences 
                        {
                            var disID = Convert.ToString(found.Attributes[Tags.ItemId].Value);

                            foreach (XmlNode item in itemDoc.DocumentElement)
                            {

                                if (item.Attributes[Tags.Type].Value == "Stock" || item.Attributes[Tags.Type].Value == "Kit")
                                {
                                    foreach (XmlNode child in item.ChildNodes)
                                    {
                                        if (child.NodeType == XmlNodeType.Element && child.HasChildNodes)
                                        {
                                            foreach (XmlNode childNodes in child.ChildNodes)
                                            {
                                                if (Convert.ToString(childNodes.Attributes[Tags.ItemId].Value) == disID)
                                                {
                                                    toDelete = childNodes;
                                                    xml.deleteItem(toDelete, deleteWarranties, itemDoc.DocumentElement);
                                                }

                                                if (childNodes.NodeType == XmlNodeType.Element && childNodes.HasChildNodes) //Delete Account Wide Discount attached to any kit components
                                                {
                                                    if (childNodes.Attributes[Tags.Type].Value == "Component")
                                                    {
                                                        foreach (XmlNode kitComponent in childNodes.ChildNodes)
                                                        {
                                                            if (kitComponent.NodeType == XmlNodeType.Element && kitComponent.HasChildNodes)
                                                            {
                                                                foreach (XmlNode kitChild in kitComponent.ChildNodes)
                                                                {
                                                                    if (Convert.ToString(kitChild.Attributes[Tags.ItemId].Value) == disID)
                                                                    {
                                                                        toDelete = kitChild;
                                                                        xml.deleteItem(toDelete, deleteWarranties, itemDoc.DocumentElement);
                                                                    }
                                                                }
                                                            }
                                                        }

                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                            }

                        }
                        else
                        {

                            toDelete = found;

                            if (toDelete != null)
                            {
                                ////69282 rdb if a replacement item was added then removed
                                ////we need to set Exchanged flag back to false
                                //Exchanged = false;

                                xml.deleteItem(toDelete, deleteWarranties, itemDoc.DocumentElement);
                            }
                        }

                        //If a warranty is to be removed set the boolean warrantySelected to false
                        if (deleteWarranties)
                        {
                            //IP - 15/04/08 - (69630) - Set 'warrantySelected' to false when there are no more warranties on the sale.
                            XmlNodeList warrantiesNodes = itemDoc.DocumentElement.SelectNodes("//Item[@ContractNumber != '' and @Quantity != 0 ]");
                            if (warrantiesNodes.Count == 0)
                            {
                                warrantySelected = false;
                            }
                        }
                        //IP - 15/04/08 - (69630)
                        //Added extra change as part of the fix for the above livewire issue.
                        //Once a warranty has been removed and if there are no warranties 
                        //remaining on the sale then disable and hide the 'Customer Search' button
                        //preventing clicking on the button.
                        if (status)
                        {
                            //IP - 23/05/08 - UAT(415) - Once a warranty has been removed then need to remove
                            //from the xml as previously adding, removing warranties multiple
                            //times would cause multiple warranty contracts to be printed.

                            if (deleteWarranties) //IP - 01/02/10 - UAT(974) 
                            {
                                if (warrantySelected)
                                {
                                    XmlNode parentnode = found.ParentNode;
                                    parentnode.RemoveChild(found);
                                }
                                else
                                {
                                    //if not warranty selected then don't need to find parent
                                    XmlNode parent = found;
                                    string parentitem = parent.Attributes == null ? String.Empty : Convert.ToString(parent.Attributes[Tags.Key].Value);
                                    string xPathWarr = "//Item[@Type = 'Warranty']";
                                    foreach (XmlNode remWarr in itemDoc.SelectNodes(xPathWarr))
                                    {
                                        if (remWarr.ParentNode.ParentNode.Attributes != null && remWarr.ParentNode.ParentNode.Attributes[Tags.Key] != null && remWarr.ParentNode.ParentNode.Attributes[Tags.Key].Value == parentitem)
                                        {
                                            parent.SelectSingleNode("RelatedItems").RemoveChild(remWarr);
                                        }
                                    }
                                }
                            }

                            XmlNodeList warrantyNodes = itemDoc.DocumentElement.SelectNodes("//Item[@ContractNumber != '' and @Quantity != 0 ]");
                            if (PaidAndTaken && ((warrantyNodes.Count == 0) && (bool)Country[CountryParameterNames.WarrantyCustomerDetails]))
                            {
                                btnCustomerSearch.Visible = false;
                                btnCustomerSearch.Enabled = false;
                                menuCustomerSearch.Visible = false;
                                menuCustomerSearch.Enabled = false;

                                btnPrintReceipt.Enabled = true;
                                menuPrintReceipt.Enabled = true;

                                if (this.CustomerID == String.Empty && this.custidCG == string.Empty) //IP - Only set back to Paid and Taken if not linked to a Customer  //IP - 23/12/10 - Bug #2751
                                    this.CustomerID = ptCustomerID; //IP - 15/04/08 - (69630) Set CustomerID back to "PAID & TAKEN".
                                SetCustomerDetails();
                            }
                        }
                    }

                    //IP - 14/01/11 - Once an item has been removed need to re-set the paymentlist
                    if (PaidAndTaken && !CashAndGoReturn)
                    {
                        InitPaymentSet();
                    }

                    populateTable();

                    //#16607
                    if (CashAndGoReturn)
                    {
                        ProcessWarrantyRefund();
                    }

                }

                if (Convert.ToBoolean(Country[CountryParameterNames.LoyaltyScheme]))
                {
                    CheckForVoucher(false, product);
                    CheckForFreeDelivery();
                }

                //IP - 03/12/10 - Store Card - If there were items that exceeded the maximum item value 
                //we need to check if these are now removed.
                if (PaidAndTaken && PayMethod.IsPayMethod(curPayMethod, PayMethod.StoreCard) && itemValExceeded)
                {
                    CheckMaxItemVal();
                }

                ClearItemDetails();
                txtProductCode.Text = "";
                accountWideDiscount = false;   //#14051
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
            }
        }

        private bool AddKit(int kitItemId, short location, string accountType, double kitQty)
        {
            Function = "BAccountManager::AddKitToAccount()";
            bool status = true;
            this._hasdatachanged = true;

            XmlNode root = itemDoc.DocumentElement;
            XmlNode comps = AccountManager.AddKitToAccount(ref root,
                currentItem,
                kitItemId,
                location,
                accountType,
                kitQty,
                Config.CountryCode,
                chxDutyFree.Checked,
                chxTaxExempt.Checked,
                Convert.ToInt16(Config.BranchCode),
                out Error);

            if (Error.Length > 0 || !comps.HasChildNodes)
            {
                if (Error.Length > 0)
                    ShowError(Error);
                else
                    ShowInfo("M_KITFAIL");
                status = false;
            }
            else
            {
                itemDoc.LoadXml(root.OuterXml);

                comps = itemDoc.ImportNode(comps, true);

                foreach (XmlNode child in currentItem.ChildNodes)
                    if (child.NodeType == XmlNodeType.Element && child.Name == Elements.RelatedItem)
                        currentItem.ReplaceChild(comps, child);

                itemDoc.DocumentElement.AppendChild(currentItem);

            }


            return status;
        }



        //private bool CheckWarranty(XmlNode parent, XmlNode warranty, short location, StringCollection branches)
        //{
        //    bool status = false;
        //    string warrantyKey = warranty.Attributes[Tags.Key].Value;
        //    string parentKey = parent.Attributes[Tags.Key].Value;

        //    //is it already connected to this item?
        //    XmlNode found = xml.findItem(parent.SelectSingleNode(Elements.RelatedItem), warrantyKey, parent.Attributes[Tags.Code].Value);
        //    if (found != null)
        //    {
        //        xml.replaceItem(found, warranty);
        //        status = true;
        //    }
        //    else
        //    {
        //        //does it exist elsewhere in the document?
        //        found = xml.findItem(itemDoc.DocumentElement, warrantyKey);
        //        if (found != null)
        //        {
        //            foreach (string s in branches)
        //            {
        //                if (Convert.ToInt16(s) > location)
        //                {
        //                    warrantyKey = warranty.Attributes[Tags.ItemId].Value + "|" + s;
        //                    warranty.Attributes[Tags.Key].Value = warrantyKey;
        //                    warranty.Attributes[Tags.Location].Value = s;
        //                    status = CheckWarranty(parent, warranty, Convert.ToInt16(s), branches);
        //                    break;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            XmlNode related = parent.SelectSingleNode(Elements.RelatedItem);
        //            if (related != null)
        //            {
        //                related.AppendChild(warranty);
        //                status = true;
        //            }
        //        }
        //    }
        //    return status;
        //}

        /// <summary>
        /// This function is used to add a warranty for a product and
        /// can be called either while adding a line item or
        /// manually afterwards.
        /// </summary>
        private bool AddWarranty(int itemId, string productCode, short location, double unitPrice, double quantity)
        {
            bool status = true;
            decimal price = 0;
            string key = itemId + "|" + location.ToString();
            this._hasdatachanged = true;
            bool isIR = false;

            /* warranties cannot be added for collections
			 * nor replacements in period 1 or 2 */
            if (Collection)
            //||
            //(Replacement &&
            //(instantReplacement.TimePeriod == OneForOneTimePeriod.IRPeriod1 ||
            //instantReplacement.TimePeriod == OneForOneTimePeriod.IRPeriod2)))
            {
                status = false;
            }

            /* warranties cannot be added to GoodsOnLoan accounts */
            if (AccountType == AT.GoodsOnLoan)
                status = false;

            //if (status)
            //{
            //    Function = "BACcountManager::GetExchangeDetails()";
            //    AttachWarranty(AccountNo, AgreementNo, key,true);
            //}

            if (status)
            {
                //#17290
                XmlNode parentItem = xml.findItem(itemDoc.DocumentElement, key);
                var refCode = parentItem.Attributes[Tags.RefCode].Value;

                var warrantyType = refCode == "ZZ" ? WarrantyType.InstantReplacement : WarrantyType.Extended;

                warranties = Services.GetService(STL.Common.Services.Services.ServiceTypes.CosacsWeb).GetWarranties(productCode, Convert.ToString(location), warrantyType);
                if (warranties.Items == null)
                    warranties.Items = new List<WarrantyResult.Item>();

                var taxType = (string)Country[CountryParameterNames.TaxType];
                //var agreementTaxType = (string)Country[CountryParameterNames.AgreementTaxType];
                var taxRate = Convert.ToDecimal(Country[CountryParameterNames.TaxRate]);
                //NewAccount.ApplyTaxInTaxInclusiveCountries(warranties, taxType, agreementTaxType, taxRate);

                if (warranties.Items != null)
                {
                    //hack to make warranty prices work without changing the Web
                    foreach (WarrantyResult.Item warrantyItem in warranties.Items)
                    {
                        if (chxTaxExempt.Checked)
                        {
                            //warrantyItem.price.RetailPrice = Convert.ToDecimal(warrantyItem.price.RetailPrice.ToString(DecimalPlaces));
                            warrantyItem.price.RetailPrice = TrimCurrency(warrantyItem.price.RetailPrice.ToString(DecimalPlaces));    //handle  decimalplaces='C2'  #18619 
                        }
                        else
                        {
                            //warrantyItem.price.RetailPrice = Convert.ToDecimal(warrantyItem.price.TaxInclusivePriceChange.ToString(DecimalPlaces));
                            warrantyItem.price.RetailPrice = TrimCurrency(warrantyItem.price.TaxInclusivePriceChange.ToString(DecimalPlaces));       //handle  decimalplaces='C2'  #18619 
                        }
                    }
                }

                status = (allowInstantReplacement.Enabled || allowSupaShield.Enabled || !PaidAndTaken) && warranties.Items.Count > 0;     // #16208

                if (status)     /* this may appear quite elaborate but I couldn't do it the easy
									 * way because you can't put wild cards in the middle of
									 * strings in a rowfilter e.g. 19%2 - JJ */
                {

                    foreach (var r in warranties.Items)
                    {
                        AccountManager.IsItemInstantReplacement(r.warrantyLink.Id, location, out isIR, out Error);
                        if (Error.Length > 0)
                            ShowError(Error);

                        if (!PaidAndTaken)
                        {
                            if (isIR)
                            {
                                if ((AccountType == AT.Cash || AccountType == AT.ReadyFinance)
                                    && (bool)Country[CountryParameterNames.IRCashRF])
                                {
                                    r.Status = "Y";
                                }
                                else
                                    r.Status = "N";
                            }
                            else
                                r.Status = "Y";
                        }
                        else
                        {
                            if (allowInstantReplacement.Enabled && allowSupaShield.Enabled)
                                r.Status = "Y";
                            else
                            {
                                if (allowInstantReplacement.Enabled)
                                {
                                    if (isIR)
                                        r.Status = "Y";
                                    else
                                        r.Status = "N";
                                }
                                if (allowSupaShield.Enabled)
                                {
                                    if (isIR)
                                        r.Status = "N";
                                    else
                                        r.Status = "Y";
                                }
                            }
                        }
                    }

                    var availableWarranties = new List<WarrantyItemXml>();
                    foreach (var r in warranties.Items)
                    {
                        if (r.Status == "Y" && !WarrantyType.IsFree(r.warrantyLink.TypeCode)) //#17883
                        {
                            availableWarranties.Add(r.ToItem());
                        }

                    }

                    //If free do other bit

                    //#17325
                    var availableWarrantiesList = new List<WarrantyItemXml>();
                    var warrantyIds = new List<int>();

                    foreach (var warranty in availableWarranties)
                    {
                        if (!warrantyIds.Contains(warranty.Id))
                        {
                            availableWarrantiesList.Add(warranty);
                        }

                        warrantyIds.Add(warranty.Id);
                    }

                    if (availableWarrantiesList.Count > 0)
                    {
                        //Launch a pop-up holding the warranties
                        Warranties warrantyPopup = new Warranties(availableWarrantiesList, quantity, currentItem, location, AccountNo, AgreementNo, this, this.FormRoot, isIR);
                        warrantyPopup.ShowDialog();     //launch as a modal dialog
                        var selectedWarranties = new List<WarrantyItemXml>();
                        foreach (var r in availableWarrantiesList)
                        {
                            if (r.Id == warrantyPopup.selectedWarrantyId)
                            {
                                for (var i = 0; i < warrantyPopup.selectedContracts.Count; i++)
                                {
                                    var w = r.Clone();
                                    w.ContractNumber = warrantyPopup.selectedContracts[i];
                                    //Commented to handle decimalplaces='C2'					
                                    //  w.Value = w.PromotionPrice.HasValue ? Convert.ToDecimal(w.PromotionPrice.Value.ToString(DecimalPlaces)) : Convert.ToDecimal(w.RetailPrice.ToString(DecimalPlaces));   //#18619
                                    w.Value = w.PromotionPrice.HasValue ? Convert.ToDecimal(w.PromotionPrice.Value.ToString(DecimalPlaces)) : TrimCurrency(w.RetailPrice.ToString(DecimalPlaces));   //#18619
                                    w.Quantity = 1;
                                    w.DeliveryDate = Convert.ToString(dtDeliveryRequired.Value);
                                    w.DeliveryTime = Convert.ToString(cbTime.SelectedItem);
                                    w.BranchForDeliveryNote = drpBranch.Text;
                                    w.Location = Convert.ToInt32(drpBranch.Text);
                                    selectedWarranties.Add(w);
                                }
                            }
                        }

                        if (selectedWarranties.Count > 0)   //a warranty was selected
                        {
                            XmlNode parent = xml.findItem(itemDoc.DocumentElement, key);

                            /* tag the warranty on to the parent item's related items node */
                            //     parent.SelectSingleNode("RelatedItems").AppendChild(new XmlNode());
                            foreach (var w in selectedWarranties)
                            {
                                string xPath = "//Item[@Type = 'Warranty' and @Code = '" + w.Code + "' and @Location = '" + w.Location.ToString() + "' and @ContractNumber = '" + w.ContractNumber + "']";
                                foreach (XmlNode toDel in itemDoc.SelectNodes(xPath))
                                {
                                    /* only delete them if their parent is the current item */
                                    if (toDel.ParentNode.ParentNode.Attributes[Tags.Key].Value == key ||
                                        toDel.Attributes[Tags.Quantity].Value == "0")
                                        toDel.ParentNode.RemoveChild(toDel); /* properly delete rather than set qty to 0 */
                                }
                                parent.SelectSingleNode("RelatedItems").AppendChild(parent.OwnerDocument.ImportNode(w.ToXml(), true));
                            }
                            //LiveWire 69185 set a boolean value to true so that it is known that a warranty has been selected
                            warrantySelected = true;

                        }
                        /* reset the warranty field */
                        Warranty = "";
                        ContractNos = null;

                        // Check whether Customer Details can be entered
                        this.SetCustomerDetails();

                        //IP - 28/04/08 - UAT(287) v5.1
                        //If performing an Instant Replacement then we do not want to generate a special account
                        //as previously this would cause an error as it would incorrectly
                        //try to retrieve the lineitems on the agreement using the new special account number
                        //which is incorrect.
                        if (!this.Replacement)
                            this.GeneratePTAccountNumber();
                    }
                }
                else
                    status = false;
            }
            //}
            ClearItemDetails();
            txtProductCode.Text = "";

            return status;
        }

        private decimal TrimCurrency(string price)
        {
            return Convert.ToDecimal(Convert.ToString(price.Remove(0, price.LastIndexOf(Convert.ToString(Country[CountryParameterNames.CurrencySymbolForPrint])) + 1)));

        }

        private void AttachWarranty(string acctno, int agrmtno, string key)
        {
            string err;
            dvExchange = null;      //#16828
            DataSet ds = AccountManager.GetExchangeDetails(acctno, agrmtno, out err);

            //warranties = Services.GetService(STL.Common.Services.Services.ServiceTypes.CosacsWeb).GetWarranties(txtProductCode.Text, drpLocation.SelectedItem.ToString());
            //if (warranties.Items == null)
            //    warranties.Items = new List<WarrantyResult.Item>();

            if (err.Length > 0)
                ShowError(Error);
            else
            {
                //if (!Exchanged)
                //{
                XmlNode exchangeParent = xml.findItem(itemDoc.DocumentElement, key);
                //DataView dvExchange 
                dvExchange = ds.Tables[TN.Warranties].DefaultView;                      //#16828

                //IP - 29/07/09 - UAT(655) - Select the category of the item we are adding back onto the sale as an exchange item.
                string category = Convert.ToString(exchangeParent.Attributes[Tags.ProductCategory].Value);
                //IP - 29/01/10 - LW 72136 - Select the refcode of the item we are adding back onto the sale as an exchange item.
                //string refcode = Convert.ToString(exchangeParent.Attributes[Tags.RefCode].Value); //#16240

                ProcessExchange(exchangeParent);        //#16828
                                                        //}
            }
        }

        //#17678
        private void AttachFreeReplacementWarranty(string code, string location, int itemId, int itemQty)
        {
            string err;
            DataSet ds = AccountManager.GetExchangeDetails(AccountNo, AgreementNo, out err);

            string key = itemId + "|" + location;

            AddFreeWarranty(code, location, key, itemQty, itemQty);         // #17677

            // #17290 - 
            if (collectionType == "I" && AccountType != AT.Special && instantReplacement.ItemId != itemId)
            {
                XmlNode exchangeParent = xml.findItem(itemDoc.DocumentElement, key);
                dvExchange = ds.Tables[TN.Warranties].DefaultView;                      //#16828

                string category = Convert.ToString(exchangeParent.Attributes[Tags.ProductCategory].Value);
                ProcessExchange(exchangeParent);
            }

            freeReplacementWarrantyAdded = true;  //#18437       
        }

        private void ProcessExchange(XmlNode exchangeParent)
        {
            foreach (DataRowView r in dvExchange)
            {
                int LinkIrwId = Convert.ToInt32(r["LinkIrwId"]);     // #17290
                var LinkIrwContractNo = LinkIrwId != 0 ? (string)r["LinkIrwContractNo"] : string.Empty;        // #17290

                var warrantyId = LinkIrwId != 0 ? LinkIrwId : Convert.ToInt32(r[CN.ItemId]);             // #17290
                string contractNo = LinkIrwId != 0 ? LinkIrwContractNo : (string)r[CN.ContractNo];      // #17290
                short locn = Convert.ToInt16(r[CN.StockLocn]);
                var itemNo = (string)r[CN.ItemNo];      // #16292

                //string xPath = "//Item[@Type = 'Warranty' and @Code = '" + warranty + "' and @Location = '" + locn.ToString() + "' and @ContractNumber = '" + contractNo + "']";
                //IP - 29/07/09 - UAT(655) - Find a warranty to attach to the product that is of the same category. IP - 29/01/10 - LW 72136 - and of the same refcode.
                string xPath = "//Item[@Type = 'Warranty' and @Quantity!= 0 and @ItemId = '" + warrantyId + "' and @Location = '" + locn.ToString() + "' and @ContractNumber = '" + contractNo + "' and @WarrantyType != '" + 'F' + "']"; // # 18401 #16240 - removed refcode check

                XmlNode toExchange = itemDoc.DocumentElement.SelectSingleNode(xPath);

                string xPathCheckParentForWarranty = ".//Item[@Type = 'Warranty' and @ItemId = '" + warrantyId + "' and @Location = '" + locn.ToString() + "' and @ContractNumber = '" + contractNo + "' and @WarrantyType != '" + 'F' + "']";
                XmlNode checkExchangeParentForWarranty = exchangeParent.SelectSingleNode(xPathCheckParentForWarranty);


                // #16292 check if exising warranties are valid for exchange item
                if (toExchange != null && checkExchangeParentForWarranty == null)
                {
                    var attach = false;
                    var x = toExchange.Attributes[Tags.Code].Value;

                    if (warranties.Items != null)           //#16828
                    {
                        foreach (var w in warranties.Items)
                        {
                            if (w.warrantyLink.Number == toExchange.Attributes[Tags.Code].Value.ToString())
                            {
                                attach = true;
                                break;
                            }
                        }
                    }

                    bool dresult = false;

                    bool maxPurchased = false;

                    string warrantyPath = "RelatedItems/Item[@Type = 'Warranty' and @Quantity != 0 and @WarrantyType !='" + WarrantyType.Free + "']";   //#17973
                    XmlNodeList warrantyList = exchangeParent.SelectNodes(warrantyPath);

                    if (warrantyList.Count == int.Parse(exchangeParent.Attributes[Tags.Quantity].Value.ToString()))
                    {
                        maxPurchased = true;
                    }

                    if (attach && maxPurchased == false)
                    {
                        if (MessageBox.Show("An existing warranty '" //+ warranty.Attributes["Description1"].Value.Trim() 
                       + "' is available for this item.  Do you want to move it to this item?", "Warranty Available", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            dresult = true;
                            AddedExistingWarrantyLink = true;
                        }
                    }

                    if (dresult)
                    {
                        toExchange.ParentNode.RemoveChild(toExchange);
                        /* properly delete rather than set qty to 0 */
                        toExchange = itemDoc.ImportNode(toExchange, true);
                        exchangeParent.SelectSingleNode("RelatedItems").AppendChild(toExchange);
                    }
                    //Exchanged = true; - IP - 29/01/10 - LW 72136
                    //break; //#16240 - There may be multiple warranties
                }
            }
        }

        private void AddAssociatedItems_Click(object sender, System.EventArgs e)
        {
            try
            {
                int index = dgLineItems.CurrentRowIndex;
                associatedItemRightClick = true;        //IP - 25/02/11 - #3230

                //string productCode = (string)((DataView)dgLineItems.DataSource)[index]["ProductCode"];
                int itemId = Convert.ToInt32(((DataView)dgLineItems.DataSource)[index]["ItemId"]);
                short location = Convert.ToInt16(((DataView)dgLineItems.DataSource)[index]["StockLocation"]);
                int Quantity = Convert.ToInt32(((DataView)dgLineItems.DataSource)[index]["QuantityOrdered"]);
                AddRelatedItems(itemId, Quantity, location);
                populateTable();
                dgLineItems.DataSource = itemsView;
                currentItem = null;
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        /// <summary>
        /// This function will manually add a warranty to a selected 
        /// product if there are any available
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddWarranty_Click(object sender, System.EventArgs e)
        {
            try
            {
                string key = "";
                string productCode = "";
                int itemId = 0;
                string location = "";
                double quantity = 0;
                double unitPrice = 0;
                string contract = "";

                if (this.KitClicked)
                {
                    key = (string)tvItems.SelectedNode.Tag;
                    string[] k = key.Split('|');
                    //itemId = k[0];  //TODO need to get ItemId
                    productCode = k[0];
                    location = k[1];
                    contract = k[2];
                    key = itemId + "|" + location;
                }
                else
                {
                    int index = dgLineItems.CurrentRowIndex;
                    productCode = (string)((DataView)dgLineItems.DataSource)[index]["ProductCode"];
                    itemId = Convert.ToInt32(((DataView)dgLineItems.DataSource)[index]["ItemId"]);
                    location = (string)((DataView)dgLineItems.DataSource)[index]["StockLocation"];
                    contract = (string)((DataView)dgLineItems.DataSource)[index][CN.ContractNo];
                    key = itemId + "|" + location;
                }

                _hasdatachanged = true;

                string xPath = "//Item[@Key = '" + key + "' and @ContractNumber = '" + contract + "' and @Quantity != 0]";
                XmlNode found = itemDoc.DocumentElement.SelectSingleNode(xPath);
                if (found != null)
                {
                    if (found.Attributes[Tags.CanAddWarranty].Value == Boolean.TrueString)
                    {
                        if (!MaximumWarrantiesPurchased(key, Convert.ToInt32(found.Attributes[Tags.Quantity].Value)))
                        {
                            quantity = Convert.ToDouble(found.Attributes[Tags.Quantity].Value);
                            unitPrice = Convert.ToDouble(found.Attributes[Tags.UnitPrice].Value);
                            if (!AddWarranty(itemId, productCode, Convert.ToInt16(location), unitPrice, quantity))
                            {
                                ShowInfo("M_NOWARRANTIES");
                            }
                            else
                            {
                                populateTable();
                            }
                        }
                    }
                    else
                    {
                        ShowInfo("M_CANTADDWARRANTY");
                    }
                }

                AddFreeWarranty(productCode, location, itemId + "|" + location, Convert.ToInt32(currentItem.Attributes[Tags.Quantity].Value));   //#18591

                populateTable();         //#18591
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void txtValue_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //IP - 26/10/09 - CoSACS Improvement - Credit Discounts
            string itemType = string.Empty;
            string itemCategory = string.Empty;
            bool inDISCategory = false;
            errorProvider1.SetError(txtValue, "");

            if (currentItem == null)
                return;

            percentage = false; //IP - 02/06/11 - CR1212 - RI - #3794

            try
            {
                if (txtValue.Text.Contains("%"))
                {
                    percentage = true;     // RI % value entered
                }
                txtValue.Text = txtValue.Text.Replace("%", "");                 // RI remove %

                //Make sure it's in the right format by stripping it
                //and then putting it back
                txtValue.Text = Convert.ToDouble(StripCurrency(txtValue.Text)).ToString(DecimalPlaces);

                //if (percentage && -Convert.ToDecimal(txtValue.Text) > 100)       // RI 
                if (percentage && -Convert.ToDecimal(StripCurrency(txtValue.Text)) > 100)       // IP - 09/09/11 - RI - #8132
                {
                    errorProvider1.SetError(txtValue, "A percentage value greater than 100 is not allowed");
                }
                else
                {
                    errorProvider1.SetError(txtValue, "");
                }


                //update the currentItem node quantity field with this new value
                currentItem.Attributes[Tags.Value].Value = this.StripCurrency(txtValue.Text);
                if (Convert.ToBoolean(currentItem.Attributes[Tags.ValueControlled].Value))
                {
                    currentItem.Attributes[Tags.UnitPrice].Value = currentItem.Attributes[Tags.Value].Value;
                }

                btnEnter.Enabled = txtProductCode.Text.Length > 0 && drpLocation.Text.Length > 0;

                if (txtProductCode.Text == "LOAN" && !this.IsLoan)
                {
                    SetLoanError();
                }


                //IP - 26/10/09 - CoSACS Improvement - Credit Discounts
                //If this is a discount that has been setup in the DIS code category and the value entered is a positive value then display an error to the user.

                itemType = currentItem.Attributes[Tags.Type].Value;

                if (itemType == IT.Discount)
                {
                    itemCategory = currentItem.Attributes[Tags.ProductCategory].Value;

                    foreach (DataRow row in ((DataTable)StaticData.Tables[TN.Discounts]).Rows)
                    {
                        if (itemCategory == Convert.ToString(row[CN.Code]))
                        {
                            inDISCategory = true;
                            break;
                        }
                    }

                    if (inDISCategory == true && Convert.ToDouble(StripCurrency(txtValue.Text)) > 0)
                    {
                        errorProviderForWarning.SetError(lblDummyForErrorProvider, "You are entering a Positive Discount which will increase the Agreement Amount");
                    }
                    else
                    {
                        errorProviderForWarning.SetError(lblDummyForErrorProvider, "");
                    }
                }

                ValidateNonGiftItem();
                // errorProvider1.SetError(txtValue, "");

                if (percentage)
                {
                    txtValue.Text = txtValue.Text + '%';        // RI 
                }
            }
            catch (Exception ex)
            {
                //e.Cancel = true;
                txtValue.Focus();
                txtValue.Select(0, txtValue.Text.Length);
                errorProvider1.SetError(txtValue, ex.Message);
            }
        }

        /// <summary>
        /// This handles clicking on the tree view.
        /// It will highlite the appropriate record on the datagrid
        /// according to which treeView node has been clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvItems_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
        {
            try
            {
                if ((string)e.Node.Tag != null)
                {
                    string key = (string)e.Node.Tag;

                    string[] k = key.Split('|');
                    string itemId = k[0];
                    string location = k[1];
                    string contract = k[2];
                    string parentItemId = k[3];

                    string parentItemNo = "";

                    int index = 0;
                    foreach (DataRowView row in itemsView)
                    {
                        if (Convert.ToString(row.Row["ItemId"]) == itemId &&
                            Convert.ToString(row.Row["StockLocation"]) == location &&
                            Convert.ToString(row.Row[CN.ContractNo]) == contract &&
                            (Convert.ToString(row["ParentItemId"]) == "0" || Convert.ToString(row["ParentItemId"]) == parentItemId))
                        {
                            dgLineItems.Select(index);
                            dgLineItems.CurrentRowIndex = index;
                            parentItemNo = Convert.ToString(row["ParentProductCode"]);

                            parentItemNo = parentItemNo == "NA" ? "" : parentItemNo;
                        }
                        else
                        {
                            dgLineItems.UnSelect(index);
                        }
                        index++;
                    }

                    string xPath = "//Item[@Key = '" + itemId + "|" + location + "' and @ContractNumber = '" + contract + "' and @Quantity != 0";
                    if (parentItemNo != String.Empty)
                    {
                        xPath += " and ../../@Code='" + parentItemNo + "'";
                    }
                    xPath += "]";
                    XmlNode found = itemDoc.DocumentElement.SelectSingleNode(xPath);

                    if (found != null)
                    {
                        currentItem = found.CloneNode(true);

                        if (!Convert.ToBoolean(found.Attributes[Tags.SPIFFItem].Value))
                            SetItemDetails(currentItem);
                        else
                        {
                            ClearItemDetails();
                            txtProductCode.Text = "";
                        }

                        switch ((string)found.Attributes[Tags.Type].Value)
                        {
                            case IT.Kit:
                                this.KitClicked = true;
                                break;
                            default:
                                this.KitClicked = false;
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// When the terms type is selected get back the values specific to this
        /// terms type. Based on this information we know whether to enable the 
        /// deposit field or not.
        /// Also recalculate the service charge in case the user is changing the 
        /// terms type from a previously selected value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void drpTermsType_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            Function = "drpTermsType_SelectedIndexChanged";
            _hasdatachanged = true;
            try
            {
                var selectedTT = drpTermsType.SelectedItem as DataRowView;
                lblFirstInstalmentWaiver.Visible = instalmentWaived &&
                                                   selectedTT != null &&
                                                   Convert.ToString(selectedTT[CN.InstalPreDel]).Trim() == "Y";

                if (staticLoaded && !supressEvents)
                {
                    // Use a new scoring band when the user selects a new terms type
                    errorProvider1.SetError(drpTermsType, "");

                    //68688 03/Jan/2006 Removed this line so that deposit is calculated everytime [PC]
                    //decimal newDeposit = Convert.ToDecimal(StripCurrency(txtDeposit.Text));

                    //68688 03/Jan/2006 Added two lines  below so that terms type will pick up the default deposit each time is is changed [PC]
                    decimal newDeposit = 0;
                    cbDeposit.Checked = false;

                    SetTermsType(drpTermsType, numPaymentHolidays,
                        cbDeposit, txtDeposit, txtNoMonths, drpLengths, _loadAccount,
                        Convert.ToDecimal(StripCurrency(txtSubTotal.Text)),
                        true, ref _defaultDeposit, ref _depositIsPercentage,
                        ref newDeposit, txtNoMonths.Value, CheckDepositWaiver());

                    // Adjust the RF Spend Available by the change in the Deposit amount
                    RFMax = RFMax - this._deposit + newDeposit;
                    _deposit = newDeposit;

                    if (txtNoMonths.Value > 0)
                    {
                        CalculateServiceCharge(Config.CountryCode,
                            Convert.ToDecimal(StripCurrency(txtDeposit.Text)),
                            Convert.ToDecimal(txtNoMonths.Value),
                            Convert.ToDecimal(StripCurrency(txtSubTotal.Text)) - Convert.ToDecimal(StripCurrency(txtSalesTax.Text)),
                            Convert.ToDecimal(numPaymentHolidays.Value));
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void drpLengths_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            // Copy the selected length to the number of months
            // This should fire the txtNoMonths_ValueChanged event
            if (IsPositive(drpLengths.Text))
            {
                txtNoMonths.Value = Convert.ToDecimal(this.drpLengths.Text);
            }
        }

        private void txtNoMonths_ValueChanged(object sender, System.EventArgs e)
        {
            _hasdatachanged = true;
            try
            {
                if (!supressEvents && txtNoMonths.Value != oldNoMonths)
                {
                    oldNoMonths = txtNoMonths.Value;

                    decimal max = Convert.ToDecimal(Math.Floor(Convert.ToDouble(txtNoMonths.Value * 0.1M)));
                    if (numPaymentHolidays.Value > max)
                    {
                        oldPaymentHolidays = max;
                        numPaymentHolidays.Value = max;
                    }

                    CalculateServiceCharge(Config.CountryCode,
                        Convert.ToDecimal(StripCurrency(txtDeposit.Text)),
                        Convert.ToDecimal(txtNoMonths.Value),
                        Convert.ToDecimal(StripCurrency(txtSubTotal.Text)) - Convert.ToDecimal(StripCurrency(txtSalesTax.Text)),
                        Convert.ToDecimal(numPaymentHolidays.Value));

                    // Check the agreement term is not too long if selling an Affinity item
                    CheckAffinityTerm();
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void txtDeposit_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (!supressEvents)
                {
                    CalculateServiceCharge(Config.CountryCode,
                        Convert.ToDecimal(StripCurrency(txtDeposit.Text)),
                        Convert.ToDecimal(txtNoMonths.Value),
                        Convert.ToDecimal(StripCurrency(txtSubTotal.Text)) - Convert.ToDecimal(StripCurrency(txtSalesTax.Text)),
                        Convert.ToDecimal(numPaymentHolidays.Value));
                }
            }
            catch (Exception ex)
            {
                //e.Cancel = true;
                txtDeposit.Focus();
                txtDeposit.Select(0, txtDeposit.Text.Length);
                errorProvider1.SetError(txtDeposit, ex.Message);
            }
        }
        //10.6 CR- Sales Order - Print Save-Function For incrementing ord/invoice version
        private void UpdateOrdInvoiceVersionText(string accountNumber)
        {
            DataSet ds = AccountManager.GetAccountForRevision(accountNumber, AgreementNo, out _scoringBand, out _currentBand, out Error);
            foreach (DataTable dt in ds.Tables)
            {
                if (dt.TableName == TN.AccountDetails)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        lblAgrInvoieNum.Text = string.IsNullOrEmpty(row["AgreementInvoiceNumber"].ToString()) ? "--" : row["AgreementInvoiceNumber"].ToString();
                        lblCHOrdInvoice.Text = "Ord/Invoice: " + lblAgrInvoieNum.Text;
                    }
                }
            }
        }

        private string GetInvoiceNumberAndVersion(string accountNumber)        {
            string invoiceNoWithVer = string.Empty;            invoiceNoWithVer = AccountManager.GetInvoiceNumberWithVersion(accountNumber, out Error);            if (Error.Length > 0)                ShowError(Error);            else            {
                lblAgrInvoieNum.Text = string.IsNullOrEmpty(invoiceNoWithVer) ? "--" : invoiceNoWithVer;                lblCHOrdInvoice.Text = "Ord/Invoice: " + lblAgrInvoieNum.Text;
            }
            return invoiceNoWithVer;
        }


        private void menuSave_Click(object sender, System.EventArgs e)
        {
            //10.6 CR- Sales Order - Print Save- Allow user to save while printing 
            //and track UI changes to increment invoice version
            _saveButtonClicked = _isSalesOrderChanged;

            bool mstatus = true;
            int noPrints = 0;
            int status = 0;
            string invoice = "";
            //Arun    
            AccountManager.DisableSave(AccountNo, out invoice, out status);

            if (status == 1 && AccountType =="R" && invoice!= "Legacy")
            {
                string message = "Are you sure? Sales Order will be sent to EMMA for approval";
                string title = "Close Window";
                MessageBoxButtons buttons = MessageBoxButtons.OKCancel;
                DialogResult result = MessageBox.Show(message, title, buttons);
                if (result == DialogResult.OK)
                {
                    //If Max Spend(Dont Ignore the Sign) < Agreement Total then Max Spend = Agreement Total 
                    //And New RF Credit Available = Existing RF Credit Available + Max Spend with positive(Ignore the negative Sign)
                    if (MoneyStrToDecimal(txtRFMax.Text)<MoneyStrToDecimal(txtAgreementTotal.Text))
                    {
                        txtRFMax.Text = txtAgreementTotal.Text;
                        decimal MaxSpend = MoneyStrToDecimal(txtRFMax.Text);
                        RFCreditLimit = RFCreditLimit + MaxSpend < 0 ? -MaxSpend : MaxSpend;
                        txtRFMax.BackColor = SystemColors.Control;
                        txtRFMax.ForeColor = SystemColors.ControlText;
                        txtSubTotal.BackColor = SystemColors.Control;
                        txtSubTotal.ForeColor = SystemColors.ControlText;
                            
                    }
                }           
                else
                    if (result == DialogResult.Cancel)
                {
                    return;
                }
            }

            var IsStoreCard = PayMethod.IsPayMethod(Convert.ToInt16(((DataRowView)drpPayMethod.SelectedItem)[CN.Code].ToString()), PayMethod.StoreCard);

            try
            {
                if (this.CustomerID.Length == 0)
                {
                    loadAccountData(AccountNo, false);
                }

                if (this.CustomerID.Length == 0)
                {
                    if (DialogResult.Yes == ShowInfo("M_ADDCUSTDETAILS", MessageBoxButtons.YesNo))
                    {
                        if (PaidAndTaken)
                        {
                            // Inform user that no transaction will take place until they return to the Cash and Go screen and print a receipt
                            ShowInfo("M_NOTRANSACTION", MessageBoxButtons.OK);
                        }
                        menuCustomerSearch_Click(menuSave, new System.EventArgs());
                        // IP - 17/04/08 - (69630) - Commented the below out as UAT304 fixed as (69630) and
                        // the below was a temporary fix.
                        //// stop linking to customer twice which causes doubling of payment and lineitem qtys
                        //btnCustomerSearch.Enabled = false;      // jec 02/01/08 UAT304
                    }
                }
                else
                {
                    if (PaidAndTaken)
                    {
                        mstatus = SaveAccount();

                        if (IsStoreCard)
                            mtb_CardNo.Enabled = false;

                        if (mstatus)
                        {
                            string temp = (string)drpSoldBy.SelectedItem;
                            int salesPerson = Convert.ToInt32(temp.Substring(0, temp.IndexOf(":") - 1));
                            string salesPersonName = temp.Replace(Convert.ToString(salesPerson), "").Replace(":", "").Trim();                //IP - 17/05/12 - #9447 - CR1239

                            XmlNodeList contracts = null;
                            string xpath = "//" + Elements.Item + "[(@" + Tags.Type + "='" + IT.Warranty + "' or @" + Tags.Type + "='" + IT.KitWarranty + "') and @" + Tags.Quantity + " > 0]";
                            contracts = itemDoc.SelectNodes(xpath);
                            int noContracts = contracts.Count;

                            ((MainForm)FormRoot).browsers = ((CommonForm)FormRoot).CreateBrowserArray(noContracts + 1);

                            this.NewPrintTaxInvoice(AccountNo, AgreementNo,
                                AccountType,
                                CustomerID,
                                PaidAndTaken,
                                Collection,
                                instantReplacement,
                                MoneyStrToDecimal(txtTendered.Text),
                                MoneyStrToDecimal(txtChange.Text),
                                itemDoc.DocumentElement,
                                AgreementNo,
                                ((MainForm)FormRoot).browsers[0], ref noPrints, false, true, salesPerson,
                                Convert.ToInt16(((DataRowView)drpPayMethod.SelectedItem)[CN.Code]),
                                payMethodSet,
                                (bool)chxTaxExempt.Checked,
                                salesPersonName);                                                                                       //IP - 17/05/12 - #9447 - CR1239
                                                                                                                                        // 17/06/08 rdb uat435 printing warranty on C+G return incorrectly, add check on returnAuthorisedBy 
                            if (contracts != null && contracts.Count > 0 && (bool)Country[CountryParameterNames.PrintWarrantyContract] && returnAuthorisedBy == 0)
                                PrintWarrantyContracts(((MainForm)FormRoot).browsers, AccountNo, CustomerID, AgreementNo, contracts, ref noPrints, 1, false);

                            if ((bool)Country[CountryParameterNames.PrizeVouchersActive] &&
                                !Collection && !Replacement)
                            {
                                PrintPrizeVouchers(AccountNo, MoneyStrToDecimal(txtSubTotal.Text), AgreementNo,
                                                    Date.blankDate, false, false);
                            }

                            if (payMethodSet.Tables["PayMethodList"].Select("PayMethod = '" + PayMethod.StoreCard + "'").Length > 0)
                            {

                                var invoiceNo = AgreementNo;
                                var receiptNo = storeCardTransRefNo;

                                foreach (DataRow row in payMethodSet.Tables["PayMethodList"].Select("PayMethod = '" + PayMethod.StoreCard + "'"))
                                {
                                    var card = storeCardPayInfoList.Find(p => p.StoreCardValidated.CardNo == Convert.ToInt64(row["chequeno"]));
                                    if (card != null || CashAndGoReturn)
                                    {
                                        if (CashAndGoReturn)
                                        {
                                            var cust = CashandGoReturnDetails.CustDetails;
                                            NewPrintStoreCardReceipt(String.Format("{0} {1}", cust.FirstName, cust.LastName),
                                                               cust.AddressLine1,
                                                               cust.AddressLine2,
                                                               cust.AddressLine3,
                                                               cust.AddressLine4,
                                                               Convert.ToString(invoiceNo), receiptNo,
                                                               row["value"].ToString().TryParseDecimalStrip().Value * -1, CashandGoReturnDetails.StoreCardValidated);
                                        }
                                        else
                                            NewPrintStoreCardReceipt(String.Format("{0} {1}", card.CustDetails.FirstName, card.CustDetails.LastName),
                                                                    card.CustDetails.AddressLine1,
                                                                    card.CustDetails.AddressLine2,
                                                                    card.CustDetails.AddressLine3,
                                                                    card.CustDetails.AddressLine4,
                                                                    Convert.ToString(invoiceNo), receiptNo,
                                                                    MoneyStrToDecimal(row["value"].ToString()), card.StoreCardValidated);  //rrtt
                                    }
                                }

                            }

                            if (instantReplacement == null)
                                InitData();
                            else
                                CloseTab();     /* if this an instant replacement close the screen so */
                                                /* it's not possible to do it twice */
                            ResetCG();
                        }

                    }
                    else
                    {
                        if (LoyaltyMember || voucheraddedonrevise)
                        {
                            //CustomerManager.LoyaltyLinkAccount(AccountNo, CustomerID);
                            LoyaltySaveVouchers();
                        }
                        mstatus = SaveAccount(); //uat 52

                        if (mstatus) //uat 52
                        {
                            //// 10.6 CR - Sales Order - Save Print: Get latest invoice number and version 
                            //// this was modified due to performance issue and for locking customer account
                            //string invcNumberAndVersion = this.GetInvoiceNumberAndVersion(_acctNo);

                            //if (string.IsNullOrEmpty(invcNumberAndVersion))
                            //    ShowInfo("M_SALESSAVEDSUCCESSFUL", MessageBoxButtons.OK);
                            //else
                            //    ShowInfo("M_SALESSAVEDSUCCESSFULRF", new object[] { invcNumberAndVersion }, MessageBoxButtons.OK);


                            //10.6 CR - Sales Order - Print Save -for showing proper message after saving
                            MessageBox.Show("Saved Successfully", "Information", MessageBoxButtons.OK);

                            this.UpdateOrdInvoiceVersionText(_acctNo);
                            AccountManager.DisableSave(AccountNo, out invoice, out status);

                            if (status == 0) 
                            {
                                btnSave.Enabled = false;
                                dgLineItems.Enabled = false;
                                txtProductCode.Enabled = false;
                                MessageBox.Show("Your request shopping has sent to EMMA, please review", "Information", MessageBoxButtons.OK);
                            }

                            // 10.7 CR - Maximum Monthly Installment
                            if (isMmiAppliedForSaleAtrr
                                    && AccountType == AT.ReadyFinance
                                    && mmiPropResult == PR.Referred)
                            {
                                bool acctHP = this.createHP;
                                string acctType = AccountType;
                                string custNo = CustomerID;
                                string acctNo = AccountNo;
                                string newAcctNo = AccountNo;
                                DateTime dtPropRF = datePropRF;
                                BasicCustomerDetails customerScreen = null;
                                string stageCheckType = string.Empty;
                                string stagePropResult = string.Empty;
                                string Error = string.Empty;

                                Crownwood.Magic.Controls.TabPage tp = ((MainForm)FormRoot).MainTabControl.SelectedTab;

                                if (AT.IsCreditType(AccountType))
                                {
                                    CreditManager.GetUnclearedStage(this.AccountNo, ref newAcctNo, ref stageCheckType, ref dtPropRF, ref stagePropResult, out Error);
                                    if (Error.Length > 0)
                                        ShowError(Error);
                                    else
                                    {
                                        string[] parms = null;
                                        SanctionStage1 stage1 = null;
                                        SanctionStage2 stage2 = null;
                                        DocumentConfirmation docComf = null;
                                        Referral refer = null;
                                        AuthoriseCheck Auth;

                                        switch (stageCheckType)
                                        {
                                            case SS.S2:
                                                Auth = new AuthoriseCheck("SanctionStatus", "tbbStage2");
                                                if (Auth.ControlPermissionCheck(Credential.User).HasValue)
                                                {
                                                    stage2 = new SanctionStage2(custNo, dtPropRF, acctNo, acctType
                                                                            , STL.Common.Constants.ScreenModes.SM.Edit, FormRoot, this, customerScreen);
                                                    ((MainForm)FormRoot).AddTabPage(stage2);
                                                }
                                                break;
                                            case SS.DC:
                                                Auth = new AuthoriseCheck("SanctionStatus", "tbbDoc");
                                                if (Auth.ControlPermissionCheck(Credential.User).HasValue)
                                                {
                                                    docComf = new DocumentConfirmation(custNo, dtPropRF, acctNo, acctType
                                                                            , STL.Common.Constants.ScreenModes.SM.Edit, FormRoot, this, customerScreen);
                                                    ((MainForm)FormRoot).AddTabPage(docComf);
                                                }
                                                break;
                                            case SS.AD: //launch additional data
                                                break;
                                            case SS.R:
                                                Auth = new AuthoriseCheck("SanctionStatus", "tbbReferral");
                                                if (Auth.ControlPermissionCheck(Credential.User).HasValue)
                                                {
                                                    refer = new Referral(this.createHP, custNo, dtPropRF, acctNo, acctType
                                                                    , STL.Common.Constants.ScreenModes.SM.Edit, FormRoot, this, customerScreen, true);
                                                    ((MainForm)FormRoot).AddTabPage(refer);
                                                }
                                                break;
                                            default:
                                                Auth = new AuthoriseCheck("SanctionStatus", "tbbStage1");
                                                if (Auth.ControlPermissionCheck(Credential.User).HasValue)
                                                {
                                                    parms = new String[3];
                                                    parms[0] = custNo;
                                                    parms[1] = acctNo;
                                                    parms[2] = acctType;
                                                    stage1 = new SanctionStage1(createHP, parms, STL.Common.Constants.ScreenModes.SM.Edit, FormRoot, this, customerScreen);
                                                    ((MainForm)this.FormRoot).AddTabPage(stage1, 18);
                                                }
                                                break;
                                        }

                                        docTab = ((MainForm)FormRoot).MainTabControl.SelectedTab;
                                        ((MainForm)FormRoot).MainTabControl.SelectedTab = tp;
                                    }
                                }
                            }
                            //else
                            //{
                            //    //10.6 CR - Sales Order - Print Save -for showing proper message after saving
                            //    ShowInfo("M_SALESSAVEDSUCCESSFUL", MessageBoxButtons.OK);

                            //    this.UpdateOrdInvoiceVersionText(_acctNo);
                            //}

                            //if (!reqTaxInvoicePrint)  //IP - 05/03/12 - #9403 - 'Print tax invoice' menu option selected therefore do not close the screen.
                            //{
                            //    //windows is closed after "Save Successfull" message.
                            //    //this was modified due to prevent account locking.
                            //    this.CloseTab();
                            //}

                            if (docTab != null)
                                ((MainForm)FormRoot).MainTabControl.SelectedTab = docTab;

                            reqTaxInvoicePrint = false;                 //IP - 05/03/12 - #9403
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "menuSave_Click");
            }
        }

        private void PrintCreditNote()
        {
            /* the credit note is basically the same as the tax invoice 
			 * but it must reflect the state of the account before it was
			 * saved which is why it must be printed before saving the 
			 * changes that have been made. */
            if ((bool)Country[CountryParameterNames.PrintCreditNote] && revision && !PaidAndTaken) //IP - 09/10/12 - #11407 - re-instated
                                                                                                   //if (revision && !PaidAndTaken)                                                              //IP - 05/03/12 - #9403 
            {
                //if (ShowInfo("M_PRINTCREDITNOTE", MessageBoxButtons.YesNo) == DialogResult.Yes)       //IP - 05/03/12 - #9403 
                //{
                int noPrints = 0;
                AxWebBrowser[] browsers = CreateBrowserArray(1);

                string temp = (string)drpSoldBy.SelectedItem;
                int salesPerson = Convert.ToInt32(temp.Substring(0, temp.IndexOf(":") - 1));
                bool taxExempt = AccountManager.IsTaxExempt(AccountNo, AgreementNo.ToString(), out Error);

                NewPrintTaxInvoice(AccountNo, AgreementNo,
                    AccountType, CustomerID, PaidAndTaken,
                    Collection, null, 0, 0, itemDoc.DocumentElement,
                    AgreementNo, browsers[0], ref noPrints, true, true, salesPerson,
                    Convert.ToInt16(((DataRowView)drpPayMethod.SelectedItem)[CN.Code]),
                    payMethodSet,
                    taxExempt);

                /* HACK give the request chance to arrive before we carry on and save the account */
                Thread.Sleep(1000);

                //}
            }
        }

        private bool SaveAccount()
        {
            bool status = true;
            string accountNo;
            short branch;
            string acctType = "";
            string CODFlag;
            int salesPerson = 0;
            string salesPersonStr;
            string SOA = "";
            double agreementTotal = 0;
            double subTotal = 0;
            double deposit;
            double serviceCharge;
            XmlNode lineItems;
            string termsType = "";
            string country;
            DateTime dateFirst = DateTime.MinValue.AddYears(1899);
            double instalAmount;
            double finalInstalment;
            string paymentMethod = "";
            int months = Convert.ToInt32(txtNoMonths.Value);
            bool taxExempt = false;
            bool dutyFree = false;
            decimal taxAmount = 0;
            string bankCode = "";
            string bankAcctNo = "";
            string chequeNo = "";
            short payMethod = 0;
            string cancellationCode = (string)Country[CountryParameterNames.CancellationRejectionCode];
            decimal tendered = 0;
            decimal dueDay = 1;
            string propResult = "";
            //string propResultIC = "";   //jec 30/11/07 69413
            int MinCRLimitRef = Convert.ToInt32(Country[CountryParameterNames.MinCRLimitRef]);                       //jec 10/08/10 CR1113
            decimal MaxExceedCRLimit = Convert.ToDecimal(Country[CountryParameterNames.MaxExceedCRLimit]);         //jec 10/08/10 CR1113
            double spendSubTotal = 0;                   //jec 10/08/10 CR1113
            double MaxSpend = Convert.ToDouble(StripCurrency(txtRFMax.Text)) + Convert.ToDouble((RFCreditLimit * ((MaxExceedCRLimit / 100))));         //jec 10/08/10 CR1113
                                                                                                                                                       //string rflimitAction = "";      //jec 10/08/10 CR1113     jec removed 25/01/11     
            DataTable dtScoringDetails = null;      //jec 10/08/10 CR1113
            string currentReferralNotes = null;     //jec 10/08/10 CR1113
            int PrefDay = 0;

            #region "MMI Variables"

            decimal mmiLimit = 0;
            decimal mmiThreshold = 0;
            decimal mmiThresholdLimit = 0;
            isMmiAllowed = Convert.ToBoolean(Country[CountryParameterNames.EnableMMI]);

            #endregion "MMI Variables"


            try
            {
                Wait();

                ((MainForm)FormRoot).statusBar1.Text = GetResource("M_SAVINGACCOUNT");

                /* Thailand need to print a Credit Note before we save the account */
                PrintCreditNote();



                //First validate the drop downs if they're visible
                if (drpSOA.Visible)
                {
                    if (drpSOA.SelectedIndex == 0)
                    {
                        drpSOA.Focus();
                        errorProvider1.SetError(drpSOA, GetResource("M_ENTERMANDATORY"));
                        //return false;
                        status = false;
                    }
                    else
                    {
                        SOA = (string)drpSOA.SelectedItem;
                        SOA = SOA.Substring(0, SOA.IndexOf("-") - 1);
                        errorProvider1.SetError(drpSOA, "");
                    }
                }

                if (status)
                {
                    if (drpAccountType.SelectedIndex == 0)
                    {
                        drpAccountType.Focus();
                        errorProvider1.SetError(drpAccountType, GetResource("M_ENTERMANDATORY"));
                        //return false;
                        status = false;
                    }
                    else
                    {
                        acctType = (string)drpAccountType.SelectedItem;
                        errorProvider1.SetError(drpAccountType, "");
                    }
                }

                /* if it's a paid and taken account, double check that a 
				 * sufficient payment has been made */
                if (status && PaidAndTaken && MoneyStrToDecimal(txtChange.Text) < 0)
                {
                    ShowInfo("M_PAYMENTTOOLOW");
                    //return false;
                    status = false;
                }

                if (status)
                {
                    if (revision && !PaidAndTaken && this.AccountType != AT.ReadyFinance)
                    {
                        // For a revision the Sales Person selected index can be zero
                        salesPersonStr = (string)drpSoldBy.SelectedItem;
                        if (salesPersonStr.IndexOf(":") == -1)
                        {
                            errorProvider1.SetError(drpSoldBy, GetResource("M_ENTERMANDATORY"));
                            return false;
                        }
                        else
                        {
                            salesPerson = Convert.ToInt32(salesPersonStr.Substring(0, salesPersonStr.IndexOf(":") - 1));
                        }
                    }
                    else
                    {
                        // Check the Sales Person selected index is not zero
                        if (drpSoldBy.Visible)
                        {
                            if (drpSoldBy.SelectedIndex == 0)
                            {
                                drpSoldBy.Focus();
                                errorProvider1.SetError(drpSoldBy, GetResource("M_ENTERMANDATORY"));
                                //return false;
                                status = false;
                            }
                            else
                            {
                                salesPersonStr = (string)drpSoldBy.SelectedItem;
                                salesPerson = Convert.ToInt32(salesPersonStr.Substring(0, salesPersonStr.IndexOf(":") - 1));
                                errorProvider1.SetError(drpSoldBy, "");
                            }
                            // check salesperson number > 0 and < 10000 (max 4 digits) 68605 jec 17/11/06
                            //if (salesPerson < 1 || salesPerson > 9999)
                            //{
                            //    errorProvider1.SetError(drpSoldBy, GetResource("M_INVALIDSALESPERSON"));
                            //    status = false;
                            //}
                            // check valid salesperson number is entered
                            if (txtEmpNumber.Text.Length > 0 && IsStrictNumeric(txtEmpNumber.Text) && Convert.ToInt32(txtEmpNumber.Text) > 0 && salesPerson < 1)
                            {
                                errorProvider1.SetError(txtEmpNumber, GetResource("M_INVALIDSALESPERSONNO"));
                                //return false;
                                status = false;
                            }
                        }
                        else
                        {
                            if (drpSoldBy.SelectedIndex != -1)
                            {
                                salesPersonStr = (string)drpSoldBy.SelectedItem;
                                salesPerson = Convert.ToInt32(salesPersonStr.Substring(0, salesPersonStr.IndexOf(":") - 1));
                            }
                        }
                    }
                }

                if (status)
                {
                    if ((bool)Country[CountryParameterNames.PaymentMethod])
                    {
                        if (drpPaymentMethod.Visible)
                        {
                            if (drpPaymentMethod.SelectedIndex == 0)
                            {
                                drpPaymentMethod.Focus();
                                errorProvider1.SetError(drpPaymentMethod, GetResource("M_ENTERMANDATORY"));
                                //return false;
                                status = false;
                            }
                            else
                            {
                                paymentMethod = (string)drpPaymentMethod.SelectedItem;
                                paymentMethod = paymentMethod.Substring(0, paymentMethod.IndexOf("-") - 1);
                                errorProvider1.SetError(drpPaymentMethod, "");
                            }
                        }
                    }
                }

                if (status)
                {
                    if ((decimal)Country[CountryParameterNames.FixedDateFirst] == 1)
                    {
                        if (dtDateFirst.Value < this._today && !revision)
                        {
                            dtDateFirst.Focus();
                            errorProvider1.SetError(dtDateFirst, GetResource("M_ENTERMANDATORY"));
                            //return false;
                            status = false;
                        }
                        else
                        {
                            dateFirst = dtDateFirst.Value;
                            dueDay = dtDateFirst.Value.Day;
                            errorProvider1.SetError(drpSoldBy, "");
                        }
                    }
                    else
                    {
                        // RD/PN 14/04/05 66723 Added as datefirst was being reset to 01-01-1900 when revising agreement found on v3504
                        if ((decimal)Country[CountryParameterNames.FixedDateFirst] == 0 && revision)
                            dateFirst = dtDateFirst.Value;
                    }
                }


                termsType = refinTermsType == null ? TermsType : refinTermsType; //#18572    // set termtype from Refinance print

                if (status)
                {
                    if (AT.IsCreditType(AccountType))
                    {
                        if (drpTermsType.Visible)
                        {
                            if (drpTermsType.SelectedIndex == 0)
                            {
                                errorProvider1.SetError(drpTermsType, GetResource("M_ENTERMANDATORY"));
                                //return false;
                                status = false;
                            }
                            else
                            {
                                termsType = drpTermsType.Text;
                                termsType = termsType.Substring(0, termsType.IndexOf("-") - 1);
                                errorProvider1.SetError(drpTermsType, "");
                            }
                        }
                        if (txtNoMonths.Value == 0)
                        {
                            errorProvider1.SetError(txtNoMonths, GetResource("M_ENTERMANDATORY"));
                            //return false;
                            status = false;
                        }
                        else if (txtNoMonths.Value > 60)
                        {
                            errorProvider1.SetError(txtNoMonths, GetResource("M_INSTALMAX"));
                            //return false;
                            status = false;
                        }
                        else
                        {
                            errorProvider1.SetError(txtNoMonths, "");
                        }
                        if ((bool)Country[CountryParameterNames.AbilitySetFirstPaymentDate] && txtPrefDay.Visible)
                        {
                            PrefDay = Convert.ToInt32(txtPrefDay.Text);
                        }
                    }

                    //IP - 24/04/08 - UAT(418) v.5.1 - Commented out the below as, if this was a 'Cash'
                    //account with an affinity item and there were no Terms Types setup for affinity
                    //a error would be raised when executing 'termsType = affinityTerms.Substring(0, affinityTerms.IndexOf("-") - 1)'
                    //due to the Terms Type being blank. Also shouldn't execute below for Cash accounts as
                    //they do not have Terms Types.

                    //else if (this._affinityTerms)
                    else if (this._affinityTerms && (!AT.IsCashType(AccountType)))
                    {
                        // If this is an affinity account set the terms type to the 
                        // first affinity terms type (prefer DelNonStocks = 0)
                        string affinityTerms = "";
                        string preferredAffinityTerms = "";

                        // Search the Terms Types filtered for this account type
                        foreach (DataRowView row in this.dvTermsTypes)
                        {
                            if (affinityTerms == "" && (string)row[CN.Affinity] == "Y" && (string)row[CN.TermsType] != "")
                                affinityTerms = (string)row[CN.TermsType];
                            if (preferredAffinityTerms == "" && (string)row[CN.Affinity] == "Y" && (short)row[CN.DeliverNonStocks] == 0 && (string)row[CN.TermsType] != "")
                                preferredAffinityTerms = (string)row[CN.TermsType];
                        }

                        if (affinityTerms == "")
                        {
                            // There are no Terms Types set up for this account type
                            // so search all terms types
                            foreach (DataRow row in this.dtTermsTypes.Rows)
                            {
                                if (affinityTerms == "" && (string)row[CN.Affinity] == "Y" && (string)row[CN.TermsType] != "")
                                    affinityTerms = (string)row[CN.TermsType];
                                if (preferredAffinityTerms == "" && (string)row[CN.Affinity] == "Y" && (short)row[CN.DeliverNonStocks] == 0 && (string)row[CN.TermsType] != "")
                                    preferredAffinityTerms = (string)row[CN.TermsType];
                            }
                        }

                        if (preferredAffinityTerms != "")
                            termsType = preferredAffinityTerms.Substring(0, preferredAffinityTerms.IndexOf("-") - 1);
                        else
                            termsType = affinityTerms.Substring(0, affinityTerms.IndexOf("-") - 1);
                    }
                    else if (WarrantiesOnCredit)
                    {
                        TermsType = termsType = TT.WarrantyOnCredit;
                    }
                    else if (PaidAndTaken && (ptWarranties || hasInstallation))
                    {
                        TermsType = termsType = TT.PTWarranty;
                    }
                    else
                        termsType = "00";                  //IP - 05/01/12 - #9382 - LW74386
                                                           //termsType = "";
                }

                // Check the agreement term is not too long if selling an Affinity item
                if (status) status = this.CheckAffinityTerm();

                if (status)     //carry on and save the account
                {
                    if (!PaidAndTaken && acctType == AT.Special && AgreementNo == 1)
                        // Agreement Total is not for one sale
                        // This was occassionaly being calculated as negative
                        agreementTotal = 0;
                    else if (PaidAndTaken && Collection)
                        agreementTotal = Convert.ToDouble(_origAgreementTotal - MoneyStrToDecimal(txtDue.Text));
                    else if (PaidAndTaken && WarrantiesOnCredit)
                    {
                        agreementTotal = Convert.ToDouble(WarrantiesOnCreditAgreementTotal);
                        tendered = Convert.ToDecimal(StripCurrency(txtSubTotal.Text));
                    }
                    else if (AT.IsCreditType(acctType))
                        agreementTotal = Convert.ToDouble(AgreementTotal);
                    else
                        agreementTotal = Convert.ToDouble(StripCurrency(txtSubTotal.Text));
                    //prevent negative agreement total but only if not legacy cash and go - legacy cash and go has authoristation required
                    if (agreementTotal < 0 && !this._authorisationRequired)
                    {
                        status = false;
                        ShowInfo("M_NEGATIVEAGRTOTAL", new object[] { agreementTotal.ToString(DecimalPlaces) });
                    }
                }

                #region "Calculate MMI For Sale"


                if (AccountType == AT.ReadyFinance && isMmiAllowed)
                {
                    CreditManager.GetMmiThresholdForSale(CustomerID, AccountNo, termsType, out isMmiAppliedForSaleAtrr, out mmiLimit, out mmiThreshold, out Error);
                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                    {
                        mmiThresholdLimit = mmiLimit + mmiThreshold;
                    }
                }

                #endregion "Calculate MMI For Sale"


                if (status)
                {
                    subTotal = Convert.ToDouble(StripCurrency(txtSubTotal.Text));
                    spendSubTotal = (subTotal - (Convert.ToDouble(nonStockTotal) - Convert.ToDouble(loanTotal)));

                    if (AccountType == AT.ReadyFinance && spaPrint == false) //IP - 24/09/10 - UAT5.4 - UAT(33) - Do not check if RF limit exceeded when doing SPA (Extended Term) from Telephone Action
                    {
                        // cr906 remove loan amounts from nonStockTotal when checking against max spend
                        if ((subTotal - (Convert.ToDouble(nonStockTotal) - Convert.ToDouble(loanTotal))) > Convert.ToDouble(StripCurrency(txtRFMax.Text)))
                        {
                            //  CR1113 RF credit limit referral changes
                            if (MaxExceedCRLimit > 0 || MinCRLimitRef > 0)           //max percentage to exceed CR limit is set 
                            {
                                // Spend limit % exceeded and RF limit< threshold
                                if (spendSubTotal > Convert.ToDouble(StripCurrency(txtRFMax.Text)) && this.RFCreditLimit < MinCRLimitRef && spendSubTotal > MaxSpend)
                                {
                                    //rflimitAction = "Exceed";
                                    {
                                        ShowInfo("M_RFLIMITEXCEEDED");
                                        status = false;
                                    }
                                }
                                else
                                    if (spendSubTotal > Convert.ToDouble(StripCurrency(txtRFMax.Text)) && spendSubTotal < MaxSpend)
                                {
                                    //rflimitAction = "Approve";
                                    // Refer account 
                                    CreditManager.SpendLimitReferral(CustomerID, AccountNo, datePropRF, "Limit exceeded within % allowed", this.RFCreditLimit,
                                        Config.CountryCode, out Error);
                                    if (Error.Length > 0)
                                        ShowError(Error);
                                    else
                                    {   // Get referal data - Proposal notes
                                        DataSet ds = CreditManager.GetReferralData(this.CustomerID, this.AccountNo, datePropRF, Config.CountryCode, out Error);
                                        if (Error.Length > 0)
                                            ShowError(Error);
                                        else
                                        {
                                            foreach (DataTable dt in ds.Tables)
                                            {
                                                if (dt.TableName == TN.ScoringDetails)
                                                    dtScoringDetails = dt;
                                                if (dt.TableName == TN.ReferralData)
                                                {
                                                    foreach (DataRow r in dt.Rows)
                                                    {
                                                        currentReferralNotes = (string)r[CN.PropNotes];
                                                    }
                                                }
                                            }
                                            // Auto approve
                                            CreditManager.CompleteReferralStage(this.CustomerID, this.AccountNo, datePropRF, "Auto Approved", currentReferralNotes, true, false, false,
                                                Convert.ToInt16(Config.BranchCode), this.RFCreditLimit, Config.CountryCode, out Error);
                                        }
                                    }
                                }

                                if (Error.Length > 0)
                                    ShowError(Error);
                                else
                                    // Spend limit % exceeded and RF limit> threshold
                                    if (spendSubTotal > Convert.ToDouble(StripCurrency(txtRFMax.Text)) && this.RFCreditLimit >= MinCRLimitRef && spendSubTotal > MaxSpend)
                                {
                                    //rflimitAction = "Refer";
                                    CreditManager.SpendLimitReferral(CustomerID, AccountNo, datePropRF, "Maximum allowed % to exceed limit exceeded", this.RFCreditLimit,
                                    Config.CountryCode, out Error);
                                    if (Error.Length > 0)
                                        ShowError(Error);
                                    else
                                    {
                                        propResult = PR.Referred;
                                        //   propResultIC = PR.Referred;
                                        PrintVouchers = false;
                                        if (instantReplacement != null)
                                            instantReplacement.Notify = true;
                                        if (lShowResult.Enabled)
                                        {
                                            //IP - 23/03/11 - CR1245 - Displaying referral reasons on the referral/rejection popup based on Country Parameter.
                                            if (Convert.ToBoolean(Country[CountryParameterNames.ReasonsReferPopup]) == false)
                                                ShowInfo("M_REFER", new object[] { string.Empty });
                                            else
                                                ShowInfo("M_REFER", new object[] { "Maximum allowed % to exceed limit exceeded" }); //IP - 15/03/11 - #3314 - CR1245
                                        }
                                    }
                                }
                            }
                            //  end CR1113 RF credit limit referral changes
                            else if (isMmiAppliedForSaleAtrr)
                            {
                                int totalTermLength = 0;
                                int monthToExtend = 0;
                                decimal amountToDeposit = 0;
                                decimal amountToReduce = 0;
                                decimal instalmentAmount = Convert.ToDecimal(StripCurrency(txtInstalment.Text));
                                _closeSaleWithoutSaving = false;

                                if (mmiThresholdLimit < instalmentAmount)
                                {
                                    PopulateActionNotesForSale(mmiThresholdLimit, out totalTermLength, out monthToExtend, out amountToDeposit, out amountToReduce);
                                    
                                    SaleAction sa = new SaleAction();
                                    sa.TotalMonth = totalTermLength;
                                    sa.MonthToExtend = monthToExtend;
                                    sa.AmountToDeposit = amountToDeposit;
                                    sa.AmountToReduce = amountToReduce;
                                    sa.SetControlsForNotes();
                                    sa.ShowDialog();

                                    if (sa.Modify)
                                    {
                                        status = false;
                                        _closeSaleWithoutSaving = true;
                                    }
                                    else if (sa.Refer)
                                    {
                                        CreditManager.SpendLimitReferral(CustomerID, AccountNo, datePropRF,
                                                                         sa.rtxtNewReferralNotes.Text, this.RFCreditLimit,
                                                                        Config.CountryCode, out Error);
                                        if (Error.Length > 0)
                                            ShowError(Error);
                                        else
                                        {
                                            propResult = PR.Referred;
                                            mmiPropResult = PR.Referred;
                                            PrintVouchers = false;
                                            if (instantReplacement != null)
                                                instantReplacement.Notify = true;

                                            ShowInfo("M_REFER", new object[] { "MMI exceeded" });
                                        }

                                    }
                                    else if (sa.CloseSale)
                                    {
                                        status = false;
                                        _closeSaleWithoutSaving = true;
                                        this.CloseTab();
                                    }
                                    else
                                    {
                                        status = false;
                                    }
                                }
                            }
                            else if (lSpendLimit.Enabled)
                            {
                                SpendLimitReferral limit = new SpendLimitReferral();
                                limit.ShowDialog();

                                if (limit.refer == true)
                                {
                                    CreditManager.SpendLimitReferral(CustomerID, AccountNo, datePropRF,
                                        limit.rtxtNewReferralNotes.Text, this.RFCreditLimit,
                                        Config.CountryCode, out Error);
                                    if (Error.Length > 0)
                                        ShowError(Error);
                                    else
                                    {
                                        propResult = PR.Referred;
                                        //  propResultIC = PR.Referred; //jec 30/11/07 69413
                                        PrintVouchers = false;
                                        if (instantReplacement != null)
                                            instantReplacement.Notify = true;

                                        if (lShowResult.Enabled)
                                        {
                                            //IP - 23/03/11 - CR1245 - Displaying referral reasons on the referral/rejection popup based on Country Parameter.
                                            if (Convert.ToBoolean(Country[CountryParameterNames.ReasonsReferPopup]) == false)
                                                ShowInfo("M_REFER", new object[] { string.Empty });
                                            else
                                                ShowInfo("M_REFER", new object[] { "Maximum allowed % to exceed limit exceeded" }); //IP - 15/03/11 - #3314 - CR1245
                                        }
                                    }
                                }
                                else
                                // if not Refer - show message and return to detail entry/ammend
                                // 68976 jec 23/05/07
                                {
                                    ShowInfo("M_RFLIMITEXCEEDED");
                                    status = false;
                                }
                            }
                            else
                            {
                                ShowInfo("M_RFLIMITEXCEEDED");
                                status = false;
                            }
                        }
                        else
                        {
                            // add code here for removing/unsetting refer flag 68538
                            // code removed as always resets R flag if limit not exceeded (68955 jec 10/05/07)
                            //       CreditManager.UnClearFlag(AccountNo, SS.R, true5, Credential.UserId, out Error);
                            //       if (Error.Length > 0)
                            //       {
                            //           ShowError(Error);
                            //           status = false;
                            //       }

                            if (isMmiAppliedForSaleAtrr)
                            {
                                int totalTermLength = 0;
                                int monthToExtend = 0;
                                decimal amountToDeposit = 0;
                                decimal amountToReduce = 0;
                                decimal instalmentAmount = Convert.ToDecimal(StripCurrency(txtInstalment.Text));
                                _closeSaleWithoutSaving = false;

                                if (mmiThresholdLimit < instalmentAmount)
                                {
                                    PopulateActionNotesForSale(mmiThresholdLimit, out totalTermLength, out monthToExtend, out amountToDeposit, out amountToReduce);
                                    
                                    SaleAction sa = new SaleAction();
                                    sa.TotalMonth = totalTermLength;
                                    sa.MonthToExtend = monthToExtend;
                                    sa.AmountToDeposit = amountToDeposit;
                                    sa.AmountToReduce = amountToReduce;
                                    sa.SetControlsForNotes();
                                    sa.ShowDialog();

                                    if (sa.Modify)
                                    {
                                        status = false;
                                        _closeSaleWithoutSaving = true;
                                    }
                                    else if (sa.Refer)
                                    {
                                        CreditManager.SpendLimitReferral(CustomerID, AccountNo, datePropRF,
                                                                         sa.rtxtNewReferralNotes.Text, this.RFCreditLimit,
                                                                        Config.CountryCode, out Error);
                                        if (Error.Length > 0)
                                            ShowError(Error);
                                        else
                                        {
                                            propResult = PR.Referred;
                                            mmiPropResult = PR.Referred;
                                            PrintVouchers = false;
                                            if (instantReplacement != null)
                                                instantReplacement.Notify = true;

                                            ShowInfo("M_REFER", new object[] { "MMI exceeded" });
                                        }
                                    }
                                    else if (sa.CloseSale)
                                    {
                                        status = false;
                                        _closeSaleWithoutSaving = true;
                                        this.CloseTab();
                                    }
                                    else
                                    {
                                        status = false;
                                    }
                                }
                            }
                        }
                    }
                }


                if (status && AT.IsCreditType(AccountType))
                {
                    if (subTotal < creditMinPrice)
                    {
                        ShowInfo("M_CREDITMINVALUE");
                        status = false;
                    }

                }

                #region Address type validation
                if (status)
                {
                    if (CustomerID.Length > 0 && !PaidAndTaken)
                    {
                        /* check that the customer has the right address types
						 * for delivery */
                        string notFound = "";
                        string[] addressTypes = CustomerManager.GetDistinctAddressTypes(CustomerID, out Error);
                        if (Error.Length > 0)
                        {
                            status = false;
                            ShowError(Error);
                        }
                        else
                        {
                            /* foreach delivery address in the xml doc check that it's
							 * in the addressTypes array */
                            XmlNodeList items = itemDoc.DocumentElement.SelectNodes("//Item[@Quantity != '0' and @DeliveryAddress != '']");
                            foreach (XmlNode item in items)
                            {
                                bool found = false;
                                string s = item.Attributes[Tags.DeliveryAddress].Value.Trim();
                                if (s.Length > 0)
                                {
                                    foreach (string addType in addressTypes)
                                    {
                                        if (s == addType)
                                        {
                                            found = true;
                                            break;
                                        }
                                    }
                                    if (!found && (notFound.IndexOf(s) < 0))
                                        notFound += s + Environment.NewLine;
                                }
                            }

                            //#14729 - Prevent save if address not found
                            if (notFound.Length > 0)
                            {
                                ShowInfo("M_MISSINGADDRESSWARNING", new object[] { notFound });
                                status = false;
                            }
                        }
                    }
                }

                //#18604 - CR15594
                if (status)
                {
                    //#18604 - CR15594
                    findReadyAssist(itemDoc.DocumentElement);
                    //if (readyAssistExists && readyAssistLength.HasValue)
                    //{
                    //    if ((decimal.Parse(txtNoMonths.Value.ToString()) != readyAssistLength.Value) && AT.IsCreditType(AccountType))
                    //    {
                    //        ShowInfo("M_READYASSISTTERM", new object[] {readyAssistLength});
                    //        status = false;
                    //    }
                    //}
                }


                #endregion

                lineItems = itemDoc.DocumentElement;

                //should not allow to save non stock account with empty lineitem grid
                if (chxNonStock.Checked && (dgLineItems.VisibleRowCount == 0))
                {
                    ShowError("Please add a non stock item");
                    status = false;

                }
                //should not allow to save stock item in non stock account
                else if (chxNonStock.Checked && lineItems.SelectNodes("//Item[@Type='Stock' and @Quantity != '0']").Count > 0)
                {
                    ShowError("Account should not contain stock items");
                    status = false;
                    return false;
                }
                //should allow to save as non stock account only if above checks are satisfied
                if (chxNonStock.Checked && status == true)
                {
                    AccountManager.SaveAccountForSaleOnlyNonStock(txtAccountNumber.UnformattedText);
                }

                if (status)
                {
                    accountNo = AccountNo;
                    branch = Convert.ToInt16((string)drpBranch.SelectedItem);
                    CODFlag = chxCOD.Checked ? "Y" : "N";

                    deposit = Convert.ToDouble(StripCurrency(txtDeposit.Text));
                    serviceCharge = Convert.ToDouble(StripCurrency(txtTerms.Text));

                    //lineItems = itemDoc.DocumentElement;
                    country = Config.CountryCode;

                    if ((decimal)Country[CountryParameterNames.FixedDateFirst] == 2)
                        dueDay = Convert.ToDecimal(drpDueDay.SelectedItem);

                    instalAmount = Convert.ToDouble(StripCurrency(txtInstalment.Text));
                    finalInstalment = Convert.ToDouble(StripCurrency(txtFinalInstalment.Text));
                    taxExempt = chxTaxExempt.Checked;
                    dutyFree = chxDutyFree.Checked;
                    taxAmount = Convert.ToDecimal(StripCurrency(txtSalesTax.Text));

                    bankCode = (string)((DataRowView)drpBank.SelectedItem)[CN.BankCode];
                    bankAcctNo = txtBankAcctNo.Text;
                    chequeNo = txtChequeNo.Text;
                    payMethod = Convert.ToInt16(((DataRowView)drpPayMethod.SelectedItem)[CN.Code]);

                    // The scoring band stays the same unless the user changes terms or band
                    string newBand = "";
                    if (termsType == _origTerms && _scoringBand == _origScoringBand)
                        newBand = _currentBand;
                    else
                        newBand = _scoringBand;

                    string bureauFailure = "";
                    DateTime dateProp = this._today;

                    XmlNode replacementXml = null;
                    if (instantReplacement != null)
                        replacementXml = instantReplacement.Serialise();

                    if (warrantyRenewals != null && !warrantyRenewalSet.Tables.Contains(TN.WarrantyList))
                        warrantyRenewalSet.Tables.Add(warrantyRenewals);

                    if (variableRatesSet.Tables.Contains(TN.Rates))
                        foreach (DataTable dt in variableRatesSet.Tables)
                            foreach (DataRow r in dt.Rows)
                                r[CN.AcctNo] = accountNo;

                    CashPrice = Convert.ToDecimal(agreementTotal) - Convert.ToDecimal(serviceCharge) -
                                dtTaxAmount - MoneyStrToDecimal(txtAdminCharge.Text) - MoneyStrToDecimal(txtInsuranceCharge.Text);

                    //LiveWire 69185
                    decimal tenderedValue = 0;
                    if (PaidAndTaken && this.CustomerID.Length == 0 && warrantySelected)
                    {
                        tenderedValue = 0;
                        //lineItems = null;             // #15066  
                    }
                    else
                    {
                        tenderedValue = MoneyStrToDecimal(this.txtTendered.Text);
                    }



                    if (lineItems != null)
                    {
                        var node = lineItems.SelectSingleNode("//Item[@Code='" + LoyaltyDropStatic.VoucherCode + "' and @Quantity != '0']");
                        if (node != null)
                            lineItems.RemoveChild(node);
                    }

                    //IP - 19/01/11 - Store Card
                    string storeCardAcctNo = string.Empty;


                    var parameters = new STL.PL.WS2.SaveNewAccountParameters
                    {
                        AccountNumber = AccountNo,
                        BranchNo = branch,
                        AccountType = AccountType,
                        CODFlag = CODFlag,
                        SalesPerson = salesPerson,
                        SOA = SOA,
                        AgreementTotal = agreementTotal,
                        Deposit = deposit,
                        ServiceCharge = serviceCharge,
                        //LineItems = itemDoc.DocumentElement, //IP - 18/02/11 - Sprint 5.11 - #2947 - Replaced with the below.
                        LineItems = (btnCustomerSearch.Visible == true && btnCustomerSearch.Enabled == true && PaidAndTaken) ? null : lineItems,       // #15122 LineItems parameter set to null if Customer Link required
                        TermsType = termsType,
                        NewBand = newBand,
                        CountryCode = country,
                        DateFirst = dateFirst,
                        InstalAmount = instalAmount,
                        FinalInstalment = finalInstalment,
                        PaymentMethod = paymentMethod,
                        Months = months,
                        TaxExempt = taxExempt,
                        DutyFree = dutyFree,
                        TaxAmount = taxAmount,
                        Collection = Collection,
                        BankCode = bankCode,
                        BankAcctNo = bankAcctNo,
                        ChequeNo = chequeNo,
                        PayMethod = payMethod,
                        ReplacementXml = replacementXml,
                        DtTaxAmount = dtTaxAmount,
                        LoyaltyCardNo = txtLoyaltyCardNo.Text,
                        ReScore = ReScore,
                        Tendered = GiftVoucherValue + tenderedValue,
                        GiftVoucherValue = GiftVoucherValue,
                        CourtsVoucher = CourtsVoucher,
                        VoucherReference = VoucherReference,
                        VoucherAuthorisedBy = VoucherAuthorisedBy,
                        AccountNoCompany = VoucherCompanyAcctNo,
                        PromoBranch = Convert.ToInt16(Config.BranchCode),
                        PaymentHolidays = Convert.ToInt16(numPaymentHolidays.Value),
                        PayMethodSet = payMethodSet,
                        DueDay = Convert.ToInt16(dueDay),
                        ReturnAuthorisedBy = returnAuthorisedBy,
                        WarrantyRenewalSet = warrantyRenewalSet,
                        VariableRatesSet = variableRatesSet,
                        ResetPropResult = true,
                        Autoda = AutoDA,
                        User = Credential.UserId,
                        StoreCardAcctNo = storeCardValidated != null ? storeCardValidated.Acctno : "",  //IP - 17/01/11 - Store Card
                        StoreCardNumber = storeCardValidated != null ? storeCardValidated.CardNo : (long?)null, //IP - 17/01/11 - Store Card
                        PaidAndTaken = PaidAndTaken,
                        HasInstallation = hasInstallation,                                                     // #14432
                                                                                                               //CustLinkRequired = btnCustomerSearch.Enabled                                          // #15122
                        CashAndGoReturn = CashAndGoReturn,       // #16339
                        CollectionType = this.collectionType,     // #17678
                        WarrantyRefunds = this.warrantyRefunds.ToDataTable(),    //#16607
                        ReadyAssist = this.readyAssistExists,                    //#18603 - CR15594
                        ReadyAssistTermLength = this.readyAssistLength           //#18603 - CR15594
                        ,
                        ReviseAccount = _saveButtonClicked,
                        PrefDay = Convert.ToInt16(PrefDay)   //CR 10.7

                    };

                    AccountManager.SaveNewAccount(parameters, CustomerID ,RFCreditLimit, MoneyStrToDecimal(txtRFMax.Text),
                                                    ref _agreementNo, ref propResult, ref dateProp,
                                                    out bureauFailure, out storeCardTransRefNo, out referralReasons,
                                                    out Error);  //IP - 15/03/11 - #3314 - CR1245 - Return referral reasons


                    //IP - 23/03/11 - CR1245 - Displaying referral reasons on the referral/rejection popup based on Country Parameter.
                    if (Convert.ToBoolean(Country[CountryParameterNames.ReasonsReferPopup]) == false)
                    {
                        referralReasons = string.Empty;
                    }

                    if (Error.Length > 0)
                    {
                        ShowError(Error);
                        status = false;
                    }
                    else
                    {
                        decimal installment = Convert.ToDecimal(StripCurrency(txtInstalment.Text));

                        if (isMmiAppliedForSaleAtrr
                            && AccountType == AT.ReadyFinance
                            && propResult != PR.Referred
                            && installment > mmiLimit
                            && installment <= (mmiLimit + mmiThreshold)
                            )
                        {
                            CreditManager.AuditMmiThresholdUsedInstalment(CustomerID, AccountNo, mmiLimit, mmiThreshold, installment, DateTime.Now, out Error);
                        }


                        if (bureauFailure.Length > 0)
                            ShowWarning(bureauFailure);

                        if ((bool)Country[CountryParameterNames.ManualRefer] &&
                            propResult == PR.Rejected &&
                            AccountType != AT.ReadyFinance &&
                            CustomerID.Length != 0 &&
                            lShowResult.Enabled)
                        {
                            // Allow a rejected application to be manually referred
                            if (DialogResult.Yes == ShowInfo("M_MANUALREFER", MessageBoxButtons.YesNo))
                            {
                                CreditManager.ManualRefer(CustomerID, accountNo, dateProp, true, false, out Error);
                                if (Error.Length > 0)
                                    ShowError(Error);
                                else
                                    propResult = PR.Referred;
                            }
                        }

                        if (propResult == PR.Rejected &&
                            AccountType != AT.ReadyFinance &&
                            CustomerID.Length != 0)
                        {
                            bool cancelled = false;
                            decimal balance = 0;
                            bool outstPayments = false;
                            bool cancel = true;

                            AccountManager.CheckAccountToCancel(accountNo, Config.CountryCode, ref balance, ref outstPayments, out Error);
                            if (Error.Length > 0)
                                ShowError(Error);
                            else
                            {
                                if (outstPayments)
                                {
                                    OutstandingPayment op = new OutstandingPayment(FormRoot);
                                    op.ShowDialog();
                                    cancel = op.rbCancel.Checked;
                                }
                                if (cancel)
                                {
                                    cancelled = AccountManager.CancelAccount(accountNo, CustomerID, Convert.ToInt16((string)drpBranch.SelectedItem),
                                        cancellationCode, balance, Config.CountryCode, "", 0, out Error);
                                    if (Error.Length > 0)
                                        ShowError(Error);
                                }
                            }
                        }

                        if (lShowResult.Enabled && AccountType != AT.ReadyFinance)
                        {
                            if (propResult == PR.Referred)
                                ShowInfo("M_REFER", new object[] { referralReasons }); //IP - 15/03/11 - #3314 - CR1245 - Display referral reasons
                            else if (propResult == PR.Rejected)
                                ShowInfo("M_REJECT", new object[] { referralReasons }); //IP - 15/03/11 - #3314 - CR1245 - Display rejection plus any referral reasons
                            else if (propResult == PR.Accepted)
                                ShowInfo("M_ACCEPT");
                        }


                        var Parms = new STL.PL.WS2.CreditParameters(); //IP - 28/02/11 - #3239 - Code re-instated
                        Parms.AccountNumber = AccountNo;
                        Parms.AccountType = AccountType;
                        //  Parms.CheckType= StageToLaunch.CheckType;
                        Parms.CustomerID = CustomerID;
                        Parms.DateProp = dateProp;
                        Parms.Deposit = Convert.ToDecimal(deposit);
                        Parms.IsLoan = IsLoan;
                        Parms.NewAccountNumber = string.Empty;
                        Parms.PropResult = propResult;
                        Parms.SPAPrint = spaPrint;
                        Parms.TermsType = termsType;
                        Parms.AgreementTotal = Convert.ToDecimal(agreementTotal);
                        Parms.User = Credential.UserId;
                        Parms.DateAccountOpened = DateAccountOpened;
                        // one web service call replaces below code - improves performance... 
                        AccountManager.NewAccountCreditSave(ref Parms);


                        if (Parms.Approved == IC.Approved)
                            ShowInfo("M_ICAPPROVED");

                        //AccountManager.UnlockItem(Credential.UserId, out Error);
                        //if (Error.Length > 0)
                        //    ShowError(Error);
                        //// CR906 loanAccounts do not qualify for instant credit
                        //if (AT.IsCreditType(AccountType) && CustomerID.Length > 0 && !this.IsLoan && spaPrint == false) //IP - 24/09/10 - UAT(33) UAT5.4 - Do not want to check Instant Credit when performing SPA.
                        //{
                        //    // Check if Customer qualifies for Instant Credit  CR907   jec 31/07/07
                        //    // and update Instalplan.InstantCredit if approved
                        //    string approved;
                        //    int agrtot = (Convert.ToInt32(Country[CountryParameterNames.IC_MaxAgrmtTotal]));
                        //    if (agreementTotal <= agrtot && agreementTotal > 0 && propResultIC != PR.Referred)    // jec 30/11/07 propresultIC 69413
                        //    {
                        //        approved = AccountManager.InstantCredit(CustomerID, accountNo, out Error);
                        //        // If approved clear sanction stage 2 and load DC screen
                        //        if (approved == IC.Approved || approved == IC.Granted)
                        //        {
                        //            if(approved == IC.Approved)
                        //                ShowInfo("M_ICAPPROVED");
                        //            // get uncleared stage (should be S2)
                        //            CreditManager.GetUnclearedStage(accountNo,
                        //                ref StageToLaunch.AccountNo,
                        //                ref StageToLaunch.CheckType,
                        //                ref StageToLaunch.DateProp, ref propResult, out Error);

                        //            //save any deposit/instalment flags
                        //            if (deposit > 0)
                        //            {
                        //                AccountManager.SaveInstantCreditFlag(_custid, "DEP", accountNo, out Error); 
                        //            }

                        //            DataSet tt = StaticDataManager.LoadTermsTypeDetails(termsType, out Error);
                        //            if (tt.Tables[TN.TermsType].Rows[0][CN.InstalPreDel].ToString() == "Y")
                        //            {
                        //                AccountManager.SaveInstantCreditFlag(_custid, "INS", accountNo, out Error); 
                        //            }

                        //            if (Error.Length > 0)
                        //                ShowError(Error);

                        //            // clear stage S2
                        //            if (StageToLaunch.CheckType == SS.S2)
                        //            {
                        //                AccountManager.ClearFlag(CustomerID, StageToLaunch.CheckType, StageToLaunch.DateProp, false, accountNo, out Error);
                        //            }
                        //            // Auto DA
                        //            AccountManager.AutoDA(accountNo, out Error);

                        //            if (Error.Length > 0)
                        //                ShowError(Error);
                        //            else
                        //            {
                        //                // 5.0 uat337 rdb 20/11/07 only show DC if open
                        if (Parms.CheckType != null && AccountType == AT.ReadyFinance) //yyzz //Launch DC but only if ready finance
                        {
                            StageToLaunch.DateProp = Parms.DateProp;
                            StageToLaunch.AccountNo = Parms.AccountNumber;
                            // Launch Document Confirmation
                            DocumentConfirmation docComf = null;
                            docComf = new DocumentConfirmation(CustomerID,
                            StageToLaunch.DateProp,
                            StageToLaunch.AccountNo,

                                // rdb 21/11/07 StageToLaunch.AcctType never gets set and will always cause
                                // an error, acctType always set so use instead
                                //StageToLaunch.AcctType,
                                acctType,
                            STL.Common.Constants.ScreenModes.SM.Edit,
                            FormRoot,
                            this, this);
                            Crownwood.Magic.Controls.TabPage tp = ((MainForm)FormRoot).MainTabControl.SelectedTab;
                            ((MainForm)FormRoot).AddTabPage(docComf);
                            docTab = ((MainForm)FormRoot).MainTabControl.SelectedTab;
                            ((MainForm)FormRoot).MainTabControl.SelectedTab = tp;
                        }
                        //            }
                        //        }
                        //    }
                        //}

                        if ((bool)Country[CountryParameterNames.PrizeVouchersActive] && PrintVouchers)
                        {
                            PrintPrizeVouchers(AccountNo, CashPrice, 0, Date.blankDate, false, false);
                            PrintVouchers = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                status = false;
                Catch(ex, Function);
            }
            finally
            {
                if (status)
                {
                    this._hasdatachanged = false;
                    //10.6 CR - Sales Order - Print Save - for SalesOrderChanged is getting reset
                    _isSalesOrderChanged = false;
                    //IP - 18/04/08 - (69630)
                    //Commenting the below out as this was always setting warrantySelected = false
                    //even when there was a warranty attached.
                    //warrantySelected = false;
                }
                else
                {
                    this._hasdatachanged = true;                // #8269 jec 28/09/11 
                    //10.6 CR - Sales Order - Print Save for SalesOrderChanged is getting reset
                    _isSalesOrderChanged = true;
                }

                StopWait();
                ((MainForm)FormRoot).statusBar1.Text = "";
            }
            return status;
        }


        private void PopulateActionNotesForSale(decimal mmiThresholdLimit, out int totalTermLength, out int monthToExtend, out decimal amountToDeposit, out decimal amountToReduce)
        {
            decimal totalMonth = mmiMonths;

            decimal max = Convert.ToDecimal(Math.Floor(Convert.ToDouble(totalMonth * 0.1M)));
            if (numPaymentHolidays.Value > max)
            {
                oldPaymentHolidays = max;
                numPaymentHolidays.Value = max;
            }

            totalMonth = CalculateMonthToExtend(Config.CountryCode,
                                                Convert.ToDecimal(StripCurrency(txtDeposit.Text)),
                                                Convert.ToDecimal(totalMonth),
                                                Convert.ToDecimal(StripCurrency(txtSubTotal.Text)) - Convert.ToDecimal(StripCurrency(txtSalesTax.Text)),
                                                Convert.ToDecimal(numPaymentHolidays.Value),
                                                mmiThresholdLimit);

            monthToExtend = Convert.ToInt32(totalMonth);
            totalTermLength = monthToExtend + Convert.ToInt32(mmiMonths);

            // Calculate amount to deposit
            amountToDeposit = CalculateDepositeAmount(mmiSubTotal, mmiThresholdLimit, mmiMonths);

            int decimalPlaces = Convert.ToInt32(Country[CountryParameterNames.IntalmentRounding]);
            amountToDeposit = Math.Ceiling(amountToDeposit);

            // Calculate amount to deposit
            amountToReduce = amountToDeposit;
        }

        private decimal CalculateDepositeAmount(decimal subTotal, decimal monthly, decimal months)
        {
            decimal amountToDeposite = 0;
            decimal depositeDefferedTermDiff = 0;
            if (months > 0 && monthly < subTotal)
            {
                if (months == 1)
                {
                    depositeDefferedTermDiff = subTotal - monthly;
                }
                else
                {
                    depositeDefferedTermDiff = subTotal - (monthly * months);
                    int decimalPlaces = Convert.ToInt32(Country[CountryParameterNames.IntalmentRounding]);
                    decimal roundTo = Convert.ToDecimal(Math.Pow(0.1, Convert.ToDouble(decimalPlaces)));

                    if (Math.Round(depositeDefferedTermDiff, decimalPlaces) < depositeDefferedTermDiff)
                        depositeDefferedTermDiff = roundTo + Math.Round(depositeDefferedTermDiff, decimalPlaces);
                    else
                        depositeDefferedTermDiff = Math.Round(depositeDefferedTermDiff, decimalPlaces);
                }
            }


            string sBranchno = _acctNo.Length != 0 ? _acctNo.Substring(0, 3) : drpBranch.SelectedItem.ToString();

            if (drpTermsType.SelectedIndex != 0 && AT.IsCreditType(drpAccountType.Text))
            {
                string terms = drpTermsType.Text;
                terms = terms.Substring(0, terms.IndexOf("-") - 1);
                string band = string.Empty;

                variableRatesSet.Tables.Clear();
                if (terms == _origTerms && _scoringBand == _origScoringBand)
                {
                    band = this._scoringBand;
                }
                else
                {
                    if (_origScoringBand == String.Empty && this._currentBand != String.Empty)
                    {
                        band = this._currentBand;
                    }
                    else
                    {
                        band = this._scoringBand;
                    }
                }

                amountToDeposite = AccountManager.CalculateAmountToDeposite(Config.CountryCode, terms, this.AccountNo, band, DateAccountOpened
                                                                    , monthly, months, ChargeableSubTotal, drpAccountType.Text
                                                                    , ChargeableAdminSubTotal
                                                                    , saleOrderInsuranceTax, saleOrderAdminTax, depositeDefferedTermDiff, out Error);

                if (Error.Length > 0)
                    ShowError(Error);
            }

            return amountToDeposite;
        }

        private void drpSoldBy_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            _hasdatachanged = true;
            if (drpSoldBy.SelectedIndex != 0)
                errorProvider1.SetError(drpSoldBy, "");
        }

        private void drpSOA_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            _hasdatachanged = true;
            if (drpSOA.SelectedIndex != 0)
                errorProvider1.SetError(drpSOA, "");
        }

        private void drpPaymentMethod_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            _hasdatachanged = true;
            if (drpPaymentMethod.SelectedIndex != 0)
                errorProvider1.SetError(drpPaymentMethod, "");
        }

        /// <summary>
        /// Since it is possible to make sales from different branch locations
        /// it must be possible to refresh branch specific static data when the 
        /// selected branch changes
        /// </summary>
        /// <param name="branch"></param>
        private void RefreshBranchSpecificData(string branch)
        {
            /* initialise the collections */

            drpSoldBy.DataSource = null;
            salesStaff = new StringCollection();

            salesStaff.Add("Sales Staff");

            if (accountTypes == null)
                accountTypes = new StringCollection();
            else
                accountTypes.Clear();
            accountTypes.Add("Type");
            drpAccountType.DataSource = null;

            XmlUtilities xml = new XmlUtilities();
            XmlDocument dropDowns = new XmlDocument();
            dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

            dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.SalesStaff, new string[] { branch, "S" }));
            dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.AccountType, new string[] { Config.CountryCode, branch }));
            dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.InstallationItemCat, new string[] { }));

            DataSet ds = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out Error);
            if (Error.Length > 0)
                ShowError(Error);
            else
            {
                foreach (DataTable dt in ds.Tables)
                {
                    if (dt.TableName == TN.AccountType)
                    {
                        foreach (DataRow row in dt.Rows)
                            accountTypes.Add((string)row["acctcat"]);

                        if (!revision && (!allowHP.Enabled || !createHP))     //CR903 jec
                            accountTypes.Remove(AT.HP);

                        if (!revision && (!allowRF.Enabled || !createRF))     //CR903 jec
                            accountTypes.Remove(AT.ReadyFinance);

                        if (!revision && !createCash)             //CR903 jec
                            accountTypes.Remove(AT.Cash);

                        if (!revision && !allowSpecial.Enabled)
                            accountTypes.Remove(AT.Special);

                        if (!revision && !allowGOL.Enabled)
                            accountTypes.Remove(AT.GoodsOnLoan);

                        if (Renewal)
                        {
                            accountTypes.Remove(AT.AddTo);
                            accountTypes.Remove(AT.BuyNowPayLater);
                            accountTypes.Remove(AT.GoodsOnLoan);
                            accountTypes.Remove(AT.ReadyFinance);
                            accountTypes.Remove(AT.HP);
                            accountTypes.Remove(AT.Special);
                            accountTypes.Remove(AT.StoreCard);
                        }

                        drpAccountType.DataSource = accountTypes;
                    }
                    if (dt.TableName == TN.SalesStaff)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            //string str = Convert.ToString(row.ItemArray[0]) + " : " + (string)row.ItemArray[1];
                            string str = (string)row.ItemArray[1];
                            salesStaff.Add(str.ToUpper());
                        }

                        drpSoldBy.DataSource = salesStaff;
                        drpSoldBy.SelectedIndex = 0;

                        if (Credential.IsInRole("S"))
                        {
                            int i = drpSoldBy.FindString(Credential.UserId.ToString() + " : " + Credential.Name);
                            if (i != -1)
                            {
                                drpSoldBy.SelectedIndex = i;
                                //drpSoldBy.Enabled = false;
                                txtEmpNumber.Enabled = false;
                            }
                            else
                            {
                                txtEmpNumber.Text = Credential.UserId.ToString();
                                txtEmpNumber_Leave(null, null);
                                txtEmpNumber.Text = "";
                            }
                        }
                    }
                }
            }
        }

        private void InitialiseStaticData()
        {
            try
            {
                this._scoringBand = (Convert.ToString(Country[CountryParameterNames.TermsTypeBandDefault]));
                lblDummyForErrorProvider.Text = ""; // Hiding this
                dtDeliveryRequired.Value = this._today.AddDays(Convert.ToInt32(Country[CountryParameterNames.DefaultDelDays]));

                StringCollection soa = new StringCollection();
                soa.Add("SOA");
                StringCollection mop = new StringCollection();
                mop.Add("Payment Method");
                StringCollection branchNos = new StringCollection();
                StringCollection branchNos2 = new StringCollection();
                StringCollection dueDays = new StringCollection();
                this.allowInstantReplacement = new Control();
                this.allowSupaShield = new Control();
                this.addAdditionalSpiff = new Control();
                this.addAdditionalSpiff.Name = "addAdditionalSpiff";
                this.allowInstantReplacement.Enabled = this.allowSupaShield.Enabled = false;
                this.addAdditionalSpiff.Enabled = false;

                dynamicMenus = new Hashtable();
                HashMenus();
                ApplyRoleRestrictions();

                chxTaxExempt.Enabled = allowTaxExempt.Enabled;

                menuStock.Enabled = false;
                menuMain = new Crownwood.Magic.Menus.MenuControl();
                menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuAccount, menuCustomer, menuStock, menuCashTill, menuHelp });

                txtAccountNumber.BackColor = SystemColors.Window;
                txtUnitPrice.BackColor = SystemColors.Window;
                txtValue.BackColor = SystemColors.Window;
                txtAvailable.BackColor = SystemColors.Window;
                //txtChange.BackColor = SystemColors.Window;
                txtChange.Text = (0).ToString(DecimalPlaces);
                //txtDue.BackColor = SystemColors.Window;
                txtDue.Text = (0).ToString(DecimalPlaces);
                txtTendered.Text = (0).ToString(DecimalPlaces);
                txtAmount.Text = (0).ToString(DecimalPlaces);

                if (ManualSale)
                {
                    txtAccountNumber.ReadOnly = false;
                }

                //initialise the XML document and the tree view
                itemDoc = new XmlDocument();
                itemDoc.LoadXml("<ITEMS></ITEMS>");
                tvItems.Nodes.Add(new TreeNode("Account"));
                tvItems.Indent = 0;

                //Set up the datagrid columns
                if (itemsTable == null)
                {
                    //Create the table to hold the Line items
                    itemsTable = new DataTable("Items");
                    itemsView = new DataView(itemsTable);
                    // rdb uat363 when  2 kits are added with same item (DS) a pk violation occurs
                    // try adding parentproductcode column and add this column to key

                    // rdb uat363
                    DataColumn[] key = new DataColumn[5];

                    itemsTable.Columns.Add(new DataColumn("ProductCode"));
                    itemsTable.Columns.Add(new DataColumn("ProductDescription"));
                    itemsTable.Columns.Add(new DataColumn("StockLocation"));
                    itemsTable.Columns.Add(new DataColumn("DelAddress")); //IP - 09/02/10 - CR1049 Merged - Malaysia Enhancements (CR1072)
                    itemsTable.Columns.Add(new DataColumn("QuantityOrdered"));
                    itemsTable.Columns.Add(new DataColumn("UnitPrice"));
                    itemsTable.Columns.Add(new DataColumn("Value"));
                    itemsTable.Columns.Add(new DataColumn("ContractNo"));
                    // rdb uat363
                    itemsTable.Columns.Add(new DataColumn("ParentProductCode"));
                    //IP - 03/12/10 - Store Card
                    itemsTable.Columns.Add(new DataColumn("ItemType"));
                    itemsTable.Columns.Add(new DataColumn("ItemID"));
                    itemsTable.Columns.Add(new DataColumn("ParentItemID"));
                    itemsTable.Columns.Add(new DataColumn("RepoItem"));         // RI jec 16/06/11
                    itemsTable.Columns.Add(new DataColumn("WarrantyType"));      //#17883   // #16607
                    itemsTable.Columns.Add(new DataColumn("DateDelivered"));    //#16607    
                    itemsTable.Columns.Add(new DataColumn("ProductCategory"));  //#16607

                    key[0] = itemsTable.Columns["ItemID"];
                    key[1] = itemsTable.Columns["StockLocation"];
                    key[2] = itemsTable.Columns["ContractNo"];
                    key[3] = itemsTable.Columns["ParentItemID"];

                    itemsTable.PrimaryKey = key;

                    //itemsView.RowFilter = "QuantityOrdered > 0";

                    //Create the table styles for the data grid
                    if (dgLineItems.TableStyles.Count == 0)
                    {
                        dgLineItems.DataSource = itemsView;
                        DataGridTableStyle tabStyle = new DataGridTableStyle();
                        tabStyle.MappingName = itemsTable.TableName;
                        dgLineItems.TableStyles.Add(tabStyle);

                        tabStyle.GridColumnStyles["ProductCode"].Width = 76;
                        tabStyle.GridColumnStyles["ProductCode"].Alignment = HorizontalAlignment.Center;
                        tabStyle.GridColumnStyles["ProductCode"].HeaderText = GetResource("T_PRODCODE");

                        tabStyle.GridColumnStyles["ProductDescription"].Width = 230;
                        tabStyle.GridColumnStyles["ProductDescription"].HeaderText = GetResource("T_PRODDESC");

                        tabStyle.GridColumnStyles["StockLocation"].Width = 70;
                        tabStyle.GridColumnStyles["StockLocation"].HeaderText = GetResource("T_STOCKLOCN");

                        //IP - 09/02/10 - CR1049 Merged - Malaysia Enhancements (CR1072)
                        tabStyle.GridColumnStyles["DelAddress"].Width = 65;
                        tabStyle.GridColumnStyles["DelAddress"].HeaderText = GetResource("T_DELADDRESS");
                        tabStyle.GridColumnStyles["DelAddress"].Alignment = HorizontalAlignment.Center;

                        tabStyle.GridColumnStyles["QuantityOrdered"].Width = 70;
                        tabStyle.GridColumnStyles["QuantityOrdered"].HeaderText = GetResource("T_QUANTITY");
                        tabStyle.GridColumnStyles["QuantityOrdered"].Alignment = HorizontalAlignment.Right;

                        tabStyle.GridColumnStyles["UnitPrice"].Width = 70;
                        tabStyle.GridColumnStyles["UnitPrice"].Alignment = HorizontalAlignment.Right;
                        tabStyle.GridColumnStyles["UnitPrice"].HeaderText = GetResource("T_UNITPRICE");
                        ((DataGridTextBoxColumn)tabStyle.GridColumnStyles["UnitPrice"]).Format = DecimalPlaces;

                        tabStyle.GridColumnStyles["Value"].Width = 70;
                        tabStyle.GridColumnStyles["Value"].Alignment = HorizontalAlignment.Right;
                        tabStyle.GridColumnStyles["Value"].HeaderText = GetResource("T_VALUE");
                        ((DataGridTextBoxColumn)tabStyle.GridColumnStyles["Value"]).Format = DecimalPlaces;

                        tabStyle.GridColumnStyles["ContractNo"].Width = 90;
                        tabStyle.GridColumnStyles["ContractNo"].HeaderText = GetResource("T_CONTRACTNO");

                        tabStyle.GridColumnStyles[CN.ItemId].Width = 0;             //IP - 09/06/11 - CR1212 - RI
                        tabStyle.GridColumnStyles[CN.ParentItemId].Width = 0;       //IP - 09/06/11 - CR1212 - RI
                        tabStyle.GridColumnStyles[CN.RepoItem].Width = 0;            //RI jec 16/06/11
                        tabStyle.GridColumnStyles["WarrantyType"].Width = 0;         // #16607
                        tabStyle.GridColumnStyles["DateDelivered"].Width = 0;        // #16607
                        tabStyle.GridColumnStyles["ProductCategory"].Width = 0;      // #16607

                    }
                }



                #region Get drop down data
                //Get the required static data for the drop down lists
                Function = "BStaticDataManager::GetDropDownData";

                XmlUtilities xml = new XmlUtilities();
                XmlDocument dropDowns = new XmlDocument();
                dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.TermsType, new string[] { Config.CountryCode }));
                if (StaticData.Tables[TN.SourceOfAttraction] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.SourceOfAttraction, null));
                if (StaticData.Tables[TN.BranchNumber] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.BranchNumber, null));
                if ((bool)Country[CountryParameterNames.PaymentMethod])
                {
                    if (StaticData.Tables[TN.MethodOfPayment] == null)
                        dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.MethodOfPayment, null));
                }
                if (StaticData.Tables[TN.Bank] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.Bank, null));
                if (StaticData.Tables[TN.PayMethod] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.PayMethod, new string[] { "FPM", "L" }));
                if (StaticData.Tables[TN.DueDay] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.DueDay, new string[] { "DDR", "L" }));
                if (StaticData.Tables[TN.Discounts] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.Discounts, new string[] { "PCDIS", "L" }));
                if (StaticData.Tables[TN.DiscountLinks] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.DiscountLinks, new string[] { "DIS", "L" }));

                if (StaticData.Tables[TN.InstallationItemCat] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.InstallationItemCat, new string[] { CAT.InstallationItem, "L" }));

                if (StaticData.Tables[TN.ReadyAssistTerms] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.ReadyAssistTerms, new string[] { "RDYAST", "L" }));



                if (dropDowns.DocumentElement.ChildNodes.Count > 0)
                {
                    //DataSet ds = drop.GetDropDownData(dropDowns.DocumentElement, out Error);
                    DataSet ds = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out Error);
                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                    {
                        foreach (DataTable dt in ds.Tables)
                        {
                            if (dt.TableName == TN.TermsType)
                            {
                                DataRow r = dt.NewRow();
                                r["termstype"] = "Terms Types";
                                r["Affinity"] = "X";
                                r[CN.IncludeWarranty] = Convert.ToInt16(0);
                                r[CN.PaymentHoliday] = false;
                                r["accounttype"] = "";
                                dt.Rows.InsertAt(r, 0);
                            }
                            StaticData.Tables[dt.TableName] = dt;
                        }
                    }
                }

                readyAssistTerms = ((DataTable)StaticData.Tables[TN.ReadyAssistTerms]);             //#18604 - CR15594

                DataTable dtPayMethod = ((DataTable)StaticData.Tables[TN.PayMethod]);

                dtPayMethod.DefaultView.RowFilter = DefaultPayFilter;

                if (!(bool)Country[CountryParameterNames.AllowCashAndGoCheques])
                    dtPayMethod.DefaultView.RowFilter += AllowCashAndGoChequesFilter; /* optionally exclude cheques */

                if (((bool)Country[CountryParameterNames.StoreCardEnabled]) != true)
                {
                    dtPayMethod.DefaultView.RowFilter += AdditionalPaymentFilterStoreCard;
                }

                drpPayMethod.DataSource = dtPayMethod.DefaultView;
                drpPayMethod.DisplayMember = CN.CodeDescription;
                drpPayMethod.ValueMember = CN.Code;

                //IP - 11/02/10 - CR1048 (Ref:3.1.2.2/3.1.2.3) Merged - Malaysia Enhancements (CR1072)
                if (drpPayMethod.Items.Count > 0)
                {
                    drpPayMethod.SelectedIndex = 0;
                }
                else
                {
                    drpPayMethod.SelectedIndex = -1;
                }
                //drpPayMethod.SelectedIndex = 0;

                dtTermsTypes = (DataTable)StaticData.Tables[TN.TermsType];
                dvTermsTypes = new DataView(dtTermsTypes);
                //RefreshTermsTypeCmb(ref dvTermsTypes, accountNumber);
                drpTermsType.DataSource = dvTermsTypes;
                drpTermsType.DisplayMember = "termstype";
                drpTermsType.ValueMember = "band";

                drpBank.DataSource = (DataTable)StaticData.Tables[TN.Bank];
                drpBank.DisplayMember = CN.BankName;

                // Delivery Area
                DataSet areaSet = SetDataManager.GetSetsForTNameBranchAll(TN.TNameDeliveryArea, out Error);
                this._areaTable = areaSet.Tables[TN.SetsData];
                delAreas = areaSet.Tables[TN.SetsData];     //CR12249 - #12224
                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    // Init the Delivery Area list to the logged in branch
                    this.SetDeliveryAreaList(Config.BranchCode);
                }

                foreach (DataRow row in ((DataTable)StaticData.Tables[TN.SourceOfAttraction]).Rows)
                {
                    string str = (string)row.ItemArray[0] + " - " + (string)row.ItemArray[1];
                    soa.Add(str.ToUpper());
                }

                if ((bool)Country[CountryParameterNames.PaymentMethod])
                {
                    foreach (DataRow row in ((DataTable)StaticData.Tables[TN.MethodOfPayment]).Rows)
                    {
                        string str = (string)row.ItemArray[0] + " - " + (string)row.ItemArray[1];
                        mop.Add(str.ToUpper());
                    }
                    drpPaymentMethod.DataSource = mop;

                }

                foreach (DataRow row in ((DataTable)StaticData.Tables[TN.BranchNumber]).Rows)
                {
                    branchNos.Add(Convert.ToString(row["branchno"]));

                    // CR903 - only display branches for the logged in store type.
                    if (row[CN.StoreType].ToString() == Config.StoreType)
                    {
                        branchNos2.Add(Convert.ToString(row["branchno"]));
                    }
                }

                //drpTermsType.DataSource = termsTypes;
                drpSOA.DataSource = soa;
                drpBranchForDel.DataSource = branchNos;
                drpBranch.DataSource = branchNos2;

                //CR903 Populate the branches ArrayList with all the Courts branches

                foreach (DataRow row in ((DataTable)StaticData.Tables[TN.BranchNumber]).Rows)
                {
                    if (row[CN.StoreType].ToString() == StoreType.Courts)
                    {
                        branches.Add(row[CN.BranchNo].ToString());
                    }
                }

                //IP - 25/02/10 - CR1072 - Malaysia 3PL for Version 5.2 --Merged from v4.3
                // cr 04/08/08 get list of branch/defaultprintlocation here
                //if (thirdPartyDeliveriesEnabled)
                //{
                if (_printBranches == null)                  //IP - 25/05/12 - #10225 - Warehouse & Deliveries Integration
                {
                    _printBranches = new List<STL.PL.WS2.BranchDefaultPrintLocation>();
                    STL.PL.WS2.BranchDefaultPrintLocation[] printBranchList = AccountManager.GetBranchDefaultPrintLocation();
                    _printBranches.AddRange(printBranchList);
                }
                //}

                int x = drpBranch.FindString(Config.BranchCode);
                if (x != -1)
                    drpBranch.SelectedIndex = x;

                // drpTermsType.DisplayMember = "termstype";

                int index = drpBranchForDel.FindString(Convert.ToString(Country[CountryParameterNames.DefaultDeliveryNoteBranch]));
                //IP/JC - 02/03/10 - CR1072 - Malaysia 3PL 
                //if (index != -1)
                //    drpBranchForDel.SelectedIndex = index;

                if (index != -1)
                    drpBranchForDel.SelectedIndex = index;
                else
                {
                    index = drpBranchForDel.FindString(Config.BranchCode);
                    drpBranchForDel.SelectedIndex = index;
                }

                DataTable dtDueDay = (DataTable)StaticData.Tables[TN.DueDay];
                if (dtDueDay.Rows.Count > 0)
                {
                    foreach (DataRow row in dtDueDay.Rows)
                    {
                        string str = (string)row[CN.Code];
                        dueDays.Add(str);
                    }
                    drpDueDay.DataSource = dueDays;
                }
                else
                {
                    int i = 0;
                    for (i = 1; i < 31; i++)
                        dueDays.AddRange(new string[] { i.ToString() });

                    drpDueDay.DataSource = dueDays;
                }

                RefreshBranchSpecificData(Config.BranchCode);

                staticLoaded = true;

                if (revision)
                {
                    /* possible that we are not t the paid and taken account
					 * for the currently logged in branch (for returns) therefore
					 * get the branchno from the acctno */
                    //string tmp = AccountManager.GetPaidAndTakenAccount(Config.BranchCode, out Error);
                    string tmp = AccountManager.GetPaidAndTakenAccount(_acctNo.Substring(0, 3), out Error);
                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                        PaidAndTaken = _acctNo == tmp;

                    if (!PaidAndTaken)
                    {
                        drpBranchForDel.Visible = lBranchForDel.Visible = (bool)Country[CountryParameterNames.DisplayDelNoteBranch];
                        string termsType = AccountManager.IsPaidAndTakenWarranty(_acctNo, out Error);
                        PaidAndTaken = (TT.IsPaidAndTaken(termsType) && _acctNo.Substring(3, 1) == "5");
                    }

                    // uat479 rdb 15/07/08 moved from top of procedure as they get superseded
                    // when PaidAndTaken property is set
                    cbAssembly.Visible = lAssembly.Visible = (bool)Country[CountryParameterNames.DisplayAssembly];
                    cbExpress.Visible = lblExpress.Visible = Convert.ToBoolean(Country[CountryParameterNames.DisplayExpressDelivery]);       //#12232 - CR12249

                    //drpBranchForDel.Visible = lBranchForDel.Visible = (bool)Country[CountryParameterNames.DisplayDelNoteBranch];

                    this.AccountLoaded = loadAccountData(_acctNo, true);

                }
                #endregion

                // CR586 Init the list of payments for Cash & Go
                InitPaymentSet();

                //perform country specific variations
                Localise();
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void LoadCustomerAddresses(string custID)
        {
            //HACK (could you pls point me to something that is not a hack in this project?) the idea was to only give the option or current 
            //addresses for the delivery address drop down
            //this doesn't work however because we probably won't know the 
            //customer at this stage - hard code options in the IDE for now

            /*
			WCustomerManager cust = new WCustomerManager();
			DataSet ds = cust.GetCustomerAddresses(custID, out Error);
			if(Error.Length>0)
				ShowError(Error);
			else
			{
				foreach(DataTable tb in ds.Tables)
				{
					if(tb.TableName==TN.CustomerAddresses)
					{
						drpDeliveryAddress.DataSource = tb;
						drpDeliveryAddress.DisplayMember = CN.AddressType;
					}
				}
			}
			*/
        }

        public bool loadAccountData(string accountNumber, bool needsLocking)
        {
            Function = "loadAccountData()";
            this._loadAccount = true;
            string acctType = "";
            bool status = true;

            /* the needsLocking parameter is for when the loadAccountData method is used
			 * to refresh the screen and therefore the account is already locked */
            if (needsLocking && !PaidAndTaken)
            {
                this.AccountLocked = false;
                this.AccountLocked = AccountManager.LockAccount(accountNumber, Credential.UserId.ToString(), true, out Error);
                if (Error.Length > 0)
                {
                    ShowError(Error);
                    status = false;
                }
            }

            if (PaidAndTaken)
                AccountLocked = true;

            if (status)
            {

                DataSet ds = AccountManager.GetAccountForRevision(accountNumber, AgreementNo, out _scoringBand, out _currentBand, out Error);

                //#10535
                if (ds.Tables.Count > 1 && calledFromScreen != "FailedDeliveriesCollections")
                {
                    LineItemBooking = ds.Tables["LineItemBooking"];
                    var exception = false;

                    foreach (DataRow dr in LineItemBooking.Rows)
                    {
                        if (Convert.ToInt32(dr[CN.FailureBookingId]) > 0 && (Convert.ToString(dr[CN.Actioned]) == string.Empty || Convert.ToString(dr[CN.Actioned]) == null))
                        {
                            exception = true;
                            break;
                        }
                    }

                    if (exception)
                    {
                        ShowInfo("M_EXCEPTIONONITEM", MessageBoxButtons.OK);
                    }
                }



                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    txtAccountNumber.ReadOnly = true;
                    menuStock.Enabled = true;
                    // revision = true;  fix unrelated issue 23 (jec 29/06/06)

                    /* parse the branch no from the acctno and set the drop down to readonly */
                    /* unless it's P&T in which case use the Config branch */
                    if (PaidAndTaken)
                    {
                        drpBranch.SelectedIndex = drpBranch.FindString(Config.BranchCode);
                    }
                    else
                    {
                        string branch = accountNumber.Substring(0, 3);
                        int i = drpBranch.FindString(branch);
                        if (i != -1)
                            drpBranch.SelectedIndex = i;

                    }
                    //Find the store type of the selected branch
                    SType = FindStoreType(accountNumber);

                    drpBranch.Enabled = Credential.HasPermission(CosacsPermissionEnum.NewSalesChangeBranch);

                    //Set all the screen controls to reflect the details returned
                    foreach (DataTable dt in ds.Tables)
                    {
                        if (dt.TableName == TN.AccountDetails)
                        {
                            foreach (DataRow row in dt.Rows)
                            {
                                /*
								PaidAndTaken = Convert.ToBoolean(row[CN.PaidAndTaken]);
								if(PaidAndTaken)
								{
									//Paid and taken account should not be locked
									AccountManager.UnlockAccount(accountNumber, Credential.User, out Error);
									if(Error.Length>0)
										ShowError(Error);
								}
								*/

                                /* make sure terms type cannot be changed if outstbal != 0 && currstatus != 0 */
                                OutstandingBalance = (decimal)row[CN.OutstBal];
                                string currentStatus = (string)row[CN.AccountStatus];

                                if (currentStatus == "S" && (string)row["accttype"] != "S") //only show revise message if not cash and go account
                                {
                                    ShowInfo("M_REVISINGSETTLED");
                                }

                                if (!changeTermsType.Enabled)
                                    drpTermsType.Enabled = (Math.Abs(OutstandingBalance) < (decimal)0.01 || currentStatus == "0");

                                groupBox2.Enabled = true;
                                gbHP.Enabled = true;
                                menuSave.Enabled = true && !PaidAndTaken;

                                this.CustomerID = (string)row[CN.CustomerID];
                                ptCustomerID = this.CustomerID;     //IP - 15/04/08 - setting variable to Paid & Taken CustomerID
                                this.LoadCustomerAddresses(this.CustomerID);

                                DateAccountOpened = (DateTime)row["dateacctopen"];

                                _origTerms = (string)row["termstype"];
                                TermsType = (string)row["termstype"];
                                WarrantiesOnCredit = (TermsType == TT.WarrantyOnCredit);
                                this.SetCustomerDetails();
                                //this.SetupPrivilegeClub(this.CustomerID);

                                // Account Type - must be set before Terms Type because
                                // Terms Types are now filtered on the Account Type
                                acctType = (string)row["accttype"];
                                int index = drpAccountType.FindString(acctType);
                                if (index == -1)
                                {
                                    StringCollection s = new StringCollection();
                                    s.AddRange(new string[] { "Type", acctType });
                                    drpAccountType.DataSource = s;
                                    index = drpAccountType.FindString(acctType);
                                }

                                drpAccountType.SelectedIndex = index;
                                drpAccountType.Enabled = false;

                                this.isStaff = Convert.ToBoolean(row["IsStaff"]);

                                // Filter the terms types for the band, account type and for affinity
                                //this._currentBand = row[CN.CurrentBand] == DBNull.Value ? "" : (string)row[CN.CurrentBand]; -- Brought back already above
                                //this._scoringBand = row[CN.ScoringBand] == DBNull.Value ? "" : (string)row[CN.ScoringBand]; -- Brought back already
                                this._origScoringBand = this._scoringBand; //row[CN.ScoringBand] == DBNull.Value ? "" : (string)row[CN.ScoringBand];
                                this._scorecardtype = (string)row[CN.ScoreCardType];
                                DataRowView termsTypeRow = (DataRowView)drpTermsType.SelectedItem;
                                this._affinityTerms = ((string)termsTypeRow["Affinity"] == "Y");
                                //FilterTermsType(ref drpTermsType, _affinityTerms, drpAccountType.Text, _scoringBand, SType, IsLoan);

                                //CR906 rdb apply  if this account is a cash loan or not
                                this.IsLoan = row["isLoan"] == DBNull.Value ? false : Convert.ToBoolean(row["isLoan"]);
                                DataView dvTermsType = (DataView)drpTermsType.DataSource;

                                if (_scoringBand == String.Empty)
                                {
                                    //If the scoring band is empty then use that for the selected terms type from the data view
                                    // int bandRow = 0;
                                    string band = String.Empty;
                                    foreach (DataRowView drv in dvTermsType)
                                    {
                                        if (drv["termstypecode"].ToString() == row["termstype"].ToString())
                                        {
                                            band = drv["band"].ToString();
                                            break;
                                        }
                                    }
                                    _scoringBand = band;
                                }


                                RefreshTermsTypeCmb(accountNumber);

                                // Get the current setting
                                //string curTermsType = drpTermsType.Text;

                                //FilterTermsType(ref dvTermsType, _affinityTerms, drpAccountType.Text, _scoringBand, SType, IsLoan);

                                //// Make sure the TermsType has not changed if it is still available
                                //int selectedTermsType = drpTermsType.FindStringExact(curTermsType);
                                //drpTermsType.SelectedIndex = (selectedTermsType != -1) ? selectedTermsType : 0;


                                // 5.1 uat89 rdb 26/11/08  i dont want to break other code 
                                // so will try to set FilterTermsType for priviledge club here
                                // i will need to set the index to the value set here
                                // otherwise use original code
                                // todo rdb 10/11/07 add check in this method to look at intratehistory 
                                // and check if TIR1 or TIR2 rate existed when agreement created
                                if (this.SetupPrivilegeClub(this.CustomerID))
                                {
                                    index = drpTermsType.SelectedIndex;
                                }
                                else
                                {
                                    index = drpTermsType.FindString((string)row["termstype"]);
                                }

                                // IP - 24/09/10 - UAT5.4 - UAT(34) - Code previously was commented out from merge incorrectly. Re-instated 
                                /* if (index == -1)
								 {
									
									 terms type may not be in the drop down if it became 
									  * inactive since the account was created - in this case it
									  * should be added manually 
									 DataSet tt = AccountManager.GetTermsTypeDetails(Config.CountryCode, (string)row["termstype"], accountNumber, DateAccountOpened, out Error);
									 if (Error.Length > 0)
										 ShowError(Error);
									 else
									 {
										 // For some reason this account terms type is not appropriate for this account
										 foreach (DataTable tab in tt.Tables)
										 {
											 foreach (DataRow r in tab.Rows)
											 {
												 // A bit dodgy - this adds the terms type to the filtered
												 // terms type list, regardless of whether the definition
												 // of this terms type says it is appropriate for this acct.
												 DataRow newTT = dtTermsTypes.NewRow();
												 newTT["termstype"] = (string)row["termstype"] + " - " + Convert.ToString(r["description"]);
												 newTT["Affinity"] = r["Affinity"];
												 newTT["IncludeWarranty"] = r["IncludeWarranty"];
												 newTT[CN.PaymentHoliday] = r[CN.PaymentHoliday];
												 newTT[CN.AccountType] = drpAccountType.Text;
												 if (_currentBand != String.Empty)
													 newTT[CN.Band] = _scoringBand;
												 else
													 newTT[CN.Band] = (string)row[CN.ScoringBand];
												 newTT[CN.StoreType] = SType;
												 newTT[CN.IsLoan] = IsLoan;
												 dtTermsTypes.Rows.Add(newTT);
												 dvTermsTypes = new DataView(dtTermsTypes);
												 RefreshTermsTypeCmb(accountNumber);
												 drpTermsType.DataSource = dvTermsTypes;
												 drpTermsType.DisplayMember = "termstype";
											 }
										 }
									 }
								 }

								 // SC CR1043 Dodgy indeed. Will load terms types later at end or something as equally dodgy.
									 
								 */



                                // Set the terms type drop down
                                index = drpTermsType.FindString((string)row["termstype"]);
                                if (index != -1)
                                    drpTermsType.SelectedIndex = index;

                                if ((string)row["codflag"] == "Y")
                                {
                                    chxCOD.Checked = true;
                                }

                                if (AccountType != AT.Cash && AccountType != AT.Special && AccountType != AT.ReadyFinance && AccountType != AT.GoodsOnLoan //Acct Type Translation DSR 29/9/03
                                    && !Convert.ToBoolean(row[CN.Stage1Complete]))
                                {
                                    menuPrint.Enabled = false;
                                    btnPrint.Enabled = false;
                                    menuReprint.Enabled = false;
                                }

                                if (row["soa"] != DBNull.Value) // UAT 95  SC 16/04/10
                                {
                                    index = drpSOA.FindString((string)row["soa"]);
                                }
                                else
                                {
                                    index = -1;
                                }

                                if (index != -1)
                                    drpSOA.SelectedIndex = index;

                                if (row["paymethod"] != DBNull.Value) // UAT 95  SC 16/04/10
                                {
                                    index = drpPaymentMethod.FindString((string)row["paymethod"]);
                                }
                                else
                                {
                                    index = -1;
                                }
                                if (index != -1)
                                    drpPaymentMethod.SelectedIndex = index;

                                txtTerms.Text = ((decimal)row["servicechg"]).ToString(DecimalPlaces);
                                txtDeposit.Text = ((decimal)row["deposit"]).ToString(DecimalPlaces);
                                this._deposit = Convert.ToDecimal(row["deposit"]);
                                txtDeposit.Enabled = true;
                                //CR-2018-13 – Raj – 05 Dec 18 –  To show AgreementInvoiceNumbe
                                // lblordvalue.Text  = row["AgreementInvoiceNumber"].ToString(); 
                                lblAgrInvoieNum.Text = string.IsNullOrEmpty(row["AgreementInvoiceNumber"].ToString()) ? "--" : row["AgreementInvoiceNumber"].ToString();
                                lblCHOrdInvoice.Text = "Ord/Invoice: " + lblAgrInvoieNum.Text;
                                //
                                decimal months = 0;
                                if (!Convert.IsDBNull(row["instalno"]))
                                    months = Convert.ToDecimal(row["instalno"]);

                                // Don't change the deposit when opening for revision
                                // Use no of months or lengths
                                errorProvider1.SetError(drpTermsType, "");
                                SetTermsType(drpTermsType, numPaymentHolidays, cbDeposit, txtDeposit,
                                    txtNoMonths, drpLengths, _loadAccount,
                                    Convert.ToDecimal(StripCurrency(txtSubTotal.Text)),
                                    true, ref _defaultDeposit, ref _depositIsPercentage,
                                     ref this._deposit, months, CheckDepositWaiver());

                                if (AccountType == AT.ReadyFinance && Convert.ToString(row["deliveryflag"]) == "Y")
                                {
                                    txtPrefDay.Enabled = false;
                                }
                                else
                                {
                                    txtPrefDay.Enabled = true;
                                }
                                txtPrefDay.Text = Convert.ToString(row["PrefInstalmentDay"]); //Setting PrefDay Value on form



                                if (!Convert.IsDBNull(row["instalamount"]))
                                {
                                    txtInstalment.Text = ((decimal)row["instalamount"]).ToString(DecimalPlaces);
                                }
                                if (!Convert.IsDBNull(row["fininstalamt"]))
                                {
                                    txtFinalInstalment.Text = ((decimal)row["fininstalamt"]).ToString(DecimalPlaces);
                                }

                                instalmentWaived = Convert.ToBoolean(row["InstalmentWaived"]);
                                chxDutyFree.Checked = Convert.ToBoolean(row["dutyfree"]);
                                chxTaxExempt.Checked = Convert.ToBoolean(row["taxexempt"]);

                                if (AT.IsCashType((string)drpAccountType.SelectedItem))
                                //if((string)drpAccountType.SelectedItem==AT.Cash ||
                                //	(string)drpAccountType.SelectedItem==AT.Special)
                                {
                                    if (PaidAndTaken)
                                    {
                                        gbPaidAndTaken.BringToFront();
                                        gbPaidAndTaken.Visible = true;
                                        gbHP.Visible = false;
                                    }
                                    else
                                    {
                                        Rectangle rect1 = new Rectangle(groupBox2.Location.X, groupBox2.Location.Y, groupBox2.Size.Width, 400);
                                        Rectangle rect2 = new Rectangle(panel1.Location.X, panel1.Location.Y, panel1.Size.Width, 200);
                                        gbHP.Visible = false;
                                        groupBox2.Bounds = rect1;
                                        panel1.Bounds = rect2;
                                        chxCOD.Enabled = true;
                                        label26.Text = "Total:";


                                    }
                                }

                                //IP - 28/11/2007 - (69360)
                                bool hasSchedule = false;
                                //If Cash account and there are schedule records then 
                                //disable the 'COD' flag.
                                if (AccountType == AT.Cash)
                                {
                                    hasSchedule = AccountManager.AccountScheduleExists(accountNumber, out Error);

                                    if (hasSchedule == true)
                                    {
                                        chxCOD.Enabled = false;
                                    }
                                }

                                if (!Convert.IsDBNull(row["deliveryflag"]))
                                    DeliveryFlag = (string)row["deliveryflag"];

                                //Function = "BAccountManager::GetLineItems()";

                                if (((!PaidAndTaken || AgreementNo != 1) && instantReplacement == null) ||      /* if it's a collection pre-populate the orignal items */
                                    (AgreementNo == 1 && instantReplacement != null))
                                {
                                    XmlNode lineItems = AccountManager.GetLineItems(accountNumber, AgreementNo, acctType, Config.CountryCode, Convert.ToInt16(Config.BranchCode), out Error);
                                    if (Error.Length > 0)
                                        ShowError(Error);
                                    else
                                    {
                                        if (lineItems != null)
                                        {
                                            //Check for null values
                                            if (row["instantcredit"] == System.DBNull.Value)
                                            {
                                                row["instantcredit"] = String.Empty;
                                            }
                                            // 5.0.5.0 UAT Issue 267 - Only rescore HP account if
                                            // stock items exist on the account.
                                            ReScore = (lineItems.SelectNodes("//Item[@Type='Stock' and @Quantity != '0']").Count > 0);
                                            //&& ((string)row["instantcredit"]).Equals("N")); //IP - 15/03/11 - Do not need to check for instant credit

                                            lineItems = itemDoc.ImportNode(lineItems, true);
                                            itemDoc.ReplaceChild(lineItems, itemDoc.DocumentElement);
                                            populateTable();

                                        }
                                    }
                                }

                                // Caribbean 121 - allow changing of employee number on all accounts 
                                // unless there are stockitems ordered	
                                if (!PaidAndTaken || AgreementNo != 1)
                                {
                                    string empee = Convert.ToString(row["empeenosale"]) + " : " + (string)row[CN.EmployeeName];

                                    if (drpSoldBy.FindString(empee) != -1)
                                        drpSoldBy.SelectedIndex = drpSoldBy.FindString(empee);
                                    else
                                    {       /* if empee not found add it to the string collection */
                                        StringCollection s = (StringCollection)drpSoldBy.DataSource;
                                        s.Add(empee);
                                        drpSoldBy.DataSource = null;
                                        drpSoldBy.DataSource = s;
                                        drpSoldBy.SelectedIndex = drpSoldBy.FindString(empee);
                                    }

                                    if (stockExists)
                                    {
                                        drpSoldBy.Enabled = false;
                                        txtEmpNumber.Enabled = false;
                                        //drpSoldBy.DataSource = new string[]{empee};
                                    }

                                    dtDateFirst.Value = Convert.ToDateTime(row[CN.DateFirst]);

                                    if (revision && (string)row[CN.AcctType] == AT.ReadyFinance && !stockExists)
                                    {
                                        decimal dueDay = AccountManager.GetDueDay(CustomerID, out Error);
                                        index = drpDueDay.FindString(dueDay.ToString());
                                        if (index != -1)
                                            drpDueDay.SelectedIndex = index;
                                    }
                                    else
                                    {
                                        index = drpDueDay.FindString(Convert.ToString(row[CN.DueDay]));
                                        if (index != -1)
                                            drpDueDay.SelectedIndex = index;
                                    }
                                }

                                //if it was an RF account make sure it's been approved
                                //and the credit limit > 0
                                if ((string)row["accttype"] == AT.ReadyFinance)
                                {
                                    if (row[CN.RFCreditLimit] != DBNull.Value)
                                    {
                                        this.RFCreditLimit = (decimal)row[CN.RFCreditLimit];
                                        this.RFAvailableCredit = (decimal)row[CN.RFAvailable];
                                        //if existing account then subtotal already included available credit total
                                        if (row["agrmttotal"].ToString() != "0")
                                            // CR906 11/09/07 remove loanTotal from nonStock when calculating max spend
                                            this.RFMax = (Convert.ToDecimal(StripCurrency(txtSubTotal.Text)) + RFAvailableCredit) - nonStockTotal;
                                        else
                                            this.RFMax = RFAvailableCredit;

                                        // CR906 if this is a loan account and is new we need to add a loan item
                                        if (this.IsLoan && itemsTable.Rows.Count == 0)
                                        {
                                            // add loan item
                                            this.ProductCode = "LOAN";
                                            this.ItemID = StockItemCache.Get(StockItemKeys.LOAN);
                                            this.Location = Config.BranchCode;
                                            drpLocationValidation();
                                            txtValue.Focus();
                                        }
                                    }
                                    if (Convert.ToBoolean(row[CN.RFBlock]))
                                    {
                                        menuSave.Enabled = false;
                                        groupBox2.Enabled = false;
                                        gbHP.Enabled = false;
                                        ShowInfo("M_RFBLOCKED");
                                    }
                                    txtRFMax.Visible = true;
                                    lblMaxSpend.Visible = true;

                                    txtInsuranceCharge.Location = new Point(txtInsuranceCharge.Location.X - 94, txtInsuranceCharge.Location.Y);
                                    txtAdminCharge.Location = new Point(txtAdminCharge.Location.X - 94, txtAdminCharge.Location.Y);
                                    lInsuranceCharge.Location = new Point(lInsuranceCharge.Location.X - 94, lInsuranceCharge.Location.Y);
                                    lAdminCharge.Location = new Point(lAdminCharge.Location.X - 94, lAdminCharge.Location.Y);
                                }
                                numPaymentHolidays.Value = Convert.ToDecimal(row[CN.PaymentHoliday]);
                            }
                        }
                    }
                    /* this doesn't really serve any purpose - JJ
					if(ds.Tables.Count==0 || ds.Tables[0].Rows.Count==0)
					{
						ShowInfo("M_NOACCOUNTDATA");	
						status = false;
					}
					*/
                }
            }
            // } -- merge error I think.

            if (acctType == AT.ReadyFinance || acctType == AT.HP)
            {
                // string err;
                // customerband = CustomerManager.CustomerGetBand(accountNumber, out err);
                //   dvTermsTypes.RowFilter = "band = '" + customerband.Trim() + "'";

                drpTermsType.DataSource = dvTermsTypes;
                RefreshTermsTypeCmb(accountNumber);
                drpTermsType.DisplayMember = "termstype";
                drpTermsType.ValueMember = "band";
            }
            LoadVoucher();

            this._loadAccount = false;
            return status;
        }


        private void RefreshTermsTypeCmb(string accountNumber)
        {

            DataView dv = new DataView((DataTable)StaticData.Tables[TN.TermsType]);
            string band = "";
            string storetype = "";
            string termstype = "";


            DataSet tt = AccountManager.TermsTypeGetDetails(accountNumber, out Error);
            if (Error.Length > 0)
                ShowError(Error);

            //// Get the current setting
            if (tt.Tables[0].Rows.Count != 0)
            {
                if (_currentBand != String.Empty)
                    band = _currentBand;
                else
                    band = tt.Tables[0].Rows[0][CN.ScoringBand].ToString();

                storetype = tt.Tables[0].Rows[0][CN.StoreType].ToString();
                termstype = tt.Tables[0].Rows[0][CN.TermsType].ToString();

            }

            FilterTermsType(ref dv, _affinityTerms, drpAccountType.Text, band, storetype, _isLoan);

            bool found = false;
            //Add old termstype to list, if not in list.

            foreach (DataRowView rv in dv)
            {
                if (rv["termstype"].ToString() == termstype && rv["band"].ToString() == band)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                dv.RowFilter = dv.RowFilter + " OR (termstype = '" + termstype + "' AND band = '" + band + "' AND (" + CN.StoreType + " = '" + storetype + "' OR " + CN.StoreType + " = 'A') AND  AccountType = '" + drpAccountType.Text + "')";
            }

            drpTermsType.DataSource = dv;
            // Make sure the TermsType has not changed if it is still available
            int selectedTermsType = drpTermsType.FindStringExact(termstype);
            if (selectedTermsType == -1)
            {
                if (tt.Tables[0] != null && tt.Tables[0].Rows.Count != 0)
                {
                    DataRow r = tt.Tables[0].Rows[0];
                    DataRow newTT = ((DataTable)StaticData.Tables[TN.TermsType]).NewRow();
                    newTT["termstype"] = (string)r["termstype"];
                    newTT["Affinity"] = r["Affinity"];
                    newTT["IncludeWarranty"] = r["IncludeWarranty"];  //IncludeWarranty
                    newTT[CN.PaymentHoliday] = r[CN.PaymentHoliday];
                    newTT[CN.AccountType] = drpAccountType.Text;
                    if (_currentBand != String.Empty)
                        newTT[CN.Band] = _currentBand;
                    else
                        newTT[CN.Band] = (string)r[CN.ScoringBand];
                    newTT[CN.StoreType] = storetype;

                    newTT[CN.IsLoan] = IsLoan;
                    // dv.Table.Rows.Clear();
                    // dv.RowFilter = "";
                    dv.Table.Rows.Add(newTT);

                    drpTermsType.DataSource = dv;
                    // Make sure the TermsType has not changed if it is still available
                    selectedTermsType = drpTermsType.FindStringExact(termstype);
                }
            }

            if (drpTermsType.Items.Count != 0)
            {
                drpTermsType.SelectedIndex = (selectedTermsType != -1) ? selectedTermsType : 0;
            }
        }

        private void txtAgreementTotal_TextChanged(object sender, System.EventArgs e)
        {
            //if (revision)
            //{
            //    //anything that causes the agreement total to change 
            //    //should mean that the account has to be re-sanctioned
            //}
        }

        private void menuCustomerSearch_Click(object sender, System.EventArgs e)
        {
            //IP - 14/03/08 - (69630)
            //If this is a Paid & Taken, and has not been previously saved
            //save the Paid & Taken. 
            //Once a Customer has been linked to the Paid & Taken save the Paid & Taken 
            //a second time where the fintrans and delivery transactions are then processed.
            if (PaidAndTaken)
            {
                bool result = true;
                if (!_cashAndGoSaved || CustomerID.Length > 0)
                {
                    result = SaveAccount();
                    _cashAndGoSaved = result;
                }
                if (result || _hasdatachanged == false)
                {
                    CustomerSearch cust = new CustomerSearch(AccountNo, (string)drpAccountType.SelectedItem, false);
                    cust.FormRoot = this.FormRoot;
                    cust.FormParent = this;
                    ((MainForm)this.FormRoot).AddTabPage(cust, 9);
                    cust.CustidSelected += new RecordIDHandler<StoreCardCustDetails>(cust_CustidSelected);
                }
            }
            else if (SaveAccount() || _hasdatachanged == false) // //#16237
            {
                CustomerSearch cust = new CustomerSearch(AccountNo, (string)drpAccountType.SelectedItem, false);
                cust.FormRoot = this.FormRoot;
                cust.FormParent = this;
                ((MainForm)this.FormRoot).AddTabPage(cust, 9);
            }

            custidrequested = true;
        }

        void cust_CustidSelected(object sender, RecordIDEventArgs<StoreCardCustDetails> args)
        {
            custidCG = args.RecordID.Custid;
        }

        private void txtProductCode_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            RichTextBox ctl = (RichTextBox)sender;

            MenuCommand m1 = new MenuCommand(GetResource("P_ENQUIREBY"));
            MenuCommand m2 = new MenuCommand(GetResource("P_LOCATION"));
            MenuCommand m3 = new MenuCommand(GetResource("P_PRODUCT"));
            m2.Click += new System.EventHandler(this.menuItem14_Click);
            m3.Click += new System.EventHandler(this.menuItem15_Click);
            m1.MenuCommands.AddRange(new MenuCommand[] { m2, m3 });

            PopupMenu popup = new PopupMenu();
            popup.Animate = Animate.Yes;
            popup.AnimateStyle = Animation.SlideHorVerPositive;

            popup.MenuCommands.Add(m1);

            MenuCommand selected = popup.TrackPopup(ctl.PointToScreen(new Point(e.X, e.Y)));
        }

        private void txtAccountNumber_Leave(object sender, System.EventArgs e)
        {
            try
            {
                int agreementNo = 0;
                DataSet payMethodSet = new DataSet();

                if (!txtAccountNumber.ReadOnly)
                {
                    //must validate the account number entered if this is a manual sale
                    if (ManualSale)
                    {
                        AccountManager.ValidateAccountNumber(AccountNo, Config.CountryCode, AccountType, out Error);
                        if (Error.Length > 0)
                        {
                            errorProvider1.SetError(txtAccountNumber, Error);
                            txtAccountNumber.Text = "000-0000-0000-0";
                            txtAccountNumber.Select(0, txtAccountNumber.Text.Length);
                        }
                        else    /* accountno is valid */
                        {
                            /* lock the account */
                            AccountLocked = false;
                            AccountLocked = AccountManager.LockAccount(AccountNo, Credential.UserId.ToString(), false, out Error);
                            if (Error.Length > 0)
                            {
                                errorProvider1.SetError(txtAccountNumber, Error);
                                txtAccountNumber.Text = "000-0000-0000-0";
                                txtAccountNumber.Select(0, txtAccountNumber.Text.Length);
                            }
                            else
                            {
                                errorProvider1.SetError(txtAccountNumber, "");
                                txtAccountNumber.ReadOnly = true;

                                groupBox2.Enabled = true;
                                gbHP.Enabled = true;

                                SupressEvents = true;
                                int x = drpBranch.FindString(AccountNo.Substring(0, 3));
                                if (x != -1)
                                    drpBranch.SelectedIndex = x;

                                //CR903 Find the account branch's store type
                                SType = FindStoreType(txtAccountNumber.Text);
                                SupressEvents = false;

                                if (AccountType == AT.ReadyFinance)
                                {
                                    string dummy = "";
                                    string bureauFailure = "";
                                    DateTime dateProp = this._today;
                                    // todo isLoan
                                    string termsType = AccountManager.GetDefaultTermsType(false, out Error);
                                    if (Error.Length > 0)
                                        ShowError(Error);
                                    else
                                    {
                                        AccountManager.SaveNewAccount(new STL.PL.WS2.SaveNewAccountParameters
                                        {
                                            AccountNumber = AccountNo,
                                            BranchNo = Convert.ToInt16((string)drpBranch.SelectedItem),
                                            AccountType = AccountType,
                                            CODFlag = "N",
                                            SalesPerson = Credential.UserId,
                                            SOA = "",
                                            AgreementTotal = Convert.ToDouble(StripCurrency(txtSubTotal.Text)),
                                            LineItems = itemDoc.DocumentElement,
                                            TermsType = termsType,
                                            NewBand = "",
                                            PaymentMethod = "",               // #8269 jec 22/09/11
                                            CountryCode = Config.CountryCode,
                                            DateFirst = DateTime.MinValue.AddYears(1899),
                                            DtTaxAmount = dtTaxAmount,
                                            LoyaltyCardNo = txtLoyaltyCardNo.Text,
                                            ReScore = ReScore,
                                            GiftVoucherValue = GiftVoucherValue,
                                            CourtsVoucher = true,
                                            AccountNoCompany = VoucherCompanyAcctNo,
                                            PromoBranch = Convert.ToInt16(Config.BranchCode),
                                            PaymentHolidays = Convert.ToInt16(numPaymentHolidays.Value),
                                            PayMethodSet = payMethodSet,
                                            VariableRatesSet = variableRatesSet,
                                            Autoda = AutoDA,
                                            User = Credential.UserId
                                        },"",0,0,
                                            ref agreementNo, ref dummy, ref dateProp, out bureauFailure, out storeCardTransRefNo, out referralReasons, out Error); //IP - 15/03/11 - #3314 - CR1245 - Return referral reasons

                                        //IP - 23/03/11 - CR1245 - Displaying referral reasons on the referral/rejection popup based on Country Parameter.
                                        if (Convert.ToBoolean(Country[CountryParameterNames.ReasonsReferPopup]) == false)
                                        {
                                            referralReasons = string.Empty;
                                        }

                                        if (Error.Length > 0)
                                            ShowError(Error);
                                        else
                                        {
                                            if (bureauFailure.Length > 0)
                                                ShowWarning(bureauFailure);
                                            ManualAccountKeepLock = true;
                                            ShowInfo("M_CUSTOMERSEARCH");
                                            CustomerSearch cust = new CustomerSearch(txtAccountNumber.Text, AccountType, false);
                                            cust.FormRoot = this.FormRoot;
                                            cust.FormParent = this;
                                            ((MainForm)this.FormRoot).AddTabPage(cust, 9);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }

        }

        private void dgLineItems_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            int index = dgLineItems.CurrentRowIndex;

            if (index < 0)  //may be empty
                return;

            // FA 27/11/09 - UAT 509 Deselect everything before selecting only one row
            for (int i = 0; i < this.dgLineItems.VisibleRowCount; i++)
            {
                this.dgLineItems.UnSelect(i);
            }

            dgLineItems.Select(dgLineItems.CurrentCell.RowNumber);

            DataView dv = (DataView)dgLineItems.DataSource;
            //uat363 rdb add parentProductCode to XPath if neccesary

            string xPath = "//Item[@Code='" + (string)dv[index]["ProductCode"] +
                    "' and @Location='" + Convert.ToString(dv[index]["StockLocation"]) +
                    "' and @ContractNumber='" + (string)dv[index][CN.ContractNo] +
                    "' and @RepoItem='" + (string)dv[index]["RepoItem"] + "'";

            if (dv[index]["ParentProductCode"].ToString() != String.Empty && dv[index]["ParentProductCode"].ToString() != "NA")
            {
                xPath += " and ../../@Code='" + dv[index]["ParentProductCode"].ToString() + "'";
            }
            xPath += "]";

            XmlNode found = itemDoc.SelectSingleNode(xPath);

            if (found != null)
            {
                currentItem = found.CloneNode(true); /* 68521 */

                if (e.Button == MouseButtons.Right)
                {
                    affinityItem = found;

                    DataGrid ctl = (DataGrid)sender;

                    MenuCommand m1 = new MenuCommand(GetResource("P_EDITWARRANTY"));
                    MenuCommand m2 = new MenuCommand(GetResource("P_REMOVEITEM"));
                    MenuCommand m3 = new MenuCommand(GetResource("P_EDITCONTRACTNO"));
                    MenuCommand m4 = new MenuCommand(GetResource("P_EDITRETURNDATE"));
                    MenuCommand m5 = new MenuCommand(GetResource("P_ADDSPIFF"));
                    MenuCommand m6 = new MenuCommand(GetResource("P_REMOVESPIFF"));
                    MenuCommand mLinked = new MenuCommand(GetResource("P_AddAssociated"));

                    m1.Click += new System.EventHandler(this.AddWarranty_Click);
                    m2.Click += new System.EventHandler(this.btnRemove_Click);
                    m3.Click += new System.EventHandler(this.OnEditContractNo);
                    m4.Click += new System.EventHandler(this.OnEditReturnDate);
                    m5.Click += new System.EventHandler(this.OnAddSPIFF);
                    m6.Click += new System.EventHandler(this.OnRemoveSPIFF);
                    mLinked.Click += new System.EventHandler(this.AddAssociatedItems_Click);
                    m1.Enabled = !Collection;

                    PopupMenu popup = new PopupMenu();
                    popup.Animate = Animate.Yes;
                    popup.AnimateStyle = Animation.SlideHorVerPositive;
                    popup.MenuCommands.AddRange(new MenuCommand[] { m1, m2, mLinked });

                    if (found.Attributes[Tags.Type].Value == IT.Affinity || found.Attributes[Tags.Type].Value == IT.ReadyAssist)  //#18604 - CR15594
                        popup.MenuCommands.Add(m3);

                    if (AccountType == AT.GoodsOnLoan)
                        popup.MenuCommands.Add(m4);

                    if (Convert.ToDecimal(found.Attributes[Tags.DeliveredQuantity].Value) == 0 &&
                        currentItem.Attributes[Tags.Type].Value == IT.Stock)
                    {
                        if (Convert.ToBoolean(found.Attributes[Tags.SPIFFItem].Value))
                            popup.MenuCommands.Add(m6);
                        else
                            popup.MenuCommands.Add(m5);
                    }

                    //IP - 28/02/11 - #3180 - Only display the option to add related items based on the Country Parameters to enable/disable
                    if (AT.IsCreditType(AccountType))
                    {
                        if (Convert.ToBoolean(Country[CountryParameterNames.AssociatedProductsCredit]))
                        {
                            callRelatedProducts = true;
                        }
                    }
                    else
                    {
                        if (Convert.ToBoolean(Country[CountryParameterNames.AssociatedProductsCash]))
                        {
                            callRelatedProducts = true;
                        }
                    }

                    //IP - 28/02/11 - #3180 
                    if (!callRelatedProducts)
                    {
                        popup.MenuCommands.Remove(mLinked);
                    }

                    //IP - 24/02/11 - #3130 - If this is an Installation item then remove the menu options to add associated products
                    //and add warranty.
                    if (currentItem.Attributes[Tags.Type].Value == IT.Installation)
                    {
                        if (popup.MenuCommands.Contains(m1)) //IP - 28/02/11 - #3180 - only remove if the menu commands contains the option and we wish to remove
                        {
                            popup.MenuCommands.Remove(m1);
                        }

                        if (popup.MenuCommands.Contains(mLinked)) //IP - 28/02/11 - #3180 
                        {
                            popup.MenuCommands.Remove(mLinked);
                        }
                    }

                    //IP - 24/02/11 - #3130 
                    if (currentItem.Attributes[Tags.Type].Value == IT.Warranty || currentItem.Attributes[Tags.Type].Value == IT.KitWarranty)
                    {
                        if (popup.MenuCommands.Contains(mLinked))  //IP - 28/02/11 - #3180 
                        {
                            popup.MenuCommands.Remove(mLinked);
                        }
                    }

                    // #15888 - do not remove free warranties 
                    //if (currentItem.Attributes[Tags.Type].Value == IT.Warranty && Convert.ToBoolean(currentItem.Attributes["IsFree"].Value) == true)
                    //{
                    //    if (popup.MenuCommands.Contains(m2))  
                    //    {
                    //        popup.MenuCommands.Remove(m2);
                    //    }
                    //}

                    MenuCommand selected = popup.TrackPopup(ctl.PointToScreen(new Point(e.X, e.Y)));
                }
                else
                {

                    if (!Convert.ToBoolean(found.Attributes[Tags.SPIFFItem].Value))
                    {
                        SetItemDetails(currentItem);
                        KitClicked = false;
                        ValueControlled = Convert.ToBoolean(currentItem.Attributes[Tags.ValueControlled].Value);
                    }
                    else
                    {
                        ClearItemDetails();
                        txtProductCode.Text = "";
                    }
                }
            }
            this.CheckDeliveryAreaDate();
        }

        private void tvItems_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                Function = "tvItems_MouseDown";
                if (e.Button != MouseButtons.Right)
                    return;

                TreeNode n = tvItems.SelectedNode = tvItems.GetNodeAt(e.X, e.Y);

                if ((string)n.Tag == null)
                    return;

                string key = (string)n.Tag;
                string[] k = key.Split('|');
                string product = k[0];
                string location = k[1];
                string contract = k[2];
                string parentProductCode = k[3];

                string xPath = "//Item[@Key = '" + product + "|" + location + "' and @ContractNumber = '" + contract + "' and @Quantity != 0";
                if (parentProductCode != String.Empty)
                {
                    xPath += " and ../../@Code='" + parentProductCode + "'";
                }
                xPath += "]";

                XmlNode found = itemDoc.DocumentElement.SelectSingleNode(xPath);

                TreeView ctl = (TreeView)sender;

                MenuCommand m1 = new MenuCommand(GetResource("P_EDITWARRANTY"));
                MenuCommand m2 = new MenuCommand(GetResource("P_REMOVEITEM"));
                MenuCommand mLinked = new MenuCommand(GetResource("P_AddAssociated"));
                m1.Click += new System.EventHandler(this.AddWarranty_Click);
                m2.Click += new System.EventHandler(this.btnRemove_Click);
                mLinked.Click += new System.EventHandler(this.AddAssociatedItems_Click); //bbcc  
                if (found != null)
                {
                    switch ((string)found.Attributes[Tags.Type].Value)
                    {
                        case IT.Warranty:
                            m1.Enabled = false;
                            mLinked.Enabled = false;     //IP - 24/02/11 - #3130
                            break;
                        case IT.Kit:
                            this.KitClicked = true;        //IP - 06/08/10 - UAT(1018) UAT5.2 - Re-instating code as kit was not being removed
                            m1.Enabled = false;
                            break;
                        case IT.Installation: //IP - 24/02/11 - #3130
                            mLinked.Enabled = false;
                            m1.Enabled = false;
                            break;
                        default:
                            this.KitClicked = false;
                            break;
                    }
                }

                if (m1.Enabled)
                    m1.Enabled = !Collection;

                PopupMenu popup = new PopupMenu();
                popup.Animate = Animate.Yes;
                popup.AnimateStyle = Animation.SlideHorVerPositive;
                popup.MenuCommands.AddRange(new MenuCommand[] { m1, m2, mLinked });

                //IP - 28/02/11 - #3180 - Only enable the option to add related items based on the Country Parameters to enable/disable
                if (AT.IsCreditType(AccountType) &&
                    Country.GetCountryParameterValue<bool>(CountryParameterNames.AssociatedProductsCredit))
                {
                    callRelatedProducts = true;
                }
                else if (Country.GetCountryParameterValue<bool>(CountryParameterNames.AssociatedProductsCash))
                {
                    callRelatedProducts = true;
                }

                if (!callRelatedProducts)
                {
                    mLinked.Enabled = false;
                }

                // #15888 - do not remove free warranties 
                //if (currentItem.Attributes[Tags.Type].Value == IT.Warranty && Convert.ToBoolean(currentItem.Attributes["IsFree"].Value) == true)
                //{
                //    if (popup.MenuCommands.Contains(m2))
                //    {
                //        popup.MenuCommands.Remove(m2);
                //    }
                //}

                MenuCommand selected = popup.TrackPopup(ctl.PointToScreen(new Point(e.X, e.Y)));
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private decimal oldNoMonths = -1;

        private void txtNoMonths_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (!supressEvents && txtNoMonths.Value != oldNoMonths)
                {
                    oldNoMonths = txtNoMonths.Value;

                    CalculateServiceCharge(Config.CountryCode,
                        Convert.ToDecimal(StripCurrency(txtDeposit.Text)),
                        Convert.ToDecimal(txtNoMonths.Value),
                        Convert.ToDecimal(StripCurrency(txtSubTotal.Text)) - Convert.ToDecimal(StripCurrency(txtSalesTax.Text)),
                        Convert.ToDecimal(numPaymentHolidays.Value));
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void menuPrint_Click(object sender, System.EventArgs e)
        {
            try
            {
                bool valid = true;

                Function = "menuPrint_Click";
                Wait();

                if (!RenewalRevision)
                {
                    //10.6 CR- Sales Order - Print Save- Allow user to save while printing 
                    //and track UI changes to increment invoice version
                    _saveButtonClicked = _isSalesOrderChanged;
                    // refresh in case of customer details added
                    valid = SaveAccount();
                    //this.GetInvoiceNumberAndVersion(_acctNo);
                    this.UpdateOrdInvoiceVersionText(_acctNo);
                }

                // refresh in case of customer details added
                if (valid)
                {

                    if (this.CustomerID.Length != 0)
                    {
                        string temp = (string)drpSoldBy.SelectedItem;
                        int salesPerson = Convert.ToInt32(temp.Substring(0, temp.IndexOf(":") - 1));
                        // CR906 if this is a loan account print cash loan promissory instead of agreement
                        if (!this.IsLoan)
                        {
                            PrintAgreementDocuments(AccountNo,
                                    this.AccountType,
                                    this.CustomerID,
                                    this.PaidAndTaken,
                                    this.Collection,
                                    0, 0,
                                    itemDoc.DocumentElement,
                                    AgreementNo,
                                    this,
                                    true,
                                    salesPerson,
                                    Convert.ToInt16(((DataRowView)drpPayMethod.SelectedItem)[CN.Code]),
                                    _spaPrint, _printSchedule);         //UAT1102 jec
                        }
                        else
                        {
                            PrintCashLoanDocuments(AccountNo,
                                    this.AccountType,
                                    this.CustomerID,
                                    this.PaidAndTaken,
                                    this.Collection,
                                    0, 0,
                                    itemDoc.DocumentElement,
                                    AgreementNo,
                                    this,
                                    true,
                                    salesPerson,
                                    Convert.ToInt16(((DataRowView)drpPayMethod.SelectedItem)[CN.Code]));
                        }

                        if ((bool)Country[CountryParameterNames.PrizeVouchersActive] && PrintVouchers)
                        {
                            PrintPrizeVouchers(AccountNo, CashPrice, 0, Date.blankDate, false, false);
                            PrintVouchers = false;
                        }
                    }
                    else
                    {
                        ShowInfo("M_NOCUSTOMERFORPRINT");
                    }
                }
                else
                {
                    ShowInfo("M_DETAILSMISSING");
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
            }
        }

        private void txtProductCode_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                drpLocation.Focus();

            if (e.KeyCode != Keys.Tab && e.KeyCode != Keys.Enter)
                ItemID = null;

            LoyaltyBreakRedemption();
        }

        private void menuPaymentCard_Click(object sender, System.EventArgs e)
        {
            if (this.CustomerID.Length != 0)
            {
                if (SaveAccount() || this._hasdatachanged == false)
                    PrintPaymentCard(this.CustomerID, AccountNo);
            }
            else
                ShowInfo("M_NOCUSTOMERFORPRINT");
        }

        private void menuPrintReceipt_Click(object sender, System.EventArgs e)
        {


            if (CashAndGoReturn && paymentform != null && (!paymentform.Complete || paymentform.Error()))
            {
                paymentform.ShowDialog();
                return;
            }


            if (custidCG != null && custidCG.Length > 0)
            {
                CustomerID = custidCG;
            }

            if ((custidCG.Length == 0) && custidrequested)
            {
                errorProvider1.SetError(btnCustomerSearch, "Please link to a customer account");
            }
            else
            {
                custidrequested = false;
                bool status = true;

                // CR586 Allow multiple pay methods held as a list
                if (CashAndGoReturn && paymentform != null)
                {
                    foreach (var pay in paymentform.Payments)
                    {
                        payMethodSet.Tables[TN.PayMethodList].Rows.Add(
                                                                        new Object[]
                                                                        {
                                                                            pay.ReturnValue,
                                                                            pay.bankcode,
                                                                            pay.paymethod == PayMethod.StoreCard ? pay.storecardno.ToString() : pay.bankacctno,
                                                                            pay.chequeno,
                                                                            pay.paymethod.Value,
                                                                            pay.PayMethodDesc
                                                                        });
                    }
                }
                else
                {
                    if (CashAndGoReturn && Convert.ToInt16(drpPayMethod.SelectedValue) == PayMethod.StoreCard)
                    {
                        status = AddPayMethod(MoneyStrToDecimal(txtAmount.Text),
                          drpBank.SelectedItem != null ? ((DataRowView)drpBank.SelectedItem)[CN.BankCode].ToString() : "",    //Bug #3041
                                                  CashandGoReturnDetails.Payments[0].storecardno.ToString(),
                                                  CashandGoReturnDetails.Payments[0].chequeno.ToString(),
                                                  Convert.ToInt16(drpPayMethod.SelectedValue),
                                                  Convert.ToString(((DataRowView)drpPayMethod.SelectedItem)[CN.CodeDescription]));
                    }
                    else
                        status = AddPayMethod(MoneyStrToDecimal(txtAmount.Text),
                              drpBank.SelectedItem != null ? ((DataRowView)drpBank.SelectedItem)[CN.BankCode].ToString() : "",    //Bug #3041
                                                      txtBankAcctNo.Text,
                                                      txtChequeNo.Text,
                                                      Convert.ToInt16(((DataRowView)drpPayMethod.SelectedItem)[CN.Code]),
                                                      Convert.ToString(((DataRowView)drpPayMethod.SelectedItem)[CN.CodeDescription]));

                }

                // Authorisation is required for Legacy Cash & Go Return
                if (status && this._authorisationRequired)
                {
                    AuthorisePrompt ap = new AuthorisePrompt(this, lAuthoriseCGR, GetResource("M_PAIDTAKENAUTH"));
                    ap.ShowDialog();
                    status = ap.Authorised;
                    if (ap.Authorised)
                        returnAuthorisedBy = ap.AuthorisedBy;
                }

                //PN - 19/03/08 - (69630) Only save Paid & Taken account if sufficient payment has been made.
                if (status)
                {
                    decimal amount = MoneyStrToDecimal(txtAmount.Text);
                    decimal tendered = amount + PayMethodTotal();
                    decimal due = MoneyStrToDecimal(txtDue.Text);

                    if (tendered < due)
                    {
                        errorProvider1.SetError(txtTendered, GetResource("M_PAYMENTTOOLOW"));
                        status = false;
                    }
                    else
                    {
                        errorProvider1.SetError(txtTendered, "");
                    }
                }



                if (status)
                {
                    //IP - 05/03/12 - #9403
                    if (revision && !PaidAndTaken)
                    {
                        reqTaxInvoicePrint = true;
                    }

                    menuSave_Click(null, null);
                }



                //IP - 17/04/08 - (69630) - Set the 'CustomerID' back to Paid & Taken and 
                //Re-set the screen so that the 'Link to Customer Account' button is not enabled
                //and visible.
                CustomerID = ptCustomerID;

                //btnCustomerSearch.Visible = false;
                //btnCustomerSearch.Enabled = false;
                //menuCustomerSearch.Visible = false;
                //menuCustomerSearch.Enabled = false;

                _cashAndGoSaved = false;
            }
        }

        private void ResetCG()
        {
            txtAccountNumber.Text = _acctNo;
            btnCustomerSearch.Visible = false;
            txt_linkedcustid.Visible = false;
            lbl_linkedcust.Visible = false;
            txt_linkedcustid.Text = "";
            custidrequested = false;
            custidCG = "";
            CustomerID = "";
        }

        private void btnAddPaymethod_Click(object sender, System.EventArgs e)
        {
            // CR586 Allow multiple pay methods held as a list
            AddPayMethod(MoneyStrToDecimal(txtAmount.Text),
                            (string)((DataRowView)drpBank.SelectedItem)[CN.BankCode],
                            txtBankAcctNo.Text,
                            txtChequeNo.Text,
                            Convert.ToInt16(((DataRowView)drpPayMethod.SelectedItem)[CN.Code]),
                            Convert.ToString(((DataRowView)drpPayMethod.SelectedItem)[CN.CodeDescription]));
        }

        private void btnRemovePaymethod_Click(object sender, System.EventArgs e)
        {
            // CR586 Clear the list of payments
            this.InitPaymentSet();
        }

        private void txtAmount_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (txtAmount.Text.Length == 0)
                    txtAmount.Text = txtDue.Text;

                decimal amount = MoneyStrToDecimal(txtAmount.Text);
                decimal tendered = amount + this.PayMethodTotal();
                decimal due = MoneyStrToDecimal(txtDue.Text);
                decimal subTotal = MoneyStrToDecimal(txtSubTotal.Text);
                decimal oldValue = 0;

                if (Replacement)
                {
                    oldValue = instantReplacement.OrderValue + instantReplacement.TaxAmount;
                }

                due = subTotal - GiftVoucherValue - oldValue;
                txtDue.Text = due.ToString(DecimalPlaces);


                //btnPrintReceipt.Enabled = (tendered >= due);

                //IP - 14/03/08 -(69630) - Handling the print receipt button
                //therefore will not enable the print receipt button 
                //when a paid & taken account is set up with a warranty until the 
                //paid and taken account has been linked to a customer.
                if (!(PaidAndTaken && warrantySelected))
                {
                    menuPrintReceipt.Enabled = btnPrintReceipt.Enabled = (tendered >= due) && errorProviderStoreCard.GetError(drpPayMethod) == string.Empty &&
                                      errorProviderStoreCard.GetError(btnStoreCardManualEntry) == string.Empty &&
                                      errorProviderStoreCard.GetError(txtAmount) == string.Empty;       //#14439
                }

                //IP - 11/02/10 - CR1048 (Ref:3.1.2.5) Merged - Malaysia Enhancements (CR1072)
                if (drpPayMethod.Items.Count == 0)
                {
                    btnPrintReceipt.Enabled = false;
                    menuPrintReceipt.Enabled = false;
                }
                else
                {
                    btnPrintReceipt.Enabled = (tendered >= due) && errorProviderStoreCard.GetError(drpPayMethod) == string.Empty &&
                                      errorProviderStoreCard.GetError(btnStoreCardManualEntry) == string.Empty &&
                                      errorProviderStoreCard.GetError(txtAmount) == string.Empty;       //#14439
                }

                txtChange.Text = (tendered - due).ToString(DecimalPlaces);
                if (MoneyStrToDecimal(txtChange.Text) < 0)
                    txtChange.Text = (0).ToString(DecimalPlaces);

                txtAmount.Text = amount.ToString(DecimalPlaces);
                txtTendered.Text = tendered.ToString(DecimalPlaces);

                errorProvider1.SetError(txtAmount, "");

            }
            catch (Exception ex)
            {
                //e.Cancel = true;
                txtAmount.Focus();
                txtAmount.Select(0, txtAmount.Text.Length);
                errorProvider1.SetError(txtAmount, ex.Message);
            }
        }

        private void menuCancel_Click(object sender, System.EventArgs e)
        {
            Cancelled = true;
            CloseTab();
        }

        private void chxDutyFree_CheckedChanged(object sender, System.EventArgs e)
        {
            Function = "chxDutyFree_CheckedChanged";
            try
            {
                ClearItemDetails();
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                Function = "End of chxDutyFree_CheckedChanged";
            }
        }

        private void chxTaxExempt_CheckedChanged(object sender, System.EventArgs e)
        {
            Function = "chxTaxExempt_CheckedChanged";
            try
            {
                ClearItemDetails();
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                Function = "End of chxTaxExempt_CheckedChanged";
            }
        }

        private void btnPrint_Click(object sender, System.EventArgs e)
        {
            menuPrint_Click(this, null);
        }

        private void drpPayMethod_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            ClearStoreCardErrors();

            itemValExceeded = false;                              //IP - 03/12/10 - Store Card  

            clearPaymentFields();              //IP - 26/01/11 - Sprint 5.9 - Clear payment fields when changing payment method.

            if (this._exchangeChanged) return;

            int curPayMethod = Convert.ToInt16(((DataRowView)drpPayMethod.SelectedItem)[CN.Code].ToString());
            if (PayMethod.IsPayMethod(curPayMethod, PayMethod.Cash) || PayMethod.IsPayMethod(curPayMethod, PayMethod.USdollars))
            {
                drpBank.Enabled = false;
                txtBankAcctNo.Enabled = false;
                txtChequeNo.Enabled = false;

                mtb_CardNo.Visible = false;    //IP - 26/01/11 - Sprint 5.9 - #2785


                //IP - 11/02/10 - CR1048 (Ref:3.1.2.5) Merged - Malaysia Enhancements (CR1072)
                if (drpBank.Items.Count > 0)
                {
                    drpBank.SelectedIndex = 0;
                }
                else
                {
                    drpBank.SelectedIndex = -1;
                }

                txtBankAcctNo.Text = "";
                txtChequeNo.Text = "";
            }
            else
            {
                drpBank.Enabled = true;
                txtBankAcctNo.Enabled = true;
                txtChequeNo.Enabled = true;

            }

            // Might need to use the Foreign Currency Calculator
            if (PayMethod.IsForeignCurrencyPayMethod(curPayMethod))
            {
                // Call the Foreign Currency Calculator popup form
                ExchangeCalculator();
                btnExchange.Visible = true;
            }
            else
            {
                btnExchange.Visible = false;
            }

            //IP - 26/01/11 - Sprint 5.9 - #2785
            if (PayMethod.IsPayMethod(curPayMethod, PayMethod.CreditCard) || PayMethod.IsPayMethod(curPayMethod, PayMethod.DebitCard))
            {
                txtChequeNo.Visible = false;
                mtb_CardNo.Visible = true;
            }

            //IP - 26/01/11 - Sprint 5.9 - #2785
            if (PayMethod.IsPayMethod(curPayMethod, PayMethod.Cheque))
            {
                txtChequeNo.Visible = true;
                mtb_CardNo.Visible = false;
            }
            SetStoreCardControls(PayMethod.IsPayMethod(curPayMethod, PayMethod.StoreCard));
        }




        private void ExchangeCalculator()
        {
            // Call the Foreign Currency Calculator popup form
            int curPayMethod = Convert.ToInt16(((DataRowView)drpPayMethod.SelectedItem)[CN.Code].ToString());
            ExchangeCalculator ExchangeCalculatorPopup = new ExchangeCalculator(this.FormRoot, this, curPayMethod, MoneyStrToDecimal(this.txtAmount.Text, DecimalPlaces));
            ExchangeCalculatorPopup.ShowDialog();
            if (ExchangeCalculatorPopup.convert)
            {
                _exchangeChanged = true;
                drpPayMethod.SelectedValue = ExchangeCalculatorPopup.payMethod;
                _exchangeChanged = false;
                txtAmount.Text = ExchangeCalculatorPopup.newAmount.ToString(DecimalPlaces);
                // Invoke leave method
                txtAmount_Validating(new object(), new System.ComponentModel.CancelEventArgs());
            }
        }

        private void txtEmpNumber_Leave(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                string err = "";
                string employee = "";
                errorProvider1.SetError(txtEmpNumber, "");
                DataSet ds = null;
                if (IsStrictNumeric(txtEmpNumber.Text)) // ensure empeeno is numeric 68605 jec 17/11/06
                {
                    if (txtEmpNumber.Text.Length > 0)
                        ds = Login.GetEmployeeDetails(Convert.ToInt32(txtEmpNumber.Text), err);

                    if (err.Length > 0)
                        ShowError(err);
                    else if (ds != null)
                    {
                        foreach (DataTable dt in ds.Tables)
                        {
                            foreach (DataRow row in dt.Rows)
                            {
                                employee = txtEmpNumber.Text + " : " + (string)row[CN.EmployeeName];
                                salesStaff.Add(employee.ToUpper());
                                drpSoldBy.DataSource = null;
                                drpSoldBy.DataSource = salesStaff;
                            }
                        }

                        int i = drpSoldBy.FindString(employee);
                        if (i != -1)
                            drpSoldBy.SelectedIndex = i;
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "txtEmpNumber_Leave");
            }
            finally
            {
                StopWait();
            }
        }

        private void drpBranch_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                //CR903 jec 07/08/07
                foreach (DataRow row in ((DataTable)StaticData.Tables[TN.BranchNumber]).Rows)
                {
                    if (row[CN.BranchNo].ToString() == (string)drpBranch.SelectedItem)
                    {
                        ScoreHPbefore = (bool)row[CN.ScoreHPbefore];
                        createRF = (bool)row[CN.CreateRFAccounts];
                        createHP = (bool)row[CN.CreateHPAccounts];
                        createCash = (bool)row[CN.CreateCashAccounts];
                        break;
                    }
                }
                if (enterRaised)
                {
                    Wait();
                    RefreshBranchSpecificData((string)drpBranch.SelectedItem);
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "drpBranch_SelectedIndexChanged");
            }
            finally
            {
                StopWait();
            }
        }

        private void NewAccount_Enter(object sender, System.EventArgs e)
        {
            enterRaised = true;
        }

        private void menuReprint_Click(object sender, System.EventArgs e)
        {
            try
            {
                Function = "menuPrint_Click";
                Wait();

                if (SaveAccount())
                {
                    if (this.CustomerID.Length != 0)
                    {
                        string temp = (string)drpSoldBy.SelectedItem;
                        int salesPerson = Convert.ToInt32(temp.Substring(0, temp.IndexOf(":") - 1));

                        PrintAgreementDocuments(AccountNo,
                            this.AccountType,
                            this.CustomerID,
                            this.PaidAndTaken,
                            this.Collection,
                            0, 0,
                            itemDoc.DocumentElement,
                            AgreementNo,
                            this,
                            false,
                            salesPerson,
                            Convert.ToInt16(((DataRowView)drpPayMethod.SelectedItem)[CN.Code]));
                    }
                    else
                    {
                        ShowInfo("M_NOCUSTOMERFORPRINT");
                    }
                }
                else
                {
                    ShowInfo("M_DETAILSMISSING");
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
            }
        }

        public void SaveAfterAddTo()
        {
            SaveAccount();
        }

        private void dtDeliveryRequired_Leave(object sender, System.EventArgs e)
        {
            try
            {
                if (!supressEvents && this.dtDeliveryRequired.Enabled)
                {
                    Wait();

                    //IP - 04/01/2008 - UAT(353)
                    DateTime dateOpened = Convert.ToDateTime(DateAccountOpened.ToShortDateString());

                    //IP - 04/01/2008 - UAT(353)
                    //if (this.dtDeliveryRequired.Value < DateAccountOpened)
                    if (this.dtDeliveryRequired.Value < dateOpened)
                    {
                        //IP - 04/01/2008 - UAT(353)
                        //ShowInfo("M_REQDELIVERYDATE", new object[] { DateAccountOpened.ToShortDateString() });
                        //this.dtDeliveryRequired.Value = DateAccountOpened;
                        ShowInfo("M_REQDELIVERYDATE", new object[] { dateOpened });
                        this.dtDeliveryRequired.Value = dateOpened;
                        this.dtDeliveryRequired.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "dtDeliveryRequired_Leave");
            }
            finally
            {
                StopWait();
            }
        }

        private void rbGiftVoucher_CheckedChanged(object sender, System.EventArgs e)
        {
            try
            {
                Wait();

                /* if they check the gift voucher button, we need to launch a dialog
				 * to ask what kind of gift voucher it is and what the reference 
				 * number is */
                if (chxGiftVoucher.Checked)
                {
                    GiftVoucher gv = new GiftVoucher(this, FormRoot, false);
                    gv.ShowDialog();
                }
                else
                {
                    GiftVoucherValue = 0;
                    CourtsVoucher = true;
                    VoucherReference = "";
                    VoucherAuthorisedBy = 0;
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "rbGiftVoucher_CheckedChanged");
            }
            finally
            {
                StopWait();
            }
        }

        private void btnExchange_Click(object sender, System.EventArgs e)
        {
            try
            {
                Function = "Cash and Go Screen: Open Exchange Calculator";
                Wait();

                // Call the Foreign Currency Calculator popup form
                this.ExchangeCalculator();
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
            }
        }

        private void menuOpenCashTill_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();

                OpenCashDrawer(true);

            }
            catch (Exception ex)
            {
                Catch(ex, "menuOpenCashTill_Click");
            }
            finally
            {
                StopWait();
            }
        }

        private decimal oldPaymentHolidays = -1;

        private void numPaymentHolidays_ValueChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (supressEvents || numPaymentHolidays.Value == oldPaymentHolidays)
                    return;

                decimal max = Convert.ToDecimal(Math.Floor(Convert.ToDouble(txtNoMonths.Value * 0.1M)));
                if (numPaymentHolidays.Value > max)
                    numPaymentHolidays.Value = max;

                if (numPaymentHolidays.Value != oldPaymentHolidays)
                {
                    oldPaymentHolidays = numPaymentHolidays.Value;
                    CalculateServiceCharge(Config.CountryCode,
                        Convert.ToDecimal(StripCurrency(txtDeposit.Text)),
                        Convert.ToDecimal(txtNoMonths.Value),
                        Convert.ToDecimal(StripCurrency(txtSubTotal.Text)) - Convert.ToDecimal(StripCurrency(txtSalesTax.Text)),
                        Convert.ToDecimal(numPaymentHolidays.Value));
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "numPaymentHolidays_ValueChanged");
            }
        }

        private void numPaymentHolidays_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (supressEvents || numPaymentHolidays.Value == oldPaymentHolidays)
                    return;

                decimal max = Convert.ToDecimal(Math.Floor(Convert.ToDouble(txtNoMonths.Value * 0.1M)));
                if (numPaymentHolidays.Value > max)
                    numPaymentHolidays.Value = max;

                if (numPaymentHolidays.Value != oldPaymentHolidays)
                {
                    oldPaymentHolidays = numPaymentHolidays.Value;
                    CalculateServiceCharge(Config.CountryCode,
                        Convert.ToDecimal(StripCurrency(txtDeposit.Text)),
                        Convert.ToDecimal(txtNoMonths.Value),
                        Convert.ToDecimal(StripCurrency(txtSubTotal.Text)) - Convert.ToDecimal(StripCurrency(txtSalesTax.Text)),
                        Convert.ToDecimal(numPaymentHolidays.Value));
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "numPaymentHolidays_Validating");
            }
        }

        private void GeneratePTAccountNumber()
        {
            if (!(PaidAndTaken && (WarrantiesOnCredit || ptWarranties || hasInstallation)))
                return;

            // Make sure a new account number has been generated
            if (PTWarrantyAccountNo.Length == 0)
            {
                AccountManager.GenerateAccountNumber(
                    Config.CountryCode,
                    Convert.ToInt16((string)drpBranch.SelectedItem),
                    AT.Special,
                    false, 0,
                    out _PTWarrantyAccountNo,
                    out Error);

                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    if (_PTWarrantyAccountNo == "000-0000-0000-0")
                        ShowInfo("M_ACCOUNTNOFAILED");
                    else
                    {
                        txtAccountNumber.Text = _PTWarrantyAccountNo;
                        _PTWarrantyAccountNo = _PTWarrantyAccountNo.Replace("-", "");
                    }
                }
                CustomerID = "";

                // Generate an agreement no other than 1 so that the special account
                // created can masquerade as a P&T account for IR warranties
                AgreementNo = AccountManager.GetBuffNo(Convert.ToInt16((string)drpBranch.SelectedItem), out Error);
                if (Error.Length > 0)
                    ShowError(Error);
            }
        }

        private bool AddRelatedItems(int itemId, double quantity, short location)
        {
            bool status = true;
            bool removeInstallationItems = false;    //IP - 24/02/11 - Sprint 5.10 - #3133
            bool removeOtherItems = false;           //IP - 24/02/11 - Sprint 5.10 - #3133
            //10.6 CR- Sales Order - Print Save- Allow user to save while printing
            // and track UI changes to increment invoice version
            _isSalesOrderChanged = true;
            int count = 0; //IP - 24/02/11 - Sprint 5.10 - #3133
            StringBuilder sb = new StringBuilder();  //IP - 24/02/11 - Sprint 5.10 - #3133

            string key = itemId + "|" + location.ToString();
            this._hasdatachanged = true;
            bool relatedProductsChosen = false;
            Function = "BACcountManager::GetRelatedItems()";
            DataSet ds = AccountManager.GetRelatedItems(itemId, location, out Error);
            if (Error.Length > 0)
                ShowError(Error);
            else
            {
                DataView relatedItems = ds.Tables["RelatedItems"].DefaultView;
                // Now check whether child items already exists in dataset and if so remove them so user can't rechoose. 
                if (relatedItems.Count > 0)
                {
                    XmlNode parent = xml.findItem(itemDoc.DocumentElement, key);
                    foreach (XmlNode child in parent.ChildNodes)
                    {
                        foreach (XmlNode grandchild in child.ChildNodes)
                        {
                            foreach (DataRowView drv in relatedItems)
                            {
                                //#16342 - remove from related items if already added
                                string xPathItemCheck = "//Item[@ItemId = '" + Convert.ToString(drv.Row["ItemId"]) + "']";
                                XmlNode checkExists = itemDoc.DocumentElement.SelectSingleNode(xPathItemCheck);

                                if (checkExists != null)
                                {
                                    removeOtherItems = true;
                                    sb.AppendFormat(" {0}productCode not = '{1}' ",
                                                    count > 0 ? "AND " : "",
                                                    drv.Row["productCode"]);
                                    count++;
                                }

                                if (grandchild.NodeType != XmlNodeType.Element || grandchild.Attributes.Count <= 0)
                                    continue;

                                if (Convert.ToString(drv.Row["productCode"]) == grandchild.Attributes[Tags.Code].Value && Convert.ToInt16(grandchild.Attributes[Tags.Quantity].Value) > 0
                                                                                && Convert.ToBoolean(drv.Row[CN.Installation]) == true)
                                {
                                    removeInstallationItems = true;      //IP - 24/02/11 - Sprint 5.10 - #3133
                                }
                                else if (Convert.ToString(drv.Row["productCode"]) == grandchild.Attributes[Tags.Code].Value && Convert.ToInt16(grandchild.Attributes[Tags.Quantity].Value) > 0
                                        && Convert.ToBoolean(drv.Row[CN.Installation]) == false)
                                {
                                    //IP - 24/02/11 - Sprint 5.10 - #3133
                                    removeOtherItems = true;
                                    sb.AppendFormat(" {0}productCode not = '{1}' ",
                                                    count > 0 ? "AND " : "",
                                                    drv.Row["productCode"]);
                                    count++;
                                }
                            }
                        }
                    }
                    //ds.Tables["RelatedItems"].AcceptChanges();     //IP - 24/02/11 - Sprint 5.10 - #3133 - commented out

                    //IP - 24/02/11 - Sprint 5.10 - #3133
                    if (removeOtherItems == true && removeInstallationItems == true)
                    {
                        sb.Append("AND Installation not = '1'");
                    }
                    else if (removeOtherItems == false && removeInstallationItems == true)
                    {
                        sb.Append("Installation not = '1'");
                    }

                    if (removeOtherItems == true || removeInstallationItems == true)
                    {
                        relatedItems.RowFilter = Convert.ToString(sb);
                    }

                    //Launch a pop-up holding related items
                    if (relatedItems.Count > 0)
                    {
                        RelatedProducts rp = new RelatedProducts(relatedItems, quantity,
                            currentItem, itemId, AccountType, this,
                            this.FormRoot);
                        rp.ParentKey = key;
                        rp.ParentLocn = location;
                        rp.DutyFree = chxDutyFree.Checked;
                        rp.TaxExempt = chxTaxExempt.Checked;
                        rp.DeliveryDate = Convert.ToString(dtDeliveryRequired.Value);
                        //rp.DeliveryTime = txtTimeRequired.Text;
                        rp.DeliveryTime = Convert.ToString(cbTime.SelectedItem);                //IP - 28/05/12 - #10225
                        rp.DelNoteBranch = drpBranchForDel.Text; //location.ToString();
                        rp.AccountNo = AccountNo;
                        rp.AgreementNo = AgreementNo;
                        rp.AllowSupaShield = allowSupaShield.Enabled;

                        rp.ShowDialog();        //launch as a modal dialog
                        relatedProductsChosen = rp.RelatedProductsChosen;
                    }
                    else if (relatedItems.Count == 0 && removeInstallationItems)
                    {
                        ShowInfo("M_MAXINSTALLATIONS");
                        status = false;
                    }
                    else
                        status = false;
                }
                else
                {
                    //IP - 25/02/11 - Sprint 5.11 - #3230 
                    if (associatedItemRightClick)
                    {
                        ShowInfo("M_NOASSOCIATEDITEMS");
                    }

                    status = false;
                }
            }

            associatedItemRightClick = false;

            SetCustomerDetails();

            if (!this.Replacement && relatedProductsChosen) //AA 29 mar
                this.GeneratePTAccountNumber();
            ClearItemDetails();
            txtProductCode.Text = "";

            return status;
        }

        private void cbDeposit_Click(object sender, System.EventArgs e)
        {
            CalculateServiceCharge(Config.CountryCode,
                Convert.ToDecimal(StripCurrency(txtDeposit.Text)),
                Convert.ToDecimal(txtNoMonths.Value),
                Convert.ToDecimal(StripCurrency(txtSubTotal.Text)) - Convert.ToDecimal(StripCurrency(txtSalesTax.Text)),
                Convert.ToDecimal(numPaymentHolidays.Value));

            decimal st = Convert.ToDecimal(StripCurrency(txtSubTotal.Text)) - Convert.ToDecimal(StripCurrency(txtDeposit.Text));
            // cr906 we do not want to subtract Loan items  from sub total 
            // for  nonStockTotal when we are calculating

            if ((st - (nonStockTotal - loanTotal)) > RFMax && this.AccountType == AT.ReadyFinance)
            {
                txtRFMax.BackColor = Color.Red;
                txtRFMax.ForeColor = Color.White;
                txtSubTotal.BackColor = Color.Red;
                txtSubTotal.ForeColor = Color.White;
            }
            else
            {
                txtRFMax.BackColor = SystemColors.Control;
                txtRFMax.ForeColor = SystemColors.ControlText;
                txtSubTotal.BackColor = SystemColors.Control;
                txtSubTotal.ForeColor = SystemColors.ControlText;
            }

            /* default the amount paid to the amount due */
            txtDue.Text = txtSubTotal.Text;
            st = Convert.ToDecimal(StripCurrency(txtSubTotal.Text));

            /* if paying my gift voucher then the amount must be at least the value of 
			 * the voucher */
            if (chxGiftVoucher.Checked)
            {
                txtDue.Text = (MoneyStrToDecimal(txtDue.Text) - GiftVoucherValue).ToString(DecimalPlaces);
            }
            if (Replacement)
            {
                decimal oldVal = instantReplacement.OrderValue + instantReplacement.TaxAmount;
                //if(st > oldVal)
                //{
                this.txtDue.Text = (st - oldVal).ToString(DecimalPlaces);
                //}
            }

            // Check the agreement term is not too long if selling an Affinity item
            this.CheckAffinityTerm();

            txtAmount.Text = txtDue.Text;
            txtAmount_Validating(null, null);
        }

        private void SetUpWarrantyRenewals()
        {
            if (warrantyRenewals.Columns.Contains(CN.CustID) == false)
            {
                warrantyRenewals.Columns.Add(new DataColumn(CN.CustID));
            }

            foreach (DataRow r in warrantyRenewals.Rows)
            {
                r[CN.CustID] = CustomerID;

                txtProductCode.Text = (string)r[CN.RenewalWarrantyNo];
                txtProductCode_Validating(this, null);

                drpLocation.DataSource = new string[] { (string)drpBranch.SelectedItem };
                txtQuantity.Text = "1";

                if (Convert.ToDateTime(r[CN.DateExpires]) > DateTime.Now)
                    dtRenewalDelDate.Value = Convert.ToDateTime(r[CN.DateExpires]).AddDays(1);
                else
                    dtRenewalDelDate.Value = this._today;

                contractNo = (string)r[CN.RenewalContractNo];

                drpLocation_Validating(this, null);
                txtQuantity_Validating(this, null);
                btnEnter_Click(this, null);

                renewalsLoaded = true;
            }

            StringCollection empty = new StringCollection();
            //drpLocation.DataSource = empty;
        }

        private void btnPlan_Click(object sender, System.EventArgs e)
        {
            DataView dvVariable = variableRatesSet.Tables[TN.Rates].DefaultView;
            VariableInstalments vi = new VariableInstalments(dvVariable, this.FormRoot, this);
            vi.ShowDialog();
        }

        private void cbImmediate_CheckedChanged(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                errorProvider1.SetError(drpDeliveryArea, "");
                SetDeliveryImmediate();
            }
            catch (Exception ex)
            {
                Catch(ex, "cbImmediate_CheckedChanged");
            }
            finally
            {
                StopWait();
            }
        }

        private void drpDeliveryAddress_SelectionChangeCommitted(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                SetDeliveryArea();
            }
            catch (Exception ex)
            {
                Catch(ex, "drpDeliveryAddress_SelectionChangeCommitted");
            }
            finally
            {
                StopWait();
            }
        }

        private void menuHelp_Click(object sender, System.EventArgs e)
        {
        }

        private void newAccount_HelpRequested(object sender, System.Windows.Forms.HelpEventArgs hlpevent)
        {
            if (!PaidAndTaken) //will have different help if cash/hp account rather than cash and go
            {
                string fileName = "newsalesorderscreen.htm";
                LaunchHelp(fileName);
            }
        }

        private void menuHelpRequested_Click(object sender, System.EventArgs e)
        {
            newAccount_HelpRequested(this, null);
        }

        private bool NewDelNoteRequired(XmlNode originalItem, string acctNo, int itemID, short location, double scheduled, double newQty, double oldQty)       //IP/NM - 18/05/11 -CR1212 - #3627 
        {
            bool status = true;

            if (currentItem.Attributes[Tags.DeliveryDate].Value.Trim() != originalItem.Attributes[Tags.DeliveryDate].Value.Trim() ||
                //newQty != oldQty ||               //IP - 08/06/12 - #10319 - This may have been added here in error. Should be able to change quantity as this cannot be done through Change Order Details.
                currentItem.Attributes[Tags.DeliveryTime].Value.Trim() != originalItem.Attributes[Tags.DeliveryTime].Value.Trim() ||
                currentItem.Attributes[Tags.BranchForDeliveryNote].Value.Trim() != originalItem.Attributes[Tags.BranchForDeliveryNote].Value.Trim() ||
                currentItem.Attributes[Tags.DeliveryArea].Value.Trim() != originalItem.Attributes[Tags.DeliveryArea].Value.Trim() ||
                currentItem.Attributes[Tags.DeliveryProcess].Value.Trim() != originalItem.Attributes[Tags.DeliveryProcess].Value.Trim() ||
                currentItem.Attributes[Tags.DeliveryAddress].Value.Trim() != originalItem.Attributes[Tags.DeliveryAddress].Value.Trim())
            {
                ShowInfo("M_REVISENOTALLOWED");
                status = false;
            }

            if (status)
            {
                if (newQty > oldQty)
                {
                    bool onPickList = false;
                    bool delNotePrinted = false;
                    bool onLoad = false;

                    AccountManager.GetScheduledDelNote(acctNo,
                        this.AgreementNo, itemID, location,                     //IP/NM - 18/05/11 -CR1212 - #3627         
                        out onPickList, out delNotePrinted, out onLoad,
                        out Error);

                    //delivery note has been scheduled for delivery through
                    //transport scheduling
                    if (delNotePrinted && CashAndGoReturn == false) //uat(5.2)-907, 4.3 merge
                    {
                        ShowInfo("M_ITEMSCHEDULED");
                        status = false;
                    }

                    if (status)
                    {
                        if (onLoad)
                        {
                            if (!lreviseScheduled.Enabled)
                            {
                                ShowInfo("M_REVISESCHEDULED");
                                status = false;
                            }
                        }
                        else
                        {
                            if (!lreviseNonScheduled.Enabled)
                            {
                                ShowInfo("M_REVISEAWAITING");
                                status = false;
                            }
                        }
                    }

                    if (status)
                    {
                        ScheduleOverride sched = new ScheduleOverride(true, acctNo, this.AgreementNo, itemID,             //IP/NM - 18/05/11 -CR1212 - #3627   
                                                            location, scheduled, FormRoot, this, true);
                        sched.ShowDialog();
                    }
                }
            }

            return status;
        }

        private bool MaximumWarrantiesPurchased(string key, int quantity)
        {
            bool maxPurchased = false;

            XmlNode found = xml.findItem(itemDoc.DocumentElement, key);
            string warrantyPath = "RelatedItems/Item[@Type = 'Warranty' and @Quantity != 0 and @WarrantyType !='" + WarrantyType.Free + "']";   //#17973
            XmlNodeList warrantyList = found.SelectNodes(warrantyPath);

            if (warrantyList.Count == quantity)
            {
                if (!AddedExistingWarrantyLink)
                    ShowInfo("M_MAXWARRANTY");

                AddedExistingWarrantyLink = false;
                maxPurchased = true;
            }
            else
                ((MainForm)this.FormRoot).statusBar1.Text = String.Empty;

            return maxPurchased;
        }

        /// <summary>
        /// OnKeyPress
        /// should trap whether data changed updating _hasdatachanged
        /// </summary>
        /// 
        public void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            _hasdatachanged = true;
        }

        private void btnTermsTypeBand_Click(object sender, EventArgs e)
        {
            try
            {
                Function = "btnTermsTypeBand_Click";
                Wait();
                if (_currentBand == String.Empty)
                    _currentBand = this._scoringBand;
                // Call the terms type band popup form
                TermsTypeMatrixPopup TermsTypeBand = new TermsTypeMatrixPopup(
                    this.FormRoot, this.FormParent,
                    this.dvTermsTypes,
                    ((DataRowView)drpTermsType.SelectedItem)[CN.TermsTypeCode].ToString(),
                      _scoringBand, //changed from current band...
                    this.AccountNo,
                    customerPClubCode,
                    privilegeClubDesc,
                    this._affinityTerms,
                    this.drpAccountType.Text, SType, IsLoan, CustomerID);

                if (TermsTypeBand.ShowDialog() == DialogResult.OK)
                {
                    // Set the selected band
                    if (_scoringBand != TermsTypeBand.band)
                        _scoringBand = TermsTypeBand.band;
                    // Filter the terms for the new band
                    DataView dvTermsType = (DataView)drpTermsType.DataSource;

                    // Get the current setting
                    string curTermsType = drpTermsType.Text;

                    FilterTermsType(ref dvTermsType, _affinityTerms, drpAccountType.Text, _scoringBand, SType, IsLoan);

                    // Make sure the TermsType has not changed if it is still available
                    int selectedTermsType = drpTermsType.FindStringExact(curTermsType);
                    drpTermsType.SelectedIndex = (selectedTermsType != -1) ? selectedTermsType : 0;

                    // Set to the selected terms type
                    foreach (DataRowView row in ((DataView)(drpTermsType.DataSource)))
                    {
                        // If the selected terms type changes this will recalculate the service charge
                        if (row[CN.TermsTypeCode].ToString() == TermsTypeBand.termsType)
                            drpTermsType.SelectedItem = row;
                    }

                    // The band may change even though the terms type has not,
                    // so make sure the Service Charge is re-calculated
                    drpTermsType_SelectedIndexChanged(sender, e);
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
            }
        }

        string BuildCategoryFilter(string category)
        {
            string filter = "and ProductCategory in (";
            bool isAdded = false;
            accountWideDiscount = false;    //IP - 31/05/11 - CR1212 - RI - #2315

            foreach (DataRow row in ((DataTable)StaticData.Tables[TN.DiscountLinks]).Rows)
            {
                if (Convert.ToString(row[CN.Reference]) == category)     // jec 14/07/08 UAT477 (string) to Convert.Tostring
                {
                    int len = row[CN.Code].ToString().Length;               // #17637
                    filter += "'" + ((string)row[CN.Code]).Substring(0, len < 3 ? len : 3) + "',";              // #17637
                    isAdded = true;
                }

            }

            accountWideDiscount = !isAdded; //IP - 31/05/11 - CR1212 - RI - #2315 - discount has not been linked to a specific category therefore it is account wide discount.

            if (!accountWideDiscount)
            {
                if (isAdded == false) //IP - 31/05/11 - CR1212 - RI - #2315
                {
                    filter = "";
                }
                else
                {
                    filter = filter.Substring(0, filter.Length - 1);
                    filter += ")";
                };
            }
            else
            {
                filter = filter + "'PCE', 'PCF', 'PCW')";
            }


            return filter;
        }

        private void ListSpiffs(string productCode, short location)
        {
            try
            {
                Function = "ListSpiffs()";

                dvLinkedSpiffs = null;
                //var repo = false;           // RI

                if (productCode.Length > 6 && productCode.Substring(0, 1) == "R")       // Repo item
                {
                    productCode = productCode.Substring(1, productCode.Length - 1);
                    //repo = true;
                }

                DataSet ds = AccountManager.GetSpiffs(productCode, location, ItemID ?? 0, out Error);           // RI 
                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    DataView dvSpiffs = ds.Tables[TN.Spiffs].DefaultView;
                    if (dvSpiffs.Count > 0)
                    {
                        //launch a pop-up listing spiffs for similar products
                        SpiffSelection ss = new SpiffSelection(this.FormRoot, this, AccountType, dvSpiffs);
                        ss.ShowDialog();
                    }

                    dvLinkedSpiffs = ds.Tables[TN.LinkedSpiffs].DefaultView;
                    btnLinkedSpiffItems.Visible = (dvLinkedSpiffs.Count > 0);
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void btnLinkedSpiffItems_Click(object sender, EventArgs e)
        {
            //launch a pop-up listing linked spiffs 
            // jec 02/07/08 UAT440 add account type
            SpiffSelection ss = new SpiffSelection(this.FormRoot, this, dvLinkedSpiffs, AccountType);
            ss.ShowDialog();
        }

        private void gbHP_Enter(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// CR903 Finds the store type of the account
        /// </summary>
        /// <returns>Store Type</returns>
        private string FindStoreType(string acctNo)
        {
            string type = String.Empty;

            if (branches.Contains(acctNo.Substring(0, 3)))
            {
                type = StoreType.Courts;
            }
            else
            {
                type = StoreType.NonCourts;
            }
            return type;
        }

        //IP - 21/01/08 - Replacing 'txtProductCode_TextChanged' as previously
        //the 'ClearItemDetails()' would be called each time a character was entered.
        private void txtProductCode_Leave(object sender, EventArgs e)
        {
            this._hasdatachanged = true;
            Function = "txtProductCode_Leave";
            try
            {
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

        public void SPARefinancePrint(string termstype)
        {
            try
            {
                // Init Delivery Address Type to HOME
                int i = drpDeliveryAddress.FindStringExact(GetResource("L_HOME"));
                if (i >= 0)
                    drpDeliveryAddress.SelectedIndex = i;

                // Reset the item Delivery Area, but only when intialising
                cbImmediate.Checked = false;
                SetDeliveryImmediate();

                dtDeliveryRequired.Value = this._today.AddDays(Convert.ToInt32(Country[CountryParameterNames.DefaultDelDays]));
                // Show button for terms type bands if this is enabled
                btnTermsTypeBand.Visible = (Convert.ToBoolean(Country[CountryParameterNames.TermsTypeBandEnabled]));

                refinTermsType = termstype;     // set termstype
                menuPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }
        // FA - 27/11/09 Added next 3 events to remove the possibility of someone selecting more than one row.
        // This is to fix UAT 509 as the code, and the system, should not allow multiple selections
        private void dgLineItems_MouseMove(object sender, MouseEventArgs e)
        {
            this.mouseX = e.X; // global variables
            this.mouseY = e.Y;
        }

        private void dgLineItems_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.DataGrid.HitTestInfo myHitTest = this.dgLineItems.HitTest(mouseX, mouseY);
            if (myHitTest.Type == DataGrid.HitTestType.RowHeader)
            {
                for (int i = 0; i < this.dgLineItems.VisibleRowCount; i++)
                {
                    this.dgLineItems.UnSelect(i);
                }
                this.dgLineItems.Select(myHitTest.Row);
            }

        }

        private void dgLineItems_CurrentCellChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < this.dgLineItems.VisibleRowCount; i++)
            {
                this.dgLineItems.UnSelect(i);
            }
            this.dgLineItems.Select(this.dgLineItems.CurrentRowIndex);
        }

        //IP - 11/02/10 - CR1048 (Ref:3.1.2.5) Merged - Malaysia Enhancements (CR1072)
        //Method to set the Pay Method drop down to the Pay Method that was used to process the Cash & Go Sale.
        //And set the Bank, Bank Account Number and ChequeNo fields if these were set.
        public void SetCashGoRefundPayMethod()
        {

            string PaymentMethod = string.Empty;
            string BankName = string.Empty;
            string BankAcct = string.Empty;
            string CardNo = string.Empty;

            DataSet dsCashGoPayments = AccountManager.GetCashAndGoLastPayMethod(_acctNo, AgreementNo, out Error);

            if (Error.Length > 0)
            {
                ShowError(Error);
            }
            else
            {
                foreach (DataTable dtCashGoPayments in dsCashGoPayments.Tables)
                {
                    if (dtCashGoPayments.TableName == "CashAndGoPayments")
                    {
                        foreach (DataRow dr in dtCashGoPayments.Rows)
                        {
                            PaymentMethod = Convert.ToString(dr[CN.PayMethod]);
                            BankName = Convert.ToString(dr[CN.BankName]);
                            BankAcct = Convert.ToString(dr[CN.BankAccountNo2]);
                            CardNo = Convert.ToString(dr[CN.ChequeNo]);
                        }
                    }
                }

                int index = drpPayMethod.FindString(PaymentMethod);
                if (index != -1)
                {
                    drpPayMethod.SelectedIndex = index;
                }
                else
                {
                    drpPayMethod.SelectedIndex = 0;
                }

                int curPayMethod = Convert.ToInt16(((DataRowView)drpPayMethod.SelectedItem)[CN.Code].ToString());

                if (BankName != string.Empty)
                {
                    int bankNameIndex = drpBank.FindString(BankName);
                    if (bankNameIndex != -1)
                    {
                        drpBank.SelectedIndex = bankNameIndex;
                    }
                    else
                    {
                        drpBank.SelectedIndex = 0;
                    }
                }

                if (BankAcct != string.Empty)
                {
                    txtBankAcctNo.Text = BankAcct;
                }

                //IP - 26/01/11 - Sprint 5.9 - #2785
                if (CardNo != string.Empty && (PayMethod.IsPayMethod(curPayMethod, PayMethod.CreditCard) || PayMethod.IsPayMethod(curPayMethod, PayMethod.DebitCard)))
                {
                    mtb_CardNo.Text = CardNo;
                }

                if (CardNo != string.Empty && PayMethod.IsPayMethod(curPayMethod, PayMethod.Cheque))
                {
                    txtChequeNo.Text = CardNo;
                }

            }

        }

        //IP - 26/02/10 - CR1072 - Malaysia 3PL for Version 5.2
        private void drpBranchForDel_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckDHLScheduled();

            //if (thirdPartyDeliveriesEnabled && !scheduleDelEnabled && _printBranches != null)
            //{
            //    cbImmediate.Enabled = false;

            //    delNoteBranch = Convert.ToInt16(drpBranchForDel.Text);

            //    WS2.BranchDefaultPrintLocation details = _printBranches.Find(delegate(WS2.BranchDefaultPrintLocation item) { return item.dpl == delNoteBranch; });

            //    //If the selected Del Note Branch is setup as a Third Party Deliveries Warehouse then this will be a scheduled delivery
            //    if (details != null)
            //    {
            //        cbImmediate.Checked = false;
            //    }
            //    else
            //    {
            //        cbImmediate.Checked = true;
            //    }

            //}
        }


        private void CheckDHLScheduled()
        {
            if (thirdPartyDeliveriesEnabled && !scheduleDelEnabled && _printBranches != null)
            {
                WS2.BranchDefaultPrintLocation details = _printBranches.Find(delegate (WS2.BranchDefaultPrintLocation item) { return item.dpl == Convert.ToInt16(drpBranchForDel.Text); });

                //If the selected Del Note Branch is setup as a Third Party Deliveries Warehouse then this will be a scheduled delivery
                if (drpBranchForDel.Text == drpBranch.Text)
                {
                    cbImmediate.Checked = true;
                    cbImmediate.Enabled = false;
                }
                else
                {
                    if (details != null)
                    {
                        cbImmediate.Checked = false;
                        cbImmediate.Enabled = false;
                    }
                    else
                    {
                        cbImmediate.Enabled = true;
                    }
                }
            }
        }




        //IP - 26/01/11 - Sprint 5.9 - Method to clear payment fields when changing payment methods.
        private void clearPaymentFields()
        {
            //IP - 31/01/11 - Sprint 5.9 - #3050 - selected index should be set to 0 if datasource is not null
            if (drpBank.DataSource != null)
            {
                drpBank.SelectedIndex = 0;
            }

            txtBankAcctNo.Text = string.Empty;
            txtChequeNo.Text = string.Empty;
            mtb_CardNo.ResetText();
            txtStoreCardBal.Text = string.Empty;
            storeCardValidated = null;
        }

        //IP - 26/01/11 - Sprint 5.9 - #2785
        private void mtb_CardNo_TextChanged(object sender, EventArgs e)
        {
            errorProviderStoreCard.SetError(btnStoreCardManualEntry, "");
            //Only retrieve the last 4 digits and set the txtCardNo as the contents of this control are used to save card/cheque details.
            txtChequeNo.Text = mtb_CardNo.Text.Trim();
            if (!CashAndGoReturn)
            {
                if (PayMethod.IsPayMethod(Convert.ToInt16(((DataRowView)drpPayMethod.SelectedItem)[CN.Code].ToString()), PayMethod.StoreCard))
                {
                    if (mtb_CardNo.Text.Length == 16)
                        ValidateStoreCard();
                    else
                    {
                        ResetStoreCard(false);
                        btnPrintReceipt.Enabled = false;
                        errorProviderStoreCard.SetError(txtAmount, string.Empty);
                    }
                }
            }
        }

        //private void drpDeliveryAddress_SelectedIndexChanged(object sender, EventArgs e)
        //{

        //}

        //IP - 10/06/11 - CR1212 - RI - #3817 - Process Account Wide Discount
        //If replace is false then the account wide discount is being added as a new discount, else the discount has been found
        //and must be recalculated and replaced for all instances.
        public bool ProcessAccountWideDiscount(bool replace)
        {
            decimal discountVal = 0;
            decimal totItemsVal = 0;
            decimal itemDiscountAmt = 0;
            XmlNode itemForDiscount = null;
            acctWideDisAuthorised = false;
            acctWideDisCancel = false;
            acctWideDisRequireAuth = false;
            decimal totItemTaxVal = 0;                          //IP - 16/04/12 - #8613

            //First we need to select the total value of the stock items/components that we wish to link the discount to

            foreach (XmlNode item in itemDoc.DocumentElement)
            {
                if (item.Attributes[Tags.Type].Value == "Stock")
                {
                    totItemsVal = totItemsVal + Convert.ToDecimal(item.Attributes[Tags.Value].Value);
                    totItemTaxVal = totItemTaxVal + Convert.ToDecimal(item.Attributes[Tags.TaxAmount].Value);               //IP - 16/04/12 - #8613
                }

                foreach (XmlNode child in item.ChildNodes)
                {
                    if (child.NodeType == XmlNodeType.Element && child.HasChildNodes) //RelatedItem node
                    {
                        foreach (XmlNode childNodes in child.ChildNodes)
                        {
                            if (childNodes.NodeType == XmlNodeType.Element && childNodes.HasChildNodes) //Component node
                            {
                                if (childNodes.Attributes[Tags.Type].Value == "Component")
                                {
                                    totItemsVal = totItemsVal + Convert.ToDecimal(childNodes.Attributes[Tags.Value].Value);
                                    totItemTaxVal = totItemTaxVal + Convert.ToDecimal(childNodes.Attributes[Tags.TaxAmount].Value); //IP - 16/04/12 - #8613
                                }
                            }
                        }
                    }
                }
            }

            if ((string)Country[CountryParameterNames.AgreementTaxType] == "I")             //IP - 16/04/12 - #8613
            {
                totItemTaxVal = 0;
            }

            //if (Convert.ToDecimal(Math.Abs(Convert.ToDecimal(currentItem.Attributes[Tags.Value].Value))) > (totItemsVal + totItemTaxVal))                  //IP - 16/04/12 - #8613
            //{
            //    errorProvider1.SetError(btnEnter, "Discount value cannot be greater than the item you are attempting to discount");
            //    status = false;
            //}
            //else
            //{

            var discountRate = (decimal)Country[CountryParameterNames.DiscountPercentage] / 100;

            if (percentage)
            {
                // RI remove % sign and calc percentage value
                currentItem.Attributes[Tags.Value].Value = currentItem.Attributes[Tags.Value].Value.Replace("%", "");
                discountVal = Convert.ToDecimal(currentItem.Attributes[Tags.Value].Value) / 100 * (totItemsVal + totItemTaxVal);                         //IP - 16/04/12 - #8613

                if (Math.Abs(discountVal) > (totItemsVal + totItemTaxVal))
                {
                    errorProvider1.SetError(btnEnter, "Discount value cannot be greater than the item you are attempting to discount");
                    return false;
                }

            }
            else
            {
                discountVal = Convert.ToDecimal(currentItem.Attributes[Tags.Value].Value);

                if (Math.Abs(discountVal) > (totItemsVal + totItemTaxVal))
                {
                    errorProvider1.SetError(btnEnter, "Discount value cannot be greater than the item you are attempting to discount");
                    return false;
                }
            }

            //#13791
            if (Math.Abs(discountVal) > ((totItemsVal + totItemTaxVal) * discountRate))
            {
                acctWideDisRequireAuth = true;
            }


            foreach (XmlNode item in itemDoc.DocumentElement)
            {

                if ((item.Attributes[Tags.Type].Value == "Stock" || item.Attributes[Tags.Type].Value == "Kit") && Convert.ToString(item.Attributes[Tags.Quantity].Value) != "0")
                {
                    currentItem.Attributes[Tags.TaxAmount].Value = Convert.ToString("0");                   //IP - 16/04/12 - #8613

                    if (item.Attributes[Tags.Type].Value == "Stock") //Setup the discount to be attached to a stock item
                    {
                        //Work out the weighted portion of the discount value for the item and update the xml
                        itemDiscountAmt = Math.Round(discountVal * Convert.ToDecimal(item.Attributes[Tags.Value].Value) / totItemsVal, 2);
                        currentItem.Attributes[Tags.Value].Value = Convert.ToString(itemDiscountAmt);
                        currentItem.Attributes[Tags.UnitPrice].Value = Convert.ToString(itemDiscountAmt);

                        itemForDiscount = item;

                        if (replace) //If we are replacing if the ParentItemId and ParentItemNo tags exist then we set them 
                        {
                            if (currentItem.Attributes[Tags.ParentItemId] != null && currentItem.Attributes[Tags.ParentItemNo] != null)
                            {
                                currentItem.Attributes[Tags.ParentItemId].Value = Convert.ToString(itemForDiscount.Attributes[Tags.ItemId].Value);
                                currentItem.Attributes[Tags.ParentItemNo].Value = Convert.ToString(itemForDiscount.Attributes[Tags.Code].Value);
                            }
                        }

                        AddAccountWideDiscount(currentItem, itemForDiscount, replace); //Call method to add the discount to the stock item in the xml
                    }
                    else //Setup discounts to be attached to kit components
                    {
                        foreach (XmlNode child in item.ChildNodes)
                        {
                            if (child.NodeType == XmlNodeType.Element && child.HasChildNodes) //RelatedItem node
                            {
                                foreach (XmlNode childNodes in child.ChildNodes)
                                {
                                    if (childNodes.NodeType == XmlNodeType.Element && childNodes.HasChildNodes) //Component node
                                    {
                                        if (childNodes.Attributes[Tags.Type].Value == "Component")
                                        {
                                            itemDiscountAmt = Math.Round(discountVal * Convert.ToDecimal(childNodes.Attributes[Tags.Value].Value) / totItemsVal, 2);
                                            currentItem.Attributes[Tags.Value].Value = Convert.ToString(itemDiscountAmt);
                                            currentItem.Attributes[Tags.UnitPrice].Value = Convert.ToString(itemDiscountAmt);

                                            itemForDiscount = childNodes;

                                            if (replace) //If we are replacing if the ParentItemId and ParentItemNo tags exist then we set them 
                                            {
                                                if (currentItem.Attributes[Tags.ParentItemId] != null && currentItem.Attributes[Tags.ParentItemNo] != null)
                                                {
                                                    currentItem.Attributes[Tags.ParentItemId].Value = Convert.ToString(itemForDiscount.Attributes[Tags.ItemId].Value);
                                                    currentItem.Attributes[Tags.ParentItemNo].Value = Convert.ToString(itemForDiscount.Attributes[Tags.Code].Value);
                                                }
                                            }

                                            AddAccountWideDiscount(currentItem, itemForDiscount, replace); //Call method to add the discount to the kit component item in the xml
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
            }
            // }

            return true;                  //IP - 16/04/12 - #8613
        }

        //IP - 01/06/11 - CR1212 - RI - #2315
        public void AddAccountWideDiscount(XmlNode currentItem, XmlNode itemForDiscount, bool replace)
        {
            decimal newValue = 0;
            decimal valueTax = 0;
            bool status = true;

            //IP - 16/04/12 - #8613 - Moved the below check on authorisation to within AuthoriseDiscount to prevent authorisation popup if already authorised.
            //if (!acctWideDisAuthorised)     //IP - 09/06/11 - CR1212 - RI - #3817 - Only want to authorise adding an account wide discount once.
            //{
            status = this.AuthoriseDiscount(currentItem, itemForDiscount);
            //}

            //IP - 03/10/11 - RI - #8132 - Moved from below as need to update discount value before it is added to the item
            if (status)
            {
                /* discount taxrate must = taxrate of the item it discounts */
                /* if taxrate has changed we must recalculate the tax amount */
                if (currentItem.Attributes[Tags.TaxRate].Value != itemForDiscount.Attributes[Tags.TaxRate].Value)
                {
                    currentItem.Attributes[Tags.TaxRate].Value = itemForDiscount.Attributes[Tags.TaxRate].Value;
                    currentItem.Attributes[Tags.TaxAmount].Value = "0"; //setting tax rate to 0 -- this allows recalculation of correct amount. 
                                                                        //if agreements exclude tax i.e. there is a seperate tax line then value of line is reduced by tax amount


                    if ((string)Country[CountryParameterNames.AgreementTaxType] == "E")
                    {
                        decimal oldTax = Convert.ToDecimal(currentItem.Attributes[Tags.TaxAmount].Value);
                        newValue = Convert.ToDecimal(currentItem.Attributes[Tags.Value].Value);
                        newValue += oldTax;//So the original value was the value + previous tax
                        currentItem.Attributes[Tags.Value].Value = newValue.ToString();
                        currentItem.Attributes[Tags.UnitPrice].Value = newValue.ToString();
                        currentItem.Attributes[Tags.TaxAmount].Value = "0"; // setting value to 0 as separate tax line
                    }

                    valueTax = AccountManager.CalculateTaxAmount(Config.CountryCode, currentItem, chxTaxExempt.Checked, out Error);
                    if (Error.Length > 0)
                    {
                        ShowError(Error);
                        status = false;
                    }
                    else
                    {
                        if ((string)Country[CountryParameterNames.AgreementTaxType] == "E")
                        {
                            newValue -= valueTax; //  take off newly calculated tax from the value as value excludes tax
                            currentItem.Attributes[Tags.Value].Value = newValue.ToString();
                            currentItem.Attributes[Tags.UnitPrice].Value = newValue.ToString();
                        }
                        // always store the tax amount even if sales include tax.... 
                        currentItem.Attributes[Tags.TaxAmount].Value = valueTax.ToString();

                    }
                }

            }

            if (status)
            {
                XmlNode field = null;

                if (itemForDiscount.Attributes[Tags.Type].Value == "Stock" || itemForDiscount.Attributes[Tags.Type].Value == "Component")
                {
                    //#14049 - Moved check to here frok below
                    var itemKey = Convert.ToString(currentItem.Attributes[Tags.ItemId].Value) + "|" + Convert.ToString(currentItem.Attributes[Tags.Location].Value);

                    var xPath = "./Item[@Key= '" + itemKey + "' and @Quantity !=0]";

                    XmlNode discountToReplace = itemForDiscount.SelectSingleNode(Elements.RelatedItem).SelectSingleNode(xPath);

                    if (!replace || discountToReplace == null) //#14049 //If we are adding a new account wide discount 
                    {
                        foreach (XmlNode child in itemForDiscount.ChildNodes)
                        {
                            if (child.NodeType == XmlNodeType.Element && child.Name == Elements.RelatedItem)
                                field = child;
                        }

                        if (field.NodeType == XmlNodeType.Element)
                        {
                            if (field.Name == Elements.RelatedItem)
                            {
                                field.AppendChild(currentItem.Clone());
                            }
                        }
                    }
                    else //Replacing an existing account wide discount
                    {
                        //#14049 - Moved check to above.
                        //var itemKey = Convert.ToString(currentItem.Attributes[Tags.ItemId].Value) + "|" + Convert.ToString(currentItem.Attributes[Tags.Location].Value);

                        //var xPath = "./Item[@Key= '" + itemKey + "' and @Quantity !=0]";

                        //XmlNode discountToReplace = itemForDiscount.SelectSingleNode(Elements.RelatedItem).SelectSingleNode(xPath);

                        xml.replaceItem(discountToReplace, currentItem.Clone());

                    }
                }

                //IP - 03/10/11 - #8132 - Moved to above before adding/replacing account wide discount
                ///* discount taxrate must = taxrate of the item it discounts */
                ///* if taxrate has changed we must recalculate the tax amount */
                //if (currentItem.Attributes[Tags.TaxRate].Value != itemForDiscount.Attributes[Tags.TaxRate].Value)
                //{
                //    currentItem.Attributes[Tags.TaxRate].Value = itemForDiscount.Attributes[Tags.TaxRate].Value;
                //    currentItem.Attributes[Tags.TaxAmount].Value = "0"; //setting tax rate to 0 -- this allows recalculation of correct amount. 
                //    //if agreements exclude tax i.e. there is a seperate tax line then value of line is reduced by tax amount

                //IP - 03/10/11 - #8132 - Moved to above before adding/replacing account wide discount
                ///* discount taxrate must = taxrate of the item it discounts */
                ///* if taxrate has changed we must recalculate the tax amount */
                //if (currentItem.Attributes[Tags.TaxRate].Value != itemForDiscount.Attributes[Tags.TaxRate].Value)
                //{
                //    currentItem.Attributes[Tags.TaxRate].Value = itemForDiscount.Attributes[Tags.TaxRate].Value;
                //    currentItem.Attributes[Tags.TaxAmount].Value = "0"; //setting tax rate to 0 -- this allows recalculation of correct amount. 
                //    //if agreements exclude tax i.e. there is a seperate tax line then value of line is reduced by tax amount

                //    if ((string)Country[CountryParameterNames.AgreementTaxType] == "E")
                //    {
                //        decimal oldTax = Convert.ToDecimal(currentItem.Attributes[Tags.TaxAmount].Value);
                //        newValue = Convert.ToDecimal(currentItem.Attributes[Tags.Value].Value);
                //        newValue += oldTax;//So the original value was the value + previous tax
                //        currentItem.Attributes[Tags.Value].Value = newValue.ToString();
                //        currentItem.Attributes[Tags.UnitPrice].Value = newValue.ToString();
                //        currentItem.Attributes[Tags.TaxAmount].Value = "0"; // setting value to 0 as separate tax line
                //    }

                //    if ((string)Country[CountryParameterNames.AgreementTaxType] == "E")
                //    {
                //        decimal oldTax = Convert.ToDecimal(currentItem.Attributes[Tags.TaxAmount].Value);
                //        newValue = Convert.ToDecimal(currentItem.Attributes[Tags.Value].Value);
                //        newValue += oldTax;//So the original value was the value + previous tax
                //        currentItem.Attributes[Tags.Value].Value = newValue.ToString();
                //        currentItem.Attributes[Tags.UnitPrice].Value = newValue.ToString();
                //        currentItem.Attributes[Tags.TaxAmount].Value = "0"; // setting value to 0 as separate tax line
                //    }

                //    valueTax = AccountManager.CalculateTaxAmount(Config.CountryCode, currentItem, chxTaxExempt.Checked, out Error);
                //    if (Error.Length > 0)
                //    {
                //        ShowError(Error);
                //        status = false;
                //    }
                //    else
                //    {
                //        if ((string)Country[CountryParameterNames.AgreementTaxType] == "E")
                //        {
                //            newValue -= valueTax; //  take off newly calculated tax from the value as value excludes tax
                //            currentItem.Attributes[Tags.Value].Value = newValue.ToString();
                //            currentItem.Attributes[Tags.UnitPrice].Value = newValue.ToString();
                //        }
                //        // always store the tax amount even if sales include tax.... 
                //        currentItem.Attributes[Tags.TaxAmount].Value = valueTax.ToString();

                //    }
                //}

            }
        }

        //IP - 09/06/11 - CR1212 - RI - Method returns if selected discount is an Account Wide Discount
        public bool isAcctWideDiscount(string category)
        {
            bool isAcctWideDis = true;

            foreach (DataRow row in ((DataTable)StaticData.Tables[TN.DiscountLinks]).Rows)
            {
                if (Convert.ToString(row[CN.Reference]) == category)
                {
                    isAcctWideDis = false;
                }
            }

            return isAcctWideDis;
        }

        private void txtAmount_TextChanged(object sender, EventArgs e)
        {
            if (loaded && PayMethod.IsPayMethod(Convert.ToInt16(((DataRowView)drpPayMethod.SelectedItem)[CN.Code].ToString()), PayMethod.StoreCard) && !CashAndGoReturn)
                StoreCardCheckPayButton();
        }

        //IP - 28/05/12 - #10225
        private void cbTime_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Convert.ToString(cbTime.SelectedItem) == "AM")
            {
                dtDeliveryRequired.Value = dtDeliveryRequired.Value;

            }
            else
            {
                dtDeliveryRequired.Value = dtDeliveryRequired.Value.AddHours(12);
            }
        }

        private void drpDeliveryArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            errorProvider1.SetError(drpDeliveryArea, "");
        }

        //#12224 - CR12249
        private void btnDelAreas_Click(object sender, EventArgs e)
        {
            DeliveryAreaPopup delArea = new DeliveryAreaPopup(FormRoot, FormParent, this.delAreas, drpDeliveryArea.Text);
            delArea.ShowDialog();

            if (delArea.SelectedDeliveryArea != string.Empty)
            {
                var index = drpDeliveryArea.FindStringExact(delArea.SelectedDeliveryArea);
                if (index > 0) drpDeliveryArea.SelectedIndex = index;
            }
        }

        //#16607
        private void PopulateWarrantyReturnDictionary()
        {
            List<WarrantyReturnList> warranties = new List<WarrantyReturnList>();
            var items = itemsTable;

            foreach (DataRow dr in items.Rows)
            {
                if (WarrantyType.IsFree(Convert.ToString(dr["WarrantyType"])) == false && Convert.ToString(dr["contractno"]) != "") //#17712  //#17883
                {
                    warranties.Add(new WarrantyReturnList
                    {
                        warrantyNumber = Convert.ToString(dr["ProductCode"]),
                        warrantyItemID = Convert.ToInt32(dr["ItemID"]),
                        contractNo = Convert.ToString(dr["ContractNo"]),
                        stocklocn = Convert.ToInt16(dr["StockLocation"])
                    });
                }
            }

            //#16607
            Client.Call(new GetManyWarrantyReturnRequest
            {
                returnInputs = warranties
            }, response =>
            {
                if (response != null && response.returnDetails != null) //#16290
                {
                    foreach (var item in response.returnDetails)
                    {
                        if (item.warrantyReturn != null)
                        {
                            var details = new WarrantyReturnDetails();

                            warrantyReturnPC.TryGetValue(new DictionaryKey(Convert.ToString(item.warrantyReturn.WarrantyNo), Convert.ToString(item.WarrantyContractNo)), out details);   // #17506

                            if (details == null)
                            {
                                warrantyReturnPC.Add(new DictionaryKey(item.warrantyReturn.WarrantyNo, Convert.ToString(item.WarrantyContractNo)), item);        // #17506
                            }
                        }
                    }

                    ProcessWarrantyRefund();  //#16607
                }
                else
                {
                    //Possibly need to do something
                }
            }, this);
        }

        //#16607 - Method to adjust the refund amount to reflect the used portion of the warranties based on warranty 
        //return percentages. This will then be used to post the relevant CRE / CRF transactions to Fintrans.
        private void ProcessWarrantyRefund()
        {
            warrantyRefunds.Clear();
            warrantyRefund = false;
            var refundAmt = MoneyStrToDecimal(txtAmount.Text);

            foreach (DataRow item in itemsTable.Rows)
            {
                if (Convert.ToString(item["WarrantyType"]) != WarrantyType.Free && Convert.ToString(item["WarrantyType"]) != string.Empty)
                {
                    var details = new WarrantyReturnDetails();

                    warrantyReturnPC.TryGetValue(new DictionaryKey(Convert.ToString(item["ProductCode"]), Convert.ToString(item["ContractNo"])), out details);

                    if (details != null)
                    {
                        if ((100 - details.warrantyReturn.PercentageReturn) > 0)
                        {
                            //Select the Product Category of the item linked to the warranty
                            string xPath = "//Item[@ItemId = '" + Convert.ToInt32(item["ParentItemId"]) + "' and @Quantity != 0]";
                            XmlNode parentItem = itemDoc.DocumentElement.SelectSingleNode(xPath);

                            var refund = Math.Round(Convert.ToDecimal(MoneyStrToDecimal(Convert.ToString(item["Value"])) * ((100 - details.warrantyReturn.PercentageReturn) / 100)), 2);

                            refundAmt -= refund;

                            warrantyRefunds.Add(new WarrantyRefund
                            {
                                Refund = refund,
                                RefundType = (parentItem != null ? parentItem.Attributes[Tags.ProductCategory].Value : Convert.ToString(item["ProductCategory"])) == "PCF" ? "F" : "E"
                            });

                            warrantyRefund = true;
                        }

                    }
                }
            }

            if (warrantyRefund)
            {
                txtAmount.Text = refundAmt.ToString(DecimalPlaces);
                txtTendered.Text = refundAmt.ToString(DecimalPlaces);
                txtDue.Text = refundAmt.ToString(DecimalPlaces);
                ShowInfo("M_WARRANTYREFUND");
            }
        }


        //#18604 - CR15594 - Check if Ready Assist item exists on account
        public void findReadyAssist(XmlNode relatedItems)
        {
            readyAssistExists = false;
            readyAssistLength = null;

            var xPath = "";
            var result = 0;

            foreach (DataRow row in readyAssistTerms.Rows)
            {
                var rdyAssistItem = Convert.ToString(row["Code"]); //Get the itemcode e.g. READY1, READY2, READY3   

                xPath = "//Item[@Quantity!='0' and @ReadyAssist='True' and @Code='" + rdyAssistItem + "']";

                XmlNode rdyAssistNode = relatedItems.SelectSingleNode(xPath);

                if (rdyAssistNode != null)
                {
                    readyAssistExists = true;
                    var parsed = int.TryParse(row["reference"].ToString(), out result);

                    if (parsed)
                    {
                        readyAssistLength = result;
                    }

                    break;
                }
            }

        }

        private void chxNonStock_CheckedChanged(object sender, EventArgs e)
        {
            ClearItemDetails();
            if (chxNonStock.Checked)
            {
                chxNonStock.Enabled = false;
            }
        }

        private void chxCOD_CheckedChanged(object sender, EventArgs e)
        {

        }

        public static void ApplyTaxInTaxInclusiveCountries(WarrantyResult warrantiesObj, string taxType, decimal taxRate)
        {
            if (taxType == "I" && taxRate > 0)
            {
                var taxMultCoeficient = 1 + (taxRate / 100.0M);

                if (warrantiesObj != null && warrantiesObj.Items != null)
                {
                    foreach (var warrantyItem in warrantiesObj.Items)
                    {
                        if (warrantyItem.price != null)
                        {
                            warrantyItem.price.RetailPrice *= taxMultCoeficient;
                        }
                    }
                }

            }
        }

    }
}
