namespace Blue.Cosacs.Merchandising.Publishers
{
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper;
    using Blue.Cosacs.Merchandising.Mappers;
    using Blue.Cosacs.Merchandising.Models;
    using Blue.Cosacs.Merchandising.Repositories;
    using Blue.Cosacs.Messages.Merchandising.Transfer;
    using Blue.Cosacs.Messages.Warehouse;
    using Blue.Cosacs.Test.Mocks;
    using Moq;
    using NUnit.Framework;

    using Ploeh.AutoFixture;

    [TestFixture]
    public class StockAllocationPublisherTests
    {
        private Fixture fixture;
        private Mock<IProductRepository> mockProductRepo;
        private Mock<ILocationRepository> mockLocationRepo;
        private MockGenericPublisher<BookingSubmit> baseCreatedPublisher;
        private MockGenericPublisher<TransferMessage> baseReceivedPublisher;

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

            var receivingLocationFixture =
                fixture.Build<LocationModel>()
                    .With(x => x.Id, 1)
                    .With(x => x.SalesId, "111")
                    .With(x => x.AddressLine1, "RAddressLine1")
                    .With(x => x.AddressLine2, "RAddressLine2")
                    .With(x => x.PostCode, "Code1")
                    .With(x => x.Fascia, "Courts")
                    .Create();

            var warehouseLocationFixture =
               fixture.Build<LocationModel>()
                   .With(x => x.Id, 2)
                   .With(x => x.SalesId, "222")
                   .With(x => x.AddressLine1, "WAddressLine1")
                   .With(x => x.AddressLine2, "WAddressLine2")
                   .With(x => x.PostCode, "Code2")
                   .With(x => x.Fascia, "NonCourts")
                   .Create();

            // Setup mocks
            mockProductRepo = new Mock<IProductRepository>();
            mockLocationRepo = new Mock<ILocationRepository>();
            mockProductRepo.Setup(x => x.GetProductMessages(new List<int> { 1 })).Returns(productMessageFixtures);
            mockProductRepo.Setup(x => x.GetProductMessages(new List<int> { 2 })).Returns(new List<ProductMessageView>());
            mockLocationRepo.Setup(x => x.Get(It.IsAny<List<int>>(), false)).Returns(new List<LocationModel>() { receivingLocationFixture, warehouseLocationFixture });

            baseCreatedPublisher = new MockGenericPublisher<BookingSubmit>();
            baseReceivedPublisher = new MockGenericPublisher<TransferMessage>();
        }

        private StockAllocationProductViewModel GetBasicProductFixture(int id)
        {
            return
               fixture.Build<StockAllocationProductViewModel>()
                   .With(x => x.ProductId, id)
                   .With(x => x.AverageWeightedCost, 10)
                   .With(x => x.ReceivingLocationId, 1)
                   .With(x => x.WarehouseLocationId, 2)
                   .With(x => x.Quantity, 10)
                   .Create();
        }

        private List<StockAllocationProductViewModel> GetBasicProductFixtureList(int id, int locId = 1, int wareId = 2)
        {
            return
               fixture.Build<StockAllocationProductViewModel>()
                   .With(x => x.ProductId, id)
                   .With(x => x.AverageWeightedCost, 10)
                   .With(x => x.Quantity, 10)
                   .With(x => x.ReceivingLocationId, locId)
                   .With(x => x.WarehouseLocationId, wareId)
                   .CreateMany(3)
                   .ToList();
        }

        // Publish Created
         private StockAllocationPublisher GetCreatedPublisher()
        {
            return new StockAllocationPublisher(baseCreatedPublisher, mockLocationRepo.Object, mockProductRepo.Object);
        }

        [Test]
        public void PublishCreated_NullCintOrder_ShouldError()
        {
            var publisher = GetCreatedPublisher();
            Assert.Catch(() => publisher.PublishCreated(null));
        }

        [Test]
        public void PublishCreated_InvalidReceivingLocation_ShouldError()
        {
            var publisher = GetCreatedPublisher();
            var productFixture = fixture.Build<StockAllocationProductViewModel>()
                 .With(x => x.ProductId, 1)
                 .With(x => x.AverageWeightedCost, 10)
                 .With(x => x.Quantity, 10)
                 .With(x => x.ReceivingLocationId, 3)
                 .With(x => x.WarehouseLocationId, 2)
                 .CreateMany(3)
                 .ToList();
            Assert.Catch(() => publisher.PublishCreated(productFixture));
        }

        [Test]
        public void PublishCreated_InvalidWarehouse_ShouldError()
        {
            var publisher = GetCreatedPublisher();
            var productFixture = fixture.Build<StockAllocationProductViewModel>()
                 .With(x => x.ProductId, 1)
                 .With(x => x.AverageWeightedCost, 10)
                 .With(x => x.Quantity, 10)
                 .With(x => x.ReceivingLocationId, 1)
                 .With(x => x.WarehouseLocationId, 3)
                 .CreateMany(3)
                 .ToList();
            Assert.Catch(() => publisher.PublishCreated(productFixture));
        }

        [Test]
        public void PublishCreated_Message_ShouldMatchQuantity()
        {
            var publisher = GetCreatedPublisher();
            publisher.PublishCreated(GetBasicProductFixtureList(1));
            Assert.AreEqual(baseCreatedPublisher.GenericMessages.First().Quantity, 10);
        }

        [Test]
        public void PublishCreated_CourtsLocation_ShouldMapFasciaAsC()
        {
            var publisher = GetCreatedPublisher();

            // Courts -> C
            var prodfixture = GetBasicProductFixtureList(1);
            publisher.PublishCreated(prodfixture);

            var result = baseCreatedPublisher.GenericMessages.First();
            Assert.AreEqual("C", result.Fascia);
        }

        [Test]
        public void PublishCreated_NonCourtsLocation_ShouldMapFasciaAsN()
        {
            var publisher = GetCreatedPublisher();
           
            // Non Courts -> N
            baseCreatedPublisher.GenericMessages.Clear();
            var prodfixture = GetBasicProductFixtureList(1, 2, 1);
            publisher.PublishCreated(prodfixture);

            var result = baseCreatedPublisher.GenericMessages.First();
            Assert.AreEqual("N", result.Fascia);
        }

        [Test]
        public void PublishCreated_ShouldMapProperties()
        {
            var publisher = GetCreatedPublisher();
            var prodfixture = GetBasicProductFixtureList(1);
            var first = prodfixture.First();

            publisher.PublishCreated(prodfixture);
            
            var result = baseCreatedPublisher.GenericMessages.First();
            Assert.AreEqual("RAddressLine1", result.AddressLine1);  
            Assert.AreEqual("RAddressLine2", result.AddressLine2);  
            Assert.AreEqual("Code1", result.PostCode);
            Assert.AreEqual(first.CreatedDate, result.RequestedDate);
            Assert.AreEqual(first.CreatedDate, result.OrderedOn);
            Assert.AreEqual(first.CreatedById, result.CreatedBy);
            Assert.AreEqual(first.BookingId, result.Id);
            Assert.AreEqual(first.Sku, result.SKU);
            Assert.AreEqual(first.ProductId, result.ItemId);
            Assert.AreEqual(first.Description, result.ProductDescription);
            Assert.AreEqual(first.Quantity, result.Quantity);
            Assert.AreEqual(first.AverageWeightedCost, result.UnitPrice);
            Assert.AreEqual(first.CorporateUPC.SafeSubstring(0, 18), result.ItemUPC);
            Assert.AreEqual(first.Brand, result.ProductBrand);
            Assert.AreEqual(first.Model, result.ProductModel);
            Assert.AreEqual(first.Category, result.ProductCategory);
            Assert.AreEqual(first.Comments, result.Comment);
            Assert.AreEqual(222, result.StockBranch);
            Assert.AreEqual(222, result.DeliveryBranch);
            Assert.AreEqual(111, result.ReceivingLocation);
            Assert.AreEqual(true, result.ReceivingLocationSpecified);
        }

        [Test]
        public void PublishCreated_Message_ShouldHaveCorrectType()
        {
            var publisher = GetCreatedPublisher();
            publisher.PublishCreated(GetBasicProductFixtureList(1));
            Assert.AreEqual("Warehouse.Booking.Submit", baseCreatedPublisher.Routing);
        }
        
        // Publish Received
        private StockAllocationPublisher GetReceivedPublisher()
        {
            return new StockAllocationPublisher(baseReceivedPublisher, mockLocationRepo.Object, mockProductRepo.Object);
        }

        [Test]
        public void PublishReceived_NullCintOrder_ShouldError()
        {
            var publisher = GetReceivedPublisher();
            Assert.Catch(() => publisher.PublishCreated(null));
        }

        [Test]
        public void PublishReceived_NoProductMessages_ShouldError()
        {
            var publisher = GetReceivedPublisher();
            Assert.Catch(() => publisher.PublishReceived(GetBasicProductFixture(2), 1, null));
        }

        [Test]
        public void PublishReceived_Message_ShouldMatchQuantity()
        {
            var publisher = GetReceivedPublisher();
            publisher.PublishReceived(GetBasicProductFixture(1), 3, null);
            Assert.AreEqual(baseReceivedPublisher.GenericMessages.First().Products.Sum(x => x.Units), 3);
        }

        [Test]
        public void PublishReceived_Message_ShouldAggregateCost()
        {
            var publisher = GetReceivedPublisher();
            publisher.PublishReceived(GetBasicProductFixture(1), 3, null);
            Assert.AreEqual(baseReceivedPublisher.GenericMessages.First().Products.Sum(x => x.Cost), 30);
        }

        [Test]
        public void PublishReceived_ShouldMapProperties()
        {
            var publisher = GetReceivedPublisher();
            var fixture = GetBasicProductFixture(1);
            publisher.PublishReceived(fixture, 3, null);
            var result = baseReceivedPublisher.GenericMessages.First();
            Assert.AreEqual(fixture.BookingId, result.Id);
            Assert.AreEqual("SHP#" + fixture.BookingId, result.Description);            
            Assert.AreEqual("Allocation", result.Type);
            Assert.AreEqual(fixture.WarehouseLocationId, result.WarehouseLocationId);
            Assert.AreEqual(fixture.WarehouseSalesLocationId, result.WarehouseLocationSalesId);
            Assert.AreEqual(fixture.ReceivingLocationId, result.ReceivingLocationId);
            Assert.AreEqual(fixture.ReceivingSalesLocationId, result.ReceivingLocationSalesId);
            Assert.IsNotEmpty(result.Products);
        }

        [Test]
        public void PublishReceived_Message_ShouldHaveCorrectType()
        {
            var publisher = GetReceivedPublisher();
            publisher.PublishReceived(GetBasicProductFixture(1), 3, null);
            Assert.AreEqual("Merchandising.Transfer", baseReceivedPublisher.Routing);
        }
    }
}