using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Financial
{
    public class TransactionTypeListProvider : LinqPickListProvider
    {
        public TransactionTypeListProvider()
            : base("TransactionType", "All transaction types (from the transtype table) used for broker configurations.", () =>
            {
                using (var scope = Financial.Context.Read())
                {
                    return (from r in scope.Context.TransactionTypeView
                            orderby r.Name
                            select new { code = r.Id, Description = r.Name }).ToList()
                            .Select(r => new PickListRow(r.code.ToString(), r.Description)).ToArray();

                }
            }) { }
    }
}
