using System;
using System.Collections;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;

using System.Reflection;
using System.Collections.Generic;
using STL.Common;
using STL.DAL;
using Blue.Cosacs;
using STL.BLL;


namespace STL.WS

{
    /// <summary>
    /// Summary description for WebService
    /// </summary>
    [WebService(Namespace = "http://strategicthought.com/webservices/")]

    public class WUpdater
    {
        [WebMethod]
        public string CheckNewVersion(string versionclient)
        {
            BUpdater bupdate = new BUpdater();
            return bupdate.CheckNewVersionServer(versionclient, Assembly.GetExecutingAssembly().GetName().Version.ToString());
        }

        [WebMethod]
        public List<UpdateFile> GetFileListForUpdate()
        {
            BUpdater bupdate = new BUpdater();
            return bupdate.GetFileListForUpdate(HttpRuntime.AppDomainAppPath, Assembly.GetExecutingAssembly().GetName().Version.ToString());
        }

        [WebMethod]
        public List<UpdateFile> GetFilesForDownload(string additionalpath)
        {
            BUpdater bupdate = new BUpdater();
            return bupdate.GetFilesForDownload(HttpRuntime.AppDomainAppPath + additionalpath);
        }

        [WebMethod]
        public string GetServerPath()
        {
            return HttpRuntime.AppDomainAppPath;
        }

        [WebMethod]
        public byte[] DownloadFile(string path)
        {

            DTransfer T = new DTransfer();
            return T.ReadBinaryFile(path);
        }

        [WebMethod]
        public string CheckDB()
        {
            try
            {
		    BCountry c = new BCountry();
		    return c.GetDataBaseVersion();
            }
            catch
            {
                throw;
            }
        }

        [WebMethod]
        public void ReportUpgrade(string machinename, string domain, string user, string oldversion, string newversion)
        {
            try
            {
                BUpdater bupdate = new BUpdater();
                bupdate.ReportUpgrade(machinename, domain, user, oldversion, newversion);
            }
            catch
            {
                throw;
            }
        }
    
    }
}


