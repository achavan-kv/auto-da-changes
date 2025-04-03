using System;
using STL.Common.Printing.AgreementPrinting;
using System.Data;
using System.Xml;
using STL.Common.Constants.ColumnNames;

namespace STL.Common
{
	/// <summary>
	/// Summary description for RebateReport.
	/// </summary>
	public class RebateReportXML : PrintXML
	{
		public RebateReportXML(string country)
		{
			this.XmlTemplate = XMLTemplates.RebateForecastRequestXML;
			this.CountryName = country;
			this.SetXsltPath("RebateReport.xslt");
		}

		public XmlElement DocumentElement
		{
			get{return _doc.DocumentElement;}
		}

		public RebateReportXML CreateRebateReport(string country)
		{
			RebateReportXML rr = new RebateReportXML(country);
			rr.Load();
			return rr;
		}

		public void ImportNode(XmlNode toImport, bool deep)
		{
			toImport = _doc.ImportNode(toImport, deep);
			_doc.DocumentElement.AppendChild(toImport);
		}

		public void AddPeriod(XmlNode rebateNode)
		{
			XmlNode node = this.GetNode("REBATEFORECAST/PERIODS");

			foreach(XmlNode item in rebateNode.SelectSingleNode("PERIODS").ChildNodes)
			{
				XmlNode period = _doc.CreateElement("PERIOD");
				XmlNode level = _doc.CreateElement("LEVEL");
				XmlNode P1 = _doc.CreateElement("P1");
				XmlNode P2 = _doc.CreateElement("P2");
				XmlNode P3 = _doc.CreateElement("P3");
				XmlNode P4 = _doc.CreateElement("P4");
				XmlNode P5 = _doc.CreateElement("P5");
				XmlNode P6 = _doc.CreateElement("P6");
				XmlNode P7 = _doc.CreateElement("P7");
				XmlNode P8 = _doc.CreateElement("P8");
				XmlNode P9 = _doc.CreateElement("P9");
				XmlNode P10 = _doc.CreateElement("P10");
				XmlNode P11 = _doc.CreateElement("P11");
				XmlNode P12 = _doc.CreateElement("P12");
	
				level.InnerText = item.SelectSingleNode("LEVEL").InnerText; 
				P1.InnerText = Convert.ToDecimal(item.SelectSingleNode("P1").InnerText).ToString("C2");
				P2.InnerText = Convert.ToDecimal(item.SelectSingleNode("P2").InnerText).ToString("C2");
				P3.InnerText = Convert.ToDecimal(item.SelectSingleNode("P3").InnerText).ToString("C2");
				P4.InnerText = Convert.ToDecimal(item.SelectSingleNode("P4").InnerText).ToString("C2");
				P5.InnerText = Convert.ToDecimal(item.SelectSingleNode("P5").InnerText).ToString("C2");
				P6.InnerText = Convert.ToDecimal(item.SelectSingleNode("P6").InnerText).ToString("C2");
				P7.InnerText = Convert.ToDecimal(item.SelectSingleNode("P7").InnerText).ToString("C2");
				P8.InnerText = Convert.ToDecimal(item.SelectSingleNode("P8").InnerText).ToString("C2");
				P9.InnerText = Convert.ToDecimal(item.SelectSingleNode("P9").InnerText).ToString("C2");
				P10.InnerText = Convert.ToDecimal(item.SelectSingleNode("P10").InnerText).ToString("C2");
				P11.InnerText = Convert.ToDecimal(item.SelectSingleNode("P11").InnerText).ToString("C2");
				P12.InnerText = Convert.ToDecimal(item.SelectSingleNode("P12").InnerText).ToString("C2");

				period.AppendChild(level);
				period.AppendChild(P1);
				period.AppendChild(P2);
				period.AppendChild(P3);
				period.AppendChild(P4);
				period.AppendChild(P5);
				period.AppendChild(P6);
				period.AppendChild(P7);
				period.AppendChild(P8);
				period.AppendChild(P9);
				period.AppendChild(P10);
				period.AppendChild(P11);
				period.AppendChild(P12);

				node.AppendChild(period);
			}
		}

	}
}
