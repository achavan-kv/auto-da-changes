using System.Collections.Generic;

namespace Blue.Cosacs.Merchandising.Enums
{
    public class CintOrderType
    {
        public const string RegularOrder = "RegularOrder";
        public const string Return = "Return";
        public const string Repossession = "Repossession";
        public const string Delivery = "Delivery";
        public const string CancelOrder = "CancelOrder";
        public const string Redelivery = "Redelivery";

        public static IEnumerable<string> OrderTypes()
        {
            yield return RegularOrder;
            yield return Return;
            yield return Repossession;
            yield return Delivery;
            yield return CancelOrder;
            yield return Redelivery;
        }
    }
}
