using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;
using System.Collections;
using STL.Common.Static;

namespace STL.PL
{
    public partial class WarrantyReturnCodes : CommonForm
    {
        private DataTable returnCodes;
        private DataTable warrantyItems;
        private DataView dvreturnCodesView;
        //private string _errorTxt = "";
        private DateTime _todayNow = Date.blankDate;

        public WarrantyReturnCodes(Form root, Form parent)
        {
            InitializeComponent();

            HashMenus();
            ApplyRoleRestrictions();

            setScreen(btnSave.Enabled);
        }

        private void WarrantyReturnCodes_Load(object sender, EventArgs e)
        {
            returnCodes = AccountManager.GetWarrantyReturnCodes(out Error);
            warrantyItems = AccountManager.GetAllWarrantyItems(out Error);

            DataView dvreturnCodesView = new DataView(returnCodes);
            dgReturnCodes.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;           

            dgReturnCodes.DataSource = dvreturnCodesView;
            dgReturnCodes.Columns[CN.ProductType].Visible = false;
            dgReturnCodes.Columns[CN.Category].Width = 110;
            dgReturnCodes.Columns[CN.ReturnCode].Width = 90;
            dgReturnCodes.Columns[CN.WarrantyMonths].Width = 110;
            dgReturnCodes.Columns[CN.WarrantyMonths].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgReturnCodes.Columns[CN.ManuFacturerMonths].Width = 130;
            dgReturnCodes.Columns[CN.ManuFacturerMonths].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgReturnCodes.Columns[CN.ExpiredPortion].Width = 100;
            dgReturnCodes.Columns[CN.ExpiredPortion].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgReturnCodes.Columns[CN.RefundPercentage].Width = 125;
            dgReturnCodes.Columns[CN.RefundPercentage].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            //dgReturnCodes.Width = 725;        //testing 
            gbAddEdit.Enabled = true;
        }

        private void HashMenus()
        {
            dynamicMenus = new Hashtable();
            dynamicMenus[this.Name + ":btnSave"] = this.btnSave;
        }

        private void setScreen(bool enabled)
        {
            gbAddEdit.Visible = enabled;
            // expand datagrid if only viewing
            if (!enabled)
            {
                gbReturnCodes.Height = 420;
                dgReturnCodes.Height = 380;
                btnReload.Visible = false;
            }
            else
            {
                setCategories();
            }
        }

        private void dgReturnCodes_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                Function = "dgReturnCodesRowSelected_click";
                Wait();

                ClearEntryFields();
                ClearErrors();

                btnAdd.Enabled = true;
                btnDelete.Enabled = true;
                btnClear.Enabled = true;

                int index = dgReturnCodes.CurrentRow.Index;
                //DataView dvreturnCodesView = new DataView(returnCodes);               
                dvreturnCodesView = (DataView)dgReturnCodes.DataSource;

                // Edit row
                if (index >= 0 )
                {
                    drpCategory.Enabled = false;
                    udWarrantyLength.Enabled = false;
                    udExpiredMonths.Enabled = false;
                    udManufactLength.Enabled = false;       //jec 21/12/10

                    drpCategory.SelectedValue = (string)dvreturnCodesView[index][CN.ProductType].ToString();
                    txtReturnCode.Text = (string)dvreturnCodesView[index][CN.ReturnCode];
                    udWarrantyLength.Value = Convert.ToInt16(dvreturnCodesView[index][CN.WarrantyMonths]);
                    udManufactLength.Value = Convert.ToInt16(dvreturnCodesView[index][CN.ManuFacturerMonths]);
                    udExpiredMonths.Value = Convert.ToInt16(dvreturnCodesView[index][CN.ExpiredPortion]);
                    udRefundPct.Value = Convert.ToDecimal(dvreturnCodesView[index][CN.RefundPercentage]);

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

        private void ClearEntryFields()
        {            
            drpCategory.SelectedIndex = -1;
            txtReturnCode.Text = "";
            drpCategory.Enabled=true;
            txtReturnCode.Enabled=true;
            udWarrantyLength.Value = 0;
            udManufactLength.Value = 0;
            udExpiredMonths.Value = 0;
            udRefundPct.Value = 0;

            udWarrantyLength.Enabled = true;
            udExpiredMonths.Enabled = true;
            udManufactLength.Enabled = true;        //jec 21/12/10

            errorProvider1.SetError(this.drpCategory, "");
            errorProvider1.SetError(this.txtReturnCode, "");
            errorProvider2.SetError(this.txtReturnCode, "");
            errorProvider1.SetError(this.udWarrantyLength, "");            
            errorProvider1.SetError(this.udManufactLength, "");
            errorProvider1.SetError(this.udExpiredMonths, "");
            errorProvider1.SetError(this.udRefundPct, "");

            btnAdd.Enabled = false;
            btnDelete.Enabled = false;

        }

        private void ClearErrors()
        {
            errorProvider1.SetError(this.drpCategory, "");
            errorProvider1.SetError(this.txtReturnCode, "");
            errorProvider1.SetError(this.udWarrantyLength, "");
            errorProvider1.SetError(this.udManufactLength, "");
            errorProvider1.SetError(this.udExpiredMonths, "");
            errorProvider1.SetError(this.udRefundPct, "");           
            //((MainForm)this.FormRoot).statusBar1.Text = "";
        }

        private void setCategories()
        {
            DataTable categories = new DataTable();
            categories.Columns.Add(CN.Code);
            categories.Columns.Add(CN.Category);
            
            // Add a blank entry at the start of list            
            DataRow Row = categories.NewRow();
            Row[CN.Code] = "";
            Row[CN.Category] = "";
            categories.Rows.Add(Row);
            DataRow Row1 = categories.NewRow();
            Row1[CN.Code] = "E";
            Row1[CN.Category] = "Electrical";
            categories.Rows.Add(Row1);
            DataRow Row2 = categories.NewRow();
            Row2[CN.Code] = "F";
            Row2[CN.Category] = "Furniture";
            categories.Rows.Add(Row2);
            DataRow Row3 = categories.NewRow();
            Row3[CN.Code] = "I";
            Row3[CN.Category] = "Instant Replacement";
            categories.Rows.Add(Row3);            

            DataView dvcategoriesView = new DataView(categories);
            drpCategory.DataSource = dvcategoriesView;
            drpCategory.ValueMember = CN.Code;
            drpCategory.DisplayMember = CN.Category;
            drpCategory.Text = CN.Category;

            drpCategory.SelectedIndex = 0;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearEntryFields();

            dgReturnCodes.ClearSelection();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                Function = "btnEnter_click";
                ClearErrors();

                //validate entry
                if (ValidEntry())
                {                    
                    // Check whether to replace a row in edit for the same return code
                    DataView currentView = new DataView(returnCodes);
                    DataRowView rowViewFound = null;
                    if (dgReturnCodes.DataSource != null)  //no rows 
                    {
                        foreach (DataRowView rowView in currentView)
                        {                            
                            if ((string)rowView[CN.ProductType] == Convert.ToString(drpCategory.SelectedValue)
                                        && Convert.ToInt16(rowView[CN.WarrantyMonths]) == udWarrantyLength.Value
                                        && Convert.ToInt16(rowView[CN.ManuFacturerMonths]) == udManufactLength.Value    //jec 21/12/10
                                        && Convert.ToInt16(rowView[CN.ExpiredPortion]) == udExpiredMonths.Value)                                        
                            {
                                rowViewFound = rowView;
                            }
                        }
                    }
                    
                    if (rowViewFound != null)
                    {
                        currentView.AllowEdit = true;
                        // Update the matching edit row found
                        if (rowViewFound[CN.Category].ToString() == "Delete")
                        {
                            rowViewFound[CN.Category] = drpCategory.Text.ToString();    //reinstate delete row 
                        }
                        rowViewFound[CN.ReturnCode] = txtReturnCode.Text;
                        //rowViewFound[CN.ManuFacturerMonths] = udManufactLength.Value;     //jec 21/12/10                        
                        rowViewFound[CN.RefundPercentage] = udRefundPct.Value;
                        
                        currentView.AllowEdit = false;
                    }
                    else
                    {
                        // Add the new row as an edit row
                        currentView.AllowNew = true;
                        DataRowView rowView = currentView.AddNew();
                        rowView[CN.ProductType] = drpCategory.SelectedValue;
                        rowView[CN.Category] = drpCategory.Text.ToString();
                        rowView[CN.ReturnCode] = txtReturnCode.Text;
                        rowView[CN.WarrantyMonths] = udWarrantyLength.Value;
                        //rowView[CN.WarrantyMonths] = udWarrantyLength.Value;
                        rowView[CN.ManuFacturerMonths] = udManufactLength.Value;
                        rowView[CN.ExpiredPortion] = udExpiredMonths.Value;
                        rowView[CN.RefundPercentage] = udRefundPct.Value;
                        rowView.EndEdit();
                        currentView.AllowNew = false;
                    }

                    // Reset the input fields
                    ClearEntryFields();
                    // enable save button 
                    btnSave.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                Function = "btnDelete_click";
                ClearErrors();

                // Check whether to replace a row in edit for the same return code
                DataView currentView = new DataView(returnCodes);
                DataRowView rowViewFound = null;
                if (dgReturnCodes.DataSource != null)  //no rows 
                {
                    foreach (DataRowView rowView in currentView)
                    {
                        //if ((string)rowView[CN.ReturnCode] == txtReturnCode.Text)
                        if ((string)rowView[CN.ProductType] == Convert.ToString(drpCategory.SelectedValue)      //jec 21/12/10
                                        && Convert.ToInt16(rowView[CN.WarrantyMonths]) == udWarrantyLength.Value
                                        && Convert.ToInt16(rowView[CN.ManuFacturerMonths]) == udManufactLength.Value    
                                        && Convert.ToInt16(rowView[CN.ExpiredPortion]) == udExpiredMonths.Value)  
                        {
                            rowViewFound = rowView;
                        }
                    }
                }

                if (rowViewFound != null)
                {
                    currentView.AllowEdit = true;
                    rowViewFound[CN.Category] = "Delete";
                    
                    // rowView.EndEdit();
                    currentView.AllowEdit = false;
                    // Reset the input fields
                    ClearEntryFields();
                    // enable save button 
                    btnSave.Enabled = true;
                }

            }

            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private bool ValidEntry()
        {
            bool valid = true;
            ClearErrors();

            if (txtReturnCode.Text.Trim() == "")
            {
                errorProvider1.SetError(txtReturnCode, "Warranty Return Code must be entered");
                valid = false;
            }
            // Check item exists 
            DataView warrantyView = new DataView(warrantyItems);
            DataRowView rowViewFound = null;
            if (warrantyView != null)  //no rows 
            {
                foreach (DataRowView rowView in warrantyView)
                {
                    if ((string)rowView[CN.ItemNo] == Convert.ToString(txtReturnCode.Text))
                    {
                        rowViewFound = rowView;
                    }
                }
                if (rowViewFound == null)
                {
                    errorProvider1.SetError(txtReturnCode, "Return Code does not exist on StockItem table - Item must be interfaced before it can be maintained");
                    valid = false;
                }
            }
                       
            
            if (drpCategory.SelectedIndex <= 0)
            {
                errorProvider1.SetError(drpCategory, "Warranty Category must be selected");
                valid = false;
            }
            
            if (udExpiredMonths.Value > udWarrantyLength.Value)
            {
                errorProvider1.SetError(udExpiredMonths, "Expired Period cannot be greater than warranty length");
                valid = false;
            }
           

            if (udWarrantyLength.Text == string.Empty)
            {
                errorProvider1.SetError(udWarrantyLength, GetResource("M_ENTERMANDATORY"));
                valid = false;
            }
            
            if (udManufactLength.Text == string.Empty)
            {
                errorProvider1.SetError(udManufactLength, GetResource("M_ENTERMANDATORY"));
                valid = false;
            }
            
            if (udExpiredMonths.Text == string.Empty)
            {
                errorProvider1.SetError(udExpiredMonths, GetResource("M_ENTERMANDATORY"));
                valid = false;
            }
            
            if (udRefundPct.Text == string.Empty)
            {
                errorProvider1.SetError(udRefundPct, GetResource("M_ENTERMANDATORY"));
                valid = false;
            }
            
            return valid;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _todayNow = StaticDataManager.GetServerDateTime();
            AccountManager.SaveWarrantyReturnCodes(returnCodes, _todayNow);

            // disable save button 
            btnSave.Enabled = false;
            //((MainForm)this.FormRoot).statusBar1.Text = " Warranty Return Codes saved";
            ClearEntryFields();

            dgReturnCodes.DataSource = null;
            
            // disable clear to force reload after save
            btnClear.Enabled = false;
            btnAdd.Enabled = false;
            btnDelete.Enabled = false;
            gbAddEdit.Enabled = false;
        
        }

        private void drpCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnAdd.Enabled = true;
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            // re-load 
            WarrantyReturnCodes_Load(null, null);
        }

        private void dgReturnCodes_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {            
            ClearEntryFields();

            dgReturnCodes.ClearSelection();            
            dvreturnCodesView = (DataView)dgReturnCodes.DataSource;
            //Set vertical scroll to first row
            dgReturnCodes.FirstDisplayedScrollingRowIndex = 0;      
        }              
    }
}
