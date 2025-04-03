using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blue.Cosacs.Shared;
using Blue.Cosacs.Shared.Services.Financial;
using System.Data.SqlClient;

namespace Blue.Cosacs.Repositories
{
    public class FinancialRepository
    {
        public List<View_FintransPayMethod> GetPayments(string acctno, int agreementno)
        {
            using (var ctx = Context.Create())
            {
                return (from f in ctx.View_FintransPayMethod
                        where f.acctno == acctno &&
                        f.agrmtno == agreementno
                        select f).AnsiToList(ctx);
            }
        }

    }

 
}
