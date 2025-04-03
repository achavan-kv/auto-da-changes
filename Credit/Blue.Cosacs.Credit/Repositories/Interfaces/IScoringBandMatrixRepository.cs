using System.Collections.Generic;

namespace Blue.Cosacs.Credit.Repositories.Interfaces
{
    public interface IScoringBandMatrixRepository
    {
        void SaveScoringBandMatrix(ScoringBandMatrix scoringBandMatrix);
        List<ScoringBandMatrix> Get();
        void Delete(int id);
    }
}
