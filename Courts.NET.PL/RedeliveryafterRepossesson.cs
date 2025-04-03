using System;
using System.Data;
using System.Windows.Forms;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using STL.Common.Static;

namespace STL.PL
{
    /// <summary>
    /// Authorises repossessed items to be re-delivered.
    /// Lists all repossessed items for a specified account number. One or more
    /// individual items can be selected for delivery with a new delivery date.
    /// These items are then scheduled for re-delivery.
    /// </summary>
    public class RedeliveryAfterRepossesson : CommonForm
    {
        private System.Windows.Forms.DataGrid dgRepossessedItems;
        public STL.PL.AccountTextBox txtAccountNumber;
        private System.Windows.Forms.Label label7;
        DataView dvRepossessedItems = null;
        private string errorTxt;
        private System.Windows.Forms.Button btnMarkDel;
        private System.Windows.Forms.Button btnActDet;
        private System.Windows.Forms.DateTimePicker dtDateIssued;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkImmediate;
        private System.Windows.Forms.Label label5;
        private DataTable _deliveryAreaData;
        private Label label1;
        private ComboBox cbTime;
        private Label lTimeRequired;
        private ErrorProvider errorProvider1;
        private System.ComponentModel.IContainer components;
        private ToolTip toolTipDelArea;
        private ComboBox drpDeliveryAdr;
        private DataTable delAreas = null;  //#12224 - CR12249

        public RedeliveryAfterRepossesson(Form root, Form parent)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            FormRoot = root;
            FormParent = parent;
            dtDateIssued.Value = DateTime.Today;
            LoadDeliveryArea();

            //toolTipDelArea.SetToolTip(btnDelAreas, GetResource("TT_DELAREA"));                //#14796
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            this.dgRepossessedItems = new System.Windows.Forms.DataGrid();
            this.txtAccountNumber = new STL.PL.AccountTextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnMarkDel = new System.Windows.Forms.Button();
            this.btnActDet = new System.Windows.Forms.Button();
            this.dtDateIssued = new System.Windows.Forms.DateTimePicker();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lTimeRequired = new System.Windows.Forms.Label();
            this.cbTime = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.chkImmediate = new System.Windows.Forms.CheckBox();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.toolTipDelArea = new System.Windows.Forms.ToolTip(this.components);
            this.drpDeliveryAdr = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgRepossessedItems)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // dgRepossessedItems
            // 
            this.dgRepossessedItems.CaptionText = "Repossessed Items";
            this.dgRepossessedItems.DataMember = "";
            this.dgRepossessedItems.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgRepossessedItems.Location = new System.Drawing.Point(24, 96);
            this.dgRepossessedItems.Name = "dgRepossessedItems";
            this.dgRepossessedItems.ReadOnly = true;
            this.dgRepossessedItems.Size = new System.Drawing.Size(744, 344);
            this.dgRepossessedItems.TabIndex = 17;
            this.dgRepossessedItems.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgRepossessedItems_MouseUp);
            // 
            // txtAccountNumber
            // 
            this.txtAccountNumber.Location = new System.Drawing.Point(100, 50);
            this.txtAccountNumber.Name = "txtAccountNumber";
            this.txtAccountNumber.PreventPaste = false;
            this.txtAccountNumber.Size = new System.Drawing.Size(104, 20);
            this.txtAccountNumber.TabIndex = 19;
            this.txtAccountNumber.TabStop = false;
            this.txtAccountNumber.Text = "000-0000-0000-0";
            this.txtAccountNumber.Leave += new System.EventHandler(this.txtAccountNumber_Leave);
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(28, 53);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(72, 17);
            this.label7.TabIndex = 18;
            this.label7.Text = "Account No:";
            // 
            // btnMarkDel
            // 
            this.btnMarkDel.Enabled = false;
            this.btnMarkDel.Location = new System.Drawing.Point(391, 21);
            this.btnMarkDel.Name = "btnMarkDel";
            this.btnMarkDel.Size = new System.Drawing.Size(75, 43);
            this.btnMarkDel.TabIndex = 20;
            this.btnMarkDel.Text = "Mark For Delivery";
            this.btnMarkDel.Click += new System.EventHandler(this.btnMarkDel_Click);
            // 
            // btnActDet
            // 
            this.btnActDet.Enabled = false;
            this.btnActDet.Location = new System.Drawing.Point(696, 40);
            this.btnActDet.Name = "btnActDet";
            this.btnActDet.Size = new System.Drawing.Size(75, 32);
            this.btnActDet.TabIndex = 21;
            this.btnActDet.Text = "Account Details ";
            this.btnActDet.Click += new System.EventHandler(this.btnActDet_Click);
            // 
            // dtDateIssued
            // 
            this.dtDateIssued.CustomFormat = "ddd dd MMM yyyy ";
            this.dtDateIssued.Enabled = false;
            this.dtDateIssued.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtDateIssued.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.dtDateIssued.Location = new System.Drawing.Point(6, 41);
            this.dtDateIssued.Name = "dtDateIssued";
            this.dtDateIssued.Size = new System.Drawing.Size(120, 20);
            this.dtDateIssued.TabIndex = 27;
            this.dtDateIssued.Value = new System.DateTime(2002, 5, 8, 0, 0, 0, 0);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.drpDeliveryAdr);
            this.groupBox1.Controls.Add(this.lTimeRequired);
            this.groupBox1.Controls.Add(this.cbTime);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.chkImmediate);
            this.groupBox1.Controls.Add(this.dtDateIssued);
            this.groupBox1.Controls.Add(this.btnMarkDel);
            this.groupBox1.Location = new System.Drawing.Point(210, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(478, 80);
            this.groupBox1.TabIndex = 24;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Delivery Details ";
            // 
            // lTimeRequired
            // 
            this.lTimeRequired.Location = new System.Drawing.Point(136, 21);
            this.lTimeRequired.Name = "lTimeRequired";
            this.lTimeRequired.Size = new System.Drawing.Size(32, 16);
            this.lTimeRequired.TabIndex = 52;
            this.lTimeRequired.Text = "Time";
            this.lTimeRequired.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cbTime
            // 
            this.cbTime.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTime.FormattingEnabled = true;
            this.cbTime.Items.AddRange(new object[] {
            "AM",
            "PM"});
            this.cbTime.Location = new System.Drawing.Point(139, 40);
            this.cbTime.Name = "cbTime";
            this.cbTime.Size = new System.Drawing.Size(42, 21);
            this.cbTime.TabIndex = 38;
            this.cbTime.SelectedIndexChanged += new System.EventHandler(this.cbTime_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(1, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 17);
            this.label1.TabIndex = 37;
            this.label1.Text = "Delivery Required";
            this.label1.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(271, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(92, 16);
            this.label5.TabIndex = 35;
            this.label5.Text = "Delivery Address";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chkImmediate
            // 
            this.chkImmediate.Location = new System.Drawing.Point(193, 36);
            this.chkImmediate.Name = "chkImmediate";
            this.chkImmediate.Size = new System.Drawing.Size(81, 28);
            this.chkImmediate.TabIndex = 28;
            this.chkImmediate.Text = "Immediate";
            this.chkImmediate.CheckedChanged += new System.EventHandler(this.chkImmediate_CheckedChanged);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // drpDeliveryAdr
            // 
            this.drpDeliveryAdr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpDeliveryAdr.FormattingEnabled = true;
            this.drpDeliveryAdr.Location = new System.Drawing.Point(285, 40);
            this.drpDeliveryAdr.Name = "drpDeliveryAdr";
            this.drpDeliveryAdr.Size = new System.Drawing.Size(56, 21);
            this.drpDeliveryAdr.TabIndex = 53;
            // 
            // RedeliveryAfterRepossesson
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(792, 470);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnActDet);
            this.Controls.Add(this.txtAccountNumber);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.dgRepossessedItems);
            this.Name = "RedeliveryAfterRepossesson";
            this.Text = "Authorise Redelivery After Repossession";
            ((System.ComponentModel.ISupportInitialize)(this.dgRepossessedItems)).EndInit();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private void txtAccountNumber_Leave(object sender, System.EventArgs e)
        {
            loadRepossessedItems();
        }

        private void btnMarkDel_Click(object sender, System.EventArgs e)
        {
            int lastLocation = 0;
            int buffNo = 0;
            bool itemsChecked = false;	// 67916	jec
            var status = true;          //#13926
            //errorProvider1.SetError(drpDeliveryArea, "");    //#13926

            try
            {
                Wait();

                // Check if account is settled   -   67887 jec
                // Check if account is settled   -   67887 jec
                if (AccountSettled(txtAccountNumber.UnformattedText))
                {
                    MessageBox.Show("You must unsettled this account before you can mark the items for re-delivery", "Account Settled",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {

                    //if (!chkImmediate.Checked && drpDeliveryArea.Text == string.Empty)      //#13926
                    //{
                    //    status = false;
                    //    errorProvider1.SetError(drpDeliveryArea, GetResource("M_ENTERMANDATORY"));
                    //}
                  

                    if (status)
                    {
                        short origBranchNo = Convert.ToInt16(Config.BranchCode);

                    DataView dv = (DataView)dgRepossessedItems.DataSource;
                    int count = dv.Count;

                    for (int i = count - 1; i >= 0; i--)
                    {
                        // create a DeliveryNoteRequest node for each selected row
                        if (dgRepossessedItems.IsSelected(i))
                        {
                            itemsChecked = true;	// 67916  jec
                            if (lastLocation == 0 || lastLocation != Convert.ToInt16(dv[i][CN.RetStockLocn]))
                                buffNo = AccountManager.GetBuffNo(Convert.ToInt16(dv[i][CN.RetStockLocn]), out errorTxt);

                            string accountNo = (string)dv[i][CN.acctno];
                            DateTime datedelplan = dtDateIssued.Value;
                            char delorcol = 'S';

                            if (chkImmediate.Checked)
                                delorcol = 'I';

                            //string itemno = (string)dv[i][CN.ItemNo];
                            int itemID = Convert.ToInt32(dv[i][CN.ItemId]);             //IP - 26/05/11 - CR1212 - RI - #3636
                            short stocklocn = Convert.ToInt16(dv[i][CN.StockLocn]);
                            short quantity = Convert.ToInt16(dv[i][CN.Quantity]);
                            short retstocklocn = Convert.ToInt16(dv[i][CN.RetStockLocn]);
                            //string retitemno = (string)dv[i][CN.RetItemNo];
                            int retItemID = Convert.ToInt32(dv[i][CN.RetItemId]);       //IP - 26/05/11 - CR1212 - RI - #3636    
                            decimal retval = Convert.ToDecimal(dv[i][CN.RetVal]);
                            //string delArea = drpDeliveryArea.SelectedIndex == 0 ? "" : drpDeliveryArea.SelectedValue.ToString();
                            string delArea = "";        // #14927
                            int agrmtNo = Convert.ToInt32(dv[i][CN.AgrmtNo]);
                            string contractNo = (string)dv[i][CN.ContractNo];
                            //string parentItemNo = dv[i][CN.ParentItemNo].ToString();
                            int parentItemID = Convert.ToInt32(dv[i][CN.ParentItemId]); //IP - 26/05/11 - CR1212 - RI - #3636  
                            int lineItemId = Convert.ToInt32(dv[i][CN.LineItemId]);     //IP - 12/06/12 - #10357 - Warehouse & Deliveries

                            AccountManager.ScheduleRedelRepo(origBranchNo, accountNo, datedelplan,
                                delorcol, itemID, stocklocn, quantity,
                                retstocklocn, retItemID, retval, origBranchNo,
                                buffNo, delArea, agrmtNo, contractNo, parentItemID, lineItemId, Credential.UserId, drpDeliveryAdr.SelectedValue.ToString(), out errorTxt);    // #14927

                                lastLocation = Convert.ToInt16(dv[i][CN.RetStockLocn]);
                            }
                        }
                        // ensure an item has been selected 67916  jec
                        if (!itemsChecked)
                        {
                            MessageBox.Show("You must select items to be delivered before clicking button", "Selection Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        loadRepossessedItems();
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

        private void btnActDet_Click(object sender, System.EventArgs e)
        {
            AccountDetails ad = new AccountDetails(txtAccountNumber.UnformattedText, this.FormRoot, this);
            ((MainForm)this.FormRoot).AddTabPage(ad);
        }

        private void loadRepossessedItems()
        {
            btnActDet.Enabled = false;
            btnMarkDel.Enabled = false;
            dtDateIssued.Enabled = false;

            dgRepossessedItems.DataSource = null;
            dgRepossessedItems.TableStyles.Clear();
            DataSet ds = AccountManager.GetRepossessedItemDetails(txtAccountNumber.UnformattedText, out errorTxt);

            dvRepossessedItems = ds.Tables[TN.Accounts].DefaultView;
            dvRepossessedItems.Sort = CN.RetStockLocn + " ASC";
            dgRepossessedItems.DataSource = dvRepossessedItems;

            DataGridTableStyle tabStyle = new DataGridTableStyle();
            tabStyle.MappingName = dvRepossessedItems.Table.TableName;
            dgRepossessedItems.TableStyles.Add(tabStyle);

            tabStyle.GridColumnStyles[CN.acctno].Width = 0;

            tabStyle.GridColumnStyles[CN.ItemNo].Width = 100;
            tabStyle.GridColumnStyles[CN.ItemNo].HeaderText = GetResource("T_PRODCODE");

            tabStyle.GridColumnStyles[CN.RetItemNo].Width = 100;
            tabStyle.GridColumnStyles[CN.RetItemNo].HeaderText = GetResource("T_RETITEM");

            tabStyle.GridColumnStyles[CN.RetStockLocn].Width = 90;
            tabStyle.GridColumnStyles[CN.RetStockLocn].HeaderText = GetResource("T_RETLOCN");

            tabStyle.GridColumnStyles[CN.DateDel].Width = 90;
            tabStyle.GridColumnStyles[CN.DateDel].HeaderText = GetResource("T_DateReposs");


            tabStyle.GridColumnStyles[CN.Quantity].Width = 60;
            tabStyle.GridColumnStyles[CN.Quantity].HeaderText = GetResource("T_QUANTITY");

            tabStyle.GridColumnStyles[CN.ItemDescr1].Width = 120;
            tabStyle.GridColumnStyles[CN.ItemDescr1].HeaderText = GetResource("T_DESCRIPTION"); // FA - UAT 893 missing header

            tabStyle.GridColumnStyles[CN.ItemDescr2].Width = 140;
            tabStyle.GridColumnStyles[CN.ItemDescr2].HeaderText = GetResource("T_DESCRIPTION2"); // FA - UAT 893 missing header

            tabStyle.GridColumnStyles[CN.AgrmtNo].Width = 0;
            tabStyle.GridColumnStyles[CN.DelOrColl].Width = 0;
            tabStyle.GridColumnStyles[CN.StockLocn].Width = 0;
            tabStyle.GridColumnStyles[CN.RetVal].Width = 0;
            tabStyle.GridColumnStyles[CN.BuffNo].Width = 0;
            tabStyle.GridColumnStyles[CN.BuffBranchNo].Width = 0;
            tabStyle.GridColumnStyles[CN.HPValue].Width = 0;
            tabStyle.GridColumnStyles[CN.CashValue].Width = 0;
            tabStyle.GridColumnStyles[CN.DateReqDel].Width = 0;
            tabStyle.GridColumnStyles[CN.TimeReqDel].Width = 0;
            tabStyle.GridColumnStyles[CN.ContractNo].Width = 0;
            tabStyle.GridColumnStyles[CN.ParentItemNo].Width = 0; //IP - 20/10/08 - UAT(5.2) - UAT(549)
            tabStyle.GridColumnStyles[CN.ItemId].Width = 0;       //IP - 26/05/11 - CR1212 - RI - #3636
            tabStyle.GridColumnStyles[CN.ParentItemId].Width = 0; //IP - 26/05/11 - CR1212 - RI - #3636
            tabStyle.GridColumnStyles[CN.RetItemId].Width = 0;    //IP - 26/05/11 - CR1212 - RI - #3636
            tabStyle.GridColumnStyles[CN.LineItemId].Width = 0;   //IP - 12/06/12 - #10357 - Warehouse & Deliveries
            tabStyle.GridColumnStyles[CN.DeliveryArea].Width = 0; //#13926

            drpDeliveryAdr.DataSource = ds.Tables["AddrTypes"];       // #14927
            drpDeliveryAdr.DisplayMember = "AddType";
            drpDeliveryAdr.ValueMember = "AddType";
            drpDeliveryAdr.Enabled = false;

            // Enable buttons 
            if (dvRepossessedItems.Count > 0)
            {
                //IP - 22/04/08 - UAT(355)
                ((MainForm)this.FormRoot).statusBar1.Text = "";

                btnActDet.Enabled = true;
                btnMarkDel.Enabled = true;
                dtDateIssued.Enabled = true;
                // Check if account is settled   -   67887 jec
                if (AccountSettled(txtAccountNumber.UnformattedText))
                {
                    MessageBox.Show("You must unsettled this account before you can mark the items for re-delivery", "Account Settled",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
            else
            {
                ((MainForm)this.FormRoot).statusBar1.Text = GetResource("M_NOREPOSSREDEL");
            }
        }

        private void LoadDeliveryArea()
        {
            //Customise the Delivery Area data to displayed in the dropdown..
            if (_deliveryAreaData == null)
            {
                string error = string.Empty;
                DataSet ds = SetDataManager.GetSetsForTNameBranch(TN.DeliveryArea, Config.BranchCode, out error);
                delAreas = ds.Tables[0];      //#CR12249 - #12224
                   
                if (error.Length > 0)
                {
                    ShowError(error);
                }
                else
                {
                    _deliveryAreaData = ds.Tables[TN.SetsData].Clone();
                    DataRow row = _deliveryAreaData.NewRow();
                    //row[CN.SetName] = string.Empty;
                    //row[CN.SetDescript] = GetResource("L_ALL");

                    row[CN.SetName] = string.Empty;                         //#13926
                    row[CN.SetDescript] = string.Empty;                     //#13926
                    _deliveryAreaData.Rows.Add(row);
                    foreach (DataRow copyRow in ds.Tables[TN.SetsData].Rows)
                    {
                        row = _deliveryAreaData.NewRow();
                        row[CN.SetName] = copyRow[CN.SetName];
                        row[CN.SetDescript] = copyRow[CN.SetName].ToString();
                        //+ " : " + copyRow[CN.SetDescript].ToString(); //#CR12249 - #12224
                        _deliveryAreaData.Rows.Add(row);
                    }
                }
            }

            //if (_deliveryAreaData != null)
            //{
            //    drpDeliveryArea.DataSource = _deliveryAreaData;
            //    drpDeliveryArea.DisplayMember = CN.SetDescript;
            //    drpDeliveryArea.ValueMember = CN.SetName;
            //}
        }
        // Check if account is settled   -   67887 jec
        private bool AccountSettled(string accountNo)
        {
            bool settled = false;
            DataSet st = AccountManager.GetAccountDetails(accountNo, out errorTxt);

            foreach (DataTable acc in st.Tables)
            {
                foreach (DataRow row in acc.Rows)
                {
                    settled = (row[CN.CurrentStatus].ToString() == "S");
                    //	settled = true;
                }
            }

            return settled;

        }

        //IP - 14/06/12 - #10386 - Warehouse & Deliveries
        private void cbTime_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Convert.ToString(cbTime.SelectedItem) == "AM")
            {
                dtDateIssued.Value = dtDateIssued.Value;
            }
            else
            {
                dtDateIssued.Value = dtDateIssued.Value.AddHours(12);
            }
        }

        private void dgRepossessedItems_MouseUp(object sender, MouseEventArgs e)
        {

            //drpDeliveryArea.SelectedValue = Convert.ToString(((DataView)dgRepossessedItems.DataSource)[dgRepossessedItems.CurrentRowIndex][CN.DeliveryArea]); ;

            //if (drpDeliveryArea.SelectedIndex != -1)
            //{
            //    errorProvider1.SetError(drpDeliveryArea, string.Empty);
            //}            

            int i = drpDeliveryAdr.FindString(Convert.ToString(((DataView)dgRepossessedItems.DataSource)[dgRepossessedItems.CurrentRowIndex][CN.DeliveryAddress]));        // #14927
            if (i != -1)
            {
                drpDeliveryAdr.SelectedIndex = i;
                drpDeliveryAdr.Enabled = true;
            }

        }

        private void chkImmediate_CheckedChanged(object sender, EventArgs e)
        {
            //if (chkImmediate.Checked)
            //{
            //    errorProvider1.SetError(drpDeliveryArea, string.Empty);         //#13926
            //}
        }
        
        //CR12249 - #12224
        private void btnDelAreas_Click(object sender, EventArgs e)
        {
            //DeliveryAreaPopup delArea = new DeliveryAreaPopup(FormRoot, FormParent, this.delAreas, drpDeliveryArea.Text);
            //delArea.ShowDialog();

            //if (delArea.SelectedDeliveryArea != string.Empty)
            //{
            //    var index = drpDeliveryArea.FindStringExact(delArea.SelectedDeliveryArea);
            //    if (index > 0) drpDeliveryArea.SelectedIndex = index;
            //}
        }

    }
}
