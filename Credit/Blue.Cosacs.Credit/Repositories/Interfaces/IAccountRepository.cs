namespace Blue.Cosacs.Credit.Repositories
{
   public interface IAccountRepository
    {
        void CreateAccountFromProposal(int proposalId, int customerId);
    }
}
