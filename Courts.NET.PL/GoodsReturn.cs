using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using STL.Common;
using STL.Common.Constants.AccountHolders;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.Categories;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using STL.Common.Static;
using STL.PL.SERVICE;
using Blue.Cosacs.Shared.Services;
using Blue.Cosacs.Shared.Services.Warranty;
using System.Collections.Generic;
using Blue.Cosacs.Shared.CosacsWeb.Models;
using STL.PL.Utils;
using Blue.Cosacs.Shared;
using STL.PL.WS2;
using STL.Common.Constants.Tags;
using STL.Common.Services;
using System.Text;

namespace STL.PL
{
    /// <summary>
    /// Goods return processing for the return, replacement and repossession
    /// of goods. An account number is entered and all of the delivered items
    /// on the account are listed. Only these delivered items can be selected
    /// for return. The user will be prompted to optionally return linked stock
    /// items (such as warranties and discounts). The return item number and
    /// the return value will be calculated based on the type of return being
    /// performed. The return delivery can either be scheduled or an immediate
    /// delivery.
    /// </summary>
    /// 

    //#15993
    struct DictionaryKey 
    {
        public readonly string itemno;
        public readonly string contractno;

        public DictionaryKey(string p1, string p2) 
        {
            itemno = p1;
            contractno = p2;
        }

        public override bool Equals(Object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            DictionaryKey p = (DictionaryKey)obj;
            return (itemno == p.itemno) && (contractno == p.contractno);
        }
    }

    public class GoodsReturn : CommonForm
    {
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private STL.PL.AccountTextBox txtAccountNo;
        private System.Windows.Forms.TextBox txtCustID;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtArrears;
        private System.Windows.Forms.TextBox txtOutstandingBalance;
        private System.Windows.Forms.TextBox txtCurrentStatus;
        private System.Windows.Forms.Label label41;
        private System.Windows.Forms.Label label40;
        private System.Windows.Forms.Label label42;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnEnter;
        private System.Windows.Forms.Label label11;
        private Crownwood.Magic.Menus.MenuCommand menuFile;
        private Crownwood.Magic.Menus.MenuCommand menuExit;
        private System.Windows.Forms.TextBox txtCreditTotal;
        private System.Windows.Forms.TextBox txtTotalValue;
        private System.Windows.Forms.TextBox txtQty;
        private System.Windows.Forms.TextBox txtItemNo;
        private System.Windows.Forms.TextBox txtLocn;
        private System.Windows.Forms.TextBox txtReturnLocn;
        private System.Windows.Forms.TextBox txtReturnItemNo;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.DataGrid dgGoodsToReturn;
        private System.Windows.Forms.ComboBox drpReposession;
        private System.Windows.Forms.ComboBox drpCredReplace;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnSave;
        //private XmlNode lineItems = null;
        private decimal agrmtTotal = 0;
        private double maxReturnQty = 0;
        private decimal price = 0;
        private DataView dv = null;
        DataTable tab = null;

        private new string Error = "";
        private string acctType = "";
        private System.Windows.Forms.Button btnClear;
        private string accountNo = "";
        private DataSet agreement = null;
        private DataSet repo = null;
        private string accountType = "";
        //bool validRetItem = true;
        private StringCollection warrantiesToCollect = new StringCollection();
        private System.Windows.Forms.ComboBox drpRepoType;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox drpReason;
        private DataSet dropDownDS = null;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Label lItemNo;
        private System.Windows.Forms.Label lWarrantyItemNo;
        private string parentReturnLocation = "";
        private System.Windows.Forms.Label label5;
        private DataTable _deliveryAreaData;
        private System.Windows.Forms.RadioButton rbScheduling;
        private System.Windows.Forms.RadioButton rbPickupImmed;
        private DataTable associatedItems = null;
        private decimal expiredPcent = 0;
        //bool payPortion = false;
        //string collectReason = "";  //CR36
        bool collectWarrantyOnly = false;
        private System.Windows.Forms.Label lAuthorise;
        new bool loaded = false;
        DataSet dsWarranties = null;
        private TextBox tbNotes;
        private Label lbNotes;
        private IContainer components;
        private bool loanAccount = false;   //LW72938 jec 25/11/10
        private int selectedItemId;         // RI jec
        public bool QuantityChanged = false;           //CR2018-013 

        public const string Stock = "S";
        public const string NonStock = "N";
        private int? requestNo = null;              //#11989
        private decimal warrantyPercentageReturn = 0;
        private ToolTip toolTipDelArea;
        private AppUpdater.AppUpdater appUpdater1;
        private DataTable delAreas = null;
        private ComboBox drpCollectionAdr;  //#12224 - CR12249
        //Dictionary<Tuple<string, DateTime>, WarrantyReturnDetails> warrantyReturnPC = new Dictionary<Tuple<string, DateTime>, WarrantyReturnDetails>(); //#15993
         Dictionary<DictionaryKey, WarrantyReturnDetails> warrantyReturnPC = new Dictionary<DictionaryKey, WarrantyReturnDetails>(); //#15993

        private DataTable instReplace = null;   //#17290     
        private DataView dvir = null;           //#17290   
        int irIndex = 0;                        //#17290
        DataSet dsI = null;                     //#17290
        RepossessedCondition[] RepossessedConditions = null;
        List<string> validRepossessionReturnItemNos = new List<string>();
        private TextBox txtShowInvoiceNo;
        private Label lblInvoiceNo;
        private Button btnInvoiceSearch;
        private bool readyAssistCancellation = false;       //#18608 - CR15594
        private string InvoiceNo = ""; //CR2018-13 06Dec2018
        private TextBox txtInvoiceNo;
        private GroupBox groupBox3;
        private DataGrid dgDeliveredItems;
        private Label lblOrder;
        private string AccountNo = ""; //CR2018-13 06Dec2018

        public GoodsReturn(TranslationDummy d)
        {
            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile });
        }

        //#11989
        public GoodsReturn(Form root, Form parent, string acctNo, int serviceRequestNo)
        {
            FormRoot = root;
            FormParent = parent;
            this.requestNo = serviceRequestNo;

            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile });

            tab = new DataTable("RetDetails");
            tab.Columns.Add(CN.Quantity);
            tab.Columns.Add(CN.ItemId);                     //IP - 17/05/11 - CR1212 - #3627
            tab.Columns.Add(CN.ItemNo);
            tab.Columns.Add(CN.RetItemNo);
            tab.Columns.Add(CN.RetItemId);                     //RI jec 20/05/11
            tab.Columns.Add(CN.StockLocn);
            tab.Columns.Add(CN.RetStockLocn);
            tab.Columns.Add(CN.OrdVal);
            tab.Columns.Add(CN.RetVal);
            tab.Columns.Add(CN.Type);
            tab.Columns.Add(CN.ContractNo);
            tab.Columns.Add(CN.ItemType);
            tab.Columns.Add(CN.Category, Type.GetType("System.Int16"));
            tab.Columns.Add(CN.AgreementNo, Type.GetType("System.Int32"));
            tab.Columns.Add(CN.Reason);
            tab.Columns.Add(CN.ParentItemId);              //IP - 17/05/11 - CR1212 - #3627
            tab.Columns.Add(CN.ParentItemNo);
            tab.Columns.Add(CN.ParentLocation);
            tab.Columns.Add(CN.TaxAmt);
            tab.Columns.Add(CN.DeliveryArea);
            tab.Columns.Add(CN.DeliveryProcess);
            tab.Columns.Add(CN.Value);
            tab.Columns.Add(CN.WarrantyFulfilled);
            tab.Columns.Add(CN.Refund);
            tab.Columns.Add(CN.RefundType);
            tab.Columns.Add(CN.WarrantyCollection);
            tab.Columns.Add(CN.ReadOnly);
            tab.Columns.Add(CN.LineItemId);         // #10411
            tab.Columns.Add("DateDel");             // #17290
            tab.Columns.Add("ReadyAssist");         // #18607 - CR15594
            tab.Columns.Add("ReadyAssistUsed");     // #18607 - CR15594
            tab.Columns.Add("AnnualServiceContract");        
            tab.Columns.Add("AnnualServiceContractUsed");  

            associatedItems = new DataTable();
            associatedItems.Columns.Add(CN.AssociatedItem);
            associatedItems.Columns.Add(CN.StockLocn);
            associatedItems.Columns.Add(CN.ContractNo);

            LoadStatic();
            //TranslateControls();

            //IP - 10/02/10 - CR1048 (Ref:3.1.4 & 3.1.5) Merged - Malaysia Enhancements (CR1072)
            tbNotes.Visible = (bool)Country[CountryParameterNames.EnableGRTnotes];      //CR1048 jec
            lbNotes.Visible = (bool)Country[CountryParameterNames.EnableGRTnotes];      //CR1048 jec		

            txtAccountNo.Text = acctNo;

            btnSearch_Click(null, null);

            drpCollectionAdr.DataSource = repo.Tables[1];       // #14927

           // toolTipDelArea.SetToolTip(btnDelAreas, "Delivery Area Descriptions");                //#14796

        }

        public GoodsReturn(Form root, Form parent)
        {
            FormRoot = root;
            FormParent = parent;

            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile });

            tab = new DataTable("RetDetails");
            tab.Columns.Add(CN.Quantity);
            tab.Columns.Add(CN.ItemId);                     //IP - 17/05/11 - CR1212 - #3627
            tab.Columns.Add(CN.ItemNo);
            tab.Columns.Add(CN.RetItemNo);
            tab.Columns.Add(CN.RetItemId);                     //RI jec 20/05/11
            tab.Columns.Add(CN.StockLocn);
            tab.Columns.Add(CN.RetStockLocn);
            tab.Columns.Add(CN.OrdVal);
            tab.Columns.Add(CN.RetVal);
            tab.Columns.Add(CN.Type);
            tab.Columns.Add(CN.ContractNo);
            tab.Columns.Add(CN.ItemType);
            tab.Columns.Add(CN.Category, Type.GetType("System.Int16"));
            tab.Columns.Add(CN.AgreementNo, Type.GetType("System.Int32"));
            tab.Columns.Add(CN.Reason);
            tab.Columns.Add(CN.ParentItemId);              //IP - 17/05/11 - CR1212 - #3627
            tab.Columns.Add(CN.ParentItemNo);
            tab.Columns.Add(CN.ParentLocation);
            tab.Columns.Add(CN.TaxAmt);
            tab.Columns.Add(CN.DeliveryArea);
            tab.Columns.Add(CN.DeliveryAddress);            // #14927   
            tab.Columns.Add(CN.DeliveryProcess);
            tab.Columns.Add(CN.Value);
            tab.Columns.Add(CN.WarrantyFulfilled);
            tab.Columns.Add(CN.Refund);
            tab.Columns.Add(CN.RefundType);
            tab.Columns.Add(CN.WarrantyCollection);
            tab.Columns.Add(CN.ReadOnly);
            tab.Columns.Add(CN.LineItemId);         // #10411
            tab.Columns.Add(CN.Price);         // #12842
            tab.Columns.Add("DateDel");             // #17290
            tab.Columns.Add("ReadyAssist");         // #18607 - CR15594
            tab.Columns.Add("ReadyAssistUsed");     // #18607 - CR15594
            tab.Columns.Add("AnnualServiceContract");     
            tab.Columns.Add("AnnualServiceContractUsed");  

            associatedItems = new DataTable();
            associatedItems.Columns.Add(CN.AssociatedItem);
            associatedItems.Columns.Add(CN.StockLocn);
            associatedItems.Columns.Add(CN.ContractNo);

            LoadStatic();
            //TranslateControls();

            //IP - 10/02/10 - CR1048 (Ref:3.1.4 & 3.1.5) Merged - Malaysia Enhancements (CR1072)
            tbNotes.Visible = (bool)Country[CountryParameterNames.EnableGRTnotes];      //CR1048 jec
            lbNotes.Visible = (bool)Country[CountryParameterNames.EnableGRTnotes];      //CR1048 jec	

           // toolTipDelArea.SetToolTip(btnDelAreas, GetResource("TT_DELAREA"));                //#14796	
        }


        public string CollectionType
        {
            get
            {
                string code = string.Empty;
                if (drpCredReplace.SelectedItem != null)
                {
                    code = (string)((DataRowView)drpCredReplace.SelectedItem)[CN.Code];
                }
                return code;
            }
        }

        private DateTime m_deliverydate;
        private DateTime deliveryDate
        {
            get
            {
                return m_deliverydate;
            }
            set
            {
                m_deliverydate = value;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GoodsReturn));
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            this.toolTipDelArea = new System.Windows.Forms.ToolTip(this.components);
            this.appUpdater1 = new STL.AppUpdater.AppUpdater(this.components);
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.dgDeliveredItems = new System.Windows.Forms.DataGrid();
            this.txtShowInvoiceNo = new System.Windows.Forms.TextBox();
            this.lblInvoiceNo = new System.Windows.Forms.Label();
            this.lWarrantyItemNo = new System.Windows.Forms.Label();
            this.drpCollectionAdr = new System.Windows.Forms.ComboBox();
            this.tbNotes = new System.Windows.Forms.TextBox();
            this.lbNotes = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.drpReason = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.drpRepoType = new System.Windows.Forms.ComboBox();
            this.drpCredReplace = new System.Windows.Forms.ComboBox();
            this.drpReposession = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtCreditTotal = new System.Windows.Forms.TextBox();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnEnter = new System.Windows.Forms.Button();
            this.dgGoodsToReturn = new System.Windows.Forms.DataGrid();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lItemNo = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtTotalValue = new System.Windows.Forms.TextBox();
            this.txtQty = new System.Windows.Forms.TextBox();
            this.txtItemNo = new System.Windows.Forms.TextBox();
            this.txtLocn = new System.Windows.Forms.TextBox();
            this.txtReturnLocn = new System.Windows.Forms.TextBox();
            this.txtReturnItemNo = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.rbScheduling = new System.Windows.Forms.RadioButton();
            this.rbPickupImmed = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblOrder = new System.Windows.Forms.Label();
            this.txtInvoiceNo = new System.Windows.Forms.TextBox();
            this.btnInvoiceSearch = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.label41 = new System.Windows.Forms.Label();
            this.label40 = new System.Windows.Forms.Label();
            this.label42 = new System.Windows.Forms.Label();
            this.txtArrears = new System.Windows.Forms.TextBox();
            this.txtOutstandingBalance = new System.Windows.Forms.TextBox();
            this.txtCurrentStatus = new System.Windows.Forms.TextBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtCustID = new System.Windows.Forms.TextBox();
            this.txtAccountNo = new STL.PL.AccountTextBox();
            this.lAuthorise = new System.Windows.Forms.Label();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.appUpdater1)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgDeliveredItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgGoodsToReturn)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
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
            this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox2.Controls.Add(this.groupBox3);
            this.groupBox2.Controls.Add(this.txtShowInvoiceNo);
            this.groupBox2.Controls.Add(this.lblInvoiceNo);
            this.groupBox2.Controls.Add(this.lWarrantyItemNo);
            this.groupBox2.Controls.Add(this.drpCollectionAdr);
            this.groupBox2.Controls.Add(this.tbNotes);
            this.groupBox2.Controls.Add(this.lbNotes);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.drpReason);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.drpRepoType);
            this.groupBox2.Controls.Add(this.drpCredReplace);
            this.groupBox2.Controls.Add(this.drpReposession);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.txtCreditTotal);
            this.groupBox2.Controls.Add(this.btnRemove);
            this.groupBox2.Controls.Add(this.btnEnter);
            this.groupBox2.Controls.Add(this.dgGoodsToReturn);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.lItemNo);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.txtTotalValue);
            this.groupBox2.Controls.Add(this.txtQty);
            this.groupBox2.Controls.Add(this.txtItemNo);
            this.groupBox2.Controls.Add(this.txtLocn);
            this.groupBox2.Controls.Add(this.txtReturnLocn);
            this.groupBox2.Controls.Add(this.txtReturnItemNo);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.rbScheduling);
            this.groupBox2.Controls.Add(this.rbPickupImmed);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 98);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(787, 463);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Return Details";
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox3.Controls.Add(this.dgDeliveredItems);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox3.Location = new System.Drawing.Point(3, 332);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(781, 128);
            this.groupBox3.TabIndex = 41;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Delivered Items";
            // 
            // dgDeliveredItems
            // 
            this.dgDeliveredItems.DataMember = "";
            this.dgDeliveredItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgDeliveredItems.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgDeliveredItems.Location = new System.Drawing.Point(3, 16);
            this.dgDeliveredItems.Name = "dgDeliveredItems";
            this.dgDeliveredItems.ReadOnly = true;
            this.dgDeliveredItems.Size = new System.Drawing.Size(775, 109);
            this.dgDeliveredItems.TabIndex = 0;
            this.dgDeliveredItems.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgDeliveredItems_MouseUp);
            // 
            // txtShowInvoiceNo
            // 
            this.txtShowInvoiceNo.Location = new System.Drawing.Point(106, 15);
            this.txtShowInvoiceNo.Name = "txtShowInvoiceNo";
            this.txtShowInvoiceNo.ReadOnly = true;
            this.txtShowInvoiceNo.Size = new System.Drawing.Size(102, 20);
            this.txtShowInvoiceNo.TabIndex = 39;
            // 
            // lblInvoiceNo
            // 
            this.lblInvoiceNo.Location = new System.Drawing.Point(12, 15);
            this.lblInvoiceNo.Name = "lblInvoiceNo";
            this.lblInvoiceNo.Size = new System.Drawing.Size(80, 16);
            this.lblInvoiceNo.TabIndex = 38;
            this.lblInvoiceNo.Text = "Ord\\InvoiceNo";
            this.lblInvoiceNo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lWarrantyItemNo
            // 
            this.lWarrantyItemNo.Location = new System.Drawing.Point(208, 34);
            this.lWarrantyItemNo.Name = "lWarrantyItemNo";
            this.lWarrantyItemNo.Size = new System.Drawing.Size(96, 16);
            this.lWarrantyItemNo.TabIndex = 33;
            this.lWarrantyItemNo.Text = "Warranty Item No";
            this.lWarrantyItemNo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lWarrantyItemNo.Visible = false;
            // 
            // drpCollectionAdr
            // 
            this.drpCollectionAdr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpCollectionAdr.FormattingEnabled = true;
            this.drpCollectionAdr.Location = new System.Drawing.Point(120, 146);
            this.drpCollectionAdr.Name = "drpCollectionAdr";
            this.drpCollectionAdr.Size = new System.Drawing.Size(48, 21);
            this.drpCollectionAdr.TabIndex = 37;
            // 
            // tbNotes
            // 
            this.tbNotes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.tbNotes.Location = new System.Drawing.Point(57, 205);
            this.tbNotes.Multiline = true;
            this.tbNotes.Name = "tbNotes";
            this.tbNotes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbNotes.Size = new System.Drawing.Size(357, 121);
            this.tbNotes.TabIndex = 36;
            // 
            // lbNotes
            // 
            this.lbNotes.AutoSize = true;
            this.lbNotes.Location = new System.Drawing.Point(16, 200);
            this.lbNotes.Name = "lbNotes";
            this.lbNotes.Size = new System.Drawing.Size(35, 13);
            this.lbNotes.TabIndex = 35;
            this.lbNotes.Text = "Notes";
            // 
            // label13
            // 
            this.label13.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label13.Location = new System.Drawing.Point(8, 114);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(48, 16);
            this.label13.TabIndex = 32;
            this.label13.Text = "Reason";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // drpReason
            // 
            this.drpReason.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpReason.Location = new System.Drawing.Point(64, 114);
            this.drpReason.Name = "drpReason";
            this.drpReason.Size = new System.Drawing.Size(144, 21);
            this.drpReason.TabIndex = 31;
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(8, 66);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(104, 16);
            this.label12.TabIndex = 30;
            this.label12.Text = "Repossession Type";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label12.Click += new System.EventHandler(this.label12_Click);
            // 
            // drpRepoType
            // 
            this.drpRepoType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpRepoType.Enabled = false;
            this.drpRepoType.Items.AddRange(new object[] {
            "Full",
            "Part",
            "Voluntary"});
            this.drpRepoType.Location = new System.Drawing.Point(128, 66);
            this.drpRepoType.Name = "drpRepoType";
            this.drpRepoType.Size = new System.Drawing.Size(80, 21);
            this.drpRepoType.TabIndex = 29;
            // 
            // drpCredReplace
            // 
            this.drpCredReplace.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpCredReplace.Enabled = false;
            this.drpCredReplace.Location = new System.Drawing.Point(64, 90);
            this.drpCredReplace.Name = "drpCredReplace";
            this.drpCredReplace.Size = new System.Drawing.Size(144, 21);
            this.drpCredReplace.TabIndex = 27;
            this.drpCredReplace.SelectedIndexChanged += new System.EventHandler(this.drpCredReplace_SelectedIndexChanged);
            // 
            // drpReposession
            // 
            this.drpReposession.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpReposession.Enabled = false;
            this.drpReposession.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.drpReposession.Location = new System.Drawing.Point(131, 42);
            this.drpReposession.Name = "drpReposession";
            this.drpReposession.Size = new System.Drawing.Size(77, 21);
            this.drpReposession.TabIndex = 26;
            this.drpReposession.SelectedIndexChanged += new System.EventHandler(this.drpReposession_SelectedIndexChanged);
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(146, 179);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(160, 16);
            this.label11.TabIndex = 25;
            this.label11.Text = "Total Credit Value To Account";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtCreditTotal
            // 
            this.txtCreditTotal.Location = new System.Drawing.Point(312, 178);
            this.txtCreditTotal.Name = "txtCreditTotal";
            this.txtCreditTotal.ReadOnly = true;
            this.txtCreditTotal.Size = new System.Drawing.Size(72, 20);
            this.txtCreditTotal.TabIndex = 24;
            // 
            // btnRemove
            // 
            this.btnRemove.BackColor = System.Drawing.Color.SlateBlue;
            this.btnRemove.Enabled = false;
            this.btnRemove.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRemove.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.btnRemove.Image = global::STL.PL.Properties.Resources.Minus;
            this.btnRemove.Location = new System.Drawing.Point(392, 151);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(22, 22);
            this.btnRemove.TabIndex = 23;
            this.btnRemove.UseVisualStyleBackColor = false;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnEnter
            // 
            this.btnEnter.BackColor = System.Drawing.Color.SlateBlue;
            this.btnEnter.Enabled = false;
            this.btnEnter.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEnter.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.btnEnter.Image = global::STL.PL.Properties.Resources.plus;
            this.btnEnter.Location = new System.Drawing.Point(392, 106);
            this.btnEnter.Name = "btnEnter";
            this.btnEnter.Size = new System.Drawing.Size(22, 22);
            this.btnEnter.TabIndex = 22;
            this.btnEnter.UseVisualStyleBackColor = false;
            this.btnEnter.Click += new System.EventHandler(this.btnEnter_Click);
            // 
            // dgGoodsToReturn
            // 
            this.dgGoodsToReturn.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgGoodsToReturn.DataMember = "";
            this.dgGoodsToReturn.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgGoodsToReturn.Location = new System.Drawing.Point(420, 16);
            this.dgGoodsToReturn.Name = "dgGoodsToReturn";
            this.dgGoodsToReturn.Size = new System.Drawing.Size(364, 310);
            this.dgGoodsToReturn.TabIndex = 16;
            this.dgGoodsToReturn.Navigate += new System.Windows.Forms.NavigateEventHandler(this.dgGoodsToReturn_Navigate);
            this.dgGoodsToReturn.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgGoodsToReturn_MouseUp);
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(240, 154);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(64, 16);
            this.label10.TabIndex = 15;
            this.label10.Text = "Total Value";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(200, 130);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(104, 16);
            this.label9.TabIndex = 14;
            this.label9.Text = "Return Stock Locn";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(240, 106);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(64, 16);
            this.label8.TabIndex = 13;
            this.label8.Text = "Stock Locn";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(216, 82);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(88, 16);
            this.label7.TabIndex = 12;
            this.label7.Text = "Return Item No";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(256, 58);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(48, 16);
            this.label6.TabIndex = 11;
            this.label6.Text = "Quantity";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lItemNo
            // 
            this.lItemNo.Location = new System.Drawing.Point(256, 34);
            this.lItemNo.Name = "lItemNo";
            this.lItemNo.Size = new System.Drawing.Size(48, 16);
            this.lItemNo.TabIndex = 10;
            this.lItemNo.Text = "Item No";
            this.lItemNo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(8, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 16);
            this.label3.TabIndex = 9;
            this.label3.Text = "Collection";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 16);
            this.label2.TabIndex = 8;
            this.label2.Text = "Repossession ";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtTotalValue
            // 
            this.txtTotalValue.Location = new System.Drawing.Point(312, 154);
            this.txtTotalValue.Name = "txtTotalValue";
            this.txtTotalValue.ReadOnly = true;
            this.txtTotalValue.Size = new System.Drawing.Size(72, 20);
            this.txtTotalValue.TabIndex = 7;
            // 
            // txtQty
            // 
            this.txtQty.Location = new System.Drawing.Point(312, 58);
            this.txtQty.Name = "txtQty";
            this.txtQty.Size = new System.Drawing.Size(64, 20);
            this.txtQty.TabIndex = 6;
            this.txtQty.TextChanged += new System.EventHandler(this.txtQty_TextChanged);
            this.txtQty.Leave += new System.EventHandler(this.txtQty_Leave);
            // 
            // txtItemNo
            // 
            this.txtItemNo.Location = new System.Drawing.Point(312, 34);
            this.txtItemNo.Name = "txtItemNo";
            this.txtItemNo.ReadOnly = true;
            this.txtItemNo.Size = new System.Drawing.Size(102, 20);
            this.txtItemNo.TabIndex = 5;
            // 
            // txtLocn
            // 
            this.txtLocn.Location = new System.Drawing.Point(312, 106);
            this.txtLocn.Name = "txtLocn";
            this.txtLocn.ReadOnly = true;
            this.txtLocn.Size = new System.Drawing.Size(72, 20);
            this.txtLocn.TabIndex = 4;
            // 
            // txtReturnLocn
            // 
            this.txtReturnLocn.Location = new System.Drawing.Point(312, 130);
            this.txtReturnLocn.Name = "txtReturnLocn";
            this.txtReturnLocn.Size = new System.Drawing.Size(72, 20);
            this.txtReturnLocn.TabIndex = 3;
            this.txtReturnLocn.Leave += new System.EventHandler(this.txtReturnLocn_Leave);
            // 
            // txtReturnItemNo
            // 
            this.txtReturnItemNo.Location = new System.Drawing.Point(312, 82);
            this.txtReturnItemNo.Name = "txtReturnItemNo";
            this.txtReturnItemNo.ReadOnly = true;
            this.txtReturnItemNo.Size = new System.Drawing.Size(102, 20);
            this.txtReturnItemNo.TabIndex = 1;
            this.txtReturnItemNo.Leave += new System.EventHandler(this.txtReturnItemNo_Leave);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(11, 146);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(101, 21);
            this.label5.TabIndex = 32;
            this.label5.Text = "Collection Address";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // rbScheduling
            // 
            this.rbScheduling.Checked = true;
            this.rbScheduling.Enabled = false;
            this.rbScheduling.Location = new System.Drawing.Point(16, 170);
            this.rbScheduling.Name = "rbScheduling";
            this.rbScheduling.Size = new System.Drawing.Size(56, 32);
            this.rbScheduling.TabIndex = 0;
            this.rbScheduling.TabStop = true;
            this.rbScheduling.Text = "Sched";
            // 
            // rbPickupImmed
            // 
            this.rbPickupImmed.Enabled = false;
            this.rbPickupImmed.Location = new System.Drawing.Point(72, 170);
            this.rbPickupImmed.Name = "rbPickupImmed";
            this.rbPickupImmed.Size = new System.Drawing.Size(80, 32);
            this.rbPickupImmed.TabIndex = 0;
            this.rbPickupImmed.Text = "Immediate";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.lblOrder);
            this.groupBox1.Controls.Add(this.txtInvoiceNo);
            this.groupBox1.Controls.Add(this.btnInvoiceSearch);
            this.groupBox1.Controls.Add(this.btnClear);
            this.groupBox1.Controls.Add(this.btnSave);
            this.groupBox1.Controls.Add(this.btnSearch);
            this.groupBox1.Controls.Add(this.label41);
            this.groupBox1.Controls.Add(this.label40);
            this.groupBox1.Controls.Add(this.label42);
            this.groupBox1.Controls.Add(this.txtArrears);
            this.groupBox1.Controls.Add(this.txtOutstandingBalance);
            this.groupBox1.Controls.Add(this.txtCurrentStatus);
            this.groupBox1.Controls.Add(this.txtName);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtCustID);
            this.groupBox1.Controls.Add(this.txtAccountNo);
            this.groupBox1.Controls.Add(this.lAuthorise);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(787, 98);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Personal Details";
            // 
            // lblOrder
            // 
            this.lblOrder.AutoSize = true;
            this.lblOrder.Location = new System.Drawing.Point(36, 56);
            this.lblOrder.Name = "lblOrder";
            this.lblOrder.Size = new System.Drawing.Size(81, 13);
            this.lblOrder.TabIndex = 61;
            this.lblOrder.Text = "Ord/Invoice No";
            // 
            // txtInvoiceNo
            // 
            this.txtInvoiceNo.Location = new System.Drawing.Point(32, 73);
            this.txtInvoiceNo.MaxLength = 14;
            this.txtInvoiceNo.Name = "txtInvoiceNo";
            this.txtInvoiceNo.Size = new System.Drawing.Size(100, 20);
            this.txtInvoiceNo.TabIndex = 60;
            this.txtInvoiceNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtInvoiceNo_KeyPress);
            // 
            // btnInvoiceSearch
            // 
            this.btnInvoiceSearch.Image = ((System.Drawing.Image)(resources.GetObject("btnInvoiceSearch.Image")));
            this.btnInvoiceSearch.Location = new System.Drawing.Point(136, 63);
            this.btnInvoiceSearch.Name = "btnInvoiceSearch";
            this.btnInvoiceSearch.Size = new System.Drawing.Size(32, 32);
            this.btnInvoiceSearch.TabIndex = 59;
            this.btnInvoiceSearch.Click += new System.EventHandler(this.btnInvoiceSearch_Click);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.Location = new System.Drawing.Point(716, 65);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(64, 23);
            this.btnClear.TabIndex = 54;
            this.btnClear.Text = "&Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSave.BackgroundImage")));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSave.Location = new System.Drawing.Point(760, 28);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(24, 24);
            this.btnSave.TabIndex = 53;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.Image = ((System.Drawing.Image)(resources.GetObject("btnSearch.Image")));
            this.btnSearch.Location = new System.Drawing.Point(136, 25);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(32, 32);
            this.btnSearch.TabIndex = 52;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // label41
            // 
            this.label41.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label41.Location = new System.Drawing.Point(596, 13);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(48, 16);
            this.label41.TabIndex = 51;
            this.label41.Text = "Arrears";
            // 
            // label40
            // 
            this.label40.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label40.Location = new System.Drawing.Point(480, 13);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(112, 16);
            this.label40.TabIndex = 50;
            this.label40.Text = "Outstanding Balance";
            // 
            // label42
            // 
            this.label42.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label42.Location = new System.Drawing.Point(664, 13);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(80, 16);
            this.label42.TabIndex = 49;
            this.label42.Text = "Current Status";
            // 
            // txtArrears
            // 
            this.txtArrears.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtArrears.Location = new System.Drawing.Point(596, 32);
            this.txtArrears.Name = "txtArrears";
            this.txtArrears.ReadOnly = true;
            this.txtArrears.Size = new System.Drawing.Size(62, 20);
            this.txtArrears.TabIndex = 47;
            // 
            // txtOutstandingBalance
            // 
            this.txtOutstandingBalance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutstandingBalance.Location = new System.Drawing.Point(480, 32);
            this.txtOutstandingBalance.Name = "txtOutstandingBalance";
            this.txtOutstandingBalance.ReadOnly = true;
            this.txtOutstandingBalance.Size = new System.Drawing.Size(112, 20);
            this.txtOutstandingBalance.TabIndex = 46;
            // 
            // txtCurrentStatus
            // 
            this.txtCurrentStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCurrentStatus.Location = new System.Drawing.Point(664, 32);
            this.txtCurrentStatus.Name = "txtCurrentStatus";
            this.txtCurrentStatus.ReadOnly = true;
            this.txtCurrentStatus.Size = new System.Drawing.Size(90, 20);
            this.txtCurrentStatus.TabIndex = 48;
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.Location = new System.Drawing.Point(175, 32);
            this.txtName.MaxLength = 30;
            this.txtName.Name = "txtName";
            this.txtName.ReadOnly = true;
            this.txtName.Size = new System.Drawing.Size(183, 20);
            this.txtName.TabIndex = 44;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(175, 13);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 16);
            this.label4.TabIndex = 45;
            this.label4.Text = "Name";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(364, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 16);
            this.label1.TabIndex = 43;
            this.label1.Text = "Customer ID";
            // 
            // txtCustID
            // 
            this.txtCustID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCustID.Location = new System.Drawing.Point(364, 31);
            this.txtCustID.MaxLength = 20;
            this.txtCustID.Name = "txtCustID";
            this.txtCustID.ReadOnly = true;
            this.txtCustID.Size = new System.Drawing.Size(110, 20);
            this.txtCustID.TabIndex = 42;
            // 
            // txtAccountNo
            // 
            this.txtAccountNo.Location = new System.Drawing.Point(32, 32);
            this.txtAccountNo.Name = "txtAccountNo";
            this.txtAccountNo.PreventPaste = false;
            this.txtAccountNo.Size = new System.Drawing.Size(94, 20);
            this.txtAccountNo.TabIndex = 41;
            this.txtAccountNo.Text = "000-0000-0000-0";
            // 
            // lAuthorise
            // 
            this.lAuthorise.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.lAuthorise.Enabled = false;
            this.lAuthorise.Location = new System.Drawing.Point(184, 72);
            this.lAuthorise.Name = "lAuthorise";
            this.lAuthorise.Size = new System.Drawing.Size(16, 16);
            this.lAuthorise.TabIndex = 57;
            this.lAuthorise.Visible = false;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // GoodsReturn
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(787, 561);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "GoodsReturn";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Goods Return";
            ((System.ComponentModel.ISupportInitialize)(this.appUpdater1)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgDeliveredItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgGoodsToReturn)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private void InitData(bool initAcctNo)
        {
            if (initAcctNo) txtAccountNo.Text = "000-0000-0000-0";
            txtArrears.Text = "";
            txtOutstandingBalance.Text = "";
            txtCurrentStatus.Text = "";
            txtShowInvoiceNo.Text = "";
            //IP - 13/11/09 - UAT5.2 (653) - Created method ResetItemDetailFields() to reset item detail fields.
            //txtCreditTotal.Text = "";

            //txtTotalValue.Text = "";
            //txtQty.Text = "";
            //txtItemNo.Text = "";
            //txtLocn.Text = "";
            //txtReturnLocn.Text = "";
            //txtReturnItemNo.Text = "";
            ResetItemDetailFields();

            maxReturnQty = 0;
            txtName.Text = "";
            txtCustID.Text = "";
            dgGoodsToReturn.DataSource = null;
            dgDeliveredItems.DataSource = null;
            dgDeliveredItems.TableStyles.Clear();
            drpReposession.Text = "";
            drpCredReplace.Text = "";
            drpReposession.SelectedIndex = -1;
            drpReposession.Enabled = false;
            drpRepoType.SelectedIndex = -1;
            drpRepoType.Enabled = false;
            drpCredReplace.SelectedIndex = 0;
            txtInvoiceNo.Text = ""; //CR 2018-13 07Dec2018

            if (drpReposession.SelectedIndex == -1)      // jec 23/06/11 
            {
                drpCredReplace.Enabled = false;
            }
            else drpCredReplace.Enabled = true;
            //drpCredReplace.Enabled = true;       //IP - 13/11/09 - UAT5.2 (653)         
            // 69614 check to see if any reasons have been entered, if not then inform user
            if (drpReason.Items.Count > 0)
            {
                drpReason.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("No reasons have been entered in Code Maintenance");
            }
           // drpDeliveryArea.SelectedIndex = 0;
            tbNotes.Text = "";  //IP - 10/02/10 - CR1048 (Ref:3.1.4 & 3.1.5) Merged - Malaysia Enhancements (CR1072)           
            tab.Clear();
            //IP - 13/11/09 - UAT5.2 (653) Moved below btnEnter and btnRemove to method ResetItemDetailFields()
            //btnEnter.Enabled = false;
            //btnRemove.Enabled = false;
            errorProvider1.SetError(drpReposession, "");
            errorProvider1.SetError(drpRepoType, "");
            errorProvider1.SetError(drpCredReplace, "");
            errorProvider1.SetError(drpReason, "");
            errorProvider1.SetError(tbNotes, "");
            associatedItems.Clear();  //IP - 10/02/10 - CR1048 (Ref:3.1.4 & 3.1.5) Merged - Malaysia Enhancements (CR1072)
            // Enable buttons if Third Party not enabled
            if (Convert.ToBoolean(Country[CountryParameterNames.ThirdPartyDeliveriesWarehouse]) == false)
            {
                rbScheduling.Enabled = true;
                rbPickupImmed.Enabled = true;
            }

            warrantyPercentageReturn = 0; //#15993
            warrantyReturnPC.Clear();     //#15993 - clear dictionary
            drpCollectionAdr.Enabled= false;        // #14927
            drpCollectionAdr.DataSource=null;
            readyAssistCancellation = false;        //#18608 - CR15594
            loanAccount = false;
        }

        private void LoadStatic()
        {
            XmlUtilities xml = new XmlUtilities();
            XmlDocument dropDowns = new XmlDocument();
            dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

            if (StaticData.Tables[TN.GRTReason] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.GRTReason, new string[] { "RGR", "L" }));

            if (StaticData.Tables[TN.DeliveryArea] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.DeliveryArea, new string[] { "DDY", "L" }));

            if (StaticData.Tables[TN.CollectionType] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.CollectionType, new string[] { "COL", "L" }));

            if (StaticData.Tables[TN.Discounts] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.Discounts, new string[] { "PCDIS", "L" }));

            dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.WarrantyCategories, new string[] { CAT.Warranty, "L" }));

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

            drpReason.DataSource = (DataTable)StaticData.Tables[TN.GRTReason];
            drpReason.DisplayMember = CN.CodeDescription;

            drpCredReplace.DataSource = (DataTable)StaticData.Tables[TN.CollectionType];
            drpCredReplace.DisplayMember = CN.CodeDescription;

            LoadDeliveryArea(Config.BranchCode);
        }

        private void SetupStock(DataRowView stockRow)
        {
            txtQty.Enabled = true; //#17290

            txtItemNo.Text = (string)stockRow[CN.ItemNo];
            selectedItemId = (int)stockRow[CN.ItemId];          // RI
            txtReturnItemNo.Text = (string)stockRow[CN.ItemNo];
            txtReturnLocn.Text = ((short)stockRow[CN.StockLocn]).ToString();
            txtLocn.Text = ((short)stockRow[CN.StockLocn]).ToString();
            //CR2018-013 Added QuantityChanged flag to capture changed quantity from GRT screen
            if (CollectionType != "I" && QuantityChanged == false) //#17290 - Only allow to process one Instant Replacement at a time
            {
                txtQty.Text = ((double)stockRow[CN.QuantityDelivered]).ToString();
            }
            else if (CollectionType != "I" && QuantityChanged == true) 
            {
                txtQty.Text = txtQty.Text;
                QuantityChanged = false;
            }
            else
            {
                txtQty.Text = "1";
                txtQty.Enabled = false;
            }

            maxReturnQty = ((double)stockRow[CN.QuantityDelivered]);

            //if (Convert.ToString(drpReposession.SelectedItem) == "Y")
            if (Convert.ToString(drpReposession.SelectedItem) == "Y" && Convert.ToString(Country[CountryParameterNames.RIInterfaceOptions]) != "FACT") //IP - 08/09/11 - RI - #8112
            {
                price = Convert.ToDecimal(stockRow[CN.Price]) * Convert.ToDecimal(stockRow["RepoCostPcent"]) / 100
                            * (1 + Convert.ToDecimal(stockRow["RepoSellPcent"]) / 100);       // RI jec 06/07/11 
            }
            else
            {
                price = Convert.ToDecimal(stockRow[CN.Price]);
            }
            this.SetTotalValue(stockRow);

            txtQty.ReadOnly = false;

            if (Convert.ToString(Country[CountryParameterNames.RIInterfaceOptions]) == "FACT")    //IP - 08/10/11 - RI - #8112 - Changed to check === FACT from !=RI//IP - 08/09/11 - RI - #8112
            {
                txtReturnItemNo.ReadOnly = false;
                txtTotalValue.ReadOnly = false;
            }

            if (drpReposession.Enabled && Convert.ToString(drpReposession.SelectedItem) == "Y")
            {
                validRepossessionReturnItemNos.Clear();

                foreach (var item in RepossessedConditions)
                {
                    validRepossessionReturnItemNos.Add((string)stockRow[CN.ItemNo] + item.SKUSuffix);
                }
            }

            txtReturnLocn.ReadOnly = false;

            //txtReturnItemNo.ReadOnly = false;
            //txtTotalValue.ReadOnly = false;
            lItemNo.Visible = true;
            lWarrantyItemNo.Visible = false;
            btnEnter.Enabled = true;
        }

        private void SetupWarranty(DataRowView warrantyRow, bool parentCollected, bool isDiscount)  //IP - 14/06/11 - CR1212 - RI
        {
            string retCode = "";

            txtItemNo.Text = (string)warrantyRow[CN.ItemNo];
            selectedItemId = (int)warrantyRow[CN.ItemId];          // RI

            int warrantyRetCodeItemID = 0;                         //IP - 09/09/11 - RI - #8112

            //PostSetupWarranty(warrantyRow, parentCollected, isDiscount, retCode, warrantyRetCodeItemID);

            // 68666 16/10/07 rbd copied from v4 code
            if ((CollectionType == "C" || !drpCredReplace.Enabled) && !isDiscount && WarrantyType.IsFree(Convert.ToString(warrantyRow["WarrantyType"])) == false)  //#17883  //#15993    // #14995 only for warranties
            {
                //#15993 - Get Return Details from dictionary
                var details = new WarrantyReturnDetails();

                warrantyReturnPC.TryGetValue(new DictionaryKey(Convert.ToString(warrantyRow[CN.ItemNo]), Convert.ToString(warrantyRow[CN.ContractNo])), out details);

                    if (details != null)
                    {
                        retCode = details.warrantyReturn.Warranty != null ? details.warrantyReturn.Warranty.Number : details.warrantyReturn.WarrantyNo;     // #17506
                        //warrantyRetCodeItemID = response.warrantyReturn.Warranty.Id;
                        warrantyRetCodeItemID = Convert.ToInt32(warrantyRow[CN.ItemID]);   //#15073 - This was incorrectly set to ID of warranty from web.
                        warrantyPercentageReturn = details.warrantyReturn.PercentageReturn.Value;
                        deliveryDate = details.WarrantyDeliveredOn.Value;
                        PostSetupWarranty(warrantyRow, parentCollected, isDiscount, retCode, warrantyRetCodeItemID);
                    }
                    else
                    {
                        retCode = (string)warrantyRow[CN.ItemNo];
                        warrantyRetCodeItemID = Convert.ToInt32(warrantyRow[CN.ItemID]);   //#15073 - This was incorrectly set to ID of warranty from web.
                        warrantyPercentageReturn = 0;
                        deliveryDate = (DateTime)warrantyRow[CN.DateDel];
                        PostSetupWarranty(warrantyRow, parentCollected, isDiscount, retCode, warrantyRetCodeItemID);
                    }
            }
            else
                PostSetupWarranty(warrantyRow, parentCollected, isDiscount, retCode, warrantyRetCodeItemID);

        }

        private void PostSetupWarranty(DataRowView warrantyRow, bool parentCollected, bool isDiscount, string retCode, int warrantyRetCodeItemID)
        {
            if (Convert.ToString(Country[CountryParameterNames.RIInterfaceOptions]) != "FACT")         //IP - 08/09/11 - RI - #8112
            {
                retCode = txtItemNo.Text;               //IP - 06/09/11 - RI - #4534 - UAT31 - Warranty Return Code should be the same as original warranty item number
            }
            else
            {
                //Need to set the retitemid for the warranty   
                warrantyRow[CN.RetItemId] = warrantyRetCodeItemID;      //IP - 13/09/11 - RI - #8112 - set the RetItemID to the warranty return code
            }


            txtReturnItemNo.Text = retCode;
            
            if (!parentCollected)
                txtReturnLocn.Text = ((short)warrantyRow[CN.StockLocn]).ToString();

            txtLocn.Text = ((short)warrantyRow[CN.StockLocn]).ToString();

            if (isDiscount)             //IP - 14/06/11 - CR1212 - RI - If we are collecting a discount then the quantity will always be 1.
            {
                txtQty.Text = "1";
            }
            else
            {
                txtQty.Text = ((double)warrantyRow[CN.QuantityDelivered]).ToString();
            }

            maxReturnQty = ((double)warrantyRow[CN.QuantityDelivered]);
            
            
           price = Convert.ToDecimal(warrantyRow[CN.Price]);  
           
           
            this.SetTotalValue(warrantyRow);  //task

            if (Convert.ToString(Country[CountryParameterNames.RIInterfaceOptions]) == "FACT")        //IP - 08/09/11 - RI - #8112
            {
                txtReturnItemNo.ReadOnly = false;
                txtTotalValue.ReadOnly = false;
            }

            txtQty.ReadOnly = false;
            //txtReturnItemNo.ReadOnly = false;
            txtReturnLocn.ReadOnly = false;
            //txtTotalValue.ReadOnly = false;
            lItemNo.Visible = false;
            lWarrantyItemNo.Visible = true;
            btnEnter.Enabled = true;
        }

        private void SetupNonStock(DataRowView nonStockRow)
        {
            txtItemNo.Text = (string)nonStockRow[CN.ItemNo];
            selectedItemId = (int)nonStockRow[CN.ItemId];          // RI
            txtReturnItemNo.Text = (string)nonStockRow[CN.ItemNo];
            txtReturnLocn.Text = ((short)nonStockRow[CN.StockLocn]).ToString();
            txtLocn.Text = ((short)nonStockRow[CN.StockLocn]).ToString();
            txtQty.Text = ((double)nonStockRow[CN.QuantityDelivered]).ToString();
            maxReturnQty = ((double)nonStockRow[CN.QuantityDelivered]);
            price = Convert.ToDecimal(nonStockRow[CN.Price]);

            this.SetTotalValue(nonStockRow);

            if (Convert.ToString(Country[CountryParameterNames.RIInterfaceOptions]) == "FACT")        //IP - 08/09/11 - RI - #8112
            {
                txtReturnItemNo.ReadOnly = false;
                txtTotalValue.ReadOnly = false;
            }

            txtQty.ReadOnly = false;
            //txtReturnItemNo.ReadOnly = false;
            txtReturnLocn.ReadOnly = false;
            //txtTotalValue.ReadOnly = false;
            lItemNo.Visible = true;
            lWarrantyItemNo.Visible = false;
            btnEnter.Enabled = true;
        }

        private void SetTotalValue(DataRowView deliveryRow)
        {
            //int zeroValue = 0;
            decimal totalValue = 0;

            if (IsNumeric(txtQty.Text) && txtQty.Text.Trim() != "")
            {
                //if (Convert.ToString(drpReposession.SelectedItem) == "Y")
                if (Convert.ToString(drpReposession.SelectedItem) == "Y" && Convert.ToString(Country[CountryParameterNames.RIInterfaceOptions]) != "FACT")    //IP - 08/09/11 - RI - #8112
                {
                    totalValue = Convert.ToDecimal(txtQty.Text) * Convert.ToDecimal(deliveryRow[CN.Price]) * Convert.ToDecimal(deliveryRow["RepoCostPcent"]) / 100
                                * (1 + Convert.ToDecimal(deliveryRow["RepoSellPcent"]) / 100);  // RI jec 06/07/11 
                }
                else
                {
                    if (dgGoodsToReturn.DataSource != null)
                    {
                        foreach (DataRowView row in (DataView)dgGoodsToReturn.DataSource)
                        {
                            if (!IsWarranty(Convert.ToString(deliveryRow[CN.Category])))
                            {
                                totalValue = Convert.ToDecimal(txtQty.Text) * Convert.ToDecimal(deliveryRow[CN.Price]); //CR2018-013 
                            }
                            else if (IsWarranty(Convert.ToString(deliveryRow[CN.Category])))
                            {
                                totalValue = Convert.ToDecimal(txtQty.Text) * Convert.ToDecimal(deliveryRow[CN.Price]); //CR2018-013 
                            }
                            else
                            {
                                totalValue = 0;
                            }
                        }
                    }
                    else
                    {
                        totalValue = Convert.ToDecimal(txtQty.Text) * Convert.ToDecimal(deliveryRow[CN.Price]); 
                    }
                }
            }

            //CR1094 - RM 15/12/10 repossessions and cancellations use same return method
            txtTotalValue.Text = totalValue.ToString(DecimalPlaces);
            //if (ST.IsWarranty((short)deliveryRow[CN.Category]))
            //{
            //    if(drpCredReplace.Enabled && CollectionType == "C")
            //        txtTotalValue.Text = totalValue.ToString(DecimalPlaces);
            //    else
            //        txtTotalValue.Text = zeroValue.ToString(DecimalPlaces);
            //}
            //else
            //{
            //txtTotalValue.Text = totalValue.ToString(DecimalPlaces);
            //}
        }

        private void ProcessWarranties(int parentItemId, short parentLocation)               // RI
        {
            int itemIndex = 0;
            try
            {
                loaded = false;
                Function = "ProcessWarranties";
                bool valid = true;
                int rowCount = 0;
                int locn;
                int itemID;

                // Save the Stock row to restore after the warranties have been processed
                itemIndex = dgDeliveredItems.CurrentRowIndex;
                itemID = Convert.ToInt32(((DataView)dgDeliveredItems.DataSource)[itemIndex][CN.ItemId]);     // RI
                DataView dv = (DataView)dgDeliveredItems.DataSource;
                int count = dv.Count;
                dv.Sort = "WarrantyType ASC";       //#17883

                int warranties = 0;
                var itemReturnQuantity = Convert.ToInt32(txtQty.Text);
                // CR784 - if collection or reposession process warranty and place in
                // goods to return datagrid
                if (CollectionType == "C" || CollectionType == "E" || !drpCredReplace.Enabled)
                {
                    for (int i = count - 1; i >= 0; i--)                      
                    {
                        //CR2018-013 to loop through all items
                        foreach (string str in warrantiesToCollect)
                        {
                            //if(str == (string)dv[i][CN.ItemNo] && (string)dv[i][CN.ParentItemNo] == parentItemNo &&
                            if (str == Convert.ToString(dv[i][CN.ItemId]) && Convert.ToString(dv[i][CN.ParentItemId]) == Convert.ToString(parentItemId) &&          // RI 
                                (short)dv[i][CN.ParentLocation] == parentLocation)
                            {
                                //UAT 381 ElapsedMonths method calculates a date difference of between 12 and 13 months as 12. Therefore '>' changed to '>='
                                if (ElapsedMonths(Convert.ToDateTime(dv[i][CN.DateDel])) >= Convert.ToInt32(Country[CountryParameterNames.WarrantyValidity]))
                                {
                                    if (dgGoodsToReturn.DataSource != null)
                                    {
                                        foreach (DataRowView row in (DataView)dgGoodsToReturn.DataSource)
                                        {
                                            //if((string)row[CN.ItemNo] == parentItemNo &&  
                                            // if ((int)row[CN.ItemId] == parentItemId &&               // RI
                                            if (Convert.ToInt32(row[CN.ItemId]) == parentItemId &&              //CR2018-013 check for parent item
                                               Convert.ToInt16(row[CN.StockLocn]) == parentLocation)
                                            {
                                                row[CN.WarrantyFulfilled] = "Y";
                                            }
  
                                        }
                                    }
                                    //Convert.ToString(dv[i][CN.WarrantyFulfilled]) = "Y";
                                }
                                
                            }
                        }
                        //if (CollectionType != "E")                    //CR2018-013 Allow for Exchange
                        //{                            
                        maxReturnQty = 0;
                            var GroupId = 0;
                            var GroupCount = 0;

                            foreach (DataRow r in dsWarranties.Tables[TN.Warranties].Rows)
                            {
                                //Only select warranties for quantity of items collected.
                                if (GroupId != Convert.ToInt32(r["WarrantyGroupId"]))
                                {
                                    GroupCount++;
                                    GroupId = Convert.ToInt32(r["WarrantyGroupId"]);
                                }
                
                                //if ((string)r[CN.AssociatedItem] == (string)dv[i][CN.ItemNo] &&
                                if ((int)r[CN.AssociatedItemID] == (int)dv[i][CN.ItemId] &&       // RI
                                    (string)r[CN.ContractNo] == (string)dv[i][CN.ContractNo] &&
                                    //(string)dv[i][CN.ParentItemNo] == parentItemNo &&
                                    (int)dv[i][CN.ParentItemId] == parentItemId &&                   // RI
                                    (short)dv[i][CN.ParentLocation] == parentLocation &&
                                    GroupCount <= itemReturnQuantity) //#15993

                                {
                                    warranties++;
                                    dgDeliveredItems.CurrentRowIndex = i;
                                    ////this.SetupWarranty(dv[i], true, false);             //IP - 14/06/11 - CR1212 - added isDiscount check

                                    //#15993
                                    DataRowView warrantyRow = dv[i];
                                    var parentCollected = true;
                                    var isDiscount = false;

                                    string retCode = "";

                                    txtItemNo.Text = (string)warrantyRow[CN.ItemNo];
                                    selectedItemId = (int)warrantyRow[CN.ItemId];          // RI

                                    int warrantyRetCodeItemID = 0;                         //IP - 09/09/11 - RI - #8112
                  
                                    //PostSetupWarranty(warrantyRow, parentCollected, isDiscount, retCode, warrantyRetCodeItemID);

                                    if ((CollectionType == "C" || CollectionType == "E" || !drpCredReplace.Enabled) && !isDiscount) //#15993 //CR2018-013  Allow for Exchange type    // #14995 only for warranties
                                {

                                        if (WarrantyType.IsFree(Convert.ToString(r["WarrantyType"])) == false)      //#17883
                                        {
                                            dgDeliveredItems.CurrentRowIndex = i;
                                            dgDeliveredItems_MouseUp(null, new System.Windows.Forms.MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));

                                            //#15993 - Get Return Details from dictionary
                                            var details = new WarrantyReturnDetails();

                                            warrantyReturnPC.TryGetValue(new DictionaryKey(Convert.ToString(warrantyRow[CN.ItemNo]), Convert.ToString(warrantyRow[CN.ContractNo])), out details);

                                            if (details != null)
                                            {
                                                retCode = details.warrantyReturn.Warranty != null ? details.warrantyReturn.Warranty.Number : details.warrantyReturn.WarrantyNo;     // #17506
                                                //warrantyRetCodeItemID = response.warrantyReturn.Warranty.Id;
                                                warrantyRetCodeItemID = Convert.ToInt32(warrantyRow[CN.ItemID]);   //#15073 - This was incorrectly set to ID of warranty from web.
                                                warrantyPercentageReturn = details.warrantyReturn.PercentageReturn.Value;
                                                deliveryDate = details.WarrantyDeliveredOn.Value;
                                                PostSetupWarranty(warrantyRow, parentCollected, isDiscount, retCode, warrantyRetCodeItemID);
                                                SetupWarrantyReturns(ref valid, ref rowCount, dv, i, out locn, out itemID);  //#15993
                                            }
                                            else
                                            {
                                                retCode = (string)warrantyRow[CN.ItemNo];
                                                warrantyRetCodeItemID = Convert.ToInt32(warrantyRow[CN.ItemID]);   //#15073 - This was incorrectly set to ID of warranty from web.
                                                warrantyPercentageReturn = 0;    //#16399
                                                deliveryDate = (DateTime)warrantyRow[CN.DateDel];
                                                PostSetupWarranty(warrantyRow, parentCollected, isDiscount, retCode, warrantyRetCodeItemID);
                                                SetupWarrantyReturns(ref valid, ref rowCount, dv, i, out locn, out itemID);  //#15993
                                            }

                                        }
                                        else //Do free warranties
                                        {
                                            //dgDeliveredItems.CurrentRowIndex = i;
                                            dgDeliveredItems_MouseUp(null, new System.Windows.Forms.MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
                                            PostSetupWarranty(warrantyRow, parentCollected, isDiscount, retCode, warrantyRetCodeItemID);
                                            SetupWarrantyReturns(ref valid, ref rowCount, dv, i, out locn, out itemID);  //#15993
                                        }
                                    }
                                    else
                                        PostSetupWarranty(warrantyRow, parentCollected, isDiscount, retCode, warrantyRetCodeItemID);

                                    //#15993 - Moved code that was here into method : SetupWarrantyReturns
                                }
                            }
                        //}            

                        if (CollectionType != "")           //IP/JC - 16/01/12 - #8917 - LW74448 - If repossession then do not automatically add linked discount
                        {
                            foreach (DataRow r in dsWarranties.Tables[TN.Discounts].Rows)
                            {
                                //if ((string)r[CN.AssociatedItem] == (string)dv[i][CN.ItemNo] &&
                                if ((int)r[CN.AssociatedItemID] == (int)dv[i][CN.ItemId] &&     // RI
                                    (string)r[CN.ContractNo] == (string)dv[i][CN.ContractNo] &&
                                    //(string)dv[i][CN.ParentItemNo] == parentItemNo &&
                                    (int)dv[i][CN.ParentItemId] == parentItemId &&              // RI
                                    (short)dv[i][CN.ParentLocation] == parentLocation)
                                {
                                    warranties++;
                                    dgDeliveredItems.CurrentRowIndex = i;
                                    this.SetupWarranty(dv[i], true, true);                  //IP - 14/06/11 - CR1212 - added isDiscount check

                                    if (CollectionType == "R")
                                        txtReturnLocn.Text = parentReturnLocation;

                                    locn = Convert.ToInt32(txtLocn.Text);
                                    //rowCount = AccountManager.GetItemCount(txtItemNo.Text, (short)locn, out Error);
                                    rowCount = AccountManager.GetItemCount(itemID, (short)locn, out Error);     // RI
                                    if (Error.Length > 0)
                                        ShowError(Error);
                                    else
                                    {
                                        if (rowCount == 0)
                                        {
                                            ShowInfo("M_STOCKNOTPRESENT");
                                            valid = false;
                                        }
                                    }

                                    if (valid && txtReturnItemNo.Text.Length > 0)
                                    {
                                        locn = Convert.ToInt32(txtReturnLocn.Text);
                                        //rowCount = AccountManager.GetItemCount(txtReturnItemNo.Text, (short)locn, out Error);
                                        rowCount = AccountManager.GetItemCount(itemID, (short)locn, out Error);       // RI
                                        if (Error.Length > 0)
                                            ShowError(Error);
                                        else
                                        {
                                            if (rowCount == 0)
                                            {
                                                ShowInfo("M_WARRANTYNOTPRESENT");
                                                valid = false;
                                            }
                                        }
                                    }

                                    if (valid)
                                    {
                                        FillDeliveryDetails(true);
                                    }
                                }
                            }
                        }

                        if (warranties == warrantiesToCollect.Count)
                            break;
                    }
                }
                // CR784 - if replacement or exchange determine if the warranty has been
                // fulfilled, so that we know whether to remove the link to the 
                // parent item.  warranties will not be placed in the goods to return
                // datagrid for replacements & exchanges
  
                else if (CollectionType != "C"  && CollectionType != "E" && drpCredReplace.Enabled && warranties != warrantiesToCollect.Count)      //CR2018-013 
                {
                    if (CollectionType != "R" && CollectionType != "I")  //#17678
                    {
                      ((DataView)dgDeliveredItems.DataSource).RowFilter = "";
                    }
              
                    for (int i = dv.Count - 1; i >= 0; i--)
                    {
                        foreach (string str in warrantiesToCollect)
                        {
                            //if(str == (string)dv[i][CN.ItemNo] && (string)dv[i][CN.ParentItemNo] == parentItemNo &&
                            if (str == Convert.ToString(dv[i][CN.ItemId]) && Convert.ToString(dv[i][CN.ParentItemId]) == Convert.ToString(parentItemId) &&          // RI 
                                (short)dv[i][CN.ParentLocation] == parentLocation)
                            {
                                //UAT 381 ElapsedMonths method calculates a date difference of between 12 and 13 months as 12. Therefore '>' changed to '>='
                                if (ElapsedMonths(Convert.ToDateTime(dv[i][CN.DateDel])) >= Convert.ToInt32(Country[CountryParameterNames.WarrantyValidity]))
                                {
                                    if (dgGoodsToReturn.DataSource != null)
                                    {
                                        foreach (DataRowView row in (DataView)dgGoodsToReturn.DataSource)
                                        {
                                            //if((string)row[CN.ItemNo] == parentItemNo &&
                                            if ((int)row[CN.ItemId] == parentItemId &&               // RI
                                                Convert.ToInt16(row[CN.StockLocn]) == parentLocation)
                                                row[CN.WarrantyFulfilled] = "Y";
                                        }
                                    }
                                }
                                else //CR2018-013 Flag if not 'Y' must be 'N'
                                {
                                    if (dgGoodsToReturn.DataSource != null)
                                    {
                                        foreach (DataRowView row in (DataView)dgGoodsToReturn.DataSource)
                                        {
                                            
                                            if (Convert.ToInt32(row[CN.ItemId]) == parentItemId &&             
                                               Convert.ToInt16(row[CN.StockLocn]) == parentLocation)
                                            {
                                                row[CN.WarrantyFulfilled] = "N";
                                            }



                                        }
                                    }
                                }
                            }
                        }
                    }
                    //((DataView)dgDeliveredItems.DataSource).RowFilter = CN.Category + " not in (12,82)";
                    if (CollectionType != "R" && CollectionType != "I") //#18575
                    {
                        ((DataView)dgDeliveredItems.DataSource).RowFilter = "Warranty" + " <> 1";       //RI Warranty item = 1
                    }
                   
                }

                loaded = true;
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                Function += Finished;
                // Restore the last user clicked row
                dgDeliveredItems.CurrentRowIndex = itemIndex;
                dgDeliveredItems_MouseUp(null, new System.Windows.Forms.MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
            }
        }

        //#15993
        private void SetupWarrantyReturns(ref bool valid, ref int rowCount, DataView dv, int i, out int locn, out int itemID)
        {
            if (CollectionType == "R")
                txtReturnLocn.Text = parentReturnLocation;

            locn = Convert.ToInt32(txtLocn.Text);
            itemID = Convert.ToInt32(dv[i][CN.ItemId]);

            //rowCount = AccountManager.GetItemCount(txtItemNo.Text, (short)locn, out Error);         
            rowCount = AccountManager.GetItemCount(itemID, (short)locn, out Error);         // RI
            if (Error.Length > 0)
                ShowError(Error);
            else
            {
                if (rowCount == 0)
                {
                    ShowInfo("M_STOCKNOTPRESENT");
                    valid = false;
                }
            }

            if (valid && txtReturnItemNo.Text.Length > 0)
            {
                locn = Convert.ToInt32(txtReturnLocn.Text);
                //rowCount = AccountManager.GetItemCount(txtReturnItemNo.Text, (short)locn, out Error);
                rowCount = AccountManager.GetItemCount(itemID, (short)locn, out Error);       // RI
                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    if (rowCount == 0)
                    {
                        ShowInfo("M_WARRANTYNOTPRESENT");
                        valid = false;
                    }
                }
            }

            if (valid)
            {
                FillDeliveryDetails(true);
            }
        }

        private void FillDeliveryDetails(bool parentCollected)
        {
            loaded = false;
            bool valid = true;
            int index = dgDeliveredItems.CurrentRowIndex;
            //int itemId;
            //itemId = Convert.ToInt32(((DataView)dgDeliveredItems.DataSource)[index][CN.ItemId]);     // RI



            if (dgGoodsToReturn.DataSource != null)              //TODO RI
            {
                foreach (DataRowView row in (DataView)dgGoodsToReturn.DataSource)
                {
                    //if( ( (string)row[CN.ItemNo] == txtItemNo.Text &&
                    if ((Convert.ToInt32(row[CN.ItemId]) == selectedItemId &&           // RI
                        (string)row[CN.ContractNo] == (string)((DataView)dgDeliveredItems.DataSource)[index][CN.ContractNo] &&
                        Convert.ToInt16(row[CN.StockLocn]) == Convert.ToInt16(txtLocn.Text) &&
                        //(string)row[CN.ParentItemNo] == (string)((DataView)dgDeliveredItems.DataSource)[index][CN.ParentItemNo]) ||
                           Convert.ToInt32(row[CN.ParentItemId]) == Convert.ToInt32(((DataView)dgDeliveredItems.DataSource)[index][CN.ParentItemId])) ||          // RI
                        //( (string)row[CN.ParentItemNo] == txtItemNo.Text) &&
                        //   Convert.ToInt16(row[CN.ParentLocation]) == Convert.ToInt16(txtLocn.Text) &&
                        (Convert.ToInt32(row[CN.ParentItemId]) == selectedItemId) &&                        // RI
                           Convert.ToInt16(row[CN.ParentLocation]) == Convert.ToInt16(txtLocn.Text) &&
                           Convert.ToBoolean(Convert.ToInt16(row[CN.WarrantyCollection])))
                        valid = false;
                }
            }

            var category = Convert.ToString(((DataView)dgDeliveredItems.DataSource)[index][CN.Category]);

            if (!IsWarranty(category) && txtReturnItemNo.Text == string.Empty)
                txtReturnItemNo.Text = txtItemNo.Text;

            collectWarrantyOnly = false;
            if (valid)
            {
                //if(ST.IsWarranty(category) && CollectionType == "C" && !parentCollected)

                //CR1094 - RM 15/12/10 collection and repossession use the same method

                if (IsWarranty(category) && !parentCollected)
                {
                    // CR784 - if a warranty is being cancelled on it's own then we need
                    // confirm with the user that the parent item is not being
                    // collected.  this makes it easier to collect the warranty
                    // immediately.
                    string parentItem = (string)((DataView)dgDeliveredItems.DataSource)[index][CN.ParentItemNo];

                    if (parentItem == string.Empty)
                    {
                        collectWarrantyOnly = true;
                        TimeSpan ts = DateTime.Now - Convert.ToDateTime(((DataView)dgDeliveredItems.DataSource)[index][CN.DateDel]);
                        if (ts.Days > Convert.ToInt32(Country[CountryParameterNames.WarrantyCancelDays]))
                        {
                            AuthorisePrompt ap = new AuthorisePrompt(this, lAuthorise, GetResource("M_CANCELAUTH"));
                            ap.ShowDialog();
                            if (!ap.Authorised)
                                valid = false;
                        }
                    }
                    else
                    {
                        if ((DialogResult.Yes == ShowInfo("M_CONFIRMWARRANTY", new object[] { parentItem }, MessageBoxButtons.YesNo)))
                        {
                            collectWarrantyOnly = true;
                            TimeSpan ts = DateTime.Now - Convert.ToDateTime(((DataView)dgDeliveredItems.DataSource)[index][CN.DateDel]);
                            if (ts.Days > Convert.ToInt32(Country[CountryParameterNames.WarrantyCancelDays]))
                            {
                                AuthorisePrompt ap = new AuthorisePrompt(this, lAuthorise, GetResource("M_CANCELAUTH"));
                                ap.ShowDialog();
                                if (!ap.Authorised)
                                    valid = false;
                            }
                        }
                        else
                        {
                            collectWarrantyOnly = false;
                            valid = false;
                        }
                    }

                }
            }

            if (valid)
            {
                bool refund = false;
                DataRow newRow;
                //tab.Clear();	
                newRow = tab.NewRow();

                decimal itemValue = (decimal)((DataView)dgDeliveredItems.DataSource)[index][CN.OrdVal];

                newRow[CN.Quantity] = txtQty.Text;
                newRow[CN.ItemNo] = txtItemNo.Text;                                                   
               
                newRow[CN.ItemId] = (int)((DataView)dgDeliveredItems.DataSource)[index][CN.ItemId];        // RI jec
                newRow[CN.RetItemNo] = txtReturnItemNo.Text;

                //if (Convert.ToString(drpReposession.SelectedItem) != "Y")
                //{
                //    newRow[CN.RetItemId] = (int)((DataView)dgDeliveredItems.DataSource)[index][CN.ItemId];        // RI jec
                //}
                //else
                //{
                //    newRow[CN.RetItemId] = (int)((DataView)dgDeliveredItems.DataSource)[index][CN.RetItemId];
                //}

                if (Convert.ToString(drpReposession.SelectedItem) != "Y")
                {
                    if (Convert.ToString(Country[CountryParameterNames.RIInterfaceOptions]) != "FACT")      //IP - 01/10/12 - #10501 - LW75161
                    {
                        newRow[CN.RetItemId] = (int)((DataView)dgDeliveredItems.DataSource)[index][CN.ItemId];        // RI jec
                    }
                    else
                    {
                        newRow[CN.RetItemId] = (int)((DataView)dgDeliveredItems.DataSource)[index][CN.RetItemId];        // RI jec
                    }
                }
                else
                {
                    newRow[CN.RetItemId] = (int)((DataView)dgDeliveredItems.DataSource)[index][CN.RetItemId];
                }

                newRow[CN.StockLocn] = txtLocn.Text;
                newRow[CN.RetStockLocn] = txtReturnLocn.Text;
                
              
                
                newRow[CN.Type] = CollectionType;
                foreach (DataRow row in ((DataTable)StaticData.Tables[TN.Discounts]).Rows)
                {
                    if (category == Convert.ToString(row[CN.Code]))
                    {
                        newRow[CN.Type] = "C";
                        break;
                    }
                }

                newRow[CN.ContractNo] = (string)((DataView)dgDeliveredItems.DataSource)[index][CN.ContractNo];
                newRow[CN.ItemType] = (string)((DataView)dgDeliveredItems.DataSource)[index][CN.ItemType];
                newRow[CN.Category] = category;
                newRow[CN.AgreementNo] = (int)((DataView)dgDeliveredItems.DataSource)[index][CN.AgrmtNo];
                newRow[CN.Reason] = (string)((DataRowView)drpReason.SelectedItem)[CN.CodeDescription];
                newRow[CN.ParentItemNo] = (string)((DataView)dgDeliveredItems.DataSource)[index][CN.ParentItemNo];
                newRow[CN.ParentItemId] = (int)((DataView)dgDeliveredItems.DataSource)[index][CN.ParentItemId];      // RI jec
                newRow[CN.ParentLocation] = (short)((DataView)dgDeliveredItems.DataSource)[index][CN.ParentLocation];
                newRow[CN.TaxAmt] = (double)((DataView)dgDeliveredItems.DataSource)[index][CN.TaxAmt];
                //newRow[CN.DeliveryArea] = drpDeliveryArea.SelectedIndex == 0 ? "" : drpDeliveryArea.SelectedValue.ToString();
                newRow[CN.DeliveryArea] = "";         // #14927
                newRow[CN.DeliveryAddress] = drpCollectionAdr.SelectedIndex < 0 ? "" : drpCollectionAdr.SelectedValue.ToString();      // #14927
                newRow[CN.DeliveryProcess] = rbScheduling.Checked ? "S" : "I";
                newRow[CN.Value] = itemValue;
                newRow[CN.RetVal] = txtTotalValue.Text;
                newRow[CN.OrdVal] = txtTotalValue.Text;


                newRow[CN.WarrantyFulfilled] = "N";   
                newRow[CN.WarrantyCollection] = Convert.ToInt16(collectWarrantyOnly);
                newRow[CN.ReadOnly] = "N";
                newRow[CN.LineItemId] = Convert.ToInt32(((DataView)dgDeliveredItems.DataSource)[index][CN.LineItemId]);
                newRow[CN.Price] = (decimal)((DataView)dgDeliveredItems.DataSource)[index][CN.Price];  //#12842
                newRow["DateDel"] = (DateTime)((DataView)dgDeliveredItems.DataSource)[index]["DateDel"];  //#17290
                newRow["ReadyAssist"] = Convert.ToBoolean(((DataView)dgDeliveredItems.DataSource)[index]["ReadyAssist"]);
                newRow["ReadyAssistUsed"] = (decimal)((DataView)dgDeliveredItems.DataSource)[index]["ReadyAssistUsed"];
                newRow["AnnualServiceContract"] = Convert.ToBoolean(((DataView)dgDeliveredItems.DataSource)[index]["AnnualServiceContract"]);
                newRow["AnnualServiceContractUsed"] = (decimal)((DataView)dgDeliveredItems.DataSource)[index]["AnnualServiceContractUsed"];


                // CR784 - the unexpired portion of the warranty needs to be calculated
                // when processing a cancellation or repossession on a warranty.
                if (IsWarranty(category))
                {
                    double warrantyTaxAmount = (double)((DataView)dgDeliveredItems.DataSource)[index][CN.TaxAmt];

                    //if (CollectionType == "C" || drpRepoType.Enabled)
                    refund = PayExpiredPortion();

                    if ((refund || (Convert.ToString(drpReposession.SelectedItem) == "Y"
                        && warrantyPercentageReturn > 0                        
                        && warrantyPercentageReturn <= 100))                // #16400 
                        &&Convert.ToDecimal(((DataView)dgDeliveredItems.DataSource)[index][CN.OrdVal]) >0)      // #16400 order value gt zero
                    {
                        if ((string)Country[CountryParameterNames.AgreementTaxType] == "E")
                        {
                            newRow[CN.Refund] = Math.Round(Convert.ToDecimal(((itemValue + (decimal)warrantyTaxAmount) * warrantyPercentageReturn) / 100), 2);
                        }
                        else
                        {
                           
                                newRow[CN.Refund] = Math.Round(Convert.ToDecimal((itemValue * warrantyPercentageReturn) / 100), 2);
                          
                            
                        }
                        
                        //newRow[CN.RefundType] = txtItemNo.Text.Substring(0,2) == "XW" ? "F" : "E";
                        //newRow[CN.RefundType] = Convert.ToString(((DataView)dgDeliveredItems.DataSource)[index][CN.CourtsCode]) == "XW" ? "F" : "E";  //IP - 21/06/11 - CR1212 - RI - #3939
                        newRow[CN.RefundType] = Convert.ToString(((DataView)dgDeliveredItems.DataSource)[index][CN.Department]) == "PCF" ? "F" : "E";   //IP - 21/07/11 - RI - #3939
                    }
                    else
                    {
                        newRow[CN.Refund] = 0;
                        newRow[CN.RefundType] = "";
                      
                       
                    }
                }
                else
                {
                    newRow[CN.Refund] = 0;
                    newRow[CN.RefundType] = "";
                }

                tab.Rows.Add(newRow);

                dv = new DataView(tab);

                dgGoodsToReturn.DataSource = dv;

                dv.AllowNew = false;
                dv.AllowEdit = true;

                if (dgGoodsToReturn.TableStyles.Count == 0)
                {
                    DataGridTableStyle tabStyle = new DataGridTableStyle();
                    tabStyle.MappingName = tab.TableName;
                    //dgGoodsToReturn.TableStyles.Add(tabStyle);

                    bool canEditReturnItemNo = (bool)Country[CountryParameterNames.AutoReturnCodes];

                    AddColumnStyle(CN.ReadOnly, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.Quantity, tabStyle, 50, true, GetResource("T_QUANTITY"), "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.ItemNo, tabStyle, 60, true, GetResource("T_ITEMNO"), "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.RetItemNo, tabStyle, 70, canEditReturnItemNo, GetResource("T_RETITEM"), "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.StockLocn, tabStyle, 100, true, GetResource("T_STOCKLOCN"), "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.RetStockLocn, tabStyle, 40, true, GetResource("T_RETLOCN"), "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.OrdVal, tabStyle, 70, true, GetResource("T_VALUE"), "", HorizontalAlignment.Left);

                    AddColumnStyle(CN.RetVal, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.Type, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.ContractNo, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.ItemType, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.AgreementNo, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.Reason, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.ParentItemNo, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.ParentLocation, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.Category, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.TaxAmt, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.DeliveryArea, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.DeliveryProcess, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.Value, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.WarrantyFulfilled, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.Refund, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.RefundType, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.WarrantyCollection, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.Price, tabStyle, 70, true, "Price", "", HorizontalAlignment.Left);       //#12842
                    tabStyle.GridColumnStyles[CN.Price].Width = 0; //#12842

                    dgGoodsToReturn.TableStyles.Add(tabStyle);

                    //dv.Table.ColumnChanged += new DataColumnChangeEventHandler(this.OnColumnChange);
                }

                decimal total = Convert.ToDecimal(StripCurrency(txtCreditTotal.Text)) + Convert.ToDecimal(StripCurrency(txtTotalValue.Text));
                txtCreditTotal.Text = total.ToString(DecimalPlaces);
            }

            loaded = true;
        }

        private void DispalyInvoiceNumber(DataTable dt)
        {
            if(dt.Rows.Count>0)
            {
                // txtShowInvoiceNo.Text = Convert.ToString(dt.Rows[0]["AgreementInvoiceNumber"]); 
                if(!DBNull.Value.Equals(dt.Rows[0]["AgreementInvoiceNumber"]))
                {
                    txtShowInvoiceNo.Text = Convert.ToString(dt.Rows[0]["AgreementInvoiceNumber"]).Insert(3, "-");
                }
                
                
                txtInvoiceNo.Text = Convert.ToString(dt.Rows[0]["AgreementInvoiceNumber"]);
            }
        }
            private void ProcessAccountDetails(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                if (DBNull.Value != row["Outstanding Balance"])
                    txtOutstandingBalance.Text = ((decimal)row["Outstanding Balance"]).ToString(DecimalPlaces);
                else
                    txtOutstandingBalance.Text = (0).ToString(DecimalPlaces);

                if (DBNull.Value != row["Arrears"])
                    txtArrears.Text = ((decimal)row["Arrears"]).ToString(DecimalPlaces);
                else
                {
                    txtArrears.Text = (0).ToString(DecimalPlaces);
                    txtArrears.ForeColor = Color.Red;
                }

                if (DBNull.Value != row["Agreement Total"])
                    agrmtTotal = ((decimal)row["Agreement Total"]);

                if (DBNull.Value != row["Current Status"])
                    txtCurrentStatus.Text = (string)row["Current Status"];

                //	if(DBNull.Value!=row["Customer ID"])
                //		txtCustID.Text = ((string)row["Customer ID"]);

                if (DBNull.Value != row["Account Type"])
                    acctType = ((string)row["Account Type"]);

                bool canRepo = (acctType != AT.Cash);
                this.drpReposession.Enabled = canRepo;
                //this.drpRepoType.Enabled = canRepo;           // jec 23/06/11
                // Only populate name/Cust Id if Main a/c holder  (jec 68080)
                if (DBNull.Value != row["Customer ID"])
                {
                    if ((string)row["Holder or Joint"] == Holder.Main)
                    {
                        txtName.Text = (string)row["First Name"] + " " + (string)row["Last Name"];
                        txtCustID.Text = ((string)row["Customer ID"]);
                    }
                }

                // Returns are not allowed on cash Loan accounts  LW72938 jec 25/11/10
                if ((bool)row["IsLoan"] == true)
                {
                    loanAccount = true;
                }

                txtCreditTotal.Text = (0).ToString(DecimalPlaces);
            }
        }

        private void ProcessFintrans(DataTable dt, ref decimal repoTotal)
        {
            foreach (DataRow row in dt.Rows)
            {
                repoTotal += (decimal)row[CN.TransValue];
            }
        }

        private void ProcessDelivery(string acctno)
        {
            loaded = false;
            int agreementNo = 1;
            //decimal orderValue = 0;

            DataSet itemSet = new DataSet();
            DataTable delTab = new DataTable("Deliveries");

            delTab.Columns.Add(CN.AcctNo);
            delTab.Columns.Add(CN.AgrmtNo);
            delTab.Columns.Add(CN.BuffBranchNo);
            delTab.Columns.Add(CN.DelOrColl);
            delTab.Columns.Add(CN.ItemNo);
            delTab.Columns.Add(CN.StockLocn);
            delTab.Columns.Add(CN.DateDel);
            delTab.Columns.Add(CN.DateTrans);
            delTab.Columns.Add(CN.TransValue);
            delTab.Columns.Add(CN.Quantity);
            delTab.Columns.Add(CN.RetVal);
            delTab.Columns.Add(CN.RetItemNo);
            delTab.Columns.Add(CN.RetStockLocn);
            delTab.Columns.Add(CN.ContractNo);
            delTab.Columns.Add(CN.Reason);
            delTab.Columns.Add(CN.DeliveryArea);
            delTab.Columns.Add(CN.DeliveryProcess);
            delTab.Columns.Add(CN.Refund);
            delTab.Columns.Add(CN.Type);
            delTab.Columns.Add(CN.CollectionType);    //CR36
            //uat363 rdb add parentitemno
            delTab.Columns.Add(CN.ParentItemNo);
            delTab.Columns.Add(CN.ItemId);             //IP - 25/05/11 - CR1212 - RI
            delTab.Columns.Add(CN.ParentItemId);       //IP - 25/05/11 - CR1212 - RI
            delTab.Columns.Add(CN.RetItemId);
            delTab.Columns.Add(CN.ItemType);
            delTab.Columns.Add(CN.Price);               //#12842
            delTab.Columns.Add("ReadyAssist");         // #18610 - CR15594
            delTab.Columns.Add("ReadyAssistUsed");     // #18610 - CR15594
            delTab.Columns.Add("AnnualServiceContract");         
            delTab.Columns.Add("AnnualServiceContractUsed");    

            int index = dgDeliveredItems.CurrentRowIndex;
            if (index >= 0)
                agreementNo = Convert.ToInt32(((DataView)dgDeliveredItems.DataSource)[index][CN.AgrmtNo]);

            foreach (DataRowView row in (DataView)dgGoodsToReturn.DataSource)
            {
                DataRow newRow;
                newRow = delTab.NewRow();

                newRow[CN.AcctNo] = acctno;
                newRow[CN.AgrmtNo] = agreementNo;
                newRow[CN.BuffBranchNo] = Convert.ToInt32(Config.BranchCode);
                newRow[CN.DelOrColl] = "R";
                newRow[CN.ItemNo] = (string)row[CN.ItemNo];
                newRow[CN.StockLocn] = (string)row[CN.StockLocn];
                newRow[CN.DateDel] = DateTime.Today;
                newRow[CN.DateTrans] = DateTime.Now; // IP - 18/09/09 - UAT5.2 UAT(877) Changed to DateTime.Now from DateTime.Today.
                newRow[CN.TransValue] = 0 - Convert.ToDecimal(StripCurrency((string)row[CN.OrdVal]));
                newRow[CN.Quantity] = 0 - Convert.ToDecimal(row[CN.Quantity]);
                newRow[CN.RetVal] = 0 - Convert.ToDecimal(StripCurrency((string)row[CN.RetVal]));
                newRow[CN.RetItemNo] = (string)row[CN.RetItemNo];
                newRow[CN.RetStockLocn] = (string)row[CN.RetStockLocn];
                newRow[CN.ContractNo] = (string)row[CN.ContractNo];
                newRow[CN.Reason] = (string)((DataRowView)drpReason.SelectedItem)[CN.CodeDescription];
                //newRow[CN.DeliveryArea] = drpDeliveryArea.SelectedIndex == 0 ? "" : drpDeliveryArea.SelectedValue.ToString();
                newRow[CN.DeliveryProcess] = rbScheduling.Checked ? "S" : "I";
                newRow[CN.Refund] = Convert.ToDecimal(StripCurrency((string)row[CN.Refund]));
                newRow[CN.Type] = (string)row[CN.RefundType];
                newRow[CN.CollectionType] = CollectionType;
                newRow[CN.ParentItemNo] = row[CN.ParentItemNo].ToString();
                newRow[CN.ItemId] = Convert.ToInt32(row[CN.ItemId]);            //IP - 25/05/11 - CR1212 - RI
                newRow[CN.ParentItemId] = Convert.ToInt32(row[CN.ParentItemId]);  //IP - 25/05/11 - CR1212 - RI
                newRow[CN.RetItemId] = Convert.ToInt32(row[CN.RetItemId]);  //IP - 25/05/11 - CR1212 - RI
                newRow[CN.ItemType] = Convert.ToString(row[CN.ItemType]);
                newRow[CN.Price] = Convert.ToDecimal(row[CN.Price]);        //#12842
                newRow["ReadyAssist"] = Convert.ToString(row["ReadyAssist"]);                   //#18610 - CR15594
                newRow["ReadyAssistUsed"] = Convert.ToDecimal(row["ReadyAssistUsed"]);          //#18610 - CR15594
                newRow["AnnualServiceContract"] = Convert.ToString(row["AnnualServiceContract"]);           
                newRow["AnnualServiceContractUsed"] = Convert.ToDecimal(row["AnnualServiceContractUsed"]);   

                delTab.Rows.Add(newRow);
            }

            itemSet.Tables.Add(delTab);

            AccountManager.SaveRepoDetails("REP", 0, txtAccountNo.Text.Replace("-", ""),
                Convert.ToDecimal(StripCurrency(txtOutstandingBalance.Text)),
                Convert.ToInt32(Config.BranchCode), Credential.UserId,
                Config.CountryCode, itemSet, accountType, out Error);

            if (Error.Length > 0) ShowError(Error);
            loaded = true;
        }

        private bool ProcessSchedule(string acctno)
        {
            decimal returnPrice = 0;
            int agreementNo = 1;
            DataSet itemSet = new DataSet();
            DataTable schedTab = new DataTable("Schedules");

            // 5.1 uat143 rdb 19/11/07 return a bool to indicate we dont want to clear screen
            // currently only when warning that a discount item has not been collected
            bool _success = true;

            schedTab.Columns.Add(CN.AcctNo);
            schedTab.Columns.Add(CN.AgrmtNo, Type.GetType("System.Int32"));
            schedTab.Columns.Add(CN.BuffNo);
            schedTab.Columns.Add(CN.DelOrColl);
            schedTab.Columns.Add(CN.ItemNo);
            schedTab.Columns.Add(CN.ItemId);                                    //IP - 17/05/11 - CR1212 - #3627 - Added ItemID
            schedTab.Columns.Add(CN.StockLocn);
            schedTab.Columns.Add(CN.Quantity);
            schedTab.Columns.Add(CN.RetItemNo);
            schedTab.Columns.Add(CN.RetItemId);                                    //RI jec 20/05/11
            schedTab.Columns.Add(CN.RetStockLocn);
            schedTab.Columns.Add(CN.DateDelPlan);
            schedTab.Columns.Add(CN.BuffBranchNo);
            schedTab.Columns.Add(CN.RetVal);
            schedTab.Columns.Add(CN.EmployeeNo);
            schedTab.Columns.Add(CN.ContractNo);
            schedTab.Columns.Add(CN.ItemType);
            schedTab.Columns.Add(CN.Reason);
            schedTab.Columns.Add(CN.DeliveryArea);
            schedTab.Columns.Add(CN.DeliveryAddress);               // #14927   
            schedTab.Columns.Add(CN.DeliveryProcess);
            schedTab.Columns.Add(CN.WarrantyFulfilled);
            schedTab.Columns.Add(CN.Refund);
            schedTab.Columns.Add(CN.RefundType);
            schedTab.Columns.Add(CN.TaxAmt, Type.GetType("System.Double"));
            schedTab.Columns.Add(CN.CollectionType);    //CR36
            //uat363 rdb add ParentItemNo
            schedTab.Columns.Add(CN.ParentItemNo);
            schedTab.Columns.Add(CN.ParentItemId);                              //IP - 17/05/11 - CR1212 - #3627 - Added ParentItemID
            schedTab.Columns.Add(CN.Notes);  //CR1048 jec  //IP - 10/02/10 - CR1048 (Ref:3.1.4 & 3.1.5) Merged - Malaysia Enhancements (CR1072)
            schedTab.Columns.Add(CN.LineItemId);  // #10441 jec 
            schedTab.Columns.Add(CN.Price); //#12842
            schedTab.Columns.Add(CN.Discount); //#18554 
            schedTab.Columns.Add("AnnualServiceContract");  
            schedTab.Columns.Add("AnnualServiceContractUsed");  


            DataTable nonStockList = new DataTable(TN.NonStockList);
            nonStockList.Columns.Add(CN.ItemNo);
            nonStockList.Columns.Add(CN.ItemId);                                //IP - 17/05/11 - CR1212 - #3627 - Added ItemId
            nonStockList.Columns.Add(CN.StockLocn, Type.GetType("System.Int16"));
            nonStockList.Columns.Add(CN.ContractNo);
            nonStockList.Columns.Add(CN.OrdVal, Type.GetType("System.Double"));
            nonStockList.Columns.Add(CN.TaxAmt, Type.GetType("System.Double"));
            nonStockList.Columns.Add(CN.DelOrColl);
            nonStockList.Columns.Add(CN.RetItemNo);
            nonStockList.Columns.Add(CN.RetItemId);                                    //RI jec 20/05/11
            nonStockList.Columns.Add(CN.EmployeeNo);
            nonStockList.Columns.Add(CN.RetStockLocn);
            nonStockList.Columns.Add(CN.Refund);
            nonStockList.Columns.Add(CN.RefundType);
            nonStockList.Columns.Add(CN.CollectionType);    //CR36
            //uat363 rdb add ParentItemNo
            nonStockList.Columns.Add(CN.ParentItemNo);
            nonStockList.Columns.Add(CN.ParentItemId);                          //IP - 17/05/11 - CR1212 - #3627 - Added ParentItemId
            nonStockList.Columns.Add(CN.Notes);  //CR1048 jec (Ref:3.1.4 & 3.1.5) Merged - Malaysia Enhancements (CR1072)
            nonStockList.Columns.Add("ReadyAssist");        //#18607 - CR15594
            nonStockList.Columns.Add("ReadyAssistUsed");     //#18607 - CR15594
            nonStockList.Columns.Add("AnnualServiceContract");       
            nonStockList.Columns.Add("AnnualServiceContractUsed");     


            int index = dgDeliveredItems.CurrentRowIndex;
            if (index >= 0)
                agreementNo = Convert.ToInt32(((DataView)dgDeliveredItems.DataSource)[index][CN.AgrmtNo]);

            bool isDotNetWarehouse = AccountManager.IsDotNetWarehouse(Convert.ToInt16(Config.BranchCode), out Error);
            if (Error.Length > 0)
                ShowError(Error);
            else
            {
                foreach (DataRowView row in (DataView)dgGoodsToReturn.DataSource)
                {
                    string type = "";

                    if (row[CN.Type] == DBNull.Value || ((string)row[CN.Type]).Length == 0)
                        type = "C";
                    else
                        type = (string)row[CN.Type];

                    //if (Convert.ToString(row[CN.ItemType]) == Stock || 
                    //    (IsWarranty(Convert.ToString(row[CN.Category])) && !Convert.ToBoolean(row[CN.WarrantyCollection])))

                    if (Convert.ToString(row[CN.ItemType]) == Stock || IsWarranty(Convert.ToString(row[CN.Category])) && !Convert.ToBoolean(Convert.ToInt32(row[CN.WarrantyCollection]))
                        || (IsDiscount(Convert.ToInt16(row[CN.Category])) && Convert.ToString(row[CN.ParentItemNo]) != "")  // if discount with a parent item also schedule
                        || (IsInstallation(Convert.ToInt16(row[CN.Category])) && Convert.ToString(row[CN.ParentItemNo]) != "")  //IP/JC - 12/01/11 - #9440 - if installation with a parent item also schedule
                        || (Convert.ToBoolean(row["AnnualServiceContract"]) == true && Convert.ToString(row[CN.ParentItemNo]) != "" && IsItemInReturnDetails(int.Parse(row["ParentItemId"].ToString())))) //Annual Service Contract with parent item
                    {
                        // Add a stock item & associated warranty to the schedule //check
                        DataRow newRow;
                        newRow = schedTab.NewRow();
                        newRow[CN.Quantity] = 0 - Convert.ToDecimal(row[CN.Quantity]);
                        newRow[CN.StockLocn] = Convert.ToInt32(row[CN.StockLocn]);
                        newRow[CN.ItemNo] = (string)row[CN.ItemNo];
                        newRow[CN.ItemId] = row[CN.ItemId];        // RI jec
                        //newRow[CN.RetItemId] = row[CN.ItemId];        // RI jec Ret Item same as orig Item

                        if (Convert.ToString(Country[CountryParameterNames.RIInterfaceOptions]) != "FACT")      //IP - 01/10/12 - #10501 - LW75161
                        {
                            newRow[CN.RetItemId] = row[CN.ItemId];        // RI jec Ret Item same as orig Item
                        }
                        else
                        {
                            newRow[CN.RetItemId] = row[CN.RetItemId];
                        }

                        newRow[CN.RetItemNo] = (string)row[CN.RetItemNo];
                        newRow[CN.RetStockLocn] = Convert.ToInt32(row[CN.RetStockLocn]);
                        newRow[CN.AcctNo] = acctno;
                        newRow[CN.AgrmtNo] = (int)row[CN.AgreementNo];
                        newRow[CN.BuffNo] = 0;
                        newRow[CN.DateDelPlan] = Date.blankDate;
                        //70988 Identical Replacement Requires dn printed at branch location
                        if ((bool)Country[CountryParameterNames.IdentRepEqualsDNStockBranch] && CollectionType == "R")
                        {
                            newRow[CN.BuffBranchNo] = (string)row[CN.RetStockLocn];
                        }
                        else
                        {
                            newRow[CN.BuffBranchNo] = 0;
                        }
                        newRow[CN.RetVal] = Convert.ToDouble(StripCurrency((string)row[CN.RetVal]));
                        returnPrice += Convert.ToDecimal(StripCurrency((string)row[CN.RetVal]));
                        if ((string)Country[CountryParameterNames.AgreementTaxType] == "E")
                            returnPrice += Convert.ToDecimal(StripCurrency((string)row[CN.TaxAmt]));

                        newRow[CN.DelOrColl] = type;
                        newRow[CN.EmployeeNo] = Credential.UserId.ToString();
                        newRow[CN.ContractNo] = (string)row[CN.ContractNo];
                        newRow[CN.ItemType] = (string)row[CN.ItemType];
                        newRow[CN.Reason] = (string)row[CN.Reason];
                        newRow[CN.DeliveryArea] = (string)row[CN.DeliveryArea];
                        newRow[CN.DeliveryAddress] = (string)row[CN.DeliveryAddress];           // #14927
                        newRow[CN.DeliveryProcess] = (string)row[CN.DeliveryProcess];
                        newRow[CN.WarrantyFulfilled] = (string)row[CN.WarrantyFulfilled];
                        newRow[CN.Refund] = Convert.ToDecimal(StripCurrency((string)row[CN.Refund]));
                        newRow[CN.RefundType] = (string)row[CN.RefundType];
                        newRow[CN.TaxAmt] = Convert.ToDouble(StripCurrency((string)row[CN.TaxAmt]));
                        newRow[CN.CollectionType] = CollectionType;
                        newRow[CN.ParentItemNo] = row[CN.ParentItemNo];
                        newRow[CN.ParentItemId] = row[CN.ParentItemId];         // RI
                        // Only add notes if visible //IP - 10/02/10 - CR1048 (Ref:3.1.4 & 3.1.5) Merged - Malaysia Enhancements (CR1072)
                        if (tbNotes.Visible == true)
                            newRow[CN.Notes] = tbNotes.Text;
                        else
                            newRow[CN.Notes] = "";
                        newRow[CN.LineItemId] = row[CN.LineItemId];         // #10441 jec
                        newRow[CN.Price] = Convert.ToDecimal(StripCurrency((string)row[CN.Price]));        // #12842
                        newRow[CN.Discount] = Convert.ToInt16(IsDiscount(Convert.ToInt16(row[CN.Category])));       // #18554

                        newRow["AnnualServiceContract"] = Convert.ToBoolean(row["AnnualServiceContract"]);
                        newRow["AnnualServiceContractUsed"] = Convert.ToDecimal(row["AnnualServiceContractUsed"]);  

                        schedTab.Rows.Add(newRow);
                    }
                    else
                    {
                        // Collect all non-stocks when the schedule is saved
                        DataRow newRow;
                        newRow = nonStockList.NewRow();
                        newRow[CN.ItemNo] = (string)row[CN.ItemNo];
                        newRow[CN.ItemId] = row[CN.ItemId];        // RI jec
                        newRow[CN.StockLocn] = Convert.ToInt16(row[CN.StockLocn]);
                        newRow[CN.ContractNo] = (string)row[CN.ContractNo];
                        newRow[CN.OrdVal] = Convert.ToDouble(StripCurrency((string)row[CN.OrdVal]));
                        returnPrice += Convert.ToDecimal(StripCurrency((string)row[CN.OrdVal]));
                        if ((string)Country[CountryParameterNames.AgreementTaxType] == "E")
                            returnPrice += Convert.ToDecimal(StripCurrency((string)row[CN.TaxAmt]));

                        newRow[CN.TaxAmt] = Convert.ToDouble(StripCurrency((string)row[CN.TaxAmt]));
                        newRow[CN.RetItemNo] = (string)row[CN.RetItemNo];

                        //newRow[CN.RetItemId] = row[CN.ItemId]; //#12861       // RI jec Ret Item same as orig Item

                        //#12861 
                        if (Convert.ToString(Country[CountryParameterNames.RIInterfaceOptions]) != "FACT")      //IP - 01/10/12 - #10501 - LW75161
                        {
                            newRow[CN.RetItemId] = row[CN.ItemId];        // RI jec Ret Item same as orig Item
                        }
                        else
                        {
                            newRow[CN.RetItemId] = row[CN.RetItemId];
                        }

                        newRow[CN.EmployeeNo] = Credential.UserId.ToString();
                        newRow[CN.RetStockLocn] = Convert.ToInt32(row[CN.RetStockLocn]);

                        if (row[CN.Type] == DBNull.Value ||
                            ((string)row[CN.Type]).Length == 0)
                            newRow[CN.DelOrColl] = "C";
                        else
                            newRow[CN.DelOrColl] = (string)row[CN.Type];

                        newRow[CN.Refund] = Convert.ToDecimal(StripCurrency((string)row[CN.Refund]));
                        newRow[CN.RefundType] = (string)row[CN.RefundType];
                        newRow[CN.CollectionType] = CollectionType;
                        newRow[CN.ParentItemNo] = row[CN.ParentItemNo];
                        newRow[CN.ParentItemId] = row[CN.ParentItemId];         // RI
                        // Only add notes if visible //IP - 10/02/10 - CR1048 (Ref:3.1.4 & 3.1.5) Merged - Malaysia Enhancements (CR1072)
                        if (tbNotes.Visible == true)
                            newRow[CN.Notes] = tbNotes.Text;
                        else
                            newRow[CN.Notes] = "";

                        newRow["ReadyAssist"] = Convert.ToBoolean(row["ReadyAssist"]);  //#18607 - CR15594
                        newRow["ReadyAssistUsed"] = Convert.ToDecimal(row["ReadyAssistUsed"]);  //#18607 - CR15594

                        newRow["AnnualServiceContract"] = Convert.ToBoolean(row["AnnualServiceContract"]);
                        newRow["AnnualServiceContractUsed"] = Convert.ToDecimal(row["AnnualServiceContractUsed"]);  

                        nonStockList.Rows.Add(newRow);

                    }
                }

                // Check the reurns will not cause a negative Agreement Total
                // (EG: Could happen if only a discount line item left)
                decimal adminPrice = 0;
                decimal cashPrice = AccountManager.GetChargeableCashPrice(acctno, ref adminPrice, out Error);
                if (Error.Length > 0)
                    ShowError(Error);
                else if (cashPrice < returnPrice && CollectionType != "R") //#17940
                {
                    _success = false;
                    decimal negTotal = cashPrice - returnPrice;
                    ShowInfo("M_NEGATIVEGOODSRETURN", new object[] { negTotal.ToString(DecimalPlaces) });
                }
                else
                {
                    itemSet.Tables.Add(schedTab);
                    itemSet.Tables.Add(nonStockList);

                    AccountManager.SaveSchedule(itemSet, Convert.ToInt16(Config.BranchCode),
                        Config.CountryCode, accountType, acctno, agreementNo, requestNo, out Error); //#11989 - added requestNo


                    if (Error.Length > 0)
                        ShowError(Error);
                }
            }
            return _success;
        }


        //
        //Raj1
        private void SearchThread()
        {
            try
            {
                Function = "SearchThread";
                bool status = true;
                Wait();

                agreement = AccountManager.GetAgreement(accountNo, 1, true, out Error);
                if (Error.Length > 0)
                {
                    status = false;
                    ShowError(Error);
                }

                if (status)
                {
                    repo = AccountManager.GetForRepo(accountNo, out accountType, out Error);

                    if (Error.Length > 0)
                    {
                        ShowError(Error);
                    }
                    else
                    {
                        //#15993
                        PopulateWarrantyReturnDictionary();
                    }
                }

            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                Function += Finished;
                StopWait();
            }
        }

        //#15993
        private void PopulateWarrantyReturnDictionary()
        {
            List<WarrantyReturnList> warranties = new List<WarrantyReturnList>();
            var items = repo.Tables[0];

            foreach (DataRow dr in items.Rows)
            {
                if (WarrantyType.IsFree(Convert.ToString(dr["WarrantyType"])) == false && Convert.ToString(dr["contractno"]) != "")     //#17883
                {
                    warranties.Add(new WarrantyReturnList
                    {
                        warrantyNumber = Convert.ToString(dr["ItemNo"]),
                        warrantyItemID = Convert.ToInt32(dr["ItemID"]),
                        contractNo = Convert.ToString(dr["contractno"]),
                        stocklocn = Convert.ToInt16(dr["StockLocn"])
                    });
                }
            }

            //#15993
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

                            warrantyReturnPC.TryGetValue(new DictionaryKey(Convert.ToString(item.warrantyReturn.WarrantyNo), item.WarrantyContractNo), out details);   // #17506

                            if (details == null)
                            {
                                warrantyReturnPC.Add(new DictionaryKey(item.warrantyReturn.WarrantyNo, Convert.ToString(item.WarrantyContractNo)), item);        // #17506
                            }
                        }
                    }
                }
                else
                {
                    //Possibly need to do something
                }
            }, this);
        }

        private void btnSearch_Click(object sender, System.EventArgs e)
        {
            bool status = true;
            loanAccount = false;
            //IP - 15/08/08 - CoSACS Improvement - Search button - Moved code to new method
            //'Public void LoadGoodsReturnData'
            //Set the class level variable to the account number text field.
            accountNo = txtAccountNo.Text;

            //IP - 25/05/10 - UAT(234) UAT5.2.1.0 Log -  Do not allow HCC accounts to be loadded.
            status = IsHomeClubAcct();

            if (status == true)
            {
                LoadGoodsReturnData(accountNo);

                if (repo != null)          //#18525
                {
                    drpCollectionAdr.DataSource = repo.Tables[1];       // #14927
                    drpCollectionAdr.DisplayMember = "AddType";
                    drpCollectionAdr.ValueMember = "AddType";
                    drpCollectionAdr.Enabled = false;
                }
          
            }

        }

        private void btnEnter_Click(object sender, System.EventArgs e)
        {
            try
            {
                Function = "btnEnter_Click";
                Wait();

                int rowCount = 0;
                int ReturnItemID = 0;                                           //IP - 08/09/11 - RI - #8112

                bool valid = true;
                int index = dgDeliveredItems.CurrentRowIndex;
                DateTime delDate;

                if (drpReposession.Enabled && drpReposession.SelectedIndex < 0)
                {
                    errorProvider1.SetError(drpReposession, GetResource("M_ENTERMANDATORY"));
                    valid = false;
                }
                else
                    errorProvider1.SetError(drpReposession, "");

                // Default to "N" if not enabled 70451 jec 13/11/08
                if (!drpReposession.Enabled)
                {
                    drpReposession.SelectedIndex = 1;
                }

                if (drpRepoType.Enabled && drpRepoType.SelectedIndex < 0)
                {
                    errorProvider1.SetError(drpRepoType, GetResource("M_ENTERMANDATORY"));
                    valid = false;
                }
                else
                    errorProvider1.SetError(drpRepoType, "");

                if (drpCredReplace.Enabled && drpCredReplace.SelectedIndex == 0)
                {
                    errorProvider1.SetError(drpCredReplace, GetResource("M_ENTERMANDATORY"));
                    valid = false;
                }
                else
                    errorProvider1.SetError(drpCredReplace, "");

                // 5.1 uat142 rdb 19/11/07 allow first item selection
                // Also fixes 69614
                if (drpReason.SelectedIndex < 0)
                {
                    errorProvider1.SetError(drpReason, GetResource("M_ENTERMANDATORY"));
                    valid = false;
                }
                else
                    errorProvider1.SetError(drpReason, "");

                if (txtTotalValue.Text.Trim() == String.Empty)
                {
                    errorProvider1.SetError(txtTotalValue, GetResource("M_ENTERMANDATORY"));
                    valid = false;
                }
                else
                    errorProvider1.SetError(txtTotalValue, "");

                //IP - 26/11/09 - UAT5.2 (927)
                int result;
                int.TryParse(txtReturnLocn.Text, out result);

                if (result == 0)
                {
                    errorProvider1.SetError(txtReturnLocn, GetResource("M_NUMERICVALUE"));
                    valid = false;
                }
                else
                {
                    errorProvider1.SetError(txtReturnLocn, "");
                }

                

                //If repossession then check Return Item Number matches item number suffixed with one of the valid Repossession Conditions.
                if (drpReposession.Enabled && Convert.ToString(drpReposession.SelectedItem) == "Y")
                {
                    if(validRepossessionReturnItemNos.Count > 0 && validRepossessionReturnItemNos.FindIndex(p => string.Compare(p, txtReturnItemNo.Text, true) == 0) == -1)
                    {
                        StringBuilder sb = new StringBuilder();
                        var count = 0;

                        foreach (var item in validRepossessionReturnItemNos)
                        {
                            count = count + 1;
                            sb.Append(item);

                            if (count < validRepossessionReturnItemNos.Count)
                            {
                                sb.Append(',');
                            }
                        }

                        ShowWarning("The Return Item Number must match one of the following valid Return Item Numbers for a Repossession: " + sb);
                        valid = false;
                    }
                }
                 
                    

                var currentRow = ((DataView)dgDeliveredItems.DataSource)[index];
                if (Convert.ToString(drpReposession.SelectedItem) == "Y" &&
                    Convert.ToString(currentRow[CN.ItemType]) == "S" &&
                    Convert.ToInt32(currentRow[CN.RetItemId]) == 0 &&
                    Convert.ToString(Country[CountryParameterNames.RIInterfaceOptions]) != "FACT")            //IP - 08/09/11 - RI - #8112
                {
                    ShowWarning("Unable to process repossession. To continue you need to have a repossessed stock item set up");
                    valid = false;
                }

                if (WarrantyType.IsFree(Convert.ToString(currentRow["WarrantyType"])) == true && Convert.ToInt32(currentRow["ParentItemId"]) != 0) //#17883
                {
                    ShowWarning("Unable to collect a free warranty by itself");
                    valid = false;
                }

                if (Convert.ToString(currentRow["Department"]) == "PCDIS") //#16208
                {
                    ShowWarning("Unable to collect a discount by itself");
                    valid = false;
                }

                // return value can never be greater than the original value
                if (valid)
                {
                    if (Convert.ToDecimal(StripCurrency(txtTotalValue.Text)) > Convert.ToDecimal(((DataView)dgDeliveredItems.DataSource)[index][CN.OrdVal]))
                    {
                        errorProvider1.SetError(txtTotalValue, GetResource("M_RETURNVALUE"));
                        valid = false;
                    }
                    else
                        errorProvider1.SetError(txtTotalValue, "");
                }

                // UAT 380 Repossession not allowed if the account has any open service requests and ServiceRepossession country parameter set to true
                if (valid)
                {
                    var openSRCount = Services.GetService(STL.Common.Services.Services.ServiceTypes.CosacsWeb).CheckForOpenServiceRequests(accountNo);

                    //int openSRs = Convert.ToInt32(((DataView)dgDeliveredItems.DataSource)[index][CN.NoOfOpenSRs]);
                    string request = openSRCount == 1 ? "request" : "requests";

                    if (drpReposession.Enabled && Convert.ToString(drpReposession.SelectedItem) == "Y" && openSRCount > 0 && (bool)Country[CountryParameterNames.ServiceRepossession])
                    {
                        MessageBox.Show("No repossessions can be processed for this account.\n\nThis is because it currently has " + openSRCount + " open service " + request + ".", "Repossession Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        valid = false;
                    }
                }

                // 69647 If item is a deleted item then authorisation required
                if (((DataView)dgDeliveredItems.DataSource)[index]["Deleted"].ToString() == "Y" && (drpReposession.Enabled == false || Convert.ToString(drpReposession.SelectedItem) == "N"))
                {
                    AuthorisePrompt ap = new AuthorisePrompt(this, lAuthorise, GetResource("M_DELETEDITEMRETURN"));
                    ap.ShowDialog();
                    if (!ap.Authorised)
                    {
                        valid = false;
                    }
                }

                warrantiesToCollect.Clear();

                if (valid)
                {
                    // 68666 rdb 26/10/07 additional fix when items are added if this is a cancelation
                    // and we are canceling a warranty and the warranty was clicked before drpReposession was set
                    // the return number will not have been set in this instance we must set it now
                    DataView dv = (DataView)dgDeliveredItems.DataSource;
                    int warrantyRetCodeItemID = 0;                              //IP - 13/09/11 - RI - #8112

                    PostbtnEnter_Click(index, valid, 0);

                    drpCollectionAdr.Enabled = false;        // #14927  
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                Function += Finished;
                StopWait();
            }
        }

        private void PostbtnEnter_Click(int index, bool valid, int ReturnItemID)
        {

            int rowCount = 0;
            int locn = Convert.ToInt32(txtLocn.Text);
            int retLocn = Convert.ToInt32(txtReturnLocn.Text);              //IP - 21/11/11 - #8647 - LW74285

            //rowCount = AccountManager.GetItemCount(txtItemNo.Text, (short)locn, out Error);
            rowCount = AccountManager.GetItemCount(Convert.ToInt32(((DataView)dgDeliveredItems.DataSource)[index][CN.ItemId]), (short)locn, out Error);         //RI 19/05/11
            if (Error.Length > 0)
                ShowError(Error);
            else
            {
                if (rowCount == 0)
                {
                    ShowInfo("M_STOCKNOTPRESENT");
                    valid = false;
                }
            }

            if (valid && txtReturnItemNo.Text.Length > 0)
            {
                locn = Convert.ToInt32(txtReturnLocn.Text);
                //rowCount = AccountManager.GetItemCount(txtReturnItemNo.Text, (short)locn, out Error);
                if (Convert.ToString(Country[CountryParameterNames.RIInterfaceOptions]) != "FACT")                   //IP - 08/09/11 - RI - #8112
                {
                    if (Convert.ToString(drpReposession.SelectedItem) == "Y")  //IP - 21/11/11 - #8647 - LW74285 - If repossession check that the repossession item exists at the return stock location
                    {
                        rowCount = AccountManager.GetItemCount(Convert.ToInt32(((DataView)dgDeliveredItems.DataSource)[index][CN.RetItemId]), (short)retLocn, out Error);
                    }
                    else
                    {
                        rowCount = AccountManager.GetItemCount(Convert.ToInt32(((DataView)dgDeliveredItems.DataSource)[index][CN.ItemId]), (short)locn, out Error);       //RI 19/05/11
                    }
                }
                else
                {
                    //rowCount = AccountManager.GetItemCount(txtReturnItemNo.Text, (short)locn, out Error);
                    ReturnItemID = AccountManager.GetReturnItemIDForItemCode(txtReturnItemNo.Text, (short)locn, out Error);    //IP - 08/09/11 - RI - #8112 - if the item exists, the itemid will be returned
                }
                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    //if(rowCount == 0)
                    //IP - 08/09/11 - RI - #8112
                    if ((rowCount == 0 && Convert.ToString(Country[CountryParameterNames.RIInterfaceOptions]) != "FACT") || (ReturnItemID == 0 && Convert.ToString(Country[CountryParameterNames.RIInterfaceOptions]) == "FACT"))
                    {
                        ShowInfo("M_RETSTOCKNOTPRESENT");
                        valid = false;
                    }
                    else
                    {
                        //We may need to set the ReturnItemID here for the itemno entered as the Return Item Number
                        if (Convert.ToString(Country[CountryParameterNames.RIInterfaceOptions]) == "FACT")
                        {
                            ((DataView)dgDeliveredItems.DataSource)[index][CN.RetItemId] = ReturnItemID;
                        }
                    }
                }
            }

            /* check to see if the item has an associated warranty */
            if (valid)
            {
                string parm = "";
                string[] warranties = new string[1];
                //dsWarranties = AccountManager.GetAssociatedWarranties(accountNo, txtItemNo.Text, Convert.ToInt16(txtLocn.Text), out Error);
                dsWarranties = AccountManager.GetAssociatedWarranties(accountNo, Convert.ToInt32(((DataView)dgDeliveredItems.DataSource)[index][CN.ItemId]),     // RI
                                    Convert.ToInt16(txtLocn.Text), out Error);
                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    int count = 0;
                    var GroupId = 0;
                    var GroupCount = 0;

                    foreach (DataRow r in dsWarranties.Tables[TN.Warranties].Rows)
                    {
                        //Only select warranties for quantity of items collected.
                        if (GroupId != Convert.ToInt32(r["WarrantyGroupId"]))
                        {
                            GroupCount++;
                            GroupId = Convert.ToInt32(r["WarrantyGroupId"]);
                        }

                        if (GroupCount > Convert.ToDouble(txtQty.Text))          //#15993
                            break;
                        //if (count == Convert.ToDouble(txtQty.Text))          //#15993
                        //count++;
                        parm += (string)r[CN.AssociatedItem] + Environment.NewLine;
                        warrantiesToCollect.Add((string)r[CN.AssociatedItem]);  




                    }


                    //if (dsWarranties.Tables[TN.Warranties].Rows.Count > 0 &&    
                    //CollectionType != "E" && CollectionType != "R")


                    if (dsWarranties.Tables[TN.Warranties].Rows.Count > 0 && CollectionType != "R")  //CR2018-013 Allow for Exchange

                        ShowInfo("M_ASSOCIATEDWARRANTY", new object[] { txtItemNo.Text, parm });
                }
            }

            //Collect associated discounts

            if (valid)
            {
                string parm = "";
                string[] warranties = new string[1];

                int count = 0;
                foreach (DataRow r in dsWarranties.Tables[TN.Discounts].Rows)
                {
                    count++;
                   

                    parm += (r[CN.AssociatedItem]).ToString() + Environment.NewLine;  //CR2018-013 
                    warrantiesToCollect.Add((string)r[CN.AssociatedItem]);   
                }

                if (dsWarranties.Tables[TN.Discounts].Rows.Count > 0 &&
                    CollectionType != "I" && CollectionType != "R" && CollectionType != "")   //IP/JC - 16/01/12  - #8917 - LW74448 - If repossession then do not display prompt for adding discount
                    ShowInfo("M_ASSOCIATEDDISCOUNT", new object[] { txtItemNo.Text, parm });

            }

            if (valid)
            {

           
                //#18608 - CR15594
                if(Convert.ToBoolean(((DataView)dgDeliveredItems.DataSource)[index]["ReadyAssist"]) == true)
                {
                         readyAssistCancellation = true;
                }

                //#18607 - CR15594
                if (Convert.ToDecimal(((DataView)dgDeliveredItems.DataSource)[index]["ReadyAssistUsed"]) > 0)
                {
                    var value = Convert.ToDecimal(((DataView)dgDeliveredItems.DataSource)[index]["ReadyAssistUsed"]);
                    ShowInfo("M_READYASSISTDEBIT", new object[] { value });
                }

                if (Convert.ToDecimal(((DataView)dgDeliveredItems.DataSource)[index]["AnnualServiceContractUsed"]) > 0)
                {
                    var value = Convert.ToDecimal(((DataView)dgDeliveredItems.DataSource)[index]["AnnualServiceContractUsed"]);
                    ShowInfo("M_ANNUALSERVICECONTRACTDEBIT", new object[] { value });
                }
                
                FillDeliveryDetails(false);
                btnRemove.Enabled = true;

                if (warrantiesToCollect.Count > 0)
                {
                    parentReturnLocation = txtReturnLocn.Text;
                    //ProcessWarranties(txtItemNo.Text, Convert.ToInt16(txtLocn.Text));           
                    ProcessWarranties(Convert.ToInt32(((DataView)dgDeliveredItems.DataSource)[index][CN.ItemId]), Convert.ToInt16(txtLocn.Text)); // RI
                    parentReturnLocation = "";
                }
            }
        }

        private void btnRemove_Click(object sender, System.EventArgs e)
        {
            try
            {
                loaded = false;
                Function = "btnRemove_Click";
                Wait();

                int index = dgGoodsToReturn.CurrentRowIndex;
                int location = 0;
                string itemNo = "";
                bool deleted = false;

                DataView dv = (DataView)dgGoodsToReturn.DataSource;
                int count = dv.Count;

                for (int i = count - 1; i >= 0; i--)
                {
                    if (dgGoodsToReturn.IsSelected(i))
                    {
                        decimal d = Convert.ToDecimal(StripCurrency(txtCreditTotal.Text));
                        d -= Convert.ToDecimal(StripCurrency((string)dv[i][CN.OrdVal]));
                        txtCreditTotal.Text = d.ToString(DecimalPlaces);
                        itemNo = (string)dv[i][CN.ItemNo];
                        location = Convert.ToInt16(dv[i][CN.StockLocn]);
                        dv[i][CN.ItemNo] = "";
                        deleted = true;
                    }
                }

                for (int i = count - 1; i >= 0; i--)
                {
                    if ((string)dv[i][CN.ParentItemNo] == itemNo && Convert.ToInt16(dv[i][CN.ParentLocation]) == location && deleted)
                    {
                        decimal d = Convert.ToDecimal(StripCurrency(txtCreditTotal.Text));
                        d -= Convert.ToDecimal(StripCurrency((string)dv[i][CN.OrdVal]));
                        txtCreditTotal.Text = d.ToString(DecimalPlaces);
                        dv[i][CN.ItemNo] = "";
                    }
                }

                for (int i = count - 1; i >= 0; i--)
                {
                    if (((string)dv[i][CN.ItemNo]).Length == 0)
                        dv.Delete(i);
                }

                loaded = true;
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                Function += Finished;
                StopWait();
            }
        }

        private void btnClear_Click(object sender, System.EventArgs e)
        {
            try
            {
                Function = "btnClear_Click";
                Wait();
                InitData(true);
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                Function += Finished;
                StopWait();
            }
        }

        private void btnSave_Click(object sender, System.EventArgs e)
        {
            try
            {
                Function = "btnSave_Click";
                Wait();

                DataSet fintrans;
                DataSet ds = new DataSet();
                decimal repoTotal = 0;
                //int agrNo = 0;
                bool valid = true;

                if (drpReposession.Enabled && drpReposession.SelectedIndex < 0)
                {
                    errorProvider1.SetError(drpReposession, GetResource("M_ENTERMANDATORY"));
                    valid = false;
                }
                else
                    errorProvider1.SetError(drpReposession, "");

                if (drpRepoType.Enabled && drpRepoType.SelectedIndex < 0)
                {
                    errorProvider1.SetError(drpRepoType, GetResource("M_ENTERMANDATORY"));
                    valid = false;
                }
                else
                    errorProvider1.SetError(drpRepoType, "");

                if (drpCredReplace.Enabled && drpCredReplace.SelectedIndex == 0)
                {
                    errorProvider1.SetError(drpCredReplace, GetResource("M_ENTERMANDATORY"));
                    valid = false;
                }
                else
                    errorProvider1.SetError(drpCredReplace, "");

                // 5.1 uat142 rdb 19/11/07 allow first item selection
                // Also fixes 69614
                if (drpReason.SelectedIndex < 0)
                {
                    errorProvider1.SetError(drpReason, GetResource("M_ENTERMANDATORY"));
                    valid = false;
                }
                else
                {
                    errorProvider1.SetError(drpReason, "");

                }

                if (tbNotes.Text.Length == 0 && tbNotes.Visible == true)      //CR1048 jec  //IP - 10/02/10 - CR1048 (Ref:3.1.4 & 3.1.5) Merged - Malaysia Enhancements (CR1072)
                {
                    errorProvider1.SetError(tbNotes, GetResource("M_ENTERMANDATORYNOTES"));
                    valid = false;
                }
                else
                    errorProvider1.SetError(tbNotes, "");

                if (valid)
                {
                    if ((DataView)dgGoodsToReturn.DataSource == null)
                    {
                        ShowInfo("M_NOGOODSTORETURN");
                    }
                    else
                    {
                        string acctNo = txtAccountNo.Text.Replace("-", "");
                        DataView dvGoodsToReturn = (DataView)dgGoodsToReturn.DataSource;
                        int agrmtno = Convert.ToInt32(dvGoodsToReturn.Table.Rows[0]["AgreementNo"].ToString());

                        if (Convert.ToString(drpReposession.SelectedItem) == "Y")
                        {
                            string repoCode = "";

                            AccountManager.SaveRepossArrears(Convert.ToDecimal(StripCurrency(txtArrears.Text)),
                                Convert.ToDecimal(StripCurrency(txtCreditTotal.Text)), txtAccountNo.Text.Replace("-", ""), out Error);

                            switch ((string)drpRepoType.SelectedItem)
                            {
                                case "Full": repoCode = "FREP";
                                    break;
                                case "Part": repoCode = "PREP";
                                    break;
                                case "Voluntary": repoCode = "VREP";
                                    break;
                                default: break;
                            }

                            DataTable tab = new DataTable();
                            tab.Columns.Add("Date Added");
                            tab.Columns.Add("Code");
                            tab.Columns.Add("Added By");

                            DataRow newRow;
                            newRow = tab.NewRow();
                            newRow["Date Added"] = DateTime.Now; //IP - 30/09/09 - UAT(897) Changed from DateTime.Today
                            newRow["Code"] = "R";
                            newRow["Added By"] = Credential.UserId;
                            tab.Rows.Add(newRow);
                            //ds.Tables.Add(tab);

                            newRow = tab.NewRow();
                            newRow["Date Added"] = DateTime.Now; //IP - 30/09/09 - UAT(897) Changed from DateTime.Today
                            newRow["Code"] = repoCode;
                            newRow["Added By"] = Credential.UserId;
                            tab.Rows.Add(newRow);
                            ds.Tables.Add(tab);

                            AccountManager.AddCodesToAccount(txtAccountNo.Text.Replace("-", ""), ds);

                            fintrans = AccountManager.GetReposessionAndRedelivery(txtAccountNo.Text.Replace("-", ""), out Error);
                            foreach (DataTable dt in fintrans.Tables)
                            {
                                switch (dt.TableName)
                                {
                                    case TN.FinTrans: ProcessFintrans(dt, ref repoTotal);
                                        break;
                                }
                            }

                            if (-agrmtTotal > repoTotal - Convert.ToDecimal(StripCurrency(txtCreditTotal.Text)))
                            {
                                ShowInfo("M_CREDREPOSS");
                                valid = false;
                            }

                            if (valid)
                            {
                                ProcessDelivery(txtAccountNo.Text.Replace("-", ""));
                                //UpdateInvoiceVersion
                                DataView dvReturnItems = (DataView)dgGoodsToReturn.DataSource;
                                DataTable dtReturnItems = new DataTable();
                                dtReturnItems = dvReturnItems.ToTable();
                                AccountManager.UpdateInvoiceVersion(acctNo, agrmtno, true, dtReturnItems);                                
                            }
                            InitData(true);
                        }
                        else
                        {

                            if (CollectionType == "I")  //#17290
                            {
                                int index = dgDeliveredItems.CurrentRowIndex;
                                DataView dv = (DataView)dgDeliveredItems.DataSource;


                                acctNo = txtAccountNo.Text.Replace("-", "");
                                dsI = AccountManager.GetIRItems(acctNo, "%", 0,
                                                                       Convert.ToDateTime(dv[index][CN.DateDel]), Convert.ToDateTime(dv[index][CN.DateDel]),
                                                                       accountType, out Error);
                            }

                            if (ProcessSchedule(txtAccountNo.Text.Replace("-", "")))
                            {
                                //#17290 Instant Replacement 
                                if (CollectionType == "I")
                                {
                                    ProcessInstantReplacement();
                                }

                                //#17678 - Identical Replacement
                                if (CollectionType == "R")
                                {
                                    NewAccount revise = new NewAccount(txtAccountNo.Text.Replace("-", ""), 1, CollectionType, (DataView)dgGoodsToReturn.DataSource, false, FormRoot, FormParent);

                                    if (revise.AccountLoaded)
                                    {
                                        ((MainForm)FormRoot).AddTabPage(revise, 24);
                                        revise.SupressEvents = false;
                                    }
                                }

                                //#18608 - CR15594 - Load New Sales screen to re-calculate instalments and re-print agreements.
                                if (CollectionType == "C" && readyAssistCancellation == true && AT.IsCreditType(accountType))
                                {
                                    NewAccount revise = new NewAccount(txtAccountNo.Text.Replace("-", ""), 1, CollectionType, null, false, FormRoot, FormParent);

                                    if (revise.AccountLoaded)
                                    {
                                        ((MainForm)FormRoot).AddTabPage(revise, 24);
                                        revise.SupressEvents = false;
                                    }
                                }

                                //UpdateInvoiceVersion
                                DataView dvReturnItems = (DataView)dgGoodsToReturn.DataSource;
                                DataTable dtReturnItems = new DataTable();
                                dtReturnItems = dvReturnItems.ToTable();                  
                                
                                AccountManager.UpdateInvoiceVersion(acctNo, agrmtno, true, dtReturnItems);                              

                                InitData(true);
                            }
                        }
                        

                        if (requestNo != null)              //#11989
                        {
                            ((BERReplacements)this.FormParent).statusBar.Text = "Goods Return has been saved, please re-load the screen";
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
                Function += Finished;
                StopWait();
            }
        }

        //#17290
        private void ProcessInstantReplacement()
        {
         
                instReplace = dsI.Tables[0];
                DataView dvir = new DataView(instReplace);
                dvir.RowFilter = CN.ItemNo + " = '" + txtItemNo.Text + "'";

                //for (int i = 0; i < Convert.ToInt16(txtQty.Text); i++)
                for (int i = 0; i < dvir.Count && i < Convert.ToInt16(dvir[irIndex][CN.Quantity]); i++)                         // #17290
                {
                    irIndex = i;
                    string acctNo = (string)dvir[irIndex]["acctNo"];
                    string productCode = (string)dvir[irIndex][CN.ItemNo];
                    int itemID = (int)dvir[irIndex][CN.ItemId];
                    string model = (string)dvir[irIndex][CN.ItemNo];
                    short stockLocn = Convert.ToInt16(dvir[irIndex][CN.StockLocn]);
                    string productDescription = (string)dvir[irIndex][CN.ItemDescr1];
                    DateTime dateTrans = (DateTime)dvir[irIndex][CN.DateTrans];
                    int buffNo = (int)dvir[irIndex][CN.BuffNo];
                    decimal price = Convert.ToDecimal(dvir[irIndex][CN.Price]);
                    decimal quantity = Convert.ToDecimal(dvir[irIndex][CN.Quantity]);
                    decimal orderValue = Convert.ToDecimal(dvir[irIndex][CN.TransValue]);
                    string warrantyNo = (string)dvir[irIndex][CN.WarrantyNo];
                    int warrantyID = (int)dvir[irIndex]["WarrantyID"];
                    decimal taxRate = Convert.ToDecimal(dvir[irIndex][CN.TaxRate]);
                    string contractno = (string)dvir[irIndex][CN.ContractNo];
                    int empeeNoSale = (int)dvir[irIndex][CN.EmpeeNoSale];
                    OneForOneReplacement o = new OneForOneReplacement(acctNo, productCode,
                                                                  model, productDescription,
                                                                  dateTrans, stockLocn.ToString(),
                                                                  buffNo, price, quantity,
                                                                  orderValue, warrantyNo,
                                                                  taxRate, FormRoot, this,
                                                                  contractno, empeeNoSale, itemID, warrantyID, accountType); //#18435
                    o.ShowDialog();
                }
            
        }

        private void menuExit_Click(object sender, System.EventArgs e)
        {
            CloseTab();
        }

        private void dgDeliveredItems_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                Function = "dgDeliveredItems_MouseUp";
                Wait();

                if (dgDeliveredItems.CurrentRowIndex >= 0)
                {
                    dgDeliveredItems.Select(dgDeliveredItems.CurrentCell.RowNumber);

                    if (e.Button == MouseButtons.Left)
                    {
                        Wait();
                        int index = dgDeliveredItems.CurrentRowIndex;
                        btnEnter.Enabled = false;
                        maxReturnQty = 0;
                        price = 0;

                        if (index >= 0)
                        {
                            DataView dv = (DataView)dgDeliveredItems.DataSource;

                            // UAT issue 3 - Allow separate collection of a warranty as well as Stock items
                            if (IsWarranty(Convert.ToString(dv[index][CN.Category])))
                            {

                                this.SetupWarranty(dv[dgDeliveredItems.CurrentRowIndex], false, false);      //IP - 14/06/11 - CR1212 - added isDiscount check
                                
                            }
                            else if (Convert.ToString(dv[index][CN.ItemType]) == Stock)
                            {
                                this.SetupStock(dv[dgDeliveredItems.CurrentRowIndex]);
                            }
                            else
                            {
                                this.SetupNonStock(dv[dgDeliveredItems.CurrentRowIndex]);
                            }
                            //jec CR1048 set immediate if buffbr=returnbr //IP - 10/02/10 - CR1048 (Ref:3.1.4 & 3.1.5) Merged - Malaysia Enhancements (CR1072)
                            if (txtLocn.Text.Trim() == txtReturnLocn.Text.Trim()
                                && (parentReturnLocation == txtLocn.Text.Trim() || parentReturnLocation == ""))
                            {
                                rbPickupImmed.Checked = true;
                            }
                            else
                            {
                                rbPickupImmed.Checked = false;
                                // reset Return location to parentlocation saved when processing associated warranty - it gets overwritten
                                if (parentReturnLocation != "")
                                    txtReturnLocn.Text = parentReturnLocation;
                            }
                            //jec CR1048 set immediate or sched  
                            txtReturnLocn_Leave(null, null);   //IP - 10/02/10 - CR1048 (Ref:3.1.4 & 3.1.5) Merged - Malaysia Enhancements (CR1072)

                            int i = drpCollectionAdr.FindString(Convert.ToString(dv[index][CN.DeliveryAddress]));        // #14927
                            if (i != -1)
                            {
                                drpCollectionAdr.SelectedIndex = i;
                                drpCollectionAdr.Enabled = true;        
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
                Function += Finished;
                StopWait();
            }
        }

        private void drpCredReplace_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            // This option effects the total value for a warranty
            try
            {
                //IP - 13/11/09 - UAT5.2 (653) - When the Collection type has changed, we need to reset the item detail fields
                //as previously this remained populated with warranty details when the collection type had changed to 'Exchange'.
                ResetItemDetailFields();

                // 5.1 uat142 rdb 19/11/07 set default reason index to 1 if not selected
                // Also fixes 69614
                if (drpCredReplace.SelectedIndex > 0 && drpReason.SelectedIndex < 0 && drpReason.Items.Count > 0)
                {
                    drpReason.SelectedIndex = 0;
                }

                if (dgDeliveredItems.DataSource != null)
                {
                    // CR784 - if replacement or exchange hide any warranties in the
                    // delivered items datagrid, as we do not want the users
                    // processing them.
                    // 68666 15/10/07 rdb copying fix from v4
                    if (CollectionType == "" || CollectionType == "C" || CollectionType == "E" || drpRepoType.Enabled)  //CR2018-013 Allow for Exchange
                        ((DataView)dgDeliveredItems.DataSource).RowFilter = "";
                    else if (CollectionType == "R")                                         //#17953
                        ((DataView)dgDeliveredItems.DataSource).RowFilter = "Warranty <> 1 AND ReadyAssist <> 1 AND AnnualServiceContract <> 1 AND Department <> 'PCDIS'";  //#19191 - CR15594
                    else if (CollectionType == "I")      //#17290 Instant Replacement 
                    {
                        ((DataView)dgDeliveredItems.DataSource).RowFilter = "";

                       ((MainForm)FormRoot).statusBar1.Text = "WARNING! Only one Instant Replacement item can be processed at a time";   //#17290

                         ((DataView)dgDeliveredItems.DataSource).RowFilter = ((DataView)dgDeliveredItems.DataSource).RowFilter +
                                 CN.RefCode + " = 'ZZ' " +      // only show IR items
                                " and " + CN.LinkedWarranty + " = 'Y' AND Department <> 'PCDIS' "; // That have a warranty attached.
                    }
                    else
                        ((DataView)dgDeliveredItems.DataSource).RowFilter = "Warranty <> 1 AND ReadyAssist <> 1 AND AnnualServiceContract <> 1";  //#19191 - CR15594     //RI Warranty item = 1
                         //((DataView)dgDeliveredItems.DataSource).RowFilter = CN.Category + " not in (12,82)";
                       

                    tab.Clear();                   
                       
                }

                Function = "drpCredReplace_SelectedIndexChanged";
                Wait();
                DataView dv = (DataView)dgDeliveredItems.DataSource;
                if (dgDeliveredItems.CurrentRowIndex >= 0)
                    this.SetTotalValue(dv[dgDeliveredItems.CurrentRowIndex]);

            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                Function += Finished;
                StopWait();
            }
        }

        private void dgGoodsToReturn_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                Function = "dgGoodsToReturn_MouseUp";
                Wait();

                //if(dgGoodsToReturn.CurrentRowIndex >= 0)
                //	dgGoodsToReturn.Select(dgGoodsToReturn.CurrentCell.RowNumber);

                if (e.Button == MouseButtons.Left)
                {
                    int index = dgGoodsToReturn.CurrentRowIndex;

                    if (index >= 0)
                    {
                        bool canRemove = false;
                        DataView dv = (DataView)dgGoodsToReturn.DataSource;

                        if ((string)dv[index][CN.ItemType] == Stock)
                            canRemove = true;
                        //else if (IsWarranty((string)dv[index][CN.Category]))
                        else if (IsWarranty(Convert.ToString(dv[index][CN.Category])))                  //IP - 29/07/11 - RI - #4434
                        {
                            canRemove = true;
                            // UAT issue 3 - Allow separate collection of a warranty 
                            // but don't allow separate removal if the parent is in the list
                            foreach (DataRowView dvRow in dv)
                            {
                                if ((string)dvRow[CN.ItemNo] == (string)dv[index][CN.ParentItemNo] &&
                                    Convert.ToInt16(dvRow[CN.StockLocn]) == Convert.ToInt16(dv[index][CN.ParentLocation]))
                                {
                                    canRemove = false;
                                }
                            }
                        }
                        // 5.1 uat141 rdb 19/11/07 allow removal of discount
                        // Also fixes 69578
                        else if (IsDiscount((short)dv[index][CN.Category]))
                        {
                            canRemove = true;
                        }


                        btnRemove.Enabled = canRemove;
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                Function += Finished;
                StopWait();
            }
        }

        // 5.1 uat141 rdb 19/11/07 allow removal of discount
        // Also fixes 69578
        public bool IsDiscount(short referenceCategory)
        {
            var isDiscount = false;
            //Codes.RowFilter = CN.Category + " = 'DIS' AND " + CN.Reference + " = " + referenceCategory.ToString();
            Codes.RowFilter = CN.Category + " = 'DIS'";

            foreach (DataRowView item in Codes)
            {
                if (item.Row["Reference"].ToString() == referenceCategory.ToString())
                {
                    isDiscount = true;
                    break;
                }
            }

            return isDiscount;
            //return Codes.Count > 0;
        }

        //IP/JC - 12/01/12 - #9440
        public bool IsInstallation(short referenceCategory)
        {
            var isInstallation = false;
            //Codes.RowFilter = CN.Category + " = 'INST' AND " + CN.Reference + " = " + referenceCategory.ToString();
            DataView CodesInst = Codes;
            CodesInst.RowFilter = string.Empty;

            CodesInst.RowFilter = CN.Category + " = 'INST'";

            foreach (DataRowView item in CodesInst)
            {
                if (item.Row["Reference"].ToString() == referenceCategory.ToString())
                {
                    isInstallation = true;
                    break;
                }
            }

            return isInstallation;
            //return Codes.Count > 0;
        }

        private DataView _codes;
        public DataView Codes
        {
            get
            {
                if (_codes == null || _codes.Count == 0)
                {
                    string Error;
                    DataSet ds = StaticDataManager.GetAllCodesAndCategories(out Error);
                    _codes = new DataView(ds.Tables[TN.Code]);
                }
                return _codes;
            }
        }


        private void txtQty_Leave(object sender, System.EventArgs e)
        {
            try
            {
                Function = "txtQty_Leave";
                int index = dgDeliveredItems.CurrentRowIndex;
                Wait();

                if (IsNumeric(txtQty.Text) && txtQty.Text.Length > 0)
                {
                    if (Convert.ToDouble(txtQty.Text) > maxReturnQty)
                    {
                        ShowInfo("M_RETURNQUANTITY");
                        txtQty.Text = maxReturnQty.ToString();
                    }
                    else if (Convert.ToDouble(txtQty.Text) <= 0)
                    {
                        ShowInfo("M_RETURNQUANTITYZERO");
                        txtQty.Text = "1";
                    }
                    // CR2018-013 
                    else if (index >= 0)
                    {
                        if (Convert.ToDouble(txtQty.Text) <= maxReturnQty)
                        {
                            QuantityChanged = true;
                            DataView dv = (DataView)dgDeliveredItems.DataSource;

                            // UAT issue 3 - Allow collection of a warranty as well as Stock items
                            if (IsWarranty(Convert.ToString(dv[index][CN.Category])))
                            {

                                this.SetupWarranty(dv[dgDeliveredItems.CurrentRowIndex], false, false);      //IP - 14/06/11 - CR1212 - added isDiscount check

                            }
                            else if (Convert.ToString(dv[index][CN.ItemType]) == Stock)
                            {
                                this.SetupStock(dv[dgDeliveredItems.CurrentRowIndex]);
                            }
                            else
                            {
                                this.SetupNonStock(dv[dgDeliveredItems.CurrentRowIndex]);
                            }

                        }
                    }
                    

                }
                else
                {
                    ShowInfo("M_NONNUMERIC");
                    txtQty.Text = maxReturnQty.ToString();
                }
                if (txtReturnItemNo.Text != txtItemNo.Text)
                {
                    calculateRepoPrice();
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                Function += Finished;
                StopWait();
            }
        }

        private void calculateRepoPrice()
        {
            string returnItemNo = txtReturnItemNo.Text;
            Int16 returnLocation = Convert.ToInt16(txtReturnLocn.Text);

            int returnItemId = AccountManager.GetReturnItemIDForItemCode(returnItemNo, (short)returnLocation, out Error);

            XmlNode returnItem = AccountManager.GetItemDetails(new GetItemDetailsRequest
            {
                ProductCode = returnItemNo,
                StockLocationNo = Convert.ToInt16(txtReturnLocn.Text),
                BranchCode = Convert.ToInt16(txtReturnLocn.Text),
                AccountType = acctType,
                CountryCode = Config.CountryCode,
                PromoBranch = Convert.ToInt16(Config.BranchCode),
                ItemID = returnItemId
            }, out Error);

            decimal total = Convert.ToDecimal(txtQty.Text) * Convert.ToDecimal(returnItem.Attributes[Tags.CashPrice].Value);
            txtTotalValue.Text = total.ToString(DecimalPlaces);
        }

        private void drpReposession_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                Function = "drpReposession_SelectedIndexChanged";
                Wait();

                if (drpReposession.Enabled)
                {
                    errorProvider1.SetError(drpReposession, "");                //IP - 13/09/11 

                    label3.Visible = drpCredReplace.Visible = drpCredReplace.Enabled = Convert.ToString(drpReposession.SelectedItem) == "Y" ? false : true;
                    drpRepoType.Enabled = (Convert.ToString(drpReposession.SelectedItem) == "Y");

                    if (!drpCredReplace.Enabled)
                        drpCredReplace.SelectedIndex = 0;

                    if (!drpRepoType.Enabled)
                        drpRepoType.SelectedIndex = -1;

                    if (Convert.ToString(drpReposession.SelectedItem) == "Y")  //Need to calculate the value for the markup
                    {
                        //If Repossession selected we need to get the valid Repossession Conditions. The Return Item Number would only be valid if it has one of these conditions
                        //suffixed onto the ReturnItemNumber.
                        RepossessedConditions = Services.GetService(STL.Common.Services.Services.ServiceTypes.CosacsWeb).GetRepossessedConditions();
                        ResetItemDetailFields();
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                Function += Finished;
                StopWait();
            }
        }

        private void txtReturnItemNo_Leave(object sender, System.EventArgs e)
        {
            try
            {
                Function = "txtReturnItemNo_Leave";
                Wait();

                string retItemNo = "";
                string itemNo = "";
                bool warranty = false;

                if (txtReturnItemNo.Text.Length > 1)
                    retItemNo = txtReturnItemNo.Text.Substring(0, 2);
                if (txtItemNo.Text.Length > 1)
                    itemNo = txtItemNo.Text.Substring(0, 2);

                if (itemNo == "19" || itemNo == "XW")
                    warranty = true;

                if (Convert.ToString(drpReposession.SelectedItem) == "Y")
                {
                    if (warranty && (retItemNo != "19" && retItemNo != "XW"))
                    {
                        ShowInfo("M_VALIDRETITEM");
                        return;
                    }

                    if (txtReturnItemNo.Text != txtItemNo.Text)
                    {
                        calculateRepoPrice();
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                Function += Finished;
                StopWait();
            }
        }

        private void LoadDeliveryArea(string branchNo)
        {
            //Customise the Delivery Area data to displayed in the dropdown..
            if (_deliveryAreaData == null)
            {
                string error = string.Empty;
                DataSet ds = SetDataManager.GetSetsForTNameBranch(TN.DeliveryArea, branchNo, out error);
                delAreas = ds.Tables[0];      //#CR12249 - #12224
                if (error.Length > 0)
                {
                    ShowError(error);
                }
                else
                {
                    _deliveryAreaData = ds.Tables[TN.SetsData].Clone();
                    DataRow row = _deliveryAreaData.NewRow();
                    row[CN.SetName] = string.Empty;
                    //row[CN.SetDescript] = GetResource("L_ALL");
                    row[CN.SetDescript] = "";                       //#CR12249 - #12224
                    _deliveryAreaData.Rows.Add(row);
                    foreach (DataRow copyRow in ds.Tables[TN.SetsData].Rows)
                    {
                        row = _deliveryAreaData.NewRow();
                        row[CN.SetName] = copyRow[CN.SetName];
                        row[CN.SetDescript] = copyRow[CN.SetName].ToString();    //#CR12249 - #12224
                            //+ " : " + copyRow[CN.SetDescript].ToString();
                        _deliveryAreaData.Rows.Add(row);
                    }
                }
            }
        }

        private void txtReturnLocn_Leave(object sender, System.EventArgs e)
        {
            //IP - 26/11/09 - UAT5.2 (927)

            errorProvider1.SetError(txtReturnLocn, "");

            int result;
            int.TryParse(txtReturnLocn.Text, out result);

            if (result > 0)
            {
                _deliveryAreaData = null;
                //drpDeliveryArea.DataSource = null;
                LoadDeliveryArea(txtReturnLocn.Text);

                //IP - 10/02/10 - CR1048 (Ref:3.1.4 & 3.1.5) Merged - Malaysia Enhancements (CR1072)
                //jec CR1048 set immediate if return branch is not DHL and do not allow changes - buttons disabled
                //IP - 12/04/10 - UAT(66) UAT5.2
                //bool isDotNetWarehouse = AccountManager.IsDotNetWarehouse(Convert.ToInt16(txtReturnLocn.Text.Trim()), out Error);
                bool isThirdPartyWarehouse = AccountManager.IsThirdPartyWarehouse(Convert.ToInt16(txtReturnLocn.Text.Trim()), out Error);
                //if (txtLocn.Text.Trim() == txtReturnLocn.Text.Trim() && !isDotNetWarehouse)
                //if (isDotNetWarehouse == false)
                //if (isThirdPartyWarehouse == false)               //IP - 21/06/12 - #10481 - Commented out
                //{
                //    rbPickupImmed.Checked = true;
                //}
                //else
                //{
                //    rbScheduling.Checked = true;
                //}
            }
            else
            {
                errorProvider1.SetError(txtReturnLocn, GetResource("M_NUMERICVALUE"));
            }
        }

        // CR784 - this method will determine whether the customer should pay for the
        // expired portion of the warranty being collected.
        private bool PayExpiredPortion()
        {
            bool refundPayment = false;

            //CR1094 Repo now same return value as collections
            warrantyPercentageReturn = 100 - warrantyPercentageReturn;      //#16400 move to here

            if (warrantyPercentageReturn < 100 && warrantyPercentageReturn > 0 && CollectionType == "C")        
            {
                //UAT 349 Check required to determine whether or not the contract is over 30 days old. 
                if ((DateTime.Now - deliveryDate.Date).Days > 30)
                {
                    refundPayment = DialogResult.Yes == ShowInfo("M_CONFIRMPAYMENT", MessageBoxButtons.YesNo);
                }
                else
                {
                    refundPayment = DialogResult.Yes == ShowInfo("M_CONFIRMPAYMENTUNDER30", MessageBoxButtons.YesNo);
                }
            }
            else
            {
                if (CollectionType == "C" || CollectionType == "E")   //CR2018-013 Allow for exchange
                {
                    refundPayment = true;           //#16400 warrantyPercentageReturn=100%
                }  
            }

            // UAT 232 If a reposession then the expiredPcent value should remain as it is in the database JH 03/12/2007
            // UAT 587 If refund payment and either cash account so not repo or repo chosen !=yes
            // UAT658 correct error introduced in UAT587 - should be OR not AND

            //if (refundPayment )// && (!drpReposession.Enabled || drpReposession.SelectedItem.ToString() != "Y"))
            //{
            //   expiredPcent = 100 - expiredPcent;
            //}


            //CR1094 Repo now same return value as collections
            //warrantyPercentageReturn = 100 - warrantyPercentageReturn;

            return refundPayment;
        }

        private int ElapsedMonths(DateTime deliveryDate)
        {
            int y = DateTime.Now.Year - deliveryDate.Year;
            int m = DateTime.Now.Month - deliveryDate.Month;
            int d = DateTime.Now.Day - deliveryDate.Day;

            if (d < 0)
                m--;

            if (m < 0)
            {
                y--;
                m += 12;
            }

            return y * 12 + m;
        }

        protected void OnColumnChange(object sender, System.Data.DataColumnChangeEventArgs e)
        {
            try
            {
                if (loaded)
                {
                    string str = e.ProposedValue.ToString();

                    int index = dgGoodsToReturn.CurrentRowIndex;

                    if (index >= 0)
                    {
                        DataView dvRow = (DataView)dgGoodsToReturn.DataSource;

                        //int rowCount = AccountManager.GetItemCount(str, Convert.ToInt16(dvRow[index][CN.RetStockLocn]), out Error);
                        int rowCount = AccountManager.GetItemCount(0, Convert.ToInt16(dvRow[index][CN.RetStockLocn]), out Error);         //TODO
                        if (Error.Length > 0)
                            ShowError(Error);
                        else
                        {
                           if (rowCount == 0 )   
                              //  if (rowCount == 0 && (Convert.ToString(dvRow[index][CN.ItemType]) == "S"))       
                            {
                                ShowInfo("M_RETSTOCKINVALID");
                                e.Row.CancelEdit();
                            }
                            else
                                e.ProposedValue = str;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        //IP - 15/08/08 - CoSACS Improvement - Search binoculars button
        public void LoadGoodsReturnData(string acctNo)
        {

            //IP - 15/08/08 - CoSACS Improvement - Moved code from 'private void btnSearch_Click'
            accountNo = acctNo;

            try
            {
                Function = "btnSearch_Click";
                Wait();

                //IP 15/08/08 - CoSACS Improvement - if an account number has been selected from the 
                //'Account Search' screen then once passed into this routine set it to the 
                //account number field.
                if (accountNo != "000-0000-0000-0" && txtAccountNo.Text != accountNo)
                {
                    txtAccountNo.Text = accountNo;
                }

                //IP - 31/07/08 - CoSACS Improvement - Search binoculars button.
                if (accountNo == "000-0000-0000-0")
                {
                    AccountSearch acctSearch = new AccountSearch();
                    acctSearch.Details = false;
                    acctSearch.FormParent = this;
                    acctSearch.FormRoot = this.FormRoot;
                    ((MainForm)this.FormRoot).AddTabPage(acctSearch, 9);
                }
                //IP - 31/07/08 - CoSACS Improvement - 
                //if(txtAccountNo.Text!="000-0000-0000-0")
                else
                {
                    InitData(false);
                    accountNo = txtAccountNo.Text.Replace("-", "");
                    agreement = null;
                    repo = null;

                    Thread data = new Thread(new ThreadStart(SearchThread));
                    data.Start();
                    data.Join();

                    if (agreement.Tables[0].TableName == "Locked")
                    {
                        MessageBox.Show("This account is currently loaded in the Revise Account screen. This must be closed before the account can be loaded here.", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        InitData(true);
                        return;
                    }

                    if (agreement != null)
                    {
                        foreach (DataTable dt in agreement.Tables)
                        {
                            switch (dt.TableName)
                            {
                                case TN.AccountDetails: ProcessAccountDetails(dt);
                                    break;

                                case TN.Agreements:DispalyInvoiceNumber(dt);
                                    break;
                            }
                        }
                    }

                    if (repo != null)
                    {
                        RoundDataGridValues(repo.Tables["Table"]);
                        dgDeliveredItems.DataSource = repo.Tables["Table"].DefaultView;

                        if (dgDeliveredItems.TableStyles.Count == 0 &&
                            ((DataView)dgDeliveredItems.DataSource).Count > 0)
                        {
                            DataGridTableStyle tabStyle = new DataGridTableStyle();
                            tabStyle.MappingName = repo.Tables["Table"].TableName;
                            dgDeliveredItems.TableStyles.Add(tabStyle);

                            tabStyle.GridColumnStyles[CN.AcctNo].Width = 0;
                            tabStyle.GridColumnStyles[CN.AgrmtNo].Width = 0;
                            tabStyle.GridColumnStyles[CN.Category].Width = 0;
                            //tabStyle.GridColumnStyles[CN.DelOrColl].Width = 0;

                            tabStyle.GridColumnStyles[CN.ItemNo].Width = 70;
                            tabStyle.GridColumnStyles[CN.ItemNo].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.ItemNo].HeaderText = GetResource("T_ITEMNO");

                            tabStyle.GridColumnStyles[CN.CourtsCode].Width = 60;
                            tabStyle.GridColumnStyles[CN.CourtsCode].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.CourtsCode].HeaderText = "Courts Code";

                            tabStyle.GridColumnStyles[CN.QuantityOrdered].Width = 50;
                            tabStyle.GridColumnStyles[CN.QuantityOrdered].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.QuantityOrdered].HeaderText = GetResource("T_QUANTITY");

                            tabStyle.GridColumnStyles[CN.ItemDescr1].Width = 150;
                            tabStyle.GridColumnStyles[CN.ItemDescr1].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.ItemDescr1].HeaderText = GetResource("T_DESCRIPTION");

                            tabStyle.GridColumnStyles[CN.ItemDescr2].Width = 150;
                            tabStyle.GridColumnStyles[CN.ItemDescr2].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.ItemDescr2].HeaderText = GetResource("T_DESCRIPTION");

                            tabStyle.GridColumnStyles[CN.ColourName].Width = 50;
                            tabStyle.GridColumnStyles[CN.ColourName].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.ColourName].HeaderText = "Colour";
                            tabStyle.GridColumnStyles[CN.ColourName].NullText = "";

                            tabStyle.GridColumnStyles[CN.Price].Width = 70;
                            tabStyle.GridColumnStyles[CN.Price].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.Price].Alignment = HorizontalAlignment.Right;
                            tabStyle.GridColumnStyles[CN.Price].HeaderText = GetResource("T_UNITPRICE");

                            tabStyle.GridColumnStyles[CN.OrdVal].Width = 70;
                            tabStyle.GridColumnStyles[CN.OrdVal].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.OrdVal].Alignment = HorizontalAlignment.Right;
                            tabStyle.GridColumnStyles[CN.OrdVal].HeaderText = GetResource("T_VALUE");

                            tabStyle.GridColumnStyles[CN.StockLocn].Width = 40;
                            tabStyle.GridColumnStyles[CN.StockLocn].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.StockLocn].Alignment = HorizontalAlignment.Right;
                            tabStyle.GridColumnStyles[CN.StockLocn].HeaderText = GetResource("T_STOCKLOCN");

                            tabStyle.GridColumnStyles[CN.QuantityDelivered].Width = 80;
                            tabStyle.GridColumnStyles[CN.QuantityDelivered].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.QuantityDelivered].HeaderText = GetResource("T_DELIVEREDQTY");

                            //tabStyle.GridColumnStyles[CN.ContractNo].Width = 0;
                            tabStyle.GridColumnStyles[CN.ItemType].Width = 0;
                            tabStyle.GridColumnStyles[CN.DateDel].Width = 0;
                            tabStyle.GridColumnStyles[CN.ParentItemNo].Width = 0;
                            tabStyle.GridColumnStyles[CN.ParentLocation].Width = 0;
                            //tabStyle.GridColumnStyles[CN.NoOfOpenSRs].Width = 0;
                            tabStyle.GridColumnStyles[CN.ItemId].Width = 0;                 //IP - 25/05/11 - CR1212 - RI
                            tabStyle.GridColumnStyles[CN.ParentItemId].Width = 0;           //IP - 25/05/11 - CR1212 - RI
                            tabStyle.GridColumnStyles["Warranty"].Width = 0;           // RI
                            tabStyle.GridColumnStyles["RepoCostPcent"].Width = 0;           // RI
                            tabStyle.GridColumnStyles["RepoSellPcent"].Width = 0;           // RI
                            tabStyle.GridColumnStyles[CN.RetItemId].Width = 0;
                            tabStyle.GridColumnStyles[CN.Department].Width = 0;             //IP - 21/07/11 - RI - #3939
                            tabStyle.GridColumnStyles[CN.LineItemId].Width = 0;             // #10411
                            tabStyle.GridColumnStyles["WarrantyType"].Width = 0;             //#17883 // #10411
                            tabStyle.GridColumnStyles["WarrantyGroupId"].Width = 0;             // #10411
                            tabStyle.GridColumnStyles[CN.LinkedWarranty].Width = 0;             //#17290
                        }

                        if ((bool)Country[CountryParameterNames.RestrictRepossessions])
                        {
                            // Restrict repossesions by account status
                            if (txtCurrentStatus.Text != "3" &&
                                txtCurrentStatus.Text != "4" &&
                                txtCurrentStatus.Text != "5" &&
                                txtCurrentStatus.Text != "6")
                            {
                                drpReposession.Enabled = false;
                                drpReposession.SelectedIndex = 1;
                                drpRepoType.Enabled = false;

                                //IP - 15/12/09 - UAT(653) - In the instance where repossession is retricted to status, we stil want to enable the 'Collection' drop down.
                                drpCredReplace.Enabled = true;
                            }
                        }

                        if (acctType == AT.Cash)
                        {
                            drpReposession.Enabled = false;
                            drpReposession.SelectedIndex = 1;
                            drpRepoType.Enabled = false;
                            drpCredReplace.Enabled = true;
                        }
                    }

                    // Returns are not allowed on cash Loan accounts  LW72938 jec 25/11/10
                    if (loanAccount == true)
                    {
                        MessageBox.Show("A Goods Return cannot be performed on a Cash Loan account.", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        InitData(true);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                Function += Finished;
                StopWait();
            }

        }


        public void RoundDataGridValues(DataTable d)
        {
            var c = new CommonObject();

            foreach (DataRow row in d.Rows)
            {
                row[CN.Price] = c.CountryRound(row[CN.Price]);
                row[CN.OrdVal] = c.CountryRound(row[CN.OrdVal]);
            }
        }

        //IP - 13/11/09 - UAT5.2 (653) - Method to reset the item detail fields.
        public void ResetItemDetailFields()
        {
            txtItemNo.Text = "";
            txtQty.Text = "";
            txtReturnItemNo.Text = "";
            txtLocn.Text = "";
            txtReturnLocn.Text = "";
            txtTotalValue.Text = "";
            txtCreditTotal.Text = (0).ToString(DecimalPlaces);

            btnEnter.Enabled = false;
            btnRemove.Enabled = false;

            ((MainForm)FormRoot).statusBar1.Text = "";
        }

        //IP - 25/05/10 - UAT(234) UAT5.2.1.0 Log - Method to check if the entered account
        //is a Home Club Account (HCC Account). If it is, do not load.
        private bool IsHomeClubAcct()
        {
            bool status = true;

            if ((bool)(Country[CountryParameterNames.LoyaltyScheme]))
            {
                decimal amount = 0;
                string acctno = "";
                bool active = false;

                STL.PL.WS3.Loyalty loyaltyinfo = CustomerManager.LoyaltyGetDatabyacctno(txtAccountNo.Text.Trim().Replace("-", ""));
                if (loyaltyinfo.custid != null && loyaltyinfo.custid.Length > 1)
                {
                    CustomerManager.LoyaltyGetCharges(loyaltyinfo.custid, ref acctno, ref amount, ref active);
                    if (acctno == txtAccountNo.Text.Trim().Replace("-", ""))
                    {
                        ShowWarning("Goods Returns on Home Club accounts are not allowed.");
                        txtAccountNo.Text = "";
                        status = false;
                    }
                }
            }
            return status;
        }

        private bool IsWarranty(string category)
        {
            var warCategories = StaticData.Tables[TN.WarrantyCategories] as DataTable;

            if (warCategories == null)
                throw new NullReferenceException("StaticData table 'WarrantyCategories' is null");

            return warCategories.Select(String.Format("Code = '{0}'", category)).Length > 0;
        }

        private bool IsItemInReturnDetails(int itemID)
        {
            var parentExists = false;
            foreach (DataRowView item in ((DataView)dgGoodsToReturn.DataSource))
            {
                if (int.Parse(item.Row["ItemId"].ToString()) == itemID)
                {
                    parentExists = true;
                    break;
                }
            }

            return parentExists;
        }

        private void txtQty_TextChanged(object sender, EventArgs e)
        {

        }

        private void dgGoodsToReturn_Navigate(object sender, NavigateEventArgs ne)
        {

        }
        //CR-2018-13 – Raj – 07 Dec 18 –  Take Formatted Invoice as input for Getting  Account Number 
        public static bool IsAllDigits(string s)
        {
            if (s == "000-00000000000")
            {
                s = s.Replace("-", "");
            }
            foreach (char c in s)
            {
                if (!Char.IsDigit(c))
                    return false;
            }
            return true;
        }

        //CR-2018-13 – Raj – 07 Dec 18 –  Take InvoiceNumber as input for Getting  Account Number 
        private void btnInvoiceSearch_Click(object sender, EventArgs e)
        {
            try
            {
                
                IsAllDigits(txtInvoiceNo.Text);
                //nameOfTextBox.Text.Any( c => c < 48 || c > 57 )
                if (IsAllDigits(txtInvoiceNo.Text)==true )
                {
                    if ((txtInvoiceNo.Text).Length == 14)
                    {
                        if (txtInvoiceNo.Text != "000-00000000000" && txtInvoiceNo.Text != InvoiceNo)
                        {
                            InvoiceNo = txtInvoiceNo.Text;
                        }
                        if (txtInvoiceNo.Text == "000-00000000000")
                        {
                            InvoiceNo = txtInvoiceNo.Text;
                        }
                        else
                        {
                            InvoiceNo = txtInvoiceNo.Text.Replace("-", "");
                        }
                        //Return Accountno to Function LoadGoodsReturnData
                        AccountNo = LoadInvoiceReturnData(InvoiceNo);
                        LoadGoodsReturnData(AccountNo);
                    }
                    else
                    {
                        ShowError("Invalid Invoice Number");
                        txtInvoiceNo.Focus();
                    }
                }
                else {
                    ShowError("Invalid Invoice Number");
                    txtInvoiceNo.Focus();
                }
            }

            catch (Exception ex)
            {
                Catch(ex, Function);
            }            
        }
        //CR-2018-13 – Raj – 07 Dec 18 –  Pass  InvoiceNumber as input for Getting  Account Number 
        public string LoadInvoiceReturnData(string InvoiceNo)
        {            
            AccountNo  = AccountManager.GetInvoiceAccountDetails(InvoiceNo, out Error);
            return AccountNo;

        }
        //CR-2018-13 – Raj – 07 Dec 18 –  To input only Integers
        private void txtInvoiceNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }
    }
}
