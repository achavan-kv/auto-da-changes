using System;
using System.Collections.Generic;
using System.Text;


namespace STL.Common
{
    public class UpdateFile
    {
        private string _filename;
        private string _fullpath;
        private string _version;
        private long _size;
        private bool _dir;

        public bool dir
        {
            get{ return _dir;}
            set {_dir = value;}
        }

        public string filename
        {
            get { return _filename; }
            set { _filename = value; }
        }

        public string fullpath
        {
            get { return _fullpath; }
            set { _fullpath = value; }
        }

        public string version
        {
            get { return _version; }
            set { _version = value; }
        }

        public long size
        {
            get { return _size; }
            set { _size = value; }
        }


    }

    public static class Helper
    { 
        public static long ConvertVersionToInt(string version)
        {
            string[] versions = version.Split('.');
            string output = "";

            for (int i = 0; i < versions.Length; i++)
            {
                output = output + (Convert.ToInt32(versions[i])).ToString("00");
            }

            return Convert.ToInt64(output);
        }

        public static string GetFileVersion(string path)
        {
            try
            {
                return System.Reflection.Assembly.Load(System.IO.File.ReadAllBytes(path)).GetName().Version.ToString();
            }
            catch 
            {
                return "";
            }
        }
    }
}
