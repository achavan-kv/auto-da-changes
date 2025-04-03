using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using STL.Common;
using System.Web.Services.Protocols;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Specialized;
using System.Globalization;
using System.Threading;
using System.Xml;
using STL.PL.WS1;
using STL.PL.WS2;
using STL.PL.WS3;
using STL.PL.WS4;
using STL.PL.WS5;
using STL.Common.Constants.Tags;
using STL.Common.Static;
using STL.Common.Constants.ItemTypes;
using STL.Common.Constants.AccountTypes;
using Crownwood.Magic.Menus;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.Elements;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.TabPageNames;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Blue.Cosacs.Shared;
//using STL.Common.Constants.ScreenModes;
//using STL.Common.Constants.SanctionStages;



namespace STL.PL
{
    /// <summary>
    /// Summary description for FollowUp5_2.
    /// </summary>
    public class FollowUp5_2 : CommonForm
    {
        private ArrayList allocatedAccounts;
        private DataSet accounts;
        private DataView acctView;
        private new string Error = String.Empty;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ToolBar toolBar1;
        private System.Data.DataSet HoldFlagSet;
        private System.Data.DataTable HoldFlags;
        private System.Data.DataColumn HoldFlag;
        private System.Data.DataColumn DateCleared;
        private System.Data.DataColumn ByUser;
        private System.Data.DataSet IncompleteSet;
        private System.Data.DataTable IncompleteAccounts;
        private System.Data.DataColumn AccountNo;
        private System.Data.DataColumn DateOpened;
        private System.Data.DataColumn SalesPerson;
        private System.Data.DataColumn CustName;
        private System.Data.DataColumn dataColumn1;
        private int searchClicked = 0;
        //private Crownwood.Magic.Menus.MenuControl menuMain;
        private System.Data.DataSet dataSet1;
        private Crownwood.Magic.Menus.MenuCommand menuFile;
        private Crownwood.Magic.Menus.MenuCommand menuExit;
        private System.ComponentModel.IContainer components;
        //private int empeeNo = 0;
        private bool staticdataloaded = false;
        StringCollection customercode = new StringCollection();
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox drpAcctAllocation;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox drpAcctLetter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox drpAcctArrears;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox drpAcctActions;
        private System.Windows.Forms.DataGrid dgAccounts;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox drpAcctCodes;
        private System.Windows.Forms.ComboBox drpCodes;
        private System.Windows.Forms.ComboBox drpAcctPoints;
        private System.Windows.Forms.DateTimePicker dateActionFrom;
        private System.Windows.Forms.NumericUpDown proposalUpDown;
        private System.Windows.Forms.DateTimePicker dateActionTo;
        private System.Windows.Forms.ComboBox drpActions;
        private System.Windows.Forms.DateTimePicker dateLetterTo;
        private System.Windows.Forms.CheckBox viewTop;
        private System.Windows.Forms.DateTimePicker dateAllocatedTo;
        private System.Windows.Forms.ComboBox drpEmpName;
        private System.Windows.Forms.DateTimePicker dateAllocatedFrom;
        private System.Windows.Forms.DateTimePicker dateLetterFrom;
        private System.Windows.Forms.NumericUpDown txtArrears;
        private System.Windows.Forms.ComboBox drpLetters;
        private System.Windows.Forms.ComboBox drpEmployeeTypes;
        private System.Windows.Forms.Button btnSearch;
        private StringCollection acctcode = new StringCollection();
        private string codeselection;
        private System.Windows.Forms.Label labelsent;
        private System.Windows.Forms.Label labeltaken;
        private string codes;
        private System.Windows.Forms.ComboBox drpEmployeeTypesResult;
        private System.Windows.Forms.ComboBox drpEmpNameResult;
        private StringCollection staff = new StringCollection();
        private System.Windows.Forms.ComboBox drpEmployeeBranch;
        private System.Windows.Forms.ComboBox drpBranchResult;
        private System.Windows.Forms.Button btnAllocate;
        private System.Windows.Forms.ComboBox drpBranch;
        private System.Windows.Forms.Button btnDeAllocate;
        private System.Windows.Forms.Button btnExcel;
        private System.Windows.Forms.Label loadBranches;
        private System.Windows.Forms.Label showSingleAccount;
        private System.Windows.Forms.Label lDateMoved;
        private System.Windows.Forms.DateTimePicker dtMovedTo;
        private System.Windows.Forms.DateTimePicker dtMovedFrom;
        private System.Windows.Forms.ComboBox drpDateMoved;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox chxActions;
        private System.Windows.Forms.CheckBox chxCharges;
        private System.Windows.Forms.NumericUpDown txtNumActions;
        private System.Windows.Forms.ComboBox drpNumAcctions;
        private System.Windows.Forms.NumericUpDown txtBalances;
        private System.Windows.Forms.ComboBox drpBalances;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private Crownwood.Magic.Controls.TabControl tcAllocated;
        private Crownwood.Magic.Controls.TabPage tpResult;
        private Crownwood.Magic.Controls.TabPage tpQuery;
        private System.Windows.Forms.DateTimePicker dtDateLastPaid;
        private System.Windows.Forms.ComboBox drpDateLastPaid;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.ImageList menuIcons;
        private StringCollection creditstaff = new StringCollection();
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Label lScore;
        private System.Windows.Forms.ToolTip ttFollowUp;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rbAdded;
        private System.Windows.Forms.RadioButton rbDue;
        public StatusBar statusBar1;
        private RadioButton rbCredit;
        private RadioButton rbCash;
        private Crownwood.Magic.Controls.TabPage tabPage1;
        private Button btnClearAcct;
        private Label label8;
        private Label label20;
        private Label label21;
        private AccountTextBox txtAccountNo;
        private Label label22;
        private Button btnAllocateAcct;
        private ComboBox drpEmpNameAcct;
        private ComboBox drpEmployeeTypesAcct;
        private ComboBox drpBranchAcct;
        //private RadioButton rbCash;
        //private RadioButton rbCredit;
        string employee = "";
        string acctNo = "";
        string branchset = "";
        StringCollection acctselectionarrearsCredit = new StringCollection();
        private TextBox textBranchSet;
        private ComboBox drpStatusSelection;
        StringCollection acctselectionarrearsCash = new StringCollection();
        private ComboBox drpAcctStatus;
        private ComboBox drpAcctMinStatus;
        private CheckBox chkServiceFilter;
        private ComboBox drpWorklists;
        private ComboBox drpAreas;
        private Label label24;
        private Label label23;
        StringCollection statusselection = new StringCollection();
        private int m_empeeNo = 0;

        public int empeeNo
        {
            get
            {
                return m_empeeNo;
            }
            set
            {
                m_empeeNo = value;
            }
        }

        //public FollowUp5_2(string acctNo, Form root, Form parent)
        public FollowUp5_2(Form root, Form parent)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            FormRoot = root;
            FormParent = parent;
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile });

            dynamicMenus = new Hashtable();
            HashMenus();
            ApplyRoleRestrictions();

            InitialiseStaticData();

            dateAllocatedFrom.Value = DateTime.Now.AddYears(-1);
            dtMovedFrom.Value = DateTime.Now.AddDays(-7);

            lScore.Visible = drpAcctPoints.Visible = proposalUpDown.Visible = (bool)Country[CountryParameterNames.DisplayScore];

            this.ttFollowUp.SetToolTip(drpAcctAllocation, GetResource("TT_ALLOCATEDFILTER"));
            this.ttFollowUp.SetToolTip(drpAcctStatus, GetResource("TT_STATUSFILTER"));
            this.ttFollowUp.SetToolTip(drpBranch, GetResource("TT_BRANCHFILTER"));
            this.ttFollowUp.SetToolTip(drpAcctArrears, GetResource("TT_ARREARSFILTER"));
            this.ttFollowUp.SetToolTip(drpAcctLetter, GetResource("TT_LETTERFILTER"));
            this.ttFollowUp.SetToolTip(drpAcctActions, GetResource("TT_ACTIONFILTER"));
            this.ttFollowUp.SetToolTip(drpAcctCodes, GetResource("TT_CODEFILTER"));
            this.ttFollowUp.SetToolTip(drpAcctPoints, GetResource("TT_SCOREFILTER"));
            this.ttFollowUp.SetToolTip(drpNumAcctions, GetResource("TT_ACTIONCOUNTFILTER"));
            this.ttFollowUp.SetToolTip(drpBalances, GetResource("TT_BALANCEFILTER"));
            this.ttFollowUp.SetToolTip(drpDateMoved, GetResource("TT_DATEMOVEDFILTER"));
            this.ttFollowUp.SetToolTip(drpDateLastPaid, GetResource("TT_DATEMOVEDFILTER"));
            this.ttFollowUp.SetToolTip(chxActions, GetResource("TT_EMPLOYEEFILTER"));
            this.ttFollowUp.SetToolTip(chxCharges, GetResource("TT_CHARGESFILTER"));
            this.ttFollowUp.SetToolTip(btnRefresh, GetResource("TT_REFRESHFILTER"));
            this.ttFollowUp.SetToolTip(rbAdded, GetResource("TT_ADDEDFILTER")); //jec 67902
            this.ttFollowUp.SetToolTip(rbDue, GetResource("TT_DUEFILTER"));	//jec 67902
            // FA UAT 873 Missing tooltips - but not added to all the fields as some don't need it (commented below)
            this.ttFollowUp.SetToolTip(drpStatusSelection, GetResource("TT_STATUSFILTER"));
            //this.ttFollowUp.SetToolTip(drpWorklists, GetResource("TT_"));
            //this.ttFollowUp.SetToolTip(drpAreas, GetResource("TT_"));
            //this.ttFollowUp.SetToolTip(drpEmployeeTypes, GetResource("TT_"));
            //this.ttFollowUp.SetToolTip(drpEmployeeBranch, GetResource("TT_"));
            //this.ttFollowUp.SetToolTip(drpEmpName, GetResource("TT_"));
            //this.ttFollowUp.SetToolTip(drpActions, GetResource("TT_"));
            //this.ttFollowUp.SetToolTip(drpLetters, GetResource("TT_"));
            //this.ttFollowUp.SetToolTip(drpCodes, GetResource("TT_"));
            dateAllocatedFrom.Visible = false;
            dateAllocatedFrom.Visible = true;


        }

        public FollowUp5_2(TranslationDummy d)
        {
            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile });
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

        /// <summary>
        /// load up branch numbers hopefully from cache
        /// </summary>
        private void InitialiseStaticData()
        {
            Function = "BStaticDataManager::GetDropDownData";
            #region loading up static data drop-downs from database
            StringCollection branchNos = new StringCollection();
            StringCollection acctBranchNos = new StringCollection();
            StringCollection resultsBranchNos = new StringCollection();

            branchNos.Add(GetResource("L_ALL"));
            acctBranchNos.Add(GetResource("L_ALL"));
            resultsBranchNos.Add(GetResource("L_ALL"));

            XmlUtilities xml = new XmlUtilities();
            XmlDocument dropDowns = new XmlDocument();
            dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

            /// <summary>
            /// 
            /// 
            /// </summary>
            /// 

            StringCollection acctselectionallocation = new StringCollection();
            StringCollection employeeselectiontypes = new StringCollection();
            StringCollection employeeresulttypes = new StringCollection();
            StringCollection acctselectionstatus = new StringCollection();
            StringCollection acctselectionletter = new StringCollection();
            StringCollection letter = new StringCollection();
            StringCollection acctselectionaction = new StringCollection();
            StringCollection selectionaction = new StringCollection();
            StringCollection acctselectioncode = new StringCollection();
            StringCollection selectionacctcode = new StringCollection();
            StringCollection selectioncustomercode = new StringCollection();
            StringCollection acctselectionpoints = new StringCollection();
            StringCollection worklists = new StringCollection();

            if (StaticData.Tables[TN.AcctSelectionAllocation] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.AcctSelectionAllocation,
                    new string[] { "AST", "L" }));

            if (StaticData.Tables[TN.WorkList] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.WorkList, null));

            if (StaticData.Tables[TN.EmployeeTypes] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.EmployeeTypes,
                    new string[] { "ET1", "L" }));
            if (StaticData.Tables[TN.AcctSelectionStatus] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.AcctSelectionStatus,
                    new string[] { "ASS", "L" }));
            if (StaticData.Tables[TN.AcctSelectionLetter] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.AcctSelectionLetter,
                    new string[] { "ASL", "L" }));

            if (StaticData.Tables[TN.Letter] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.Letter,
                    new string[] { "LT1", "L" }));
            if (StaticData.Tables[TN.AcctSelectionArrears] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.AcctSelectionArrears,
                    new string[] { "ASR", "L" }));
            if (StaticData.Tables[TN.SelectionAction] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.SelectionAction,
                    new string[] { "FUP", "L" }));

            if (StaticData.Tables[TN.AcctSelectionAction] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.AcctSelectionAction,
                    new string[] { "ASA", "L" }));
            if (StaticData.Tables[TN.AcctSelectionCodes] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.AcctSelectionCodes,
                    new string[] { "ASC", "L" }));
            if (StaticData.Tables[TN.CustomerCodes] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.CustomerCodes,
                    new string[] { "CC1", "L" }));

            if (StaticData.Tables[TN.AccountCodes] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.AccountCodes,
                    new string[] { "AC1", "L" }));
            if (StaticData.Tables[TN.AcctSelectionPoints] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.AcctSelectionPoints,
                    new string[] { "ASP", "L" }));

            if (StaticData.Tables[TN.BranchNumber] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.BranchNumber, null));

            if (dropDowns.DocumentElement.ChildNodes.Count > 0)
            {
                DataSet ds = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out Error);
                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    foreach (DataTable dt in ds.Tables)
                        StaticData.Tables[dt.TableName] = dt;
                }
            }

            worklists.Add("NL - NO LIMITATION");
            foreach (DataRow row in ((DataTable)StaticData.Tables[TN.WorkList]).Rows)
            {
                string str = (string)row.ItemArray[0] + " " + (string)row.ItemArray[1];
                worklists.Add(str.ToUpper());
            }
            drpWorklists.DataSource = worklists;

            CommonForm form = new CommonForm();
            DataSet areaSet = form.SetDataManager.GetSetsForTNameBranch(TN.TNameDeliveryArea, Config.BranchCode, out Error);
            DataTable areaTable = areaSet.Tables[TN.SetsData];
            if (Error.Length > 0)
            {
                form.ShowError(Error);
            }
            else
            {
                StringCollection areaList = new StringCollection();
                areaList.Add(CommonForm.GetResource("L_ALL"));
                foreach (DataRow row in areaTable.Rows)
                {
                    areaList.Add((string)row.ItemArray[0]);
                }
                drpAreas.DataSource = areaList;
            }

            foreach (DataRow row in ((DataTable)StaticData.Tables[TN.AcctSelectionAllocation]).Rows)
            {
                string str = (string)row.ItemArray[0] + " - " + (string)row.ItemArray[1];
                acctselectionallocation.Add(str.ToUpper());
            }
            drpAcctAllocation.DataSource = acctselectionallocation;

            foreach (DataRow row in CollectionsManager.RolesGet((int)CosacsPermissionEnum.Bailiff).Tables[0].Rows)
            {
                string str = string.Format("{0} : {1}", row[0], row[1]);
                employeeselectiontypes.Add(str.ToUpper());
                employeeresulttypes.Add(str.ToUpper());
            }

            // moved to here (jec 28/03/07)
            // drpEmpName was not being populated properly on initial load
            acctBranchNos.Add(CN.BranchSet);
            foreach (DataRow row in ((DataTable)StaticData.Tables[TN.BranchNumber]).Rows)
            {
                branchNos.Add(Convert.ToString(row["branchno"]));
                acctBranchNos.Add(Convert.ToString(row["branchno"]));
                resultsBranchNos.Add(Convert.ToString(row["branchno"]));
            }
            drpEmployeeBranch.DataSource = branchNos;
            drpEmployeeBranch.Text = Config.BranchCode;
            // end move

            drpEmployeeTypes.DataSource = employeeselectiontypes;
            drpEmployeeTypesResult.DataSource = employeeresulttypes;
            drpEmployeeTypesAcct.DataSource = employeeresulttypes;
            StringCollection acctminselectionstatus = new StringCollection();
            foreach (DataRow row in ((DataTable)StaticData.Tables[TN.AcctSelectionStatus]).Rows)
            {
                string str = (string)row.ItemArray[0] + " - " + (string)row.ItemArray[1];
                acctselectionstatus.Add(str.ToUpper());
                acctminselectionstatus.Add(str.ToUpper());
            }
            drpAcctStatus.DataSource = acctselectionstatus;
            drpAcctMinStatus.DataSource = acctminselectionstatus;

            foreach (DataRow row in ((DataTable)StaticData.Tables[TN.AcctSelectionLetter]).Rows)
            {
                string str = (string)row.ItemArray[0] + " - " + (string)row.ItemArray[1];
                acctselectionletter.Add(str.ToUpper());
            }
            drpAcctLetter.DataSource = acctselectionletter;

            foreach (DataRow row in ((DataTable)StaticData.Tables[TN.Letter]).Rows)
            {
                string str = (string)row.ItemArray[0] + " - " + (string)row.ItemArray[1];
                letter.Add(str.ToUpper());
            }
            drpLetters.DataSource = letter;

            foreach (DataRow row in ((DataTable)StaticData.Tables[TN.AcctSelectionArrears]).Rows)
            {
                string str = (string)row.ItemArray[0] + " - " + (string)row.ItemArray[1];
                acctselectionarrearsCredit.Add(str.ToUpper());

                if ((string)row.ItemArray[0] != "A<I" && (string)row.ItemArray[0] != "A=I" &&
                    (string)row.ItemArray[0] != "A>I")
                {
                    string cashStr = (string)row.ItemArray[0] + " - " + (string)row.ItemArray[1];
                    acctselectionarrearsCash.Add(cashStr.ToUpper());
                }
            }

            drpAcctArrears.DataSource = acctselectionarrearsCredit;
            statusselection.Add("NL -No Limitation");
            statusselection.Add("=");
            statusselection.Add(GetResource("TT_BETWEEN"));
            drpStatusSelection.DataSource = statusselection;

            foreach (DataRow row in ((DataTable)StaticData.Tables[TN.AcctSelectionAction]).Rows)
            {
                string str = (string)row.ItemArray[0] + " - " + (string)row.ItemArray[1];
                acctselectionaction.Add(str.ToUpper());
            }

            drpAcctActions.DataSource = acctselectionaction;
            foreach (DataRow row in ((DataTable)StaticData.Tables[TN.SelectionAction]).Rows)
            {
                string str = (string)row.ItemArray[0] + " - " + (string)row.ItemArray[1];
                selectionaction.Add(str.ToUpper());
            }
            drpActions.DataSource = selectionaction;
            //                
            foreach (DataRow row in ((DataTable)StaticData.Tables[TN.AcctSelectionCodes]).Rows)
            {
                string str = (string)row.ItemArray[0] + " - " + (string)row.ItemArray[1];
                acctselectioncode.Add(str.ToUpper());
            }
            drpAcctCodes.DataSource = acctselectioncode;
            /*next two only required if chosen*/
            foreach (DataRow row in ((DataTable)StaticData.Tables[TN.CustomerCodes]).Rows)
            {
                string str = (string)row.ItemArray[0] + " - " + (string)row.ItemArray[1];
                customercode.Add(str.ToUpper());
            }
            foreach (DataRow row in ((DataTable)StaticData.Tables[TN.AccountCodes]).Rows)
            {
                string str = (string)row.ItemArray[0] + " - " + (string)row.ItemArray[1];
                acctcode.Add(str.ToUpper());
            }

            foreach (DataRow row in ((DataTable)StaticData.Tables[TN.AcctSelectionPoints]).Rows)
            {
                string str = (string)row.ItemArray[0] + " - " + (string)row.ItemArray[1];
                acctselectionpoints.Add(str.ToUpper());
            }

            drpAcctPoints.DataSource = acctselectionpoints;
            /*
            // moved from here (jec 28/03/07)
            // drpEmpName was not being populated properly on initial load
             
            acctBranchNos.Add(CN.BranchSet);
            
            foreach (DataRow row in ((DataTable)StaticData.Tables[TN.BranchNumber]).Rows)
            {
                branchNos.Add(Convert.ToString(row["branchno"]));
                acctBranchNos.Add(Convert.ToString(row["branchno"]));
                resultsBranchNos.Add(Convert.ToString(row["branchno"]));
            }
             
            drpEmployeeBranch.DataSource = branchNos;
            drpEmployeeBranch.Text = Config.BranchCode;
            */

            drpBranch.DataSource = acctBranchNos;
            drpBranch.Text = Config.BranchCode;


            drpBranchResult.DataSource = resultsBranchNos;
            drpBranchResult.Text = Config.BranchCode;

            drpBranchAcct.DataSource = branchNos;
            drpBranchAcct.Text = Config.BranchCode;


            //int branch = Convert.ToInt32(Config.BranchCode);
            if (loadBranches.Enabled)
                drpBranch.Enabled = true;
            else
                drpBranch.Enabled = false;

            //CR852
            if (showSingleAccount.Enabled == false)
            {
                tcAllocated.TabPages.Remove(tabPage1);
            }

            drpNumAcctions.SelectedIndex = 0;
            drpBalances.SelectedIndex = 0;
            drpDateMoved.SelectedIndex = 0;
            drpDateLastPaid.SelectedIndex = 0;

            staticdataloaded = true;
            #endregion
        }
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FollowUp5_2));
            this.IncompleteAccounts = new System.Data.DataTable();
            this.dataColumn1 = new System.Data.DataColumn();
            this.AccountNo = new System.Data.DataColumn();
            this.DateOpened = new System.Data.DataColumn();
            this.SalesPerson = new System.Data.DataColumn();
            this.CustName = new System.Data.DataColumn();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.statusBar1 = new System.Windows.Forms.StatusBar();
            this.tcAllocated = new Crownwood.Magic.Controls.TabControl();
            this.tpQuery = new Crownwood.Magic.Controls.TabPage();
            this.label24 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.drpWorklists = new System.Windows.Forms.ComboBox();
            this.drpAreas = new System.Windows.Forms.ComboBox();
            this.chkServiceFilter = new System.Windows.Forms.CheckBox();
            this.drpAcctStatus = new System.Windows.Forms.ComboBox();
            this.drpAcctMinStatus = new System.Windows.Forms.ComboBox();
            this.drpStatusSelection = new System.Windows.Forms.ComboBox();
            this.textBranchSet = new System.Windows.Forms.TextBox();
            this.rbCash = new System.Windows.Forms.RadioButton();
            this.rbCredit = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbDue = new System.Windows.Forms.RadioButton();
            this.rbAdded = new System.Windows.Forms.RadioButton();
            this.btnClear = new System.Windows.Forms.Button();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.dtDateLastPaid = new System.Windows.Forms.DateTimePicker();
            this.drpDateLastPaid = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.chxActions = new System.Windows.Forms.CheckBox();
            this.chxCharges = new System.Windows.Forms.CheckBox();
            this.txtNumActions = new System.Windows.Forms.NumericUpDown();
            this.drpNumAcctions = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtBalances = new System.Windows.Forms.NumericUpDown();
            this.drpBalances = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.lDateMoved = new System.Windows.Forms.Label();
            this.dtMovedTo = new System.Windows.Forms.DateTimePicker();
            this.dtMovedFrom = new System.Windows.Forms.DateTimePicker();
            this.drpDateMoved = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.loadBranches = new System.Windows.Forms.Label();
            this.showSingleAccount = new System.Windows.Forms.Label();
            this.drpBranch = new System.Windows.Forms.ComboBox();
            this.drpEmployeeBranch = new System.Windows.Forms.ComboBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.drpEmployeeTypes = new System.Windows.Forms.ComboBox();
            this.drpLetters = new System.Windows.Forms.ComboBox();
            this.txtArrears = new System.Windows.Forms.NumericUpDown();
            this.dateLetterFrom = new System.Windows.Forms.DateTimePicker();
            this.dateAllocatedFrom = new System.Windows.Forms.DateTimePicker();
            this.drpEmpName = new System.Windows.Forms.ComboBox();
            this.dateAllocatedTo = new System.Windows.Forms.DateTimePicker();
            this.viewTop = new System.Windows.Forms.CheckBox();
            this.dateLetterTo = new System.Windows.Forms.DateTimePicker();
            this.drpActions = new System.Windows.Forms.ComboBox();
            this.labeltaken = new System.Windows.Forms.Label();
            this.dateActionTo = new System.Windows.Forms.DateTimePicker();
            this.proposalUpDown = new System.Windows.Forms.NumericUpDown();
            this.dateActionFrom = new System.Windows.Forms.DateTimePicker();
            this.drpAcctPoints = new System.Windows.Forms.ComboBox();
            this.drpCodes = new System.Windows.Forms.ComboBox();
            this.drpAcctCodes = new System.Windows.Forms.ComboBox();
            this.lScore = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.labelsent = new System.Windows.Forms.Label();
            this.drpAcctActions = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.drpAcctArrears = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.drpAcctLetter = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.drpAcctAllocation = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tpResult = new Crownwood.Magic.Controls.TabPage();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.menuIcons = new System.Windows.Forms.ImageList(this.components);
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.btnExcel = new System.Windows.Forms.Button();
            this.btnDeAllocate = new System.Windows.Forms.Button();
            this.btnAllocate = new System.Windows.Forms.Button();
            this.drpEmpNameResult = new System.Windows.Forms.ComboBox();
            this.drpEmployeeTypesResult = new System.Windows.Forms.ComboBox();
            this.drpBranchResult = new System.Windows.Forms.ComboBox();
            this.dgAccounts = new System.Windows.Forms.DataGrid();
            this.tabPage1 = new Crownwood.Magic.Controls.TabPage();
            this.btnAllocateAcct = new System.Windows.Forms.Button();
            this.drpEmpNameAcct = new System.Windows.Forms.ComboBox();
            this.drpEmployeeTypesAcct = new System.Windows.Forms.ComboBox();
            this.drpBranchAcct = new System.Windows.Forms.ComboBox();
            this.label22 = new System.Windows.Forms.Label();
            this.txtAccountNo = new STL.PL.AccountTextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.btnClearAcct = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.toolBar1 = new System.Windows.Forms.ToolBar();
            this.HoldFlags = new System.Data.DataTable();
            this.HoldFlag = new System.Data.DataColumn();
            this.DateCleared = new System.Data.DataColumn();
            this.ByUser = new System.Data.DataColumn();
            this.HoldFlagSet = new System.Data.DataSet();
            this.IncompleteSet = new System.Data.DataSet();
            this.dataSet1 = new System.Data.DataSet();
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            this.ttFollowUp = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.IncompleteAccounts)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.tcAllocated.SuspendLayout();
            this.tpQuery.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtNumActions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBalances)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtArrears)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.proposalUpDown)).BeginInit();
            this.tpResult.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgAccounts)).BeginInit();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.HoldFlags)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.HoldFlagSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.IncompleteSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).BeginInit();
            this.SuspendLayout();
            // 
            // IncompleteAccounts
            // 
            this.IncompleteAccounts.Columns.AddRange(new System.Data.DataColumn[] {
            this.dataColumn1});
            this.IncompleteAccounts.TableName = "IncompleteAccounts";
            // 
            // dataColumn1
            // 
            this.dataColumn1.ColumnName = "AccountNo";
            // 
            // AccountNo
            // 
            this.AccountNo.ColumnName = "AccountNo";
            this.AccountNo.MaxLength = 15;
            // 
            // DateOpened
            // 
            this.DateOpened.ColumnName = "DateOpened";
            this.DateOpened.MaxLength = 20;
            // 
            // SalesPerson
            // 
            this.SalesPerson.ColumnName = "SalesPerson";
            this.SalesPerson.MaxLength = 20;
            // 
            // CustName
            // 
            this.CustName.ColumnName = "Name";
            this.CustName.MaxLength = 20;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.statusBar1);
            this.groupBox1.Controls.Add(this.tcAllocated);
            this.groupBox1.Location = new System.Drawing.Point(8, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(776, 472);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            // 
            // statusBar1
            // 
            this.statusBar1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.statusBar1.Location = new System.Drawing.Point(3, 449);
            this.statusBar1.Name = "statusBar1";
            this.statusBar1.Size = new System.Drawing.Size(770, 20);
            this.statusBar1.TabIndex = 53;
            // 
            // tcAllocated
            // 
            this.tcAllocated.IDEPixelArea = true;
            this.tcAllocated.Location = new System.Drawing.Point(16, 16);
            this.tcAllocated.Name = "tcAllocated";
            this.tcAllocated.PositionTop = true;
            this.tcAllocated.SelectedIndex = 1;
            this.tcAllocated.SelectedTab = this.tpResult;
            this.tcAllocated.Size = new System.Drawing.Size(736, 440);
            this.tcAllocated.TabIndex = 52;
            this.tcAllocated.TabPages.AddRange(new Crownwood.Magic.Controls.TabPage[] {
            this.tpQuery,
            this.tpResult,
            this.tabPage1});
            this.tcAllocated.SelectionChanged += new System.EventHandler(this.tcAllocated_SelectionChanged);
            // 
            // tpQuery
            // 
            this.tpQuery.Controls.Add(this.label24);
            this.tpQuery.Controls.Add(this.label23);
            this.tpQuery.Controls.Add(this.drpWorklists);
            this.tpQuery.Controls.Add(this.drpAreas);
            this.tpQuery.Controls.Add(this.chkServiceFilter);
            this.tpQuery.Controls.Add(this.drpAcctStatus);
            this.tpQuery.Controls.Add(this.drpAcctMinStatus);
            this.tpQuery.Controls.Add(this.drpStatusSelection);
            this.tpQuery.Controls.Add(this.textBranchSet);
            this.tpQuery.Controls.Add(this.rbCash);
            this.tpQuery.Controls.Add(this.rbCredit);
            this.tpQuery.Controls.Add(this.groupBox2);
            this.tpQuery.Controls.Add(this.btnClear);
            this.tpQuery.Controls.Add(this.label19);
            this.tpQuery.Controls.Add(this.label18);
            this.tpQuery.Controls.Add(this.label17);
            this.tpQuery.Controls.Add(this.dtDateLastPaid);
            this.tpQuery.Controls.Add(this.drpDateLastPaid);
            this.tpQuery.Controls.Add(this.label13);
            this.tpQuery.Controls.Add(this.chxActions);
            this.tpQuery.Controls.Add(this.chxCharges);
            this.tpQuery.Controls.Add(this.txtNumActions);
            this.tpQuery.Controls.Add(this.drpNumAcctions);
            this.tpQuery.Controls.Add(this.label11);
            this.tpQuery.Controls.Add(this.txtBalances);
            this.tpQuery.Controls.Add(this.drpBalances);
            this.tpQuery.Controls.Add(this.label9);
            this.tpQuery.Controls.Add(this.lDateMoved);
            this.tpQuery.Controls.Add(this.dtMovedTo);
            this.tpQuery.Controls.Add(this.dtMovedFrom);
            this.tpQuery.Controls.Add(this.drpDateMoved);
            this.tpQuery.Controls.Add(this.label10);
            this.tpQuery.Controls.Add(this.drpBranch);
            this.tpQuery.Controls.Add(this.drpEmployeeBranch);
            this.tpQuery.Controls.Add(this.btnSearch);
            this.tpQuery.Controls.Add(this.drpEmployeeTypes);
            this.tpQuery.Controls.Add(this.drpLetters);
            this.tpQuery.Controls.Add(this.txtArrears);
            this.tpQuery.Controls.Add(this.dateLetterFrom);
            this.tpQuery.Controls.Add(this.dateAllocatedFrom);
            this.tpQuery.Controls.Add(this.drpEmpName);
            this.tpQuery.Controls.Add(this.dateAllocatedTo);
            this.tpQuery.Controls.Add(this.viewTop);
            this.tpQuery.Controls.Add(this.dateLetterTo);
            this.tpQuery.Controls.Add(this.drpActions);
            this.tpQuery.Controls.Add(this.labeltaken);
            this.tpQuery.Controls.Add(this.dateActionTo);
            this.tpQuery.Controls.Add(this.proposalUpDown);
            this.tpQuery.Controls.Add(this.dateActionFrom);
            this.tpQuery.Controls.Add(this.drpAcctPoints);
            this.tpQuery.Controls.Add(this.drpCodes);
            this.tpQuery.Controls.Add(this.drpAcctCodes);
            this.tpQuery.Controls.Add(this.lScore);
            this.tpQuery.Controls.Add(this.label12);
            this.tpQuery.Controls.Add(this.labelsent);
            this.tpQuery.Controls.Add(this.drpAcctActions);
            this.tpQuery.Controls.Add(this.label7);
            this.tpQuery.Controls.Add(this.drpAcctArrears);
            this.tpQuery.Controls.Add(this.label6);
            this.tpQuery.Controls.Add(this.label1);
            this.tpQuery.Controls.Add(this.drpAcctLetter);
            this.tpQuery.Controls.Add(this.label3);
            this.tpQuery.Controls.Add(this.label5);
            this.tpQuery.Controls.Add(this.drpAcctAllocation);
            this.tpQuery.Controls.Add(this.label4);
            this.tpQuery.Controls.Add(this.label2);
            this.tpQuery.Controls.Add(this.loadBranches);
            this.tpQuery.Controls.Add(this.showSingleAccount);
            this.tpQuery.Location = new System.Drawing.Point(0, 25);
            this.tpQuery.Name = "tpQuery";
            this.tpQuery.Selected = false;
            this.tpQuery.Size = new System.Drawing.Size(736, 415);
            this.tpQuery.TabIndex = 0;
            this.tpQuery.Title = "Query";
            this.tpQuery.PropertyChanged += new Crownwood.Magic.Controls.TabPage.PropChangeHandler(this.tabPage1_PropertyChanged);
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(3, 17);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(61, 15);
            this.label24.TabIndex = 126;
            this.label24.Text = "Worklists :";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(274, 17);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(42, 15);
            this.label23.TabIndex = 125;
            this.label23.Text = "Areas :";
            // 
            // drpWorklists
            // 
            this.drpWorklists.FormattingEnabled = true;
            this.drpWorklists.Location = new System.Drawing.Point(60, 14);
            this.drpWorklists.Name = "drpWorklists";
            this.drpWorklists.Size = new System.Drawing.Size(180, 23);
            this.drpWorklists.TabIndex = 124;
            // 
            // drpAreas
            // 
            this.drpAreas.FormattingEnabled = true;
            this.drpAreas.Location = new System.Drawing.Point(322, 14);
            this.drpAreas.Name = "drpAreas";
            this.drpAreas.Size = new System.Drawing.Size(128, 23);
            this.drpAreas.TabIndex = 123;
            // 
            // chkServiceFilter
            // 
            this.chkServiceFilter.AutoSize = true;
            this.chkServiceFilter.Location = new System.Drawing.Point(368, 200);
            this.chkServiceFilter.Name = "chkServiceFilter";
            this.chkServiceFilter.Size = new System.Drawing.Size(285, 19);
            this.chkServiceFilter.TabIndex = 119;
            this.chkServiceFilter.Text = "Restrict To Accounts With Open Service Requests";
            this.chkServiceFilter.UseVisualStyleBackColor = true;
            // 
            // drpAcctStatus
            // 
            this.drpAcctStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpAcctStatus.DropDownWidth = 300;
            this.drpAcctStatus.ItemHeight = 15;
            this.drpAcctStatus.Items.AddRange(new object[] {
            "",
            "No Limitation",
            "Between"});
            this.drpAcctStatus.Location = new System.Drawing.Point(304, 126);
            this.drpAcctStatus.Name = "drpAcctStatus";
            this.drpAcctStatus.Size = new System.Drawing.Size(31, 23);
            this.drpAcctStatus.TabIndex = 56;
            this.drpAcctStatus.SelectedIndexChanged += new System.EventHandler(this.drpAcctStatus_SelectedIndexChanged);
            // 
            // drpAcctMinStatus
            // 
            this.drpAcctMinStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpAcctMinStatus.DropDownWidth = 300;
            this.drpAcctMinStatus.ItemHeight = 15;
            this.drpAcctMinStatus.Items.AddRange(new object[] {
            "",
            "No Limitation",
            "Between"});
            this.drpAcctMinStatus.Location = new System.Drawing.Point(250, 126);
            this.drpAcctMinStatus.Name = "drpAcctMinStatus";
            this.drpAcctMinStatus.Size = new System.Drawing.Size(31, 23);
            this.drpAcctMinStatus.TabIndex = 118;
            this.drpAcctMinStatus.SelectedIndexChanged += new System.EventHandler(this.drpAcctMinStatus_SelectedIndexChanged);
            // 
            // drpStatusSelection
            // 
            this.drpStatusSelection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpStatusSelection.Items.AddRange(new object[] {
            "> 0",
            "<",
            ">"});
            this.drpStatusSelection.Location = new System.Drawing.Point(60, 126);
            this.drpStatusSelection.Name = "drpStatusSelection";
            this.drpStatusSelection.Size = new System.Drawing.Size(184, 23);
            this.drpStatusSelection.TabIndex = 117;
            this.drpStatusSelection.SelectedIndexChanged += new System.EventHandler(this.drpStatusSelection_SelectedIndexChanged);
            // 
            // textBranchSet
            // 
            this.textBranchSet.Location = new System.Drawing.Point(483, 126);
            this.textBranchSet.Name = "textBranchSet";
            this.textBranchSet.ReadOnly = true;
            this.textBranchSet.Size = new System.Drawing.Size(80, 23);
            this.textBranchSet.TabIndex = 116;
            // 
            // rbCash
            // 
            this.rbCash.Location = new System.Drawing.Point(571, 119);
            this.rbCash.Name = "rbCash";
            this.rbCash.Size = new System.Drawing.Size(60, 24);
            this.rbCash.TabIndex = 115;
            this.rbCash.TabStop = true;
            this.rbCash.Text = "Cash";
            this.rbCash.Click += new System.EventHandler(this.rbCash_Click);
            // 
            // rbCredit
            // 
            this.rbCredit.Checked = true;
            this.rbCredit.Location = new System.Drawing.Point(570, 89);
            this.rbCredit.Name = "rbCredit";
            this.rbCredit.Size = new System.Drawing.Size(60, 24);
            this.rbCredit.TabIndex = 114;
            this.rbCredit.TabStop = true;
            this.rbCredit.Text = "Credit";
            this.rbCredit.Click += new System.EventHandler(this.rbCredit_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbDue);
            this.groupBox2.Controls.Add(this.rbAdded);
            this.groupBox2.Location = new System.Drawing.Point(670, 280);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(58, 60);
            this.groupBox2.TabIndex = 113;
            this.groupBox2.TabStop = false;
            // 
            // rbDue
            // 
            this.rbDue.Location = new System.Drawing.Point(3, 32);
            this.rbDue.Name = "rbDue";
            this.rbDue.Size = new System.Drawing.Size(104, 24);
            this.rbDue.TabIndex = 1;
            this.rbDue.Text = "Due";
            // 
            // rbAdded
            // 
            this.rbAdded.Checked = true;
            this.rbAdded.Location = new System.Drawing.Point(3, 8);
            this.rbAdded.Name = "rbAdded";
            this.rbAdded.Size = new System.Drawing.Size(104, 24);
            this.rbAdded.TabIndex = 0;
            this.rbAdded.TabStop = true;
            this.rbAdded.Text = "Added";
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(648, 89);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(72, 24);
            this.btnClear.TabIndex = 110;
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // label19
            // 
            this.label19.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.label19.Location = new System.Drawing.Point(544, 38);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(96, 16);
            this.label19.TabIndex = 109;
            this.label19.Text = "Employee Name";
            // 
            // label18
            // 
            this.label18.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.label18.Location = new System.Drawing.Point(464, 38);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(74, 16);
            this.label18.TabIndex = 108;
            this.label18.Text = "Emp. Branch";
            // 
            // label17
            // 
            this.label17.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.label17.Location = new System.Drawing.Point(322, 38);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(96, 16);
            this.label17.TabIndex = 107;
            this.label17.Text = "Employee Type";
            // 
            // dtDateLastPaid
            // 
            this.dtDateLastPaid.CustomFormat = "dd MMM yyyy";
            this.dtDateLastPaid.Enabled = false;
            this.dtDateLastPaid.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtDateLastPaid.Location = new System.Drawing.Point(570, 238);
            this.dtDateLastPaid.Name = "dtDateLastPaid";
            this.dtDateLastPaid.Size = new System.Drawing.Size(96, 23);
            this.dtDateLastPaid.TabIndex = 106;
            // 
            // drpDateLastPaid
            // 
            this.drpDateLastPaid.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpDateLastPaid.Items.AddRange(new object[] {
            "NR - No Restriction",
            ">",
            "<"});
            this.drpDateLastPaid.Location = new System.Drawing.Point(468, 239);
            this.drpDateLastPaid.Name = "drpDateLastPaid";
            this.drpDateLastPaid.Size = new System.Drawing.Size(96, 23);
            this.drpDateLastPaid.TabIndex = 105;
            this.drpDateLastPaid.SelectedIndexChanged += new System.EventHandler(this.drpDateLastPaid_SelectedIndexChanged);
            // 
            // label13
            // 
            this.label13.Location = new System.Drawing.Point(370, 228);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(96, 32);
            this.label13.TabIndex = 104;
            this.label13.Text = "Limit Date Last Paid To:";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // chxActions
            // 
            this.chxActions.Location = new System.Drawing.Point(368, 174);
            this.chxActions.Name = "chxActions";
            this.chxActions.Size = new System.Drawing.Size(216, 24);
            this.chxActions.TabIndex = 103;
            this.chxActions.Text = "Restrict To Allocated Employee";
            // 
            // chxCharges
            // 
            this.chxCharges.Checked = true;
            this.chxCharges.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chxCharges.Location = new System.Drawing.Point(368, 155);
            this.chxCharges.Name = "chxCharges";
            this.chxCharges.Size = new System.Drawing.Size(128, 24);
            this.chxCharges.TabIndex = 102;
            this.chxCharges.Text = "Exclude Charges";
            // 
            // txtNumActions
            // 
            this.txtNumActions.Enabled = false;
            this.txtNumActions.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.txtNumActions.Location = new System.Drawing.Point(250, 202);
            this.txtNumActions.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.txtNumActions.Minimum = new decimal(new int[] {
            1410065407,
            2,
            0,
            -2147483648});
            this.txtNumActions.Name = "txtNumActions";
            this.txtNumActions.Size = new System.Drawing.Size(56, 23);
            this.txtNumActions.TabIndex = 101;
            // 
            // drpNumAcctions
            // 
            this.drpNumAcctions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpNumAcctions.Items.AddRange(new object[] {
            "NR - No Restriction",
            "=",
            ">",
            "<"});
            this.drpNumAcctions.Location = new System.Drawing.Point(60, 202);
            this.drpNumAcctions.Name = "drpNumAcctions";
            this.drpNumAcctions.Size = new System.Drawing.Size(184, 23);
            this.drpNumAcctions.TabIndex = 100;
            this.drpNumAcctions.SelectedIndexChanged += new System.EventHandler(this.drpNumAcctions_SelectedIndexChanged);
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(8, 191);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(64, 32);
            this.label11.TabIndex = 99;
            this.label11.Text = "No. Of Actions:";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtBalances
            // 
            this.txtBalances.DecimalPlaces = 2;
            this.txtBalances.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txtBalances.Location = new System.Drawing.Point(250, 240);
            this.txtBalances.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.txtBalances.Minimum = new decimal(new int[] {
            1410065407,
            2,
            0,
            -2147483648});
            this.txtBalances.Name = "txtBalances";
            this.txtBalances.Size = new System.Drawing.Size(56, 23);
            this.txtBalances.TabIndex = 98;
            // 
            // drpBalances
            // 
            this.drpBalances.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpBalances.Items.AddRange(new object[] {
            "NR - No Restriction",
            ">",
            "<"});
            this.drpBalances.Location = new System.Drawing.Point(60, 240);
            this.drpBalances.Name = "drpBalances";
            this.drpBalances.Size = new System.Drawing.Size(184, 23);
            this.drpBalances.TabIndex = 97;
            this.drpBalances.SelectedIndexChanged += new System.EventHandler(this.drpBalances_SelectedIndexChanged);
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(8, 236);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(64, 24);
            this.label9.TabIndex = 96;
            this.label9.Text = "Balances:";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lDateMoved
            // 
            this.lDateMoved.Location = new System.Drawing.Point(415, 353);
            this.lDateMoved.Name = "lDateMoved";
            this.lDateMoved.Size = new System.Drawing.Size(51, 16);
            this.lDateMoved.TabIndex = 94;
            this.lDateMoved.Text = "Between";
            this.lDateMoved.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dtMovedTo
            // 
            this.dtMovedTo.CustomFormat = "dd MMM yyyy";
            this.dtMovedTo.Enabled = false;
            this.dtMovedTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtMovedTo.Location = new System.Drawing.Point(570, 352);
            this.dtMovedTo.Name = "dtMovedTo";
            this.dtMovedTo.Size = new System.Drawing.Size(96, 23);
            this.dtMovedTo.TabIndex = 93;
            // 
            // dtMovedFrom
            // 
            this.dtMovedFrom.CustomFormat = "dd MMM yyyy";
            this.dtMovedFrom.Enabled = false;
            this.dtMovedFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtMovedFrom.Location = new System.Drawing.Point(472, 352);
            this.dtMovedFrom.Name = "dtMovedFrom";
            this.dtMovedFrom.Size = new System.Drawing.Size(96, 23);
            this.dtMovedFrom.TabIndex = 92;
            // 
            // drpDateMoved
            // 
            this.drpDateMoved.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpDateMoved.DropDownWidth = 200;
            this.drpDateMoved.Items.AddRange(new object[] {
            "NL - No Limitation ",
            "AM - Accounts Moving Into Arrears",
            "NA - New Accounts In Arrears"});
            this.drpDateMoved.Location = new System.Drawing.Point(72, 352);
            this.drpDateMoved.Name = "drpDateMoved";
            this.drpDateMoved.Size = new System.Drawing.Size(168, 23);
            this.drpDateMoved.TabIndex = 91;
            this.drpDateMoved.SelectedIndexChanged += new System.EventHandler(this.drpDateMoved_SelectedIndexChanged);
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(8, 334);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(72, 40);
            this.label10.TabIndex = 90;
            this.label10.Text = "Date Moved To Higher Arrears:";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // loadBranches
            // 
            this.loadBranches.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.loadBranches.Enabled = false;
            this.loadBranches.Location = new System.Drawing.Point(592, 60);
            this.loadBranches.Name = "loadBranches";
            this.loadBranches.Size = new System.Drawing.Size(38, 16);
            this.loadBranches.TabIndex = 89;
            this.loadBranches.Visible = false;
            // 
            // showSingleAccount
            // 
            this.showSingleAccount.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.showSingleAccount.Enabled = false;
            this.showSingleAccount.Location = new System.Drawing.Point(593, 60);
            this.showSingleAccount.Name = "showSingleAccount";
            this.showSingleAccount.Size = new System.Drawing.Size(38, 16);
            this.showSingleAccount.TabIndex = 122;
            this.showSingleAccount.Visible = false;
            // 
            // drpBranch
            // 
            this.drpBranch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpBranch.Location = new System.Drawing.Point(397, 126);
            this.drpBranch.Name = "drpBranch";
            this.drpBranch.Size = new System.Drawing.Size(80, 23);
            this.drpBranch.TabIndex = 88;
            this.drpBranch.SelectedIndexChanged += new System.EventHandler(this.drpBranch_SelectedIndexChanged);
            // 
            // drpEmployeeBranch
            // 
            this.drpEmployeeBranch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpEmployeeBranch.Location = new System.Drawing.Point(467, 57);
            this.drpEmployeeBranch.Name = "drpEmployeeBranch";
            this.drpEmployeeBranch.Size = new System.Drawing.Size(64, 23);
            this.drpEmployeeBranch.TabIndex = 87;
            this.drpEmployeeBranch.SelectedIndexChanged += new System.EventHandler(this.drpEmployeeBranch_SelectedIndexChanged);
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(648, 119);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(72, 24);
            this.btnSearch.TabIndex = 86;
            this.btnSearch.Text = "Run";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click_1);
            // 
            // drpEmployeeTypes
            // 
            this.drpEmployeeTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpEmployeeTypes.Enabled = false;
            this.drpEmployeeTypes.Location = new System.Drawing.Point(322, 57);
            this.drpEmployeeTypes.Name = "drpEmployeeTypes";
            this.drpEmployeeTypes.Size = new System.Drawing.Size(128, 23);
            this.drpEmployeeTypes.TabIndex = 85;
            this.drpEmployeeTypes.SelectedIndexChanged += new System.EventHandler(this.drpEmployeeTypes_SelectedIndexChanged);
            // 
            // drpLetters
            // 
            this.drpLetters.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpLetters.Enabled = false;
            this.drpLetters.Location = new System.Drawing.Point(250, 280);
            this.drpLetters.Name = "drpLetters";
            this.drpLetters.Size = new System.Drawing.Size(168, 23);
            this.drpLetters.TabIndex = 84;
            // 
            // txtArrears
            // 
            this.txtArrears.DecimalPlaces = 2;
            this.txtArrears.Enabled = false;
            this.txtArrears.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txtArrears.Location = new System.Drawing.Point(250, 165);
            this.txtArrears.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.txtArrears.Minimum = new decimal(new int[] {
            1410065407,
            2,
            0,
            -2147483648});
            this.txtArrears.Name = "txtArrears";
            this.txtArrears.Size = new System.Drawing.Size(56, 23);
            this.txtArrears.TabIndex = 83;
            // 
            // dateLetterFrom
            // 
            this.dateLetterFrom.CustomFormat = "dd MMM yyyy";
            this.dateLetterFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateLetterFrom.Location = new System.Drawing.Point(472, 279);
            this.dateLetterFrom.Name = "dateLetterFrom";
            this.dateLetterFrom.Size = new System.Drawing.Size(96, 23);
            this.dateLetterFrom.TabIndex = 82;
            // 
            // dateAllocatedFrom
            // 
            this.dateAllocatedFrom.CustomFormat = "dd MMM yyyy";
            this.dateAllocatedFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateAllocatedFrom.Location = new System.Drawing.Point(173, 84);
            this.dateAllocatedFrom.Name = "dateAllocatedFrom";
            this.dateAllocatedFrom.Size = new System.Drawing.Size(96, 23);
            this.dateAllocatedFrom.TabIndex = 81;
            // 
            // drpEmpName
            // 
            this.drpEmpName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpEmpName.Enabled = false;
            this.drpEmpName.Location = new System.Drawing.Point(544, 57);
            this.drpEmpName.Name = "drpEmpName";
            this.drpEmpName.Size = new System.Drawing.Size(184, 23);
            this.drpEmpName.TabIndex = 80;
            this.drpEmpName.SelectedIndexChanged += new System.EventHandler(this.drpEmpName_SelectedIndexChanged);
            // 
            // dateAllocatedTo
            // 
            this.dateAllocatedTo.CustomFormat = "dd MMM yyyy";
            this.dateAllocatedTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateAllocatedTo.Location = new System.Drawing.Point(292, 85);
            this.dateAllocatedTo.Name = "dateAllocatedTo";
            this.dateAllocatedTo.Size = new System.Drawing.Size(96, 23);
            this.dateAllocatedTo.TabIndex = 79;
            // 
            // viewTop
            // 
            this.viewTop.Checked = true;
            this.viewTop.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewTop.Location = new System.Drawing.Point(648, 159);
            this.viewTop.Name = "viewTop";
            this.viewTop.Size = new System.Drawing.Size(72, 16);
            this.viewTop.TabIndex = 78;
            this.viewTop.Text = "Top 500";
            // 
            // dateLetterTo
            // 
            this.dateLetterTo.CustomFormat = "dd MMM yyyy";
            this.dateLetterTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateLetterTo.Location = new System.Drawing.Point(571, 279);
            this.dateLetterTo.Name = "dateLetterTo";
            this.dateLetterTo.Size = new System.Drawing.Size(96, 23);
            this.dateLetterTo.TabIndex = 77;
            // 
            // drpActions
            // 
            this.drpActions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpActions.DropDownWidth = 200;
            this.drpActions.Location = new System.Drawing.Point(250, 310);
            this.drpActions.Name = "drpActions";
            this.drpActions.Size = new System.Drawing.Size(168, 23);
            this.drpActions.TabIndex = 76;
            // 
            // labeltaken
            // 
            this.labeltaken.Location = new System.Drawing.Point(418, 312);
            this.labeltaken.Name = "labeltaken";
            this.labeltaken.Size = new System.Drawing.Size(53, 16);
            this.labeltaken.TabIndex = 75;
            this.labeltaken.Text = "Between";
            this.labeltaken.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dateActionTo
            // 
            this.dateActionTo.CustomFormat = "dd MMM yyyy";
            this.dateActionTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateActionTo.Location = new System.Drawing.Point(570, 309);
            this.dateActionTo.Name = "dateActionTo";
            this.dateActionTo.Size = new System.Drawing.Size(96, 23);
            this.dateActionTo.TabIndex = 74;
            // 
            // proposalUpDown
            // 
            this.proposalUpDown.Enabled = false;
            this.proposalUpDown.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.proposalUpDown.Location = new System.Drawing.Point(680, 384);
            this.proposalUpDown.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.proposalUpDown.Name = "proposalUpDown";
            this.proposalUpDown.Size = new System.Drawing.Size(56, 23);
            this.proposalUpDown.TabIndex = 73;
            // 
            // dateActionFrom
            // 
            this.dateActionFrom.CustomFormat = "dd MMM yyyy";
            this.dateActionFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateActionFrom.Location = new System.Drawing.Point(472, 312);
            this.dateActionFrom.Name = "dateActionFrom";
            this.dateActionFrom.Size = new System.Drawing.Size(96, 23);
            this.dateActionFrom.TabIndex = 72;
            // 
            // drpAcctPoints
            // 
            this.drpAcctPoints.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpAcctPoints.Location = new System.Drawing.Point(552, 384);
            this.drpAcctPoints.Name = "drpAcctPoints";
            this.drpAcctPoints.Size = new System.Drawing.Size(120, 23);
            this.drpAcctPoints.TabIndex = 71;
            this.drpAcctPoints.SelectedIndexChanged += new System.EventHandler(this.drpAcctPoints_SelectedIndexChanged);
            // 
            // drpCodes
            // 
            this.drpCodes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpCodes.Location = new System.Drawing.Point(250, 384);
            this.drpCodes.Name = "drpCodes";
            this.drpCodes.Size = new System.Drawing.Size(168, 23);
            this.drpCodes.TabIndex = 70;
            this.drpCodes.SelectedIndexChanged += new System.EventHandler(this.drpCodes_SelectedIndexChanged);
            // 
            // drpAcctCodes
            // 
            this.drpAcctCodes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpAcctCodes.DropDownWidth = 250;
            this.drpAcctCodes.Location = new System.Drawing.Point(60, 384);
            this.drpAcctCodes.Name = "drpAcctCodes";
            this.drpAcctCodes.Size = new System.Drawing.Size(184, 23);
            this.drpAcctCodes.TabIndex = 69;
            this.drpAcctCodes.SelectedIndexChanged += new System.EventHandler(this.drpAcctCodes_SelectedIndexChanged_1);
            // 
            // lScore
            // 
            this.lScore.Location = new System.Drawing.Point(480, 376);
            this.lScore.Name = "lScore";
            this.lScore.Size = new System.Drawing.Size(72, 32);
            this.lScore.TabIndex = 68;
            this.lScore.Text = "Limit Credit Score To:";
            this.lScore.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(8, 382);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(48, 24);
            this.label12.TabIndex = 67;
            this.label12.Text = "Codes:";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelsent
            // 
            this.labelsent.Enabled = false;
            this.labelsent.Location = new System.Drawing.Point(418, 281);
            this.labelsent.Name = "labelsent";
            this.labelsent.Size = new System.Drawing.Size(50, 16);
            this.labelsent.TabIndex = 66;
            this.labelsent.Text = "Between";
            this.labelsent.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // drpAcctActions
            // 
            this.drpAcctActions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpAcctActions.Location = new System.Drawing.Point(60, 310);
            this.drpAcctActions.Name = "drpAcctActions";
            this.drpAcctActions.Size = new System.Drawing.Size(184, 23);
            this.drpAcctActions.TabIndex = 65;
            this.drpAcctActions.SelectedIndexChanged += new System.EventHandler(this.drpAcctActions_SelectedIndexChanged_1);
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(8, 306);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(56, 24);
            this.label7.TabIndex = 64;
            this.label7.Text = "Actions:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // drpAcctArrears
            // 
            this.drpAcctArrears.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpAcctArrears.Items.AddRange(new object[] {
            "> 0",
            "<",
            ">"});
            this.drpAcctArrears.Location = new System.Drawing.Point(60, 165);
            this.drpAcctArrears.Name = "drpAcctArrears";
            this.drpAcctArrears.Size = new System.Drawing.Size(184, 23);
            this.drpAcctArrears.TabIndex = 63;
            this.drpAcctArrears.SelectedIndexChanged += new System.EventHandler(this.drpAcctArrears_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(8, 166);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 16);
            this.label6.TabIndex = 61;
            this.label6.Text = "Arrears :";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(394, 99);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 24);
            this.label1.TabIndex = 60;
            this.label1.Text = "Account Branch";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // drpAcctLetter
            // 
            this.drpAcctLetter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpAcctLetter.Location = new System.Drawing.Point(60, 280);
            this.drpAcctLetter.Name = "drpAcctLetter";
            this.drpAcctLetter.Size = new System.Drawing.Size(184, 23);
            this.drpAcctLetter.TabIndex = 59;
            this.drpAcctLetter.SelectedIndexChanged += new System.EventHandler(this.drpAcctLetter_SelectedIndexChanged_1);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(8, 277);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 24);
            this.label3.TabIndex = 58;
            this.label3.Text = "Letters:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(8, 115);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 32);
            this.label5.TabIndex = 57;
            this.label5.Text = "Status Code :";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // drpAcctAllocation
            // 
            this.drpAcctAllocation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpAcctAllocation.DropDownWidth = 250;
            this.drpAcctAllocation.Items.AddRange(new object[] {
            "",
            "Not Allocated",
            "Courts person",
            "Courts Person Set"});
            this.drpAcctAllocation.Location = new System.Drawing.Point(60, 57);
            this.drpAcctAllocation.Name = "drpAcctAllocation";
            this.drpAcctAllocation.Size = new System.Drawing.Size(184, 23);
            this.drpAcctAllocation.TabIndex = 55;
            this.drpAcctAllocation.SelectedIndexChanged += new System.EventHandler(this.drpAcctAllocation_SelectedIndexChanged_1);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(8, 46);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 32);
            this.label4.TabIndex = 54;
            this.label4.Text = "Select Accounts:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.Enabled = false;
            this.label2.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.label2.Location = new System.Drawing.Point(57, 89);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(128, 16);
            this.label2.TabIndex = 50;
            this.label2.Text = "Allocated Between:";
            // 
            // tpResult
            // 
            this.tpResult.Controls.Add(this.btnRefresh);
            this.tpResult.Controls.Add(this.label16);
            this.tpResult.Controls.Add(this.label15);
            this.tpResult.Controls.Add(this.label14);
            this.tpResult.Controls.Add(this.btnExcel);
            this.tpResult.Controls.Add(this.btnDeAllocate);
            this.tpResult.Controls.Add(this.btnAllocate);
            this.tpResult.Controls.Add(this.drpEmpNameResult);
            this.tpResult.Controls.Add(this.drpEmployeeTypesResult);
            this.tpResult.Controls.Add(this.drpBranchResult);
            this.tpResult.Controls.Add(this.dgAccounts);
            this.tpResult.Location = new System.Drawing.Point(0, 25);
            this.tpResult.Name = "tpResult";
            this.tpResult.Size = new System.Drawing.Size(736, 415);
            this.tpResult.TabIndex = 1;
            this.tpResult.Title = "Result";
            // 
            // btnRefresh
            // 
            this.btnRefresh.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.btnRefresh.ImageIndex = 4;
            this.btnRefresh.ImageList = this.menuIcons;
            this.btnRefresh.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnRefresh.Location = new System.Drawing.Point(440, 24);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(24, 24);
            this.btnRefresh.TabIndex = 94;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // menuIcons
            // 
            this.menuIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("menuIcons.ImageStream")));
            this.menuIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.menuIcons.Images.SetKeyName(0, "");
            this.menuIcons.Images.SetKeyName(1, "");
            this.menuIcons.Images.SetKeyName(2, "");
            this.menuIcons.Images.SetKeyName(3, "");
            this.menuIcons.Images.SetKeyName(4, "");
            this.menuIcons.Images.SetKeyName(5, "");
            this.menuIcons.Images.SetKeyName(6, "");
            // 
            // label16
            // 
            this.label16.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.label16.Location = new System.Drawing.Point(8, 8);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(96, 16);
            this.label16.TabIndex = 93;
            this.label16.Text = "Employee Type";
            // 
            // label15
            // 
            this.label15.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.label15.Location = new System.Drawing.Point(152, 8);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(48, 16);
            this.label15.TabIndex = 92;
            this.label15.Text = "Branch";
            // 
            // label14
            // 
            this.label14.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.label14.Location = new System.Drawing.Point(232, 8);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(96, 16);
            this.label14.TabIndex = 91;
            this.label14.Text = "Employee Name";
            // 
            // btnExcel
            // 
            this.btnExcel.Enabled = false;
            this.btnExcel.Location = new System.Drawing.Point(664, 24);
            this.btnExcel.Name = "btnExcel";
            this.btnExcel.Size = new System.Drawing.Size(48, 23);
            this.btnExcel.TabIndex = 90;
            this.btnExcel.Text = "Excel";
            this.btnExcel.Visible = false;
            this.btnExcel.Click += new System.EventHandler(this.btnExcel_Click);
            // 
            // btnDeAllocate
            // 
            this.btnDeAllocate.Enabled = false;
            this.btnDeAllocate.Location = new System.Drawing.Point(576, 24);
            this.btnDeAllocate.Name = "btnDeAllocate";
            this.btnDeAllocate.Size = new System.Drawing.Size(80, 24);
            this.btnDeAllocate.TabIndex = 89;
            this.btnDeAllocate.Text = "DeAllocate";
            this.btnDeAllocate.Visible = false;
            this.btnDeAllocate.Click += new System.EventHandler(this.btnDeAllocate_Click);
            // 
            // btnAllocate
            // 
            this.btnAllocate.Enabled = false;
            this.btnAllocate.Location = new System.Drawing.Point(488, 24);
            this.btnAllocate.Name = "btnAllocate";
            this.btnAllocate.Size = new System.Drawing.Size(80, 24);
            this.btnAllocate.TabIndex = 88;
            this.btnAllocate.Text = "Allocate";
            this.btnAllocate.Click += new System.EventHandler(this.btnAllocate_Click);
            // 
            // drpEmpNameResult
            // 
            this.drpEmpNameResult.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpEmpNameResult.Enabled = false;
            this.drpEmpNameResult.Location = new System.Drawing.Point(232, 24);
            this.drpEmpNameResult.Name = "drpEmpNameResult";
            this.drpEmpNameResult.Size = new System.Drawing.Size(184, 23);
            this.drpEmpNameResult.TabIndex = 87;
            this.drpEmpNameResult.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
            // 
            // drpEmployeeTypesResult
            // 
            this.drpEmployeeTypesResult.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpEmployeeTypesResult.Location = new System.Drawing.Point(8, 24);
            this.drpEmployeeTypesResult.Name = "drpEmployeeTypesResult";
            this.drpEmployeeTypesResult.Size = new System.Drawing.Size(128, 23);
            this.drpEmployeeTypesResult.TabIndex = 86;
            this.drpEmployeeTypesResult.SelectedIndexChanged += new System.EventHandler(this.drpEmployeeTypes2_SelectedIndexChanged);
            // 
            // drpBranchResult
            // 
            this.drpBranchResult.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpBranchResult.Location = new System.Drawing.Point(152, 24);
            this.drpBranchResult.Name = "drpBranchResult";
            this.drpBranchResult.Size = new System.Drawing.Size(64, 23);
            this.drpBranchResult.TabIndex = 88;
            this.drpBranchResult.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // dgAccounts
            // 
            this.dgAccounts.DataMember = "";
            this.dgAccounts.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgAccounts.Location = new System.Drawing.Point(8, 64);
            this.dgAccounts.Name = "dgAccounts";
            this.dgAccounts.ReadOnly = true;
            this.dgAccounts.Size = new System.Drawing.Size(720, 336);
            this.dgAccounts.TabIndex = 4;
            this.dgAccounts.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgAccounts_MouseUp);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btnAllocateAcct);
            this.tabPage1.Controls.Add(this.drpEmpNameAcct);
            this.tabPage1.Controls.Add(this.drpEmployeeTypesAcct);
            this.tabPage1.Controls.Add(this.drpBranchAcct);
            this.tabPage1.Controls.Add(this.label22);
            this.tabPage1.Controls.Add(this.txtAccountNo);
            this.tabPage1.Controls.Add(this.label20);
            this.tabPage1.Controls.Add(this.label21);
            this.tabPage1.Controls.Add(this.btnClearAcct);
            this.tabPage1.Controls.Add(this.label8);
            this.tabPage1.Location = new System.Drawing.Point(0, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Selected = false;
            this.tabPage1.Size = new System.Drawing.Size(736, 415);
            this.tabPage1.TabIndex = 3;
            this.tabPage1.Title = "Allocate Single Account";
            this.tabPage1.Visible = false;
            // 
            // btnAllocateAcct
            // 
            this.btnAllocateAcct.Enabled = false;
            this.btnAllocateAcct.Location = new System.Drawing.Point(621, 45);
            this.btnAllocateAcct.Name = "btnAllocateAcct";
            this.btnAllocateAcct.Size = new System.Drawing.Size(72, 24);
            this.btnAllocateAcct.TabIndex = 121;
            this.btnAllocateAcct.Text = "Allocate";
            this.btnAllocateAcct.Click += new System.EventHandler(this.btnAllocateAcct_Click);
            // 
            // drpEmpNameAcct
            // 
            this.drpEmpNameAcct.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpEmpNameAcct.Enabled = false;
            this.drpEmpNameAcct.Location = new System.Drawing.Point(390, 45);
            this.drpEmpNameAcct.Name = "drpEmpNameAcct";
            this.drpEmpNameAcct.Size = new System.Drawing.Size(184, 23);
            this.drpEmpNameAcct.TabIndex = 119;
            this.drpEmpNameAcct.SelectedIndexChanged += new System.EventHandler(this.drpEmpNameAcct_SelectedIndexChanged);
            // 
            // drpEmployeeTypesAcct
            // 
            this.drpEmployeeTypesAcct.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpEmployeeTypesAcct.Location = new System.Drawing.Point(136, 45);
            this.drpEmployeeTypesAcct.Name = "drpEmployeeTypesAcct";
            this.drpEmployeeTypesAcct.Size = new System.Drawing.Size(128, 23);
            this.drpEmployeeTypesAcct.TabIndex = 118;
            this.drpEmployeeTypesAcct.SelectedIndexChanged += new System.EventHandler(this.drpEmployeeTypesAcct_SelectedIndexChanged);
            // 
            // drpBranchAcct
            // 
            this.drpBranchAcct.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpBranchAcct.Location = new System.Drawing.Point(283, 45);
            this.drpBranchAcct.Name = "drpBranchAcct";
            this.drpBranchAcct.Size = new System.Drawing.Size(64, 23);
            this.drpBranchAcct.TabIndex = 120;
            this.drpBranchAcct.SelectedIndexChanged += new System.EventHandler(this.drpBranchAcct_SelectedIndexChanged);
            // 
            // label22
            // 
            this.label22.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.label22.Location = new System.Drawing.Point(280, 26);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(48, 16);
            this.label22.TabIndex = 117;
            this.label22.Text = "Branch";
            // 
            // txtAccountNo
            // 
            this.txtAccountNo.BackColor = System.Drawing.SystemColors.Window;
            this.txtAccountNo.Location = new System.Drawing.Point(15, 45);
            this.txtAccountNo.MaxLength = 20;
            this.txtAccountNo.Name = "txtAccountNo";
            this.txtAccountNo.PreventPaste = false;
            this.txtAccountNo.Size = new System.Drawing.Size(94, 23);
            this.txtAccountNo.TabIndex = 115;
            this.txtAccountNo.Text = "000-0000-0000-0";
            // 
            // label20
            // 
            this.label20.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.label20.Location = new System.Drawing.Point(133, 26);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(96, 16);
            this.label20.TabIndex = 113;
            this.label20.Text = "Employee Type";
            // 
            // label21
            // 
            this.label21.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.label21.Location = new System.Drawing.Point(387, 26);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(96, 16);
            this.label21.TabIndex = 112;
            this.label21.Text = "Employee Name";
            // 
            // btnClearAcct
            // 
            this.btnClearAcct.Location = new System.Drawing.Point(621, 85);
            this.btnClearAcct.Name = "btnClearAcct";
            this.btnClearAcct.Size = new System.Drawing.Size(72, 24);
            this.btnClearAcct.TabIndex = 111;
            this.btnClearAcct.Text = "Clear";
            this.btnClearAcct.Click += new System.EventHandler(this.btnClearAcct_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 26);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(99, 15);
            this.label8.TabIndex = 1;
            this.label8.Text = "Account Number";
            // 
            // toolBar1
            // 
            this.toolBar1.DropDownArrows = true;
            this.toolBar1.Location = new System.Drawing.Point(0, 0);
            this.toolBar1.Name = "toolBar1";
            this.toolBar1.ShowToolTips = true;
            this.toolBar1.Size = new System.Drawing.Size(768, 42);
            this.toolBar1.TabIndex = 10;
            // 
            // HoldFlags
            // 
            this.HoldFlags.Columns.AddRange(new System.Data.DataColumn[] {
            this.HoldFlag,
            this.DateCleared,
            this.ByUser});
            this.HoldFlags.Constraints.AddRange(new System.Data.Constraint[] {
            new System.Data.UniqueConstraint("Constraint1", new string[] {
                        "HoldFlag"}, true)});
            this.HoldFlags.MinimumCapacity = 10;
            this.HoldFlags.PrimaryKey = new System.Data.DataColumn[] {
        this.HoldFlag};
            this.HoldFlags.TableName = "HoldFlags";
            // 
            // HoldFlag
            // 
            this.HoldFlag.AllowDBNull = false;
            this.HoldFlag.ColumnName = "HoldFlag";
            this.HoldFlag.MaxLength = 4;
            // 
            // DateCleared
            // 
            this.DateCleared.ColumnName = "DateCleared";
            this.DateCleared.MaxLength = 15;
            // 
            // ByUser
            // 
            this.ByUser.ColumnName = "ByUser";
            this.ByUser.MaxLength = 10;
            // 
            // HoldFlagSet
            // 
            this.HoldFlagSet.DataSetName = "NewDataSet";
            this.HoldFlagSet.Locale = new System.Globalization.CultureInfo("en-GB");
            // 
            // IncompleteSet
            // 
            this.IncompleteSet.DataSetName = "NewDataSet";
            this.IncompleteSet.Locale = new System.Globalization.CultureInfo("en-GB");
            // 
            // dataSet1
            // 
            this.dataSet1.DataSetName = "NewDataSet";
            this.dataSet1.Locale = new System.Globalization.CultureInfo("en-GB");
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
            // FollowUp5_2
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.groupBox1);
            this.Name = "FollowUp5_2";
            this.Text = "Collection Account Analysis";
            ((System.ComponentModel.ISupportInitialize)(this.IncompleteAccounts)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.tcAllocated.ResumeLayout(false);
            this.tpQuery.ResumeLayout(false);
            this.tpQuery.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtNumActions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBalances)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtArrears)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.proposalUpDown)).EndInit();
            this.tpResult.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgAccounts)).EndInit();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.HoldFlags)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.HoldFlagSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.IncompleteSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private void btnSearch_Click(object sender, System.EventArgs e)
        {
            Function = "btnSearch_Click";
            Wait();
            try
            {
                drpEmployeeTypesResult.SelectedIndex = 0;
                //drpEmpNameResult.SelectedIndex = 0;
                drpBranchResult.Text = Config.BranchCode;

                string alreadyAllocated = (string)drpAcctAllocation.SelectedItem;
                alreadyAllocated = alreadyAllocated.Substring(0, alreadyAllocated.IndexOf("-") - 1);
                string employeeType = "";

                string employee = "";
                if (alreadyAllocated != "NA")
                {
                    employeeType = (string)drpEmployeeTypes.SelectedItem;
                    employeeType = employeeType.Split(':')[0].Trim();
                    employee = (string)drpEmpName.SelectedItem;
                    employee = employee.Substring(0, employee.IndexOf(":") - 1);
                    empeeNo = Convert.ToInt32(employee);

                }
                string currStatus = "";
                string statustype = "";
                if (drpStatusSelection.SelectedIndex != 0)
                {
                    statustype = "=";
                    currStatus = (string)drpAcctStatus.SelectedItem;
                    currStatus = currStatus.Substring(0, currStatus.IndexOf("-") - 1);
                }

                DateTime dateStartAllocated = Convert.ToDateTime(dateAllocatedFrom.Value);

                DateTime dateFinishedAllocated = Convert.ToDateTime(dateAllocatedTo.Value);

                string actionRestriction = (string)drpAcctActions.SelectedItem;
                actionRestriction = actionRestriction.Substring(0, actionRestriction.IndexOf("-") - 1);


                string actionCode = (string)drpActions.SelectedItem;
                actionCode = actionCode.Substring(0, actionCode.IndexOf("-") - 1);
                
                if (!drpActions.Enabled)
                    actionCode = string.Empty;

                DateTime actiondateStart = Convert.ToDateTime(dateActionFrom.Value);
                DateTime actiondateFinish = Convert.ToDateTime(dateActionTo.Value);

                string alreadyallocated = (string)drpAcctCodes.SelectedItem;
                alreadyallocated = alreadyallocated.Substring(0, alreadyallocated.IndexOf("-") - 1);

                string letterRestriction = (string)drpAcctLetter.SelectedItem;
                letterRestriction = letterRestriction.Substring(0, letterRestriction.IndexOf("-") - 1);
                Double Arrears = Convert.ToDouble(StripCurrency(txtArrears.Value.ToString()));

                DateTime dateStartLetter = Convert.ToDateTime(dateLetterFrom.Value);
                DateTime dateFinishLetter = Convert.ToDateTime(dateLetterTo.Value);

                string branch = (string)drpEmployeeBranch.SelectedItem;
                if (!IsNumeric(branch)) branch = "0";
                short branchno = Convert.ToInt16(branch);

                string branchacct = (string)drpBranchAcct.SelectedItem;
                if (!IsNumeric(branch))
                    branch = "0";
                short branchnoacct = 0;
                if (branchacct != "ALL")
                    branchnoacct = Convert.ToInt16(branchacct);

                string letterCode = (string)drpLetters.SelectedItem;
                letterCode = letterCode.Substring(0, letterCode.IndexOf("-") - 1);
                string accountBranch = (string)drpBranch.SelectedItem;
                if (!IsNumeric(accountBranch)) accountBranch = "0";
                accountBranch = accountBranch + "%";

                string propPointsDirection = (string)drpAcctPoints.SelectedItem;
                propPointsDirection = propPointsDirection.Substring(0, propPointsDirection.IndexOf("-") - 1);

                bool rowLimited = false;
                string arrearsChoice = (string)drpAcctArrears.SelectedItem;
                arrearsChoice = arrearsChoice.Substring(0, arrearsChoice.IndexOf("-") - 1);

                if (searchClicked > 0) /*clear existing list if clicked beforehand*/
                    accounts.Clear();

                short numActions = Convert.ToInt16(txtNumActions.Value);

                string actionOperand = "";
                if (drpNumAcctions.SelectedIndex > 0)
                    actionOperand = (string)drpNumAcctions.SelectedItem;

                string balanceOperand = "";
                if (drpBalances.SelectedIndex > 0)
                    balanceOperand = (string)drpBalances.SelectedItem;

                short restrictEmployee = Convert.ToInt16(chxActions.Checked);
                decimal balance = txtBalances.Value;
                short includeCharges = Convert.ToInt16(chxCharges.Checked);

                DateTime dateMovedFrom = Convert.ToDateTime(dtMovedFrom.Value);
                DateTime dateMovedTo = Convert.ToDateTime(dtMovedTo.Value);
                string dateMovedRestriction = (string)drpDateMoved.SelectedItem;

                DateTime datelastPaid = Convert.ToDateTime(dtDateLastPaid.Value);
                string dateOperand = "";
                if (drpDateLastPaid.SelectedIndex > 0)
                    dateOperand = (string)drpDateLastPaid.SelectedItem;

                bool actionDueDate = rbDue.Checked;	//jec 67902
                bool credit = rbCredit.Checked;     // RD CR813
                bool cash = rbCash.Checked;         // RD CR813

                string minstatus = "";

                if (drpStatusSelection.SelectedIndex == 2) // "between"
                {
                    minstatus = (string)drpAcctMinStatus.SelectedItem;
                    statustype = "Between";
                    minstatus = minstatus.Substring(0, minstatus.IndexOf("-") - 1);
                }

                string worklist = drpWorklists.SelectedItem.ToString();
                worklist = worklist.Remove(worklist.IndexOf(" "));
                if (worklist == "NL")
                {
                    worklist = "%";
                }

                string deliveryArea = drpAreas.SelectedItem.ToString();
                if (deliveryArea == "ALL")
                {
                    deliveryArea = "%";
                }

                accounts = AccountManager.Getacctsforalloc5_2(alreadyAllocated,
                 minstatus,
                 currStatus,
                 employeeType,
                 dateStartAllocated,
                 dateFinishedAllocated,
                 actionRestriction,
                 actiondateStart,
                 actiondateFinish,
                 Arrears,
                 Arrears,
                 statustype,
                 arrearsChoice,
                 Arrears,
                 actionCode,
                 letterCode,
                 letterRestriction,
                 true,//letterRadio,
                 dateStartLetter,
                 dateFinishLetter,
                 actionRestriction,
                 empeeNo,
                 actiondateStart,
                 actiondateFinish,
                 true,//includePhone,
                 true,//includeAddress,
                 branchno,
                 branchset,
                 (short)proposalUpDown.Value,
                 propPointsDirection,
                 codeselection,
                 accountBranch,
                 viewTop.Checked,
                 codes,
                 numActions,
                 actionOperand,
                 balanceOperand,
                 restrictEmployee,
                 balance,
                 includeCharges,
                 dateMovedFrom,
                 dateMovedTo,
                 dateMovedRestriction,
                 datelastPaid,
                 dateOperand,
                 actionDueDate,		//jec 67902
                 credit,             //RD CR813
                 cash,               //RD CR813
                 chkServiceFilter.Checked, //pc CR802
                 worklist, //CR852 JH 17/05/07
                 deliveryArea, //CR852 JH 17/05/07
                 ref rowLimited,
                 out Error);

                if (Error.Length > 0)
                {
                    ShowError(Error);
                }
                else
                {

                    DataTable dt1 = new DataTable();
                    dt1 = accounts.Tables[0];
                    DataTable dt2 = new DataTable();
                    dt2 = accounts.Tables[1];

                    // foreach (DataTable dt in accounts.Tables)
                    //{
                    // if (dt.TableName == TN.ArrearsAccounts)
                    //{
                    //dt1.TableName = TN.ArrearsAccounts;
                    acctView = new DataView(dt1);
                    statusBar1.Text = acctView.Count.ToString() + " rows returned";

                    if (acctView.Count > 0)
                    {
                        // 68315 RD 28/06/06 Allocate account status bar
                        // ((MainForm)this.FormRoot).statusBar1.Text = acctView.Count.ToString()+ " rows returned";
                        //statusBar1.Text = acctView.Count.ToString() + " rows returned";
                        dgAccounts.DataSource = acctView;
                        //acctView.d
                        if (dgAccounts.TableStyles.Count == 0)
                        {
                            DataGridTableStyle tabStyle = new DataGridTableStyle();
                            tabStyle.MappingName = dt1.TableName;
                            dgAccounts.TableStyles.Add(tabStyle);

                            /* Set the table style according to the user's preference  */

                            tabStyle.GridColumnStyles[CN.FirstName].Width = 145;
                            tabStyle.GridColumnStyles[CN.FirstName].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.FirstName].HeaderText = GetResource("T_FIRSTNAME");

                            tabStyle.GridColumnStyles[CN.Title].Width = 49;
                            tabStyle.GridColumnStyles[CN.Title].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.Title].HeaderText = GetResource("T_TITLE");

                            tabStyle.GridColumnStyles[CN.acctno].Width = 92;
                            tabStyle.GridColumnStyles[CN.acctno].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.acctno].HeaderText = GetResource("T_ACCOUNTNO");

                            tabStyle.GridColumnStyles[CN.name].Width = 140;
                            tabStyle.GridColumnStyles[CN.name].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.name].HeaderText = GetResource("T_NAME");

                            tabStyle.GridColumnStyles[CN.cusaddr1].Width = 200;
                            tabStyle.GridColumnStyles[CN.cusaddr1].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.cusaddr1].HeaderText = GetResource("T_ADDRESS1");

                            tabStyle.GridColumnStyles[CN.cusaddr2].Width = 200;
                            tabStyle.GridColumnStyles[CN.cusaddr2].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.cusaddr2].HeaderText = GetResource("T_ADDRESS2");

                            tabStyle.GridColumnStyles[CN.cusaddr3].Width = 200;
                            tabStyle.GridColumnStyles[CN.cusaddr3].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.cusaddr3].HeaderText = GetResource("T_ADDRESS3");

                            tabStyle.GridColumnStyles[CN.cuspocode].Width = 100;
                            tabStyle.GridColumnStyles[CN.cuspocode].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.cuspocode].HeaderText = GetResource("T_POSTCODE");

                            tabStyle.GridColumnStyles[CN.Balance].Width = 150;
                            tabStyle.GridColumnStyles[CN.Balance].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.Balance].HeaderText = GetResource("T_BALANCE");

                            tabStyle.GridColumnStyles[CN.Arrears].Width = 100;
                            tabStyle.GridColumnStyles[CN.Arrears].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.Arrears].HeaderText = GetResource("T_ARREARS");

                            tabStyle.GridColumnStyles[CN.ArrearsLevel2].Width = 100;
                            tabStyle.GridColumnStyles[CN.ArrearsLevel2].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.ArrearsLevel2].HeaderText = GetResource("T_ARREARSLEVEL");

                            tabStyle.GridColumnStyles[CN.ArrearsExCharges].Width = 150;
                            tabStyle.GridColumnStyles[CN.ArrearsExCharges].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.ArrearsExCharges].HeaderText = GetResource("T_ARREARSEXCHARGES");

                            tabStyle.GridColumnStyles[CN.Instalment].Width = 150;
                            tabStyle.GridColumnStyles[CN.Instalment].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.Instalment].HeaderText = GetResource("T_INSTAL");

                            tabStyle.GridColumnStyles[CN.CustomerID].Width = 100;
                            tabStyle.GridColumnStyles[CN.CustomerID].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.CustomerID].HeaderText = GetResource("T_CUSTID");

                            tabStyle.GridColumnStyles[CN.PercentagePaid].Width = 49;
                            tabStyle.GridColumnStyles[CN.PercentagePaid].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.PercentagePaid].HeaderText = GetResource("T_PERCENTAGEPAID");

                            tabStyle.GridColumnStyles[CN.Status].Width = 49;
                            tabStyle.GridColumnStyles[CN.Status].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.Status].HeaderText = GetResource("T_STATUS");

                            tabStyle.GridColumnStyles[CN.EmployeeNo].Width = 80;
                            tabStyle.GridColumnStyles[CN.EmployeeNo].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.EmployeeNo].HeaderText = GetResource("T_EMPEENO");

                            tabStyle.GridColumnStyles[CN.DateLastPaid].Width = 70;
                            tabStyle.GridColumnStyles[CN.DateLastPaid].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.DateLastPaid].HeaderText = GetResource("T_DATELASTPAID");

                            tabStyle.GridColumnStyles[CN.DueDay].Width = 70;
                            tabStyle.GridColumnStyles[CN.DueDay].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.DueDay].HeaderText = GetResource("T_DUEDAY");

                            tabStyle.GridColumnStyles[CN.BDWBalance].Width = 80;
                            tabStyle.GridColumnStyles[CN.BDWBalance].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.BDWBalance].HeaderText = GetResource("T_BDWBALANCE");

                            tabStyle.GridColumnStyles[CN.BDWCharges].Width = 80;
                            tabStyle.GridColumnStyles[CN.BDWCharges].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.BDWCharges].HeaderText = GetResource("T_BDWCHARGES");
                            // jec 67911 - add date status changed
                            tabStyle.GridColumnStyles[CN.DateStatChge].Width = 80;
                            tabStyle.GridColumnStyles[CN.DateStatChge].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.DateStatChge].HeaderText = GetResource("T_DATESTATCHANGED");
                        }

                        tcAllocated.SelectedTab = tpResult;

                        if (alreadyAllocated == "CP")
                        {
                            btnAllocate.Text = "Re-Allocate";
                            btnDeAllocate.Enabled = true;
                        }
                        else
                        {
                            btnAllocate.Text = "Allocate";
                            btnDeAllocate.Enabled = false;
                        }
                    }
                    else
                    {
                        // clear result grid (next pass 0 rows returned)
                        dgAccounts.DataSource = null;       //UAT 5.0 iss 79 jec 28/03/07
                    }

                    allocatedAccounts = new ArrayList();
                    foreach (DataRow dtRow in dt2.Rows)
                    {
                        allocatedAccounts.Add(dtRow[0].ToString());
                    }
                }
                //}
                //}


                if (rowLimited == true)
                    MessageBox.Show("Initial select was restricted to save CPU. Either run this on the reporting db "
                        + "\n\n or increase the max rows for this employee in Staff and Contractor maintenance");

                Function = "End of btnSearch_Click";
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                /*if (dgAccounts.VisibleRowCount > 0) 
                {
                    //Display the results tab..
                    tcAllocated.SelectedTab = tpResult;
                }*/
                StopWait();
            }
        }

        private void menuExit_Click(object sender, System.EventArgs e)
        {
            this.CloseTab();
        }

        private void drpAcctAllocation_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            string allocationtype;

            try
            {
                allocationtype = (string)drpAcctAllocation.SelectedItem;
                allocationtype = allocationtype.Substring(0, allocationtype.IndexOf("-") - 1);
                if (allocationtype == "CP") //allocated to a courts person
                {
                    drpEmployeeTypes.Enabled = true;
                    drpEmployeeBranch.Enabled = true;
                    drpEmpName.Enabled = true;
                    dateAllocatedFrom.Enabled = true;
                    dateAllocatedTo.Enabled = true;
                    label2.Enabled = true;
                }
                else
                {
                    drpEmployeeTypes.Enabled = false;
                    drpEmployeeBranch.Enabled = false;
                    drpEmpName.Enabled = false;
                    dateAllocatedFrom.Enabled = false;
                    dateAllocatedTo.Enabled = false;
                    label2.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void drpEmpName_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            string salesPersonStr = "";
            try
            {
                if (drpEmpName.SelectedIndex > 0)
                {
                    salesPersonStr = (string)drpEmpName.SelectedItem;
                    empeeNo = Convert.ToInt32(salesPersonStr.Substring(0, salesPersonStr.IndexOf(":") - 1));
                }
                else empeeNo = 0;
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void drpEmployeeTypes_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            string empType;
            string empTypeStr;
            DataSet ds = null;
            string branch;
            creditstaff.Clear();

            try
            {
                if (drpEmployeeTypes.SelectedIndex >= 0)    // && staticdataloaded == true) removed (jec 28/03/07)
                {
                    if (drpEmployeeTypes.Enabled) //UAT(5.2) - 872
                        drpEmpName.Enabled = true;
                    drpEmpName.DataSource = null;
                    empTypeStr = (string)drpEmployeeTypes.SelectedItem;
                    empType = empTypeStr.Split(':')[0].Trim();
                    branch = (string)drpEmployeeBranch.SelectedItem;
                    if (!IsNumeric(branch)) branch = "0";

                    //IP - 02/06/08 - Credit Collections - change to cater for (3) charcter Employee Types.
                  //  int empTypeStrRemove = empTypeStr.IndexOf("-") + 1;
                    //IP - 02/06/08 - Credit Collections - change to cater for (3) charcter Employee Types.
                    //creditstaff.Add("0 : " + empTypeStr.Substring(empTypeStr.IndexOf("-") + 1, empTypeStr.Length - 3));
                    creditstaff.Add("0 : " + empTypeStr.Split(':')[0].Trim());

                    ds = Login.GetStaffAllocationByType(Convert.ToInt32(empType), Convert.ToInt32(branch), out Error);

                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                    {
                        foreach (DataTable dt in ds.Tables)
                        {
                            if (dt.TableName == TN.SalesStaff)
                            {
                                foreach (DataRow row in dt.Rows)
                                {
                                    string str = Convert.ToString(row.ItemArray[0]) + " : " + (string)row.ItemArray[1];
                                    creditstaff.Add(str.ToUpper());
                                }
                            }
                        }
                        drpEmpName.DataSource = creditstaff;
                        if (creditstaff.Count > 0) drpEmpName.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        //private void drpEmployeeTypes3_SelectedIndexChanged(object sender, System.EventArgs e)
        //{
        //    string empType;
        //    string empTypeStr;
        //    DataSet ds = null;
        //    string branch;
        //    creditstaff.Clear();

        //    try
        //    {
        //        if (drpEmployeeTypes.SelectedIndex >= 0 && staticdataloaded == true)
        //        {
        //            drpEmpName.Enabled = true;
        //            drpEmpName.DataSource = null;
        //            empTypeStr = (string)drpEmployeeTypes.SelectedItem;
        //            empType = empTypeStr.Substring(0, empTypeStr.IndexOf("-") - 1);
        //            branch = (string)drpEmployeeBranch.SelectedItem;
        //            if (!IsNumeric(branch)) branch = "0";

        //            creditstaff.Add("0 : " + empTypeStr.Substring(empTypeStr.IndexOf("-") + 1, empTypeStr.Length - 3));

        //            ds = Login.GetStaffAllocationByType(empType, Convert.ToInt32(branch), out Error);

        //            if (Error.Length > 0)
        //                ShowError(Error);
        //            else
        //            {
        //                foreach (DataTable dt in ds.Tables)
        //                {
        //                    if (dt.TableName == TN.SalesStaff)
        //                    {
        //                        foreach (DataRow row in dt.Rows)
        //                        {
        //                            string str = Convert.ToString(row.ItemArray[0]) + " : " + (string)row.ItemArray[1];
        //                            creditstaff.Add(str.ToUpper());
        //                        }
        //                    }
        //                }
        //                drpEmpName.DataSource = creditstaff;
        //                if (creditstaff.Count > 0) drpEmpName.SelectedIndex = 0;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Catch(ex, Function);
        //    }
        //}

        private void drpAcctLetter_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            string letterselection;
            try
            {
                letterselection = (string)drpAcctLetter.SelectedItem;
                letterselection = letterselection.Substring(0, letterselection.IndexOf("-") - 1);
                if (letterselection == "SL") //restricted to a specific letter
                {
                    drpLetters.Enabled = true;

                }
                else
                {
                    drpLetters.Enabled = false;

                }
                if (letterselection != "NR")//nr -no restriction
                {
                    dateLetterFrom.Enabled = true;
                    dateLetterTo.Enabled = true;
                    labelsent.Enabled = true;
                }
                else
                {
                    dateLetterFrom.Enabled = false;
                    dateLetterTo.Enabled = false;
                    labelsent.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void drpAcctActions_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            string actionselection;
            try
            {
                actionselection = (string)drpAcctActions.SelectedItem;
                actionselection = actionselection.Substring(0, actionselection.IndexOf("-") - 1);
                if (actionselection != "NR") //restricted to a specific action
                {
                    drpActions.Enabled = true;
                    dateActionFrom.Enabled = true;
                    dateActionTo.Enabled = true;
                    labeltaken.Enabled = true;
                }
                else
                {
                    drpActions.Enabled = false;
                    dateActionFrom.Enabled = false;
                    dateActionTo.Enabled = false;
                    labeltaken.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void drpAcctCodes_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                codeselection = (string)drpAcctCodes.SelectedItem;
                codeselection = codeselection.Substring(0, codeselection.IndexOf("-") - 1);
                if (codeselection != "NR") //not restricted to a specific code
                {
                    drpCodes.DataSource = customercode;
                    if (codeselection == "AC" || codeselection == "NA") //account code or no account code
                        drpCodes.DataSource = acctcode;

                    drpCodes.Enabled = true;

                }
                else
                {
                    drpCodes.Enabled = false;

                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void dgAccounts_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            DataView dv = (DataView)dgAccounts.DataSource;
            if (dv != null)
            {
                int count = dv.Count;
                btnDeAllocate.Enabled = true;
                if (e.Button == MouseButtons.Right)
                {
                    if (dgAccounts.CurrentRowIndex >= 0)
                    {
                        DataGrid ctl = (DataGrid)sender;

                        MenuCommand m1 = new MenuCommand(GetResource("P_ACCOUNT_DETAILS"));
                        MenuCommand m2 = new MenuCommand(GetResource("P_FOLLOWUP"));
                        MenuCommand m3 = new MenuCommand(GetResource("P_ALLOCATION_HISTORY"));

                        m1.Click += new System.EventHandler(this.OnAccountDetails);
                        m2.Click += new System.EventHandler(this.OnFollowUpDetails);
                        m3.Click += new System.EventHandler(this.OnAllocationHistory);

                        PopupMenu popup = new PopupMenu();
                        popup.MenuCommands.AddRange(new MenuCommand[] { m1, m2, m3 });
                        MenuCommand selected = popup.TrackPopup(ctl.PointToScreen(new Point(e.X, e.Y)));


                        for (int i = count - 1; i >= 0; i--)
                        {
                            if (dgAccounts.IsSelected(i))
                            {
                                string al = (string)dv[i][CN.Status];
                                if (al == "5") //Prevent Deallocation of accounts if in status code 5
                                {
                                    btnDeAllocate.Enabled = false;
                                }
                            }
                        }

                    }
                }
                else //left mouse click
                {
                    for (int i = count - 1; i >= 0; i--)
                    {
                        if (dgAccounts.IsSelected(i))
                        {
                            string al = (string)dv[i][CN.Status];
                            if (al == "5") //Prevent Deallocation of accounts if in status code 5
                            {
                                btnDeAllocate.Enabled = false;
                            }
                            // Only allocate accounts that are in the allocatedAccounts ArrayList
                            string accountNo = dv[i][CN.acctno].ToString();
                            if (allocatedAccounts.Contains(accountNo) && empeeNo == 0)
                            {
                                btnAllocate.Enabled = true;
                            }
                            else
                            {
                                btnAllocate.Enabled = false;
                            }
                        }
                    }
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

        private void OnAllocationHistory(object sender, System.EventArgs e)
        {
            try
            {
                Function = "OnAllocationHistory";
                int index = dgAccounts.CurrentRowIndex;

                if (index >= 0)
                {
                    string acctNo = (string)dgAccounts[index, 0];
                    AccountDetails details = new AccountDetails(acctNo, FormRoot, this, TPN.AllocationHistory);
                    ((MainForm)this.FormRoot).AddTabPage(details, 7);
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }


        private void OnFollowUpDetails(object sender, System.EventArgs e)
        {
            try
            {
                Function = "OnFollowUpDetails";
                int index = dgAccounts.CurrentRowIndex;

                if (index >= 0)
                {
                    string acctNo = (string)dgAccounts[index, 0];
                    BailActions actions = new BailActions(acctNo, FormRoot, this);
                    actions.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }


        private void drpAcctLetter_SelectedIndexChanged_1(object sender, System.EventArgs e)
        {
            this.drpAcctLetter_SelectedIndexChanged(sender, e);
        }

        private void drpAcctActions_SelectedIndexChanged_1(object sender, System.EventArgs e)
        {
            this.drpAcctActions_SelectedIndexChanged(sender, e);
        }

        private void drpAcctCodes_SelectedIndexChanged_1(object sender, System.EventArgs e)
        {
            this.drpAcctCodes_SelectedIndexChanged(sender, e);
        }

        private void tabPage1_PropertyChanged(Crownwood.Magic.Controls.TabPage page, Crownwood.Magic.Controls.TabPage.Property prop, object oldValue)
        {

        }

        private void btnSearch_Click_1(object sender, System.EventArgs e)
        {
            this.btnSearch_Click(sender, e);
        }

        private void drpCodes_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            codes = (string)drpCodes.SelectedItem;
            codes = codes.Substring(0, codes.IndexOf("-") - 1);
        }

        private void drpAcctArrears_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            string arrearsselection;
            if (staticdataloaded == true)
            {
                arrearsselection = (string)drpAcctArrears.SelectedItem;
                arrearsselection = arrearsselection.Substring(0, arrearsselection.IndexOf("-") - 1);

                if (drpAcctArrears.SelectedIndex > 0)
                    txtArrears.Enabled = true;
                else
                {
                    txtArrears.Value = 0;
                    txtArrears.Enabled = false;
                }

                if (arrearsselection == "<" || arrearsselection == ">")
                    txtArrears.Increment = 100;
                else
                    txtArrears.Increment = Convert.ToDecimal(.5);
            }

        }

        private void drpAcctAllocation_SelectedIndexChanged_1(object sender, System.EventArgs e)
        {
            this.drpAcctAllocation_SelectedIndexChanged(sender, e);
        }

        private void drpEmployeeTypes2_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            string empType;
            string empTypeStr;
            DataSet ds = null;
            string branch;
            staff.Clear();

            try
            {
                if (drpEmployeeTypesResult.SelectedIndex >= 0 && staticdataloaded == true)
                {
                    drpEmpNameResult.Enabled = true;
                    drpEmpNameResult.DataSource = null;
                    empTypeStr = (string)drpEmployeeTypesResult.SelectedItem;
                    empType = empTypeStr.Split(':')[0].Trim();

                    //IP - 02/06/08 - Credit Collections - Altered to cater for (3) character Employee Types.
                    int empTypeStrRemove = empTypeStr.IndexOf("-") + 1;
                    //staff.Add("0 : " + empTypeStr.Substring(empTypeStr.IndexOf("-") + 1, empTypeStr.Length - 3));
                    staff.Add("0 : " + empTypeStr.Substring(empTypeStr.IndexOf("-") + 1, empTypeStr.Length - empTypeStrRemove));

                    branch = (string)drpBranchResult.SelectedItem;
                    if (!IsNumeric(branch)) branch = "0";
                    int employeeBranch = Convert.ToInt32(branch);
                    ds = Login.GetStaffAllocationByType(Convert.ToInt32(empType), employeeBranch, out Error);

                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                    {
                        foreach (DataTable dt in ds.Tables)
                        {
                            if (dt.TableName == TN.SalesStaff)
                            {
                                foreach (DataRow row in dt.Rows)
                                {
                                    string str = Convert.ToString(row.ItemArray[0]) + " : " + (string)row.ItemArray[1];
                                    staff.Add(str.ToUpper());
                                }
                            }
                        }

                        drpEmpNameResult.DataSource = staff;
                        if (employee.Length > 0)
                        {
                            int i = drpEmpNameResult.FindString(employee);
                            if (i != -1)
                                drpEmpNameResult.SelectedIndex = i;
                        }
                    }
                    employee = "";
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }


        private void comboBox2_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            string employee;
            //int empeeNo;

            if (drpEmpNameResult.SelectedIndex >= 0)
            {
                employee = (string)drpEmpNameResult.SelectedItem;
                employee = employee.Substring(0, employee.IndexOf(":") - 1);
                empeeNo = Convert.ToInt32(employee);

                DataView dvAccounts = new DataView();
                dvAccounts = (DataView)dgAccounts.DataSource;

                if (dvAccounts != null)
                {
                    int count = dvAccounts.Count;

                    if (dgAccounts.CurrentRowIndex >= 0)
                    {
                        for (int i = count - 1; i >= 0; i--)
                        {
                            if (dgAccounts.IsSelected(i))
                            {
                                string accountNo = dvAccounts[i][CN.acctno].ToString();
                                if (empeeNo != 0 && allocatedAccounts.Contains(accountNo))
                                {
                                    btnAllocate.Enabled = true;
                                }
                                else
                                {
                                    btnAllocate.Enabled = false;
                                }
                            }
                        }
                    }
                }
                //if (empeeNo != 0)
                //    btnAllocate.Enabled = true;
                //else
                //    btnAllocate.Enabled = false;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.drpEmployeeTypes2_SelectedIndexChanged(sender, e);
        }

        private void drpEmployeeBranch_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.drpEmployeeTypes_SelectedIndexChanged(sender, e);
        }

        //IP - 01/06/09 - Credit Collection Walkthrough Changes
        //'Allocate Single Account' functionality now moved to 'BailReview5_2.cs'.
        private void drpBranchAcct_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.drpEmployeeTypesAcct_SelectedIndexChanged(sender, e);
        }

        private void btnAllocate_Click(object sender, System.EventArgs e)
        {
            /*taTable dtFields = fields.Tables["Deliveries"];
            int count = dtFields.Rows.Count;*/
            Wait();
            Function = "btnAllocate_Click";

            try
            {
                if (dgAccounts.CurrentRowIndex >= 0)
                {
                    //int empeeNo;

                    employee = (string)drpEmpNameResult.SelectedItem;
                    employee = employee.Substring(0, employee.IndexOf(":") - 1);
                    empeeNo = Convert.ToInt32(employee);

                    ArrayList al = new ArrayList();
                    CurrencyManager cm = (CurrencyManager)this.BindingContext[dgAccounts.DataSource, dgAccounts.DataMember];
                    DataView dv = (DataView)cm.List;

                    for (int i = 0; i < dv.Count; ++i)
                    {
                        // store selected rows in an array list
                        if (dgAccounts.IsSelected(i))
                            al.Add((string)dv[i][CN.acctno]);
                    }

                    // loop through array list and process selected accounts
                    foreach (String acct in al)
                    {
                        //AccountManager.AllocateAccount(acct, empeeNo, false, out  Error);
                        AccountManager.AllocateAccount(acct, empeeNo, out  Error); //IP - 08/10/09 - UAT(909)

                        if (Error.Length > 0)
                        {
                            ShowError(Error);
                            break;
                        }
                    }

                    for (int i = dv.Count - 1; i >= 0; i--)
                    {
                        if (al.Contains((string)dv[i][CN.acctno]))
                            dv.Delete(i);
                    }

                    //refresh rowcounts of allocations
                    this.drpEmployeeTypes2_SelectedIndexChanged(this, null);
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


        private void btnDeAllocate_Click(object sender, System.EventArgs e)
        {
            Wait();
            Function = "btnDeAllocate_Click";

            try
            {
                if (dgAccounts.CurrentRowIndex >= 0)
                {   //not sure why need employee filled here but filling anyway 
                    // as deallocating then can fill it with the select employee number as the allocate to employee number wont be filled
                    employee = (string)drpEmpName.SelectedItem;
                    employee = employee.Substring(0, employee.IndexOf(":") - 1);


                    ArrayList al = new ArrayList();
                    CurrencyManager cm = (CurrencyManager)this.BindingContext[dgAccounts.DataSource, dgAccounts.DataMember];
                    DataView dv = (DataView)cm.List;

                    for (int i = 0; i < dv.Count; ++i)
                    {
                        // Only interested in selected rows
                        if (dgAccounts.IsSelected(i))
                            al.Add((string)dgAccounts[i, 0]);
                    }

                    foreach (String acct in al)
                    {
                        AccountManager.DeAllocateAccount(acct, out Error);

                        if (Error.Length > 0)
                        {
                            ShowError(Error);
                            break;
                        }
                    }

                    for (int i = dv.Count - 1; i >= 0; i--)
                    {
                        if (al.Contains((string)dv[i][CN.acctno]))
                            dv.Delete(i);
                    }

                    //refresh rowcounts of allocations
                    this.drpEmployeeTypes2_SelectedIndexChanged(this, null);
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

        private void btnExcel_Click(object sender, System.EventArgs e)
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
                    string path = "C:\\Temp\\Accounts.csv";
                    FileInfo fi = new FileInfo(path);
                    if (fi.Exists)
                    {
                        fi.Delete();
                    }
                    else
                    {
                        var dir = @"C:\\Temp";
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                    }
                    FileStream fs = fi.OpenWrite();

                    string line = CN.acctno + comma +
                        CN.Status + comma +
                        CN.Balance + comma +
                        CN.Arrears + comma +
                        CN.ArrearsLevel + comma +
                        GetResource("T_ARREARSEXCHARGES") + comma +
                        CN.PercentagePaid + comma +
                        CN.Instalment + comma +
                        CN.CustomerID + comma +
                        CN.Title + comma +
                        CN.FirstName + comma +
                        CN.name + comma +
                        CN.cusaddr1 + comma +
                        CN.cusaddr2 + comma +
                        CN.cusaddr3 + comma +
                        CN.cuspocode + comma +
                        GetResource("T_EMPEENO") + comma +
                        GetResource("T_DATELASTPAID") + comma +
                        GetResource("T_DUEDAY") + comma +
                        GetResource("T_BDWBALANCE") + comma +
                        GetResource("T_BDWCHARGES") + comma + Environment.NewLine + Environment.NewLine;
                    byte[] blob = System.Text.Encoding.UTF8.GetBytes(line);
                    fs.Write(blob, 0, blob.Length);

                    foreach (DataRowView row in dv)
                    {
                        line = "'" + row[CN.acctno] + "'" + comma +
                            row[CN.Status] + comma +
                            row[CN.Balance] + comma +
                            row[CN.Arrears] + comma +
                            row[CN.ArrearsLevel2] + comma +
                            row[CN.ArrearsExCharges] + comma +
                            row[CN.PercentagePaid] + comma +
                            row[CN.Instalment] + comma +
                            row[CN.CustomerID] + comma +
                            row[CN.Title] + comma +
                            row[CN.FirstName] + comma +
                            row[CN.name] + comma +
                            row[CN.cusaddr1] + comma +
                            row[CN.cusaddr2] + comma +
                            row[CN.cusaddr3] + comma +
                            row[CN.cuspocode] + comma +
                            row[CN.EmployeeNo] + comma +
                            row[CN.DateLastPaid] + comma +
                            row[CN.DueDay] + comma +
                            row[CN.BDWBalance] + comma +
                            row[CN.BDWCharges] + Environment.NewLine;

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

        private void HashMenus()
        {
            dynamicMenus[this.Name + ":btnExcel"] = this.btnExcel;
            dynamicMenus[this.Name + ":loadBranches"] = this.loadBranches;
            dynamicMenus[this.Name + ":btnAllocate"] = this.btnAllocate;
            // dynamicMenus[this.Name + ":btnDeAllocate"] = this.btnDeAllocate; --IP - 01/06/09 - Credit Collection Walkthrough Changes. Deallocate button no longer required.
            // dynamicMenus[this.Name + ":showSingleAccount"] = this.showSingleAccount; --IP - 03/06/09 - Credit Collection Walkthrough Changes. Allocate Single Account moved to 'BailReview5_2.cs'.
        }

        private void btnRefresh_Click(object sender, System.EventArgs e)
        {
            this.drpEmployeeTypes2_SelectedIndexChanged(this, null);
        }

        private void drpAcctPoints_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            string scoreSelection = (string)drpAcctPoints.SelectedItem;
            scoreSelection = scoreSelection.Substring(0, scoreSelection.IndexOf("-") - 1);

            if (scoreSelection != "NL")
                proposalUpDown.Enabled = true;
            else
            {
                proposalUpDown.Value = 0;
                proposalUpDown.Enabled = false;
            }
        }

        private void btnClear_Click(object sender, System.EventArgs e)
        {
            staticdataloaded = false;
            drpAcctAllocation.SelectedIndex = 0;
            drpAcctStatus.SelectedIndex = 0;
            drpStatusSelection.SelectedIndex = 0;
            drpAcctMinStatus.SelectedIndex = 0;
            drpAcctArrears.SelectedIndex = 0;
            drpAcctLetter.SelectedIndex = 0;
            drpNumAcctions.SelectedIndex = 0;
            drpBalances.SelectedIndex = 0;
            drpDateMoved.SelectedIndex = 0;
            drpDateLastPaid.SelectedIndex = 0;
            drpAcctActions.SelectedIndex = 0;
            drpAcctCodes.SelectedIndex = 0;
            drpAcctPoints.SelectedIndex = 0;
            drpBranch.SelectedIndex = 0;
            drpActions.SelectedIndex = 0;
            drpLetters.SelectedIndex = 0;
            staticdataloaded = true;

            txtArrears.Value = 0;
            txtNumActions.Value = 0;
            txtBalances.Value = 0;
            proposalUpDown.Value = 0;

            dtDateLastPaid.Value = DateTime.Now;
            dateLetterFrom.Value = DateTime.Now;
            dateLetterTo.Value = DateTime.Now;
            dateActionFrom.Value = DateTime.Now;
            dateActionTo.Value = DateTime.Now;
            dateAllocatedFrom.Value = DateTime.Now.AddYears(-1);
            dateAllocatedTo.Value = DateTime.Now;
            dtMovedTo.Value = DateTime.Now;
            dtMovedFrom.Value = DateTime.Now.AddDays(-7);

            chxCharges.Checked = true;
            chxActions.Checked = false;
            rbAdded.Checked = true;

            drpEmployeeBranch.Text = Config.BranchCode;

            drpBranchAcct.Text = Config.BranchCode;

            drpAcctAllocation_SelectedIndexChanged(this, null);
            drpAcctArrears_SelectedIndexChanged(this, null);
            drpAcctLetter_SelectedIndexChanged(this, null);
            drpAcctActions_SelectedIndexChanged(this, null);

            dgAccounts.DataSource = null;
            dgAccounts.TableStyles.Clear();

            // 68315 RD 28/06/06 Allocate account status bar Added to clear the rows returned in the status bar
            statusBar1.Text = " ";

            // 68203 RD/AA 01/06/06 
            if (drpEmpName.Items.Count > 0)
            {
                drpEmpName.SelectedIndex = 0;
            }
        }

        private void drpNumAcctions_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (drpNumAcctions.SelectedIndex > 0)
                txtNumActions.Enabled = true;
            else
            {
                txtNumActions.Value = 0;
                txtNumActions.Enabled = false;
            }
        }

        private void drpBalances_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (drpBalances.SelectedIndex > 0)
                txtBalances.Enabled = true;
            else
            {
                txtBalances.Value = 0;
                txtBalances.Enabled = false;
            }
        }

        private void drpDateLastPaid_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (drpDateLastPaid.SelectedIndex > 0)
                dtDateLastPaid.Enabled = true;
            else
            {
                dtDateLastPaid.Enabled = false;
            }
        }

        private void drpDateMoved_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (drpDateMoved.SelectedIndex > 0)
            {
                dtMovedFrom.Enabled = true;
                dtMovedTo.Enabled = true;
            }
            else
            {
                dtMovedFrom.Enabled = false;
                dtMovedTo.Enabled = false;
            }
        }

        //IP - 01/06/09 - Credit Collection Walkthrough Changes
        //The 'Allocate Single Account' functionality has now been moved to BailReview5_2.cs.
        private void drpEmployeeTypesAcct_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            string empType;
            string empTypeStr;
            DataSet ds = null;
            string branch;
            creditstaff.Clear();

            try
            {
                if (drpEmployeeTypesAcct.SelectedIndex >= 0 && staticdataloaded == true)
                {
                    drpEmpNameAcct.Enabled = true;
                    drpEmpNameAcct.DataSource = null;
                    empTypeStr = (string)drpEmployeeTypesAcct.SelectedItem;
                    empType = empTypeStr.Split(':')[0].Trim();
                    branch = (string)drpBranchAcct.SelectedItem;
                    if (!IsNumeric(branch)) branch = "0";

                    //IP - 02/06/08 - Credit Collections - Altered to cater for (3) character Employee Types.
                    int empTypeStrRemove = empTypeStr.IndexOf("-") + 1;

                    //creditstaff.Add("0 : " + empTypeStr.Substring(empTypeStr.IndexOf("-") + 1, empTypeStr.Length - 3));
                    creditstaff.Add("0 : " + empTypeStr.Substring(empTypeStr.IndexOf("-") + 1, empTypeStr.Length - empTypeStrRemove));

                    ds = Login.GetStaffAllocationByType(Convert.ToInt32(empType), Convert.ToInt32(branch), out Error);

                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                    {
                        foreach (DataTable dt in ds.Tables)
                        {
                            if (dt.TableName == TN.SalesStaff)
                            {
                                foreach (DataRow row in dt.Rows)
                                {
                                    string str = Convert.ToString(row.ItemArray[0]) + " : " + (string)row.ItemArray[1];
                                    creditstaff.Add(str.ToUpper());
                                }
                            }
                        }
                        drpEmpNameAcct.DataSource = creditstaff;
                        if (creditstaff.Count > 0 && drpEmpNameAcct.Visible == true)
                        {
                            drpEmpNameAcct.SelectedIndex = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        //01/06/09 - Credit Collection Walkthrough Changes.
        //'Allocate Single Account' functionality moved to 'BailReview5_2.cs'.
        private void drpEmpNameAcct_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            string employee;
            //int empeeNo;

            if (drpEmpNameAcct.SelectedIndex >= 0)
            {
                employee = (string)drpEmpNameAcct.SelectedItem;
                employee = employee.Substring(0, employee.IndexOf(":") - 1);
                empeeNo = Convert.ToInt32(employee);

                string accountNo = txtAccountNo.UnformattedText;
                if (empeeNo != 0 && allocatedAccounts.Contains(accountNo))
                    btnAllocateAcct.Enabled = true;
                else
                    btnAllocateAcct.Enabled = false;
            }
        }

        //IP - 01/06/09 - Credit Collection Walkthrough Changes.
        //'Allocate Single Account' functionality moved to 'BailReview5_2.cs'.
        private void btnAllocateAcct_Click(object sender, System.EventArgs e)
        {
            // taTable dtFields = fields.Tables["Deliveries"];
            // int count = dtFields.Rows.Count;
            Wait();
            Function = "btnAllocateAcct_Click";

            acctNo = txtAccountNo.Text.Replace("-", "");

            //txtAccountNo.Text = acctNo;

            try
            {
                if (txtAccountNo.Text != "000-0000-0000-0")
                {
                    //int empeeNo;

                    employee = (string)drpEmpNameAcct.SelectedItem;
                    employee = employee.Substring(0, employee.IndexOf(":") - 1);
                    empeeNo = Convert.ToInt32(employee);

                    //AccountManager.AllocateAccount(txtAccountNo.Text.Replace("-", ""), empeeNo, false, out  Error);
                    AccountManager.AllocateAccount(txtAccountNo.Text.Replace("-", ""), empeeNo, out  Error); //IP - 08/10/09 - UAT(909)

                    if (Error.Length > 0)
                    {
                        ShowError(Error);

                    }
                    //refresh rowcounts of allocations
                    //    this.drpEmployeeTypesAcct_SelectedIndexChanged(this, null);

                    statusBar1.Text = "Account allocated";

                    txtAccountNo.Text = "000-0000-0000-0";

                    drpEmployeeTypesAcct.SelectedIndex = 0;
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

        //IP - 01/06/09 - Credit Collection Walkthrough Changes.
        //Moved 'Allocate Single Account' functionality to 'BailReview5_2.cs'.
        private void btnClearAcct_Click(object sender, EventArgs e)
        {
            statusBar1.Text = " ";

            txtAccountNo.Text = "000-0000-0000-0";

            drpEmployeeTypesAcct.SelectedIndex = 0;
            //drpEmpNameAcct.SelectedIndex = 0;

            if (drpEmpNameAcct.Items.Count > 0)
            {
                drpEmpNameAcct.SelectedIndex = 0;
            }
        }

        private void rbCredit_Click(object sender, EventArgs e)
        {
            if (rbCredit.Checked)
                drpAcctArrears.DataSource = acctselectionarrearsCredit;
        }

        private void rbCash_Click(object sender, EventArgs e)
        {
            if (rbCash.Checked)
                drpAcctArrears.DataSource = acctselectionarrearsCash;
        }

        private void drpBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (drpBranch.SelectedIndex == 1)
            {
                SetSelection selection = new SetSelection("Branches", 45, 195, 64, textBranchSet, TN.TNameBranch, TN.BranchNumber, false);
                selection.FormRoot = this.FormRoot;
                selection.FormParent = this;
                selection.ShowDialog(this);
                branchset = textBranchSet.Text;

            }
            else
                branchset = "";
        }

        private void drpAcctMinStatus_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void drpAcctStatus_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void drpStatusSelection_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (drpStatusSelection.SelectedIndex == 0) //no limitation
            {
                drpAcctMinStatus.Enabled = false;
                drpAcctStatus.Enabled = false;
            }
            if (drpStatusSelection.SelectedIndex == 1) //=
            {
                drpAcctMinStatus.Enabled = false;
                drpAcctStatus.Enabled = true;
            }
            else
                if (drpStatusSelection.SelectedIndex == 2) //between
                {
                    drpAcctMinStatus.Enabled = true;
                    drpAcctStatus.Enabled = true;
                }




        }

        //IP - 01/06/09 - Credit Collection Walkthrough Changes.
        //Moved 'Allocate Single Account' functionality to 'BailReview5_2.cs'.
        private void tcAllocated_SelectionChanged(object sender, EventArgs e)
        {
            if (tcAllocated.SelectedTab.Name == "tabPage1")
            {
                if (allocatedAccounts == null)
                {
                    //Populate the array list with those accounts which can be allocated.
                    DataSet dsStrategyAccounts = new DataSet();
                    dsStrategyAccounts = AccountManager.GetStrategyAccountsToAllocate(out Error);
                    allocatedAccounts = new ArrayList();
                    foreach (DataRow dtRow in dsStrategyAccounts.Tables[0].Rows)
                    {
                        allocatedAccounts.Add(dtRow[0].ToString());
                    }
                }
            }
        }

    }
}

