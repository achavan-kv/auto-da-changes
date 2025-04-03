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
	/// Summary description for JournalEnquiryXML.
	/// </summary>
	public class JournalEnquiryXML : PrintXML
	{
		public JournalEnquiryXML(string country)
		{
			this.XmlTemplate = XMLTemplates.JournalEnquiryXML;
			this.CountryName = country;
			this.SetXsltPath("JournalEnquiry.xslt");
		}

		public void AddTransactions(DataSet totals, string numberFormat)
		{
			int transPerPage = 50;
			XmlNode transactions = _doc.DocumentElement.SelectSingleNode("//TRANSACTIONS");
			
			if(transactions!=null)
			{
				XmlNode transaction = transactions.SelectSingleNode("//TRANSACTION");
				transactions.RemoveChild(transaction);

				int trans = 1;
				
				foreach(DataRow r in totals.Tables[0].Rows)
				{
					transaction = transaction.CloneNode(true);
					transaction.SelectSingleNode("BRANCH").InnerText = Convert.ToString(r[CN.BranchNo]);
					transaction.SelectSingleNode("EMPLOYEE").InnerText = Convert.ToString(r[CN.EmployeeNo]);
					transaction.SelectSingleNode("DATETRANS").InnerText = ((DateTime)r[CN.DateTrans]).ToShortDateString();;
					transaction.SelectSingleNode("REFNO").InnerText = Convert.ToString(r[CN.TransRefNo]);
					transaction.SelectSingleNode("ACCTNO").InnerText = Convert.ToString(r[CN.AcctNo]);
					transaction.SelectSingleNode("TRANSTYPE").InnerText = Convert.ToString(r[CN.TransTypeCode]);
					transaction.SelectSingleNode("TRANSVALUE").InnerText = ((decimal)r[CN.TransValue]).ToString();
					transactions.AppendChild(transaction);

					if(trans > transPerPage)
					{
						XmlNode pb = _doc.CreateElement("PB");
						transaction.AppendChild(pb);
						trans = 0;
					}
					trans++;
				}
			}
		}
	}
}
