using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Blue.Cosacs.Sales.Models
{
    public class IndexedProducts
    {
        public string Id { get; set; }
        public int ProductId { get; set; }
        public string Sku { get; set; }
        public string ProductType { get; set; }
        public string ProductStatus { get; set; }
        public string RepossessedCondition { get; set; }
        public string Type { get; set; }
        public string LongDescription { get; set; }
        public string PosDescription { get; set; }
        public IEnumerable<string> PriceData { get; set; }
        public IEnumerable<string> PromoData { get; set; }
        public string CreatedOn { get; set; }
        public int? StockAvailable { get; set; }
        public int? StockOnHand { get; set; }
        public int? StockOnOrder { get; set; }
        public int? StockAllocated { get; set; }
        public bool LabelRequired { get; set; }
        public int BranchesWithStock { get; set; }

        public IList<string> Tags { get; set; }
        public IList<string> StoreTypes { get; set; }
        public IList<string> Vendors { get; set; }
        public int SalesThisPeriod { get; set; }
        public int SalesLastPeriod { get; set; }
        public int SalesThisYTD { get; set; }
        public int SalesLastYTD { get; set; }
        public string HierarchyTags { get; set; }

        public string[] MerchandisingLevel_1 { get; set; }
        public string[] MerchandisingLevel_2 { get; set; }
        public string[] MerchandisingLevel_3 { get; set; }

        public string Description
        {
            get
            {
                if ("NonStock".Equals(Type))
                {
                    return Description1 + Description2;
                }

                return LongDescription;
            }
        }
        public IEnumerable<PriceData> PriceDataObject
        {
            get
            {
                var result = new List<PriceData>();

                if (PriceData != null)
                {
                    foreach (var priceData in PriceData)
                    {
                        result.Add(JsonConvert.DeserializeObject<PriceData>(priceData));
                    }
                }
 //Change for ZEN/UNC/CRF/CR2018-011 Pricing Promotion - Happy Hour
                return result.Where(p => p.EffectiveDate.Date <= DateTime.Now.Date);
            }
        }
        public IEnumerable<PriceData> PromoDataObject
        {
            get
            {
                var result = new List<PriceData>();

                if (PromoData != null)
                {
                    JsonSerializerSettings settings = new JsonSerializerSettings();
                    settings.Converters.Add(new DecimalJsonConverter());
                    settings.Formatting = Formatting.Indented;

                    foreach (var priceData in PromoData)
                    {
                        result.Add(JsonConvert.DeserializeObject<PriceData>(priceData, settings));
                    }
                }
                TimeSpan currentTime = TimeSpan.Parse(DateTime.Now.ToString("HH:mm:ss"));
                return result.Where(p =>    p.EffectiveDate.Date <= DateTime.Now.Date 
                                            && p.EndDate.Date >= DateTime.Now.Date
                                            && TimeSpan.Parse(p.EffectiveDate.ToString("HH:mm")) <= currentTime
                                            && TimeSpan.Parse(p.EndDate.ToString("HH:mm")) >= currentTime);
            }
        }

        public string Level_1
        {
            get
            {
                if (Type == "MerchandiseStockSummary" && MerchandisingLevel_1 != null)
                {
                    return MerchandisingLevel_1[0];
                }
                else if (Type == "NonStock")
                {
                    return Division;
                }
                return string.Empty;
            }
        }
        public string Level_2
        {
            get
            {
                if (Type == "MerchandiseStockSummary" && MerchandisingLevel_2 != null)
                {
                    return MerchandisingLevel_2[0];
                }
                else if (Type == "NonStock")
                {
                    return Department;
                }
                return string.Empty;
            }
        }
        public string Level_3
        {
            get
            {
                if (Type == "MerchandiseStockSummary" && MerchandisingLevel_3 != null)
                {
                    return MerchandisingLevel_3[0];
                }
                else if (Type == "NonStock")
                {
                    return Class;
                }
                return string.Empty;
            }
        }

        public string Division { get; set; }
        public string Department { get; set; }
        public string Class { get; set; }
        public string Description1 { get; set; }
        public string Description2 { get; set; }
    }
}
