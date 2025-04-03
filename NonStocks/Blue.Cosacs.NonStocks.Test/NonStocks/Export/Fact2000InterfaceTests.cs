using Blue.Cosacs.NonStocks.Export;
using NUnit.Framework;
using System;

namespace Blue.Cosacs.NonStocks.Test.NonStocks.Export
{
    [TestFixture]
    public class Fact2000InterfaceTests
    {
        private const decimal priceTooBig = 100000000m;
        private static DateTime dateTooBig = new DateTime(2075, 1, 1);
        private static DateTime dateTooSmall = new DateTime(1899, 1, 1);

        [TestFixture]
        public class CreateProductAmmendment
        {
            [Test]
            public void FileTest1()
            {
                var productAmendmentFile = new ProductAmendmentFile()
                {
                    WarehouseNumber = 3,
                    ProductCode = "ABC",
                    SupplierProductDescription = string.Empty,
                    ProductMainDescription = "LABOUR",
                    ProductExtraDescription = string.Empty,
                    HPPrice = 0,
                    CashPrice = 0,
                    ProductCategory = "37",
                    SupplierACNo = string.Empty,
                    ProductStatus = ' ',
                    WarrantableProduct = 'N',
                    //ProductTypeType,
                    SpecialPrice = 0,
                    WarrantyReference = "00",
                    ProductEANCode = string.Empty,
                    ProductLeadTime = string.Empty,
                    WarrantyRenewal = ' ',
                    ReadyToAssemble = ' ',
                    DeletionIndicator = 'N',
                    CostPrice = 0,
                    SupplierName = string.Empty
                };

                var exported = (new Fact2000Interface()).CreateProductAmendmentFileLine(productAmendmentFile);
                var expectedResult = "03,ABC     ,                  ,LABOUR                   ," +
                                     "                                        ,+0000000000,+0000000000," +
                                     "37,          , ,N,06,+0000000000,00,             ,   , , ,N,           ,";

                Assert.True(exported.StartsWith(expectedResult));
            }

            [Test]
            public void FileTest2()
            {
                var productAmendmentFile = new ProductAmendmentFile()
                {
                    WarehouseNumber = 4,
                    ProductCode = "DWD",
                    SupplierProductDescription = string.Empty,
                    ProductMainDescription = "DISCOUNT WORKSTATION",
                    ProductExtraDescription = "DAMAGED",
                    HPPrice = 0,
                    CashPrice = 0,
                    ProductCategory = "36",
                    SupplierACNo = string.Empty,
                    ProductStatus = ' ',
                    WarrantableProduct = 'N',
                    //ProductTypeType,
                    SpecialPrice = 0,
                    WarrantyReference = "00",
                    ProductEANCode = string.Empty,
                    ProductLeadTime = string.Empty,
                    WarrantyRenewal = ' ',
                    ReadyToAssemble = ' ',
                    DeletionIndicator = 'N',
                    CostPrice = 0,
                    SupplierName = string.Empty
                };

                var exported = (new Fact2000Interface()).CreateProductAmendmentFileLine(productAmendmentFile);
                var expectedResult = "04,DWD     ,                  ,DISCOUNT WORKSTATION     ," +
                                     "DAMAGED                                 ,+0000000000,+0000000000," +
                                     "36,          , ,N,06,+0000000000,00,             ,   , , ,N,           ,";

                Assert.True(exported.StartsWith(expectedResult));
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void WarehouseNumber_TooBig()
            {
                var productAmendmentFile = new ProductAmendmentFile()
                {
                    WarehouseNumber = 1000,
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void ProductCode_TooBig()
            {
                var productAmendmentFile = new ProductAmendmentFile()
                {
                    ProductCode = "123456789",
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void SupplierProductDescription_TooBig()
            {
                var productAmendmentFile = new ProductAmendmentFile()
                {
                    SupplierProductDescription = "123456789-123456789",
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void ProductMainDescription_TooBig()
            {
                var productAmendmentFile = new ProductAmendmentFile()
                {
                    ProductMainDescription = "123456789-123456789-123456",
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void ProductExtraDescription_TooBig()
            {
                var productAmendmentFile = new ProductAmendmentFile()
                {
                    ProductExtraDescription = "123456789-123456789-123456789-123456789-1",
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void HPPrice_TooBig()
            {
                var productAmendmentFile = new ProductAmendmentFile()
                {
                    HPPrice = priceTooBig,
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void HPPrice_TooSmall()
            {
                var productAmendmentFile = new ProductAmendmentFile()
                {
                    HPPrice = -priceTooBig,
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void CashPrice_TooBig()
            {
                var productAmendmentFile = new ProductAmendmentFile()
                {
                    CashPrice = priceTooBig,
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void CashPrice_TooSmall()
            {
                var productAmendmentFile = new ProductAmendmentFile()
                {
                    CashPrice = -priceTooBig,
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void ProductCategory_TooBig()
            {
                var productAmendmentFile = new ProductAmendmentFile()
                {
                    ProductCategory = "123456",
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void ProductStatus_InvalidChar()
            {
                var productAmendmentFile = new ProductAmendmentFile()
                {
                    ProductStatus = 'A',
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void WarrantableProduct_NotYOrN()
            {
                var productAmendmentFile = new ProductAmendmentFile()
                {
                    WarrantableProduct = 'A',
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void SpecialPrice_TooBig()
            {
                var productAmendmentFile = new ProductAmendmentFile()
                {
                    SpecialPrice = priceTooBig,
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void SpecialPrice_TooSmall()
            {
                var productAmendmentFile = new ProductAmendmentFile()
                {
                    SpecialPrice = -priceTooBig,
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void WarrantyReference_TooBig()
            {
                var productAmendmentFile = new ProductAmendmentFile()
                {
                    WarrantyReference = "123",
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void ProductEANCode_TooBig()
            {
                var productAmendmentFile = new ProductAmendmentFile()
                {
                    ProductEANCode = "123456789-1234",
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void ProductLeadTime_TooBig()
            {
                var productAmendmentFile = new ProductAmendmentFile()
                {
                    ProductLeadTime = "1234",
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void WarrantyRenewal__NotYOrN()
            {
                var productAmendmentFile = new ProductAmendmentFile()
                {
                    WarrantyRenewal = 'A',
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void ReadyToAssemble__NotYOrN()
            {
                var productAmendmentFile = new ProductAmendmentFile()
                {
                    ReadyToAssemble = 'A',
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void DeletionIndicator__NotYOrN()
            {
                var productAmendmentFile = new ProductAmendmentFile()
                {
                    DeletionIndicator = 'A',
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void CostPrice_TooBig()
            {
                var productAmendmentFile = new ProductAmendmentFile()
                {
                    CostPrice = priceTooBig,
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void CostPrice_TooSmall()
            {
                var productAmendmentFile = new ProductAmendmentFile()
                {
                    CostPrice = -priceTooBig,
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void SupplierName_TooBig()
            {
                var productAmendmentFile = new ProductAmendmentFile()
                {
                    SupplierName = "123456789-123456789-123456",
                };
            }

        }

        [TestFixture]
        public class CreatePromotionalPrice
        {
            [Test]
            public void FileTest1()
            {
                var promotionalPriceFile = new PromotionalPriceFile()
                {
                    ProductCode = "106853",
                    WarehouseNumber = 99,
                    HPPrice1 = 1424.00m,
                    HPDateFrom1 = new DateTime(2014, 9, 15),
                    HPDateTo1 = new DateTime(2014, 9, 18),
                    HPPrice2 = 0,
                    HPDateFrom2 = null,
                    HPDateTo2 = null,
                    HPPrice3 = 0,
                    HPDateFrom3 = null,
                    HPDateTo3 = null,
                    CashPrice1 = 1424.00m,
                    CashDateFrom1 = new DateTime(2014, 9, 15),
                    CashDateTo1 = new DateTime(2014, 9, 18),
                    CashPrice2 = 0,
                    CashDateFrom2 = null,
                    CashDateTo2 = null,
                    CashPrice3 = 0,
                    CashDateFrom3 = null,
                    CashDateTo3 = null,
                };

                var exported = (new Fact2000Interface()).CreatePromotionalPriceFileLine(promotionalPriceFile);
                var expectedResult = "106853  ,99," +
                    "+0000142400,150914,180914,+0000000000,000000,000000,+0000000000,000000,000000," +
                    "+0000142400,150914,180914,+0000000000,000000,000000,+0000000000,000000,000000";

                Assert.True(exported.StartsWith(expectedResult));
            }

            [Test]
            public void FileTest2()
            {
                var promotionalPriceFile = new PromotionalPriceFile()
                {
                    ProductCode = "107018",
                    WarehouseNumber = 5,
                    HPPrice1 = 4599.00m,
                    HPDateFrom1 = new DateTime(2013, 6, 1),
                    HPDateTo1 = new DateTime(2071, 1, 5),
                    HPPrice2 = 0,
                    HPDateFrom2 = null,
                    HPDateTo2 = null,
                    HPPrice3 = 0,
                    HPDateFrom3 = null,
                    HPDateTo3 = null,
                    CashPrice1 = 4599.00m,
                    CashDateFrom1 = new DateTime(2013, 6, 1),
                    CashDateTo1 = new DateTime(2071, 1, 5),
                    CashPrice2 = 0,
                    CashDateFrom2 = null,
                    CashDateTo2 = null,
                    CashPrice3 = 0,
                    CashDateFrom3 = null,
                    CashDateTo3 = null,
                };

                var exported = (new Fact2000Interface()).CreatePromotionalPriceFileLine(promotionalPriceFile);
                var expectedResult = "107018  ,05," +
                    "+0000459900,010613,050171,+0000000000,000000,000000,+0000000000,000000,000000," +
                    "+0000459900,010613,050171,+0000000000,000000,000000,+0000000000,000000,000000";

                Assert.True(exported.StartsWith(expectedResult));
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void ProductCode_TooBig()
            {
                var productAmendmentFile = new PromotionalPriceFile()
                {
                    ProductCode = "123456789",
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void WarehouseNumber_TooBig()
            {
                var productAmendmentFile = new PromotionalPriceFile()
                {
                    WarehouseNumber = 1000,
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void HPPrice1_TooBig()
            {
                var promotionalPriceFile = new PromotionalPriceFile()
                {
                    HPPrice1 = priceTooBig,
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void HPPrice1_TooSmall()
            {
                var promotionalPriceFile = new PromotionalPriceFile()
                {
                    HPPrice1 = -priceTooBig,
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void HPDateFrom1_TooBig()
            {
                var promotionalPriceFile = new PromotionalPriceFile()
                {
                    HPDateFrom1 = dateTooBig,
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void HPDateFrom1_TooSmall()
            {
                var promotionalPriceFile = new PromotionalPriceFile()
                {
                    HPDateFrom1 = dateTooSmall,
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void HPDateTo1_TooBig()
            {
                var promotionalPriceFile = new PromotionalPriceFile()
                {
                    HPDateTo1 = dateTooBig,
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void HPDateTo1_TooSmall()
            {
                var promotionalPriceFile = new PromotionalPriceFile()
                {
                    HPDateTo1 = dateTooSmall,
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void HPPrice2_TooBig()
            {
                var promotionalPriceFile = new PromotionalPriceFile()
                {
                    HPPrice2 = priceTooBig,
                };
            }
            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void HPPrice2_TooSmall()
            {
                var promotionalPriceFile = new PromotionalPriceFile()
                {
                    HPPrice2 = -priceTooBig,
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void HPDateFrom2_TooBig()
            {
                var promotionalPriceFile = new PromotionalPriceFile()
                {
                    HPDateFrom2 = dateTooBig,
                };
            }


            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void HPDateFrom2_TooSmall()
            {
                var promotionalPriceFile = new PromotionalPriceFile()
                {
                    HPDateFrom2 = dateTooSmall,
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void HPDateTo2_TooBig()
            {
                var promotionalPriceFile = new PromotionalPriceFile()
                {
                    HPDateTo2 = dateTooBig,
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void HPDateTo2_TooSmall()
            {
                var promotionalPriceFile = new PromotionalPriceFile()
                {
                    HPDateTo2 = dateTooSmall,
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void HPPrice3_TooBig()
            {
                var promotionalPriceFile = new PromotionalPriceFile()
                {
                    HPPrice3 = priceTooBig,
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void HPPrice3_TooSmall()
            {
                var promotionalPriceFile = new PromotionalPriceFile()
                {
                    HPPrice3 = -priceTooBig,
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void HPDateFrom3_TooBig()
            {
                var promotionalPriceFile = new PromotionalPriceFile()
                {
                    HPDateFrom3 = dateTooBig,
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void HPDateFrom3_TooSmall()
            {
                var promotionalPriceFile = new PromotionalPriceFile()
                {
                    HPDateFrom3 = dateTooSmall,
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void HPDateTo3_TooBig()
            {
                var promotionalPriceFile = new PromotionalPriceFile()
                {
                    HPDateTo3 = dateTooBig,
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void HPDateTo3_TooSmall()
            {
                var promotionalPriceFile = new PromotionalPriceFile()
                {
                    HPDateTo3 = dateTooSmall,
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void CashPrice1_TooBig()
            {
                var promotionalPriceFile = new PromotionalPriceFile()
                {
                    CashPrice1 = priceTooBig,
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void CashPrice1_TooSmall()
            {
                var promotionalPriceFile = new PromotionalPriceFile()
                {
                    CashPrice1 = -priceTooBig,
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void CashDateFrom1_TooBig()
            {
                var promotionalPriceFile = new PromotionalPriceFile()
                {
                    CashDateFrom1 = dateTooBig,
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void CashDateFrom1_TooSmall()
            {
                var promotionalPriceFile = new PromotionalPriceFile()
                {
                    CashDateFrom1 = dateTooSmall,
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void CashDateTo1_TooBig()
            {
                var promotionalPriceFile = new PromotionalPriceFile()
                {
                    CashDateTo1 = dateTooBig,
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void CashDateTo1_TooSmall()
            {
                var promotionalPriceFile = new PromotionalPriceFile()
                {
                    CashDateTo1 = dateTooSmall,
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void CashPrice2_TooBig()
            {
                var promotionalPriceFile = new PromotionalPriceFile()
                {
                    CashPrice2 = priceTooBig,
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void CashPrice2_TooSmall()
            {
                var promotionalPriceFile = new PromotionalPriceFile()
                {
                    CashPrice2 = -priceTooBig,
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void CashDateFrom2_TooBig()
            {
                var promotionalPriceFile = new PromotionalPriceFile()
                {
                    CashDateFrom2 = dateTooBig,
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void CashDateFrom2_TooSmall()
            {
                var promotionalPriceFile = new PromotionalPriceFile()
                {
                    CashDateFrom2 = dateTooSmall,
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void CashDateTo2_TooBig()
            {
                var promotionalPriceFile = new PromotionalPriceFile()
                {
                    CashDateTo2 = dateTooBig,
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void CashDateTo2_TooSmall()
            {
                var promotionalPriceFile = new PromotionalPriceFile()
                {
                    CashDateTo2 = dateTooSmall,
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void CashPrice3_TooBig()
            {
                var promotionalPriceFile = new PromotionalPriceFile()
                {
                    CashPrice3 = priceTooBig,
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void CashPrice3_TooSmall()
            {
                var promotionalPriceFile = new PromotionalPriceFile()
                {
                    CashPrice3 = -priceTooBig,
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void CashDateFrom3_TooBig()
            {
                var promotionalPriceFile = new PromotionalPriceFile()
                {
                    CashDateFrom3 = dateTooBig,
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void CashDateFrom3_TooSmall()
            {
                var promotionalPriceFile = new PromotionalPriceFile()
                {
                    CashDateFrom3 = dateTooSmall,
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void CashDateTo3_TooBig()
            {
                var promotionalPriceFile = new PromotionalPriceFile()
                {
                    CashDateTo3 = dateTooBig,
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void CashDateTo3_TooSmall()
            {
                var promotionalPriceFile = new PromotionalPriceFile()
                {
                    CashDateTo3 = dateTooSmall,
                };
            }
        }

        [TestFixture]
        public class CreateProductLink
        {
            [Test]
            public void FileTest1()
            {
                var productLink = new ProductLinkFile()
                {
                    ProductGroup = "PCE",
                    Category = "03",
                    Class = "031",
                    SubClass = string.Empty,
                    AssociatedItemId = "MOC23",
                };

                var exported = (new Fact2000Interface()).CreateProductLinkFileLine(productLink);
                var expectedResult = "PCE,   03,031,     ,MOC23";

                Assert.True(exported.StartsWith(expectedResult));
            }

            [Test]
            public void FileTest2()
            {
                var productLink = new ProductLinkFile()
                {
                    ProductGroup = "PCF",
                    Category = "04",
                    Class = "1",
                    SubClass = string.Empty,
                    AssociatedItemId = "ABD 12345 FFFASG",
                };

                var exported = (new Fact2000Interface()).CreateProductLinkFileLine(productLink);
                var expectedResult = "PCF,   04,  1,     ,ABD 12345 FFFASG";

                Assert.True(exported.StartsWith(expectedResult));
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void ProductGroup_TooBig()
            {
                var productLink = new ProductLinkFile()
                {
                    ProductGroup = "123456",
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void Category_TooBig()
            {
                var productLink = new ProductLinkFile()
                {
                    Category = "123456",
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void Class_TooBig()
            {
                var productLink = new ProductLinkFile()
                {
                    Class = "123456",
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void SubClass_TooBig()
            {
                var productLink = new ProductLinkFile()
                {
                    SubClass = "123456",
                };
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void AssociatedItemId_TooBig()
            {
                var productLink = new ProductLinkFile()
                {
                    AssociatedItemId = "123456789-123456789",
                };
            }

        }

    }
}
