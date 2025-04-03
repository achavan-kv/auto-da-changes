using System;
namespace Blue.Cosacs.Credit.Repositories.Reindex
{
    public interface ICustomerSolrIndex
    {
        int Reindex();
        void Reindex(int[] customerIds);
    }
}
