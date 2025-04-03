
namespace Blue.Cosacs.Shared.Net
{
    public class Header
    {
        public static Header Client()
        {
            return new Header
            {
                User = STL.Common.Static.Credential.User,
                UserId = STL.Common.Static.Credential.UserId,
                Password = STL.Common.Static.Credential.Password,
                Cookie = STL.Common.Static.Credential.Cookie,
                Culture = STL.Common.Static.Config.Culture,
                CountryCode = STL.Common.Static.Config.CountryCode,
                Version = STL.Common.Static.Config.Version
            };
        }

        public static void SetCookie(string cookie)
        {
            STL.Common.Static.Credential.Cookie = cookie;
        }

        public const string HttpHeaderUser = "X-Cosacs-User";
        public const string HttpHeaderUserId = "X-Cosacs-UserId";
        public const string HttpHeaderPassword = "X-Cosacs-Password";
        public const string HttpHeaderCulture = "X-Cosacs-Culture";
        public const string HttpHeaderCountryCode = "X-Cosacs-CountryCode";
        public const string HttpHeaderVersion = "X-Cosacs-Version";
        public const string HttpHeaderCookie = "X-Cosacs-Cookie";

        public string User { get; set; }
        public int UserId { get; set; }
        public string Password { get; set; }
        public string Culture { get; set; }
        public string CountryCode { get; set; }
        public string Version { get; set; }
        public string Cookie { get; set; }
    }
}
