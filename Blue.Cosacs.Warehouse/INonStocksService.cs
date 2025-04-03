namespace Blue.Cosacs.Warehouse
{
    public interface INonStocksService
    {
        string NonStockServiceType { get; }
        string NonStockServiceItemNo { get; }
        string NonStockServiceDescription { get; }
    }
}
