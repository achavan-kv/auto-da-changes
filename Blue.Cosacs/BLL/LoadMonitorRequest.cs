using System;
using System.Xml;

namespace STL.BLL.CreditBureau
{
	/// <summary>
	/// Summary description for LoadMonitorRequest.
	/// </summary>
	public class LoadMonitorRequest : CreditBureauMessage, ICreditBureauRequest
	{
		public LoadMonitorRequest()
		{
		}

		public XmlDocument Serialise()
		{
			XmlDocument request = new XmlDocument();

			return request;
		}
	}
}
