using System;
using STL.Common.Printing.AgreementPrinting;
using System.Data;
using System.Xml;
using STL.Common.Constants.ColumnNames;
using System.Collections;

namespace STL.Common
{
	/// <summary>
	/// Summary description for PickListXML.
	/// </summary>
	public class PickListXML : PrintXML
	{
		public PickListXML(string country, bool isOrderPicklist)
		{
			this.XmlTemplate = XMLTemplates.PickListXML;
			this.CountryName = country;
			
			if(isOrderPicklist)
				this.SetXsltPath("PickList.xslt");
			else
				this.SetXsltPath("TransportPickList.xslt");
		}

		public void AddLineItem(DataRowView r, bool deliveries,ArrayList branches, ref int items)
		{
			XmlNode node = this.GetNode("PICKLIST/DELIVERYNOTES");
			XmlNode item = _doc.CreateElement("DELIVERYNOTE");

			XmlNode buffNo = _doc.CreateElement("BUFFNO");
			XmlNode category = _doc.CreateElement("CATEGORY");
			XmlNode code = _doc.CreateElement("ITEMNO");
            XmlNode locn = _doc.CreateElement("LOCN");
			XmlNode desc1 = _doc.CreateElement("DESC1");
			XmlNode qty = _doc.CreateElement("QUANTITY");
			XmlNode loadNo = _doc.CreateElement("LOADNO");
            XmlNode storeType = _doc.CreateElement("STORETYPE");

             string acctNo = r[CN.acctno].ToString();
             if (branches.Contains(acctNo.Substring(0, 3)))
             {
                storeType.InnerText = "Courts";
             }
             else
             {
                storeType.InnerText = "Non Courts";
             }

			buffNo.InnerText = Convert.ToInt32(r[CN.BuffNo]).ToString();
            //category.InnerText = Convert.ToInt16(r[CN.Category]).ToString();
            category.InnerText = Convert.ToString(r[CN.Category]);                              //IP - 22/09/11 - RI - #8224 - CR8201
            //code.InnerText = r[CN.ItemNo].ToString();
            locn.InnerText = r[CN.StockLocn].ToString();

            //IP - 22/09/11 - RI - #8224 - CR8201
            if (Convert.ToString(Country[CountryParameterNames.RIInterfaceOptions]) != "FACT")
            {
                code.InnerText = r[CN.ItemNo].ToString() + " " + r[CN.CourtsCode].ToString();
                desc1.InnerText = r[CN.ItemDescr1].ToString() + " " + r[CN.Brand].ToString() + " " + r[CN.Style].ToString();
            }
            else
            {
                code.InnerText = r[CN.ItemNo].ToString();
                desc1.InnerText = r[CN.ItemDescr1].ToString();
            }
			
			qty.InnerText = Convert.ToDecimal(r[CN.Quantity]).ToString();
			loadNo.InnerText = Convert.ToDecimal(r[CN.LoadNo]).ToString();

			item.AppendChild(buffNo);
			item.AppendChild(category);
			item.AppendChild(code);
            item.AppendChild(locn);
            item.AppendChild(desc1);
			item.AppendChild(qty);
			item.AppendChild(loadNo);
            item.AppendChild(storeType);

			if(items==STL.Common.Printing.AgreementPrinting.Document.PickListItems)
			{
				XmlNode pb = _doc.CreateElement("PB");
				item.AppendChild(pb);
				items = 0;
			}
			
			node.AppendChild(item);
		}

		public PickListXML CreatePickList(string country, bool isOrderPicklist)
		{
			PickListXML dn = new PickListXML(country, isOrderPicklist);
			dn.Load();
			return dn;
		}

		public void ImportNode(XmlNode toImport, bool deep)
		{
			toImport = _doc.ImportNode(toImport, deep);
			_doc.DocumentElement.AppendChild(toImport);
		}

		public XmlElement DocumentElement
		{
			get{return _doc.DocumentElement;}
		}
	}
}
