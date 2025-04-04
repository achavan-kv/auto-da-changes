using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using STL.Common;
using System.Data;
using STL.PL.WS1;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.ColumnNames;
using System.Xml;
using STL.Common.Constants.Elements;
using STL.Common.Constants.Tags;
using Crownwood.Magic.Menus;
using STL.Common.Static;
using System.Collections.Generic;
using STL.Common.Services.Model;

namespace STL.PL
{
    /// <summary>
    /// Popup prompt that lists all the warranties that are suitable for a particular
    /// stock item. This popup is used when a warrantable stock item is added to a 
    /// customer order. The user can select a warranty from the list. Each warranty 
    /// must have a unique contract number. This will either be automatically 
    /// generated by the application, or entered by the user. Automatic contract
    /// number generation is controlled by a country parameter.
    /// </summary>
    public class Warranties : CommonForm
    {
        private System.Windows.Forms.DataGridView dgWarranties;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnBuy;
        private System.Windows.Forms.Label message;
        private DataTable _contracts = null;
        private new string Error;
        private double _qty = 0;
        public double Quantity
        {
            get { return _qty; }
            set { _qty = value; }
        }
        //	int oldCurrentRow;
        short _location;
        string AccountNo;
        int AgreementNo = 1;
        XmlNode node = null;
        new XmlUtilities xml;
        private System.Windows.Forms.Button btnShow;
        private System.Windows.Forms.DataGrid dgContracts;
        private System.Windows.Forms.CheckBox chxCredit;
        private Control allowCashAndGoCredit;
        private Control allowNewAccountCredit;
        private string acctType = "";
        private bool _manualCDV = false;
        private Button btnRemove;
        private Button btnEnter;
        private IContainer components;
        public int selectedWarrantyId { get; set; }
        public List<string> selectedContracts { get; set; }

        public Warranties(TranslationDummy d)
        {
            InitializeComponent();
        }

        private void HashMenus()
        {
            dynamicMenus = new Hashtable();
            dynamicMenus[this.Name + ":allowCashAndGoCredit"] = this.allowCashAndGoCredit;
            dynamicMenus[this.Name + ":allowNewAccountCredit"] = this.allowNewAccountCredit;
        }

        public Warranties(IList<WarrantyItemXml> warranties, double quantity, XmlNode currentItem, short location, string accountNo, int agreementNo, System.Windows.Forms.Form parent, Form root, bool isIR)
        {
            InitializeComponent();
            TranslateControls();

            FormParent = parent;
            FormRoot = root;
            Quantity = quantity;
            _location = location;
            node = currentItem;
            AccountNo = accountNo;
            AgreementNo = agreementNo;

            var newAccount = (NewAccount)parent;

            chxCredit.Checked = newAccount.WarrantiesOnCredit;
            chxCredit.Enabled = newAccount.AccountType == AT.Cash || newAccount.PaidAndTaken;

            _manualCDV = AccountManager.ManualCDVExists(AccountNo, out Error);
            if (Error.Length > 0) ShowError(Error);

            if (chxCredit.Enabled)
            {
                HashMenus();
                ApplyRoleRestrictions();

                chxCredit.Enabled = (newAccount.PaidAndTaken && allowCashAndGoCredit.Enabled) ||
                                    (newAccount.AccountType == AT.Cash && allowNewAccountCredit.Enabled);
            }

            if (isIR && chxCredit.Enabled)
            {
                chxCredit.Enabled = newAccount.PaidAndTaken && (bool)Country[CountryParameterNames.ReplacementCredit];
            }

            acctType = newAccount.AccountType;

            xml = new XmlUtilities();

         
            dgWarranties.DataSource = warranties;
            dgWarranties.Columns["Id"].Visible = false;
            dgWarranties.Columns["BranchForDeliveryNote"].Visible = false;
            dgWarranties.Columns["DeliveryTime"].Visible = false;
            dgWarranties.Columns["DeliveryDate"].Visible = false;
            dgWarranties.Columns["Quantity"].Visible = false;
            dgWarranties.Columns["Description"].Width = 150;
            dgWarranties.Columns["Description"].HeaderText = GetResource("T_DESCRIPTION");
            dgWarranties.Columns["Length"].Width = 50;
            dgWarranties.Columns["Length"].HeaderText = GetResource("T_DURATION");
            dgWarranties.Columns["Location"].Visible = false;
            dgWarranties.Columns["Value"].Visible = false;
            dgWarranties.Columns["WarrantyType"].Visible = false;               //#17883
            dgWarranties.Columns["Code"].HeaderText = "Warranty Code";
            dgWarranties.Columns["ContractNumber"].Visible = false;
            //tabStyle.GridColumnStyles["HP Price"].HeaderText = GetResource("T_HPPRICE");
            //tabStyle.GridColumnStyles["Cash Price"].HeaderText = GetResource("T_CASHPRICE");

            //if (AT.IsCreditType(((NewAccount)FormParent).AccountType))
            //{
            //    tabStyle.GridColumnStyles["HP Price"].Width = 50;
            //    tabStyle.GridColumnStyles["HP Price"].Alignment = HorizontalAlignment.Right;
            //    tabStyle.GridColumnStyles["Cash Price"].Width = 0;
            //}
            //else
            //{
            //    tabStyle.GridColumnStyles["HP Price"].Width = 0;
            //    tabStyle.GridColumnStyles["Cash Price"].Width = 50;
            //    tabStyle.GridColumnStyles["Cash Price"].Alignment = HorizontalAlignment.Right;
            //}

            //tabStyle.GridColumnStyles["Location"].Width = 50;     // RI
            //tabStyle.GridColumnStyles[CN.Code].Width = 50;     // RI
            //tabStyle.GridColumnStyles[CN.Status].Width = 50;     // RI
            //tabStyle.GridColumnStyles["IUPC"].Width = 90;     // RI
            //tabStyle.GridColumnStyles[CN.ItemId].Width = 0;     // RI

            _contracts = new DataTable();
            _contracts.Columns.AddRange(new DataColumn[] { new DataColumn(CN.ContractNo), new DataColumn(CN.ReadOnly) });

            dgContracts.DataSource = _contracts.DefaultView;
            _contracts.DefaultView.AllowDelete = false;
            _contracts.DefaultView.AllowNew = false;
            _contracts.DefaultView.AllowEdit = !((bool)Country[CountryParameterNames.AutomaticWarrantyNo]) || ((NewAccount)FormParent).ManualSale || this._manualCDV || (acctType == AT.ReadyFinance);

            var tabStyle = new DataGridTableStyle();
            tabStyle.MappingName = _contracts.TableName;

            AddColumnStyle(CN.ReadOnly, tabStyle, 0, true, "", "", HorizontalAlignment.Left);

            DataGridEditColumn aColumnEditColumn = new DataGridEditColumn(CN.ReadOnly, "Y");
            aColumnEditColumn.MappingName = CN.ContractNo;
            aColumnEditColumn.HeaderText = GetResource("T_CONTRACTNO");
            aColumnEditColumn.Width = 136;
            aColumnEditColumn.ReadOnly = false;
            aColumnEditColumn.NullText = "";
            tabStyle.GridColumnStyles.Add(aColumnEditColumn);

            dgContracts.TableStyles.Add(tabStyle);

            GetExistingContracts(0);

            message.Text = GetResource("M_ADDWARRANTY", new Object[] { quantity });

            CheckForDiscount(currentItem);
        }

        private void CheckForDiscount(XmlNode item)
        {
            /* if the current item has a discount attached, then enable the show all button */
            string xpath = "//Item[@Type='Discount' and @Quantity!='0']";
            XmlNodeList discounts = item.SelectNodes(xpath);
            btnShow.Enabled = discounts.Count > 0;
        }

        private void GetExistingContracts(int index)
        {
            string warrItem = ((IList<WarrantyItemXml>)dgWarranties.DataSource)[index].Code;
            string xpath = Elements.RelatedItem + "/" + Elements.Item + "[@" +
                            Tags.Code + "='" + warrItem + "' and @Quantity != '0']";
            XmlNodeList contracts = node.SelectNodes(xpath);
            if (contracts != null)
            {
                foreach (XmlNode child in contracts)
                {
                    DataRow r = _contracts.NewRow();

                    r[CN.ContractNo] = child.Attributes[Tags.ContractNumber].Value;
                    r[CN.ReadOnly] = acctType != AT.ReadyFinance ? "Y" : "N"; //IP/JC - 14/04/10 - UAT(80) UAT5.2 

                    _contracts.Rows.Add(r);
                }
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Warranties));
            this.allowCashAndGoCredit = new System.Windows.Forms.Control();
            this.allowNewAccountCredit = new System.Windows.Forms.Control();
            this.dgWarranties = new System.Windows.Forms.DataGridView();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnEnter = new System.Windows.Forms.Button();
            this.chxCredit = new System.Windows.Forms.CheckBox();
            this.dgContracts = new System.Windows.Forms.DataGrid();
            this.btnShow = new System.Windows.Forms.Button();
            this.message = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnBuy = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgWarranties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgContracts)).BeginInit();
            this.SuspendLayout();
            // 
            // allowCashAndGoCredit
            // 
            this.allowCashAndGoCredit.Enabled = false;
            this.allowCashAndGoCredit.Location = new System.Drawing.Point(0, 0);
            this.allowCashAndGoCredit.Name = "allowCashAndGoCredit";
            this.allowCashAndGoCredit.Size = new System.Drawing.Size(0, 0);
            this.allowCashAndGoCredit.TabIndex = 0;
            // 
            // allowNewAccountCredit
            // 
            this.allowNewAccountCredit.Enabled = false;
            this.allowNewAccountCredit.Location = new System.Drawing.Point(0, 0);
            this.allowNewAccountCredit.Name = "allowNewAccountCredit";
            this.allowNewAccountCredit.Size = new System.Drawing.Size(0, 0);
            this.allowNewAccountCredit.TabIndex = 0;
            // 
            // dgWarranties
            // 
            this.dgWarranties.AllowUserToAddRows = false;
            this.dgWarranties.AllowUserToDeleteRows = false;
            this.dgWarranties.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgWarranties.Location = new System.Drawing.Point(8, 8);
            this.dgWarranties.Name = "dgWarranties";
            this.dgWarranties.ReadOnly = true;
            this.dgWarranties.Size = new System.Drawing.Size(320, 136);
            this.dgWarranties.TabIndex = 3;
            this.dgWarranties.Click += new System.EventHandler(this.dgWarranties_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.Icon = ((System.Drawing.Icon)(resources.GetObject("errorProvider1.Icon")));
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnRemove);
            this.groupBox1.Controls.Add(this.btnEnter);
            this.groupBox1.Controls.Add(this.chxCredit);
            this.groupBox1.Controls.Add(this.dgContracts);
            this.groupBox1.Controls.Add(this.btnShow);
            this.groupBox1.Controls.Add(this.message);
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.btnBuy);
            this.groupBox1.Location = new System.Drawing.Point(8, 152);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(320, 160);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            // 
            // btnRemove
            // 
            this.btnRemove.BackColor = System.Drawing.Color.SlateBlue;
            this.btnRemove.Font = new System.Drawing.Font("Arial Narrow", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRemove.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.btnRemove.Image = ((System.Drawing.Image)(resources.GetObject("btnRemove.Image")));
            this.btnRemove.Location = new System.Drawing.Point(204, 86);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(22, 22);
            this.btnRemove.TabIndex = 147;
            this.btnRemove.UseVisualStyleBackColor = false;
            this.btnRemove.Click += new System.EventHandler(this.DeleteContract_Click);
            // 
            // btnEnter
            // 
            this.btnEnter.BackColor = System.Drawing.Color.SlateBlue;
            this.btnEnter.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEnter.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.btnEnter.Image = ((System.Drawing.Image)(resources.GetObject("btnEnter.Image")));
            this.btnEnter.Location = new System.Drawing.Point(204, 56);
            this.btnEnter.Name = "btnEnter";
            this.btnEnter.Size = new System.Drawing.Size(22, 22);
            this.btnEnter.TabIndex = 146;
            this.btnEnter.UseVisualStyleBackColor = false;
            this.btnEnter.Click += new System.EventHandler(this.btnEnter_Click);
            // 
            // chxCredit
            // 
            this.chxCredit.Location = new System.Drawing.Point(224, 16);
            this.chxCredit.Name = "chxCredit";
            this.chxCredit.Size = new System.Drawing.Size(88, 24);
            this.chxCredit.TabIndex = 7;
            this.chxCredit.Text = "buy on credit";
            // 
            // dgContracts
            // 
            this.dgContracts.CaptionVisible = false;
            this.dgContracts.DataMember = "";
            this.dgContracts.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgContracts.Location = new System.Drawing.Point(16, 56);
            this.dgContracts.Name = "dgContracts";
            this.dgContracts.Size = new System.Drawing.Size(184, 96);
            this.dgContracts.TabIndex = 11;
            this.dgContracts.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgContracts_MouseUp);
            // 
            // btnShow
            // 
            this.btnShow.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnShow.Location = new System.Drawing.Point(248, 120);
            this.btnShow.Name = "btnShow";
            this.btnShow.Size = new System.Drawing.Size(56, 23);
            this.btnShow.TabIndex = 10;
            this.btnShow.Text = "Show All";
            this.btnShow.Click += new System.EventHandler(this.btnShow_Click);
            // 
            // message
            // 
            this.message.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.message.Location = new System.Drawing.Point(16, 8);
            this.message.Name = "message";
            this.message.Size = new System.Drawing.Size(192, 40);
            this.message.TabIndex = 9;
            this.message.Text = "Enter a contract number for each item purchased.";
            // 
            // btnCancel
            // 
            this.btnCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnCancel.Location = new System.Drawing.Point(248, 88);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(56, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnBuy
            // 
            this.btnBuy.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnBuy.Location = new System.Drawing.Point(248, 56);
            this.btnBuy.Name = "btnBuy";
            this.btnBuy.Size = new System.Drawing.Size(56, 23);
            this.btnBuy.TabIndex = 6;
            this.btnBuy.Text = "Buy";
            this.btnBuy.Click += new System.EventHandler(this.btnBuy_Click);
            // 
            // Warranties
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(338, 314);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.dgWarranties);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Warranties";
            this.ShowInTaskbar = false;
            ((System.ComponentModel.ISupportInitialize)(this.dgWarranties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgContracts)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        private void btnBuy_Click(object sender, System.EventArgs e)
        {
            bool valid = true;
            int count = 0;
            selectedContracts = new List<string>();

            //Validate all contract no's so they can't save if there are errors
            foreach (DataRowView row in (DataView)dgContracts.DataSource)
            {
                valid = ValidateContracts(row["contractno"]);
                count++;
                selectedContracts.Add(row["contractno"].ToString());
            }

            if (valid && count > 0)
            {
                /* Don't need to check schedules for warranties, they will be automatically collected if necessary */
                //if(((NewAccount)this.FormParent).CheckSchedules(this.AccountNo, (string)dgWarranties[dgWarranties.CurrentRowIndex, 0], _location, _contracts.DefaultView.Count, IT.Warranty))
                //{
                errorProvider1.SetError(dgContracts, "");
                //((NewAccount)this.FormParent).Warranty = (string)dgWarranties[dgWarranties.CurrentRowIndex, ""];
                //((NewAccount)this.FormParent).WarrantyId =  Convert.ToInt32(dgWarranties[dgWarranties.CurrentRowIndex, 8]);
                ((NewAccount)this.FormParent).ContractNos = _contracts;
                ((NewAccount)FormParent).WarrantiesOnCredit = chxCredit.Checked;
                selectedWarrantyId = Convert.ToInt32(dgWarranties.CurrentRow.Cells["Id"].Value);
                Close();
                //}
            }
        }

        private bool ValidateContracts(object txt)
        {
            string msg = "";
            bool valid = true;
            string text = "";

            if (txt == DBNull.Value)
            {
                valid = false;
                msg = "You must enter a contract number";
            }
            else
            {
                text = (string)txt;
            }

            if (valid)
            {
                if (text.Length > 10)
                {
                    valid = false;
                    msg = "Contract number must be less than 11 characters.";
                }
            }

            //Make sure there aren't too many
            if (valid)
            {
                if (_contracts.DefaultView.Count > Quantity)
                {
                    valid = false;
                    msg = "You have entered too many contract numbers.";
                }
            }

            if (valid)
            {
                /* if the contract no has not been automatically generated
                 * then we need to make sure it's unique */
                //IP-17/10/2007 UAT(343)contract number should be validated when the below country
                //parameter is true or false, as the user can overide the automatically generated contract number
                //and enter their own.

                //-if(!(bool)Country[CountryParameterNames.AutomaticWarrantyNo])
                //IP -17/10/2007 UAT(343){

                /* 1) check the items XmlDocument and see if there
                 * are any contract nodes with this contract number 
                 * 2) if found see if it's for the same item and stocklocn 
                 * if it is then that's OK, otherwise it's not unique */

                //string xpath = "//"+Elements.ContractNo+"[@"+Tags.ContractNumber+" = '"+text+"']";

                string xpath = "//Item[@ContractNumber='" + text + "']";
                XmlNode duplicate = ((NewAccount)FormParent).LineItems.SelectSingleNode(xpath);

                if (duplicate != null)
                {
                    string waritemno = dgWarranties.CurrentRow.Cells["Code"].Value.ToString();
                    if ((Convert.ToInt16(duplicate.Attributes[Tags.Location].Value) != this._location ||
                        duplicate.Attributes[Tags.Code].Value != waritemno) &&
                        duplicate.Attributes[Tags.Quantity].Value != "0")
                    {
                        /* this is a duplicate and not allowed */
                        valid = false;
                        msg = "Contract number " + text + " used elsewhere on this account";
                    }
                }

                if (valid)
                {
                    /* check the database and make sure this contract no
                     * hasn't been used on another account */
                    bool unique = false;
                    AccountManager.ContractNoUnique(this.AccountNo, AgreementNo, text, out unique, out Error);
                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                    {
                        if (!unique)
                        {
                            valid = false;
                            msg = "Contract number " + text + " is used on another account";
                        }
                    }
                }
                //IP-17/10/2007 (UAT343)}
            }

            if (!valid)
            {
                //	dgContracts.CurrentCell = new DataGridCell(oldCurrentRow, 0);
                errorProvider1.SetError(dgContracts, msg);
            }
            else
            {
                errorProvider1.SetError(dgContracts, "");
            }
            return valid;
        }

        private void dgWarranties_Click(object sender, System.EventArgs e)
        {

            if (dgWarranties.CurrentRow.Index >= 0)
            {
                _contracts.Clear();
                this.GetExistingContracts(dgWarranties.CurrentRow.Index);
            }
        }

        private void dgContracts_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            int index = dgContracts.CurrentRowIndex;
            if (index >= 0)	//may be empty
            {
                if (e.Button == MouseButtons.Right)
                {
                    DataGrid ctl = (DataGrid)sender;

                    MenuCommand m1 = new MenuCommand(GetResource("P_DELETE"));
                    m1.Click += new System.EventHandler(this.DeleteContract_Click);

                    PopupMenu popup = new PopupMenu();
                    popup.Animate = Animate.Yes;
                    popup.AnimateStyle = Animation.SlideHorVerPositive;
                    popup.MenuCommands.Add(m1);
                    MenuCommand selected = popup.TrackPopup(ctl.PointToScreen(new Point(e.X, e.Y)));
                }
            }
        }

        private void DeleteContract_Click(object sender, System.EventArgs e)
        {
            try
            {
                int index = dgContracts.CurrentRowIndex;
                int index2 = dgWarranties.CurrentRow.Index;

                if (index >= 0)
                {
                    //if((string)((DataView)dgContracts.DataSource)[index][CN.ReadOnly] == "Y") //IP/JC - 14/04/10 - UAT(80) UAT5.2 Removed
                    //{
                    /* must set the associated item qty to zero otherwise the warranty will
                     * not be collected (if necessary) */
                    string warrItem = ((IList<WarrantyItemXml>)dgWarranties.DataSource)[index2].Code;
                    string contractNo = ((DataView)dgContracts.DataSource)[index]["contractno"].ToString();
                    //string xpath = "RelatedItems/Item[@Code='"+warrItem+"' and @Quantity!='0' and @ContractNumber='"+contractNo+"']";
                    string xpath = "//Item[@Code='" + warrItem + "' and @Quantity!='0' and @ContractNumber='" + contractNo + "']";

                    //XmlNodeList contracts = node.SelectNodes(xpath);	/* there should be at most one */
                    XmlNodeList contracts = ((NewAccount)FormParent).itemDoc.DocumentElement.SelectNodes(xpath);
                    if (contracts != null)
                    {
                        foreach (XmlNode child in contracts)
                        { 
                            child.Attributes[Tags.Quantity].Value = "0";
                            child.Attributes[Tags.ParentItemId].Value = "0";
                            child.Attributes[Tags.ParentItemNo].Value = string.Empty;
                        }
                            
                    }
                    //}
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
        }

        private void btnShow_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();

                //if (dgWarranties.VisibleRowCount > 0 && dgWarranties.VisibleColumnCount > 0)
                //{
                    string code = dgWarranties[0, 5].ToString();
                    short location = Convert.ToInt16(dgWarranties[0, 6]);
                    dgWarranties.DataSource = null;
                    DataSet ds = AccountManager.GetProductWarranties(0, location,
                                                                    0, code,
                                                                    ((NewAccount)FormParent).PaidAndTaken,
                                                                    out Error);
                    if (Error.Length > 0)
                        ShowError(Error);
                    dgWarranties.DataSource = ds.Tables["Warranties"];
                //}
            }
            catch (Exception ex)
            {
                Catch(ex, "btnShow_Click");
            }
            finally
            {
                StopWait();
            }
        }

        private void btnEnter_Click(object sender, System.EventArgs e)
        {
            try
            {
                Function = "btnEnter_Click";
                Wait();

                if (_contracts.Rows.Count < Quantity)
                {
                    DataRow row = _contracts.NewRow();
                    row[CN.ReadOnly] = "N";
                    if ((bool)Country[CountryParameterNames.AutomaticWarrantyNo])
                    {
                        string contractNo = AccountManager.AutoWarranty(Config.BranchCode, out Error);
                        if (Error.Length > 0)
                            ShowError(Error);
                        else
                        {
                            row[CN.ContractNo] = contractNo;

                            if (acctType != AT.ReadyFinance && !((NewAccount)FormParent).ManualSale && !this._manualCDV)
                                row[CN.ReadOnly] = "Y";
                        }
                    }
                    _contracts.Rows.Add(row);
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
    }
}
