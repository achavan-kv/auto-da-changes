using System;
using STL.Common.Printing.AgreementPrinting;
using System.Data;
using System.Xml;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.Tags;
using STL.Common.Constants.Elements;
using STL.Common.Constants.ItemTypes;
using STL.Common.Constants.TableNames;

namespace STL.Common
{
	/// <summary>
	/// Summary description for StatementOfAccountXML.
	/// </summary>
	public class StatementOfAccountXML : PrintXML
	{
		public StatementOfAccountXML(string country)
		{
			this.XmlTemplate = XMLTemplates.StatementOfAccountXML;
			this.CountryName = country;
			this.SetXsltPath("StatementOfAccount.xslt");
		}
	}
}
