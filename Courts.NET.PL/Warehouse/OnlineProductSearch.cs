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
using System.Collections.Specialized;
//using STL.PL.WS1;
using STL.PL.WS2;
using STL.PL.WSStock;

namespace STL.PL.Warehouse
{
    public partial class OnlineProductSearch : CommonForm
    {
        private DataTable categories;
        private DataSet byLocation;
        private DataTable location;
        private DataTable online;
        private string locationx="";
        private short category;
        private DateTime dateAdded;
        private DateTime dateRemoved;
        private string showOnline;
        private string productDesc = "";
        private DateTime nullDate = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
        private BindingSource boundStockList;       // #14491

        //public OnlineProductSearch(Form root, Form parent)
        public OnlineProductSearch()
        {
            InitializeComponent();

            location = new DataTable();
            DataColumn dc = new DataColumn("Text", typeof(string));
            location.Columns.Add(dc);
            DataColumn dc2 = new DataColumn("DistCtr", typeof(string));
            location.Columns.Add(dc2);

            DataRow Row = location.NewRow();
            //DataRow blankRow = location.NewRow();
            Row["Text"] = "DC Only";
            Row["DistCtr"] = "DC";
            location.Rows.Add(Row);
            DataRow Row2 = location.NewRow();
            Row2["Text"] = "All Branches";
            Row2["DistCtr"] = "A";
            location.Rows.Add(Row2);
            DataRow Row6 = location.NewRow();
            Row6["Text"] = "Both";
            Row6["DistCtr"] = "B";
            location.Rows.Add(Row6);

            DataView dvlocationView = new DataView(location);
            drpLocation.DataSource = dvlocationView;
            drpLocation.ValueMember = "DistCtr";
            drpLocation.DisplayMember = "Text";

            online = new DataTable();
            DataColumn dc3 = new DataColumn("Text", typeof(string));
            online.Columns.Add(dc3);
            DataColumn dc4= new DataColumn("Online", typeof(string));
            online.Columns.Add(dc4);

            DataRow Row3 = online.NewRow();
            Row3["Text"] = "All";
            Row3["Online"] = "A";
            online.Rows.Add(Row3);
            DataRow Row5 = online.NewRow();
            Row5["Text"] = "No";
            Row5["Online"] = "N";
            online.Rows.Add(Row5);
            DataRow Row4 = online.NewRow();
            Row4["Text"] = "Yes";
            Row4["Online"] = "Y";
            online.Rows.Add(Row4);
            

            DataView dvonlineView = new DataView(online);
            drpOnline.DataSource = dvonlineView;
            drpOnline.ValueMember = "Online";
            drpOnline.DisplayMember = "Text";

            getCategories();

            btnClear_Click(null,null);

            dtpDateAdded.Enabled=cbDateAdded.Checked;
            dtpDateRemoved.Enabled = cbDateRemoved.Checked;

            drpLocation.Enabled = false;
            lblLocation.Enabled = false;
            drpLocation.SelectedValue = "B";
        }
        

        private void getCategories()
        {
            categories = AccountManager.GetCategories(out Error);
            // Add a blank entry at the start of list            
            DataRow blankRow = categories.NewRow();
            blankRow[CN.Code] = "00";
            blankRow[CN.Category] = "All Categories";
            categories.Rows.InsertAt(blankRow, 0);

            DataView dvcategoriesView = new DataView(categories);
            //foreach (DataRowView rowView in dvcategoriesView)
            //{
            //    if (Convert.ToString(rowView["category"]).Contains("Discount") || Convert.ToString(rowView["category"]).Contains("Second Hand")
            //            //|| Convert.ToString(rowView["category"]).Contains("Affinity") 
            //            || Convert.ToString(rowView["category"]).Contains("Spare Part")
            //            || Convert.ToString(rowView["category"]).Contains("Category")) 
            //    {
            //        rowView.Delete();
            //    }
            //}
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
            drpItemCategory.SelectedIndex = 0;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                Function = "BAccountManager::GetStockByLocation()";
                Wait();

                cbDCmarkAll.Checked = false;
                cbMarkAll.Checked = false;

                gpUpdate.Enabled = false; 

                if(drpLocation.Enabled==true)
                {
                    locationx = Convert.ToString(drpLocation.SelectedValue);
                }
                else
                {
                    locationx="B";
                }

                category = Convert.ToInt16(drpItemCategory.SelectedValue);

                showOnline = Convert.ToString(drpOnline.SelectedValue);
               
                productDesc = txtProdDesc.Text;  //Enclosed by double quotes for FTS Fulltext function
                //limit = chxLimit.Checked;

                if (cbDateAdded.Checked)
                {
                    dateAdded=dtpDateAdded.Value;
                }
                else
                {
                    dateAdded = nullDate;
                }
                if (cbDateRemoved.Checked)
                {
                    dateRemoved = dtpDateRemoved.Value;
                }
                else
                {
                    dateRemoved = nullDate;
                }

                
                SearchThread();

                if (byLocation != null)
                {
                    boundStockList = new BindingSource();           // #14491
                    boundStockList.DataSource = byLocation.Tables["ByLocation"];
                    dgProductDetails.DataSource = boundStockList;

                    ////dgProductDetails.DataSource = byLocation.Tables["ByLocation"];
                    //dgProductDetails.DataMember = "ByLocation";
                    SetGridTableStyle();
                    //if (locationx=="DC")
                    //{
                    //    cbDCmarkAll.Checked=true;
                    //}
                    //if (showOnline == "Y")
                    //{
                    //    cbMarkAll.Checked = true;
                    //}

                    if (byLocation.Tables["ByLocation"].Rows.Count > 0)
                    {
                        gpUpdate.Enabled = true;
                    }                  

                    ((MainForm)this.FormRoot).StatusBarText = String.Format("{0} rows returned", byLocation.Tables["ByLocation"].Rows.Count);

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

        private void SearchThread()
        {
            try
            {
                Wait();
                Function = "SearchThread";

                byLocation = AccountManager.OnlineProductSearch(new OnlineProductSearchRequest
                {
                    Location = locationx,
                    Category = category,   //Convert.ToInt16(Config.BranchCode),
                    Online = showOnline,
                    DateAdded = dateAdded,
                    DateRemoved = dateRemoved,
                    ProductDesction = productDesc,
                    Limit = false
                }, out Error);

                if (Error.Length > 0)
                    ShowError(Error);
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
                Function = "End of SearchThread";
            }
        }

        private void cbDateAdded_CheckedChanged(object sender, EventArgs e)
        {
            dtpDateAdded.Enabled = cbDateAdded.Checked;
            dgProductDetails.DataSource = null;
        }

        private void cbDateRemoved_CheckedChanged(object sender, EventArgs e)
        {
            dtpDateRemoved.Enabled = cbDateRemoved.Checked;
            dgProductDetails.DataSource = null;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            StockManager.UpdateOnlineProducts(byLocation.Tables["ByLocation"]);
            btnSearch_Click(null,null);
            ((MainForm)this.FormRoot).StatusBarText = "Product details Online status details updated";
        }

        private void cbMarkAll_CheckedChanged(object sender, EventArgs e)
        {
            DataView productView = new DataView(byLocation.Tables["ByLocation"]);
            foreach (DataRowView rowView in productView)
            {
                rowView["Online"]= Convert.ToBoolean(cbMarkAll.Checked);
                
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            drpLocation.SelectedValue = "B";
            lblLocation.Enabled = false;
            drpItemCategory.SelectedIndex=0;
            drpOnline.SelectedIndex=0;
            cbDateAdded.Checked=false;
            cbDateRemoved.Checked=false;
            txtProdDesc.Text="";
            resetScreen();
        }

        private void resetScreen()
        {
            dgProductDetails.DataSource = null;
            gpUpdate.Enabled = false;
            cbMarkAll.Checked = false;
            cbDCmarkAll.Checked = false;
        }


        private void btnExit_Click(object sender, EventArgs e)
        {
            // exit screen
            CloseTab();
        }

        private void SetGridTableStyle()
        {
            dgProductDetails.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgProductDetails.Columns["Online"].Width = 55;
            dgProductDetails.Columns["Online"].HeaderText = "Available Online";            
            dgProductDetails.Columns["Product Code"].Width = 70;
            dgProductDetails.Columns["Product Code"].ReadOnly = true;
            //dgProductDetails.Columns["Product Type"].Width = 60;
            dgProductDetails.Columns["Product Description 1"].Width = 130;
            dgProductDetails.Columns["Product Description 1"].ReadOnly = true;
            dgProductDetails.Columns["Product Description 2"].Width = 130;
            dgProductDetails.Columns["Product Description 2"].ReadOnly = true;
            dgProductDetails.Columns["Cash Price"].Width = 70;
            dgProductDetails.Columns["Cash Price"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgProductDetails.Columns["Cash Price"].ReadOnly = true;
            //dgProductDetails.Columns["Weekly Price"].Width = 70;
            //dgProductDetails.Columns["Weekly Price"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgProductDetails.Columns["Cash Promotional Price"].Width = 70;
            dgProductDetails.Columns["Cash Promotional Price"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgProductDetails.Columns["Cash Promotional Price"].ReadOnly = true;
            dgProductDetails.Columns["Date Promo Starts"].Width = 70;
            dgProductDetails.Columns["Date Promo Starts"].ReadOnly = true;
            dgProductDetails.Columns["Date Promo Ends"].Width = 70;
            dgProductDetails.Columns["Date Promo Ends"].ReadOnly = true;
            dgProductDetails.Columns["DC Only"].Width = 60;
            dgProductDetails.Columns["DC Only"].HeaderText = "Stock Qty from DC only";
            //dgProductDetails.Columns["DC Only"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgProductDetails.Columns["Available Stock"].Width = 55;
            dgProductDetails.Columns["Available Stock"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgProductDetails.Columns["Available Stock"].ReadOnly = true;
            dgProductDetails.Columns["Date Added"].Width = 70;
            dgProductDetails.Columns["Date Added"].ReadOnly = true;
            dgProductDetails.Columns["Date Removed"].Width = 70;
            dgProductDetails.Columns["Date Removed"].ReadOnly = true;
            dgProductDetails.Columns["ItemId"].Visible = false;
            dgProductDetails.Columns["StkDC"].Visible = false;
            dgProductDetails.Columns["StkAll"].Visible = false;
        }

        private void btnOnlineProductsExcel_Click(object sender, EventArgs e)
        {
            SaveExcel(dgProductDetails);
        }

        private void SaveExcel(DataGridView dg)
        {
            string filePath = STL.PL.Utils.ReportUtils.CreateCSVFile(dg, "Save Online Product Search to Excel");

            if (filePath.Length.Equals(0))
                MessageBox.Show("Save Failed");

            try
            {
                STL.PL.Utils.ReportUtils.OpenExcelCSV(filePath);
            }
            catch { }
        }

        private void cbDCmarkAll_CheckedChanged(object sender, EventArgs e)
        {
            DataView productView = new DataView(byLocation.Tables["ByLocation"]);
            foreach (DataRowView rowView in productView)
            {
                rowView["DC Only"] = Convert.ToBoolean(cbDCmarkAll.Checked);
                if (Convert.ToBoolean(rowView["DC Only"]) == true)
                {
                    rowView["Available Stock"] = rowView["StkDC"];
                }
                else
                {
                    rowView["Available Stock"] = rowView["StkAll"];
                }
            }
        }      

        private void drpOnline_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Convert.ToString(drpOnline.SelectedValue)!="Y")
            {
                drpLocation.Enabled=false;
                lblLocation.Enabled = false;
                drpLocation.SelectedValue="B";
            }
            else
            {
                drpLocation.Enabled = true;
                lblLocation.Enabled = true;
            }
            dgProductDetails.DataSource = null;
        }


        private void dgProductDetails_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                Wait();

                var curRow = dgProductDetails.CurrentRow.Index;

                if (Convert.ToBoolean(((DataRowView)boundStockList[curRow]).Row["DC Only"]) == true)        // #14491
                {
                    ((DataRowView)boundStockList[curRow]).Row["Available Stock"] = ((DataRowView)boundStockList[curRow]).Row["StkDC"];
                }
                else
                {
                    ((DataRowView)boundStockList[curRow]).Row["Available Stock"] = ((DataRowView)boundStockList[curRow]).Row["StkAll"];
                }

               // if (Convert.ToBoolean(((DataTable)(dgProductDetails.DataSource)).Rows[curRow]["DC Only"]) == true)
                //{
                //    ((DataTable)(dgProductDetails.DataSource)).Rows[curRow]["Available Stock"]=((DataTable)(dgProductDetails.DataSource)).Rows[curRow]["StkDC"];
                //}
                //else
                //{
                //    ((DataTable)(dgProductDetails.DataSource)).Rows[curRow]["Available Stock"] = ((DataTable)(dgProductDetails.DataSource)).Rows[curRow]["StkAll"];
                //}

                dgProductDetails.Refresh();                
            }
            catch
            {
            }
            finally
            {
                StopWait();
            }
        }
        
        private void dgProductDetails_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            //if (dgProductDetails.IsCurrentCellDirty)
            //{
            //try
            //{
            //    if (dgProductDetails.CurrentCell is DataGridViewCheckBoxCell && dgProductDetails.CurrentCell.OwningColumn.Name == "DC Only")
            //    {
                    dgProductDetails.CommitEdit(DataGridViewDataErrorContexts.Commit);
            //    }
            //}
            //catch
            //{
            //}

        }

        private void drpItemCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            resetScreen();
        }

        private void txtProdDesc_TextChanged(object sender, EventArgs e)
        {
            resetScreen();
        }

        private void drpLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            resetScreen();
        }

        private void dtpDateAdded_ValueChanged(object sender, EventArgs e)
        {
            resetScreen();
        }

        private void dtpDateRemoved_ValueChanged(object sender, EventArgs e)
        {
            resetScreen();
        }

        
    }
}
