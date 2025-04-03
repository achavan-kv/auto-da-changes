using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using STL.Common;
using STL.Common.Static;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.StoreInfo;
using System.Collections;
using System.Xml;
using Blue.Cosacs.Shared.Extensions;
using STL.PL.Utils;

namespace STL.PL
{
    public partial class NonStock : CommonForm
    {
        private DataTable nonStock = new DataTable();
        private DataTable prices = new DataTable();
        private DataTable branches;
        private DataTable stockWarranty;
        private DataTable categories;
        private DataSet nonStockItems;
        //private DataView dvNonStockView;
        //private DataView dvPricesView;
        private new bool loaded = false;
        private int nonstockIndex = 0;
        private int priceIndex = 0;
        private bool clear = false;
      //  private bool valid = false;
        private bool edit = false;
        private bool stockItem = false;
        private DateTime _today = Date.blankDate;
        private bool endDateTbChange = false;         //tick box changed used in datechange
   //     bool loading = false;
        private BindingSource boundNonStockList;
        private BindingSource boundPrices;

        public NonStock(Form root, Form parent)
        {
            InitializeComponent();
            FormRoot = root;
            FormParent = parent;
            
            getCategories();
            HashMenus();
            ApplyRoleRestrictions();

            edit = btnSave.Enabled;     //Edit mode
            setScreen(false);
        }

        private void NonStock_Load(object sender, EventArgs e)
        {
            ClearScreen();
            
            loaded = false;
            
            _today = StaticDataManager.GetServerDate();
            //txtTaxRate.Text = Convert.ToString(Country[CountryParameterNames.TaxRate]);

            nonStockItems = AccountManager.GetNonStockByCode(txtItemNo.Text, out Error);
            
            nonStock      = nonStockItems.Tables["Table"];
            prices        = nonStockItems.Tables["Table1"];
            branches      = nonStockItems.Tables["Table2"];
            stockWarranty = nonStockItems.Tables["Table3"];

            nonStock.Columns.AddWithDefaultValue("NewItem", "N");
            nonStock.Columns.AddWithDefaultValue("Updated", "N");
            prices.Columns.AddWithDefaultValue("Updated", "N");

            prices.Columns.Add("priceHPBefore");
            prices.Columns.Add("priceCashBefore");
            prices.Columns.Add("priceDutyFreeBefore");
            prices.Columns.Add("priceCostbefore");
            foreach (DataRow dr in prices.Rows)
            {
                dr["priceHPBefore"]       = dr[CN.UnitPriceHP];
                dr["priceCashBefore"]     = dr[CN.UnitPriceCash];
                dr["priceDutyFreeBefore"] = dr[CN.UnitPriceDutyFree];
                dr["priceCostbefore"]     = dr[CN.CostPrice];
            }
            boundNonStockList = new BindingSource();
            boundNonStockList.DataSource = nonStock;
            dgItemDetails.DataSource = boundNonStockList;

            boundPrices = new BindingSource();
            boundPrices.DataSource = prices;
            dgPriceDetails.DataSource = boundPrices;
            
            //dvNonStockView = new DataView(nonStock);
            //dvPricesView = new DataView(prices);

            dgItemDetails
                .ColumnStyleInit()
                .ColumnStyle(CN.ItemNo, null, 60)
                .ColumnStyle(CN.ItemDescr1, "Description 1", 150)
                .ColumnStyle(CN.ItemDescr2, "Description 2", 150)
                .ColumnStyle(CN.StartDate, null, 70)
                .ColumnStyle(CN.EndDate, null, 70)
                .ColumnStyle("NewItem", null, 40);
            
            var decimalStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleRight,
                Format = base.Format
            };

            dgPriceDetails
                .ColumnStyleInit()
                .ColumnStyle(CN.BranchNo, "Branch No", 47)
                .ColumnStyle(CN.UnitPriceHP, null, 69, decimalStyle)
                .ColumnStyle(CN.UnitPriceCash, null, 69, decimalStyle)
                .ColumnStyle(CN.UnitPriceDutyFree, null, 69, decimalStyle)
                .ColumnStyle(CN.CostPrice, null, 69, decimalStyle);

            loaded = true;
            nonstockIndex = 1;
            priceIndex = 1;

            dgPriceDetails.Enabled = false;

            if (NSRow("TaxType").ToString() == "I")
                lbTax.Text = "Prices Inclusive of Tax";
            else
                lbTax.Text = "Prices Exclusive of Tax";

            priceFilter(); 
        }

        private void HashMenus()
        {
            dynamicMenus = new Hashtable();
            dynamicMenus[this.Name + ":btnSave"] = this.btnSave;            
        }
                
        private void setScreen(bool enabled)
        {
            txtItemDescr1.ReadOnly = !enabled;
            txtItemDescr2.ReadOnly = !enabled;
            txtSupplierName.ReadOnly = !enabled;
            txtSupplierCode.ReadOnly = !enabled;
            txtItemDescr1.Enabled = enabled;
            txtItemDescr2.Enabled = enabled;
            txtSupplierName.Enabled = enabled;
            txtSupplierCode.Enabled = enabled;            
            drpItemCategory.Enabled = enabled;            
            chxDeleted.Enabled = enabled;               //IP - 14/06/11 - CR1212 - RI - #3815 - Re-instated
            cbTaxable.Enabled = enabled;
            //dtStartDate.Enabled = enabled;
            //dtEndDate.Enabled = enabled;

            groupBox3.Enabled = false;

            //IP - 25/07/11 - CR1254 - RI - #4036
            if (Convert.ToBoolean(Country[CountryParameterNames.RIDispCatAsDept]))
            {
                label6.Text = label6.Text.Replace("Category", GetResource("T_DEPARTMENT"));
            }
        }        

        private void getCategories()
        {
            categories = AccountManager.GetCategories( out Error);
            // Add a blank entry at the start of list            
            DataRow blankRow = categories.NewRow();
            blankRow[CN.Code] = "";
            blankRow[CN.Category] = "";
            categories.Rows.InsertAt(blankRow, 0);
            
            DataView dvcategoriesView = new DataView(categories);            
            drpItemCategory.DataSource = dvcategoriesView;
            drpItemCategory.ValueMember = CN.Code;
            drpItemCategory.DisplayMember = CN.Category;
            // remove Warranty categories
            drpItemCategory.SelectedValue = 12;
            int index = drpItemCategory.SelectedIndex;
            if (index != -1)
            {
                dvcategoriesView.Delete(index);
            }
            drpItemCategory.SelectedValue = 82;
            index = drpItemCategory.SelectedIndex;
            if (index != -1)
            {
                dvcategoriesView.Delete(index);
            }
            drpItemCategory.SelectedIndex=0; 
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            NonStock_Load(null, null);            
        }

        private void ClearScreen()
        {
            //Remove new item from grid if clearing when error on new item
            if (!ValidScreenData() && nonstockIndex >= 0 && ((DataRowView)boundNonStockList[nonstockIndex]).Row["NewItem"].ToString() == "Y")
            {
                cbRemoveItem.Checked = true;       
            }
            clear = true;
            txtItemDescr1.Text = "";
            txtItemDescr2.Text = "";
            txtSupplierName.Text = "";
            txtSupplierCode.Text = "";
            udCashPrice.Value = 0;
            udCreditPrice.Value = 0;
            udDutyFreePrice.Value = 0;
            udCostPrice.Value = 0;                       
            drpItemCategory.SelectedValue = "";            
            errorProvider1.SetError(txtItemNo, "");
            errorProvider2.SetError(txtItemNo, "");
            chxDeleted.Checked = false;
            btnLoad.Enabled = true;
            rbAll.Checked = false;
            rbCourts.Checked = false;
            rbNonCourts.Checked = false;
            rbBranch.Checked = false;
            rbUndo.Checked = false;
            gbApply.Enabled = false;

            setScreen(false);        //edit or view only

            nonStock.RejectChanges();
            dgItemDetails.Refresh();
            prices.RejectChanges();
            dgPriceDetails.Refresh();

            errorProvider1.SetError(txtItemDescr1, "");
            errorProvider1.SetError(drpItemCategory, "");
            errorProvider1.SetError(udCostPrice, "");
            errorProvider1.SetError(udCashPrice, "");
            errorProvider1.SetError(udCreditPrice, "");
            errorProvider1.SetError(udDutyFreePrice, "");
            txtItemNo.Enabled = true;
            txtItemNo.Text = "";
            cbTaxable.Checked = false;
            //cbTaxable.Enabled = true;
            nonstockIndex = -1;         //so Enddate not setby datechange
            dgItemDetails.ClearSelection();
            dgItemDetails.Enabled=true;
            clear = false;
            cbRemoveItem.Visible = false;
            txtTaxRate.Visible = false;
            txtTaxRate.ReadOnly = true;
            lbTaxRate.Visible = false;
            dtStartDate.MinDate = Date.blankDate;
            dtStartDate.Value = Date.blankDate;
            dtStartDate.Enabled = false;
            dtStartDate.Checked = false;
            dtEndDate.MinDate = Date.blankDate;
            dtEndDate.Value = Date.blankDate;
            dtEndDate.Enabled = false;
            dtEndDate.Checked = false;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearScreen();
            //txtItemNo.Text = "";            
        }

        //private void txtItemNo_TextChanged(object sender, EventArgs e)
        //{
        //    errorProvider1.SetError(txtItemNo, "");
        //    errorProvider2.SetError(txtItemNo, "");
        //    if (txtItemDescr1.Text != "")
        //    {
        //        ClearScreen();
        //    }
        //}

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            DataTable nonStockUpdated =nonStock.Clone();            // RI
            DataTable pricesUpdated = prices.Clone();

            prices.AcceptChanges();
            nonStock.AcceptChanges();

            foreach (var row in nonStock.Select(@"Updated = 'Y' "))
	        {
                nonStockUpdated.ImportRow(row);
	        }

            foreach (var row in prices.Select(@"Updated = 'Y' "))
            {
                pricesUpdated.ImportRow(row);
            }

            AccountManager.SaveNonStockItem(nonStockUpdated, pricesUpdated);

            clear = true;
            btnClear_Click(null,null);
            dgItemDetails.DataSource = null;
            dgPriceDetails.DataSource = null;
            txtItemNo.Enabled = false;
            btnClear.Enabled = false;
            btnSave.Enabled = false;
            
            
            setScreen(false);
            clear = false;
                //((MainForm)this.FormRoot).statusBar1.Text = "Item saved successfully";
        }

        private bool ValidScreenData()
        {
            bool valid = true;
           // bool warranty = false;

            if (stockItem==true)
            {
                errorProvider1.SetError(txtItemNo, "Stock Items and Warranties are not able to be viewed or maintained in CoSACS");
                valid = false;
            }
            else
            if (txtItemNo.Text.StartsWith("19") || txtItemNo.Text.StartsWith("XW"))
            {
                errorProvider1.SetError(txtItemNo, "Item Numbers starting 19 or XW are reserved for Warranties");
                valid = false;
               // warranty = true;        //Bug #3190 jec 28/02/11
            }
            else
            {
                if (edit)       //If in Edit mode
                {
                    errorProvider1.SetError(txtItemNo, "");

                    if (txtItemDescr1.Text.Trim() == "" && txtItemNo.Text != "")
                    {
                        errorProvider1.SetError(txtItemDescr1, "Item Description must be entered");
                        valid = false;
                    }
                    else
                    {
                        errorProvider1.SetError(txtItemDescr1, "");
                    }

                    if (drpItemCategory.SelectedIndex <= 0 && edit && txtItemNo.Text != "")
                    {
                        errorProvider1.SetError(drpItemCategory, "Item Category must be selected");
                        valid = false;
                    }
                    else
                    {
                        errorProvider1.SetError(drpItemCategory, "");
                    }

                    if (dtEndDate.Value < dtStartDate.Value && dtEndDate.Value!=Date.blankDate)
                    {
                        errorProvider1.SetError(dtEndDate, "End date must be later than Start date");
                        valid = false;
                    }
                    else
                    {
                        errorProvider1.SetError(dtEndDate, "");
                    }

                    //if (dtEndDate.Value < _today && dtEndDate.Value!=Date.blankDate && valid) //IP - 18/02/11 - Sprint 5.11 - #3170 - Commented out and replaced with below. Removed check on valid
                    if (dtEndDate.Value < _today && dtEndDate.Value != Date.blankDate && !chxDeleted.Checked)
                    {
                        errorProvider1.SetError(dtEndDate, "End date must not be earlier than today");
                        valid = false;
                    }
                    else
                    {
                        errorProvider1.SetError(dtEndDate, "");
                    }
                }
            }
            //do not allow Save or Clear if errors
            btnSave.Enabled = valid;
            //btnClear.Enabled = valid || stockItem || warranty;          
            btnClear.Enabled = true;    //Bug #3190 jec 28/02/11

            dgItemDetails.Enabled = valid || stockItem;      //do not allow change of item if errors
            dgPriceDetails.Enabled = valid;

            

            return valid;
        }

        private void txtItemNo_Leave(object sender, EventArgs e)
        {
            if (txtItemNo.Text.Trim().Length == 0)
            {
                ClearScreen();
            }

            if (txtItemNo.Text.Length > 0)
            {
                txtItemNo.Text = txtItemNo.Text.ToUpper();
                btnLoad.Enabled = false;
                var found = false;
                var rowindex = 0;
                stockItem = false;

                DataView currentView = new DataView(nonStock);
                if (dgItemDetails.DataSource != null)  //no rows 
                {
                    foreach (DataRowView rowView in currentView)
                    {
                        if ((string)rowView[CN.ItemNo] == Convert.ToString(txtItemNo.Text))
                        {
                            found = true;
                            break;
                        }
                        rowindex++;
                    }
                }

                if (found)
                {
                    dgItemDetails.ClearSelection();
                    dgItemDetails.CurrentCell = dgItemDetails.Rows[rowindex].Cells[0];
                    dgItemDetails.Rows[rowindex].Selected = true;
                    dgItemDetails_RowHeaderMouseClick(null, null);
                }
                else
                {
                    DataView stockWarrantyView = new DataView(stockWarranty);
                    if (dgItemDetails.DataSource != null)  //no rows 
                    {
                        foreach (DataRowView rowView in stockWarrantyView)
                        {
                            if ((string)rowView[CN.ItemNo] == Convert.ToString(txtItemNo.Text))
                            {
                                found = true;
                                break;
                            }
                            rowindex++;
                        }
                    }

                    if (found)
                    {
                        stockItem = true;
                    }
                    else
                        if (edit && (!txtItemNo.Text.StartsWith("19") && !txtItemNo.Text.StartsWith("XW")))
                    {
                        dgItemDetails.AllowUserToAddRows = true;
                        DataRow Row = nonStock.NewRow();
                        Row[CN.ItemNo] = txtItemNo.Text.ToString();
                        Row[CN.ItemDescr1] = "";
                        Row[CN.ItemDescr2] = "";
                        Row[CN.TaxRate] = Convert.ToDecimal(0);
                        Row[CN.Supplier] = "";
                        Row[CN.ItemDescr2] = "";
                        Row[CN.SupplierCode] = "";
                        Row[CN.Deleted] = "N";
                        Row[CN.StartDate] = _today;
                        Row[CN.EndDate] = Date.blankDate;
                        Row["NewItem"] = "Y";
                        Row["Updated"] = "Y";
                        nonStock.Rows.InsertAt(Row, 0);
                        dgItemDetails.AllowUserToAddRows = false;
                        dgItemDetails.ClearSelection();
                        dgItemDetails.CurrentCell = dgItemDetails.Rows[0].Cells[0];
                        dgItemDetails.Rows[0].Selected = true;
                        dtStartDate.MinDate = _today;       // new items cannot start before today
                        
                        errorProvider2.SetError(txtItemNo, "Item Number not found - item will be added when saved");
                        // Add zeroo prices
                        dgPriceDetails.AllowUserToAddRows = true;
                        foreach (DataRow row in branches.Rows)
                        {
                            DataRow Rowp = prices.NewRow();
                            Rowp[CN.ItemNo] = txtItemNo.Text.ToString();
                            Rowp[CN.BranchNo] = row[CN.BranchNo];
                            Rowp[CN.UnitPriceHP] = (string)"0.00";
                            Rowp[CN.UnitPriceCash] = (string)"0.00";
                            Rowp[CN.UnitPriceDutyFree] = (string)"0.00";
                            Rowp[CN.CostPrice] = (string)"0.00";
                            Rowp["priceHPBefore"] = (string)"0.00";
                            Rowp["priceCashBefore"] = (string)"0.00";
                            Rowp["priceDutyFreeBefore"] = (string)"0.00";
                            Rowp["priceCostbefore"] = (string)"0.00";
                            Rowp[CN.StoreType] = (string)row[CN.StoreType];
                            Rowp["Updated"] = "Y";
                            prices.Rows.InsertAt(Rowp, 0);
                        }
                        dgPriceDetails.AllowUserToAddRows = false;
                        priceFilter();
                        dgPriceDetails.ClearSelection();
                        //dgPriceDetails.CurrentCell = dgPriceDetails.Rows[0].Cells[0];
                        dgPriceDetails.Rows[0].Selected = true;

                        txtItemNo.Enabled = false;
                        drpItemCategory.Enabled = true;
                        txtItemDescr1.ReadOnly = false;
                        txtItemDescr2.ReadOnly = false;
                        txtItemDescr1.Enabled = true;
                        txtItemDescr2.Enabled = true;
                        cbTaxable.Enabled = true;
                        chxDeleted.Enabled = false;         //not allow for new item
                        cbRemoveItem.Visible = true;
                        btnSave.Enabled = false;
                        dgPriceDetails.Enabled = edit;
                    }
                    else
                    {
                        errorProvider1.SetError(txtItemNo, "Item not found - New Items can only be added if you have permissions");
                    }
                }

                //dgItemDetails_RowHeaderMouseClick(null, null);

                btnLoad.Enabled = true;

                priceFilter();

                ValidScreenData();
            }
        }

        //private void setDefaults()
        //{   
        //    udCashPrice.Value = 0;
        //    udCreditPrice.Value = 0;
        //    udDutyFreePrice.Value = 0;
        //    udCostPrice.Value = 0;
        //    drpItemCategory.SelectedValue = "";            
        //}

        private void chxDeleted_CheckedChanged(object sender, EventArgs e)
        {
            if (!clear)
            {
                if (chxDeleted.Checked)
                {
                    ((DataRowView)boundNonStockList[nonstockIndex]).Row[CN.Deleted] = "Y";
                    dtEndDate.Enabled = false;
                }
                else
                {
                    ((DataRowView)boundNonStockList[nonstockIndex]).Row[CN.Deleted] = "N";
                    dtEndDate.Enabled = true;
                }

                ValidScreenData();
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {

        }

        private void btnExport_Click(object sender, EventArgs e)
        {

        }    

        private void dgItemDetails_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            stockItem = false;      //IP - 15/02/11 - Sprint 5.10 - #3151
            nonstockIndex = dgItemDetails.CurrentRow.Index;
            
            txtItemNo.Text = NSRow(CN.ItemNo).ToString();
            txtItemDescr1.Text = NSRow(CN.ItemDescr1).ToString();
            txtItemDescr2.Text =  NSRow(CN.ItemDescr2).ToString();          
            
            //IP - 16/02/11 - Sprint 5.10 - #3149 - if category < 10 then need to add a '0' to the front. For e.g. category 1 is displayed as 01 in the drop down.
            var category = Convert.ToString( NSRow(CN.Category)).TryParseInt32();
            if(category.HasValue)
                drpItemCategory.SelectedValue = category.Value.ToString("0#");

            chxDeleted.Checked = ( NSRow(CN.Deleted).ToString() == "Y" ? true : false);

            AddZeroPrices(null,null);        //insert any missing prices

            //existing items will have null date
            //IP - 15/02/11 - Sprint 5.10 - #3151- for existing items the start date maybe 01/01/1900 therefore need to set  dtStartDate.MinDate to blank to allow this.
            if (((DataRowView)boundNonStockList[nonstockIndex]).Row[CN.NewItem].ToString() == "N")
            {
                dtStartDate.MinDate = Date.blankDate;
            }
            else
            {
                dtStartDate.MinDate = _today;
            }

            dtEndDate.MinDate = Date.blankDate;
            dtEndDate.Value = Date.blankDate;
            dtEndDate.Checked = false;
            dtEndDate.Enabled = false; 

            dtStartDate.Value = Convert.ToDateTime(((DataRowView)boundNonStockList[nonstockIndex]).Row[CN.StartDate]);
            if (dtStartDate.Value < _today)
            {
                dtStartDate.Enabled = false;
            }
            else
            {
                dtStartDate.Enabled = true;
            }
            dtEndDate.MinDate = Convert.ToDateTime(((DataRowView)boundNonStockList[nonstockIndex]).Row[CN.EndDate]);
            dtEndDate.Value = Convert.ToDateTime(((DataRowView)boundNonStockList[nonstockIndex]).Row[CN.EndDate]);

            if (dtEndDate.Value < _today && dtEndDate.Value!= Date.blankDate)
            {
                dtEndDate.Enabled = false;
            }
            else
            {
                dtEndDate.Enabled = true;
                dtEndDate.Checked = false;
            }

            priceFilter();

            txtItemNo.Enabled = false;
            setScreen(edit);

            if (Convert.ToDecimal(NSRow(CN.TaxRate)) == 0)
            {
                cbTaxable.Checked = false;
            }
            else
            {
                cbTaxable.Checked = true;
            }

            //if taxrate not same as country rate and not zero - rate is maintained in Code maintenance
            if (Convert.ToString(NSRow(CN.TaxRate)) != Convert.ToString(Country[CountryParameterNames.TaxRate])
                    && Convert.ToDouble(NSRow(CN.TaxRate)) != 0.0)
            {
                cbTaxable.Enabled=false;                
            }

            cbTaxable_CheckedChanged(null, null);       //force taxrate check

            udCreditPrice.Value = 0;
            udCashPrice.Value = 0;
            udDutyFreePrice.Value = 0;
            udCostPrice.Value = 0;
            dgPriceDetails.Enabled = edit;
            chxDeleted.Enabled = edit && (NSRow("NewItem").ToString() != "Y");  //not allowed for New item
            errorProvider2.SetError(txtItemNo, "");
            errorProvider1.SetError(txtItemNo, "");
            cbRemoveItem.Visible = (NSRow("NewItem").ToString() == "Y" ? true : false);

            ValidScreenData();

        }

        private object NSRow(string column)
        { 
           return ((DataRowView)boundNonStockList[nonstockIndex]).Row[column];
        }

        private void NSRowSet(string column, object value)
        {
            ((DataRowView)boundNonStockList[nonstockIndex]).Row[column] = value;
        }

        private object PriceRow(string column)
        {
            return ((DataRowView)boundPrices[priceIndex]).Row[column];
        }

        private void PriceRowSet(string column, object value)
        {
            ((DataRowView)boundPrices[priceIndex]).Row[column] = value;
        }

        private void dgPriceDetails_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            priceIndex = dgPriceDetails.CurrentRow.Index;

            udCreditPrice.Value = Convert.ToDecimal(PriceRow(CN.UnitPriceHP));
            udCashPrice.Value = Convert.ToDecimal(PriceRow(CN.UnitPriceCash));
            udDutyFreePrice.Value = Convert.ToDecimal(PriceRow(CN.UnitPriceDutyFree));
            udCostPrice.Value = Convert.ToDecimal(PriceRow(CN.CostPrice));
            groupBox3.Enabled = true;
            rbBranch.Checked = true;
            rbAll.Checked = false;
            rbCourts.Checked = false;
            rbNonCourts.Checked = false;
            rbUndo.Checked = false;
            udCostPrice.Enabled = edit;
            udCashPrice.Enabled = edit;
            udDutyFreePrice.Enabled = edit;
            udCreditPrice.Enabled = edit;
            gbApply.Enabled = edit;
        }

        private void priceFilter()
        {
            boundPrices.Filter = CN.ItemNo + " = '" + txtItemNo.Text + "'";
            boundPrices.Sort = CN.BranchNo;
            dgPriceDetails.DataSource = boundPrices;
            dgPriceDetails.ClearSelection();           
        }              

        private void txtItemDescr1_Leave(object sender, EventArgs e)
        {
            ValidScreenData();

            if (!clear)
            {
                NSRowSet(CN.ItemDescr1,txtItemDescr1.Text.ToString());
                NSRowSet("Updated","Y");
            }                                
        }

        private void txtItemDescr2_Leave(object sender, EventArgs e)
        {
            ValidScreenData();
            if (!clear)
            {
                NSRowSet(CN.ItemDescr2, txtItemDescr2.Text.ToString());
                NSRowSet("Updated", "Y");
            }
        }

        private void drpItemCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (loaded == true && !clear && ValidScreenData()) //IP - 16/02/11 - Sprint 5.10 - #3149 - Replaced this with the below. Check category selected from the drop down before setting DataView value.
            if (loaded == true && 
                !clear && 
                Convert.ToString(drpItemCategory.SelectedValue) != string.Empty) 
            {
                var existingCategory = Convert.ToString(NSRow(CN.Category));

                if (existingCategory.TryParseInt16(0) != Convert.ToInt16(drpItemCategory.SelectedValue))
                {
                    NSRowSet("Updated","Y");
                }
                NSRowSet(CN.Category, Convert.ToInt16(drpItemCategory.SelectedValue));
                
            }

            ValidScreenData();
        }

        //private void drpItemStatus_SelectedIndexChanged(object sender, EventArgs e)
        //{

        //}

        private void cbTaxable_CheckedChanged(object sender, EventArgs e)
        {
            if (!clear)
            {
                if (cbTaxable.Checked == true)
                {
                    if (NSRow("NewItem").ToString() == "Y")        //new item use country rate
                    {
                        NSRowSet(CN.TaxRate, (decimal)(Country[CountryParameterNames.TaxRate]));
                        NSRowSet("Updated","Y");
                        txtTaxRate.Text = Convert.ToString(Country[CountryParameterNames.TaxRate]);
                        txtTaxRate.Visible = true;
                        lbTaxRate.Visible = true;
                    }
                    else
                    {
                        if (Convert.ToDouble(NSRow(CN.TaxRate)) == 0.0)
                        {
                             NSRowSet(CN.TaxRate,(decimal)(Country[CountryParameterNames.TaxRate]));
                             NSRowSet("Updated", "Y");
                        }
                        
                        txtTaxRate.Text = Convert.ToString(NSRow(CN.TaxRate));
                        txtTaxRate.Visible = true;
                        lbTaxRate.Visible = true;
                    }
                }
                else
                {
                     NSRowSet(CN.TaxRate,0);
                    NSRowSet("Updated","Y");
                    txtTaxRate.Visible = false;
                    lbTaxRate.Visible = false;                    
                }
            }
        }

        private void rbAll_CheckedChanged(object sender, EventArgs e)
        { 
            if (rbAll.Checked == true)
            {
                foreach (DataRow dr in prices.Rows)
                {
                    if(dr[CN.ItemNo].ToString()==txtItemNo.Text)
                    {
                        dr[CN.UnitPriceHP] = udCreditPrice.Value.ToString();
                        dr[CN.UnitPriceCash] = udCashPrice.Value.ToString();
                        dr[CN.UnitPriceDutyFree] = udDutyFreePrice.Value.ToString();
                        dr[CN.CostPrice] = udCostPrice.Value.ToString();
                        dr["Updated"] = "Y";
                    }
                }
            }            
        }

        private void rbCourts_CheckedChanged(object sender, EventArgs e)
        {
            if (rbCourts.Checked == true)
            {
                foreach (DataRow dr in prices.Rows)
                {
                    if (dr[CN.ItemNo].ToString() == txtItemNo.Text && dr[CN.StoreType].ToString() == StoreType.Courts)
                    {
                        dr[CN.UnitPriceHP] = udCreditPrice.Value.ToString();
                        dr[CN.UnitPriceCash] = udCashPrice.Value.ToString();
                        dr[CN.UnitPriceDutyFree] = udDutyFreePrice.Value.ToString();
                        dr[CN.CostPrice] = udCostPrice.Value.ToString();
                        dr["Updated"] = "Y";
                    }
                }
            }
        }

        private void rbNonCourts_CheckedChanged(object sender, EventArgs e)
        {
            if (rbNonCourts.Checked == true)
            {
                foreach (DataRow dr in prices.Rows)
                {
                    if (dr[CN.ItemNo].ToString() == txtItemNo.Text && dr[CN.StoreType].ToString() == StoreType.NonCourts)
                    {
                        dr[CN.UnitPriceHP] = udCreditPrice.Value.ToString();
                        dr[CN.UnitPriceCash] = udCashPrice.Value.ToString();
                        dr[CN.UnitPriceDutyFree] = udDutyFreePrice.Value.ToString();
                        dr[CN.CostPrice] = udCostPrice.Value.ToString();
                    }
                }
            }
        }

        private void rbBranch_CheckedChanged(object sender, EventArgs e)
        {
            if (rbBranch.Checked == true)
            {
                var branch = PriceRow(CN.BranchNo).ToString();                
                foreach (DataRow dr in prices.Rows)
                {
                    if (dr[CN.ItemNo].ToString() == txtItemNo.Text && dr[CN.BranchNo].ToString() == branch)
                    {
                        dr[CN.UnitPriceHP] = udCreditPrice.Value.ToString();
                        dr[CN.UnitPriceCash] = udCashPrice.Value.ToString();
                        dr[CN.UnitPriceDutyFree] = udDutyFreePrice.Value.ToString();
                        dr[CN.CostPrice] = udCostPrice.Value.ToString();
                    }
                }
            }
        }

        private void rbUndo_CheckedChanged(object sender, EventArgs e)
        {
            if (rbUndo.Checked == true)
            {
                foreach (DataRow dr in prices.Rows)
                {
                    if (dr[CN.ItemNo].ToString() == txtItemNo.Text)
                    {
                        dr[CN.UnitPriceHP] = dr["priceHPBefore"];
                        dr[CN.UnitPriceCash] = dr["priceCashBefore"];
                        dr[CN.UnitPriceDutyFree] = dr["priceDutyFreeBefore"];
                        dr[CN.CostPrice] = dr["priceCostbefore"];
                    }
                }
            }     
        }

        private void lbTax_Click(object sender, EventArgs e)
        {

        }

        private void AddZeroPrices(object sender, EventArgs e)
        {
            var found = false;            
            // Add zero prices
            dgPriceDetails.AllowUserToAddRows = true;
            foreach (DataRow row in branches.Rows)
            {
                found = false;
                foreach (DataRow rowc in prices.Rows)
                {
                    if (Convert.ToString(rowc[CN.ItemNo]) == txtItemNo.Text.ToString() && Convert.ToInt16(rowc[CN.BranchNo]) == Convert.ToInt16(row[CN.BranchNo]))
                    {
                        found = true;
                        break;                        
                    }                    
                }
                if (!found)
                {
                    DataRow Rowp = prices.NewRow();
                    Rowp[CN.ItemNo] = txtItemNo.Text.ToString();
                    Rowp[CN.ItemId] = NSRow(CN.ItemId); 
                    Rowp[CN.BranchNo] = row[CN.BranchNo];
                    Rowp[CN.UnitPriceHP] = (string)"0.00";
                    Rowp[CN.UnitPriceCash] = (string)"0.00";
                    Rowp[CN.UnitPriceDutyFree] = (string)"0.00";
                    Rowp[CN.CostPrice] = (string)"0.00";
                    Rowp["priceHPBefore"] = (string)"0.00";
                    Rowp["priceCashBefore"] = (string)"0.00";
                    Rowp["priceDutyFreeBefore"] = (string)"0.00";
                    Rowp["priceCostbefore"] = (string)"0.00";
                    Rowp[CN.StoreType] = (string)row[CN.StoreType];
                    Rowp["Updated"] = "Y";
                    prices.Rows.InsertAt(Rowp, 0);
                }
            }
            dgPriceDetails.AllowUserToAddRows = false;
            priceFilter();
            dgPriceDetails.ClearSelection();
            //dgPriceDetails.CurrentCell = dgPriceDetails.Rows[0].Cells[0];
            dgPriceDetails.Rows[0].Selected = true;

        }

        private void cbRemove_CheckedChanged(object sender, EventArgs e)
        {
            if (cbRemoveItem.Checked == true)
            {
                boundNonStockList.RemoveAt(nonstockIndex);                
                txtItemNo.Enabled = true;
                errorProvider2.SetError(txtItemNo, "");
                cbRemoveItem.Visible = false;
                cbRemoveItem.Checked = false;
                txtTaxRate.Visible = false;
                lbTaxRate.Visible = false;
                var rowindex = 0;

                DataView priceView = new DataView(prices);
                foreach (DataRowView rowView in priceView)
                {
                    if ((string)rowView[CN.ItemNo] == Convert.ToString(txtItemNo.Text))
                    {
                        boundPrices.RemoveAt(rowindex);
                    }
                    //rowindex++;
                }

                btnClear_Click(null, null);                
            }
        }

        private void udCreditPrice_ValueChanged(object sender, EventArgs e)
        {
            PriceRowSet(CN.UnitPriceHP, udCreditPrice.Value.ToString());
            PriceRowSet("Updated","Y");
            dgPriceDetails.Refresh();             
        }

        private void udCashPrice_ValueChanged(object sender, EventArgs e)
        {
            PriceRowSet(CN.UnitPriceCash, udCashPrice.Value.ToString());
            PriceRowSet("Updated", "Y");
            dgPriceDetails.Refresh(); 
        }

        private void udDutyFreePrice_ValueChanged(object sender, EventArgs e)
        {
            PriceRowSet(CN.UnitPriceDutyFree,udDutyFreePrice.Value.ToString());
            PriceRowSet("Updated", "Y");
            dgPriceDetails.Refresh(); 
        }

        private void udCostPrice_ValueChanged(object sender, EventArgs e)
        {
            PriceRowSet(CN.CostPrice,udCostPrice.Value.ToString());
            PriceRowSet("Updated", "Y");
            dgPriceDetails.Refresh(); 
        }

        //private void udCreditPrice_Validated(object sender, EventArgs e)
        //{
        //    PriceRowSet(CN.UnitPriceHP,udCreditPrice.Value.ToString());
        //    PriceRowSet("Updated", "Y");
        //    dgPriceDetails.Refresh();
        //}

        private void dtStartDate_ValueChanged(object sender, EventArgs e)
        {
            if (nonstockIndex >= 0)
            {
                NSRowSet(CN.StartDate,dtStartDate.Value.ToString());
                NSRowSet("Updated","Y");
                ValidScreenData();
            }
        }

        private void dtEndDate_ValueChanged(object sender, EventArgs e)
        {
            if (dtEndDate.Checked == true && endDateTbChange==false)
            {
                if (nonstockIndex >= 0)
                {
                    if (dtEndDate.MinDate >= _today)
                    {
                        dtEndDate.MinDate = _today;
                    }
                    else
                    {
                        dtEndDate.MinDate = Convert.ToDateTime(NSRow(CN.EndDate));
                    }
                    
                    NSRowSet(CN.EndDate,dtEndDate.Value.ToString());
                    NSRowSet("Updated", "Y");
                }
            }
            else
            {
                endDateTbChange = !endDateTbChange;     //required cos change in checkbox not reflected immediately
                dtEndDate.MinDate = Date.blankDate;
                dtEndDate.Value = Date.blankDate;
                NSRowSet(CN.EndDate,dtEndDate.Value.ToString());
                NSRowSet("Updated","Y");
            }

            ValidScreenData();
        }
    }  
}
