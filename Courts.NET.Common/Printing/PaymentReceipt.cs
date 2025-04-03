using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Printing;
using System.Drawing;
using BBSL.Libraries.General;

namespace BBSL.Libraries.Printing.PrintDocuments
{
    public class PaymentReceipt : PrintContent
    {
        #region Title Text

        static string titleAccountNo = "";
        static string titleAccountBalance = "";
        static string titleAvailableSpend = "";
        static string titleTransactionNo = "";
        static string titleType = "";
        static string titleAmount = "";
        static string titleCustomerName = "";
        static string titleStoreAddress = "";
        static string titleBranchCode = "";
        static string titleTransactionDate = "";
        static string titleSalesPerson = "";
        static string titleCashierID = "";
        
        static PrintDataWrapper<string> footer = new PrintDataWrapper<string>("", true);

        public static string TitleType { get { return titleType; } set { titleType = value; } }
        public static string TitleTransactionNo { get { return titleTransactionNo; } set { titleTransactionNo = value; } }
        public static string TitleAmount { get { return titleAmount; } set { titleAmount = value; } }
        public static string TitleAccountNo { get { return titleAccountNo; } set { titleAccountNo = value; } }
        public static string TitleAccountBalance { get { return titleAccountBalance; } set { titleAccountBalance = value; } }
        public static string TitleAvailableSpend { get { return titleAvailableSpend; } set { titleAvailableSpend = value; } }
        public static string TitleCustomerName { get { return titleCustomerName; } set { titleCustomerName = value; } }
        public static string TitleStoreAddress { get { return titleStoreAddress; } set { titleStoreAddress = value; } }
        public static string TitleBranchCode { get { return titleBranchCode; } set { titleBranchCode = value; } }
        public static string TitleTransactionDate { get { return titleTransactionDate; } set { titleTransactionDate = value; } }
        public static string TitleSalesPerson { get { return titleSalesPerson; } set { titleSalesPerson = value; } }
        public static string TitleCashierID { get { return titleCashierID; } set { titleCashierID = value; } }

        public static PrintDataWrapper<string> Footer { get { return footer; } set { footer = value; } }

        #endregion

        #region Constructors

        public PaymentReceipt()
            : base()
        {
            this.AccountBalance = new PrintDataWrapper<decimal?>(null, true);
            this.AccountNo = new PrintDataWrapper<string>(default(string), true);
            this.TransactionDate = new PrintDataWrapper<DateTime>(default(DateTime), true);
            this.CustomerName = new PrintDataWrapper<string>(default(string), true);
            this.BranchCode = new PrintDataWrapper<string>(default(string), true);
            this.CustomerTitle = new PrintDataWrapper<string>(default(string), true);
            this.Amount = new PrintDataWrapper<decimal?>(null, true);
            this.CustomerTitle = new PrintDataWrapper<string>(default(string), true);
            this.TransactionNo = new PrintDataWrapper<string>(default(string), true);
            this.Type = new PrintDataWrapper<string>(default(string), true);
            this.Image = new PrintDataWrapper<Image>(default(Image), true);
            this.SalesPerson = new PrintDataWrapper<string>(default(string), true);
            this.CashierID = new PrintDataWrapper<string>(default(string), true);
            this.BusinessTitle = new PrintDataWrapper<string>(default(string), true);
            this.BusinessTaxNo = new PrintDataWrapper<string>(default(string), true);
        }

        #endregion

        #region Public Propeties

        public override string PrintString
        {
            get { throw new NotImplementedException(); }
        }

        public PrintDataWrapper<string> CustomerTitle { get; set; }

        public PrintDataWrapper<string> CustomerName { get; set; }

        public PrintDataWrapper<string> AccountNo { get; set; }

        public PrintDataWrapper<decimal?> AccountBalance { get; set; }
        public PrintDataWrapper<string> BusinessTitle { get; set; }
        public PrintDataWrapper<string> BusinessTaxNo { get; set; }
        public PrintDataWrapper<DateTime> TransactionDate { get; set; }

        public PrintDataWrapper<string> TransactionNo { get; set; }

        public PrintDataWrapper<string> Type { get; set; }

        public PrintDataWrapper<decimal?> Amount { get; set; }

        public PrintDataWrapper<string> BranchCode { get; set; }

        public PrintDataWrapper<string> SalesPerson { get; set; }

        public PrintDataWrapper<string> CashierID { get; set; }

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
                TitleType, 
                TitleTransactionNo, 
                TitleAccountNo, 
                TitleAccountNo,
                TitleAccountBalance, 
                TitleAvailableSpend,
                TitleAmount,
                TitleCustomerName
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

            this.DrawPaymentHeader(e, paperWidthInPoints, ref y, header1Font, bodyFont);
            //DrawHeader(e, paperWidthInPoints, ref y, header1Font, bodyFont);

            if (CustomerName.ShouldBePrinted && !String.IsNullOrEmpty(CustomerName.Value))
            {
                e.Graphics.DrawString(TitleCustomerName, bodyFont, Brushes.Black, x, y);
                DrawAndWrapString(CustomerName.Value, e.Graphics, bodyFont, remainingSpace, lineOffset, topSectionContentX, ref y);
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
            if (TransactionDate.ShouldBePrinted && TransactionDate.Value != default(DateTime))
            {
                string dateString = "";

                switch (PaymentReceipt.DateTimeFormat)
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
            if (Amount.ShouldBePrinted && Amount.Value.HasValue)
            {
                e.Graphics.DrawString(TitleAmount, bodyFont, Brushes.Black, x, y);
                e.Graphics.DrawString(MoneyString(Amount.Value), bodyFont, Brushes.Black, topSectionContentX, y);

                NewLine(lineOffset * lineSpace2);
            }
            if (SalesPerson.ShouldBePrinted && !String.IsNullOrEmpty(SalesPerson.Value))
            {
                e.Graphics.DrawString(TitleSalesPerson, bodyFont, Brushes.Black, x, y);
                e.Graphics.DrawString(SalesPerson.Value, bodyFont, Brushes.Black, topSectionContentX, y);

                NewLine(lineOffset);
            }
            if (CashierID.ShouldBePrinted && !String.IsNullOrEmpty(CashierID.Value))
            {
                e.Graphics.DrawString(TitleCashierID, bodyFont, Brushes.Black, x, y);
                e.Graphics.DrawString(CashierID.Value, bodyFont, Brushes.Black, topSectionContentX, y);

                NewLine(lineOffset);
            }
            if (BranchCode.ShouldBePrinted && !String.IsNullOrEmpty(BranchCode.Value))
            {
                e.Graphics.DrawString(TitleBranchCode, bodyFont, Brushes.Black, x, y);
                e.Graphics.DrawString(BranchCode.Value, bodyFont, Brushes.Black, topSectionContentX, y);

                NewLine(lineOffset);
            }

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
