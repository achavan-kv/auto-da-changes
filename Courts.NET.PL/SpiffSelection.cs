using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using STL.Common;
using System.Data;
using System.Web.Services.Protocols;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.ColumnNames;
using System.Collections.Specialized;
using System.Xml;
using STL.Common.Constants.Elements;
using STL.Common.Constants.Tags;
using Crownwood.Magic.Menus;
using STL.Common.Constants.ItemTypes;
using STL.Common.Static;


namespace STL.PL
{
    public partial class SpiffSelection : CommonForm
    {
        string location = "";
        string productCode = "";
        int? itemID = null; 
        string spiffItem = "";
        bool linkedSpiffs = false;

        public SpiffSelection()
        {
            InitializeComponent();
        }

        public SpiffSelection(Form root, Form parent, string accountType, DataView dvSpiffs)
        {
            InitializeComponent();
            FormRoot = root;
            FormParent = parent;

            if (FormParent.GetType().Name == "NewAccount")
            {
                location = ((NewAccount)this.FormParent).Location;
                productCode = ((NewAccount)this.FormParent).ProductCode;
                itemID = ((NewAccount)this.FormParent).ItemID;
            }

            LoadData(dvSpiffs, accountType);
        }

        public SpiffSelection(Form root, Form parent, DataView dvLinkedSpiffs, string accountType)
        {
            InitializeComponent();
            FormRoot = root;
            FormParent = parent;

            btnOK.Visible = true;
            btnReplace.Visible = false;
            btnCancel.Visible = false;
            linkedSpiffs = true;

            grpSpiffs.Text = "Linked Spiff Items";

            LoadLinkedSpiffs(dvLinkedSpiffs, accountType);
        }

        private void LoadData(DataView dvSpiffs, string accountType)
        {
            dgvSpiffs.DataSource = dvSpiffs;

            // Displayed columns
            dgvSpiffs.Columns[CN.ItemText].HeaderText = GetResource("T_ITEMNO");
            dgvSpiffs.Columns[CN.ItemText].ReadOnly = true;
            dgvSpiffs.Columns[CN.ItemText].Width = 80;

            dgvSpiffs.Columns[CN.ItemDescription].HeaderText = GetResource("T_ITEM_DESCRIPTION");
            dgvSpiffs.Columns[CN.ItemDescription].ReadOnly = true;
            dgvSpiffs.Columns[CN.ItemDescription].Width = 250;

            if (AT.IsCashType(accountType))
            {
                dgvSpiffs.Columns[CN.CashPrice].HeaderText = GetResource("T_PRICE");
                dgvSpiffs.Columns[CN.CashPrice].ReadOnly = true;
                dgvSpiffs.Columns[CN.CashPrice].Width = 80;
                dgvSpiffs.Columns[CN.CashPrice].DefaultCellStyle.Format = DecimalPlaces;
                dgvSpiffs.Columns[CN.CashPrice].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvSpiffs.Columns[CN.UnitPrice].Visible = false;
            }
            else
            {
                dgvSpiffs.Columns[CN.UnitPrice].HeaderText = GetResource("T_PRICE");
                dgvSpiffs.Columns[CN.UnitPrice].ReadOnly = true;
                dgvSpiffs.Columns[CN.UnitPrice].Width = 80;
                dgvSpiffs.Columns[CN.UnitPrice].DefaultCellStyle.Format = DecimalPlaces;
                dgvSpiffs.Columns[CN.UnitPrice].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvSpiffs.Columns[CN.CashPrice].Visible = false;
            }

            dgvSpiffs.Columns[CN.Value].HeaderText = GetResource("T_SPIFFVALUE");
            dgvSpiffs.Columns[CN.Value].ReadOnly = true;
            dgvSpiffs.Columns[CN.Value].DefaultCellStyle.Format = DecimalPlaces;
            dgvSpiffs.Columns[CN.Value].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight; 
            dgvSpiffs.Columns[CN.Value].Width = 70;

            dgvSpiffs.Columns[CN.Percentage].HeaderText = GetResource("T_SPIFFPERCENTAGE");
            dgvSpiffs.Columns[CN.Percentage].ReadOnly = true;
            dgvSpiffs.Columns[CN.Percentage].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvSpiffs.Columns[CN.Percentage].Width = 75;

            dgvSpiffs.Columns[CN.ItemId].Width = 0;         // RI

            dgvSpiffs.ReadOnly = true;
            dgvSpiffs.MultiSelect = false;
            dgvSpiffs.AllowUserToAddRows = false;
        }
        // jec 02/07/08 UAT440 add accounttype
        private void LoadLinkedSpiffs(DataView dvLinkedSpiffs, string accountType)
        {
            dgvSpiffs.DataSource = dvLinkedSpiffs;

            // Displayed columns
            dgvSpiffs.Columns[CN.ItemNo].HeaderText = GetResource("T_ITEMNO");
            dgvSpiffs.Columns[CN.ItemNo].ReadOnly = true;
            dgvSpiffs.Columns[CN.ItemNo].Width = 70;

            dgvSpiffs.Columns[CN.ItemDescription].HeaderText = GetResource("T_ITEM_DESCRIPTION");
            dgvSpiffs.Columns[CN.ItemDescription].ReadOnly = true;
            dgvSpiffs.Columns[CN.ItemDescription].Width = 300;

            // Include price - jec 02/07/08 UAT440

            if (AT.IsCashType(accountType))
            {
                dgvSpiffs.Columns[CN.CashPrice].HeaderText = GetResource("T_PRICE");
                dgvSpiffs.Columns[CN.CashPrice].ReadOnly = true;
                dgvSpiffs.Columns[CN.CashPrice].Width = 80;
                dgvSpiffs.Columns[CN.CashPrice].DefaultCellStyle.Format = DecimalPlaces;
                dgvSpiffs.Columns[CN.UnitPrice].Visible = false;
            }
            else
            {
                dgvSpiffs.Columns[CN.UnitPrice].HeaderText = GetResource("T_PRICE");
                dgvSpiffs.Columns[CN.UnitPrice].ReadOnly = true;
                dgvSpiffs.Columns[CN.UnitPrice].Width = 80;
                dgvSpiffs.Columns[CN.UnitPrice].DefaultCellStyle.Format = DecimalPlaces;
                dgvSpiffs.Columns[CN.CashPrice].Visible = false;
            }
            
            dgvSpiffs.Columns[CN.Value].HeaderText = GetResource("T_SPIFFVALUE");
            dgvSpiffs.Columns[CN.Value].ReadOnly = true;
            dgvSpiffs.Columns[CN.Value].DefaultCellStyle.Format = DecimalPlaces;
            dgvSpiffs.Columns[CN.Value].Width = 80;

            dgvSpiffs.Columns[CN.Percentage].HeaderText = GetResource("T_SPIFFPERCENTAGE");
            dgvSpiffs.Columns[CN.Percentage].ReadOnly = true;
            dgvSpiffs.Columns[CN.Percentage].Width = 80;

            dgvSpiffs.Columns[CN.ItemId].Width = 0;         // RI 

            dgvSpiffs.ReadOnly = true;
            dgvSpiffs.MultiSelect = false;
            dgvSpiffs.AllowUserToAddRows = false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (FormParent.GetType().Name == "NewAccount")
            {
                ((NewAccount)this.FormParent).DisplaySpiffs = false;
                ((NewAccount)this.FormParent).ClearItemDetails();
                ((NewAccount)this.FormParent).ProductCode = productCode;
                ((NewAccount)this.FormParent).ItemID = itemID;
                ((NewAccount)this.FormParent).Location = location;
                ((NewAccount)FormParent).drpLocation_Validating(this, new CancelEventArgs());
                ((NewAccount)this.FormParent).DisplaySpiffs = true;
            }

            Close();
        }

        private void btnReplace_Click(object sender, EventArgs e)
        {
            if (FormParent.GetType().Name == "NewAccount")
            {
                ((NewAccount)this.FormParent).DisplaySpiffs = false;
                ((NewAccount)this.FormParent).ClearItemDetails();
                ((NewAccount)this.FormParent).Location = location;
                ((NewAccount)FormParent).drpLocation_Validating(this, new CancelEventArgs());
                ((NewAccount)this.FormParent).DisplaySpiffs = true;
            }

            if (FormParent.GetType().Name == "RelatedProducts")
                ((RelatedProducts)this.FormParent).ItemNo = spiffItem;

            Close();
        }

        private void dgvSpiffs_MouseUp(object sender, MouseEventArgs e)
        {
            if (!linkedSpiffs)
            {
                int index = dgvSpiffs.CurrentRow.Index;
                if (index >= 0)
                {
                    spiffItem = Convert.ToString(dgvSpiffs[CN.ItemText, index].Value);

                    if (FormParent.GetType().Name == "NewAccount")
                    {
                        ((NewAccount)this.FormParent).ProductCode = spiffItem;
                        ((NewAccount)this.FormParent).ItemID = Convert.ToInt32(dgvSpiffs[CN.ItemId, index].Value); // RI
                    }
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}