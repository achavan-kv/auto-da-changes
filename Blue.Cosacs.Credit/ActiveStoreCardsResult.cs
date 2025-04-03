using FileHelpers;

namespace Blue.Cosacs.Credit
{
    [DelimitedRecord(",")]
    public class ActiveStoreCardsResult
    {
        public string CardNumber { get; set; }
        public string NameOnCard { get; set; }
        public string AvailableSpend { get; set; }
        public string AccountNumber { get; set; }
    }
}
