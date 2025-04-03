using System;
using System.Collections.Generic;
namespace Blue.Cosacs.Credit.Repositories
{
    public interface ITermsTypeRepository
    {
        Blue.Cosacs.Credit.Model.TermsTypeDetails Get(int id);
        void Save(Blue.Cosacs.Credit.Model.TermsTypeDetails termsType, int updatedBy, DateTime updatedOn);
        List<Blue.Cosacs.Credit.Model.TermsTypeSimulator> Search(Blue.Cosacs.Credit.Model.TermsTypeSearch search);
    }
}
