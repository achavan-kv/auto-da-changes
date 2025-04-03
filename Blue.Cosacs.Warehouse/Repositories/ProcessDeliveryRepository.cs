using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.Warehouse.Repositories
{
    public class ProcessDeliveryRepository
    {
        public string IsMambuAccount(string AccountId)
        {
            string InvoiceNumber = string.Empty;
            using (var context = new ContextBase())
            {
                var conn = context.Database.Connection;
                var connectionState = conn.State;
                try
                {
                    if (connectionState != ConnectionState.Open) conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "select 1 from BSAccountMapping where CosacsAcctNo =  '" + AccountId +"'";
                        cmd.CommandType = CommandType.Text;
                        int? b = 0;
                        b = (int?)cmd.ExecuteScalar();
                        if (b==1)
                        {
                            cmd.CommandText = "select ISNULL(AgreementInvoicenumber,'') from Agreement where acctno ='" + AccountId +"'";
                            cmd.CommandType = CommandType.Text;
                            using (var result = cmd.ExecuteReader())
                            {
                                if (result.HasRows)
                                {
                                    while (result.Read())
                                    {
                                        InvoiceNumber = result.GetString(0);
                                    }
                                            
                                            
                                }

                            }
                        } 
                           
                        
                    }
                }
                catch (Exception ex)
                {
                    // error handling
                    throw;
                }
                finally
                {
                    if (connectionState != ConnectionState.Closed) conn.Close();
                }
                return InvoiceNumber;
            }

        }
    }

}
