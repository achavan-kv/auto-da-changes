using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Blue.Cosacs.Shared;
using STL.PL.Utils;
using Blue.Cosacs.Shared.Services;
using Blue.Cosacs.Shared.Services.StoreCard;

namespace STL.PL.StoreCard
{
    public partial class StoreCardTransactions : UserControl
    {
        public StoreCardTransactions()
        {
            InitializeComponent();
            SetupDGVs();
            
        }
        public void Setup(List<view_FintranswithTransfers> fin)
        {
            dgv_Transactions.DataSource = fin;
        }

        private void SetupDGVs()
        {
            dgv_Transactions.ColumnStyleInit();
            dgv_Transactions.ColumnStylePreLoad("BranchNo", null, 45, null, DataGridViewColumnSortMode.Automatic, DataGridViewContentAlignment.MiddleLeft);
            dgv_Transactions.ColumnStylePreLoad("Description", null, 135, null, DataGridViewColumnSortMode.Automatic, DataGridViewContentAlignment.MiddleLeft);
            dgv_Transactions.ColumnStylePreLoad("DateTrans", "Date", 135, null, DataGridViewColumnSortMode.Automatic, DataGridViewContentAlignment.MiddleLeft);
            dgv_Transactions.ColumnStylePreLoad("Value",null, 135, null, DataGridViewColumnSortMode.Automatic, DataGridViewContentAlignment.MiddleLeft,"n2");
            dgv_Transactions.ColumnStylePreLoad("TransferAccount", "For Account", 100, null, DataGridViewColumnSortMode.Automatic, DataGridViewContentAlignment.MiddleLeft);
            dgv_Transactions.ColumnStylePreLoad("agrmtno", "Invoice Ref", 100, null, DataGridViewColumnSortMode.Automatic, DataGridViewContentAlignment.MiddleLeft);
            dgv_Transactions.ColumnStylePreLoad("Name");
            dgv_Transactions.ColumnStylePreLoad("CardNumber", null, 75, null, DataGridViewColumnSortMode.Automatic, DataGridViewContentAlignment.MiddleLeft);
 
            dgvSales.ColumnStyleInit();
            dgvSales.ColumnStylePreLoad("ItemNo", null, 150, null, DataGridViewColumnSortMode.Automatic, DataGridViewContentAlignment.MiddleLeft);
            dgvSales.ColumnStylePreLoad("quantity", "Quantity", 45, null, DataGridViewColumnSortMode.Automatic, DataGridViewContentAlignment.MiddleLeft);
            dgvSales.ColumnStylePreLoad("Value", null, 150, null, DataGridViewColumnSortMode.Automatic, DataGridViewContentAlignment.MiddleLeft,"n2");
            dgvSales.ColumnStylePreLoad("Description1", null, 180, null, DataGridViewColumnSortMode.Automatic, DataGridViewContentAlignment.MiddleLeft);
            dgvSales.ColumnStylePreLoad("Description2", null, 250, null, DataGridViewColumnSortMode.Automatic, DataGridViewContentAlignment.MiddleLeft);
            dgvSales.ColumnStylePreLoad("SoldBy", null, 180, null, DataGridViewColumnSortMode.Automatic, DataGridViewContentAlignment.MiddleLeft);
            dgvSales.ColumnStylePreLoad("ContractNo");
            
        }

        private void dgv_Transactions_Click(object sender, EventArgs e)
        {
            if (dgv_Transactions.CurrentRow != null && dgv_Transactions.CurrentRow.Cells["TransferAccount"].Value != null)
            {
                Client.Call(new GetDeliveryDetailsRequest
                {
                    AcctNo = dgv_Transactions.CurrentRow.Cells["TransferAccount"].Value.ToString(),
                    Agrmtno = Convert.ToInt32(dgv_Transactions.CurrentRow.Cells["agrmtno"].Value)
                },
                response =>
                {
                    dgvSales.DataSourceSafe(response.Details);
                }, this);
            }
            else
            {
                dgvSales.DataSource = null;
            }
        }

    }
}
