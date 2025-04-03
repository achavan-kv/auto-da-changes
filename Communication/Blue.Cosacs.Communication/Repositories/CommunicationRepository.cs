using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Blue.Cosacs.Communication.Messages;

namespace Blue.Cosacs.Communication.Repositories
{
    public sealed class CommunicationRepository : ICommunicationRepository
    {
        private readonly IClock clock;

        public CommunicationRepository(IClock clock)
        {
            this.clock = clock;
        }

        public IList<MailchimpTemplateID> GetMailchimpTemplateID(int? id = null)
        {
            using (var scope = Context.Read())
            {
                var values = scope.Context.MailchimpTemplateID
                    .Select(p => p);

                if (id.HasValue)
                {
                    values = values
                        .Where(p => p.Id == id.Value);
                }

                return values.ToList();
            }
        }

        public void SaveMailchimpTemplateID(MailchimpTemplateID templateId)
        {
            using (var scope = Context.Write())
            {
                if (scope.Context.Set<MailchimpTemplateID>().Any(p => p.Id == templateId.Id))
                {
                    scope.Context.Entry<MailchimpTemplateID>(templateId).State = EntityState.Modified;
                }
                else
                {
                    scope.Context.Entry<MailchimpTemplateID>(templateId).State = EntityState.Added;
                }

                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public bool DeleteMailchimpTemplateID(int id)
        {
            using (var scope = Context.Read())
            {
                var value = scope.Context.MailchimpTemplateID
                    .FirstOrDefault(p => p.Id == id);

                if (value != null)
                {
                    return DeleteMailchimpTemplateID(value);
                }
            }

            return true;
        }

        public bool DeleteMailchimpTemplateID(MailchimpTemplateID templateId)
        {
            try
            {
                using (var scope = Context.Write())
                {
                    if (scope.Context.Set<MailchimpTemplateID>().Any(p => p.Id == templateId.Id))
                    {
                        scope.Context.Entry<MailchimpTemplateID>(templateId).State = EntityState.Deleted;
                    }

                    scope.Context.SaveChanges();
                    scope.Complete();

                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void InsertSandBoxMails(SandBoxMails value)
        {
            using (var scope = Context.Write())
            {
                scope.Context.SandBoxMails.Add(value);

                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        #region Emails

        public IList<BlackEmailList> GetEmailUnsubcription(IList<string> emails)
        {
            using (var scope = Context.Read())
            {
                var query = scope.Context.BlackEmailList
                    .Select(p => p);

                if (emails != null && emails.Any())
                {
                    query = query
                        .Where(p => emails.Contains(p.Email));
                }

                return query.ToList();
            }
        }

        public void InsertBlackEmailList(BlackEmailList value)
        {
            InsertBlackEmailList(new List<BlackEmailList> { value });
        }

        public void InsertBlackEmailList(IList<BlackEmailList> value)
        {
            using (var scope = Context.Write())
            {
                var sb = new StringBuilder();

                sb.Append("<Emails>");
                foreach (var item in value)
                {
                    sb.Append("<Email Value=\"").Append(System.Security.SecurityElement.Escape(item.Email))
                        .Append("\" Reason=\"").Append(System.Security.SecurityElement.Escape(item.Reason))
                        .Append("\" Provider=\"").Append(System.Security.SecurityElement.Escape(item.Provider))
                        .Append("\" />");
                }
                sb.Append("</Emails>");

                var sp = new InsertBlackEmailList();


                sp.Date = clock.Now;
                sp.Xml = sb.ToString();

                sp.ExecuteNonQuery();
                scope.Context.SaveChanges();
                scope.Complete();
            }

            ComHub.PublishBalckListEmails(new Messages.BalckListEmailsMessage
            {
                ArrayOfMails = value
                   .Select(p => new StringKeyValuePair
                   {
                       Email = p.Email,
                       Reason = p.Reason
                   })
                   .ToArray()
            });
        }

        #endregion

        #region SmsToSend

        public void InsertSmsToSend(IList<SmsToSend> sms)
        {
            using (var scope = Context.Write())
            {
                scope.Context.SmsToSend.AddRange(sms);
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public int GetTotalSmsNotSent()
        {
            using (var scope = Context.Read())
            {
                return scope.Context.SmsToSend
                    .Count(p => p.ExportedOn == null);
            }
        }

        private IList<Tuple<string, string, string>> GetSmsNotSent()
        {
            using (var scope = Context.Write())
            {
                //for the record i have no other option besides using a dataset to get the result from 
                //a stored procedure
                var sp = new GetSmsNotSent
                {
                    ExportedOn = clock.Now
                };
                var data = new System.Data.DataSet();

                sp.Fill(data);

                var results = data.Tables[0].Rows
                    .OfType<System.Data.DataRow>()
                    .Select(p => new Tuple<string, string, string>(p["PhoneNumber"].ToString(), p["Body"].ToString(), p["CustomerId"].ToString()))
                    .ToList();

                scope.Context.SaveChanges();
                scope.Complete();

                return results;
            }
        }

        public IList<Tuple<string, string, string>> GetExport(DateTime? exportedOn)
        {
            if (exportedOn.HasValue)
            {

                using (var scope = Context.Read())
                {
                    return scope.Context.SmsToSend
                        .Where(p => p.ExportedOn == exportedOn)
                        .Select(p => new
                        {
                            p.PhoneNumber,
                            p.Body, 
                            p.CustomerId
                        })
                        .ToList()
                        .Select(p => new Tuple<string, string, string>(p.PhoneNumber, p.Body, p.CustomerId))
                        .ToList();
                }
            }
            else
            {
                return this.GetSmsNotSent();
            }
        }

        public IList<Tuple<DateTime, int>> GetPreviousExports(int totalToReturn)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.SmsToSend
                    .Where(p => p.ExportedOn != null)
                    .GroupBy(p => p.ExportedOn)
                    .Select(p => new
                    {
                        Key = p.Key.Value,
                        Total = p.Count()
                    })
                    .OrderByDescending(p=> p.Key)
                    .Take(totalToReturn)
                    .ToList()
                    .Select(p => new Tuple<DateTime, int>(p.Key, p.Total))
                    .ToList();
            }
        }

        #endregion

        #region SmsUnsubcription

        public void InsertSmsUnsubcription(SmsUnsubcription value)
        {
            using (var scope = Context.Write())
            {
                scope.Context.Entry<SmsUnsubcription>(value).State = EntityState.Added;
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public void InsertSmsUnsubcription(CustomerPhoneNumbers value)
        {
            using (var scope = Context.Write())
            {
                var data = scope.Context.SmsUnsubcription
                    .Where(p => p.CustomerId == value.CustomerId)
                    .ToList();

                if (!value.Unsubscribe)
                {
                    scope.Context.SmsUnsubcription.RemoveRange(data);
                }
                else
                {
                    var currentPhones = data
                        .Select(p => p.PhoneNumber)
                        .ToList();

                    var deletedPhones = data
                        .Where(p => !value.PhoneNumbers.Contains(p.PhoneNumber))
                        .Select(p => p)
                        .ToList();

                    var newPhones = value.PhoneNumbers
                        .Except(currentPhones)
                        .Select(p => new SmsUnsubcription
                        {
                            CreatedOn = clock.Now,
                            CustomerId = value.CustomerId,
                            PhoneNumber = p
                        })
                        .ToList();

                    scope.Context.SmsUnsubcription.RemoveRange(deletedPhones);
                    scope.Context.SmsUnsubcription.AddRange(newPhones);
                }

                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public IList<SmsUnsubcription> GetSmsUnsubcription(IList<string> customerId)
        {
            using (var scope = Context.Read())
            {
                var query = scope.Context.SmsUnsubcription
                    .Select(p => p);

                if (customerId != null && customerId.Any())
                {
                    query = query.Where(p => customerId.Contains(p.CustomerId));
                }

                return query.ToList();
            }
        }

        public void UpdateSmsUnsubcription(SmsUnsubcription value)
        {
            using (var scope = Context.Write())
            {
                scope.Context.Entry<SmsUnsubcription>(value).State = EntityState.Modified;
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public void DeleteSmsUnsubcription(string customerId)
        {
            using (var scope = Context.Write())
            {
                var valueToDelete = new SmsUnsubcription
                {
                    CustomerId = customerId
                };

                scope.Context.SmsUnsubcription.Attach(valueToDelete);
                scope.Context.SmsUnsubcription.Remove(valueToDelete);
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        #endregion 

        public void UpdateCustomerInteraction(List<CustomerInteraction> customers)
        {
            using (var scope = Context.Write())
            {
                var ids = customers
                    .Select(p=> p.CustomerId)
                    .ToList();

                var currentData = scope.Context.CustomerInteraction
                    .Where(p => ids.Contains(p.CustomerId))
                    .ToList()
                    .ToDictionary(p=> p.CustomerId);
                
                foreach (var value in customers)
                {
                    
                /*}
                Parallel.ForEach<CustomerInteraction>(customers, value =>
                    {
                 */
                        CustomerInteraction current;

                        currentData.TryGetValue(value.CustomerId, out current);

                        if (current == null)
                        {
                            scope.Context.CustomerInteraction.Add(new CustomerInteraction
                            {
                                CustomerId = value.CustomerId,
                                LastEmailSentOn = value.LastEmailSentOn,
                                LastSmsSentOn = value.LastSmsSentOn
                            });
                        }
                        else
                        {
                            current.LastEmailSentOn = value.LastEmailSentOn ?? current.LastEmailSentOn;
                            current.LastSmsSentOn = value.LastSmsSentOn ?? current.LastSmsSentOn;
                        }
                    };//);

                scope.Context.SaveChanges();
                scope.Complete();
            }
        }
    }
}
