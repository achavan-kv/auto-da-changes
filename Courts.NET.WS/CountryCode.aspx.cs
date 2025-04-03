using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Blue.Cosacs.Repositories;

public partial class CountryCode : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.ClearHeaders();
        Response.Write(new ConfigRepository().CountryCode());
        Response.End();
    }
}