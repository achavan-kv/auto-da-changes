using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using STL.Common;
using System.Data;
using System.Web.Services.Protocols;
using STL.PL.WS1;
using STL.PL.WS2;
using STL.PL.WS3;
using STL.PL.WS4;
using STL.PL.WS5;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using System.Collections.Specialized;
using System.Xml;
using STL.Common.Constants.Elements;
using STL.Common.Constants.Tags;
using Crownwood.Magic.Menus;
using STL.Common.Constants.ItemTypes;
using STL.Common.Static;
using Blue.Cosacs.Shared.Services;
using Blue.Cosacs.Shared.Services.Warranty;
using System.Collections.Generic;

namespace STL.PL
{
    /// <summary>
    /// An account number is entered to retrieve any past warranties on the
    /// account. The user then has the option to renew one or more of the 
    /// warranties listed.
    /// </summary>
    public class WarrantyRenewals : CommonForm
    {
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnEnter;
        private System.Windows.Forms.DataGrid dgContracts;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnBuy;
        private System.Windows.Forms.DataGrid dgWarrantiesRenewals;
        private DataTable _contracts = null;
        private string errorTxt = "";
        private string _customerId = "";
        public STL.PL.AccountTextBox txtAccountNumber;
        private System.Windows.Forms.Label labelAcct;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        //      int oldCurrentRow;
        private IContainer components;

        public WarrantyRenewals()
        {
            InitializeComponent();
        }

        public WarrantyRenewals(DataView warrantiesRenewals, string customerId, Form root, Form parent)
        {
            InitializeComponent();

            FormRoot = root;
            FormParent = parent;

            TranslateControls();
            if (customerId == "")
            {
                txtAccountNumber.Visible = true;
                txtAccountNumber.ReadOnly = false;
                labelAcct.Visible = true;
                btnBuy.Enabled = false;
                btnEnter.Enabled = false;
                btnRemove.Enabled = false;
            }
            else
            {
                populateDataFields(warrantiesRenewals, customerId);
            }
        }

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WarrantyRenewals));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnEnter = new System.Windows.Forms.Button();
            this.dgContracts = new System.Windows.Forms.DataGrid();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnBuy = new System.Windows.Forms.Button();
            this.dgWarrantiesRenewals = new System.Windows.Forms.DataGrid();
            this.txtAccountNumber = new STL.PL.AccountTextBox();
            this.labelAcct = new System.Windows.Forms.Label();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgContracts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgWarrantiesRenewals)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnRemove);
            this.groupBox1.Controls.Add(this.btnEnter);
            this.groupBox1.Controls.Add(this.dgContracts);
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.btnBuy);
            this.groupBox1.Location = new System.Drawing.Point(56, 216);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(512, 120);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            // 
            // btnRemove
            // 
            this.btnRemove.BackColor = System.Drawing.Color.SlateBlue;
            this.btnRemove.Font = new System.Drawing.Font("Arial Narrow", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRemove.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.btnRemove.Image = global::STL.PL.Properties.Resources.Minus;
            this.btnRemove.Location = new System.Drawing.Point(360, 64);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(22, 22);
            this.btnRemove.TabIndex = 23;
            this.btnRemove.UseVisualStyleBackColor = false;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnEnter
            // 
            this.btnEnter.BackColor = System.Drawing.Color.SlateBlue;
            this.btnEnter.Font = new System.Drawing.Font("Arial Narrow", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEnter.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.btnEnter.Image = global::STL.PL.Properties.Resources.plus;
            this.btnEnter.Location = new System.Drawing.Point(360, 40);
            this.btnEnter.Name = "btnEnter";
            this.btnEnter.Size = new System.Drawing.Size(22, 22);
            this.btnEnter.TabIndex = 22;
            this.btnEnter.UseVisualStyleBackColor = false;
            this.btnEnter.Click += new System.EventHandler(this.btnEnter_Click);
            // 
            // dgContracts
            // 
            this.dgContracts.CaptionVisible = false;
            this.dgContracts.DataMember = "";
            this.dgContracts.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgContracts.Location = new System.Drawing.Point(8, 16);
            this.dgContracts.Name = "dgContracts";
            this.dgContracts.Size = new System.Drawing.Size(288, 96);
            this.dgContracts.TabIndex = 11;
            // 
            // btnCancel
            // 
            this.btnCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnCancel.Location = new System.Drawing.Point(392, 72);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(56, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnBuy
            // 
            this.btnBuy.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnBuy.Location = new System.Drawing.Point(392, 40);
            this.btnBuy.Name = "btnBuy";
            this.btnBuy.Size = new System.Drawing.Size(56, 23);
            this.btnBuy.TabIndex = 6;
            this.btnBuy.Text = "Buy";
            this.btnBuy.Click += new System.EventHandler(this.btnBuy_Click);
            // 
            // dgWarrantiesRenewals
            // 
            this.dgWarrantiesRenewals.CaptionText = "Available WarrantiesRenewals";
            this.dgWarrantiesRenewals.DataMember = "";
            this.dgWarrantiesRenewals.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgWarrantiesRenewals.Location = new System.Drawing.Point(8, 40);
            this.dgWarrantiesRenewals.Name = "dgWarrantiesRenewals";
            this.dgWarrantiesRenewals.ReadOnly = true;
            this.dgWarrantiesRenewals.Size = new System.Drawing.Size(608, 160);
            this.dgWarrantiesRenewals.TabIndex = 7;
            // 
            // txtAccountNumber
            // 
            this.txtAccountNumber.Location = new System.Drawing.Point(96, 8);
            this.txtAccountNumber.Name = "txtAccountNumber";
            this.txtAccountNumber.ReadOnly = true;
            this.txtAccountNumber.Size = new System.Drawing.Size(88, 20);
            this.txtAccountNumber.TabIndex = 9;
            this.txtAccountNumber.TabStop = false;
            this.txtAccountNumber.Text = "000-0000-0000-0";
            this.txtAccountNumber.Visible = false;
            this.txtAccountNumber.Leave += new System.EventHandler(this.txtAccountNumber_Leave);
            // 
            // labelAcct
            // 
            this.labelAcct.Location = new System.Drawing.Point(16, 8);
            this.labelAcct.Name = "labelAcct";
            this.labelAcct.Size = new System.Drawing.Size(72, 16);
            this.labelAcct.TabIndex = 10;
            this.labelAcct.Text = "Account No:";
            this.labelAcct.Visible = false;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.Icon = ((System.Drawing.Icon)(resources.GetObject("errorProvider1.Icon")));
            // 
            // WarrantyRenewals
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(624, 342);
            this.ControlBox = false;
            this.Controls.Add(this.txtAccountNumber);
            this.Controls.Add(this.labelAcct);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.dgWarrantiesRenewals);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WarrantyRenewals";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Warranty Renewals";
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgContracts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgWarrantiesRenewals)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        const int warrantyModifer = 100000;

        private void btnEnter_Click(object sender, System.EventArgs e)
        {
            bool status = true;

            try
            {
                Function = "btnEnter_Click";
                Wait();

                bool isSelected = false;
                int index = dgWarrantiesRenewals.CurrentRowIndex;
                if (index >= 0)
                {
                    var warRenewRow = ((DataView)dgWarrantiesRenewals.DataSource)[index];

                    string warrantyNo = Convert.ToString(warRenewRow[CN.RenewalWarrantyNo]);
                    int warrantyId = Convert.ToInt32(warRenewRow[CN.NewWarrantyID]);
                    string accountNo = Convert.ToString(warRenewRow[CN.AcctNo]);
                    int warrantyLocn = Convert.ToInt16(warRenewRow[CN.WarrantyLocation]);
                    string origContractNo = Convert.ToString(warRenewRow[CN.ContractNo]);
                    string itemNo = Convert.ToString(warRenewRow[CN.ItemNo]);
                    short stockLocn = Convert.ToInt16(warRenewRow[CN.StockLocn]);
                    int itemId = Convert.ToInt32(warRenewRow[CN.ItemId]);
                    DateTime expirtDate = (DateTime)warRenewRow[CN.DateExpires];
                    var newWarrantyDescription = warRenewRow["Description"];
                    var newWarrantyTaxRate = warRenewRow["taxRate"];
                    var newWarrantyLength = warRenewRow["length"];
                    var newWarrantyPrice = warRenewRow["warrantyprice"];
                    var newWarrantyCostPrice= warRenewRow["warrantycostprice"];
                    string typeCode = Convert.ToString(warRenewRow["TypeCode"]);                // #17313


                    foreach (DataRowView addedItems in (DataView)dgContracts.DataSource)
                    {
                        if (Convert.ToInt32(addedItems[CN.NewWarrantyID]) == warrantyId &&
                            Convert.ToString(addedItems[CN.AccountNo]) == accountNo &&
                            Convert.ToInt16(addedItems[CN.WarrantyLocation]) == warrantyLocn)
                        {
                            isSelected = true;
                            break;
                        }
                    }

                    if (!isSelected)
                    {
                        DataRow row = _contracts.NewRow();
                        string contractNo = "";

                        if ((bool)Country[CountryParameterNames.AutomaticWarrantyNo])
                        {
                            contractNo = AccountManager.AutoWarranty(Config.BranchCode, out errorTxt);
                            if (errorTxt.Length > 0)
                            {
                                ShowError(errorTxt);
                                status = false;
                            }
                        }

                        if (status)
                        {
                            row[CN.ContractNo] = origContractNo;
                            row[CN.RenewalContractNo] = contractNo;
                            row[CN.RenewalWarrantyNo] = warrantyNo;
                            row[CN.AccountNo] = accountNo;
                            row[CN.WarrantyLocation] = warrantyLocn;
                            row[CN.DateExpires] = expirtDate;
                            row[CN.ItemNo] = itemNo;
                            row[CN.StockLocn] = stockLocn;
                            row[CN.ItemId] = itemId;
                            row[CN.NewWarrantyID] = warrantyId + warrantyModifer;
                            row["Description"] = newWarrantyDescription;
                            row["length"] = newWarrantyLength;
                            row["taxRate"] = newWarrantyTaxRate;
                            row["Price"] = newWarrantyPrice;
                            row["CostPrice"] = newWarrantyCostPrice;
                            row["TypeCode"] = typeCode;             // #17313
                            _contracts.Rows.Add(row);
                        }
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
                Function = "End of btnEnter_Click";
            }
        }

        private void btnRemove_Click(object sender, System.EventArgs e)
        {
            try
            {
                Function = "btnRemove_Click";
                int index = dgContracts.CurrentRowIndex;

                if (index >= 0)
                {
                    /* remove the actuall datagrid entry */
                    _contracts.DefaultView.AllowDelete = true;
                    _contracts.DefaultView[index].Delete();
                    _contracts.DefaultView.AllowDelete = false;
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
                Function = "End of btnRemove_Click";
            }
        }

        private void btnBuy_Click(object sender, System.EventArgs e)
        {
            bool valid = true;
            int count = 0;
            var req = new List<SaveWarrantyStockinfoRequest.WarrantyStockInfo>();

            try
            {
                foreach (DataRowView row in (DataView)dgContracts.DataSource)
                {
                    valid = ValidateContracts((string)row[CN.RenewalContractNo], (string)row[CN.AccountNo]);
                    count++;

                    req.Add (new SaveWarrantyStockinfoRequest.WarrantyStockInfo{
                        Description = row["Description"].ToString(),
                        Id = Convert.ToInt32(row[CN.NewWarrantyID]),
                        ItemNo = row[CN.RenewalWarrantyNo].ToString(),
                        Length = Convert.ToInt32(row["length"]),
                        Location = Convert.ToInt16(row[CN.WarrantyLocation]),
                        TaxRate =  Convert.ToDecimal(row["taxRate"]),
                        Price = Convert.ToDecimal(row["Price"]),
                        CostPrice = Convert.ToDecimal(row["CostPrice"]),
                        WarrantyType = row["TypeCode"].ToString(),              // #17313
                    });



                    if (!valid)
                        break;
                }

                if (valid && count > 0)
                {
                    Client.Call(new SaveWarrantyStockinfoRequest
                    {
                        StockInfo = req
                    }, response =>
                    {
                        errorProvider1.SetError(dgContracts, "");
                        NewAccount acct = new NewAccount(_contracts, false, _customerId, true, this.FormRoot, this);
                        acct.Renewal = true;

                        ((MainForm)this.FormRoot).AddTabPage(acct);
                        Close();
                    }, this);
                 
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
                Function = "End of btnBuy_Click";
            }
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            if (_customerId != "")
            {
                foreach (DataRowView addedItems in (DataView)dgWarrantiesRenewals.DataSource)
                {
                    AccountManager.AddWarrantRenewalCode((string)addedItems[CN.AcctNo],
                                                         (string)addedItems[CN.ContractNo],
                                                         out errorTxt);
                    if (errorTxt.Length > 0)
                        ShowInfo(errorTxt);
                }
            }
            Close();
        }

        private void txtAccountNumber_Leave(object sender, System.EventArgs e)
        {

            string customerId = "";
            //DataSet ds = AccountManager.GetWarrantyRenewals(txtAccountNumber.UnformattedText, false, true, ref customerId, out errorTxt);

            Client.Call(new GetWarrantyRenewalsRequest { AccountNumber = txtAccountNumber.UnformattedText }, response =>
               {

                   var renewal = new DataTable();

                   if (response.WarrantyRenewal.Tables.Count > 0)  //#15168
                   {
                       renewal = response.WarrantyRenewal.Tables[0];
                   }    

                   if (renewal.Rows.Count > 0)
                   {
                       customerId = _customerId = Convert.ToString(renewal.Rows[0]["custid"]);            //#16237
                       populateDataFields(new DataView(renewal), customerId);
                   }
                   else
                   {
                       txtAccountNumber.Text = "000-0000-0000-0";
                   }
               }, this);

            //DataView warrantyrenewals = ds.Tables[TN.Accounts].DefaultView;

        }

        private void populateDataFields(DataView warrantiesRenewals, string customerId)
        {
            txtAccountNumber.Visible = false;
            txtAccountNumber.ReadOnly = true;
            labelAcct.Visible = false;
            btnBuy.Enabled = true;
            btnEnter.Enabled = true;
            btnRemove.Enabled = true;
            _customerId = customerId;

            dgWarrantiesRenewals.DataSource = warrantiesRenewals;

            DataGridTableStyle tabStyle = new DataGridTableStyle();
            tabStyle.MappingName = "Renewal";
            dgWarrantiesRenewals.TableStyles.Add(tabStyle);

            tabStyle.GridColumnStyles[CN.AcctNo].Width = 75;
            tabStyle.GridColumnStyles[CN.AcctNo].HeaderText = GetResource("T_ACCOUNTNO");

            tabStyle.GridColumnStyles[CN.ItemNo].Width = 0;
            tabStyle.GridColumnStyles[CN.ItemNo].HeaderText = GetResource("T_ITEMNO");

            tabStyle.GridColumnStyles[CN.ItemId].Width = 0;                                             //IP - 16/06/11 - CR1212 - #3941

            tabStyle.GridColumnStyles[CN.StockLocn].Width = 0;
            tabStyle.GridColumnStyles[CN.StockLocn].HeaderText = GetResource("T_STOCKLOCN");

            tabStyle.GridColumnStyles[CN.Description].Width = 130;
            tabStyle.GridColumnStyles[CN.Description].HeaderText = GetResource("T_DESCRIPTION");

            tabStyle.GridColumnStyles[CN.WarrantyNo].Width = 0;
            tabStyle.GridColumnStyles[CN.WarrantyNo].HeaderText = GetResource("T_WARRANTY_NO");

            tabStyle.GridColumnStyles[CN.WarrantyId].Width = 0;                                          //IP - 16/06/11 - CR1212 - RI - #3941            

            tabStyle.GridColumnStyles[CN.ContractNo].Width = 0;
            tabStyle.GridColumnStyles[CN.ContractNo].HeaderText = GetResource("T_CONTRACTNO");

            tabStyle.GridColumnStyles[CN.WarrantyLocation].Width = 0;
            tabStyle.GridColumnStyles[CN.WarrantyLocation].HeaderText = GetResource("T_LOCATION");

            tabStyle.GridColumnStyles[CN.DateExpires].Width = 80;
            tabStyle.GridColumnStyles[CN.DateExpires].HeaderText = "Expiry Date";

            tabStyle.GridColumnStyles[CN.NewWarrantyID].Width = 0;                                      //IP - 16/06/11 - CR1212 - RI - #3941

            tabStyle.GridColumnStyles[CN.RenewalWarrantyNo].Width = 75;
            tabStyle.GridColumnStyles[CN.RenewalWarrantyNo].HeaderText = "Warranty No";

            tabStyle.GridColumnStyles[CN.WarrantyDescr1].Width = 130;
            tabStyle.GridColumnStyles[CN.WarrantyDescr1].HeaderText = "New Warranty Desciption";

            tabStyle.GridColumnStyles[CN.WarrantyPrice].Width = 75;
            tabStyle.GridColumnStyles[CN.WarrantyPrice].HeaderText = "Price";

            tabStyle.GridColumnStyles["warrantycostprice"].Width = 0;
            tabStyle.GridColumnStyles["warrantycostprice"].HeaderText = "Cost Price";

            tabStyle.GridColumnStyles["TypeCode"].Width = 0;                    // #17313
            tabStyle.GridColumnStyles["TypeCode"].HeaderText = "Type";

            _contracts = new DataTable(TN.WarrantyList);
            _contracts.Columns.AddRange(new DataColumn[]{new DataColumn(CN.ContractNo), 
															new DataColumn(CN.RenewalContractNo),
															new DataColumn(CN.RenewalWarrantyNo), 
															new DataColumn(CN.WarrantyLocation),
															new DataColumn(CN.AccountNo),
															new DataColumn(CN.DateExpires),
															new DataColumn(CN.ItemNo),
															new DataColumn(CN.StockLocn),
                                                            new DataColumn(CN.ItemId),
                                                            new DataColumn(CN.NewWarrantyID),
                                                            new DataColumn("Description"),
                                                            new DataColumn("taxRate"),
                                                            new DataColumn("length"),
                                                            new DataColumn("Price"),
                                                            new DataColumn("CostPrice"),
                                                            new DataColumn("TypeCode")          // #17313
            
            });

            dgContracts.DataSource = _contracts.DefaultView;
            _contracts.DefaultView.AllowDelete = false;
            _contracts.DefaultView.AllowNew = false;
            _contracts.DefaultView.AllowEdit = !(bool)Country[CountryParameterNames.AutomaticWarrantyNo];

            tabStyle = new DataGridTableStyle();
            tabStyle.MappingName = _contracts.TableName;

            AddColumnStyle(CN.ContractNo, tabStyle, 0, true, GetResource("T_CONTRACTNO"), "", HorizontalAlignment.Left);
            AddColumnStyle(CN.WarrantyLocation, tabStyle, 0, true, GetResource("T_LOCATION"), "", HorizontalAlignment.Left);
            AddColumnStyle(CN.AccountNo, tabStyle, 100, true, GetResource("T_ACCOUNTNO"), "", HorizontalAlignment.Left);
            AddColumnStyle(CN.RenewalWarrantyNo, tabStyle, 0, true, GetResource("T_WARRANTY_NO"), "", HorizontalAlignment.Left);
            AddColumnStyle(CN.RenewalContractNo, tabStyle, 100, (bool)Country[CountryParameterNames.AutomaticWarrantyNo], "Contract No", "", HorizontalAlignment.Left);
            AddColumnStyle(CN.DateExpires, tabStyle, 0, true, "New Expiry Date", "", HorizontalAlignment.Left);
            AddColumnStyle(CN.ItemNo, tabStyle, 0, true, GetResource("T_ITEMNO"), "", HorizontalAlignment.Left);
            AddColumnStyle(CN.StockLocn, tabStyle, 0, true, GetResource("T_STOCKLOCN"), "", HorizontalAlignment.Left);
            AddColumnStyle(CN.ItemId, tabStyle, 0, true, "ItemId", "", HorizontalAlignment.Left);
            dgContracts.TableStyles.Add(tabStyle);
        }

        // Amended check on contract number to allow 10 digit no
        private bool ValidateContracts(string contractNo, string acc)
        {
            string msg = "";
            bool valid = true;

            //make sure it's not DBNull
            if (contractNo.Length == 0)
            {
                valid = false;
                msg = "You must enter a contract number";
            }

            //make sure it's not too long
            if (valid)
            {
                if (contractNo.Length > 10)
                {
                    valid = false;
                    msg = "Contract number must be less than 11 characters.";
                }
            }

            if (valid)
            {
                /* if the contract no has not been automatically generated
                 * then we need to make sure it's unique */
                if (!(bool)Country[CountryParameterNames.AutomaticWarrantyNo])
                {
                    /* 1) check the items XmlDocument and see if there
                     * are any contract nodes with this contract number 
                     * 2) if found see if it's for the same item and stocklocn 
                     * if it is then that's OK, otherwise it's not unique */

                    //string xpath = "//"+Elements.ContractNo+"[@"+Tags.ContractNumber+" = '"+text+"']";

                    if (valid)
                    {
                        /* check the database and make sure this contract no
                         * hasn't been used on another account 
                         * passing in a blank account number as the new account number
                         * will not be generated until we click the buy button */

                        bool unique = false;
                        AccountManager.ContractNoUnique("", 1, contractNo, out unique, out errorTxt);
                        if (errorTxt.Length > 0)
                            ShowError(errorTxt);
                        else
                        {
                            if (!unique)
                            {
                                valid = false;
                                msg = "Contract number " + contractNo + " is used on another account";
                            }
                        }
                    }
                }
            }

            if (!valid)
            {
                // dgContracts.CurrentCell = new DataGridCell(oldCurrentRow, 0);
                errorProvider1.SetError(dgContracts, msg);
            }
            else
            {
                errorProvider1.SetError(dgContracts, "");
            }
            return valid;
        }
    }
}
