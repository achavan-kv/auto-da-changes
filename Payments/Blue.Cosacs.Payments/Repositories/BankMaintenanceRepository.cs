using System.Collections.Generic;
using System.Linq;

namespace Blue.Cosacs.Payments
{
    public class BankMaintenanceRepository
    {
        public Bank SaveBank(Bank bank)
        {
            using (var scope = Context.Write())
            {
                var existingBank = (from b in scope.Context.Bank
                                    where b.Id == bank.Id
                                    select b).FirstOrDefault();

                if (existingBank != null)
                {
                    existingBank.BankName = bank.BankName;
                    existingBank.BankCode = bank.BankCode;
                    existingBank.Active = bank.Active;
                }
                else
                {
                    var newBank = scope.Context.Bank.Add(new Bank
                    {
                        BankName = bank.BankName,
                        BankCode = bank.BankCode,
                        Active = bank.Active
                    });

                    bank.Id = newBank.Id;
                }

                scope.Context.SaveChanges();
                scope.Complete();
            }

            return bank;
        }

        public List<Bank> GetBanks(bool getActiveBanks = false)
        {
            List<Bank> banks = null;

            using (var scope = Context.Read())
            {
                if (getActiveBanks)
                {
                    banks = (from b in scope.Context.Bank
                             where b.Active == true
                             select b).ToList();
                }
                else
                {
                    banks = (from b in scope.Context.Bank
                             select b).ToList();
                }
            }

            return banks;
        }

    }
}
