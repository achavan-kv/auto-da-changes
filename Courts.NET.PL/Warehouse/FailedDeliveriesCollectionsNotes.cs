using System;
using System.Windows.Forms;
using Blue.Cosacs.Shared.Services;
using Blue.Cosacs.Shared.Services.Warehouse;
using STL.Common.Static;

namespace STL.PL.Warehouse
{
    public partial class FailureNotes : CommonForm
    {
        public FailureNotes()
        {
            InitializeComponent();
        }

        private string AcctNo { get; set; }
        private string Notes { get; set; }

        public FailureNotes(string acctNo, System.Windows.Forms.Form parent, Form root)
        {

            InitializeComponent();
            TranslateControls();

            FormParent = parent;
            FormRoot = root;

            this.AcctNo = acctNo;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Client.Call(new SaveLineItemFailureNotesRequest
            {
                AcctNo = this.AcctNo,
                Notes = txtFailureNotes.Text,
                EmpeeNo = Credential.UserId

            },
            response =>
            {
                this.Close();
            },
                this);



        }
    }
}
