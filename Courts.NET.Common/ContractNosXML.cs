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
	public class ContractNosXML : PrintXML
	{
		public ContractNosXML(string country)
		{
			this.CountryName = country;
			this.SetXsltPath("ContractNos.xslt");			
		}

		public void AddContractNos(string[] contracts)
		{
			Load("<CONTRACTNOS/>");

			foreach(string s in contracts)
			{
				XmlNode n = _doc.CreateElement("CONTRACTNO");
				n.InnerText = s;
				this._doc.DocumentElement.AppendChild(n);
			}
		}
	}
}
