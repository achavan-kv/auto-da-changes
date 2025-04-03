using System;
using System.Xml;

namespace STL.BLL.CreditBureau
{
	/// <summary>
	/// Summary description for ICreditBureauRequest.
	/// </summary>
	public interface ICreditBureauRequest
	{
		/// <summary>
		/// Formats a subset of inherited properties into an appropriate
		/// ICreditBureauRequest Xml document, each implementation will call
		/// the base class Validate method to validate the document against 
		/// the Bayscorp schema. 
		/// </summary>
		/// <returns>Validated XmlDocument</returns>
		XmlDocument Serialise();
	}
}
