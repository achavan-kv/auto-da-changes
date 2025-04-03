using System;
using System.Xml;

namespace STL.Common
{
	/// <summary>
	/// Summary description for CommissionPaymentXML.
	/// </summary>
	public class CommissionPaymentXML : PrintXML
	{
		public CommissionPaymentXML(string country, bool rePrint)
		{
			this.CountryName = country;
			
			if(!rePrint)
				this.SetXsltPath("CommissionPayment.xslt");
			else
				this.SetXsltPath("CommissionPaymentReprint.xslt");
		}

		public void ImportNode(XmlNode toImport, bool deep)
		{
			toImport = _doc.ImportNode(toImport, deep);
			_doc.DocumentElement.AppendChild(toImport);
		}
	}
}
