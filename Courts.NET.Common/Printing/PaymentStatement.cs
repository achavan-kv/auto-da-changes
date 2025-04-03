using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Printing;
using System.Drawing;
using BBSL.Libraries.General;

namespace BBSL.Libraries.Printing.PrintDocuments
{
    public class PaymentStatement : PrintContent
    {
        #region Title Text

        static string titleAvailableSpend = "";
        static string titleCashierID = "";
        static string titleCustomerName = "";
        static string titleCustomerAddress = "";
        static string titleCreditLimit = "";
        static string titleReferenceNo = "";
        static string titleDatePaid = "";
        static string titleAccountNo = "";
        static string titleAgreementTotal = "";
        static string titlePaymentType = "";
        static string titleAmountPaid = "";
        static string titleRemainingBalance = "";
        static string titlePayments = "";
        static string titlePaymentTotal = "";
        static string titleBalanceTotal = "";

        public static string TitleAccountNo { get { return titleAccountNo; } set { titleAccountNo = value; } }
        public static string TitleAvailableSpend { get { return titleAvailableSpend; } set { titleAvailableSpend = value; } }
        public static string TitleCashierID { get { return titleCashierID; } set { titleCashierID = value; } }
        public static string TitleCustomerName { get { return titleCustomerName; } set { titleCustomerName = value; } }
        public static string TitleCustomerAddress { get { return titleCustomerAddress; } set { titleCustomerAddress = value; } }
        public static string TitleCreditLimit { get { return titleCreditLimit; } set { titleCreditLimit = value; } }
        public static string TitleReferenceNo { get { return titleReferenceNo; } set { titleReferenceNo = value; } }
        public static string TitleDatePaid { get { return titleDatePaid; } set { titleDatePaid = value; } }
        public static string TitleAgreementTotal { get { return titleAgreementTotal; } set { titleAgreementTotal = value; } }
        public static string TitlePaymentType { get { return titlePaymentType; } set { titlePaymentType = value; } }
        public static string TitleAmountPaid { get { return titleAmountPaid; } set { titleAmountPaid = value; } }
        public static string TitleRemainingBalance { get { return titleRemainingBalance; } set { titleRemainingBalance = value; } }
        public static string TitlePayments { get { return titlePayments; } set { titlePayments = value; } }
        public static string TitlePaymentTotal { get { return titlePaymentTotal; } set { titlePaymentTotal = value; } }
        public static string TitleBalanceTotal { get { return titleBalanceTotal; } set { titleBalanceTotal = value; } }

        #endregion

        #region Constructors

        public PaymentStatement()
            : base()
        {
            this.AvailableSpend = new PrintDataWrapper<decimal?>(null, true);
            this.Date = new PrintDataWrapper<DateTime>(default(DateTime), true);
            this.Footer = new PrintDataWrapper<string>(default(string), true);
            this.CustomerName = new PrintDataWrapper<string>(default(string), true);
            this.CustomerAddress1 = new PrintDataWrapper<string>(default(string), true);
            this.CustomerAddress2 = new PrintDataWrapper<string>(default(string), true);
            this.CustomerAddress3 = new PrintDataWrapper<string>(default(string), true);
            this.CustomerAddress4 = new PrintDataWrapper<string>(default(string), true);
            this.CustomerAddress5 = new PrintDataWrapper<string>(default(string), true);
            this.CreditLimit = new PrintDataWrapper<decimal?>(default(decimal?), true);
            this.PaymentTotal = new PrintDataWrapper<decimal?>(default(decimal?), true);
            this.BalanceTotal = new PrintDataWrapper<decimal?>(default(decimal?), true);
            this.ReferenceNo = new PrintDataWrapper<string>(default(string), true);
            this.Payments = new PrintDataWrapper<List<Transaction>>(new List<Transaction>(), true);
            this.BusinessTitle = new PrintDataWrapper<string>(default(string), true);
            this.BusinessTaxNo = new PrintDataWrapper<string>(default(string), true);
        }

        #endregion

        #region Public Propeties

        public override string PrintString
        {
            get { throw new NotImplementedException(); }
        }

        public PrintDataWrapper<string> Footer { get; set; }
        public PrintDataWrapper<string> BusinessTitle { get; set; }
        public PrintDataWrapper<string> BusinessTaxNo { get; set; }
        public PrintDataWrapper<string> CustomerName { get; set; }

        public PrintDataWrapper<string> CustomerAddress1 { get; set; }

        public PrintDataWrapper<string> CustomerAddress2 { get; set; }

        public PrintDataWrapper<string> CustomerAddress3 { get; set; }

        public PrintDataWrapper<string> CustomerAddress4 { get; set; }

        public PrintDataWrapper<string> CustomerAddress5 { get; set; }

        public PrintDataWrapper<decimal?> CreditLimit { get; set; }

        public PrintDataWrapper<decimal?> AvailableSpend { get; set; }

        public PrintDataWrapper<decimal?> PaymentTotal { get; set; }

        public PrintDataWrapper<decimal?> BalanceTotal { get; set; }

        public PrintDataWrapper<string> ReferenceNo { get; set; }

        public PrintDataWrapper<DateTime> Date { get; set; }
        
        public PrintDataWrapper<List<Transaction>> Payments { get; set; }

        #endregion

        #region Print Event Handlers

        public override void PrintContent_PrintPage(object sender, PrintPageEventArgs e)
        {
            Font bodyFont = BodyFont != null ? BodyFont : defaultBodyFont;
            Font header1Font = Header1Font != null ? Header1Font : defaultHeader1Font;
            Font header2Font = Header1Font != null ? Header2Font : defaultHeader2Font;
            Font header3Font = Header3Font != null ? Header3Font : defaultHeader3Font;

            e.Graphics.PageUnit = GraphicsUnit.Point;

            float pintableAreaWidthInPoints = GetPrintableAreaWidth(e);

            float x = 0,
                  lineOffset = bodyFont.GetHeight(e.Graphics) - 0.5F,
                  y = lineOffset,
                  lineStringX = e.MarginBounds.Width - e.Graphics.MeasureString(lineString, bodyFont).Width - widthCorrection;

           this.DrawPaymentHeader(e, pintableAreaWidthInPoints, ref y, header1Font, bodyFont);
            
            {//***** TOP SECTION ******

                string[] titles = 
                {
                    TitleAvailableSpend,
                    TitleCashierID,
                    TitleCustomerName,
                    TitleCustomerAddress,
                    TitleCreditLimit,
                    TitleAvailableSpend,
                    TitleReferenceNo,
                    TitleDatePaid
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
                var remainingSpace = pintableAreaWidthInPoints - topSectionContentX;
               
                if (CustomerName.ShouldBePrinted && !String.IsNullOrEmpty(CustomerName.Value))
                {
                    e.Graphics.DrawString(TitleCustomerName, bodyFont, Brushes.Black, x, y);
                    DrawAndWrapString(CustomerName.Value, e.Graphics, bodyFont, remainingSpace, lineOffset, topSectionContentX, ref y);
                }
                if (CustomerAddress1.ShouldBePrinted)
                {
                    e.Graphics.DrawString(TitleCustomerAddress, bodyFont, Brushes.Black, x, y);
                    if (!String.IsNullOrEmpty(CustomerAddress1.Value))
                        DrawAndWrapString(CustomerAddress1.Value, e.Graphics, bodyFont, remainingSpace, lineOffset, topSectionContentX, ref y);
                    if (!String.IsNullOrEmpty(CustomerAddress2.Value))
                        DrawAndWrapString(CustomerAddress2.Value, e.Graphics, bodyFont, remainingSpace, lineOffset, topSectionContentX, ref y);
                    if (!String.IsNullOrEmpty(CustomerAddress3.Value))
                        DrawAndWrapString(CustomerAddress3.Value, e.Graphics, bodyFont, remainingSpace, lineOffset, topSectionContentX, ref y);
                    if (!String.IsNullOrEmpty(CustomerAddress4.Value))
                        DrawAndWrapString(CustomerAddress4.Value, e.Graphics, bodyFont, remainingSpace, lineOffset, topSectionContentX, ref y);
                    if (!String.IsNullOrEmpty(CustomerAddress5.Value))
                        DrawAndWrapString(CustomerAddress5.Value, e.Graphics, bodyFont, remainingSpace, lineOffset, topSectionContentX, ref y);
                }
                NewLine(lineOffset, ref y);
                if (CreditLimit.ShouldBePrinted && CreditLimit.Value.HasValue)
                {
                    e.Graphics.DrawString(TitleCreditLimit, bodyFont, Brushes.Black, x, y);
                    e.Graphics.DrawString(MoneyString(CreditLimit.Value), bodyFont, Brushes.Black, topSectionContentX, y);

                    NewLine(lineOffset, ref y);
                }
                if (AvailableSpend.ShouldBePrinted && AvailableSpend.Value.HasValue)
                {
                    e.Graphics.DrawString(TitleAvailableSpend, bodyFont, Brushes.Black, x, y);
                    e.Graphics.DrawString(MoneyString(AvailableSpend.Value), bodyFont, Brushes.Black, topSectionContentX, y);

                    NewLine(lineOffset, ref y);
                }
                if (ReferenceNo.ShouldBePrinted && !String.IsNullOrEmpty(ReferenceNo.Value))
                {
                    e.Graphics.DrawString(TitleReferenceNo, bodyFont, Brushes.Black, x, y);
                    e.Graphics.DrawString(ReferenceNo.Value, bodyFont, Brushes.Black, topSectionContentX, y);

                    NewLine(lineOffset, ref y);
                }
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

                    e.Graphics.DrawString(TitleDatePaid, bodyFont, Brushes.Black, x, y);
                    e.Graphics.DrawString(dateString, bodyFont, Brushes.Black, topSectionContentX, y);

                    NewLine(lineOffset, ref y);
                }
                if (PaymentTotal.ShouldBePrinted && PaymentTotal.Value != null)
                {
                    e.Graphics.DrawString(TitlePaymentTotal, bodyFont, Brushes.Black, x, y);
                    e.Graphics.DrawString(MoneyString(PaymentTotal.Value), bodyFont, Brushes.Black, topSectionContentX, y);

                    NewLine(lineOffset, ref y);
                }
                if (BalanceTotal.ShouldBePrinted && BalanceTotal.Value != null)
                {
                    e.Graphics.DrawString(TitleBalanceTotal, bodyFont, Brushes.Black, x, y);
                    e.Graphics.DrawString(MoneyString(BalanceTotal.Value), bodyFont, Brushes.Black, topSectionContentX, y);

                    NewLine(lineOffset, ref y);
                }

                NewLine(lineOffset, ref y);

            }//*** END TOP SECTION ****
            
            {//***** PRODUCTS & TOTAL SECTION ******
                if (Payments.ShouldBePrinted && Payments.Value != null)
                {
                    int type = 2;

                    switch (type)
                    {
                        case 0:
                            {
                                string[] strings = 
                                { 
                                    TitleAccountNo, 
                                    TitleAgreementTotal, 
                                    TitlePaymentType,
                                    TitleAmountPaid,
                                    TitleRemainingBalance
                                };

                                float columnWidth;
                                List<float> columns = GetXCoordinatesForNColumns(pintableAreaWidthInPoints, strings.Length, out columnWidth);

                                int i = 0, lines = 1;
                                foreach (string str1 in strings)
                                {
                                    float tmpY = y;
                                    List<string> splitStrings = Split(str1, e.Graphics, columnWidth, bodyFont);
                                    lines = splitStrings.Count > lines ? splitStrings.Count : lines;
                                    foreach (string str2 in splitStrings)
                                    {
                                        e.Graphics.DrawString(str2, bodyFont, Brushes.Black, columns[i], tmpY);
                                        NewLine(lineOffset, ref tmpY);
                                    }
                                    i++;
                                }
                                for (int j = 1; j <= lines; j++)
                                    NewLine(lineOffset, ref y);

                                foreach (Transaction payment in Payments.Value)
                                {
                                    string[] paymentStrings = 
                                { 
                                    payment.AccountNo, 
                                    MoneyString(payment.AgreementTotal), 
                                    payment.PaymentType,
                                    MoneyString(payment.Amount),
                                    MoneyString(payment.RemainingBalance)
                                };
                                    i = 0;
                                    foreach (string str in paymentStrings)
                                    {
                                        e.Graphics.DrawString(str, bodyFont, Brushes.Black, columns[i], y);
                                        i++;
                                    }
                                    NewLine(lineOffset, ref y);
                                }
                            }
                            break;

                        case 1:
                            {
                                string[] titles = 
                                { 
                                    TitleAccountNo, 
                                    TitleAgreementTotal, 
                                    TitlePaymentType,
                                    TitleAmountPaid,
                                    TitleRemainingBalance
                                };

                                float titleWidth = e.Graphics.MeasureString(titles[0], bodyFont).Width;
                                float contentX = titleWidth;
                                float contentOffset = titleWidth * 0.25F;

                                foreach (string title in titles)
                                {
                                    titleWidth = e.Graphics.MeasureString(title, bodyFont).Width;
                                    if (titleWidth > contentX)
                                    {
                                        contentX = titleWidth;
                                        contentOffset = titleWidth * 0.25F;
                                    }
                                }
                                contentX = contentX + contentOffset;

                                DrawLine(e, pintableAreaWidthInPoints, y, bodyFont);
                                NewLine(lineOffset * 0.2F, ref y);
                                DrawLine(e, pintableAreaWidthInPoints, y, bodyFont);
                                NewLine(lineOffset, ref y);

                                e.Graphics.DrawString("Payments", header2Font, Brushes.Black, x, y);
                                NewLine(lineOffset, ref y);
                                DrawLine(e, pintableAreaWidthInPoints, y, bodyFont);

                                foreach (Transaction payment in Payments.Value)
                                {
                                    NewLine(lineOffset, ref y);

                                    e.Graphics.DrawString(TitleAccountNo, bodyFont, Brushes.Black, x, y);
                                    e.Graphics.DrawString(payment.AccountNo, bodyFont, Brushes.Black, contentX, y);
                                    NewLine(lineOffset, ref y);

                                    e.Graphics.DrawString(TitleAgreementTotal, bodyFont, Brushes.Black, x, y);
                                    e.Graphics.DrawString(MoneyString(payment.AgreementTotal), bodyFont, Brushes.Black, contentX, y);
                                    NewLine(lineOffset, ref y);

                                    e.Graphics.DrawString(TitlePaymentType, bodyFont, Brushes.Black, x, y);
                                    e.Graphics.DrawString(payment.PaymentType, bodyFont, Brushes.Black, contentX, y);
                                    NewLine(lineOffset, ref y);

                                    e.Graphics.DrawString(TitleAmountPaid, bodyFont, Brushes.Black, x, y);
                                    e.Graphics.DrawString(MoneyString(payment.Amount), bodyFont, Brushes.Black, contentX, y);
                                    NewLine(lineOffset, ref y);

                                    e.Graphics.DrawString(TitleRemainingBalance, bodyFont, Brushes.Black, x, y);
                                    e.Graphics.DrawString(MoneyString(payment.RemainingBalance), bodyFont, Brushes.Black, contentX, y);
                                    NewLine(lineOffset, ref y);
                                    DrawLine(e, pintableAreaWidthInPoints, y, bodyFont);
                                }
                            }
                            break;

                        case 2:
                            {
                                string[] topTitles = 
                                { 
                                    TitleAccountNo, 
                                    TitleAgreementTotal,
                                };

                                float titleWidth = e.Graphics.MeasureString(topTitles[0], bodyFont).Width;
                                float contentX = titleWidth;
                                float contentOffset = titleWidth * 0.25F;

                                foreach (string title in topTitles)
                                {
                                    titleWidth = e.Graphics.MeasureString(title, bodyFont).Width;
                                    if (titleWidth > contentX)
                                    {
                                        contentX = titleWidth;
                                        contentOffset = titleWidth * 0.25F;
                                    }
                                }
                                contentX = contentX + contentOffset;


                                string[] titles = 
                                { 
                                    TitlePaymentType,
                                    TitleAmountPaid,
                                    TitleRemainingBalance
                                };
                                float columnWidth;
                                List<float> columns1 = GetXCoordinatesForNColumns(pintableAreaWidthInPoints, titles.Length, out columnWidth);

                                NewLine(lineOffset, ref y);

                                e.Graphics.DrawString(TitlePayments, header2Font, Brushes.Black, x, y);
                                
                                NewLine(lineOffset * 0.5F, ref y);
                                DrawLine(e, pintableAreaWidthInPoints, y, new Font(FontFamily.GenericSansSerif, 12F));
                                NewLine(lineOffset * 0.5F, ref y);
                                //DrawLine(e, pintableAreaWidthInPoints, y, bodyFont);

                                List<List<Transaction>> FilteredPayments = new List<List<Transaction>>();
                                foreach (Transaction payment in Payments.Value)
                                {
                                    List<Transaction> filteredPayments = new List<Transaction>();

                                    foreach (Transaction payment1 in Payments.Value)
                                        if (payment1.AccountNo == payment.AccountNo)
                                            filteredPayments.Add(payment1);

                                    FilteredPayments.Add(filteredPayments);
                                }

                                List<String> printed = new List<string>();

                                foreach (List<Transaction> PaymentList in FilteredPayments)
                                {
                                    if (printed.Contains(PaymentList[0].AccountNo))
                                        continue;
                                    printed.Add(PaymentList[0].AccountNo);

                                    NewLine(lineOffset, ref y);
                                    e.Graphics.DrawString(TitleAccountNo, bodyFont, Brushes.Black, x, y);
                                    e.Graphics.DrawString(PaymentList[0].AccountNo, bodyFont, Brushes.Black, contentX, y);
                                    NewLine(lineOffset, ref y);

                                    e.Graphics.DrawString(TitleAgreementTotal, bodyFont, Brushes.Black, x, y);
                                    e.Graphics.DrawString(MoneyString(PaymentList[0].AgreementTotal), bodyFont, Brushes.Black, contentX, y);
                                    NewLine(lineOffset * lineSpace1, ref y);

                                    int i = 0, lines = 1;
                                    foreach (string str1 in titles)
                                    {
                                        float tmpY = y;
                                        List<string> splitStrings = Split(str1, e.Graphics, columnWidth, bodyFont);
                                        lines = splitStrings.Count > lines ? splitStrings.Count : lines;
                                        foreach (string str2 in splitStrings)
                                        {
                                            e.Graphics.DrawString(str2, bodyFont, Brushes.Black, columns1[i], tmpY);
                                            NewLine(lineOffset, ref tmpY);
                                        }
                                        i++;
                                    }
                                    for (int j = 1; j <= lines; j++)
                                        NewLine(lineOffset, ref y);

                                    foreach (Transaction payment2 in PaymentList)
                                    {
                                        string[] paymentStrings = 
                                        { 
                                            payment2.PaymentType,
                                            MoneyString(payment2.Amount),
                                            MoneyString(payment2.RemainingBalance)
                                        };

                                        i = 0;
                                        foreach (string str in paymentStrings)
                                        {
                                            e.Graphics.DrawString(str, bodyFont, Brushes.Black, columns1[i], y);
                                            i++;
                                        }
                                        NewLine(lineOffset, ref y);
                                    }

                                    DrawLine(e, pintableAreaWidthInPoints, y, bodyFont);
                                }

                            }
                            break;

                    }
                }
            }//*** END PRODUCTS & TOTAL SECTION ****

            {//***** FOOTER ******
                //NewLine(lineOffset*0.2F, ref y);
                float footerX;
                DrawLine(e, pintableAreaWidthInPoints, y, bodyFont);

                if (Footer.ShouldBePrinted && !String.IsNullOrEmpty(Footer.Value))
                {
                    NewLine(lineOffset * lineSpace1, ref y);

                    List<string> strings = Split(Footer.Value, e, bodyFont);
                    foreach (string str in strings)
                    {
                        footerX = GetCenterXCoordinate(e.Graphics, pintableAreaWidthInPoints, str, header3Font);
                        e.Graphics.DrawString(str, header3Font, Brushes.Black, footerX, y);
                        NewLine(lineOffset, ref y);
                    }
                }
            }//*** END FOOTER ****

            while ((y / pointsPerMillimeter) < MinimumHeight)
            {//Ensure Receipt is Minimum Height
                NewLine(lineOffset, ref y);                
            }
            e.Graphics.DrawString(".", bodyFont, Brushes.Black, 0, y);

            e.HasMorePages = false;
        }

        #endregion


        #region Private Methods
        private void DrawPaymentHeader(PrintPageEventArgs e, float paperWidthInPoints, ref float y, Font header1Font, Font bodyFont)
        {
            float lineOffset = bodyFont.GetHeight(e.Graphics) - 0.5F;

            if (AdditionalText.ShouldBePrinted && !String.IsNullOrEmpty(AdditionalText.Value))
            {
                e.Graphics.DrawString(AdditionalText.Value, bodyFont, Brushes.Black, 0, y);
                NewLine(lineOffset, ref y);
            }
            if (this.Image.ShouldBePrinted && this.Image.Value != null)
            {
                e.Graphics.DrawImage(this.Image.Value, 0, 0);
                NewLine(this.Image.Value.Height, ref y);
            }
            {//***** HEADER ******
                float headerX;

                if (BusinessTitle.ShouldBePrinted && !String.IsNullOrEmpty(this.BusinessTitle.Value))
                {
                    headerX = GetCenterXCoordinate(e.Graphics, paperWidthInPoints, BusinessTitle.Value, header1Font);
                    e.Graphics.DrawString(this.BusinessTitle.Value, header1Font, Brushes.Black, headerX, y);

                    NewLine(lineOffset * lineSpace2, ref y);
                }
                if (Store.ShouldBePrinted && !String.IsNullOrEmpty(Store.Value))
                {
                    headerX = GetCenterXCoordinate(e.Graphics, paperWidthInPoints, Store.Value, bodyFont);

                    e.Graphics.DrawString(Store.Value, bodyFont, Brushes.Black, headerX, y);
                    NewLine(lineOffset, ref y);
                }
                if (StoreAddress1.ShouldBePrinted)
                {
                    if (!String.IsNullOrEmpty(Store.Value))
                    {
                        headerX = GetCenterXCoordinate(e.Graphics, paperWidthInPoints, StoreAddress1.Value, bodyFont);

                        e.Graphics.DrawString(StoreAddress1.Value, bodyFont, Brushes.Black, headerX, y);
                    }

                    if (!String.IsNullOrEmpty(StoreAddress2.Value))
                    {
                        NewLine(lineOffset, ref y);

                        headerX = GetCenterXCoordinate(e.Graphics, paperWidthInPoints, StoreAddress2.Value, bodyFont);

                        e.Graphics.DrawString(StoreAddress2.Value, bodyFont, Brushes.Black, headerX, y);
                    }

                    if (!String.IsNullOrEmpty(StoreAddress3.Value))
                    {
                        NewLine(lineOffset, ref y);

                        headerX = GetCenterXCoordinate(e.Graphics, paperWidthInPoints, StoreAddress3.Value, bodyFont);

                        e.Graphics.DrawString(StoreAddress3.Value, bodyFont, Brushes.Black, headerX, y);
                    }

                    NewLine(lineOffset * lineSpace1, ref y);
                }
                if (BusinessRegNo.ShouldBePrinted && !String.IsNullOrEmpty(BusinessRegNo.Value))
                {
                    if (BusinessRegNoLabel.ShouldBePrinted)
                    {
                        headerX = GetCenterXCoordinate(e.Graphics, paperWidthInPoints, BusinessRegNoLabel.Value + " " + BusinessRegNo.Value, bodyFont);
                        e.Graphics.DrawString(BusinessRegNoLabel.Value + " " + BusinessRegNo.Value, bodyFont, Brushes.Black, headerX, y);
                    }
                    else
                    {
                        headerX = GetCenterXCoordinate(e.Graphics, paperWidthInPoints, BusinessRegNo.Value, bodyFont);
                        e.Graphics.DrawString(BusinessRegNo.Value, bodyFont, Brushes.Black, headerX, y);
                    }
                    NewLine(lineOffset * lineSpace2, ref y);
                }
                if (BusinessTaxNo.ShouldBePrinted && !String.IsNullOrEmpty(this.BusinessTaxNo.Value))
                {
                    if (BusinessTaxNoLabel.ShouldBePrinted)
                    {
                        headerX = GetCenterXCoordinate(e.Graphics, paperWidthInPoints, BusinessTaxNoLabel.Value + " " + this.BusinessTaxNo.Value, bodyFont);
                        e.Graphics.DrawString(BusinessTaxNoLabel.Value + " " + this.BusinessTaxNo.Value, bodyFont, Brushes.Black, headerX, y);
                    }
                    else
                    {
                        headerX = GetCenterXCoordinate(e.Graphics, paperWidthInPoints, this.BusinessTaxNo.Value, bodyFont);
                        e.Graphics.DrawString(this.BusinessTaxNo.Value, bodyFont, Brushes.Black, headerX, y);
                    }
                    NewLine(lineOffset * lineSpace2, ref y);
                }
                if (Title.ShouldBePrinted && !String.IsNullOrEmpty(Title.Value))
                {
                    e.Graphics.DrawString(Title.Value, bodyFont, Brushes.Black, 0, y);

                    NewLine(lineOffset, ref y);
                }

                DrawLine(e, paperWidthInPoints, y, bodyFont);
                NewLine(lineOffset * lineSpace2, ref y);
            }//*** END HEADER ****
        }

        #endregion
    }
}
