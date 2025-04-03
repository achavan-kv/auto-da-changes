using System.Collections;
using System;
using STL.Common.Static;
using System.Configuration;

namespace STL.Common.Services
{

    public static class Services
    {
        public static ClientRequest GetService(ServiceTypes service)
        {
            var s = service;
            return new ClientRequest(service.Uri,Credential.User ?? Credential.Name,Credential.Password, service.LoginPath);
        }

        public sealed class ServiceTypes
        {
            public string Type { get; set; }
            public Uri Uri { get; set; }
            public string LoginPath { get; set; }

            public ServiceTypes(string type, Uri uri, string loginPath)
            {
                this.Type = type;
                this.Uri = uri;
                this.LoginPath = loginPath;
            }

            public static readonly ServiceTypes CosacsWeb = new ServiceTypes("CosacsWeb", new Uri(ConfigurationManager.AppSettings["WebSiteURL"]), "/api/LoginUser");
        }
    }

   

    public static class ServicesAuth
    {
        private static Hashtable auth = new Hashtable();
        static object l = new object();

        public static Hashtable GetCookie
        {
            get { return auth; }
            set
            {
                lock (l)
                { 
                    auth = value; 
                };
            }
        }
    }
}
