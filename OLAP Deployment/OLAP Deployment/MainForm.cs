using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OLAP_Deployment
{
    public partial class MainForm : Form
    {
        private readonly OlapDeployment _olapDeployment  = new OlapDeployment();

        public MainForm()
        {
            InitializeComponent();

            dbDefinitionFileTextBox.Text = _olapDeployment.DatabaseDefinitionFile;
            OLAPSrvConnectionTextBox.Text = _olapDeployment.OlapServerConnection;
            SourceDataMartTextBox.Text = _olapDeployment.SourceDataMart;
            ProcessDbCheckBox.Checked = _olapDeployment.ProcessDb;

            _olapDeployment.Progress += LogStatus;
        }

        private void BrowseBtn_Click(object sender, EventArgs e)
        {

            if (openFileDialog1.ShowDialog()== DialogResult.OK)
            {
                dbDefinitionFileTextBox.Text = openFileDialog1.FileName;
            }
        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void OkBtn_Click(object sender, EventArgs e)
        {
            OkBtn.Enabled = false;

            try
            {
                _olapDeployment.DatabaseDefinitionFile = dbDefinitionFileTextBox.Text;
                _olapDeployment.OlapServerConnection = OLAPSrvConnectionTextBox.Text;
                _olapDeployment.SourceDataMart = SourceDataMartTextBox.Text;
                _olapDeployment.ProcessDb = ProcessDbCheckBox.Checked;

                _olapDeployment.Deploy();
            }
            catch (Exception ex)
            {
                LogTextBox.AppendText(ex.Message + "\n");  
            }
            finally
            {
                OkBtn.Enabled = true;  
            }
          

        }

        private void LogStatus(object sender, Progress e)
        {
            LogTextBox.AppendText(e.Status + "\n");
        }
    }
}
