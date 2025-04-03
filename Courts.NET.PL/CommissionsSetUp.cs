using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
//using STL.PL.WS5;
using STL.PL.WS8;
using System.Windows.Forms;
using STL.Common.Constants;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.Tags;
using System.Xml;
using System.Web.Services.Protocols;
using STL.Common;
using STL.Common.Static;
using System.Collections;
using STL.PL.Utils;
using System.Collections.Specialized;       //CR1035



namespace STL.PL
{
    public partial class CommissionsSetUp : CommonForm
    {
        private string _errorTxt = "";
        private string commItemStr = "";        
        private string spiffTypeStr = "";        
        private DateTime deletedComm = new System.DateTime(1999, 1, 1, 00, 0, 00, 000);
        private DateTime date2050 = new System.DateTime(2050, 1, 1, 00, 0, 00, 000);

        private DateTime _serverDate;
       // public string commItemStr;
        private string user = Credential.UserId.ToString();
        private int maxCommRate = 0;
        private int maxSpiffValue = 0;
        private bool commPerAccType = false;
        private bool SpiffPerBranch = false; 

        public CommissionsSetUp()
        {
            InitializeComponent();
           // maxCommRate = Convert.ToInt32(Country[CountryParameterNames.MaxCommRate]);
           // maxSpiffValue = Convert.ToInt32(Country[CountryParameterNames.MaxSpiffValue]);
           // commPerAccType = Convert.ToBoolean(Country[CountryParameterNames.ComPerAccType]);
            
        }

        // Added for invoke from MainForm
        public CommissionsSetUp(Form root, Form parent)
        {
            FormRoot = root;
            FormParent = parent;

            InitializeComponent();

            maxCommRate = Convert.ToInt32(Country[CountryParameterNames.MaxCommRate]);
            maxSpiffValue = Convert.ToInt32(Country[CountryParameterNames.MaxSpiffValue]);
            commPerAccType = Convert.ToBoolean(Country[CountryParameterNames.ComPerAccType]);
            SpiffPerBranch = Convert.ToBoolean(Country[CountryParameterNames.SpiffPerBranch]);
            ClearEntryFields();
                      
            HashMenus();
            ApplyRoleRestrictions();
            // load Branches
            StringCollection branchNos = new StringCollection();
            branchNos.Add("All");       //CR1035
            foreach (DataRow row in ((DataTable)StaticData.Tables[TN.BranchNumber]).Rows)
            {
                branchNos.Add(Convert.ToString(row["branchno"]));
            }

            drpSpiffBranch.DataSource = branchNos;
            //default to All
            int x = drpSpiffBranch.FindString("All");
            if (x != -1)
                drpSpiffBranch.SelectedIndex = x;

            // disable Save buttons
            btnSaveComm.Enabled = false;
            btnSaveSpiff.Enabled = false;
            // Initial Load
            drpCommItem.SelectedIndex = 0;  // Product Category
            drpSpiffType.SelectedIndex = 0;  // Single Spiffs
            LoadCommissions();
            

        }

        private void HashMenus()
        {
            dynamicMenus = new Hashtable();
            dynamicMenus[this.Name + ":btnSaveSpiff"] = this.btnSaveSpiff;
            dynamicMenus[this.Name + ":btnEnterSpiff"] = this.btnEnterSpiff;
            dynamicMenus[this.Name + ":btnDeleteSpiff"] = this.btnDeleteSpiff;
        }


        private void tcMain_SelectionChanged(object sender, EventArgs e)
        {
            if (tbCommissions.Selected)
            {
                drpSpiffType.SelectedIndex = 0;  // Single Spiffs 
                //default to All for Commissions
                int x = drpSpiffBranch.FindString("All");       //CR1035
                if (x != -1)
                    drpSpiffBranch.SelectedIndex = x;
            }
            if (tbSpiffs.Selected)
            {                
                // user not allowed to maintain - hide entry fields
                if (btnEnterSpiff.Visible == false)
                {
                    groupBox3.Visible = false;
                    dgvSpiffs.Height = 350;
                }

            }
            //  Clear Status message;
            ((MainForm)this.FormRoot).statusBar1.Text = "";
        }

        private void btn_Reload_click(object sender, EventArgs e)
        {
            if (!CheckForSave())
            {
                LoadCommissions();
            }
        }

        private void LoadCommissions()
        {
            ClearEntryFields();
            // disable save button until change made
            btnSaveComm.Enabled = false;
            //enable clear
            btnClear.Enabled = true;       // jec 20/11/07

            dgvCommissions.DataSource = null;
            //dgvCommissions.TableStyles.Clear();
            commItemStr = (string)drpCommItem.SelectedItem;
            DateTime selectDate = dtpCommDate.Value;
            commItemLabel.Text = commItemStr;
            // show spiff Branch items if country parameter true  CR1035
            drpSpiffBranch.Visible = SpiffPerBranch;
            lblSpiffBranch.Visible = SpiffPerBranch;
            tbSpiffBranch.Visible = SpiffPerBranch;
            lblSpiffBrn.Visible = SpiffPerBranch;
            tbSpiffBranch.Enabled = false;
             
            SetScreen();

           if (commItemStr != null)
            {
                //string commTypeStr = commItemStr;
                DataSet ds = PaymentManager.GetSalesCommissionRates(commItemStr, selectDate, out _errorTxt);
                DataView dvCommissions = ds.Tables[TN.SalesCommissionRates].DefaultView;
                
                dgvCommissions.DataSource = dvCommissions;

                dgvCommissions.ColumnHeadersVisible = true;
                dgvCommissions.AutoGenerateColumns = true;   //was false; 28/09/06
                //dgvCommissions.Columns[CN.ItemText].HeaderText = GetResource("T_Category");
                dgvCommissions.Columns[CN.ItemText].HeaderText = commItemStr;
                dgvCommissions.Columns[CN.ItemText].Width = 65;
                dgvCommissions.Columns[CN.DateFrom].HeaderText = GetResource("T_DATEFROM");
                dgvCommissions.Columns[CN.DateTo].HeaderText = GetResource("T_DATETO");
                dgvCommissions.Columns[CN.DateFrom].Width = 70;
                dgvCommissions.Columns[CN.DateTo].Width = 70;
                dgvCommissions.Columns[CN.Percentage].Width = 65;
                dgvCommissions.Columns[CN.Percentage].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvCommissions.Columns[CN.Branch].Width = 45;           //CR1035
                dgvCommissions.Columns[CN.Branch].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvCommissions.Columns[CN.Branch].Visible = false;

                if (commPerAccType == true && commItemStr != "Terms Type")
                {
                    dgvCommissions.Columns[CN.Percentage].HeaderText = GetResource("T_CreditPct");
                    dgvCommissions.Columns[CN.PercentageCash].HeaderText = GetResource("T_CashPct");
                    // Set visibility of Cash percentage column based on Country parameter
                    dgvCommissions.Columns[CN.PercentageCash].Visible = commPerAccType;
                    dgvCommissions.Columns[CN.PercentageCash].Width = 65;
                    dgvCommissions.Columns[CN.PercentageCash].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    // set visibility of Cash percentage text box,checkbox & labels
                    tbCommPcentCash.Visible = true;
                    cbCopyCredit.Visible = true;
                    Cash_Label.Visible = true;
                    Credit_Label.Visible = true;
                }
                else                    
                {
                    dgvCommissions.Columns[CN.Percentage].HeaderText = GetResource("T_Percentage");
                    dgvCommissions.Columns[CN.PercentageCash].Visible = false;
                    if (commItemStr == "Terms Type")
                    {
                        dgvCommissions.Columns[CN.Value].Width = 65;
                        dgvCommissions.Columns[CN.Value].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    }
                    // set visibility of Cash percentage text box,checkbox & labels
                    tbCommPcentCash.Visible = false;
                    cbCopyCredit.Visible = false;
                    Cash_Label.Visible = false;
                    Credit_Label.Visible = false;
                }
                
                ((MainForm)this.FormRoot).statusBar1.Text = " ";
                tbCommItem.Enabled = true;
                tbCommPcent.Enabled = true;
                tbCommPcentCash.Enabled = true;
                tbCommValue.Enabled = true;
               
            }
        }

        private void ClearEntryFields()
        {
            this._serverDate = StaticDataManager.GetServerDate();
            //Commission Screen
            tbCommItem.Text = "";
            tbCommPcent.Text = "";
            tbCommPcentCash.Text = "";
            tbCommValue.Text = "";
            dtpCommDateFrom.Value = _serverDate.AddDays(1);
            dtpCommDateTo.Value = new System.DateTime(2050, 1, 1, 00, 0, 00, 000);
            dtpCommDateTo.Enabled = false;
            tbCommItem.Enabled = true;
            dtpCommDateFrom.Enabled = true;
            tbCommPcent.Enabled = true;
            tbCommPcentCash.Enabled = true;
            tbCommValue.Enabled = true;
            //errorProvider1.SetError(this.dtpCommDateFrom, "");
            //errorProvider1.SetError(this.dtpCommDateTo, "");
            //errorProvider1.SetError(this.tbCommItem, "");
            //errorProvider1.SetError(this.tbCommValue, "");
            //errorProvider1.SetError(this.tbCommPcent, "");

            btnEnterComm.Enabled = false;
            btnDeleteComm.Enabled = false;
            
            //Spiff Screen
            tbSpiffProduct.Text = "";
            tbSpiffPcent.Text = "";
            tbSpiffValue.Text = "";
            dtpSpiffDateFrom.Value = _serverDate.AddDays(1);
            dtpSpiffDateTo.Value = new System.DateTime(2050, 1, 1, 00, 0, 00, 000);
            dtpSpiffDateTo.Enabled = false;
            tbSpiffProduct.Enabled = true;
            tbSpiffPcent.Enabled = true;
            tbSpiffValue.Enabled = true;
            dtpSpiffDateFrom.Enabled = true;
            tbSpiffItem2.Enabled = true;
            tbSpiffItem3.Enabled = true;
            tbSpiffItem4.Enabled = true;
            tbSpiffItem5.Enabled = true;
            tbSpiffDescription.Enabled = true;
            tbSpiffItem2.Text = "";
            tbSpiffItem3.Text = "";
            tbSpiffItem4.Text = "";
            tbSpiffItem5.Text = "";
            tbSpiffDescription.Text = "";   //CR1035
            tbSpiffBranch.Text = drpSpiffBranch.Text;

            btnEnterSpiff.Enabled = false;
            btnDeleteSpiff.Enabled = false;

            ClearErrors();
                                
        }

        
        private void dgvCommissionsRowSelected_click(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                Function = "dgvCommissionsRowSelected_click";
                Wait();

                ClearEntryFields();

                btnEnterComm.Enabled = true;
                btnDeleteComm.Enabled = true;
                dtpCommDateTo.Enabled = false;
                tbCommItem.Enabled = false;

                int index = dgvCommissions.CurrentRow.Index;
                if (index >= 0 && commItemStr != "Terms Type")
                {
                    tbCommItem.Text = (string)((DataView)dgvCommissions.DataSource)[index][CN.ItemText];
                    tbCommPcent.Text = Convert.ToString(((DataView)dgvCommissions.DataSource)[index][CN.Percentage]);
                    tbCommPcentCash.Text = Convert.ToString(((DataView)dgvCommissions.DataSource)[index][CN.PercentageCash]);
                    dtpCommDateFrom.Value = (DateTime)((DataView)dgvCommissions.DataSource)[index][CN.DateFrom];
                    dtpCommDateTo.Value = (DateTime)((DataView)dgvCommissions.DataSource)[index][CN.DateTo];

                }
                else
                    // Terms Type only
                {
                    tbCommItem.Text = (string)((DataView)dgvCommissions.DataSource)[index][CN.ItemText];
                    tbCommPcent.Text = Convert.ToString(((DataView)dgvCommissions.DataSource)[index][CN.Percentage]);
                    tbCommPcentCash.Text = Convert.ToString(((DataView)dgvCommissions.DataSource)[index][CN.PercentageCash]);
                    tbCommValue.Text = Convert.ToString(((DataView)dgvCommissions.DataSource)[index][CN.Value]);
                    dtpCommDateFrom.Value = (DateTime)((DataView)dgvCommissions.DataSource)[index][CN.DateFrom];
                    dtpCommDateTo.Value = (DateTime)((DataView)dgvCommissions.DataSource)[index][CN.DateTo];
                }
                
                //  Clear Status message;
                if (dtpCommDateTo.Value == date2050)
                    ((MainForm)this.FormRoot).statusBar1.Text = GetResource("M_CHANGEDATE");   //"Change Date From to add new commission rate";
                else
                    ((MainForm)this.FormRoot).statusBar1.Text = GetResource("M_NEWRATE");      //"A subsequent commission rate has been added..."
                // rate ends no changes allowed - add a new rate
                if (dtpCommDateTo.Value <= _serverDate)
                {
                    tbCommPcent.Enabled = false;
                    tbCommPcentCash.Enabled = false;
                    tbCommValue.Enabled = false;
                    dtpCommDateFrom.Enabled = false;
                    dtpCommDateTo.Enabled = false;
                    ((MainForm)this.FormRoot).statusBar1.Text = GetResource("M_RATEENDED");    //"Commission rate ended - no changes allowed";
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

        private void btnEnterComm_click(object sender, EventArgs e)
        {
            try
            {
                Function = "btnEnterComm_click";

                //validate entry
                if (ValidCommission())

                {
                    //int newindex = dgvCommissions.NewRowIndex;

                    // Check whether to replace a row in edit for the same currency
                    DataView currentView = (DataView)dgvCommissions.DataSource;
                    DataRowView rowViewFound = null;
                    if (dgvCommissions.DataSource != null)  //no rows 
                    {
                        foreach (DataRowView rowView in currentView)
                        {
                            if ((string)rowView[CN.ItemText] == tbCommItem.Text)
                            {
                                rowViewFound = rowView;
                            }
                        }
                    }

                    if (rowViewFound != null)
                    {
                        currentView.AllowEdit = true;
                        // Update the matching edit row found
                        rowViewFound[CN.Percentage] = tbCommPcent.Text;
                        rowViewFound[CN.PercentageCash] = tbCommPcentCash.Text;
                        if (commItemStr == "Terms Type")
                            rowViewFound[CN.Value] = Convert.ToDouble(tbCommValue.Text);
                      //  else
                      //      rowViewFound[CN.Value] = Convert.ToDouble("0");
                        rowViewFound[CN.DateFrom] = dtpCommDateFrom.Value;
                        rowViewFound[CN.DateTo] = dtpCommDateTo.Value;
                        currentView.AllowEdit = false;
                    }
                    else
                    {
                        // Add the new row as an edit row
                        currentView.AllowNew = true;
                        DataRowView rowView = currentView.AddNew();
                        rowView[CN.ItemText] = tbCommItem.Text;
                        rowView[CN.Percentage] = Convert.ToDouble(tbCommPcent.Text);
                        rowView[CN.PercentageCash] = Convert.ToDouble(tbCommPcentCash.Text);
                        if (commItemStr == "Terms Type")
                            rowView[CN.Value] = Convert.ToDouble(tbCommValue.Text);
                        //else
                        //    rowView[CN.Value] = Convert.ToDouble(Convert.ToString("0"));
                        rowView[CN.DateFrom] = dtpCommDateFrom.Value;
                        rowView[CN.DateTo] = dtpCommDateTo.Value;
                        rowView[CN.Branch] = tbSpiffBranch.Text;        //CR1035
                        rowView.EndEdit();
                        currentView.AllowNew = false;
                    }

                    // Reset the input fields
                    ClearEntryFields();
                    // enable save button 
                    btnSaveComm.Enabled = true;

                }


            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private bool ValidCommission()
        {
            bool valid = true;            
            ClearErrors();           
            // default blank fields to 0 for Termstype
            if (commItemStr == "Terms Type")
            {
                if (tbCommPcent.Text.Trim().Length == 0)
                    tbCommPcent.Text = "0";
                if (tbCommPcentCash.Text.Trim().Length == 0)
                    tbCommPcentCash.Text = "0";
                if (tbCommValue.Text.Trim().Length == 0)
                    tbCommValue.Text = "0";
            }
            if (commItemStr == "Product Category")
            {
                // remove leading zero (if entered) from categories < 10 to avoid ambiguity e.g 1 and 01
                if (tbCommItem.Text.Length == 2 && Convert.ToInt16(tbCommItem.Text) < 10)
                    tbCommItem.Text = tbCommItem.Text.Replace("0", "");
            }

            //IP - 22/05/08 - UAT(454) v5.1 - Should not be able to enter a 'Product Category' > 2 characters.
            //jec moved to here 11/06/08 UAT
            if (tbCommItem.Text.Length > 2 && commItemStr == "Product Category")
            {
                errorProvider1.SetError(tbCommItem, GetResource("M_PRODCAT"));
                //btnEnterComm.Enabled = false;
                //btnDeleteComm.Enabled = false;
                valid = false;
            }

            // Termstype must be 2 characters
            if (tbCommItem.Text.Length != 2 && commItemStr == GetResource("T_TERMSTYPE"))
            {
                errorProvider1.SetError(this.tbCommItem, GetResource("M_TERMSTYPELEN"));
                valid = false;
            }
            // Product Level must be 3 characters
            if (tbCommItem.Text.Length != 3 && commItemStr == GetResource("T_PRODUCTLEVEL"))
            {
                errorProvider1.SetError(this.tbCommItem, GetResource("M_PRODUCTLEVELLEN"));
                valid = false;
            }
            // Product must not be > 8 characters
            if (tbCommItem.Text.Length > 8 && commItemStr == GetResource("T_PRODUCT"))
            {
                errorProvider1.SetError(this.tbCommItem, GetResource("M_PRODUCTLEN"));
                valid = false;
            }

            if (!IsStrictNumeric(tbCommPcent.Text) || tbCommPcent.Text.Trim().Length == 0)
            {
                errorProvider1.SetError(this.tbCommPcent, GetResource("M_NUMERIC"));
                valid = false;
            }
            if (!IsStrictNumeric(tbCommPcentCash.Text) || tbCommPcentCash.Text.Trim().Length == 0)
            {
                errorProvider1.SetError(this.tbCommPcentCash, GetResource("M_NUMERIC"));
                valid = false;
            }

            if (!IsStrictNumeric(tbCommValue.Text) || tbCommValue.Text.Trim().Length == 0
                            && commItemStr == GetResource("T_TERMSTYPE"))
            {
                errorProvider1.SetError(this.tbCommValue, GetResource("M_NUMERIC"));
                valid = false;
            }

            if (valid)
            {

                // only percentage OR value may be entered not both
                if (commItemStr == GetResource("T_TERMSTYPE") && 
                        (Convert.ToDouble(tbCommPcent.Text) != 0 || Convert.ToDouble(tbCommPcentCash.Text) != 0)
                            && Convert.ToDouble(tbCommValue.Text) != 0)
                {
                    errorProvider1.SetError(this.tbCommValue, GetResource("M_PCENTORVALUE"));
                    valid = false;
                }
                // A percentage OR value must be entered - both can not be 0
                if (commItemStr == GetResource("T_TERMSTYPE") && 
                        Convert.ToDouble(tbCommPcent.Text) == 0 && Convert.ToDouble(tbCommPcentCash.Text) == 0
                            && Convert.ToDouble(tbCommValue.Text) == 0)
                {
                    errorProvider1.SetError(this.tbCommValue, GetResource("M_PCENTANDVALUE"));
                    valid = false;
                }

                // % rate must not exceed country parameter
                if (Convert.ToDouble(tbCommPcent.Text) > maxCommRate)
                {
                    errorProvider1.SetError(this.tbCommPcent, GetResource("M_COMMISSIONRATEEXCEEDED", new object[] { maxCommRate }));
                    valid = false;
                }
                if (Convert.ToDouble(tbCommPcentCash.Text) > maxCommRate)
                {
                    errorProvider1.SetError(this.tbCommPcentCash, GetResource("M_COMMISSIONRATEEXCEEDED", new object[] { maxCommRate }));
                    valid = false;
                }

                // value must not exceed country parameter
                if (commItemStr == "Terms Type" && Convert.ToDouble(tbCommValue.Text) > maxSpiffValue)
                {
                    errorProvider1.SetError(this.tbCommValue, GetResource("M_COMMISSIONVALUEEXCEEDED", new object[] { maxSpiffValue }));
                    valid = false;
                }
                

                // date from check
                if (dtpCommDateFrom.Value < _serverDate.AddDays(1))
                {
                    errorProvider1.SetError(this.dtpCommDateFrom, GetResource("M_DATEMUSTBEFUTURE"));
                    valid = false;
                }
                // date to check
                if (dtpCommDateTo.Value <= dtpCommDateFrom.Value && dtpCommDateTo.Value != deletedComm && !this.DesignMode)
                {
                    errorProvider1.SetError(this.dtpCommDateTo, GetResource("M_DATETOLATER"));
                    valid = false;
                }
            }
            // Does the product exist or Is there already an existing rate for the dates entered?
            if (valid)
            {
                string check = PaymentManager.ValidateCommItem(commItemStr, this.tbCommItem.Text, dtpCommDateFrom.Value, dtpCommDateTo.Value, tbSpiffBranch.Text, out _errorTxt);

                if (check == "X")   // not exists
                {                    
                    if (DialogResult.OK == ShowInfo("M_PRODUCTNOTEXIST", new object[] { commItemStr, this.tbCommItem.Text }, MessageBoxButtons.OKCancel))
                        valid = true;
                    else
                        valid = false;
                }

                if (check == "F")   // From date
                {
                    errorProvider1.SetError(this.dtpCommDateFrom, GetResource("M_COMMISSIONEXISTS"));
                    valid = false;
                }
                if (check == "T")   // To date
                {
                    errorProvider1.SetError(this.dtpCommDateTo, GetResource("M_COMMISSIONEXISTS"));
                    valid = false;
                }

            }

            return valid;
        }

        private void btnDeleteComm_click(object sender, EventArgs e)
        {
            try
            {
                Function = "btnDeleteComm_click";

                 // Check whether to replace a row in edit for the same currency
                    DataView currentView = (DataView)dgvCommissions.DataSource;
                    DataRowView rowViewFound = null;
                    foreach (DataRowView rowView in currentView)
                    {
                        if ((string)rowView[CN.ItemText] == tbCommItem.Text)
                        {
                            rowViewFound = rowView;
                        }
                    }

                    if (rowViewFound != null)  
                    {
                        currentView.AllowEdit = true;
                        // Commission active 
                        
                        //if (rowViewFound != null) 
                        if ((DateTime)rowViewFound[CN.DateFrom] <= _serverDate)
                        {
                        // Update "date to" to today
                            rowViewFound[CN.Percentage] = tbCommPcent.Text;
                            rowViewFound[CN.PercentageCash] = tbCommPcentCash.Text;
                            rowViewFound[CN.DateFrom] = new System.DateTime(2006, 1, 1, 00, 0, 00, 000);
                            rowViewFound[CN.DateFrom] = dtpCommDateFrom.Value;
                            rowViewFound[CN.DateTo] = _serverDate;
                        // display status message
                       ((MainForm)this.FormRoot).statusBar1.Text = "Commission terminated with today's date";

                        }
                        else
                        {
                        // Flag as "delete"
                            rowViewFound[CN.Percentage] = tbCommPcent.Text;
                            rowViewFound[CN.PercentageCash] = tbCommPcentCash.Text;
                            // rowViewFound[CN.DateFrom] = new System.DateTime(1900, 1, 1, 00, 0, 00, 000);
                            rowViewFound[CN.DateTo] = new System.DateTime(1999, 1, 1, 00, 0, 00, 000);
                        }
                       // rowView.EndEdit();
                        currentView.AllowEdit = false;
                        // Reset the input fields
                        ClearEntryFields();
                        // enable save button 
                        btnSaveComm.Enabled = true;
                    }
                                
            }

            catch (Exception ex)
            {
                Catch(ex, Function);
            }

        }
        
        private void btnSaveComm_click(object sender, EventArgs e)
        {
            // do not allow save if errors or entry not complete
            if (btnEnterComm.Enabled)
            {
                ShowInfo("M_SAVECOMMNOTALLOWED", MessageBoxButtons.OK);
            }
            else
                SaveCommissions(); 
            
        }
        // Save Commissions
        private void SaveCommissions()
        {
            DataSet commissionRateSet = null;
            commissionRateSet = ((DataView)dgvCommissions.DataSource).Table.DataSet;

            _errorTxt = PaymentManager.SaveCommissionRates(commItemStr, commissionRateSet);

            if (_errorTxt.Length > 0) ShowError(_errorTxt);

            // disable save button 
            btnSaveComm.Enabled = false;
            ((MainForm)this.FormRoot).statusBar1.Text = " Commission Rates saved";
            // clear datagrid
            dgvCommissions.DataSource = null;
            tbCommItem.Enabled = false;
            tbCommPcent.Enabled = false;
            tbCommPcentCash.Enabled = false;
            tbCommValue.Enabled = false;
            // disable clear to force reload after save
            btnClear.Enabled = false;       // jec 20/11/07
        }

        // Save Spiffs
        private void SaveSpiffs()       
        {
            DataSet commissionRateSet = null;
            commissionRateSet = ((DataView)dgvSpiffs.DataSource).Table.DataSet;

            _errorTxt = PaymentManager.SaveCommissionRates(spiffTypeStr, commissionRateSet);

            if (_errorTxt.Length > 0) ShowError(_errorTxt);

            // disable save button 
            btnSaveSpiff.Enabled = false;
            ((MainForm)this.FormRoot).statusBar1.Text = " Spiff Rates saved";
            // clear datagrid
            dgvSpiffs.DataSource = null;   
            tbSpiffProduct.Enabled = false;
            tbSpiffPcent.Enabled = false;
            tbSpiffValue.Enabled = false;
            // disable clear to force reload after save
            btnClear.Enabled = false;       // jec 20/11/07
        }

        private void tbCommItem_leave(object sender, EventArgs e)
        {
            errorProvider1.SetError(this.tbCommItem, "");

            // remove embedded spaces
            tbCommItem.Text = tbCommItem.Text.ToUpper().Replace(" ", "").Trim();
            

            //IP - 22/05/08 - UAT(454) v5.1 - Should not be able to enter a 'Product Category' > 2 characters.
            //if (tbCommItem.Text.Length > 2)
            //{
            //    errorProvider1.SetError(tbCommItem, GetResource("M_PRODCAT"));
            //    btnEnterComm.Enabled = false;
            //    btnDeleteComm.Enabled = false;
            //}
            //else
            //{


                if (tbCommItem.Text != "")
                {
                    btnEnterComm.Enabled = true;
                    btnDeleteComm.Enabled = true;
                }
                else
                {
                    btnEnterComm.Enabled = false;
                    btnDeleteComm.Enabled = false;
                }
            //}

        }

        
        private void SetScreen()
        {
            //set Grid size for commissions
            dgvCommissions.Width = 400;
            if (commItemStr == "Terms Type")
            {
                label_OR.Visible = true;
                label_CommValue.Visible = true;
                tbCommValue.Visible = true;
            }
            else
            {
                label_OR.Visible = false;
                label_CommValue.Visible = false;
                tbCommValue.Visible = false;
            }


        }
        
        private void btnExit_Click(object sender, EventArgs e)
        {
            //ClosingForm();
            // exit screen
            CloseTab(); // The ConfirmClose method in this class will be executed in CommonForm overridden to this
        }

        private void btnReloadSpiff_Click(object sender, EventArgs e)
        {
            if (!CheckForSave())
            {
                LoadSpiffs();
            }
        }

        private void LoadSpiffs()
        {
            ClearEntryFields();
            // disable save button until change made
            btnSaveSpiff.Enabled = false;
            //enable clear
            btnClear.Enabled = true;       // jec 20/11/07

            dgvSpiffs.DataSource = null;
            
            //commItemStr = (string)drpCommItem.SelectedItem;
            DateTime selectDate = dtpSpiffDate.Value;
                        
            DataSet ds = PaymentManager.GetSalesCommissionRates(spiffTypeStr, selectDate, out _errorTxt);
                DataView dvSpiffs = ds.Tables[TN.SalesCommissionRates].DefaultView;

                dgvSpiffs.DataSource = dvSpiffs;

                dgvSpiffs.ColumnHeadersVisible = true;
                dgvSpiffs.AutoGenerateColumns = true;
                lbSpiffProduct.Text = GetResource("T_PRODUCT");
                dgvSpiffs.Columns[CN.DateFrom].HeaderText = GetResource("T_DATEFROM");
                dgvSpiffs.Columns[CN.DateFrom].Width = 70;
                dgvSpiffs.Columns[CN.DateTo].HeaderText = GetResource("T_DATETO");
                dgvSpiffs.Columns[CN.DateTo].Width = 70;
                dgvSpiffs.Columns[CN.Percentage].Width = 65;
                dgvSpiffs.Columns[CN.Percentage].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvSpiffs.Columns[CN.Value].Width = 65;
                dgvSpiffs.Columns[CN.Value].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvSpiffs.Columns[CN.Branch].Width = 45;        //CR1035
                dgvSpiffs.Columns[CN.Branch].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvSpiffs.Columns[CN.Branch].Visible = SpiffPerBranch;
                    
                ((MainForm)this.FormRoot).statusBar1.Text = " ";
                tbSpiffProduct.Enabled = true;
                tbSpiffPcent.Enabled = true;
                tbSpiffValue.Enabled = true;

                if (spiffTypeStr == "Linked Spiffs")
                {
                    //set Grid size for Linked Spiffs
                    dgvSpiffs.Width = 642;
                    tbSpiffItem2.Enabled = true;
                    tbSpiffItem3.Enabled = true;
                    tbSpiffItem4.Enabled = true;
                    tbSpiffItem5.Enabled = true;
                    tbSpiffDescription.Enabled = true;
                    ReportUtils.ApplyGridHeadings(dgvSpiffs, this);
                    // dgvCommissions.Columns[CN.Item1].HeaderText = GetResource("T_Category");
                    lbSpiffProduct.Text = GetResource("T_PRODUCT1");
                    dgvSpiffs.Columns[CN.Item1].Width = 65;
                    dgvSpiffs.Columns[CN.Item2].Width = 65;
                    dgvSpiffs.Columns[CN.Item3].Width = 65;
                    dgvSpiffs.Columns[CN.Item4].Width = 65;
                    dgvSpiffs.Columns[CN.Item5].Width = 65;
                    
                }
                else
                {
                    //set Grid size for Spiffs
                    if (SpiffPerBranch == true)
                    {
                        dgvSpiffs.Width = 440;          //CR1035
                    }
                    else
                    {
                        dgvSpiffs.Width = 400;          //CR1035
                    }
                    dgvSpiffs.Columns[CN.ItemText].HeaderText = GetResource("T_PRODUCT");
                    dgvSpiffs.Columns[CN.ItemText].Width = 65;
                    // Set visibility of Cash percentage column based on Country parameter 
                    dgvSpiffs.Columns[CN.PercentageCash].Visible = false;       // jec Note: set to false until Spiff changes done
                    dgvSpiffs.Columns[CN.PercentageCash].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }

                if (spiffTypeStr == "Terms Type Spiffs")
                {
                    dgvSpiffs.Columns[CN.ItemText].HeaderText = GetResource("T_TERMSTYPE");
                    lbSpiffProduct.Text = GetResource("T_TERMSTYPE");
                }


        }

        private void btnEnterSpiff_Click(object sender, EventArgs e)
        {
            try
            {
                Function = "btnEnterSpiff_click";

                //validate entry
                if (spiffTypeStr == "Single Spiffs" && ValidSpiff() ||
                    spiffTypeStr == "Terms Type Spiffs" && ValidSpiff())
                {
                    // Check whether to replace a row in edit for the same currency
                    DataView currentView = (DataView)dgvSpiffs.DataSource;
                    DataRowView rowViewFound = null;
                    foreach (DataRowView rowView in currentView)
                    {
                        if ((string)rowView[CN.ItemText] == tbSpiffProduct.Text && (string)rowView[CN.Branch]==tbSpiffBranch.Text)  //CR1035
                        {
                            rowViewFound = rowView;
                        }
                    }

                    if (rowViewFound != null)
                    {
                        currentView.AllowEdit = true;
                        // Update the matching edit row found
                        rowViewFound[CN.Percentage] = tbSpiffPcent.Text;
                        rowViewFound[CN.PercentageCash] = tbSpiffPcent.Text;    // jec Note: defaults cash to credit until spiff changes are done
                        rowViewFound[CN.Value] = Convert.ToDouble(tbSpiffValue.Text);
                        rowViewFound[CN.DateFrom] = dtpSpiffDateFrom.Value;
                        rowViewFound[CN.DateTo] = dtpSpiffDateTo.Value;
                        currentView.AllowEdit = false;
                    }
                    else
                    {
                        // Add the new row as an edit row
                        currentView.AllowNew = true;
                        DataRowView rowView = currentView.AddNew();
                        rowView[CN.ItemText] = tbSpiffProduct.Text;
                        rowView[CN.Percentage] = Convert.ToDouble(tbSpiffPcent.Text);
                        rowView[CN.PercentageCash] = Convert.ToDouble(tbSpiffPcent.Text);   // jec Note: defaults cash to credit until spiff changes are done
                        rowView[CN.Value] = Convert.ToDouble(tbSpiffValue.Text);
                        rowView[CN.DateFrom] = dtpSpiffDateFrom.Value;
                        rowView[CN.DateTo] = dtpSpiffDateTo.Value;
                        rowView[CN.Branch] = tbSpiffBranch.Text;        //CR1035
                        rowView.EndEdit();
                        currentView.AllowNew = false;
                    }
                    // Reset the input fields
                    ClearEntryFields();
                    // enable save button 
                    btnSaveSpiff.Enabled = true;
                }
                else
                {
                    //validate entry
                    if (spiffTypeStr == "Linked Spiffs" && ValidSpiff())
                    {
                        // Check whether to replace a row in edit for the same currency
                        DataView currentView = (DataView)dgvSpiffs.DataSource;
                        DataRowView rowViewFound = null;
                        foreach (DataRowView rowView in currentView)
                        {
                            if ((string)rowView[CN.Item1] == tbSpiffProduct.Text && (string)rowView[CN.Branch] == tbSpiffBranch.Text)   //CR1035
                            {
                                rowViewFound = rowView;
                            }
                        }

                        if (rowViewFound != null)
                        {
                            currentView.AllowEdit = true;
                            // Update the matching edit row found
                            rowViewFound[CN.Percentage] = tbSpiffPcent.Text;
                            rowViewFound[CN.Value] = Convert.ToDouble(tbSpiffValue.Text);
                            rowViewFound[CN.DateFrom] = dtpSpiffDateFrom.Value;
                            rowViewFound[CN.DateTo] = dtpSpiffDateTo.Value;
                            rowViewFound[CN.Item2] = tbSpiffItem2.Text;
                            rowViewFound[CN.Item3] = tbSpiffItem3.Text;
                            rowViewFound[CN.Item4] = tbSpiffItem4.Text;
                            rowViewFound[CN.Item5] = tbSpiffItem5.Text;
                            rowViewFound[CN.Description] = tbSpiffDescription.Text;
                            currentView.AllowEdit = false;
                        }
                        else
                        {
                            // Add the new row as an edit row
                            currentView.AllowNew = true;
                            DataRowView rowView = currentView.AddNew();
                            rowView[CN.Item1] = tbSpiffProduct.Text;
                            rowView[CN.Percentage] = Convert.ToDouble(tbSpiffPcent.Text);
                            rowView[CN.Value] = Convert.ToDouble(tbSpiffValue.Text);
                            rowView[CN.DateFrom] = dtpSpiffDateFrom.Value;
                            rowView[CN.DateTo] = dtpSpiffDateTo.Value;
                            rowView[CN.Item2] = tbSpiffItem2.Text;
                            rowView[CN.Item3] = tbSpiffItem3.Text;
                            rowView[CN.Item4] = tbSpiffItem4.Text;
                            rowView[CN.Item5] = tbSpiffItem5.Text;
                            rowView[CN.Description] = tbSpiffDescription.Text;
                            rowView[CN.Branch] = tbSpiffBranch.Text;        //CR1035
                            rowView.EndEdit();
                            currentView.AllowNew = false;
                        }
                        // Reset the input fields
                        ClearEntryFields();
                        // enable save button 
                        btnSaveSpiff.Enabled = true;

                    }                  
                }                
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void btnDeleteSpiff_Click(object sender, EventArgs e)
        {
            try
            {
                Function = "btnDeleteSpiff_click";

                // Check whether to replace a row in edit for the same item
                DataView currentView = (DataView)dgvSpiffs.DataSource;
                DataRowView rowViewFound = null;
                foreach (DataRowView rowView in currentView)
                {
                    if (spiffTypeStr == "Linked Spiffs")
                    {
                        if ((string)rowView[CN.Item1] == tbSpiffProduct.Text)
                        {
                            rowViewFound = rowView;
                        }
                    }
                    else
                    {
                        if ((string)rowView[CN.ItemText] == tbSpiffProduct.Text)
                        {
                            rowViewFound = rowView;
                        }
                    }
                }

                if (rowViewFound != null)
                {
                    currentView.AllowEdit = true;
                    // Commission active 

                    //if (rowViewFound != null) 
                    if ((DateTime)rowViewFound[CN.DateFrom] <= _serverDate)
                    {
                        // Update "date to" to today
                        rowViewFound[CN.Percentage] = tbSpiffPcent.Text;
                        rowViewFound[CN.DateFrom] = new System.DateTime(2006, 1, 1, 00, 0, 00, 000);
                        rowViewFound[CN.DateFrom] = dtpSpiffDateFrom.Value;
                        rowViewFound[CN.DateTo] = _serverDate;
                        // display status message
                        ((MainForm)this.FormRoot).statusBar1.Text = "Spiff terminated with today's date";

                    }
                    else
                    {
                        // Flag as "delete"
                        rowViewFound[CN.Percentage] = tbSpiffPcent.Text;
                       // rowViewFound[CN.DateFrom] = new System.DateTime(1900, 1, 1, 00, 0, 00, 000);
                        rowViewFound[CN.DateTo] = new System.DateTime(1999, 1, 1, 00, 0, 00, 000);
                    }
                    // rowView.EndEdit();
                    currentView.AllowEdit = false;
                    // Reset the input fields
                    ClearEntryFields();
                    // enable save button 
                    btnSaveSpiff.Enabled = true;
                }
            }

            catch (Exception ex)
            {
                Catch(ex, Function);
            }
             
        }

        private bool ValidSpiff()
        {
            bool valid = true;
            string spiffType=" ";
            ClearErrors();

            // default blank precentage/value fields to 0 
            
            if (tbSpiffPcent.Text.Trim().Length == 0)
                tbSpiffPcent.Text = "0";
            if (tbSpiffValue.Text.Trim().Length == 0)
                tbSpiffValue.Text = "0";

            // Check Product item1 length       jec 16/0/08
            if (this.tbSpiffProduct.Text.Length > 8)
            {
                errorProvider1.SetError(this.tbSpiffProduct, GetResource("M_PRODUCTLEN"));
                valid = false;
            }

            //if (tbSpiffProduct.Text.Trim().Length > 2 && spiffTypeStr == "Terms Type Spiffs")
            if (tbSpiffProduct.Text.Trim().Length != 2 && spiffTypeStr == "Terms Type Spiffs")  //jec 16/06/08
            {
                errorProvider1.SetError(this.tbSpiffProduct, GetResource("M_TERMSTYPELEN"));
                valid = false;
            }
                       
            if (!IsStrictNumeric(tbSpiffPcent.Text) || tbSpiffPcent.Text.Trim().Length == 0)
            {
                errorProvider1.SetError(this.tbSpiffPcent, GetResource("M_NUMERIC"));
                valid = false;
            }

            if (!IsStrictNumeric(tbSpiffValue.Text) || tbSpiffValue.Text.Trim().Length == 0)
            {
                errorProvider1.SetError(this.tbSpiffValue, GetResource("M_NUMERIC"));
                valid = false;
            }
            if (valid)
            {
                // only percentage OR value may be entered not both
                if (Convert.ToDouble(tbSpiffPcent.Text) != 0 && Convert.ToDouble(tbSpiffValue.Text)!= 0)
                {
                    errorProvider1.SetError(this.tbSpiffValue, GetResource("M_PCENTORVALUE"));
                    valid = false;
                }
                // A percentage OR value must be entered - both can not be 0
                if (Convert.ToDouble(tbSpiffPcent.Text) == 0 && Convert.ToDouble(tbSpiffValue.Text)== 0)
                {
                    errorProvider1.SetError(this.tbSpiffValue, GetResource("M_PCENTANDVALUE"));
                    valid = false;
                }

                // % rate must not exceed country parameter
                if (Convert.ToDouble(tbSpiffPcent.Text) > maxCommRate)
                {
                    errorProvider1.SetError(this.tbSpiffPcent, GetResource("M_COMMISSIONRATEEXCEEDED", new object[] { maxCommRate }));
                    valid = false;
                }

                // value must not exceed country parameter
                if (Convert.ToDouble(tbSpiffValue.Text) > maxSpiffValue)
                {
                    errorProvider1.SetError(this.tbSpiffValue, GetResource("M_COMMISSIONVALUEEXCEEDED", new object[] { maxSpiffValue }));
                    valid = false;
                }
                
                
                // date from check
                if (dtpSpiffDateFrom.Value < _serverDate.AddDays(1))
                {
                    errorProvider1.SetError(this.dtpSpiffDateFrom, GetResource("M_DATEMUSTBEFUTURE"));
                    valid = false;
                }
                // date to check
                if (dtpSpiffDateTo.Value <= dtpSpiffDateFrom.Value && dtpSpiffDateTo.Value != deletedComm && !this.DesignMode)
                {
                    errorProvider1.SetError(this.dtpSpiffDateTo, GetResource("M_DATETOLATER"));
                    valid = false;
                }
            }
            // Is there already an existing rate for the dates entered?
            if (valid)
            {
                string check = PaymentManager.ValidateCommItem(spiffTypeStr, this.tbSpiffProduct.Text, dtpSpiffDateFrom.Value, dtpSpiffDateTo.Value, tbSpiffBranch.Text, out _errorTxt);
                                
                if (check == "X")   // not exists
                {
                    if (lbSpiffProduct.Text==GetResource("T_TERMSTYPE"))
                        spiffType=lbSpiffProduct.Text;
                    else
                        spiffType=GetResource("T_PRODUCT");

                    if (DialogResult.OK == ShowInfo("M_PRODUCTNOTEXIST", new object[] { spiffType,this.tbSpiffProduct.Text }, MessageBoxButtons.OKCancel))
                        valid = true;
                    else
                        valid = false;
                }
                // Check for 'non stock' removed   - jec CR36 Enhancements  22/06/07
                //if (check == "W")   // non stock item
                //{
                //    errorProvider1.SetError(this.tbSpiffProduct, GetResource("M_PRODUCTNONSTOCK"));
                //    valid = false;
                //}
                if (check == "F")   // From date
                {
                    errorProvider1.SetError(this.dtpSpiffDateFrom, GetResource("M_COMMISSIONEXISTS"));
                    valid = false;
                }
                if (check == "T")   // To date
                {
                    errorProvider1.SetError(this.dtpSpiffDateTo, GetResource("M_COMMISSIONEXISTS"));
                    valid = false;
                }
            }

            if (valid)
            {
                if (spiffTypeStr == "Linked Spiffs")
                {
                    spiffType = GetResource("T_PRODUCT");
                    // Check that description has been entered - Primary key
                    if (this.tbSpiffDescription.Text.Trim().Length == 0)
                    {
                        errorProvider1.SetError(this.tbSpiffDescription, GetResource("M_LINKEDSPIFFDESCR"));                        
                        valid = false;
                    }
                    // Check at least first two items are entered
                    if (this.tbSpiffProduct.Text.Length == 0 || this.tbSpiffItem2.Text.Length == 0)
                    {
                        errorProvider1.SetError(this.tbSpiffProduct, GetResource("M_TWOPRODUCTSREQUIRED"));
                        errorProvider1.SetError(this.tbSpiffItem2, GetResource("M_TWOPRODUCTSREQUIRED"));
                        valid = false;
                    }
                    // Check Product item2 length       jec 16/0/08
                    if (this.tbSpiffItem2.Text.Length > 8)
                    {
                        errorProvider1.SetError(this.tbSpiffItem2, GetResource("M_PRODUCTLEN"));
                        valid = false;
                    }
                    // Check Product item3length       jec 16/0/08
                    if (this.tbSpiffItem3.Text.Length > 8)
                    {
                        errorProvider1.SetError(this.tbSpiffItem3, GetResource("M_PRODUCTLEN"));
                        valid = false;
                    }
                    // Check Product item3 length       jec 16/0/08
                    if (this.tbSpiffItem3.Text.Length > 8)
                    {
                        errorProvider1.SetError(this.tbSpiffItem3, GetResource("M_PRODUCTLEN"));
                        valid = false;
                    }
                    // Check Product item4length       jec 16/0/08
                    if (this.tbSpiffItem4.Text.Length > 8)
                    {
                        errorProvider1.SetError(this.tbSpiffItem4, GetResource("M_PRODUCTLEN"));
                        valid = false;
                    }
                    // Check Product item5length       jec 16/0/08
                    if (this.tbSpiffItem5.Text.Length > 8)
                    {
                        errorProvider1.SetError(this.tbSpiffItem5, GetResource("M_PRODUCTLEN"));
                        valid = false;
                    }
                 
                    // check all linked spiff items
                    //Item 2
                    if (this.tbSpiffItem2.Text.Length > 0 && valid)
                    {
                        string check2 = PaymentManager.ValidateCommItem(spiffTypeStr, this.tbSpiffItem2.Text, dtpSpiffDateFrom.Value, dtpSpiffDateTo.Value, tbSpiffBranch.Text, out _errorTxt);

                        if (check2 == "X")   // not exists
                        {
                            if (DialogResult.OK == ShowInfo("M_PRODUCTNOTEXIST", new object[] { spiffType,this.tbSpiffItem2.Text }, MessageBoxButtons.OKCancel))
                                valid = true;
                            else
                                valid = false;
                        }

                        //if (check2 == "W")   // non stock item
                        //{
                        //    errorProvider1.SetError(this.tbSpiffItem2, GetResource("M_PRODUCTNONSTOCK"));
                        //    valid = false;
                        //}
                    }
                    // Item3
                    if (this.tbSpiffItem3.Text.Length > 0 && valid)
                    {
                        string check3 = PaymentManager.ValidateCommItem(spiffTypeStr, this.tbSpiffItem3.Text, dtpSpiffDateFrom.Value, dtpSpiffDateTo.Value, tbSpiffBranch.Text, out _errorTxt);

                        if (check3 == "X")   // not exists
                        {
                            if (DialogResult.OK == ShowInfo("M_PRODUCTNOTEXIST", new object[] { spiffType, this.tbSpiffItem3.Text }, MessageBoxButtons.OKCancel))
                                valid = true;
                            else
                                valid = false;
                        }

                        //if (check3 == "W")   // non stock item
                        //{
                        //    errorProvider1.SetError(this.tbSpiffItem3, GetResource("M_PRODUCTNONSTOCK"));
                        //    valid = false;
                        //}
                    }
                    // Item4
                    if (this.tbSpiffItem4.Text.Length > 0 && valid)
                    {
                        string check4 = PaymentManager.ValidateCommItem(spiffTypeStr, this.tbSpiffItem4.Text, dtpSpiffDateFrom.Value, dtpSpiffDateTo.Value, tbSpiffBranch.Text, out _errorTxt);

                        if (check4 == "X")   // not exists
                        {
                            if (DialogResult.OK == ShowInfo("M_PRODUCTNOTEXIST", new object[] { spiffType, this.tbSpiffItem4.Text }, MessageBoxButtons.OKCancel))
                                valid = true;
                            else
                                valid = false;
                        }

                        //if (check4 == "W")   // non stock item
                        //{
                        //    errorProvider1.SetError(this.tbSpiffItem4, GetResource("M_PRODUCTNONSTOCK"));
                        //    valid = false;
                        //}
                    }
                    // Item5
                    if (this.tbSpiffItem5.Text.Length > 0 && valid)
                    {
                        string check5 = PaymentManager.ValidateCommItem(spiffTypeStr, this.tbSpiffItem5.Text, dtpSpiffDateFrom.Value, dtpSpiffDateTo.Value, tbSpiffBranch.Text, out _errorTxt);

                        if (check5 == "X")   // not exists
                        {
                            if (DialogResult.OK == ShowInfo("M_PRODUCTNOTEXIST", new object[] { spiffType, this.tbSpiffItem5.Text }, MessageBoxButtons.OKCancel))
                                valid = true;
                            else
                                valid = false;
                        }

                        //if (check5 == "W")   // non stock item
                        //{
                        //    errorProvider1.SetError(this.tbSpiffItem5, GetResource("M_PRODUCTNONSTOCK"));
                        //    valid = false;
                        //}
                    }                

                }

            }

            return valid;
        }

        private void tbSpiffProduct_Leave(object sender, EventArgs e)
        {
            // remove embedded spaces & convert to Upper case
            tbSpiffProduct.Text = tbSpiffProduct.Text.ToUpper().Replace(" ", "").Trim();

            if (tbSpiffProduct.Text == "")
            {
                tbSpiffPcent.Enabled = false;
                tbSpiffValue.Enabled = false;
                dtpSpiffDateFrom.Enabled = false;
                dtpSpiffDateTo.Enabled = false;
                tbSpiffItem2.Enabled = false;
                tbSpiffItem3.Enabled = false;
                tbSpiffItem4.Enabled = false;
                tbSpiffItem5.Enabled = false;
                tbSpiffDescription.Enabled = false;
                ((MainForm)this.FormRoot).statusBar1.Text = "You must enter a Product before continuing";
            }
            else
            {
                tbSpiffPcent.Enabled = true;
                tbSpiffValue.Enabled = true;
                dtpSpiffDateFrom.Enabled = true;
                dtpSpiffDateTo.Enabled = false;
                tbSpiffItem2.Enabled = true;
                tbSpiffItem3.Enabled = true;
                tbSpiffItem4.Enabled = true;
                tbSpiffItem5.Enabled = true;
                tbSpiffDescription.Enabled = true;
            }

            if (tbSpiffProduct.Text != "")
            {
                btnEnterSpiff.Enabled = true;
                btnDeleteSpiff.Enabled = true;
            }
           
            else
            {
                btnEnterSpiff.Enabled = false;
                btnDeleteSpiff.Enabled = false;
                
            }
            if (spiffTypeStr == "Linked Spiffs" && tbSpiffItem2.Text == "" && tbSpiffProduct.Text != "")
            // display status message
                ((MainForm)this.FormRoot).statusBar1.Text = "You must enter Product 2 before the Linked Spiff can be added";
            else
            {
                if (tbSpiffProduct.Text != "")
                {
                    btnEnterSpiff.Enabled = true;
                    btnDeleteSpiff.Enabled = true;
                    ((MainForm)this.FormRoot).statusBar1.Text = "";
                }
            }
        }

        private void dgvSpiffsRowSelected_click(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                Function = "dgvSpiffsRowSelected_click";
                Wait();

                ClearEntryFields();

                btnEnterSpiff.Enabled = true;
                btnDeleteSpiff.Enabled = true;
                dtpSpiffDateTo.Enabled = false;
                tbSpiffProduct.Enabled = false;
                                               
                int index = dgvSpiffs.CurrentRow.Index;
                // Single Spiffs
                if (index >= 0 && spiffTypeStr == "Single Spiffs")
                {
                    tbSpiffProduct.Text = (string)((DataView)dgvSpiffs.DataSource)[index][CN.ItemText];
                    tbSpiffPcent.Text = Convert.ToString(((DataView)dgvSpiffs.DataSource)[index][CN.Percentage]);
                    tbSpiffValue.Text = Convert.ToString(((DataView)dgvSpiffs.DataSource)[index][CN.Value]);
                    dtpSpiffDateFrom.Value = (DateTime)((DataView)dgvSpiffs.DataSource)[index][CN.DateFrom];
                    dtpSpiffDateTo.Value = (DateTime)((DataView)dgvSpiffs.DataSource)[index][CN.DateTo];
                    tbSpiffBranch.Text = (string)((DataView)dgvSpiffs.DataSource)[index][CN.Branch];        //CR1035

                }
                // Linked Spiffs
                if (index >= 0 && spiffTypeStr == "Linked Spiffs")
                {
                    tbSpiffProduct.Text = (string)((DataView)dgvSpiffs.DataSource)[index][CN.Item1];
                    tbSpiffPcent.Text = Convert.ToString(((DataView)dgvSpiffs.DataSource)[index][CN.Percentage]);
                    tbSpiffValue.Text = Convert.ToString(((DataView)dgvSpiffs.DataSource)[index][CN.Value]);
                    dtpSpiffDateFrom.Value = (DateTime)((DataView)dgvSpiffs.DataSource)[index][CN.DateFrom];
                    dtpSpiffDateTo.Value = (DateTime)((DataView)dgvSpiffs.DataSource)[index][CN.DateTo];
                    tbSpiffItem2.Text = (string)((DataView)dgvSpiffs.DataSource)[index][CN.Item2];
                    tbSpiffItem3.Text = (string)((DataView)dgvSpiffs.DataSource)[index][CN.Item3];
                    tbSpiffItem4.Text = (string)((DataView)dgvSpiffs.DataSource)[index][CN.Item4];
                    tbSpiffItem5.Text = (string)((DataView)dgvSpiffs.DataSource)[index][CN.Item5];
                    tbSpiffDescription.Text = (string)((DataView)dgvSpiffs.DataSource)[index][CN.Description];
                    tbSpiffBranch.Text = (string)((DataView)dgvSpiffs.DataSource)[index][CN.Branch];        //CR1035

                }
                //  Clear Status message;
                if (dtpSpiffDateTo.Value == date2050)
                    ((MainForm)this.FormRoot).statusBar1.Text = GetResource("M_CHANGEDATE");   //"Change Date From to add new commission rate";
                else
                    ((MainForm)this.FormRoot).statusBar1.Text = GetResource("M_NEWRATE");      //"A subsequent commission rate has been added..."
                // rate ends no changes allowed - add a new rate
                if (dtpSpiffDateTo.Value <= _serverDate)
                {
                    tbSpiffPcent.Enabled = false;
                    tbSpiffValue.Enabled = false;
                    dtpSpiffDateFrom.Enabled = false;
                    dtpSpiffDateTo.Enabled = false;
                    tbSpiffItem2.Enabled = false;
                    tbSpiffItem3.Enabled = false;
                    tbSpiffItem4.Enabled = false;
                    tbSpiffItem5.Enabled = false;
                    tbSpiffDescription.Enabled = false;
                    ((MainForm)this.FormRoot).statusBar1.Text = GetResource("M_RATEENDED");    //"Commission rate ended - no changes allowed";
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

        private void btnSaveSpiff_Click(object sender, EventArgs e)
        {
            // do not allow save if errors or entry not complete
            if (btnEnterSpiff.Enabled)
            {
                ShowInfo("M_SAVECOMMNOTALLOWED", MessageBoxButtons.OK);               
            }
            else
                SaveSpiffs();
        }

        private void drpCommItem_IndexChanged(object sender, EventArgs e)
        {            
            if(!CheckForSave())
                // clear Data grid
                dgvCommissions.DataSource = null;

            cbCopyCredit.Checked = true;
            LoadCommissions();

        }

        private bool CheckForSave()
        {
            bool canceled = false;
 
            // check if changes are to be saved
            if (btnSaveComm.Enabled)
            {
                DialogResult userResponse = ShowInfo("M_SAVECOMMCHANGES", new object[] { "Commission" }, MessageBoxButtons.YesNoCancel);
                if (userResponse == DialogResult.Yes)
                {
                    SaveCommissions();
                }
                if (userResponse == DialogResult.No)
                {
                    // clear Data grid
                    dgvCommissions.DataSource = null;
                    btnSaveComm.Enabled = false;
                }
                if (userResponse == DialogResult.Cancel)
                {
                    canceled = true;
                }
                
            }

            if (btnSaveSpiff.Enabled && !canceled)
            {
                DialogResult userResponse = ShowInfo("M_SAVECOMMCHANGES", new object[] { "SPIFF" }, MessageBoxButtons.YesNoCancel);
                if (userResponse == DialogResult.Yes)
                {
                    SaveSpiffs();
                }
                if (userResponse == DialogResult.No)
                {
                    // clear Data grid
                    dgvSpiffs.DataSource = null;
                    btnSaveSpiff.Enabled = false;
                }
                if (userResponse == DialogResult.Cancel)
                {
                    canceled = true;
                }
                
            }
            return canceled;
        }

        private void drpSpiffType_IndexChanged(object sender, EventArgs e)
        {
            // check for Save - if changes have been made to grid 
            // pop-up is displayed to confirm save or not
            //
            if (!CheckForSave())
            {

                spiffTypeStr = (string)drpSpiffType.SelectedItem;
                if (spiffTypeStr == "Single Spiffs" || spiffTypeStr == "Terms Type Spiffs")
                {
                    tbSpiffItem2.Visible = false;
                    tbSpiffItem3.Visible = false;
                    tbSpiffItem4.Visible = false;
                    tbSpiffItem5.Visible = false;
                    tbSpiffDescription.Visible = false;
                    lbSpiffItem2.Visible = false;
                    lbSpiffItem3.Visible = false;
                    lbSpiffItem4.Visible = false;
                    lbSpiffItem5.Visible = false;
                    lbSpiffDescription.Visible = false;

                }
                else
                {
                    tbSpiffItem2.Visible = true;
                    tbSpiffItem3.Visible = true;
                    tbSpiffItem4.Visible = true;
                    tbSpiffItem5.Visible = true;
                    tbSpiffDescription.Visible = true;
                    lbSpiffItem2.Visible = true;
                    lbSpiffItem3.Visible = true;
                    lbSpiffItem4.Visible = true;
                    lbSpiffItem5.Visible = true;
                    lbSpiffDescription.Visible = true;

                }
                // clear Data grid
                dgvSpiffs.DataSource = null;
                
            }

            LoadSpiffs();
        }

        private void ClearErrors()
        {
            errorProvider1.SetError(this.dtpCommDateFrom, "");
            errorProvider1.SetError(this.dtpCommDateTo, "");
            errorProvider1.SetError(this.tbCommItem, "");
            errorProvider1.SetError(this.tbCommValue, "");
            errorProvider1.SetError(this.tbCommPcent, "");
            errorProvider1.SetError(this.tbCommPcentCash, "");
            errorProvider1.SetError(this.tbCommItem, ""); //IP - 22/05/08 - UAT(454) v5.1

            errorProvider1.SetError(this.dtpSpiffDateFrom, "");
            errorProvider1.SetError(this.dtpSpiffDateTo, "");
            errorProvider1.SetError(this.tbSpiffProduct, "");
            errorProvider1.SetError(this.tbSpiffValue, "");
            errorProvider1.SetError(this.tbSpiffPcent, "");
            errorProvider1.SetError(this.tbSpiffDescription, "");
            errorProvider1.SetError(this.tbSpiffItem2, "");     //jec 16/06/08
            errorProvider1.SetError(this.tbSpiffItem3, "");     //jec 16/06/08
            errorProvider1.SetError(this.tbSpiffItem4, "");     //jec 16/06/08
            errorProvider1.SetError(this.tbSpiffItem5, "");     //jec 16/06/08            
            ((MainForm)this.FormRoot).statusBar1.Text = "";

          
        }

        private void dtpCommDateFrom_ValueChanged(object sender, EventArgs e)
        {

        }

        private void dtpSpiffDateFrom_ValueChanged(object sender, EventArgs e)
        {

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearEntryFields();
        }
        // Closing form - Exit button or X (form Close)
        //private void ClosingForm()
        //{
        //    if (!CheckForSave())
        //    {
        //        // need to determine response YES/NO/CANCEL  ?
        //        {
        //            // Check a valid Commission exists for all Product Categories
        //            string category = PaymentManager.ValidateCategory(out _errorTxt);

        //            if (category != "")   // missing category returned
        //            {
        //                if (DialogResult.OK == ShowInfo("M_MISSINGCOMMISSION", new object[] { category }, MessageBoxButtons.OK))
        //                {
        //                }
        //            }
        //            else
        //                // exit screen
        //                CloseTab();
        //        }
        //    }
            
        //    ConfirmClose();
                      
        //}       

        private void tbSpiffItem2_Leave(object sender, EventArgs e)
        {
            // remove embedded spaces & convert to upper case
            tbSpiffItem2.Text = tbSpiffItem2.Text.ToUpper().Replace(" ", "").Trim();

            if (tbSpiffItem2.Text != "" && tbSpiffProduct.Text != "" && spiffTypeStr == "Linked Spiffs")
            {
                btnEnterSpiff.Enabled = true;
                btnDeleteSpiff.Enabled = true;
                ((MainForm)this.FormRoot).statusBar1.Text = "";
            }

            else
            {
                btnEnterSpiff.Enabled = false;
                btnDeleteSpiff.Enabled = false;
                // display status message
                ((MainForm)this.FormRoot).statusBar1.Text = "Product 2 must be entered";
            }       
        }     

       
    // This method overides the Common Form Method to catch FormClose when X clicked
        public override bool ConfirmClose()
        {
            bool status = false;
            if (!CheckForSave())
            {
                // need to determine response YES/NO/CANCEL  ?
                {
                    // Check a valid Commission exists for all Product Categories
                    string category = PaymentManager.ValidateCategory(out _errorTxt);

                    if (category != "")   // missing category returned
                    {
                        if (DialogResult.OK == ShowInfo("M_MISSINGCOMMISSION", new object[] { category }, MessageBoxButtons.OK))
                        {
                        }
                    }
                    else
                        // exit screen
                        status = true;
                }
            }
            return status;
        }

        private void tbCommPcent_Leave(object sender, EventArgs e)
        {
        // Copy Credit% to Cash%
            if (cbCopyCredit.Checked)
                tbCommPcentCash.Text = tbCommPcent.Text;

        }

        private void cbCopyCredit_CheckStateChanged(object sender, EventArgs e)
        {
            // Copy Credit% to Cash%
            if (cbCopyCredit.Checked)
                tbCommPcentCash.Text = tbCommPcent.Text;

        }        
        // Date selected
        private void dtpCommDate_CloseUp(object sender, EventArgs e)
        {
            if (!CheckForSave())
                // clear Data grid
                dgvCommissions.DataSource = null;

            cbCopyCredit.Checked = true;
            LoadCommissions();

        }

        private void dtpSpiffDate_CloseUp(object sender, EventArgs e)
        {
            // check for Save - if changes have been made to grid 
            // pop-up is displayed to confirm save or not
            //
            if (!CheckForSave())
            {

                spiffTypeStr = (string)drpSpiffType.SelectedItem;
                if (spiffTypeStr == "Single Spiffs" || spiffTypeStr == "Terms Type Spiffs")
                {
                    tbSpiffItem2.Visible = false;
                    tbSpiffItem3.Visible = false;
                    tbSpiffItem4.Visible = false;
                    tbSpiffItem5.Visible = false;
                    tbSpiffDescription.Visible = false;
                    lbSpiffItem2.Visible = false;
                    lbSpiffItem3.Visible = false;
                    lbSpiffItem4.Visible = false;
                    lbSpiffItem5.Visible = false;
                    lbSpiffDescription.Visible = false;

                }
                else
                {
                    tbSpiffItem2.Visible = true;
                    tbSpiffItem3.Visible = true;
                    tbSpiffItem4.Visible = true;
                    tbSpiffItem5.Visible = true;
                    tbSpiffDescription.Visible = true;
                    lbSpiffItem2.Visible = true;
                    lbSpiffItem3.Visible = true;
                    lbSpiffItem4.Visible = true;
                    lbSpiffItem5.Visible = true;
                    lbSpiffDescription.Visible = true;

                }
                // clear Data grid
                dgvSpiffs.DataSource = null;

            }

            LoadSpiffs();
        }

        private void tbSpiffItem3_Leave(object sender, EventArgs e)
        {
            // remove embedded spaces & convert to upper case
            tbSpiffItem3.Text = tbSpiffItem3.Text.ToUpper().Replace(" ", "").Trim();
        }

        private void tbSpiffItem4_Leave(object sender, EventArgs e)
        {
            // remove embedded spaces & convert to upper case
            tbSpiffItem4.Text = tbSpiffItem4.Text.ToUpper().Replace(" ", "").Trim();
        }

        private void tbSpiffItem5_Leave(object sender, EventArgs e)
        {
            // remove embedded spaces & convert to upper case
            tbSpiffItem5.Text = tbSpiffItem5.Text.ToUpper().Replace(" ", "").Trim();
        }
    
        private void drpSpiffBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            // set spiff branch to dropdown branch
            tbSpiffBranch.Text = drpSpiffBranch.Text;
            ClearEntryFields();
        }

                                
    }
}