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
    /// <summary>
    /// Summary description for TaxInvoiceXML.
    /// </summary>
    public class TaxInvoiceXML : PrintXML
    {
        public string CountryCode = "";
        public string TaxType = "";
        public string AgreementTaxType = "";
        public string LineItemTemplate = "";
        public string PayMethodTemplate = "";
        private int totalrows;
        private DataRow[] SortedRows;
        
        public decimal CalculateTaxAmount(XmlNode item, decimal taxRate, ref decimal unitPrice, ref decimal orderValue)
        {
            decimal taxamt = 0;

            //order values held tax exclusive
            if (AgreementTaxType == "E")
            {
                if (item.Attributes[Tags.Type].Value != IT.Discount)
                    taxamt = Math.Round(((orderValue * taxRate) / 100), 2);
                else
                    taxamt = Convert.ToDecimal(item.Attributes[Tags.TaxAmount].Value);

                orderValue += taxamt;
                unitPrice += Math.Round(((unitPrice * taxRate) / 100), 2);
            }

            //order values held tax inclusive
            if (AgreementTaxType == "I")
            {
                taxamt = Math.Round(((orderValue * taxRate) / (100 + taxRate)), 2);
            }

            return taxamt;
        }       

        public TaxInvoiceXML(string taxType, string agreementTaxType, string country, bool IsProofofPurchase)
        {
            this.TaxType = taxType;
            this.AgreementTaxType = agreementTaxType;
            this.XmlTemplate = XMLTemplates.TaxInvoiceXML;
            this.LineItemTemplate = XMLTemplates.TaxInvoiceItemXML;
            this.PayMethodTemplate = XMLTemplates.TaxInvoicePayMethodXML;//BOC Added by Suvidha - CR 2018-13 - 21/12/18 - to print the Invoice.
            this.CountryName = country;
            if (IsProofofPurchase)
            {
                this.SetXsltPath("ProofofPurchase.xslt");
            }
            else
            {
                this.SetXsltPath("TaxInvoice.xslt");
            }
        }

        public class LineOrder : IComparable
        {
            public string SortOrder;
            public string itemNo;
            public string parentitemno;
            public int order;
            public string ContractNo;
            public string StockLocation;
        
            public int CompareTo(object x)
            {
                LineOrder LO = (LineOrder)x;
                return string.Compare(SortOrder + itemNo, LO.SortOrder + LO.itemNo);
            }

            public bool find(string ItemNo, string ContractNo, string StockLocation)
            {
                if (this.itemNo.CompareTo(ItemNo) == 0 &
                    this.ContractNo.CompareTo(ContractNo) == 0 &
                    this.StockLocation.CompareTo(StockLocation) == 0)
                    return true;
                else
                    return false;
            }
        }
        /// <summary>
        /// Adds item to line order array used for sorting. This is because the stylsheets will sort this later so need this to be able to put in page break in document. 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="counter"></param>
        /// <param name="LO"></param>
        private void DetermineSortOrder(XmlNode item, ref int counter, ref LineOrder[] LO,
                    string parentsortorder, string parentitemno)
        {

            string sortorder = string.Empty;

            bool found = false;
            for (int i = 0; i < counter; i++)
            {
                if (LO[i] != null)
                {
                    found = LO[i].find(item.Attributes[Tags.Code].Value, item.Attributes[Tags.ContractNumber].Value,
                        item.Attributes[Tags.Location].Value);
                    if (found)
                        break;
                }

            }

            if (!found & item.Attributes[Tags.Quantity].Value != "0" & item.Attributes[Tags.Value].Value != "0")
            {


                LO[counter] = new LineOrder();
                LO[counter].itemNo = item.Attributes[Tags.Code].Value;
                LO[counter].ContractNo = item.Attributes[Tags.ContractNumber].Value;
                LO[counter].StockLocation = item.Attributes[Tags.Location].Value;
                //we are going to sort initally by parent item number... 
                //if ( parentitemno == string.Empty);
                //   parentitemno = item.Attributes[Tags.Code].Value;

                LO[counter].parentitemno = parentitemno;
                // using sort to determine if page break required... 

                if ((item.Attributes[Tags.Code].Value == "DT") ||
                                    (item.Attributes[Tags.Code].Value == "SD"))
                    sortorder = "K";
                else if (item.Attributes[Tags.Code].Value == "DF")
                    sortorder = "J";
                else if (item.Attributes[Tags.Type].Value == IT.Unknown)
                    sortorder = "I";
                else if (item.Attributes[Tags.Type].Value == IT.SundryCharge)
                    sortorder = "H";
                else if (item.Attributes[Tags.Type].Value == IT.Affinity)
                    sortorder = "G";
                else if (item.Attributes[Tags.Type].Value == IT.KitDiscount)
                    sortorder = "F";
                else if (item.Attributes[Tags.Type].Value == IT.Discount)
                    sortorder = "E";
                else if (item.Attributes[Tags.Type].Value == IT.KitWarranty)
                    sortorder = "D";
                else if (item.Attributes[Tags.Type].Value == IT.Warranty
                    || item.Attributes[Tags.Type].Value == IT.Component)
                    sortorder = "C";
                else if (item.Attributes[Tags.Type].Value == IT.Kit)
                    sortorder = "B";
                else
                    sortorder = "A";




            }

            //here we are widening sort order so that child elements are sorted with their parents...
            XmlNode related = item.SelectSingleNode(Elements.RelatedItem);
            if (LO != null & found == false & item.Attributes[Tags.Quantity].Value != "0" & item.Attributes[Tags.Value].Value != "0")
            {
                if (parentitemno != String.Empty) //so child item
                {
                    LO[counter].SortOrder = parentsortorder + parentitemno + sortorder + item.Attributes[Tags.Code].Value; // put parent sort first

                }
                else
                    LO[counter].SortOrder = sortorder + item.Attributes[Tags.Code].Value; // use own sort.

                counter++;
            }



            if (parentitemno == string.Empty)
                parentitemno = item.Attributes[Tags.Code].Value;
            foreach (XmlNode child in related.ChildNodes)
            {

                if (!(item.Attributes[Tags.Type].Value == IT.Kit && child.Attributes[Tags.Code].Value == "DS") || Convert.ToBoolean(Country[CountryParameterNames.TaxInvKitDisc]))
                {
                    DetermineSortOrder(child, ref  counter, ref LO, sortorder, parentitemno);
                }
            }
            this.totalrows = counter;

        }

        /// <summary>
        /// Creates and sorts SortedRows array so that page breaks can be put in.... 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private int SortLine(LineOrder[] x)
        {
            // assumes stringdata[row, col] is your 2D string array
            DataTable dt = new DataTable();
            DataColumn DSortOrder = new DataColumn();
            DSortOrder.DataType = Type.GetType("System.Int32");
            DSortOrder.ColumnName = "SortOrder";
            dt.Columns.Add(DSortOrder);

            DataColumn DitemNo = new DataColumn();
            DitemNo.DataType = Type.GetType("System.String");
            DitemNo.ColumnName = "ItemNo";
            dt.Columns.Add(DitemNo);

            DataColumn AlphaSortOrder = new DataColumn();
            AlphaSortOrder.DataType = Type.GetType("System.String");
            AlphaSortOrder.ColumnName = "AlphaSortOrder";
            dt.Columns.Add(AlphaSortOrder);

            DataColumn Location = new DataColumn();
            Location.DataType = Type.GetType("System.String");
            Location.ColumnName = "Location";
            dt.Columns.Add(Location);

            DataColumn ContractNo = new DataColumn();
            ContractNo.DataType = Type.GetType("System.String");
            ContractNo.ColumnName = "ContractNo";
            dt.Columns.Add(ContractNo);

            for (int col = 0; col < x.GetLength(0); col++)
            {
                if (x[col] != null)
                {
                    DataRow row = dt.NewRow();
                    row["Itemno"] = x[col].itemNo;
                    row["AlphaSortOrder"] = x[col].SortOrder;//+ x[col].parentitemno;
                    row["SortOrder"] = x[col].order;
                    row["ContractNo"] = x[col].ContractNo;
                    row["Location"] = x[col].StockLocation;

                    dt.Rows.Add(row);
                }
            }

            //  DataRow[] sortedrows = dt.Select("", "3");
            // sort by column name, descending:
            SortedRows = dt.Select("", "AlphaSortOrder, Location, ContractNo ASC");

            int counter = 0;
            for (int i = 0; i < SortedRows.Length; i++)
            {
                counter++;
                SortedRows[i]["SortOrder"] = counter;
            }

            return 0;
        }
        
        public TaxInvoiceXML(string taxType, string agreementTaxType, string country)
        {
            this.TaxType = taxType;
            this.AgreementTaxType = agreementTaxType;
            this.XmlTemplate = XMLTemplates.TaxInvoiceXML;
            this.LineItemTemplate = XMLTemplates.TaxInvoiceItemXML;
            this.CountryName = country;
            this.SetXsltPath("TaxInvoice.xslt");
        }

        public void AddFooter(NumberFormatInfo localFormat)
        {
            decimal extotal = 0;
            decimal inctotal = 0;
            decimal taxtotal = 0;

            //string xpath = "//LINEITEM[TYPE!='KitDiscount' and TYPE!='Component' and TYPE!='KitWarranty']";  //Redmine #3861
            string xpath = "//LINEITEM[TYPE!='KitDiscount' and TYPE!='Component']";
            XmlNodeList items = _doc.SelectNodes(xpath);

            foreach (XmlNode i in items)
            {
                //string taxValue = (i["TAXAMOUNTFOOTER"].InnerText);
                //taxValue = taxValue.Replace(localFormat.CurrencySymbol, "");

                if (!(Convert.ToBoolean(Country[CountryParameterNames.LoyaltyScheme]) && i["ITEMNO"].InnerText.ToString() == LoyaltyDropStatic.VoucherCode))
                {
                    extotal += (Convert.ToDecimal(i["ORDERVALUEFULL"].InnerText) - Convert.ToDecimal(i["TAXAMOUNTFOOTER"].InnerText));
                    inctotal += Convert.ToDecimal(i["ORDERVALUEFULL"].InnerText);
                    taxtotal += Convert.ToDecimal(i["TAXAMOUNTFULL"].InnerText);
                }
            }

            _doc.SelectSingleNode("//FOOTER/EXTOTAL").InnerText = localFormat.CurrencySymbol + extotal.ToString("N", localFormat);
            _doc.SelectSingleNode("//FOOTER/TAXTOTAL").InnerText = localFormat.CurrencySymbol + taxtotal.ToString("N", localFormat);
            _doc.SelectSingleNode("//FOOTER/INCTOTAL").InnerText = localFormat.CurrencySymbol + inctotal.ToString("N", localFormat);           
        }
        //CR-2018-13  To Print with Version Number
        bool ReprintInvoice = false;
        public void AddLineItemshVersion(string termsType, XmlNode items, bool collection,
            bool taxExempt, NumberFormatInfo localFormat, string country, bool reprintInvoice, ref decimal noItems)
        {
            ReprintInvoice = reprintInvoice;

            AddLineItems(termsType, items, collection, taxExempt, localFormat, country, ref noItems);
        }
        public void AddLineItems(string termsType, XmlNode items, bool collection,
            bool taxExempt, NumberFormatInfo localFormat, string country, ref decimal noItems)
        {
            XmlNode addTo = GetNode("TAXINVOICE/LINEITEMS");
            int number = 1;

            if (items != null)
            {
                XmlNodeList allLineItems = null;
                if (ReprintInvoice == true)
                {
                    if (CountryName != "I")
                        allLineItems = items.SelectNodes("//Item[@Code != 'STAX']");
                    else
                        allLineItems = items.SelectNodes("//Item[@Code != 'STAX' and @Code != 'DT']");
                }
                else
                {

                    // CR892
                    if (CountryName != "I")
                        allLineItems = items.SelectNodes("//Item[@Code != 'STAX' and (@Value != '0' or (@FreeGift = 'true' and @Value != '0')) ]");
                    else
                        allLineItems = items.SelectNodes("//Item[@Code != 'STAX' and @Code != 'DT' and (@Value != '0' or (@FreeGift = 'true' and @Value != '0')) ]");
                }

                //LineOrder[] LO = new LineOrder[items.ChildNodes.Count * 10];
                LineOrder[] LO = new LineOrder[allLineItems.Count];
                //totalrows = items.ChildNodes.Count * 10; // just a large enough number to make sure count all existing items
                totalrows = allLineItems.Count; // just a large enough number to make sure count all existing items
                int counter = 0;
                foreach (XmlNode item in items.ChildNodes)
                { // add the alphanumeric sor to the LO array
                    if(item.Attributes[Tags.Code].Value.ToString() != "STAX")
                    {
                        DetermineSortOrder(item, ref counter, ref LO, string.Empty, string.Empty);
                    }
                }

                this.SortLine(LO);// create sortedRows array from Datatable and add order...

                foreach (XmlNode item in items.ChildNodes)
                    AddLineItem(termsType, addTo, item, collection, taxExempt, localFormat, country,
                         allLineItems.Count, ref noItems, ref number);
            }
        }

        private void AddLineItem(string termsType, XmlNode addTo, XmlNode item, bool collection, bool taxExempt, 
                                    NumberFormatInfo localFormat, string country, int numLineItems, ref decimal noItems, ref int number)
        {
            bool furnitureCategory = false;

            decimal tax = 0;
            decimal taxRate = 0;
            decimal unitPrice = 0;
            decimal quantity = 0;
            decimal orderValue = 0;
            XmlUtilities xml = new XmlUtilities();

            if (Convert.ToInt32(item.Attributes[Tags.ItemId].Value) != StockItemCache.Get(StockItemKeys.STAX) &&
                (
                    Convert.ToDecimal(item.Attributes[Tags.Value].Value) != 0 ||
                    (Convert.ToBoolean(item.Attributes[Tags.FreeGift].Value) && Convert.ToDecimal(item.Attributes[Tags.Quantity].Value) != 0)
                )
                &&
                (Convert.ToInt32(item.Attributes[Tags.ItemId].Value) != StockItemCache.Get(StockItemKeys.DT) || CountryName != "I")
               )
            {
                if (item.Attributes[Tags.Type].Value == IT.Kit)
                    xml.RecalculateKitPrice(item, true);

                quantity = Convert.ToDecimal(item.Attributes[Tags.Quantity].Value);
                taxRate = Convert.ToDecimal(item.Attributes[Tags.TaxRate].Value);

                if (ReprintInvoice==true)
                {
                    if (addTo.LocalName == "RELATED")
                    {
                        unitPrice = Convert.ToDecimal(item.Attributes[Tags.CashPrice].Value);
                    }
                    else
                    {
                        unitPrice = Convert.ToDecimal(item.Attributes[Tags.UnitPrice].Value);
                    }
                }
                else
                {
                    unitPrice = Convert.ToDecimal(item.Attributes[Tags.UnitPrice].Value);
                }
                               
                furnitureCategory = (item.Attributes[Tags.ProductCategory].Value == CAT.ProductCatFurniture);

                orderValue = unitPrice * quantity;

                if ((bool)Country[CountryParameterNames.LoyaltyScheme] && item.Attributes[Tags.Code].Value == LoyaltyDropStatic.VoucherCode)
                    orderValue = Convert.ToDecimal(item.Attributes[Tags.Value].Value);
                else
                    orderValue = item.Attributes[Tags.Code].Value == "DEL" ? Convert.ToDecimal(item.Attributes[Tags.Value].Value) * quantity : unitPrice * quantity;

                //bool test; = false;
                //if (!taxExempt)
                //    tax = CalculateTaxAmount(item, taxRate, ref unitPrice, ref orderValue);
                if (!taxExempt)
                {
                    if (Convert.ToBoolean(item.Attributes[Tags.IsAmortized].Value))
                    {
                        if ((bool)(Country[CountryParameterNames.CL_TaxRateApplied]))
                        {
                            tax = orderValue - ((orderValue * 100) / (Convert.ToDecimal(taxRate) + 100));
                        }
                        else
                        {   //this will add the tax amount on the admin charge for invoice which should not happen as admin charge already includes tax
                            tax = 0;
                        }
                    }
                    else
                    {
                        tax = CalculateTaxAmount(item, taxRate, ref unitPrice, ref orderValue);
                    }
                }
                    


                XmlDocument i = new XmlDocument();
                i.LoadXml(LineItemTemplate);
                XmlNode itemNode = i.DocumentElement;
                itemNode = _doc.ImportNode(itemNode, true);
                int iCount = itemNode.ChildNodes.Count; /* if you are importing here then you are inputting at the top? */
                foreach (XmlNode child in itemNode.ChildNodes)
                {
                    switch (child.Name)
                    {
                        case "TYPE": child.InnerText = item.Attributes[Tags.Type].Value;
                            break;
                        case "ITEMNO": child.InnerText = item.Attributes[Tags.Code].Value;
                            break;
                        case "ITEMID": child.InnerText = item.Attributes[Tags.ItemId].Value;
                            break;
                        case "QUANTITY": child.InnerText = collection ? (-quantity).ToString() : quantity.ToString();
                            break;
                        case "ORDERVALUE": child.InnerText = collection ? localFormat.CurrencySymbol + (-orderValue).ToString("N", localFormat) : localFormat.CurrencySymbol + orderValue.ToString("N", localFormat);
                            break;
                        case "UNITPRICE": child.InnerText = collection ? localFormat.CurrencySymbol + (-unitPrice).ToString("N", localFormat) : localFormat.CurrencySymbol + unitPrice.ToString("N", localFormat);
                            break;
                        case "DESC": if (furnitureCategory && country == "S")
                            {
                                child.InnerText = "";
                            }
                            else
                            {
                                //IP - 23/09/11 - RI - #8227 - CR8201
                                if (Convert.ToString(Country[CountryParameterNames.RIInterfaceOptions]) != "FACT")
                                {
                                    child.InnerText = item.Attributes[Tags.Description1].Value + " " + item.Attributes[Tags.Brand].Value + " " + item.Attributes[Tags.Style].Value;
                                }
                                else
                                {
                                    child.InnerText = item.Attributes[Tags.Description1].Value;
                                }
                            }
                            break;
                        case "DESC2": child.InnerText = item.Attributes[Tags.Description2].Value;
                            break;
                        case "TAXRATE": child.InnerText = item.Attributes[Tags.TaxRate].Value;
                            break;
                        case "TAXAMOUNT": child.InnerText = collection ? localFormat.CurrencySymbol + (-tax).ToString("N", localFormat) : localFormat.CurrencySymbol + tax.ToString("N", localFormat);
                            break;
                        case "TAXAMOUNTFOOTER": child.InnerText = collection ? (-tax).ToString("N") : tax.ToString("N");
                            break;
                        case "INDEX": if (Convert.ToInt32(item.Attributes[Tags.ItemId].Value) == StockItemCache.Get(StockItemKeys.DT) ||
                                          Convert.ToInt32(item.Attributes[Tags.ItemId].Value) == StockItemCache.Get(StockItemKeys.SD))
                                child.InnerText = "K";
                            else if (item.Attributes[Tags.Code].Value == "DF")
                                child.InnerText = "J";
                            else if (item.Attributes[Tags.Type].Value == IT.Unknown)
                                child.InnerText = "I";
                            else if (item.Attributes[Tags.Type].Value == IT.SundryCharge)
                                child.InnerText = "H";
                            else if (item.Attributes[Tags.Type].Value == IT.Affinity)
                                child.InnerText = "G";
                            else if (item.Attributes[Tags.Type].Value == IT.KitDiscount)
                                child.InnerText = "F";
                            else if (item.Attributes[Tags.Type].Value == IT.Discount)
                                child.InnerText = "E";
                            else if (item.Attributes[Tags.Type].Value == IT.KitWarranty)
                                child.InnerText = "D";
                            else if (item.Attributes[Tags.Type].Value == IT.Warranty || item.Attributes[Tags.Type].Value == IT.Component)
                                child.InnerText = "C";
                            else if (item.Attributes[Tags.Type].Value == IT.Kit)
                                child.InnerText = "B";
                            else
                                child.InnerText = "A";
                            break;
                        case "NUMBER": child.InnerText = number.ToString();
                            number++;
                            break;
                        case "TERMSTYPE": child.InnerText = termsType;
                            break;
                        case "CONTRACTNO": child.InnerText = item.Attributes[Tags.ContractNumber].Value.Trim(); //IP - 09/02/10 - CR1048 (Ref:3.1.1) Merged - Malaysia Enhancements (CR1072)
                            break;
                        case "NOTES": child.InnerText = item.Attributes[Tags.ColourTrim].Value.Trim(); //CR1048  //IP - 09/02/10 - CR1048 (Ref:3.1.1) Merged - Malaysia Enhancements (CR1072)
                            break;
                        case "ORDERVALUEEXTAX": child.InnerText = collection ? localFormat.CurrencySymbol + (-orderValue + tax).ToString("N", localFormat) : localFormat.CurrencySymbol + (orderValue - tax).ToString("N", localFormat);
                            break;
                        case "ORDERVALUEFULL": child.InnerText = collection ? (-orderValue).ToString() : orderValue.ToString();
                            break;
                        case "TAXAMOUNTFULL": child.InnerText = collection ? (-tax).ToString() : tax.ToString();
                            break;
                        case "TRIM": child.InnerText = item.Attributes[Tags.ColourTrim].Value;
                            break;
                        case "SERIALNOS": 
                            foreach (XmlNode servicechild in item.SelectNodes("SERVICE"))
                            {
                                child.AppendChild(child.OwnerDocument.ImportNode(servicechild, true));
                            }
                            break;
                        default:
                            break;
                    }
                }
                addTo.AppendChild(itemNode);

                // CR892 - do not print tax invoice summary on a seperate page if the number of items on the 
                // account is divisible by the number of items per page (X).  the tax invoice prints X items per page and, will print items on 
                // a second page if there are more than X item to print.
                for (int counter = 0; counter < SortedRows.Length; counter++)
                {

                    if (item.Attributes[Tags.Code].Value.CompareTo(SortedRows[counter]["itemno"]) == 0 &
                        item.Attributes[Tags.ContractNumber].Value.CompareTo(SortedRows[counter]["ContractNo"]) == 0 &
                        item.Attributes[Tags.Location].Value.CompareTo(SortedRows[counter]["Location"]) == 0)
                    {   // modulus for number of items per page. 
                        if ((counter + 1) % Convert.ToInt32(Country[CountryParameterNames.TaxInvoiceItemsPerPage]) == 0 && counter > 0
                            && counter + 1 != this.totalrows) //don't add pagebreak for last row
                        {
                            XmlNode pb = _doc.CreateElement("PB"); //add pagebreak
                            itemNode.AppendChild(pb);
                        }
                    }
                }

                noItems++;

                //recurse
                XmlNode related = item.SelectSingleNode(Elements.RelatedItem);

                //IP - 19/02/10 - CR1072 - LW 69807 - Printing Fixes from 4.3 - Merge - Do not display Kit Discount for Malaysia
                //foreach(XmlNode child in related.ChildNodes)
                //    AddLineItem(termsType, itemNode.SelectSingleNode("RELATED"), child, collection, taxExempt, localFormat, 
                //                country, numLineItems, ref items, ref number);	

                foreach (XmlNode child in related.ChildNodes)
                {
                    //If Country Parameter true, all items added.
                    if (!(item.Attributes[Tags.Type].Value == IT.Kit && child.Attributes[Tags.Code].Value == "DS") || Convert.ToBoolean(Country[CountryParameterNames.TaxInvKitDisc]))
                    {

                        AddLineItem(termsType, itemNode.SelectSingleNode("RELATED"), child, 
                                    collection, taxExempt, localFormat,
                                    country, numLineItems, ref noItems, ref number);
                    }
                }
            }
        }

        //BOC Added by Suvidha - CR 2018-13 - 21/12/18 - to print the Invoice.
        public Decimal AddPayMethods(DataTable dtPayMethods, NumberFormatInfo localFormat, string country, bool ReprintInvoice = false, int agrmtno = 0)
        {
            XmlNode addTo = GetNode("TAXINVOICE/PAYMETHODS");
            Decimal total_amt = 0;
            foreach (DataRow dr in dtPayMethods.Rows)
            {
                string descr = Convert.ToString(dr["payMethod"]);
                Decimal amt = 0;
                if (ReprintInvoice == true && agrmtno != 1)//Web case should display -ve amount for returns case
                {
                    amt = Convert.ToDecimal(Convert.ToDecimal(dr["amount"]));
                }
                else
                {
                    amt = Convert.ToDecimal(Math.Abs(Convert.ToDecimal(dr["amount"])));
                }
                total_amt += amt;
                AddPayMethod(addTo, descr, amt, localFormat, country);
            }            
            return total_amt;
        }

        //BOC Added by Suvidha - CR 2018-13 - 21/12/18 - to print the Invoice.
        private void AddPayMethod(XmlNode addTo, string description, Decimal amount, NumberFormatInfo localFormat, string country)
        {
            XmlUtilities xml = new XmlUtilities();

            XmlDocument i = new XmlDocument();
            i.LoadXml(PayMethodTemplate);
            XmlNode itemNode = i.DocumentElement;
            itemNode = _doc.ImportNode(itemNode, true);
            int iCount = itemNode.ChildNodes.Count; /* if you are importing here then you are inputting at the top? */
            foreach (XmlNode child in itemNode.ChildNodes)
            {
                switch (child.Name)
                {
                    case "DESCRIPTION":
                        child.InnerText = description;
                        break;
                    case "AMOUNT":
                        child.InnerText = localFormat.CurrencySymbol +  " " + amount.ToString("N", localFormat);
                        break;                    
                    default:
                        break;
                }
            }
            addTo.AppendChild(itemNode);

        }
            public void CreateCopies(int copies)
        {
            /* duplicate the TAXINVOICE node x times and append it 
             * to a TAXINVOICES node */
            XmlDocument x = new XmlDocument();
            x.LoadXml("<TAXINVOICES/>");
            for (int i = 0; i < copies; i++)
            {
                string last = i == copies - 1 ? "TRUE" : "FALSE";
                SetNode("TAXINVOICE/LAST", last);
                XmlNode duplicate = _doc.DocumentElement;
                duplicate = x.ImportNode(duplicate, true);
                x.DocumentElement.AppendChild(duplicate);
            }
            _doc = x;
        }
    }
}



