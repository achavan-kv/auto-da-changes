namespace Blue.Cosacs.Unipay.Web.Model
{
    public class SignedDocumentFile
    {
        //[Required]
        public string accountNumber { get; set; }
        public string FileName { get; set; }
        public byte[] signedContract { get; set; }
    }
}