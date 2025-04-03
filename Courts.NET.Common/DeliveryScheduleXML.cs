using System;
using STL.Common.Printing.AgreementPrinting;
using System.Data;
using System.Xml;
using STL.Common.Constants.ColumnNames;


namespace STL.Common
{
	/// <summary>
	/// Summary description for DeliveryScheduleXML.
	/// </summary>
	public class DeliveryScheduleXML : PrintXML
	{
		public DeliveryScheduleXML(string country)
		{
			this.XmlTemplate = XMLTemplates.DeliveryScheduleXML;
			this.CountryName = country;
			this.SetXsltPath("DeliverySchedule.xslt");
		}

		public DeliveryScheduleXML CreateDeliveryNote(string country)
		{
			DeliveryScheduleXML ds = new DeliveryScheduleXML(country);
			ds.Load();
			return ds;
		}

		public void ImportNode(XmlNode toImport, bool deep)
		{
			toImport = _doc.ImportNode(toImport, deep);
			_doc.DocumentElement.AppendChild(toImport);
		}

		public XmlElement DocumentElement
		{
			get{return _doc.DocumentElement;}
		}
	}
}
