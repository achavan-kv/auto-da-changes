using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Merchandising.Helpers
{
    public class LevelComparer : IEqualityComparer<HierarchyLevel>
    {
        // Products are equal if their names and product numbers are equal.
        public bool Equals(HierarchyLevel x, HierarchyLevel y)
        {
            //Check whether the compared objects reference the same data.
            if (object.ReferenceEquals(x, y))
            {
                return true;
            }

            //Check whether any of the compared objects is null.
            if (object.ReferenceEquals(x, null) || object.ReferenceEquals(y, null))
            {
                return false;
            }

            //Check whether the products' properties are equal.
            return x.Id == y.Id && x.Id == y.Id;
        }

        // If Equals() returns true for a pair of objects 
        // then GetHashCode() must return the same value for these objects.

        public int GetHashCode(HierarchyLevel level)
        {
            //Check whether the object is null
            if (object.ReferenceEquals(level, null))
            {
                return 0;
            }

            //Get hash code for the Name field if it is not null.
            int hashLevelName = level.Name == null ? 0 : level.Name.GetHashCode();

            //Get hash code for the Id field.
            int hashTagId = level.Id.GetHashCode();

            //Calculate the hash code for the Tag.
            return hashLevelName ^ hashTagId;
        }
    }
}
