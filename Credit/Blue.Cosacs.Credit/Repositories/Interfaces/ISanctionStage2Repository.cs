using System;
using Base = Blue.Cosacs.Credit.Model.SanctionStage2;
namespace Blue.Cosacs.Credit.Repositories
{
    public interface ISanctionStage2Repository
    {
        Blue.Cosacs.Credit.Model.SanctionStage2.SanctionStage2 GetApplicant1Details(int id, Blue.IClock clock);
        Blue.Cosacs.Credit.Model.SanctionStage2.SanctionStage2 GetApplicant2Details(int id);
        void SaveSanctionStage2Applicant1(int id, Base.SanctionStage2 proposal, bool isApplicant2, int updatedBy, DateTime? updatedOn);
        void SaveSanctionStage2Applicant2(int id, Base.SanctionStage2 proposal, bool isApplicant2, int updatedBy, DateTime? updatedOn);
    }
}
