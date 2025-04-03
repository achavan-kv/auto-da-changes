using System;
using System.Xml;

namespace STL.BLL.CreditBureau
{
	/// <summary>
	/// Summary description for HistoricalReportRequest.
	/// </summary>
	public class HistoricalReportRequest : CreditBureauMessage, ICreditBureauRequest
	{
		public HistoricalReportRequest()
		{
		}

		public XmlDocument Serialise()
		{
			XmlDocument request = new XmlDocument();

			return request;
		}
	}
}
