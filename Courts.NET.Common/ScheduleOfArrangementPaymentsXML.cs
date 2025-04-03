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
    public class ScheduleOfArrangementPaymentsXML : PrintXML
    {
        public ScheduleOfArrangementPaymentsXML(string country)
		{
            this.XmlTemplate = XMLTemplates.ScheduleOfArrangementPaymentsXML;
			this.CountryName = country;
            this.SetXsltPath("ScheduleOfArrangementPayments.xslt");
		}
    }
}
