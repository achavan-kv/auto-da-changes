using System;
using System.Xml;
using System.Reflection;
using System.IO;
using STL.Common.Constants.Elements;

namespace STL.PL.XMLPrinting
{
	/// <summary>
	/// structure containing constant strings for the various 
	/// print types
	/// </summary>
	public struct PrintType
	{
		public const string Agreement = "Agreement";
		public const string TaxInvoice = "TaxInvoice";
		public const string RFSummary = "RFSummary";
		public const string DeliveryNote = "DeliveryNote";
		public const string CollectionNote = "CollectionNote";
		public const string WarrantyContract = "WarrantyContract";
	}

	/// <summary>
	/// Summary description for XMLPrint.
	/// </summary>
	public class XMLPrint
	{
		private static PrinterProperties _propertiesForm = null;
		private static XmlDocument _propertiesDoc = null;
		private static string _path = "";

		public XMLPrint()
		{
			
		}

		private static void Initialise()
		{
			/* read the existing printer properties from local disc */
			if(_propertiesDoc == null)
			{
				Assembly asm = Assembly.GetExecutingAssembly();
				_path = asm.Location.Substring(0, asm.Location.ToLower().IndexOf("courts.net.pl.exe"));
				_path += "PrinterProperties.xml";
				_propertiesDoc = new XmlDocument();

				FileInfo fileInfo = new FileInfo(_path);
				if(fileInfo.Exists)				
					_propertiesDoc.Load(fileInfo.FullName);
				else
					CreatePropertiesDocument();
			}
		}

		private static void CreatePropertiesDocument()
		{
			XmlNode pp = _propertiesDoc.CreateElement(Elements.PrinterProperties);
			
			Type t = typeof(PrintType);
			FieldInfo[] f = t.GetFields();

			foreach(FieldInfo fi in f)
			{
				XmlNode printType = _propertiesDoc.CreateElement(fi.Name);
				XmlNode printer = _propertiesDoc.CreateElement(Elements.Printer);
				XmlNode tray = _propertiesDoc.CreateElement(Elements.Tray);
				printType.AppendChild(printer);
				printType.AppendChild(tray);
				pp.AppendChild(printType);
			}
			_propertiesDoc.LoadXml(pp.OuterXml);
		}

		public static void SetPrinterProperties(string[] printTypes, bool force)
		{
			Initialise();
			_propertiesForm  = new PrinterProperties(printTypes, _propertiesDoc);
			if(_propertiesForm.ShowScreen || force)
				_propertiesForm.ShowDialog();
			_propertiesDoc.Save(_path);
		}

		public static string[] PrintTypes()
		{
			Type t = typeof(PrintType);
			FieldInfo[] f = t.GetFields();
			string[] types = new string[f.Length];

			for(int i=0; i<f.Length; i++)
				types[i] = f[i].Name;

			return types;
		}

		public static string Source(string printType)
		{
			if(_propertiesDoc == null)
				Initialise();
			return _propertiesDoc.DocumentElement[printType][Elements.Tray].InnerText;
		}

		public static string Printer(string printType)
		{
			if(_propertiesDoc == null)
				Initialise();
			return  _propertiesDoc.DocumentElement[printType][Elements.Printer].InnerText;
		}
	}
}
