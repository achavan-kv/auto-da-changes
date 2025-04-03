using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Printing;
using System.Drawing;
using BBSL.Libraries.General;
namespace BBSL.Libraries.Printing.PrintDocuments
{
    public class DisbursementReceipt : PrintContent
    {
        #region Title Text

        static string titleCashier = "";
        static string titleDisbursementDate = "";
        static string titleDisbursement = "";
        static string titlePayMethod = "";
        static string titleValue = "";
        static string titleReference = "";
        static string titleSignatureText = "";

        public static string TitleCashier { get { return titleCashier; } set { titleCashier = value; } }
        public static string TitleDisbursementDate { get { return titleDisbursementDate; } set { titleDisbursementDate = value; } }
        public static string TitleDisbursement { get { return titleDisbursement; } set { titleDisbursement = value; } }
        public static string TitlePayMethod { get { return titlePayMethod; } set { titlePayMethod = value; } }
        public static string TitleValue { get { return titleValue; } set { titleValue = value; } }
        public static string TitleReference { get { return titleReference; } set { titleReference = value; } }
        public static string TitleSignatureText { get { return titleSignatureText; } set { titleSignatureText = value; } }
        
        #endregion

        #region Constructors

        public DisbursementReceipt()
            : base()
        {
            this.Footer = new PrintDataWrapper<string>(default(string), true);
            this.Cashier= new PrintDataWrapper<string>(default(string), true);
            this.DisbursementDate = new PrintDataWrapper<DateTime>(default(DateTime), true);
            this.Disbursements = new PrintDataWrapper<List<Disbursement>>(new List<Disbursement>(), true);
        }

        #endregion

        #region Public Propeties

        public override string PrintString
        {
            get { throw new NotImplementedException(); }
        }

        public PrintDataWrapper<string> Footer { get; set; }

        public PrintDataWrapper<string> Cashier { get; set; }

        public PrintDataWrapper<DateTime> DisbursementDate { get; set; }
        
        public PrintDataWrapper<List<Disbursement>> Disbursements { get; set; }

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
                    TitleCashier,
                    TitleDisbursementDate
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
               
                var remainingSpace = pintableAreaWidthInPoints - topSectionContentX;                        //IP - 18/05/12 - #10143
                
                if (Cashier.ShouldBePrinted && !String.IsNullOrEmpty(Cashier.Value))
                {
                    e.Graphics.DrawString(TitleCashier, bodyFont, Brushes.Black, x, y);
                    //e.Graphics.DrawString(Cashier.Value, bodyFont, Brushes.Black, topSectionContentX, y);
                    DrawAndWrapString(Cashier.Value, e.Graphics, bodyFont, remainingSpace, lineOffset, topSectionContentX, ref y);          //IP - 18/05/12 - #10143

                    NewLine(lineOffset, ref y);
                }

                if (DisbursementDate.ShouldBePrinted && DisbursementDate.Value != null)
                {
                    string dateString = "";

                    switch (Receipt.DateTimeFormat)
                    {
                        case DateTimeFormats.LongDate:
                            dateString = DisbursementDate.Value.ToLongDateString(); break;
                        case DateTimeFormats.LongTime:
                            dateString = DisbursementDate.Value.ToLongTimeString(); break;
                        case DateTimeFormats.LongDateAndTime:
                            dateString = DisbursementDate.Value.ToLongDateString() + " " + DisbursementDate.Value.ToLongTimeString(); break;
                        case DateTimeFormats.LongDateAndShortTime:
                            dateString = DisbursementDate.Value.ToLongDateString() + " " + DisbursementDate.Value.ToShortTimeString(); break;
                        case DateTimeFormats.ShortDate:
                            dateString = DisbursementDate.Value.ToShortDateString(); break;
                        case DateTimeFormats.ShortTime:
                            dateString = DisbursementDate.Value.ToShortTimeString(); break;
                        case DateTimeFormats.ShortDateAndTime:
                            dateString = DisbursementDate.Value.ToShortDateString() + " " + DisbursementDate.Value.ToShortTimeString(); break;
                        case DateTimeFormats.ShortDateAndLongTime:
                            dateString = DisbursementDate.Value.ToShortDateString() + " " + DisbursementDate.Value.ToLongTimeString(); break;
                    }

                    e.Graphics.DrawString(TitleDisbursementDate, bodyFont, Brushes.Black, x, y);
                    e.Graphics.DrawString(dateString, bodyFont, Brushes.Black, topSectionContentX, y);

                    NewLine(lineOffset, ref y);
                }

            }//*** END TOP SECTION ****
            
            {//***** DISBURSEMENTS SECTION ******
                if (Disbursements.ShouldBePrinted && Disbursements.Value != null)
                {
                    
                                string[] titles = 
                                { 
                                    TitleDisbursement, 
                                    TitlePayMethod, 
                                    TitleValue,
                                    TitleReference
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
                                NewLine(lineOffset, ref y);

                                e.Graphics.DrawString("Disbursements", header2Font, Brushes.Black, x, y);
                                NewLine(lineOffset, ref y);
                                DrawLine(e, pintableAreaWidthInPoints, y, bodyFont);

                                foreach (var disbursement in Disbursements.Value)
                                {
                                    NewLine(lineOffset, ref y);

                                    e.Graphics.DrawString(TitleDisbursement, bodyFont, Brushes.Black, x, y);
                                    e.Graphics.DrawString(disbursement.DisbursementType, bodyFont, Brushes.Black, contentX, y);
                                    NewLine(lineOffset, ref y);

                                    e.Graphics.DrawString(TitlePayMethod, bodyFont, Brushes.Black, x, y);
                                    e.Graphics.DrawString(disbursement.PayMethod, bodyFont, Brushes.Black, contentX, y);
                                    NewLine(lineOffset, ref y);

                                    e.Graphics.DrawString(TitleValue, bodyFont, Brushes.Black, x, y);
                                    e.Graphics.DrawString(MoneyString(disbursement.Value), bodyFont, Brushes.Black, contentX, y);
                                    NewLine(lineOffset, ref y);

                                    e.Graphics.DrawString(TitleReference, bodyFont, Brushes.Black, x, y);
                                    e.Graphics.DrawString(disbursement.Reference, bodyFont, Brushes.Black, contentX, y);

                                    //Display Signature line

                                    for (int i = 0; i < 10; i++)
                                    {
                                        NewLine(lineOffset, ref y);
                                    }

                                    x = GetRightXCoordinate(e.Graphics, pintableAreaWidthInPoints, TitleSignatureText, bodyFont);
                                    e.Graphics.DrawString(TitleSignatureText, bodyFont, Brushes.Black, x, y);
                                    x = 0;

                                    NewLine(lineOffset, ref y);

                                    string divider = CreateFullLine(DividerCharacter, e, bodyFont, pintableAreaWidthInPoints);
                                    e.Graphics.DrawString(divider, bodyFont, Brushes.Black, x, y);

                                }
                           
                }
            }//*** END DISBURSEMENTS SECTION ****

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
    }
    
}
