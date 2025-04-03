using System;

namespace Blue.Cosacs.Warehouse.Common
{
    using System.Linq.Expressions;

    public static class Expressions
    {
        public static Expression<Func<T, bool>> IsInternal<T>(bool flag) where T : class, IDeliverable
        {
            return
                d =>
                flag && (d.DeliveryOrCollection == "T" || d.DeliveryOrCollection == "A" || d.DeliveryOrCollection == "Q")
                || !flag && (d.DeliveryOrCollection != "T" && d.DeliveryOrCollection != "A" && d.DeliveryOrCollection != "Q");
        }
    }
}
