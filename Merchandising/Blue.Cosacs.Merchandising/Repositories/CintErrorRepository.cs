using Blue.Cosacs.Merchandising.Models;
using System.Collections.Generic;
using System.Linq;

namespace Blue.Cosacs.Merchandising.Repositories
{
    public interface ICintErrorRepository
    {
        void SaveError(List<CintError> model);
        void SaveBulkError(CintsError model);
        IEnumerable<CintsError> SearchBulk(CintErrorQueryModel model);
        IEnumerable<CintError> Search(CintErrorQueryModel model);
    }

    public class CintErrorRepository : ICintErrorRepository
    {
        public void SaveError(List<CintError> model)
        {
            using (var scope = Context.Write())
            {
                model.ForEach(m =>
                {
                    scope.Context.Entry(new CintError { MessageId = m.MessageId }).State = System.Data.Entity.EntityState.Deleted;
                });
                scope.Context.SaveChanges();
                scope.Context.CintError.AddRange(model);
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public void SaveBulkError(CintsError model)
        {
            using (var scope = Context.Write())
            {
                scope.Context.Entry(new CintsError { MessageId = model.MessageId }).State = System.Data.Entity.EntityState.Deleted;
                scope.Context.SaveChanges();
                scope.Context.CintsError.Add(model);
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public IEnumerable<CintError> Search(CintErrorQueryModel model)
        {
            using (var scope = Context.Read())
            {
                var query = scope.Context.CintError.AsNoTracking().AsQueryable();

                if (model.RunNo.HasValue)
                {
                    query = query.Where(q => q.RunNo == model.RunNo);
                }

                if (!string.IsNullOrEmpty(model.PrimaryReference))
                {
                    query = query.Where(q => q.PrimaryReference == model.PrimaryReference);
                }

                if (model.FromDate.HasValue)
                {
                    var d = model.FromDate.Value.ToUniversalTime();
                    query = query.Where(q => q.Date >= d);
                }

                if (model.ToDate.HasValue)
                {
                    var d = model.ToDate.Value.AddDays(1).ToUniversalTime();
                    query = query.Where(q => q.Date < d);
                }

                return query.OrderByDescending(v => v.Id).Take(500).ToList();
            }
        }

        public IEnumerable<CintsError> SearchBulk(CintErrorQueryModel model)
        {
            using (var scope = Context.Read())
            {
                var query = scope.Context.CintsError.AsNoTracking().AsQueryable();

                if (model.RunNo.HasValue)
                {
                    query = query.Where(q => q.Runno == model.RunNo);
                }

                if (model.FromDate.HasValue)
                {
                    var d = model.FromDate.Value.ToUniversalTime();
                    query = query.Where(q => q.CreatedOn >= d);
                }

                if (model.ToDate.HasValue)
                {
                    var d = model.ToDate.Value.AddDays(1).ToUniversalTime();
                    query = query.Where(q => q.CreatedOn < d);
                }

                var count = query.Count();
                return query.OrderByDescending(v => v.MessageId).Take(500).ToList();
            }
        }
    }
}