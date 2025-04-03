using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using Blue.Cosacs.Shared;
using ComLib;
using ComLib.ValidationSupport;
using System.Data.SqlTypes;
using System.Data;

namespace Blue.Cosacs.Repositories
{
    public class CustomerRepository
    {
        //public bool CustomerCreditBlocked(string custId)
        //{
        //    bool? creditblocked = false;
        //    new CustomerCheckCreditBlockedSP().Execute(custId, out creditblocked);
        //    return (bool)creditblocked;
        //}

        public void GetRFLimitAvailableSpend(string custId, ref decimal rfLimit, ref decimal availableSpend)
        {
            using (var ctx = Context.Create())
            {
                var record = (from c in ctx.Customer
                              where c.custid == custId
                              select new { c.AvailableSpend, c.RFCreditLimit }).First();

                rfLimit        = record.RFCreditLimit;
                availableSpend = record.AvailableSpend;

            }
        }

        //IP - 25/11/10 - Calculate the Available Spend for the Customer.
        public void CalcCustomerAvailableSpend(string custId)
        {
            new CalculateAvailableSpendForCustomer().ExecuteNonQuery(custId);
        }

        //IP - 20/12/10 - Update the StoreCardLimit on the Customer table 
        public void CustomerUpdateStoreCardLimit(SqlConnection conn, SqlTransaction trans, string custId, decimal storeCardLimit)
        {
            int? ret;
            new CustomerUpdateStoreCardLimit(conn, trans).ExecuteNonQuery(custId, storeCardLimit, out ret);
        }

        //IP - 20/12/10 - update the StoreCardAvailable on the Customer table
        public void CustomerUpdateAndGetStoreCardAvailable(SqlConnection conn, SqlTransaction trans, string custId, out decimal? storeCardLimit, out decimal? storeCardAvailable)
        {
            storeCardLimit = 0;
            storeCardAvailable = 0;
            new CustomerUpdateStoreCardAvailable(conn, trans) 
            { 
                custID             = custId, 
                storeCardLimit     = 0, 
                storeCardAvailable = 0, 
                Return             = 0 
            }.ExecuteNonQuery();

            //using (var ctx = Context.Create())
            //{
            //    var record = (from c in ctx.Customer
            //                  where c.custid == custId
            //                  select new { c.StoreCardAvailable, c.StoreCardLimit }).First();

            //    storeCardLimit = Convert.ToDecimal(record.StoreCardLimit);
            //    storeCardAvailable = Convert.ToDecimal(record.StoreCardAvailable);
            //}
        }

        public Customer Load(string custId)
        {
            using (var context = Context.Create())
            {
                Customer customer = (from c in context.Customer
                             //join a in context.CustAddress on c.custid equals a.custid into tmpAddress
                            // from address in tmpAddress.DefaultIfEmpty()
                             where c.custid == custId
                             select c).AnsiFirstOrDefault(context);
                 
                if (customer !=null )
                {
                    var address = (from c in context.CustAddress
                                            where c.custid == custId && c.datemoved == null
                                            select  c).AnsiToList(context);
                    return customer;
                }
                else
                    return null;    
            }
        }

        //public Customer LoadByAcctno(string acctno)
        //{
        //    using (var context = Context.Create())
        //    {
        //        var query = (from c in context.Customer
        //                    join ca in context.CustAcct on c.custid equals ca.custid
        //                    join a in context.CustAddress on c.custid equals a.custid into tmpAddress
        //                    from address in tmpAddress.DefaultIfEmpty()
        //                    where ca.acctno == acctno &&
        //                    ca.hldorjnt == "H"
        //                    select new
        //                    {
        //                        Customer = c,
        //                        Address = address
        //                    }).ToList();

        //        if (query.Any())
        //        {
        //            var customer = query.First().Customer;
        //            customer.CustAddress = query.Select(r => r.Address).ToList();
        //            return customer;
        //        }
        //        else
        //            return null;
        //    }
        //}

        public BoolResult<CustomerResult> Create(CustomerResult customer)
        {
            //var execute = new Action<Context>((c) => { });
            if (customer.dateborn == default(DateTime))
                customer.dateborn = DateTime.Today;

            var validator = Validate(customer);

            if (!validator.HasErrors)
            {
                using (var conn = new SqlConnection(STL.DAL.Connections.Default))
                {
                    conn.Open();
                    using (var trans = conn.BeginTransaction())
                    {
                        using (var context = Context.Create(conn, trans))
                        {
                            context.Customer.InsertOnSubmit(customer.ToCustomer());
                            context.SubmitChanges();

                            customer.CustAddress.ForEach(a =>
                            {
                                a.custid = customer.custid;
                                InsertCustAddress.Execute(a, conn, trans);
                            });

                            trans.Commit();
                            conn.Close();
                        }
                    }
                }
            }

            return new BoolResult<CustomerResult>(customer, !validator.HasErrors, validator.Errors.Message(), validator.Errors);
        }

        public ValidatorFluent Validate(Customer customer)
        {
            return new ValidatorFluent(typeof(Customer))
               .Check(() => customer.custid)        .IsNotNull().IsBetween(0, 20)
               .Check(() => customer.name)          .IsNotNull().IsBetween(0, 60)
               .Check(() => customer.morerewardsno) .IsNotNull().IsBetween(0, 16)
               .Check(() => customer.IdNumber)      .IsNotNull().IsBetween(0, 30)
               .Check(() => customer.IdType)        .IsNotNull().IsBetween(0, 4)
               .Check(() => customer.RFCardPrinted) .IsNotNull().IsBetween(0, 1)
               .Check(() => customer.LimitType)     .IsNotNull().IsBetween(0, 1)
               .Check(() => customer.InstantCredit) .IsNotNull().IsBetween(0, 1)
               .Check(() => customer.StoreType)     .IsNotNull().IsBetween(0, 1)
               .Check(() => customer.maritalstat)   .IsNotNull().IsBetween(0, 1)
               .Check(() => customer.Nationality)   .IsNotNull().IsBetween(0, 4)
               .Check(() => customer.ScoreCardType) .IsNotNull().IsBetween(0, 1)
               .Check(() => customer.dateborn)      .IsNotNull().IsBefore(SqlDateTime.MaxValue.Value).IsAfter(SqlDateTime.MinValue.Value)
               ;
        }

        public IEnumerable<Customer> Search(CustomerResult.Parameters.Search parameters)
        {
            var dataSet = new DN_CustomerSearchSP
            {
                address   = parameters.Address,
                custid    = parameters.CustomerId,
                first     = parameters.FirstName,
                last      = parameters.Surname,
                limit     = parameters.Limit,
                phone     = parameters.PhoneNumber,
                storetype = parameters.StoreType,
                settled   = 2,
                exact     = 0,
                Return    = 0

            }.ExecuteDataSet();

            if(dataSet.Tables.Count == 0)
                return Enumerable.Empty<Customer>();
            
            var rows = dataSet.Tables[0].AsEnumerable();
            
            return rows.Select(row => new CustomerResult
            {
                custid    = row.Field<String>("Customer ID"),
                title     = row.Field<String>("Title"),
                firstname = row.Field<String>("First Name"),
                name      = row.Field<String>("Last Name"),
                alias     = row.Field<String>("Alias"),

                CustAddress = new List<CustAddress>
                {   
                    new CustAddress 
                    { 
                        cusaddr1  = row.Field<String>("cusaddr1"),
                        cusaddr2  = row.Field<String>("cusaddr2"),
                        cusaddr3  = row.Field<String>("cusaddr3"),
                        cuspocode = row.Field<String>("postcode"),                                    
                    } 
                }
            })
            .Distinct(new CustomerComparer());
        }


        //#19422 - CR17976
        public DataSet GetDuplicateCustomers(SqlConnection conn, SqlTransaction trans)
        {
            return new GetDuplicateCustomersSP(conn, trans).ExecuteDataSet();
        }

        //#19422 - CR17976
        public void UpdateDuplicateCustomers(SqlConnection conn, SqlTransaction trans, string custid, string duplicateCustid, bool resolved)
        {
            using (var context = Context.Create())
            {

                DuplicateCustomers dupCust = (from d in context.DuplicateCustomers
                                                where d.Custid == custid && d.DuplicateCustid == duplicateCustid 
                                                select d).AnsiFirstOrDefault(context);

                //Delete the record as now marking as unresolved
                if (dupCust != null && resolved == false)
                {
                    context.DuplicateCustomers.DeleteOnSubmit(dupCust); 
                }

                //Insert a new entry as marking as resolved
                if (dupCust == null && resolved == true)
                {
                    DuplicateCustomers d = new DuplicateCustomers
                    {
                        Custid = custid,
                        DuplicateCustid = duplicateCustid
                    };

                    context.DuplicateCustomers.InsertOnSubmit(d);
                }

                context.SubmitChanges();
 
            }
           
        }
    }
}
