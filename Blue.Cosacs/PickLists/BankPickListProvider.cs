using System.Linq;

namespace Blue.Cosacs.PickLists
{
    public class BankPickListProvider : LinqPickListProvider
    {
        public BankPickListProvider() :
            base("Bank",
            "Bank Names",
            () =>
            {
                using (var ctx = Context.Create())
                {
                    return (from b in ctx.Bank
                            select new
                            {
                                b.bankcode, 
                                b.bankname
                            }).ToList()
                        .Select(b => new PickListRow(b.bankcode, b.bankname)).ToArray();
                }
            }){ }
    }
}
