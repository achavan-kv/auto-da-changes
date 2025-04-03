using System;

namespace Unicomer.Cosacs.Model
{
    public class TokenResponse
    {
        public string token { get; set; }
        public DateTime expiration { get; set; }
    }
    //public class TokenRequest
    //{
    //    public string key { get; set; }
    //    public string user { get; set; }
    //}
}
