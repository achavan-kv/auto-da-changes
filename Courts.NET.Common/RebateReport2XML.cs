using System;
using STL.Common.Printing.AgreementPrinting;
using System.Data;
using System.Xml;
using STL.Common.Constants.ColumnNames;

namespace STL.Common
{
	/// <summary>
	/// Summary description for RebateReport.
	/// </summary>
	public class RebateReport2XML : PrintXML
	{
		public RebateReport2XML(string country)
		{
			this.CountryName = country;
			this.SetXsltPath("RebateReport2.xslt");
		}

		public XmlElement DocumentElement
		{
			get{return _doc.DocumentElement;}
		}
	}
}
