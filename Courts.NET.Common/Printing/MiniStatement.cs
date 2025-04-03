using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Printing;
using System.Drawing;
using BBSL.Libraries.General;

namespace BBSL.Libraries.Printing.PrintDocuments
{
    public class MiniStatement : PrintContent
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
        static string titleDateFrom = "";
        static string titleDateTo = "";

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
        public static string TitleDateFrom { get { return titleDateFrom; } set { titleDateFrom = value; } }
        public static string TitleDateTo { get { return titleDateTo; } set { titleDateTo = value; } }

        #endregion

        #region Constructors

        public MiniStatement()
            : base()
        {
            this.AvailableSpend = new PrintDataWrapper<decimal?>(null, true);
            this.DateFrom = new PrintDataWrapper<DateTime>(default(DateTime), true);
            this.DateTo = new PrintDataWrapper<DateTime>(default(DateTime), true);
            this.Footer = new PrintDataWrapper<string>(default(string), true);
            this.CustomerName = new PrintDataWrapper<string>(default(string), true);
            this.CustomerAddress1 = new PrintDataWrapper<string>(default(string), true);
            this.CustomerAddress2 = new PrintDataWrapper<string>(default(string), true);
            this.CustomerAddress3 = new PrintDataWrapper<string>(default(string), true);
            this.CustomerAddress4 = new PrintDataWrapper<string>(default(string), true);
            this.CustomerAddress5 = new PrintDataWrapper<string>(default(string), true);
            this.CreditLimit = new PrintDataWrapper<decimal?>(default(decimal?), true);
            this.ReferenceNo = new PrintDataWrapper<string>(default(string), true);
            this.Payments = new PrintDataWrapper<List<Transaction>>(new List<Transaction>(), true);
            this.Accounts = new PrintDataWrapper<List<Account>>(new List<Account>(), true);
        }

        #endregion

        #region Public Propeties

        public override string PrintString
        {
            get { throw new NotImplementedException(); }
        }

        public PrintDataWrapper<string> Footer { get; set; }

        public PrintDataWrapper<string> CustomerName { get; set; }

        public PrintDataWrapper<string> CustomerAddress1 { get; set; }

        public PrintDataWrapper<string> CustomerAddress2 { get; set; }

        public PrintDataWrapper<string> CustomerAddress3 { get; set; }

        public PrintDataWrapper<string> CustomerAddress4 { get; set; }

        public PrintDataWrapper<string> CustomerAddress5 { get; set; }

        public PrintDataWrapper<decimal?> CreditLimit { get; set; }

        public PrintDataWrapper<decimal?> AvailableSpend { get; set; }

        public PrintDataWrapper<string> ReferenceNo { get; set; }

        public PrintDataWrapper<DateTime> DateFrom { get; set; }
        
        public PrintDataWrapper<DateTime> DateTo { get; set; }

        public PrintDataWrapper<List<Transaction>> Payments { get; set; }

        public PrintDataWrapper<List<Account>> Accounts { get; set; }

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

            DrawHeader(e, pintableAreaWidthInPoints, ref y, header1Font, bodyFont);

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
                    if (AvailableSpend.Value < 0)
                        AvailableSpend.Value = 0;
                    e.Graphics.DrawString(TitleAvailableSpend, bodyFont, Brushes.Black, x, y);
                    e.Graphics.DrawString(MoneyString(AvailableSpend.Value), bodyFont, Brushes.Black, topSectionContentX, y);

                    NewLine(lineOffset, ref y);
                }
                if (DateFrom.ShouldBePrinted && DateFrom.Value != null)
                {
                    string dateString = "";

                    switch (Receipt.DateTimeFormat)
                    {
                        case DateTimeFormats.LongDate:
                            dateString = DateFrom.Value.ToLongDateString(); break;
                        case DateTimeFormats.LongTime:
                            dateString = DateFrom.Value.ToLongTimeString(); break;
                        case DateTimeFormats.LongDateAndTime:
                            dateString = DateFrom.Value.ToLongDateString() + " " + DateFrom.Value.ToLongTimeString(); break;
                        case DateTimeFormats.LongDateAndShortTime:
                            dateString = DateFrom.Value.ToLongDateString() + " " + DateFrom.Value.ToShortTimeString(); break;
                        case DateTimeFormats.ShortDate:
                            dateString = DateFrom.Value.ToShortDateString(); break;
                        case DateTimeFormats.ShortTime:
                            dateString = DateFrom.Value.ToShortTimeString(); break;
                        case DateTimeFormats.ShortDateAndTime:
                            dateString = DateFrom.Value.ToShortDateString() + " " + DateFrom.Value.ToShortTimeString(); break;
                        case DateTimeFormats.ShortDateAndLongTime:
                            dateString = DateFrom.Value.ToShortDateString() + " " + DateFrom.Value.ToLongTimeString(); break;
                    }

                    e.Graphics.DrawString(TitleDateFrom, bodyFont, Brushes.Black, x, y);
                    e.Graphics.DrawString(dateString, bodyFont, Brushes.Black, topSectionContentX, y);

                    NewLine(lineOffset, ref y);
                } 
                if (DateTo.ShouldBePrinted && DateFrom.Value != null)
                {
                    string dateString = "";

                    switch (Receipt.DateTimeFormat)
                    {
                        case DateTimeFormats.LongDate:
                            dateString = DateTo.Value.ToLongDateString(); break;
                        case DateTimeFormats.LongTime:
                            dateString = DateTo.Value.ToLongTimeString(); break;
                        case DateTimeFormats.LongDateAndTime:
                            dateString = DateTo.Value.ToLongDateString() + " " + DateFrom.Value.ToLongTimeString(); break;
                        case DateTimeFormats.LongDateAndShortTime:
                            dateString = DateTo.Value.ToLongDateString() + " " + DateFrom.Value.ToShortTimeString(); break;
                        case DateTimeFormats.ShortDate:
                            dateString = DateTo.Value.ToShortDateString(); break;
                        case DateTimeFormats.ShortTime:
                            dateString = DateTo.Value.ToShortTimeString(); break;
                        case DateTimeFormats.ShortDateAndTime:
                            dateString = DateTo.Value.ToShortDateString() + " " + DateFrom.Value.ToShortTimeString(); break;
                        case DateTimeFormats.ShortDateAndLongTime:
                            dateString = DateTo.Value.ToShortDateString() + " " + DateFrom.Value.ToLongTimeString(); break;
                    }

                    e.Graphics.DrawString(TitleDateTo, bodyFont, Brushes.Black, x, y);
                    e.Graphics.DrawString(dateString, bodyFont, Brushes.Black, topSectionContentX, y);

                    NewLine(lineOffset, ref y);
                } 

            }//*** END TOP SECTION ****

            {//***** PRODUCTS & TOTAL SECTION ******
                if (Payments.ShouldBePrinted && Payments.Value != null)
                {
                    NewLine(lineOffset, ref y);

                    e.Graphics.DrawString(TitlePayments, header2Font, Brushes.Black, x, y);
                    NewLine(lineOffset, ref y);
                    DrawLine(e, pintableAreaWidthInPoints, y, bodyFont);
                    NewLine(lineOffset * 0.1F, ref y);
                    DrawLine(e, pintableAreaWidthInPoints, y, bodyFont);

                    string[] topTitles = 
                                { 
                                    TitleAccountNo,
                                    TitleAgreementTotal
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
                                    TitleDatePaid,
                                    TitleReferenceNo,
                                    TitlePaymentType,
                                    TitleAmountPaid,
                                    //TitleAgreementTotal,
                                    TitleRemainingBalance
                                };
                    float columnWidth;
                    List<float> columns = GetXCoordinatesForNColumns(pintableAreaWidthInPoints, titles.Length, out columnWidth);

                    switch(_paymentDiplayType)
                    {
                        case PaymentDiplayTypes.ByAccount:
                            {
                                foreach (Account account in Accounts.Value)
                                {
                                    NewLine(lineOffset, ref y);
                                    e.Graphics.DrawString(TitleAccountNo, bodyFont, Brushes.Black, x, y);
                                    e.Graphics.DrawString(account.AccountNo, bodyFont, Brushes.Black, contentX, y);
                                    NewLine(lineOffset, ref y);
                                    e.Graphics.DrawString(TitleAgreementTotal, bodyFont, Brushes.Black, x, y);
                                    e.Graphics.DrawString(MoneyString(account.AgreementTotal), bodyFont, Brushes.Black, contentX, y);
                                    NewLine(lineOffset * lineSpace1, ref y);

                                    //Write the column headings
                                    int i = 0, lines = 1;
                                    foreach (string str1 in titles)
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

                                    foreach (Transaction payment in account.Transactions)
                                    {
                                        string[] paymentStrings = 
                                        { 
                                            payment.Date.ToShortDateString(),
                                            payment.ReferenceNo,
                                            payment.PaymentType,
                                            MoneyStringWithoutCurrencySymbol(payment.Amount),
                                            //MoneyStringWithoutCurrencySymbol(payment.AgreementTotal),
                                            MoneyStringWithoutCurrencySymbol(payment.RemainingBalance)
                                        };

                                        i = 0;
                                        foreach (string str in paymentStrings)
                                        {
                                            e.Graphics.DrawString(str, new Font("Microsoft Sans Serif", 6.5F, FontStyle.Regular, GraphicsUnit.Point), Brushes.Black, columns[i], y);
                                            i++;
                                        }
                                        NewLine(lineOffset, ref y);
                                    }
                                    DrawLine(e, pintableAreaWidthInPoints, y, bodyFont);
                                }
                            } break;
                        case PaymentDiplayTypes.byAccountByPaymentType:
                            {
                                #region paymentsByAccountByPaymentType

                                List<string> accountNos = new List<string>();
                                List<string> paymentTypes = new List<string>();
                                Dictionary<string, Dictionary<string, List<Transaction>>> paymentsByAccountByPaymentType = new Dictionary<string, Dictionary<string, List<Transaction>>>();
                                foreach (Transaction payment in Payments.Value)
                                {
                                    accountNos.Add(payment.AccountNo);
                                    paymentTypes.Add(payment.PaymentType);
                                }
                                accountNos.RemoveDuplicates();
                                paymentTypes.RemoveDuplicates();
                                foreach (string accountNo in accountNos)
                                {
                                    paymentsByAccountByPaymentType.Add(accountNo, new Dictionary<string, List<Transaction>>());
                                    foreach (string paymentType in paymentTypes)
                                        paymentsByAccountByPaymentType[accountNo].Add(paymentType,
                                            Payments.Value.FindAll(new Predicate<Transaction>(delegate(Transaction payment) { return payment.AccountNo == accountNo; }))
                                            .FindAll(new Predicate<Transaction>(delegate(Transaction payment) { return payment.PaymentType == paymentType; })));
                                }

                                foreach (string accountNo in paymentsByAccountByPaymentType.Keys)
                                {
                                    NewLine(lineOffset, ref y);
                                    e.Graphics.DrawString(TitleAccountNo, bodyFont, Brushes.Black, x, y);
                                    e.Graphics.DrawString(accountNo, bodyFont, Brushes.Black, contentX, y);
                                    NewLine(lineOffset, ref y);
                                    e.Graphics.DrawString(TitleAgreementTotal, bodyFont, Brushes.Black, x, y);
                                    e.Graphics.DrawString(MoneyString(paymentsByAccountByPaymentType[accountNo]["Cash"][0].AgreementTotal), bodyFont, Brushes.Black, contentX, y);
                                    NewLine(lineOffset * lineSpace1, ref y);

                                    int i = 0, lines = 1;
                                    foreach (string str1 in titles)
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

                                    foreach (string paymentType in paymentsByAccountByPaymentType[accountNo].Keys)
                                    {
                                        if (paymentsByAccountByPaymentType[accountNo][paymentType].Count == 0)
                                            continue;

                                        //e.Graphics.DrawString(TitlePaymentType + " " + paymentType, bodyFont, Brushes.Black, x, y);
                                        //e.Graphics.DrawString(paymentType, bodyFont, Brushes.Black, x, y);
                                        //NewLine(lineOffset, ref y);


                                        foreach (Transaction payment in paymentsByAccountByPaymentType[accountNo][paymentType])
                                        {
                                            string[] paymentStrings = 
                                        { 
                                            payment.Date.ToShortDateString(),
                                            payment.ReferenceNo,
                                            payment.PaymentType,
                                            MoneyStringWithoutCurrencySymbol(payment.Amount),
                                            //MoneyStringWithoutCurrencySymbol(payment.AgreementTotal),
                                            MoneyStringWithoutCurrencySymbol(payment.RemainingBalance)
                                        };

                                            i = 0;
                                            foreach (string str in paymentStrings)
                                            {
                                                e.Graphics.DrawString(str, new Font("Microsoft Sans Serif", 6.5F, FontStyle.Regular, GraphicsUnit.Point), Brushes.Black, columns[i], y);
                                                i++;
                                            }
                                            NewLine(lineOffset, ref y);
                                        }
                                    }
                                    DrawLine(e, pintableAreaWidthInPoints, y, bodyFont);
                                }

                                #endregion
                            }break;
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

        PaymentDiplayTypes _paymentDiplayType = PaymentDiplayTypes.ByAccount;
        public PaymentDiplayTypes PaymentDiplayType { set { _paymentDiplayType = value; } }
        public enum PaymentDiplayTypes
        {
            ByAccount,
            byAccountByPaymentType
        }
    }
}
