using System;
using STL.Common.Printing.AgreementPrinting;
using System.Data;
using System.Xml;
using STL.Common.Constants.ColumnNames;


namespace STL.Common
{
	/// <summary>
	/// Contains class to set XSLT path ADDCSSPath and other methods to add nodes to Actionsheet
	/// </summary>
    public class ActionSheetXML : PrintXMLClient
	{
		public ActionSheetXML(string country)
		{
			this.XmlTemplate = XMLTemplates.ActionSheetXML;
			this.CountryName = country;
			this.SetXsltPath("ActionSheet.xslt");
		}

        public ActionSheetXML(string country, string DPlaces)
        {
            this.XmlTemplate = XMLTemplates.DeliveryNoteXML;
            this.CountryName = country;
            this.SetXsltPath("ActionSheet.xslt");

           // DecimalPlaces = DPlaces; TODO Check these issues 

            //int number;
           /* if (int.TryParse(DPlaces.Substring(1, 1), out number))
                DecimalPlacesNo = number;
            else
                DecimalPlacesNo = 2;*/
        }

        /// <summary>
        /// Add css path for styles.css
        /// </summary>
        public void AddCSSPath()
        {
            XmlAttribute attribute;

            XmlNode node = _doc.SelectSingleNode("ACTIONSHEETS");
            if (node != null)
            {
                attribute = node.OwnerDocument.CreateAttribute("CSSPATH");
                attribute.Value = System.Windows.Forms.Application.StartupPath + @"\Stylesheets\" + "styles.css";
                node.Attributes.Append(attribute);
            }

        }

		public void AddLineItem(DataRow r, decimal orderVal)
		{
			XmlNode node = this.GetNode("ACTIONSHEET/LINEITEMS");
			XmlNode item = _doc.CreateElement("LINEITEM");
			XmlNode qty = _doc.CreateElement("QUANTITY");
			XmlNode price = _doc.CreateElement("PRICE");
			XmlNode desc1 = _doc.CreateElement("DESC1");
			XmlNode desc2 = _doc.CreateElement("DESC2");			

			qty.InnerText = Convert.ToDecimal(r[CN.Quantity]).ToString("F2");
			price.InnerText = orderVal.ToString("C2");

            //IP - 22/09/11 - RI - #8229 - CR8201
            if (Convert.ToString(Country[CountryParameterNames.RIInterfaceOptions]) != "FACT")
            {
                desc1.InnerText = r[CN.ItemDescr1].ToString() + " " + r[CN.Brand].ToString() + " " + r[CN.Style].ToString();
            }
            else
            {
                desc1.InnerText = r[CN.ItemDescr1].ToString();
            }

			
			desc2.InnerText = r[CN.ItemDescr2].ToString();			

			item.AppendChild(qty);
			item.AppendChild(price);
			item.AppendChild(desc1);
			item.AppendChild(desc2);
			node.AppendChild(item);
		}
        public void AddServiceRequest(DataRow r)
        {
            XmlNode node = this.GetNode("ACTIONSHEET/SERVICEREQUESTS");
            XmlNode item =                  _doc.CreateElement("SERVICEREQUEST");
            XmlNode srNo =                  _doc.CreateElement("SERVICEREQUESTNO");
            XmlNode DateLogged =            _doc.CreateElement("DATELOGGED");
            XmlNode ProductCode =           _doc.CreateElement("PRODUCTCODE");
            XmlNode ProductDescription =    _doc.CreateElement("PRODUCTDESCRIPTION");
            XmlNode DateClosed =            _doc.CreateElement("DATECLOSED"); 
            XmlNode ReplacementStatus  =    _doc.CreateElement("REPLACEMENTSTATUS");
            XmlNode DelivererName =         _doc.CreateElement("DELIVERERNAME");

            srNo.InnerText                  = r[CN.ServiceRequestNoStr].ToString();
            DateLogged.InnerText            = Convert.ToDateTime(r[CN.DateLogged]).ToShortDateString();
            ProductCode.InnerText           = r[CN.ProductCode].ToString();
            ProductDescription.InnerText    = r[CN.Description].ToString();
            DateClosed.InnerText            = Convert.ToDateTime(r[CN.DateClosed]).ToShortDateString();
            ReplacementStatus.InnerText     = r[CN.ReplacementStatus].ToString();
            DelivererName.InnerText         = r[CN.Deliverer].ToString();			
            
            item.AppendChild(srNo);
            item.AppendChild(DateLogged);
            item.AppendChild(ProductCode);
            item.AppendChild(ProductDescription);
            item.AppendChild(DateClosed);
            item.AppendChild(ReplacementStatus);
            item.AppendChild(DelivererName);
            node.AppendChild(item);

        }
		
        public void AddOtherCustomers(DataRow r, string CustName)
		{
			XmlNode node = this.GetNode("ACTIONSHEET/CUSTOMERS");
			XmlNode cust = _doc.CreateElement("CUSTOMER");
			XmlNode name = _doc.CreateElement("NAME");
			XmlNode addr1 = _doc.CreateElement("ADDR1");
			XmlNode addr2 = _doc.CreateElement("ADDR2");
			XmlNode addr3 = _doc.CreateElement("ADDR3");
			XmlNode pCode = _doc.CreateElement("POSTCODE");			
			XmlNode telNo = _doc.CreateElement("TELNO");
			XmlNode workNo = _doc.CreateElement("WORKNO");			
			XmlNode mobileNo = _doc.CreateElement("MOBILENO");			
			XmlNode comment = _doc.CreateElement("COMMENT");			
			XmlNode directions = _doc.CreateElement("DIRECTIONS");			

			name.InnerText = CustName;
			addr1.InnerText = (string)r[CN.Address1];
			addr2.InnerText = (string)r[CN.Address2];
			addr3.InnerText = (string)r[CN.Address3];
			pCode.InnerText = (string)r[CN.PostCode];			

			if(((string)r[CN.TelNoHome]).Length > 0)
				telNo.InnerText = "H : " + (string)r[CN.TelNoHome];
			else
				telNo.InnerText = "";

			if(((string)r[CN.TelNoWork]).Length > 0)
				workNo.InnerText = "W : " + (string)r[CN.TelNoWork];
			else
				workNo.InnerText = "";

			if(((string)r[CN.MobileNo]).Length > 0)
				mobileNo.InnerText = "M : " + (string)r[CN.MobileNo];
			else
				mobileNo.InnerText = "";
			
			comment.InnerText = (string)r[CN.Comment];
			directions.InnerText = (string)r[CN.Directions];

			cust.AppendChild(name);
			cust.AppendChild(addr1);
			cust.AppendChild(addr2);
			cust.AppendChild(addr3);
			cust.AppendChild(pCode);
			cust.AppendChild(telNo);
			cust.AppendChild(workNo);
			cust.AppendChild(mobileNo);
			cust.AppendChild(comment);
			cust.AppendChild(directions);
			node.AppendChild(cust);
		}
        /// <summary>
        /// Creates action sheet and calls PrintXML.load - which justs creates the XML file from a string
        /// </summary>
		public ActionSheetXML CreateActionSheet(string country)
		{
			ActionSheetXML dn = new ActionSheetXML(country);
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
