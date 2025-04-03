using System;
using System.Collections.Generic;
using System.Text;
using STL.Common;
using System.IO;
using System.Data.SqlClient;
using System.Data;

namespace STL.DAL
{
    public class DUpdater : DALObject
    {
        public List<UpdateFile> GetFilesForDownload(string path)
        {
            List<UpdateFile> filelist = new List<UpdateFile>();

            DirectoryInfo maindir = new DirectoryInfo(path);
            AddDirectory(filelist, maindir);
            PopulateSubDir(maindir, ref filelist);
            return filelist;

        }

        public List<UpdateFile> GetFileListForUpdate(string serverpath, string version)
        {
            List<UpdateFile> filelist = new List<UpdateFile>();

            DirectoryInfo maindir = new DirectoryInfo(serverpath + version);
            AddDirectory(filelist, maindir);
            PopulateSubDir(maindir, ref filelist);
            return filelist;
        }

        private void PopulateSubDir(DirectoryInfo maindir, ref  List<UpdateFile> filelist)
        {
    
            if (maindir.Exists)
            {

                DirectoryInfo[] subdirs = maindir.GetDirectories();
                if (subdirs.Length != 0)
                {
                    foreach (DirectoryInfo newsubdir in subdirs)
                    {
                        PopulateSubDir(newsubdir, ref  filelist);
                    }
                }
                AddDirectory(filelist, maindir);
            }
            FileInfo[] files = maindir.GetFiles();
            foreach (FileInfo file in files)
            {
                UpdateFile newfile = new UpdateFile();
                newfile.filename = file.Name;
                newfile.fullpath = file.FullName;
                newfile.size = file.Length;
                newfile.dir = false;
                try
                {
                    newfile.version = System.Reflection.Assembly.LoadFile(file.FullName.ToString()).GetName().Version.ToString();
                }
                catch 
                {
                }

                filelist.Add(newfile);
            }
        }

        private void AddDirectory(List<UpdateFile> filelist, DirectoryInfo dir)
        {
            UpdateFile newdir = new UpdateFile();
            newdir.dir = true;
            newdir.filename = dir.Name;
            newdir.fullpath = dir.FullName;
            filelist.Add(newdir);
        }

        public void ReportUpgrade(string machinename, string domain, string user, string oldversion, string newversion)
        { 
            try
            {
            	SqlParameter[] parmArray = new SqlParameter[5];
				parmArray[0] = new SqlParameter("@machinename", SqlDbType.VarChar,200);
				parmArray[0].Value = machinename;
                parmArray[1] = new SqlParameter("@domain", SqlDbType.VarChar,200);
				parmArray[1].Value = domain;
                parmArray[2] = new SqlParameter("@user", SqlDbType.VarChar,200);
				parmArray[2].Value = user;
                parmArray[3] = new SqlParameter("@oldversion", SqlDbType.VarChar,200);
				parmArray[3].Value = oldversion;
                parmArray[4] = new SqlParameter("@newversion", SqlDbType.VarChar,200);
				parmArray[4].Value = newversion;
               
				this.RunSP("AutoUpdateSave", parmArray);
		
			}
			catch (SqlException)
            {
			}
        }
    }
}
