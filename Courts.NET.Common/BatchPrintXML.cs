using System;
using System.Collections.Generic;
using System.Text;
using STL.Common.Printing.AgreementPrinting;
using System.Xml;
using STL.Common.Constants.ColumnNames;


namespace STL.Common
{
    public class BatchPrintXML : PrintXMLClient
    {
        public BatchPrintXML(string country)
        {
            this.XmlTemplate = XMLTemplates.BatchPrintXML;
            this.CountryName = country;
            this.SetXsltPath("BatchPrinting.xslt");
        }

        public BatchPrintXML CreateBatch(string country)
        {
            BatchPrintXML dn = new BatchPrintXML(country);
            dn.Load();
            return dn;
        }

        public XmlNode ImportNode(XmlNode toImport, bool deep)
        {
            toImport = _doc.ImportNode(toImport, deep);
            _doc.DocumentElement.AppendChild(toImport);
            return toImport;
        }

        public XmlElement DocumentElement
        {
            get { return _doc.DocumentElement; }
        }

  

        public void AddPart(XmlNode collectionNode, ref int items)
        {
            XmlNode node = this.GetNode("BATCHPRINT/PARTS");
            XmlNode part = _doc.CreateElement("PART");
            XmlNode qty = _doc.CreateElement("QUANTITY");
            XmlNode type = _doc.CreateElement("TYPE");
            XmlNode partNo = _doc.CreateElement("PARTNO");
            

            //qty.InnerText = collectionNode.SelectSingleNode("QUANTITY").InnerText.ToString();
            type.InnerText = collectionNode.SelectSingleNode("TYPE").InnerText.ToString();
            if (collectionNode.SelectSingleNode("QUANTITY").InnerText == "")
            {
                partNo.InnerText = collectionNode.SelectSingleNode("PARTNO").InnerText;
            }
            else
            {
                partNo.InnerText = collectionNode.SelectSingleNode("QUANTITY").InnerText.ToString() + " X " + collectionNode.SelectSingleNode("PARTNO").InnerText;
            }

            part.AppendChild(qty);
            part.AppendChild(type);
            part.AppendChild(partNo);
            
            node.AppendChild(part);
        }

        //--CR 1024 - New method to duplicate XML (NM 23/04/2009)-------------
        public BatchPrintXML CreateCopies(int noOfCopies, string countrycode)
        {
            //-- duplicate the BATCHPRINT node x times and append it to a BATCHPRINTS node
            BatchPrintXML batchPrints = new BatchPrintXML(countrycode);
            batchPrints.Load("<BATCHPRINTS/>");
            batchPrints.AddCSSPath("BATCHPRINTS");

            XmlNode lastPageIndicatorNode;

            for (int i = 0; i < noOfCopies; i++)
            {
                foreach (XmlNode childNode in _doc.DocumentElement.ChildNodes)
                {
                    //--Initially setting the lastpage indicator to FALSE
                    lastPageIndicatorNode = childNode.SelectSingleNode("LAST");
                    if (lastPageIndicatorNode != null)
                        lastPageIndicatorNode.InnerText = "FALSE";

                    XmlNode tempNode = batchPrints.ImportNode(childNode, true);
                    batchPrints.DocumentElement.AppendChild(tempNode);
                }
            }

            //--Now setting the lastpage indicator to TRUE only for the last child node
            lastPageIndicatorNode = batchPrints.DocumentElement.LastChild.SelectSingleNode("LAST");
            if (lastPageIndicatorNode != null)
                lastPageIndicatorNode.InnerText = "TRUE";

            return batchPrints;
        }
        //--------------------------------------------------------------------
    }
}
