using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Globalization;

namespace Blue.Cosacs.Warranty.Solr
{
    public class SolrIndex
    {
        private static ICosacsTaxSettings _settings;
        public const string SolrType = "Warranty";

        public static void IndexWarranty(ICosacsTaxSettings settings,int[] warrantyIds = null)
        {
            _settings = settings;

            using (var scope = Context.Read())
            {
                var war = warrantyIds == null ? scope.Context.Warranty.ToList() : scope.Context.Warranty.Where(u => warrantyIds.Contains(u.Id)).ToList();
                var warIds = war.Select(w => w.Id);

                var tags = (from wt in scope.Context.WarrantyTags
                            join l in scope.Context.Level on wt.LevelId equals l.Id
                            join t in scope.Context.Tag on wt.TagId equals t.Id into q
                            from joinedTags in q.DefaultIfEmpty()
                            where warIds.Contains(wt.WarrantyId)
                            select new { wt, joinedTags, l }).ToList();

                var priceLocations = (from wc in scope.Context.WarrantyPrice
                                      where warIds.Contains(wc.WarrantyId)
                                      select new { wc.WarrantyId, wc.BranchType, wc.BranchNumber }).ToList();
                var priceLocationsLookup = priceLocations.ToLookup(p => p.WarrantyId);


                var warranties = new List<dynamic>();
                war.ForEach(w =>
                {
                    dynamic newWarranty = new ExpandoObject();
                    newWarranty.Id = string.Format("{0}:{1}", SolrType, w.Id); ;
                    newWarranty.Type = SolrType;
                    newWarranty.WarrantyId = w.Id.ToString();
                    newWarranty.WarrantyNumber = w.Number;
                    newWarranty.ItemDescription = w.Description;
                    newWarranty.Length = w.Length;
                    newWarranty.TaxRate = w.TaxRate.HasValue ? w.TaxRate.Value : settings.TaxRate;
                    newWarranty.WarrantyType = WarrantyType.GetNameForType(w.TypeCode);  // w.TypeCode == "F" ? "Yes" : "No";
                    newWarranty.Deleted = w.Deleted ? "Yes" : "No";
                    tags.Where(t => t.wt.WarrantyId == w.Id).ToList().ForEach(t =>
                    {
                        ((IDictionary<string, object>)newWarranty)["Level_" + t.l.Id] = t.joinedTags == null ? "Unassigned" : t.joinedTags.Name;
                    });

                    if (priceLocationsLookup.Contains(w.Id))
                    {
                        newWarranty.WarrantyHasPrices = "Yes";
                        newWarranty.BranchType = (from pl in priceLocationsLookup[w.Id]
                                                  select pl.BranchType == null ? "All" : StoreType.GetNameForType(pl.BranchType)
                                                  ).ToArray();
                        newWarranty.BranchNumber = (from pl in priceLocationsLookup[w.Id]
                                                    select pl.BranchNumber.HasValue ? pl.BranchNumber.Value.ToString(CultureInfo.InvariantCulture) : "All"
                                                    ).ToArray();

                    }
                    else
                    {
                        newWarranty.WarrantyHasPrices = "No";
                    }

                    warranties.Add(newWarranty);
                });
                new Blue.Solr.WebClient().Update(warranties);
            }
        }

    }
}