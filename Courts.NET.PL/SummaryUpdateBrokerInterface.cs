using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using STL.Common;
using STL.Common.Static;
using STL.PL.WS9;
using System.Collections.Specialized;
using System.Xml;
using STL.Common.Constants.TableNames;





namespace STL.PL
{
    public partial class SummaryUpdateBrokerInterface : CommonForm
    {
        public SummaryUpdateBrokerInterface(Form root, Form parent)
        {
            InitializeComponent();


            WEODManager EodManager = new WEODManager(true);
            string error = string.Empty;
            DataSet ds = EodManager.GetInterfaceControl("Broker", "", false, out error);        //UAT1010 jec 09/07/10
            dgBrokerDetails.DataSource= ds.Tables[0].DefaultView;


            StringCollection branchNos = new StringCollection();
            branchNos.Add("ALL");

            XmlDocument dropDowns = new XmlDocument();
            dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");
			
            if (dropDowns.DocumentElement.ChildNodes.Count > 0)
            {
                ds = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out Error);
                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    foreach (DataTable dt in ds.Tables)
                        StaticData.Tables[dt.TableName] = dt;
                }
            }

            foreach (DataRow row in ((DataTable)StaticData.Tables[TN.BranchNumber]).Rows)
            {
                branchNos.Add(Convert.ToString(row["branchno"]));
            }
/*
            if ((string)drpBranch.SelectedItem == "ALL")
                branchFilter = "%";
            else
                branchFilter = (string)drpBranch.SelectedItem + "%";*/
		}

        private void drpBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            string branchFilter = "";

            if ((string)drpBranch.SelectedItem == "ALL")
                branchFilter = "%";
            else
                branchFilter = (string)drpBranch.SelectedItem + "%";
            
            // Now load up the totals --hmm

        }



    }
}
