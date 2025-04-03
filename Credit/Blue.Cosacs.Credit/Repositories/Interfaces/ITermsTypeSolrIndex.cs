using System;
namespace Blue.Cosacs.Credit.Repositories.Reindex
{
    public interface ITermsTypeSolrIndex
    {
        int Reindex();
        void Reindex(int[] termsTypeIds);
    }
}
