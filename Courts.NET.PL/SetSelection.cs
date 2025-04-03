using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using STL.Common;
using System.Data;
using STL.Common.Constants.ColumnNames;
using STL.Common.Static;
using System.Xml;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.RateTypes;

namespace STL.PL
{
    /// <summary>
    /// Maintenance screen for user created sets of data, such as grouping
    /// transaction types or delivery times in delivery areas.
    /// An 'Available' pane lists all possible members of the set being maintained.
    /// The user can create a new set and add new members to a 'Selected' pane or
    /// remove members from the 'Selected' pane.
    /// An optional branch list can be displayed that can be used for certain
    /// types of sets, such as Delivery Areas. The branch list ticks those
    /// branches where the set (or Delivery Area) can be used.
    /// </summary>
    public class SetSelection : CommonForm
    {
        private string _objectName;
        private int _col1Width;
        private int _col2Width;
        private TextBox _parentTextBox;
        private DataTable _dtAvailable;
        private DataTable _dtSelected;
        private DataTable _branchList = null;
        private int _tickClickIndex = -1;
        private int _branchIndex = -1;
        private DataView _availableView;
        private DataView _selectedView;
        private string _tName;
        private string _staticDataTableName;
        private bool _showBranch = false;
        private bool _detectChanges = false;
        private new string Error = "";
        private readonly int MAX_SETNAME_CHARS = 32;

        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.GroupBox gbSetDetailsSelection;
        private System.Windows.Forms.Button btnRemoveAll;
        private System.Windows.Forms.Button btnRemoveSelection;
        private System.Windows.Forms.Button btnAddAll;
        private System.Windows.Forms.Button btnAddSelection;
        private System.Windows.Forms.DataGrid dgAvailable;
        private System.Windows.Forms.DataGrid dgSelected;
        private System.Windows.Forms.GroupBox gbSetSelection;
        private System.Windows.Forms.DataGrid dgBranchList;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Button btnSetTickAll;
        private System.Windows.Forms.Button btnClearTickAll;
        private System.Windows.Forms.GroupBox gbBranch;
        private System.Windows.Forms.Label lbSaveAs;
        private System.Windows.Forms.ComboBox drpSetNames;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lbSelect;
        private System.Windows.Forms.TextBox txtSetName;
        private System.Windows.Forms.Button btnUse;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtSetDesc;
        private System.Windows.Forms.Panel plcriteria;
        private System.Windows.Forms.GroupBox gbSelection;
        private System.Windows.Forms.TextBox textItemEntry;
        private System.Windows.Forms.Button buttonAddText;
        private NumericUpDown udValue;
        private Label lbValue;
        private System.ComponentModel.IContainer components;

        public SetSelection()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
        }

        /// <summary>
        /// Display a modal selection screen that allows the user to group selected data
        /// into 'sets'.
        /// </summary>
        /// <param name="objectName">The name to display on the set selection screen - eg. 'Branches'</param>
        /// <param name="col1Width">The width to use when displaying the first column of selectable data in a datagrid</param>
        /// <param name="col2Width">The width to use when displaying the second column of selectable data in a datagrid</param>
        /// <param name="setNameLength">The maximum number of characters that can be used for the name of a set. 
        /// Consider if the setname will be used as a foreign key in other tables and use an appropriate length.</param>
        /// <param name="parentTextBox">A reference to the Text Box that needs to be updated on the parent screen that instantiated 
        /// this SetSelection object. It will be updated with the Name of the selected set.
        /// </param>
        /// <param name="tName">The 'TName' that identifies the 'type' of set being used, eg. 'Employee','Branch','Delivery Area'</param>
        /// <param name="staticDataTableName">The name of a (Static data area) DataTable that contains the selection data - this is usually data from the Code table.</param>
        public SetSelection(string objectName, int col1Width, int col2Width,
            int setNameLength, TextBox parentTextBox, string tName,
            string staticDataTableName, bool showBranch)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            _objectName = objectName;
            this._col1Width = col1Width;
            this._col2Width = col2Width;
            this._parentTextBox = parentTextBox;
            this._tName = tName;
            this._staticDataTableName = staticDataTableName;
            this._showBranch = showBranch;

            if (setNameLength >= 0 && setNameLength <= MAX_SETNAME_CHARS)
            {
                txtSetName.MaxLength = setNameLength;
            }
            else
            {
                txtSetName.MaxLength = MAX_SETNAME_CHARS;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetSelection));
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.gbSelection = new System.Windows.Forms.GroupBox();
            this.gbBranch = new System.Windows.Forms.GroupBox();
            this.btnSetTickAll = new System.Windows.Forms.Button();
            this.btnClearTickAll = new System.Windows.Forms.Button();
            this.dgBranchList = new System.Windows.Forms.DataGrid();
            this.gbSetSelection = new System.Windows.Forms.GroupBox();
            this.plcriteria = new System.Windows.Forms.Panel();
            this.udValue = new System.Windows.Forms.NumericUpDown();
            this.lbValue = new System.Windows.Forms.Label();
            this.lbSaveAs = new System.Windows.Forms.Label();
            this.drpSetNames = new System.Windows.Forms.ComboBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.lbSelect = new System.Windows.Forms.Label();
            this.txtSetName = new System.Windows.Forms.TextBox();
            this.btnUse = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtSetDesc = new System.Windows.Forms.TextBox();
            this.gbSetDetailsSelection = new System.Windows.Forms.GroupBox();
            this.buttonAddText = new System.Windows.Forms.Button();
            this.btnRemoveAll = new System.Windows.Forms.Button();
            this.btnRemoveSelection = new System.Windows.Forms.Button();
            this.btnAddAll = new System.Windows.Forms.Button();
            this.btnAddSelection = new System.Windows.Forms.Button();
            this.dgSelected = new System.Windows.Forms.DataGrid();
            this.textItemEntry = new System.Windows.Forms.TextBox();
            this.dgAvailable = new System.Windows.Forms.DataGrid();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.gbSelection.SuspendLayout();
            this.gbBranch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgBranchList)).BeginInit();
            this.gbSetSelection.SuspendLayout();
            this.plcriteria.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udValue)).BeginInit();
            this.gbSetDetailsSelection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgSelected)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgAvailable)).BeginInit();
            this.SuspendLayout();
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // gbSelection
            // 
            this.gbSelection.BackColor = System.Drawing.SystemColors.Control;
            this.gbSelection.Controls.Add(this.gbBranch);
            this.gbSelection.Controls.Add(this.gbSetSelection);
            this.gbSelection.Controls.Add(this.gbSetDetailsSelection);
            this.gbSelection.Location = new System.Drawing.Point(2, 1);
            this.gbSelection.Name = "gbSelection";
            this.gbSelection.Size = new System.Drawing.Size(862, 475);
            this.gbSelection.TabIndex = 33;
            this.gbSelection.TabStop = false;
            // 
            // gbBranch
            // 
            this.gbBranch.Controls.Add(this.btnSetTickAll);
            this.gbBranch.Controls.Add(this.btnClearTickAll);
            this.gbBranch.Controls.Add(this.dgBranchList);
            this.gbBranch.Location = new System.Drawing.Point(648, 160);
            this.gbBranch.Name = "gbBranch";
            this.gbBranch.Size = new System.Drawing.Size(120, 312);
            this.gbBranch.TabIndex = 34;
            this.gbBranch.TabStop = false;
            this.gbBranch.Text = "Branches";
            // 
            // btnSetTickAll
            // 
            this.btnSetTickAll.Enabled = false;
            this.btnSetTickAll.Location = new System.Drawing.Point(64, 280);
            this.btnSetTickAll.Name = "btnSetTickAll";
            this.btnSetTickAll.Size = new System.Drawing.Size(40, 23);
            this.btnSetTickAll.TabIndex = 24;
            this.btnSetTickAll.Text = "All";
            this.btnSetTickAll.Click += new System.EventHandler(this.btnSetTickAll_Click);
            // 
            // btnClearTickAll
            // 
            this.btnClearTickAll.Enabled = false;
            this.btnClearTickAll.Location = new System.Drawing.Point(16, 280);
            this.btnClearTickAll.Name = "btnClearTickAll";
            this.btnClearTickAll.Size = new System.Drawing.Size(40, 23);
            this.btnClearTickAll.TabIndex = 23;
            this.btnClearTickAll.Text = "Clear";
            this.btnClearTickAll.Click += new System.EventHandler(this.btnClearTickAll_Click);
            // 
            // dgBranchList
            // 
            this.dgBranchList.CaptionText = "                                Selected";
            this.dgBranchList.ColumnHeadersVisible = false;
            this.dgBranchList.DataMember = "";
            this.dgBranchList.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgBranchList.Location = new System.Drawing.Point(8, 24);
            this.dgBranchList.Name = "dgBranchList";
            this.dgBranchList.ReadOnly = true;
            this.dgBranchList.Size = new System.Drawing.Size(104, 248);
            this.dgBranchList.TabIndex = 21;
            this.dgBranchList.TabStop = false;
            this.dgBranchList.CurrentCellChanged += new System.EventHandler(this.dgBranchList_CurrentCellChanged);
            this.dgBranchList.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgBranchList_MouseUp);
            // 
            // gbSetSelection
            // 
            this.gbSetSelection.Controls.Add(this.plcriteria);
            this.gbSetSelection.Location = new System.Drawing.Point(8, 8);
            this.gbSetSelection.Name = "gbSetSelection";
            this.gbSetSelection.Size = new System.Drawing.Size(760, 136);
            this.gbSetSelection.TabIndex = 33;
            this.gbSetSelection.TabStop = false;
            // 
            // plcriteria
            // 
            this.plcriteria.Controls.Add(this.udValue);
            this.plcriteria.Controls.Add(this.lbValue);
            this.plcriteria.Controls.Add(this.lbSaveAs);
            this.plcriteria.Controls.Add(this.drpSetNames);
            this.plcriteria.Controls.Add(this.btnSave);
            this.plcriteria.Controls.Add(this.lbSelect);
            this.plcriteria.Controls.Add(this.txtSetName);
            this.plcriteria.Controls.Add(this.btnUse);
            this.plcriteria.Controls.Add(this.label2);
            this.plcriteria.Controls.Add(this.label3);
            this.plcriteria.Controls.Add(this.txtSetDesc);
            this.plcriteria.Location = new System.Drawing.Point(16, 8);
            this.plcriteria.Name = "plcriteria";
            this.plcriteria.Size = new System.Drawing.Size(712, 128);
            this.plcriteria.TabIndex = 33;
            // 
            // udValue
            // 
            this.udValue.DecimalPlaces = 4;
            this.udValue.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.udValue.Location = new System.Drawing.Point(230, 101);
            this.udValue.Maximum = new decimal(new int[] {
            25,
            0,
            0,
            0});
            this.udValue.Name = "udValue";
            this.udValue.Size = new System.Drawing.Size(65, 20);
            this.udValue.TabIndex = 52;
            this.udValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.udValue.ValueChanged += new System.EventHandler(this.udValue_ValueChanged);
            // 
            // lbValue
            // 
            this.lbValue.AutoSize = true;
            this.lbValue.Location = new System.Drawing.Point(137, 104);
            this.lbValue.Name = "lbValue";
            this.lbValue.Size = new System.Drawing.Size(90, 13);
            this.lbValue.TabIndex = 50;
            this.lbValue.Text = "Driver Payment %";
            // 
            // lbSaveAs
            // 
            this.lbSaveAs.Location = new System.Drawing.Point(120, 37);
            this.lbSaveAs.Name = "lbSaveAs";
            this.lbSaveAs.Size = new System.Drawing.Size(104, 16);
            this.lbSaveAs.TabIndex = 46;
            this.lbSaveAs.Text = "Save As..";
            this.lbSaveAs.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // drpSetNames
            // 
            this.drpSetNames.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpSetNames.Location = new System.Drawing.Point(230, 8);
            this.drpSetNames.Name = "drpSetNames";
            this.drpSetNames.Size = new System.Drawing.Size(296, 21);
            this.drpSetNames.TabIndex = 43;
            this.drpSetNames.SelectedIndexChanged += new System.EventHandler(this.drpSetNames_SelectedIndexChanged);
            this.drpSetNames.SelectionChangeCommitted += new System.EventHandler(this.drpSetNames_SelectionChangeCommitted);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(534, 96);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(56, 23);
            this.btnSave.TabIndex = 41;
            this.btnSave.Tag = "Save the selected set or create a new set if a new name is specified";
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lbSelect
            // 
            this.lbSelect.Location = new System.Drawing.Point(9, 8);
            this.lbSelect.Name = "lbSelect";
            this.lbSelect.Size = new System.Drawing.Size(215, 23);
            this.lbSelect.TabIndex = 47;
            this.lbSelect.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtSetName
            // 
            this.txtSetName.Enabled = false;
            this.txtSetName.Location = new System.Drawing.Point(230, 53);
            this.txtSetName.MaxLength = 60;
            this.txtSetName.Name = "txtSetName";
            this.txtSetName.Size = new System.Drawing.Size(152, 20);
            this.txtSetName.TabIndex = 49;
            this.txtSetName.TextChanged += new System.EventHandler(this.txtSetName_TextChanged);
            // 
            // btnUse
            // 
            this.btnUse.Enabled = false;
            this.btnUse.Location = new System.Drawing.Point(534, 8);
            this.btnUse.Name = "btnUse";
            this.btnUse.Size = new System.Drawing.Size(56, 23);
            this.btnUse.TabIndex = 42;
            this.btnUse.Tag = "Save the selected set or create a new set if a new name is specified";
            this.btnUse.Text = "Use";
            this.btnUse.Click += new System.EventHandler(this.btnUse_Click);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(120, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 16);
            this.label2.TabIndex = 45;
            this.label2.Text = "Name";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(120, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 16);
            this.label3.TabIndex = 44;
            this.label3.Text = "Description";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtSetDesc
            // 
            this.txtSetDesc.Enabled = false;
            this.txtSetDesc.Location = new System.Drawing.Point(230, 77);
            this.txtSetDesc.MaxLength = 70;
            this.txtSetDesc.Name = "txtSetDesc";
            this.txtSetDesc.Size = new System.Drawing.Size(296, 20);
            this.txtSetDesc.TabIndex = 48;
            this.txtSetDesc.TextChanged += new System.EventHandler(this.txtSetDesc_TextChanged);
            // 
            // gbSetDetailsSelection
            // 
            this.gbSetDetailsSelection.Controls.Add(this.buttonAddText);
            this.gbSetDetailsSelection.Controls.Add(this.btnRemoveAll);
            this.gbSetDetailsSelection.Controls.Add(this.btnRemoveSelection);
            this.gbSetDetailsSelection.Controls.Add(this.btnAddAll);
            this.gbSetDetailsSelection.Controls.Add(this.btnAddSelection);
            this.gbSetDetailsSelection.Controls.Add(this.dgSelected);
            this.gbSetDetailsSelection.Controls.Add(this.textItemEntry);
            this.gbSetDetailsSelection.Controls.Add(this.dgAvailable);
            this.gbSetDetailsSelection.Location = new System.Drawing.Point(8, 160);
            this.gbSetDetailsSelection.Name = "gbSetDetailsSelection";
            this.gbSetDetailsSelection.Size = new System.Drawing.Size(632, 312);
            this.gbSetDetailsSelection.TabIndex = 28;
            this.gbSetDetailsSelection.TabStop = false;
            this.gbSetDetailsSelection.Text = "Use the buttons to modify selected details ";
            // 
            // buttonAddText
            // 
            this.buttonAddText.Location = new System.Drawing.Point(296, 40);
            this.buttonAddText.Name = "buttonAddText";
            this.buttonAddText.Size = new System.Drawing.Size(40, 23);
            this.buttonAddText.TabIndex = 27;
            this.buttonAddText.Text = ">";
            this.buttonAddText.Click += new System.EventHandler(this.buttonAddText_Click);
            // 
            // btnRemoveAll
            // 
            this.btnRemoveAll.Location = new System.Drawing.Point(296, 200);
            this.btnRemoveAll.Name = "btnRemoveAll";
            this.btnRemoveAll.Size = new System.Drawing.Size(40, 23);
            this.btnRemoveAll.TabIndex = 25;
            this.btnRemoveAll.Text = "<<";
            this.btnRemoveAll.Click += new System.EventHandler(this.btnRemoveAll_Click);
            // 
            // btnRemoveSelection
            // 
            this.btnRemoveSelection.Location = new System.Drawing.Point(296, 136);
            this.btnRemoveSelection.Name = "btnRemoveSelection";
            this.btnRemoveSelection.Size = new System.Drawing.Size(40, 23);
            this.btnRemoveSelection.TabIndex = 23;
            this.btnRemoveSelection.Text = "<";
            this.btnRemoveSelection.Click += new System.EventHandler(this.btnRemoveSelection_Click);
            // 
            // btnAddAll
            // 
            this.btnAddAll.Location = new System.Drawing.Point(296, 168);
            this.btnAddAll.Name = "btnAddAll";
            this.btnAddAll.Size = new System.Drawing.Size(40, 23);
            this.btnAddAll.TabIndex = 24;
            this.btnAddAll.Text = ">>";
            this.btnAddAll.Click += new System.EventHandler(this.btnAddAll_Click);
            // 
            // btnAddSelection
            // 
            this.btnAddSelection.Location = new System.Drawing.Point(296, 104);
            this.btnAddSelection.Name = "btnAddSelection";
            this.btnAddSelection.Size = new System.Drawing.Size(40, 23);
            this.btnAddSelection.TabIndex = 22;
            this.btnAddSelection.Text = ">";
            this.btnAddSelection.Click += new System.EventHandler(this.btnAddSelection_Click);
            // 
            // dgSelected
            // 
            this.dgSelected.CaptionText = "                                Selected";
            this.dgSelected.DataMember = "";
            this.dgSelected.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgSelected.Location = new System.Drawing.Point(344, 24);
            this.dgSelected.Name = "dgSelected";
            this.dgSelected.ReadOnly = true;
            this.dgSelected.RowHeadersVisible = false;
            this.dgSelected.Size = new System.Drawing.Size(280, 280);
            this.dgSelected.TabIndex = 20;
            // 
            // textItemEntry
            // 
            this.textItemEntry.Location = new System.Drawing.Point(16, 24);
            this.textItemEntry.Name = "textItemEntry";
            this.textItemEntry.Size = new System.Drawing.Size(264, 20);
            this.textItemEntry.TabIndex = 26;
            // 
            // dgAvailable
            // 
            this.dgAvailable.CaptionText = "                                Available";
            this.dgAvailable.ColumnHeadersVisible = false;
            this.dgAvailable.DataMember = "";
            this.dgAvailable.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgAvailable.Location = new System.Drawing.Point(8, 24);
            this.dgAvailable.Name = "dgAvailable";
            this.dgAvailable.ReadOnly = true;
            this.dgAvailable.Size = new System.Drawing.Size(280, 280);
            this.dgAvailable.TabIndex = 19;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "");
            this.imageList1.Images.SetKeyName(1, "");
            // 
            // SetSelection
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(866, 477);
            this.Controls.Add(this.gbSelection);
            this.MaximizeBox = false;
            this.Name = "SetSelection";
            this.Text = "Select";
            this.Load += new System.EventHandler(this.SetSelection_Load);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.gbSelection.ResumeLayout(false);
            this.gbBranch.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgBranchList)).EndInit();
            this.gbSetSelection.ResumeLayout(false);
            this.plcriteria.ResumeLayout(false);
            this.plcriteria.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udValue)).EndInit();
            this.gbSetDetailsSelection.ResumeLayout(false);
            this.gbSetDetailsSelection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgSelected)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgAvailable)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private void SetSelection_Load(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                _detectChanges = false;

                if (this._showBranch)
                {
                    this.gbBranch.Visible = true;
                    this.Width = 800;
                    this.gbSelection.Width = 788;           // #4422 jec 29/07/11
                    this.gbSetSelection.Width = 760;
                    this.plcriteria.Left = this.gbSetSelection.Left + ((this.gbSetSelection.Width - this.plcriteria.Width) / 2);
                }
                else
                {
                    this.gbBranch.Visible = false;
                    this.Width = 672;
                    this.gbSelection.Width = 788;           // #4422 jec 29/07/11
                    this.gbSetSelection.Width = 760;
                    this.plcriteria.Left = this.gbSetSelection.Left + ((this.gbSetSelection.Width - this.plcriteria.Width) / 2);
                }
                btnUse.Visible = (_parentTextBox != null);
                LoadSetsForTName();
                // only load up if a static table

                LoadAvailableDataForTName();
                if (this._staticDataTableName != "")
                    MapDataTableToAvailableGrid(false);
                textItemEntry.Visible = false;

                if (this._staticDataTableName == "")  // not loading from code table, but manually entereing
                {
                    dgAvailable.Visible = false;
                    textItemEntry.Visible = true;
                    btnAddSelection.Visible = false;
                    //btnRemoveSelection.Text = "Remove";
                    btnAddAll.Visible = false;
                    btnRemoveAll.Visible = false;
                    buttonAddText.Visible = true;
                    if (_objectName == "Item Categories")
                        gbSetDetailsSelection.Text = "Enter 2 digit category restriction here";
                    else
                        if (_objectName == "Items")
                            gbSetDetailsSelection.Text = "Enter item codes here";
                        else
                            gbSetDetailsSelection.Text = "Enter item starting codes here";
                }
                else
                {
                    buttonAddText.Visible = false;
                }
                //		this._dtSelected = new DataTable();	
                //if	
                if (drpSetNames.Items.Count == 0)
                {
                    if(this._dtSelected == null)
                        this._dtSelected = new DataTable();	

                    DataColumn myDataColumn = new DataColumn();
                    myDataColumn.DataType = System.Type.GetType("System.String");
                    myDataColumn.ColumnName = "Code";
                    myDataColumn.ReadOnly = true;
                    myDataColumn.Unique = true;
                    this._dtSelected.Columns.Add(myDataColumn);
                }

                if (this._staticDataTableName == "DeliveryArea")            // #13691
                {
                    udValue.Visible=true; 
                    lbValue.Visible=true;               
                }
                else
                {
                    udValue.Visible = false;
                    lbValue.Visible = false; 
                }
                
                 
                //Always force the user to select a Set
                //if (this._staticDataTableName != "")
                    MapDataTableToSelectedGrid();
                //_parenttextbox is the parent text box that this screen will fill on return.
                if (_parentTextBox != null && _parentTextBox.Text.Length > 0 )
                {
                    drpSetNames.SelectedValue = _parentTextBox.Text;
                    SelectSetName(_parentTextBox.Text);
                    this.Text += " " + this._objectName;
                    lbSelect.Text = GetResource("L_SELECTSET");
                }
                else
                {
                        if (this._staticDataTableName != "")
                        {
                            RefreshAvailableGrid();
                            btnUse.Enabled = (drpSetNames.SelectedIndex >= 0 && _selectedView.Count > 0);
                            drpSetNames.SelectedIndex = -1;
                            _dtSelected.Clear();
                        }
                        else
                            btnUse.Enabled = (drpSetNames.SelectedIndex >= 0);
                    btnSave.Enabled = false;
                    txtSetName.Text = string.Empty;
                    txtSetDesc.Text = string.Empty;
                    udValue.Value = 0;                // #13691
                    txtSetName.Enabled = false;
                    txtSetDesc.Enabled = false;
                    this.Text = " " + this._objectName + GetResource("Maintenance");
                    lbSelect.Text = GetResource("L_SELECTA") + " " + this._objectName;
                    if (((DataTable)drpSetNames.DataSource).Rows.Count > 0)
                    {
                        drpSetNames.SelectedIndex = 0;
                        drpSetNames_SelectionChangeCommitted(null, null);
                    }
                    else
                        SelectSetName("");
                }

                // Loop through the controls that belong to the branch datagrid
                // to enable the scroll bars, as they are being disabled
                foreach (Control c in dgBranchList.Controls)
                {
                    if (c.GetType().Name == "HScrollBar" || c.GetType().Name == "VScrollBar")
                    {
                        c.Enabled = true;
                    }
                }

            }
            catch (Exception ex)
            {
                Catch(ex, "SetSelection_Load");
            }
            finally
            {
                _detectChanges = true;
                StopWait();
            }
        }

        /// <summary>
        /// LoadSetsforTname will create a dataset for sets data from the sets table and fill drpSetNames
        /// with this information
        /// </summary>
        private void LoadSetsForTName()
        {
            DataSet ds = new DataSet();
            ds = SetDataManager.GetSetsForTName(this._tName, out Error);
            DataTable dt = ds.Tables[TN.SetsData];

            if (Error.Length > 0)
                ShowError(Error);
            else
            {
                // add row to allow adding new category sets
                DataRow dr = dt.NewRow();
                dr[CN.SetName] = "New..";
                dt.Rows.Add(dr);

                drpSetNames.DataSource = dt;
                drpSetNames.DisplayMember = CN.SetName;
                drpSetNames.ValueMember = CN.SetName;

            }
        }


        /// <summary>
        /// Will create a new dataview _availableView from _dtavailable and assigns dgAvailable datasource to this
        /// Also Sorts _available view on CN.Code and suppresses all but the first two columns
        /// - FA UAT 671 - Changed to allows forced reset of Available list.
        /// </summary>
        private void MapDataTableToAvailableGrid(bool ForceReset)
        {
            _availableView = new DataView(_dtAvailable);

            if ((dgAvailable.TableStyles.Count == 0) || (ForceReset))
            {
                DataGridTableStyle tabStyle = new DataGridTableStyle();
                tabStyle.MappingName = _availableView.Table.TableName;

                dgAvailable.TableStyles.Clear();
                dgAvailable.TableStyles.Add(tabStyle);
                dgAvailable.DataSource = _availableView;

                tabStyle.GridColumnStyles[0].Width = this._col1Width;
                tabStyle.GridColumnStyles[0].ReadOnly = true;
                tabStyle.GridColumnStyles[0].HeaderText = string.Empty;

                if (_dtAvailable.Columns.Count > 1)
                {
                    tabStyle.GridColumnStyles[1].Width = this._col2Width;
                    tabStyle.GridColumnStyles[1].ReadOnly = true;
                    tabStyle.GridColumnStyles[1].HeaderText = string.Empty;
                }

                //Suppress all remaining columns
                for (int i = 2; i < _dtAvailable.Columns.Count; i++)
                {
                    tabStyle.GridColumnStyles[i].HeaderText = "1111";
                    tabStyle.GridColumnStyles[i].Width = 0;
                }
            }
            if (this._staticDataTableName != "")
                _availableView.Sort = CN.Code + " ASC ";
        }

        /// <summary>
        /// Will clone _dtSelected from _dtAvailable. _selectedView becomes a new DataView of _dtSelected
        /// Assigns the correct width of the first 2 columns then finally sorts _selectedView 
        /// </summary>
        private void MapDataTableToSelectedGrid()
        {
            if (_dtAvailable != null && _dtSelected == null)
            {
                _dtSelected = _dtAvailable.Clone();
            }
            /*else
            {
                _dtSelected = new DataTable();
            }*/

            _selectedView = new DataView(_dtSelected);

            if (dgSelected.TableStyles.Count == 0)
            {
                DataGridTableStyle tabStyle = new DataGridTableStyle();
                tabStyle.MappingName = _selectedView.Table.TableName;

                dgSelected.TableStyles.Clear();
                dgSelected.TableStyles.Add(tabStyle);
                dgSelected.DataSource = _selectedView;

                tabStyle.GridColumnStyles[0].Width = this._col1Width;
                tabStyle.GridColumnStyles[0].ReadOnly = true;
                tabStyle.GridColumnStyles[0].HeaderText = string.Empty;

                if (_dtSelected.Columns.Count > 1)
                {
                    tabStyle.GridColumnStyles[1].Width = this._col2Width;
                    tabStyle.GridColumnStyles[1].ReadOnly = true;
                    tabStyle.GridColumnStyles[1].HeaderText = string.Empty;
                }
                //Suppress all remaining columns
                for (int i = 2; i < _dtSelected.Columns.Count; i++)
                {
                    tabStyle.GridColumnStyles[i].HeaderText = "2222";
                    tabStyle.GridColumnStyles[i].Width = 0;
                }
            }
            if (this._staticDataTableName != "")
                _selectedView.Sort = CN.Code + " ASC ";
        }

        private void btnAddSelection_Click(object sender, System.EventArgs e)
        {
            Function = "btnAddSelection_Click";

            try
            {
                for (int index = _availableView.Count - 1; index >= 0; index--)
                {
                    if (dgAvailable.IsSelected(index) || index == dgAvailable.CurrentRowIndex)
                    {
                        SelectAvailableRow(index);
                    }
                }
                if (this._staticDataTableName != "")
                      RefreshAvailableGrid();
                DisableUseAllowSave();
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                Function = "End of btnAddSelection_Click";
            }
        }

        private void RefreshAvailableGrid()
        {

            if (_dtSelected.Rows.Count > 0)
            {

                string filter = CN.Code + " not in (";
                foreach (DataRowView row in _selectedView)
                {
                    filter += "'" + row[CN.Code].ToString() + "',";
                }

                filter += ")";
                _availableView.RowFilter = filter;
            }
            else
            {
                if (_availableView != null)
                    _availableView.RowFilter = string.Empty;
            }

        }

        private void btnRemoveSelection_Click(object sender, System.EventArgs e)
        {
            Function = "btnRemoveSelection_Click";

            try
            {
                //Get a list of the codes that need to be removed from the selected table
                ArrayList codes = new ArrayList();
                for (int index = _selectedView.Count - 1; index >= 0; index--)
                {
                    if (dgSelected.IsSelected(index) || index == dgSelected.CurrentRowIndex)
                    {
                        codes.Add(dgSelected[index, 0].ToString());
                    }
                }
                DataRow selectedRow;
                for (int i = _dtSelected.Rows.Count - 1; i >= 0; i--)
                {
                    selectedRow = _dtSelected.Rows[i];
                    if (codes.Contains(selectedRow[CN.Code].ToString()))
                    {
                        _dtSelected.Rows.Remove(selectedRow);
                    }
                }
                _dtSelected.AcceptChanges();
                if (this._staticDataTableName != "")
                    RefreshAvailableGrid();
                DisableUseAllowSave();
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                Function = "End of btnRemoveSelection_Click";
            }
        }

        private void btnAddAll_Click(object sender, System.EventArgs e)
        {
            Function = "btnAddAll_Click";

            try
            {
                for (int index = _availableView.Count - 1; index >= 0; index--)
                {
                    SelectAvailableRow(index);
                }
                if (this._staticDataTableName != "")
                    RefreshAvailableGrid();
                DisableUseAllowSave();
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                Function = "End of btnAddAll_Click";
            }
        }

        private void btnRemoveAll_Click(object sender, System.EventArgs e)
        {
            Function = "btnRemoveAll_Click";

            try
            {
                DataRow selectedRow;
                for (int i = _dtSelected.Rows.Count - 1; i >= 0; i--)
                {
                    selectedRow = _dtSelected.Rows[i];
                    _dtSelected.Rows.Remove(selectedRow);
                }
                _dtSelected.AcceptChanges();
                if (this._staticDataTableName != "")
                    RefreshAvailableGrid();
                DisableUseAllowSave();
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                Function = "End of btnRemoveAll_Click";
            }
        }

        private void btnSave_Click(object sender, System.EventArgs e)
        {
            SaveSetDetails();
        }


        private void UpdateParentFormText()
        {
            if (FormParent.GetType().Name == "WarrantyReporting")
            {
                ((WarrantyReporting)this.FormParent).LoadCategories(drpSetNames.SelectedValue.ToString());
                CloseTab();
            }
            else
            {
                _parentTextBox.Text = drpSetNames.SelectedValue.ToString();
                this.Close();
            }
        }

        /// <summary>
        /// Use the currently selected Set. The name of the selected set will
        /// be displayed in the parent form's TextBox control that was passed 
        /// to this Form.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Details of the event</param>
        private void btnUse_Click(object sender, System.EventArgs e)
        {
            UpdateParentFormText();
        }

        private void drpSetNames_SelectionChangeCommitted(object sender, System.EventArgs e)
        {
            // uat432 rdb 17/06/08 clean up any validation warnings if required
            errorProvider1.SetError(txtSetName, "");
            //Get DataTable containing Code selections for the specified SetName
            string setName = drpSetNames.SelectedValue.ToString();
            if (setName != "New..")
                SelectSetName(setName);
            else
            {
                txtSetName.Text = string.Empty;
                txtSetDesc.Text = string.Empty;
                udValue.Value=0;                // #13691
                if(dgSelected.DataSource is DataView)
                    ((DataView)dgSelected.DataSource).Table.Rows.Clear(); //UAT(5.2) - 585 - NM
                else
                    ((DataTable)dgSelected.DataSource).Rows.Clear();

                // FA - UAT 671 - reset Available list
                MapDataTableToAvailableGrid(true);

            }

        }

        /// <summary>
        /// Retrieves selection data for a specified SetName. 
        /// Also sets txtSetName.Text, txtSetDesc.Text
        /// and button availability.
        /// </summary>
        /// <param name=CN.SetName></param>
        private void SelectSetName(string setName)
        {
            _detectChanges = false;

            //Get the Sets description..
            DataSet dsSetDetails = SetDataManager.GetSets(setName, this._tName, out this.Error);
            if (Error.Length > 0)
                ShowError(Error);
            else
            {
                //Only make use of a valid SetName..
                if (dsSetDetails.Tables.Count > 0
                    && dsSetDetails.Tables[0].Rows.Count > 0)
                {
                    txtSetDesc.Text = dsSetDetails.Tables[0].Rows[0][CN.SetDescript].ToString();
                    txtSetName.Text = setName;
                    udValue.Value = Convert.ToDecimal(dsSetDetails.Tables[0].Rows[0]["Value"]);     // #13691
                }
                //Now get the actual SetDetails rows for this Set..
                DataSet dsCodesForSetName = SetDataManager.GetSetDetailsForSetName(setName, this._tName, out this.Error);
                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    if (_dtSelected != null)
                        _dtSelected.Rows.Clear();
                    else
                        this._dtSelected = new DataTable();

                    if (this._staticDataTableName != "") 
                        this.RefreshAvailableGrid();
                    else
                        LoadAvailableDataForTName();
                        
                    //Only make use of a valid SetName..
                    if (dsCodesForSetName.Tables.Count > 0
                        && dsCodesForSetName.Tables[0].Rows.Count > 0)
                    {
                        //Foreach Code in this DataTable, select it using button selection code
                        foreach (DataRow row in dsCodesForSetName.Tables[0].Rows)
                        {
                            if (LocateAvailableIndexFor(row["code"].ToString()) >=0 )
                                SelectAvailableRow(LocateAvailableIndexFor(row["code"].ToString())); //aa changed from data
                        }
                        if (this._staticDataTableName != "")
                            this.RefreshAvailableGrid();
                        txtSetName.Enabled = true;
                        txtSetDesc.Enabled = true;
                    }
                }

                if (this._showBranch)
                {
                    // Display the branch list for this set
                    this.LoadBranch(dsCodesForSetName.Tables[TN.SetBranchData]);
                }
            }

            btnUse.Enabled = (drpSetNames.SelectedIndex >= 0);
            //&& _selectedView.Count > 0);
            btnSave.Enabled = false;
            _detectChanges = true;
        }

        private void LoadBranch(DataTable branchList)
        {
            try
            {
                Wait();
                Function = "LoadBranch";
                _detectChanges = false;
                _tickClickIndex = -1;

                this._branchList = branchList;

                dgBranchList.DataSource = null;
                dgBranchList.ResetText();
                this.btnClearTickAll.Enabled = false;
                this.btnSetTickAll.Enabled = false;

                if (this._branchList != null)
                {
                    // Load the data grid
                    _branchList.DefaultView.AllowNew = false;
                    dgBranchList.DataSource = _branchList.DefaultView;
                    this._branchIndex = 2;

                    // Add an unbound stand-alone icon column to tick a branch
                    this._branchList.DefaultView.Table.Columns.Add("TestIcon");

                    if (dgBranchList.TableStyles.Count == 0)
                    {
                        DataGridTableStyle tabStyle = new DataGridTableStyle();
                        tabStyle.MappingName = _branchList.TableName;

                        AddColumnStyle(CN.Selected, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                        AddColumnStyle(CN.BranchNo, tabStyle, 30, true, GetResource("T_BRANCH"), "", HorizontalAlignment.Left);

                        // Icon column to tick a branch
                        DataGridIconColumn iconColumn = new DataGridIconColumn(imageList1.Images[0], imageList1.Images[1], CN.Selected, "0", "1");
                        iconColumn.HeaderText = "";
                        iconColumn.MappingName = "TestIcon";
                        iconColumn.Width = imageList1.Images[0].Size.Width;
                        tabStyle.GridColumnStyles.Add(iconColumn);

                        dgBranchList.TableStyles.Add(tabStyle);
                    }
                    this.btnClearTickAll.Enabled = true;
                    this.btnSetTickAll.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                _detectChanges = true;
                StopWait();
                Function = "End of LoadBranch";
            }
        }    // End of LoadBranch


        private void SelectAvailableRow(int selectIndex)
        {
            DataRow row = _dtSelected.NewRow();
            if (this._staticDataTableName != "")
            {
                row[0] = dgAvailable[selectIndex, 0];

                if (_dtSelected.Columns.Count > 1)
                {
                    row[1] = dgAvailable[selectIndex, 1];
                }
                _dtSelected.Rows.Add(row);
            }
        }

        /// <summary>
        /// Returns the index of the row in the Available DataGrid corresponding to the
        /// specified code.
        /// </summary>
        /// <param name="code">The code value you want to locate in the Available DataGrid</param>
        /// <returns>The index of the available grid row for the specified code. -1 is returned
        /// if the specified code cannot be found.</returns>
        private int LocateAvailableIndexFor(string code)
        {


            int index = -1;
            if (_availableView != null)
            {
                for (int i = 0; i < _availableView.Count; i++)
                {
                    if (_availableView[i][0].ToString() == code)
                    {
                        index = i;
                        break;
                    }
                }
            }
            return index;
        }

        private void DisableUseAllowSave()
        {
            _detectChanges = false;
            btnUse.Enabled = false;
            if (drpSetNames.SelectedIndex >= 0 //Modifying existing
                || (drpSetNames.SelectedIndex < 0 && _dtSelected.Rows.Count > 0)) //Adding new
            {
                btnSave.Enabled = true;
                if (_dtSelected.Rows.Count > 0)
                {
                    btnSave.Text = "Save";
                    lbSaveAs.Visible = true;
                    txtSetName.Enabled = true;
                    txtSetDesc.Enabled = true;
                }
                else
                {
                    txtSetName.Enabled = false;
                    txtSetDesc.Enabled = false;
                    btnSave.Text = "Delete";
                    lbSaveAs.Visible = false;
                }
                if (drpSetNames.SelectedIndex >= 0)
                {
                    if (txtSetName.Text.Length == 0)
                    {
                        txtSetName.Text = drpSetNames.SelectedValue.ToString();
                    }
                }
                else
                {
                    txtSetName.Text = GetResource("EnterName");
                }
                txtSetName.Focus();
            }
            else
            {
                btnSave.Enabled = false;
                txtSetDesc.Text = string.Empty;
                txtSetName.Text = string.Empty;
                udValue.Value = 0;                // #13691
            }
            _detectChanges = true;
        }

        private void SaveSetDetails()
        {
            _detectChanges = false;
            if (txtSetName.Text.Length > 0 && txtSetName.Text != GetResource("EnterName"))
            {
                bool existingSet = false;
                foreach (DataRowView drv in drpSetNames.Items)
                {
                    if (drv.Row[0].ToString() == txtSetName.Text)
                    {
                        existingSet = true;
                        break;
                    }
                }
                if ((existingSet && _dtSelected.Rows.Count == 0 &&
                    DialogResult.Yes == ShowInfo("M_DELETESET", MessageBoxButtons.YesNo))
                    ||
                    (existingSet && _dtSelected.Rows.Count > 0 &&
                    DialogResult.Yes == ShowInfo("M_SETNAMEEXISTS", MessageBoxButtons.YesNo)
                    )
                    || !existingSet)
                {
                    //Do the save... Note: The Stored procedure will determine if update or
                    //insert is required - just pass txtSetName to it..
                    ArrayList codes = new ArrayList();
                    foreach (DataRow row in _dtSelected.Rows)
                    {
                        codes.Add(row[0].ToString()); 
                    }
                    ArrayList branchList = new ArrayList();
                    if (this._showBranch)
                    {
                        foreach (DataRow branchRow in _branchList.Rows)
                        {
                            if (Convert.ToBoolean(branchRow[CN.Selected]))
                                branchList.Add(branchRow[CN.BranchNo].ToString());
                        }
                    }
                    string[] codeSelections = (string[])codes.ToArray(typeof(string));
                    string[] branchSelections = (string[])branchList.ToArray(typeof(string));

                    string setName = txtSetName.Text;
                    SetDataManager.SaveSetDetails(
                        txtSetName.Text, this._tName, codeSelections,
                        int.Parse(Credential.UserId.ToString()), "V", txtSetDesc.Text, udValue.Value,
                        branchSelections, out Error);

                    if (Error.Length > 0)
                    {
                        ShowError(Error);
                    }
                    else
                    {
                        //If successful, select the set in the dropdown with this name
                        // - and call the method that retrieves a selected set

                        //uat 276
                        if (btnSave.Text != "Delete")
                        {

                            LoadSetsForTName();
                            drpSetNames.SelectedValue = txtSetName.Text;
                            this.SelectSetName(txtSetName.Text);
                            //Finally, allow the newly saved set to be used

                            if (this._staticDataTableName != "")
                                btnUse.Enabled = (drpSetNames.SelectedIndex >= 0 && _selectedView.Count > 0);
                            else
                                btnUse.Enabled = (drpSetNames.SelectedIndex >= 0);
                            btnSave.Enabled = false;

                        }

                        if (btnSave.Text == "Delete")
                        {
                            //uat 276
                            // virtually impentetrable code but i think this will clear the 2nd grid and
                            // display all values in the first
                            _availableView.RowFilter = string.Empty;
                            drpSetNames.SelectedIndex = -1;
                            _dtSelected.Clear();
                            DataTable setNameSource =(DataTable)drpSetNames.DataSource;
                            DataRow[] toRemove = setNameSource.Select("setName = '" + setName + "'");
                            setNameSource.Rows.Remove(toRemove[0]);






                            txtSetName.Text = string.Empty;
                            txtSetDesc.Text = string.Empty;
                            udValue.Value = 0;                // #13691
                        }
                        btnSave.Text = "Save";
                        lbSaveAs.Visible = true;
                    }
                }
                errorProvider1.SetError(txtSetName, "");
            }
            else
            {
                errorProvider1.SetError(txtSetName, GetResource("M_INVALIDSETNAME"));
                txtSetName.Focus();
            }
            _detectChanges = true;
            //If we have just done a delete, the parent Text Box text needs to be set
            //to a 'non selection' value..
            if (drpSetNames.SelectedIndex < 0 && _parentTextBox != null)
            {
                _parentTextBox.Text = GetResource("NoSetSpecified");
            }
        }

        private void txtSetName_TextChanged(object sender, System.EventArgs e)
        {
            TextDataChanged();
        }
        /// <summary>
        /// If _static data Will invoke dropdowns.loadXML will create an XML node for the _staticDataTableName supplied
        /// and copies this to _dtAvailable Datatable.
        /// Otherwise will fill _dtAvailable with SetDetails from Set Name and then fill dgSelected with data
        /// </summary>
        private void LoadAvailableDataForTName()
        {
          
            if (this._staticDataTableName != "")
            {
                _dtAvailable = new DataTable();
                XmlUtilities xml = new XmlUtilities();
                XmlDocument dropDowns = new XmlDocument();
                dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

                if (StaticData.Tables[_staticDataTableName] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, _staticDataTableName, null));
                _dtAvailable = ((DataTable)StaticData.Tables[_staticDataTableName]).Copy();
                //_dtAvailable.PrimaryKey = new DataColumn[]{_dtAvailable.Columns[0]};
                _dtAvailable.Columns[0].ColumnName = CN.Code;
            }
            else // load data from database
            {
                string currentsetname = drpSetNames.Text;
                string err = "";
                DataSet ds = SetDataManager.GetSetDetailsForSetName(currentsetname, this._tName, out err);
                _dtAvailable = ds.Tables[0]; //' .d.DataSet;
                _dtSelected = ds.Tables[0];
                _selectedView = new DataView(_dtSelected);
                dgSelected.DataSource = _dtAvailable;// _dtAvailable;
                dgAvailable.DataSource = _dtAvailable;// _dtAvailable;
                
                

            }

        }

        private void txtSetDesc_TextChanged(object sender, System.EventArgs e)
        {
            TextDataChanged();
        }

        private void TextDataChanged()
        {
            if (_detectChanges)
            {
                if (_dtSelected.Rows.Count > 0)
                {
                    if (!btnSave.Enabled)
                    {
                        btnSave.Enabled = true;
                    }
                    if (drpSetNames.SelectedIndex >= 0
                        && txtSetName.Text.Length == 0
                        && drpSetNames.SelectedValue.ToString() != "New..")
                    {
                        txtSetName.Text = drpSetNames.SelectedValue.ToString();
                    }
                }
                else
                {
                    btnSave.Enabled = false;
                }
            }
        }
        private void dgBranchList_CurrentCellChanged(object sender, System.EventArgs e)
        {
            // The cell will change either because the user tabbed or clicked
            // with the mouse. In both cases the focus is prevented from landing
            // on the tick box. In this event we don't know whether the mouse
            // was clicked, so just record the row index if the focus was 
            // aiming for the tick box. If the mouse_up event fires and this
            // row was recorded then that event will know the tick was clicked.
            try
            {
                Function = "dgBranchList_CurrentCellChanged";

                if (_detectChanges)
                {
                    Wait();
                    _detectChanges = false;
                    this._tickClickIndex = -1;

                    int index = dgBranchList.CurrentRowIndex;
                    if (index >= 0)
                    {
                        if (dgBranchList.CurrentCell.ColumnNumber == this._branchIndex)
                        {
                            // The focus is on the branch tick column but we don't know
                            // whether the user tabbed here or clicked with the mouse.
                            // So move the focus to the previous column on the row,
                            // but first record the row number for the MouseUp event.
                            this._tickClickIndex = index;
                            this.SetBranchFocus(index, false);
                        }
                    }
                    _detectChanges = true;
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
                Function = "End of dgBranchList_CurrentCellChanged";
            }
        }

        private void dgBranchList_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                Function = "dgBranchList_MouseUp";

                if (_detectChanges)
                {
                    Wait();
                    _detectChanges = false;

                    if (e.Button == MouseButtons.Left)
                    {
                        if (_tickClickIndex >= 0)
                        {
                            // The dgBranchList_CurrentCellChanged event has fired first
                            // and that moved the focus to another column in case the user
                            // tabbed to the tick box. However, this event means the user
                            // clicked the tick.
                            this.SetBranchFocus(_tickClickIndex, true);
                            _tickClickIndex = -1;
                        }
                        else
                        {
                            int index = dgBranchList.CurrentRowIndex;
                            if (index >= 0)
                            {
                                if (dgBranchList.CurrentCell.ColumnNumber == this._branchIndex)
                                {
                                    // The user has clicked on the same tick box again so the
                                    // dgBranchList_CurrentCellChanged event has not fired.
                                    this.SetBranchFocus(index, true);
                                }
                            }
                        }
                    }
                    _detectChanges = true;
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
                Function = "End of dgBranchList_MouseUp";
            }
        }

        private void SetBranchFocus(int index, bool changeTick)
        {
            // The tick box will never take focus.
            // The Branch No can only take focus.
            // So the user cannot tab onto the tick box nor past the end of a row.
            DataView branchListView = this._branchList.DefaultView;
            DataRowView branchRow = branchListView[index];

            if (changeTick)
            {
                branchRow[CN.Selected] = !(Convert.ToBoolean(branchRow[CN.Selected]));
                DisableUseAllowSave();
            }
            // Move the focus to the Branch No
            dgBranchList.CurrentCell = new DataGridCell(index, 1);
        }

        private void btnClearTickAll_Click(object sender, System.EventArgs e)
        {
            foreach (DataRow branchRow in _branchList.Rows)
            {
                branchRow[CN.Selected] = 0;
            }
            DisableUseAllowSave();
        }

        private void btnSetTickAll_Click(object sender, System.EventArgs e)
        {
            foreach (DataRow branchRow in _branchList.Rows)
            {
                branchRow[CN.Selected] = 1;
            }
            DisableUseAllowSave();
        }

        private void buttonAddText_Click(object sender, System.EventArgs e)
        {
            string categoryCode = textItemEntry.Text;
            bool found = false;
            // ensure category not already added
            foreach (DataRow dr in _dtSelected.Rows)
            {
                if (categoryCode == dr[0].ToString())
                {
                    found = true;
                    MessageBox.Show("Item already added.");
                    break;
                }
            }

            if (!found)
            {
                WS10.CategoryItem catItem = SetDataManager.GetCategoryItem(categoryCode);

                if (catItem.Code != null)
                {

                    DataRow row = this._dtSelected.NewRow();

                    //row[0] = textItemEntry.Text;
                    row[0] = catItem.Code;
                    row[1] = catItem.CodeDescript;

                    _dtSelected.Rows.Add(row);
                    textItemEntry.Text = "";

                    TextDataChanged();
                    DisableUseAllowSave();
                }
                else
                {
                    ShowError("Code not found.");
                }
            }
        }

        private void drpSetNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            //SelectSetName
            //string setName = drpSetNames.SelectedValue.ToString();
            //SelectSetName(setName);

        }

        private void udValue_ValueChanged(object sender, EventArgs e)
        {
            TextDataChanged();
        }

    }
}
