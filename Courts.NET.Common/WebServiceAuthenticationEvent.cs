using System;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Security;

namespace STL.Common
{
    public class WebServiceAuthenticationEvent : EventArgs
    {
        private IPrincipal _IPrincipalUser;
        private HttpContext _Context;

        public WebServiceAuthenticationEvent(HttpContext context)
        {
            _Context = context;
        }

        public WebServiceAuthenticationEvent(HttpContext context, string user, int userId, string password, string cookie, string culture, string country, string version)
        {
            _Context = context;
            User = user;
            UserId = userId;
            Password = password;
            Cookie = cookie;
            Culture = culture;
            Country = country;
            Version = version;
        }

        public HttpContext Context
        {
            get { return _Context; }
        }

        public IPrincipal Principal
        {
            get { return _IPrincipalUser; }
            set { _IPrincipalUser = value; }
        }

        public void Authenticate() //string[] roles)
        {
            InitContext();
            FormsAuthentication.SetAuthCookie(User, false);
        }

        public void InitContext()
        {
            var i = new GenericIdentity(User, "CoSaCS");
            Thread.CurrentPrincipal = this.Principal = new GenericPrincipal(i, new string[] { }); // roles);
            STL.Common.Static.Credential.UserId = UserId;
            STL.Common.Static.Credential.Name = User;                   //#12200
            STL.Common.Static.Credential.Password = Password;
        }

        public string User { get; set; }
        public int UserId { get; set; }
        public string Password { get; set; }
        public string Cookie { get; set; }
        public string Culture { get; set; }
        public string Country { get; set; }
        public string Version { get; set; }
    }
}

