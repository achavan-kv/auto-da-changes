using System;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;
using STL.Common;
using STL.Common.Static;
using System.Data;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.FTransaction;
using System.Xml;
using Crownwood.Magic.Menus;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using STL.PL.Utils;

namespace STL.PL
{
    public partial class WarrantyReporting : CommonForm
    {
        private string error = "";
        public StringCollection scCategory = null;
        private StringCollection salesStaff = null;
        DataView WarrantyView = null;

        private bool staticLoaded = false;
        
        public WarrantyReporting(Form root, Form parent)
        {
            InitializeComponent();

            FormRoot = root;
            FormParent = parent;
            
            SetUpForm();
        }

        private void SetUpForm()
        {
            try
            {
                dtDateFrom.MaxDate = DateTime.Today;
                dtDateTo.MaxDate = DateTime.Today;
                dtDateTo.Value = DateTime.Today;
                dtDateFrom.Value = DateTime.Today.AddDays(-7);

                XmlUtilities xml = new XmlUtilities();
                XmlDocument dropDowns = new XmlDocument();
                dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

                if (StaticData.Tables[TN.BranchNumber] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.BranchNumber, null));

                if (dropDowns.DocumentElement.ChildNodes.Count > 0)
                {
                    DataSet ds = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out error);
                    if (error.Length > 0)
                        ShowError(error);
                    else
                    {
                        foreach (DataTable dt in ds.Tables)
                        {
                            StaticData.Tables[dt.TableName] = dt;
                        }
                    }
                }

                StringCollection scBranch = new StringCollection();
                scBranch.Add("ALL");
                scBranch.Add("ALL COURTS");
                scBranch.Add("ALL NON-COURTS");

                foreach (DataRow row in ((DataTable)StaticData.Tables[TN.BranchNumber]).Rows)
                    scBranch.Add(Convert.ToString(row[CN.BranchNo]));

                drpBranch.DataSource = scBranch;

                drpBranch.Text = Config.BranchCode;
                //drpBranch.SelectedIndex = 0;
                int branch = Convert.ToInt32(Config.BranchCode);
                if (branch == (decimal)Country[CountryParameterNames.HOBranchNo])
                    drpBranch.Enabled = true;
                else
                    drpBranch.Enabled = false;
                
                drpDates.SelectedIndex = 0;
                dynamicMenus = new Hashtable();
                HashMenus();
                ApplyRoleRestrictions();
                radioDBLive.Visible = true; // want this visible but dimmed or it looks strange. 
                salesStaff = new StringCollection();
                salesStaff.Add("Sales Staff");
                drpSalesPerson.DataSource = salesStaff;
                LoadSalesStaff();
                LoadReports();
                LoadCategories("");
                staticLoaded = true;

                //IP - 25/07/11 - CR1254 - RI - #4036
                if (Convert.ToBoolean(Country[CountryParameterNames.RIDispCatAsDept]))
                {
                    label4.Text = GetResource("T_DEPARTMENT");
                    chxCategory.Text = GetResource("T_DEPARTMENT");
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }
        private void HashMenus()
        {
            dynamicMenus = new Hashtable();
            dynamicMenus[this.Name + ":radioDBLive"] = this.radioDBLive;
        }
        private void LoadReports()
        {
            DataTable dtReport = new DataTable();
            dtReport.Columns.Add(CN.Code);
            dtReport.Columns.Add(CN.Description);

            DataRow newRow;

            newRow = dtReport.NewRow();
            newRow[CN.Code] = "SU";
            newRow[CN.Description] = "Supashield Sales";
            dtReport.Rows.Add(newRow);

            newRow = dtReport.NewRow();
            newRow[CN.Code] = "IR";
            newRow[CN.Description] = "Instant Replacement Sales";
            dtReport.Rows.Add(newRow);

            newRow = dtReport.NewRow();
            newRow[CN.Code] = "IRC"; ;
            newRow[CN.Description] = "Instant Replacement Claims";
            dtReport.Rows.Add(newRow);

            newRow = dtReport.NewRow();
            newRow[CN.Code] = "WDR"; ;
            newRow[CN.Description] = "Warranties Due For Renewal";
            dtReport.Rows.Add(newRow);

            newRow = dtReport.NewRow();
            newRow[CN.Code] = "RE"; ;
            newRow[CN.Description] = "Renewal Sales";
            dtReport.Rows.Add(newRow);

            newRow = dtReport.NewRow();
            newRow[CN.Code] = "SE"; ;
            newRow[CN.Description] = "Second Effort Solicitation Sales";
            dtReport.Rows.Add(newRow);

            newRow = dtReport.NewRow();
            newRow[CN.Code] = "RP"; ;
            newRow[CN.Description] = "Repossessions";
            dtReport.Rows.Add(newRow);

            newRow = dtReport.NewRow();
            newRow[CN.Code] = "CN"; ;
            newRow[CN.Description] = "Cancellations";
            dtReport.Rows.Add(newRow);

          /* Removed as warranty will always remain on the account for an exchange and replacement 
           * newRow = dtReport.NewRow();
            newRow[CN.Code] = "EXR"; ;
            newRow[CN.Description] = "Exchanges/Identical Replacement ";
            dtReport.Rows.Add(newRow);
            */
            newRow = dtReport.NewRow();
            newRow[CN.Code] = "MS"; ;
            newRow[CN.Description] = "Missed Sales";
            dtReport.Rows.Add(newRow);

            newRow = dtReport.NewRow();
            newRow[CN.Code] = "HR";
            newRow[CN.Description] = "Hit Rate";
            dtReport.Rows.Add(newRow);

            newRow = dtReport.NewRow();
            newRow[CN.Code] = "IC";
            newRow[CN.Description] = "Insurance Claims";
            dtReport.Rows.Add(newRow);

            newRow = dtReport.NewRow();
            newRow[CN.Code] = "LC";
            newRow[CN.Description] = "Lost Warranty Sales Commission";
            dtReport.Rows.Add(newRow);

            drpReport.DataSource = dtReport;
            drpReport.ValueMember = CN.Code;
            drpReport.DisplayMember = CN.Description;
        }

        private void enableReturns(bool enable)
        {
            //enable select returns
            grbxReturns.Enabled = enable;
            chxCancellations.Enabled = enable;
            chxRepossessions.Enabled = enable;
            //default returns to checked
            chxCancellations.Checked = enable;
            chxRepossessions.Checked = enable;
        }

        private void drpReport_SelectedIndexChanged(object sender, EventArgs e)
        {
            enableReturns(false);
            if (staticLoaded)
            {
                if (drpReport.SelectedValue.ToString() == "RP" || drpReport.SelectedValue.ToString() == "CN" || drpReport.SelectedValue.ToString() == "EXR")
                    lDate.Text = "Return Date";

                if (drpReport.SelectedValue.ToString() == "MS" || drpReport.SelectedValue.ToString() == "LC")
                    lDate.Text = "Delivery Date Of Item";

                if (drpReport.SelectedValue.ToString() == "HR")
                {
                    lDate.Text = "Delivery Date Of Warranty";
                    enableReturns(true);
                }

                if (drpReport.SelectedValue.ToString() == "IC")
                    lDate.Text = "Date Claimed Against";


                if (drpReport.SelectedValue.ToString() == "IRC")
                    lDate.Text = "Returned Date Of IR Item";

                if (drpReport.SelectedValue.ToString() == "RE")
                    lDate.Text = "Renewal Sale Date";

                if (drpReport.SelectedValue.ToString() == "WDR")
                    lDate.Text = "Items Due For Warranty Renewal";
                if (drpReport.SelectedValue.ToString() == "SU" || drpReport.SelectedValue.ToString() == "IR"
                    || drpReport.SelectedValue.ToString() == "MS")
                {
                    drpDates.Enabled = true;
                    lDate.Text = drpDates.SelectedItem.ToString();
                }
                else
                {
                    drpDates.Enabled = false;
                    if (drpReport.SelectedValue.ToString() != "LC")
                        drpDates.SelectedIndex = 2; //Warranty Delivery date
                    else
                        drpDates.SelectedIndex = 1; // Item Delivery date for lost warranty sales commission
                }

                if (drpReport.SelectedValue.ToString() == "HR") //cannot restrict category for hitrate
                {
                    drpCategory.Enabled = false;
                    drpCategory.SelectedIndex = 0; // No limitation
                }
                else
                    drpCategory.Enabled = true;
                // Warranties due for renewal should be able to look into the future for warranties about to expire
                if (drpReport.SelectedValue.ToString() == "WDR")
                {
                    dtDateFrom.MaxDate = DateTime.Today.AddMonths(1);
                    dtDateTo.MaxDate = DateTime.Today.AddMonths(6);
                    dtDateTo.Value = DateTime.Today.AddMonths(2);
                    dtDateFrom.Value = DateTime.Today.AddMonths(-1);
                }
                else
                {   dtDateFrom.MaxDate = DateTime.Today;
                    dtDateTo.MaxDate = DateTime.Today;
                }

            }
        }

        private void LoadSalesStaff()
        {
            drpSalesPerson.DataSource = null;
            salesStaff = new StringCollection();
            salesStaff.Add("Sales Staff");

            XmlUtilities xml = new XmlUtilities();
            XmlDocument dropDowns = new XmlDocument();
            dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

            dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.SalesStaff, new string[] { drpBranch.Text, "S" }));
            DataSet ds = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out Error);

            if (Error.Length > 0)
                ShowError(Error);
            else
            {
                foreach (DataTable dt in ds.Tables)
                {
                    if (dt.TableName == TN.SalesStaff)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            string str = Convert.ToString(row.ItemArray[0]) + " : " + (string)row.ItemArray[3];
                            salesStaff.Add(str.ToUpper());
                        }
                    }
                }

                drpSalesPerson.DataSource = salesStaff;
                drpSalesPerson.SelectedIndex = 0;
            }
        }

        private void drpBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            //uat375 added ALL Courts and ALL non-courts options
            //if (staticLoaded && drpBranch.SelectedIndex != 0) // don't load if ALL salesperson
            if (staticLoaded && drpBranch.SelectedIndex > 2)
                LoadSalesStaff();
        }

        private void drpCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (staticLoaded)
            {
                //if (drpCategory.SelectedIndex == 1)
                if(drpCategory.Text == "New..")
                {
                    var labelText = "Item Category";
                    if(Convert.ToBoolean(Country[CountryParameterNames.RIDispCatAsDept]))          // RI
                        labelText = "Item Department";
                    SetSelection selection = new SetSelection(labelText, 40, 200, 64, txtDummy, TN.ProductCategories, "", false);       // RI
                    selection.FormRoot = this.FormRoot;
                    selection.FormParent = this;
                    ((MainForm)this.FormRoot).AddTabPage(selection);
                }
            }
        }

        public void LoadCategories(string categoryName)
        {
            DataSet ds = new DataSet();
            ds = SetDataManager.GetSetsForTName("Productcategories", out Error);
            DataTable dt = ds.Tables[TN.SetsData];

            if (Error.Length > 0)
                ShowError(Error);
            else
            {
                // add row to allow adding new category sets
                DataRow dr = dt.NewRow();
                dr[CN.SetName] = "New..";
                dt.Rows.Add(dr);

                dr = dt.NewRow();
                dr[CN.SetName] = "ALL";
                dt.Rows.InsertAt(dr,0);

                drpCategory.DataSource = dt;
                drpCategory.DisplayMember = CN.SetName;
                drpCategory.ValueMember = CN.SetName;

            }

            //drpCategory.DataSource = scCategory;
            int i = drpCategory.FindStringExact(categoryName);
            if (i != -1)
                drpCategory.SelectedIndex = i;
        }


        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                Wait();
                ((MainForm)this.FormRoot).statusBar1.Text = "";
                dgResults.DataSource = null;
                int uselivedatabase =0; //by default we are using the test database
                if (radioDBLive.Checked == true)
                    uselivedatabase = 1;

                DataSet ds = AccountManager.WarrantySalesReport(drpReport.SelectedValue.ToString(), (string)drpBranch.SelectedItem,
                                SelectedEmpeeNo(), drpCategory.Text, Convert.ToInt16(chxCash.Checked), 
                                Convert.ToInt16(chxCredit.Checked), Convert.ToInt16(chxSpecial.Checked), dtDateFrom.Value, 
                                dtDateTo.Value, Convert.ToInt16(chxBranch.Checked), Convert.ToInt16(chxCategory.Checked), 
                                Convert.ToInt16(chxSalesPerson.Checked), Convert.ToInt16(chxAccountType.Checked),
                                (string)drpDates.SelectedItem, uselivedatabase, Convert.ToInt16(chxRepossessions.Checked),
                                Convert.ToInt16(chxCancellations.Checked), out error);
                
                if (error.Length > 0)
                    ShowError(error);
                else
                {
                    if(ds != null)
                    {
                        DataTable dt = ds.Tables[TN.Warranties];
                        DataGridTableStyle tabStyle = new DataGridTableStyle();
                        tabStyle.MappingName = dt.TableName;
                        dgResults.DataSource = dt;
                        dgResults.ReadOnly = true;
                        dgResults.AllowUserToAddRows = false;
                        WarrantyView = new DataView(dt);

                       // dgResults.AutoSize = true;

                        
                        
                        
                        
                         dgResults.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.ColumnHeader);


                         if (drpReport.SelectedValue.ToString() == "HR")
                         {
                             if (!chxCancellations.Checked)
                             {
                                 dgResults.Columns[CN.ValueCancelled].Visible = false;
                                 dgResults.Columns[CN.NoCancelled].Visible = false;
                             }

                             if (!chxRepossessions.Checked)
                             {
                                 dgResults.Columns[CN.ValueRepossession].Visible = false;
                                 dgResults.Columns[CN.NoRepossessed].Visible = false;
                             }
                         }

                         if (chxAccountType.Checked || drpReport.SelectedValue.ToString() == "HR" || drpReport.SelectedValue.ToString() == "RP")
                        {
                            dgResults.Columns[CN.AccountType2].Width = 70;
                            dgResults.Columns[CN.AccountType2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                        }
                         if (chxBranch.Checked || drpReport.SelectedValue.ToString() == "HR" || drpReport.SelectedValue.ToString() == "RP" || drpReport.SelectedValue.ToString() == "CN"
                             || drpReport.SelectedValue.ToString() == "IC" || drpReport.SelectedValue.ToString() == "MS" || drpReport.SelectedValue.ToString() == "IRC"
                             || drpReport.SelectedValue.ToString() == "WDR" || drpReport.SelectedValue.ToString() == "SE" || drpReport.SelectedValue.ToString() == "IR"
                             || drpReport.SelectedValue.ToString() == "SU")
                        {
                            dgResults.Columns[CN.BranchNumber].Width = 50;
                            dgResults.Columns[CN.BranchNumber].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        }
                        if (drpReport.SelectedValue.ToString() != "CN" && drpReport.SelectedValue.ToString() != "RE" && drpReport.SelectedValue.ToString() != "SU"
                            && drpReport.SelectedValue.ToString() != "IR" && drpReport.SelectedValue.ToString() != "IRC" && drpReport.SelectedValue.ToString() != "WDR"
                            && drpReport.SelectedValue.ToString() != "SE" && drpReport.SelectedValue.ToString() != "MS" && drpReport.SelectedValue.ToString() != "HR"
                            && drpReport.SelectedValue.ToString() != "IC" && drpReport.SelectedValue.ToString() != "LC")
                        {
                            dgResults.Columns[CN.CustomerDebitAmount].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            dgResults.Columns[CN.RepossessionDate].Width = 80;
                        }
                        if (drpReport.SelectedValue.ToString() != "RE" && drpReport.SelectedValue.ToString() != "SU" && drpReport.SelectedValue.ToString() != "IR"
                            && drpReport.SelectedValue.ToString() != "IRC" && drpReport.SelectedValue.ToString() != "WDR" && drpReport.SelectedValue.ToString() != "SE"
                            && drpReport.SelectedValue.ToString() != "MS" && drpReport.SelectedValue.ToString() != "HR" && drpReport.SelectedValue.ToString() != "IC"
                            && drpReport.SelectedValue.ToString() != "LC")
                        {
                            dgResults.Columns[CN.WarrantyReturnCode].Width = 60;
                            dgResults.Columns[CN.WarrantyReturnLocation].Width = 60;
                            dgResults.Columns[CN.WarrantyReturnLocation].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            dgResults.Columns[CN.AIGClaim].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            dgResults.Columns[CN.Rebatepcent].Width = 70;
                            dgResults.Columns[CN.Rebatepcent].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;                        
                        
                        }
                        if (drpReport.SelectedValue.ToString() != "IRC" && drpReport.SelectedValue.ToString() != "HR" && drpReport.SelectedValue.ToString() != "LC")
                        {
                            dgResults.Columns[CN.Product_Code].Width = 80;
                            dgResults.Columns[CN.ValueOfProduct].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            dgResults.Columns[CN.ProductQuantity].Width = 60;
                            dgResults.Columns[CN.ProductQuantity].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                            //IP - 08/07/11 - CR1254 - RI
                            dgResults.Columns[CN.Courts_Code].Width = 50;
                            dgResults.Columns[CN.Courts_Code].Visible = Convert.ToBoolean(Country[CountryParameterNames.RIDispCourtsCode]);

                        }
                        if (drpReport.SelectedValue.ToString() != "HR" && drpReport.SelectedValue.ToString() != "LC")
                        {
                            dgResults.Columns[CN.WarrantyCode].Width = 80;
                            dgResults.Columns[CN.WarrantyCostPrice].Width = 70;
                            dgResults.Columns[CN.WarrantyCostPrice].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            dgResults.Columns[CN.WarrantyRetailValue].Width = 70;
                            dgResults.Columns[CN.WarrantyRetailValue].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            dgResults.Columns[CN.ProfitMargin].Width = 60;
                            dgResults.Columns[CN.ProfitMargin].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            dgResults.Columns[CN.AdminFee].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            dgResults.Columns[CN.Premium].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                            //IP - 08/07/11 - CR1254 - RI
                            dgResults.Columns[CN.Warranty_Courts_Code].Width = 60;
                            dgResults.Columns[CN.Warranty_Courts_Code].Visible = Convert.ToBoolean(Country[CountryParameterNames.RIDispCourtsCode]);
                        
                        
                        }
                        if (drpReport.SelectedValue.ToString() == "HR")
                        {
                            if(chxBranch.Checked ||chxCategory.Checked || chxSalesPerson.Checked || chxAccountType.Checked)
                            {
                                dgResults.Columns[CN.Total].Width = 80;
                            }
                            dgResults.Columns[CN.WarrantyCost].Width = 70;
                            dgResults.Columns[CN.WarrantyCost].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            dgResults.Columns[CN.ActualWarrantySales].Width = 70;
                            dgResults.Columns[CN.ActualWarrantySales].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            dgResults.Columns[CN.TotalSales].Width = 70;
                            dgResults.Columns[CN.TotalSales].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            dgResults.Columns[CN.NoOfWarrantySales].Width = 55;
                            dgResults.Columns[CN.NoOfWarrantySales].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;                            
                            dgResults.Columns[CN.MaxNoWarrantableSales].Width = 70;
                            dgResults.Columns[CN.MaxNoWarrantableSales].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            dgResults.Columns[CN.MaxValueWarrantableSales].Width = 70;
                            dgResults.Columns[CN.MaxValueWarrantableSales].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            dgResults.Columns[CN.HitRate].Width = 70;
                            dgResults.Columns[CN.HitRate].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                                                       
                        }
                        if (drpReport.SelectedValue.ToString() != "LC")
                        {

                        }
                        if (drpReport.SelectedValue.ToString() == "LC")
                        {
                            dgResults.Columns[CN.Attachmentpct].Width = 70;
                            dgResults.Columns[CN.Attachmentpct].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            dgResults.Columns[CN.MaxNoWarrantableSales].Width = 70;
                            dgResults.Columns[CN.MaxNoWarrantableSales].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            dgResults.Columns[CN.MaxWarrantySalesValue].Width = 70;
                            dgResults.Columns[CN.MaxWarrantySalesValue].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            dgResults.Columns[CN.ActualWarrantySalesValue].Width = 70;
                            dgResults.Columns[CN.ActualWarrantySalesValue].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            dgResults.Columns[CN.LostCommission].Width = 70;                             
                            dgResults.Columns[CN.LostCommission].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            dgResults.Columns[CN.WarrantiesSold].Width = 60;        //UAT219 jec
                            dgResults.Columns[CN.WarrantiesSold].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                            //IP - 13/07/11 - CR1254 - RI
                            dgResults.Columns[CN.Warranty_Courts_Code].Width = 60;
                            dgResults.Columns[CN.Warranty_Courts_Code].Visible = Convert.ToBoolean(Country[CountryParameterNames.RIDispCourtsCode]);

                            foreach (DataRow dr in WarrantyView.Table.Rows)     //UAT219 jec
                            {
                                dr[CN.MaxWarrantySalesValue] = Math.Round(Convert.ToDecimal(dr[CN.MaxWarrantySalesValue]), 2);
                                dr[CN.ActualWarrantySalesValue] = Math.Round(Convert.ToDecimal(dr[CN.ActualWarrantySalesValue]), 2);
                                dr[CN.LostCommission] = Math.Round(Convert.ToDecimal(dr[CN.LostCommission]), 2);
                                dr[CN.RequiredSalesValue] = Math.Round(Convert.ToDecimal(dr[CN.RequiredSalesValue]), 2);
                                dr[CN.LostSalesValue] = Math.Round(Convert.ToDecimal(dr[CN.LostSalesValue]), 2);

                            }
                        }

                        //IP - 25/05/11 - CR1254 - RI - #4036
                        if (Convert.ToBoolean(Country[CountryParameterNames.RIDispCatAsDept]))
                        {
                            if (dgResults.Columns.Contains("Product Category"))
                            {
                                dgResults.Columns[CN.Product_Category].HeaderText = GetResource("T_DEPARTMENT");
                            }
                        }
                        //tabStyle.GridColumnStyles["Branch Number"].Width = 49;
                        
                        btnExcel.Enabled = (dt.Rows.Count > 0);

                        ((MainForm)this.FormRoot).statusBar1.Text = dt.Rows.Count.ToString() + GetResource("M_ROWSRETURNED");
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

        private string SelectedEmpeeNo()
        {
            string empeeNo = "ALL";
            if (drpSalesPerson.DataSource != null && drpSalesPerson.SelectedIndex > 0)
            {
                int index = ((string)drpSalesPerson.SelectedItem).IndexOf(":");
                string empeeNoStr = ((string)drpSalesPerson.SelectedItem).Substring(0, index - 1);
                empeeNo = empeeNoStr;
            }
            return empeeNo;
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            try
            {
                string path = ReportUtils.CreateCSVFile(dgResults, "Save Report to Excel");

                if (path.Length.Equals(0))
                {
                    MessageBox.Show("Save Failed");
                }
                else if (File.Exists(path))
                {
                    try
                    {
                        ReportUtils.OpenExcelCSV(path);
                    }
                    catch { }  
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ((MainForm)this.FormRoot).statusBar1.Text = "";
            dgResults.DataSource = null;
            staticLoaded = false;
            SetUpForm();
        }

        private void drpDates_SelectedIndexChanged(object sender, EventArgs e)
        {
           if (drpReport.DataSource != null)
            if (drpReport.SelectedValue.ToString() == "IR" || drpReport.SelectedValue.ToString() == "SE"
                || drpReport.SelectedValue.ToString() == "SU" || drpReport.SelectedValue.ToString() == "MS")
                lDate.Text = drpDates.SelectedItem.ToString();
        }
    }
}
