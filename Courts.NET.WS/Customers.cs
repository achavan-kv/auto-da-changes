using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Blue.Cosacs.ComLib;
using Blue.Cosacs.Repositories;
using Blue.Cosacs.Shared;
using ComLib;

namespace Blue.Cosacs.Web
{
    /// <summary>
    /// Summary description for Customers
    /// </summary>
    [WebService(Namespace = "http://www.bluebridgeltd.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Customers : System.Web.Services.WebService
    {
        CustomerRepository repository;

        public Customers()
        {

            //Uncomment the following line if using designed components 
            //InitializeComponent(); 

            repository = new CustomerRepository();
        }

        [WebMethod]
        public Customer Create(CustomerResult customer)
        {
            var result = repository.Create(customer);

            return result.Success ? result.Item : null; //new BoolResult { Success = result.Success, Message = result.Message };
        }

        //[WebMethod]
        //public List<Customer> Search(Customer.Parameters.Search parameters)
        //{
        //    return repository.Search(parameters).ToList();
        //}
    }
}