using System.Globalization;
using Blue.Cosacs.Sales.Models;

namespace Blue.Cosacs.Sales.Common
{
    public class ItemPrintDetails
    {
        private readonly ItemDto item;
        private readonly decimal taxRate;
        private readonly bool isTaxFreeSale;

        public ItemPrintDetails(OrderExtendedDto order, ItemDto item)
        {
            this.item = item;
            taxRate = item.TaxRate <= 0 ? order.TaxRate : item.TaxRate;
            isTaxFreeSale = order.IsTaxFreeSale;
        }

        public string GetItemAmount(bool isPrice = true)
        {
            var amount = item.ItemTypeId == (int)ItemTypeEnum.Discount
                ? item.ManualDiscount
                : item.Price;
            var totalAmount = amount * item.Quantity;

            if (isPrice)
            {
                return GetCurrencyString(totalAmount);
            }

            if (isTaxFreeSale)
            {
                return GetCurrencyString(0);
            }

            if (item.ItemTypeId != (int)ItemTypeEnum.Discount)
            {
                return GetCurrencyString(item.TaxAmount != null ? item.TaxAmount * item.Quantity : 0);
            }

            var discount = item.ManualDiscount * item.Quantity;
            var discountTax = discount * taxRate * 0.01M;

            return GetCurrencyString(discountTax);
        }

        public string GetItemTotal()
        {
            var ret = (item.Price + item.TaxAmount) * item.Quantity;

            if (item.ItemTypeId != (int) ItemTypeEnum.Discount)
            {
                return @GetCurrencyString(ret);
            }

            var discount = item.ManualDiscount * item.Quantity;
            var discountTax = isTaxFreeSale ? 0 : discount * taxRate * 0.01M;

            ret = discount + discountTax;

            return @GetCurrencyString(ret);
        }

        public static string GetCurrencyString(decimal? val)
        {
            var ret = val ?? 0;
            var settings = new Settings();
            var localFormat = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
            var decimalplaces = settings.DecimalPlaces;
            localFormat.CurrencySymbol = settings.CurrencySymbolForPrint;

            return ret.ToString(decimalplaces, localFormat);
        }
    }
}
