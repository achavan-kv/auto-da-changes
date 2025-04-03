using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Runtime.Serialization;
using System.IO;
using System.Diagnostics;

namespace STL.AppUpdater
{
	public enum UpdatePhases:int {Complete=0, InProgress=1, Scavenging=2, Downloading=3, Validating=4, Merging=5, Finalizing=6};

	//**************************************************************
	// AppManifest Class
	// -  This class is used for accessing and updating the application
	//    updater manifest. The Manifest stores information such as the 
	//    current phase the update operation is in.
	// -  Xml serialization is used to persist the manifest.
	//**************************************************************
	[Serializable]
	public class AppManifest
	{
		//Manifest File Path
		[NonSerialized] 
		private string _FilePath;
		public string FilePath 
		{ get { return _FilePath; } set { _FilePath = value; } }
		
		// Resources
		private ResourceFiles _Resources;
		public ResourceFiles Resources 
		{ get { return _Resources; } set { _Resources = value; } }

		//VersionInfo
		private AppVersionInfo _VersionInfo;
		public AppVersionInfo VersionInfo 
		{ get { return _VersionInfo; } set { _VersionInfo = value; } }

		//State
		private UpdateState _State;
		public UpdateState State 
		{ get { return _State; } set { _State = value; } }

		//**************************************************************
		// AppState constructor
		//**************************************************************
		public AppManifest(string manifestFilePath)
		{
			FilePath = manifestFilePath;

			Resources = new ResourceFiles();
			State = new UpdateState();
			VersionInfo = new AppVersionInfo();
		}

		//**************************************************************
		// Load()	
		// This static method is the way to instantiate manifest objects
		//**************************************************************
		public static AppManifest Load(string manifestFilePath)
		{
			Stream stream = null;

			try 
			{
				if (!File.Exists(manifestFilePath))
					return new AppManifest(manifestFilePath);

				IFormatter formatter = new System.Runtime.Serialization.Formatters.Soap.SoapFormatter();
				stream = new FileStream(manifestFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
				AppManifest Manifest = (AppManifest) formatter.Deserialize(stream);
				Manifest.FilePath = manifestFilePath;
				stream.Close();
				return Manifest;
			} 
			catch (Exception e)
			{
				if (stream != null)
					stream.Close();
				
				Debug.WriteLine("APPMANAGER:  ERROR loading app manifest, creating a new manifest.");
				Debug.WriteLine("APPMANAGER:  " + e.ToString());
				return new AppManifest(manifestFilePath);
			}
		} 

		//**************************************************************
		// Update()	
		//**************************************************************
		public void Update()
		{
			Stream stream = null;

			try
			{
				IFormatter formatter = new System.Runtime.Serialization.Formatters.Soap.SoapFormatter();
				stream = new FileStream(FilePath, FileMode.Create, FileAccess.Write, FileShare.None);
				formatter.Serialize(stream, this);
				stream.Close();
			} 
			catch (Exception e)
			{
				if (stream != null)
					stream.Close();

				Debug.WriteLine("APPMANAGER:  Error saving app manfiest.");
				throw e;	
			}
		}
	}

	//**************************************************************
	// State	
	//**************************************************************
	[Serializable]
	public class UpdateState 
	{
		private UpdatePhases _Phase = UpdatePhases.Complete;
		public UpdatePhases Phase 
			{ get { return _Phase; } set { _Phase = value; } }

		private bool _UpdateFailureEncoutered = false;
		public bool UpdateFailureEncoutered 
			{ get { return _UpdateFailureEncoutered; } set { _UpdateFailureEncoutered = value; } }
		
		private int _UpdateFailureCount = 0;
		public int UpdateFailureCount 
			{ get { return _UpdateFailureCount; } set { _UpdateFailureCount = value; } }
		
		private string _DownloadSource = "";
		public string DownloadSource 
			{ get { return _DownloadSource; } set { _DownloadSource = value; } }
		
		private string _DownloadDestination = "";
		public string DownloadDestination 
		{ get { return _DownloadDestination; } set { _DownloadDestination = value; } }
		
		private string _NewVersionDirectory = "";
		public string NewVersionDirectory 
		{ get { return _NewVersionDirectory; } set { _NewVersionDirectory = value; } }

	}
	
	//**************************************************************
	// VersionInfo	
	//**************************************************************
	[Serializable]
	public class AppVersionInfo
	{
		private string _VersionManifestUrl = "";
		public string VersionManifestUrl 
		{ get { return _VersionManifestUrl; } set { _VersionManifestUrl = value; } }

		private string _VersionNumber = "";
		public string VersionNumber 
		{ get { return _VersionNumber; } set { _VersionNumber = value; } }

		private DateTime _VersionLastChangeTime = DateTime.Now;
		public DateTime VersionLastChangeTime 
		{ get { return _VersionLastChangeTime; } set { _VersionLastChangeTime = value; } }
	}

	
	//**************************************************************
	// ResourceFiles class
	//**************************************************************
	[Serializable]
	public class ResourceFiles
	{

		public readonly SortedList ResourceList;
		
		public  ResourceFiles()
		{
			ResourceList = new SortedList();
		}
			
		//**************************************************************
		// AddResource()	
		//**************************************************************
		public void AddResource(Resource newResource)
		{
			Resource tempResource = (Resource)ResourceList[newResource.Name];
			if (tempResource == null)
				ResourceList.Add(newResource.Name,newResource);

		}

		//**************************************************************
		// GetResource()	
		//**************************************************************
		public Resource GetResource(string name)
		{
			return (Resource)ResourceList[name];
		}

		//**************************************************************
		// ResourceExists()	
		//**************************************************************
		public bool ResourceExists(string name)
		{
			Resource tempResource = (Resource)ResourceList[name];
			if (tempResource == null)
				return false;
			else
				return true;
		}
	}

	//**************************************************************
	// Resource Class()	
	//A resource is a file / assembly
	//**************************************************************
	[Serializable]
	public class Resource 
	{
		private string _Name;
		public string Name 
		{ get { return _Name; } set { _Name = value; } }
		
		private string _Url;
		public string Url 
		{ get { return _Url; } set { _Url = value; } }

		private string _FilePath;
		public string FilePath 
		{ get { return _FilePath; } set { _FilePath = value; } }

		private bool _IsFolder;
		public bool IsFolder 
		{ get { return _IsFolder; } set { _IsFolder = value; } }
		
		private DateTime _LastModified;
		public DateTime LastModified 
		{ get { return _LastModified; } set { _LastModified = value; } }
		
		private bool _AddedAtRuntime;
		public bool AddedAtRuntime 
		{ get { return _AddedAtRuntime; } set { _AddedAtRuntime = value; } }

	}

}
