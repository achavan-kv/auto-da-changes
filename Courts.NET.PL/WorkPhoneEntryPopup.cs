using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace STL.PL
{
    public partial class WorkPhoneEntryPopup : Form
    {
        private bool _ok = false;
        public bool OK
        {
            get { return _ok; }
        }

        public string WorkDialCode2
        {
            get { return txtWorkDialCode2.Text;}
            set { txtWorkDialCode2.Text = value; }
        }

        public string WorkDialCode3
        {
            get { return txtWorkDialCode3.Text; }
            set { txtWorkDialCode3.Text = value; }
        }

        public string WorkDialCode4
        {
            get { return txtWorkDialCode4.Text; }
            set { txtWorkDialCode4.Text = value; }
        }

        public string WorkNum2
        {
            get { return txtWorkNum2.Text; }
            set { txtWorkNum2.Text = value; }
        }

        public string WorkNum3
        {
            get { return txtWorkNum3.Text; }
            set { txtWorkNum3.Text = value; }
        }

        public string WorkNum4
        {
            get { return txtWorkNum4.Text; }
            set { txtWorkNum4.Text = value; }
        }

        public string WorkExt2
        {
            get { return txtWorkExt2.Text; }
            set { txtWorkExt2.Text = value; }
        }

        public string WorkExt3
        {
            get { return txtWorkExt3.Text; }
            set { txtWorkExt3.Text = value; }
        }

        public string WorkExt4
        {
            get { return txtWorkExt4.Text; }
            set { txtWorkExt4.Text = value; }
        }

        public WorkPhoneEntryPopup(bool readOnly)
        {
            InitializeComponent();

            txtWorkDialCode2.ReadOnly = readOnly;
            txtWorkDialCode2.Enabled = !readOnly;

            txtWorkNum2.ReadOnly = readOnly;
            txtWorkNum2.Enabled = !readOnly;

            txtWorkExt2.ReadOnly = readOnly;
            txtWorkExt2.Enabled = !readOnly;

            txtWorkDialCode3.ReadOnly = readOnly;
            txtWorkDialCode3.Enabled = !readOnly;

            txtWorkNum3.ReadOnly = readOnly;
            txtWorkNum3.Enabled = !readOnly;

            txtWorkExt3.ReadOnly = readOnly;
            txtWorkExt3.Enabled= !readOnly;

            txtWorkDialCode4.ReadOnly = readOnly;
            txtWorkDialCode4.Enabled = !readOnly;

            txtWorkNum4.ReadOnly = readOnly;
            txtWorkNum4.Enabled = !readOnly;

            txtWorkExt4.ReadOnly = readOnly;
            txtWorkExt4.Enabled = !readOnly;

        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this._ok = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this._ok = false;
            this.Close();
        }

      
    }
}
