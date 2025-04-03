using Blue.Cosacs.Credit.Constants;

namespace Blue.Cosacs.Credit.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        public void CreateAccountFromProposal(int proposalId, int customerId)
        {
            using (var scope = Context.Write())
            {
                var branch = scope.Context.Proposal.Find(proposalId).Branch.ToString();
                var next = HiLoMananger.Get(HiLoTypes.Account).NextId();
                var accountNumber = GenerateCardNumber(branch, next);
                
                scope.Context.Account.Add(new Account()
                {
                    AccountNumber = accountNumber,
                    CustomerId = customerId
                });

                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        private static long GenerateCardNumber(string prefix, int nextNumber)
        {
            var newcardnum = prefix + nextNumber.ToString().PadLeft(15 - prefix.Length, '0');
            var plus = 0;
            var sum = 0;
            var multi = 2;

            for (var i = newcardnum.Length - 1; i >= 0; i--)
            {
                plus = multi * int.Parse(newcardnum[i].ToString());
                multi = 3 - multi;
                sum += plus / 10 + plus % 10;
            }

            var validchar = sum % 10 == 0 ? "0" : (10 - sum % 10).ToString();
            return long.Parse(newcardnum + validchar);
        }
    }
}
