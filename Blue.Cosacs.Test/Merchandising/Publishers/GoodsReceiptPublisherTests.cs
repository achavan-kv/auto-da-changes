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
    public class GoodsReceiptPublisherTests
    {
        private Fixture fixture;
        private IGoodsReceiptPublisher publisher;
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

            publisher = new GoodsReceiptPublisher(basePublisher, mockProductRepo.Object);
        }

        private List<GoodsReceiptPurchaseOrderViewModel> GeneratePurchaseOrders()
        {
            // Creating 3 PO's, each with 3 products. 
            var productFixture =
                fixture.Build<GoodsReceiptPurchaseOrderViewModel>()
                    .With(
                        x => x.Products,
                        new List<GoodsReceiptProductViewModel>()
                            {
                                new GoodsReceiptProductViewModel()
                                    {
                                        ProductId = 1,
                                        QuantityReceived = 3,
                                        LastLandedCost = 10
                                    }
                            })
                    .CreateMany(3)
                    .ToList();

            // No product messages
            productFixture.Add(
                fixture.Build<GoodsReceiptPurchaseOrderViewModel>()
                    .With(
                        x => x.Products,
                        new List<GoodsReceiptProductViewModel>()
                            {
                                new GoodsReceiptProductViewModel()
                                    {
                                        ProductId = 2,
                                        QuantityReceived = 3,
                                        LastLandedCost = 10
                                    }
                            })
                    .Create());

            // No quantity received
            productFixture.Add(
                fixture.Build<GoodsReceiptPurchaseOrderViewModel>()
                    .With(
                        x => x.Products,
                        new List<GoodsReceiptProductViewModel>()
                            {
                                new GoodsReceiptProductViewModel()
                                    {
                                        ProductId = 3,
                                        QuantityReceived = 0,
                                        LastLandedCost = 10
                                    }
                            })
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
            var noMessagePurchaseOrders = GeneratePurchaseOrders()
                .Where(p => p.Products.Any(x => x.ProductId == 2)).ToList();

            Assert.Catch(() => publisher.PublishCreated(
                new GoodsReceiptViewModel()
                    {
                        PurchaseOrders = noMessagePurchaseOrders
                    }));
        }

        [Test]
        public void PublishCreated_Message_ShouldAggregateCount()
        {
            publisher.PublishCreated(new GoodsReceiptViewModel() { PurchaseOrders = GeneratePurchaseOrders() });
            Assert.AreEqual(basePublisher.GenericMessages.First().Products.Count(), 1);
        }

        [Test]
        public void PublishCreated_Message_ShouldAggregateQuantity()
        {
            publisher.PublishCreated(new GoodsReceiptViewModel() { PurchaseOrders = GeneratePurchaseOrders() });
            Assert.AreEqual(basePublisher.GenericMessages.First().Products.Sum(x => x.Units), 9);
        }

        [Test]
        public void PublishCreated_Message_ShouldAggregateCost()
        {
            publisher.PublishCreated(new GoodsReceiptViewModel() { PurchaseOrders = GeneratePurchaseOrders() });
            Assert.AreEqual(basePublisher.GenericMessages.First().Products.Sum(x => x.Cost), 90);
        }

        [Test]
        public void PublishCreated_Message_ShouldHaveCorrectType()
        {
            publisher.PublishCreated(new GoodsReceiptViewModel() { PurchaseOrders = GeneratePurchaseOrders() });
            Assert.AreEqual("Merchandising.GoodsReceiptCreated", basePublisher.Routing);
        }
    }
}