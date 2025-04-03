namespace Unicomer.Cosacs.Model
{
    public class JResponse
    {
        public dynamic Result { get; set; }
        public dynamic Message { get; set; }
        public int StatusCode { get; set; }
        public bool Status { get; set; }
    }
}
