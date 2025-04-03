using System;
using STL.Common.Printing.AgreementPrinting;
using System.Data;
using System.Xml;
using STL.Common.Constants.ColumnNames;
using System.Configuration;
using STL.Common.Constants.Tags;
using STL.Common.Constants.ItemTypes;
using STL.Common.Constants.Elements;

namespace STL.Common
{
    /// <summary>
    /// Summary description for CashLoanXML.
    /// </summary>
    public class CashLoanXML : PrintXML
    {
        public CashLoanXML(string country)
        {
            this.XmlTemplate = XMLTemplates.AgreementXML;
            this.CountryName = country;
            this.SetXsltPath("CashLoan.xslt");
        }

        //CR 440 - Capability to use a different xslt file as required
        public CashLoanXML(string country, string xsltFilename)
        {
            this.XmlTemplate = XMLTemplates.AgreementXML;
            this.CountryName = country;
            this.SetXsltPath(xsltFilename);
        }

        private decimal _goodsValue = 0;
        public decimal GoodsValue
        {
            get { return _goodsValue; }
        }

        public void AddLineItem(XmlNode item,
                                ref int items,
                                ref decimal goodsValue)
        {
            decimal quantity = 0;
            decimal itemValue = 0;

            if (item.Attributes[Tags.Type].Value != IT.Kit &&
                Convert.ToInt32(item.Attributes[Tags.ItemId].Value) != StockItemCache.Get(StockItemKeys.DT) &&
                Convert.ToDouble(item.Attributes[Tags.Quantity].Value) > 0)
            {
                quantity = Convert.ToDecimal(item.Attributes[Tags.Quantity].Value);
                itemValue = Convert.ToDecimal(item.Attributes[Tags.Value].Value);

                XmlNode node = this.GetNode("AGREEMENT/LINEITEMS");
                XmlNode itemNode = _doc.CreateElement("LINEITEM");
                XmlNode itemNoNode = _doc.CreateElement("ITEMNO");
                XmlNode qtyNode = _doc.CreateElement("QUANTITY");
                XmlNode priceNode = _doc.CreateElement("VALUE");
                XmlNode descNode = _doc.CreateElement("DESC");
                XmlNode desc2Node = _doc.CreateElement("DESC2");
                XmlNode trimNode = _doc.CreateElement("TRIM");

                qtyNode.InnerText = quantity.ToString("F2");
                priceNode.InnerText = itemValue.ToString("C2");
                descNode.InnerText = item.Attributes[Tags.Description1].Value;
                desc2Node.InnerText = item.Attributes[Tags.Description2].Value;
                trimNode.InnerText = item.Attributes[Tags.ColourTrim].Value;
                itemNoNode.InnerText = item.Attributes[Tags.Code].Value;

                itemNode.AppendChild(qtyNode);
                itemNode.AppendChild(priceNode);
                itemNode.AppendChild(descNode);
                itemNode.AppendChild(desc2Node);
                itemNode.AppendChild(trimNode);
                itemNode.AppendChild(itemNoNode);
                node.AppendChild(itemNode);

                goodsValue += itemValue;

                if (items > STL.Common.Printing.AgreementPrinting.Document.ItemsPerPage)
                {
                    XmlNode pb = _doc.CreateElement("PB");
                    itemNode.AppendChild(pb);
                    items = 0;
                }
                items++;

                //recurse
                XmlNode related = item.SelectSingleNode(Elements.RelatedItem);
                foreach (XmlNode child in related.ChildNodes)
                    AddLineItem(child, ref items, ref goodsValue);
            }
            else
            {
                if (item.Attributes[Tags.Type].Value == IT.Kit)
                {
                    AddKitLineItem(item, ref items, ref goodsValue);
                }
            }
        }

        public void AddKitLineItem(XmlNode item, ref int items, ref decimal goodsValue)
        {
            decimal quantity = 0;
            decimal kitValue = 0;

            quantity = Convert.ToDecimal(item.Attributes[Tags.Quantity].Value);

            XmlNode node = this.GetNode("AGREEMENT/LINEITEMS");
            XmlNode itemNode = _doc.CreateElement("LINEITEM");
            XmlNode qtyNode = _doc.CreateElement("QUANTITY");
            XmlNode priceNode = _doc.CreateElement("VALUE");
            XmlNode descNode = _doc.CreateElement("DESC");
            XmlNode itemNoNode = _doc.CreateElement("ITEMNO");

            qtyNode.InnerText = quantity.ToString("F2");
            descNode.InnerText = item.Attributes[Tags.Description1].Value;
            itemNoNode.InnerText = item.Attributes[Tags.Code].Value;

            itemNode.AppendChild(qtyNode);
            itemNode.AppendChild(priceNode);
            itemNode.AppendChild(descNode);
            itemNode.AppendChild(itemNoNode);

            node.AppendChild(itemNode);

            if (items == STL.Common.Printing.AgreementPrinting.Document.ItemsPerPage)
            {
                XmlNode pb = _doc.CreateElement("PB");
                itemNode.AppendChild(pb);
                items = 0;
            }
            items++;

            XmlNode related = item.SelectSingleNode(Elements.RelatedItem);
            foreach (XmlNode comp in related.ChildNodes)
                AddComponent(comp, ref kitValue, ref items, ref goodsValue);

            priceNode.InnerText = kitValue.ToString("C2");
            goodsValue += kitValue;
        }

        private void AddComponent(XmlNode component, ref decimal kitValue, ref int items, ref decimal goodsValue)
        {
            /* JJ - components should not be itemised on the agreement, but if
             * the kit has a warranty sold with it this should be displayed */
            if (component.Attributes[Tags.Type].Value == IT.KitWarranty)
            {
                XmlNode node = this.GetNode("AGREEMENT/LINEITEMS");
                XmlNode itemNode = _doc.CreateElement("LINEITEM");
                XmlNode qtyNode = _doc.CreateElement("QUANTITY");
                XmlNode priceNode = _doc.CreateElement("VALUE");
                XmlNode descNode = _doc.CreateElement("DESC");
                XmlNode itemNoNode = _doc.CreateElement("ITEMNO");

                qtyNode.InnerText = Convert.ToDecimal(component.Attributes[Tags.Quantity].Value).ToString("F2");
                descNode.InnerText = component.Attributes[Tags.Description1].Value;
                priceNode.InnerText = Convert.ToDecimal(component.Attributes[Tags.Value].Value).ToString("C2");
                itemNoNode.InnerText = component.Attributes[Tags.Code].Value;

                itemNode.AppendChild(qtyNode);
                itemNode.AppendChild(priceNode);
                itemNode.AppendChild(descNode);
                itemNode.AppendChild(itemNoNode);

                node.AppendChild(itemNode);

                if (items == STL.Common.Printing.AgreementPrinting.Document.ItemsPerPage)
                {
                    XmlNode pb = _doc.CreateElement("PB");
                    itemNode.AppendChild(pb);
                    items = 0;
                }
                items++;

                goodsValue += Convert.ToDecimal(component.Attributes[Tags.Value].Value);
            }
            else
                kitValue += Convert.ToDecimal(component.Attributes[Tags.Value].Value);

            //recurse
            XmlNode related = component.SelectSingleNode(Elements.RelatedItem);
            foreach (XmlNode comp in related.ChildNodes)
                AddComponent(comp, ref kitValue, ref items, ref goodsValue);
        }

        public void CreateCopies(int copies, int custCopies)
        {
            /* duplicate the AGREEMENT node x times and append it 
             * to an AGREEMENTS node */
            XmlDocument x = new XmlDocument();
            x.LoadXml("<AGREEMENTS/>");

            for (int i = 0; i < custCopies; i++)
            {
                SetNode("AGREEMENT/CUSTOMER", "TRUE");
                SetNode("AGREEMENT/LAST", "FALSE");
                XmlNode duplicate = _doc.DocumentElement;
                duplicate = x.ImportNode(duplicate, true);
                x.DocumentElement.AppendChild(duplicate);
            }

            for (int i = 0; i < copies; i++)
            {
                string last = i == copies - 1 ? "TRUE" : "FALSE";
                SetNode("AGREEMENT/LAST", last);
                SetNode("AGREEMENT/CUSTOMER", "FALSE");
                XmlNode duplicate = _doc.DocumentElement;
                duplicate = x.ImportNode(duplicate, true);
                x.DocumentElement.AppendChild(duplicate);
            }
            _doc = x;
        }

        public void Initialise()
        {

        }


    }
}
