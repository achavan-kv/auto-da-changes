using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml;
using Crownwood.Magic.Menus;
using STL.Common;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using STL.Common.Static;


namespace STL.PL
{
    /// <summary>
    /// Summary description for WriteOffReview.
    /// </summary>
    public class WriteOffReview : CommonForm
    {
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox drpBranch;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGrid dgAccounts;
        private Crownwood.Magic.Menus.MenuCommand menuExit;
        private Crownwood.Magic.Menus.MenuCommand menuFile;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.ComboBox drpCode;
        private string err = "";
        private DataSet dropDownDS = null;
        private System.Windows.Forms.Button btnReject;
        private System.Windows.Forms.Button btnAccept;
        private System.Windows.Forms.CheckBox chxAccepted;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.CheckBox chxLimit;
        private string code = "";
        private string category = "";
        private bool staticLoaded = false;
        public Button btnExportDetails;
        //private DataGridView dgvAccounts;
        private ToolTip toolTip1;
        private IContainer components;

        public WriteOffReview(TranslationDummy d)
        {
            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile });
        }

        public WriteOffReview(Form root, Form parent)
        {
            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile });

            FormRoot = root;
            FormParent = parent;

            dynamicMenus = new Hashtable();
            HashMenus();
            ApplyRoleRestrictions();

            LoadStatic();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WriteOffReview));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnExportDetails = new System.Windows.Forms.Button();
            this.chxLimit = new System.Windows.Forms.CheckBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.chxAccepted = new System.Windows.Forms.CheckBox();
            this.btnReject = new System.Windows.Forms.Button();
            this.btnAccept = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.drpCode = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dgAccounts = new System.Windows.Forms.DataGrid();
            this.drpBranch = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgAccounts)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.btnExportDetails);
            this.groupBox1.Controls.Add(this.chxLimit);
            this.groupBox1.Controls.Add(this.btnClear);
            this.groupBox1.Controls.Add(this.chxAccepted);
            this.groupBox1.Controls.Add(this.btnReject);
            this.groupBox1.Controls.Add(this.btnAccept);
            this.groupBox1.Controls.Add(this.btnLoad);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.drpCode);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.drpBranch);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(8, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(776, 472);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            // 
            // btnExportDetails
            // 
            this.btnExportDetails.Enabled = false;
            this.btnExportDetails.Image = ((System.Drawing.Image)(resources.GetObject("btnExportDetails.Image")));
            this.btnExportDetails.Location = new System.Drawing.Point(712, 21);
            this.btnExportDetails.Name = "btnExportDetails";
            this.btnExportDetails.Size = new System.Drawing.Size(32, 32);
            this.btnExportDetails.TabIndex = 138;
            this.btnExportDetails.Click += new System.EventHandler(this.btnExportDetails_Click);
            this.btnExportDetails.MouseHover += new System.EventHandler(this.btnExportDetails_MouseHover);
            // 
            // chxLimit
            // 
            this.chxLimit.Checked = true;
            this.chxLimit.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chxLimit.Location = new System.Drawing.Point(16, 64);
            this.chxLimit.Name = "chxLimit";
            this.chxLimit.Size = new System.Drawing.Size(104, 24);
            this.chxLimit.TabIndex = 97;
            this.chxLimit.Text = "view top 500";
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(448, 64);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(88, 24);
            this.btnClear.TabIndex = 96;
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // chxAccepted
            // 
            this.chxAccepted.Checked = true;
            this.chxAccepted.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chxAccepted.Location = new System.Drawing.Point(152, 64);
            this.chxAccepted.Name = "chxAccepted";
            this.chxAccepted.Size = new System.Drawing.Size(168, 24);
            this.chxAccepted.TabIndex = 95;
            this.chxAccepted.Text = "Exclude Approved Accounts";
            // 
            // btnReject
            // 
            this.btnReject.Enabled = false;
            this.btnReject.Location = new System.Drawing.Point(594, 63);
            this.btnReject.Name = "btnReject";
            this.btnReject.Size = new System.Drawing.Size(88, 24);
            this.btnReject.TabIndex = 94;
            this.btnReject.Text = "Decline";
            this.btnReject.Visible = false;
            this.btnReject.Click += new System.EventHandler(this.btnReject_Click);
            // 
            // btnAccept
            // 
            this.btnAccept.Enabled = false;
            this.btnAccept.Location = new System.Drawing.Point(594, 24);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(88, 24);
            this.btnAccept.TabIndex = 93;
            this.btnAccept.Text = "Approve";
            this.btnAccept.Visible = false;
            this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Image = ((System.Drawing.Image)(resources.GetObject("btnLoad.Image")));
            this.btnLoad.Location = new System.Drawing.Point(472, 24);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(32, 32);
            this.btnLoad.TabIndex = 25;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(152, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 16);
            this.label3.TabIndex = 23;
            this.label3.Text = "Code:";
            // 
            // drpCode
            // 
            this.drpCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpCode.Location = new System.Drawing.Point(152, 32);
            this.drpCode.Name = "drpCode";
            this.drpCode.Size = new System.Drawing.Size(248, 21);
            this.drpCode.TabIndex = 22;
            this.drpCode.SelectedIndexChanged += new System.EventHandler(this.drpCode_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dgAccounts);
            this.groupBox2.Location = new System.Drawing.Point(16, 96);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(744, 352);
            this.groupBox2.TabIndex = 21;
            this.groupBox2.TabStop = false;
            // 
            // dgAccounts
            // 
            this.dgAccounts.DataMember = "";
            this.dgAccounts.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgAccounts.Location = new System.Drawing.Point(24, 24);
            this.dgAccounts.Name = "dgAccounts";
            this.dgAccounts.ReadOnly = true;
            this.dgAccounts.Size = new System.Drawing.Size(704, 312);
            this.dgAccounts.TabIndex = 0;
            this.dgAccounts.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgAccounts_MouseUp);
            // 
            // drpBranch
            // 
            this.drpBranch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpBranch.Items.AddRange(new object[] {
            "900"});
            this.drpBranch.Location = new System.Drawing.Point(16, 32);
            this.drpBranch.Name = "drpBranch";
            this.drpBranch.Size = new System.Drawing.Size(48, 21);
            this.drpBranch.TabIndex = 18;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(16, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 16);
            this.label1.TabIndex = 17;
            this.label1.Text = "Branch:";
            // 
            // menuExit
            // 
            this.menuExit.Description = "MenuItem";
            this.menuExit.Text = "E&xit";
            // 
            // menuFile
            // 
            this.menuFile.Description = "MenuItem";
            this.menuFile.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuExit});
            this.menuFile.Text = "&File";
            // 
            // WriteOffReview
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.groupBox1);
            this.Name = "WriteOffReview";
            this.Text = "Write Off Review";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgAccounts)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private void HashMenus()
        {
            dynamicMenus[this.Name + ":btnAccept"] = this.btnAccept;
            dynamicMenus[this.Name + ":btnReject"] = this.btnReject;
        }

        private void btnLoad_Click(object sender, System.EventArgs e)
        {
            DataSet ds = null;
            string branchFilter = "";
            int limit = 0;

            try
            {
                Wait();

                dgAccounts.TableStyles.Clear();
                dgAccounts.DataSource = null;

                if ((string)drpBranch.SelectedItem == "ALL")
                    branchFilter = "%";
                else
                    branchFilter = (string)drpBranch.SelectedItem + "%";

                int exclude = Convert.ToInt32(chxAccepted.Checked);

                if (chxLimit.Checked)
                    limit = 500;

                ds = AccountManager.GetForWOReview(code, branchFilter, exclude, limit, category, out err);
                if (err.Length > 0)
                    ShowError(err);
                else
                {
                    if (ds != null)
                    {
                        dgAccounts.DataSource = ds.Tables["Table1"].DefaultView;

                        if (dgAccounts.TableStyles.Count == 0)
                        {
                            DataGridTableStyle tabStyle = new DataGridTableStyle();
                            tabStyle.MappingName = ds.Tables["Table1"].TableName;
                            dgAccounts.TableStyles.Add(tabStyle);

                            tabStyle.GridColumnStyles[CN.acctno].Width = 90;
                            tabStyle.GridColumnStyles[CN.acctno].HeaderText = GetResource("T_ACCOUNTNO");

                            tabStyle.GridColumnStyles[CN.Strategy].Width = 50;

                            tabStyle.GridColumnStyles[CN.StatusCode].Width = 40;
                            tabStyle.GridColumnStyles[CN.StatusCode].HeaderText = GetResource("T_STATUS");  // GetResource("T_STATUSCODE"); jec 27/06/07

                            tabStyle.GridColumnStyles[CN.CustID].Width = 75;
                            tabStyle.GridColumnStyles[CN.CustID].HeaderText = GetResource("T_CUSTID");

                            tabStyle.GridColumnStyles[CN.CustomerName].Width = 110;
                            tabStyle.GridColumnStyles[CN.CustomerName].HeaderText = GetResource("T_NAME");

                            tabStyle.GridColumnStyles[CN.BalExclInt].Width = 115;
                            tabStyle.GridColumnStyles[CN.BalExclInt].HeaderText = GetResource("T_BALANCEEXCLINT");
                            tabStyle.GridColumnStyles[CN.BalExclInt].Alignment = HorizontalAlignment.Right;
                            ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.BalExclInt]).Format = DecimalPlaces;

                            tabStyle.GridColumnStyles[CN.Charges].Width = 70;
                            tabStyle.GridColumnStyles[CN.Charges].HeaderText = GetResource("T_CHARGES");
                            tabStyle.GridColumnStyles[CN.Charges].Alignment = HorizontalAlignment.Right;
                            ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.Charges]).Format = DecimalPlaces;

                            tabStyle.GridColumnStyles[CN.AgreementTotal].Width = 90;
                            tabStyle.GridColumnStyles[CN.AgreementTotal].HeaderText = GetResource("T_AGREETOTAL");
                            tabStyle.GridColumnStyles[CN.AgreementTotal].Alignment = HorizontalAlignment.Right;
                            ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.AgreementTotal]).Format = DecimalPlaces;

                            tabStyle.GridColumnStyles[CN.ProvisionAmount].Width = 90;
                            tabStyle.GridColumnStyles[CN.ProvisionAmount].HeaderText = "Provision Amount";
                            tabStyle.GridColumnStyles[CN.ProvisionAmount].Alignment = HorizontalAlignment.Right;
                            ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.ProvisionAmount]).Format = DecimalPlaces;

                            tabStyle.GridColumnStyles[CN.Code].Width = 40;
                            tabStyle.GridColumnStyles[CN.Code].HeaderText = GetResource("T_CODE");

                            tabStyle.GridColumnStyles[CN.RejectCode].Width = 55;
                            tabStyle.GridColumnStyles[CN.RejectCode].HeaderText = GetResource("T_REJCODE");  //GetResource("T_REJECTCODE");

                            tabStyle.GridColumnStyles[CN.EmployeeName].Width = 90;
                            tabStyle.GridColumnStyles[CN.EmployeeName].HeaderText = GetResource("T_EMPEENAME");

                            tabStyle.GridColumnStyles[CN.ManualName].Width = 120;
                            tabStyle.GridColumnStyles[CN.ManualName].HeaderText = GetResource("T_MANUALNAME");

                            tabStyle.GridColumnStyles[CN.Arrears].Width = 60;
                            tabStyle.GridColumnStyles[CN.Arrears].Alignment = HorizontalAlignment.Right;
                            ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.Arrears]).Format = DecimalPlaces;  //IP - 09/09/10 - CR1107 - WriteOff Review screen Enhancements

                            tabStyle.GridColumnStyles[CN.ArrearsExCharges].Width = 140;
                            tabStyle.GridColumnStyles[CN.ArrearsExCharges].HeaderText = GetResource("T_ARREARSEXCHARGES");
                            tabStyle.GridColumnStyles[CN.ArrearsExCharges].Alignment = HorizontalAlignment.Right;
                            ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.ArrearsExCharges]).Format = DecimalPlaces;  //IP - 09/09/10 - CR1107 - WriteOff Review screen Enhancements

                            tabStyle.GridColumnStyles[CN.Instalment].Width = 80;
                            tabStyle.GridColumnStyles[CN.Instalment].HeaderText = GetResource("T_INSTAL");
                            tabStyle.GridColumnStyles[CN.Instalment].Alignment = HorizontalAlignment.Right;
                            ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.Instalment]).Format = DecimalPlaces;        //IP - 09/09/10 - CR1107 - WriteOff Review screen Enhancements

                            tabStyle.GridColumnStyles[CN.PaymentAmt].Width = 90;
                            tabStyle.GridColumnStyles[CN.PaymentAmt].HeaderText = GetResource("T_PAIDAMOUNT");
                            tabStyle.GridColumnStyles[CN.PaymentAmt].Alignment = HorizontalAlignment.Right;
                            ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.PaymentAmt]).Format = DecimalPlaces;         //IP - 09/09/10 - CR1107 - WriteOff Review screen Enhancements

                            tabStyle.GridColumnStyles[CN.MonthsInArrears].Width = 95;
                            tabStyle.GridColumnStyles[CN.MonthsInArrears].HeaderText = GetResource("T_MONTHSINARREARS");
                            tabStyle.GridColumnStyles[CN.MonthsInArrears].Alignment = HorizontalAlignment.Right;              //IP - 09/09/10 - CR1107 - WriteOff Review screen Enhancements

                            tabStyle.GridColumnStyles[CN.DeliveryDate].Width = 80;
                            tabStyle.GridColumnStyles[CN.DeliveryDate].HeaderText = GetResource("T_DELIVERYDATE");            //IP - 09/09/10 - CR1107 - WriteOff Review screen Enhancements

                            tabStyle.GridColumnStyles[CN.ServiceCharge].Width = 90;
                            tabStyle.GridColumnStyles[CN.ServiceCharge].HeaderText = GetResource("T_SERVICECHARGE2");          //IP - 09/09/10 - CR1107 - WriteOff Review screen Enhancements
                            tabStyle.GridColumnStyles[CN.ServiceCharge].Alignment = HorizontalAlignment.Right;
                            ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.ServiceCharge]).Format = DecimalPlaces;

                            tabStyle.GridColumnStyles[CN.SegmentName2].Width = 120;
                            tabStyle.GridColumnStyles[CN.SegmentName2].HeaderText = GetResource("T_SEGMENTNAME");             //IP - 09/09/10 - CR1107 - WriteOff Review screen Enhancements

                            tabStyle.GridColumnStyles[CN.Rebate].Width = 80;
                            tabStyle.GridColumnStyles[CN.Rebate].HeaderText = GetResource("T_REBATE");                        //IP - 09/09/10 - CR1107 - WriteOff Review screen Enhancements
                            tabStyle.GridColumnStyles[CN.Rebate].Alignment = HorizontalAlignment.Right;
                            ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.Rebate]).Format = DecimalPlaces;

                            tabStyle.GridColumnStyles[CN.BranchNo].Width = 90;
                            tabStyle.GridColumnStyles[CN.BranchNo].HeaderText = GetResource("T_BRANCHNO");                    //IP - 09/09/10 - CR1107 - WriteOff Review screen Enhancements
                            tabStyle.GridColumnStyles[CN.BranchNo].Alignment = HorizontalAlignment.Center;

                            tabStyle.GridColumnStyles[CN.LastTransDate].Width = 120;
                            tabStyle.GridColumnStyles[CN.LastTransDate].HeaderText = GetResource("T_LASTTRANSDATE");          //IP - 09/09/10 - CR1107 - WriteOff Review screen Enhancements



                            //if(code == "REP%")    jec 27/06/07
                            if (code == "REP")
                            {
                                tabStyle.GridColumnStyles[CN.BalBeforeRepo].Width = 115;
                                tabStyle.GridColumnStyles[CN.BalBeforeRepo].HeaderText = GetResource("T_BALBEFORE");
                                tabStyle.GridColumnStyles[CN.BalBeforeRepo].Alignment = HorizontalAlignment.Right;
                                ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.BalBeforeRepo]).Format = DecimalPlaces;

                                tabStyle.GridColumnStyles[CN.RepoValue].Width = 90;
                                tabStyle.GridColumnStyles[CN.RepoValue].HeaderText = GetResource("T_REPOVALUE");
                                tabStyle.GridColumnStyles[CN.RepoValue].Alignment = HorizontalAlignment.Right;
                                ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.RepoValue]).Format = DecimalPlaces;

                                tabStyle.GridColumnStyles[CN.RepPercent].Width = 50;
                                tabStyle.GridColumnStyles[CN.RepPercent].HeaderText = GetResource("T_REPOPC");

                                tabStyle.GridColumnStyles[CN.BalAfterRepo].Width = 105;
                                tabStyle.GridColumnStyles[CN.BalAfterRepo].HeaderText = GetResource("T_BALAFTER");
                                tabStyle.GridColumnStyles[CN.BalAfterRepo].Alignment = HorizontalAlignment.Right;
                                ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.BalAfterRepo]).Format = DecimalPlaces;
                            }
                            else
                            {
                                tabStyle.GridColumnStyles[CN.BalBeforeRepo].Width = 0;
                                tabStyle.GridColumnStyles[CN.RepoValue].Width = 0;
                                tabStyle.GridColumnStyles[CN.RepPercent].Width = 0;
                                tabStyle.GridColumnStyles[CN.BalAfterRepo].Width = 0;
                            }

                            //IP - 29/09/10 - CR1107 - Writeoff Review Enhancements
                            if (code == "MRA")
                            {
                                tabStyle.GridColumnStyles[CN.RepoValue].Width = 90;
                                tabStyle.GridColumnStyles[CN.RepoValue].HeaderText = GetResource("T_REPOVALUE");
                                tabStyle.GridColumnStyles[CN.RepoValue].Alignment = HorizontalAlignment.Right;
                                ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.RepoValue]).Format = DecimalPlaces;

                                //IP - 30/09/10 - CR1107 - Writeoff Review Enhancements
                                tabStyle.GridColumnStyles[CN.ReposDate].Width = 120;
                                tabStyle.GridColumnStyles[CN.ReposDate].HeaderText = GetResource("T_REPOSSESSIONDATE");
                            }

                            //if(code == "NMV%")    --jec 27/06/07
                            if (code == "NMV")
                            {
                                tabStyle.GridColumnStyles[CN.PaidPcent].Width = 50;
                                tabStyle.GridColumnStyles[CN.PaidPcent].HeaderText = GetResource("T_PERCENTAGEPAID2");

                                tabStyle.GridColumnStyles[CN.DateLastPaid].Width = 80;
                                tabStyle.GridColumnStyles[CN.DateLastPaid].HeaderText = GetResource("T_DATELASTPAID");
                            }
                            else
                            {
                                tabStyle.GridColumnStyles[CN.PaidPcent].Width = 0;
                                tabStyle.GridColumnStyles[CN.DateLastPaid].Width = 80;      // 68894 jec 27/06/07
                                tabStyle.GridColumnStyles[CN.DateLastPaid].HeaderText = GetResource("T_DATELASTPAID");
                            }

                            //if(code == "BPT%")    --jec 27/06/07
                            if (code == "BPT")
                            {
                                tabStyle.GridColumnStyles[CN.ClaimNumber].Width = 80;
                                tabStyle.GridColumnStyles[CN.ClaimNumber].HeaderText = GetResource("T_CLAIMNO");
                            }
                            else
                            {
                                tabStyle.GridColumnStyles[CN.ClaimNumber].Width = 0;
                            }
                        }

                        ((MainForm)this.FormRoot).statusBar1.Text = ds.Tables[0].Rows.Count + " Row(s) returned.";
                        btnExportDetails.Enabled = true;
                    }
                    else
                    {
                        ((MainForm)this.FormRoot).statusBar1.Text = "0 Row(s) returned.";
                        btnExportDetails.Enabled = false;
                    }
                }

                StopWait();
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void LoadStatic()
        {
            StringCollection branchNos = new StringCollection();
            branchNos.Add("ALL");

            StringCollection codes = new StringCollection();
            codes.Add("ALL");

            XmlUtilities xml = new XmlUtilities();
            XmlDocument dropDowns = new XmlDocument();
            dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

            if (StaticData.Tables[TN.BranchNumber] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.BranchNumber, null));

            if (StaticData.Tables[TN.WriteOffCodes] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.WriteOffCodes, null));

            if (dropDowns.DocumentElement.ChildNodes.Count > 0)
            {
                dropDownDS = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out err);
                if (err.Length > 0)
                    ShowError(err);
                else
                {
                    foreach (DataTable dt in dropDownDS.Tables)
                        StaticData.Tables[dt.TableName] = dt;
                }
            }

            foreach (DataRow row in ((DataTable)StaticData.Tables[TN.BranchNumber]).Rows)
            {
                branchNos.Add(Convert.ToString(row["branchno"]));
            }
            drpBranch.DataSource = branchNos;

            foreach (DataRow row in ((DataTable)StaticData.Tables[TN.WriteOffCodes]).Rows)
            {
                codes.Add((string)(row[CN.Code]) + " : " + (string)row[CN.CodeDescript]);
            }
            drpCode.DataSource = codes;

            staticLoaded = true;
        }

        private void dgAccounts_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (dgAccounts.CurrentRowIndex >= 0)
                {
                    DataGrid ctl = (DataGrid)sender;

                    MenuCommand m1 = new MenuCommand(GetResource("P_ACCOUNT_DETAILS"));

                    m1.Click += new System.EventHandler(this.OnAccountDetails);

                    PopupMenu popup = new PopupMenu();
                    popup.MenuCommands.AddRange(new MenuCommand[] { m1 });
                    MenuCommand selected = popup.TrackPopup(ctl.PointToScreen(new Point(e.X, e.Y)));
                }
            }
        }

        private void OnAccountDetails(object sender, System.EventArgs e)
        {
            try
            {
                Function = "OnAccountDetails";
                int index = dgAccounts.CurrentRowIndex;

                if (index >= 0)
                {
                    string acctNo = (string)dgAccounts[index, 0];
                    AccountDetails details = new AccountDetails(acctNo, FormRoot, this);
                    ((MainForm)this.FormRoot).AddTabPage(details, 7);
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void btnAccept_Click(object sender, System.EventArgs e)
        {
            try
            {
                DataView dv = (DataView)dgAccounts.DataSource;
                if (dv != null)
                {
                    int count = dv.Count;
                    int accepted = 0;
                    //bool displayed = false;
                    bool status = true;


                    for (int i = count - 1; i >= 0; i--)
                    {
                        if (dgAccounts.IsSelected(i))
                        {
                            AccountManager.AcceptForWO((string)dv[i][CN.acctno], out accepted, out err);
                            if (err.Length > 0)
                            {
                                ShowError(err);
                                status = false;
                            }

                            if (!Convert.ToBoolean(accepted) && status)
                            {
                                AccountManager.SavePending((string)dv[i][CN.acctno], Credential.UserId,
                                    (string)dv[i][CN.Code], 0, Credential.UserId, out err);
                                if (err.Length > 0)
                                {
                                    ShowError(err);
                                    status = false;
                                }
                            }

                            if (status)
                                dv[i][CN.acctno] = "";
                        }
                    }

                    for (int i = count - 1; i >= 0; i--)
                    {
                        if (((string)dv[i][CN.acctno]).Length == 0)
                            dv.Delete(i);
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

        private void btnReject_Click(object sender, System.EventArgs e)
        {
            DataView dv = (DataView)dgAccounts.DataSource;
            int count = dv.Count;
            string category = "BDD";
            string reasonCode = "";
            bool displayed = false;

            try
            {
                for (int i = count - 1; i >= 0; i--)
                {
                    if (dgAccounts.IsSelected(i))
                    {
                        if (!displayed)
                        {
                            ManualWriteOff mwo = new ManualWriteOff(category);
                            mwo.ShowDialog();
                            reasonCode = mwo.reasonCode;
                            displayed = true;
                        }

                        if (reasonCode.Length > 0)
                        {
                            AccountManager.SaveRejection((string)dv[i][CN.acctno], reasonCode, out err);
                            if (err.Length > 0)
                                ShowError(err);

                            dv[i][CN.acctno] = "";
                        }
                    }
                }

                for (int i = count - 1; i >= 0; i--)
                {
                    if (((string)dv[i][CN.acctno]).Length == 0)
                        dv.Delete(i);
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

        private void btnClear_Click(object sender, System.EventArgs e)
        {
            drpBranch.SelectedIndex = 0;
            drpCode.SelectedIndex = 0;
            dgAccounts.TableStyles.Clear();
            dgAccounts.DataSource = null;
        }

        private void drpCode_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (staticLoaded)
            {
                if ((string)drpCode.SelectedItem == "ALL")
                {
                    code = "%";
                    category = "ALL";
                }
                else
                {
                    string codeString = (string)drpCode.SelectedItem;
                    code = codeString.Substring(0, codeString.IndexOf(":") - 1);

                    foreach (DataRow row in ((DataTable)StaticData.Tables[TN.WriteOffCodes]).Rows)
                    {
                        if (code == (string)row[CN.Code])
                            category = (string)row[CN.Category];
                    }
                }
            }
        }

        private void btnExportDetails_Click(object sender, EventArgs e)
        {
            try
            {
                Function = "LaunchExcel";
                Wait();

                /* save the current data grid contents to a CSV */
                string comma = ",";

                if (dgAccounts.CurrentRowIndex >= 0)
                {
                    DataView dv = (DataView)dgAccounts.DataSource;

                    Assembly asm = Assembly.GetExecutingAssembly();
                    string path = "C:\\Temp\\WriteOffReview.csv";
                    FileInfo fi = new FileInfo(path);
                    if (fi.Exists)
                    {
                        fi.Delete();
                    }
                    FileStream fs = fi.OpenWrite();

                    //IP 19/12/2007 - Added (DateStatChge, ServiceRequestNo, CashLoan)
                    string line = GetResource("T_ACCTNO") + comma +
                        CN.Strategy + comma +
                        CN.StatusCode + comma +
                        CN.CustomerID + comma +
                        CN.CustomerName + comma +
                        GetResource("T_BALANCEEXCLINT") + comma +
                        CN.Charges + comma +
                        CN.AgreementTotal + comma +
                        CN.Code + comma +
                        CN.RejectCode + comma +
                        CN.EmployeeName + comma +
                        GetResource("T_BALBEFORE") + comma +
                        GetResource("T_REPOVALUE") + comma +
                        GetResource("T_REPOSSESSIONDATE") + comma +         //IP - 30/09/10 - CR1107 - Writeoff Review Enhancements
                        GetResource("T_REPOPCENT") + comma +
                        GetResource("T_BALAFTER") + comma +
                        GetResource("T_PERCENTAGEPAID") + comma +
                        GetResource("T_DATELASTPAID") + comma +
                        CN.ClaimNumber + comma +
                        CN.ManualName + comma +
                        CN.Arrears + comma +                                //IP - 30/09/10 - CR1107 - Writeoff Review Enhancements
                        GetResource("T_ARREARSEXCHARGES") + comma +         //IP - 30/09/10 - CR1107 - Writeoff Review Enhancements
                        GetResource("T_INSTAL") + comma +                   //IP - 30/09/10 - CR1107 - Writeoff Review Enhancements
                        GetResource("T_PAIDAMOUNT") + comma +               //IP - 30/09/10 - CR1107 - Writeoff Review Enhancements
                        GetResource("T_MONTHSINARREARS") + comma +          //IP - 30/09/10 - CR1107 - Writeoff Review Enhancements
                        GetResource("T_DELIVERYDATE") + comma +             //IP - 30/09/10 - CR1107 - Writeoff Review Enhancements
                        GetResource("T_SERVICECHARGE2") + comma +           //IP - 30/09/10 - CR1107 - Writeoff Review Enhancements
                        GetResource("T_SEGMENTNAME") + comma +              //IP - 30/09/10 - CR1107 - Writeoff Review Enhancements
                        GetResource("T_REBATE") + comma +                   //IP - 30/09/10 - CR1107 - Writeoff Review Enhancements
                        GetResource("T_BRANCHNO") + comma +                 //IP - 30/09/10 - CR1107 - Writeoff Review Enhancements
                        GetResource("T_LASTTRANSDATE") +                    //IP - 30/09/10 - CR1107 - Writeoff Review Enhancements
                        Environment.NewLine + Environment.NewLine;
                    byte[] blob = System.Text.Encoding.UTF8.GetBytes(line);
                    fs.Write(blob, 0, blob.Length);

                    foreach (DataRowView row in dv)
                    {
                        line = "'" + row[CN.acctno] + "'" + comma +
                            row[CN.Strategy] + comma +
                            row[CN.StatusCode] + comma +
                            row[CN.CustID] + comma +
                            row[CN.CustomerName] + comma +
                            row[CN.BalExclInt] + comma +
                            row[CN.Charges] + comma +
                            row[CN.AgreementTotal] + comma +
                            row[CN.Code] + comma +
                            row[CN.RejectCode] + comma +
                            row[CN.EmployeeName] + comma +
                            row[CN.BalBeforeRepo] + comma +
                            row[CN.RepoValue] + comma +
                            row[CN.ReposDate] + comma +                     //IP - 30/09/10 - CR1107 - Writeoff Review Enhancements
                            row[CN.RepPercent] + comma +
                            row[CN.BalAfterRepo] + comma +
                            row[CN.PaidPcent] + comma +
                            row[CN.DateLastPaid] + comma +
                            row[CN.ClaimNumber] + comma +
                            row[CN.ManualName] + comma +
                            row[CN.Arrears] + comma +                       //IP - 30/09/10 - CR1107 - Writeoff Review Enhancements
                            row[CN.ArrearsExCharges] + comma +              //IP - 30/09/10 - CR1107 - Writeoff Review Enhancements
                            row[CN.Instalment] + comma +                    //IP - 30/09/10 - CR1107 - Writeoff Review Enhancements
                            row[CN.PaymentAmt] + comma +                    //IP - 30/09/10 - CR1107 - Writeoff Review Enhancements
                            row[CN.MonthsInArrears] + comma +               //IP - 30/09/10 - CR1107 - Writeoff Review Enhancements
                            row[CN.DeliveryDate] + comma +                  //IP - 30/09/10 - CR1107 - Writeoff Review Enhancements
                            row[CN.ServiceCharge] + comma +                 //IP - 30/09/10 - CR1107 - Writeoff Review Enhancements
                            row[CN.SegmentName2] + comma +                  //IP - 30/09/10 - CR1107 - Writeoff Review Enhancements
                            row[CN.Rebate] + comma +                        //IP - 30/09/10 - CR1107 - Writeoff Review Enhancements
                            row[CN.BranchNo] + comma +                      //IP - 30/09/10 - CR1107 - Writeoff Review Enhancements
                            row[CN.LastTransDate] +                         //IP - 30/09/10 - CR1107 - Writeoff Review Enhancements
                            Environment.NewLine;

                        blob = System.Text.Encoding.UTF8.GetBytes(line);
                        fs.Write(blob, 0, blob.Length);
                    }
                    fs.Close();

                    /* attempt to launch Excel. May get a COMException if Excel is not 
                     * installed */
                    try
                    {
                        /* we have to use Reflection to call the Open method because 
                         * the methods have different argument lists for the 
                         * different versions of Excel - JJ */
                        object[] args = null;
                        Excel.Application excel = new Excel.Application();

                        if (excel.Version == "10.0")	/* Excel2002 */
                            args = new object[] { path, 0, true, 2, "", "", true, Excel.XlPlatform.xlWindows, comma, true, false, 0, true, false, false };
                        else
                            args = new object[] { path, 0, true, 2, "", "", true, Excel.XlPlatform.xlWindows, comma, true, false, 0, true };

                        /* Retrieve the Workbooks property */
                        object wbs = excel.GetType().InvokeMember("Workbooks", BindingFlags.Public | BindingFlags.GetField | BindingFlags.GetProperty, null, excel, new Object[] { });

                        /* call the Open method */
                        object wb = wbs.GetType().InvokeMember("Open", BindingFlags.Public | BindingFlags.InvokeMethod, null, wbs, args);

                        excel.Visible = true;
                    }
                    catch (COMException)
                    {
                        /*change back slashes to forward slashes so the path doesn't
                         * get split into multiple lines */
                        ShowInfo("M_EXCELNOTFOUND", new object[] { path.Replace("\\", "/") });
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

        private void btnExportDetails_MouseHover(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(btnExportDetails, "Export to Excel");
        }
    }
}

