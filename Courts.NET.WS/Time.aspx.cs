using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Time : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.ClearHeaders();
        Response.Write(DateTime.Now.ToUniversalTime().ToString("R"));
        Response.End();
    }
}