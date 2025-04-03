using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Printing;
using System.Drawing;
using BBSL.Libraries.General;

namespace BBSL.Libraries.Printing.PrintDocuments
{
    public class CashLoanReceipt : PrintContent
    {
        #region Title Text

        static string titleCustomerName = "";
        static string titleAccountNo = "";
        static string titleTransactionDate = "";
        static string titleType = "";
        static string titleAmountDisbursed = "";
        static string titleTransactionNo = "";
        static string titleAccountBalance = "";
        static string titleFirstPaymentDate = "";
        static string disbursementType = "";

        static PrintDataWrapper<string> footer = new PrintDataWrapper<string>("", true);

        public static string TitleCustomerName { get { return titleCustomerName; } set { titleCustomerName = value; } }
        public static string TitleAccountNo { get { return titleAccountNo; } set { titleAccountNo = value; } }
        public static string TitleTransactionDate { get { return titleTransactionDate; } set { titleTransactionDate = value; } }
        public static string TitleType { get { return titleType; } set { titleType = value; } }
        public static string TitleAmountDisbursed { get { return titleAmountDisbursed; } set { titleAmountDisbursed = value; } }
        public static string TitleTransactionNo { get { return titleTransactionNo; } set { titleTransactionNo = value; } }
        public static string TitleAccountBalance { get { return titleAccountBalance; } set { titleAccountBalance = value; } }
        public static string TitleFirstPaymentDate { get { return titleFirstPaymentDate; } set { titleFirstPaymentDate = value; } }
        public static string TitleDisbursementType { get { return disbursementType; } set { disbursementType = value; } }

        public static PrintDataWrapper<string> Footer { get { return footer; } set { footer = value; } }

        #endregion

         #region Constructors

        public CashLoanReceipt()
            : base()
        {
            this.CustomerName = new PrintDataWrapper<string>(default(string), true);
            this.AccountNo = new PrintDataWrapper<string>(default(string), true);
            this.TransactionDate = new PrintDataWrapper<DateTime>(default(DateTime), true);
            this.Type = new PrintDataWrapper<string>(default(string), true);
            this.AmountDisbursed = new PrintDataWrapper<decimal?>(null, true);
            this.TransactionNo = new PrintDataWrapper<string>(default(string), true);
            this.AccountBalance = new PrintDataWrapper<decimal?>(null, true);
            this.FirstPaymentDate = new PrintDataWrapper<DateTime>(default(DateTime), true);
            this.DisbursementType = new PrintDataWrapper<string>(default(string), true);
        }

        #endregion

        #region Public Propeties

        public override string PrintString
        {
            get { throw new NotImplementedException(); }
        }


        public PrintDataWrapper<string> CustomerName { get; set; }

        public PrintDataWrapper<string> AccountNo { get; set; }

        public PrintDataWrapper<DateTime> TransactionDate { get; set; }

        public PrintDataWrapper<string> Type { get; set; }

        public PrintDataWrapper<decimal?> AmountDisbursed { get; set; }

        public PrintDataWrapper<string> TransactionNo { get; set; }

        public PrintDataWrapper<decimal?> AccountBalance { get; set; }

        public PrintDataWrapper<DateTime> FirstPaymentDate { get; set; }

        public PrintDataWrapper<string> DisbursementType { get; set; }

        
        #endregion

        #region Print Event Handlers

        public override void PrintContent_PrintPage(object sender, PrintPageEventArgs e)
        {
            Font bodyFont = BodyFont != null ? BodyFont : defaultBodyFont;
            Font header1Font = Header1Font != null ? Header1Font : defaultHeader1Font;
            Font header2Font = Header1Font != null ? Header2Font : defaultHeader2Font;
            Font header3Font = Header3Font != null ? Header3Font : defaultHeader3Font;

            e.Graphics.PageUnit = GraphicsUnit.Point;

            float paperWidthInPoints = GetPrintableAreaWidth(e);

            float x = 0,
                  y = 0,
                  lineOffset = bodyFont.GetHeight(e.Graphics) - 0.5F,
                  lineStringX = e.MarginBounds.Width - e.Graphics.MeasureString(lineString, bodyFont).Width - widthCorrection;

            Action<float> NewLine =
                delegate(float offset)
                {
                    y += offset;
                };


            string[] titles = 
            {
                TitleCustomerName,
                TitleAccountNo, 
                TitleTransactionDate,
                TitleType, 
                TitleAmountDisbursed,
                TitleTransactionNo,
                TitleAccountBalance,
                TitleFirstPaymentDate,
                TitleDisbursementType
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
            float pintableAreaWidthInPoints = GetPrintableAreaWidth(e);                     //IP - 18/05/12 - #10143
            var remainingSpace = pintableAreaWidthInPoints - topSectionContentX;            //IP - 18/05/12 - #10143

            DrawHeader(e, paperWidthInPoints, ref y, header1Font, bodyFont);

            if (CustomerName.ShouldBePrinted && !String.IsNullOrEmpty(CustomerName.Value))
            {
                e.Graphics.DrawString(TitleCustomerName, bodyFont, Brushes.Black, x, y);
                //e.Graphics.DrawString(CustomerName.Value, bodyFont, Brushes.Black, topSectionContentX, y);
                DrawAndWrapString(CustomerName.Value, e.Graphics, bodyFont, remainingSpace, lineOffset, topSectionContentX, ref y);             //IP - 18/05/12 - #10143

                NewLine(lineOffset);
            }
            if (AccountNo.ShouldBePrinted && !String.IsNullOrEmpty(AccountNo.Value))
            {
                e.Graphics.DrawString(TitleAccountNo, bodyFont, Brushes.Black, x, y);
                e.Graphics.DrawString(AccountNo.Value, bodyFont, Brushes.Black, topSectionContentX, y);

                NewLine(lineOffset);
            }
            if (AccountBalance.ShouldBePrinted && AccountBalance.Value.HasValue)
            {
                e.Graphics.DrawString(TitleAccountBalance, bodyFont, Brushes.Black, x, y);
                e.Graphics.DrawString(MoneyString(AccountBalance.Value), bodyFont, Brushes.Black, topSectionContentX, y);

                NewLine(lineOffset * lineSpace2);
            }
            if (DisbursementType.ShouldBePrinted)
            {
                e.Graphics.DrawString(TitleDisbursementType, bodyFont, Brushes.Black, x, y);
                e.Graphics.DrawString(DisbursementType.Value, bodyFont, Brushes.Black, topSectionContentX, y);

                NewLine(lineOffset * lineSpace2);
            }
            if (TransactionDate.ShouldBePrinted && TransactionDate.Value != default(DateTime))
            {
                string dateString = "";

                switch (CashLoanReceipt.DateTimeFormat)
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
                NewLine(lineOffset);
            }
            if (Type.ShouldBePrinted && !String.IsNullOrEmpty(Type.Value))
            {
                e.Graphics.DrawString(TitleType, bodyFont, Brushes.Black, x, y);
                e.Graphics.DrawString(Type.Value, bodyFont, Brushes.Black, topSectionContentX, y);

                NewLine(lineOffset);
            }
            if (TransactionNo.ShouldBePrinted && !String.IsNullOrEmpty(Type.Value))
            {
                e.Graphics.DrawString(TitleTransactionNo, bodyFont, Brushes.Black, x, y);
                e.Graphics.DrawString(TransactionNo.Value, bodyFont, Brushes.Black, topSectionContentX, y);

                NewLine(lineOffset);
            }
            if (AmountDisbursed.ShouldBePrinted && AmountDisbursed.Value.HasValue)
            {
                e.Graphics.DrawString(TitleAmountDisbursed, bodyFont, Brushes.Black, x, y);
                e.Graphics.DrawString(MoneyString(AmountDisbursed.Value), bodyFont, Brushes.Black, topSectionContentX, y);

                NewLine(lineOffset * lineSpace2);
            }
            if (FirstPaymentDate.ShouldBePrinted && FirstPaymentDate.Value != default(DateTime))
            {
                string dateString = "";

                //switch (CashLoanReceipt.DateTimeFormat)
                //{
                //    case DateTimeFormats.LongDate:
                //        dateString = FirstPaymentDate.Value.ToLongDateString(); break;
                //    case DateTimeFormats.LongTime:
                //        dateString = FirstPaymentDate.Value.ToLongTimeString(); break;
                //    case DateTimeFormats.LongDateAndTime:
                //        dateString = FirstPaymentDate.Value.ToLongDateString() + " " + FirstPaymentDate.Value.ToLongTimeString(); break;
                //    case DateTimeFormats.LongDateAndShortTime:
                //        dateString = FirstPaymentDate.Value.ToLongDateString() + " " + FirstPaymentDate.Value.ToShortTimeString(); break;
                //    case DateTimeFormats.ShortDate:
                //        dateString = FirstPaymentDate.Value.ToShortDateString(); break;
                //    case DateTimeFormats.ShortTime:
                //        dateString = FirstPaymentDate.Value.ToShortTimeString(); break;
                //    case DateTimeFormats.ShortDateAndTime:
                //        dateString = FirstPaymentDate.Value.ToShortDateString() + " " + FirstPaymentDate.Value.ToShortTimeString(); break;
                //    case DateTimeFormats.ShortDateAndLongTime:
                //        dateString = FirstPaymentDate.Value.ToShortDateString() + " " + FirstPaymentDate.Value.ToLongTimeString(); break;
                //}

                dateString = FirstPaymentDate.Value.ToShortDateString();        // #11419 Short date only

                e.Graphics.DrawString(TitleFirstPaymentDate, bodyFont, Brushes.Black, x, y);
                e.Graphics.DrawString(dateString, bodyFont, Brushes.Black, topSectionContentX, y);
                NewLine(lineOffset);
            }


            //if (SalesPerson.ShouldBePrinted && !String.IsNullOrEmpty(SalesPerson.Value))
            //{
            //    e.Graphics.DrawString(TitleSalesPerson, bodyFont, Brushes.Black, x, y);
            //    e.Graphics.DrawString(SalesPerson.Value, bodyFont, Brushes.Black, topSectionContentX, y);

            //    NewLine(lineOffset);
            //}
            //if (CashierID.ShouldBePrinted && !String.IsNullOrEmpty(CashierID.Value))
            //{
            //    e.Graphics.DrawString(TitleCashierID, bodyFont, Brushes.Black, x, y);
            //    e.Graphics.DrawString(CashierID.Value, bodyFont, Brushes.Black, topSectionContentX, y);

            //    NewLine(lineOffset);
            //}
            //if (BranchCode.ShouldBePrinted && !String.IsNullOrEmpty(BranchCode.Value))
            //{
            //    e.Graphics.DrawString(TitleBranchCode, bodyFont, Brushes.Black, x, y);
            //    e.Graphics.DrawString(BranchCode.Value, bodyFont, Brushes.Black, topSectionContentX, y);

            //    NewLine(lineOffset);
            //}

            {//***** FOOTER ******
                NewLine(lineOffset);
                float footerX;
                string divider = CreateFullLine(DividerCharacter, e, bodyFont, paperWidthInPoints);
                footerX = GetCenterXCoordinate(e.Graphics, paperWidthInPoints, divider, bodyFont);
                e.Graphics.DrawString(divider, bodyFont, Brushes.Black, footerX, y);

                if (Footer.ShouldBePrinted && !String.IsNullOrEmpty(Footer.Value))
                {
                    NewLine(lineOffset * lineSpace1);

                    List<string> strings = Split(Footer.Value, e, bodyFont);
                    foreach (string str in strings)
                    {
                        footerX = GetCenterXCoordinate(e.Graphics, paperWidthInPoints, str, header3Font);
                        e.Graphics.DrawString(str, header3Font, Brushes.Black, footerX, y);
                        NewLine(lineOffset);
                    }
                }
            }//*** END FOOTER ****

            do
            {
                NewLine(lineOffset);
                e.Graphics.DrawString(".", bodyFont, Brushes.Black, 0, y);
            } while ((y / pointsPerMillimeter) < MinimumHeight) ;

            e.HasMorePages = false;
        }

        #endregion
    }
}
