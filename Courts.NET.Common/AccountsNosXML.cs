using System;
using STL.Common.Printing.AgreementPrinting;
using System.Data;
using System.Xml;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.Tags;
using STL.Common.Constants.Elements;
using STL.Common.Constants.ItemTypes;
using STL.Common.Constants.TableNames;

namespace STL.Common
{
	/// <summary>
	/// Summary description for ContractNosXML.
	/// </summary>
	public class AccountNosXML : PrintXML
	{
		public AccountNosXML(string country)
		{
			this.CountryName = country;
			this.SetXsltPath("AccountNos.xslt");			
		}

		public void AddAccountNos(string[] accounts)
		{
			Load("<ACCOUNTNOS/>");

			foreach(string s in accounts)
			{
				XmlNode n = _doc.CreateElement("ACCOUNTNO");
				n.InnerText = s;
				this._doc.DocumentElement.AppendChild(n);
			}
		}
	}
}
