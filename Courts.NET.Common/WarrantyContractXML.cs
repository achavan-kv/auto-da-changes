using System;
using STL.Common.Printing.AgreementPrinting;
using System.Data;
using System.Xml;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.Tags;
using STL.Common.Constants.Elements;
using STL.Common.Constants.ItemTypes;

namespace STL.Common
{
	/// <summary>
	/// Summary description for WarrantyContractXML.
	/// </summary>
	public class WarrantyContractXML : PrintXML
	{
		public WarrantyContractXML(string country)
		{
			this.XmlTemplate = XMLTemplates.WarrantyContractXML;
			this.CountryName = country;
			//this.SetXsltPath("WarrantyContract.xslt");
		}

		public void CreateCopies(short creditCopies,
									short custCopies,
									short hoCopies )
		{
			int copies = creditCopies + custCopies + hoCopies;
			int i=0;
			string [] copy = new string[copies];
			
			for(/*i=i*/; i<creditCopies; i++)
				copy[i] = "Credit Department Copy";

			for(/*i=i*/; i<creditCopies+custCopies; i++)
				copy[i] = "Customer Copy";

			for(/*i=i*/; i<copies; i++)
				copy[i] = "Head Office Copy";

			XmlDocument x = new XmlDocument();
			x.LoadXml("<CONTRACTS/>");
			for(i=0; i<copies; i++)
			{
				string last = i==copies-1 ? "TRUE":"FALSE";
				SetNode("CONTRACT/LAST", last);
				SetNode("CONTRACT/COPY", copy[i]);
				XmlNode duplicate = _doc.DocumentElement;
				duplicate = x.ImportNode(duplicate, true);
				x.DocumentElement.AppendChild(duplicate);
			}
			_doc = x;
		}
	}
}
