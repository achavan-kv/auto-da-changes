using System;
using STL.Common.Printing.AgreementPrinting;
using System.Data;
using System.Xml;
using STL.Common.Constants.ColumnNames;

namespace STL.Common
{
    /// <summary>
    /// Class for creating XML for delivery note. Sets XsltPath and adds CSS Path. Checks decimal places
    /// </summary>
    public class DeliveryNoteXML_Printing : PrintXMLClient
    {
        new int DecimalPlacesNo;
        new string DecimalPlaces;

        public DeliveryNoteXML_Printing(string country, string DPlaces)
        {
            this.XmlTemplate = XMLTemplates.DeliveryNoteXML;
            this.CountryName = country;
            this.SetXsltPath("DeliveryNote.xslt");

            DecimalPlaces = DPlaces;

            int number;
            if (int.TryParse(DPlaces.Substring(1, 1), out number))
                DecimalPlacesNo = number;
            else
                DecimalPlacesNo = 2;
        }

        /// <summary>
        /// Add css path for styles.css
        /// </summary>
        public void AddCSSPath()
        {
            XmlAttribute attribute;

            XmlNode node = _doc.SelectSingleNode("DELIVERYNOTES");
            if (node != null)
            {
                attribute = node.OwnerDocument.CreateAttribute("CSSPATH");
                attribute.Value = System.Windows.Forms.Application.StartupPath + @"\Stylesheets\" + "styles.css";
                node.Attributes.Append(attribute);
            }

        }

        /// <summary>
        /// Add collection item adds item details for item, qty,priceNode,desc1,desc2,itemNode,notesNode
        /// </summary>
        public void AddCollectionItem(string itemDescr, decimal quantity, string price,
                                      string itemNo, string itemDescr2, string notes, string supplier, ref int items)  //UAT54 jec 01/04/10  ref CR1048
        {
            XmlNode node = this.GetNode("DELIVERYNOTE/LINEITEMS");
            XmlNode item = _doc.CreateElement("LINEITEM");
            XmlNode qty = _doc.CreateElement("QUANTITY");
            XmlNode priceNode = _doc.CreateElement("PRICE");
            XmlNode desc1 = _doc.CreateElement("DESC1");
            XmlNode desc2 = _doc.CreateElement("DESC2");
            XmlNode itemNode = _doc.CreateElement("ITEMNO");
            XmlNode notesNode = _doc.CreateElement("NOTES");
            XmlNode supplierNode = _doc.CreateElement("SUPPLIER"); //UAT54 jec 01/04/10  ref CR1048
	
            qty.InnerText = quantity.ToString("F2");
            priceNode.InnerText = Convert.ToDecimal(price).ToString("C2");
            desc1.InnerText = itemDescr;
            desc2.InnerText = itemDescr2;
            itemNode.InnerText = itemNo;
            notesNode.InnerText = notes;
            supplierNode.InnerText = supplier; //UAT54 jec 01/04/10  ref CR1048

            item.AppendChild(qty);
            item.AppendChild(priceNode);
            item.AppendChild(desc1);
            item.AppendChild(desc2);
            item.AppendChild(itemNode);
            item.AppendChild(notesNode);
            item.AppendChild(supplierNode); //UAT54 jec 01/04/10  ref CR1048

            if (items == STL.Common.Printing.AgreementPrinting.Document.ItemsPerPage)
            {
                XmlNode pb = _doc.CreateElement("PB");
                item.AppendChild(pb);
                items = 0;
            }
            node.AppendChild(item);
        }

        /// <summary>
        /// Add AddLineitem - setting new xml nodes for item,qty,price etc.
        /// </summary>
        public void AddLineItem(DataRow r, int numLineItems, ref int items)
        {
            XmlNode node = this.GetNode("DELIVERYNOTE/LINEITEMS");
            XmlNode item = _doc.CreateElement("LINEITEM");
            XmlNode qty = _doc.CreateElement("QUANTITY");
            XmlNode price = _doc.CreateElement("PRICE");
            XmlNode desc1 = _doc.CreateElement("DESC1");
            XmlNode desc2 = _doc.CreateElement("DESC2");
            XmlNode notes = _doc.CreateElement("NOTES");
            XmlNode itemNo = _doc.CreateElement("ITEMNO");
            XmlNode returnDetails = _doc.CreateElement("RETDETAILS");
            XmlNode supplier = _doc.CreateElement("SUPPLIER"); //UAT54 jec 01/04/10  ref CR1048
            XmlNode comments = _doc.CreateElement("GRTNOTES"); //CR 158

            decimal lineQty = Convert.ToDecimal(r[CN.Quantity]);

            qty.InnerText = lineQty.ToString("F2");
            // UAT 349 If item is a redelivery after repossession then the price should be the return value JH 16/10/2007
            decimal itemPrice;
            if (r[CN.DelOrColl].ToString() == "R" && r["retval"] != DBNull.Value && lineQty > 0)
            {
                itemPrice = Convert.ToDecimal(lineQty * Convert.ToDecimal(r["retval"]));
                itemPrice = Math.Round(itemPrice, DecimalPlacesNo, MidpointRounding.AwayFromZero);
            }
            else
            {

                itemPrice = Convert.ToDecimal(lineQty * Convert.ToDecimal(r[CN.Price]));
                itemPrice = Math.Round(itemPrice, DecimalPlacesNo, MidpointRounding.AwayFromZero);
            }
            price.InnerText = itemPrice.ToString("#0.00");


            //IP - 20/09/11 - RI - #8220 - CR8201
            if (Convert.ToString(Country[CountryParameterNames.RIInterfaceOptions]) != "FACT")
            {
                desc1.InnerText = r[CN.ItemDescr1].ToString() + " " + r[CN.Brand].ToString() + " " + r[CN.Style].ToString();
            }
            else
            {
                desc1.InnerText = r[CN.ItemDescr1].ToString();
            }

            desc2.InnerText = r[CN.ItemDescr2].ToString();
            notes.InnerText = r[CN.ItemNotes].ToString();
            itemNo.InnerText = r[CN.ItemNo].ToString();
            supplier.InnerText = r[CN.Supplier].ToString();  //UAT54 jec 01/04/10  ref CR1048
            comments.InnerText = r["grtnotes"].ToString();

            if ((string)r[CN.DelOrColl] == "R" && lineQty > 0)
                returnDetails.InnerText = "(" + Convert.ToInt16(r[CN.StockLocn]).ToString() + "/" + r[CN.RetItemNo].ToString() + ")";
            else
                returnDetails.InnerText = "";

            item.AppendChild(qty);
            item.AppendChild(price);
            item.AppendChild(desc1);
            item.AppendChild(desc2);
            item.AppendChild(notes);
            item.AppendChild(itemNo);
            item.AppendChild(returnDetails);
            item.AppendChild(supplier); //UAT54 jec 01/04/10  ref CR1048
            item.AppendChild(comments); //UAT 

            //IP - 12/03/08 - (69461)
            //Add a page break after the fifth item.
            if (items % STL.Common.Printing.AgreementPrinting.Document.ItemsPerPage == 0 && numLineItems > items)
            {
                XmlNode pb = _doc.CreateElement("PB");
                item.AppendChild(pb);
                //items = 0;
            }
            node.AppendChild(item);
        }

        public XmlElement DocumentElement
        {
            get { return _doc.DocumentElement; }
        }

        /// <summary>
        /// Creates New Instance of DeliveryNoteXML_Printing then loads the XML
        /// </summary>
        public DeliveryNoteXML_Printing CreateDeliveryNote(string country, string decimalplaces)
        {
            DeliveryNoteXML_Printing dn = new DeliveryNoteXML_Printing(country, decimalplaces);
            dn.Load();
            return dn;
        }

        public void ImportNode(XmlNode toImport, bool deep)
        {
            toImport = _doc.ImportNode(toImport, deep);
            _doc.DocumentElement.AppendChild(toImport);
        }

        public void AddCollectionItem(XmlNode collectionNode, ref int items)
        {
            XmlNode node = this.GetNode("DELIVERYNOTE/LINEITEMS");
            XmlNode item = _doc.CreateElement("LINEITEM");
            XmlNode qty = _doc.CreateElement("QUANTITY");
            XmlNode priceNode = _doc.CreateElement("PRICE");
            XmlNode desc1 = _doc.CreateElement("DESC1");
            XmlNode desc2 = _doc.CreateElement("DESC2");
            XmlNode itemNode = _doc.CreateElement("ITEMNO");
            XmlNode notesNode = _doc.CreateElement("NOTES");
            XmlNode supplierNode = _doc.CreateElement("SUPPLIER"); //UAT54 jec 01/04/10  ref CR1048

            qty.InnerText = Convert.ToDecimal(collectionNode.SelectSingleNode("QUANTITY").InnerText).ToString("F2");
            priceNode.InnerText = Convert.ToDecimal(collectionNode.SelectSingleNode("PRICE").InnerText).ToString("C2");
            desc1.InnerText = collectionNode.SelectSingleNode("DESC1").InnerText;
            desc2.InnerText = collectionNode.SelectSingleNode("DESC2").InnerText;
            itemNode.InnerText = collectionNode.SelectSingleNode("ITEMNO").InnerText;
            notesNode.InnerText = collectionNode.SelectSingleNode("NOTES").InnerText;
            supplierNode.InnerText = collectionNode.SelectSingleNode("SUPPLIER").InnerText; //UAT54 jec 01/04/10  ref CR1048

            item.AppendChild(qty);
            item.AppendChild(priceNode);
            item.AppendChild(desc1);
            item.AppendChild(desc2);
            item.AppendChild(itemNode);
            item.AppendChild(notesNode);
            item.AppendChild(supplierNode); //UAT54 jec 01/04/10  ref CR1048

            if (items == STL.Common.Printing.AgreementPrinting.Document.ItemsPerPage)
            {
                XmlNode pb = _doc.CreateElement("PB");
                item.AppendChild(pb);
                items = 0;
            }
            node.AppendChild(item);
        }
    }
}
