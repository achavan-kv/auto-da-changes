using Blue.Collections.Generic;
using Blue.Cosacs.Model;
using Blue.Cosacs.Sales;
using Blue.Cosacs.Shared;
using STL.Common.Constants.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using STL.BLL;//Suvidha

namespace Blue.Cosacs.Repositories
{
    public class SalesRepository
    {
        private readonly IClock clock;

        public SalesRepository()
        {
            this.clock = StructureMap.ObjectFactory.Container.GetInstance<IClock>();
        }

        //Method to fetch available discounts for passed item
        public IList<DiscountListItem> GetLinkDiscounts(int branch, string itemNo, string department)
        {
            department = ProductDepartment.Data.FirstOrDefault(x => x.Value.Equals(department)).Key;
            //if (department == null) return new List<ListItem>();

            using (var ctx = Context.Create())
            {
                return (from c in ctx.Code
                        join si in ctx.StockItem
                        on c.reference equals si.category.ToString()
                        //where c.code.Contains(department) && c.category.Equals("DIS") && ((int)si.origbr == branch)
                        where c.category.Equals("DIS") && ((int)si.origbr == branch) && (department == null || c.code.Contains(department)) // !c.code.Equals("DS")
                        orderby si.itemno
                        select new DiscountListItem
                        {
                            k = si.ItemID.ToString(),
                            v = si.itemno.Trim() + " - " + (si.itemdescr1.Trim() + " " + si.itemdescr2.Trim()),
                            Price = si.unitpricecash ?? 0,
                            DutyFreePrice = si.unitpricedutyfree ?? 0,
                            TaxRate = (decimal)si.taxrate
                        })
                            .Distinct()
                            .ToList();
            }
        }

        public decimal GetDiscountPrice(int branchNo, string sku)
        {
            var ret = 0m;

            using (var ctx = Context.Create())
            {
                var discount = ctx.StockItem.SingleOrDefault(s => s.SKU == sku && s.origbr == branchNo);

                if (discount != null)
                {
                    ret = discount.unitpricecash ?? 0;
                }
            }

            return ret;
        }

        //Method to fetch kit discounts for passed item
        public ListItem GetKitDiscount(int branch, string category)
        {
            using (var ctx = Context.Create())
            {
                var discount = (from c in ctx.Code
                                join s in ctx.StockInfo on c.reference equals s.IUPC
                                where c.category == CAT.KitItemCatDiscount && c.statusflag == "L" &&
                                      c.code == category
                                select new ListItem { k = s.itemno.Trim(), v = s.itemno.Trim() + " - " + (s.itemdescr1.Trim() + " " + s.itemdescr2.Trim()) })
                             .Distinct()
                            .SingleOrDefault();

                return discount;
            }
        }

        //Method to fetch available associated products for passed item
        public IList<string> GetAssociatedProducts(int branch, string itemNo)
        {
            var associateItemsList = new List<string>();

            using (var ctx = Context.Create())
            {
                var requiredDetails = GetLevelsDetails(branch, itemNo);

                if (requiredDetails == null)
                {
                    return associateItemsList;
                }

                // using the information fetched from above linq , the below linq will fetch associated item numbers
                associateItemsList = (from sa in ctx.StockInfoAssociated
                                      join s in ctx.StockItem
                                         on sa.AssocItemId equals s.ID
                                      where (sa.ProductGroup.Equals(requiredDetails.Department) || sa.ProductGroup.Equals("Any"))
                                              && (sa.Category.Equals(requiredDetails.Category) || sa.Category.Equals("0"))
                                              && (sa.Class.Equals("Any") || sa.Class.Equals(requiredDetails.Class))
                                              && (sa.SubClass.Equals("Any") || sa.SubClass.Equals(requiredDetails.SubClass))
                                              && s.origbr.Equals(branch) && s.category != 93
                                      select s.itemno)
                                        .Distinct()
                                        .ToList();
                return associateItemsList;
            }
        }

        public List<SalesKitItem> GetItemKitProducts(int branch, string itemNo)
        {
            using (var ctx = Context.Create())
            {
                var kits = (from kit in ctx.KitProduct
                            join s in ctx.StockInfo on kit.ComponentID equals s.Id
                            where kit.origbr == branch && kit.itemno == itemNo && kit.deleted == false
                            select new SalesKitItem
                            {
                                ItemNo = kit.componentno.Trim(),
                                ParentId = kit.ItemID,
                                Quantity = (int)kit.componentqty,
                                Category = s.category
                            }).Distinct().ToList();

                // var retKit = new SalesKitItem { ItemNo = itemNo, Items = kits };

                foreach (var kit in kits)
                {
                    kit.DiscountItem = (from c in ctx.Code
                                        join s in ctx.StockInfo on c.reference equals s.IUPC
                                        where c.category == CAT.KitItemCatDiscount &&
                                              c.statusflag == "L" &&
                                              c.code.Trim() == kit.Category.ToString()
                                        select new SalesKitItemDiscount
                                        {
                                            Id = s.Id,
                                            ItemNo = s.itemno,
                                            Description = (s.itemdescr1.Trim() + " " + s.itemdescr2.Trim()).Trim(),
                                            TaxRate = (decimal)s.taxrate
                                        }).SingleOrDefault();
                }

                return kits;
            }
        }

        //Method to fetch internal installations for passed item
        public IList<InternalInstallationProduct> GetInternalInstallations(int branch, string itemNo, string taxType, decimal taxRate)
        {
            var internalInstallationsList = new List<InternalInstallationProduct>();

            using (var ctx = Context.Create())
            {
                ItemLevels requiredDetails = GetLevelsDetails(branch, itemNo);

                if (requiredDetails == null)
                {
                    return internalInstallationsList;
                }

                // using the information fetched in requiredDetails , the below linq will fetch internal installations for passed item
                internalInstallationsList = (from sa in ctx.StockInfoAssociated
                                             join si in ctx.StockItem
                                                on sa.AssocItemId equals si.ID
                                             where (sa.ProductGroup.Equals(requiredDetails.Department) || sa.ProductGroup.Equals("Any"))
                                                     && (sa.Category.Equals(requiredDetails.Category) || sa.Category.Equals("0"))
                                                     && (sa.Class.Equals("Any") || sa.Class.Equals(requiredDetails.Class))
                                                     && (sa.SubClass.Equals("Any") || sa.SubClass.Equals(requiredDetails.SubClass))
                                                     && si.origbr.Equals(branch) && si.category == 93
                                             select new InternalInstallationProduct
                                             {
                                                 ItemId = si.ItemID,
                                                 ItemNo = si.itemno,
                                                 TaxExclusivePrice = GetTaxExclusivePrice(si, taxType),
                                                 TaxInclusivePrice = GetTaxInclusivePrice(si, taxType),
                                                 TaxRate = si.taxrate,
                                                 TaxAmount = GetTaxAmount(si, taxType),
                                                 ItemDescription1 = si.itemdescr1,
                                                 ItemDescription2 = si.itemdescr2,
                                                 DutyFreePrice = si.unitpricedutyfree
                                             })
                                                                                .ToList();
                return internalInstallationsList;
            }
        }

        //This method will fetch department,category,class and subclass of the passed item 
        private ItemLevels GetLevelsDetails(int branch, string itemNo)
        {
            using (var ctx = Context.Create())
            {
                ItemLevels levelDetails = (from si in ctx.StockItem
                                           join p in ctx.ProductHeirarchy
                                               on si.category.ToString() equals p.PrimaryCode
                                           where (p.CatalogType.Equals("02") && si.origbr.Equals(branch) && si.itemno.Equals(itemNo))
                                           select new ItemLevels
                                           {
                                               ItemNo = si.itemno,
                                               Department = p.ParentCode,
                                               Category = si.category.ToString(),
                                               Class = si.Class,
                                               SubClass = si.SubClass
                                           })
                                         .FirstOrDefault();
                return levelDetails;
            }
        }

        //Method to calculate Tax Amount for Internal Installation item
        private decimal GetTaxAmount(Shared.StockItem item, string taxType)
        {
            if (!item.unitpricecash.HasValue)
            {
                return 0M;
            }

            if (taxType.Equals("E"))
            {
                return decimal.Round(item.unitpricecash.Value * Convert.ToDecimal(item.taxrate) / 100, 2);
            }

            return taxType.Equals("I") ?
                decimal.Round((item.unitpricecash.Value * Convert.ToDecimal(item.taxrate)) / Convert.ToDecimal(100 + item.taxrate), 2) : 0M;
        }

        //Method to calculate Tax Inclusive Price for Internal Installation item
        private decimal? GetTaxInclusivePrice(Shared.StockItem item, string taxType)
        {
            var ret = 0M;

            if (!item.unitpricecash.HasValue)
            {
                return ret;
            }

            switch (taxType)
            {
                case "E":
                    var taxAmount = GetTaxAmount(item, taxType);

                    ret = item.unitpricecash.Value + taxAmount;
                    break;
                case "I":
                    ret = item.unitpricecash.Value;
                    break;
            }

            return ret;
        }

        //Method to calculate Tax Exclusive Price for Internal Installation item
        private decimal? GetTaxExclusivePrice(Shared.StockItem item, string taxType)
        {
            if (!item.unitpricecash.HasValue)
            {
                return 0M;
            }

            var taxInclusivePrice = GetTaxInclusivePrice(item, taxType);
            var taxAmount = GetTaxAmount(item, taxType);

            return taxInclusivePrice - taxAmount;
        }

        //Method to check available balance on Store Card
        public dynamic GetStoreCardAvailableBalance(long storeCardNo)
        {
            using (var ctx = Context.Create())
            {
                var dateChanged = ctx.StoreCardStatus
                    .Where(x => x.CardNumber == storeCardNo && x.DateChanged != null)
                    .Select(x => (DateTime?)x.DateChanged).Max();

                var storeCardDetails = (from sc in ctx.StoreCard
                                        join c in ctx.Customer
                                            on sc.CustID equals c.custid
                                        join ss in ctx.StoreCardStatus
                                            on sc.CardNumber equals ss.CardNumber
                                        where sc.CardNumber.Equals(storeCardNo) && ss.DateChanged.Equals(dateChanged)
                                        select new
                                        {
                                            storeCardNo = sc.CardNumber,
                                            isExpired = sc.ExpirationYear < clock.UtcNow.Year ||
                                            (sc.ExpirationYear == clock.UtcNow.Year && sc.ExpirationMonth < clock.UtcNow.Month),
                                            availableBalance = c.StoreCardAvailable,
                                            isActive = ss.StatusCode.Equals("A"),
                                            customerId = sc.CustID
                                        })
                                        .FirstOrDefault();

                dynamic response = null;
                if (storeCardDetails == null)
                {
                    response = new
                    {
                        errorMessage = "Store Card Number is invalid.",
                        availableBalance = (decimal)0,
                        customerId = string.Empty
                    };
                }
                else
                {
                    if (storeCardDetails.isExpired)
                    {
                        response = new
                        {
                            errorMessage = "Store Card is expired.",
                            availableBalance = (decimal)0,
                            storeCardDetails.customerId
                        };
                    }
                    else if (!storeCardDetails.isActive)
                    {
                        response = new
                        {
                            errorMessage = "Store Card is not active.",
                            availableBalance = (decimal)0,
                            storeCardDetails.customerId
                        };
                    }
                    else
                    {
                        response = new
                        {
                            errorMessage = string.Empty,
                            availableBalance = Convert.ToDecimal(storeCardDetails.availableBalance),
                            storeCardDetails.customerId
                        };
                    }
                }
                return response;
            }
        }

        public dynamic GetStoreCardCustomerId(long storeCardNo)
        {
            using (var ctx = Context.Create())
            {
                var dateChanged = ctx.StoreCardStatus
                    .Where(x => x.CardNumber == storeCardNo && x.DateChanged != null)
                    .Select(x => (DateTime?)x.DateChanged).Max();

                var custId = (from sc in ctx.StoreCard
                              join ss in ctx.StoreCardStatus
                                on sc.CardNumber equals ss.CardNumber
                              where sc.CardNumber.Equals(storeCardNo) && ss.DateChanged.Equals(dateChanged)
                              select sc.CustID
                                        )
                                        .FirstOrDefault();

                return custId;
            }
        }

        public List<int> GetWarrantyContractNumbers(short? branch, int noOfContracts)
        {
            var contractNumbers = new List<int>();

            if (noOfContracts == 0)
            {
                return contractNumbers;
            }

            for (var i = noOfContracts - 1; i >= 0; i--)
            {
                var contractNo = 0;
                var ret = 0;

                var cmd = new DN_BranchWarrantyGetContractNoSP { branchno = branch };
                cmd.ExecuteNonQuery();
                contractNo = cmd.contractno.HasValue ? cmd.contractno.Value : 0;

                contractNumbers.Add(contractNo);
            }

            return contractNumbers;
        }

        public dynamic GetGiftVoucherDetails(char giftVoucherIssuer, string otherCompanyNo, string giftVoucherNo)
        {
            dynamic response = null;
            dynamic giftVoucherDetails = null;
            using (var ctx = Context.Create())
            {
                if (giftVoucherIssuer == 'C')
                {
                    giftVoucherDetails = (from g in ctx.GiftVoucherCourts
                                          where g.reference.Equals(giftVoucherNo)
                                          select new
                                          {
                                              voucherValue = g.value.ToString(),
                                              giftVoucherNo = g.reference,
                                              isRedeemed = g.dateredeemed != null,
                                              isExpired = g.dateexpiry <= clock.UtcNow
                                          })
                                            .FirstOrDefault();
                }
                else
                {
                    //TO DO check for expiry of Non Courts Gift Voucher
                    giftVoucherDetails = (from g in ctx.GiftVoucherOther
                                          where g.reference.Equals(giftVoucherNo) && g.acctnocompany.Equals(otherCompanyNo)
                                          select new
                                          {
                                              voucherValue = g.value.ToString(),
                                              giftVoucherNo = g.reference,
                                              isRedeemed = g.dateredeemed != null,
                                              isExpired = false
                                          })
                                             .FirstOrDefault();
                }

                if (giftVoucherDetails == null)
                {
                    response = new
                    {
                        errorMessage = "Gift Voucher Number is invalid.",
                        value = 0
                    };
                }
                else
                {
                    if (giftVoucherDetails.isRedeemed)
                    {
                        response = new
                        {
                            errorMessage = "Gift Voucher has been redeemed.",
                            value = 0
                        };
                    }
                    else if (giftVoucherDetails.isExpired)
                    {
                        response = new
                        {
                            errorMessage = "Gift Voucher is expired.",
                            value = 0
                        };
                    }
                    else
                    {
                        response = new
                        {
                            errorMessage = string.Empty,
                            value = Convert.ToDecimal(giftVoucherDetails.voucherValue)
                        };
                    }
                }
            }
            return response;
        }

        public List<string> GetCashAndGoAccountNo(int branchNo)
        {
            using (var ctx = Context.Create())
            {
                var acct = (from c in ctx.CustAcct
                            where c.custid.Contains("PAID & TAKEN") && c.acctno.Contains(branchNo.ToString() + "5")
                            select c.acctno).FirstOrDefault();
                var accountNo = new List<string>();
                accountNo.Add(acct);
                return accountNo;
            }
        }

        //BOC Added by Suvidha - CR 2018-13 - 21/12/18 - to get the InvoiceNo.
        public List<string> GetAgrInvoiceNo(int agrmtno)
        {
            using (var ctx = Context.Create())
            {
                var acct = (from a in ctx.Agreement
                            where a.agrmtno.Equals(agrmtno)
                            select a.AgreementInvoiceNumber).FirstOrDefault();
                var accountNo = new List<string>();
                accountNo.Add(acct);
                return accountNo;
            }
        }
        //EOC Added by Suvidha - CR 2018-13 - 21/12/18 - to get the InvoiceNo.

        public bool IsCashierBalanceOutstanding(int userId)
        {
            var cmd = new DN_CashierMustDepositSP { empeeno = userId };
            cmd.ExecuteNonQuery();
            return cmd.mustdeposit ?? false;
        }

        public double GetAvailableQuantity(string itemNo, short branchNo)
        {
            using (var ctx = Context.Create())
            {
                var item = ctx.StockQuantity.SingleOrDefault(c => c.itemno == itemNo && c.stocklocn == branchNo);
                return item == null ? 0 : item.qtyAvailable;
            }
        }

        private static class ProductDepartment
        {
            private static ReadOnlyDictionary<string, string> department = null;

            public static ReadOnlyDictionary<string, string> Data
            {
                get
                {
                    return department ?? (department = new ReadOnlyDictionary<string, string>(GetDepartmentCodes()));
                }
            }

            private static Dictionary<string, string> GetDepartmentCodes()
            {
                return new Dictionary<string, string>
            {
                { "PCDIS", "Discount" },
                { "PCE", "Electrical" },
                { "PCF", "Furniture" },
                { "PCW", "Workstation" },
                { "PCO", "Other" }
            };
            }
        }

        public List<PayMethodMap> GetPaymentMethodMapping(short branchNo)
        {
            var lst = new List<PayMethodMap>();

            using (var ctx = Context.Create())
            {
                var countryCode = ctx.Country.FirstOrDefault().countrycode;

                if (!string.IsNullOrEmpty(countryCode))
                {
                    lst = ctx.PaymentMethodLookUp
                        .Where(x => x.CountryCode == countryCode)
                        .Select(m => new PayMethodMap
                        {
                            PosId = m.PosPayMethodId,
                            WinCosacsId = m.WinCosacsPayMethodId
                        }).ToList();
                }
            }

            return lst;
        }

        public string SavePaymentMethodMapping(short branchNo, short posId, short winCosacsId)
        {
            var ret = "";

            try
            {
                using (var ctx = Context.Create())
                {
                    var countryCode = ctx.Country.FirstOrDefault().countrycode;

                    if (!string.IsNullOrEmpty(countryCode))
                    {
                        var oldMapping = ctx.PaymentMethodLookUp
                            .SingleOrDefault(x => x.CountryCode == countryCode && x.PosPayMethodId == posId);

                        if (oldMapping != null)
                        {
                            ctx.PaymentMethodLookUp.DeleteOnSubmit(oldMapping);
                            ctx.SubmitChanges();
                        }

                        if (winCosacsId > 0)
                        {
                            ctx.PaymentMethodLookUp.InsertOnSubmit(new PaymentMethodLookUp
                            {
                                CountryCode = countryCode,
                                PosPayMethodId = posId,
                                WinCosacsPayMethodId = winCosacsId
                            });
                        }

                        ctx.SubmitChanges();
                    }
                }
            }
            catch (Exception ex)
            {

                ret = ex.Message;
            }

            return ret;
        }

        public string GetBranchDisplayType(short branchNo)
        {
            using (var ctx = Context.Create())
            {
                string displayType = "";
                if (branchNo > 0)
                {
                    displayType = (from b in ctx.Branch
                                   where b.branchno == branchNo
                                   select b.DisplayType).FirstOrDefault();
                }

                return displayType;
            }
        }

        //BOC Added by Suvidha - CR 2018-13 - 21/12/18 - to generate Agreement Invoice number
        public List<string> GenerateAgreementInvNum(string branch_no)
        {
            BCustomer bcust = new BCustomer();
            string agr_inv_no = "";
            var agr_inv_no1 = new List<string>();

            BCustomer cust = new BCustomer();
            agr_inv_no = cust.GenerateAgreementInvNo(branch_no);

            agr_inv_no1.Add(agr_inv_no);
            return agr_inv_no1;
        }
        //EOC Added by Suvidha - CR 2018-13 - 21/12/18 - to generate Agreement Invoice number

    }
}
