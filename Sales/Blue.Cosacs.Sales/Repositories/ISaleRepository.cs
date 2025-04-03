using System.Collections.Generic;
using Blue.Cosacs.Sales.Models;
using System;

namespace Blue.Cosacs.Sales.Repositories
{
    public interface ISaleRepository
    {
        CustomResponseMessage InsertDiscountLimit(DiscountLimit discountLimitDetail, int currentUserId);
        CustomResponseMessage UpdateDiscountLimit(DiscountLimit discountLimitDetail, int currentUserId);
        void DeleteDiscountLimit(DiscountLimit discountLimitDetail);
        IEnumerable<DiscountLimit> GetDiscountLimitData(DiscountLimit discountLimitDetail);
        decimal GetDiscountLimit(int branchNumber);
    }
}