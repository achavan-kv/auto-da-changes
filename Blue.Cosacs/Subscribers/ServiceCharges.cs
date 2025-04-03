using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data.SqlClient;
using STL.DAL;
using Blue.Cosacs.Repositories;
using STL.BLL;
using STL.Common;
using STL.Common.Constants.AccountTypes;
using Blue.Cosacs.Shared;
using STL.Common.ServiceRequest;
using STL.Common.Constants.FTransaction;


namespace Blue.Cosacs.Subscribers
{
    public class ServiceCharges : Hub.Client.Subscriber
    {
        public const string InternalCustomer = "SI";
        public const string ExternalInternal = "SE";
        public const string StockRepair = "S";
        public const string InternalInstallation = "II";
        public const string ExternalInstallation = "IE";

        public override void Sink(int id, XmlReader message)
        {
            var serviceCharges = Deserialize<Blue.Cosacs.Messages.Service.ServiceCharges>(message);

            if (string.Compare(serviceCharges.ServiceType, StockRepair, true) == 0)
            {
                return;
            }

            if (serviceCharges.ServiceCharge.Any(p => string.IsNullOrEmpty(p.Account) && string.IsNullOrEmpty(p.CustomerId)))
            {
                throw new Exception("Either CustomerId or Account must be populated");
            }

            using (var conn = new SqlConnection(Connections.Default))
            {
                conn.Open();

                using (var trans = conn.BeginTransaction())
                {
                    using (var ctx = Context.Create(conn, trans))
                    {
                        var rescore = false;
                        var count = 0;
                        var chargeToAcctNo = string.Empty;
                        var country = ctx.Country.FirstOrDefault();
                        var labourValue = Convert.ToDecimal(0.0);
                        var labourTax = Convert.ToDecimal(0.0);
                        var totalTax = Convert.ToDecimal(0.0);
                                        
                        BBranch br = new BBranch();
                        BServiceRequest bsr = new BServiceRequest();

                        new InitCountryParamCache().PopulateCacheCountryParams(country.countrycode.Trim());

                        foreach (var charge in serviceCharges.ServiceCharge)
                        {
                            count += 1;

                            var chargesCount = serviceCharges.ServiceCharge.Where(w => w.ChargeType == "Customer" || w.ChargeType == "Deliverer").Count();

                            if (charge.ChargeType == "Customer" || charge.ChargeType == "Deliverer")
                            {
                                if (!(string.IsNullOrEmpty(charge.CustomerId)) && String.IsNullOrEmpty(charge.Account)) 
                                {

                                    //First check if a Charge Account exists for the Service Request (migrated Service Requests that previously had a charge account)
                                    var existingChargeAcct = (from s in ctx.SR_ChargeAcct
                                                             where s.ServiceRequestNo == charge.ServiceRequestNo
                                                             && s.ChargeType == (charge.ChargeType == "Customer" ? ServiceAcct.Customer: charge.ChargeType == "Deliverer"? ServiceAcct.Deliverer: charge.ChargeType)
                                                             select s.AcctNo).FirstOrDefault();

                                    //First check if a Charge Account exists for the Service Request
                                    var serviceChargeAcct = (from s in ctx.ServiceChargeAcct
                                                             where s.ServiceRequestNo == charge.ServiceRequestNo
                                                             && s.ChargeType == charge.ChargeType
                                                             select s.AcctNo).FirstOrDefault();
                                    //Create a Charge Account
                                    if (existingChargeAcct == null && serviceChargeAcct == null)
                                    {
                                        // Create a new cash account
                                        BAccount customerAccount = new BAccount();
                                        chargeToAcctNo = customerAccount.CreateCustomerAccount(conn, trans,
                                            country.countrycode.Trim(),
                                            Convert.ToInt16(charge.Branch),
                                            charge.CustomerId,
                                            AT.Cash,
                                            serviceCharges.EmpeeNo,
                                            false,
                                            out rescore,
                                            "",
                                            Convert.ToString(charge.ServiceRequestNo));

                                        chargeToAcctNo = chargeToAcctNo.Trim().Replace("-", "");
                                        if (chargeToAcctNo.Length == 12)
                                        {
                                            // Link the Charge To account to the SR
                                            ServiceChargeAcct chargeAcct = new ServiceChargeAcct
                                            {
                                                AcctNo = chargeToAcctNo,
                                                ServiceRequestNo = charge.ServiceRequestNo,
                                                ChargeType = charge.ChargeType
                                            };

                                            ctx.ServiceChargeAcct.InsertOnSubmit(chargeAcct);
                                            ctx.SubmitChanges();
                                        }
                                    }
                                    else
                                    {
                                        chargeToAcctNo = existingChargeAcct != null? existingChargeAcct : serviceChargeAcct;
                                        //chargeToAcctNo = serviceChargeAcct;
                                    }
                                }
                                else if (!(String.IsNullOrEmpty(charge.Account)))
                                {
                                    chargeToAcctNo = charge.Account;

                                    var serviceChargeAcct = (from s in ctx.ServiceChargeAcct
                                                             where s.ServiceRequestNo == charge.ServiceRequestNo
                                                             && s.ChargeType == charge.ChargeType
                                                             select s.AcctNo).FirstOrDefault();

                                    //If ChargeTo is not Customer or Deliverer (then supplier, warranty or internal) need to store link in linking table.
                                    if (serviceChargeAcct == null)
                                    {
                                        ServiceChargeAcct chargeAcct = new ServiceChargeAcct
                                        {
                                            AcctNo = chargeToAcctNo,
                                            ServiceRequestNo = charge.ServiceRequestNo,
                                            ChargeType = charge.ChargeType
                                        };

                                        ctx.ServiceChargeAcct.InsertOnSubmit(chargeAcct);
                                        ctx.SubmitChanges();
                                    }
                                }

                                var chargeLabelItemNumber = GetChargeTypeItemNumber(charge);
                                ////Retrieve the ItemId of the charge item (e.g. Parts Courts, Parts Non Courts, Labour)
                                //var itemNumber = charge.Label == "Parts Cosacs"? "7C0001" : charge.Label == "Parts Other"? "7N0001" : charge.Label == "Labour"? "7L0001" : "7L0001";    // #16486
                                var itemId = (from s in ctx.StockInfo
                                              join sq in ctx.StockQuantity on s.Id equals sq.ID
                                              where sq.stocklocn == Convert.ToInt16(charge.Branch) && s.IUPC == chargeLabelItemNumber
                                              select s.Id).FirstOrDefault();

                                var taxItemId = (from s in ctx.StockInfo
                                                 join sq in ctx.StockQuantity on s.Id equals sq.ID
                                                 where sq.stocklocn == Convert.ToInt16(charge.Branch)
                                                 && s.IUPC == "STAX"
                                                 select s.Id).FirstOrDefault();

                                if (charge.Label == "Additional")
                                {
                                    //Labour tax is included in Additional
                                    charge.Value += labourValue;
                                    charge.Tax += labourTax;
                                }

                                if (charge.Label != "Labour")
                                {
                                    //Finally apply the charges to the account.
                                    if (charge.Value != 0)
                                    {
                                        bsr.SaveServiceRequestCharges(conn,
                                            trans,
                                            string.IsNullOrEmpty(charge.Account) ? AT.Cash : AT.Special,
                                            chargeToAcctNo,
                                            Convert.ToInt32(charge.ServiceRequestNo),
                                            Convert.ToInt16(charge.Branch),
                                            charge.ItemNumber,
                                            charge.Value,
                                            itemId,
                                            count == serviceCharges.ServiceCharge.Count() ? true : false,
                                            charge.Tax,
                                            taxItemId);
                                    }

                                    //Labour tax is included in Additional
                                    if (charge.Tax != null)
                                    {
                                        totalTax += Convert.ToDecimal(charge.Tax);
                                    }
                                }
                                else    // #16486 save Labour to add to Additional
                                {
                                    labourValue = charge.Value;
                                    labourTax = Convert.ToDecimal(charge.Tax);
                                }

                                if (totalTax > 0 && count == chargesCount)
                                {
                                   bsr.SaveServiceChargeTax(conn, trans, totalTax, string.IsNullOrEmpty(charge.Account) ? AT.Cash : AT.Special,chargeToAcctNo,
                                                            Convert.ToInt32(charge.ServiceRequestNo),
                                                            Convert.ToInt16(charge.Branch), taxItemId);
                                }

                            }
                        } // foreach

                        ctx.SubmitChanges();
                        trans.Commit();
                    }
                }
            }
        }

        private string GetChargeTypeItemNumber(Messages.Service.ServiceCharge charge)
        {
            CountryParameterCache.Cache = new CacheProvider();

            // TODO: get these settings...
            var ServiceItemLabour = CountryParameterCache.GetCountryParameter<string>(CountryParameterNames.ServiceItemLabour);
            var ServiceItemPartsCourts = CountryParameterCache.GetCountryParameter<string>(CountryParameterNames.ServiceItemPartsCourts);
            var ServiceItemPartsOther = CountryParameterCache.GetCountryParameter<string>(CountryParameterNames.ServiceItemPartsOther);

            if (charge == null)
            {
                return string.Empty;
            }

            if (string.Compare(charge.Label, "Labour", true) == 0 ||
                string.Compare(charge.Label, "Additional", true) == 0)
            {
                return ServiceItemLabour;
            }
            else if (string.Compare(charge.Label, "Parts Cosacs", true) == 0)
            {
                return ServiceItemPartsCourts;
            }
            else if (string.Compare(charge.Label, "Parts External", true) == 0 ||
                string.Compare(charge.Label, "Parts Other", true) == 0 ||
                string.Compare(charge.Label, "Parts Salvaged", true) == 0)
            {
                return ServiceItemPartsOther;
            }

            return string.Empty;
        }

        public class CacheProvider : ICache
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            public object Get(string key)
            {
                if (dictionary.ContainsKey(key))
                    return dictionary[key];

                return null;
            }

            public object Insert(string key, object item)
            {
                dictionary.Add(key, item);
                return item;
            }
        }
    }
}


