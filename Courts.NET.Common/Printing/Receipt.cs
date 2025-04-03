using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Printing;
using System.Drawing;
using BBSL.Libraries.General;
using System.Data;
using STL.Common.Constants.ColumnNames;
using STL.Common;
using System.Xml;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.TableNames;

namespace BBSL.Libraries.Printing.PrintDocuments
{
    /// <summary>
    /// 
    /// </summary>
    public class Receipt : PrintContent
    {
        #region Title Text

        static string titleTaxInvoice = "";
        static string titleSalesPerson = "";
        static string titleInvoiceNo = "";
        static string titleMoreRewardsNo = "";
        static string titleAmount = "";
        static string titleTotalAmount = "";
        static string titleTax = "";
        /// <summary>
        /// TODO - ASH
        /// </summary>
        static string titleLuxTax = "";
        static string titleSubTotal = "";
        static string titleInvoiceTotal = "";
        static string titlePayMethod = "";
        static string titleAccountNo = "";
        static string titleAccountBalance = "";
        static string titleAvailableSpend = "";
        static string titleAmountTendered = "";
        static string titleChangeGiven = "";
        static string titleCashierID = "";
        static string titleDeferedTermsAmount = "";
        static string titleBranchCode = "";
        static string titleSignatureText = "";
        static string titleCustomerName = "";
        static string titleCustomerAddress = "";
        static string titleDatePrinted = "";                        //IP - 08/05/12 - #9608 - CR8520
        static string titleReprintDate = "";                        //IP - 08/05/12 - #9608 - CR8520
        static string titleInvoiceDate = "";//CR 2018-13

        public static string TitleTaxInvoice { get { return titleTaxInvoice; } set { titleTaxInvoice = value; } }
        public static string TitleSalesPerson { get { return titleSalesPerson; } set { titleSalesPerson = value; } }
        public static string TitleInvoiceNo { get { return titleInvoiceNo; } set { titleInvoiceNo = value; } }
        public static string TitleMoreRewardsNo { get { return titleMoreRewardsNo; } set { titleMoreRewardsNo = value; } }
        public static string TitleAmount { get { return titleAmount; } set { titleAmount = value; } }
        public static string TitleTotalAmount { get { return titleTotalAmount; } set { titleTotalAmount = value; } }
        public static string TitleTax { get { return titleTax; } set { titleTax = value; } }
        /// <summary>
        /// TODO - ASH
        /// </summary>
        public static string TitleLuxTax { get { return titleLuxTax; } set { titleLuxTax = value; } }

        /// <summary>
        /// TODO - ASH
        /// </summary>
        public static string LuxTax { get { return LuxTax; } set { LuxTax = value; } }

        public static string TitleSubTotal { get { return titleSubTotal; } set { titleSubTotal = value; } }
        public static string TitleInvoiceTotal { get { return titleInvoiceTotal; } set { titleInvoiceTotal = value; } }
        public static string TitlePayMethod { get { return titlePayMethod; } set { titlePayMethod = value; } }
        public static string TitleAccountNo { get { return titleAccountNo; } set { titleAccountNo = value; } }
        public static string TitleAccountBalance { get { return titleAccountBalance; } set { titleAccountBalance = value; } }
        public static string TitleAvailableSpend { get { return titleAvailableSpend; } set { titleAvailableSpend = value; } }
        public static string TitleAmountTendered { get { return titleAmountTendered; } set { titleAmountTendered = value; } }
        public static string TitleChangeGiven { get { return titleChangeGiven; } set { titleChangeGiven = value; } }
        public static string TitleCashierID { get { return titleCashierID; } set { titleCashierID = value; } }
        public static string TitleDeferredTermsAmount { get { return titleDeferedTermsAmount; } set { titleDeferedTermsAmount = value; } }
        public static string TitleBranchCode { get { return titleBranchCode; } set { titleBranchCode = value; } }
        public static string TitleSignatureText { get { return titleSignatureText; } set { titleSignatureText = value; } }
        public static string TitleCustomerName { get { return titleCustomerName; } set { titleCustomerName = value; } }
        public static string TitleCustomerAddress { get { return titleCustomerAddress; } set { titleCustomerAddress = value; } }
        public static string TitleDatePrinted { get { return titleDatePrinted; } set { titleDatePrinted = value; } }                                    //IP - 08/05/12 - #9608 - CR8520
        public static string TitleReprintDate { get { return titleReprintDate; } set { titleReprintDate = value; } }                                    //IP - 08/05/12 - #9608 - CR8520
        public static string TitleInvoiceDate { get { return titleInvoiceDate; } set { titleInvoiceDate = value; } }//CR 2018-13

        #endregion

        #region Constructors

        public Receipt()
            : base()
        {
            this.AccountBalance = new PrintDataWrapper<decimal?>(null, true);
            this.AccountNo = new PrintDataWrapper<string>(default(string), true);
            this.AmountTendered = new PrintDataWrapper<decimal?>(null, true);
            this.AvailableSpend = new PrintDataWrapper<decimal?>(null, true);
            this.ChangeGiven = new PrintDataWrapper<decimal?>(null, true);
            this.Date = new PrintDataWrapper<DateTime>(default(DateTime), true);
            this.DeferredTermsAmount = new PrintDataWrapper<decimal?>(null, true);
            this.InvoiceNo = new PrintDataWrapper<string>(default(string), true);                      
            this.MoreRewardsNo = new PrintDataWrapper<string>(default(string), true);
            this.PayMethod = new PrintDataWrapper<string>(default(string), true);
            this.Products = new PrintDataWrapper<List<Product>>(default(List<Product>), true);
            this.SalesPerson = new PrintDataWrapper<string>(default(string), true);
            this.CashierID = new PrintDataWrapper<string>(default(string), true);
            this.SubTotal = new PrintDataWrapper<decimal?>(null, true);
            this.TaxInvoice = new PrintDataWrapper<string>(default(string), true);
            this.Total = new PrintDataWrapper<decimal?>(null, true);
            this.AdditionalTotalTax = new PrintDataWrapper<decimal?>(null, true);
            //CR20181018
            this.OtherCurrencyTotal = new PrintDataWrapper<decimal?>(null, true);
            this.OtherCurrencyName = new PrintDataWrapper<string>(null, true);
            this.isOtherCorruncyActive = new PrintDataWrapper<bool>(false, true);
            //CR20181018 - end 
            this.TotalTax = new PrintDataWrapper<decimal?>(null, true);
            this.VATIDNo = new PrintDataWrapper<string>(default(string), true);
            this.BranchCode = new PrintDataWrapper<string>(default(string), true);
            this.Footer = new PrintDataWrapper<string>(default(string), true);
            this.Customer = new PrintDataWrapper<BBSCustomer>(default(BBSCustomer), true);
            this.ReprintDate = new PrintDataWrapper<DateTime>(default(DateTime), true);                 //IP - 08/05/12 -  #9608 - CR8520
            this.InvoiceDate = new PrintDataWrapper<string>(default(string), true);//CR 2018-13
            this.CurrencySymbol = new PrintDataWrapper<string>(default(string), true);//CR 2018-13
            this.CountryCode = new PrintDataWrapper<string>(default(string), true);//CR 2018-13
        }

        #endregion

        #region Public Propeties

        public override string PrintString
        {
            get { throw new NotImplementedException(); }
        }

        public PrintDataWrapper<string> Footer { get; set; }

        public PrintDataWrapper<decimal?> AccountBalance { get; set; }

        public PrintDataWrapper<string> AccountNo { get; set; }

        public PrintDataWrapper<decimal?> AmountTendered { get; set; }

        public PrintDataWrapper<decimal?> AvailableSpend { get; set; }

        public PrintDataWrapper<string> BranchCode { get; set; }

        public PrintDataWrapper<string> CashierID { get; set; }

        public PrintDataWrapper<decimal?> ChangeGiven { get; set; }

        public PrintDataWrapper<DateTime> Date { get; set; }

        public PrintDataWrapper<decimal?> DeferredTermsAmount { get; set; }

        public PrintDataWrapper<string> InvoiceNo { get; set; }                         

        public PrintDataWrapper<string> MoreRewardsNo { get; set; }

        public PrintDataWrapper<string> PayMethod { get; set; }
        public DataSet payMethodSet { get; set; }
        public PrintDataWrapper<List<Product>> Products { get; set; }

        //public   PrintDataWrapper<List<PayMethod>> PayMethods { get; set; }
         
        public PrintDataWrapper<string> SalesPerson { get; set; }

        public PrintDataWrapper<decimal?> SubTotal { get; set; }

        public PrintDataWrapper<string> TaxInvoice { get; set; }

        public PrintDataWrapper<decimal?> Total { get; set; }
        //CR201801014-TaxInvoice Amount displayed in Exchange Rate
        public PrintDataWrapper<decimal?> OtherCurrencyTotal { get; set; }
        public PrintDataWrapper<string> OtherCurrencyName { get; set; }
        public PrintDataWrapper<bool> isOtherCorruncyActive { get; set; }
        //CR201801014 - End

        public PrintDataWrapper<decimal?> TotalTax { get; set; }

        public PrintDataWrapper<string> VATIDNo { get; set; }

        public PrintDataWrapper<BBSCustomer> Customer { get; set; }

        public bool IncludeSignatureStrip { get; set; }

        public bool Reprint { get; set; }                                               //IP - 09/05/12 - #9608 - CR8520

        public PrintDataWrapper<DateTime> ReprintDate { get; set; }                     //IP - 08/05/12 - #9608 - CR8520
        public PrintDataWrapper<decimal?> AdditionalTotalTax { get; set; }
        public PrintDataWrapper<string> InvoiceDate { get; set; }       //CR 2018-13
        public PrintDataWrapper<string> CurrencySymbol { get; set; }       //CR 2018-13
        public PrintDataWrapper<string> CountryCode { get; set; }       //CR 2018-13


        #endregion

        #region Print Event Handlers

        bool printheader = true;
        bool printfooter = true;
        int productindex = 0;

        public override void PrintContent_PrintPage(object sender, PrintPageEventArgs e)
        {         
            Font bodyFont = BodyFont != null ? BodyFont : defaultBodyFont;
            Font header1Font = Header1Font != null ? Header1Font : defaultHeader1Font;
            Font header2Font = Header1Font != null ? Header2Font : defaultHeader2Font;
            Font header3Font = Header3Font != null ? Header3Font : defaultHeader3Font;

            e.Graphics.PageUnit = GraphicsUnit.Point;


            float paperWidthInPoints = GetPrintableAreaWidth(e);

            float x = 0,
                  lineOffset = bodyFont.GetHeight(e.Graphics) - 0.5F,
                  y = lineOffset;
            if (printheader == true)
            {
                DrawHeader(e, paperWidthInPoints, ref y, header1Font, bodyFont);            
            
            {//***** TOP SECTION ******

                string[] titles = 
                {
                    TitleTaxInvoice, 
                    TitleSalesPerson, 
                    TitleInvoiceNo, 
                    TitleMoreRewardsNo, 
                    TitleAccountNo, 
                    TitleAccountNo,
                    TitleAccountBalance, 
                    TitleAvailableSpend,
                    TitleDeferredTermsAmount,
                    TitleBranchCode,
                    TitleCustomerName,
                    TitleCustomerAddress,
                    TitleDatePrinted,                                   //IP - 08/05/12 - #9608 - CR8520
                    TitleReprintDate                                    //IP - 08/05/12 - #9608 - CR8520
                    , TitleInvoiceDate
                };

                float titleWidth = e.Graphics.MeasureString(titles[0], bodyFont).Width;
                float topSectionContentX = titleWidth;
                float topSectionContentOffset = titleWidth * 0.15F;


                foreach (string title in titles)
                {
                    titleWidth = e.Graphics.MeasureString(title, bodyFont).Width;
                    if (titleWidth > topSectionContentX)
                    {
                        topSectionContentX = titleWidth;
                        topSectionContentOffset = titleWidth * 0.25F;
                    }
                }
                topSectionContentX = topSectionContentX + topSectionContentOffset;
                float pintableAreaWidthInPoints = GetPrintableAreaWidth(e);
                var remainingSpace = pintableAreaWidthInPoints - topSectionContentX;

                if (BranchCode.ShouldBePrinted && !String.IsNullOrEmpty(BranchCode.Value))
                {
                    e.Graphics.DrawString(TitleBranchCode, bodyFont, Brushes.Black, x, y);
                    e.Graphics.DrawString(BranchCode.Value, bodyFont, Brushes.Black, topSectionContentX, y);

                    NewLine(lineOffset, ref y);
                }
                if (TaxInvoice.ShouldBePrinted && !String.IsNullOrEmpty(TaxInvoice.Value))
                {
                    e.Graphics.DrawString(TitleTaxInvoice, bodyFont, Brushes.Black, x, y);
                    e.Graphics.DrawString(TaxInvoice.Value, bodyFont, Brushes.Black, topSectionContentX, y);

                    NewLine(lineOffset, ref y);
                }
                //BOC CR 2018-13
                if (InvoiceDate.Value != null)
                {
                    e.Graphics.DrawString(TitleInvoiceDate, bodyFont, Brushes.Black, x, y);
                    e.Graphics.DrawString(InvoiceDate.Value, bodyFont, Brushes.Black, topSectionContentX, y);

                    NewLine(lineOffset, ref y);
                }
                //EOC CR 2018-13
                if (SalesPerson.ShouldBePrinted && !String.IsNullOrEmpty(SalesPerson.Value))
                {
                    e.Graphics.DrawString(TitleSalesPerson, bodyFont, Brushes.Black, x, y);
                    //e.Graphics.DrawString(SalesPerson.Value, bodyFont, Brushes.Black, topSectionContentX, y);
                    DrawAndWrapString(SalesPerson.Value, e.Graphics, bodyFont, remainingSpace, lineOffset, topSectionContentX, ref y);          //IP - 18/05/12 - #10143

                    NewLine(lineOffset, ref y);
                }
                if (CashierID.ShouldBePrinted && !String.IsNullOrEmpty(CashierID.Value))
                {
                    e.Graphics.DrawString(TitleCashierID, bodyFont, Brushes.Black, x, y);
                    //e.Graphics.DrawString(CashierID.Value, bodyFont, Brushes.Black, topSectionContentX, y);
                    DrawAndWrapString(CashierID.Value, e.Graphics, bodyFont, remainingSpace, lineOffset, topSectionContentX, ref y);           //IP - 18/05/12 - #10143

                    NewLine(lineOffset, ref y);
                }
                if (Customer.ShouldBePrinted && Customer.Value != null && !Customer.Value.IsEmpty)
                {
                    NewLine(lineOffset, ref y);

                    e.Graphics.DrawString(TitleCustomerName, bodyFont, Brushes.Black, x, y);
                    DrawAndWrapString(Customer.Value.Name, e.Graphics, bodyFont, remainingSpace, lineOffset, topSectionContentX, ref y);

                    bool[] addressShouldBePrinted = 
                    {
                        !String.IsNullOrEmpty(Customer.Value.Address1),
                        !String.IsNullOrEmpty(Customer.Value.Address2),
                        !String.IsNullOrEmpty(Customer.Value.Address3),
                        !String.IsNullOrEmpty(Customer.Value.Address4),
                        !String.IsNullOrEmpty(Customer.Value.Address5)
                    };

                    if (addressShouldBePrinted.OneIsTrue())
                        e.Graphics.DrawString(TitleCustomerAddress, bodyFont, Brushes.Black, x, y);

                    if (addressShouldBePrinted[0])
                        DrawAndWrapString(Customer.Value.Address1, e.Graphics, bodyFont, remainingSpace, lineOffset, topSectionContentX, ref y);
                    if (addressShouldBePrinted[1])
                        DrawAndWrapString(Customer.Value.Address2, e.Graphics, bodyFont, remainingSpace, lineOffset, topSectionContentX, ref y);
                    if (addressShouldBePrinted[2])
                        DrawAndWrapString(Customer.Value.Address3, e.Graphics, bodyFont, remainingSpace, lineOffset, topSectionContentX, ref y);
                    if (addressShouldBePrinted[3])
                        DrawAndWrapString(Customer.Value.Address4, e.Graphics, bodyFont, remainingSpace, lineOffset, topSectionContentX, ref y);
                    if (addressShouldBePrinted[4])
                        DrawAndWrapString(Customer.Value.Address5, e.Graphics, bodyFont, remainingSpace, lineOffset, topSectionContentX, ref y);
                    NewLine(lineOffset, ref y);
                }
                //if (InvoiceNo.ShouldBePrinted && !String.IsNullOrEmpty(InvoiceNo.Value))                                              //IP - 09/05/12 - #9610 - CR8520 - not required to be printed
                //{
                //    e.Graphics.DrawString(TitleInvoiceNo, bodyFont, Brushes.Black, x, y);
                //    e.Graphics.DrawString(InvoiceNo.Value, bodyFont, Brushes.Black, topSectionContentX, y);

                //    NewLine(lineOffset, ref y);
                //}
                if (MoreRewardsNo.ShouldBePrinted && !String.IsNullOrEmpty(MoreRewardsNo.Value))
                {
                    e.Graphics.DrawString(TitleMoreRewardsNo, bodyFont, Brushes.Black, x, y);
                    e.Graphics.DrawString(MoreRewardsNo.Value, bodyFont, Brushes.Black, topSectionContentX, y);

                    NewLine(lineOffset, ref y);
                }
                if (AccountNo.ShouldBePrinted && !String.IsNullOrEmpty(AccountNo.Value))
                {
                    e.Graphics.DrawString(TitleAccountNo, bodyFont, Brushes.Black, x, y);
                    e.Graphics.DrawString(AccountNo.Value, bodyFont, Brushes.Black, topSectionContentX, y);

                    NewLine(lineOffset, ref y);
                }
                if (AccountBalance.ShouldBePrinted && AccountBalance.Value.HasValue)
                {
                    e.Graphics.DrawString(TitleAccountBalance, bodyFont, Brushes.Black, x, y);
                    e.Graphics.DrawString(MoneyString(AccountBalance.Value), bodyFont, Brushes.Black, topSectionContentX, y);

                    NewLine(lineOffset, ref y);
                }
                if (AvailableSpend.ShouldBePrinted && AvailableSpend.Value.HasValue)
                {
                    e.Graphics.DrawString(TitleAvailableSpend, bodyFont, Brushes.Black, x, y);
                    e.Graphics.DrawString(MoneyString(AvailableSpend.Value), bodyFont, Brushes.Black, topSectionContentX, y);

                    NewLine(lineOffset, ref y);
                }

                //IP - 09/05/12 - #9608 - CR8520 - Moved time to top section
                if (Date.ShouldBePrinted && Date.Value != null)
                {
                    string dateString = "";

                    switch (Receipt.DateTimeFormat)
                    {
                        case DateTimeFormats.LongDate:
                            dateString = Date.Value.ToLongDateString(); break;
                        case DateTimeFormats.LongTime:
                            dateString = Date.Value.ToLongTimeString(); break;
                        case DateTimeFormats.LongDateAndTime:
                            dateString = Date.Value.ToLongDateString() + " " + Date.Value.ToLongTimeString(); break;
                        case DateTimeFormats.LongDateAndShortTime:
                            dateString = Date.Value.ToLongDateString() + " " + Date.Value.ToShortTimeString(); break;
                        case DateTimeFormats.ShortDate:
                            dateString = Date.Value.ToShortDateString(); break;
                        case DateTimeFormats.ShortTime:
                            dateString = Date.Value.ToShortTimeString(); break;
                        case DateTimeFormats.ShortDateAndTime:
                            dateString = Date.Value.ToShortDateString() + " " + Date.Value.ToShortTimeString(); break;
                        case DateTimeFormats.ShortDateAndLongTime:
                            dateString = Date.Value.ToShortDateString() + " " + Date.Value.ToLongTimeString(); break;
                    }

                    NewLine(lineOffset, ref y);
                   
                    e.Graphics.DrawString(TitleDatePrinted, bodyFont, Brushes.Black, x, y);
                    e.Graphics.DrawString(dateString, bodyFont, Brushes.Black, topSectionContentX, y);

                    //NewLine(lineOffset * lineSpace2, ref y);
                }

                //  IP - 09/05/12 - #9608 - CR8520
                if (Reprint && ReprintDate.ShouldBePrinted && ReprintDate.Value != null)
                {
                    string dateString = "";

                    switch (Receipt.DateTimeFormat)
                    {
                        case DateTimeFormats.LongDate:
                            dateString = ReprintDate.Value.ToLongDateString(); break;
                        case DateTimeFormats.LongTime:
                            dateString = ReprintDate.Value.ToLongTimeString(); break;
                        case DateTimeFormats.LongDateAndTime:
                            dateString = ReprintDate.Value.ToLongDateString() + " " + ReprintDate.Value.ToLongTimeString(); break;
                        case DateTimeFormats.LongDateAndShortTime:
                            dateString = ReprintDate.Value.ToLongDateString() + " " + ReprintDate.Value.ToShortTimeString(); break;
                        case DateTimeFormats.ShortDate:
                            dateString = ReprintDate.Value.ToShortDateString(); break;
                        case DateTimeFormats.ShortTime:
                            dateString = ReprintDate.Value.ToShortTimeString(); break;
                        case DateTimeFormats.ShortDateAndTime:
                            dateString = ReprintDate.Value.ToShortDateString() + " " + ReprintDate.Value.ToShortTimeString(); break;
                        case DateTimeFormats.ShortDateAndLongTime:
                            dateString = ReprintDate.Value.ToShortDateString() + " " + ReprintDate.Value.ToLongTimeString(); break;
                    }

                    NewLine(lineOffset, ref y);

                    e.Graphics.DrawString(TitleReprintDate, bodyFont, Brushes.Black, x, y);
                    e.Graphics.DrawString(dateString, bodyFont, Brushes.Black, topSectionContentX, y);

                        NewLine(lineOffset, ref y);
                        NewLine(lineOffset, ref y);
                }
                NewLine(lineOffset, ref y);
                NewLine(lineOffset, ref y);
                NewLine(lineOffset, ref y);

                }//*** END TOP SECTION ****
                printheader = false;
            }

            //{//***** TIME SECTION ******

            //    if (Date.ShouldBePrinted && Date.Value != null)
            //    {
            //        string dateString = "";

            //        switch (Receipt.DateTimeFormat)
            //        {
            //            case DateTimeFormats.LongDate:
            //                dateString = Date.Value.ToLongDateString(); break;
            //            case DateTimeFormats.LongTime:
            //                dateString = Date.Value.ToLongTimeString(); break;
            //            case DateTimeFormats.LongDateAndTime:
            //                dateString = Date.Value.ToLongDateString() + " " + Date.Value.ToLongTimeString(); break;
            //            case DateTimeFormats.LongDateAndShortTime:
            //                dateString = Date.Value.ToLongDateString() + " " + Date.Value.ToShortTimeString(); break;
            //            case DateTimeFormats.ShortDate:
            //                dateString = Date.Value.ToShortDateString(); break;
            //            case DateTimeFormats.ShortTime:
            //                dateString = Date.Value.ToShortTimeString(); break;
            //            case DateTimeFormats.ShortDateAndTime:
            //                dateString = Date.Value.ToShortDateString() + " " + Date.Value.ToShortTimeString(); break;
            //            case DateTimeFormats.ShortDateAndLongTime:
            //                dateString = Date.Value.ToShortDateString() + " " + Date.Value.ToLongTimeString(); break;
            //        }

            //        NewLine(lineOffset, ref y);
            //        e.Graphics.DrawString(dateString, bodyFont, Brushes.Black, x, y);
            //        NewLine(lineOffset * lineSpace2, ref y);
            //    }

            //    //  IP - 08/05/12 - #9608 - CR8520
            //    if (ReprintDate.ShouldBePrinted && ReprintDate.Value != null)
            //    {
            //        string dateString = "";

            //        switch (Receipt.DateTimeFormat)
            //        {
            //            case DateTimeFormats.LongDate:
            //                dateString = ReprintDate.Value.ToLongDateString(); break;
            //            case DateTimeFormats.LongTime:
            //                dateString = ReprintDate.Value.ToLongTimeString(); break;
            //            case DateTimeFormats.LongDateAndTime:
            //                dateString = ReprintDate.Value.ToLongDateString() + " " + ReprintDate.Value.ToLongTimeString(); break;
            //            case DateTimeFormats.LongDateAndShortTime:
            //                dateString = ReprintDate.Value.ToLongDateString() + " " + ReprintDate.Value.ToShortTimeString(); break;
            //            case DateTimeFormats.ShortDate:
            //                dateString = ReprintDate.Value.ToShortDateString(); break;
            //            case DateTimeFormats.ShortTime:
            //                dateString = ReprintDate.Value.ToShortTimeString(); break;
            //            case DateTimeFormats.ShortDateAndTime:
            //                dateString = ReprintDate.Value.ToShortDateString() + " " + ReprintDate.Value.ToShortTimeString(); break;
            //            case DateTimeFormats.ShortDateAndLongTime:
            //                dateString = ReprintDate.Value.ToShortDateString() + " " + ReprintDate.Value.ToLongTimeString(); break;
            //        }

            //        NewLine(lineOffset, ref y);
            //        e.Graphics.DrawString(dateString, bodyFont, Brushes.Black, x, y);
            //        NewLine(lineOffset * lineSpace2, ref y);
            //    }

            //}//*** END TIME SECTION ****

            NewLine(lineOffset * lineSpace2, ref y);

            float lineStringX = GetRightXCoordinate(e.Graphics, paperWidthInPoints, lineString, bodyFont);

            {//***** PRODUCTS & TOTAL SECTION ******
                if (Products.ShouldBePrinted && Products.Value != null)
                {
                    float titleTaxWidth = e.Graphics.MeasureString(TitleTax, bodyFont).Width;
                    float titleSubTotalWidth = e.Graphics.MeasureString(TitleSubTotal, bodyFont).Width;
                    float titleTaxX = GetCenterXCoordinate(e.Graphics, paperWidthInPoints, TitleTax, bodyFont);
                    //TODO - Ash
                    float titleLuxTaxWidth = e.Graphics.MeasureString(TitleLuxTax, bodyFont).Width;
                    float titleLuxTaxX = GetCenterXCoordinate(e.Graphics, paperWidthInPoints, TitleLuxTax, bodyFont) + 55;
                    float titleSubTotalX = GetRightXCoordinate(e.Graphics, paperWidthInPoints, TitleSubTotal, bodyFont);
                    
                   

                    int pageCharacterWidth = Convert.ToInt32(e.PageSettings.PaperSize.Width / bodyFont.SizeInPoints);

                   
                    for (int j = productindex; j <= Products.Value.Count; j++)
                    {
                        if (Products.Value.Count == productindex)
                        {
                            printfooter = true;
                            break;
                        }
                        Product product = Products.Value[j];
                        string productDescription = product.Quantity + "  (" + product.ProductCode + ") " + product.Description;

                        for (var i = 0; i * pageCharacterWidth < productDescription.Length; i++)
                        {
                            if (i > 0) //If not first line
                                NewLine(lineOffset, ref y);

                            var desc = productDescription.SafeSubstring(i * pageCharacterWidth, pageCharacterWidth);
                            e.Graphics.DrawString(desc, bodyFont, Brushes.Black, x, y);
                        }

                        NewLine(lineOffset * lineSpace1, ref y);

                        string unitPrice = MoneyString(product.UnitPrice);
                        string taxAmount = MoneyString(product.TaxAmount);
                        string LuxtaxAmount = MoneyString(product.AdditionalTaxAmount);
                        string totalPrice = MoneyString(product.TotalPrice);

                        float taxAmountWidth = e.Graphics.MeasureString(taxAmount, bodyFont).Width;
                        float LuxtaxAmountWidth = e.Graphics.MeasureString(taxAmount, bodyFont).Width;

                        e.Graphics.DrawString(TitleAmount, bodyFont, Brushes.Black, x, y);
                        if (CountryCode.Value.Trim() == "Q")
                        {
                            e.Graphics.DrawString(TitleTax + " (" + product.TaxPercent + "%) ", bodyFont, Brushes.Black, titleTaxX, y);
                            e.Graphics.DrawString(TitleLuxTax + " (" + product.AdditionalTaxPercent + "%) ", bodyFont, Brushes.Black, titleLuxTaxX, y);
                        }
                        else
                        {
                            e.Graphics.DrawString(TitleTax, bodyFont, Brushes.Black, titleTaxX, y);
                        }
                        e.Graphics.DrawString(TitleSubTotal, bodyFont, Brushes.Black, titleSubTotalX, y);

                        

                        NewLine(lineOffset, ref y);

                        e.Graphics.DrawString(unitPrice, bodyFont, Brushes.Black, x, y);

                        x = titleTaxX + titleTaxWidth - taxAmountWidth;
                        e.Graphics.DrawString(taxAmount, bodyFont, Brushes.Black, x, y);

                        if (CountryCode.Value.Trim() == "Q")
                        {
                            x = titleLuxTaxX + titleLuxTaxWidth - LuxtaxAmountWidth;
                            e.Graphics.DrawString(LuxtaxAmount, bodyFont, Brushes.Black, x + 10, y);
                        }

                        x = GetRightXCoordinate(e.Graphics, paperWidthInPoints, totalPrice, bodyFont);
                        e.Graphics.DrawString(totalPrice, bodyFont, Brushes.Black, x, y);

                        x = 0;
                        NewLine(lineOffset * lineSpace2, ref y);                   
                        productindex++;
                        if (j != 0 && j % 10 == 0)
                        {                            
                            e.HasMorePages = true;
                            printfooter = false;
                            break;
                        }
                        else
                        {
                            printfooter = true;
                        }
                        if(Products.Value.Count == 0)
                        {
                            printfooter = true;
                        }
                       
                    }
                    if (DeferredTermsAmount.ShouldBePrinted && DeferredTermsAmount.Value.HasValue)
                    {
                        e.Graphics.DrawString(TitleDeferredTermsAmount, bodyFont, Brushes.Black, x, y);
                        NewLine(lineOffset * lineSpace1, ref y);
                        e.Graphics.DrawString(TitleAmount, bodyFont, Brushes.Black, x, y);
                        e.Graphics.DrawString(TitleSubTotal, bodyFont, Brushes.Black, titleSubTotalX, y);
                        NewLine(lineOffset, ref y);
                        e.Graphics.DrawString(MoneyString(DeferredTermsAmount.Value), bodyFont, Brushes.Black, x, y);
                        x = GetRightXCoordinate(e.Graphics, paperWidthInPoints, MoneyString(DeferredTermsAmount.Value), bodyFont);
                        e.Graphics.DrawString(MoneyString(DeferredTermsAmount.Value), bodyFont, Brushes.Black, x, y);
                        x = 0;
                        NewLine(lineOffset * lineSpace2, ref y);
                    }                
                }

                
            }//*** END PRODUCTS & TOTAL SECTION ****

            if (printfooter == true)
            {
                //***** TOTAL AMOUNT SECTION ******
                if (Total.ShouldBePrinted && Total.Value.HasValue)
                {
                    float titleTaxWidth = e.Graphics.MeasureString(TitleTax, bodyFont).Width;
                    float titleAdditionalTaxWidth = e.Graphics.MeasureString(TitleLuxTax, bodyFont).Width;
                    float titleInvoiceTotalWidth = e.Graphics.MeasureString(TitleInvoiceTotal, bodyFont).Width;
                    float titleTaxX = GetCenterXCoordinate(e.Graphics, paperWidthInPoints, TitleTax, bodyFont) + 50;
                    float titleAdditioalTaxX = GetCenterXCoordinate(e.Graphics, paperWidthInPoints, TitleLuxTax, bodyFont);
                    float titleInvoiceTotalX = GetRightXCoordinate(e.Graphics, paperWidthInPoints, TitleInvoiceTotal, bodyFont);

                    e.Graphics.DrawString(TitleTotalAmount, bodyFont, Brushes.Black, x, y);
                    e.Graphics.DrawString(TitleTax, bodyFont, Brushes.Black, titleTaxX - 50, y);
                    e.Graphics.DrawString(TitleLuxTax, bodyFont, Brushes.Black, titleAdditioalTaxX + 50, y);
                    e.Graphics.DrawString(TitleInvoiceTotal, bodyFont, Brushes.Black, titleInvoiceTotalX, y);

                    NewLine(lineOffset, ref y);

                    string subTotal = CurrencySymbol.Value + " " + MoneyString(SubTotal.Value);
                    string totalTax = CurrencySymbol.Value + " " + MoneyString(TotalTax.Value);
                    string AdditionaltotalTax = CurrencySymbol.Value + " " + MoneyString(AdditionalTotalTax.Value);
                    string total = CurrencySymbol.Value + " " + MoneyString(Total.Value);
                    string OtherCurrencytotal = MoneyString(OtherCurrencyTotal.Value);
                    float totalTaxWidth = e.Graphics.MeasureString(totalTax, bodyFont).Width;
                    float totalAdditionalTaxWidth = e.Graphics.MeasureString(AdditionaltotalTax, bodyFont).Width;

                    e.Graphics.DrawString(subTotal, bodyFont, Brushes.Black, x, y);



                    x = titleTaxX + titleTaxWidth - totalTaxWidth;
                    e.Graphics.DrawString(totalTax, bodyFont, Brushes.Black, x - 50, y);

                    if (CountryCode.Value.Trim() == "Q")
                    {
                        x = titleAdditioalTaxX + titleAdditionalTaxWidth - totalAdditionalTaxWidth;
                        e.Graphics.DrawString(AdditionaltotalTax, bodyFont, Brushes.Black, x + 50, y);
                    }

                    x = GetRightXCoordinate(e.Graphics, paperWidthInPoints, total, bodyFont);
                    e.Graphics.DrawString(total, bodyFont, Brushes.Black, x, y);

                    NewLine(lineOffset * lineStringSpacing, ref y);

                    //CR20181014
                    if (isOtherCorruncyActive.Value == true)
                    {
                        x = GetRightXCoordinate(e.Graphics, paperWidthInPoints, OtherCurrencyName.Value + " " + OtherCurrencytotal, bodyFont);
                        e.Graphics.DrawString(OtherCurrencyName.Value + " " + OtherCurrencytotal, bodyFont, Brushes.Black, x, y);
                        NewLine(lineOffset * lineStringSpacing, ref y);
                        //CR20181014
                    }
                    e.Graphics.DrawString(lineString, bodyFont, Brushes.Black, lineStringX, y);
                    NewLine(lineOffset * doubleLineStringSpacing, ref y);
                    e.Graphics.DrawString(lineString, bodyFont, Brushes.Black, lineStringX, y);

                    x = 0;
                    NewLine(lineOffset, ref y);
                }

                {//***** BOTTOM SECTION ******             
                float titleAmountTenderedWidth = e.Graphics.MeasureString(TitleAmountTendered, bodyFont).Width;
                float titleAmountTenderedX = lineStringX - bottomSectionTitleOffset - titleAmountTenderedWidth;

                bool multipleMethods = false;
                if (PayMethod.ShouldBePrinted) //print all payment methods if more than one....
                {
                    if (payMethodSet.Tables.Count > 0 && payMethodSet.Tables[STL.Common.Constants.TableNames.TN.PayMethodList].Rows.Count > 1)
                    {
                        foreach (DataRow row in payMethodSet.Tables[STL.Common.Constants.TableNames.TN.PayMethodList].Rows)
                        {
                            var   Amount =System.Convert.ToSingle(row[CN.Value]);
                            var Method = System.Convert.ToString(row[CN.CodeDescription]);
                            float AmountGivenWidth = e.Graphics.MeasureString(Amount.ToString(), bodyFont).Width;
                            string amount = MoneyString(Amount);
                            multipleMethods = true;      
                            e.Graphics.DrawString(Method, bodyFont, Brushes.Black, titleAmountTenderedX, y);

                            x = GetRightXCoordinate(e.Graphics, paperWidthInPoints, amount, bodyFont);
                            e.Graphics.DrawString(amount, bodyFont, Brushes.Black, x, y);

                            NewLine(lineOffset, ref y);
                        }
                    }

                }

                if (AmountTendered.ShouldBePrinted && AmountTendered.Value.HasValue)
                {
                    e.Graphics.DrawString(TitleAmountTendered, bodyFont, Brushes.Black, titleAmountTenderedX, y);

                    string amountTendered = MoneyString(AmountTendered.Value);

                    x = GetRightXCoordinate(e.Graphics, paperWidthInPoints, amountTendered, bodyFont);
                    e.Graphics.DrawString(amountTendered, bodyFont, Brushes.Black, x, y);

                    NewLine(lineOffset * lineStringSpacing, ref y);

                    e.Graphics.DrawString(lineString, bodyFont, Brushes.Black, lineStringX, y);

                    x = 0;
                    NewLine(lineOffset, ref y);
                }
                if (ChangeGiven.ShouldBePrinted && ChangeGiven.Value.HasValue)
                {
                    float titleChangeGivenWidth = e.Graphics.MeasureString(TitleChangeGiven, bodyFont).Width;
                    float titleChangeGivenX = titleAmountTenderedX + titleAmountTenderedWidth - titleChangeGivenWidth;

                    e.Graphics.DrawString(TitleChangeGiven, bodyFont, Brushes.Black, titleChangeGivenX, y);

                    string changeGiven = MoneyString(ChangeGiven.Value);

                    x = GetRightXCoordinate(e.Graphics, paperWidthInPoints, changeGiven, bodyFont);
                    e.Graphics.DrawString(changeGiven, bodyFont, Brushes.Black, x, y);

                    NewLine(lineOffset * lineStringSpacing, ref y);

                    e.Graphics.DrawString(lineString, bodyFont, Brushes.Black, lineStringX, y);

                    x = 0;
                    NewLine(lineOffset, ref y);
                }
                if (PayMethod.ShouldBePrinted && !String.IsNullOrEmpty(PayMethod.Value) && !multipleMethods)
                {
                    float titlePayMethodWidth = e.Graphics.MeasureString(TitlePayMethod, bodyFont).Width;
                    float titlePayMethodX = titleAmountTenderedX + titleAmountTenderedWidth - titlePayMethodWidth;

                    e.Graphics.DrawString(TitlePayMethod, bodyFont, Brushes.Black, titlePayMethodX, y);

                    x = GetRightXCoordinate(e.Graphics, paperWidthInPoints, PayMethod.Value, bodyFont); 
                    e.Graphics.DrawString(PayMethod.Value, bodyFont, Brushes.Black, x, y);

                    x = 0;
                    NewLine(lineOffset, ref y);
                }
                


                }//*** END BOTTOM SECTION ****

            
                {//***** SIGNATURE ******

                if (IncludeSignatureStrip)
                {
                    string divider = CreateFullLine(DividerCharacter, e, bodyFont, paperWidthInPoints);
                    e.Graphics.DrawString(divider, bodyFont, Brushes.Black, x, y);
                    
                    for (int i = 0; i < 10; i++)
                    {
                        NewLine(lineOffset, ref y);
                    }

                    x = GetRightXCoordinate(e.Graphics, paperWidthInPoints, TitleSignatureText, bodyFont);
                    e.Graphics.DrawString(TitleSignatureText, bodyFont, Brushes.Black, x, y);
                    x = 0;
                }

                }//*** END SIGNATURE ****

            
                {//***** FOOTER ******
                    NewLine(lineOffset, ref y);
                    float footerX;
                    string divider = CreateFullLine(DividerCharacter, e, bodyFont, paperWidthInPoints);
                    footerX = GetCenterXCoordinate(e.Graphics, paperWidthInPoints, divider, bodyFont);
                    e.Graphics.DrawString(divider, bodyFont, Brushes.Black, x, y);

                    if (Footer.ShouldBePrinted && !String.IsNullOrEmpty(Footer.Value))
                    {
                        NewLine(lineOffset * lineSpace1, ref y);

                        List<string> strings = Split(Footer.Value, e, bodyFont);
                        foreach (string str in strings)
                        {
                            footerX = GetCenterXCoordinate(e.Graphics, paperWidthInPoints, str, header3Font);
                            e.Graphics.DrawString(str, header3Font, Brushes.Black, footerX, y);
                            NewLine(lineOffset, ref y);
                        }
                    }
                }//*** END FOOTER ****
                printfooter = false;
                e.HasMorePages = false;
            }

            while ((y / pointsPerMillimeter) < MinimumHeight)
            {//Ensure Receipt is Minimum Height
                NewLine(lineOffset, ref y);
                
            }
            e.Graphics.DrawString(".", bodyFont, Brushes.Black, 0, y);
        }

        #endregion
    }

    //IP - 11/05/12 - #9609 - CR8520
    public class ReceiptBulk : Receipt
    {
        public ReceiptBulk(DataSet payMethodSet)
        { 
            this.rowCount = payMethodSet.Tables[TN.Accounts].Rows.Count;
        }

        private int i = 0, rowCount;

        public Action<int> BeforePrintPage;

        public override void PrintContent_PrintPage(object sender, PrintPageEventArgs e)
        {
            if (BeforePrintPage != null)
                BeforePrintPage(i);

            base.PrintContent_PrintPage(sender, e);


            e.HasMorePages = (++i < rowCount);
            
         
        }
    }
}
