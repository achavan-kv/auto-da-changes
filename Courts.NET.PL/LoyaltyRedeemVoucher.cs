using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using STL.PL.WS3;
using STL.Common;

namespace STL.PL
{
    public partial class LoyaltyRedeemVoucher : CommonForm
    {
        NewAccount NewAcct_frm;

        public LoyaltyRedeemVoucher(string decimalformat,string custid, string memberno, NewAccount Nacct_frm)
        {
            try
            {
                InitializeComponent();
                NewAcct_frm = Nacct_frm;
                this.ControlBox = false;
                Custno_lbl.Text = custid;
                //  memno_lbl.Text = string.Format("{0}-{1}-{2}-{3}", memberno.Substring(0, 4), memberno.Substring(4, 4), memberno.Substring(8, 4), memberno.Substring(12, 4));
                GetVouchers(custid,decimalformat);
            }
            catch (Exception ex)
            {
                Catch(ex, "Voucher Redemption");
            }

        }

        private void GetVouchers(string custid, string decimalformat)
        {
            STL.PL.WS3.LoyaltyVoucher[] lvoucher = CustomerManager.LoyaltyGetVouchers(custid);

            List<STL.PL.WS3.LoyaltyVoucher> Lvoucher = new List<STL.PL.WS3.LoyaltyVoucher>();

            Lvoucher.AddRange(lvoucher);

            //foreach (STL.Common.LoyaltyVoucherRef usedv in NewAcct_frm.usedvouchers)
            //{
            //    for (int i = Lvoucher.Count -1; i >= 0 ; i--)
            //    {
            //        if (Lvoucher[i].voucherref == usedv.voucherno)
            //        {
            //            Lvoucher.RemoveAt(i);
            //        }
            //    }
            //}

     

            BindingList<STL.PL.WS3.LoyaltyVoucher> BVouchers = new BindingList<STL.PL.WS3.LoyaltyVoucher>(Lvoucher);
            Loyalty_Voucher_dgv.DataSource = BVouchers;

     

            Loyalty_Voucher_dgv.Columns["Custid"].Visible = false;
            Loyalty_Voucher_dgv.Columns["voucherdate"].DefaultCellStyle.Format = "dd MMM yyyy";

         


            Loyalty_Voucher_dgv.Columns["VoucherRef"].DisplayIndex = 1;
            Loyalty_Voucher_dgv.Columns["VoucherDate"].DisplayIndex = 2;
            Loyalty_Voucher_dgv.Columns["memberno"].DisplayIndex = 3;
            Loyalty_Voucher_dgv.Columns["Acctno"].DisplayIndex = 4;
            Loyalty_Voucher_dgv.Columns["VoucherValue"].DisplayIndex = 5;

            Loyalty_Voucher_dgv.Columns["VoucherValue"].DefaultCellStyle.Format = decimalformat;

            Loyalty_Voucher_dgv.Columns["memberno"].HeaderText = "Membership No.";
            Loyalty_Voucher_dgv.Columns["Acctno"].HeaderText = "Account No.";
            Loyalty_Voucher_dgv.Columns["VoucherRef"].HeaderText = "Voucher Ref";
            Loyalty_Voucher_dgv.Columns["VoucherValue"].HeaderText = "Value";
            Loyalty_Voucher_dgv.Columns["VoucherDate"].HeaderText = "Date";

            Loyalty_Voucher_dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;


            //Loyalty_Voucher_dgv.Columns["MemberNo"].DisplayIndex = 0;
            //Loyalty_Voucher_dgv.Columns["MemberNo"].HeaderText = "Membership No";

            //Loyalty_Voucher_dgv.Columns["VoucherValue"].DisplayIndex = 1;
            //Loyalty_Voucher_dgv.Columns["VoucherValue"].HeaderText = "Value";


      



        }

        private void close_btn_Click(object sender, EventArgs e)
        {
            NewAcct_frm.ClearItemDetails();
            NewAcct_frm.ClearItemCode();
            this.Close();
        }

       

        

        private void Select_btn_Click(object sender, EventArgs e)
        {
            if (Loyalty_Voucher_dgv.SelectedRows.Count > 0)
            {
                ProcessSelectedVouchers(Loyalty_Voucher_dgv.SelectedRows[0]);
            }
   
        }

        private void Loyalty_Voucher_dgv_DoubleClick(object sender, EventArgs e)
        {
            if (Loyalty_Voucher_dgv.SelectedRows.Count > 0)
            {
                ProcessSelectedVouchers(Loyalty_Voucher_dgv.SelectedRows[0]);
            }
        }

        private void ProcessSelectedVouchers(DataGridViewRow row)
        {
           
            NewAcct_frm.AddItem(row.Cells["VoucherValue"].Value.ToString());
            NewAcct_frm.RedemptionNo = Convert.ToInt32(row.Cells["VoucherRef"].Value);
            this.Close();
        }

      
       
    }
}
