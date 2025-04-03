using System;
using STL.Common.Printing.AgreementPrinting;
using System.Data;
using System.Xml;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.Tags;
using STL.Common.Constants.Elements;
using STL.Common.Constants.ItemTypes;
using STL.Common.Constants.Categories;
using System.Threading;
using System.Globalization;
using System.Collections;


namespace STL.Common
{
    public class FoodLossXML:PrintXML
    {

        public FoodLossXML(string country)
        {
            this.XmlTemplate = XMLTemplates.FoodLossXML;
            this.CountryName = country;
            
            this.SetXsltPath("FoodLoss.xslt");
            
        }

    }
}
