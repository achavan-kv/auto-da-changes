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
    /// This form is used to allow the user to make selections from an 'Available' list
    /// of items to a 'Selected' list. It expects to be passed a DataSet containing 2 
    /// DataTables - DataTable[0] contains the 'Available' Data, and DataTable[1] contains
    /// the 'Selected' Data. DataTable[1] can be empty but should have the same structure
    /// as DataTable[0]. The form assumes that the first DataColumn of each table contains
    /// the 'key' or 'code' that is being selected and uniquely identifies each row - ie.
    /// the primary key.
    /// </summary>
    public class SelectionPane : CommonForm
    {
        private DataSet _selectionDataSet;
        private string _objectName;
        private int _col1Width;
        private int _col2Width;
        private TextBox _parentTextBox;
        private DataTable _dtAvailable;
        private DataTable _dtSelected;
        private DataView _availableView;
        private DataView _selectedView;
        //private string Error; // = "";
        private System.Windows.Forms.DataGrid dgAvailable;
        private System.Windows.Forms.DataGrid dgSelected;
        private System.Windows.Forms.Button btnAddSelection;
        private System.Windows.Forms.Button btnRemoveSelection;
        private System.Windows.Forms.Button btnAddAll;
        private System.Windows.Forms.Button btnRemoveAll;
        private System.Windows.Forms.GroupBox gbSetDetailsSelection;
        private System.Windows.Forms.Button btnUse;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public SelectionPane(DataSet selectionDataSet, string objectName, int col1Width, int col2Width, TextBox parentTextBox)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            _selectionDataSet = selectionDataSet;
            _objectName = objectName;
            this._col1Width = col1Width;
            this._col2Width = col2Width;
            this._parentTextBox = parentTextBox;
        }

        public SelectionPane(DataSet selectionDataSet, string objectName, int col1Width, int col2Width)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            _selectionDataSet = selectionDataSet;
            _objectName = objectName;
            this._col1Width = col1Width;
            this._col2Width = col2Width;
            this._parentTextBox = null;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if(components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

		#region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dgAvailable = new System.Windows.Forms.DataGrid();
            this.dgSelected = new System.Windows.Forms.DataGrid();
            this.btnAddSelection = new System.Windows.Forms.Button();
            this.btnRemoveSelection = new System.Windows.Forms.Button();
            this.btnAddAll = new System.Windows.Forms.Button();
            this.btnRemoveAll = new System.Windows.Forms.Button();
            this.gbSetDetailsSelection = new System.Windows.Forms.GroupBox();
            this.btnUse = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgAvailable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgSelected)).BeginInit();
            this.gbSetDetailsSelection.SuspendLayout();
            this.SuspendLayout();
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
            this.dgAvailable.Size = new System.Drawing.Size(280, 296);
            this.dgAvailable.TabIndex = 19;
            // 
            // dgSelected
            // 
            this.dgSelected.CaptionText = "                                Selected";
            this.dgSelected.ColumnHeadersVisible = false;
            this.dgSelected.DataMember = "";
            this.dgSelected.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgSelected.Location = new System.Drawing.Point(344, 24);
            this.dgSelected.Name = "dgSelected";
            this.dgSelected.ReadOnly = true;
            this.dgSelected.Size = new System.Drawing.Size(280, 296);
            this.dgSelected.TabIndex = 20;
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
            // btnRemoveAll
            // 
            this.btnRemoveAll.Location = new System.Drawing.Point(296, 200);
            this.btnRemoveAll.Name = "btnRemoveAll";
            this.btnRemoveAll.Size = new System.Drawing.Size(40, 23);
            this.btnRemoveAll.TabIndex = 25;
            this.btnRemoveAll.Text = "<<";
            this.btnRemoveAll.Click += new System.EventHandler(this.btnRemoveAll_Click);
            // 
            // gbSetDetailsSelection
            // 
            this.gbSetDetailsSelection.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                                                this.btnUse,
                                                                                                this.btnRemoveAll,
                                                                                                this.btnRemoveSelection,
                                                                                                this.btnAddAll,
                                                                                                this.btnAddSelection,
                                                                                                this.dgAvailable,
                                                                                                this.dgSelected});
            this.gbSetDetailsSelection.Location = new System.Drawing.Point(8, 8);
            this.gbSetDetailsSelection.Name = "gbSetDetailsSelection";
            this.gbSetDetailsSelection.Size = new System.Drawing.Size(632, 328);
            this.gbSetDetailsSelection.TabIndex = 27;
            this.gbSetDetailsSelection.TabStop = false;
            this.gbSetDetailsSelection.Text = "Use the buttons to modify selected details ";
            // 
            // btnUse
            // 
            this.btnUse.Location = new System.Drawing.Point(296, 288);
            this.btnUse.Name = "btnUse";
            this.btnUse.Size = new System.Drawing.Size(40, 23);
            this.btnUse.TabIndex = 26;
            this.btnUse.Text = "Use";
            this.btnUse.Click += new System.EventHandler(this.btnUse_Click);
            // 
            // SelectionPane
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(648, 349);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.gbSetDetailsSelection});
            this.Name = "SelectionPane";
            this.Text = "Select ";
            this.Load += new System.EventHandler(this.SelectionPane_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgAvailable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgSelected)).EndInit();
            this.gbSetDetailsSelection.ResumeLayout(false);
            this.ResumeLayout(false);

        }
		#endregion

        private void SelectionPane_Load(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                LoadAvailableData();
                LoadSelectedData();
                RefreshAvailableGrid();
                this.Text += this._objectName;
            }
            catch(Exception ex)
            {
                Catch(ex, "SelectionPane_Load");
            }
            finally
            {
                StopWait();
            }
        }

        private void LoadAvailableData()
        {
            _dtAvailable = this._selectionDataSet.Tables[0];
            _dtAvailable.Columns[0].ColumnName = CN.Code;
            _dtAvailable.PrimaryKey = new DataColumn[]{_dtAvailable.Columns[0]};

            _availableView = new DataView(_dtAvailable);

            if (dgAvailable.TableStyles.Count == 0)
            {
                DataGridTableStyle tabStyle = new DataGridTableStyle();
                tabStyle.MappingName = _availableView.Table.TableName;

                dgAvailable.TableStyles.Clear();
                dgAvailable.TableStyles.Add(tabStyle);
                dgAvailable.DataSource = _availableView;

                tabStyle.GridColumnStyles[0].Width = this._col1Width;
                tabStyle.GridColumnStyles[0].ReadOnly = true;
                tabStyle.GridColumnStyles[0].HeaderText = string.Empty;

                tabStyle.GridColumnStyles[1].Width = this._col2Width;
                tabStyle.GridColumnStyles[1].ReadOnly = true;
                tabStyle.GridColumnStyles[1].HeaderText = string.Empty;
            }
            _availableView.Sort = CN.Code + " ASC ";
        }

        private void LoadSelectedData()
        {
            _dtSelected = this._selectionDataSet.Tables[1];
            _dtSelected.Columns[0].ColumnName = CN.Code;
            _dtSelected.PrimaryKey = new DataColumn[]{_dtSelected.Columns[0]};

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

                tabStyle.GridColumnStyles[1].Width = this._col2Width;
                tabStyle.GridColumnStyles[1].ReadOnly = true;
                tabStyle.GridColumnStyles[1].HeaderText = string.Empty;
            }
            _selectedView.Sort = CN.Code + " ASC ";
        }

        private void btnAddSelection_Click(object sender, System.EventArgs e)
        {
            Function = "btnAddSelection_Click";

            try
            {
                for (int index = _availableView.Count -1; index>=0; index--)
                {
                    if(dgAvailable.IsSelected(index) || index == dgAvailable.CurrentRowIndex)
                    {
                        SelectAvailableRow(index);
                    }
                }
                RefreshAvailableGrid();
            }
            catch(Exception ex)
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
            if (_dtSelected.Rows.Count >0)
            {
                btnUse.Enabled = true;
                string filter = CN.Code + " not in (";
                foreach(DataRowView row in _selectedView)
                {
                    filter += "'" + (string)row[CN.Code] + "',";
                }

                filter += ")";
                _availableView.RowFilter = filter;
            }
            else 
            {
                _availableView.RowFilter = string.Empty;
                btnUse.Enabled = false;
            }
        }

        private void btnRemoveSelection_Click(object sender, System.EventArgs e)
        {
            Function = "btnRemoveSelection_Click";

            try
            {
                //Get a list of the codes that need to be removed from the selected table
                ArrayList codes = new ArrayList();
                for (int index = _selectedView.Count -1; index>=0; index--)
                {
                    if(dgSelected.IsSelected(index) || index == dgSelected.CurrentRowIndex)
                    {
                        codes.Add(dgSelected[index,0].ToString());
                    }
                }
                DataRow selectedRow;
                for (int i=_dtSelected.Rows.Count - 1;i>=0;i--)
                {
                    selectedRow = _dtSelected.Rows[i];
                    if (codes.Contains(selectedRow[CN.Code].ToString())) 
                    {
                        _dtSelected.Rows.Remove(selectedRow);
                    }
                }
                _dtSelected.AcceptChanges();
                RefreshAvailableGrid();
            }
            catch(Exception ex)
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
                for (int index = _availableView.Count -1; index>=0; index--)
                {
                    SelectAvailableRow(index);
                }
                RefreshAvailableGrid();
            }
            catch(Exception ex)
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
                for (int i=_dtSelected.Rows.Count - 1;i>=0;i--)
                {
                    selectedRow = _dtSelected.Rows[i];
                    _dtSelected.Rows.Remove(selectedRow);
                }
                _dtSelected.AcceptChanges();
                RefreshAvailableGrid();
            }
            catch(Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                Function = "End of btnRemoveAll_Click";
            }        
        }

        private void UpdateParentFormText() 
        {
            if (_parentTextBox != null) 
            {
                if (this._dtSelected.Rows.Count > 0 && _availableView.Count > 0)
                {
                    string selections = _dtSelected.Rows[0][CN.Code].ToString();
                    for (int i=1; i<_dtSelected.Rows.Count; i++)
                    {
                        selections += "," + _dtSelected.Rows[i][CN.Code].ToString();
                    }
                    _parentTextBox.Text = selections;
                }
                else 
                {
                    _parentTextBox.Text = string.Empty;
                }
            }
            this.Close();
        }

        private void SelectAvailableRow(int selectIndex) 
        {
            DataRow row = _dtSelected.NewRow();

            row[0] = dgAvailable[selectIndex, 0];
            row[1] = dgAvailable[selectIndex, 1];
            _dtSelected.Rows.Add(row);
        }

        private void btnUse_Click(object sender, System.EventArgs e)
        {
            UpdateParentFormText();
        }

    }
}
