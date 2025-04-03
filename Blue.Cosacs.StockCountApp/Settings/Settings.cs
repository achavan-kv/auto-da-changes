using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Collections.Specialized;

namespace Blue.Cosacs.StockCountApp
{
    public class Settings
    {
        private static NameValueCollection m_settings;
        private static string m_settingsPath;
       
        public static void Init()
        { 
            // Get the path of the settings file.
            m_settingsPath = Path.GetDirectoryName(
            System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            m_settingsPath += @"\Settings\Settings.xml";

            if (!File.Exists(m_settingsPath))
                throw new FileNotFoundException(
                                  m_settingsPath + " could not be found.");

            var xdoc = new XmlDocument();
            using (var xmlFile = new FileStream(m_settingsPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                xdoc.Load(xmlFile);
            }

            var root = xdoc.DocumentElement;
            var nodeList = root.ChildNodes.Item(0).ChildNodes;

            // Add settings to the NameValueCollection.
            m_settings = new NameValueCollection();
            m_settings.Add("userAgent", nodeList.Item(0).Attributes["value"].Value);
            m_settings.Add("databasePath", nodeList.Item(1).Attributes["value"].Value);
            m_settings.Add("authPath", nodeList.Item(2).Attributes["value"].Value); 
           
            // Load other user specified settings
            var settings = new SettingsRepository().Get();
            if (settings != null)
            {
                AuthHost = settings.Host;                
            }
        }

        public static bool IsValid
        {
            get { return !string.IsNullOrEmpty(AuthHost); }
        }

        public static string AuthHost
        {
            get; set;
        }     

        public static string AuthPath
        {
            get { return m_settings.Get("authPath"); }
            set { m_settings.Set("authPath", value); }
        }

        public static string UserAgent
        {
            get { return m_settings.Get("userAgent"); }
            set { m_settings.Set("userAgent", value); }
        }

        public static string ConnectionString
        {
            get { return "Data Source=" + DatabasePath; }
        }

        public static string DatabasePath
        {
            get { return m_settings.Get("databasePath"); }
            set { m_settings.Set("databasePath", value); }
        }        
    }
}
