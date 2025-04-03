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
using STL.DAL;

namespace STL.WS
{
	/// <summary>
	/// Summary description for WContractNos.
	/// </summary>
	public partial class WAccountNos : CommonWebPage
	{
        int times = 0;  //CR1072 Malaysia merge 70286 

        protected void Page_Load(object sender, System.EventArgs e)
        {
            // fix: times added to stop code executing twice - bit of a hack
            // can't find reason for it
            if (times < 1)      //70286 jec 23/10/08 
            {
                SqlConnection conn = null;


                try
                {
                    short branchNo = Convert.ToInt16(Request[CN.BranchNo]);
                    int number = Convert.ToInt32(Request[CN.AccountNo]);
                    string accountType = Request[CN.AccountType];

                    conn = new SqlConnection(Connections.Default);
                    do
                    {
                        try
                        {
                            conn.Open();
                            using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                            {

                                string[] accountNos = new BAccount().GetAccountNos(conn, trans, accountType, branchNo, number);

                                AccountNosXML axml = new AccountNosXML((string)Country[CountryParameterNames.CountryCode]);

                                axml.AddAccountNos(accountNos);
                                Response.Write(axml.Transform());

                                trans.Commit();
                            }
                            break;
                        }
                        catch (SqlException ex)
                        {
                            CatchDeadlock(ex, conn);
                        }
                    } while (retries <= maxRetries);
                }
                catch (Exception ex)
                {
                    logException(ex, "Page_Load");
                    Response.Write(ex.Message);
                }
                finally
                {
                    if (conn.State != ConnectionState.Closed)
                        conn.Close();
                }
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
