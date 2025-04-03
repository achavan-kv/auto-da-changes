using System;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using System.Reflection;
using System.Xml;
using STL.Common.Constants.Elements;
using System.IO;
using System.Windows.Forms;

namespace STL.PL
{
	/// <summary>
	/// This class checks whether a later version of the client application is
	/// available for download at the web server. Firstly a file is downloaded
	/// that contains the current version number of the client software at the
	/// server. If the version number at the server is different to that at the
	/// client, then the user is prompted that an update is available, and a
	/// server side process is invoked to download the new client version. The 
	/// current client application exits and the new version is then automatically
	/// invoked as the new client application.
	/// </summary>
	public class Updater : System.ComponentModel.Component
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Updater(System.ComponentModel.IContainer container)
		{
			/// <summary>
			/// Required for Windows.Forms Class Composition Designer support
			/// </summary>
			container.Add(this);
			InitializeComponent();
			common = new CommonForm();
		}
		private string _version = "";
		[Browsable(false)]
		public string Version
		{
			get{return _version;}
			set{_version = value;}
		}

		//private System.Windows.Forms.Form _main = null;

		private CommonForm common = null;

		private string _updatePath = "";
		public string UpdatePath
		{
			get{return _updatePath;}
			set{_updatePath = value;}
		}

		private int _pollInterval = 10;
		public int PollInterval
		{
			get{return _pollInterval;}
			set{_pollInterval = value;}
		}

		private bool _alive = true;
		[Browsable(false)]
		public bool Alive
		{
			get{return _alive;}
			set{_alive = value;}
		}

		public void CheckForUpdates()
		{
			try
			{
				while(this.Alive)
				{
				#if(DEBUG)
                    common.logMessage("Checking For updates", "UPDATER_THREAD", STL.PL.WS4.EventLogEntryType.Information);
				#endif

					string newVersion = "";
					string newVersionPath = "";

					//Load the xml file defined in the update path
					FileInfo file = new FileInfo(this.UpdatePath);

					if(!file.Exists)
						throw new Exception("Updates file cannot be found at "+UpdatePath+" .Updates thread terminating");
			
					//Load the updates file into the XML DOM.
					XmlDocument xml = new XmlDocument();
					xml.Load(file.FullName);


					foreach (XmlNode node in xml.DocumentElement.ChildNodes)
					{
						switch(node.Name)
						{
							case Elements.CurrentVersion:	newVersion = node.InnerText;
								break;
							case Elements.Installer:		newVersionPath = node.InnerText;
								break;
							default:
								break;
						}
					}

					//check that the available version is not higher
					//than the current version
					if(newVersion.Length==0||newVersionPath.Length==0)
					{
						common.ShowInfo("M_UPDATEERROR");
						break;
					}
					else
					{
						if(newVersion != this.Version)
						{
							common.ShowInfo("M_UPDATEAVAILABLE");
							Process proc = new Process();
							proc.StartInfo.FileName = file.DirectoryName + newVersionPath;
							proc.StartInfo.Arguments = "";
							proc.Start();
							Application.Exit();	
							break;
						}
						else
						{
							Thread.Sleep(TimeSpan.FromSeconds(this.PollInterval));
						}
					}
				}
			}
			catch(ThreadAbortException)
			{
				//don't bother to report this as it is normal when the 
				//application closes
			}
			catch(Exception ex)
			{
				common.Catch(ex, "CheckForUpdates");
			}
		}


		public Updater()
		{
			/// <summary>
			/// Required for Windows.Forms Class Composition Designer support
			/// </summary>
			InitializeComponent();
		}

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion
	}
}
