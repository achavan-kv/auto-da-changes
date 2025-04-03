using System;
using STL.Common.Printing.AgreementPrinting;
using System.Data;
using System.Xml;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.Tags;
using STL.Common.Constants.Elements;
using STL.Common.Constants.ItemTypes;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.StoreInfo;

namespace STL.Common
{
	/// <summary>
	/// Summary description for RFTermsXML.
	/// </summary>
	public class RFTermsXML : PrintXML
	{
        public RFTermsXML(string country, string storeType, bool? ashleyStore, bool? luckyDollarStore, bool? radioShackStore)             //#19687
		{
			this.XmlTemplate = XMLTemplates.RFTermsXML;
			this.CountryName = country;

            if ((storeType == StoreType.NonCourts && luckyDollarStore.HasValue) && luckyDollarStore.Value == true)                    //#19687
            {
                this.SetXsltPath("RFTerms_LD.xslt");
            }
            else if ((storeType == StoreType.NonCourts && ashleyStore.HasValue) && ashleyStore.Value == true)
            {
                this.SetXsltPath("RFTerms_Ashley.xslt");
            }
            else if ((storeType == StoreType.NonCourts && radioShackStore.HasValue) && radioShackStore.Value == true)
            {
                this.SetXsltPath("RFTerms_Radioshack.xslt");
            }
            else
            {
                this.SetXsltPath("RFTerms.xslt");
            }
		}
	}
}
