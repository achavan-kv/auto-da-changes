using System;
using System.Xml;
using System.Data;
using System.Data.SqlClient;

namespace STL.BLL.CreditBureau
{
	/// <summary>
	/// Validates the incoming XmlDocument against the appropriate 
	/// credit bureau schema. If it is valid, the required information is
	/// picked out using an xPath query and exposed as properties of the
	/// ICreditBureauResponse instance
	/// </summary>
	/// <returns>ICreditBureauResponse</returns>
	public interface ICreditBureauResponse
	{
		ICreditBureauResponse DeSerialise(SqlConnection conn, SqlTransaction trans, string customerID, XmlDocument response);
	}
}
