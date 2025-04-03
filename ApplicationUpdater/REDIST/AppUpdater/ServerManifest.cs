using System;
using System.Xml;
using System.Reflection;
using System.IO;
using System.Diagnostics;

namespace STL.AppUpdater
{
	//**************************************************************
	// ServerManifest Class	
	// - When doing server manifest style update checks, this class
	//   is used to download & inspect the server manifest
	//**************************************************************
	public class ServerManifest
	{
		private XmlDocument _manifest; 
		private string _url;

		private string _AvailableVersion;
		public string AvailableVersion 
		{ get { return _AvailableVersion; } set { _AvailableVersion = value; } }

		private string _ApplicationUrl;
		public string ApplicationUrl 
		{ get { return _ApplicationUrl; } set { _ApplicationUrl = value; } }

		public DateTime LastModTime
		{
			get
			{
				return WebFileLoader.GetLastModTime(_url);
			}
		}

		//**************************************************************
		// ServerManifest()	
		//**************************************************************
		public ServerManifest()
		{
		}

		//**************************************************************
		// Load()	
		//**************************************************************
        [DebuggerHidden]
        public void Load(string url)
		{
			_url = url;
			String LocalManifestPath = AppDomain.CurrentDomain.BaseDirectory  + 
				Path.GetFileName((new Uri(_url)).LocalPath);

			//STL added a new parameter to control whether to check the 
			//last modified property of the file to download
			WebFileLoader.UpdateFile(_url, LocalManifestPath, false);

			_manifest = new XmlDocument();
			_manifest.Load(LocalManifestPath);

			ApplicationUrl = _manifest.GetElementsByTagName("ApplicationUrl")[0].InnerText;
			AvailableVersion = _manifest.GetElementsByTagName("AvailableVersion")[0].InnerText;
		}

		//**************************************************************
		// IsServerVersionNewer()	
		//**************************************************************
		public bool IsServerVersionNewer(Version currentVersion)
		{
			
			Version ServerVersion = new Version(AvailableVersion);

			if (ServerVersion > currentVersion)
				return true;
			else 
				return false;
    	}

		//**************************************************************
		// IsServerVersionDifferent()	
		//**************************************************************
		public bool IsServerVersionDifferent(Version currentVersion)
		{
			
			Version ServerVersion = new Version(AvailableVersion);

			if (ServerVersion != currentVersion)
				return true;
			else 
				return false;
		}

	}
}
