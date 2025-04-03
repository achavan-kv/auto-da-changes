using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Warehouse.Common
{
    public static class DeliveryOrCollectionType
    {
        public const string Delivery = "D";

        public const string Collection = "C";

        public const string Redelivery = "R";

        public const string Allocation = "A";

        public const string Requisition = "Q";

        public const string Transfer = "T";

        public static bool IsInternal(string type)
        {
            return type == "T" || type == "A" || type == "Q";
        }
    }
}
