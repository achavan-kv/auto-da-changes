using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using STL.Common;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using STL.Common.Static;


namespace STL.PL
{
    public partial class MmiMatrix : CommonForm
    {
        private string _error = string.Empty;
        private bool _userChanged = false;
        private DataSet _matrix = null;
        private bool _savePending = false;

        #region " Public Methods "
        public MmiMatrix(TranslationDummy d)
        {
            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile });
        }

        public MmiMatrix(Form root, Form parent)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            FormRoot = root;
            FormParent = parent;
            HashMenus();
            this.ApplyRoleRestrictions();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile });
        }

        #endregion " Public Methods "

        #region " Private Methods "
        private void HashMenus()
        {
            dynamicMenus = new Hashtable();
            dynamicMenus[this.Name + ":allowEdit"] = this.allowEdit;
        }

        private void LoadMatrix()
        {
            this._userChanged = false;
            _matrix = CreditManager.GetMmiMatrix(out _error);
            if (_error.Length > 0)
                ShowError(_error);
            else
            {
                foreach (DataTable dt in _matrix.Tables)
                {
                    if (dt.TableName == TN.MmiMatrix)
                    {
                        dgMmiMatrix.DataSource = dt;
                        SetGridHeader();
                        EnableFields(this.allowEdit.Enabled);
                        SetGridCellStyle();
                        this.dgMmiMatrix.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
                    }
                }
            }

            this._userChanged = true;
        }

        private void SetGridCellStyle()
        {
            dgMmiMatrix.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgMmiMatrix.Columns[CN.Label].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgMmiMatrix.Columns[CN.FromScore].DefaultCellStyle.Format = "N0";
            dgMmiMatrix.Columns[CN.FromScore].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgMmiMatrix.Columns[CN.ToScore].DefaultCellStyle.Format = "N0";
            dgMmiMatrix.Columns[CN.ToScore].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgMmiMatrix.Columns[CN.MmiPercentage].DefaultCellStyle.Format = "F3";
            dgMmiMatrix.Columns[CN.MmiPercentage].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void SetGridHeader()
        {
            dgMmiMatrix.Columns[CN.Label].HeaderText = GetResource("T_SCORE");
            dgMmiMatrix.Columns[CN.FromScore].HeaderText = GetResource("T_FROMSCORE");
            dgMmiMatrix.Columns[CN.ToScore].HeaderText = GetResource("T_TOSCORE");
            dgMmiMatrix.Columns[CN.MmiPercentage].HeaderText = GetResource("T_MMIPERCENTAGE");
        }

        private void EnableFields(bool enabled)
        {
            dgMmiMatrix.AllowUserToAddRows = enabled;
            dgMmiMatrix.Columns[CN.Label].ReadOnly = !enabled;
            dgMmiMatrix.Columns[CN.FromScore].ReadOnly = !enabled;
            dgMmiMatrix.Columns[CN.ToScore].ReadOnly = !enabled;
            dgMmiMatrix.Columns[CN.MmiPercentage].ReadOnly = !enabled;
            btnSave.Visible = enabled;
        }

        private bool SaveMatrix(string fileName, bool newImport)
        {
            bool valid = true;
            this._userChanged = false;

            // Sort the label on the points ranges
            DataRow[] scoreRows = _matrix.Tables[TN.MmiMatrix].Select("1=1", "FromScore ASC");

            // The points ranges must be contiguous
            for (int rowNo = 0; rowNo < scoreRows.Length; rowNo++)
            {
                // Each range should be from a lower value to a higher value
                valid = valid && ((short)scoreRows[rowNo][CN.FromScore] <= (short)scoreRows[rowNo][CN.ToScore]);
                // Each range should follow on from the previous range
                if (rowNo == 0)
                    valid = valid && ((short)scoreRows[rowNo][CN.FromScore] == 0);
                else
                    valid = valid && ((short)scoreRows[rowNo - 1][CN.ToScore] == (short)scoreRows[rowNo][CN.FromScore] - 1);
            }

            if (valid)
            {
                errorProvider1.SetError(dgMmiMatrix, string.Empty);

                CreditManager.SaveMmiMatrix(_matrix, out _error);

                if (_error.Length > 0)
                {
                    valid = false;
                    ShowError(_error);
                }
                else
                {
                    this._savePending = false;
                }
            }
            else
                errorProvider1.SetError(dgMmiMatrix, GetResource("M_MMILEVELOVERLAP"));

            this._userChanged = true;
            return valid;
        }

        #endregion " Private Methods "

        #region " Events "
        private void MmiMatrix_Load(object sender, EventArgs e)
        {
            try
            {
                Function = "MmiMatrix_Load";
                Wait();
                this.LoadMatrix();
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

        private void btnSave_Click(object sender, System.EventArgs e)
        {
            try
            {
                Function = "btnSave_Click";
                Wait();
                SaveMatrix(string.Empty, false);
                ((MainForm)FormRoot).StatusBarText = GetResource("M_MATRIXSAVED");
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

        private void menuExit_Click(object sender, System.EventArgs e)
        {
            try
            {
                Function = "menuExit_Click";
                Wait();
                CloseTab();
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

        private void dgMmiMatrix_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            bool error = false;
            try
            {
                if (this.dgMmiMatrix[e.ColumnIndex, e.RowIndex].Value != null)
                {
                    string value = this.dgMmiMatrix[e.ColumnIndex, e.RowIndex].Value.ToString();

                    if (String.IsNullOrEmpty(((DataGridView)sender)[e.ColumnIndex, e.RowIndex].Value.ToString()))
                    {
                        this.dgMmiMatrix[e.ColumnIndex, e.RowIndex].ErrorText = GetResource("M_ENTERMANDATORY");
                        error = true;
                    }

                    if (!error && this.dgMmiMatrix.Columns[e.ColumnIndex].Name == CN.Label)
                    {
                        if (value.Length > 50)
                        {
                            this.dgMmiMatrix[e.ColumnIndex, e.RowIndex].ErrorText = "Label can only be 50 letters.";
                            error = true;
                        }
                    }

                    if (!error && this.dgMmiMatrix.Columns[e.ColumnIndex].Name == CN.FromScore ||
                           this.dgMmiMatrix.Columns[e.ColumnIndex].Name == CN.ToScore ||
                           this.dgMmiMatrix.Columns[e.ColumnIndex].Name == CN.MmiPercentage)
                    {
                        // Validate the numeric cells
                        bool numberOK = false;

                        if (!String.IsNullOrEmpty(value.Trim()))
                        {
                            numberOK = numberOK ||
                                     ((this.dgMmiMatrix.Columns[e.ColumnIndex].Name == CN.FromScore ||
                                       this.dgMmiMatrix.Columns[e.ColumnIndex].Name == CN.ToScore) &&
                                      IsPositive(value.ToString()));

                            numberOK = numberOK ||
                                     ((this.dgMmiMatrix.Columns[e.ColumnIndex].Name == CN.MmiPercentage) &&
                                      IsStrictNumeric(value));
                        }

                        if (numberOK && Convert.ToDecimal(value) < 0)
                            numberOK = false;

                        if (!numberOK)
                        {
                            this.dgMmiMatrix.Rows[e.RowIndex].ErrorText = GetResource("M_POSITIVENUM");
                            error = true;
                        }
                    }

                    if (!error)
                    {
                        this.dgMmiMatrix[e.ColumnIndex, e.RowIndex].ErrorText = string.Empty;
                        this.dgMmiMatrix.Rows[e.RowIndex].ErrorText = string.Empty;
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

        private void dgMmiMatrix_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            try
            {
                e.Control.KeyPress -= new KeyPressEventHandler(PositiveColumn_KeyPress);

                if (this.dgMmiMatrix.CurrentCell.ColumnIndex == 1 ||
                    this.dgMmiMatrix.CurrentCell.ColumnIndex == 2 ||
                    this.dgMmiMatrix.CurrentCell.ColumnIndex == 3
                    )
                {
                    TextBox tb = e.Control as TextBox;
                    if (tb != null)
                    {
                        tb.KeyPress += new KeyPressEventHandler(PositiveColumn_KeyPress);
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

        private void PositiveColumn_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar)
                && !char.IsDigit(e.KeyChar)
                && e.KeyChar != '.')
                {
                    e.Handled = true;
                }

                // only allow one decimal point
                if (e.KeyChar == '.'
                    && (sender as TextBox).Text.IndexOf('.') > -1)
                {
                    e.Handled = true;
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

        #endregion " Events "

    }
}



