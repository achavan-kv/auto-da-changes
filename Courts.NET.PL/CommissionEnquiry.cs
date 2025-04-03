using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.Xml;
using STL.Common;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using STL.Common.Static;


namespace STL.PL
{
    public class CommissionEnquiry : CommonForm
    {
        private System.Windows.Forms.ComboBox drpEmployee;
        private System.Windows.Forms.DateTimePicker dtpCommDateFrom;
        private System.Windows.Forms.DateTimePicker dtpCommDateTo;
        // private System.Windows.Forms.TextBox txtAccountNo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnExit;

        private bool _staticLoaded = false;
        private StringCollection salesStaff = null;
        private new string Error = "";
        private string employeeStr = "";
        private string accountNo = "";
        private int agreementNo = 0;
        private string _errorTxt = "";
        private DateTime _serverDate;
        private short branchcode = Convert.ToInt16(Config.BranchCode);
        private ErrorProvider errorProvider1;
        private IContainer components;
        private string user = Credential.UserId.ToString();
        private DateTime commRunDate;
        private AccountTextBox txtAccountNo;
        private ComboBox drpBranchNo;
        private Label label8;
        private Label lblCategory;
        private ComboBox drpCategory;
        private Panel panel1;
        public Button btnTransactionDetailsExcel;
        public Button btnCommissionTotalsExcel;
        private Label label6;
        private Label label5;
        private DataGridView dgvCommissionDetails;
        private DataGridView dgvCommissionsView;
        private Panel panel2;
        public Button btnOrderDetailsExcel;
        private Label label7;
        private DataGridView dgvOrderDetails;
        private string employee = "0";                     //#15412
        //private int employee = 0;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.GroupBox gbFilterOptions;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CommissionEnquiry));
            this.drpCategory = new System.Windows.Forms.ComboBox();
            this.lblCategory = new System.Windows.Forms.Label();
            this.drpEmployee = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.dtpCommDateFrom = new System.Windows.Forms.DateTimePicker();
            this.drpBranchNo = new System.Windows.Forms.ComboBox();
            this.dtpCommDateTo = new System.Windows.Forms.DateTimePicker();
            this.txtAccountNo = new STL.PL.AccountTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.dgvCommissionsView = new System.Windows.Forms.DataGridView();
            this.dgvCommissionDetails = new System.Windows.Forms.DataGridView();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btnCommissionTotalsExcel = new System.Windows.Forms.Button();
            this.btnTransactionDetailsExcel = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnOrderDetailsExcel = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.dgvOrderDetails = new System.Windows.Forms.DataGridView();
            gbFilterOptions = new System.Windows.Forms.GroupBox();
            gbFilterOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCommissionsView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCommissionDetails)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrderDetails)).BeginInit();
            this.SuspendLayout();
            // 
            // gbFilterOptions
            // 
            gbFilterOptions.Controls.Add(this.drpCategory);
            gbFilterOptions.Controls.Add(this.lblCategory);
            gbFilterOptions.Controls.Add(this.drpEmployee);
            gbFilterOptions.Controls.Add(this.label8);
            gbFilterOptions.Controls.Add(this.dtpCommDateFrom);
            gbFilterOptions.Controls.Add(this.drpBranchNo);
            gbFilterOptions.Controls.Add(this.dtpCommDateTo);
            gbFilterOptions.Controls.Add(this.txtAccountNo);
            gbFilterOptions.Controls.Add(this.label1);
            gbFilterOptions.Controls.Add(this.label2);
            gbFilterOptions.Controls.Add(this.label3);
            gbFilterOptions.Controls.Add(this.label4);
            gbFilterOptions.Location = new System.Drawing.Point(18, 5);
            gbFilterOptions.Name = "gbFilterOptions";
            gbFilterOptions.Size = new System.Drawing.Size(685, 99);
            gbFilterOptions.TabIndex = 140;
            gbFilterOptions.TabStop = false;
            gbFilterOptions.Text = "Filter Options";
            // 
            // drpCategory
            // 
            this.drpCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpCategory.FormattingEnabled = true;
            this.drpCategory.Items.AddRange(new object[] {
            "All",
            "Electrical",
            "Furniture",
            "Workstation",
            "Other"});
            this.drpCategory.Location = new System.Drawing.Point(84, 72);
            this.drpCategory.Name = "drpCategory";
            this.drpCategory.Size = new System.Drawing.Size(184, 21);
            this.drpCategory.TabIndex = 142;
            this.drpCategory.SelectedIndexChanged += new System.EventHandler(this.drpCategory_SelectedIndexChanged);
            // 
            // lblCategory
            // 
            this.lblCategory.AutoSize = true;
            this.lblCategory.Location = new System.Drawing.Point(84, 55);
            this.lblCategory.Name = "lblCategory";
            this.lblCategory.Size = new System.Drawing.Size(76, 13);
            this.lblCategory.TabIndex = 141;
            this.lblCategory.Text = "Product Group";
            // 
            // drpEmployee
            // 
            this.drpEmployee.Enabled = false;
            this.drpEmployee.FormattingEnabled = true;
            this.drpEmployee.Location = new System.Drawing.Point(84, 31);
            this.drpEmployee.Name = "drpEmployee";
            this.drpEmployee.Size = new System.Drawing.Size(184, 21);
            this.drpEmployee.TabIndex = 1;
            this.drpEmployee.SelectedIndexChanged += new System.EventHandler(this.drpEmployee_IndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 14);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(41, 13);
            this.label8.TabIndex = 139;
            this.label8.Text = "Branch";
            // 
            // dtpCommDateFrom
            // 
            this.dtpCommDateFrom.Location = new System.Drawing.Point(402, 31);
            this.dtpCommDateFrom.Name = "dtpCommDateFrom";
            this.dtpCommDateFrom.Size = new System.Drawing.Size(123, 20);
            this.dtpCommDateFrom.TabIndex = 2;
            this.dtpCommDateFrom.ValueChanged += new System.EventHandler(this.dtpCommDateFrom_Changed);
            // 
            // drpBranchNo
            // 
            this.drpBranchNo.Enabled = false;
            this.drpBranchNo.FormattingEnabled = true;
            this.drpBranchNo.Location = new System.Drawing.Point(8, 31);
            this.drpBranchNo.Name = "drpBranchNo";
            this.drpBranchNo.Size = new System.Drawing.Size(59, 21);
            this.drpBranchNo.TabIndex = 138;
            this.drpBranchNo.SelectedIndexChanged += new System.EventHandler(this.drpBranchNo_SelectedIndexChanged);
            // 
            // dtpCommDateTo
            // 
            this.dtpCommDateTo.Location = new System.Drawing.Point(541, 31);
            this.dtpCommDateTo.Name = "dtpCommDateTo";
            this.dtpCommDateTo.Size = new System.Drawing.Size(124, 20);
            this.dtpCommDateTo.TabIndex = 3;
            this.dtpCommDateTo.ValueChanged += new System.EventHandler(this.dtpCommDateTo_Changed);
            // 
            // txtAccountNo
            // 
            this.txtAccountNo.Location = new System.Drawing.Point(284, 31);
            this.txtAccountNo.MaxLength = 15;
            this.txtAccountNo.Name = "txtAccountNo";
            this.txtAccountNo.PreventPaste = false;
            this.txtAccountNo.Size = new System.Drawing.Size(92, 20);
            this.txtAccountNo.TabIndex = 4;
            this.txtAccountNo.Text = "000-0000-0000-0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(81, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Employee";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(281, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Account Number";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(399, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Date From";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(538, 13);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Date To";
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(711, 28);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 23);
            this.btnLoad.TabIndex = 9;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(711, 75);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 10;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // dgvCommissionsView
            // 
            this.dgvCommissionsView.AccessibleRole = System.Windows.Forms.AccessibleRole.Row;
            this.dgvCommissionsView.AllowUserToAddRows = false;
            this.dgvCommissionsView.AllowUserToDeleteRows = false;
            this.dgvCommissionsView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dgvCommissionsView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCommissionsView.Location = new System.Drawing.Point(15, 21);
            this.dgvCommissionsView.MultiSelect = false;
            this.dgvCommissionsView.Name = "dgvCommissionsView";
            this.dgvCommissionsView.ReadOnly = true;
            this.dgvCommissionsView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCommissionsView.ShowEditingIcon = false;
            this.dgvCommissionsView.ShowRowErrors = false;
            this.dgvCommissionsView.Size = new System.Drawing.Size(260, 132);
            this.dgvCommissionsView.TabIndex = 138;
            this.dgvCommissionsView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dgvCommissionsRowSelected_click);
            // 
            // dgvCommissionDetails
            // 
            this.dgvCommissionDetails.AccessibleRole = System.Windows.Forms.AccessibleRole.Row;
            this.dgvCommissionDetails.AllowUserToAddRows = false;
            this.dgvCommissionDetails.AllowUserToDeleteRows = false;
            this.dgvCommissionDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvCommissionDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCommissionDetails.Location = new System.Drawing.Point(281, 21);
            this.dgvCommissionDetails.MultiSelect = false;
            this.dgvCommissionDetails.Name = "dgvCommissionDetails";
            this.dgvCommissionDetails.ReadOnly = true;
            this.dgvCommissionDetails.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCommissionDetails.Size = new System.Drawing.Size(451, 132);
            this.dgvCommissionDetails.TabIndex = 139;
            this.dgvCommissionDetails.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dgvCommissionsDetailsRowSelected_click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(278, 3);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(156, 13);
            this.label5.TabIndex = 140;
            this.label5.Text = "Commission Transaction Details";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 3);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(94, 13);
            this.label6.TabIndex = 141;
            this.label6.Text = "Commission Totals";
            // 
            // btnCommissionTotalsExcel
            // 
            this.btnCommissionTotalsExcel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCommissionTotalsExcel.Enabled = false;
            this.btnCommissionTotalsExcel.Image = ((System.Drawing.Image)(resources.GetObject("btnCommissionTotalsExcel.Image")));
            this.btnCommissionTotalsExcel.Location = new System.Drawing.Point(243, 159);
            this.btnCommissionTotalsExcel.Name = "btnCommissionTotalsExcel";
            this.btnCommissionTotalsExcel.Size = new System.Drawing.Size(32, 28);
            this.btnCommissionTotalsExcel.TabIndex = 142;
            this.btnCommissionTotalsExcel.Click += new System.EventHandler(this.btnCommissionTotalsExcel_Click);
            // 
            // btnTransactionDetailsExcel
            // 
            this.btnTransactionDetailsExcel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTransactionDetailsExcel.Enabled = false;
            this.btnTransactionDetailsExcel.Image = ((System.Drawing.Image)(resources.GetObject("btnTransactionDetailsExcel.Image")));
            this.btnTransactionDetailsExcel.Location = new System.Drawing.Point(736, 159);
            this.btnTransactionDetailsExcel.Name = "btnTransactionDetailsExcel";
            this.btnTransactionDetailsExcel.Size = new System.Drawing.Size(32, 28);
            this.btnTransactionDetailsExcel.TabIndex = 143;
            this.btnTransactionDetailsExcel.Click += new System.EventHandler(this.btnTransactionDetailsExcel_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.btnTransactionDetailsExcel);
            this.panel1.Controls.Add(this.btnCommissionTotalsExcel);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.dgvCommissionDetails);
            this.panel1.Controls.Add(this.dgvCommissionsView);
            this.panel1.Location = new System.Drawing.Point(9, 109);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(777, 192);
            this.panel1.TabIndex = 141;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel2.Controls.Add(this.btnOrderDetailsExcel);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.dgvOrderDetails);
            this.panel2.Location = new System.Drawing.Point(9, 302);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(777, 181);
            this.panel2.TabIndex = 142;
            // 
            // btnOrderDetailsExcel
            // 
            this.btnOrderDetailsExcel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOrderDetailsExcel.Enabled = false;
            this.btnOrderDetailsExcel.Image = ((System.Drawing.Image)(resources.GetObject("btnOrderDetailsExcel.Image")));
            this.btnOrderDetailsExcel.Location = new System.Drawing.Point(736, 139);
            this.btnOrderDetailsExcel.Name = "btnOrderDetailsExcel";
            this.btnOrderDetailsExcel.Size = new System.Drawing.Size(32, 28);
            this.btnOrderDetailsExcel.TabIndex = 138;
            this.btnOrderDetailsExcel.Click += new System.EventHandler(this.btnOrderDetails_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(11, 4);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(113, 13);
            this.label7.TabIndex = 137;
            this.label7.Text = "Account/Order Details";
            // 
            // dgvOrderDetails
            // 
            this.dgvOrderDetails.AllowUserToAddRows = false;
            this.dgvOrderDetails.AllowUserToDeleteRows = false;
            this.dgvOrderDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvOrderDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvOrderDetails.Location = new System.Drawing.Point(11, 21);
            this.dgvOrderDetails.Name = "dgvOrderDetails";
            this.dgvOrderDetails.ReadOnly = true;
            this.dgvOrderDetails.Size = new System.Drawing.Size(721, 156);
            this.dgvOrderDetails.TabIndex = 136;
            // 
            // CommissionEnquiry
            // 
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(792, 488);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(gbFilterOptions);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnLoad);
            this.Name = "CommissionEnquiry";
            this.Text = "Commission Enquiry";
            gbFilterOptions.ResumeLayout(false);
            gbFilterOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCommissionsView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCommissionDetails)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrderDetails)).EndInit();
            this.ResumeLayout(false);

        }

        public CommissionEnquiry()
        {
            InitializeComponent();

        }

        // Added for invoke from MainForm
        public CommissionEnquiry(Form root, Form parent)
        {
            FormRoot = root;
            FormParent = parent;

            InitializeComponent();

            HashMenus();
            ApplyRoleRestrictions();
            InitialiseStaticData();
            this._serverDate = StaticDataManager.GetServerDate();
            dtpCommDateFrom.Value = _serverDate.AddMonths(-1);
            dtpCommDateTo.Value = _serverDate;
            drpEmployee.Visible = true;
            drpBranchNo.Visible = true;     //CR1035
            dgvCommissionsView.DataSource = null;

        }

        private void HashMenus()
        {
            dynamicMenus = new Hashtable();
            dynamicMenus[this.Name + ":drpEmployee"] = this.drpEmployee;
            dynamicMenus[this.Name + ":drpBranchNo"] = this.drpBranchNo;        //CR1035
            //    dynamicMenus[this.Name + ":btnEnterSpiff"] = this.btnEnterSpiff;
            //    dynamicMenus[this.Name + ":btnDeleteSpiff"] = this.btnDeleteSpiff;
        }

        private void InitialiseStaticData()
        {
            try
            {
                Function = "CommissionEnquiry::InitialiseStaticData";
                /* initialise the collections */
                // Load Branches
                StringCollection branchNos = new StringCollection();
                if (drpBranchNo.Enabled == true)
                {
                    branchNos.Add("All");       //CR1035
                }
                foreach (DataRow row in ((DataTable)StaticData.Tables[TN.BranchNumber]).Rows)
                {
                    branchNos.Add(Convert.ToString(row["branchno"]));
                }

                drpBranchNo.DataSource = branchNos;

                int x = drpBranchNo.FindString(Config.BranchCode);
                if (x != -1)
                    drpBranchNo.SelectedIndex = x;

                int y = drpCategory.FindString("All");      //CR1035
                if (y != -1)
                    drpCategory.SelectedIndex = y;

                LoadSaleStaff();
                // If Head Office and user has All employee rights, enable branch dropdown 
                if (Convert.ToInt32(Config.BranchCode) == (decimal)Country[CountryParameterNames.HOBranchNo]
                        && drpEmployee.Enabled)
                    drpBranchNo.Enabled = true;

                _staticLoaded = true;
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        // load sales staff for Branch
        private void LoadSaleStaff()
        {
            try
            {
                Function = "CommissionEnquiry::LoadSaleStaff";
                drpEmployee.DataSource = null;
                salesStaff = new StringCollection();

                salesStaff.Add("All Sales Staff");

                XmlUtilities xml = new XmlUtilities();
                XmlDocument dropDowns = new XmlDocument();
                dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

                //dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.SalesStaff, new string[] { Config.BranchCode, "S" }));
                //dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.AllStaff, null)); 
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.CommStaff, new string[] { Convert.ToString(drpBranchNo.SelectedValue), " " }));

                DataSet ds = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out Error);
                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    foreach (DataTable dt in ds.Tables)
                    {
                        //if (dt.TableName == TN.SalesStaff)
                        if (dt.TableName == TN.CommStaff)
                        {
                            foreach (DataRow row in dt.Rows)
                            {
                                string str = Convert.ToString(row.ItemArray[0]) + " : " + (string)row.ItemArray[1];
                                salesStaff.Add(str.ToUpper());
                            }

                            drpEmployee.DataSource = salesStaff;
                            drpEmployee.SelectedIndex = 0;

                            if (Credential.IsInRole("S"))   // Sales Staff
                            {
                                int i = drpEmployee.FindString(Credential.UserId.ToString() + " : " + Credential.Name);
                                if (i != -1)
                                {
                                    drpEmployee.SelectedIndex = i;

                                }
                                else
                                {

                                }
                            }
                            else  // this duplicated code can be removed when "if (Credential.IsInRole("S"))" removed 
                            {
                                int i = drpEmployee.FindString(Credential.UserId.ToString() + " : " + Credential.Name);
                                if (i != -1)
                                {
                                    drpEmployee.SelectedIndex = i;

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


        // load basic commission summary
        private void LoadCommissions()
        {
            //  ClearEntryFields();

            dgvCommissionsView.DataSource = null;

            employeeStr = (string)drpEmployee.SelectedItem;
            //int employee = 0;
            var employee = "0";                        //#15412
            DateTime fromDate = dtpCommDateFrom.Value;
            DateTime toDate = dtpCommDateTo.Value.AddDays(1);
            toDate = toDate.AddMinutes(-1);
            string sumDet = "S";    // summary
            string category = drpCategory.SelectedItem != null ? drpCategory.SelectedItem.ToString() : "";     //CR1035

            accountNo = txtAccountNo.Text.Replace("-", "");
            agreementNo = 0;

            if (employeeStr != null)
            {
                if (drpEmployee.SelectedIndex > 0)  // && (drpEmployee.Enabled) && accountNo == "000000000000")
                    employee = employeeStr.Substring(0, employeeStr.IndexOf(":") - 1);                                      //#15412
                    // employee = Convert.ToInt16(employeeStr.Substring(0, employeeStr.IndexOf(":") - 1));
                    //employee = Convert.ToInt32(employeeStr.Substring(0, employeeStr.IndexOf(":") - 1));     // jec 23/01/07
                else if (drpEmployee.Enabled && accountNo != "000000000000")
                {
                    employee = "-99"; // signifies to SQL User has all saleperson permission                        //#15412
                    drpEmployee.SelectedIndex = 0;
                }
                else
                    employee = "-99"; // signifies to SQL User has all saleperson permission                        //#15412

                DataSet ds = PaymentManager.GetBasicSalesCommission(Convert.ToString(drpBranchNo.SelectedValue), employee, fromDate, toDate, accountNo, agreementNo, sumDet, category, out _errorTxt);
                DataView dvCommissions = ds.Tables[TN.SalesCommission].DefaultView;

                dgvCommissionsView.DataSource = dvCommissions;

                dgvCommissionsView.ColumnHeadersVisible = true;
                dgvCommissionsView.AutoGenerateColumns = true;

                //IP - 06/08/09 - UAT(701) - The dataview in certain instances does not include the Category column
                //therefore only do the following if it exists in the dataview.
                if (dgvCommissionsView.Columns.Contains(CN.ProductGroup))               // RI
                {
                    dgvCommissionsView.Columns[CN.ProductGroup].DisplayIndex = 2;   // false;
                    dgvCommissionsView.Columns[CN.ProductGroup].Visible = false;
                }
                dgvCommissionsView.Columns[CN.RunDate].Width = 120;
                dgvCommissionsView.Columns[CN.Commission].Width = 75;
                dgvCommissionsView.Columns[CN.Commission].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvCommissionsView.Columns[CN.Commission].HeaderText = "Total Commission Value";       // #9783


                if (dgvCommissionsView.RowCount > 0)
                    ((MainForm)this.FormRoot).statusBar1.Text = " ";
                else
                    ((MainForm)this.FormRoot).statusBar1.Text = "No commission details found";
                // clear detail datagrids
                dgvCommissionDetails.DataSource = null;
                dgvOrderDetails.DataSource = null;

            }
        }


        private void btnLoad_Click(object sender, EventArgs e)
        {


            //IP & JC - 06/08/09 - Improvements - check on the account number field.
            if (txtAccountNo.Text == "000-0000-0000-0" || txtAccountNo.Text == "" || !IsNumeric(txtAccountNo.Text.Replace("-", "")) || txtAccountNo.Text.Replace("-", "").Length != 12)
            {
                txtAccountNo.Text = "000-0000-0000-0";
            }
            // if (drpEmployee.SelectedIndex > 0)
            LoadCommissions();
            btnCommissionTotalsExcel.Enabled = (dgvCommissionsView.Rows.Count > 0);

            // else
            // {
            //     errorProvider1.SetError(this.drpEmployee, GetResource("M_DATETOLATER"));
            // }
        }
        // row selected in summary grid - show details
        private void dgvCommissionsRowSelected_click(object sender, MouseEventArgs e)
        {
            //int employee = 0;
            var employee = "0";            //#15412

            try
            {
                Function = "dgvCommissionsRowSelected_click";
                Wait();
                dgvOrderDetails.DataSource = null;  // Clear Orders grid
                if (dgvCommissionsView.DataSource != null && dgvCommissionsView.RowCount > 0)
                {
                    commRunDate = new System.DateTime(1999, 1, 1, 00, 0, 00, 000);
                    int index = dgvCommissionsView.CurrentRow.Index;
                    if (index >= 0)
                    {
                        commRunDate = (DateTime)((DataView)dgvCommissionsView.DataSource)[index][CN.RunDate];

                        dgvCommissionDetails.DataSource = null;

                        //employeeStr = (string)drpEmployee.SelectedItem;
                        agreementNo = 0;
                        DateTime fromDate = commRunDate;
                        DateTime toDate = commRunDate;
                        string sumDet = "S";    // Summary
                        string category = (string)drpCategory.SelectedItem;     //CR1035
                        accountNo = txtAccountNo.Text.Replace("-", "");         //CR1035 
                        if (accountNo == "000000000000")
                        {
                            sumDet = "D";    // Details
                            accountNo = "999999999999";
                        }
                        else
                            agreementNo = Convert.ToInt32(((DataView)dgvCommissionsView.DataSource)[index][CN.AgreementNum]);

                        if (employeeStr != null)
                        {
                            // if (!drpEmployee.Enabled)   // change 01/11/06
                            if (drpEmployee.SelectedIndex > 0)
                                employee = employeeStr.Substring(0, employeeStr.IndexOf(":") - 1);                          //#15412
                                //employee = Convert.ToInt32(employeeStr.Substring(0, employeeStr.IndexOf(":") - 1));
                            else if (drpEmployee.Enabled && accountNo != "000000000000" && accountNo != "999999999999")
                                employee = "-99"; // signifies to SQL User has all saleperson permission
                            else
                                employee = "-99";

                            DataSet ds = PaymentManager.GetBasicSalesCommission(Convert.ToString(drpBranchNo.SelectedValue), employee, fromDate, toDate, accountNo, agreementNo, sumDet, category, out _errorTxt);

                            //This added because the second click was causing an error [pc] 2006-11-10 
                            if (accountNo == "999999999999")
                                accountNo = "000000000000";

                            DataView dtCommissions = ds.Tables[TN.SalesCommission].DefaultView;

                            dgvCommissionDetails.DataSource = dtCommissions;

                            dgvCommissionDetails.ColumnHeadersVisible = true;
                            dgvCommissionDetails.AutoGenerateColumns = true;
                            dgvCommissionDetails.Columns[CN.AgreementNum].HeaderText = GetResource("T_INVOICENO");
                            dgvCommissionDetails.Columns[CN.AgreementNum].Width = 70;
                            dgvCommissionDetails.Columns[CN.Commission].Width = 75;
                            dgvCommissionDetails.Columns[CN.Commission].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            dgvCommissionDetails.Columns[CN.Commission].HeaderText = "Total Commission Value";       // #9783

                            //IP - 06/08/09 - UAT(701) - The dataview in certain instances does not include the Category column
                            //therefore only do the following if it exists in the dataview.
                            if (dgvCommissionDetails.Columns.Contains(CN.Category))
                            {
                                dgvCommissionDetails.Columns[CN.Category].Visible = false;        //CR1035
                            }

                        }
                        else
                        // error
                        {

                        }

                        //  Clear Status message;
                        ((MainForm)this.FormRoot).statusBar1.Text = "";
                        btnTransactionDetailsExcel.Enabled = (dgvCommissionDetails.Rows.Count > 0);
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

        private void btnExit_Click(object sender, EventArgs e)
        {
            // exit screen
            CloseTab();
        }
        // row selected in Detail grid - show order details
        private void dgvCommissionsDetailsRowSelected_click(object sender, MouseEventArgs e)
        {
            try
            {
                if (dgvCommissionDetails.DataSource != null)
                {
                    Function = "dgvCommissionsDetailsRowSelected_click";
                    Wait();

                    string orderAccountNo = "";
                    //DateTime commRunDate = new System.DateTime(1999, 1, 1, 00, 0, 00, 000);
                    int index = dgvCommissionDetails.CurrentRow.Index;
                    if (index >= 0)
                    {
                        //commRunDate = (DateTime)((DataView)dgvCommissionDetails.DataSource)[index][CN.RunDate];
                        orderAccountNo = (string)((DataView)dgvCommissionDetails.DataSource)[index][CN.AccountNo];
                        agreementNo = Convert.ToInt32(((DataView)dgvCommissionDetails.DataSource)[index][CN.AgreementNum]);
                        dgvOrderDetails.DataSource = null;

                        //employeeStr = (string)drpEmployee.SelectedItem;

                        DateTime fromDate = commRunDate;
                        DateTime toDate = commRunDate;
                        string sumDet = "D";    // Details
                        string category = (string)drpCategory.SelectedItem;     //CR1035
                        //accountNo = "999999999999";

                        if (employeeStr != null)
                        {
                            if (drpEmployee.SelectedIndex > 0)
                                employee = employeeStr.Substring(0, employeeStr.IndexOf(":") - 1);                      //#15412
                                //employee = Convert.ToInt32(employeeStr.Substring(0, employeeStr.IndexOf(":") - 1));
                            if (drpEmployee.Enabled && accountNo != "000000000000")
                                employee = "-99"; // signifies to SQL User has all saleperson permission                //#15412

                            DataSet ds = PaymentManager.GetBasicSalesCommission(Convert.ToString(drpBranchNo.SelectedValue), employee, fromDate, toDate, orderAccountNo, agreementNo, sumDet, category, out _errorTxt);
                            DataView dtOrders = ds.Tables[TN.SalesCommission].DefaultView;

                            dgvOrderDetails.DataSource = dtOrders;

                            dgvOrderDetails.ColumnHeadersVisible = true;
                            dgvOrderDetails.AutoGenerateColumns = true;
                            dgvOrderDetails.Columns[CN.ProductGroup].Visible = false;        //CR1035
                            dgvOrderDetails.Columns[CN.AccountNo].Visible = false;        //CR1035
                            dgvOrderDetails.Columns[CN.ItemDescr1].HeaderText = GetResource("T_DESCRIPTION");
                            dgvOrderDetails.Columns[CN.ItemDescr2].HeaderText = GetResource("T_DESCRIPTION2");
                            dgvOrderDetails.Columns[CN.Employee].Width = 120;
                            dgvOrderDetails.Columns[CN.ItemNo].Width = 80;
                            dgvOrderDetails.Columns[CN.CourtsCode].Visible = Convert.ToBoolean(Country[CountryParameterNames.RIDispCourtsCode]);     // RI jec
                            dgvOrderDetails.Columns[CN.DelCol].Width = 45;
                            dgvOrderDetails.Columns[CN.Commission_Type].Width = 65;
                            dgvOrderDetails.Columns[CN.DelAmount].Width = 65;
                            dgvOrderDetails.Columns[CN.DelAmount].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            dgvOrderDetails.Columns[CN.Commission].HeaderText = "Total Commission Value";       // #9783
                            dgvOrderDetails.Columns[CN.Uplift_Commission_pcRate].Width = 65;
                            dgvOrderDetails.Columns[CN.Uplift_Commission_pcRate].Visible = false;       // #9783
                            //dgvOrderDetails.Columns[CN.Uplift_Commission_pcRate].Width = 0;         // #9783
                            dgvOrderDetails.Columns[CN.Uplift_Commission_pcRate].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            dgvOrderDetails.Columns[CN.Commission_Percent].Width = 65;
                            dgvOrderDetails.Columns[CN.Commission_Percent].Visible = false;            // #9783
                            //dgvOrderDetails.Columns[CN.Commission_Percent].Width = 0;               // #9783
                            dgvOrderDetails.Columns[CN.Commission_Percent].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            dgvOrderDetails.Columns[CN.Commission].Width = 75; //IP - 24/09/09 - UAT(441) - Reinstated this line and commented out below CN.Commission_Amount
                            dgvOrderDetails.Columns[CN.Commission].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            //dgvOrderDetails.Columns[CN.Commission_Amount].Width = 75;    //jec 03/07/08 UAT442
                            //dgvOrderDetails.Columns[CN.Commission_Amount].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            dgvOrderDetails.Columns[CN.TotalCommission_Percent].Width = 75;    //jec 17/07/09 UAT442
                            dgvOrderDetails.Columns[CN.TotalCommission_Percent].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            if (Convert.ToBoolean(Country[CountryParameterNames.RIDispCatAsDept]))          // RI
                            {
                                dgvOrderDetails.Columns[CN.Category].HeaderText = GetResource("T_DEPARTMENT");
                            }
                            dgvOrderDetails.Columns[CN.BuffNo].Visible = false;        // #10691 
                        }
                        else
                        // error
                        {

                        }

                        //  Clear Status message;
                        ((MainForm)this.FormRoot).statusBar1.Text = "";
                        btnOrderDetailsExcel.Enabled = (dgvOrderDetails.Rows.Count > 0);
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

        private void drpEmployee_IndexChanged(object sender, EventArgs e)
        {
            // Clear grids on change of employee
            dgvCommissionsView.DataSource = null;
            dgvCommissionDetails.DataSource = null;
            dgvOrderDetails.DataSource = null;

            this.btnCommissionTotalsExcel.Enabled = false;
            this.btnTransactionDetailsExcel.Enabled = false;
            this.btnOrderDetailsExcel.Enabled = false;
            // Load commissions
            btnLoad_Click(null, null);

        }

        private void dtpCommDateFrom_Changed(object sender, EventArgs e)
        {
            // if fromdate > todate - set todate one month after
            if (dtpCommDateFrom.Value >= dtpCommDateTo.Value)
                dtpCommDateTo.Value = dtpCommDateFrom.Value.AddMonths(1);
        }

        private void dtpCommDateTo_Changed(object sender, EventArgs e)
        {
            // if to date < fromdate - set fromdate one month before
            if (dtpCommDateTo.Value <= dtpCommDateFrom.Value)
                dtpCommDateFrom.Value = dtpCommDateTo.Value.AddMonths(-1);
        }



        private void btnCommissionTotalsExcel_Click(object sender, EventArgs e)
        {
            //Make Category Column Visible for excel        CR1035
            foreach (DataGridViewColumn dc in dgvCommissionsView.Columns)
            {
                if (dc.Visible == false && dc.Name == CN.ProductGroup)          // RI
                {
                    dc.Visible = true;
                }
            }
            SaveExcel(dgvCommissionsView);
            //Make Category Column InVisible for screen     CR1035
            foreach (DataGridViewColumn dc in dgvCommissionsView.Columns)
            {
                if (dc.Visible == true && dc.Name == CN.ProductGroup)           // RI
                {
                    dc.Visible = false;
                }
            }
        }

        private void btnTransactionDetailsExcel_Click(object sender, EventArgs e)
        {
            //Make Category Column Visible for excel        CR1035
            foreach (DataGridViewColumn dc in dgvCommissionDetails.Columns)
            {
                if (dc.Visible == false && dc.Name == CN.ProductGroup)          // RI
                {
                    dc.Visible = true;
                }
            }
            SaveExcel(dgvCommissionDetails);
            //Make Category Column InVisible for screen     CR1035
            foreach (DataGridViewColumn dc in dgvCommissionDetails.Columns)
            {
                if (dc.Visible == true && dc.Name == CN.ProductGroup)           // RI
                {
                    dc.Visible = false;
                }
            }

        }

        private void btnOrderDetails_Click(object sender, EventArgs e)
        {
            //Make Category & AccountNo Column Visible for excel        CR1035
            foreach (DataGridViewColumn dc in dgvOrderDetails.Columns)
            {
                if (dc.Visible == false && (dc.Name == CN.ProductGroup || dc.Name == CN.AccountNo))     // RI
                {
                    dc.Visible = true;
                }
            }
            SaveExcel(dgvOrderDetails);
            //Make Category & AccountNo Column InVisible for screen     CR1035
            foreach (DataGridViewColumn dc in dgvOrderDetails.Columns)
            {
                if (dc.Visible == true && (dc.Name == CN.ProductGroup || dc.Name == CN.AccountNo))      // RI
                {
                    dc.Visible = false;
                }
            }
        }

        private void SaveExcel(DataGridView dg)
        {
            string filePath = STL.PL.Utils.ReportUtils.CreateCSVFile(dg, "Save Report to Excel");

            if (filePath.Length.Equals(0))
                MessageBox.Show("Save Failed");

            try
            {
                STL.PL.Utils.ReportUtils.OpenExcelCSV(filePath);
            }
            catch { }
        }

        //private void dgvCommissionDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        //{

        //}

        private void drpBranchNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            // avoid executing routine during initial load
            if (_staticLoaded)
            {
                LoadSaleStaff();
            }
        }

        private void drpCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Clear grids on change of category
            dgvCommissionsView.DataSource = null;
            dgvCommissionDetails.DataSource = null;
            dgvOrderDetails.DataSource = null;

            this.btnCommissionTotalsExcel.Enabled = false;
            this.btnTransactionDetailsExcel.Enabled = false;
            this.btnOrderDetailsExcel.Enabled = false;
            // Load commissions
            btnLoad_Click(null, null);
        }
    }
}
