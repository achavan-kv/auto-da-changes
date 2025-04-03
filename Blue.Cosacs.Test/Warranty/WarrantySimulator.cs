using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Blue.Cosacs.Stock.Repositories;
using Blue.Events;
using CsvHelper.Configuration;
using Merch = Blue.Cosacs.Stock;
using Warr = Blue.Cosacs.Warranty;
using Blue.Transactions;
using Moq;
using NUnit.Framework;
using StructureMap;
using Blue.Cosacs.Test;
using Blue.Cosacs.Warranty.Repositories;
using Blue.Cosacs.Warranty.Model;
using Blue.Cosacs.Stock;
using Blue.Cosacs.Warranty;
using System.Collections;

namespace Blue.Cosacs.Test.Warranty
{
    [TestFixture]
    public class WarrantySimulator
    {
        [TestFixtureSetUp]
        public virtual void Setup()
        {
            ObjectFactory.Initialize(p =>
                {
                    p.AddRegistry(new Blue.Cosacs.Test.Warranty.Registry());
                    p.AddRegistry(new WarrantySimulator.Registry());
                });
        }

        [Test]
        [Ignore("This tests are hiting the database. We need to fix this")]
        public void GetProductsByCategory_NumberOfRows_StockBranchNameWarrantyLink_StoreType()
        {
            //const string path = @"CsvSources\Warranty\WarrantySimulator\GetProductsByCategory\{0}";
            //var repository = ObjectFactory.GetInstance<ProductRepository>();

            //var branchData = new List<Branch>
            //{
            //    new Branch { branchname = "BRIDGETOWN", branchno = 900, StoreType = "C" },
            //    new Branch { branchname = "SPEIGHTSTOWN", branchno = 901, StoreType = "C" },
            //    new Branch { branchname = "OISTINS", branchno = 902, StoreType = "C" },
            //    new Branch { branchname = "SHERATON", branchno = 903, StoreType = "C" },
            //    new Branch { branchname = "CONTEMPO", branchno = 904, StoreType = "C" },
            //    new Branch { branchname = "JULIE'N", branchno = 905, StoreType = "C" },
            //    new Branch { branchname = "COURTS AT CAVE SHEPH", branchno = 906, StoreType = "C" },
            //    new Branch { branchname = "EMERALD CITY", branchno = 907, StoreType = "C" },
            //    new Branch { branchname = "SPRING GARDEN W/HOUS", branchno = 908, StoreType = "C" },
            //    new Branch { branchname = "WILDEY W/HOUSE", branchno = 909, StoreType = "C" },
            //    new Branch { branchname = "SEVICE DEPARTMENT", branchno = 910, StoreType = "C" },
            //    new Branch { branchname = "ELLERTON", branchno = 911, StoreType = "C" },
            //    new Branch { branchname = "SHOP HILL", branchno = 912, StoreType = "C" },
            //    new Branch { branchname = "CAVANS LANE", branchno = 913, StoreType = "C" },
            //    new Branch { branchname = "BARGAIN CENTRE", branchno = 916, StoreType = "C" },
            //    new Branch { branchname = "SHOPCOURTS", branchno = 917, StoreType = "C" },
            //    new Branch { branchname = "COMMERCIAL SALES", branchno = 918, StoreType = "C" },
            //    new Branch { branchname = "BRIDGETOWN OPTICAL", branchno = 919, StoreType = "C" },
            //    new Branch { branchname = "SHERATON OPTICAL", branchno = 920, StoreType = "C" },
            //}.AsQueryable();


            //var mockContext = new Mock<Blue.Cosacs.Merchandising.Context>();
            //var mockBranchLookup = new Mock<DbSet<Branch>>();

            //var stockItemViewData = CvsReader.Reader<StockItemView>.Read(string.Format(path, @"products.csv")).AsQueryable();
            //var mockstockItemView = new Mock<DbSet<StockItemView>>();

            //var mockReadScope = new Mock<ReadScope<Blue.Cosacs.Merchandising.Context>>();

            //mockBranchLookup.SetuptQuerableMethods(branchData);
            //mockstockItemView.SetuptQuerableMethods(stockItemViewData);

            //mockContext.Setup(c => c.Branch).Returns(mockBranchLookup.Object);
            //mockContext.Setup(c => c.StockItemView).Returns(mockstockItemView.Object);

            //mockReadScope.Setup(p => p.Context).Returns(mockContext.Object);

            //ObjectFactory.Inject<ReadScope<Blue.Cosacs.Merchandising.Context>>(mockReadScope.Object);


            //var results = repository.GetProductsByCategory("PCE", 5, 913);

            //Assert.AreEqual(627, results.Count);
            //Assert.AreEqual("913 CAVANS LANE", results[0].StockBranchNameWarrantyLink);
            //Assert.AreEqual("C", results[0].StoreType);
        }

        [Test]
        [Ignore("This tests are hiting the database. We need to fix this")]
        public void GetProductsByCategory_Level2_IsEmpty()
        {
            //const string path = @"CsvSources\Warranty\WarrantySimulator\GetProductsByCategory\{0}";
            //var repository = ObjectFactory.GetInstance<ProductRepository>();

            //var branchData = new List<Branch>
            //{
            //    new Branch { branchname = "CAVANS LANE", branchno = 913, StoreType = "C" },
            //}.AsQueryable();


            //var mockContext = new Mock<Blue.Cosacs.Merchandising.Context>();
            //var mockBranchLookup = new Mock<DbSet<Branch>>();

            //var stockItemViewData = CvsReader.Reader<StockItemView>.Read(string.Format(path, @"products.csv")).AsQueryable();
            //var mockstockItemView = new Mock<DbSet<StockItemView>>();

            //var mockReadScope = new Mock<ReadScope<Blue.Cosacs.Merchandising.Context>>();

            //mockBranchLookup.SetuptQuerableMethods(branchData);
            //mockstockItemView.SetuptQuerableMethods(stockItemViewData);

            //mockContext.Setup(c => c.Branch).Returns(mockBranchLookup.Object);
            //mockContext.Setup(c => c.StockItemView).Returns(mockstockItemView.Object);

            //mockReadScope.Setup(p => p.Context).Returns(mockContext.Object);

            //ObjectFactory.Inject<ReadScope<Blue.Cosacs.Merchandising.Context>>(mockReadScope.Object);


            //var results = repository.GetProductsByCategory("PCE", 0, 913);

            //Assert.AreEqual(string.Empty, results[0].Level_2);        
        }

        [Test]
        [Ignore("This tests are hiting the database. We need to fix this")]
        public void AllProducts_Eletrical_Vision_900_19390903()
        {
            const string path = @"CsvSources\Warranty\WarrantySimulator\AllProducts_Eletrical_Vision_900_19390903\{0}";
            ObjectFactory.Inject<IProductRepository>(new MockProductRepository { path = path });

            var repository = ObjectFactory.GetInstance<Warr.Repositories.WarrantyLinkRepository>();
            var mockReadScope = new Mock<ReadScope<Warr.Context>>();
            var mockContext = new Mock<Warr.Context>();
            var branchLookupData = new List<Warr.BranchLookup>
            {
                new Warr.BranchLookup { BranchNameLong = "CONTEMPO", StoreType = "C", branchname = "CONTEMPO", branchno = 900 }
            }.AsQueryable();

            var mockBranchLookup = new Mock<DbSet<Warr.BranchLookup>>();



            var mockWarrantyTags = new Mock<DbSet<Warr.WarrantyTags>>();
            var warrantyTagsData = CvsReader.Reader<Warr.WarrantyTags>.Read(string.Format(path, @"WarrantyTags.csv")).AsQueryable();
            var mockTags = new Mock<DbSet<Warr.Tag>>();
            var tagsData = CvsReader.Reader<Warr.Tag>.Read(string.Format(path, @"Tag.csv")).AsQueryable();

            var mockProductWarrantyLinkView = new Mock<DbSet<Warr.ProductWarrantyLinkView>>();
            var productWarrantyLinkViewData = CvsReader.Reader<Warr.ProductWarrantyLinkView>.Read(string.Format(path, @"ProductWarrantyLinkView.csv"))
                .AsQueryable();

            mockBranchLookup.SetuptQuerableMethods(branchLookupData);
            mockProductWarrantyLinkView.SetuptQuerableMethods(productWarrantyLinkViewData);
            mockWarrantyTags.SetuptQuerableMethods(warrantyTagsData);
            mockTags.SetuptQuerableMethods(tagsData);

            mockContext.Setup(c => c.BranchLookup).Returns(mockBranchLookup.Object);
            mockContext.Setup(c => c.ProductWarrantyLinkView).Returns(mockProductWarrantyLinkView.Object);
            mockContext.Setup(c => c.WarrantyTags).Returns(mockWarrantyTags.Object);
            mockContext.Setup(c => c.Tag).Returns(mockTags.Object);

            var search = new Warr.Model.WarrantySearchByProduct
            {
                Product = null,
                Date = new DateTime(1939, 9, 3),
                Department = "PCE",
                CategoryId = 1, //Vision
                Location = 900,
            };
            mockReadScope.Setup(p => p.Context).Returns(mockContext.Object);

            ObjectFactory.Inject<ReadScope<Warr.Context>>(mockReadScope.Object);

            var results = repository.Search(search);

            Assert.AreEqual(83, results.Items.Count(), "Number of items with warranty should be 83");
            Assert.AreEqual(431, results.ItemsWithoutWarranties.Count(), "Number of items without warranty should be 431");
        }

        [Test]
        [Ignore("This tests are hiting the database. We need to fix this")]
        public void AllProducts_Eletrical_Audio_900_19390903()
        {
            const string path = @"CsvSources\Warranty\WarrantySimulator\AllProducts_Eletrical_Audio_900_19390903\{0}";
            ObjectFactory.Inject<IProductRepository>(new MockProductRepository { path = path });
            var repository = ObjectFactory.GetInstance<Warr.Repositories.WarrantyLinkRepository>();
            var mockReadScope = new Mock<ReadScope<Warr.Context>>();
            var mockContext = new Mock<Warr.Context>();
            var branchLookupData = new List<Warr.BranchLookup>
            {
                new Warr.BranchLookup { BranchNameLong = "CONTEMPO", StoreType = "C", branchname = "CONTEMPO", branchno = 900 }
            }.AsQueryable();

            var mockBranchLookup = new Mock<DbSet<Warr.BranchLookup>>();

            var mockWarrantyTags = new Mock<DbSet<Warr.WarrantyTags>>();
            var warrantyTagsData = CvsReader.Reader<Warr.WarrantyTags>.Read(string.Format(path, @"WarrantyTags.csv")).AsQueryable();
            var mockTags = new Mock<DbSet<Warr.Tag>>();
            var tagsData = CvsReader.Reader<Warr.Tag>.Read(string.Format(path, @"Tag.csv")).AsQueryable();

            var mockProductWarrantyLinkView = new Mock<DbSet<Warr.ProductWarrantyLinkView>>();
            var productWarrantyLinkViewData = CvsReader.Reader<Warr.ProductWarrantyLinkView>.Read(string.Format(path, @"ProductWarrantyLinkView.csv"))
                .AsQueryable();

            mockBranchLookup.SetuptQuerableMethods(branchLookupData);
            mockProductWarrantyLinkView.SetuptQuerableMethods(productWarrantyLinkViewData);
            mockWarrantyTags.SetuptQuerableMethods(warrantyTagsData);
            mockTags.SetuptQuerableMethods(tagsData);

            mockContext.Setup(c => c.BranchLookup).Returns(mockBranchLookup.Object);
            mockContext.Setup(c => c.ProductWarrantyLinkView).Returns(mockProductWarrantyLinkView.Object);
            mockContext.Setup(c => c.WarrantyTags).Returns(mockWarrantyTags.Object);
            mockContext.Setup(c => c.Tag).Returns(mockTags.Object);

            var search = new Warr.Model.WarrantySearchByProduct
            {
                Product = null,
                Date = new DateTime(1939, 9, 3),
                Department = "PCE",
                CategoryId = 2, //Vision
                Location = 900,
            };
            mockReadScope.Setup(p => p.Context).Returns(mockContext.Object);

            ObjectFactory.Inject<ReadScope<Warr.Context>>(mockReadScope.Object);


            var results = repository.Search(search);

            Assert.AreEqual(66, results.Items.Count(), "Number of items with warranty should be 83");
            Assert.AreEqual(351, results.ItemsWithoutWarranties.Count(), "Number of items without warranty should be 431");
        }

        private void SearchByProduct_SetupMockData(string path)
        {
            var mockReadScope = new Mock<ReadScope<Warr.Context>>();
            var mockContext = new Mock<Warr.Context>();

            ObjectFactory.Inject<ReadScope<Warr.Context>>(mockReadScope.Object);
            var mockProductWarrantyLinkView = new Mock<DbSet<Warr.ProductWarrantyLinkView>>();

            var branchLookupData = new List<Warr.BranchLookup>
            {
                new Warr.BranchLookup { BranchNameLong = "CONTEMPO", StoreType = "C", branchname = "CONTEMPO", branchno = 900 }
            }.AsQueryable();

            var productWarrantyLinkViewData = CvsReader.Reader<Warr.ProductWarrantyLinkView>.Read(string.Format(path, @"ProductWarrantyLinkView.csv"))
                .AsQueryable();
            var mockBranchLookup = new Mock<DbSet<Warr.BranchLookup>>();
            var mockWarrantyTags = new Mock<DbSet<Warr.WarrantyTags>>();
            var warrantyTagsData = CvsReader.Reader<Warr.WarrantyTags>.Read(string.Format(path, @"WarrantyTags.csv")).AsQueryable();
            var mockTags = new Mock<DbSet<Warr.Tag>>();
            var tagsData = CvsReader.Reader<Warr.Tag>.Read(string.Format(path, @"Tag.csv")).AsQueryable();


            mockBranchLookup.SetuptQuerableMethods(branchLookupData);
            mockProductWarrantyLinkView.SetuptQuerableMethods(productWarrantyLinkViewData);
            mockWarrantyTags.SetuptQuerableMethods(warrantyTagsData);
            mockTags.SetuptQuerableMethods(tagsData);

            mockContext.Setup(c => c.BranchLookup).Returns(mockBranchLookup.Object);
            mockContext.Setup(c => c.ProductWarrantyLinkView).Returns(mockProductWarrantyLinkView.Object);
            mockContext.Setup(c => c.WarrantyTags).Returns(mockWarrantyTags.Object);
            mockContext.Setup(c => c.Tag).Returns(mockTags.Object);
            mockReadScope.Setup(p => p.Context).Returns(mockContext.Object);

        }

        [Test]
        [Ignore("This tests are hiting the database. We need to fix this")]
        public void Search_AllProducts_NoLocation_ReturnNoResults()
        {
            const string path = @"CsvSources\Warranty\WarrantySimulator\AllProducts_Eletrical_Audio_900_19390903\{0}";
            ObjectFactory.Inject<IProductRepository>(new MockProductRepository { path = path });
            var repository = ObjectFactory.GetInstance<Warr.Repositories.WarrantyLinkRepository>();
            var mockReadScope = new Mock<ReadScope<Warr.Context>>();
            var mockContext = new Mock<Warr.Context>();
            var branchLookupData = new List<Warr.BranchLookup>
            {
                new Warr.BranchLookup { BranchNameLong = "CONTEMPO", StoreType = "C", branchname = "CONTEMPO", branchno = 900 }
            }.AsQueryable();

            var mockBranchLookup = new Mock<DbSet<Warr.BranchLookup>>();

            var mockWarrantyTags = new Mock<DbSet<Warr.WarrantyTags>>();
            var warrantyTagsData = CvsReader.Reader<Warr.WarrantyTags>.Read(string.Format(path, @"WarrantyTags.csv")).AsQueryable();
            var mockTags = new Mock<DbSet<Warr.Tag>>();
            var tagsData = CvsReader.Reader<Warr.Tag>.Read(string.Format(path, @"Tag.csv")).AsQueryable();

            var mockProductWarrantyLinkView = new Mock<DbSet<Warr.ProductWarrantyLinkView>>();
            var productWarrantyLinkViewData = CvsReader.Reader<Warr.ProductWarrantyLinkView>.Read(string.Format(path, @"ProductWarrantyLinkView.csv"))
                .AsQueryable();

            mockBranchLookup.SetuptQuerableMethods(branchLookupData);
            mockProductWarrantyLinkView.SetuptQuerableMethods(productWarrantyLinkViewData);
            mockWarrantyTags.SetuptQuerableMethods(warrantyTagsData);
            mockTags.SetuptQuerableMethods(tagsData);

            mockContext.Setup(c => c.BranchLookup).Returns(mockBranchLookup.Object);
            mockContext.Setup(c => c.ProductWarrantyLinkView).Returns(mockProductWarrantyLinkView.Object);
            mockContext.Setup(c => c.WarrantyTags).Returns(mockWarrantyTags.Object);
            mockContext.Setup(c => c.Tag).Returns(mockTags.Object);

            var search = new Warr.Model.WarrantySearchByProduct
            {
                Product = null,
                Date = new DateTime(1939, 9, 3),
                Department = "PCE",
                CategoryId = 2, //Vision
                Location = 0,
            };
            mockReadScope.Setup(p => p.Context).Returns(mockContext.Object);

            ObjectFactory.Inject<ReadScope<Warr.Context>>(mockReadScope.Object);
            var results = repository.Search(search);

            Assert.AreEqual(0, results.Items.Count());
        }

        [Test]
        [Ignore("This tests are hiting the database. We need to fix this")]
        public void SearchByProduct_SingleProductWithSpecificPrice_ReturnItem()
        {
            const string path = @"CsvSources\Warranty\WarrantySimulator\SearchByProduct\{0}";
            ObjectFactory.Inject<IProductRepository>(new MockProductRepository { path = path });
            var warrantyLinkRepository = ObjectFactory.GetInstance<Warr.Repositories.WarrantyLinkRepository>();
            var productRepository = ObjectFactory.GetInstance<ProductRepository>();

            SearchByProduct_SetupMockData(path);
            var search = new Warr.Model.WarrantySearchByProduct
            {
                Product = "12222",
                CategoryId = 0,
                Location = 900,
                PriceVATEx = 35,
                WarrantyTypeCode = null
            };

            var result = warrantyLinkRepository.SearchByProduct(search);
            Assert.That(result.Items.Count(), Is.GreaterThan(0));
        }

        [Test]
        [Ignore("This tests are hiting the database. We need to fix this")]
        public void SearchByProduct_SingleProduct_ReturnItemWithNoWarranties()
        {
            const string path = @"CsvSources\Warranty\WarrantySimulator\SearchByProduct\{0}";
            ObjectFactory.Inject<IProductRepository>(new MockProductRepository { path = path });
            var warrantyLinkRepository = ObjectFactory.GetInstance<Warr.Repositories.WarrantyLinkRepository>();
            var productRepository = ObjectFactory.GetInstance<ProductRepository>();

            SearchByProduct_SetupMockData(path);
            var search = new Warr.Model.WarrantySearchByProduct
            {
                Product = "1999999",
                CategoryId = 0,
                Location = 900
            };

            var result = warrantyLinkRepository.SearchByProduct(search);
            Assert.IsNull(result);
        }

        [Test]
        [Ignore("This tests are hiting the database. We need to fix this")]
        public void SearchByProduct_SingleProductWithSpecificDate_ReturnItem()
        {
            const string path = @"CsvSources\Warranty\WarrantySimulator\SearchByProduct\{0}";
            ObjectFactory.Inject<IProductRepository>(new Blue.Cosacs.Test.Warranty.WarrantySimulator.MockProductRepository { path = path });
            var warrantyLinkRepository = ObjectFactory.GetInstance<Warr.Repositories.WarrantyLinkRepository>();
            var productRepository = ObjectFactory.GetInstance<ProductRepository>();

            SearchByProduct_SetupMockData(path);
            var search = new Warr.Model.WarrantySearchByProduct
            {
                Product = "12222",
                Date = Convert.ToDateTime("25/09/2014"),
                CategoryId = 0,
                Location = 900,
                WarrantyTypeCode = null
            };

            var result = warrantyLinkRepository.SearchByProduct(search);
            Assert.That(result.Items.Count(), Is.GreaterThan(0));
        }

        [Test]
        [Ignore("This tests are hiting the database. We need to fix this")]
        public void SearchByProduct_AllProducts_ReturnProductsWithWarranties()
        {
            ObjectFactory.Initialize(p =>
            {
                p.AddRegistry(new Blue.Cosacs.Test.Warranty.Registry());
                p.AddRegistry(new WarrantySimulator.Registry());
            });

            const string path = @"CsvSources\Warranty\WarrantySimulator\SearchByProduct\{0}";
            ObjectFactory.Inject<IProductRepository>(new MockProductRepository { path = path });
            var warrantyLinkRepository = ObjectFactory.GetInstance<Warr.Repositories.WarrantyLinkRepository>();

            SearchByProduct_SetupMockData(path);
            var search = new Warr.Model.WarrantySearchByProduct
            {
                Product = null,
                Department = "PCE",
                CategoryId = 2,
                Location = 900,
                WarrantyTypeCode = null

            };

            var result = warrantyLinkRepository.SearchByProduct(search);
            Assert.That(result.Items.Count(), Is.GreaterThan(0));
        }

        [Test]
        [Ignore("This tests are hiting the database. We need to fix this")]
        public void SearchByProduct_AllProducts_ReturnItemsWithFreeWarranties()
        {
            const string path = @"CsvSources\Warranty\WarrantySimulator\SearchByProduct\{0}";
            ObjectFactory.Inject<IProductRepository>(new MockProductRepository { path = path });
            var warrantyLinkRepository = ObjectFactory.GetInstance<Warr.Repositories.WarrantyLinkRepository>();
            var productRepository = ObjectFactory.GetInstance<ProductRepository>();

            SearchByProduct_SetupMockData(path);
            var search = new Warr.Model.WarrantySearchByProduct
            {
                Product = null,
                Department = "PCE",
                CategoryId = 2,
                Location = 900,
                WarrantyTypeCode = "F"
            };

            var result = warrantyLinkRepository.SearchByProduct(search);
            Assert.That(result.Items.Count(), Is.GreaterThan(0));
        }

        [Test]
        [Ignore("This tests are hiting the database. We need to fix this")]
        public void SearchByProduct_AllProducts_ReturnItemsWithWarrantiesFromASpecificDate()
        {
            const string path = @"CsvSources\Warranty\WarrantySimulator\SearchByProduct\{0}";
            ObjectFactory.Inject<IProductRepository>(new MockProductRepository { path = path });
            var warrantyLinkRepository = ObjectFactory.GetInstance<Warr.Repositories.WarrantyLinkRepository>();
            var productRepository = ObjectFactory.GetInstance<ProductRepository>();

            SearchByProduct_SetupMockData(path);
            var search = new Warr.Model.WarrantySearchByProduct
            {
                Product = null,
                Date = Convert.ToDateTime("25/09/2014"),
                Department = "PCE",
                CategoryId = 2,
                Location = 900,
                WarrantyTypeCode = null
            };

            var result = warrantyLinkRepository.SearchByProduct(search);
            Assert.That(result.Items.Count(), Is.GreaterThan(0));
        }

        private void FindMatchingProduct_SetupMockData(string path)
        {
            var branch = new List<Stock.Branch>
            {
                new Stock.Branch { branchname = "CONTEMPO", StoreType = "C", branchno = 745 }
            }.AsQueryable();

            var mockBranch = new Mock<DbSet<Stock.Branch>>();
            mockBranch.SetuptQuerableMethods(branch);

            ObjectFactory.Inject<IProductRepository>(new MockProductRepository { path = path });
            var warrantyLinkRepository = ObjectFactory.GetInstance<Warr.Repositories.WarrantyLinkRepository>();

            var mockStockItemViewContext = new Mock<Stock.Context>();
            var mockReadScopeMerchandising = new Mock<ReadScope<Stock.Context>>();
            var mockStockItemView = new Mock<DbSet<Stock.StockItemView>>();
            var stockItemViewData = CvsReader.Reader<Stock.StockItemView>.Read(string.Format(path, @"StockItemData.csv"))
                .AsQueryable();
            mockStockItemView.SetuptQuerableMethods(stockItemViewData);

            mockStockItemViewContext.Setup(c => c.Branch).Returns(mockBranch.Object);
            mockStockItemViewContext.Setup(c => c.StockItemView).Returns(mockStockItemView.Object);

            mockReadScopeMerchandising.Setup(p => p.Context).Returns(mockStockItemViewContext.Object);
            ObjectFactory.Inject<ReadScope<Stock.Context>>(mockReadScopeMerchandising.Object);
        }

        //[Test]
        //[TestCase("11111")]
        //[TestCase("12345")]
        //[TestCase("23456")]
        //public void FindMatchingProduct_WhenPassingValidData_ShouldReturnProductDetails(string productNo)
        //{
        //    const string path = @"CsvSources\Warranty\WarrantySimulator\FindMatchingProduct\{0}";
        //    FindMatchingProduct_SetupMockData(path);

        //    var productRepository = ObjectFactory.GetInstance<ProductRepository>();
        //    short branchNumber = 745;
        //    var product = productRepository.FindMatchingProduct(productNo, branchNumber);

        //    Assert.IsNotNull(product);
        //}

        //[Test]
        //[TestCase("44444")]
        //public void FindMatchingProduct_WhenPassingInValidData_ShouldReturnEmptyObject(string productNo)
        //{
        //    const string path = @"CsvSources\Warranty\WarrantySimulator\FindMatchingProduct\{0}";
        //    var productRepository = ObjectFactory.GetInstance<ProductRepository>();

        //    FindMatchingProduct_SetupMockData(path);

        //    short location = 745;
        //    var product = productRepository.FindMatchingProduct(productNo, location);

        //    Assert.IsNull(product);
        //}

        [Test]
        [Ignore("This tests are hiting the database. We need to fix this")]
        public void GetItemDetails_SearchForElectricalItem_ReturnItemDetails()
        {
            WarrantySearchByProduct search = new WarrantySearchByProduct()
            {
                CategoryId = 2,
                Department = "PCE",
                Location = 900
            };

            var product = new WarrantyProductLinkSearch()
            {

                CashPrice = "521",
                Category = "Audio",
                CostPrice = "869",
                Description = "SONY",
                ItemNoWarrantyLink = "226164",
                Level_1 = "PCE",
                Level_2 = "2",
                Level_3 = "22",
                RefCodeWarrantyLink = "54",
                StockBranchProduct = 900,
                StoreType = "C"

            };

            var productWarrantyLinkViewList = new List<ProductWarrantyLinkView>();

            var productWarrantyLinkView = new ProductWarrantyLinkView()
            {
                EffectiveDate = Convert.ToDateTime("03/09/1939"),
                Id = 1,
                ItemNumber = string.Empty,
                Level1 = "PCE",
                Level2 = "2",
                Level3 = "22",
                LinkId = 136,
                LinkName = "LinkTest1",
                ProductMin = 500,
                ProductMax = 750,
                RefCode = string.Empty,
                StoreType = string.Empty,
                WarrantyDescription = "WarrantyDescription",
                WarrantyId = 3,
                WarrantyLenght = 24,
                WarrantyNumber = "191033",
                WarrantyTaxRange = Convert.ToDecimal(17.5),
                WarrantyTypeCode = "E"
            };

            productWarrantyLinkViewList.Add(productWarrantyLinkView);
            var productWarrantyLinks = productWarrantyLinkViewList.ToLookup(p => new Tuple<decimal, decimal>(p.ProductMax, p.ProductMin));

            ObjectFactory.Inject<IProductRepository>(new MockProductRepository { path = string.Empty });
            var warrantyLinkRepository = ObjectFactory.GetInstance<Warr.Repositories.WarrantyLinkRepository>();

            var result = warrantyLinkRepository.GetItemDetails(search, product, productWarrantyLinks);
            Assert.That(result.Count, Is.GreaterThanOrEqualTo(1));
        }

        [Test]
        [Ignore("This tests are hiting the database. We need to fix this")]
        public void GetItemDetails_CashPriceNotBetweenMinAndMaxPrice_ReturnNoDetails()
        {
            WarrantySearchByProduct search = new WarrantySearchByProduct()
            {
                CategoryId = 2,
                Department = "PCE",
                Location = 900
            };

            var product = new WarrantyProductLinkSearch()
            {

                CashPrice = "921",
                Category = "Audio",
                CostPrice = "869",
                Description = "SONY",
                ItemNoWarrantyLink = "226164",
                Level_1 = "PCE",
                Level_2 = "2",
                Level_3 = "22",
                RefCodeWarrantyLink = "54",
                StockBranchNameWarrantyLink = "900 BridgeTown",
                StockBranchProduct = 900,
                StoreType = "C"

            };

            var productWarrantyLinkViewList = new List<ProductWarrantyLinkView>();

            var productWarrantyLinkView = new ProductWarrantyLinkView()
            {
                EffectiveDate = Convert.ToDateTime("03/09/1939"),
                Id = 1,
                ItemNumber = string.Empty,
                Level1 = "PCE",
                Level2 = "2",
                Level3 = "22",
                LinkId = 136,
                LinkName = "LinkTest1",
                ProductMin = 500,
                ProductMax = 750,
                RefCode = string.Empty,
                StockBranch = 900,
                StoreType = string.Empty,
                WarrantyDescription = "WarrantyDescription",
                WarrantyId = 3,
                WarrantyLenght = 24,
                WarrantyNumber = "191033",
                WarrantyTaxRange = Convert.ToDecimal(17.5),
                WarrantyTypeCode = "E"
            };

            productWarrantyLinkViewList.Add(productWarrantyLinkView);
            var productWarrantyLinks = productWarrantyLinkViewList.ToLookup(p => new Tuple<decimal, decimal>(p.ProductMax, p.ProductMin));

            ObjectFactory.Inject<IProductRepository>(new MockProductRepository { path = string.Empty });
            var warrantyLinkRepository = ObjectFactory.GetInstance<Warr.Repositories.WarrantyLinkRepository>();

            var result = warrantyLinkRepository.GetItemDetails(search, product, productWarrantyLinks);
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        [Ignore("This tests are hiting the database. We need to fix this")]
        public void GetItemDetails_NoItemNo_SameRefCode_ReturnItemDetails()
        {
            WarrantySearchByProduct search = new WarrantySearchByProduct()
            {
                CategoryId = 2,
                Department = "PCE",
                Location = 900
            };

            var product = new WarrantyProductLinkSearch()
            {

                CashPrice = "521",
                Category = "Audio",
                CostPrice = "869",
                Description = "SONY",
                //  ItemNoWarrantyLink = "226164",
                Level_1 = "PCE",
                Level_2 = "2",
                Level_3 = "22",
                RefCodeWarrantyLink = "54",
                StockBranchNameWarrantyLink = "900 BridgeTown",
                StockBranchProduct = 900,
                StoreType = "C"

            };

            var productWarrantyLinkViewList = new List<ProductWarrantyLinkView>();

            var productWarrantyLinkView = new ProductWarrantyLinkView()
            {
                EffectiveDate = Convert.ToDateTime("03/09/1939"),
                Id = 1,
                ItemNumber = string.Empty,
                Level1 = "PCE",
                Level2 = "2",
                Level3 = "22",
                LinkId = 136,
                LinkName = "LinkTest1",
                ProductMin = 500,
                ProductMax = 750,
                RefCode = "54",
                StockBranch = 900,
                StoreType = string.Empty,
                WarrantyDescription = "WarrantyDescription",
                WarrantyId = 3,
                WarrantyLenght = 24,
                WarrantyNumber = "191033",
                WarrantyTaxRange = Convert.ToDecimal(17.5),
                WarrantyTypeCode = "E"
            };

            productWarrantyLinkViewList.Add(productWarrantyLinkView);
            var productWarrantyLinks = productWarrantyLinkViewList.ToLookup(p => new Tuple<decimal, decimal>(p.ProductMax, p.ProductMin));

            ObjectFactory.Inject<IProductRepository>(new MockProductRepository { path = string.Empty });
            var warrantyLinkRepository = ObjectFactory.GetInstance<Warr.Repositories.WarrantyLinkRepository>();

            var result = warrantyLinkRepository.GetItemDetails(search, product, productWarrantyLinks);
            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test]
        [Ignore("This tests are hiting the database. We need to fix this")]
        public void GetItemDetails_NoItemNo_DifferentRefCode_ReturnNoDetails()
        {
            WarrantySearchByProduct search = new WarrantySearchByProduct()
            {
                CategoryId = 2,
                Department = "PCE",
                Location = 900
            };

            var product = new WarrantyProductLinkSearch()
            {

                CashPrice = "521",
                Category = "Audio",
                CostPrice = "869",
                Description = "SONY",
                //  ItemNoWarrantyLink = "226164",
                Level_1 = "PCE",
                Level_2 = "2",
                Level_3 = "22",
                RefCodeWarrantyLink = "54",

            };

            var productWarrantyLinkViewList = new List<ProductWarrantyLinkView>();

            var productWarrantyLinkView = new ProductWarrantyLinkView()
            {
                EffectiveDate = Convert.ToDateTime("03/09/1939"),
                Id = 1,
                ItemNumber = string.Empty,
                Level1 = "PCE",
                Level2 = "2",
                Level3 = "22",
                LinkId = 136,
                LinkName = "LinkTest1",
                ProductMin = 500,
                ProductMax = 750,
                RefCode = "541",
                StoreType = string.Empty,
                WarrantyDescription = "WarrantyDescription",
                WarrantyId = 3,
                WarrantyLenght = 24,
                WarrantyNumber = "191033",
                WarrantyTaxRange = Convert.ToDecimal(17.5),
                WarrantyTypeCode = "E"
            };

            productWarrantyLinkViewList.Add(productWarrantyLinkView);
            var productWarrantyLinks = productWarrantyLinkViewList.ToLookup(p => new Tuple<decimal, decimal>(p.ProductMax, p.ProductMin));

            ObjectFactory.Inject<IProductRepository>(new MockProductRepository { path = string.Empty });
            var warrantyLinkRepository = ObjectFactory.GetInstance<Warr.Repositories.WarrantyLinkRepository>();

            var result = warrantyLinkRepository.GetItemDetails(search, product, productWarrantyLinks);
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        [Ignore("This tests are hiting the database. We need to fix this")]
        public void GetItemDetails_NoLinkLevels_ReturnItemDetails()
        {
            WarrantySearchByProduct search = new WarrantySearchByProduct()
            {
                CategoryId = 2,
                Department = "PCE",
                Location = 900
            };

            var product = new WarrantyProductLinkSearch()
            {

                CashPrice = "521",
                Category = "Audio",
                CostPrice = "869",
                Description = "SONY",
                ItemNoWarrantyLink = "226164",
                RefCodeWarrantyLink = "54",
                StockBranchProduct = 900,
                StoreType = "C",
                Level_1 = string.Empty,
                Level_2 = string.Empty,
                Level_3 = string.Empty

            };

            var productWarrantyLinkViewList = new List<ProductWarrantyLinkView>();

            var productWarrantyLinkView = new ProductWarrantyLinkView()
            {
                EffectiveDate = Convert.ToDateTime("03/09/1939"),
                Id = 1,
                ItemNumber = string.Empty,
                Level1 = string.Empty,
                Level2 = string.Empty,
                Level3 = string.Empty,
                LinkId = 136,
                LinkName = "LinkTest1",
                ProductMin = 500,
                ProductMax = 750,
                RefCode = "54",
                StoreType = string.Empty,
                WarrantyDescription = "WarrantyDescription",
                WarrantyId = 3,
                WarrantyLenght = 24,
                WarrantyNumber = "191033",
                WarrantyTaxRange = Convert.ToDecimal(17.5),
                WarrantyTypeCode = "E"
            };

            productWarrantyLinkViewList.Add(productWarrantyLinkView);
            var productWarrantyLinks = productWarrantyLinkViewList.ToLookup(p => new Tuple<decimal, decimal>(p.ProductMax, p.ProductMin));

            ObjectFactory.Inject<IProductRepository>(new MockProductRepository { path = string.Empty });
            var warrantyLinkRepository = ObjectFactory.GetInstance<Warr.Repositories.WarrantyLinkRepository>();

            var result = warrantyLinkRepository.GetItemDetails(search, product, productWarrantyLinks);
            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test]
        [Ignore("This tests are hiting the database. We need to fix this")]
        public void GetItemDetails_DifferentLinkLevels_ReturnNoDetails()
        {
            WarrantySearchByProduct search = new WarrantySearchByProduct()
            {
                CategoryId = 2,
                Department = "PCE",
                Location = 900
            };

            var product = new WarrantyProductLinkSearch()
            {

                CashPrice = "521",
                Category = "Audio",
                CostPrice = "869",
                Description = "SONY",
                ItemNoWarrantyLink = "226164",
                RefCodeWarrantyLink = "54",
                StockBranchProduct = 900,
                StoreType = "C",
                Level_1 = string.Empty,
                Level_2 = "2",
                Level_3 = string.Empty

            };

            var productWarrantyLinkViewList = new List<ProductWarrantyLinkView>();

            var productWarrantyLinkView = new ProductWarrantyLinkView()
            {
                EffectiveDate = Convert.ToDateTime("03/09/1939"),
                Id = 1,
                ItemNumber = string.Empty,
                Level1 = string.Empty,
                Level2 = "3",
                Level3 = string.Empty,
                LinkId = 136,
                LinkName = "LinkTest1",
                ProductMin = 500,
                ProductMax = 750,
                RefCode = "54",
                StoreType = string.Empty,
                WarrantyDescription = "WarrantyDescription",
                WarrantyId = 3,
                WarrantyLenght = 24,
                WarrantyNumber = "191033",
                WarrantyTaxRange = Convert.ToDecimal(17.5),
                WarrantyTypeCode = "E"
            };

            productWarrantyLinkViewList.Add(productWarrantyLinkView);
            var productWarrantyLinks = productWarrantyLinkViewList.ToLookup(p => new Tuple<decimal, decimal>(p.ProductMax, p.ProductMin));

            ObjectFactory.Inject<IProductRepository>(new MockProductRepository { path = string.Empty });
            var warrantyLinkRepository = ObjectFactory.GetInstance<Warr.Repositories.WarrantyLinkRepository>();

            var result = warrantyLinkRepository.GetItemDetails(search, product, productWarrantyLinks);
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        [Ignore("This tests are hiting the database. We need to fix this")]
        public void GetItemDetails_NoStockBranch_ReturnItemDetails()
        {
            WarrantySearchByProduct search = new WarrantySearchByProduct()
            {
                CategoryId = 2,
                Department = "PCE",
                Location = 900
            };

            var product = new WarrantyProductLinkSearch()
            {

                CashPrice = "521",
                Category = "Audio",
                CostPrice = "869",
                Description = "SONY",
                ItemNoWarrantyLink = "226164",
                Level_1 = "PCE",
                Level_2 = "2",
                Level_3 = "22",
                RefCodeWarrantyLink = "54",
                StockBranchProduct = null,
                StoreType = "C"

            };

            var productWarrantyLinkViewList = new List<ProductWarrantyLinkView>();

            var productWarrantyLinkView = new ProductWarrantyLinkView()
            {
                EffectiveDate = Convert.ToDateTime("03/09/1939"),
                Id = 1,
                ItemNumber = string.Empty,
                Level1 = "PCE",
                Level2 = "2",
                Level3 = "22",
                LinkId = 136,
                LinkName = "LinkTest1",
                ProductMin = 500,
                ProductMax = 750,
                RefCode = string.Empty,
                StoreType = string.Empty,
                WarrantyDescription = "WarrantyDescription",
                WarrantyId = 3,
                WarrantyLenght = 24,
                WarrantyNumber = "191033",
                WarrantyTaxRange = Convert.ToDecimal(17.5),
                WarrantyTypeCode = "E"
            };

            productWarrantyLinkViewList.Add(productWarrantyLinkView);
            var productWarrantyLinks = productWarrantyLinkViewList.ToLookup(p => new Tuple<decimal, decimal>(p.ProductMax, p.ProductMin));

            ObjectFactory.Inject<IProductRepository>(new MockProductRepository { path = string.Empty });
            var warrantyLinkRepository = ObjectFactory.GetInstance<Warr.Repositories.WarrantyLinkRepository>();

            var result = warrantyLinkRepository.GetItemDetails(search, product, productWarrantyLinks);
            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test]
        [Ignore("This tests are hiting the database. We need to fix this")]
        public void GetItemDetails_DifferentStockBranch_ReturnNoDetails()
        {
            WarrantySearchByProduct search = new WarrantySearchByProduct()
            {
                CategoryId = 2,
                Department = "PCE",
                Location = 900
            };

            var product = new WarrantyProductLinkSearch()
            {

                CashPrice = "521",
                Category = "Audio",
                CostPrice = "869",
                Description = "SONY",
                ItemNoWarrantyLink = "226164",
                Level_1 = "PCE",
                Level_2 = "2",
                Level_3 = "22",
                RefCodeWarrantyLink = "54",
                StockBranchNameWarrantyLink = " 900 BranchName",
                StockBranchProduct = 900,
                StoreType = "C"

            };

            var productWarrantyLinkViewList = new List<ProductWarrantyLinkView>();

            var productWarrantyLinkView = new ProductWarrantyLinkView()
            {
                EffectiveDate = Convert.ToDateTime("03/09/1939"),
                Id = 1,
                ItemNumber = string.Empty,
                Level1 = "PCE",
                Level2 = "2",
                Level3 = "22",
                LinkId = 136,
                LinkName = "LinkTest1",
                ProductMin = 500,
                ProductMax = 750,
                RefCode = string.Empty,
                StockBranch = 123,
                StoreType = string.Empty,
                WarrantyDescription = "WarrantyDescription",
                WarrantyId = 3,
                WarrantyLenght = 24,
                WarrantyNumber = "191033",
                WarrantyTaxRange = Convert.ToDecimal(17.5),
                WarrantyTypeCode = "E"
            };

            productWarrantyLinkViewList.Add(productWarrantyLinkView);
            var productWarrantyLinks = productWarrantyLinkViewList.ToLookup(p => new Tuple<decimal, decimal>(p.ProductMax, p.ProductMin));

            ObjectFactory.Inject<IProductRepository>(new MockProductRepository { path = string.Empty });
            var warrantyLinkRepository = ObjectFactory.GetInstance<Warr.Repositories.WarrantyLinkRepository>();

            var result = warrantyLinkRepository.GetItemDetails(search, product, productWarrantyLinks);
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        [Ignore("This tests are hiting the database. We need to fix this")]
        public void GetItemDetails_StockBranchAndDifferentStoreType_ReturnItemDetails()
        {
            WarrantySearchByProduct search = new WarrantySearchByProduct()
            {
                CategoryId = 2,
                Department = "PCE",
                Location = 900
            };

            var product = new WarrantyProductLinkSearch()
            {

                CashPrice = "521",
                Category = "Audio",
                CostPrice = "869",
                Description = "SONY",
                ItemNoWarrantyLink = "226164",
                Level_1 = "PCE",
                Level_2 = "2",
                Level_3 = "22",
                RefCodeWarrantyLink = "54",
                StockBranchNameWarrantyLink = "900 BridgeTown",
                StockBranchProduct = 900,
                StoreType = "C"

            };

            var productWarrantyLinkViewList = new List<ProductWarrantyLinkView>();

            var productWarrantyLinkView = new ProductWarrantyLinkView()
            {
                EffectiveDate = Convert.ToDateTime("03/09/1939"),
                Id = 1,
                ItemNumber = string.Empty,
                Level1 = "PCE",
                Level2 = "2",
                Level3 = "22",
                LinkId = 136,
                LinkName = "LinkTest1",
                ProductMin = 500,
                ProductMax = 750,
                RefCode = string.Empty,
                StockBranch = 900,
                StoreType = "CC",
                WarrantyDescription = "WarrantyDescription",
                WarrantyId = 3,
                WarrantyLenght = 24,
                WarrantyNumber = "191033",
                WarrantyTaxRange = Convert.ToDecimal(17.5),
                WarrantyTypeCode = "E"
            };

            productWarrantyLinkViewList.Add(productWarrantyLinkView);
            var productWarrantyLinks = productWarrantyLinkViewList.ToLookup(p => new Tuple<decimal, decimal>(p.ProductMax, p.ProductMin));

            ObjectFactory.Inject<IProductRepository>(new MockProductRepository { path = string.Empty });
            var warrantyLinkRepository = ObjectFactory.GetInstance<Warr.Repositories.WarrantyLinkRepository>();

            var result = warrantyLinkRepository.GetItemDetails(search, product, productWarrantyLinks);
            Assert.That(result.Count, Is.GreaterThanOrEqualTo(1));
        }

        [Test]
        [Ignore("This tests are hiting the database. We need to fix this")]
        public void GetItemDetails_NoStockBranchAndDifferentStoreType_ReturnItemDetails()
        {
            WarrantySearchByProduct search = new WarrantySearchByProduct()
            {
                CategoryId = 2,
                Department = "PCE",
                Location = 900
            };

            var product = new WarrantyProductLinkSearch()
            {

                CashPrice = "521",
                Category = "Audio",
                CostPrice = "869",
                Description = "SONY",
                ItemNoWarrantyLink = "226164",
                Level_1 = "PCE",
                Level_2 = "2",
                Level_3 = "22",
                RefCodeWarrantyLink = "54",
                StockBranchProduct = null,
                StoreType = "C"

            };

            var productWarrantyLinkViewList = new List<ProductWarrantyLinkView>();

            var productWarrantyLinkView = new ProductWarrantyLinkView()
            {
                EffectiveDate = Convert.ToDateTime("03/09/1939"),
                Id = 1,
                ItemNumber = string.Empty,
                Level1 = "PCE",
                Level2 = "2",
                Level3 = "22",
                LinkId = 136,
                LinkName = "LinkTest1",
                ProductMin = 500,
                ProductMax = 750,
                RefCode = string.Empty,
                StoreType = "CC",
                WarrantyDescription = "WarrantyDescription",
                WarrantyId = 3,
                WarrantyLenght = 24,
                WarrantyNumber = "191033",
                WarrantyTaxRange = Convert.ToDecimal(17.5),
                WarrantyTypeCode = "E"
            };

            productWarrantyLinkViewList.Add(productWarrantyLinkView);
            var productWarrantyLinks = productWarrantyLinkViewList.ToLookup(p => new Tuple<decimal, decimal>(p.ProductMax, p.ProductMin));

            ObjectFactory.Inject<IProductRepository>(new MockProductRepository { path = string.Empty });
            var warrantyLinkRepository = ObjectFactory.GetInstance<Warr.Repositories.WarrantyLinkRepository>();

            var result = warrantyLinkRepository.GetItemDetails(search, product, productWarrantyLinks);
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        [Ignore("This tests are hiting the database. We need to fix this")]
        public void GetItemDetails_NoStockBranchAndSameStoreType_ReturnItemDetails()
        {
            WarrantySearchByProduct search = new WarrantySearchByProduct()
            {
                CategoryId = 2,
                Department = "PCE",
                Location = 900
            };

            var product = new WarrantyProductLinkSearch()
            {

                CashPrice = "521",
                Category = "Audio",
                CostPrice = "869",
                Description = "SONY",
                ItemNoWarrantyLink = "226164",
                Level_1 = "PCE",
                Level_2 = "2",
                Level_3 = "22",
                RefCodeWarrantyLink = "54",
                StockBranchProduct = null,
                StoreType = "C"

            };

            var productWarrantyLinkViewList = new List<ProductWarrantyLinkView>();

            var productWarrantyLinkView = new ProductWarrantyLinkView()
            {
                EffectiveDate = Convert.ToDateTime("03/09/1939"),
                Id = 1,
                ItemNumber = string.Empty,
                Level1 = "PCE",
                Level2 = "2",
                Level3 = "22",
                LinkId = 136,
                LinkName = "LinkTest1",
                ProductMin = 500,
                ProductMax = 750,
                RefCode = string.Empty,
                StoreType = "C",
                WarrantyDescription = "WarrantyDescription",
                WarrantyId = 3,
                WarrantyLenght = 24,
                WarrantyNumber = "191033",
                WarrantyTaxRange = Convert.ToDecimal(17.5),
                WarrantyTypeCode = "E"
            };

            productWarrantyLinkViewList.Add(productWarrantyLinkView);
            var productWarrantyLinks = productWarrantyLinkViewList.ToLookup(p => new Tuple<decimal, decimal>(p.ProductMax, p.ProductMin));

            ObjectFactory.Inject<IProductRepository>(new MockProductRepository { path = string.Empty });
            var warrantyLinkRepository = ObjectFactory.GetInstance<Warr.Repositories.WarrantyLinkRepository>();

            var result = warrantyLinkRepository.GetItemDetails(search, product, productWarrantyLinks);
            Assert.That(result.Count, Is.EqualTo(1));
        }

        #region Helper classes

        public class WarrantyPromotionRepository : IWarrantyPromotionRepository
        {
            #region IWarrantyPromotionRepository Members

            public void Delete(int promotionId)
            {
                throw new NotImplementedException();
            }

            public List<Warr.Model.PromotionAggregate> GetPromotionAggregate(int branch, DateTime? date)
            {
                var path = @"CsvSources\Warranty\WarrantySimulator\SearchByProduct\{0}";
                var promotionAggregateList = CvsReader.Reader<Warr.Model.PromotionAggregate>.Read(string.Format(path, "PromotionAggregate.csv"), new CsvConfiguration
                {
                    WillThrowOnMissingField = false
                }).ToList()
                  .Where(e => e.BranchNumber == branch).ToList();

                return promotionAggregateList;
            }

            public Data.IPagedSearchResults<Warr.Model.WarrantyPromotionSettings> GetPromotions(Warr.Promotions.Filter filterValues)
            {
                throw new NotImplementedException();
            }

            public List<Warr.Model.PromotionBasic> GetPromotions(IEnumerable<Warr.Model.WarrantyLocation> warrantyLocation)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<Warr.Model.WarrantyPromotionSettings> GetPromotionsForWarranty(int warrantyId, DateTime? endDate, IEnumerable<int> warrantyPriceId = null, bool getCurrentPromotions = false)
            {
                throw new NotImplementedException();
            }

            public Warr.Model.WarrantyPromotionSettings Save(Warr.Model.WarrantyPromotionSettings promotion)
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        public class WarrantyPriceRepository : IWarrantyPriceRepository
        {
            #region IWarrantyPriceRepository Members

            public Warr.Model.WarrantyLocationPrice Delete(int id)
            {
                throw new NotImplementedException();
            }

            public bool DeleteBulkEdit(int bulkEditId)
            {
                throw new NotImplementedException();
            }

            public string GetBulkEditInfo(int[] filteredIds, Warr.Model.WarrantyEditRequest editRequest)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<Warr.Model.WarrantyPrice> GetWarrantyPrices(IEnumerable<Warr.Model.WarrantyLocation> warrantyLocation)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<Warr.Model.WarrantyCalculatedPrice> GetWarrantyPrices(IEnumerable<int> warrantyIds, short branch, DateTime? date)
            {
                var path = @"CsvSources\Warranty\WarrantySimulator\SearchByProduct\{0}";
                var warrantyPrices = CvsReader.Reader<Warr.Model.WarrantyCalculatedPrice>.Read(string.Format(path, "PriceCalcView.csv"), new CsvConfiguration
                {
                    WillThrowOnMissingField = false
                }).ToList()
                    .Where(e => e.BranchNumber == branch).ToList();

                return warrantyPrices;
            }

            public IEnumerable<Warr.Model.WarrantyLocationPrice> GetWarrantyPrices(int warrantyId)
            {
                throw new NotImplementedException();
            }

            public void InsertBulkEdit(int[] filteredIds, Warr.Model.WarrantyEditRequest editRequest)
            {
                throw new NotImplementedException();
            }

            public Warr.Model.WarrantyLocationPrice Save(Warr.Model.WarrantyLocationPrice warrantyPrice)
            {
                throw new NotImplementedException();
            }

            public void DeletePriceCalcViewCache()
            {
                throw new NotImplementedException();
            }
            #endregion
        }

        public class MockProductRepository : IProductRepository
        {
            public string path { get; set; }

            #region IProductRepository Members

            public IList<Stock.Models.Product> GetAll()
            {
                throw new NotImplementedException();
            }

            public IList<string> GetStockItemForValidation(Stock.WarrantyProductLinkSearch[] productSearch)
            {
                throw new NotImplementedException();
            }

            public IList<Stock.Models.Installation> GetInstallations(string itemNumber, short location)
            {
                throw new NotImplementedException();
            }

            public Stock.Models.Product Convert(Stock.StockItemView stock)
            {
                throw new NotImplementedException();
            }

            public Stock.StockItemViewRelations GetProductRelationsByItemNumber(string itemNumber)
            {
                throw new NotImplementedException();
            }

            public Stock.WarrantyProductLinkSearch FindMatchingProduct(string productNumber, short branchNumber)
            {
                var product = CvsReader.Reader<Merch.WarrantyProductLinkSearch>.Read(string.Format(path, "products.csv"), new CsvConfiguration
                {
                    WillThrowOnMissingField = false
                }).ToList()
                 .Where(e => e.ItemNoWarrantyLink.Trim() == productNumber.Trim() && e.StockBranchProduct == branchNumber)
                 .FirstOrDefault();

                return product;
            }

            public List<Stock.WarrantyProductLinkSearch> GetProductsByCategory(string department, short category, short branchNumber)
            {
                var products = CvsReader.Reader<Merch.WarrantyProductLinkSearch>.Read(string.Format(path, "products.csv"), new CsvConfiguration
                {
                    WillThrowOnMissingField = false
                }).ToList()
                .Where(e => e.Level_1 == department && e.Level_2 == category.ToString() && e.StockBranchProduct == branchNumber)
                .ToList();

                return products;
            }

            #endregion
        }

        public class Registry : StructureMap.Configuration.DSL.Registry
        {
            public Registry()
            {
                For<IWarrantyPromotionRepository>().Use<WarrantySimulator.WarrantyPromotionRepository>();
                For<IWarrantyPriceRepository>().Use<WarrantySimulator.WarrantyPriceRepository>();
            }
        }

        #endregion
    }
}