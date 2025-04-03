using System;
using STL.Common.Printing.AgreementPrinting;
using System.Data;
using System.Xml;
using STL.Common.Constants.ColumnNames;
using System.Configuration;
using STL.Common.Constants.Tags;
using STL.Common.Constants.ItemTypes;
using STL.Common.Constants.Elements;
using STL.Common.Constants.TableNames;

namespace STL.Common
{
	/// <summary>
	/// Summary description for CashierTotalsSummaryXML.
	/// </summary>
	public class CashierTotalsSummaryXML : PrintXML
	{
		public CashierTotalsSummaryXML(string country)
		{
			this.XmlTemplate = XMLTemplates.CashierTotalsSummaryXML;
			this.CountryName = country;
			this.SetXsltPath("CashierTotalsSummary.xslt");
		}

		public void AddPayMethods(DataSet totals,  
									DataSet pm,
									ref decimal systemTotal, 
									ref decimal userTotal,
									ref decimal depositTotal, 
									ref decimal differenceTotal,
									ref decimal securitisedTotal)
		{
			XmlNode payMethods = this.Document.SelectSingleNode("//PAYMETHODS");
			XmlNode payMethod = payMethods.SelectSingleNode("//PAYMETHOD");
			payMethods.RemoveAll();

			foreach(DataRow r in totals.Tables[TN.CashierTotals].Rows)
			{
				decimal systemValue = (decimal)r[CN.SystemTotal];
				decimal userValue = (decimal)r[CN.UserTotal];
				decimal depositValue  = (decimal)r[CN.Deposit];
				decimal difference = (decimal)r[CN.Difference];
				decimal securitisedValue = (decimal)r[CN.SecuritisedValue];

				systemTotal += systemValue;
				userTotal += userValue;
				depositTotal += depositValue;
				differenceTotal += difference;
				securitisedTotal += securitisedValue;

				XmlNode newPayMethod = payMethod.CloneNode(true);
				newPayMethod["NAME"].InnerText = (string)r[CN.CodeDescription];
				newPayMethod["SYSTEMVALUE"].InnerText = systemValue.ToString(DecimalPlaces);
				newPayMethod["USERVALUE"].InnerText = userValue.ToString(DecimalPlaces);
				newPayMethod["DEPOSITVALUE"].InnerText = depositValue.ToString(DecimalPlaces);
				newPayMethod["DIFFERENCEVALUE"].InnerText = difference.ToString(DecimalPlaces);
				newPayMethod["SECURITISEDVALUE"].InnerText = securitisedValue.ToString(DecimalPlaces);
				payMethods.AppendChild(newPayMethod);
			}

			/* need to pad out the XML with nodes for any paymethods we don't 
			 * already have */
			foreach (DataTable dt in pm.Tables)
			{
				foreach(DataRow r in dt.Rows)
				{
					if((string)r[CN.Code]!="0")
					{
						string desc = (string)r[CN.CodeDescription];
						if(payMethods.SelectSingleNode("//PAYMETHOD[NAME='"+desc+"']")==null)
						{
							XmlNode newPayMethod = payMethod.CloneNode(true);
							newPayMethod["NAME"].InnerText = desc;
							newPayMethod["SYSTEMVALUE"].InnerText = (0).ToString(DecimalPlaces);
							newPayMethod["USERVALUE"].InnerText = (0).ToString(DecimalPlaces);
							newPayMethod["DEPOSITVALUE"].InnerText = (0).ToString(DecimalPlaces);
							newPayMethod["DIFFERENCEVALUE"].InnerText = (0).ToString(DecimalPlaces);
							newPayMethod["SECURITISEDVALUE"].InnerText = (0).ToString(DecimalPlaces);
							payMethods.AppendChild(newPayMethod);
						}
					}
				}
			}			
		}
	}
}
