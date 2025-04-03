using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Merchandising.Helpers
{
    public class TagComparer : IEqualityComparer<HierarchyTag>
    {
        // Products are equal if their names and product numbers are equal.
        public bool Equals(HierarchyTag x, HierarchyTag y)
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
            return x.LevelId == y.LevelId && x.Id == y.Id;
        }

        // If Equals() returns true for a pair of objects 
        // then GetHashCode() must return the same value for these objects.

        public int GetHashCode(HierarchyTag tag)
        {
            //Check whether the object is null
            if (object.ReferenceEquals(tag, null))
            {
                return 0;
            }

            //Get hash code for the Name field if it is not null.
            int hashTagName = tag.Name == null ? 0 : tag.Name.GetHashCode();

            //Get hash code for the Id field.
            int hashTagId = tag.Id.GetHashCode();

            //Calculate the hash code for the Tag.
            return hashTagName ^ hashTagId;
        }
    }
}
