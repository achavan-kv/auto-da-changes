namespace Blue.Cosacs.Warranty.Solr
{
    public interface ICosacsTaxSettings
    {
        decimal TaxRate { get; set; }
        string TaxType { get; set; }
    }
}
