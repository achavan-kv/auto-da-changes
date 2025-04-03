using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using STL.Common;
using STL.Common.Constants.AccountHolders;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.SanctionStages;
using STL.Common.Constants.ScreenModes;
using STL.Common.Constants.TableNames;
using STL.Common.Static;
using STL.PL.WS7;



namespace STL.PL
{
    /// <summary>
    ///
    ///  Sanction Stage Two Mandatory Fields
    ///  -----------------------------------
    ///  
    ///  Hash Tables:
    ///    _formFields ~ to address all fields by name
    ///    _mandatoryFields ~ to address all mandatory fields by name
    ///    _visibleFields ~ to address all visible fields by name
    ///  
    ///  Data Tables:
    ///    _formFieldDef ~ list of fields from GetMandatoryFields for the main form
    ///    _referenceFieldDef ~ list of fields from GetMandatoryFields for one reference tab
    ///  
    ///  Procedures to control the form
    ///  ------------------------------
    ///  SetupFrame()
    ///    Creates hash tables and calls SetupForm() passing in the root control on
    ///    this form and the list of mandatory field definitions in _formFieldDef.
    ///  
    ///  SetupForm()
    ///    Calls SetupHashTable() and SetFieldProperties().
    ///  
    ///  SetupHashTable()
    ///    Starts with the root control and recursively traverses all the controls on
    ///    the form and adds the full name for each control to the _formFields hash table.
    ///  
    ///  SetFieldProperties()
    ///    Uses the mandatory field definitions in _formFieldDef to set the Visible
    ///    and Enabled property on each field, directly addressing each field by
    ///    full name via the _formFields hash table. If a field is also mandatory,
    ///    then it is added to the _mandatoryFields hash table and highlighted.
    ///    All visible fields are added to the _visibleFields hash table.
    ///    Calls SetFieldBias().
    ///  
    ///  SetFieldBias()
    ///    Sets the Enabled or ReadOnly property on each field depending on the type
    ///    of field from the list of mandatory field definitions in _formFieldDef, or
    ///    to ReadOnly if the whole form is ReadOnly.
    ///  
    ///  ValidateFields()
    ///    Checks for blank mandatory fields in the list of mandatory field definitions
    ///    in _formFieldDef.
    ///  
    ///  Procedures to control the Reference tab page
    ///  --------------------------------------------
    ///  
    ///  CreateReferenceTab()
    ///    Calls SetupForm() passing in the root control on this tab
    ///    and the list of mandatory field definitions in _referenceFieldDef.
    ///    The rest of the procedures are used as above. The only difference is
    ///    that the root control is now the tab instead of the form, and the list
    ///    of mandatory field definitions is _referenceFieldDef instead of _formFieldDef.
    ///    When another reference tab is added it is distinguised from the others by
    ///    suffixing the name of the root control with a number. The list of mandatory
    ///    field definitions in _referenceFieldDef is re-used (because all the tabs are
    ///    the same).
    ///  
    ///  ValidateFields()
    ///    Also checks at least one reference tab has been added.
    ///
    ///  Notes
    ///  -----
    ///  
    ///  When Stage 2 is closed all of the fields are displayed as ReadOnly. To change a
    ///  field between ReadOnly and not ReadOnly it must be listed in either _referenceFieldDef
    ///  or _formFieldDef. Therefore these fields must be listed in the MandatoryFields table.
    ///  (The only exception is a field that is permanently ReadOnly.)
    ///  
    ///  The reference tab contains some field names that are the same as field names elswhere
    ///  on the Sanction Stage Two form. Therefore, the full name with dot notation is used to
    ///  identify each field. The full name has to be entered into the MandatoryFields table.
    ///  
    /// </summary>
    public class SanctionStage2 : CommonForm
    {

        //
        // Form control
        //
        private const short _maxReferences = 5;
        private string _controlTree;
        private DataSet _formFieldDef = null;
        private DataSet _referenceFieldDef = null;
        private Hashtable _formFields = null;
        private Hashtable _mandatoryFields = null;
        private Hashtable _visibleFields = null;
        public Hashtable _inError = null;
        private string ResidentialStatus = String.Empty;

        //
        // Local properties
        //
        private bool _readOnly = false;
        public bool ReadOnly
        {
            get { return _readOnly; }
            set { _readOnly = value; }
        }

        private bool _confirm = true;
        public bool Confirm
        {
            get { return _confirm; }
            set { _confirm = value; }
        }

        private string _currentStatus = String.Empty;
        public string CurrentStatus
        {
            get { return _currentStatus; }
            set { _currentStatus = value; }
        }

        private bool _locked = false;
        public bool AccountLocked
        {
            get { return _locked; }
            set { _locked = value; }
        }
        private string _acctno = String.Empty;
        public string AccountNo
        {
            get { return _acctno; }
            set { _acctno = value; }
        }

        private string _acctType = String.Empty;
        public string acctType
        {
            get { return _acctType; }
            set { _acctType = value; }
        }

        private string _custid = String.Empty;
        public string CustomerID
        {
            get { return _custid; }
            set { _custid = value; }
        }

        private string _screenMode = SM.Edit;
        public string ScreenMode
        {
            get { return _screenMode; }
            set { _screenMode = value; }
        }


        private string _error = String.Empty;
        private DataSet _stage2DataA1 = null;
        private DataSet _stage2DataA2 = null;
        private DataSet _dropDownSet = null;
        private DateTime _dateProp;
        private int _yearsInCurrentAddress = 0;
        private const string _unemployed = "U";
        private string _empStatus = String.Empty;
        private BasicCustomerDetails _customerScreen = null;
        private TelephoneAction5_2 _telephoneAction = null;     //CR1084
        private BailReview5_2 _bailReview = null;     //CR1084


        //
        // Form properties
        //
        private System.Windows.Forms.GroupBox gbData;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox drpApplicationType;
        private System.Windows.Forms.ComboBox drpIDSelection;
        private System.Windows.Forms.GroupBox gpCustomer;
        private System.Windows.Forms.TextBox txtCustomerID;
        private System.Windows.Forms.DateTimePicker dtDateProp;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtLastName;
        private System.Windows.Forms.TextBox txtFirstName;
        private Crownwood.Magic.Controls.TabPage tpApp1;
        private Crownwood.Magic.Controls.TabPage tpPreviousAddress;
        private Crownwood.Magic.Controls.TabPage tpEmployer;
        private Crownwood.Magic.Controls.TabPage tpReferences;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnEnter;
        private Crownwood.Magic.Controls.TabControl tcApplicants;
        private Crownwood.Magic.Controls.TabControl tcApp1;
        private TextBox txtStaffNo;
        private System.Windows.Forms.TextBox txtEmpDept;
        private System.Windows.Forms.TextBox txtEmpName;
        private System.Windows.Forms.ComboBox drpSpecialPromo;
        private System.Windows.Forms.GroupBox grpPrevAddress;
        private System.Windows.Forms.Label lSpecialPromo;
        private System.Windows.Forms.GroupBox grpEmployer;
        private System.Windows.Forms.Label lStaffNo;
        private System.Windows.Forms.Label lEmpDept;
        private System.Windows.Forms.Label lEmpName;
        private System.Windows.Forms.Label lEmpAddress;
        private System.Windows.Forms.Label lCustomerID;
        private System.Windows.Forms.Button btnComplete;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private Crownwood.Magic.Menus.MenuCommand menuFile;
        private Crownwood.Magic.Menus.MenuCommand menuExit;
        private System.Windows.Forms.ImageList menuIcons;
        private System.Windows.Forms.ToolTip toolTip1;
        private STL.PL.AddressTab tcPrevAddress;
        private STL.PL.AddressTab tcEmpAddress;
        private Crownwood.Magic.Menus.MenuCommand menuSanction;
        private Crownwood.Magic.Menus.MenuCommand menuReopen;
        private Crownwood.Magic.Menus.MenuCommand menuSave;
        private Crownwood.Magic.Menus.MenuCommand menuComplete;
        private Crownwood.Magic.Controls.TabControl tcReferrences;
        private System.ComponentModel.IContainer components;
        private Crownwood.Magic.Controls.TabPage tpComments;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RichTextBox txtNewComment;
        private System.Windows.Forms.RichTextBox txtComment;
        private System.Windows.Forms.Label lNewS1Comment;
        private System.Windows.Forms.Label lRegistration;
        private System.Windows.Forms.TextBox txtRegistration;
        private Crownwood.Magic.Controls.TabPage tpApp2;
        private Crownwood.Magic.Controls.TabControl tcApp2;
        private Crownwood.Magic.Controls.TabPage tpEmployer2;
        private System.Windows.Forms.TextBox txtEmpDept2;
        private System.Windows.Forms.TextBox txtEmpName2;
        private System.Windows.Forms.Label lStaffNo2;
        private System.Windows.Forms.Label lEmpDept2;
        private System.Windows.Forms.TextBox txtStaffNo2;
        private System.Windows.Forms.Label lEmpName2;
        private System.Windows.Forms.Label lEmpAddress2;
        private System.Windows.Forms.GroupBox grpEmployer2;
        public System.Windows.Forms.TextBox txtAddress2;
        public System.Windows.Forms.TextBox txtAddress1;
        public System.Windows.Forms.TextBox txtPostCode;
        public System.Windows.Forms.TextBox txtDialCode2;
        public System.Windows.Forms.TextBox txtPhoneNo2;
        private System.Windows.Forms.Label lPhone;
        private System.Windows.Forms.Button btnPreviousRefs;

        //
        // Constructors
        //
        public SanctionStage2(TranslationDummy d)
        {
            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile, menuSanction });
        }

        public SanctionStage2(string custId, DateTime dateProp, string accountNo,
                            string acctType, string mode,
                            Form root, Form parent, BasicCustomerDetails customerScreen)
        {
            //
            // Required for Windows Form Designer support
            //

            FormRoot = root;
            FormParent = parent;
            InitializeComponent();          //CR1084 moved to after FormRoot initialise
            _customerScreen = customerScreen;

            Function = "SanctionStage2()";
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile, menuSanction });

            // 5.1 uat138 rdb 7/12/07 fix to hide EmpAddress label
            tcEmpAddress.txtAddress1.Tag = "lEmpAddress";

            HashMenus();
            this.ApplyRoleRestrictions();

            try
            {
                Wait();

                switch (mode)
                {
                    case SM.New: _readOnly = false;
                        break;
                    case SM.Edit: _readOnly = false;
                        break;
                    case SM.View: _readOnly = true;
                        break;
                    default:
                        break;
                }
                Confirm = !_readOnly;
                btnComplete.Enabled = !_readOnly;
                //menuSave.Enabled = !_readOnly;

                this.CustomerID = custId;
                _dateProp = this.dtDateProp.Value = dateProp;
                this.AccountNo = accountNo;
                this.acctType = acctType;
                ScreenMode = mode;
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

        public SanctionStage2(string custId, DateTime dateProp, string accountNo,
                            string acctType, string mode,
                            Form root, Form parent, TelephoneAction5_2 telephoneAction)
        {
            //
            // Required for Windows Form Designer support
            //

            FormRoot = root;
            FormParent = parent;
            InitializeComponent();          //CR1084 moved to after FormRoot initialise
            _telephoneAction = telephoneAction;

            Function = "SanctionStage2()";
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile, menuSanction });

            // 5.1 uat138 rdb 7/12/07 fix to hide EmpAddress label
            tcEmpAddress.txtAddress1.Tag = "lEmpAddress";

            HashMenus();
            this.ApplyRoleRestrictions();

            try
            {
                Wait();

                switch (mode)
                {
                    case SM.New: _readOnly = false;
                        break;
                    case SM.Edit: _readOnly = false;
                        break;
                    case SM.View: _readOnly = true;
                        break;
                    default:
                        break;
                }
                Confirm = !_readOnly;
                btnComplete.Enabled = !_readOnly;
                //menuSave.Enabled = !_readOnly;

                this.CustomerID = custId;
                _dateProp = this.dtDateProp.Value = dateProp;
                this.AccountNo = accountNo;
                this.acctType = acctType;
                ScreenMode = mode;

                tpReferences.Selected = true;     //CR1084
                //((MainForm)this.FormRoot).tbSanction.Visible = false;
                //tcApp1.TabPages.Remove(tpPreviousAddress);
                //tcApp1.TabPages.Remove(tpEmployer);
                //tcApp1.TabPages.Remove(tpComments);


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

        public SanctionStage2(string custId, DateTime dateProp, string accountNo,
                            string acctType, string mode,
                            Form root, Form parent, BailReview5_2 bailReview)
        {
            //
            // Required for Windows Form Designer support
            //

            FormRoot = root;
            FormParent = parent;
            InitializeComponent();          //CR1084 moved to after FormRoot initialise
            _bailReview = bailReview;

            Function = "SanctionStage2()";
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile, menuSanction });

            // 5.1 uat138 rdb 7/12/07 fix to hide EmpAddress label
            tcEmpAddress.txtAddress1.Tag = "lEmpAddress";

            HashMenus();
            this.ApplyRoleRestrictions();

            try
            {
                Wait();

                switch (mode)
                {
                    case SM.New: _readOnly = false;
                        break;
                    case SM.Edit: _readOnly = false;
                        break;
                    case SM.View: _readOnly = true;
                        break;
                    default:
                        break;
                }
                Confirm = !_readOnly;
                btnComplete.Enabled = !_readOnly;
                //menuSave.Enabled = !_readOnly;

                this.CustomerID = custId;
                _dateProp = this.dtDateProp.Value = dateProp;
                this.AccountNo = accountNo;
                this.acctType = acctType;
                ScreenMode = mode;

                tpReferences.Selected = true;


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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SanctionStage2));
            this.gbData = new System.Windows.Forms.GroupBox();
            this.btnComplete = new System.Windows.Forms.Button();
            this.menuIcons = new System.Windows.Forms.ImageList(this.components);
            this.tcApplicants = new Crownwood.Magic.Controls.TabControl();
            this.tpApp1 = new Crownwood.Magic.Controls.TabPage();
            this.tcApp1 = new Crownwood.Magic.Controls.TabControl();
            this.tpPreviousAddress = new Crownwood.Magic.Controls.TabPage();
            this.txtRegistration = new System.Windows.Forms.TextBox();
            this.lRegistration = new System.Windows.Forms.Label();
            this.lSpecialPromo = new System.Windows.Forms.Label();
            this.drpSpecialPromo = new System.Windows.Forms.ComboBox();
            this.grpPrevAddress = new System.Windows.Forms.GroupBox();
            this.tcPrevAddress = new STL.PL.AddressTab(FormRoot);           //CR1084 jec
            this.tpEmployer = new Crownwood.Magic.Controls.TabPage();
            this.grpEmployer = new System.Windows.Forms.GroupBox();
            this.tcEmpAddress = new STL.PL.AddressTab(FormRoot);            //CR1084 jec
            this.txtEmpDept = new System.Windows.Forms.TextBox();
            this.txtEmpName = new System.Windows.Forms.TextBox();
            this.lStaffNo = new System.Windows.Forms.Label();
            this.lEmpDept = new System.Windows.Forms.Label();
            this.txtStaffNo = new System.Windows.Forms.TextBox();
            this.lEmpName = new System.Windows.Forms.Label();
            this.lEmpAddress = new System.Windows.Forms.Label();
            this.tpReferences = new Crownwood.Magic.Controls.TabPage();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnEnter = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tcReferrences = new Crownwood.Magic.Controls.TabControl();
            this.btnPreviousRefs = new System.Windows.Forms.Button();
            this.tpComments = new Crownwood.Magic.Controls.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtNewComment = new System.Windows.Forms.RichTextBox();
            this.txtComment = new System.Windows.Forms.RichTextBox();
            this.lNewS1Comment = new System.Windows.Forms.Label();
            this.tpApp2 = new Crownwood.Magic.Controls.TabPage();
            this.tcApp2 = new Crownwood.Magic.Controls.TabControl();
            this.tpEmployer2 = new Crownwood.Magic.Controls.TabPage();
            this.grpEmployer2 = new System.Windows.Forms.GroupBox();
            this.lPhone = new System.Windows.Forms.Label();
            this.txtDialCode2 = new System.Windows.Forms.TextBox();
            this.txtPhoneNo2 = new System.Windows.Forms.TextBox();
            this.txtPostCode = new System.Windows.Forms.TextBox();
            this.txtAddress2 = new System.Windows.Forms.TextBox();
            this.txtAddress1 = new System.Windows.Forms.TextBox();
            this.txtEmpDept2 = new System.Windows.Forms.TextBox();
            this.txtEmpName2 = new System.Windows.Forms.TextBox();
            this.lStaffNo2 = new System.Windows.Forms.Label();
            this.lEmpDept2 = new System.Windows.Forms.Label();
            this.txtStaffNo2 = new System.Windows.Forms.TextBox();
            this.lEmpName2 = new System.Windows.Forms.Label();
            this.lEmpAddress2 = new System.Windows.Forms.Label();
            this.drpApplicationType = new System.Windows.Forms.ComboBox();
            this.drpIDSelection = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.gpCustomer = new System.Windows.Forms.GroupBox();
            this.txtCustomerID = new System.Windows.Forms.TextBox();
            this.dtDateProp = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lCustomerID = new System.Windows.Forms.Label();
            this.txtLastName = new System.Windows.Forms.TextBox();
            this.txtFirstName = new System.Windows.Forms.TextBox();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.menuSanction = new Crownwood.Magic.Menus.MenuCommand();
            this.menuSave = new Crownwood.Magic.Menus.MenuCommand();
            this.menuComplete = new Crownwood.Magic.Menus.MenuCommand();
            this.menuReopen = new Crownwood.Magic.Menus.MenuCommand();
            this.gbData.SuspendLayout();
            this.tcApplicants.SuspendLayout();
            this.tpApp1.SuspendLayout();
            this.tcApp1.SuspendLayout();
            this.tpPreviousAddress.SuspendLayout();
            this.grpPrevAddress.SuspendLayout();
            this.tpEmployer.SuspendLayout();
            this.grpEmployer.SuspendLayout();
            this.tpReferences.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tpComments.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tpApp2.SuspendLayout();
            this.tcApp2.SuspendLayout();
            this.tpEmployer2.SuspendLayout();
            this.grpEmployer2.SuspendLayout();
            this.gpCustomer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // gbData
            // 
            this.gbData.BackColor = System.Drawing.SystemColors.Control;
            this.gbData.Controls.Add(this.btnComplete);
            this.gbData.Controls.Add(this.tcApplicants);
            this.gbData.Controls.Add(this.drpApplicationType);
            this.gbData.Controls.Add(this.drpIDSelection);
            this.gbData.Controls.Add(this.label8);
            this.gbData.Controls.Add(this.label10);
            this.gbData.Location = new System.Drawing.Point(8, 64);
            this.gbData.Name = "gbData";
            this.gbData.Size = new System.Drawing.Size(776, 408);
            this.gbData.TabIndex = 1;
            this.gbData.TabStop = false;
            this.gbData.Text = "Stage 2 Applicant Data";
            // 
            // btnComplete
            // 
            this.btnComplete.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnComplete.ImageIndex = 1;
            this.btnComplete.ImageList = this.menuIcons;
            this.btnComplete.Location = new System.Drawing.Point(736, 18);
            this.btnComplete.Name = "btnComplete";
            this.btnComplete.Size = new System.Drawing.Size(20, 18);
            this.btnComplete.TabIndex = 21;
            this.btnComplete.Click += new System.EventHandler(this.btnComplete_Click);
            // 
            // menuIcons
            // 
            this.menuIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("menuIcons.ImageStream")));
            this.menuIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.menuIcons.Images.SetKeyName(0, "");
            this.menuIcons.Images.SetKeyName(1, "");
            this.menuIcons.Images.SetKeyName(2, "");
            // 
            // tcApplicants
            // 
            this.tcApplicants.ControlTopOffset = 10;
            this.tcApplicants.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.tcApplicants.IDEPixelArea = true;
            this.tcApplicants.Location = new System.Drawing.Point(8, 48);
            this.tcApplicants.Name = "tcApplicants";
            this.tcApplicants.PositionTop = true;
            this.tcApplicants.SelectedIndex = 0;
            this.tcApplicants.SelectedTab = this.tpApp1;
            this.tcApplicants.Size = new System.Drawing.Size(760, 352);
            this.tcApplicants.TabIndex = 12;
            this.tcApplicants.TabPages.AddRange(new Crownwood.Magic.Controls.TabPage[] {
            this.tpApp1,
            this.tpApp2});
            // 
            // tpApp1
            // 
            this.tpApp1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tpApp1.Controls.Add(this.tcApp1);
            this.tpApp1.Location = new System.Drawing.Point(0, 35);
            this.tpApp1.Name = "tpApp1";
            this.tpApp1.Size = new System.Drawing.Size(760, 317);
            this.tpApp1.TabIndex = 3;
            this.tpApp1.Title = "Applicant 1";
            // 
            // tcApp1
            // 
            this.tcApp1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.tcApp1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcApp1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.tcApp1.HotTextColor = System.Drawing.SystemColors.Highlight;
            this.tcApp1.IDEPixelArea = true;
            this.tcApp1.Location = new System.Drawing.Point(0, 0);
            this.tcApp1.Name = "tcApp1";
            this.tcApp1.PositionTop = true;
            this.tcApp1.SelectedIndex = 1;
            this.tcApp1.SelectedTab = this.tpEmployer;
            this.tcApp1.Size = new System.Drawing.Size(756, 313);
            this.tcApp1.TabIndex = 1;
            this.tcApp1.TabPages.AddRange(new Crownwood.Magic.Controls.TabPage[] {
            this.tpPreviousAddress,
            this.tpEmployer,
            this.tpReferences,
            this.tpComments});
            // 
            // tpPreviousAddress
            // 
            this.tpPreviousAddress.BackColor = System.Drawing.SystemColors.ControlLight;
            this.tpPreviousAddress.Controls.Add(this.txtRegistration);
            this.tpPreviousAddress.Controls.Add(this.lRegistration);
            this.tpPreviousAddress.Controls.Add(this.lSpecialPromo);
            this.tpPreviousAddress.Controls.Add(this.drpSpecialPromo);
            this.tpPreviousAddress.Controls.Add(this.grpPrevAddress);
            this.tpPreviousAddress.Location = new System.Drawing.Point(0, 25);
            this.tpPreviousAddress.Name = "tpPreviousAddress";
            this.tpPreviousAddress.Selected = false;
            this.tpPreviousAddress.Size = new System.Drawing.Size(756, 288);
            this.tpPreviousAddress.TabIndex = 3;
            this.tpPreviousAddress.Title = "Previous Address";
            // 
            // txtRegistration
            // 
            this.txtRegistration.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.txtRegistration.Location = new System.Drawing.Point(344, 64);
            this.txtRegistration.MaxLength = 15;
            this.txtRegistration.Name = "txtRegistration";
            this.txtRegistration.Size = new System.Drawing.Size(100, 20);
            this.txtRegistration.TabIndex = 2;
            this.txtRegistration.Tag = "lRegistration";
            // 
            // lRegistration
            // 
            this.lRegistration.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lRegistration.Location = new System.Drawing.Point(226, 62);
            this.lRegistration.Name = "lRegistration";
            this.lRegistration.Size = new System.Drawing.Size(112, 23);
            this.lRegistration.TabIndex = 26;
            this.lRegistration.Text = "Vehicle Registration ";
            this.lRegistration.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lSpecialPromo
            // 
            this.lSpecialPromo.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lSpecialPromo.Location = new System.Drawing.Point(234, 22);
            this.lSpecialPromo.Name = "lSpecialPromo";
            this.lSpecialPromo.Size = new System.Drawing.Size(104, 23);
            this.lSpecialPromo.TabIndex = 4;
            this.lSpecialPromo.Text = "Special Promotion";
            this.lSpecialPromo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // drpSpecialPromo
            // 
            this.drpSpecialPromo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpSpecialPromo.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.drpSpecialPromo.Location = new System.Drawing.Point(344, 24);
            this.drpSpecialPromo.Name = "drpSpecialPromo";
            this.drpSpecialPromo.Size = new System.Drawing.Size(72, 21);
            this.drpSpecialPromo.TabIndex = 1;
            this.drpSpecialPromo.Tag = "lSpecialPromo";
            this.drpSpecialPromo.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // grpPrevAddress
            // 
            this.grpPrevAddress.Controls.Add(this.tcPrevAddress);
            this.grpPrevAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.grpPrevAddress.Location = new System.Drawing.Point(186, 104);
            this.grpPrevAddress.Name = "grpPrevAddress";
            this.grpPrevAddress.Size = new System.Drawing.Size(384, 168);
            this.grpPrevAddress.TabIndex = 2;
            this.grpPrevAddress.TabStop = false;
            this.grpPrevAddress.Text = "Previous Address";
            // 
            // tcPrevAddress
            // 
            this.tcPrevAddress.Location = new System.Drawing.Point(100, 32);
            this.tcPrevAddress.Name = "tcPrevAddress";
            this.tcPrevAddress.ReadOnly = false;
            this.tcPrevAddress.SimpleAddress = true;
            this.tcPrevAddress.Size = new System.Drawing.Size(250, 112);
            this.tcPrevAddress.TabIndex = 0;
            this.tcPrevAddress.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            this.tcPrevAddress.Title.Visible = false;
            this.tcPrevAddress.FirstName.Visible = false;
            this.tcPrevAddress.LastName.Visible = false;
            this.tcPrevAddress.drptitleC.Visible = false;
            this.tcPrevAddress.CFirstname.Visible = false;
            this.tcPrevAddress.CLastname.Visible = false;
            // 
            // tpEmployer
            // 
            this.tpEmployer.BackColor = System.Drawing.SystemColors.ControlLight;
            this.tpEmployer.Controls.Add(this.grpEmployer);
            this.tpEmployer.Location = new System.Drawing.Point(0, 25);
            this.tpEmployer.Name = "tpEmployer";
            this.tpEmployer.Size = new System.Drawing.Size(756, 288);
            this.tpEmployer.TabIndex = 4;
            this.tpEmployer.Title = "Employer Details";
            this.tpEmployer.Visible = false;
            // 
            // grpEmployer
            // 
            this.grpEmployer.Controls.Add(this.tcEmpAddress);
            this.grpEmployer.Controls.Add(this.txtEmpDept);
            this.grpEmployer.Controls.Add(this.txtEmpName);
            this.grpEmployer.Controls.Add(this.lStaffNo);
            this.grpEmployer.Controls.Add(this.lEmpDept);
            this.grpEmployer.Controls.Add(this.txtStaffNo);
            this.grpEmployer.Controls.Add(this.lEmpName);
            this.grpEmployer.Controls.Add(this.lEmpAddress);
            this.grpEmployer.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.grpEmployer.Location = new System.Drawing.Point(138, 28);
            this.grpEmployer.Name = "grpEmployer";
            this.grpEmployer.Size = new System.Drawing.Size(480, 232);
            this.grpEmployer.TabIndex = 12;
            this.grpEmployer.TabStop = false;
            this.grpEmployer.Text = "Employer";
            // 
            // tcEmpAddress
            // 
            this.tcEmpAddress.Location = new System.Drawing.Point(158, 52);
            this.tcEmpAddress.Name = "tcEmpAddress";
            this.tcEmpAddress.ReadOnly = false;
            this.tcEmpAddress.SimpleAddress = true;
            this.tcEmpAddress.Size = new System.Drawing.Size(256, 111);
            this.tcEmpAddress.TabIndex = 2;
            this.tcEmpAddress.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            this.tcEmpAddress.Title.Visible = false;
            this.tcEmpAddress.FirstName.Visible = false;
            this.tcEmpAddress.LastName.Visible = false;
            this.tcEmpAddress.drptitleC.Visible = false;
            this.tcEmpAddress.CFirstname.Visible = false;
            this.tcEmpAddress.CLastname.Visible = false;
            this.tcEmpAddress.txtCoordinate.Visible = false; // Address Standardization CR2019 - 025
            // 
            // txtEmpDept
            // 
            this.txtEmpDept.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.txtEmpDept.Location = new System.Drawing.Point(168, 168);
            this.txtEmpDept.MaxLength = 26;
            this.txtEmpDept.Name = "txtEmpDept";
            this.txtEmpDept.Size = new System.Drawing.Size(160, 20);
            this.txtEmpDept.TabIndex = 3;
            this.txtEmpDept.Tag = "lEmpDept";
            this.txtEmpDept.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // txtEmpName
            // 
            this.txtEmpName.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.txtEmpName.Location = new System.Drawing.Point(168, 24);
            this.txtEmpName.MaxLength = 26;
            this.txtEmpName.Name = "txtEmpName";
            this.txtEmpName.Size = new System.Drawing.Size(160, 20);
            this.txtEmpName.TabIndex = 1;
            this.txtEmpName.Tag = "lEmpName";
            this.txtEmpName.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // lStaffNo
            // 
            this.lStaffNo.CausesValidation = false;
            this.lStaffNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lStaffNo.Location = new System.Drawing.Point(30, 192);
            this.lStaffNo.Name = "lStaffNo";
            this.lStaffNo.Size = new System.Drawing.Size(112, 16);
            this.lStaffNo.TabIndex = 19;
            this.lStaffNo.Text = "Staff/works number";
            this.lStaffNo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lEmpDept
            // 
            this.lEmpDept.CausesValidation = false;
            this.lEmpDept.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lEmpDept.Location = new System.Drawing.Point(62, 168);
            this.lEmpDept.Name = "lEmpDept";
            this.lEmpDept.Size = new System.Drawing.Size(80, 16);
            this.lEmpDept.TabIndex = 18;
            this.lEmpDept.Text = "Department";
            this.lEmpDept.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtStaffNo
            // 
            this.txtStaffNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.txtStaffNo.Location = new System.Drawing.Point(168, 192);
            this.txtStaffNo.MaxLength = 20;
            this.txtStaffNo.Name = "txtStaffNo";
            this.txtStaffNo.Size = new System.Drawing.Size(140, 20);
            this.txtStaffNo.TabIndex = 4;
            this.txtStaffNo.Tag = "lStaffNo";
            this.txtStaffNo.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // lEmpName
            // 
            this.lEmpName.CausesValidation = false;
            this.lEmpName.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lEmpName.Location = new System.Drawing.Point(86, 24);
            this.lEmpName.Name = "lEmpName";
            this.lEmpName.Size = new System.Drawing.Size(56, 16);
            this.lEmpName.TabIndex = 13;
            this.lEmpName.Text = "Name";
            this.lEmpName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lEmpAddress
            // 
            this.lEmpAddress.CausesValidation = false;
            this.lEmpAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lEmpAddress.Location = new System.Drawing.Point(86, 66);
            this.lEmpAddress.Name = "lEmpAddress";
            this.lEmpAddress.Size = new System.Drawing.Size(56, 16);
            this.lEmpAddress.TabIndex = 12;
            this.lEmpAddress.Text = "Address";
            this.lEmpAddress.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tpReferences
            // 
            this.tpReferences.BackColor = System.Drawing.SystemColors.ControlLight;
            this.tpReferences.Controls.Add(this.btnRemove);
            this.tpReferences.Controls.Add(this.btnEnter);
            this.tpReferences.Controls.Add(this.groupBox1);
            this.tpReferences.Controls.Add(this.btnPreviousRefs);
            this.tpReferences.Location = new System.Drawing.Point(0, 25);
            this.tpReferences.Name = "tpReferences";
            this.tpReferences.Selected = false;
            this.tpReferences.Size = new System.Drawing.Size(756, 288);
            this.tpReferences.TabIndex = 5;
            this.tpReferences.Title = "References";
            this.tpReferences.Visible = false;
            // 
            // btnRemove
            // 
            this.btnRemove.BackColor = System.Drawing.Color.SlateBlue;
            this.btnRemove.Enabled = false;
            this.btnRemove.Font = new System.Drawing.Font("Arial Narrow", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRemove.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.btnRemove.Image = global::STL.PL.Properties.Resources.Minus;
            this.btnRemove.Location = new System.Drawing.Point(733, 126);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(20, 20);
            this.btnRemove.TabIndex = 8;
            this.btnRemove.UseVisualStyleBackColor = false;
            this.btnRemove.Visible = false;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnEnter
            // 
            this.btnEnter.BackColor = System.Drawing.Color.SlateBlue;
            this.btnEnter.Font = new System.Drawing.Font("Arial Narrow", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEnter.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.btnEnter.Image = global::STL.PL.Properties.Resources.plus;
            this.btnEnter.Location = new System.Drawing.Point(733, 94);
            this.btnEnter.Name = "btnEnter";
            this.btnEnter.Size = new System.Drawing.Size(20, 20);
            this.btnEnter.TabIndex = 7;
            this.btnEnter.UseVisualStyleBackColor = false;
            this.btnEnter.Click += new System.EventHandler(this.btnEnter_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tcReferrences);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(8, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(719, 272);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "References";
            // 
            // tcReferrences
            // 
            this.tcReferrences.IDEPixelArea = true;
            this.tcReferrences.Location = new System.Drawing.Point(12, 26);
            this.tcReferrences.Name = "tcReferrences";
            this.tcReferrences.Size = new System.Drawing.Size(694, 233);
            this.tcReferrences.TabIndex = 6;
            // 
            // btnPreviousRefs
            // 
            this.btnPreviousRefs.BackColor = System.Drawing.SystemColors.HotTrack;
            this.btnPreviousRefs.Image = ((System.Drawing.Image)(resources.GetObject("btnPreviousRefs.Image")));
            this.btnPreviousRefs.Location = new System.Drawing.Point(729, 15);
            this.btnPreviousRefs.Name = "btnPreviousRefs";
            this.btnPreviousRefs.Size = new System.Drawing.Size(22, 23);
            this.btnPreviousRefs.TabIndex = 9;
            this.btnPreviousRefs.UseVisualStyleBackColor = false;
            this.btnPreviousRefs.Click += new System.EventHandler(this.btnPreviousRefs_Click);
            // 
            // tpComments
            // 
            this.tpComments.Controls.Add(this.groupBox2);
            this.tpComments.Location = new System.Drawing.Point(0, 25);
            this.tpComments.Name = "tpComments";
            this.tpComments.Selected = false;
            this.tpComments.Size = new System.Drawing.Size(756, 288);
            this.tpComments.TabIndex = 6;
            this.tpComments.Title = "Comments";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtNewComment);
            this.groupBox2.Controls.Add(this.txtComment);
            this.groupBox2.Controls.Add(this.lNewS1Comment);
            this.groupBox2.Location = new System.Drawing.Point(10, 8);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(736, 272);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Comments";
            // 
            // txtNewComment
            // 
            this.txtNewComment.Location = new System.Drawing.Point(24, 24);
            this.txtNewComment.MaxLength = 1000;
            this.txtNewComment.Name = "txtNewComment";
            this.txtNewComment.Size = new System.Drawing.Size(684, 88);
            this.txtNewComment.TabIndex = 1;
            this.txtNewComment.Tag = "lNewS1Comment";
            this.txtNewComment.Text = "";
            // 
            // txtComment
            // 
            this.txtComment.BackColor = System.Drawing.SystemColors.Control;
            this.txtComment.Location = new System.Drawing.Point(24, 112);
            this.txtComment.MaxLength = 1000;
            this.txtComment.Name = "txtComment";
            this.txtComment.ReadOnly = true;
            this.txtComment.Size = new System.Drawing.Size(684, 144);
            this.txtComment.TabIndex = 0;
            this.txtComment.Text = "";
            // 
            // lNewS1Comment
            // 
            this.lNewS1Comment.Location = new System.Drawing.Point(136, 48);
            this.lNewS1Comment.Name = "lNewS1Comment";
            this.lNewS1Comment.Size = new System.Drawing.Size(80, 16);
            this.lNewS1Comment.TabIndex = 2;
            this.lNewS1Comment.Text = "dummy";
            // 
            // tpApp2
            // 
            this.tpApp2.Controls.Add(this.tcApp2);
            this.tpApp2.Location = new System.Drawing.Point(0, 35);
            this.tpApp2.Name = "tpApp2";
            this.tpApp2.Selected = false;
            this.tpApp2.Size = new System.Drawing.Size(760, 317);
            this.tpApp2.TabIndex = 4;
            this.tpApp2.Title = "Applicant 2";
            // 
            // tcApp2
            // 
            this.tcApp2.BackColor = System.Drawing.SystemColors.ControlLight;
            this.tcApp2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcApp2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.tcApp2.HotTextColor = System.Drawing.SystemColors.Highlight;
            this.tcApp2.IDEPixelArea = true;
            this.tcApp2.Location = new System.Drawing.Point(0, 0);
            this.tcApp2.Name = "tcApp2";
            this.tcApp2.PositionTop = true;
            this.tcApp2.SelectedIndex = 0;
            this.tcApp2.SelectedTab = this.tpEmployer2;
            this.tcApp2.Size = new System.Drawing.Size(760, 317);
            this.tcApp2.TabIndex = 2;
            this.tcApp2.TabPages.AddRange(new Crownwood.Magic.Controls.TabPage[] {
            this.tpEmployer2});
            // 
            // tpEmployer2
            // 
            this.tpEmployer2.BackColor = System.Drawing.SystemColors.ControlLight;
            this.tpEmployer2.Controls.Add(this.grpEmployer2);
            this.tpEmployer2.Location = new System.Drawing.Point(0, 25);
            this.tpEmployer2.Name = "tpEmployer2";
            this.tpEmployer2.Size = new System.Drawing.Size(760, 292);
            this.tpEmployer2.TabIndex = 4;
            this.tpEmployer2.Title = "Employer Details";
            // 
            // grpEmployer2
            // 
            this.grpEmployer2.BackColor = System.Drawing.SystemColors.ControlLight;
            this.grpEmployer2.Controls.Add(this.lPhone);
            this.grpEmployer2.Controls.Add(this.txtDialCode2);
            this.grpEmployer2.Controls.Add(this.txtPhoneNo2);
            this.grpEmployer2.Controls.Add(this.txtPostCode);
            this.grpEmployer2.Controls.Add(this.txtAddress2);
            this.grpEmployer2.Controls.Add(this.txtAddress1);
            this.grpEmployer2.Controls.Add(this.txtEmpDept2);
            this.grpEmployer2.Controls.Add(this.txtEmpName2);
            this.grpEmployer2.Controls.Add(this.lStaffNo2);
            this.grpEmployer2.Controls.Add(this.lEmpDept2);
            this.grpEmployer2.Controls.Add(this.txtStaffNo2);
            this.grpEmployer2.Controls.Add(this.lEmpName2);
            this.grpEmployer2.Controls.Add(this.lEmpAddress2);
            this.grpEmployer2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.grpEmployer2.Location = new System.Drawing.Point(140, 30);
            this.grpEmployer2.Name = "grpEmployer2";
            this.grpEmployer2.Size = new System.Drawing.Size(480, 232);
            this.grpEmployer2.TabIndex = 12;
            this.grpEmployer2.TabStop = false;
            this.grpEmployer2.Text = "Employer";
            // 
            // lPhone
            // 
            this.lPhone.BackColor = System.Drawing.SystemColors.ControlLight;
            this.lPhone.CausesValidation = false;
            this.lPhone.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lPhone.Location = new System.Drawing.Point(106, 145);
            this.lPhone.Name = "lPhone";
            this.lPhone.Size = new System.Drawing.Size(56, 16);
            this.lPhone.TabIndex = 26;
            this.lPhone.Text = "Phone";
            this.lPhone.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtDialCode2
            // 
            this.txtDialCode2.BackColor = System.Drawing.SystemColors.Window;
            this.txtDialCode2.Location = new System.Drawing.Point(168, 144);
            this.txtDialCode2.MaxLength = 8;
            this.txtDialCode2.Name = "txtDialCode2";
            this.txtDialCode2.Size = new System.Drawing.Size(40, 20);
            this.txtDialCode2.TabIndex = 5;
            this.txtDialCode2.Tag = "";
            // 
            // txtPhoneNo2
            // 
            this.txtPhoneNo2.BackColor = System.Drawing.SystemColors.Window;
            this.txtPhoneNo2.Location = new System.Drawing.Point(216, 144);
            this.txtPhoneNo2.MaxLength = 13;
            this.txtPhoneNo2.Name = "txtPhoneNo2";
            this.txtPhoneNo2.Size = new System.Drawing.Size(74, 20);
            this.txtPhoneNo2.TabIndex = 6;
            this.txtPhoneNo2.Tag = "lPhone";
            // 
            // txtPostCode
            // 
            this.txtPostCode.BackColor = System.Drawing.SystemColors.Window;
            this.txtPostCode.Location = new System.Drawing.Point(168, 112);
            this.txtPostCode.MaxLength = 26;
            this.txtPostCode.Name = "txtPostCode";
            this.txtPostCode.Size = new System.Drawing.Size(160, 20);
            this.txtPostCode.TabIndex = 4;
            // 
            // txtAddress2
            // 
            this.txtAddress2.BackColor = System.Drawing.SystemColors.Window;
            this.txtAddress2.Location = new System.Drawing.Point(168, 88);
            this.txtAddress2.MaxLength = 26;
            this.txtAddress2.Name = "txtAddress2";
            this.txtAddress2.Size = new System.Drawing.Size(160, 20);
            this.txtAddress2.TabIndex = 3;
            // 
            // txtAddress1
            // 
            this.txtAddress1.BackColor = System.Drawing.SystemColors.Window;
            this.txtAddress1.Location = new System.Drawing.Point(168, 64);
            this.txtAddress1.MaxLength = 26;
            this.txtAddress1.Name = "txtAddress1";
            this.txtAddress1.Size = new System.Drawing.Size(160, 20);
            this.txtAddress1.TabIndex = 2;
            this.txtAddress1.Tag = "lEmpAddress2";
            // 
            // txtEmpDept2
            // 
            this.txtEmpDept2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.txtEmpDept2.Location = new System.Drawing.Point(168, 168);
            this.txtEmpDept2.MaxLength = 26;
            this.txtEmpDept2.Name = "txtEmpDept2";
            this.txtEmpDept2.Size = new System.Drawing.Size(160, 20);
            this.txtEmpDept2.TabIndex = 7;
            this.txtEmpDept2.Tag = "lEmpDept2";
            // 
            // txtEmpName2
            // 
            this.txtEmpName2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.txtEmpName2.Location = new System.Drawing.Point(168, 24);
            this.txtEmpName2.MaxLength = 26;
            this.txtEmpName2.Name = "txtEmpName2";
            this.txtEmpName2.Size = new System.Drawing.Size(160, 20);
            this.txtEmpName2.TabIndex = 1;
            this.txtEmpName2.Tag = "lEmpName2";
            // 
            // lStaffNo2
            // 
            this.lStaffNo2.BackColor = System.Drawing.SystemColors.ControlLight;
            this.lStaffNo2.CausesValidation = false;
            this.lStaffNo2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lStaffNo2.Location = new System.Drawing.Point(50, 193);
            this.lStaffNo2.Name = "lStaffNo2";
            this.lStaffNo2.Size = new System.Drawing.Size(112, 16);
            this.lStaffNo2.TabIndex = 19;
            this.lStaffNo2.Text = "Staff/works number";
            this.lStaffNo2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lEmpDept2
            // 
            this.lEmpDept2.BackColor = System.Drawing.SystemColors.ControlLight;
            this.lEmpDept2.CausesValidation = false;
            this.lEmpDept2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lEmpDept2.Location = new System.Drawing.Point(82, 169);
            this.lEmpDept2.Name = "lEmpDept2";
            this.lEmpDept2.Size = new System.Drawing.Size(80, 16);
            this.lEmpDept2.TabIndex = 18;
            this.lEmpDept2.Text = "Department";
            this.lEmpDept2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtStaffNo2
            // 
            this.txtStaffNo2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.txtStaffNo2.Location = new System.Drawing.Point(168, 192);
            this.txtStaffNo2.MaxLength = 20;
            this.txtStaffNo2.Name = "txtStaffNo2";
            this.txtStaffNo2.Size = new System.Drawing.Size(140, 20);
            this.txtStaffNo2.TabIndex = 8;
            this.txtStaffNo2.Tag = "lStaffNo2";
            // 
            // lEmpName2
            // 
            this.lEmpName2.BackColor = System.Drawing.SystemColors.ControlLight;
            this.lEmpName2.CausesValidation = false;
            this.lEmpName2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lEmpName2.Location = new System.Drawing.Point(106, 25);
            this.lEmpName2.Name = "lEmpName2";
            this.lEmpName2.Size = new System.Drawing.Size(56, 16);
            this.lEmpName2.TabIndex = 13;
            this.lEmpName2.Text = "Name";
            this.lEmpName2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lEmpAddress2
            // 
            this.lEmpAddress2.BackColor = System.Drawing.SystemColors.ControlLight;
            this.lEmpAddress2.CausesValidation = false;
            this.lEmpAddress2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lEmpAddress2.Location = new System.Drawing.Point(106, 64);
            this.lEmpAddress2.Name = "lEmpAddress2";
            this.lEmpAddress2.Size = new System.Drawing.Size(56, 16);
            this.lEmpAddress2.TabIndex = 12;
            this.lEmpAddress2.Text = "Address";
            this.lEmpAddress2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // drpApplicationType
            // 
            this.drpApplicationType.ItemHeight = 13;
            this.drpApplicationType.Location = new System.Drawing.Point(336, 17);
            this.drpApplicationType.Name = "drpApplicationType";
            this.drpApplicationType.Size = new System.Drawing.Size(104, 21);
            this.drpApplicationType.TabIndex = 6;
            this.drpApplicationType.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // drpIDSelection
            // 
            this.drpIDSelection.ItemHeight = 13;
            this.drpIDSelection.Location = new System.Drawing.Point(568, 17);
            this.drpIDSelection.Name = "drpIDSelection";
            this.drpIDSelection.Size = new System.Drawing.Size(118, 21);
            this.drpIDSelection.TabIndex = 10;
            this.drpIDSelection.Visible = false;
            this.drpIDSelection.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(490, 19);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(72, 16);
            this.label8.TabIndex = 11;
            this.label8.Text = "ID Selection";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label8.Visible = false;
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(234, 19);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(96, 16);
            this.label10.TabIndex = 7;
            this.label10.Text = "Application Type";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // gpCustomer
            // 
            this.gpCustomer.BackColor = System.Drawing.SystemColors.Control;
            this.gpCustomer.Controls.Add(this.txtCustomerID);
            this.gpCustomer.Controls.Add(this.dtDateProp);
            this.gpCustomer.Controls.Add(this.label4);
            this.gpCustomer.Controls.Add(this.label3);
            this.gpCustomer.Controls.Add(this.label2);
            this.gpCustomer.Controls.Add(this.lCustomerID);
            this.gpCustomer.Controls.Add(this.txtLastName);
            this.gpCustomer.Controls.Add(this.txtFirstName);
            this.gpCustomer.Location = new System.Drawing.Point(8, 0);
            this.gpCustomer.Name = "gpCustomer";
            this.gpCustomer.Size = new System.Drawing.Size(776, 64);
            this.gpCustomer.TabIndex = 2;
            this.gpCustomer.TabStop = false;
            // 
            // txtCustomerID
            // 
            this.txtCustomerID.Location = new System.Drawing.Point(16, 32);
            this.txtCustomerID.MaxLength = 20;
            this.txtCustomerID.Name = "txtCustomerID";
            this.txtCustomerID.Size = new System.Drawing.Size(112, 20);
            this.txtCustomerID.TabIndex = 24;
            this.txtCustomerID.Tag = "lCustomerID";
            // 
            // dtDateProp
            // 
            this.dtDateProp.CustomFormat = "ddd dd MMM yyyy";
            this.dtDateProp.Enabled = false;
            this.dtDateProp.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtDateProp.Location = new System.Drawing.Point(638, 32);
            this.dtDateProp.Name = "dtDateProp";
            this.dtDateProp.Size = new System.Drawing.Size(112, 20);
            this.dtDateProp.TabIndex = 23;
            this.dtDateProp.Tag = "";
            this.dtDateProp.Value = new System.DateTime(2002, 5, 21, 0, 0, 0, 0);
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(635, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(104, 16);
            this.label4.TabIndex = 8;
            this.label4.Text = "Date of Application:";
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(354, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 16);
            this.label3.TabIndex = 6;
            this.label3.Text = "Last Name:";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(159, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 16);
            this.label2.TabIndex = 5;
            this.label2.Text = "First Name:";
            // 
            // lCustomerID
            // 
            this.lCustomerID.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lCustomerID.Location = new System.Drawing.Point(13, 16);
            this.lCustomerID.Name = "lCustomerID";
            this.lCustomerID.Size = new System.Drawing.Size(72, 16);
            this.lCustomerID.TabIndex = 4;
            this.lCustomerID.Text = "Customer:";
            // 
            // txtLastName
            // 
            this.txtLastName.Location = new System.Drawing.Point(356, 32);
            this.txtLastName.MaxLength = 60;
            this.txtLastName.Name = "txtLastName";
            this.txtLastName.Size = new System.Drawing.Size(248, 20);
            this.txtLastName.TabIndex = 2;
            // 
            // txtFirstName
            // 
            this.txtFirstName.Location = new System.Drawing.Point(162, 32);
            this.txtFirstName.MaxLength = 30;
            this.txtFirstName.Name = "txtFirstName";
            this.txtFirstName.Size = new System.Drawing.Size(160, 20);
            this.txtFirstName.TabIndex = 1;
            // 
            // errorProvider1
            // 
            this.errorProvider1.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider1.ContainerControl = this;
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
            this.menuExit.ImageIndex = 0;
            this.menuExit.ImageList = this.menuIcons;
            this.menuExit.Text = "E&xit";
            this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
            // 
            // menuSanction
            // 
            this.menuSanction.Description = "MenuItem";
            this.menuSanction.ImageIndex = 0;
            this.menuSanction.ImageList = this.menuIcons;
            this.menuSanction.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuSave,
            this.menuComplete,
            this.menuReopen});
            this.menuSanction.Text = "&Sanction";
            // 
            // menuSave
            // 
            this.menuSave.Description = "MenuItem";
            this.menuSave.Text = "&Save";
            this.menuSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // menuComplete
            // 
            this.menuComplete.Description = "MenuItem";
            this.menuComplete.Text = "&Complete";
            this.menuComplete.Click += new System.EventHandler(this.btnComplete_Click);
            // 
            // menuReopen
            // 
            this.menuReopen.Description = "MenuItem";
            this.menuReopen.Enabled = false;
            this.menuReopen.Text = "&Re-open Stage 2";
            this.menuReopen.Visible = false;
            this.menuReopen.Click += new System.EventHandler(this.menuReopen_Click);
            // 
            // SanctionStage2
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.gpCustomer);
            this.Controls.Add(this.gbData);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SanctionStage2";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Credit Sanction - Stage 2";
            this.Load += new System.EventHandler(this.SanctionStage2_Load);
            this.Enter += new System.EventHandler(this.SanctionStage2_Enter);
            this.Leave += new System.EventHandler(this.SanctionStage2_Leave);
            this.gbData.ResumeLayout(false);
            this.tcApplicants.ResumeLayout(false);
            this.tpApp1.ResumeLayout(false);
            this.tcApp1.ResumeLayout(false);
            this.tpPreviousAddress.ResumeLayout(false);
            this.tpPreviousAddress.PerformLayout();
            this.grpPrevAddress.ResumeLayout(false);
            this.tpEmployer.ResumeLayout(false);
            this.grpEmployer.ResumeLayout(false);
            this.grpEmployer.PerformLayout();
            this.tpReferences.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.tpComments.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.tpApp2.ResumeLayout(false);
            this.tcApp2.ResumeLayout(false);
            this.tpEmployer2.ResumeLayout(false);
            this.grpEmployer2.ResumeLayout(false);
            this.grpEmployer2.PerformLayout();
            this.gpCustomer.ResumeLayout(false);
            this.gpCustomer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        //
        // Local procedures
        //
        private void HashMenus()
        {
            dynamicMenus = new Hashtable();
            dynamicMenus[this.Name + ":menuReopen"] = this.menuReopen;
            dynamicMenus[this.Name + ":btnRemove"] = this.btnRemove;
        }

        private void LoadData()
        {
            Thread data = new Thread(new ThreadStart(LoadDataThread));
            data.Start();
            data.Join();
        }

        private void LoadDataThread()
        {
            bool status = true;
            try
            {
                Wait();
                Function = "LoadDataThread";

                //Lock account
                LockAccount();

                //Get data for set up frame
                _formFieldDef = StaticDataManager.GetMandatoryFields(Config.CountryCode, this.Name, out _error);
                if (_error.Length > 0)
                {
                    status = false;
                    ShowError(_error);
                }

                if (status)
                {
                    string refFormName = this.Name + "Ref";
                    _referenceFieldDef = StaticDataManager.GetMandatoryFields(Config.CountryCode, refFormName, out _error);
                    if (_error.Length > 0)
                    {
                        status = false;
                        ShowError(_error);
                    }
                }

                //Load the static data for the drop downs
                if (status)
                {

                    XmlUtilities xml = new XmlUtilities();
                    XmlDocument dropDowns = new XmlDocument();
                    dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

                    if (StaticData.Tables[TN.IDSelection] == null)
                        dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.IDSelection, new string[] { "IT1", "L" }));
                    if (StaticData.Tables[TN.ApplicationType] == null)
                        dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.ApplicationType, new string[] { this.CustomerID, this.AccountNo }));
                    if (StaticData.Tables[TN.Villages] == null)
                        dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.Villages, null));

                    if (dropDowns.DocumentElement.ChildNodes.Count > 0)
                    {
                        _dropDownSet = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out _error);
                        if (_error.Length > 0)
                        {
                            status = false;
                            ShowError(_error);
                        }
                    }
                }

                //load the application data for stage 2
                if (status)
                {
                    _stage2DataA1 = CreditManager.GetProposalStage2(CustomerID, _dateProp, AccountNo, Holder.Main, out _error);
                    if (_error.Length > 0)
                    {
                        status = false;
                        ShowError(_error);
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
                Function = "End of LoadDataThread";
            }
        }

        private bool LockAccount()
        {
            if (this.AccountNo.Length != 0)
            {
                AccountLocked = AccountManager.LockAccount(this.AccountNo, Credential.UserId.ToString(), out _error);
                if (_error.Length > 0)
                {
                    ShowError(_error);
                    _readOnly = !AccountLocked;
                    SetFieldBias(this.Name, _formFieldDef.Tables["Fields"], _readOnly);
                    int refNo = 0;
                    foreach (Crownwood.Magic.Controls.TabPage tp in tcReferrences.TabPages)		//one or many rows
                    {
                        refNo = refNo + 1;
                        SetFieldBias("Reference " + refNo.ToString(), _referenceFieldDef.Tables["Fields"], _readOnly);
                    }
                }
            }
            else
                AccountLocked = true;

            return AccountLocked;
        }

        /// <summary>
        /// This method will load all the required static data for drop down 
        /// fields etc, required by the screen
        /// </summary>
        private void LoadStatic()
        {
            Function = "SanctionStage2::LoadStatic()";
            drpSpecialPromo.DataSource = new string[] { String.Empty, "No", "Yes" };

            if (_dropDownSet != null)
            {
                foreach (DataTable dt in _dropDownSet.Tables)
                {
                    switch (dt.TableName)
                    {
                        case TN.ApplicationType:
                            DataRow r = dt.NewRow();
                            r[CN.Code] = "H";
                            r[CN.CodeDescription] = "Sole";
                            dt.Rows.InsertAt(r, 0);
                            drpApplicationType.DataSource = dt;
                            drpApplicationType.DisplayMember = CN.CodeDescription;
                            break;
                        default:
                            StaticData.Tables[dt.TableName] = dt;
                            break;
                    }
                }
            }

            drpIDSelection.DataSource = (DataTable)StaticData.Tables[TN.IDSelection];
            drpIDSelection.DisplayMember = CN.CodeDescription;

        }

        /// <summary>
        /// This function will load any details we have in the proposal table for
        /// this customer for Stage 2 and populate the screen fields
        /// Also gets information from the customer and custaddress tables
        /// </summary>
        private void LoadStage2Details()
        {
            Function = "SanctionStage2::LoadStage2Details()";
            bool PrevAddressBlank = true;
            txtCustomerID.Text = this.CustomerID;
            string rel = String.Empty;


            if (tcApplicants.TabPages.Contains(tpApp2))
            {
                tcApplicants.TabPages.Remove(tpApp2);
            }

            if (_stage2DataA1 != null)
            {
                foreach (DataTable dt in _stage2DataA1.Tables)
                {
                    if (dt.TableName == TN.Customer)
                    {

                        drpIDSelection.SelectedIndex = 0;
                        foreach (DataRow row in dt.Rows)		//should only be one row
                        {
                            ResidentialStatus = (string)row[CN.ResidentialStatus];

                            txtFirstName.Text = (string)row[CN.FirstName];
                            txtLastName.Text = (string)row[CN.LastName];

                            if ((DateTime)row[CN.DateIn] > DateTime.MinValue)
                            {
                                _yearsInCurrentAddress = DateTime.Today.Year - ((DateTime)row[CN.DateIn]).Year;
                                if (DateTime.Today.Month < ((DateTime)row[CN.DateIn]).Month)
                                {
                                    --_yearsInCurrentAddress;
                                }
                            }

                            if ((DateTime)row[CN.PrevDateIn] > DateTime.MinValue)
                            {
                                // DSR 24 Jan 2003 - Populate Previous Address from
                                // current data instead of from Proposal
                                PrevAddressBlank = false;
                                tcPrevAddress.txtAddress1.Text = (string)row[CN.PAddress1];

                                if (tcPrevAddress.cmbVillage.Items.Count > 0 &&
                                row[CN.PAddress2] != DBNull.Value) // Address Standardization CR2019 - 025
                                {
                                    var villageIndex = tcPrevAddress.cmbVillage.FindStringExact((string)row[CN.PAddress2]);
                                    if (villageIndex != -1)
                                        tcPrevAddress.cmbVillage.SelectedIndex = villageIndex;
                                    else
                                        tcPrevAddress.cmbVillage.SelectedText = (string)row[CN.PAddress2];
                                }
                                else if (row[CN.PAddress2] != DBNull.Value)
                                    tcPrevAddress.cmbVillage.SelectedText = (string)row[CN.PAddress2];
                                if (tcPrevAddress.cmbRegion.Items.Count > 0 &&
                                    row[CN.PAddress3] != DBNull.Value) // Address Standardization CR2019 - 025
                                {
                                    var regionIndex = tcPrevAddress.cmbRegion.FindStringExact((string)row[CN.PAddress3]);
                                    if (regionIndex != -1)
                                        tcPrevAddress.cmbRegion.SelectedIndex = regionIndex;
                                    else
                                        tcPrevAddress.cmbRegion.SelectedText = (string)row[CN.PAddress3];
                                }
                                else if (row[CN.PAddress3] != DBNull.Value) // Address Standardization CR2019 - 025
                                    tcPrevAddress.cmbRegion.SelectedText = (string)row[CN.PAddress3];
                                tcPrevAddress.txtPostCode.Text = ((string)row[CN.PPostCode]).Trim();
                                if (!Convert.IsDBNull(row["Latitude"]) && !Convert.IsDBNull(row["Longitude"])) // Address Standardization CR2019 - 025
                                    tcPrevAddress.txtCoordinate.Text = string.Format("{0},{1}", row["Latitude"].ToString(), row["Longitude"].ToString());
                            }
                            else
                            {
                                // DSR 24 Jan 2003 - Previous Address can be loaded from Proposal.
                                PrevAddressBlank = true;
                            }

                            foreach (DataRowView r in drpIDSelection.Items)
                            {
                                if ((string)r[CN.Code] == ((string)row[CN.IDType]).Trim())
                                {
                                    drpIDSelection.SelectedItem = r;
                                    break;
                                }
                            }
                        }
                    }

                    if (dt.TableName == TN.Proposal)
                    {
                        foreach (DataRow row in dt.Rows)		//should only be one row
                        {
                            foreach (DataRowView r in drpApplicationType.Items)
                            {
                                if ((string)r[CN.Code] == ((string)row[CN.A2Relation]).Trim())
                                {
                                    drpApplicationType.SelectedItem = r;
                                    break;
                                }
                            }

                            switch ((string)row[CN.SpecialPromo])
                            {
                                case "N": drpSpecialPromo.SelectedIndex = 1;
                                    break;
                                case "Y": drpSpecialPromo.SelectedIndex = 2;
                                    break;
                                default: drpSpecialPromo.SelectedIndex = 0;
                                    break;
                            }

                            if (PrevAddressBlank == true)
                            {
                                tcPrevAddress.txtAddress1.Text = (string)row[CN.PAddress1];
                                if (tcPrevAddress.cmbVillage.Items.Count > 0 &&
                                row[CN.PAddress2] != DBNull.Value) // Address Standardization CR2019 - 025
                                {
                                    var villageIndex = tcPrevAddress.cmbVillage.FindStringExact((string)row[CN.PAddress2]);
                                    if (villageIndex != -1)
                                        tcPrevAddress.cmbVillage.SelectedIndex = villageIndex;
                                    else
                                        tcPrevAddress.cmbVillage.SelectedText = (string)row[CN.PAddress2];
                                }
                                else if (row[CN.PAddress2] != DBNull.Value)
                                    tcPrevAddress.cmbVillage.SelectedText = (string)row[CN.PAddress2];
                                if (tcPrevAddress.cmbRegion.Items.Count > 0 &&
                                    row[CN.PCity] != DBNull.Value) // Address Standardization CR2019 - 025
                                {
                                    var regionIndex = tcPrevAddress.cmbRegion.FindStringExact((string)row[CN.PCity]);
                                    if (regionIndex != -1)
                                        tcPrevAddress.cmbRegion.SelectedIndex = regionIndex;
                                    else
                                        tcPrevAddress.cmbRegion.SelectedText = (string)row[CN.PCity];
                                }
                                else if (row[CN.PCity] != DBNull.Value) // Address Standardization CR2019 - 025
                                    tcPrevAddress.cmbRegion.SelectedText = (string)row[CN.PCity];
                                
                                tcPrevAddress.txtPostCode.Text = ((string)row[CN.PPostCode]).Trim();
                            }
                            txtEmpName.Text = (string)row[CN.EmployeeName];
                            txtEmpDept.Text = (string)row[CN.EmpDept];
                            tcEmpAddress.txtAddress1.Text = (string)row[CN.EAddress1];

                            if (tcEmpAddress.cmbVillage.Items.Count > 0 &&
                                row[CN.EAddress2] != DBNull.Value) // Address Standardization CR2019 - 025
                            {
                                var villageIndex = tcEmpAddress.cmbVillage.FindStringExact((string)row[CN.EAddress2]);
                                if (villageIndex != -1)
                                    tcEmpAddress.cmbVillage.SelectedIndex = villageIndex;
                                else
                                    tcEmpAddress.cmbVillage.SelectedText = (string)row[CN.EAddress2];
                            }
                            else if (row[CN.EAddress2] != DBNull.Value)
                                tcEmpAddress.cmbVillage.SelectedText = (string)row[CN.EAddress2];
                            if (tcEmpAddress.cmbRegion.Items.Count > 0 &&
                                row[CN.ECity] != DBNull.Value) // Address Standardization CR2019 - 025
                            {
                                var regionIndex = tcEmpAddress.cmbRegion.FindStringExact((string)row[CN.ECity]);
                                if (regionIndex != -1)
                                    tcEmpAddress.cmbRegion.SelectedIndex = regionIndex;
                                else
                                    tcEmpAddress.cmbRegion.SelectedText = (string)row[CN.ECity];
                            }
                            else if (row[CN.ECity] != DBNull.Value) // Address Standardization CR2019 - 025
                                tcEmpAddress.cmbRegion.SelectedText = (string)row[CN.ECity];
                            
                            tcEmpAddress.txtPostCode.Text = ((string)row[CN.EPostCode]).Trim();
                            txtComment.Text = (string)row[CN.S1Comment];
                            txtRegistration.Text = (string)row[CN.VehicleRegistration];

                            rel = (string)row[CN.A2Relation];
                        }
                    }

                    if (dt.TableName == TN.Employment)
                    {
                        foreach (DataRow row in dt.Rows)		//should only be one row
                        {
                            txtStaffNo.Text = (string)row[CN.StaffNo];
                            _empStatus = ((string)row[CN.EmploymentStatus]).Trim();
                        }
                    }

                    if (dt.TableName == TN.References)
                    {
                        foreach (DataRow row in dt.Rows)		//one or many rows
                        {
                            this.PopulateReference(row);
                        }

                        while (tcReferrences.TabPages.Count < (decimal)Country[CountryParameterNames.MinReferences] &&
                               tcReferrences.TabPages.Count < SanctionStage2._maxReferences)
                        {
                            this.CreateReferenceTab();
                        }

                        this.btnPreviousRefs.Enabled = (tcReferrences.TabPages.Count < SanctionStage2._maxReferences);
                    }
                }

                if (_empStatus == SanctionStage2._unemployed)
                {
                    if (tcApp1.TabPages.Contains(tpEmployer))
                        tcApp1.TabPages.Remove(tpEmployer);
                }
                else
                {
                    if (!tcApp1.TabPages.Contains(tpEmployer))
                        tcApp1.TabPages.Add(tpEmployer);
                }

                CheckLandlordReference();
            }

            string relation = ((DataRowView)drpApplicationType.SelectedItem)[CN.Code].ToString();
            if (rel.Length != 0)
            {
                LoadApplicantTwoDetails(relation);
            }

            // Previous address is not displayed when in current address >= min years
            if (_yearsInCurrentAddress >= (decimal)Country[CountryParameterNames.SanctionMinYears])
            {
                this.grpPrevAddress.Enabled = false;
                this.grpPrevAddress.Visible = false;
            }
            else
            {
                this.grpPrevAddress.Enabled = true;
                this.grpPrevAddress.Visible = true;
            }
        }	// End of LoadStage2Details

        private void CheckLandlordReference()
        {
            bool landlordFound = false;

            if (ResidentialStatus == "R" &&
                (bool)Country[CountryParameterNames.CaptureLandlordDetails])
            {
                /* is there already a landlord reference */
                foreach (Crownwood.Magic.Controls.TabPage tp in tcReferrences.TabPages)
                {
                    ReferenceTab rt = (ReferenceTab)tp.Controls[0];
                    if ((string)((DataRowView)rt.drpRelation.SelectedItem)[CN.Code] == "L")
                        landlordFound = true;
                }

                if (!landlordFound)
                {
                    /* set the first blank tab to be a landlord tab */
                    foreach (Crownwood.Magic.Controls.TabPage tp in tcReferrences.TabPages)
                    {
                        ReferenceTab rt = (ReferenceTab)tp.Controls[0];
                        if (rt.drpRelation.SelectedIndex == 0)
                        {
                            foreach (DataRowView r in ((DataTable)rt.drpRelation.DataSource).DefaultView)
                            {
                                if ((string)r[CN.Code] == "L")
                                {
                                    rt.drpRelation.SelectedItem = r;
                                    landlordFound = true;
                                    break;
                                }
                            }
                            if (landlordFound)
                                break;
                        }
                    }

                    if (!landlordFound)
                    {
                        /* do we have any spare slots to create a landlord reference? */
                        if (tcReferrences.TabPages.Count < SanctionStage2._maxReferences)
                        {
                            ReferenceTab rt = CreateReferenceTab();
                            foreach (DataRowView r in ((DataTable)rt.drpRelation.DataSource).DefaultView)
                            {
                                if ((string)r[CN.Code] == "L")
                                {
                                    rt.drpRelation.SelectedItem = r;
                                    break;
                                }
                            }
                        }
                        else	/* need to change an existing reference */
                        {
                            ShowInfo("M_LANDLORDREQUIRED");
                        }
                    }
                }
            }
        }

        private void PopulateReference(DataRow row)
        {
            ReferenceTab rt = this.CreateReferenceTab();

            rt.txtFirstName.Text = (string)row[CN.RefFirstName];
            rt.txtLastName.Text = (string)row[CN.RefLastName];
            foreach (DataRowView r in rt.drpRelation.Items)
            {
                if ((string)r[CN.Code] == ((string)row[CN.RefRelation]).Trim())
                {
                    rt.drpRelation.SelectedItem = r;
                    break;
                }
            }
            rt.txtYearsKnown.Text = Convert.ToInt32(row[CN.YrsKnown]).ToString();

            rt.txtAddress1.Text = (string)row[CN.RefAddress1];
            rt.txtAddress2.Text = (string)row[CN.RefAddress2];
            rt.txtAddress3.Text = (string)row[CN.RefCity];
            rt.txtPostCode.Text = ((string)row[CN.RefPostCode]).Trim();
            rt.txtWAddress1.Text = (string)row[CN.RefWAddress1];
            rt.txtWAddress2.Text = (string)row[CN.RefWAddress2];
            rt.txtWAddress3.Text = (string)row[CN.RefWCity];
            rt.txtWPostCode.Text = ((string)row[CN.RefWPostCode]).Trim();

            rt.txtDialCode.Text = (string)row[CN.RefDialCode];
            rt.txtPhoneNo.Text = (string)row[CN.RefPhoneNo];
            rt.txtWDialCode.Text = (string)row[CN.RefWDialCode];
            rt.txtWPhoneNo.Text = (string)row[CN.RefWPhoneNo];
            rt.txtMDialCode.Text = (string)row[CN.RefMDialCode];
            rt.txtMPhoneNo.Text = (string)row[CN.RefMPhoneNo];
            rt.txtDirections.Text = (string)row[CN.RefDirections];
            rt.txtComment.Text = (string)row[CN.RefComment];
            rt.txtNewComment.Text = "";

            rt.empeeNo = Convert.ToInt32(row[CN.EmpeeNoChange]);
            if (rt.empeeNo != 0)
                rt.txtCheckedBy.Text = rt.empeeNo.ToString() + " : " + Login.GetEmployeeName((rt.empeeNo), out _error);

            if (row[CN.DateChange] != DBNull.Value)
                rt.txtDateChecked.Text = Convert.ToDateTime(row[CN.DateChange]).ToString();
        }


        /// <summary>
        /// Calls SetupForm() passing in the root control on this tab
        /// and the list of mandatory field definitions in _referenceFieldDef.
        /// The rest of the procedures are used as for the form. The only difference is
        /// that the root control is now the tab instead of the form, and the list
        /// of mandatory field definitions is _referenceFieldDef instead of _formFieldDef.
        /// When another reference tab is added it is distinguised from the others by
        /// suffixing the name of the root control with a number. The list of mandatory
        /// field definitions in _referenceFieldDef is re-used (because all the tabs are
        /// the same).
        /// </summary>
        private ReferenceTab CreateReferenceTab()
        {
            Crownwood.Magic.Controls.TabPage tp = new Crownwood.Magic.Controls.TabPage("Reference " + (tcReferrences.TabPages.Count + 1).ToString());
            ReferenceTab rt = new ReferenceTab(tp);
            rt.Dock = DockStyle.Fill;
            rt.drpRelation.SelectedIndex = 0;

            foreach (Control c in rt.Controls)
            {
                c.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
                foreach (Control x in c.Controls)
                    x.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            }

            tp.BorderStyle = BorderStyle.Fixed3D;
            tp.Controls.Add(rt);
            tcReferrences.TabPages.Add(tp);

            // Set up hash table for this tab
            tp.Name = "Reference " + (tcReferrences.TabPages.Count).ToString();
            SetupForm(tp.Name, rt.Controls, _referenceFieldDef.Tables["Fields"]);

            rt.ChxReference.Enabled = rt.lCheckAllowed.Enabled;
            rt.ChxReference.Visible = rt.lCheckAllowed.Enabled;

            ValidateControl(null, null);

            return rt;
        }

        private void LoadApplicantTwoDetails(string relation)
        {
            try
            {
                Wait();
                Function = "SanctionStage2::LoadApplicantTwoDetails()";

                WCreditManager credit = new WCreditManager(true);
                if (tcApplicants.TabPages.Contains(tpApp2))
                    tcApplicants.TabPages.Remove(tpApp2);
                tcApplicants.TabPages.Add(tpApp2);

                _stage2DataA2 = credit.GetProposalStage2(this.CustomerID, this.dtDateProp.Value, this.AccountNo, relation, out _error);
                if (_error.Length > 0)
                    ShowError(_error);
                else
                {
                    foreach (DataTable dt in _stage2DataA2.Tables)
                    {
                        if (dt.TableName == TN.Customer)
                        {
                            foreach (DataRow row in dt.Rows)		//should only be one row
                            {
                                txtEmpName2.Text = (string)row[CN.Address1];
                                txtAddress1.Text = (string)row[CN.Address2];
                                txtAddress2.Text = (string)row[CN.Address3];
                                txtPostCode.Text = ((string)row[CN.PostCode]).Trim();
                            }
                        }

                        if (dt.TableName == TN.Employment)
                        {
                            foreach (DataRow row in dt.Rows)		//should only be one row
                            {
                                txtStaffNo2.Text = (string)row[CN.StaffNo];
                                txtDialCode2.Text = (string)row[CN.PersDialCode];
                                txtPhoneNo2.Text = (string)row[CN.PersTel];
                                txtEmpDept2.Text = (string)row[CN.Department];
                            }
                        }
                    }
                }
                credit.Dispose();
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


        /// <summary>
        /// Creates hash tables and calls SetupForm() passing in the root control on
        /// this form and the list of mandatory field definitions in _formFieldDef.
        /// </summary>
        private void SetupFrame()
        {
            _formFields = new Hashtable();
            _mandatoryFields = new Hashtable();
            _visibleFields = new Hashtable();
            _inError = new Hashtable();

            // Set up the form fields according to stored properties
            if (_formFieldDef != null)
            {
                SetupForm(this.Name, this.Controls, _formFieldDef.Tables["Fields"]);
            }
        }

        /// <summary>
        /// Calls SetupHashTable() and SetFieldProperties().
        /// </summary>
        private void SetupForm(
            string formName,
            System.Windows.Forms.Control.ControlCollection formControls,
            DataTable formDef)
        {
            // Set up the hash table for the form controls
            _controlTree = String.Empty;
            SetupHashTable(formName, formControls);

#if (LOGNAMES)
			// Log the full path names on this form
			logMessage("Form fields found for SanctionStage2:\n\n" + _controlTree, "LOGNAMES", EventLogEntryType.Information);
			// Log the fields that have been recognised and stored in the hash table
			IDictionaryEnumerator FieldEnumerator = _formFields.GetEnumerator();
			_controlTree = "";
			while (FieldEnumerator.MoveNext())
				_controlTree = _controlTree + "\t  KEY: " + FieldEnumerator.Key + "\n\tVALUE: " + FieldEnumerator.Value + "\n\n";
				
			logMessage("Form fields recognised for SanctionStage2:\n\n" + _controlTree, "LOGNAMES", EventLogEntryType.Information);
			_controlTree = "";
#endif

            // Set the properties on each form control
            SetFieldProperties(formName, formDef);
        }

        /// <summary>
        /// Starts with the root control and recursively traverses all the controls on
        /// the form and adds the full name for each control to the _formFields hash table.
        /// </summary>
        private void SetupHashTable(
            string curName,
            System.Windows.Forms.Control.ControlCollection curControls)
        {
            string fullName;

            foreach (Control fieldControl in curControls)
            {
                // Construct the full path name to this control
                //JJ - due to a peculiarity in the MagicLibrary v1.7.1 tabControl some controls 
                //may appear which have no name, but contain child controls. This was causing 
                //the FullName to be constructed incorrectly in some cases. The following line 
                //fixes this problem.
                fullName = fieldControl.Name.Length == 0 ? curName : curName + "." + fieldControl.Name;

                // Ignore blank names
                if (fieldControl.Name.Length > 0)
                {
                    // Add control to hash table.
                    // NOTE: This control could already be in the hash table if a
                    // reference tab has been removed and then added again.
                    // In this case the old control must be overwritten.
                    _formFields[fullName] = fieldControl;
#if (LOGNAMES)
						_controlTree = _controlTree + "\t" + fullName + "\tTYPE: " + "\n";
#endif
                }

                // Process any nested controls
                SetupHashTable(fullName, fieldControl.Controls);
            }

        }

        /// <summary>
        /// Uses the mandatory field definitions in _formFieldDef to set the Visible
        /// and Enabled property on each field, directly addressing each field by
        /// full name via the _formFields hash table. If a field is also mandatory,
        /// then it is added to the _mandatoryFields hash table and highlighted.
        /// All visible fields are added to the _visibleFields hash table.
        /// Calls SetFieldBias().
        /// </summary>
        private void SetFieldProperties(string formName, DataTable formDef)
        {
            string fieldName;
            string tagName;
            string fullTagName;
            string namePrefix = String.Empty;

            if (formName.Length > 0) namePrefix = formName + '.';

            foreach (DataRow fieldDef in formDef.Rows)
            {
                fieldName = namePrefix + (string)fieldDef["control"];
                ((Control)_formFields[fieldName]).Visible = Convert.ToBoolean(fieldDef["visible"]);
                ((Control)_formFields[fieldName]).Enabled = Convert.ToBoolean(fieldDef["enabled"]);

                if (Convert.ToBoolean(fieldDef["visible"])
                    && Convert.ToBoolean(fieldDef["enabled"])
                    && Convert.ToBoolean(fieldDef["mandatory"]))
                {
                    // uat366 rdb 11/04/08 btnEnter (reference Tab) cannot be added to mandatory fields as there is no was of setting it as complete, instead this is handled by minimum referenc enumber
                    if (!fieldName.Contains("gbData.tcApplicants.tpApp1.tcApp1.tpReferences.btnEnter"))
                    {
                        // Only validate mandatory fields that are visible and enabled
                        _mandatoryFields[fieldName] = ((Control)_formFields[fieldName]);
                        HighliteControl(((Control)_formFields[fieldName]));
                    }
                }
                if (Convert.ToBoolean(fieldDef["visible"]))
                    _visibleFields[fieldName] = _formFields[fieldName];

                // Set up a tag related control
                tagName = (string)((Control)_formFields[fieldName]).Tag;
                if (tagName != null)
                {
                    if (tagName.Length > 0)
                    {
                        fullTagName = fieldName.Substring(0, fieldName.LastIndexOf(".") + 1) + tagName;
                        // 5.1 uat 248 rdb 7/12/07 reference for address tag fix
                        if (fullTagName == "SanctionStage2.gbData.tcApplicants.tpApp1.tcApp1.tpEmployer.grpEmployer.tcEmpAddress.lEmpAddress")
                        {
                            fullTagName = "SanctionStage2.gbData.tcApplicants.tpApp1.tcApp1.tpEmployer.grpEmployer.lEmpAddress";
                        }
                        ((Control)_formFields[fullTagName]).Visible = Convert.ToBoolean(fieldDef["visible"]);
                        ((Control)_formFields[fullTagName]).Enabled = Convert.ToBoolean(fieldDef["enabled"]);
                    }
                }

            }

            // Set as disabled / read only
            SetFieldBias(formName, formDef, _readOnly);
        }

        private void HighliteControl(Control c)
        {
            //Create a highlite box
            STL.PL.HighliteBox h = new STL.PL.HighliteBox();
            h.Border = 6;
            h.Alpha = 50;
            h.Color = SystemColors.Highlight;
            h.TabStop = false;
            h.TabIndex = 0;

            if (c.GetType().Name == "DatePicker")
                ((DatePicker)c).Highlite(h);
            else
            {
                c.Parent.Controls.Add(h);
                h.Location = c.Location;
                h.Size = c.Size;
            }
            //h.Dispose();
        }

        /// <summary>
        /// Sets the Enabled or ReadOnly property on each field depending on the type
        /// of field from the list of mandatory field definitions in _formFieldDef, or
        /// to ReadOnly if the whole form is ReadOnly.
        /// </summary>
        private void SetFieldBias(string formName, DataTable formDef, bool newReadOnly)
        {
            string fieldName;
            string namePrefix = String.Empty;

            if (formName.Length > 0) namePrefix = formName + '.';

            _readOnly = newReadOnly;
            menuSave.Enabled = !_readOnly;
            menuComplete.Enabled = btnComplete.Enabled = !_readOnly;

            foreach (DataRow fieldDef in formDef.Rows)
            {
                fieldName = namePrefix + (string)fieldDef["control"];

                // Set as disabled / read only
                switch (_formFields[fieldName].GetType().Name)
                {
                    case "TextBox":
                        ((TextBox)_formFields[fieldName]).ReadOnly = (!Convert.ToBoolean(fieldDef["enabled"]) || _readOnly);

                        ////CR1084 only show References
                        //if (!fieldName.Contains("Reference") && (_telephoneAction != null || _bailReview != null))  
                        //    ((TextBox)_formFields[fieldName]).Visible = false;

                        if (_readOnly)
                            ((TextBox)_formFields[fieldName]).BackColor = SystemColors.Control;
                        else
                            ((TextBox)_formFields[fieldName]).BackColor = SystemColors.Window;

                        break;
                    case "PhoneNumberBox":
                        //CR1084 only show References
                        if (!fieldName.Contains("Reference") && (_telephoneAction != null || _bailReview != null))
                            ((PhoneNumberBox)_formFields[fieldName]).Visible = false;
                        ((PhoneNumberBox)_formFields[fieldName]).ReadOnly = (!Convert.ToBoolean(fieldDef["enabled"]) || _readOnly);
                        if (_readOnly)
                            ((PhoneNumberBox)_formFields[fieldName]).BackColor = SystemColors.Control;
                        else
                            ((PhoneNumberBox)_formFields[fieldName]).BackColor = SystemColors.Window;

                        break;
                    case "DateTimePicker":
                    case "CheckBox":
                    case "ComboBox":
                    //if (!fieldName.Contains("Reference") && (_telephoneAction != null || _bailReview != null))
                    //    ((ComboBox)_formFields[fieldName]).Visible = false;
                    //break;
                    case "Button":
                        ((Control)_formFields[fieldName]).Enabled = (Convert.ToBoolean(fieldDef["enabled"]) && !_readOnly);
                        break;
                    //case "RichTextBox":
                    //    if (!fieldName.Contains("Reference") && (_telephoneAction != null || _bailReview != null))
                    //        ((RichTextBox)_formFields[fieldName]).Visible = false;
                    //    break;
                    default:
                        break;
                }
            }
            btnRemove.Enabled = !_readOnly;

        }


        /// <summary>
        /// Checks for blank mandatory fields in the list of mandatory field definitions
        /// in _formFieldDef.
        /// Also checks at least one reference tab has been added.
        /// </summary>
        private void ValidateFields(string formName, DataTable formDef, bool complete)
        {
            string fieldName;
            int i;
            string namePrefix = String.Empty;
            bool valid = true;

            if (formName.Length > 0) namePrefix = formName + '.';

            foreach (DataRow fieldDef in formDef.Rows)
            {
                fieldName = namePrefix + (string)fieldDef["control"];
                valid = true;

                // Validation for mandatory fields when 'complete'
                if ((Control)_mandatoryFields[fieldName] != null
                    && complete)
                {
                    /* A slightly different approach for referrences */
                    if (((Control)_formFields[fieldName]).Name == tcReferrences.Name)
                    {
                        if (((Crownwood.Magic.Controls.TabControl)_formFields[fieldName]).TabPages.Count == 0)
                            valid = false;
                        else
                            valid = true;
                    }
                    else
                    {
                        if (((Control)_formFields[fieldName]).Text.Length == 0)
                            valid = false;
                        else
                            valid = true;
                    }
                }
                // FA UAT 594 checks for numbers in two fields
                if (((Control)_formFields[fieldName]).Name == txtStaffNo.Name)
                {
                    //valid = int.TryParse(txtStaffNo.Text, out i);
                }
                else if ((((Control)_formFields[fieldName]).Name == txtStaffNo2.Name) && (_stage2DataA2 != null))
                {
                    //valid = int.TryParse(txtStaffNo2.Text, out i);
                }
                else if (fieldName.Contains("txtYearsKnown"))
                {
                    valid = int.TryParse(((Control)_formFields[fieldName]).Text, out i);
                }


                if (!valid)
                {
                    // FA UAT 594 Different error as these two fields are not mandatory
                    if ((((Control)_formFields[fieldName]).Name == txtStaffNo.Name) || ((((Control)_formFields[fieldName]).Name == txtStaffNo2.Name)
                        || fieldName.Contains("txtYearsKnown")))
                    //&& (_stage2DataA2 != null))) //IP - 26/11/09 - UAT5.2 (926) - Added fieldName.Contains("txtYearsKnown" and removed check on _stage2DataA2
                    {
                        errorProvider1.SetError(((Control)_formFields[fieldName]), GetResource("M_INVALIDFIELDS"));
                    }
                    else
                    {
                        errorProvider1.SetError(((Control)_formFields[fieldName]), GetResource("M_ENTERMANDATORY"));
                    }
                    //errorProvider1.SetError(((Control)_formFields[fieldName]), GetResource("M_MANDATORYFIELD", new object[]{fieldName}));
                    //ShowInfo("M_MANDATORYFIELD", new object[]{fieldName});
                    _inError.Remove(fieldName);		//in case it's already there
                    _inError.Add(fieldName, true);
                }
                else
                {
                    errorProvider1.SetError(((Control)_formFields[fieldName]), String.Empty);
                    _inError.Remove(fieldName);
                }

            }
        }

        //private bool DisplayValidationErrors()
        //{
        //    bool display = false;
        //    string fieldName;
        //    string fieldTitle;
        //    string tagName;
        //    string fullTagName;

        //    if (_inError.Keys.Count > 0)
        //    {
        //        display = true;
        //        string errors = "The following fields are in error: \n\n";
        //        foreach (object o in _inError.Keys)
        //        {
        //            fieldName = o.ToString();
        //            fieldTitle = ((Control)_formFields[fieldName]).Name;
        //            tagName = (string)((Control)_formFields[fieldName]).Tag;
        //            if (tagName != null)
        //            {
        //                if (tagName.Length > 0)
        //                {
        //                    fullTagName = fieldName.Substring(0,fieldName.LastIndexOf(".")+1) + tagName;
        //                    fieldTitle = ((Control)_formFields[fullTagName]).Text;
        //                }
        //            }

        //            errors += fieldTitle + "\n";
        //        }
        //        errors += "\nPlease correct and try again.";
        //        ShowError(errors);			
        //    }
        //    _inError.Clear();
        //    return display;
        //}

        public void ValidateControl(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (!_readOnly)     //CR1084 only validate if not View mode (causes error when screen called from TelephoneAction)
                {
                    int refNo = 0;

                    ValidateFields(this.Name, _formFieldDef.Tables["Fields"], true);
                    foreach (Crownwood.Magic.Controls.TabPage tp in tcReferrences.TabPages)		//one or many rows
                    {
                        refNo += 1;
                        ValidateFields("Reference " + refNo.ToString(), _referenceFieldDef.Tables["Fields"], true);
                    }

                    if (_inError.Count > 0)
                        btnComplete.ImageIndex = 1;
                    else
                        btnComplete.ImageIndex = 2;

                    _inError.Clear();
                }

            }
            catch (Exception ex)
            {
                Catch(ex, "ValidateControl");
            }
        }

        private bool Save(bool complete)
        {
            bool valid = true;
            int refNo = 0;

            try
            {
                Wait();

                ValidateControl(null, null);

                if (btnComplete.ImageIndex == 2) // FA UAT 594 don't save if any field is not valid (ValidateControl sets button image to 1 is not //IP - 26/11/09 - UAT5.2 (926) - Only save if ImageIndex= 2 - fields are valid.
                {
                    complete = btnComplete.ImageIndex == 2;

                    /*

                    // Validate the fields
                    ValidateFields(this.Name, _formFieldDef.Tables["Fields"], complete);
                    foreach (Crownwood.Magic.Controls.TabPage tp in tcReferrences.TabPages)		//one or many rows
                    {
                        refNo = refNo + 1;
                        ValidateFields("Reference " + refNo.ToString(), _referenceFieldDef.Tables["Fields"], complete);
                    }
                    */

                    // Copy the fields back into the same dataset that was originally retrieved
                    // and send that dataset back to the server to save.

                    //if (!DisplayValidationErrors())
                    //{
                    #region applicant 1
                    #region save proposal table
                    DataRow r = _stage2DataA1.Tables[TN.Proposal].Rows[0];

                    switch (drpSpecialPromo.SelectedIndex)
                    {
                        case 0: r[CN.SpecialPromo] = "";
                            break;
                        case 1: r[CN.SpecialPromo] = "N";
                            break;
                        case 2: r[CN.SpecialPromo] = "Y";
                            break;
                        default:
                            break;
                    }
                    r[CN.PAddress1] = tcPrevAddress.txtAddress1.Text;
                    r[CN.PAddress2] = tcPrevAddress.cmbVillage.SelectedIndex != -1 ? (string)tcPrevAddress.cmbVillage.SelectedValue : tcPrevAddress.cmbVillage.Text; // Address Standardization CR2019 - 025
                    r[CN.PCity] = tcPrevAddress.cmbRegion.SelectedIndex != -1 ? (string)tcPrevAddress.cmbRegion.SelectedValue : tcPrevAddress.cmbRegion.Text; // Address Standardization CR2019 - 025
                    r[CN.PPostCode] = tcPrevAddress.txtPostCode.Text;
                    r[CN.EmployeeName] = txtEmpName.Text;
                    r[CN.EmpDept] = txtEmpDept.Text;
                    r[CN.EAddress1] = tcEmpAddress.txtAddress1.Text;
                    r[CN.EAddress2] = tcEmpAddress.cmbVillage.SelectedIndex != -1 ? (string)tcEmpAddress.cmbVillage.SelectedValue : tcEmpAddress.cmbVillage.Text; // Address Standardization CR2019 - 025
                    r[CN.ECity] = tcEmpAddress.cmbRegion.SelectedIndex != -1 ? (string)tcEmpAddress.cmbRegion.SelectedValue : tcEmpAddress.cmbRegion.Text; // Address Standardization CR2019 - 025
                    r[CN.EPostCode] = tcEmpAddress.txtPostCode.Text;
                    r[CN.NoOfRef] = tcReferrences.TabPages.Count;
                    r[CN.NewComment] = txtNewComment.Text;
                    r[CN.VehicleRegistration] = txtRegistration.Text;
                    #endregion

                    #region save employment table
                    r = _stage2DataA1.Tables[TN.Employment].Rows[0];

                    r[CN.StaffNo] = txtStaffNo.Text;

                    #endregion

                    #region save references

                    // Clear the old references
                    _stage2DataA1.Tables[TN.References].Clear();

                    // Create the new references
                    bool landlordFound = false;
                    ReferenceTab rt;
                    foreach (Crownwood.Magic.Controls.TabPage tp in tcReferrences.TabPages)		//one or many rows
                    {
                        rt = (ReferenceTab)tp.Controls[0];
                        r = _stage2DataA1.Tables[TN.References].NewRow();
                        r[CN.RefFirstName] = rt.txtFirstName.Text;
                        r[CN.RefLastName] = rt.txtLastName.Text;
                        r[CN.RefRelation] = ((DataRowView)rt.drpRelation.SelectedItem)[CN.Code];
                        r[CN.YrsKnown] = Convert.ToInt32(rt.txtYearsKnown.Text);
                        r[CN.RefAddress1] = rt.txtAddress1.Text;
                        r[CN.RefAddress2] = rt.txtAddress2.Text;
                        r[CN.RefCity] = rt.txtAddress3.Text;
                        r[CN.RefPostCode] = rt.txtPostCode.Text;
                        r[CN.RefWAddress1] = rt.txtWAddress1.Text;
                        r[CN.RefWAddress2] = rt.txtWAddress2.Text;
                        r[CN.RefWCity] = rt.txtWAddress3.Text;
                        r[CN.RefWPostCode] = rt.txtWPostCode.Text;
                        r[CN.RefDialCode] = rt.txtDialCode.Text;
                        r[CN.RefPhoneNo] = rt.txtPhoneNo.Text;
                        r[CN.RefWDialCode] = rt.txtWDialCode.Text;
                        r[CN.RefWPhoneNo] = rt.txtWPhoneNo.Text;
                        r[CN.RefMDialCode] = rt.txtMDialCode.Text;
                        r[CN.RefMPhoneNo] = rt.txtMPhoneNo.Text;
                        r[CN.RefDirections] = rt.txtDirections.Text;
                        r[CN.RefComment] = rt.txtComment.Text;
                        r[CN.EmpeeNoChange] = rt.empeeNo;
                        r[CN.NewComment] = rt.txtNewComment.Text;


                        if ((string)((DataRowView)rt.drpRelation.SelectedItem)[CN.Code] == "L")
                            landlordFound = true;

                        if (rt.txtDateChecked.Text.Length == 0)
                            r[CN.DateChange] = DBNull.Value;
                        else
                            r[CN.DateChange] = Convert.ToDateTime(rt.txtDateChecked.Text);

                        _stage2DataA1.Tables[TN.References].Rows.Add(r);
                    }
                    if (complete && ResidentialStatus == "R" &&
                        (bool)Country[CountryParameterNames.CaptureLandlordDetails])
                    {
                        if (!landlordFound)
                        {
                            ShowInfo("M_NOLANDLORDDETAILS");
                            valid = false;
                        }
                    }

                    #endregion

                    #endregion

                    if (_stage2DataA2 != null)
                    {
                        r = _stage2DataA2.Tables[TN.Customer].Rows[0];
                        r[CN.Address1] = txtEmpName2.Text;
                        r[CN.Address2] = txtAddress1.Text;
                        r[CN.Address3] = txtAddress2.Text;
                        r[CN.PostCode] = txtPostCode.Text;

                        r = _stage2DataA2.Tables[TN.Employment].Rows[0];
                        r[CN.StaffNo] = txtStaffNo2.Text;
                        r[CN.PersDialCode] = txtDialCode2.Text;
                        r[CN.PersTel] = txtPhoneNo2.Text;
                        r[CN.Department] = txtEmpDept2.Text;
                    }

                    if (valid)
                    {
                        CreditManager.SaveProposalStage2(txtCustomerID.Text, this.AccountNo, _stage2DataA1, _stage2DataA2, complete, out _error);
                        if (_error.Length > 0)
                        {
                            ShowError(_error);
                            valid = false;
                        }
                        else
                        {
                            ((MainForm)FormRoot).statusBar1.Text = GetResource("M_STAGE2SAVED");

                            if (complete)
                            {
                                //reload the sanction status control
                                ((MainForm)this.FormRoot).tbSanction.Load(true, this.CustomerID, this.dtDateProp.Value, this.AccountNo, this.acctType, this.ScreenMode);
                                CurrentStatus = ((MainForm)this.FormRoot).tbSanction.CurrentStatus;
                                ((MainForm)this.FormRoot).tbSanction.SetCurrentStage(SS.S2);
                                _readOnly = ((MainForm)this.FormRoot).tbSanction.ReadOnly(SS.S2);

                                // Set the bias on each form control to read only
                                SetFieldBias(this.Name, _formFieldDef.Tables["Fields"], complete);
                                refNo = 0;
                                foreach (Crownwood.Magic.Controls.TabPage tp in tcReferrences.TabPages)		//one or many rows
                                {
                                    refNo = refNo + 1;
                                    SetFieldBias("Reference " + refNo.ToString(), _referenceFieldDef.Tables["Fields"], complete);
                                }
                            }
                        }
                    }
                }
                //}
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
            }
            return valid;
        }

        public override bool ConfirmClose()
        {
            bool status = true;
            try
            {
                Function = "ConfirmClose()";
                Wait();
                AccountManager.UnlockAccount(this.AccountNo, Credential.UserId, out _error);
                if (_error.Length > 0)
                {
                    status = false;
                    ShowError(_error);
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();

                Dispose(true);

                //If dispose is called already then say GC to skip finalize on this instance.

                GC.SuppressFinalize(this);
            }

            return status;
        }


        //
        // Form events
        //
        private void SanctionStage2_Load(object sender, System.EventArgs e)
        {
            toolTip1.SetToolTip(this.btnComplete, GetResource("TT_COMPLETE"));
            //toolTip1.SetToolTip(this.btnSave, GetResource("TT_SAVE"));
            toolTip1.SetToolTip(this.btnPreviousRefs, GetResource("TT_COPYREFERENCES"));
            Function = "SanctionStage2::SanctionStage2_Load()";

            try
            {
                Wait();

                LoadData();

                if (this.AccountLocked)
                {
                    // Set up field biases and highlight mandatory fields
                    SetupFrame();

                    //Load all the static data into the drop down lists
                    LoadStatic();

                    //load existing data for this proposal
                    LoadStage2Details();


                    ValidateControl(null, null);

                    if (_bailReview != null || _telephoneAction != null)
                    {
                        //Only show References tab if S2 entered from Telephone Action or Bailiff Reviw screens  CR1084
                        tcApp1.TabPages.Remove(tpPreviousAddress);
                        tcApp1.TabPages.Remove(tpEmployer);
                        tcApp1.TabPages.Remove(tpComments);

                        if (tcApplicants.TabPages.Count == 2) //IP - 27/09/10 - UAT(35)UAT5.4 - Remove Applicant 2 tab if visible.
                        {
                            tcApplicants.TabPages.Remove(tpApp2);
                        }
                        //((MainForm)this.FormRoot).tbSanction.Visible = false;
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

        private void SanctionStage2_Leave(object sender, System.EventArgs e)
        {
            ((MainForm)this.FormRoot).tbSanction.Visible = false;
        }

        private void SanctionStage2_Enter(object sender, System.EventArgs e)
        {
            try
            {

                ((MainForm)this.FormRoot).tbSanction.CustomerScreen = _customerScreen;
                ((MainForm)this.FormRoot).tbSanction.Settled = false;
                ((MainForm)this.FormRoot).tbSanction.Load(true, this.CustomerID, this.dtDateProp.Value, this.AccountNo, this.acctType, this.ScreenMode);
                CurrentStatus = ((MainForm)this.FormRoot).tbSanction.CurrentStatus;
                ((MainForm)this.FormRoot).tbSanction.SetCurrentStage(SS.S2);
                if (_bailReview != null || _telephoneAction != null)        //CR1084 dont show Stage buttons if entered from TelAction or Bailiff screen
                {
                    ((MainForm)this.FormRoot).tbSanction.Visible = false;
                }
                else
                {
                    ((MainForm)this.FormRoot).tbSanction.Visible = true;
                }
                _readOnly = ((MainForm)this.FormRoot).tbSanction.ReadOnly(SS.S2);

                SetFieldBias(this.Name, _formFieldDef.Tables["Fields"], _readOnly);
                int refNo = 0;
                foreach (Crownwood.Magic.Controls.TabPage tp in tcReferrences.TabPages)		//one or many rows
                {
                    refNo = refNo + 1;
                    SetFieldBias("Reference " + refNo.ToString(), _referenceFieldDef.Tables["Fields"], _readOnly);
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "SanctionStage2_Enter");
            }
        }

        private void btnEnter_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (tcReferrences.TabPages.Count < SanctionStage2._maxReferences)
                {
                    this.CreateReferenceTab();
                    tcReferrences.SelectedIndex = tcReferrences.TabPages.Count - 1;
                    if (tcReferrences.TabPages.Count == SanctionStage2._maxReferences)
                    {
                        this.btnPreviousRefs.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnEnter_Click");
            }
        }

        private void btnRemove_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (tcReferrences.TabPages.Count > (decimal)Country[CountryParameterNames.MinReferences])
                {
                    Crownwood.Magic.Controls.TabPage tp = tcReferrences.SelectedTab;
                    tcReferrences.TabPages.Remove(tp);
                    this.btnPreviousRefs.Enabled = true;

                    // Renumber the tab titles - no longer needed - titles are Brother etc
                    //int refNo = 1;
                    //foreach (Crownwood.Magic.Controls.TabPage tpr in tcReferrences.TabPages)		//one or many rows
                    //{
                    //	tpr.Title = "Reference "+refNo.ToString();
                    //	refNo = refNo + 1;
                    //}
                }
                else
                {
                    short minRefs = ((decimal)Country[CountryParameterNames.MinReferences] <= SanctionStage2._maxReferences) ? Convert.ToInt16(Country[CountryParameterNames.MinReferences]) : SanctionStage2._maxReferences;

                    ShowInfo("M_MINREFERENCES", new object[] { minRefs.ToString() });
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnRemove_Click");
            }
        }

        private void btnSave_Click(object sender, System.EventArgs e)
        {
            // Save but do not complete
            Save(false);
        }

        private void btnComplete_Click(object sender, System.EventArgs e)
        {
            // Save and complete - this screen becomes read only
            Save(true);
        }

        //private void menuItem2_Click(object sender, System.EventArgs e)
        //{
        //    Close();
        //}

        private void menuExit_Click(object sender, System.EventArgs e)
        {
            CloseTab();
        }

        private void menuReopen_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (((MainForm)FormRoot).tbSanction.ReadOnly(SS.S2))
                {
                    CreditManager.UnClearFlag(AccountNo, SS.S2, true, Credential.UserId, out _error);
                    if (_error.Length > 0)
                        ShowError(_error);
                    else
                    {
                        SanctionStage2_Enter(sender, e);
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

        private void btnPreviousRefs_Click(object sender, System.EventArgs e)
        {
            try
            {
                Function = "Sanction Stage 2 Screen: Reference List popup button";

                Wait();

                // Call the reference list popup form
                string custName = this.txtFirstName.Text + "  " + this.txtLastName.Text;
                int copyLeft = SanctionStage2._maxReferences - tcReferrences.TabPages.Count;
                ReferenceList ReferenceListPopup = new ReferenceList(this.txtCustomerID.Text, custName, copyLeft);
                ReferenceListPopup.ShowDialog();
                DataTable referenceTable = ReferenceListPopup.referenceTable;
                foreach (DataRow row in ReferenceListPopup.referenceTable.Rows)
                {
                    if (row.RowState != DataRowState.Deleted && tcReferrences.TabPages.Count < SanctionStage2._maxReferences)
                    {
                        this.PopulateReference(row);
                    }
                }

                tcReferrences.SelectedIndex = tcReferrences.TabPages.Count - 1;
                if (tcReferrences.TabPages.Count == SanctionStage2._maxReferences)
                {
                    this.btnPreviousRefs.Enabled = false;
                }
                ReferenceListPopup.Dispose();
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
