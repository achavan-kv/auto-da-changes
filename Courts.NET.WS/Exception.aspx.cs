using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Cosacs.Web
{
    public partial class Exception : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            throw new System.Exception("Test exception!");
        }
    }
}