using System;
using STL.Common.Printing.AgreementPrinting;
using System.Data;
using System.Xml;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.Tags;
using STL.Common.Constants.Elements;
using STL.Common.Constants.ItemTypes;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.StoreInfo;
using System.Collections;
using System.Collections.Generic;

namespace STL.Common
{
	/// <summary>
	/// Summary description for RFSummaryXML.
	/// </summary>
	public class RFSummaryXML : PrintXML
	{
        public RFSummaryXML(string country, string storeType, bool? ashleyStore, bool? luckyDollarStore, bool? RadioShackStore, bool? OmniStore)
		{
			this.XmlTemplate = XMLTemplates.RFSummaryXML;
			this.CountryName = country;

            if ((storeType == StoreType.NonCourts && luckyDollarStore.HasValue) && luckyDollarStore.Value == true)
            {
                this.SetXsltPath("RFSummary_LD.xslt");
            }
            else if ((storeType == StoreType.NonCourts && ashleyStore.HasValue) && ashleyStore.Value == true)
            {
                this.SetXsltPath("RFSummary_Ashley.xslt");
            }
            else if ((storeType == StoreType.NonCourts && RadioShackStore.HasValue) && RadioShackStore.Value == true)
            {
                this.SetXsltPath("RFSummary_Radioshack.xslt");
            }
            else if ((storeType == StoreType.NonCourts && OmniStore.HasValue) && OmniStore.Value == true)
            {
                this.SetXsltPath("RFSummary_Omni.xslt");
            }
            else 
            {
                this.SetXsltPath("RFSummary.xslt");
            }
		}

		public void AddAccount(string accountNo, DataSet agreement, XmlNode lineItems, ref int noAccounts)
		{
			decimal instalAmount = 0;
			decimal instalNo = 0;
			string date;
			string dateLast = "__ /__ /";
			
			DateTime tmpDate = DateTime.MinValue.AddYears(1909);

			XmlDocument x = new XmlDocument();
			x.LoadXml(XMLTemplates.RFAccountXML);
			XmlNode acct = x.DocumentElement;
			acct = _doc.ImportNode(acct, true);

			if(noAccounts++ > STL.Common.Printing.AgreementPrinting.Document.MaxAccts)
			{
				acct.AppendChild(_doc.CreateElement("PB"));
				noAccounts = 0;
			}

			foreach (DataTable dt in agreement.Tables)
			{
				if(dt.TableName == TN.Agreements )
					foreach(DataRow row in dt.Rows)
					{
						instalAmount = Math.Round(Convert.ToDecimal(row["Instalment Amount"]),2);
						instalNo = Convert.ToDecimal(row["instalno"]);
						
						if(row["Date Last Instalment"] != DBNull.Value)
							if(Convert.ToDateTime(row["Date Last Instalment"]) > tmpDate)
								dateLast = ((DateTime)row["Date Last Instalment"]).ToShortDateString();
						
						date = ((DateTime)row["Agreement Date"]).ToShortDateString();
						(acct.SelectSingleNode("ACCTNO")).InnerText = accountNo;
						(acct.SelectSingleNode("DATEOPEN")).InnerText = date;
						(acct.SelectSingleNode("MONTHLYINSTALMENT")).InnerText = instalAmount.ToString();
						(acct.SelectSingleNode("NOOFINSTALMENTS")).InnerText = instalNo.ToString();;
						(acct.SelectSingleNode("DATEOFFINALINSTAL")).InnerText = dateLast;
					}
			}			
			XmlNode items = acct.SelectSingleNode("ITEMS");
			AddLineItem(lineItems, items);

			XmlNode accounts = _doc.SelectSingleNode("RFSUMMARY/ACCTDETAILS/ACCOUNTS");
			accounts.AppendChild(acct);
		}
	
		private void AddLineItem(XmlNode relatedItems, XmlNode items)
		{
			if(relatedItems!=null)
			{
				foreach(XmlNode item in relatedItems.ChildNodes)
				{
					if(item.NodeType == XmlNodeType.Element &&
						item.Name == Elements.Item)
					{
						if( Convert.ToDecimal(item.Attributes[Tags.Quantity].Value) != 0 &&
							Convert.ToInt32(item.Attributes[Tags.ItemId].Value) != StockItemCache.Get(StockItemKeys.STAX) &&
							Convert.ToInt32(item.Attributes[Tags.ItemId].Value) != StockItemCache.Get(StockItemKeys.DT) &&
							Convert.ToInt32(item.Attributes[Tags.ItemId].Value) != StockItemCache.Get(StockItemKeys.SD) &&
							item.Attributes[Tags.Type].Value != IT.Component &&
							item.Attributes[Tags.Type].Value != IT.KitDiscount)
						{
							XmlNode n = _doc.CreateElement("ITEM");
							n.AppendChild(_doc.CreateElement("DESCRIPTION"));
							n.FirstChild.InnerText = item.Attributes[Tags.Description1].Value + "\n";
							items.AppendChild(n);
							XmlNode r = item.SelectSingleNode(Elements.RelatedItem);
							AddLineItem(r, items);
						}
					}
				}
			}
		}
	}
}
