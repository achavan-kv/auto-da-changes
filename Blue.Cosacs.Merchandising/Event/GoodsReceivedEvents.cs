namespace Blue.Cosacs.Merchandising.Event
{
    public class GoodsReceivedEvents
    {
        public const string Create = "CreateGoodsReceipt";
        public const string CreateCancellation = "CreateCancellationGoodsReceipt";
        public const string Update = "UpdateGoodsReceipt";
        public const string Approve = "ApproveGoodsReceipt";
        public const string Confirm = "ConfirmGoodsReceipt";
        public const string CreateDirect = "CreateDirectGoodsReceipt";
        public const string PrintGoodsReceiptWithCost = "PrintGoodsReceiptWithCost";
        public const string PrintGoodsReceiptPrintWithoutCost = "PrintGoodsReceiptPrintWithoutCost";
    }
}
