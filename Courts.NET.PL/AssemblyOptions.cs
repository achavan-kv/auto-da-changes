using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace STL.PL
{
    public partial class AssemblyOptions : CommonForm
    {

        private bool assemblyRequired = true;
        public bool AssemblyRequired
        {
            set { assemblyRequired = value; }
            get { return assemblyRequired; }
        }

        public AssemblyOptions()
        {
            InitializeComponent();
        }

        public AssemblyOptions(System.Windows.Forms.Form parent, Form root)
        {
            InitializeComponent();
            FormParent = parent;
            FormRoot = root;

        }

        //private void btnCancel_Click(object sender, EventArgs e)
        //{
        //    this.Close();
        //}

        private void btnOk_Click(object sender, EventArgs e)
        {

            //Assembly only required if Courts or Contractor selected. Not required if Customer selected.
            if (radioCourts.Checked || radioContractor.Checked)
            {
                assemblyRequired = true;
            }
            else
            {
                assemblyRequired = false;
            }
            this.Close();
        }

     

    }
}
