using Blue.Cosacs.Merchandising.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Merchandising.Repositories
{
    public interface ICintErrorRepository
    {
        void SaveError(List<CintError> model);
        void SaveBulkError(CintsError model);
        void MarkResolved(int messageId);
        IQueryable<CintsError> SearchBulk(CintErrorQueryModel model);
        IQueryable<CintError> Search(CintErrorQueryModel model);
        Tuple<string, string> ExportBulk(CintErrorQueryModel model);
        Tuple<string, string> Export(CintErrorQueryModel model);
    }

    public class CintErrorRepository : ICintErrorRepository
    {
        private readonly IClock clock;

        public CintErrorRepository(IClock clock)
        {
            this.clock = clock;
        }

        public void SaveError(List<CintError> model)
        {
            using (var scope = Context.Write())
            {
                var ids = model.Select(s => s.MessageId);
                // Get old cint errors
                var delCints = scope.Context.CintError.Where(c => ids.Contains(c.MessageId));
                if (delCints.Any())
                {
                    // Update current date with old date if exists
                    var oldDates = delCints.Select(s => new { s.MessageId, s.Date }).ToDictionary(d => d.MessageId, d => d.Date);
                    model.ForEach(m =>
                    {
                        if (oldDates.ContainsKey(m.MessageId))
                        {
                            m.Date = oldDates[m.MessageId];
                        }
                    });
                    scope.Context.CintError.RemoveRange(delCints);
                    scope.Context.SaveChanges();
                }
                scope.Context.CintError.AddRange(model);
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public void SaveBulkError(CintsError model)
        {
            using (var scope = Context.Write())
            {
                var delCints = scope.Context.CintsError.Where(c => c.MessageId == model.MessageId);
                if (delCints.Any())
                {
                    model.CreatedOn = delCints.Min(c => c.CreatedOn);
                    scope.Context.CintsError.RemoveRange(delCints);
                    scope.Context.SaveChanges();
                }
                scope.Context.CintsError.Add(model);
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public void MarkResolved(int messageId)
        {
            using (var scope = Context.Write())
            {
                var error = scope.Context.CintError.Find(messageId);
                if (error != null)
                {
                    error.Resolved = clock.Now;
                    scope.Context.SaveChanges();
                }
                scope.Complete();
            }
        }

        // Queue 200 - Single entry.
        public IQueryable<CintError> Search(CintErrorQueryModel model)
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

                if (model.Filter)
                {
                    query = query.Where(q => !q.Resolved.HasValue);
                }

                return query.OrderByDescending(v => v.MessageId);
            }
        }

        // Queue 201 - Bulk cints. 
        public IQueryable<CintsError> SearchBulk(CintErrorQueryModel model)
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
                return query.OrderByDescending(v => v.MessageId);
            }
        }

        public Tuple<string, string> ExportBulk(CintErrorQueryModel model)
        {
            var results = SearchBulk(model).ToList();
            var csv = new StringBuilder();
            csv.AppendLine("MessageId, Runno, CreatedOn, Exception");
            results.ForEach(s =>
            {
                csv.AppendLine(string.Format("{0}, {1}, {2}, {3}", s.MessageId, s.Runno, s.CreatedOn, s.Exception));
            });
            return new Tuple<string, string>("Cint_Errors_bulk_" + clock.Now.ToString("yyyy-MM-dd") + ".csv", csv.ToString());
        }

        public Tuple<string, string> Export(CintErrorQueryModel model)
        {
            var results = Search(model).ToList();
            var csv = new StringBuilder();
            csv.AppendLine("Date, Runno, MessageId, Product Code, Primary Reference, Type, Stock Location, Sale Location, Error, ResovledOn");
            results.ForEach(s =>
            {
                csv.AppendLine(string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}", s.Date, s.RunNo, s.MessageId, s.ProductCode, s.PrimaryReference, s.Type, s.StockLocation, s.SaleLocation, s.ErrorMessage, s.Resolved));
            });
            return new Tuple<string, string>("Cint_Errors_validation_" + clock.Now.ToString("yyyy-MM-dd") + ".csv", csv.ToString());
        }
    }
}