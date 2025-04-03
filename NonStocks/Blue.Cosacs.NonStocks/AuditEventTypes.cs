using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.NonStocks
{
    public class AuditEventTypes
    {
        public const string ViewNonStock = "ViewNonStock";
        public const string ViewNonStockBySku = "ViewNonStockBySku";
        public const string CreateNonStock = "CreateNonStock";

        public const string GuiGetAllNonStocks = "GuiGetAllNonStocks";
        public const string IndexAllNonStocks = "IndexAllNonStocks";

        public const string ViewEspecificPromotions = "ViewEspecificPromotions";
        public const string SearchPromotions = "SearchPromotions";
        public const string CreatePromotions = "CreatePromotions";
        public const string DeletePromotion = "DeletePromotion";

        public const string ExportNonStocks = "ExportNonStocks";
        public const string ExportNonStocksPromotions = "ExportNonStocksPromotions";
        public const string ExportNonStocksProductLinks = "ExportNonStocksProductLinks";

        public const string DownloadProductsFile = "DownloadProductsFile";
        public const string DownloadPromotionsFile = "DownloadPromotionsFile";
        public const string DownloadProductAssociationsFile = "DownloadProductAssociationsFile";

        public const string ViewNonStockPrices = "ViewNonStockPrices";
        public const string CreateNonStockPrice = "CreateNonStockPrice";
        public const string DeleteNonStockPrice = "DeleteNonStockPrice";

        public const string SearchProductLinks = "SearchProductLinks";
        public const string ViewProductLinkById = "ViewProductLinkById";
        public const string CreateProductLink = "CreateProductLink";
        public const string DeleteProductLink = "DeleteProductLink";

    }

}
