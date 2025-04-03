using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using STL.Common;
using STL.PL.WS5;
using System.Web.Services.Protocols;
using STL.Common.Static;
using System.Xml;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;
using STL.PL.WS12;



namespace STL.PL
{
    /// <summary>
    /// User control of address details that appears as a common set
    /// of address details on various screens. Some screens only
    /// display a subset of the fields. For user entry postcode
    /// validation is provided and the customer deluvery area can
    /// be selected.
    /// </summary>
    /// 

    public delegate void OnChangeAddressDate(object sender, GenericEventHandler<DateTime> date);



    public class AddressTab : UserControl
    {

        public event OnChangeAddressDate onChangeAddressDate;

        private bool _readonly = false;
        private bool _enable = true; // Address Standardization CR2019 - 025
        private bool _SimpleAddress = false;
        private string _error = "";
        private bool _valid;
        private string _deliveryArea = "";
        public bool ValidatePostcode = true;
        private string _zone = "";      //CR1084
        private DataTable delAreas = null;  //#12224 - CR12249

        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.ToolTip toolTip1;

        // Address Standardization CR2019 - 025
        public CustomControl.WatermarkTextBox txtAddress1;
        public CustomControl.AutoSuggestCombo cmbVillage;
        public CustomControl.AutoSuggestCombo cmbRegion;
        public CustomControl.WatermarkTextBox txtPostCode;
        public CustomControl.WatermarkTextBox txtCoordinate;
        // Address Standardization CR2019 - 025

        public System.Windows.Forms.TextBox CFirstname;//KD
        public System.Windows.Forms.TextBox CLastname;//KD
                                                      //public System.Windows.Forms.ComboBox TitleCDrp;
        public System.Windows.Forms.TextBox txtDialCode;
        public System.Windows.Forms.TextBox txtPhoneNo;
        public System.Windows.Forms.TextBox txtExtension;
        public System.Windows.Forms.TextBox txtMobile;
        public System.Windows.Forms.TextBox txtEmail;
        public System.Windows.Forms.Label lDateIn;
        public System.Windows.Forms.DateTimePicker dtDateIn;
        public System.Windows.Forms.ComboBox drpDeliveryArea;
        public System.Windows.Forms.TextBox txtNotes;

        private System.Windows.Forms.Label lDialCode;
        private System.Windows.Forms.Label lPhoneNo;
        private System.Windows.Forms.Label lExtension;
        private System.Windows.Forms.Label lMobile;
        private System.Windows.Forms.Label lEmail;
        private System.Windows.Forms.Label lDeliveryArea;
        private System.Windows.Forms.Button btnNotes;
        private System.Windows.Forms.Button btnSaveNotes;
        public Button btnMobile;
        private System.ComponentModel.IContainer components;

        private TextBox _mobile2 = new TextBox();
        private TextBox _mobile3 = new TextBox();
        public ComboBox drpZone;
        private Label lbZone;
        private TextBox _mobile4 = new TextBox();
        public Button btnWork;

        Form formRoot;
        //Return textboxes for additional mobile numbers to keep usage consistent
        public TextBox txtMobile2
        {
            get
            {
                return _mobile2;
            }
        }

        public TextBox txtMobile3
        {
            get
            {
                return _mobile3;
            }
        }

        public TextBox txtMobile4
        {
            get
            {
                return _mobile4;
            }
        }

        //**********************************************************

        //IP - 16/03/11 - #3317 - CR1245
        private TextBox _workDialCode2 = new TextBox();
        public TextBox txtWorkDialCode2
        {
            get
            {
                return _workDialCode2;
            }
        }

        private TextBox _workDialCode3 = new TextBox();
        public TextBox txtWorkDialCode3
        {
            get
            {
                return _workDialCode3;
            }
        }

        private TextBox _workDialCode4 = new TextBox();
        public TextBox txtWorkDialCode4
        {
            get
            {
                return _workDialCode4;
            }
        }

        private TextBox _workNum2 = new TextBox();
        public TextBox txtWorkNum2
        {
            get
            {
                return _workNum2;
            }
        }

        private TextBox _workNum3 = new TextBox();
        public TextBox txtWorkNum3
        {
            get
            {
                return _workNum3;
            }
        }

        private TextBox _workNum4 = new TextBox();
        public TextBox txtWorkNum4
        {
            get
            {
                return _workNum4;
            }
        }

        private TextBox _workExt2 = new TextBox();
        public TextBox txtWorkExt2
        {
            get
            {
                return _workExt2;
            }
        }

        private TextBox _workExt3 = new TextBox();
        public Button btnDelAreas;
        private ToolTip toolTipDelArea;
        //private Label Title;
        //private Label LastName;
        //private Label FirstName;
        public ComboBox drptitleC;
        public Label FirstName;
        public Label Title;
        public Label LastName;
        public CheckBox chkSelected;
        public TextBox txtWorkExt3
        {
            get
            {
                return _workExt3;
            }
        }

        private TextBox _workExt4 = new TextBox();
        public TextBox txtWorkExt4
        {
            get
            {
                return _workExt4;
            }
        }


        //**********************************************************

        public bool ReadOnly
        {
            get { return _readonly; }
            set
            {
                _readonly = value;
                this.txtAddress1.ReadOnly = value;

                this.cmbVillage.Enabled = !value; // Address Standardization CR2019 - 025
                this.cmbRegion.Enabled = !value; // Address Standardization CR2019 - 025
                this.txtCoordinate.ReadOnly = value; // Address Standardization CR2019 - 025
                // this.TitleC.ReadOnly = value;
                this.CFirstname.ReadOnly = value;
                this.CLastname.ReadOnly = value;
                this.drptitleC.Enabled = !value;
                this.txtPostCode.ReadOnly = value;
                this.txtDialCode.ReadOnly = value;
                this.txtPhoneNo.ReadOnly = value;
                this.txtExtension.ReadOnly = value;
                this.txtEmail.ReadOnly = value;
                this.dtDateIn.Enabled = !value;
                this.txtNotes.ReadOnly = value;
                this.txtNotes.BackColor = SystemColors.Window;
                this.txtMobile.ReadOnly = value;
                this.drpDeliveryArea.Enabled = !value;
            }
        }
        // Address Standardization CR2019 - 025
        public bool Enable
        {
            get { return _enable; }
            set
            {
                _enable = value;

                this.txtAddress1.Enabled = _enable;
                this.cmbVillage.Enabled = _enable;
                this.cmbRegion.Enabled = _enable;
                this.txtCoordinate.Enabled = _enable;
                this.CFirstname.Enabled = _enable;
                this.CLastname.Enabled = _enable;
                this.drptitleC.Enabled = _enable;
                this.txtPostCode.Enabled = _enable;
                this.txtDialCode.Enabled = _enable;
                this.txtPhoneNo.Enabled = _enable;
                this.txtExtension.Enabled = _enable;
                this.txtEmail.Enabled = _enable;
                this.dtDateIn.Enabled = _enable;
                this.txtNotes.Enabled = _enable;
                this.txtMobile.Enabled = _enable;
            }
        }
        // When set to true only display the address lines
        public bool SimpleAddress
        {
            get { return _SimpleAddress; }
            set
            {
                _SimpleAddress = value;
                if (_SimpleAddress)
                {
                    this.txtDialCode.Visible = false;
                    this.txtPhoneNo.Visible = false;
                    this.txtExtension.Visible = false;
                    this.txtEmail.Visible = false;
                    this.dtDateIn.Visible = false;
                    this.lDialCode.Visible = false;
                    this.lPhoneNo.Visible = false;
                    this.lExtension.Visible = false;
                    this.lEmail.Visible = false;
                    this.lDateIn.Visible = false;
                    this.btnNotes.Visible = false;
                    this.txtMobile.Visible = false;
                    this.lMobile.Visible = false;
                    this.drpDeliveryArea.Visible = false;
                    this.lDeliveryArea.Visible = false;
                    this.Width = 184;
                    this.btnMobile.Visible = false;
                    this.drpZone.Visible = false;       //CR1084
                }
                else
                {
                    this.txtDialCode.Visible = true;
                    this.txtPhoneNo.Visible = true;
                    this.txtExtension.Visible = true;
                    this.txtEmail.Visible = true;
                    this.dtDateIn.Visible = true;
                    this.lDialCode.Visible = true;
                    this.lPhoneNo.Visible = true;
                    this.lExtension.Visible = true;
                    this.lEmail.Visible = true;
                    this.lDateIn.Visible = true;
                    this.btnNotes.Visible = true;
                    this.txtMobile.Visible = true;
                    this.lMobile.Visible = true;
                    this.drpDeliveryArea.Visible = true;
                    this.lDeliveryArea.Visible = true;
                    this.Width = 376;

                }
            }
        }

        public AddressTab(Form formRoot, Form parent, string addtype) : this(formRoot)
        {
            if (addtype == "H")
            {
                ((BasicCustomerDetails)parent).onChangeAddressDate += new OnChangeAddressDate(AddressTab_onChangeAddressDate);
            }
            if (addtype.Contains("D") == false)
            {
                Title.Visible = false;
                FirstName.Visible = false;
                LastName.Visible = false;
                drptitleC.Visible = false;
                CFirstname.Visible = false;
                CLastname.Visible = false;
            }
            if (addtype.Contains("W") == true) // Address Standardization CR2019 - 025
            {
                Title.Visible = false;
                FirstName.Visible = true;
                FirstName.Text = "Company";
                LastName.Visible = false;
                drptitleC.Visible = false;
                CFirstname.Visible = true;
                CLastname.Visible = false;
            }
        }

        public AddressTab(Form formRoot)
        {
            InitializeComponent();
            this.dtDateIn.Value = DateTime.Today;
            this.formRoot = formRoot;
            if(this.formRoot != null) // Address Standardization CR2019 - 025
                PopulateVillages(); // Address Standardization CR2019 - 025
        }

        public AddressTab(bool readOnly, Form formRoot, Form parent, string addtype) : this(formRoot)
        {
            this.Dock = System.Windows.Forms.DockStyle.Fill;
            ReadOnly = readOnly;
            if (addtype == "H")
            {
                ((BasicCustomerDetails)parent).onChangeAddressDate += new OnChangeAddressDate(AddressTab_onChangeAddressDate);
            }

            if (addtype.Contains("D") == false)
            {
                Title.Visible = false;
                FirstName.Visible = false;
                LastName.Visible = false;
                drptitleC.Visible = false;
                CFirstname.Visible = false;
                CLastname.Visible = false;
            }
            if (addtype.Contains("W") == true) // Address Standardization CR2019 - 025
            {
                Title.Visible = false;
                FirstName.Visible = true;
                FirstName.Text = "Company";
                LastName.Visible = false;
                drptitleC.Visible = false;
                CFirstname.Visible = true;
                CLastname.Visible = false;
            }
        }

        public AddressTab(bool readOnly, Form formRoot, string addtype) : this(formRoot)
        {
            this.Dock = System.Windows.Forms.DockStyle.Fill;
            ReadOnly = readOnly;

            if (addtype.Contains("D") == false)
            {
                Title.Visible = false;
                FirstName.Visible = false;
                LastName.Visible = false;
                drptitleC.Visible = false;
                CFirstname.Visible = false;
                CLastname.Visible = false;
            }
            if (addtype.Contains("W") == true) // Address Standardization CR2019 - 025
            {
                Title.Visible = false;
                FirstName.Visible = true;
                FirstName.Text = "Company";
                LastName.Visible = false;
                drptitleC.Visible = false;
                CFirstname.Visible = true;
                CLastname.Visible = false;
            }
        }

        void AddressTab_onChangeAddressDate(object sender, GenericEventHandler<DateTime> date)
        {
            if (date.Results != dtDateIn.Value)
                dtDateIn.Value = date.Results;
        }

        public AddressTab(TranslationDummy d, Form formRoot)
        {
            InitializeComponent();
            this.formRoot = formRoot;
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

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddressTab));
            this.lDialCode = new System.Windows.Forms.Label();
            this.dtDateIn = new System.Windows.Forms.DateTimePicker();
            this.lDateIn = new System.Windows.Forms.Label();
            this.txtPhoneNo = new System.Windows.Forms.TextBox();
            this.CFirstname = new System.Windows.Forms.TextBox();
            this.CLastname = new System.Windows.Forms.TextBox();
            this.txtPostCode = new STL.PL.CustomControl.WatermarkTextBox(); // Address Standardization CR2019 - 025
            this.txtAddress1 = new STL.PL.CustomControl.WatermarkTextBox(); // Address Standardization CR2019 - 025
            this.txtCoordinate = new STL.PL.CustomControl.WatermarkTextBox(); // Address Standardization CR2019 - 025
            this.cmbRegion = new STL.PL.CustomControl.AutoSuggestCombo(); // Address Standardization CR2019 - 025
            this.cmbVillage = new STL.PL.CustomControl.AutoSuggestCombo(); // Address Standardization CR2019 - 025
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.txtExtension = new System.Windows.Forms.TextBox();
            this.txtDialCode = new System.Windows.Forms.TextBox();
            this.lEmail = new System.Windows.Forms.Label();
            this.lExtension = new System.Windows.Forms.Label();
            this.lPhoneNo = new System.Windows.Forms.Label();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnNotes = new System.Windows.Forms.Button();
            this.btnSaveNotes = new System.Windows.Forms.Button();
            this.txtMobile = new System.Windows.Forms.TextBox();
            this.lMobile = new System.Windows.Forms.Label();
            this.drpDeliveryArea = new System.Windows.Forms.ComboBox();
            this.lDeliveryArea = new System.Windows.Forms.Label();
            this.drpZone = new System.Windows.Forms.ComboBox();
            this.lbZone = new System.Windows.Forms.Label();
            this.btnDelAreas = new System.Windows.Forms.Button();
            this.btnWork = new System.Windows.Forms.Button();
            this.btnMobile = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.toolTipDelArea = new System.Windows.Forms.ToolTip(this.components);
            this.drptitleC = new System.Windows.Forms.ComboBox();
            this.FirstName = new System.Windows.Forms.Label();
            this.Title = new System.Windows.Forms.Label();
            this.LastName = new System.Windows.Forms.Label();
            this.chkSelected = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // lDialCode
            // 
            this.lDialCode.BackColor = System.Drawing.SystemColors.Control;
            this.lDialCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lDialCode.Location = new System.Drawing.Point(295, 30);
            this.lDialCode.Margin = new System.Windows.Forms.Padding(0);
            this.lDialCode.Name = "lDialCode";
            this.lDialCode.Size = new System.Drawing.Size(8, 8);
            this.lDialCode.TabIndex = 38;
            this.lDialCode.Text = "-";
            this.lDialCode.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // dtDateIn
            // 
            this.dtDateIn.CustomFormat = "MMM yyyy";
            this.dtDateIn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtDateIn.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtDateIn.Location = new System.Drawing.Point(255, 103);
            this.dtDateIn.Name = "dtDateIn";
            this.dtDateIn.Size = new System.Drawing.Size(77, 20);
            this.dtDateIn.TabIndex = 9;
            this.dtDateIn.Tag = "";
            this.dtDateIn.Value = new System.DateTime(2002, 5, 14, 0, 0, 0, 0);
            this.dtDateIn.ValueChanged += new System.EventHandler(this.dtDateIn_ValueChanged);
            this.dtDateIn.Validating += new System.ComponentModel.CancelEventHandler(this.dtDateIn_Validating);
            // 
            // lDateIn
            // 
            this.lDateIn.BackColor = System.Drawing.SystemColors.Control;
            this.lDateIn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lDateIn.Location = new System.Drawing.Point(210, 105);
            this.lDateIn.Name = "lDateIn";
            this.lDateIn.Size = new System.Drawing.Size(45, 16);
            this.lDateIn.TabIndex = 36;
            this.lDateIn.Text = "Date In";
            this.lDateIn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtPhoneNo
            // 
            this.txtPhoneNo.BackColor = System.Drawing.SystemColors.Window;
            this.txtPhoneNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPhoneNo.Location = new System.Drawing.Point(311, 26);
            this.txtPhoneNo.MaxLength = 20;
            this.txtPhoneNo.Name = "txtPhoneNo";
            this.txtPhoneNo.Size = new System.Drawing.Size(74, 19);
            this.txtPhoneNo.TabIndex = 5;
            this.txtPhoneNo.Tag = "";
            this.txtPhoneNo.Validating += new System.ComponentModel.CancelEventHandler(this.txtPhoneNo_Validating);
            // Address Standardization CR2019 - 025
            // 
            // txtPostCode
            //
            this.txtPostCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPostCode.Location = new System.Drawing.Point(9, 78);
            this.txtPostCode.Name = "txtPostCode";
            this.txtPostCode.Size = new System.Drawing.Size(160, 17);
            this.txtPostCode.TabIndex = 3;
            this.txtPostCode.Watermark = "Zip Code";
            // 
            // txtAddress1
            // 
            this.txtAddress1.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAddress1.Location = new System.Drawing.Point(9, 28);
            this.txtAddress1.Name = "txtAddress1";
            this.txtAddress1.Size = new System.Drawing.Size(160, 17);
            this.txtAddress1.TabIndex = 75;
            this.txtAddress1.Watermark = "House No, Street Name, Area";
            this.txtAddress1.TabIndex = 0;
            this.txtAddress1.Validating += new CancelEventHandler(txtAddress1_Validating);
            // 
            // txtCoordinate
            //
            this.txtCoordinate.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCoordinate.Location = new System.Drawing.Point(9, 93);
            this.txtCoordinate.Name = "txtCoordinate";
            this.txtCoordinate.Size = new System.Drawing.Size(160, 17);
            this.txtCoordinate.TabIndex = 4;
            this.txtCoordinate.Watermark = "Latitude, Longitude e.g. 10.35, -50.45";
            this.txtCoordinate.Validating += new CancelEventHandler(txtCoordinate_Validating);
            // 
            // cmbRegion
            // 
            this.cmbRegion.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cmbRegion.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbRegion.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)), true);
            this.cmbRegion.FormattingEnabled = true;
            this.cmbRegion.Location = new System.Drawing.Point(9, 62);
            this.cmbRegion.Name = "cmbRegion";
            this.cmbRegion.Size = new System.Drawing.Size(160, 20);
            this.cmbRegion.TabIndex = 2;
            this.cmbRegion.Watermark = "Parish / Regional Corporation";
            this.cmbRegion.SelectedIndexChanged += new System.EventHandler(cmbRegion_SelectedIndexChanged);
            this.cmbRegion.Validating += new CancelEventHandler(cmbRegion_Validating);
            // 
            // cmbVillage
            // 
            this.cmbVillage.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cmbVillage.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbVillage.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbVillage.FormattingEnabled = true;
            this.cmbVillage.Location = new System.Drawing.Point(9, 45);
            this.cmbVillage.Name = "cmbVillage";
            this.cmbVillage.Size = new System.Drawing.Size(160, 20);
            this.cmbVillage.TabIndex = 1;
            this.cmbVillage.Watermark = "Village / Town / City";
            this.cmbVillage.SelectedIndexChanged += new System.EventHandler(cmbVillage_SelectedIndexChanged);
            this.cmbVillage.Validating += new CancelEventHandler(cmbVillage_Validating);
            // Address Standardization CR2019 - 025
            // 
            // CFirstname
            // 
            this.CFirstname.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CFirstname.Location = new System.Drawing.Point(133, 4);
            this.CFirstname.MinimumSize = new System.Drawing.Size(50, 15);
            this.CFirstname.Name = "CFirstname";
            this.CFirstname.Size = new System.Drawing.Size(110, 19);
            this.CFirstname.TabIndex = 66;
            // 
            // CLastname
            // 
            this.CLastname.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CLastname.Location = new System.Drawing.Point(299, 4);
            this.CLastname.Name = "CLastname";
            this.CLastname.Size = new System.Drawing.Size(71, 19);
            this.CLastname.TabIndex = 64;
            
            // 
            // txtEmail
            // 
            this.txtEmail.BackColor = System.Drawing.SystemColors.Window;
            this.txtEmail.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtEmail.Location = new System.Drawing.Point(255, 63);
            this.txtEmail.MaxLength = 60;
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(130, 19);
            this.txtEmail.TabIndex = 7;
            this.txtEmail.Tag = "";
            // 
            // txtExtension
            // 
            this.txtExtension.BackColor = System.Drawing.SystemColors.Window;
            this.txtExtension.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtExtension.Location = new System.Drawing.Point(255, 45);
            this.txtExtension.MaxLength = 6;
            this.txtExtension.Name = "txtExtension";
            this.txtExtension.Size = new System.Drawing.Size(130, 19);
            this.txtExtension.TabIndex = 6;
            this.txtExtension.Tag = "";
            // 
            // txtDialCode
            // 
            this.txtDialCode.BackColor = System.Drawing.SystemColors.Window;
            this.txtDialCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDialCode.Location = new System.Drawing.Point(255, 26);
            this.txtDialCode.MaxLength = 8;
            this.txtDialCode.Name = "txtDialCode";
            this.txtDialCode.Size = new System.Drawing.Size(32, 19);
            this.txtDialCode.TabIndex = 4;
            this.txtDialCode.Tag = "";
            this.txtDialCode.Validating += new System.ComponentModel.CancelEventHandler(this.txtDialCode_Validating);
            // 
            // lEmail
            // 
            this.lEmail.BackColor = System.Drawing.SystemColors.Control;
            this.lEmail.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lEmail.Location = new System.Drawing.Point(210, 67);
            this.lEmail.Name = "lEmail";
            this.lEmail.Size = new System.Drawing.Size(40, 16);
            this.lEmail.TabIndex = 29;
            this.lEmail.Text = "E-mail";
            this.lEmail.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lExtension
            // 
            this.lExtension.BackColor = System.Drawing.SystemColors.Control;
            this.lExtension.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lExtension.Location = new System.Drawing.Point(210, 49);
            this.lExtension.Name = "lExtension";
            this.lExtension.Size = new System.Drawing.Size(32, 16);
            this.lExtension.TabIndex = 28;
            this.lExtension.Text = "Ext";
            this.lExtension.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lPhoneNo
            // 
            this.lPhoneNo.BackColor = System.Drawing.SystemColors.Control;
            this.lPhoneNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lPhoneNo.Location = new System.Drawing.Point(210, 30);
            this.lPhoneNo.Name = "lPhoneNo";
            this.lPhoneNo.Size = new System.Drawing.Size(40, 16);
            this.lPhoneNo.TabIndex = 26;
            this.lPhoneNo.Text = "Phone";
            this.lPhoneNo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtNotes
            // 
            this.txtNotes.Location = new System.Drawing.Point(9, 2);
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Size = new System.Drawing.Size(332, 126);
            this.txtNotes.TabIndex = 11;
            this.txtNotes.Visible = false;
            // 
            // btnNotes
            // 
            this.errorProvider1.SetIconAlignment(this.btnNotes, System.Windows.Forms.ErrorIconAlignment.TopLeft);
            this.btnNotes.Image = ((System.Drawing.Image)(resources.GetObject("btnNotes.Image")));
            this.btnNotes.Location = new System.Drawing.Point(355, 101);
            this.btnNotes.Name = "btnNotes";
            this.btnNotes.Size = new System.Drawing.Size(32, 24);
            this.btnNotes.TabIndex = 10;
            this.toolTip1.SetToolTip(this.btnNotes, "Notes");
            this.btnNotes.Click += new System.EventHandler(this.btnNotes_Click);
            // 
            // btnSaveNotes
            // 
            this.btnSaveNotes.Image = ((System.Drawing.Image)(resources.GetObject("btnSaveNotes.Image")));
            this.btnSaveNotes.Location = new System.Drawing.Point(355, 101);
            this.btnSaveNotes.Name = "btnSaveNotes";
            this.btnSaveNotes.Size = new System.Drawing.Size(32, 24);
            this.btnSaveNotes.TabIndex = 41;
            this.toolTip1.SetToolTip(this.btnSaveNotes, "Save");
            this.btnSaveNotes.Visible = false;
            this.btnSaveNotes.Click += new System.EventHandler(this.btnSaveNotes_Click);
            // 
            // txtMobile
            // 
            this.txtMobile.BackColor = System.Drawing.SystemColors.Window;
            this.txtMobile.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMobile.Location = new System.Drawing.Point(255, 81);
            this.txtMobile.MaxLength = 60;
            this.txtMobile.Name = "txtMobile";
            this.txtMobile.Size = new System.Drawing.Size(130, 19);
            this.txtMobile.TabIndex = 8;
            this.txtMobile.Tag = "";
            this.txtMobile.Validating += new CancelEventHandler(txtMobile_Validating); // Address Standardization CR2019 - 025
            // 
            // lMobile
            // 
            this.lMobile.BackColor = System.Drawing.SystemColors.Control;
            this.lMobile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lMobile.Location = new System.Drawing.Point(210, 85);
            this.lMobile.Name = "lMobile";
            this.lMobile.Size = new System.Drawing.Size(40, 16);
            this.lMobile.TabIndex = 43;
            this.lMobile.Text = "Mobile";
            this.lMobile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // drpDeliveryArea
            // 
            this.drpDeliveryArea.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpDeliveryArea.DropDownWidth = 40;
            this.drpDeliveryArea.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.drpDeliveryArea.Location = new System.Drawing.Point(92, 109);
            this.drpDeliveryArea.Name = "drpDeliveryArea";
            this.drpDeliveryArea.Size = new System.Drawing.Size(77, 20);
            this.drpDeliveryArea.TabIndex = 54;
            this.drpDeliveryArea.SelectedIndexChanged += new System.EventHandler(this.drpDeliveryArea_SelectedIndexChanged);
            // 
            // lDeliveryArea
            // 
            this.lDeliveryArea.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lDeliveryArea.Location = new System.Drawing.Point(15, 111);
            this.lDeliveryArea.Name = "lDeliveryArea";
            this.lDeliveryArea.Size = new System.Drawing.Size(71, 13);
            this.lDeliveryArea.TabIndex = 55;
            this.lDeliveryArea.Text = "Delivery Area";
            this.lDeliveryArea.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // drpZone
            // 
            this.drpZone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpZone.FormattingEnabled = true;
            this.drpZone.Location = new System.Drawing.Point(210, 130);
            this.drpZone.Name = "drpZone";
            this.drpZone.Size = new System.Drawing.Size(126, 21);
            this.drpZone.TabIndex = 57;
            // 
            // lbZone
            // 
            this.lbZone.AutoSize = true;
            this.lbZone.Location = new System.Drawing.Point(8, 180);
            this.lbZone.Name = "lbZone";
            this.lbZone.Size = new System.Drawing.Size(81, 13);
            this.lbZone.TabIndex = 58;
            this.lbZone.Text = "Collection Zone";
            // 
            // btnDelAreas
            // 
            this.btnDelAreas.BackColor = System.Drawing.SystemColors.HotTrack;
            this.btnDelAreas.Image = ((System.Drawing.Image)(resources.GetObject("btnDelAreas.Image")));
            this.btnDelAreas.Location = new System.Drawing.Point(175, 111);
            this.btnDelAreas.Name = "btnDelAreas";
            this.btnDelAreas.Size = new System.Drawing.Size(20, 18);
            this.btnDelAreas.TabIndex = 60;
            this.btnDelAreas.UseVisualStyleBackColor = false;
            this.btnDelAreas.Click += new System.EventHandler(this.btnDelAreas_Click);
            // 
            // btnWork
            // 
            this.btnWork.BackColor = System.Drawing.SystemColors.HotTrack;
            this.btnWork.Image = ((System.Drawing.Image)(resources.GetObject("btnWork.Image")));
            this.btnWork.Location = new System.Drawing.Point(391, 26);
            this.btnWork.Name = "btnWork";
            this.btnWork.Size = new System.Drawing.Size(20, 18);
            this.btnWork.TabIndex = 59;
            this.btnWork.UseVisualStyleBackColor = false;
            this.btnWork.Click += new System.EventHandler(this.btnWork_Click);
            // 
            // btnMobile
            // 
            this.btnMobile.BackColor = System.Drawing.SystemColors.HotTrack;
            this.btnMobile.Image = ((System.Drawing.Image)(resources.GetObject("btnMobile.Image")));
            this.btnMobile.Location = new System.Drawing.Point(391, 81);
            this.btnMobile.Name = "btnMobile";
            this.btnMobile.Size = new System.Drawing.Size(20, 18);
            this.btnMobile.TabIndex = 56;
            this.btnMobile.UseVisualStyleBackColor = false;
            this.btnMobile.Click += new System.EventHandler(this.btnMobile_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // drptitleC
            // 
            this.drptitleC.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drptitleC.DropDownWidth = 45;
            this.drptitleC.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.drptitleC.FormattingEnabled = true;
            this.drptitleC.Location = new System.Drawing.Point(38, 4);
            this.drptitleC.Name = "drptitleC";
            this.drptitleC.Size = new System.Drawing.Size(41, 21);
            this.drptitleC.TabIndex = 65;
            this.drptitleC.SelectedIndexChanged += new System.EventHandler(this.drptitleC_SelectedIndexChanged);
            // 
            // FirstName
            // 
            this.FirstName.AutoSize = true;
            this.FirstName.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FirstName.Location = new System.Drawing.Point(80, 6);
            this.FirstName.Name = "FirstName";
            this.FirstName.Size = new System.Drawing.Size(53, 13);
            this.FirstName.TabIndex = 67;
            this.FirstName.Text = "FirstName";
            // 
            // Title
            // 
            this.Title.AutoSize = true;
            this.Title.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Title.Location = new System.Drawing.Point(10, 7);
            this.Title.Name = "Title";
            this.Title.Size = new System.Drawing.Size(25, 13);
            this.Title.TabIndex = 67;
            this.Title.Text = "Title";
            // 
            // LastName
            // 
            this.LastName.AutoSize = true;
            this.LastName.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LastName.Location = new System.Drawing.Point(245, 6);
            this.LastName.Name = "LastName";
            this.LastName.Size = new System.Drawing.Size(54, 13);
            this.LastName.TabIndex = 67;
            this.LastName.Text = "LastName";
            // 
            // chkSelected
            // 
            this.chkSelected.AutoSize = true;
            this.chkSelected.Location = new System.Drawing.Point(372, 6);
            this.chkSelected.Name = "chkSelected";
            this.chkSelected.Size = new System.Drawing.Size(15, 14);
            this.chkSelected.TabIndex = 68;
            this.chkSelected.UseVisualStyleBackColor = true;
            this.chkSelected.Visible = false;
            // 
            // AddressTab
            // 
            this.Controls.Add(this.chkSelected);
            this.Controls.Add(this.Title);
            this.Controls.Add(this.LastName);
            this.Controls.Add(this.FirstName);
            this.Controls.Add(this.drptitleC);
            this.Controls.Add(this.btnDelAreas);
            this.Controls.Add(this.btnWork);
            this.Controls.Add(this.lbZone);
            this.Controls.Add(this.drpZone);
            this.Controls.Add(this.btnMobile);
            this.Controls.Add(this.drpDeliveryArea);
            this.Controls.Add(this.lDeliveryArea);
            this.Controls.Add(this.lMobile);
            this.Controls.Add(this.txtMobile);
            this.Controls.Add(this.lDialCode);
            this.Controls.Add(this.dtDateIn);
            this.Controls.Add(this.lDateIn);
            this.Controls.Add(this.txtPostCode); // Address Standardization CR2019 - 025
            this.Controls.Add(this.txtAddress1); // Address Standardization CR2019 - 025
            this.Controls.Add(this.txtCoordinate); // Address Standardization CR2019 - 025
            this.Controls.Add(this.cmbRegion); // Address Standardization CR2019 - 025
            this.Controls.Add(this.cmbVillage); // Address Standardization CR2019 - 025
            this.Controls.Add(this.CFirstname);
            this.Controls.Add(this.CLastname);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.txtExtension);
            this.Controls.Add(this.txtDialCode);
            this.Controls.Add(this.lEmail);
            this.Controls.Add(this.lExtension);
            this.Controls.Add(this.lPhoneNo);
            this.Controls.Add(this.txtPhoneNo);
            this.Controls.Add(this.txtNotes);
            this.Controls.Add(this.btnNotes);
            this.Controls.Add(this.btnSaveNotes);
            this.Name = "AddressTab";
            this.Size = new System.Drawing.Size(470, 174);
            this.Load += new System.EventHandler(this.AddressTab_Load);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        // Address Standardization CR2019 - 025
        private void cmbRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedRegion = cmbRegion.SelectedValue as string;
            var selectedVillage = cmbVillage.SelectedValue as string;
            if (!string.IsNullOrEmpty(selectedRegion) && !string.IsNullOrEmpty(selectedVillage))
            {
                string error = string.Empty;
                var postCode = ((MainForm)formRoot).CollectionsManager.GetZipCode(selectedRegion, selectedVillage, out error);
                if (error.Length > 0)
                {
                    ((MainForm)formRoot).ShowError(error);
                }

                txtPostCode.Text = postCode;
            }
        }

        private void cmbVillage_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedVillage = cmbVillage.SelectedValue as string;
            if (!string.IsNullOrEmpty(selectedVillage))
            {
                string error = string.Empty;
                DataSet dsRegion = ((MainForm)formRoot).CollectionsManager.GetRegions(selectedVillage, out error);
                if (error.Length > 0)
                {
                    ((MainForm)formRoot).ShowError(error);
                }
                DataRow intialRow = dsRegion.Tables[0].NewRow();
                intialRow["Region"] = "--Select Region--";
                dsRegion.Tables[0].Rows.InsertAt(intialRow, 0);
                cmbRegion.DataSource = dsRegion.Tables[0];
                cmbRegion.DisplayMember = "Region";
                cmbRegion.ValueMember = "Region";

                if (dsRegion.Tables[0].Rows.Count == 2)
                    cmbRegion.SelectedIndex = cmbRegion.FindStringExact((string)dsRegion.Tables[0].Rows[1]["Region"]);
                else if (dsRegion.Tables[0].Rows.Count > 2 || dsRegion.Tables[0].Rows.Count == 1)
                    cmbRegion.SelectedIndex = -1;
            }
        }
        // Address Standardization CR2019 - 025
        #endregion

        private void AddressTab_Load(object sender, System.EventArgs e)
        {
            // Do not do attempt this in design view
            if (this.DesignMode) return;
            this.LoadDeliveryArea();
            this.SetDeliveryArea(this._deliveryArea);
            if (_SimpleAddress == false)       //CR1084
            {
                if (Convert.ToBoolean(((MainForm)formRoot).Country[CountryParameterNames.ZoneAddresses]))           //CR1084 jec 
                {
                    drpZone.Visible = true;
                }
                else
                {
                    drpZone.Visible = false;
                    lbZone.Visible = false;
                }
            }

            toolTipDelArea.SetToolTip(btnDelAreas, "Delivery Area Descriptions");                //#14796
        }

        private void LoadDeliveryArea()
        {
            // Get the static data for the drop down lists
            CommonForm form = new CommonForm();
            DataSet areaSet = form.SetDataManager.GetSetsForTNameBranch(TN.TNameDeliveryArea, Config.BranchCode, out _error);
            DataTable areaTable = areaSet.Tables[TN.SetsData];
            delAreas = areaSet.Tables[TN.SetsData];       //#12224 - CR12249

            if (_error.Length > 0)
                form.ShowError(_error);
            else
            {
                StringCollection areaList = new StringCollection();
                //areaList.Add(CommonForm.GetResource("L_ALL"));
                areaList.Add(" ");
                foreach (DataRow row in areaTable.Rows)
                {
                    areaList.Add((string)row.ItemArray[0]);
                }
                drpDeliveryArea.DataSource = areaList;// this line displays delivery area on screen so dont delete it
            }
        }

        public void SetDeliveryArea(string deliveryArea)
        {
            // This will set the Delivery Area drop down if the Address Tab
            // has loaded. If not then the AddressTab_Load event will use
            // the private variable _deliveryArea to set the drop down.
            this._deliveryArea = deliveryArea;
            if (drpDeliveryArea.Items.Count == 0)           //#13548
            {
                this.LoadDeliveryArea();
            }

            int index = drpDeliveryArea.FindStringExact(deliveryArea);
            if (index > 0) drpDeliveryArea.SelectedIndex = index;

            drptitleC.DataSource = (DataTable)StaticData.Tables[TN.Title];
            drptitleC.DisplayMember = CN.CodeDescription;

        }
        // Address Standardization CR2019 - 025
        private void txtAddress1_Validating(object sender, CancelEventArgs e)
        {
            CommonForm form = null;
            if (!_readonly)
            {
                try
                {
                    form = new CommonForm();
                    form.Function = "txtAddress1_Validating";
                    if (string.IsNullOrEmpty(txtAddress1.Text))
                    {
                        errorProvider1.SetError(txtAddress1, CommonForm.GetResource("M_MANDATORYFIELD", new object[] { txtAddress1.Watermark }));
                        _valid = false;
                    }
                    else
                    {
                        errorProvider1.SetError(txtAddress1, string.Empty);
                    }
                }
                catch (Exception ex)
                {
                    form.Catch(ex, form.Function);
                }
            }
        }
        private void cmbRegion_Validating(object sender, CancelEventArgs e)
        {
            CommonForm form = null;
            if (!_readonly)
            {
                try
                {
                    form = new CommonForm();
                    form.Function = "cmbRegion_Validating";
                    if (cmbRegion.SelectedIndex == -1 || cmbRegion.SelectedIndex == 0)
                    {
                        errorProvider1.SetError(cmbRegion, CommonForm.GetResource("M_MANDATORYFIELD", new object[] { cmbRegion.Watermark }));
                        _valid = false;
                    }
                    else
                    {
                        errorProvider1.SetError(cmbRegion, string.Empty);
                    }
                }
                catch (Exception ex)
                {
                    form.Catch(ex, form.Function);
                }
            }
        }
        private void cmbVillage_Validating(object sender, CancelEventArgs e)
        {
            CommonForm form = null;
            if (!_readonly)
            {
                try
                {
                    form = new CommonForm();
                    form.Function = "cmbVillage_Validating";
                    if (cmbVillage.SelectedIndex == -1 || cmbVillage.SelectedIndex == 0)
                    {
                        errorProvider1.SetError(cmbVillage, CommonForm.GetResource("M_MANDATORYFIELD", new object[] { cmbVillage.Watermark }));
                        _valid = false;
                    }
                    else
                    {
                        errorProvider1.SetError(cmbVillage, string.Empty);
                    }
                }
                catch (Exception ex)
                {
                    form.Catch(ex, form.Function);
                }
            }
        }
        private void txtCoordinate_Validating(object sender, CancelEventArgs e)
        {
            CommonForm form = null;
            if (!_readonly)
            {
                try
                {
                    form = new CommonForm();
                    form.Function = "txtCoordinate_Validating";
                    if (string.IsNullOrEmpty(txtCoordinate.Text))
                    {
                        errorProvider1.SetError(txtCoordinate, string.Empty);
                    }
                    else
                    {
                        var coordinate = txtCoordinate.Text.Split(',');
                        if (coordinate.Length != 2) // // Address Standardization CR2019 - 025// To control without Comma value.
                        {
                            errorProvider1.SetError(txtCoordinate, CommonForm.GetResource("M_INVALIDCOORDINATE"));
                            _valid = false;
                        }
                        else
                        {
                            double latitude, longitude;

                            if (double.TryParse(coordinate[0], out latitude) && double.TryParse(coordinate[1], out longitude))
                            {
                                errorProvider1.SetError(txtCoordinate, string.Empty);
                            }
                            else
                            {
                                errorProvider1.SetError(txtCoordinate, CommonForm.GetResource("M_INVALIDCOORDINATE"));
                                _valid = false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    form.Catch(ex, form.Function);
                }
            }
        }
        // Address Standardization CR2019 - 025
        private void txtPostCode_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CommonForm form = null;
            string Function = "txtPostCode_Validating";
            string Error = "";

            if (!_readonly && Config.CountryCode == "S")
            {
                try
                {
                    //need to create a common form object to access the common functionality
                    //because we're not inheriting it in this class
                    form = new CommonForm();
                    form.Wait();

                    WStaticDataManager po = new WStaticDataManager(true);
                    DataSet ds = po.PostCodeLookUp(txtPostCode.Text, out Error);
                    if (Error.Length > 0)
                    {
                        form.ShowError(Error);
                    }
                    else
                    {
                        if (ds != null)
                        {
                            DataRow row = ds.Tables["Address"].Rows[0];

                            if (txtAddress1.Text.Length == 0)
                                txtAddress1.Text = (string)row["Address1"];
                            if (cmbVillage.Text.Length == 0) // Address Standardization CR2019 - 025
                                cmbVillage.Text = (string)row["Address2"];
                            if (cmbRegion.Text.Length == 0) // Address Standardization CR2019 - 025
                                cmbRegion.Text = (string)row["Address3"];
                            if (CFirstname.Text.Length == 0)
                                CFirstname.Text = (string)row["DELFirstname"];
                            if (CLastname.Text.Length == 0)
                                CLastname.Text = (string)row["DELLastname"];
                            if (drptitleC.Text.Length == 0)
                                drptitleC.Text = (string)row["DELTitleC"];
                        }
                    }
                }
                catch (Exception ex)
                {
                    form.Catch(ex, Function);
                }
                finally
                {
                    form.StopWait();
                }
            }
        }

        private void txtDialCode_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CommonForm form = null;
            if (!_readonly)
            {
                try
                {
                    form = new CommonForm();
                    form.Function = "txtDialCode_Validating";
                    if (!form.IsNumeric(txtDialCode.Text))
                    {
                        errorProvider1.SetError(txtDialCode, CommonForm.GetResource("M_NONNUMERIC"));
                        _valid = false;
                    }
                    else
                    {
                        errorProvider1.SetError(txtDialCode, "");
                    }
                }
                catch (Exception ex)
                {
                    form.Catch(ex, form.Function);
                }
            }
        }

        private void txtPhoneNo_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CommonForm form = null;
            if (!_readonly)
            {
                try
                {
                    form = new CommonForm();
                    form.Function = "txtPhoneNo_Validating";
                    if (string.IsNullOrEmpty(txtPhoneNo.Text)) // Address Standardization CR2019 - 025
                    {
                        errorProvider1.SetError(txtPhoneNo, CommonForm.GetResource("M_MANDATORYFIELD", new object[] { lPhoneNo.Text }));
                        _valid = false;
                    }
                    else if (!form.IsNumeric(txtPhoneNo.Text))
                    {
                        errorProvider1.SetError(txtPhoneNo, CommonForm.GetResource("M_NONNUMERIC"));
                        _valid = false;
                    }
                    else
                    {
                        errorProvider1.SetError(txtPhoneNo, "");
                    }
                }
                catch (Exception ex)
                {
                    form.Catch(ex, form.Function);
                }
            }
        }

        private void txtMobile_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CommonForm form = null;
            if (!_readonly)
            {
                try
                {
                    form = new CommonForm();
                    form.Function = "txtMobile_Validating";
                    if (string.IsNullOrEmpty(txtMobile.Text)) // Address Standardization CR2019 - 025
                    {
                        errorProvider1.SetError(txtMobile, CommonForm.GetResource("M_MANDATORYFIELD", new object[] { lMobile.Text }));
                        _valid = false;
                    }
                    else if(!form.IsNumeric(txtMobile.Text))
                    {
                        errorProvider1.SetError(txtMobile, CommonForm.GetResource("M_NONNUMERIC"));
                        _valid = false;
                    }
                    else
                    {
                        errorProvider1.SetError(txtMobile, "");
                    }
                }
                catch (Exception ex)
                {
                    form.Catch(ex, form.Function);
                }
            }
        }

        public bool IsValid()
        {
            _valid = true;
            if (!this.ReadOnly)
            {
                CommonForm form = new CommonForm();
                this.txtAddress1_Validating(this, new CancelEventArgs()); // Address Standardization CR2019 - 025
                this.cmbVillage_Validating(this, new CancelEventArgs()); // Address Standardization CR2019 - 025
                this.cmbRegion_Validating(this, new CancelEventArgs()); // Address Standardization CR2019 - 025
                this.txtCoordinate_Validating(this, new CancelEventArgs()); // Address Standardization CR2019 - 025
                this.txtDialCode_Validating(this, new System.ComponentModel.CancelEventArgs());
                this.txtPhoneNo_Validating(this, new System.ComponentModel.CancelEventArgs());
                if (txtMobile.Enabled) // Address Standardization CR2019 - 025
                    this.txtMobile_Validating(this, new System.ComponentModel.CancelEventArgs());
                this.dtDateIn_Validating(this, new System.ComponentModel.CancelEventArgs());
                if (Convert.ToBoolean(((MainForm)formRoot).Country[CountryParameterNames.ZoneAddresses]))           //CR1084 jec 
                {
                    this.drpZone_Validating2(this, new System.ComponentModel.CancelEventArgs());     //CR1084
                }
                this.drpDeliveryArea_Validating2(this, new System.ComponentModel.CancelEventArgs());     // #12226 jec        
            }
            return _valid;
        }

        /*hide all fields except the notes*/
        private void btnNotes_Click(object sender, System.EventArgs e)
        {
            this.btnNotes.Visible = false;
            this.txtNotes.Visible = true;
            this.txtNotes.BringToFront();
            this.btnSaveNotes.Visible = true;
            this.txtPhoneNo.Visible = false;
            this.txtPhoneNo.Visible = false;
            this.txtExtension.Visible = false;
            this.txtEmail.Visible = false;
            this.txtMobile.Visible = false;
            btnMobile.Visible = false;
        }
        /* display normal fields*/
        private void btnSaveNotes_Click(object sender, System.EventArgs e)
        {
            this.btnNotes.Visible = true;
            this.txtNotes.Visible = false;
            this.btnSaveNotes.Visible = false;
            this.txtPhoneNo.Visible = true;
            this.txtExtension.Visible = true;
            this.txtEmail.Visible = true;
            this.txtMobile.Visible = true;
            btnMobile.Visible = true;
        }

        private void dtDateIn_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //bool status = false;
            CommonForm cf = new CommonForm();
            if (!_readonly)
            {
                try
                {
                    cf.Function = "ValidateDate()";

                    if (dtDateIn.Value > DateTime.Today)
                    {
                        errorProvider1.SetError(dtDateIn, CommonForm.GetResource("M_INVALIDDATE"));
                        _valid = false;
                    }
                    else
                    {
                        errorProvider1.SetError(dtDateIn, "");
                    }
                }
                catch (Exception ex)
                {
                    cf.Catch(ex, cf.Function);
                }
            }
        }

        
        private void CFirstName_Enter(object sender, System.EventArgs e)
        {
            if (CFirstname.Text == "DELFirstname")
            {
                CFirstname.Text = "";
                //CLastname.Text = "";
                //TitleC.Text = "";

            }
        }
        private void CLastname_Enter(object sender, System.EventArgs e)
        {
            if (CLastname.Text == "DELLastname")
            {
                //CFirstname.Text = "";
                CLastname.Text = "";
                // TitleC.Text = "";

            }
        }


        private void btnMobile_Click(object sender, EventArgs e)
        {

            MobilePhoneEntryPopup pu = new MobilePhoneEntryPopup(this._readonly);

            pu.Mobile1 = this.txtMobile2.Text;
            pu.Mobile2 = this.txtMobile3.Text;
            pu.Mobile3 = this.txtMobile4.Text;

            pu.ShowDialog();

            if (pu.OK)
            {
                this._mobile2.Text = pu.Mobile1;
                this._mobile3.Text = pu.Mobile2;
                this._mobile4.Text = pu.Mobile3;
            }
        }

        // Address Standardization CR2019 - 025
        public void PopulateVillages()
        {
            if (StaticData.Tables[TN.Villages] == null) return;

            DataTable dtVillage = (DataTable)StaticData.Tables[TN.Villages];

            cmbVillage.DataSource = dtVillage;
            cmbVillage.DisplayMember = "Village";
            cmbVillage.ValueMember = "Village";
            cmbVillage.SelectedIndex = -1;
        }
        // Address Standardization CR2019 - 025
        public void PopulateZones()
        {
            string error = "";
            DataSet dsZone = ((MainForm)formRoot).CollectionsManager.GetZones(out error);
            if (error.Length > 0)
            {
                ((MainForm)formRoot).ShowError(error);
            }

            drpZone.DataSource = dsZone.Tables[0];
            dsZone.Tables[0].Rows.Add("");
            drpZone.DisplayMember = "concatDesc";
            drpZone.ValueMember = "concatDesc";

        }

        //private void drpZone_SelectedIndexChanged(object sender, EventArgs e)
        //{

        //}

        public void SetZone(string zone)
        {
            // This will set the Zone drop down if the Address Tab
            // has loaded. If not then the AddressTab_Load event will use
            // the private variable _zone to set the drop down.
            this._zone = zone;
            int index = drpZone.FindStringExact(zone);
            if (index >= 0) drpZone.SelectedIndex = index;
        }

        //private void drpZone_Validating(object sender, CancelEventArgs e)
        //{

        //}


        private void txtPostCode_TextChanged(object sender, EventArgs e)
        {
            validatepostcode();
        }

        public void validatepostcode()
        {
            if (ValidatePostcode && Config.CountryCode == "Y") // For Malaysia only
            {
                int number;
                Int32.TryParse(txtPostCode.Text, out number);

                if (number > 9999)
                {
                    errorProvider1.SetError(txtPostCode, "");
                }
                else
                {
                    errorProvider1.SetError(txtPostCode, "This postcode is not a valid Malaysian Postcode");
                }
            }
        }

        // not using drpZone_Validating as doesn't save if zone changed before save button clicked  - jec CR1084
        private void drpZone_Validating2(object sender, CancelEventArgs e)
        {
            CommonForm cf = new CommonForm();
            if (!_readonly)
            {
                try
                {
                    cf.Function = "Validate Zone";

                    //if (drpZone.SelectedText == "" || drpZone.SelectedText == String.Empty || drpZone.SelectedIndex==3)
                    if (drpZone.SelectedIndex == drpZone.Items.Count - 1)         //UAT62 - last item is blank
                    {
                        errorProvider1.SetError(drpZone, CommonForm.GetResource("M_MANDATORYZONE"));
                        _valid = false;
                    }
                    else
                    {
                        errorProvider1.SetError(drpZone, "");
                    }
                }
                catch (Exception ex)
                {
                    cf.Catch(ex, cf.Function);
                }
            }
        }

        // #12226 - Delivery Area is Mandatory
        private void drpDeliveryArea_Validating2(object sender, CancelEventArgs e)
        {
            CommonForm cf = new CommonForm();
            if (!_readonly)
            {
                try
                {
                    cf.Function = "Validate Delivery Area";

                    //if (drpDeliveryArea.SelectedIndex == 0)         
                    if (drpDeliveryArea.SelectedIndex == 0 || drpDeliveryArea.SelectedIndex == -1)    //#13459
                    {
                        errorProvider1.SetError(drpDeliveryArea, CommonForm.GetResource("M_MANDATORYDELAREA"));
                        _valid = false;
                    }
                    else
                    {
                        errorProvider1.SetError(drpDeliveryArea, "");
                    }
                }
                catch (Exception ex)
                {
                    cf.Catch(ex, cf.Function);
                }
            }

        }

        //IP - 16/03/11 - #3317 - CR1245
        private void btnWork_Click(object sender, EventArgs e)
        {
            WorkPhoneEntryPopup w = new WorkPhoneEntryPopup(this._readonly);

            w.WorkDialCode2 = this.txtWorkDialCode2.Text;
            w.WorkNum2 = this.txtWorkNum2.Text;
            w.WorkExt2 = this.txtWorkExt2.Text;

            w.WorkDialCode3 = this.txtWorkDialCode3.Text;
            w.WorkNum3 = this.txtWorkNum3.Text;
            w.WorkExt3 = this.txtWorkExt3.Text;

            w.WorkDialCode4 = this.txtWorkDialCode4.Text;
            w.WorkNum4 = this.txtWorkNum4.Text;
            w.WorkExt4 = this.txtWorkExt4.Text;

            w.ShowDialog();

            if (w.OK)
            {
                this._workDialCode2.Text = w.WorkDialCode2;
                this._workNum2.Text = w.WorkNum2;
                this._workExt2.Text = w.WorkExt2;

                this._workDialCode3.Text = w.WorkDialCode3;
                this._workNum3.Text = w.WorkNum3;
                this._workExt3.Text = w.WorkExt3;

                this._workDialCode4.Text = w.WorkDialCode4;
                this._workNum4.Text = w.WorkNum4;
                this._workExt4.Text = w.WorkExt4;
            }
        }

        private void dtDateIn_ValueChanged(object sender, EventArgs e)
        {
            if (onChangeAddressDate != null)
                onChangeAddressDate(this, new GenericEventHandler<DateTime>(dtDateIn.Value));
        }

        //#13548
        private void drpDeliveryArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (drpDeliveryArea.Text.Trim() != "")
                SetDeliveryArea(drpDeliveryArea.Text);

        }


        //#12224 - CR12249
        private void btnDelAreas_Click(object sender, EventArgs e)
        {
            DeliveryAreaPopup delArea = new DeliveryAreaPopup(this.formRoot, this.ParentForm, this.delAreas, drpDeliveryArea.Text);
            delArea.ShowDialog();

            if (delArea.SelectedDeliveryArea != string.Empty)
            {
                var index = drpDeliveryArea.FindStringExact(delArea.SelectedDeliveryArea);
                if (index > 0) drpDeliveryArea.SelectedIndex = index;
            }
        }

        private void drptitleC_SelectedIndexChanged(object sender, EventArgs e)
        {



        }

        //private void at_onChangeAddressDate(object sender, GenericEventHandler<DateTime> Handler)
        //{
        //    if (dtDateInCurrentAddress1.Value != Handler.Results)
        //        dtDateInCurrentAddress1.Value = Handler.Results;
        //}
    }
}
