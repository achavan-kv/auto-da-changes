using Blue.Cosacs.Stock;
using Blue.Cosacs.Stock.Repositories;
using Blue.Cosacs.Warranty;
using Blue.Cosacs.Warranty.Model;
using Blue.Cosacs.Warranty.Repositories;
using Blue.Linq;
using Blue.Transactions;
using CsvHelper.Configuration;
using Moq;
using NUnit.Framework;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Merch = Blue.Cosacs.Stock;
using Warr = Blue.Cosacs.Warranty;

namespace Blue.Cosacs.Test.Warranty
{
    [TestFixture]
    class WarrantyProductLink
    {
        const string PATH = @"CsvSources\Warranty\WarrantyProductLink\ValidateWarrantyLink\{0}";

        [TestFixtureSetUp]
        public virtual void Setup()
        {
            ObjectFactory.Initialize(p =>
            {
                p.AddRegistry(new Blue.Cosacs.Test.Warranty.Registry());
                p.AddRegistry(new WarrantyProductLink.Registry());
            });
        }


        //All the Test Methods are testing -> "internal string ValidateWarrantyLink(WarrantyLinkProduct productSearch, string type)" method
        //of WarrantyLinkRepository.cs with different different inputs

        [Test]
        public void Validate_ItemCodeStoreTypeHaveExtended_InstantIsAppliedToItemCodeStoreType_ShouldNotAllow()
        {
            ValidateWarrantyLink_SetupMockData(PATH);

            var warrantyLinkRepository = ObjectFactory.GetInstance<WarrantyLinkRepository>();

            var warrantyLinkProduct = new WarrantyLinkProduct()
            {
                ItemNoWarrantyLink = "CC704",
                StoreType = "C" //Courts
            };

            if (String.IsNullOrEmpty(warrantyLinkRepository.ValidateWarrantyLink(warrantyLinkProduct, "I")))
                Assert.True(false);
            else
                Assert.True(true);
        }

        [Test]
        [TestCase("F")]
        [TestCase("E")]
        public void Validate_ItemCodeStoreTypeHaveExtended_ExtendedIsAppliedToItemCodeStoreType_ShouldAllow(string warrantyType)
        {
            ValidateWarrantyLink_SetupMockData(PATH);

            var warrantyLinkRepository = ObjectFactory.GetInstance<WarrantyLinkRepository>();

            var warrantyLinkProduct = new WarrantyLinkProduct()
            {
                ItemNoWarrantyLink = "CC704",
                StoreType = "C", //Courts             
            };

            if (String.IsNullOrEmpty(warrantyLinkRepository.ValidateWarrantyLink(warrantyLinkProduct, warrantyType)))
                Assert.True(true);
            else
                Assert.True(false);
        }

        [Test]
        public void Validate_DepartmentStoreTypeHaveInstant_ExtendedIsAppliedToDepartmentCategory_ShouldNotAllow()
        {
            ValidateWarrantyLink_SetupMockData(PATH);

            var warrantyLinkRepository = ObjectFactory.GetInstance<WarrantyLinkRepository>();

            var warrantyLinkProduct = new WarrantyLinkProduct()
            {
                Level_1 = "PCE", //Electrical
                Level_2 = "1",  //Vision
            };

            if (String.IsNullOrEmpty(warrantyLinkRepository.ValidateWarrantyLink(warrantyLinkProduct, "E")))
                Assert.True(false);
            else
                Assert.True(true);
        }

        [Test]
        [TestCase("F")]
        [TestCase("I")]
        public void Validate_DepartmentStoreTypeHaveInstant_InstantIsAppliedToDepartmentCategory_ShouldAllow(string warrantyType)
        {
            ValidateWarrantyLink_SetupMockData(PATH);

            var warrantyLinkRepository = ObjectFactory.GetInstance<WarrantyLinkRepository>();

            var warrantyLinkProduct = new WarrantyLinkProduct()
            {
                Level_1 = "PCE", //Electrical
                Level_2 = "1",  //Vision
            };

            if (String.IsNullOrEmpty(warrantyLinkRepository.ValidateWarrantyLink(warrantyLinkProduct, warrantyType)))
                Assert.True(true);
            else
                Assert.True(false);
        }

        [Test]
        public void Validate_DepartmentCategoryStoreHaveExtended_InstantIsAppliedToDepartmentCategoryClassStore_ShouldNotAllow()
        {
            ValidateWarrantyLink_SetupMockData(PATH);

            var warrantyLinkRepository = ObjectFactory.GetInstance<WarrantyLinkRepository>();

            var warrantyLinkProduct = new WarrantyLinkProduct()
            {
                Level_1 = "PCF", //Furniture
                Level_2 = "50", //Lounge
                Level_3 = "K1",
                StoreType = "N" //Non Courts
            };

            if (String.IsNullOrEmpty(warrantyLinkRepository.ValidateWarrantyLink(warrantyLinkProduct, "I")))
                Assert.True(false);
            else
                Assert.True(true);
        }

        [Test]
        [TestCase("F")]
        [TestCase("E")]
        public void Validate_DepartmentCategoryStoreHaveExtendedWarranty_ExtendedWarrantyIsAppliedToDepartmentCategoryClassStore_ShouldAllow(string warrantyType)
        {
            ValidateWarrantyLink_SetupMockData(PATH);

            var warrantyLinkRepository = ObjectFactory.GetInstance<WarrantyLinkRepository>();

            var warrantyLinkProduct = new WarrantyLinkProduct()
            {
                Level_1 = "PCF",    //Furniture
                Level_2 = "50",     //Lounge
                Level_3 = "K1",
                StoreType = "N"     //Non Courts
            };

            if (String.IsNullOrEmpty(warrantyLinkRepository.ValidateWarrantyLink(warrantyLinkProduct, warrantyType)))
                Assert.True(true);
            else
                Assert.True(false);
        }

        [Test]
        public void Validate_ItemCodeStoreTypeHaveExtended_InstantIsAppliedToDepartmentStore_ShouldNotAllow()
        {
            ValidateWarrantyLink_SetupMockData(PATH);

            var warrantyLinkRepository = ObjectFactory.GetInstance<WarrantyLinkRepository>();

            var warrantyLinkProduct = new WarrantyLinkProduct()
            {
                Level_1 = "PCF",    //Furniture
                StoreType = "C"     //Courts
            };

            if (String.IsNullOrEmpty(warrantyLinkRepository.ValidateWarrantyLink(warrantyLinkProduct, "I")))
                Assert.True(false);
            else
                Assert.True(true);
        }

        [Test]
        [TestCase("F")]
        [TestCase("E")]
        public void Validate_ItemCodeStoreTypeHaveExtended_ExtendedIsAppliedToDepartmentStore_ShouldAllow(string warrantyType)
        {
            ValidateWarrantyLink_SetupMockData(PATH);

            var warrantyLinkRepository = ObjectFactory.GetInstance<WarrantyLinkRepository>();

            var warrantyLinkProduct = new WarrantyLinkProduct()
            {
                Level_1 = "PCF",    //Furniture
                StoreType = "C"     //Courts
            };

            if (String.IsNullOrEmpty(warrantyLinkRepository.ValidateWarrantyLink(warrantyLinkProduct, warrantyType)))
                Assert.True(true);
            else
                Assert.True(false);
        }

        [Test]
        [TestCase("I")]
        [TestCase("E")]
        public void Validate_ItemCodeStoreTypeHaveExtendedAndDepartmentStoreHaveInstant_InstantOrExtendedIsAppliedToStore_ShouldNotAllow(string warrantyType)
        {
            ValidateWarrantyLink_SetupMockData(PATH);

            var warrantyLinkRepository = ObjectFactory.GetInstance<WarrantyLinkRepository>();

            var warrantyLinkProduct = new WarrantyLinkProduct()
            {
                StoreType = "C"
            };

            if (String.IsNullOrEmpty(warrantyLinkRepository.ValidateWarrantyLink(warrantyLinkProduct, warrantyType)))
                Assert.True(false);
            else
                Assert.True(true);
        }

        [Test]       
        public void Validate_ItemCodeStoreTypeHaveExtended_InstantIsAppliedToRefNo_ShouldNotAllow()
        {
            ValidateWarrantyLink_SetupMockData(PATH);

            var warrantyLinkRepository = ObjectFactory.GetInstance<WarrantyLinkRepository>();

            var warrantyLinkProduct = new WarrantyLinkProduct()
            {
                RefCodeWarrantyLink = "67"
            };

            if (String.IsNullOrEmpty(warrantyLinkRepository.ValidateWarrantyLink(warrantyLinkProduct, "I")))
                Assert.True(false);
            else
                Assert.True(true);
        }

        [Test]
        [TestCase("F")]
        [TestCase("E")]
        public void Validate_ItemCodeStoreTypeHaveExtended_ExtendedIsAppliedToRefNo_ShouldAllow(string warrantyType)
        {
            ValidateWarrantyLink_SetupMockData(PATH);

            var warrantyLinkRepository = ObjectFactory.GetInstance<WarrantyLinkRepository>();

            var warrantyLinkProduct = new WarrantyLinkProduct()
            {
                RefCodeWarrantyLink = "67"
            };

            if (String.IsNullOrEmpty(warrantyLinkRepository.ValidateWarrantyLink(warrantyLinkProduct, warrantyType)))
                Assert.True(true);
            else
                Assert.True(false);
        }


        [Test]
        public void Validate_DepartmentStoreBranchHaveExtended_InstantIsAppliedToStore_ShouldNotAllow()
        {
            ValidateWarrantyLink_SetupMockData(PATH);

            var warrantyLinkRepository = ObjectFactory.GetInstance<WarrantyLinkRepository>();

            var warrantyLinkProduct = new WarrantyLinkProduct()
            {
                StockBranchNameWarrantyLink = 745
            };

            if (String.IsNullOrEmpty(warrantyLinkRepository.ValidateWarrantyLink(warrantyLinkProduct, "I")))
                Assert.True(false);
            else
                Assert.True(true);
        }

        private void ValidateWarrantyLink_SetupMockData(string path)
        {
            var mockReadScopeForWarranty = new Mock<ReadScope<Warr.Context>>();
            var mockContextForWarranty = new Mock<Warr.Context>();
            mockReadScopeForWarranty.Setup(p => p.Context).Returns(mockContextForWarranty.Object);
            ObjectFactory.Inject<ReadScope<Warr.Context>>(mockReadScopeForWarranty.Object);


            var linkWarrantyTypeView = CvsReader.Reader<Warr.LinkWarrantyTypeView>.Read(string.Format(path, @"LinkWarrantyTypeView.csv"))
                .AsQueryable();
            var mockLinkWarrantyTypeView = new Mock<DbSet<Warr.LinkWarrantyTypeView>>();
            mockLinkWarrantyTypeView.SetuptQuerableMethods(linkWarrantyTypeView);
            mockContextForWarranty.Setup(c => c.LinkWarrantyTypeView).Returns(mockLinkWarrantyTypeView.Object);                        

            var mockReadScopeForMerchandising = new Mock<ReadScope<Merch.Context>>();
            var mockContextForMerchandising = new Mock<Merch.Context>();
            mockReadScopeForMerchandising.Setup(p => p.Context).Returns(mockContextForMerchandising.Object);
            ObjectFactory.Inject<ReadScope<Merch.Context>>(mockReadScopeForMerchandising.Object);          

            var queryableData = CvsReader.Reader<Merch.ProductLinkValidateView>.Read(string.Format(path, @"ProductLinkValidateView.csv"))
                .AsQueryable();
            var mockDbSet = new Mock<DbSet<Merch.ProductLinkValidateView>>();
            mockDbSet.SetuptQuerableMethods(queryableData);
            var q = mockDbSet.As<IQueryable<Merch.ProductLinkValidateView>>();
            q.Setup(m => m.GetEnumerator()).Returns(() => queryableData.GetEnumerator());
            mockContextForMerchandising.Setup(c => c.ProductLinkValidateView).Returns(mockDbSet.Object);
        }

        #region Helper Classes

        public class Registry : StructureMap.Configuration.DSL.Registry
        {
            public Registry()
            {
                For<IWarrantyPromotionRepository>().Use<WarrantySimulator.WarrantyPromotionRepository>();
                For<IWarrantyPriceRepository>().Use<WarrantySimulator.WarrantyPriceRepository>();
                For<IProductRepository>().Use<ProductRepository>();
            }
        }

        #endregion
    }
}
