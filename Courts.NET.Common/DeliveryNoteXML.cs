using System;
using STL.Common.Printing.AgreementPrinting;
using System.Data;
using System.Xml;
using STL.Common.Constants.ColumnNames;

namespace STL.Common
{
	/// <summary>
	/// Summary description for DeliveryNoteXML.
	/// </summary>
	public class DeliveryNoteXML : PrintXMLClient
	{
		public DeliveryNoteXML(string country)
		{
			this.XmlTemplate = XMLTemplates.DeliveryNoteXML;
			this.CountryName = country;
			this.SetXsltPath("DeliveryNote.xslt");
			
		}

        //public void AddCSSPath()
        //{
        //    XmlNode path = _doc.CreateElement("CSSPATH");
        //    path.InnerText = System.Windows.Forms.Application.StartupPath + @"\Stylesheets\" + "styles.css";
        //}

		public void AddCollectionItem(string itemDescr, decimal quantity, string price,
                                      string itemNo, string itemDescr2, string notes, string supplier, ref int items)  //IP - 09/02/10 - CR1048 (Ref:3.1.2) Merged - Malaysia Enhancements (CR1072) - Added supplier
		{
			XmlNode node = this.GetNode("DELIVERYNOTE/LINEITEMS");
			XmlNode item = _doc.CreateElement("LINEITEM");
			XmlNode qty = _doc.CreateElement("QUANTITY");
			XmlNode priceNode = _doc.CreateElement("PRICE");
			XmlNode desc1 = _doc.CreateElement("DESC1");
			XmlNode desc2 = _doc.CreateElement("DESC2");
			XmlNode itemNode = _doc.CreateElement("ITEMNO");
			XmlNode notesNode = _doc.CreateElement("NOTES");
            XmlNode supplierNode = _doc.CreateElement("SUPPLIER"); //CR 1048 //IP - 09/02/10 - CR1048 (Ref:3.1.2) Merged - Malaysia Enhancements (CR1072)
	
			qty.InnerText = quantity.ToString("F2"); 
			priceNode.InnerText = Convert.ToDecimal(price).ToString("C2");
			desc1.InnerText = itemDescr;	
			desc2.InnerText = itemDescr2;			
			itemNode.InnerText = itemNo;
			notesNode.InnerText = notes;
            supplierNode.InnerText = supplier; //IP - 09/02/10 - CR1048 (Ref:3.1.2) Merged - Malaysia Enhancements (CR1072)

			item.AppendChild(qty);
			item.AppendChild(priceNode);
			item.AppendChild(desc1);
			item.AppendChild(desc2);
			item.AppendChild(itemNode);
			item.AppendChild(notesNode);
            item.AppendChild(supplierNode); //IP - 09/02/10 - CR1048 (Ref:3.1.2) Merged - Malaysia Enhancements (CR1072)

			if(items==STL.Common.Printing.AgreementPrinting.Document.ItemsPerPage)
			{
				XmlNode pb = _doc.CreateElement("PB");
				item.AppendChild(pb);
				items = 0;
			}
			node.AppendChild(item);
		}		

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
            XmlNode supplier = _doc.CreateElement("SUPPLIER"); //CR 1048 //IP - 09/02/10 - CR1048 (Ref:3.1.2) Merged - Malaysia Enhancements (CR1072)

			decimal lineQty = Convert.ToDecimal(r[CN.Quantity]);
			
			qty.InnerText = lineQty.ToString("F2");
         // UAT 349 If item is a redelivery after repossession then the price should be the return value JH 16/10/2007
            decimal itemPrice;
         if (r[CN.DelOrColl].ToString() == "R" && r["retval"] != DBNull.Value && lineQty > 0)
         {
             itemPrice = Convert.ToDecimal(lineQty * Convert.ToDecimal(r["retval"]));
             itemPrice=Math.Round(itemPrice,DecimalPlacesNo,MidpointRounding.AwayFromZero);
         }
         else
         {

             itemPrice = Convert.ToDecimal(lineQty * Convert.ToDecimal(r[CN.Price]));
             itemPrice = Math.Round(itemPrice, DecimalPlacesNo, MidpointRounding.AwayFromZero);
         }
         price.InnerText = itemPrice.ToString("#0.00");

			desc1.InnerText = r[CN.ItemDescr1].ToString();
			desc2.InnerText = r[CN.ItemDescr2].ToString();			
			notes.InnerText = r[CN.ItemNotes].ToString();
			itemNo.InnerText = r[CN.ItemNo].ToString();
            supplier.InnerText = r[CN.Supplier].ToString();  //IP - 09/02/10 - CR1048 (Ref:3.1.2) Merged - Malaysia Enhancements (CR1072)

			if((string)r[CN.DelOrColl] == "R" && lineQty > 0)
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
            item.AppendChild(supplier); //IP - 09/02/10 - CR1048 (Ref:3.1.2) Merged - Malaysia Enhancements (CR1072)

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
			get{return _doc.DocumentElement;}
		}

		public DeliveryNoteXML CreateDeliveryNote(string country)
		{
			DeliveryNoteXML dn = new DeliveryNoteXML(country);
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
            XmlNode supplierNode = _doc.CreateElement("SUPPLIER"); //CR 1048 //IP - 09/02/10 - CR1048 (Ref:3.1.2) Merged - Malaysia Enhancements (CR1072)
	
			qty.InnerText = Convert.ToDecimal(collectionNode.SelectSingleNode("QUANTITY").InnerText).ToString("F2"); 
			priceNode.InnerText = Convert.ToDecimal(collectionNode.SelectSingleNode("PRICE").InnerText).ToString("C2");
			desc1.InnerText = collectionNode.SelectSingleNode("DESC1").InnerText;	
			desc2.InnerText = collectionNode.SelectSingleNode("DESC2").InnerText;			
			itemNode.InnerText = collectionNode.SelectSingleNode("ITEMNO").InnerText;
			notesNode.InnerText = collectionNode.SelectSingleNode("NOTES").InnerText;
            supplierNode.InnerText = collectionNode.SelectSingleNode("SUPPLIER").InnerText; //IP - 09/02/10 - CR1048 (Ref:3.1.2) Merged - Malaysia Enhancements (CR1072)

			item.AppendChild(qty);
			item.AppendChild(priceNode);
			item.AppendChild(desc1);
			item.AppendChild(desc2);
			item.AppendChild(itemNode);
			item.AppendChild(notesNode);
            item.AppendChild(supplierNode); //IP - 09/02/10 - CR1048 (Ref:3.1.2) Merged - Malaysia Enhancements (CR1072)

			if(items==STL.Common.Printing.AgreementPrinting.Document.ItemsPerPage)
			{
				XmlNode pb = _doc.CreateElement("PB");
				item.AppendChild(pb);
				items = 0;
			}
			node.AppendChild(item);
		}
	}
}
