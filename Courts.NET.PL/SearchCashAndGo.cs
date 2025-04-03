using System;
using STL.PL.WS2;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using Crownwood.Magic.Menus;
using System.Windows.Forms;
using System.Web.Services.Protocols;
using STL.Common;
using STL.Common.Static;
using STL.Common.Constants.ColumnNames;
using System.Text.RegularExpressions;
using System.Xml;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.TableNames;
using mshtml;
using AxSHDocVw;
using System.Drawing.Printing;
using System.Text;
using Blue.Cosacs.Shared;


namespace STL.PL
{
	/// <summary>
	/// Lists Cash and Go stock items for a certain branch number and date range.
	/// Alternatively an invoice number (buff number) can be entered to retrieve
	/// a known invoice. The individual items can then be returned or replaced.
	/// </summary>
	public class SearchCashAndGo : CommonForm
	{
        private DataSet fields = null;
        private DataView payments = null;                                       //IP - 08/05/12 - #9608 - CR8520
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.DataGrid dgAccounts;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button btnClear;
		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.TextBox txtBranch;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtInvoiceNum;
		private System.Windows.Forms.Label label2;
		//private Crownwood.Magic.Menus.MenuControl menuMain;
		private Crownwood.Magic.Menus.MenuCommand menuFile;
		private Crownwood.Magic.Menus.MenuCommand menuExit;
		private System.Windows.Forms.DateTimePicker dtEnd;
		private System.Windows.Forms.DateTimePicker dtStart;
		private System.Windows.Forms.Button btnSearch;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ErrorProvider errorProvider1;
        private int index = 0;
        private IContainer components;
		private string _acctno = "";
		private System.Windows.Forms.Label menuInstantReplacement;
		private System.Windows.Forms.Label lAuthorise;
		private new string Error = "";
	
		public string AccountNo
		{
			get{return _acctno;}
			set{_acctno = value;}
		}

        private int _agreementno = 1;
        private Button btnPrintAll;

    
		public int AgreementNo
		{
			get{return _agreementno;}
			set{_agreementno = value;}
		}     


        //IP - 08/05/12 - #9608 - CR8520
        private int SalesPerson
        {
            get;
            set;
        }

        //IP - 17/05/12 - #9447 - CR1239
        private string SalesPersonName
        {
            get;
            set;
        }

        //IP - 17/05/12 - #9447 - CR1239
        private int CashierEmpeeNo
        {
            get;
            set;
        }

        //IP - 17/05/12 - #9447 - CR1239
        private string CashierName
        {
            get;
            set;
        }

        //IP - 18/05/12 - #10144 - CR1239
        private bool TaxExempt
        {
            get;
            set;
        }

        //IP - 21/05/12 - #10145 - CR1239
        private decimal Change
        {
            get;
            set;
        }
        
        //IP - 09/05/12 - #9609 - CR8520
        private int rowCount
        {
            get;
            set;
        }

        UserRight ReprintInvoice = UserRight.Create("ReprintReceipt");          //IP - 09/05/12 - #9608 - CR8520
        UserRight PrintAll = UserRight.Create("PrintAllReceipts");              //IP - 09/05/12 - #9608 - CR8520

		public SearchCashAndGo(TranslationDummy d )
		{
			InitializeComponent();
			menuMain = new Crownwood.Magic.Menus.MenuControl();
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile});
		}

		public SearchCashAndGo(Form root, Form parent)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			HashMenus();
			ApplyRoleRestrictions();
            CheckUserRights(ReprintInvoice, PrintAll);                                    //IP - 09/05/12 - #9608 - CR8520
           
			menuMain = new Crownwood.Magic.Menus.MenuControl();
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile});
			FormRoot = root;
			FormParent = parent;

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			txtBranch.Text = Config.BranchCode;
			dtStart.Value = DateTime.Today;
			dtEnd.Value = DateTime.Today;

            if (PrintAll.IsAllowed)                                                      //IP - 09/05/12 - #9608 - CR8520
            {
                btnPrintAll.Visible = true;
            }
            else
            {
                btnPrintAll.Visible = false;
            }
           
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchCashAndGo));
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.dgAccounts = new System.Windows.Forms.DataGrid();
            this.menuInstantReplacement = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnPrintAll = new System.Windows.Forms.Button();
            this.lAuthorise = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.dtEnd = new System.Windows.Forms.DateTimePicker();
            this.dtStart = new System.Windows.Forms.DateTimePicker();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.txtBranch = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtInvoiceNum = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgAccounts)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox3.CausesValidation = false;
            this.groupBox3.Controls.Add(this.dgAccounts);
            this.groupBox3.Controls.Add(this.menuInstantReplacement);
            this.groupBox3.Location = new System.Drawing.Point(8, 120);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(776, 352);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Search Results";
            // 
            // dgAccounts
            // 
            this.dgAccounts.CaptionText = "Accounts";
            this.dgAccounts.DataMember = "";
            this.dgAccounts.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgAccounts.Location = new System.Drawing.Point(8, 16);
            this.dgAccounts.Name = "dgAccounts";
            this.dgAccounts.ReadOnly = true;
            this.dgAccounts.Size = new System.Drawing.Size(760, 328);
            this.dgAccounts.TabIndex = 0;
            this.dgAccounts.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgAccounts_MouseUp);
            // 
            // menuInstantReplacement
            // 
            this.menuInstantReplacement.Enabled = false;
            this.menuInstantReplacement.Location = new System.Drawing.Point(248, 80);
            this.menuInstantReplacement.Name = "menuInstantReplacement";
            this.menuInstantReplacement.Size = new System.Drawing.Size(72, 16);
            this.menuInstantReplacement.TabIndex = 44;
            this.menuInstantReplacement.Text = "dummyMenu";
            this.menuInstantReplacement.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.CausesValidation = false;
            this.groupBox1.Controls.Add(this.btnPrintAll);
            this.groupBox1.Controls.Add(this.lAuthorise);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.dtEnd);
            this.groupBox1.Controls.Add(this.dtStart);
            this.groupBox1.Controls.Add(this.btnSearch);
            this.groupBox1.Controls.Add(this.btnClear);
            this.groupBox1.Controls.Add(this.btnClose);
            this.groupBox1.Controls.Add(this.txtBranch);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtInvoiceNum);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(8, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(776, 118);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Search Criteria";
            // 
            // btnPrintAll
            // 
            this.btnPrintAll.CausesValidation = false;
            this.btnPrintAll.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnPrintAll.Location = new System.Drawing.Point(549, 83);
            this.btnPrintAll.Name = "btnPrintAll";
            this.btnPrintAll.Size = new System.Drawing.Size(75, 23);
            this.btnPrintAll.TabIndex = 45;
            this.btnPrintAll.Text = "Print All";
            this.btnPrintAll.Click += new System.EventHandler(this.btnPrintAll_Click);
            // 
            // lAuthorise
            // 
            this.lAuthorise.Enabled = false;
            this.lAuthorise.Location = new System.Drawing.Point(48, 88);
            this.lAuthorise.Name = "lAuthorise";
            this.lAuthorise.Size = new System.Drawing.Size(16, 16);
            this.lAuthorise.TabIndex = 44;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(277, 32);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(120, 16);
            this.label4.TabIndex = 43;
            this.label4.Text = "Sales date between:";
            // 
            // dtEnd
            // 
            this.dtEnd.CustomFormat = "ddd dd MMM yyyy";
            this.dtEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtEnd.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.dtEnd.Location = new System.Drawing.Point(405, 48);
            this.dtEnd.Name = "dtEnd";
            this.dtEnd.Size = new System.Drawing.Size(112, 20);
            this.dtEnd.TabIndex = 3;
            this.dtEnd.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            // 
            // dtStart
            // 
            this.dtStart.CustomFormat = "ddd dd MMM yyyy";
            this.dtStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtStart.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.dtStart.Location = new System.Drawing.Point(277, 48);
            this.dtStart.Name = "dtStart";
            this.dtStart.Size = new System.Drawing.Size(112, 20);
            this.dtStart.TabIndex = 2;
            this.dtStart.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            // 
            // btnSearch
            // 
            this.btnSearch.CausesValidation = false;
            this.btnSearch.Image = ((System.Drawing.Image)(resources.GetObject("btnSearch.Image")));
            this.btnSearch.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnSearch.Location = new System.Drawing.Point(549, 48);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 4;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnClear
            // 
            this.btnClear.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnClear.Location = new System.Drawing.Point(678, 49);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 6;
            this.btnClear.Text = "&Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click_1);
            // 
            // btnClose
            // 
            this.btnClose.CausesValidation = false;
            this.btnClose.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnClose.Location = new System.Drawing.Point(678, 19);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "&Exit";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // txtBranch
            // 
            this.txtBranch.Location = new System.Drawing.Point(165, 48);
            this.txtBranch.MaxLength = 30;
            this.txtBranch.Name = "txtBranch";
            this.txtBranch.Size = new System.Drawing.Size(88, 20);
            this.txtBranch.TabIndex = 1;
            this.txtBranch.Tag = "FNAME";
            this.txtBranch.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtBranch.Validating += new System.ComponentModel.CancelEventHandler(this.txtBranch_Validating);
            // 
            // label3
            // 
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(165, 32);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "Branch:";
            // 
            // txtInvoiceNum
            // 
            this.txtInvoiceNum.Location = new System.Drawing.Point(45, 48);
            this.txtInvoiceNum.MaxLength = 15;
            this.txtInvoiceNum.Name = "txtInvoiceNum";
            this.txtInvoiceNum.Size = new System.Drawing.Size(88, 20);
            this.txtInvoiceNum.TabIndex = 0;
            this.txtInvoiceNum.Tag = "CUSTID";
            this.txtInvoiceNum.Text = "0";
            this.txtInvoiceNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtInvoiceNum.Validating += new System.ComponentModel.CancelEventHandler(this.txtInvoiceNum_Validating);
            // 
            // label2
            // 
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(45, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Invoice Number:";
            // 
            // menuFile
            // 
            this.menuFile.Description = "MenuItem";
            this.menuFile.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuExit});
            this.menuFile.Text = "&File";
            // 
            // menuExit
            // 
            this.menuExit.Description = "MenuItem";
            this.menuExit.Text = "E&xit";
            this.menuExit.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.DataMember = "";
            // 
            // SearchCashAndGo
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox3);
            this.Name = "SearchCashAndGo";
            this.Text = "Search Cash and Go";
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgAccounts)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		private bool valid = true;

		private bool Valid()
		{
			valid = true;
			ValidateDates();
			txtInvoiceNum_Validating(null, null);
			txtBranch_Validating(null, null);
			return valid;
		}

		private void HashMenus()
		{
			dynamicMenus = new Hashtable();
			dynamicMenus[this.Name+":menuInstantReplacement"] = this.menuInstantReplacement;
		}

		private void btnSearch_Click(object sender, System.EventArgs e)
		{
			Function = "btnSearch_Click";
			
			try
			{
				Wait();

                if (Valid())
                {
                    // Clear the data grid
                    dgAccounts.DataSource = null;

                    ((MainForm)this.FormRoot).statusBar1.Text = "Searching for accounts";

                    int branch = 0;
                    long buffno = 0;

                    if (this.txtInvoiceNum.Text.Length > 0)
                        if (long.TryParse(this.txtInvoiceNum.Text, out buffno))
                        {

                            if (this.txtBranch.Text.Length > 0)
                                branch = Convert.ToInt32(this.txtBranch.Text);

                            fields = AccountManager.GetCashAndGoAccts(buffno, branch, dtStart.Value,
                                                                        dtEnd.Value, false, out Error);

                            if (Error.Length > 0)
                                ShowError(Error);
                            else
                            {
                                if (fields != null)
                                {
                                    ((MainForm)this.FormRoot).statusBar1.Text = fields.Tables[0].Rows.Count + " Row(s) returned.";

                                    if (fields.Tables[0].Rows.Count > 0)
                                    {

                                        fields.Tables[0].DefaultView.AllowNew = false;
                                        dgAccounts.DataSource = fields.Tables[0].DefaultView;

                                        payments = fields.Tables[1].DefaultView;                    //IP - 08/05/12 - #9608 - CR8520

                                        if (dgAccounts.TableStyles.Count == 0)
                                        {
                                            DataGridTableStyle tabStyle = new DataGridTableStyle();
                                            tabStyle.MappingName = fields.Tables[0].TableName;
                                            dgAccounts.TableStyles.Add(tabStyle);

                                            /* set up the header text */
                                            tabStyle.GridColumnStyles[CN.BuffNo].HeaderText = GetResource("T_INVOICE_NUMBER");
                                            tabStyle.GridColumnStyles[CN.InvoiceNo].HeaderText = GetResource("T_INVOICENOCASHGO");
                                            tabStyle.GridColumnStyles[CN.ItemNo].HeaderText = GetResource("T_ITEMNO");
                                            tabStyle.GridColumnStyles[CN.ItemDescr1].HeaderText = GetResource("T_ITEM_DESCRIPTION");
                                            tabStyle.GridColumnStyles[CN.TransValue].HeaderText = GetResource("T_VALUE");
                                            tabStyle.GridColumnStyles[CN.Discount].HeaderText = GetResource("T_DISCOUNT");
                                            tabStyle.GridColumnStyles[CN.Quantity].HeaderText = GetResource("T_QUANTITY");
                                            tabStyle.GridColumnStyles[CN.Price].HeaderText = GetResource("T_UNITPRICE");
                                            tabStyle.GridColumnStyles[CN.WarrantyNo].HeaderText = GetResource("T_WARRANTY_NO");
                                            tabStyle.GridColumnStyles[CN.StockLocn].HeaderText = GetResource("T_STOCKLOCN");
                                            tabStyle.GridColumnStyles[CN.ContractNo].HeaderText = GetResource("T_CONTRACTNO");

                                            /* set up the column widths */
                                            tabStyle.GridColumnStyles[CN.BuffNo].Width = 1;
                                            tabStyle.GridColumnStyles[CN.InvoiceNo].Width = 100;
                                            tabStyle.GridColumnStyles[CN.ItemNo].Width = 75;
                                            tabStyle.GridColumnStyles[CN.ItemDescr1].Width = 115;
                                            tabStyle.GridColumnStyles[CN.TransValue].Width = 100;
                                            tabStyle.GridColumnStyles[CN.Discount].Width = 80;
                                            tabStyle.GridColumnStyles[CN.Quantity].Width = 40;
                                            tabStyle.GridColumnStyles[CN.Price].Width = 100;
                                            tabStyle.GridColumnStyles[CN.WarrantyNo].Width = 75;
                                            tabStyle.GridColumnStyles[CN.acctno].Width = 0;
                                            tabStyle.GridColumnStyles[CN.StockLocn].Width = 60;
                                            tabStyle.GridColumnStyles[CN.AgreementDate].Width = 0;
                                            tabStyle.GridColumnStyles[CN.TaxRate].Width = 0;
                                            tabStyle.GridColumnStyles[CN.ValueControlled].Width = 0;
                                            tabStyle.GridColumnStyles[CN.ContractNo].Width = 75;
                                            tabStyle.GridColumnStyles[CN.EmpeeNoSale].Width = 0;
                                            tabStyle.GridColumnStyles[CN.ItemID].Width = 0;                    //IP - 09/05/12 - #9608 - CR8520
                                            tabStyle.GridColumnStyles[CN.WarItemID].Width = 0;                 //IP - 09/05/12 - #9608 - CR8520
                                            tabStyle.GridColumnStyles[CN.SalesPersonName].Width = 0;           //IP - 17/05/12 - #9447 - CR1239
                                            tabStyle.GridColumnStyles[CN.CashierEmpeeNo].Width = 0;            //IP - 17/05/12 - #9447 - CR1239
                                            tabStyle.GridColumnStyles[CN.CashierName].Width = 0;               //IP - 17/05/12 - #9447 - CR1239
                                            tabStyle.GridColumnStyles[CN.TaxExempt].Width = 0;                 //IP - 18/05/12 - #10144 - CR1239
                                            tabStyle.GridColumnStyles[CN.Change].Width = 0;                    //IP - 21/05/12 - #10145 - CR1239
                                            tabStyle.GridColumnStyles["accttype"].Width = 0;                   //#18435


                                            tabStyle.GridColumnStyles[CN.ContractNo].NullText = "";

                                            /* set alignment */
                                            tabStyle.GridColumnStyles[CN.TransValue].Alignment = HorizontalAlignment.Right;
                                            tabStyle.GridColumnStyles[CN.Price].Alignment = HorizontalAlignment.Right;
                                            tabStyle.GridColumnStyles[CN.Discount].Alignment = HorizontalAlignment.Right;

                                            ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.TransValue]).Format = DecimalPlaces;
                                            ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.Price]).Format = DecimalPlaces;
                                            ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.Discount]).Format = DecimalPlaces;
                                        }
                                    }

                                    rowCount = fields.Tables[0].Rows.Count;                 //IP - 09/05/12 - #9608 - CR8520
                                }
                                else
                                {
                                    ((MainForm)this.FormRoot).statusBar1.Text = "0 Row(s) returned.";
                                }
                            }
                        }
                }
				Function = "End of btnSearch_Click";
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}	
			finally
			{
				StopWait();
			}		
		}

		private void Clear() 
		{
			txtBranch.Text = Config.BranchCode;
			txtInvoiceNum.Text = "";
			dgAccounts.DataSource = null;
			dtStart.Value = DateTime.Today;
			dtEnd.Value = DateTime.Today;
            fields = null;                      //IP - 11/05/12 - #9609 - CR8520
		}

		private void btnClose_Click(object sender, System.EventArgs e)
		{
			CloseTab();
		}

		private void btnClear_Click_1(object sender, System.EventArgs e)
		{
			Clear();
		}

		private void OnCashAndGoReturn(object sender, System.EventArgs e) 
		{
			try
			{
				Wait();

				AuthorisePrompt ap = new AuthorisePrompt(this, lAuthorise, GetResource("M_PAIDTAKENAUTH"));
				ap.ShowDialog();
				if (ap.Authorised)
				{
					NewAccount revise = new NewAccount(AccountNo, AgreementNo, null, false, FormRoot, this);
					revise.Collection = true;
					revise.Text = GetResource("P_CASH_GO_RETURN");
					revise.btnPrint.Visible = false;
                    revise.CashAndGoReturn = true; //uat(5.2) - 907 4.3 merge // uat(4.3)162, 172 //LW71722
					revise.returnAuthorisedBy = ap.AuthorisedBy;
					if(revise.AccountLoaded)
					{
						((MainForm)FormRoot).AddTabPage(revise,24);
						revise.SupressEvents = false;

                        //IP - 18/03/08 - (69630)
                        //As this is a Cash & Go Return, the 'Link to Customer Account' button,
                        //'Customer Search' menu option
                        //do NOT need to be enabled. Previously, by clicking on this button
                        //multiple times would cause multiple GRT transactions to be posted to 
                        //'fintrans'.
                        revise.btnCustomerSearch.Enabled = false;
                        revise.menuCustomerSearch.Enabled = false;
                        revise.menuPrintReceipt.Enabled = true;
					}
					RemoveLine();
				}
			}
			catch(Exception ex)
			{
				Catch(ex, "OnCashAndGoReplacement");
			}
			finally
			{
				StopWait();
			}
		}

		private void OnWarrantyReplace(object sender, System.EventArgs e) 
		{
			try
			{
				Wait();

				DataView dv = (DataView)dgAccounts.DataSource;
				string ProductCode = (string)dv[index][CN.ItemNo]; 
				string Model = (string)dv[index][CN.ItemNo]; 
				string ProductDescription = (string)dv[index][CN.ItemDescr1]; 
				DateTime DateTrans = (DateTime)dv[index][CN.AgreementDate];
				int buffNo = (int)dv[index][CN.BuffNo];
				decimal price = Convert.ToDecimal(dv[index][CN.Price]);
				decimal quantity = Convert.ToDecimal(dv[index][CN.Quantity]);
				decimal orderValue = Convert.ToDecimal(dv[index][CN.TransValue]);
				string warrantyNo = (string)dv[index][CN.WarrantyNo];
				decimal taxRate = Convert.ToDecimal(dv[index][CN.TaxRate]);
				string contractno = (string)dv[index][CN.ContractNo];
				int empeeNoSale = (int)dv[index][CN.EmpeeNoSale];
                int itemId = (int)dv[index]["ItemId"];                  //RI - CR1212
                //int parentItemId = (int)dv[index]["ParentItemId"];
                int warItemId = (int)dv[index]["WarItemId"];
                string accttype = (string)dv[index]["accttype"];        //#18435

				OneForOneReplacement o = new OneForOneReplacement(AccountNo, ProductCode, 
																	Model, ProductDescription, 
																	DateTrans, this.txtBranch.Text,
																	buffNo,	price, quantity,
																	orderValue, warrantyNo,
																	taxRate, FormRoot, this,
																	contractno, empeeNoSale,
                                                                    itemId, warItemId, accttype);   //#18435      //#17290
				o.ShowDialog();
				RemoveLine();
			}
			catch(Exception ex)
			{
				Catch(ex, "OnWarrantyReplace");
			}
			finally
			{
				StopWait();
			}

		}

		private void RemoveLine()
		{
			/* remove the line given by the curernt value of index from the dg */
			((DataView)dgAccounts.DataSource).Delete(index);
		}

		private void dgAccounts_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			try
			{
				Wait();

				index = dgAccounts.CurrentRowIndex;

				if(index >= 0)
				{
					dgAccounts.Select(dgAccounts.CurrentCell.RowNumber);

					if (e.Button == MouseButtons.Right)
					{
						DataGrid ctl = (DataGrid)sender;
						DataView view = (DataView)dgAccounts.DataSource;
						AccountNo = (string)view[index][CN.acctno];
						AgreementNo = Convert.ToInt32(view[index][CN.BuffNo]);
                        SalesPerson = (int)view[index][CN.EmpeeNoSale];                                                //IP - 08/05/12 - #9608 - CR8520
                        SalesPersonName = (string)view[index][CN.SalesPersonName];                                     //IP - 17/05/12 - #9447 - CR1239
                        CashierEmpeeNo = (int)view[index][CN.CashierEmpeeNo];                                          //IP - 17/05/12 - #9447 - CR1239
                        CashierName = (string)view[index][CN.CashierName];                                             //IP - 17/05/12 - #9447 - CR1239
                        TaxExempt = (bool)view[index][CN.TaxExempt];                                                   //IP - 18/05/12 - #10144 - CR1239
                        Change = (decimal)view[index][CN.Change];                                                      //IP - 21/05/12 - #10145 - CR1239

                        //MenuCommand m1 = new MenuCommand(GetResource("P_CASH_GO_RETURN"));                           //ST - 03/02/21 - #7549781
                        MenuCommand m2 = new MenuCommand(GetResource("P_WARRANTY_REPLACE"));
                        MenuCommand m3 = new MenuCommand(GetResource("P_CASH_GO_REPRINT"));                            //IP - 08/05/12 - #9608 - CR8520

                        //m1.Click += new System.EventHandler(this.OnCashAndGoReturn);                                //ST - 03/02/21 - #7549781
                        m2.Click += new System.EventHandler(this.OnWarrantyReplace);
                        m3.Click += new System.EventHandler(this.ReprintReceipt);                                      //IP - 08/05/12 - #9608 - CR8520

						PopupMenu popup = new PopupMenu(); 

						bool isIR = false;
						AccountManager.IsItemInstantReplacement(view[index]["WarItemId"] != DBNull.Value ?  (int)view[index]["itemID"] : 0,  
							                                    Convert.ToInt16(view[index][CN.StockLocn]), 
							                                    out isIR, 
                                                                out Error);
						if(Error.Length>0)
							ShowError(Error);

						string warrantyno = (string)view[index][CN.WarrantyNo];
						//Regex reg = new Regex("^19.*2$");	/* ^ (starts with) 19 (literal) .(any character) *(zero or more) 2$ (ends with 2)	*/
						if (warrantyno.Length==0 ||
							//!reg.IsMatch(warrantyno) )
							!isIR )
							m2.Enabled = false;

                        // This check to see if the Cash & Go Return option should be 
                        // enabled is no longer valid, as Cash & Go accounts that have
                        // warranties have a different account number to the standard
                        // Cash & Go account number for the branch.
                        //m1.Enabled = AccountNo == AccountManager.GetPaidAndTakenAccount(Config.BranchCode, out Error);

                        //popup.MenuCommands.Add(m1);    //ST - 03/02/21 - #7549781

                        //IP - 09/05/12 - #9608 - CR8520
                        if (ReprintInvoice.IsAllowed)
                        {
                            popup.MenuCommands.Add(m3);                                                              
                        }

                        //#17290
                        if (Credential.HasPermission(CosacsPermissionEnum.InstantReplacementCashAndGo))
                        {
                            popup.MenuCommands.Add(m2);
                        }

                        //if (menuInstantReplacement.Enabled)
                        //    popup.MenuCommands.Add(m2);

						MenuCommand selected = popup.TrackPopup(ctl.PointToScreen(new Point(e.X, e.Y)));
					}
				}
			}
			catch(Exception ex)
			{
				Catch(ex, "dgAccounts_MouseUp");
			}
			finally
			{
				StopWait();
			}
		}

		private void txtInvoiceNum_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			try
			{
				if(!IsStrictNumeric(txtInvoiceNum.Text))
				{
					valid = false;
					errorProvider1.SetError(txtInvoiceNum, GetResource("M_ENTERMANDATORY"));
				}
				else
				{
					valid = valid?true:false;
					errorProvider1.SetError(txtInvoiceNum, "");
				}
			}
			catch(Exception ex)
			{
				Catch(ex, "txtInvoiceNum_Validating");
			}
		}

		private void ValidateDates()
		{
			if(dtStart.Value > dtEnd.Value) 
			{
				valid = false;
				errorProvider1.SetError(dtStart, GetResource("M_ENTERMANDATORY"));
				errorProvider1.SetError(dtEnd, GetResource("M_ENTERMANDATORY"));
			}
			else
			{
				valid = valid?true:false;
				errorProvider1.SetError(dtStart, "");
				errorProvider1.SetError(dtEnd, "");
			}
		}

		private void txtBranch_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			try
			{
				if(!IsStrictNumeric(txtBranch.Text))
				{
					valid = false;
					errorProvider1.SetError(txtBranch, GetResource("M_ENTERMANDATORY"));
				}
				else
				{
					valid = valid?true:false;
					errorProvider1.SetError(txtBranch, "");
				}
			}
			catch(Exception ex)
			{
				Catch(ex, "txtBranch_Validating");
			}
		}

        //IP - 08/05/12 - #9608 - CR8520 - Reprint Cash & Go Receipt
        private void ReprintReceipt(object sender, System.EventArgs e)
        {
            try
            {
                Wait();

                var index = dgAccounts.CurrentRowIndex;

                int noPrints = 0;
                DataView dv = new DataView(fields.Tables[1]);       //DataView of Paymethods used on invoice
                DataSet payMethodSet = new DataSet();
                DataTable payments = new DataTable();

                dgAccounts.CurrentRowIndex = index;

                dv.RowFilter = CN.acctno + "= '" + AccountNo + "' and " + CN.AgrmtNo + "= '" + AgreementNo + "'";
                payments = dv.ToTable();

                var payMethod = payments.Rows.Count > 1 ? Convert.ToInt16(0) : Convert.ToInt16(payments.Rows[0][CN.PayMethod]);

                payMethodSet.Tables.Add(payments);
                payMethodSet.Tables[0].TableName = TN.PayMethodList;

                ((MainForm)FormRoot).browsers = ((CommonForm)FormRoot).CreateBrowserArray(1);

                XmlNode lineItems = AccountManager.GetLineItems(AccountNo, AgreementNo, AT.Special, Config.CountryCode, Convert.ToInt16(Config.BranchCode), out Error);

                this.NewPrintTaxInvoice(AccountNo, AgreementNo,
                accountType: AT.Special,
                customerID: "PAID & TAKEN",
                paidAndTaken: true,
                collection: false,
                replacement:null,
                paid:0,
                change:Change,
                lineItems:lineItems,
                buffNo:AgreementNo,
                browser:((MainForm)FormRoot).browsers[0], 
                noPrints: ref noPrints, 
                creditNote: false, 
                multiple:true, 
                salesPerson: SalesPerson,
                paymentMethod:payMethod,
                payMethodSet:payMethodSet,
                taxExempt:TaxExempt,                                        //IP - 18/05/12 - #10144 - CR1239
                salesPersonName: SalesPersonName,                           //IP - 17/05/12 - #9447 - CR1239
                cashierName: CashierName,                                   //IP - 17/05/12 - #9447 - CR1239
                cashierID: CashierEmpeeNo);                                 //IP - 17/05/12 - #9447 - CR1239

            }
            catch (Exception ex)
            {
                Catch(ex, "OnCashAndGoReplacement");
            }
            finally
            {
                StopWait();
            }
        }

 

        //IP - 10/05/12 - #9609 - CR8520
        private void btnPrintAll_Click(object sender, EventArgs e)
        {
            DataView dvAccts = new DataView(fields.Tables[0].Copy());      //IP - 14/05/12
            DataTable dtAccts = dvAccts.ToTable(true, CN.acctno, CN.BuffNo, CN.EmpeeNoSale, CN.SalesPersonName, CN.CashierEmpeeNo, CN.CashierName, CN.TaxExempt, CN.Change); //IP - 21/05/12 - Added TaxExempt and Change //IP - 17/05/12 - #9447 - CR1239 - added CN.SalesPersonName, CN.CashierEmpeeNo, CN.CashierName

            if (dgAccounts.DataSource!=null && ((DataView)dgAccounts.DataSource).Table.Rows.Count > 0)
            {

                if (DialogResult.OK == ShowInfo("M_BULKPRINTRECEIPTS", new object[] { dtAccts.Rows.Count }, MessageBoxButtons.OKCancel))
                {
                    DataSet payMethodSet = new DataSet();

                    payMethodSet.Tables.Add(fields.Tables[1].Copy());
                    payMethodSet.Tables[0].TableName = TN.PayMethodList;        //Add fintrans transactions to set

                    payMethodSet.Tables.Add(dtAccts);                           //IP - 14/05/12
                    payMethodSet.Tables[1].TableName = TN.Accounts;             //Add the accounts to set

                    this.NewPrintTaxInvoicePaidAndTakenBulk(payMethodSet);
                }
            }

        }
	}
}
