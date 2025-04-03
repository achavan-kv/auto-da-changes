using System;
using System.Data.SqlClient;
using System.Linq;
using System.Xml;
using Blue.Cosacs.Shared;
using STL.BLL;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.FTransaction;
using STL.DAL;
using STL.Common.ServiceRequest;

namespace Blue.Cosacs.Subscribers
{
    public class ServicePayment : Hub.Client.Subscriber
    {
        public override void Sink(int id, XmlReader message)
        {
            using (var conn = new SqlConnection(Connections.Default))
            {
                conn.Open();

                using (var trans = conn.BeginTransaction())
                {
                    using (var ctx = Context.Create(conn, trans))
                    {
                        var rescore = false;
                        var customerAcctNo = string.Empty;
                        var country = ctx.Country.FirstOrDefault();
                        BBranch br = new BBranch();

                        var b = Deserialize<Blue.Cosacs.Messages.Service.ServicePayment>(message);

                        new Blue.Cosacs.InitCountryParamCache().PopulateCacheCountryParams(country.countrycode.Trim());

                        //First check if a Charge Account exists for the Service Request (migrated Service Requests that previously had a charge account)
                        var existingChargeAcct = (from s in ctx.SR_ChargeAcct
                                                  where s.ServiceRequestNo == b.ServiceRequestNo
                                                  && s.ChargeType == (b.ChargeType == "Customer" ? ServiceAcct.Customer : b.ChargeType == "Deliverer" ? ServiceAcct.Deliverer : b.ChargeType)
                                                  select s.AcctNo).FirstOrDefault();

                        //First check if a Charge Account exists for the Service Request
                        var serviceChargeAcct = (from s in ctx.ServiceChargeAcct
                                                 where s.ServiceRequestNo == b.ServiceRequestNo
                                                 && s.ChargeType == b.ChargeType
                                                 select s.AcctNo).FirstOrDefault();

                        //Create a Charge Account
                        if (existingChargeAcct == null && serviceChargeAcct == null)
                        {
                            if (ctx.Branch
                                .Where(p => p.CreateCashAccounts && p.branchno == Convert.ToInt16(b.Branch))
                                .Any())
                            {
                            // Create a new cash account

                            BAccount customerAccount = new BAccount();
                            customerAcctNo = customerAccount.CreateCustomerAccount(conn, trans,
                                country.countrycode.Trim(),
                                    b.ServiceBranch == null?Convert.ToInt16(b.Branch):Convert.ToInt16(b.ServiceBranch),
                                b.CustomerId,
                                AT.Cash,
                                b.EmpeeNo,
                                false,
                                out rescore,
                                "",
                                Convert.ToString(b.ServiceRequestNo));         // #16624

                            customerAcctNo = customerAcctNo.Trim().Replace("-", "");
                            if (customerAcctNo.Length == 12)
                            {
                                // Link the Charge To account to the SR
                                ServiceChargeAcct chargeAcct = new ServiceChargeAcct
                                {
                                    AcctNo = customerAcctNo,
                                    ServiceRequestNo = b.ServiceRequestNo,
                                    ChargeType = b.ChargeType
                                };

                                ctx.ServiceChargeAcct.InsertOnSubmit(chargeAcct);

                            }
                        }
                        else
                        {
                                throw new Exception(string.Format("Can't create an Cash account for branch {0}", b.Branch));
                            }
                        }
                        else
                        {
                            customerAcctNo = existingChargeAcct != null ? existingChargeAcct : serviceChargeAcct;
                            //customerAcctNo = serviceChargeAcct; 
                        }

                        //Process the payment to the Charge Account
                        var TransRefNo = br.GetTransRefNo(conn, trans, Convert.ToInt16(b.Branch));

                        BTransaction AddTrans = new BTransaction(
                            conn, 
                            trans, 
                            customerAcctNo, 
                            Convert.ToInt16(b.Branch),
                            TransRefNo, (b.Amount *-1),
                            b.EmpeeNo,
                            TransType.Payment,
                            b.Bank == null? string.Empty: b.Bank,
                            b.BankAccountNumber == null? string.Empty: b.BankAccountNumber,
                            b.ChequeNumber == null? string.Empty: b.ChequeNumber, 
                            Convert.ToInt16(b.PayMethod), 
                            country.countrycode.Trim(),
                            DateTime.Now,
                             "", 
                            b.ServiceRequestNo
                       );

                        ctx.SubmitChanges();
                        trans.Commit();
                       
                    }

                }

            }
        }
    }
}


