using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Reflection;
using System.IO;
using System.Xml;

namespace STL.PL
{
	/// <summary>
	/// This is an installer class used to implement a custom action as part
	/// of an msi package. It gathers information from the user to 
	/// populate the application config file at installation time. 
	/// </summary>
	[RunInstaller(true)]
	public class ConfigInstaller : System.Configuration.Install.Installer
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ConfigInstaller()
		{
			// This call is required by the Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitComponent call
		}
		public override void Install(System.Collections.IDictionary stateSaver)
		{
			base.Install(stateSaver);

			string webServiceUrl = "";
			string branch = "";
			string country = "";

			webServiceUrl = this.Context.Parameters["URL"];
			branch = this.Context.Parameters["BRANCH"];
			country = this.Context.Parameters["COUNTRY"];

			if(webServiceUrl.Length==0 ||
				branch.Length==0 ||
				country.Length==0)
			{
				throw new InstallException("Missing installation arguments");
			}
		
			Assembly asm = Assembly.GetExecutingAssembly();
			FileInfo fileInfo = new FileInfo(asm.Location + ".config");

			if(!fileInfo.Exists)
				throw new InstallException("Missing config file" + asm.Location+".config");
			
			//Load the config file into the XML DOM.
			XmlDocument xml = new XmlDocument();
			xml.Load(fileInfo.FullName);

			foreach (XmlNode node in xml["configuration"]["appSettings"])
			{
				if(node.Name == "add")
				{
					switch(node.Attributes["key"].Value)
					{
						case "WebServiceURL":	node.Attributes["value"].Value = webServiceUrl;
							break;
						case "CountryCode":		node.Attributes["value"].Value = country;
							break;
						case "BranchCode":		node.Attributes["value"].Value = branch;
							break;
						default:
							break;
					}
				}
			}
			//Write out the new config file.
			xml.Save(fileInfo.FullName);

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
