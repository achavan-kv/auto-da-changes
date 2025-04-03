using System.Web.Services.Protocols;

namespace STL.Common
{
    /// <summary>
    /// This class is used to add user credentials to the SOAP
    /// header in web service calls and also to pass the front end culture setting 
    /// to the web service layer.
    /// </summary>
    public class Authentication : SoapHeader
    {
        public string User;
        public int UserId;
        public string Cookie;
        public string Password;
        public string Culture;
        public string Country;
        public string Version;
    }
}