using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using Blue.Cosacs.Sales.Models;

namespace Blue.Cosacs.Sales.Common
{
    /// <summary>
    /// For the complete list of commands: 
    /// TM-T70: https://www.epson-biz.com/modules/ref_escpos/index.php?content_id=80  and
    /// TM-T88V: https://www.epson-biz.com/modules/ref_escpos/index.php?content_id=83
    /// </summary>
    public class ThermalPrinter
    {
        private char esc = (char)27; // "\u001b";
        private char gs = (char)29; // "\u001d";
        private char tab = (char)9;
        private int dividerWidth = 56;

        struct Justification
        {
            public static string Left
            {
                get
                {
                    return (char)27 + "a" + 0;
                }
            }
            public static string Centered
            {
                get
                {
                    return (char)27 + "a" + (char)1;
                }
            }
            public static string Right
            {
                get
                {
                    return (char)27 + "a" + 2;
                }
            }
        }

        private string SetFont(int param)
        {
            return esc + "M" + param;
        }

        private string SetCharacterSize(int param)
        {
            return gs + "!" + (char)param;
        }

        private string Bold(bool isOn)
        {
            var flag = isOn ? "1" : "0";
            return esc + "E" + flag;
        }

        private string Line(int length, char shape = '.')
        {
            return string.Empty.PadLeft(length, shape);
        }

        private string Clear
        {
            get { return esc + "@"; }
        }

        private string Cut
        {
            get { return gs + "V"; }
        }

        private string ExecuteEsc(string param, int arg)
        {
            return esc + param + arg;
        }

        private string SetTabs(int[] tabWidths)
        {
            var widthsStr = new StringBuilder();
            var comma = "";

            foreach (var tabWidth in tabWidths)
            {
                widthsStr.AppendFormat("{0}{1}", comma, (char)tabWidth);
                comma = " ";
            }
            return esc + "D" + widthsStr.ToString().Trim() + (char)0;
        }

        private string SetMotionUnit()
        {
            return gs + "P" + (char)180 + "" + (char)180;
        }

        private string CancelTabs()
        {
            return esc + "D" + (char)0;
        }

        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        private string GetBarcode(string barcode)
        {
            return string.Format("{0}k{1}{2}{3}", gs, (char)4, barcode, (char)0);
        }

        string PaperCut
        {
            get { return string.Format("\n\n\n\n\n{0}V0", gs); }
        }

        string Drawer
        {

            get { return Clear + esc + "p" + (char)0 + ""; }
        }

        string BarCodeWidth(int length)
        {
            return string.Format("{0}w{1}", gs, (char)length);
        }

        string BarCodeHeight(int length)
        {
            return string.Format("{0}h{1}", gs, (char)length);
        }

        string PrintBarCode(string barcode)
        {
            barcode = string.Format("IN{0}{1}", @"$", barcode);

            return string.Format("{0}{1}{2}", BarCodeHeight(81), BarCodeWidth(2), GetBarcode(barcode));
        }

        public string GetThermalReciept(IEnumerable<OrderExtendedDto> models)
        {
            var ret = new StringBuilder();

            foreach (var order in models)
            {
                // Initialize printer
                ret.Append(Clear);
                ret.Append(SetFont(1));
                // Select justification: Centering
                ret.Append(Justification.Centered);

                ret.AppendFormat("\r\n{0} {2}\r\n {1}",
                    this.Bold(true), this.Bold(false), order.CountryName);

                ret.AppendFormat("\r\n{0} {1}\r\n {2}",
                    this.Bold(true), order.BranchName, this.Bold(false));

                ret.AppendFormat("{0}\r\n", order.BranchAddress1);
                ret.AppendFormat("{0}\r\n", order.BranchAddress2);
                ret.AppendFormat("{0}\r\n", order.BranchAddress3);
                ret.AppendFormat("{0}\r\n", order.CompanyTaxNumber);
                ret.AppendFormat("{0}\r\n", PrintBarCode(order.Id.ToString()));

                ret.Append(Justification.Left);
                ret.Append(SetTabs(new int[] { 16, 36 }));

                var receiptType = string.IsNullOrEmpty(order.ReceiptType) ? string.Empty : order.ReceiptType.Trim() + " ";
                ret.AppendFormat("{0}Cash & Go Receipt / {1}\r\n", receiptType, order.PrintCopy);
                ret.AppendFormat("{0}\r\n", Line(dividerWidth));
                // Set Tabs
                ret.Append("Tax Invoice:");
                ret.AppendFormat("{0}{1}\r\n", tab, string.Format("{0} / {1}", order.BranchNo, order.Id));

                ret.Append("Sales Person:");
                ret.AppendFormat("{0}{1}\r\n", tab, order.CreatedBy);

                ret.Append("Cashier:");
                ret.AppendFormat("{0}{1}\r\n", tab, order.CurrentUser);

                ret.Append("Date Printed:");
                ret.AppendFormat("{0}{1}\r\n", tab, string.Format("{0:dd/MM/yyyy HH:mm tt}", order.CreatedOn));

                if (!string.IsNullOrEmpty(order.ReceiptType))
                {
                    ret.Append("Date Re-Printed:");
                    ret.AppendFormat("{0}{1}\r\n", tab, string.Format("{0:dd/MM/yyyy HH:mm tt}", DateTime.Now));
                }

                if (order.Customer != null)
                {
                    ret.Append("Customer Name:");
                    ret.AppendFormat("{0}{1}\r\n", tab, string.Format("{0} {1} {2}", order.Customer.Title, order.Customer.FirstName, order.Customer.LastName));

                    ret.Append("Customer Address:");
                    ret.AppendFormat("{0}{1}\r\n", tab, order.Customer.AddressLine1);

                    ret.Append(tab);
                    ret.AppendFormat("{0}\r\n", order.Customer.AddressLine2);

                    ret.Append(tab);
                    ret.AppendFormat("{0}\r\n", string.Format("{0} {1}", order.Customer.TownOrCity, order.Customer.PostCode));
                }

                ret.AppendFormat("{0}\r\n", Line(dividerWidth));

                foreach (var item in order.Items)
                {
                    var itemDetails = new ItemPrintDetails(order, item);

                    var description = string.IsNullOrEmpty(item.PosDescription) ? item.Description : item.PosDescription;
                    ret.Append(Justification.Left);
                    ret.AppendFormat("{0}({1}){2}\r\n", item.Quantity, item.ItemNo, description);

                    ret.Append("Amt");
                    ret.Append(Justification.Right);
                    ret.AppendFormat("{0}{1}", tab, order.TaxName);
                    ret.AppendFormat("{0}Sub Total\r\n", tab);
                    ret.Append(Justification.Left);

                    ret.Append(itemDetails.GetItemAmount());
                    ret.Append(Justification.Right);
                    ret.AppendFormat("{0}{1}", tab, order.IsTaxFreeSale ? ItemPrintDetails.GetCurrencyString(0) : itemDetails.GetItemAmount(false));
                    ret.AppendFormat("{0}{1}\r\n", tab, itemDetails.GetItemTotal());

                }
                ret.Append(Justification.Left);
                ret.Append("\r\nTotal Amt");
                ret.Append(Justification.Right);
                ret.AppendFormat("{0}{1}", tab, order.TaxName);
                ret.AppendFormat("{0} Invoice Total\r\n", tab);

                ret.Append(ItemPrintDetails.GetCurrencyString(order.TotalAmount - order.TotalTaxAmount));
                ret.AppendFormat("{0}{1}", tab, ItemPrintDetails.GetCurrencyString(order.TotalTaxAmount));
                ret.AppendFormat("{0}{1}\r\n", tab, ItemPrintDetails.GetCurrencyString(order.TotalAmount));

                ret.Append(CancelTabs());

                ret.Append(SetTabs(new int[] { 15, 60 }));

                ret.AppendFormat("{0}{0}{1}\r\n", tab, Line(24));

                foreach (var payment in order.Payments)
                {
                    if ((order.ChangeGiven && payment.Amount > 0) || (!order.ChangeGiven))
                    {
                        ret.AppendFormat("{0}{1}", tab, payment.PaymentMethod);
                        ret.Append(Justification.Right);
                        ret.AppendFormat("{0}{1}\r\n", tab, ItemPrintDetails.GetCurrencyString(payment.Amount));
                    }

                }

                ret.Append(Justification.Left);

                if (order.PositiveAmountSum >= (order.NegativeAmountSum * -1))
                {
                    ret.AppendFormat("{0}Amount Tendered", tab);
                }
                else if (!order.ChangeGiven && order.NegativeAmountSum < 0)
                {
                    ret.AppendFormat("{0} Amount Returned", tab);
                }

                if (order.PositiveAmountSum >= (order.NegativeAmountSum * -1))
                {
                    ret.AppendFormat("{0}{1}\r\n", tab, ItemPrintDetails.GetCurrencyString(order.PositiveAmountSum));
                }
                else if (!order.ChangeGiven && order.NegativeAmountSum < 0)
                {
                    ret.AppendFormat("{0}{1}\r\n", tab, ItemPrintDetails.GetCurrencyString(order.NegativeAmountSum));
                }

                if (order.ChangeGiven)
                {
                    ret.AppendFormat("{0}{0}{1}\r\n", tab, Line(24));
                    ret.AppendFormat("{0}Change Given", tab);
                    ret.AppendFormat("{0}{1}\r\n", tab, ItemPrintDetails.GetCurrencyString(order.NegativeAmountSum * -1));
                }

                ret.AppendFormat("{0}\r\n\r\n\r\n", Line(dividerWidth));
                ret.Append("Signature\r\n");

                ret.AppendFormat("{0}\r\n", Line(dividerWidth));
                //ret.Append(Justification.Centered);
                //ret.Append("Thanks for shopping\r\n");
                ret.Append(Justification.Centered);
                ret.AppendFormat("SEE WARRANTY DETAILS ATTACHED\r\n{0}{1}{2}", PaperCut, Drawer, Clear);
            }

            return ret.ToString();
        }
    }
}