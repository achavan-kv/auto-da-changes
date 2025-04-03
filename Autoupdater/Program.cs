using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.Threading;
using System.IO;

namespace AppStart
{
    static class Program
    {
         static Mutex AppStartMutex;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                //Check to see if AppStart is already running
                bool ObtainedOwnership = false;
                AppStartMutex = new Mutex(true, "AppStart", out ObtainedOwnership);

                if (!ObtainedOwnership)
                {
                    Environment.Exit(1);
                }
                else
                {
                    FindApp();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured in starting CoSACS AppStart program!" + Environment.NewLine + Environment.NewLine + ex.Message.ToString(), "Critical Error in CoSACS Start Program!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void FindApp()
        {
            bool check = false;
            string currentdir = System.Reflection.Assembly.GetExecutingAssembly().Location;
            currentdir = currentdir.Substring(0, currentdir.LastIndexOf("AppStart") -1);

            string appstartpath = "";
            long version = 0;

            DirectoryInfo startdir = new DirectoryInfo(currentdir);

            foreach (DirectoryInfo subdirs in startdir.GetDirectories())
            {
                FileInfo file = new FileInfo(subdirs.FullName + @"\Courts.NET.PL.exe");
                FileInfo verified = new FileInfo(subdirs.FullName + @"\DLverified.dat");

                if (verified.Exists)
                {
                    check = true;
                }

                if (file.Exists && (check == verified.Exists))    
                {
                        CheckVersion(file,ref appstartpath, ref version);
                }
            }

            LauchApp(appstartpath);

        }

        private static void CheckVersion(FileInfo file, ref string appstartpath, ref long version)
        {
            string newversion = System.Reflection.Assembly.LoadFile(file.FullName.ToString()).GetName().Version.ToString();
            long newversionint = ConvertVersionToInt(newversion);

            if (newversionint > version)
            {
                version = newversionint;
                appstartpath = file.FullName;
            }
        }

        private static void LauchApp(string fullpath)
        {
            if (fullpath == "")
            {
                MessageBox.Show("An error occured in starting CoSACS AppStart program!" + Environment.NewLine + Environment.NewLine +
                                "No valid CoSACS installments found!" + Environment.NewLine +
                                "Please reinstall client.", "Critical Error in CoSACS Start Program!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                System.Diagnostics.Process.Start(fullpath);
            }
            
        }


        private static long ConvertVersionToInt(string version)
        {
            string[] versions = version.Split('.');
            string output = "";

            for (int i = 0; i < versions.Length; i++)
            {
                output = output + (Convert.ToInt32(versions[i])).ToString("00");
            }

            return Convert.ToInt64(output);
        }
    }
}

