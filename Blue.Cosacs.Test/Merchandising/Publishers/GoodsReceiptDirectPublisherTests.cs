namespace Blue.Cosacs.Merchandising.Publishers
{
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper;
    using Blue.Cosacs.Merchandising.Mappers;
    using Blue.Cosacs.Merchandising.Models;
    using Blue.Cosacs.Merchandising.Repositories;
    using Blue.Cosacs.Messages.Merchandising.GoodsReceipt;
    using Blue.Cosacs.Test.Mocks;
    using Moq;
    using NUnit.Framework;
    using Ploeh.AutoFixture;

    [TestFixture]
    public class GoodsReceiptDirectPublisherTests
    {
        private Fixture fixture;
        private IGoodsReceiptDirectPublisher publisher;
        private MockGenericPublisher<GoodsReceiptMessage> basePublisher;

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
            fixture = new Fixture();
            var productMessageFixtures = fixture.Build<ProductMessageView>()
                .With(x => x.Id, 1)
                .CreateMany(1)
                .ToList();

            // Setup mocks
            var mockProductRepo = new Mock<IProductRepository>();
            mockProductRepo.Setup(x => x.GetProductMessages(new List<int> { 1, 2 })).Returns(productMessageFixtures);
            mockProductRepo.Setup(x => x.GetProductMessages(new List<int> { 2 })).Returns(new List<ProductMessageView>());
            basePublisher = new MockGenericPublisher<GoodsReceiptMessage>();

            publisher = new GoodsReceiptDirectPublisher(basePublisher, mockProductRepo.Object);
        }

        private List<GoodsReceiptDirectProduct> GenerateProducts()
        {
            var productFixture = fixture.Build<GoodsReceiptDirectProduct>()
             .With(x => x.ProductId, 1)
             .With(x => x.UnitLandedCost, 10)
             .With(x => x.QuantityReceived, 3)
             .CreateMany(3)
             .ToList();

            productFixture.Add(fixture.Build<GoodsReceiptDirectProduct>()
              .With(x => x.ProductId, 2)
              .With(x => x.UnitLandedCost, 10)
              .With(x => x.QuantityReceived, 3)
              .Create());

            return productFixture;
        }
            
        [Test]
        public void PublishCreated_NullCintOrder_ShouldError()
        {
            Assert.Catch(() => publisher.PublishCreated(null));
        }

        [Test]
        public void PublishCreated_NoProductMessages_ShouldError()
        {
            var productFixture = fixture.Build<GoodsReceiptDirectProduct>()
               .With(x => x.ProductId, 2)
               .Create();
            Assert.Catch(() => publisher.PublishCreated(new GoodsReceiptDirectViewModel() { Products = { productFixture } }));
        }

        [Test]
        public void PublishCreated_Message_ShouldAggregateCount()
        {
            var productFixture = GenerateProducts();
            publisher.PublishCreated(new GoodsReceiptDirectViewModel() { Products = productFixture });
            Assert.AreEqual(basePublisher.GenericMessages.First().Products.Count(), 1);
        }

        [Test]
        public void PublishCreated_Message_ShouldAggregateQuantity()
        {
            var productFixture = GenerateProducts();
            publisher.PublishCreated(new GoodsReceiptDirectViewModel() { Products = productFixture });
            Assert.AreEqual(basePublisher.GenericMessages.First().Products.Sum(x => x.Units), 9);
        }

        [Test]
        public void PublishCreated_Message_ShouldAggregateCost()
        {
            var productFixture = GenerateProducts();
            publisher.PublishCreated(new GoodsReceiptDirectViewModel() { Products = productFixture });
            Assert.AreEqual(basePublisher.GenericMessages.First().Products.Sum(x => x.Cost), 90);
        }

        [Test]
        public void PublishCreated_Message_ShouldHaveCorrectType()
        {
            var productFixture = GenerateProducts();
            publisher.PublishCreated(new GoodsReceiptDirectViewModel() { Products = productFixture });
            Assert.AreEqual("Merchandising.GoodsReceiptCreated", basePublisher.Routing);
        }
    }
}