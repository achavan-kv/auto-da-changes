using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Specialized;
using System.Web.Services.Protocols;
using System.Xml;
using System.Data;
using STL.PL.WS1;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;
using STL.Common.Static;
using STL.Common;
using mshtml;
using System.Text;

namespace STL.PL
{
	/// <summary>
	/// Payment file definition for defining the format of Payment files e.g Standing Order, 
	/// that are received from Banks or other sources i.e internet.
	/// </summary>
	/// 
    public class PaymentFileDefn : CommonForm
    {
        private string error = "";
        private string _errorTxt = "";
        private DataSet dsDefn = new DataSet();
        private DataSet pmDefn = new DataSet();

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label lblTrailerMoneyBegin;
        private System.Windows.Forms.Label lblTrailerMoneyLength;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txtSourceName;
        private System.Windows.Forms.TextBox txtFileExt;
        private System.Windows.Forms.TextBox txtAcctNoBegin;
        private System.Windows.Forms.TextBox txtAcctNoLength;
        private System.Windows.Forms.TextBox txtMoneyBegin;
        private System.Windows.Forms.TextBox txtMoneyLength;
        private System.Windows.Forms.CheckBox cbHasDecimal;
        private System.Windows.Forms.CheckBox cbHasHeader;
        private System.Windows.Forms.TextBox txtDateBegin;
        private System.Windows.Forms.TextBox txtDateLength;
        private System.Windows.Forms.ComboBox drpDateFormat;
        private System.Windows.Forms.CheckBox cbHasTrailer;
        private System.Windows.Forms.TextBox txtTrailerBegin;
        private System.Windows.Forms.TextBox txtTrailerLength;
        private System.Windows.Forms.ComboBox drpPayMethod;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.ComboBox drpSourceName;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Button btnDelete;
        private TextBox txtTrailerId;
        private Label lblTrailerId;
        private TextBox txtTrailerIdLength;
        private Label lblTrailerIdLength;
        private TextBox txtTrailerIdBegin;
        private Label lblTrailerIdBegin;
        private TextBox txtHeaderId;
        private Label lblHeaderId;
        private TextBox txtHeaderIdLength;
        private Label lblHeaderIdLength;
        private Label lblHeaderIdBegin;
        private TextBox txtHeaderIdBegin;
        private GroupBox grpBatch;
        private CheckBox cbBatchHeaderHasTotal;
        private Label lblBatchHeaderHasTotal;
        private TextBox txtBatchHeaderId;
        private Label lblBatchHeaderId;
        private TextBox txtBatchHeaderIdLength;
        private Label lblBatchHeaderIdLength;
        private TextBox txtBatchHeaderIdBegin;
        private Label lblBatchHeaderIdBegin;
        private Label lblIsBatch;
        private CheckBox cbIsBatch;
        private TextBox txtBatchHeaderMoneyLength;
        private Label lblBatchHeaderMoneyLength;
        private TextBox txtBatchHeaderMoneyBegin;
        private Label lblBatchHeaderMoneyBegin;
        private Label lblDelimited;
        private Label lblDelimitedMoneyColumn;
        private Label lblDelimitedDateColumn;
        private Label lblDelimitedAcctNoColumn;
        private Label lblDelimtedNoColumns;
        private ComboBox drpDelimiter;
        private ComboBox drpMoneyCol;
        private ComboBox drpDateCol;
        private ComboBox drpAcctNoCol;
        private Label lblDelimiter;
        private ComboBox drpDelimitedDateFormat;
        private Label lblDelimiterDateFormat;
        private CheckBox cbDelimitedHasDecimal;
        private Label lblDelimitedMoneyHasDecimal;
        private RadioButton rbFixedFile;
        private Label lblFixedFile;
        private RadioButton rbDelimitedFile;
        private GroupBox grpFixedFile;
        private GroupBox grpDelimitedFile;
        private NumericUpDown numNoOfCols;
        private Label lblDelimiterDateLength;
        private TextBox txtDelimiterDateLength;
        private RadioButton rbIsInterest;
        private Label lblIsInterest;
        private RadioButton rbIsPayment;
        private Label lblPaymentFile;
        private GroupBox grpTransType;
        private GroupBox grpFileType;
        private TextBox txtBatchTrailerId;
        private Label lblBatchTrailerId;
        private TextBox txtBatchTrailerIdLength;
        private Label lblBatchTrailerIdLength;
        private TextBox txtBatchTrailerIdBegin;
        private Label lblBatchTrailerIdBegin;
        private IContainer components;

        public PaymentFileDefn(TranslationDummy d)
        {
            InitializeComponent();
            //	menuMain = new Crownwood.Magic.Menus.MenuControl();
            //	menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile});
        }

        public PaymentFileDefn(Form root, Form parent)
        {
            FormRoot = root;
            FormParent = parent;

            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            InitialiseStaticData();

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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.grpTransType = new System.Windows.Forms.GroupBox();
            this.lblPaymentFile = new System.Windows.Forms.Label();
            this.rbIsPayment = new System.Windows.Forms.RadioButton();
            this.rbIsInterest = new System.Windows.Forms.RadioButton();
            this.lblIsInterest = new System.Windows.Forms.Label();
            this.grpFileType = new System.Windows.Forms.GroupBox();
            this.lblFixedFile = new System.Windows.Forms.Label();
            this.rbFixedFile = new System.Windows.Forms.RadioButton();
            this.lblDelimited = new System.Windows.Forms.Label();
            this.rbDelimitedFile = new System.Windows.Forms.RadioButton();
            this.drpSourceName = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.btnDelete = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnClear = new System.Windows.Forms.Button();
            this.drpPayMethod = new System.Windows.Forms.ComboBox();
            this.txtSourceName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtFileExt = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cbHasHeader = new System.Windows.Forms.CheckBox();
            this.label15 = new System.Windows.Forms.Label();
            this.cbHasTrailer = new System.Windows.Forms.CheckBox();
            this.label12 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.grpFixedFile = new System.Windows.Forms.GroupBox();
            this.grpDelimitedFile = new System.Windows.Forms.GroupBox();
            this.lblDelimiterDateLength = new System.Windows.Forms.Label();
            this.txtDelimiterDateLength = new System.Windows.Forms.TextBox();
            this.numNoOfCols = new System.Windows.Forms.NumericUpDown();
            this.lblDelimtedNoColumns = new System.Windows.Forms.Label();
            this.cbDelimitedHasDecimal = new System.Windows.Forms.CheckBox();
            this.drpDelimitedDateFormat = new System.Windows.Forms.ComboBox();
            this.lblDelimitedMoneyHasDecimal = new System.Windows.Forms.Label();
            this.lblDelimitedAcctNoColumn = new System.Windows.Forms.Label();
            this.lblDelimiterDateFormat = new System.Windows.Forms.Label();
            this.lblDelimitedDateColumn = new System.Windows.Forms.Label();
            this.drpDelimiter = new System.Windows.Forms.ComboBox();
            this.lblDelimitedMoneyColumn = new System.Windows.Forms.Label();
            this.drpMoneyCol = new System.Windows.Forms.ComboBox();
            this.lblDelimiter = new System.Windows.Forms.Label();
            this.drpDateCol = new System.Windows.Forms.ComboBox();
            this.drpAcctNoCol = new System.Windows.Forms.ComboBox();
            this.grpBatch = new System.Windows.Forms.GroupBox();
            this.txtBatchTrailerId = new System.Windows.Forms.TextBox();
            this.lblBatchTrailerId = new System.Windows.Forms.Label();
            this.txtBatchTrailerIdLength = new System.Windows.Forms.TextBox();
            this.lblBatchTrailerIdLength = new System.Windows.Forms.Label();
            this.txtBatchTrailerIdBegin = new System.Windows.Forms.TextBox();
            this.lblBatchTrailerIdBegin = new System.Windows.Forms.Label();
            this.txtBatchHeaderMoneyLength = new System.Windows.Forms.TextBox();
            this.lblBatchHeaderMoneyLength = new System.Windows.Forms.Label();
            this.txtBatchHeaderMoneyBegin = new System.Windows.Forms.TextBox();
            this.lblBatchHeaderMoneyBegin = new System.Windows.Forms.Label();
            this.cbBatchHeaderHasTotal = new System.Windows.Forms.CheckBox();
            this.lblBatchHeaderHasTotal = new System.Windows.Forms.Label();
            this.txtBatchHeaderId = new System.Windows.Forms.TextBox();
            this.lblBatchHeaderId = new System.Windows.Forms.Label();
            this.txtBatchHeaderIdLength = new System.Windows.Forms.TextBox();
            this.lblBatchHeaderIdLength = new System.Windows.Forms.Label();
            this.txtBatchHeaderIdBegin = new System.Windows.Forms.TextBox();
            this.lblBatchHeaderIdBegin = new System.Windows.Forms.Label();
            this.lblHeaderIdBegin = new System.Windows.Forms.Label();
            this.cbIsBatch = new System.Windows.Forms.CheckBox();
            this.txtTrailerIdLength = new System.Windows.Forms.TextBox();
            this.lblIsBatch = new System.Windows.Forms.Label();
            this.lblHeaderIdLength = new System.Windows.Forms.Label();
            this.drpDateFormat = new System.Windows.Forms.ComboBox();
            this.txtTrailerLength = new System.Windows.Forms.TextBox();
            this.cbHasDecimal = new System.Windows.Forms.CheckBox();
            this.lblTrailerIdLength = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtTrailerBegin = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtHeaderIdLength = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtMoneyLength = new System.Windows.Forms.TextBox();
            this.txtTrailerIdBegin = new System.Windows.Forms.TextBox();
            this.txtMoneyBegin = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtDateLength = new System.Windows.Forms.TextBox();
            this.lblTrailerId = new System.Windows.Forms.Label();
            this.txtDateBegin = new System.Windows.Forms.TextBox();
            this.txtTrailerId = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtAcctNoBegin = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.lblTrailerIdBegin = new System.Windows.Forms.Label();
            this.lblTrailerMoneyLength = new System.Windows.Forms.Label();
            this.lblHeaderId = new System.Windows.Forms.Label();
            this.txtHeaderId = new System.Windows.Forms.TextBox();
            this.txtHeaderIdBegin = new System.Windows.Forms.TextBox();
            this.lblTrailerMoneyBegin = new System.Windows.Forms.Label();
            this.txtAcctNoLength = new System.Windows.Forms.TextBox();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.groupBox1.SuspendLayout();
            this.grpTransType.SuspendLayout();
            this.grpFileType.SuspendLayout();
            this.grpFixedFile.SuspendLayout();
            this.grpDelimitedFile.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numNoOfCols)).BeginInit();
            this.grpBatch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.grpDelimitedFile);
            this.groupBox1.Controls.Add(this.grpTransType);
            this.groupBox1.Controls.Add(this.grpFileType);
            this.groupBox1.Controls.Add(this.drpSourceName);
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Controls.Add(this.btnDelete);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnClear);
            this.groupBox1.Controls.Add(this.drpPayMethod);
            this.groupBox1.Controls.Add(this.txtSourceName);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtFileExt);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.cbHasHeader);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.cbHasTrailer);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.btnSave);
            this.groupBox1.Controls.Add(this.grpFixedFile);
            this.groupBox1.Location = new System.Drawing.Point(1, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(775, 460);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Payment File Definition";
            // 
            // grpTransType
            // 
            this.grpTransType.Controls.Add(this.lblPaymentFile);
            this.grpTransType.Controls.Add(this.rbIsPayment);
            this.grpTransType.Controls.Add(this.rbIsInterest);
            this.grpTransType.Controls.Add(this.lblIsInterest);
            this.grpTransType.Location = new System.Drawing.Point(364, 86);
            this.grpTransType.Name = "grpTransType";
            this.grpTransType.Size = new System.Drawing.Size(263, 32);
            this.grpTransType.TabIndex = 65;
            this.grpTransType.TabStop = false;
            this.grpTransType.Text = "Transaction Type";
            // 
            // lblPaymentFile
            // 
            this.lblPaymentFile.AutoSize = true;
            this.lblPaymentFile.Location = new System.Drawing.Point(6, 12);
            this.lblPaymentFile.Name = "lblPaymentFile";
            this.lblPaymentFile.Size = new System.Drawing.Size(51, 13);
            this.lblPaymentFile.TabIndex = 60;
            this.lblPaymentFile.Text = "Payment:";
            // 
            // rbIsPayment
            // 
            this.rbIsPayment.AutoSize = true;
            this.rbIsPayment.Location = new System.Drawing.Point(63, 12);
            this.rbIsPayment.Name = "rbIsPayment";
            this.rbIsPayment.Size = new System.Drawing.Size(14, 13);
            this.rbIsPayment.TabIndex = 61;
            this.rbIsPayment.UseVisualStyleBackColor = true;
            // 
            // rbIsInterest
            // 
            this.rbIsInterest.AutoSize = true;
            this.rbIsInterest.Location = new System.Drawing.Point(195, 12);
            this.rbIsInterest.Name = "rbIsInterest";
            this.rbIsInterest.Size = new System.Drawing.Size(14, 13);
            this.rbIsInterest.TabIndex = 63;
            this.rbIsInterest.UseVisualStyleBackColor = true;
            // 
            // lblIsInterest
            // 
            this.lblIsInterest.AutoSize = true;
            this.lblIsInterest.Location = new System.Drawing.Point(144, 12);
            this.lblIsInterest.Name = "lblIsInterest";
            this.lblIsInterest.Size = new System.Drawing.Size(45, 13);
            this.lblIsInterest.TabIndex = 62;
            this.lblIsInterest.Text = "Interest:";
            // 
            // grpFileType
            // 
            this.grpFileType.Controls.Add(this.lblFixedFile);
            this.grpFileType.Controls.Add(this.rbFixedFile);
            this.grpFileType.Controls.Add(this.lblDelimited);
            this.grpFileType.Controls.Add(this.rbDelimitedFile);
            this.grpFileType.Location = new System.Drawing.Point(103, 85);
            this.grpFileType.Name = "grpFileType";
            this.grpFileType.Size = new System.Drawing.Size(251, 33);
            this.grpFileType.TabIndex = 64;
            this.grpFileType.TabStop = false;
            this.grpFileType.Text = "File Type";
            // 
            // lblFixedFile
            // 
            this.lblFixedFile.AutoSize = true;
            this.lblFixedFile.Location = new System.Drawing.Point(6, 14);
            this.lblFixedFile.Name = "lblFixedFile";
            this.lblFixedFile.Size = new System.Drawing.Size(54, 13);
            this.lblFixedFile.TabIndex = 58;
            this.lblFixedFile.Text = "Fixed File:";
            // 
            // rbFixedFile
            // 
            this.rbFixedFile.AutoSize = true;
            this.rbFixedFile.Location = new System.Drawing.Point(66, 14);
            this.rbFixedFile.Name = "rbFixedFile";
            this.rbFixedFile.Size = new System.Drawing.Size(14, 13);
            this.rbFixedFile.TabIndex = 59;
            this.rbFixedFile.UseVisualStyleBackColor = true;
            this.rbFixedFile.CheckedChanged += new System.EventHandler(this.rbFixedFile_CheckedChanged);
            // 
            // lblDelimited
            // 
            this.lblDelimited.AutoSize = true;
            this.lblDelimited.Location = new System.Drawing.Point(151, 14);
            this.lblDelimited.Name = "lblDelimited";
            this.lblDelimited.Size = new System.Drawing.Size(72, 13);
            this.lblDelimited.TabIndex = 55;
            this.lblDelimited.Text = "Delimited File:";
            // 
            // rbDelimitedFile
            // 
            this.rbDelimitedFile.AutoSize = true;
            this.rbDelimitedFile.Location = new System.Drawing.Point(229, 13);
            this.rbDelimitedFile.Name = "rbDelimitedFile";
            this.rbDelimitedFile.Size = new System.Drawing.Size(14, 13);
            this.rbDelimitedFile.TabIndex = 57;
            this.rbDelimitedFile.UseVisualStyleBackColor = true;
            this.rbDelimitedFile.CheckedChanged += new System.EventHandler(this.rbDelimitedFile_CheckedChanged);
            // 
            // drpSourceName
            // 
            this.drpSourceName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpSourceName.Location = new System.Drawing.Point(111, 33);
            this.drpSourceName.Name = "drpSourceName";
            this.drpSourceName.Size = new System.Drawing.Size(121, 21);
            this.drpSourceName.Sorted = true;
            this.drpSourceName.TabIndex = 35;
            this.drpSourceName.SelectedIndexChanged += new System.EventHandler(this.drpSourceName_SelectedIndexChanged);
            // 
            // label16
            // 
            this.label16.Location = new System.Drawing.Point(108, 14);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(120, 16);
            this.label16.TabIndex = 34;
            this.label16.Text = "Existing Source Files";
            // 
            // btnDelete
            // 
            this.btnDelete.Enabled = false;
            this.btnDelete.Location = new System.Drawing.Point(684, 69);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 37;
            this.btnDelete.Text = "Delete";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(251, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 16);
            this.label1.TabIndex = 4;
            this.label1.Text = "Source Name:";
            // 
            // btnClear
            // 
            this.btnClear.Enabled = false;
            this.btnClear.Location = new System.Drawing.Point(684, 11);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 36;
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // drpPayMethod
            // 
            this.drpPayMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpPayMethod.Location = new System.Drawing.Point(509, 56);
            this.drpPayMethod.Name = "drpPayMethod";
            this.drpPayMethod.Size = new System.Drawing.Size(115, 21);
            this.drpPayMethod.TabIndex = 33;
            // 
            // txtSourceName
            // 
            this.txtSourceName.Location = new System.Drawing.Point(254, 34);
            this.txtSourceName.Name = "txtSourceName";
            this.txtSourceName.Size = new System.Drawing.Size(100, 20);
            this.txtSourceName.TabIndex = 19;
            this.txtSourceName.TextChanged += new System.EventHandler(this.txtSourceNameChanged);
            this.txtSourceName.Leave += new System.EventHandler(this.txtSourceName_Leave);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(368, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 16);
            this.label2.TabIndex = 5;
            this.label2.Text = "File Extension:";
            // 
            // txtFileExt
            // 
            this.txtFileExt.Location = new System.Drawing.Point(371, 34);
            this.txtFileExt.MaxLength = 3;
            this.txtFileExt.Name = "txtFileExt";
            this.txtFileExt.Size = new System.Drawing.Size(47, 20);
            this.txtFileExt.TabIndex = 20;
            this.txtFileExt.Leave += new System.EventHandler(this.txtFileExt_Leave);
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(109, 61);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(103, 18);
            this.label8.TabIndex = 11;
            this.label8.Text = "File has Header:";
            // 
            // cbHasHeader
            // 
            this.cbHasHeader.Checked = true;
            this.cbHasHeader.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbHasHeader.Location = new System.Drawing.Point(218, 56);
            this.cbHasHeader.Name = "cbHasHeader";
            this.cbHasHeader.Size = new System.Drawing.Size(21, 24);
            this.cbHasHeader.TabIndex = 26;
            this.cbHasHeader.CheckedChanged += new System.EventHandler(this.cbHasHeader_CheckedChanged);
            // 
            // label15
            // 
            this.label15.Location = new System.Drawing.Point(403, 61);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(100, 16);
            this.label15.TabIndex = 18;
            this.label15.Text = "Payment Method:";
            // 
            // cbHasTrailer
            // 
            this.cbHasTrailer.Checked = true;
            this.cbHasTrailer.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbHasTrailer.Location = new System.Drawing.Point(371, 59);
            this.cbHasTrailer.Name = "cbHasTrailer";
            this.cbHasTrailer.Size = new System.Drawing.Size(25, 19);
            this.cbHasTrailer.TabIndex = 30;
            this.cbHasTrailer.CheckedChanged += new System.EventHandler(this.cbHasTrailer_CheckedChanged);
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(251, 61);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(100, 16);
            this.label12.TabIndex = 15;
            this.label12.Text = "File has Trailer:";
            // 
            // btnSave
            // 
            this.btnSave.Enabled = false;
            this.btnSave.Location = new System.Drawing.Point(684, 40);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // grpFixedFile
            // 
            this.grpFixedFile.Controls.Add(this.grpBatch);
            this.grpFixedFile.Controls.Add(this.lblHeaderIdBegin);
            this.grpFixedFile.Controls.Add(this.cbIsBatch);
            this.grpFixedFile.Controls.Add(this.txtTrailerIdLength);
            this.grpFixedFile.Controls.Add(this.lblIsBatch);
            this.grpFixedFile.Controls.Add(this.lblHeaderIdLength);
            this.grpFixedFile.Controls.Add(this.drpDateFormat);
            this.grpFixedFile.Controls.Add(this.txtTrailerLength);
            this.grpFixedFile.Controls.Add(this.cbHasDecimal);
            this.grpFixedFile.Controls.Add(this.lblTrailerIdLength);
            this.grpFixedFile.Controls.Add(this.label5);
            this.grpFixedFile.Controls.Add(this.txtTrailerBegin);
            this.grpFixedFile.Controls.Add(this.label7);
            this.grpFixedFile.Controls.Add(this.txtHeaderIdLength);
            this.grpFixedFile.Controls.Add(this.label6);
            this.grpFixedFile.Controls.Add(this.label3);
            this.grpFixedFile.Controls.Add(this.txtMoneyLength);
            this.grpFixedFile.Controls.Add(this.txtTrailerIdBegin);
            this.grpFixedFile.Controls.Add(this.txtMoneyBegin);
            this.grpFixedFile.Controls.Add(this.label4);
            this.grpFixedFile.Controls.Add(this.txtDateLength);
            this.grpFixedFile.Controls.Add(this.lblTrailerId);
            this.grpFixedFile.Controls.Add(this.txtDateBegin);
            this.grpFixedFile.Controls.Add(this.txtTrailerId);
            this.grpFixedFile.Controls.Add(this.label11);
            this.grpFixedFile.Controls.Add(this.txtAcctNoBegin);
            this.grpFixedFile.Controls.Add(this.label9);
            this.grpFixedFile.Controls.Add(this.label10);
            this.grpFixedFile.Controls.Add(this.lblTrailerIdBegin);
            this.grpFixedFile.Controls.Add(this.lblTrailerMoneyLength);
            this.grpFixedFile.Controls.Add(this.lblHeaderId);
            this.grpFixedFile.Controls.Add(this.txtHeaderId);
            this.grpFixedFile.Controls.Add(this.txtHeaderIdBegin);
            this.grpFixedFile.Controls.Add(this.lblTrailerMoneyBegin);
            this.grpFixedFile.Controls.Add(this.txtAcctNoLength);
            this.grpFixedFile.Location = new System.Drawing.Point(64, 121);
            this.grpFixedFile.Name = "grpFixedFile";
            this.grpFixedFile.Size = new System.Drawing.Size(582, 315);
            this.grpFixedFile.TabIndex = 56;
            this.grpFixedFile.TabStop = false;
            this.grpFixedFile.Text = "Fixed File";
            // 
            // grpDelimitedFile
            // 
            this.grpDelimitedFile.Controls.Add(this.lblDelimiterDateLength);
            this.grpDelimitedFile.Controls.Add(this.txtDelimiterDateLength);
            this.grpDelimitedFile.Controls.Add(this.numNoOfCols);
            this.grpDelimitedFile.Controls.Add(this.lblDelimtedNoColumns);
            this.grpDelimitedFile.Controls.Add(this.cbDelimitedHasDecimal);
            this.grpDelimitedFile.Controls.Add(this.drpDelimitedDateFormat);
            this.grpDelimitedFile.Controls.Add(this.lblDelimitedMoneyHasDecimal);
            this.grpDelimitedFile.Controls.Add(this.lblDelimitedAcctNoColumn);
            this.grpDelimitedFile.Controls.Add(this.lblDelimiterDateFormat);
            this.grpDelimitedFile.Controls.Add(this.lblDelimitedDateColumn);
            this.grpDelimitedFile.Controls.Add(this.drpDelimiter);
            this.grpDelimitedFile.Controls.Add(this.lblDelimitedMoneyColumn);
            this.grpDelimitedFile.Controls.Add(this.drpMoneyCol);
            this.grpDelimitedFile.Controls.Add(this.lblDelimiter);
            this.grpDelimitedFile.Controls.Add(this.drpDateCol);
            this.grpDelimitedFile.Controls.Add(this.drpAcctNoCol);
            this.grpDelimitedFile.Location = new System.Drawing.Point(65, 121);
            this.grpDelimitedFile.Name = "grpDelimitedFile";
            this.grpDelimitedFile.Size = new System.Drawing.Size(658, 159);
            this.grpDelimitedFile.TabIndex = 57;
            this.grpDelimitedFile.TabStop = false;
            this.grpDelimitedFile.Text = "Delimited File";
            // 
            // lblDelimiterDateLength
            // 
            this.lblDelimiterDateLength.Location = new System.Drawing.Point(284, 106);
            this.lblDelimiterDateLength.Name = "lblDelimiterDateLength";
            this.lblDelimiterDateLength.Size = new System.Drawing.Size(73, 18);
            this.lblDelimiterDateLength.TabIndex = 54;
            this.lblDelimiterDateLength.Text = "DateLength:";
            // 
            // txtDelimiterDateLength
            // 
            this.txtDelimiterDateLength.Location = new System.Drawing.Point(370, 103);
            this.txtDelimiterDateLength.Name = "txtDelimiterDateLength";
            this.txtDelimiterDateLength.ReadOnly = true;
            this.txtDelimiterDateLength.Size = new System.Drawing.Size(46, 20);
            this.txtDelimiterDateLength.TabIndex = 53;
            // 
            // numNoOfCols
            // 
            this.numNoOfCols.Location = new System.Drawing.Point(133, 29);
            this.numNoOfCols.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.numNoOfCols.Name = "numNoOfCols";
            this.numNoOfCols.Size = new System.Drawing.Size(77, 20);
            this.numNoOfCols.TabIndex = 52;
            this.numNoOfCols.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // lblDelimtedNoColumns
            // 
            this.lblDelimtedNoColumns.Location = new System.Drawing.Point(5, 31);
            this.lblDelimtedNoColumns.Name = "lblDelimtedNoColumns";
            this.lblDelimtedNoColumns.Size = new System.Drawing.Size(122, 18);
            this.lblDelimtedNoColumns.TabIndex = 12;
            this.lblDelimtedNoColumns.Text = "No of Columns in file:";
            // 
            // cbDelimitedHasDecimal
            // 
            this.cbDelimitedHasDecimal.Location = new System.Drawing.Point(419, 134);
            this.cbDelimitedHasDecimal.Name = "cbDelimitedHasDecimal";
            this.cbDelimitedHasDecimal.Size = new System.Drawing.Size(31, 24);
            this.cbDelimitedHasDecimal.TabIndex = 51;
            // 
            // drpDelimitedDateFormat
            // 
            this.drpDelimitedDateFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpDelimitedDateFormat.Location = new System.Drawing.Point(514, 103);
            this.drpDelimitedDateFormat.Name = "drpDelimitedDateFormat";
            this.drpDelimitedDateFormat.Size = new System.Drawing.Size(82, 21);
            this.drpDelimitedDateFormat.TabIndex = 49;
            this.drpDelimitedDateFormat.SelectedIndexChanged += new System.EventHandler(this.drpDelimitedDateFormat_SelectedIndexChanged);
            // 
            // lblDelimitedMoneyHasDecimal
            // 
            this.lblDelimitedMoneyHasDecimal.Location = new System.Drawing.Point(284, 138);
            this.lblDelimitedMoneyHasDecimal.Name = "lblDelimitedMoneyHasDecimal";
            this.lblDelimitedMoneyHasDecimal.Size = new System.Drawing.Size(136, 16);
            this.lblDelimitedMoneyHasDecimal.TabIndex = 50;
            this.lblDelimitedMoneyHasDecimal.Text = "Money has decimal point:";
            // 
            // lblDelimitedAcctNoColumn
            // 
            this.lblDelimitedAcctNoColumn.Location = new System.Drawing.Point(6, 72);
            this.lblDelimitedAcctNoColumn.Name = "lblDelimitedAcctNoColumn";
            this.lblDelimitedAcctNoColumn.Size = new System.Drawing.Size(109, 18);
            this.lblDelimitedAcctNoColumn.TabIndex = 40;
            this.lblDelimitedAcctNoColumn.Text = "Account No Column:";
            // 
            // lblDelimiterDateFormat
            // 
            this.lblDelimiterDateFormat.Location = new System.Drawing.Point(444, 108);
            this.lblDelimiterDateFormat.Name = "lblDelimiterDateFormat";
            this.lblDelimiterDateFormat.Size = new System.Drawing.Size(73, 16);
            this.lblDelimiterDateFormat.TabIndex = 48;
            this.lblDelimiterDateFormat.Text = "Date format:";
            // 
            // lblDelimitedDateColumn
            // 
            this.lblDelimitedDateColumn.Location = new System.Drawing.Point(6, 103);
            this.lblDelimitedDateColumn.Name = "lblDelimitedDateColumn";
            this.lblDelimitedDateColumn.Size = new System.Drawing.Size(109, 18);
            this.lblDelimitedDateColumn.TabIndex = 41;
            this.lblDelimitedDateColumn.Text = "Date Column:";
            // 
            // drpDelimiter
            // 
            this.drpDelimiter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpDelimiter.FormattingEnabled = true;
            this.drpDelimiter.Location = new System.Drawing.Point(370, 29);
            this.drpDelimiter.Name = "drpDelimiter";
            this.drpDelimiter.Size = new System.Drawing.Size(121, 21);
            this.drpDelimiter.TabIndex = 47;
            // 
            // lblDelimitedMoneyColumn
            // 
            this.lblDelimitedMoneyColumn.Location = new System.Drawing.Point(6, 137);
            this.lblDelimitedMoneyColumn.Name = "lblDelimitedMoneyColumn";
            this.lblDelimitedMoneyColumn.Size = new System.Drawing.Size(109, 18);
            this.lblDelimitedMoneyColumn.TabIndex = 42;
            this.lblDelimitedMoneyColumn.Text = "Money Column:";
            // 
            // drpMoneyCol
            // 
            this.drpMoneyCol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpMoneyCol.FormattingEnabled = true;
            this.drpMoneyCol.Location = new System.Drawing.Point(133, 134);
            this.drpMoneyCol.Name = "drpMoneyCol";
            this.drpMoneyCol.Size = new System.Drawing.Size(121, 21);
            this.drpMoneyCol.TabIndex = 46;
            // 
            // lblDelimiter
            // 
            this.lblDelimiter.Location = new System.Drawing.Point(284, 31);
            this.lblDelimiter.Name = "lblDelimiter";
            this.lblDelimiter.Size = new System.Drawing.Size(54, 18);
            this.lblDelimiter.TabIndex = 43;
            this.lblDelimiter.Text = "Delimiter:";
            // 
            // drpDateCol
            // 
            this.drpDateCol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpDateCol.FormattingEnabled = true;
            this.drpDateCol.Location = new System.Drawing.Point(133, 103);
            this.drpDateCol.Name = "drpDateCol";
            this.drpDateCol.Size = new System.Drawing.Size(121, 21);
            this.drpDateCol.TabIndex = 45;
            // 
            // drpAcctNoCol
            // 
            this.drpAcctNoCol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpAcctNoCol.FormattingEnabled = true;
            this.drpAcctNoCol.Location = new System.Drawing.Point(133, 69);
            this.drpAcctNoCol.Name = "drpAcctNoCol";
            this.drpAcctNoCol.Size = new System.Drawing.Size(121, 21);
            this.drpAcctNoCol.TabIndex = 44;
            // 
            // grpBatch
            // 
            this.grpBatch.Controls.Add(this.txtBatchTrailerId);
            this.grpBatch.Controls.Add(this.lblBatchTrailerId);
            this.grpBatch.Controls.Add(this.txtBatchTrailerIdLength);
            this.grpBatch.Controls.Add(this.lblBatchTrailerIdLength);
            this.grpBatch.Controls.Add(this.txtBatchTrailerIdBegin);
            this.grpBatch.Controls.Add(this.lblBatchTrailerIdBegin);
            this.grpBatch.Controls.Add(this.txtBatchHeaderMoneyLength);
            this.grpBatch.Controls.Add(this.lblBatchHeaderMoneyLength);
            this.grpBatch.Controls.Add(this.txtBatchHeaderMoneyBegin);
            this.grpBatch.Controls.Add(this.lblBatchHeaderMoneyBegin);
            this.grpBatch.Controls.Add(this.cbBatchHeaderHasTotal);
            this.grpBatch.Controls.Add(this.lblBatchHeaderHasTotal);
            this.grpBatch.Controls.Add(this.txtBatchHeaderId);
            this.grpBatch.Controls.Add(this.lblBatchHeaderId);
            this.grpBatch.Controls.Add(this.txtBatchHeaderIdLength);
            this.grpBatch.Controls.Add(this.lblBatchHeaderIdLength);
            this.grpBatch.Controls.Add(this.txtBatchHeaderIdBegin);
            this.grpBatch.Controls.Add(this.lblBatchHeaderIdBegin);
            this.grpBatch.Location = new System.Drawing.Point(4, 190);
            this.grpBatch.Name = "grpBatch";
            this.grpBatch.Size = new System.Drawing.Size(572, 115);
            this.grpBatch.TabIndex = 52;
            this.grpBatch.TabStop = false;
            this.grpBatch.Text = "Batch";
            // 
            // txtBatchTrailerId
            // 
            this.txtBatchTrailerId.Location = new System.Drawing.Point(510, 87);
            this.txtBatchTrailerId.MaxLength = 20;
            this.txtBatchTrailerId.Name = "txtBatchTrailerId";
            this.txtBatchTrailerId.Size = new System.Drawing.Size(46, 20);
            this.txtBatchTrailerId.TabIndex = 70;
            // 
            // lblBatchTrailerId
            // 
            this.lblBatchTrailerId.Location = new System.Drawing.Point(433, 92);
            this.lblBatchTrailerId.Name = "lblBatchTrailerId";
            this.lblBatchTrailerId.Size = new System.Drawing.Size(66, 19);
            this.lblBatchTrailerId.TabIndex = 69;
            this.lblBatchTrailerId.Text = "Trailer Id:";
            // 
            // txtBatchTrailerIdLength
            // 
            this.txtBatchTrailerIdLength.Location = new System.Drawing.Point(364, 89);
            this.txtBatchTrailerIdLength.Name = "txtBatchTrailerIdLength";
            this.txtBatchTrailerIdLength.Size = new System.Drawing.Size(46, 20);
            this.txtBatchTrailerIdLength.TabIndex = 68;
            this.txtBatchTrailerIdLength.TextChanged += new System.EventHandler(this.txtBatchTrailerIdLength_TextChanged);
            // 
            // lblBatchTrailerIdLength
            // 
            this.lblBatchTrailerIdLength.Location = new System.Drawing.Point(226, 93);
            this.lblBatchTrailerIdLength.Name = "lblBatchTrailerIdLength";
            this.lblBatchTrailerIdLength.Size = new System.Drawing.Size(132, 19);
            this.lblBatchTrailerIdLength.TabIndex = 67;
            this.lblBatchTrailerIdLength.Text = "Trailer Id Length:";
            // 
            // txtBatchTrailerIdBegin
            // 
            this.txtBatchTrailerIdBegin.Location = new System.Drawing.Point(123, 88);
            this.txtBatchTrailerIdBegin.Name = "txtBatchTrailerIdBegin";
            this.txtBatchTrailerIdBegin.Size = new System.Drawing.Size(46, 20);
            this.txtBatchTrailerIdBegin.TabIndex = 66;
            // 
            // lblBatchTrailerIdBegin
            // 
            this.lblBatchTrailerIdBegin.Location = new System.Drawing.Point(2, 93);
            this.lblBatchTrailerIdBegin.Name = "lblBatchTrailerIdBegin";
            this.lblBatchTrailerIdBegin.Size = new System.Drawing.Size(120, 19);
            this.lblBatchTrailerIdBegin.TabIndex = 65;
            this.lblBatchTrailerIdBegin.Text = "Trailer Id Begin:";
            // 
            // txtBatchHeaderMoneyLength
            // 
            this.txtBatchHeaderMoneyLength.Location = new System.Drawing.Point(364, 67);
            this.txtBatchHeaderMoneyLength.Name = "txtBatchHeaderMoneyLength";
            this.txtBatchHeaderMoneyLength.Size = new System.Drawing.Size(46, 20);
            this.txtBatchHeaderMoneyLength.TabIndex = 64;
            // 
            // lblBatchHeaderMoneyLength
            // 
            this.lblBatchHeaderMoneyLength.Location = new System.Drawing.Point(224, 71);
            this.lblBatchHeaderMoneyLength.Name = "lblBatchHeaderMoneyLength";
            this.lblBatchHeaderMoneyLength.Size = new System.Drawing.Size(123, 16);
            this.lblBatchHeaderMoneyLength.TabIndex = 63;
            this.lblBatchHeaderMoneyLength.Text = " Header Money Length:";
            // 
            // txtBatchHeaderMoneyBegin
            // 
            this.txtBatchHeaderMoneyBegin.Location = new System.Drawing.Point(123, 63);
            this.txtBatchHeaderMoneyBegin.Name = "txtBatchHeaderMoneyBegin";
            this.txtBatchHeaderMoneyBegin.Size = new System.Drawing.Size(46, 20);
            this.txtBatchHeaderMoneyBegin.TabIndex = 62;
            // 
            // lblBatchHeaderMoneyBegin
            // 
            this.lblBatchHeaderMoneyBegin.Location = new System.Drawing.Point(-1, 67);
            this.lblBatchHeaderMoneyBegin.Name = "lblBatchHeaderMoneyBegin";
            this.lblBatchHeaderMoneyBegin.Size = new System.Drawing.Size(123, 16);
            this.lblBatchHeaderMoneyBegin.TabIndex = 60;
            this.lblBatchHeaderMoneyBegin.Text = " Header Money Begin:";
            // 
            // cbBatchHeaderHasTotal
            // 
            this.cbBatchHeaderHasTotal.Checked = true;
            this.cbBatchHeaderHasTotal.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbBatchHeaderHasTotal.Location = new System.Drawing.Point(123, 13);
            this.cbBatchHeaderHasTotal.Name = "cbBatchHeaderHasTotal";
            this.cbBatchHeaderHasTotal.Size = new System.Drawing.Size(28, 24);
            this.cbBatchHeaderHasTotal.TabIndex = 59;
            this.cbBatchHeaderHasTotal.CheckedChanged += new System.EventHandler(this.cbBactHeaderHasTotal_CheckedChanged);
            // 
            // lblBatchHeaderHasTotal
            // 
            this.lblBatchHeaderHasTotal.Location = new System.Drawing.Point(3, 18);
            this.lblBatchHeaderHasTotal.Name = "lblBatchHeaderHasTotal";
            this.lblBatchHeaderHasTotal.Size = new System.Drawing.Size(138, 19);
            this.lblBatchHeaderHasTotal.TabIndex = 58;
            this.lblBatchHeaderHasTotal.Text = "Header has Total:";
            // 
            // txtBatchHeaderId
            // 
            this.txtBatchHeaderId.Location = new System.Drawing.Point(510, 44);
            this.txtBatchHeaderId.MaxLength = 20;
            this.txtBatchHeaderId.Name = "txtBatchHeaderId";
            this.txtBatchHeaderId.Size = new System.Drawing.Size(46, 20);
            this.txtBatchHeaderId.TabIndex = 57;
            this.txtBatchHeaderId.TextChanged += new System.EventHandler(this.txtBatchHeaderId_TextChanged);
            // 
            // lblBatchHeaderId
            // 
            this.lblBatchHeaderId.Location = new System.Drawing.Point(433, 47);
            this.lblBatchHeaderId.Name = "lblBatchHeaderId";
            this.lblBatchHeaderId.Size = new System.Drawing.Size(71, 19);
            this.lblBatchHeaderId.TabIndex = 56;
            this.lblBatchHeaderId.Text = "Header Id:";
            // 
            // txtBatchHeaderIdLength
            // 
            this.txtBatchHeaderIdLength.Location = new System.Drawing.Point(364, 42);
            this.txtBatchHeaderIdLength.Name = "txtBatchHeaderIdLength";
            this.txtBatchHeaderIdLength.Size = new System.Drawing.Size(46, 20);
            this.txtBatchHeaderIdLength.TabIndex = 55;
            this.txtBatchHeaderIdLength.TextChanged += new System.EventHandler(this.txtBatchHeaderIdLength_TextChanged);
            // 
            // lblBatchHeaderIdLength
            // 
            this.lblBatchHeaderIdLength.Location = new System.Drawing.Point(226, 45);
            this.lblBatchHeaderIdLength.Name = "lblBatchHeaderIdLength";
            this.lblBatchHeaderIdLength.Size = new System.Drawing.Size(132, 19);
            this.lblBatchHeaderIdLength.TabIndex = 54;
            this.lblBatchHeaderIdLength.Text = "Header Id Length:";
            // 
            // txtBatchHeaderIdBegin
            // 
            this.txtBatchHeaderIdBegin.Location = new System.Drawing.Point(123, 40);
            this.txtBatchHeaderIdBegin.Name = "txtBatchHeaderIdBegin";
            this.txtBatchHeaderIdBegin.Size = new System.Drawing.Size(46, 20);
            this.txtBatchHeaderIdBegin.TabIndex = 53;
            // 
            // lblBatchHeaderIdBegin
            // 
            this.lblBatchHeaderIdBegin.Location = new System.Drawing.Point(3, 43);
            this.lblBatchHeaderIdBegin.Name = "lblBatchHeaderIdBegin";
            this.lblBatchHeaderIdBegin.Size = new System.Drawing.Size(120, 19);
            this.lblBatchHeaderIdBegin.TabIndex = 52;
            this.lblBatchHeaderIdBegin.Text = "Header Id Begin:";
            // 
            // lblHeaderIdBegin
            // 
            this.lblHeaderIdBegin.AutoSize = true;
            this.lblHeaderIdBegin.Location = new System.Drawing.Point(6, 17);
            this.lblHeaderIdBegin.Name = "lblHeaderIdBegin";
            this.lblHeaderIdBegin.Size = new System.Drawing.Size(86, 13);
            this.lblHeaderIdBegin.TabIndex = 39;
            this.lblHeaderIdBegin.Text = "Header Id begin:";
            // 
            // cbIsBatch
            // 
            this.cbIsBatch.Checked = true;
            this.cbIsBatch.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbIsBatch.Location = new System.Drawing.Point(122, 169);
            this.cbIsBatch.Name = "cbIsBatch";
            this.cbIsBatch.Size = new System.Drawing.Size(28, 24);
            this.cbIsBatch.TabIndex = 50;
            this.cbIsBatch.CheckedChanged += new System.EventHandler(this.cbIsBatch_CheckedChanged);
            // 
            // txtTrailerIdLength
            // 
            this.txtTrailerIdLength.Location = new System.Drawing.Point(301, 44);
            this.txtTrailerIdLength.Name = "txtTrailerIdLength";
            this.txtTrailerIdLength.Size = new System.Drawing.Size(46, 20);
            this.txtTrailerIdLength.TabIndex = 47;
            this.txtTrailerIdLength.TextChanged += new System.EventHandler(this.txtTrailerIdLength_TextChanged);
            // 
            // lblIsBatch
            // 
            this.lblIsBatch.Location = new System.Drawing.Point(6, 173);
            this.lblIsBatch.Name = "lblIsBatch";
            this.lblIsBatch.Size = new System.Drawing.Size(63, 15);
            this.lblIsBatch.TabIndex = 51;
            this.lblIsBatch.Text = "Batch File:";
            // 
            // lblHeaderIdLength
            // 
            this.lblHeaderIdLength.AutoSize = true;
            this.lblHeaderIdLength.Location = new System.Drawing.Point(184, 19);
            this.lblHeaderIdLength.Name = "lblHeaderIdLength";
            this.lblHeaderIdLength.Size = new System.Drawing.Size(93, 13);
            this.lblHeaderIdLength.TabIndex = 40;
            this.lblHeaderIdLength.Text = "Header Id Length:";
            // 
            // drpDateFormat
            // 
            this.drpDateFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpDateFormat.Location = new System.Drawing.Point(445, 126);
            this.drpDateFormat.Name = "drpDateFormat";
            this.drpDateFormat.Size = new System.Drawing.Size(82, 21);
            this.drpDateFormat.TabIndex = 29;
            this.drpDateFormat.SelectedIndexChanged += new System.EventHandler(this.drpDateFormat_SelectedIndexChanged);
            // 
            // txtTrailerLength
            // 
            this.txtTrailerLength.Location = new System.Drawing.Point(301, 70);
            this.txtTrailerLength.Name = "txtTrailerLength";
            this.txtTrailerLength.Size = new System.Drawing.Size(46, 20);
            this.txtTrailerLength.TabIndex = 32;
            // 
            // cbHasDecimal
            // 
            this.cbHasDecimal.Location = new System.Drawing.Point(506, 154);
            this.cbHasDecimal.Name = "cbHasDecimal";
            this.cbHasDecimal.Size = new System.Drawing.Size(31, 24);
            this.cbHasDecimal.TabIndex = 25;
            // 
            // lblTrailerIdLength
            // 
            this.lblTrailerIdLength.AutoSize = true;
            this.lblTrailerIdLength.Location = new System.Drawing.Point(184, 47);
            this.lblTrailerIdLength.Name = "lblTrailerIdLength";
            this.lblTrailerIdLength.Size = new System.Drawing.Size(83, 13);
            this.lblTrailerIdLength.TabIndex = 46;
            this.lblTrailerIdLength.Text = "Trailer Id length:";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(5, 153);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(100, 16);
            this.label5.TabIndex = 8;
            this.label5.Text = "Money begin:";
            // 
            // txtTrailerBegin
            // 
            this.txtTrailerBegin.Location = new System.Drawing.Point(121, 71);
            this.txtTrailerBegin.Name = "txtTrailerBegin";
            this.txtTrailerBegin.Size = new System.Drawing.Size(46, 20);
            this.txtTrailerBegin.TabIndex = 31;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(374, 158);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(129, 16);
            this.label7.TabIndex = 10;
            this.label7.Text = "Money has decimal point:";
            // 
            // txtHeaderIdLength
            // 
            this.txtHeaderIdLength.Location = new System.Drawing.Point(301, 16);
            this.txtHeaderIdLength.Name = "txtHeaderIdLength";
            this.txtHeaderIdLength.Size = new System.Drawing.Size(46, 20);
            this.txtHeaderIdLength.TabIndex = 41;
            this.txtHeaderIdLength.TextChanged += new System.EventHandler(this.txtHeaderIdLength_TextChanged);
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(184, 157);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(100, 16);
            this.label6.TabIndex = 9;
            this.label6.Text = "Money length:";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(6, 97);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 22);
            this.label3.TabIndex = 6;
            this.label3.Text = "Account No. begin:";
            // 
            // txtMoneyLength
            // 
            this.txtMoneyLength.Location = new System.Drawing.Point(301, 154);
            this.txtMoneyLength.Name = "txtMoneyLength";
            this.txtMoneyLength.Size = new System.Drawing.Size(46, 20);
            this.txtMoneyLength.TabIndex = 24;
            // 
            // txtTrailerIdBegin
            // 
            this.txtTrailerIdBegin.Location = new System.Drawing.Point(121, 44);
            this.txtTrailerIdBegin.Name = "txtTrailerIdBegin";
            this.txtTrailerIdBegin.Size = new System.Drawing.Size(46, 20);
            this.txtTrailerIdBegin.TabIndex = 45;
            // 
            // txtMoneyBegin
            // 
            this.txtMoneyBegin.Location = new System.Drawing.Point(121, 149);
            this.txtMoneyBegin.Name = "txtMoneyBegin";
            this.txtMoneyBegin.Size = new System.Drawing.Size(46, 20);
            this.txtMoneyBegin.TabIndex = 23;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(184, 100);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(104, 16);
            this.label4.TabIndex = 7;
            this.label4.Text = "Account No. length:";
            // 
            // txtDateLength
            // 
            this.txtDateLength.Location = new System.Drawing.Point(301, 125);
            this.txtDateLength.Name = "txtDateLength";
            this.txtDateLength.ReadOnly = true;
            this.txtDateLength.Size = new System.Drawing.Size(46, 20);
            this.txtDateLength.TabIndex = 28;
            // 
            // lblTrailerId
            // 
            this.lblTrailerId.AutoSize = true;
            this.lblTrailerId.Location = new System.Drawing.Point(373, 48);
            this.lblTrailerId.Name = "lblTrailerId";
            this.lblTrailerId.Size = new System.Drawing.Size(51, 13);
            this.lblTrailerId.TabIndex = 48;
            this.lblTrailerId.Text = "Trailer Id:";
            // 
            // txtDateBegin
            // 
            this.txtDateBegin.Location = new System.Drawing.Point(121, 123);
            this.txtDateBegin.Name = "txtDateBegin";
            this.txtDateBegin.Size = new System.Drawing.Size(46, 20);
            this.txtDateBegin.TabIndex = 27;
            // 
            // txtTrailerId
            // 
            this.txtTrailerId.Location = new System.Drawing.Point(443, 42);
            this.txtTrailerId.MaxLength = 20;
            this.txtTrailerId.Name = "txtTrailerId";
            this.txtTrailerId.Size = new System.Drawing.Size(113, 20);
            this.txtTrailerId.TabIndex = 49;
            this.txtTrailerId.TextChanged += new System.EventHandler(this.txtTrailerId_TextChanged);
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(373, 130);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(73, 16);
            this.label11.TabIndex = 14;
            this.label11.Text = "Date format:";
            // 
            // txtAcctNoBegin
            // 
            this.txtAcctNoBegin.Location = new System.Drawing.Point(121, 96);
            this.txtAcctNoBegin.Name = "txtAcctNoBegin";
            this.txtAcctNoBegin.Size = new System.Drawing.Size(46, 20);
            this.txtAcctNoBegin.TabIndex = 21;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(6, 130);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(100, 16);
            this.label9.TabIndex = 12;
            this.label9.Text = "Date begin:";
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(184, 126);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(100, 16);
            this.label10.TabIndex = 13;
            this.label10.Text = "Date length:";
            // 
            // lblTrailerIdBegin
            // 
            this.lblTrailerIdBegin.AutoSize = true;
            this.lblTrailerIdBegin.Location = new System.Drawing.Point(6, 44);
            this.lblTrailerIdBegin.Name = "lblTrailerIdBegin";
            this.lblTrailerIdBegin.Size = new System.Drawing.Size(80, 13);
            this.lblTrailerIdBegin.TabIndex = 44;
            this.lblTrailerIdBegin.Text = "Trailer Id begin:";
            // 
            // lblTrailerMoneyLength
            // 
            this.lblTrailerMoneyLength.Location = new System.Drawing.Point(184, 71);
            this.lblTrailerMoneyLength.Name = "lblTrailerMoneyLength";
            this.lblTrailerMoneyLength.Size = new System.Drawing.Size(111, 16);
            this.lblTrailerMoneyLength.TabIndex = 17;
            this.lblTrailerMoneyLength.Text = "Trailer money length:";
            // 
            // lblHeaderId
            // 
            this.lblHeaderId.AutoSize = true;
            this.lblHeaderId.Location = new System.Drawing.Point(373, 21);
            this.lblHeaderId.Name = "lblHeaderId";
            this.lblHeaderId.Size = new System.Drawing.Size(57, 13);
            this.lblHeaderId.TabIndex = 42;
            this.lblHeaderId.Text = "Header Id:";
            // 
            // txtHeaderId
            // 
            this.txtHeaderId.Location = new System.Drawing.Point(443, 16);
            this.txtHeaderId.MaxLength = 20;
            this.txtHeaderId.Name = "txtHeaderId";
            this.txtHeaderId.Size = new System.Drawing.Size(113, 20);
            this.txtHeaderId.TabIndex = 43;
            this.txtHeaderId.TextChanged += new System.EventHandler(this.txtHeaderId_TextChanged);
            // 
            // txtHeaderIdBegin
            // 
            this.txtHeaderIdBegin.Location = new System.Drawing.Point(121, 17);
            this.txtHeaderIdBegin.Name = "txtHeaderIdBegin";
            this.txtHeaderIdBegin.Size = new System.Drawing.Size(46, 20);
            this.txtHeaderIdBegin.TabIndex = 38;
            // 
            // lblTrailerMoneyBegin
            // 
            this.lblTrailerMoneyBegin.Location = new System.Drawing.Point(6, 69);
            this.lblTrailerMoneyBegin.Name = "lblTrailerMoneyBegin";
            this.lblTrailerMoneyBegin.Size = new System.Drawing.Size(113, 18);
            this.lblTrailerMoneyBegin.TabIndex = 16;
            this.lblTrailerMoneyBegin.Text = "Trailer money begin:";
            // 
            // txtAcctNoLength
            // 
            this.txtAcctNoLength.Location = new System.Drawing.Point(301, 96);
            this.txtAcctNoLength.Name = "txtAcctNoLength";
            this.txtAcctNoLength.Size = new System.Drawing.Size(46, 20);
            this.txtAcctNoLength.TabIndex = 22;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // PaymentFileDefn
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(768, 438);
            this.Controls.Add(this.groupBox1);
            this.Location = new System.Drawing.Point(8, 0);
            this.Name = "PaymentFileDefn";
            this.Text = "Payment File Definition";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grpTransType.ResumeLayout(false);
            this.grpTransType.PerformLayout();
            this.grpFileType.ResumeLayout(false);
            this.grpFileType.PerformLayout();
            this.grpFixedFile.ResumeLayout(false);
            this.grpFixedFile.PerformLayout();
            this.grpDelimitedFile.ResumeLayout(false);
            this.grpDelimitedFile.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numNoOfCols)).EndInit();
            this.grpBatch.ResumeLayout(false);
            this.grpBatch.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion


        private void InitialiseStaticData()
        {
            try
            {
                // Get Payment Definition  - StorderControl
                dsDefn = PaymentManager.GetDefinition(out error);
                if (error.Length > 0)
                    ShowError(error);
                else
                {
                    // Get Payment Methods  - Code
                    pmDefn = PaymentManager.GetPayMethod("FPM", "L", TN.Code, out error);

                    if (error.Length > 0)
                        ShowError(error);
                    else
                    {

                        drpSourceName.DataSource = (DataTable)dsDefn.Tables[TN.StorderControl];
                        drpSourceName.DisplayMember = CN.BankName;
                        drpSourceName.ValueMember = CN.Paymentmethod;

                        drpPayMethod.DataSource = (DataTable)pmDefn.Tables[TN.Code];
                        drpPayMethod.DisplayMember = CN.CodeDescript;
                        drpPayMethod.ValueMember = CN.Code;

                        //IP - 24/08/10 - CR1092 - COASTER to CoSACS Enhancements
                       
                        drpDelimiter.DataSource = (DataTable)dsDefn.Tables[TN.StorderDelimiters];
                        drpDelimiter.DisplayMember = CN.Delimiter;


                        rbFixedFile.Checked = true;
                        rbIsPayment.Checked = true;     //IP - 03/09/10 - CR1112 - Tallyman Interest Charges

                        cbHasHeader.Checked = false;
                        cbHasTrailer.Checked = false;

                        //IP - 25/08/10 - CR1092 - Only enabled once value > 0 entered.
                        drpAcctNoCol.Enabled = false;
                        drpDateCol.Enabled = false;
                        drpMoneyCol.Enabled = false;

                        ClearDetails();
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }


        private void btnSave_Click(object sender, System.EventArgs e)
        {

            try
            {
                Function = "btnSave_Click";
                Wait();

                if (ValidDefault())
                {
                    //	string empTypeStr = (string)drpEmpCatCommBasis.SelectedItem;
                    //	int index = empTypeStr.IndexOf(":");
                    //	string empType = empTypeStr.Substring(0, index - 1);
                    txtSourceName.Text = txtSourceName.Text.ToUpper();
                    txtFileExt.Text = txtFileExt.Text.ToUpper();
                    int decPoint = 0;
                    int hasHead = cbHasHeader.Checked ? (short)1 : (short)0;
                    int hasTrail = cbHasTrailer.Checked ? (short)1 : (short)0;
                    bool isBatch = Convert.ToBoolean(cbIsBatch.Checked);        //IP - 20/08/10 - CR1092 - COASTER to CoSACS Enhancements
                    bool batchHeaderHasTotal = Convert.ToBoolean(cbBatchHeaderHasTotal.Checked); //IP - 20/08/10 - CR1092 - COASTER to CoSACS Enhancements
                    bool isDelimited = Convert.ToBoolean(rbDelimitedFile.Checked);    //IP - 25/08/10 - CR1092 - COASTER to CoSACS Enhancements
                    string dateformat = string.Empty;        //IP - 25/08/10 - CR1092 - COASTER to CoSACS Enhancements
                    int dateLength = 0;     //IP - 26/08/10 - CR1092 - COASTER to CoSACS Enhancements

                    //IP - 25/08/10 - CR1092 - COASTER to CoSACS Enhancements - Set variables based on either Fixed file or Delimited file.
                    if (rbDelimitedFile.Checked)
                    {
                        dateformat = drpDelimitedDateFormat.Text;
                        dateLength = Convert.ToInt16(txtDelimiterDateLength.Text);
                        decPoint = cbDelimitedHasDecimal.Checked ? (short)1 : (short)0;
                    }
                    else
                    {
                        dateformat = drpDateFormat.Text;
                        dateLength = Convert.ToInt16(txtDateLength.Text);
                        decPoint = cbHasDecimal.Checked ? (short)1 : (short)0;
                    }

                    int payType = 0;
                    //	set payType to payment method code 
                    string payMethod = drpPayMethod.Text;

                    foreach (DataTable pm in pmDefn.Tables)
                    {
                        foreach (DataRow pmrow in pm.Rows)
                        {
                            if (payMethod == (string)pmrow[CN.CodeDescript])
                            {
                                payType = (Convert.ToInt16(pmrow[CN.Code]));
                            }
                        }
                    }

                    PaymentManager.SavePaymentDefinition(txtSourceName.Text, txtFileExt.Text,
                        (Convert.ToInt16(txtAcctNoBegin.Text)), (Convert.ToInt16(txtAcctNoLength.Text)),
                        (Convert.ToInt16(txtMoneyBegin.Text)), (Convert.ToInt16(txtMoneyLength.Text)),
                        decPoint, hasHead, (Convert.ToInt16(txtDateBegin.Text)),
                        dateLength, dateformat, (Convert.ToInt16(txtTrailerBegin.Text)),
                        (Convert.ToInt16(txtTrailerLength.Text)), payType, hasTrail,
                        Convert.ToInt16(txtHeaderIdBegin.Text), Convert.ToInt16(txtHeaderIdLength.Text), txtHeaderId.Text,                  //IP - 12/08/10 - CR1092 - COASTER to CoSACS Enhancements
                        Convert.ToInt16(txtTrailerIdBegin.Text), Convert.ToInt16(txtTrailerIdLength.Text), txtTrailerId.Text,
                        isBatch, Convert.ToInt16(txtBatchHeaderIdBegin.Text), Convert.ToInt16(txtBatchHeaderIdLength.Text), txtBatchHeaderId.Text, //IP - 20/08/10 - CR1092 - COASTER to CoSACS Enhancements
                        batchHeaderHasTotal, Convert.ToInt16(txtBatchHeaderMoneyBegin.Text), Convert.ToInt16(txtBatchHeaderMoneyLength.Text),
                        Convert.ToInt16(txtBatchTrailerIdBegin.Text), Convert.ToInt16(txtBatchTrailerIdLength.Text), txtBatchTrailerId.Text,    //IP - 03/09/10 - CR1092 - COASTER to CoSACS Enhancements
                        isDelimited, drpDelimiter.Text, Convert.ToInt16(numNoOfCols.Value), drpAcctNoCol.Text, drpDateCol.Text, drpMoneyCol.Text, //IP - 25/08/10 - CR1092 - COASTER to CoSACS Enhancements
                        rbIsInterest.Checked,                                                                                                     //IP - 03/09/10 - CR1112 - Tallyman Interest Charges
                        out _errorTxt);

                    if (_errorTxt.Length > 0) ShowInfo(_errorTxt);
                    else InitialiseStaticData();
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

        // source name selected - Load details

        //private void drpSourceName_selected(object sender, System.EventArgs e)
        //{
            //try
            //{
            //    Wait();
            //    this.ClearError();

            //    foreach (DataTable defn in dsDefn.Tables)
            //    {
            //        foreach (DataRow row in defn.Rows)
            //        {
            //            if (drpSourceName.Text == (string)row[CN.BankName])
            //            {

            //                txtSourceName.Text = (string)row[CN.BankName];
            //                txtFileExt.Text = (string)row[CN.FileName];
            //                txtAcctNoBegin.Text = row[CN.AcctNoBegin].ToString();
            //                txtAcctNoLength.Text = row[CN.AcctNoLength].ToString();
            //                txtMoneyBegin.Text = row[CN.MoneyBegin].ToString();
            //                txtMoneyLength.Text = row[CN.MoneyLength].ToString();
            //                if (Convert.ToInt16(row[CN.MoneyPoint]) == 1)
            //                    cbHasDecimal.Checked = true;
            //                else
            //                    cbHasDecimal.Checked = false;
            //                if (Convert.ToInt16(row[CN.HeadLine]) == 1)
            //                    cbHasHeader.Checked = true;
            //                else
            //                {
            //                    cbHasHeader.Checked = false;
            //                    setHeaderFieldsVisibility(false);   //IP - 31/08/10 - CR1092 
            //                }

            //                txtDateBegin.Text = row[CN.DateBegin].ToString();
            //                txtDateLength.Text = row[CN.DateLength].ToString();
            //                //IP - 13/08/10 - CR1092 - COASTER to CoSACS Enhancements
            //                //drpDateFormat.Items.Clear();
            //                //drpDateFormat.Items.Add("yyyymmdd");
            //                //drpDateFormat.Items.Add("ddmmyyyy");
            //                SetupDateFormat();                                          //IP - 13/08/10 - CR1092 - COASTER to CoSACS Enhancements
            //                drpDateFormat.Text = (string)row[CN.DateFormat];
            //                txtTrailerBegin.Text = row[CN.TrailerBegin].ToString();
            //                txtTrailerLength.Text = row[CN.TrailerLength].ToString();
            //                if (Convert.ToInt16(row[CN.HasTrailer]) == 1)
            //                    cbHasTrailer.Checked = true;
            //                else
            //                {
            //                    cbHasTrailer.Checked = false;

            //                    setTrailerFieldsVisibility(false);  //IP - 31/08/10 - CR1092 

            //                }

            //                //IP - 13/08/10 - CR1092 -  - COASTER to CoSACS Enhancements
            //                txtHeaderIdBegin.Text = Convert.ToString(row[CN.HeaderIdBegin]);
            //                txtHeaderIdLength.Text = Convert.ToString(row[CN.HeaderIdLength]);
            //                txtHeaderId.Text = Convert.ToString(row[CN.HeaderId]);
            //                txtTrailerIdBegin.Text = Convert.ToString(row[CN.TrailerIdBegin]);
            //                txtTrailerIdLength.Text = Convert.ToString(row[CN.TrailerIdLength]);
            //                txtTrailerId.Text = Convert.ToString(row[CN.TrailerId]);

            //                //IP - 13/08/10 - CR1092 - COASTER to CoSACS Enhancements - set the maximum number of characters than can be entered as the Id based on the length of the Id.
            //                if (cbHasHeader.Checked)
            //                {
            //                    txtHeaderId.MaxLength = Convert.ToInt16(txtHeaderIdLength.Text);
            //                }

            //                if (cbHasTrailer.Checked)
            //                {
            //                    txtTrailerId.MaxLength = Convert.ToInt16(txtTrailerIdLength.Text);
            //                }

            //                //IP - 20/08/10 - CR1092 - COASTER to CoSACS Enhancements -  set the maximum number of characters than can be entered as the Id based on the length of the Id.
            //                if (cbIsBatch.Checked)
            //                {
            //                    txtBatchHeaderId.MaxLength = Convert.ToInt16(txtBatchHeaderIdLength.Text);
            //                }

            //                //IP 20/08/10 - CR1092 - COASTER to CoSACS Enhancements 
            //                cbIsBatch.Checked = Convert.ToBoolean(row[CN.IsBatch]);
            //                cbBatchHeaderHasTotal.Checked = Convert.ToBoolean(row[CN.BatchHeaderHasTotal]);
            //                txtBatchHeaderIdBegin.Text = Convert.ToString(row[CN.BatchHeaderIdBegin]);
            //                txtBatchHeaderIdLength.Text = Convert.ToString(row[CN.BatchHeaderIdLength]);
            //                txtBatchHeaderId.Text = Convert.ToString(row[CN.BatchHeaderId]);
            //                txtBatchHeaderMoneyBegin.Text = Convert.ToString(row[CN.BatchHeaderMoneyBegin]);
            //                txtBatchHeaderMoneyLength.Text = Convert.ToString(row[CN.BatchHeaderMoneyLength]);
            //                //IP - 03/09/10 - CR1092 - COASTER to CoSACS Enhancements
            //                txtBatchTrailerIdBegin.Text = Convert.ToString(row[CN.BatchTrailerIdBegin]);
            //                txtBatchTrailerIdLength.Text = Convert.ToString(row[CN.BatchTrailerIdLength]);
            //                txtBatchTrailerId.Text = Convert.ToString(row[CN.BatchTrailerId]);


            //                //IP - 25/08/10 - CR1092 - COASTER to CoSACS Enhancements
            //                if (Convert.ToBoolean(row[CN.IsDelimited]))
            //                {
            //                    rbDelimitedFile.Checked = true;
            //                    drpDelimiter.Text = Convert.ToString(row[CN.Delimiter]);
            //                    drpDelimitedDateFormat.Text = Convert.ToString(row[CN.DateFormat]);
            //                    if (Convert.ToInt16(row[CN.MoneyPoint]) == 1)
            //                    {
            //                        cbDelimitedHasDecimal.Checked = true;
            //                    }
            //                    numNoOfCols.Value = Convert.ToInt16(row[CN.DelimitedNoOfCols]);
            //                    drpAcctNoCol.Text = Convert.ToString(row[CN.DelimitedAcctNoColNo]);
            //                    drpDateCol.Text = Convert.ToString(row[CN.DelimitedDateColNo]);
            //                    drpMoneyCol.Text = Convert.ToString(row[CN.DelimitedMoneyColNo]);

            //                    rbFixedFile.Enabled = false; //Disable as we do not want users changing the definition to Fixed file if defined as Delimited.

            //                }
            //                else
            //                {
            //                    rbFixedFile.Checked = true;

            //                    rbDelimitedFile.Enabled = false;   //Disable as we do not want users changing the definition to Delimited file if defined as Fixed.
            //                }

            //                //IP - 03/09/10 - CR1112 - Tallyman Interest Charges
            //                if (Convert.ToBoolean(row[CN.IsInterest]))
            //                {
            //                    rbIsInterest.Checked = true;
            //                    rbIsPayment.Enabled = false;
            //                }
            //                else
            //                {
            //                    rbIsPayment.Checked = true;
            //                    rbIsInterest.Enabled = false;
            //                }


            //                // Set payment method dropdown
            //                string payMethod = row[CN.Paymentmethod].ToString();
            //                foreach (DataTable pm in pmDefn.Tables)
            //                {
            //                    foreach (DataRow pmrow in pm.Rows)
            //                    {
            //                        if (payMethod == (string)pmrow[CN.Code])
            //                        {
            //                            int i = drpPayMethod.FindStringExact((string)pmrow[CN.CodeDescript]);
            //                            if (i != -1)
            //                                drpPayMethod.SelectedIndex = i;
            //                        }
            //                    }
            //                }
            //                //int payMethodx = Convert.ToInt16 (row); //causes unmonitored error

            //                btnDelete.Enabled = true;
            //                btnClear.Enabled = true;
            //                btnSave.Enabled = true;

            //                //IP - 13/09/10 - CR1092  - Disable fields when existing bank selected.
            //                txtSourceName.Enabled = false;
            //                txtFileExt.Enabled = false;

            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Catch(ex, Function);
            //}
            //finally
            //{
            //    StopWait();
            //}


        //}
        // Clear Details

        private void btnClear_Click(object sender, System.EventArgs e)
        {
            //ClearDetails();
            InitialiseStaticData(); //IP - 14/09/10 - CR1092
       
        }

        // Validate Entries
        private bool ValidDefault()
        {
            bool status = true;

            //IP - 20/08/10 - CR1092 - COASTER to CoSACS Enhancements - File Extension mandatory
            if (txtFileExt.Text.Trim().Length == 0 || txtFileExt.Text.Trim().Length < 3)
            {
                errorProvider1.SetError(this.txtFileExt, GetResource("M_ENTERMANDATORY"));
                status = false;
            }
            else
                errorProvider1.SetError(this.txtFileExt, "");

            //IP - 24/08/10 - CR1092 - Only validate the following for a Fixed File
            if (rbFixedFile.Checked)
            {
                // Account No Begin
                if (!IsStrictNumeric(txtAcctNoBegin.Text) || txtAcctNoBegin.Text.Trim().Length == 0)
                {
                    errorProvider1.SetError(this.txtAcctNoBegin, GetResource("M_NUMERIC"));
                    status = false;
                }
                else errorProvider1.SetError(this.txtAcctNoBegin, "");
                // Account No Length
                if (!IsStrictNumeric(txtAcctNoLength.Text) || txtAcctNoLength.Text.Trim().Length == 0)
                {
                    errorProvider1.SetError(this.txtAcctNoLength, GetResource("M_NUMERIC"));
                    status = false;
                }
                else errorProvider1.SetError(this.txtAcctNoLength, "");

                // Money Begin
                if (!IsStrictNumeric(txtMoneyBegin.Text) || txtMoneyBegin.Text.Trim().Length == 0)
                {
                    errorProvider1.SetError(this.txtMoneyBegin, GetResource("M_NUMERIC"));
                    status = false;
                }
                else errorProvider1.SetError(this.txtMoneyBegin, "");

                // Money Length
                if (!IsStrictNumeric(txtMoneyLength.Text) || txtMoneyLength.Text.Trim().Length == 0)
                {
                    errorProvider1.SetError(this.txtMoneyLength, GetResource("M_NUMERIC"));
                    status = false;
                }
                else errorProvider1.SetError(this.txtMoneyLength, "");

                // Date Begin
                if (!IsStrictNumeric(txtDateBegin.Text) || txtDateBegin.Text.Trim().Length == 0)
                {
                    errorProvider1.SetError(this.txtDateBegin, GetResource("M_NUMERIC"));
                    status = false;
                }
                else errorProvider1.SetError(this.txtDateBegin, "");

                // Date Length
                if (!IsStrictNumeric(txtDateLength.Text) || txtDateLength.Text.Trim().Length == 0)
                {
                    errorProvider1.SetError(this.txtDateLength, GetResource("M_NUMERIC"));
                    status = false;
                }
                else errorProvider1.SetError(this.txtDateLength, "");

                if (cbHasTrailer.Checked)
                {

                    // Trailer Begin
                    if (!IsStrictNumeric(txtTrailerBegin.Text) || txtTrailerBegin.Text.Trim().Length == 0)
                    {
                        errorProvider1.SetError(this.txtTrailerBegin, GetResource("M_NUMERIC"));
                        status = false;
                    }
                    else errorProvider1.SetError(this.txtTrailerBegin, "");

                    // Trailer Length
                    if (!IsStrictNumeric(txtTrailerLength.Text) || txtTrailerLength.Text.Trim().Length == 0)
                    {
                        errorProvider1.SetError(this.txtTrailerLength, GetResource("M_NUMERIC"));
                        status = false;
                    }
                    else errorProvider1.SetError(this.txtTrailerLength, "");

                    //IP - 12/08/10 - CR1092 - COASTER to CoSACS Enhancements
                    //Trailer Id Begin
                    if (!IsNumeric(txtTrailerIdBegin.Text) || txtTrailerIdBegin.Text.Trim().Length == 0 || Convert.ToInt16(txtTrailerIdBegin.Text) == 0)
                    {

                        errorProvider1.SetError(this.txtTrailerIdBegin, GetResource("M_NUMERIC"));
                        status = false;
                    }
                    else errorProvider1.SetError(this.txtTrailerIdBegin, "");

                    //Trailer Id Length
                    if (!IsNumeric(txtTrailerIdLength.Text) || txtTrailerIdLength.Text.Trim().Length == 0 || Convert.ToInt16(txtTrailerIdLength.Text) == 0)
                    {

                        errorProvider1.SetError(this.txtTrailerIdLength, GetResource("M_NUMERIC"));
                        status = false;
                    }
                    else errorProvider1.SetError(this.txtTrailerIdLength, "");

                    //Trailer Id
                    //if (txtTrailerId.Text.Trim().Length == 0) //IP - 13/09/10 - CR1092 - do not trim as a space could be used as an ID.
                    if (txtTrailerId.Text.Length == 0)
                    {
                        errorProvider1.SetError(this.txtTrailerId, GetResource("M_ENTERMANDATORY"));
                        status = false;
                    }
                    else errorProvider1.SetError(this.txtTrailerId, "");

                    //IP - 13/09/10 - CR1092 - COASTER to CoSACS Enhancements
                    if (txtTrailerId.Text.Length != Convert.ToInt16(txtTrailerIdLength.Text))
                    {
                        errorProvider1.SetError(this.txtTrailerId, GetResource("M_PAYMENTFILEDEFNIDLENGTH"));
                        status = false;
                    }
                    else errorProvider1.SetError(this.txtTrailerId, "");
                }
                else
                {
                    errorProvider1.SetError(this.txtTrailerBegin, "");
                    errorProvider1.SetError(this.txtTrailerLength, "");
                    txtTrailerBegin.Text = "0";
                    txtTrailerLength.Text = "0";

                    //IP - 12/08/10 - CR1092 - COASTER to CoSACS Enhancements
                    errorProvider1.SetError(this.txtTrailerIdBegin, "");
                    errorProvider1.SetError(this.txtTrailerIdLength, "");
                    errorProvider1.SetError(this.txtTrailerId, "");

                    txtTrailerIdBegin.Text = "0";
                    txtTrailerIdLength.Text = "0";
                    txtTrailerId.Text = "";
                }

                //IP - 12/08/10 - CR1092 - COASTER to CoSACS Enhancements
                if (cbHasHeader.Checked)
                {

                    //IP - 12/08/10 - CR1092 - COASTER to CoSACS Enhancements
                    //Header Id Begin
                    if (!IsNumeric(txtHeaderIdBegin.Text) || txtHeaderIdBegin.Text.Trim().Length == 0 || Convert.ToInt16(txtHeaderIdBegin.Text) == 0)
                    {

                        errorProvider1.SetError(this.txtHeaderIdBegin, GetResource("M_NUMERIC"));
                        status = false;
                    }
                    else errorProvider1.SetError(this.txtHeaderIdBegin, "");


                    //Header Id Length
                    if (!IsNumeric(txtHeaderIdLength.Text) || txtHeaderIdLength.Text.Trim().Length == 0 || Convert.ToInt16(txtHeaderIdLength.Text) == 0)
                    {

                        errorProvider1.SetError(this.txtHeaderIdLength, GetResource("M_NUMERIC"));
                        status = false;
                    }
                    else errorProvider1.SetError(this.txtHeaderIdLength, "");

                    //Header Id
                    //if (txtHeaderId.Text.Trim().Length == 0) //IP - 13/09/10 - CR1092 - do not trim as a space could be used as an ID.
                    if (txtHeaderId.Text.Length == 0)
                    {
                        errorProvider1.SetError(this.txtHeaderId, GetResource("M_ENTERMANDATORY"));
                        status = false;
                    }
                    else errorProvider1.SetError(this.txtHeaderId, "");

                    //IP - 13/09/10 - CR1092 - COASTER to CoSACS Enhancements
                    if (txtHeaderId.Text.Length != Convert.ToInt16(txtHeaderIdLength.Text))
                    {
                        errorProvider1.SetError(this.txtHeaderId, GetResource("M_PAYMENTFILEDEFNIDLENGTH"));
                        status = false;
                    }
                    else errorProvider1.SetError(this.txtHeaderId, "");
                }
                else
                {
                    //IP - 12/08/10 - CR1092 - COASTER to CoSACS Enhancements
                    errorProvider1.SetError(this.txtHeaderIdBegin, "");
                    errorProvider1.SetError(this.txtHeaderIdLength, "");
                    errorProvider1.SetError(this.txtHeaderId, "");

                    txtHeaderIdBegin.Text = "0";
                    txtHeaderIdLength.Text = "0";
                    txtHeaderId.Text = "";
                }

                //IP - 20/08/10 - CR1092 - COASTER to CoSACS Enhancements
                if (cbIsBatch.Checked)
                {
                    if (!IsNumeric(txtBatchHeaderIdBegin.Text) || txtBatchHeaderIdBegin.Text.Trim().Length == 0 || Convert.ToInt16(txtBatchHeaderIdBegin.Text) == 0)
                    {

                        errorProvider1.SetError(this.txtBatchHeaderIdBegin, GetResource("M_NUMERIC"));
                        status = false;
                    }
                    else errorProvider1.SetError(this.txtBatchHeaderIdBegin, "");

                    if (!IsNumeric(txtBatchHeaderIdLength.Text) || txtBatchHeaderIdLength.Text.Trim().Length == 0 || Convert.ToInt16(txtBatchHeaderIdLength.Text) == 0)
                    {

                        errorProvider1.SetError(this.txtBatchHeaderIdLength, GetResource("M_NUMERIC"));
                        status = false;
                    }
                    else errorProvider1.SetError(this.txtBatchHeaderIdLength, "");


                    if (txtBatchHeaderId.Text.Trim().Length == 0)
                    {
                        errorProvider1.SetError(this.txtBatchHeaderId, GetResource("M_ENTERMANDATORY"));
                        status = false;
                    }
                    else errorProvider1.SetError(this.txtBatchHeaderId, "");

                    //IP - 03/09/10 - CR1092 - COASTER to CoSACS Enhancements
                    if (!IsNumeric(txtBatchTrailerIdBegin.Text) || txtBatchTrailerIdBegin.Text.Trim().Length == 0 || Convert.ToInt16(txtBatchTrailerIdBegin.Text) == 0)
                    {

                        errorProvider1.SetError(this.txtBatchTrailerIdBegin, GetResource("M_NUMERIC"));
                        status = false;
                    }
                    else errorProvider1.SetError(this.txtBatchTrailerIdBegin, "");

                    if (!IsNumeric(txtBatchTrailerIdLength.Text) || txtBatchTrailerIdLength.Text.Trim().Length == 0 || Convert.ToInt16(txtBatchTrailerIdLength.Text) == 0)
                    {

                        errorProvider1.SetError(this.txtBatchTrailerIdLength, GetResource("M_NUMERIC"));
                        status = false;
                    }
                    else errorProvider1.SetError(this.txtBatchTrailerIdLength, "");


                    if (txtBatchTrailerId.Text.Trim().Length == 0)
                    {
                        errorProvider1.SetError(this.txtBatchTrailerId, GetResource("M_ENTERMANDATORY"));
                        status = false;
                    }
                    else errorProvider1.SetError(this.txtBatchTrailerId, "");

                    if (cbBatchHeaderHasTotal.Checked)
                    {
                        if (!IsNumeric(txtBatchHeaderMoneyBegin.Text) || txtBatchHeaderMoneyBegin.Text.Trim().Length == 0 || Convert.ToInt16(txtBatchHeaderMoneyBegin.Text) == 0)
                        {

                            errorProvider1.SetError(this.txtBatchHeaderMoneyBegin, GetResource("M_NUMERIC"));
                            status = false;
                        }
                        else errorProvider1.SetError(this.txtBatchHeaderMoneyBegin, "");

                        if (!IsNumeric(txtBatchHeaderMoneyLength.Text) || txtBatchHeaderMoneyLength.Text.Trim().Length == 0 || Convert.ToInt16(txtBatchHeaderMoneyLength.Text) == 0)
                        {

                            errorProvider1.SetError(this.txtBatchHeaderMoneyLength, GetResource("M_NUMERIC"));
                            status = false;
                        }
                        else errorProvider1.SetError(this.txtBatchHeaderMoneyLength, "");

                    }
                    else
                    {
                        errorProvider1.SetError(this.txtBatchHeaderMoneyBegin, "");
                        errorProvider1.SetError(this.txtBatchHeaderMoneyLength, "");

                        txtBatchHeaderMoneyBegin.Text = "0";
                        txtBatchHeaderMoneyLength.Text = "0";
                    }

                }
                else
                {
                    errorProvider1.SetError(this.txtBatchHeaderIdBegin, "");
                    errorProvider1.SetError(this.txtBatchHeaderIdLength, "");
                    errorProvider1.SetError(this.txtBatchHeaderId, "");

                    errorProvider1.SetError(this.txtBatchTrailerIdBegin, "");
                    errorProvider1.SetError(this.txtBatchTrailerIdLength, "");
                    errorProvider1.SetError(this.txtBatchTrailerId, "");

                    txtBatchHeaderIdBegin.Text = "0";
                    txtBatchHeaderIdLength.Text = "0";
                    txtBatchHeaderId.Text = "";

                    txtBatchTrailerIdBegin.Text = "0";
                    txtBatchTrailerIdLength.Text = "0";
                    txtBatchTrailerId.Text = "";

                    txtBatchHeaderMoneyBegin.Text = "0";
                    txtBatchHeaderMoneyLength.Text = "0";
                }
            }

            //IP - 24/08/10 - CR1092 - COASTER to CoSACS Enhancements
            if (rbDelimitedFile.Checked)
            {
                if (numNoOfCols.Value == 0)
                {
                    errorProvider1.SetError(this.numNoOfCols, GetResource("M_GREATERTHANZERO2"));
                    status = false;
                }
                else errorProvider1.SetError(this.numNoOfCols, "");


                if (drpDelimitedDateFormat.SelectedItem == null)
                {
                    errorProvider1.SetError(this.drpDelimitedDateFormat, GetResource("M_ENTERMANDATORY"));
                    status = false;
                }
                else errorProvider1.SetError(this.drpDelimitedDateFormat, "");

                if (drpAcctNoCol.SelectedItem == null)
                {
                    errorProvider1.SetError(this.drpAcctNoCol, GetResource("M_ENTERMANDATORY"));
                    status = false;
                }
                else if (drpAcctNoCol.Text == drpMoneyCol.Text || drpAcctNoCol.Text == drpDateCol.Text)
                {
                    errorProvider1.SetError(this.drpAcctNoCol, GetResource("M_OPTIONALREADYSELECTED"));
                    status = false;
                }
                else errorProvider1.SetError(this.drpAcctNoCol, "");

                if (drpDateCol.SelectedItem == null)
                {
                    errorProvider1.SetError(this.drpDateCol, GetResource("M_ENTERMANDATORY"));
                    status = false;
                }
                else if (drpDateCol.Text == drpAcctNoCol.Text || drpDateCol.Text == drpMoneyCol.Text)
                {
                    errorProvider1.SetError(this.drpDateCol, GetResource("M_OPTIONALREADYSELECTED"));
                    status = false;
                }
                else errorProvider1.SetError(this.drpDateCol, "");

                if (drpMoneyCol.SelectedItem == null)
                {
                    errorProvider1.SetError(this.drpMoneyCol, GetResource("M_ENTERMANDATORY"));
                    status = false;
                }
                else if (drpMoneyCol.Text == drpAcctNoCol.Text || drpMoneyCol.Text == drpDateCol.Text)
                {
                    errorProvider1.SetError(this.drpMoneyCol, GetResource("M_OPTIONALREADYSELECTED"));
                    status = false;
                }
                else errorProvider1.SetError(this.drpMoneyCol, "");

                if (Convert.ToString(drpDelimiter.SelectedItem) == "")
                {
                    errorProvider1.SetError(this.drpDelimiter, GetResource("M_ENTERMANDATORY"));
                    status = false;
                }
                else errorProvider1.SetError(this.drpDelimiter, "");

            }


            return status;
        }
        // Clear Errors
        private void ClearError()
        {
            errorProvider1.SetError(this.txtAcctNoBegin, "");
            errorProvider1.SetError(this.txtAcctNoLength, "");
            errorProvider1.SetError(this.txtMoneyBegin, "");
            errorProvider1.SetError(this.txtMoneyLength, "");
            errorProvider1.SetError(this.txtDateBegin, "");
            errorProvider1.SetError(this.txtDateLength, "");
            errorProvider1.SetError(this.txtTrailerBegin, "");
            errorProvider1.SetError(this.txtTrailerLength, "");

            //IP - 24/08/10 - CR1092 - COASTER to CoSACS Enhancements
            errorProvider1.SetError(this.txtFileExt, "");
            errorProvider1.SetError(this.txtHeaderIdBegin, "");
            errorProvider1.SetError(this.txtHeaderIdLength, "");
            errorProvider1.SetError(this.txtHeaderId, "");
            errorProvider1.SetError(this.txtTrailerIdBegin, "");
            errorProvider1.SetError(this.txtTrailerIdLength, "");
            errorProvider1.SetError(this.txtTrailerId, "");
            errorProvider1.SetError(this.numNoOfCols, "");
            errorProvider1.SetError(this.drpAcctNoCol, "");
            errorProvider1.SetError(this.drpDateCol, "");
            errorProvider1.SetError(this.drpMoneyCol, "");
            errorProvider1.SetError(this.drpDelimitedDateFormat, "");
            errorProvider1.SetError(this.drpDelimiter, "");

            //IP - 03/09/10 - CR1092 - COASTER to CoSACS Enhancements
            errorProvider1.SetError(this.txtBatchHeaderId, "");
            errorProvider1.SetError(this.txtBatchHeaderIdBegin, "");
            errorProvider1.SetError(this.txtBatchHeaderIdLength, "");
            errorProvider1.SetError(this.txtBatchHeaderMoneyBegin, "");
            errorProvider1.SetError(this.txtBatchHeaderMoneyLength, "");
            errorProvider1.SetError(this.txtBatchTrailerId, "");
            errorProvider1.SetError(this.txtBatchTrailerIdBegin, "");
            errorProvider1.SetError(this.txtBatchTrailerIdLength, "");

        }

        private void btnDelete_Click(object sender, System.EventArgs e)
        {
            try
            {
                Function = "btnDelete_Click";
                Wait();
                if (DialogResult.Yes == ShowInfo("M_DELETECONFIRM", MessageBoxButtons.YesNo))
                {
                    PaymentManager.DeletePaymentDefinition(txtSourceName.Text, out _errorTxt);

                    if (_errorTxt.Length > 0) ShowInfo(_errorTxt);
                    else InitialiseStaticData();
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

        // Clear Details

        private void ClearDetails()
        {

            txtSourceName.Text = "";
            txtFileExt.Text = "";
            txtAcctNoBegin.Text = "";
            txtAcctNoLength.Text = "";
            txtMoneyBegin.Text = "";
            txtMoneyLength.Text = "";
            cbHasDecimal.Checked = false;
            cbHasHeader.Checked = false;
            cbHasHeader.Enabled = true;                 //IP - 14/09/10 - CR1092
            txtDateBegin.Text = "";
            txtDateLength.Text = "";
            //drpDateFormat.Items.Clear();
            //drpDateFormat.Items.Add("yyyymmdd");
            //drpDateFormat.Items.Add("ddmmyyyy");
            SetupDateFormat();                          //IP - 09/08/10 - CR1092 - COASTER to CoSACS Enhancements
            cbHasTrailer.Checked = false;
            cbHasTrailer.Enabled = true;                //IP - 14/09/10 - CR1092
            //IP - 13/08/10 - CR1092 - COASTER to CoSACS Enhancements
            txtTrailerBegin.Text = "";
            txtTrailerLength.Text = "";
            txtHeaderIdBegin.Text = "";
            txtHeaderIdLength.Text = "";
            txtHeaderId.Text = "";
            txtTrailerIdBegin.Text = "";
            txtTrailerIdLength.Text = "";
            txtTrailerId.Text = "";

            //IP - 20/08/10 - CR1092 - COASTER to CoSACS Enhancements
            cbIsBatch.Checked = false;
            cbBatchHeaderHasTotal.Checked = false;
            txtBatchHeaderIdBegin.Text = "";
            txtBatchHeaderIdLength.Text = "";
            txtBatchHeaderId.Text = "";
            txtBatchHeaderMoneyBegin.Text = "";
            txtBatchHeaderMoneyLength.Text = "";
            //IP - 03/09/10 - CR1092 - COASTER to CoSACS Enhancements
            txtBatchTrailerIdBegin.Text = "";
            txtBatchTrailerIdLength.Text = "";
            txtBatchTrailerId.Text = "";

            //IP - 24/08/10 - CR1092 - COASTER to CoSACS Enhancements
            rbDelimitedFile.Checked = false;
            numNoOfCols.Value = 0;
            cbDelimitedHasDecimal.Checked = false;

            // Default Payment Method to Standing Order
            int i = drpPayMethod.FindStringExact("Standing Order");
            if (i != -1)
                drpPayMethod.SelectedIndex = i;

            this.ClearError();
            btnDelete.Enabled = false;
            btnClear.Enabled = false;
            btnSave.Enabled = false;

            //IP - 25/08/10 - CR1092 - COASTER to CoSACS Enhancements
            rbFixedFile.Enabled = true;
            rbDelimitedFile.Enabled = true;
            drpAcctNoCol.Enabled = false;
            drpDateCol.Enabled = false;
            drpMoneyCol.Enabled = false;

            //IP - 03/09/10 - CR1112 - Tallyman Interest Charges
            rbIsPayment.Enabled = true;
            rbIsInterest.Enabled = true;

            //IP - 13/09/10 - CR1092 - Enable source and extension fields when screen cleared
            txtSourceName.Enabled = true;
            txtFileExt.Enabled = true;

            //IP - 14/09/10 - CR1092 - Enable the Existing Source drop down
            drpSourceName.Enabled = true;
            drpSourceName.SelectedIndex = -1;
     
        }

        // Enable buttons if Source Name not blank
        private void txtSourceNameChanged(object sender, System.EventArgs e)
        {
            if (txtSourceName.Text.Trim().Length != 0)
            {
                btnDelete.Enabled = true;
                btnClear.Enabled = true;
                btnSave.Enabled = true;
                txtSourceName.Text = txtSourceName.Text.Trim();
                txtSourceName.Text = txtSourceName.Text.Replace(" ", "");

               
            }
            else
            {
                btnDelete.Enabled = false;
                btnClear.Enabled = false;
                btnSave.Enabled = false;
            }


        }

        //IP - 09/08/10 - CR1092 - COASTER to CoSACS Enhancements
        private void SetupDateFormat()
        {

            drpDateFormat.Items.Clear();
            drpDelimitedDateFormat.Items.Clear();

            drpDateFormat.Items.Add("ddmmyy");
            drpDelimitedDateFormat.Items.Add("ddmmyy");

            drpDateFormat.Items.Add("dd-mm-yy");
            drpDelimitedDateFormat.Items.Add("dd-mm-yy");

            drpDateFormat.Items.Add("dd/mm/yy");
            drpDelimitedDateFormat.Items.Add("dd/mm/yy");

            drpDateFormat.Items.Add("ddmmyyyy");
            drpDelimitedDateFormat.Items.Add("ddmmyyyy");
            drpDateFormat.Items.Add("dd-mm-yyyy");
            drpDelimitedDateFormat.Items.Add("dd-mm-yyyy");
            drpDateFormat.Items.Add("dd/mm/yyyy");
            drpDelimitedDateFormat.Items.Add("dd/mm/yyyy");

            drpDateFormat.Items.Add("yymmdd");
            drpDelimitedDateFormat.Items.Add("yymmdd");
            drpDateFormat.Items.Add("yy-mm-dd");
            drpDelimitedDateFormat.Items.Add("yy-mm-dd");
            drpDateFormat.Items.Add("yy/mm/dd");
            drpDelimitedDateFormat.Items.Add("yy/mm/dd");

            drpDateFormat.Items.Add("yyyymmdd");
            drpDelimitedDateFormat.Items.Add("yyyymmdd");
            drpDateFormat.Items.Add("yyyy-mm-dd");
            drpDelimitedDateFormat.Items.Add("yyyy-mm-dd");
            drpDateFormat.Items.Add("yyyy/mm/dd");
            drpDelimitedDateFormat.Items.Add("yyyy/mm/dd");

        }

        //IP - 09/08/10 - CR1092 - COASTER to CoSACS Enhancements
        private void drpDateFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Convert.ToString(drpDateFormat.Text) == "ddmmyy"
                    || Convert.ToString(drpDateFormat.Text) == "yymmdd")
            {
                txtDateLength.Text = "6";
            }
            else if (Convert.ToString(drpDateFormat.Text) == "dd-mm-yy"
                    || Convert.ToString(drpDateFormat.Text) == "dd/mm/yy"
                    || Convert.ToString(drpDateFormat.Text) == "ddmmyyyy"
                    || Convert.ToString(drpDateFormat.Text) == "yy-mm-dd"
                    || Convert.ToString(drpDateFormat.Text) == "yy/mm/dd"
                    || Convert.ToString(drpDateFormat.Text) == "yyyymmdd")
            {
                txtDateLength.Text = "8";
            }
            else if (Convert.ToString(drpDateFormat.Text) == "dd-mm-yyyy"
                    || Convert.ToString(drpDateFormat.Text) == "dd/mm/yyyy"
                    || Convert.ToString(drpDateFormat.Text) == "yyyy-mm-dd"
                    || Convert.ToString(drpDateFormat.Text) == "yyyy/mm/dd")
            {
                txtDateLength.Text = "10";
            }

        }

        //IP - 12/08/10 - CR1092 - COASTER to CoSACS Enhancements
        private void cbHasHeader_CheckedChanged(object sender, EventArgs e)
        {

            if (cbHasHeader.Checked)
            {
                setHeaderFieldsVisibility(true);    //IP - 31/08/10 - CR1092
            }
            else
            {
                setHeaderFieldsVisibility(false);   //IP - 31/08/10 - CR1092

                txtHeaderIdBegin.Text = "0";
                txtHeaderIdLength.Text = "0";
                txtHeaderId.Text = "";
            }
        }

        //IP - 12/08/10 - CR1092 - COASTER to CoSACS Enhancements
        private void cbHasTrailer_CheckedChanged(object sender, EventArgs e)
        {
            if (cbHasTrailer.Checked)
            {

                setTrailerFieldsVisibility(true); //IP - 31/08/10 - CR1092
            }
            else
            {
                setTrailerFieldsVisibility(false);  //IP - 31/08/10 - CR1092

                txtTrailerIdBegin.Text = "0";
                txtTrailerIdLength.Text = "0";
                txtTrailerId.Text = "";
            }
        }

        //IP - 13/08/10 - CR1092 - COASTER to CoSACS Enhancements
        private void txtHeaderIdLength_TextChanged(object sender, EventArgs e)
        {
            errorProvider1.SetError(this.txtHeaderIdLength, "");
            txtHeaderId.Text = "";

            //Set the maximum number of characters that can be entered into the Id field based on the length of the Id.
            if (!IsNumeric(txtHeaderIdLength.Text) || txtHeaderIdLength.Text.Trim().Length == 0 || Convert.ToInt16(txtHeaderIdLength.Text) == 0)
            {
                errorProvider1.SetError(this.txtHeaderIdLength, GetResource("M_NUMERIC"));
                txtHeaderId.Text = "";
                txtHeaderId.ReadOnly = true;

            }
            else
            {
                txtHeaderId.MaxLength = Convert.ToInt16(txtHeaderIdLength.Text);
                txtHeaderId.ReadOnly = false;
            }

        }

        //IP - 13/08/10 - CR1092 - COASTER to CoSACS Enhancements
        private void txtTrailerIdLength_TextChanged(object sender, EventArgs e)
        {
            errorProvider1.SetError(this.txtTrailerIdLength, "");
            txtTrailerId.Text = "";

            //Set the maximum number of characters that can be entered into the Id field based on the length of the Id.
            if (!IsNumeric(txtTrailerIdLength.Text) || txtTrailerIdLength.Text.Trim().Length == 0 || Convert.ToInt16(txtTrailerIdLength.Text) == 0)
            {
                errorProvider1.SetError(this.txtTrailerIdLength, GetResource("M_NUMERIC"));
                txtTrailerId.Text = "";
                txtTrailerId.ReadOnly = true;
            }
            else
            {
                txtTrailerId.MaxLength = Convert.ToInt16(txtTrailerIdLength.Text);
                txtTrailerId.ReadOnly = false;

            }

        }

        //IP - 13/08/10 - CR1092 - COASTER to CoSACS Enhancements
        private void txtHeaderId_TextChanged(object sender, EventArgs e)
        {
            txtHeaderId.Text = txtHeaderId.Text.ToUpper();
        }

        //IP - 13/08/10 - CR1092 - COASTER to CoSACS Enhancements
        private void txtTrailerId_TextChanged(object sender, EventArgs e)
        {
            txtTrailerId.Text = txtTrailerId.Text.ToUpper();
        }

        //IP - 20/08/10 - CR1092 - COASTER to CoSACS Enhancements
        private void cbIsBatch_CheckedChanged(object sender, EventArgs e)
        {
            if (cbIsBatch.Checked)
            {
                grpBatch.Visible = true;
            }
            else
            {
                grpBatch.Visible = false;

                txtBatchHeaderIdBegin.Text = "0";
                txtBatchHeaderIdLength.Text = "0";
                txtBatchHeaderId.Text = "";
                txtBatchHeaderMoneyBegin.Text = "0";
                txtBatchHeaderMoneyLength.Text = "0";
                cbBatchHeaderHasTotal.Checked = false;
                //IP - 03/09/10 - CR1092
                txtBatchTrailerIdBegin.Text = "0";
                txtBatchTrailerIdLength.Text = "0";
                txtBatchTrailerId.Text = "";
            }
        }

        //IP - 20/08/10 - CR1092 - COASTER to CoSACS Enhancements
        private void cbBactHeaderHasTotal_CheckedChanged(object sender, EventArgs e)
        {
            if (cbBatchHeaderHasTotal.Checked)
            {
                lblBatchHeaderMoneyBegin.Visible = true;
                lblBatchHeaderMoneyLength.Visible = true;

                txtBatchHeaderMoneyBegin.Visible = true;
                txtBatchHeaderMoneyLength.Visible = true;
            }
            else
            {
                lblBatchHeaderMoneyBegin.Visible = false;
                lblBatchHeaderMoneyLength.Visible = false;

                txtBatchHeaderMoneyBegin.Visible = false;
                txtBatchHeaderMoneyLength.Visible = false;

                txtBatchHeaderMoneyBegin.Text = "0";
                txtBatchHeaderMoneyLength.Text = "0";
            }
        }

        //IP - 20/08/10 - CR1092 - COASTER to CoSACS Enhancements
        private void txtBatchHeaderIdLength_TextChanged(object sender, EventArgs e)
        {
            errorProvider1.SetError(this.txtBatchHeaderIdLength, "");
            txtBatchHeaderId.Text = "";

            //Set the maximum number of characters that can be entered into the Id field based on the length of the Id.
            if (!IsNumeric(txtBatchHeaderIdLength.Text) || txtBatchHeaderIdLength.Text.Trim().Length == 0 || Convert.ToInt16(txtBatchHeaderIdLength.Text) == 0)
            {
                errorProvider1.SetError(this.txtBatchHeaderIdLength, GetResource("M_NUMERIC"));
                txtBatchHeaderId.Text = "";
                txtBatchHeaderId.ReadOnly = true;

            }
            else
            {
                txtBatchHeaderId.MaxLength = Convert.ToInt16(txtBatchHeaderIdLength.Text);
                txtBatchHeaderId.ReadOnly = false;
            }
        }

        //IP - 20/08/10 - CR1092 - COASTER to CoSACS Enhancements
        private void txtBatchHeaderId_TextChanged(object sender, EventArgs e)
        {
            txtBatchHeaderId.Text = txtBatchHeaderId.Text.ToUpper();
        }



        //IP - 24/08/10 - CR1092 - COASTER to CoSACS Enhancements
        public void ClearFixedGroupBoxControls()
        {
            txtHeaderIdBegin.Text = "0";
            txtHeaderIdLength.Text = "0";
            txtHeaderId.Text = "";

            txtTrailerIdBegin.Text = "0";
            txtTrailerIdLength.Text = "0";
            txtTrailerId.Text = "";

            txtMoneyBegin.Text = "0";
            txtMoneyLength.Text = "0";

            txtTrailerBegin.Text = "0";
            txtTrailerLength.Text = "0";

            txtAcctNoBegin.Text = "0";
            txtAcctNoLength.Text = "0";

            txtDateBegin.Text = "0";
            txtDateLength.Text = "0";

            txtMoneyBegin.Text = "0";
            txtMoneyLength.Text = "0";

            cbIsBatch.Checked = false;

            txtBatchHeaderIdBegin.Text = "0";
            txtBatchHeaderIdLength.Text = "0";
            txtBatchHeaderId.Text = "";

            //IP - 14/09/10 - CR1092 - UAT5.4 - UAT(11)
            txtBatchTrailerIdBegin.Text = "0";
            txtBatchTrailerIdLength.Text = "0";
            txtBatchTrailerId.Text = "";

            txtBatchHeaderMoneyBegin.Text = "0";
            txtBatchHeaderMoneyLength.Text = "0";
            cbBatchHeaderHasTotal.Checked = false;

            //IP - 14/09/10 - CR1092 - If delimited checked then disable the File has Header/Trailer checkbox's
            cbHasHeader.Checked = false;
            cbHasHeader.Enabled = false;
            cbHasTrailer.Checked = false;
            cbHasTrailer.Enabled = false;
        }

        //IP - 24/08/10 - CR1092 - COASTER to CoSACS Enhancements
        public void ClearDelimitedGroupBoxControls()
        {
            numNoOfCols.Value = 0;
            drpDelimiter.Text = "";
            drpAcctNoCol.Text = "";
            drpDateCol.Text = "";
            drpMoneyCol.Text = "";

        }

        private void rbFixedFile_CheckedChanged(object sender, EventArgs e)
        {
            if (rbFixedFile.Checked)
            {
                //IP - 14/09/10 - CR1092 - If delimited checked then disable the File has Header/Trailer checkbox's
                cbHasHeader.Enabled = true;
                cbHasTrailer.Enabled = true;

                grpFixedFile.Visible = true;
                grpDelimitedFile.Visible = false;
                ClearDelimitedGroupBoxControls();
            }

        }

        private void rbDelimitedFile_CheckedChanged(object sender, EventArgs e)
        {
            if (rbDelimitedFile.Checked)
            {
                grpFixedFile.Visible = false;
                grpDelimitedFile.Visible = true;
                ClearFixedGroupBoxControls();
            }

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            string Column = "Column";

            int NoOfCols = Convert.ToInt16(numNoOfCols.Value);

            drpAcctNoCol.Items.Clear();
            drpDateCol.Items.Clear();
            drpMoneyCol.Items.Clear();

            //IP - 25/08/10 - CR1092 - Only enable the drop downs if a value > 0 entered.
            if (NoOfCols > 0)
            {
                drpAcctNoCol.Enabled = true;
                drpDateCol.Enabled = true;
                drpMoneyCol.Enabled = true;

                for (int i = 1; i <= NoOfCols; i++)
                {

                    drpAcctNoCol.Items.Add(Column + Convert.ToString(i));
                    drpDateCol.Items.Add(Column + Convert.ToString(i));
                    drpMoneyCol.Items.Add(Column + Convert.ToString(i));
                }
            }
            else
            {
                drpAcctNoCol.Enabled = false;
                drpDateCol.Enabled = false;
                drpMoneyCol.Enabled = false;
            }

        }

        //IP - 26/08/10 - CR1092 - COASTER to CoSACS Enhancements
        private void drpDelimitedDateFormat_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (Convert.ToString(drpDelimitedDateFormat.Text) == "ddmmyy"
                    || Convert.ToString(drpDelimitedDateFormat.Text) == "yymmdd")
            {
                txtDelimiterDateLength.Text = "6";
            }
            else if (Convert.ToString(drpDelimitedDateFormat.Text) == "dd-mm-yy"
                    || Convert.ToString(drpDelimitedDateFormat.Text) == "dd/mm/yy"
                    || Convert.ToString(drpDelimitedDateFormat.Text) == "ddmmyyyy"
                    || Convert.ToString(drpDelimitedDateFormat.Text) == "yy-mm-dd"
                    || Convert.ToString(drpDelimitedDateFormat.Text) == "yy/mm/dd"
                    || Convert.ToString(drpDelimitedDateFormat.Text) == "yyyymmdd")
            {
                txtDelimiterDateLength.Text = "8";
            }
            else if (Convert.ToString(drpDelimitedDateFormat.Text) == "dd-mm-yyyy"
                    || Convert.ToString(drpDelimitedDateFormat.Text) == "dd/mm/yyyy"
                    || Convert.ToString(drpDelimitedDateFormat.Text) == "yyyy-mm-dd"
                    || Convert.ToString(drpDelimitedDateFormat.Text) == "yyyy/mm/dd")
            {
                txtDelimiterDateLength.Text = "10";
            }
        }

        //IP - 31/08/10 - CR1092 - COASTER to CoSACS Enhancements
        private void txtFileExt_Leave(object sender, EventArgs e)
        {

            int index = -1;

            index = txtFileExt.Text.IndexOf('.');

            if (index >= 0)
            {
                txtFileExt.Text = txtFileExt.Text.Replace('.', ' ').Remove(index, 1);
            }

        }

        //IP - 31/08/10 - CR1092 - COASTER to CoSACS Enhancements
        private void setTrailerFieldsVisibility(bool visibility)
        {

            lblTrailerId.Visible = visibility;
            lblTrailerIdBegin.Visible = visibility;
            lblTrailerIdLength.Visible = visibility;
            lblTrailerMoneyBegin.Visible = visibility;
            lblTrailerMoneyLength.Visible = visibility;

            txtTrailerIdBegin.Visible = visibility;
            txtTrailerIdLength.Visible = visibility;
            txtTrailerId.Visible = visibility;
            txtTrailerBegin.Visible = visibility;
            txtTrailerLength.Visible = visibility;
        }

        //IP - 31/08/10 - CR1092 - COASTER to CoSACS Enhancements
        private void setHeaderFieldsVisibility(bool visibility)
        {
            lblHeaderIdBegin.Visible = visibility;
            lblHeaderIdLength.Visible = visibility;
            lblHeaderId.Visible = visibility;

            txtHeaderIdBegin.Visible = visibility;
            txtHeaderIdLength.Visible = visibility;
            txtHeaderId.Visible = visibility;
        }

        //IP - 03/09/10 - CR1092 - COASTER to CoSACS Enhancements
        private void txtBatchTrailerIdLength_TextChanged(object sender, EventArgs e)
        {
            errorProvider1.SetError(this.txtBatchTrailerIdLength, "");
            txtBatchTrailerId.Text = "";

            //Set the maximum number of characters that can be entered into the Id field based on the length of the Id.
            if (!IsNumeric(txtBatchTrailerIdLength.Text) || txtBatchTrailerIdLength.Text.Trim().Length == 0 || Convert.ToInt16(txtBatchTrailerIdLength.Text) == 0)
            {
                errorProvider1.SetError(this.txtBatchTrailerIdLength, GetResource("M_NUMERIC"));
                txtBatchTrailerId.Text = "";
                txtBatchTrailerId.ReadOnly = true;

            }
            else
            {
                txtBatchTrailerId.MaxLength = Convert.ToInt16(txtBatchTrailerIdLength.Text);
                txtBatchTrailerId.ReadOnly = false;
            }
        }


        private void drpSourceName_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Wait();
                this.ClearError();

                if (drpSourceName.SelectedIndex >= 0) //IP - 14/09/10 - CR1092
                {
                    //foreach (DataTable defn in dsDefn.Tables) //IP - 16/09/10 - CR1092
                    //{
                    foreach (DataRow row in dsDefn.Tables[TN.StorderControl].Rows) //IP - 16/09/10 - CR1092
                        {
                            if (drpSourceName.Text == (string)row[CN.BankName])
                            {

                                txtSourceName.Text = (string)row[CN.BankName];
                                txtFileExt.Text = (string)row[CN.FileName];
                                txtAcctNoBegin.Text = row[CN.AcctNoBegin].ToString();
                                txtAcctNoLength.Text = row[CN.AcctNoLength].ToString();
                                txtMoneyBegin.Text = row[CN.MoneyBegin].ToString();
                                txtMoneyLength.Text = row[CN.MoneyLength].ToString();
                                if (Convert.ToInt16(row[CN.MoneyPoint]) == 1)
                                    cbHasDecimal.Checked = true;
                                else
                                    cbHasDecimal.Checked = false;
                                if (Convert.ToInt16(row[CN.HeadLine]) == 1)
                                    cbHasHeader.Checked = true;
                                else
                                {
                                    cbHasHeader.Checked = false;
                                    setHeaderFieldsVisibility(false);   //IP - 31/08/10 - CR1092 
                                }

                                txtDateBegin.Text = row[CN.DateBegin].ToString();
                                txtDateLength.Text = row[CN.DateLength].ToString();
                                //IP - 13/08/10 - CR1092 - COASTER to CoSACS Enhancements
                                //drpDateFormat.Items.Clear();
                                //drpDateFormat.Items.Add("yyyymmdd");
                                //drpDateFormat.Items.Add("ddmmyyyy");
                                SetupDateFormat();                                          //IP - 13/08/10 - CR1092 - COASTER to CoSACS Enhancements
                                drpDateFormat.Text = (string)row[CN.DateFormat];
                                txtTrailerBegin.Text = row[CN.TrailerBegin].ToString();
                                txtTrailerLength.Text = row[CN.TrailerLength].ToString();
                                if (Convert.ToInt16(row[CN.HasTrailer]) == 1)
                                    cbHasTrailer.Checked = true;
                                else
                                {
                                    cbHasTrailer.Checked = false;

                                    setTrailerFieldsVisibility(false);  //IP - 31/08/10 - CR1092 

                                }

                                //IP - 13/08/10 - CR1092 -  - COASTER to CoSACS Enhancements
                                txtHeaderIdBegin.Text = Convert.ToString(row[CN.HeaderIdBegin]);
                                txtHeaderIdLength.Text = Convert.ToString(row[CN.HeaderIdLength]);
                                txtHeaderId.Text = Convert.ToString(row[CN.HeaderId]);
                                txtTrailerIdBegin.Text = Convert.ToString(row[CN.TrailerIdBegin]);
                                txtTrailerIdLength.Text = Convert.ToString(row[CN.TrailerIdLength]);
                                txtTrailerId.Text = Convert.ToString(row[CN.TrailerId]);

                                //IP - 13/08/10 - CR1092 - COASTER to CoSACS Enhancements - set the maximum number of characters than can be entered as the Id based on the length of the Id.
                                if (cbHasHeader.Checked)
                                {
                                    txtHeaderId.MaxLength = Convert.ToInt16(txtHeaderIdLength.Text);
                                }

                                if (cbHasTrailer.Checked)
                                {
                                    txtTrailerId.MaxLength = Convert.ToInt16(txtTrailerIdLength.Text);
                                }

                                //IP - 20/08/10 - CR1092 - COASTER to CoSACS Enhancements -  set the maximum number of characters than can be entered as the Id based on the length of the Id.
                                if (cbIsBatch.Checked)
                                {
                                    txtBatchHeaderId.MaxLength = Convert.ToInt16(txtBatchHeaderIdLength.Text);
                                }

                                //IP 20/08/10 - CR1092 - COASTER to CoSACS Enhancements 
                                cbIsBatch.Checked = Convert.ToBoolean(row[CN.IsBatch]);
                                cbBatchHeaderHasTotal.Checked = Convert.ToBoolean(row[CN.BatchHeaderHasTotal]);
                                txtBatchHeaderIdBegin.Text = Convert.ToString(row[CN.BatchHeaderIdBegin]);
                                txtBatchHeaderIdLength.Text = Convert.ToString(row[CN.BatchHeaderIdLength]);
                                txtBatchHeaderId.Text = Convert.ToString(row[CN.BatchHeaderId]);
                                txtBatchHeaderMoneyBegin.Text = Convert.ToString(row[CN.BatchHeaderMoneyBegin]);
                                txtBatchHeaderMoneyLength.Text = Convert.ToString(row[CN.BatchHeaderMoneyLength]);
                                //IP - 03/09/10 - CR1092 - COASTER to CoSACS Enhancements
                                txtBatchTrailerIdBegin.Text = Convert.ToString(row[CN.BatchTrailerIdBegin]);
                                txtBatchTrailerIdLength.Text = Convert.ToString(row[CN.BatchTrailerIdLength]);
                                txtBatchTrailerId.Text = Convert.ToString(row[CN.BatchTrailerId]);


                                //IP - 25/08/10 - CR1092 - COASTER to CoSACS Enhancements
                                if (Convert.ToBoolean(row[CN.IsDelimited]))
                                {
                                    rbDelimitedFile.Checked = true;
                                    drpDelimiter.Text = Convert.ToString(row[CN.Delimiter]);

                                    drpDelimitedDateFormat.Text = Convert.ToString(row[CN.DateFormat]);
                                    if (Convert.ToInt16(row[CN.MoneyPoint]) == 1)
                                    {
                                        cbDelimitedHasDecimal.Checked = true;
                                    }
                                    numNoOfCols.Value = Convert.ToInt16(row[CN.DelimitedNoOfCols]);
                                    drpAcctNoCol.Text = Convert.ToString(row[CN.DelimitedAcctNoColNo]);
                                    drpDateCol.Text = Convert.ToString(row[CN.DelimitedDateColNo]);
                                    drpMoneyCol.Text = Convert.ToString(row[CN.DelimitedMoneyColNo]);

                                    rbFixedFile.Enabled = false; //Disable as we do not want users changing the definition to Fixed file if defined as Delimited.

                                }
                                else
                                {
                                    rbFixedFile.Checked = true;

                                    rbDelimitedFile.Enabled = false;   //Disable as we do not want users changing the definition to Delimited file if defined as Fixed.
                                }

                                //IP - 03/09/10 - CR1112 - Tallyman Interest Charges
                                if (Convert.ToBoolean(row[CN.IsInterest]))
                                {
                                    rbIsInterest.Checked = true;
                                    rbIsPayment.Enabled = false;
                                }
                                else
                                {
                                    rbIsPayment.Checked = true;
                                    rbIsInterest.Enabled = false;
                                }


                                // Set payment method dropdown
                                string payMethod = row[CN.Paymentmethod].ToString();
                                foreach (DataTable pm in pmDefn.Tables)
                                {
                                    foreach (DataRow pmrow in pm.Rows)
                                    {
                                        if (payMethod == (string)pmrow[CN.Code])
                                        {
                                            int i = drpPayMethod.FindStringExact((string)pmrow[CN.CodeDescript]);
                                            if (i != -1)
                                                drpPayMethod.SelectedIndex = i;
                                        }
                                    }
                                }
                                //int payMethodx = Convert.ToInt16 (row); //causes unmonitored error

                                btnDelete.Enabled = true;
                                btnClear.Enabled = true;
                                btnSave.Enabled = true;

                                //IP - 13/09/10 - CR1092  - Disable fields when existing bank selected.
                                txtSourceName.Enabled = false;
                                txtFileExt.Enabled = false;

                            }
                        }
                    //}
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

        //IP - 14/09/10 - CR1092 - COASTER to CoSACS Enhancements
        private void txtSourceName_Leave(object sender, EventArgs e)
        {
            bool foundBank = false;
            if (txtSourceName.Text.Trim().Length != 0)
            {
                for (int i = 0; i < ((DataTable)drpSourceName.DataSource).Rows.Count; i++)
                {
                    if (Convert.ToString(((DataTable)drpSourceName.DataSource).Rows[i][CN.BankName]) == txtSourceName.Text.Trim())
                    {
                        foundBank = true;
                        drpSourceName.SelectedIndex = i;
                        break;
                    }
                }

                //If this is a new bank then disable the Existing Source Files drop down
                if (foundBank == false)
                {
                    drpSourceName.Enabled = false;
                }
            }
        }

    }
      

  }

