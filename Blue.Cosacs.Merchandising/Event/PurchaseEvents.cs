namespace Blue.Cosacs.Merchandising.Event
{
    public static class PurchaseEvents
    {
        public const string Create = "CreatePurchaseOrder";
        public const string Edit = "EditPurchaseOrder";

        public const string CreatePurchaseOrderProduct = "CreatePurchaseOrderProduct";
        public const string EditPurchaseOrderProduct = "EditPurchaseOrderProduct";

        public const string PrintPurchaseOrderWithCost = "PrintPurchaseOrderWithCost";
        public const string PrintPurchaseOrderWithoutCost = "PrintPurchaseOrderWithoutCost";
    }
}
