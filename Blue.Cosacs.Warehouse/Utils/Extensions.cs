using System;
using System.Collections.Generic;
using System.Web;
using Blue.Cosacs.Warehouse.Repositories;

namespace Blue.Cosacs.Warehouse.Utils
{
	public static class Extensions
	{
        public static string ToSolrDate(this DateTime date)
        {
            return date.ToString("yyyy-MM-ddTHH:mm:ssZ");
        }

		public static string GetWarehouseZone(this PickListView pickListView)
		{
			var zoneRepository = new ZoneRepository();
			var zoneMapper = zoneRepository.GetMapper(pickListView.DeliveryBranch);
			var warehouseZone =
				zoneMapper.Map(new Dictionary<string, string> 
									{ 
										{ "ItemId", pickListView.ItemId.ToString() }, 
										{ "ItemNo", pickListView.ItemNo },
										{ "ItemUPC", pickListView.ItemUPC }, 
										{ "ProductArea", pickListView.ProductArea },
										{ "ProductBrand", pickListView.ProductBrand }, 
										{ "Category", pickListView.ProductCategory }, 
									});

			return warehouseZone.HasValue ? warehouseZone.ToString() : "Unknown";
		}

		public static string HtmlEncode(this string text)
		{
			return HttpUtility.HtmlEncode(text);
		}

        public static string HtmlDecode(this string text)
        {
            return HttpUtility.HtmlDecode(text);
        }

        public static string ToCurrency(this decimal amount)
        {
            return amount.ToString("N" + new Config.Settings().DecimalPlaces);
        }

        public static string ToCurrencyWithSymbol(this decimal amount)
        {
            return new Config.Settings().CurrencySymbol + ToCurrency(amount);
        }
    }
}

