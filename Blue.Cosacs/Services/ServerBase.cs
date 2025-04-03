using System.Web;
using Blue.Cosacs.Shared.Net;
using STL.Common;

namespace Blue.Cosacs.Services
{
    public abstract class ServerBase : ServiceStack.ServiceHost.IRequiresRequestContext
    {
        public static WebServiceAuthenticationEventHandler Authenticate;

        public ServiceStack.ServiceHost.IRequestContext RequestContext
        {
            get;
            set;
        }

        protected void Pre()
        {
            // check authentication
            var header = new Header
            {
                User = RequestContext.GetHeader(Header.HttpHeaderUser),
                UserId = (RequestContext.GetHeader(Header.HttpHeaderUserId) == null ? 0 : int.Parse(RequestContext.GetHeader(Header.HttpHeaderUserId))),
                Password = RequestContext.GetHeader(Header.HttpHeaderPassword),
                Cookie = RequestContext.GetHeader(Header.HttpHeaderCookie),
                Culture = RequestContext.GetHeader(Header.HttpHeaderCulture),
                CountryCode = RequestContext.GetHeader(Header.HttpHeaderCountryCode),
                Version = RequestContext.GetHeader(Header.HttpHeaderVersion)
            };

            if (Authenticate != null)
            {
                Authenticate(this, new WebServiceAuthenticationEvent(HttpContext.Current,
                    header.User, header.UserId, header.Password, header.Cookie, header.Culture, header.CountryCode, header.Version));
            }
        }
    }
}
