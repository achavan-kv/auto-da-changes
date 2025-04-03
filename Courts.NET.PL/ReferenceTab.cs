using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Xml;
using STL.Common;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using STL.Common.Static;
using STL.PL.WS5;


namespace STL.PL
{
    /// <summary>
    /// Details tab for a referee in a credit application. A new instance of this
    /// tab is created for each referee in a credit application. The number of 
    /// referees and therefore tabs will vary by application.
    /// </summary>
    public class ReferenceTab : CommonUserControl
    {
        private int _empeeNo;
        public int empeeNo
        {
            get { return _empeeNo; }
            set { _empeeNo = value; }
        }

        //private bool valid;
        private CommonForm cForm = null;

        private System.Windows.Forms.ErrorProvider errorProvider1;
        //	private System.ComponentModel.IContainer components;
        private System.Windows.Forms.GroupBox gbNotes;
        private System.Windows.Forms.Label lComments;
        private System.Windows.Forms.Label lHomeDirections;
        private System.Windows.Forms.Button btnNotesClose;
        private System.Windows.Forms.GroupBox gbDetails;
        private System.Windows.Forms.Button btnNotes;
        private System.Windows.Forms.Label lWorkPostCode;
        private System.Windows.Forms.Label lWorkAddress;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lMobile;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lWorkTel;
        public System.Windows.Forms.CheckBox ChxReference;
        private System.Windows.Forms.Label lYearsKnown;
        public System.Windows.Forms.TextBox txtYearsKnown;
        private System.Windows.Forms.Label lDateChecked;
        private System.Windows.Forms.Label lCheckedBy;
        public System.Windows.Forms.TextBox txtDateChecked;
        public System.Windows.Forms.TextBox txtCheckedBy;
        private System.Windows.Forms.Label lRelation;
        public System.Windows.Forms.ComboBox drpRelation;
        private System.Windows.Forms.Label lHomePostCode;
        private System.Windows.Forms.Label lHomeAddress;
        public System.Windows.Forms.TextBox txtLastName;
        private System.Windows.Forms.Label lLastName;
        public System.Windows.Forms.TextBox txtFirstName;
        private System.Windows.Forms.Label lFirstName;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.TextBox txtPostCode;
        public System.Windows.Forms.TextBox txtAddress3;
        public System.Windows.Forms.TextBox txtAddress2;
        public System.Windows.Forms.TextBox txtAddress1;
        private System.Windows.Forms.Label lHomeTel;
        public System.Windows.Forms.Label lCheckAllowed;
        public System.Windows.Forms.TextBox txtWPostCode;
        public System.Windows.Forms.TextBox txtWAddress3;
        public System.Windows.Forms.TextBox txtWAddress2;
        public System.Windows.Forms.TextBox txtWAddress1;
        public System.Windows.Forms.TextBox txtDirections;
        public System.Windows.Forms.TextBox txtComment;
        public STL.PL.PhoneNumberBox txtDialCode;
        public STL.PL.PhoneNumberBox txtPhoneNo;
        public STL.PL.PhoneNumberBox txtWPhoneNo;
        public STL.PL.PhoneNumberBox txtWDialCode;
        public STL.PL.PhoneNumberBox txtMPhoneNo;
        public STL.PL.PhoneNumberBox txtMDialCode;
        public System.Windows.Forms.TextBox txtNewComment;
        private IContainer components;

        private Crownwood.Magic.Controls.TabPage _tp = null;

        public ReferenceTab(TranslationDummy d)
        {
            InitializeComponent();
        }

        public ReferenceTab(Crownwood.Magic.Controls.TabPage tp)
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            cForm = new CommonForm();
            cForm.Name = "ReferenceTab";

            HashMenus();
            cForm.ApplyRoleRestrictions();

            // Init the Relationship Combo Box
            LoadStatic();
            TranslateControls();
            _tp = tp;

            // Resize the form and hide the notes fields
            this.Height = this.gbDetails.Height = this.gbNotes.Height = 220;
            this.gbDetails.Width = this.gbNotes.Width = this.Width;
            this.gbNotes.Location = this.gbDetails.Location;
            this.btnNotesClose.Location = this.btnNotes.Location;
            this.gbNotes.Visible = false;
            this.gbNotes.Enabled = false;
            this.gbNotes.SendToBack();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            //if( disposing )
            //{
            //    if(components != null)
            //    {
            //        components.Dispose();
            //    }
            //}
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReferenceTab));
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.gbNotes = new System.Windows.Forms.GroupBox();
            this.txtNewComment = new System.Windows.Forms.TextBox();
            this.btnNotesClose = new System.Windows.Forms.Button();
            this.lComments = new System.Windows.Forms.Label();
            this.lHomeDirections = new System.Windows.Forms.Label();
            this.txtDirections = new System.Windows.Forms.TextBox();
            this.txtComment = new System.Windows.Forms.TextBox();
            this.gbDetails = new System.Windows.Forms.GroupBox();
            this.txtMPhoneNo = new STL.PL.PhoneNumberBox();
            this.txtMDialCode = new STL.PL.PhoneNumberBox();
            this.txtWPhoneNo = new STL.PL.PhoneNumberBox();
            this.txtWDialCode = new STL.PL.PhoneNumberBox();
            this.txtPhoneNo = new STL.PL.PhoneNumberBox();
            this.txtDialCode = new STL.PL.PhoneNumberBox();
            this.btnNotes = new System.Windows.Forms.Button();
            this.lWorkPostCode = new System.Windows.Forms.Label();
            this.lWorkAddress = new System.Windows.Forms.Label();
            this.txtWPostCode = new System.Windows.Forms.TextBox();
            this.txtWAddress3 = new System.Windows.Forms.TextBox();
            this.txtWAddress2 = new System.Windows.Forms.TextBox();
            this.txtWAddress1 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.lMobile = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lWorkTel = new System.Windows.Forms.Label();
            this.ChxReference = new System.Windows.Forms.CheckBox();
            this.lYearsKnown = new System.Windows.Forms.Label();
            this.txtYearsKnown = new System.Windows.Forms.TextBox();
            this.lDateChecked = new System.Windows.Forms.Label();
            this.lCheckedBy = new System.Windows.Forms.Label();
            this.txtDateChecked = new System.Windows.Forms.TextBox();
            this.txtCheckedBy = new System.Windows.Forms.TextBox();
            this.lRelation = new System.Windows.Forms.Label();
            this.drpRelation = new System.Windows.Forms.ComboBox();
            this.lHomePostCode = new System.Windows.Forms.Label();
            this.lHomeAddress = new System.Windows.Forms.Label();
            this.txtLastName = new System.Windows.Forms.TextBox();
            this.lLastName = new System.Windows.Forms.Label();
            this.txtFirstName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtPostCode = new System.Windows.Forms.TextBox();
            this.txtAddress3 = new System.Windows.Forms.TextBox();
            this.txtAddress2 = new System.Windows.Forms.TextBox();
            this.txtAddress1 = new System.Windows.Forms.TextBox();
            this.lHomeTel = new System.Windows.Forms.Label();
            this.lCheckAllowed = new System.Windows.Forms.Label();
            this.lFirstName = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.gbNotes.SuspendLayout();
            this.gbDetails.SuspendLayout();
            this.SuspendLayout();
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // gbNotes
            // 
            this.gbNotes.Controls.Add(this.txtNewComment);
            this.gbNotes.Controls.Add(this.btnNotesClose);
            this.gbNotes.Controls.Add(this.lComments);
            this.gbNotes.Controls.Add(this.lHomeDirections);
            this.gbNotes.Controls.Add(this.txtDirections);
            this.gbNotes.Controls.Add(this.txtComment);
            this.gbNotes.Enabled = false;
            this.gbNotes.Location = new System.Drawing.Point(-2, 219);
            this.gbNotes.Name = "gbNotes";
            this.gbNotes.Size = new System.Drawing.Size(708, 214);
            this.gbNotes.TabIndex = 0;
            this.gbNotes.TabStop = false;
            this.gbNotes.Visible = false;
            // 
            // txtNewComment
            // 
            this.txtNewComment.Location = new System.Drawing.Point(16, 98);
            this.txtNewComment.MaxLength = 300;
            this.txtNewComment.Multiline = true;
            this.txtNewComment.Name = "txtNewComment";
            this.txtNewComment.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtNewComment.Size = new System.Drawing.Size(632, 49);
            this.txtNewComment.TabIndex = 711;
            // 
            // btnNotesClose
            // 
            this.btnNotesClose.Image = ((System.Drawing.Image)(resources.GetObject("btnNotesClose.Image")));
            this.btnNotesClose.Location = new System.Drawing.Point(656, 168);
            this.btnNotesClose.Name = "btnNotesClose";
            this.btnNotesClose.Size = new System.Drawing.Size(32, 32);
            this.btnNotesClose.TabIndex = 0;
            this.btnNotesClose.TabStop = false;
            this.btnNotesClose.Click += new System.EventHandler(this.btnNotesClose_Click);
            // 
            // lComments
            // 
            this.lComments.BackColor = System.Drawing.SystemColors.Control;
            this.lComments.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lComments.Location = new System.Drawing.Point(16, 84);
            this.lComments.Name = "lComments";
            this.lComments.Size = new System.Drawing.Size(64, 16);
            this.lComments.TabIndex = 86;
            this.lComments.Text = "Comments";
            this.lComments.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lHomeDirections
            // 
            this.lHomeDirections.BackColor = System.Drawing.SystemColors.Control;
            this.lHomeDirections.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lHomeDirections.Location = new System.Drawing.Point(16, 16);
            this.lHomeDirections.Name = "lHomeDirections";
            this.lHomeDirections.Size = new System.Drawing.Size(112, 16);
            this.lHomeDirections.TabIndex = 85;
            this.lHomeDirections.Text = "Home Directions";
            this.lHomeDirections.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtDirections
            // 
            this.txtDirections.Location = new System.Drawing.Point(16, 32);
            this.txtDirections.MaxLength = 300;
            this.txtDirections.Multiline = true;
            this.txtDirections.Name = "txtDirections";
            this.txtDirections.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDirections.Size = new System.Drawing.Size(632, 42);
            this.txtDirections.TabIndex = 700;
            // 
            // txtComment
            // 
            this.txtComment.Location = new System.Drawing.Point(16, 151);
            this.txtComment.MaxLength = 300;
            this.txtComment.Multiline = true;
            this.txtComment.Name = "txtComment";
            this.txtComment.ReadOnly = true;
            this.txtComment.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtComment.Size = new System.Drawing.Size(632, 49);
            this.txtComment.TabIndex = 710;
            // 
            // gbDetails
            // 
            this.gbDetails.Controls.Add(this.txtMPhoneNo);
            this.gbDetails.Controls.Add(this.txtMDialCode);
            this.gbDetails.Controls.Add(this.txtWPhoneNo);
            this.gbDetails.Controls.Add(this.txtWDialCode);
            this.gbDetails.Controls.Add(this.txtPhoneNo);
            this.gbDetails.Controls.Add(this.txtDialCode);
            this.gbDetails.Controls.Add(this.btnNotes);
            this.gbDetails.Controls.Add(this.lWorkPostCode);
            this.gbDetails.Controls.Add(this.lWorkAddress);
            this.gbDetails.Controls.Add(this.txtWPostCode);
            this.gbDetails.Controls.Add(this.txtWAddress3);
            this.gbDetails.Controls.Add(this.txtWAddress2);
            this.gbDetails.Controls.Add(this.txtWAddress1);
            this.gbDetails.Controls.Add(this.label6);
            this.gbDetails.Controls.Add(this.lMobile);
            this.gbDetails.Controls.Add(this.label3);
            this.gbDetails.Controls.Add(this.lWorkTel);
            this.gbDetails.Controls.Add(this.ChxReference);
            this.gbDetails.Controls.Add(this.lYearsKnown);
            this.gbDetails.Controls.Add(this.txtYearsKnown);
            this.gbDetails.Controls.Add(this.lDateChecked);
            this.gbDetails.Controls.Add(this.lCheckedBy);
            this.gbDetails.Controls.Add(this.txtDateChecked);
            this.gbDetails.Controls.Add(this.txtCheckedBy);
            this.gbDetails.Controls.Add(this.lRelation);
            this.gbDetails.Controls.Add(this.drpRelation);
            this.gbDetails.Controls.Add(this.lHomePostCode);
            this.gbDetails.Controls.Add(this.lHomeAddress);
            this.gbDetails.Controls.Add(this.txtLastName);
            this.gbDetails.Controls.Add(this.lLastName);
            this.gbDetails.Controls.Add(this.txtFirstName);
            this.gbDetails.Controls.Add(this.label5);
            this.gbDetails.Controls.Add(this.txtPostCode);
            this.gbDetails.Controls.Add(this.txtAddress3);
            this.gbDetails.Controls.Add(this.txtAddress2);
            this.gbDetails.Controls.Add(this.txtAddress1);
            this.gbDetails.Controls.Add(this.lHomeTel);
            this.gbDetails.Controls.Add(this.lCheckAllowed);
            this.gbDetails.Controls.Add(this.lFirstName);
            this.gbDetails.Location = new System.Drawing.Point(-2, -7);
            this.gbDetails.Name = "gbDetails";
            this.gbDetails.Size = new System.Drawing.Size(708, 210);
            this.gbDetails.TabIndex = 0;
            this.gbDetails.TabStop = false;
            // 
            // txtMPhoneNo
            // 
            this.txtMPhoneNo.Location = new System.Drawing.Point(596, 112);
            this.txtMPhoneNo.MaxLength = 13;
            this.txtMPhoneNo.Name = "txtMPhoneNo";
            this.txtMPhoneNo.Size = new System.Drawing.Size(80, 20);
            this.txtMPhoneNo.TabIndex = 676;
            // 
            // txtMDialCode
            // 
            this.txtMDialCode.Location = new System.Drawing.Point(556, 112);
            this.txtMDialCode.MaxLength = 8;
            this.txtMDialCode.Name = "txtMDialCode";
            this.txtMDialCode.Size = new System.Drawing.Size(32, 20);
            this.txtMDialCode.TabIndex = 675;
            // 
            // txtWPhoneNo
            // 
            this.txtWPhoneNo.Location = new System.Drawing.Point(596, 88);
            this.txtWPhoneNo.MaxLength = 13;
            this.txtWPhoneNo.Name = "txtWPhoneNo";
            this.txtWPhoneNo.Size = new System.Drawing.Size(80, 20);
            this.txtWPhoneNo.TabIndex = 674;
            // 
            // txtWDialCode
            // 
            this.txtWDialCode.Location = new System.Drawing.Point(556, 88);
            this.txtWDialCode.MaxLength = 8;
            this.txtWDialCode.Name = "txtWDialCode";
            this.txtWDialCode.Size = new System.Drawing.Size(32, 20);
            this.txtWDialCode.TabIndex = 673;
            // 
            // txtPhoneNo
            // 
            this.txtPhoneNo.Location = new System.Drawing.Point(596, 64);
            this.txtPhoneNo.MaxLength = 13;
            this.txtPhoneNo.Name = "txtPhoneNo";
            this.txtPhoneNo.Size = new System.Drawing.Size(80, 20);
            this.txtPhoneNo.TabIndex = 672;
            // 
            // txtDialCode
            // 
            this.txtDialCode.Location = new System.Drawing.Point(556, 64);
            this.txtDialCode.MaxLength = 8;
            this.txtDialCode.Name = "txtDialCode";
            this.txtDialCode.Size = new System.Drawing.Size(32, 20);
            this.txtDialCode.TabIndex = 671;
            // 
            // btnNotes
            // 
            this.btnNotes.Image = ((System.Drawing.Image)(resources.GetObject("btnNotes.Image")));
            this.btnNotes.Location = new System.Drawing.Point(653, 168);
            this.btnNotes.Name = "btnNotes";
            this.btnNotes.Size = new System.Drawing.Size(32, 32);
            this.btnNotes.TabIndex = 0;
            this.btnNotes.TabStop = false;
            this.btnNotes.Click += new System.EventHandler(this.btnNotes_Click);
            // 
            // lWorkPostCode
            // 
            this.lWorkPostCode.BackColor = System.Drawing.SystemColors.Control;
            this.lWorkPostCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lWorkPostCode.Location = new System.Drawing.Point(236, 126);
            this.lWorkPostCode.Name = "lWorkPostCode";
            this.lWorkPostCode.Size = new System.Drawing.Size(64, 16);
            this.lWorkPostCode.TabIndex = 116;
            this.lWorkPostCode.Text = "Post Code";
            this.lWorkPostCode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lWorkAddress
            // 
            this.lWorkAddress.BackColor = System.Drawing.SystemColors.Control;
            this.lWorkAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lWorkAddress.Location = new System.Drawing.Point(244, 64);
            this.lWorkAddress.Name = "lWorkAddress";
            this.lWorkAddress.Size = new System.Drawing.Size(56, 32);
            this.lWorkAddress.TabIndex = 115;
            this.lWorkAddress.Text = "Work Address";
            this.lWorkAddress.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtWPostCode
            // 
            this.txtWPostCode.BackColor = System.Drawing.SystemColors.Window;
            this.txtWPostCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtWPostCode.Location = new System.Drawing.Point(308, 124);
            this.txtWPostCode.MaxLength = 10;
            this.txtWPostCode.Name = "txtWPostCode";
            this.txtWPostCode.Size = new System.Drawing.Size(80, 20);
            this.txtWPostCode.TabIndex = 590;
            // 
            // txtWAddress3
            // 
            this.txtWAddress3.BackColor = System.Drawing.SystemColors.Window;
            this.txtWAddress3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtWAddress3.Location = new System.Drawing.Point(308, 104);
            this.txtWAddress3.MaxLength = 26;
            this.txtWAddress3.Name = "txtWAddress3";
            this.txtWAddress3.Size = new System.Drawing.Size(160, 20);
            this.txtWAddress3.TabIndex = 580;
            // 
            // txtWAddress2
            // 
            this.txtWAddress2.BackColor = System.Drawing.SystemColors.Window;
            this.txtWAddress2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtWAddress2.Location = new System.Drawing.Point(308, 84);
            this.txtWAddress2.MaxLength = 26;
            this.txtWAddress2.Name = "txtWAddress2";
            this.txtWAddress2.Size = new System.Drawing.Size(160, 20);
            this.txtWAddress2.TabIndex = 570;
            // 
            // txtWAddress1
            // 
            this.txtWAddress1.BackColor = System.Drawing.SystemColors.Window;
            this.txtWAddress1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtWAddress1.Location = new System.Drawing.Point(308, 64);
            this.txtWAddress1.MaxLength = 26;
            this.txtWAddress1.Name = "txtWAddress1";
            this.txtWAddress1.Size = new System.Drawing.Size(160, 20);
            this.txtWAddress1.TabIndex = 560;
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.SystemColors.Control;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(588, 112);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(8, 8);
            this.label6.TabIndex = 110;
            this.label6.Text = "-";
            // 
            // lMobile
            // 
            this.lMobile.BackColor = System.Drawing.SystemColors.Control;
            this.lMobile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lMobile.Location = new System.Drawing.Point(484, 112);
            this.lMobile.Name = "lMobile";
            this.lMobile.Size = new System.Drawing.Size(64, 16);
            this.lMobile.TabIndex = 109;
            this.lMobile.Text = "Mobile";
            this.lMobile.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.SystemColors.Control;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(588, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(8, 8);
            this.label3.TabIndex = 106;
            this.label3.Text = "-";
            // 
            // lWorkTel
            // 
            this.lWorkTel.BackColor = System.Drawing.SystemColors.Control;
            this.lWorkTel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lWorkTel.Location = new System.Drawing.Point(484, 88);
            this.lWorkTel.Name = "lWorkTel";
            this.lWorkTel.Size = new System.Drawing.Size(64, 16);
            this.lWorkTel.TabIndex = 105;
            this.lWorkTel.Text = "Work Tel";
            this.lWorkTel.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // ChxReference
            // 
            this.ChxReference.Enabled = false;
            this.ChxReference.Location = new System.Drawing.Point(14, 172);
            this.ChxReference.Name = "ChxReference";
            this.ChxReference.Size = new System.Drawing.Size(144, 24);
            this.ChxReference.TabIndex = 0;
            this.ChxReference.TabStop = false;
            this.ChxReference.Text = "Reference Checked";
            this.ChxReference.CheckedChanged += new System.EventHandler(this.ChxReference_CheckedChanged);
            // 
            // lYearsKnown
            // 
            this.lYearsKnown.BackColor = System.Drawing.SystemColors.Control;
            this.lYearsKnown.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lYearsKnown.Location = new System.Drawing.Point(476, 40);
            this.lYearsKnown.Name = "lYearsKnown";
            this.lYearsKnown.Size = new System.Drawing.Size(72, 16);
            this.lYearsKnown.TabIndex = 100;
            this.lYearsKnown.Text = "Years Known";
            this.lYearsKnown.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtYearsKnown
            // 
            this.txtYearsKnown.BackColor = System.Drawing.SystemColors.Window;
            this.txtYearsKnown.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtYearsKnown.Location = new System.Drawing.Point(556, 40);
            this.txtYearsKnown.MaxLength = 8;
            this.txtYearsKnown.Name = "txtYearsKnown";
            this.txtYearsKnown.Size = new System.Drawing.Size(32, 20);
            this.txtYearsKnown.TabIndex = 610;
            this.txtYearsKnown.Text = "0";
            this.txtYearsKnown.TextChanged += new System.EventHandler(this.txtYearsKnown_TextChanged);
            // 
            // lDateChecked
            // 
            this.lDateChecked.BackColor = System.Drawing.SystemColors.Control;
            this.lDateChecked.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lDateChecked.Location = new System.Drawing.Point(425, 176);
            this.lDateChecked.Name = "lDateChecked";
            this.lDateChecked.Size = new System.Drawing.Size(80, 16);
            this.lDateChecked.TabIndex = 98;
            this.lDateChecked.Text = "Date Checked";
            this.lDateChecked.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lCheckedBy
            // 
            this.lCheckedBy.BackColor = System.Drawing.SystemColors.Control;
            this.lCheckedBy.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lCheckedBy.Location = new System.Drawing.Point(154, 176);
            this.lCheckedBy.Name = "lCheckedBy";
            this.lCheckedBy.Size = new System.Drawing.Size(64, 16);
            this.lCheckedBy.TabIndex = 97;
            this.lCheckedBy.Text = "Checked By";
            this.lCheckedBy.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtDateChecked
            // 
            this.txtDateChecked.BackColor = System.Drawing.SystemColors.Window;
            this.txtDateChecked.Enabled = false;
            this.txtDateChecked.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDateChecked.Location = new System.Drawing.Point(514, 174);
            this.txtDateChecked.MaxLength = 26;
            this.txtDateChecked.Name = "txtDateChecked";
            this.txtDateChecked.ReadOnly = true;
            this.txtDateChecked.Size = new System.Drawing.Size(112, 20);
            this.txtDateChecked.TabIndex = 0;
            this.txtDateChecked.TabStop = false;
            // 
            // txtCheckedBy
            // 
            this.txtCheckedBy.BackColor = System.Drawing.SystemColors.Window;
            this.txtCheckedBy.Enabled = false;
            this.txtCheckedBy.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCheckedBy.Location = new System.Drawing.Point(226, 174);
            this.txtCheckedBy.MaxLength = 26;
            this.txtCheckedBy.Name = "txtCheckedBy";
            this.txtCheckedBy.ReadOnly = true;
            this.txtCheckedBy.Size = new System.Drawing.Size(192, 20);
            this.txtCheckedBy.TabIndex = 0;
            this.txtCheckedBy.TabStop = false;
            // 
            // lRelation
            // 
            this.lRelation.BackColor = System.Drawing.SystemColors.Control;
            this.lRelation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lRelation.Location = new System.Drawing.Point(484, 16);
            this.lRelation.Name = "lRelation";
            this.lRelation.Size = new System.Drawing.Size(68, 16);
            this.lRelation.TabIndex = 94;
            this.lRelation.Text = "Relationship";
            // 
            // drpRelation
            // 
            this.drpRelation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpRelation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.drpRelation.ItemHeight = 13;
            this.drpRelation.Location = new System.Drawing.Point(556, 16);
            this.drpRelation.Name = "drpRelation";
            this.drpRelation.Size = new System.Drawing.Size(121, 21);
            this.drpRelation.TabIndex = 600;
            this.drpRelation.SelectedIndexChanged += new System.EventHandler(this.drpRelation_SelectedIndexChanged);
            // 
            // lHomePostCode
            // 
            this.lHomePostCode.BackColor = System.Drawing.SystemColors.Control;
            this.lHomePostCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lHomePostCode.Location = new System.Drawing.Point(1, 126);
            this.lHomePostCode.Name = "lHomePostCode";
            this.lHomePostCode.Size = new System.Drawing.Size(64, 16);
            this.lHomePostCode.TabIndex = 93;
            this.lHomePostCode.Text = "Post Code";
            this.lHomePostCode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lHomeAddress
            // 
            this.lHomeAddress.BackColor = System.Drawing.SystemColors.Control;
            this.lHomeAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lHomeAddress.Location = new System.Drawing.Point(9, 64);
            this.lHomeAddress.Name = "lHomeAddress";
            this.lHomeAddress.Size = new System.Drawing.Size(56, 32);
            this.lHomeAddress.TabIndex = 92;
            this.lHomeAddress.Text = "Home Address";
            this.lHomeAddress.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtLastName
            // 
            this.txtLastName.BackColor = System.Drawing.SystemColors.Window;
            this.txtLastName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLastName.Location = new System.Drawing.Point(308, 16);
            this.txtLastName.MaxLength = 35;
            this.txtLastName.Name = "txtLastName";
            this.txtLastName.Size = new System.Drawing.Size(161, 20);
            this.txtLastName.TabIndex = 510;
            // 
            // lLastName
            // 
            this.lLastName.BackColor = System.Drawing.SystemColors.Control;
            this.lLastName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lLastName.Location = new System.Drawing.Point(236, 16);
            this.lLastName.Name = "lLastName";
            this.lLastName.Size = new System.Drawing.Size(64, 16);
            this.lLastName.TabIndex = 91;
            this.lLastName.Text = "Last name";
            this.lLastName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtFirstName
            // 
            this.txtFirstName.BackColor = System.Drawing.SystemColors.Window;
            this.txtFirstName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFirstName.Location = new System.Drawing.Point(73, 16);
            this.txtFirstName.MaxLength = 30;
            this.txtFirstName.Name = "txtFirstName";
            this.txtFirstName.Size = new System.Drawing.Size(158, 20);
            this.txtFirstName.TabIndex = 500;
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.SystemColors.Control;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(588, 64);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(8, 8);
            this.label5.TabIndex = 89;
            this.label5.Text = "-";
            // 
            // txtPostCode
            // 
            this.txtPostCode.BackColor = System.Drawing.SystemColors.Window;
            this.txtPostCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPostCode.Location = new System.Drawing.Point(73, 124);
            this.txtPostCode.MaxLength = 10;
            this.txtPostCode.Name = "txtPostCode";
            this.txtPostCode.Size = new System.Drawing.Size(80, 20);
            this.txtPostCode.TabIndex = 550;
            // 
            // txtAddress3
            // 
            this.txtAddress3.BackColor = System.Drawing.SystemColors.Window;
            this.txtAddress3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAddress3.Location = new System.Drawing.Point(73, 104);
            this.txtAddress3.MaxLength = 26;
            this.txtAddress3.Name = "txtAddress3";
            this.txtAddress3.Size = new System.Drawing.Size(160, 20);
            this.txtAddress3.TabIndex = 540;
            // 
            // txtAddress2
            // 
            this.txtAddress2.BackColor = System.Drawing.SystemColors.Window;
            this.txtAddress2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAddress2.Location = new System.Drawing.Point(73, 84);
            this.txtAddress2.MaxLength = 26;
            this.txtAddress2.Name = "txtAddress2";
            this.txtAddress2.Size = new System.Drawing.Size(160, 20);
            this.txtAddress2.TabIndex = 530;
            // 
            // txtAddress1
            // 
            this.txtAddress1.BackColor = System.Drawing.SystemColors.Window;
            this.txtAddress1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAddress1.Location = new System.Drawing.Point(73, 64);
            this.txtAddress1.MaxLength = 26;
            this.txtAddress1.Name = "txtAddress1";
            this.txtAddress1.Size = new System.Drawing.Size(160, 20);
            this.txtAddress1.TabIndex = 520;
            // 
            // lHomeTel
            // 
            this.lHomeTel.BackColor = System.Drawing.SystemColors.Control;
            this.lHomeTel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lHomeTel.Location = new System.Drawing.Point(484, 64);
            this.lHomeTel.Name = "lHomeTel";
            this.lHomeTel.Size = new System.Drawing.Size(64, 16);
            this.lHomeTel.TabIndex = 88;
            this.lHomeTel.Text = "Home Tel";
            this.lHomeTel.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // lCheckAllowed
            // 
            this.lCheckAllowed.BackColor = System.Drawing.SystemColors.Control;
            this.lCheckAllowed.Enabled = false;
            this.lCheckAllowed.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lCheckAllowed.Location = new System.Drawing.Point(118, 176);
            this.lCheckAllowed.Name = "lCheckAllowed";
            this.lCheckAllowed.Size = new System.Drawing.Size(40, 16);
            this.lCheckAllowed.TabIndex = 102;
            this.lCheckAllowed.Visible = false;
            // 
            // lFirstName
            // 
            this.lFirstName.BackColor = System.Drawing.SystemColors.Control;
            this.lFirstName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lFirstName.Location = new System.Drawing.Point(1, 16);
            this.lFirstName.Name = "lFirstName";
            this.lFirstName.Size = new System.Drawing.Size(64, 16);
            this.lFirstName.TabIndex = 90;
            this.lFirstName.Text = "First name";
            this.lFirstName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ReferenceTab
            // 
            this.Controls.Add(this.gbDetails);
            this.Controls.Add(this.gbNotes);
            this.Name = "ReferenceTab";
            this.Size = new System.Drawing.Size(704, 437);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.gbNotes.ResumeLayout(false);
            this.gbNotes.PerformLayout();
            this.gbDetails.ResumeLayout(false);
            this.gbDetails.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        private void HashMenus()
        {
            cForm.dynamicMenus = new Hashtable();
            cForm.dynamicMenus[this.Name + ":lCheckAllowed"] = this.lCheckAllowed;
        }

        private void LoadStatic()
        {
            string Function = "ReferenceTab::LoadStatic()";
            CommonForm form = null;
            string Error = "";
            WStaticDataManager drop = new WStaticDataManager(true);

            try
            {
                //need to create a common form object to access the common functionality
                //because we're not inheriting it in this class
                form = new CommonForm();
                form.Wait();

                XmlUtilities xml = new XmlUtilities();
                XmlDocument dropDowns = new XmlDocument();
                dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

                if (StaticData.Tables[TN.RefRelation] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.RefRelation, new string[] { "RL1", "L" }));

                if (dropDowns.DocumentElement.ChildNodes.Count > 0)
                {
                    DataSet ds = drop.GetDropDownData(dropDowns.DocumentElement, out Error);
                    if (Error.Length > 0 | ds == null) form.ShowError(Error);
                    StaticData.Tables[TN.RefRelation] = ds.Tables[TN.RefRelation];
                }

                drpRelation.DataSource = (DataTable)StaticData.Tables[TN.RefRelation];
                drpRelation.DisplayMember = CN.CodeDescription;
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

        private void drpRelation_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (drpRelation.SelectedIndex > 0)
                _tp.Title = (string)((DataRowView)drpRelation.SelectedItem)[CN.CodeDescription];
        }

        private void ChxReference_CheckedChanged(object sender, System.EventArgs e)
        {
            if (ChxReference.Checked)
            {
                string err = "";
                txtCheckedBy.Text = Credential.User + " : " + cForm.Login.GetEmployeeName(Credential.UserId, out err);
                txtDateChecked.Text = DateTime.Today.ToString();
                this._empeeNo = Credential.UserId;
            }
            else
            {
                txtCheckedBy.Text = "";
                txtDateChecked.Text = "";
                this._empeeNo = 0;
            }
        }

        private void btnNotes_Click(object sender, System.EventArgs e)
        {
            this.gbDetails.Visible = false;
            this.gbDetails.Enabled = false;
            this.gbDetails.SendToBack();
            this.gbNotes.Visible = true;
            this.gbNotes.Enabled = true;
            this.gbNotes.BringToFront();
            this.txtDirections.Focus();
        }

        private void btnNotesClose_Click(object sender, System.EventArgs e)
        {
            this.gbNotes.Visible = false;
            this.gbNotes.Enabled = false;
            this.gbNotes.SendToBack();
            this.gbDetails.Visible = true;
            this.gbDetails.Enabled = true;
            this.gbDetails.BringToFront();
            this.txtFirstName.Focus();
        }

        //private void txtDialCode_TextChanged(object sender, System.EventArgs e)
        //{

        //}

        //IP - 21/11/11 - #8684 
        private void txtYearsKnown_TextChanged(object sender, EventArgs e)
        {
            if (!cForm.IsNumeric(txtYearsKnown.Text))
            {
                txtYearsKnown.Text = "0";
            }
        }

    }
}
