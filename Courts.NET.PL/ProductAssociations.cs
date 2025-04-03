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
using Blue.Cosacs.Shared.Extensions;
using STL.PL.Utils;


namespace STL.PL
{
    public partial class Product_Associations : CommonForm
    {
        private DataSet productAssoc;
        private DataView dvproductAssocView;
        private DataTable associatedItems;
        private DataView dvProductGroupView;
        private DataView dvCategoryView;
        private DataView dvClassView;
        private DataView dvSubClassView;
        private DataTable productGroup;
        private DataTable category;
        private DataTable Class;
        private DataTable subClass;

        public int? ItemID { get; set; }
        public string ItemDescr1 { get; set; }
        public string ItemDescr2 { get; set; }

        private bool enableFilter = true;

        public Product_Associations(Form root, Form parent)
        {
            InitializeComponent();

            FormRoot = root;
            FormParent = parent;

            btnReload_Click(null,null);

            //IP - 25/07/11 - CR1254 - RI - #4036
            if (Convert.ToBoolean(Country[CountryParameterNames.RIDispCatAsDept]))
            {
                lbCategory.Text = GetResource("T_DEPARTMENT");
            }
        }


        private void LoadData()
        {
            productAssoc = StaticDataManager.ProductAssociationGetDetails();

            //drpProductGroup.DataSource = productAssoc.Tables["Table"];

            drpProductGroup.DisplayMember = "ProductGroupDescr";
            drpProductGroup.ValueMember = CN.ProductGroup;
            drpProductGroup.Text = CN.ProductGroup;
            //drpCategory.DataSource = productAssoc.Tables["Table1"];
            drpCategory.DisplayMember = "CategoryDescr";
            drpCategory.ValueMember = CN.Category;
            drpCategory.Text = CN.Category;
            //drpClass.DataSource = productAssoc.Tables["Table2"];
            drpClass.DisplayMember = "ClassDescr";
            drpClass.ValueMember = CN.Class;
            drpClass.Text = CN.Class;
            //drpSubClass.DataSource = productAssoc.Tables["Table3"];
            drpSubClass.DisplayMember = "SubClassDescr";
            drpSubClass.ValueMember = CN.SubClass;
            drpSubClass.Text = CN.SubClass;

            DataRow blankRow = productAssoc.Tables["Table"].NewRow();
            blankRow[CN.ProductGroup] = "Any";
            blankRow["ProductGroupDescr"] = "Any";            
            productAssoc.Tables["Table"].Rows.InsertAt(blankRow, 0);

            DataRow blankRow1 = productAssoc.Tables["Table1"].NewRow();
            blankRow1[CN.Category] = "0";
            blankRow1["CategoryDescr"] = "Any";
            blankRow1["ParentCode"] = "Any";
            productAssoc.Tables["Table1"].Rows.InsertAt(blankRow1, 0);

            DataRow blankRow2 = productAssoc.Tables["Table2"].NewRow();
            blankRow2[CN.Class] = "Any";
            blankRow2["ClassDescr"] = "Any";
            blankRow2["ParentCode"] = "Any";
            productAssoc.Tables["Table2"].Rows.InsertAt(blankRow2, 0);

            DataRow blankRow3 = productAssoc.Tables["Table3"].NewRow();
            blankRow3[CN.SubClass] = "Any";
            blankRow3["SubClassDescr"] = "Any";
            blankRow3["ParentCode"] = "Any";
            productAssoc.Tables["Table3"].Rows.InsertAt(blankRow3, 0);

            //SetDropdownDefaults();

            productGroup = productAssoc.Tables["Table"];
            category = productAssoc.Tables["Table1"];
            Class = productAssoc.Tables["Table2"];
            subClass = productAssoc.Tables["Table3"];
            associatedItems = productAssoc.Tables["Table4"];
            associatedItems.Columns.AddWithDefaultValue("Deleted", "N");


            dvProductGroupView = new DataView(productGroup);
            dvCategoryView = new DataView(category);
            dvClassView = new DataView(Class);
            dvSubClassView = new DataView(subClass);
            drpProductGroup.DataSource = dvProductGroupView;
            drpCategory.DataSource = dvCategoryView;
            drpClass.DataSource = dvClassView;
            drpSubClass.DataSource = dvSubClassView;

            dgvAssociatedItems.DataSource = associatedItems;
            dvproductAssocView = new DataView(associatedItems);
            dgvAssociatedItems.DataSource = dvproductAssocView;

            dgvAssociatedItems
                .ColumnStyleInit()
                .ColumnStyle("ProductGroupDescr", null, 150)
                .ColumnStyle("CategoryDescr", null, 150)                  
                .ColumnStyle("ClassDescr", null, 150)
                .ColumnStyle("SubClassDescr", null, 150)
                .ColumnStyle(CN.AssociatedItem, null, 80)
                .ColumnStyle(CN.Description1, null, 150)
                .ColumnStyle(CN.Description2, null, 150);

            dgvAssociatedItems.ClearSelection();

            //IP - 25/07/11 - CR1254 - RI - #4036
            if (Convert.ToBoolean(Country[CountryParameterNames.RIDispCatAsDept]))
            {
                if (dgvAssociatedItems.Columns.Contains("CategoryDescr"))
                {
                    dgvAssociatedItems.Columns["CategoryDescr"].HeaderText = GetResource("T_DEPARTMENT") + " Descr";
                }
            }

            btnSave.Enabled = false;
            drpCategory.Enabled = false;
            drpClass.Enabled = false;
            drpSubClass.Enabled = false;

            SetDropdownDefaults();

        }


        //private void Product_Associations_Load(object sender, EventArgs e)
        //{
        //    loaded = false;
        //}

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                Function = "btnAdd_Click";
                //ClearErrors();

                //validate entry
                if (ValidEntry())
                {
                    // Check whether to replace a row in edit for the same return code
                    DataView currentView = new DataView(associatedItems);
                    DataRowView rowViewFound = null;
                    if (dgvAssociatedItems.DataSource != null)  //no rows 
                    {
                        foreach (DataRowView rowView in currentView)
                        {
                            if ((string)rowView[CN.ProductGroup] == Convert.ToString(drpProductGroup.SelectedValue)
                                        && Convert.ToString(rowView[CN.Category]) == Convert.ToString(drpCategory.SelectedValue)
                                        && Convert.ToString(rowView[CN.Class]) == Convert.ToString(drpClass.SelectedValue)
                                        && Convert.ToString(rowView[CN.SubClass]) == Convert.ToString(drpSubClass.SelectedValue)
                                        && Convert.ToString(rowView[CN.AssociatedItem]) == Convert.ToString(txtAssociatedItem.Text))
                            {
                                rowViewFound = rowView;
                            }
                        }
                    }

                    if (rowViewFound != null)
                    {
                        currentView.AllowEdit = true;
                        // Update the matching edit row found
                        if (rowViewFound[CN.AssociatedItem].ToString() == "Delete")
                        {
                            rowViewFound[CN.AssociatedItem] = txtAssociatedItem.Text;
                            rowViewFound[CN.Description1] = ItemDescr1;
                            rowViewFound[CN.Description2] = ItemDescr2;
                            rowViewFound[CN.Deleted] = "N";     //reinstate delete row
                        }

                        currentView.AllowEdit = false;

                        dvproductAssocView.RowFilter = CN.Deleted + " = '" + Convert.ToString("N") + "'";
                        dgvAssociatedItems.DataSource = dvproductAssocView;
                    }
                    else
                    {
                        // Add the new row as an edit row
                        currentView.AllowNew = true;
                        DataRowView rowView = currentView.AddNew();
                        rowView[CN.ProductGroup] = drpProductGroup.SelectedValue;
                        rowView[CN.Category] = drpCategory.SelectedValue;
                        rowView[CN.Class] = drpClass.SelectedValue;
                        rowView[CN.SubClass] = drpSubClass.SelectedValue;
                        rowView[CN.AssociatedItem] = txtAssociatedItem.Text;
                        rowView["ProductGroupDescr"] = drpProductGroup.Text.ToString();
                        rowView["CategoryDescr"] = drpCategory.Text.ToString();
                        rowView["ClassDescr"] = drpClass.Text.ToString();
                        rowView["SubClassDescr"] = drpSubClass.Text.ToString();
                        rowView[CN.Description1] = ItemDescr1;
                        rowView[CN.Description2] = ItemDescr2;
                        rowView[CN.ItemId] = ItemID;
                        rowView[CN.Deleted] = "N";

                        rowView.EndEdit();
                        currentView.AllowNew = false;
                    }

                    // Reset the input fields
                    txtAssociatedItem.Text = "";
                    // enable save button 
                    btnSave.Enabled = true;

                    SetDropdownDefaults();

                    dvproductAssocView = new DataView(associatedItems);
                    dgvAssociatedItems.DataSource = dvproductAssocView;

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
                //ClearErrors();

                // Check whether to replace a row in edit for the same return code
                DataView currentView = new DataView(associatedItems);
                int index = dgvAssociatedItems.CurrentRow.Index;
                //DataRowView rowViewFound = null;
                //if (dgvAssociatedItems.DataSource != null)  //no rows 
                //{
                //    foreach (DataRowView rowView in currentView)
                //    {
                //        if ((string)rowView[CN.ProductGroup] == Convert.ToString(drpProductGroup.SelectedValue)
                //                        && Convert.ToString(rowView[CN.Category]) == Convert.ToString(drpCategory.SelectedValue)
                //                        && Convert.ToString(rowView[CN.Class]) == Convert.ToString(drpClass.SelectedValue)
                //                        && Convert.ToString(rowView[CN.SubClass]) == Convert.ToString(drpSubClass.SelectedValue))
                //        {
                //            rowViewFound = rowView;
                //        }
                //    }
                //}

                
                //if (rowViewFound != null)
                //{
                    //currentView.AllowEdit = true;
                    //rowViewFound[CN.AssociatedItem] = "Delete";
                    //rowViewFound[CN.Description1] = "Delete";
                    //rowViewFound[CN.Description2] = "Delete";
                    //rowViewFound[CN.Deleted] = "Y";

                    //currentView.AllowEdit = false

                    //currentView.AllowEdit = true;
                    dvproductAssocView[index][CN.AssociatedItem] = "Delete";
                    dvproductAssocView[index][CN.Description1] = "Delete";
                    dvproductAssocView[index][CN.Description2] = "Delete";
                    dvproductAssocView[index][CN.Deleted] = "Y";

                    //currentView.AllowEdit = false;

                    dvproductAssocView.RowFilter = CN.Deleted + " = '" + Convert.ToString("N") + "'";

                    associatedItems.AcceptChanges();
                    //dvproductAssocView.Sort = CN.BranchNo;
                    dgvAssociatedItems.DataSource = dvproductAssocView;
                    // Reset the input fields
                    txtAssociatedItem.Text = "";
                    // enable save button 
                    btnSave.Enabled = true;
                //}

            }

            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            btnClear_Click(null, null);
            LoadData();
            //dvproductAssocView.RowFilter=null;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            StaticDataManager.ProductAssociationSaveDetails(associatedItems);
            dgvAssociatedItems.DataSource = null;

            btnClear_Click(null, null);
        }


        private void dgvAssociatedItems_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (dgvAssociatedItems.CurrentRow != null)          // #8719 jec 24/11/11 ensure row is selected 
                {
                    enableFilter = false;
                    int index = dgvAssociatedItems.CurrentRow.Index;
                    drpProductGroup.SelectedValue = (string)dvproductAssocView[index][CN.ProductGroup].ToString();
                    drpCategory.SelectedValue = (string)dvproductAssocView[index][CN.Category].ToString();
                    drpClass.SelectedValue = (string)dvproductAssocView[index][CN.Class].ToString();
                    drpSubClass.SelectedValue = (string)dvproductAssocView[index][CN.SubClass].ToString();

                    txtAssociatedItem.Text = (string)dvproductAssocView[index]["AssociatedItem"].ToString();

                    btnDelete.Enabled = true;
                }
            }
            finally
            {
                enableFilter = true;
            }
        }

        private bool ValidEntry()
        {
            var valid = true;

            if (drpProductGroup.SelectedIndex == -1)
            {
                errorProvider1.SetError(drpProductGroup, "Product Group must be selected");
                valid = false;
            }
            else
                errorProvider1.SetError(drpProductGroup, "");

            if (drpCategory.SelectedIndex == -1)
            {
                errorProvider1.SetError(drpCategory, "Category must be selected");
                valid = false;
            }
            else
                errorProvider1.SetError(drpCategory, "");

            if (drpClass.SelectedIndex == -1)
            {
                errorProvider1.SetError(drpClass, "Class must be selected");
                valid = false;
            }
            else
                errorProvider1.SetError(drpClass, "");

            if (drpSubClass.SelectedIndex == -1)
            {
                errorProvider1.SetError(drpSubClass, "Sub Class must be selected");
                valid = false;
            }
            else
                errorProvider1.SetError(drpSubClass, "");

            if (txtAssociatedItem.Text == "")
            {
                errorProvider1.SetError(txtAssociatedItem, "Associated Item must be entered");
                valid = false;
            }
            else
                errorProvider1.SetError(txtAssociatedItem, "");

            return valid;
        }

        private void txtAssociatedItem_Leave(object sender, EventArgs e)
        {
            _hasdatachanged = true;
            Function = "txtAssociatedItem_Validating";
            ItemID = 0;
            errorProvider1.SetError(txtAssociatedItem, "");

            try
            {
                Wait();
                //needs to Be upper case
                txtAssociatedItem.Text = txtAssociatedItem.Text.ToUpper();

                if (txtAssociatedItem.Text == "LOAN")
                {
                    errorProvider1.SetError(txtAssociatedItem, "Invalid Associated Item");
                }
                else
                {
                    if (txtAssociatedItem.Text.Length > 0)
                    {
                        btnAdd.Enabled = true;
                        var items = new List<WSStock.StockInfo>(StockManager.GetStockInfo(txtAssociatedItem.Text, false, null, false, out Error));  //jec 16/06/11

                        if (Error.Length > 0)
                        {
                            ShowError(Error);
                            return;
                        }


                        if (items.Count == 1)
                        {
                            ItemID = items[0].Id;
                            txtAssociatedItem.Text = items[0].IUPC;  //if the barcode was entered
                            ItemDescr1 = items[0].itemdescr1;
                            ItemDescr2 = items[0].itemdescr2;
                        }
                        else if (items.Count > 1 && ItemID == 0)
                        {
                            //If the SKU has been entered and more than one item returned (IUPC different) then load the
                            //Stock Enquiry By Product screen.

                            var mainForm = FormRoot as MainForm;
                            var form = mainForm.GetIfExists<CodeStockEnquiry>();

                            if (form == null)
                            {
                                form = new CodeStockEnquiry();
                                mainForm.AddTabPage(form);
                            }

                            form.ItemNo = txtAssociatedItem.Text;
                            form.FormParent = this;
                            form.FormRoot = this.FormRoot;

                            mainForm.FocusIfExists<CodeStockEnquiry>(form);

                            //txtQuantity.Select(0, txtQuantity.Text.Length);
                            //txtQuantity.Focus();
                        }
                        else if (items.Count == 0)
                        {
                            errorProvider1.SetError(txtAssociatedItem, "Item not found");
                            btnAdd.Enabled=false;
                        }

                    }

                    //if (PaidAndTaken)
                    //    drpLocation_Validating(this, null);
                }
            }
            catch (NullReferenceException ex)
            {
                txtAssociatedItem.Focus();
                txtAssociatedItem.Select(0, txtAssociatedItem.Text.Length);
                errorProvider1.SetError(txtAssociatedItem, ex.Message);
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

        private void SetDropdownDefaults()
        {
            drpProductGroup.SelectedIndex = -1;
            drpCategory.SelectedIndex = -1;
            drpClass.SelectedIndex = -1;
            drpSubClass.SelectedIndex = -1;
            txtAssociatedItem.Text = "";

            drpCategory.Enabled = false;
            drpClass.Enabled = false;
            drpSubClass.Enabled = false;
            btnAdd.Enabled = false;
            btnDelete.Enabled = false;
            txtAssociatedItem.Enabled = false;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            SetDropdownDefaults();
            txtAssociatedItem.Text = "";
            dgvAssociatedItems.DataSource = null;
            //associatedItems.Clear();

            errorProvider1.SetError(drpProductGroup, "");
            errorProvider1.SetError(drpCategory, "");
            errorProvider1.SetError(drpClass, "");
            errorProvider1.SetError(drpSubClass, "");
            errorProvider1.SetError(txtAssociatedItem, "");

            btnSave.Enabled = false;            
        }

        private void drpProductGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (drpProductGroup.SelectedIndex != -1)
            {
                dvCategoryView.RowFilter = "ParentCode" + " = '" + Convert.ToString(drpProductGroup.SelectedValue) + "'" + " or ParentCode" + " = '" +  Convert.ToString("Any") + "'";
                drpCategory.Enabled = true;
                drpCategory.SelectedIndex = -1;
                drpClass.SelectedIndex = -1;
                drpSubClass.SelectedIndex = -1;

                ApplyFilter();
                
            }
        }

        private void drpCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (drpCategory.SelectedIndex != -1)
            {
                dvClassView.RowFilter = "ParentCode" + " = '" + Convert.ToString(drpCategory.SelectedValue) + "'" + " or ParentCode" + " = '" + Convert.ToString("Any") + "'";
                drpClass.Enabled = true;
                drpClass.SelectedIndex = -1;
                drpSubClass.SelectedIndex = -1;

                ApplyFilter();
                
            }
        }

        private void drpClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (drpClass.SelectedIndex != -1)
            {
                dvSubClassView.RowFilter = "ParentCode" + " = '" + Convert.ToString(drpClass.SelectedValue) + "'" + " or ParentCode" + " = '" + Convert.ToString("Any") + "'";
                drpSubClass.Enabled = true;
                drpSubClass.SelectedIndex = -1;

                ApplyFilter();
                
            }
        }

        private void drpSubClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (drpSubClass.SelectedIndex != -1)
            {
                txtAssociatedItem.Enabled = true;

                ApplyFilter();

            }
        }

        private void ApplyFilter()
        {
            if (enableFilter == false)
                return;

            var filter = "";
            
            if(drpProductGroup.SelectedIndex>0)
                filter += " and " + CN.ProductGroup + " = '" + Convert.ToString(drpProductGroup.SelectedValue) + "'";

            if (drpCategory.SelectedIndex > 0)
                filter += " and " + CN.Category + " = '" + Convert.ToString(drpCategory.SelectedValue) + "'";

            if (drpClass.SelectedIndex > 0)
                filter += " and " + CN.Class + " = '" + Convert.ToString(drpClass.SelectedValue) + "'";

            if (drpSubClass.SelectedIndex > 0)
                filter += " and " + CN.SubClass + " = '" + Convert.ToString(drpSubClass.SelectedValue) + "'";

            if (filter.Length > 5)
            {
                filter = filter.Remove(0, 4);       // remove first " and "
                dvproductAssocView.RowFilter = filter;
            }
        }
    }
}
