using Blue.Cosacs.NonStocks.Models;
using System;
using System.Collections.Generic;

namespace Blue.Cosacs.NonStocks
{
    public interface IProductLinkRepository
    {
        Models.Link LoadLink(int id);
        List<Models.Link> LoadLinks(int[] ids, NonStockLinkSearch search);
        List<Models.Link> LoadAllLinks(NonStockLinkSearch search);
        Models.Link SaveLink(Models.Link link);
        bool DeleteLink(int id);
        int[] GetIdsUsingNameDateFilters(string name, DateTime? dateFrom, DateTime? dateTo);
        int GetLinkCount(NonStockLinkSearch search);
        Link GetLink(int linkId);
    }
}
