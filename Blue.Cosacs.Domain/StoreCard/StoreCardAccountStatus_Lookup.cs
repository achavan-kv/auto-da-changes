using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared
{
    public sealed class StoreCardAccountStatus_Lookup
    {
        private StoreCardAccountStatus_Lookup(string code, string description)
        {
            this.Code = code;
            this.Description = description;
        }
           

        public string Code { get; private set; }
        public string Description { get; private set; }

        // These status haven't been validated. Still subject to revision
        public static readonly StoreCardAccountStatus_Lookup Active = new StoreCardAccountStatus_Lookup("A", "Active");
        public static readonly StoreCardAccountStatus_Lookup AwaitingActivation = new StoreCardAccountStatus_Lookup("AA", "Awaiting Activation");
        public static readonly StoreCardAccountStatus_Lookup Cancelled = new StoreCardAccountStatus_Lookup("C", "Cancelled");
        public static readonly StoreCardAccountStatus_Lookup CardToBeIssued = new StoreCardAccountStatus_Lookup("TBI", "Card To Be Issued");
        public static readonly StoreCardAccountStatus_Lookup Suspended = new StoreCardAccountStatus_Lookup("S", "Offer Expired");
        public static readonly StoreCardAccountStatus_Lookup Blocked = new StoreCardAccountStatus_Lookup("B", "Blocked");
        public static readonly StoreCardAccountStatus_Lookup Unknown = new StoreCardAccountStatus_Lookup("U", "");


        public override bool Equals(object obj)
        {
            if (obj is StoreCardAccountStatus_Lookup)
                return this.Code == ((StoreCardAccountStatus_Lookup)obj).Code;
            else if (obj is string) 
                return this.Code == (((string)obj).Trim().ToUpper());
            else return false;
        }

        public override int GetHashCode()
        {
            return Code.GetHashCode();
        }

        public static StoreCardAccountStatus_Lookup FromString(string code)
        {
           
                foreach (var status in AsEnumerable())
                {
                    if (status.Equals(code))
                        return status;
                }
                return StoreCardAccountStatus_Lookup.Unknown;
            
        } 

        public static IEnumerable<StoreCardAccountStatus_Lookup> AsEnumerable()
        {
            yield return Unknown;
            yield return Active;
            yield return AwaitingActivation;
            yield return Cancelled;
            yield return CardToBeIssued;
            yield return Blocked;
            yield return Suspended;
        }
    }

   
}
