using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Blue.Cosacs.Repositories;

public class StylesheetResult : ViewResult
{
    protected override ViewEngineResult FindView(ControllerContext context)
    {
        var countryCode = new ConfigRepository().CountryCode();
        var viewName = "~/Stylesheets/" + countryCode + "/" + ViewName + ".cshtml";
        if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath(viewName)))
            ViewName = viewName;
        return base.FindView(context);
    }
}
