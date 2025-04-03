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
using Blue.Cosacs.Shared;

namespace STL.PL
{
    /// <summary>
    /// Summary description for ChangeItemLocation.
    /// </summary>
    public class ChangeItemLocation : CommonForm
    {
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private string error = "";
        private string acctNo = "";
        bool isDotNetWarehouse = false;
        string awaitDA = "N";
        private System.Windows.Forms.ComboBox drpDeliveryArea;
        private System.Windows.Forms.Label lDeliveryArea;
        private System.Windows.Forms.Label lTimeRequired;
        private System.Windows.Forms.DateTimePicker dtDeliveryRequired;
        private System.Windows.Forms.Label lDeliveryRequired;
        private System.Windows.Forms.ComboBox drpBranchForDel;
        private System.Windows.Forms.Label lBranchForDel;
        private System.Windows.Forms.ComboBox drpDeliveryAddress;
        private System.Windows.Forms.ComboBox drpLocation;
        private System.Windows.Forms.TextBox txtColourTrim;
        private System.Windows.Forms.Label lColourTrim;
        private System.Windows.Forms.Label lLocation;
        private System.Windows.Forms.CheckBox cbImmediate;
        private System.Windows.Forms.Label lDeliveryAddress;
        private System.Windows.Forms.Label lImmediate;
        private System.Windows.Forms.DataGrid dgOrderDetails;
        public STL.PL.AccountTextBox txtAccountNo;          // #10327 jec 08/06/12
        private System.Windows.Forms.Label lAccountNo;
        private DataTable _areaTable = null;
        private new string Error = "";
        private System.Windows.Forms.GroupBox gbAmend;
        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox drpDamaged;
        public System.Windows.Forms.DataGrid dgApplicationStatus;
        private System.Windows.Forms.Label lreviseNonScheduled;
        private System.Windows.Forms.Label lreviseScheduled;
        private CheckBox cbAssembly;
        private IContainer components;
        private Label lblBeforeDA;
        private ComboBox cbTime;
        private CheckBox cbExpress;
        private Label lblExpress;
        private Label lAssembly;
        private ImageList imageList1;
        private ToolTip toolTip1; //IP - 29/04/09 - CR929 & 974 - Deliveries
        private DataTable locations;

        public ChangeItemLocation(TranslationDummy d)
        {
            InitializeComponent();
        }

        public ChangeItemLocation(Form root, Form parent)
        {
            InitializeComponent();

            FormRoot = root;
            FormParent = parent;

            Wait();

            dynamicMenus = new Hashtable();

            HashMenus();
            ApplyRoleRestrictions();

            InitialiseStaticData();
            dtDeliveryRequired.Value = DateTime.Today;

            toolTip1.SetToolTip(lblExpress, GetResource("T_EXPRESS"));                            //jec #10229
            toolTip1.SetToolTip(lAssembly, GetResource("T_ASSEMBLY"));                            //jec #10229
        }

        private void InitialiseStaticData()
        {
            try
            {
                StringCollection branchNos = new StringCollection();
                StringCollection branchNos2 = new StringCollection();

                Function = "BStaticDataManager::GetDropDownData";

                XmlUtilities xml = new XmlUtilities();
                XmlDocument dropDowns = new XmlDocument();
                dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

                if (StaticData.Tables[TN.BranchNumber] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.BranchNumber, null));

                if (dropDowns.DocumentElement.ChildNodes.Count > 0)
                {
                    DataSet ds = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out error);
                    if (error.Length > 0)
                        ShowError(error);
                }

                foreach (DataRow row in ((DataTable)StaticData.Tables[TN.BranchNumber]).Rows)
                {
                    branchNos.Add(Convert.ToString(row["branchno"]));
                    branchNos2.Add(Convert.ToString(row["branchno"]));
                }

                drpBranchForDel.DataSource = branchNos;
                drpLocation.DataSource = branchNos2;

                DataSet areaSet = SetDataManager.GetSetsForTNameBranchAll(TN.TNameDeliveryArea, out Error);
                this._areaTable = areaSet.Tables[TN.SetsData];
                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    // Init the Delivery Area list to the logged in branch
                    this.SetDeliveryAreaList(Config.BranchCode);
                }

                isDotNetWarehouse = AccountManager.IsDotNetWarehouse(Convert.ToInt16(Config.BranchCode), out error);
                if (error.Length > 0)
                    ShowError(error);
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChangeItemLocation));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.gbAmend = new System.Windows.Forms.GroupBox();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.lAssembly = new System.Windows.Forms.Label();
            this.lblExpress = new System.Windows.Forms.Label();
            this.cbExpress = new System.Windows.Forms.CheckBox();
            this.cbTime = new System.Windows.Forms.ComboBox();
            this.cbAssembly = new System.Windows.Forms.CheckBox();
            this.lreviseNonScheduled = new System.Windows.Forms.Label();
            this.lreviseScheduled = new System.Windows.Forms.Label();
            this.drpDamaged = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.drpDeliveryArea = new System.Windows.Forms.ComboBox();
            this.lDeliveryArea = new System.Windows.Forms.Label();
            this.lTimeRequired = new System.Windows.Forms.Label();
            this.dtDeliveryRequired = new System.Windows.Forms.DateTimePicker();
            this.lDeliveryRequired = new System.Windows.Forms.Label();
            this.drpBranchForDel = new System.Windows.Forms.ComboBox();
            this.lBranchForDel = new System.Windows.Forms.Label();
            this.drpDeliveryAddress = new System.Windows.Forms.ComboBox();
            this.drpLocation = new System.Windows.Forms.ComboBox();
            this.txtColourTrim = new System.Windows.Forms.TextBox();
            this.lColourTrim = new System.Windows.Forms.Label();
            this.lLocation = new System.Windows.Forms.Label();
            this.cbImmediate = new System.Windows.Forms.CheckBox();
            this.lDeliveryAddress = new System.Windows.Forms.Label();
            this.lImmediate = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblBeforeDA = new System.Windows.Forms.Label();
            this.dgApplicationStatus = new System.Windows.Forms.DataGrid();
            this.lAccountNo = new System.Windows.Forms.Label();
            this.dgOrderDetails = new System.Windows.Forms.DataGrid();
            this.btnClear = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.txtAccountNo = new STL.PL.AccountTextBox();
            this.gbAmend.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgApplicationStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgOrderDetails)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
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
            // gbAmend
            // 
            this.gbAmend.BackColor = System.Drawing.SystemColors.Control;
            this.gbAmend.Controls.Add(this.btnConfirm);
            this.gbAmend.Controls.Add(this.lAssembly);
            this.gbAmend.Controls.Add(this.lblExpress);
            this.gbAmend.Controls.Add(this.cbExpress);
            this.gbAmend.Controls.Add(this.cbTime);
            this.gbAmend.Controls.Add(this.cbAssembly);
            this.gbAmend.Controls.Add(this.lreviseNonScheduled);
            this.gbAmend.Controls.Add(this.lreviseScheduled);
            this.gbAmend.Controls.Add(this.drpDamaged);
            this.gbAmend.Controls.Add(this.label1);
            this.gbAmend.Controls.Add(this.drpDeliveryArea);
            this.gbAmend.Controls.Add(this.lDeliveryArea);
            this.gbAmend.Controls.Add(this.lTimeRequired);
            this.gbAmend.Controls.Add(this.dtDeliveryRequired);
            this.gbAmend.Controls.Add(this.lDeliveryRequired);
            this.gbAmend.Controls.Add(this.drpBranchForDel);
            this.gbAmend.Controls.Add(this.lBranchForDel);
            this.gbAmend.Controls.Add(this.drpDeliveryAddress);
            this.gbAmend.Controls.Add(this.drpLocation);
            this.gbAmend.Controls.Add(this.txtColourTrim);
            this.gbAmend.Controls.Add(this.lColourTrim);
            this.gbAmend.Controls.Add(this.lLocation);
            this.gbAmend.Controls.Add(this.cbImmediate);
            this.gbAmend.Controls.Add(this.lDeliveryAddress);
            this.gbAmend.Controls.Add(this.lImmediate);
            this.gbAmend.Enabled = false;
            this.gbAmend.Location = new System.Drawing.Point(23, 336);
            this.gbAmend.Name = "gbAmend";
            this.gbAmend.Size = new System.Drawing.Size(752, 128);
            this.gbAmend.TabIndex = 3;
            this.gbAmend.TabStop = false;
            this.gbAmend.Text = "Amend Details";
            // 
            // btnConfirm
            // 
            this.btnConfirm.Location = new System.Drawing.Point(560, 95);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(96, 23);
            this.btnConfirm.TabIndex = 54;
            this.btnConfirm.Text = "Confirm Change";
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // lAssembly
            // 
            this.lAssembly.ImageIndex = 4;
            this.lAssembly.ImageList = this.imageList1;
            this.lAssembly.Location = new System.Drawing.Point(129, 68);
            this.lAssembly.Name = "lAssembly";
            this.lAssembly.Size = new System.Drawing.Size(24, 16);
            this.lAssembly.TabIndex = 94;
            this.lAssembly.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblExpress
            // 
            this.lblExpress.Image = global::STL.PL.Properties.Resources.ExpDelivery;
            this.lblExpress.Location = new System.Drawing.Point(234, 69);
            this.lblExpress.Name = "lblExpress";
            this.lblExpress.Size = new System.Drawing.Size(24, 16);
            this.lblExpress.TabIndex = 93;
            this.lblExpress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cbExpress
            // 
            this.cbExpress.Location = new System.Drawing.Point(239, 91);
            this.cbExpress.Name = "cbExpress";
            this.cbExpress.Size = new System.Drawing.Size(24, 18);
            this.cbExpress.TabIndex = 66;
            // 
            // cbTime
            // 
            this.cbTime.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTime.FormattingEnabled = true;
            this.cbTime.Items.AddRange(new object[] {
            "AM",
            "PM"});
            this.cbTime.Location = new System.Drawing.Point(459, 35);
            this.cbTime.Name = "cbTime";
            this.cbTime.Size = new System.Drawing.Size(53, 21);
            this.cbTime.TabIndex = 64;
            this.cbTime.SelectedIndexChanged += new System.EventHandler(this.cbTime_SelectedIndexChanged);
            // 
            // cbAssembly
            // 
            this.cbAssembly.Location = new System.Drawing.Point(136, 93);
            this.cbAssembly.Name = "cbAssembly";
            this.cbAssembly.Size = new System.Drawing.Size(24, 16);
            this.cbAssembly.TabIndex = 63;
            // 
            // lreviseNonScheduled
            // 
            this.lreviseNonScheduled.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.lreviseNonScheduled.Enabled = false;
            this.lreviseNonScheduled.Location = new System.Drawing.Point(629, 104);
            this.lreviseNonScheduled.Name = "lreviseNonScheduled";
            this.lreviseNonScheduled.Size = new System.Drawing.Size(8, 8);
            this.lreviseNonScheduled.TabIndex = 61;
            this.lreviseNonScheduled.Visible = false;
            // 
            // lreviseScheduled
            // 
            this.lreviseScheduled.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.lreviseScheduled.Enabled = false;
            this.lreviseScheduled.Location = new System.Drawing.Point(611, 104);
            this.lreviseScheduled.Name = "lreviseScheduled";
            this.lreviseScheduled.Size = new System.Drawing.Size(8, 8);
            this.lreviseScheduled.TabIndex = 60;
            this.lreviseScheduled.Visible = false;
            // 
            // drpDamaged
            // 
            this.drpDamaged.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpDamaged.DropDownWidth = 48;
            this.drpDamaged.Items.AddRange(new object[] {
            "N",
            "Y"});
            this.drpDamaged.Location = new System.Drawing.Point(40, 91);
            this.drpDamaged.Name = "drpDamaged";
            this.drpDamaged.Size = new System.Drawing.Size(48, 21);
            this.drpDamaged.TabIndex = 55;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(38, 70);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 16);
            this.label1.TabIndex = 56;
            this.label1.Text = "Damaged";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // drpDeliveryArea
            // 
            this.drpDeliveryArea.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpDeliveryArea.DropDownWidth = 48;
            this.drpDeliveryArea.Location = new System.Drawing.Point(632, 37);
            this.drpDeliveryArea.Name = "drpDeliveryArea";
            this.drpDeliveryArea.Size = new System.Drawing.Size(88, 21);
            this.drpDeliveryArea.TabIndex = 45;
            this.drpDeliveryArea.SelectedIndexChanged += new System.EventHandler(this.drpDeliveryArea_SelectedIndexChanged);
            // 
            // lDeliveryArea
            // 
            this.lDeliveryArea.Location = new System.Drawing.Point(629, 16);
            this.lDeliveryArea.Name = "lDeliveryArea";
            this.lDeliveryArea.Size = new System.Drawing.Size(72, 16);
            this.lDeliveryArea.TabIndex = 53;
            this.lDeliveryArea.Text = "Delivery Area";
            this.lDeliveryArea.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lTimeRequired
            // 
            this.lTimeRequired.Location = new System.Drawing.Point(456, 16);
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
            this.dtDeliveryRequired.Location = new System.Drawing.Point(312, 36);
            this.dtDeliveryRequired.Name = "dtDeliveryRequired";
            this.dtDeliveryRequired.Size = new System.Drawing.Size(112, 20);
            this.dtDeliveryRequired.TabIndex = 46;
            this.dtDeliveryRequired.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            // 
            // lDeliveryRequired
            // 
            this.lDeliveryRequired.Location = new System.Drawing.Point(312, 16);
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
            this.drpBranchForDel.Location = new System.Drawing.Point(120, 35);
            this.drpBranchForDel.Name = "drpBranchForDel";
            this.drpBranchForDel.Size = new System.Drawing.Size(56, 21);
            this.drpBranchForDel.TabIndex = 44;
            // 
            // lBranchForDel
            // 
            this.lBranchForDel.Location = new System.Drawing.Point(116, 16);
            this.lBranchForDel.Name = "lBranchForDel";
            this.lBranchForDel.Size = new System.Drawing.Size(98, 16);
            this.lBranchForDel.TabIndex = 45;
            this.lBranchForDel.Text = "Delivery Location";
            this.lBranchForDel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // drpDeliveryAddress
            // 
            this.drpDeliveryAddress.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpDeliveryAddress.DropDownWidth = 48;
            this.drpDeliveryAddress.Items.AddRange(new object[] {
            "H",
            "W",
            "D",
            "D1",
            "D2",
            "D3"});
            this.drpDeliveryAddress.Location = new System.Drawing.Point(225, 35);
            this.drpDeliveryAddress.Name = "drpDeliveryAddress";
            this.drpDeliveryAddress.Size = new System.Drawing.Size(48, 21);
            this.drpDeliveryAddress.TabIndex = 11;
            // 
            // drpLocation
            // 
            this.drpLocation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpLocation.DropDownWidth = 48;
            this.drpLocation.Location = new System.Drawing.Point(40, 35);
            this.drpLocation.Name = "drpLocation";
            this.drpLocation.Size = new System.Drawing.Size(48, 21);
            this.drpLocation.TabIndex = 1;
            // 
            // txtColourTrim
            // 
            this.txtColourTrim.Location = new System.Drawing.Point(313, 86);
            this.txtColourTrim.Multiline = true;
            this.txtColourTrim.Name = "txtColourTrim";
            this.txtColourTrim.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtColourTrim.Size = new System.Drawing.Size(200, 32);
            this.txtColourTrim.TabIndex = 12;
            // 
            // lColourTrim
            // 
            this.lColourTrim.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lColourTrim.Location = new System.Drawing.Point(312, 70);
            this.lColourTrim.Name = "lColourTrim";
            this.lColourTrim.Size = new System.Drawing.Size(132, 16);
            this.lColourTrim.TabIndex = 20;
            this.lColourTrim.Text = "Delivery Instructions";
            this.lColourTrim.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lLocation
            // 
            this.lLocation.Location = new System.Drawing.Point(36, 16);
            this.lLocation.Name = "lLocation";
            this.lLocation.Size = new System.Drawing.Size(80, 16);
            this.lLocation.TabIndex = 4;
            this.lLocation.Text = "Stock Location";
            this.lLocation.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cbImmediate
            // 
            this.cbImmediate.Location = new System.Drawing.Point(560, 37);
            this.cbImmediate.Name = "cbImmediate";
            this.cbImmediate.Size = new System.Drawing.Size(24, 16);
            this.cbImmediate.TabIndex = 43;
            this.cbImmediate.CheckedChanged += new System.EventHandler(this.cbImmediate_CheckedChanged);
            // 
            // lDeliveryAddress
            // 
            this.lDeliveryAddress.Location = new System.Drawing.Point(222, 16);
            this.lDeliveryAddress.Name = "lDeliveryAddress";
            this.lDeliveryAddress.Size = new System.Drawing.Size(72, 16);
            this.lDeliveryAddress.TabIndex = 30;
            this.lDeliveryAddress.Text = "Del Address";
            this.lDeliveryAddress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lImmediate
            // 
            this.lImmediate.Location = new System.Drawing.Point(536, 16);
            this.lImmediate.Name = "lImmediate";
            this.lImmediate.Size = new System.Drawing.Size(72, 16);
            this.lImmediate.TabIndex = 42;
            this.lImmediate.Text = "Immediate";
            this.lImmediate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.lblBeforeDA);
            this.groupBox1.Controls.Add(this.dgApplicationStatus);
            this.groupBox1.Controls.Add(this.txtAccountNo);
            this.groupBox1.Controls.Add(this.lAccountNo);
            this.groupBox1.Controls.Add(this.dgOrderDetails);
            this.groupBox1.Controls.Add(this.btnClear);
            this.groupBox1.Location = new System.Drawing.Point(23, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(752, 320);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Item Details";
            // 
            // lblBeforeDA
            // 
            this.lblBeforeDA.AutoSize = true;
            this.lblBeforeDA.Location = new System.Drawing.Point(702, 27);
            this.lblBeforeDA.Name = "lblBeforeDA";
            this.lblBeforeDA.Size = new System.Drawing.Size(0, 13);
            this.lblBeforeDA.TabIndex = 57;
            this.lblBeforeDA.Visible = false;
            // 
            // dgApplicationStatus
            // 
            this.dgApplicationStatus.CaptionText = "Application Status";
            this.dgApplicationStatus.DataMember = "";
            this.dgApplicationStatus.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgApplicationStatus.Location = new System.Drawing.Point(272, 16);
            this.dgApplicationStatus.Name = "dgApplicationStatus";
            this.dgApplicationStatus.ReadOnly = true;
            this.dgApplicationStatus.Size = new System.Drawing.Size(420, 120);
            this.dgApplicationStatus.TabIndex = 56;
            this.dgApplicationStatus.TabStop = false;
            // 
            // lAccountNo
            // 
            this.lAccountNo.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lAccountNo.Location = new System.Drawing.Point(40, 48);
            this.lAccountNo.Name = "lAccountNo";
            this.lAccountNo.Size = new System.Drawing.Size(64, 16);
            this.lAccountNo.TabIndex = 13;
            this.lAccountNo.Text = "Account No";
            this.lAccountNo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // dgOrderDetails
            // 
            this.dgOrderDetails.CaptionText = "Order Details";
            this.dgOrderDetails.DataMember = "";
            this.dgOrderDetails.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgOrderDetails.Location = new System.Drawing.Point(16, 144);
            this.dgOrderDetails.Name = "dgOrderDetails";
            this.dgOrderDetails.ReadOnly = true;
            this.dgOrderDetails.Size = new System.Drawing.Size(721, 160);
            this.dgOrderDetails.TabIndex = 12;
            this.dgOrderDetails.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgOrderDetails_MouseUp);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(80, 88);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(96, 23);
            this.btnClear.TabIndex = 55;
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // txtAccountNo
            // 
            this.txtAccountNo.BackColor = System.Drawing.SystemColors.Window;
            this.txtAccountNo.Location = new System.Drawing.Point(112, 48);
            this.txtAccountNo.MaxLength = 20;
            this.txtAccountNo.Name = "txtAccountNo";
            this.txtAccountNo.PreventPaste = false;
            this.txtAccountNo.Size = new System.Drawing.Size(94, 20);
            this.txtAccountNo.TabIndex = 14;
            this.txtAccountNo.Text = "000-0000-0000-0";
            this.txtAccountNo.Leave += new System.EventHandler(this.txtAccountNo_Leave);
            // 
            // ChangeItemLocation
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(792, 470);
            this.Controls.Add(this.gbAmend);
            this.Controls.Add(this.groupBox1);
            this.Name = "ChangeItemLocation";
            this.Text = "Change Order Details";
            this.gbAmend.ResumeLayout(false);
            this.gbAmend.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgApplicationStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgOrderDetails)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion


        private bool CheckSchedules(int index)
        {
            bool status = true;
            double delivered = 0;
            double scheduled = 0;
            bool repo = false;                          //IP - 26/06/12 - #10516
            int agrmtNo = 0;
            int itemId = 0;
            int parentItemID = 0;                       //IP - 23/06/11 - CR1212 - RI - #4067
            short locn = 0;
            bool delNotePrinted = false;
            bool onPickList = false;
            bool onLoad = false;

            try
            {
                Function = "ChangeItemLocation::CheckSchedules()";

                agrmtNo = Convert.ToInt32(((DataView)dgOrderDetails.DataSource)[index][CN.AgrmtNo]);
                locn = Convert.ToInt16(((DataView)dgOrderDetails.DataSource)[index][CN.StockLocn]);
                itemId = Convert.ToInt32(((DataView)dgOrderDetails.DataSource)[index][CN.ItemId]);
                parentItemID = Convert.ToInt32(((DataView)dgOrderDetails.DataSource)[index][CN.ParentItemId]);         //IP - 23/06/11 - CR1212 - RI - #4067      

                AccountManager.GetItemsDeliveredAndScheduled(txtAccountNo.UnformattedText, agrmtNo, itemId, locn,
                                                        "", parentItemID, out delivered, out scheduled, out repo, out error);  //IP - 26/06/12 - #10516        //IP - 23/06/11 - CR1212 - RI - #4067  
                if (error.Length > 0)
                    ShowError(error);
                else
                {
                    //sheduled deliveries can be overridden with the right permission
                    if (scheduled > 0)
                    {
                        AccountManager.GetScheduledDelNote(txtAccountNo.UnformattedText,
                            agrmtNo, itemId, locn, out onPickList,
                            out delNotePrinted, out onLoad, out error);

                        if (error.Length > 0)
                        {
                            ShowError(error);
                            status = false;
                        }
                        else
                        {
                            //delivery note has been printed so inform user
                            //that changes cannot be made to this item
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
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }

            return status;
        }

        //private void CancelDelNote(DataSet ds)
        //{
        //    try
        //    {
        //        Function = "BAccountManager::CancelDelNote()";

        //        foreach (DataRow row in ds.Tables["TN.Items"].Rows)
        //        {
        //            DataSet schedules = AccountManager.Schedule_GetByBuffNo(
        //                                            Convert.ToInt16(row["OldLocation"]),
        //                                            Convert.ToInt32(row[CN.BuffNo]), out error);

        //            schedules.Tables[TN.Schedules].DefaultView.RowFilter = "Quantity >= 1";
        //            AccountManager.CancelDeliveryNote(acctNo, schedules, isDotNetWarehouse, out error);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Catch(ex, Function);
        //    }
        //}

        private void LoadOrderDetails()
        {
            try
            {

                DataSet ds = AccountManager.GetItemsForLocationChange(txtAccountNo.UnformattedText, Credential.HasPermission(CosacsPermissionEnum.ChangeOrderBeforeDA), out error);
                if (error.Length > 0)
                    ShowError(error);
                else
                {
                    if (ds != null)
                    {
                        ds.Tables["Table"].DefaultView.RowFilter = "itemtype = 'S'";

                        dgOrderDetails.DataSource = ds.Tables["Table"].DefaultView;

                        locations = ds.Tables["Table1"];

                        if (dgOrderDetails.TableStyles.Count == 0)
                        {
                            DataGridTableStyle tabStyle = new DataGridTableStyle();
                            tabStyle.MappingName = ds.Tables["Table"].TableName;
                            dgOrderDetails.TableStyles.Add(tabStyle);

                            tabStyle.GridColumnStyles[CN.AcctNo].HeaderText = GetResource("T_ACCOUNTNO");     // #10401

                            tabStyle.GridColumnStyles[CN.ItemNo].Width = 70;
                            tabStyle.GridColumnStyles[CN.ItemNo].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.ItemNo].HeaderText = GetResource("T_ITEMNO");

                            tabStyle.GridColumnStyles[CN.StockLocn].Width = 80;
                            tabStyle.GridColumnStyles[CN.StockLocn].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.StockLocn].HeaderText = GetResource("T_STOCKLOCN");
                            tabStyle.GridColumnStyles[CN.StockLocn].Alignment = HorizontalAlignment.Center;

                            tabStyle.GridColumnStyles[CN.DelNoteBranch].Width = 90;
                            tabStyle.GridColumnStyles[CN.DelNoteBranch].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.DelNoteBranch].HeaderText = GetResource("T_DELIVERYLOCATION");     // #10401
                            tabStyle.GridColumnStyles[CN.DelNoteBranch].Alignment = HorizontalAlignment.Center;

                            tabStyle.GridColumnStyles[CN.DeliveryAddress].Width = 100;
                            tabStyle.GridColumnStyles[CN.DeliveryAddress].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.DeliveryAddress].HeaderText = GetResource("T_DELIVERYADDRESS");

                            tabStyle.GridColumnStyles[CN.BuffNo].Width = 125;
                            tabStyle.GridColumnStyles[CN.BuffNo].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.BuffNo].HeaderText = GetResource("T_DELNOTENUMBER");

                            tabStyle.GridColumnStyles[CN.DateReqDel].Width = 100;
                            tabStyle.GridColumnStyles[CN.DateReqDel].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.DateReqDel].HeaderText = GetResource("T_REQDELDATE");
                            tabStyle.GridColumnStyles[CN.DateReqDel].Alignment = HorizontalAlignment.Center;

                            tabStyle.GridColumnStyles[CN.TimeReqDel].Width = 100;
                            tabStyle.GridColumnStyles[CN.TimeReqDel].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.TimeReqDel].HeaderText = GetResource("T_REQDELTIME");
                            tabStyle.GridColumnStyles[CN.TimeReqDel].Alignment = HorizontalAlignment.Center;

                            tabStyle.GridColumnStyles[CN.DeliveryProcess].Width = 70;
                            tabStyle.GridColumnStyles[CN.DeliveryProcess].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.DeliveryProcess].HeaderText = GetResource("T_IMMEDIATE");

                            tabStyle.GridColumnStyles[CN.AssemblyRequired].Width = 60;
                            tabStyle.GridColumnStyles[CN.AssemblyRequired].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.AssemblyRequired].HeaderText = GetResource("T_ASSEMBLY");

                            tabStyle.GridColumnStyles[CN.Notes].Width = 120;
                            tabStyle.GridColumnStyles[CN.Notes].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.Notes].HeaderText = GetResource("T_NOTES");

                            tabStyle.GridColumnStyles[CN.Damaged].Width = 90;
                            tabStyle.GridColumnStyles[CN.Damaged].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.Damaged].HeaderText = GetResource("T_DAMAGESTOCK");

                            tabStyle.GridColumnStyles[CN.DeliveryArea].Width = 90;
                            tabStyle.GridColumnStyles[CN.DeliveryArea].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.DeliveryArea].HeaderText = GetResource("T_DELIVERYAREA");

                            //IP - 07/06/12 - #10229 - Warehouse & Deliveries
                            tabStyle.GridColumnStyles[CN.Express].Width = 55;
                            tabStyle.GridColumnStyles[CN.Express].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.Express].HeaderText = GetResource("T_EXPRESS");

                            tabStyle.GridColumnStyles[CN.Qtydiff].Width = 0;
                            tabStyle.GridColumnStyles[CN.ItemType].Width = 0;
                            tabStyle.GridColumnStyles[CN.ContractNo].Width = 0;
                            tabStyle.GridColumnStyles[CN.AgrmtNo].Width = 0;
                            tabStyle.GridColumnStyles[CN.Quantity].Width = 0;
                            tabStyle.GridColumnStyles[CN.ItemId].Width = 0;
                            tabStyle.GridColumnStyles[CN.ParentItemId].Width = 0;   // 01/06/12 jec
                            tabStyle.GridColumnStyles[CN.ID].Width = 0;             // #10284
                            tabStyle.GridColumnStyles[CN.Price].Width = 0;             // #13490
                            tabStyle.GridColumnStyles["origdeliveryprocess"].Width = 0;             // #10284
                        }

                        gbAmend.Enabled = (ds.Tables["Table1"].Rows.Count > 0);
                        // #10342 set delivery address dropdown   jec 11/06/12
                        drpDeliveryAddress.DataSource = ds.Tables["Table2"];
                        drpDeliveryAddress.DisplayMember = "AddType";

                        int i = 0;
                        i = this.drpDeliveryAddress.FindStringExact(GetResource("L_HOME"));
                        if (i >= 0) this.drpDeliveryAddress.SelectedIndex = i;

                        i = this.drpDamaged.FindStringExact("N");
                        if (i >= 0) this.drpDamaged.SelectedIndex = i;
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        public void txtAccountNo_Leave(object sender, System.EventArgs e)       // #10327 public - accessed from failed bookings screen
        {
            LoadOrderDetails();
            LoadAccountStatus();
            btnConfirm.Enabled = false; //SC 20/9/07 issue 69213
        }

        private void SetDeliveryAreaList(string branchNo)
        {
            // The Delivery Area list is specific to the branch selected
            string curDeliveryArea = drpDeliveryArea.Text;
            StringCollection areaList = new StringCollection();
            //areaList.Add(GetResource("L_ALL"));                       //#12855
            areaList.Add(" ");                                          //#12855
            if (this._areaTable != null)
            {
                foreach (DataRow row in this._areaTable.Rows)
                {
                    if (Convert.ToString(row[CN.BranchNo]) == branchNo)
                        areaList.Add((string)row.ItemArray[1]);
                }
            }
            drpDeliveryArea.DataSource = areaList;

            // Leave the Delivery Area on the current setting if it
            // is still in the list
            int i = drpDeliveryArea.FindStringExact(curDeliveryArea);
            if (i >= 0) drpDeliveryArea.SelectedIndex = i;
            else drpDeliveryArea.SelectedIndex = 0;
        }

        private void btnConfirm_Click(object sender, System.EventArgs e)
        {
            var status = true;           //#12855

            try
            {
                Wait();

                //#12855
                if (cbImmediate.Checked == false)
                {
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

                acctNo = txtAccountNo.UnformattedText;

                if (status)                  //#12855
                {
                    bool locked = AccountManager.LockAccount(acctNo, Credential.UserId.ToString(), out error);
                    if (error.Length > 0)
                        ShowError(error);
                    else
                    {
                        if (locked)
                        {
                            int index = dgOrderDetails.CurrentRowIndex;
                            if (index >= 0)
                            {
                                if (CheckSchedules(index))
                                {
                                    DataSet ds = new DataSet();
                                    DataTable dt = new DataTable("TN.Items");
                                    dt.Columns.Add(new DataColumn(CN.AcctNo, System.Type.GetType("System.String")));
                                    dt.Columns.Add(new DataColumn(CN.AgrmtNo, System.Type.GetType("System.Int32")));
                                    dt.Columns.Add(new DataColumn(CN.ItemNo, System.Type.GetType("System.String")));
                                    dt.Columns.Add(new DataColumn(CN.StockLocn, System.Type.GetType("System.Int16")));
                                    dt.Columns.Add(new DataColumn(CN.DelNoteBranch, System.Type.GetType("System.Int16")));
                                    dt.Columns.Add(new DataColumn(CN.DeliveryAddress, System.Type.GetType("System.String")));
                                    dt.Columns.Add(new DataColumn(CN.DateReqDel, System.Type.GetType("System.DateTime")));
                                    dt.Columns.Add(new DataColumn(CN.TimeReqDel, System.Type.GetType("System.String")));
                                    dt.Columns.Add(new DataColumn(CN.DeliveryArea, System.Type.GetType("System.String")));
                                    dt.Columns.Add(new DataColumn(CN.DeliveryProcess, System.Type.GetType("System.String")));
                                    dt.Columns.Add(new DataColumn(CN.Notes, System.Type.GetType("System.String")));
                                    dt.Columns.Add(new DataColumn(CN.Damaged, System.Type.GetType("System.String")));
                                    dt.Columns.Add(new DataColumn(CN.BranchNo, System.Type.GetType("System.Int16")));
                                    dt.Columns.Add(new DataColumn(CN.BuffNo, System.Type.GetType("System.Int32")));
                                    dt.Columns.Add(new DataColumn(CN.AssemblyRequired, System.Type.GetType("System.String")));
                                    dt.Columns.Add(new DataColumn(CN.ItemId, System.Type.GetType("System.Int32")));
                                    dt.Columns.Add(new DataColumn(CN.ParentItemId, System.Type.GetType("System.Int32")));
                                    dt.Columns.Add(new DataColumn(CN.ID, System.Type.GetType("System.Int32")));      // #10230
                                    dt.Columns.Add(new DataColumn("OrigDelProc", System.Type.GetType("System.String")));      // #10230
                                    dt.Columns.Add(new DataColumn(CN.Express, System.Type.GetType("System.String")));         //IP - 07/06/12 - #10229 - Warehouse & Deliveries
                                    dt.Columns.Add(new DataColumn("AwaitDA", System.Type.GetType("System.String")));        // #10346
                                    dt.Columns.Add(new DataColumn(CN.Price, System.Type.GetType("System.Single")));        // #13490


                                    DataRow row = dt.NewRow();
                                    row[CN.AcctNo] = acctNo;
                                    row[CN.AgrmtNo] = Convert.ToInt32(((DataView)dgOrderDetails.DataSource)[index][CN.AgrmtNo]);
                                    row[CN.ItemNo] = (string)((DataView)dgOrderDetails.DataSource)[index][CN.ItemNo];
                                    row[CN.StockLocn] = Convert.ToInt16(drpLocation.SelectedValue);
                                    row[CN.DelNoteBranch] = Convert.ToInt16(drpBranchForDel.SelectedValue);
                                    row[CN.DeliveryAddress] = ((DataRowView)drpDeliveryAddress.SelectedItem)[CN.AddType];      // #10342 
                                    row[CN.DateReqDel] = dtDeliveryRequired.Value;
                                    //row[CN.TimeReqDel] = txtTimeRequired.Text;
                                    row[CN.TimeReqDel] = Convert.ToString(cbTime.SelectedItem);             //IP - 30/05/12 - #10230 - Warehouse & Deliveries Integration
                                    row[CN.DeliveryArea] = drpDeliveryArea.Text == GetResource("L_ALL") ? "" : (string)drpDeliveryArea.SelectedItem;
                                    row[CN.DeliveryProcess] = cbImmediate.Checked ? "I" : "S";
                                    row[CN.Notes] = txtColourTrim.Text;
                                    row[CN.Damaged] = (string)drpDamaged.SelectedItem;
                                    row[CN.BranchNo] = Convert.ToInt16(((DataView)dgOrderDetails.DataSource)[index][CN.StockLocn]);
                                    row[CN.BuffNo] = Convert.ToInt32(((DataView)dgOrderDetails.DataSource)[index][CN.BuffNo]);
                                    row[CN.AssemblyRequired] = cbAssembly.Checked ? "Y" : "N";
                                    row[CN.ItemId] = Convert.ToInt32(((DataView)dgOrderDetails.DataSource)[index][CN.ItemId]);
                                    row[CN.ParentItemId] = Convert.ToInt32(((DataView)dgOrderDetails.DataSource)[index][CN.ParentItemId]);
                                    row[CN.ID] = Convert.ToInt32(((DataView)dgOrderDetails.DataSource)[index][CN.ID]);        // #10230
                                    row["OrigDelProc"] = Convert.ToString(((DataView)dgOrderDetails.DataSource)[index]["origdeliveryprocess"]);        // #10230
                                    row[CN.Express] = cbExpress.Checked ? "Y" : "N";                    //IP - 07/06/12 - #10229 - Warehouse & Deliveries
                                    row["AwaitDA"] = awaitDA;           // #10346
                                    row[CN.Price] = Convert.ToSingle(((DataView)(dgOrderDetails.DataSource))[index][CN.Price]);         //#13490
                                    //IP - 29/04/09 - CR929 & 974 - Deliveries
                                    //If the item being changed has no schedule record (before DA) then we do not want
                                    //to check if a newdelnote is required.
                                    //bool newDelNote = NewDelNoteRequired(index);

                                    bool newDelNote = false;
                                    if (Convert.ToInt32(row[CN.BuffNo]) != 0)
                                    {
                                        newDelNote = NewDelNoteRequired(index);
                                    }
                                    else
                                    {
                                        newDelNote = false;
                                    }

                                    dt.Rows.Add(row);
                                    ds.Tables.Add(dt);

                                    AccountManager.UpdateItemLocation(ds, newDelNote, out error);
                                    if (error.Length > 0)
                                        ShowError(error);
                                    else
                                    {
                                        btnClear_Click(this, null);
                                        txtAccountNo.Text = acctNo;
                                        txtAccountNo_Leave(this, null);
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
            finally
            {
                if (status)      //#12855
                {
                    AccountManager.UnlockAccount(acctNo, Credential.UserId, out error);
                    if (error.Length > 0)
                        ShowError(error);
                    btnConfirm.Enabled = false; //SC 20/9/07 issue 69213 
                    StopWait();
                }

            }
        }

        private bool NewDelNoteRequired(int rowIndex)
        {
            bool newDelNote = false;

            string delArea = drpDeliveryArea.Text == GetResource("L_ALL") ? "" : (string)drpDeliveryArea.SelectedItem;

            if (Convert.ToInt16(drpLocation.SelectedValue) != Convert.ToInt16(((DataView)dgOrderDetails.DataSource)[rowIndex][CN.StockLocn]) ||
                Convert.ToInt16(drpBranchForDel.SelectedValue) != Convert.ToInt16(((DataView)dgOrderDetails.DataSource)[rowIndex][CN.DelNoteBranch]) ||
                Convert.ToString(drpDeliveryAddress.SelectedItem) != Convert.ToString((string)((DataView)dgOrderDetails.DataSource)[rowIndex][CN.DeliveryAddress]).Trim() ||        // #10712
                dtDeliveryRequired.Value != Convert.ToDateTime(((DataView)dgOrderDetails.DataSource)[rowIndex][CN.DateReqDel]) ||
                //txtTimeRequired.Text != (string)((DataView)dgOrderDetails.DataSource)[rowIndex][CN.TimeReqDel] ||
                 Convert.ToString(cbTime.SelectedItem) != (string)((DataView)dgOrderDetails.DataSource)[rowIndex][CN.TimeReqDel] ||                     //IP - 30/05/12 -#10230 -  Warehouse & Deliveries Integration
                delArea != Convert.ToString((string)((DataView)dgOrderDetails.DataSource)[rowIndex][CN.DeliveryArea]).Trim() ||         // #10712
                cbImmediate.Checked != Convert.ToBoolean(((DataView)dgOrderDetails.DataSource)[rowIndex][CN.DeliveryProcess]))
                newDelNote = true;

            return newDelNote;
        }

        private void btnClear_Click(object sender, System.EventArgs e)
        {
            txtAccountNo.Text = "000000000000";
            drpLocation.SelectedIndex = 0;
            drpBranchForDel.SelectedIndex = 0;
            drpDeliveryAddress.SelectedIndex = 0;
            drpDeliveryArea.SelectedIndex = 0;
            drpDamaged.SelectedIndex = 0;
            dtDeliveryRequired.Value = DateTime.Today;
            //txtTimeRequired.Text = "";                        //IP - 30/05/12 -#10230 - Warehouse & Deliveries Integration
            cbImmediate.Checked = false;
            txtColourTrim.Text = "";
            dgOrderDetails.DataSource = null;
            dgApplicationStatus.DataSource = null;
            btnConfirm.Enabled = false; //SC 20/9/07 69213
        }

        private void LoadAccountStatus()
        {
            try
            {
                Function = "LoadAccountStatus";
                Wait();

                // Load account status
                DataSet accountStatus = AccountManager.AccountApplicationStatus(txtAccountNo.UnformattedText, out Error);

                if (Error.Length > 0)
                {
                    ShowError(Error);
                }
                else
                {
                    DataView ApplicationStatusView = new DataView(accountStatus.Tables[0]);

                    if (ApplicationStatusView.Count > 0)		//make sure we have some records
                    {
                        //Set the datasource of the datagrid that is a member variable of
                        //the transactionsTab object (set up the column styles also)
                        dgApplicationStatus.DataSource = ApplicationStatusView;

                        if (dgApplicationStatus.TableStyles.Count == 0)
                        {
                            DataGridTableStyle tabStyle = new DataGridTableStyle();
                            tabStyle.MappingName = ApplicationStatusView.Table.TableName;
                            dgApplicationStatus.TableStyles.Add(tabStyle);

                            // Set the table style according to the user's preference
                            tabStyle.GridColumnStyles[CN.StatusCode].Width = 100;
                            tabStyle.GridColumnStyles[CN.StatusCode].HeaderText = GetResource("T_STATUSCODE");
                            tabStyle.GridColumnStyles[CN.StatusDescription].Width = 300;
                            tabStyle.GridColumnStyles[CN.StatusDescription].HeaderText = GetResource("T_STATUSDESC");
                        }

                        // #10346 check for authorised
                        foreach (DataRow dr in accountStatus.Tables[0].Rows)
                        {
                            if (Convert.ToString(dr[CN.StatusCode]) == "ADA")
                            {
                                awaitDA = "Y";
                            }
                        }
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
                Function = "End of LoadAccountStatus";
            }
        }

        private void HashMenus()
        {
            dynamicMenus[this.Name + ":lreviseNonScheduled"] = this.lreviseNonScheduled;
            dynamicMenus[this.Name + ":lreviseScheduled"] = this.lreviseScheduled;
            dynamicMenus[this.Name + ":lblBeforeDA"] = this.lblBeforeDA; //IP - 28/04/09 - CR929 & 974 - Deliveries
        }

        private void SetComboLocations(int index)
        {
            var delloc = locations.Copy();
            locations.DefaultView.RowFilter = "itemid = '" + Convert.ToInt32(((DataView)dgOrderDetails.DataSource)[index][CN.ItemId]) + "'";
            delloc.DefaultView.RowFilter = "itemid = '" + Convert.ToInt32(((DataView)dgOrderDetails.DataSource)[index][CN.ItemId]) + "'";

            drpLocation.DataSource = locations.DefaultView;
            drpBranchForDel.DataSource = delloc.DefaultView;
            drpLocation.DisplayMember = "stocklocn";
            drpBranchForDel.DisplayMember = "stocklocn";
            drpLocation.ValueMember = "stocklocn";
            drpBranchForDel.ValueMember = "stocklocn";


        }

        private void dgOrderDetails_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            int index = dgOrderDetails.CurrentRowIndex;
            SetComboLocations(index);
            errorProvider1.SetError(drpDeliveryArea, "");               //#12855

            if (index >= 0)
            {
                int x = 0;
                int branchNo = Convert.ToInt32(((DataView)dgOrderDetails.DataSource)[index][CN.StockLocn]);
                x = drpLocation.FindString(branchNo.ToString());
                if (x != -1)
                    drpLocation.SelectedIndex = x;

                int delNotebranchNo = Convert.ToInt32(((DataView)dgOrderDetails.DataSource)[index][CN.DelNoteBranch]);
                x = drpBranchForDel.FindString(delNotebranchNo.ToString());
                if (x != -1)
                    drpBranchForDel.SelectedIndex = x;

                string delAddress = ((string)((DataView)dgOrderDetails.DataSource)[index][CN.DeliveryAddress]).Trim();
                x = drpDeliveryAddress.FindString(delAddress);
                if (x != -1)
                    drpDeliveryAddress.SelectedIndex = x;

                dtDeliveryRequired.Value = Convert.ToDateTime(((DataView)dgOrderDetails.DataSource)[index][CN.DateReqDel]);
                //txtTimeRequired.Text = (string)((DataView)dgOrderDetails.DataSource)[index][CN.TimeReqDel];
                cbTime.SelectedItem = (string)((DataView)dgOrderDetails.DataSource)[index][CN.TimeReqDel];                      //IP - 30/05/12 -#10230 -  Warehouse & Deliveries Integration

                bool immediate = Convert.ToBoolean(((DataView)dgOrderDetails.DataSource)[index][CN.DeliveryProcess]);
                cbImmediate.Checked = immediate;

                bool assembly = Convert.ToBoolean(((DataView)dgOrderDetails.DataSource)[index][CN.AssemblyRequired]);
                cbAssembly.Checked = assembly;

                string express = Convert.ToString(((DataView)dgOrderDetails.DataSource)[index][CN.Express]);                     //IP - 07/06/12 - #10229 - Warehouse & Deliveries
                cbExpress.Checked = express == "Y" ? true : false;

                string delArea = ((string)((DataView)dgOrderDetails.DataSource)[index][CN.DeliveryArea]).Trim();
                x = drpDeliveryArea.FindString(delArea);
                if (x != -1)
                    drpDeliveryArea.SelectedIndex = x;

                string damaged = ((string)((DataView)dgOrderDetails.DataSource)[index][CN.Damaged]).Trim();
                x = drpDamaged.FindString(damaged);
                if (x != -1)
                    drpDamaged.SelectedIndex = x;

                txtColourTrim.Text = (string)((DataView)dgOrderDetails.DataSource)[index][CN.Notes];
                btnConfirm.Enabled = true; // SC 20/9/07 - issue 69213
            }
        }

        //IP - 08/06/12 - #10300 - Warehouse & Deliveries
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

        private void cbImmediate_CheckedChanged(object sender, EventArgs e)
        {
            errorProvider1.SetError(drpDeliveryArea, "");
        }
    }
}
