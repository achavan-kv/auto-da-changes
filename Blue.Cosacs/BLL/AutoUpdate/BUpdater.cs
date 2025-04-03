using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using STL.Common;
using STL.DAL;


namespace STL.BLL
{
    public class BUpdater
    {
        public string CheckNewVersionServer(string versionclient, string versionserver)
        {
            if (Helper.ConvertVersionToInt(versionserver) > Helper.ConvertVersionToInt(versionclient))
            {
                return versionserver;
            }
            else
            {
                return "";
            }
        }

        public List<UpdateFile> GetFileListForUpdate(string serverpath, string version)
        {
            DUpdater dupdater = new DUpdater();
            return dupdater.GetFileListForUpdate(serverpath, version);
        }

        public List<UpdateFile> GetFilesForDownload(string path)
        {
            DUpdater dupdater = new DUpdater();
            return dupdater.GetFilesForDownload(path);
        }
        public void ReportUpgrade(string machinename, string domain, string user, string oldversion, string newversion)
        {
            DUpdater dupdater = new DUpdater();
            dupdater.ReportUpgrade(machinename, domain, user, oldversion, newversion);
        }



    }


}
