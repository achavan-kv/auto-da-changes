//using STL.PL.SERVICE;
//using STL.PL.Installation;
using Blue.Cosacs.Shared.Extensions;
using STL.Common.Constants.Tags;
using STL.Common.Static;
using STL.PL.WS2;
using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.Xml;

namespace STL.PL
{
    /// <summary>
    /// Search screen to list stock items that can be selected and added to a sale.
    /// A specific product code must be entered and this product is then listed for
    /// each stock location. The list includes columns to show availability and
    /// stock on order.
    /// </summary>
    public class CodeStockEnquiry : CommonForm
    {
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TreeView tvUnitPrice;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtProductCode;
        private System.Windows.Forms.TextBox txtProdDesc2;
        private System.Windows.Forms.TextBox txtSupplierCode;
        private System.Windows.Forms.TextBox txtProdDesc1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtProductCodeIn;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private DataSet byCode;
        private new string Error = "";
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.DataGrid dgItems;
        private System.Windows.Forms.CheckBox chxLimit;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnEnter;
        //private Crownwood.Magic.Menus.MenuControl menuMain;
        private Crownwood.Magic.Menus.MenuCommand menuFile;
        private Crownwood.Magic.Menus.MenuCommand menuSearch;
        private Crownwood.Magic.Menus.MenuCommand menuClear;
        private Crownwood.Magic.Menus.MenuCommand menuExit;
        private Label label8;
        private Label label7;
        private TextBox txtColour;
        private TextBox txtStyle;
        private IContainer components;

        //IP - 16/05/11 - #3626
        public string ItemNo
        {
            get { return txtProductCodeIn.Text; }
            set { txtProductCodeIn.Text = value; }
        }

        public CodeStockEnquiry(TranslationDummy d)
        {
            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile });
        }

        public CodeStockEnquiry()
        {
            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile });
            txtProductCode.Focus();
        }

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CodeStockEnquiry));
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.menuSearch = new Crownwood.Magic.Menus.MenuCommand();
            this.menuClear = new Crownwood.Magic.Menus.MenuCommand();
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.dgItems = new System.Windows.Forms.DataGrid();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtColour = new System.Windows.Forms.TextBox();
            this.txtStyle = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tvUnitPrice = new System.Windows.Forms.TreeView();
            this.label3 = new System.Windows.Forms.Label();
            this.txtProductCode = new System.Windows.Forms.TextBox();
            this.txtProdDesc2 = new System.Windows.Forms.TextBox();
            this.txtSupplierCode = new System.Windows.Forms.TextBox();
            this.txtProdDesc1 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnEnter = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.chxLimit = new System.Windows.Forms.CheckBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtProductCodeIn = new System.Windows.Forms.TextBox();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgItems)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // menuFile
            // 
            this.menuFile.Description = "MenuItem";
            this.menuFile.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuSearch,
            this.menuClear,
            this.menuExit});
            this.menuFile.Text = "&File";
            // 
            // menuSearch
            // 
            this.menuSearch.Description = "MenuItem";
            this.menuSearch.Text = "&Search";
            this.menuSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // menuClear
            // 
            this.menuClear.Description = "MenuItem";
            this.menuClear.Text = "&Clear";
            this.menuClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // menuExit
            // 
            this.menuExit.Description = "MenuItem";
            this.menuExit.Text = "E&xit";
            this.menuExit.Click += new System.EventHandler(this.btnEnter_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox3.Controls.Add(this.dgItems);
            this.groupBox3.Location = new System.Drawing.Point(8, 208);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(776, 264);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Locations";
            // 
            // dgItems
            // 
            this.dgItems.CaptionText = "Available Stock";
            this.dgItems.DataMember = "";
            this.dgItems.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgItems.Location = new System.Drawing.Point(8, 16);
            this.dgItems.Name = "dgItems";
            this.dgItems.ReadOnly = true;
            this.dgItems.Size = new System.Drawing.Size(752, 240);
            this.dgItems.TabIndex = 0;
            this.dgItems.TabStop = false;
            this.dgItems.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgItems_MouseUp);
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.txtColour);
            this.groupBox2.Controls.Add(this.txtStyle);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.tvUnitPrice);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.txtProductCode);
            this.groupBox2.Controls.Add(this.txtProdDesc2);
            this.groupBox2.Controls.Add(this.txtSupplierCode);
            this.groupBox2.Controls.Add(this.txtProdDesc1);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Location = new System.Drawing.Point(8, 80);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(776, 128);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Details";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(447, 71);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(37, 13);
            this.label8.TabIndex = 19;
            this.label8.Text = "Colour";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(447, 23);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(64, 13);
            this.label7.TabIndex = 18;
            this.label7.Text = "Style/Model";
            // 
            // txtColour
            // 
            this.txtColour.Location = new System.Drawing.Point(447, 88);
            this.txtColour.Name = "txtColour";
            this.txtColour.Size = new System.Drawing.Size(100, 20);
            this.txtColour.TabIndex = 17;
            // 
            // txtStyle
            // 
            this.txtStyle.Location = new System.Drawing.Point(447, 40);
            this.txtStyle.Name = "txtStyle";
            this.txtStyle.Size = new System.Drawing.Size(100, 20);
            this.txtStyle.TabIndex = 16;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(152, 72);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(128, 16);
            this.label6.TabIndex = 3;
            this.label6.Text = "Product Description 2:";
            // 
            // tvUnitPrice
            // 
            this.tvUnitPrice.BackColor = System.Drawing.SystemColors.Control;
            this.tvUnitPrice.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tvUnitPrice.Location = new System.Drawing.Point(581, 30);
            this.tvUnitPrice.Name = "tvUnitPrice";
            this.tvUnitPrice.Size = new System.Drawing.Size(181, 80);
            this.tvUnitPrice.TabIndex = 11;
            this.tvUnitPrice.TabStop = false;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(30, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 16);
            this.label3.TabIndex = 0;
            this.label3.Text = "Item No:";
            // 
            // txtProductCode
            // 
            this.txtProductCode.Location = new System.Drawing.Point(30, 40);
            this.txtProductCode.Name = "txtProductCode";
            this.txtProductCode.Size = new System.Drawing.Size(100, 20);
            this.txtProductCode.TabIndex = 7;
            this.txtProductCode.TabStop = false;
            // 
            // txtProdDesc2
            // 
            this.txtProdDesc2.Location = new System.Drawing.Point(152, 88);
            this.txtProdDesc2.Name = "txtProdDesc2";
            this.txtProdDesc2.Size = new System.Drawing.Size(272, 20);
            this.txtProdDesc2.TabIndex = 10;
            this.txtProdDesc2.TabStop = false;
            // 
            // txtSupplierCode
            // 
            this.txtSupplierCode.Location = new System.Drawing.Point(30, 88);
            this.txtSupplierCode.Name = "txtSupplierCode";
            this.txtSupplierCode.Size = new System.Drawing.Size(100, 20);
            this.txtSupplierCode.TabIndex = 8;
            this.txtSupplierCode.TabStop = false;
            // 
            // txtProdDesc1
            // 
            this.txtProdDesc1.Location = new System.Drawing.Point(152, 40);
            this.txtProdDesc1.Name = "txtProdDesc1";
            this.txtProdDesc1.Size = new System.Drawing.Size(272, 20);
            this.txtProdDesc1.TabIndex = 9;
            this.txtProdDesc1.TabStop = false;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(30, 72);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 16);
            this.label4.TabIndex = 1;
            this.label4.Text = "Supplier Code:";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(152, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(128, 16);
            this.label5.TabIndex = 2;
            this.label5.Text = "Product Description 1:";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.btnEnter);
            this.groupBox1.Controls.Add(this.btnClear);
            this.groupBox1.Controls.Add(this.chxLimit);
            this.groupBox1.Controls.Add(this.btnSearch);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtProductCodeIn);
            this.groupBox1.Location = new System.Drawing.Point(8, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(776, 80);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Product";
            // 
            // btnEnter
            // 
            this.btnEnter.Location = new System.Drawing.Point(712, 48);
            this.btnEnter.Name = "btnEnter";
            this.btnEnter.Size = new System.Drawing.Size(48, 23);
            this.btnEnter.TabIndex = 4;
            this.btnEnter.Text = "Exit";
            this.btnEnter.Click += new System.EventHandler(this.btnEnter_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(712, 16);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(48, 23);
            this.btnClear.TabIndex = 3;
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // chxLimit
            // 
            this.chxLimit.Checked = true;
            this.chxLimit.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chxLimit.Location = new System.Drawing.Point(280, 40);
            this.chxLimit.Name = "chxLimit";
            this.chxLimit.Size = new System.Drawing.Size(104, 24);
            this.chxLimit.TabIndex = 2;
            this.chxLimit.Text = "view top 250";
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(192, 40);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(48, 23);
            this.btnSearch.TabIndex = 1;
            this.btnSearch.Text = "Search";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(27, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Item No:";
            // 
            // txtProductCodeIn
            // 
            this.txtProductCodeIn.Location = new System.Drawing.Point(30, 42);
            this.txtProductCodeIn.Name = "txtProductCodeIn";
            this.txtProductCodeIn.Size = new System.Drawing.Size(100, 20);
            this.txtProductCodeIn.TabIndex = 0;
            this.txtProductCodeIn.Validating += new System.ComponentModel.CancelEventHandler(this.txtProductCodeIn_Validating);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // CodeStockEnquiry
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CodeStockEnquiry";
            this.Text = "Stock Enquiry By Product";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.CodeStockEnquiry_Closing);
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgItems)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private void txtProductCodeIn_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (txtProductCodeIn.Text.Length > 0 && txtProductCodeIn.Text.Length <= 18)
                {
                    txtProductCodeIn.Text = txtProductCodeIn.Text.ToUpper();
                    errorProvider1.SetError(txtProductCodeIn, "");
                }
                else
                {
                    throw new Exception("You must enter a valid product code");
                }
            }
            catch (Exception ex)
            {
                //e.Cancel = true;
                txtProductCodeIn.Focus();
                txtProductCodeIn.Select(0, txtProductCodeIn.Text.Length);
                errorProvider1.SetError(txtProductCodeIn, ex.Message);
            }
        }

        private void btnSearch_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                Function = "BAccountManager::GetStockByCode";
                byCode = AccountManager.GetStockByCode(txtProductCodeIn.Text, out Error);
                if (Error.Length > 0)
                    ShowError(Error);
                else
                {

                    dgItems.DataSource = byCode;
                    dgItems.DataMember = "ByCode";
                    if (dgItems.TableStyles.Count == 0)
                    {
                        DataGridTableStyle tabStyle = new DataGridTableStyle();
                        tabStyle.MappingName = byCode.Tables[0].TableName;
                        dgItems.TableStyles.Add(tabStyle);


                        tabStyle.GridColumnStyles["Location"].Width = 99;
                        tabStyle.GridColumnStyles["Location"].HeaderText = GetResource("T_STOCKLOCN");
                        tabStyle.GridColumnStyles["Location"].Alignment = HorizontalAlignment.Center;
                        tabStyle.GridColumnStyles["Actual Stock"].Width = 99;
                        tabStyle.GridColumnStyles["Actual Stock"].HeaderText = GetResource("T_ACTUALSTOCK");
                        tabStyle.GridColumnStyles["Actual Stock"].Alignment = HorizontalAlignment.Center;
                        tabStyle.GridColumnStyles["Available Stock"].Width = 99;
                        tabStyle.GridColumnStyles["Available Stock"].HeaderText = GetResource("T_AVAILSTOCK");
                        tabStyle.GridColumnStyles["Available Stock"].Alignment = HorizontalAlignment.Center;
                        tabStyle.GridColumnStyles["Damage Stock"].Width = 99;
                        tabStyle.GridColumnStyles["Damage Stock"].HeaderText = GetResource("T_DAMAGESTOCK");
                        tabStyle.GridColumnStyles["Damage Stock"].Alignment = HorizontalAlignment.Center;
                        tabStyle.GridColumnStyles["Stock On Order"].Width = 99;
                        tabStyle.GridColumnStyles["Stock On Order"].HeaderText = GetResource("T_STOCKONORDER");
                        tabStyle.GridColumnStyles["Stock On Order"].Alignment = HorizontalAlignment.Center;
                        tabStyle.GridColumnStyles["Stock Delivery Date"].Width = 99;
                        tabStyle.GridColumnStyles["Stock Delivery Date"].HeaderText = GetResource("T_DELIVERYDATE");
                        tabStyle.GridColumnStyles["Stock Delivery Date"].Alignment = HorizontalAlignment.Center;
                        tabStyle.GridColumnStyles["Stock Delivery Date"].NullText = string.Empty;                       //IP - 06/09/11 - RI - #8104 - UAT40
                        //69647 Add a Deleted Product column to the data grid
                        tabStyle.GridColumnStyles["deleted"].Width = 99;
                        tabStyle.GridColumnStyles["deleted"].HeaderText = GetResource("T_DELETEDITEM");
                        tabStyle.GridColumnStyles["deleted"].Alignment = HorizontalAlignment.Center;
                        tabStyle.GridColumnStyles["Vendor EAN"].NullText = "";
                        tabStyle.GridColumnStyles["Repossessed"].Width = 85;
                        tabStyle.GridColumnStyles["Repossessed"].Alignment = HorizontalAlignment.Center;

                        tabStyle.GridColumnStyles["ItemID"].Width = 0;      //IP - 26/07/11 - RI
                        tabStyle.GridColumnStyles["Colour"].Width = 0;      //IP - 27/07/11 - RI
                        tabStyle.GridColumnStyles["Brand"].Width = 99;      //RM - 06/10/11 - RI
                        tabStyle.GridColumnStyles["Brand"].Alignment = HorizontalAlignment.Center;
                        tabStyle.GridColumnStyles["Model"].Width = 99;      //RM - 06/10/11 - RI
                        tabStyle.GridColumnStyles["Model"].Alignment = HorizontalAlignment.Center;
                        tabStyle.GridColumnStyles["VendorStyle"].Width = 0;      //IP - 27/07/11 - RI
                    }
                    ((MainForm)this.FormRoot).statusBar1.Text = Convert.ToString(byCode.Tables["ByCode"].Rows.Count) + " rows returned";
                }
                StopWait();
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void btnEnter_Click(object sender, System.EventArgs e)
        {
            CloseTab();
        }

        private void btnClear_Click(object sender, System.EventArgs e)
        {
            txtProductCodeIn.Text = "";
            txtProductCode.Text = "";
            txtProdDesc1.Text = "";
            txtProdDesc2.Text = "";
            txtSupplierCode.Text = "";
            txtStyle.Text = "";
            txtColour.Text = "";

            // 5.1 uat107 rdb 26/11/07 check if object referenced
            if (byCode != null)
            {
                byCode.Clear();
            }
        }

        private void dgItems_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Function = "dgItems_MouseUp";

            try
            {
                if (dgItems.CurrentRowIndex < 0)
                    return;

                dgItems.Select(dgItems.CurrentCell.RowNumber);

                Wait();

                string acctType = "H";

                //Get the details for the selected product
                XmlNode currentItem = AccountManager.GetItemDetails(new GetItemDetailsRequest
                {
                    ProductCode = txtProductCodeIn.Text,
                    StockLocationNo = Convert.ToInt16(dgItems[dgItems.CurrentRowIndex, 0]),
                    BranchCode = Config.BranchCode.TryParseInt16(0).Value,
                    AccountType = acctType,
                    CountryCode = Config.CountryCode,
                    PromoBranch = Convert.ToInt16(Config.BranchCode),
                    //ItemID = Convert.ToInt32(dgItems[dgItems.CurrentRowIndex, 10])
                    ItemID = Convert.ToInt32(((DataTable)((DataSet)dgItems.DataSource).Tables["ByCode"]).Rows[dgItems.CurrentRowIndex]["ItemID"]) //IP - 27/07/11 - RI  
                }, out Error);

                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    tvUnitPrice.Nodes.Clear();
                    TreeNode node = new TreeNode("Unit Price");

                    txtProductCode.Text = currentItem.Attributes[Tags.Code].Value;
                    txtProdDesc1.Text = currentItem.Attributes[Tags.Description1].Value;
                    txtProdDesc2.Text = currentItem.Attributes[Tags.Description2].Value;
                    txtSupplierCode.Text = currentItem.Attributes[Tags.SupplierCode].Value;
                    //txtColour.Text = currentItem.Attributes[Tags.ColourName].Value;    //CR1212    
                    txtColour.Text = Convert.ToString(((DataTable)((DataSet)dgItems.DataSource).Tables["ByCode"]).Rows[dgItems.CurrentRowIndex]["Colour"]);     //IP - 27/07/11 - RI
                    //txtStyle.Text = currentItem.Attributes[Tags.Style].Value;          //CR1212 
                    //txtStyle.Text = Convert.ToString(((DataTable)((DataSet)dgItems.DataSource).Tables["ByCode"]).Rows[dgItems.CurrentRowIndex]["VendorLongStyle"]) == string.Empty ? Convert.ToString(((DataTable)((DataSet)dgItems.DataSource).Tables["ByCode"]).Rows[dgItems.CurrentRowIndex]["VendorStyle"]) : Convert.ToString(((DataTable)((DataSet)dgItems.DataSource).Tables["ByCode"]).Rows[dgItems.CurrentRowIndex]["VendorLongStyle"]); //IP - 27/07/11 - RI
                    txtStyle.Text = Convert.ToString(((DataTable)((DataSet)dgItems.DataSource).Tables["ByCode"]).Rows[dgItems.CurrentRowIndex]["Model"]); //IP - 30/08/11 - RI - #4623 - Always display Vendor Long Style
                    //label7.Text = Convert.ToString(((DataTable)((DataSet)dgItems.DataSource).Tables["ByCode"]).Rows[dgItems.CurrentRowIndex]["VendorLongStyle"]) == string.Empty ? "Style" : "Long Style";               //IP - 27/07/11 - RI
                    //label7.Text = "Long Style"; //IP - 30/08/11 - RI - #4623 - Always display Long Style

                    node.Nodes.Add("Cash: " + Convert.ToDouble(currentItem.Attributes[Tags.CashPrice].Value).ToString(DecimalPlaces));
                    node.Nodes.Add("Regular: " + Convert.ToDouble(currentItem.Attributes[Tags.HPPrice].Value).ToString(DecimalPlaces));
                    node.Nodes.Add("Duty Free: " + Convert.ToDouble(currentItem.Attributes[Tags.DutyFreePrice].Value).ToString(DecimalPlaces));

                    tvUnitPrice.Nodes.Add(node);
                    tvUnitPrice.ExpandAll();
                }

                if (!FormParent.IsDisposed && FormParent is NewAccount)
                {
                    var parent = FormParent as NewAccount;
                    parent.ProductCode = Convert.ToString(dgItems[dgItems.CurrentRowIndex, 8]);
                    parent.ItemID = Convert.ToInt32(dgItems[dgItems.CurrentRowIndex, 10]); //TODO need to replace datagrid with datagridview, then can access the columns by name
                    parent.Location = Convert.ToString(dgItems[dgItems.CurrentRowIndex, 0]);
                    acctType = parent.AccountType;
                }
                //else if (!FormParent.IsDisposed && FormParent is SR_ServiceRequest)
                //{
                //    var parent = FormParent as SR_ServiceRequest;

                //    parent.searchLocation = Convert.ToString(dgItems[dgItems.CurrentRowIndex, 0]);
                //    parent.productCode = Convert.ToString(dgItems[dgItems.CurrentRowIndex, 8]);
                //    parent.stockLocation = Convert.ToString(dgItems[dgItems.CurrentRowIndex, 0]);
                //  //  parent.StockLocationChanged();
                //    parent.ValidateSparePart = true;                                            //IP - 04/07/11 - CR1254 - #3994 - Only validate the part once part has been selected from this screen.
                //    parent.ItemID = Convert.ToInt32(dgItems[dgItems.CurrentRowIndex, 10]);      // RI

                //}
                //else if (!FormParent.IsDisposed && FormParent is InstManagement) //IP - 15/02/11 - Sprint 5.10 - #3177
                //{
                //    var itemNo  = Convert.ToString(dgItems[dgItems.CurrentRowIndex, 8]);
                //    var itemId  = Convert.ToInt32(dgItems[dgItems.CurrentRowIndex, 10]);
                //    short? stockLocation = Convert.ToString(dgItems[dgItems.CurrentRowIndex, 0]).TryParseInt16();

                //    (FormParent as InstManagement).UpdateCourtsPart(itemNo, itemId, stockLocation);
                //}
                else if (!FormParent.IsDisposed && FormParent is Product_Associations)      // RI jec 10/06/11
                {
                    var parent = FormParent as Product_Associations;
                    parent.ItemID = Convert.ToInt32(dgItems[dgItems.CurrentRowIndex, 10]);
                    parent.ItemDescr1 = txtProdDesc1.Text;
                    parent.ItemDescr2 = txtProdDesc2.Text;
                }

                StopWait();
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void CodeStockEnquiry_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (FormParent.GetType().Name == "NewAccount")
            {
                string loc = ((NewAccount)this.FormParent).Location;
                ((NewAccount)this.FormParent).ClearItemDetails();
                ((NewAccount)this.FormParent).Location = loc;
            }
        }

        public override bool ConfirmClose()
        {
            if (FormParent.GetType().Name == "NewAccount")
            {
                string loc = ((NewAccount)this.FormParent).Location;
                ((NewAccount)this.FormParent).ClearItemDetails();
                ((NewAccount)this.FormParent).Location = loc;
                ((NewAccount)FormParent).drpLocation_Validating(this, new CancelEventArgs());
                ((NewAccount)FormParent).txtQuantity.Focus();
            }
            return true;
        }
    }
}
