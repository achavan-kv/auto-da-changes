using System;
using System.Xml;

namespace STL.BLL.CreditBureau
{
	/// <summary>
	/// Summary description for LoadDefaultRequest.
	/// </summary>
	public class LoadDefaultRequest : CreditBureauMessage, ICreditBureauRequest
	{
		public LoadDefaultRequest()
		{
		}

		public XmlDocument Serialise()
		{
			XmlDocument request = new XmlDocument();

			return request;
		}
	}
}
