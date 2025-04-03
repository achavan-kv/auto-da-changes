using Blue.Cosacs.NonStocks.ExternalHttpService;
using Blue.Cosacs.NonStocks.Models.WinCosacs;
using Blue.Cosacs.NonStocks.Test.Extensions;
using Blue.Cosacs.Test.CvsReader;
using Blue.Data;
using Blue.Transactions;
using CsvHelper.Configuration;
using Moq;
using NUnit.Framework;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;

namespace Blue.Cosacs.NonStocks.Test
{
    //[TestFixture]
    public class ProcessPricesForExport
    {
        //[TestFixtureSetUp]
        public virtual void Setup()
        {
            ObjectFactory.Initialize(p =>
            {
                p.AddRegistry(new Blue.Cosacs.NonStocks.Test.NonStocks.Registry());
            });
        }

        //[Test]
        public void CheckThatExportProductAmendmentFileWorks()
        {
            var courtsNetWsPath = @"Sources\NonStocks\ExternalHttpSources\CourtsNetWS\{0}";
            ObjectFactory.Inject<ICourtsNetWS>(new CourtsNetWSMock { path = courtsNetWsPath });

            var mockContext = new Mock<Context>();

            SetupEntityFrameworkDBMock_NonStock(mockContext,
                @"Sources\NonStocks\Export\{0}", "NonStocksData.csv");

            SetupEntityFrameworkDBMock_NonStockPrice(mockContext,
                @"Sources\NonStocks\Export\NonStockPrices\{0}", "NonStockPricesData.csv");

            var mockReadScope = new Mock<ReadScope<Context>>();
            mockReadScope.Setup(p => p.Context).Returns(mockContext.Object);

            var repository = ObjectFactory.GetInstance<NonStocksRepository>();

            var testFile = repository.ExportProductsFile("99999");

            Assert.True(testFile.Length == 252644);
        }

        //[Test]
        public void CheckThatExportPromotionalPriceFileWorks()
        {
            var courtsNetWsPath = @"Sources\NonStocks\ExternalHttpSources\CourtsNetWS\{0}";
            ObjectFactory.Inject<ICourtsNetWS>(new CourtsNetWSMock { path = courtsNetWsPath });

            var mockContext = new Mock<Context>();

            SetupEntityFrameworkDBMock_NonStock(mockContext,
                @"Sources\NonStocks\Export\{0}", "NonStocksData.csv");

            SetupEntityFrameworkDBMock_NonStockPrice(mockContext,
                @"Sources\NonStocks\Export\NonStockPrices\{0}", "NonStockPricesData.csv");

            SetupEntityFrameworkDBMock_NonStockPromotion(mockContext,
                @"Sources\NonStocks\Export\NonStockPromotions\{0}", "NonStockPromotionsData.csv");

            var mockReadScope = new Mock<ReadScope<Context>>();
            mockReadScope.Setup(p => p.Context).Returns(mockContext.Object);

            var repository = ObjectFactory.GetInstance<NonStocksRepository>();

            var testFile = repository.ExportPromotionsFile("99999");

            Assert.True(testFile.Length == 3887);
        }

        //[Test]
        public void CheckThatExportProductLinkFileWorks()
        {
            var courtsNetWsPath = @"Sources\NonStocks\ExternalHttpSources\CourtsNetWS\{0}";
            ObjectFactory.Inject<ICourtsNetWS>(new CourtsNetWSMock { path = courtsNetWsPath });

            var mockContext = new Mock<Context>();

            SetupEntityFrameworkDBMock_NonStocks_Link(mockContext,
                @"Sources\NonStocks\Export\ProductLinks\{0}",
                "NonStocks_Link.csv", "NonStocks_LinkProduct.csv", "NonStocks_LinkNonStock.csv");

            var mockReadScope = new Mock<ReadScope<Context>>();
            mockReadScope.Setup(p => p.Context).Returns(mockContext.Object);

            var repository = ObjectFactory.GetInstance<NonStocksRepository>();

            var testFile = repository.ExportProductLinksFile("99999");

            Assert.True(testFile.Length == 880);
        }

        //[Test]
        public void CheckInvalidWarehouseNumberRepetitions()
        {
            var courtsNetWsPath = @"Sources\NonStocks\ExternalHttpSources\CourtsNetWS\{0}";
            ObjectFactory.Inject<ICourtsNetWS>(new CourtsNetWSMock { path = courtsNetWsPath });

            var mockContext = new Mock<Context>();
            SetupEntityFrameworkDBMock_NonStockPrice(mockContext,
                @"Sources\NonStocks\Export\NonStockPrice\{0}", "NonStockPriceData.csv");

            var mockReadScope = new Mock<ReadScope<Context>>();
            mockReadScope.Setup(p => p.Context).Returns(mockContext.Object);

            var repository = ObjectFactory.GetInstance<NonStocksRepository>();
            var testFile = repository.ExportProductsFile("99999");

            var allExportedWarehouseNumber = testFile
                .Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(e => int.Parse(e.Substring(0, 2)))
                .ToList();

            var detectRepetitions = new List<int>();
            foreach (var warehouseNumber in allExportedWarehouseNumber)
            {
                if (!detectRepetitions.Contains(warehouseNumber))
                {
                    detectRepetitions.Add(warehouseNumber);
                }
            }
        }

        private void SetupEntityFrameworkDBMock_NonStock(Mock<Context> mockContext,
            string datafilePath, string dataFileName)
        {
            var dbSetObj = new Mock<DbSet<NonStock>>();
            ObjectFactory.Inject<INonStocksRepository>(new NonStockRepositoryMock
            {
                path = datafilePath,
                fileName = dataFileName
            });

            var nonStockData = Reader<NonStock>
                .Read(string.Format(datafilePath, dataFileName))
                .AsQueryable();

            mockContext.Setup(e => e.NonStock).Returns(dbSetObj.Object);
            dbSetObj.SetupQueryableMethods(nonStockData);
        }

        private void SetupEntityFrameworkDBMock_NonStockPrice(Mock<Context> mockContext,
            string datafilePath, string dataFileName)
        {
            var dbSetObj = new Mock<DbSet<NonStockPrice>>();
            ObjectFactory.Inject<IPriceRepository>(new PriceRepositoryMock
            {
                path = datafilePath,
                fileName = dataFileName
            });

            var nonStockPriceData = Reader<NonStockPrice>
                .Read(string.Format(datafilePath, dataFileName))
                .AsQueryable();

            mockContext.Setup(e => e.NonStockPrice).Returns(dbSetObj.Object);
            dbSetObj.SetupQueryableMethods(nonStockPriceData);
        }

        private void SetupEntityFrameworkDBMock_NonStockPromotion(Mock<Context> mockContext,
            string datafilePath, string dataFileName)
        {
            var dbSetObj = new Mock<DbSet<NonStockPromotion>>();
            ObjectFactory.Inject<IPromotionsRepository>(new PromotionsRepositoryMock
            {
                path = datafilePath,
                fileName = dataFileName
            });

            var nonStockPromotionData = Reader<NonStockPromotion>
                .Read(string.Format(datafilePath, dataFileName))
                .AsQueryable();

            mockContext.Setup(e => e.NonStockPromotion).Returns(dbSetObj.Object);
            dbSetObj.SetupQueryableMethods(nonStockPromotionData);
        }

        private void SetupEntityFrameworkDBMock_NonStocks_Link(Mock<Context> mockContext,
            string datafilePath, string linksFileName, string linkProductsFileName, string linkNonStocksFileName)
        {
            ObjectFactory.Inject<IProductLinkRepository>(new ProductLinkRepositoryMock
            {
                path = datafilePath,
                linksFileName = linksFileName,
                linkProductsFileName = linkProductsFileName,
                linkNonStocksFileName = linkNonStocksFileName
            });

            var dbSetLObj = new Mock<DbSet<Link>>();
            var dbSetLPObj = new Mock<DbSet<LinkProduct>>();
            var dbSetLNSObj = new Mock<DbSet<LinkNonStock>>();

            var linksData = Reader<Link>
                .Read(string.Format(datafilePath, linksFileName))
                .AsQueryable();
            var linkProductsData = Reader<LinkProduct>
                .Read(string.Format(datafilePath, linkProductsFileName))
                .AsQueryable();
            var linkNonStocksData = Reader<LinkNonStock>
                .Read(string.Format(datafilePath, linkNonStocksFileName))
                .AsQueryable();

            mockContext.Setup(e => e.Link).Returns(dbSetLObj.Object);
            dbSetLObj.SetupQueryableMethods(linksData);
            mockContext.Setup(e => e.LinkProduct).Returns(dbSetLPObj.Object);
            dbSetLPObj.SetupQueryableMethods(linkProductsData);
            mockContext.Setup(e => e.LinkNonStock).Returns(dbSetLNSObj.Object);
            dbSetLNSObj.SetupQueryableMethods(linkNonStocksData);
        }
    }

    #region mocked repositories for tests

    //public class CustomDbContext : DbContext, IDisposable
    //{
    //    public virtual DbSet<NonStock> NonStock { get; set; }
    //    public virtual DbSet<NonStockHierarchy> NonStockHierarchy { get; set; }
    //    public virtual DbSet<NonStockPrice> NonStockPrice { get; set; }
    //    public virtual DbSet<NonStockPromotion> NonStockPromotion { get; set; }
    //    public virtual DbSet<Link> Link { get; set; }
    //    public virtual DbSet<LinkProduct> LinkProduct { get; set; }
    //    public virtual DbSet<LinkNonStock> LinkNonStock { get; set; }
    //    public virtual DbSet<NonStockType> NonStockType { get; set; }
    //}

    public class NonStockRepositoryMock : INonStocksRepository
    {
        public string path { get; set; }
        public string fileName { get; set; }

        public int SaveNonStockDetails(NonStock nonStock, List<NonStockHierarchy> hierarchy)
        {
            throw new NotImplementedException();
        }

        public Models.NonStockModel Load(int id)
        {
            throw new NotImplementedException();
        }

        public NonStock Load(string sku)
        {
            throw new NotImplementedException();
        }

        public List<Models.NonStockModel> Load(int[] ids)
        {
            throw new NotImplementedException();
        }

        public List<Models.NonStockModel> LoadAll()
        {
            var nonStocks = Reader<Models.NonStockModel>.Read(
                string.Format(path, fileName),
                new CsvConfiguration
                {
                    WillThrowOnMissingField = false
                })
                .ToList();

            return nonStocks;
        }

        public string ExportProductsFile(string user)
        {
            throw new NotImplementedException();
        }

        public string ExportPromotionsFile(string user)
        {
            throw new NotImplementedException();
        }

        public string ExportProductLinksFile(string user)
        {
            throw new NotImplementedException();
        }

        public List<Models.NonStockModel> GetActiveNonStocks(IList<string> types)
        {
            throw new NotImplementedException();
        }

        public string UpdateCodeMaintenance()
        {
            throw new NotImplementedException();
        }
    }

    public class PriceRepositoryMock : IPriceRepository
    {
        public string path { get; set; }
        public string fileName { get; set; }
        //public bool forPriceView { get; set; }

        public List<Models.NonStockPriceModel> GetPrices(int id)
        {
            var prices = Reader<Models.NonStockPriceModel>.Read(
                string.Format(path, fileName),
                new CsvConfiguration
                {
                    WillThrowOnMissingField = false
                })
                .Where(e => e.NonStockId == id)
                .ToList();

            return prices;
        }

        public List<Models.NonStockPriceModel> GetActiveExpandedPrices(int id, IList<BranchInfo> allBranches)
        {
            throw new NotImplementedException();
        }

        public Models.NonStockPriceModel SavePrice(Models.NonStockPriceModel nonStockPrice)
        {
            throw new NotImplementedException();
        }

        public Models.NonStockPriceModel DeletePrice(int id)
        {
            throw new NotImplementedException();
        }
    }

    public class PromotionsRepositoryMock : IPromotionsRepository
    {
        public string path { get; set; }
        public string fileName { get; set; }


        public void DeletePromotion(int promotionId)
        {
            throw new NotImplementedException();
        }

        public Data.IPagedSearchResults<Models.NonStockPromotionModel> GetPromotions(Promotions.Filter filterValues)
        {
            var promotions = Reader<Models.NonStockPromotionModel>.Read(
                string.Format(path, fileName),
                new CsvConfiguration
                {
                    WillThrowOnMissingField = false
                })
                .Where(e => e.NonStockId == filterValues.NonStockId)
                .AsQueryable();

            return filterValues.Page(promotions);
        }

        public IEnumerable<Models.NonStockPromotionModel> GetPromotionsForNonStock(int nonStockId, DateTime? endDate, IEnumerable<int> nonStockPriceId, bool getCurrentPromotions)
        {
            throw new NotImplementedException();
        }

        public Models.NonStockPromotionModel SavePromotion(Models.NonStockPromotionModel promotion)
        {
            throw new NotImplementedException();
        }
    }

    public class ProductLinkRepositoryMock : IProductLinkRepository
    {
        public string path { get; set; }
        public string linksFileName { get; set; }
        public string linkProductsFileName { get; set; }
        public string linkNonStocksFileName { get; set; }

        public Models.Link LoadLink(int id)
        {
            throw new NotImplementedException();
        }

        public List<Models.Link> LoadLinks(int[] ids, Models.NonStockLinkSearch search)
        {
            throw new NotImplementedException();
        }

        public List<Models.Link> LoadAllLinks(Models.NonStockLinkSearch search)
        {
            var links = Reader<Models.Link>.Read(
                string.Format(path, linksFileName),
                new CsvConfiguration
                {
                    WillThrowOnMissingField = false
                })
                .AsQueryable();

            var linkProducts = Reader<Models.LinkProduct>.Read(
                string.Format(path, linkProductsFileName),
                new CsvConfiguration
                {
                    WillThrowOnMissingField = false
                })
                .AsQueryable();

            var linkNonStocks = Reader<Models.LinkNonStock>.Read(
                string.Format(path, linkNonStocksFileName),
                new CsvConfiguration
                {
                    WillThrowOnMissingField = false
                })
                .AsQueryable();

            foreach (var link in links)
            {
                link.linkProducts = linkProducts.Where(e => e.LinkId == link.Id).ToList();
                link.linkNonStocks = linkNonStocks.Where(e => e.LinkId == link.Id).ToList();
            }

            return links.ToList();
        }

        public Models.Link SaveLink(Models.Link link)
        {
            throw new NotImplementedException();
        }

        public bool DeleteLink(int id)
        {
            throw new NotImplementedException();
        }

        public int[] GetIdsUsingNameDateFilters(string name, DateTime? dateFrom, DateTime? dateTo)
        {
            throw new NotImplementedException();
        }

        public int GetLinkCount(Models.NonStockLinkSearch search)
        {
            throw new NotImplementedException();
        }

        public Link GetLink(int linkId)
        {
            throw new NotImplementedException();
        }
    }

    public class CourtsNetWSMock : ICourtsNetWS
    {
        public string path { get; set; }

        public List<Models.WinCosacs.BranchInfo> GetDboInfoBranches()
        {
            var tmpPath = string.Format(path, "DboInfoBranches.json");
            if (!File.Exists(tmpPath))
                return null;

            return ReadBranchesJsonData<List<Models.WinCosacs.BranchInfo>>(tmpPath);
        }

        public List<Models.WinCosacs.HierarchyInfo> GetDboInfoHierarchy()
        {
            var tmpPath = string.Format(path, "DboInfoHierarchy.json");
            if (!File.Exists(tmpPath))
                return null;

            return ReadHierarchyJsonData<List<Models.WinCosacs.HierarchyInfo>>(tmpPath);
        }

        private List<BranchInfo> ReadBranchesJsonData<T>(string path)
        {
            var retData = new List<BranchInfo>();
            using (var reader = new StreamReader(path, false))
            {
                var stringReader = new StringReader(reader.ReadToEnd());
                retData = (List<Models.WinCosacs.BranchInfo>)new Newtonsoft.Json.JsonSerializer()
                    .Deserialize(stringReader, (new List<Models.WinCosacs.BranchInfo>())
                    .GetType()
                    );
            }

            return retData;
        }

        private List<HierarchyInfo> ReadHierarchyJsonData<T>(string path)
        {
            var retData = new List<HierarchyInfo>();
            using (var reader = new StreamReader(path, false))
            {
                var stringReader = new StringReader(reader.ReadToEnd());
                retData = (List<Models.WinCosacs.HierarchyInfo>)new Newtonsoft.Json.JsonSerializer()
                    .Deserialize(stringReader, (new List<Models.WinCosacs.HierarchyInfo>())
                    .GetType()
                    );
            }

            return retData;
        }

        public CourtsNetWS.SetupEodResult Fact2000SetupEODFile(object jsonFileContent)
        {
            throw new NotImplementedException();
        }

        public CourtsNetWS.CodeMaintenanceUpdateResult UpdateCodeMaintenance(List<Models.NonStockModel> nonStocks)
        {
            throw new NotImplementedException();
        }
    }

    #endregion
}
