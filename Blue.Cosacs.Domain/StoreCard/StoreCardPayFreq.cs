using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared
{
    public sealed class StoreCardFreq_Lookup
    {
        private StoreCardFreq_Lookup(string code, string description)
        {
            this.Code = code;
            this.Description = description;
        }
           

        public string Code { get; private set; }
        public string Description { get; private set; }

        // These status haven't been validated. Still subject to revision
        public static readonly StoreCardFreq_Lookup None = new StoreCardFreq_Lookup("N", "None");
        public static readonly StoreCardFreq_Lookup Monthly = new StoreCardFreq_Lookup("M", "Monthly");
        public static readonly StoreCardFreq_Lookup Quarterly = new StoreCardFreq_Lookup("Q", "Quarterly");
        public static readonly StoreCardFreq_Lookup BiMonthly = new StoreCardFreq_Lookup("B", "Bi-Monthly");
        public static readonly StoreCardFreq_Lookup SixMonths = new StoreCardFreq_Lookup("E","Every Six Months");
   
        public override bool Equals(object obj)
        {
            if (obj is StoreCardFreq_Lookup)
                return this.Code == ((StoreCardFreq_Lookup)obj).Code;
            else if (obj is string) 
                return this.Code == ((string)obj);
            else if (obj == null)
                throw new ArgumentNullException("obj");
            return false;
        }

        public override int GetHashCode()
        {
            return Code.GetHashCode();
        }

        public static StoreCardFreq_Lookup FromString(string code)
        {
            foreach (var status in AsEnumerable())
            {
                if (status.Equals(code))
                    return status;
            }

            throw new InvalidCastException(String.Format("Cannot cast {0} to StoreCardStatus", code));
        }

        public static IEnumerable<StoreCardFreq_Lookup> AsEnumerable()
        {
            yield return None;
            yield return Monthly;
            yield return Quarterly;
            yield return BiMonthly;
            yield return SixMonths;
        }

        public static List<StoreCardFreq_Lookup> AsList()
        {
            return new List<StoreCardFreq_Lookup>(AsEnumerable());
        }
    }
}
