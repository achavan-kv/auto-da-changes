using System;
using System.Collections.Generic;

namespace Blue.Cosacs.NonStocks.Models
{
    public class Link
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime EffectiveDate { get; set; }
        public List<Models.LinkProduct> linkProducts { get; set; }
        public List<Models.LinkNonStock> linkNonStocks { get; set; }

        public static Models.Link ToModel(NonStocks.Link link,
            List<Models.LinkProduct> linkProducts,
            List<Models.LinkNonStock> linkNonStocks)
        {
            var retVal = new Models.Link()
            {
                Id = link.Id,
                Name = link.Name,
                EffectiveDate = link.EffectiveDate,
                linkProducts = linkProducts,
                linkNonStocks = linkNonStocks,
            };
            return retVal;
        }

        public static NonStocks.Link ToEntity(Models.Link link)
        {
            var retVal = new NonStocks.Link()
            {
                Id = link.Id,
                Name = link.Name,
                EffectiveDate = link.EffectiveDate
            };

            return retVal;
        }
    }

    public class LinkProduct
    {
        public int Id { get; set; }
        public int LinkId { get; set; }
        public string Level_1 { get; set; }
        public string Level_2 { get; set; }
        public string Level_3 { get; set; }
        public string Level_4 { get; set; }
        public string Level_5 { get; set; }
        public Int16 Order { get; set; }

        public static List<Models.LinkProduct> AllToModel(List<NonStocks.LinkProduct> links)
        {
            var retList = new List<Models.LinkProduct>();

            foreach (var l in links)
            {
                retList.Add(ToModel(l));
            }

            return retList;
        }

        public static Models.LinkProduct ToModel(NonStocks.LinkProduct link)
        {
            var retVal = new Models.LinkProduct()
            {
                Id = link.Id,
                LinkId = link.LinkId,
                Level_1 = link.Level_1,
                Level_2 = link.Level_2,
                Level_3 = link.Level_3,
                Level_4 = link.Level_4,
                Level_5 = link.Level_5,
                Order = link.Order.HasValue ? link.Order.Value : (Int16)0,
            };
            return retVal;
        }

        public static NonStocks.LinkProduct ToEntity(Models.LinkProduct link)
        {
            var retVal = new NonStocks.LinkProduct()
            {
                Id = link.Id,
                LinkId = link.LinkId,
                Level_1 = link.Level_1,
                Level_2 = link.Level_2,
                Level_3 = link.Level_3,
                Level_4 = link.Level_4,
                Level_5 = link.Level_5,
                Order = link.Order,
            };
            return retVal;
        }

    }

    public class LinkNonStock
    {
        public int Id { get; set; }
        public int LinkId { get; set; }
        public int NonStockId { get; set; }
        public Int16 Order { get; set; }
        public Models.NonStockModel NonStockObj { get; set; }

        public static List<Models.LinkNonStock> AllToModel(List<NonStocks.LinkNonStock> links)
        {
            var retList = new List<Models.LinkNonStock>();

            foreach (var l in links)
            {
                retList.Add(ToModel(l));
            }

            return retList;
        }

        public static Models.LinkNonStock ToModel(NonStocks.LinkNonStock link)
        {
            var retVal = new Models.LinkNonStock()
            {
                Id = link.Id,
                LinkId = link.LinkId,
                NonStockId = link.NonStockId,
                Order = link.Order.HasValue ? link.Order.Value : (Int16)0,
            };
            return retVal;
        }

        public static NonStocks.LinkNonStock ToEntity(Models.LinkNonStock link)
        {
            var retVal = new NonStocks.LinkNonStock()
            {
                Id = link.Id,
                LinkId = link.LinkId,
                NonStockId = link.NonStockId,
                Order = link.Order,
            };
            return retVal;
        }
    }
}
