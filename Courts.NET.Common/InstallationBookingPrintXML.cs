using System;
using System.Collections.Generic;
using System.Text;
using STL.Common.Printing.AgreementPrinting;
using System.Xml;
using STL.Common.Constants.ColumnNames;


namespace STL.Common
{
    public class InstallationBookingPrintXML : PrintXML
    {
        public InstallationBookingPrintXML(string country)
        {
            XmlTemplate = XMLTemplates.InstallationBookingPrintXML;
            CountryName = country;
            SetXsltPath("InstallationBooking.xslt");
        }

        public InstallationBookingPrintXML Create(string country)
        {
            InstallationBookingPrintXML printXml = new InstallationBookingPrintXML(country);
            printXml.Load();
            return printXml;
        }

        public void ImportNode(XmlNode toImport, bool deep)
        {
            toImport = _doc.ImportNode(toImport, deep);
            _doc.DocumentElement.AppendChild(toImport);
        }

        public XmlElement DocumentElement
        {
            get { return _doc.DocumentElement; }
        }

        public void CreateCopies(int noOfCopies)
        {
            XmlDocument x = new XmlDocument();
            x.LoadXml("<BOOKINGS/>");

            XmlNode lastPageIndicatorNode;

            for (int i = 0; i < noOfCopies; i++)
            {
                foreach (XmlNode childNode in _doc.DocumentElement.ChildNodes)
                {
                    //--Initially setting the lastpage indicator to FALSE
                    lastPageIndicatorNode = childNode.SelectSingleNode("LAST");
                    if (lastPageIndicatorNode != null)
                        lastPageIndicatorNode.InnerText = "FALSE";

                    XmlNode tempNode = x.ImportNode(childNode, true);
                    x.DocumentElement.AppendChild(tempNode);
                }
            }

            //--Now setting the lastpage indicator to TRUE only for the last child node
            lastPageIndicatorNode = x.DocumentElement.LastChild.SelectSingleNode("LAST");
            if (lastPageIndicatorNode != null)
                lastPageIndicatorNode.InnerText = "TRUE";

            _doc = x;
        }
    }
}
