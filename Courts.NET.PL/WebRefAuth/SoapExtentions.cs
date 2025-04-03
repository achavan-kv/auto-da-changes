using System;
using System.Web.Services.Protocols;
using System.Net;

namespace STL.PL
{
    public interface IAuthentication
    {
        string User { get; set; }
        int UserId { get; set; }
        string Cookie { get; set; }
        string Password { get; set; }
        string Culture { get; set; }
        string Country { get; set; }
        string Version { get; set; }
    }

    public static class SoapExtentions
    {
        public static T Setup<T>(this System.Web.Services.Protocols.SoapHttpClientProtocol client, int timeout) where T : IAuthentication, new()
        {
            client.CookieContainer = new System.Net.CookieContainer();
            if (STL.Common.Static.Credential.Cookie != null)
                client.CookieContainer.SetCookies(BaseUri, STL.Common.Static.Credential.Cookie);

            client.Url = STL.Common.Static.Config.Url + client.GetType().Name + ".asmx";
            var authentication = new T
            {
                User = STL.Common.Static.Credential.User,
                UserId = STL.Common.Static.Credential.UserId,
                Cookie = STL.Common.Static.Credential.Cookie,
                Password = STL.Common.Static.Credential.Password,
                Culture = STL.Common.Static.Config.Culture,
                Country = STL.Common.Static.Config.CountryCode,
                Version = STL.Common.Static.Config.Version
            };

            client.Timeout = timeout;
            client.UseDefaultCredentials = false;
            client.EnableDecompression = true;
            return authentication;
        }

        private static Uri BaseUri
        {
            get { return new Uri(STL.Common.Static.Config.Url); }
        }

        public static void PostInvoke(this SoapHttpClientProtocol client, IAuthentication authentication)
        {
            SaveNewCookieForLater(client.CookieContainer);

            if (STL.Common.Static.Credential.Cookie != null)
            {
                client.CookieContainer = new System.Net.CookieContainer();
            }

            UpdateCookieOnAuthenticationInfo(authentication);
        }

        private static void SaveNewCookieForLater(CookieContainer cookieContainer)
        {
            var cookieCollection = cookieContainer.GetCookies(BaseUri);
            foreach (System.Net.Cookie cookie in cookieCollection)
            {
                if (IsNewCookie(cookie))
                {
                    STL.Common.Static.Credential.Cookie = cookie.ToString();
                }
            }
        }

        private static bool IsNewCookie(System.Net.Cookie cookie)
        {
            var possibleNewCookie = cookie.ToString();
            return !STL.Common.Static.Credential.Cookie.StartsWith(possibleNewCookie);
        }

        private static void UpdateCookieOnAuthenticationInfo(IAuthentication authentication)
        {
            if (authentication != null)
            {
                authentication.Cookie = STL.Common.Static.Credential.Cookie;
            }
        }
    }
}
