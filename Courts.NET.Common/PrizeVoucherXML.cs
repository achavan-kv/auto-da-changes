using System;
using STL.Common.Printing.AgreementPrinting;
using System.Data;
using System.Xml;
using STL.Common.Constants.ColumnNames;

namespace STL.Common
{
    public class PrizeVoucherXML : PrintXML
    {
        public PrizeVoucherXML(string country)
		{
			this.CountryName = country;
			this.SetXsltPath("PrizeVoucher.xsl");
		}

        public void ImportNode(XmlNode toImport, bool deep)
        {
            toImport = _doc.ImportNode(toImport, deep);
            _doc.DocumentElement.AppendChild(toImport);
        }
    }
}
