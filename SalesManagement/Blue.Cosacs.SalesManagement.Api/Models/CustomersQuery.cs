using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Blue.Networking;

namespace Blue.Cosacs.SalesManagement.Api.Models
{
    public class CustomersQuery
    {
        private readonly IClock clock;
        private readonly IHttpClient httpClient;

        public CustomersQuery(IClock clock, IHttpClient httpClient)
        {
            this.clock = clock;
            this.httpClient = httpClient;
        }

        public CustomersQuery()
        { }

        public response response { get; set; }


        public IList<doc> QueryCustomers(string queryString, short userBranch, int userId)
        {
            var url = string.Format("/Customer/Api/Reindex/Search?branch={0}&q={1}&rows=25", userBranch, queryString);

            var jsonClient = new HttpClientJsonAuth(httpClient, clock, userId.ToString());
            var result = new List<doc>();
            int current = 0;

            while (true)
            {
                var request = RequestJson<byte[]>.Create(string.Format("{0}&start={1}", url, current), System.Net.WebRequestMethods.Http.Get);
                var data = jsonClient.Do<byte[], CustomersQuery>(request).Body;

                if (data.response.docs.Length == 0)
                {
                    break;
                }

                current = current + 25;
                result.AddRange(data.response.docs);
            }

            return result;
        }

    }

    public class response
    {
        public int numFound { get; set; }
        public doc[] docs { get; set; }
    }


    public class doc
    {

        public string CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string HomePhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string ReceiveEmails { get; set; }
        public string ReceiveSms { get; set; }
        public bool DoNotCallAgain { get; set; }
    }
}