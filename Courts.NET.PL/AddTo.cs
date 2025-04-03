using System;
using System.Data;
using System.Windows.Forms;
using System.Xml;
using STL.Common;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.ScreenModes;
using STL.Common.Constants.TableNames;
using STL.Common.Static;




namespace STL.PL
{
    /// <summary>
    /// Popup prompt to provide a list of accounts eligible to be added
    /// to a new 'Add-To' account for a customer. New agreement terms type,
    /// deposit, instalments and payment holidays can be set here. The user
    /// first clicks a calculate button to return a summary of the new
    /// agreement. The user then has the option to accept the new agreement
    /// and print the new agreement details.
    /// If the customer has insufficient credit then the only option will be
    /// to refer or cancel the Add-To. 
    /// </summary>
    public class AddTo : CommonForm
    {
        private new string Error = "";
        private bool _userChanged = false;
        private bool _canSetTermsType = false;
        private DataTable _dtTermsTypes = null;
        private DataView _dvTermsTypes = null;
        private string _acctType = "";
        private string _scoringBand = "";
        private decimal _defaultDeposit = 0;
        private bool _depositIsPercentage = false;
        private short _payMethod = 0;
        private string _customerId = "";
        private bool isMember = false;
        private bool hasPClubDiscount = false;
        private string customerPClubCode = "";
        private string privilegeClubDesc = "";
        private decimal _RFCreditLimit = 0;
        private decimal _creditRemaining = 0;
        private string _addToAccountNo = "";
        public string addToAccountNo
        {
            get { return _addToAccountNo; }
            set { _addToAccountNo = value; }
        }

        //CR903 Include a public property to hold the selected branch's store type
        private string m_storeType = String.Empty;
        public string SType
        {
            get
            {
                return m_storeType;
            }
            set
            {
                m_storeType = value;
            }
        }

        private bool _isLoan;
        public bool IsLoan
        {
            get
            {
                return _isLoan;
            }
            set
            {
                _isLoan = value;
            }
        }

        private AddToCollection _addToAccountList = null;

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RichTextBox rtbMessage;
        private System.Windows.Forms.Button btnCalculate;
        private System.Windows.Forms.Button btnAccept;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ColumnHeader accountNo;
        private System.Windows.Forms.ColumnHeader agreementTotal;
        private System.Windows.Forms.ColumnHeader outstandingBalance;
        private System.Windows.Forms.ListView lvAccounts;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox gbTerms;
        private System.Windows.Forms.CheckBox cbDeposit;
        private System.Windows.Forms.NumericUpDown numPaymentHolidays;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox drpTermsType;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox txtDeposit;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.ComboBox drpLengths;
        private System.Windows.Forms.NumericUpDown txtNoMonths;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnRefer;
        private System.Windows.Forms.ColumnHeader type;
        private Button btnTermsTypeBand;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        /// <summary>
        /// Translation constructor
        /// </summary>
        /// <param name="d"></param>
        public AddTo(TranslationDummy d)
        {
            InitializeComponent();
        }

        public AddTo(string accountNo, AddToCollection addToAccounts, Form root, Form parent, string storeType)
        {
            _userChanged = false;
            InitializeComponent();
            FormRoot = root;
            FormParent = parent;
            addToAccountNo = accountNo;
            Text += " " + FormatAccountNo(accountNo);
            _addToAccountList = addToAccounts;
            this._scoringBand = (Convert.ToString(Country[CountryParameterNames.TermsTypeBandDefault]));

            TranslateControls();

            // Load Terms Type drop down
            XmlUtilities xml = new XmlUtilities();
            XmlDocument dropDowns = new XmlDocument();
            dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");
            dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.TermsType, new string[] { Config.CountryCode }));

            DataSet termsSet = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out Error);
            if (Error.Length > 0)
                ShowError(Error);
            else
            {
                foreach (DataTable dt in termsSet.Tables)
                {
                    if (dt.TableName == TN.TermsType)
                    {
                        DataRow r = dt.NewRow();
                        r["termstype"] = "Terms Types";
                        r["Affinity"] = "X";
                        r[CN.IncludeWarranty] = Convert.ToInt16(0);
                        r[CN.PaymentHoliday] = false;
                        r[CN.DeliverNonStocks] = "0";
                        r["accounttype"] = "";
                        dt.Rows.InsertAt(r, 0);
                    }
                    StaticData.Tables[dt.TableName] = dt;
                }
            }
            _dtTermsTypes = (DataTable)StaticData.Tables[TN.TermsType];
            _dvTermsTypes = new DataView(_dtTermsTypes);
            drpTermsType.DataSource = _dvTermsTypes;
            drpTermsType.DisplayMember = "termstype";

            //CR903 Set the store type
            SType = storeType;
            string currentband = "";
            // Load any pre-existing Account details
            DataSet ds = AccountManager.GetAccountForRevision(accountNo, 1, out _scoringBand, out currentband, out Error);
            if (Error.Length > 0)
                ShowError(Error);
            else
            {
                foreach (DataTable dt in ds.Tables)
                {
                    if (dt.TableName == TN.AccountDetails)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            // Get the account type and band
                            this._acctType = (string)row["accttype"];
                            //    this._scoringBand = (string)row[CN.ScoringBand]; -- removing as done above -- also should be only one row brought back. 

                            // Terms types are specific to certain account types and bands
                            DataView dvTermsType = (DataView)drpTermsType.DataSource;

                            // Get the current setting
                            string curTermsType = drpTermsType.Text;

                            FilterTermsType(ref dvTermsType, false, _acctType, _scoringBand, SType, IsLoan);

                            // Make sure the TermsType has not changed if it is still available
                            int selectedTermsType = drpTermsType.FindStringExact(curTermsType);
                            drpTermsType.SelectedIndex = (selectedTermsType != -1) ? selectedTermsType : 0;

                            // Set the terms type
                            int index = drpTermsType.FindString((string)row["termstype"]);
                            if (index != -1)
                                drpTermsType.SelectedIndex = index;

                            // Set the Deposit
                            this.cbDeposit.Checked = (Convert.ToDecimal(row["deposit"]) > 0);
                            txtDeposit.Text = ((decimal)row["deposit"]).ToString(DecimalPlaces);

                            // Set the no of instalments
                            if (!Convert.IsDBNull(row["instalno"]))
                                txtNoMonths.Value = Convert.ToDecimal(row["instalno"]);

                            // Set the Payment Holidays
                            numPaymentHolidays.Value = Convert.ToDecimal(row[CN.PaymentHoliday]);

                            // Get the customer ID and Pay Method for agreement printing
                            this._customerId = (string)row[CN.CustomerID];
                            if (IsStrictNumeric((string)row["paymethod"]) && (string)row["paymethod"] != "")
                                this._payMethod = Convert.ToInt16(row["paymethod"]);
                        }
                    }
                }
            }

            if ((bool)Country[CountryParameterNames.LoyaltyCard])
            {
                CustomerManager.IsPrivilegeMember(this._customerId, out isMember, out customerPClubCode, out privilegeClubDesc, out hasPClubDiscount, out Error);
            }
            // Terms type can only be changed if it is not already set
            this._canSetTermsType = (this.drpTermsType.SelectedIndex == 0 || txtNoMonths.Value == 0);
            this.gbTerms.Enabled = _canSetTermsType;
            this.btnCalculate.Enabled = !_canSetTermsType;

            // If the terms type is not set then it should default to the
            // first terms type with delnonstocks = 1. This is required to
            // immediately deliver the non-stock on a new Add-To account.
            if (_canSetTermsType)
            {
                // Search the Terms Types filtered for this account type
                int index = 0;
                bool found = false;
                while (index < this._dvTermsTypes.Count && !found)
                {
                    DataRow row = this._dvTermsTypes[index].Row;
                    found = ((short)row[CN.DeliverNonStocks] == 1);
                    index++;
                }

                if (found)
                {
                    drpTermsType.SelectedIndex = --index;
                    this.btnCalculate.Enabled = true;
                }
            }

            _userChanged = true;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddTo));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnPrint = new System.Windows.Forms.Button();
            this.gbTerms = new System.Windows.Forms.GroupBox();
            this.cbDeposit = new System.Windows.Forms.CheckBox();
            this.numPaymentHolidays = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.drpTermsType = new System.Windows.Forms.ComboBox();
            this.label19 = new System.Windows.Forms.Label();
            this.txtDeposit = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.txtNoMonths = new System.Windows.Forms.NumericUpDown();
            this.drpLengths = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnAccept = new System.Windows.Forms.Button();
            this.btnCalculate = new System.Windows.Forms.Button();
            this.rtbMessage = new System.Windows.Forms.RichTextBox();
            this.lvAccounts = new System.Windows.Forms.ListView();
            this.accountNo = new System.Windows.Forms.ColumnHeader();
            this.type = new System.Windows.Forms.ColumnHeader();
            this.agreementTotal = new System.Windows.Forms.ColumnHeader();
            this.outstandingBalance = new System.Windows.Forms.ColumnHeader();
            this.btnRefer = new System.Windows.Forms.Button();
            this.btnTermsTypeBand = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.gbTerms.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPaymentHolidays)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNoMonths)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.btnPrint);
            this.groupBox1.Controls.Add(this.gbTerms);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.btnAccept);
            this.groupBox1.Controls.Add(this.btnCalculate);
            this.groupBox1.Controls.Add(this.rtbMessage);
            this.groupBox1.Controls.Add(this.lvAccounts);
            this.groupBox1.Controls.Add(this.btnRefer);
            this.groupBox1.Location = new System.Drawing.Point(8, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(512, 384);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // btnPrint
            // 
            this.btnPrint.Enabled = false;
            this.btnPrint.Image = ((System.Drawing.Image)(resources.GetObject("btnPrint.Image")));
            this.btnPrint.Location = new System.Drawing.Point(464, 72);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(32, 40);
            this.btnPrint.TabIndex = 58;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // gbTerms
            // 
            this.gbTerms.Controls.Add(this.btnTermsTypeBand);
            this.gbTerms.Controls.Add(this.cbDeposit);
            this.gbTerms.Controls.Add(this.numPaymentHolidays);
            this.gbTerms.Controls.Add(this.label7);
            this.gbTerms.Controls.Add(this.label4);
            this.gbTerms.Controls.Add(this.drpTermsType);
            this.gbTerms.Controls.Add(this.label19);
            this.gbTerms.Controls.Add(this.txtDeposit);
            this.gbTerms.Controls.Add(this.label18);
            this.gbTerms.Controls.Add(this.txtNoMonths);
            this.gbTerms.Controls.Add(this.drpLengths);
            this.gbTerms.Location = new System.Drawing.Point(8, 160);
            this.gbTerms.Name = "gbTerms";
            this.gbTerms.Size = new System.Drawing.Size(496, 80);
            this.gbTerms.TabIndex = 57;
            this.gbTerms.TabStop = false;
            this.gbTerms.Text = "New Agreement Terms";
            // 
            // cbDeposit
            // 
            this.cbDeposit.Enabled = false;
            this.cbDeposit.Location = new System.Drawing.Point(184, 40);
            this.cbDeposit.Name = "cbDeposit";
            this.cbDeposit.Size = new System.Drawing.Size(16, 24);
            this.cbDeposit.TabIndex = 65;
            this.cbDeposit.Click += new System.EventHandler(this.cbDeposit_Click);
            // 
            // numPaymentHolidays
            // 
            this.numPaymentHolidays.Enabled = false;
            this.numPaymentHolidays.Location = new System.Drawing.Point(392, 40);
            this.numPaymentHolidays.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numPaymentHolidays.Name = "numPaymentHolidays";
            this.numPaymentHolidays.Size = new System.Drawing.Size(48, 20);
            this.numPaymentHolidays.TabIndex = 64;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(392, 24);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(96, 16);
            this.label7.TabIndex = 63;
            this.label7.Text = "Payment Holidays";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(8, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(96, 16);
            this.label4.TabIndex = 62;
            this.label4.Text = "Terms Type:";
            // 
            // drpTermsType
            // 
            this.drpTermsType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpTermsType.DropDownWidth = 152;
            this.drpTermsType.Location = new System.Drawing.Point(8, 40);
            this.drpTermsType.Name = "drpTermsType";
            this.drpTermsType.Size = new System.Drawing.Size(152, 21);
            this.drpTermsType.TabIndex = 58;
            this.drpTermsType.SelectedIndexChanged += new System.EventHandler(this.drpTermsType_SelectedIndexChanged);
            // 
            // label19
            // 
            this.label19.Location = new System.Drawing.Point(320, 24);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(64, 16);
            this.label19.TabIndex = 61;
            this.label19.Text = "Instalments:";
            // 
            // txtDeposit
            // 
            this.txtDeposit.Enabled = false;
            this.txtDeposit.Location = new System.Drawing.Point(208, 40);
            this.txtDeposit.Name = "txtDeposit";
            this.txtDeposit.Size = new System.Drawing.Size(96, 20);
            this.txtDeposit.TabIndex = 59;
            this.txtDeposit.Text = "0";
            this.txtDeposit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtDeposit.Validating += new System.ComponentModel.CancelEventHandler(this.txtDeposit_Validating);
            // 
            // label18
            // 
            this.label18.Location = new System.Drawing.Point(208, 24);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(48, 16);
            this.label18.TabIndex = 57;
            this.label18.Text = "Deposit:";
            // 
            // txtNoMonths
            // 
            this.txtNoMonths.Enabled = false;
            this.txtNoMonths.Location = new System.Drawing.Point(320, 40);
            this.txtNoMonths.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txtNoMonths.Name = "txtNoMonths";
            this.txtNoMonths.Size = new System.Drawing.Size(56, 20);
            this.txtNoMonths.TabIndex = 60;
            this.txtNoMonths.ValueChanged += new System.EventHandler(this.txtNoMonths_ValueChanged);
            this.txtNoMonths.Leave += new System.EventHandler(this.txtNoMonths_Leave);
            // 
            // drpLengths
            // 
            this.drpLengths.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpLengths.DropDownWidth = 152;
            this.drpLengths.Enabled = false;
            this.drpLengths.ItemHeight = 13;
            this.drpLengths.Location = new System.Drawing.Point(320, 40);
            this.drpLengths.Name = "drpLengths";
            this.drpLengths.Size = new System.Drawing.Size(56, 21);
            this.drpLengths.TabIndex = 66;
            this.drpLengths.Visible = false;
            this.drpLengths.SelectedIndexChanged += new System.EventHandler(this.drpLengths_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.Highlight;
            this.label2.Location = new System.Drawing.Point(56, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(264, 16);
            this.label2.TabIndex = 6;
            this.label2.Text = "SELECT ACCOUNT(S) TO CONSOLIDATE";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 248);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 16);
            this.label1.TabIndex = 5;
            this.label1.Text = "Add To Results";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(392, 128);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(104, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnAccept
            // 
            this.btnAccept.Enabled = false;
            this.btnAccept.Location = new System.Drawing.Point(392, 72);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(64, 40);
            this.btnAccept.TabIndex = 3;
            this.btnAccept.Text = "Accept (no print)";
            this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
            // 
            // btnCalculate
            // 
            this.btnCalculate.Location = new System.Drawing.Point(392, 32);
            this.btnCalculate.Name = "btnCalculate";
            this.btnCalculate.Size = new System.Drawing.Size(104, 23);
            this.btnCalculate.TabIndex = 2;
            this.btnCalculate.Text = "Calculate";
            this.btnCalculate.Click += new System.EventHandler(this.btnCalculate_Click);
            // 
            // rtbMessage
            // 
            this.rtbMessage.Enabled = false;
            this.rtbMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.rtbMessage.ForeColor = System.Drawing.SystemColors.InfoText;
            this.rtbMessage.Location = new System.Drawing.Point(8, 264);
            this.rtbMessage.Name = "rtbMessage";
            this.rtbMessage.ReadOnly = true;
            this.rtbMessage.Size = new System.Drawing.Size(496, 112);
            this.rtbMessage.TabIndex = 1;
            this.rtbMessage.Text = "";
            // 
            // lvAccounts
            // 
            this.lvAccounts.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.accountNo,
            this.type,
            this.agreementTotal,
            this.outstandingBalance});
            this.lvAccounts.FullRowSelect = true;
            this.lvAccounts.GridLines = true;
            this.lvAccounts.HideSelection = false;
            this.lvAccounts.Location = new System.Drawing.Point(8, 32);
            this.lvAccounts.Name = "lvAccounts";
            this.lvAccounts.Size = new System.Drawing.Size(368, 120);
            this.lvAccounts.TabIndex = 0;
            this.lvAccounts.UseCompatibleStateImageBehavior = false;
            this.lvAccounts.View = System.Windows.Forms.View.Details;
            this.lvAccounts.SelectedIndexChanged += new System.EventHandler(this.lvAccounts_SelectedIndexChanged);
            // 
            // accountNo
            // 
            this.accountNo.Text = "Account Number";
            this.accountNo.Width = 100;
            // 
            // type
            // 
            this.type.Text = "Type";
            this.type.Width = 30;
            // 
            // agreementTotal
            // 
            this.agreementTotal.Text = "Agreement Total";
            this.agreementTotal.Width = 120;
            // 
            // outstandingBalance
            // 
            this.outstandingBalance.Text = "Outstanding Balance";
            this.outstandingBalance.Width = 120;
            // 
            // btnRefer
            // 
            this.btnRefer.Enabled = false;
            this.btnRefer.Location = new System.Drawing.Point(392, 72);
            this.btnRefer.Name = "btnRefer";
            this.btnRefer.Size = new System.Drawing.Size(104, 40);
            this.btnRefer.TabIndex = 59;
            this.btnRefer.Text = "Refer RF";
            this.btnRefer.Visible = false;
            this.btnRefer.Click += new System.EventHandler(this.btnRefer_Click);
            // 
            // btnTermsTypeBand
            // 
            this.btnTermsTypeBand.Image = global::STL.PL.Properties.Resources.TTBands1;
            this.btnTermsTypeBand.Location = new System.Drawing.Point(136, 14);
            this.btnTermsTypeBand.Name = "btnTermsTypeBand";
            this.btnTermsTypeBand.Size = new System.Drawing.Size(24, 22);
            this.btnTermsTypeBand.TabIndex = 67;
            this.btnTermsTypeBand.Visible = false;
            this.btnTermsTypeBand.Click += new System.EventHandler(this.btnTermsTypeBand_Click);
            // 
            // AddTo
            // 
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(526, 387);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddTo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Add To Account";
            this.Load += new System.EventHandler(this.AddTo_Load);
            this.groupBox1.ResumeLayout(false);
            this.gbTerms.ResumeLayout(false);
            this.gbTerms.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPaymentHolidays)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNoMonths)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        /*
		private void FilterTermsType(bool affinity)
		{
			// Optionally filter Terms Types for Affinity and
			// always filter for the account type selected
			string newFilter = "";
			_affinityTerms = affinity;

			if (_affinityTerms)
				newFilter = "Affinity = 'Y'";
			else
				newFilter = "Affinity = 'N'";

			newFilter += " AND AccountType = '" + this._acctType + "'";

			_dvTermsTypes.RowFilter = "(" + newFilter + ") or termstype = 'Terms Types'";
		}
        */

        /// <summary>
        /// Set up the list view to display the accounts available 
        /// for add to.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddTo_Load(object sender, System.EventArgs e)
        {
            try
            {
                Wait();

                // Show button for terms type bands if this is enabled
                this.btnTermsTypeBand.Visible = (Convert.ToBoolean(Country[CountryParameterNames.TermsTypeBandEnabled]));

                foreach (AddToAccount acct in _addToAccountList)
                {
                    lvAccounts.Items.Add(new ListViewItem(new string[] {FormatAccountNo(acct.AccountNo),
																		   acct.AccountType,
																		   acct.AgreementTotal.ToString(DecimalPlaces), 
																		   acct.OutstandingBalance.ToString(DecimalPlaces),
																		   acct.CashPrice.ToString(DecimalPlaces)}));

                }
                btnCalculate.Enabled = false;           // #9496   jec 05/03/12
            }
            catch (Exception ex) { Catch(ex, "AddTo_Load"); }
            finally { StopWait(); }
        }

        private bool AcceptAddTo()
        {
            string termsType = drpTermsType.Text;
            termsType = termsType.Substring(0, termsType.IndexOf("-") - 1);
            decimal curDeposit = Convert.ToDecimal(StripCurrency(txtDeposit.Text));
            decimal deposit = curDeposit;
            short months = Convert.ToInt16(txtNoMonths.Value);
            short paymentHolidays = Convert.ToInt16(numPaymentHolidays.Value);

            if (lvAccounts.SelectedItems.Count > 0)
            {
                int x = 0;
                string[] accounts = new string[lvAccounts.SelectedItems.Count];
                foreach (ListViewItem i in lvAccounts.SelectedItems)
                    accounts[x++] = i.Text.Replace("-", "");

                AccountManager.ProcessAddTo(addToAccountNo, accounts,
                    Config.CountryCode,
                    termsType,
                    this._scoringBand,
                    ref deposit,
                    months,
                    paymentHolidays,
                    this.cbDeposit.Checked,
                    out Error);

                if (Error.Length > 0)
                {
                    ShowError(Error);
                    return false;
                }
                else
                {
                    if (deposit != curDeposit)
                    {
                        txtDeposit.Text = deposit.ToString(DecimalPlaces);
                    }
                    ((BasicCustomerDetails)FormParent).txtCustID_Leave(null, null);
                    RefreshAccountScreens(addToAccountNo, accounts);
                    return true;
                }
            }
            else
            {
                ShowInfo("M_NOACCOUNTSELECTED");
                return false;
            }
        }

        /// <summary>
        /// Find any open account screens that are involved in the add to and reload them
        /// </summary>
        /// <param name="addToAccount"></param>
        /// <param name="accounts"></param>
        private void RefreshAccountScreens(string addToAccount, string[] accounts)
        {
            foreach (Crownwood.Magic.Controls.TabPage tp in ((MainForm)FormRoot).MainTabControl.TabPages)
            {
                if (tp.Control is NewAccount)
                {
                    NewAccount acct = (NewAccount)tp.Control;

                    if (acct.txtAccountNumber.UnformattedText == addToAccount)
                    {
                        acct.SupressEvents = true;
                        acct.loadAccountData(addToAccount, false);
                        acct.SupressEvents = false;
                    }

                    foreach (string s in accounts)
                        if (acct.txtAccountNumber.UnformattedText == s)
                        {
                            acct.SupressEvents = true;
                            acct.loadAccountData(s, false);
                            acct.SupressEvents = false;
                        }
                }
            }
        }

        private void SetResult(bool enabled, bool refer)
        {
            // Enabled should be true after calculating an Add-To
            // Refer will be true if the add-to exceeds the RF Credit Limit

            // Enable or disable the result fields / buttons
            this.rtbMessage.Enabled = (enabled || refer);
            if (!this.rtbMessage.Enabled) this.rtbMessage.Clear();
            this.btnAccept.Enabled = (enabled && !refer);
            this.btnPrint.Enabled = (enabled && !refer);
            // The refer btn is normally hidden
            this.btnAccept.Visible = !refer;
            this.btnPrint.Visible = !refer;
            this.btnRefer.Visible = refer;
            this.btnRefer.Enabled = refer;
            if (lvAccounts.SelectedItems.Count > 0)         // #9496   jec 05/03/12
            {
                this.btnCalculate.Enabled = true;
            }
            else
            {
                this.btnCalculate.Enabled = false;
            }

        }

        private void btnCalculate_Click(object sender, System.EventArgs e)
        {
            Function = "btnCalculate_Click";

            try
            {
                Wait();

                string termsType = drpTermsType.Text;
                termsType = termsType.Substring(0, termsType.IndexOf("-") - 1);
                decimal curDeposit = Convert.ToDecimal(StripCurrency(txtDeposit.Text));
                decimal deposit = curDeposit;
                short months = Convert.ToInt16(txtNoMonths.Value);
                short paymentHolidays = Convert.ToInt16(numPaymentHolidays.Value);

                if (lvAccounts.SelectedItems.Count > 0)
                {
                    string[] accounts = new string[lvAccounts.SelectedItems.Count];
                    decimal sumBalances = 0;
                    decimal newCashPrice = 0;
                    decimal newAgreementTotal = 0;
                    decimal newMonthlyInstal = 0;
                    decimal newFinalInstal = 0;
                    int newNoInstalments = 0;
                    int x = 0;
                    string excludeList = "";
                    string prefix = "'";
                    decimal addToAvailable = 0.0M;
                    bool wrongType = false;

                    foreach (ListViewItem i in lvAccounts.SelectedItems)
                    {
                        accounts[x] = i.Text.Replace("-", "");
                        excludeList += prefix + accounts[x] + "'";
                        prefix = ",'";
                        x++;
                    }

                    CreditManager.GetRFLimit(this._customerId, excludeList, AT.ReadyFinance, out this._RFCreditLimit, out addToAvailable, out wrongType, out Error);
                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                    {
                        AccountManager.CalculateAddTo(
                            addToAccountNo,
                            accounts,
                            Config.CountryCode,
                            Convert.ToInt16(Config.BranchCode),
                            termsType,
                            this._scoringBand,
                            ref deposit,
                            months,
                            paymentHolidays,
                            this.cbDeposit.Checked,
                            out sumBalances,
                            out newCashPrice,
                            out newAgreementTotal,
                            out newMonthlyInstal,
                            out newFinalInstal,
                            out newNoInstalments,
                            out Error);
                        if (Error.Length > 0)
                            ShowError(Error);
                        else
                        {
                            // Check the new agreement total is not more than the RF credit limit for this Add-To
                            this._creditRemaining = addToAvailable + deposit - sumBalances;
                            if (this._acctType == AT.ReadyFinance && this._creditRemaining < 0)
                            {
                                rtbMessage.Text = GetResource("M_ADDTORESULTREFER", new object[] { FormatAccountNo(addToAccountNo),
																									  newCashPrice.ToString(DecimalPlaces),
																									  newAgreementTotal.ToString(DecimalPlaces),
																									  newMonthlyInstal.ToString(DecimalPlaces),
																									  newNoInstalments,
																									  addToAvailable.ToString(DecimalPlaces),
																									  (-this._creditRemaining).ToString(DecimalPlaces)});
                                SetResult(false, true);
                            }
                            else
                            {
                                rtbMessage.Text = GetResource("M_ADDTORESULT", new object[] { FormatAccountNo(addToAccountNo),
																								 newCashPrice.ToString(DecimalPlaces),
																								 newAgreementTotal.ToString(DecimalPlaces),
																								 newMonthlyInstal.ToString(DecimalPlaces),
																								 newNoInstalments });
                                SetResult(true, false);
                            }

                            if (deposit != curDeposit)
                            {
                                txtDeposit.Text = deposit.ToString(DecimalPlaces);
                                rtbMessage.Text += GetResource("M_ADDTODEPOSIT", new object[] { deposit.ToString(DecimalPlaces) });
                            }
                        }
                    }
                }
                else
                    ShowInfo("M_NOACCOUNTSELECTED");
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

        private void btnAccept_Click(object sender, System.EventArgs e)
        {
            Function = "btnAccept_Click";

            try
            {
                Wait();
                if (this.AcceptAddTo()) this.Close();
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

        private void lvAccounts_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.SetResult(false, false);
        }

        /// <summary>
        /// When the terms type is selected get back the values specific to this
        /// terms type. Based on this information we know whether to enable the 
        /// deposit field or not.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void drpTermsType_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            Function = "drpTermsType_SelectedIndexChanged";
            try
            {
                Wait();

                SetResult(false, false);
                if (_userChanged && this._canSetTermsType)
                {
                    decimal newDeposit = Convert.ToDecimal(StripCurrency(txtDeposit.Text));

                    SetTermsType(drpTermsType, numPaymentHolidays, cbDeposit, txtDeposit, txtNoMonths,
                        drpLengths, false, 0, false, ref _defaultDeposit, ref _depositIsPercentage,
                        ref newDeposit, txtNoMonths.Value, false);

                    this.btnCalculate.Enabled = (drpTermsType.SelectedIndex != 0 && txtNoMonths.Value > 0);
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

        private void drpLengths_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            Function = "drpLengths_SelectedIndexChanged";
            try
            {
                Wait();

                SetResult(false, false);
                if (_userChanged && this._canSetTermsType)
                {
                    // Copy the selected length to the number of months
                    // This should fire the txtNoMonths_ValueChanged event
                    if (IsStrictNumeric(this.drpLengths.Text))
                        txtNoMonths.Value = Convert.ToDecimal(this.drpLengths.Text);
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

        private void txtNoMonths_Leave(object sender, System.EventArgs e)
        {
            Function = "txtNoMonths_Leave";
            try
            {
                Wait();

                SetResult(false, false);
                if (_userChanged && this._canSetTermsType)
                {
                    // Copy the selected length to the number of months
                    // This should fire the txtNoMonths_ValueChanged event
                    if (IsStrictNumeric(txtNoMonths.Text))
                        txtNoMonths.Value = Convert.ToDecimal(txtNoMonths.Text);
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

        private void txtNoMonths_ValueChanged(object sender, System.EventArgs e)
        {
            Function = "txtNoMonths_ValueChanged";
            try
            {
                Wait();

                SetResult(false, false);
                if (_userChanged && this._canSetTermsType)
                {
                    decimal max = Convert.ToDecimal(Math.Floor(Convert.ToDouble(txtNoMonths.Value * 0.1M)));
                    if (numPaymentHolidays.Value > max)
                    {
                        numPaymentHolidays.Value = max;
                    }

                    this.btnCalculate.Enabled = (drpTermsType.SelectedIndex != 0 && txtNoMonths.Value > 0);
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

        private void txtDeposit_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Function = "txtDeposit_Validating";
            try
            {
                Wait();

                SetResult(false, false);
                if (_userChanged && this._canSetTermsType)
                {
                    decimal newDeposit = Convert.ToDecimal(StripCurrency(txtDeposit.Text));
                    SetDeposit(this.cbDeposit, this.txtDeposit, _defaultDeposit, _depositIsPercentage,
                        ref newDeposit, 0, false, false);
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

        private void cbDeposit_Click(object sender, System.EventArgs e)
        {
            Function = "cbDeposit_Click";
            try
            {
                Wait();

                SetResult(false, false);
                if (_userChanged && this._canSetTermsType)
                {
                    decimal newDeposit = Convert.ToDecimal(StripCurrency(txtDeposit.Text));
                    SetDeposit(this.cbDeposit, this.txtDeposit, _defaultDeposit, _depositIsPercentage,
                        ref newDeposit, 0, false, false);
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

        private void btnPrint_Click(object sender, System.EventArgs e)
        {
            Function = "btnPrint_Click";
            try
            {
                Wait();

                if (this._customerId.Trim().Length > 0)
                {
                    if (this.AcceptAddTo())
                    {
                        // Get the Sales Person
                        int salesPerson = Credential.UserId;

                        // Get the Line Items to print
                        XmlDocument itemDoc = new XmlDocument();
                        XmlNode lineItems = AccountManager.GetLineItems(addToAccountNo, 1, this._acctType, Config.CountryCode, Convert.ToInt16(Config.BranchCode), out Error);
                        if (Error.Length > 0)
                            ShowError(Error);
                        else
                        {
                            if (lineItems != null)
                            {
                                lineItems = itemDoc.ImportNode(lineItems, true);
                                //itemDoc.ReplaceChild(lineItems, itemDoc.DocumentElement);
                                //itemDoc.AppendChild(lineItems);

                                PrintAgreementDocuments(addToAccountNo,
                                    this._acctType,
                                    this._customerId,
                                    false,
                                    false,
                                    0, 0,
                                    itemDoc.DocumentElement,
                                    1,
                                    this,
                                    true,
                                    salesPerson,
                                    this._payMethod);

                                this.Close();
                            }
                        }

                    }
                }
                else
                {
                    ShowInfo("M_NOCUSTOMERFORPRINT");
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

        private void btnRefer_Click(object sender, System.EventArgs e)
        {
            Function = "btnRefer_Click";

            try
            {
                Wait();
                // Refer this account
                DataSet proposalSet = CreditManager.GetProposalStage1(this._customerId, this.addToAccountNo, SM.New, "H", out Error);
                if (Error.Length > 0)
                {
                    ShowError(Error);
                }
                else
                {
                    DateTime dateProp = StaticDataManager.GetServerDateTime();
                    foreach (DataTable dt in proposalSet.Tables)
                        if (dt.TableName == TN.Proposal)
                            foreach (DataRow row in dt.Rows)
                                dateProp = System.Convert.ToDateTime(row[CN.DateProp]);

                    decimal requiredExtra = -this._creditRemaining;
                    decimal requiredLimit = this._RFCreditLimit + requiredExtra;
                    string referralNote = GetResource("M_ADDTORESULTREFERINFO",
                        new object[] {this.addToAccountNo,
										  this._RFCreditLimit.ToString(DecimalPlaces),
										  requiredExtra.ToString(DecimalPlaces),
										  requiredLimit.ToString(DecimalPlaces)});
                    CreditManager.SpendLimitReferral(this._customerId, this.addToAccountNo, dateProp, referralNote, this._RFCreditLimit, Config.CountryCode, out Error);
                }
                this.Close();
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

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                Function = "btnCancel_Click";
                this.Close();
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

        private void btnTermsTypeBand_Click(object sender, EventArgs e)
        {
            try
            {
                Function = "btnTermsTypeBand_Click";
                Wait();

                // Call the terms type band popup form
                TermsTypeMatrixPopup TermsTypeBand = new TermsTypeMatrixPopup(
                    this.FormRoot, this.FormParent,
                    this._dvTermsTypes,
                    ((DataRowView)drpTermsType.SelectedItem)[CN.TermsTypeCode].ToString(),
                    this._scoringBand,
                    this.addToAccountNo,
                    customerPClubCode,
                    privilegeClubDesc,
                    false,
                    this._acctType, SType, IsLoan, _customerId);

                if (TermsTypeBand.ShowDialog() == DialogResult.OK)
                {
                    // Set the selected band
                    this._scoringBand = TermsTypeBand.band;
                    // Filter the terms for the new band
                    DataView dvTermsType = (DataView)drpTermsType.DataSource;

                    // Get the current setting
                    string curTermsType = drpTermsType.Text;

                    FilterTermsType(ref dvTermsType, false, this._acctType, _scoringBand, SType, IsLoan);

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
                    this.drpTermsType_SelectedIndexChanged(sender, e);
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

    }
}
