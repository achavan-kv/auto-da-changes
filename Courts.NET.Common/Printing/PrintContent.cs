using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Printing;
using System.Drawing;
using System.Threading;
using System.Globalization;

namespace BBSL.Libraries.Printing.PrintDocuments
{
    public abstract class PrintContent : PrintDocument, IDisposable
    {
        #region Static

        #region Private/Protected Static

        protected readonly Font defaultHeader1Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold, GraphicsUnit.Point);
        protected readonly Font defaultHeader2Font = new Font("Microsoft Sans Serif", 8.5F, FontStyle.Bold, GraphicsUnit.Point);
        protected readonly Font defaultHeader3Font = new Font("Microsoft Serif", 9F, FontStyle.Regular, GraphicsUnit.Point);
        protected readonly Font defaultBodyFont = new Font("Microsoft Sans Serif", 8.5F, FontStyle.Regular, GraphicsUnit.Point);
        protected readonly Font smallPrintFont = new Font("Microsoft Sans Serif", 7.0F, FontStyle.Regular, GraphicsUnit.Point); //IP - 18/01/11
        protected const string lineString = "------------------";
        protected static float widthCorrection = 100;
        protected const float pointsPerMillimeter = 2.83464567F;
        protected const float pointsPerInch = 72F;
        protected const float lineStringSpacing = 0.8F;
        protected const float doubleLineStringSpacing = 0.25F;
        protected const int bottomSectionTitleOffset = 15;
        protected static float lineSpace1 = 1.5F;
        protected static float lineSpace2 = 2F;
        protected static int minimumHeight = 0;
        protected static int decimalPlaces = 2;
        protected static string moneyStringFormat = "C";
        protected static string currencySymbol = "";
        protected static string currencyFormat = "";
        protected static DateTimeFormats dateTimeFormat = DateTimeFormats.ShortDate;
        protected static char dividerCharacter = '-';

        static void CreateMoneyStringFormat()
        {
            StringBuilder stringBuilder;

            if (!String.IsNullOrEmpty(currencyFormat))
            {
                moneyStringFormat = currencyFormat;
                return;
            }
            else if (String.IsNullOrEmpty(currencySymbol))
            {
                stringBuilder = new StringBuilder("C");

                if (decimalPlaces > 0)
                {
                    stringBuilder.Append(decimalPlaces);
                }
            }
            else
            {
                stringBuilder = new StringBuilder(currencySymbol + "0");

                if (decimalPlaces > 0)
                {
                    stringBuilder.Append(".");
                    for (int i = 0; i < decimalPlaces; i++)
                        stringBuilder.Append("0");
                }
            }

            moneyStringFormat = stringBuilder.ToString();
        }

        public static float GetCenterXCoordinate(Graphics graphic, float paperWidthInPoints, string str, Font font)
        {
            graphic.PageUnit = GraphicsUnit.Point;
            return (paperWidthInPoints - graphic.MeasureString(str, font).Width) / 2;
        }
        public static float GetRightXCoordinate(Graphics graphic, float paperWidthInPoints, string str, Font font)
        {
            graphic.PageUnit = GraphicsUnit.Point;
            return (paperWidthInPoints - graphic.MeasureString(str, font).Width);
        }
        public static List<float> GetXCoordinatesForNColumns(float paperWidthInPoints, int columns, out float columnWidth)
        {
            float printableAreaWidth = paperWidthInPoints;//GetPrintableAreaWidth(printPageEventArgs);
            columnWidth = printableAreaWidth / columns;
            List<float> columnsList = new List<float>();
            for (float x = 0, i = 0; i < columns; i++, x = x + columnWidth)
                columnsList.Add(x);
            return columnsList;
        }
        public static float GetPrintableAreaWidth(PrintPageEventArgs printPageEventArgs)
        {
            //return 226.771654F;//80mm in points
            return (printPageEventArgs.PageSettings.PrintableArea.Width / 100F) * pointsPerInch;
        }

        /// <summary>
        /// Splits a string up into lines, based on the width of the paper
        /// </summary>
        /// <param name="str"></param>
        /// <param name="printPageEventArgs"></param>
        /// <param name="font"></param>
        /// <returns></returns>
        public static List<string> Split(string str, PrintPageEventArgs printPageEventArgs, Font font)
        {
            Graphics graphics = printPageEventArgs.Graphics;
            
            float printableAreaWidth = GetPrintableAreaWidth(printPageEventArgs);
            printableAreaWidth = printableAreaWidth - (printableAreaWidth * 0.02F);

            return Split(str, graphics, printableAreaWidth, font);
        }

        /// <summary>
        /// Splits a string up into lines, based on the printableAreaWidth
        /// </summary>
        /// <param name="str"></param>
        /// <param name="graphics"></param>
        /// <param name="printableAreaWidth"></param>
        /// <param name="font"></param>
        /// <returns></returns>
        public static List<string> Split(string str, Graphics graphics, float printableAreaWidth, Font font)
        {
            List<string> splitString = new List<string>();

            bool stringNotEntirelyWrapped = false;

            //Checks to see if the entire string has been wrapped
            //and we can finish wrapping
            Action<List<string>> checkString =
                delegate(List<string> SplitString)
                {
                    string lengthTestString = "";
                    foreach (string s in SplitString)
                        lengthTestString += s;

                    stringNotEntirelyWrapped = (lengthTestString.Length + SplitString.Count) < str.Length;
                };

            string tmp = str;

            do
            {
                while (graphics.MeasureString(tmp, font).Width > printableAreaWidth && tmp.LastIndexOf(' ') > -1)
                {
                    tmp = tmp.Substring(0, tmp.LastIndexOf(' '));
                }
                splitString.Add(tmp);

                int length = 0;
                foreach (string s in splitString)
                    length += s.Length + 1;

                if (length <= str.Length)
                    tmp = str.Substring(length).TrimStart();
                else
                    tmp = "";

                checkString(splitString);

            } while (stringNotEntirelyWrapped);


            return splitString;
        }
        
        /// <summary>
        /// Creates a line of characters, the width of the page
        /// </summary>
        /// <param name="character"></param>
        /// <param name="printPageEventArgs"></param>
        /// <param name="font"></param>
        /// <returns></returns>
        public static string CreateFullLine(char character, PrintPageEventArgs printPageEventArgs, Font font, float paperWidthInPoints)
        {
            Graphics graphics = printPageEventArgs.Graphics;

            string str = character.ToString();

            while (graphics.MeasureString(str, font).Width < paperWidthInPoints)
                str += character.ToString();

            return str;
        }

        /// <summary>
        /// Increases <paramref name="y"/> by <paramref name="offset"/>
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="y"></param>
        public void NewLine(float offset, ref float y)
        {
            y += offset;
        }

        #endregion

        #region Public Static

        /// <summary>
        /// The culture string for the application. If this is not set, the
        /// system culture default will be used.
        /// </summary>
        public static string Culture { get; set; }
        /// <summary>
        /// The windows printer name
        /// </summary>
        public static string PrinterName { get; set; }

        /// <summary>
        /// Minimum Receipt Height in millimeters
        /// </summary>
        public static int MinimumHeight
        { get { return minimumHeight; } set { minimumHeight = value; } }
        public static DateTimeFormats DateTimeFormat { get { return dateTimeFormat; } set { dateTimeFormat = value; } }
        /// <summary>
        /// This is the master currency format (e.g. C2, C3, F2...).
        /// If this is set (!= null) then CurrencySymbol and DecimalPlaces
        /// will be ignored
        /// </summary>
        public static string CurrencyFormat
        {
            get { return currencyFormat; }
            set
            {
                currencyFormat = value;
                CreateMoneyStringFormat();
            }
        }
        /// <summary>
        /// This defines a prefix for currency strings.
        /// If this is not set, the system culture default will be used
        /// for currency (C2). 
        /// This will be ignored if CurrencyFormat != null
        /// </summary>
        public static string CurrencySymbol
        {
            get { return currencySymbol; }
            set
            {
                currencySymbol = value;
                CreateMoneyStringFormat();
            }
        }
        /// <summary>
        /// This defines the number of decimal places to use for currency strings.
        /// This will be ignored if CurrencyFormat != null
        /// </summary>
        public static int DecimalPlaces
        {
            get { return decimalPlaces; }
            set
            {
                decimalPlaces = value;
                CreateMoneyStringFormat();
            }
        }
        public static float LineSpace1 { get { return lineSpace1; } set { lineSpace1 = value; } }
        public static float LineSpace2 { get { return lineSpace2; } set { lineSpace2 = value; } }
        /// <summary>
        /// The character to use for separator lines (e.g. -------------).
        /// The deafualt is '-'
        /// </summary>
        public static char DividerCharacter { get { return dividerCharacter; } set { dividerCharacter = value; } }

        public static PrintDataWrapper<string> BusinessTitle { get; private set; }
        public static PrintDataWrapper<string> BusinessRegNo { get; private set; }
        public static PrintDataWrapper<string> BusinessTaxNo { get; private set; }
        public static PrintDataWrapper<string> BusinessTaxNoLabel { get; private set; }
        public static PrintDataWrapper<string> BusinessRegNoLabel { get; private set; }
        public static PrintDataWrapper<string> BusinessOtherInfo { get; private set; }
        public static PrintDataWrapper<string> Store { get; private set; }
        public static PrintDataWrapper<string> StoreAddress1 { get; private set; }
        public static PrintDataWrapper<string> StoreAddress2 { get; private set; }
        public static PrintDataWrapper<string> StoreAddress3 { get; private set; }

        #endregion

        #endregion

        #region Constructors

        public PrintContent()
        {
            _PrintPageEventHandler = new PrintPageEventHandler(PrintContent_PrintPage);
            _EndPrintEventHandler = new PrintEventHandler(PrintContent_EndPrint);
            this.PrintPage += _PrintPageEventHandler;
            this.EndPrint += _EndPrintEventHandler;
            if (PrinterName != null)
                this.PrinterSettings.PrinterName = PrinterName;

            if(Culture != null)
                Thread.CurrentThread.CurrentCulture = new CultureInfo(Culture);

            AdditionalText = new PrintDataWrapper<string>(default(string), true);
            Title = new PrintDataWrapper<string>(default(string), true);
            this.Image = new PrintDataWrapper<Image>(default(Image), true);
        }

        static PrintContent()
        {
            Store = new PrintDataWrapper<string>(default(string), true);
            StoreAddress1 = new PrintDataWrapper<string>(default(string), true);
            StoreAddress2 = new PrintDataWrapper<string>(default(string), true);
            StoreAddress3 = new PrintDataWrapper<string>(default(string), true);
            BusinessTitle = new PrintDataWrapper<string>(default(string), true);
            BusinessRegNo = new PrintDataWrapper<string>(default(string), true);
            BusinessOtherInfo = new PrintDataWrapper<string>(default(string), true);
            BusinessTaxNo = new PrintDataWrapper<string>(default(string), true);
            BusinessRegNoLabel = new PrintDataWrapper<string>(default(string), true);
            BusinessTaxNoLabel = new PrintDataWrapper<string>(default(string), true);
        }

        #endregion

        #region Public Mehtods

        public virtual new void Print()
        {
            try
            {
                if (PrePrintAction != null)
                    PrePrintAction(this);

                base.Print();
            }
            catch (InvalidPrinterException ex)
            {
                OnInvalidPrinterException(ex);
            }
        }

        public string MoneyString(object str)
        {
            return String.Format("{0, 0:" + moneyStringFormat + "}", str);
        }
        public string MoneyStringWithoutCurrencySymbol(object str)
        {
            return String.Format("{0, 0:" + moneyStringFormat.ToUpper().Replace("C", "F") + "}", str);
        }

        public void DrawHeader(PrintPageEventArgs e, float paperWidthInPoints, ref float y, Font header1Font, Font bodyFont)
        {
            float  lineOffset = bodyFont.GetHeight(e.Graphics) - 0.5F;

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

                if (BusinessTitle.ShouldBePrinted && !String.IsNullOrEmpty(BusinessTitle.Value))
                {
                    headerX = GetCenterXCoordinate(e.Graphics, paperWidthInPoints, BusinessTitle.Value, header1Font);
                    e.Graphics.DrawString(BusinessTitle.Value, header1Font, Brushes.Black, headerX, y);

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
                if (BusinessTaxNo.ShouldBePrinted && !String.IsNullOrEmpty(BusinessTaxNo.Value))
                {
                    if (BusinessTaxNoLabel.ShouldBePrinted)
                    {
                        headerX = GetCenterXCoordinate(e.Graphics, paperWidthInPoints, BusinessTaxNoLabel.Value + " " + BusinessTaxNo.Value, bodyFont);
                        e.Graphics.DrawString(BusinessTaxNoLabel.Value + " " + BusinessTaxNo.Value, bodyFont, Brushes.Black, headerX, y);
                    }
                    else
                    {
                        headerX = GetCenterXCoordinate(e.Graphics, paperWidthInPoints, BusinessTaxNo.Value, bodyFont);
                        e.Graphics.DrawString(BusinessTaxNo.Value, bodyFont, Brushes.Black, headerX, y);
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

        public void DrawLine(PrintPageEventArgs e, float pintableAreaWidthInPoints, float y, Font font)
        {
            string divider = CreateFullLine(DividerCharacter, e, font, pintableAreaWidthInPoints);
            float x = GetCenterXCoordinate(e.Graphics, pintableAreaWidthInPoints, divider, font);
            e.Graphics.DrawString(divider, font, Brushes.Black, x, y);
        }

        public void DrawAndWrapString(string str, Graphics graphics, Font bodyFont, float remainingSpace, float lineOffset, float topSectionContentX, ref float y)
        {
            if (graphics.MeasureString(str, bodyFont).Width > remainingSpace)
            {// Draw string over multiple lines

                var strings = Split(str, graphics, remainingSpace, bodyFont);
                foreach (var strval in strings)
                {
                    graphics.DrawString(strval, bodyFont, Brushes.Black, topSectionContentX, y);
                    NewLine(lineOffset, ref y);
                }
            }
            else
            {
                graphics.DrawString(str, bodyFont, Brushes.Black, topSectionContentX, y);
                NewLine(lineOffset, ref y);
            }
        }

        #endregion

        #region Public Properties

        public abstract string PrintString { get; }

        public bool HasPrinted { get; set; }

        public Font Header1Font { get; set; }
        public Font Header2Font { get; set; }
        public Font Header3Font { get; set; }
        public Font BodyFont { get; set; }
        public PrintDataWrapper<string> AdditionalText { get; set; }
        public PrintDataWrapper<string> Title { get; set; }
        public PrintDataWrapper<Image> Image { get; set; }

        /// <summary>
        /// This can be used to set the 'ShouldBePrinted' property on
        /// the PrintDataWrapper properties.
        /// </summary>
        public Action<PrintContent> PrePrintAction { get; set; }

        #endregion

        #region Events

        public static event Action<InvalidPrinterException> InvalidPrinterExceptionEvent;

        protected virtual void OnInvalidPrinterException(InvalidPrinterException e)
        {
            Action<InvalidPrinterException> handler = PrintContent.InvalidPrinterExceptionEvent;
            if (handler != null)
                handler(e);
        }

        #endregion

        #region Event Handlers

        PrintPageEventHandler _PrintPageEventHandler;
        PrintEventHandler _EndPrintEventHandler;

        public abstract void PrintContent_PrintPage(object sender, PrintPageEventArgs e);

        public virtual void PrintContent_EndPrint(object sender, PrintEventArgs e)
        {
            HasPrinted = true;
            //this.Dispose();
        }

        #endregion

        #region IDisposable Members

        public new void Dispose()
        {
            base.Dispose(true);
            this.PrintPage -= _PrintPageEventHandler;
            this.EndPrint -= _EndPrintEventHandler;
        }

        #endregion
    }
}
