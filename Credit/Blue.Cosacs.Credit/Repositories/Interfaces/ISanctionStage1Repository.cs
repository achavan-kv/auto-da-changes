using System;
using Base = Blue.Cosacs.Credit.Model.SanctionStage1;
namespace Blue.Cosacs.Credit.Repositories
{
    public interface ISanctionStage1Repository
    {
        Blue.Cosacs.Credit.Model.SanctionStage1.SanctionStage1 GetApplicant1Details(int id);
        Blue.Cosacs.Credit.Model.SanctionStage1.SanctionStage1 GetApplicant2Details(int id);
        void SaveSanctionStage1Applicant1(int id, Base.SanctionStage1 proposal, int updatedBy, DateTime? updatedOn);
        void SaveSanctionStage1Applicant2(int id, Base.SanctionStage1 proposal, int updatedBy, DateTime? updatedOn);
    }
}
