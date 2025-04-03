using System;
using System.Web;
using System.Web.Services.Protocols;
using System.Xml;

namespace STL.Common
{

    public sealed class WebServiceAuthenticationModule : IHttpModule
    {
        private WebServiceAuthenticationEventHandler _eventHandler = null;

        public event WebServiceAuthenticationEventHandler Authenticate
        {
            add { _eventHandler += value; }
            remove { _eventHandler -= value; }
        }

        public void Dispose()
        {
        }

        public void Init(HttpApplication app)
        {
            app.AuthenticateRequest += new EventHandler(this.OnEnter);
        }

        private void OnAuthenticate(WebServiceAuthenticationEvent e)
        {
            if (_eventHandler == null)
                return;

            _eventHandler(this, e);
            if (e.User != null)
                e.Context.User = e.Principal;
        }

        public string ModuleName
        {
            get { return "WebServiceAuthentication"; }
        }

        void OnEnter(Object source, EventArgs eventArgs)
        {
            var authenticate = true;
            var app = (HttpApplication)source;
            var context = app.Context;
            var HttpStream = context.Request.InputStream;

            var httpTrace = new HttpTrace();
            httpTrace.Log(context);

            // Current position of stream
            var posStream = HttpStream.Position;

            // If the request contains an HTTP_SOAPACTION 
            // header we'll look at this message
            if (context.Request.ServerVariables["HTTP_SOAPACTION"] == null)
                return;

            // Load the body of the HTTP message
            // into an XML document
            var dom = new XmlDocument();
            var soapUser = "";
            var soapUserId = 0;
            var soapPassword = "";
            var soapCookie = "";
            var soapCulture = "";
            var soapCountry = "";
            var soapVersion = "";

            try
            {
                //HACK to stop weird problem where exception is thrown for non-authenticating calls
                /* the reason this is a hack is because it specifically applies to the GetDictionary 
                 * call rather than generically to any call without an authentication SOAP header
                 * as originally designed */
                var _soapAction = context.Request.Headers["SOAPAction"];
                if (_soapAction != "\"http://strategicthought.com/webservices/GetDictionary\"")
                {

                    dom.Load(HttpStream);
                    // Reset the stream position
                    HttpStream.Position = posStream;

                    // Bind to the Authentication header
                    //Changed so that we can optionally allow through SOAP requests with no 
                    //authentication header.
                    var nodes = dom.GetElementsByTagName("Authentication");
                    authenticate = nodes.Count > 0 ? true : false;

                    foreach (XmlNode n in nodes)
                    {
                        soapUser = n["User"].InnerText;
                        soapUserId = int.Parse(n["UserId"].InnerText);
                        soapPassword = n["Password"].InnerText;
                        soapCookie = n["Cookie"] == null ? null : n["Cookie"].InnerText;
                        soapCulture = n["Culture"].InnerText;
                        soapCountry = n["Country"].InnerText;
                        soapVersion = n["Version"].InnerText;
                    }
                }
                else
                    authenticate = false;
            }
            catch (Exception e)
            {
                // Reset Position of stream
                HttpStream.Position = posStream;

                // Throw a SOAP Exception
                var name = new XmlQualifiedName("Load");
                var soapException = new SoapException("Unable to read SOAP request", name, e);
                throw soapException;
            }

            if (authenticate)
            {
                // Raise the custom global.asax event
                OnAuthenticate(new WebServiceAuthenticationEvent(context, soapUser, soapUserId, soapPassword, soapCookie, soapCulture, soapCountry, soapVersion));
            }

            return;
        }
    }
}
