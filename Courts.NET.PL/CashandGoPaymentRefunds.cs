using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using STL.PL.Utils;
using Blue.Cosacs.Shared;

namespace STL.PL
{
    public partial class PaymentReturn : Form
    {
        private decimal returnTarget = 0;
        public decimal ReturnTarget 
        {
            get { return returnTarget; }
            set
            {
                returnTarget = value; 
                txtTarget.Text = value.ToString();
                SetReturnTotal(CalcReturn());
               
            }
        }

        public List<View_FintransPayMethod> Payments 
        {
            get { return (List <View_FintransPayMethod>)dgv_refunds.DataSource; } 
        }

        public bool Complete
        {
            get
            {
                if (returnTarget - CalcReturn() == 0 && !Error())
                {
                    lbl_message.Visible = false;
                    return true;
                }
                else
                {
                    lbl_message.Visible = true;
                    return false;
                }
            }
        }

        public PaymentReturn(List<View_FintransPayMethod> Payments, decimal returnTarget)
        {
            InitializeComponent();

            this.returnTarget = returnTarget;
            txtTarget.Text = returnTarget.ToString();
            
            dgv_refunds.ColumnStyleInit();
            dgv_refunds.ColumnStylePreLoad("PayMethodDesc", "PaymentType", readOnly: true)
            .ColumnStylePreLoad("bankacctno", "BankAcctNo", readOnly: true)
            .ColumnStylePreLoad("bankcode", "BankCode", readOnly: true)
            .ColumnStylePreLoad("chequeno", "ChequeNo", readOnly: true)
            .ColumnStylePreLoad("storecardno", "StoreCardNo", readOnly: true)
            .ColumnStylePreLoad("transvalue", "Value", readOnly: true)
            .ColumnStylePreLoad("ReturnValue", "ReturnValue", width: 100);
            dgv_refunds.DataSource = Payments;
            dgv_refunds.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(dgv_refunds_EditingControlShowing);
            SetReturnTotal(CalcReturn());
        }

        void dgv_refunds_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is TextBox)
            {
                TextBox tb = e.Control as TextBox;
                tb.KeyPress += new KeyPressEventHandler(tb_KeyPress);
            }
        }

        void tb_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar) ||
                  e.KeyChar == (Char)Keys.Decimal ||
                  e.KeyChar == (Char)Keys.Back ||
                  e.KeyChar == (Char)Keys.Delete))
            {
                e.Handled = true;
            }
        }

        private void dgv_refunds_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
               var cell = dgv_refunds[e.ColumnIndex,e.RowIndex];
            if (Convert.ToDecimal(dgv_refunds["transvalue", e.RowIndex].Value) + Convert.ToDecimal(cell.Value) > 0)
            {
                cell.ErrorText = "Value exceeds available returns";
            }
            else
            {
                cell.ErrorText = "";
              
            } 
            SetReturnTotal(CalcReturn());
        }

        public bool Error()
        {
            return Payments.FindAll(p => p.ReturnValue + p.transvalue > 0).Count > 0;
        }

        private decimal CalcReturn()
        {
            decimal sum = 0;

            Payments.FindAll(p =>
            {
                sum += p.ReturnValue.HasValue?p.ReturnValue.Value:0;
                return true;
            });
            return sum;
        }

        private void SetReturnTotal(decimal returnTotal)
        { 
             txtAllocated.Text = returnTotal.ToString();
             txtNotAllocated.Text = (returnTarget - returnTotal).ToString();
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgv_refunds_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            dgv_refunds[e.ColumnIndex, e.RowIndex].ErrorText = "This is not a valid value.";
            dgv_refunds[e.ColumnIndex, e.RowIndex].Value = 0m;
        }
    }
}
 
      
 
       