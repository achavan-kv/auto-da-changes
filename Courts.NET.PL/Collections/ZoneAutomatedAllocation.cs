using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using STL.Common.Collections;
using System.Windows.Forms;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using STL.PL.Collections.CollectionsClasses;
using Crownwood.Magic.Menus;
using Microsoft.Win32;

namespace STL.PL.Collections
{
    public partial class ZoneAutomatedAllocation : CommonForm
    {
        #region --Private Member Variables------------------------------------------------
        //--------------------------------------------------------------------------------

        private const int MIN_ACCT_RANGE_START = 0;
        //private const int MIN_ACCT_RANGE_END = 100;
        private const int MIN_ACCT_RANGE_END = 10000; //IP - 10/05/10 - UAT(10) UAT5.2
        private const int MAX_ACCT_RANGE_START = 0;
        //private const int MAX_ACCT_RANGE_END = 100;
        private const int MAX_ACCT_RANGE_END = 10000; //IP - 10/05/10 - UAT(10) UAT5.2
        private const int ALLOC_RANK_RANGE_START = 1;
        private const int ALLOC_RANK_RANGE_END = 5;
        private string reallocateTooltipText = "";

        private string error = "";
        private DataTable dtZoneAddressFields = null; //Zone Setup Tab
        private List<Int32> reusableOrClauseNumbers = new List<Int32>(); //Zone Setup Tab
        DataTable dtGlobalEmpZoneAlloc = new DataTable(); //       

        //--------------------------------------------------------------------------------
        #endregion -----------------------------------------------------------------------

        public ZoneAutomatedAllocation(Form root, Form parent)
        {
            dtZoneAddressFields = CreateZoneAddressFieldDT();
            InitializeComponent();
            FormRoot = root;
            FormParent = parent;
        }

        private void ZoneAutomatedAllocation_Load(object sender, EventArgs e)
        {
            try
            {
                if (!PopulateInitialData_ZoneSetup())
                    return;

                if (!PopulateInitialData_EmployeeAlloc())
                    return;

                reallocateTooltipText = GetResource("TT_REALLOCATEWARNING");
            }
            catch (Exception ex)
            {
                Catch(ex, "ZoneAutomatedAllocation_Load");
            }
        }

        private bool IsExcelInstalled()
        {
            RegistryKey key = Registry.ClassesRoot;
            RegistryKey excelKey = key.OpenSubKey("Excel.Application");
            return excelKey != null;
        }

        #region --Zone Setup Tab----------------------------------------------------------
        //--------------------------------------------------------------------------------

        enum Alphabet
        {
            A, B, C, E, F, G, H, I, J, K
        }

        private int AlphabetToNumber(string strAlphabet)
        {
            if (strAlphabet == null || strAlphabet.Trim().Length == 0)
                return 0;

            int rtnValue = 0;
            byte power = 0;
            strAlphabet = strAlphabet.Trim();

            for (int i = strAlphabet.Length - 1; i >= 0; i--)
            {
                if (Enum.IsDefined(typeof(Alphabet), strAlphabet[i].ToString()))
                {
                    Alphabet alp = (Alphabet)Enum.Parse(typeof(Alphabet), strAlphabet[i].ToString(), true);
                    rtnValue += (int)alp * (int)Math.Pow(10, power);
                    power++;
                }
            }

            return rtnValue;
        }

        private string NumberToAlphabet(int number)
        {
            if (number == 0)
                return "";

            string strReturn = "";
            foreach (char c in number.ToString())
            {
                string name = Enum.GetName(typeof(Alphabet), Convert.ToInt32(c.ToString()));
                strReturn += name;
            }

            return strReturn;
        }

        private DataTable CreateZoneAddressFieldDT()
        {
            DataTable dt = new DataTable("AddressField");
            dt.Columns.Add("DisplayName");
            dt.Columns.Add("ColumnName");

            DataRow drAddressField = dt.NewRow();
            drAddressField["DisplayName"] = "Address 1";
            drAddressField["ColumnName"] = CN.cusaddr1;
            dt.Rows.Add(drAddressField);

            drAddressField = dt.NewRow();
            drAddressField["DisplayName"] = "Address 2";
            drAddressField["ColumnName"] = CN.cusaddr2;
            dt.Rows.Add(drAddressField);

            drAddressField = dt.NewRow();
            drAddressField["DisplayName"] = "Address 3";
            drAddressField["ColumnName"] = CN.cusaddr3;
            dt.Rows.Add(drAddressField);

            drAddressField = dt.NewRow();
            drAddressField["DisplayName"] = "Post Code";
            drAddressField["ColumnName"] = CN.cuspocode;
            dt.Rows.Add(drAddressField);

            drAddressField = dt.NewRow();
            drAddressField["DisplayName"] = "Delivery Area";
            drAddressField["ColumnName"] = CN.DeliveryArea;
            dt.Rows.Add(drAddressField);

            return dt;
        }

        private bool PopulateInitialData_ZoneSetup()
        {
            //--Rules Grid---------------------------------------------------
            DataSet dsZoneRules = CollectionsManager.LoadZoneRules("", out Error); ;
            if (error.Length > 0)
            {
                ShowError(error);
                return false;
            }
            else if (dsZoneRules.Tables.Count < 1)
            {
                return false;
            }
            dsZoneRules.Tables[0].Columns.Add("Operator"); //--Temp DataColumn to show operator
            dsZoneRules.Tables[0].Columns.Add("OrClauseNumberRep", typeof(int)); //--Temp DataColumn to hold number representaion of orClause

            foreach (DataRow dr in dsZoneRules.Tables[0].Rows)
            {
                dr["Operator"] = Convert.ToBoolean(dr[CN.NotLike]) ? "Not Like" : "Like";       //CR1084
                dr["OrClauseNumberRep"] = AlphabetToNumber(dr[CN.Or_Clause].ToString());
            }

            cmbColumn_columnName.DataSource = dtZoneAddressFields.Copy();
            cmbColumn_columnName.DisplayMember = "DisplayName";
            cmbColumn_columnName.ValueMember = "ColumnName";

            dgvRules.DataSource = dsZoneRules.Tables[0].DefaultView;

            foreach (DataGridViewColumn column in dgvRules.Columns)
            {
                if (column.DataPropertyName.ToLower() == CN.Column_Name.ToLower() || column.DataPropertyName.ToLower() == CN.Query.ToLower() ||
                        column.DataPropertyName.ToLower() == CN.Or_Clause.ToLower() || column.DataPropertyName.ToLower() == "operator")
                {
                    column.Visible = true;
                }
                else
                {
                    column.Visible = false;
                }
            }
            //---------------------------------------------------------------

            //--Zone---------------------------------------------------------
            DataSet dsZone = CollectionsManager.GetZones(out error);
            if (error.Length > 0)
            {
                ShowError(error);
                return false;
            }
            else if (dsZone.Tables.Count < 1)
            {
                return false;
            }

            cmbZones.SelectedIndexChanged -= new EventHandler(cmbZones_SelectedIndexChanged);
            cmbZones.DataSource = dsZone.Tables[0];
            cmbZones.DisplayMember = "concatDesc";
            cmbZones.ValueMember = CN.Zone;
            cmbZones.SelectedIndexChanged += new EventHandler(cmbZones_SelectedIndexChanged);

            if (cmbZones.Items.Count > 0 && cmbZones.SelectedIndex != -1)
                cmbZones_SelectedIndexChanged(cmbZones, null);
            //---------------------------------------------------------------


            //--Address Fields-----------------------------------------------
            lstAddressFields.DataSource = dtZoneAddressFields;
            lstAddressFields.DisplayMember = "DisplayName";
            lstAddressFields.ValueMember = "ColumnName";
            //---------------------------------------------------------------

            CalcGridSize();

            return true;
        }

        private void RemoveZoneRuleDuplicateRows()
        {
            if (dgvRules.DataSource == null)
            {
                return;
            }

            DataTable dtSource = ((DataView)dgvRules.DataSource).Table;
            DataRow[] drArrayDuplicates = null;
            int lastIndexScanned = 0;

            while (lastIndexScanned < dtSource.Rows.Count - 1)
            {
                for (int i = lastIndexScanned; i < dtSource.Rows.Count - 1; i++)
                {
                    drArrayDuplicates = dtSource.Select(CN.Zone + " = '" + dtSource.Rows[i][CN.Zone] + "' and " +
                                                                CN.Column_Name + " = '" + dtSource.Rows[i][CN.Column_Name] + "' and " +
                                                                CN.Query + " = '" + dtSource.Rows[i][CN.Query].ToString().Replace("'", "''") + "' and " +
                                                                CN.Or_Clause + " = '" + dtSource.Rows[i][CN.Or_Clause] + "'");

                    if (drArrayDuplicates.Length > 1) //Duplicates found
                    {
                        lastIndexScanned = i;
                        break;
                    }
                }

                if (drArrayDuplicates.Length <= 1)
                    break;

                //--If duplicates found removing all except the first one in the array ----
                for (int i = 1; i < drArrayDuplicates.Length; i++)
                {
                    dtSource.Rows.Remove(drArrayDuplicates[i]);
                }
                //-------------------------------------------------------------------------


                //-------------------------------------------------------------------------
                //-- Clear previous 'OR' conditions applied jointly with the rows removed 
                DataRow[] drArraySelected = dtSource.Select("OrClauseNumberRep = " + drArrayDuplicates[0]["OrClauseNumberRep"].ToString());

                if (drArraySelected.Length == 1 && Convert.ToInt32(drArraySelected[0]["OrClauseNumberRep"]) != 0) //If there's a single row left without any joining rows
                {
                    int currentOrClauseNumber = Convert.ToInt32(drArraySelected[0]["OrClauseNumberRep"]);
                    if (reusableOrClauseNumbers.Contains(currentOrClauseNumber) == false)
                        reusableOrClauseNumbers.Add(currentOrClauseNumber);

                    drArraySelected[0][CN.Or_Clause] = "";
                    drArraySelected[0]["OrClauseNumberRep"] = 0;
                }
                //-------------------------------------------------------------------------
            }

            dgvRules.Refresh();
        }

        private Color GenerateColor(int value)
        {
            value = (value % 256);
            //Please don't bother about these peculiar calculations. 
            //They are here to make sure the method returns contrastingly different colors
            //if two consecutive numbers are passed in, for eg. like 20 & 21
            int r = ((value * value) * 3) % 255;
            int g = ((value * 23) + 59) % 255;
            int b = ((r + g) * ((value % 2) + 1)) % 255;
            return Color.FromArgb(r, g, b);
        }

        private void btnAddZone_Click(object sender, EventArgs e)
        {
            try
            {
                errorProvider1.SetError(txtZoneCode, "");
                errorProvider1.SetError(txtZoneDescription, "");

                //-- Validating the Fields -----------------------------------------
                bool valid = true;
                if (txtZoneCode.Text.Trim() == "")
                {
                    errorProvider1.SetError(txtZoneCode, GetResource("M_ENTERMANDATORY"));
                    valid = false;
                }

                if (txtZoneDescription.Text.Trim() == "")
                {
                    errorProvider1.SetError(txtZoneDescription, GetResource("M_ENTERMANDATORY"));
                    valid = false;
                }

                //-- UAT(5.2) 759------------------------------------------------------------------
                foreach (char c in txtZoneCode.Text.Trim())
                {
                    if (Char.IsDigit(c))
                    {
                        errorProvider1.SetError(txtZoneCode, GetResource("M_NONNUMERICVALUE"));
                        valid = false;
                        break;
                    }
                }

                foreach (char c in txtZoneDescription.Text.Trim())
                {
                    if (Char.IsDigit(c))
                    {
                        errorProvider1.SetError(txtZoneDescription, GetResource("M_NONNUMERICVALUE"));
                        valid = false;
                        break;
                    }
                }
                //---------------------------------------------------------------------------------


                if (valid == false)
                    return;
                //------------------------------------------------------------------

                if (cmbZones.DataSource != null)
                {
                    DataTable dtSource = ((DataTable)cmbZones.DataSource);
                    DataRow[] drArraySelection = dtSource.Select(CN.Zone + " = '" + txtZoneCode.Text.Trim() + "'");

                    if (drArraySelection.Length == 0 || drArraySelection[0]["ZoneDescription"].ToString() != txtZoneDescription.Text.Trim())
                    {
                        CollectionsManager.SaveZones(txtZoneCode.Text.Trim(), txtZoneDescription.Text.Trim(), out error); //An update
                        if (error.Length > 0)
                        {
                            ShowError(error);
                            return;
                        }
                    }

                    //-- Updating the combobox datasource ------------------------------
                    cmbZones.SelectedIndexChanged -= new EventHandler(cmbZones_SelectedIndexChanged);
                    if (drArraySelection.Length == 0) //An insert
                    {
                        DataRow dr = dtSource.NewRow();
                        dtSource.Rows.Add(dr);

                        dr["ZoneDescription"] = txtZoneDescription.Text.Trim();
                        dr[CN.Zone] = txtZoneCode.Text.Trim();
                        dr["concatDesc"] = txtZoneCode.Text.Trim() + " " + txtZoneDescription.Text.Trim();
                    }
                    else if (drArraySelection.Length > 0 && drArraySelection[0]["ZoneDescription"].ToString() != txtZoneDescription.Text.Trim()) //An update
                    {
                        drArraySelection[0]["ZoneDescription"] = txtZoneDescription.Text.Trim();
                        drArraySelection[0]["concatDesc"] = txtZoneCode.Text.Trim() + " " + txtZoneDescription.Text.Trim();
                    }
                    cmbZones.SelectedIndexChanged += new EventHandler(cmbZones_SelectedIndexChanged);
                    //------------------------------------------------------------------

                    cmbZones.Refresh();
                    cmbZones_SelectedIndexChanged(cmbZones, null);
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnAddZone_Click");
            }
        }

        private void btnDeleteZone_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbZones.DataSource != null && cmbZones.SelectedIndex >= 0)
                {
                    DataTable dtSourceZone = ((DataTable)cmbZones.DataSource);
                    DataRow[] drArraySelectionZone = dtSourceZone.Select(CN.Zone + " = '" + cmbZones.SelectedValue.ToString() + "'");

                    if (drArraySelectionZone.Length > 0)
                    {
                        string strZone = drArraySelectionZone[0][CN.Zone].ToString();

                        //------------------------------------------------------------------------
                        CollectionsManager.DeleteZone(strZone, out error);
                        if (error.Length > 0)
                        {
                            ShowError(error);
                            return;
                        }

                        dtSourceZone.Rows.Remove(drArraySelectionZone[0]);

                        cmbZones.Refresh();
                        cmbZones_SelectedIndexChanged(cmbZones, null);
                        //------------------------------------------------------------------------


                        //------------------------------------------------------------------------
                        DataTable dtSourceRule = ((DataView)dgvRules.DataSource).Table;
                        DataRow[] drArraySelectionRule = dtSourceRule.Select(CN.Zone + " = '" + strZone + "'");

                        foreach (DataRow dr in drArraySelectionRule)
                        {
                            dtSourceRule.Rows.Remove(dr);
                        }
                        dgvRules.Refresh();
                        //------------------------------------------------------------------------
                    }


                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnDeleteZone_Click");
            }
        }

        private void cmbZones_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbZones.DataSource != null && cmbZones.SelectedIndex >= 0)
                {
                    //------------------------------------------------------------------------
                    DataTable dtSource = ((DataTable)cmbZones.DataSource);
                    DataRow[] drArraySelection = dtSource.Select(CN.Zone + " = '" + cmbZones.SelectedValue.ToString() + "'");

                    if (drArraySelection.Length > 0)
                    {
                        txtZoneCode.Text = drArraySelection[0][CN.Zone].ToString();
                        txtZoneDescription.Text = drArraySelection[0]["ZoneDescription"].ToString();
                    }
                    else
                    {
                        txtZoneCode.Text = "";
                        txtZoneDescription.Text = "";
                    }
                    //------------------------------------------------------------------------

                    //------------------------------------------------------------------------
                    if (dgvRules.DataSource != null)
                    {
                        ((DataView)dgvRules.DataSource).RowFilter = CN.Zone + " = '" + cmbZones.SelectedValue.ToString().Trim() + "'";
                        dgvRules.Refresh();
                        CalcGridSize();
                    }
                    //------------------------------------------------------------------------
                }
                else
                {
                    txtZoneCode.Text = "";
                    txtZoneDescription.Text = "";
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "cmbZones_SelectedIndexChanged");
            }
        }

        private void btnAddRow_Click(object sender, EventArgs e)
        {
            try
            {
                errorProvider1.SetError(lstAddressFields, "");
                errorProvider1.SetError(cmbZones, "");
                errorProvider1.SetError(txtValue, "");

                //-- Validating the Fields -----------------------------------------
                bool valid = true;

                if (cmbZones.SelectedIndex < 0)
                {
                    errorProvider1.SetError(cmbZones, GetResource("M_ENTERMANDATORY"));
                    valid = false;
                }

                if (lstAddressFields.SelectedIndex < 0)
                {
                    errorProvider1.SetError(lstAddressFields, GetResource("M_ENTERMANDATORY"));
                    valid = false;
                }

                if (txtValue.Text == "")
                {
                    errorProvider1.SetError(txtValue, GetResource("M_ENTERMANDATORY"));
                    valid = false;
                }

                if (valid == false)
                    return;
                //------------------------------------------------------------------

                if (dgvRules.DataSource != null)
                {
                    DataTable dtSource = ((DataView)dgvRules.DataSource).Table;

                    DataRow dr = dtSource.NewRow();
                    dtSource.Rows.Add(dr);

                    dr[CN.Zone] = cmbZones.SelectedValue.ToString();
                    dr[CN.Column_Name] = lstAddressFields.SelectedValue.ToString();
                    dr[CN.Query] = txtValue.Text;
                    dr[CN.Or_Clause] = "";
                    dr[CN.NotLike] = false;
                    dr["Operator"] = "Like";        //CR1084
                    dr["OrClauseNumberRep"] = 0;

                    txtValue.Text = "";
                }

                RemoveZoneRuleDuplicateRows();
                CalcGridSize();
            }
            catch (Exception ex)
            {
                Catch(ex, "btnAddRow_Click");
            }
        }

        //IP - 12/06/09 - Credit Collection Walkthrough Changes - moved to 
        //menuDeleteZoneCriteria_Click
        //private void btnRemoveRows_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (dgvRules.DataSource == null)
        //        {
        //            return;
        //        }

        //        DataTable dtSource = ((DataView)dgvRules.DataSource).Table;
        //        int[] removedOrClauseArray = new int[dgvRules.SelectedRows.Count];
        //        int index = 0;

        //        //-- Removing selected rows from the DataTable ----------------------------
        //        foreach (DataGridViewRow selectedRow in dgvRules.SelectedRows)
        //        {
        //            DataRow dr = ((DataView)dgvRules.DataSource)[selectedRow.Index].Row;
        //            removedOrClauseArray[index++] = Convert.ToInt32(dr["OrClauseNumberRep"]);
        //            dtSource.Rows.Remove(dr);
        //        }
        //        //-------------------------------------------------------------------------


        //        //-------------------------------------------------------------------------
        //        //-- Clear previous 'OR' conditions applied jointly with the rows removed 
        //        for (int i = 0; i < removedOrClauseArray.Length; i++)
        //        {
        //            if (removedOrClauseArray[i] != 0)
        //            {
        //                DataRow[] drArraySelected = dtSource.Select("OrClauseNumberRep = " + removedOrClauseArray[i].ToString());

        //                if (drArraySelected.Length <= 1 && reusableOrClauseNumbers.Contains(removedOrClauseArray[i]) == false)
        //                {
        //                    reusableOrClauseNumbers.Add(removedOrClauseArray[i]);
        //                }

        //                if (drArraySelected.Length == 1) //If there's a single row left without any joining rows
        //                {
        //                    drArraySelected[0][CN.Or_Clause] = "";
        //                    drArraySelected[0]["OrClauseNumberRep"] = 0;
        //                }
        //            }
        //        }
        //        //-------------------------------------------------------------------------

        //        dgvRules.Refresh();
        //        calcGridSize();
        //    }
        //    catch (Exception ex)
        //    {
        //        Catch(ex, "btnRemoveRows_Click");
        //    }
        //}

        private void dgvRules_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                if (dgvRules.Columns[e.ColumnIndex].Name == "txtColumn_orClause")
                {
                    if (e.Value != null && e.Value.ToString().Trim() != "")
                        e.CellStyle.BackColor = GenerateColor(AlphabetToNumber(e.Value.ToString()));
                    e.CellStyle.ForeColor = e.CellStyle.BackColor;
                    e.CellStyle.SelectionForeColor = e.CellStyle.SelectionBackColor;
                }
            }
            catch
            {
            }
        }

        private void btnOrClause_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvRules.DataSource == null)
                {
                    return;
                }

                if (dgvRules.SelectedRows.Count <= 1)
                {
                    errorProvider1.SetError(btnOrClause, @"'OR' condition can be applied only if more than one row is selected");
                    return;
                }

                DataTable dtSource = ((DataView)dgvRules.DataSource).Table;
                int[] modifiedOrClauseArray = new int[dgvRules.SelectedRows.Count];
                int index = 0;

                //-- Applying the new 'OR' condition --------------------------------------
                reusableOrClauseNumbers.Sort();
                int newOrClauseNumber;
                if (reusableOrClauseNumbers.Count > 0)
                {
                    newOrClauseNumber = reusableOrClauseNumbers[0];
                    reusableOrClauseNumbers.RemoveAt(0);
                }
                else
                {
                    newOrClauseNumber = Convert.ToInt32(dtSource.Compute("MAX(OrClauseNumberRep)", "")) + 1;
                }

                string newOrClauseString = NumberToAlphabet(newOrClauseNumber % 100); //return string must be two character long

                foreach (DataGridViewRow selectedRow in dgvRules.SelectedRows)
                {
                    DataRow dr = ((DataView)dgvRules.DataSource)[selectedRow.Index].Row;
                    modifiedOrClauseArray[index++] = Convert.ToInt32(dr["OrClauseNumberRep"]);

                    dr[CN.Or_Clause] = newOrClauseString;
                    dr["OrClauseNumberRep"] = newOrClauseNumber;
                }
                //-------------------------------------------------------------------------

                //-------------------------------------------------------------------------
                //-- Clear previous 'OR' conditions applied jointly with the rows modified
                for (int i = 0; i < modifiedOrClauseArray.Length; i++)
                {
                    if (modifiedOrClauseArray[i] != 0)
                    {
                        DataRow[] drArraySelected = dtSource.Select("OrClauseNumberRep = " + modifiedOrClauseArray[i].ToString());

                        if (drArraySelected.Length <= 1 && reusableOrClauseNumbers.Contains(modifiedOrClauseArray[i]) == false)
                        {
                            reusableOrClauseNumbers.Add(modifiedOrClauseArray[i]);
                        }

                        if (drArraySelected.Length == 1) //If there's a single row left without any joining rows
                        {
                            drArraySelected[0][CN.Or_Clause] = "";
                            drArraySelected[0]["OrClauseNumberRep"] = 0;
                        }
                    }
                }
                //-------------------------------------------------------------------------

                dgvRules.Refresh();
            }
            catch (Exception ex)
            {
                Catch(ex, "btnOrClause_Click");
            }
        }

        private void btnOrClause_Leave(object sender, EventArgs e)
        {
            try
            {
                errorProvider1.SetError(btnOrClause, "");
            }
            catch
            {
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            try
            {
                btnSaveRule_Click(sender, e); //save first 
                if (cmbZones.Items.Count == 0 || cmbZones.SelectedIndex < 0)
                {
                    //Show Message Box
                    return;
                }

                Cursor.Current = Cursors.WaitCursor;

                CollectionsManager.ApplyZones(cmbZones.SelectedValue.ToString(), out error);

                if (error.Length > 0)
                {
                    ShowError(error);
                }
                else
                {
                    //TODO
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnApply_Click");
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void btnApplyAll_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                CollectionsManager.ApplyZones("All", out error);

                if (error.Length > 0)
                {
                    ShowError(error);
                }
                else
                {
                    //TODO
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnApplyAll_Click");
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void btnSaveRule_Click(object sender, EventArgs e)
        {
            try
            {
                RemoveZoneRuleDuplicateRows();

                if (dgvRules.DataSource == null || cmbZones.SelectedIndex < 0)
                {
                    return;
                }

                Cursor.Current = Cursors.WaitCursor;

                //-----------------------------------------------------------
                DataSet dsSave = new DataSet();
                DataTable dtSave = new DataTable();
                dtSave.Columns.Add(CN.Zone);
                dtSave.Columns.Add(CN.Column_Name);
                dtSave.Columns.Add(CN.Query);
                dtSave.Columns.Add(CN.Or_Clause);
                dtSave.Columns.Add(CN.NotLike);
                dsSave.Tables.Add(dtSave);
                //-----------------------------------------------------------

                //-----------------------------------------------------------
                DataTable dtSource = ((DataView)dgvRules.DataSource).Table;
                DataRow[] drArraySelected = dtSource.Select(CN.Zone + " = '" + cmbZones.SelectedValue.ToString() + "'");

                foreach (DataRow drSelected in drArraySelected)
                {
                    DataRow drSave = dtSave.NewRow();
                    dtSave.Rows.Add(drSave);

                    drSave[CN.Zone] = drSelected[CN.Zone];
                    drSave[CN.Column_Name] = drSelected[CN.Column_Name];
                    drSave[CN.Query] = drSelected[CN.Query];
                    drSave[CN.Or_Clause] = drSelected[CN.Or_Clause];
                    drSave[CN.NotLike] = drSelected["Operator"].ToString() == "Not Like" ? true : false;        //CR1084

                }
                //-----------------------------------------------------------

                CollectionsManager.SaveZoneRule(cmbZones.SelectedValue.ToString(), dsSave, out error);

                if (error.Length > 0)
                {
                    ShowError(error);
                }
                else
                {
                    //TODO
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnSaveRule_Click");
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void btnSaveAllRule_Click(object sender, EventArgs e)
        {
            try
            {
                RemoveZoneRuleDuplicateRows();

                if (dgvRules.DataSource == null)
                {
                    return;
                }

                Cursor.Current = Cursors.WaitCursor;

                //-----------------------------------------------------------
                DataSet dsSave = new DataSet();
                DataTable dtSave = ((DataView)dgvRules.DataSource).Table.Copy();
                dsSave.Tables.Add(dtSave);
                //-----------------------------------------------------------

                //-----------------------------------------------------------
                foreach (DataRow dr in dtSave.Rows)
                {
                    dr[CN.NotLike] = dr["Operator"].ToString() == "Not Like" ? true : false;        //CR1084
                }
                dtSave.Columns.Remove("Operator");
                dtSave.Columns.Remove("OrClauseNumberRep");
                //-----------------------------------------------------------

                CollectionsManager.SaveZoneRule("", dsSave, out error);

                if (error.Length > 0)
                {
                    ShowError(error);
                }
                else
                {
                    //TODO
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnSaveAllRule_Click");
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Title = "Zone Rules Export";
                saveDialog.CheckPathExists = true;
                saveDialog.FileName = "Exported_Zone_Rules";
                saveDialog.Filter = "XML Files|*.xml";
                saveDialog.OverwritePrompt = true;
                saveDialog.ValidateNames = true;
                //saveDialog.InitialDirectory = @"D:\Users\Default"; //IP - 05/01/2010 - UAT(953) - Commented out as not required.

                if (saveDialog.ShowDialog(this) == DialogResult.OK && saveDialog.FileName != "")
                {
                    Cursor.Current = Cursors.WaitCursor;
                    DataTable dtExport = ((DataView)dgvRules.DataSource).Table.Copy();
                    dtExport.WriteXml(saveDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnExport_Click");
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openDialog = new OpenFileDialog();
                openDialog.Title = "Zone Rules Import";
                openDialog.CheckPathExists = true;
                openDialog.CheckFileExists = true;
                openDialog.Filter = "XML Files|*.xml";
                openDialog.ValidateNames = true;
                openDialog.Multiselect = false;
                //openDialog.InitialDirectory = @"D:\Users\Default"; //IP - 05/01/2010 - UAT(953) - Commented out as not required.

                if (openDialog.ShowDialog(this) == DialogResult.OK && openDialog.FileName != "")
                {
                    Cursor.Current = Cursors.WaitCursor;
                    DataTable dtSource = ((DataView)dgvRules.DataSource).Table;
                    dtSource.AcceptChanges();
                    dtSource.Clear();
                    dtSource.ReadXml(openDialog.FileName);
                    dgvRules.Refresh();
                    CalcGridSize();//IP - 05/01/2010 - UAT(953)
                }
            }
            catch (Exception ex)
            {
                ((DataView)dgvRules.DataSource).Table.RejectChanges();
                Catch(ex, "btnImport_Click");
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void CalcGridSize()
        {
            if (dgvRules.RowCount < 10)
            {
                dgvRules.Height = (dgvRules.RowCount * 22) + 24;
                dgvRules.Width = 428;
            }
            else
            {
                dgvRules.Height = 222;
                dgvRules.Width = 445;
            }
        }

        //--------------------------------------------------------------------------------
        #endregion -----------------------------------------------------------------------


        #region --Unzonned Address Tab----------------------------------------------------
        //--------------------------------------------------------------------------------

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                DataSet dsUnzonnedAddress = CollectionsManager.LoadUnallocatedAddressZones(out error);

                if (error.Length > 0)
                {
                    ShowError(error);
                    return;
                }

                if (dsUnzonnedAddress.Tables.Count > 0)
                {
                    dgvUnzonedAddress.DataSource = dsUnzonnedAddress.Tables[0];
                    dgvUnzonedAddress.Columns[0].Width = 90;
                }
                else
                {
                    dgvUnzonedAddress.DataSource = null;
                }


            }
            catch (Exception ex)
            {
                Catch(ex, "btnLoad_Click");
            }
        }

        private void dgvUnzonedAddress_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right && dgvUnzonedAddress.RowCount > 0 && dgvUnzonedAddress.CurrentRow.Index >= 0)
                {
                    dgvUnzonedAddress.Rows[dgvUnzonedAddress.CurrentRow.Index].Selected = true;

                    MenuCommand menuCustDetail = new MenuCommand(GetResource("P_CUSTDETAILS"));
                    menuCustDetail.Click += new System.EventHandler(menuCustDetail_Click);

                    PopupMenu popup = new PopupMenu();
                    popup.Animate = Animate.Yes;
                    popup.AnimateStyle = Animation.SlideHorVerPositive;
                    popup.MenuCommands.Add(menuCustDetail);
                    popup.TrackPopup(dgvUnzonedAddress.PointToScreen(new Point(e.X, e.Y)));
                }
            }
            catch
            {
            }
        }

        private void menuCustDetail_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (dgvUnzonedAddress.RowCount > 0 && dgvUnzonedAddress.CurrentRow.Index >= 0)
                {
                    string custId = dgvUnzonedAddress[txtColumn_CustId.Name, dgvUnzonedAddress.CurrentRow.Index].Value.ToString();

                    BasicCustomerDetails frmCustomer = new BasicCustomerDetails(false, custId, FormRoot, FormParent);
                    ((MainForm)FormRoot).AddTabPage(frmCustomer, 10);
                    frmCustomer.loaded = true;
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "menuCustDetail_Click");
            }
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvUnzonedAddress.DataSource == null || dgvUnzonedAddress.Rows.Count <= 0)
                {
                    return;
                }

                if (IsExcelInstalled() == false) // UAT(5.2) - 670
                {
                    ShowWarning("Excel application not found");
                    return;
                }

                Excel.ApplicationClass excelApp = new Excel.ApplicationClass();
                excelApp.Application.Workbooks.Add(true);
                DataTable dtSource = (DataTable)dgvUnzonedAddress.DataSource;

                //--Writing the headers ---------------------------------------------
                int columnIndex = 0;
                foreach (DataGridViewColumn dgvc in dgvUnzonedAddress.Columns)
                {
                    if (dgvc.Visible == false)
                        continue;

                    columnIndex++;
                    excelApp.Cells[1, columnIndex] = dgvc.HeaderText;
                    ((Excel.Range)excelApp.Cells[1, columnIndex]).Interior.Color = Color.BlanchedAlmond.ToArgb();
                    ((Excel.Range)excelApp.Cells[1, columnIndex]).EntireColumn.NumberFormat = "@";
                }

                //-------------------------------------------------------------------

                //--Writing the data ------------------------------------------------
                int rowIndex = 1;
                foreach (DataGridViewRow dgvr in dgvUnzonedAddress.Rows)
                {
                    rowIndex++;
                    columnIndex = 0;
                    foreach (DataGridViewColumn dgvc in dgvUnzonedAddress.Columns)
                    {
                        if (dgvc.Visible == false)
                            continue;

                        columnIndex++;
                        excelApp.Cells[rowIndex, columnIndex] = dgvr.Cells[dgvc.Name].Value.ToString();
                    }
                }
                //-------------------------------------------------------------------

                excelApp.Visible = true;
                Excel.Worksheet worksheet = (Excel.Worksheet)excelApp.ActiveSheet;
                worksheet.Columns.AutoFit();
                worksheet.Activate();
            }
            catch (Exception ex)
            {
                Catch(ex, "btnExportExcel_Click");
            }
        }

        //--------------------------------------------------------------------------------
        #endregion -----------------------------------------------------------------------


        #region --Employee Allocation Tab-------------------------------------------------
        //--------------------------------------------------------------------------------

        private static class NodeTypes
        {
            public static NodeType AllEmployeeGroup = new NodeType("AllEmployeeGroup");
            public static NodeType EmployeeGroup    = new NodeType("EmployeeGroup");
            public static NodeType AllBranch        = new NodeType("AllBranch");
            public static NodeType Branch           = new NodeType("Branch");
        }

        private bool PopulateInitialData_EmployeeAlloc()
        {
            DataSet dsEmployeeInfo = CollectionsManager.GetZoneAllocatableEmployeeInfo(out error);
            if (error.Length > 0)
            {
                ShowError(error);
                return false;
            }
            else if (dsEmployeeInfo.Tables.Count < 3)
            {
                return false;
            }

            //--Global DataTable dtGlobalEmpZoneAlloc------------------------
            DataSet dsEmpZoneAllocation = CollectionsManager.BailiffAllocationRulesLoad(out error);
            if (error.Length > 0)
            {
                ShowError(error);
                return false;
            }
            else if (dsEmpZoneAllocation.Tables.Count > 0)
            {
                dtGlobalEmpZoneAlloc = dsEmpZoneAllocation.Tables[0];
            }
            //---------------------------------------------------------------

            //--Filter TreeView----------------------------------------------
            CMTreeNode allEmpTypeNode = tvFilter.Nodes.AddCMNode("ALL_EMP_TYPE", "Employee Types", null, NodeTypes.AllEmployeeGroup);
            foreach (DataRow dr in dsEmployeeInfo.Tables[TN.EmployeeTypes].Rows)
            {
                allEmpTypeNode.Nodes.AddCMNode(dr[CN.Code].ToString(), dr["concatDesc"].ToString(), null, NodeTypes.EmployeeGroup);
            }

            CMTreeNode allBranchNode = tvFilter.Nodes.AddCMNode("ALL_BRANCH", "Branches", null, NodeTypes.AllBranch);
            foreach (DataRow dr in dsEmployeeInfo.Tables[TN.BranchDetails].Rows)
            {
                allBranchNode.Nodes.AddCMNode(dr[CN.BranchNo].ToString(), dr["concatDesc"].ToString(), null, NodeTypes.Branch);
            }
            //---------------------------------------------------------------

            //--Employee DataGrid--------------------------------------------
            for (int i = MIN_ACCT_RANGE_START; i <= MIN_ACCT_RANGE_END; i++)
                cmbColumn_MinAccounts.Items.Add(i);

            for (int i = MAX_ACCT_RANGE_START; i <= MAX_ACCT_RANGE_END; i++)
                cmbColumn_MaxAccounts.Items.Add(i);

            for (Int16 i = ALLOC_RANK_RANGE_START; i <= ALLOC_RANK_RANGE_END; i++)
                cmbColumn_AllocationRank.Items.Add(i);

            DataTable dtEmplyee = dsEmployeeInfo.Tables[TN.Employee];
            DataView dvEmployee = dtEmplyee.DefaultView;
            //dvEmployee.RowFilter = CN.EmployeeType + " = '' and " + CN.BranchNo + " = '' "; //This will return 0 records
            dgvEmployee.DataSource = dvEmployee;

            foreach (DataGridViewColumn column in dgvEmployee.Columns)
            {
                if (column.Name == txtColumn_EmployeeDetail.Name || column.Name == cmbColumn_MinAccounts.Name ||
                    column.Name == cmbColumn_MaxAccounts.Name || column.Name == cmbColumn_AllocationRank.Name || column.Name == txtColumn_Allocated.Name)
                {
                    column.Visible = true;
                }
                else
                {
                    column.Visible = false;
                }
            }

            dgvEmployee.Refresh();
            CalcEmpeeGridSize();
            //---------------------------------------------------------------

            //--Zone Allocation Grid-----------------------------------------
            DataTable dtGrid = new DataTable();
            dtGrid.Columns.Add(CN.EmployeeNo);
            dtGrid.Columns.Add(CN.EmployeeType);
            dtGrid.Columns.Add("BranchOrZone");
            dtGrid.Columns.Add("BranchOrZoneNo");
            dtGrid.Columns.Add("BranchOrZoneType");
            dtGrid.Columns.Add("Bailiffs");
            dtGrid.Columns.Add("Accounts");
            dtGrid.Columns.Add("IsAllocated", typeof(Boolean));
            dtGrid.Columns.Add("AllocOrder");
            dtGrid.Columns.Add("Reallocate", typeof(Boolean));

            //--Appending Zones--
            if (cmbZones.DataSource != null)
            {
                foreach (DataRow dr in ((DataTable)cmbZones.DataSource).Rows)
                {
                    DataRow drNew = dtGrid.NewRow();
                    dtGrid.Rows.Add(drNew);
                    drNew[CN.EmployeeNo] = "";
                    drNew[CN.EmployeeType] = "";
                    drNew["BranchOrZoneNo"] = dr[CN.Zone].ToString();
                    drNew["BranchOrZone"] = dr["concatDesc"].ToString();
                    drNew["BranchOrZoneType"] = "Zone";
                    drNew["IsAllocated"] = false;
                    drNew["AllocOrder"] = "0";
                    drNew["Reallocate"] = false;
                }
            }

            //--Appending Branches--
            foreach (DataRow dr in dsEmployeeInfo.Tables[TN.BranchDetails].Rows)
            {
                DataRow drNew = dtGrid.NewRow();
                dtGrid.Rows.Add(drNew);
                drNew[CN.EmployeeNo] = "";
                drNew[CN.EmployeeType] = "";
                drNew["BranchOrZoneNo"] = dr[CN.BranchNo].ToString();
                drNew["BranchOrZone"] = dr["concatDesc"].ToString();
                drNew["BranchOrZoneType"] = "Branch";
                drNew["IsAllocated"] = false;
                drNew["AllocOrder"] = "0";
                drNew["Reallocate"] = false;
            }

            //--Updating Bailiffs & Accounts per zone/Branch--
            foreach (DataRow dr1 in dtGrid.Rows)
            {
                foreach (DataRow dr2 in dsEmpZoneAllocation.Tables[1].Rows)
                {
                    if (dr1["BranchorZoneNo"].ToString() == dr2["BranchorZone"].ToString())
                    {
                        dr1["Bailiffs"] = dr2["Bailiffs"].ToString();
                        dr1["Accounts"] = dr2["NumAccs"].ToString();
                    }
                }
            }

            dgvEmpZoneAllocation.AutoGenerateColumns = false;
            dgvEmpZoneAllocation.Columns["chkColumn_IsAllocated"].HeaderCell.Style.BackColor = Color.Silver;
            dgvEmpZoneAllocation.DataSource = dtGrid;
            //---------------------------------------------------------------

            if (dgvEmployee.Rows.Count <= 0)
                dgvEmpZoneAllocation.Enabled = false;

            CalcEmpZoneAllocGridSize();

            return true;
        }

        private void CalcEmpeeGridSize()
        {
            if (dgvEmployee.RowCount < 10)
            {
                // Calc grid Height as (Rows * 22) + Header height
                dgvEmployee.Height = (dgvEmployee.RowCount * 22) + 24;
                dgvEmployee.Width = 450;
            }
            else
            {
                // Calc grid Width to allow for scroll bar
                dgvEmployee.Height = 203;
                dgvEmployee.Width = 468;
            }
        }

        private void CalcEmpZoneAllocGridSize()
        {
            if (dgvEmpZoneAllocation.RowCount < 10)
            {
                // Calc grid Height as (Rows * 22) + Header height
                dgvEmpZoneAllocation.Height = (dgvEmpZoneAllocation.RowCount * 22) + 24;
                dgvEmpZoneAllocation.Width = 468;
            }
            else
            {
                // Calc grid Width to allow for scroll bar
                dgvEmpZoneAllocation.Height = 183;
                dgvEmpZoneAllocation.Width = 482;
            }
        }

        private void ReloadEmployleeGrid()
        {
            string strFilterEmpType = "";
            string strFilterBranch = "";

            foreach (CMTreeNode node in tvFilter.Nodes)
            {
                if (node.NodeType == NodeTypes.AllEmployeeGroup && node.Checked)
                {
                    strFilterEmpType = "";
                    continue;
                }
                else if (node.NodeType == NodeTypes.AllBranch && node.Checked)
                {
                    strFilterBranch = "";
                    continue;
                }

                foreach (CMTreeNode childNode in node.Nodes)
                {
                    if (childNode.NodeType == NodeTypes.EmployeeGroup && childNode.Checked)
                    {
                        strFilterEmpType = strFilterEmpType + CN.EmployeeType + " = '" + childNode.Name + "' or ";
                    }
                    else if (childNode.NodeType == NodeTypes.Branch && childNode.Checked)
                    {
                        strFilterBranch = strFilterBranch + CN.BranchNo + " = '" + childNode.Name + "' or ";
                    }
                }
            }

            if (strFilterEmpType != "")
            {
                int tempIndex = strFilterEmpType.LastIndexOf(" or ");
                strFilterEmpType = strFilterEmpType.Remove(tempIndex < 0 ? 0 : tempIndex);
                strFilterEmpType = "(" + strFilterEmpType + ")";
                strFilterEmpType += " and ";
            }

            if (strFilterBranch != "")
            {
                int tempIndex = strFilterBranch.LastIndexOf(" or ");
                strFilterBranch = strFilterBranch.Remove(tempIndex < 0 ? 0 : tempIndex);
                strFilterBranch = "(" + strFilterBranch + ")";
                strFilterBranch += " and ";
            }

            string strFilter = strFilterEmpType + strFilterBranch;
            if (strFilter != "")
            {
                int tempIndex = strFilter.LastIndexOf(" and ");
                strFilter = strFilter.Remove(tempIndex < 0 ? 0 : tempIndex);
            }

            ((DataView)dgvEmployee.DataSource).RowFilter = strFilter;
            dgvEmployee.Refresh();
            CalcEmpeeGridSize();
        }

        private void SyncEmpZoneAllocationGrid(string newEmpeeNo, string newEmpeeType)
        {
            if (dgvEmpZoneAllocation.DataSource == null || dtGlobalEmpZoneAlloc.IsInitialized == false)
            {
                return;
            }

            DataTable dtGrid = (DataTable)dgvEmpZoneAllocation.DataSource;

            #region -- Sync the Global DataTable dtGlobalEmpZoneAlloc with the Grid--
            //-----------------------------------------------------------------------
            string previousEmpeeNo = dtGrid.Rows.Count > 0 ? dtGrid.Rows[0][CN.EmployeeNo].ToString().Trim() : "";
            string previousEmpeeType = dtGrid.Rows.Count > 0 ? dtGrid.Rows[0][CN.EmployeeType].ToString().Trim() : "";

            if (previousEmpeeNo != "")
            {
                //--Remove existing records-------------------------------------
                DataRow[] drArraySelection = dtGlobalEmpZoneAlloc.Select(CN.EmployeeNo + " = '" + previousEmpeeNo + "'");
                foreach (DataRow drSelection in drArraySelection)
                {
                    dtGlobalEmpZoneAlloc.Rows.Remove(drSelection);
                }
                //--------------------------------------------------------------

                //--Adding updated records from the grid------------------------
                DataRow[] drArrayGrid = dtGrid.Select("IsAllocated = true", "BranchOrZoneType, BranchorZone");
                foreach (DataRow drGrid in drArrayGrid)
                {
                    DataRow drNew = dtGlobalEmpZoneAlloc.NewRow();
                    dtGlobalEmpZoneAlloc.Rows.Add(drNew);

                    drNew[CN.EmployeeNo] = previousEmpeeNo;
                    drNew[CN.EmployeeType] = previousEmpeeType;
                    drNew["BranchorZone"] = drGrid["BranchOrZoneNo"].ToString();
                    //drNew["Bailiffs"] = drGrid["Bailiffs"];
                    //drNew["Numaccs"] = drGrid["Accounts"];
                    drNew["IsZone"] = drGrid["BranchOrZoneType"].ToString() == "Zone" ? true : false;
                    drNew["AllocationOrder"] = drGrid["AllocOrder"].ToString();
                    drNew["reallocate"] = Convert.ToBoolean(drGrid["Reallocate"].ToString());
                }
                //--------------------------------------------------------------
            }
            //-----------------------------------------------------------------------
            #endregion --------------------------------------------------------------


            #region -- Reload the Grid with new Employee Selected--------------------
            //-----------------------------------------------------------------------
            if (newEmpeeNo != previousEmpeeNo)
            {
                //Resetting DataGrid Records------------------------------------
                foreach (DataRow drGrid in dtGrid.Rows)
                {
                    drGrid[CN.EmployeeNo] = newEmpeeNo;
                    drGrid[CN.EmployeeType] = newEmpeeType;
                    drGrid["IsAllocated"] = false;
                    drGrid["AllocOrder"] = "0";
                    drGrid["Reallocate"] = false;
                }
                //--------------------------------------------------------------

                //Updating DataGrid with New Emp Records------------------------
                if (newEmpeeNo != "")
                {
                    DataRow[] drArraySelection = dtGlobalEmpZoneAlloc.Select(CN.EmployeeNo + " = '" + newEmpeeNo + "'");
                    foreach (DataRow drSelection in drArraySelection)
                    {
                        string tempBranchOrZone = Convert.ToBoolean(drSelection["IsZone"].ToString()) ? "Zone" : "Branch";
                        DataRow[] drArrayGrid = dtGrid.Select("BranchOrZoneNo = '" + drSelection["BranchorZone"].ToString() + "' and " +
                                                              "BranchOrZoneType = '" + tempBranchOrZone + "'");
                        if (drArrayGrid.Length > 0)
                        {
                            drArrayGrid[0]["IsAllocated"] = true;
                            drArrayGrid[0]["AllocOrder"] = drSelection["AllocationOrder"].ToString();
                            drArrayGrid[0]["Reallocate"] = Convert.ToBoolean(drSelection["reallocate"].ToString());
                        }
                    }

                    dgvEmpZoneAllocationHeader.Text = "Zone/Branch Allocation for Employee - " + newEmpeeNo;
                }
                else
                {
                    dgvEmpZoneAllocationHeader.Text = "Zone/Branch Allocation";
                }
                //--------------------------------------------------------------
            }
            //-----------------------------------------------------------------------
            #endregion --------------------------------------------------------------

            dgvEmpZoneAllocation.Refresh();
            if (dgvEmpZoneAllocation.CurrentRow != null)
                dgvEmpZoneAllocation.CurrentRow.Selected = false;
        }

        private void dgvZoneAllocation_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                if (dgvEmpZoneAllocation.Rows[e.RowIndex].Cells["chkColumn_IsAllocated"].Value.ToString() == "True")
                {
                    foreach (DataGridViewCell dgvc in dgvEmpZoneAllocation.Rows[e.RowIndex].Cells)
                    {
                        if (dgvc.OwningColumn.Name == "chkColumn_Reallocate")
                        {
                            dgvc.ReadOnly = false;
                            dgvc.Style.SelectionBackColor = Color.Beige;
                        }
                        if (dgvc.OwningColumn.Name != "chkColumn_IsAllocated")
                        {
                            dgvc.Style.BackColor = Color.Beige;
                        }
                    }
                }
                else
                {
                    foreach (DataGridViewCell dgvc in dgvEmpZoneAllocation.Rows[e.RowIndex].Cells)
                    {
                        if (dgvc.OwningColumn.Name != "chkColumn_IsAllocated")
                        {
                            dgvc.Style.BackColor = Color.White;
                            dgvc.ReadOnly = true;
                        }
                    }
                }

                e.CellStyle.SelectionBackColor = e.CellStyle.BackColor;
                e.CellStyle.SelectionForeColor = e.CellStyle.ForeColor;

                //-- Tooltip ------------------------------------------
                if (e.ColumnIndex == dgvEmpZoneAllocation.Columns["chkColumn_Reallocate"].Index)
                {
                    dgvEmpZoneAllocation.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = reallocateTooltipText;
                }
            }
            catch (Exception ex)
            {
                string abc = ex.ToString();
            }
        }

        private void dgvEmpZoneAllocation_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            try
            {
                if (dgvEmpZoneAllocation.CurrentCell is DataGridViewCheckBoxCell && dgvEmpZoneAllocation.CurrentCell.OwningColumn.Name == "chkColumn_IsAllocated")
                {
                    dgvEmpZoneAllocation.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            }
            catch
            {
            }
        }

        private void dgvEmpZoneAllocation_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dgvEmpZoneAllocation.Columns[e.ColumnIndex].Name == "chkColumn_IsAllocated")
                {
                    if (dgvEmpZoneAllocation.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "True")
                    {
                        int maxValue = Convert.ToInt32(((DataTable)dgvEmpZoneAllocation.DataSource).Compute("MAX(AllocOrder)", ""));
                        dgvEmpZoneAllocation.Rows[e.RowIndex].Cells["txtColumn_AllocOrder"].Value = (maxValue + 1).ToString();
                    }
                    else
                    {
                        int uncheckedValue = Convert.ToInt32(dgvEmpZoneAllocation.Rows[e.RowIndex].Cells["txtColumn_AllocOrder"].Value);
                        dgvEmpZoneAllocation.Rows[e.RowIndex].Cells["txtColumn_AllocOrder"].Value = "0";

                        //-- Updating rest of the records accordingly---------------------
                        foreach (DataGridViewRow dgvr in dgvEmpZoneAllocation.Rows)
                        {
                            if (Convert.ToInt32(dgvr.Cells["txtColumn_AllocOrder"].Value.ToString()) > uncheckedValue)
                            {
                                int temp = Convert.ToInt32(dgvr.Cells["txtColumn_AllocOrder"].Value.ToString());
                                dgvr.Cells["txtColumn_AllocOrder"].Value = (temp - 1).ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string abc = ex.ToString();
            }
        }

        private void btnSaveEmpAlloc_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvEmployee.DataSource == null || dgvEmployee.Rows.Count <= 0 || dgvEmployee.CurrentRow == null)
                {
                    return;
                }

                Cursor.Current = Cursors.WaitCursor;

                //-----------------------------------------------------------
                string empeeNo = dgvEmployee.Rows[dgvEmployee.CurrentRow.Index].Cells[CN.EmployeeNo].Value.ToString();
                string empeeType = dgvEmployee.Rows[dgvEmployee.CurrentRow.Index].Cells[CN.EmployeeType].Value.ToString();
                SyncEmpZoneAllocationGrid(empeeNo, empeeType);
                //-----------------------------------------------------------

                //-----------------------------------------------------------
                DataSet dsSave = new DataSet();
                DataTable dtSaveZoneAlloc = dtGlobalEmpZoneAlloc.Copy();
                dtSaveZoneAlloc.Clear();
                dsSave.Tables.Add(dtSaveZoneAlloc);

                DataTable dtSaveEmpUpdate = new DataTable();
                dtSaveEmpUpdate.Columns.Add(CN.EmployeeNo);
                dtSaveEmpUpdate.Columns.Add("MinAccounts");
                dtSaveEmpUpdate.Columns.Add("MaxAccounts");
                dtSaveEmpUpdate.Columns.Add("AllocationRank");
                dsSave.Tables.Add(dtSaveEmpUpdate);
                //-----------------------------------------------------------

                DataRow[] drArrayZoneAlloc = dtGlobalEmpZoneAlloc.Select(CN.EmployeeNo + " = '" + empeeNo + "'");
                foreach (DataRow dr in drArrayZoneAlloc)
                {
                    dtSaveZoneAlloc.ImportRow(dr);
                }

                DataRow[] drArrayEmpUpdate = ((DataView)dgvEmployee.DataSource).Table.Select(CN.EmployeeNo + " = '" + empeeNo + "'");
                foreach (DataRow dr in drArrayEmpUpdate)
                {
                    DataRow drNew = dtSaveEmpUpdate.NewRow();
                    dtSaveEmpUpdate.Rows.Add(drNew);
                    drNew[CN.EmployeeNo] = dr[CN.EmployeeNo].ToString();
                    drNew["MinAccounts"] = dr["MinAccounts"].ToString();
                    drNew["MaxAccounts"] = dr["MaxAccounts"].ToString();
                    drNew["AllocationRank"] = dr["AllocationRank"].ToString();
                }

                CollectionsManager.SaveBailiffZoneAllocation(Convert.ToInt32(empeeNo), dsSave, out error);

                if (error.Length > 0)
                    ShowError(error);
                else
                {
                    //TODO
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnSaveEmpAlloc_Click");
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void btnSaveAllEmpAlloc_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                //-----------------------------------------------------------
                string empeeNo = "";
                string empeeType = "";
                if (dgvEmployee.DataSource != null && dgvEmployee.Rows.Count > 0) // && dgvEmployee.CurrentRow.Index != null)
                {
                    empeeNo = dgvEmployee.Rows[dgvEmployee.CurrentRow.Index].Cells[CN.EmployeeNo].Value.ToString();
                    empeeType = dgvEmployee.Rows[dgvEmployee.CurrentRow.Index].Cells[CN.EmployeeType].Value.ToString();
                }
                SyncEmpZoneAllocationGrid(empeeNo, empeeType);
                //-----------------------------------------------------------

                //-----------------------------------------------------------
                DataSet dsSave = new DataSet();
                DataTable dtSaveZoneAlloc = dtGlobalEmpZoneAlloc.Copy();
                dsSave.Tables.Add(dtSaveZoneAlloc);

                DataTable dtSaveEmpUpdate = new DataTable();
                dtSaveEmpUpdate.Columns.Add(CN.EmployeeNo);
                dtSaveEmpUpdate.Columns.Add("MinAccounts");
                dtSaveEmpUpdate.Columns.Add("MaxAccounts");
                dtSaveEmpUpdate.Columns.Add("AllocationRank");
                dsSave.Tables.Add(dtSaveEmpUpdate);
                //-----------------------------------------------------------

                foreach (DataRow dr in ((DataView)dgvEmployee.DataSource).Table.Rows)
                {
                    DataRow drNew = dtSaveEmpUpdate.NewRow();
                    dtSaveEmpUpdate.Rows.Add(drNew);
                    drNew[CN.EmployeeNo] = dr[CN.EmployeeNo].ToString();
                    drNew["MinAccounts"] = dr["MinAccounts"].ToString();
                    drNew["MaxAccounts"] = dr["MaxAccounts"].ToString();
                    drNew["AllocationRank"] = dr["AllocationRank"].ToString();
                }

                CollectionsManager.SaveBailiffZoneAllocation(0, dsSave, out error); // Save All

                if (error.Length > 0)
                {
                    ShowError(error);
                }
                else
                {
                    //TODO
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnSaveAllEmpAlloc_Click");
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void dgvEmployee_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (dgvEmployee.DataSource == null || dgvEmployee.Rows.Count <= 0 || dgvEmployee.CurrentRow == null)
                {
                    SyncEmpZoneAllocationGrid("", "");
                    dgvEmpZoneAllocation.Enabled = false;
                    return;
                }

                string empeeNo = dgvEmployee.Rows[dgvEmployee.CurrentRow.Index].Cells[CN.EmployeeNo].Value.ToString();
                string empeeType = dgvEmployee.Rows[dgvEmployee.CurrentRow.Index].Cells[CN.EmployeeType].Value.ToString();
                dgvEmpZoneAllocation.Enabled = true;
                SyncEmpZoneAllocationGrid(empeeNo, empeeType);
            }
            catch (Exception ex)
            {
                Catch(ex, "lstEmployee_SelectedIndexChanged");
            }
        }

        private void dgvEmpZoneAllocation_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            try
            {
                //This method is to handle any data error in the grid.
                ShowError("dgvEmpZoneAllocation_DataError. Detail : Error in 'dgvEmpZoneAllocation' " + "ColumnIndex : " + e.ColumnIndex + " Context : " + e.Context + " " +
                            e.Exception.ToString());
            }
            catch
            {
            }
        }

        private void dgvEmployee_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            try
            {
                //This method is to handle any data error in the grid.
                ShowError("dgvEmployee_DataError. Detail : Error in 'dgvEmployee' " + "ColumnIndex : " + e.ColumnIndex + " Context : " + e.Context + " " +
                             e.Exception.ToString());
            }
            catch
            {
            }
        }

        private void dgvEmployee_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            try
            {
                if (e.Control is DataGridViewComboBoxEditingControl)
                {
                    (e.Control as DataGridViewComboBoxEditingControl).DropDownStyle = ComboBoxStyle.DropDownList;
                    (e.Control as DataGridViewComboBoxEditingControl).AutoCompleteSource = AutoCompleteSource.ListItems;
                    (e.Control as DataGridViewComboBoxEditingControl).IntegralHeight = false;
                    (e.Control as DataGridViewComboBoxEditingControl).MaxDropDownItems = 5;
                }
            }
            catch
            {
            }
        }

        private void tvFilter_AfterCheck(object sender, TreeViewEventArgs e)
        {
            try
            {
                this.tvFilter.AfterCheck -= new System.Windows.Forms.TreeViewEventHandler(this.tvFilter_AfterCheck);

                CMTreeNode currentNode = (CMTreeNode)e.Node;

                if (currentNode.NodeType == NodeTypes.AllEmployeeGroup || currentNode.NodeType == NodeTypes.AllBranch)
                {
                    foreach (CMTreeNode childNode in currentNode.Nodes)
                    {
                        childNode.Checked = currentNode.Checked;
                    }
                }
                else if (currentNode.NodeType == NodeTypes.EmployeeGroup || currentNode.NodeType == NodeTypes.Branch)
                {
                    if (currentNode.Parent != null)
                    {
                        int checkedChildCount = 0;
                        var parentNode = (CMTreeNode)currentNode.Parent;
                        
                        foreach (CMTreeNode childNode in parentNode.Nodes)
                        {
                            checkedChildCount += childNode.Checked ? 1 : 0;
                        }

                        parentNode.Checked = (checkedChildCount == parentNode.Nodes.Count);
                    }
                }

                ReloadEmployleeGrid();
            }
            catch (Exception ex)
            {
                Catch(ex, "tvFilter_AfterCheck");
            }
            finally
            {
                tvFilter.AfterCheck -= new System.Windows.Forms.TreeViewEventHandler(tvFilter_AfterCheck);
                tvFilter.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(tvFilter_AfterCheck);
            }
        }

        //IP - 12/06/09 - Credit Collection Walkthrough Changes - right-click delete menu option.
        private void dgvRules_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                DataGridView ctl = (DataGridView)sender;

                MenuCommand deleteCriteria = new MenuCommand(GetResource("P_DELETE"));
                deleteCriteria.Click += new System.EventHandler(this.menuDeleteZoneCriteria_Click);

                deleteCriteria.Enabled = true;
                deleteCriteria.Visible = true;

                PopupMenu popup = new PopupMenu();
                popup.Animate = Animate.Yes;
                popup.AnimateStyle = Animation.SlideHorVerPositive;

                popup.MenuCommands.Add(deleteCriteria);

                MenuCommand selected = popup.TrackPopup(ctl.PointToScreen(new Point(e.X, e.Y)));
            }

        }

        //IP - 12/06/09 - Credit Collection Walkthrough Changes - moved from btnRemoveRows_Click
        private void menuDeleteZoneCriteria_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvRules.DataSource == null)
                {
                    return;
                }

                DataTable dtSource = ((DataView)dgvRules.DataSource).Table;
                int[] removedOrClauseArray = new int[dgvRules.SelectedRows.Count];
                int index = 0;

                //-- Removing selected rows from the DataTable ----------------------------
                foreach (DataGridViewRow selectedRow in dgvRules.SelectedRows)
                {
                    DataRow dr = ((DataView)dgvRules.DataSource)[selectedRow.Index].Row;
                    removedOrClauseArray[index++] = Convert.ToInt32(dr["OrClauseNumberRep"]);
                    dtSource.Rows.Remove(dr);
                }
                //-------------------------------------------------------------------------


                //-------------------------------------------------------------------------
                //-- Clear previous 'OR' conditions applied jointly with the rows removed 
                for (int i = 0; i < removedOrClauseArray.Length; i++)
                {
                    if (removedOrClauseArray[i] != 0)
                    {
                        DataRow[] drArraySelected = dtSource.Select("OrClauseNumberRep = " + removedOrClauseArray[i].ToString());

                        if (drArraySelected.Length <= 1 && reusableOrClauseNumbers.Contains(removedOrClauseArray[i]) == false)
                        {
                            reusableOrClauseNumbers.Add(removedOrClauseArray[i]);
                        }

                        if (drArraySelected.Length == 1) //If there's a single row left without any joining rows
                        {
                            drArraySelected[0][CN.Or_Clause] = "";
                            drArraySelected[0]["OrClauseNumberRep"] = 0;
                        }
                    }
                }
                //-------------------------------------------------------------------------

                dgvRules.Refresh();
                CalcGridSize();
            }
            catch (Exception ex)
            {
                Catch(ex, "btnRemoveRows_Click");
            }
        }

        //--------------------------------------------------------------------------------
        #endregion -----------------------------------------------------------------------
    }
}


