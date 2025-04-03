using System;
using System.Data;
using System.Windows.Forms;
using STL.Common;
using STL.Common.Constants.CashLoans;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using STL.Common.Static;
using System.Xml;
using STL.Common.Constants.Categories;
using STL.Common.Constants.FTransaction;


namespace STL.PL.CashLoan
{
    public partial class CashLoanDisbursement : CommonForm
    {
        private STL.PL.WS2.CashLoanDetails cashLoanDet = null;
        private STL.PL.WS2.CashLoanDisbursementDetails cashLoanDisbursementDet = null;
        string err = string.Empty;
        private DataView Deposits = null;
        decimal forDeposit = 0;
        private string error = "";
        private bool IsMambuAccount = false;

        public CashLoanDisbursement()
        {
            InitializeComponent();
        }

        public CashLoanDisbursement(Form root, Form parent, STL.PL.WS2.CashLoanDetails det, bool isMambuAccount)
        {
            FormRoot = root;
            FormParent = parent;

            cashLoanDet = det;

            InitializeComponent();
            IsMambuAccount = isMambuAccount;

            PopulateScreen(cashLoanDet);
            RefreshDeposits();
        }

        public void PopulateScreen(STL.PL.WS2.CashLoanDetails cashLoanDet)
        {
            txtCustId.Text = cashLoanDet.custId;
            txtCustomerName.Text = cashLoanDet.custName;
            txtAcctNo.Text = cashLoanDet.accountNo;
            if ((bool)Country[CountryParameterNames.CL_Amortized])
            {
                txtDisburseAmt.Text = Convert.ToString(Math.Round((cashLoanDet.loanAmount), 2));
            }
            else
            {
                //If mambu account
                 if (IsMambuAccount)
                {
                    txtDisburseAmt.Text = Convert.ToString(Math.Round((cashLoanDet.loanAmount), 2));
                }
                else
                {
                    txtDisburseAmt.Text = Convert.ToString(Math.Round((cashLoanDet.loanAmount - cashLoanDet.adminChg), 2));
                }
                               // #8771
            }

            if (cashLoanDet.adminChg > 0)
            {
                if ((bool)Country[CountryParameterNames.CL_Amortized])
                {
                    lblCashierPaymentReminder.Visible = false;
                    lblCashierPaymentReminder.Text = "";

                }
                else
                {

                    lblCashierPaymentReminder.Visible = IsMambuAccount?false:true;
                    lblCashierPaymentReminder.Text = GetResource("M_CASHLOANDISBURSEMENTPAYADMIN", new object[] { cashLoanDet.adminChg.ToString(DecimalPlaces) });
                }
            }
           
        }

        /// <summary>
        /// TAKEN FROM CASHIER DISBURSEMENTS RM #9818
        /// 
        /// This will retrieve the outstanding deposits for a particular cashier.
        /// This is requierd so we can check whether they are trying to disburse
        /// more than they have available. 
        /// </summary>
        private void RefreshDeposits()
        {
            short branchno = Convert.ToInt16(Config.BranchCode);
            DataSet ds = PaymentManager.GetCashierOutstandingIncomeByPayMethod(Credential.UserId, branchno, out Error);
            if (Error.Length > 0)
                ShowError(Error);
            else
                Deposits = ds.Tables[TN.CashierOutstandingIncome].DefaultView;
            Deposits.RowFilter = CN.CodeDescript + " = 'CASH'";
            foreach (DataRowView r in Deposits)
                forDeposit += (decimal)r[CN.ForDeposit];
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnPrintReceipt_Click(object sender, EventArgs e)
        {
            int currentDisbursementMethod = Convert.ToInt16(((DataRowView)drpDisbursementType.SelectedItem)[CN.Code].ToString());

            bool payByCash = PayMethod.IsPayMethod(currentDisbursementMethod, PayMethod.Cash);
            bool payByCheque = PayMethod.IsPayMethod(currentDisbursementMethod, PayMethod.Cheque);
            bool payByCard = (PayMethod.IsPayMethod(currentDisbursementMethod, PayMethod.CreditCard) || PayMethod.IsPayMethod(currentDisbursementMethod, PayMethod.DebitCard));
            bool payByElectronicBankTransfer = PayMethod.IsPayMethod(currentDisbursementMethod, PayMethod.ElectronicBankTransfer);
            
            //No Disbursement Method selected
            if (currentDisbursementMethod == 0)
            {
                ShowInfo("M_REQUIREPAYMETHOD");
                this.drpDisbursementType.Focus();
                return;
            }

            // When paying by card the card type must be entered
            if (this.drpCardType.SelectedIndex == 0
                && payByCard)
            {
                ShowInfo("M_REQUIRECARDTYPE");
                this.drpCardType.Focus();
                return;
            }

            //When paying by Credit/Debit card the card number must be entered
            if (mtb_CardNo.Text.Trim().Length == 0
               && mtb_CardNo.Visible == true
               && payByCard)
            {
                ShowInfo("M_INCOMPLETECARDNO");
                this.mtb_CardNo.Focus();
                return;
            }

            if (this.txtCardNo.Text.Trim().Length == 0
                && txtCardNo.Visible == true
                && payByCheque)
            {
                ShowInfo("M_REQUIRECHEQUENO");
                this.txtCardNo.Focus();
                return;
            }
            
            if (this.txtBankAcctNo.Text.Trim().Length == 0
                  && (payByCheque ||payByElectronicBankTransfer))
            {
                if (payByCheque)
                {
                    ShowInfo("M_REQUIREBANKACCOUNTNO");
                }
                else
                {
                    ShowInfo("M_REQUIREBANKACCOUNTNOEBT");
                }

                
                this.txtBankAcctNo.Focus();
                return;
            }

            if (payByElectronicBankTransfer && this.drpBank.SelectedIndex == 0)
            {
                ShowInfo("M_REQUIREBANK");
                this.drpBank.Focus();
                return;
            }

            if (payByElectronicBankTransfer && this.drpBankAccountType.SelectedIndex == 0)
            {
                ShowInfo("M_REQUIREBANKACCOUNTTYPE");
                this.drpBankAccountType.Focus();
                return;
            }

            if (payByElectronicBankTransfer && txtBankBranch.Text.Trim().Length == 0)
            {
                ShowInfo("M_REQUIREBANKBRANCH");
                this.txtBankBranch.Focus();
                return;
            }

            if (payByElectronicBankTransfer && txtBankReferenceNo.Text.Trim().Length == 0)
            {
                ShowInfo("M_REQUIREBANKREFERENCE");
                this.txtBankReferenceNo.Focus();
                return;
            }

            if (payByElectronicBankTransfer && txtBankAccountName.Text.Trim().Length == 0)
            {
                ShowInfo("M_REQUIREBANKACCOUNTNAME");
                this.txtBankAccountName.Focus();
                return;
            }

            btnPrintReceipt.Enabled = false;                        // IP - 27/09/12 - #10480 - LW75156
            drpDisbursementType.Enabled = false;
            txtCardNo.Enabled = false;
            drpBank.Enabled = false;
            drpBankAccountType.Enabled = false;
            txtBankBranch.Enabled = false;
            txtBankAcctNo.Enabled = false;
            txtNotes.Enabled = false;
            txtBankReferenceNo.Enabled = false;
            txtBankAccountName.Enabled = false;

            if (cashLoanDet.loanAmount > forDeposit && payByCash)
            {
                ShowInfo("M_DEPOSITTOOHIGH", new Object[] { cashLoanDet.loanAmount.ToString(DecimalPlaces), forDeposit.ToString(DecimalPlaces) });
                //val = forDeposit; //KEF don't want to enter row if deposit is too high
                this.Close();
                return;
            }

            this.statusStrip1.Text = "Printing Cash Loan receipt";

            cashLoanDet.loanStatus = CashLoanStatus.Disbursed;
            cashLoanDet.empeenoDisburse = Credential.UserId;

            cashLoanDisbursementDet = new WS2.CashLoanDisbursementDetails
            {
                accountNo = cashLoanDet.accountNo,
                custId = cashLoanDet.custId,
                loanAmount = cashLoanDet.loanAmount,
                disbursementType = Convert.ToInt16(((DataRowView)drpDisbursementType.SelectedItem)[CN.Code].ToString()),
                cardType = payByCard ? ((DataRowView)drpCardType.SelectedItem)[CN.Code].ToString() : null,
                chequeCardNo = payByElectronicBankTransfer || payByCash ? null : PayMethod.IsPayMethod(currentDisbursementMethod, PayMethod.Cheque) ? txtCardNo.Text.Trim() : mtb_CardNo.Text.Trim(),
                bankName = payByCash ? null : ((DataRowView)drpBank.SelectedItem)[CN.BankCode].ToString(),
                bankAccountType = payByElectronicBankTransfer ? ((DataRowView)drpBankAccountType.SelectedItem)[CN.Code].ToString() : null,
                bankBranch = payByElectronicBankTransfer ? txtBankBranch.Text.Trim() : null,
                bankAccountNo = payByCash ? null : txtBankAcctNo.Text.Trim(),
                notes = payByElectronicBankTransfer ? txtNotes.Text.Trim() : null,
                bankReferenceNo = payByElectronicBankTransfer ? txtBankReferenceNo.Text.Trim() : null,
                bankAccountName = payByElectronicBankTransfer ? txtBankAccountName.Text.Trim() : null

            };
            
            
            err = AccountManager.CashLoanDeliverAccount(ref cashLoanDet, Convert.ToInt16(Config.BranchCode), cashLoanDisbursementDet);                 //IP - 21/02/12 - #9626

            if (err.Length > 0)
            {
                ShowError(err);

                // IP - 27/09/12 - #10480 - LW75156
                btnPrintReceipt.Enabled = true;  

            }
            else
            {


                //Print receipt
                if (ThermalPrintingEnabled || SlipPrinterOK())
                {
                    var disburseMethodDescription = ((DataRowView)drpDisbursementType.SelectedItem)[CN.CodeDescript].ToString();

                    NewPrintCashLoanReceipt(cashLoanDet.custName, cashLoanDet.accountNo, DateTime.Now, "CLD", cashLoanDet.loanAmount, cashLoanDet.transrefno, cashLoanDet.outstBal, cashLoanDet.firstInstalDate, disburseMethodDescription);   // #11419
                }

                //Disable controls on the Cash Loan Application screen
                ((CashLoanApplication)FormParent).disbursementBtnEnable = false;
                ((CashLoanApplication)FormParent).udLoanAmountEnable = false;
                ((CashLoanApplication)FormParent).udMonthsEnable = false;
                ((CashLoanApplication)FormParent).cmbTermsTypeEnable = false;
                ((CashLoanApplication)FormParent).btnPrintPromissoryEnable = false;
                ((CashLoanApplication)FormParent).numAdminChgEnable = false;
        
                ((MainForm)FormRoot).StatusBarText = "Loan disbursed";

                //btnPrintReceipt.Enabled = false;


                //Print Electronic Bank Transfer Sheet
                if (payByElectronicBankTransfer)
                {
                    PrintElectronicBankTransferSheet(cashLoanDet.accountNo);
                }
       
                //this.Close();

            }
        }

        private bool SlipPrinterOK()
        {
            Function = "Cash Loan Disbursement: Check Slip Printer connected";

            while (1 == 1)
                try
                {
                    // Early warning if receipt printer not available
                    ((MainForm)this.FormRoot).statusBar1.Text = GetResource("M_SLIPCHECK");
                    //         this._slipRequired = true;
                    ReceiptPrinter rp = null;
                    rp = new ReceiptPrinter(this);
                    rp.OpenPrinter();

                    // Check for paper
                    if (rp.SlpEmpty)
                    {
                        rp.ClosePrinter();
                        ((MainForm)this.FormRoot).statusBar1.Text = GetResource("M_SLIPPAPEROUT");
                        DialogResult userRequest = ShowInfo("M_SLIPPAPER", MessageBoxButtons.AbortRetryIgnore);
                        if (userRequest == DialogResult.Abort)
                        {
                            return false;
                        }
                        else if (userRequest == DialogResult.Ignore)
                        {
                            //   this._slipRequired = false;
                            return true;
                        }
                        // Otherwise Retry
                    }
                    else
                    {
                        rp.ClosePrinter();
                        ((MainForm)this.FormRoot).statusBar1.Text = GetResource("M_SLIPOK");
                        return true;
                    }
                }
                catch
                {
                    ((MainForm)this.FormRoot).statusBar1.Text = GetResource("M_SLIPNOCONNECT");
                    DialogResult userRequest = ShowInfo("M_SLIPCONNECT", MessageBoxButtons.AbortRetryIgnore);
                    if (userRequest == DialogResult.Abort)
                    {
                        ((MainForm)FormRoot).lDownloading.Visible = false;
                        ((MainForm)FormRoot).pbDownloading.Visible = false;
                        return false;
                    }
                    else if (userRequest == DialogResult.Ignore)
                    {
                        ((MainForm)FormRoot).lDownloading.Visible = false;
                        ((MainForm)FormRoot).pbDownloading.Visible = false;
                        //     this._slipRequired = false;
                        return true;
                    }
                    // Otherwise Retry
                }

        }  // End of SlipPrinterOK


        private void CashLoanDisbursement_Load(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                if (!ThermalPrintingEnabled && this.SlipPrinterOK() != true)
                {
                    this.CloseTab();
                }

                LoadStaticData();

                PopulateElectronicBankTransferDetails();
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
                ((MainForm)this.FormRoot).statusBar1.Text = "";
            }
            finally
            {
                StopWait();
            }
        }

        private void PopulateElectronicBankTransferDetails()
        {
            //If Bank Details have been entered then select Electronic Bank Transfer Payment Method
            if (cashLoanDet.Bank != null && cashLoanDet.Bank != string.Empty)
            {
                DataTable disbursement = ((DataView)drpDisbursementType.DataSource).ToTable().Copy();
                DataView disbursementView = new DataView(disbursement);

                disbursementView.RowFilter = "Code = '84'";

                if (disbursementView.Count > 0)
                {
                    drpDisbursementType.SelectedValue = "84";
                    drpBank.SelectedValue = cashLoanDet.Bank;
                    drpBankAccountType.SelectedValue = cashLoanDet.BankAccountType;
                    txtBankAcctNo.Text = cashLoanDet.BankAccountNo;
                    txtBankBranch.Text = cashLoanDet.BankBranch;
                    txtNotes.Text = cashLoanDet.Notes;
                    txtBankReferenceNo.Text = cashLoanDet.BankReferenceNumber;
                    txtBankAccountName.Text = cashLoanDet.BankAccountName;

                    drpBank.Enabled = false;
                    drpBankAccountType.Enabled = false;
                    txtBankAcctNo.Enabled = false;
                    txtBankBranch.Enabled = false;
                    txtBankReferenceNo.Enabled = false;
                    txtBankAccountName.Enabled = false;
    
                }
            }
        }

        private void LoadStaticData()
        {
            //Get the required static data for the drop down lists

            XmlUtilities xml = new XmlUtilities();
            XmlDocument dropDowns = new XmlDocument();
            dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

            if (StaticData.Tables[TN.CashLoanDisbursementMethods] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.CashLoanDisbursementMethods, null));
            if (StaticData.Tables[TN.CreditCard] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.CreditCard, new string[] { CAT.CreditCardType, "L" }));
            if (StaticData.Tables[TN.Bank] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.Bank, null));
            if (StaticData.Tables[TN.BankAccountType] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.BankAccountType, new string[] { "BA2", "L" }));
            if (StaticData.Tables[TN.BankBranches] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.BankBranches, new string[] { "BBR", "L" }));

            if (dropDowns.DocumentElement.ChildNodes.Count > 0)
            {
                DataSet ds = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out error);
                if (error.Length > 0)
                    ShowError(error);
                else
                {
                    foreach (DataTable dt in ds.Tables)
                    {
                        StaticData.Tables[dt.TableName] = dt;
                    }
                }
            }

            DataTable dtPayMethod = ((DataTable)StaticData.Tables[TN.CashLoanDisbursementMethods]).Copy();

            drpDisbursementType.DataSource = dtPayMethod.DefaultView;
            drpDisbursementType.ValueMember = CN.Code;
            drpDisbursementType.DisplayMember = CN.CodeDescription;

            drpCardType.DataSource = (DataTable)StaticData.Tables[TN.CreditCard];
            drpCardType.DisplayMember = CN.CodeDescription;

            drpBank.DataSource = (DataTable)StaticData.Tables[TN.Bank];
            drpBank.DisplayMember = CN.BankName;
            drpBank.ValueMember = CN.BankCode;

            drpBankAccountType.DataSource = (DataTable)StaticData.Tables[TN.BankAccountType];
            drpBankAccountType.DisplayMember = CN.CodeDescription;
            drpBankAccountType.ValueMember = CN.Code;
        }

        private void drpPayMethod_SelectedIndexChanged(object sender, EventArgs e)
        {

            int curPayMethod = Convert.ToInt16(((DataRowView)drpDisbursementType.SelectedItem)[CN.Code].ToString());

            bool payByEntered = (curPayMethod != 0);

            bool payByCash = PayMethod.IsPayMethod(curPayMethod, PayMethod.Cash);
            bool payByCheque = PayMethod.IsPayMethod(curPayMethod, PayMethod.Cheque);
            bool payByCard = (PayMethod.IsPayMethod(curPayMethod, PayMethod.CreditCard) || PayMethod.IsPayMethod(curPayMethod, PayMethod.DebitCard));
            bool payByElectronicBankTransfer = PayMethod.IsPayMethod(curPayMethod, PayMethod.ElectronicBankTransfer);

            this.lCardNo.Visible = payByEntered && (payByCard || payByCheque);
            this.mtb_CardNo.Enabled = payByEntered && (payByCard);
            this.mtb_CardNo.Visible = payByEntered && (payByCard);

            this.txtCardNo.Enabled = payByEntered && (payByCheque);
            this.txtCardNo.Visible = payByEntered && (payByCheque);

            this.drpBank.Enabled = payByEntered && (payByCheque || payByCard || payByElectronicBankTransfer);
            this.txtBankAcctNo.Enabled = payByEntered && (payByCheque || payByCard || payByElectronicBankTransfer);

            this.drpCardType.Enabled = payByEntered && payByCard;

            this.drpBankAccountType.Enabled = payByEntered && payByElectronicBankTransfer;

            this.txtBankBranch.Enabled = payByEntered && payByElectronicBankTransfer;

            this.txtNotes.Enabled = payByEntered && payByElectronicBankTransfer;

            this.txtBankReferenceNo.Enabled = payByEntered && payByElectronicBankTransfer;

            this.txtBankAccountName.Enabled = payByEntered && payByElectronicBankTransfer;

            if (payByElectronicBankTransfer)
            {
                PopulateElectronicBankTransferDetails();
            }
            else
            {
                drpBank.SelectedIndex = -1;
                drpBankAccountType.SelectedIndex = -1;
                txtBankAcctNo.Text= string.Empty;
                txtBankBranch.Text = string.Empty;
                txtNotes.Text = string.Empty;
                txtBankReferenceNo.Text = string.Empty;
                txtBankAccountName.Text = string.Empty;
            }

        }



    }
}
