using System;
using System.Data;
using System.Windows.Forms;

namespace STL.PL
{
    public partial class AuthoriseCheck : CommonForm
    {
        private readonly string screen;
        private readonly string control;
        private bool isAuthorised;
        private int? authorisedBy;

        public bool IsAuthorised 
        {
            get { return isAuthorised; }
        }

        public int? AuthorisedBy 
        { 
            get { return authorisedBy; } 
        }

        public AuthoriseCheck(string screen, string control, string explanation = null)
        {
            InitializeComponent();
            this.screen = screen;
            this.control = control;
            lblExplanation.Text = explanation ?? "Authorisation Required";
        }


        public int? ControlPermissionCheck(string login, string password) // Not current user
        {
            return StaticDataManager.ControlPermissionPasswordCheck(login,password, screen, control);
        }

        public int? ControlPermissionCheck(string login) // Logged in user
        {
            return StaticDataManager.ControlPermissionCheck(login, screen, control);
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
            var id = ControlPermissionCheck(txtUser.Text,txtPassword.Text);
            if (id.HasValue)
            {
                isAuthorised = true;
                authorisedBy = id.Value;
                this.Close();
            }
            else
            {
                lblNotAuthorised.Visible = true;
            }
        }

        private void AuthoriseCheck_Load(object sender, EventArgs e)
        {
            txtUser.Clear();
            txtPassword.Clear();
            isAuthorised = false;
            authorisedBy = null;
            lblNotAuthorised.Visible = false;
        }
    }
}
