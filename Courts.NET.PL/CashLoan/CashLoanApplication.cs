using Blue.Cosacs.Shared;
using Blue.Cosacs.Shared.Services;
using Blue.Cosacs.Shared.Services.Credit;
using STL.Common;
using STL.Common.Constants.CashLoans;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.SanctionStages;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.Values;
using STL.Common.Static;
using STL.PL.Utils;
using System;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Xml;


namespace STL.PL.CashLoan
{
    public partial class CashLoanApplication : CommonForm
    {
        private DataSet custAddresses;
        private Crownwood.Magic.Controls.TabPage currentTab;
        private bool loading = false;
        private bool callCalcLoanRepayment = true;
        private bool BlockedloanAlready = false;//prevents error if updating twice using linq. 
        private bool dalAuth = false;
        private int maxPctRFavail = 0;
        private int minLoanAmt = 0;
        private int maxLoanAmt = 0;
        private int maxLoanAmtNewCustomer = 0;
        private int maxLoanAmtRecentCustomer = 0;
        private int maxLoanAmtStaffCustomer = 0;
        private double servicePercent = 0;
        private decimal insPercent = 0;
        private decimal adminPercent = 0;
        private string validCustid = "";
        private new string Error = String.Empty;
        private string acctno = "000000000000";
        private decimal taxRate = 0;
        private string termsType = String.Empty;
        private bool referral = false;
        private Crownwood.Magic.Controls.TabPage docTab;
        bool displayMessages = true;
        private int existCashLoan = 0;
        private string cashLoanBlocked = string.Empty;
        private bool showDet = false;
        bool refer = false;
        bool exceedAvail = false;
        StringBuilder refReasonsSB = new StringBuilder();                        //IP - 24/02/12 - #9598 - UAT87
        private DataSet dropDownDS = null;                                       //#19337 - CR18568
        private bool enableCashLoanPurpose = false;                              //#19337 - CR18568
        private bool ReviseCashLoanDisbursLimitsEnable = false;

        STL.PL.WS2.CashLoanDetails det = null;
        TermsTypeAllBands selectedTermsType = null;
        UserRight allowDisburseCashLoan = UserRight.Create("AllowDisburseCashLoan");
        UserRight allowCashLoanSetup = UserRight.Create("AllowApplication");

        private bool autoAccept = false;
		public XmlDocument itemDoc;

        bool isMmiAppliedForSaleAtrr = false;
        decimal mmiLimit = 0;
        decimal mmiThreshold = 0;
        decimal mmiThresholdLimit = 0;
        bool isMmiCalculated = false;
        bool isMmiAllowed = false;
        decimal creditLimit = 0;
        bool isReferedForMMI = false;
        bool isMambuAccount = false;
        public bool disbursementBtnEnable
        {
            get { return Convert.ToBoolean(btnDisburse.Enabled); }
            set { btnDisburse.Enabled = value; }
        }

        public bool udLoanAmountEnable
        {
            get { return Convert.ToBoolean(udLoanAmount.Enabled); }
            set { udLoanAmount.Enabled = value; }
        }

        public bool udMonthsEnable
        {
            get { return Convert.ToBoolean(udMonths.Enabled); }
            set { udMonths.Enabled = value; }
        }

        public bool cmbTermsTypeEnable
        {
            get { return Convert.ToBoolean(cmbTermsType.Enabled); }
            set { cmbTermsType.Enabled = value; }
        }

        public bool btnPrintPromissoryEnable
        {
            get { return Convert.ToBoolean(btnPrint.Enabled); }
            set { btnPrint.Enabled = value; }
        }

        public bool numAdminChgEnable
        {
            get { return Convert.ToBoolean(numAdminChg.Enabled); }
            set { numAdminChg.Enabled = value; }
        }

        public enum ReferralMessages
        {

        }

        private bool waivAdminCharge = false;
        private int? empeenoAdminChargeWaived;
        private int? empeenoLoanAmountChanged;
        private Customer cust = null;
        private bool deliveryAuthorised = false;

        //set to true when loan amount / term length / terms type changed as we want to bypass authorisation prompt to appear.
        //This should ONLY appear when the user makes a change to admin charge and not by re-calculation based on a change to the mentioned.
        private bool byPassAuthorisation = false;
        private bool byPassLoanAmountAuthorisation = false;
        private string defaultScoreBand = string.Empty;
        private string LoanQual = string.Empty;

        double MaximumLimitForCustomer = 0;
        double CustomerMaxLimit = 0;

        public CashLoanApplication(Form root, Form parent)
        {
            InitializeComponent();

            FormRoot = root;
            FormParent = parent;
            btnSearch.Enabled = false;   // #9733
            btnAccept.Enabled = false;
            btnDisburse.Enabled = false;
            btnCancel.Enabled = true;
            btnPrint.Enabled = false;
            btnUpdateBankDetails.Enabled = false;
            btnAccountDetails.Enabled = false;

            tcAddress.Visible = false;
            lblMMI.Visible = false;
            txtMMI.Visible = false;

            maxPctRFavail = Convert.ToInt32(Country[CountryParameterNames.CL_MaxPctRFavail]);
            minLoanAmt = Convert.ToInt32(Country[CountryParameterNames.CL_MinLoanAmount]);
            maxLoanAmt = Convert.ToInt32(Country[CountryParameterNames.CL_MaxLoanAmount]);
            maxLoanAmtNewCustomer = Convert.ToInt32(Country[CountryParameterNames.CL_NewCustMaxLoanAmount]);
            maxLoanAmtRecentCustomer = Convert.ToInt32(Country[CountryParameterNames.CL_RecentCustMaxLoanAmount]);
            maxLoanAmtStaffCustomer = Convert.ToInt32(Country[CountryParameterNames.CL_StaffCustMaxLoanAmount]);
            taxRate = Convert.ToDecimal(Country[CountryParameterNames.TaxRate]);
            enableCashLoanPurpose = Convert.ToBoolean(Country[CountryParameterNames.CL_EnablePurposeDropDown]);         //#19337 - CR18568
            defaultScoreBand = Convert.ToString(Country[CountryParameterNames.TermsTypeBandDefault]);
            isMmiAllowed = Convert.ToBoolean(Country[CountryParameterNames.EnableMMI]);
            ReviseCashLoanDisbursLimitsEnable = Convert.ToBoolean(Country[CountryParameterNames.ReviseCashLoanDisbursLimits]);

            base.CheckUserRights(allowDisburseCashLoan);
            base.CheckUserRights(allowCashLoanSetup);

            LoadStatic();

            //#19337 - CR18568
            if (enableCashLoanPurpose)
            {
                lblCashLoanPurpose.Visible = true;
                drpCashLoanPurpose.Visible = true;

                if (drpCashLoanPurpose.Items.Count > 0)
                {
                    drpCashLoanPurpose.SelectedIndex = -1;
                }
                else
                {
                    MessageBox.Show("No reasons have been entered in Code Maintenance");
                }

            }
            else
            {
                lblCashLoanPurpose.Visible = false;
                drpCashLoanPurpose.Visible = false;
            }

            //Hide sales tax label & text box
            if ((bool)Country[CountryParameterNames.CL_Amortized])
            {
                lblSalesTax.Visible = false;
                txtSalesTax.Visible = false;

                numAdminChg.Enabled = false;
                numAdminChg.ReadOnly = true;
            }
            if (isMmiAllowed)
            {
                lblMMI.Visible = true;
                txtMMI.Visible = true;
            }
        }


        private void txtCustId_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                if (Validate() == true)
                {
                    CheckQual();
                }
            }
            else // prevent running twice
            {
                if (e.KeyChar == (char)Keys.Tab)
                {
                    if (Validate() == true)
                    {
                        loading = true;
                        CheckQual();
                        loading = false;
                    }
                }
            }

        }


        private void CheckQual()
        {
            BlockedloanAlready = false;
            LoanQual = string.Empty;
            Client.Call(new CashLoanQualificationRequest
            {
                CustId = txtCustId.Text,
                Branch = Convert.ToInt32(Config.BranchCode)
            },
                response =>
                {
                    loading = true;
                    if (response.Qualified && (response.Customer.CashLoanBlocked != CashLoanBlockedStatus.Blocked || response.Cashloan == null) //#8768 #8626 - Check if credit is blocked
                            && response.LoanAllowed)     //#8852 - Check if Loan account can be created
                    {
                        var loanAmt = 0;
                        if (response.IsMambuAccount)
                            isMambuAccount = true;
                        else
                            isMambuAccount = false;

                        if (!isMambuAccount)
                        {
                            MessageBox.Show("Legacy Customers are not allowed for Cash Loan", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        if (response.Cashloan != null)                                    //#8558
                        {
                            loanAmt = Convert.ToInt32(response.Cashloan.LoanAmount);

                            //If the Admin Charge was previously waived then we want to use the changed value
                            //so that the Admin Charge does not become recalculated.
                            waivAdminCharge = response.Cashloan.AdminChargeWaived;
                            empeenoAdminChargeWaived = response.Cashloan.EmpeenoAdminChargeWaived;

                            if (response.Cashloan.AdminCharge.HasValue && waivAdminCharge == true)
                            {
                                numAdminChg.Value = response.Cashloan.AdminCharge.Value;
                            }

                            if (response.Cashloan.LoanStatus != CashLoanStatus.Cancelled)
                            {
                                acctno = response.Cashloan.AcctNo;                  // #8761 set acctno 
                            }

                        }
                        if (!isMambuAccount) 
                        {
                            referral = displayReferralReasons(response.Referral, response.Customer.CashLoanBlocked, Convert.ToInt32(response.Customer.AvailableSpend),
                                           loanAmt);

                            // #8670 if Refer button chosen in referral screen - Create account
                            if (refer)
                            {
                                ((MainForm)FormRoot).StatusBarText = "Customer has been Referred";
                                Refer(response);        // Create account     
                            }
                        }
                        if (!referral || showDet)       //#8558
                        {
                            PopulateScreen(response);
                            SetCtrlForOnlyDisbursementRight();
                        }
                    }
                    else
                    {

                        if (response.IsMambuAccount)
                            isMambuAccount = true;
                        else
                            isMambuAccount = false;

                        if (!isMambuAccount)
                        {
                            MessageBox.Show("Legacy Customers are not allowed for Cash Loan", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        if (response.LoanQual == "X")
                        {
                            errorProvider1.SetError(txtCustId, "Customer ID not found");
                            btnSearch.Enabled = false;       // #9733
                        }
                        else if (response.Customer.CashLoanBlocked == CashLoanBlockedStatus.Blocked)  //#8626 
                        {
                            bool Return;
                            StringBuilder sb = new StringBuilder();
                            sb.Append("Cash Loan has been Referred and blocked for this Customer");
                            sb.Append(Environment.NewLine);
                            sb.Append(Environment.NewLine);
                            sb.Append("Please refer to the Credit Department.");
                            //MessageBox.Show("Customers Credit is currently blocked", "Credit Blocked", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            CashLoanReferral referpopup = new CashLoanReferral(Convert.ToString(sb), Convert.ToString(CashLoanBlockedStatus.Blocked), out Return);    // #8761
                            referpopup.ShowDialog();
                            //refer = referpopup.Return;
                        }

                        else if (response.Qualified && !response.LoanAllowed)          // RF accounts not able to be created 
                        {
                            MessageBox.Show("Cash Loan accounts are not allowed at this branch", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }

                        else
                        {
                            // #cr-7- extend referrals to cash loans
                            PopulateScreen(response);
                            SetCtrlForOnlyDisbursementRight();
                            LoanQual = response.LoanQual;
                            //lblNotQual.Visible = true;
                            //referral = displayReferralReasons(response.Referral, response.Customer.CashLoanBlocked);
                        }

                    }
                    loading = false;
                    txtCustId.Enabled = true;       // #13323   
                    if (response.IsMambuAccount)
                    {
                        txtAgrmtTotal.Enabled = false;
                        txtInstalment.Enabled = false;
                        txtFinInstal.Enabled = false;
                        numAdminChg.Enabled = false;
                        txtSalesTax.Enabled = false;
                        txtInsuranceChg.Enabled = false;
                        txtServiceChg.Enabled = false;
                        udLoanAmount.Enabled = false;
                        udMonths.Enabled = false;
                        btnPrint.Enabled = false;
                        btnDisburse.Enabled = false;
                        cmbTermsType.Enabled = false;
                        ((MainForm)FormRoot).StatusBarText = "Account awaiting disbursement.";
                    }
                    
                },
                this);

        }

        private void SetCtrlForOnlyDisbursementRight()
        {
            if (allowCashLoanSetup.IsAllowed == false) //user only has disbursement right
            {
                cmbTermsType.Enabled = false;
                udLoanAmount.Enabled = false;
                udMonths.Enabled = false;
                btnAccept.Enabled = false;
                //btnprint.enabled = false;             //#15282
            }
        }

        // Create a Loan Account
        private void CreateAccount()
        {
            det.empeenoAccept = Credential.UserId;
            det.loanAmount = udLoanAmount.Value;
            det.term = Convert.ToInt32(udMonths.Value);
            det.instalment = Convert.ToDecimal(txtInstalment.Text);
            det.finInstal = Convert.ToDecimal(txtFinInstal.Text);
            det.serviceChg = Convert.ToDecimal(txtServiceChg.Text);
            det.insuranceChg = Convert.ToDecimal(txtInsuranceChg.Text);
            det.adminChg = numAdminChg.Value;
            det.agreementTotal = Convert.ToDecimal(txtAgrmtTotal.Text);
            det.accountNo = acctno;
            det.custId = txtCustId.Text;
            det.custName = txtFirstName.Text + " " + txtLastName.Text;
            det.loanStatus = string.Empty;
            if (selectedTermsType != null)
                det.termsType = selectedTermsType.TermsType;
            det.datePrinted = null;
            det.cashLoanPurpose = drpCashLoanPurpose.Visible && (DataRowView)drpCashLoanPurpose.SelectedItem != null ? (string)((DataRowView)drpCashLoanPurpose.SelectedItem)[CN.CodeDescription] : null;  //#19560
            det.empeenoAdminChargeWaived = empeenoAdminChargeWaived;
            det.empeenoLoanAmountChanged = empeenoLoanAmountChanged;
            det.Bank = chkEnableEBT.Checked ? ((DataRowView)drpBank.SelectedItem)[CN.BankCode].ToString() : null;
            det.BankAccountType = chkEnableEBT.Checked ? ((DataRowView)drpBankAccountType.SelectedItem)[CN.Code].ToString() : null;
            det.BankAccountNo = chkEnableEBT.Checked ? txtBankAcctNo.Text.Trim() : null;
            det.BankBranch = chkEnableEBT.Checked ? txtBankBranch.Text.Trim() : null;
            det.Notes = chkEnableEBT.Checked ? txtNotes.Text.Trim() : null;
            det.BankReferenceNumber = chkEnableEBT.Checked ? txtBankReferenceNo.Text.Trim() : null;
            det.BankAccountName = chkEnableEBT.Checked ? txtBankAccountName.Text.Trim() : null;

            var rescore = false;
            var reOpenS1 = false;
            var reOpenS2 = false;
            acctno = AccountManager.CreateRFAccount(Config.CountryCode, Convert.ToInt16(Config.BranchCode),
                                this.txtCustId.Text, Credential.UserId, true, ref det, out rescore,
                                out reOpenS1, out reOpenS2, out Error);


        }

        private string CustomerCashLoanType(Customer customer)
        {
            var customerType = string.Empty;

            if (customer.CashLoanExisting)
            {
                customerType = "Existing Customer";
            }
            else if (customer.CashLoanRecent)
            {
                customerType = "Recent Customer";
            }
            else if (customer.CashLoanNew)
            {
                customerType = "New Customer";
            }
            else if (customer.CashLoanStaff)
            {
                customerType = "Staff";
            }

            return customerType;
        }

        private void PopulateScreen(CashLoanQualificationResponse customerDetails)
        {

            if (customerDetails.TermsType.Count == 0)
            {
                if(string.IsNullOrEmpty(customerDetails.Customer.ScoringBand))
                {
                    ShowInfo("M_CUSTOMERWITHNOSCORINGBAND");
                    return;
                }

                var customerType = CustomerCashLoanType(customerDetails.Customer);
                ShowInfo("M_NOCASHLOANTERMSTYPE", new object[] { customerType });
                return;
            }

            det = new STL.PL.WS2.CashLoanDetails();

            cmbTermsType.Enabled = true;

            cmbTermsType.Set(customerDetails.TermsType, "Description");
            //var exceedAvail = false;
            var lowAvailable = false;
            var maxLoanAvail = 0;

            //For legacy cash loan accounts & stamp duty is applicable
            if (!(bool)Country[CountryParameterNames.CL_Amortized] && customerDetails.StampDuty > 0)          // #10013
            {
                lblSalesTax.Text = "Sales Tax + StampDuty";
            }

            //If this is not an existing Cash Loan then default the Terms Type drop down to the first available Cash Loan Terms Type
            if (customerDetails.Cashloan == null)
            {
                cmbTermsType.SelectedIndex = 0;

                drpCashLoanPurpose.SelectedIndex = -1;   //#19337 - CR18568
            }
            else
            {
                cmbTermsType.SelectedItem = customerDetails.TermsType.Find(t => t.TermsType == customerDetails.Cashloan.TermsType);

                if (customerDetails.Cashloan.CashLoanPurpose != null)           //#19337 - CR18568
                {
                    drpCashLoanPurpose.Text = customerDetails.Cashloan.CashLoanPurpose;
                }
            }

            selectedTermsType = (TermsTypeAllBands)cmbTermsType.SelectedItem;

            txtCustId.Enabled = false;
            validCustid = txtCustId.Text;
            cust = customerDetails.Customer;
            deliveryAuthorised = customerDetails.DAed;
            cashLoanBlocked = cust.CashLoanBlocked;
            //var Terms = customerDetails.TermsType;
            var loan = customerDetails.Cashloan;
            txtFirstName.Text = cust.firstname;
            txtLastName.Text = cust.name;
            txtScoreBand.Text = cust.ScoringBand;
            creditLimit = cust.RFCreditLimit;
            if (txtScoreBand.Text == string.Empty)
            {
                txtScoreBand.Text = defaultScoreBand;
            }

            det.custId = txtCustId.Text;
            det.custName = txtFirstName.Text + " " + txtLastName.Text;
            det.stampDuty = customerDetails.StampDuty;          // #10013
            det.cashLoanPurpose = drpCashLoanPurpose.Visible && (DataRowView)drpCashLoanPurpose.SelectedItem != null ? (string)((DataRowView)drpCashLoanPurpose.SelectedItem)[CN.CodeDescription] : null;   //#19560

            if (selectedTermsType != null)
            {
                txtServiceChargePct.Text = Convert.ToString(selectedTermsType.ServPcent);
                servicePercent = selectedTermsType.ServPcent;
           
                insPercent = Convert.ToDecimal(selectedTermsType.InsPcent);
                adminPercent = Convert.ToDecimal(selectedTermsType.AdminPcent);
                termsType = selectedTermsType.TermsType;
            }
            udLoanAmount.Enabled = true;
            udMonths.Enabled = true;
            if ((bool)Country[CountryParameterNames.CL_Amortized])
            {
                numAdminChg.Enabled = false;
                numAdminChg.ReadOnly = true;
            }

            if (selectedTermsType != null)
            {
                udMonths.Maximum = selectedTermsType.MaxTerm;
                udMonths.Minimum = selectedTermsType.MinTerm;
                udMonths.Value = selectedTermsType.MaxTerm;         // #8780
            }
            if (isMambuAccount && (cust.AvailableSpend < 0))
            {
                
                det.loanAmount = Convert.ToDecimal(loan.LoanAmount);
                cust.AvailableSpend = det.loanAmount - (cust.AvailableSpend);
            }
            if (isMambuAccount && (cust.AvailableSpend < loan.LoanAmount))
            { 
                det.loanAmount = Convert.ToDecimal(loan.LoanAmount);

                cust.AvailableSpend = det.loanAmount + (cust.AvailableSpend);
            }
            if (ReviseCashLoanDisbursLimitsEnable)
            {
                MaximumLimitForCustomer = cust.CashLoanExisting ? maxLoanAmt : cust.CashLoanRecent ? maxLoanAmtRecentCustomer : cust.CashLoanNew ? maxLoanAmtNewCustomer : cust.CashLoanStaff ? maxLoanAmtStaffCustomer : maxLoanAmt;
                CustomerMaxLimit = Convert.ToInt32(cust.AvailableSpend * maxPctRFavail / 100);

                if (customerDetails.LoanQual == "N" && customerDetails.Cashloan == null)
                {
                    existCashLoan = Convert.ToInt32(customerDetails.TotalCreditUsed);
                }

                if (CustomerMaxLimit < MaximumLimitForCustomer)
                {
                    maxLoanAvail = Convert.ToInt32(cust.AvailableSpend - existCashLoan);
                }

                if (CustomerMaxLimit > MaximumLimitForCustomer)
                {
                    maxLoanAvail = Convert.ToInt32(MaximumLimitForCustomer);
                }
            }
            else
            {
                maxLoanAvail = Convert.ToInt32((cust.RFCreditLimit * maxPctRFavail / 100) - existCashLoan);
            }

            if (loan != null)
            {
                acctno = loan.AcctNo;           // #8670 set acctno so that existing account is used

                if (loan.Bank != string.Empty && loan.Bank != null)
                {
                    chkEnableEBT.Checked = true;
                    btnUpdateBankDetails.Enabled = true;

                    drpBank.SelectedValue = det.Bank = loan.Bank.Trim();
                    drpBankAccountType.SelectedValue = det.BankAccountType = loan.BankAccountType.Trim();
                    txtBankAcctNo.Text = det.BankAccountNo = loan.BankAcctNo.Trim();
                    txtBankBranch.Text = det.BankBranch = loan.BankBranch.Trim();
                    txtNotes.Text = det.Notes = loan.Notes.Trim();
                    txtBankReferenceNo.Text = det.BankReferenceNumber = loan.BankReferenceNo.Trim();
                    txtBankAccountName.Text = det.BankAccountName = loan.BankAccountName.Trim();
                }
            }

            if (cust.AvailableSpend < minLoanAmt || maxLoanAvail < minLoanAmt)
            {

                if (loan != null)
                {
                    loan.LoanStatus = CashLoanStatus.LowAvailableSpend;

                    UpdateCashLoanStatus(loan);

                    udLoanAmount.Minimum = det.loanAmount;
                    udLoanAmount.Maximum = det.loanAmount;
                }
                else
                {

                    udLoanAmount.Minimum = 0;
                    udLoanAmount.Maximum = 0;
                }

                btnAccept.Enabled = false;
                btnDisburse.Enabled = false;
                btnPrint.Enabled = false;
                udLoanAmount.Enabled = false;
                udMonths.Enabled = false;
                cmbTermsType.Enabled = false;

                lowAvailable = true;
            }
            else
            {
                if (loan != null && loan.LoanStatus == CashLoanStatus.LowAvailableSpend)
                {
                    loan.LoanStatus = "";

                    UpdateCashLoanStatus(loan);
                }



                if (ReviseCashLoanDisbursLimitsEnable)
                {
                    if (CustomerMaxLimit < MaximumLimitForCustomer)
                    {
                        udLoanAmount.Maximum = Convert.ToInt32(cust.AvailableSpend - existCashLoan);
                    }

                    if (CustomerMaxLimit > MaximumLimitForCustomer)
                    {
                        udLoanAmount.Maximum = Convert.ToInt32(MaximumLimitForCustomer);
                    }
                }
                else
                {
                    udLoanAmount.Maximum = Convert.ToInt32(cust.RFCreditLimit * maxPctRFavail / 100);
                }


                if ((cust.RFCreditLimit * maxPctRFavail / 100 > maxLoanAmt && cust.CashLoanExisting) ||
                        (cust.RFCreditLimit * maxPctRFavail / 100 > maxLoanAmtRecentCustomer && cust.CashLoanRecent) ||
                            (cust.RFCreditLimit * maxPctRFavail / 100 > maxLoanAmtNewCustomer && cust.CashLoanNew) ||
                                 (cust.RFCreditLimit * maxPctRFavail / 100 > maxLoanAmtStaffCustomer && cust.CashLoanStaff)
                                  || (cust.RFCreditLimit * maxPctRFavail / 100) > cust.AvailableSpend)  //#8600
                {


                    if (ReviseCashLoanDisbursLimitsEnable)
                    {
                        if (CustomerMaxLimit < MaximumLimitForCustomer)
                        {
                            udLoanAmount.Maximum = Convert.ToInt32(cust.AvailableSpend - existCashLoan);
                        }

                        if (CustomerMaxLimit > MaximumLimitForCustomer)
                        {
                            udLoanAmount.Maximum = Convert.ToInt32(MaximumLimitForCustomer);
                        }
                    }
                    else
                    {
                        udLoanAmount.Maximum = cust.CashLoanExisting ? maxLoanAmt : cust.CashLoanRecent ? maxLoanAmtRecentCustomer : cust.CashLoanNew ? maxLoanAmtNewCustomer : cust.CashLoanStaff ? maxLoanAmtStaffCustomer : maxLoanAmt;
                    }


                    // check for % RF allocaetd to existing loans   #8586

                    if (ReviseCashLoanDisbursLimitsEnable)
                    {
                        if (CustomerMaxLimit < MaximumLimitForCustomer)
                        {
                            udLoanAmount.Maximum = Convert.ToInt32(cust.AvailableSpend - existCashLoan);
                            udLoanAmount.Value = Convert.ToInt32(cust.AvailableSpend - existCashLoan);
                        }

                        if (CustomerMaxLimit > MaximumLimitForCustomer)
                        {
                            udLoanAmount.Maximum = Convert.ToInt32(MaximumLimitForCustomer);
                            udLoanAmount.Value = Convert.ToInt32(MaximumLimitForCustomer);
                        }
                    }
                    else
                    {
                        if ((cust.RFCreditLimit * maxPctRFavail / 100) - (existCashLoan) < udLoanAmount.Maximum)
                        {
                            udLoanAmount.Maximum = (cust.RFCreditLimit * maxPctRFavail / 100) - existCashLoan;
                        }

                        if (cust.AvailableSpend < udLoanAmount.Maximum) //Available spend less than maximum
                        {
                            udLoanAmount.Maximum = Convert.ToInt32(Math.Floor(cust.AvailableSpend));
                            udLoanAmount.Value = Convert.ToInt32(Math.Floor(cust.AvailableSpend));
                        }
                    }
                }
                //else
                //{
                //udLoanAmount.Maximum = Convert.ToInt32(cust.RFCreditLimit * maxPctRFavail / 100);

                if (loan != null && loan.EmpeenoLoanAmountChanged != null && loan.LoanAmount.HasValue)
                {
                    empeenoLoanAmountChanged = loan.EmpeenoLoanAmountChanged;

                    det.empeenoLoanAmountChanged = empeenoLoanAmountChanged;

                    LoanAmountChangeAuthorised(loan.LoanAmount.Value);
                }

                if (loan != null && loan.LoanAmount > udLoanAmount.Maximum)
                {
                    errorProviderForWarning.SetError(udLoanAmount, "Pending Loan Amount exceeds current maximum avaliable - details updated");
                    exceedAvail = true;
                }
                //else
                //{
                //    udLoanAmount.Maximum = Convert.ToInt32(cust.RFCreditLimit* maxPctRFavail / 100);
                //}
                //}

                if (loan != null && loan.LoanAmount < minLoanAmt)
                {
                    udLoanAmount.Minimum = Convert.ToDecimal(loan.LoanAmount);
                }
                else
                {
                    udLoanAmount.Minimum = minLoanAmt;
                }
            }


            if (loan != null && loan.LoanStatus != CashLoanStatus.Referred                  // #8670 and not referred
                        && loan.LoanStatus != CashLoanStatus.LowAvailableSpend)           // #8680 and not low spend
            {

                det.empeenoAccept = loan.EmpeenoAccept;
                det.empeenoDisburse = loan.EmpeenoDisburse;
                det.loanStatus = loan.LoanStatus;
                det.datePrinted = Convert.ToDateTime(loan.DatePrinted);           //  #8491

                acctno = loan.AcctNo;
                if (exceedAvail == false)
                {
                    udLoanAmount.Value = Convert.ToDecimal(loan.LoanAmount);
                }
                else
                {
                    udLoanAmount.Value = udLoanAmount.Maximum;
                }

                udMonths.Value = Convert.ToDecimal(loan.Term);

                btnAccept.Enabled = false;                                  // #8765 existing loan, only enable if something changes
                //btnAccept.Enabled = true;                   // #8744
                btnAccountDetails.Enabled = true;

                if (allowDisburseCashLoan.IsAllowed)
                {
                    btnPrint.Enabled = true;
                }

                if (customerDetails.DAed != true)
                {
                    btnDisburse.Enabled = false;
                    ((MainForm)FormRoot).StatusBarText = "Account must be Delivery Authorised before disbursing the loan";

                }
                else
                {
                    ////if (allowDisburseCashLoan.IsAllowed)            //Previously only allowed to print Promissiory Note after DA, now required before DA.
                    ////{
                    ////    //btnDisburse.Enabled = true;
                    ////    btnPrint.Enabled = true;
                    ////}

                    if (det.loanStatus == CashLoanStatus.PromissoryPrinted && det.datePrinted == DateTime.Today && allowDisburseCashLoan.IsAllowed) //#15298 // #8491 promissiory note must be printed today
                    {
                        btnDisburse.Enabled = true;
                        ((MainForm)FormRoot).StatusBarText = "Account awaiting disbursement.";
                    }
                    else
                    {
                        if (det.loanStatus == CashLoanStatus.PromissoryPrinted && det.datePrinted == DateTime.Today)
                        {
                            ((MainForm)FormRoot).StatusBarText = "Promissory Note printed. Account awaiting disbursement.";  //#15298
                        }
                        else
                        {
                            ((MainForm)FormRoot).StatusBarText = "Account awaiting disbursement. Print Promissory Note.";
                        }
                    }
                }

                dalAuth = customerDetails.DAed;                             //Account Da'ed?                      

            }
            else
            {
                if (!lowAvailable)
                {
                    udLoanAmount.Value = udLoanAmount.Maximum;
                    //udMonths.Value = Terms.MaxTerm;
                    if (selectedTermsType != null)
                         udMonths.Value = selectedTermsType.MaxTerm;
                    btnAccept.Enabled = true;

                    //if (cust.AvailableSpend < udLoanAmount.Maximum) //Available spend less than maximum
                    //{
                    //    udLoanAmount.Maximum = Convert.ToInt32(cust.AvailableSpend);
                    //    udLoanAmount.Value = Convert.ToInt32(cust.AvailableSpend);
                    //}
                    if (maxLoanAvail < udLoanAmount.Maximum) // #8853 Max loan avail less than maximum
                    {
                        udLoanAmount.Maximum = Convert.ToInt32(maxLoanAvail);
                        udLoanAmount.Value = Convert.ToInt32(maxLoanAvail);
                    }
                }
            }

            //txtLoanRFSpend.Text = Convert.ToString(Math.Round(udLoanAmount.Value));
            txtLoanRFSpend.Text = Convert.ToString(Math.Round(udLoanAmount.Maximum));

            CalcLoanRepayment(true);
            SetMmiValue();

            if (loading)
            {
                loadAddresses(true);
            }

            txtCustId.Enabled = true;
            //btnAccept.Enabled = true;

            // Pending loan exceeds current max available - force accept to update pending details
            if ((exceedAvail && !lowAvailable)                          // and not < min loan
                    || (loan != null && loan.LoanStatus == CashLoanStatus.Referred))        // #8761   or previously referred
            {
                autoAccept = true;              //#19560

                btnAccept_Click(null, null);
                btnAccept.Enabled = false;              // #8761
                exceedAvail = false;    // #8747 jec 28/11/11
                dalAuth = false;        // #8828 jec 05/12/11
                btnPrint.Enabled = false;              // #8828 jec 05/12/11
                btnDisburse.Enabled = false;           // #8828 jec 05/12/11
            }
            else
            {
                if (lowAvailable && displayMessages)
                {
                    displayMessages = false;

                    bool Return;
                    StringBuilder sb = new StringBuilder();

                    if (loan != null)
                    {
                        //MessageBox.Show("Available Spend is less than the pending loan amount. Please refer to the Credit Department", "Referral Reasons", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        sb.Append("Available Spend is less than the pending loan amount.");     // #8780
                    }
                    else
                    {
                        //MessageBox.Show("Available Spend for Cash Loan is less than the minimum loan amount. Please refer to the Credit Department", "Referral Reasons", MessageBoxButtons.OK, MessageBoxIcon.Information); //#8614
                        sb.Append("Available Spend for Cash Loan is less than the minimum loan amount.");       // #8780
                    }

                    sb.Append(Environment.NewLine);
                    sb.Append(Environment.NewLine);
                    sb.Append("Please refer to the Credit Department.");
                    CashLoanReferral referpopup = new CashLoanReferral(Convert.ToString(sb), Convert.ToString(CashLoanStatus.LowAvailableSpend), out Return);    // #8780
                    referpopup.ShowDialog();

                }
            }

        }

        private void loadAddresses(bool readOnly)
        {
            Function = "LoadAddresses";
            tcAddress.Visible = true;
            string mobileNo = String.Empty;
            string mobileNo2 = String.Empty;
            string mobileNo3 = String.Empty;
            string mobileNo4 = String.Empty;
            string addDesc = String.Empty;
            string addType = string.Empty;

            //IP - 17/03/11 - #3317 - CR1245
            string workDialCode2 = string.Empty;
            string workNum2 = string.Empty;
            string workExt2 = string.Empty;

            string workDialCode3 = string.Empty;
            string workNum3 = string.Empty;
            string workExt3 = string.Empty;

            string workDialCode4 = string.Empty;
            string workNum4 = string.Empty;
            string workExt4 = string.Empty;

            // Address Standardization CR2019 - 025
            string deliveryMobile = String.Empty;
            string delivery1Mobile = String.Empty;
            string delivery2Mobile = String.Empty;
            string delivery3Mobile = String.Empty;




            var tabcount = tcAddress.TabPages.Count - 1;
            if (tcAddress.TabPages.Count > 0)
            {
                for (int i = tabcount; i >= 0; i--)
                {
                    tcAddress.TabPages.RemoveAt(0);
                }
            }

            custAddresses = CustomerManager.GetCustomerAddresses(txtCustId.Text, out Error);

            if (custAddresses != null)
            {
                StringCollection tabs = new StringCollection();
                foreach (DataRow row in custAddresses.Tables[TN.CustomerAddresses].Rows)
                {
                    addType = ((string)row["AddressType"]).Trim();
                    if (row["AddressDescription"] != DBNull.Value)
                        addDesc = ((string)row["AddressDescription"]).Trim();
                    else
                        addDesc = String.Empty;

                    if (addType == GetResource("L_MOBILE") && row["Phone"] != DBNull.Value)
                        mobileNo = ((string)row["Phone"]).Trim();

                    if (addType == GetResource("L_MOBILE2") && row["Phone"] != DBNull.Value)
                        mobileNo2 = ((string)row["Phone"]).Trim();

                    if (addType == GetResource("L_MOBILE3") && row["Phone"] != DBNull.Value)
                        mobileNo3 = ((string)row["Phone"]).Trim();

                    if (addType == GetResource("L_MOBILE4") && row["Phone"] != DBNull.Value)
                        mobileNo4 = ((string)row["Phone"]).Trim();

                    // Address Standardization CR2019 - 025
                    if (addType == GetResource("L_DMOBILE") && row["Phone"] != DBNull.Value)
                        deliveryMobile = ((string)row["Phone"]).Trim();

                    if (addType == GetResource("L_D1MOBILE") && row["Phone"] != DBNull.Value)
                        delivery1Mobile = ((string)row["Phone"]).Trim();

                    if (addType == GetResource("L_D2MOBILE") && row["Phone"] != DBNull.Value)
                        delivery2Mobile = ((string)row["Phone"]).Trim();

                    if (addType == GetResource("L_D3MOBILE") && row["Phone"] != DBNull.Value)
                        delivery3Mobile = ((string)row["Phone"]).Trim();


                    if (addType == GetResource("L_WORK2") && row["DialCode"] != DBNull.Value)
                        workDialCode2 = ((string)row["DialCode"]).Trim();

                    if (addType == GetResource("L_WORK2") && row["Phone"] != DBNull.Value)
                        workNum2 = ((string)row["Phone"]).Trim();

                    if (addType == GetResource("L_WORK2") && row["Ext"] != DBNull.Value)
                        workExt2 = ((string)row["Ext"]).Trim();

                    if (addType == GetResource("L_WORK3") && row["DialCode"] != DBNull.Value)
                        workDialCode3 = ((string)row["DialCode"]).Trim();

                    if (addType == GetResource("L_WORK3") && row["Phone"] != DBNull.Value)
                        workNum3 = ((string)row["Phone"]).Trim();

                    if (addType == GetResource("L_WORK3") && row["Ext"] != DBNull.Value)
                        workExt3 = ((string)row["Ext"]).Trim();

                    if (addType == GetResource("L_WORK4") && row["DialCode"] != DBNull.Value)
                        workDialCode4 = ((string)row["DialCode"]).Trim();

                    if (addType == GetResource("L_WORK4") && row["Phone"] != DBNull.Value)
                        workNum4 = ((string)row["Phone"]).Trim();

                    if (addType == GetResource("L_WORK4") && row["Ext"] != DBNull.Value)
                        workExt4 = ((string)row["Ext"]).Trim();

                    //IP - 17/03/11 - #3317 - CR1245 - Don't add seperate Work tabs for W2, W3, W4
                    if (!tabs.Contains(addType) && addType != GetResource("L_MOBILE") && addType != GetResource("L_MOBILE2") && addType != GetResource("L_MOBILE3") && addType != GetResource("L_MOBILE4")
                        && addType != GetResource("L_WORK2") && addType != GetResource("L_WORK3") && addType != GetResource("L_WORK4") && addType != GetResource("L_DMOBILE") && addType != GetResource("L_D1MOBILE") && addType != GetResource("L_D2MOBILE") && addType != GetResource("L_D3MOBILE"))	//if this is a new tab
                    {
                        tabs.Add(addType);
                        Crownwood.Magic.Controls.TabPage tp = new Crownwood.Magic.Controls.TabPage(addDesc);
                        currentTab = tp;
                        tp.Tag = false;
                        tp.BorderStyle = BorderStyle.Fixed3D;
                        AddressTab at = new AddressTab(readOnly, FormRoot, addType);     //CR1084 jec
                        at.Enable = false; // Address Standardization CR2019 - 025     

                        this.tcAddress.TabPages.Add(tp);

                        tp.Controls.Add(at);
                        tp.Name = "tp" + addType;
                    }

                    foreach (Crownwood.Magic.Controls.TabPage t in tcAddress.TabPages)
                    {
                        if (t.Name == "tp" + addType)
                        {
                            if (row["DELFirstname"] != DBNull.Value) // Address Standardization CR2019 - 025
                                ((AddressTab)t.Controls[0]).CFirstname.Text = (string)row["DELFirstname"];
                            if (row["DELLastname"] != DBNull.Value) // Address Standardization CR2019 - 025
                                ((AddressTab)t.Controls[0]).CLastname.Text = (string)row["DELLastname"];
                            if (row["Address1"] != DBNull.Value) // Address Standardization CR2019 - 025
                                ((AddressTab)t.Controls[0]).txtAddress1.Text = (string)row["Address1"];
                            if (((AddressTab)t.Controls[0]).cmbVillage.Items.Count > 0 &&
                                    row["Address2"] != DBNull.Value)
                            {
                                var villageIndex = ((AddressTab)t.Controls[0]).cmbVillage.FindStringExact((string)row["Address2"]);
                                if (villageIndex != -1)
                                    ((AddressTab)t.Controls[0]).cmbVillage.SelectedIndex = villageIndex;
                                else
                                    ((AddressTab)t.Controls[0]).cmbVillage.SelectedText = (string)row["Address2"];
                            }
                            else if (row["Address2"] != DBNull.Value) // Address Standardization CR2019 - 025
                                ((AddressTab)t.Controls[0]).cmbVillage.SelectedText = (string)row["Address2"];
                            if (((AddressTab)t.Controls[0]).cmbRegion.Items.Count > 0 &&
                            row["Address3"] != DBNull.Value) // Address Standardization CR2019 - 025
                            {
                                var regionIndex = ((AddressTab)t.Controls[0]).cmbRegion.FindStringExact((string)row["Address3"]);
                                if (regionIndex != -1)
                                    ((AddressTab)t.Controls[0]).cmbRegion.SelectedIndex = regionIndex;
                                else
                                    ((AddressTab)t.Controls[0]).cmbRegion.SelectedText = (string)row["Address3"];
                            }
                            else if (row["Address3"] != DBNull.Value) // Address Standardization CR2019 - 025
                                ((AddressTab)t.Controls[0]).cmbRegion.SelectedText = (string)row["Address3"];
                            if (row["PostCode"] != DBNull.Value)
                                ((AddressTab)t.Controls[0]).txtPostCode.Text = (string)row["PostCode"];
                            if (!Convert.IsDBNull(row["Latitude"]) && !Convert.IsDBNull(row["Longitude"]))
                                ((AddressTab)t.Controls[0]).txtCoordinate.Text = string.Format("{0},{1}", row["Latitude"].ToString(), row["Longitude"].ToString()); // Address Standardization CR2019 - 025
                            if (((AddressTab)t.Controls[0]).txtNotes.Text.Length == 0 && row["Notes"] != DBNull.Value)
                                ((AddressTab)t.Controls[0]).txtNotes.Text = (string)row["Notes"];
                            if (((AddressTab)t.Controls[0]).txtEmail.Text.Length == 0)
                                ((AddressTab)t.Controls[0]).txtEmail.Text = (string)row["Email"];
                            if (((AddressTab)t.Controls[0]).txtDialCode.Text.Length == 0)
                                ((AddressTab)t.Controls[0]).txtDialCode.Text = (string)row["DialCode"];
                            if (((AddressTab)t.Controls[0]).txtPhoneNo.Text.Length == 0)
                                ((AddressTab)t.Controls[0]).txtPhoneNo.Text = (string)row["Phone"];
                            if (((AddressTab)t.Controls[0]).txtExtension.Text.Length == 0)
                                if (DBNull.Value != row["Ext"])
                                    ((AddressTab)t.Controls[0]).txtExtension.Text = (string)row["Ext"];
                            if (!Convert.IsDBNull(row["Date In"]))
                                ((AddressTab)t.Controls[0]).dtDateIn.Value = (DateTime)row["Date In"];
                            // Delivery Area
                            ((AddressTab)t.Controls[0]).SetDeliveryArea((string)row[CN.DeliveryArea]);

                            ((AddressTab)t.Controls[0]).CFirstname.TabStop = false;
                            ((AddressTab)t.Controls[0]).CLastname.TabStop = false;
                            ((AddressTab)t.Controls[0]).txtAddress1.TabStop = false;
                            ((AddressTab)t.Controls[0]).cmbVillage.TabStop = true; // Address Standardization CR2019 - 025
                            ((AddressTab)t.Controls[0]).cmbRegion.TabStop = true; // Address Standardization CR2019 - 025
                            ((AddressTab)t.Controls[0]).txtPostCode.TabStop = false;
                            ((AddressTab)t.Controls[0]).txtNotes.TabStop = false;
                            ((AddressTab)t.Controls[0]).txtEmail.TabStop = false;
                            ((AddressTab)t.Controls[0]).txtDialCode.TabStop = false;
                            ((AddressTab)t.Controls[0]).txtPhoneNo.TabStop = false;
                            ((AddressTab)t.Controls[0]).txtExtension.TabStop = false;
                            ((AddressTab)t.Controls[0]).txtDialCode.TabStop = false;
                            ((AddressTab)t.Controls[0]).txtMobile.TabStop = false;

                            //Disable Mobile No field if not Home tab
                            if (addType == "W")// Address Standardization CR2019 - 025
                            {
                                ((AddressTab)t.Controls[0]).txtMobile.Enabled = false;
                                ((AddressTab)t.Controls[0]).btnMobile.Visible = false;

                            }
                            //IP - 17/03/11 - #3317 - CR1245 - Do not display the button to add multiple work numbers if this is not the work address tab
                            if (addType != "W")
                            {
                                ((AddressTab)t.Controls[0]).btnWork.Visible = false;
                            }
                        }
                    }

                    //Display Mobile No on the Home Address Tab.
                    addType = "H";
                    foreach (Crownwood.Magic.Controls.TabPage t in tcAddress.TabPages)
                    {
                        if (t.Name == "tp" + addType)
                        {
                            ((AddressTab)t.Controls[0]).txtMobile.Text = mobileNo;
                            ((AddressTab)t.Controls[0]).txtMobile2.Text = mobileNo2;
                            ((AddressTab)t.Controls[0]).txtMobile3.Text = mobileNo3;
                            ((AddressTab)t.Controls[0]).txtMobile4.Text = mobileNo4;                           

                        }
                        //  Display Mobile No on the Delivery Tab
                        if (t.Name == "tpD") // Address Standardization CR2019 - 025
                        {
                            ((AddressTab)t.Controls[0]).txtMobile.Text = deliveryMobile;
                            ((AddressTab)t.Controls[0]).btnMobile.Visible = false;
                        }

                        //  Display Mobile No on the Delivery1 Tab
                        if (t.Name == "tpD1")
                        {
                            ((AddressTab)t.Controls[0]).txtMobile.Text = delivery1Mobile;
                            ((AddressTab)t.Controls[0]).btnMobile.Visible = false;
                        }

                        //  Display Mobile No on the Delivery2 Tab
                        if (t.Name == "tpD2")
                        {
                            ((AddressTab)t.Controls[0]).txtMobile.Text = delivery2Mobile;
                            ((AddressTab)t.Controls[0]).btnMobile.Visible = false;
                        }

                        //  Display Mobile No on the Delivery3 Tab
                        if (t.Name == "tpD3")
                        {
                            ((AddressTab)t.Controls[0]).txtMobile.Text = delivery3Mobile;
                            ((AddressTab)t.Controls[0]).btnMobile.Visible = false;
                        }
                    }
                }

                tabs.Clear();

                //IP - 17/03/11 - #3317 - CR1245 - Display any additional work numbers on the Work Address tab
                addType = "W";

                foreach (Crownwood.Magic.Controls.TabPage t in tcAddress.TabPages)
                {
                    if (t.Name == "tp" + addType)
                    {
                        ((AddressTab)t.Controls[0]).txtWorkDialCode2.Text = workDialCode2;
                        ((AddressTab)t.Controls[0]).txtWorkNum2.Text = workNum2;
                        ((AddressTab)t.Controls[0]).txtWorkExt2.Text = workExt2;

                        ((AddressTab)t.Controls[0]).txtWorkDialCode3.Text = workDialCode3;
                        ((AddressTab)t.Controls[0]).txtWorkNum3.Text = workNum3;
                        ((AddressTab)t.Controls[0]).txtWorkExt3.Text = workExt3;

                        ((AddressTab)t.Controls[0]).txtWorkDialCode4.Text = workDialCode4;
                        ((AddressTab)t.Controls[0]).txtWorkNum4.Text = workNum4;
                        ((AddressTab)t.Controls[0]).txtWorkExt4.Text = workExt4;
                    }
                }
            }

            Function = "End of LoadAddresses";
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            //btnPrint.Enabled = true;

            refer = false;
            byPassAuthorisation = true;
            byPassLoanAmountAuthorisation = true;
            if (txtCustId.Enabled && Validate() == true)
            {
                txtCustId.Enabled = false;
                CheckQual();
            }

            byPassAuthorisation = false;
            byPassLoanAmountAuthorisation = false;
        }

        private void txtCustId_Leave(object sender, EventArgs e)
        {
            if (validCustid == "" || validCustid.Trim() != txtCustId.Text.Trim())
            {
                btnSearch_Click(null, null);
                validCustid = txtCustId.Text;
                btnSearch.Enabled = true;       // #9733
            }

        }

        private bool Validate()// Fix for Issue #6398662 : Cancel Button not working
        {
            bool valid = true;
            errorProvider1.SetError(txtCustId, "");

            ResetScreen();

            if (txtCustId.Text.Trim().Length == 0)
            {
                errorProvider1.SetError(txtCustId, "Enter a valid Customer ID");
                valid = false;
            }


            return valid;
        }

        private void udLoanAmount_ValueChanged(object sender, EventArgs e)
        {
            if (!loading)   //Do not call the below method whilst popualting the screen.
            {
                byPassAuthorisation = true;

                dalAuth = false;

                udLoanAmount.Value = Convert.ToInt32(udLoanAmount.Value);       // ensure value does not contain decimals

                CalcLoanRepayment();

                btnAccept.Enabled = true;
                btnDisburse.Enabled = false;
                btnPrint.Enabled = false;

                byPassAuthorisation = false;
            }
        }

        private void udMonths_ValueChanged(object sender, EventArgs e)
        {
            if (!loading) //Do not call the below method whilst populating the screen
            {
                // current system supports maximum 60 months term length.
                int terms = Convert.ToInt32(udMonths.Value);
                if (terms > VL.MaxInstallmentTerms)
                {
                    ShowInfo("M_MAXTERMS", new object[] { VL.MaxInstallmentTerms });
                    return;
                }

                byPassAuthorisation = true;
                dalAuth = false;

                CalcLoanRepayment();

                btnAccept.Enabled = true;
                btnDisburse.Enabled = false;
                btnPrint.Enabled = false;

                byPassAuthorisation = false;
            }
        }

        private void CalcLoanRepayment(bool calledFromPopulateScreen = false)
        {
            if (udMonths.Value > 0 && udLoanAmount.Value > 0 && callCalcLoanRepayment == true)
            {
                //decimal serviceChg = 0;
                decimal instalment = 0;
                decimal finInstalment = 0;
                if (selectedTermsType != null)
                     det.termsType = selectedTermsType.TermsType;
                //det.scoreBand = txtScoreBand.Text;
                det.scoreBand = txtScoreBand.Text == string.Empty ? defaultScoreBand : txtScoreBand.Text;
                det.accountNo = acctno;
                det.loanAmount = udLoanAmount.Value;
                det.term = Convert.ToInt32(udMonths.Value);
                det.taxRate = taxRate;
                det.adminChg = numAdminChg.Value;
                det.waiveAdminCharge = waivAdminCharge;
                if (det.termsType != string.Empty)
                {
                    Error = AccountManager.CalculateCashLoanTerms(Config.CountryCode, ref det, Convert.ToInt16(Config.BranchCode), calledFromPopulateScreen,isMambuAccount);
                }
                instalment = det.instalment;
                finInstalment = det.finInstal;
                if (Convert.ToDouble(udMonths.Value) == 1)
                {
                    instalment = finInstalment;
                    finInstalment = 0;
                }

                txtServiceChg.Text = Convert.ToString(det.serviceChg);
                txtInstalment.Text = Convert.ToString(det.instalment);
                txtFinInstal.Text = Convert.ToString(det.finInstal);
                numAdminChg.Value = det.adminChg;
                txtInsuranceChg.Text = Convert.ToString(det.insuranceChg);
                txtSalesTax.Text = Convert.ToString(Math.Round(det.insuranceTax + det.adminTax + det.stampDuty, 2));       // #10013
                txtAgrmtTotal.Text = Convert.ToString(Math.Round(det.agreementTotal, 2));
            }
        }

        private void ResetScreen()
        {
            errorProvider1.SetError(txtCustId, "");
            errorProvider1.SetError(btnDisburse, "");
            errorProviderForWarning.SetError(udLoanAmount, "");
            errorProvider1.SetError(drpCashLoanPurpose, "");            //#19337 - CR18568

            errorProviderBankAcctNo.SetError(txtBankAcctNo, string.Empty);
            errorProviderBank.SetError(drpBank, string.Empty);
            errorProviderBankAccountType.SetError(drpBankAccountType, string.Empty);
            errorProviderBankBranch.SetError(txtBankBranch, string.Empty);
            errorProviderBankReference.SetError(txtBankReferenceNo, string.Empty);
            errorProviderNameOnAccount.SetError(txtBankAccountName, string.Empty);

            ((MainForm)FormRoot).StatusBarText = "";

            lblNotQual.Visible = false;

            // remove address tabs - need txtCustId.Enabled = false to avoid firing leave event when removing tabs
            txtCustId.Enabled = false;
            var tabcount = tcAddress.TabPages.Count - 1;
            if (tcAddress.TabPages.Count > 0)
            {
                for (int i = tabcount; i >= 0; i--)
                {
                    tcAddress.TabPages.RemoveAt(0);
                }
            }

            byPassAuthorisation = true;

            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtLoanRFSpend.Text = "";
            txtServiceChargePct.Text = "";
            txtScoreBand.Text = "";
            txtInstalment.Text = "";
            txtFinInstal.Text = "";
            txtAgrmtTotal.Text = "";
            txtServiceChg.Text = "";
            numAdminChg.Value = 0;
            txtInsuranceChg.Text = "";
            txtSalesTax.Text = "";
            txtMMI.Text = string.Empty;
            udLoanAmount.Minimum = 0;
            udLoanAmount.Value = 0;
            udMonths.Minimum = 0;
            udMonths.Value = 0;
            udLoanAmount.Enabled = false;
            udMonths.Enabled = false;
            tcAddress.Visible = false;
            btnAccept.Enabled = false;
            cmbTermsType.Enabled = false;
            drpCashLoanPurpose.SelectedIndex = -1;          //#19337 - CR18568
            btnDisburse.Enabled = false;
            btnPrint.Enabled = false;
            btnAccountDetails.Enabled = false;
            dalAuth = false;
            refer = false;                  // #8739
            exceedAvail = false;            // #8739

            txtCustId.Enabled = true;
            txtCustId.Focus();

            acctno = "000000000000";
            det = null;
            showDet = false;

            byPassAuthorisation = false;
            empeenoAdminChargeWaived = null;
            empeenoLoanAmountChanged = null;

            btnUpdateBankDetails.Enabled = false;
            chkEnableEBT.Checked = false;

        }

        private void btnAccountDetails_Click(object sender, EventArgs e)
        {
            acctno = acctno.Replace("-", "");

            AccountDetails details = new AccountDetails(acctno, FormRoot, this);
            ((MainForm)this.FormRoot).AddTabPage(details, 7);
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            
            //#19337 - CR18568
            errorProvider1.SetError(drpCashLoanPurpose, "");

            if (drpCashLoanPurpose.Visible && drpCashLoanPurpose.SelectedIndex == -1)
            {
                errorProvider1.SetError(drpCashLoanPurpose, "Please select a purpose");
                return;
            }            

            var status = CheckBankDetails();
            if (status == false)
            {
                return;
            }

            // current system supports maximum 60 months term length.
            int terms = Convert.ToInt32(udMonths.Value);
            if (terms > VL.MaxInstallmentTerms)
            {
                ShowInfo("M_MAXTERMS", new object[] { VL.MaxInstallmentTerms });
                return;
            }

            if (isMmiAllowed && !isMmiCalculated)
            {
                ShowInfo("M_MMINOTCALC");
                return;
            }

            //#CR-7- Extend Referrals to Cash Loans
            if (LoanQual == "N")
            {
                bool refReturn = false;
                string refDesc = "Customer does not qualify for Cash Loan." + Environment.NewLine + Environment.NewLine
                                 + "Please refer to the Credit Department.";

                CashLoanReferral referpopup = new CashLoanReferral(refDesc, Convert.ToString(CashLoanBlockedStatus.NotQualified), out refReturn);
                referpopup.ReasonForReferral = Convert.ToString(CashLoanBlockedStatus.NotQualified);
                referpopup.ReferralNote = "Customer does not qualify for Cash Loan. Do you wish to Refer?";
                referpopup.SetCtrlForReferral();
                referpopup.ShowDialog();
                refer = referpopup.Return;
                if (!refer)
                {
                    return;
                }
            }

            #region "Calculate MMI For Loan"
            if (LoanQual != "N" && isMmiAllowed)
            {
                string custId = txtCustId.Text;
                string AccountNo = acctno;

                CreditManager.GetMmiThresholdForSale(custId, AccountNo, termsType, out isMmiAppliedForSaleAtrr, out mmiLimit, out mmiThreshold, out Error);
                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    mmiThresholdLimit = mmiLimit + mmiThreshold;
                }

                if (isMmiAppliedForSaleAtrr)
                {
                    int totalMonth = 0;
                    int monthToExtend = 0;
                    decimal amountToReduce = 0;
                    decimal instalmentAmount = Convert.ToDecimal(StripCurrency(txtInstalment.Text));
                    if (mmiThresholdLimit < instalmentAmount)
                    {
                        PopulateActionNotesForLoan(mmiThresholdLimit, out totalMonth, out monthToExtend, out amountToReduce);

                        SaleAction sa = new SaleAction();
                        sa.TotalMonth = totalMonth;
                        sa.MonthToExtend = monthToExtend;
                        sa.AmountToReduce = amountToReduce;
                        sa.IsCashLoanDetails = true;
                        sa.SetControlsForNotes();
                        sa.ShowDialog();

                        if (sa.Modify)
                        {
                            status = false;
                            return;
                        }
                        else if (sa.Refer)
                        {
                            isReferedForMMI = true;
                            CreateAccount();
                            acctno = acctno.Replace("-", "");
                            det.referralReasons = ":MMI Exceeded";
                            CreditManager.SaveManualReferralNotes(custId, AccountNo, det.dateprop, sa.rtxtNewReferralNotes.Text, creditLimit, Config.CountryCode, out Error);

                            if (Error.Length > 0)
                                ShowError(Error);
                            else
                            {
                                ((MainForm)FormRoot).StatusBarText = "Customer has been Referred for MMI exceed";

                                string _error = string.Empty;
                                CreditManager.UnClearFlag(acctno, SS.R, true, Credential.UserId, out _error);
                                if (_error.Length > 0)
                                    ShowError(_error);
                            }
                        }
                        else if (sa.CloseSale)
                        {
                            status = false;
                            this.CloseTab();
                            return;
                        }
                        else
                        {
                            status = false;
                            return;
                        }
                    }
                }
            }
            #endregion "Calculate MMI For Loan"

            if(!isReferedForMMI)
                CreateAccount();

            //MmiCalculationVerification();            

            if (acctno != "000000000000")
            {
                btnAccountDetails.Enabled = true;
                btnAccept.Enabled = false;

                if (chkEnableEBT.Checked)
                {
                    btnUpdateBankDetails.Enabled = true;
                }

                if (dalAuth)
                {
                    btnDisburse.Enabled = true;
                }
                else
                {
                    btnDisburse.Enabled = false;
                }
            }
            else
            {
                btnAccountDetails.Enabled = false;
            }

            if(isMmiAppliedForSaleAtrr)
            {
                string customerId = this.txtCustId.Text;
                string formatedAcctNo = acctno.Replace("-", string.Empty);
                decimal installment = Convert.ToDecimal(StripCurrency(txtInstalment.Text));

                if (isReferedForMMI)
                {
                    OpenSanctionStage(det.dateprop, formatedAcctNo);
                }
                else if(installment > mmiLimit && installment <= (mmiLimit + mmiThreshold))
                {
                    CreditManager.AuditMmiThresholdUsedInstalment(customerId, formatedAcctNo, mmiLimit, mmiThreshold, installment, DateTime.Now, out Error);
                }

                if (!isReferedForMMI)
                {
                    OpenDocumentConfirmationStage(this.txtCustId.Text, det.dateprop, acctno);
                }
            }
            else if (det.unclearStage == SS.DC)      // #8761     && !exceedAvail)              // #8739
            {
                //errorProviderForWarning.SetError(udLoanAmount, "");
                // Load Document Confirmation   - #8487
                OpenDocumentConfirmationStage(this.txtCustId.Text, det.dateprop, acctno);
            }
            // Moving this down here to prevent error as customer record may be updated above
            if (cashLoanBlocked == CashLoanBlockedStatus.UnBlocked)            // #8761     
            {
                Client.Call(new UpdateCashLoanBlockedRequest
                {
                    CustId = txtCustId.Text,
                    BlockedStatus = CashLoanBlockedStatus.NotBlocked    //Set to "" NotBlocked
                },
                                  response =>
                                  {
                                  });
            }

            //if (!exceedAvail)           // #8747 jec 28/11/11
            //{
            //errorProviderForWarning.SetError(udLoanAmount, "");
            ((MainForm)FormRoot).StatusBarText = "Cash Loan accepted. Account must be Delivery Authorised before disbursing the loan";
            //}
            LoanQual = string.Empty;
        }
                

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (txtCustId.Text == string.Empty)
            {
                return;
            }

            byPassAuthorisation = true;

            ResetScreen();
            txtCustId.Text = "";// Fix for Issue #6398662 : Cancel Button not working
            validCustid = "";
            displayMessages = true;
            txtAgrmtTotal.Text = string.Empty;
            btnSearch.Enabled = false;          // #9733

            byPassAuthorisation = false;
        }

        private void btnDisburse_Click(object sender, EventArgs e)
        {
            if (dalAuth)
            {
                CashLoanDisbursement disburse = new CashLoanDisbursement(FormRoot, this, det,isMambuAccount);
                disburse.ShowDialog();
            }
            //else
            //{
            //    errorProvider1.SetError(btnDisburse, "Account not Delivery Authorised");
            //}
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            itemDoc = new XmlDocument();
            itemDoc.LoadXml("<ITEMS/>");
            XmlNode lineItems = AccountManager.GetLineItems(det.accountNo, 1, "R", Config.CountryCode, Convert.ToInt16(Config.BranchCode), out Error);
            if (Error.Length > 0)
                ShowError(Error);
            else
            {
                if (lineItems != null)
                {
                    lineItems = itemDoc.ImportNode(lineItems, true);

                    PrintCashLoanDocuments(det.accountNo,
                                    "R",
                                    det.custId,
                                    false,
                                    false,
                                    0, 0,
                                    itemDoc.DocumentElement,
                                    1,
                                    this,
                                    true,
                                    Credential.UserId,
                                    1);
                    // Set LoanStatus to Promissory printed
                    det.loanStatus = "P";
                    det.datePrinted = DateTime.Today;                               // #8491
                    det.empeenoAdminChargeWaived = empeenoAdminChargeWaived;
                    Error = AccountManager.CashLoanPromissoryNoteStatus(ref det);

                    if (Error.Length > 0)
                    {
                        ShowError(Error);
                    }
                    else
                    {
                        if (deliveryAuthorised)
                        {
                            btnDisburse.Enabled = true;
                        }
                    }

                }
            }
        }

        private void txtCustId_TextChanged(object sender, EventArgs e)
        {

            if (validCustid != "" && validCustid.Trim() != txtCustId.Text.Trim())
            {
                ResetScreen();
                validCustid = "";
                btnSearch.Enabled = false;       // #9733
            }

        }

        //Re-calculate LoanRepayment according to the Terms Type selected.
        private void cmbTermsType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!loading) //This event is fired when the combobox is populated, therefore do not fire this when populating the screen.
            {

                dalAuth = false;
                waivAdminCharge = false;
                empeenoAdminChargeWaived = null;

                selectedTermsType = (TermsTypeAllBands)cmbTermsType.SelectedItem;
                if (selectedTermsType != null)
                {
                    txtServiceChargePct.Text = Convert.ToString(selectedTermsType.ServPcent);
                    servicePercent = selectedTermsType.ServPcent;
                    insPercent = Convert.ToDecimal(selectedTermsType.InsPcent);
                    adminPercent = Convert.ToDecimal(selectedTermsType.AdminPcent);
                    termsType = selectedTermsType.TermsType;

                    

                    udMonths.Maximum = selectedTermsType.MaxTerm;
                    udMonths.Minimum = selectedTermsType.MinTerm;

                    udMonths.Value = selectedTermsType.MaxTerm;
                }
                callCalcLoanRepayment = false;
                callCalcLoanRepayment = true;

                byPassAuthorisation = true;

                CalcLoanRepayment();

                btnAccept.Enabled = true;
                btnDisburse.Enabled = false;
                btnPrint.Enabled = false;

                byPassAuthorisation = false;
            }
        }

        //Method to display any referral reasons a customer may have hit.
        private bool displayReferralReasons(DataSet referralReasons, string BlockedStatus, int availSpend, int loanAmt)
        {
            bool Return;
            StringBuilder sb = new StringBuilder();
            refReasonsSB = new StringBuilder();

            var num = 1;
            var status = false;

            if (BlockedStatus != CashLoanBlockedStatus.UnBlocked) //If the status is still blocked or not blocked
            {
                if (referralReasons.Tables.Count > 0)
                {

                    //Table [2] holds the referral messages to display
                    if (referralReasons.Tables[2].Rows.Count > 0)
                    {
                        foreach (DataRow dr in referralReasons.Tables[2].Rows)
                        {
                            if (Convert.ToString(dr[0]) == "Y")
                            {
                                sb.Append(Convert.ToString(num));
                                ReferralMessageArrears(referralReasons.Tables[0], ref sb);

                                num += 1;
                                status = true;
                                refReasonsSB.Append(":Arrears");
                            }

                            if (Convert.ToString(dr[1]) == "Y")
                            {
                                sb.Append(Environment.NewLine);
                                sb.Append(Convert.ToString(num));
                                sb.Append(" .Account referred for rescoring.");

                                num += 1;
                                status = true;
                                refReasonsSB.Append(":Referred for rescoring");
                            }

                            if (Convert.ToString(dr[2]) == "Y")
                            {
                                sb.Append(Environment.NewLine);
                                sb.Append(Convert.ToString(num));
                                ReferralMessageStatus(referralReasons.Tables[1], ref sb);

                                num += 1;
                                status = true;
                                refReasonsSB.Append(":Status Exceeded");
                            }

                            if (Convert.ToString(dr[3]) == "Y")
                            {
                                sb.Append(Environment.NewLine);
                                sb.Append(Convert.ToString(num));
                                sb.Append(" .Residence has changed.");

                                num += 1;
                                status = true;
                                refReasonsSB.Append(":Residence changed");
                            }

                            if (Convert.ToString(dr[4]) == "Y")
                            {
                                sb.Append(Environment.NewLine);
                                sb.Append(Convert.ToString(num));
                                sb.Append(" .Employment information has changed.");

                                num += 1;
                                status = true;
                                refReasonsSB.Append(":Employment info changed");
                            }

                            if (Convert.ToString(dr[5]) == "Y")
                            {
                                sb.Append(Environment.NewLine);
                                sb.Append(Convert.ToString(num));
                                sb.Append(" .Your Available Spend is less than required.");

                                num += 1;
                                status = true;
                                refReasonsSB.Append(":Available Spend");
                            }

                        }


                    }


                    //if (status && !BlockedloanAlready)           //If referred then block Cash Loan
                    //{
                    //    BlockedloanAlready = true;
                    //    Client.Call(new UpdateCashLoanBlockedRequest
                    //    {
                    //        CustId = txtCustId.Text,
                    //        BlockedStatus = CashLoanBlockedStatus.Blocked
                    //    },
                    //            response =>
                    //            {
                    //            });
                    //}

                }
            }
            if (ReviseCashLoanDisbursLimitsEnable)
            {
                if (referralReasons.Tables.Count > 0)
                {
                    foreach (DataRow dr in referralReasons.Tables[3].Rows)
                    {
                        existCashLoan = Convert.ToInt32(dr[0]);            // #8586 Cash Loan percentage used  
                    }
                }
            }
            else
            {
                decimal ExistCashLoan = 0;
                CreditManager.GetExistCashLoan(txtCustId.Text, out ExistCashLoan, out Error);
                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    existCashLoan = Convert.ToInt32(ExistCashLoan);
                }
            }


            //else                        //Status is UnBlocked
            //{
            //Client.Call(new UpdateCashLoanBlockedRequest
            //{
            //    CustId = txtCustId.Text,
            //    BlockedStatus = CashLoanBlockedStatus.NotBlocked    //Set to "" NotBlocked
            //},
            //                response =>
            //                {
            //                });

            //}

            if (loanAmt != 0 && (availSpend < loanAmt) && BlockedStatus == CashLoanBlockedStatus.NotBlocked)                  // #8747 #8558 #8559
            {
                sb.Append(Environment.NewLine);
                sb.Append(Convert.ToString(1));
                sb.Append(" .Your pending Loan Amount is less than Available Spend.");

                num += 1;

                showDet = true;
            }

            //#8546 jec 31/10/11   If no referrals but Cash Loan blocked - show message so screen is not loaded
            if (sb.Length == 0 && BlockedStatus == CashLoanBlockedStatus.Blocked)
            {
                sb.Append("Cash Loan has been Referred and blocked for this Customer");             // #9644
            }

            if (sb.Length > 0)
            {
                sb.Append(Environment.NewLine);
                sb.Append(Environment.NewLine);
                sb.Append("Please refer to the Credit Department.");

                if (displayMessages)
                {
                    displayMessages = false;
                    // MessageBox.Show(Convert.ToString(sb), "Referral Reasons", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    CashLoanReferral referpopup = new CashLoanReferral(Convert.ToString(sb), BlockedStatus, out Return);
                    referpopup.ShowDialog();
                    refer = referpopup.Return;
                }

                status = true;
            }


            return status;
        }

        public void ReferralMessageArrears(DataTable arrears, ref StringBuilder sb)
        {

            //Check arrears on accounts
            if (arrears.Rows.Count > 0)
            {
                sb.Append(". The following account(s) are currently in arrears. Please clear arrears before loan can be issued.");

                foreach (DataRow dr in arrears.Rows)
                {
                    sb.Append(Environment.NewLine);
                    sb.Append(Convert.ToString(dr[CN.AcctNo]));
                }

            }
        }

        public void ReferralMessageStatus(DataTable status, ref StringBuilder sb)
        {

            //Check status on accounts
            if (status.Rows.Count > 0)
            {
                sb.Append(". The following account(s) went above the maximum status code.");

                foreach (DataRow dr in status.Rows)
                {
                    sb.Append(Environment.NewLine);
                    sb.Append(Convert.ToString(dr[CN.AcctNo]));
                }

            }
        }

        public void UpdateCashLoanStatus(Blue.Cosacs.Shared.CashLoan loan)
        {

            det.empeenoAccept = loan.EmpeenoAccept;
            det.empeenoDisburse = loan.EmpeenoDisburse;
            det.loanAmount = Convert.ToDecimal(loan.LoanAmount);
            det.term = Convert.ToInt32(loan.Term);
            det.termsType = loan.TermsType;
            det.accountNo = loan.AcctNo;
            det.loanStatus = loan.LoanStatus;


            Error = AccountManager.CashLoanPromissoryNoteStatus(ref det);
        }

        // #8670 Create account and Refer
        private void Refer(CashLoanQualificationResponse customerDetails)
        {
            det = new STL.PL.WS2.CashLoanDetails();
            det.referralReasons = Convert.ToString(refReasonsSB);                     //IP - 24/02/12 - #9598 - UAT 87


            if (txtInstalment.Text == "")
            {
                txtInstalment.Text = "0";
                txtFinInstal.Text = "0";
                txtServiceChg.Text = "0";
                txtInsuranceChg.Text = "0";
                numAdminChg.Value = 0;
                txtSalesTax.Text = "0";
                txtAgrmtTotal.Text = "0";
                cmbTermsType.Set(customerDetails.TermsType, "Description");
                cmbTermsType.SelectedItem = 0;
                selectedTermsType = (TermsTypeAllBands)cmbTermsType.SelectedItem;
                if (selectedTermsType != null)
                    udMonths.Value = selectedTermsType.MaxTerm;
            }


            CreateAccount();

            Client.Call(new UpdateCashLoanBlockedRequest
            {
                CustId = txtCustId.Text,
                BlockedStatus = CashLoanBlockedStatus.Blocked
            },
                    response =>
                    {
                    });

        }

        //#19337 - CR18568
        private void LoadStatic()
        {
            XmlUtilities xml = new XmlUtilities();
            XmlDocument dropDowns = new XmlDocument();
            dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

            if (StaticData.Tables[TN.CashLoanPurpose] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.CashLoanPurpose, new string[] { "CLP", "L" }));

            if (dropDowns.DocumentElement.ChildNodes.Count > 0)
            {
                dropDownDS = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out Error);
                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    foreach (DataTable dt in dropDownDS.Tables)
                        StaticData.Tables[dt.TableName] = dt;
                }
            }

            drpCashLoanPurpose.DataSource = (DataTable)StaticData.Tables[TN.CashLoanPurpose];
            drpCashLoanPurpose.DisplayMember = CN.CodeDescription;

            drpBank.DataSource = (DataTable)StaticData.Tables[TN.Bank];
            drpBank.DisplayMember = CN.BankName;
            drpBank.ValueMember = CN.BankCode;

            drpBankAccountType.DataSource = (DataTable)StaticData.Tables[TN.BankAccountType];
            drpBankAccountType.DisplayMember = CN.CodeDescription;
            drpBankAccountType.ValueMember = CN.Code;

        }

        //#19560
        private void drpCashLoanPurpose_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (autoAccept
            && errorProvider1.GetError(drpCashLoanPurpose).Length > 0
            && drpCashLoanPurpose.SelectedIndex != -1)
            {
                btnAccept_Click(null, null);

                autoAccept = false;

            }
            if (isMambuAccount)
            {
                if (drpCashLoanPurpose.SelectedIndex > -1)
                {
                    btnDisburse.Enabled = true;
                }

            }
            

            errorProvider1.SetError(drpCashLoanPurpose, "");

        }

        private void numAdminChg_ValueChanged(object sender, EventArgs e)
        {
            var callFromPopulateScreen = true; //This parameter controls the tax rate retrieved for Admin / Insurance charge. Here we want it to behave as if we are loading the account
                                               //in the screen until the Admin Charge change has been authorised then we get the Tax Rate from Stockinfo otherwise from LineItem.
                                               //If not authorised then everything should return back to what it was.

            if (!loading) //Do not call the below method whilst populating the screen
            {
                waivAdminCharge = false;
                AuthorisePrompt auth = null;

                if (byPassAuthorisation == false)
                {
                    auth = new AuthorisePrompt(this, lAuthorise, GetResource("M_CASHLOANADMINCHARGE"));

                    auth.ShowDialog();
                }

                if (auth != null && auth.Authorised)
                {
                    waivAdminCharge = true;
                    empeenoAdminChargeWaived = auth.AuthorisedBy;

                    callFromPopulateScreen = false;
                    btnAccept.Enabled = true;
                    btnDisburse.Enabled = false;
                    btnPrint.Enabled = false;
                }


                dalAuth = false;
                byPassAuthorisation = true;
                CalcLoanRepayment(callFromPopulateScreen);
                byPassAuthorisation = false;

            }
        }

        private void udLoanAmount_Leave(object sender, EventArgs e)
        {

            var changedLoanValue = Decimal.Parse(((UpDownBase)sender).Text);

            if (byPassLoanAmountAuthorisation == false && changedLoanValue > udLoanAmount.Maximum)
            {
                var loanCurrentMax = udLoanAmount.Maximum;
                var loanCurrentValue = udLoanAmount.Value;

                //Set to 0 so that this can be changed 
                //udLoanAmount.Maximum = 0;

                if ((changedLoanValue <= maxLoanAmtNewCustomer && cust.CashLoanNew
                           || (changedLoanValue <= maxLoanAmtRecentCustomer && cust.CashLoanRecent)
                           || (changedLoanValue <= maxLoanAmt && cust.CashLoanExisting)
                           || (changedLoanValue <= maxLoanAmtStaffCustomer && cust.CashLoanStaff)) && changedLoanValue <= cust.AvailableSpend)
                {
                    AuthorisePrompt auth = null;

                    auth = new AuthorisePrompt(this, lAuthoriseLoanAmount, GetResource("M_CASHLOANAMOUNTCHANGED"));

                    auth.ShowDialog();

                    if (auth != null && auth.Authorised)
                    {
                        LoanAmountChangeAuthorised(changedLoanValue);

                        empeenoLoanAmountChanged = auth.AuthorisedBy;
                    }
                    else
                    {
                        udLoanAmount.Maximum = loanCurrentMax;
                        udLoanAmount.Value = Decimal.Parse(loanCurrentValue.ToString());
                    }

                }
            }

        }

        //If Loan Amount change has been authorised then need to set the correct maximum limit for the Loan Amount
        private void LoanAmountChangeAuthorised(decimal changedLoanValue)
        {
            if (cust.CashLoanNew)
            {
                if (cust.AvailableSpend <= maxLoanAmtNewCustomer)
                {
                    udLoanAmount.Maximum = cust.AvailableSpend;
                    udLoanAmount.Value = changedLoanValue;
                }
                else
                {
                    udLoanAmount.Maximum = maxLoanAmtNewCustomer;
                    udLoanAmount.Value = changedLoanValue;
                }
            }

            if (cust.CashLoanRecent)
            {
                if (cust.AvailableSpend <= maxLoanAmtRecentCustomer)
                {
                    udLoanAmount.Maximum = cust.AvailableSpend;
                    udLoanAmount.Value = changedLoanValue;
                }
                else
                {
                    udLoanAmount.Maximum = maxLoanAmtRecentCustomer;
                    udLoanAmount.Value = changedLoanValue;
                }
            }

            if (cust.CashLoanExisting)
            {
                if (cust.AvailableSpend <= maxLoanAmt)
                {
                    udLoanAmount.Maximum = cust.AvailableSpend;
                    udLoanAmount.Value = changedLoanValue;
                }
                else
                {
                    udLoanAmount.Maximum = maxLoanAmt;
                    udLoanAmount.Value = changedLoanValue;
                }
            }

            if (cust.CashLoanStaff)
            {
                if (cust.AvailableSpend <= maxLoanAmtStaffCustomer)
                {
                    udLoanAmount.Maximum = cust.AvailableSpend;
                    udLoanAmount.Value = changedLoanValue;
                }
                else
                {
                    udLoanAmount.Maximum = maxLoanAmtStaffCustomer;
                    udLoanAmount.Value = changedLoanValue;
                }
            }

            txtLoanRFSpend.Text = Convert.ToString(Math.Round(udLoanAmount.Maximum));
        }

        private void chkEnableEBT_CheckedChanged(object sender, EventArgs e)
        {
            if (chkEnableEBT.Checked)
            {
                drpBank.Enabled = true;
                drpBankAccountType.Enabled = true;
                txtBankAcctNo.Enabled = true;
                txtNotes.Enabled = true;
                txtBankBranch.Enabled = true;
                txtBankReferenceNo.Enabled = true;
                txtBankAccountName.Enabled = true;

                if (acctno != "000000000000")
                {
                    btnUpdateBankDetails.Enabled = true;
                }
            }
            else
            {
                errorProviderBankAcctNo.SetError(txtBankAcctNo, string.Empty);
                errorProviderBank.SetError(drpBank, string.Empty);
                errorProviderBankAccountType.SetError(drpBankAccountType, string.Empty);
                errorProviderBankBranch.SetError(txtBankBranch, string.Empty);
                errorProviderBankReference.SetError(txtBankReferenceNo, string.Empty);
                errorProviderNameOnAccount.SetError(txtBankAccountName, string.Empty);

                if (acctno != "000000000000")
                {
                    if (DialogResult.Yes == ShowInfo("M_REMOVEEBTDETAILS", MessageBoxButtons.YesNo))
                    {
                        btnUpdateBankDetails.Enabled = false;

                        drpBank.Enabled = false;
                        drpBank.SelectedIndex = -1;
                        drpBankAccountType.Enabled = false;
                        drpBankAccountType.SelectedIndex = -1;
                        txtBankAcctNo.Enabled = false;
                        txtNotes.Enabled = false;
                        txtBankBranch.Enabled = false;
                        txtBankReferenceNo.Enabled = false;
                        txtBankAccountName.Enabled = false;

                        txtBankAcctNo.Text = string.Empty;
                        txtNotes.Text = string.Empty;
                        txtBankBranch.Text = string.Empty;
                        txtBankReferenceNo.Text = string.Empty;
                        txtBankAccountName.Text = string.Empty;

                        det.Bank = chkEnableEBT.Checked ? ((DataRowView)drpBank.SelectedItem)[CN.BankCode].ToString() : null;
                        det.BankAccountType = chkEnableEBT.Checked ? ((DataRowView)drpBankAccountType.SelectedItem)[CN.Code].ToString() : null;
                        det.BankBranch = chkEnableEBT.Checked ? txtBankBranch.Text.Trim() : null;
                        det.BankAccountNo = chkEnableEBT.Checked ? txtBankAcctNo.Text.Trim() : null;
                        det.Notes = chkEnableEBT.Checked ? txtNotes.Text.Trim() : null;
                        det.BankReferenceNumber = chkEnableEBT.Checked ? txtBankReferenceNo.Text.Trim() : null;
                        det.BankAccountName = chkEnableEBT.Checked ? txtBankAccountName.Text.Trim() : null;

                        Error = AccountManager.UpdateCashLoanBankDetails(this.txtCustId.Text, acctno.Replace("-", ""), det.Bank, det.BankAccountType, det.BankBranch, det.BankAccountNo, det.Notes,
                                                                         det.BankReferenceNumber, det.BankAccountName);

                        ((MainForm)FormRoot).StatusBarText = "Bank details have been successfully updated";
                    }
                    else
                    {
                        //They do not with to remove bank details therefore leave as checked.
                        chkEnableEBT.Checked = true;
                    }
                }
            }
        }

        private void btnUpdateBankDetails_Click(object sender, EventArgs e)
        {
            var status = CheckBankDetails();

            if (status == false)
            {
                return;
            }

            det.Bank = chkEnableEBT.Checked ? ((DataRowView)drpBank.SelectedItem)[CN.BankCode].ToString() : null;
            det.BankAccountType = chkEnableEBT.Checked ? ((DataRowView)drpBankAccountType.SelectedItem)[CN.Code].ToString() : null;
            det.BankBranch = chkEnableEBT.Checked ? txtBankBranch.Text.Trim() : null;
            det.BankAccountNo = chkEnableEBT.Checked ? txtBankAcctNo.Text.Trim() : null;
            det.Notes = chkEnableEBT.Checked ? txtNotes.Text.Trim() : null;
            det.BankReferenceNumber = chkEnableEBT.Checked ? txtBankReferenceNo.Text.Trim() : null;
            det.BankAccountName = chkEnableEBT.Checked ? txtBankAccountName.Text.Trim() : null;

            Error = AccountManager.UpdateCashLoanBankDetails(this.txtCustId.Text, acctno.Replace("-", ""), det.Bank, det.BankAccountType, det.BankBranch, det.BankAccountNo, det.Notes,
                                                                det.BankReferenceNumber, det.BankAccountName);

            ((MainForm)FormRoot).StatusBarText = "Bank details have been successfully updated";
        }

        private bool CheckBankDetails()
        {
            var status = true;
            errorProviderBankAcctNo.SetError(txtBankAcctNo, string.Empty);
            errorProviderBank.SetError(drpBank, string.Empty);
            errorProviderBankAccountType.SetError(drpBankAccountType, string.Empty);
            errorProviderBankBranch.SetError(txtBankBranch, string.Empty);
            errorProviderBankReference.SetError(txtBankReferenceNo, string.Empty);
            errorProviderNameOnAccount.SetError(txtBankAccountName, string.Empty);

            if (chkEnableEBT.Checked)
            {
                if (this.txtBankAcctNo.Text.Trim().Length == 0)
                {
                    errorProviderBankAcctNo.SetError(txtBankAcctNo, "Please enter Bank Account Number");
                    this.txtBankAcctNo.Focus();
                    status = false;
                }

                if (this.drpBank.SelectedIndex == 0)
                {
                    errorProviderBank.SetError(drpBank, "Please select a Bank");
                    this.drpBank.Focus();
                    status = false;
                }

                if (this.drpBankAccountType.SelectedIndex == 0)
                {
                    errorProviderBankAccountType.SetError(drpBankAccountType, "Please select a Bank Account Type");
                    this.drpBankAccountType.Focus();
                    status = false;
                }

                if (txtBankBranch.Text.Trim().Length == 0)
                {
                    errorProviderBankBranch.SetError(txtBankBranch, "Please enter Bank Branch");
                    this.txtBankBranch.Focus();
                    status = false;
                }

                if (txtBankReferenceNo.Text.Trim().Length == 0)
                {
                    errorProviderBankReference.SetError(txtBankReferenceNo, "Please enter Bank Reference Number");
                    this.txtBankReferenceNo.Focus();
                    status = false;
                }

                if (txtBankAccountName.Text.Trim().Length == 0)
                {
                    errorProviderNameOnAccount.SetError(txtBankAccountName, "Please enter Name on Account");
                    this.txtBankAccountName.Focus();
                    status = false;
                }

            }

            return status;

        }


        #region "10.7 CR: Extend Referrals to Cash Loans"

        private void SetMmiValue()
        {
            if (isMmiAllowed)
            {
                string custId = txtCustId.Text;
                decimal dCustMMI = 0;                
                dCustMMI = CustomerManager.GetCustomerMmiLimit(custId, out isMmiCalculated);
                txtMMI.Text = dCustMMI.ToString((string)Country[CountryParameterNames.DecimalPlaces]);
            }
        }

        private decimal CalcLoanInstalmentAmount(int month, decimal loanAmount, bool calledFromPopulateScreen)
        {
            decimal instalment = 0;
            decimal finInstalment = 0;

            if (month > 0 && loanAmount > 0 && callCalcLoanRepayment == true)
            {
                if (selectedTermsType != null)
                    det.termsType = selectedTermsType.TermsType;
                det.scoreBand = (txtScoreBand.Text == string.Empty) ? defaultScoreBand : txtScoreBand.Text;
                det.accountNo = acctno;
                det.loanAmount = loanAmount;
                det.term = month;
                det.taxRate = taxRate;
                det.adminChg = numAdminChg.Value;
                det.waiveAdminCharge = waivAdminCharge;
                if (det.termsType != string.Empty)
                {
                    Error = AccountManager.CalculateCashLoanTerms(Config.CountryCode, ref det, Convert.ToInt16(Config.BranchCode), calledFromPopulateScreen,isMambuAccount);
                }

                instalment = det.instalment;
                finInstalment = det.finInstal;
                if (Convert.ToDouble(month) == 1)
                {
                    instalment = finInstalment;
                    finInstalment = 0;
                }
            }

            return instalment;
        }

        private void OpenSanctionStage(DateTime dateprop, string formatedAcctNo)
        {
            string acctType = "R";
            string custNo = this.txtCustId.Text;
            string acctNo = formatedAcctNo;
            string newAcctNo = formatedAcctNo;
            DateTime dtPropRF = dateprop;
            BasicCustomerDetails customerScreen = null;
            string stageCheckType = string.Empty;
            string stagePropResult = string.Empty;
            string Error = string.Empty;

            Crownwood.Magic.Controls.TabPage tp = ((MainForm)FormRoot).MainTabControl.SelectedTab;

            CreditManager.GetUnclearedStage(acctNo, ref newAcctNo, ref stageCheckType, ref dtPropRF, ref stagePropResult, out Error);
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
                            refer = new Referral(false, custNo, dtPropRF, acctNo, acctType
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
                            stage1 = new SanctionStage1(false, parms, STL.Common.Constants.ScreenModes.SM.Edit, FormRoot, this, customerScreen);
                            ((MainForm)this.FormRoot).AddTabPage(stage1, 18);
                        }
                        break;
                }

                docTab = ((MainForm)FormRoot).MainTabControl.SelectedTab;
                ((MainForm)FormRoot).MainTabControl.SelectedTab = tp;
            }
        }

        private void PopulateActionNotesForLoan(decimal mmiThresholdLimit, out int totalMonth, out int monthToExtend, out decimal amountToReduce)
        {
            int selectedMonth = Convert.ToInt32(udMonths.Value);
            totalMonth = selectedMonth;
            decimal currentInstallement = Convert.ToDecimal(StripCurrency(txtInstalment.Text));
            decimal newMonthlyInstalment = currentInstallement;
            decimal loanAmount = udLoanAmount.Value;
            // Calculate extend the term length by months
            while (newMonthlyInstalment > Convert.ToDecimal(mmiThresholdLimit) && totalMonth <= VL.MaxInstallmentTerms)
            {
                totalMonth = totalMonth + 1;
                newMonthlyInstalment = CalcLoanInstalmentAmount(totalMonth, loanAmount, false);
            }
            monthToExtend = Convert.ToInt32(totalMonth - selectedMonth);


            // Calculate amount to reduce
            decimal calculatedLoanAmount = CalculateLoanAmountForMmi(mmiThresholdLimit);
            amountToReduce = loanAmount - Math.Ceiling(calculatedLoanAmount);
        }

        public decimal CalculateLoanAmountForMmi(decimal mmiThresholdLimit)
        {
            decimal minLoanAmount = 0;
            decimal maxLoanAmount = udLoanAmount.Value;
            decimal newMonthlyInstalment = 0;
            int selectedMonth = Convert.ToInt32(udMonths.Value);
            int counter = 0;
            decimal calculatedLoanAmount = 0;
            while (minLoanAmount <= maxLoanAmount)
            {
                counter++;
                if (counter > 50)
                    break;
                calculatedLoanAmount = (minLoanAmount + maxLoanAmount) / 2;
                newMonthlyInstalment = CalcLoanInstalmentAmount(selectedMonth, calculatedLoanAmount, false);
                if (mmiThresholdLimit == newMonthlyInstalment)
                {
                    return calculatedLoanAmount;
                }
                else if (mmiThresholdLimit < newMonthlyInstalment)
                {
                    maxLoanAmount = calculatedLoanAmount - 1;
                }
                else
                {
                    minLoanAmount = calculatedLoanAmount + 1;
                }
            }
            return calculatedLoanAmount;
        }

        private void MmiCalculationVerification()
        {
            if (isMmiAllowed && !isMmiCalculated && !string.IsNullOrEmpty(acctno) && acctno != "000000000000")
            {
                string _error = string.Empty;
                string acctNo = acctno.Replace("-", string.Empty);
                string newAcctNo = acctNo;
                DateTime dtPropRF = det.dateprop;
                string stageCheckType = string.Empty;
                string stagePropResult = string.Empty;

                ShowInfo("M_MMINOTCALC");

                CreditManager.GetUnclearedStage(acctNo, ref newAcctNo, ref stageCheckType, ref dtPropRF, ref stagePropResult, out _error);
                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    _error = string.Empty;
                    CreditManager.UnClearFlag(acctNo, SS.S1, true, Credential.UserId, out _error);
                    if (_error.Length > 0)
                            ShowError(_error);
                    return;
                }
            }
        }

        private void OpenDocumentConfirmationStage(string custId, DateTime dateprop, string acctno)
        {
            DocumentConfirmation docComf = null;
            docComf = new DocumentConfirmation(custId, dateprop, acctno.Replace("-", ""), "R", STL.Common.Constants.ScreenModes.SM.Edit, FormRoot, this, this);

            Crownwood.Magic.Controls.TabPage tp = ((MainForm)FormRoot).MainTabControl.SelectedTab;
            ((MainForm)FormRoot).AddTabPage(docComf);
            docTab = ((MainForm)FormRoot).MainTabControl.SelectedTab;
            ((MainForm)FormRoot).MainTabControl.SelectedTab = docTab;
        }

        #endregion "10.7 CR: Extend Referrals to Cash Loans"

    }

}
