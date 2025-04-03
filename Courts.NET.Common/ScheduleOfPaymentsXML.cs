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
	/// Summary description for ScheduleOfPaymentsXML.
	/// </summary>
	public class ScheduleOfPaymentsXML : PrintXML
	{
		public ScheduleOfPaymentsXML(string country)
		{
			this.XmlTemplate = XMLTemplates.ScheduleOfPaymentsXML;
			this.CountryName = country;
			this.SetXsltPath("ScheduleOfPayments.xslt");
		}
	}
}
