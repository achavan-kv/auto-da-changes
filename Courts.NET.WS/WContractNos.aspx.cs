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
using STL.Common;
using STL.Common.Constants.TableNames;
using System.Xml;
using System.Data.SqlClient;
using System.Configuration;
using STL.Common.Constants.ColumnNames;
using System.Threading;
using System.Globalization;

namespace STL.WS
{
	/// <summary>
	/// Summary description for WContractNos.
	/// </summary>
	public partial class WContractNos : CommonWebPage
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				short branchNo = Convert.ToInt16(Request[CN.BranchNo]);
				int number = Convert.ToInt32(Request[CN.ContractNo]);
				
				string [] contractNos = new BItem().GetContractNos(branchNo, number);

				ContractNosXML cxml = new ContractNosXML((string)Country[CountryParameterNames.CountryCode]);

				cxml.AddContractNos(contractNos);

				Response.Write(cxml.Transform());				

			}
			catch(Exception ex)
			{
				logException(ex, "Page_Load");
				Response.Write(ex.Message);
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
