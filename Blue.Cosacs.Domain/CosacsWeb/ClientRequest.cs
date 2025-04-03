using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Blue.Cosacs.Shared;
using STL.Common.Services.Model;
using Blue.Cosacs.Shared.CosacsWeb.Models;

namespace STL.Common.Services
{
    public class ClientRequest
    {
        private const string userAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.110 Safari/537.36 CosacsClient/10";
        private const string cookieId = ".COSACS";
        private readonly Uri baseUri;
        private readonly string user;
        private readonly string password;
        private readonly string loginPath;

        public ClientRequest(Uri baseUri, string user, string password, string loginPath)
        {
            this.baseUri = baseUri;
            this.user = user;
            this.password = password;
            this.loginPath = loginPath;
        }


        private void login()
        {
            var i = 0;
            var success = false;
            while (i < 3 && !success)
            {
                var request = (HttpWebRequest)WebRequest.Create(new Uri(baseUri, loginPath));
                request.Method = WebRequestMethods.Http.Post;
                request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                request.UserAgent = userAgent;
                request.Accept = "*/*";
                var payload = Encoding.UTF8.GetBytes(string.Format("username={0}&password={1}", System.Web.HttpUtility.UrlEncode(user), System.Web.HttpUtility.UrlEncode(password)));
                request.ContentLength = payload.Length;
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(payload, 0, payload.Length);
                }

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (!string.IsNullOrEmpty(response.Headers["Set-Cookie"]))
                    {
                        ServicesAuth.GetCookie[user] = StripCookie(response.Headers["Set-Cookie"]);
                        success = true;
                    }
                }
                i++;
            }
            if (!success)
                throw new LoginException(); // Todo Add detail of error. 
        }

        private string StripCookie(string cookies)
        {
            foreach (var c in cookies.Split(','))
            {
                if (string.Compare(c, 0, cookieId, 0, cookieId.Length) == 0)
                    return c;
            }
            return string.Empty;
        }

        public WarrantyResult GetWarranties(string product, string location, string warrantyType = "E")
        {
            var body = string.Format("{{\"Product\":\"{0}\",\"Location\":\"{1}\",\"WarrantyTypeCode\":\"{2}\",\"typeCode\":\"{3}\"}}", product, location, warrantyType, warrantyType);

            return GetData<WarrantyResult>(body, new Uri(baseUri, "/Cosacs/Warranty/Link/Search"), WebRequestMethods.Http.Post);
        }

        public WarrantyResult[] GetWarranties(List<ItemsWithoutWarrantiesView> items)
        {
            var payloadList = new List<string>();

            foreach (var item in items)
            {
                payloadList.Add(string.Format("{{\"Product\":\"{0}\",\"Location\":\"{1}\"}}", item.itemno, item.stocklocn));
            }

            var body = string.Format("[{0}]", string.Join(",", payloadList.ToArray()));

            return GetData<WarrantyResult[]>(body, new Uri(baseUri, "/Cosacs/Warranty/Link/SearchMany"), WebRequestMethods.Http.Post);
        }

        public WarrantyResult GetFreeWarranties(string product, string location)
        {
            var body = string.Format("{{\"Product\":\"{0}\",\"Location\":\"{1}\"}}", product, location);

            return GetData<WarrantyResult>(body, new Uri(baseUri, "/Cosacs/Warranty/Link/searchFree"), WebRequestMethods.Http.Post);
        }

        public WarrantyRenewal[] GetRenewals(List<WarrantyDeliveredView> deliveredWarranties)
        {
            var body = RenewalPayload(deliveredWarranties);

            return GetData<WarrantyRenewal[]>(body, new Uri(baseUri, "/Cosacs/Warranty/Link/SearchRenewals"), WebRequestMethods.Http.Post);
        }

        public WarrantyReturnModel GetWarrantyReturn(string warrantyNumber, int branch, int elapsedMonths, int freeWarrantyLength = 12)
        {
            var body = string.Format("{{\"warrantyNumber\":\"{0}\",\"branch\":\"{1}\",\"elapsedMonths\":\"{2}\",\"freeWarrantyLength\":\"{3}\"}}", warrantyNumber, branch, elapsedMonths, freeWarrantyLength);

            return GetData<WarrantyReturnModel>(body, new Uri(baseUri, "/Cosacs/Warranty/WarrantyReturn/Get"), WebRequestMethods.Http.Post);
        }

        public int CheckForOpenServiceRequests(string acctno)
        {
            var body = string.Format("{{\"acctno\":\"{0}\"}}", acctno);

            return GetData<int>(body, new Uri(baseUri, "/Cosacs/Service/Requests/CheckForOpenServiceRequests"), WebRequestMethods.Http.Post);
        }


        public NonStock GetDiscountDetails(string itemno)
        {
            var url = string.Format("{0}://{1}/NonStocks/Api/NonStock/LoadBySku?sku={2}", baseUri.Scheme, baseUri.Authority, itemno);

            return GetData<NonStock>(string.Empty, new Uri(url), WebRequestMethods.Http.Get);
        }

        public RepossessedCondition[] GetRepossessedConditions()
        {
            var values = GetData<RepossessedConditions>(string.Empty, new Uri(baseUri, "/Cosacs/Merchandising/RepossessedConditions/Get"), WebRequestMethods.Http.Get);

            if (values == null || string.Compare(values.Status, "success", true) != 0)
            {
                throw new Exception("Error retrieving Repossession Conditions");
            }

            return values.Data;
        }

        private T GetData<T>(string bodyContent, Uri uri, string httpMethod, int counter = 0)
        {
            if (counter == 3)
            {
                //to many times we try
                throw new LoginException();
            }

            if (!ServicesAuth.GetCookie.ContainsKey(user))
            {
                login();
            }

            var request = HttpWebRequest.Create(uri) as HttpWebRequest;

            request.AllowAutoRedirect = false;
            request.Method = httpMethod;
            request.ContentType = "application/json;charset=UTF-8";
            request.UserAgent = userAgent;
            request.Accept = "application/json, text/plain, */*";
            request.Headers["Cookie"] = ServicesAuth.GetCookie[user].ToString();

            if (!string.IsNullOrEmpty(bodyContent))
            {
                var payload = Encoding.UTF8.GetBytes(bodyContent);
                request.ContentLength = payload.Length;
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(payload, 0, payload.Length);
                }
            }

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    //cookie is expired
                    if (response.StatusCode == HttpStatusCode.Found)
                    {
                        ServicesAuth.GetCookie.Remove(user);
                        return GetData<T>(bodyContent, uri, httpMethod, counter);
                    }
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {

                        var stringReader = new StringReader(reader.ReadToEnd());
                        return (T)new Newtonsoft.Json.JsonSerializer().Deserialize(stringReader, typeof(T));
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Status != WebExceptionStatus.ProtocolError || ((HttpWebResponse)ex.Response).StatusCode != HttpStatusCode.Unauthorized)
                    throw new LoginException();
                else
                    return GetData<T>(bodyContent, uri, httpMethod, counter++);
            }
        }

        private string RenewalPayload(List<WarrantyDeliveredView> deliveredWarranties)
        {
            var items = new List<string>();
            deliveredWarranties.ForEach(d =>
            {
                var i = string.Format("{{\"WarrantyNumber\":\"{0}\",\"Location\":\"{1}\"}}", d.Itemno, d.stocklocn);
                if (!items.Contains(i))
                    items.Add(i);
            });
            return string.Format("[{0}]", string.Join(",", items.ToArray()));
        }

    }

    [Serializable]
    public class LoginException : Exception
    {
        public LoginException() { }
    }
}
