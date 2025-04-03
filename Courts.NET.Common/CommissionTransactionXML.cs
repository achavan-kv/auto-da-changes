using System;

namespace STL.Common
{
	/// <summary>
	/// Summary description for CommissionTransactionXML.
	/// </summary>
	public class CommissionTransactionXML : PrintXML
	{
		public CommissionTransactionXML(string country)
		{
				this.CountryName = country;
				this.SetXsltPath("CommissionTransactions.xslt");
		}
	}
}
