using System;
using System.Data;
using System.Windows.Forms;

namespace STL.PL
{
    public partial class AddSpiff : CommonForm
    {
        const string screen = "NewAccount";
        const string control = "addAdditionalSpiff";
        string acctno;
        string itemno;
        short stockLocn;
        int agreementno;
        int salesPerson;

        public bool SpiffAdded { get; set; }

        public AddSpiff(string acctno, string itemno, short stockLocn, int agreementno, int salesPerson)
        {
            InitializeComponent();
            this.acctno = acctno;
            this.itemno = itemno;
            this.stockLocn = stockLocn;
            this.agreementno = agreementno;
            this.salesPerson = salesPerson;
        }


        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAuthorise_Click(object sender, EventArgs e)
        {
            ProcessAuthorise();
        }

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                ProcessAuthorise();
            }
        }

        private void ProcessAuthorise()
        {
            string err;
            if (Valid())
            {
                var id = StaticDataManager.ControlPermissionPasswordCheck(txtUser.Text, txtPassword.Text, screen, control);
                if (id.HasValue)
                {
                    AccountManager.AddAdditionalSpiff(this.acctno, id.Value, this.itemno, this.stockLocn,
                                            Convert.ToDecimal(txtAmount.Text), this.agreementno, this.salesPerson, out err);
                    SpiffAdded = true;
                }
                else
                {
                    lblNotAuthorised.Visible = true;
                    SpiffAdded = false;
                }

                this.Close();
            }
        }

        private bool Valid()
        {
            ClearErrors();
            if (CheckEmpty(txtUser, "user") & CheckEmpty(txtPassword, "password") & CheckAmount())
                return true;
            else
                return false;
        }

        private bool CheckEmpty(Control control, string type)
        {
            if (string.IsNullOrEmpty(control.Text))
            {
                errorProvider1.SetError(control, "Please enter a valid " + type);
                return false;
            }
            else
                return true;
        }

        private bool CheckAmount()
        {
            if (string.IsNullOrEmpty(txtAmount.Text) || IsStrictNumeric(StripCurrency(txtAmount.Text)))
            {
                errorProvider1.SetError(txtAmount, "Please enter a valid amount");
                return false;
            }
            else
                return true;
        }

        private void ClearErrors()
        {
            errorProvider1.SetError(txtUser, String.Empty);
            errorProvider1.SetError(txtPassword, String.Empty);
            errorProvider1.SetError(txtAmount, String.Empty);
            lblNotAuthorised.Visible = false;
        }

    }
}
