using Blue.Cosacs.Credit.Repositories.Interfaces;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace Blue.Cosacs.Credit.Repositories
{
    public class ScoringBandMatrixRepository : IScoringBandMatrixRepository
    {
        public void SaveScoringBandMatrix(ScoringBandMatrix scoringBandMatrix)
        {
            using (var scope = Context.Write())
            {
                var result = scope.Context.ScoringBandMatrix.Where(p => p.Id == scoringBandMatrix.Id).FirstOrDefault();

                if (result == null)
                {
                    scope.Context.ScoringBandMatrix.Add(scoringBandMatrix);
                }
                else
                {
                    result.PointsFrom = scoringBandMatrix.PointsFrom;
                    result.ScoreCard = scoringBandMatrix.ScoreCard;
                    result.PointsTo = scoringBandMatrix.PointsTo;
                    result.Band = scoringBandMatrix.Band;
                    result.CreatedOn = scoringBandMatrix.CreatedOn;
                    result.CreatedBy = scoringBandMatrix.CreatedBy;
                }
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public List<ScoringBandMatrix> Get()
        {
            using (var scope = Context.Read())
            {
                return scope.Context.ScoringBandMatrix.ToList();
            }
        }

        public void Delete(int id)
        {
            using (var scope = Context.Write())
            {
                var scoringBandMatrix = scope.Context.ScoringBandMatrix.Find(id);
                if (scoringBandMatrix != null)
                {
                    scope.Context.ScoringBandMatrix.Remove(scoringBandMatrix);
                    scope.Context.SaveChanges();
                    scope.Complete();
                }
            }
        }
    }
}
