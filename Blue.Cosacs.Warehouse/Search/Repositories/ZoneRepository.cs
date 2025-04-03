using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain = Blue.Cosacs.Warehouse;

namespace Blue.Cosacs.Warehouse.Repositories
{

    public class ZoneRepository
    {
        /// <summary>
        /// zoneRepository.Mapper("ItemNo", "SubClass", "Class", "Category")
        /// Make sure that the most important item (overriding item) is first.
        /// </summary>
        public IZoneMapper Mapper(short branchId, params string[] attributesInPrecedenceOrder)
        {
            using (var scope = Domain.Context.Read())
            {
                var assignments = (from za in scope.Context.ZoneAssignment
                                   join z in scope.Context.Zone on za.ZoneId equals z.Id
                                   where z.Branch == branchId
                                   select za).ToList();

                return new ZoneMapperDefault(assignments, attributesInPrecedenceOrder);
            };
        }


        public IZoneMapper GetMapper(short branchId)
        {
            using (var scope = Domain.Context.Read())
            {
                var attributesInPrecedenceOrder = (from code in scope.Context.ZonePriorityView
                                                   orderby code.sortorder
                                                   select code.code).ToArray();
                return Mapper(branchId, attributesInPrecedenceOrder);
            }
        }
   

    }
    /// <summary>
    /// This class does not go to the database.
    /// </summary>
    public class ZoneMapperDefault : IZoneMapper
    {
        private readonly ILookup<string, ZoneAssignment> assignments;
        private readonly string[] attributesInPrecedenceOrder;

        public ZoneMapperDefault(IEnumerable<ZoneAssignment> assignments, string[] attributesInPrecedenceOrder)
        {
            // turn into lookup to make the search by attribute name faster
            this.assignments = assignments.ToLookup(r => r.AttributeName);
            this.attributesInPrecedenceOrder = attributesInPrecedenceOrder;
        }

        #region IZoneMapper Members
        public int? Map(IDictionary<string, string> attributes)
        {
            foreach (var attributeName in attributesInPrecedenceOrder)
            {

                if (!attributes.ContainsKey(attributeName))
                    continue; // skip this attribute in the precedence list since this record does not have a value for it

                var entries = assignments[attributeName];
                var entry = entries.Where(e => e.AttributeValue == attributes[attributeName]).FirstOrDefault();

                if (entry != null)
                    return entry.ZoneId;
            }
            return null;
        }
        #endregion
    }

    public interface IZoneMapper
    {
        /// <summary>
        /// Returns the zone from the product attributes.
        /// Example:
        ///     Category = "TVs"
        ///     Class    = "LED TVs"
        ///     SubClass = "40\""
        ///     ItemNo   = "12121"
        ///     UPC      = "23-4829083290302"
        /// </summary>
        int? Map(IDictionary<string, string> attributes);
    }
}
