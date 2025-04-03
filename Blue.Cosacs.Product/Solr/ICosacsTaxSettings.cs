namespace Blue.Cosacs.Stock.Solr
{
    public interface ICosacsTaxSettings
    {
        decimal TaxRate { get; set; }
        string TaxType { get; set; }
    }
}
