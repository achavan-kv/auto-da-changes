using System;
using STL.Common.Printing.AgreementPrinting;
using System.Data;
using System.Xml;
using STL.Common.Constants.ColumnNames;

namespace STL.Common
{
    public class StoreCardStatementXML : PrintXMLClient
    {
        public StoreCardStatementXML(string country)
		{
			this.XmlTemplate = XMLTemplates.StoreCardStatementXML;
			this.CountryName = country;
			this.SetXsltPath("StoreCardStatement.xslt");
		}


        /// <summary>
        /// Add css path for styles.css
        /// </summary>
        public void AddCSSPath()
        {
            XmlAttribute attribute;

            XmlNode node = _doc.SelectSingleNode("STORECARDSTATEMENT");
            if (node != null)
            {
                attribute = node.OwnerDocument.CreateAttribute("CSSPATH");
                attribute.Value = System.Windows.Forms.Application.StartupPath + @"\Stylesheets\" + "styles.css";
                node.Attributes.Append(attribute);
            }

        }

        /// <summary>
        /// Creates action sheet and calls PrintXML.load - which justs creates the XML file from a string
        /// </summary>
        public StoreCardStatementXML CreateActionSheet(string country)
        {
            StoreCardStatementXML dn = new StoreCardStatementXML(country);
            dn.Load();
            return dn;
        }

    }
}
