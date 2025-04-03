using System;
using System.Data;
using System.Xml;
using STL.Common.Constants.ColumnNames;
using STL.Common.Printing.AgreementPrinting;

namespace STL.Common
{
	/// <summary>
	/// Summary description for DeliveryNoteXML.
	/// </summary>
	public class OneForOneReplacementNoteXML : PrintXML
	{
		public OneForOneReplacementNoteXML(string country)
		{
			this.XmlTemplate = XMLTemplates.OneForOneReplacementNoteXML;
			this.CountryName = country;
			this.SetXsltPath("OneForOneReplacementNote.xslt");
			
		}
	}
}
