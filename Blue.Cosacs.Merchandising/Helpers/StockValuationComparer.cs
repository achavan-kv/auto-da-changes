using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Merchandising.Helpers
{
    public class StockValuationComparer : IEqualityComparer<StockValuationSnapshot>
    {
        // objects are equal if their names and product numbers are equal.
        public bool Equals(StockValuationSnapshot x, StockValuationSnapshot y)
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

            //Check whether the objects' properties are equal.
            return x.ProductId == y.ProductId && x.LocationId == y.LocationId && x.SnapshotDateId == y.SnapshotDateId;
        }

        // If Equals() returns true for a pair of objects 
        // then GetHashCode() must return the same value for these objects.

        public int GetHashCode(StockValuationSnapshot stock)
        {
            //Check whether the object is null
            if (object.ReferenceEquals(stock, null))
            {
                return 0;
            }

            //Get hash code for the ProductId field if it is not null.
            int hashProductId = stock.ProductId == null ? 0 : stock.ProductId.GetHashCode();

            //Get hash code for the LocationId field.
            int hashLocationId = stock.LocationId.GetHashCode();

            //Get hash code for the LocationId field.
            int hashDate = stock.SnapshotDateId.GetHashCode();

            //Calculate the hash code for the StockValuation.
            return hashProductId ^ hashLocationId ^ hashDate;
        }
    }
}