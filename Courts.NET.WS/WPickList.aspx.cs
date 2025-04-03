using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using STL.BLL;
using STL.DAL;
using STL.Common;
using STL.Common.Constants.TableNames;
using System.Xml;
using STL.Common.Constants.ColumnNames;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading;
using System.Globalization;
using STL.Common.Printing.AgreementPrinting;
using STL.Common.Constants.FTransaction;

namespace STL.WS
{
	/// <summary>
	/// Summary description for WPickList.
	/// </summary>
	public partial class WPickList : CommonWebPage
	{
		string empeeName = "";
		string branchName = "";
		int pickListNo = 0;
		int empeeNo = 0;
		short branchNo = 0;
		short delNoteBranch = 0;
		string countryCode = "";
		string culture = "";
		bool isAddition = false;
		bool isReprint = false;
		bool isOrderPicklist = false;
      string storeType = String.Empty;
      string acctno = String.Empty;


		protected void Page_Load(object sender, System.EventArgs e)
		{
			//SqlConnection conn = null;
			//SqlTransaction trans = null;

         DDelivery del = new DDelivery();
         BLogin login = new BLogin();
			try
			{				
				pickListNo = Convert.ToInt32(Request["pickListNo"]);
				empeeNo = Convert.ToInt32(Request["empeeNo"]);
				branchName = Request["branchName"];
				branchNo = Convert.ToInt16(Request["branchNo"]);
				delNoteBranch = Convert.ToInt16(Request["delNoteBranch"]);
				isAddition = Convert.ToBoolean(Convert.ToInt16(Request["isAddition"]));
				isReprint = Convert.ToBoolean(Convert.ToInt16(Request["isReprint"]));
				isOrderPicklist = Convert.ToBoolean(Convert.ToInt16(Request["isOrderPicklist"]));
				countryCode = Request["countryCode"];
				culture = Request["culture"];

            string courtsBranches = Request["courtsBranches"];
            //CR903 Create an array list to hold all the courts branches
            ArrayList branches = new ArrayList();
            //RDB/IP - 17/10/2007 UAT(347) - Create an array of strings to store
            //elements from the 'courtsBranches' string, split on '|'.
            string[] stringBranches = courtsBranches.Split('|');
            //Iterate through the array and whilst the element
            //is not an empty string add the element to the array list.
            foreach (string currBranch in stringBranches)
            {
                if (currBranch != String.Empty)
                {
                    branches.Add(currBranch);
                }
            }
            

            //while (courtsBranches.Length > 1)
            //{
              
            //   string branch = courtsBranches.Substring(0, 3);
            //   courtsBranches = courtsBranches.Remove(0, 4);
            //   branches.Add(branch);
            //}

				do
				{
               using (SqlConnection conn = new SqlConnection(Connections.Default))
               {
                   try
                   {
                       conn.Open();
                       using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                       {

                           // set the culture for currency formatting
                           //Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
                           base.SetCulture();

                           PickListXML pxml = new PickListXML(countryCode, isOrderPicklist);
                           pxml.Load("<PICKLISTS/>");

                           empeeName = login.GetEmployeeName(conn, trans, empeeNo);

                           // retrieve items for this pick list

                           del.GetPickList(conn, trans, pickListNo, branchNo, isReprint, isAddition, isOrderPicklist);
                           int totalCount = del.Deliveries.Rows.Count;
                           DataView dvDeliveries = new DataView(del.Deliveries);

                           // load deliveries that belong to this pick list
                           dvDeliveries.RowFilter = CN.Quantity + " > 0";
                           if (dvDeliveries.Count > 0)
                           {
                               PickListXML pickList = pxml.CreatePickList(countryCode, isOrderPicklist);

                               CreateHeader(pickList, true);
                               int items = 0;
                               foreach (DataRowView row in dvDeliveries)
                               {
                                   items++;
                                   pickList.AddLineItem(row, true, branches, ref items);
                               }

                               if (dvDeliveries.Count == totalCount)
                                   pickList.SetNode("PICKLIST/LAST", "TRUE");
                               else
                               {
                                   pickList.SetNode("PICKLIST/LAST", "FALSE");
                               }

                               pxml.ImportNode(pickList.DocumentElement, true);
                           }

                           // load collections that belong to this pick list
                           dvDeliveries.RowFilter = CN.Quantity + " < 0";
                           if (dvDeliveries.Count > 0)
                           {
                               PickListXML pickList = pxml.CreatePickList(countryCode, isOrderPicklist);

                               CreateHeader(pickList, false);
                               int items = 0;
                               foreach (DataRowView row in dvDeliveries)
                               {
                                   items++;
                                   pickList.AddLineItem(row, false, branches, ref items);
                               }

                               pickList.SetNode("PICKLIST/LAST", "TRUE");

                               pxml.ImportNode(pickList.DocumentElement, true);
                           }

                           if (!isReprint)
                               del.SetPickListPrinted(conn, trans, pickListNo, isOrderPicklist);

                           dvDeliveries = null;

                           trans.Commit();
                           Response.Write(pxml.Transform());
                           pxml = null;
                           break;
                       }
                   }
                   catch (SqlException ex)
                   {
                       CatchDeadlock(ex, conn);
                   }
               }
				}while (retries <= maxRetries);

            branches = null;
			}
			catch(Exception ex)
			{
				logException(ex, "Page_Load");
				Response.Write(ex.Message);
			}
			finally
			{
            //if(conn.State != ConnectionState.Closed)
            //   conn.Close();
            del = null;
            login = null;
			}		
		}

		private void CreateHeader(PickListXML pxml, bool deliveries)
		{
         //pxml.SetNode("ACTIONSHEET/HEADER/STORETYPE", storeType);
			if(isAddition)
				pxml.SetNode("PICKLIST/HEADER/PICKTEXT", GetResource("T_ADDITIONS"));
			
			if(isReprint)
				pxml.SetNode("PICKLIST/HEADER/PICKTEXT", GetResource("T_REPRINT"));
			
			pxml.SetNode("PICKLIST/HEADER/PICKNUMBER", pickListNo.ToString());
			pxml.SetNode("PICKLIST/HEADER/BRANCHNAME", branchName);
			pxml.SetNode("PICKLIST/HEADER/BRANCH", branchNo.ToString());
			
			if((bool)Country[CountryParameterNames.DisplayDelNoteBranch])
				pxml.SetNode("PICKLIST/HEADER/DELNOTEBRANCH", "Delivery Note Branch " + delNoteBranch.ToString());
			else
				pxml.SetNode("PICKLIST/HEADER/DELNOTEBRANCH", "");
			
			pxml.SetNode("PICKLIST/HEADER/PRINTED", DateTime.Today.ToLongDateString() +  " " + DateTime.Now.ToLongTimeString());
			pxml.SetNode("PICKLIST/HEADER/USERNAME", empeeName);
			pxml.SetNode("PICKLIST/HEADER/USER", empeeNo.ToString());

			if(deliveries)
				pxml.SetNode("PICKLIST/HEADER/DELTEXT", GetResource("M_PICKLISTDEL"));
			else
				pxml.SetNode("PICKLIST/HEADER/DELTEXT", GetResource("M_PICKLISTCOLL"));

            //IP - 22/09/11 - RI - #8224 - CR8201
            if (Convert.ToBoolean(Country[CountryParameterNames.RIDispCatAsDept]) == true)
            {
                pxml.SetNode("PICKLIST/HEADER/CATEGORYHEADING", "Department");
            }
            else
            {
                pxml.SetNode("PICKLIST/HEADER/CATEGORYHEADING", "Category");
            }
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
		}
		#endregion
	}
}
