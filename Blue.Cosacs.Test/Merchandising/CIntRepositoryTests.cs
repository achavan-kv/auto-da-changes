//using NUnit.Framework;

//namespace Blue.Cosacs.Test.Merchandising
//{
//    [TestFixture]
//    public class CIntRepositoryTests
//    //{
//    //    private models.ProductAWCInput input = new models.ProductAWCInput();

//    //    [TestFixtureSetUp]
//    //    public virtual void Setup()
//    //    {
//    //        input = new Blue.Cosacs.Merchandising.Models.ProductAWCInput()
//    //        {
//    //            CostPrice = new Dictionary<int, models.GeneralCostPrice>
//    //            {
//    //                {
//    //                    1,
//    //                    new models.GeneralCostPrice()
//    //                    {
//    //                        AverageWeightedCost = 20,
//    //                        LastLandedCost = 50,
//    //                        ProductId = 1,
//    //                        SupplierCost = 10
//    //                    }
//    //                },
//    //                {
//    //                    2,
//    //                    new models.GeneralCostPrice()
//    //                    {
//    //                        AverageWeightedCost = 30,
//    //                        LastLandedCost = 55,
//    //                        ProductId = 2,
//    //                        SupplierCost = 25
//    //                    }
//    //                },
//    //                {
//    //                     3,
//    //                    new models.GeneralCostPrice()
//    //                    {
//    //                        AverageWeightedCost = 10,
//    //                        LastLandedCost = 60,
//    //                        ProductId = 3,
//    //                        SupplierCost = 30
//    //                    }
//    //                }
//    //            },

//    //            ReposessionProductPrice = new Dictionary<int, decimal> { { 1, 80 } },
//    //            StockOnHand = new Dictionary<int, int> { { 1, 100 }, { 2, 200 } },
//    //            ReposessionCostSetting = 100
//    //        };
//    //    }

//    //[Test]
//    //public void CalcAWC_Return_2DeliveryOrderTypes()
//    //{
//    //    var repository = new CINTRepository(null, null, null, null, null, null, null);

//    //    var cintOrders = new List<Blue.Cosacs.Merchandising.Model.CintOrder>()
//    //    {
//    //        new model.CintOrder
//    //        {
//    //           Type = Blue.Cosacs.Merchandising.Enums.CintOrderType.Return,
//    //           Id = 1,
//    //           MerchProductId = 1,
//    //           Quantity = 7
//    //        },
//    //        new model.CintOrder
//    //        {
//    //           Type = Blue.Cosacs.Merchandising.Enums.CintOrderType.Delivery,
//    //           Id = 2,
//    //           MerchProductId = 2,
//    //           Quantity = 3
//    //        },
//    //        new model.CintOrder
//    //        {
//    //           Type = Blue.Cosacs.Merchandising.Enums.CintOrderType.Delivery,
//    //           Id = 3,
//    //           MerchProductId = 2,
//    //           Quantity = 5
//    //        },
//    //    };

//    //    var result = repository.CalcAWC(cintOrders, input);
//    //    Assert.AreEqual(result.Count(), 2);
//    //    var product2 = result.Where(p => p.ProductId == 2).First();

//    //    Assert.AreEqual(product2.Awc, 30);
//    //    Assert.AreEqual(product2.LastLandedCost, 55);
//    //    Assert.AreEqual(product2.SupplierCost, 25);
//    //}
//}
//}
