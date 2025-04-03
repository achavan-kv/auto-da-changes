using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Printing;
using System.Drawing;
using BBSL.Libraries.General;

namespace BBSL.Libraries.Printing.PrintDocuments
{
    public class StoreCardReceipt : PrintContent
    {
        #region Title Text
                  
        static string titleCustomerName = "";
        static string titleCustomerAddress = "";
        static string titleMachineName = "";
        static string titleInvoiceNumber = "";
        static string titleReceiptNumber = "";
        static string titleStoreCardName = "";
        static string titleStoreCardNumber = "";
        static string titleStoreCardLimit = "";
        static string titleStoreCardAvailable= "";
        static string titleStoreCardExpiryDate = "";
        static string titleAmountPaid = "";
        static string titleTransactionDate = "";
        static string titleSignatureText = "";

      
        public static string TitleCustomerName { get { return titleCustomerName; } set { titleCustomerName = value; } }
        public static string TitleCustomerAddress { get { return titleCustomerAddress; } set { titleCustomerAddress = value; } }
        public static string TitleMachineName { get { return titleMachineName; } set { titleMachineName = value; } }
        public static string TitleInvoiceNumber { get { return titleInvoiceNumber; } set { titleInvoiceNumber = value; } }
        public static string TitleReceiptNumber { get { return titleReceiptNumber; } set { titleReceiptNumber = value; } }
        public static string TitleStoreCardName { get { return titleStoreCardName; } set { titleStoreCardName = value; } }
        public static string TitleStoreCardNumber { get { return titleStoreCardNumber; } set { titleStoreCardNumber = value; } }
        public static string TitleStoreCardLimit { get { return titleStoreCardLimit; } set { titleStoreCardLimit = value; } }
        public static string TitleStoreCardAvailable { get { return titleStoreCardAvailable; } set { titleStoreCardAvailable = value; } }
        public static string TitleStoreCardExpiryDate { get { return titleStoreCardExpiryDate; } set { titleStoreCardExpiryDate = value; } }
        public static string TitleAmountPaid { get { return titleAmountPaid; } set { titleAmountPaid = value; } }
        public static string TitleTransactionDate { get { return titleTransactionDate; } set { titleTransactionDate = value; } }
        public static string TitleSignatureText { get { return titleSignatureText; } set { titleSignatureText = value; } }

        #endregion

        #region Constructors

        public StoreCardReceipt()
            : base()
        {

            this.CustomerName = new PrintDataWrapper<string>(default(string), true);
            this.CustomerAddress1 = new PrintDataWrapper<string>(default(string), true);
            this.CustomerAddress2 = new PrintDataWrapper<string>(default(string), true);
            this.CustomerAddress3 = new PrintDataWrapper<string>(default(string), true);
            this.CustomerAddress4 = new PrintDataWrapper<string>(default(string), true);
            this.CustomerAddress5 = new PrintDataWrapper<string>(default(string), true);
            this.MachineName= new PrintDataWrapper<string>(default(string), true);
            this.StoreCardName = new PrintDataWrapper<string>(default(string), true);
            this.InvoiceNumber = new PrintDataWrapper<string>(default(string), true);
            this.ReceiptNumber = new PrintDataWrapper<int?>(default(int?), true);
            this.StoreCardNumber = new PrintDataWrapper<string>(default(string), true);
            this.StoreCardLimit = new PrintDataWrapper<decimal?>(default(decimal?), true);
            this.StoreCardAvailableSpend = new PrintDataWrapper<decimal?>(default(decimal?), true);
            this.StoreCardExpiryDate = new PrintDataWrapper<string>(default(string),true);
            this.TransactionDate = new PrintDataWrapper<DateTime>(default(DateTime), true);
            this.AmountPaid = new PrintDataWrapper<decimal?>(default(decimal?), true);
            this.Footer = new PrintDataWrapper<string>(default(string), true);
 
        }

        #endregion

        #region Public Properties

        public override string PrintString
        {
            get { throw new NotImplementedException(); }
        }

        public PrintDataWrapper<string> CustomerName { get; set; }

        public PrintDataWrapper<string> CustomerAddress1 { get; set; }

        public PrintDataWrapper<string> CustomerAddress2 { get; set; }

        public PrintDataWrapper<string> CustomerAddress3 { get; set; }

        public PrintDataWrapper<string> CustomerAddress4 { get; set; }

        public PrintDataWrapper<string> CustomerAddress5 { get; set; }

        public PrintDataWrapper<string> MachineName { get; set; }

        public PrintDataWrapper<string> InvoiceNumber { get; set; }

        public PrintDataWrapper<int?> ReceiptNumber { get; set; }

        public PrintDataWrapper<string> StoreCardName { get; set; }

        public PrintDataWrapper<string> StoreCardNumber { get; set; }

        public PrintDataWrapper<decimal?> StoreCardLimit { get; set; }

        public PrintDataWrapper<decimal?> StoreCardAvailableSpend { get; set; }

        public PrintDataWrapper<string> StoreCardExpiryDate { get; set; }

        public PrintDataWrapper<DateTime> TransactionDate { get; set; }

        public PrintDataWrapper<decimal?> AmountPaid{ get; set; }

        public PrintDataWrapper<string> Footer { get; set; }

        public bool IncludeSignatureStrip { get; set; }

        #endregion

        #region Print Event Handlers

        public override void PrintContent_PrintPage(object sender, PrintPageEventArgs e)
        {
            Font bodyFont = BodyFont != null ? BodyFont : defaultBodyFont;
            Font header1Font = Header1Font != null ? Header1Font : defaultHeader1Font;
            Font header2Font = Header1Font != null ? Header2Font : defaultHeader2Font;
            Font header3Font = Header3Font != null ? Header3Font : defaultHeader3Font;
            Font smallPrint = smallPrintFont;   //IP - 18/01/11 

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
                   TitleCustomerName,
                   TitleCustomerAddress,
                   TitleStoreCardNumber,
                   TitleStoreCardLimit,
                   TitleStoreCardAvailable,
                   TitleTransactionDate,
                   TitleAmountPaid
                };

                float titleWidth = e.Graphics.MeasureString(titles[0], bodyFont).Width;
                float topSectionContentX = titleWidth;
                float topSectionContentOffset = titleWidth * 0.25F;

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

                if (MachineName.ShouldBePrinted && !string.IsNullOrEmpty(MachineName.Value))
                {
                    e.Graphics.DrawString(TitleMachineName, bodyFont, Brushes.Black, x, y);
                    e.Graphics.DrawString(Convert.ToString(MachineName.Value), bodyFont, Brushes.Black, topSectionContentX, y);

                    NewLine(lineOffset, ref y);
                }

                if (InvoiceNumber.ShouldBePrinted && !string.IsNullOrEmpty(InvoiceNumber.Value))
                {
                    e.Graphics.DrawString(TitleInvoiceNumber, bodyFont, Brushes.Black, x, y);
                    e.Graphics.DrawString(Convert.ToString(InvoiceNumber.Value), bodyFont, Brushes.Black, topSectionContentX, y);

                    NewLine(lineOffset, ref y);
                }

                if (ReceiptNumber.ShouldBePrinted && ReceiptNumber.Value.HasValue)
                {
                    e.Graphics.DrawString(TitleReceiptNumber, bodyFont, Brushes.Black, x, y);
                    e.Graphics.DrawString(Convert.ToString(ReceiptNumber.Value), bodyFont, Brushes.Black, topSectionContentX, y);

                    NewLine(lineOffset, ref y);
                }

                NewLine(lineOffset, ref y);

                if (StoreCardName.ShouldBePrinted && !string.IsNullOrEmpty(StoreCardName.Value))
                {
                    e.Graphics.DrawString(TitleStoreCardName, bodyFont, Brushes.Black, x, y);
                    e.Graphics.DrawString(Convert.ToString(StoreCardName.Value), bodyFont, Brushes.Black, topSectionContentX, y);

                    NewLine(lineOffset, ref y);
                }

                if (StoreCardNumber.ShouldBePrinted && !string.IsNullOrEmpty(StoreCardNumber.Value))
                {
                    e.Graphics.DrawString(TitleStoreCardNumber, bodyFont, Brushes.Black, x, y);
                    e.Graphics.DrawString(Convert.ToString(StoreCardNumber.Value), bodyFont, Brushes.Black, topSectionContentX, y);

                    NewLine(lineOffset, ref y);
                }

                if (StoreCardExpiryDate.ShouldBePrinted && !string.IsNullOrEmpty(StoreCardExpiryDate.Value))
                {
                    e.Graphics.DrawString(TitleStoreCardExpiryDate, bodyFont, Brushes.Black, x, y);
                    e.Graphics.DrawString(Convert.ToString(StoreCardExpiryDate.Value), bodyFont, Brushes.Black, topSectionContentX, y);

                    NewLine(lineOffset, ref y);
                }


                if (StoreCardLimit.ShouldBePrinted && StoreCardLimit.Value.HasValue)
                {
                    e.Graphics.DrawString(TitleStoreCardLimit, bodyFont, Brushes.Black, x, y);
                    e.Graphics.DrawString(MoneyString(StoreCardLimit.Value), bodyFont, Brushes.Black, topSectionContentX, y);

                    NewLine(lineOffset, ref y);
                }

                if (StoreCardAvailableSpend.ShouldBePrinted && StoreCardAvailableSpend.Value.HasValue)
                {
                    e.Graphics.DrawString(TitleStoreCardAvailable, bodyFont, Brushes.Black, x, y);
                    e.Graphics.DrawString(MoneyString(StoreCardAvailableSpend.Value), bodyFont, Brushes.Black, topSectionContentX, y);

                    NewLine(lineOffset, ref y);
                }

                if (TransactionDate.ShouldBePrinted && TransactionDate.Value != null)
                {
                    string dateString = "";

                    switch (StoreCardReceipt.dateTimeFormat)
                    {
                        case DateTimeFormats.LongDate:
                            dateString = TransactionDate.Value.ToLongDateString(); break;
                        case DateTimeFormats.LongTime:
                            dateString = TransactionDate.Value.ToLongTimeString(); break;
                        case DateTimeFormats.LongDateAndTime:
                            dateString = TransactionDate.Value.ToLongDateString() + " " + TransactionDate.Value.ToLongTimeString(); break;
                        case DateTimeFormats.LongDateAndShortTime:
                            dateString = TransactionDate.Value.ToLongDateString() + " " + TransactionDate.Value.ToShortTimeString(); break;
                        case DateTimeFormats.ShortDate:
                            dateString = TransactionDate.Value.ToShortDateString(); break;
                        case DateTimeFormats.ShortTime:
                            dateString = TransactionDate.Value.ToShortTimeString(); break;
                        case DateTimeFormats.ShortDateAndTime:
                            dateString = TransactionDate.Value.ToShortDateString() + " " + TransactionDate.Value.ToShortTimeString(); break;
                        case DateTimeFormats.ShortDateAndLongTime:
                            dateString = TransactionDate.Value.ToShortDateString() + " " + TransactionDate.Value.ToLongTimeString(); break;
  
                    }

                    e.Graphics.DrawString(TitleTransactionDate, bodyFont, Brushes.Black, x, y);
                    e.Graphics.DrawString(dateString, bodyFont, Brushes.Black, topSectionContentX, y);
                    NewLine(lineOffset, ref y);
                }

                if (AmountPaid.ShouldBePrinted && AmountPaid.Value.HasValue)
                {
                    e.Graphics.DrawString(TitleAmountPaid, bodyFont, Brushes.Black, x, y);
                    e.Graphics.DrawString(MoneyString(AmountPaid.Value), bodyFont, Brushes.Black, topSectionContentX, y);

                    NewLine(lineOffset, ref y);
                }


            }//*** END TOP SECTION ****

            {//***** SIGNATURE ******

                if (IncludeSignatureStrip)
                {
                    string divider = CreateFullLine(DividerCharacter, e, bodyFont, pintableAreaWidthInPoints);
                    e.Graphics.DrawString(divider, bodyFont, Brushes.Black, x, y);

                    for (int i = 0; i < 10; i++)
                    {
                        NewLine(lineOffset, ref y);
                    }

                    x = GetRightXCoordinate(e.Graphics, pintableAreaWidthInPoints, TitleSignatureText, bodyFont);
                    e.Graphics.DrawString(TitleSignatureText, bodyFont, Brushes.Black, x, y);
                    x = 0;
                }

            }//*** END SIGNATURE ****

            NewLine(lineOffset, ref y);

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
                        //footerX = GetCenterXCoordinate(e.Graphics, pintableAreaWidthInPoints, str, header3Font);
                        footerX = GetCenterXCoordinate(e.Graphics, pintableAreaWidthInPoints, str, smallPrint);
                        //e.Graphics.DrawString(str, header3Font, Brushes.Black, footerX, y);
                        e.Graphics.DrawString(str, smallPrint, Brushes.Black, footerX, y);
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

       
    }
}
