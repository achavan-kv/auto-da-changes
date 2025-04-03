namespace Blue.Cosacs.Merchandising.Publishers
{
    using AutoMapper;
    using Blue.Cosacs.Merchandising.Mappers;
    using Blue.Cosacs.Merchandising.Models;
    using Blue.Cosacs.Merchandising.Repositories;
    using Blue.Cosacs.Messages.Merchandising.CintOrderReceipt;
    using Blue.Cosacs.Test.Mocks;
    using Moq;
    using NUnit.Framework;
    using Ploeh.AutoFixture;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public class CintOrderPublisherTests
    {
        private const string TestSku = "11111";
        private const string SkuNoMessages = "22222";
        private const string SkuNoCosts = "33333";
        private ICintOrderPublisher publisher;
        private MockGenericPublisher<CintOrderReceiptMessage> basePublisher;

        [SetUp]
        public void SetUp()
        {
            // Setup automapper mappings
            Mapper.Initialize(
              cfg =>
              {
                  cfg.AddProfile<MerchandisingAutomapperProfile>();
              });

            // Setup fixtures
            var fixture = new Fixture();
            var productFixture = fixture.Build<Product>()
                .With(x => x.Id, 1)
                .Create();
            var productFixture2 = fixture.Build<Product>()
              .With(x => x.Id, 2)
              .Create();
            var productFixture3 = fixture.Build<Product>()
            .With(x => x.Id, 3)
            .Create();
            var productMessageFixtures = fixture.CreateMany<ProductMessageView>(1).ToList();

            // Setup mocks
            var mockCostRepo = new Mock<ICostRepository>();
            var mockProductRepo = new Mock<IProductRepository>();

            mockProductRepo.Setup(x => x.GetBySku(TestSku)).Returns(productFixture);
            mockProductRepo.Setup(x => x.GetBySku(SkuNoMessages)).Returns(productFixture2);
            mockProductRepo.Setup(x => x.GetBySku(SkuNoCosts)).Returns(productFixture3);
            mockProductRepo.Setup(x => x.GetProductMessages(new List<int> { 1 })).Returns(productMessageFixtures);
            mockProductRepo.Setup(x => x.GetProductMessages(new List<int> { 3 })).Returns(productMessageFixtures);
            mockProductRepo.Setup(x => x.GetProductMessages(new List<int> { 2 })).Returns(new List<ProductMessageView>());
            mockCostRepo.Setup(x => x.GetLatestByProduct(1)).Returns(new CostPriceModel() { AverageWeightedCost = 2 });
            basePublisher = new MockGenericPublisher<CintOrderReceiptMessage>();

            publisher = new CintOrderPublisher(basePublisher, mockCostRepo.Object, mockProductRepo.Object);
        }

        [Test]
        public void PublishDelivered_NullCintOrder_ShouldError()
        {
            Assert.Catch(() => publisher.PublishDelivered(null, null, 0));
        }

        [Test]
        public void PublishCostOfSale_NullCintOrder_ShouldError()
        {
            Assert.Catch(() => publisher.PublishCostOfSale(null, null, 0));
        }

        [Test]
        public void PublishDelivered_NoMessagesFound_ShouldError()
        {
            Assert.Catch(() => publisher.PublishDelivered(new Model.CintOrder() { Sku = SkuNoMessages }, null, 0), null);
        }

        [Test]
        public void PublishCostOfSale_NoMessagesFound_ShouldError()
        {
            Assert.Catch(() => publisher.PublishCostOfSale(new Model.CintOrder() { Sku = SkuNoMessages }, null, 0));
        }

        //[Test]
        //public void PublishDelivered_NoCost_AWCShouldBeZero()
        //{
        //    publisher.PublishDelivered(new CintOrder() { Sku = SkuNoCosts });
        //    Assert.AreEqual(basePublisher.GenericMessages.First().TotalAWC, 0M);
        //}

        //[Test]
        //public void PublishCostOfSale_NoCost_AWCShouldBeZero()
        //{
        //    publisher.PublishCostOfSale(new CintOrder() { Sku = SkuNoCosts });
        //    Assert.AreEqual(basePublisher.GenericMessages.First().TotalAWC, 0M);
        //}

        //[Test]
        //public void PublishDelivered_AWC_ShouldBeCostTimesQuantity()
        //{
        //    publisher.PublishDelivered(new CintOrder() { Sku = TestSku, Quantity = 5 });
        //    Assert.AreEqual(basePublisher.GenericMessages.First().TotalAWC, 10M);
        //}

        //[Test]
        //public void PublishCostOfSale_AWC_ShouldBeCostTimesQuantity()
        //{
        //    publisher.PublishCostOfSale(new CintOrder() { Sku = TestSku, Quantity = 5 });
        //    Assert.AreEqual(basePublisher.GenericMessages.First().TotalAWC, 10M);
        //}

        //[Test]
        //public void PublishDelivered_AWC_ShouldHaveCorrectType()
        //{
        //    publisher.PublishDelivered(new CintOrder() { Sku = TestSku, Quantity = 5 });
        //    Assert.AreEqual("Merchandising.CintOrderDelivered", basePublisher.Routing);
        //}

        //[Test]
        //public void PublishCostOfSale_AWC_ShouldHaveCorrectType()
        //{
        //    publisher.PublishCostOfSale(new CintOrder() { Sku = TestSku, Quantity = 5 });
        //    Assert.AreEqual("Merchandising.CintOrderReturned", basePublisher.Routing);
        //}
    }
}