using System;
using System.Data;
using System.Windows.Forms;
using STL.Common.Static;


namespace STL.PL
{

    public partial class CustomerMailingLoadQuery : CommonForm
    {
        public string _QueryName = "";
        public CustomerMailingLoadQuery(out string QueryName)
        {

            InitializeComponent();
            DataSet ds = CustomerManager.CustomerMailingQueryLoadbyEmpeeno(Credential.UserId);
            dgvQueries.DataSource = ds.Tables[0];
            QueryName = this._QueryName;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this._QueryName = dgvQueries.SelectedRows[0].Cells[0].Value.ToString();
            //DataView dv = (DataView)dgvQueries.DataSource;
            //for (int i = dv.Count - 1; i >= 0; i--)
            //{



            //}

            //this.Close;
            //textBox1.Text = this._QueryName;
            Close();

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }


        private void rowSelected_MouseUp(object sender, MouseEventArgs e)
        {
            //   try
            //   {
            //       if (dgvQueries.DataSource != null)
            //       {
            //           Wait();

            //           DataView dv = (DataView)dgvQueries.DataSource;
            //           for (int i = dv.Count - 1; i >= 0; i--)
            //           {
            //               if (dgvQueries.SelectedRows != "")
            //               {
            btnOK.Enabled = true;
            //               }
            //               else
            //               {
            //                   btnOK.Enabled = false;
            //               }

            //           }
            //       }
            //   }
            //   catch (Exception ex)
            //   {
            //       Catch(ex, Function);
            //   }
            //   finally
            //   {
            //       StopWait();
            //   }
        }

    }

}