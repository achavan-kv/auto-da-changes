using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using STL.Common.Static;

namespace STL.DAL
{
    public class Dprinting :DALObject
    {
        public Dprinting()
        {
        }

        public void VerifyFile(ref DateTime modified, ref double size, ref string path, string filename, string serverdir,string countrycode)
        {
            path = "";
            size = 0;
            modified = DateTime.MinValue;

            DirectoryInfo Styledir = new DirectoryInfo(serverdir + @"\stylesheets\" + countrycode);
            foreach (FileInfo stylefile in Styledir.GetFiles())
            {
                //IP - 14/04/10 - UAT(54) UAT5.2 make strings lower case when comparing as case sensitive.
                if (stylefile.Name.ToLower() == filename.ToLower())
                {
                    modified = stylefile.LastWriteTime;
                    size = stylefile.Length;
                    path = stylefile.FullName;
                }

            }
            if (path.Length == 0)
            {
               
                FileInfo stylefile = new FileInfo(serverdir + @"\stylesheets\" + filename);
                if (stylefile.Exists)
                {
                    modified = stylefile.LastWriteTime;
                    size = stylefile.Length;
                    path = stylefile.FullName;
                }
            }

            if (path.Length == 0 || size == 0 || modified == DateTime.MinValue)
            { 
                throw (new Exception ("Error! Stylesheet " + filename + " not found or can not be accessed in " + serverdir + @"\stylesheets\" ));
            }
        }


        //public string FindPath(string homepath, string filename)
        //{
        //    string fullpath = "";

        //    DirectoryInfo Styledir = new DirectoryInfo(homepath + @"stylesheets\" + Config.CountryCode);
        //    foreach (FileInfo stylefile in Styledir.GetFiles())
        //    {
        //        if (stylefile.Name == filename)
        //        {
        //            fullpath = stylefile.FullName;
        //        }

        //    }
        //    if (fullpath.Length == 0)
        //    {
        //        FileInfo stylefile = new FileInfo(homepath + @"stylesheets\" + filename);

        //        if (!stylefile.Exists)
        //        {
        //            throw new Exception("There is no style sheet located for " + filename);
        //        }
        //        else
        //        {
        //            fullpath = stylefile.FullName;
        //        }
        //    }

        //    return fullpath;
        //}

    }
}
