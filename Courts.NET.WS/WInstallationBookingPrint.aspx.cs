using System;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using STL.BLL;
using STL.Common;
using Blue.Cosacs.Repositories;


public partial class WInstallationBookingPrint : CommonWebPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            
                string strInstallationNos = Request["installationNos"];
                string countryCode        = Request["countryCode"]; //TODO read from DB
                
                var installationNos = new List<int>();

                foreach (var str in strInstallationNos.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
	            {
		            int i;
                    if (Int32.TryParse(str, out i))
                        installationNos.Add(i);
	            }

                var mainXml = new InstallationRepository()
                                    .GetBookingPrintXML(installationNos, countryCode);

                Response.Write(mainXml.Transform());
            
        }
        catch (Exception ex)
        {
            logException(ex, Function);
            Response.Write(ex.Message);
        }
    }
}