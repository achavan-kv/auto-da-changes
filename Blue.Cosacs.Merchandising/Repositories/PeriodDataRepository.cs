namespace Blue.Cosacs.Merchandising.Repositories
{
    using Blue.Cosacs.Merchandising.Event;
    using Blue.Cosacs.Merchandising.Models;
    using Blue.Events;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface IPeriodDataRepository
    {
        PeriodYear SaveData(PeriodYear data);

        List<PeriodYear> GetYears();
        
        bool AreUnique(PeriodYear periodData);

        List<PeriodData> GetCurrentAndPreviousPeriods();

        PeriodData GetPeriod(DateTime date);

        PeriodEndDatesView GetNextDate(DateTime today);

    }

    public class PeriodDataRepository : IPeriodDataRepository
    {
        private readonly IEventStore audit;

        public PeriodDataRepository(IEventStore audit)
        {
            this.audit = audit;
        }

        public PeriodYear SaveData(PeriodYear data)
        {
            using (var scope = Context.Write())
            {
                if (data.periods != null)
                {
                    // #14883
                    if (data.periods.Count() != 0)
                    {
                        // Delete all records then save
                        var oldPeriods = scope.Context.PeriodData.Where(p => p.year == data.year).ToList();
                        oldPeriods.ForEach(p => scope.Context.PeriodData.Remove(p));
                        scope.Context.SaveChanges();

                        data.periods.ToList().ForEach(p =>
                        {
                            p.StartDate = p.StartDate.Date;
                            p.EndDate = p.EndDate.Date;
                            var periodData = new PeriodData { year = data.year, period = p.PeriodNo, week = p.Week, startdate = p.StartDate, enddate = p.EndDate };
                            scope.Context.PeriodData.Add(periodData);
                        });
                    }
                }
                scope.Context.SaveChanges();

                // update the date dimension for the cube
                scope.Context.Database.ExecuteSqlCommand("[Merchandising].[ImportDateDimension]");
              
                this.audit.LogAsync(data, EventType.CreatePeriodData, EventCategories.Merchandising);
                scope.Complete();

            }
            return data;
        }

        public List<PeriodYear> GetYears()
        {
            //If we are before April still in the previous Financial Year
            var year = DateTime.Now.Month < 4 ? DateTime.Now.Year - 2 : DateTime.Now.Year - 1;

            var data = new List<PeriodYear>();

            for (var i = 1; i <= 3; i++)
            {
                List<Period> periods;
                using (var scope = Context.Read())
                {
                    periods =
                        scope.Context.PeriodData.Where(y => y.year == year)
                            .ToList()
                            .Select(
                                y =>
                                new Period { PeriodNo = y.period, Week = y.week, StartDate = (y.startdate ?? DateTime.Now).Date, EndDate = (y.enddate ?? DateTime.Now.Date).Date })
                            .OrderBy(y => y.PeriodNo)
                            .ThenBy(y => y.Week)
                            .ToList();
                }

                data.Add(new PeriodYear { year = Convert.ToInt16(year), periods = periods.ToArray() });
                year++;
            }

            return data;
        }

        public PeriodData GetPeriod(DateTime date)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.PeriodData.FirstOrDefault(p => p.enddate.Value == date);
            }
        }

        public List<PeriodData> GetCurrentAndPreviousPeriods()
        {
            using (var scope = Context.Read())
            {
                var today = DateTime.UtcNow.Date;
                
                var allPeriods = scope.Context.PeriodData
                    .GroupBy(p => new { p.period, p.year })
                    .Select(g => g.OrderByDescending(p => p.enddate).FirstOrDefault())
                    .OrderByDescending(p => p.enddate).ToList();

                var previousPeriods = allPeriods.Where(p => p.enddate < today).ToList();
                var thisPeriod = allPeriods.Where(p => p.enddate >= today).OrderBy(p => p.enddate).FirstOrDefault();
                previousPeriods.Add(thisPeriod);
                return previousPeriods.OrderByDescending(p => p.enddate).ToList();
            }
        }

        public bool AreUnique(PeriodYear periodData)
        {
            var toReturn = true;

            if (periodData.periods != null)
            {
                if (periodData.periods.Count() != 0)
                {
                    var duplicated = periodData.periods.ToList().GroupBy(p => new { Period = p.PeriodNo, p.Week }).Where(grp => grp.Count() > 1).Select(grp => grp.Key);

                    if (duplicated.Any())
                    {
                        toReturn = false;
                    }
                }
            }

            return toReturn;
        }

        private static class EventType
        {
            public const string CreatePeriodData = "ReplacePeriodData";
        }

        public PeriodEndDatesView GetNextDate(DateTime today)
        {
            using (var scope = Context.Read())
            {
                var enddate = scope.Context.PeriodEndDatesView.FirstOrDefault(d => d.EndDate == today);
                if (enddate == null || !enddate.EndDate.HasValue)
                {
                    //If today is not EOM, get next EOM
                    enddate = scope.Context.PeriodEndDatesView.Where(d => d.EndDate >= today).OrderBy(d => d.EndDate).FirstOrDefault();
                }

                if (enddate == null)
                {
                    throw new Exception("Period Data doesn't exist");
                }
                return enddate;
            }
        }
    }
}