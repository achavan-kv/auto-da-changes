using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Security;
using System.Threading;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Blue.Admin;
using Blue.Cosacs;
using Blue.Cosacs.Services;
using Blue.Cosacs.Shared.Net;
using ServiceStack.ServiceHost;
using ServiceStack.WebHost.Endpoints;
using STL.Common;
using System.Web;
using System.Text;

namespace STL.WS
{
    public class Global : System.Web.HttpApplication
    {
        private Logging log;
        private StaticDataSingleton _sd = StaticDataSingleton.Instance();
        private HybridDictionary Cache
        {
            get { return _sd.Data; }
            set { _sd.Data = value; }
        }

        private Blue.Cosacs.ServiceStackHost serviceHost;

        protected void Application_Error(object sender, EventArgs e)
        {
            var log = global::Elmah.ErrorLog.GetDefault(HttpContext.Current);
            if (log != null)
            {
                var ex = Server.GetLastError();
                var errorLogId = log.Log(new global::Elmah.Error(ex, HttpContext.Current));
            }
        }

        protected void Application_Start(Object sender, EventArgs e)
        {
            EndpointHost.ContentTypeFilter.ClearCustomFilters();
            EndpointHost.ContentTypeFilter.Register("application/json", StreamSerializer, StreamDeserializerDelegate);

            serviceHost = new Blue.Cosacs.ServiceStackHost();
            try
            {
                serviceHost.Init();
            }
            catch (System.Reflection.ReflectionTypeLoadException ex)
            {
                var sb = new StringBuilder();
                foreach (var e1 in ex.LoaderExceptions)
                {
                    sb.AppendFormat("{0}\n{1}\n", e1.Message, e1.StackTrace);
                    if (e1.InnerException != null)
                    { 
                        var e2 = e1.InnerException;
                        sb.AppendFormat("\t{0}\n\t{1}", e2.Message, e2.StackTrace);
                    }
                    sb.AppendLine("-------------------------------------------------");
                }
                throw new Exception("Reflection type load exception: " + sb.ToString());
            }

            //AreaRegistration.RegisterAllAreas();
            RegisterRoutes(RouteTable.Routes);

            Blue.Cosacs.Web.WS.Bootstrapper.Initialize(); // StructureMap/Dependency Injection stuff
            ServerBase.Authenticate = WebServiceAuthentication_OnAuthenticate;
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Barcode", "barcode/{type}/{value}", new { controller = "BarCode", action = "Index" }, null);

            routes.MapRoute(
                "Home", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
        }

        private static void StreamSerializer(IRequestContext requestContext, object dto, Stream toStream)
        {
            using (var writer = new StreamWriter(toStream))
                CreateSerializer().Serialize(writer, dto);
        }

        private static Newtonsoft.Json.JsonSerializer CreateSerializer()
        {
            var serializer = new Newtonsoft.Json.JsonSerializer();
            serializer.Converters.Insert(0, new JsonDataSetConverter());
            serializer.Converters.Insert(0, new JsonDataTableConverter());
            return serializer;
        }

        private static object StreamDeserializerDelegate(Type type, Stream fromStream)
        {
            using (var reader = new StreamReader(fromStream))
                return CreateSerializer().Deserialize(reader, type);
        }

        protected void Session_Start(Object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            MainCache.SetCulture(System.Web.HttpContext.Current);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture = ((CultureInfo)System.Web.HttpContext.Current.Cache["Culture"]);
        }

        protected void Application_EndRequest(Object sender, EventArgs e)
        {

        }

        protected void Session_End(Object sender, EventArgs e)
        {

        }

        protected void Application_End(Object sender, EventArgs e)
        {

        }

        private static readonly string version = typeof(STL.DAL.Connections).Assembly.GetName().Version.ToString();

        public void WebServiceAuthentication_OnAuthenticate(Object sender, WebServiceAuthenticationEvent e)
        {
            try
            {
                if (e.Version != version)
                    throw new ApplicationException("Client Server versions do not match");

                if (IfCookieIsNotOnTheAuthEvent(e))
                {
                    // Found the cookie here on the JSON call's
                    TryReadCookieFromServerHttpRequestContext(sender, e);
                }

                if (!string.IsNullOrEmpty(e.Cookie))
                {
                    var cookieString = e.Cookie.Split('=', ';')[1].Trim();
                    var ticket = FormsAuthentication.Decrypt(cookieString);
                    if (ticket == null || ticket.Expired)
                    {
                        if (Validate(e)) // attempt to validate based on user and password
                            return;
                        throw new SecurityException("Cookie validation failed " + (ticket == null ? null : ticket.Name));
                    }
                    else
                    {
                        e.InitContext();
                        new Blue.Cosacs.InitCountryParamCache().PopulateCacheCountryParams(e.Country); //#11245 - Caching country parameters (lost when IISRESET).
                        return;
                    }
                }

                if (Validate(e))
                    return;

                throw new SecurityException("Authentication failed for employee " + e.User);
            }
            catch (Exception ex)
            {
                log = new Logging();
                log.logException(ex, e.User, "LogIn");
                throw;
            }
        }

        private static bool IfCookieIsNotOnTheAuthEvent(WebServiceAuthenticationEvent e)
        {
            return string.IsNullOrEmpty(e.Cookie);
        }

        private static void TryReadCookieFromServerHttpRequestContext(Object sender, WebServiceAuthenticationEvent e)
        {
            try
            {
                var cookie = ((ServiceStack.ServiceHost.IRequiresRequestContext)sender).RequestContext.GetHeader("Cookie");
                e.Cookie = cookie;
            }
            catch (Exception) { }
        }

        private bool Validate(WebServiceAuthenticationEvent e)
        {
            var result = new UserPasswordValidation(
                new Blue.Admin.UserRepository(EventStore.Instance),
                new Blue.DateTimeClock()).Validate(e.User, e.Password);

            if (result.IsValid)
            {
                e.Authenticate();
                return true;
            }
            return false;
        }

        /*private void OldAuthenticate(WebServiceAuthenticationEvent e)
        {
            var login = new BLogin();

            // Create a Cache object that holds all user details
            if (Cache["Users"] == null)
            {
                var userDetails = login.GetUserDetails();
                Cache["Users"] = userDetails;
            }

            //Check to see if user is in Cache["Users"]
            var user = (List<UserDetails>)Cache["Users"];
            var foundUser = (UserDetails)user.Find(ud => e.User == ud.empeeno.ToString());

            if (foundUser.empeeno == 0)
                foundUser = (UserDetails)user.Find(ud1 => e.User == ud1.factEmployeeNo);

            if (foundUser.empeeno != 0 && foundUser.factEmployeeNo != null)
            {
                // Now check password for that user
                var x = 1;
                var encryptedPassword = 113;

                while (x <= e.Password.Length)
                {
                    var c = e.Password.Substring(x - 1, 1);
                    var ascii = Convert.ToChar(c);
                    encryptedPassword = encryptedPassword + ascii * x;
                    x += 1;
                }

                if (foundUser.password == encryptedPassword.ToString())
                {
                    var roles = foundUser.roles;
                    e.Authenticate(roles);

                    new Blue.Cosacs.InitCountryParamCache().PopulateCacheCountryParams(e.Country); //IP - 11/06/12 - #10231
                }
                else throw new SecurityException("Authentication failed for employee " + e.User);
            }
            else throw new SecurityException("Authentication failed for employee " + e.User);
        }*/
    }
}

